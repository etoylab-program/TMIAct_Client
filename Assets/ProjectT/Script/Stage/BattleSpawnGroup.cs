
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleSpawnGroup : MonoBehaviour
{
    public enum eSpawnTimingType
    {
        Hp = 0,
        Deactivate,
    }


    [Header("[Property]")]
    public eSpawnTimingType spawnTimingType = eSpawnTimingType.Hp;
    public List<BattleSpawnPoint> kSpawnPointList = new List<BattleSpawnPoint>();

    [Header("[Camera Setting]")]
    [HideInInspector] public bool                               UseCameraSetting            = false;
    [HideInInspector] public InGameCamera.EMode                 CameraMode                  = InGameCamera.EMode.DEFAULT;
    [HideInInspector] public InGameCamera.sDefaultSetting       CameraDefaultSetting        = new InGameCamera.sDefaultSetting(InGameCamera.DEFAULT_CAMERA_DISTANCE, InGameCamera.DEFAULT_CAMERA_LOOKAT);
    [HideInInspector] public InGameCamera.sSideSetting          CameraSideSetting           = new InGameCamera.sSideSetting(new Vector3(0.0f, 1.5f, 4.0f), new Vector2(0.0f, 1.0f), Unit.eAxisType.Z);
    [HideInInspector] public InGameCamera.sFixedSetting         CameraFixedSetting          = new InGameCamera.sFixedSetting();
    [HideInInspector] public InGameCamera.sFollowPlayerSetting  CameraFollowPlayerSetting   = new InGameCamera.sFollowPlayerSetting();

	public Dictionary<int, int>	dicMonsterInfo	{ get { return m_dicMonsterInfo; } }
	public int					monsterCount	{ get; private set; }

	private WorldStage				mWorldStage = null;
    private BattleArea				mBattleArea = null;
    private bool					m_waiting = false;
    private Dictionary<int, int>	m_dicMonsterInfo = new Dictionary<int, int>();
    private Director				mBossAppearDirector = null;
	private WaitForFixedUpdate		mWaitForFixedUpdate = new WaitForFixedUpdate();


    public void Init(BattleArea battleArea)
    {
        mWorldStage = World.Instance as WorldStage;
        mBattleArea = battleArea;

        BattleSpawnPoint bossPoint = null;
        int bossPointIndex = -1;
        for (int i = 0; i < kSpawnPointList.Count; i++)
        {
            GameClientTable.Monster.Param param = GameInfo.Instance.GameClientTable.FindMonster(kSpawnPointList[i].kMonsterID);
            if (param == null)
            {
                Debug.LogError(kSpawnPointList[i].kMonsterID + "는 없는 아이디입니다.");
                continue;
            }

            if (!string.IsNullOrEmpty(param.BossAppear))
            {
                bossPoint = kSpawnPointList[i];
                bossPointIndex = i;

                break;
            }
        }

        if (bossPointIndex != -1)
        {
            kSpawnPointList.Insert(kSpawnPointList.Count, bossPoint);
            kSpawnPointList.RemoveAt(bossPointIndex);
        }

        monsterCount = 0;
        for (int i = 0; i < kSpawnPointList.Count; i++)
        {
            AddMonsterInfo(kSpawnPointList[i].kMonsterID, kSpawnPointList[i].GetMaxMonsterCount());
            monsterCount += kSpawnPointList[i].GetMaxMonsterCount();

            if (kSpawnPointList[i].GateObjTableId > 0)
            {
                AddMonsterInfo(kSpawnPointList[i].GateObjTableId, 1);
                ++monsterCount;
            }
        }
    }

    private void AddMonsterInfo(int tableId, int count)
    {
        if (m_dicMonsterInfo.ContainsKey(tableId))
        {
            if (World.Instance.EnemyMgr.MaxSpawnMonsterCount <= 0)
            {
                m_dicMonsterInfo[tableId] += count;
            }
            else
            {
                m_dicMonsterInfo[tableId] = Mathf.Clamp(m_dicMonsterInfo[tableId] + count, 1, World.Instance.EnemyMgr.MaxSpawnMonsterCount);
            }
        }
        else
            m_dicMonsterInfo.Add(tableId, count);
    }

    public void Clean()
    {
        for (int i = 0; i < kSpawnPointList.Count; i++)
        {
            BattleSpawnPoint SP = kSpawnPointList[i];
            if (SP == null)
            {
                kSpawnPointList.RemoveAt(i);
                i -= 1;
            }
        }
    }

    public void Active()
    {
        m_waiting = false;
        SpawnEnemy();
    }

    public void Deactivate()
    {
        for (int i = 0; i < kSpawnPointList.Count; i++)
        {
            kSpawnPointList[i].SetSpawned();
        }
         
        gameObject.SetActive(false);
    }

    private void CameraSetting()
    {
        if (!UseCameraSetting)
        {
            return;
        }

        if (CameraMode == InGameCamera.EMode.DEFAULT)
        {
            World.Instance.InGameCamera.SetMode(CameraMode, CameraDefaultSetting, 0.2f);
            FSaveData.Instance.RemoveCameraSettingData();
        }
        else if (CameraMode == InGameCamera.EMode.SIDE)
        {
            World.Instance.InGameCamera.SetMode(CameraMode, CameraSideSetting);
            CameraSideSetting.Save();
        }
        else if (CameraMode == InGameCamera.EMode.FIXED)
        {
            World.Instance.InGameCamera.SetMode(CameraMode, CameraFixedSetting);
            CameraFixedSetting.Save();
        }
        else if (CameraMode == InGameCamera.EMode.FOLLOW_PLAYER)
        {
            World.Instance.InGameCamera.SetMode(CameraMode, CameraFollowPlayerSetting);
        }
    }

    BattleSpawnPoint m_bossAppearSpawnPoint = null;
    Enemy mBoss = null;
    private void SpawnEnemy()
    {
        World.Instance.Player.ClearEnemyNavigators();
        World.Instance.Player.ClearGateNavigator();

        m_bossAppearSpawnPoint = null;
        for (int i = 0; i < kSpawnPointList.Count; i++)
        {
            if(!kSpawnPointList[i].Init())
			{
				continue;
			}

            //World.Instance.Player.SetEnemyNavigatorTarget(i, kSpawnPointList[i].kMonsterObj);

            if(kSpawnPointList[i].GateObjTableId > 0)
            {
                World.Instance.Player.AddGateNavigator(kSpawnPointList[i].GateObj);
            }

            if (kSpawnPointList[i].kMonsterObj.grade == Unit.eGrade.Boss)
            {
                m_bossAppearSpawnPoint = kSpawnPointList[i];
                mBoss = kSpawnPointList[i].kMonsterObj;
            }
        }

        for (int i = 0; i < kSpawnPointList.Count; i++)
        {
            if (kSpawnPointList[i].kMonsterObj == null)
            {
                continue;
            }

            kSpawnPointList[i].kMonsterObj.Deactivate();
        }

        if (m_bossAppearSpawnPoint == null)
        {
            if (kSpawnPointList.Count > 0)
            {
                CameraSetting();

                //7799
                /*
                if (World.Instance.player.boSupporter != null)
                    VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.Monster, World.Instance.player.boSupporter.data.TableID);
                    */
                //List<Vector3> listTurnPosition = new List<Vector3>();
                for (int i = 0; i < kSpawnPointList.Count; i++)
                {
                    BattleSpawnPoint SP = kSpawnPointList[i];
                    SP.Spawn();
                }

                //World.Instance.playerCam.SetControlMode(PlayerCamera.eOtherControlMode.Battle);
            }
            else
            {
                World.Instance.EnemyMgr.SkipSpawnEnemy();
            }
        }
        else
        {
            mBossAppearDirector = World.Instance.GetDirector("BossAppear");

            if (World.Instance.StageData != null && World.Instance.StageData.ScenarioID_BeforeBossAppear > 0)
            {
                mWorldStage.ShowScenario(World.Instance.StageData.ScenarioID_BeforeBossAppear, OnEndBeforeBossAppear);
            }
            else
                OnEndBeforeBossAppear();
        }
    }

	private IEnumerator SpawnEnemyWithBoss() {
        Debug.Log( "Boss Warning!!!!!!!!!!!!!!!!!!!!!!!!!!" );

        float duration = 1.0f;
        bool resumePlayerInput = false;

        World.Instance.InGameCamera.TurnToTarget( m_bossAppearSpawnPoint.transform.position, duration, true );

        if ( World.Instance.StageType == eSTAGETYPE.STAGE_RAID ) {
            for ( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                Player player = World.Instance.ListPlayer[i];
                if ( player == null ) {
                    continue;
                }

                if ( player.Input && !player.Input.isPause ) {
                    player.Input.Pause( true );
                    player.Input.LockPause = true;

                    resumePlayerInput = true;
                }
                else if ( player.AI && !player.AI.IsStop ) {
                    player.StopBT();
                }
            }

            yield return new WaitForSeconds( duration );
        }
		else if( World.Instance.StageData == null || World.Instance.StageData.ScenarioID_BeforeBossAppear <= 0 ) {
			for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
				Player player = World.Instance.ListPlayer[i];
				if( player == null ) {
					continue;
				}

				if( player.actionSystem.currentAction ) {
					if( player.actionSystem.GetCurrentAction<ActionSelectSkillBase>() && !player.UsingUltimateSkill ) {
						ActionChargeAttack actionChargeAttack = player.actionSystem.GetCurrentAction<ActionChargeAttack>();
						ActionTargetingAttack actionTargetingAtk = player.actionSystem.GetCurrentAction<ActionTargetingAttack>();

						if( actionChargeAttack || actionTargetingAtk ) {
							player.actionSystem.CancelCurrentAction();
						}
						else {
							ActionBase action = player.actionSystem.currentAction;
							while( action && action.isPlaying ) {
								yield return null;
							}
						}
					}
					else {
						player.actionSystem.CancelCurrentAction();
					}
				}

				if( player.Input && !player.Input.isPause ) {
                    player.Input.Pause( true );
                    player.Input.LockPause = true;

					resumePlayerInput = true;
				}
				else if( player.AI && !player.AI.IsStop ) {
					player.StopBT();
				}
			}

			World.Instance.UIPlay.m_screenEffect.ShowBossWarning( duration );
			yield return new WaitForSeconds( duration );
		}

		EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_ENEMY_BOSS_APPEAR, mBoss );

        // For tutorial
		if( GameSupport.IsInGameTutorial() && GameInfo.Instance.UserData.GetTutorialState() == (int)eTutorialState.TUTORIAL_STATE_Init ) {
			World.Instance.Player.AddSp( GameInfo.Instance.BattleConfig.USMaxSP );
			World.Instance.ShowTutorialHUD( 3 );
		}

		if( resumePlayerInput ) {
			Debug.Log( "인풋컨트롤러 펄스 in Boss Warning!!!!!!!!!!!!!!!!!!!!!!!!!!" );

			for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
				World.Instance.ListPlayer[i].Input.LockPause = false;
				World.Instance.ListPlayer[i].Input.Pause( false );
			}
		}

		World.Instance.BossSpawnPosition = m_bossAppearSpawnPoint.transform.position;
		World.Instance.BossSpawnRotation = m_bossAppearSpawnPoint.transform.rotation;

		m_bossAppearSpawnPoint.Spawn();

		if( mBossAppearDirector == null ) {
			mBossAppearDirector = m_bossAppearSpawnPoint.kMonsterObj.GetDirector( "BossAppear" );
		}

		if( mBossAppearDirector ) {
			while( mBossAppearDirector.playableDirector.state == UnityEngine.Playables.PlayState.Paused ) {
				yield return mWaitForFixedUpdate;
			}

			mBossAppearDirector.SetCallbackOnEnd3( StartSpawnWithBoss );
		}
		else {
			StartSpawnWithBoss();
		}
	}

	private void StartSpawnWithBoss()
    {
        for (int i = 0; i < kSpawnPointList.Count; i++)
        {
            if (kSpawnPointList[i] == m_bossAppearSpawnPoint)
                continue;

            BattleSpawnPoint SP = kSpawnPointList[i];
            SP.Spawn();
        }

        //World.Instance.playerCam.battleDistance = new Vector3(World.Instance.playerCam.battleDistance.x, World.Instance.playerCam.battleDistance.y, -5.5f);
        //World.Instance.playerCam.SetControlMode(PlayerCamera.eOtherControlMode.Battle);

        //World.Instance.playerCam.Turn(m_bossAppearSpawnPoint.kMonsterObj.transform.position);

        CameraSetting();
        //World.Instance.InGameCamera.TurnToTarget(m_bossAppearSpawnPoint.kMonsterObj.transform.position, 0.5f);
    }

    private void OnEndBeforeBossAppear()
    {
        GameUIManager.Instance.HideUI("GameOffPopup", false);
        //World.Instance.InGameCamera.SetDefaultMode();

        StartCoroutine("SpawnEnemyWithBoss");
    }

    public bool IsMonsterActive()
    {
        if (m_waiting == true || World.Instance.EnemyMgr.WaitForSpawn)
        {
            return true;
        }

        for (int i = 0; i < kSpawnPointList.Count; i++)
        {
            BattleSpawnPoint SP = kSpawnPointList[i];
            if (SP.kMonsterObj == null)
            {
                continue;
            }

            if (spawnTimingType == eSpawnTimingType.Deactivate && SP.kMonsterObj.IsActivate() || SP.IsAllMonsterDie() == false)
                return true;
        }

        //World.Instance.playerCam.SetControlMode(PlayerCamera.eOtherControlMode.None, 30.0f);
        return false;
    }

	public bool IsAllMonsterDie() {
		for( int i = 0; i < kSpawnPointList.Count; i++ ) {
            if( !kSpawnPointList[i].IsAllMonsterDie() ) {
                return false;
            }
		}

		return true;
	}

    public bool IsAllMonsterHpIsZero() {
        for( int i = 0; i < kSpawnPointList.Count; i++ ) {
            if( !kSpawnPointList[i].IsAllMonsterHpIsZero() ) {
                return false;
            }
        }

        return true;
    }

    public bool HasBossMonster()
    {
        GameClientTable.Monster.Param param = null;
        for (int i = 0; i < kSpawnPointList.Count; i++)
        {
            param = GameInfo.Instance.GameClientTable.FindMonster(kSpawnPointList[i].kMonsterID);
            if (param.Grade == (int)Unit.eGrade.Boss)
                return true;
        }

        return false;
    }

    public int GetTotalMonsterCount()
    {
        int n = 0;
        for (int i = 0; i < kSpawnPointList.Count; i++)
            n += kSpawnPointList[i].GetMaxMonsterCount();

        return n;
    }
}
