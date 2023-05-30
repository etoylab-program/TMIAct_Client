
using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public partial class Player : Unit
{
    public enum eComboResetCondition
    {
        None = 0,   // 리셋 없음
        ByTime,     // 시간 제한
        ByHit,      // 피격 시
    }


    [Header("[Player Property]")]
    public float    minDistanceForCamera            = 5.0f;
    public Vector3  CommunicationCameraPos          = new Vector3(0.53f, 1.45f, 1.58f);
    public Vector3  CommunicationCameraRot          = new Vector3(5.0f, 30.0f, 0.0f);
    public float    TargetCameraWeight              = 0.4f;
    public int      UnitWeaponCloneCount            = 0;
    public float    PVPRenderTexCamSize             = 0.9f;
    public string   ChangeWeaponName                = "";
    public bool     UseCutframeForLastComboAttack   = false;
    public bool     UseGuardian                     = false;
    public Vector3  _ExtraCameraDistance            = Vector3.zero;

    public CharData         charData                        { get { return m_charData; } }
    public BOSupporter      boSupporter                     { get { return m_boSupporter; } }
    public BOWeapon         boWeapon                        { get { return mBOWeapons[mCurWeaponIndex]; } }
    public Unit             TargetEffectOwner               { get { return m_parentEffTarget; } }

    public bool             ExtreamEvading                  { get; set; } = false;    // 극한 회피 중인지
    public bool             ContinuingDash                  { get; set; } = false;    // 극한 회피 사용 후 대쉬가 진행 중인지
    public bool             UsingUltimateSkill              { get; set; } = false;    // 오의 사용 중인지
    public bool             OpponentPlayer                  { get; set; } = false;
    public int              PVPWinCount                     { get; set; } = 0;
    public bool             CancelComboAttack               { get; set; } = true;
    public bool             IsFriendChar                    { get; set; } = false;
    public TeamCharData     FriendCharData                  { get; set; } = null;
    public int              CharIndexInTower                { get; set; } = -1;
    public int              CheckMonsterDieCountForSummon   { get; set; } = 0;
    public bool             IsHelper                        { get; set; } = false;
    public bool             AutoPlay                        { get; set; } = false;
    public bool             AutoSupporterSkill              { get; set; } = false;
    public bool             AutoWeaponSkill                 { get; set; } = false;
    public bool             AutoUltimateSkill               { get; set; } = false;
	public bool             AutoGuardianSkill               { get; set; } = false;
    public float            CheckSupporterCoolTime          { get; set; } = 0.0f;

    public List<CardData>   ListEquipCard                   { get; protected set; } = new List<CardData>(); 
    public eCharAttrType    CharAttrType                    { get; protected set; } = eCharAttrType.None;
    public float            SupporterCoolTimeDecRate        { get; protected set; } = 0.0f;                 // 서포터 쿨타임 감소 비율 (0.1=1%)
    public float            DecreaseSkillCoolTimeValue      { get; protected set; } = 0.0f;                 // 캐릭터 기술 쿨타임 감소 비율 (0.1=1%)
    public ObscuredFloat    SPRegenIncRate                  { get; protected set; } = 0.0f;                 // 대마입자 충전량 증가 비율 (0.1=1%)
    //public float            CurSupporterCoolTime            { get; protected set; } = 0.0f;
    public bool             IsAutoMove                      { get; protected set; } = false;
    public List<List<Unit>> ListUnitWeaponClone             { get; protected set; } = new List<List<Unit>>();
    public bool             IsMissionStart                  { get; protected set; } = false;
    
    public int              maxCombo                        { get; private set; } = 0;
    public int              hitCount                        { get; private set; } = 0;
    public ObscuredInt      goldCount                       { get; private set; } = 0;
    public float            LifeTime                        { get; private set; } = 0.0f;
	public PlayerGuardian   Guardian                        { get; private set; } = null;
    public List<Type>       AfterActionTypeList             { get; private set; } = new List<Type>();       // 나중에 액션을 추가해 주기 위한 리스트

    protected GameTable.Character.Param     m_data                      = null;
    protected CharData                      m_charData                  = null;
    protected BOCharacter                   mBOCharacter                = null;
    protected BOSupporter                   m_boSupporter               = null;                                     // 장착한 서포터 배틀 옵션
    protected long[]                        mWeaponIds                  = new long[(int)eCOUNT.WEAPONSLOT];         // 장착한 무기 아이디
    protected WeaponData[]                  mWeaponDatas                = new WeaponData[(int)eCOUNT.WEAPONSLOT];
    protected BOWeapon[]                    mBOWeapons                  = new BOWeapon[(int)eCOUNT.WEAPONSLOT];     // 장착한 무기들 배틀 옵션
    protected int                           mCurWeaponIndex             = 0;                                        // 현재 선택된 무기 아이디 인덱스
    protected float                         mOriginalMaxHp              = 0.0f;
    protected List<ActionSelectSkillBase>   mListSelectSkill            = new List<ActionSelectSkillBase>();

    protected GameObject                    m_defaultFace;
    protected GameObject                    m_boneFace;

    protected ParticleSystem                m_effTarget                 = null;
    protected Unit                          m_parentEffTarget           = null;
    protected ParticleSystem                mEffBarricadeBlock          = null;

    protected eComboResetCondition          m_comboResetCondition       = eComboResetCondition.ByTime;
    protected int                           m_addAttackCount            = 0;

    protected BattleAreaNavigator           mBattleAreaNavigator        = null;
    protected List<EnemyNavigator>          mListEnemyNavigator         = new List<EnemyNavigator>();
    protected EnemyNavigator                mOtherObjectNavigator       = null;
    protected List<EnemyNavigator>          mListGateNavigator          = new List<EnemyNavigator>();

    protected Coroutine                     m_crChargingSP              = null;
    protected ObscuredFloat                 m_chargingSPTime            = 0.0f;

    protected int                           mPVPOrderIndex              = 0;
    protected bool                          mPVPAtkBuffed               = false;
    protected bool                          mPVPDefBuffed               = false;
    //protected Coroutine                     mCrUpdateSupporterCoolTime  = null;
    protected Vector3                       mAutoMoveDest               = Vector3.zero;
    protected Coroutine                     mCrUpdateFindPath           = null;

    protected InputEvent                    mInputEvent                 = new InputEvent();
    protected Vector3                       mInputDir                   = Vector3.zero;
    protected Vector3                       mBeforeInputDir             = Vector3.zero;

	protected float                         mAttackPowerUpRate          = 0.0f;
	protected float                         mGuardianAttackPowerRate    = 0.0f;

    private bool                                    mCheckKeepTargeting         = false;
    private float                                   mCheckKeepTargetingTime     = 0.0f;
    private List<ActionSelectSkillBase>             mTempSelectSkillList        = new List<ActionSelectSkillBase>();
    private List<BattleOption.sBattleOptionData>    mAddBuffDebuffDurationList  = new List<BattleOption.sBattleOptionData>();


    public override void Init( int tableId, eCharacterType type, string faceAniControllerPath ) {
		Deactivate();

		// 서포터 액션 전용 시스템
		m_actionSystem2 = gameObject.AddComponent<ActionSystem>();
		m_actionSystem2.Init( this );

		// 배틀옵션으로 만들어진 독립된 액션에 사용
		ActionSystem3 = gameObject.AddComponent<ActionSystem>();
		ActionSystem3.Init( this );

		base.Init( tableId, type, faceAniControllerPath );

		m_originalMass = m_rigidBody.mass;
		m_massOnFloating = m_rigidBody.mass * 1000.0f;

		if( !IsFriendChar && !OpponentPlayer ) {
			SetAction( World.Instance.TestScene || AppMgr.Instance.SceneType == AppMgr.eSceneType.None );
		}

		Activate();

		m_combo = 0;
		maxCombo = 0;
		hitCount = 0;

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP && World.Instance.StageType != eSTAGETYPE.STAGE_TOWER && !IsHelper ) {
			World.Instance.UIPlay.SetPlayer( this, m_boSupporter == null ? null : m_boSupporter.data );
		}

        //CurSupporterCoolTime = 0.0f;
        SupporterCoolingTime = 0.0f;

		Transform headSocket = m_aniEvent.GetBoneByName("Socket_head");
		if( headSocket == null ) {
			headSocket = m_aniEvent.GetBoneByName( "socket_head" );
		}

		if( headSocket ) {
			Transform[] faces = headSocket.GetComponentsInChildren<Transform>(true);
			for( int i = 0; i < faces.Length; i++ ) {
				if( !faces[i].name.Contains( "fbx" ) || faces[i].name.Contains( "ear" ) )
					continue;

				if( faces[i].name.Contains( "shy" ) ) {
					faces[i].gameObject.SetActive( false );
				}
				else {
					if( faces[i].name.Contains( "none" ) )
						m_defaultFace = faces[i].gameObject;
					else
						m_boneFace = faces[i].gameObject;
				}
			}

			EnableDefaultFace();
		}

		goldCount = 0;
		OpponentPlayer = false;
		mPVPAtkBuffed = false;
		mPVPDefBuffed = false;
		IsAutoMove = false;
		mInputEvent.sender = this;
		CheckMonsterDieCountForSummon = 0;

        AddInputController();

        if( IsHelper || AutoPlay ) {
			AddAIController( m_data.AI );
		}
	}

	public override void Deactivate()
    {
        base.Deactivate();
        Utility.StopCoroutine(World.Instance, ref m_crChargingSP);

        if ( Guardian ) {
            Guardian.actionSystem.CancelCurrentAction();
            Guardian.StopBT();
            Guardian.Deactivate();
        }
    }

    public override void GetBones()
    {
        if (m_aniEvent == null)
        {
            return;
        }

        m_aniEvent.GetBones();

        if (m_costumeUnit)
        {
            m_costumeUnit.ChangeAllWeaponName(ChangeWeaponName);
            m_aniEvent.Rebind();

            if (m_costumeUnit.CostumeBody && m_costumeUnit.CostumeBody.TexPartsColor == null)
            {
                m_aniEvent.SetMaskTexture(null);
            }
        }
    }

	public void ReInitPlayerUI( float curHp, float curSp ) {
		m_curHp = curHp;
		m_curSp = curSp;

		if( IsHelper ) {
			return;
		}

		World.Instance.UIPlay.SetPlayer( this, m_boSupporter == null ? null : m_boSupporter.data );
		World.Instance.UIPlay.InitPlayerSp( this, boWeapon == null ? 0.0f : boWeapon.GetUseWeaponSkillSp() );
		World.Instance.UIPlay.InitSupporterCoolTime( true );
	}

	public void DeleteUsedBattleOptionSet(int boSetId)
    {
        if(mBOCharacter != null)
        {
            mBOCharacter.DeleteBattleOptionSet(boSetId);
        }

        if(m_boSupporter != null)
        {
            m_boSupporter.DeleteBattleOptionSet(boSetId);
        }

        if(mBOWeapons != null && mBOWeapons.Length > 0)
        {
            for(int i = 0; i < mBOWeapons.Length; i++)
            {
                if(mBOWeapons[i] == null)
                {
                    continue;
                }

                mBOWeapons[i].DeleteBattleOptionSet(boSetId);
            }
        }
    }

	public void InitAfterEnemyMgrInit() {
		if( World.Instance.EnemyMgr && World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup > 0 ) {
			mBattleAreaNavigator = ResourceMgr.Instance.CreateFromAssetBundle<BattleAreaNavigator>( "unit", "Unit/Character/BattleAreaNavigator.prefab" );
			mBattleAreaNavigator.Init( this );

			for( int i = 0; i < World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup; i++ ) {
				AddEnemyNavigatorTarget();
			}
		}

		mEffBarricadeBlock = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>( "effect", "Effect/Background/prf_fx_barricade_block.prefab" );

		m_effTarget = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>( "effect", "Effect/UI/prf_fx_ui_monster_target.prefab" );
		m_effTarget.gameObject.SetActive( false );

		ActionSystemLoadAfterEnemyMgrInit();
	}

	public void ActionSystemLoadAfterEnemyMgrInit() {
		m_actionSystem.LoadAfterEnemyMgrInit();
		m_actionSystem2.LoadAfterEnemyMgrInit();
	}

	public void SetCharData(CharData charData)
    {
        m_charData = charData;

        if (!string.IsNullOrEmpty(m_charData.TableData.CharBOSetList))
        {
			string[] split = Utility.Split(m_charData.TableData.CharBOSetList, ',');//m_charData.TableData.CharBOSetList.Split(',');

			int[] ids = new int[split.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = int.Parse(split[i]);
            }

            mBOCharacter = new BOCharacter(ids, this);
        }
    }

    public override void SetData(int tableId)
    {
        //m_charData = GameInfo.Instance.GetMainChar();

        //190717
        //스테이지 선택창에서 선택한 캐릭터의 정보를 가지고 온다,
        //위 코드는 메인 캐릭터를 가지고와서 서포터장착여부가 메인캐릭터 기준이 된다.
        if (World.Instance.StageType == eSTAGETYPE.STAGE_TRAINING)
        {
            CharData originData = GameInfo.Instance.GetCharDataByTableID(tableId);
            m_charData = new CharData();

            m_charData.Init(originData.CUID, originData.TableID);
            for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
                m_charData.EquipCard[i] = originData.EquipCard[i];
            m_charData.EquipWeaponUID = originData.EquipWeaponUID;
            m_charData.EquipCostumeID = originData.EquipCostumeID;
            m_charData.CostumeColor = originData.CostumeColor;
            m_charData.CostumeStateFlag = originData.CostumeStateFlag;
        }
        else if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP) // PVP 모드에선 CharData 세팅 따로 해줌
        {
            if(m_charData == null)
            {
                m_charData = GameInfo.Instance.GetCharDataByTableID(tableId);
            }

            SetCharData(m_charData);
        }

        /*if (!World.Instance.isTestScene)
        {
            for (int i = 0; i < qtePassiveDatas.Length; i++)
            {
                qtePassiveDatas[i].Init();
                if (m_charData != null)
                {
                    if (m_charData.PassvieList != null && m_charData.PassvieList.Count > 0)
                    {
                        //if (m_charData.IsEquipSkill(qtePassiveDatas[i].id))
                            qtePassiveDatas[i].level = eActionLevel.Lv1;
                    }
                }
            }
        }*/

        m_data = GameInfo.Instance.GetCharacterData(tableId);
        if (m_data == null)
            return;

        m_tableId = tableId;
        m_monType = (eMonType)m_data.MonType;
        IconName = m_data.Icon;
        tableName = FLocalizeString.Instance.GetText(m_charData.TableData.Name);
        CharAttrType = (eCharAttrType)m_data.Type;

        mWeaponIds[(int)eWeaponSlot.MAIN] = m_charData.EquipWeaponUID;
        mWeaponIds[(int)eWeaponSlot.SUB] = m_charData.EquipWeapon2UID;
        mCurWeaponIndex = (int)eWeaponSlot.MAIN;

        SetStats();
        //AddQTEComponent();

        m_folder = Utility.GetFolderFromPath(m_data.Model);

        if (string.IsNullOrEmpty(m_data.FxSndArmorHit) == false)
            SoundManager.Instance.AddAudioClip("ArmorHit", "Sound/" + m_data.FxSndArmorHit, FSaveData.Instance.GetSEVolume());

        if (string.IsNullOrEmpty(m_data.FxSndArmorBreak) == false)
            SoundManager.Instance.AddAudioClip("ArmorBreak", "Sound/" + m_data.FxSndArmorBreak, FSaveData.Instance.GetSEVolume());

        if (string.IsNullOrEmpty(m_data.EffTarget) == false)
        {
            m_effTarget = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>("effect", "Effect/" + m_data.EffTarget);
            if (m_effTarget)
                m_effTarget.gameObject.SetActive(false);
        }

        m_listAniHit.Clear();
        m_listAniHit.Add(GetHitValue(m_data.Hit_01));
        m_listAniHit.Add(GetHitValue(m_data.Hit_02));
        m_listAniHit.Add(GetHitValue(m_data.Hit_03));
        m_listAniHit.Add(GetHitValue(m_data.Hit_04));

        // 배치된 서포터가 한장이라도 있으면 서포터 배틀 옵션 세팅
        ListEquipCard.Clear();
        bool created = false;
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            if (m_charData.EquipCard[i] > (int)eCOUNT.NONE)
            {
                if (!created)
                {
                    m_boSupporter = new BOSupporter(m_charData, this);
                    created = true;
                }

                CardData cardData = GameInfo.Instance.GetCardData(m_charData.EquipCard[i]);
                if(cardData != null)
                {
                    ListEquipCard.Add(cardData);
                }
            }
        }

        for (int i = 0; i < mWeaponIds.Length; i++)
        {
            mBOWeapons[i] = null;
            mWeaponDatas[i] = null;

            if (mWeaponIds[i] <= 0)
            {
                continue;
            }

            mBOWeapons[i] = new BOWeapon(mWeaponIds[i], this);
            mWeaponDatas[i] = GameInfo.Instance.GetWeaponData(mWeaponIds[i]);
        }
    }

	public void SetGuardian( PlayerGuardian playerGuardian ) {
		Guardian = playerGuardian;
	}

    public void AddBOCharBattleOptionSet(int boSetId, int level)
    {
        if(boSetId <= 0)
        {
            return;
        }

        if (mBOCharacter == null)
        {
            mBOCharacter = new BOCharacter(boSetId, this);
        }
        else
        {
            mBOCharacter.AddBattleOptionSet(boSetId, level);
        }
    }

    protected override float GetMaxHp()
    {
        float maxHp = GameSupport.GetMaxHp(m_charData, eWeaponSlot.MAIN);
        //maxHp += (maxHp * mAddMaxHpRate);

        /*
#if ARENA_TOWER
        if(World.Instance.StageType == eSTAGETYPE.STAGE_TOWER)
        {
            maxHp += (maxHp * GameSupport.GetAddHpFromCardFormations());
        }
#endif
*/

        if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
        {
            maxHp *= GameInfo.Instance.BattleConfig.ArenaHPRate;
        }

        mOriginalMaxHp = maxHp;
        return maxHp;
    }

    private float GetMaxHp(WeaponData mainWeaponData, WeaponData subWeaponData, List<CardData> listCard, 
                           List<GemData> mainGemData, List<GemData> subGemData, float teamHp)
    {
        float maxHp = GameSupport.GetMaxHp(m_charData, mainWeaponData, subWeaponData, listCard, mainGemData, subGemData, teamHp);
        maxHp += (maxHp * mAddMaxHpRate);

        if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
        {
            maxHp *= GameInfo.Instance.BattleConfig.ArenaHPRate;
        }

        return maxHp;
    }

    public void AddMaxHp(float addRate)
    {
        m_maxHp += m_maxHp * addRate;
        m_curHp = m_maxHp;
    }

    public void AddAttackPower(float addRate)
    {
        m_attackPower += m_attackPower * addRate;
    }

    public void AddDefenceRate(float addRate)
    {
        m_defenceRate += m_defenceRate * addRate;
    }

    public void AddCriticalRate(float addRate)
    {
        m_criticalRate += m_criticalRate * addRate;
    }

    public void AddMaxHp(float originalMaxHp, float addRate)
    {
        m_maxHp += originalMaxHp * addRate;
        m_curHp = m_maxHp;

        mOriginalMaxHp = m_maxHp;
    }

    public void AddAttackPower(float originalAttackPower, float addRate)
    {
        m_attackPower += originalAttackPower * addRate;
    }

    public void AddDefenceRate(float originalDefenceRate, float addRate)
    {
        m_defenceRate += originalDefenceRate * addRate;
    }

    public void AddCriticalRate(float originalCriticalRate, float addRate)
    {
        m_criticalRate += originalCriticalRate * addRate;
    }

    public void SetDataForPVP(CharData charData)
    {
        m_charData = charData;

        m_data = GameInfo.Instance.GetCharacterData(m_charData.TableID);
        if (m_data == null)
            return;

        m_tableId = tableId;
        IconName = m_data.Icon;
        tableName = FLocalizeString.Instance.GetText(m_charData.TableData.Name);

        mWeaponIds[(int)eWeaponSlot.MAIN] = m_charData.EquipWeaponUID;
        mWeaponIds[(int)eWeaponSlot.SUB] = m_charData.EquipWeapon2UID;
        mCurWeaponIndex = (int)eWeaponSlot.MAIN;

        SetStats();

        m_folder = Utility.GetFolderFromPath(m_data.Model);

        if (string.IsNullOrEmpty(m_data.FxSndArmorHit) == false)
            SoundManager.Instance.AddAudioClip("ArmorHit", "Sound/" + m_data.FxSndArmorHit, FSaveData.Instance.GetSEVolume());

        if (string.IsNullOrEmpty(m_data.FxSndArmorBreak) == false)
            SoundManager.Instance.AddAudioClip("ArmorBreak", "Sound/" + m_data.FxSndArmorBreak, FSaveData.Instance.GetSEVolume());

        if (string.IsNullOrEmpty(m_data.EffTarget) == false)
        {
            m_effTarget = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>("effect", "Effect/" + m_data.EffTarget);
            if (m_effTarget)
                m_effTarget.gameObject.SetActive(false);
        }

        m_listAniHit.Clear();
        m_listAniHit.Add(GetHitValue(m_data.Hit_01));
        m_listAniHit.Add(GetHitValue(m_data.Hit_02));
        m_listAniHit.Add(GetHitValue(m_data.Hit_03));
        m_listAniHit.Add(GetHitValue(m_data.Hit_04));
    }

    private void SetStats()
    {
        if (!isClone && World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
        {
            m_maxHp = GetMaxHp();//GameSupport.GetMaxHp(m_charData, eWeaponSlot.MAIN);
            m_curHp = m_maxHp;

            m_attackPower = GameSupport.GetAttackPower(m_charData, eWeaponSlot.MAIN);
            m_defenceRate = GameSupport.GetDefenceRate(m_charData, eWeaponSlot.MAIN);
            m_criticalRate = GameSupport.GetCriticalRate(m_charData, eWeaponSlot.MAIN);

            m_curSp = GameInfo.Instance.BattleConfig.USInitSP;

            m_maxShield = 0;
            m_curShield = m_maxShield;

            if (m_curShield > 0.0f)
            {
                mChangedSuperArmorId = SetSuperArmor(eSuperArmor.Lv2);
            }
            else
            {
                mChangedSuperArmorId = SetSuperArmor(eSuperArmor.None);
            }

            m_originalSpeed = m_data.MoveSpeed;
            m_curSpeed = m_originalSpeed;
        }
        else if (isClone)
        {
            m_maxHp = m_cloneOwner.maxHp;
            m_curHp = m_cloneOwner.curHp;

            m_maxShield = m_cloneOwner.maxShield;
            m_curShield = m_cloneOwner.curShield;

            mChangedSuperArmorId = SetSuperArmor(eSuperArmor.Invincible);

            m_originalSpeed = m_cloneOwner.speed;
            m_curSpeed = m_cloneOwner.speed;

            m_attackPower = m_cloneOwner.attackPower;
            m_defenceRate = m_cloneOwner.defenceRate;
            m_criticalRate = m_cloneOwner.criticalRate;
        }
    }

    public void SetStatsForPVPPlayer(int orderIndex)
    {
        m_maxHp = GetMaxHp();//GameSupport.GetMaxHp(m_charData, eWeaponSlot.MAIN);
        m_curHp = m_maxHp;

        m_attackPower = GameSupport.GetAttackPower(m_charData, eWeaponSlot.MAIN);
        m_defenceRate = GameSupport.GetDefenceRate(m_charData, eWeaponSlot.MAIN);
        m_criticalRate = GameSupport.GetCriticalRate(m_charData, eWeaponSlot.MAIN);

        m_curSp = GameInfo.Instance.BattleConfig.USInitSP;

        m_maxShield = 0.0f;
        m_curShield = m_maxShield;

        if (m_curShield > 0.0f)
        {
            mChangedSuperArmorId = SetSuperArmor(eSuperArmor.Lv2);
        }
        else
        {
            mChangedSuperArmorId = SetSuperArmor(eSuperArmor.None);
        }

        m_originalSpeed = m_data.MoveSpeed;
        m_curSpeed = m_originalSpeed;

        // 배치된 서포터가 한장이라도 있으면 서포터 배틀 옵션 세팅
        ListEquipCard.Clear();
        bool created = false;
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            if (m_charData.EquipCard[i] > (int)eCOUNT.NONE)
            {
                if (!created)
                {
                    m_boSupporter = new BOSupporter(m_charData, this);
                    created = true;
                }

                CardData cardData = GameInfo.Instance.GetCardData(m_charData.EquipCard[i]);
                if (cardData != null)
                {
                    ListEquipCard.Add(cardData);
                }

                break;
            }
        }

        for (int i = 0; i < mWeaponIds.Length; i++)
        {
            mBOWeapons[i] = null;

            if (mWeaponIds[i] <= 0)
            {
                continue;
            }

            mBOWeapons[i] = new BOWeapon(mWeaponIds[i], this);
        }

        mPVPOrderIndex = orderIndex;
    }

    public void Set2ndStatsForPVPPlayer()
    {
        m_2ndStats.RemoveStatByAddType(Unit2ndStatsTable.eAddType.Weapon);
        m_2ndStats.RemoveStatByAddType(Unit2ndStatsTable.eAddType.Skill);

        sprintStartTimeRate = 1.0f;
        onlyLastAttack = false;
        lastAttackKnockBack = false;
        SupporterCoolTimeDecRate = 0.0f;
        DecreaseSkillCoolTimeValue = 0.0f;
        SPRegenIncRate = 0.0f;
        IncreaseSkillAtkValue = 0.0f;
        m_fixCriticalRate = 1.0f;
        IncreaseSummonsAttackPowerRate = 0.0f;
        Penetrate = 0.0f;
        Sufferance = 0.0f;

        // 무기에 장착된 곡옥 스탯 추가
        WeaponData weaponData = GameInfo.Instance.GetWeaponData(mWeaponIds[mCurWeaponIndex]);
        for (int i = 0; i < weaponData.SlotGemUID.Length; i++)
        {
            if (weaponData.SlotGemUID[i] <= (int)eCOUNT.NONE)
                continue;

            GemData gemData = GameInfo.Instance.GetGemData(weaponData.SlotGemUID[i]);
            if (gemData == null)
                continue;

            for (int j = 0; j < gemData.RandOptID.Length; j++)
            {
                if (gemData.RandOptID[j] <= 0)
                    continue;

                GameTable.GemRandOpt.Param param = GameInfo.Instance.GameTable.FindGemRandOpt(x => x.GroupID == gemData.TableData.RandOptGroup && x.ID == gemData.RandOptID[j]);
                if (param == null)
                    continue;

                float value = (param.Min + (int)((float)gemData.RandOptValue[j] * param.Value)) / (float)eCOUNT.MAX_RATE_VALUE;
                if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
                {
                    value *= GameInfo.Instance.BattleConfig.ArenaGemOptRate;
                }

                string str = param.EffectType;
				string[] splits = Utility.Split(str, '_'); //str.Split('_');
                Unit2ndStatsTable.eSubjectType subjectType = Unit2ndStatsTable.GetSubjectType(splits[1]);
                if (subjectType == Unit2ndStatsTable.eSubjectType.HpUp)
                {
                    m_maxHp += m_maxHp * value;
                    m_curHp = m_maxHp;
                }
                else if (subjectType == Unit2ndStatsTable.eSubjectType.AtkUp)
                {
                    m_attackPower += m_attackPower * value;
                }
                else if (subjectType == Unit2ndStatsTable.eSubjectType.DefUp)
                {   // 방어율과 치명률은 절대비율 합연산(즉, 방어율 10%에서 5% 증가면 15%)
                    m_defenceRate += (value * (float)eCOUNT.MAX_PROBABILITY);
                }
                else if (subjectType == Unit2ndStatsTable.eSubjectType.CriUp)
                {   // 방어율과 치명률은 절대비율 합연산(즉, 방어율 10%에서 5% 증가면 15%)
                    m_criticalRate += (value * (float)eCOUNT.MAX_PROBABILITY);
                }
                else if (subjectType == Unit2ndStatsTable.eSubjectType.CoolTime)
                {
                    DecreaseSkillCoolTimeValue += value;
                }
                else if (subjectType == Unit2ndStatsTable.eSubjectType.SkillAtk)
                {
                    IncreaseSkillAtkValue += value;
                }
                else if (subjectType == Unit2ndStatsTable.eSubjectType.Penetrate)
                {
                    value /= 10.0f;
                    Penetrate += value;
                }
                else if (subjectType == Unit2ndStatsTable.eSubjectType.Sufferance)
                {
                    value /= 10.0f;
                    Sufferance += value;
                }

                if( splits.Length > 2 ) {
                    Unit2ndStatsTable.eIncreaseType increaseType = Unit2ndStatsTable.GetIncreaseType(splits[2]);
                    int statValue = param.Min + (int)((float)gemData.RandOptValue[j] * param.Value);

                    m_2ndStats.AddStat( Unit2ndStatsTable.eAddType.Weapon, subjectType, increaseType, statValue );
                }
            }
        }

        SetSkillStats();

        OriginalPenetrate = Penetrate;
        OriginalSufferance = Sufferance;

        // 친구와 대전하기가 아니면
        if (!UIValue.Instance.ContainsKey(UIValue.EParamType.IsFriendPVP))
        {
            // PVP 공격력 증가 물약
            if (GameInfo.Instance.ArenaATK_Buff_Flag && !mPVPAtkBuffed)
            {
                m_attackPower += (m_attackPower * GameInfo.Instance.BattleConfig.ArenaAtkBuffRate);
                mPVPAtkBuffed = true;
            }

            // PVP 체력 증가 물약
            if (GameInfo.Instance.ArenaDEF_Buff_Flag && !mPVPDefBuffed)
            {
                m_maxHp += (m_maxHp * GameInfo.Instance.BattleConfig.ArenaDefBuffRate);
                m_curHp = m_maxHp;
                mPVPDefBuffed = true;
            }
        }

        mOriginalMaxHp = m_maxHp;
    }

    public void SetStatsForPVPOpponent(List<CardData> listCard, WeaponData mainWeaponData, WeaponData subWeaponData, 
                                       List<GemData> mainGemData, List<GemData> subGemData, int orderIndex, float teamHp, float teamAtk)
    {
        m_maxHp = GetMaxHp(mainWeaponData, subWeaponData, listCard, mainGemData, subGemData, teamHp);//GameSupport.GetMaxHp(m_charData, mainWeaponData, subWeaponData, listCard);
        m_curHp = m_maxHp;

        m_attackPower = GameSupport.GetAttackPower(m_charData, mainWeaponData, subWeaponData, mainGemData, subGemData, teamAtk);
        m_defenceRate = GameSupport.GetDefenceRate(m_charData, mainWeaponData, subWeaponData, listCard, mainGemData, subGemData);
        m_criticalRate = GameSupport.GetCriticalRate(m_charData, mainWeaponData, subWeaponData, mainGemData, subGemData);

        m_curSp = GameInfo.Instance.BattleConfig.USInitSP;

        m_maxShield = 0.0f;
        m_curShield = m_maxShield;

        if (m_curShield > 0.0f)
        {
            mChangedSuperArmorId = SetSuperArmor(eSuperArmor.Lv2);
        }
        else
        {
            mChangedSuperArmorId = SetSuperArmor(eSuperArmor.None);
        }

        m_originalSpeed = m_data.MoveSpeed;
        m_curSpeed = m_originalSpeed;

        if (listCard != null && listCard.Count > 0 && listCard[0] != null)
        {
            m_boSupporter = new BOSupporter(listCard, this);
        }

        ListEquipCard.Clear();
        for (int i = 0; i < listCard.Count; i++)
        {
            if(listCard[i] == null)
            {
                continue;
            }

            ListEquipCard.Add(listCard[i]);
        }

        mWeaponDatas[0] = mainWeaponData;
        mWeaponDatas[1] = subWeaponData;

        for (int i = 0; i < mWeaponIds.Length; i++)
        {
            mBOWeapons[i] = null;
        }

        if (mainWeaponData != null)
        {
            mBOWeapons[0] = new BOWeapon(mainWeaponData, this);
        }

        if(subWeaponData != null)
        {
            mBOWeapons[1] = new BOWeapon(subWeaponData, this);
        }

        mPVPOrderIndex = orderIndex;
    }

    public void Set2ndStatsForPVPOpponent(List<GemData> listGem)
    {
        m_2ndStats.RemoveStatByAddType(Unit2ndStatsTable.eAddType.Weapon);
        m_2ndStats.RemoveStatByAddType(Unit2ndStatsTable.eAddType.Skill);

        sprintStartTimeRate = 1.0f;
        onlyLastAttack = false;
        lastAttackKnockBack = false;
        SupporterCoolTimeDecRate = 0.0f;
        DecreaseSkillCoolTimeValue = 0.0f;
        SPRegenIncRate = 0.0f;
        IncreaseSkillAtkValue = 0.0f;
        m_fixCriticalRate = 1.0f;
        IncreaseSummonsAttackPowerRate = 0.0f;
        Penetrate = 0.0f;
        Sufferance = 0.0f;

        // 무기에 장착된 곡옥 스탯 추가
        if (listGem != null)
        {
            for (int i = 0; i < listGem.Count; i++)
            {
                GemData gemData = listGem[i];
                if (gemData == null)
                {
                    continue;
                }

                for (int j = 0; j < gemData.RandOptID.Length; j++)
                {
                    if (gemData.RandOptID[j] <= 0)
                    {
                        continue;
                    }

                    GameTable.GemRandOpt.Param param = GameInfo.Instance.GameTable.FindGemRandOpt(x => x.GroupID == gemData.TableData.RandOptGroup && x.ID == gemData.RandOptID[j]);
                    if (param == null)
                    {
                        continue;
                    }

                    float value = (param.Min + (int)((float)gemData.RandOptValue[j] * param.Value)) / (float)eCOUNT.MAX_RATE_VALUE;
                    if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
                    {
                        value *= GameInfo.Instance.BattleConfig.ArenaGemOptRate;
                    }

                    string str = param.EffectType;
					string[] splits = Utility.Split(str, '_'); //str.Split('_');
                    Unit2ndStatsTable.eSubjectType subjectType = Unit2ndStatsTable.GetSubjectType(splits[1]);
                    if (subjectType == Unit2ndStatsTable.eSubjectType.HpUp)
                    {
                        m_maxHp += m_maxHp * value;
                        m_curHp = m_maxHp;
                    }
                    else if (subjectType == Unit2ndStatsTable.eSubjectType.AtkUp)
                    {
                        m_attackPower += m_attackPower * value;
                    }
                    else if (subjectType == Unit2ndStatsTable.eSubjectType.DefUp)
                    {   // 방어율과 치명률은 절대비율 합연산(즉, 방어율 10%에서 5% 증가면 15%)
                        m_defenceRate += (value * (float)eCOUNT.MAX_PROBABILITY);
                    }
                    else if (subjectType == Unit2ndStatsTable.eSubjectType.CriUp)
                    {   // 방어율과 치명률은 절대비율 합연산(즉, 방어율 10%에서 5% 증가면 15%)
                        m_criticalRate += (value * (float)eCOUNT.MAX_PROBABILITY);
                    }
                    else if (subjectType == Unit2ndStatsTable.eSubjectType.CoolTime)
                    {
                        DecreaseSkillCoolTimeValue += value;
                    }
                    else if (subjectType == Unit2ndStatsTable.eSubjectType.SkillAtk)
                    {
                        IncreaseSkillAtkValue += value;
                    }
                    else if (subjectType == Unit2ndStatsTable.eSubjectType.Penetrate)
                    {
                        value /= 10.0f;
                        Penetrate += value;
                    }
                    else if (subjectType == Unit2ndStatsTable.eSubjectType.Sufferance)
                    {
                        value /= 10.0f;
                        Sufferance += value;
                    }

                    if( splits.Length > 2 ) {
                        Unit2ndStatsTable.eIncreaseType increaseType = Unit2ndStatsTable.GetIncreaseType(splits[2]);
                        int statValue = param.Min + (int)((float)gemData.RandOptValue[j] * param.Value);

                        m_2ndStats.AddStat( Unit2ndStatsTable.eAddType.Weapon, subjectType, increaseType, statValue );
                    }
                }
            }
        }

        SetSkillStats();

        OriginalPenetrate = Penetrate;
        OriginalSufferance = Sufferance;

        // 친구와 대전하기가 아니면
        if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP && !UIValue.Instance.ContainsKey(UIValue.EParamType.IsFriendPVP))
        {
            // 승급전이 아닌 경우 연승에 의한 체력, 공격력 증감
            if (GameInfo.Instance.UserBattleData.Now_PromotionRemainCnt <= 0)
            {
                float fRate = Mathf.Max(GameInfo.Instance.BattleConfig.ArenaWinLoseBuffRate * GameInfo.Instance.UserBattleData.Now_WinLoseCnt, -0.99f);
                m_maxHp += (m_maxHp * fRate);
                m_curHp = m_maxHp;
                m_attackPower += (m_attackPower * fRate);
            }
        }

        mOriginalMaxHp = m_maxHp;
    }

    private void SetSkillStats()
    {
        // 장착한 스킬 스탯 추가
        for (int i = 0; i < m_charData.PassvieList.Count; i++)
        {
            int skillid = m_charData.PassvieList[i].SkillID;

            var tabledata = GameInfo.Instance.GameTable.FindCharacterSkillPassive(skillid);
            if (tabledata == null)
                continue;

            if (tabledata.Type != (int)eCHARSKILLPASSIVETYPE.UPGRADE_NORMAL && tabledata.Type != (int)eCHARSKILLPASSIVETYPE.UPGRADE_ULTIMATE)
                continue;

            int level = m_charData.PassvieList[i].SkillLevel;
            float incValue = (level - 1) * tabledata.IncValue1;
            float value = (tabledata.Value1 + incValue) / (float)eCOUNT.MAX_RATE_VALUE;
            if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
            {
                value *= GameInfo.Instance.BattleConfig.ArenaCharSkillOptRate;
            }

            string str = tabledata.Effect;
			string[] splits = Utility.Split(str, '_'); //str.Split('_');
            Unit2ndStatsTable.eSubjectType subjectType = Unit2ndStatsTable.GetSubjectType(splits[1]);
            if (subjectType == Unit2ndStatsTable.eSubjectType.HpUp)
            {
                m_maxHp += m_maxHp * value;
                m_curHp = m_maxHp;
            }
            else if (subjectType == Unit2ndStatsTable.eSubjectType.AtkUp)
            {
                m_attackPower += m_attackPower * value;
            }
            else if (subjectType == Unit2ndStatsTable.eSubjectType.DefUp)
            {   // 방어율과 치명률은 절대비율 합연산(즉, 방어율 10%에서 5% 증가면 15%)
                m_defenceRate += (value * (float)eCOUNT.MAX_PROBABILITY);
            }
            else if (subjectType == Unit2ndStatsTable.eSubjectType.CriUp)
            {   // 방어율과 치명률은 절대비율 합연산(즉, 방어율 10%에서 5% 증가면 15%)
                m_criticalRate += (value * (float)eCOUNT.MAX_PROBABILITY);
            }
            else if (subjectType == Unit2ndStatsTable.eSubjectType.CoolTime)
            {
                DecreaseSkillCoolTimeValue += value;
            }
            else if (subjectType == Unit2ndStatsTable.eSubjectType.SkillAtk)
            {
                IncreaseSkillAtkValue += value;
            }
            else if (subjectType == Unit2ndStatsTable.eSubjectType.Penetrate)
            {
                value /= 10.0f;
                Penetrate += value;
            }
            else if (subjectType == Unit2ndStatsTable.eSubjectType.Sufferance)
            {
                value /= 10.0f;
                Sufferance += value;
            }
            else if( splits.Length > 2 )
            {
                Unit2ndStatsTable.eIncreaseType increaseType = Unit2ndStatsTable.GetIncreaseType(splits[2]);
                int statValue = (int)(tabledata.Value1 + (tabledata.IncValue1 * (m_charData.PassvieList[i].SkillLevel - 1)));

                m_2ndStats.AddStat(Unit2ndStatsTable.eAddType.Skill, subjectType, increaseType, statValue);
            }
        }
    }

	public virtual void SetAction( bool usePassiveSkillList ) {
        ActionBase action = null;

        if( m_actionSystem.GetAction( eActionCommand.Die ) == null ) {
			m_actionSystem.AddAction( gameObject.AddComponent<ActionPlayerDie>(), 0, null );

            action = m_actionSystem.GetAction( eActionCommand.Die );
            if( action ) {
                action.OnEndCallback = Deactivate;
            }
        }

		action = m_actionSystem.GetAction(eActionCommand.Hit);
		action.OnEndCallback = OnEndHit;

		mListSelectSkill.Clear();
		int uskillId = (m_charData.TableID * 1000) + 301;

		// 선택한 스킬로 공격 액션들 세팅
		GameTable.CharacterSkillPassive.Param paramParent = null;
		GameTable.CharacterSkillPassive.Param param = null;

		if( !usePassiveSkillList ) {
			for( int i = 0; i < (int)eCOUNT.SKILLSLOT; i++ ) {
				paramParent = GameInfo.Instance.GameTable.FindCharacterSkillPassive( m_charData.EquipSkill[i] );
                if( paramParent == null || string.IsNullOrEmpty( paramParent.SkillAction ) ) {
                    continue;
                }

				param = paramParent;

				List<GameTable.CharacterSkillPassive.Param> listCondSkill = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.ParentsID == param.ID && x.Type == (int)eCHARSKILLPASSIVETYPE.CONDITION_SKILL);
				if( listCondSkill != null && listCondSkill.Count > 0 ) {
					for( int j = 0; j < listCondSkill.Count; j++ ) {
						if( !GameSupport.IsCharSkillCond( m_charData, listCondSkill[j] ) ) {
							listCondSkill.Remove( listCondSkill[j] );
							--j;
						}
					}
				}

				if( listCondSkill.Count > 0 ) {
					param = listCondSkill[listCondSkill.Count - 1];
					listCondSkill.Remove( param );

					listCondSkill.Add( paramParent );
				}

				if( !AddSelectSkillAction( param, listCondSkill ) ) {
					continue;
				}
			}

			// 오의도 추가
			param = GameInfo.Instance.GameTable.FindCharacterSkillPassive( uskillId );
			AddSelectSkillAction( param, null );
		}
		else { // 테스트 시
			if( m_charData.PassvieList.Find( x => x.TableData.ID == uskillId ) == null ) {
				m_charData.PassvieList.Add( new PassiveData( uskillId, 1 ) ); // 오의
			}

			for( int i = 0; i < m_charData.PassvieList.Count; i++ ) {
				paramParent = m_charData.PassvieList[i].TableData;
				if( paramParent == null || string.IsNullOrEmpty( paramParent.SkillAction ) )
					continue;

				param = paramParent;

				List<GameTable.CharacterSkillPassive.Param> listCondSkill = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.ParentsID == param.ID && x.Type == (int)eCHARSKILLPASSIVETYPE.CONDITION_SKILL);
				if( listCondSkill != null && listCondSkill.Count > 0 ) {
					for( int j = 0; j < listCondSkill.Count; j++ ) {
						if( !GameSupport.IsCharSkillCond( m_charData, listCondSkill[j] ) ) {
							listCondSkill.Remove( listCondSkill[j] );
							--j;
						}
					}
				}

				if( listCondSkill.Count > 0 ) {
					param = listCondSkill[listCondSkill.Count - 1];
					listCondSkill.Remove( param );

					listCondSkill.Add( paramParent );
				}

				if( !AddSelectSkillAction( param, listCondSkill ) ) {
					continue;
				}

				for ( int index = 0; index < (int)eCOUNT.WEAPONSLOT; index++ ) {
					AddGuardianSkillAction( param, listCondSkill, index );
				}
			}
		}
	}

	public virtual void SetAction(bool usePassiveSkillList, List<CardData> listEquipCardData)
    {
        if (m_actionSystem.GetAction(eActionCommand.Die) == null)
        {
            m_actionSystem.AddAction(gameObject.AddComponent<ActionPlayerDie>(), 0, null);
        }

        ActionBase action = m_actionSystem.GetAction(eActionCommand.Hit);
        action.OnEndCallback = OnEndHit;

        mListSelectSkill.Clear();
        int uskillId = (m_charData.TableID * 1000) + 301;

        // 선택한 스킬로 공격 액션들 세팅
        GameTable.CharacterSkillPassive.Param paramParent = null;
        GameTable.CharacterSkillPassive.Param param = null;
        if (!usePassiveSkillList)
        {
            for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
            {
                paramParent = GameInfo.Instance.GameTable.FindCharacterSkillPassive(m_charData.EquipSkill[i]);
                if (paramParent == null || string.IsNullOrEmpty(paramParent.SkillAction))
                    continue;

                param = paramParent;

                List<GameTable.CharacterSkillPassive.Param> listCondSkill = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.ParentsID == param.ID && x.Type == (int)eCHARSKILLPASSIVETYPE.CONDITION_SKILL);
                if (listCondSkill != null && listCondSkill.Count > 0)
                {
                    for (int j = 0; j < listCondSkill.Count; j++)
                    {
                        if (!GameSupport.IsCharSkillCondWithTableData(m_charData, listEquipCardData, listCondSkill[j]))
                        {
                            listCondSkill.Remove(listCondSkill[j]);
                            --j;
                        }
                    }
                }

                if (listCondSkill.Count > 0)
                {
                    param = listCondSkill[listCondSkill.Count - 1];
                    listCondSkill.Remove(param);

                    listCondSkill.Add(paramParent);
                }

                if (!AddSelectSkillAction(param, listCondSkill))
                {
                    continue;
                }
            }

            // 오의도 추가
            param = GameInfo.Instance.GameTable.FindCharacterSkillPassive(uskillId);
            AddSelectSkillAction(param, null);
        }
        else
        {
            if (m_charData.PassvieList.Find(x => x.TableData.ID == uskillId) == null)
            {
                m_charData.PassvieList.Add(new PassiveData(uskillId, 1)); // 오의
            }

            for (int i = 0; i < m_charData.PassvieList.Count; i++)
            {
                paramParent = m_charData.PassvieList[i].TableData;
                if (paramParent == null || string.IsNullOrEmpty(paramParent.SkillAction))
                    continue;

                param = paramParent;

                List<GameTable.CharacterSkillPassive.Param> listCondSkill = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.ParentsID == param.ID && x.Type == (int)eCHARSKILLPASSIVETYPE.CONDITION_SKILL);
                if (listCondSkill != null && listCondSkill.Count > 0)
                {
                    for (int j = 0; j < listCondSkill.Count; j++)
                    {
                        if (!GameSupport.IsCharSkillCondWithTableData(m_charData, listEquipCardData, listCondSkill[j]))
                        {
                            listCondSkill.Remove(listCondSkill[j]);
                            --j;
                        }
                    }
                }

                if (listCondSkill.Count > 0)
                {
                    param = listCondSkill[listCondSkill.Count - 1];
                    listCondSkill.Remove(param);

                    listCondSkill.Add(paramParent);
                }

                if (!AddSelectSkillAction(param, listCondSkill))
                {
                    continue;
                }
            }
        }
    }

    protected bool AddSelectSkillAction(GameTable.CharacterSkillPassive.Param param, List<GameTable.CharacterSkillPassive.Param> listAddParam)
    {
        ActionBase action = null;
        Type type = null;
        string[] split = null;

        if (!string.IsNullOrEmpty(param.ReplacedAction))
        {
            type = Type.GetType("Action" + param.ReplacedAction);
            if (type == null)
            {
                Debug.LogError(param.ReplacedAction + "액션을 찾을 수 없습니다.");
                return false;
            }

            action = GetComponent(type) as ActionBase;
            if (action)
            {
                m_actionSystem.RemoveAction(action.actionCommand);
                DestroyImmediate(action);
            }
        }

		split = Utility.Split(param.SkillAction, ','); //param.SkillAction.Split(',');
		if (split.Length <= 0)
            return false;

        ActionSelectSkillBase actionSelectSkill = null;
        ActionSelectSkillBase beforeAction = null;

        for (int j = 0; j < split.Length; j++)
        {
            type = Type.GetType("Action" + split[j]);
            action = gameObject.AddComponent(type) as ActionBase;

            actionSelectSkill = action as ActionSelectSkillBase;

            if (actionSelectSkill)
            {
                actionSelectSkill.IsLastSkill = true;
            }

            if (split.Length > 1)
            {
                actionSelectSkill.Index = j;

                if(j < (split.Length - 1))
                {
                    actionSelectSkill.IsLastSkill = false;
                }

                if (beforeAction)
                {
                    beforeAction.Child = actionSelectSkill;
                }
            }

            //action.LoadCameraAni(param.CameraAni);
            m_actionSystem.AddAction(action, param.ID, listAddParam);

            if (actionSelectSkill != null && actionSelectSkill.Index == 0)
            {
                mListSelectSkill.Add(actionSelectSkill);
            }

            beforeAction = actionSelectSkill;
        }

        return true;
    }

	public void SetOpponentPlayer( string aiFileName ) {
		OpponentPlayer = true;
		AddAIController( aiFileName );
	}

	public virtual void ChangeWeapon( bool init ) {
		int beforeIndex = mCurWeaponIndex;
		float nowHpPer = 1.0f;

		mAddMaxHpRate = 0.0f;

		if( !init ) {
			nowHpPer = m_curHp / m_maxHp;

			int index = beforeIndex + 1;
			if( index >= mWeaponDatas.Length ) {
				index = 0;
			}

			if( mWeaponDatas[index] == null ) {
				return;
			}

			mCurWeaponIndex = index;

			if( !IsFriendChar ) {
				m_maxHp = GetMaxHp();
			}
			else {
				float addCardFormationHpRate = GameSupport.GetTotalCardFormationEffectValue() / 100.0f;

				m_maxHp = GetMaxHp( FriendCharData.MainWeaponData, FriendCharData.SubWeaponData, FriendCharData.CardList,
								    FriendCharData.MainGemList, FriendCharData.SubGemList, addCardFormationHpRate );
			}

			if( !init && World.Instance.StageType == eSTAGETYPE.STAGE_TOWER ) {
				WorldStage worldStage = World.Instance as WorldStage;
				if( worldStage ) {
					worldStage.SetBadgeStats( worldStage.ListTempPlayerInTower.ToArray(), GameSupport.GetEquipTowerBadgeList() );
				}
			}

			mOriginalMaxHp = m_maxHp;

			m_attackPower = GameSupport.GetAttackPower( m_charData, (eWeaponSlot)mCurWeaponIndex );
			m_defenceRate = GameSupport.GetDefenceRate( m_charData, (eWeaponSlot)mCurWeaponIndex );
			m_criticalRate = GameSupport.GetCriticalRate( m_charData, (eWeaponSlot)mCurWeaponIndex );

            StartWeaponChange( beforeIndex );
			EffectManager.Instance.Play( this, 60001, EffectManager.eType.Common );
		}
        else if( World.Instance.StageType == eSTAGETYPE.STAGE_RAID ) {
            nowHpPer = charData.RaidHpPercentage / 100.0f;
        }

		m_2ndStats.RemoveStatByAddType( Unit2ndStatsTable.eAddType.Weapon );
		m_2ndStats.RemoveStatByAddType( Unit2ndStatsTable.eAddType.Skill );

		sprintStartTimeRate = 1.0f;
		onlyLastAttack = false;
		lastAttackKnockBack = false;
		SupporterCoolTimeDecRate = 0.0f;
		DecreaseSkillCoolTimeValue = 0.0f;
		SPRegenIncRate = 0.0f;
		IncreaseSkillAtkValue = 0.0f;
		m_fixCriticalRate = 1.0f;
		IncreaseSummonsAttackPowerRate = 0.0f;
        Penetrate = 0.0f;
        Sufferance = 0.0f;

		List<ActionSelectSkillBase> listSelectSkill = m_actionSystem.GetActionList<ActionSelectSkillBase>();
		for( int i = 0; i < listSelectSkill.Count; ++i ) {
			listSelectSkill[i].SetIncreaseCoolingTimeBySec( 0.0f );
		}

		// 무기에 장착된 곡옥 스탯 추가
		WeaponData weaponData = mWeaponDatas[mCurWeaponIndex];
		for( int i = 0; i < weaponData.SlotGemUID.Length; i++ ) {
            if( weaponData.SlotGemUID[i] <= (int)eCOUNT.NONE ) {
                continue;
            }

			GemData gemData = GameInfo.Instance.GetGemData( weaponData.SlotGemUID[i] );
            if( gemData == null ) {
                continue;
            }

			for( int j = 0; j < gemData.RandOptID.Length; j++ ) {
                if( gemData.RandOptID[j] <= 0 ) {
                    continue;
                }

				GameTable.GemRandOpt.Param param = GameInfo.Instance.GameTable.FindGemRandOpt(x => x.GroupID == gemData.TableData.RandOptGroup && 
                                                                                                   x.ID == gemData.RandOptID[j]);
                if( param == null ) {
                    continue;
                }

				float value = ( param.Min + (int)( (float)gemData.RandOptValue[j] * param.Value ) ) / (float)eCOUNT.MAX_RATE_VALUE;
				if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
					value *= GameInfo.Instance.BattleConfig.ArenaGemOptRate;
				}

				string str = param.EffectType;
				string[] splits = Utility.Split(str, '_');
				Unit2ndStatsTable.eSubjectType subjectType = Unit2ndStatsTable.GetSubjectType( splits[1] );

				if( subjectType == Unit2ndStatsTable.eSubjectType.HpUp ) {
					m_maxHp += m_maxHp * value;
					m_curHp = m_maxHp;
				}
				else if( subjectType == Unit2ndStatsTable.eSubjectType.AtkUp ) {
					m_attackPower += m_attackPower * value;
				}
				else if( subjectType == Unit2ndStatsTable.eSubjectType.DefUp ) {   // 방어율과 치명률은 절대비율 합연산(즉, 방어율 10%에서 5% 증가면 15%)
					m_defenceRate += ( value * (float)eCOUNT.MAX_PROBABILITY );
				}
				else if( subjectType == Unit2ndStatsTable.eSubjectType.CriUp ) {   // 방어율과 치명률은 절대비율 합연산(즉, 방어율 10%에서 5% 증가면 15%)
					m_criticalRate += ( value * (float)eCOUNT.MAX_PROBABILITY );
				}
				else if( subjectType == Unit2ndStatsTable.eSubjectType.CoolTime ) {
					DecreaseSkillCoolTimeValue += value;
				}
				else if( subjectType == Unit2ndStatsTable.eSubjectType.SkillAtk ) {
					IncreaseSkillAtkValue += value;
				}
                else if (subjectType == Unit2ndStatsTable.eSubjectType.Penetrate)
                {
                    value /= 10.0f;
                    Penetrate += value;
                }
                else if (subjectType == Unit2ndStatsTable.eSubjectType.Sufferance)
                {
                    value /= 10.0f;
                    Sufferance += value;
                }

                if( splits.Length > 2 ) {
                    Unit2ndStatsTable.eIncreaseType increaseType = Unit2ndStatsTable.GetIncreaseType( splits[2] );
                    int statValue = param.Min + (int)((float)gemData.RandOptValue[j] * param.Value);

                    m_2ndStats.AddStat( Unit2ndStatsTable.eAddType.Weapon, subjectType, increaseType, statValue );
                }
			}
		}

		SetSkillStats();
        OriginalPenetrate = Penetrate;
        OriginalSufferance = Sufferance;

        // 레이드 버프 아이템 적용
        if( World.Instance.StageType == eSTAGETYPE.STAGE_RAID ) {
            if( GameInfo.Instance.RaidAtkBuffRateFlag ) {
                m_attackPower += ( m_attackPower * GameInfo.Instance.BattleConfig.RaidAtkBuffRate );
            }

            if( GameInfo.Instance.RaidHpBuffRateFlag ) {
                m_maxHp += ( m_maxHp * GameInfo.Instance.BattleConfig.RaidHPBuffRate );
                m_curHp = m_maxHp;
            }
        }

		m_costumeUnit.ShowWeaponByIndex( mCurWeaponIndex, ChangeWeaponName );
		for( int i = 0; i < CloneCount; i++ ) {
			ChangeCloneWeaponByWeaponIndex( i, mCurWeaponIndex, ChangeWeaponName );
		}

		if( m_aniEvent ) {
			m_aniEvent.Rebind();
		}

		if( beforeIndex != mCurWeaponIndex ) {
			BOWeapon boBeforeWeapon = mBOWeapons[beforeIndex];
			if( boBeforeWeapon != null ) {
				for( int i = 0; i < boBeforeWeapon.ListBattleOptionData.Count; i++ ) {
					int battleOptionSetId = boBeforeWeapon.ListBattleOptionData[i].battleOptionSetId;

					m_buffStats.RemoveBuffStat( battleOptionSetId );
					m_cmptBuffDebuff.RemoveExecute( battleOptionSetId );

					if( boBeforeWeapon.ListBattleOptionData[i].dataOnEndCall != null ) {
						m_buffStats.RemoveBuffStat( boBeforeWeapon.ListBattleOptionData[i].dataOnEndCall.battleOptionSetId );
						m_cmptBuffDebuff.RemoveExecute( boBeforeWeapon.ListBattleOptionData[i].dataOnEndCall.battleOptionSetId );
					}

					EndBattleOption( battleOptionSetId );
					SetSpeedRateByCalcBuff( BattleOption.eToExecuteType.Weapon, true, true );
				}
			}
		}

		mOriginalMaxHp = m_maxHp;

        for ( int i = 0; i < mAddBuffDebuffDurationList.Count; i++ ) {
            if ( mAddBuffDebuffDurationList[i] != null ) {
                BuffEvent buffEvt = mAddBuffDebuffDurationList[i].evt as BuffEvent;
                if ( buffEvt != null ) {
                    buffEvt.ChangeDuration( buffEvt.battleOptionData.duration );
                }
                else {
                    mAddBuffDebuffDurationList[i].duration = mAddBuffDebuffDurationList[i].originalDuration;
                }
            }
        }

		if( boWeapon != null ) {
			if( m_actionSystem ) {
				m_actionSystem.ResetAllSetAddActioin();
			}

			boWeapon.Execute( BattleOption.eBOTimingType.GameStart );

            if ( !init ) {
                boWeapon.Execute( BattleOption.eBOTimingType.MissionStart );
            }
        }

		if( boSupporter != null ) {
			if( !init && World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
				boSupporter.Execute( BattleOption.eBOTimingType.GameStart, 0, null, true );

                if ( !init ) {
                    boSupporter.Execute( BattleOption.eBOTimingType.MissionStart, 0, null, true );
                }
            }
			else {
				boSupporter.Execute( BattleOption.eBOTimingType.GameStart );

                if ( !init ) {
                    boSupporter.Execute( BattleOption.eBOTimingType.MissionStart );
                }
            }
		}

		if( mBOCharacter != null ) {
			mBOCharacter.Execute( BattleOption.eBOTimingType.GameStart );

            if ( !init ) {
                mBOCharacter.Execute( BattleOption.eBOTimingType.MissionStart );
            }
        }

		if( m_curSp >= GameInfo.Instance.BattleConfig.USMaxSP ) {
			ExecuteBattleOption( BattleOption.eBOTimingType.MaxSP, 0, null );
		}

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			m_curHp = m_maxHp * nowHpPer;

            if( World.Instance.Player == this ) {
                if( !IsHelper ) {
                    World.Instance.UIPlay.m_playerHp.InitUIGaugeUnit( (int)m_curHp, (int)m_maxHp );
                }

                World.Instance.UIPlay.InitPlayerSp( this, boWeapon == null ? 0.0f : boWeapon.GetUseWeaponSkillSp() );
                World.Instance.UIPlay.ShowWpnSkillBtn( this );
            }
		}

		OnAfterChangeWeapon();
	}

	public bool HasSubWeapon()
    {
        if (mWeaponDatas == null || mWeaponDatas.Length <= 1)
        {
            return false;
        }

        return (mWeaponDatas[1] != null);
    }

    public void StartWpnSkill() {
        if( World.Instance.IsEndGame || World.Instance.IsPause || Input.isPause || isPause || m_curHp <= 0.0f ) {
            return;
        }

        if( !HasWeaponActiveSkill() || !IsActiveWeaponSkill() ) {
            return;
        }

        ActionWeaponSkillBase actionWeaponSkill = m_actionSystem.GetCurrentAction<ActionWeaponSkillBase>();
        if( actionWeaponSkill != null ) {
            return;
        }

        ActionSelectSkillBase actionSelectSkill = m_actionSystem.GetCurrentAction<ActionSelectSkillBase>();
        if( actionSelectSkill ) {
            bool skipReturn = false;

			if( actionSelectSkill.extraCancelCondition != null && actionSelectSkill.extraCancelCondition.Length > 0 ) {
				for( int i = 0; i < actionSelectSkill.extraCancelCondition.Length; i++ ) {
					if( actionSelectSkill.extraCancelCondition[i] == eActionCondition.UseSkill ) {
						skipReturn = true;
						break;
					}
				}
			}

			if( !skipReturn ) {
                return;
            }
        }

        Debug.Log( "Play WpnSKill" );
        EventMgr.Instance.SendEvent( eEventSubject.Self, eEventType.EVENT_PLAYER_INPUT_WEAPON_SKILL, this );
    }

    public override void SetSpeedRateByBuff(float rate, bool withAniSpeed = true, bool changeEffectSpeed = true)
    {
        base.SetSpeedRateByBuff(rate, withAniSpeed, changeEffectSpeed);

        Unit unitWeapon = GetCurrentUnitWeaponOrNull();
        if (unitWeapon)
        {
            unitWeapon.SetSpeedRateByBuff(rate, withAniSpeed, changeEffectSpeed);
        }
    }

    public virtual void OnAfterChangeWeapon()
    {
    }

    public BOWeapon GetBOWeaponBySlot(eWeaponSlot wpnSlot)
    {
        return mBOWeapons[(int)wpnSlot];
    }

    public override void ShowWeapon(bool show)
    {
        if (m_costumeUnit == null)
        {
            return;
        }

        if (show)
        {
            //m_costumeUnit.ShowWeaponByWeaponUID(mWeaponIds[mCurWeaponIndex]);
            m_costumeUnit.ShowWeaponByIndex(mCurWeaponIndex);
        }
        else
        {
            m_costumeUnit.HideWeapon();
        }
    }

    protected override void LoadShieldEffects()
    {
        if (!string.IsNullOrEmpty(m_data.EffShield))
        {
            m_psShieldOnHit = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>("effect", "Effect/prf_fx_shield.prefab");
            if (m_psShieldOnHit != null)
                m_psShieldOnHit.gameObject.SetActive(false);
        }

        if (!string.IsNullOrEmpty(m_data.EffShieldBreak))
        {
            m_psShieldBreak = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>("effect", "Effect/prf_fx_broken_shield.prefab");
            if (m_psShieldBreak != null)
                m_psShieldBreak.gameObject.SetActive(false);
        }

        if (!string.IsNullOrEmpty(m_data.EffShieldAttack))
        {
            m_psShieldAttack = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>("effect", "Effect/prf_fx_shield_crash.prefab");
            if (m_psShieldAttack != null)
                m_psShieldAttack.gameObject.SetActive(false);
        }
    }

    public override float GetUltimateSkillDefaultAtkPower()
    {
        float atkPower = m_attackPower;
        atkPower = m_buffStats.CalcBuffStat(BattleOption.eToExecuteType.Unit, UnitBuffStats.eBuffStatType.UltimateSkillAtkPower,
                                            UnitBuffStats.eIncreaseType.Increase, atkPower, true, false, true, false, null);
        atkPower = m_buffStats.CalcBuffStat(BattleOption.eToExecuteType.Unit, UnitBuffStats.eBuffStatType.UltimateSkillAtkPower,
                                            UnitBuffStats.eIncreaseType.Decrease, atkPower, true, false, true, false, null);
        // 데미지 최소값 체크
        if (atkPower < 0.0f)
            atkPower = 0.0f;
        return atkPower;
    }

	public override void ExecuteBattleOption( BattleOption.eBOTimingType timingType, int actionTableId, Projectile projectile, bool skipWeaponBO = false ) {
		if( !IsActivate() ) {
			return;
		}

		if( m_clone && SendBattleOptionToOwner && m_cloneOwner ) {
			m_cloneOwner.ExecuteBattleOption( timingType, actionTableId, projectile );
		}
		else {
			if( !skipWeaponBO ) {
                if( m_boSupporter != null ) {
                    m_boSupporter.Execute( timingType, actionTableId, projectile );
                }

                if( boWeapon != null ) {
                    boWeapon.Execute( timingType, actionTableId, projectile );
                }

				if( mBOCharacter != null ) {
					mBOCharacter.Execute( timingType, actionTableId, projectile );
				}
			}

			if( actionTableId > 0 ) {
                ActionSelectSkillBase action = m_actionSystem.GetActionOrNullByTableId<ActionSelectSkillBase>( actionTableId );
				if( action ) {
					action.ExecuteBattleOption( timingType, action.TableId, projectile );
				}
			}
		}
	}

	// PVP에서 유닛 생성 후
	public virtual void OnAfterCreateInPVP()
    {
        ActionSystemLoadAfterEnemyMgrInit();
    }

    // PVP에서 한판 끝났을 때 
    public virtual void OnAfterPVPBattle()
    {
        HideAllClone();

        if(m_cmptBuffDebuff)
        {
            m_cmptBuffDebuff.Clear();
        }

        if ( Guardian ) {
			Utility.SetLayer( Guardian.gameObject, gameObject.layer, true );
			Guardian.actionSystem.CancelCurrentAction();
			Guardian.StopBT();
		}
    }

    // PVP에서 항복했을 때
    public virtual void OnPVPSurrender()
    {
        StopBT();
        HideAllClone();
        DeactivateAllPlayerMinion();
        actionSystem.CancelCurrentAction();
    }

    /// <summary>
    /// 게임 시작 전에 World 및 TestScene에서 한번 호출해줄 함수 
    /// </summary>
    public override void OnGameStart() {
		if ( IsHelper ) {
			Activate();
		}

		InverseXAxis = false;
		InverseYAxis = false;

		if ( m_crChargingSP != null ) {
			Utility.StopCoroutine( World.Instance, ref m_crChargingSP );
		}

		m_crChargingSP = World.Instance.StartCoroutine( ChargingSP() );

		mAddMaxHpRate = 0.0f;
		ExecuteBattleOption( BattleOption.eBOTimingType.GameStart, 0, null, true );

		mCurWeaponIndex = 0;

		if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			AppMgr.Instance.CustomInput.ShowCursor( false );
			ChangeWeapon( true );
		}
		else {
			m_costumeUnit.ShowWeaponByIndex( 0 );

			if ( boWeapon != null ) {
				boWeapon.Execute( BattleOption.eBOTimingType.GameStart );
			}

			if ( boSupporter != null ) {
				boSupporter.Execute( BattleOption.eBOTimingType.GameStart );
			}

			if ( mBOCharacter != null ) {
				mBOCharacter.Execute( BattleOption.eBOTimingType.GameStart );
			}
		}

		for ( int i = 0; i < m_actionSystem.ListAction.Count; i++ ) {
			ActionSelectSkillBase action = m_actionSystem.ListAction[i] as ActionSelectSkillBase;
			if ( action == null ) {
				continue;
			}

			action.ExecuteBattleOption( BattleOption.eBOTimingType.GameStart, action.TableId, null );
		}

		IsMissionStart = false;
	}

	/// <summary>
	/// 미션 스타트 UI 뜬 후 호출
	/// </summary>
	public override void OnMissionStart() {
		IsMissionStart = true;

		if ( boWeapon != null ) {
			boWeapon.Execute( BattleOption.eBOTimingType.MissionStart );
		}

		if ( boSupporter != null ) {
			if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
				boSupporter.Execute( BattleOption.eBOTimingType.MissionStart, 0, null, true );
			}
			else {
				boSupporter.Execute( BattleOption.eBOTimingType.MissionStart );
			}
		}

		if ( mBOCharacter != null ) {
			mBOCharacter.Execute( BattleOption.eBOTimingType.MissionStart );
		}

		List<ActionSelectSkillBase> listActionSkill = m_actionSystem.GetActionList<ActionSelectSkillBase>();
		for ( int i = 0; i < listActionSkill.Count; i++ ) {
			ActionSelectSkillBase action = listActionSkill[i];
			if ( action.BOCharSkill == null ) {
				continue;
			}

			action.ExecuteBattleOption( BattleOption.eBOTimingType.MissionStart );
		}

		BattleAreaManager battleAreaMgr = World.Instance.EnemyMgr as BattleAreaManager;
		if ( battleAreaMgr == null ) {
			return;
		}

		BattleArea battleArea = battleAreaMgr.GetCurrentBattleArea();
		if ( battleArea == null ) {
			battleArea = battleAreaMgr.GetBattleArea( 0 );
			if ( battleArea == null ) {
				return;
			}
		}
	}

	public override void OnGameEnd() {
		base.OnGameEnd();

        if( IsHelper ) {
            Deactivate();
		}
	}

	public virtual void OnStartSkill(ActionSelectSkillBase currentSkillAction)
    {
    }

    public virtual void OnStartUSkill()
    {
    }

	public override void Activate() {
		base.Activate();
		LockChangeFace = false;

		if( costumeUnit ) {
            bool isHideWeapon = false;
            if ( m_charData != null ) {
				isHideWeapon = m_charData.IsHideWeapon;
			}
			costumeUnit.ShowObject( costumeUnit.Param.LobbyOnly, isHideWeapon );
		}

		if( m_actionSystem != null && !isClone ) {
			m_actionSystem.CancelCurrentAction();
			CommandAction( eActionCommand.Idle, null );
		}

		if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP && IsSupporterCoolingTime ) {
            mbContinueCoolTime = true;

            Utility.StopCoroutine( World.Instance, ref mCrUpdateCoolTime );
            mCrUpdateCoolTime = World.Instance.StartCoroutine( UpdateSupporterCoolTime() );
		}

		mCheckKeepTargeting = false;
		mCheckKeepTargetingTime = 0.0f;

		mUIBuffDebuffIcon = ResourceMgr.Instance.CreateFromAssetBundle<UI3DBuffDebuffIcon>( "ui", "UI/BuffDebuff.prefab" );
		if( mUIBuffDebuffIcon ) {
			mUIBuffDebuffIcon.name = "BuffDebuffIcon_" + name;
			mUIBuffDebuffIcon.transform.SetParent( transform );
			Utility.InitTransform( mUIBuffDebuffIcon.gameObject );

			mUIBuffDebuffIcon.Init( this );
		}

        if ( Guardian ) {
            Guardian.Activate();
        }
	}

	public override bool ShowMesh( bool show, bool isLock = false ) {
		if ( !base.ShowMesh( show, isLock ) ) {
			return false;
		}

        if ( m_costumeUnit != null ) {
            CostumeUnit.sWeaponItem weaponItem = m_costumeUnit.GetWeaponItemOrNull( mCurWeaponIndex );
            if ( weaponItem != null ) {
                weaponItem.ShowEffect( show );
			}
        }

        EnableDefaultFace();
		return true;
	}

	public bool LockChangeFace { get; set; } = false;
    public void EnableDefaultFace()
    {
        if (LockChangeFace)
        {
            return;
        }

        if (m_defaultFace)
            m_defaultFace.SetActive(true);

        if (m_boneFace)
            m_boneFace.SetActive(false);
    }

    public void EnableBoneFace()
    {
        if (LockChangeFace)
        {
            return;
        }

        if (m_defaultFace)
            m_defaultFace.SetActive(false);

        if (m_boneFace)
        {
            m_boneFace.SetActive(true);
            PlayFaceAni(eFaceAnimation.Idle01);
        }
    }

    public override bool PlayDirector(string key, Action CallbackOnEnd2 = null, Action CallbackOnEnd3 = null, Action CallbackOnEndLoopOnce = null)
    {
        EnableBoneFace();

        if (CallbackOnEnd3 == null)
            base.PlayDirector(key, CallbackOnEnd2, EnableDefaultFace, CallbackOnEndLoopOnce);
        else
            base.PlayDirector(key, CallbackOnEnd2, CallbackOnEnd3, CallbackOnEndLoopOnce);

        return true;
    }

    public override bool HoldPlayDirector(string key, Action CallbackOnEnd2 = null, Action CallbackOnEnd3 = null, Action HoldCallbackEnd = null)
    {
        EnableBoneFace();

        if (CallbackOnEnd3 == null)
            base.HoldPlayDirector(key, CallbackOnEnd2, EnableDefaultFace, HoldCallbackEnd);
        else
            base.HoldPlayDirector(key, CallbackOnEnd2, CallbackOnEnd3, HoldCallbackEnd);

        return true;
    }

    protected virtual void AddQTEComponent()
    {
        //#if UNITY_EDITOR
        //if (!World.Instance.isTestScene && !GameInfo.Instance.isTest)
        /*#endif
                {
                    for (int i = 0; i < m_charData.EquipSkill.Length; i++)
                    {
                        int id = m_charData.EquipSkill[i];
                        if (id <= 0)
                            continue;

                        GameTable.CharacterSkill.Param param = GameInfo.Instance.GameTable.FindCharacterSkill(id);
                        if (param == null)
                        {
                            Debug.LogError(name + "에 " + id + "번 QTE가 없습니다.");
                            continue;
                        }

                        AddQTEComponent(param);
                    }
                }*/
        //#if UNITY_EDITOR
        //else
        /*{
            List<GameTable.CharacterSkill.Param> listParam = GameInfo.Instance.GameTable.FindAllCharacterSkill(x => x.CharacterID == m_tableId);
            for (int i = 0; i < listParam.Count; i++)
                AddQTEComponent(listParam[i]);
        }*/
        //#endif
    }

    /*private void AddQTEComponent(GameTable.CharacterSkill.Param param)
    {
        if (string.IsNullOrEmpty(param.QteScript))
            return;

        ActionQTEBase actionQTE = (ActionQTEBase)gameObject.AddComponent(Type.GetType(param.QteScript));
        if (actionQTE == null)
            return;

        actionQTE.qteType = (ActionQTEBase.eQTEType)param.QteType;
        actionQTE.executeCondition = (ActionQTEBase.eExecuteCondition)param.ExecuteCondition;
        actionQTE.executeButton = (ActionQTEBase.eQTEButton)param.ExecuteButton;
        actionQTE.duration = param.Duration;

        string str = param.Commands.Replace("{", "");
        str = str.Replace("}", "");

        string[] commands = str.Split(',');
        actionQTE.commands = new ActionQTEBase.sCommandInfo[commands.Length];
        for (int i = 0; i < commands.Length; i++)
        {
            actionQTE.commands[i].command = (ActionQTEBase.eQTEButton)int.Parse(commands[i]);
            actionQTE.commands[i].aniLength = 0.0f;
        }

        actionQTE.isAllUnitPause = param.IsAllUnitPause;

        eAnimation aniPrepare = (eAnimation)Enum.Parse(typeof(eAnimation), param.AniPrepare);
        actionQTE.aniPrepare = aniPrepare;
    }*/

    public void AddDirector()
    {
        Director director = null;
        if (string.IsNullOrEmpty(m_data.StartDrt) == false)
        {
            director = GameSupport.CreateDirector(m_data.StartDrt);
            AddDirector("Start", director);
        }

        if (string.IsNullOrEmpty(m_data.WinDrt_01) == false)
        {
            director = GameSupport.CreateDirector(m_data.WinDrt_01);
            AddDirector("Win01", director);
        }

        /*
        if (string.IsNullOrEmpty(m_data.WinDrt_02) == false)
        {
            director = GameSupport.CreateDirector(m_data.WinDrt_02);
            AddDirector("Win02", director);
        }
        */

        //if (World.Instance.gameMode == World.eGameMode.Normal)
        {
            if (string.IsNullOrEmpty(m_data.ResurrectionDrt_02) == false)
            {
                director = GameSupport.CreateDirector(m_data.ResurrectionDrt_02);
                AddDirector("Resurrection", director);
            }
        }

        if (string.IsNullOrEmpty(m_data.GroggyDrt_02) == false)
        {
            director = GameSupport.CreateDirector(m_data.GroggyDrt_02);
            AddDirector("Groggy", director);
        }

        if (string.IsNullOrEmpty(m_data.DieDrt_02) == false)
        {
            director = GameSupport.CreateDirector(m_data.DieDrt_02);
            AddDirector("Die", director);
        }

        if (!string.IsNullOrEmpty(m_data.USkillDrt_01))
        {
            director = GameSupport.CreateDirector(m_data.USkillDrt_01);
            AddDirector("USkill01", director);
        }
    }

	protected override void Reborn() {
		base.Reborn();

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			m_curHp = m_maxHp;
			m_curSp = GameInfo.Instance.BattleConfig.USInitSP;
		}

		if( m_buffStats != null ) {
			m_buffStats.Clear();
		}

		if( m_cmptBuffDebuff != null ) {
			m_cmptBuffDebuff.Clear();
		}
	}

	public void Resurrection() {
		SetGroundedRigidBody();

		// 부활 시 효과 (HP & SP 최대치로 회복)
		m_curHp = m_maxHp;
		AddSp( GameInfo.Instance.BattleConfig.USMaxSP );

		World.Instance.InGameCamera.RestoreCullingMask();
		World.Instance.InGameCamera.SetDefaultMode();

		GameUIManager.Instance.ShowUI( "GamePlayPanel", true );

		World.Instance.UIPlay.SubPlayerHp( this );
		World.Instance.UIPlay.AddPlayerSp( this, boWeapon == null ? 0.0f : boWeapon.GetUseWeaponSkillSp() );

		isDying = false;

		m_actionSystem.CancelCurrentAction();
		PlayAniImmediate( eAnimation.Idle01 );
	}

	/*public void SetInputForward(Vector3 dir)
    {
        inputForward = dir;
        unitForward = dir == Vector3.zero ? transform.forward : dir;
    }*/

	public override List<UnitCollider> GetTargetColliderList(bool onlyMainCollider = false)
    {
        if (isClone)
        {
            return m_cloneOwner.GetTargetColliderList();
        }

        List<UnitCollider> list = new List<UnitCollider>();

        List<Unit> listAllActiveUnit = World.Instance.EnemyMgr.GetAllActiveUnit(this);
        for (int i = 0; i < listAllActiveUnit.Count; i++)
        {
            if (listAllActiveUnit[i].ignoreHit)
            {
                continue;
            }

            if (listAllActiveUnit[i].MainCollider.IsEnable())
            {
                list.Add(listAllActiveUnit[i].MainCollider);
            }

            if (!onlyMainCollider)
            {
                for (int j = 0; j < listAllActiveUnit[i].ListCollider.Count; j++)
                {
                    if (listAllActiveUnit[i].ListCollider[j].IsEnable())
                    {
                        list.Add(listAllActiveUnit[i].ListCollider[j]);
                    }
                }
            }
        }

        return list;
    }

    public override List<UnitCollider> GetTargetColliderListByAround(Vector3 pos, float radius, bool onlyMainCollider = false)
    {
        Collider[] cols = Physics.OverlapSphere(pos, radius, Utility.GetEnemyLayer((eLayer)gameObject.layer));// | (1 << (int)eLayer.EnvObject));
        if (cols.Length <= 0)
            return null;

        List<UnitCollider> list = new List<UnitCollider>();
        for (int i = 0; i < cols.Length; i++)
        {
            UnitCollider capsuleCollider = cols[i].GetComponent<UnitCollider>();
            if (capsuleCollider == null || !capsuleCollider.IsEnable() || capsuleCollider.Owner.ignoreHit ||
                capsuleCollider.Owner.gameObject.layer == (int)eLayer.EnvObject)
            {
                continue;
            }

            if (onlyMainCollider)
            {
                if (capsuleCollider.Owner.MainCollider != capsuleCollider)
                {
                    continue;
                }
            }

            list.Add(capsuleCollider);
        }

        return list;
    }

	public override UnitCollider GetMainTargetCollider( bool onlyEnemy, float checkDist = 0.0f, bool skipHasShieldTarget = false, bool onlyAir = false )
	{
		UnitCollider nearestHitCollider = null;

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP && m_mainTarget ) {
			if( FSaveData.Instance.AutoTargeting || ( mInput && mInput.GetDirection() != Vector3.zero ) || m_mainTarget.curHp <= 0.0f || !m_mainTarget.IsActivate() ||
			   ( checkDist > 0.0f && Vector3.Distance( transform.position, m_mainTarget.transform.position ) > checkDist ) ) {
				m_mainTarget = null;
			}
		}

		if( m_mainTarget ) {
			nearestHitCollider = m_mainTarget.GetNearestColliderFromPos( transform.position );
		}
		else {
			Unit target = World.Instance.EnemyMgr.GetNearestTarget( this, onlyEnemy, skipHasShieldTarget, onlyAir );
			if( target ) {
				SetMainTarget( target );
				nearestHitCollider = target.GetNearestColliderFromPos( transform.position );
			}
			else if( ( AI && World.Instance.EnemyMgr.HasAliveMonster() ) || !onlyEnemy ) {
				target = World.Instance.EnemyMgr.GetNearestTarget( this, World.Instance.EnemyMgr.GetActiveEnvObjects() );
				if( target ) {
					SetMainTarget( target );
					nearestHitCollider = target.GetNearestColliderFromPos( transform.position );
				}
			}
		}

		return nearestHitCollider;
	}

	public override UnitCollider GetRandomTargetCollider(bool onlyEnemy = false)
    {
        List<Unit> listTarget = World.Instance.EnemyMgr.GetActiveEnemies(this);
        if (listTarget.Count <= 0 && !onlyEnemy)
        {
            listTarget = World.Instance.EnemyMgr.GetActiveEnvObjects();
        }

        if (listTarget.Count <= 0)
        {
            return null;
        }

        Unit target = listTarget[UnityEngine.Random.Range(0, listTarget.Count)];
        if (target)
        {
            return target.GetNearestColliderFromPos(transform.position);
        }

        return null;
    }

    public override UnitCollider GetRandomTargetColliderByAround(float radius)
    {
        List<UnitCollider> listHitCollider = GetTargetColliderListByAround(transform.position, radius);
        if (listHitCollider == null || listHitCollider.Count <= 0)
            return null;

        return listHitCollider[UnityEngine.Random.Range(0, listHitCollider.Count)];
    }

    /*
    public override List<Unit> GetTargetList()
    {
        if (isClone)
            return m_cloneOwner.GetTargetList();

        if (m_onlyQTETarget && m_qteTarget)
        {
            List<Unit> list = new List<Unit>();
            list.Add(m_qteTarget);

            return list;
        }
        else
        {
            return World.Instance.enemyMgr.GetAllActiveUnit();
        }
    }

    public override List<Unit> GetTargetListByAround(float radius)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, (1 << (int)eLayer.Enemy) | (1 << (int)eLayer.EnvObject));
        if (cols.Length <= 0)
            return null;

        List<Unit> listResult = new List<Unit>();
        for (int i = 0; i < cols.Length; i++)
        {
            Unit target = cols[i].GetComponent<Unit>();
            if (target == null || target as FigureUnit)
                continue;

            listResult.Add(target);
        }

        return listResult;
    }

    public override List<Unit> GetTargetListByAround(Vector3 pos, float radius)
    {
        Collider[] cols = Physics.OverlapSphere(pos, radius, (1 << (int)eLayer.Enemy) | (1 << (int)eLayer.EnvObject));
        if (cols.Length <= 0)
            return null;

        List<Unit> listResult = new List<Unit>();
        for (int i = 0; i < cols.Length; i++)
        {
            Unit target = cols[i].GetComponent<Unit>();
            if (target == null || target as FigureUnit)
                continue;

            listResult.Add(target);
        }

        return listResult;
    }

    public override Unit GetMainTarget(float checkDist = 0.0f, bool skipHasShieldTarget = false, bool onlyAir = false, bool checkSelfAir = true)
    {
        if (m_actionSystem.IsCurrentQTEAction())
            return m_qteTarget;

        if (m_mainTarget)
        {
            if (m_controller.GetDirection() != Vector3.zero)
                m_mainTarget = null;
            else if ((m_mainTarget.curHp <= 0.0f || !m_mainTarget.IsActivate()) ||
                     (checkDist > 0.0f && Vector3.Distance(transform.position, m_mainTarget.transform.position) > checkDist))
            {
                m_mainTarget = null;
            }
        }

        if (m_mainTarget)
            return m_mainTarget;
        else
        {
            Unit target = World.Instance.enemyMgr.GetNearestTarget(this, skipHasShieldTarget, onlyAir, checkSelfAir);
            if (target)
                SetMainTarget(target);
            else
            {
                target = World.Instance.enemyMgr.GetNearestTarget(this, World.Instance.enemyMgr.GetActiveEnvObjects());
                if (target)
                    SetMainTarget(target);
            }

            return target;
        }
    }

    public override Unit GetRandomTarget()
    {
        List<Unit> listTarget = World.Instance.enemyMgr.GetActiveEnemies();
        if (listTarget.Count <= 0)
        {
            listTarget = World.Instance.enemyMgr.GetActiveEnvObjects();
            if (listTarget.Count <= 0)
                return null;
        }

        return listTarget[UnityEngine.Random.Range(0, listTarget.Count)];
    }

    public override Unit GetRandomTargetByAround(float radius)
    {
        List<Unit> listTarget = GetTargetListByAround(radius);
        if (listTarget == null || listTarget.Count <= 0)
            return null;

        return listTarget[UnityEngine.Random.Range(0, listTarget.Count)];
    }
    */

    List<Unit> listTemp = new List<Unit>();
    public override List<Unit> GetEnemyList(bool onlyEnemy = false)
    {
        listTemp.Clear();

        List<Unit> listTarget = World.Instance.EnemyMgr.GetActiveEnemies(this);
        for(int i = 0; i < listTarget.Count; i++)
        {
            listTemp.Add(listTarget[i]);
        }

        if (listTarget.Count <= 0 && !onlyEnemy)
        {
            listTarget = World.Instance.EnemyMgr.GetActiveEnvObjects();
            if (listTarget.Count <= 0)
            {
                return null;
            }

            for (int i = 0; i < listTarget.Count; i++)
            {
                listTemp.Add(listTarget[i]);
            }
        }

        return listTemp;
    }

    public override List<Unit> GetEnemyListByAbnormalAttr(EAttackAttr attr)
    {
        listTemp.Clear();

        List<Unit> listTarget = World.Instance.EnemyMgr.GetActiveEnemies(this);
        for (int i = 0; i < listTarget.Count; i++)
        {
            Unit target = listTarget[i];
            if(!target.IsCurrentAbnormalAttr(attr))
            {
                continue;
            }

            listTemp.Add(listTarget[i]);
        }

        return listTemp;
    }

    public override List<UnitCollider> GetEnemyColliderList()
    {
        List<UnitCollider> list = new List<UnitCollider>();

        List<Unit> listTarget = World.Instance.EnemyMgr.GetActiveEnemies(this);
        if (listTarget.Count <= 0)
        {
            listTarget = World.Instance.EnemyMgr.GetActiveEnvObjects();
            if (listTarget.Count <= 0)
                return null;
        }

        for (int i = 0; i < listTarget.Count; i++)
        {
            list.Add(listTarget[i].MainCollider);
        }

        return list;
    }

    public override List<UnitCollider> GetAllyColliderListByAround( Vector3 pos, float radius, bool onlyMainCollider = false ) {
        int count = ( World.Instance.ListPlayer.Count > 0 ) ? World.Instance.ListPlayer.Count : 1;
        List<UnitCollider> list = new List<UnitCollider>( count );

        if ( World.Instance.ListPlayer.Count > 0 ) {
            for ( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                float dist = Utility.GetDistanceWithoutY( pos, World.Instance.ListPlayer[i].transform.position );
                if ( dist <= radius ) {
                    list.Add( World.Instance.ListPlayer[i].MainCollider );
				}
			}
		}
        else {
            list.Add( MainCollider );
		}

        return list;
    }

    public override void SetMainTarget(Unit target)
    {
        if (m_actionSystem.IsCurrentQTEAction() || World.Instance.StageType == eSTAGETYPE.STAGE_PVP && target == null)
        {
            mCheckKeepTargeting = false;
            return;
        }

        if(target == null)
        {
            mCheckKeepTargeting = false;
            mCheckKeepTargetingTime = 0.0f;
        }
        else if(target != m_mainTarget)
        {
            mCheckKeepTargeting = true;
            mCheckKeepTargetingTime = 0.0f;

            World.Instance.InGameCamera.StopTurnToTarget();
        }

        base.SetMainTarget(target);

        if (m_effTarget == null)
        {
            return;
        }

        if (target == null || (target.gameObject.layer != (int)eLayer.Enemy && target.gameObject.layer != (int)eLayer.EnemyGate))
        {
            m_effTarget.transform.SetParent(null);
            m_effTarget.gameObject.SetActive(false);

            m_parentEffTarget = null;
            return;
        }

        Transform hitPos = target.aniEvent ? target.aniEvent.GetBoneByName("HitPos") : null;
        if (hitPos)
        {
            m_effTarget.transform.SetParent(hitPos);
            Utility.InitTransform(m_effTarget.gameObject);
        }
        else
        {
            m_effTarget.transform.SetParent(target.transform);
            Utility.InitTransform(m_effTarget.gameObject, target.MainCollider.HitCollider.center, Quaternion.identity, Vector3.one);
        }

        m_parentEffTarget = target;
    }

	public override bool OnEvent( BaseEvent evt, IActionBaseParam param = null ) {
		if( base.OnEvent( evt, param ) ) {
			return false;
		}

		// 플레이어만 걸리는 디버프 효과 무시하는 옵션이 있는지 검사
		eEventType findImmuneDebuff = eEventType.EVENT_NONE;
		for( int i = 0; i < ListImmuneDebuffType.Count; i++ ) {
			if( ListImmuneDebuffType[i] == evt.eventType ) {
				findImmuneDebuff = ListImmuneDebuffType[i];
				break;
			}
		}

		if( findImmuneDebuff != eEventType.EVENT_NONE ) {
			return true;
		}

		switch( evt.eventType ) {
			case eEventType.EVENT_PLAYER_INPUT_DIR:
			case eEventType.EVENT_PLAYER_INPUT_DIR_AUTO:
				OnEventInput( evt );
				return true;

			case eEventType.EVENT_PLAYER_INPUT_ATK:
				OnEventAtk();
				return true;

			case eEventType.EVENT_PLAYER_INPUT_JUMP:
				OnEventJump();
				return true;

			case eEventType.EVENT_PLAYER_INPUT_DEFENCE:
				OnEventDefence( param );
				return true;

			case eEventType.EVENT_PLAYER_INPUT_CHARGE_ATK_START:
				OnEventChargeAtkStart( param );
				return true;

			case eEventType.EVENT_PLAYER_INPUT_CHARGE_ATK_END:
				OnEventChargeAtkEnd( param );
				return true;

			case eEventType.EVENT_PLAYER_INPUT_SPECIAL_ATK:
				OnEventSpecialAtk();
				return true;

			case eEventType.EVENT_PLAYER_INPUT_ATK_PRESS_START:
				OnEventAtkPressStart();
				return true;

			case eEventType.EVENT_PLAYER_INPUT_ATK_TOUCH_END:
				OnEventAtkTouchEnd();
				return true;

			case eEventType.EVENT_PLAYER_INPUT_ULTIMATE_SKILL:
				OnEventUltimateSkill();
				return true;

			case eEventType.EVENT_PLAYER_INPUT_WEAPON_SKILL:
				OnEventWeaponSkill();
				break;

			case eEventType.EVENT_ENEMY_SNAKELADY_CHANGE_ACTION:
				m_lastMainTarget = null;
				m_mainTarget = null;
				return true;

			case eEventType.EVENT_ACTION_SET_SPRINT_START_TIME_RATE:
				sprintStartTimeRate = ( evt as ActionEvent ).value;
				return true;

			case eEventType.EVENT_ACTION_SET_ONLY_LAST_MELEE_ATTACK:
				onlyLastAttack = ( evt as ActionEvent ).value == 0.0f ? false : true;
				return true;

			case eEventType.EVENT_ACTION_SET_LAST_MELEE_ATTACK_KNOCKBACK:
				lastAttackKnockBack = ( evt as ActionEvent ).value == 0.0f ? false : true;
				return true;

			case eEventType.EVENT_BUFF_ADD_SP: {
                int checkActionTableId = -1;

                if ( m_actionSystem && evt.battleOptionData.CheckActionTableId == -1 ) {
                    ActionSelectSkillBase actionSelectSkill = m_actionSystem.GetCurrentAction<ActionSelectSkillBase>();
                    if ( actionSelectSkill ) {
                        checkActionTableId = actionSelectSkill.TableId;
                    }
                }

                if ( !GameSupport.IsBOConditionCheck( evt.battleOptionData.conditionType, evt.battleOptionData.CondValue, evt.sender, this, 
                                                      checkActionTableId, evt.battleOptionData.Pjt ) ) {
                    return true;
                }

                AddSp( ( evt as BuffEvent ).value );
            }
            return true;

			case eEventType.EVENT_ATTACK_ADD:
				int effID = 0;

                if( evt.battleOptionData != null && evt.battleOptionData.effId1 > 0 ) {
                    effID = evt.battleOptionData.effId1;
                }

				AddAttack( evt.value, (int)evt.value2, (int)evt.value3, effID );
				return true;

            case eEventType.EVENT_SP_ATTACK_ADD:
                effID = 0;

                if( evt.battleOptionData != null && evt.battleOptionData.effId1 > 0 ) {
                    effID = evt.battleOptionData.effId1;
                }

                AddSpAttack( evt.value, (int)evt.value2, (int)evt.value3, effID );
                return true;

            case eEventType.EVENT_ACTION_HIT_ATTACK_REPEAT: 
                if( !GameSupport.IsBOConditionCheck( evt.battleOptionData.conditionType, evt.battleOptionData.CondValue, this ) ) {
                    return true;
                }

                World.Instance.StartCoroutine( AttackRepeat( evt ) );
                return true;

            case eEventType.EVENT_COMBO_RESET_BY_HIT:
				m_comboResetCondition = eComboResetCondition.ByHit;

				StopCoroutine( "UpdateClearCombo" );
				ClearCombo();
				return true;

			case eEventType.EVENT_SUPPORTER_COOL_TIME_DECREASE:
				SupporterCoolTimeDecRate += evt.value;
				break;

			case eEventType.EVENT_SUPPORTER_COOL_TIME_DECREASE_IN_REAL_TIME: {
				if ( evt.battleOptionData.preConditionType != BattleOption.eBOConditionType.None && evt != null ) {
					if ( !GameSupport.IsBOConditionCheck( evt.battleOptionData.preConditionType, evt.battleOptionData.CondValue, 
                                                          evt.battleOptionData.evt.sender, null, -1,  evt.battleOptionData.Pjt ) ) {
						return true;
					}
				}

                float value = m_boSupporter.data.TableData.CoolTime * evt.value;

                if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
                    DecreaseSupporterCoolTimeInRealTime( value );
                }
                else {
                    SupporterCoolingTime = Mathf.Max( 0.0f, SupporterCoolingTime - value );
                }
            }
            break;

			case eEventType.EVENT_SKILL_COOL_TIME_DECREASE:
				DecreaseSkillCoolTimeValue += evt.value;
				break;

			case eEventType.EVENT_SKILL_ACTION_COOL_TIME_DECREASE:
				ActionSelectSkillBase action = m_actionSystem.GetActionOrNullByTableId<ActionSelectSkillBase>(evt.battleOptionData.actionTableId);
				if( action ) {
					action.DecreaseSkillCoolTimeValue = evt.value;
				}
				break;

			case eEventType.EVENT_SKILL_ACTION_COOL_TIME_DECREASE_IN_REAL_TIME:
				if( m_actionSystem ) {
					ActionSelectSkillBase actionCoolTime = m_actionSystem.GetSelectSkillActionDuringCooltimeOrNull();
					if( actionCoolTime ) {
						actionCoolTime.DecreaseSkillCoolTime( evt.value );
					}
				}
				break;

			case eEventType.EVENT_SKILL_ONE_ACTION_COOL_TIME_DECREASE_SEC:
				if( m_actionSystem ) {
					ActionSelectSkillBase actionSkill = m_actionSystem.GetActionOrNullByTableId<ActionSelectSkillBase>((int)evt.value2);
					if( actionSkill ) {
						actionSkill.SetIncreaseCoolingTimeBySec( evt.value );
					}
				}
				break;

			case eEventType.EVENT_SKILL_ONLY_ACTION_COOL_TIME_DECREASE_SEC:
				if( m_actionSystem ) {
					ActionSelectSkillBase actionSkill = m_actionSystem.GetActionOrNullByTableId<ActionSelectSkillBase>( (int)evt.value2 );
					if( actionSkill ) {
						actionSkill.DecreaseSkillCoolTimeBySec( evt.value );
					}
				}
				break;

			case eEventType.EVENT_SKILL_RESET_ACTION_COOL_TIME_BY_USE_SP:
				if( evt.battleOptionData.conditionType != BattleOption.eBOConditionType.None ) {
					Type type = Type.GetType( "Action" + evt.battleOptionData.conditionType.ToString() );
					if( type == null ) {
						return true;
					}

					action = GetComponent( type ) as ActionSelectSkillBase;
					if( action == null || action != actionSystem.currentAction ) {
						return true;
					}
				}
				else {
					action = m_actionSystem.GetActionOrNullByTableId<ActionSelectSkillBase>( evt.battleOptionData.actionTableId );
				}

				if( action && m_curSp > evt.value2 ) {
					if( UnityEngine.Random.Range( 0, (int)eCOUNT.MAX_BO_FUNC_VALUE ) <= (int)evt.value ) {
						UseSp( evt.value2 );
						action.DecreaseSkillCoolTime( 1.0f );
					}
				}

				break;

			case eEventType.EVENT_SKILL_RESET_ALL_COOL_TIME: {
                List<ActionSelectSkillBase> list = m_actionSystem.GetActionList<ActionSelectSkillBase>();
                for ( int i = 0; i < list.Count; i++ ) {
                    list[i].DecreaseSkillCoolTime( 1.0f );
                }
            }
            break;

            case eEventType.EVENT_RANDOM_SKILL_RESET: {
                int checkActionTableId = -1;

				ActionSelectSkillBase actionSelectSkill = m_actionSystem.GetCurrentAction<ActionSelectSkillBase>();
				if ( actionSelectSkill ) {
                    checkActionTableId = actionSelectSkill.TableId;
				}

				if ( !GameSupport.IsBOConditionCheck( evt.battleOptionData.conditionType, evt.battleOptionData.CondValue, evt.sender, evt.sender, 
                                                      checkActionTableId ) ) {
                    return true;
                }

                bool checkRandom = true;

                if( evt.battleOptionData.conditionType == BattleOption.eBOConditionType.RinChargeAttack ) {
                    ActionRinChargeAttack rinChargeAtk = m_actionSystem.GetCurrentAction<ActionRinChargeAttack>();
                    if ( rinChargeAtk.SetAddAction ) {
                        checkRandom = false;
                    }
				}

                if ( checkRandom && UnityEngine.Random.Range( 0, (int)eCOUNT.MAX_BO_FUNC_VALUE ) > (int)evt.value ) {
                    return true;
                }

                mTempSelectSkillList.Clear();

                List<ActionSelectSkillBase> list = m_actionSystem.GetActionList<ActionSelectSkillBase>();
                for ( int i = 0; i < list.Count; i++ ) {
                    int checkTableId = (int)evt.value2;
                    if ( list[i].TableId == checkTableId || list[i].ParentTableId == checkTableId ) {
                        continue;
					}

                    if ( list[i].MaxCoolTime > 0.0f && !list[i].PossibleToUse ) {
                        mTempSelectSkillList.Add( list[i] );
					}
				}

                if ( mTempSelectSkillList.Count > 0 ) {
                    int index = UnityEngine.Random.Range( 0, mTempSelectSkillList.Count );
                    mTempSelectSkillList[index].DecreaseSkillCoolTime( 1.0f );
                }
            }
            break;

            case eEventType.EVENT_BUFF_ADD_MAX_HP:
				if( evt.battleOptionData != null && evt.battleOptionData.preConditionType != BattleOption.eBOConditionType.None ) {
					if( !GameSupport.IsBOConditionCheck( evt.battleOptionData.preConditionType, evt.battleOptionData.CondValue, this ) ) {
						return true;
					}
				}

				float curHpRate = m_curHp / m_maxHp;
				mAddMaxHpRate += evt.value;

				m_maxHp = mOriginalMaxHp + ( mOriginalMaxHp * mAddMaxHpRate );
				m_curHp = m_maxHp * curHpRate;

				if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
                    if( World.Instance.Player == this ) {
                        World.Instance.UIPlay.m_playerHp.InitUIGaugeUnit( (int)m_curHp, (int)m_maxHp );
                    }
				}
				else {
					World.Instance.UIPVP.Renewal( true );
				}

				break;

			case eEventType.EVENT_BUFF_INCREASE_SUPER_ARMOR_BY_SKILL:
				m_actionSystem.IncreaseSuperArmorDurationBySkill( evt.value );
				break;

			case eEventType.EVENT_STAT_ADD_BUFF_DEBUFF_DURATION:
				AddBuffDebuffDuration( (eBuffDebuffType)evt.value2, (int)evt.value3, evt.value );
				break;

			case eEventType.EVENT_SUMMONS_ATTACK_POWER_INCREASE:
				IncreaseSummonsAttackPowerRate += evt.value;
				break;

            case eEventType.EVENT_INCREASE_CHAR_SKILL_COOL_TIME:
                for( int i = 0; i < mListSelectSkill.Count; i++ ) {
                    mListSelectSkill[i].AddExtraCoolTime( evt.value );
                }
                return true;

            case eEventType.EVENT_INCREASE_CHAR_SUPPORTER_SKILL_COOL_TIME:
                AddExtraSupporterCoolTime( evt.value );
                return true;

            case eEventType.EVENT_DEBUFF_USE_SP: {
                UseSp( evt.value, true );
            }
            return true;
        }

		return false;
	}

	public override void Retarget() {
		base.Retarget();

		WorldPVP worldPVP = World.Instance as WorldPVP;
		if ( worldPVP ) {
			Player enemy = worldPVP.GetCurrentPlayerTeamCharOrNull();
			if ( enemy == this ) {
				enemy = worldPVP.GetCurrentOpponentTeamCharOrNull();
			}

			if ( enemy ) {
				SetMainTarget( enemy.GetHighestAggroUnit( this ) );
			}
		}
	}

	protected virtual void AddAttack( float atkRatio, int attackCount, int criComboCount, int effID ) {
		AniEvent.sEvent evt = AniEvent.CreateEvent( m_onAttackAniEvent );
		evt.behaviour = eBehaviour.Attack;
		evt.hitEffectId = effID;
		evt.hitDir = eHitDirection.None;

		decideCritical = false;

		if( m_addAttackCount >= criComboCount ) {
			decideCritical = true;
			m_addAttackCount = 0;
		}

		for( int i = 0; i < attackCount; i++ ) {
			OnAttack( evt, atkRatio, true );
		}
	}

    protected virtual void AddSpAttack( float atkRatio, int attackCount, float sp, int effID ) {
        if( m_curSp < sp || m_onAttackAniEvent == null ) {
            return;
		}

        AniEvent.sEvent evt = AniEvent.CreateEvent( m_onAttackAniEvent );
        evt.behaviour = eBehaviour.Attack;
        evt.hitEffectId = effID;
        evt.hitDir = eHitDirection.None;

        if( m_onAttackAniEvent.behaviour == eBehaviour.Projectile ) {
            if ( LastProjectile == null || LastProjectile.BoxCol == null ) {
                return;
			}

            evt.hitBoxSize = LastProjectile.BoxCol.size;
            evt.hitBoxPosition = LastProjectile.TargetCollider ? LastProjectile.TargetCollider.GetCenterPos() : LastProjectile.transform.position;
            evt.hitBoxBoneName = "";
            evt.useOnlyHitBoxPos = true;
        }

        decideCritical = false;
        UseSp( sp );

        for( int i = 0; i < attackCount; i++ ) {
            OnAttack( evt, atkRatio, true );
        }
    }

    private IEnumerator AttackRepeat( BaseEvent evt ) {
        BuffEvent buffEvt = evt as BuffEvent;

        int effId = 0;
        float value1 = 0.0f;
        float attackPower = evt.sender.attackPower;

        ActionEvent actionEvt = evt as ActionEvent;
        if( actionEvt != null ) {
            effId = actionEvt.EffId;
            value1 = evt.battleOptionData.value;
        }
        else if( buffEvt != null ) {
            effId = buffEvt.effId;
            value1 = evt.value;
            attackPower = buffEvt.ExtraValue > 0.0f ? buffEvt.ExtraValue : attackPower;
        }

        eBehaviour behaviour = eBehaviour.Attack;

        AniEvent.sEvent attackEvt = new AniEvent.sEvent();
        attackEvt.behaviour = behaviour;
        attackEvt.hitEffectId = 0;
        attackEvt.hitDir = eHitDirection.None;
        attackEvt.atkRatio = 1.0f;

        bool onlyDamageHit = evt.battleOptionData.value2 >= 1.0f ? true : false;

        AttackEvent atkEvent = new AttackEvent();
        atkEvent.SkipCheckOwnerAction = onlyDamageHit;

        if( m_actionSystem ) {
            ActionHit actionHit = m_actionSystem.GetCurrentAction<ActionHit>();
            if( actionHit && actionHit.State == ActionHit.eState.Float ) // 공중으로 뜨는 상태면 대미지만 주도록
            {
                onlyDamageHit = true;
            }
        }

        float checkTime = 0.0f;
        List<UnitCollider> list = null;

        while( m_curHp > 0.0f ) {
            checkTime += Time.fixedDeltaTime;

            if( checkTime >= evt.battleOptionData.tick ) {
                if( evt.battleOptionData.targetValue > 0.0f ) {
                    list = GetTargetColliderListByAround( transform.position, evt.battleOptionData.targetValue );
                }
                else {
                    list = GetTargetColliderList();
				}

                if( list != null && list.Count > 0 ) {
                    if( effId > 0 ) {
                        EffectManager.Instance.Play( this, effId, EffectManager.eType.Common );
					}

                    World.Instance.EnemyMgr.SortListTargetByNearDistance( this, ref list );

                    atkEvent.Set( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, evt.sender, evt.battleOptionData.toExecuteType, attackEvt,
                                  attackPower * value1, eAttackDirection.Skip, false, 0, EffectManager.eType.None, list, 0.0f, true, false, onlyDamageHit,
                                  0, null, (int)evt.battleOptionData.value2 );

                    EventMgr.Instance.SendEvent( atkEvent );
                }

                checkTime = 0.0f;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    /*protected void StackOnHit()
    {
        List<sStackBattleOption> list = m_listStackBattleOption.FindAll(x => x.timingType == BattleOption.eBOTimingType.OnHit);
        if (list == null || list.Count <= 0)
            return;

        for (int i = 0; i < list.Count; i++)
            list[i].OnStack();
    }

    protected void StackOnCombo()
    {
        List<sStackBattleOption> list = m_listStackBattleOption.FindAll(x => x.timingType == BattleOption.eBOTimingType.UpdateCombo);
        if (list == null || list.Count <= 0)
            return;

        for (int i = 0; i < list.Count; i++)
            list[i].OnStack();
    }

    protected void ClearComboStack()
    {
        List<sStackBattleOption> list = m_listStackBattleOption.FindAll(x => x.timingType == BattleOption.eBOTimingType.UpdateCombo);
        if (list == null || list.Count <= 0)
            return;

        for (int i = 0; i < list.Count; i++)
            list[i].ClearStack();
    }*/

    protected virtual void OnEventInput( BaseEvent evt ) {
		InputEvent inputEvt = evt as InputEvent;
		if( inputEvt == null ) {
			return;
		}

		if( evt.eventType == eEventType.EVENT_PLAYER_INPUT_DIR ) {
			if( inputEvt.dir == Vector3.zero && !IsAutoMove ) {
				if( !IsHelper && !AutoPlay ) {
					SetAutoMove( true );
				}
			}
			else if( inputEvt.dir != Vector3.zero && IsAutoMove ) {
				SetAutoMove( false );
			}
		}

		if( m_actionSystem.currentAction != null ) {
			ActionMoveByDirection action = m_actionSystem.GetCurrentAction<ActionMoveByDirection>();
			if( action ) {
				action.OnUpdating( new ActionParamMoveByDirection( inputEvt.dir, inputEvt.SkipRunStop ) );
				return;
			}
		}

		if( m_actionSystem.currentAction != null ) {
			return;
		}

		if( GameSupport.IsInGameTutorial() ) {
			if( GameInfo.Instance.UserData.GetTutorialState() == (int)eTutorialState.TUTORIAL_STATE_Init ) {
				GameInfo.Instance.UserData.SetTutorial( GameInfo.Instance.UserData.GetTutorialState(), 2 );
			}
		}

		CommandAction( eActionCommand.MoveByDirection, new ActionParamMoveByDirection( inputEvt.dir, inputEvt.SkipRunStop ) );
        mBeforeInputDir = inputEvt.dir;
    }

	//protected virtual void OnEventJump()
	//{
	//    /*QTEMgr qteMgr = World.Instance.qteMgr;
	//    if (qteMgr.isQTE == true)
	//    {
	//        if (qteMgr.IsPrepareRecieveButton() == true)
	//            qteMgr.ExecuteQTE(ActionQTEBase.eQTEButton.Jump, this);
	//
	//        return;
	//    }*/
	//
	//    if (m_actionSystem.IsCurrentUSkillAction() == true || m_actionSystem.IsCurrentAction(eActionCommand.JumpDownAttack) || !isGrounded)
	//        return;
	//
	//    /*ActionJump actionJump = m_actionSystem.currentAction as ActionJump;
	//    if (actionJump != null && isGrounded == false)
	//        actionJump.OnUpdating(null);
	//    else if (isGrounded == true)
	//        CommandAction(eActionCommand.Jump, null);*/
	//
	//    CommandAction(eActionCommand.JumpDownAttack, null);
	//}

	protected virtual void OnEventAtk()
    {
        /*QTEMgr qteMgr = World.Instance.qteMgr;
        if (qteMgr.isQTE == true)
        {
            if (qteMgr.IsPrepareRecieveButton() == true)
                qteMgr.ExecuteQTE(ActionQTEBase.eQTEButton.Attack, this);

            return;
        }*/

        if (!m_actionSystem.IsCurrentUSkillAction())
        {
            bool isJumpAttack = false;

            ActionBase currentAction = m_actionSystem.currentAction;
            if (currentAction != null && (currentAction.actionCommand == eActionCommand.Jump || currentAction.actionCommand == eActionCommand.UpperJump ||
                                          currentAction.actionCommand == eActionCommand.JumpAttack) && isGrounded == false && m_cmptJump.highest >= 0.8f)
            {
                if (World.Instance.EnemyMgr.HasFloatingEnemy())
                {
                    if (currentAction != null && currentAction.actionCommand == eActionCommand.JumpAttack)
                    {
                        isJumpAttack = true;
                        currentAction.OnUpdating(null);
                    }
                    else
                    {
                        isJumpAttack = true;
                        CommandAction(eActionCommand.JumpAttack, null);
                    }
                }
            }

            if (!isJumpAttack)
            {
                ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
                if (actionDash && actionDash.IsPossibleToDashAttack())
                {
                    CommandAction(eActionCommand.RushAttack, null);
                }
                else
                {
                    if (currentAction != null && currentAction.actionCommand == eActionCommand.Attack01)
                    {
                        currentAction.OnUpdating(null);
                    }
                    else
                    {
                        CommandAction(eActionCommand.Attack01, null);
                    }
                }
            }
        }
    }

    protected virtual void OnEventDefence( IActionBaseParam param = null )
    {
        if (m_actionSystem.IsCurrentUSkillAction() == true)
            return;

        ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
        if (m_actionSystem.HasAction(eActionCommand.Teleport) && actionDash && actionDash.IsPossibleToDashAttack())
        {
            float checkTime = actionDash.GetEvadeCutFrameLength();
            if (World.Instance.UIPlay.btnDash.deltaTime < checkTime)
            {
                CommandAction(eActionCommand.Teleport, null);
                return;
            }
        }

        CommandAction( eActionCommand.Defence, param );
    }

    protected virtual void OnEventJump()
    {
        if (m_actionSystem.IsCurrentUSkillAction() == true || m_actionSystem.IsCurrentAction(eActionCommand.HoldingDefBtnAttack))
            return;

        ActionBase action = m_actionSystem.GetAction(eActionCommand.HoldingDefBtnAttack);
        if (action == null)
            return;

        CommandAction(eActionCommand.HoldingDefBtnAttack, null);
    }

	protected virtual void OnEventChargeAtkStart( IActionBaseParam param = null ) {
        if( m_actionSystem.IsCurrentSkillAction() || m_actionSystem.IsCurrentUSkillAction() ) {
            return;
        }

		ActionBase actionBase = m_actionSystem.GetAction(eActionCommand.ChargingAttack);
        if( actionBase == null ) {
            return;
        }

		CommandAction( eActionCommand.ChargingAttack, param );
	}

	protected virtual void OnEventChargeAtkEnd( IActionBaseParam param = null ) {
        ActionChargeAttack action = m_actionSystem.currentAction as ActionChargeAttack;
        if( action == null ) {
            return;
        }

        action.OnUpdating( param );
    }

	protected virtual void OnEventSpecialAtk() {
        if( m_actionSystem.IsCurrentSkillAction() || m_actionSystem.IsCurrentUSkillAction() ) {
            return;
        }

		ActionUpperAttack actionUpperAtk = m_actionSystem.GetAction<ActionUpperAttack>(eActionCommand.AttackDuringAttack);
        if( actionUpperAtk == null ) {
            return;
        }

		if( m_actionSystem.IsCurrentAction( eActionCommand.Attack01 ) ) {
            if( !IsHelper ) {
                World.Instance.UIPlay.btnAtk.lockCharge = true;
            }

			CommandAction( eActionCommand.AttackDuringAttack, null );
		}
	}

	protected virtual void OnEventAtkPressStart() {
	}

	protected virtual void OnEventAtkTouchEnd() {
	}

	protected virtual void OnEventUltimateSkill() {
		if( World.Instance.EnemyMgr.IsEmptyEnemy( this ) || UsingUltimateSkill || m_curHp <= 0.0f ) {
			return;
		}

        if( m_curSp < GameInfo.Instance.BattleConfig.USUseSP || m_actionSystem.IsCurrentUSkillAction() ) {
            return;
        }

        if( World.Instance.StageData != null && ( World.Instance.StageType == eSTAGETYPE.STAGE_RAID || World.Instance.StageData.PlayerMode == 1 ) ) {
            for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                if( World.Instance.ListPlayer[i].UsingUltimateSkill ) {
                    return;
				}
			}
		}

		World.Instance.EnemyMgr.SetSpeedRateAll( 0.0f );
		SetSpeedRate( 0.0f );

		if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
			World.Instance.UIPVP.HideCheerBuff();
		}

		CommandAction( eActionCommand.USkill01, null );
	}

	protected virtual void OnEventWeaponSkill() {
        if( boWeapon == null || m_curSp < boWeapon.GetUseWeaponSkillSp() ) {
            return;
        }

		UseWeaponActiveSkill();
	}

	protected override void FixedUpdate() {
		if( !gameObject.activeSelf ) {
			return;
		}

		base.FixedUpdate();
		UpdateEffTarget();

		if( !isClone && ( Mathf.Abs( transform.position.y ) > 150.0f ) ) {
			EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_PLAYER_DEAD, this );

			SetDie();
			Deactivate();
		}
        else if( IsHelper || AutoPlay ) {
            BattleAreaManager bam = World.Instance.EnemyMgr as BattleAreaManager;
            if( bam ) {
                BattleSpawnGroup bsg = bam.GetCurrentSpawnGroup();
                if( bsg && bsg.IsAllMonsterHpIsZero() ) {
                    SendStopInputAutoEvent();
				}
			}
        }

#if UNITY_EDITOR
		if( UnityEngine.Input.GetKeyDown( KeyCode.Alpha6 ) && !OpponentPlayer ) {
            //if( !IsHelper ) {
                AddSp( GameInfo.Instance.BattleConfig.USMaxSP * 0.5f );
            //}
		}
        if( UnityEngine.Input.GetKeyDown( KeyCode.Alpha7 ) && !OpponentPlayer ) {
            for( int i = 0; i < mListSelectSkill.Count; i++ ) {
                mListSelectSkill[i].AddExtraCoolTime( 3.0f );
			}
            //for( int i = 0; i < 3; i++ ) {
            //    ShowClone( i, transform.position, transform.rotation, true );
            //    SetCloneShader( i, Shader.Find( "eTOYLab/Silhouette" ) );
            //}
        }
        if( UnityEngine.Input.GetKeyDown( KeyCode.Alpha8 ) && !OpponentPlayer ) {
            AniEvent.sEvent evt = m_aniEvent.CreateEvent( eBehaviour.Attack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f );

            eHitState hitState = eHitState.Success;
            bool isCritical = false;

            World.Instance.InvinciblePlayer = false;
            OnHit( this, BattleOption.eToExecuteType.Unit, evt, m_maxHp * 5.0f, ref isCritical, ref hitState, null, false, true );
        }
        else if( UnityEngine.Input.GetKeyDown( KeyCode.Alpha9 ) ) {
			List<Unit> list = World.Instance.EnemyMgr.GetActiveEnemies( this );
			for( int i = 0; i < list.Count; i++ ) {
				if( list[i].grade == eGrade.Boss ) {
					continue;
				}

				list[i].actionSystem.CancelCurrentAction();
				list[i].CommandAction( eActionCommand.Die, null );
			}

            SetAutoMove( true );
		}
		else if( UnityEngine.Input.GetKeyDown( KeyCode.Alpha0 ) ) {
			AniEvent.sEvent evt = m_aniEvent.CreateEvent( eBehaviour.Attack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f );
			eHitState hitState = eHitState.Success;
			bool isCritical = false;

			List<Unit> list = World.Instance.EnemyMgr.GetActiveEnemies( this );
			for( int i = 0; i < list.Count; i++ ) {
				list[i].actionSystem.CancelCurrentAction();
				list[i].OnHit( this, BattleOption.eToExecuteType.Unit, evt, 100000000.0f, ref isCritical, ref hitState, null, false, true );
			}

            SetAutoMove( true );
        }
		else if( UnityEngine.Input.GetKeyDown( KeyCode.C ) ) {
            //if( !IsHelper ) {
                AniEvent.sEvent evt = m_aniEvent.CreateEvent( eBehaviour.DownAttack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f );
                eHitState hitState = eHitState.Success;

                bool isCritical = false;
                OnHit( this, BattleOption.eToExecuteType.Unit, evt, m_maxHp * 2.5f, ref isCritical, ref hitState, null, false, true );
            //}
		}
        else if( UnityEngine.Input.GetKeyDown( KeyCode.V ) ) {
			/*
            if( World.Instance.ListPlayer[1] == this ) {
                AniEvent.sEvent evt = m_aniEvent.CreateEvent( eBehaviour.DownAttack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f );
                eHitState hitState = eHitState.Success;

                bool isCritical = false;
                OnHit( this, BattleOption.eToExecuteType.Unit, evt, m_maxHp * 3.0f, ref isCritical, ref hitState, null, false, true );
            }
            */
			/*
            BuffEvent buffEvt = new BuffEvent();
            buffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
            buffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
            buffEvt.Set( m_data.ID, eEventSubject.ActiveEnemiesInRange, eEventType.EVENT_DEBUFF_SPEED_DOWN, this, 0.5f, 10.0f, 0.0f, 10.0f, 0.0f, 20063, buffEvt.effId2, eBuffIconType.Debuff_Speed );

            EventMgr.Instance.SendEvent( buffEvt );
            */

			if ( IsHelper ) {
				AniEvent.sEvent evt = m_aniEvent.CreateEvent( eBehaviour.KnockBackAttack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f );
				eHitState hitState = eHitState.Success;

				bool isCritical = false;
				OnHit( this, BattleOption.eToExecuteType.Unit, evt, 1, ref isCritical, ref hitState, null, false, true );
			}
		}
        else if( UnityEngine.Input.GetKeyDown( KeyCode.R ) ) {
			List<Unit> list = World.Instance.EnemyMgr.GetActiveEnemies(this);
			for( int i = 0; i < list.Count; i++ ) {
				list[i].actionSystem.CancelCurrentAction();
				list[i].CommandAction( eActionCommand.Die, null );
			}

			EndGameEvent evt = new EndGameEvent();

			if( World.Instance.ClearMode == World.eClearMode.BossDie ) {
				evt.Set( eEventType.EVENT_GAME_ENEMY_BOSS_DIE, 0.0f );
			}
			else {
				evt.Set( eEventType.EVENT_GAME_ALL_ENEMY_DEAD, 0.0f );
			}

			EventMgr.Instance.SendEvent( evt );
		}

        else if ( UnityEngine.Input.GetKeyDown( KeyCode.L )) {
            if ( World.Instance.ListPlayer[0] != this ) {
                return;
            }

			AniEvent.sEvent evt = m_aniEvent.CreateEvent( eBehaviour.Attack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f );

			eHitState hitState = eHitState.Success;
			bool isCritical = false;

			World.Instance.InvinciblePlayer = false;
			OnHit( this, BattleOption.eToExecuteType.Unit, evt, m_maxHp * 5.0f, ref isCritical, ref hitState, null, false, true );
			World.Instance.InvinciblePlayer = true;
		}
#endif
	}

	protected override void LateUpdate()
    {
		base.LateUpdate();

        if (object.ReferenceEquals(Camera.main, null) || object.ReferenceEquals(transform, null) ||
            object.ReferenceEquals(m_rigidBody, null) || object.ReferenceEquals(MainCollider, null))
        {
            return;
        }

        // PVP에서 Z축 이동 강제 보정
        if (World.Instance.InGameCamera.Mode == InGameCamera.EMode.PVP)
        {
            if (transform.position.z != 0.0f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
            }
        }
    }

	private IEnumerator ChargingSP() {
		ObscuredFloat fChargePerSec = GameInfo.Instance.BattleConfig.USMaxSP / GameInfo.Instance.BattleConfig.USSPRegenSpeedTime;
		fChargePerSec += fChargePerSec * SPRegenIncRate;

        bool isDirectorPause = false;
        m_chargingSPTime = 0.0f;

		while( m_curSp < GameInfo.Instance.BattleConfig.USMaxSP ) {
            isDirectorPause = ( Director.CurrentPlaying && Director.CurrentPlaying.isPause );

            if( !World.Instance.IsPause && !isDirectorPause ) {
				m_chargingSPTime += deltaTime;

				AddSp( fChargePerSec * m_chargingSPTime );
				m_chargingSPTime = 0.0f;
			}

			yield return null;
		}

		m_curSp = GameInfo.Instance.BattleConfig.USMaxSP;
		m_crChargingSP = null;
	}

	public override void OnEndAction()
    {
        base.OnEndAction();

        if (mInput)
        {
            mInput.ResetBeforeDir();
        }
    }

	private void UpdateEffTarget() {
        if( IsHelper || m_effTarget == null || m_effTarget.gameObject == null ) {
            return;
		}

		if( m_parentEffTarget == null ) {
			if( m_effTarget && m_effTarget.gameObject && m_effTarget.gameObject.activeSelf )
				m_effTarget.gameObject.SetActive( false );

			return;
		}

		if( World.Instance.IsEndGame || m_parentEffTarget.curHp <= 0.0f || !m_parentEffTarget.IsShowMesh ) {
            if( m_effTarget && m_effTarget.gameObject ) {
                m_effTarget.transform.SetParent( null );
                m_effTarget.gameObject.SetActive( false );
            }

			m_parentEffTarget = null;
			SetMainTarget( null );

			return;
		}

		if( mCheckKeepTargeting ) {
			mCheckKeepTargetingTime += fixedDeltaTime;
			if( mCheckKeepTargetingTime >= GameInfo.Instance.BattleConfig.ReleaseTargetingTime ) {
				SetMainTarget( null );
				mCheckKeepTargetingTime = 0.0f;
			}
			else if( m_actionSystem.IsCurrentAnyAttackAction() ) {
				mCheckKeepTargetingTime = 0.0f;
			}
		}

		if( FSaveData.Instance.TurnCameraToTarget ) {
			if( World.Instance.InGameCamera.Mode == InGameCamera.EMode.DEFAULT && 
                m_parentEffTarget && !m_parentEffTarget.isVisible &&
				m_actionSystem.IsCurrentAnyAttackAction() ) {
				World.Instance.InGameCamera.TurnToTarget( m_parentEffTarget.transform.position - ( World.Instance.InGameCamera.transform.right * 3.0f ),
														 GameInfo.Instance.BattleConfig.CameraTurnSpeed, false );
			}
		}

        if( !m_effTarget.gameObject.activeSelf ) {
            m_effTarget.gameObject.SetActive( true );
        }
	}

	public override void OnSuccessAttack( AniEvent.sEvent atkEvt, bool skipBattleOption, int actionTableId, Projectile projectile, bool isCritical ) {
		base.OnSuccessAttack( atkEvt, skipBattleOption, actionTableId, projectile, isCritical );

        if( m_actionSystem == null || m_actionSystem.HasNoAction() == true ) {
            return;
        }

		decideCritical = false;

		UpdateCombo( atkEvt );
		SetCameraShaking( atkEvt );

        if( OnAttackOnce != null ) {
            OnAttackOnce();
            OnAttackOnce = null;
        }
        else if( OnAttackAlways != null ) {
            OnAttackAlways();
        }

		if( GameSupport.IsInGameTutorial() && GameInfo.Instance.UserData.GetTutorialState() == (int)eTutorialState.TUTORIAL_STATE_Init ) {
			World.Instance.HideTutorialHUD();
			GameInfo.Instance.UserData.SetTutorial( GameInfo.Instance.UserData.GetTutorialState(), 3 );
		}
	}

	public override void OnOnlyDamageAttack( AniEvent.sEvent atkEvt, bool skipBattleOption, int actionTableId, Projectile projectile, bool isCritical ) {
		base.OnOnlyDamageAttack( atkEvt, skipBattleOption, actionTableId, projectile, isCritical );
		UpdateCombo( atkEvt );
	}

	protected void UpdateCombo(AniEvent.sEvent atkEvt)
    {
        if (m_clone == false)
            UpdateComobo(atkEvt.behaviour);
        else
        {
            Player owner = m_cloneOwner as Player;
            if (owner != null)
                owner.UpdateComobo(atkEvt.behaviour);
        }
    }

	protected void SetCameraShaking( AniEvent.sEvent atkEvt ) {
		if( IsHelper ) {
			return;
		}

		if( atkEvt.shaking ) {
			//카메라 쉐이킹 강도
			World.Instance.InGameCamera.EnableMotionBlur( 0.3f );
			World.Instance.InGameCamera.PlayShake( this, 0.2f, GameInfo.Instance.BattleConfig.ShakePower, 2.0f, 0.5f );
		}
	}

	private void UpdateComobo( eBehaviour behaviour ) {
		++m_combo;

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP && !IsHelper ) {
			World.Instance.UIPlay.UpdateCombo( this, m_comboResetCondition != eComboResetCondition.ByTime );
		}

        if( m_combo > maxCombo ) {
            maxCombo = m_combo;
        }

		++m_addAttackCount;

		if( m_comboResetCondition == eComboResetCondition.ByTime ) {
			StopCoroutine( "UpdateClearCombo" );
			StartCoroutine( "UpdateClearCombo" );
		}

		// 공격 적중 시 SP 회복량 증가에 의한 SP 회복량 계산
		float fAddSp = m_buffStats.CalcBuffStat( BattleOption.eToExecuteType.Unit, UnitBuffStats.eBuffStatType.AddSPRate, UnitBuffStats.eIncreaseType.Increase,
												 GameInfo.Instance.BattleConfig.USAddSPByCombo, false, false, false, false, this );

		// 공격 적중 시 SP 회복량 감소에 의한 SP 회복량 계산
		fAddSp = m_buffStats.CalcBuffStat( BattleOption.eToExecuteType.Unit, UnitBuffStats.eBuffStatType.AddSPRate, UnitBuffStats.eIncreaseType.Decrease,
										   fAddSp, false, false, false, false, this );

		fAddSp = Mathf.Max( 0.0f, fAddSp );
		AddSp( fAddSp );
	}

	private IEnumerator UpdateClearCombo()
    {
        float time = 0.0f;
        bool end = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!end)
        {
            if (!World.Instance.IsPause)
                time += Time.fixedDeltaTime;

            if (time >= GameInfo.Instance.BattleConfig.ComboClearTime)
                end = true;

            yield return mWaitForFixedUpdate;
        }

        ClearCombo();
    }

	public override void ClearCombo() {
		m_combo = 0;

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			World.Instance.UIPlay.UpdateCombo( this, m_comboResetCondition != eComboResetCondition.ByTime );
		}
	}

	public void SetHp( float hp ) {
		m_curHp = hp;
	}

	public override bool AddHp( BattleOption.eToExecuteType toExecuteType, float add, bool skipCheckDied ) {
        if( isClone ) {
            return cloneOwner.AddHp( toExecuteType, add, skipCheckDied );
		}

		if( !base.AddHp( toExecuteType, add, skipCheckDied ) ) {
            return false;
		}

        if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			World.Instance.UIPlay.AddPlayerHp( this );
		}
		else {
			World.Instance.UIPVP.AddPlayerHp( this );
		}

        return true;
	}

	public override bool AddHpPercentage( BattleOption.eToExecuteType toExecuteType, float addPercentage, bool skipCheckDied ) {
        if( isClone ) {
            return cloneOwner.AddHpPercentage( toExecuteType, addPercentage, skipCheckDied );
        }

        if( !base.AddHpPercentage( toExecuteType, addPercentage, skipCheckDied ) ) {
            return false;
		}

        if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			World.Instance.UIPlay.AddPlayerHp( this );
		}
		else {
			World.Instance.UIPVP.AddPlayerHp( this );
		}

        return true;
	}

	public override bool AddHpPerCost( BattleOption.eToExecuteType toExecuteType, float addPercentage, bool skipCheckDied ) {
        if( isClone ) {
            return cloneOwner.AddHpPerCost( toExecuteType, addPercentage, skipCheckDied );
        }

        if( !base.AddHpPerCost( toExecuteType, addPercentage, skipCheckDied ) ) {
            return false;
        }

        if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			World.Instance.UIPlay.AddPlayerHp( this );
		}
		else {
			World.Instance.UIPVP.AddPlayerHp( this );
		}

        return true;
	}

	public override void SubHp( ObscuredFloat damage, bool isCritical ) {
		base.SubHp( damage, isCritical );

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			World.Instance.UIPlay.SubPlayerHp( this );
		}
		else {
			World.Instance.UIPVP.SubPlayerHp( this );
		}
	}

	public void AddSp( ObscuredFloat add ) {
        if( m_curSp == GameInfo.Instance.BattleConfig.USMaxSP ) {
            return;
        }

		// 1-3 스킬 사용 튜토리얼에선 SP 안참
		if( GameSupport.IsInGameTutorial() && GameInfo.Instance.UserData.GetTutorialState() == (int)eTutorialState.TUTORIAL_STATE_Stage3Clear &&
			GameInfo.Instance.UserData.GetTutorialStep() == 1 ) {
			return;
		}

		m_curSp = Mathf.Clamp( m_curSp + add, 0.0f, GameInfo.Instance.BattleConfig.USMaxSP );

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
            if( boWeapon != null ) {
                World.Instance.UIPlay.ShowWpnSkillBtn( this );
            }

			World.Instance.UIPlay.AddPlayerSp( this, boWeapon == null ? 0.0f : boWeapon.GetUseWeaponSkillSp() );
		}
		else {
			World.Instance.UIPVP.AddPlayerSp( this );
		}

		if( m_curSp >= GameInfo.Instance.BattleConfig.USMaxSP ) {
			ExecuteBattleOption( BattleOption.eBOTimingType.OnMaxSP, 0, null );
			ExecuteBattleOption( BattleOption.eBOTimingType.MaxSP, 0, null );
		}
	}

	public override void UseSp( ObscuredFloat sp, bool decrease = false ) {
		if( sp <= 0.0f ) {
			return;
		}

		float beforeSp = m_curSp;
		base.UseSp( sp );

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			if( !decrease ) {
				World.Instance.UIPlay.SubPlayerSp( this, beforeSp, boWeapon == null ? 0.0f : boWeapon.GetUseWeaponSkillSp() );
			}
			else {
				World.Instance.UIPlay.DecreasePlayerSp( this, boWeapon == null ? 0.0f : boWeapon.GetUseWeaponSkillSp() );
			}

			if( boWeapon != null && m_curSp < boWeapon.GetUseWeaponSkillSp() )
				World.Instance.UIPlay.ShowWpnSkillBtn( this );
		}
		else {
			World.Instance.UIPVP.SubPlayerSp( this, beforeSp );
		}

		// 대마력 게이지가 가득 찬 상태에서 스탯 증가한게 있으면 여기서 제거
		if( m_curSp < GameInfo.Instance.BattleConfig.USMaxSP ) {
			if( m_boSupporter != null ) {
				BattleOption.sBattleOptionData data = m_boSupporter.GetBattleOptionInfo(BattleOption.eBOTimingType.MaxSP);
				if( data != null ) {
					EndBattleOption( data.battleOptionSetId );
					SetSpeedRateByCalcBuff( BattleOption.eToExecuteType.Supporter, true, true );
				}
			}

			if( boWeapon != null ) {
				BattleOption.sBattleOptionData data = boWeapon.GetBattleOptionInfo(BattleOption.eBOTimingType.MaxSP);
				if( data != null ) {
					EndBattleOption( data.battleOptionSetId );
					SetSpeedRateByCalcBuff( BattleOption.eToExecuteType.Weapon, true, true );
				}
			}
		}

		if( m_crChargingSP == null ) {
			m_crChargingSP = World.Instance.StartCoroutine( ChargingSP() );
		}
	}

	public override void SpRegenIncRate( ObscuredFloat value ) {
		SPRegenIncRate += value;

		Utility.StopCoroutine( World.Instance, ref m_crChargingSP );
		m_crChargingSP = World.Instance.StartCoroutine( ChargingSP() );
	}

	public override bool StopBT() {
		if( base.StopBT() ) {
            if( mInput ) {
                mInput.SendEvent( eEventType.EVENT_PLAYER_INPUT_CHARGE_ATK_END, false, null, null );
            }

            return true;
		}

        return false;
	}

	public override bool OnHit( Unit attacker, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent attackerAniEvt, ObscuredFloat damage,
								ref bool isCritical, ref eHitState hitState, Projectile projectile, bool isUltimateSkill, bool skipMaxDamageRecord ) {
		if ( m_pause || World.Instance.IsEndGame ) {
			return false;
		}

#if UNITY_EDITOR
        //World.Instance.InvinciblePlayer = false;

        if ( World.Instance.InvinciblePlayer || ( World.Instance.TestScene && World.Instance.TestScene.IsInvinciblePlayer ) ) { 
            SendRemoveHitTarget();
			return false;
		}

        if ( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
            WorldPVP worldPVP = World.Instance as WorldPVP;
            if ( worldPVP ) {
                if ( worldPVP.InvinciblePlayerTeam && !OpponentPlayer ) {
                    SendRemoveHitTarget();
                    return false;
                }
                
                if ( worldPVP.InvincibleOpponentTeam && OpponentPlayer ) {
                    SendRemoveHitTarget();
                    return false;
                }
			}
		}
#endif

        float beforeCurHp = m_curHp;
		float beforeCurShield = m_curShield;

		bool breakShield = base.OnHit( attacker, toExecuteType, attackerAniEvt, damage, ref isCritical, ref hitState, 
                                       projectile, isUltimateSkill, skipMaxDamageRecord );

		if ( hitState == eHitState.OnlyEffect ) {
			return false;
		}

		if ( m_curHp <= 0.0f ) {
			if ( m_boSupporter != null && ( m_boSupporter.HasBattleOption( BattleOption.eBOTimingType.OnDie, BattleOption.eBOFuncType.Heal ) ||
										    m_boSupporter.HasBattleOption( BattleOption.eBOTimingType.OnDie, BattleOption.eBOFuncType.DotHeal ) ) ) {
				ExecuteBattleOption( BattleOption.eBOTimingType.OnDie, 0, null );
				return false;
			}
			else if ( boWeapon != null && ( boWeapon.HasBattleOption( BattleOption.eBOTimingType.OnDie, BattleOption.eBOFuncType.Heal ) ||
										    boWeapon.HasBattleOption( BattleOption.eBOTimingType.OnDie, BattleOption.eBOFuncType.DotHeal ) ) ) {
				ExecuteBattleOption( BattleOption.eBOTimingType.OnDie, 0, null );
				return false;
			}
            else if ( HoldDying ) {
                return false;
			}
			else {
				isDying = true;

				m_actionSystem.CancelCurrentAction();
				mInput.Pause( true );

				if ( !IsHelper ) {
					GameUIManager.Instance.HideUI( "GamePlayPanel", false );
				}
			}

			if ( mAI ) {
				mAI.StopBT();
			}
		}

		ActionDash actionDash = m_actionSystem.GetAction<ActionDash>( eActionCommand.Defence );
		actionDash.BlockEmergencyEvade = attackerAniEvt.blockEmergencyEvade;

		if ( hitState == eHitState.Success ) {
			ActionParamHit param = new ActionParamHit( attacker, attackerAniEvt.behaviour, attacker.GetAirAttackJumpPower(),
													   attackerAniEvt.hitDir, attackerAniEvt.hitEffectId, isCritical,
													   attacker.aniEvent != null ? attacker.aniEvent.GetCurCutFrameLength() : 0.0f,
													   hitState, attackerAniEvt.atkAttr );

			ActionHit actionHit = m_actionSystem.GetCurrentAction<ActionHit>();
			if ( actionHit != null && actionHit.State != ActionHit.eState.StandUp ) {
				if ( actionHit.State == ActionHit.eState.Normal && attackerAniEvt.behaviour != eBehaviour.Attack ) {
					m_actionSystem.CancelCurrentAction();
					CommandAction( eActionCommand.Hit, param );
				}
				else {
					actionHit.OnUpdating( param );
				}
			}
			else {
				m_actionSystem.CancelCurrentAction();
				CommandAction( eActionCommand.Hit, param );
			}
		}
		else if ( hitState == eHitState.OnlyDamage && m_curHp <= 0.0f ) {
			m_actionSystem.CancelCurrentAction();
			CommandAction( eActionCommand.Die, null );
		}

		if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			if ( m_comboResetCondition == eComboResetCondition.ByHit ) {
				ClearCombo();
			}
		}
		else {
			if ( !OpponentPlayer ) {
				World.Instance.UIPVP.AddCheerUp();
			}

			if ( beforeCurHp != m_curHp || beforeCurShield != m_curShield ) {
				World.Instance.UIPVP.SubPlayerHp( this );
			}
		}

		// 히트 시 초기화 되는 버프들은 여기서 제거
		if ( m_boSupporter != null ) {
			BattleOption.sBattleOptionData data = m_boSupporter.GetBattleOptionInfo( BattleOption.eBOConditionType.CancelOnHit );
			if ( data != null ) {
				m_buffStats.RemoveBuffStat( data.battleOptionSetId );
				m_cmptBuffDebuff.RemoveExecute( data.battleOptionSetId );
			}
		}

		if ( boWeapon != null ) {
			BattleOption.sBattleOptionData data = boWeapon.GetBattleOptionInfo( BattleOption.eBOConditionType.CancelOnHit );
			if ( data != null ) {
				m_buffStats.RemoveBuffStat( data.battleOptionSetId );
				m_cmptBuffDebuff.RemoveExecute( data.battleOptionSetId );
			}
		}

		++hitCount;
		return breakShield;
	}

	public void GetItem( DropItem item ) {
		if( item.type == global::DropItem.eType.Coin ) {
			++goldCount;
			World.Instance.UIPlay.LbGold.text = goldCount.ToString();
		}
	}

	public void SetGoldCount(int gold)
    {
        goldCount = gold;
    }

    protected override void OnCollisionEnter(Collision col)
    {
        base.OnCollisionEnter(col);

        if (col.collider.CompareTag("Wall_Barricade"))
        {
            Vector3 centerPos = GetCenterPos();

            if (mEffBarricadeBlock && col.contacts != null && col.contacts.Length > 0)
            {
                Vector3 contactPoint = col.contacts[0].point;
                contactPoint.y = centerPos.y;

                mEffBarricadeBlock.transform.position = contactPoint;
                mEffBarricadeBlock.transform.rotation = Quaternion.LookRotation(centerPos - contactPoint);

                mEffBarricadeBlock.Play();
            }
        }
    }

	protected void SendInputAutoEvent( bool skipCheckBeforeDir = false ) {
		if( !skipCheckBeforeDir && mInputDir == Vector3.zero && mBeforeInputDir == Vector3.zero ) {
			return;
		}

		if( World.Instance.InGameCamera.Mode == InGameCamera.EMode.SIDE ) {
			if( World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.Z ) {
				mInputDir.y = 0.0f;
			}
			else if( World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.X ) {
				mInputDir.x = 0.0f;
			}
		}

		mInputEvent.eventType = eEventType.EVENT_PLAYER_INPUT_DIR_AUTO;
		mInputEvent.dir = mInputDir;
		mInputEvent.beforeDir = mBeforeInputDir;
		mInputEvent.SkipRunStop = true;

		EventMgr.Instance.SendEvent( mInputEvent, null );
		mBeforeInputDir = mInputDir;
	}

    public void SendStopInputAutoEvent() {
        mInputDir = Vector3.zero;
        SendInputAutoEvent();
    }

	public virtual void SetAutoMove( bool on ) {
		if ( !IsActivate() || AI == null || World.Instance.StageType == eSTAGETYPE.STAGE_PVP || m_curHp <= 0.0f ) {
			return;
		}

		if ( on && Input && Input.isPause ) {
			Utility.StopCoroutine( this, ref mCrUpdateFindPath );
			IsAutoMove = false;

			return;
		}

		IsAutoMove = on;

		if ( on ) {
			StopBT();

			Utility.StopCoroutine( this, ref mCrUpdateFindPath );
			mCrUpdateFindPath = StartCoroutine( UpdateFindPath() );
		}
		else {
			Utility.StopCoroutine( this, ref mCrUpdateFindPath );

			SendStopInputAutoEvent();
			StartBT();
		}
	}

	public void StopAutoMove() {
        IsAutoMove = false;
        Utility.StopCoroutine( this, ref mCrUpdateFindPath );

        SendStopInputAutoEvent();
    }

    public virtual void OnAfterChangePlayableChar() {
	}

    protected virtual IEnumerator UpdateFindPath() {
		BattleAreaManager battleAreaMgr = World.Instance.EnemyMgr as BattleAreaManager;
		if( battleAreaMgr == null ) {
			SetAutoMove( false );
			yield break;
		}

		BattleArea battleArea = null;

        // 스테이지 시작 전
        if( battleAreaMgr.CurBattleAreaIndex == -1 )  {
			battleArea = battleAreaMgr.GetBattleArea( 0 );
			mAutoMoveDest = battleArea.GetCenterPos();

			battleArea = null;
		}
        // 스테이지 시작 후
        else {
			battleArea = battleAreaMgr.GetCurrentBattleArea();

            // 현재 배틀 에어리어가 꺼져 있으면 PortalEntry 사용 안하고 적 다 잡은 상태
            if( !battleArea.gameObject.activeSelf ) 
			{
				BattleArea nextBattleArea = battleAreaMgr.GetNextBattleAreaOrNull(); 
				if( nextBattleArea ) {
					if( nextBattleArea.portalEntry ) {
						mAutoMoveDest = nextBattleArea.portalEntry.transform.position;
					}
                    else if( nextBattleArea.kPathEffect && nextBattleArea.kPathEffect.activeSelf ) {
                        mAutoMoveDest = nextBattleArea.kPathEffect.transform.position;
                    }
					else { // 최종 목적지는 다음 배틀 에어리어의 센터
                        mAutoMoveDest = nextBattleArea.GetCenterPos();
					}
				}
			}
            // 현재 배틀 에어리어가 켜져 있고 포털 엔트리도 켜져 있으면 포털 엔트리를 최종 목적지로 함.
            else if( battleArea.portalEntry && battleArea.portalEntry.gameObject.activeSelf ) {
				mAutoMoveDest = battleArea.portalEntry.transform.position;
			}
			else {
				// 현재 배틀 에어리어는 켜져 있는데 포털 엔트리가 없거나 꺼져있으면 가까운 적을 찾음
				Unit nearestTarget = World.Instance.EnemyMgr.GetNearestTarget(this, false);
				if( nearestTarget ) {
					mAutoMoveDest = nearestTarget.transform.position;
				}
				else {
					mAutoMoveDest = transform.position;
					
                    SetAutoMove( false );
                    SendStopInputAutoEvent();

                    // 적도 없으면 그냥 나가자
                    yield break;
				}
			}
		}

        List<Vector3> listPathPoint = null;
        if( battleArea ) {
            listPathPoint = battleArea.GetPathPointListOrNull( transform.position );
            if( listPathPoint == null ) { // 배틀 에어리어에 패쓰가 없으면 맵에 기본으로 깔린걸 가져옴
                listPathPoint = PathMgr.Instance.GetPathPointListOrNull( transform.position );
            }
        }

        if( listPathPoint == null ) { // 패쓰가 맵에도 없고 배틀 에어리어에도 없으면 목적지만 설정
            listPathPoint = new List<Vector3>();
        }

        listPathPoint.Add( mAutoMoveDest );

        if( listPathPoint.Count <= 0 ) {
            SetAutoMove( false );
            yield break;
        }

        while( !isGrounded ) {
            yield return null;
        }

        int curIndex = 0;
        Vector3 curDest = listPathPoint[curIndex];
        curDest.y = transform.position.y;

        mBeforeInputDir = Vector3.zero;

        while( true ) {
            if( isGrounded ) {
                if( AI && World.Instance.EnemyMgr.HasAliveMonster() ) {
                    SetAutoMove( false );
                    SendStopInputAutoEvent();

                    break;
                }
                else {
                    Vector3 finalDest = listPathPoint[listPathPoint.Count - 1];
                    finalDest.y = curDest.y = transform.position.y;

                    if( Vector3.Distance( transform.position, curDest ) <= 0.2f ) {
                        ++curIndex;
                        if( curIndex >= listPathPoint.Count ) {
                            SetAutoMove( false );
                            SendStopInputAutoEvent();

                            break;
                        }

                        curDest = listPathPoint[curIndex];
                        curDest.y = transform.position.y;
                    }
                    else if( Vector3.Distance( transform.position, mAutoMoveDest ) <= 0.2f ) {
                        SetAutoMove( false );
                        SendStopInputAutoEvent();

                        break;
                    }
                }

                mInputDir = ( curDest - transform.position ).normalized * 1000.0f;
                SendInputAutoEvent();
            }

            yield return null;
        }
    }

	protected override void UpdateShieldGauge() {
        if( IsHelper ) {
            return;
		}

        World.Instance.UIPlay.SubPlayerHp( this );
	}

	private void OnEndHit()
    {
        if(mInput == null)
        {
            return;
        }

        mInput.Pause(false);
    }

    public override void Pause()
    {
        if (gameObject.activeSelf == false)
            return;

        if (mInput)
        {
            mInput.Pause(true);
        }

        //UpdateDirection(Vector3.zero);

        if(mAI)
        {
            mAI.StopBT();
        }

        if(!m_actionSystem.IsCurrentAction(eActionCommand.Die)) // 이거 말고 뭐 더 좋은 방법 없나??
            m_actionSystem.CancelCurrentAction();

        if (isGrounded == false)
            SetFallingRigidBody();

        PlayAniImmediate(eAnimation.Idle01);

        for(int i = 0; i < m_listClone.Count; i++)
        {
            m_listClone[i].Pause();
        }

        base.Pause();
    }

    public override void Resume()
    {
        if (gameObject.activeSelf == false)
            return;

        if (mInput)
        {
            mInput.Pause(false);
        }

        if(mAI)
        {
            ResetBT();
        }

        for (int i = 0; i < m_listClone.Count; i++)
        {
            m_listClone[i].Resume();
        }

        base.Resume();
        //CommandAction(eActionCommand.Idle);
    }

    public void EndGame(bool win)
    {
        mInput.Pause(true);
        ForceSetSuperArmor(eSuperArmor.Invincible);

        if (win == false)
            MissionFail();
    }

    public void MissionFail()
    {
        mInput.Pause(true);
        StartCoroutine("WaitingForPlayerOnGround");
    }

    private IEnumerator WaitingForPlayerOnGround()
    {
        while (isGrounded == false)
            yield return null;

        actionSystem.CancelCurrentAction();
        CommandAction(eActionCommand.Die, null);
    }

    public override void SetDie( bool setHpToZero = false )
    {
        base.SetDie( setHpToZero );

        if(!isGrounded)
        {
            SetFallingRigidBody();
        }

        m_actionSystem.CancelCurrentAction();
        CommandAction(eActionCommand.Die, null);
    }

    /*
    public void StartSupporterCoolTime()
    {
        if(boSupporter == null)
        {
            return;
        }

        float supporterCoolTime = boSupporter.data.TableData.CoolTime;
        CurSupporterCoolTime = Mathf.Clamp(supporterCoolTime - (supporterCoolTime * SupporterCoolTimeDecRate), 0, supporterCoolTime);

#if UNITY_EDITOR
        if (GameInfo.Instance.GameConfig.TestMode)
        {
            CurSupporterCoolTime = 3.0f;
        }
#endif

        Utility.StopCoroutine(World.Instance, ref mCrUpdateSupporterCoolTime);
        mCrUpdateSupporterCoolTime = World.Instance.StartCoroutine(UpdateSupporterCoolTime());
    }

    private IEnumerator UpdateSupporterCoolTime()
    {
        WorldPVP worldPVP = World.Instance as WorldPVP;
        while (CurSupporterCoolTime > 0.0f)
        {
            if (!HoldSupporterSkillCoolTime)
            {
                if (worldPVP)
                {
                    if (worldPVP.IsBattleStart)
                    {
                        CurSupporterCoolTime -= Time.fixedDeltaTime;

                        if (!OpponentPlayer)
                        {
                            Debug.Log("서포터 쿨타임 도는 중~~");
                        }
                    }

                    if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP && !OpponentPlayer)
                    {
                        World.Instance.UIPVP.UpdateSupporterCoolTime(CurSupporterCoolTime, m_boSupporter.data.TableData.CoolTime);
                    }
                }
                else
                {
                    CurSupporterCoolTime -= Time.fixedDeltaTime;
                }
            }
            else
            {
                Debug.Log(tableName + "의 서포터 쿨타임 막힘!!!");
            }

            yield return mWaitForFixedUpdate;
        }

        if (!OpponentPlayer)
        {
            Debug.Log("서포터 쿨타임 끝남");
        }

        if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP && !OpponentPlayer)
        {
            SoundManager.sSoundInfo info = VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.SkillGauge, m_boSupporter.data.TableID);
            if(info != null && info.clip)
            {
                World.Instance.UIPVP.SupporterSpeak(info.clip.length);
            }
        }

        InitSupporterCoolTime(false);
    }
    */

    public void UseWeaponActiveSkill()
    {
        if (boWeapon == null || m_curHp <= 0.0f)
        {
            return;
        }

        //boWeapon.Execute(BattleOption.eBOTimingType.Use);
        boWeapon.Execute(BattleOption.eBOTimingType.UseAction);
    }

    public bool HasWeaponActiveSkill()
    {
        if (boWeapon == null)
            return false;

        return boWeapon.HasActiveSkill();
    }

	public bool IsActiveWeaponSkill() {
        if( boWeapon == null ) {
            return false;
        }

        if( boWeapon.GetUseWeaponSkillSp() > 0.0f && m_curSp >= boWeapon.GetUseWeaponSkillSp() ) {
            return true;
        }

		return false;
	}

	public void SetNextBattleAreaNavigator(Vector3 nextBattleAreaPos)
    {
        if (mBattleAreaNavigator == null)
            return;

        mBattleAreaNavigator.SetNextBattleArea(nextBattleAreaPos);
    }

	public void HideBattleAreaNavigator() {
		if( IsHelper || mBattleAreaNavigator == null )
			return;

		mBattleAreaNavigator.Show( false );
	}

	public void CopyNextBattleAreaNavigator(Player player)
    {
        if(player.mBattleAreaNavigator == null || !player.mBattleAreaNavigator.IsShow())
        {
            return;
        }

        mBattleAreaNavigator.Show( true );
        mBattleAreaNavigator.SetNextBattleArea(player.mBattleAreaNavigator.Dest);
    }

	public void AddEnemyNavigatorTarget() {
		EnemyNavigator navigator = ResourceMgr.Instance.CreateFromAssetBundle<EnemyNavigator>("unit", "Unit/Character/EnemyNavigator.prefab");

		if( navigator ) {
			navigator.Init( Color.white );
			mListEnemyNavigator.Add( navigator );
		}
	}

	public void SetEnemyNavigatorTarget(int index, Unit target)
    {
        if(mListEnemyNavigator.Count <= 0)
        {
            return;
        }

        if(index < 0 || index >= mListEnemyNavigator.Count)
        {
            Debug.LogError("네비게이터 생성 개수 잘못됐다-");
            return;
        }

        mListEnemyNavigator[index].SetTarget(target);
    }

    public void SetEnemyNavigatorTarget(Unit target)
    {
        EnemyNavigator find = mListEnemyNavigator.Find(x => !x.HasTarget());
        if(find == null)
        {
            Debug.LogError("남는 적 내비가 없습니다.");
            return;
        }

        find.SetTarget(target);
    }

    public void ClearEnemyNavigators()
    {
        for (int i = 0; i < mListEnemyNavigator.Count; i++)
            mListEnemyNavigator[i].SetTarget(null);
    }

    public void CopyEnemyNavigators(Player player)
    {
        ClearEnemyNavigators();

        for(int i = 0; i < player.mListEnemyNavigator.Count; i++)
        {
            if(!player.mListEnemyNavigator[i].HasTarget())
            {
                continue;
            }

            EnemyNavigator find = mListEnemyNavigator.Find(x => !x.HasTarget());
            if(!find)
            {
                continue;
            }

            find.SetTarget(player.mListEnemyNavigator[i].Target);
        }
    }

	public void SetOtherObjectNavigator( Unit otherObject ) {
		mOtherObjectNavigator = ResourceMgr.Instance.CreateFromAssetBundle<EnemyNavigator>( "unit", "Unit/Character/EnemyNavigator.prefab" );
		if( mOtherObjectNavigator == null ) {
			Debug.LogError( "OtherObject 네비게이터를 생성할 수 없습니다." );
			return;
		}

		mOtherObjectNavigator.Init( new Color( 0.7f, 0.9f, 0.11f ), 1.1f );
		mOtherObjectNavigator.SetTarget( otherObject );
	}

	public void ClearOtherObjectNavigator()
    {
        if(mOtherObjectNavigator == null)
        {
            return;
        }

        mOtherObjectNavigator.SetTarget(null);
    }

    public void CopyOtherObjectNavigator(Player player)
    {
        if(player.mOtherObjectNavigator == null || !player.mOtherObjectNavigator.HasTarget())
        {
            return;
        }

        mOtherObjectNavigator = player.mOtherObjectNavigator;
        mOtherObjectNavigator.SetTarget(player.mOtherObjectNavigator.Target);
    }

	public void AddGateNavigator( EnemyGate gate ) {
		EnemyNavigator navigator = mListGateNavigator.Find(x => !x.HasTarget());
		if( navigator == null ) {
			navigator = ResourceMgr.Instance.CreateFromAssetBundle<EnemyNavigator>( "unit", "Unit/Character/EnemyNavigator.prefab" );
		}

		if( navigator ) {
			navigator.Init( new Color( 1.0f, 0.5f, 0.15f ), 1.1f );
			navigator.SetTarget( gate );

			mListGateNavigator.Add( navigator );
		}
	}

	public void ClearGateNavigator()
    {
        for(int i = 0; i < mListGateNavigator.Count; i++)
        {
            mListGateNavigator[i].SetTarget(null);
        }
    }

    public override void OnEndGame()
    {
        base.OnEndGame();

        HideAllClone();
        StopCoroutine("UpdateClearCombo");

        ClearCombo();
    }

	public WeaponData GetCurrentWeaponDataOrNull()
	{
		if(mCurWeaponIndex < 0 || mCurWeaponIndex >= mWeaponDatas.Length)
		{
			return null;
		}

		return mWeaponDatas[mCurWeaponIndex];
	}

    public WeaponData GetWaitWeaponDataOrNull() {
        if( mCurWeaponIndex < 0 || mWeaponDatas.Length <= 1 ) {
            return null;
		}

        if( mCurWeaponIndex == 0 ) {
            return mWeaponDatas[1];
		}
        else {
            return mWeaponDatas[0];
        }
	}

    public CostumeUnit.sWeaponItem GetCurrentWeaponItemOrNull()
    {
        if(m_costumeUnit == null)
        {
            return null;
        }

        return m_costumeUnit.GetWeaponItemOrNull(mCurWeaponIndex);
    }

    public AttachObject GetCurrentLWeaponOrNull()
    {
        CostumeUnit.sWeaponItem item = m_costumeUnit.GetWeaponItemOrNull(mCurWeaponIndex);
        if(item == null)
        {
            return null;
        }

        return item.LeftObj;
    }

    public AttachObject GetCurrentRWeaponOrNull()
    {
        CostumeUnit.sWeaponItem item = m_costumeUnit.GetWeaponItemOrNull(mCurWeaponIndex);
        if (item == null)
        {
            return null;
        }

        return item.RightObj;
    }

	public Unit GetCurrentUnitWeaponOrNull() {
		if ( mCurWeaponIndex < 0 )// || mCurWeaponIndex >= ListUnitWeaponClone.Count)
		{
			return null;
		}

		CostumeUnit.sWeaponItem weaponItem = m_costumeUnit.GetWeaponItemOrNull( mCurWeaponIndex );
		if ( weaponItem == null ) {
			return null;
		}

		return weaponItem.AddedUnitWeapon;
	}

	public Unit GetCurrentUnitWeaponCloneOrNull( int index ) {
		if ( mCurWeaponIndex < 0 ) {
			return null;
		}

		if ( index == 0 ) {
			CostumeUnit.sWeaponItem weaponItem = m_costumeUnit.GetWeaponItemOrNull( mCurWeaponIndex );
			if ( weaponItem == null ) {
				return null;
			}

			return weaponItem.AddedUnitWeapon;
		}

		if ( mCurWeaponIndex >= ListUnitWeaponClone.Count ) {
			return null;
		}

		int realIndex = index - 1;

		List<Unit> list = ListUnitWeaponClone[mCurWeaponIndex];
		if ( realIndex < 0 || realIndex >= list.Count ) {
			return null;
		}

		return list[realIndex];
	}

	public bool IsFirstWeapon()
    {
        return mCurWeaponIndex == 0;
    }

    public void HideCurrentUnitWeaponClone()
    {
        for(int i = 0; i < ListUnitWeaponClone.Count; i++)
        {
            for(int j = 0; j < ListUnitWeaponClone[i].Count; j++)
            {
                ListUnitWeaponClone[i][j].Deactivate();
            }
        }
    }

    public bool CheckTimingHoldAttack()
    {
        if (!m_actionSystem.HasAction(eActionCommand.TimingHoldAttack))
        {
            return false;
        }

        AniEvent.sAniInfo aniInfo = m_aniEvent.GetAniInfo(m_aniEvent.curAniType);
        if (aniInfo.timingHoldFrame > 0.0f && m_aniEvent.GetCurrentFrame() >= aniInfo.timingHoldFrame)
        {
            if(m_actionSystem.currentAction)
            {
                m_actionSystem.currentAction.SkipConditionCheck = false;
            }

            CommandAction(eActionCommand.TimingHoldAttack, null);
            return true;
        }

        return false;
    }

	public void AddBuffDebuffDuration( eBuffDebuffType buffType, int count, float add ) {
        mAddBuffDebuffDurationList.Clear();

        List<BattleOption.sBattleOptionData> list = new List<BattleOption.sBattleOptionData>( count );

		// 서포터 배틀옵션
		if ( m_boSupporter != null ) {
			FindBattleOption( ref list, m_boSupporter.ListBattleOptionData, buffType );
		}

		// 캐릭터 배틀옵션
		if ( mBOCharacter != null ) {
			FindBattleOption( ref list, mBOCharacter.ListBattleOptionData, buffType );
		}

		// 무기 배틀옵션
		for ( int i = 0; i < mBOWeapons.Length; i++ ) {
			BOWeapon boWeapon = mBOWeapons[i];
			if ( boWeapon == null ) {
				continue;
			}

			FindBattleOption( ref list, boWeapon.ListBattleOptionData, buffType );
		}

		// 스킬 배틀옵션
		List<ActionSelectSkillBase> listActionSkill = m_actionSystem.GetActionList<ActionSelectSkillBase>();
		for ( int i = 0; i < listActionSkill.Count; i++ ) {
			ActionSelectSkillBase action = listActionSkill[i];
			if ( action == null ) {
				continue;
			}

			CheckSetAddAction( ref list, add, action.GetType() );

			switch ( buffType ) {
				case eBuffDebuffType.Buff: {
					action.AddBuffDuration = add;
				}
				break;

				case eBuffDebuffType.Debuff: {
					action.AddDebuffDuration = add;
				}
				break;

				default: {

				}
				break;
			}

			if ( action.BOCharSkill == null ) {
				continue;
			}

			FindBattleOption( ref list, action.BOCharSkill.ListBattleOptionData, buffType );
		}

		if ( list.Count <= 0 ) {
			return;
		}

        for ( int i = 0; i < count; i++ ) {
			if ( list.Count <= 0 ) {
				break;
			}

			BattleOption.sBattleOptionData data = list[UnityEngine.Random.Range( 0, list.Count )];
			if ( data == null ) {
				return;
			}

			BuffEvent buffEvt = data.evt as BuffEvent;
			if ( buffEvt != null ) {
				buffEvt.AddDuration( add );
			}
			else {
				data.duration += add;
			}

            data.AddDuration = add;

			list.Remove( data );
            mAddBuffDebuffDurationList.Add( data );
        }
	}

	public Unit GetHighestAggroUnit( Unit enemy ) {
		Unit find = this;
		int compare = -1;

		for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
			Player player = World.Instance.ListPlayer[i];
			if( !player.IsActivate() || player.curHp <= 0.0f ) {
				continue;
			}

            float dist = Utility.GetDistanceWithoutY( enemy.transform.position, player.transform.position );
            int n = Mathf.RoundToInt( 20.0f - dist );

            if( n > compare ) {
                compare = n + player.AggroValue;
                find = player;
            }
        }

		for( int i = 0; i < mListMinion.Count; i++ ) {
			PlayerMinion minion = mListMinion[i];
			if( !minion.IsActivate() || minion.AggroValue == -1 ) {
				continue;
			}

            float dist = Utility.GetDistanceWithoutY( enemy.transform.position, minion.transform.position );
            int n = Mathf.RoundToInt( 20.0f - dist );

            if( n > compare ) {
				compare = n + minion.AggroValue;
				find = minion;
			}
		}

		return find;
	}

	public void SetLifeTime( float lifeTime ) {
        LifeTime = lifeTime;
	}

    public void AddGuardianSkillAction( GameTable.CharacterSkillPassive.Param param, List<GameTable.CharacterSkillPassive.Param> listAddParam, int index ) {
        if ( UseGuardian == false ) {
            return;
        }

		CostumeUnit.sWeaponItem weaponItem = costumeUnit.GetWeaponItemOrNull( index );
		if ( weaponItem == null || weaponItem.AddedUnitWeapon == null ) {
			return;
		}

		Type type;
		ActionBase actionBase;
		if ( !string.IsNullOrEmpty( param.ReplacedAction ) ) {
			type = Type.GetType( Utility.AppendString( "ActionGuardian", param.ReplacedAction ) );
			if ( type != null ) {
				actionBase = weaponItem.AddedUnitWeapon.gameObject.GetComponent( type ) as ActionBase;
				if ( actionBase ) {
					weaponItem.AddedUnitWeapon.actionSystem.RemoveAction( actionBase.actionCommand );
					DestroyImmediate( actionBase );
				}
			}
		}

		type = Type.GetType( Utility.AppendString( "ActionGuardian", param.SkillAction ) );
		if ( type == null ) {
			return;
		}

		actionBase = weaponItem.AddedUnitWeapon.gameObject.AddComponent( type ) as ActionBase;
		weaponItem.AddedUnitWeapon.actionSystem.AddAction( actionBase, param.ID, listAddParam );
	}

    public void AddAfterActionType( Type type ) {
		AfterActionTypeList.Add( type );
	}

	public void SetAttackPowerUpRate( float attackPowerUpRate ) {
		mAttackPowerUpRate = attackPowerUpRate;
	}

	public void SetGuardianAttackPowerRate( float guardianAttackPowerRate ) {
		mGuardianAttackPowerRate = guardianAttackPowerRate;
	}

    public void OnAfterAttackPower() {
        if ( Guardian ) {
			Guardian.SetGuardianAttackPower( m_attackPower + m_attackPower * mGuardianAttackPowerRate );
		}

		AddAttackPower( mAttackPowerUpRate );
	}

    protected override bool OnUseCameraSetting( Vector3 distance, Vector2 lookAt, bool lookAtPlayerBack, float smoothTimeRatio, float FOV, 
                                                bool keepCameraSetting ) {
        if( IsHelper ) {
            return false;
		}

        base.OnUseCameraSetting( distance, lookAt, lookAtPlayerBack, smoothTimeRatio, FOV, keepCameraSetting );
        return true;
    }

    protected override void OnEndCameraSetting() {
        if( IsHelper ) {
            return;
        }

        base.OnEndCameraSetting();
    }

	private void CheckSetAddAction( ref List<BattleOption.sBattleOptionData> boDataList, float addDuration, Type actionType ) {
		foreach ( BattleOption.sBattleOptionData boData in boDataList ) {
			if ( boData.funcType != BattleOption.eBOFuncType.SetAddAction ) {
				continue;
			}

			if ( boData.conditionType == BattleOption.eBOConditionType.None ) {
				continue;
			}

			if ( Type.GetType( Utility.AppendString( "Action", boData.conditionType.ToString() ) ) != actionType ) {
				continue;
			}

			if ( ( boData.evt as BuffEvent ) != null ) {
				continue;
			}

			boData.duration -= addDuration;
		}
	}

	private void FindBattleOption( ref List<BattleOption.sBattleOptionData> findBoDataList, List<BattleOption.sBattleOptionData> boDataList, eBuffDebuffType buffType ) {
		findBoDataList.AddRange( boDataList.FindAll( x => FindBattleOption( x, buffType ) ) );

		foreach ( BattleOption.sBattleOptionData boData in boDataList ) {
			if ( FindBattleOptionOnEndCall( boData.dataOnEndCall, buffType ) ) {
				findBoDataList.Add( boData.dataOnEndCall );
			}
		}
	}

	private bool FindBattleOption( BattleOption.sBattleOptionData boData, eBuffDebuffType buffType ) {
		if ( boData == null ) {
			return false;
		}

		return ( boData.buffDebuffType == buffType && boData.timingType != BattleOption.eBOTimingType.GameStart && boData.timingType != BattleOption.eBOTimingType.MaxSP && boData.timingType != BattleOption.eBOTimingType.AddCall && boData.duration > 0.0f );
	}

	private bool FindBattleOptionOnEndCall( BattleOption.sBattleOptionData boData, eBuffDebuffType buffType ) {
		if ( boData == null ) {
			return false;
		}

		return ( boData.buffDebuffType == buffType && boData.timingType != BattleOption.eBOTimingType.GameStart && boData.timingType != BattleOption.eBOTimingType.MaxSP && boData.funcType != BattleOption.eBOFuncType.SpeedUpOnEffect && boData.duration > 0.0f );
	}
}
