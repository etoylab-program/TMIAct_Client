
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterSawawashiAttack : ActionSupporterSkillBase
{
    private Projectile					mPjt            = null;
	private AniEvent.sProjectileInfo	mPjtInfo		= null;
	private float						mAttackPower    = 0.0f;
	private AniEvent.sEvent				mAniEvt			= null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterSawawashiAttack;

        mPjt = GameSupport.CreateProjectile("Projectile/pjt_supporter_63.prefab");
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

        mAttackPower = m_owner.attackPower * mParamFromBO.battleOptionData.value;

        UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
        if (targetCollider)
        {
			mPjtInfo.followParentRot = true;
			mPjtInfo.addedPosition = mOwnerPlayer.GetCenterPos() - transform.position;
			mPjtInfo.notAniEventAtk = true;

			mAniEvt.atkRatio = mParamFromBO.battleOptionData.value;

			mPjt.Fire(mOwnerPlayer, BattleOption.eToExecuteType.Supporter, mAniEvt, mPjtInfo, targetCollider.Owner, -1);
        }
    }

    private bool OnProjectileHit(Unit hitTarget)
    {
        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
        mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.None;

        mBuffEvt.Set(0, eEventSubject.HitTargetList, eEventType.EVENT_DEBUFF_DOT_DMG, mOwnerPlayer, mParamFromBO.battleOptionData.value2, 
                     (float)EAttackAttr.POISON, 0.0f, mParamFromBO.battleOptionData.duration, mParamFromBO.battleOptionData.tick, 0, 0, 
                     eBuffIconType.Debuff_Addiction);

        EventMgr.Instance.SendEvent(mBuffEvt);
        return true;
    }
}
