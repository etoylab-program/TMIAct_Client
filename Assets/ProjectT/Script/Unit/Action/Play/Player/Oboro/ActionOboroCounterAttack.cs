
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionOboroCounterAttack : ActionCounterAttack
{
    private eAnimation  mCurAni     = eAnimation.CounterAttack;
    private bool        mAttack     = false;
    private Unit        mTarget     = null;
    private Vector3     mDest       = Vector3.zero;


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

        superArmor = Unit.eSuperArmor.Lv2;

        if (mValue1 > 0.0f)
        {
            AniEvent.sEvent atkEvt = m_owner.aniEvent.GetLastAttackEvent(eAnimation.CounterAttack);
            if(atkEvt != null)
            {
                atkEvt.behaviour = eBehaviour.KnockBackAttack;
            }
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.Counter);
        mAttack = false;

        ShowSkillNames(m_data);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }
            else if(mAttack && m_checkTime >= m_aniCutFrameLength && !m_owner.IsShowMesh)
            {
                m_owner.ShowMesh(true);
                SetDest();
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        if(mAttack)
        {
            return;
        }

        mAttack = true;
        m_checkTime = 0.0f;

        mDest = m_owner.transform.position;
        if (FSaveData.Instance.AutoTargetingSkill)
        {
            mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
            if (mTarget)
            {
                m_owner.LookAtTarget(mTarget.transform.position);
            }
        }
        else
        {
            m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
        }

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.CounterAttack);
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.CounterAttack);

        m_owner.ShowMesh(false);
    }

    public override void OnEnd()
    {
        base.OnEnd();

        ForceQuitBuffDebuff = true;
        m_owner.ShowMesh(true);
        m_owner.checkRayCollision = true;

        BOCharSkill.EndBattleOption(BattleOption.eBOTimingType.DuringSkill, TableId);
    }

    public override void OnCancel()
    {
        base.OnCancel();

        ForceQuitBuffDebuff = true;
        m_owner.ShowMesh(true);
        m_owner.checkRayCollision = true;
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
        if (evt == null)
        {
            return 0.0f;
        }

        return evt.visionRange;
    }

    private void SetDest()
    {
        m_owner.checkRayCollision = false;

        mDest = m_owner.GetTargetCapsuleEdgePosForTeleport( mTarget );//mTarget.transform.position - (mTarget.transform.forward * 2.0f);
        mDest.y = m_owner.transform.position.y;

        /*
        if (Physics.Linecast(mDest, mTarget.transform.position, (1 << (int)eLayer.Wall) | (1 << (int)eLayer.EnvObject)))
        {
            mDest = mTarget.transform.position + (mTarget.transform.forward * 2.0f);
            mDest.y = m_owner.transform.position.y;
        }
        else
        {
            Collider[] cols = Physics.OverlapBox(mDest, Vector3.one * 0.6f, Quaternion.identity, (1 << (int)eLayer.Wall) | (1 << (int)eLayer.EnvObject));
            if (cols.Length > 0)
            {
                mDest = mTarget.transform.position + (mTarget.transform.forward * 2.0f);
                mDest.y = m_owner.transform.position.y;
            }
        }
        */

        m_owner.SetInitialPosition(mDest, m_owner.transform.rotation);
    }
}
