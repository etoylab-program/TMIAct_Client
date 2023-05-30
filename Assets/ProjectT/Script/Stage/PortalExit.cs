
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PortalExit : MonoBehaviour
{
    public enum ESideViewType
    {
        NONE = 0,
        LEFT,
        RIGHT,
    }


    [Header("[Proprety]")]
    public bool                         UseScenario             = false;
    public PortalEntry.EScenarioType    ScenarioType            = PortalEntry.EScenarioType.SCENARIO;
    public int                          ScenarioId              = 0;
    public string                       ScenarioDirectorPath    = null;

    public bool                         UseCameraSetting        = false;
    public InGameCamera.EMode           CameraMode              = InGameCamera.EMode.DEFAULT;
    public InGameCamera.sDefaultSetting CameraDefaultSetting    = new InGameCamera.sDefaultSetting(InGameCamera.DEFAULT_CAMERA_DISTANCE, InGameCamera.DEFAULT_CAMERA_LOOKAT);
    public InGameCamera.sSideSetting    CameraSideSetting       = new InGameCamera.sSideSetting(new Vector3(0.0f, 1.5f, 4.0f), new Vector2(0.0f, 1.0f), Unit.eAxisType.Z);
    public InGameCamera.sFixedSetting   CameraFixedSetting      = new InGameCamera.sFixedSetting();

    private WorldStage      mWorldStage     = null;
    private BattleArea      mBattleArea     = null;
    private PortalEntry     mEntry          = null;
    private Player          mPlayer         = null;
	private PlayerGuardian  mPlayerGuardian = null;


	public void Init(BattleArea battleArea)
    {
        mWorldStage = World.Instance as WorldStage;

        mBattleArea = battleArea;
        mEntry = mBattleArea.portalEntry.GetComponent<PortalEntry>();
    }

	private void OnTriggerEnter( Collider col ) {
		if( col.CompareTag( "Player" ) == false ) {
			return;
		}

		mPlayer = col.GetComponent<Player>();
		if( mPlayer == null || mPlayer.IsHelper ) {
			return;
		}

		mPlayerGuardian = mPlayer.Guardian;

		StartCoroutine( "PlayerOnTrigger" );
	}

	private IEnumerator PlayerOnTrigger()
    {
        while (!mEntry.PortalEntryEnd)
        {
            yield return null;
        }

        //mPlayer.LockZAxis(false);
        mPlayer.SetLockAxis(Unit.eAxisType.None);
        if(CameraMode == InGameCamera.EMode.SIDE)
        {
            //mPlayer.LockZAxis(true);
            mPlayer.SetLockAxis(CameraSideSetting.LockAxis);
        }

        if (UseScenario)
        {
            if (ScenarioType == PortalEntry.EScenarioType.SCENARIO)
            {
                mWorldStage.ShowScenario(ScenarioId, StartExitCoroutine);
            }
            else if (ScenarioType == PortalEntry.EScenarioType.DIRECTOR)
            {
                PlayScenarioDirector();
            }
        }
        else
        {
            StartExitCoroutine();
        }
    }

    private void StartExitCoroutine()
    {
        StartCoroutine("ExitPortal");
    }

	private IEnumerator ExitPortal() {
		while( !mEntry.PortalEntryEnd ) {
			yield return null;
		}

        if( !UseScenario ) {
			mPlayer.Input.Pause( false );
		}

        for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
            World.Instance.ListPlayer[i].actionSystem.CancelCurrentAction();
        }

        mBattleArea.ShowPortal( false );
		mBattleArea.gameObject.SetActive( false );

		if( UseCameraSetting ) {
			if( CameraMode == InGameCamera.EMode.DEFAULT ) {
				World.Instance.InGameCamera.SetMode( CameraMode, CameraDefaultSetting );
				FSaveData.Instance.RemoveCameraSettingData();
			}
			else if( CameraMode == InGameCamera.EMode.SIDE ) {
				World.Instance.InGameCamera.SetMode( CameraMode, CameraSideSetting );
				CameraSideSetting.Save();
			}
			else if( CameraMode == InGameCamera.EMode.FIXED ) {
				World.Instance.InGameCamera.SetMode( CameraMode, CameraFixedSetting );
				CameraFixedSetting.Save();
			}
		}
		else {
			World.Instance.InGameCamera.SetDefaultMode( InGameCamera.DEFAULT_CAMERA_DISTANCE, InGameCamera.DEFAULT_CAMERA_LOOKAT, true );
		}

        mPlayer.EnableRayCastCol( true );
        mPlayer.SetGroundedRigidBody();

        for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
            World.Instance.ListPlayer[i].EnableRayCastCol( true );
            World.Instance.ListPlayer[i].SetGroundedRigidBody();
        }

        mPlayer.SetAutoMove( true );
        World.Instance.ShowHelperMesh( true, mPlayer.transform.forward * 0.5f );

		GuardianExitPortal();

		yield return null;
	}

	private void PlayScenarioDirector()
    {
        Director director = GameSupport.CreateDirector(ScenarioDirectorPath);
        if (director == null)
        {
            Debug.Log(ScenarioDirectorPath + " 파일을 찾을 수 없습니다.");
            return;
        }

        director.Init(null);
        director.Play();
    }

    private void GuardianExitPortal() {
        if ( mPlayerGuardian == null ) {
            return;
        }

		mPlayerGuardian.Deactivate();
		mPlayerGuardian.Activate();

		mPlayerGuardian.ResetBT();
	}
}
