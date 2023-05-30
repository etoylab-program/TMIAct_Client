
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionJingleiChainAttack : ActionSelectSkillBase
{
    public enum eState
    {
        Start,
        Chain,
        Finish
    }


    private State   mState          = new State();
    private int     mRepeatCount    = 2;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.AttackDuringAttack;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        superArmor = Unit.eSuperArmor.Lv1;

		mState.Init(3);
        mState.Bind(eState.Start, ChangeStartState);
        mState.Bind(eState.Chain, ChangeChainState);
        mState.Bind(eState.Finish, ChangeFinishState);

        if(mValue1 >= 1.0f)
        {
            mRepeatCount += 2;
        }
        
        if(mValue1 >= 2.0f)
        {
            List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent(eAnimation.ComboAttackFinish);
            for (int i = 0; i < listAtkEvt.Count; i++)
            {
                listAtkEvt[i].behaviour = eBehaviour.KnockBackAttack;
            }
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        if (FSaveData.Instance.AutoTargetingSkill)
        {
            Unit target = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
            if (target)
            {
                m_owner.LookAtTarget(target.transform.position);
            }
        }
        else
        {
            m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
        }

        mState.ChangeState(eState.Start, true);
    }

    public override IEnumerator UpdateAction()
    {
        int curRepeatCount = 0;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate)
        {
            switch((eState)mState.current)
            {
                case eState.Start:
                    m_checkTime += m_owner.fixedDeltaTime;
                    if(m_checkTime >= m_aniLength)
                    {
                        mState.ChangeState(eState.Chain, true);
                    }
                    break;

                case eState.Chain:
                    m_checkTime += m_owner.fixedDeltaTime;
                    if (m_checkTime >= m_aniLength)
                    {
                        if (++curRepeatCount < mRepeatCount)
                        {
                            ChangeChainState(true);
                        }
                        else
                        {
                            mState.ChangeState(eState.Finish, true);
                        }
                    }
                    break;

                case eState.Finish:
                    m_checkTime += m_owner.fixedDeltaTime;
                    if (m_checkTime >= m_aniLength)
                    {
                        m_endUpdate = true;
                    }
                    break;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.ComboAttack);
        if (evt == null)
        {
            Debug.LogError("ComboAttack 공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError("ComboAttack Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    private bool ChangeStartState(bool changeAni)
    {
        m_checkTime = 0.0f;
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.ComboAttack);

        return true;
    }

    private bool ChangeChainState(bool changeAni)
    {
		if ( FSaveData.Instance.AutoTargetingSkill ) {
			Unit target = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
			if ( target ) {
				m_owner.LookAtTarget( target.transform.position );
			}
		}
		else {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}

		m_checkTime = 0.0f;
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.ComboAttackRepeat);

        return true;
    }

    private bool ChangeFinishState(bool changeAni)
    {
        m_checkTime = 0.0f;
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.ComboAttackFinish);

        return true;
    }
}
