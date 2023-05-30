using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

public class WorldThrowShooting : World
{
    [Header("Throw Mode Property")]
    public Transform kStartTransform;
    public float kScoreRate = 0.03f;

    public bool kTestMode = false;


	//[Header("Test Property - Delete..")]


	public override void Init( int tableId, eSTAGETYPE stageType = eSTAGETYPE.STAGE_NONE ) {
		base.Init( tableId, stageType );
		AppMgr.Instance.CustomInput.ShowCursor( false );

    #if !UNITY_EDITOR
        kTestMode = false;
    #endif

        if( StageData == null ) {
            return;
        }

		StageData.StageType = (int)eSTAGETYPE.STAGE_SPECIAL;
		StageType = eSTAGETYPE.STAGE_SPECIAL;

		if( !string.IsNullOrEmpty( StageData.BGM ) ) {
			mBgm = LoadAudioClip( StageData.BGM );
		}

		if( !string.IsNullOrEmpty( StageData.AmbienceSound ) ) {
			mAmbienceSnd = LoadAudioClip( StageData.AmbienceSound );
		}

		GameUIManager.Instance.HideUI( "GamePlayPanel", false );

		CharData yukikaze = GameInfo.Instance.GetCharDataByTableID((int)ePlayerCharType.Yukikaze);
		WeaponData defaultWeapon = null;
		if( yukikaze == null || kTestMode ) {
			CharData originYukikazeData = yukikaze;

			if( yukikaze != null ) {
				GameInfo.Instance.CharList.Remove( yukikaze );
				yukikaze = null;
			}

			if( GameInfo.Instance.WeaponList.Find( x => x.TableID == 3003 ) == null ) {
				defaultWeapon = new WeaponData();
				defaultWeapon.WeaponUID = 1;
				defaultWeapon.TableData = GameInfo.Instance.GameTable.FindWeapon( x => x.ID == 3003 );
				defaultWeapon.TableID = 3003;
				GameInfo.Instance.WeaponList.Add( defaultWeapon );
			}

			yukikaze = new CharData();
			yukikaze.TableID = (int)ePlayerCharType.Yukikaze;
			yukikaze.Level = 1;
			yukikaze.TableData = GameInfo.Instance.GameTable.FindCharacter( x => x.ID == yukikaze.TableID );
			yukikaze.EquipCostumeID = 3003;

			if( defaultWeapon == null ) {
				WeaponData weapondata = GameInfo.Instance.WeaponList.Find(x => x.TableID == 3003);
                if( weapondata != null ) {
                    yukikaze.EquipWeaponUID = weapondata.WeaponUID;
                }
			}
			else {
				yukikaze.EquipWeaponUID = defaultWeapon.WeaponUID;
			}

			GameInfo.Instance.CharList.Add( yukikaze );
			Player = GameSupport.CreatePlayer( yukikaze );

			//임싷로 추가 했던 유키카제 제거
			GameInfo.Instance.CharList.Remove( yukikaze );

			//임시로 추가 했던 유키카제 미니게임 기본 무기 제거
			if( defaultWeapon != null ) {
				GameInfo.Instance.WeaponList.Remove( defaultWeapon );
			}

			if( originYukikazeData != null ) {
				GameInfo.Instance.CharList.Add( originYukikazeData );
			}
		}
		else {
			Player = GameSupport.CreatePlayer( yukikaze );
		}

		if( Player != null ) {
			Player.SetHp( 100f );
			Player.Input.enabled = false;
			Player.SetKinematicRigidBody( true );
			Player.MainCollider.Enable( false );
			Player.Input.Pause( true );
			Player.transform.localPosition = kStartTransform.position;
			Player.transform.localRotation = kStartTransform.rotation;
		}

		Director director = null;
		if( !string.IsNullOrEmpty( StageData.ScenarioDrt_Start ) ) {
			director = GameSupport.CreateDirector( StageData.ScenarioDrt_Start );
			AddDirector( Player, "Start", director );
		}

		director = GameSupport.CreateDirector( "drt_minigame_shooting_start_yukikaze" );
		Utility.InitTransform( director.gameObject, kStartTransform.position, kStartTransform.rotation, Vector3.one );
		AddDirector( Player, "ShootingStart", director );

		director = GameSupport.CreateDirector( "drt_minigame_shooting_fail_yukikaze" );
		AddDirector( Player, "GameFailed", director );
		Utility.InitTransform( director.gameObject, kStartTransform.position, kStartTransform.rotation, Vector3.one );

		director = GameSupport.CreateDirector( "drt_minigame_shooting_clear_yukikaze" );
		AddDirector( Player, "GameSuccess", director );
		Utility.InitTransform( director.gameObject, kStartTransform.position, kStartTransform.rotation, Vector3.one );

		PlayDirector( "ShootingStart", EndIntro );

		PostInit();
        GameUIManager.Instance.HideUI( "GameOffPopup", false );
    }

