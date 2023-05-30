
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionYukikazeAirShotAttack : ActionSelectSkillBase
{
    public enum eState
    {
        Start = 0,
        Doing,
        End,
    }


    private State           mState          = new State();
    private eAnimation[]    mAniCombo       = null;
    private int             mCurComboAni    = 0;
    private bool            mNextComboAni   = false;
    private UnitCollider    mTargetCollider = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.AirShotAttack;

        IsRepeatSkill = true;
        ExplicitStartCoolTime = true;

        mAniCombo = new eAnimation[3];

        if (mValue1 <= 0)
        {
            mAniCombo[0] = eAnimation.AirShot01;
            mAniCombo[1] = eAnimation.AirShot02;
            mAniCombo[2] = eAnimation.AirShot03;
        }
        else
        {
            mAniCombo[0] = eAnimation.AirShot04;
            mAniCombo[1] = eAnimation.AirShot05;
            mAniCombo[2] = eAnimation.AirShot06;
        }

        superArmor = Unit.eSuperArmor.Lv1;

		mState.Init(3);
        mState.Bind(eState.Start, ChangeStartState);
        mState.Bind(eState.Doing, ChangeDoingState);
        mState.Bind(eState.End, ChangeEndState);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mState.ChangeState(eState.Start, true);
    }

    public override IEnumerator UpdateAction()
    {
        Vector3 ikPos = Vector3.zero;

        Unit.sIKObject ikObjLH = m_owner.GetIKObject(Unit.eBipedIK.LeftHand);
        Unit.sIKObject ikObjRH = m_owner.GetIKObject(Unit.eBipedIK.RightHand);
        Unit.sIKObject ikObjBody = m_owner.GetIKObject(Unit.eBipedIK.Body);

        m_owner.EnableIK(true);
        bool endIK = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            switch ((eState)mState.current)
            {
                case eState.Start:
                    if (m_checkTime >= m_aniLength)
                    {
                        if (mNextComboAni)
                            mState.ChangeState(eState.Doing, true);
                        else
                            mState.ChangeState(eState.End, true);
                    }
                    break;

                case eState.Doing:
                    if (m_checkTime >= m_aniLength)
                    {
                        ++mCurComboAni;
                        if (!mNextComboAni || mCurComboAni >= mAniCombo.Length)
                            mState.ChangeState(eState.End, true);
                        else
                            mState.ChangeState(eState.Doing, true);
                    }
                    break;

                case eState.End:
                    if (m_owner.aniEvent.IsAniPlaying(eAnimation.FinishAirShot) == eAniPlayingState.End)
                        m_endUpdate = true;
                    else
                    {
                        if (!endIK && m_checkTime >= m_aniCutFrameLength)
                        {
                            endIK = true;
                            m_owner.EnableIK(false);
                        }
                    }
                    break;
            }

            if (!endIK)
            {
                ikPos = mTargetCollider.GetCenterPos();//.Owner.transform.position;
                ikObjLH.targetMovement.transform.position = ikPos;
                ikObjRH.targetMovement.transform.position = ikPos;

                Vector3 v = m_owner.GetCenterPos() + (m_owner.transform.forward * 1.5f);
                v.y = mTargetCollider.Owner.GetHeadPos().y;
                ikObjBody.targetMovement.transform.position = Vector3.Slerp(m_owner.GetCenterPos() - (m_owner.transform.forward * 3.0f), v, 0.55f);
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        if ((eState)mState.current == eState.End)
            return;

        mNextComboAni = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        DisableIK();
    }

    public override void OnCancel()
    {
        base.OnCancel();
        DisableIK();
    }

    private bool ChangeStartState(bool changeAni)
    {
        m_checkTime = 0.0f;
        mCurComboAni = 0;
        mNextComboAni = false;

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider)
        {
            m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
            //World.Instance.InGameCamera.SetDefaultModeTarget(mTargetCollider.Owner, false);
        }

        m_aniLength = m_owner.PlayAni(eAnimation.PrepareAirShot);
        return true;
    }

    private bool ChangeDoingState(bool changeAni)
    {
        m_checkTime = 0.0f;
        mNextComboAni = false;

        m_aniLength = m_owner.PlayAni(mAniCombo[mCurComboAni]);
        return true;
    }

    private bool ChangeEndState(bool changeAni)
    {
        m_checkTime = 0.0f;
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.FinishAirShot);
        m_aniLength = m_owner.PlayAni(eAnimation.FinishAirShot);

        return true;
    }

    private void DisableIK()
    {
        m_owner.EnableIK(false);

        /*
        if (World.Instance.Boss && World.Instance.Boss.data.FixedCamera)
        {
            World.Instance.InGameCamera.SetDefaultModeTarget(World.Instance.Boss, true);
        }
        else
        {
            World.Instance.InGameCamera.SetDefaultModeTarget(null, false);
        }
        */
    }
}
