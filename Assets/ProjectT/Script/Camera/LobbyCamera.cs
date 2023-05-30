
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


[RequireComponent(typeof(Rigidbody))]
public class LobbyCamera : MonoBehaviour
{
    public enum eType
    {
        None = 0,
        Normal,
        FixedTarget,
        LookAtTarget,
        Joystick,
    }


    [Header("[Constraints]")]
    public float MIN_ROT_X = -80.0f;
    public float MAX_ROT_X = 80.0f;
    public float MIN_FOV = 20.0f;
    public float MAX_FOV = 80.0f;
    public float DEFAULT_FOV = 55.0f;

    [Header("[Look at Target]")]
    public Vector3 distance = new Vector3(0.0f, 1.0f, -2.0f);
    public float lookAtY = 0.5f;

    public Rigidbody RigidBody { get; private set; }

    private eType	m_type		= eType.None;
    private Camera	m_camera	= null;
    
    private VirtualJoystick m_joystick = null;
    private GameObject m_target = null;

    private Quaternion m_qOriginal = Quaternion.identity;
    private Vector3 m_beforePos = Vector3.zero;
    private Vector3 m_lookAtPoint = Vector3.zero;

    private float moveSpeed = 3.0f;
    private float rotateSpeed = 5.0f;

    private float m_originalMoveSpeed = 0.0f;
    private List<Material> m_listTransparentObject = new List<Material>();

    private PostProcessLayer    mPostProcessLayer   = null;
    private int                 mCullingMask        = 0;

	private bool mbLock = false;

    public Camera camera { get { return m_camera; } }
    public eType type { get { return m_type; } }


    public void EnablePostProcess(bool enable)
    {
        if(mPostProcessLayer == null)
        {
            return;
        }

        mPostProcessLayer.enabled = enable;
    }

    private void Awake()
    {
        m_camera = GetComponent<Camera>();
        RigidBody = GetComponent<Rigidbody>();

        m_qOriginal = transform.rotation;
        m_originalMoveSpeed = moveSpeed;

        InitCameraTransform();
        m_listTransparentObject.Clear();

        mPostProcessLayer = GetComponent<PostProcessLayer>();

        if ( m_camera != null ) {
			mCullingMask = m_camera.cullingMask;
        }
    }

    public void InitCameraTransform()
    {
        transform.position = distance;
        transform.rotation = m_qOriginal;

        m_camera.fieldOfView = DEFAULT_FOV;
        //mAddDelta = 0.0f;
    }

    public void EnableCamera(bool enable)
    {
        if ( !enable ) {
			m_camera.cullingMask = 0;
		}
        else {
            m_camera.cullingMask = mCullingMask;
		}
    }

    public void SetCameraType(eType type, Transform target, bool keepPoint = true)
    {
		m_type = type;
    
        if (target)
            m_target = target.gameObject;

        if (type == eType.FixedTarget || type == eType.Joystick)
        {
            if (target)
            {
                transform.position = target.position;
                transform.rotation = target.rotation;
            }
        }
        else if (type == eType.LookAtTarget)
        {
            if (target)
            {
                if (keepPoint)
                {
                    transform.position = m_target.transform.position + distance;

                    m_lookAtPoint = m_target.transform.position;
                    m_lookAtPoint.y += lookAtY;

                    transform.LookAt(m_lookAtPoint);
                }
                else
                {
                    //m_lookAtPoint = m_target.transform.position;
                    //m_lookAtPoint.y += lookAtY;
                }
            }
        }
    }

	public void LockCamera(bool isLock)
	{
		mbLock = isLock;
	}

    public void SetJoystick(VirtualJoystick joystick)
    {
        m_joystick = joystick;

        if (m_joystick)
        {
            m_joystick.Init();
        }
    }

    private void Update()
    {
        if (mbLock || m_type == eType.FixedTarget || AppMgr.Instance.CustomInput.IsOverUI())
        {
            return;
        }

        if (Mathf.Abs(AppMgr.Instance.CustomInput.Delta) > 0.1f)
        {
            Vector3 v = transform.TransformDirection(Vector3.forward) * AppMgr.Instance.CustomInput.Delta * Time.fixedDeltaTime;
            Vector3 pos = CheckCollisionWall(RigidBody.position + v);
            RigidBody.MovePosition(pos);

            return;
        }

        if (m_type == eType.Joystick)
        {
            if (m_joystick == null)
            {
                return;
            }

            if (m_joystick.rawDir != Vector3.zero)
            {
                Vector3 dir = Utility.Get3DDirFrom2DDir(m_joystick.rawDir, false);
                Vector3 v = transform.TransformDirection(dir) * moveSpeed * Time.fixedDeltaTime;

				Vector3 newPos = CheckCollisionWall(RigidBody.position + v);
				RigidBody.MovePosition(newPos);
            }
        }
        else if(m_type != eType.FixedTarget)
        {
            if (AppMgr.Instance.CustomInput.MultiTouchDeltaPos != Vector3.zero)
            {
                Vector3 v1 = -transform.right * AppMgr.Instance.CustomInput.MultiTouchDeltaPos.x;
                Vector3 v2 = -transform.up * AppMgr.Instance.CustomInput.MultiTouchDeltaPos.y;

				Vector3 newPos = CheckCollisionWall(RigidBody.position + ((v1 + v2) * 0.5f * Time.deltaTime));
				RigidBody.MovePosition(newPos);
            }
        }

        if (AppMgr.Instance.CustomInput.DeltaPos != Vector3.zero)
        {
            if (m_type != eType.LookAtTarget)
            {
                transform.RotateAround(transform.position, Vector3.up, AppMgr.Instance.CustomInput.DeltaPos.x * (rotateSpeed * Time.deltaTime));
                transform.RotateAround(transform.position, transform.right, -AppMgr.Instance.CustomInput.DeltaPos.y * (rotateSpeed * Time.deltaTime));
            }
            else
            {
                if (m_target != null)
                {
                    Vector3 v = transform.position - m_beforePos;

                    transform.RotateAround(m_target.transform.position - v, Vector3.up, AppMgr.Instance.CustomInput.DeltaPos.x * (rotateSpeed * Time.deltaTime));
                    transform.RotateAround(m_target.transform.position - v, transform.right, -AppMgr.Instance.CustomInput.DeltaPos.y * (rotateSpeed * Time.deltaTime));
                }

                m_beforePos = RigidBody.position;
            }
        }
    }

    public void MoveVertical(bool up)
    {
        Vector3 v;
        if (up)
        {
            v = Vector3.up * moveSpeed * Time.fixedDeltaTime;
        }
        else
        {
            v = -Vector3.up * moveSpeed * Time.fixedDeltaTime;
        }

        RigidBody.MovePosition(RigidBody.position + v);
    }

    public void SpeedUp(float ratio)
    {
        moveSpeed *= ratio;
    }

    public void ResetSpeed()
    {
        moveSpeed = m_originalMoveSpeed;
    }

	private Vector3 CheckCollisionWall(Vector3 nextPos)
	{
		Vector3 pos = nextPos;

		if (Physics.Linecast(transform.position, nextPos, out RaycastHit hitInfo, 1 << (int)eLayer.Wall))
		{
			pos = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
		}

		return pos;
	}
}
