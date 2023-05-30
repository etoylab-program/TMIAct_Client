
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RootMotion;
using CodeStage.AntiCheat.ObscuredTypes;


public class Enemy : Unit
{
    protected enum eShieldBreakEffectType
    {
        NONE = 0,
        STUN,
        GROGGY,
        NOTHING,
    }

    
    [Header("[Enemy Property]")]
    public float        dissolveDuration    = 0.7f;
    public Vector3      addHpPos            = Vector3.zero;
    public Vector3      addHpLyingPos       = Vector3.zero;
    public int          BaseTaimaninTableId = 0;

    [Header("[Enemy Change Mtrl Mesh]")]
    public Renderer[]   ChangeMtrlMesh; 

    [Header("[Enemy Battle Option]")]
    public int[]        BOSetIds;


    public List<EnemyMesh>                  ListEnemyMesh       { get; set; } = new List<EnemyMesh>();
    public List<Enemy>                      ListMinion          { get; set; } = new List<Enemy>();
    public bool                             SummoningMinion     { get; set; } = false;
    public bool                             IsSummonEnemy       { get; set; } = false;
    public bool                             LockTarget          { get; set; } = false;
    public BattleSpawnPoint                 myBattleSpawnPoint  { get; set; }
    public float                            checkAttackTime     { get; set; } // 디펜스, 블록킹 모드에서 몬스터가 플레이어를 타겟으로 잡았을 때 5초 간 공격을 안하면 원래 타겟으로 돌아갈 때 쓰임
    public Enemy                            ChangeEnemy         { get; protected set; } = null;
    public List<BOEnemy>                    ListBO              { get; protected set; } = new List<BOEnemy>();
    public GameClientTable.Monster.Param    data                { get { return m_data; } }

    protected GameClientTable.Monster.Param m_data					= null;
    protected Coroutine                     m_crChargeShield		= null;
    protected float                         m_chargeTime			= 0.0f;
    protected float                         m_atkImpossibleTime		= 0;
    protected List<Material>                mListSkinnedMeshMtrlDie	= new List<Material>();
	protected List<Material>				mListMeshMtrlDie		= new List<Material>();
    protected Coroutine                     mCrAggro				= null;
    protected ActionParamHit                mActionParamHit         = new ActionParamHit();
    protected bool                          mStartDieDissolve       = false;
    protected List<Material>                mListDissolveMtrl       = new List<Material>();


    public override void Init( int tableId, eCharacterType type, string faceAniControllerPath ) {
		ListBO.Clear();

		for( int i = 0; i < BOSetIds.Length; i++ ) {
			BOEnemy bo = new BOEnemy(BOSetIds[i], this);
			ListBO.Add( bo );
		}

		Debug.Log( "Enemy Table Id : " + tableId );
		base.Init( tableId, type, faceAniControllerPath );

		m_originalMass = Mathf.Min( 3000, m_rigidBody.mass );
		m_massOnFloating = m_rigidBody.mass * 100.0f;

		if( !m_actionSystem.HasNoAction() ) {
			ActionBase actionBase = m_actionSystem.GetAction(eActionCommand.Die);
			if( actionBase == null ) {
				m_actionSystem.AddAction( gameObject.AddComponent<ActionDie>(), 0, null );
			}

			actionBase = m_actionSystem.GetAction( eActionCommand.Die );
			actionBase.OnStartCallback = StartDropItem;
			actionBase.OnEndCallback = OnDie;

			actionBase = m_actionSystem.GetAction( eActionCommand.Hit );
			actionBase.OnEndCallback = ResetBT;

			for( int i = 0; i < m_actionSystem.ListAction.Count; i++ ) {
				m_actionSystem.ListAction[i].InitAfterOwnerInit();
			}

			AddAIController( m_data.AI );
		}

		m_noAttack = true;
		checkAttackTime = 5.0f;
		IsSummonEnemy = false;

		Material src = ResourceMgr.Instance.LoadFromAssetBundle("etc", "Etc/Material/mat_fx_behimos_die_dissolve.mat") as Material;

		List <Material> list = m_aniEvent.GetSkinnedMeshMtrls();
		for( int i = 0; i < list.Count; ++i ) {
			Material mtrl = new Material(src);
			if( mtrl ) {
				mtrl.SetColor( "_Main_Tex_Color", Color.white );
				mListSkinnedMeshMtrlDie.Add( mtrl );
			}
		}

		list = m_aniEvent.GetMeshMtrls();
		for( int i = 0; i < list.Count; ++i ) {
			Material mtrl = new Material(src);
			if( mtrl ) {
				mtrl.SetColor( "_Main_Tex_Color", Color.white );
				mListMeshMtrlDie.Add( mtrl );
			}
		}

		ChangeMainTexture();

		EnemyMesh[] enemyMeshes = GetComponentsInChildren<EnemyMesh>();
		for( int i = 0; i < enemyMeshes.Length; i++ ) {
			EnemyMesh enemyMesh = enemyMeshes[i];
			enemyMesh.Init( this );

			ListEnemyMesh.Add( enemyMesh );
		}
	}

