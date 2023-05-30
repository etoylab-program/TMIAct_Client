
using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;


public class World : MonoSingleton<World>//, IObserver
{
    public enum eClearMode
    {
        None = 0,
        BossDie,
        AllEnemyDead,
        MonsterCount,
    }

    public enum eStageInfoType
    {
        NoStageData = 0,
        New,
        Clear
    }

    public enum EMissionClearStatus
    {
        FAIL    = 0,    // 못깸
        CLEAR   = 1,    // 깸
        CLEARED = 2,    // 이미 깸
    }

	public enum eSpecialStageType
	{
		None = 0,
		Tower,
		Secret,
	}

    public enum eTestHelperType {
        NONE = 0,
        MANUAL,
        IN_MY_CHAR_LIST,
        LOBBY_CHARS,
	}


    [Header("[Stage Property]")]
    public GameObject[] TutorialBarricades;
    public bool         EnableDamage            = true;
    public Vector3      LightRotate             = Vector3.zero;
    public bool         UseBossSpawnPosition    = true;
    public bool         IsOpenSlopeMap          = false;
    public bool         TurnCameraToTarget      = false;

    [Header("[Test Stage Table Id]")]
    public int TestSelectStageTableId;
    public int TestRaidSeason                   = 1;
    public int TestRaidLevel                    = 1;

    [Header("[Test Player]")]
    public bool             InvinciblePlayer    = false;
    public sTestPlayerData  TestPlayerData;

    [Header("[Test Helper]")]
    public bool                 UseHelper       = false;
    public eTestHelperType      TestHelperType  = eTestHelperType.NONE;
    public sTestPlayerData[]    TestHelperDatas;

	// for Tower
	[HideInInspector] public eSpecialStageType	SpecialStageType		= eSpecialStageType.None;
    [HideInInspector] public sTestPlayerData    TestPlayer2Data;
    [HideInInspector] public sTestPlayerData    TestPlayer3Data;
    [HideInInspector] public int				CardFormationTableId    = 0;

	// for Secret Quest
	[HideInInspector] public int				SecretQuestLevelId			= 0;
	[HideInInspector] public int				SecretQuestStageBOSetId		= 0;

    //== Monster info
	public Vector3      BossSpawnPosition               { get; set; }   = Vector3.zero;
    public Quaternion   BossSpawnRotation               { get; set; }   = Quaternion.identity;
    public bool         SkipControllerPauseInWorldPause { get; set; }	= false; // World에서 전체 정지를 했을 때 컨트롤러 정지는 제외할건지 여부
    public bool         IsEndBossDie                    { get; set; }   = false;
    public int          CheckDieMonsterCount            { get; set; }   = 0;
    //== Stage info
    public GameTable.Stage.Param    StageData           { get; protected set; } = null;
    public eSTAGETYPE               StageType           { get; protected set; } = eSTAGETYPE.STAGE_NONE;
    public eClearMode               ClearMode           { get; protected set; } = eClearMode.None;
    public eStageInfoType           StageInfoType       { get; protected set; } = eStageInfoType.NoStageData;
    public int                      ClearConditionValue { get; protected set; } = 0;
    public EMissionClearStatus      Mission01           { get; protected set; } = EMissionClearStatus.FAIL;
    public EMissionClearStatus      Mission02           { get; protected set; } = EMissionClearStatus.FAIL;
    public EMissionClearStatus      Mission03           { get; protected set; } = EMissionClearStatus.FAIL;
    public int                      DieMonsterCount     { get; protected set; } = 0;
    public bool                     IsTimeLimit         { get; protected set; } = false;
    //== Mgr
    public ProjectileMgr            ProjectileMgr       { get; protected set; } = null;
    public BaseEnemyMgr             EnemyMgr            { get; protected set; } = null;
    public TestScene                TestScene           { get; protected set; } = null;
    //== UI, Camera
    public UIGamePlayPanel          UIPlay              { get; protected set; } = null;
    public UIArenaPlayPanel         UIPVP               { get; protected set; } = null;
    public InGameCamera             InGameCamera        { get; protected set; } = null;
    //== Player, Boss
    public Player                   Player              { get; protected set; } = null;
    public List<Player>             ListPlayer          { get; protected set; } = new List<Player>();
    public Enemy                    Boss                { get; protected set; } = null;
    //== Game info
    public float                    GameSpeed           { get; protected set; } = 1.0f;
    public bool                     IsPause             { get; protected set; } = false;
    public bool                     IsEndGame           { get; protected set; } = false;
    public bool                     ProcessingEnd       { get; protected set; } = false;
    public bool                     IsBossDead          { get; protected set; } = false;
    public List<string>             ListAniString       { get; protected set; } = new List<string>();
    public ObscuredFloat            ProgressTime        { get; protected set; } = 0.0f;
    public ObscuredFloat            GameTime            { get; protected set; } = 0.0f;
    //== etc.
    public List<Unit>               ListPlayerSummons   { get; protected set; } = new List<Unit>();
    //==

    protected AudioClip								mBgm							= null;
    protected AudioClip								mAmbienceSnd					= null;
    protected Dictionary<string, Queue<Director>>	mDicDirector					= new Dictionary<string, Queue<Director>>();
	protected string								mCurScenarioDirectorKey			= "";
	protected Action								mCurScenarioDirectorCallback	= null;
	protected bool									mbLastStorySequence				= false;
    protected string								mCurrentPlayDirectorKey			= "";
    protected float									mBossDieDuration				= 0.0f;
    protected int									mCheckConditionValue			= 0;
    protected float									mShowGamePlayUILength			= 0.0f;
    protected Coroutine								mCrTimer						= null;
    protected List<BattleOption>					mListAllBO						= new List<BattleOption>();
    protected bool									mShowResult						= false;
	protected WaitForFixedUpdate					mWaitForFixedUpdate				= new WaitForFixedUpdate();
    protected List<Unit>                            mTempTargetList                 = new List<Unit>();


	public void GetComponents()
    {
#if UNITY_EDITOR
        if (TestScene == null)
        {
            TestScene = FindObjectOfType<TestScene>();
        }
#else
        InvinciblePlayer = false;
        EnableDamage = true;
#endif

        InGameCamera = FindObjectOfType<InGameCamera>();
        UIPlay = GameUIManager.Instance.GetActiveUI<UIGamePlayPanel>("GamePlayPanel");

        ProjectileMgr = new ProjectileMgr();

        GetAnimationStrings();
    }

    public virtual void Init(int tableId, eSTAGETYPE stageType = eSTAGETYPE.STAGE_NONE)
    {
        ProgressTime.RandomizeCryptoKey();
        GameTime.RandomizeCryptoKey();

        GetComponents();
        ShowPlayUI(true);

        IsPause = false;
        IsEndGame = false;
        ProcessingEnd = false;
        IsBossDead = false;
		ListPlayer.Clear();

        if (FSaveData.Instance.Graphic <= 1)
        {
            InGameCamera.EnablePostEffects(false);
        }
        else
        {
            InGameCamera.EnablePostEffects(true);
        }

        if (TestScene)
        {
            EnemyMgr = TestScene;
            StageType = eSTAGETYPE.STAGE_TEST;
        }
        else
        {
            //GameUIManager.Instance.HideUI("GameOffPopup", false);
            IsTimeLimit = false;

            if (tableId > 0 && stageType != eSTAGETYPE.STAGE_TOWER)
            {
                StageData = GameInfo.Instance.GetStageData(tableId);
                StageType = (eSTAGETYPE)StageData.StageType;
                ClearMode = (eClearMode)StageData.ClearCondition;

                if (GameInfo.Instance.StageClearList.Find(x => x.TableID == StageData.ID) == null)
                {
                    StageInfoType = eStageInfoType.New;
                }
                else
                {
                    StageInfoType = eStageInfoType.Clear;
                }

                if(ClearMode != eClearMode.MonsterCount && StageData.ConditionValue > 0)
                {
                    IsTimeLimit = true;
                }
            }
            else
            {
                StageType = stageType;
            }

            if (!GameInfo.Instance.ContinueStage)
            {
                if (StageData != null && IsTimeLimit)
                {
                    GameTime = StageData.ConditionValue;
                }
                else
                {
                    GameTime = 0.0f;
                }

                ProgressTime = 0.0f;
            }
            else
            {
                GameTime = PlayerPrefs.GetFloat("SAVE_STAGE_TIME_");
                ProgressTime = PlayerPrefs.GetFloat("SAVE_STAGE_PROGRESS_TIME_");
            }
        }

        DieMonsterCount = 0;
        CheckDieMonsterCount = 0;
    }

