
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PortalEntry : MonoBehaviour
{
    public enum EType
    {
        NONE = 0,
        SCENARIO,
        ACROSS,
        JUMP,
    }

    public enum EScenarioType
    {
        SCENARIO = 0,
        DIRECTOR,
    }

    public enum EAcrossType
    {
        DIRECTOR = 0,
        ANIMATION,
    }


    public static bool IsAcrossByAniPlaying { get; private set; } = false;

    [Header("[Property]")]
    public EType                                PortalType                  = EType.NONE;

    public EScenarioType                        ScenarioType                = EScenarioType.SCENARIO;
    public int                                  ScenarioId                  = 0;
    public string                               ScenarioDirectorPath        = null;

    public EAcrossType                          AcrossType                  = EAcrossType.ANIMATION;
    public Transform                            AcrossStartPos              = null;
    public eAnimation                           AcrossAni                   = eAnimation.None;
    public string                               AcrossDirectorPath          = "";

    public int                                  ActivePrefabSize            = 0;
    public GameObject[]                         ActivePrefabs;

    public int                                  DeactivePrefabSize          = 0;
    public GameObject[]                         DeactivePrefabs;

    public bool                                 UseCameraSetting            = false;
    public InGameCamera.EMode                   CameraMode                  = InGameCamera.EMode.DEFAULT;
    public InGameCamera.sDefaultSetting         CameraDefaultSetting        = new InGameCamera.sDefaultSetting(InGameCamera.DEFAULT_CAMERA_DISTANCE, InGameCamera.DEFAULT_CAMERA_LOOKAT);
    public InGameCamera.sSideSetting            CameraSideSetting           = new InGameCamera.sSideSetting(new Vector3(0.0f, 1.5f, 4.0f), new Vector2(0.0f, 1.0f), Unit.eAxisType.Z);
    public InGameCamera.sFixedSetting           CameraFixedSetting          = new InGameCamera.sFixedSetting();
    public InGameCamera.sFollowPlayerSetting    CameraFollowPlayerSetting   = new InGameCamera.sFollowPlayerSetting();

    public float JumpWeight = 3.0f;
    public float JumpTime   = 1.0f;

    [System.NonSerialized] 
    public bool PortalEntryEnd = false;

    private WorldStage			mWorldStage         = null;
    private Player				mPlayer             = null;
	private PlayerGuardian      mPlayerGuardian     = null;
	private BattleArea			mBattleArea         = null;
    private PortalExit			mExit               = null;
    private BoxCollider			mBoxCol             = null;
    private Collider[]			mCheckCollisions    = null;
    private bool				mbPlayerOnTrigger   = false;
    private Director			mDirector           = null;
	private WaitForFixedUpdate	mWaitForFixedUpdate = new WaitForFixedUpdate();
	private WaitForEndOfFrame	mWaitForEndOfFrame	= new WaitForEndOfFrame();


	public void Init( BattleArea battleArea, PortalExit portalExit ) {
		mWorldStage = World.Instance as WorldStage;
        mWorldStage.HideHelperMesh();

		mBattleArea = battleArea;
		mExit = portalExit;
		mbPlayerOnTrigger = false;

		mBoxCol = GetComponent<BoxCollider>();
        mPlayer = null;

        System.Text.StringBuilder sb = new System.Text.StringBuilder( AcrossDirectorPath );
		sb.Append( "_" );
		sb.Append( World.Instance.Player.IconName.ToLower() );

		if( AcrossType == EAcrossType.DIRECTOR ) {
			mDirector = GameSupport.CreateDirector( sb.ToString() );
			if( mDirector == null ) {
				Debug.Log( sb.ToString() + " 파일을 찾을 수 없습니다." );
				return;
			}
        }
	}

	private void FixedUpdate() {
		if( mbPlayerOnTrigger ) {
			return;
		}

		mCheckCollisions = Physics.OverlapBox( transform.position, mBoxCol.size * 0.5f, transform.rotation, 1 << (int)eLayer.Player );
		if( mCheckCollisions.Length > 0 ) {
			for( int i = 0; i < mCheckCollisions.Length; i++ ) {
				if( !mCheckCollisions[i].CompareTag( "Player" ) ) {
					continue;
				}

				Player p = mCheckCollisions[i].gameObject.GetComponent<Player>();
				if( p == null ) {
					continue;
				}
                else if( p.IsHelper ) {
                    return;
                }

                mPlayer = p;
				PlayerOnTrigger();
			}
		}
	}

	private void OnTriggerEnter( Collider col ) {
        if( mbPlayerOnTrigger || col.CompareTag( "Player" ) == false ) {
            return;
        }

		Player p = col.GetComponent<Player>();
		if( p == null ) {
			return;
		}
		else if( p.IsHelper ) {
            p.SendStopInputAutoEvent();
			return;
		}

		mPlayer = p;
		PlayerOnTrigger();
	}

	private void PlayerOnTrigger()
    {
        if( mPlayer.IsHelper ) {
            return;
		}

		mPlayerGuardian = mPlayer.Guardian;

		if (mPlayer.UsingUltimateSkill)
        {
            ActionNoahUSkill action = mPlayer.actionSystem.GetAction<ActionNoahUSkill>(eActionCommand.USkill01);
            if (action)
            {
                action.ForceEnd(false);
                mPlayer.SetInitialPosition(transform.position, mPlayer.transform.rotation);
            }
        }

        mPlayer.HideBattleAreaNavigator();
        mPlayer.Input.Pause(true);

        for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
            World.Instance.ListPlayer[i].EnableRayCastCol( false );
            World.Instance.ListPlayer[i].actionSystem.CancelCurrentAction();
        }

        if (UseCameraSetting)
        {
            if (CameraMode == InGameCamera.EMode.DEFAULT)
            {
                World.Instance.InGameCamera.SetMode(CameraMode, CameraDefaultSetting);
            }
            else if (CameraMode == InGameCamera.EMode.SIDE)
            {
                World.Instance.InGameCamera.SetMode(CameraMode, CameraSideSetting, 0.2f);
            }
            else if (CameraMode == InGameCamera.EMode.FIXED)
            {
                World.Instance.InGameCamera.SetMode(CameraMode, CameraFixedSetting);
            }
            else if(CameraMode == InGameCamera.EMode.FOLLOW_PLAYER)
            {
                World.Instance.InGameCamera.SetMode(CameraMode, CameraFollowPlayerSetting);
            }
        }
        else
        {
            World.Instance.InGameCamera.SetFollowPlayerMode(new Vector3(0.0f, 2.0f, -3.0f), new Vector2(0.0f, 0.7f));
        }

        mbPlayerOnTrigger = true;
        PortalEntryEnd = false;

        if(ActivePrefabSize > 0)
        {
            for(int i = 0; i < ActivePrefabSize; i++)
            {
                ActivePrefabs[i].SetActive(true);
            }
        }

        if (DeactivePrefabSize > 0)
        {
            for (int i = 0; i < DeactivePrefabSize; i++)
            {
                DeactivePrefabs[i].SetActive(false);
            }
        }

        mPlayer.PlayAniImmediate(eAnimation.Idle01);

        if(PortalType == EType.NONE)
        {
            TeleportToExitPos();
        }
        else if(PortalType == EType.ACROSS)
        {
            if (AcrossType == EAcrossType.ANIMATION)
            {
                StartCoroutine("AcrossByAni");
            }
            else if(AcrossType == EAcrossType.DIRECTOR)
            {
                PlayAcrossDirector();
            }
        }
        else if(PortalType == EType.SCENARIO)
        {
            if (ScenarioType == EScenarioType.SCENARIO)
            {
                mWorldStage.ShowScenario(ScenarioId, TeleportToExitPos);
            }
            else if(ScenarioType == EScenarioType.DIRECTOR)
            {
                PlayScenarioDirector();
            }
        }
        else if(PortalType == EType.JUMP)
        {
            StartCoroutine("PlayJumpAction");
        }
    }

    private void PlayAcrossDirector()
    {
        mPlayer.SetDieRigidbody();
        mPlayer.aniEvent.StartRootMotion(true);
        mPlayer.EnableBoneFace();

		if ( mPlayerGuardian ) {
			mPlayerGuardian.SetDieRigidbody();
			mPlayerGuardian.aniEvent.StartRootMotion(true);
		}

        mDirector.Init( mPlayer );
        mDirector.SetCallbackOnEnd2( EndAcrossDirector );
        mDirector.Play( true );
    }

    private void EndAcrossDirector()
    {
        mPlayer.aniEvent.EndRootMotion();
        mPlayer.SetGroundedRigidBody();
        mPlayer.EnableDefaultFace();

        mPlayer.ShowMesh( true );
        if ( mPlayerGuardian ) {
			mPlayerGuardian.aniEvent.EndRootMotion();
			mPlayerGuardian.SetGroundedRigidBody();
			mPlayerGuardian.ShowMesh( true );
        }

        if ( mWorldStage ) {
			mWorldStage.RestoreIngameCamera();
		}

        //World.Instance.playerCam.SetControlMode(PlayerCamera.eOtherControlMode.None, 2.0f);
        Invoke("DelayedPortalEntryEnd", 1.2f);
    }

    private void DelayedPortalEntryEnd()
    {
        PortalEntryEnd = true;
    }

    private void TeleportToExitPos()
    {
        if (!mPlayer.isGrounded)
        {
            mPlayer.SetFallingRigidBody();
        }

        mPlayer.transform.position = mExit.transform.position;
        mPlayer.transform.rotation = mExit.transform.rotation;

        if ( mPlayerGuardian ) {
			mPlayerGuardian.transform.position = mExit.transform.position - mPlayer.transform.forward;
			mPlayerGuardian.transform.rotation = mExit.transform.rotation;
		}

        PortalEntryEnd = true;
    }

    private IEnumerator AcrossByAni()
    {
        while (!mPlayer.isGrounded)
        {
            yield return null;
        }

        IsAcrossByAniPlaying = true;
        World.Instance.UIPlay.SetWidgetsAlpha(0.0f, true);

        World.Instance.InGameCamera.SkipCollisionWall = true;
        World.Instance.InGameCamera.TurnToTarget(mPlayer.transform.position + mPlayer.transform.forward, 1.0f, true);

        mPlayer.SetDieRigidbody();
        mPlayer.aniEvent.StartRootMotion(true);

        mPlayer.transform.position = AcrossStartPos.transform.position;
        mPlayer.transform.rotation = AcrossStartPos.transform.rotation;

		GuardianAcrossByAni01();

		//yield return new WaitForEndOfFrame();
		yield return mWaitForEndOfFrame;

        Debug.Log("Start Across Ani : " + AcrossAni);
        float aniLength = mPlayer.PlayAniImmediate(AcrossAni);
        bool end = false;
        float checkTime = 0.0f;

		GuardianAcrossByAni02();

		//WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
		while (!end)
        {
            checkTime += Time.deltaTime;
            if (checkTime >= aniLength)
            {
                end = true;
            }

            yield return mWaitForFixedUpdate;
        }

        mPlayer.aniEvent.EndRootMotion();
        mPlayer.SetGroundedRigidBody();

		GuardianAcrossByAni03();

		World.Instance.InGameCamera.SkipCollisionWall = false;
        World.Instance.InGameCamera.StopTurnToTarget();

        mPlayer.PlayAniImmediate(eAnimation.Idle01);

        GuardianAcrossByAni04();

		PortalEntryEnd = true;
        IsAcrossByAniPlaying = false;

        World.Instance.UIPlay.SetWidgetsAlpha(1.0f, true);
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
        director.SetCallbackOnEnd2(TeleportToExitPos);

        director.Play();
    }

    private IEnumerator PlayJumpAction()
    {
        while (!mPlayer.isGrounded)
        {
            yield return null;
        }

        World.Instance.UIPlay.SetWidgetsAlpha(0.0f, true);

        World.Instance.InGameCamera.SkipCollisionWall = true;
        World.Instance.InGameCamera.TurnToTarget(mPlayer.transform.position + mPlayer.transform.forward, 2.0f, true);

        mPlayer.SetDieRigidbody();
        mPlayer.aniEvent.StartRootMotion(true);        

        mPlayer.transform.position = transform.position;
        mPlayer.LookAtTarget(mExit.transform.position);

		mPlayer.CommandAction(eActionCommand.Jump, new ActionParamAcrossJump(this, mExit.transform.position, JumpTime));

		GuardianPlayJumpAction01();

		while (mPlayer.actionSystem.IsCurrentAction(eActionCommand.Jump))
        {
            yield return null;
        }

        mPlayer.SetGroundedRigidBody();

		World.Instance.InGameCamera.SkipCollisionWall = false;
        World.Instance.InGameCamera.StopTurnToTarget();

        mPlayer.PlayAniImmediate(eAnimation.Idle01);

		GuardianPlayJumpAction02();

		PortalEntryEnd = true;

        World.Instance.UIPlay.SetWidgetsAlpha(1.0f, true);
    }

    private void GuardianAcrossByAni01() {
        if ( mPlayerGuardian == null ) {
            return;
        }

		mPlayerGuardian.actionSystem.CancelCurrentAction();
		mPlayerGuardian.StopBT();
		mPlayerGuardian.SetDieRigidbody();
		mPlayerGuardian.aniEvent.StartRootMotion( true );

		mPlayerGuardian.transform.position = AcrossStartPos.transform.position;
		mPlayerGuardian.transform.rotation = AcrossStartPos.transform.rotation;
	}

	private void GuardianAcrossByAni02() {
		if ( mPlayerGuardian == null ) {
			return;
		}

		mPlayerGuardian.PlayAniImmediate( AcrossAni );
	}

	private void GuardianAcrossByAni03() {
		if ( mPlayerGuardian == null ) {
			return;
		}

		mPlayerGuardian.aniEvent.EndRootMotion();
		mPlayerGuardian.SetGroundedRigidBody();
	}

	private void GuardianAcrossByAni04() {
		if ( mPlayerGuardian == null ) {
			return;
		}

		mPlayerGuardian.PlayAniImmediate( eAnimation.Idle01 );
		mPlayerGuardian.StartBT();
	}

    private void GuardianPlayJumpAction01() {
		if ( mPlayerGuardian == null ) {
			return;
		}

		mPlayerGuardian.actionSystem.CancelCurrentAction();
		mPlayerGuardian.StopBT();
		mPlayerGuardian.SetDieRigidbody();
		mPlayerGuardian.aniEvent.StartRootMotion( true );

		mPlayerGuardian.transform.position = transform.position - mPlayer.transform.forward;
		mPlayerGuardian.LookAtTarget( mExit.transform.position );

		mPlayerGuardian.CommandAction( eActionCommand.Jump, new ActionParamAcrossJump( this, mExit.transform.position - mPlayer.transform.forward, JumpTime ) );
	}

	private void GuardianPlayJumpAction02() {
		if ( mPlayerGuardian == null ) {
			return;
		}

		mPlayerGuardian.SetGroundedRigidBody();
		mPlayerGuardian.PlayAniImmediate( eAnimation.Idle01 );
		mPlayerGuardian.StartBT();
	}
}
