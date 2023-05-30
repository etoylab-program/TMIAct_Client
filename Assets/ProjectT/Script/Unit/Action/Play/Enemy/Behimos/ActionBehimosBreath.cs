
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionBehimosBreath : ActionEnemyAttackBase
{
    public enum eState
    {
        TakeOff = 0,        // 이륙
        MoveToEdge,         // 맵의 끝 부분으로 이동
        PrepareToAttack,    
        Attack,             
        MoveToCenter,       // 맵 가운데로 이동
        Landing,            // 착륙
        Grounded,           // 도착
    }


    [Header("Property")]
    public float takeOffDistance1 = 10.0f;
    public float takeOffDistance2 = 7.0f;

    private State m_state = new State();
    private float m_takeOffDistance = 10.0f;
    private float m_checkDistance = 0.0f;
    private eState m_nextStateAfterTakeOff;
    private Vector3 m_startPos = Vector3.zero;
    private Vector3 m_center = Vector3.zero;
    private Vector3 m_dest = Vector3.zero;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.BehimosBreath;

        m_state.Init(7);
        m_state.Bind(eState.TakeOff, ChangeTakeOffState);
        m_state.Bind(eState.MoveToEdge, ChangeMoveToEdgeState);
        m_state.Bind(eState.PrepareToAttack, ChangePrepareToAttackState);
        m_state.Bind(eState.Attack, ChangeAttackState);
        m_state.Bind(eState.MoveToCenter, ChangeMoveToCenterState);
        m_state.Bind(eState.Landing, ChangeLandingState);
        m_state.Bind(eState.Grounded, ChangeGroundedState);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        m_checkDistance = m_owner.MainCollider.radius * 3.0f;

        m_takeOffDistance = takeOffDistance1;
        m_nextStateAfterTakeOff = eState.MoveToEdge;

        m_state.ChangeState(eState.TakeOff, true);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            switch ((eState)m_state.current)
            {
                case eState.TakeOff:
                    {
                        m_owner.SetFloatingRigidBody();

                        m_owner.posOnGround = m_owner.transform.position;
                        m_owner.cmptMovement.UpdatePosition(Vector3.up, m_owner.speed * 1.5f, false);

                        if (Vector3.Distance(m_owner.transform.position, m_startPos) > m_takeOffDistance)
                        {
                            m_state.ChangeState(m_nextStateAfterTakeOff, true);
                        }
                    }
                    break;

                case eState.MoveToEdge:
                    {
                        Vector3 v = (m_dest - m_owner.transform.position).normalized;
                        m_owner.cmptMovement.UpdatePosition(v, m_owner.speed * 3.0f, false);
                        m_owner.cmptRotate.UpdateRotation(v, false);

                        if (Vector3.Distance(m_owner.transform.position, m_dest) <= m_checkDistance)
                            m_state.ChangeState(eState.PrepareToAttack, true);
                    }
                    break;

                case eState.PrepareToAttack:
                    {
                        m_owner.cmptMovement.UpdatePosition(-Vector3.up, m_owner.speed, false);

                        Vector3 fixedCenter = new Vector3(m_center.x, m_owner.transform.position.y, m_center.z);
                        m_owner.cmptRotate.UpdateRotation(fixedCenter - m_owner.transform.position, false, 0.5f);

                        if (Vector3.Distance(m_owner.transform.position, m_startPos) > takeOffDistance2)
                            m_state.ChangeState(eState.Attack, true);
                    }
                    break;

                case eState.Attack:
                    {
                        m_checkTime += m_owner.fixedDeltaTime;
                        if (m_checkTime >= m_aniLength)
                        {
                            m_state.ChangeState(eState.TakeOff, true);
                        }
                    }
                    break;

                case eState.MoveToCenter:
                    {
                        Vector3 v = (m_dest - m_owner.transform.position).normalized;
                        m_owner.cmptMovement.UpdatePosition(v, m_owner.speed, false);
                        m_owner.cmptRotate.UpdateRotation(v, false);

                        if (Vector3.Distance(m_owner.transform.position, m_dest) <= m_checkDistance)
                            m_state.ChangeState(eState.Landing, true);
                    }
                    break;

                case eState.Landing:
                    {
                        m_owner.cmptMovement.UpdatePosition(-Vector3.up, m_owner.speed * 4.0f, false);
                        if(m_owner.isGrounded)
                            m_state.ChangeState(eState.Grounded, true);
                    }
                    break;

                case eState.Grounded:
                    {
                        m_checkTime += m_owner.fixedDeltaTime;
                        if (m_checkTime >= m_aniLength)
                            m_endUpdate = true;
                    }
                    break;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnEnd()
    {
        m_owner.ForceSetSuperArmor(superArmor);
        base.OnEnd();
    }

    public override void OnCancel()
    {
        m_owner.ForceSetSuperArmor(superArmor);
        m_owner.TemporaryInvincible = false;

        base.OnCancel();
    }

    private bool ChangeTakeOffState(bool changeAni)
    {
        mChangedSuperArmorId = m_owner.SetSuperArmor(Unit.eSuperArmor.Lv2);

        m_owner.SetFloatingRigidBody();
        m_owner.PlayAni(eAnimation.FlyIdle);

        m_startPos = m_owner.transform.position;
        return true;
    }

    private bool ChangeMoveToEdgeState(bool changeAni)
    {
        //m_owner.SetSuperArmor(Unit.eSuperArmor.Invincible, GetType());
        m_owner.TemporaryInvincible = true;

        if (World.Instance.TestScene)
        {
            m_dest = new Vector3(25.0f, 0.0f, 0.0f);
            m_center = Vector3.zero;
        }
        else
        {
            BattleAreaManager battleAreaMgr = World.Instance.EnemyMgr as BattleAreaManager;
            BattleArea battleArea = battleAreaMgr.GetCurrentBattleArea();

            m_center = battleArea.center.transform.position;
            Transform tf = battleArea.edgePoints[UnityEngine.Random.Range(0, battleAreaMgr.GetCurrentBattleArea().edgePoints.Length - 1)];
            m_dest = tf.position;
        }

        m_dest.y = m_owner.transform.position.y;
        return true;
    }

    private bool ChangePrepareToAttackState(bool changeAni)
    {
        //m_owner.RestoreSuperArmor(Unit.eSuperArmor.Invincible, GetType());
        m_owner.TemporaryInvincible = false;
        m_startPos = m_owner.transform.position;

        return true;
    }

    private bool ChangeAttackState(bool changeAni)
    {
        mChangedSuperArmorId = m_owner.SetSuperArmor(Unit.eSuperArmor.Lv2);
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.AirAttack01);

        m_takeOffDistance = takeOffDistance2;
        m_nextStateAfterTakeOff = eState.MoveToCenter;

        return true;
    }

    private bool ChangeMoveToCenterState(bool changeAni)
    {
        //m_owner.SetSuperArmor(Unit.eSuperArmor.Invincible, GetType());
        m_owner.TemporaryInvincible = true;

        m_dest = m_center;
        m_dest.y = m_owner.transform.position.y;

        return true;
    }

    private bool ChangeLandingState(bool changeAni)
    {
        //m_owner.RestoreSuperArmor(Unit.eSuperArmor.Invincible, GetType());
        m_owner.TemporaryInvincible = false;
        //mChangedSuperArmorId = m_owner.SetSuperArmor(Unit.eSuperArmor.Lv2);

        m_owner.PlayAni(eAnimation.FlyIdle);
        return true;
    }

    private bool ChangeGroundedState(bool changeAni)
    {
        //mChangedSuperArmorId = m_owner.SetSuperArmor(Unit.eSuperArmor.Lv2);
        m_owner.SetGroundedRigidBody();

        m_checkTime = 0.0f;
        m_aniLength = m_owner.PlayAni(eAnimation.FlyLanding);

        return true;
    }
}