    public void OnPlayerMissionStart() {
        Player.OnMissionStart();

        if( StageType != eSTAGETYPE.STAGE_TOWER ) {
            for( int i = 0; i < ListPlayer.Count; i++ ) {
                if( ListPlayer[i].IsHelper ) {
                    ListPlayer[i].OnMissionStart();
                    ListPlayer[i].StartBT();
                }
            }
        }
    }

    public void ShowHelperMesh( bool showOnPlayerPos, Vector3 addPos ) {
        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        for( int i = 0; i < ListPlayer.Count; i++ ) {
            if( !ListPlayer[i].IsActivate() || ListPlayer[i].curHp <= 0.0f ) {
                continue;
			}

            if( !ListPlayer[i].IsHelper ) {
                pos = ListPlayer[i].transform.position + addPos;
                rot = ListPlayer[i].transform.rotation;

                continue;
            }

            if( showOnPlayerPos ) {
                ListPlayer[i].SetInitialPosition( pos, rot );
            }

            ListPlayer[i].ShowMesh( true );
            ListPlayer[i].ResetBT();
        }
    }

    public void HideHelperMesh() {
        for( int i = 0; i < ListPlayer.Count; i++ ) {
            if( !ListPlayer[i].IsHelper ) {
                continue;
			}

            ListPlayer[i].StopBT();
            ListPlayer[i].ShowMesh( false );
		}
	}

    public void ActiveEnemyMgrOnTutorial()
    {
        for (int i = 0; i < TutorialBarricades.Length; i++)
        {
            TutorialBarricades[i].SetActive(false);
        }

        Pause(false);
        EnemyMgr.Active();
    }

    public bool HasDirector(string key)
    {
        return mDicDirector.ContainsKey(key);
    }

    public Director GetDirector(string key)
    {
		if (mDicDirector.ContainsKey(key) == false || mDicDirector[key].Count <= 0)
		{
			return null;
		}

		return mDicDirector[key].Peek();
    }

    public float GetDirectorDuration(string key)
    {
        Director drt = GetDirector(key);
        if (drt == null)
        {
            return 0.0f;
        }

        return drt.GetDuration();
    }

	public bool PlayDirector( string key, Action CallbackOnEnd2 = null, bool startPosIsZero = false ) {
		if( mDicDirector.ContainsKey( key ) == false ) {
			return false;
		}

		mCurrentPlayDirectorKey = key;
		bool lastDirector = false;

		Director curDirector = null;
		if( mDicDirector[key].Count > 1 ) {
			curDirector = mDicDirector[key].Dequeue();
		}
		else {
			curDirector = mDicDirector[key].Peek();
			lastDirector = true;
		}

		if( StageInfoType == eStageInfoType.New ) {
			if( key == "Start" ) {
				Firebase.Analytics.FirebaseAnalytics.LogEvent( "WatchStartScenarioCutScene", "StageId", StageData.ID );
			}
			else if( key == "BossAppear" ) {
				Firebase.Analytics.FirebaseAnalytics.LogEvent( "WatchBossAppearCutScene", "StageId", StageData.ID );
			}
			else if( key == "BossDie" ) {
				Firebase.Analytics.FirebaseAnalytics.LogEvent( "WatchBossDieCutScene", "StageId", StageData.ID );
			}

			curDirector.SetSkipBtnCallback( OnBtnSkipInDirector );
		}

		if( lastDirector ) {
			if( CallbackOnEnd2 != null ) {
				curDirector.SetCallbackOnEnd2( CallbackOnEnd2 );
			}
		}
		else {
			mCurrentPlayDirectorKey = key;
			mCurScenarioDirectorCallback = CallbackOnEnd2;

			curDirector.SetCallbackOnEnd2( PlayNextScenarioDirector );
		}

		curDirector.Play( startPosIsZero );
		return true;
	}

	public void SkipAllCutScene(Director curDirector)
	{
		if (mDicDirector.ContainsKey(mCurrentPlayDirectorKey))
		{
            mDicDirector[mCurrentPlayDirectorKey].Clear();
		}

        if (mCurScenarioDirectorCallback != null)
		{
			curDirector.SetCallbackOnEnd2(mCurScenarioDirectorCallback);
            mCurScenarioDirectorCallback = null;
        }

		curDirector.ForceQuit(true);
	}

	public void PlayNextScenarioDirector()
	{
		if(!mDicDirector.ContainsKey(mCurrentPlayDirectorKey))
		{
			return;
		}

		Director curDirector = mDicDirector[mCurrentPlayDirectorKey].Dequeue();

		if (mDicDirector[mCurrentPlayDirectorKey].Count <= 0)
		{
			if (mCurScenarioDirectorCallback != null)
			{
				curDirector.SetCallbackOnEnd2(mCurScenarioDirectorCallback);
                mCurScenarioDirectorCallback = null;
			}
		}
		else
		{
			curDirector.SetCallbackOnEnd2(PlayNextScenarioDirector);
		}

		curDirector.Play();
	}

    public void SetGameSpeed(float gameSpeed)
    {
        GameSpeed = gameSpeed;
        Time.timeScale = gameSpeed;
    }

    public void SetDieMonsterCount(int count)
    {
        DieMonsterCount = count;
    }

	public bool IsAllPlayersDead() {
		Player find = null;

        if( StageData != null && ( StageType == eSTAGETYPE.STAGE_RAID || StageData.PlayerMode == 1 ) ) {
            find = ListPlayer.Find( x => ( x.curHp > 0.0f ) && ( x.IsActivate() ) );
        }
        else {
            find = ListPlayer.Find( x => x.curHp > 0.0f );
        }

		return ( find == null );
	}

	public void OnEvent( BaseEvent evt ) {
		if( IsEndGame || ProcessingEnd ) {
			return;
		}

		switch( evt.eventType ) {
			// 공격 이벤트
			case eEventType.EVENT_BATTLE_ON_MELEE_ATTACK:
				CheckMeleeAttack( evt as AttackEvent );
				break;

			case eEventType.EVENT_BATTLE_ON_PROJECTILE_ATTACK:
				CheckProjectileHit( evt as AttackEvent );
				break;

			case eEventType.EVENT_BATTLE_ON_DIRECT_HIT:
				CheckDirectHit( evt as AttackEvent );
				break;

			// 배틀 옵션 이벤트
			//case eEventType.EVENT_BATTLE_DAMAGE_ONCE:
				//DamageOnce( evt as AttackEvent );
				//break;

			// 게임 이벤트
			case eEventType.EVENT_GAME_ALL_ENEMY_DEAD:
				if( ClearMode == eClearMode.AllEnemyDead && EnemyMgr.IsAllMonsterDie() ) {
					ProcessingEnd = true;
					EndGame( evt.eventType, evt.sender );
				}
				break;

			case eEventType.EVENT_GAME_PLAYER_DEAD:
			case eEventType.EVENT_GAME_OTHER_OBJECT_DEAD:
			case eEventType.EVENT_GAME_END_TIMER:
			case eEventType.EVENT_RACE_FAILED:
			case eEventType.EVENT_RACE_SUCCESS:
				ProcessingEnd = true;
				EndGame( evt.eventType, evt.sender );
				break;

			case eEventType.EVENT_GAME_ENEMY_BOSS_APPEAR:
				Boss = evt.sender as Enemy;
				UIPlay.UpdateTargetHp( Boss, Boss.tableName, Boss.curHp, Boss.maxHp, Boss.data.HPShow );

				if( Boss.maxShield > 0.0f ) {
					UIPlay.UpdateTargetShield( Boss.curShield, Boss.maxShield );
				}
				break;

			case eEventType.EVENT_GAME_ENEMY_BOSS_DIE:
				IsBossDead = true;

				SoundManager.Instance.StopSnd( SoundManager.eSoundType.Monster );

				EndGameEvent endGameEvt = evt as EndGameEvent;
				if( endGameEvt != null ) {
					mBossDieDuration = endGameEvt.duration <= 0.0f ? 0.0f : endGameEvt.duration + ( 1.0f / Application.targetFrameRate );
				}
				else {
					mBossDieDuration = 0.0f;
				}

				if( ClearMode == eClearMode.BossDie ) {
					EndGame( evt.eventType, evt.sender );
				}
				else if( ClearMode == eClearMode.MonsterCount ) {
					EventEnemyDead();
				}
				break;

			case eEventType.EVENT_GAME_ENEMY_DEAD:
				EventEnemyDead();
				break;

			case eEventType.EVENT_GAME_PASS_PORTAL:
				--mCheckConditionValue;

				if( mCheckConditionValue <= 0 ) {
					ProcessingEnd = true;
					EndGame( evt.eventType, evt.sender );
				}
				break;

			// PlayerMinion이 소환됐을 때
			case eEventType.EVENT_SUMMON_PLAYER_MINION:
				AddPlayerSummons( evt.sender );
				AllUnitRetarget();
				break;

			// PlayerMinion 소환이 끝났을 때
			case eEventType.EVENT_SUMMON_OFF_PLAYER_MINION:
				RemovePlayerSummons( evt.sender );
				AllUnitRetarget();
				break;

            case eEventType.EVENT_GAME_REMOVE_HIT_TARGET: {
                if ( ListPlayer.Count <= 0 ) {
                    this.Player.RemoveHitTarget( evt.sender );
                }
                else {
                    for ( int i = 0; i < ListPlayer.Count; i++ ) {
                        ListPlayer[i].RemoveHitTarget( evt.sender );
					}
				}

                EnemyMgr.RemoveHitTarget( evt.sender );
            }
            break;
        }
	}

