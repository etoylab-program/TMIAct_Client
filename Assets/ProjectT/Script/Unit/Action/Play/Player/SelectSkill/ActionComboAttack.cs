
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionComboAttack : ActionSelectSkillBase
{
    [Header("[Set Animation]")]
    public eAnimation[] attackAnimations;

    public bool reserved            { get { return m_reserved; } }
    public int  CurrentAttackIndex  { get; protected set; } = 0;

    protected eAnimation        m_currentAni                = eAnimation.None;
    protected eAnimation[]      mOriginalAtkAnis            = null;
    protected int               m_nextAttackIndex           = 0;
    protected bool              m_reserved                  = false;
    protected float             m_atkRange                  = 0.0f;
    protected UnitCollider      mTargetCollider             = null;
    protected bool              m_noneTarget                = false;
    protected List<eBehaviour>  mListBehaviour              = new List<eBehaviour>();
    //protected bool              mbAlreadySuperArmor = false;
    protected bool              mForceLastAttackCutFrame    = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Attack01;

        conditionActionCommand = new eActionCommand[1];
        conditionActionCommand[0] = eActionCommand.MoveByDirection;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Grounded;

        cancelActionCommand = new eActionCommand[2];
        cancelActionCommand[0] = eActionCommand.Defence;
        cancelActionCommand[1] = eActionCommand.Jump;

        extraCancelCondition = new eActionCondition[3];
        extraCancelCondition[0] = eActionCondition.UseSkill;
        extraCancelCondition[1] = eActionCondition.UseQTE;
        extraCancelCondition[2] = eActionCondition.UseUSkill;

        if (attackAnimations != null)
        {
            mOriginalAtkAnis = new eAnimation[attackAnimations.Length];
            for (int i = 0; i < attackAnimations.Length; i++)
            {
                mOriginalAtkAnis[i] = attackAnimations[i];
            }
        }

		mForceLastAttackCutFrame = false;
		mOwnerPlayer.AutoGuardianSkill = false;
	}

    public virtual bool IsLastAttack()
    {
        if (CurrentAttackIndex == attackAnimations.Length - 1)
            return true;

        return false;
    }

    public virtual void RestoreAttackAnimations()
    {
        attackAnimations = new eAnimation[mOriginalAtkAnis.Length];
        for (int i = 0; i < mOriginalAtkAnis.Length; i++)
        {
            attackAnimations[i] = mOriginalAtkAnis[i];
        }
    }

    protected virtual eAnimation GetCurAni()
    {
        if (m_owner.onlyLastAttack)
            return attackAnimations[attackAnimations.Length - 1];

        if (IsLastAttack() == true && m_owner.lastAttackKnockBack)
        {
            mListBehaviour.Clear();

            List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent(attackAnimations[CurrentAttackIndex]);
            for (int i = 0; i < listAtkEvt.Count; i++)
            {
                mListBehaviour.Add(listAtkEvt[i].behaviour);
                listAtkEvt[i].behaviour = eBehaviour.KnockBackAttack;
            }
        }

        return attackAnimations[CurrentAttackIndex];
    }

    protected virtual void StartAttack()
    {
        m_currentAni = GetCurAni();
        m_aniLength = m_owner.PlayAniImmediate(m_currentAni);

        m_atkRange = m_owner.aniEvent.GetFirstAttackEventRange(m_currentAni);
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(m_currentAni);

        if (mLastAtkSuperArmor > Unit.eSuperArmor.None && IsLastAttack())
        {
            /*
            mbAlreadySuperArmor = false;
            if (m_owner.CurrentSuperArmor >= mLastAtkSuperArmor)
            {
                mbAlreadySuperArmor = true;
            }
            */
            mChangedSuperArmorId = m_owner.SetSuperArmor(mLastAtkSuperArmor);
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        
        StartAttack();
        LookAtTarget();
    }

	public override IEnumerator UpdateAction() {
        float endAniTime = 0.0f;

		while( m_endUpdate == false ) {
			m_checkTime += m_owner.fixedDeltaTime;

            if( ( m_owner.AI && !IsLastAttack() ) || m_nextAttackIndex > CurrentAttackIndex || ( IsLastAttack() && ( mOwnerPlayer.UseCutframeForLastComboAttack || mForceLastAttackCutFrame ) ) ) {
                endAniTime = m_owner.aniEvent.GetCutFrameLength( m_currentAni );
            }
            else {
                endAniTime = m_owner.aniEvent.GetAniLength( m_currentAni );
            }

			if( ( m_owner.AI && !IsLastAttack() ) || m_nextAttackIndex > CurrentAttackIndex ) {
				if( m_checkTime >= m_owner.aniEvent.GetCutFrameLength( m_currentAni ) ) {
					if( m_owner.AI ) {
						bool skip = false;
						if( m_owner.mainTarget ) {
							float dist = Utility.GetDistanceWithoutY(m_owner.transform.position, m_owner.mainTarget.transform.position);
							if( dist >= GetAtkRange() * 1.5f ) {
								dist = Utility.GetDistanceWithoutY( m_owner.transform.position, m_owner.GetTargetCapsuleEdgePos( m_owner.mainTarget ) );
								if( dist >= GetAtkRange() * 1.5f ) {
									skip = true;
								}
							}
						}

						if( !skip ) {
							OnUpdating( null );
						}
					}

					m_endUpdate = true;
				}
			}
			else if( m_checkTime >= endAniTime ) {
                m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnUpdating(IActionBaseParam param)
    {
        if (m_reserved)
        {
            return;
        }

        m_reserved = true;

        if (IsLastAttack() == true)
        {
            m_nextAttackIndex = CurrentAttackIndex;
        }
        else
        {
            m_nextAttackIndex = CurrentAttackIndex + 1;
        }
    }

    public override void OnEnd()
    {
        isPlaying = false;

        if (mListBehaviour.Count > 0 && m_owner.lastAttackKnockBack && IsLastAttack())
        {
            List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent(attackAnimations[CurrentAttackIndex]);
            for (int i = 0; i < listAtkEvt.Count; i++)
            {
                listAtkEvt[i].behaviour = mListBehaviour[i];
            }
        }

        /*
        if (m_owner.CurrentSuperArmor <= mLastAtkSuperArmor && IsLastAttack())
        {
            //m_owner.RestoreSuperArmor(mLastAtkSuperArmor, GetType());
            m_owner.RestoreSuperArmor(mChangedSuperArmorId);
        }
        */

        if (m_nextAttackIndex == CurrentAttackIndex)
        {
            if(mOwnerPlayer.CancelComboAttack)
            {
                ResetComboAttackIndex();
            }
            else
            {
                ContinueComboAttackIndex();
            }

            base.OnEnd();
        }
        else
        {
            CurrentAttackIndex = m_nextAttackIndex;
            m_nextAction = eActionCommand.Attack01;
        }

        m_reserved = false;

        /*
        if (mLastAtkSuperArmor > Unit.eSuperArmor.None && !mbAlreadySuperArmor)
        {
            m_owner.SetSuperArmor(Unit.eSuperArmor.None, GetType());
        }
        */

        m_owner.RestoreSuperArmor(mChangedSuperArmorId);
    }

    public override void OnCancel()
    {
        IsSkillEnd = true;

        isPlaying = false;
        isCancel = true;

        if (mOwnerPlayer.CancelComboAttack)
        {
            ResetComboAttackIndex();
        }
        else
        {
            ContinueComboAttackIndex();
        }

        m_reserved = false;

        /*
        if (mLastAtkSuperArmor > Unit.eSuperArmor.None && !mbAlreadySuperArmor && m_owner.CurrentSuperArmor <= mLastAtkSuperArmor)
        {
            m_owner.RestoreSuperArmor(mLastAtkSuperArmor, GetType());
        }
        else
        {
            m_owner.RestoreSuperArmor(superArmor, GetType());
        }
        */

        m_owner.RestoreSuperArmor(mChangedSuperArmorId);
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(attackAnimations[0]);
        if(evt == null)
        {
            return 0.0f;
        }

        if( evt.visionRange == 0.0f && evt.behaviour == eBehaviour.Projectile ) {
            return 10.0f;
		}

        return evt.visionRange;
    }

    public override float GetCurAniCutFrameLength()
    {
        eAnimation atkAni = m_currentAni;
        if (atkAni == eAnimation.None)
        {
            atkAni = attackAnimations[0];
        }

        return m_owner.aniEvent.GetCutFrameLength(atkAni);
    }

    public virtual void LookAtTarget()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetNextAttackEvent(m_currentAni);
        if (evt != null)
        {
            m_noneTarget = true;

            float checkDist = evt.atkRange;
            mTargetCollider = FSaveData.Instance.AutoTargeting ? m_owner.GetMainTargetCollider(false, checkDist) : null;
            if (mTargetCollider)
            {
                m_noneTarget = false;
                LookAtTarget(mTargetCollider.Owner);
            }
            else if (m_owner.Input != null)
            {
                m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
            }

            /*
            if (mTargetCollider != null && mTargetCollider.Owner.isVisible == false)
            {
                Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(mTargetCollider.Owner.transform.position);
                Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(Camera.main.transform.position);

                Vector3 side = mTargetCollider.Owner.transform.right;
                if (targetScreenPos.x > playerScreenPos.x)
                    side = -mTargetCollider.Owner.transform.right;

                if (mTargetCollider.Owner.gameObject.layer == (int)eLayer.Enemy)
                {
                    World.Instance.InGameCamera.TurnToTarget(mTargetCollider.Owner.transform.position + (side * 1.25f), 2.0f);
                }
            }
            */
        }
    }

    public void ResetComboAttackIndex()
    {
        CurrentAttackIndex = 0;
        m_nextAttackIndex = 0;
    }

    public void ContinueComboAttackIndex()
    {
        if (IsLastAttack())
        {
            ResetComboAttackIndex();
        }
        else
        {
            ++CurrentAttackIndex;
            m_nextAttackIndex = CurrentAttackIndex;
        }
    }
}
