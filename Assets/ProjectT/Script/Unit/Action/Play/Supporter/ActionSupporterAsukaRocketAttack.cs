
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterAsukaRocketAttack : ActionSupporterSkillBase
{
    private int								mRocketMaxCount = 0;
    private List<Projectile>				mListPjtRocket  = new List<Projectile>();
	private List<AniEvent.sProjectileInfo>	mListPjtInfo	= new List<AniEvent.sProjectileInfo>();
	private AniEvent.sEvent					mAniEvt			= null;

    public override void SetBattleOptionSetParam(GameClientTable.BattleOptionSet.Param param)
    {
        if (param == null)
        {
            mRocketMaxCount = 1;
            return;
        }

        mRocketMaxCount = param.BOFuncValue2;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterAsukaRocketAttack;

        for (int i = 0; i < mRocketMaxCount; i++)
        {
            Projectile pjt = GameSupport.CreateProjectile("Projectile/pjt_supporter_asuka_rocket_explosion.prefab");
            mListPjtRocket.Add(pjt);
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
			for (int i = 0; i < mRocketMaxCount; i++)
			{
				mListPjtInfo.Add(mOwnerPlayer.aniEvent.CreateProjectileInfo(mListPjtRocket[i]));
			}
		}

		int rocketIndex = 0;

        List<Unit> listTarget = mOwnerPlayer.GetEnemyList(true);
        for(int i = 0; i < listTarget.Count; i++)
        {
            if(rocketIndex >= mListPjtRocket.Count)
            {
                break;
            }

			mListPjtInfo[rocketIndex].addedPosition = mOwnerPlayer.GetCenterPos() - transform.position;
			mListPjtInfo[rocketIndex].notAniEventAtk = true;

			mAniEvt.atkRatio = mParamFromBO.battleOptionData.value;

			mListPjtRocket[rocketIndex].Fire(mOwnerPlayer, BattleOption.eToExecuteType.Supporter, mAniEvt, mListPjtInfo[rocketIndex], listTarget[i], -1);
            ++rocketIndex;
        }

        m_endUpdate = true;
    }
}
