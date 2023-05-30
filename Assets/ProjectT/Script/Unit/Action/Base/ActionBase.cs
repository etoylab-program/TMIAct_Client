
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public interface IActionBaseParam
{
}


public enum eActionCommand
{
    None = 0,

    Idle,
    Appear,

    // Move
    ___StartMove,
    MoveByDirection,
    MoveToTarget,
    HangingAround,
    Jump,
    Defence,
    ___EndMove,

    // Hit
    __StartHit,
    Hit,
    Floating,
    KnockBack,
    Stun,
    StandUp,
    Down,
    Die,
    __EndHit,

    // Attack
    __StartAttack,
    Attack01,
    Attack02,
    Attack03,
    Attack04,
    Attack05,
    Attack06,
    Attack07,
    Attack08,
    Attack09,
    __EndAttack,

    JumpAttack,
    AirShotAttack,

    //=============================================================================================
    // 여기서 부터 중간 추가 가능
    //=============================================================================================

    // Skill
    __StartSkill,
    ChargingAttack,
    ChargingAttack2,
    RushAttack,
    AttackDuringAttack,
    AirDownAttack,          // 공중에서 내려찍기
    HoldingDefBtnAttack,    // 방어(대시)버튼 누르고 있을 때 나가는 공격
    ExtreamEvade,           // 극한 회피
    CounterAttack,          // 카운터 공격
    Teleport,
    TeleportAttack,
    UpperJump,
    GroundSkillCombo,
    EmergencyAttack,
    TimingHoldAttack,
    __EndSkill,

    // USkill
    __StartUSkill,
    USkill01,
    __EndUSkill,

    // QTE
    __StartQTE,
    QTEFromEvade,
    QTEFromUpperAttack,
    QTEFromDownAttack,
    QTEFromStrongAttack,
    __EndQTE,

    // Clone Attack
    __StartCloneAttack,
    CloneHomingAttack,
    CloneAttack,
    __EndCloneAttack,

    // Enemy Action for Player Skill
    ActionForPlayerSkill,

    // Lobby
    LobbyMoveByDirection,
    LobbyDroneMoveByDirection,

    // SnakeLady
    ChangeToKaliya,

    Disappear,
    AcrossArea,

    Wait,
    BackDash,
    Threat,

    AttackPattern, // 사용안함

    __StartBossSkill,
    // Terrorist Boss
    TBossLegFollowTarget,
    // Behimos
    BehimosBreath,
    BehimosDownhill,
    // Oboro
    WheelAttack,   
    // Momochi
    Summon,
    ThrowChain,
    HideAndAttack,
	// Black
	BlackBlackhole,
	BlackBlackhole2,
	BlackFloor,
    // Shikanosuke
    ShikanosukeBlackhole,
	__EndBossSkill,

    Immune,
    Dash,

    __StartWpnSkill,
    WpnBrandish,
    WpnAsagiDemonSword,
    WpnAsagiEgergencyWhirlWind,
    WpnSakuraWhirlwind,
    WpnSakuraPhantomKnife,
    WpnYukikazeFireBall,
    WpnSnakeLadySword,
    WpnRinkoBrandish,
    WpnBlackDragonSword,
    WpnSwimSuitSword,
    WpnWhirlWind,
    WpnCasting,
    WpnExtreamEvade,
    WpnHalloween,
    WpnRushAttack,
    WpnShiranui17,
    WpnAsagiCloneChainRushAttack,
    WpnYukikazeStackParalysis,
    WpnEmily17,
    WpnSakuraSummon,
    WpnKurenai17,
    WpnFreeze,
    WpnShizuruSummon,
    WpnRin17,
    __EndWpnSkill,

    __StartSupporterSkill,
    SupporterMizukiTornado,
    SupporterArikaBlackhole,
    SupporterArikaBlackholeExplosion,
    SupporterMeizhaBatAttack,
    SupporterKayoruStunTornado,
    SupporterEmilyDrone,
    SupporterMurasakiBlackholeExplosion,
    SupporterYukikazeChainAttack,
    SupporterRinkoDraggedAttack,
    SupporterAsukaRocketAttack,
    SupporterAsukaRocketAttackOnEvade,
    SupporterJingleiPjtAttack,
    SupporterKiraraIce,
    SupporterShirayukiIce,
    SupporterShiranuiSummon,
    SupporterWinchesterSummonDrone,
    SupporterSawawashiAttack,
    SupporterAstarotSummon,
    SupporterShiranuiFloor,
    SupporterReinaAttack,
    Supporter112Attack,
    Supporter114Attack,
    Supporter119Attack,
    __EndSupporterSkill,

    LobbyTouchAction01,
    LobbyTouchAction02,
    LobbyTouchAction03,

    BrokenWings,
    StatSpeedUp,

    BrokenArm,
    ToGroggy,

    UseBattleOption,

    TeleportForward,
    TeleportBack,

    DroneFollowOwner,
    DroneLaserAttack,

