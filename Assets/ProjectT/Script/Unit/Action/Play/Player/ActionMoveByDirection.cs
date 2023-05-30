
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ActionMoveByDirection : ActionBase
{
    [Header("[Set Animation]")]
    public eAnimation moveAni = eAnimation.Run;
    public eAnimation walkAni = eAnimation.Walk;
    public eAnimation stopAni = eAnimation.None;

    public bool SkipStopAni { get; set; }
    public bool isSprint    { get; protected set; }

    protected Player                      mOwnerPlayer                = null;
    protected ActionParamMoveByDirection  mParamMoveByDir             = null;
    protected eAnimation                  mOriginalMoveAni            = eAnimation.Run;
    protected eAnimation                  mOriginalWalkAni            = eAnimation.Walk;
    protected Vector3                     mDir                        = Vector3.zero;
    protected bool                        mbCameraAnimationInPlayer   = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.MoveByDirection;

        extraCondition = new eActionCondition[2];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;

        cancelActionCommand = new eActionCommand[2];
        cancelActionCommand[0] = eActionCommand.Jump;
        cancelActionCommand[1] = eActionCommand.Defence;

        extraCancelCondition = new eActionCondition[3];
        extraCancelCondition[0] = eActionCondition.UseSkill;
        extraCancelCondition[1] = eActionCondition.UseQTE;
        extraCancelCondition[2] = eActionCondition.UseUSkill;

        mOwnerPlayer = m_owner as Player;

        SkipStopAni = false;

        mOriginalMoveAni = moveAni;
        mOriginalWalkAni = walkAni;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        if(mOwnerPlayer && !mOwnerPlayer.IsMissionStart)
        {
            m_endUpdate = true;
            return;
        }

        mParamMoveByDir = param as ActionParamMoveByDirection;

        isSprint = true;
        mbCameraAnimationInPlayer = World.Instance.InGameCamera.IsAnimationPlayingInPlayer();

        ActionBase currentAction = m_owner.actionSystem.currentAction;
        m_aniLength = 0.0f;

        m_owner.rigidBody.velocity = Vector3.zero;

        if (mParamMoveByDir.Dir != Vector3.zero && m_owner.aniEvent.HasAni(eAnimation.Run) && 
            (currentAction == null || currentAction.actionCommand != eActionCommand.Jump) && !mbCameraAnimationInPlayer)
        {
            m_aniLength = m_owner.PlayAniImmediate(moveAni);
        }
    }

    public override IEnumerator UpdateAction()
    {
        if(m_endUpdate)
        {
            yield break;
        }

        Vector3 inputDir = Vector3.zero;
        float slowValue = ((InputController.MIN_MOVE_DISTANCE * 0.5f) / (InputController.MAX_THUMB_RADIUS));
        float runningTime = 0.0f;
        float walkRatio = 0.5f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        //while (m_owner.Input.GetRawDirection() != Vector3.zero && !mbCameraAnimationInPlayer)
        while (mParamMoveByDir.Dir != Vector3.zero && !mbCameraAnimationInPlayer)
        {
            if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
            {
                SetDirection(ref inputDir, ref walkRatio, slowValue);
            }
            else
            {
                SetDirectionInPVPMode();
                walkRatio = 1.0f;
            }

            m_owner.cmptRotate.UpdateRotation(mDir, true);//, 0.3f);
            m_owner.cmptMovement.UpdatePosition(mDir, m_owner.speed * walkRatio, false);

            /*runningTime += m_owner.fixedDeltaTime;
            if(!isSprint && runningTime >= GameInfo.Instance.BattleConfig.SprintStartConditionTime * m_owner.sprintStartTimeRate)
            {
                isSprint = true;
                World.Instance.playerCam.SetSprintMode(true);

                m_aniLength = m_owner.PlayAniImmediate(sprintAni);
                m_owner.SetSpeedRateBySprint(GameInfo.Instance.BattleConfig.SprintSpeedRatio);
            }*/

            yield return mWaitForFixedUpdate;
        }

        if(!SkipStopAni)
        {
            SkipStopAni = mParamMoveByDir == null ? false : mParamMoveByDir.SkipRunStop;
        }

        if (!SkipStopAni && m_aniLength > 0.0f && isSprint && stopAni != eAnimation.None && m_owner.aniEvent.HasAni(stopAni) && !mbCameraAnimationInPlayer)
        {
            isSprint = false;
            //World.Instance.playerCam.SetSprintMode(false);

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
            yield return new WaitForEndOfFrame(); // Run에서 바로 Idle을 넘겨버리면 블렌딩이 꼬임
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        mParamMoveByDir = param as ActionParamMoveByDirection;
    }

    public override void OnCancel()
    {
        OnEnd();

        m_owner.rigidBody.velocity = Vector3.zero;
        m_owner.PlayAniImmediate(eAnimation.Idle01);//, false, 0.0f, 0.0f); // 다른 애니로 바꿔야 루핑 사운드 사라짐
    }

    public override void OnEnd()
    {
        base.OnEnd();

        m_owner.rigidBody.velocity = Vector3.zero;
        m_owner.SetSpeedRateBySprint(0.0f);

        isSprint = false;
        //World.Instance.playerCam.SetSprintMode(false);
    }

    public void RestoreMoveAni()
    {
        moveAni = mOriginalMoveAni;
        walkAni = mOriginalWalkAni;
    }

    protected virtual void SetDirection(ref Vector3 inputDir, ref float walkRatio, float slowValue)
    {
        bool isAI = false;
        inputDir = mParamMoveByDir.Dir;//m_owner.Input.GetRawDirection();

		if( mOwnerPlayer && ( mOwnerPlayer.IsAutoMove || mOwnerPlayer.AI ) ) {
			mDir = inputDir;
            isAI = true;
        }
		else
        {
            mDir = Vector3.zero;

            Vector3 cameraRight = World.Instance.InGameCamera.transform.right;
            Vector3 cameraForward = World.Instance.InGameCamera.transform.forward;

            if (World.Instance.InGameCamera.Mode != InGameCamera.EMode.SIDE || World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.None)
			{
				mDir.x = (inputDir.x * cameraRight.x) + (inputDir.y * cameraForward.x);
				mDir.z = (inputDir.x * cameraRight.z) + (inputDir.y * cameraForward.z);
			}
			else
            {
                if (World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.X)
                {
                    mDir.z = (inputDir.x * -cameraForward.x);
                }
                else if (World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.Z)
                {
                    mDir.x = (inputDir.x * cameraForward.z);
                }
            }

            mDir.y = 0.0f;
        }

		if( isAI || ( ( Mathf.Abs( inputDir.x ) >= slowValue ) || ( Mathf.Abs( inputDir.y ) >= slowValue ) ) ) {
			if( !isSprint ) {
				m_aniLength = m_owner.PlayAniImmediate( moveAni );
			}

			isSprint = true;
			walkRatio = 1.0f;
		}
		else {
			if( isSprint ) {
				m_aniLength = m_owner.PlayAniImmediate( walkAni );
			}

			isSprint = false;
			walkRatio = GameInfo.Instance.BattleConfig.PlayerWalkSpeedRatio;
		}
	}

    protected void SetDirectionInPVPMode()
    {
        mDir = mParamMoveByDir.Dir;//m_owner.Input.GetRawDirection();
        mDir.y = 0.0f;

        if (!isSprint)
        {
            m_aniLength = m_owner.PlayAniImmediate(moveAni);
            isSprint = true;
        }
    }
}
