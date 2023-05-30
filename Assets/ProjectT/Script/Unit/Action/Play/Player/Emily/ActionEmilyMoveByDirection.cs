
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ActionEmilyMoveByDirection : ActionMoveByDirection
{
    private Emily				mOwnerEmily			= null;
	private WaitForEndOfFrame	mWaitForEndOfFrame	= new WaitForEndOfFrame();


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        mOwnerEmily = m_owner as Emily;
    }

    public override void OnStart(IActionBaseParam param)
    {
        isPlaying = true;

        m_param = param;

        m_endUpdate = false;
        m_checkTime = 0.0f;

        isCancel = false;

        mChangedSuperArmorId = m_owner.SetSuperArmor(superArmor);

        if (OnStartCallback != null)
            OnStartCallback();

        mParamMoveByDir = param as ActionParamMoveByDirection;

        if ((mOwnerPlayer && !mOwnerPlayer.IsMissionStart) || mParamMoveByDir.Dir == Vector3.zero)
        {
            m_endUpdate = true;
            return;
        }

        isSprint = false;
        mbCameraAnimationInPlayer = World.Instance.InGameCamera.IsAnimationPlayingInPlayer();

        /*
        ActionBase currentAction = m_owner.actionSystem.currentAction;
        m_aniLength = 0.0f;
        if (m_owner.aniEvent.HasAni(eAnimation.Run) == true && (currentAction == null || currentAction.actionCommand != eActionCommand.Jump) && !mbCameraAnimationInPlayer)
            m_aniLength = m_owner.PlayAniImmediate(moveAni);
            */
    }

    public override IEnumerator UpdateAction()
    {
        if(m_endUpdate)
        {
            m_owner.actionSystem.CancelCurrentAction();
            yield break;
        }

        Vector3 inputDir = Vector3.zero;
        float slowValue = ((InputController.MIN_MOVE_DISTANCE * 0.5f) / (InputController.MAX_THUMB_RADIUS));
        float walkRatio = 0.5f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (mParamMoveByDir != null && mParamMoveByDir.Dir != Vector3.zero && !mbCameraAnimationInPlayer)
        {
            if (mOwnerEmily.IsDroneDefaultAttack)
            {
                mOwnerEmily.CheckTime += m_owner.fixedDeltaTime;
                if (mOwnerEmily.CheckTime >= m_aniLength)
                {
                    ++mOwnerEmily.CurAtkAniIndex;
                    if (mOwnerEmily.CurAtkAniIndex >= mOwnerEmily.DefaultAttackAnis.Length)
                    {
                        mOwnerEmily.CurAtkAniIndex = mOwnerEmily.DefaultAttackAnis.Length - 1;
                    }

                    m_aniLength = m_owner.PlayAni(mOwnerEmily.DefaultAttackAnis[mOwnerEmily.CurAtkAniIndex]);
                    mOwnerEmily.CheckTime = 0.0f;
                }
            }

            if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
            {
                SetDirection(ref inputDir, ref walkRatio, slowValue);
            }
            else
            {
                SetDirectionInPVPMode();
                walkRatio = 1.0f;
            }

            if (mOwnerEmily.IsDroneDefaultAttack)
            {
                if (mOwnerEmily.Drone.mainTarget)
                {
                    Vector3 lookAt = Utility.GetDirWithoutY(mOwnerEmily.Drone.mainTarget.transform.position, transform.position); //(mOwnerEmily.Drone.mainTarget.transform.position - transform.position).normalized;
                    m_owner.cmptRotate.UpdateRotation(lookAt, true);
                }
                else
                {
                    m_owner.cmptRotate.UpdateRotation(mDir, true);
                }
            }
            else
            {
                m_owner.cmptRotate.UpdateRotation(mDir, true);
            }

            m_owner.cmptMovement.UpdatePosition(mDir, m_owner.speed * walkRatio, false);
            yield return mWaitForFixedUpdate;
        }

        if (!SkipStopAni)
        {
            SkipStopAni = mParamMoveByDir == null ? false : mParamMoveByDir.SkipRunStop;
        }

        if (!SkipStopAni && m_aniLength > 0.0f && isSprint && stopAni != eAnimation.None && m_owner.aniEvent.HasAni(stopAni) && !mbCameraAnimationInPlayer)
        {
            isSprint = false;

            m_owner.PlayAni(stopAni);
            float cutFrameLength = m_owner.aniEvent.GetCutFrameLength(stopAni);

            m_endUpdate = false;

            float t = 0.0f;
            while (m_endUpdate == false)
            {
                if (m_owner.aniEvent.IsAniPlaying(stopAni) == eAniPlayingState.End || m_owner.Input.GetDirection() != Vector3.zero)
                    m_endUpdate = true;

                mDir = Vector3.Lerp(mDir, Vector3.zero, t);
                t += (m_owner.fixedDeltaTime / cutFrameLength);

                m_owner.cmptRotate.UpdateRotation(mDir, true);
                m_owner.cmptMovement.UpdatePosition(mDir, m_owner.speed * 0.65f, false);

                yield return mWaitForFixedUpdate;
            }
        }
        else
        {
            isSprint = false;
            yield return mWaitForEndOfFrame; // Run에서 바로 Idle을 넘겨버리면 블렌딩이 꼬임
        }
    }

	protected override void SetDirection( ref Vector3 inputDir, ref float walkRatio, float slowValue ) {
		inputDir = mParamMoveByDir.Dir;

		if( mOwnerPlayer && ( mOwnerPlayer.IsAutoMove || mOwnerPlayer.AI ) ) {
			mDir = inputDir;
		}
		else {
			mDir = Vector3.zero;

			Vector3 cameraRight = World.Instance.InGameCamera.transform.right;
			Vector3 cameraForward = World.Instance.InGameCamera.transform.forward;

			if( World.Instance.InGameCamera.Mode != InGameCamera.EMode.SIDE || World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.None ) {
				mDir.x = ( inputDir.x * cameraRight.x ) + ( inputDir.y * cameraForward.x );
				mDir.z = ( inputDir.x * cameraRight.z ) + ( inputDir.y * cameraForward.z );
			}
			else {
				if( World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.X ) {
					mDir.z = ( inputDir.x * -cameraForward.x );
				}
				else if( World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.Z ) {
					mDir.x = ( inputDir.x * cameraForward.z );
				}
			}

			mDir.y = 0.0f;
		}

		if( mOwnerEmily.IsDroneDefaultAttack ) {
			m_aniLength = m_owner.PlayAni( mOwnerEmily.DefaultAttackAnis[mOwnerEmily.CurAtkAniIndex] );

			isSprint = false;
			walkRatio = GameInfo.Instance.BattleConfig.EmilySpeedAtAttack;
		}
		else {
			mOwnerEmily.CurAtkAniIndex = 0;

			if( mOwnerEmily.IsAutoMove || ( ( Mathf.Abs( inputDir.x ) >= slowValue ) || ( Mathf.Abs( inputDir.y ) >= slowValue ) ) ) {
				if( !isSprint ) {
					m_aniLength = m_owner.PlayAniImmediate( moveAni );
				}

				isSprint = true;
				walkRatio = 1.0f;
			}
			else {
				if( isSprint || m_owner.aniEvent.IsAniPlaying( walkAni ) != eAniPlayingState.Playing ) {
					m_aniLength = m_owner.PlayAniImmediate( walkAni );
				}

				isSprint = false;
				walkRatio = GameInfo.Instance.BattleConfig.PlayerWalkSpeedRatio;
			}
		}
	}

	public override void OnCancel()
    {
        base.OnCancel();
        isSprint = false;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        isSprint = false;
    }
}
