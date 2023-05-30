
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionUpperAttack : ActionSelectSkillBase
{
    protected eAnimation m_curAni = eAnimation.None;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        actionCommand = eActionCommand.AttackDuringAttack;
        IsRepeatSkill = true;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        superArmor = Unit.eSuperArmor.Lv1;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        ShowSkillNames(m_data);

        m_curAni = GetCurAni();

        m_aniLength = m_owner.PlayAniImmediate(m_curAni);
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(m_curAni);

        if (FSaveData.Instance.AutoTargeting)
        {
            UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
            if (targetCollider != null)
            {
                LookAtTarget(targetCollider.Owner);
            }
        }

        //World.Instance.InGameCamera.SetDefaultMode(false, 7.5f);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            if (m_aniCutFrameLength > 0.0f)
            {
                m_checkTime += m_owner.fixedDeltaTime;
                if(m_checkTime >= m_aniCutFrameLength)
                {
                    m_endUpdate = true;
                    //m_owner.SetMainTarget(null);

                    ToUpperJump();
                }
            }
            else if (m_owner.aniEvent.IsAniPlaying(m_curAni) == eAniPlayingState.End)
                m_endUpdate = true;

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.UpperAttack01);
        if (evt == null)
        {
            return 0.0f;
        }

        return evt.visionRange;
    }

    protected virtual eAnimation GetCurAni()
    {
        return eAnimation.UpperAttack01;
    }

    protected virtual void ToUpperJump()
    {
        ActionParamUpperJump paramUpperJump = new ActionParamUpperJump(m_owner.cmptJump.m_jumpPower, ActionParamUpperJump.eCheckFallingType.Animation, false, this);
        SetNextAction(eActionCommand.UpperJump, paramUpperJump);
    }
}
