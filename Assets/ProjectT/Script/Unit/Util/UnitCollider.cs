
using UnityEngine;


[RequireComponent(typeof(CapsuleCollider))]
public class UnitCollider : MonoBehaviour
{
    public float            CompareDistScore    { get; set; }
    public float            CompareDistAngle    { get; set; }
    public int              HitCount            { get; set; }
    public CapsuleCollider  HitCollider         { get; private set; }
    public Unit             Owner               { get; private set; }

    // CapsuleCollider에 있는 프로퍼티들
    public float    radius      { get { return HitCollider.radius; } }
    public float    height      { get { return HitCollider.height; } }
    public Bounds   bounds      { get { return HitCollider.bounds; } }
    public int      direction   { get { return HitCollider.direction; } }

    private Rigidbody mRigidBody = null;

    private float   mOriginRadius = 0.0f;
	private float   mOriginHeight = 0.0f;

    public void Init()
    {
        HitCollider = GetComponent<CapsuleCollider>();
        if (HitCollider == null)
        {
            Debug.LogError("CapsuleCollider 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        Owner = Utility.GetUnitByCollider(HitCollider);
        if (Owner == null)
        {
            Debug.LogError("Unit 컴포넌트를 찾을 수 없습니다.");
        }

        HitCount = 0;
        mRigidBody = GetComponent<Rigidbody>();

		mOriginRadius = HitCollider.radius;
        mOriginHeight = HitCollider.height;
	}

    public bool IsEnable()
    {
        if(HitCollider == null)
        {
            return false;
        }

        return HitCollider.enabled;
    }

    public void Enable(bool enable)
    {
        if (HitCollider == null)
        {
            return;
        }

        HitCollider.enabled = enable;
    }

    public void SetRadius(float radius)
    {
        if (HitCollider == null)
        {
            return;
        }

        HitCollider.radius = radius;
    }

    public void SetHeight(float height)
    {
        if (HitCollider == null)
        {
            return;
        }

        HitCollider.height = height;
    }

    public void SetTrigger(bool isTrigger)
    {
        if (HitCollider == null)
        {
            return;
        }

        HitCollider.isTrigger = isTrigger;
    }

    public void Restore() {
		HitCollider.radius = mOriginRadius;
		HitCollider.height = mOriginHeight;
	}

    public Vector3 GetCenterPos()
    {
        if (HitCollider == null)
        {
            Debug.LogError(Owner.name + "의 HitCollider가 null입니다.");
            return Vector3.zero;
        }

        return HitCollider.transform.TransformPoint(HitCollider.center);
    }

    private void Awake()
    {
        if(HitCollider == null)
        {
            Init();
        }
    }

    private void SetZeroVelocity(Collision col, bool isKinematic)
    {
        if(object.ReferenceEquals(mRigidBody, null))
        {
            return;
        }

        mRigidBody.velocity = Vector3.zero;

        Unit unit = col.gameObject.GetComponent<Unit>();
        if (unit && unit.rigidBody)
        {
            unit.rigidBody.velocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        SetZeroVelocity(col, true);
    }

    private void OnCollisionExit(Collision col)
    {
        SetZeroVelocity(col, false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Owner == null || Owner.transform == null || HitCollider == null)
        {
            return;
        }

        DrawCollider(GetCenterPos(), new Vector3(HitCollider.radius, HitCollider.height * 0.5f, HitCollider.radius), Owner.transform.rotation);
    }

    private void DrawCollider(Vector3 pos, Vector3 size, Quaternion q)
    {
        Color cGizmo = Gizmos.color;
        Matrix4x4 matGizmo = Gizmos.matrix;

        Matrix4x4 mat = Matrix4x4.TRS(pos, q, size);

        Gizmos.color = Color.green;
        Gizmos.matrix *= mat;
        Gizmos.DrawWireSphere(Vector3.zero, 1.0f);

        Gizmos.color = cGizmo;
        Gizmos.matrix = matGizmo;

        Player p = Owner as Player;
        if( p && !p.IsHelper && HitCollider.enabled == false ) {
            int a = 0;
            a = 1;
		}
    }
#endif
}
