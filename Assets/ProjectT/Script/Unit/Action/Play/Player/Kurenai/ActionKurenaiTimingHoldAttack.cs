
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionKurenaiTimingHoldAttack : ActionSelectSkillBase
{
    public enum eState
    {
        //Teleport,
        //Turn,
        Attack,
        FireProjectile,
    }


    private State                       mState          = new State();
    private UnitCollider                mTargetCollider = null;
    private Projectile                  mPjt            = null;
    private AniEvent.sEvent             mAniEvt         = null;
    private AniEvent.sProjectileInfo    mPjtInfo        = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.TimingHoldAttack;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Grounded;

        superArmor = Unit.eSuperArmor.Lv1;

		mState.Init(2);
        //mState.Bind(eState.Teleport, ChangeTeleportState);
        //mState.Bind(eState.Turn, ChangeTurnState);
        mState.Bind(eState.Attack, ChangeAttackState);
        mState.Bind(eState.FireProjectile, ChangeFireProjectileState);

        mPjt = GameSupport.CreateProjectile("Projectile/pjt_character_kurenai_wskill_17_02.prefab");
        mPjt.OnHitFunc = OnProjectileHit;

        mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 0.0f);

        mPjtInfo = m_owner.aniEvent.CreateProjectileInfo(mPjt);
        mPjtInfo.attach = false;
        mPjtInfo.boneName = "root";
        mPjtInfo.followParentRot = true;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        m_owner.cmptJump.StartJump(13.5f, false);

        //mState.ChangeState(eState.Teleport, true);
        mState.ChangeState(eState.Attack, true);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;

            switch ((eState)mState.current)
            {
                /*
                case eState.Teleport:
                    if(m_checkTime >= m_aniLength)
                    {
                        mState.ChangeState(eState.Turn, true);
                    }
                    break;

                case eState.Turn:
                    if (m_checkTime >= m_aniLength)
                    {
                        mState.ChangeState(eState.Attack, true);
                    }
                    break;
                    */

                case eState.Attack:
                    if (m_checkTime >= m_aniLength)
                    {
                        m_endUpdate = true;
                    }
                    else if(SetAddAction && m_checkTime >= m_aniLength * 0.8f)
                    {
                        mState.ChangeState(eState.FireProjectile, true);
                    }
                    break;

                case eState.FireProjectile:
                    if (m_checkTime >= m_aniLength)
                    {
                        m_endUpdate = true;
                    }
                    break;
            }

            m_owner.cmptJump.UpdateJump();
            yield return mWaitForFixedUpdate;
        }
    }

    public override float GetAtkRange()
    {
        return 20.0f;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        m_owner.ShowMesh(true);

        //Utility.SetPhysicsLayerCollision(m_owner.rigidBody, eLayer.Player, eLayer.Enemy);
        //Utility.SetPhysicsLayerCollision(m_owner.rigidBody, eLayer.Player, eLayer.EnemyGate);
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_owner.ShowMesh(true);
        
        //Utility.SetPhysicsLayerCollision(m_owner.rigidBody, eLayer.Player, eLayer.Enemy);
        //Utility.SetPhysicsLayerCollision(m_owner.rigidBody, eLayer.Player, eLayer.EnemyGate);
    }

    /*
    private bool ChangeTeleportState(bool changeAni)
    {
        Utility.IgnorePhysics(m_owner.rigidBody, eLayer.Player, eLayer.Enemy);
        Utility.IgnorePhysics(m_owner.rigidBody, eLayer.Player, eLayer.EnemyGate);

        m_owner.ShowMesh(false);
        m_owner.checkRayCollision = false;

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.Teleport);
        m_checkTime = 0.0f;

        return true;
    }

    private bool ChangeTurnState(bool changeAni)
    {
        mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
        if (mTarget && !mTarget.alwaysKinematic && !Physics.Raycast(mTarget.transform.position, -mTarget.transform.forward, 1.5f, (1 << (int)eLayer.Wall) | (1 << (int)eLayer.EnvObject)))
        {
            Debug.Log("타겟 등뒤로 이동!!!!!!!!!");

            Vector3 pos = mTarget.transform.position - (mTarget.transform.forward * 1.5f);
            if(pos.y < m_owner.posOnGround.y)
            {
                pos.y = m_owner.posOnGround.y;
            }

            m_owner.rigidBody.MovePosition(pos);
        }

        m_aniLength = 0.1f;
        m_checkTime = 0.0f;

        return true;
    }
    */

    private bool ChangeAttackState(bool changeAni)
    {
        m_owner.checkRayCollision = true;
        m_owner.ShowMesh(true);

        //Utility.SetPhysicsLayerCollision(m_owner.rigidBody, eLayer.Player, eLayer.Enemy);
        //Utility.SetPhysicsLayerCollision(m_owner.rigidBody, eLayer.Player, eLayer.EnemyGate);

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider)
        {
            m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
        }

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.ComboAttack);
        m_checkTime = 0.0f;

        return true;
    }

    private bool ChangeFireProjectileState(bool changeAni)
    {
        Unit target = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
        if(target == null)
        {
            return true;
        }

        mAniEvt.atkRatio = AddActionValue1;
        mPjtInfo.addedPosition = new Vector3(0.0f, 0.0f, -5.0f);

        mPjt.Fire(m_owner, BattleOption.eToExecuteType.Unit, mAniEvt, mPjtInfo, target, TableId);
        return true;
    }

    private bool OnProjectileHit(Unit target)
    {
        if(target == null)
        {
            return false;
        }

        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
        mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
        mBuffEvt.Set(GetInstanceID(), eEventSubject.Self, eEventType.EVENT_DEBUFF_SPEED_DOWN, target, 0.1f, 0.0f, 0.0f,
                     10.0f, 0.0f, 0, mBuffEvt.effId2, eBuffIconType.Debuff_Speed);

        EventMgr.Instance.SendEvent(mBuffEvt);
        return true;
    }
}
