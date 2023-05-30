
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEmilyEvadeHealing : ActionExtreamEvade
{
    private float           mDuration       = 0.0f;
    private ParticleSystem  mEffHealFloor   = null;
    private BoxCollider     mBoxHealFloor   = null;
    private Vector3         mHealFloorPos   = Vector3.zero;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        IsIgnoreAttackSkip = true;
        mDuration = GameInfo.Instance.BattleConfig.BuffDuration;

        if (mValue1 > 0.0f)
        {
            mEffHealFloor = GameSupport.CreateParticle("Effect/Character/prf_fx_emily_evade_skill_2.prefab", null);
        }
        else
        {
            mEffHealFloor = GameSupport.CreateParticle("Effect/Character/prf_fx_emily_evade_skill_1.prefab", null);
        }

        mBoxHealFloor = mEffHealFloor.GetComponent<BoxCollider>();
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        StopCoroutine("UpdateHealing");
        StartCoroutine("UpdateHealing");
    }

    private IEnumerator UpdateHealing()
    {
        yield return StartCoroutine(ContinueDash());
        //m_owner.RestoreSuperArmor(superArmor, GetType());

        mHealFloorPos = m_owner.posOnGround + (m_owner.transform.forward * 1.25f);

        mEffHealFloor.transform.position = mHealFloorPos;
        mEffHealFloor.gameObject.SetActive(true);
        EffectManager.Instance.RegisterStopEffByDuration(mEffHealFloor, null, mDuration);

        m_checkTime = 0.0f;
        float tickCheckTime = mValue3;
        float healPercentage = mValue2 / (float)eCOUNT.MAX_BO_FUNC_VALUE;

        WorldPVP worldPVP = World.Instance as WorldPVP;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= mDuration || (worldPVP && (!worldPVP.IsBattleStart || m_owner.curHp <= 0.0f)))
            {
                mOwnerPlayer.ExtreamEvading = false;
                EffectManager.Instance.StopEffImmediate(mEffHealFloor, null);

                yield break;
            }

            /*
            if(mBoxHealFloor.bounds.Contains(m_owner.transform.position))
            {
                tickCheckTime += m_owner.fixedDeltaTime;
                if(tickCheckTime >= mValue3)
                {
                    m_owner.AddHpPercentage(BattleOption.eToExecuteType.Unit, healPercentage);
                    tickCheckTime = 0.0f;
                }
            }
            else
            {
                tickCheckTime = 0.0f;
            }
            */

            tickCheckTime += m_owner.fixedDeltaTime;

            if( tickCheckTime >= mValue3 ) {
                if( World.Instance.ListPlayer.Count > 0 ) {
                    for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                        if( World.Instance.ListPlayer[i].curHp <= 0.0f || !World.Instance.ListPlayer[i].IsActivate() ) {
                            continue;
						}

                        if( mBoxHealFloor.bounds.Contains( World.Instance.ListPlayer[i].transform.position ) ) {
                            World.Instance.ListPlayer[i].AddHpPercentage( BattleOption.eToExecuteType.Unit, healPercentage, false );
                        }
                    }
                }
                else {
					if( m_owner.curHp > 0.0f && mBoxHealFloor.bounds.Contains( m_owner.transform.position ) ) {
						m_owner.AddHpPercentage( BattleOption.eToExecuteType.Unit, healPercentage, false );
					}
				}

                tickCheckTime = 0.0f;
            }

            yield return mWaitForFixedUpdate;
        }
    }
}
