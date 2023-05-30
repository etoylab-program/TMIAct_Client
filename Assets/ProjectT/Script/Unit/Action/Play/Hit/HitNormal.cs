
using UnityEngine;
using System.Collections;


public class HitNormal : ActionHitBase
{
    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        Collider[] cols = Physics.OverlapSphere(m_owner.transform.position, 0.01f, 1 << (int)eLayer.Floor | 1 << (int)eLayer.EnvObject);
        if (m_owner.isGrounded == false)
        {
            if (cols.Length > 0) // 점프가 시작하는 찰나의 그 순간 맞으면 아직 점프는 안한걸로 간주하고 RigidBody세팅을 그라운드로 강제로 맞춤
                m_owner.SetGroundedRigidBody();
            else
                m_owner.SetFallingRigidBody();
        }

        if (m_hitAni == eAnimation.None)
            m_hitAni = eAnimation.Hit;

        if (!m_owner.aniEvent.HasAni(m_hitAni))
            m_endUpdate = true;
        else
        {
            m_aniLength = m_owner.PlayAniImmediate(m_hitAni);

            StopCoroutine("StartCheckPauseFrame");
            StartCoroutine("StartCheckPauseFrame");
        }
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            //CheckPauseFrame();

            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= m_aniLength)//m_owner.aniEvent.IsAniPlaying(m_hitAni) == eAniPlayingState.End)
            {
                m_endUpdate = true;
            }
            /*else if(m_checkTime >= m_aniLength * GameInfo.Instance.BattleConfig.StartPauseFrameHitLengthRatio)
                CheckPauseFrame();*/

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        /*if (m_owner.curHp <= 0.0f && !m_owner.isLockDie)
        {
            m_owner.actionSystem.CancelCurrentAction();
            m_owner.CommandAction(eActionCommand.Die, null);
        }
        else*/
            OnStart(param);
    }
}
