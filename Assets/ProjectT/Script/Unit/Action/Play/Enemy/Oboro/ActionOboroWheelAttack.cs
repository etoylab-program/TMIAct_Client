using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionOboroWheelAttack : ActionEnemyAttackBase
{
    public enum eWheelState
    {
        Ready,
        WheelAttack,
        Finish,
    }

    private UnitCollider mTargetCollider = null;
    private int m_checkRotateNum = 0;
    private State m_state = new State();

    public int rotateCnt = 3;

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.WheelAttack;

		m_state.Init(3);
        m_state.Bind(eWheelState.Ready, ChangeWheelAttackReady);
        m_state.Bind(eWheelState.WheelAttack, ChangeWheelAttack);
        m_state.Bind(eWheelState.Finish, ChangeWheelAttackFinish);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        m_state.ChangeState(eWheelState.Ready, true);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            switch((eWheelState)m_state.current)
            {
                case eWheelState.Ready:
                    {
                        m_checkTime += m_owner.fixedDeltaTime;
                        if (m_checkTime >= m_aniLength)
                            m_state.ChangeState(eWheelState.WheelAttack, true);
                    }
                    break;
                case eWheelState.WheelAttack:
                    {
                        m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
                        m_checkTime += m_owner.fixedDeltaTime;
                        if (m_checkTime >= m_aniLength)
                        {
                            if(m_checkRotateNum < rotateCnt)
                                m_state.ChangeState(eWheelState.WheelAttack, true);
                            else
                                m_state.ChangeState(eWheelState.Finish, true);
                        }
                    }
                    break;
                case eWheelState.Finish:
                    {
                        m_checkTime += m_owner.fixedDeltaTime;
                        if (m_checkTime >= m_aniLength)
                            m_endUpdate = true;
                    }
                    break;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public bool ChangeWheelAttackReady(bool changeAni)
    {
        //World.Instance.playerCam.SetAlwaysLookAtBoss(false);
        m_checkRotateNum = 0;
        m_checkTime = 0.0f;
        m_aniLength = m_owner.PlayAni(eAnimation.Attack02);
        return true;
    }

    public bool ChangeWheelAttack(bool changeAni)
    {
        Log.Show(m_checkRotateNum + " / " + rotateCnt);
        m_checkRotateNum++;
        m_checkTime = 0.0f;
        m_aniLength = m_owner.PlayAni(eAnimation.Attack03);
        return true;
    }

    public bool ChangeWheelAttackFinish(bool changeAni)
    {
        m_checkTime = 0.0f;
        m_aniLength = m_owner.PlayAni(eAnimation.Attack04);
        return true;
    }
}
