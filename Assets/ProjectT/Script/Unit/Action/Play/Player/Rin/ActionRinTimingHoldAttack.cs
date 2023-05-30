
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionRinTimingHoldAttack : ActionSelectSkillBase
{
    public enum eState
    {
        Teleport,
        Turn,
        Attack,
    }


    private State   mState  = new State();
    private Unit    mTarget = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.TimingHoldAttack;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Grounded;

        superArmor = Unit.eSuperArmor.Lv1;

		mState.Init(3);
        mState.Bind(eState.Teleport, ChangeTeleportState);
        mState.Bind(eState.Turn, ChangeTurnState);
        mState.Bind(eState.Attack, ChangeAttackState);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        mState.ChangeState(eState.Teleport, true);
    }

    public override IEnumerator UpdateAction()
    {
        while(!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;

            switch ((eState)mState.current)
            {
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

                case eState.Attack:
                    if (m_checkTime >= m_aniLength)
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
        return 20.0f;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        m_owner.ShowMesh(true);

        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_owner.ShowMesh(true);
        
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);
    }

    private bool ChangeTeleportState(bool changeAni)
    {
        Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);
        Utility.IgnorePhysics(eLayer.Player, eLayer.EnemyGate);

        m_owner.ShowMesh(false);
        m_owner.checkRayCollision = false;

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.EvadeTiming);
        m_checkTime = 0.0f;

        return true;
    }

    private bool ChangeTurnState(bool changeAni)
    {
        if ( m_owner.holdPositionRef <= 0 ) {
            mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
            if ( mTarget ) {
                if ( !mTarget.alwaysKinematic && !Physics.Raycast( mTarget.transform.position, -mTarget.transform.forward, 1.5f, ( 1 << ( int )eLayer.Wall ) | ( 1 << ( int )eLayer.EnvObject ) ) ) {
                    Debug.Log( "타겟 등뒤로 이동!!!!!!!!!" );

                    Vector3 pos = mTarget.transform.position - (mTarget.transform.forward * 1.5f);
                    if ( pos.y < m_owner.posOnGround.y ) {
                        pos.y = m_owner.posOnGround.y;
                    }

                    m_owner.rigidBody.MovePosition( pos );
                }
                else {
                    Vector3 p = m_owner.GetTargetCapsuleEdgePosForTeleport( mTarget );
                    m_owner.rigidBody.MovePosition( p );
                }
            }
        }

        m_aniLength = 0.1f;
        m_checkTime = 0.0f;

        return true;
    }

    private bool ChangeAttackState(bool changeAni)
    {
        m_owner.checkRayCollision = true;
        m_owner.ShowMesh(true);

        if (mTarget)
        {
            m_owner.LookAtTarget(mTarget.transform.position);
        }

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.EvadeTiming_1);
        m_checkTime = 0.0f;

        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);

        return true;
    }
}
