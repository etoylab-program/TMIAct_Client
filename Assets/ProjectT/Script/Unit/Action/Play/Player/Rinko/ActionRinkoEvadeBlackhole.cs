
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionRinkoEvadeBlackhole : ActionExtreamEvade
{
    private float           mDuration       = 0.0f;
    private ParticleSystem  mEffBlackhole   = null;
    private Vector3         mBlackholePos   = Vector3.zero;
    private List<Unit>      mListTarget     = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        mDuration = GameInfo.Instance.BattleConfig.BuffDuration + (GameInfo.Instance.BattleConfig.BuffDuration * mValue1);
        mEffBlackhole = GameSupport.CreateParticle("Effect/Character/prf_fx_rinko_evade_portal.prefab", null);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mBlackholePos = m_owner.posOnGround + (m_owner.transform.forward * 1.25f);

        mEffBlackhole.transform.position = mBlackholePos;
        mEffBlackhole.gameObject.SetActive(true);
        EffectManager.Instance.RegisterStopEffByDuration(mEffBlackhole, null, mDuration);

        mListTarget = m_owner.GetEnemyList(true);
        if (mListTarget != null && mListTarget.Count > 0)
        {
            for (int i = 0; i < mListTarget.Count; i++)
            {
                Unit target = mListTarget[i];
                if (target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback())
                {
                    continue;
                }

                target.actionSystem.CancelCurrentAction();
                target.StopStepForward();
            }
        }

        StopCoroutine("UpdateBlackhole");
        StartCoroutine("UpdateBlackhole");
    }

    private IEnumerator UpdateBlackhole()
    {
        if (mListTarget == null || mListTarget.Count <= 0)
        {
            EffectManager.Instance.StopEffImmediate(mEffBlackhole, null);
            yield break;
        }

        yield return StartCoroutine(ContinueDash());
        //m_owner.RestoreSuperArmor(superArmor, GetType());

        m_checkTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= mDuration)
            {
                mOwnerPlayer.ExtreamEvading = false;
                EffectManager.Instance.StopEffImmediate(mEffBlackhole, null);

                break;
            }

            for (int i = 0; i < mListTarget.Count; i++)
            {
                Unit target = mListTarget[i];
                if (target.MainCollider == null)
                {
                    continue;
                }

                if (target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback())
                {
                    continue;
                }

                target.StopStepForward();

                Vector3 v = (mBlackholePos - target.transform.position).normalized;
                v.y = 0.0f;

                target.cmptMovement.UpdatePosition(v, Mathf.Max(GameInfo.Instance.BattleConfig.BlackholeMinSpeed, target.speed * 1.2f), false);
            }

            yield return mWaitForFixedUpdate;
        }

        IsSkillEnd = true;
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        EffectManager.Instance.StopEffImmediate(mEffBlackhole, null);
    }

	public override void OnEnd() {
		base.OnEnd();

        IsSkillEnd = false;
	}
}