	public override void SetData( int tableId ) {
		m_data = GameInfo.Instance.GetMonsterData( tableId );
        if( m_data == null ) {
            return;
        }

		tableName = FLocalizeString.Instance.GetText( m_data.Name );

		m_tableId = tableId;
		m_monType = (eMonType)m_data.MonType;
		m_grade = (eGrade)m_data.Grade;
		m_groggy = m_data.ShieldBreakEffect == (int)eShieldBreakEffectType.GROGGY ? true : false;
        m_folder = Utility.GetFolderFromPath( m_data.ModelPb );

        SetStats();

        Director director = null;
		if( !string.IsNullOrEmpty( m_data.BossAppear ) && HasDirector( "BossAppear" ) == false ) {
			director = GameSupport.CreateDirector( m_data.BossAppear );
			AddDirector( "BossAppear", director );
		}

		if( !string.IsNullOrEmpty( m_data.DieDrt ) && HasDirector( "BossDie" ) == false ) {
			director = GameSupport.CreateDirector( m_data.DieDrt );
			AddDirector( "BossDie", director );
		}

        if( !string.IsNullOrEmpty( m_data.FxSndArmorHit ) ) {
            SoundManager.Instance.AddAudioClip( "ArmorHit", "Sound/" + m_data.FxSndArmorHit, FSaveData.Instance.GetSEVolume() );
        }

        if( !string.IsNullOrEmpty( m_data.FxSndArmorBreak ) ) {
            SoundManager.Instance.AddAudioClip( "ArmorBreak", "Sound/" + m_data.FxSndArmorBreak, FSaveData.Instance.GetSEVolume() );
        }

        if( !string.IsNullOrEmpty( m_data.HitSnd ) ) {
            SoundManager.Instance.AddAudioClip( "HitSnd", "Sound/" + m_data.HitSnd, FSaveData.Instance.GetSEVolume() );
        }

		m_listAniHit.Clear();
		m_listAniHit.Add( GetHitValue( m_data.Hit_01 ) );
		m_listAniHit.Add( GetHitValue( m_data.Hit_02 ) );
		m_listAniHit.Add( GetHitValue( m_data.Hit_03 ) );
		m_listAniHit.Add( GetHitValue( m_data.Hit_04 ) );

		if( !string.IsNullOrEmpty( m_data.EffBossHit ) ) {
			m_psHit = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>( "effect", m_data.EffBossHit + ".prefab" );

			if( m_psHit != null ) {
				m_psHit.transform.SetParent( transform );
				Utility.InitTransform( m_psHit.gameObject );
				Utility.SetLayer( m_psHit.gameObject, (int)eLayer.Default, true );

				m_psHit.gameObject.SetActive( false );
			}
		}

		if( m_data.ChangeId > 0 && ChangeEnemy == null ) {
			ChangeEnemy = GameSupport.CreateEnemyWithoutAddEnemy( m_data.ChangeId );
			ChangeEnemy.Deactivate();
		}

		if( !string.IsNullOrEmpty( m_data.MonBOSetList ) ) {
			string[] split = Utility.Split(m_data.MonBOSetList, ',');

			for( int i = 0; i < split.Length; i++ ) {
				BOEnemy bo = new BOEnemy(int.Parse(split[i]), this);
				ListBO.Add( bo );
			}
		}
	}

    public virtual void SetParent( Enemy parent ) {
        m_parent = parent;
        transform.parent = m_parent.transform;
    }

    public void SetChild( Enemy child ) {
        m_child = child;
    }

    public void AddBOSet( int id ) {
        GameClientTable.BattleOptionSet.Param paramBOSet = GameInfo.Instance.GameClientTable.FindBattleOptionSet(id);
        if( paramBOSet == null ) {
            return;
        }

        GameClientTable.BattleOption.Param paramBO = GameInfo.Instance.GameClientTable.FindBattleOption(paramBOSet.BattleOptionID);
        if( paramBO == null ) {
            return;
        }

        if( m_actionSystem && paramBO.CheckTimingType.CompareTo( "OnAttack" ) == 0 ) {
            List<ActionEnemyBase> list = m_actionSystem.GetActionList<ActionEnemyBase>();
            for( int i = 0; i < list.Count; i++ ) {
                ActionEnemyBase actionEnemyBase = list[i];
                if( actionEnemyBase == null ) {
                    continue;
                }

                ActionEnemyAttackBase actionAtkBase = actionEnemyBase as ActionEnemyAttackBase;
                if( actionAtkBase ) {
                    actionAtkBase.AddBOSet( id );
                    continue;
                }

                ActionEnemyTaimaninBase actionEnemyTaimaninBase = actionEnemyBase as ActionEnemyTaimaninBase;
                if( actionEnemyTaimaninBase ) {
                    actionEnemyTaimaninBase.AddBOSet( id );
                }
            }
        }
        else {
            BOEnemy bo = new BOEnemy(id, this);
            ListBO.Add( bo );
        }
    }

    public override void Activate() {
        base.Activate();

        if( isClone ) {
            return;
        }

        if( m_grade == eGrade.Boss ) {
            mUIBoss.gameObject.SetActive( true );
        }

        if( World.Instance.InGameCamera.Mode == InGameCamera.EMode.SIDE ) {
            SetLockAxis( World.Instance.InGameCamera.SideSetting.LockAxis );
        }
        else {
            SetLockAxis( eAxisType.None );
        }

        StartBT();
    }

