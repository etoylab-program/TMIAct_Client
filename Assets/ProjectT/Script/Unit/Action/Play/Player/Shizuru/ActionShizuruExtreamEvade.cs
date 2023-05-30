
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionShizuruExtreamEvade : ActionExtreamEvade
{
    private float           mDuration                   = 0.0f;
    private ParticleSystem  mEffFloor                   = null;
    private BoxCollider     mBoxFloor                   = null;
    private Vector3         mFloorPos                   = Vector3.zero;
    private List<Unit>      mListStayOnBoxColliderEnemy = new List<Unit>();


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        mDuration = GameInfo.Instance.BattleConfig.BuffDuration + 2.0f;

        if (mValue1 > 0.0f)
        {
            mEffFloor = GameSupport.CreateParticle("Effect/Character/prf_fx_shizuru_plant_floor_skill.prefab", null);
        }
        else
        {
            mEffFloor = GameSupport.CreateParticle("Effect/Character/prf_fx_shizuru_plant_floor.prefab", null);
        }

        mBoxFloor = mEffFloor.GetComponent<BoxCollider>();
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        StopCoroutine("UpdateSkill");
        StartCoroutine("UpdateSkill");
    }

    private IEnumerator UpdateSkill()
    {
        yield return StartCoroutine(ContinueDash());

        mFloorPos = m_owner.posOnGround + (m_owner.transform.forward * 1.25f);

        mEffFloor.transform.position = mFloorPos;
        mEffFloor.gameObject.SetActive(true);
        EffectManager.Instance.RegisterStopEffByDuration(mEffFloor, null, mDuration);
        
        float tickCheckTime = 0.5f;
        float healPercentage = mValue2 / (float)eCOUNT.MAX_BO_FUNC_VALUE;
        float speedDownPercentage = mValue3 / (float)eCOUNT.MAX_BO_FUNC_VALUE;

        WorldPVP worldPVP = World.Instance as WorldPVP;
        List<Unit> listEnemy = null;

        m_checkTime = 0.0f;
        mListStayOnBoxColliderEnemy.Clear();

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= mDuration || (worldPVP && (!worldPVP.IsBattleStart || m_owner.curHp <= 0.0f)))
            {
                mOwnerPlayer.ExtreamEvading = false;
                EffectManager.Instance.StopEffImmediate(mEffFloor, null);

                for(int i = 0; i < mListStayOnBoxColliderEnemy.Count; i++)
                {
                    mListStayOnBoxColliderEnemy[i].StayOnBoxCollider = null;
                }

                yield break;
            }

            listEnemy = m_owner.GetEnemyList(true);
            for (int i = 0; i < listEnemy.Count; i++)
            {
                Unit enemy = listEnemy[i];
                if(enemy.curHp <= 0.0f)
                {
                    continue;
                }

                bool isEnemyInBox = mBoxFloor.bounds.Contains(enemy.transform.position);
                if (isEnemyInBox && enemy.StayOnBoxCollider == null)
                {
                    enemy.StayOnBoxCollider = mBoxFloor;
                    mListStayOnBoxColliderEnemy.Add(enemy);

                    mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
                    mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
                    mBuffEvt.battleOptionData.conditionType = BattleOption.eBOConditionType.StayOnBoxCollider;
                    mBuffEvt.battleOptionData.buffIconFlash = false;

                    mBuffEvt.Set(m_data.ID, eEventSubject.Self, eEventType.EVENT_DEBUFF_SPEED_DOWN, enemy, speedDownPercentage, 0.0f, 0.0f, mDuration, 0.0f,
                                 mBuffEvt.effId, mBuffEvt.effId2, eBuffIconType.Debuff_Speed);

                    EventMgr.Instance.SendEvent(mBuffEvt);
                }
                else if(!isEnemyInBox && enemy.StayOnBoxCollider)
                {
                    enemy.StayOnBoxCollider = null;
                    mListStayOnBoxColliderEnemy.Remove(enemy);
                }
            }

            /*
            if(mBoxFloor.bounds.Contains(m_owner.transform.position))
            {
                tickCheckTime += m_owner.fixedDeltaTime;
                if(tickCheckTime >= 0.5f)
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

            if( tickCheckTime >= 0.5f ) {
                if( World.Instance.ListPlayer.Count > 0 ) {
                    for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                        if( World.Instance.ListPlayer[i].curHp <= 0.0f || !World.Instance.ListPlayer[i].IsActivate() ) {
                            continue;
                        }

                        if( mBoxFloor.bounds.Contains( World.Instance.ListPlayer[i].transform.position ) ) {
                            World.Instance.ListPlayer[i].AddHpPercentage( BattleOption.eToExecuteType.Unit, healPercentage, false );
                        }
                    }
                }
                else {
                    if( m_owner.curHp > 0.0f && mBoxFloor.bounds.Contains( m_owner.transform.position ) ) {
                        m_owner.AddHpPercentage( BattleOption.eToExecuteType.Unit, healPercentage, false );
                    }
                }

                tickCheckTime = 0.0f;
            }

            yield return mWaitForFixedUpdate;
        }
    }
}
