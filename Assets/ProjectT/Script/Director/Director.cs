
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


public class Director : MonoBehaviour
{
    [System.Serializable]
    public class sActorInfo
    {
        public string		trackNumber;
        public string		prefabName;
        public GameObject	parent;
        public bool			isEnemy;

        [System.NonSerialized] public Unit			unit;
        [System.NonSerialized] public AttachObject	attachObj;
        [System.NonSerialized] public Vector3		originalPos;
        [System.NonSerialized] public Quaternion	originalRot;
		[System.NonSerialized] public Vector3		originalAniEventScale;
		[System.NonSerialized] public Transform		originalParent;
		[System.NonSerialized] public int			CloneIndex;
	}

    public class sVirtualCameraInfo
    {
        public Cinemachine.CinemachineVirtualCamera vCam;
        public Vector3 originalVitualCameraLocalPos;
        public Quaternion originalVitualCameraLocalRot;
        public float originalFieldOfView;
        public float PrevFOV;
    }


    public static bool      IsPlaying       = false;
    public static Director  CurrentPlaying  = null;
	public static bool		IsSkipBtnOn		= false;


    [Header("[Play]")]
    public float StartFrame = 0.0f;

    [Header("[Property]")]
    public bool		PauseGameBGM				= false;
    public bool		holdCamPosition				= false;
    public bool		showUIAfterEnd				= true;
    public bool		backToActorOriginalPos		= true;
    public float	clickStartTime				= 0.0f;
    public float	loopStartTime				= 0.0f;
    public float	loopEndTime					= 0.0f;
    public int		showUIAniIndex				= 0;
    public bool		forceQuit					= false;
    public Vector3	startPos					= Vector3.zero;
    public Vector3	startRot					= Vector3.zero;
    public Light	CharLight					= null;
    public bool		moveToParentOnlyAniEvent	= false;

    [Header("[Layer Option]")]
    [Tooltip("모든 자식 오브젝트의 레이어를 강제로 Director로 바꿔줄지 여부")]
    public bool ChangeAllLayerToDirector = false;

    //[Header("BGM")]
    //public AudioClip bgm;

    [Header("[Timeline]")]
    public List<sActorInfo> listActivationActorInfo;
    public List<sActorInfo> listAnimationActorInfo;

    [HideInInspector] public eLayer camLayer;
    //[HideInInspector] public eLayerFlag CamLayerFlag;
    [HideInInspector] public Unit Owner;

    protected PlayableDirector m_playableDirector;
    protected List<sVirtualCameraInfo> m_listVirtualCamInfo = new List<sVirtualCameraInfo>();
    //private sVirtualCameraInfo m_vMainCamInfo = new sVirtualCameraInfo();
    protected List<sActorInfo> m_listActor = new List<sActorInfo>();
    protected Canvas m_canvas;
    protected TimeSpan m_doubleClickCheckTime;
    protected bool m_loopSection = false;
    protected bool m_lockInput = false;
    protected float m_checkTime;
    protected float m_endTime;
    protected Vector3 OwnerStartPos;
    protected Vector3 OwnerStartRot;
    protected Vector3 OwnerScale;
	protected Vector3 OwnerAniEventScale;
    protected float mInGameCameraFOV = 0.0f;
    protected float mInGameCameraNearClip = 0.0f;
    protected float mInGameCameraFarClip = 0.0f;
    protected bool mbSkip = false;
    protected bool mbUseFog = false;

    public PlayableDirector playableDirector { get { return m_playableDirector; } }
    public bool isPause { get; private set; }
    public bool isEnd { get; private set; }
    public int cullingMask { get; set; }

    protected Func<bool, bool> OnStart;
    protected bool m_onStartParam;

    protected Func<bool, bool> OnEnd;
    protected bool m_onEndParam;

    protected Action OnEnd2;
    protected Action OnEnd3;
    protected Action OnHoldCallback;
    protected Action OnSkipBtnCallback;
    protected Action OnEndLoopOnce; // 디렉터가 루프인 경우에 첫번째 끝났을 때 한번만 호출 되는 콜백

    protected UICamera uiCam = null;

	protected WaitForFixedUpdate mWaitForFixedUpdate = new WaitForFixedUpdate();

	protected UICinematicPopup	mCinematicPopup			= null;
	protected bool				mbShowSkipBtn			= false;
	protected float				mCheckShowSkipBtnTime	= 0.0f;


    private void Awake()
    {
        m_playableDirector = GetComponent<PlayableDirector>();
        m_canvas = GetComponentInChildren<Canvas>();
    }

    public virtual void Init(Unit owner)
    {
        Owner = owner;

        if(m_playableDirector == null)
            m_playableDirector = GetComponent<PlayableDirector>();

        m_listVirtualCamInfo.Clear();
        Cinemachine.CinemachineVirtualCamera[] virtualCams = GetComponentsInChildren<Cinemachine.CinemachineVirtualCamera>();
        for(int i = 0; i < virtualCams.Length; i++)
        {
            /*if (virtualCams[i].name.Contains("MainCam") == true)
            {
                m_vMainCamInfo.vCam = virtualCams[i];
                m_vMainCamInfo.vCam.transform.SetParent(null);
                m_vMainCamInfo.vCam.gameObject.SetActive(false);

                continue;
            }*/

            sVirtualCameraInfo virtualCamInfo = new sVirtualCameraInfo();

            virtualCamInfo.vCam = virtualCams[i];
            virtualCamInfo.originalVitualCameraLocalPos = virtualCams[i].transform.localPosition;
            virtualCamInfo.originalVitualCameraLocalRot = virtualCams[i].transform.localRotation;
            virtualCamInfo.originalFieldOfView = virtualCams[i].m_Lens.FieldOfView;

            m_listVirtualCamInfo.Add(virtualCamInfo);
        }

        isPause = false;
        isEnd = false;
        m_lockInput = false;
    }

