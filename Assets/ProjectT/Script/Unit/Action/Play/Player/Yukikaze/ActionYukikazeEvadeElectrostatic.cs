
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionYukikazeEvadeElectrostatic : ActionExtreamEvade
{
    public	float			ElectrostaticInterval       = 0.8f;
    public	float			ElectrostaticDamageRatio    = 1.6f;
    public	float			ElectrostaticRange          = 8.0f;
    public	int				ElectrostaticTargetCount    = 1;
	private	AniEvent.sEvent	mAniEvt						= null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        ElectrostaticDamageRatio += mValue1;
        ElectrostaticRange += mValue2;
        ElectrostaticTargetCount += (int)mValue3;

		mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.Attack, 203, eHitDirection.None, "", Vector3.zero, Vector3.zero, Vector3.zero);
	}

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        PrepareElectrostaticAttack();
    }

    private void PrepareElectrostaticAttack()
    {
        float duration = GameInfo.Instance.BattleConfig.BuffDuration + mParam.AddedDuration;
        EffectManager.Instance.Play(m_owner, 50007, EffectManager.eType.Common, duration);

        StopCoroutine("StartElectrostaticAttack");
        StartCoroutine("StartElectrostaticAttack", duration);
    }

    private IEnumerator StartElectrostaticAttack(float duration)
    {
        yield return StartCoroutine(ContinueDash());
        //m_owner.RestoreSuperArmor(superArmor, GetType());

        m_checkTime = 0.0f;
        float attackCheckTime = ElectrostaticInterval;

		mAniEvt.isRangeAttack = true;

        List<UnitCollider> list = new List<UnitCollider>();

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= duration)
            {
                mOwnerPlayer.ExtreamEvading = false;
                EffectManager.Instance.StopEffImmediate(50007, EffectManager.eType.Common, null);

                break;
            }
            else
            {
                attackCheckTime += m_owner.fixedDeltaTime;
                if (attackCheckTime >= ElectrostaticInterval)
                {
                    attackCheckTime = 0.0f;

                    List<UnitCollider> listTargetCollider = m_owner.GetTargetColliderListByAround(m_owner.transform.position, ElectrostaticRange);
                    if (listTargetCollider != null && listTargetCollider.Count > 0)
                    {
                        World.Instance.EnemyMgr.SortListTargetByNearDistance(m_owner, ref listTargetCollider);

                        list.Clear();
                        for (int i = 0; i < ElectrostaticTargetCount; i++)
                        {
                            if (i >= listTargetCollider.Count)
                            {
                                break;
                            }

                            list.Add(listTargetCollider[i]);
                        }

                        if (list.Count > 0)
                        {
                            mAtkEvt.Set(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Unit, mAniEvt, 
										mOwnerPlayer.attackPower * ElectrostaticDamageRatio, eAttackDirection.Skip, false, 0, EffectManager.eType.None, 
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
