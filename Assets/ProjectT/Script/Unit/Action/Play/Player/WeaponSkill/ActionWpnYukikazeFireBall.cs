
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionWpnYukikazeFireBall : ActionWeaponSkillBase
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.WpnYukikazeFireBall;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        superArmor = Unit.eSuperArmor.Invincible;
        mCurAni = eAnimation.YukikazeFireBall;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        ActionParamFromBO paramFromBO = param as ActionParamFromBO;

        List<AniEvent.sEvent> listAtkEvent = m_owner.aniEvent.GetAllAttackEvent(mCurAni);
        for (int i = 0; i < listAtkEvent.Count; i++)
            listAtkEvent[i].atkRatio *= paramFromBO.battleOptionData.value;

        m_aniLength = m_owner.PlayAniImmediate(mCurAni);

        UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
        if (targetCollider)
        {
            LookAtTarget(targetCollider.Owner);
            //World.Instance.InGameCamera.TurnToTarget(targetCollider.GetCenterPos(), 2.0f);
        }
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            //if (m_owner.aniEvent.IsAniPlaying(m_curAni) == eAniPlayingState.End)
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= m_aniLength)
                m_endUpdate = true;

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