	public void BindingActors() {
		m_listActor.Clear();

		int animationCloneCount = 0;
		int activationCloneCount = 0;
		int animationUnitWeaponCloneCount = 1;
		int activationUnitWeaponCloneCount = 1;

		IEnumerable<PlayableBinding> playableBinding = m_playableDirector.playableAsset.outputs;
		foreach( PlayableBinding binding in playableBinding ) {
			if( binding.streamName == "Cinemachine Track" ) {
				Cinemachine.CinemachineBrain cinemachineBrain = Camera.main.GetComponent<Cinemachine.CinemachineBrain>();
				if( cinemachineBrain == null ) {
					continue;
				}

				m_playableDirector.SetGenericBinding( binding.sourceObject, cinemachineBrain );
			}
			else if( binding.streamName.Contains( "Animation Track" ) ) {
				sActorInfo actorInfo = null;
				string compare = "";
				for( int i = 0; i < listAnimationActorInfo.Count; i++ ) {
					if( listAnimationActorInfo[i].trackNumber != "0" ) {
						compare = "Animation Track" + listAnimationActorInfo[i].trackNumber.ToString();
					}
					else {
						compare = "Animation Track";
					}

					if( binding.streamName == compare ) {
						actorInfo = listAnimationActorInfo[i];
						break;
					}
				}

				if( actorInfo == null || string.IsNullOrEmpty( actorInfo.prefabName ) ) {
					continue;
				}

				Unit unit = null;
				AttachObject attachObj = null;
				actorInfo.originalParent = null;
				actorInfo.isEnemy = false;
				actorInfo.CloneIndex = -1;

				if( actorInfo.prefabName == "player" ) {
					if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
						unit = null;
					}
					else {
						unit = Owner;

						if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
							Player player = unit as Player;

							if( player && player.costumeUnit ) {
								player.costumeUnit.ChangeAllWeaponName( player.ChangeWeaponName );
								player.aniEvent.Rebind();
							}
						}
					}

					if( unit ) {
						m_playableDirector.SetGenericBinding( binding.sourceObject, unit.aniEvent.gameObject );
					}
				}
				else if( actorInfo.prefabName == "player_clone" ) {
					if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
						unit = null;
					}
					else {
						actorInfo.CloneIndex = animationCloneCount;
						unit = Owner.GetClone( animationCloneCount++ );
					}

					if( unit ) {
						Player player = Owner as Player;

						if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP && player && unit.costumeUnit ) {
							unit.costumeUnit.ChangeAllWeaponName( player.ChangeWeaponName );
							unit.aniEvent.Rebind();
						}

						m_playableDirector.SetGenericBinding( binding.sourceObject, unit.aniEvent.gameObject );
					}
				}
				else if( actorInfo.prefabName == "player_face" ) {
					unit = Owner;

					if( unit && unit.aniEvent.aniFace ) {
						m_playableDirector.SetGenericBinding( binding.sourceObject, unit.aniEvent.aniFace.gameObject );
					}
				}
				else if( actorInfo.prefabName == "weapon_r" ) {
					Player player = Owner as Player;
					if( player ) {
						attachObj = player.GetCurrentRWeaponOrNull();
					}

					if( attachObj ) {
						actorInfo.originalParent = attachObj.transform.parent;

						Animator animator = attachObj.GetComponentInChildren<Animator>();
						if( animator ) {
							m_playableDirector.SetGenericBinding( binding.sourceObject, animator.gameObject );
						}
						else {
							m_playableDirector.SetGenericBinding( binding.sourceObject, attachObj.gameObject );
						}
					}
				}
				else if( actorInfo.prefabName == "weapon_l" ) {
					Player player = Owner as Player;
					if( player ) {
						attachObj = player.GetCurrentLWeaponOrNull();
					}

					if( attachObj ) {
						actorInfo.originalParent = attachObj.transform.parent;
						m_playableDirector.SetGenericBinding( binding.sourceObject, attachObj.gameObject );
					}
				}
				else if( actorInfo.prefabName == "unit_weapon" ) {
					Player player = Owner as Player;
					if( player ) {
						unit = player.GetCurrentUnitWeaponCloneOrNull( 0 );
					}

					if( unit && !unit.withoutAniEvent ) {
						if( !unit.IsActivate() ) {
							unit.Activate();
						}

						unit.actionSystem.CancelCurrentAction();
						unit.PlayAniImmediate( eAnimation.Idle01 );

						Utility.InitTransform( unit.aniEvent.gameObject, true );						
						m_playableDirector.SetGenericBinding( binding.sourceObject, unit.aniEvent.gameObject );
					}
				}
				else if( actorInfo.prefabName == "unit_weapon_clone" ) {
					Player player = Owner as Player;
					if( player ) {
						unit = player.GetCurrentUnitWeaponCloneOrNull( animationUnitWeaponCloneCount++ );
					}

					if( unit && !unit.withoutAniEvent ) {
						if( !unit.IsActivate() ) {
							unit.Activate();
						}

						m_playableDirector.SetGenericBinding( binding.sourceObject, unit.aniEvent.gameObject );
					}
				}
				else if( actorInfo.prefabName == "player_acc" ) {
					if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
						unit = null;
					}
					else {
						unit = Owner;
					}

					if( unit.aniEvent.AniAcc ) {
						m_playableDirector.SetGenericBinding( binding.sourceObject, unit.aniEvent.AniAcc.gameObject );
					}
				}
				else {
					if( World.Instance.StageType != eSTAGETYPE.STAGE_SPECIAL ) {
						if( Owner ) {
							string name = Owner.name.ToLower();

							if( name == ( actorInfo.prefabName.ToLower() + "(clone)" ) ) {
								unit = Owner;
								Utility.SetLayer( unit.gameObject, (int)eLayer.Default, true, (int)eLayer.Wall );

								unit.ShowShadow( false );
								m_playableDirector.SetGenericBinding( binding.sourceObject, unit.aniEvent.gameObject );

								actorInfo.isEnemy = true;
							}
							else if( name == actorInfo.prefabName.ToLower().Replace( "_face", "" ) + "(clone)" ) {
								unit = Owner;

								if( unit.aniEvent.aniFace ) {
									m_playableDirector.SetGenericBinding( binding.sourceObject, unit.aniEvent.aniFace.gameObject );
								}
							}
						}

						if( unit == null ) {
							for( int i = 0; i < World.Instance.EnemyMgr.listEnemy.Count; i++ ) {
								name = World.Instance.EnemyMgr.listEnemy[i].name.ToLower();

								if( name == ( actorInfo.prefabName.ToLower() + "(clone)" ) ) {
									unit = World.Instance.EnemyMgr.listEnemy[i];
									Utility.SetLayer( unit.gameObject, (int)eLayer.Default, true, (int)eLayer.Wall );

									unit.ShowShadow( false );
									m_playableDirector.SetGenericBinding( binding.sourceObject, unit.aniEvent.gameObject );

									actorInfo.isEnemy = true;
									break;
								}
								else if( name == actorInfo.prefabName.ToLower().Replace( "_face", "" ) + "(clone)" ) {
									unit = World.Instance.EnemyMgr.listEnemy[i];
									if( unit.aniEvent.aniFace ) {
										m_playableDirector.SetGenericBinding( binding.sourceObject, unit.aniEvent.aniFace.gameObject );
										break;
									}
								}
							}
						}
					}
				}

				actorInfo.unit = unit;
				actorInfo.attachObj = attachObj;

				m_listActor.Add( actorInfo );
			}
			else if( binding.streamName.Contains( "Activation Track" ) == true ) {
				sActorInfo actorInfo = null;
				string compare = "";

				for( int i = 0; i < listActivationActorInfo.Count; i++ ) {
					if( listActivationActorInfo[i].trackNumber != "0" ) {
						compare = "Activation Track" + listActivationActorInfo[i].trackNumber.ToString();
					}
					else {
						compare = "Activation Track";
					}

					if( binding.streamName == compare ) {
						actorInfo = listActivationActorInfo[i];
						break;
					}
				}

				if( actorInfo == null || string.IsNullOrEmpty( actorInfo.prefabName ) ) {
					continue;
				}

				Unit unit = null;
				AttachObject attachObj = null;

				actorInfo.originalParent = null;
				actorInfo.isEnemy = false;

				if( actorInfo.prefabName == "player" ) {
					if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
						unit = null;
					}
					else {
						unit = Owner;
					}
				}
				else if( actorInfo.prefabName == "player_clone" ) {
					if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
						unit = null;
					}
					else {
						actorInfo.CloneIndex = animationCloneCount;
						unit = Owner.GetClone( activationCloneCount++ );
					}
				}
				else if( actorInfo.prefabName == "weapon_r" ) {
					if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
						attachObj = null;
					}
					else {
						Player player = Owner as Player;
						if( player ) {
							attachObj = player.GetCurrentRWeaponOrNull();
						}
					}
				}
				else if( actorInfo.prefabName == "weapon_l" ) {
					if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
						attachObj = null;
					}
					else {
						Player player = Owner as Player;
						if( player ) {
							attachObj = player.GetCurrentLWeaponOrNull();
						}
					}
				}
				else if( actorInfo.prefabName == "unit_weapon" ) {
					if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
						unit = null;
					}
					else {
						Player player = Owner as Player;
						if( player ) {
							unit = player.GetCurrentUnitWeaponCloneOrNull( activationUnitWeaponCloneCount++ );
						}
					}

					if( unit && !unit.IsActivate() ) {
						unit.Activate();
					}
				}
				else if( actorInfo.prefabName == "unit_weapon_clone" ) {
					if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
						unit = null;
					}
					else {
						Player player = Owner as Player;
						if( player ) {
							unit = player.GetCurrentUnitWeaponCloneOrNull( activationUnitWeaponCloneCount++ );
						}
					}

					if( unit && !unit.IsActivate() ) {
						unit.Activate();
					}
				}
				else {
					if( Owner ) {
						string name = Owner.name.ToLower();
						name = name.Replace( "_aos", "" );

						if( name == ( actorInfo.prefabName.ToLower() + "(clone)" ) ) {
							unit = Owner;
							Utility.SetLayer( unit.gameObject, (int)eLayer.Default, true, (int)eLayer.Wall );

							actorInfo.isEnemy = true;
						}
					}

					if( unit == null ) {
						for( int i = 0; i < World.Instance.EnemyMgr.listEnemy.Count; i++ ) {
							name = World.Instance.EnemyMgr.listEnemy[i].name.ToLower();
							name = name.Replace( "_aos", "" );

							if( name == ( actorInfo.prefabName.ToLower() + "(clone)" ) ) {
								unit = World.Instance.EnemyMgr.listEnemy[i];
								Utility.SetLayer( unit.gameObject, (int)eLayer.Default, true, (int)eLayer.Wall );

								actorInfo.isEnemy = true;
								break;
							}
						}
					}
				}

				if( unit ) {
					m_playableDirector.SetGenericBinding( binding.sourceObject, unit.gameObject );
				}
				else if( attachObj ) {
					actorInfo.originalParent = attachObj.transform.parent;
					m_playableDirector.SetGenericBinding( binding.sourceObject, attachObj.gameObject );
				}

				actorInfo.unit = unit;
				actorInfo.attachObj = attachObj;

				m_listActor.Add( actorInfo );
			}
			else if( AppMgr.Instance.SceneType != AppMgr.eSceneType.Title ) {
				AudioTrack audioTrack = binding.sourceObject as AudioTrack;
				if( audioTrack ) {
					bool isBgm = false;
					foreach( TimelineClip clip in audioTrack.GetClips() ) {
						AudioClip audioClip = ((AudioPlayableAsset)clip.asset).clip;
						if( audioClip ) {
							if( audioClip.loadType == AudioClipLoadType.CompressedInMemory ) {
								isBgm = true;
								break;
							}
						}
					}

					if( isBgm ) {
						m_playableDirector.SetGenericBinding( binding.sourceObject, SoundManager.Instance.DirectorBGMSrc );
					}
					else {
						m_playableDirector.SetGenericBinding( binding.sourceObject, SoundManager.Instance.DirectorFXSrc );
					}
				}
			}
		}
	}

	public void SetCallbackOnStart(Func<bool, bool> CallbackOnStart, bool param)
    {
        OnStart = CallbackOnStart;
        m_onStartParam = param;
    }

    public void SetCallbackOnEnd(Func<bool, bool> CallbackOnEnd, bool param)
    {
        OnEnd = CallbackOnEnd;
        m_onEndParam = param;
    }

    public void SetCallbackOnEnd2(Action CallbackOnEnd2)
    {
        OnEnd2 = CallbackOnEnd2;
    }

    public void SetCallbackOnEnd3(Action CallbackOnEnd3)
    {
        OnEnd3 = CallbackOnEnd3;
    }

    public void SetHoldCallbackOnHoldCallback(Action CallbackOnHoldCallback)
    {
        OnHoldCallback = CallbackOnHoldCallback;
    }

    public void SetSkipBtnCallback(Action callback)
    {
        OnSkipBtnCallback = callback;
    }

    public void SetCallbackOnEndLoopOnce(Action callbackOnEndLoopOnce)
    {
        OnEndLoopOnce = callbackOnEndLoopOnce;
    }

	public virtual void Play( bool startPosIsZero = false ) {
        if( m_playableDirector.state == PlayState.Playing ) {
            return;
        }

		if( Owner && Owner as Player ) {
			cullingMask ^= ( 1 << (int)eLayer.PlayerClone );
		}

		Debug.Log( gameObject.name + " 연출 시작" );

        if( m_playableDirector.extrapolationMode == DirectorWrapMode.None ) {
			IsPlaying = true;
			CurrentPlaying = this;
		}

		if( !gameObject.activeSelf )
			gameObject.SetActive( true );

		/*
		if( Camera.main == null ) {
			World.Instance.InGameCamera.MainCamera.enabled = true;
		}
		*/

		for( int i = 0; i < m_listVirtualCamInfo.Count; i++ ) {
			m_listVirtualCamInfo[i].vCam.m_Lens.FieldOfView *= AppMgr.Instance.FieldOfViewRatio;
			m_listVirtualCamInfo[i].PrevFOV = m_listVirtualCamInfo[i].vCam.m_Lens.FieldOfView;
		}

        Camera.main.useOcclusionCulling = false;

        mbUseFog = false;
		if( RenderSettings.fog ) {
			mbUseFog = true;
			RenderSettings.fog = false;
		}

        isPause = false;
        isEnd = false;
        mbSkip = false;
		m_lockInput = false;
		m_doubleClickCheckTime = TimeSpan.Zero;
		m_checkTime = 0.0f;
		m_endTime = (float)m_playableDirector.duration;

		m_loopSection = false;
        if( loopEndTime > 0.0f ) {
            m_loopSection = true;
        }

		if( World.Instance.InGameCamera ) {
			World.Instance.InGameCamera.ActivePlayerLight( false );
			World.Instance.InGameCamera.EnableCamera( true );
			World.Instance.InGameCamera.ResetPostProcess();
			World.Instance.InGameCamera.SetCullingMask( (int)camLayer );

			mInGameCameraFOV = World.Instance.InGameCamera.MainCamera.fieldOfView;
			mInGameCameraNearClip = Mathf.Max( 0.1f, World.Instance.InGameCamera.MainCamera.nearClipPlane );
			mInGameCameraFarClip = World.Instance.InGameCamera.MainCamera.farClipPlane;
		}

		if( AppMgr.Instance.SceneType != AppMgr.eSceneType.Title && AppMgr.Instance.SceneType != AppMgr.eSceneType.Lobby ) {
			if ( mCinematicPopup == null ) {
				mCinematicPopup = GameUIManager.Instance.ShowUI( "CinematicPopup", false ) as UICinematicPopup;
			}

			if ( PauseGameBGM ) {
				SoundManager.Instance.PauseBgm();
			}

			World.Instance.UIPlay.m_screenEffect.ClearSkillNameQueue();
			World.Instance.ProjectileMgr.DestroyAllProjectile( true );
			World.Instance.Pause( true, false, Owner );

			for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
				if( i == 0 ) {
					World.Instance.ListPlayer[0].Input.LockPause = true;
				}

				World.Instance.ListPlayer[i].SetMainTarget( null );

				if( Owner != World.Instance.ListPlayer[i]) {
					Utility.SetLayer( World.Instance.ListPlayer[i].gameObject, (int)eLayer.EnemyClone, true, (int)eLayer.PostProcess );

					if ( World.Instance.ListPlayer[i].Guardian ) {
						Utility.SetLayer( World.Instance.ListPlayer[i].Guardian.gameObject, (int)eLayer.EnemyClone, true, (int)eLayer.PostProcess );
					}
				}
				else if ( Owner is Player ) {
					if ( World.Instance.ListPlayer[i].Guardian ) {
						Utility.SetLayer( World.Instance.ListPlayer[i].Guardian.gameObject, (int)eLayer.Player, true, (int)eLayer.PostProcess );
					}
				}
			}

			GameUIManager.Instance.HideUI( "GamePlayPanel", false );

			if( World.Instance.StageType == eSTAGETYPE.STAGE_SPECIAL ) {
				GameUIManager.Instance.HideUI( "BikeRaceModePanel", false );
			}
			else if( World.Instance.StageType == eSTAGETYPE.STAGE_TRAINING ) {
				GameUIManager.Instance.HideUI( "SkillTrainingPanel", false );
			}
			else if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
				GameUIManager.Instance.HideUI( "ArenaPlayPanel", false );
			}
		}

		BindingActors();
		gameObject.SetActive( true );

		for( int i = 0; i < m_listActor.Count; i++ ) {
			sActorInfo actorInfo = m_listActor[i];
            if( actorInfo == null ) {
                continue;
            }

			actorInfo.originalAniEventScale = Vector3.one;

			if ( actorInfo.parent ) {
				if( actorInfo.unit ) {
					actorInfo.originalPos = actorInfo.unit.transform.position;
					actorInfo.originalRot = actorInfo.unit.transform.rotation;

					actorInfo.originalAniEventScale = actorInfo.unit.aniEvent ? actorInfo.unit.aniEvent.transform.localScale : Vector3.one;
				}
				else if( actorInfo.attachObj ) {
					actorInfo.originalPos = actorInfo.attachObj.transform.position;
					actorInfo.originalRot = actorInfo.attachObj.transform.rotation;
				}
			}
		}

		if( Owner != null ) {
			OwnerScale = Owner.transform.localScale;
			OwnerAniEventScale = Owner.aniEvent ? Owner.aniEvent.transform.localScale : Vector3.one;

			OwnerStartPos = Owner.transform.position;
			OwnerStartRot = Owner.transform.rotation.eulerAngles;

			if( startPos != Vector3.zero ) {
				transform.position = startPos;

				if( startRot != Vector3.zero ) {
					transform.rotation = Quaternion.Euler( startRot );
				}
			}
			else if( !startPosIsZero ) {
				if( Owner.isGrounded ) {
					transform.position = Owner.transform.position;
				}
				else {
					transform.position = new Vector3( Owner.transform.position.x, Owner.posOnGround.y, Owner.transform.position.z );
				}

				transform.rotation = Owner.transform.rotation;
			}

			Owner.transform.SetParent( transform );
			Utility.InitTransform( Owner.gameObject );
		}

		for( int i = 0; i < m_listActor.Count; i++ ) {
			sActorInfo actorInfo = m_listActor[i];
            if( actorInfo == null ) {
                continue;
            }

			if( actorInfo.unit ) {
				actorInfo.unit.SetKinematicRigidBody();
				actorInfo.unit.IgnoreCollision();
			}

			if( actorInfo.parent ) {
				if( moveToParentOnlyAniEvent && actorInfo.unit && actorInfo.unit.aniEvent ) {
					actorInfo.unit.aniEvent.transform.SetParent( actorInfo.parent.transform );
					Utility.InitTransform( actorInfo.unit.aniEvent.gameObject );
				}
				else {
					if( actorInfo.unit ) {
						actorInfo.unit.transform.SetParent( actorInfo.parent.transform );
						Utility.InitTransform( actorInfo.unit.gameObject );
					}
					else if( actorInfo.attachObj ) {
						actorInfo.attachObj.transform.SetParent( actorInfo.parent.transform );
						Utility.InitTransform( actorInfo.attachObj.gameObject );
					}
				}
			}
		}

		if( ChangeAllLayerToDirector ) {
			Utility.SetLayer( gameObject, (int)eLayer.Director, true );
		}

		if( OnStart != null )
			OnStart( m_onStartParam );

		if( StartFrame > 0.0f ) {
			SetTime( StartFrame / 60.0f );
		}

		mbShowSkipBtn = false;
		mCheckShowSkipBtnTime = 0.0f;
		IsSkipBtnOn = false;

		StartCoroutine( "EndDirector" );
	}

	protected virtual void Update()
    {
		if ( !m_lockInput ) {
			if ( AppMgr.Instance.CustomInput.GetButtonUp( BaseCustomInput.eKeyKind.Select ) ) {
				if ( m_loopSection && /*m_checkTime*/m_playableDirector.time >= loopStartTime ) {
					m_playableDirector.time = loopEndTime;
					m_loopSection = false;
				}

				if ( forceQuit ) {
					ForceQuit();
				}
			}
			else if ( AppMgr.Instance.CustomInput.GetButtonUp( BaseCustomInput.eKeyKind.Pause ) && IsCancelDirector() ) {
				ForceQuit( true );
			}
		}
	}

    private void LateUpdate()
    {
        if (m_playableDirector.state != PlayState.Playing)
        {
			for (int i = 0; i < m_listVirtualCamInfo.Count; i++)
			{
				if (m_listVirtualCamInfo[i].vCam.m_Lens.FieldOfView != m_listVirtualCamInfo[i].PrevFOV)
				{
					m_listVirtualCamInfo[i].vCam.m_Lens.FieldOfView *= AppMgr.Instance.FieldOfViewRatio;
					m_listVirtualCamInfo[i].PrevFOV = m_listVirtualCamInfo[i].vCam.m_Lens.FieldOfView;
				}
			}

			return;
        }

        for (int i = 0; i < m_listVirtualCamInfo.Count; i++)
        {
            if (m_listVirtualCamInfo[i].vCam.m_Lens.FieldOfView != m_listVirtualCamInfo[i].PrevFOV)
            {
                m_listVirtualCamInfo[i].vCam.m_Lens.FieldOfView *= AppMgr.Instance.FieldOfViewRatio;
                m_listVirtualCamInfo[i].PrevFOV = m_listVirtualCamInfo[i].vCam.m_Lens.FieldOfView;
            }
        }
    }

    public void ForceQuit(bool isSkipBtn = false)
    {
        mbSkip = isSkipBtn;

		m_playableDirector.time = m_playableDirector.playableAsset.duration;
		m_playableDirector.Evaluate();
        m_playableDirector.Stop();

        StopCoroutine("EndDirector");
        End();
    }

	protected virtual IEnumerator EndDirector() {
		bool checkEnd = false;
		bool end = false;

		bool isFirstDirector = name.Contains( "drt_cutscene_stage_1-1" );

		while ( !end ) {
			if ( AppMgr.Instance.SceneType != AppMgr.eSceneType.Title && AppMgr.Instance.SceneType != AppMgr.eSceneType.Lobby ) {
				if ( !isFirstDirector && mCinematicPopup && !World.Instance.IsEndGame && !World.Instance.ProcessingEnd ) {
					mCheckShowSkipBtnTime += Time.fixedDeltaTime;

					if ( !mbShowSkipBtn && mCheckShowSkipBtnTime > 1.0f ) {
						mCinematicPopup.ShowSkipButton( this );
						IsSkipBtnOn = true;

						if ( FSaveData.Instance.SkipDirector ) {
							mCinematicPopup.OnBtnSkip();
						}
						else {
							mbShowSkipBtn = true;
						}
					}
				}
			}

			//190906
			//Hold 일때 콜백함수 타는 부분으로 넘어가지 않아서 임시로 변경
			if ( !checkEnd && m_playableDirector.time > 0 ) { // 디렉터가 끝나면 Time이 0이됨.
				checkEnd = true;
			}
			else if ( checkEnd && ( m_playableDirector.time <= 0 || (float)m_playableDirector.time > m_endTime ) ) {
				if ( !isEnd ) {
					End();
				}

				end = true;
			}
			else if ( checkEnd && ( m_playableDirector.time <= 0 || (float)m_playableDirector.time >= m_endTime ) && OnHoldCallback != null ) {
				if ( !isEnd ) {
					End();
				}

				end = true;
			}

			if ( !end ) {
				if ( m_loopSection ) {
					if ( m_playableDirector.time >= loopEndTime ) {
						if ( OnEndLoopOnce != null ) {
							OnEndLoopOnce();
							OnEndLoopOnce = null;
						}

						m_playableDirector.time = loopStartTime;
					}
				}

				// 어디서 y축을 돌리는줄 모르겠네!!!
				if ( Owner && Owner.transform.localEulerAngles.y > 0.0f ) { 
					Owner.transform.localRotation = Quaternion.identity;
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public void SetLoop(float startTime, float endTime)
    {
        m_loopSection = true;

        loopStartTime = startTime;
        loopEndTime = endTime;
    }

    public void LockInput(bool lockInput)
    {
        m_lockInput = lockInput;
    }

    private Canvas mCanvas = null;
    public void Pause()
    {
        if (m_playableDirector == null)
            return;

        mCanvas = GetComponentInChildren<Canvas>();
        if (mCanvas)
        {
            mCanvas.gameObject.SetActive(false);
        }

        isPause = true;
        //m_playableDirector.Pause();
        m_playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }

    public void Resume()
    {
        if (m_playableDirector == null)
            return;

        if (mCanvas)
        {
            mCanvas.gameObject.SetActive(true);
        }

        isPause = false;
        //m_playableDirector.Resume();
        m_playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

	public void EndLoop() {
		if( !m_loopSection ) {
			return;
		}

		m_playableDirector.time = loopEndTime;
		m_loopSection = false;
	}

	public void End() {
		if ( isEnd ) {
			return;
		}

		IsPlaying = false;
		CurrentPlaying = null;

		if ( Owner != null ) {
			Owner.transform.SetParent( null );
			Owner.transform.localScale = OwnerScale;
			
			if ( ( Owner is Player ) && Owner.aniEvent ) {
				Owner.aniEvent.transform.localScale = OwnerAniEventScale;
			}

			if ( startPos != Vector3.zero ) {
				Owner.transform.position = OwnerStartPos;
				Owner.transform.rotation = Quaternion.Euler( OwnerStartRot );
			}

			Player ownerPlayer = Owner as Player;
			if ( ownerPlayer ) {
				ownerPlayer.HideCurrentUnitWeaponClone();
			}

			Aina aina = Owner as Aina;
			if ( aina ) {
				aina.RestoreEyePatch();
			}
		}

		if ( AppMgr.Instance.IsInGameScene() && World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			AppMgr.Instance.CustomInput.ShowCursor( false );

			if ( mCinematicPopup ) {
				mCinematicPopup.HideSkipButton();
			}
		}

		if ( Camera.main ) {
			Camera.main.useOcclusionCulling = true;
		}

		if ( mbUseFog ) {
			RenderSettings.fog = true;
		}

		// 인게임 카메라에서만 필요한 위치,회전 초기화
		if ( Camera.main && AppMgr.Instance.SceneType != AppMgr.eSceneType.Title && AppMgr.Instance.SceneType != AppMgr.eSceneType.Lobby ) {
			Camera.main.transform.localPosition = Vector3.zero;
			Camera.main.transform.localRotation = Quaternion.identity;
		}

		if ( holdCamPosition ) {
			sVirtualCameraInfo vCamInfo = m_listVirtualCamInfo[m_listVirtualCamInfo.Count - 1];

			if ( vCamInfo != null && vCamInfo.vCam ) {
				World.Instance.InGameCamera.SetFixedMode( vCamInfo.vCam.transform.position,
														  vCamInfo.vCam.transform.rotation.eulerAngles,
														  vCamInfo.vCam.m_Lens.FieldOfView,
														  true );
			}
		}
		else {
			for ( int i = 0; i < m_listVirtualCamInfo.Count; i++ ) {
				if ( m_listVirtualCamInfo[i] == null || m_listVirtualCamInfo[i].vCam == null ) {
					continue;
				}

				m_listVirtualCamInfo[i].vCam.transform.localPosition = m_listVirtualCamInfo[i].originalVitualCameraLocalPos;
				m_listVirtualCamInfo[i].vCam.transform.localRotation = m_listVirtualCamInfo[i].originalVitualCameraLocalRot;
				m_listVirtualCamInfo[i].vCam.m_Lens.FieldOfView = m_listVirtualCamInfo[i].originalFieldOfView;
			}
		}

		for ( int i = 0; i < m_listActor.Count; i++ ) {
			sActorInfo actorInfo = m_listActor[i];
			if ( actorInfo == null ) {
				continue;
			}

			if ( AppMgr.Instance.SceneType != AppMgr.eSceneType.Lobby && AppMgr.Instance.SceneType != AppMgr.eSceneType.Title && !World.Instance.IsEndGame ) {
				if ( actorInfo.unit ) {
					actorInfo.unit.SetGroundedRigidBody();
					actorInfo.unit.RestoreCollision();
				}
			}

			if ( actorInfo.unit && actorInfo.isEnemy ) {
				actorInfo.unit.ShowShadow( true );
				Utility.SetLayer( actorInfo.unit.gameObject, (int)eLayer.Enemy, true, (int)eLayer.Wall );
			}
			else if( actorInfo.unit && actorInfo.CloneIndex >= 0 ) {
				Owner.HideClone( actorInfo.CloneIndex );
			}

			if ( actorInfo.unit && actorInfo.parent ) {
				if ( actorInfo.unit && !actorInfo.unit.isClone ) {
					actorInfo.unit.gameObject.SetActive( true );
				}

				if ( moveToParentOnlyAniEvent && actorInfo.unit && actorInfo.unit.aniEvent ) {
					actorInfo.unit.aniEvent.transform.SetParent( actorInfo.unit.transform );
					Utility.InitTransform( actorInfo.unit.aniEvent.gameObject, Vector3.zero, Quaternion.identity, actorInfo.originalAniEventScale );
				}
				else {
					if ( actorInfo.unit ) {
						actorInfo.unit.transform.SetParent( null );
					}
					else if ( actorInfo.attachObj ) {
						actorInfo.attachObj.transform.SetParent( actorInfo.originalParent );
					}
				}

				if ( backToActorOriginalPos ) {
					if ( actorInfo.unit == null && actorInfo.attachObj == null ) {
						Owner.transform.position = OwnerStartPos;
						Owner.transform.rotation = Quaternion.Euler( OwnerStartRot );
					}
					else {
						if ( actorInfo.unit ) {
							Vector3 initPos = actorInfo.originalPos;
							Quaternion initRot = actorInfo.originalRot;
							if ( actorInfo.unit is PlayerGuardian ) {
								initPos = actorInfo.parent.transform.position;
								initRot = actorInfo.parent.transform.rotation;
							}
							actorInfo.unit.SetInitialPositionWithoutScale( initPos, initRot );
						}
						else if ( actorInfo.attachObj ) {
							actorInfo.attachObj.transform.SetPositionAndRotation( actorInfo.originalPos, actorInfo.originalRot );
						}
					}
				}
			}
		}

		if ( m_playableDirector && m_playableDirector.extrapolationMode == DirectorWrapMode.None ) {
			gameObject.SetActive( false );

			if ( AppMgr.Instance.IsInGameScene() ) {
				World.Instance.InGameCamera.ActivePlayerLight( true );
			}
		}

		if ( AppMgr.Instance.IsInGameScene() ) {
			for ( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
				if ( World.Instance.ListPlayer[i] == null ) {
					continue;
				}

				if ( World.Instance.StageType != eSTAGETYPE.STAGE_TOWER ) {
					if ( !World.Instance.ListPlayer[i].gameObject.activeSelf && World.Instance.ListPlayer[i].curHp > 0.0f ) {
						World.Instance.ListPlayer[i].gameObject.SetActive( true );
					}
				}

				if ( Owner != World.Instance.ListPlayer[i] ) {
					Utility.SetLayer( World.Instance.ListPlayer[i].gameObject, (int)eLayer.Player, true, (int)eLayer.PostProcess );
				}

				if ( World.Instance.ListPlayer[i].Guardian ) {
					Utility.SetLayer( World.Instance.ListPlayer[i].Guardian.gameObject, (int)eLayer.PlayerClone, true, (int)eLayer.PostProcess );
				}
			}

			if ( PauseGameBGM ) {
				SoundManager.Instance.ResumeBgm();
			}

			if ( World.Instance.InGameCamera && World.Instance.InGameCamera.MainCamera ) {
				if ( !holdCamPosition ) {
					World.Instance.InGameCamera.MainCamera.fieldOfView = mInGameCameraFOV;
					World.Instance.InGameCamera.MainCamera.nearClipPlane = Mathf.Max( 0.1f, mInGameCameraNearClip );
					World.Instance.InGameCamera.MainCamera.farClipPlane = mInGameCameraFarClip;
				}

				World.Instance.InGameCamera.RestoreCullingMask();
			}

			if ( World.Instance.Player ) {
				World.Instance.Player.Input.LockPause = false;
			}

			if ( showUIAfterEnd == true ) {
				if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
					World.Instance.ShowGamePlayUI( showUIAniIndex, Owner );
				}
				else {
					GameUIManager.Instance.ShowUI( "ArenaPlayPanel", false );
					World.Instance.Pause( false );
				}
			}
			else
				World.Instance.Pause( false );

			if ( World.Instance.StageType == eSTAGETYPE.STAGE_TRAINING ) {
				GameUIManager.Instance.ShowUI( "SkillTrainingPanel", false );
			}
		}

		isEnd = true;

		OnEnd?.Invoke( m_onEndParam );
		OnEnd2?.Invoke();
		OnEnd3?.Invoke();

		OnHoldCallback?.Invoke();
		OnHoldCallback = null;

		if ( mbSkip ) {
			OnSkipBtnCallback?.Invoke();
		}

		Debug.Log( "End Director" );
	}

	public float GetDuration()
    {
        if (m_playableDirector == null)
            return 0.0f;

        return (float)m_playableDirector.duration;
    }

    public float GetTime()
    {
        if (m_playableDirector == null)
            return 0.0f;

        return (float)m_playableDirector.time;
    }

    public void SetTime(float time)
    {
        if (m_playableDirector == null || time < m_playableDirector.time)
        {
            return;
        }

        m_playableDirector.time = time;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ActiveCanvas(bool active)
    {
        m_canvas.gameObject.SetActive(active);
    }

    public Unit GetUnitOrNullByName(string name)
    {
        for(int i = 0; i < m_listActor.Count; i++)
        {
            if(m_listActor[i].unit == null)
            {
                continue;
            }

            if(m_listActor[i].prefabName == name)
            {
                return m_listActor[i].unit;
            }
        }

        return null;
    }

	private bool IsCancelDirector() {
		if ( World.Instance.IsEndGame || World.Instance.ProcessingEnd || isPause ) {
			return false;
		}

		UICinematicPopup cinematicPopup = GameUIManager.Instance.GetActiveUI<UICinematicPopup>( "CinematicPopup" );
		if ( cinematicPopup != null && cinematicPopup.IsActiveSkipBtn() ) {
			/*
			if ( IsPlayingDirectorFromOwner( "USkill01" ) ) {
				return true;
			}

			if ( IsPlayingDirectorFromOwner( "BossAppear" ) ) {
				return true;
			}
			*/

			return true;
		}

		return false;
	}

	private bool IsPlayingDirectorFromOwner( string key ) {
		if ( Owner == null ) {
			return false;
		}

		Director director = Owner.GetDirector( key );
		if ( director == null ) {
			return false;
		}

		return director == CurrentPlaying;
	}
}
