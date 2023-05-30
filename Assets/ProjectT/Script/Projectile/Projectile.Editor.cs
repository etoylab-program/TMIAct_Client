
#if UNITY_EDITOR

using UnityEngine;


public partial class Projectile : MonoBehaviour
{
    private AniEvent m_aniEvent;
    private AniEvent.sProjectileInfo m_projectileInfo;

    private AniEvent.sParticleSimulateInfo m_psiMain;

    private Vector3 m_startPos;
    private Vector3 m_destPos;
    private Vector3 m_prepareDir;
    private Vector3 m_targetDir;
    private Vector3 m_p;
    private Vector3 m_v;

    private float m_speed;
    private float m_prevTime = 0.0f;


    public void EDInit(AniEvent aniEvent, AniEvent.sProjectileInfo projectileInfo)
    {
        m_aniEvent = aniEvent;
        m_projectileInfo = projectileInfo;

        Init();

        m_psiMain = null;

        ParticleSystem ps = null;
        if (main != null)
        {
            ps = main.GetComponent<ParticleSystem>();
            if (ps == null)
                ps = main.GetComponentInChildren<ParticleSystem>();

            if (ps != null)
            {
                m_psiMain = new AniEvent.sParticleSimulateInfo();
                m_psiMain.ps = ps;

                m_originalPosMain = main.transform.position;
                m_originalRotMain = main.transform.eulerAngles;
            }
            else
                m_aniMain = main.GetComponent<Animation>();
        }
    }

    public void Show(bool show)
    {
        if (main == null)
            return;

        m_time = 0.0f;
        m_t = 0.0f;

        m_startPos = Vector3.zero;
        m_destPos = Vector3.zero;
        m_bonePos = Vector3.zero;

        main.SetActive(show);
    }

    public void EDSetPos(Vector3 ownerPos, Vector3 stepForwardPos, Vector3 pushedPos)
    {
        if (m_projectileInfo.projectile == null)
            return;

        Vector3 forward = Vector3.forward;
        Vector3 right = Vector3.right;
        Vector3 up = Vector3.up;

        if (m_projectileInfo.selectedBoneIndex > 0)
        {
            Transform bone = m_aniEvent.GetBone(m_projectileInfo.selectedBoneIndex);
            if (bone != null)
            {
                if(m_projectileInfo.attach)
                    m_bonePos = bone.transform.position + stepForwardPos + pushedPos;
                else if(m_bonePos == Vector3.zero)
                    m_bonePos = bone.transform.position + stepForwardPos + pushedPos;

                //forward = bone.forward;
                //right = bone.transform.right;
                //up = bone.transform.up;
            }
        }

        if(m_projectileInfo.useAddRotOnFire)
        {
            Quaternion qR = Quaternion.AngleAxis(m_projectileInfo.addedRotOnFire.x, right);
            Quaternion qU = Quaternion.AngleAxis(m_projectileInfo.addedRotOnFire.y, up);
            forward = (qR * qU) * forward;
        }

        m_startPos = ownerPos + m_projectileInfo.addedPosition;

        m_destPos = ownerPos + forward * 10.0f;
        if(type == eType.Forward)
        {
            m_destPos = ownerPos;
            m_destPos.x = main.transform.position.x;
            m_destPos.y = main.transform.position.y;
            m_destPos += forward * 10.0f;
        }

        m_p = Vector3.Lerp(m_startPos, m_destPos, 0.65f);
        m_p += Vector3.up * value1;

        transform.rotation = Quaternion.Euler(m_projectileInfo.addedRotation);
    }

    public void EDUpdate(float time, float deltaTime, Vector3 stepForwardPos, Vector3 pushedPos, float startFrame, float sampleFrame)
    {
        switch(type)
        {
            case eType.Forward:
                EDUpdateForward(time);
                break;

            case eType.Parabola:
                EDUpdateParabola(time);
                break;

            case eType.Zigzag:
                EDUpdateZigzag(time, startFrame, sampleFrame);
                break;

            case eType.Homing:
            case eType.TargetListHoming:
                EDUpdateHoming(time);
                break;

            case eType.Effect:
                EDUpdateEffect(time, startFrame, sampleFrame);
                break;
        }
    }

