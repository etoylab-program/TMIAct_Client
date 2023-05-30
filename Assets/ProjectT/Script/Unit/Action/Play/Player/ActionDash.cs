
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;


public class ActionDash : ActionBase
{
    public enum eState
    {
        Start = 0,
        Doing,
        End,
    }


    [Header("Property")]
    public bool     ShowMeshOnFrontDash             = true;
    public float    DashSpeedRatio                  = 5.0f;
    public float    BackDashSpeedRatio              = 3.0f;
    public float    DashTime                        = 0.2f;
    public bool     ReleaseSuperArmorAfterCutFrame  = false;    // 에밀리 같은 경우 대쉬 컷 프레임이 끝나자마자 무적을 풀어줌

    public bool         BlockEmergencyEvade { get; set; }           = false;
    public Vector3      Dir                 { get; protected set; } = Vector3.zero;
    public float        CurDashSpeedRatio   { get; protected set; } = 0.0f;
    public eAnimation   CurAni              { get; protected set; } = eAnimation.Dash;
    public bool         IsNoDir             { get; protected set; } = false;
    public float        DashDistance        { get; private set; }   = 0.0f;
    public float        SlowTime            { get; private set; }   = 0.0f;

    protected Player                mOwnerPlayer                = null;
    protected State                 mState                      = new State();
    
    protected bool                  mEmergencyEvade             = false;
    protected bool                  mEvading                    = false;
    protected int                   mChainCount                 = 0;
    protected PostProcessProfile    mSlowMotionInvertProfile    = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Defence;

        mOwnerPlayer = m_owner as Player;

        conditionActionCommand = new eActionCommand[5];
        conditionActionCommand[0] = eActionCommand.MoveByDirection;
        conditionActionCommand[1] = eActionCommand.Attack01;
        conditionActionCommand[2] = eActionCommand.Hit;
        conditionActionCommand[3] = eActionCommand.Defence;
        conditionActionCommand[4] = eActionCommand.ChargingAttack;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Grounded;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.Defence;

        cancelDuringSameActionCount = 1;
        cancelDuringSameActionByCutFrame = true;

        superArmor = Unit.eSuperArmor.None;//superArmor = Unit.eSuperArmor.Invincible;

		mState.Init(3);
        mState.Bind(eState.Start, ChangeStartState);
        mState.Bind(eState.Doing, ChangeDoingState);
        mState.Bind(eState.End, ChangeEndState);

