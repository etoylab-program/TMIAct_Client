
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using CodeStage.AntiCheat.ObscuredTypes;


public abstract partial class Unit : MonoBehaviour//, IObserverSubject
{
    public enum eMonType {
        None = 0,
        Human,
        Machine,
        Devil,
    }

    public enum eGrade {
        None = 0,
        Normal,
        Epic,
        Boss,
    }

    public enum eDirection {
        Forward = 0,
        Left,
        Right,
        Back,

        Forward_Left,
        Forward_Right,

        Back_Left,
        Back_Right,

        All,
    }

    public enum eSuperArmor {
        None = 0,
        Lv1,
        Lv2,
        Invincible,
    }

    public enum eKnockBackType {
        None = 0,
        Normal,
        KnockDown,
    }

    public enum ESuperArmorConditionType {
        NONE = 0,
        GRADE,
        MON_TYPE,
    }

    public enum eAxisType {
        Z = 0,
        X,
        None,
    }

    public enum eMarkingType {
        None = 0,
        ConnectingAttack,
        GiveHp,
        AccumDamageDown,
        EndOfMarkingAttack,
        EndOfMarkingRangeAttack,
    }

    public enum eAllowAction {
        None = 0,       // 모두 불가
        MoveAndDash,    // 이동, 대쉬만 가능
        Attack,         // 공격만 가능
        All,            // 모두 가능
    }

    public enum ignoreHitAniCond_t {
        NONE = 0,
        MELEE_ATTACK,
        RANGE_ATTACK,
        ALL,
    }


    public struct sMovingPositionEvent {
        public enum eStartMovingPositionState {
            None = 0,
            Start,
            End,
        }


        public eStartMovingPositionState state;
        public Vector3 dir;
        public float remainTime;
        public float dist;
        public float remainDist;
        public bool ignoreCollision;
        public bool IgnoreY;
        public float PosYOnStart;


        public void Init() {
            state = eStartMovingPositionState.None;
            dir = Vector3.zero;
            remainTime = 0.0f;
            dist = 0.0f;
            remainDist = 0.0f;
            ignoreCollision = false;
            IgnoreY = false;
            PosYOnStart = 0.0f;
        }
    }

    public struct sDirection {
        public bool used;
        public Unit usingUnit;

        public eDirection dir;
    }

    public class sConditionalSuperArmor {
        public ESuperArmorConditionType conditionType = ESuperArmorConditionType.NONE;
        public int compareValue = 0;
        public eSuperArmor superArmor = eSuperArmor.None;


        public void Set( ESuperArmorConditionType condtiionType, int compareValue, int superArmor ) {
            this.conditionType = condtiionType;
            this.compareValue = compareValue;
            this.superArmor = ( eSuperArmor )superArmor;
        }
    }

    public class sMarkingInfo {
        public eMarkingType MarkingType = eMarkingType.None;
        public float        Value1      = 0.0f;
        public float        Value2      = 0.0f;
        public float        Value3      = 0.0f;
        public float        AccumValue  = 0.0f;
        public float        DmgRatio    = 1.0f;
        public int          EffId       = 0;


        public void Init() {
            MarkingType = eMarkingType.None;
            Value1 = 0.0f;
            Value2 = 0.0f;
            Value3 = 0.0f;
            AccumValue = 0.0f;
            DmgRatio = 1.0f;
            EffId = 0;
        }
    }


    public static int DIRECTION_COUNT = 360;
    public static float FLOATING_JUMP_POWER_RATIO = 1.17f;


    //new Transform transform;

    [Header("[Property]")]
    public bool withoutAniEvent = false;
    public bool lookTargetOnHit = true;
    public bool ignoreHit = false;
    public bool alwaysKinematic = false;
    public bool IgnoreCheckRaycastCol = false;
    public float stunDuration = 5.0f;
    public float DownDuration = 3.0f;
    public Vector3 addIconPos = new Vector3(0.0f, 0.5f, 0.0f);
    public Vector3 addIconLyingPos = new Vector3(0.0f, 0.5f, 0.0f);
    public bool noBack = false; // 모모치 크리쳐 처럼 뒤쪽은 아무것도 없고 앞만 보면서 전투해야하는 놈들을 위한 플래그
    //public Vector3 colZAxisCenter = Vector3.zero;

    [Header("[Color Property]")]
    public Color rimColorOnHit = Color.white;
    public float rimMinOnHit = 0.0f;
    public float rimMaxOnHit = 1.0f;

    [Header("[Shake Property]")]
    public float shakeDuration = 0.0f;
    public float shakeSpeed = 0.3f;

    [Header("[Clone Property]")]
    public int      CloneCount          = 0;
    public Color    CloneColor          = Color.white;
    public Color    CloneHColor         = Color.white;
    public Color    CloneSColor         = Color.black;
    public Color    CloneRimColor       = new Color(1.0f, 1.0f, 1.0f, 0.6f);
    public float    CloneRimMin         = 0.636f;
    public Color    CloneOutlineColor   = Color.white;
    public float    CloneOutlineWidth   = 0.5f;

    //public List<IObserver> m_listObserver { get; set; }
    public bool IsShow { get; set; }

    protected AniEvent m_aniEvent = null;

    protected int m_tableId;
    protected string m_folder;
    protected eCharacterType m_charType;

    protected ObscuredFloat m_maxHp;
    protected ObscuredFloat m_curHp;
    protected ObscuredFloat m_curSp;
    protected ObscuredFloat m_curShield;
    protected ObscuredFloat m_maxShield;
    protected ObscuredFloat m_originalSpeed;
    protected ObscuredFloat m_originalSpeedRate = 0.0f;
    protected ObscuredFloat m_backwardSpeed;
    protected ObscuredFloat m_curSpeed;
    protected ObscuredFloat m_speedRateByBuff   = 0.0f;
    protected ObscuredFloat m_speedRateBySprint = 0.0f;
    protected ObscuredFloat m_attackPower;
    protected ObscuredFloat m_defenceRate;
    protected ObscuredFloat m_criticalRate;
    protected ObscuredFloat mAddCriticalDmgRate = 0.0f;
    protected ObscuredFloat mCriticalResist     = 0.0f;
    protected ObscuredFloat mCriticalDmgDef     = 0.0f;
    protected ObscuredFloat m_fixCriticalRate   = 1.0f;
    protected ObscuredFloat mAddMaxHpRate       = 0.0f;
    protected float m_scale;

    protected eMonType m_monType = eMonType.None;
    protected eGrade m_grade = eGrade.None;
    protected Unit2ndStatsTable m_2ndStats = new Unit2ndStatsTable();
    protected UnitBuffStats m_buffStats = null;

    protected WaitForFixedUpdate mWaitForFixedUpdate = new WaitForFixedUpdate();

    protected ActionSystem m_actionSystem = null;
    protected ActionSystem m_actionSystem2 = null;
    protected List<CmptBase> m_listCmpt = new List<CmptBase>( 4 );
    protected CmptMovement m_cmptMovement = null;
    protected CmptRotateByDirection m_cmptRotate = null;
    protected CmptJump m_cmptJump = null;
    protected CmptBuffDebuff m_cmptBuffDebuff = null;

    //protected IUnitController   m_controller    = null;

    protected InputController   mInput          = null;
    protected BTBase            mAI             = null;
    protected BTBase            mAIBackup       = null;

    protected Rigidbody m_rigidBody = null;
    protected RigidbodyConstraints mDefaultRigidbodyConstraints = /*RigidbodyConstraints.FreezePositionY |*/ RigidbodyConstraints.FreezeRotation;
    protected float m_originalMass = 1.0f;
    protected float m_massOnFloating = 1.0f;
    protected Vector3 m_contactNormal;

    protected sDirection[] m_directions = new sDirection[8];

    //protected float m_originalCapsuleColHeight;
    //protected float m_originalCapsuleColRadius;
    //protected Vector3 m_originalCapsuleColCenter;

    //protected Light m_light;

    /*protected ShadowTextureRenderer m_shadowTexRenderer = null;
    protected Projector m_shadowProjector = null;
    protected DrawTargetObject m_dawTargetObject = null;*/
    protected AfterImageComponent m_afterImg = null;
    protected CostumeUnit m_costumeUnit = null;

    protected Unit m_parent = null;
    protected Unit m_child = null;

    //protected eSuperArmor m_defaultSuperArmor = eSuperArmor.None;
    protected bool m_groggy = false;

    //protected Vector3 m_inputDir;
    //protected Vector3 m_dir;

    protected Vector3 m_posOnGround;
    protected Vector3 m_originalPos = Vector3.zero;
    protected Vector3 m_prevPos = Vector3.zero;
    protected sMovingPositionEvent m_stepForwardEvt;
    //protected sMovingPositionEvent m_pushedEvt;
    protected float m_stepForwardDistRatio = 1.0f;
    protected float m_stepForwardSpeedRatio = 1.0f;

    protected Dictionary<string, Director> m_dicDirector = new Dictionary<string, Director>();

    //protected AttackEvent m_atkEvent = new AttackEvent();
    //protected BuffEvent m_buffEvent = new BuffEvent();

    protected ObscuredInt   m_combo;
    protected bool m_checkRayCastCol = false;

    protected Unit m_mainTarget = null;
    protected Unit m_lastMainTarget = null;
    protected Unit m_evadedTarget = null;
    protected List<Unit> m_listHitTarget = new List<Unit>();
    protected List<eAnimation> m_listAniHit = new List<eAnimation>();
    protected ParticleSystem m_psHit = null;

    protected AniEvent.sEvent m_onAttackAniEvent = null;
    protected eAttackDirection m_atkDir = eAttackDirection.Front;

    protected Unit              m_attacker          = null;
    protected AniEvent.sEvent   m_attackerAniEvent  = null;

    protected float m_curAniLength = 0.0f;
    protected float m_curPauseFrame = 0.0f;
    protected bool m_pauseFrame = false;
    protected float m_pauseFrameRate = 0.0f;

    protected List<EAttackAttr> mListCurrentAbnormalAttr = new List<EAttackAttr>();

    //protected bool      m_lockZAxis = false;
    protected bool m_lockDie = false;
    protected bool m_pause = false;
    protected bool mLockShowMesh = false;

    protected Coroutine m_crStepForward = null;

    protected Coroutine m_crRestoreShield = null;
    protected ParticleSystem m_psShieldOnHit = null;
    protected ParticleSystem m_psShieldBreak = null;
    protected ParticleSystem m_psShieldAttack = null;

    //protected List<ParticleSystem> m_listPSBind = new List<ParticleSystem>();

    //protected UIDamageText m_uiDamageText;
    //protected UIRecoveryText m_uiRecoveryText;

    //protected List<UIDamageText> mListUIDamageText = new List<UIDamageText>(); 220203
    //protected List<UIRecoveryText> mListUIRecoveryText = new List<UIRecoveryText>();

    protected UIHpBar mUIHpBar;
    protected UI3DBuffDebuffIcon mUIBuffDebuffIcon;
    protected UIBoss mUIBoss;

    protected Dictionary<string, AudioClip> m_dicFxSnd = new Dictionary<string, AudioClip>();
    //protected Obstacle m_curObstacle = null;

    //protected List<DropItem> m_listDropItem = new List<DropItem>();

    protected Coroutine m_crAddHp = null;
    protected Coroutine m_crSubHp = null;

    protected List<UnitCollider> mListTempUnitCollider = new List<UnitCollider>();
    protected List<Unit> mListTempUnit = new List<Unit>();

    protected bool m_noAttack = true;

    protected uint mChangedSuperArmorId = 0;
    protected bool mLockAddDropItemRate = false;

    protected Material mMtrlDissolve = null;

    protected List<PlayerMinion> mListMinion = new List<PlayerMinion>();

    //protected Type mChangeSuperArmorType        = null;
    //protected Type mBeforeChangeSuperArmorType  = null;

    // Raycast Collision
    private float m_minExtent;
    private float m_partialExtent;
    private float m_sqrMinExtent;
    private float m_skinWidth = 0.1f;

    private uint mCreatedSuperArmorId = 0;

    private Coroutine mCrUpdateFirePjt = null;

    // Get
    public AniEvent aniEvent { get { return m_aniEvent; } }

    public int tableId { get { return m_tableId; } }
    public string folder { get { return m_folder; } }

    public ObscuredFloat maxHp              { get { return m_maxHp; } }
    public ObscuredFloat curHp              { get { return m_curHp; } }
    public ObscuredFloat maxShield          { get { return m_maxShield; } }
    public ObscuredFloat curShield          { get { return m_curShield; } }
    public ObscuredFloat curSp              { get { return m_curSp; } }
    public ObscuredFloat originalSpeed      { get { return m_originalSpeed; } }
    public ObscuredFloat backwardSpeed      { get { return m_backwardSpeed; } }
    public ObscuredFloat speed              { get { return m_curSpeed; } }
    public ObscuredFloat SpeedRateByBuff    { get { return m_speedRateByBuff; } }
    public ObscuredFloat SpeedRateBySprint  { get { return m_speedRateBySprint; } }
    public ObscuredFloat attackPower        { get { return m_attackPower; } }
    public ObscuredFloat defenceRate        { get { return m_defenceRate; } }
    public ObscuredFloat criticalRate       { get { return m_criticalRate; } }
    public ObscuredFloat fixCriticalRate    { get { return m_fixCriticalRate; } }
    public ObscuredFloat Penetrate          { get; protected set; } = 0.0f; // 관통
    public ObscuredFloat OriginalPenetrate  { get; protected set; } = 0.0f;
    public ObscuredFloat Sufferance         { get; protected set; } = 0.0f; // 인내
    public ObscuredFloat OriginalSufferance { get; protected set; } = 0.0f;

    public float scale { get { return m_scale; } }

    public eMonType monType { get { return m_monType; } }
    public eGrade grade { get { return m_grade; } }
    public Unit2ndStatsTable unit2ndStats { get { return m_2ndStats; } }
    public UnitBuffStats unitBuffStats { get { return m_buffStats; } }

    public ActionSystem actionSystem { get { return m_actionSystem; } }
    public ActionSystem actionSystem2 { get { return m_actionSystem2; } }
    public ActionSystem ActionSystem3 { get; protected set; } = null;
    public eActionCommand IgnoreAction { get; protected set; } = eActionCommand.None;
    //public IUnitController controller { get { return m_controller; } }

    public bool IsHit { get; protected set; } = false;

    //public AniEvent.sEvent OnAttackAniEvent { get { return m_onAttackAniEvent; } }

    public Rigidbody rigidBody { get { return m_rigidBody; } }
    public float originalMass { get { return m_originalMass; } }

    public float Height { get; protected set; }

    public int AggroValue { get; protected set; } = 0;

    public InputController Input { get { return mInput; } }
    public BTBase AI { get { return mAI; } }

    public UnitCollider MainCollider { get; protected set; }
    public List<UnitCollider> ListCollider { get; protected set; } = new List<UnitCollider>();

    //public float originalCapsuleColHeight { get { return m_originalCapsuleColHeight; } }
    //public float originalCapsuleColRadius { get { return m_originalCapsuleColRadius; } }
    //public Vector3 originalCapsuleColCenter { get { return m_originalCapsuleColCenter; } }
    //public Vector3 capsuleColCenter { get { return capsuleCol == null ? Vector3.zero : new Vector3(capsuleCol.center.x, 0.0f, capsuleCol.center.z); } }

    public CmptMovement cmptMovement { get { return m_cmptMovement; } }
    public CmptRotateByDirection cmptRotate { get { return m_cmptRotate; } }
    public CmptJump cmptJump { get { return m_cmptJump; } }
    public CmptBuffDebuff cmptBuffDebuff { get { return m_cmptBuffDebuff; } }
    //public Light lightPrivate { get { return m_light; } }

    public AfterImageComponent afterImg { get { return m_afterImg; } }
    public CostumeUnit costumeUnit { get { return m_costumeUnit; } }

    //public eSuperArmor defaultSuperArmor { get { return m_defaultSuperArmor; } }

    public Unit parent { get { return m_parent; } }
    public Unit child { get { return m_child; } }

    public bool isGroggy { get { return m_groggy; } }
    //public bool isLockZAxis { get { return m_lockZAxis; } }

    //public Vector3 inputDir { get { return m_inputDir; } }
    //public Vector3 dir { get { return m_dir; } }
    public sDirection[] directions { get { return m_directions; } }

    public Vector3 posOnGround { get { return m_posOnGround; } set { m_posOnGround = value; } }
    //public sMovingPositionEvent pushedEvt { get { return m_pushedEvt; } }

    public int comboCount { get { return m_combo; } }

    public Unit mainTarget { get { return m_mainTarget; } }
    public Unit lastHitTarget { get { return m_lastMainTarget; } }
    public Unit evadedTarget { get { return m_evadedTarget; } }
    public List<Unit> listHitTarget { get { return m_listHitTarget; } }
    public sMarkingInfo MarkingInfo { get; protected set; } = new sMarkingInfo();

    public AniEvent.sEvent onAtkAniEvt { get { return m_onAttackAniEvent; } }
    public eAttackDirection currentAtkDir { get { return m_atkDir; } }

    public Unit attacker { get { return m_attacker; } }
    public AniEvent.sEvent attackerAniEvt { get { return m_attackerAniEvent; } }
    public ActionBase AttackerActionOnHit { get; protected set; }

    public float curAniLength { get { return m_curAniLength; } }
    public bool isPause { get { return m_pause; } }
    public bool isPauseFrame { get { return m_pauseFrame; } }

    public bool isLockDie { get { return m_lockDie; } } // 일반 Hit에서 죽는 상태로 빠지는걸 막음

    public UIHpBar hpBar { get { return mUIHpBar; } }
    public UI3DBuffDebuffIcon uiBuffDebuffIcon { get { return mUIBuffDebuffIcon; } }

    public bool isNoAttack { get { return m_noAttack; } }

    public eDebuffImmuneType_f DebuffImmuneFlag { get; protected set; } = eDebuffImmuneType_f.NONE;

    //public Obstacle curObstacle { get { return m_curObstacle; } }

    public float deltaTime {
        get {
            float f = (Director.CurrentPlaying && Director.CurrentPlaying.isPause) ? 0.0f : 1.0f;
            if ( !withoutAniEvent )
                return Time.deltaTime * aniEvent.aniSpeed * f;
            else
                return Time.deltaTime * f;
        }
    }

    public float fixedDeltaTime {
        get {
            float f = (Director.CurrentPlaying && Director.CurrentPlaying.isPause) ? 0.0f : 1.0f;
            if ( !withoutAniEvent )
                return Time.fixedDeltaTime * aniEvent.aniSpeed * f;
            else
                return Time.fixedDeltaTime * f;
        }
    }

    // Property
    public string tableName { get; protected set; } = null;
    public string IconName { get; protected set; } = null;

    public bool isGrounded { get; set; }
    public bool isFalling { get; set; }
    //public bool isHit { get; set; } = false;
    public bool isDying { get; set; } = false;
    //public bool isEvading { get; set; }
    public bool isFloating { get; protected set; }
    public bool isVisible { get; protected set; }
    public bool decideCritical { get; set; }

    public bool IsShowMesh { get; protected set; } = true;
    public eAxisType LockAxis { get; protected set; } = eAxisType.None;

    public float IncreaseSkillAtkValue { get; protected set; } = 0.0f;
    public float IncreaseSummonsAttackPowerRate { get; protected set; } = 0.0f; // 소환수들(분신, 미니언, 드론 등) 공격력 증가 비율

    public float sprintStartTimeRate { get; protected set; }
    public bool onlyLastAttack { get; protected set; }
    public bool lastAttackKnockBack { get; protected set; }

    public bool     skipAttack              { get; set; }
    public float    lastAttackPower         { get; set; }
    public float    LastNormalAttackPower   { get; set; }
    public float    LastDamage              { get; private set; }

    public eAllowAction AllowAction = eAllowAction.All;

    public Projectile LastProjectile { get; set; } = null;

    public List<EAttackAttr> ListImmuneAtkAttr { get; protected set; } = new List<EAttackAttr>();
    public List<eEventType> ListImmuneDebuffType { get; protected set; } = new List<eEventType>();

    public eHitState curHitState { get; set; }
    public bool curSuccessAttack { get; set; }
    public bool reserveStun { get; set; }
    public int holdPositionRef { get; set; } = 0;
    public bool checkRayCollision { get; set; }
    public float floatingJumpPowerRatio { get; set; }
    public Unit beforeChangeUnit { get; set; }
    public bool HoldSupporterSkillCoolTime { get; set; } = false;
    public Projectile StayOnProjectile { get; set; } = null;
    public BoxCollider StayOnBoxCollider { get; set; } = null;

    public bool                 TemporaryInvincible     { get; set; }           = false;                    // 극한회피 등의 액션에서 임시적으로 사용할 무적 판정
    public ignoreHitAniCond_t   TemporaryIgnoreHitAni   { get; set; }           = ignoreHitAniCond_t.NONE;
    public bool                 HoldDying               { get; set; }           = false;                    // 특정 스킬로 인해 죽음을 잠시 홀딩
    public float                TemporarySpeedRate      { get; protected set; } = 0.0f;

    public sConditionalSuperArmor ConditionalSuperArmor { get; protected set; } = new sConditionalSuperArmor();
    public eSuperArmor CurrentSuperArmor { get; private set; } = eSuperArmor.None;
    public eSuperArmor BeforeSuperArmor { get; private set; } = eSuperArmor.None;

    public eCharacterType charType { get { return m_charType; } }

    //public Vector3 unitForward { get; protected set; }

    public float compareScore { get; set; }
    public float compareDist { get; set; }
    public float compareAngle { get; set; }

    public float MaxAttackPower     { get; set; }           = 0.0f;
    public float DamagePerSecond    { get; private set; }   = 0.0f;
    public float MaxDamagePerSecond { get; private set; }   = 0.0f;
    public float DpsAverage         { get; private set; }   = 0.0f;
    public float Damaged            { get; private set; }   = 0.0f;
    public float Healed             { get; private set; }   = 0.0f;
    public float HealedWithOverHeal { get; private set; }   = 0.0f;

    public bool InverseXAxis { get; set; } = false;
    public bool InverseYAxis { get; set; } = false;

    private Coroutine   mCrCheckDPS = null;
    private float       mDpsAccum   = 0.0f;

    // Callback
    public Action OnAttackOnce = null;
    public Action OnAttackAlways = null;

    // Pure virtual function
    public abstract void SetData( int tableId );


    protected virtual void Awake() {
        m_costumeUnit = GetComponent<CostumeUnit>();
    }

    public virtual void Init( int tableId, eCharacterType type, string faceAniControllerPath ) {
        SetRandomizeCryptoKey();

        //transform = GetComponent<Transform>();
        beforeChangeUnit = null;

        //AddObserver(World.Instance);
        //AddObserver(World.Instance.qteMgr);

        bool isLobbyPlayer = ((this as LobbyPlayer) != null);

        if ( !isLobbyPlayer ) {
            m_cmptBuffDebuff = gameObject.AddComponent<CmptBuffDebuff>();
            m_cmptBuffDebuff.Unused = false;
        }

        m_buffStats = new UnitBuffStats( this );

        m_listCmpt.Clear();
        m_listCmpt.AddRange( GetComponentsInChildren<CmptBase>() );

        m_cmptMovement = GetComponent<CmptMovement>();
        m_cmptRotate = GetComponent<CmptRotateByDirection>();
        m_cmptJump = GetComponent<CmptJump>();

        m_actionSystem = gameObject.AddComponent<ActionSystem>();
        m_actionSystem.Init( this );
        m_actionSystem.AddAction( GetComponentsInChildren<ActionBase>() );

        if ( !isLobbyPlayer && m_actionSystem && !m_actionSystem.HasNoAction() ) {
            m_actionSystem.AddAction( gameObject.AddComponent<ActionHit>(), 0, null );
            m_actionSystem.AddAction( gameObject.AddComponent<ActionImmune>(), 0, null );
        }

        mListMinion.Clear();

        m_2ndStats.Clear();
        SetData( tableId );

        mCreatedSuperArmorId = 0;
        mChangedSuperArmorId = 0;

        if ( m_curShield > 0.0f ) {
            mChangedSuperArmorId = SetSuperArmor( eSuperArmor.Lv2 );
        }
        else {
            mChangedSuperArmorId = SetSuperArmor( eSuperArmor.None );
        }

        m_crRestoreShield = null;

        m_curSp = GameInfo.Instance.BattleConfig.USInitSP;

		m_rigidBody = GetComponent<Rigidbody>();
		if ( m_rigidBody ) {
			if ( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
				m_rigidBody.interpolation = RigidbodyInterpolation.Extrapolate;
			}
			else {
				m_rigidBody.interpolation = RigidbodyInterpolation.None;
			}

			mDefaultRigidbodyConstraints = RigidbodyConstraints.FreezeRotation;
			m_rigidBody.constraints = mDefaultRigidbodyConstraints;
		}

		MainCollider = GetComponent<UnitCollider>();
        if ( MainCollider == null ) {
            MainCollider = gameObject.AddComponent<UnitCollider>();
            MainCollider.Init();
        }

        UnitCollider[] arrUnitCollider = GetComponentsInChildren<UnitCollider>();
        for ( int i = 0; i < arrUnitCollider.Length; i++ ) {
            if ( arrUnitCollider[i] == MainCollider )
                continue;

            ListCollider.Add( arrUnitCollider[i] );
        }

        if ( m_rigidBody && MainCollider ) {
            m_rigidBody.centerOfMass = -Vector3.up * ( MainCollider.height * 0.5f );
        }

        m_afterImg = GetComponent<AfterImageComponent>();
        if ( m_afterImg != null ) {
            m_afterImg.SetParent( this );
        }

        if ( m_costumeUnit == null ) {
            m_costumeUnit = GetComponent<CostumeUnit>();
        }

        if ( !withoutAniEvent ) {
            m_aniEvent = GetComponentInChildren<AniEvent>();
            if ( m_aniEvent != null )
                m_aniEvent.Init( this, m_folder, faceAniControllerPath );
        }

        m_contactNormal = Vector3.zero;

        m_originalPos = transform.localPosition;
        m_scale = 1.0f;

        mAddMaxHpRate = 0.0f;

        if ( withoutAniEvent == false ) {
            string[] fileName = Utility.Split(name, ' '); //name.Split(' ');
            fileName[0] = fileName[0].Replace( "(Clone)", "" );
            fileName[0] = fileName[0].Replace( "_aos", "" );

            TextAsset aniEventFile = ResourceMgr.Instance.LoadFromAssetBundle("unit", string.Format("Unit/{0}/{1}/{2}.bytes", m_folder, fileName[0], fileName[0])) as TextAsset;
            TextAsset aniSndEventFile = ResourceMgr.Instance.LoadFromAssetBundle("unit", string.Format("Unit/{0}/{1}/{2}_snd.bytes", m_folder, fileName[0], fileName[0])) as TextAsset;

            if ( m_aniEvent == null ) {
                Debug.LogError( fileName[0] + "(m_aniEvent == null)" );
            }

            m_aniEvent.GetBones();
            m_aniEvent.LoadEvent( aniEventFile, aniSndEventFile );

            m_aniEvent.OnAttack = OnAttack;
            m_aniEvent.OnFire = OnFire;
            m_aniEvent.OnStepForward = OnStepForward;
            //m_aniEvent.OnPushed = OnPushed;
            m_aniEvent.OnChangeColor = OnChangeColor;
            m_aniEvent.OnJump = OnJump;
            m_aniEvent.OnUseCameraSetting += OnUseCameraSetting;
            m_aniEvent.OnEndCameraSetting += OnEndCameraSetting;
            m_aniEvent.OnCameraShake += OnCameraShake;
        }

        holdPositionRef = 0;

        m_attacker = null;
        AttackerActionOnHit = null;
        m_mainTarget = null;
        m_evadedTarget = null;

        isFloating = false;

        m_pause = false;
        m_pauseFrame = false;

        m_charType = type;
        //LoadExpression();
        LoadShieldEffects();

        if ( World.Instance.UIPlay != null ) {
            LoadHUDUI();
        }

        isDying = false;
        isVisible = false;

        m_checkRayCastCol = true;
        skipAttack = false;

        sprintStartTimeRate = 1.0f;
        onlyLastAttack = false;
        lastAttackKnockBack = false;
        decideCritical = false;
        floatingJumpPowerRatio = FLOATING_JUMP_POWER_RATIO;

        SetShadow();

        // 각종 면역 들
        ListImmuneAtkAttr.Clear();
        ListImmuneDebuffType.Clear();

        if ( MainCollider ) {
            PhysicMaterial pmWall = ResourceMgr.Instance.LoadFromAssetBundle("etc", "Etc/PhysicMaterial/PM_Wall.physicMaterial") as PhysicMaterial;
            MainCollider.HitCollider.material = pmWall;

            if ( ListCollider != null ) {
                for ( int i = 0; i < ListCollider.Count; i++ ) {
                    ListCollider[i].HitCollider.material = pmWall;
                }
            }
        }

        Height = GetHeight();
    }

