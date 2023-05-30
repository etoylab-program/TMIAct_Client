
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporter112Attack : ActionSupporterSkillBase
{
    private List<Projectile>			mListPjt        = new List<Projectile>();
	private AniEvent.sProjectileInfo	mPjtInfo		= null;
	private float						mAttackPower    = 0.0f;
	private AniEvent.sEvent				mAniEvt			= null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Supporter112Attack;

        for( int i = 0; i < 10; i++ ) {
            Projectile pjt = GameSupport.CreateProjectile( "Projectile/pjt_supporter_112.prefab" );
            pjt.OnHitFunc = OnProjectileHit;

            mListPjt.Add( pjt );
        }
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
			mPjtInfo = mOwnerPlayer.aniEvent.CreateProjectileInfo(mListPjt[0]);
		}

        mAttackPower = m_owner.attackPower * mParamFromBO.battleOptionData.value;

        StopCoroutine( "Fire" );
        StartCoroutine( "Fire" );
    }

    private IEnumerator Fire() {
        float checkTime = mParamFromBO.battleOptionData.value3;
        int count = 0;
        int maxCount = Mathf.RoundToInt( mParamFromBO.battleOptionData.value2 * (float)eCOUNT.MAX_BO_FUNC_VALUE );

        while( true ) {
            if( count >= maxCount ) {
                break;
			}

            checkTime += Time.fixedDeltaTime;
            if( checkTime >= mParamFromBO.battleOptionData.value3 ) {
                UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
                if( targetCollider ) {
                    mPjtInfo.followParentRot = true;
                    mPjtInfo.addedPosition = mOwnerPlayer.GetCenterPos() - transform.position;
                    mPjtInfo.notAniEventAtk = true;

                    mAniEvt.atkRatio = mParamFromBO.battleOptionData.value;

                    mListPjt[count].Fire( mOwnerPlayer, BattleOption.eToExecuteType.Supporter, mAniEvt, mPjtInfo, targetCollider.Owner, -1 );
                }

                checkTime = 0.0f;
                ++count;
			}

            yield return mWaitForFixedUpdate;
		}
	}

    private bool OnProjectileHit(Unit hitTarget)
    {
        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
        mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.None;

        mBuffEvt.Set(0, eEventSubject.HitTargetList, eEventType.EVENT_DEBUFF_DOT_DMG, mOwnerPlayer, 0.5f, 
                     (float)EAttackAttr.POISON, 0.0f, mParamFromBO.battleOptionData.duration, mParamFromBO.battleOptionData.tick, 0, 0, 
                     eBuffIconType.Debuff_Addiction);

        EventMgr.Instance.SendEvent(mBuffEvt);
        return true;
    }
}