    public override void Deactivate() {
        if( m_cmptBuffDebuff != null ) {
            m_cmptBuffDebuff.Clear();
        }

        if( mUIBuffDebuffIcon ) {
            mUIBuffDebuffIcon.End();
        }

        if( mAI != null && !m_actionSystem.HasNoAction() ) {
            StopBT();
        }

        if( m_aniEvent && IsActivate() ) {
            m_aniEvent.PlayAni( eAnimation.Idle01 );
        }

        mAddMaxHpRate = 0.0f;
        base.Deactivate();
    }

    public override bool SubShield( float sub ) {
        if( !base.SubShield( sub ) ) {
            return false;
        }

        UpdateShieldGauge();
        return true;
    }

    public BTBase.eTargetOnStart GetTargetOnStart() {
        if( mAI == null ) {
            return BTBase.eTargetOnStart.None;
        }

        return mAI.targetOnStart;
    }

    public override bool StopBT() {
        if( base.StopBT() ) {
            StopCoroutine( "CheckAttack" );
            return true;
        }

        return false;
    }

    public override bool OnEvent( BaseEvent evt, IActionBaseParam param = null ) {
        if( base.OnEvent( evt ) ) {
            return false;
        }

        switch( evt.eventType ) {
            case eEventType.EVENT_BUFF_ADD_MAX_HP:
                if( !GameSupport.IsBOConditionCheck( evt.battleOptionData.preConditionType, evt.battleOptionData.CondValue, this ) ) {
                    return true;
                }

                float curHpRate = m_curHp / m_maxHp;

                mAddMaxHpRate += evt.value;
                m_maxHp = m_maxHp + ( m_maxHp * mAddMaxHpRate );
                m_curHp = m_maxHp * curHpRate;

                if( grade == eGrade.Boss && World.Instance.Player ) {
                    World.Instance.Player.UpdateTargetHp( this );
                }

                break;
        }

        return false;
    }

    public override bool OnAttack( AniEvent.sEvent evt, float atkRatio, bool notAniEventAtk ) {
        float originalAtkRange = evt.atkRange;
        evt.atkRange *= GameInfo.Instance.BattleConfig.AttackRangeAllowGap;

        bool result = base.OnAttack(evt, atkRatio, notAniEventAtk);
        evt.atkRange = originalAtkRange;

        return result;
    }

    public override void ExecuteBattleOption( BattleOption.eBOTimingType timingType, int actionTableId, Projectile projectile, bool skipWeaponBO = false ) {
        if( timingType == BattleOption.eBOTimingType.OnAttack ) {
            ActionEnemyBase action = null;

            if( projectile && projectile.OwnerAction ) {
                action = projectile.OwnerAction as ActionEnemyBase;
            }
            else {
                action = m_actionSystem.GetCurrentAction<ActionEnemyBase>();
            }

            if( action ) {
                action.ExecuteBattleOption( timingType, projectile );
            }
        }
        else {
            for( int i = 0; i < ListBO.Count; i++ ) {
                ListBO[i].Execute( timingType, 0, null );
            }
        }
    }

    public override bool IsImmuneFloat() {
        if( m_data == null ) {
            return false;
        }

        return m_data.Immune_Fly;
    }

    public override bool IsImmuneDown() {
        if( m_data == null ) {
            return false;
        }

        return m_data.Immune_Down;
    }

    public override bool IsImmuneKnockback() {
        if( m_data == null ) {
            return false;
        }

        return m_data.Immune_KnockBack;
    }

    public override bool IsImmunePulling() {
        if( m_data == null ) {
            return false;
        }

        return m_data.Immune_Pulling;
    }

    public override Unit.eKnockBackType GetKnockBackType() {
        if( m_data == null ) {
            return Unit.eKnockBackType.None;
        }

        return (Unit.eKnockBackType)m_data.KnockBack_Type;
    }

    public override bool AddHp( BattleOption.eToExecuteType toExecuteType, float addPercentage, bool skipCheckDie ) {
        if( !base.AddHp( toExecuteType, addPercentage, skipCheckDie ) ) {
            return false;
		}

        UpdateShieldGauge();
        return true;
    }

    public override bool AddHpPercentage( BattleOption.eToExecuteType toExecuteType, float addPercentage, bool skipCheckDie ) {
        if( !base.AddHpPercentage( toExecuteType, addPercentage, skipCheckDie ) ) {
            return false;
		}

        UpdateShieldGauge();
        return true;
    }

    public override bool AddHpPerCost( BattleOption.eToExecuteType toExecuteType, float addPercentage, bool skipCheckDie ) {
        if( !base.AddHpPerCost( toExecuteType, addPercentage, skipCheckDie ) ) {
            return false;
		}

        UpdateShieldGauge();
        return true;
    }

    public override void SubHp( ObscuredFloat damage, bool isCritical ) {
        base.SubHp( damage, isCritical );
        UpdateShieldGauge();
    }

    public override List<UnitCollider> GetTargetColliderList( bool onlyMainCollider = false ) {
        List<UnitCollider> list = new List<UnitCollider>();
        list.Add( GetMainTargetCollider( true ) );

        return list;
    }

