
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterKiraraIce : ActionSupporterSkillBase
{
    private Projectile					mProjectile     = null;
	private AniEvent.sProjectileInfo	mPjtInfo		= null;
	private float						mFreezeDuration = 5.0f;
	private AniEvent.sEvent				mAniEvt			= null;
    

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterKiraraIce;

        mProjectile = GameSupport.CreateProjectile("Projectile/pjt_supporter_56.prefab");
        mProjectile.OnHitFunc = OnProjectileHit;
	}

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParamFromBO = param as ActionParamFromBO;

		if(mAniEvt == null)
		{
			mAniEvt = mOwnerPlayer.aniEvent.CreateEvent(eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f);
		}

		if(mPjtInfo == null)
		{
			mPjtInfo = mOwnerPlayer.aniEvent.CreateProjectileInfo(mProjectile);
		}

        Unit target = m_owner.mainTarget == null ? World.Instance.EnemyMgr.GetNearestTarget(m_owner, true) : m_owner.mainTarget;
        if (target)
        {
			mPjtInfo.addedPosition = mOwnerPlayer.GetCenterPos() - transform.position;
			mPjtInfo.notAniEventAtk = true;

			mAniEvt.atkRatio = mParamFromBO.battleOptionData.value;

			mProjectile.Fire(mOwnerPlayer, BattleOption.eToExecuteType.Supporter, mAniEvt, mPjtInfo, target, -1);
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
        mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.None;

        mBuffEvt.Set(0, eEventSubject.Self, eEventType.EVENT_DEBUFF_FREEZE, hitTarget, 0.0f, 0.0f, 0.0f, mFreezeDuration, 0.0f,
                     mBuffEvt.effId, mBuffEvt.effId2, eBuffIconType.Debuff_Freeze);

        EventMgr.Instance.SendEvent(mBuffEvt);
        return true;
    }
}
