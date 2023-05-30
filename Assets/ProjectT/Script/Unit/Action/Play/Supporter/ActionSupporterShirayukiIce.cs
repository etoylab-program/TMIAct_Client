
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterShirayukiIce : ActionSupporterSkillBase
{
    private int								mProjectileCount    = 5;
    private float							mProjectileFireTerm = 0.15f;
    private List<Projectile>				mListProjectile     = new List<Projectile>();
	private List<AniEvent.sProjectileInfo>	mListPjtInfo		= new List<AniEvent.sProjectileInfo>();
	private AniEvent.sEvent					mAniEvt				= null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterShirayukiIce;

        for (int i = 0; i < mProjectileCount; i++)
        {
            Projectile pjt = GameSupport.CreateProjectile("Projectile/pjt_supporter_85.prefab");
            pjt.OnHitFunc = OnProjectileHit;

            mListProjectile.Add(pjt);
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

		if (mListPjtInfo.Count <= 0)
		{
			for (int i = 0; i < mProjectileCount; i++)
			{
				mListPjtInfo.Add(mOwnerPlayer.aniEvent.CreateProjectileInfo(mListProjectile[i]));
			}
		}
	}

    public override IEnumerator UpdateAction()
    {
        int projectileIndex = 0;
        m_checkTime = mProjectileFireTerm;

        Unit target = m_owner.mainTarget == null ? World.Instance.EnemyMgr.GetNearestTarget(m_owner, true) : m_owner.mainTarget;
        if(target == null)
        {
            m_endUpdate = true;
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate)
        {
            m_checkTime += Time.fixedDeltaTime;
            if(m_checkTime >= mProjectileFireTerm)
            {
				mAniEvt.atkRatio = mParamFromBO.battleOptionData.value;
				mAniEvt.pauseFrame = 0.0f;

				mListPjtInfo[projectileIndex].addedPosition = mOwnerPlayer.GetCenterPos() - transform.position;
				mListPjtInfo[projectileIndex].notAniEventAtk = false;

                mListProjectile[projectileIndex].Fire(mOwnerPlayer, BattleOption.eToExecuteType.Supporter, mAniEvt, mListPjtInfo[projectileIndex], 
													  target, TableId);

                m_checkTime = 0.0f;

                ++projectileIndex;
                if(projectileIndex >= mListProjectile.Count)
                {
                    break;
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }

    private bool OnProjectileHit(Unit target)
    {
        if (mParamFromBO.battleOptionData.addCallTiming == BattleOption.eBOAddCallTiming.OnSend)
        {
            if (mParamFromBO.battleOptionData.timingType == BattleOption.eBOTimingType.Use &&
                mParamFromBO.battleOptionData.dataOnEndCall.conditionType == BattleOption.eBOConditionType.ComboCountAsValue)
            {
                mParamFromBO.battleOptionData.dataOnEndCall.evt.value = mParamFromBO.battleOptionData.evt.value;
            }

            EffectManager.Instance.Play(m_owner, mParamFromBO.battleOptionData.dataOnEndCall.startEffId, (EffectManager.eType)mParamFromBO.battleOptionData.dataOnEndCall.effType);

            mParamFromBO.battleOptionData.dataOnEndCall.useTime = System.DateTime.Now;
            EventMgr.Instance.SendEvent(mParamFromBO.battleOptionData.dataOnEndCall.evt);

            Log.Show(mParamFromBO.battleOptionData.dataOnEndCall.evt.battleOptionData.battleOptionSetId + "번 배틀옵션셋 사용 (애드콜)!!!", Log.ColorType.Green);
        }

        return true;
    }
}