    public override UnitCollider GetMainTargetCollider( bool onlyEnemy, float checkDist = 0.0f, bool skipHasShieldTarget = false, bool onlyAir = false ) {
        if( LockTarget ) {
            if( m_mainTarget == null || m_mainTarget.MainCollider == null ) {
                BTBase.eTargetOnStart targetOnStart = GetTargetOnStart();
                if( targetOnStart == BTBase.eTargetOnStart.Player ) {
                    return World.Instance.Player.MainCollider;
                }
                else if( targetOnStart == BTBase.eTargetOnStart.OtherObject ) {
                    return World.Instance.EnemyMgr.otherObject.MainCollider;
                }
            }
            else {
                return m_mainTarget.MainCollider;
            }
        }

        if( m_mainTarget && ( m_mainTarget.curHp <= 0.0f || !m_mainTarget.IsActivate() ) ) {
            m_mainTarget = null;
        }

        Unit playerOrPlayerMinion = World.Instance.Player.GetHighestAggroUnit( this );
        Unit target = null;

        if( !onlyEnemy ) {
            List<Unit> list = World.Instance.EnemyMgr.GetActiveEnvObjects();
            list.Add( playerOrPlayerMinion );

            World.Instance.EnemyMgr.SortListTargetByNearDistance( this, ref list );
            target = list[0];
        }
        else {
            UnitCollider unitCollider = playerOrPlayerMinion.GetNearestColliderFromPos(transform.position);
            target = unitCollider.Owner;
        }

        SetMainTarget( target );
        return target.MainCollider;
    }

    public override UnitCollider GetRandomTargetCollider( bool onlyEnemy = false ) {
        return GetMainTargetCollider( true );
    }

	public override List<UnitCollider> GetTargetColliderListByAround( Vector3 pos, float radius, bool onlyMainCollider = false ) {
		int enemyLayer = Utility.GetEnemyLayer( (eLayer)gameObject.layer );

		Collider[] cols = Physics.OverlapSphere( pos, radius, enemyLayer );
		if ( cols.Length <= 0 ) {
			return null;
		}

		List<UnitCollider> list = new List<UnitCollider>();

		for ( int i = 0; i < cols.Length; i++ ) {
			UnitCollider capsuleCollider = cols[i].GetComponent<UnitCollider>();

			if ( capsuleCollider == null || !capsuleCollider.IsEnable() || capsuleCollider.Owner.ignoreHit ) {
				continue;
			}

			if ( onlyMainCollider ) {
				if ( capsuleCollider.Owner.MainCollider != capsuleCollider ) {
					continue;
				}
			}

			if ( capsuleCollider == MainCollider ) {
				continue;
			}

			list.Add( capsuleCollider );
		}

		return list;
	}

	public override List<UnitCollider> GetEnemyColliderList() {
        mListTempUnitCollider.Clear();

        for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
            if( !World.Instance.ListPlayer[i].IsActivate() ) {
                continue;
            }

            mListTempUnitCollider.Add( World.Instance.ListPlayer[i].MainCollider );
        }

        List<Unit> listTarget = World.Instance.EnemyMgr.GetActiveEnvObjects();
        for( int i = 0; i < listTarget.Count; i++ ) {
            mListTempUnitCollider.Add( listTarget[i].MainCollider );
        }

