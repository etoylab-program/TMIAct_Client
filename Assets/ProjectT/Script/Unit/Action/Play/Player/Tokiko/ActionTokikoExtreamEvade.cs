
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionTokikoExtreamEvade : ActionExtreamEvade
{
    private ParticleSystem              mPsStart        = null;
    private List<Projectile>            mListPjt        = new List<Projectile>();
    private int                         mPjtCount       = 3;
    private int                         mCurPjtIndex    = 0;
    private AniEvent.sProjectileInfo    mPjtInfo        = null;
    private AniEvent.sEvent             mAniEvt         = null;
    private float                       mFireTerm       = 0.7f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        mPsStart = GameSupport.CreateParticle("Effect/Character/prf_fx_tokiko_buff.prefab", null);

        for (int i = 0; i < mPjtCount; i++)
        {
            Projectile pjt = GameSupport.CreateProjectile("Projectile/pjt_character_tokiko_kunai_nohit.prefab");
            mListPjt.Add(pjt);
        }

        mPjtInfo = mOwnerPlayer.aniEvent.CreateProjectileInfo(mListPjt[0]);
        mPjtInfo.attach = true;
        mPjtInfo.boneName = "Bip001 R Hand";

        mAniEvt = mOwnerPlayer.aniEvent.CreateEvent(eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 0.2f + (0.2f * mValue2));
        mCurPjtIndex = 0;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        StartEvadingBuff();
    }

    private void StartEvadingBuff()
    {
        mOwnerPlayer.ExtreamEvading = true;

        mPsStart.transform.SetParent(mOwnerPlayer.transform);
        Utility.InitTransform(mPsStart.gameObject);
        mPsStart.gameObject.SetActive(true);

        float duration = 6.0f + (6.0f * mValue3);

        StopCoroutine("EndEvadingBuff");
        StartCoroutine("EndEvadingBuff", duration);

        // 방어력 감소
        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
        mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
        mBuffEvt.Set(GetInstanceID(), eEventSubject.ActiveEnemies, eEventType.EVENT_DEBUFF_DMG_RATE_UP, m_owner, 0.5f + (0.5f * mValue1), 0.0f, 0.0f,
                     duration, 0.0f, 0, mBuffEvt.effId2, eBuffIconType.Debuff_Def);

        EventMgr.Instance.SendEvent(mBuffEvt);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        IsSkillEnd = false;
    }

    private IEnumerator EndEvadingBuff(float duration)
    {
        yield return StartCoroutine(ContinueDash());

        float checkFireTime = mFireTerm;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate)
        {
            m_checkTime += Time.fixedDeltaTime;
            if (m_checkTime >= duration)
            {
                m_endUpdate = true;
            }

            checkFireTime += Time.fixedDeltaTime;
            if(checkFireTime >= mFireTerm)
            {
                Unit target = World.Instance.EnemyMgr.GetNearestTarget(mOwnerPlayer, true);
                if (target)
                {
                    mListPjt[mCurPjtIndex++].Fire(mOwnerPlayer, BattleOption.eToExecuteType.Unit, mAniEvt, mPjtInfo, target, TableId, null, null);
                    if (mCurPjtIndex >= mPjtCount)
                    {
                        mCurPjtIndex = 0;
                    }
                }

                checkFireTime = 0.0f;
            }

            yield return mWaitForFixedUpdate;
        }

        mOwnerPlayer.ExtreamEvading = false;
        IsSkillEnd = true;

        mPsStart.gameObject.SetActive(false);
    }
}