    private void EDUpdateForward(float time)
    {
        m_time += (time - m_prevTime) / duration;
        m_t = Mathf.SmoothStep(0.0f, 1.0f, m_time);
        if (m_t < 1.0f && main.gameObject.activeSelf == false)
            Show(true);

        m_pos.x = EasingFunction.GetEasingFunction(ease)(m_startPos.x + m_bonePos.x, m_destPos.x, m_t);
        m_pos.y = EasingFunction.GetEasingFunction(ease)(m_startPos.y + m_bonePos.y, m_destPos.y, m_t);
        m_pos.z = EasingFunction.GetEasingFunction(ease)(m_startPos.z + m_bonePos.z, m_destPos.z, m_t);

        transform.position = m_pos;

        if (m_psiMain != null)
            EDSimulateParticle(m_psiMain, time, m_pos);

        m_prevTime = time;
    }

    private void EDUpdateParabola(float time)
    {
        m_time += (time - m_prevTime) / duration;
        m_t = Mathf.SmoothStep(0.0f, 1.0f, m_time);
        if (m_t < 1.0f && main.gameObject.activeSelf == false)
            Show(true);

        //m_pos = Utility.Parabola(m_startPos + m_bonePos, m_destPos, value1, m_t);
        m_pos = Utility.Bezier(m_startPos + m_bonePos, m_p, m_destPos, m_t);
        transform.position = m_pos;

        transform.LookAt(m_pos);

        if (m_psiMain != null)
            EDSimulateParticle(m_psiMain, time, m_pos);

        m_prevTime = time;
    }

    private void EDUpdateZigzag(float time, float startFrame, float sampleFrame)
    {
        m_time += (time - m_prevTime) / duration;
        m_t = Mathf.SmoothStep(1.0f, 0.0f, m_time);
        if (m_t > 0.0f && main.gameObject.activeSelf == false)
        {
            m_prepareDir = (-Vector3.forward + Vector3.up + (transform.right * (float)Random.Range(-1, 2))).normalized * value2;
            m_speed = value1;

            Show(true);
        }

        m_targetDir = (m_destPos - m_startPos).normalized;
        m_v = (m_prepareDir * m_speed * m_t) + (m_targetDir * m_speed * (1.0f - m_t));

        m_speed = EasingFunction.GetEasingFunction(ease)(value1, value1 * accel, 1.0f - m_t);

        float frame = Mathf.Max(0.0f, sampleFrame - startFrame);
        if (m_t > 0.0f)
        {
            m_pos = (m_startPos + m_bonePos) + ((m_v * frame) * Time.fixedDeltaTime);
            transform.position = m_pos;

            if (m_psiMain != null)
                EDSimulateParticle(m_psiMain, time, m_pos);

            //Quaternion q = Quaternion.LookRotation(m_v.normalized);
            //transform.rotation = q;
        }

        m_prevTime = time;
    }

	private void EDUpdateHoming( float time ) {
		if ( duration <= 0.0f ) {
			return;
		}

		m_time += ( time - m_prevTime ) / duration;

		m_t = Mathf.SmoothStep( 0.0f, 1.0f, m_time );
        if ( m_t < 1.0f && main.gameObject.activeSelf == false ) {
            Show( true );
        }

		m_pos.x = EasingFunction.GetEasingFunction( ease )( m_startPos.x + m_bonePos.x, m_destPos.x, m_t );
		m_pos.y = EasingFunction.GetEasingFunction( ease )( m_startPos.y + m_bonePos.y, m_destPos.y, m_t );
		m_pos.z = EasingFunction.GetEasingFunction( ease )( m_startPos.z + m_bonePos.z, m_destPos.z, m_t );

		transform.position = m_pos;

        if ( m_psiMain != null ) {
            EDSimulateParticle( m_psiMain, time, m_pos );
        }

		m_prevTime = time;
	}

	private void EDUpdateEffect(float time, float startFrame, float sampleFrame)
    {
        if (main.activeSelf == false)
            Show(true);

        if (m_psiMain != null)
            EDSimulateParticle(m_psiMain, time, (m_startPos + m_bonePos) + m_originalPosMain);
        else if (m_aniMain != null)
        {
            m_aniMain.clip.SampleAnimation(m_aniMain.gameObject, time);
            transform.position = m_destPos;
        }
    }

    private void EDSimulateParticle(AniEvent.sParticleSimulateInfo psi, float time, Vector3 pos)
    {
        if (psi == null || psi.ps.gameObject.activeSelf == false)
            return;

        psi.simulateTime = time;

        if (psi.simulateTime != psi.beforeSimulateTime || m_projectileInfo.selectedBoneIndex != m_projectileInfo.beforeSelectedBoneIndex)
            psi.ps.Simulate(psi.simulateTime);

        m_projectileInfo.beforeSelectedBoneIndex = m_projectileInfo.selectedBoneIndex;
        psi.beforeSimulateTime = psi.simulateTime;

        psi.ps.transform.position = pos;
    }
}

#endif
