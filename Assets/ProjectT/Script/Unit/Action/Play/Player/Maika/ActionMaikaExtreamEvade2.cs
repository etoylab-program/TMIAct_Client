
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMaikaExtreamEvade2 : ActionExtreamEvade
{
    private Projectile					mPjt		= null;
    private AniEvent.sProjectileInfo	mPjtInfo	= null;
    private AniEvent.sEvent				mAniEvt     = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

		if (mValue3 >= 1.0f)
		{
			mPjt = GameSupport.CreateProjectile("Projectile/pjt_character_maika_airstrike_skill_02.prefab");
		}
		else if (mValue2 >= 1.0f)
		{
			mPjt = GameSupport.CreateProjectile("Projectile/pjt_character_maika_airstrike_skill_01.prefab");
		}
		else
		{
			mPjt = GameSupport.CreateProjectile("Projectile/pjt_character_maika_airstrike.prefab");
		}

		mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 0.6f);
        //mAniEvt.atkRatio = mValue2;

		mPjtInfo = m_owner.aniEvent.CreateProjectileInfo(mPjt);
		mPjtInfo.attach = false;
		mPjtInfo.followParentRot = true;
	}

	public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        StartSkill();
    }

    private void StartSkill()
    {
        mOwnerPlayer.ExtreamEvading = true;

        StopCoroutine("EndSkill");
        StartCoroutine("EndSkill", mValue1);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        IsSkillEnd = false;
    }

    private IEnumerator EndSkill(float duration)
    {
		Unit target = World.Instance.EnemyMgr.GetNearestTarget(mOwnerPlayer, true);
		mPjt.Fire(m_owner, BattleOption.eToExecuteType.Unit, mAniEvt, mPjtInfo, target == null ? m_owner.evadedTarget : target, TableId, null, null, true);

		yield return StartCoroutine(ContinueDash());

		m_checkTime = 0.0f;
        while (true)
        {
            if (!World.Instance.IsPause)
            {
                m_checkTime += Time.fixedDeltaTime;
            }

            if (m_checkTime >= duration)
            {
                mOwnerPlayer.ExtreamEvading = false;
                IsSkillEnd = true;

                break;
            }

            yield return mWaitForFixedUpdate;
        }
    }
}