    private void SetRandomizeCryptoKey() {
        m_maxHp.RandomizeCryptoKey();
        m_curHp.RandomizeCryptoKey();
        m_curSp.RandomizeCryptoKey();
        m_curShield.RandomizeCryptoKey();
        m_maxShield.RandomizeCryptoKey();
        m_originalSpeed.RandomizeCryptoKey();
        m_originalSpeedRate.RandomizeCryptoKey();
        m_backwardSpeed.RandomizeCryptoKey();
        m_curSpeed.RandomizeCryptoKey();
        m_speedRateByBuff.RandomizeCryptoKey();
        m_speedRateBySprint.RandomizeCryptoKey();
        m_attackPower.RandomizeCryptoKey();
        m_defenceRate.RandomizeCryptoKey();
        m_criticalRate.RandomizeCryptoKey();
        m_fixCriticalRate.RandomizeCryptoKey();
        mAddMaxHpRate.RandomizeCryptoKey();

        m_combo.RandomizeCryptoKey();
         
        maxHp.RandomizeCryptoKey();
        curHp.RandomizeCryptoKey();
        maxShield.RandomizeCryptoKey();
        curShield.RandomizeCryptoKey();
        curSp.RandomizeCryptoKey();
        originalSpeed.RandomizeCryptoKey();
        backwardSpeed.RandomizeCryptoKey();
        speed.RandomizeCryptoKey();
        SpeedRateByBuff.RandomizeCryptoKey();
        SpeedRateBySprint.RandomizeCryptoKey();
        attackPower.RandomizeCryptoKey();
        defenceRate.RandomizeCryptoKey();
        criticalRate.RandomizeCryptoKey();
        fixCriticalRate.RandomizeCryptoKey();
    }

    public virtual void GetBones() {
        if ( m_aniEvent == null ) {
            return;
        }

        m_aniEvent.GetBones();

        if ( m_costumeUnit && m_costumeUnit.CostumeBody && m_costumeUnit.CostumeBody.TexPartsColor == null ) {
            m_aniEvent.SetMaskTexture( null );
        }
    }

    public void AddInputController() {
        if ( mInput == null ) {
            mInput = gameObject.AddComponent<InputController>();
        }

        mInput.Init( DIRECTION_COUNT, World.Instance.UIPlay );
    }

    public void ChangeInputController( Player beforePlayer ) {
        if ( beforePlayer ) {
            DestroyImmediate( beforePlayer.Input );
            mInput = null;
        }

        AddInputController();
    }

    public void AddAIController( string aiFileName ) {
        if( mAIBackup ) {
            mAI = mAIBackup;
		}

        if ( string.IsNullOrEmpty( aiFileName ) ) {
            Debug.Log( tableName + " 얘는 AI가 없는 앤가?" );
            return;
        }

        if ( mAI ) {
            return;
        }

        mAI = gameObject.AddComponent<BTBase>();
        if ( mAI == null ) {
            Debug.LogError( "Behaviour Tree를 가져올 수 없습니다." );
            return;
        }

        mAI.LoadBT( aiFileName );
        mAIBackup = mAI;
    }

    public void RemoveAIController() {
        mAI = null;
	}

    public void AddCloneDefaultAction() {
        aniEvent.RestoreOriginalColor();

        m_rigidBody = gameObject.AddComponent<Rigidbody>();

        m_cmptJump = gameObject.AddComponent<CmptJump>();
        m_cmptJump.m_jumpPower = m_cloneOwner.cmptJump.m_jumpPower;

        m_cmptMovement = gameObject.AddComponent<CmptMovement>();
        m_cmptRotate = gameObject.AddComponent<CmptRotateByDirection>();

        MainCollider = gameObject.AddComponent<UnitCollider>();

        if ( m_cloneOwner.MainCollider ) {
            MainCollider.SetRadius( m_cloneOwner.MainCollider.radius );
            MainCollider.SetHeight( m_cloneOwner.MainCollider.height );
        }

        actionSystem.AddAction( gameObject.AddComponent<ActionMoveToTarget>(), 0, null );
        actionSystem.AddAction( gameObject.AddComponent<ActionWait>(), 0, null );

        actionSystem.AddAction( gameObject.AddComponent<ActionIdle>(), 0, null );
    }

    public void AddAI( string aiFileName ) {
        AddCloneDefaultAction();
        AddAIController( aiFileName );
    }

    protected virtual void LoadShieldEffects() {
    }

    protected virtual void LoadHUDUI() {
        /* 220203
		for (int i = 0; i < 5; i++)
        {
            UIDamageText uiDmgText = ResourceMgr.Instance.CreateFromAssetBundle<UIDamageText>("ui", "UI/DamageText.prefab");
            uiDmgText.name = "DamageText_" + name + "_" + i.ToString();
            uiDmgText.transform.SetParent(World.Instance.StageType != eSTAGETYPE.STAGE_PVP ? World.Instance.UIPlay.transform : World.Instance.UIPVP.transform);
            Utility.InitTransform(uiDmgText.gameObject);

            mListUIDamageText.Add(uiDmgText);
			
            UIRecoveryText uiRecoveryText = ResourceMgr.Instance.CreateFromAssetBundle<UIRecoveryText>("ui", "UI/RecoveryText.prefab");
            uiRecoveryText.name = "RecoveryText_" + name + "_" + i.ToString();
            uiRecoveryText.transform.SetParent(World.Instance.StageType != eSTAGETYPE.STAGE_PVP ? World.Instance.UIPlay.transform : World.Instance.UIPVP.transform);
            Utility.InitTransform(uiRecoveryText.gameObject);

            mListUIRecoveryText.Add(uiRecoveryText);
        }
		*/

        if ( m_grade != eGrade.Boss ) {
            mUIBuffDebuffIcon = ResourceMgr.Instance.CreateFromAssetBundle<UI3DBuffDebuffIcon>( "ui", "UI/BuffDebuff.prefab" );
            if ( mUIBuffDebuffIcon != null ) {
                mUIBuffDebuffIcon.name = "BuffDebuffIcon_" + name;
                mUIBuffDebuffIcon.transform.SetParent( transform );
                Utility.InitTransform( mUIBuffDebuffIcon.gameObject );

                mUIBuffDebuffIcon.Init( this );
            }
        }
    }

    protected eAnimation GetHitValue( string str ) {
        if ( World.Instance.ListAniString.Count <= 0 )
            return eAnimation.None;

        for ( int i = 1; i < ( int )eAnimation.Max; i++ ) {
            if ( World.Instance.ListAniString[i] == str )
                return ( eAnimation )i;
        }

        return eAnimation.None;
    }

    protected virtual float GetMaxHp() {
        return 0.0f;
    }

    public virtual float GetUltimateSkillDefaultAtkPower() {
        return m_attackPower;
    }

    // 게임 시작 전에 World 및 TestScene에서 한번 호출해줄 함수
    public virtual void OnGameStart() {
    }

    // 게임 시작 후 미션 스타트 UI 뜬 후에 호출
    public virtual void OnMissionStart() {
    }

    // 게임 종료 (World에 EndGame호출 시)
    public virtual void OnGameEnd() {
    }

    public virtual void Activate() {
        IsShow = true;
        Reborn();

        SetGroundedRigidBody();
        RestoreCollision();
        ShowShadow( true );

        if ( m_aniEvent )
            m_aniEvent.SetOriginalShaderColor();

        gameObject.SetActive( true );

        if ( MainCollider != null ) {
            m_minExtent = Mathf.Min( Mathf.Min( MainCollider.bounds.extents.x, MainCollider.bounds.extents.y ), MainCollider.bounds.extents.z );
            m_partialExtent = m_minExtent * ( 1.0f - m_skinWidth );
            m_sqrMinExtent = m_minExtent * m_minExtent;
        }

        for ( int i = 0; i < ( int )eDirection.All; i++ ) {
            m_directions[i].used = false;
            m_directions[i].dir = ( eDirection )i;
        }

        m_prevPos = transform.position;

        if ( m_child )
            m_child.Activate();

        //if (alwaysKinematic)
        m_posOnGround = transform.position;

        if ( mUIHpBar ) {
            mUIHpBar.Init( this );
            mUIHpBar.gameObject.SetActive( false );
        }

        if ( m_grade != eGrade.Boss && mUIBuffDebuffIcon )
            mUIBuffDebuffIcon.Init( this );

        Utility.StopCoroutine( this, ref m_crAddHp );
        Utility.StopCoroutine( this, ref m_crSubHp );

        if ( isClone ) {
            World.Instance.AddPlayerSummons( this );
        }

        DamagePerSecond = 0.0f;
        MaxDamagePerSecond = 0.0f;
        mDpsAccum = 0.0f;
        DpsAverage = 0.0f;
        Damaged = 0.0f;
        Healed = 0.0f;
        HealedWithOverHeal = 0.0f;

        Utility.StopCoroutine( this, ref mCrCheckDPS );
    }

    public virtual void Deactivate() {
        IsShow = false;
        IsUsing = false;

        if ( !isClone && m_actionSystem != null && m_actionSystem.HasNoAction() == false )
            m_actionSystem.CancelCurrentAction();

        if ( m_aniEvent != null )
            m_aniEvent.Clear( true );

        if ( mUIHpBar )
            mUIHpBar.gameObject.SetActive( false );

        if ( m_grade != eGrade.Boss && mUIBuffDebuffIcon )
            mUIBuffDebuffIcon.End();

        /*for(int i = 0; i < m_listPSBind.Count; i++)
            m_listPSBind[i].gameObject.SetActive(false);*/

        if ( isClone ) {
            World.Instance.RemovePlayerSummons( this );
        }

        StopAllCoroutines();
        gameObject.SetActive( false );
    }

    public void SetInitialPosition( Vector3 pos, Quaternion rot, float Scale = 1.0f ) {
        transform.SetPositionAndRotation( pos, rot );
        transform.localScale = Vector3.one * Scale;

        if ( m_cmptRotate != null )
            m_cmptRotate.Init( transform.localEulerAngles );

        m_prevPos = pos;
    }

    public void SetInitialPositionWithoutScale( Vector3 pos, Quaternion rot ) {
        transform.SetPositionAndRotation( pos, rot );

        if ( m_cmptRotate != null )
            m_cmptRotate.Init( transform.localEulerAngles );

        m_prevPos = pos;
    }

    public void SetInitialRigidBodyPos( Vector3 pos, Quaternion rot ) {
        if ( m_rigidBody == null )
            return;

        m_rigidBody.MovePosition( pos );
        m_rigidBody.MoveRotation( rot );

        if ( m_cmptRotate != null )
            m_cmptRotate.Init( transform.eulerAngles );

        SetInitialPosition( pos, rot );
        m_prevPos = pos;
    }

    protected float GetHeight() {
        if ( m_aniEvent ) {
            Transform head = m_aniEvent.GetBoneByName("Bip001 Head");
            if ( head == null && MainCollider == null ) {
                return 0.0f;
            }
            else if ( head == null && MainCollider ) {
                return MainCollider.height;
            }
            else {
                return ( head.position - transform.position ).y;
            }
        }
        else {
            if ( MainCollider == null ) {
                return 0.0f;
            }
            else {
                return MainCollider.height;
            }
        }
    }

    public Vector3 GetCenterPos() {
        if ( MainCollider != null ) {
            /*
            if (MainCollider.direction == (int)eAxis.Y)
            {
                y += (MainCollider.height * 0.5f);
            }
            else
            {
                y += (MainCollider.radius * 0.5f);
            }
            */

            return MainCollider.GetCenterPos();
        }

        return new Vector3( transform.position.x, Height * 0.5f, transform.position.z );
    }

    public virtual Vector3 GetHeadPos( float heightRatio = 0.9f ) {
        float y = transform.position.y;
        if ( MainCollider != null )
            y += ( MainCollider.height * heightRatio );

        return new Vector3( transform.position.x, y, transform.position.z );
    }

    public bool IsActivate() {
        if( gameObject == null ) {
            return false;
		}

        return gameObject.activeSelf && IsShow;
    }

    protected virtual void Reborn() {
        m_attacker = null;
        AttackerActionOnHit = null;
        m_mainTarget = null;
        m_evadedTarget = null;

        isFloating = false;
        isDying = false;
        IsHit = false;

        mLockAddDropItemRate = false;
        
        MarkingInfo.Init();

        if ( m_rigidBody != null )
            m_rigidBody.velocity = Vector3.zero;

        if ( m_actionSystem != null )
            m_actionSystem.CancelCurrentAction();

        SetSpeedRate( 0.0f );
        StopPauseFrame();

        m_stepForwardEvt.Init();

        reserveStun = false;
        checkRayCollision = true;

        TemporaryInvincible = false;
        mCreatedSuperArmorId = 0;

        if ( MainCollider && MainCollider.IsEnable() )
            SetGroundedRigidBody();

        mListCurrentAbnormalAttr.Clear();

        StayOnProjectile = null;
        StayOnBoxCollider = null;

        InverseXAxis = false;
        InverseYAxis = false;

        m_listHitTarget.Clear();
    }

    public bool HasHitAni() {
        if ( m_listAniHit.Count > 0 )
            return false;

        if ( m_aniEvent.HasAni( eAnimation.Hit ) )
            return false;

        return true;
    }

    protected virtual void AddDirector( string key, Director director ) {
        if ( director == null )
            return;

        director.Init( this );
        m_dicDirector.Add( key, director );
    }

    public bool HasDirector( string key ) {
        return m_dicDirector.ContainsKey( key );
    }

    public Director GetDirector( string key ) {
        if ( m_dicDirector.ContainsKey( key ) == false )
            return null;

        return m_dicDirector[key];
    }

    public float GetDirectorDuration( string key ) {
        Director drt = GetDirector(key);
        if ( drt == null )
            return 0.0f;

        return drt.GetDuration();
    }

    public virtual bool PlayDirector( string key, Action CallbackOnEnd2 = null, Action CallbackOnEnd3 = null, Action CallbackOnEndLoopOnce = null ) {
        if ( m_dicDirector.ContainsKey( key ) == false )
            return false;

        if ( CallbackOnEnd2 != null )
            m_dicDirector[key].SetCallbackOnEnd2( CallbackOnEnd2 );

        if ( CallbackOnEnd3 != null )
            m_dicDirector[key].SetCallbackOnEnd3( CallbackOnEnd3 );

        if ( CallbackOnEndLoopOnce != null ) {
            m_dicDirector[key].SetCallbackOnEndLoopOnce( CallbackOnEndLoopOnce );
        }

        m_dicDirector[key].Play();
        return true;
    }

    public virtual bool HoldPlayDirector( string key, Action CallbackOnEnd2 = null, Action CallbackOnEnd3 = null, Action HoldCallbackOnEnd = null ) {
        if ( m_dicDirector.ContainsKey( key ) == false )
            return false;

        if ( CallbackOnEnd2 != null )
            m_dicDirector[key].SetCallbackOnEnd2( CallbackOnEnd2 );

        if ( CallbackOnEnd3 != null )
            m_dicDirector[key].SetCallbackOnEnd3( CallbackOnEnd3 );

        if ( HoldCallbackOnEnd != null )
            m_dicDirector[key].SetHoldCallbackOnHoldCallback( HoldCallbackOnEnd );

        m_dicDirector[key].Play();
        return true;
    }

    public virtual bool DeactivateDirector( string key ) {
        if ( m_dicDirector.ContainsKey( key ) == false )
            return false;

        m_dicDirector[key].gameObject.SetActive( false );
        return true;
    }

    public bool IsPlayingDirector( string key ) {
        Director director = GetDirector(key);
        if ( director == null || director.isEnd == false )
            return false;

        return true;
    }

    public void SetAlive() {
        //if (transform == null)
        //    transform = GetComponent<Transform>();

        m_curHp = 1.0f;
    }

    public virtual void SetDie( bool setHpToZero = false ) {
        Debug.Log( "SetDie!!" );

        if( setHpToZero ) {
            m_curHp = 0.0f;
        }

        RestoreSpeed();

        if ( mUIHpBar )
            mUIHpBar.gameObject.SetActive( false );

        if ( mUIBoss ) {
            mUIBoss.gameObject.SetActive( false );
        }

        holdPositionRef = 0;
        skipAttack = false;

        if ( m_cmptBuffDebuff != null ) {
            m_cmptBuffDebuff.Clear();
        }

        StopStepForward();
        EndPushed();

        /*if (isGrounded == false)
            SetFallingRigidBody();
        else*/
        SetDieRigidbody();

        OnEndGame();
    }

    public virtual void OnDie() {
        SetDie();
    }

    public uint SetSuperArmor( eSuperArmor newSuperArmor ) {
        if ( m_curShield > 0.0f && newSuperArmor <= eSuperArmor.Lv2 ) {
            ForceSetSuperArmor( eSuperArmor.Lv2 );
            return 0;
        }

        if ( CurrentSuperArmor > newSuperArmor ) {
            return 0;
        }

        BeforeSuperArmor = CurrentSuperArmor;
        CurrentSuperArmor = newSuperArmor;

        return ++mCreatedSuperArmorId;
    }

    public void RestoreSuperArmor( uint changedSuperArmorId ) {
        if ( m_curShield > 0.0f && CurrentSuperArmor <= eSuperArmor.Lv2 ) {
            ForceSetSuperArmor( eSuperArmor.Lv2 );
            return;
        }

        if ( mCreatedSuperArmorId != changedSuperArmorId ) {
            BeforeSuperArmor = eSuperArmor.None;
            return;
        }

        CurrentSuperArmor = BeforeSuperArmor;
        BeforeSuperArmor = eSuperArmor.None;

        if ( mCreatedSuperArmorId > 0 ) {
            --mCreatedSuperArmorId;
        }
    }

    public void ForceSetSuperArmor( eSuperArmor superArmor ) {
        CurrentSuperArmor = superArmor;
        BeforeSuperArmor = eSuperArmor.None;

        mCreatedSuperArmorId = 0;
    }

    public void SetIncreaseSkillAtkValue( float value ) {
        IncreaseSkillAtkValue = value;
    }

    protected virtual void CheckRaycastCollision() {
        if ( IgnoreCheckRaycastCol || World.Instance.IsOpenSlopeMap ) {
            return;
        }

        if ( checkRayCollision && m_rigidBody && MainCollider && MainCollider.IsEnable() ) {
            // 더블 대쉬하면 바닥에 살짝 묻히는 경우가 있는 건지 레이캐스트가 걸려서 일단 이렇게 막아둠
            if ( m_actionSystem && m_actionSystem.currentAction && m_actionSystem.currentAction.actionCommand == eActionCommand.Defence ) {
                m_prevPos = transform.position;
                return;
            }

            Vector3 deltaPos = transform.position - m_prevPos;
            if ( deltaPos.sqrMagnitude > m_sqrMinExtent ) {
                int layer = (1 << (int)eLayer.Floor) | (1 << (int)eLayer.Wall) /*| (1 << (int)eLayer.Wall_Inside)*/ | (1 << (int)eLayer.EnvObject);
                if ( m_stepForwardDistRatio > 1.0f ) {
                    layer |= Utility.GetEnemyLayer( ( eLayer )gameObject.layer );
                }

                RaycastHit hitInfo;
                if ( Physics.Raycast( m_prevPos, deltaPos.normalized, out hitInfo, deltaPos.sqrMagnitude, layer ) ) {
                    if ( hitInfo.collider == null )
                        return;

                    Debug.Log( tableName + "::CheckRaycastCollision!!!!!!!!!!!!!!!!!!!" );
                    transform.position = hitInfo.point;// - (deltaPos / deltaPos.sqrMagnitude) * m_partialExtent;

                    // 띄워졌다가 떨어질때 그라운드 리지드바디로 안만들어주면 
                    // 포지션은 강제로 바닥인데 점프 중 상태라 점프 종료 처리가 안됨
                    if ( m_actionSystem.currentAction && m_actionSystem.currentAction.actionCommand == eActionCommand.Floating )
                        SetGroundedRigidBody();
                }
            }
        }

        /*if (m_rigidBody && MainCollider && MainCollider.enabled)
            m_prevPos = transform.position;
        else*/
        m_prevPos = transform.position;
    }

    public Vector3 CheckCollisionWall( Vector3 startPos, Vector3 endPos ) {
        Vector3 newPos = endPos;

        RaycastHit hitInfo;
        if ( Physics.Linecast( startPos, endPos, out hitInfo, /*(1 << (int)eLayer.Wall_Inside) |*/ ( 1 << ( int )eLayer.Wall ) | ( 1 << ( int )eLayer.EnvObject ) ) == true )
            newPos = new Vector3( hitInfo.point.x, transform.position.y, hitInfo.point.z );

        return newPos;
    }

    protected virtual bool IsCollisionWall( Vector3 startPos, Vector3 endPos, ref Vector3 hitPoint ) {
        if ( MainCollider == null ) {
            return false;
        }

        RaycastHit hitInfo;
        if ( Physics.Linecast( startPos, endPos, out hitInfo, /*(1 << (int)eLayer.Wall_Inside) |*/ ( 1 << ( int )eLayer.Wall ) | ( 1 << ( int )eLayer.EnvObject ) ) == true ) {
            hitPoint = hitInfo.point;
            return true;
        }

        if ( Physics.CheckBox( MainCollider.GetCenterPos(), new Vector3( 0.15f, 0.15f, 0.15f ), Quaternion.identity, ( 1 << ( int )eLayer.Wall ) | ( 1 << ( int )eLayer.EnvObject ) ) ) {
            return true;
        }
        /*Collider[] cols = Physics.OverlapBox(MainCollider.GetCenterPos(), new Vector3(0.15f, 0.15f, 0.15f), Quaternion.identity, 1 << (int)eLayer.Wall);
        if(cols.Length > 0)
        {
            return true;
        }*/

        return false;
    }

    protected virtual void CheckVisible() {
        Vector3 viewPortPt = Camera.main.WorldToViewportPoint(GetCenterPos());
        if ( Utility.IsInViewPort( viewPortPt, 0.2f, 0.8f ) ) {
            isVisible = true;
        }
        else {
            isVisible = false;
        }
    }

    protected virtual void FixedUpdate() {
        if ( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            return;
        }

        if ( object.ReferenceEquals( Camera.main, null ) || object.ReferenceEquals( transform, null ) ||
            object.ReferenceEquals( m_rigidBody, null ) || object.ReferenceEquals( MainCollider, null ) ) {
            return;
        }

        CheckVisible();

        if ( isGrounded ) {
            m_posOnGround = transform.position;
        }

        if ( m_rigidBody ) {
            if ( alwaysKinematic ) {
                m_rigidBody.isKinematic = true;
            }

            if ( World.Instance.InGameCamera.Mode == InGameCamera.EMode.SIDE ) {
                if ( World.Instance.InGameCamera.SideSetting.LockAxis == eAxisType.Z && ( ( m_rigidBody.constraints & RigidbodyConstraints.FreezePositionZ ) == 0 ) ) {
                    m_rigidBody.constraints &= ~RigidbodyConstraints.FreezePositionX;
                    m_rigidBody.constraints |= RigidbodyConstraints.FreezePositionZ;
                }
                else if ( World.Instance.InGameCamera.SideSetting.LockAxis == eAxisType.X && ( ( m_rigidBody.constraints & RigidbodyConstraints.FreezePositionX ) == 0 ) ) {
                    m_rigidBody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                    m_rigidBody.constraints |= RigidbodyConstraints.FreezePositionX;
                }
            }

            if ( m_checkRayCastCol ) {
                CheckRaycastCollision();
            }
        }

        CheckCloneAI();
    }

    protected virtual void LateUpdate() {
        /*
		if( isClone && m_cloneOwner && IsFollowOwner ) {
			float f = 0.2f * (showCloneIndex + 1);
			transform.SetPositionAndRotation( m_cloneOwner.transform.position - ( m_cloneOwner.transform.forward * f ), m_cloneOwner.transform.rotation );
		}
        */
	}

    public void EnableRayCastCol( bool enable ) {
        m_checkRayCastCol = enable;

        if ( enable )
            m_prevPos = transform.position;
    }

    protected virtual void OnCollisionEnter( Collision col ) {
        if ( transform == null || m_rigidBody == null )
            return;

        m_contactNormal = Vector3.zero;

        if ( isGrounded )
            return;

        if ( col.gameObject.CompareTag( "Floor" ) == true )
            isGrounded = true;

        if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
            if ( isFalling && col.gameObject.layer == ( int )eLayer.Wall ) {
                if ( col.contacts != null && col.contacts.Length > 0 ) {
                    if ( col.contacts[0].normal.y > 0.0f ) {
                        isGrounded = true;
                    }
                }
            }

            int enemyLayer = Utility.GetEnemyLayer((eLayer)gameObject.layer);
            if ( ( enemyLayer & 1 << col.gameObject.layer ) != 0 ) {
                if ( Physics.Linecast( transform.position, transform.position - ( transform.up * 0.5f ), out RaycastHit hitInfo, 1 << ( int )enemyLayer ) ) {
                    isGrounded = true;
                }
            }
        }