	private void EventEnemyDead() {
        ++DieMonsterCount;

        for( int i = 0; i < ListPlayer.Count; i++ ) {
            ++ListPlayer[i].CheckMonsterDieCountForSummon;
            ListPlayer[i].ExecuteBattleOption( BattleOption.eBOTimingType.OnEnemyDie, 0, null );
        }

        if( ClearMode == eClearMode.MonsterCount ) {
            PlayerPrefs.SetInt( "SAVE_STAGE_DIE_MONSTER_COUNT_", DieMonsterCount );
            UIPlay.UpdateMonsterCount( DieMonsterCount, ClearConditionValue );

            if( DieMonsterCount >= ClearConditionValue ) {
                EndGame( eEventType.EVENT_GAME_ALL_ENEMY_DEAD, null );
            }
        }
    }

	public virtual void Pause( bool pause, bool withTimeScale = true, Unit exceptUnit = null, bool withoutUIPlayWidgetAlpha = false ) {
		IsPause = pause;

		if( pause == true ) {
			StopCoroutine( "ResumeAll" );

			if( withTimeScale ) {
				Time.timeScale = 0.0f;
			}
			else {
				Time.timeScale = GameSpeed;
			}

			if( Player ) // 튜토리얼땐 플레이어 생성을 나중에 함
			{
                for( int i = 0; i < ListPlayer.Count; i++ ) {
                    ActionChargeAttack chargeAttack = ListPlayer[i].actionSystem.GetCurrentAction<ActionChargeAttack>();
                    if( chargeAttack ) {
                        chargeAttack.OnUpdating( null );
                    }
                    else {
                        ActionMurasakiComboAttack murasakiComboAttack = ListPlayer[i].actionSystem.GetCurrentAction<ActionMurasakiComboAttack>();
                        if( murasakiComboAttack ) {
                            murasakiComboAttack.EndCharging();
                        }
                    }

                    ListPlayer[i].Input.Pause( true );
                }
			}

			if( EnemyMgr ) {
				EnemyMgr.PauseAll( exceptUnit );
			}
		}
		else {
			Time.timeScale = GameSpeed;

			if( !IsEndGame && !SkipControllerPauseInWorldPause ) {
				if( UIPlay && !withoutUIPlayWidgetAlpha ) {
					UIPlay.SetWidgetsAlpha( 1.0f );
				}

				if( Player ) // 튜토리얼땐 플레이어 생성을 나중에 함
				{
                    for( int i = 0; i < ListPlayer.Count; i++ ) {
                        ListPlayer[i].Input.Pause( false );
                    }
				}

				if( EnemyMgr ) {
					EnemyMgr.ResumeAll();
				}
			}

			SkipControllerPauseInWorldPause = false;
		}
	}

	public void ShowTutorialHUD(int step, float hideDuration = 0.0f)
    {
        int tutorialState = GameInfo.Instance.UserData.GetTutorialState();
        GameClientTable.Tutorial.Param param = GameInfo.Instance.GameClientTable.FindTutorial(x => x.StateID == tutorialState && x.Step == step);

        string[] sprs = null;
        if (!string.IsNullOrEmpty(param.HUDSprite))
        {
			sprs = Utility.Split(param.HUDSprite, ','); //param.HUDSprite.Split(',');
		}

        UIPlay.m_screenEffect.ShowHUD(FLocalizeString.Instance.GetText(param.Desc), sprs, param.HUDUseSupporterTex == 1 ? true : false);

        if (hideDuration > 0.0f)
        {
            Invoke("HideTutorialHUD", hideDuration);
        }
    }

    public void HideTutorialHUD()
    {
        UIPlay.m_screenEffect.HideHUD();
    }

    public void OnCloseHelpPopup()
    {
        if (UIValue.Instance.ContainsKey(UIValue.EParamType.ShowHelpPopupInPausePopup))
        {
            bool b = (bool)UIValue.Instance.GetValue(UIValue.EParamType.ShowHelpPopupInPausePopup, true);
            if (!b)
            {
                ShowTutorialHUD(1);
                Pause(false);

                AppMgr.Instance.CustomInput.ShowCursor(false);
            }
        }
        else
        {
            AppMgr.Instance.CustomInput.ShowCursor(false);
            Pause(false);
        }
    }

    public void ChangeBoss(Enemy enemy)
    {
        Boss = enemy;

        UIPlay.UpdateTargetHp(Boss, Boss.tableName, Boss.curHp, Boss.maxHp, Boss.data.HPShow);
        if (Boss.maxShield > 0.0f)
        {
            UIPlay.UpdateTargetShield(Boss.curShield, Boss.maxShield);
        }

        /*
        if (Boss.data.FixedCamera)
        {
            InGameCamera.SetDefaultModeTarget(Boss, true);
        }
        */
    }

	public void PlayBgm() {
		if( mBgm == null ) {
			return;
		}
	
        SoundManager.Instance.PlayBgm( mBgm, FSaveData.Instance.GetMasterVolume(), true );
	}

	public void SetSlowTime(float timeScale, float duration)
    {
        Time.timeScale = timeScale;
        Invoke("RestoreTimeScale", duration);
    }

    public virtual void ShowGamePlayUI(int aniIndex, Unit exceptUnit = null)
    {
        Pause(true, false, exceptUnit);
        mShowGamePlayUILength = UIPlay.ShowUI(aniIndex);

        StopCoroutine("ResumeAll");
        StartCoroutine("ResumeAll", mShowGamePlayUILength);
    }

    public EMissionClearStatus CheckStageMission(int index, int missionId)
    {
        GameClientTable.StageMission.Param param = GameInfo.Instance.GameClientTable.FindStageMission(missionId);
        if (param == null)
        {
            return EMissionClearStatus.FAIL;
        }

        StageClearData stageClearData = GameInfo.Instance.StageClearList.Find(x => x.TableID == StageData.ID);
        if (stageClearData != null && stageClearData.Mission[index] > 0)
        {
            return EMissionClearStatus.CLEARED;
        }

        float compareValue = 0.0f;
        eStageClearCondition condition = Utility.GetStageClearConditionByString(param.MissionCondition);
        switch (condition)
        {
            case eStageClearCondition.STAGE_CLEAR:
                return EMissionClearStatus.CLEAR;

            case eStageClearCondition.MAX_COMBO_COUNT:
                compareValue = Player.maxCombo;
                break;

            case eStageClearCondition.REMAIN_HP_PERCENTAGE:
                compareValue = (Player.curHp / Player.maxHp) * 100.0f;
                break;

            case eStageClearCondition.CLEAR_TIME:
                compareValue = ProgressTime;
                break;

            case eStageClearCondition.HIT_COUNT:
                compareValue = Player.hitCount;
                break;

            case eStageClearCondition.NPC_REMAIN_HP_PERCENTAGE:
                if (EnemyMgr != null && EnemyMgr.otherObject != null)
                {
                    compareValue = (EnemyMgr.otherObject.curHp / EnemyMgr.otherObject.maxHp) * 100.0f;
                }
                break;

            case eStageClearCondition.NPC_HIT_COUNT:
                if (EnemyMgr != null && EnemyMgr.otherObject != null)
                {
                    compareValue = EnemyMgr.otherObject.HitCount;
                }
                break;
        }

        eCompareType compareType = Utility.GetCompareTypeByString(param.ConditionCompareType);
        if (CheckValueType(compareType, compareValue, param.ConditionValue))
        {
            return EMissionClearStatus.CLEAR;
        }

        return EMissionClearStatus.FAIL;
    }