    // ExtraAttack
    __StartExtraAttack,
    Attack10,
    Attack11,
    Attack12,
    Attack13,
    Attack14,
    Attack15,
    Attack16,
    Attack17,
    Attack18,
    Attack19,
    Attack20,
    __EndExtraAttack,

    MoveToOwner,

    Nothing = 9998,
    RemoveAction = 9999, // 이 밑으론 액션 추가 금지
}

public enum eActionCondition
{
    None = 0,

    Grounded,
    Jumping,

    NoUsingSkill,
    NoUsingQTE,
    NoUsingUSkill,

    All,

    UseSkill,
    UseQTE,
    UseUSkill,
}

public enum eActionLevel
{
    None = 0,

    Lv1,
    Lv2,
    Lv3
}


public abstract class ActionBase : MonoBehaviour
{
    [Header("[Execute Condition]")]
    public eActionCommand[]     conditionActionCommand;
    public eActionCondition[]   extraCondition;

    [Header("[Cancel Condition]")]
    public eActionCommand[]     cancelActionCommand;
    public eActionCondition[]   extraCancelCondition;
    public int                  cancelDuringSameActionCount;        // 같은 액션으로 캔슬할 수 있는 횟수
    public bool                 cancelDuringSameActionByCutFrame;   // 컷 프레임으로 같은 액션을 캔슬할 수 있는지 여부 (해당 액션의 컷 프레임이 0이면 의미없음)

    [Header("[Set Super Armor Level]")]
    public Unit.eSuperArmor superArmor;

    public Action					OnStartCallback						= null;
    public Action					OnEndCallback						= null;
    public bool						DontUse								= false;

    public bool						SkipShowNames						{ get; set; } = false;
    public bool						IndependentActionSystem				{ get; set; } = false;
    public bool						SkipConditionCheck					{ get; set; } = false;
    public int						TableId								{ get; protected set; } = 0;
    public eActionCommand			actionCommand						{ get; protected set; }
    public bool						isPlaying							{ get; protected set; }
    public bool						isCancel							{ get; protected set; }
    public bool						IsCommandCloneAttack2				{ get; protected set; } = false;
    public bool                     IsIgnoreAttackSkip                  { get; protected set; } = false;
    public Unit						owner								{ get { return m_owner; } }
    public float					aniLength							{ get { return m_aniLength; } }
    public eActionCommand			nextAction							{ get { return m_nextAction; } }
    public IActionBaseParam			nextParam							{ get { return m_nextParam; } }
    public bool						isJumpAttack						{ get { return m_jumpAttack; } }

    protected Unit					m_owner                             = null;
    protected CharData				m_charData                          = null;
    protected IActionBaseParam		m_param;
    protected IActionBaseParam		m_nextParam;
    protected eActionCommand		m_nextAction                        = eActionCommand.None;
    protected uint					mChangedSuperArmorId                = 0;
    protected float					m_aniLength                         = 0.0f;
    protected float					m_aniCutFrameLength                 = 0.0f;
    protected float					m_checkTime                         = 0.0f;
    protected bool					m_endUpdate                         = false;
    protected bool					m_jumpAttack                        = false;
    protected int					m_curCancelDuringSameActionCount    = 0;
	protected WaitForFixedUpdate	mWaitForFixedUpdate					= new WaitForFixedUpdate();
    protected AnimationClip			mCameraAni							= null;
    protected BuffEvent				mBuffEvt							= new BuffEvent();
    protected AttackEvent			mAtkEvt								= new AttackEvent();
    protected ActionParamAI			mParamAI							= null;

    private List<string>            mShowSkillNameStrList               = new List<string>( 3 );


