
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ActionMoveToTarget : ActionEnemyBase
{
    private UnitCollider    mTargetCollider     = null;
    private bool            mAvoid              = false;
    private Vector2         mOriginColliderSize = Vector2.zero;


	public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.MoveToTarget;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mParamAI = param as ActionParamAI;

        if (m_owner.aniEvent.HasAni(eAnimation.RunFast))
        {
            m_owner.PlayAni(eAnimation.RunFast);
        }
        else
        {
            m_owner.PlayAni(eAnimation.Run);
        }

        mTargetCollider = m_owner.GetMainTargetCollider(false);
        mAvoid = false;

        if ( m_owner is PlayerGuardian ) {
			mOriginColliderSize.x = m_owner.MainCollider.radius;
			mOriginColliderSize.y = m_owner.MainCollider.height;

            m_owner.MainCollider.SetRadius( 0.0f );
			m_owner.MainCollider.SetHeight( 0.0f );

            m_owner.SetSpeedRate( 0.5f );
		}
    }

    public override IEnumerator UpdateAction()
    {
        if (mTargetCollider == null)
        {
            OnEnd();
            yield break;
        }

        LookAtTarget(mTargetCollider.Owner);

        Vector3 targetPos = mTargetCollider.Owner.transform.position;
        int targetDirIndex = 0;
        Vector3 targetDir = m_owner.GetTargetDirection(mTargetCollider.Owner, ref targetDirIndex);
        Vector3 dest = Vector3.zero;
        Vector3 dir = dest - m_owner.transform.position;
        Vector3 beforeDir = dir;
        HashSet<int> listReturnTargetIndex = new HashSet<int>();

        //Enemy enemy = m_owner as Enemy;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            //if (m_target == null)
            if(object.ReferenceEquals(mTargetCollider.Owner, null) || (!mTargetCollider.Owner.IsShowMesh && mTargetCollider.Owner.TemporaryInvincible))
            {
                OnEnd();
                yield break;
            }

            Vector3 rayPos = m_owner.transform.position;
            rayPos.y = m_owner.GetCenterPos().y;
            RaycastHit hitInfo;

            UnitCollider mainCollider = m_owner.MainCollider;
            if(mainCollider == null && m_owner.cloneOwner)
            {
                mainCollider = m_owner.cloneOwner.MainCollider;
            }

            if (!mAvoid)
            {
                float minDist = (mainCollider.radius + mTargetCollider.Owner.MainCollider.radius);
                float compare = minDist;
                if (mParamAI.distCommand != eActionCommand.None)
                {
                    ActionBase action = m_owner.actionSystem.GetAction(mParamAI.distCommand);
                    if (action)
                    {
                        compare = Mathf.Max(minDist, action.GetAtkRange());
                    }
                    else
                    {
                        compare = minDist;
                    }
                }

                targetPos = mTargetCollider.Owner.transform.position;
                targetPos.y = m_owner.transform.position.y;

                Vector3 p = Vector3.zero;
                if (mParamAI.posByTargetDir)
                    p = m_owner.GetTargetDirection(mTargetCollider.Owner, targetDirIndex);

                dest = targetPos + (p * Mathf.Max(1.0f, minDist));
                float dist = Vector3.Distance(m_owner.transform.position, dest);

                if(dist > compare && mTargetCollider.Owner.ListCollider.Count > 1)
                {
                    targetPos = m_owner.GetTargetCapsuleEdgePos(mTargetCollider.Owner);
                    targetPos.y = m_owner.transform.position.y;

                    dest = targetPos + (p * Mathf.Max(1.0f, minDist));
                    dist = Vector3.Distance(m_owner.transform.position, dest);
                }

                float angle = 0.0f;
                if (mParamAI.posByTargetDir)
                    angle = Vector3.Angle((dest - targetPos).normalized, (m_owner.transform.position - targetPos).normalized);

                if (dist <= compare && angle <= 10.0f)
                {
                    if (mTargetCollider.Owner.isGrounded == true)
                        LookAtTarget(mTargetCollider.Owner);

                    m_endUpdate = true;

                    foreach (int idx in listReturnTargetIndex)
                        m_owner.ReturnTargetDirection(mTargetCollider.Owner, idx);

                    m_owner.ReturnTargetDirection(mTargetCollider.Owner, targetDirIndex);
                }
                else
                {
                    if (Physics.Raycast(rayPos, m_owner.transform.forward, out hitInfo, mainCollider.radius * 2.0f,
                                        /*1 << (int)eLayer.Wall_Inside |*/ 1 << (int)eLayer.EnvObject | 1 << (int)eLayer.Player) == true)
                    {
                        if (hitInfo.transform.gameObject.layer == (int)eLayer.EnvObject)
                        {
                            m_endUpdate = true;

                            foreach (int idx in listReturnTargetIndex)
                                m_owner.ReturnTargetDirection(mTargetCollider.Owner, idx);

                            m_owner.ReturnTargetDirection(mTargetCollider.Owner, targetDirIndex);
                        }
                        else
                        {
                            listReturnTargetIndex.Add(targetDirIndex);
                            targetDir = m_owner.GetTargetDirection(mTargetCollider.Owner, ref targetDirIndex);

                            //dir += hitInfo.normal;
                            dir = UnityEngine.Random.Range(0, 2) == 0 ? m_owner.transform.right : -m_owner.transform.right;
                            m_owner.cmptRotate.UpdateRotation(dir, true);

                            beforeDir = dir;

                            mAvoid = true;
                        }
                    }
                    else
                    {
                        dir = dest - m_owner.transform.position;

                        if (dir != beforeDir)
                            m_owner.cmptRotate.UpdateRotation(dir, false);

                        beforeDir = dir;
                    }

                    if (m_owner.aniEvent.HasAni(eAnimation.RunFast))
                    {
                        if (m_owner.aniEvent.curAniType != eAnimation.RunFast)
                        {
                            m_owner.PlayAni(eAnimation.RunFast);
                        }
                    }
                    else
                    {
                        if (m_owner.aniEvent.curAniType != eAnimation.Run)
                        {
                            m_owner.PlayAni(eAnimation.Run);
                        }
                    }

                    m_owner.cmptMovement.UpdatePosition(m_owner.transform.forward, m_owner.speed, true);
                }
            }
            else
            {
                /*Vector3 lookAtTarget = (mTargetCollider.Owner.transform.position - transform.position).normalized;
                if (Physics.Raycast(rayPos, lookAtTarget, out hitInfo, m_owner.MainCollider.radius, (1 << (int)eLayer.EnvObject) | (1 << (int)eLayer.Wall)))
                {
                }
                else*/
                {
                    LookAtTarget(mTargetCollider.Owner);
                    mAvoid = false;
                }

                if (m_owner.aniEvent.HasAni(eAnimation.RunFast))
                {
                    if (m_owner.aniEvent.curAniType != eAnimation.RunFast)
                    {
                        m_owner.PlayAni(eAnimation.RunFast);
                    }
                }
                else
                {
                    if (m_owner.aniEvent.curAniType != eAnimation.Run)
                    {
                        m_owner.PlayAni(eAnimation.Run);
                    }
                }

                m_owner.cmptMovement.UpdatePosition(m_owner.transform.forward, m_owner.speed, true);
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnEnd()
    {
        m_endUpdate = true;
        m_owner.cmptMovement.UpdatePosition(m_owner.transform.forward, m_owner.speed, true);

		if ( m_owner is PlayerGuardian ) {
			m_owner.MainCollider.SetRadius( mOriginColliderSize.x );
			m_owner.MainCollider.SetHeight( mOriginColliderSize.y );

			mOriginColliderSize = Vector2.zero;
		}

        if ( m_owner is PlayerGuardian ) {
			m_owner.RestoreSpeed();
		}

		base.OnEnd();
    }

	public override void OnCancel() {
		m_endUpdate = true;
		m_owner.cmptMovement.UpdatePosition( m_owner.transform.forward, m_owner.speed, true );

		if ( m_owner is PlayerGuardian ) {
			m_owner.MainCollider.SetRadius( mOriginColliderSize.x );
			m_owner.MainCollider.SetHeight( mOriginColliderSize.y );

			mOriginColliderSize = Vector2.zero;
		}

		if ( m_owner is PlayerGuardian ) {
			m_owner.RestoreSpeed();
		}

		base.OnCancel();
	}
}
