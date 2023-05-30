
using UnityEngine;
using System.Collections;


public class CmptRotateByDirection : CmptBase
{
	public float t { get { return mT; } }

	private Quaternion	mBeforeRot			= Quaternion.identity;
    private float		mT					= 0.0f;
    private float		mLerpSpeed			= 0.7f;
	WaitForFixedUpdate	mWaitForFixedUpdate	= new WaitForFixedUpdate();

	
    public void Init(Vector3 rot)
    {
        mBeforeRot = Quaternion.Euler(rot);
    }

    public void UpdateRotation(Vector3 dir, bool rotImmediate, float lerpSpeed = 1.0f)
    {
        if (dir == Vector3.zero)
            return;

        //Quaternion q1 = Quaternion.Euler(mBeforeRot);
        Quaternion q2 = Quaternion.LookRotation(dir.normalized);

        // 플레이어는 y축 회전만 함
        if(gameObject.layer == (int)eLayer.Player)
        {
            q2.x = q2.z = 0.0f;
        }

        if (rotImmediate == false)
        {
            float angle = Quaternion.Angle(mBeforeRot, q2);
            if (angle <= 1.0f)
                mT = 0.0f;
            else
                mT += m_owner.fixedDeltaTime * lerpSpeed;
        }
        else
            mT = 1.0f;

        if (m_owner.rigidBody == null || m_owner.rigidBody.isKinematic == true)
        {
            transform.rotation = Quaternion.Lerp(mBeforeRot, q2, mT);
            mBeforeRot = m_owner.transform.rotation;
        }
        else
        {
            m_owner.rigidBody.MoveRotation(Quaternion.Lerp(mBeforeRot, q2, mT));
            mBeforeRot = m_owner.rigidBody.rotation;
        }
    }

    public void LookAtTarget(Vector3 pos, float lerpSpeed = 0.7f)
    {
        mLerpSpeed = lerpSpeed;

        StopCoroutine("UpdateLookAt");
        StartCoroutine("UpdateLookAt", pos);
    }

    private IEnumerator UpdateLookAt(Vector3 pos)
    {
        Vector3 targetPos = pos;
        targetPos.y = transform.position.y;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (mT < 1.0f)
        {
            UpdateRotation(targetPos - transform.position, false, mLerpSpeed);
            yield return mWaitForFixedUpdate;
        }
    }
}