        if ( isGrounded == true ) {
            if ( m_curHp > 0.0f ) {
                SetGroundedRigidBody();
            }
            else {
                SetDieRigidbody();
            }
        }
    }

    protected virtual void OnCollisionStay( Collision col ) {
    }

    protected virtual void OnCollisionExit( Collision col ) {
        if ( ( m_rigidBody && m_rigidBody.isKinematic ) || alwaysKinematic )
            return;

        float distance = 1.0f;
        if ( Physics.Raycast( transform.position, -transform.up, out RaycastHit hitInfo, Mathf.Infinity, 1 << ( int )eLayer.Floor ) ) {
            distance = hitInfo.distance;
        }

        // 요정도 뜬건 뜬걸로 처리안함
        if ( distance < 0.15f ) {
            return;
        }

        if ( col.gameObject.CompareTag( "Floor" ) == true ) {
            isGrounded = false;
            //m_curObstacle = null;
        }

        if ( ( col.gameObject.CompareTag( "Obstacle" ) == true /*|| col.gameObject.CompareTag("Enemy") == true*/) && m_contactNormal.y > 0.0f ) {
            isGrounded = false;
            //m_curObstacle = null;
        }

        if ( isGrounded == false ) {
            if ( actionSystem != null && actionSystem.IsCurrentAction( eActionCommand.MoveByDirection ) == true )
                SetFallingRigidBody();
        }
    }

    private IEnumerator BackToGroundBody() {
        SetKinematicRigidBody( true );

        yield return new WaitForEndOfFrame();
        SetGroundedRigidBody();
    }

    public virtual void ShowWeapon( bool show ) {
    }

    /*public void Notify(eObserverMsg observerMsg)
    {
        for (int i = 0; i < m_listObserver.Count; i++)
            m_listObserver[i].OnNotify(observerMsg);
    }

    public void AddObserver(IObserver observer)
    {
        if (m_listObserver == null)
            m_listObserver = new List<IObserver>();

        IObserver find = m_listObserver.Find(x => x == observer);
        if (find != null)
            return;

        m_listObserver.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        IObserver find = m_listObserver.Find(x => x == observer);
        if (find == null)
            return;

        m_listObserver.Remove(observer);
    }*/

    public bool HasBuff() {
        //if (m_cmptBuffDebuff.HasBuff() || m_buffStats.HasBuff())
        if ( m_cmptBuffDebuff.HasBuff() )
            return true;

        return false;
    }
    public bool HasDebuff() {
        //if (m_cmptBuffDebuff.HasDebuff() || m_buffStats.HasDebuff())
        if ( m_cmptBuffDebuff.HasDebuff() )
            return true;

        return false;
    }
    public bool HasBuffIcon( eBuffIconType bufficon ) {
        if ( m_cmptBuffDebuff.HasBuffIcon( bufficon ) )
            return true;

        return false;
    }

	public virtual bool OnEvent( BaseEvent evt, IActionBaseParam param = null ) {
		if ( !IsActivate() && m_curHp <= 0.0f || evt == null ) {
			return true;
		}

		if ( evt.Ignore ) {
			return true;
		}

		if ( evt.battleOptionData != null && evt.battleOptionData.useOnceAndIgnoreFunc ) {
			evt.Ignore = true;
		}

		// 디버프 효과 무시하는 옵션이 있는지 검사
		eEventType findImmuneDebuff = eEventType.EVENT_NONE;
		for ( int i = 0; i < ListImmuneDebuffType.Count; i++ ) {
			if ( ListImmuneDebuffType[i] == evt.eventType ) {
				findImmuneDebuff = ListImmuneDebuffType[i];
				break;
			}
		}

		if ( findImmuneDebuff != eEventType.EVENT_NONE ) {
			return true;
		}

		if ( evt.battleOptionData != null ) {
			if ( evt.battleOptionData.preConditionType == BattleOption.eBOConditionType.Character ) {
				if ( (int)evt.battleOptionData.targetValue != tableId ) {
					return true;
				}
			}

            if ( evt.battleOptionData.preConditionType == BattleOption.eBOConditionType.CharacterSkillId ) {
                ActionSelectSkillBase actionSelectSkill = m_actionSystem.currentAction as ActionSelectSkillBase;
                if ( actionSelectSkill && actionSelectSkill.ParentTableId != ( int)evt.battleOptionData.targetValue ) {
                    return true;
                }
            }

            if ( CurrentSuperArmor == eSuperArmor.Invincible &&
				evt.battleOptionData.timingType != BattleOption.eBOTimingType.OnStartAddAction && // AddAction의 경우 디버프 체크하면 안됨.
				evt.battleOptionData.buffDebuffType == eBuffDebuffType.Debuff ) {
				return true;
			}
		}

		BuffEvent buffEvt = evt as BuffEvent;

		switch ( evt.eventType ) {
			case eEventType.EVENT_ACTION_COMMAND_FROM_BATTLE_OPTION:
				ActionEvent actionEvt = evt as ActionEvent;
				if ( actionEvt != null ) {
					CommandAction( actionEvt.ActionCommand, new ActionParamFromBO( evt.battleOptionData ) );
				}
				return true;

			case eEventType.EVENT_ACTION_FIRE_PROJECTILE:
				ProjectileEvent projectileEvt = evt as ProjectileEvent;
				if ( projectileEvt == null || projectileEvt.sender == null || m_aniEvent == null ) {
					return true;
				}

				AniEvent.sEvent atkEvt = m_aniEvent.CreateEvent( eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip,
																 0.0f, 0.0f, projectileEvt.value );

				if ( projectileEvt.value2 > 1 ) {
					List<UnitCollider> listTargetCollider = projectileEvt.sender.GetTargetColliderList();
					for ( int i = 0; i < listTargetCollider.Count; i++ ) {
						if ( i >= projectileEvt.ListProjectile.Count ) {
							break;
						}

						if ( projectileEvt.ListProjectile[i] == null || listTargetCollider[i] == null || listTargetCollider[i].Owner == null ) {
							continue;
						}

						AniEvent.sProjectileInfo info = m_aniEvent.CreateProjectileInfo( projectileEvt.ListProjectile[i] );
						info.addedPosition = GetCenterPos() - transform.position;
						info.notAniEventAtk = true;

						if ( projectileEvt.value3 > 0 ) {
							info.followParentRot = true;
						}

                        BattleOption.eToExecuteType executeType = BattleOption.eToExecuteType.Unit;
                        if ( evt.battleOptionData != null ) {
                            executeType = evt.battleOptionData.toExecuteType;
                        }

                        projectileEvt.ListProjectile[i].Fire( projectileEvt.sender, executeType, atkEvt, info, listTargetCollider[i].Owner, -1 );
					}
				}
				else if ( projectileEvt.value2 > 0 && projectileEvt.ListProjectile.Count > 0 && projectileEvt.ListProjectile[0] != null ) {
					for ( int i = 0; i < projectileEvt.ListProjectile.Count; i++ ) {
                        if ( projectileEvt.ListProjectile[i].IsActivate() ) {
                            continue;
						}

						AniEvent.sProjectileInfo info = m_aniEvent.CreateProjectileInfo( projectileEvt.ListProjectile[0] );
						info.addedPosition = GetCenterPos() - transform.position;
						info.notAniEventAtk = true;

						if ( projectileEvt.value3 > 0 ) {
							info.followParentRot = true;
						}

						UnitCollider collider = projectileEvt.sender.GetMainTargetCollider( true );
						if ( collider && collider.Owner ) {
							projectileEvt.ListProjectile[i].Fire( projectileEvt.sender,
																 evt.battleOptionData != null ? evt.battleOptionData.toExecuteType : BattleOption.eToExecuteType.Unit,
																 atkEvt, info, collider.Owner, -1 );

                            break;
						}
					}
				}
				return true;

			case eEventType.EVENT_ACTION_FIRE_PROJECTILE_REGARDLESS_TARGET_COUNT:
				projectileEvt = evt as ProjectileEvent;
                if ( m_aniEvent == null || projectileEvt == null ) {
                    return true;
				}

				atkEvt = m_aniEvent.CreateEvent( eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, projectileEvt.value );

				if ( projectileEvt.value2 > 1 ) {
					int targetIndex = 0;
					List<UnitCollider> listTargetCollider = projectileEvt.sender.GetTargetColliderList();

					if ( listTargetCollider != null && listTargetCollider.Count > 0 ) {
						for ( int i = 0; i < projectileEvt.value2; i++ ) {
                            if( i >= projectileEvt.ListProjectile.Count ) {
                                break;
							}

                            if ( projectileEvt.ListProjectile[i] == null ) {
                                continue;
							}

							AniEvent.sProjectileInfo info = m_aniEvent.CreateProjectileInfo( projectileEvt.ListProjectile[i] );
							info.addedPosition = GetCenterPos() - transform.position;
							info.notAniEventAtk = true;

							if ( projectileEvt.value3 > 0 ) {
								info.followParentRot = true;
							}

                            BattleOption.eToExecuteType executeType = BattleOption.eToExecuteType.Unit;
                            if ( evt.battleOptionData != null ) {
                                executeType = evt.battleOptionData.toExecuteType;
                            }

                            projectileEvt.ListProjectile[i].Fire( projectileEvt.sender, executeType, atkEvt, info, listTargetCollider[targetIndex].Owner, -1 );

							if ( i < listTargetCollider.Count - 1 ) {
								++targetIndex;
							}
						}
					}
				}
				else if ( projectileEvt.value2 > 0 && projectileEvt.ListProjectile.Count > 0 && projectileEvt.ListProjectile[0] != null ) {
					AniEvent.sProjectileInfo info = m_aniEvent.CreateProjectileInfo( projectileEvt.ListProjectile[0] );
					info.addedPosition = GetCenterPos() - transform.position;
					info.notAniEventAtk = true;

					if ( projectileEvt.value3 > 0 ) {
						info.followParentRot = true;
					}

					UnitCollider collider = projectileEvt.sender.GetMainTargetCollider( true );
					if ( collider && collider.Owner ) {
                        BattleOption.eToExecuteType executeType = BattleOption.eToExecuteType.Unit;
                        if ( evt.battleOptionData != null ) {
                            executeType = evt.battleOptionData.toExecuteType;
                        }

                        projectileEvt.ListProjectile[0].Fire( projectileEvt.sender, executeType, atkEvt, info, collider.Owner, -1 );
					}
				}
				return true;

			case eEventType.EVENT_ACTION_FIRE_PROJECTILE_REPEAT:
				Utility.StopCoroutine( this, ref mCrUpdateFirePjt );

                projectileEvt = evt as ProjectileEvent;
                if ( projectileEvt != null ) {
                    mCrUpdateFirePjt = StartCoroutine( UpdateFireProjectile( projectileEvt ) );
                }
				return true;

			case eEventType.EVENT_ACTION_HIT_ATTACK:
			case eEventType.EVENT_ACTION_HIT_UPPER_ATTACK:
			case eEventType.EVENT_ACTION_HIT_KNOCKBACK_ATTACK:
			case eEventType.EVENT_ACTION_HIT_DOWN_ATTACK:
			case eEventType.EVENT_ACTION_HIT_STUN_ATTACK:
			case eEventType.EVENT_ACTION_HIT_GROGGY_ATTACK:
			case eEventType.EVENT_ACTION_HIT_ATTACK_CRITICAL: {
                if ( evt.sender == null || evt.battleOptionData == null || MainCollider == null || MainCollider.Owner == null ) {
                    return true;
				}

				if ( !GameSupport.IsBOConditionCheck( evt.battleOptionData.conditionType, evt.battleOptionData.CondValue, this ) ) {
					return true;
				}

				int effId = 0;
				float value1 = 0.0f;
				float attackPower = evt.sender.attackPower;

				actionEvt = evt as ActionEvent;
				if ( actionEvt != null ) {
					effId = actionEvt.EffId;
					value1 = evt.battleOptionData.value;
				}
				else if ( buffEvt != null ) {
					effId = buffEvt.effId;
					value1 = evt.value;
					attackPower = buffEvt.ExtraValue > 0.0f ? buffEvt.ExtraValue : attackPower;
				}

				if ( effId > 0 ) { // 발동 조건에 만족하지 않는다해도 스킬 사용 이펙트는 표시, 표시조차 안하면 기능 오류로 느껴짐
					EffectManager.Instance.Play( this, effId, (EffectManager.eType)evt.battleOptionData.effType );
				}

				eBehaviour behaviour = eBehaviour.None;
				switch ( evt.eventType ) {
					case eEventType.EVENT_ACTION_HIT_ATTACK:
					case eEventType.EVENT_ACTION_HIT_ATTACK_CRITICAL:
						behaviour = eBehaviour.Attack;
						break;
					case eEventType.EVENT_ACTION_HIT_UPPER_ATTACK:
						behaviour = eBehaviour.UpperAttack;
						break;
					case eEventType.EVENT_ACTION_HIT_KNOCKBACK_ATTACK:
						behaviour = eBehaviour.KnockBackAttack;
						break;
					case eEventType.EVENT_ACTION_HIT_DOWN_ATTACK:
						behaviour = eBehaviour.DownAttack;
						break;
					case eEventType.EVENT_ACTION_HIT_STUN_ATTACK:
						behaviour = eBehaviour.StunAttack;
						break;
					case eEventType.EVENT_ACTION_HIT_GROGGY_ATTACK:
						behaviour = eBehaviour.GroggyAttack;
						break;
				}

				AniEvent.sEvent attackEvt = new AniEvent.sEvent();
				attackEvt.behaviour = behaviour;
				attackEvt.hitEffectId = ( evt.battleOptionData == null || evt.battleOptionData.effId2 == 0 ) ? 1 : evt.battleOptionData.effId2;
				attackEvt.hitDir = eHitDirection.None;
				attackEvt.atkRatio = 1.0f;

				bool onlyDamageHit = evt.battleOptionData.value2 >= 1.0f ? true : false;

				AttackEvent atkEvent = new AttackEvent();
				atkEvent.SkipCheckOwnerAction = onlyDamageHit;

				if ( m_actionSystem ) {
					ActionHit actionHit = m_actionSystem.GetCurrentAction<ActionHit>();
					if ( actionHit && actionHit.State == ActionHit.eState.Float ) // 공중으로 뜨는 상태면 대미지만 주도록
					{
						onlyDamageHit = true;
					}
				}

				bool isCritical = evt.eventType == eEventType.EVENT_ACTION_HIT_ATTACK_CRITICAL;
				if ( evt.battleOptionData.value3 <= 0.0f ) {
					atkEvent.SetWithSingleTarget( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, evt.sender, evt.battleOptionData.toExecuteType, attackEvt,
												  attackPower * value1, eAttackDirection.Skip, isCritical, 0, EffectManager.eType.None,
												  MainCollider, 0.0f, true, false, onlyDamageHit );

					EventMgr.Instance.SendEvent( atkEvent );
				}
				else {
					atkEvent.SetWithSingleTarget( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, evt.sender, evt.battleOptionData.toExecuteType, attackEvt,
												  attackPower * value1, eAttackDirection.Skip, isCritical, 0, EffectManager.eType.None,
												  MainCollider, 0.0f, true, false, onlyDamageHit );

					World.Instance.StartCoroutine( DealyedSendAttackEvent( evt.battleOptionData.value3, atkEvent ) );
				}
			}
			return true;

			case eEventType.EVENT_ACTION_REFLECTION: {
                if ( m_aniEvent == null || evt.battleOptionData == null || attacker == null ) {
                    return true;
				}

				buffEvt = evt as BuffEvent;
                if ( buffEvt == null ) {
                    return true;
				}

				EffectManager.Instance.Play( this, evt.battleOptionData.effId1, (EffectManager.eType)evt.battleOptionData.effType );

                AniEvent.sEvent attackEvt = m_aniEvent.CreateEvent( eBehaviour.Attack, 1, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f );
                AttackEvent atkEvent = new AttackEvent();

				atkEvent.SetWithSingleTarget( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, this, evt.battleOptionData.toExecuteType, attackEvt,
											  attacker.attackPower * evt.battleOptionData.value, eAttackDirection.Skip, false, 0, EffectManager.eType.None,
											  attacker.MainCollider, 0.0f, true );

				EventMgr.Instance.SendEvent( atkEvent );
			}
			return true;

			case eEventType.EVENT_ACTION_REFLECTION_AROUND: {
                if ( m_aniEvent == null || evt.battleOptionData == null || attacker == null ) {
                    return true;
                }

                buffEvt = evt as BuffEvent;

				EffectManager.Instance.Play( this, evt.battleOptionData.effId1, (EffectManager.eType)evt.battleOptionData.effType );

                float targetValue = evt.battleOptionData.targetValue;
                AniEvent.sEvent attackEvt = m_aniEvent.CreateEvent( eBehaviour.Attack, 1, eHitDirection.None, eAttackDirection.Skip, 0.0f, targetValue, 1.0f );
                
                AttackEvent atkEvent = new AttackEvent();
                atkEvent.Set( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, this, evt.battleOptionData.toExecuteType, attackEvt,
							  attacker.attackPower * evt.battleOptionData.value, eAttackDirection.Skip, false, 0, EffectManager.eType.None,
							  GetEnemyColliderList(), 0.0f, true );

				EventMgr.Instance.SendEvent( atkEvent );
			}
			return true;

			case eEventType.EVENT_ATTACK_USING_ACCUM_DAMAGE: {
                if ( m_aniEvent == null || evt.sender == null || evt.battleOptionData == null || MainCollider == null || MainCollider.Owner == null ) {
                    return true;
                }

                buffEvt = evt as BuffEvent;

                EffectManager.Instance.Play( this, evt.battleOptionData.effId1, (EffectManager.eType)evt.battleOptionData.effType );

                float targetValue = evt.battleOptionData.targetValue;
                AniEvent.sEvent attackEvt = m_aniEvent.CreateEvent( eBehaviour.Attack, 1, eHitDirection.None, eAttackDirection.Skip, 0.0f, targetValue, 1.0f );

				AttackEvent atkEvent = new AttackEvent();
				atkEvent.SetWithSingleTarget( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, evt.sender, evt.battleOptionData.toExecuteType, attackEvt,
											  evt.battleOptionData.value, eAttackDirection.Skip, false, 0, EffectManager.eType.None,
											  MainCollider, 0.0f, true );

				EventMgr.Instance.SendEvent( atkEvent );
			}
			return true;

			case eEventType.EVENT_BUFF_HEAL: {
                buffEvt = evt as BuffEvent;
                if ( buffEvt == null || buffEvt.battleOptionData == null ) {
                    return true;
                }

                BattleOption.eToExecuteType executeType = BattleOption.eToExecuteType.Unit;
                if ( evt.battleOptionData != null ) {
                    executeType = evt.battleOptionData.toExecuteType;
                }

                AddHpPercentage( executeType, buffEvt.value, buffEvt.value3 >= 1.0f );
            }
			return true;

			case eEventType.EVENT_BUFF_HEAL_ABS: {
                buffEvt = evt as BuffEvent;
                if ( buffEvt == null || buffEvt.battleOptionData == null ) {
                    return true;
                }

                BattleOption.eToExecuteType executeType = BattleOption.eToExecuteType.Unit;
                if ( evt.battleOptionData != null ) {
                    executeType = evt.battleOptionData.toExecuteType;
                }

                AddHp( executeType, buffEvt.value, false );
            }
            return true;

			case eEventType.EVENT_BUFF_HEAL_COST_HP: {
                if ( curHp <= 0.0f ) {
                    return true;
                }

                buffEvt = evt as BuffEvent;
                if ( buffEvt == null || buffEvt.battleOptionData == null ) {
                    return true;
                }

                BattleOption.eToExecuteType executeType = BattleOption.eToExecuteType.Unit;
                if ( evt.battleOptionData != null ) {
                    executeType = evt.battleOptionData.toExecuteType;
                }

                AddHpPerCost( executeType, buffEvt.value, false );
            }
			return true;

			case eEventType.EVENT_STAT_CRITICAL_RATE_UP:
			case eEventType.EVENT_STAT_CRITICAL_RATE_DOWN: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
				}

				UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                if ( evt.eventType == eEventType.EVENT_STAT_CRITICAL_RATE_DOWN ) {
                    increaseType = UnitBuffStats.eIncreaseType.Decrease;
                }

				m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.CriticalRate, increaseType );
			}
			return true;

			case eEventType.EVENT_STAT_CRITICAL_RATE_UP_WITH_ACTIVE_ENEMY_COUNT_RATIO:
			case eEventType.EVENT_STAT_CRITICAL_RATE_DOWN_WITH_ACTIVE_ENEMY_COUNT_RATIO: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
				if ( evt.eventType == eEventType.EVENT_STAT_CRITICAL_RATE_DOWN_WITH_ACTIVE_ENEMY_COUNT_RATIO ) {
					increaseType = UnitBuffStats.eIncreaseType.Decrease;
				}

				m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.CriticalRate, increaseType, false, true );
			}
			return true;

			case eEventType.EVENT_STAT_CRITICAL_DMG_UP:
			case eEventType.EVENT_STAT_CRITICAL_DMG_DOWN: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                if ( evt.eventType == eEventType.EVENT_STAT_CRITICAL_DMG_DOWN ) {
                    increaseType = UnitBuffStats.eIncreaseType.Decrease;
                }

				m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.CriticalDmg, increaseType );
			}
			return true;

			case eEventType.EVENT_STAT_CRITICAL_RESIST_UP:
			case eEventType.EVENT_STAT_CRITICAL_RESIST_DOWN: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                if ( evt.eventType == eEventType.EVENT_STAT_CRITICAL_RESIST_UP ) {
                    increaseType = UnitBuffStats.eIncreaseType.Decrease;
                }

				m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.CriticalResist, increaseType );
			}
			return true;

			case eEventType.EVENT_STAT_CRITICAL_DEF_UP:
			case eEventType.EVENT_STAT_CRITICAL_DEF_DOWN: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                if ( evt.eventType == eEventType.EVENT_STAT_CRITICAL_DEF_UP ) {
                    increaseType = UnitBuffStats.eIncreaseType.Decrease;
                }

				m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.CriticalDef, increaseType );
			}
			return true;

			case eEventType.EVENT_BUFF_IGNORE_CRI_RATE:
				m_fixCriticalRate = 0.0f;
				break;

			case eEventType.EVENT_STAT_DMG_RATE_DOWN:
			case eEventType.EVENT_STAT_DMG_RATE_UP: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                if ( evt.eventType == eEventType.EVENT_STAT_DMG_RATE_DOWN ) {
                    increaseType = UnitBuffStats.eIncreaseType.Decrease;
                }

				m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.Damage, increaseType );
			}
			return true;

			case eEventType.EVENT_STAT_DMG_RATE_UP_WITH_ACTIVE_ENEMY_COUNT_RATIO:
			case eEventType.EVENT_STAT_DMG_RATE_DOWN_WITH_ACTIVE_ENEMY_COUNT_RATIO: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
				if ( evt.eventType == eEventType.EVENT_STAT_DMG_RATE_DOWN_WITH_ACTIVE_ENEMY_COUNT_RATIO ) {
					increaseType = UnitBuffStats.eIncreaseType.Decrease;
				}

				m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.Damage, increaseType, false, true );
			}
			return true;

			case eEventType.EVENT_STAT_DMG_RATE_DOWN_BY_BUFF_COUNT: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.Damage, UnitBuffStats.eIncreaseType.Decrease, false, false, true );
			}
			return true;

			case eEventType.EVENT_STAT_HEAL_RATE_UP:
			case eEventType.EVENT_STAT_HEAL_RATE_DOWN: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                if ( evt.eventType == eEventType.EVENT_STAT_HEAL_RATE_DOWN ) {
                    increaseType = UnitBuffStats.eIncreaseType.Decrease;
                }

				m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.Heal, increaseType );
			}
			return true;

			case eEventType.EVENT_STAT_TIME_DEBUFF_REDUCE:
			case eEventType.EVENT_STAT_TIME_DEBUFF_PROMOTE: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                if ( evt.eventType == eEventType.EVENT_STAT_TIME_DEBUFF_PROMOTE ) {
                    increaseType = UnitBuffStats.eIncreaseType.Decrease;
                }

				m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.DebuffDurTime, increaseType );
			}
			return true;

			case eEventType.EVENT_STAT_SP_ADD_INC_RATE:
			case eEventType.EVENT_STAT_SP_ADD_DEC_RATE: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                if ( evt.eventType == eEventType.EVENT_STAT_SP_ADD_DEC_RATE ) {
                    increaseType = UnitBuffStats.eIncreaseType.Decrease;
                }

				m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.AddSPRate, increaseType );
			}
			return true;

			case eEventType.EVENT_STAT_ATKPOWER_RATE_UP:
			case eEventType.EVENT_STAT_ATKPOWER_RATE_DOWN: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                if ( evt.eventType == eEventType.EVENT_STAT_ATKPOWER_RATE_DOWN ) {
                    increaseType = UnitBuffStats.eIncreaseType.Decrease;
                }

				m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.AttackPower, increaseType );
			}
			return true;

			case eEventType.EVENT_STAT_ATKPOWER_RATE_UP_BY_BUFF_COUNT: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.AttackPower, UnitBuffStats.eIncreaseType.Increase, false, false, true );
			}
			return true;

			case eEventType.EVENT_STAT_ATKPOWER_RATE_UP_WITH_ACTIVE_ENEMY_COUNT_RATIO: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.AttackPower, UnitBuffStats.eIncreaseType.Increase, false, true );
			}
			return true;

			case eEventType.EVENT_STAT_ATKPOWER_RATE_UP_BY_ENEMY_DEBUFF_STACK: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.AttackPower, UnitBuffStats.eIncreaseType.Increase,
												 false, false, false, (int)buffEvt.value2 );
			}
			return true;

			case eEventType.EVENT_STAT_ATKPOWER_RATE_DOWN_WITH_ACTIVE_ENEMY_COUNT_RATIO: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.AttackPower, UnitBuffStats.eIncreaseType.Decrease, false, true );
			}
			return true;

			case eEventType.EVENT_STAT_ATKPOWER_RATE_UP_ONCE: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.AttackPower, UnitBuffStats.eIncreaseType.Increase, true );
			}
			return true;


			case eEventType.EVENT_STAT_ATKPOWER_TO_SHIELD_RATE_UP:
			case eEventType.EVENT_STAT_ATKPOWER_TO_SHIELD_RATE_DOWN: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                if ( evt.eventType == eEventType.EVENT_STAT_ATKPOWER_TO_SHIELD_RATE_DOWN ) {
                    increaseType = UnitBuffStats.eIncreaseType.Decrease;
                }

				m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.AttackPowerToShield, increaseType );
			}
			return true;

			case eEventType.EVENT_STAT_ULTIMATESKILL_ATKPOWER_RATE_UP:
			case eEventType.EVENT_STAT_ULTIMATESKILL_ATKPOWER_RATE_DOWN: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                if ( evt.eventType == eEventType.EVENT_STAT_ULTIMATESKILL_ATKPOWER_RATE_DOWN ) {
                    increaseType = UnitBuffStats.eIncreaseType.Decrease;
                }

				m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.UltimateSkillAtkPower, increaseType );
			}
			return true;

			case eEventType.EVENT_STAT_SPEED_UP:
			case eEventType.EVENT_STAT_SPEED_DOWN: {
                if ( buffEvt == null || m_buffStats == null ) {
                    return true;
                }

                UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                if ( evt.eventType == eEventType.EVENT_STAT_SPEED_DOWN ) {
                    increaseType = UnitBuffStats.eIncreaseType.Decrease;
                }

				m_buffStats.AddBuffStatAndStack( buffEvt, UnitBuffStats.eBuffStatType.Speed, increaseType );

                BattleOption.eToExecuteType executeType = BattleOption.eToExecuteType.Unit;
                if ( buffEvt.battleOptionData != null ) {
                    executeType = buffEvt.battleOptionData.toExecuteType;
                }

                SetSpeedRateByCalcBuff( executeType, true, true );
			}
			return true;

			case eEventType.EVENT_STAT_PENETRATE_UP: {
                if ( buffEvt == null ) {
                    return true;
				}

				AddPenetrate( buffEvt.value );
			}
			return true;

			case eEventType.EVENT_STAT_SUFFERANCE_UP: {
                if ( buffEvt == null ) {
                    return true;
                }

                AddSufferance( buffEvt.value );
			}
			return true;

			case eEventType.EVENT_BUFF_BLOODSUCKING:
			case eEventType.EVENT_BUFF_DOT_HEAL:
			case eEventType.EVENT_BUFF_DOT_HEAL_ABS:
			case eEventType.EVENT_BUFF_DOT_HEAL_COST_HP:
			case eEventType.EVENT_BUFF_DOT_HEAL_COST_HP_CONDI:
			case eEventType.EVENT_BUFF_ATKPOWER_RATE_UP:
			case eEventType.EVENT_BUFF_ATKPOWER_TO_SHIELD_RATE_UP:
			case eEventType.EVENT_BUFF_ULTIMATESKILL_ATKPOWER_RATE_UP:
			case eEventType.EVENT_BUFF_DMG_RATE_DOWN:
			case eEventType.EVENT_BUFF_DMG_RATE_DOWN_WITH_ACCUM_DAMAGE:
            case eEventType.EVENT_BUFF_HEAL_RATE_UP:
            case eEventType.EVENT_BUFF_HEAL_ON_DIE_IN_TIME:
            case eEventType.EVENT_BUFF_SPEED_UP:
			case eEventType.EVENT_BUFF_SPEED_UP_ON_EFFECT:
			case eEventType.EVENT_BUFF_CRITICAL_RATE_UP:
			case eEventType.EVENT_BUFF_CRITICAL_DMG_UP:
			case eEventType.EVENT_BUFF_CRITICAL_RESIST_UP:
			case eEventType.EVENT_BUFF_CRITICAL_DEF_UP:
			case eEventType.EVENT_BUFF_REFLECTION:
			case eEventType.EVENT_BUFF_REFLECTION_AROUND:
			case eEventType.EVENT_BUFF_RANDOM_KNOCKBACK:
			case eEventType.EVENT_BUFF_SUPER_ARMOR:
			case eEventType.EVENT_BUFF_CONDITIONAL_SUPER_ARMOR:
			case eEventType.EVENT_BUFF_HOLD_POSITION:
			case eEventType.EVENT_BUFF_DOT_DMG_TO_TARGET:
			case eEventType.EVENT_BUFF_ATTACK_TO_TARGET:
			case eEventType.EVENT_BUFF_TIME_DEBUFF_REDUCE:
			case eEventType.EVENT_BUFF_SP_ADD_INC_RATE:
			case eEventType.EVENT_BUFF_PENETRATE_UP:
			case eEventType.EVENT_BUFF_SUFFERANCE_UP:
			case eEventType.EVENT_DEBUFF_DOT_DMG:
			case eEventType.EVENT_DEBUFF_ATKPOWER_RATE_DOWN:
			case eEventType.EVENT_DEBUFF_ATKPOWER_TO_SHIELD_RATE_DOWN:
			case eEventType.EVENT_DEBUFF_ULTIMATESKILL_ATKPOWER_RATE_DOWN:
			case eEventType.EVENT_DEBUFF_DMG_RATE_UP:
			case eEventType.EVENT_DEBUFF_HEAL_RATE_DOWN:
			case eEventType.EVENT_DEBUFF_SPEED_DOWN:
			case eEventType.EVENT_DEBUFF_SPEED_DOWN_AND_SKIP_CUR_ANI:
			case eEventType.EVENT_DEBUFF_SPEED_DOWN_TO_TARGET:
			case eEventType.EVENT_DEBUFF_CRITICAL_RATE_DOWN:
			case eEventType.EVENT_DEBUFF_CRITICAL_DMG_DOWN:
			case eEventType.EVENT_DEBUFF_CRITICAL_RESIST_DOWN:
			case eEventType.EVENT_DEBUFF_CRITICAL_DEF_DOWN:
			case eEventType.EVENT_DEBUFF_HOLD_POSITION:
			case eEventType.EVENT_DEBUFF_SKIP_ATTACK:
			case eEventType.EVENT_DEBUFF_TIME_DEBUFF_PROMOTE:
			case eEventType.EVENT_DEBUFF_DECREASE_SP:
			case eEventType.EVENT_DEBUFF_SP_ADD_DEC_RATE:
			case eEventType.EVENT_DEBUFF_MOVEMENT_INVERSE:
			case eEventType.EVENT_DEBUFF_FREEZE:
			case eEventType.EVENT_BUFF_COMMAND_CLONE_ATTACK02:
			case eEventType.EVENT_DEBUFF_MARKING:
			case eEventType.EVENT_DEBUFF_HOLD_SUPPORTER_SKILL_COOLTIME:
			case eEventType.EVENT_BLACKHOLE:
			case eEventType.EVENT_HIDE:
			case eEventType.EVENT_SUMMON_PLAYER_MINION:
			case eEventType.EVENT_SUMMON_PLAYER_MINION_BY_DIE_ENEMY_COUNT:
			case eEventType.EVENT_SP_REGEN_INC_RATE:
			case eEventType.EVENT_DEBUFF_ELECTRIC_SHOCK:
			case eEventType.EVENT_BUFF_LIGHTNING_ATTACK:
			case eEventType.EVENT_BUFF_FIRE_AURA:
                if ( buffEvt == null || m_cmptBuffDebuff == null ) {
                    return true;
				}

				if ( evt.eventType == eEventType.EVENT_DEBUFF_MOVEMENT_INVERSE ) {
					if ( gameObject && gameObject.layer == (int)eLayer.OtherObject ) {
						return true;
					}
					else {
						Player player = this as Player;
						if ( player && player.IsHelper ) {
							return true;
						}
					}
				}

				m_cmptBuffDebuff.Execute( buffEvt, ( param as ActionParamForCallback )?.OnCallback );
				return true;

			case eEventType.EVENT_BUFF_CLEAR_DEBUFF:
                if ( m_cmptBuffDebuff == null ) {
                    return true;
				}

				m_cmptBuffDebuff.ClearAllDebuff();
				return true;

			case eEventType.EVENT_BUFF_REMOVE_DEBUFF:
                if ( buffEvt == null || m_cmptBuffDebuff == null ) {
                    return true;
                }

                m_cmptBuffDebuff.RemoveDebuff( (int)buffEvt.value, false );
				break;

			case eEventType.EVENT_BUFF_REMOVE_DEBUFF_AND_INCAPABLE:
                if ( buffEvt == null || m_cmptBuffDebuff == null ) {
                    return true;
                }

                m_cmptBuffDebuff.RemoveDebuff( (int)buffEvt.value, true );
				break;

			case eEventType.EVENT_BUFF_SET_DEBUFF_IMMUNE:
                if ( buffEvt == null ) {
                    return true;
                }

                AddDebuffImmuneType( (eDebuffImmuneType_f)( 1 << (int)buffEvt.value ) );
				break;

			case eEventType.EVENT_DEBUFF_CLEAR_BUFF:
                if ( m_cmptBuffDebuff == null ) {
                    return true;
				}

				m_cmptBuffDebuff.ClearAllBuff();
				return true;

			case eEventType.EVENT_DEBUFF_REMOVE_BUFF:
                if ( buffEvt == null || m_cmptBuffDebuff == null ) {
                    return true;
                }

                if ( buffEvt.value2 > 0.0f && UnityEngine.Random.Range( 0, (int)eCOUNT.MAX_BO_FUNC_VALUE ) > (int)buffEvt.value2 ) {
                    return true;
                }

                m_cmptBuffDebuff.RemoveBuff( (int)buffEvt.value, buffEvt.value2 > 0.0f );
				break;

			case eEventType.EVENT_DEBUFF_DOT_SHIELD_DMG: {
                if ( buffEvt == null || m_cmptBuffDebuff == null ) {
                    return true;
                }

                if ( m_maxShield > 0 && m_curShield > 0.0f ) {
                    m_cmptBuffDebuff.Execute( buffEvt, EndDotShieldDmg );
                }
			}
			return true;

			case eEventType.EVENT_ITEM_DROP: {
                if ( evt.sender == null ) {
                    return true;
				}

				if ( !mLockAddDropItemRate && UnityEngine.Random.Range( 0, (int)eCOUNT.MAX_BO_FUNC_VALUE ) <= (int)evt.value ) {
					if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
						DropItemMgr.Instance.DropItemBySkill( null, GetCenterPos(), (int)evt.value2, (int)evt.value3 );
					}
					else if ( evt.battleOptionData != null ) {
						Vector3 pos = GetCenterPos();
						if ( evt.battleOptionData.timingType == BattleOption.eBOTimingType.OnHit ) {
							pos = evt.sender.GetCenterPos();
						}

						DropItemMgr.Instance.DropItemBySkill( evt.sender, pos, (int)evt.value2, (int)evt.value3 );
					}

					if ( m_curHp <= 0.0f ) {
						mLockAddDropItemRate = true;
					}
				}
			}
			return true;

			case eEventType.EVENT_IMMUNE_ATKATTR:
				ListImmuneAtkAttr.Add( (EAttackAttr)evt.value );
				return true;

			case eEventType.EVENT_IMMUNE_ATKPOWER_RATE_DOWN:
				ListImmuneDebuffType.Add( eEventType.EVENT_DEBUFF_ATKPOWER_RATE_DOWN );
				ListImmuneDebuffType.Add( eEventType.EVENT_STAT_ATKPOWER_RATE_DOWN );
				return true;

			case eEventType.EVENT_IMMUNE_DMG_RATE_UP:
				ListImmuneDebuffType.Add( eEventType.EVENT_DEBUFF_DMG_RATE_UP );
				ListImmuneDebuffType.Add( eEventType.EVENT_STAT_DMG_RATE_UP );
				return true;

			case eEventType.EVENT_IMMUNE_SPEED_DOWN:
				ListImmuneDebuffType.Add( eEventType.EVENT_DEBUFF_SPEED_DOWN );
				ListImmuneDebuffType.Add( eEventType.EVENT_STAT_SPEED_DOWN );
				return true;

			case eEventType.EVENT_SET_ADD_ACTION: {
                if ( actionSystem == null || evt.battleOptionData == null ) {
                    return true;
                }

				if ( evt.battleOptionData.conditionType != BattleOption.eBOConditionType.None ) {
					SetAddAction( evt.battleOptionData.conditionType, evt );
				}
				else if ( evt.battleOptionData.ActionConditionType != null ) {
					for ( int i = 0; i < evt.battleOptionData.ActionConditionType.Length; i++ ) {
						SetAddAction( evt.battleOptionData.ActionConditionType[i], evt );
					}
				}
			}
            return true;

			case eEventType.EVENT_INCREASE_ADD_ACTION_VALUE: {
				if ( actionSystem == null || evt.battleOptionData == null ) {
					return true;
				}

				if ( evt.battleOptionData.conditionType != BattleOption.eBOConditionType.None ) {
					SetIncreaseAddAction( evt.battleOptionData.conditionType, evt );
				}
				else if ( evt.battleOptionData.ActionConditionType != null ) {
					for ( int i = 0; i < evt.battleOptionData.ActionConditionType.Length; i++ ) {
						SetIncreaseAddAction( evt.battleOptionData.ActionConditionType[i], evt );
					}
				}
			}
			return true;

			case eEventType.EVENT_DEBUFF_BREAK_SHIELD:
				if ( m_maxShield > 0.0f && m_curShield > 0.0f ) {
					BreakShield();
					UpdateShieldGauge();

					StunAfterShieldBreak();
				}
				return true;

			case eEventType.EVENT_BUFF_ADD_BUFF_DEBUFF_DURATION:
                if ( buffEvt == null ) {
                    return true;
				}

				AddCmptDebuffDuration( buffEvt );
				return true;

			case eEventType.EVENT_PUSHED:
                if ( buffEvt == null || buffEvt.sender == null ) {
                    return true;
                }

                Pushed( buffEvt.sender, buffEvt.duration, buffEvt.value );
				return true;

			case eEventType.EVENT_DEATH: {
                bool b = ( UnityEngine.Random.Range( 0, (int)eCOUNT.MAX_BO_FUNC_VALUE ) <= (int)evt.value );

                if ( !b || ( this as Player ) != null || m_grade >= eGrade.Boss ) { // 즉사 실패
                    if ( evt.battleOptionData != null && evt.battleOptionData.dataOnEndCall != null && evt.battleOptionData.dataOnEndCall.evt != null ) {
                        evt.battleOptionData.dataOnEndCall.evt.SelectTarget = this;
                        evt.battleOptionData.dataOnEndCall.useTime = DateTime.Now;

                        EventMgr.Instance.SendEvent( evt.battleOptionData.dataOnEndCall.evt );
                        Log.Show( evt.battleOptionData.dataOnEndCall.evt.battleOptionData.battleOptionSetId + "번 배틀옵션셋 사용 (애드콜)!!!", Log.ColorType.Green );
                    }
                }
                else if ( evt.battleOptionData != null && MainCollider && MainCollider.Owner ) {
                    EffectManager.Instance.Play( this, evt.battleOptionData.effId1, EffectManager.eType.Each_Monster_Normal_Hit, 0.0f, false, true );

                    AniEvent.sEvent attackEvt = new AniEvent.sEvent();
                    attackEvt.behaviour = eBehaviour.Attack;
                    attackEvt.hitEffectId = ( evt.battleOptionData == null || evt.battleOptionData.effId2 == 0 ) ? 1 : evt.battleOptionData.effId2;
                    attackEvt.hitDir = eHitDirection.None;
                    attackEvt.atkRatio = 1.0f;

                    AttackEvent atkEvent = new AttackEvent();
                    atkEvent.SkipCheckOwnerAction = true;
                    atkEvent.SkipMaxDamageRecord = true;

                    atkEvent.SetWithSingleTarget( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, evt.sender, evt.battleOptionData.toExecuteType, attackEvt,
                                                  maxHp * 5.0f, eAttackDirection.Skip, false, 0, EffectManager.eType.None,
                                                  MainCollider, 0.0f, true, true, false );

                    EventMgr.Instance.SendEvent( atkEvent );
                }
            }
			return true;
		}

		return false;
	}

	private IEnumerator DealyedSendAttackEvent( float delay, AttackEvent attackEvt ) {
        yield return new WaitForSeconds( delay );
        EventMgr.Instance.SendEvent( attackEvt );
    }

	public virtual void ExecuteBattleOption( BattleOption.eBOTimingType timingType, int actionTableId, Projectile projectile, bool skipWeaponBO = false ) {
    }

    public virtual void EndBattleOption( int battleOptionSetId ) {
        m_buffStats.RemoveBuffStat( battleOptionSetId );
        m_cmptBuffDebuff.RemoveExecute( battleOptionSetId );
    }

    /*protected virtual void UpdateDirection(Vector3 dir)
    {
        //m_inputDir = dir;
        m_dir = (dir == Vector3.zero) ? transform.forward : dir;
    }*/

    protected void SetAtkPowerUpRate( BuffEvent evt ) {
        if ( evt == null )
            return;

        if ( evt.effId > 0 ) {
            AttachObject attachObj = m_costumeUnit.GetAttachObject(AttachObject.eTYPE.WEAPON_R);
            if ( attachObj != null && attachObj.attachedEffId != evt.effId ) {
                if ( attachObj.attachedEffId > 0 )
                    EffectManager.Instance.StopEffImmediate( attachObj.attachedEffId, EffectManager.eType.Common, null );

                attachObj.attachedEffId = evt.effId;
                EffectManager.Instance.Play( attachObj.gameObject, evt.effId, EffectManager.eType.Common );
            }

            attachObj = m_costumeUnit.GetAttachObject( AttachObject.eTYPE.WEAPON_L );
            if ( attachObj != null && attachObj.attachedEffId != evt.effId ) {
                if ( attachObj.attachedEffId > 0 )
                    EffectManager.Instance.StopEffImmediate( attachObj.attachedEffId, EffectManager.eType.Common, null );

                attachObj.attachedEffId = evt.effId;
                EffectManager.Instance.Play( attachObj.gameObject, evt.effId, EffectManager.eType.Common );
            }
        }
    }

    protected void ClearAtkPowerUpRate() {
        //m_atkPowerUpRate = 0.0f;

        AttachObject attachObj = m_costumeUnit.GetAttachObject(AttachObject.eTYPE.WEAPON_R);
        if ( attachObj != null ) {
            if ( attachObj.attachedEffId > 0 ) {
                EffectManager.Instance.StopEffImmediate( attachObj.attachedEffId, EffectManager.eType.Common, null );
                attachObj.attachedEffId = 0;
            }
        }

        attachObj = m_costumeUnit.GetAttachObject( AttachObject.eTYPE.WEAPON_L );
        if ( attachObj != null ) {
            if ( attachObj.attachedEffId > 0 ) {
                EffectManager.Instance.StopEffImmediate( attachObj.attachedEffId, EffectManager.eType.Common, null );
                attachObj.attachedEffId = 0;
            }
        }
    }

    /*public bool ReserveNextAttack()
    {
        ActionBase currentAction = m_actionSystem.currentAction;
        if (currentAction != null)
        {
            ActionComboAttack actionComboAttack = currentAction as ActionComboAttack;
            if (actionComboAttack != null)
            {
                if (actionComboAttack.reserved == true)
                    return false;

                return actionComboAttack.ReserveNextAttack();
            }
        }

        return true;
    }

    private IEnumerator SetOriginalRigidBodyMass()
    {
        yield return new WaitForSeconds(1.0f);
        m_rigidBody.mass = m_originalMass;
    }*/

    public virtual void SetGroundedRigidBody()
    {
        if (m_rigidBody == null || MainCollider == null)
            return;

        Debug.Log(name + "바닥리지드바디 설정!!!!!!!!!");

        isGrounded = true;
        isFalling = false;

        MainCollider.Enable(true);

        m_rigidBody.mass = m_originalMass;
        m_rigidBody.useGravity = true;
        m_rigidBody.constraints = mDefaultRigidbodyConstraints;

        if (!alwaysKinematic)
            m_rigidBody.isKinematic = false;

        m_posOnGround = transform.position;
    }

    public virtual bool SetFloatingRigidBody()
    {
        if (m_rigidBody == null || MainCollider == null)
            return false;

        Debug.Log(name + "SetFloatingRigidBody 설정!!!!!!!!");

        if (isGrounded)
        {
            m_posOnGround = transform.position;
        }

        isGrounded = false;
        isFalling = false;

        m_rigidBody.mass = m_massOnFloating;
        m_rigidBody.useGravity = false;

        if (!alwaysKinematic)
            m_rigidBody.isKinematic = false;

        m_rigidBody.constraints = mDefaultRigidbodyConstraints;// ^ RigidbodyConstraints.FreezePositionY;
        if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
        {
            //m_rigidBody.constraints &= ~RigidbodyConstraints.FreezePositionY;
            MainCollider.Enable(false);
        }
        else
        {
            MainCollider.Enable(true);
        }

        return true;
    }

    public virtual void SetFallingRigidBody(bool useGravity = true)
    {
        if (m_rigidBody == null || MainCollider == null)
            return;

        Debug.Log(name + "FallingRigidBody 설정!!!!!!!!");

        isGrounded = false;
        isFalling = true;

        MainCollider.Enable(true);

        m_rigidBody.useGravity = useGravity;
        m_rigidBody.mass = m_massOnFloating;

        m_rigidBody.constraints = mDefaultRigidbodyConstraints;
        /*
        if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
        {
            m_rigidBody.constraints ^= RigidbodyConstraints.FreezePositionY;
        }
        */
        if (!alwaysKinematic)
            m_rigidBody.isKinematic = false;
    }

    public void SetKinematicRigidBody(bool kinematic = true, bool ignoreGravity = false)
    {
        if (m_rigidBody == null || (alwaysKinematic && !kinematic))
            return;

        if (kinematic)
        {
            Debug.Log(name + "키네마틱 설정!!!!!!!!");
        }
        else
        {
            Debug.Log(name + "키네마틱 해제!!!!!!!!");
        }

        m_rigidBody.velocity = Vector3.zero;
        m_rigidBody.isKinematic = kinematic;

        if (ignoreGravity)
        {
            m_rigidBody.useGravity = false;
        }
    }

    public void SetDieRigidbody()
    {
        MainCollider.Enable(false);
        SetKinematicRigidBody();
    }

    public void IgnoreCollision()
    {
        checkRayCollision = false;

        Utility.IgnorePhysics(eLayer.Wall, eLayer.Player);
        Utility.IgnorePhysics(eLayer.Enemy, eLayer.Player);
        Utility.IgnorePhysics(eLayer.EnemyGate, eLayer.Player);
        Utility.IgnorePhysics(eLayer.EnvObject, eLayer.Player);
    }

    public void RestoreCollision()
    {
        checkRayCollision = true;

        Utility.SetPhysicsLayerCollision(eLayer.Wall, eLayer.Player);
        Utility.SetPhysicsLayerCollision(eLayer.Enemy, eLayer.Player);
        Utility.SetPhysicsLayerCollision(eLayer.EnemyGate, eLayer.Player);
        Utility.SetPhysicsLayerCollision(eLayer.EnvObject, eLayer.Player);
    }

    /*public void SetDieRigidBody()
    {
        StopStepForward();
        StopPushed();

        if (isGrounded == false)
            SetFallingRigidBody();
        else
            SetKinematicRigidBody(true);
    }

    public void SetEndGameRigidBody()
    {
        Utility.IgnorePhysics(m_rigidBody, eLayer.Player, eLayer.Enemy);

        StopStepForward();
        StopPushed();

        if (isGrounded == false)
            SetFallingRigidBody();
        else
            SetKinematicRigidBody();
    }*/

    public float GetDebuffTimeReduceValue(BattleOption.eToExecuteType toExecuteType)
    {
        float debuffTimeDurRate = m_buffStats.CalcBuffStat(toExecuteType, UnitBuffStats.eBuffStatType.DebuffDurTime, UnitBuffStats.eIncreaseType.Increase,
                                                           0.0f, false, false, false, false, this);

        debuffTimeDurRate = m_buffStats.CalcBuffStat(toExecuteType, UnitBuffStats.eBuffStatType.DebuffDurTime, UnitBuffStats.eIncreaseType.Decrease,
                                                     debuffTimeDurRate, false, false, false, false, this);

        debuffTimeDurRate = Mathf.Min(1.0f, debuffTimeDurRate);
        return debuffTimeDurRate;
    }

    public virtual void SetSpeedRateByCalcBuff(BattleOption.eToExecuteType toExecuteType, bool withAniSpeed, bool changeEffectSpeed)
    {
		bool isUltimateSkill = false;
		Player player = this as Player;
		if(player && player.UsingUltimateSkill)
		{
			isUltimateSkill = true;
		}

		float speedRate = m_buffStats.CalcBuffStat(toExecuteType, UnitBuffStats.eBuffStatType.Speed, UnitBuffStats.eIncreaseType.Increase,
                                                   0.0f, false, false, isUltimateSkill, false, this);

        speedRate = m_buffStats.CalcBuffStat(toExecuteType, UnitBuffStats.eBuffStatType.Speed, UnitBuffStats.eIncreaseType.Decrease,
                                             speedRate, false, false, isUltimateSkill, false, this);

		SetSpeedRateByBuff(speedRate, withAniSpeed, changeEffectSpeed);
    }

    public virtual void SetSpeedRateByBuff(float rate, bool withAniSpeed = true, bool changeEffectSpeed = true)
    {
        m_speedRateByBuff = rate;
        SetSpeedRate(0.0f, withAniSpeed, changeEffectSpeed);
    }

    public void SetSpeedRateBySprint(float rate)
    {
        m_speedRateBySprint = rate;
        SetSpeedRate(0.0f, false);
    }

    public void SetSpeedRate(float aniSpeedRate, bool withAniSpeed = true, bool changeEffectSpeed = true)
    {
        aniSpeedRate += (m_speedRateByBuff + m_speedRateBySprint);
        m_curSpeed = Mathf.Max(0.0f, m_originalSpeed + (m_originalSpeed * aniSpeedRate));
		m_curSpeed += (m_curSpeed * TemporarySpeedRate);

		if (withAniSpeed && m_aniEvent)
		{
			m_aniEvent.SetAniSpeed(Mathf.Max(0.0f, 1.0f + (aniSpeedRate - m_speedRateBySprint)), changeEffectSpeed);
		}

		if (child)
		{
			child.SetSpeedRate(aniSpeedRate);
		}
    }

    public void RestoreSpeed()
    {
        m_curSpeed = m_originalSpeed;

        if (m_aniEvent)
        {
            m_aniEvent.SetAniSpeed(1.0f);
        }

        if (child)
        {
            child.RestoreSpeed();
        }
    }

	public virtual bool AddHp( BattleOption.eToExecuteType toExecuteType, float add, bool skipCheckDie ) {
        if( !skipCheckDie && m_curHp <= 0.0f ) {
            return false;
		}

		float resultAdd = add;

		// 회복 능력 증가 버프에 의한 회복력 계산
		resultAdd = m_buffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.Heal, UnitBuffStats.eIncreaseType.Increase,
											 resultAdd, false, false, false, false, this );

		// 회복 능력 감소 버프에 의한 회복력 계산
		resultAdd = m_buffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.Heal, UnitBuffStats.eIncreaseType.Decrease,
											 resultAdd, false, false, false, false, this );

        HealedWithOverHeal += resultAdd;

        float beforeCurHp = m_curHp;
		m_curHp = Mathf.Clamp( m_curHp + resultAdd, m_curHp, m_maxHp );
        Healed += m_curHp - beforeCurHp;

		HUDResMgr.Instance.ShowRecovery( this, gameObject.layer == (int)eLayer.Player, (int)resultAdd );

		if( m_grade != eGrade.Boss && mUIHpBar ) {
			mUIHpBar.AddHp( (int)resultAdd );
		}

        ExecuteBattleOption( BattleOption.eBOTimingType.OnRecovery, 0, null );
        return true;
	}

	public virtual bool AddHpPercentage( BattleOption.eToExecuteType toExecuteType, float addPercentage, bool skipCheckDie ) {
        if( !skipCheckDie && m_curHp <= 0.0f ) {
            return false;
        }

        float resultAdd = m_maxHp * addPercentage;

		// 회복 능력 증가 버프에 의한 회복력 계산
		resultAdd = m_buffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.Heal, UnitBuffStats.eIncreaseType.Increase,
											  resultAdd, false, false, false, false, this );

		// 회복 능력 감소 버프에 의한 회복력 계산
		resultAdd = m_buffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.Heal, UnitBuffStats.eIncreaseType.Decrease,
											  resultAdd, false, false, false, false, this );

        HealedWithOverHeal += resultAdd;

        float beforeCurHp = m_curHp;
        m_curHp = Mathf.Clamp( m_curHp + resultAdd, m_curHp, m_maxHp );
        Healed += m_curHp - beforeCurHp;

        HUDResMgr.Instance.ShowRecovery( this, gameObject.layer == (int)eLayer.Player, (int)resultAdd );

        if( m_grade != eGrade.Boss && mUIHpBar ) {
            mUIHpBar.AddHp( (int)resultAdd );
        }

        ExecuteBattleOption( BattleOption.eBOTimingType.OnRecovery, 0, null );
        return true;
	}

	public virtual bool AddHpPerCost( BattleOption.eToExecuteType toExecuteType, float addPercentage, bool skipCheckDie ) {
        if( !skipCheckDie && m_curHp <= 0.0f ) {
            return false;
        }

        float f = m_maxHp - m_curHp;
        if( f <= 0.0f ) {
            return false;
		}

        float resultAdd = f * addPercentage;

		// 회복 능력 증가 버프에 의한 회복력 계산
		resultAdd = m_buffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.Heal, UnitBuffStats.eIncreaseType.Increase,
											  resultAdd, false, false, false, false, this );

		// 회복 능력 감소 버프에 의한 회복력 계산
		resultAdd = m_buffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.Heal, UnitBuffStats.eIncreaseType.Decrease,
											  resultAdd, false, false, false, false, this );

        HealedWithOverHeal += resultAdd;

        float beforeCurHp = m_curHp;
        m_curHp = Mathf.Clamp( m_curHp + resultAdd, m_curHp, m_maxHp );
        Healed += m_curHp - beforeCurHp;

        HUDResMgr.Instance.ShowRecovery( this, gameObject.layer == (int)eLayer.Player, (int)resultAdd );

		if( m_grade != eGrade.Boss && mUIHpBar ) {
			mUIHpBar.AddHp( (int)resultAdd );
		}

        ExecuteBattleOption( BattleOption.eBOTimingType.OnRecovery, 0, null );
        return true;
	}

	public virtual void SubHp( ObscuredFloat damage, bool isCritical ) {
		if( ( !World.Instance.TestScene && World.Instance.EnableDamage ) || ( World.Instance.TestScene && World.Instance.TestScene.IsEnableDamage ) ) {
			m_curHp = Mathf.Clamp( m_curHp - damage, 0.0f, m_maxHp );
		}

		if( m_aniEvent ) {
			m_aniEvent.SetFlash( rimColorOnHit, rimMinOnHit, rimMaxOnHit, 9.0f / Application.targetFrameRate );

            if( m_child ) {
                m_child.m_aniEvent.SetFlash( rimColorOnHit, rimMinOnHit, rimMaxOnHit, 9.0f / Application.targetFrameRate );
            }
		}

		HUDResMgr.Instance.ShowDamage( this, gameObject.layer == (int)eLayer.Player, isCritical, (int)( damage > 0.0f ? Mathf.Max( 1, (int)damage ) : 0.0f ) );

        if( m_grade != eGrade.Boss && mUIHpBar ) {
            mUIHpBar.SubHp( (int)damage );
        }
	}

	public virtual bool SubShield(float sub)
    {
        if (m_curShield <= 0.0f)
            return false;

        m_curShield = Mathf.Clamp(m_curShield - sub, 0.0f, m_maxShield);
        return true;
    }

	/* 220203
    public void HideAllDamageFont()
    {
        for (int i = 0; i < mListUIDamageText.Count; i++)
        {
            mListUIDamageText[i].HideDamage();
        }
    }
	*/

	protected virtual void EndDotShieldDmg()
    {
    }

    /*public void SetCapsuleColDirection(eCapsuleColAxis axis, bool changeCenter = false)
    {
        if (MainCollider == null)
            return;

        MainCollider.direction = (int)axis;

        if (axis == eCapsuleColAxis.Y)
            MainCollider.center = m_originalCapsuleColCenter;
        else if (axis == eCapsuleColAxis.Z)
            MainCollider.center = colZAxisCenter;
    }*/

    public virtual ActionBase CommandAction(eActionCommand actionCommand, IActionBaseParam param)
    {
        switch(AllowAction)
        {
            case eAllowAction.None:
                return null;

            case eAllowAction.MoveAndDash:
                if(!m_actionSystem.IsMoveAction(actionCommand) && !m_actionSystem.IsDashAction(actionCommand))
                {
                    return null;
                }
                break;

            case eAllowAction.Attack:
                if(!m_actionSystem.IsAnyAttackAction(actionCommand))
                {
                    return null;
                }
                break;
        }

        if (m_actionSystem2 && m_actionSystem2.IsSupporterAction(actionCommand))
        {
            m_actionSystem2.DoAction(actionCommand, param, false);
            return null;
        }

        if (ActionSystem3)
        {
            ActionBase action = ActionSystem3.GetAction(actionCommand);
            if (action && action.IndependentActionSystem)
            {
                ActionSystem3.DoAction(actionCommand, param, false);
                return null;
            }
        }

        if (m_actionSystem == null)
        {
            return null;
        }

        return m_actionSystem.DoAction(actionCommand, param);
    }

    public virtual void OnEndAction()
    {
    }

    public virtual void Show()
    {
        IsShow = true;
        ShowMesh(true);

        if (hpBar)
        {
            hpBar.Show(false);
        }
        if (uiBuffDebuffIcon)
        {
            uiBuffDebuffIcon.Show(false);
        }

        if (!alwaysKinematic)
        {
            SetKinematicRigidBody(false);
        }

        if (MainCollider)
        {
            MainCollider.Enable(true);
        }
    }

    public virtual void Hide()
    {
        IsShow = false;
        ShowMesh(false);

        if (hpBar)
        {
            hpBar.Show(false);
        }
        if (uiBuffDebuffIcon)
        {
            uiBuffDebuffIcon.Show(false);
        }

        if (!alwaysKinematic)
        {
            SetKinematicRigidBody(true);
        }

        if (MainCollider)
        {
            MainCollider.Enable(false);
        }
    }

	public virtual bool ShowMesh( bool show, bool isLock = false ) {
		if ( mLockShowMesh || IsShowMesh == show ) {
			return false;
		}

		mLockShowMesh = isLock;
		IsShowMesh = show;
		m_aniEvent.ShowMesh( show );

        if ( m_costumeUnit != null ) {
            m_costumeUnit.ShowAttachedObject( show );
        }

		if ( FSaveData.Instance.Graphic <= 0 ) {
			ShowShadow( false );
		}
		else {
			ShowShadow( show );
		}

		return true;
	}

	public virtual void ReleaseLockShowMesh()
    {
        mLockShowMesh = false;
    }

    public virtual void LookAtTarget(Vector3 targetPos)
    {
        if(m_aniEvent && m_aniEvent.aniSpeed <= 0.0f)
        {
            return;
        }

        targetPos.y = transform.position.y;
        if (Vector3.Normalize(transform.position - targetPos) == Vector3.zero)
            return;

        if (World.Instance.InGameCamera.Mode == InGameCamera.EMode.SIDE && World.Instance.InGameCamera.SideSetting.LockAxis != eAxisType.None)
        {
            Vector3 v = Vector3.Normalize(targetPos - transform.position);
            float dot = Vector3.Dot(v, transform.forward);
            if (dot > 0.0f)
            {
                targetPos = transform.position + transform.forward;
            }
            else
            {
                targetPos = transform.position - transform.forward;
            }
        }

        transform.LookAt(targetPos);
    }

    public void LookAtTargetByCmptRotate(Vector3 targetPos, float lerpSpeed = 0.7f)
    {
        if (cmptRotate == null)
            return;

        targetPos.y = transform.position.y;

        if (World.Instance.InGameCamera.Mode == InGameCamera.EMode.SIDE && World.Instance.InGameCamera.SideSetting.LockAxis != eAxisType.None)
        {
            Vector3 v = Vector3.Normalize(targetPos - transform.position);
            float dot = Vector3.Dot(v, transform.forward);
            if (dot > 0.0f)
            {
                targetPos = transform.position + transform.forward;
            }
            else
            {
                targetPos = transform.position - transform.forward;
            }
        }

        cmptRotate.LookAtTarget(targetPos, lerpSpeed);
    }

    public UnitCollider GetNearestColliderFromPos(Vector3 comparePos)
    {
        if (MainCollider == null)
            return null;

        UnitCollider nearestCollider = MainCollider;

        float compare = Vector3.Distance(MainCollider.GetCenterPos(), comparePos);
        for (int i = 0; i < ListCollider.Count; i++)
        {
            if (!ListCollider[i].IsEnable())
                continue;

            float dist = Vector3.Distance(comparePos, ListCollider[i].GetCenterPos());
            if (dist < compare)
            {
                compare = dist;
                nearestCollider = ListCollider[i];
            }
        }

        return nearestCollider;
    }

    public Vector3 GetTargetCapsuleEdgePos(Unit target)
    {
        UnitCollider nearestTargetCollider = target.GetNearestColliderFromPos(transform.position);
        if (nearestTargetCollider == null)
            return Vector3.zero;

        Vector3 targetPos = nearestTargetCollider.GetCenterPos();
        targetPos.y = transform.position.y;

        Vector3 dirTargetToMine = (transform.position - targetPos).normalized;
        Vector3 targetCapsuleEdgePos = targetPos + (dirTargetToMine * nearestTargetCollider.radius);

        return targetCapsuleEdgePos;
    }

    public Vector3 GetTargetCapsuleEdgePos( Vector3 myPos, Unit target ) {
        UnitCollider nearestTargetCollider = target.GetNearestColliderFromPos( myPos );
        if ( nearestTargetCollider == null )
            return Vector3.zero;

        Vector3 targetPos = nearestTargetCollider.GetCenterPos();
        targetPos.y = transform.position.y;

        Vector3 dirTargetToMine = ( myPos - targetPos ).normalized;
        Vector3 targetCapsuleEdgePos = targetPos + ( dirTargetToMine * nearestTargetCollider.radius );

        return targetCapsuleEdgePos;
    }

    public Vector3 GetTargetCapsuleEdgePosForTeleport( Unit target ) {
		UnitCollider nearestTargetCollider = target.GetNearestColliderFromPos( transform.position );
		if ( nearestTargetCollider == null )
			return Vector3.zero;

		Vector3 targetPos = nearestTargetCollider.GetCenterPos();
		targetPos.y = transform.position.y;

		Vector3 dirTargetToMine = ( transform.position - targetPos ).normalized;
		Vector3 targetCapsuleEdgePos = targetPos + ( dirTargetToMine * nearestTargetCollider.radius ) + ( dirTargetToMine * MainCollider.radius );

		return targetCapsuleEdgePos;
	}

    public float GetDistance(Unit target)
    {
        UnitCollider nearestTargetCollider = target.GetNearestColliderFromPos(transform.position);
        if (nearestTargetCollider == null)
            return 0.0f;

        Vector3 nearestTargetColliderPos = nearestTargetCollider.GetCenterPos();
        Vector3 hitPoint = nearestTargetColliderPos;

        if (Physics.Raycast(transform.position, nearestTargetColliderPos - transform.position,
                           out RaycastHit hitInfo, Mathf.Infinity, 1 << target.gameObject.layer))
        {
            if (hitInfo.collider == nearestTargetCollider)
            {
                hitPoint = hitInfo.point;
            }
        }

        return Vector3.Distance(transform.position, hitPoint);
    }

	public virtual void Retarget() {
		SetMainTarget( null );

		if ( child ) {
			Enemy childEnemy = child as Enemy;
			if ( childEnemy ) {
				childEnemy.Retarget();
			}
		}
	}

    public virtual List<UnitCollider> GetTargetColliderList(bool onlyMainCollider = false)
    {
        return null;
    }

    public virtual List<UnitCollider> GetTargetColliderListByAround(Vector3 pos, float radius, bool onlyMainCollider = false)
    {
        return null;
    }

    public virtual UnitCollider GetMainTargetCollider(bool onlyEnemy, float checkDist = 0.0f, bool skipHasShieldTarget = false, bool onlyAir = false)//, bool checkSelfAir = true)
    {
        return null;
    }

    public virtual UnitCollider GetRandomTargetCollider(bool onlyEnemy = false)
    {
        return null;
    }

    public virtual UnitCollider GetRandomTargetColliderByAround(float radius)
    {
        return null;
    }

    /*public virtual List<Unit> GetTargetList()
    {
        return null;
    }

    public virtual List<Unit> GetTargetListByAround(float radius)
    {
        return null;
    }

    public virtual List<Unit> GetTargetListByAround(Vector3 pos, float radius)
    {
        return null;
    }

    public virtual Unit GetMainTarget(float checkDist = 0.0f, bool skipHasShieldTarget = false, bool onlyAir = false, bool checkSelfAir = true)
    {
        return null;
    }

    public virtual Unit GetRandomTarget()
    {
        return null;
    }

    public virtual Unit GetRandomTargetByAround(float radius)
    {
        return null;
    }*/

    public virtual List<Unit> GetEnemyList(bool onlyEnemy = false)
    {
        return null;
    }

    public virtual List<Unit> GetEnemyListByAbnormalAttr(EAttackAttr attr)
    {
        return null;
    }

    public virtual List<UnitCollider> GetEnemyColliderList()
    {
        return null;
    }

    public virtual List<UnitCollider> GetAllyColliderListByAround( Vector3 pos, float radius, bool onlyMainCollider = false ) {
        return null;
    }

    public void SortTargetColliderListByNearDistance(ref List<UnitCollider> listCollider)
    {
        // 거리 체크
        listCollider.Sort(delegate (UnitCollider lhs, UnitCollider rhs)
        {
            float lhsDist = Vector3.Distance(transform.position, lhs.GetCenterPos());
            float rhsDist = Vector3.Distance(transform.position, rhs.GetCenterPos());

            if (lhsDist > rhsDist)
                return 1;
            else if (lhsDist < rhsDist)
                return -1;

            return 0;
        });

        for (int i = 0; i < listCollider.Count; i++)
        {
            listCollider[i].CompareDistScore = (listCollider.Count - i) * 0.3f;
        }

        // 각도 체크
        listCollider.Sort(delegate (UnitCollider lhs, UnitCollider rhs)
        {
            lhs.CompareDistAngle = Vector3.Angle(transform.forward, (lhs.GetCenterPos() - transform.position).normalized);
            rhs.CompareDistAngle = Vector3.Angle(transform.forward, (rhs.GetCenterPos() - transform.position).normalized);

            if (lhs.CompareDistAngle > rhs.CompareDistAngle)
                return 1;
            else if (lhs.CompareDistAngle < rhs.CompareDistAngle)
                return -1;

            return 0;
        });

        for (int i = 0; i < listCollider.Count; i++)
        {
            float angle = listCollider[i].CompareDistAngle;
            listCollider[i].CompareDistScore += ((360.0f - angle) / 360.0f) * 0.7f;
        }

        // 거리와 각도로 정해진 targetScore로 최종 정렬
        listCollider.Sort(delegate (UnitCollider lhs, UnitCollider rhs)
        {
            if (lhs.CompareDistScore > rhs.CompareDistScore)
                return -1;
            else if (lhs.CompareDistScore < rhs.CompareDistScore)
                return 1;

            return 0;
        });
    }

    public void SortTargetListByNearDistance(ref List<Unit> listTarget)
    {
        // 거리 체크
        listTarget.Sort(delegate (Unit lhs, Unit rhs)
        {
            lhs.compareDist = Vector3.Distance(transform.position, lhs.transform.position);
            rhs.compareDist = Vector3.Distance(transform.position, rhs.transform.position);

            if (lhs.compareDist > rhs.compareDist)
                return 1;
            else if (lhs.compareDist < rhs.compareDist)
                return -1;

            return 0;
        });

        for (int i = 0; i < listTarget.Count; i++)
            listTarget[i].compareScore = (listTarget.Count - i) * 0.3f;

        // 각도 체크
        listTarget.Sort(delegate (Unit lhs, Unit rhs)
        {
            lhs.compareAngle = Vector3.Angle(transform.forward, (lhs.transform.position - transform.position).normalized);
            rhs.compareAngle = Vector3.Angle(transform.forward, (rhs.transform.position - transform.position).normalized);

            if (lhs.compareAngle > rhs.compareAngle)
                return 1;
            else if (lhs.compareAngle < rhs.compareAngle)
                return -1;

            return 0;
        });

        for (int i = 0; i < listTarget.Count; i++)
        {
            float angle = listTarget[i].compareAngle;
            listTarget[i].compareScore += ((360.0f - angle) / 360.0f) * 0.7f;
        }

        // 거리와 각도로 정해진 targetScore로 최종 정렬
        listTarget.Sort(delegate (Unit lhs, Unit rhs)
        {
            if (lhs.compareScore > rhs.compareScore)
                return -1;
            else if (lhs.compareScore < rhs.compareScore)
                return 1;

            return 0;
        });
    }

    public bool IsTargetInAttackRange(UnitCollider hitCollider, eAttackDirection atkDir, float angle, float atkRange)
    {
        if (hitCollider == null)
            return false;

        Vector3 pos = transform.position;
        pos.y = GetCenterPos().y;

        Vector3 targetPos = hitCollider.GetCenterPos();
        targetPos.y = pos.y;

        Vector3 dirToHitCollider = (targetPos - pos).normalized;

        if (atkDir == eAttackDirection.Front)
        {
            RaycastHit hitInfo;

            Vector3 end = hitCollider.GetCenterPos();
            if (!Physics.Raycast(pos, (end - pos).normalized, out hitInfo, atkRange, 1 << hitCollider.Owner.gameObject.layer))
            {
                return false;
            }
        }
        else if (atkDir == eAttackDirection.Back)
        {
            Vector3 v = Vector3.Normalize(targetPos - pos);
            float dot = Vector3.Dot(v, transform.forward);

            if (dot > 0.0f)
                return false;
        }
        else if (atkDir == eAttackDirection.Around)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, atkRange, 1 << hitCollider.Owner.gameObject.layer);
            if (cols.Length <= 0)
                return false;

            bool find = false;
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].gameObject == hitCollider.gameObject)
                {
                    find = true;
                    break;
                }
            }

            if (find == false)
                return find;
        }
        else if (atkDir == eAttackDirection.Angle)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, atkRange, 1 << hitCollider.Owner.gameObject.layer);
            if (cols.Length <= 0)
            {
                return false;
            }

            bool find = false;
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].gameObject == hitCollider.gameObject)
                {
                    find = true;
                    break;
                }
            }

            if (!find)
            {
                return false;
            }

            if (Vector3.Angle(transform.forward, dirToHitCollider) > angle * 0.5f)
            {
                return false;
            }
        }

        return true;
    }

    /*public bool IsTargetInAttackRange(Unit target, Vector3 dir, float atkRange)
    {
        if (target == null)
            return false;

        if (Mathf.Abs(target.transform.position.y - transform.position.y) > capsuleCol.height)
            return false;

        //float minDist = capsuleCol.radius + target.capsuleCol.radius;
        float dist = GetDistance(target);//Vector3.Distance(transform.position, target.transform.position);
        if (dist > atkRange)//Mathf.Max(minDist, atkRange))
            return false;

        Vector3 pos = transform.position;
        pos.y = GetCenterPos().y;

        RaycastHit hitInfo;
        if (!Physics.Raycast(pos, dir, out hitInfo, atkRange, 1 << target.gameObject.layer))
            return false;

        return true;
    }

    public bool IsTargetInAtkBox(Unit target, AniEvent.sEvent evt)
    {
        Vector3 bonePos = transform.position;
        Transform bone = aniEvent.GetBoneByName(evt.hitBoxBoneName);
        if (bone)
            bonePos = bone.transform.position;

        Vector3 pos = bonePos + (transform.rotation * evt.hitBoxPosition);

        Bounds bounds = new Bounds(pos, evt.hitBoxSize);
        return bounds.Contains(target.transform.position);
    }*/

    public virtual bool OnAttackOnEndAction(AniEvent.sEvent evt)
    {
        if (evt.effects != null && evt.effects.Length > 0)
            m_aniEvent.PlayEventParticles(evt.effects, true);

        if (evt.soundEffs != null && evt.soundEffs.Length > 0)
            m_aniEvent.PlaySound(evt.soundEffs);

        return OnAttack(evt, 1.0f, false);
    }

    protected float GetAttackPower(AniEvent.sEvent evt, ref bool isCritical)
    {
        float finalAttackPower = (m_attackPower * evt.atkRatio);
        return finalAttackPower;
    }

    public void SetAttackPower(float attackPower)
    {
        m_attackPower = attackPower;
    }

    public void SetSpeed(float speed)
    {
        m_originalSpeed = speed;
        m_curSpeed = speed;
    }

    public void AddPenetrate( float add ) {
        Penetrate += add;
	}

    public void AddSufferance( float add ) {
        Sufferance += add;
    }

    public void ResetPenetrate() {
        Penetrate = OriginalPenetrate;
    }

    private Coroutine mCrDrag = null;
	private void PlayDragged( Unit attacker, float speed, eDragDirection dragDir, float limitedDist ) {
		if ( IsImmunePulling() ) {
			return;
		}

		if ( m_actionSystem.IsCurrentAction( eActionCommand.Appear ) || curHp <= 0.0f ) {
			return;
		}

		m_actionSystem.CancelCurrentAction();

		StopStepForward();
		SetKinematicRigidBody();

		Vector3 edgePos = GetTargetCapsuleEdgePos( attacker );

		switch ( dragDir ) {
			case eDragDirection.FRONT: {
				edgePos += attacker.transform.forward;
			}
			break;

			case eDragDirection.BACK: {
				edgePos -= attacker.transform.forward;
			}
			break;

			case eDragDirection.LEFT: {
				edgePos -= attacker.transform.right;
			}
			break;

			case eDragDirection.RIGHT: {
				edgePos += attacker.transform.right;
			}
			break;

			default: {

			}
			break;
		}

		Utility.StopCoroutine( this, ref mCrDrag );
		if ( 0.0f < limitedDist ) {
			mCrDrag = StartCoroutine( UpdateDraggingForLimitedDistance( edgePos, speed, limitedDist ) );
		}
		else {
			mCrDrag = StartCoroutine( UpdateDragging( edgePos, speed ) );
		}
	}

	public virtual void Dragged( Unit attacker, float speed, eDragDirection dragDir ) {
		PlayDragged( attacker, speed, dragDir, float.MinValue );
	}

	public virtual void DraggedForLimitedDistance( Unit attacker, float speed, eDragDirection dragDir, float limitedDist ) {
		PlayDragged( attacker, speed, dragDir, limitedDist );
	}

	protected virtual void EndDragged() {
		SetGroundedRigidBody();
		m_rigidBody.velocity = Vector3.zero;
	}

	protected virtual IEnumerator UpdateDragging( Vector3 dest, float speed ) {
		bool end = false;
		Vector3 v = Vector3.zero;

		while ( !end ) {
			if ( Utility.GetDistanceWithoutY( dest, transform.position ) <= 0.3f ) {
				end = true;
			}
			else {
				if ( m_cmptMovement != null ) {
					v = dest - transform.position;
					m_cmptMovement.UpdatePosition( v, speed, false );
				}
				else {
					end = true;
				}
			}

			yield return mWaitForFixedUpdate;
		}

		EndDragged();
	}

	protected virtual IEnumerator UpdateDraggingForLimitedDistance( Vector3 dest, float speed, float limitedDist ) {
		bool end = false;
		Vector3 v = Vector3.zero;

		while ( !end ) {
			if ( Utility.GetDistanceWithoutY( dest, transform.position ) <= 0.3f ) {
				end = true;
			}
			else if ( Vector3.Distance( dest, transform.position ) <= limitedDist ) {
				end = true;
			}
			else {
                if ( m_cmptMovement != null ) {
					v = dest - transform.position;
					m_cmptMovement.UpdatePosition( v, speed, false );
				}
				else {
                    end = true;
                }
			}

			yield return mWaitForFixedUpdate;
		}

		EndDragged();
	}

	private Coroutine mCrPushed = null;
    public virtual void Pushed( Unit attacker, float time, float speed ) {
        if( IsImmunePulling() ) {
            return;
        }

        if( m_actionSystem.IsCurrentAction( eActionCommand.Appear ) || curHp <= 0.0f ) {
            return;
        }

        m_actionSystem.CancelCurrentAction();

        StopStepForward();
        SetKinematicRigidBody();

        Utility.StopCoroutine( this, ref mCrPushed );
        mCrPushed = StartCoroutine( UpdatePushing( attacker, time, speed ) );
    }

    protected virtual void EndPushed() {
        if( m_curHp > 0.0f ) {
            SetGroundedRigidBody();
        }

        Utility.StopCoroutine( this, ref mCrPushed );
        m_rigidBody.velocity = Vector3.zero;
    }

    protected virtual IEnumerator UpdatePushing( Unit attacker, float time, float speed ) {
        bool end = false;
        float checkTime = 0.0f;
        float t = 0.0f;
        Vector3 dir = Vector3.zero;
        Vector3 hitPoint = Vector3.zero;
        float radius = MainCollider ? MainCollider.radius : 0.3f;

        while( !end ) {
            checkTime += Time.deltaTime / time;
            t = Mathf.SmoothStep( 0.0f, 1.0f, checkTime );

            if( t >= 1.0f || curHp <= 0.0f ) {
                end = true;
			}
            else {
                dir = (transform.position - attacker.transform.position).normalized;
                dir.y = 0.01f;

                if( !IsCollisionWall( transform.position, transform.position + ( dir * radius ), ref hitPoint ) ) {
                    m_cmptMovement.UpdatePosition( dir, EasingFunction.EaseOutExpo( speed, 0.0f, t ), false );
                }
                else {
                    end = true;
				}
            }

            yield return mWaitForFixedUpdate;
        }

        EndPushed();
    }

	public virtual bool OnAttack( AniEvent.sEvent evt, float atkRatio, bool notAniEventAtk ) {
		if( !notAniEventAtk ) {
			m_onAttackAniEvent = evt;

			curSuccessAttack = false;
			curHitState = eHitState.Fail;
		}

		bool isCritical = false;
		float finalAttackPower = GetAttackPower(evt, ref isCritical) * atkRatio;
        if( !isCritical ) {
            isCritical = decideCritical;
        }

		m_atkDir = evt.atkDirection;
        if( m_actionSystem && m_actionSystem.IsCurrentUSkillAction() ) {
            m_atkDir = eAttackDirection.Skip;
        }

		// OnAttack을 직접 호출 하는 부분을 빼던가 아니면 eToExecuteType을 인자로 받아야함 (지금은 딱히 문제는 없는데...200116)
		AttackEvent atkEvent = new AttackEvent();
		atkEvent.Set( eEventType.EVENT_BATTLE_ON_MELEE_ATTACK, this, BattleOption.eToExecuteType.Unit, evt, finalAttackPower, m_atkDir, isCritical, 0,
					  EffectManager.eType.None, null, 0.0f, notAniEventAtk );

		EventMgr.Instance.SendEvent( atkEvent );
		return true;
	}

	public virtual void OnSuccessAttack( AniEvent.sEvent atkEvt, bool notAniEventAtk, int actionTableId, Projectile projectile, bool isCritical ) {
		m_onAttackAniEvent = atkEvt;

		if( !notAniEventAtk && atkEvt.startEvent == false && atkEvt.atkEndFrame > 0.0f )
			atkEvt.startEvent = true;

		if( !notAniEventAtk ) {
			ExecuteBattleOption( BattleOption.eBOTimingType.OnAttack, 0, projectile, false );

			if( isCritical ) {
				ExecuteBattleOption( BattleOption.eBOTimingType.OnCriAttack, 0, projectile, false );
			}

			bool isNormalAttackAction = true;
			if( actionTableId > 0 && projectile ) {
				ActionSelectSkillBase action = m_actionSystem.GetActionOrNullByTableId<ActionSelectSkillBase>(actionTableId);

				if( action ) {
					if( action as ActionComboAttack || action as ActionTargetingAttack ) {
						isNormalAttackAction = m_actionSystem.IsNormalAttackAction( action.actionCommand );
					}
					else {
						isNormalAttackAction = action.IsNormalAttack;
					}
				}
			}
			else {
				isNormalAttackAction = m_actionSystem.IsCurrentNormalAttackAction();
			}

			if( isNormalAttackAction ) {
				ExecuteBattleOption( BattleOption.eBOTimingType.OnNormalAttack, actionTableId, projectile, false );

				if( isCritical ) {
					ExecuteBattleOption( BattleOption.eBOTimingType.OnCriNormalAttack, actionTableId, projectile, false );
				}
			}
			else {
				ExecuteBattleOption( BattleOption.eBOTimingType.OnSkillAttack, actionTableId, projectile, false );

				if( isCritical ) {
					ExecuteBattleOption( BattleOption.eBOTimingType.OnCriSkillAttack, actionTableId, projectile, false );
				}
			}

			if ( projectile ) {
				projectile.ExecuteBattleOption( BattleOption.eBOTimingType.OnProjectileHit, actionTableId, projectile );
			}
		}
	}

	public virtual void OnOnlyDamageAttack( AniEvent.sEvent atkEvt, bool notAniEventAtk, int actionTableId, Projectile projectile, bool isCritical ) {
		m_onAttackAniEvent = atkEvt;

		if( !notAniEventAtk && atkEvt.startEvent == false && atkEvt.atkEndFrame > 0.0f )
			atkEvt.startEvent = true;

		if( !notAniEventAtk ) {
			ExecuteBattleOption( BattleOption.eBOTimingType.OnAttack, 0, projectile, false );

			if( isCritical ) {
				ExecuteBattleOption( BattleOption.eBOTimingType.OnCriAttack, 0, projectile, false );
			}

			bool isNormalAttackAction = true;
			if( actionTableId > 0 && projectile ) {
				ActionSelectSkillBase action = m_actionSystem.GetActionOrNullByTableId<ActionSelectSkillBase>(actionTableId);

				if( action as ActionComboAttack || action as ActionTargetingAttack ) {
					isNormalAttackAction = m_actionSystem.IsNormalAttackAction( action.actionCommand );
				}
				else {
					isNormalAttackAction = false;
				}
			}
			else {
				isNormalAttackAction = m_actionSystem.IsCurrentNormalAttackAction();
			}

			if( isNormalAttackAction ) {
				ExecuteBattleOption( BattleOption.eBOTimingType.OnNormalAttack, actionTableId, projectile, false );

				if( isCritical ) {
					ExecuteBattleOption( BattleOption.eBOTimingType.OnCriNormalAttack, actionTableId, projectile, false );
				}
			}
			else {
				ExecuteBattleOption( BattleOption.eBOTimingType.OnSkillAttack, actionTableId, projectile, false );

				if( isCritical ) {
					ExecuteBattleOption( BattleOption.eBOTimingType.OnCriSkillAttack, actionTableId, projectile, false );
				}
			}

			if ( projectile ) {
				projectile.ExecuteBattleOption( BattleOption.eBOTimingType.OnProjectileHit, actionTableId, projectile );
			}
		}
	}

	public virtual void ClearCombo()
    {
    }

	protected virtual bool OnFire( AniEvent.sEvent evt ) {
        if( evt == null || evt.projectiles == null || evt.projectiles.Length <= 0 ) {
            return false;
		}

		UnitCollider targetCollider = GetMainTargetCollider( false );
        if ( targetCollider == null ) {
            return false;
        }

		int actionTableId = -1;
		if ( m_actionSystem && m_actionSystem.currentAction ) {
			actionTableId = m_actionSystem.currentAction.TableId;
		}

		int fireCount = 0;
		ActionBase currentAction = null;

		for ( int i = 0; i < evt.projectileSize + Utility.ADDED_PROJECTILE_COUNT; i++ ) {
            if( i >= evt.projectiles.Length ) {
                break;
			}

            if ( evt.projectiles[i].projectile.IsActivate() || evt.projectiles[i].projectile == null ) {
                continue;
            }

			currentAction = null;
			if ( m_actionSystem ) {
				currentAction = m_actionSystem.currentAction;
			}

			evt.projectiles[i].projectile.Fire( this, BattleOption.eToExecuteType.Unit, evt, evt.projectiles[i], targetCollider.Owner, actionTableId,
											   null, currentAction );

			++fireCount;
            if ( fireCount >= evt.projectileSize ) {
                break;
            }
		}

		if ( fireCount == 0 ) {
			currentAction = null;

			if ( m_actionSystem ) {
				currentAction = m_actionSystem.currentAction;
			}

			evt.projectiles[0].projectile.Fire( this, BattleOption.eToExecuteType.Unit, evt, evt.projectiles[0], targetCollider.Owner, actionTableId,
				null, currentAction );
		}

		return true;
	}

	public virtual void OnProjectileHit( Projectile projectile, UnitCollider targetCollider, bool notAniEventAtk ) {
		bool isCritical = false;
		float finalAtkPower = GetAttackPower( projectile.evt, ref isCritical );

		if ( projectile.ProjectileAtkAttr == Projectile.EProjectileAtkAttr.KNOCKBACK ) {
			projectile.evt.behaviour = eBehaviour.KnockBackAttack;
		}
		else if ( projectile.ProjectileAtkAttr == Projectile.EProjectileAtkAttr.DOWN ) {
			projectile.evt.behaviour = eBehaviour.DownAttack;
		}
		else if ( projectile.ProjectileAtkAttr == Projectile.EProjectileAtkAttr.UPPER ) {
			projectile.evt.behaviour = eBehaviour.UpperAttack;
		}
		else if ( projectile.ProjectileAtkAttr == Projectile.EProjectileAtkAttr.STUN ) {
			projectile.evt.behaviour = eBehaviour.StunAttack;
		}
		else if ( projectile.ProjectileAtkAttr == Projectile.EProjectileAtkAttr.GROGGY ) {
			projectile.evt.behaviour = eBehaviour.GroggyAttack;
		}

		projectile.evt.atkAttr = projectile.AttackAttr;
		projectile.evt.isRangeAttack = !projectile.IsMelee;

		Unit attacker = this;
		DroneUnit drone = this as DroneUnit;
		if ( drone && drone.DroneType == DroneUnit.eDroneType.ByCharacter ) {
			attacker = drone.Owner;
		}

		AttackEvent atkEvent = new AttackEvent();
		atkEvent.SetWithSingleTarget( eEventType.EVENT_BATTLE_ON_PROJECTILE_ATTACK, attacker, projectile.ToExecuteType, projectile.evt, finalAtkPower,
									  m_atkDir, isCritical, 0, EffectManager.eType.None, targetCollider, 0.0f, notAniEventAtk, false, false, 0, projectile );

		EventMgr.Instance.SendEvent( atkEvent );

		List<Unit> listMarkingEnemy = World.Instance.EnemyMgr.GetMarkingEnemies( eMarkingType.ConnectingAttack );
		for ( int i = 0; i < listMarkingEnemy.Count; i++ ) {
			Unit enemy = listMarkingEnemy[i];
			if ( enemy == null || enemy == targetCollider.Owner ) {
				continue;
			}

			AttackEvent atkEvent2 = new AttackEvent();
			atkEvent2.SetWithSingleTarget( eEventType.EVENT_BATTLE_ON_PROJECTILE_ATTACK, attacker, projectile.ToExecuteType, projectile.evt, finalAtkPower,
									   m_atkDir, isCritical, 0, EffectManager.eType.None, enemy.MainCollider, 0.0f, notAniEventAtk );

			EventMgr.Instance.SendEvent( atkEvent2 );
		}

		projectile.evt.behaviour = eBehaviour.Projectile;
	}

	public virtual void SetMainTarget(Unit target)
    {
        if (m_mainTarget != null)
            m_lastMainTarget = m_mainTarget;

        m_mainTarget = target;
    }

    public void AddHitTarget( Unit target ) {
        for ( int i = 0; i < m_listHitTarget.Count; i++ ) {
            if( m_listHitTarget[i] == target ) {
                return;
			}
		}

        bool isEnemyLayer = Utility.IsEnemyLayer( (eLayer)gameObject.layer, (eLayer)target.gameObject.layer );
        if ( isEnemyLayer && target.curHp > 0.0f && target.IsActivate() ) {
            m_listHitTarget.Add( target );
		}
    }

	public void AddHitTargetList( List<Unit> hitTargetList, bool skipClear = false ) {
        if ( !skipClear ) {
            m_listHitTarget.Clear();
        }

		for ( int i = 0; i < hitTargetList.Count; i++ ) {
            Unit find = m_listHitTarget.Find( x => x == hitTargetList[i] );
            if( find ) {
                continue;
			}

            bool isEnemyLayer = Utility.IsEnemyLayer( (eLayer)gameObject.layer, (eLayer)hitTargetList[i].gameObject.layer );

            if ( isEnemyLayer && hitTargetList[i].curHp > 0.0f && hitTargetList[i].IsActivate() ) {
                m_listHitTarget.Add( hitTargetList[i] );
            }
		}
	}

	public void RemoveHitTarget( Unit target ) {
		Unit find = m_listHitTarget.Find( x => x == target );
		if ( find == null ) {
			return;
		}

		m_listHitTarget.Remove( target );
	}

	public void SetMarkingType(eMarkingType type, float value1, float value2 = 0.0f, float value3 = 0.0f, int effId = 0)
    {
        MarkingInfo.MarkingType = type;
        MarkingInfo.Value1 = value1;
		MarkingInfo.Value2 = value2;
		MarkingInfo.Value3 = value3;
		MarkingInfo.EffId = effId;

		if (type == eMarkingType.ConnectingAttack)
        {
            MarkingInfo.DmgRatio = value1;
        }
        else
        {
            MarkingInfo.DmgRatio = 1.0f;
        }
    }

    /*public bool IsAllFloatingTargetDead()
    {
        if (m_listHitTarget.Count <= 0)
            return true;

        for(int i = 0; i < m_listHitTarget.Count; i++)
        {
            ActionHitFloat action = m_listHitTarget[i].actionSystem.GetCurrentAction<ActionHitFloat>();
            if (action == null)
                continue;

            if (m_listHitTarget[i].curHp > 0.0f)
                return false;
        }

        return true;
    }*/

    public virtual bool IsImmuneFloat()
    {
        return false;
    }

    public virtual bool IsImmuneDown()
    {
        return false;
    }

    public virtual bool IsImmuneKnockback()
    {
        return false;
    }

    public virtual bool IsImmunePulling()
    {
        return false;
    }

    public void AddDebuffImmuneType( eDebuffImmuneType_f debuffImmuneType ) {
        DebuffImmuneFlag |= debuffImmuneType;
	}

    public void RemoveDebuffImmuneType( eDebuffImmuneType_f debuffImmuneType ) {
        DebuffImmuneFlag ^= debuffImmuneType;
    }

    public bool HasDebuffImmuneType( eDebuffImmuneType_f debuffImmuneType ) {
        return ( ( DebuffImmuneFlag & debuffImmuneType ) == debuffImmuneType );
    }

    public virtual eKnockBackType GetKnockBackType()
    {
        return eKnockBackType.Normal;
    }

    /// <summary>
    /// 대미지 계산
    /// </summary>
    /// <param name="attacker">공격자</param>
    /// <param name="toExecuteType">실행 타입</param>
    /// <param name="damage">공격자의 기본 공격력 * 애니이벤트에 설정된 공격 배율</param>
    /// <param name="isCritical">크리티컬 여부</param>
    /// <param name="isRangeAttack">원거리 공격 여부</param>
    /// <param name="skipCritical">크리티컬 무시 여부</param>
    /// <param name="projectile">발사체</param>
    /// <param name="isUltimateSkill">오의인지 여부</param>
    /// <returns></returns>    
    public float CalcDamage( Unit attacker, BattleOption.eToExecuteType toExecuteType, ObscuredFloat damage, ref bool isCritical, bool isRangeAttack,
							 bool skipCritical, Projectile projectile, bool isUltimateSkill ) {
		if( attacker.isClone ) {
			attacker = attacker.cloneOwner;
		}

		attacker.LastProjectile = projectile;

        float finalDamage = attacker.unit2ndStats.CalcAttackPower( attacker, this, damage ); // 2차 스탯에 의한 공격자의 데미지 증가 계산
        finalDamage = m_2ndStats.CalcDamageDown( attacker, this, finalDamage ); // 2차 스탯에 의한 피격자의 데미지 감소 계산

        // 현재 액션이 선택 스킬이면 
        if( toExecuteType == BattleOption.eToExecuteType.Unit && attacker.actionSystem ) {
			ActionSelectSkillBase actionSelectSkill = attacker.actionSystem.GetCurrentAction<ActionSelectSkillBase>();
			if( projectile ) {
				actionSelectSkill = attacker.actionSystem.GetActionOrNullByTableId<ActionSelectSkillBase>( projectile.OwnerActionTableId );
			}

			if( actionSelectSkill && actionSelectSkill.MaxCoolTime > 0.0f ) {
				finalDamage += ( finalDamage * attacker.IncreaseSkillAtkValue ); // 캐릭터 패시브 스킬에 의한 공격자의 스킬 데미지 증가 계산
                finalDamage += ( finalDamage * GameInfo.Instance.BattleConfig.CharSkillDmgRatio ); // 캐릭터 스킬 데미지 가중치 계산
            }
            else {
				// 현재 액션이 가디언 스킬이면
				Player player = attacker as Player;
                if ( player && player.Guardian ) {
					ActionGuardianBase actionGuardianBase = player.Guardian.actionSystem.GetCurrentAction<ActionGuardianBase>();
                    if ( actionGuardianBase && actionGuardianBase.GetMaxCoolTime() > 0.0f ) {
						finalDamage += ( finalDamage * attacker.IncreaseSkillAtkValue ); // 캐릭터 패시브 스킬에 의한 공격자의 스킬 데미지 증가 계산
						finalDamage += ( finalDamage * GameInfo.Instance.BattleConfig.CharSkillDmgRatio ); // 캐릭터 스킬 데미지 가중치 계산
					}
				}
            }
		}

        attacker.LastNormalAttackPower = finalDamage;

        bool isNormalAttck = attacker.actionSystem ? attacker.actionSystem.IsCurrentNormalAttackAction() : true; // 일반 공격인지 체크
		bool isWeaponSkill = attacker.actionSystem ? attacker.actionSystem.IsCurrentWeaponSkillAction() : false; // 무기 스킬인지 체크

		if( !isWeaponSkill && projectile ) {
			ActionWeaponSkillBase actionWpn = projectile.OwnerAction as ActionWeaponSkillBase;
			if( actionWpn ) {
				isWeaponSkill = true;
			}
		}

		if( attacker.unitBuffStats != null ) {
			// 공격자의 공격력 증가 효과에 의한 데미지 계산
			finalDamage = attacker.unitBuffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.AttackPower, UnitBuffStats.eIncreaseType.Increase,
															   finalDamage, isRangeAttack, isNormalAttck, isUltimateSkill, isWeaponSkill, this );

            // 공격자의 공격력 감소 효과에 의한 데미지 계산
            finalDamage = attacker.unitBuffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.AttackPower, UnitBuffStats.eIncreaseType.Decrease,
															   finalDamage, isRangeAttack, isNormalAttck, isUltimateSkill, isWeaponSkill, this );
        }

		// 피격자의 받는 데미지 증가(방어력 감소) 효과에 의한 데미지 계산
		finalDamage = m_buffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.Damage, UnitBuffStats.eIncreaseType.Increase, finalDamage,
											    isRangeAttack, isNormalAttck, isUltimateSkill, isWeaponSkill, attacker );

        // 피격자의 받는 데미지 감소(방어력 증가) 효과에 의한 데미지 계산
        finalDamage = m_buffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.Damage, UnitBuffStats.eIncreaseType.Decrease, finalDamage,
											    isRangeAttack, isNormalAttck, isUltimateSkill, isWeaponSkill, attacker );

        isCritical = attacker.decideCritical;
		if( !skipCritical && !isCritical ) {
            // 2차 스탯에 의한 공격자의 치명 확률 계산
            float criticalRate = attacker.unit2ndStats.CalcCriticalProbability( attacker.criticalRate - mCriticalResist ); 

            if( attacker.unitBuffStats != null ) {
				// 공격자의 치명 확률 증가 효과에 의한 치명 확률 계산
				criticalRate = attacker.unitBuffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.CriticalRate, 
                                                                    UnitBuffStats.eIncreaseType.Increase, criticalRate, isRangeAttack, isNormalAttck, 
                                                                    isUltimateSkill, isWeaponSkill, this );

				// 공격자의 치명 확률 감소 효과에 의한 치명 확률 계산
				criticalRate = attacker.unitBuffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.CriticalRate, 
                                                                    UnitBuffStats.eIncreaseType.Decrease, criticalRate, isRangeAttack, isNormalAttck, 
                                                                    isUltimateSkill, isWeaponSkill, this );
			}

			// 피격자의 치명 확률 증가 효과에 의한 치명 확률 계산
			criticalRate = m_buffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.CriticalResist, UnitBuffStats.eIncreaseType.Increase, 
                                                     criticalRate, isRangeAttack, isNormalAttck, isUltimateSkill, isWeaponSkill, attacker );

			// 피격자의 치명 확률 감소 효과에 의한 치명 확률 계산
			criticalRate = m_buffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.CriticalResist, UnitBuffStats.eIncreaseType.Decrease, 
                                                     criticalRate, isRangeAttack, isNormalAttck, isUltimateSkill, isWeaponSkill, attacker );

            // 크리티컬 확률 계산
            if( UnityEngine.Random.Range( 0, (int)eCOUNT.MAX_PROBABILITY ) < ( criticalRate * attacker.fixCriticalRate ) ) {
                isCritical = true;
            }
		}

    #if UNITY_EDITOR
		if( World.Instance.TestScene ) {
			isCritical = World.Instance.TestScene.IsAlwaysCritical;
		}