    public virtual void ShowResultUI()
    {
        AppMgr.Instance.CustomInput.ShowCursor(true);
    }

    public virtual void ToLobby()
    {
		Director.IsPlaying = false;

        //AppMgr.Instance.CustomInput.InverseXAxis = false;
        //AppMgr.Instance.CustomInput.InverseYAxis = false;

        GameUIManager.Instance.ShowUI("GameOffPopup", false);
        GameUIManager.Instance.HideUI("GameFailPanel", false);
        GameUIManager.Instance.HideUI("GamePlayPanel", false);

        StartCoroutine("LoadLobby");
    }

	public virtual void AllUnitRetarget() {
		if ( !EnemyMgr ) {
			return;
		}
			
		EnemyMgr.AllEnemyRetarget();
	}

    public void AddBO(BattleOption bo)
    {
        BattleOption find = mListAllBO.Find(x => x == bo);
        if(find != null)
        {
            return;
        }

        mListAllBO.Add(bo);
    }

    public void AddPlayerSummons(Unit unit)
    {
        Unit find = ListPlayerSummons.Find(x => x == unit);
        if(find)
        {
            return;
        }

        ListPlayerSummons.Add(unit);
    }

    public void RemovePlayerSummons(Unit unit)
    {
        Unit find = ListPlayerSummons.Find(x => x == unit);
        if (find == null)
        {
            return;
        }

        if (find.unitBuffStats != null)
        {
            find.unitBuffStats.RemoveAllBuffStat();
        }

        ListPlayerSummons.Remove(find);
    }

    /*
	public void RemoveHitTarget( Unit target ) {
		Unit find = ListHitTarget.Find(x => x == target);
		if( find == null ) {
			return;
		}

		ListHitTarget.Remove( target );

		if( ListPlayer.Count > 0 ) {
            for( int i = 0; i < ListPlayer.Count; i++ ) {
                ListPlayer[i].RemoveTarget( target );
            }
		}
	}
    */

    public void ChangePlayableCharacter( Player newPlayableChar ) {
        int changeIndex = 0;
        for( int i = 0; i < ListPlayer.Count; i++ ) {
            if( ListPlayer[i] == newPlayableChar ) {
                changeIndex = i;
            }
        }

        if( changeIndex == 0 ) {
            return;
        }

        bool isAutoPlay = false;

        // 기존 플레이어블 캐릭터
        Player.AddAIController( Player.charData.TableData.AI );
        Player.ClearEnemyNavigators();
        Player.ClearOtherObjectNavigator();
        Player.HideBattleAreaNavigator();
        Player.IsHelper = true;

        isAutoPlay = Player.AutoPlay;
        Player.AutoPlay = false;

        // 바꾼 플레이어블 캐릭터
        Player beforePlayer = Player;

        Player = ListPlayer[changeIndex];
        Player.IsHelper = false;

        Player.StopBT();
        Player.CopyEnemyNavigators( beforePlayer );
        Player.CopyNextBattleAreaNavigator( beforePlayer );
        Player.CopyOtherObjectNavigator( beforePlayer );
        Player.ReInitPlayerUI( Player.curHp, Player.curSp );

        if( !isAutoPlay ) {
            Player.RemoveAIController();
        }

        Player.AutoPlay = isAutoPlay;
        Player.Input.SetInputUI();

        UIPlay.m_screenEffect.ClearSkillNameQueue();
        UIPlay.ShowUSkillCoolTime( Player, false );

        if ( beforePlayer.curHp <= 0.0f ) {
            UIPlay.ShowUI(0);
        }
        else {
			UIPlay.SetUIActive( true, false );
			UIPlay.OnEnable();
		}

        InGameCamera.SetPlayer( Player );

        ListPlayer[0] = Player;
        ListPlayer[changeIndex] = beforePlayer;

        ListPlayer[changeIndex].SetAutoMove( Player.IsAutoMove );
        ListPlayer[changeIndex].OnAfterChangePlayableChar();

        if( !Player.AutoPlay ) {
            Player.StopAutoMove();
        }
        else {
            Player.SetAutoMove( Player.IsAutoMove );
        }

        Player.OnAfterChangePlayableChar();

        Player.MainCollider.Enable( false );
        Invoke( "EnablePlayerMainCollider", 1.0f / Application.targetFrameRate );
    }

    public Player GetPlayerByCuidOrNull( long cuid ) {
        return ListPlayer.Find( x => x.charData.CUID == cuid );
	}

    public virtual void OnEndResult() {
    }

    public void ForceQuit() {
        FSaveData.Instance.RemoveStageData();
        Pause( true, false );

        GameUIManager.Instance.HideUI( "GameFailPanel", false );
        GameUIManager.Instance.HideUI( "GamePlayPanel", false );

        UIValue.Instance.SetValue( UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.StageToLobby );
        UIValue.Instance.SetValue( UIValue.EParamType.LoadingStage, -1 );
        GameUIManager.Instance.ShowUI( "LoadingPopup", false );
        AppMgr.Instance.LoadScene( AppMgr.eSceneType.Lobby, "Lobby" );
    }

#if UNITY_EDITOR
    public void SetPlayerInTestScene(Player player)
    {
        Player = player;
    }
#endif

	protected void PostInit() {
		if( EnemyMgr ) {
			EnemyMgr.Init();
		}

		DropItemMgr.Instance.LoadDropItems();
		HUDResMgr.Instance.LoadHUD();
		EffectManager.Instance.LoadEffs();

		if( !GameInfo.Instance.CharSelete && ListPlayer.Count > 0 ) {
			for( int i = 0; i < ListPlayer.Count; i++ ) {
				ListPlayer[i].InitAfterEnemyMgrInit();
			}
		}

		if( ClearMode == eClearMode.MonsterCount ) {
			UIPlay.ActiveMonsetCount( true, ClearConditionValue );
		}

		//Bike Scene에는 존재하지 않아서 null체크
		if( TutorialBarricades != null && TutorialBarricades.Length > 0 ) {
			for( int i = 0; i < TutorialBarricades.Length; i++ ) {
				TutorialBarricades[i].SetActive( false );
			}
		}

		if( GameSupport.IsInGameTutorial() ) {
			eTutorialState tutorialState = (eTutorialState)GameInfo.Instance.UserData.GetTutorialState();
			if( tutorialState == eTutorialState.TUTORIAL_STATE_Stage2Clear ) {
				TutorialBarricades[0].SetActive( true );
			}
			else if( tutorialState == eTutorialState.TUTORIAL_STATE_Stage3Clear ) {
				TutorialBarricades[1].SetActive( true );

				// 아무것도 안하는 적 하나 생성
				GameClientTable.Monster.Param param = GameInfo.Instance.GameClientTable.FindMonster(3);
				Enemy enemy = EnemyMgr.CreateEnemy(param);
				enemy.transform.position = Player.transform.position + ( ( transform.right + transform.forward ) * 3.5f );
				enemy.LookAtTarget( EnemyMgr.m_startPoint.transform.position );
				enemy.Activate();
			}
		}

		Debug.Log( "End World Init" );
	}

	protected AudioClip LoadAudioClip(string path)
    {
        UnityEngine.Object obj = ResourceMgr.Instance.LoadFromAssetBundle("sound", "Sound/" + path);
        if (obj == null)
        {
            Debug.LogError(path + " 오디오 파일을 읽어올 수 없습니다.");
            return null;
        }

        return obj as AudioClip;
    }

    protected void ChangeToFinishBGM(bool win)
    {
        if (win == true)
        {
            SoundManager.Instance.StopBgm();
            //SoundManager.Instance.PlayBgm(2000, SoundManager.Instance.GetVolume(SoundManager.eSoundType.DirectorBGM, 1f), false);
        }
        else
        {
            SoundManager.Instance.PlayBgm(2001, SoundManager.Instance.GetVolume(SoundManager.eSoundType.DirectorBGM, 1f), true);
        }

        SoundManager.Instance.StopAmbienceSnd();
    }

