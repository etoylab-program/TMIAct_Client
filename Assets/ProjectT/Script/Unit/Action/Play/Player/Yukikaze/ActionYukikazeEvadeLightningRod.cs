
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionYukikazeEvadeLightningRod : ActionExtreamEvade
{
    public	float			StunRange				= 4.0f;

    private float			mLightningRodDamageRatio = 0.3f;
    private int				mEffId = 202;
	private AniEvent.sEvent	mAniEvt = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        StunRange += mValue1;
        mLightningRodDamageRatio += mValue2;

        if (mValue1 > 0.0f)
        {
            mEffId = 204;
        }
        else
        {
            mEffId = 202;
        }

		mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.StunAttack, 0, eHitDirection.None, "", Vector3.zero, Vector3.zero, Vector3.zero);
	}

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        PrepareStunAttack();
    }

    private void PrepareStunAttack()
    {
        mOwnerPlayer.ExtreamEvading = true;
        float duration = GameInfo.Instance.BattleConfig.BuffDuration + mParam.AddedDuration;

        StopCoroutine("StartStunAttack");
        StartCoroutine("StartStunAttack", duration);
    }

    private IEnumerator StartStunAttack(float duration)
    {
        yield return StartCoroutine(ContinueDash());

        m_checkTime = 0.0f;
        float stunCheckTime = 0.4f;

        Vector3 stunPos = m_owner.evadedTarget.transform.position;
        stunPos.y = m_owner.transform.position.y;

		mAniEvt.isRangeAttack = true;

        List<UnitCollider> list = new List<UnitCollider>();

        bool showEff = false;

        while (true)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= duration)
            {
                mOwnerPlayer.ExtreamEvading = false;
                EffectManager.Instance.StopEffImmediate(mEffId, EffectManager.eType.Common, null);
                
                break;
            }
            else
            {
                stunCheckTime += m_owner.fixedDeltaTime;
                if (stunCheckTime >= 0.4f)
                {
                    stunCheckTime = 0.0f;

                    List<UnitCollider> listTargetCollider = m_owner.GetTargetColliderListByAround(stunPos, StunRange);
                    World.Instance.EnemyMgr.SortListTargetByNearDistance(m_owner, ref listTargetCollider);

                    if (listTargetCollider != null && listTargetCollider.Count > 0)
                    {
                        list.Clear();
                        for (int i = 0; i < listTargetCollider.Count; i++)
                        {
                            Unit target = listTargetCollider[i].Owner;
                            if(target == null)
                            {
                                continue;
                            }

                            ActionHit actionHit = target.actionSystem.GetCurrentAction<ActionHit>();
                            if(actionHit != null && actionHit.State == ActionHit.eState.Stun)
                            {
                                continue;
                            }

                            list.Add(listTargetCollider[i]);
                        }

                        if (list.Count > 0)
                        {
                            if (!showEff)
                            {
                                EffectManager.Instance.Play(list[0].Owner, mEffId, EffectManager.eType.Common, duration);
                                showEff = true;
                            }

                            mAtkEvt.Set(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Unit, mAniEvt, 
										mOwnerPlayer.attackPower * mLightningRodDamageRatio, eAttackDirection.Skip, false, 0, EffectManager.eType.None, 
										list, 0.0f, false, false, false, TableId, null);

                            EventMgr.Instance.SendEvent(mAtkEvt);
                        }
                    }
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }
}
