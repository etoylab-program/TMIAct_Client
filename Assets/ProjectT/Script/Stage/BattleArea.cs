
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleArea : MonoBehaviour
{
    [Header("[Property]")]
    //public World.eGameMode gameMode = World.eGameMode.Normal;
    //public bool usePool = false;

    public Barricade kEnterBarricade;
    public Barricade kExitBarricade;
    public GameObject kPathEffect;

    public List<BattleSpawnGroup> kSpawnGroupList = new List<BattleSpawnGroup>();

    [Header("[Area Points]")]
    public Transform center;
    public Transform[] edgePoints;

    [Header("[Env Objects]")]
    public EnvObject[] EnvObjects;

    /*[Header("[Crash MiniGame]")]
    public string uiCrashName;
    public float crashTime;*/

    [Header("[Portal]")]
    public GameObject portalEntry;
    public GameObject portalExit;
    public ParticleSystem m_psPathEnd = null;

    [Header("[Path]")]
    public List<PathGroup> ListPathGroup = new List<PathGroup>();

    public BattleAreaManager    BattleAreaMgr   { get; private set; } = null;
    public int                  Index           { get; private set; } = 0;

    [Header("[Camera Setting]")]
    [HideInInspector] public bool                                 UseCameraSetting            = false;
    [HideInInspector] public InGameCamera.EMode                   CameraMode                  = InGameCamera.EMode.DEFAULT;
    [HideInInspector] public InGameCamera.sDefaultSetting         CameraDefaultSetting        = new InGameCamera.sDefaultSetting(InGameCamera.DEFAULT_CAMERA_DISTANCE, InGameCamera.DEFAULT_CAMERA_LOOKAT);
    [HideInInspector] public InGameCamera.sSideSetting            CameraSideSetting           = new InGameCamera.sSideSetting(new Vector3(0.0f, 1.5f, 4.0f), new Vector2(0.0f, 1.0f), Unit.eAxisType.Z);
    [HideInInspector] public InGameCamera.sFixedSetting           CameraFixedSetting          = new InGameCamera.sFixedSetting();
    [HideInInspector] public InGameCamera.sFollowPlayerSetting    CameraFollowPlayerSetting   = new InGameCamera.sFollowPlayerSetting();

    private BattleAreaScenario  mScenarioObject = null;
    private List<Unit>          mListTemp       = new List<Unit>();

    private bool _activearea;
    private Dictionary<int, int> m_dicMonsterInfo = new Dictionary<int, int>();

    public Dictionary<int, int> dicMonsterInfo { get { return m_dicMonsterInfo; } }
    public int maxMonsterCountInSpawnGroup { get; private set; }


    void Awake()
    {
        BattleAreaMgr = this.transform.parent.GetComponent<BattleAreaManager>();
        _activearea = false;

        mScenarioObject = GetComponentInChildren<BattleAreaScenario>();
        if(mScenarioObject)
        {
            mScenarioObject.gameObject.SetActive(true);
        }

        if (kExitBarricade != null)
            kExitBarricade.gameObject.SetActive(false);
    }

	public void Init( int index ) {
		Index = index;
		maxMonsterCountInSpawnGroup = 0;

		if ( kSpawnGroupList.Count > 0 ) {
            for ( int i = 0; i < kSpawnGroupList.Count; i++ ) {
                kSpawnGroupList[i].Init( this );
            }

			// 마지막 스폰그룹엔 HP 스폰타이밍 사용 못함
			if ( kSpawnGroupList[kSpawnGroupList.Count - 1].spawnTimingType == BattleSpawnGroup.eSpawnTimingType.Hp ) {
				kSpawnGroupList[kSpawnGroupList.Count - 1].spawnTimingType = BattleSpawnGroup.eSpawnTimingType.Deactivate;
			}

			for ( int i = 0; i < kSpawnGroupList.Count; i++ ) {
                if ( kSpawnGroupList[i].monsterCount > maxMonsterCountInSpawnGroup ) {
                    maxMonsterCountInSpawnGroup = kSpawnGroupList[i].monsterCount;
                }

                foreach ( KeyValuePair<int, int> kv in kSpawnGroupList[i].dicMonsterInfo ) {
                    AddMonsterInfo( kv.Key, kv.Value );
                }
			}
		}

		if ( portalEntry != null && portalExit != null ) {
			PortalExit exit = portalExit.GetComponent<PortalExit>();

			portalEntry.GetComponent<PortalEntry>().Init( this, exit );
			exit.Init( this );

			ShowPortal( false );
		}

		if ( kPathEffect != null ) {
			kPathEffect.SetActive( false );
		}

		if ( m_psPathEnd ) {
			m_psPathEnd.gameObject.SetActive( false );
		}
	}

	public void InitEnvObjects()
    {
        for (int i = 0; i < EnvObjects.Length; i++)
        {
            if (EnvObjects[i] == null)
                continue;

            EnvObjects[i].Init(-1, eCharacterType.Other, null);
            EnvObjects[i].Activate();
        }
    }

    public List<Unit> GetActiveEnvObjects()
    {
        mListTemp.Clear();
        for (int i = 0; i < EnvObjects.Length; i++)
        {
            if (EnvObjects[i] == null || !EnvObjects[i].IsActivate() || EnvObjects[i].curHp <= 0.0f)
            {
                continue;
            }

            GimmickObject gimmick = EnvObjects[i] as GimmickObject;
            if(gimmick)
            {
                continue;
            }

            mListTemp.Add(EnvObjects[i]);
        }

        return mListTemp;
    }

    private void AddMonsterInfo(int tableId, int count)
    {
        if (m_dicMonsterInfo.ContainsKey(tableId) == true)
        {
            m_dicMonsterInfo[tableId] += count;
        }
        else
        {
            m_dicMonsterInfo.Add(tableId, count);
        }
    }

    public void Clean()
    {
        for (int i = 0; i < kSpawnGroupList.Count; i++)
        {
            BattleSpawnGroup SG = kSpawnGroupList[i];
            if (SG == null)
            {
                kSpawnGroupList.RemoveAt(i);
                i -= 1;
            }
        }
    }

    public void ActiveArea()
    {
        _activearea = true;

        if (kPathEffect != null)
            kPathEffect.SetActive(false);
    }

    public BattleSpawnGroup GetBattleSpawnGroup(int index)
    {
        if (index < 0 || index >= kSpawnGroupList.Count)
            return null;

        return kSpawnGroupList[index];
    }

    public int GetSpawnGroupIndex(BattleSpawnGroup spawnGroup)
    {
        for(int i = 0; i < kSpawnGroupList.Count; i++)
        {
            if(kSpawnGroupList[i] == spawnGroup)
            {
                return i;
            }
        }

        return -1;
    }

	private void OnTriggerEnter( Collider Other ) {
        if( BattleAreaMgr == null || !Other.gameObject.CompareTag( "Player" ) || Other.gameObject.layer == (int)eLayer.PlayerClone || _activearea ) {
            return;
        }

        Player player = Other.GetComponent<Player>();
        if( player == null || player && player.IsHelper ) {
            return;
		}

		if( UseCameraSetting ) {
			if( CameraMode == InGameCamera.EMode.DEFAULT ) {
				World.Instance.InGameCamera.SetMode( CameraMode, CameraDefaultSetting );
				FSaveData.Instance.RemoveCameraSettingData();
			}
			else if( CameraMode == InGameCamera.EMode.SIDE ) {
				World.Instance.InGameCamera.SetMode( CameraMode, CameraSideSetting, CameraSideSetting.ResumeTime );
				CameraSideSetting.Save();
			}
			else if( CameraMode == InGameCamera.EMode.FIXED ) {
				World.Instance.InGameCamera.SetMode( CameraMode, CameraFixedSetting );
				CameraFixedSetting.Save();
			}
			else if( CameraMode == InGameCamera.EMode.FOLLOW_PLAYER ) {
				World.Instance.InGameCamera.SetMode( CameraMode, CameraFollowPlayerSetting );
			}
		}

		StartCoroutine( NextBattleArea( 0.0f ) );
	}

	private IEnumerator NextBattleArea( float delay ) {
		yield return new WaitForSeconds( delay );

		if( delay > 0.0f ) {
			for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                World.Instance.ListPlayer[i].Input.Pause( false );
			}
		}

		World.Instance.BossSpawnPosition = transform.position;
		World.Instance.BossSpawnRotation = transform.rotation;

		_activearea = true;
		BattleAreaMgr.NextBattleArea();
	}

	public void OnBattleArea() {
		for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
			World.Instance.ListPlayer[i].SetAutoMove( false );
			World.Instance.ListPlayer[i].HideBattleAreaNavigator();
		}

        if( kPathEffect != null && kPathEffect.activeSelf ) {
            if( m_psPathEnd ) {
                if( GameSupport.IsInGameTutorial() && 
                    GameInfo.Instance.UserData.GetTutorialState() == (int)eTutorialState.TUTORIAL_STATE_Init && 
                    GameInfo.Instance.UserData.GetTutorialStep() == 2 ) {
                    World.Instance.ShowTutorialHUD( 2 );
                }
                else if( GameSupport.IsInGameTutorial() && 
                         GameInfo.Instance.UserData.GetTutorialState() == (int)eTutorialState.TUTORIAL_STATE_Stage2Clear && 
                         GameInfo.Instance.UserData.GetTutorialStep() == 2 ) {
                    GameInfo.Instance.UserData.SetTutorial( GameInfo.Instance.UserData.GetTutorialState(), 3 );
                    World.Instance.ShowTutorialHUD( 2 );
                }

                m_psPathEnd.gameObject.SetActive( true );
            }

            Activate();
        }
        else if( kPathEffect == null ) {
            Activate();
        }

        if( kPathEffect != null ) {
            kPathEffect.SetActive( false );
        }
	}

	public void Activate() {
        if( kEnterBarricade != null ) {
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;

            for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                if( !World.Instance.ListPlayer[i].IsHelper ) {
                    pos = World.Instance.ListPlayer[i].transform.position;
                    rot = World.Instance.ListPlayer[i].transform.rotation;

                    continue;
                }

                World.Instance.ListPlayer[i].SetInitialPosition( pos + ( World.Instance.Player.transform.forward * 0.5f ), rot );
            }

            kEnterBarricade.gameObject.SetActive( true );
        }

        if( kExitBarricade != null ) {
            kExitBarricade.gameObject.SetActive( true );
        }

		gameObject.SetActive( true );

		if( m_psPathEnd ) {
			Invoke( "DeactivePathEnd", m_psPathEnd.main.duration );
		}
	}

	private void DeactivePathEnd()
    {
        if(m_psPathEnd == null)
        {
            return;
        }

        m_psPathEnd.gameObject.SetActive(false);
    }

    public void Deactivate()
    {
        OffBattleArea();

        for(int i = 0; i < kSpawnGroupList.Count; i++)
        {
            kSpawnGroupList[i].Deactivate();
        }

        if (kPathEffect != null)
            kPathEffect.SetActive(false);
    }

	public void OffBattleArea() {
        if( portalEntry == null ) {
            this.gameObject.SetActive( false );
        }

        if( kExitBarricade ) {
            kExitBarricade.gameObject.SetActive( false );
        }

        if( portalEntry ) {
            ShowPortal( true );
        }
	}

	public void AddPortal(GameObject entry, GameObject exit)
    {
        // Entry
        portalEntry = entry;
        portalEntry.AddComponent<BoxCollider>();
        portalEntry.AddComponent<PortalEntry>();
        Utility.SetLayer(portalEntry.gameObject, (int)eLayer.BattleAreaPortal, true);

        BoxCollider boxCol = portalEntry.GetComponent<BoxCollider>();
        boxCol.size = new Vector3(0.1f, 1.0f, 0.1f);
        boxCol.isTrigger = true;

        // Exit
        portalExit = exit;
        portalExit.AddComponent<BoxCollider>();
        portalExit.AddComponent<PortalExit>();
        Utility.SetLayer(portalExit.gameObject, (int)eLayer.BattleAreaPortal, true);

        boxCol = portalExit.GetComponent<BoxCollider>();
        boxCol.size = new Vector3(1.0f, 0.2f, 1.0f);
        boxCol.isTrigger = true;
    }

    public void ShowPortal(bool show)
    {
        portalEntry.SetActive(show);
        portalExit.SetActive(show);
    }

    /*public void EnterPortal(Player player)
    {
        player.SetInitialPosition(new Vector3(portalExit.transform.position.x, portalExit.transform.position.y, portalExit.transform.position.z),
                                  portalExit.transform.rotation);

        World.Instance.playerCam.SetDefaultPlayerCameraPos();

        ShowPortal(false);
        gameObject.SetActive(false);
    }*/

    public Vector3 GetCenterPos()
    {
        if(center)
        {
            return center.transform.position;
        }
        else
        {
            return transform.position;
        }
    }

    public void GetNearestPathGroupIndexAndStartPointIndex(Vector3 pos, ref int groupIndex, ref int startPointIndex)
    {
        groupIndex = -1;
        startPointIndex = -1;

        float compare = 99999.0f;
        for (int i = 0; i < ListPathGroup.Count; i++)
        {
            int pointIndex = ListPathGroup[i].GetNearestPointIndex(pos);
            if (pointIndex == -1)
            {
                continue;
            }

            float dist = Vector3.Distance(pos, ListPathGroup[i].ListPoint[pointIndex].transform.position);
            if (dist < compare)
            {
                compare = dist;

                groupIndex = i;
                startPointIndex = pointIndex;
            }
        }
    }

    public List<Vector3> GetPathPointListOrNull(Vector3 pos)
    {
        int groupIndex = -1;
        int startPointIndex = -1;

        GetNearestPathGroupIndexAndStartPointIndex(pos, ref groupIndex, ref startPointIndex);
        if (groupIndex == -1 || startPointIndex == -1)
        {
            return null;
        }

        return ListPathGroup[groupIndex].GetPointList(startPointIndex);
    }

    public void EDAddPath(PathGroup group)
    {
        int index = ListPathGroup.Count;
        group.EDInit(this, index);

        ListPathGroup.Add(group);
    }

    public void EDDeletePath(int index)
    {
        PathGroup find = ListPathGroup.Find(x => x.Index == index);
        if (find == null)
        {
            return;
        }

        DestroyImmediate(find.gameObject);
        ListPathGroup.Remove(find);

        for (int i = index; i < ListPathGroup.Count; i++)
        {
            ListPathGroup[i].EDSetIndex(i);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        DrawPathGizmos();

        if (this.portalEntry == null || this.portalExit == null)
        {
            return;
        }

        PortalEntry portalEntry = this.portalEntry.GetComponent<PortalEntry>();
        if (portalEntry == null || portalEntry.PortalType != PortalEntry.EType.JUMP)
        {
            return;
        }

        Color color = Gizmos.color;
        Gizmos.color = Color.red;

        Vector3 low = portalEntry.transform.position.y <= portalExit.transform.position.y ? portalEntry.transform.position : portalExit.transform.position;
        Vector3 high = portalEntry.transform.position.y >= portalExit.transform.position.y ? portalEntry.transform.position : portalExit.transform.position;
        Vector3 center = new Vector3(low.x, high.y * portalEntry.JumpWeight, low.z);

        for (int i = 0; i < 50; i++)
        {
            Vector3 pos = Utility.Bezier(portalEntry.transform.position, center, this.portalExit.transform.position, (float)i / 50.0f);
            Gizmos.DrawSphere(pos, 0.05f);
        }

        Gizmos.color = color;
    }

    private void DrawPathGizmos()
    {
        Color originalColor = Gizmos.color;

        for (int i = 0; i < ListPathGroup.Count; i++)
        {
            if (ListPathGroup[i].ListPoint.Count <= 0)
            {
                continue;
            }

            for (int j = 0; j < ListPathGroup[i].ListPoint.Count; j++)
            {
                Gizmos.color = Color.cyan;

                Gizmos.DrawSphere(ListPathGroup[i].ListPoint[j].transform.position, 0.2f);

                if (j < ListPathGroup[i].ListPoint.Count - 1)
                {
                    Gizmos.DrawSphere(ListPathGroup[i].ListPoint[j + 1].transform.position, 0.2f);

                    // 선명하게 라인 보이도록 5번 그려줌
                    for (int k = 0; k < 5; k++)
                    {
                        Gizmos.DrawLine(ListPathGroup[i].ListPoint[j].transform.position, ListPathGroup[i].ListPoint[j + 1].transform.position);
                    }
                }
            }
        }

        Gizmos.color = originalColor;
    }
#endif
}