	protected void AddDirector( Unit owner, string key, Director director ) {
		if( director == null ) {
			return;
		}

		director.Init( owner );

		if( mDicDirector.ContainsKey( key ) ) {
			mDicDirector[key].Enqueue( director );
		}
		else {
			mDicDirector.Add( key, new Queue<Director>() );
			mDicDirector[key].Enqueue( director );
		}
	}

	protected void StartTimer()
    {
        if (mCrTimer != null)
        {
            StopCoroutine(mCrTimer);
            mCrTimer = null;
        }

        mCrTimer = StartCoroutine(UpdateTimer());
    }

	protected virtual void EndIntro() {
        GameUIManager.Instance.HideUI( "GameOffPopup", false );
        PlayBgm();

		if( mAmbienceSnd ) {
			SoundManager.Instance.PlayAmbienceSnd( mAmbienceSnd, 0.2f, true );
		}
	}

	protected virtual void SendStageFailed()
    {
    }

    protected IEnumerator LoadLobby()
    {
        yield return new WaitForSeconds(0.1f);
        Player.DeactivateDirector("Die");

        UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.StageToLobby);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, -1);
        GameUIManager.Instance.ShowUI("LoadingPopup", false);

        AppMgr.Instance.LoadScene(AppMgr.eSceneType.Lobby, "Lobby");
    }

    protected virtual void Update()
    {
		if (AppMgr.Instance.SceneType != AppMgr.eSceneType.Stage)
		{
			return;
		}

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P) == true)
        {
            UIPlay.OnBtnPause();
        }
        else if (Input.GetKeyDown(KeyCode.T) == true)
        {
            World.Instance.Pause( true, false );
            MessagePopup.OK( eTEXTID.TITLE_NOTICE, "레이드가 종료 되었습니다. 로비로 이동합니다.", World.Instance.ForceQuit );
            //EndGameEvent evt = new EndGameEvent();
            //evt.Set(eEventType.EVENT_GAME_ENEMY_BOSS_DIE_AFTER_DIRECTOR, 0.0f);
            //EventMgr.Instance.SendEvent(evt);
        }
