
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldBike : World
{
    [Header("[Bike Mode]")]
    public RaceRider kRider;

    private RaceGameManager     mRaceMgr;
    private UIBikeRaceModePanel mUIRacePlay;
	private WaitForFixedUpdate	mWaitForFixedUpdate	= new WaitForFixedUpdate();


	public override void Init( int tableId, eSTAGETYPE stageType = eSTAGETYPE.STAGE_NONE ) {
		base.Init( tableId, stageType );
		AppMgr.Instance.CustomInput.ShowCursor( false );

		if( StageData == null ) {
			return;
		}

		mUIRacePlay = GameUIManager.Instance.GetActiveUI<UIBikeRaceModePanel>( "BikeRaceModePanel" );
		GameUIManager.Instance.HideUI( "GamePlayPanel", false );

		mbLastStorySequence = true;

		if( !string.IsNullOrEmpty( StageData.BGM ) ) {
			mBgm = LoadAudioClip( StageData.BGM );
		}

		if( !string.IsNullOrEmpty( StageData.AmbienceSound ) ) {
			mAmbienceSnd = LoadAudioClip( StageData.AmbienceSound );
		}

		//레이스 모드 바닥에 까는 아이템, 장애물 생성
		string strModel = Utility.AppendString("BackGround/StageLevel/", StageData.LevelPB, ".prefab");
		mRaceMgr = ResourceMgr.Instance.CreateFromAssetBundle<RaceGameManager>( "background", strModel );

		ClearConditionValue = StageData.ConditionValue;
		mCheckConditionValue = 0;
		mUIRacePlay.ClearConditionValue = ClearConditionValue;

		GameTime = 0.0f;

		CharData charAsagi = GameInfo.Instance.GetCharDataByTableID(1);
		bool bRemoveCharData = false;

		if( charAsagi == null ) {
			charAsagi = new CharData();
			charAsagi.TableID = (int)ePlayerCharType.Asagi;
			charAsagi.Level = 1;
			charAsagi.TableData = GameInfo.Instance.GameTable.FindCharacter( x => x.ID == charAsagi.TableID );
			charAsagi.EquipCostumeID = 1001;
			charAsagi.EquipWeaponUID = GameInfo.Instance.WeaponList[0].WeaponUID;
			GameInfo.Instance.CharList.Add( charAsagi );

			bRemoveCharData = true;
			Log.Show( "Add Dummy Asagi" );
		}

		Player = GameSupport.CreatePlayer( charAsagi );

		if( Player != null ) {
			Director director = null;

			if( !string.IsNullOrEmpty( StageData.ScenarioDrt_Start ) ) {
				director = GameSupport.CreateDirector( StageData.ScenarioDrt_Start );
				AddDirector( Player, "Start", director );
			}

			AddDirector( Player, "Race_Start", GameSupport.CreateDirector( "drt_race_start_asagi" ) );
			AddDirector( Player, "Race_Failed", GameSupport.CreateDirector( "drt_race_fail_asagi" ) );
			AddDirector( Player, "Race_Success", GameSupport.CreateDirector( "drt_race_clear_asagi" ) );

			//무기 안보이게 변경
			if( Player.GetComponentInChildren( typeof( AttachObject ), true ) != null ) {
				AttachObject attachObject = (AttachObject)Player.GetComponentInChildren(typeof(AttachObject), true);
                if( attachObject != null ) {
                    attachObject.gameObject.SetActive( false );
                }
			}

			if( Player.Input != null ) {
				Player.Input.enabled = false;
			}

			mRaceMgr.SetPlayer( kRider );
		}

		kRider.gameObject.SetActive( false );
        if( !PlayDirector( "Race_Start", EndIntro ) ) {
            EndIntro();
        }

		if( bRemoveCharData ) {
			GameInfo.Instance.CharList.Remove( charAsagi );
		}

		PostInit();
        GameUIManager.Instance.HideUI( "GameOffPopup", false );
	}

	public override void Pause(bool pause, bool withTimeScale = true, Unit exceptUnit = null, bool withoutUIPlayWidgetAlpha = false)
    {
        IsPause = pause;

        if (pause == true)
        {
            StopCoroutine("ResumeAll");

            if (withTimeScale)
            {
                Time.timeScale = 0.0f;
            }
            else
            {
                Time.timeScale = 1.0f;
            }

            kRider.PlayMoveAudio(pause);            
        }
        else
        {
            Time.timeScale = 1.0f;
            kRider.PlayMoveAudio();
        }
    }

    public override void ShowGamePlayUI(int aniIndex, Unit exceptUnit = null)
    {
        Pause(true, false, exceptUnit);
        mShowGamePlayUILength = mUIRacePlay.ShowUI(aniIndex);

        StopCoroutine("ResumeAll");
        StartCoroutine("ResumeAll", mShowGamePlayUILength);
    }

	public void OnRaceSuccess() {
		IsEndGame = true;
		kRider.PlayMoveAudio( true );
		PlayDirector( "Race_Success", null, true );
		Invoke( "ToLobbyWithRaceSuccess", 1f );
	}

	public void OnRaceFailed() {
		IsEndGame = true;
		kRider.PlayMoveAudio( true );

		TimeSpan ts = TimeSpan.FromSeconds(ProgressTime);
		GameInfo.Instance.Send_ReqStageEndFail( (int)ts.TotalMilliseconds, null );

		PlayDirector( "Race_Failed", null, true );
		Invoke( "ToLobbyWithRaceFailed", 3f );
	}

	public override void ShowResultUI()
    {
        base.ShowResultUI();
        IsEndGame = true;

        Mission01 = CheckStageMission(0, StageData.Mission_00);
        Mission02 = CheckStageMission(1, StageData.Mission_01);
        Mission03 = CheckStageMission(2, StageData.Mission_02);

        GameInfo.Instance.MaxNormalBoxCount = Mathf.Min(GameInfo.Instance.MaxNormalBoxCount, StageData.N_DropMaxCnt);
        TimeSpan ts = TimeSpan.FromSeconds(ProgressTime);

        GameInfo.Instance.Send_ReqStageEnd(StageData.ID, GameInfo.Instance.SeleteCharUID, (int)ts.TotalMilliseconds, mUIRacePlay.CurCoins * mUIRacePlay.CurHp,
                                           GameInfo.Instance.MaxNormalBoxCount,
                                           Mission01 != EMissionClearStatus.FAIL ? true : false,
                                           Mission02 != EMissionClearStatus.FAIL ? true : false,
                                           Mission03 != EMissionClearStatus.FAIL ? true : false,
                                           OnNetRaceGameResult);
    }

    public void OnNetRaceGameResult(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        BikeModeResultPopup.Show(true, mUIRacePlay.CurHp, mUIRacePlay.CurCoins * mUIRacePlay.CurHp);
    }

    protected override void EndIntro()
    {
        base.EndIntro();

        GameUIManager.Instance.ShowUI("GameStartPanel", true);
        if (!Player.gameObject.activeSelf)
            Player.gameObject.SetActive(true);

        //77    VoiceMgr.Instance.PlayChar(eVOICECHAR.Start, Player.tableId);

        //레이싱 모드 플레이어 설정, 바이크 자식으로 붙도록 설정
        Player.SetKinematicRigidBody(true);
        Player.MainCollider.Enable(false);
        Player.aniEvent.PlayAni(eAnimation.Race_Idle, false, 0);
        Player.Input.Pause(true);
        Player.transform.parent = kRider.kPlayerPos;
        Player.transform.localPosition = Vector3.zero;
        Player.transform.localRotation = Quaternion.identity;

        kRider.gameObject.SetActive(true);

        mUIRacePlay.SetUIActive(true);
        mUIRacePlay.StartRaceMode(0f);
        mCrTimer = StartCoroutine(RaceStartTimer(0f));
    }

    private IEnumerator RaceStartTimer(float delay)
    {
        GameTime = 0.0f;
        TimeSpan ts = TimeSpan.FromSeconds(GameTime);
        mUIRacePlay.UpdateTime(ts);
        yield return new WaitForSeconds(delay);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (IsEndGame == false)
        {
            if (!IsPause)
            {
                //ProgressTime += Time.fixedDeltaTime * Time.timeScale;
                GameTime += Time.fixedDeltaTime * Time.timeScale;
                ts = TimeSpan.FromSeconds(GameTime);
                mUIRacePlay.UpdateTime(ts);

            }

            yield return mWaitForFixedUpdate;
        }
    }

    private void ToLobbyWithRaceSuccess()
    {
        GameUIManager.Instance.HideUI("BikeRaceModePanel", false);
        ShowResultUI();
    }

    private void ToLobbyWithRaceFailed()
    {
        GameUIManager.Instance.HideUI("BikeRaceModePanel", false);
        BikeModeResultPopup.Show(false, 0, 0);
    }
}
