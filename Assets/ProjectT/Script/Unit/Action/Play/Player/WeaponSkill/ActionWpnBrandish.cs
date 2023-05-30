
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionWpnBrandish : ActionWeaponSkillBase
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.WpnBrandish;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        superArmor = Unit.eSuperArmor.Invincible;
        mCurAni = eAnimation.Brandish;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        ActionParamFromBO paramFromBO = param as ActionParamFromBO;

        List<AniEvent.sEvent> listAtkEvent = m_owner.aniEvent.GetAllAttackEvent(mCurAni);
        for (int i = 0; i < listAtkEvent.Count; i++)
            listAtkEvent[i].atkRatio *= paramFromBO.battleOptionData.value;

        m_aniLength = m_owner.PlayAniImmediate(mCurAni);
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(mCurAni);

        UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
        if (targetCollider != null)
            LookAtTarget(targetCollider.Owner);
    }

    public override IEnumerator UpdateAction()
    {
        float checkTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            checkTime += m_owner.fixedDeltaTime;
            if (checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);

        Debug.Log("End of ActionWpnBrandish");
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_owner.StopStepForward();

        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);
    }
}