#endif
    }

    /*
    protected virtual void FixedUpdate()
    {
        for(int i = 0; i < mListAllBO.Count; i++)
        {
            mListAllBO[i].UpdateCheckCoolTime();
        }
    }
    */

    private void GetAnimationStrings()
    {
		string[] strs = Enum.GetNames(typeof(eAnimation));
		ListAniString.AddRange(strs);
		/*
		foreach (eAnimation ani in Enum.GetValues(typeof(eAnimation)))
        {
			ListAniString.Add(ani.ToString());
		}
		*/
    }

    private IEnumerator UpdateTimer()
    {
        bool includeMilliseconds = false;
        if (StageData != null && (StageType == eSTAGETYPE.STAGE_TIMEATTACK || StageData.ConditionValue > 0))
        {
            includeMilliseconds = true;
        }

        //GameTime.RandomizeCryptoKey();
        //ProgressTime.RandomizeCryptoKey();

        TimeSpan ts = TimeSpan.FromSeconds(GameTime);
        UIPlay.UpdateTime(ts, includeMilliseconds, IsTimeLimit);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (IsEndGame == false)
        {
            if (!IsPause)// && (Player.Input && !Player.Input.isPause))
            {
                ProgressTime += Time.fixedDeltaTime * Time.timeScale;

                //if (StageData == null || ClearMode == eClearMode.MonsterCount || StageData.ConditionValue <= 0)
                if(ClearMode == eClearMode.MonsterCount || ClearConditionValue <= 0)
                {
                    GameTime += Time.fixedDeltaTime * Time.timeScale;
                }
                else
                {
                    GameTime = Mathf.Max(0.0f, GameTime - (Time.fixedDeltaTime * Time.timeScale));

                    if (GameTime <= 0.0f)
                    {
                        EventMgr.Instance.SendEvent(eEventSubject.World, eEventType.EVENT_GAME_END_TIMER, null);
                    }
                }

                ts = TimeSpan.FromSeconds(GameTime);
                UIPlay.UpdateTime(ts, includeMilliseconds, IsTimeLimit);
            }

            //GameTime.RandomizeCryptoKey();
            //ProgressTime.RandomizeCryptoKey();

            yield return mWaitForFixedUpdate;
        }
    }

    private void OnBtnSkipInDirector()
    {
        if (mCurrentPlayDirectorKey == "Start")
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("SkipStartScenarioCutScene", "StageId", StageData.ID);
        }
        else if (mCurrentPlayDirectorKey == "BossAppear")
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("SkipBossAppearCutScene", "StageId", StageData.ID);
        }
        else if (mCurrentPlayDirectorKey == "BossDie")
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("SkipBossDieCutScene", "StageId", StageData.ID);
        }
    }

    private IEnumerator ResumeAll(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (IsPause)
        {
            Pause(false);
        }
    }

    private void RestoreTimeScale()
    {
        Time.timeScale = GameSpeed;
    }

    private void EnableMainCamera()
    {
		//InGameCamera.MainCamera.enabled = true;
		GameUIManager.Instance.HideUI( "GameOffPopup", false );
	}

    private void ShowPlayUI(bool show)
    {
        if (UIPlay == null || UIPlay.gameObject == null)
        {
            Debug.LogError("GamePanel이 없습니다.");
            return;
        }

        UIPlay.gameObject.SetActive(show);
    }

	private void CheckDirectHit( AttackEvent atkEvent ) {
        if ( Director.IsPlaying || atkEvent.listTargetCollider == null || atkEvent.listTargetCollider.Count <= 0 ) {
            return;
        }

		bool successHit = false;
		bool checkCritical = false;
		bool isCritical = false;

        mTempTargetList.Clear();

		for ( int i = 0; i < atkEvent.listTargetCollider.Count; i++ ) {
			Unit target = atkEvent.listTargetCollider[i].Owner;
            if ( target == null || target.IsActivate() == false || !target.IsShowMesh ) {
                continue;
            }

            if ( target.actionSystem.IsCurrentQTEAction() || target.actionSystem.IsCurrentUSkillAction() ) {
                continue;
            }

            if ( target.isDying == true ) {
                continue;
            }

			Unit.eSuperArmor targetSuperArmor = target.CurrentSuperArmor;
			eHitState enemyHitState = eHitState.Fail;

			bool onlyEffect = false;
			if ( StageType == eSTAGETYPE.STAGE_PVP ) {
				onlyEffect = targetSuperArmor == Unit.eSuperArmor.Invincible || target.TemporaryInvincible;
			}
			else {
				onlyEffect = !atkEvent.isUltimateSkill && ( targetSuperArmor == Unit.eSuperArmor.Invincible || target.TemporaryInvincible );
			}

			if ( onlyEffect ) {
				enemyHitState = eHitState.OnlyEffect;
			}
			else {
				if ( !atkEvent.isUltimateSkill ) {
					if ( ( !Utility.IsSpecialAttack( atkEvent.aniEvent, null ) && targetSuperArmor == Unit.eSuperArmor.Lv1 ) || atkEvent.onlyDamageHit ) {
						enemyHitState = eHitState.OnlyDamage;
					}
					else if ( targetSuperArmor >= Unit.eSuperArmor.Lv2 ) {
						enemyHitState = eHitState.OnlyDamage;
					}
					else if ( target.reserveStun == false ) {
						enemyHitState = eHitState.Success;
						successHit = true;
					}
				}
				else if ( target.reserveStun == false ) {
					enemyHitState = eHitState.Success;
					successHit = true;
				}
			}

			if ( enemyHitState == eHitState.Success || enemyHitState == eHitState.OnlyDamage || enemyHitState == eHitState.OnlyEffect ) {
                mTempTargetList.Add( target );

                checkCritical = !atkEvent.isCritical ? atkEvent.aniEvent.IsCritical : atkEvent.isCritical;
				if ( checkCritical && atkEvent.aniEvent.IsCritical ) {
					Player player = atkEvent.sender as Player;
					if ( player ) {
						player.decideCritical = true;
					}
				}

				target.OnHit( atkEvent.sender, atkEvent.ToExecuteType, atkEvent.aniEvent, atkEvent.atkPower, ref checkCritical, ref enemyHitState,
							  null, atkEvent.isUltimateSkill, atkEvent.SkipMaxDamageRecord );

				if ( checkCritical && !isCritical ) {
					isCritical = true;
				}

                if ( target.MainCollider ) {
                    ++target.MainCollider.HitCount;
                }
			}
		}

		if ( mTempTargetList.Count > 0 ) {
			atkEvent.sender.AddHitTargetList( mTempTargetList, true );
		}

		List<Unit> listMarkingEnemy = EnemyMgr.GetMarkingEnemies( Unit.eMarkingType.ConnectingAttack );
		for ( int i = 0; i < listMarkingEnemy.Count; i++ ) {
            atkEvent.sender.AddHitTarget( listMarkingEnemy[i] );
        }

		if ( mTempTargetList.Count > 0 ) {
			int actionTableId = 0;

			if ( !atkEvent.SkipCheckOwnerAction && atkEvent.sender.actionSystem ) {
				ActionSelectSkillBase action = atkEvent.sender.actionSystem.GetCurrentAction<ActionSelectSkillBase>();
				if ( action ) {
					actionTableId = action.TableId;
				}

				if ( ( action == null || action.TableId == 0 ) && atkEvent.ActionTableId > 0 ) {
					actionTableId = atkEvent.ActionTableId;
				}
			}

            if ( successHit == true ) {
                atkEvent.sender.OnSuccessAttack( atkEvent.aniEvent, atkEvent.notAniEventAtk, actionTableId, atkEvent.Pjt, isCritical );
            }
            else {
                atkEvent.sender.OnOnlyDamageAttack( atkEvent.aniEvent, atkEvent.notAniEventAtk, actionTableId, atkEvent.Pjt, isCritical );
            }
		}
	}

	private void CheckMeleeAttack( AttackEvent atkEvent ) {
        if( Director.IsPlaying || atkEvent == null || IsEndGame ) {
            return;
        }

		if( ClearMode == eClearMode.BossDie && IsBossDead ) {
			return;
		}

		bool successHit = false;
		bool checkCritical = false;

        mTempTargetList.Clear();

        if ( atkEvent.aniEvent.hitBoxSize != Vector3.zero ) {
			Unit atkUnit = atkEvent.sender;
			if ( atkEvent.DroneUnit != null ) {
				atkUnit = atkEvent.DroneUnit;
			}

			Vector3 bonePos = atkUnit.transform.position;
			Vector3 boneRot = Vector3.zero;

			Transform bone = atkUnit.aniEvent.GetBoneByName( atkEvent.aniEvent.hitBoxBoneName );
			if ( bone ) {
				if ( atkEvent.aniEvent.useBoneRot ) {
					bonePos = bone.transform.position + ( bone.transform.forward * ( atkEvent.aniEvent.hitBoxSize.z * 0.5f ) );
					boneRot = bone.transform.rotation.eulerAngles;
				}
				else {
					bonePos = bone.transform.position;
				}
			}

			Vector3 pos = bonePos + ( atkUnit.transform.rotation * atkEvent.aniEvent.hitBoxPosition );
			if ( atkEvent.aniEvent.useOnlyHitBoxPos ) {
				pos = atkEvent.aniEvent.hitBoxPosition;
			}

			Quaternion q = Quaternion.identity;

			if ( !atkEvent.aniEvent.useOnlyHitBoxPos ) {
				q = Quaternion.Euler( boneRot + atkEvent.aniEvent.hitBoxPosition + new Vector3( 0.0f, atkUnit.transform.eulerAngles.y, 0.0f ) );

				if ( atkEvent.aniEvent.useBoneRot ) {
					q = Quaternion.Euler( boneRot + atkEvent.aniEvent.hitBoxRotation );
				}
			}

			int layer = Utility.GetEnemyLayer( (eLayer)atkUnit.gameObject.layer ) | 1 << (int)eLayer.EnvObject;

			Collider[] cols = Physics.OverlapBox( pos, atkEvent.aniEvent.hitBoxSize * 0.5f, q, layer );
			if ( cols.Length > 0 ) {
				for ( int i = 0; i < cols.Length; i++ ) {
					UnitCollider unitCollider = cols[i].GetComponent<UnitCollider>();
					if ( unitCollider == null || unitCollider.Owner.isDying || ( unitCollider && unitCollider.Owner as FigureUnit ) || 
                         ( unitCollider && unitCollider.Owner as GimmickObject ) ) {
						continue;
					}

					mTempTargetList.Add( unitCollider.Owner );
				}
			}
		}
		else {
			List<UnitCollider> listAttackerTargetCollider = atkEvent.sender.GetTargetColliderList();
			for( int i = 0; i < listAttackerTargetCollider.Count; i++ ) {
				UnitCollider collider = listAttackerTargetCollider[i];
                if ( collider == null || collider.Owner.isDying || ( collider && collider.Owner as FigureUnit ) || 
                     ( collider && collider.Owner as GimmickObject ) ) {
                    continue;
                }

				if( !atkEvent.sender.IsTargetInAttackRange( collider, atkEvent.atkDir, atkEvent.aniEvent.atkDirAngle, atkEvent.aniEvent.atkRange ) ) {
					continue;
				}

                mTempTargetList.Add( collider.Owner );
			}
		}

        if ( mTempTargetList.Count > 0 ) {
            atkEvent.sender.AddHitTargetList( mTempTargetList );
		}

		List<Unit> listMarkingEnemy = EnemyMgr.GetMarkingEnemies(Unit.eMarkingType.ConnectingAttack);
		for( int i = 0; i < listMarkingEnemy.Count; i++ ) {
            atkEvent.sender.AddHitTarget( listMarkingEnemy[i] );
		}

        if( mTempTargetList.Count <= 0 ) {
            return;
        }

		bool isCritical = false;
		bool hasTarget = false;

		for( int i = 0; i < mTempTargetList.Count; i++ ) {
			Unit target = mTempTargetList[i];
            if( target == null || target.IsActivate() == false || !target.IsShowMesh ) {
                continue;
            }

            if( target.actionSystem.IsCurrentQTEAction() || target.actionSystem.IsCurrentUSkillAction() ) {
                continue;
            }

			Unit.eSuperArmor targetSuperArmor = target.CurrentSuperArmor;
			eHitState enemyHitState = eHitState.Fail;

			if( targetSuperArmor == Unit.eSuperArmor.Invincible || target.TemporaryInvincible ) {
				enemyHitState = eHitState.OnlyEffect;
			}
			else {
                if( ( !Utility.IsSpecialAttack( atkEvent.aniEvent, null ) && targetSuperArmor == Unit.eSuperArmor.Lv1 ) ||
                    ( atkEvent.notAniEventAtk && targetSuperArmor == Unit.eSuperArmor.Lv1 ) || atkEvent.aniEvent.IsOnlyDamage ) {
                    enemyHitState = eHitState.OnlyDamage;
                }
                else if( targetSuperArmor >= Unit.eSuperArmor.Lv2 ) {
                    enemyHitState = eHitState.OnlyDamage;
                }
                else if( target.reserveStun == false ) {
                    enemyHitState = eHitState.Success;
                    successHit = true;
                }
			}

			if( enemyHitState == eHitState.Success || enemyHitState == eHitState.OnlyDamage || enemyHitState == eHitState.OnlyEffect ) {
				checkCritical = !atkEvent.isCritical ? atkEvent.aniEvent.IsCritical : atkEvent.isCritical;
				if( checkCritical && atkEvent.aniEvent.IsCritical ) {
					Player player = atkEvent.sender as Player;
					if( player ) {
						player.decideCritical = true;
					}
				}

				target.OnHit( atkEvent.sender, atkEvent.ToExecuteType, atkEvent.aniEvent, atkEvent.atkPower, ref checkCritical, ref enemyHitState,
							  null, atkEvent.isUltimateSkill, atkEvent.SkipMaxDamageRecord );

				if( checkCritical && !isCritical ) {
					isCritical = true;
				}

                if ( target.MainCollider ) {
                    ++target.MainCollider.HitCount;
                }

				hasTarget = true;
			}
		}

		int actionTableId = 0;
		ActionSelectSkillBase action = atkEvent.sender.actionSystem.GetCurrentAction<ActionSelectSkillBase>();
		if( action ) {
			actionTableId = action.TableId;
		}

		if( actionTableId == 0 && atkEvent.sender.isClone && atkEvent.sender.SendBattleOptionToOwner && atkEvent.sender.cloneOwner ) {
			action = atkEvent.sender.cloneOwner.actionSystem.GetCurrentAction<ActionSelectSkillBase>();
			if( action ) {
				actionTableId = action.TableId;
			}
		}

        if ( actionTableId == 0 ) {
			ActionGuardianBase actionGuardianBase = atkEvent.sender.actionSystem.GetCurrentAction<ActionGuardianBase>();
            if ( actionGuardianBase != null ) {
				actionTableId = actionGuardianBase.TableId;

			}
		}

		if( mTempTargetList.Count > 0 ) {
            if( successHit ) {
                atkEvent.sender.OnSuccessAttack( atkEvent.aniEvent, atkEvent.notAniEventAtk, actionTableId, null, isCritical );
            }
            else {
                atkEvent.sender.OnOnlyDamageAttack( atkEvent.aniEvent, atkEvent.notAniEventAtk, actionTableId, null, isCritical );
            }
		}
		else if( hasTarget ) { // 히트타겟이 있었는데 타겟의 OnHit안에서 히트타겟에서 빠지는 조건 때문에 (슈퍼 아머 변경 등) 히트타겟이 없어진 경우 or 환경 오브젝트
            if( successHit ) {
                atkEvent.sender.OnSuccessAttack( atkEvent.aniEvent, atkEvent.notAniEventAtk, actionTableId, null, isCritical );
            }
            else {
                atkEvent.sender.OnOnlyDamageAttack( atkEvent.aniEvent, atkEvent.notAniEventAtk, actionTableId, null, isCritical );
            }
		}
	}

    private void CheckProjectileHit( AttackEvent atkEvent ) {
        if ( Director.IsPlaying || atkEvent == null || IsEndGame == true || atkEvent.listTargetCollider == null || atkEvent.listTargetCollider.Count <= 0 )
            return;

        if ( ClearMode == eClearMode.BossDie && IsBossDead ) {
            return;
        }

        UnitCollider atkEvtTargetCollider = atkEvent.listTargetCollider[0];

        if ( atkEvtTargetCollider.Owner.IsActivate() == false || !atkEvtTargetCollider.Owner.IsShowMesh ) {
            return;
        }

        if ( atkEvtTargetCollider.Owner.actionSystem.IsCurrentQTEAction() || atkEvtTargetCollider.Owner.actionSystem.IsCurrentUSkillAction() ) {
            return;
        }

        if ( atkEvtTargetCollider.Owner.isDying == true ) {
            return;
        }

		Unit nearestHitTarget = null;
		bool successHit = false;
		Unit.eSuperArmor targetSuperArmor = atkEvtTargetCollider.Owner.CurrentSuperArmor;
		eHitState enemyHitState = eHitState.Fail;

		if ( targetSuperArmor == Unit.eSuperArmor.Invincible || atkEvtTargetCollider.Owner.TemporaryInvincible ) {
			enemyHitState = eHitState.OnlyEffect;
		}
		else {
			if ( atkEvent.aniEvent.behaviour == eBehaviour.GroggyAttack && targetSuperArmor < Unit.eSuperArmor.Lv2 ) {
				enemyHitState = eHitState.Success;
				successHit = true;
			}
			else {
				if ( atkEvent.Pjt.ProjectileAtkAttr == Projectile.EProjectileAtkAttr.ONLY_DAMAGE ||
					( !Utility.IsSpecialAttack( atkEvent.aniEvent, atkEvent.Pjt ) && targetSuperArmor == Unit.eSuperArmor.Lv1 ) ||
					( atkEvent.notAniEventAtk && targetSuperArmor == Unit.eSuperArmor.Lv1 ) ||
					( atkEvent.Pjt.ProjectileAtkAttr == Projectile.EProjectileAtkAttr.NORMAL && targetSuperArmor == Unit.eSuperArmor.Lv1 ) ) {
					enemyHitState = eHitState.OnlyDamage;
				}
				else if ( targetSuperArmor >= Unit.eSuperArmor.Lv2 ) {
					enemyHitState = eHitState.OnlyDamage;
				}
				else if ( atkEvtTargetCollider.Owner.reserveStun == false ) {
					enemyHitState = eHitState.Success;
					successHit = true;
				}
			}
		}

		bool checkCritical = false;
		bool isCritical = false;
		bool hasTarget = false;

		mTempTargetList.Clear();

		if ( enemyHitState == eHitState.Success || enemyHitState == eHitState.OnlyDamage || enemyHitState == eHitState.OnlyEffect ) {
            mTempTargetList.Add( atkEvtTargetCollider.Owner );

            checkCritical = !atkEvent.isCritical ? atkEvent.aniEvent.IsCritical : atkEvent.isCritical;
			if ( checkCritical && atkEvent.aniEvent.IsCritical ) {
				Player player = atkEvent.sender as Player;
				if ( player ) {
					player.decideCritical = true;
				}
			}

			atkEvtTargetCollider.Owner.OnHit( atkEvent.sender, atkEvent.ToExecuteType, atkEvent.aniEvent, atkEvent.atkPower, ref checkCritical,
											  ref enemyHitState, atkEvent.Pjt, atkEvent.isUltimateSkill, atkEvent.SkipMaxDamageRecord );

			if ( checkCritical && !isCritical ) {
				isCritical = true;
			}

			++atkEvtTargetCollider.HitCount;
			hasTarget = true;

            nearestHitTarget = atkEvtTargetCollider.Owner;
        }

        if ( mTempTargetList.Count > 0 ) {
            atkEvent.sender.AddHitTargetList( mTempTargetList );
        }

        List<Unit> listMarkingEnemy = EnemyMgr.GetMarkingEnemies( Unit.eMarkingType.ConnectingAttack );
		for ( int i = 0; i < listMarkingEnemy.Count; i++ ) {
            atkEvent.sender.AddHitTarget( listMarkingEnemy[i] );
		}

		int actionTableId = 0;
		if ( atkEvent.Pjt ) {
			actionTableId = atkEvent.Pjt.OwnerActionTableId;
		}

		if ( mTempTargetList.Count > 0 && nearestHitTarget != null ) {
			if ( enemyHitState == eHitState.Success ) {
				atkEvent.sender.OnSuccessAttack( atkEvent.aniEvent, atkEvent.notAniEventAtk, actionTableId, atkEvent.Pjt, isCritical );
			}
			else if ( enemyHitState == eHitState.OnlyDamage ) {
				atkEvent.sender.OnOnlyDamageAttack( atkEvent.aniEvent, atkEvent.notAniEventAtk, actionTableId, atkEvent.Pjt, isCritical );
			}
		}
		else if ( hasTarget ) { // 히트타겟이 있었는데 타겟의 OnHit안에서 히트타겟에서 빠지는 조건 때문에 (슈퍼 아머 변경 등) 히트타겟이 없어진 경우 or 환경 오브젝트
			if ( successHit == true ) {
				atkEvent.sender.OnSuccessAttack( atkEvent.aniEvent, atkEvent.notAniEventAtk, actionTableId, null, isCritical );
			}
			else {
				atkEvent.sender.OnOnlyDamageAttack( atkEvent.aniEvent, atkEvent.notAniEventAtk, actionTableId, null, isCritical );
			}
		}
	}

    /*
	private void DamageOnce(AttackEvent atkEvent)
    {
        if (atkEvent == null)
            return;

        StopCoroutine("DelayedDamageOnce");
        StartCoroutine("DelayedDamageOnce", atkEvent);
    }

    private IEnumerator DelayedDamageOnce(AttackEvent atkEvent)
    {
        List<UnitCollider> listTargetCollider = atkEvent.listTargetCollider;
        if (IsEndGame == true || listTargetCollider.Count <= 0)
            yield break;

        if (atkEvent.attackerEffType == EffectManager.eType.Common)
        {
            EffectManager.Instance.Play(atkEvent.sender, atkEvent.attackerEffId, atkEvent.attackerEffType);
        }
        else if (atkEvent.attackerEffType == EffectManager.eType.Common_Show_CenterOfEnemies)
        {
            Vector3 v = Vector3.zero;
            for (int i = 0; i < listTargetCollider.Count; i++)
            {
                Unit target = listTargetCollider[i].Owner;
                if (target == null || target.IsActivate() == false)
                    continue;

                v += target.transform.position;
            }

            v /= listTargetCollider.Count;
            v.y = atkEvent.sender.transform.position.y;
            EffectManager.Instance.Play(v, atkEvent.attackerEffId, atkEvent.attackerEffType);
        }

        //ListHitTarget.Clear();

        WaitForSeconds waitForSeconds = new WaitForSeconds(atkEvent.delay);
        for (int i = 0; i < listTargetCollider.Count; i++)
        {
            Unit target = listTargetCollider[i].Owner;
            if (target == null || target.IsActivate() == false || target.curHp <= 0.0f)
                continue;

            if (target.actionSystem.IsCurrentQTEAction() || target.actionSystem.IsCurrentUSkillAction())
                continue;

            if (target.isDying == true)
                continue;

            Unit.eSuperArmor targetSuperArmor = target.CurrentSuperArmor;
            if (targetSuperArmor == Unit.eSuperArmor.Invincible || target.TemporaryInvincible)
            {
                continue;
            }

            AddHitTargetList(target);
        }

        if (ListHitTarget.Count > 0)
            atkEvent.sender.SetListTarget(ListHitTarget);

        yield return waitForSeconds;

        bool isCritical = false;
        for (int i = 0; i < ListHitTarget.Count; i++)
        {
            eHitState enemyHitState = eHitState.Success;

            isCritical = atkEvent.isCritical;
            ListHitTarget[i].OnHit( atkEvent.sender, atkEvent.ToExecuteType, atkEvent.aniEvent, atkEvent.atkPower, ref isCritical, ref enemyHitState, 
                                    null, atkEvent.isUltimateSkill, atkEvent.SkipMaxDamageRecord );
        }
    }
    */

	protected virtual void EndGame( eEventType eventType, Unit sender ) {
		if( IsEndGame ) {
			return;
		}

		AppMgr.Instance.CustomInput.ShowCursor( true );
		IsEndGame = true;

		ProjectileMgr.DestroyAllProjectile( false );
		InGameCamera.SetFixedMode( InGameCamera.transform.position, InGameCamera.transform.eulerAngles, InGameCamera.DEFAULT_FOV );

		Utility.StopCoroutine( this, ref mCrTimer );

		Player.ClearEnemyNavigators();
		Player.ClearGateNavigator();
		Player.ClearOtherObjectNavigator();

        for( int i = 0; i < ListPlayer.Count; i++ ) {
            ListPlayer[i].Input.Pause( true );
            ListPlayer[i].OnGameEnd();
        }

		bool win = CheckWinGame( eventType );
		if( win == true ) {
			if( mBossDieDuration <= 0.0f ) {
				Time.timeScale = 0.2f;
			}

			GameUIManager.Instance.HideUI( "GamePlayPanel", false );
			StartCoroutine( "CheckWinDirector" );
		}
		else {
            for( int i = 0; i < ListPlayer.Count; i++ ) {
                ListPlayer[i].HideAllClone();
            }

			SendStageFailed();
		}
	}

	private bool CheckWinGame(eEventType eventType)
    {
        if (StageType == eSTAGETYPE.STAGE_TIMEATTACK || (StageData != null && StageData.ConditionValue > 0))
        {
            if (eventType == eEventType.EVENT_GAME_ALL_ENEMY_DEAD || eventType == eEventType.EVENT_GAME_ENEMY_BOSS_DIE)
            {
                return true;
            }
            else if (GameTime <= 0.0f || eventType == eEventType.EVENT_GAME_PLAYER_DEAD)
            {
                return false;
            }
        }
        else if (StageType == eSTAGETYPE.STAGE_SPECIAL)
        {
            if (eventType == eEventType.EVENT_RACE_FAILED)
            {
                return false;
            }
            else if (eventType == eEventType.EVENT_RACE_SUCCESS)
            {
                return true;
            }
        }
        else
        {
            if(eventType == eEventType.EVENT_GAME_OTHER_OBJECT_DEAD)
            {
                return false;
            }
            else if (eventType == eEventType.EVENT_GAME_ALL_ENEMY_DEAD || eventType == eEventType.EVENT_GAME_ENEMY_BOSS_DIE)
            {
                return true;
            }
            else if (Player.curHp <= 0.0f)
            {
                return false;
            }
        }

        return false;
    }

	private IEnumerator CheckWinDirector() {
		Director directorUSkill = Player.GetDirector( "USkill01" );
		if( directorUSkill.playableDirector.state == PlayState.Playing ) {
			directorUSkill.End();
		}

		if( mBossDieDuration <= 0.0f ) {
			yield return new WaitForSeconds( 2.0f * Time.timeScale );
			Time.timeScale = 1.0f;
		}

		for( int i = 0; i < ListPlayer.Count; i++ ) {
			ListPlayer[i].OnEndGame();
		}

		float fWaitTime = 0;
		float fBossDieDuration = mBossDieDuration - ( 1.0f / Application.targetFrameRate );

		while( fWaitTime <= fBossDieDuration ) {
			fWaitTime += Time.fixedDeltaTime;
			if( IsEndBossDie ) {
				break;
			}

			yield return mWaitForFixedUpdate;
		}

		IsEndBossDie = false;

		GameUIManager.Instance.ShowUI( "GameOffPopup", false );
		yield return new WaitForSeconds( 0.1f );

		EnemyMgr.CollectDropItem();
		EnemyMgr.ClearEnvObjects();

		GameUIManager.Instance.HideUI( "GameOffPopup", false );
		GameUIManager.Instance.HideUI( "GamePlayPanel", false );

		if( Player.HasDirector( "Win01" ) == false ) { // 미니게임이나 스토리엔 승리 포즈 없음
			if( mbLastStorySequence == false )
				IsEndGame = true;
			else
				ShowResultUI();
		}
		else {
			StartCoroutine( "WaitForGroundedAndPlayWinDirector" );
		}
	}

	private IEnumerator WaitForGroundedAndPlayWinDirector() {
		Time.timeScale = 1.0f;

		if( !Player.isGrounded ) {
			Player.transform.position = Player.posOnGround;
		}

		Player.aniEvent.StopEffects();

		Player.actionSystem.CancelCurrentAction();
		Player.StopStepForward();
		Player.PlayAni( eAnimation.Win, 0, eFaceAnimation.WinIdle, 0 );

		if ( Player.UseGuardian == true && Player.Guardian != null ) {
			Player.Guardian.StopBT();
			Player.Guardian.actionSystem.CancelCurrentAction();
			Player.Guardian.PlayAni( eAnimation.Win );
		}

		ChangeToFinishBGM( true );

		Vector3 pos = Player.transform.position;
		Quaternion rot = Player.transform.rotation;

		if( UseBossSpawnPosition ) {
			pos = BossSpawnPosition;
			rot = BossSpawnRotation;
		}

		BattleAreaManager battleAreaMgr = EnemyMgr as BattleAreaManager;
		if( battleAreaMgr ) {
			BattleArea battleArea = battleAreaMgr.GetCurrentBattleArea();
			if( battleArea && battleArea.center ) {
				pos = battleArea.center.position;
				rot = battleArea.center.rotation;
			}
		}

		Player.transform.position = new Vector3( pos.x, Player.posOnGround.y, pos.z );
		Player.transform.rotation = rot;

		Director clearDirector = Player.GetDirector("Win01");

		Player.PlayDirector( "Win01", null, ShowResultUI, null );

		//스테이지 클리어 연출중에 터치시 결과 UI 바로 출력
		if( clearDirector != null ) {
			while( !clearDirector.isEnd ) {
				if( AppMgr.Instance.CustomInput.GetButtonDown( BaseCustomInput.eKeyKind.Select ) ) {
					clearDirector.SetTime( (float)clearDirector.GetDuration() );
					ShowResultUI();

					break;
				}
				yield return null;
			}
		}

		yield return null;
	}

	private bool CheckValueType(eCompareType compareType, float compareValue, float conditionValue)
    {
        switch (compareType)
        {
            case eCompareType.Equal:
                if (compareValue.CompareTo(conditionValue) == 0)
                    return true;
                break;

            case eCompareType.NotEqual:
                if (compareValue.CompareTo(conditionValue) != 0)
                    return true;
                break;

            case eCompareType.Less:
                if (compareValue.CompareTo(conditionValue) < 0)
                    return true;
                break;

            case eCompareType.LessEqual:
                if (compareValue.CompareTo(conditionValue) <= 0)
                    return true;
                break;

            case eCompareType.Greater:
                if (compareValue.CompareTo(conditionValue) > 0)
                    return true;
                break;

            case eCompareType.GreaterEqual:
                if (compareValue.CompareTo(conditionValue) >= 0)
                    return true;
                break;
        }

        return false;
    }

    private void EnablePlayerMainCollider() {
        if( Player ) {
            Player.MainCollider.Enable( true );
		}
	}
}
