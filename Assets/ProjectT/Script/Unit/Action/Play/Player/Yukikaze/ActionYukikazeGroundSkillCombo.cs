
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionYukikazeGroundSkillCombo : ActionSelectSkillBase
{
    public enum eState
    {
        Start = 0,
        Doing,
        End,
    }


    private State m_state = new State();
    private eAnimation[] m_aniCombo = null;
    private int m_curComboAni = 0;
    private bool m_nextComboAni = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.GroundSkillCombo;

        m_aniCombo = new eAnimation[4];
        m_aniCombo[0] = eAnimation.GroundSkillCombo01;
        m_aniCombo[1] = eAnimation.GroundSkillCombo02;
        m_aniCombo[2] = eAnimation.GroundSkillCombo03;
        m_aniCombo[3] = eAnimation.GroundSkillCombo04;

        conditionActionCommand = new eActionCommand[1];
        conditionActionCommand[0] = eActionCommand.Attack01;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Grounded;

        extraCancelCondition = new eActionCondition[3];
        extraCancelCondition[0] = eActionCondition.UseSkill;
        extraCancelCondition[1] = eActionCondition.UseQTE;
        extraCancelCondition[2] = eActionCondition.UseUSkill;

		m_state.Init(3);
        m_state.Bind(eState.Start, ChangeStartState);
        m_state.Bind(eState.Doing, ChangeDoingState);
        m_state.Bind(eState.End, ChangeEndState);

        for (int i = 0; i < m_aniCombo.Length; i++)
        {
            List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent(m_aniCombo[i]);
            for (int j = 0; j < listAtkEvt.Count; j++)
                listAtkEvt[j].atkRatio += listAtkEvt[j].atkRatio * (mValue1 / (float)eCOUNT.MAX_BO_FUNC_VALUE);
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        m_state.ChangeState(eState.Start, true);

        float dmgDownRatio = mValue1 / (float)eCOUNT.MAX_BO_FUNC_VALUE;
        m_owner.unitBuffStats.AddBuffStat(eBuffDebuffType.Buff, m_data.ID * 10, UnitBuffStats.eBuffStatType.Damage, UnitBuffStats.eIncreaseType.Decrease,
                                          dmgDownRatio, -1, BattleOption.eToExecuteType.Unit, BattleOption.eBOConditionType.None, BattleOption.eBOConditionType.None,
                                          BattleOption.eBOAtkConditionType.None, TableId, false);
    }

    public override IEnumerator UpdateAction()
    {
        UnitCollider targetCollider = null;
        if (FSaveData.Instance.AutoTargetingSkill)
        {
            targetCollider = m_owner.GetMainTargetCollider(true);
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            if (targetCollider)
            {
                m_owner.LookAtTarget(targetCollider.GetCenterPos());
            }

            m_checkTime += m_owner.fixedDeltaTime;
            switch ((eState)m_state.current)
            {
                case eState.Start:
                    if (m_checkTime >= m_aniLength)
                    {
                        if(m_nextComboAni)
                            m_state.ChangeState(eState.Doing, true);
                        else
                            m_state.ChangeState(eState.End, true);
                    }
                    break;

                case eState.Doing:
                    if (m_checkTime >= m_aniLength)
                    {
                        ++m_curComboAni;
                        if (!m_nextComboAni || m_curComboAni >= m_aniCombo.Length)
                            m_state.ChangeState(eState.End, true);
                        else
                            m_state.ChangeState(eState.Doing, true);
                    }
                    break;

                case eState.End:
                    if (m_owner.aniEvent.IsAniPlaying(eAnimation.FinishGroundSkillCombo) == eAniPlayingState.End)
                        m_endUpdate = true;
                    break;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        if ((eState)m_state.current == eState.End)
            return;

        if(!FSaveData.Instance.AutoTargetingSkill)
        {
            m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
        }

        m_nextComboAni = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        m_owner.unitBuffStats.RemoveBuffStat(m_data.ID * 10);
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_owner.unitBuffStats.RemoveBuffStat(m_data.ID * 10);
    }

    private bool ChangeStartState(bool changeAni)
    {
        m_checkTime = 0.0f;
        m_curComboAni = 0;
        m_nextComboAni = false;

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.PrepareGroundSkillCombo);
        return true;
    }

    private bool ChangeDoingState(bool changeAni)
    {
        m_checkTime = 0.0f;
        m_nextComboAni = false;

        m_aniLength = m_owner.PlayAniImmediate(m_aniCombo[m_curComboAni]);
        return true;
    }

    private bool ChangeEndState(bool changeAni)
    {
        m_checkTime = 0.0f;
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.FinishGroundSkillCombo);

        return true;
    }
}