        return mListTempUnitCollider;
    }

    public override List<Unit> GetEnemyList( bool onlyEnemy = false ) {
        mListTempUnit.Clear();

        for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
            if( !World.Instance.ListPlayer[i].IsActivate() ) {
                continue;
            }

            mListTempUnit.Add( World.Instance.ListPlayer[i] );
        }

        return mListTempUnit;
    }

    public override bool OnHit( Unit attacker, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent attackerAniEvt, ObscuredFloat damage, 
                                ref bool isCritical, ref eHitState hitState, Projectile projectile, bool isUltimateSkill, bool skipMaxDamageRecord ) {
    #if UNITY_EDITOR
        if( GameInfo.Instance.BattleConfig.TestPlayerAtkPower > 0.0f ) {
            damage = GameInfo.Instance.BattleConfig.TestPlayerAtkPower;
        }
    #endif

        bool breakShield = base.OnHit( attacker, toExecuteType, attackerAniEvt, damage, ref isCritical, ref hitState, projectile, isUltimateSkill, skipMaxDamageRecord );
        if( m_parent ) {
            return false;
        }

        if( m_grade == eGrade.Boss ) {
            World.Instance.Player.UpdateTargetHp( this );
        }

        if( hitState == eHitState.OnlyEffect ) {
            return false;
        }

        if( m_data.ShieldBreakEffect == (int)eShieldBreakEffectType.NOTHING ) {
            breakShield = false;
        }

        mActionParamHit.Set( attacker, attackerAniEvt.behaviour, attacker.GetAirAttackJumpPower() * floatingJumpPowerRatio, attackerAniEvt.hitDir,
                             attackerAniEvt.hitEffectId, isCritical, attacker.aniEvent ? attacker.aniEvent.GetCurCutFrameLength() : 0.0f,
                             hitState, attackerAniEvt.atkAttr );

        ActionEnemyCounterAttackBase actionEnemyCounterAttackBase = m_actionSystem.GetCurrentAction<ActionEnemyCounterAttackBase>();

        if( actionEnemyCounterAttackBase ) {
            actionEnemyCounterAttackBase.OnUpdating( null );
        }
        else if( hitState == eHitState.Success && attacker.actionSystem != null ) {
            StopBT();
            ActionHit actionHit = m_actionSystem.GetCurrentAction<ActionHit>();

            if ( !attacker.aniEvent.IsCurAttack( eBehaviour.FastFallAttack ) && attacker.actionSystem.IsCurrentSkillAction() ) {
                reserveStun = false;

                if( actionHit == null || actionHit.State != ActionHit.eState.Stun ) {
                    m_actionSystem.CancelCurrentAction();
                    actionHit = null;
                }
            }

            if( actionHit != null && actionHit.State != ActionHit.eState.StandUp ) {
                if( actionHit.State == ActionHit.eState.Normal && attackerAniEvt.behaviour != eBehaviour.Attack ) {
                    m_actionSystem.CancelCurrentAction();
                    CommandAction( eActionCommand.Hit, mActionParamHit );
                }
                else {
                    actionHit.OnUpdating( mActionParamHit );
                }
            }
            else {
                m_actionSystem.CancelCurrentAction();
                CommandAction( eActionCommand.Hit, mActionParamHit );
            }
        }
        else if( breakShield && hitState == eHitState.OnlyDamage ) {
            if( m_aniEvent.aniSpeed <= 0.0f ) {
                RestoreSpeed();
            }

            m_actionSystem.CancelCurrentAction();
            StopBT();

            eBehaviour behaviour = attackerAniEvt.behaviour;
            if( behaviour != eBehaviour.StunAttack || behaviour == eBehaviour.Projectile ) {
                behaviour = eBehaviour.StunAttack;
            }

            mActionParamHit.Set( attacker, behaviour, attacker.GetAirAttackJumpPower() * 1.17f, eHitDirection.Up, mActionParamHit.hitEffId,
                                 mActionParamHit.critical, mActionParamHit.attackerAniCutFrameLength, mActionParamHit.hitState, attackerAniEvt.atkAttr );

            CommandAction( eActionCommand.Hit, mActionParamHit );

            if( m_crChargeShield != null ) {
                Utility.StopCoroutine( this, ref m_crChargeShield );
            }
        }
        else {
            if( hitState == eHitState.OnlyDamage )
            {
                if( m_curHp <= 0.0f ) {
                    ActionParamDie paramDie = null;
                    ActionHit actionHit = m_actionSystem.GetAction<ActionHit>(eActionCommand.Hit);
                    if( actionHit && ( actionHit.State == ActionHit.eState.Float || actionHit.State == ActionHit.eState.Down ) ) {
                        paramDie = new ActionParamDie( ActionParamDie.eState.Down, null );
                    }

                    m_actionSystem.CancelCurrentAction();
                    CommandAction( eActionCommand.Die, paramDie );
                }
                else if( m_curShield > 0.0f ) {
                    Utility.StopCoroutine( this, ref m_crRestoreShield );
                    Utility.StopCoroutine( this, ref m_crChargeShield );
                    m_crChargeShield = StartCoroutine( ChargingShield() );
                }
            }
            else if( actionSystem.currentAction ) {
                actionSystem.currentAction.OnUpdating( mActionParamHit );
            }
        }

        if( m_curHp > 0.0f ) {
            if( mAI && mAI.CheckAggro && ( m_curHp / m_maxHp ) <= mAI.AggroHpPercentage ) {
                if( mCrAggro != null ) {
                    Utility.StopCoroutine( this, ref mCrAggro );
                }

                mCrAggro = StartCoroutine( UpdateAggro( attacker ) );
            }
        }
        else if( mCrAggro != null ) {
            Utility.StopCoroutine( this, ref mCrAggro );
        }

        if( !string.IsNullOrEmpty( m_data.HitSnd ) ) {
            SoundManager.Instance.PlaySnd( SoundManager.eSoundType.Monster, "HitSnd" );
        }

        return breakShield;
    }

    public override void OnDie() {
        if( m_child ) {
            m_child.OnDie();
        }

        base.OnDie();
        if( !IsActivate() ) {
            return;
        }

        if( !mStartDieDissolve && m_aniEvent && dissolveDuration > 0.0f ) {
            if( mListSkinnedMeshMtrlDie.Count > 0 ) {
                m_aniEvent.SetSkinnedMeshMtrl( mListSkinnedMeshMtrlDie, "_Main_Tex" );
            }

            if( mListMeshMtrlDie.Count > 0 ) {
                m_aniEvent.SetMeshMtrl( mListMeshMtrlDie, "_Main_Tex" );
            }

            StartCoroutine( "UpdateDissolve" );
        }
        else if ( dissolveDuration <= 0.0f ) {
            if ( gameObject.layer == (int)eLayer.Enemy ) {
                World.Instance.EnemyMgr.DeactivateEnemy();
            }

            Deactivate();
		}
    }

    public override void Pause() {
        if( !IsActivate() || m_curHp <= 0.0f ) {
            return;
        }

        base.Pause();
        StopBT();

        if( m_child ) {
            m_child.Pause();
        }
    }

    public override void Resume() {
        if( m_curHp <= 0.0f || !IsActivate() ) {
            return;
        }

        base.Resume();
        StartBT();

        if( m_child ) {
            m_child.Resume();
        }
    }

    public override void Show() {
        base.Show();
        ResetBT();
    }

    public override void Hide() {
        base.Hide();
        StopBT();
    }

    public override bool IsAttackImpossible() {
        if( mAI == null ) {
            return false;
        }

        return m_atkImpossibleTime > 0;
    }

    public EnemyMesh GetEnemyMeshByName( string name ) {
        for( int i = 0; i < ListEnemyMesh.Count; i++ ) {
            if( ListEnemyMesh[i].name.CompareTo( name ) == 0 ) {
                return ListEnemyMesh[i];
            }
        }

        return null;
    }

    public bool IsAllMinionDead() {
        bool allDead = true;

        for( int i = 0; i < ListMinion.Count; i++ ) {
            if( ListMinion[i].curHp > 0.0f ) {
                allDead = false;
                break;
            }
        }

        return allDead;
    }

    protected override void LoadShieldEffects() {
        if( !string.IsNullOrEmpty( m_data.EffShield ) ) {
            m_psShieldOnHit = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>( "effect", "Effect/prf_fx_shield.prefab" );

            if( m_psShieldOnHit != null )
                m_psShieldOnHit.gameObject.SetActive( false );
        }

        if( !string.IsNullOrEmpty( m_data.EffShieldBreak ) ) {
            m_psShieldBreak = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>( "effect", "Effect/prf_fx_broken_shield.prefab" );

            if( m_psShieldBreak != null )
                m_psShieldBreak.gameObject.SetActive( false );
        }

        if( !string.IsNullOrEmpty( m_data.EffShieldAttack ) ) {
            m_psShieldAttack = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>( "effect", "Effect/prf_fx_shield_crash.prefab" );

            if( m_psShieldAttack != null )
                m_psShieldAttack.gameObject.SetActive( false );
        }
    }

    protected override void LoadHUDUI() {
        base.LoadHUDUI();

        if( m_grade != Unit.eGrade.Boss ) {
            mUIHpBar = ResourceMgr.Instance.CreateFromAssetBundle<UIHpBar>( "ui", "UI/EnemyHp.prefab" );

            if( mUIHpBar ) {
                mUIHpBar.name = "HpBar_" + name;
                mUIHpBar.transform.SetParent( transform );
                Utility.InitTransform( mUIHpBar.gameObject );
                mUIHpBar.Show( false );
            }
        }
        else {
            mUIBoss = ResourceMgr.Instance.CreateFromAssetBundle<UIBoss>( "ui", "UI/icoBoss.prefab" );

            if( mUIBoss ) {
                mUIBoss.name = "IconBoss_" + name;
                mUIBoss.transform.SetParent( transform );
                Utility.InitTransform( mUIBoss.gameObject );

                mUIBoss.Init( this );
            }
        }
    }

    protected override void DropItem() {
        base.DropItem();
        DropItemMgr.Instance.DropItem( this );
    }

    protected override void Reborn() {
        base.Reborn();

        if( isClone ) {
            return;
        }

        SetStats();
        InitAttackImpossibleTime();

        if( m_crChargeShield != null )
            Utility.StopCoroutine( this, ref m_crChargeShield );

        if( m_aniEvent ) {
            m_aniEvent.RestoreOriginalColor();
        }
    }

    protected override void EndDotShieldDmg() {
        if( m_curShield <= 0.0f && m_data.ShieldBreakEffect != (int)eShieldBreakEffectType.NOTHING ) {
            BreakShield();
            StunAfterShieldBreak();
        }
        else {
            Utility.StopCoroutine( this, ref m_crRestoreShield );
            Utility.StopCoroutine( this, ref m_crChargeShield );

            m_crChargeShield = StartCoroutine( ChargingShield() );
        }
    }

    protected override void UpdateShieldGauge() {
        if( World.Instance.Player == null ) {
            return;
        }

        if( m_grade == eGrade.Boss ) {
            Unit playerMainTarget = World.Instance.Boss;
            if( playerMainTarget == null || playerMainTarget != this ) {
                return;
            }

            World.Instance.Player.UpdateTargetHp( playerMainTarget );
        }
        else if( m_grade == eGrade.Epic ) {
            if( mUIHpBar == null ) {
                return;
            }

            mUIHpBar.UpdateShield();
        }
    }

    protected override void StunAfterShieldBreak() {
        Utility.StopCoroutine( this, ref m_crChargeShield );

        m_actionSystem.CancelCurrentAction();
        StopBT();

        CommandAction( eActionCommand.Hit, new ActionParamHit( attacker, eBehaviour.StunAttack, attacker.GetAirAttackJumpPower() * 1.17f, eHitDirection.Up,
                                                               attackerAniEvt.hitEffectId, false, 0.0f, eHitState.Success, EAttackAttr.NORMAL ) );
    }

    protected override bool IsCollisionWall( Vector3 startPos, Vector3 endPos, ref Vector3 HitPoint ) {
        RaycastHit hitInfo;
        if( Physics.Linecast( startPos, endPos, out hitInfo, ( 1 << (int)eLayer.Wall ) ) == true ) {
            return true;
        }

        Collider[] cols = Physics.OverlapBox( MainCollider.GetCenterPos(), new Vector3(0.15f, 0.15f, 0.15f), Quaternion.identity, 
                                              (1 << (int)eLayer.Player) | (1 << (int)eLayer.Wall));
        if( cols.Length > 0 ) {
            bool isMine = false;

            for( int i = 0; i < cols.Length; i++ ) {
                if( cols[i].transform.root.gameObject == gameObject ) {
                    isMine = true;
                    break;
                }
            }

            if( !isMine ) {
                return true;
            }
        }

        return false;
    }

    protected override bool OnChangeColor( float duration, Color startColor, Color endColor ) {
        StartCoroutine( UpdateChangeColor( duration, startColor, endColor ) );
        return true;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if( !Director.IsPlaying ) {
            if( m_atkImpossibleTime > 0 && !World.Instance.IsPause ) {
                m_atkImpossibleTime -= Time.deltaTime;
            }

            if( Mathf.Abs( transform.position.y ) > 150.0f ) {
                SetDie();
                Deactivate();

                if( m_grade == Unit.eGrade.Boss ) {
                    EndGameEvent evt = new EndGameEvent();
                    evt.Set( eEventType.EVENT_GAME_ENEMY_BOSS_DIE, 0.0f );
                    EventMgr.Instance.SendEvent( evt );
                }
            }
        }
    }

	public List<UnitCollider> GetColliderListFromAroundOrNull( Vector3 pos, float radius ) {
		Collider[] colliders = Physics.OverlapSphere( pos, radius, 1 << gameObject.layer );
		if ( colliders.Length <= 0 ) {
			return null;
		}

		List<UnitCollider> unitColliderList = new List<UnitCollider>();

		for ( int i = 0; i < colliders.Length; i++ ) {
			UnitCollider unitCollider = colliders[i].GetComponent<UnitCollider>();

			if ( unitCollider == null || !unitCollider.IsEnable() || unitCollider.Owner.ignoreHit ) {
				continue;
			}

			if ( unitCollider == MainCollider ) {
				continue;
			}

			unitColliderList.Add( unitCollider );
		}

		return unitColliderList;
	}

    private void SetStats() {
        m_maxHp = m_data.MaxHP;
        m_curHp = m_maxHp;

        m_maxShield = Mathf.Max( 0.0f, m_data.Shield );
        m_curShield = m_maxShield;

        if( m_curShield > 0.0f ) {
            mChangedSuperArmorId = SetSuperArmor( eSuperArmor.Lv2 );
        }
        else {
            mChangedSuperArmorId = SetSuperArmor( eSuperArmor.None );
        }

        m_originalSpeed = m_data.MoveSpeed;
        m_curSpeed = m_originalSpeed;
        m_backwardSpeed = m_data.BackwardSpeed;

        m_attackPower = m_data.AttackPower;
        m_defenceRate = m_data.DefenceRate;
        m_criticalRate = m_data.CriticalRate;

        m_scale = m_data.Scale;
        transform.localScale = new Vector3( m_scale, m_scale, m_scale );

        if( World.Instance.StageType == eSTAGETYPE.STAGE_RAID ) {
            IncreaseRaidStats();
            AddRaidStats();
        }
    }

    private void IncreaseRaidStats() {
        m_maxHp = m_maxHp + ( m_maxHp * GameInfo.Instance.BattleConfig.RaidIncHPRate * GameInfo.Instance.SelectedRaidLevel );
        m_curHp = m_maxHp;

        m_attackPower = m_attackPower + ( m_attackPower * GameInfo.Instance.BattleConfig.RaidIncAtkRate * GameInfo.Instance.SelectedRaidLevel );
        
        m_maxShield = m_maxShield + ( m_maxShield * GameInfo.Instance.BattleConfig.RaidIncShieldRate * GameInfo.Instance.SelectedRaidLevel );
        m_curShield = m_maxShield;
    }

    private void AddRaidStats() {
        List<GameClientTable.RaidAddStat.Param> list = GameInfo.Instance.GameClientTable.FindAllRaidAddStat( x => x.RaidStep <= GameInfo.Instance.SelectedRaidLevel );
        if( list == null || list.Count <= 0 ) {
            return;
		}

        for( int i = 0; i < list.Count; i++ ) {
            m_defenceRate = Mathf.Min( m_defenceRate + list[i].AddDef, GameInfo.Instance.BattleConfig.MonsterLimitDef );

            m_originalSpeed = Mathf.Min( m_originalSpeed + list[i].AddSpd, GameInfo.Instance.BattleConfig.MonsterLimitSpd );
            m_curSpeed = m_originalSpeed;

            m_criticalRate = Mathf.Min( m_criticalRate + list[i].AddCriRate, GameInfo.Instance.BattleConfig.MonsterLimitCriRate );
            mAddCriticalDmgRate = Mathf.Min( mAddCriticalDmgRate + list[i].AddCriDmg, GameInfo.Instance.BattleConfig.MonsterLimitCriDmg );
            mCriticalResist = Mathf.Min( mCriticalResist + list[i].AddCriReg, GameInfo.Instance.BattleConfig.MonsterLimitCriReg );
            mCriticalDmgDef = Mathf.Min( mCriticalDmgDef + list[i].AddCriDef, GameInfo.Instance.BattleConfig.MonsterLimitCriDef );
            Penetrate = Mathf.Min( Penetrate + ( list[i].AddPenetrate / (float)eCOUNT.MAX_RATE_VALUE ), GameInfo.Instance.BattleConfig.MonsterLimitPenetrate );
        }
    }

    private void ChangeMainTexture() {
		if( string.IsNullOrEmpty( m_data.Texture ) ) {
			return;
		}

		string[] fileName = Utility.Split(name, ' ');
		fileName[0] = fileName[0].Replace( "(Clone)", "" );
		string path = string.Format("{0}.png", m_data.Texture);

		Texture2D tex = ResourceMgr.Instance.LoadFromAssetBundle("model_monster", path) as Texture2D;
		if( tex == null ) {
			Debug.LogError( path + "에 텍스쳐를 읽어올 수 없습니다." );
			return;
		}
		else {
			if( ChangeMtrlMesh != null && ChangeMtrlMesh.Length > 0 ) {
				for( int i = 0; i < ChangeMtrlMesh.Length; i++ ) {
					ChangeMtrlMesh[i].material.mainTexture = tex;
				}
			}
			else {
				for( int i = 0; i < m_aniEvent.ListMtrl.Count; i++ ) {
					m_aniEvent.ListMtrl[i].mainTexture = tex;
				}
			}
		}
	}

	private void StartDropItem() {
		StopBT();
		DropItem();
	}

	private IEnumerator ChargingShield() {
		yield return new WaitForSeconds( GameInfo.Instance.BattleConfig.SuperArmorRegenStartTime );

		m_chargeTime = 0.0f;
		float fChargePerSec = m_maxShield / GameInfo.Instance.BattleConfig.SuperArmorRegenSpeedTime;

		while( m_curShield < m_maxShield && m_curHp > 0.0f ) {
			if( m_cmptBuffDebuff.IsExecution( eEventType.EVENT_DEBUFF_DOT_SHIELD_DMG ) ) {
				m_crChargeShield = null;
				yield break;
			}

			m_chargeTime += Time.deltaTime;
			m_curShield += ( fChargePerSec * m_chargeTime );

			UpdateShieldGauge();

			m_chargeTime = 0.0f;
			yield return null;
		}

		m_curShield = m_maxShield;
		UpdateShieldGauge();

		m_crChargeShield = null;
	}

	private IEnumerator UpdateAggro( Unit attacker ) {
		SetMainTarget( attacker );

		float checkTime = 0.0f;
		while( checkTime < mAI.AggroEndTime ) {
			checkTime += Time.deltaTime;
			yield return null;
		}

		SetMainTarget( World.Instance.EnemyMgr.otherObject );
	}

	private IEnumerator UpdateChangeColor( float duration, Color startColor, Color endColor ) {
		Color c = Color.white;
		float t = 0.0f;

		while( t < 1.0f && m_curHp > 0.0f ) {
			t += fixedDeltaTime / duration;

			c.r = Mathf.Lerp( startColor.r, endColor.r, t );
			c.g = Mathf.Lerp( startColor.g, endColor.g, t );
			c.b = Mathf.Lerp( startColor.b, endColor.b, t );

			m_aniEvent.SetShaderColor( "_Color", c );
			yield return mWaitForFixedUpdate;
		}
	}

	private IEnumerator UpdateDissolve() {
		if( dissolveDuration > 0.0f ) {
			mStartDieDissolve = true;

			mListDissolveMtrl.Clear();
			mListDissolveMtrl.AddRange( mListSkinnedMeshMtrlDie );
			mListDissolveMtrl.AddRange( mListMeshMtrlDie );

			float value = 1.0f;
			while( value > 0.0f ) {
				for( int i = 0; i < mListDissolveMtrl.Count; i++ ) {
					if( mListDissolveMtrl[i].HasProperty( "_Dissolve_power" ) == false )
						continue;

					mListDissolveMtrl[i].SetFloat( "_Dissolve_power", value );
				}

				value -= Time.fixedDeltaTime / dissolveDuration;
				yield return mWaitForFixedUpdate;
			}

			for( int i = 0; i < mListDissolveMtrl.Count; i++ ) {
				if( mListDissolveMtrl[i].HasProperty( "_Dissolve_power" ) == false )
					continue;

				mListDissolveMtrl[i].SetFloat( "_Dissolve_power", 1.0f );
			}
		}

		StopAllCoroutines();
		m_aniEvent.RestoreMtrl();

		if( gameObject.layer == (int)eLayer.Enemy ) {
			World.Instance.EnemyMgr.DeactivateEnemy();
		}

		mStartDieDissolve = false;

		for( int i = 0; i < mListSkinnedMeshMtrlDie.Count; ++i ) {
			mListSkinnedMeshMtrlDie[i].SetTexture( "_Main_Tex", null );
		}

		for( int i = 0; i < mListMeshMtrlDie.Count; ++i ) {
			mListMeshMtrlDie[i].SetTexture( "_Main_Tex", null );
		}

		Deactivate();
	}

	private void InitAttackImpossibleTime() {
		m_atkImpossibleTime = ( mAI != null ) ? mAI.attackImpossibleTime : 0;
	}
}

