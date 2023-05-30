
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterShiranuiFloor : ActionSupporterSkillBase
{
    private ParticleSystem  mEffFloor                   = null;
    private BoxCollider     mBoxFloor                   = null;
    private Vector3         mFloorPos                   = Vector3.zero;
    private List<Unit>      mListStayOnBoxColliderEnemy = new List<Unit>();


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterShiranuiFloor;

        mEffFloor = GameSupport.CreateParticle("Effect/Supporter/prf_fx_supporter_skill_108.prefab", null);
        mBoxFloor = mEffFloor.GetComponent<BoxCollider>();
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParamFromBO = param as ActionParamFromBO;

        StopCoroutine("UpdateFloor");
        StartCoroutine("UpdateFloor");
    }

    private IEnumerator UpdateFloor()
    {
        mFloorPos = m_owner.posOnGround + (m_owner.transform.forward * 1.25f);

        mEffFloor.transform.position = mFloorPos;
        mEffFloor.gameObject.SetActive(true);
        EffectManager.Instance.RegisterStopEffByDuration(mEffFloor, null, mParamFromBO.battleOptionData.duration);
        
        BuffEvent buffEvt2 = new BuffEvent();
        WorldPVP worldPVP = World.Instance as WorldPVP;
        List<Unit> listEnemy = null;

        m_checkTime = 0.0f;
        mListStayOnBoxColliderEnemy.Clear();

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= mParamFromBO.battleOptionData.duration || (worldPVP && (!worldPVP.IsBattleStart || m_owner.curHp <= 0.0f)))
            {
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

                    // 속도 감소
                    mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
                    mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
                    mBuffEvt.battleOptionData.conditionType = BattleOption.eBOConditionType.StayOnBoxCollider;
                    mBuffEvt.battleOptionData.buffIconFlash = false;

                    mBuffEvt.Set(0, eEventSubject.Self, eEventType.EVENT_DEBUFF_SPEED_DOWN, enemy, mParamFromBO.battleOptionData.value, 0.0f, 0.0f, 
                                 mParamFromBO.battleOptionData.duration, 0.0f, mBuffEvt.effId, mBuffEvt.effId2, eBuffIconType.Debuff_Speed);

                    EventMgr.Instance.SendEvent(mBuffEvt);

                    // 받는 대미지 증가
                    buffEvt2.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
                    buffEvt2.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
                    buffEvt2.battleOptionData.conditionType = BattleOption.eBOConditionType.StayOnBoxCollider;
                    buffEvt2.battleOptionData.buffIconFlash = false;

                    buffEvt2.Set(1, eEventSubject.Self, eEventType.EVENT_DEBUFF_DMG_RATE_UP, enemy, mParamFromBO.battleOptionData.value, 
                                 0.0f, 0.0f, mParamFromBO.battleOptionData.duration, 0.0f, buffEvt2.effId, buffEvt2.effId2, eBuffIconType.Debuff_Def);

                    EventMgr.Instance.SendEvent(buffEvt2);
                }
                else if(!isEnemyInBox && enemy.StayOnBoxCollider)
                {
                    enemy.StayOnBoxCollider = null;
                    mListStayOnBoxColliderEnemy.Remove(enemy);
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }
}
