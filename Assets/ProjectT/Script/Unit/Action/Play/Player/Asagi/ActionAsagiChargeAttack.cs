
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAsagiChargeAttack : ActionChargeAttack
{
    private List<int> mListUseCloneIndex = new List<int>();


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        World.Instance.UIPlay.btnAtk.m_maxChargeCount = 2;

        m_psStart = GameSupport.CreateParticle("Effect/Character/prf_fx_asagi_charge_step01.prefab", m_owner.transform);
        m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_asagi_charge_step02.prefab", m_owner.transform));
        m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_asagi_charge_step02.prefab", m_owner.transform));
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider)
        {
            m_owner.LookAtTarget(mTargetCollider.GetCenterPos());

            //m_owner.SetMainTarget(mTargetCollider.Owner);
            //World.Instance.InGameCamera.TurnToTarget(mTargetCollider.GetCenterPos(), 2.0f);
        }

        m_psStart.gameObject.SetActive(true);
    }

    public override IEnumerator UpdateAction()
    {
        float chargingTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            if (m_chargeCount < World.Instance.UIPlay.btnAtk.m_maxChargeCount)
            {
                chargingTime += m_owner.fixedDeltaTime;

                m_chargeCount = Mathf.Clamp((int)(chargingTime / World.Instance.UIPlay.btnAtk.m_chargeTime), 1, World.Instance.UIPlay.btnAtk.m_maxChargeCount);
                if (m_chargeCount != m_beforeChargeCount)
                    PlayEffCharge(m_chargeCount - 1);

                m_beforeChargeCount = m_chargeCount;
            }

            UpdateMove(m_owner.speed);
            yield return mWaitForFixedUpdate;
        }

        for(int i = 0; i < mListUseCloneIndex.Count; i++)
        {
            m_owner.HideClone(mListUseCloneIndex[i]);
        }
        mListUseCloneIndex.Clear();

        if (m_chargeCount > 0)
        {
            m_chargeCount = World.Instance.UIPlay.btnAtk.m_maxChargeCount + (int)mValue1;
            StartChargeAttack();

            if (mTargetCollider && FSaveData.Instance.AutoTargeting)
            {
                m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
            }

            yield return new WaitForSeconds(m_aniCutFrameLength);

            List<Unit> list = World.Instance.EnemyMgr.GetActiveEnemies(m_owner);
            mListTarget.Clear();
            mListTarget.AddRange(list.ToArray());

            bool showSkillName = false;
            for (int i = 0; i < m_chargeCount; i++)
            {
                Unit target = null;

                target = World.Instance.EnemyMgr.GetNearestTarget(m_owner, mListTarget);
                if (target != null)// && target.isGrounded)
                {
                    mListTarget.Remove(target);
                }
                else
                {
                    target = null;
                }

                int cloneIndex = m_owner.GetDeactivateCloneIndex(0); //m_owner.GetClone(i);
                if( cloneIndex == -1 ) {
                    continue;
				}

                Unit clone = m_owner.GetClone( cloneIndex );
                if( clone == null ) {
                    continue;
				}

                ActionCloneHomingAttack action = clone.actionSystem.GetAction<ActionCloneHomingAttack>(eActionCommand.CloneHomingAttack);
                if (action)
                {
                    /*
                    int cloneIndex = m_owner.GetDeactivateCloneIndex(0);
                    if(cloneIndex == -1)
                    {
                        continue;
                    }
                    */

                    m_owner.ShowClone(cloneIndex, m_owner.transform.position, m_owner.transform.rotation);
                    clone.CommandAction( eActionCommand.CloneHomingAttack, new ActionParamAttack( cloneIndex, target, AddActionValue1, SetAddAction ) );

                    if (!showSkillName)
                    {
                        ShowSkillNames(m_data);
                        showSkillName = true;
                    }

                    mListUseCloneIndex.Add(cloneIndex);
                }
            }
        }
        else
        {
            m_owner.actionSystem.CancelCurrentAction();
        }

        StopAllEffCharge();

        if (m_chargeCount > 0)
            yield return new WaitForSeconds(m_aniLength - m_aniCutFrameLength);
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        if (World.Instance.IsPause || mbStartAttack)
        {
            if(World.Instance.IsPause && !mbStartAttack)
            {
                OnCancel();
            }

            return;
        }

        if (m_chargeCount > 0)
        {
            m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.ChargeEnd);
            m_aniLength = m_owner.PlayAniImmediate(eAnimation.ChargeEnd);

            //m_chargeCount += (int)mValue1;
        }

        m_endUpdate = true;
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.CloneRushAttack);
        if (evt == null)
        {
            Debug.LogError("CloneRushAttack 공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError("CloneRushAttack Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }
}
