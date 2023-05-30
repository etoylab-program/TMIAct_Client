
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionUpperJump : ActionSelectSkillBase
{
    public enum eState
    {
        Start = 0,
        Doing,
        End,
    }


    protected ActionParamUpperJump  mParamUpperJump         = null;
    protected State                 mState                  = new State();
    protected eAnimation            mStartAni               = eAnimation.None;
    protected float                 mStartAniNormalizedTime = 0.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        actionCommand = eActionCommand.UpperJump;
        IsRepeatSkill = true;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Grounded;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.JumpAttack;

        superArmor = Unit.eSuperArmor.Lv1;

		mState.Init(3);
        mState.Bind(eState.Start, ChangeStartState);
        mState.Bind(eState.Doing, ChangeDoingState);
        mState.Bind(eState.End, ChangeEndState);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        m_owner.StopStepForward();

        mParamUpperJump = param as ActionParamUpperJump;
        if (mParamUpperJump == null)
        {
            m_endUpdate = true;
            return;
        }

        ExecuteAction = mParamUpperJump.ExecuteAction;
        m_owner.cmptJump.StartJump(mParamUpperJump.jumpPower, mParamUpperJump.SlowFalling, mParamUpperJump.SlowFallingDuration);

        mStartAniNormalizedTime = m_owner.aniEvent.GetCutFrameNomalizedTime(GetStartAni());
        mState.ChangeState(eState.Start, true);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            switch ((eState)mState.current)
            {
                case eState.Start:
                    bool isFalling = false;
					if (mParamUpperJump.checkFallingType == ActionParamUpperJump.eCheckFallingType.Animation)
					{
						isFalling = m_owner.aniEvent.IsAniPlaying(GetStartAni()) == eAniPlayingState.End;
					}
					else
						isFalling = m_owner.isFalling;

                    if(isFalling)
                        mState.ChangeState(eState.Doing, true);
                    break;

                case eState.Doing:
                    if (m_owner.isGrounded == true)
                        mState.ChangeState(eState.End, true);
                    break;

                case eState.End:
                    if (m_owner.aniEvent.IsAniPlaying(eAnimation.Jump03) == eAniPlayingState.End)
                        m_endUpdate = true;
                    break;
            }

            m_owner.cmptJump.UpdateJump();

            if ((eState)mState.current != eState.End)
            {
                m_owner.cmptMovement.UpdatePosition(Vector3.zero, 0.0f, true);
                m_owner.cmptRotate.UpdateRotation(Vector3.zero, false);
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        OnStart(param);
    }

    public override void OnEnd()
    {
        base.OnEnd();

        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);

        if (mParamUpperJump.hideMeshOnJumping)
        {
            m_owner.ShowMesh(true);
        }

        if ((eState)mState.current == eState.End && m_owner.Input.GetDirection() != Vector3.zero)
        {
            m_owner.PlayAni(eAnimation.Run);
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();

        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);

        if ((eState)mState.current == eState.End && m_owner.Input.GetDirection() != Vector3.zero)
        {
            m_owner.PlayAni(eAnimation.Run);
        }
    }

    protected virtual eAnimation GetStartAni()
    {
        return eAnimation.UpperAttack01;
    }

    private bool ChangeStartState(bool changeAni)
    {
        Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);
        Utility.IgnorePhysics(eLayer.Player, eLayer.EnemyGate);

        if (mParamUpperJump.hideMeshOnJumping)
        {
            m_owner.ShowMesh(false);
        }
        else
        {
			m_owner.PlayAniImmediate(GetStartAni(), mStartAniNormalizedTime);
        }

		return true;
    }

    private bool ChangeDoingState(bool changeAni)
    {
        if (mParamUpperJump.hideMeshOnJumping)
            m_owner.ShowMesh(true);

        m_owner.PlayAniImmediate(eAnimation.Jump02);
        return true;
    }

    private bool ChangeEndState(bool changeAni)
    {
        m_owner.PlayAniImmediate(eAnimation.Jump03);
        return true;
    }
}
