
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterAsukaRocketAttackOnEvade : ActionSupporterSkillBase
{
    private Projectile					mPjtRocket      = null;
	private AniEvent.sProjectileInfo	mPjtInfo		= null;
	private float						mCoolTime       = 0.0f;
    private bool						mPossibleToUse  = false;
    private Coroutine					mCr             = null;
	private AniEvent.sEvent				mAniEvt			= null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterAsukaRocketAttackOnEvade;

        mPjtRocket = GameSupport.CreateProjectile("Projectile/pjt_supporter_asuka_rocket.prefab");
		mPossibleToUse = true;
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
			mPjtInfo = mOwnerPlayer.aniEvent.CreateProjectileInfo(mPjtRocket);
		}

        mCoolTime = (mParamFromBO.battleOptionData.value2 * (float)eCOUNT.MAX_BO_FUNC_VALUE);

        if (mPossibleToUse)
        {
            UnitCollider targetCollider = mOwnerPlayer.GetMainTargetCollider(true, mParamFromBO.battleOptionData.value3);
            if (targetCollider)
            {
				mPjtInfo.addedPosition = mOwnerPlayer.GetCenterPos() - transform.position;
				mPjtInfo.notAniEventAtk = true;

				mAniEvt.atkRatio = mParamFromBO.battleOptionData.value;

				mPjtRocket.Fire(mOwnerPlayer, BattleOption.eToExecuteType.Supporter, mAniEvt, mPjtInfo, targetCollider.Owner, -1);
                
                Utility.StopCoroutine(World.Instance, ref mCr);
                mCr = World.Instance.StartCoroutine(CheckCoolTime());
            }
        }

        m_endUpdate = true;
    }

    private IEnumerator CheckCoolTime()
    {
        float checkTime = 0.0f;
        mPossibleToUse = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(checkTime <= mCoolTime)
        {
            checkTime += m_owner.fixedDeltaTime;
            yield return mWaitForFixedUpdate;
        }

        mPossibleToUse = true;
    }
}
