
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionWpnKurenai17 : ActionWeaponSkillBase
{
    private float                       mDuration   = 0.0f;
    private Projectile                  mPjtBlade   = null;
    private AniEvent.sEvent             mAniEvt     = null;
    private AniEvent.sProjectileInfo    mPjtInfo    = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.WpnKurenai17;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        superArmor = Unit.eSuperArmor.Invincible;

        mPjtBlade = GameSupport.CreateProjectile("Projectile/pjt_character_kurenai_wskill_17_01.prefab");
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ActionParamFromBO paramFromBO = param as ActionParamFromBO;

        if (mAniEvt == null)
        {
            mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f);
        }

        if (mPjtInfo == null)
        {
            mPjtInfo = m_owner.aniEvent.CreateProjectileInfo(mPjtBlade);
            mPjtInfo.attach = true;
            mPjtInfo.boneName = "root";
            mPjtInfo.followParentRot = true;
        }

        mDuration = paramFromBO.battleOptionData.duration;

        mPjtBlade.duration = mDuration;
        mAniEvt.atkRatio = paramFromBO.battleOptionData.value;
        
        StopCoroutine("UpdateBlade");
        StartCoroutine("UpdateBlade");
    }

    private IEnumerator UpdateBlade()
    {
        Unit target = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
        if(target == null)
        {
            yield break;
        }

        mPjtBlade.Fire(m_owner, BattleOption.eToExecuteType.Unit, mAniEvt, mPjtInfo, target, TableId);
        m_checkTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= mDuration)
            {
                break;
            }

            yield return mWaitForFixedUpdate;
        }
    }
}
