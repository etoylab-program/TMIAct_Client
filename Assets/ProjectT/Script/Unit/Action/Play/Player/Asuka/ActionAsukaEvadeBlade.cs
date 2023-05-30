
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAsukaEvadeBlade : ActionExtreamEvade
{
    private float                       mDuration   = 0.0f;
    private Projectile                  mPjtBlade   = null;
    private AniEvent.sEvent             mAniEvt     = null;
    private AniEvent.sProjectileInfo    mPjtInfo    = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        mDuration = GameInfo.Instance.BattleConfig.BuffDuration * 1.5f;

        if (mValue2 > 0.0f)
        {
            mPjtBlade = GameSupport.CreateProjectile("Projectile/pjt_character_asuka_blade_02.prefab");
        }
        else
        {
            mPjtBlade = GameSupport.CreateProjectile("Projectile/pjt_character_asuka_blade.prefab");
        }

        mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 0.4f);
        if (mValue1 > 0.0f)
        {
            mAniEvt.atkRatio = 0.6f;
        }

        mPjtInfo = m_owner.aniEvent.CreateProjectileInfo(mPjtBlade);
        mPjtInfo.attach = true;
        mPjtInfo.boneName = "root";
        mPjtInfo.followParentRot = true;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mPjtBlade.duration = mDuration;

        StopCoroutine("UpdateBlade");
        StartCoroutine("UpdateBlade");
    }

    public override void OnEnd()
    {
        base.OnEnd();
        IsSkillEnd = false;
    }

    private IEnumerator UpdateBlade()
    {
        yield return StartCoroutine(ContinueDash());
        //m_owner.RestoreSuperArmor(superArmor, GetType());

        mPjtBlade.Fire(m_owner, BattleOption.eToExecuteType.Unit, mAniEvt, mPjtInfo, m_owner.evadedTarget, TableId);
        m_checkTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= mDuration)
            {
                mOwnerPlayer.ExtreamEvading = false;
                break;
            }

            yield return mWaitForFixedUpdate;
        }

		mPjtBlade.Hide( false, true );

		IsSkillEnd = true;
    }
}
