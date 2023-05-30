
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionCloneHomingAttack : ActionSelectSkillBase
{
    public static int DirIndex { get; private set; } = 0;

    private ActionParamAttack   mParamAtk   = null;
    private int                 mIndex      = 0;
    private Unit                mTarget     = null;
    private Unit                mCloneOwner = null;
    private List<Vector3>       mListDir    = new List<Vector3>();
    private bool                mExtraAtk   = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.CloneHomingAttack;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mParamAtk = param as ActionParamAttack;
        if (mParamAtk != null)
        {
            mIndex = mParamAtk.index;
            mTarget = mParamAtk.target;
            mExtraAtk = mParamAtk.HasExtraAttack;

            if(mExtraAtk)
            {
                List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllAttackEvent(eAnimation.CloneRushAttack);
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].behaviour = eBehaviour.Attack;
                }
            }

            m_owner.SetAttackPower(m_owner.attackPower * mParamAtk.atkRatio);
        }
        else
        {
            List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllAttackEvent(eAnimation.CloneRushAttack);
            for(int i = 0; i < list.Count; i++)
            {
                list[i].behaviour = eBehaviour.Attack;
            }

            mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
        }

        m_owner.PlayAniImmediate(eAnimation.Dash);
    }

    public override IEnumerator UpdateAction()
    {
        mCloneOwner = m_owner.cloneOwner;//m_owner.cloneOwner as Player;
        Vector3 dir = Vector3.zero;

        if (mCloneOwner == null)
            m_endUpdate = true;
        else
        {
            mListDir.Clear();
            mListDir.Add(mCloneOwner.transform.forward);

            if (mCloneOwner.LockAxis != Unit.eAxisType.None)
            {
                mListDir.Add((mCloneOwner.transform.forward).normalized);
                mListDir.Add((mCloneOwner.transform.forward).normalized);
            }
            else
            {
                mListDir.Add((mCloneOwner.transform.forward - (mCloneOwner.transform.right * 0.65f)).normalized);
                mListDir.Add((mCloneOwner.transform.forward + (mCloneOwner.transform.right * 0.65f)).normalized);
            }

            if( DirIndex >= mListDir.Count ) {
                DirIndex = 0;
			}

            Vector3 targetPos = mCloneOwner.transform.position + (mListDir[DirIndex++] * 5.0f);
            if (mTarget)
            {
                targetPos = mTarget.transform.position;
                targetPos.y = mCloneOwner.transform.position.y;
            }

            AniEvent.sEvent evt = m_owner.aniEvent.GetNextAttackEvent(eAnimation.CloneRushAttack);
            float atkRange = evt.atkRange * 0.5f;
            m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.CloneRushAttack);

            Collider[] cols = null;
            bool attack = false;

            //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            while (mCloneOwner && m_endUpdate == false)
            {
                if (!attack)
                {
                    dir = (targetPos - m_owner.transform.position).normalized;
                    dir.y = 0.0f;

                    m_owner.transform.position += dir * (mCloneOwner.speed * 8.0f) * m_owner.fixedDeltaTime;
                    m_owner.LookAtTarget(targetPos);

                    Vector3 ownerPos = m_owner.transform.position;
                    ownerPos.y = targetPos.y;

                    if (mTarget)
                    {
                        cols = Physics.OverlapSphere(m_owner.transform.position, atkRange, 1 << mTarget.gameObject.layer);
                        if (cols.Length > 0)
                        {
                            m_aniLength = m_owner.PlayAniImmediate(eAnimation.CloneRushAttack);
                            attack = true;
                        }
                        else
                        {
                            float dist = Vector3.Distance(ownerPos, targetPos);
                            if (dist <= atkRange)
                            {
                                m_aniLength = m_owner.PlayAniImmediate(eAnimation.CloneRushAttack);
                                attack = true;
                            }
                        }
                    }
                    else
                    {
                        float dist = Vector3.Distance(ownerPos, targetPos);
                        if (dist <= atkRange)
                        {
                            m_aniLength = m_owner.PlayAniImmediate(eAnimation.CloneRushAttack);
                            attack = true;
                        }
                    }
                }
                else
                {
                    m_checkTime += m_owner.fixedDeltaTime;
                    if (m_checkTime >= m_aniLength)
                    {
                        m_endUpdate = true;
                    }
                    else if(mExtraAtk && m_checkTime >= m_aniCutFrameLength)
                    {
                        m_endUpdate = true;
                    }
                }

                yield return mWaitForFixedUpdate;
            }

            // 연진화 사용
            if(mCloneOwner && mExtraAtk && m_owner.listHitTarget.Count > 0)
            {
                mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
                if (mTarget)
                {
                    m_owner.LookAtTarget(mTarget.transform.position);
                }

                m_aniLength = m_owner.PlayAniImmediate(eAnimation.RushAttack);
                m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.RushAttack);

                m_checkTime = 0.0f;
                m_endUpdate = false;

                int extraAtkCount = 4;
                while (!m_endUpdate)
                {
                    m_checkTime += m_owner.fixedDeltaTime;
                    if (m_checkTime >= m_aniLength)
                    {
                        m_endUpdate = true;
                    }
                    else if(extraAtkCount > 0 && m_checkTime >= m_aniCutFrameLength)
                    {
                        m_aniLength = m_owner.PlayAniImmediate(eAnimation.RushAttackRepeat);
                        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.RushAttackRepeat);
                        m_checkTime = 0.0f;

                        --extraAtkCount;
                    }

                    yield return mWaitForFixedUpdate;
                }

                mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
                if (mTarget)
                {
                    m_owner.LookAtTarget(mTarget.transform.position);
                }

                m_aniLength = m_owner.PlayAniImmediate(eAnimation.RushAttackRepeatFinish);
                yield return new WaitForSeconds(m_aniLength);
            }
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        if (mParamAtk != null)
        {
            mCloneOwner.HideClone(mIndex);
        }
    }
}