        mSlowMotionInvertProfile = ResourceMgr.Instance.LoadFromAssetBundle("etc", "Etc/PostProcess/special_skill_ryu.asset") as PostProcessProfile;
    }

    public override void OnStart(IActionBaseParam param)
    {
        World.Instance.InGameCamera.StopAni();

        if (BlockEmergencyEvade)
        {
            m_endUpdate = true;
        }
        else
        {
            base.OnStart(param);
            m_owner.TemporaryInvincible = true;

            mParamAI = param as ActionParamAI;

            ++mChainCount;

            mEmergencyEvade = false;
            if (m_owner.actionSystem.BeforeActionCommand == eActionCommand.Hit && m_owner.actionSystem.BeforeHitActionState == ActionHit.eState.Normal)
            {
                mEmergencyEvade = true;
            }

            mState.ChangeState(eState.Start, true);
        }
    }

    public override IEnumerator UpdateAction()
    {
        bool showMesh = false;

        m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            if (!IsNoDir)
                m_owner.cmptRotate.UpdateRotation(Dir, true);

            if (m_checkTime < DashTime)
            {
                m_checkTime += m_owner.fixedDeltaTime;
                m_owner.cmptMovement.UpdatePosition(Dir, m_owner.originalSpeed * CurDashSpeedRatio, true);

                if (!showMesh && m_checkTime >= DashTime)
                {
                    m_owner.ShowMesh(true);
                    showMesh = true;
                }
            }
            else if ((eState)mState.current == eState.Doing)
            {
                if (m_owner.isGrounded == true)
                {
                    mState.ChangeState(eState.End, true);
                }
                else if (m_owner.isGrounded == false && m_owner.IsBeforeGround(0.2f))
                {
                    m_owner.SetGroundedRigidBody();
                    mState.ChangeState(eState.End, true);
                }
            }
            else if ((eState)mState.current == eState.End && m_owner.aniEvent.IsAniPlaying(eAnimation.Jump03) == eAniPlayingState.End)
            {
                m_endUpdate = true;
            }
            else
            {
                m_checkTime += m_owner.fixedDeltaTime;
                if (ReleaseSuperArmorAfterCutFrame && m_checkTime >= m_aniCutFrameLength && m_owner.TemporaryInvincible) //m_owner.CurrentSuperArmor == Unit.eSuperArmor.Invincible)
                {
                    //m_owner.RestoreSuperArmor(mChangedSuperArmorId);
                    m_owner.TemporaryInvincible = false;
                }

                if (m_checkTime >= m_aniLength && m_owner.isGrounded)
                {
                    m_endUpdate = true;
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        BlockEmergencyEvade = false;
        mChainCount = 0;

        m_owner.ShowMesh(true);
        m_owner.TemporaryInvincible = false;
        m_owner.cmptMovement.UpdatePosition(Vector3.zero, 0.0f, true);

        if (!m_owner.isGrounded)
        {
            m_owner.SetFallingRigidBody();
        }

        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);
    }

    public override void OnCancel()
    {
        isCancel = true;
        BlockEmergencyEvade = false;

        if (m_owner.actionSystem.NextActionCommand == eActionCommand.Attack01 ||
            m_owner.actionSystem.NextActionCommand == eActionCommand.RushAttack ||
            m_owner.actionSystem.NextActionCommand == eActionCommand.Teleport)
        {
            mChainCount = 0;
        }

        m_owner.ShowMesh(true);
        //m_owner.RestoreSuperArmor(mChangedSuperArmorId);
        m_owner.TemporaryInvincible = false;
        m_owner.cmptMovement.UpdatePosition(Vector3.zero, 0.0f, true);

        if (!m_owner.isGrounded)
        {
            m_owner.SetFallingRigidBody();
        }

        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);
    }

    public bool IsPossibleToDashAttack()
    {
        if (mChainCount == 1 && (CurAni == eAnimation.Dash || CurAni == eAnimation.BackDash))
        {
            return true;
        }

        return false;
    }

    public void InitChainCount()
    {
        mChainCount = 0;
    }

    public float GetEvadeCutFrameLength()
    {
        float cutFrameLength1 = m_owner.aniEvent.GetCutFrameLength(eAnimation.Dash);
        float cutFrameLength2 = m_owner.aniEvent.GetCutFrameLength(eAnimation.BackDash);

        if (cutFrameLength1 < cutFrameLength2)
            return cutFrameLength1;

        return cutFrameLength2;
    }

	protected virtual bool ChangeStartState( bool changeAni ) {
		Utility.IgnorePhysics( eLayer.Player, eLayer.Enemy );
		Utility.IgnorePhysics( eLayer.Player, eLayer.EnemyGate );

		IsNoDir = false;
		Vector3 inputDir = Vector3.zero;
		if ( mParamAI != null ) {
			inputDir = Utility.GetDirectionVector( m_owner, mParamAI.Direction );
		}
		else {
			inputDir = m_owner.Input.GetDirection();
		}

		if ( m_owner.isGrounded && ( mEmergencyEvade || inputDir == Vector3.zero ) ) {
			if ( World.Instance.StageType == eSTAGETYPE.STAGE_PVP && inputDir == Vector3.zero && m_owner.NeedEscape() ) {
				Dir = m_owner.transform.forward;
			}
			else {
				Dir = -m_owner.transform.forward;
			}

			IsNoDir = true;
			CurDashSpeedRatio = BackDashSpeedRatio;

			CurAni = eAnimation.BackDash;
		}
		else {
			Dir = m_owner.isGrounded ? inputDir : m_owner.transform.forward;
			CurDashSpeedRatio = DashSpeedRatio;

			CurAni = eAnimation.Dash;
		}

		if ( mEmergencyEvade ) {
			CurAni = eAnimation.DashOnHit;

			float duration = m_owner.aniEvent.GetAniLength( CurAni ) * 0.5f;
			m_owner.afterImg.Play( m_owner.aniEvent.listSkinnedMesh.ToArray(), duration, duration * 0.1f, duration * 2.0f, new Color( 0.1f, 0.1f, 0.1f ) );
		}

		RaycastHit hitInfo;
		if ( Physics.Raycast( m_owner.MainCollider.GetCenterPos(), Dir, out hitInfo, CurDashSpeedRatio, 
                              ( 1 << (int)eLayer.EnvObject ) | ( 1 << (int)eLayer.Wall ) ) ) {
            if ( hitInfo.distance < CurDashSpeedRatio ) {
                CurDashSpeedRatio = hitInfo.distance <= 1.0f ? 0.0f : hitInfo.distance;
            }
		}

		Dir = new Vector3( Dir.x, 0.0f, Dir.z );
		m_owner.StopStepForward();

		if ( ShowMeshOnFrontDash && !IsNoDir ) {
			m_owner.ShowMesh( false );
		}

		m_aniLength = m_owner.PlayAniImmediate( CurAni );

		DashDistance = ( m_owner.originalSpeed * CurDashSpeedRatio * m_owner.fixedDeltaTime ) * ( DashTime / m_owner.fixedDeltaTime );

		Player player = m_owner as Player;
		if ( mEmergencyEvade ) {
			if ( player != null ) {
				player.ExecuteBattleOption( BattleOption.eBOTimingType.OnEmergencyEvade, 0, null );

				ActionSelectSkillBase action = m_owner.actionSystem.GetAction<ActionSelectSkillBase>( eActionCommand.EmergencyAttack );
				if ( ( action && action.PossibleToUse ) || ( action == null && m_owner.actionSystem.HasAction( eActionCommand.EmergencyAttack ) ) ) {
					m_endUpdate = true;
					SetNextAction( eActionCommand.EmergencyAttack, null );
				}
			}
		}
		else if ( CurAni == eAnimation.Dash || CurAni == eAnimation.BackDash ) {
			if ( player ) {
				player.ExecuteBattleOption( BattleOption.eBOTimingType.OnEvade, 0, null );
			}

			ActionSelectSkillBase actionExtreamEvade = m_owner.actionSystem.GetAction<ActionSelectSkillBase>( eActionCommand.ExtreamEvade );
			if ( actionExtreamEvade && actionExtreamEvade.PossibleToUse && World.Instance.EnemyMgr.HasAliveMonster() ) {
				Unit evadedTarget = World.Instance.EnemyMgr.GetEvadedTarget( m_owner );
				m_owner.SetEvadedTarget( evadedTarget );

				if ( !mOwnerPlayer.ExtreamEvading && ( evadedTarget != null || GetEvadedProjectile() ) ) {
					SlowTime = StartEvading();

					ActionParamExtreamEvade actionParamExtreamEvade = new ActionParamExtreamEvade( CurAni, 0.0f, Vector3.zero, 0.0f );
					SetNextAction( eActionCommand.ExtreamEvade, actionParamExtreamEvade );

					m_endUpdate = true;
				}
				else {
					m_nextAction = eActionCommand.None;
				}
			}
		}

		if ( player ) {
			player.ExecuteBattleOption( BattleOption.eBOTimingType.OnAllEvade, 0, null );
		}

		return true;
	}

	protected virtual float StartEvading()
    {
        if( mOwnerPlayer && mOwnerPlayer.IsHelper ) {
            return 0.0f;
		}

        float length = m_owner.aniEvent.GetAniLength(CurAni);
        float slowTime = length * GameInfo.Instance.BattleConfig.EvadeSlowMotionAniRate;

        World.Instance.SetSlowTime(GameInfo.Instance.BattleConfig.EvadeSlowTimeScale, slowTime);
        World.Instance.InGameCamera.EnableMotionBlur(slowTime);

        return slowTime;
    }

    protected bool GetEvadedProjectile()
    {
        List<Projectile> listProjectile = World.Instance.ProjectileMgr.GetProjectile(m_owner.GetCenterPos(), GameInfo.Instance.BattleConfig.EvadeProjectileDistance);

        Projectile nearestProjectile = null;
        float compare = 9999.0f;
        for (int i = 0; i < listProjectile.Count; i++)
        {
            Projectile projectile = listProjectile[i];
            if (!projectile || !projectile.IsActivate())
            {
                continue;
            }

            // 같은편 프로젝타일은 거르고
            if(Utility.IsMySideLayer((eLayer)m_owner.gameObject.layer, (eLayer)projectile.owner.gameObject.layer))

            {
                continue;
            }

            float dist = Vector3.Distance(projectile.transform.position, m_owner.transform.position);
            if (dist < compare)
            {
                nearestProjectile = projectile;
                compare = dist;
            }
        }

        if (!nearestProjectile || !nearestProjectile.owner)
        {
            return false;
        }

        m_owner.SetEvadedTarget(nearestProjectile.owner);
        return true;
    }

    private bool ChangeDoingState(bool changeAni)
    {
        m_owner.SetFallingRigidBody();
        m_owner.PlayAniImmediate(eAnimation.Jump02);

        return true;
    }

    private bool ChangeEndState(bool changeAni)
    {
        m_owner.PlayAniImmediate(eAnimation.Jump03);
        return true;
    }
}
