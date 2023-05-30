
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionWpnYukikazeStackParalysis : ActionWeaponSkillBase
{
    private int mStack = 0;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.WpnYukikazeStackParalysis;

        SkipShowNames = true;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        ActionParamFromBO paramFromBO = param as ActionParamFromBO;
        mBuffEvt.CompareUnit = null;

        int maxStack = (int)(paramFromBO.battleOptionData.value * (float)eCOUNT.MAX_BO_FUNC_VALUE);

        ++mStack;
        if (mStack >= maxStack)
        {
            mStack = 0;

            float value1 = paramFromBO.battleOptionData.value2 * (float)eCOUNT.MAX_BO_FUNC_VALUE; // 범위
            float value2 = paramFromBO.battleOptionData.value3; // 인원

            mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
            mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
            mBuffEvt.Set(TableId, eEventSubject.ActiveEnemiesInRange, eEventType.EVENT_DEBUFF_SPEED_DOWN_AND_SKIP_CUR_ANI, m_owner, 10.0f,
                         value1, value2, paramFromBO.battleOptionData.duration, 0.0f, 20063, mBuffEvt.effId2, eBuffIconType.Debuff_Speed);

            Unit target = m_owner.listHitTarget.Count > 0 ? m_owner.listHitTarget[0] : null;//World.Instance.EnemyMgr.GetNearestTarget(m_owner, m_owner.listHitTarget);
            mBuffEvt.CompareUnit = target;

            EventMgr.Instance.SendEvent(mBuffEvt);
        }

        if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
        {
            World.Instance.UIPlay.UpdateWeaponReferenceBattleOption(mStack);
        }
    }
}
