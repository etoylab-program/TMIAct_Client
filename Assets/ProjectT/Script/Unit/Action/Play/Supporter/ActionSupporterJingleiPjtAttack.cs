
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterJingleiPjtAttack : ActionSupporterSkillBase
{
    private Projectile					mPjt            = null;
	private AniEvent.sProjectileInfo	mPjtInfo		= null;
	private UnitCollider				mTargetCollider = null;
	private AniEvent.sEvent				mAniEvt			= null;


	public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterJingleiPjtAttack;

        mPjt = GameSupport.CreateProjectile("Projectile/pjt_supporter_jinglei.prefab");
        mPjt.OnHitFunc = OnProjectileHit;

		
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
			mPjtInfo = mOwnerPlayer.aniEvent.CreateProjectileInfo(mPjt);
		}

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider)
        {
			mPjtInfo.followParentRot = true;
			mPjtInfo.addedPosition = mOwnerPlayer.GetCenterPos() - transform.position;
			mPjtInfo.notAniEventAtk = true;

			mAniEvt.atkRatio = mParamFromBO.battleOptionData.value;

			mPjt.Fire(mOwnerPlayer, BattleOption.eToExecuteType.Supporter, mAniEvt, mPjtInfo, mTargetCollider.Owner, -1);
        }
    }

    private bool OnProjectileHit(Unit hitTarget)
    {
        if(hitTarget == null)
        {
            return false;
        }

        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
        mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.None;

        mBuffEvt.Set(0, eEventSubject.Self, eEventType.EVENT_DEBUFF_BREAK_SHIELD, hitTarget, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                     mBuffEvt.effId, mBuffEvt.effId2, eBuffIconType.None);

        EventMgr.Instance.SendEvent(mBuffEvt);
        return true;
    }
}
