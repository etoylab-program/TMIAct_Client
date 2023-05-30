
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionHangingAround : ActionEnemyBase
{
    private eDirection m_dir = eDirection.None;
    private eDirection[] m_appointDir = null;
    private List<int> m_listDirIndex = new List<int>();

    private eAnimation m_curAni = eAnimation.None;
    private eActionCommand m_distActionCommand = eActionCommand.None;
    private bool m_randomCheckDist = false;
    private float m_endDuration = 0.0f;
    private float m_moveTime = 2.0f;
    private float m_minWait = 0.0f;
    private float m_maxWait = 1.0f;
    private float m_checkMoveTime = 0.0f;
    private bool m_waiting = false;
    private bool m_turn = false;

    private UnitCollider mTargetCollider = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.HangingAround;
    }

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		ActionParamHangingAround paramHangingAround = param as ActionParamHangingAround;

		m_endDuration = Utility.GetRandom( paramHangingAround.minDuration, paramHangingAround.maxDuration, 10.0f );
		m_distActionCommand = paramHangingAround.distActionCommand;
		m_randomCheckDist = paramHangingAround.randomCheckDist;
		m_appointDir = paramHangingAround.directions;
		m_turn = paramHangingAround.turn;
		m_checkMoveTime = 0.0f;

		mTargetCollider = m_owner.GetMainTargetCollider( true );

		InitListDir();
		m_dir = GetCurDirection();

		m_owner.PlayAni( m_curAni, 0, ( m_dir == eDirection.LeftDash || m_dir == eDirection.RightDash ) ? true : false, false );
	}

	private void InitListDir()
    {
        m_listDirIndex.Clear();

        if (m_appointDir == null)
        {
            for (int j = 0; j < 2; j++) // 적당한 랜덤을 위해~
            {
                for (int i = 1; i < (int)eDirection.Max; i++)
                    m_listDirIndex.Add(i);
            }

            m_listDirIndex.RemoveAt(m_listDirIndex.Count - 1);
        }
        else
        {
            for (int i = 0; i < m_appointDir.Length; i++)
                m_listDirIndex.Add((int)m_appointDir[i]);
        }
    }

    public override IEnumerator UpdateAction()
    {
        Vector3 v = Vector3.zero;
        bool getV = true;
        float waitTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            if (m_distActionCommand != eActionCommand.None && m_owner.CheckTargetDist(m_distActionCommand))
            {
                if (!m_randomCheckDist || (m_randomCheckDist && Random.Range(0, 4) == 0))
                    m_endUpdate = true;
            }

            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= m_endDuration)
                m_endUpdate = true;

            if (!m_waiting)
            {
                m_checkMoveTime += m_owner.fixedDeltaTime;
                if (m_checkMoveTime >= m_moveTime)
                {
                    m_dir = GetCurDirection();
                    m_checkMoveTime = 0.0f;
                    m_waiting = true;
                    getV = false;

                    m_owner.PlayAni(eAnimation.Idle01);
                }
            }
            else
            {
                waitTime = Utility.GetRandom(m_minWait, m_maxWait, 10.0f);

                while (m_waiting)
                {
                    if (m_distActionCommand != eActionCommand.None && m_owner.CheckTargetDist(m_distActionCommand))
                    {
                        if (!m_randomCheckDist || (m_randomCheckDist && Random.Range(0, 4) == 0))
                            m_waiting = false;
                    }

                    m_checkMoveTime += m_owner.fixedDeltaTime;
                    if(m_checkMoveTime >= waitTime)
                    {
                        m_checkMoveTime = 0.0f;
                        m_waiting = false;
                    }

                    yield return mWaitForFixedUpdate;
                }

                m_owner.PlayAni(m_curAni);
            }

            if (mTargetCollider == null || mTargetCollider.Owner == null)
            {
                m_owner.PlayAni(eAnimation.Idle01);
                break;
            }

            if (mTargetCollider && mTargetCollider.Owner)
            {
                if (getV)
                {
                    switch (m_dir)
                    {
                        case eDirection.Left:
                            if (mTargetCollider.Owner && m_turn)
                                v = mTargetCollider.Owner.transform.right;
                            else
                                v = -m_owner.transform.right;
                            break;

                        case eDirection.Right:
                            if (mTargetCollider.Owner && m_turn)
                                v = -mTargetCollider.Owner.transform.right;
                            else
                                v = m_owner.transform.right;
                            break;

                        case eDirection.Back:
                            if (mTargetCollider.Owner && m_turn)
                                v = (transform.position - mTargetCollider.Owner.transform.position).normalized;
                            else
                                v = -m_owner.transform.forward;
                            break;

                        case eDirection.LeftBack:
                            if (mTargetCollider.Owner && m_turn)
                                v = (mTargetCollider.Owner.transform.right + (transform.position - mTargetCollider.Owner.transform.position)).normalized;
                            else
                                v = (-m_owner.transform.right - m_owner.transform.forward).normalized;
                            break;

                        case eDirection.RightBack:
                            if (mTargetCollider.Owner && m_turn)
                                v = (-mTargetCollider.Owner.transform.right + (transform.position - mTargetCollider.Owner.transform.position)).normalized;
                            else
                                v = (m_owner.transform.right - m_owner.transform.forward).normalized;
                            break;

                        case eDirection.LeftForward:
                            if (mTargetCollider.Owner && m_turn)
                                v = (mTargetCollider.Owner.transform.right + (mTargetCollider.Owner.transform.position - transform.position)).normalized;
                            else
                                v = (-m_owner.transform.right - (m_owner.transform.forward * 0.8f)).normalized;
                            break;

                        case eDirection.RightForward:
                            if (mTargetCollider.Owner && m_turn)
                                v = (-mTargetCollider.Owner.transform.right + (mTargetCollider.Owner.transform.position - transform.position)).normalized;
                            else
                                v = (m_owner.transform.right + (m_owner.transform.forward * 0.8f)).normalized;
                            break;

                        case eDirection.Forward:
                            if (mTargetCollider.Owner && m_turn)
                                v = (transform.position - mTargetCollider.Owner.transform.position).normalized;
                            else
                                v = m_owner.transform.forward;
                            break;
                    }

                    getV = false;
                }

                if (m_turn)
                    m_owner.cmptRotate.UpdateRotation(v, false);
                else if (!m_turn && mTargetCollider.Owner)
                    m_owner.LookAtTarget(mTargetCollider.Owner.transform.position);
            }

            if (World.Instance.InGameCamera.Mode == InGameCamera.EMode.SIDE)
            {
                if (World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.Z)
                {
                    v.z = 0.0f;
                }
                else if (World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.X)
                {
                    v.x = 0.0f;
                }
            }

            m_owner.cmptMovement.UpdatePosition(v, m_owner.backwardSpeed, true);

            if (Physics.Raycast(transform.position, v, out RaycastHit hitInfo, 1.0f, (1 << (int)eLayer.EnvObject) | 
																					 (1 << (int)eLayer.Wall) | 
																					 (1 << (int)eLayer.Wall_Inside)))
            {
                break;
            }

            yield return mWaitForFixedUpdate;
        }
    }

	public override void OnEnd() {
		base.OnEnd();
        m_owner.PlayAni( eAnimation.Idle01 );
    }

	private eDirection GetCurDirection() {
        if ( m_listDirIndex.Count <= 0 ) {
            InitListDir();
        }

		int dir = m_listDirIndex[Random.Range( 0, m_listDirIndex.Count )];
		m_listDirIndex.Remove( dir );

		switch ( (eDirection)dir ) {
			case eDirection.LeftForward:
			case eDirection.RightForward:
			case eDirection.Forward: {
                m_curAni = eAnimation.Run;
            }
            break;

			case eDirection.Right:
			case eDirection.Left:
			case eDirection.Back:
			case eDirection.LeftBack:
			case eDirection.RightBack: {
                m_curAni = eAnimation.RunBack;
            }
            break;

			case eDirection.RightDash: {
				m_curAni = eAnimation.RightDash;
			}
			break;

            case eDirection.LeftDash: {
                m_curAni = eAnimation.LeftDash;
            }
            break;

            default: {
                Debug.LogError( dir + " 이런 번호의 방향은 없는데??" );
			}
            break;
		}

        if ( !m_turn && mTargetCollider ) {
            m_owner.LookAtTarget( mTargetCollider.Owner.transform.position );
        }

		return (eDirection)dir;
	}
}