	protected override void EndIntro()
    {
        base.EndIntro();

        Player.SetKinematicRigidBody(true);
        Player.PlayAniImmediate(eAnimation.ThrowIdle);

        GameStart();
    }

    private void GameStart()
    {
        GameUIManager.Instance.ShowUI("GameStartPanel", true);

        //호루라기 사운드
        SoundManager.Instance.PlayUISnd(74);
        if (!Player.gameObject.activeSelf)
            Player.gameObject.SetActive(true);

        Log.Show("List Mtrl : " + Player.aniEvent.ListMtrl.Count);
        

        ThrowingManager.Instance.ThrowMiniGameStart(Player);
        GameUIManager.Instance.HideUI("GamePlayPanel", false);
        GameUIManager.Instance.ShowUI("ShootingModePanel", true);
    }

    public override void Pause(bool pause, bool withTimeScale = true, Unit exceptUnit = null, bool withoutUIPlayWidgetAlpha = false)
    {
        IsPause = pause;

        if (pause == true)
        {
            if (withTimeScale)
            {
                Time.timeScale = 0.0f;
            }
            else
            {
                Time.timeScale = 1.0f;
            }
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    public void OnShootingGameFailed()
    {
        IsEndGame = true;
        ChangeToFinishBGM(true);
        GameUIManager.Instance.HideUI("ShootingModePanel", false);
        TimeSpan ts = TimeSpan.FromSeconds(ThrowingManager.Instance.TimeChecker);
        GameInfo.Instance.Send_ReqStageEndFail((int)ts.TotalMilliseconds, null);
        PlayDirector("GameFailed");
        Invoke("GameFailedMoveToLobby", 3f);
    }

    private void GameFailedMoveToLobby()
    {
        //실패 사운드 
        SoundManager.Instance.PlayUISnd(76);
        BikeModeResultPopup.Show(false, 0, 0);
    }

    public void OnShootingGameSuccess()
    {
        IsEndGame = true;
        ChangeToFinishBGM(true);
        GameUIManager.Instance.HideUI("ShootingModePanel", false);
        PlayDirector("GameSuccess");
        Invoke("GameSuccessMoveToLobby", 3f);
    }

    private void GameSuccessMoveToLobby()
    {
        //성공 사운드
        SoundManager.Instance.PlayUISnd(75);
        ShowResultUI();
    }

    public override void ShowResultUI()
    {
        base.ShowResultUI();
        IsEndGame = true;

        Mission01 = CheckStageMission(0, StageData.Mission_00);
        Mission02 = CheckStageMission(1, StageData.Mission_01);
        Mission03 = CheckStageMission(2, StageData.Mission_02);

        GameInfo.Instance.MaxNormalBoxCount = Mathf.Min(GameInfo.Instance.MaxNormalBoxCount, StageData.N_DropMaxCnt);

        TimeSpan ts = TimeSpan.FromSeconds(ThrowingManager.Instance.TimeChecker);

        GameInfo.Instance.Send_ReqStageEnd(StageData.ID, GameInfo.Instance.SeleteCharUID, (int)ts.Milliseconds, (ObscuredInt)((ThrowingManager.Instance.GetScore() * kScoreRate)),
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

        BikeModeResultPopup.ScoreResultShot(true, ThrowingManager.Instance.GetScore(), (ObscuredInt)(ThrowingManager.Instance.GetScore() * kScoreRate));
    }
}
