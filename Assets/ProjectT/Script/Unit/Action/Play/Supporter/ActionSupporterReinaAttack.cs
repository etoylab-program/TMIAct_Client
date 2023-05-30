
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterReinaAttack : ActionSupporterSkillBase
{
    private Projectile                  mPjt        = new Projectile();
    private AniEvent.sEvent             mAniEvt     = null;
    private AniEvent.sProjectileInfo    mPjtInfo    = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterReinaAttack;

        mPjt = GameSupport.CreateProjectile("Projectile/pjt_supporter_109.prefab");
        mPjt.OnHitFunc = OnProjectileHit;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParamFromBO = param as ActionParamFromBO;

        UnitCollider targetCollider = mOwnerPlayer.GetMainTargetCollider(true);
        if (targetCollider && targetCollider.Owner)
        {
            if (mPjtInfo == null)
            {
                mPjtInfo = mOwnerPlayer.aniEvent.CreateProjectileInfo(mPjt);
                mPjtInfo.addedPosition = mOwnerPlayer.GetCenterPos() - transform.position;
                mPjtInfo.notAniEventAtk = true;
            }

            if (mAniEvt == null)
            {
                mAniEvt = mOwnerPlayer.aniEvent.CreateEvent(eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f,
                                                            mParamFromBO.battleOptionData.value);
            }

            mPjt.Fire(mOwnerPlayer, BattleOption.eToExecuteType.Supporter, mAniEvt, mPjtInfo, targetCollider.Owner, -1);
        }

        m_endUpdate = true;
    }

    private bool OnProjectileHit(Unit hitTarget)
    {
        if (hitTarget == null)
        {
            return false;
        }

        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
        mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Common;
        mBuffEvt.Set(TableId, eEventSubject.Self, eEventType.EVENT_DEBUFF_FREEZE, hitTarget, 0.0f, 0.0f, 0.0f,
                     mParamFromBO.battleOptionData.duration, 0.0f, 0, 0, eBuffIconType.Debuff_Freeze);

        EventMgr.Instance.SendEvent(mBuffEvt);
        return true;
    }
}