#endif

        // 2차 스탯에 의한 공격자의 치명 피해 계산
        if( isCritical ) { 
			finalDamage *= attacker.unit2ndStats.CalcCriticalRatio( attacker.mAddCriticalDmgRate - mCriticalDmgDef );

            if( attacker.unitBuffStats != null ) {
				// 공격자의 치명 데미지 증가 효과에 의한 데미지 계산
				finalDamage = attacker.unitBuffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.CriticalDmg, 
                                                                   UnitBuffStats.eIncreaseType.Increase, finalDamage, isRangeAttack, isNormalAttck, 
                                                                   isUltimateSkill, isWeaponSkill, this );

                // 공격자의 치명 데미지 감소 효과에 의한 데미지 계산
                finalDamage = attacker.unitBuffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.CriticalDmg, 
                                                                   UnitBuffStats.eIncreaseType.Decrease, finalDamage, isRangeAttack, isNormalAttck, 
                                                                   isUltimateSkill, isWeaponSkill, this );
            }

			// 피격자의 치명 데미지 증가 효과에 의한 데미지 계산
			finalDamage = m_buffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.CriticalDef, UnitBuffStats.eIncreaseType.Increase, finalDamage,
												    isRangeAttack, isNormalAttck, isUltimateSkill, isWeaponSkill, attacker );

            // 피격자의 치명 데미지 감소 효과에 의한 데미지 계산
            finalDamage = m_buffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.CriticalDef, UnitBuffStats.eIncreaseType.Decrease, finalDamage,
												    isRangeAttack, isNormalAttck, isUltimateSkill, isWeaponSkill, attacker );
        }

		// 랜덤 데미지(고정 데미지형 공격이 생긴다면 아래의 랜덤 데미지 계산은 들어가지 않도록 처리)
		finalDamage *= UnityEngine.Random.Range( GameInfo.Instance.BattleConfig.MinDmgRndRatio, GameInfo.Instance.BattleConfig.MaxDmgRndRatio );

        // 방어율, 공격자 관통력 적용
        float d = m_defenceRate / (float)eCOUNT.MAX_PROBABILITY;
		finalDamage -= ( finalDamage * Mathf.Min( 1.0f, d * ( Mathf.Max( 0.0f, 1.0f - attacker.Penetrate ) ) ) );

        // 데미지 최소값 체크
        if( finalDamage < 0.0f ) {
            finalDamage = 0.0f;
        }

        // 인내력 체크
        if( UnityEngine.Random.Range( 0, (int)eCOUNT.MAX_PROBABILITY ) < ( Mathf.Min( GameInfo.Instance.BattleConfig.MaxSufferance, ( Sufferance * 100.0f ) ) * 100.0f ) ) {
            finalDamage = 0.0f;
        }

        attacker.lastAttackPower = finalDamage;
		return finalDamage;
	}

	/// <summary>
    /// 히트 처리
    /// </summary>
    /// <returns>쉴드가 깨졌는지 여부</returns>
	public virtual bool OnHit( Unit attacker, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent attackerAniEvt, ObscuredFloat damage,
							   ref bool isCritical, ref eHitState hitState, Projectile projectile, bool isUltimateSkill, bool skipMaxDamageRecord ) {
		IsHit = false;

        if ( ignoreHit || !IsActivate() || m_curHp <= 0.0f ) {
            return false;
        }

		PlayerGuardian playerGuardian = attacker as PlayerGuardian;
		if ( playerGuardian != null && playerGuardian.OwnerPlayer != null ) {
			attacker = playerGuardian.OwnerPlayer;
		}

		if ( m_parent ) {
			m_parent.OnHit( attacker, toExecuteType, attackerAniEvt, damage, ref isCritical, ref hitState, projectile, isUltimateSkill, skipMaxDamageRecord );
			return false;
		}

		// 1-3 스킬 사용 튜토리얼에선 캐릭터 스킬 공격만 데미지 들어감
		if ( GameSupport.IsInGameTutorial() && 
             GameInfo.Instance.UserData.GetTutorialState() == (int)eTutorialState.TUTORIAL_STATE_Stage3Clear && 
             GameInfo.Instance.UserData.GetTutorialStep() == 1 ) {
			// 일반 공격이나 서포터 스킬인 경우 대미지 안들어가도록 처리
			if ( ( gameObject.layer == (int)eLayer.Enemy || gameObject.layer == (int)eLayer.EnemyGate ) &&
				( attacker.actionSystem.IsCurrentNormalAttackAction() || toExecuteType == BattleOption.eToExecuteType.Supporter ) ) {
				damage = 0.0f;
			}
		}

		if ( MarkingInfo.MarkingType == eMarkingType.ConnectingAttack ) {
			damage *= MarkingInfo.DmgRatio;
		}

		bool onlyEffect = false;
		if ( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
			onlyEffect = CurrentSuperArmor == eSuperArmor.Invincible;
		}
		else {
			onlyEffect = !isUltimateSkill && CurrentSuperArmor == eSuperArmor.Invincible;
		}

		// 무적이면 히트 이펙트만 나옴
		if ( onlyEffect ) {
			hitState = eHitState.OnlyEffect;
		}

		// 공격 속성 무시하는 옵션이 있는지 검사해서 있으면 히트 이펙트만 나옴
		EAttackAttr atkAttr = ListImmuneAtkAttr.Find( x => x == attackerAniEvt.atkAttr );
		if ( ( !isUltimateSkill && atkAttr != EAttackAttr.NONE ) ) {
			hitState = eHitState.OnlyEffect;
		}

        if ( m_grade != eGrade.Boss && isVisible && mUIHpBar && !mUIHpBar.gameObject.activeSelf ) {
            mUIHpBar.gameObject.SetActive( true );
        }

		ShowMesh( true );

		m_attacker = attacker;
		m_attackerAniEvent = attackerAniEvt;

		if ( attacker.actionSystem && attacker.actionSystem.currentAction ) {
			AttackerActionOnHit = attacker.actionSystem.GetCurrentAction<ActionBase>();
		}

		bool removeHitTarget = false;

		bool breakShield = false;
		if ( hitState != eHitState.OnlyEffect ) {
			if ( hitState != eHitState.OnlyDamage && TemporaryIgnoreHitAni != ignoreHitAniCond_t.NONE ) {
				if ( TemporaryIgnoreHitAni == ignoreHitAniCond_t.MELEE_ATTACK && !attackerAniEvt.isRangeAttack && projectile == null ) {
					hitState = eHitState.OnlyDamage;
				}
				else if ( TemporaryIgnoreHitAni == ignoreHitAniCond_t.RANGE_ATTACK && ( attackerAniEvt.isRangeAttack || projectile ) ) {
					hitState = eHitState.OnlyDamage;
				}
				else if ( TemporaryIgnoreHitAni == ignoreHitAniCond_t.ALL ) {
					hitState = eHitState.OnlyDamage;
				}
			}

			ExecuteBattleOption( BattleOption.eBOTimingType.OnHit, 0, null );

			bool dmgRateUpOnce = false;
			// 히트 시 빙결 상태 해제 후 첫 히트는 받는 데미지 30% 증가
			if ( m_cmptBuffDebuff && m_cmptBuffDebuff.HasDebuff( eEventType.EVENT_DEBUFF_FREEZE ) ) {
				m_cmptBuffDebuff.RemoveDebuff( eEventType.EVENT_DEBUFF_FREEZE );

				BuffEvent buffEvent = new BuffEvent();
				buffEvent.Set( 9999999, eEventSubject.MainTarget, eEventType.EVENT_DEBUFF_DMG_RATE_UP, attacker, 0.3f, 0.0f, 0.0f, 1.0f, 0.0f, 0, 0, eBuffIconType.None );
				buffEvent.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;

				EventMgr.Instance.SendEvent( buffEvent );
				dmgRateUpOnce = true;
			}

			if ( attackerAniEvt.atkAttr == EAttackAttr.Freeze ) {
				if ( attackerAniEvt.AtkAttrValue1 > 0.0f ) {
					BuffEvent buffEvent = new BuffEvent();

					buffEvent.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
					buffEvent.battleOptionData.effType = (int)EffectManager.eType.None;

					buffEvent.Set( 0, eEventSubject.Self, eEventType.EVENT_DEBUFF_FREEZE, this, 0.0f, 0.0f, 0.0f, attackerAniEvt.AtkAttrValue1, 0.0f,
								  buffEvent.effId, buffEvent.effId2, eBuffIconType.Debuff_Freeze );

					EventMgr.Instance.SendEvent( buffEvent );
				}
			}

			// 공격자와 피격자의 2차스탯, 전투옵션에 기반해 최종 데미지 계산
			bool calcCritical = false;
			ObscuredFloat finalDamage = damage;

			if ( !attackerAniEvt.SkipCalcDamage ) {
				finalDamage = CalcDamage( attacker, toExecuteType, damage, ref calcCritical, attackerAniEvt.isRangeAttack, false, projectile, isUltimateSkill );
			}

			if ( !skipMaxDamageRecord && finalDamage > attacker.MaxAttackPower ) {
				attacker.MaxAttackPower = finalDamage;
			}

            attacker.UpdateDamagePerSecond( finalDamage );
            Damaged += finalDamage;

            if ( dmgRateUpOnce ) { // 빙결 상태 해제 시 받는 데미지 30% 증가 디버프는 일회용
                m_cmptBuffDebuff.RemoveDebuffById( 9999999 );
			}

			isCritical = calcCritical || isCritical;
			if ( isCritical ) {
				World.Instance.InGameCamera.PlayShake( this, 0.2f, GameInfo.Instance.BattleConfig.ShakePower, 2.0f, 0.2f );
			}

			bool isAlreadyBreakShield = m_curShield <= 0.0f;

			if ( World.Instance.StageType != eSTAGETYPE.STAGE_TRAINING ) {
				float finalDamageToShield = finalDamage;
				if ( attacker.unitBuffStats != null ) {
					bool isWeaponSkill = attacker.actionSystem.IsCurrentWeaponSkillAction(); // 무기 스킬인지 체크
					if ( !isWeaponSkill && projectile ) {
						ActionWeaponSkillBase actionWpn = projectile.OwnerAction as ActionWeaponSkillBase;
						if ( actionWpn ) {
							isWeaponSkill = true;
						}
					}

					// 공격자의 쉴드에 주는 데미지 증가 버프에 의한 공격력 계산
					finalDamageToShield = attacker.unitBuffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.AttackPowerToShield,
																			  UnitBuffStats.eIncreaseType.Increase, finalDamageToShield,
																			  attackerAniEvt.isRangeAttack, attacker.actionSystem.IsCurrentNormalAttackAction(),
																			  isUltimateSkill, isWeaponSkill, this );
					// 공격자의 쉴드에 주는 데미지 감소 버프에 의한 공격력 계산
					finalDamageToShield = attacker.unitBuffStats.CalcBuffStat( toExecuteType, UnitBuffStats.eBuffStatType.AttackPowerToShield,
																			  UnitBuffStats.eIncreaseType.Decrease, finalDamageToShield,
																			  attackerAniEvt.isRangeAttack, attacker.actionSystem.IsCurrentNormalAttackAction(),
																			  isUltimateSkill, isWeaponSkill, this );
					// 데미지 최소값 체크
					if ( finalDamageToShield < 0.0f )
						finalDamageToShield = 0.0f;

					if ( attacker.tableId == 5 ) {
						Debug.Log( "**************************************************" );
						Debug.Log( "* 기본 데미지 : " + finalDamage );
						Debug.Log( "* 쉴드에 주는 데미지 : " + finalDamageToShield );
						Debug.Log( "**************************************************" );
					}
				}

				// 쉴드에 대한 데미지 증가로만 사용, 쉴드가 파괴된 대상에 대한 데미지 증가는 별도 기능으로 분리
				//if (m_curShield <= 0.0f && m_maxShield > 0.0f)
				//    finalDamage = finalDamageToShield; // 쉴드가 파괴 됐을땐 기본 공격에도 쉴드 데미지 증가 버프 적용

				m_curShield = Mathf.Max( 0.0f, m_curShield - finalDamageToShield );
				SubHp( finalDamage, isCritical );

				IsHit = true;
				Invoke( "EndHit", 5.0f / Application.targetFrameRate );
			}

			if ( m_curHp < 1.0f ) {
				if ( GameSupport.IsInGameTutorial() && gameObject.layer == (int)eLayer.Player ) { // 튜토리얼에선 캐릭터 사망 없음
					m_curHp = 1.0f;
				}
				else {
					m_curHp = 0.0f;
				}
			}

			if ( lookTargetOnHit == true && !m_actionSystem.IsCurrentNormalAttackAction() && attacker.gameObject.layer != (int)eLayer.PlayerClone &&
				attacker.curHp > 0.0f ) {
				LookAtTarget( attacker.transform.position );
			}

			if ( MarkingInfo.MarkingType == eMarkingType.GiveHp ) {
				BuffEvent buffEvent = new BuffEvent();
				buffEvent.Set( 9999998, eEventSubject.Self, eEventType.EVENT_BUFF_HEAL_ABS, attacker, finalDamage * MarkingInfo.Value1, 0.0f, 0.0f,
							  0.0f, 0.0f, 0, 0, eBuffIconType.None );
				buffEvent.battleOptionData.buffDebuffType = eBuffDebuffType.Buff;

				if ( MarkingInfo.Value2 <= 0.0f ) {
					EventMgr.Instance.SendEvent( buffEvent );
				}
				else {
					float delay = EffectManager.Instance.Play( this, attacker, MarkingInfo.EffId, EffectManager.eType.Each_Monster_Normal_Hit,
															  30030, EffectManager.eType.Common );
					StartCoroutine( DelayedGiveHp( buffEvent, delay * 0.9f ) );
				}
			}

			LastDamage = finalDamage;

			if ( m_curHp <= 0.0f && !m_lockDie ) {
				hitState = eHitState.Success;
				return false;
			}
			else if ( hitState == eHitState.Success ) {
				if ( ConditionalSuperArmor.conditionType != ESuperArmorConditionType.NONE ) {
					eSuperArmor condtionalSuperArmor = eSuperArmor.None;
					switch ( ConditionalSuperArmor.conditionType ) {
						case ESuperArmorConditionType.GRADE:
							if ( attacker.grade == (eGrade)ConditionalSuperArmor.compareValue ) {
								condtionalSuperArmor = (eSuperArmor)ConditionalSuperArmor.superArmor;
							}
							break;

						case ESuperArmorConditionType.MON_TYPE:
							if ( attacker.monType == (eMonType)ConditionalSuperArmor.compareValue ) {
								condtionalSuperArmor = (eSuperArmor)ConditionalSuperArmor.superArmor;
							}
							break;
					}

					if ( condtionalSuperArmor >= eSuperArmor.Lv1 ) {
						hitState = eHitState.OnlyDamage;
					}
				}
			}
			else if ( hitState == eHitState.OnlyDamage ) {
				if ( !attackerAniEvt.isRangeAttack && projectile == null ) {
					attacker.StartPauseFrame( false, attackerAniEvt.pauseFrame, true );
				}
			}

			if ( !isAlreadyBreakShield && m_maxShield > 0.0f && m_curShield <= 0.0f ) {
				BreakShield();
				breakShield = true;
			}

			if ( ( hitState == eHitState.OnlyDamage || m_aniEvent == null ) && m_curHp > 0.0f ) {
				if ( attackerAniEvt.hitEffectId > 0 ) {
					EffectManager.Instance.Play( this, attackerAniEvt.hitEffectId, EffectManager.eType.Each_Monster_DamageOnly_Hit );
				}

				Shake( true );
				removeHitTarget = true;
			}

			if ( attackerAniEvt.IsDragAtk ) {
				Dragged( attacker, attackerAniEvt.DragAttackSpeed, attackerAniEvt.DragDirection );
			}

			if ( hitState == eHitState.Success )
				attacker.curSuccessAttack = true;

			if ( hitState == eHitState.Success ) {
				ExecuteBattleOption( BattleOption.eBOTimingType.OnHitAction, 0, null );
			}
		}
		else { // OnlyEffect
            LastDamage = 0.0f;

			if ( attacker ) {
				attacker.lastAttackPower = 0.0f;
			}

			if ( !attackerAniEvt.isRangeAttack && projectile == null ) {
				attacker.StartPauseFrame( false, attackerAniEvt.pauseFrame, true );
			}
		}

		if ( CurrentSuperArmor < eSuperArmor.Lv1 || CurrentSuperArmor == eSuperArmor.Invincible ) {
			if ( m_aniEvent ) {
				Action callback = null;
				if ( hitState == eHitState.OnlyEffect ) {
					callback = SendRemoveHitTarget;
				}

				m_aniEvent.SetFlash( rimColorOnHit, rimMinOnHit, rimMaxOnHit, 9.0f / Application.targetFrameRate, callback );

				if ( m_child ) {
					m_child.m_aniEvent.SetFlash( rimColorOnHit, rimMinOnHit, rimMaxOnHit, 9.0f / Application.targetFrameRate, callback );
				}
			}
		}
		else if ( !removeHitTarget ) {
            SendRemoveHitTarget();
		}

		PlayHitEffect();
		return breakShield;
	}

	private void EndHit() {
        IsHit = false;
    }

	private IEnumerator DelayedGiveHp(BaseEvent evt, float delay)
	{
		yield return new WaitForSeconds(delay);
		EventMgr.Instance.SendEvent(evt);
	}

	protected void SendRemoveHitTarget() {
		EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_REMOVE_HIT_TARGET, this );
	}

	public virtual void BreakShield()
    {
        m_curShield = 0.0f;

        //UnlockSuperArmorValue();
        ForceSetSuperArmor(eSuperArmor.None);

		Utility.StopCoroutine(this, ref m_crRestoreShield);
		m_crRestoreShield = StartCoroutine(CheckRestoreShild());

        if (m_psShieldBreak != null)
            PlayShieldEffect(m_psShieldBreak);

        if (charType == eCharacterType.Character)
            SoundManager.Instance.PlaySnd(SoundManager.eSoundType.Player, "ArmorBreak");
        else if (charType == eCharacterType.Monster)
            SoundManager.Instance.PlaySnd(SoundManager.eSoundType.Monster, "ArmorBreak");
    }

    protected IEnumerator CheckRestoreShild()
    {
        float time = 0.0f;
        bool end = false;

        float waitTime = 0.0f;
        waitTime = stunDuration;

        while (end == false)
        {
            if (time < waitTime)
                time += Time.deltaTime;
            else
            {
                end = true;
            }

            yield return null;
        }

        m_crRestoreShield = null;
        ChargeShield();
    }

    protected virtual void UpdateShieldGauge()
    {
    }

    protected virtual void StunAfterShieldBreak()
    {
    }

    private void ChargeShield()
    {
        m_curShield = m_maxShield;
        ForceSetSuperArmor(eSuperArmor.Lv2);

        UpdateShieldGauge();
        PlayShieldEffect(m_psShieldOnHit);
    }

    public void ChargeShieldImmediate()
    {
        if (m_maxShield <= 0.0f)
            return;

        Utility.StopCoroutine(this, ref m_crRestoreShield);
        ChargeShield();
    }

    protected virtual bool OnChangeColor(float duration, Color startColor, Color endColor)
    {
        return true;
    }

    protected bool OnJump(float jumpPower, bool dirJump, bool fastFall)
    {
        m_cmptJump.StartJump(jumpPower);
        StartCoroutine(UpdateJump(dirJump, fastFall));

        return true;
    }

	protected virtual bool OnUseCameraSetting( Vector3 distance, Vector2 lookAt, bool lookAtPlayerBack, float smoothTimeRatio, float FOV, bool keepCameraSetting ) {
		if( World.Instance.InGameCamera.Mode != InGameCamera.EMode.DEFAULT || gameObject.layer != (int)eLayer.Player ) {
			return false;
		}

		if( distance == Vector3.zero ) {
			distance = World.Instance.InGameCamera.DefaultSetting.Distance;
		}

		if( lookAt == Vector2.zero ) {
			lookAt = World.Instance.InGameCamera.DefaultSetting.LookAt;
		}

		World.Instance.InGameCamera.SetUserSetting( distance, lookAt, lookAtPlayerBack, smoothTimeRatio, null, FOV );

		Debug.Log( "스킬 카메라 세팅 사용!!!!!!!!!!!!!!" );
		return true;
	}

	protected virtual void OnEndCameraSetting() {
		if( World.Instance.InGameCamera.Mode != InGameCamera.EMode.DEFAULT ) {
			return;
		}

		World.Instance.InGameCamera.EndUserSetting();
	}

	/*protected bool OnCameraAni(AnimationClip clip)
    {
        if(World.Instance.InGameCamera.Mode != InGameCamera.EMode.DEFAULT)
        {
            return false;
        }

        World.Instance.InGameCamera.PlayAnimation(clip, gameObject);
        return true;
    }*/

	protected bool OnCameraShake( float duration, float power, float speed ) {
		World.Instance.InGameCamera.PlayShake( this, duration, power, speed, 0.2f );
		return true;
	}

	private IEnumerator UpdateJump(bool dirJump, bool fastFall)
    {
        bool startFastFall = false;
        Vector3 dir = transform.forward;

        float speed = m_originalSpeed;
        if (m_mainTarget)
        {
            Vector3 v1 = transform.position;
            Vector3 v2 = m_mainTarget.transform.position;
            v1.y = v2.y = 0.0f;

            speed = Utility.GetDistanceWithoutY(v1, v2) * 1.5f;// Vector3.Distance(v1, v2);
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!isGrounded)
        {
            if (isFalling && !startFastFall)
            {
                m_cmptJump.SetFastFall();
            }

            m_cmptJump.UpdateJump();

            if (dirJump)
            {
                m_cmptMovement.UpdatePosition(dir, speed, true);
            }

            if(Director.IsPlaying && m_curHp <= 0.0f)
            {
                Utility.InitTransform(gameObject);
                yield break;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public eAnimation GetHitAni(eHitDirection hitDir)
    {
        if (hitDir == eHitDirection.None || m_listAniHit.Count <= 0 || (int)hitDir - 1 >= m_listAniHit.Count)
            return eAnimation.Hit;

        return m_listAniHit[(int)hitDir - 1];
    }

    protected void PlayShieldEffect(ParticleSystem ps)
    {
        if (ps == null || MainCollider == null)
            return;

        if (ps.gameObject.activeSelf == false)
            ps.gameObject.SetActive(true);

        if (attacker == null)
            return;

        Vector3 shieldOnHitDir = attacker.transform.position - transform.position;
        shieldOnHitDir.y = transform.position.y;

        Vector3 shieldOnHit = transform.position + (Vector3.Normalize(shieldOnHitDir) * (MainCollider.radius));
        shieldOnHit.y += (MainCollider.height * 0.5f);
        ps.transform.position = shieldOnHit;

        Vector3 attackerPos = attacker.transform.position;
        attackerPos.y = transform.position.y;
        ps.transform.LookAt(attackerPos);

        Vector3 shieldOnHitRot = ps.transform.eulerAngles;
        shieldOnHitRot.x = shieldOnHitRot.z = 0.0f;
        ps.transform.eulerAngles = shieldOnHitRot;

        ps.Play();
    }

    public void PlayHitEffect()
    {
        if (!m_psHit)
            return;

        m_psHit.gameObject.SetActive(true);
        //m_psHit.Play();

        EffectManager.Instance.RegisterStopEff(m_psHit, transform);
    }

    private IEnumerator PauseFrame()
    {
        m_aniEvent.PauseFrame();
        yield return new WaitForSeconds((m_pauseFrameRate / 30.0f) * Time.timeScale);

        m_aniEvent.Resume();
        m_pauseFrame = false;
    }

    public void StartPauseFrame(bool isCritical, float pauseFrame, bool isAttacker)
    {
        if (withoutAniEvent || m_curHp <= 0.0 || !IsActivate())
        {
            return;
        }

        m_pauseFrameRate = pauseFrame;

        if (GameInfo.Instance.BattleConfig.SamePauseFrameOnHit || (!isAttacker && !GameInfo.Instance.BattleConfig.SamePauseFrameOnHit))
            m_pauseFrameRate *= GameInfo.Instance.BattleConfig.PauseFrameGapRatio;

        if (isCritical)
            m_pauseFrameRate *= GameInfo.Instance.BattleConfig.CriticalPauseFrameRatio;

        StopPauseFrame();

        m_pauseFrame = true;

        StopCoroutine("PauseFrame");
        StartCoroutine("PauseFrame");
    }

    public void StopPauseFrame()
    {
        if (withoutAniEvent || m_curHp <= 0.0)
            return;

        StopCoroutine("PauseFrame");

        m_aniEvent.Resume();
        m_pauseFrame = false;
    }

    public void UpdateTargetHp(Unit target)
    {
        if (isClone || (target && target.grade != eGrade.Boss))
            return;

        if (target == null || target.curHp <= 0.0f)
        {
            World.Instance.UIPlay.ActiveTargetGauge(false, target);
        }
        else
        {
            World.Instance.UIPlay.UpdateTargetHp(target, target.tableName, target.curHp, target.maxHp);

            if (target.maxShield > 0.0f)
                World.Instance.UIPlay.UpdateTargetShield(target.curShield, target.maxShield);
        }
    }

	public virtual void UseSp( ObscuredFloat sp, bool decrease = false ) {
		m_curSp = Mathf.Clamp( m_curSp - sp, 0.0f, GameInfo.Instance.BattleConfig.USMaxSP );
	}

	public virtual void SpRegenIncRate( ObscuredFloat value ) {
	}

	/*public void UseMarble()
    {
        m_curMarble = 0;
        World.Instance.uiPlay.SubPlayerMarble(m_curMarble, m_curMarble);
    }*/

	public virtual bool OnStepForward( float remainTime, bool isSideStep, float dist, bool ignoreTarget, float cutFrameLength, bool ignoreY = false ) {
        // 이건 일단 임시
        if ( holdPositionRef > 0 || ( m_actionSystem.currentAction && m_actionSystem.currentAction.actionCommand == eActionCommand.QTEFromEvade ) ) {
            return false;
        }

		m_stepForwardEvt.IgnoreY = ignoreY;
		m_stepForwardEvt.PosYOnStart = transform.position.y;

		m_stepForwardEvt.dir = !isSideStep ? transform.forward : transform.right;

		m_stepForwardEvt.remainTime = remainTime;
		m_stepForwardEvt.remainTime -= ( m_stepForwardEvt.remainTime * m_speedRateByBuff );

		m_stepForwardEvt.dist = dist;

		UnitCollider targetCollider = GetMainTargetCollider( false );
		if ( dist > 0.0f && targetCollider != null ) {
			Unit target = targetCollider.Owner;
			if ( Vector3.Distance( target.transform.position, transform.position ) < dist ) {
				UnitCollider mainCollider = MainCollider;
				if ( mainCollider == null && cloneOwner ) {
					mainCollider = cloneOwner.MainCollider;
				}

				if ( ignoreTarget == false ) {
					float minDist = ( mainCollider.radius + target.MainCollider.radius ) * 0.8f;
					m_stepForwardEvt.dir = transform.forward;

					Vector3 dest = target.transform.position + ( -transform.forward * minDist );
					m_stepForwardEvt.dist = Vector3.Distance( dest, transform.position );
				}
				else if ( ignoreTarget == true ) {
					m_stepForwardEvt.ignoreCollision = true;

					if ( m_rigidBody && !alwaysKinematic ) {
						m_rigidBody.isKinematic = true;
					}

					if ( mainCollider ) {
						mainCollider.Enable( false );
					}
				}
			}
		}
		else if ( dist < 0.0f ) {
			RaycastHit hitInfo;
			if ( Physics.Raycast( transform.position, transform.forward, out hitInfo, dist,
								( 1 << (int)eLayer.EnvObject ) | ( 1 << (int)eLayer.Wall ) ) == true ) {
				if ( hitInfo.distance < dist )
					m_stepForwardEvt.dist = hitInfo.distance;
			}
		}

		m_stepForwardEvt.dir.y = 0.0f;

		if ( m_rigidBody && !alwaysKinematic && m_rigidBody.isKinematic && !m_stepForwardEvt.ignoreCollision ) {
			m_stepForwardEvt.ignoreCollision = true;
		}

		Utility.StopCoroutine( this, ref m_crStepForward );
		m_crStepForward = StartCoroutine( UpdateStepForward( null ) );

		return true;
	}

	public void OnStepForward(Unit target, float remainTime, float dist)
    {
        if (holdPositionRef > 0)
            return;

        m_stepForwardEvt.remainTime = remainTime;
        m_stepForwardEvt.remainTime -= (m_stepForwardEvt.remainTime * m_speedRateByBuff);

        m_stepForwardEvt.dir = Vector3.Normalize(target.transform.position - transform.position);

        if (dist == 0.0f)
        {
            float minDist = MainCollider.radius + target.MainCollider.radius;

            Vector3 dest = target.transform.position + (Vector3.Normalize(transform.position - target.transform.position) * minDist);
            m_stepForwardEvt.dist = Vector3.Distance(dest, transform.position);
        }
        else
            m_stepForwardEvt.dist = dist;

        m_stepForwardEvt.dir.y = 0.0f;

        //StopCoroutine("UpdateStepForward");
        Utility.StopCoroutine(this, ref m_crStepForward);
        m_crStepForward = StartCoroutine(UpdateStepForward(null));
    }

    public void StartStepForward(Unit target, float remainTime, float dist = 0.0f)
    {
        if (holdPositionRef > 0)
            return;

        m_stepForwardEvt.remainTime = remainTime;
        m_stepForwardEvt.remainTime -= (m_stepForwardEvt.remainTime * m_speedRateByBuff);

        m_stepForwardEvt.dir = Vector3.Normalize(target.transform.position - transform.position);

        if (dist == 0.0f)
        {
            float minDist = MainCollider.radius + target.MainCollider.radius;

            Vector3 dest = target.transform.position + (Vector3.Normalize(transform.position - target.transform.position) * minDist);
            m_stepForwardEvt.dist = Vector3.Distance(dest, transform.position);
        }
        else
            m_stepForwardEvt.dist = dist;

        m_stepForwardEvt.dir.y = 0.0f;

        //StopCoroutine("UpdateStepForward");
        Utility.StopCoroutine(this, ref m_crStepForward);
        m_crStepForward = StartCoroutine(UpdateStepForward(null));
    }

    public void StartStepForward(float remainTime, Vector3 dest, Action endCallback)
    {
        if (holdPositionRef > 0)
        {
            endCallback?.Invoke();
            return;
        }

        m_stepForwardEvt.remainTime = remainTime;
        m_stepForwardEvt.remainTime -= (m_stepForwardEvt.remainTime * m_speedRateByBuff);

        m_stepForwardEvt.dir = Vector3.Normalize(dest - transform.position);
        m_stepForwardEvt.dir.y = 0.0f;

        m_stepForwardEvt.dist = Vector3.Distance(dest, transform.position);

        //StopCoroutine("UpdateStepForward");
        Utility.StopCoroutine(this, ref m_crStepForward);
        m_crStepForward = StartCoroutine(UpdateStepForward(endCallback));
    }

    public void StartStepForward(float remainTime, Vector3 dir, float dist, Action endCallback)
    {
        if (holdPositionRef > 0)
            return;

        m_stepForwardEvt.remainTime = remainTime;

        m_stepForwardEvt.dir = dir;
        m_stepForwardEvt.dist = dist;

        Utility.StopCoroutine(this, ref m_crStepForward);
        m_crStepForward = StartCoroutine(UpdateStepForward(endCallback));
    }

    private IEnumerator UpdateStepForward(Action endCallback)
    {
        if(AllowAction == eAllowAction.None || AllowAction == eAllowAction.Attack)
        {
            yield break;
        }

        float time = 0.0f;
        bool end = false;

        float v = 0.0f;//(m_stepForwardEvt.dist * m_stepForwardDistRatio) / (m_stepForwardEvt.remainTime / (Time.fixedDeltaTime * m_stepForwardSpeedRatio));

        m_stepForwardEvt.state = sMovingPositionEvent.eStartMovingPositionState.Start;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (end == false)
        {
            if (holdPositionRef > 0)
                time = m_stepForwardEvt.remainTime;

            Vector3 newPos = Vector3.zero;
            if (m_pauseFrame == false)
            {
                time += Time.fixedDeltaTime;
                v = (m_stepForwardEvt.dist * m_stepForwardDistRatio) / (m_stepForwardEvt.remainTime / (fixedDeltaTime * m_stepForwardSpeedRatio));

                if (time >= m_stepForwardEvt.remainTime)
                {
                    Debug.Log("End StepForward");

                    end = true;
                    //UpdateStepForwardPosition(v);

                    if (m_stepForwardEvt.ignoreCollision)
                    {
                        if (m_rigidBody && !alwaysKinematic)
                            m_rigidBody.isKinematic = false;

                        MainCollider.Enable(true);
                    }

                    m_stepForwardEvt.Init();
                }

                UpdateStepForwardPosition(v);
            }

            yield return mWaitForFixedUpdate;
        }

        m_stepForwardDistRatio = 1.0f;
        m_stepForwardSpeedRatio = 1.0f;

        m_stepForwardEvt.state = sMovingPositionEvent.eStartMovingPositionState.End;

        if (m_rigidBody)
        {
            m_rigidBody.mass = m_originalMass;
        }

        //if (m_stepForwardEvt.ignoreCollision)
        //    Invoke("DisableKinematic", Time.fixedDeltaTime);

        if (endCallback != null)
            endCallback();
    }

    private void UpdateStepForwardPosition(float v)
    {
        if (LockAxis == eAxisType.X)
        {
            m_stepForwardEvt.dir.x = 0.0f;
        }
        else if (LockAxis == eAxisType.Z)
        {
            m_stepForwardEvt.dir.z = 0.0f;
        }

        Vector3 pos = Vector3.zero;
        if(m_rigidBody)
        {
            pos = m_rigidBody.position;
        }
        else
        {
            pos = transform.position;
        }

        Vector3 newPos = pos + (m_stepForwardEvt.dir * v);

        if (float.IsNaN(newPos.x) == false && float.IsNaN(newPos.y) == false && float.IsNaN(newPos.z) == false)
        {
            Vector3 hitPoint = Vector3.zero;
            if (m_rigidBody && m_rigidBody.isKinematic == false)
            {
                if (!IsCollisionWall(transform.position, newPos, ref hitPoint))
                {
                    if (m_stepForwardEvt.IgnoreY)
                    {
                        newPos.y = m_stepForwardEvt.PosYOnStart;
                    }

                    m_rigidBody.MovePosition(newPos);
                }
                else if (World.Instance.IsOpenSlopeMap)
                {
                    Vector3 higherPos = newPos;
                    higherPos.y = 30.0f;

                    RaycastHit hitInfo;
                    //if (Physics.Linecast(higherPos, newPos, out hitInfo, 1 << (int)eLayer.Floor) == true)
                    if(Physics.Raycast(higherPos, -Vector3.up, out hitInfo, Mathf.Infinity, 1 << (int)eLayer.Floor))
                    {
                        transform.position = new Vector3(newPos.x, hitInfo.point.y, newPos.z);
                    }
                }
            }
            else
            {
                if (!IsCollisionWall(transform.position, newPos, ref hitPoint))
                {
                    transform.position = CheckCollisionWall(transform.position, newPos);

                    if (m_stepForwardEvt.IgnoreY)
                    {
                        transform.position = new Vector3(transform.position.x, m_stepForwardEvt.PosYOnStart, transform.position.z);
                    }
                }
                else if(World.Instance.IsOpenSlopeMap)
                {
                    Vector3 higherPos = newPos;
                    higherPos.y = 30.0f;

                    RaycastHit hitInfo;
                    //if (Physics.Linecast(higherPos, newPos, out hitInfo, 1 << (int)eLayer.Floor) == true)
                    if(Physics.Raycast(higherPos, -Vector3.up, out hitInfo, Mathf.Infinity, 1 << (int)eLayer.Floor))
                    {
                        transform.position = new Vector3(newPos.x, hitInfo.point.y, newPos.z);
                    }
                }
            }
        }
    }

    public void StopStepForward()
    {
        if (m_stepForwardEvt.ignoreCollision)
        {
            if (m_rigidBody && !alwaysKinematic)
            {
                m_rigidBody.isKinematic = false;
            }

            MainCollider.Enable(true);
        }

        m_stepForwardEvt.Init();

        if (m_crStepForward != null)
        {
            Utility.StopCoroutine(this, ref m_crStepForward);
        }
    }

    public void SetStepForwardRatio(float distRatio, float speedRatio)
    {
        m_stepForwardDistRatio = distRatio;
        m_stepForwardSpeedRatio = speedRatio;
    }

    public void SetStepForwardDir(Vector3 dir)
    {
        m_stepForwardEvt.dir = dir;
    }

    public bool IsBeforeGround(float dist)
    {
        //eLayer enemyLayer = Utility.GetEnemyLayer((eLayer)gameObject.layer);

        int layer = (1 << (int)eLayer.Floor) | (1 << (int)eLayer.Wall) | (1 << (int)eLayer.EnvObject);
        bool b = Physics.Linecast(transform.position, transform.position + (-Vector3.up * dist), layer);

        if(!b)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, dist * 0.5f, layer);
            if(cols.Length > 0)
            {
                b = true;
            }
        }

        return b;

        /*RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, dist, (1 << (int)eLayer.Floor) |
                                                                                  (1 << (int)eLayer.Wall) |
                                                                                  //(1 << (int)eLayer.Wall_Inside) |
                                                                                  (1 << (int)eLayer.EnvObject)))
        //(1 << (int)enemyLayer)))
        {
            return true;
        }*/

        //return false;
    }

    public bool CheckTargetDist(eActionCommand actionCommand, float f = 1.2f, bool isOnlyEnemy = true)
    {
        UnitCollider targetCollider = GetMainTargetCollider( isOnlyEnemy );
        if (targetCollider == null)
        {
            return false;
        }

        float dist = 0.0f;

        ActionBase action = m_actionSystem.GetAction(actionCommand);
		if(action == null)
		{
            if( tableId == 8 && actionCommand == eActionCommand.Attack01 ) {
                dist = Vector3.Distance(MainCollider.GetCenterPos(), GetTargetCapsuleEdgePos(targetCollider.Owner));
                if( dist <= ( 20.0f * f ) ) {
                    return true;
                }
            }

			return false;
		}

        float atkRange = action.GetAtkRange();
        if(atkRange == 0.0f)
        {
            Debug.LogError(tableName + "의 " + actionCommand + " 공격 범위가 0이면 안되는디!!!?");
        }

        dist = Vector3.Distance(MainCollider.GetCenterPos(), GetTargetCapsuleEdgePos(targetCollider.Owner));
        if (dist <= (atkRange * f))
        {
            return true;
        }

        return false;
    }

    public void SetEvadedTarget(Unit target)
    {
        if (target == null)
            return;

        m_evadedTarget = target;
        //SetMainHitTarget(target);
    }

    /*public virtual bool HasTarget()
    {
        Unit find = m_listEnemy.Find(x => x.curHp > 0.0f);
        if (find == null)
            return false;

        if (m_listEnemy.Count > 0)
            return true;

        return false;
    }*/

    public bool HasHitTarget()
    {
        return m_listHitTarget.Count > 0;
    }

    public Unit GetAliveHitTarget()
    {
        for(int i = 0; i < m_listHitTarget.Count; ++i)
        {
            if(m_listHitTarget[i].curHp > 0.0f)
            {
                return m_listHitTarget[i];
            }
        }

        return null;
    }

	/*public void AddEnemy(Unit target)
    {
        if (target == null || target.curHp <= 0.0f)//target.IsActivate() == false)
            return;

        Unit find = m_listEnemy.Find(x => x == target);
        if (find != null)
            return;

        m_listEnemy.Add(target);
    }

    public void AddEnemy(Unit[] targets)
    {
        m_listEnemy.Clear();

        for (int i = 0; i < targets.Length; i++)
            m_listEnemy.Add(targets[i]);
    }

    public void ClearEnemyList()
    {
        m_listEnemy.Clear();
    }*/

	public void UpdateDamagePerSecond( float damage ) {
        if ( !( this is Player ) ) {
            return;
		}

		DamagePerSecond += damage;
		
        if ( mCrCheckDPS == null ) {
			mCrCheckDPS = StartCoroutine( CheckDPS() );
		}
	}

	private float PlayAnimation(eAnimation aniType, int layer = 0)
    {
        if (withoutAniEvent || m_aniEvent.IsPause() == true)
            return 0.0f;

        bool blending = true;
        if (m_aniEvent.curAniType == aniType || m_aniEvent.IsAniPlaying(m_aniEvent.curAniType) == eAniPlayingState.Blending)
            blending = false;

        float blendingTime = 0.0f;
        if (blending == true && m_aniEvent.curAniClip != null && m_aniEvent.curAniType != eAnimation.None)
            blendingTime = (Application.targetFrameRate / (m_aniEvent.curAniClip.frameRate * m_aniEvent.curAniClip.length)) * 0.2f;

        return m_aniEvent.PlayAni(aniType, blending, blendingTime, 0.0f, layer);
    }

    private float PlayAnimationImmediate(eAnimation aniType, float normalizeTime = 0f)
    {
        if (withoutAniEvent || m_aniEvent.IsPause() == true)
            return 0.0f;

        // 같은 애니가 들어왔을때 다른 애니를 한번 틀어줘야 현재 애니가 취소되고 다시 플레이 됨???
        if (m_aniEvent.IsAniPlaying(aniType) != eAniPlayingState.None)
            m_aniEvent.StopAni(aniType);//m_aniEvent.PlayAni(eAnimation.Idle01, false, 0.0f, 0.0f);

        return m_aniEvent.PlayAni(aniType, false, 0.0f, normalizeTime);
    }

	public float PlayAni( eAnimation aniType, int layer = 0, bool backToIdle = false, bool lockCloneFollowAni = false ) {
		float aniLength = PlayAnimation(aniType, layer);

		StopCoroutine( "BackToIdle" );
        if( backToIdle ) {
            StartCoroutine( "BackToIdle", aniLength );
        }

		for( int i = 0; i < m_listClone.Count; ++i ) {
			Unit clone = m_listClone[i];
			if( clone == null || !clone.IsActivate() || !clone.IsFollowOwner || clone.LockFollowAni ) {
				continue;
			}

            clone.LockFollowAni = lockCloneFollowAni;

			if( aniType == eAnimation.Run || aniType == eAnimation.RunBack || aniType == eAnimation.RunFast || aniType == eAnimation.RunStop ||
                aniType == eAnimation.Dash || aniType == eAnimation.BackDash ) {
                continue;
			}

			clone.DelayedPlayAni( m_aniEvent.GetFirstAttackEventLength( aniType ), aniType, layer, backToIdle );
		}

		return aniLength;
	}

	public float PlayAniImmediate( eAnimation aniType, float normalizeTime = 0.0f, bool backToIdle = false, bool lockCloneFollowAni = false ) {
		float aniLength = PlayAnimationImmediate(aniType, normalizeTime);

		StopCoroutine( "BackToIdle" );
        if( backToIdle ) {
            StartCoroutine( "BackToIdle", aniLength * ( 1f - normalizeTime ) );
        }

		for( int i = 0; i < m_listClone.Count; ++i ) {
			Unit clone = m_listClone[i];
			if( clone == null || !clone.IsActivate() || !clone.IsFollowOwner || clone.LockFollowAni ) {
				continue;
			}

            clone.LockFollowAni = lockCloneFollowAni;

            if( aniType == eAnimation.Run || aniType == eAnimation.RunBack || aniType == eAnimation.RunFast || aniType == eAnimation.RunStop ||
                aniType == eAnimation.Dash || aniType == eAnimation.BackDash ) {
                continue;
            }

            clone.DelayedPlayAniImmediate( m_aniEvent.GetFirstAttackEventLength( aniType ), aniType, normalizeTime, backToIdle );
		}

		return aniLength;
	}

	private IEnumerator BackToIdle(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayAni(eAnimation.Idle01);
    }

    //돌아갈 애니메이션 설정
    public float PlayAniToBackAni(eAnimation aniType, int layer = 0, bool backToAni = false, eAnimation backAniType = eAnimation.None)
    {
        float aniLength = PlayAnimation(aniType, layer);

        StopCoroutine("BackToAni");
        if (backToAni)
            StartCoroutine(BackToAni(aniLength, backAniType));

        return aniLength;
    }

    //돌아갈 애니메이션 설정
    public float PlayAniImmediateToBackAni(eAnimation aniType, float normalizeTime = 0.0f, bool backToAni = false, eAnimation backAniType = eAnimation.None)
    {
        float aniLength = PlayAnimationImmediate(aniType, normalizeTime);

        StopCoroutine("BackToAni");
        if (backToAni)
            StartCoroutine(BackToAni(aniLength * (1f - normalizeTime), backAniType));

        return aniLength;
    }

    //돌아갈 애니메이션 설정
    private IEnumerator BackToAni(float delay, eAnimation backAniType)
    {
        yield return new WaitForSeconds(delay);
        PlayAni(backAniType);
    }

    public float PlayAniToBackAniWithAlpha(eAnimation aniType, int layer = 0, float alpha = 1f, bool backToAni = false, eAnimation backAniType = eAnimation.None)
    {
        float aniLength = PlayAnimation(aniType, layer);
        aniEvent.SetShaderAlpha("_Color", alpha);

        StopCoroutine("BackToAniWithAlpha");
        if (backToAni)
            StartCoroutine(BackToAniWithAlpha(aniLength, backAniType, alpha));

        return aniLength;
    }

    public float PlayAniImmediateToBackAniWithAlpha(eAnimation aniType, float normalizeTime = 0.0f, float alpha = 1f, bool backToAni = false, eAnimation backAniType = eAnimation.None)
    {
        float aniLength = PlayAnimationImmediate(aniType, normalizeTime);
        aniEvent.SetShaderAlpha("_Color", alpha);

        StopCoroutine("BackToAniWithAlpha");
        if (backToAni)
            StartCoroutine(BackToAniWithAlpha(aniLength * (1f - normalizeTime), backAniType, alpha));

        return aniLength;
    }

    private IEnumerator BackToAniWithAlpha(float delay, eAnimation backaniType, float alpha)
    {
        yield return new WaitForSeconds(delay);
        PlayAni(backaniType);
        aniEvent.SetShaderAlpha("_Color", alpha);
    }

    public void SkipCurrentAni()
    {
		//m_aniEvent.PlayAni(m_aniEvent.curAniType, false, 0.0f, 1.0f);
		PlayAniImmediate( m_aniEvent.curAniType, 1.0f );
	}

    public void PlayFaceAni(eFaceAnimation faceAni, int layer = 0)
    {
        if (m_aniEvent == null || withoutAniEvent || m_aniEvent.IsPause())
            return;

        m_aniEvent.PlayFaceAni(faceAni, layer);
    }

    public float PlayAni(eAnimation aniType, int layer, eFaceAnimation faceAniType, int faceAniLayer)
    {
        float length = PlayAni(aniType, layer);
        if (length > 0.0f)
        {
            PlayFaceAni(faceAniType, faceAniLayer);
        }

        return length;
    }

    public float PlayAniImmediate(eAnimation aniType, eFaceAnimation faceAniType, int faceAniLayer)
    {
        float length = PlayAniImmediate(aniType, 0.0f);
        if (length > 0.0f)
        {
            PlayFaceAni(faceAniType, faceAniLayer);
        }

        return length;
    }

    public virtual void Pause()
    {
        m_pause = true;

        if (m_aniEvent)
        {
            m_aniEvent.Pause();
        }
    }

    public virtual void Resume()
    {
        m_pause = false;

        if (m_aniEvent)
        {
            m_aniEvent.Resume();
        }

        ShowMesh(true);
    }

    private Coroutine mCrShake = null;
	public virtual void Shake( bool forOnlyDamage ) {
		if( shakeDuration <= 0.0f ) {
			shakeDuration = 0.1f;
		}

		if( m_aniEvent ) {
			m_aniEvent.transform.localPosition = Vector3.zero;
		}

        Utility.StopCoroutine( this, ref mCrShake );
		mCrShake = StartCoroutine( UpdateShake( forOnlyDamage ) );
	}

	private IEnumerator UpdateShake( bool forOnlyDamage ) {
		float elapsed = 0.0f;
		Vector3 originPos = m_aniEvent ? m_aniEvent.transform.localPosition : transform.position;

		while ( elapsed < shakeDuration ) {
			elapsed += Time.deltaTime;
			Vector3 newPos = originPos + UnityEngine.Random.insideUnitSphere * ( shakeSpeed * 0.2f );

			if ( m_aniEvent ) {
				newPos.y = 0.0f;
				m_aniEvent.transform.localPosition = newPos;
			}
			else {
				transform.position = newPos;
			}

			yield return null;
		}

		if ( m_aniEvent ) {
			m_aniEvent.transform.localPosition = Vector3.zero;
		}
		else {
			transform.position = originPos;
		}

        SendRemoveHitTarget();
	}

	public void LockDie(bool lockDie)
    {
        m_lockDie = lockDie;

        /*if (lockDie == false && m_curHp <= 0.0f)
        {
            m_actionSystem.CancelCurrentAction();
            CommandAction(eActionCommand.Die, null);
        }*/
    }

	/*
    protected void LoadDropItem()
    {
        m_listDropItem.Clear();

        List<GameClientTable.DropItem.Param> listParamDropItem = GameInfo.Instance.GameClientTable.DropItems;
        for (int i = 0; i < listParamDropItem.Count; i++)
        {
            for (int j = 0; j < GameInfo.Instance.BattleConfig.MaxDropItemNum; j++)
            {
                DropItem dropItem = ResourceMgr.Instance.CreateFromAssetBundle<DropItem>("item", listParamDropItem[i].ModelPb + ".prefab");
                dropItem.Init(listParamDropItem[i]);
                dropItem.gameObject.SetActive(false);

                m_listDropItem.Add(dropItem);
            }
        }
    }
	*/

    protected virtual void DropItem()
    {
        isDying = true;
    }

	/*
    protected virtual void DropItemBySkill(int dropItemId, int dropCnt)
    {
        List<DropItem> listDropItem = m_listDropItem.FindAll(x => x.data.ID == dropItemId);
        int max = Mathf.Min(dropCnt, GameInfo.Instance.BattleConfig.MaxDropItemNum);
        for (int i = 0; i < max; i++)
            listDropItem[i].Drop(GetCenterPos());
    }
	*/

    public virtual float GetAirAttackJumpPower()
    {
        if (m_cmptJump == null)
            return 0.0f;

        return m_cmptJump.m_jumpPower;
    }

    public Vector3 GetTargetDirection(Unit target, ref int selectedIndex)
    {
        for (int i = 0; i < (int)eDirection.All; i++)
        {
            if (target.directions[i].usingUnit == null || !target.directions[i].usingUnit.IsActivate() || target.directions[i].usingUnit.curHp <= 0.0f)
                target.directions[i].used = false;
        }

        selectedIndex = -1;
        eDirection dir = eDirection.Forward;
        for (int i = 0; i < (int)eDirection.All; i++)
        {
            if (!target.directions[i].used)
            {
                dir = target.directions[i].dir;

                target.directions[i].used = true;
                target.directions[i].usingUnit = target;

                selectedIndex = i;
                break;
            }
        }

        if (selectedIndex == -1)
        {
            selectedIndex = UnityEngine.Random.Range(0, (int)eDirection.All);
            dir = target.directions[selectedIndex].dir;

            target.directions[selectedIndex].used = true;
            target.directions[selectedIndex].usingUnit = target;
        }

        Vector3 v = Vector3.zero;
        switch (dir)
        {
            case eDirection.Forward:
                v = target.transform.forward;
                break;

            case eDirection.Left:
                v = -target.transform.right;
                break;

            case eDirection.Right:
                v = target.transform.right;
                break;

            case eDirection.Back:
                v = -target.transform.forward;
                break;

            case eDirection.Forward_Left:
                v = (target.transform.forward - target.transform.right).normalized;
                break;

            case eDirection.Forward_Right:
                v = (target.transform.forward + target.transform.right).normalized;
                break;

            case eDirection.Back_Left:
                v = (-target.transform.forward - target.transform.right).normalized;
                break;

            case eDirection.Back_Right:
                v = (-target.transform.forward + target.transform.right).normalized;
                break;
        }

        return v;
    }

    public Vector3 GetTargetDirection(Unit target, int index)
    {
        eDirection dir = target.directions[index].dir;
        Vector3 v = Vector3.zero;
        switch (dir)
        {
            case eDirection.Forward:
                v = target.transform.forward;
                break;

            case eDirection.Left:
                v = -target.transform.right;
                break;

            case eDirection.Right:
                v = target.transform.right;
                break;

            case eDirection.Back:
                v = -target.transform.forward;
                break;

            case eDirection.Forward_Left:
                v = (target.transform.forward - target.transform.right).normalized;
                break;

            case eDirection.Forward_Right:
                v = (target.transform.forward + target.transform.right).normalized;
                break;

            case eDirection.Back_Left:
                v = (-target.transform.forward - target.transform.right).normalized;
                break;

            case eDirection.Back_Right:
                v = (-target.transform.forward + target.transform.right).normalized;
                break;
        }

        return v;
    }

    public void ReturnTargetDirection(Unit target, int index)
    {
        target.directions[index].used = false;
        target.directions[index].usingUnit = null;
    }

    /*public void LockZAxis(bool lockZAxis)
    {
        m_lockZAxis = lockZAxis;

        if (!lockZAxis)
        {
            mDefaultRigidbodyConstraints = RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            mDefaultRigidbodyConstraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }

        m_rigidBody.constraints = mDefaultRigidbodyConstraints;
    }*/
    
    /*
    public void AddLockYAxis()
    {
        mDefaultRigidbodyConstraints |= RigidbodyConstraints.FreezePositionY;
        m_rigidBody.constraints = mDefaultRigidbodyConstraints;
    }
    */

    public void SetLockAxis(eAxisType axisType)
    {
        LockAxis = axisType;

        if (LockAxis == eAxisType.None)
        {
            mDefaultRigidbodyConstraints = RigidbodyConstraints.FreezeRotation;
        }
        else if (LockAxis == eAxisType.Z)
        {
            mDefaultRigidbodyConstraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
        else if (LockAxis == eAxisType.X)
        {
            mDefaultRigidbodyConstraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
        }
        
        /*
        if(World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
        {
            AddLockYAxis();
        }
        */

        m_rigidBody.constraints = mDefaultRigidbodyConstraints;
    }

    public virtual void OnEndGame()
    {
    }

	public virtual bool StartBT() {
		if( mAI == null || !IsActivate() || m_actionSystem.HasNoAction() || m_curHp <= 0.0f || World.Instance.IsEndGame || World.Instance.IsPause ) {
			return false;
		}

		mAI.StartBT();

		if( m_child ) {
			m_child.StartBT();
		}

		return true;
	}

	public virtual bool StopBT() {
		if( mAI == null || !IsActivate() || m_actionSystem.HasNoAction() || mAI.IsStop ) {
			return false;
		}

		mAI.StopBT();

		if( m_child ) {
			m_child.StopBT();
		}

		return true;
	}

	public virtual void ResetBT() {
		if( mAI == null || !IsActivate() || m_actionSystem.HasNoAction() || m_curHp <= 0.0f ) {
			return;
		}

		StopBT();
		StartBT();

		if( m_child ) {
			m_child.ResetBT();
		}
	}

	public virtual bool IsAttackImpossible()
    {
        return false;
    }

    public bool NeedEscape()
    {
        if (Physics.Raycast(MainCollider.GetCenterPos(), -transform.forward, out RaycastHit hitInfo, 2.0f, 1 << (int)eLayer.Wall))
        {
            return true;
        }

        return false;
    }

    public void StartDissolve(float duration, bool backward, Color color)
    {
        if (m_aniEvent == null)
        {
            return;
        }

        if(mMtrlDissolve == null)
        {
			Material src = ResourceMgr.Instance.LoadFromAssetBundle("etc", "Etc/Material/mat_fx_behimos_die_dissolve.mat") as Material;
			mMtrlDissolve = new Material(src);
        }

        if(mMtrlDissolve == null)
        {
            return;
        }

        if (mMtrlDissolve.HasProperty("_Main_Tex_Color"))
        {
            mMtrlDissolve.SetColor("_Main_Tex_Color", color);
        }

        Texture mainTex = null;

        List<Material> list = m_aniEvent.GetAllMtrl();
        if (list != null && list.Count > 0)
        {
            mainTex = list[0].mainTexture;
        }

        m_aniEvent.SetMtrl(mMtrlDissolve, mainTex, "_Main_Tex");

        if (!backward)
        {
            StartCoroutine("UpdateDissolveForward", duration);
        }
        else
        {
            StartCoroutine("UpdateDissolveBackward", duration);
        }
    }

    private IEnumerator UpdateDissolveForward(float duration)
    {
        if (duration <= 0.0f)
        {
            yield break;
        }

        List<Material> listMtrl = m_aniEvent.GetAllMtrl();
        float value = 1.0f;

        while (value > 0.0f)
        {
            for (int i = 0; i < listMtrl.Count; i++)
            {
                if (listMtrl[i].HasProperty("_Dissolve_power") == false)
                {
                    continue;
                }

                listMtrl[i].SetFloat("_Dissolve_power", value);
            }

            value -= Time.fixedDeltaTime / duration;
            yield return mWaitForFixedUpdate;
        }

        for (int i = 0; i < listMtrl.Count; i++)
        {
            if (listMtrl[i].HasProperty("_Dissolve_power") == false)
            {
                continue;
            }

            listMtrl[i].SetFloat("_Dissolve_power", 1.0f);
        }

		mMtrlDissolve.SetTexture("_Main_Tex", null);
		mMtrlDissolve.SetColor("_Main_Tex_Color", Color.white);

		m_aniEvent.RestoreMtrl();
    }

    private IEnumerator UpdateDissolveBackward(float duration)
    {
        if (duration <= 0.0f)
        {
            yield break;
        }

        List<Material> listMtrl = m_aniEvent.GetAllMtrl();
        for (int i = 0; i < listMtrl.Count; i++)
        {
            if (listMtrl[i].HasProperty("_Dissolve_power") == false)
            {
                continue;
            }

            listMtrl[i].SetFloat("_Dissolve_power", 0.0f);
        }

        float value = 0.0f;

        while (value <= 1.0f)
        {
            for (int i = 0; i < listMtrl.Count; i++)
            {
                if (listMtrl[i].HasProperty("_Dissolve_power") == false)
                {
                    continue;
                }

                listMtrl[i].SetFloat("_Dissolve_power", value);
            }

            value += Time.fixedDeltaTime / duration;
            yield return mWaitForFixedUpdate;
        }

		mMtrlDissolve.SetTexture("_Main_Tex", null);
		mMtrlDissolve.SetColor("_Main_Tex_Color", Color.white);

		m_aniEvent.RestoreMtrl();
    }
    
    public void AddPlayerMinion(PlayerMinion minion)
    {
        mListMinion.Add(minion);
    }

    public List<PlayerMinion> GetAllActiveMinion()
    {
        List<PlayerMinion> list = new List<PlayerMinion>();
        
        for(int i = 0; i < mListMinion.Count; i++)
        {
            if(mListMinion[i].IsActivate())
            {
                list.Add(mListMinion[i]);
            }
        }

        return list;
    }

    public void DeactivateAllPlayerMinion()
    {
        for (int i = 0; i < mListMinion.Count; i++)
        {
            mListMinion[i].StopBT();
            mListMinion[i].Deactivate();
        }
    }

	public void DeactivatePlayerMinionByIds(int[] ids)
	{
        for(int i = 0; i < mListMinion.Count; i++)
        {
            for(int j = 0; j < ids.Length; j++)
            {
                if(mListMinion[i].tableId != ids[j])
                {
                    continue;
                }

                mListMinion[i].StopBT();
                mListMinion[i].Deactivate();
            }
        }
	}

    public PlayerMinion GetMinionOrNullByIds(int[] ids)
    {
        int randIndex = UnityEngine.Random.Range(0, ids.Length);
        return mListMinion.Find(x => x.tableId == ids[randIndex]);
    }

    public PlayerMinion GetDeactivateMinionOrNullByIds( int[] ids ) {
        for( int i = 0; i < mListMinion.Count; i++ ) {
            for( int j = 0; j < ids.Length; j++ ) {
                if( !mListMinion[i].IsActivate() && mListMinion[i].tableId == ids[j] ) {
                    return mListMinion[i];
                }
			}
		}

        return null;
	}

    public void AddAbnormalAttr(EAttackAttr attr)
    {
        EAttackAttr find = mListCurrentAbnormalAttr.Find(x => x == attr);
        if(find != EAttackAttr.NONE)
        {
            return;
        }

        mListCurrentAbnormalAttr.Add(attr);
    }

    public void RemoveAbnormalAttr(EAttackAttr attr)
    {
        EAttackAttr find = mListCurrentAbnormalAttr.Find(x => x == attr);
        if (find == EAttackAttr.NONE)
        {
            return;
        }

        mListCurrentAbnormalAttr.Remove(attr);
    }

    public bool IsCurrentAbnormalAttr(EAttackAttr attr)
    {
        EAttackAttr find = mListCurrentAbnormalAttr.Find(x => x == attr);
        if (find != EAttackAttr.NONE)
        {
            return true;
        }

        return false;
    }

	public void AddCmptDebuffDuration( BuffEvent evt ) {
		if( m_cmptBuffDebuff == null ) {
			return;
		}

		eBuffDebuffType buffDebuffType = (eBuffDebuffType)evt.value2;

		if( evt.battleOptionData != null ) {
			if( evt.battleOptionData.preConditionType != BattleOption.eBOConditionType.None &&
				evt.battleOptionData.timingType != BattleOption.eBOTimingType.GameStart ) {
				if( buffDebuffType == eBuffDebuffType.Debuff ) {
					if( !GameSupport.IsBOConditionCheck( evt.battleOptionData.preConditionType, evt.battleOptionData.CondValue, evt.sender, this,
														 evt.battleOptionData.CheckActionTableId, evt.battleOptionData.Pjt ) ) {
						return;
					}
				}
				else {
					if( !GameSupport.IsBOConditionCheck( evt.battleOptionData.preConditionType, evt.battleOptionData.CondValue, this, evt.sender,
														 evt.battleOptionData.CheckActionTableId, evt.battleOptionData.Pjt ) ) {
						return;
					}
				}
			}
		}

		m_cmptBuffDebuff.AddDebuffDuration( buffDebuffType, (int)evt.value3, evt.value );
	}

	public void SetTemporarySpeedRate(float rate)
	{
		TemporarySpeedRate = rate;
		m_curSpeed += (m_curSpeed * TemporarySpeedRate);

		if(rate == 0.0f)
		{
			SetSpeedRateBySprint(0.0f);
		}
	}

	private IEnumerator UpdateFireProjectile(ProjectileEvent projectileEvt)
	{
		if(projectileEvt == null)
		{
			yield break;
		}

		AniEvent.sEvent atkEvt = m_aniEvent.CreateEvent(eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f,
														projectileEvt.value);

		int count = (int)projectileEvt.value2;

		List<AniEvent.sProjectileInfo> listPjtInfo = new List<AniEvent.sProjectileInfo>(count);
		for(int i = 0; i < count; ++i)
		{
			AniEvent.sProjectileInfo info = m_aniEvent.CreateProjectileInfo(projectileEvt.ListProjectile[i]);
			info.addedPosition = GetCenterPos() - transform.position;
			info.notAniEventAtk = true;

			listPjtInfo.Add(info);
		}

		bool end = false;
		float checkTime = 0.0f;
		float checkTick = 0.0f;

		while (!end && curHp > 0.0f)
		{
			checkTick += Time.fixedDeltaTime;

			if (projectileEvt.Duration > -1 && checkTime >= projectileEvt.Duration)
			{
				end = true;
			}
			else
			{
				if (checkTick >= projectileEvt.Tick)
				{
					if (projectileEvt.value2 > 1)
					{
						List<UnitCollider> listTargetCollider = projectileEvt.sender.GetTargetColliderList();
						for (int i = 0; i < listTargetCollider.Count; i++)
						{
							if (i >= projectileEvt.ListProjectile.Count)
							{
								break;
							}

							projectileEvt.ListProjectile[i].Fire(projectileEvt.sender,
																 projectileEvt.battleOptionData != null ? projectileEvt.battleOptionData.toExecuteType : BattleOption.eToExecuteType.Unit,
																 atkEvt, listPjtInfo[i], listTargetCollider[i].Owner, -1);
						}
					}
					else if (projectileEvt.value2 > 0)
					{
						UnitCollider collider = projectileEvt.sender.GetMainTargetCollider(true);
						if (collider)
						{
							projectileEvt.ListProjectile[0].Fire(projectileEvt.sender,
																 projectileEvt.battleOptionData != null ? projectileEvt.battleOptionData.toExecuteType : BattleOption.eToExecuteType.Unit,
																 atkEvt, listPjtInfo[0], collider.Owner, -1);
						}
					}

					checkTime += checkTick;
					checkTick = 0.0f;
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}

    private void SetAddAction( BattleOption.eBOConditionType condiType, BaseEvent evt ) {
		Type type = Type.GetType( Utility.AppendString( "Action", condiType.ToString() ) );
        if ( type == null ) {
            return;
        }

		ActionSelectSkillBase action = GetComponent( type ) as ActionSelectSkillBase;
		if ( action == null ) {
			return;
		}

		action.SetAddAction = false;

		if ( evt.battleOptionData.timingType == BattleOption.eBOTimingType.AddCall ||
             evt.battleOptionData.timingType == BattleOption.eBOTimingType.GameStart ||
			 ( actionSystem.currentAction && actionSystem.currentAction.TableId == action.TableId ) ) {

			action.SetAddAction = true;

			action.AddActionValue1 = evt.value;
			action.AddActionValue2 = evt.value2;
			action.AddActionValue3 = evt.value3;
			action.AddActionDuration = evt.battleOptionData.duration;
		}
	}

    private void SetIncreaseAddAction( BattleOption.eBOConditionType condiType, BaseEvent evt ) {
		Type type = Type.GetType( Utility.AppendString( "Action", condiType.ToString() ) );
		if ( type == null ) {
			return;
		}

		ActionSelectSkillBase action = GetComponent( type ) as ActionSelectSkillBase;
		if ( action == null ) {
			return;
		}

		if ( evt.battleOptionData.timingType == BattleOption.eBOTimingType.AddCall ||
             evt.battleOptionData.timingType == BattleOption.eBOTimingType.GameStart ||
             ( actionSystem.currentAction && actionSystem.currentAction.TableId == action.TableId ) ) {

			switch ( evt.value2 ) {
				case 1: {
					action.AddActionValue1 += evt.value;
				}
				break;

				case 2: {
					action.AddActionValue2 += evt.value;
				}
				break;

				case 3: {
					action.AddActionValue3 += evt.value;
				}
				break;

				default: {

				}
				break;
			}
		}
	}

    private IEnumerator CheckDPS() {
        float checkTime = 0.0f;
        float accumTime = 0.0f;

        while( true ) {
            checkTime += Time.fixedDeltaTime;

            if ( checkTime >= 1.0f ) {
                accumTime += checkTime;

                if ( DamagePerSecond > MaxDamagePerSecond ) {
                    MaxDamagePerSecond = DamagePerSecond;
                }

                if ( DamagePerSecond > 0.0f ) {
                    mDpsAccum += DamagePerSecond;
                    DpsAverage = mDpsAccum / accumTime;
                }

                if ( m_curHp <= 0.0f || World.Instance.IsEndGame || World.Instance.ProcessingEnd ) {
                    break;
                }
                else {
                    DamagePerSecond = 0.0f;
                    checkTime = 0.0f;
                }
            }

            yield return mWaitForFixedUpdate;
		}

        if ( DamagePerSecond > MaxDamagePerSecond ) {
            MaxDamagePerSecond = DamagePerSecond;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(m_aniEvent == null)
        {
            return;
        }

        AniEvent.sEvent evt = m_aniEvent.GetCurEventByAtkFrameOrNull();
        if(evt == null)
        {
            return;
        }

        if (evt.hitBoxSize != Vector3.zero)
        {
            Vector3 bonePos = transform.position;
            Vector3 boneRot = Vector3.zero;

            Transform bone = m_aniEvent.GetBoneByName(evt.hitBoxBoneName);
            if (bone)
            {
                if (evt.useBoneRot)
                {
                    bonePos = bone.transform.position + (bone.transform.forward * (evt.hitBoxSize.z * 0.5f));
                    boneRot = bone.transform.rotation.eulerAngles;
                }
                else
                {
                    bonePos = bone.transform.position;
                }
            }

            Vector3 pos = bonePos + (transform.rotation * evt.hitBoxPosition);

            Quaternion q = Quaternion.Euler(boneRot + evt.hitBoxPosition + new Vector3(0.0f, transform.eulerAngles.y, 0.0f));
            if (evt.useBoneRot)
            {
                q = Quaternion.Euler(boneRot + evt.hitBoxRotation);
            }

            DrawRotatedCube(pos, evt.hitBoxSize, q);
        }
    }

    private void DrawRotatedCube(Vector3 pos, Vector3 size, Quaternion q)
    {
        Color cGizmo = Gizmos.color;
        Matrix4x4 matGizmo = Gizmos.matrix;

        Matrix4x4 mat = Matrix4x4.TRS(pos, q, Vector3.one);

        Gizmos.color = Color.red;
        Gizmos.matrix *= mat;
        Gizmos.DrawWireCube(Vector3.zero, size);

        Gizmos.color = cGizmo;
        Gizmos.matrix = matGizmo;
    }
#endif
}