    public virtual void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        m_owner = GetComponent<Unit>();
        TableId = tableId;
        isPlaying = false;
        IsCommandCloneAttack2 = false;
    }

    public virtual void InitAfterOwnerInit()
    {
    }

    public virtual void LoadAfterEnemyMgrInit()
    {
    }

    public virtual void SetBattleOptionSetParam(GameClientTable.BattleOptionSet.Param param)
    {
    }

    public void SetTableId(int tableId)
    {
        TableId = tableId;
    }

	public virtual void ShowSkillNames( params GameTable.CharacterSkillPassive.Param[] datas ) {
        if( SkipShowNames || datas == null || datas.Length <= 0 ) {
            return;
        }

        Player player = m_owner as Player;
        if( player && player.IsHelper ) {
            return;
		}

		int voiceId = 0;
        mShowSkillNameStrList.Clear();

        for( int i = 0; i < datas.Length; i++ ) {
			if( datas[i].CoolTime <= 0.0f ) {
				continue;
			}

            if( voiceId == 0 ) {
                voiceId = datas[i].VoiceID;
            }

            mShowSkillNameStrList.Add( FLocalizeString.Instance.GetText( datas[i].Name ) );
		}

		if( mShowSkillNameStrList.Count <= 0 ) {
			return;
		}

        if( voiceId > 0 ) { // 지금은 첫번째 패시브 음성만 출력. 추후에 뭘 출력해야할지 정해야함. 190712
            VoiceMgr.Instance.PlayPassiveSkill( m_owner.tableId, voiceId );
        }

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			World.Instance.UIPlay.m_screenEffect.ShowSkillName( mShowSkillNameStrList.ToArray() );
		}
		else {
			PlayerGuardian guardian = m_owner as PlayerGuardian;
            if ( guardian ) {
				player = guardian.OwnerPlayer;
			}
            World.Instance.UIPVP.ShowSkillName( player, mShowSkillNameStrList[0], UIArenaPlayPanel.eSkillNameType.Character );
		}
	}

	public virtual void OnStart(IActionBaseParam param)
    {
        isPlaying = true;
        isCancel = false;

        m_param = param;
        m_endUpdate = false;
        m_checkTime = 0.0f;

        mChangedSuperArmorId = m_owner.SetSuperArmor(superArmor);
        OnStartCallback?.Invoke();
    }

    public virtual IEnumerator UpdateAction()
    {
        yield return null;
    }

    public virtual void OnUpdating(IActionBaseParam param)
    {
    }

    public virtual void OnEnd()
    {
        isPlaying = false;
        m_curCancelDuringSameActionCount = 0;

        m_owner.ShowMesh(true);
        m_owner.RestoreSuperArmor(mChangedSuperArmorId);

        if(m_owner.curHp <= 0.0f)
        {
            ActionParamDie paramDie = null;

            ActionHit actionHit = m_owner.actionSystem.GetAction<ActionHit>(eActionCommand.Hit);
            if(actionHit && (actionHit.State == ActionHit.eState.Float || actionHit.State == ActionHit.eState.Down))
            {
                paramDie = new ActionParamDie(ActionParamDie.eState.Down, null);
            }

            SetNextAction(eActionCommand.Die, paramDie);
        }

        OnEndCallback?.Invoke();
    }

    public virtual void OnCancel()
    {
        if (isPlaying == false)
        {
            return;
        }

        isCancel = true;
        isPlaying = false;

        m_owner.ShowMesh(true);
        m_owner.RestoreSuperArmor(mChangedSuperArmorId);

        if (!m_owner.isGrounded)
        {
            m_owner.SetFallingRigidBody();
        }
        
        OnEndCallback?.Invoke();
    }

    protected virtual void LookAtTarget(Unit target)
    {
        if (target != null)
        {
            Vector3 lookAtPos = target.transform.position;

            UnitCollider nearestCollider = target.GetNearestColliderFromPos(m_owner.transform.position);
            if(nearestCollider)
            {
                lookAtPos = nearestCollider.GetCenterPos();
            }

            m_owner.LookAtTarget(lookAtPos);
        }
        else
            m_owner.LookAtTarget(transform.position + m_owner.transform.forward);
    }

    protected void LookAtTargetByCmptRotate(Unit target)
    {
        if (target != null)
        {
            Vector3 lookAtPos = target.transform.position;

            UnitCollider nearestCollider = target.GetNearestColliderFromPos(m_owner.transform.position);
            if (nearestCollider)
            {
                lookAtPos = nearestCollider.GetCenterPos();
            }

            m_owner.LookAtTargetByCmptRotate(lookAtPos);
        }
        else
            m_owner.LookAtTargetByCmptRotate(transform.position + m_owner.transform.forward);
    }

    public void SetNextAction(eActionCommand action, IActionBaseParam nextParam)
    {
        m_nextAction = action;
        m_nextParam = nextParam;
    }

    public virtual float GetAtkRange()
    {
        return 0.0f;
    }

    public virtual float GetCurAniCutFrameLength()
    {
        return 0.0f;
    }

    public void ChangeConditionActionCommand(eActionCommand curActionCommand, eActionCommand newActionCommand)
    {
        if (conditionActionCommand == null)
            return;

        for(int i = 0; i < conditionActionCommand.Length; i++)
        {
            if(conditionActionCommand[i] == curActionCommand)
            {
                conditionActionCommand[i] = newActionCommand;
                break;
            }
        }
    }

    public bool CancelActionDuringSameAction()
    {
        m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();
        if (m_aniCutFrameLength > 0.0f && cancelDuringSameActionByCutFrame)
        {
            if (m_owner.aniEvent.normalizedTime < m_aniCutFrameLength)
            {
                return false;
            }
        }

        if (m_curCancelDuringSameActionCount >= cancelDuringSameActionCount)
        {
            return false;
        }

        ++m_curCancelDuringSameActionCount;
        return true;
    }

    public void LoadCameraAni(string cameraAniName)
    {
        if(string.IsNullOrEmpty(cameraAniName))
        {
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder("Animation_Char/");
        sb.Append("Camera/");
        sb.Append(cameraAniName);
        sb.Append(".anim");

        mCameraAni = ResourceMgr.Instance.LoadFromAssetBundle("animation_char", sb.ToString()) as AnimationClip;
        mCameraAni.legacy = true;
    }

    public void ChangeCameraAni(AnimationClip clip)
    {
        mCameraAni = clip;
    }

    public virtual void RemoveEffect()
    {
    }
}