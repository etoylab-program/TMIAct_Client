
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionShizuruEvadeDouble2Attack : ActionSelectSkillBase
{
    public enum eState
    {
        StartAttack,
        Move,
        EndAttack,
    }


    private State   mState      = new State();
    private Unit    mTarget     = null;
    private float   mMoveTime   = 0.1f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Teleport;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        extraCancelCondition = new eActionCondition[3];
        extraCancelCondition[0] = eActionCondition.UseSkill;
        extraCancelCondition[1] = eActionCondition.UseQTE;
        extraCancelCondition[2] = eActionCondition.UseUSkill;

        superArmor = Unit.eSuperArmor.Lv1;

		mState.Init(3);
        mState.Bind(eState.StartAttack, ChangeStartAttackState);
        mState.Bind(eState.Move, ChangeMoveState);
        mState.Bind(eState.EndAttack, ChangeEndAttackState);

        if (mValue1 >= 1.0f)
        {
            List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllAttackEvent(eAnimation.EvadeDouble2_1);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].behaviour = eBehaviour.StunAttack;
            }
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        ShowSkillNames(m_data);
        mState.ChangeState(eState.StartAttack, true);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            switch ((eState)mState.current)
            {
                case eState.StartAttack:
                    if (m_checkTime >= m_aniCutFrameLength)
                    {
                        if (mTarget)
                        {
                            mState.ChangeState(eState.Move, true);
                        }
                        else if (mTarget == null || m_checkTime >= m_aniLength)
                        {
                            m_endUpdate = true;
                        }
                    }
                    break;

                case eState.Move:
                    if (m_owner.holdPositionRef > 0 || m_checkTime >= mMoveTime || Vector3.Distance(m_owner.transform.position, GetDestPos()) <= 0.5f)
                    {
                        mState.ChangeState(eState.EndAttack, true);
                    }
                    break;

                case eState.EndAttack:
                    if(m_checkTime >= m_aniCutFrameLength)
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
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.EvadeDouble2);
        if (evt == null)
        {
            Debug.LogError(eAnimation.EvadeDouble2.ToString() + "공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError(eAnimation.EvadeDouble2.ToString() + "Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    protected override void LookAtTarget(Unit target)
    {
        if (FSaveData.Instance.AutoTargetingSkill && target)
        {
            m_owner.LookAtTarget(target.transform.position);
        }
        else
        {
            m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
        }
    }

    private bool ChangeStartAttackState(bool changeAni)
    {
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.EvadeDouble2);
        m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();

        mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
        LookAtTarget(mTarget);

        m_checkTime = 0.0f;
        return true;
    }

    private bool ChangeMoveState(bool changeAni)
    {
        m_owner.StartStepForward(mMoveTime, GetDestPos(), null);
        m_checkTime = 0.0f;

        return true;
    }

    private bool ChangeEndAttackState(bool changeAni)
    {
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.EvadeDouble2_1);
        m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();

        m_checkTime = 0.0f;
        return true;
    }

    private Vector3 GetDestPos()
    {
        Vector3 targetPos = m_owner.transform.position;

        if (mTarget)
        {
            targetPos = m_owner.GetTargetCapsuleEdgePosForTeleport( mTarget );
            targetPos.y = m_owner.transform.position.y;

            float dist = Vector3.Distance(m_owner.transform.position, targetPos);
            if (Physics.Raycast(m_owner.transform.position, (targetPos - m_owner.transform.position).normalized, out RaycastHit hitInfo,
                                dist, (1 << (int)eLayer.Wall)))
            {
                targetPos = hitInfo.point;
                targetPos.y = m_owner.transform.position.y;
            }

            Vector3 v = (m_owner.transform.position - targetPos).normalized;
            targetPos += (v * m_owner.MainCollider.radius);
        }

        return targetPos;
    }
}
