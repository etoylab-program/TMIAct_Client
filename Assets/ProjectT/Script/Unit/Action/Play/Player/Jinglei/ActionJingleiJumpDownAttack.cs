
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionJingleiJumpDownAttack : ActionSelectSkillBase
{
    public enum eState
    {
        StartAttack,
        Jump,
        Falling,
        FinishAttack,
    }

    
    private State mState = new State();
    private float mDragFrameLength;
    private AniEvent.sEvent mDragAttackEvent;



    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.HoldingDefBtnAttack;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        superArmor = Unit.eSuperArmor.Lv1;

		mState.Init(4);
        mState.Bind(eState.StartAttack, ChangeStartAttackState);
        mState.Bind(eState.Jump, ChangeJumpState);
        mState.Bind(eState.Falling, ChangeFallingState);
        mState.Bind(eState.FinishAttack, ChangeFinishAttackState);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        mState.ChangeState(eState.StartAttack, true);
    }

    public override IEnumerator UpdateAction()
    {
        bool fastFall = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate)
        {
            switch((eState)mState.current)
            {
                case eState.StartAttack:
                    m_checkTime += m_owner.fixedDeltaTime;

					if ( mValue2 > 0.0f && mDragAttackEvent != null && m_checkTime >= mDragFrameLength ) {
						Collider[] colliders = Physics.OverlapBox( m_owner.GetCenterPos(), mDragAttackEvent.hitBoxSize * 0.5f, Quaternion.identity, Utility.GetEnemyLayer( eLayer.Player ) );
						for ( int i = 0; i < colliders.Length; i++ ) {
							UnitCollider unitCollider = colliders[i].GetComponent<UnitCollider>();
							if ( unitCollider == null ) {
								continue;
							}

							Unit unit = unitCollider.Owner;
							if ( unit == null ) {
								continue;
							}

							unit.Dragged( m_owner, mValue2, eDragDirection.FRONT );
						}

						mDragFrameLength = float.MaxValue;
					}

                    if (m_checkTime >= m_aniCutFrameLength)
                    {
                        mState.ChangeState(eState.Jump, true);
                    }
                    break;

                case eState.Jump:
                    if (m_owner.isFalling || m_owner.isGrounded)
                    {
                        mState.ChangeState(eState.Falling, true);
                    }
                    break;

                case eState.Falling:
                    if (m_owner.isGrounded)
                    {
                        mState.ChangeState(eState.FinishAttack, true);
                    }
                    break;

                case eState.FinishAttack:
                    m_checkTime += m_owner.fixedDeltaTime;
                    if(m_checkTime >= m_aniLength)
                    {
                        m_endUpdate = true;
                    }
                    break;
            }

            if ((eState)mState.current >= eState.Falling && (eState)mState.current <= eState.FinishAttack)
            {
                if (!fastFall && m_owner.isFalling)
                {
					m_checkTime += m_owner.fixedDeltaTime;
					if ( m_checkTime >= m_aniCutFrameLength ) {
						m_owner.cmptJump.SetFastFall();
						fastFall = true;
					}
                }
            }

            m_owner.cmptJump.UpdateJump();
            yield return mWaitForFixedUpdate;
        }
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.GestureSkill03);
        if (evt == null)
        {
            Debug.LogError("GestureSkill03 공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError("GestureSkill03 Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    public bool ChangeStartAttackState(bool changeAni)
    {
        m_owner.PlayAniImmediate(eAnimation.UpperAttack01);

        m_checkTime = 0.0f;
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.UpperAttack01);

		if ( mValue2 > 0 ) {
			mDragFrameLength = float.MaxValue;
            if ( mDragAttackEvent == null ) {
				mDragAttackEvent = m_owner.aniEvent.GetEvent( eBehaviour.Attack );
			}
			
			if ( mDragAttackEvent != null ) {
				mDragFrameLength = m_owner.aniEvent.GetFrameLength( eAnimation.UpperAttack01, mDragAttackEvent.frame );
			}
		}

        return true;
    }

    public bool ChangeJumpState(bool changeAni)
    {
        m_checkTime = 0.0f;
		m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( eAnimation.PrepareGestureSkill03 );
		m_owner.cmptJump.StartJump( m_owner.cmptJump.m_jumpPower, true, m_aniCutFrameLength );
		return true;
    }

    public bool ChangeFallingState(bool changeAni)
    {
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

        m_owner.PlayAniImmediate(eAnimation.PrepareGestureSkill03);

        return true;
    }

    public bool ChangeFinishAttackState(bool changeAni)
    {
        eAnimation curAni = eAnimation.GestureSkill03;
        if(mValue1 >= 1.0f)
        {
            curAni = eAnimation.GestureSkill03_02;
        }

        AniEvent.sEvent evt = m_owner.aniEvent.GetEventOnEndAction(curAni);
        if (evt != null)
        {
            m_owner.OnAttackOnEndAction(evt);
        }

        m_checkTime = 0.0f;
        m_aniLength = m_owner.PlayAniImmediate(curAni);

        return true;
    }
}
