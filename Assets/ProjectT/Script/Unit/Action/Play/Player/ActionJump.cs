
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionJump : ActionBase
{
    public enum eState
    {
        Start = 0,
        Jumping,
        End,
    }


    private ActionParamAcrossJump   mParam  = null;
    private State                   mState  = new State();


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Jump;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Grounded;

		mState.Init(3);
        mState.Bind(eState.Start, ChangeStartState);
        mState.Bind(eState.Jumping, ChangeJumpingState);
        mState.Bind(eState.End, ChangeEndState);
    }

    public override void OnStart(IActionBaseParam param)
    {
        if (param == null)
        {
            m_endUpdate = true;
            return;
        }

        base.OnStart(param);

        mParam = param as ActionParamAcrossJump;
        mState.ChangeState(eState.Start, true);
    }

    public override IEnumerator UpdateAction()
    {
        float time = 0.0f;
        float t = 0.0f;

        Vector3 low = mParam.PortalEntry.transform.position.y <= mParam.EndPos.y ? mParam.PortalEntry.transform.position : mParam.EndPos;
        Vector3 high = mParam.PortalEntry.transform.position.y >= mParam.EndPos.y ? mParam.PortalEntry.transform.position : mParam.EndPos;
        Vector3 p2 = new Vector3(low.x, high.y * mParam.PortalEntry.JumpWeight, low.z);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            switch((eState)mState.current)
            {
                case eState.Start:
                    m_checkTime += m_owner.fixedDeltaTime;
                    if(m_checkTime >= m_aniLength)
                    {
                        mState.ChangeState(eState.Jumping, true);
                    }
                    break;

                case eState.Jumping:
                    if(t >= 0.99f)
                    {
                        mState.ChangeState(eState.End, true);
                    }
                    break;

                case eState.End:
                    m_checkTime += m_owner.fixedDeltaTime;
                    if (m_checkTime >= m_aniLength)
                    {
                        m_endUpdate = true;
                    }
                    break;
            }

            time += m_owner.fixedDeltaTime / mParam.Duration;
            t = Mathf.SmoothStep(0.0f, 1.0f, time);

            m_owner.transform.position = Utility.Bezier(mParam.PortalEntry.transform.position, p2, mParam.EndPos, t);
            yield return mWaitForFixedUpdate;
        }
    }

    private bool ChangeStartState(bool changeAni)
    {
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.Jump01);
        m_checkTime = 0.0f;

        return true;
    }

    private bool ChangeJumpingState(bool changeAni)
    {
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.Jump02);
        m_checkTime = 0.0f;

        return true;
    }

    private bool ChangeEndState(bool changeAni)
    {
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.Jump03);
        m_checkTime = 0.0f;

        return true;
    }
}
