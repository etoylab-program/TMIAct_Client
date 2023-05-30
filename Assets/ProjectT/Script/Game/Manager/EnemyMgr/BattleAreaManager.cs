
using System.Collections.Generic;
using UnityEngine;


public class BattleAreaManager : BaseEnemyMgr
{
    [Header("[Defence Object]")]
    public int          DefenceObjTableId;
    public GameObject   DefenceObjStartPos;

    [Header("[Battle Area]")]
    public List<BattleArea> kBattleAreaList = new List<BattleArea>();

    public int CurBattleAreaIndex { get; private set; } = -1;
    public int CurSpawnGroupIndex { get; private set; } = -1;

    private Dictionary<int, int>    DicMonsterInfo  = new Dictionary<int, int>();
    private bool                    NotifyOnce      = false;


    public override void Init()
    {
        if (kBattleAreaList.Count != 0)
        {
            for (int i = 0; i < kBattleAreaList.Count; i++)
            {
                kBattleAreaList[i].InitEnvObjects();
                kBattleAreaList[i].gameObject.SetActive(false);

                if (kBattleAreaList[i].kEnterBarricade != null)
                    kBattleAreaList[i].kEnterBarricade.gameObject.SetActive(false);
                if (kBattleAreaList[i].kExitBarricade != null)
                    kBattleAreaList[i].kExitBarricade.gameObject.SetActive(false);
            }

            //kBattleAreaList[0].OnBattleArea();
        }

        for (int i = 0; i < kBattleAreaList.Count; i++)
        {
            kBattleAreaList[i].Init(i);
        }

        if (DefenceObjTableId > 0)
        { // OtherObject를 몬스터 보다 먼저 생성해둬야 몬스터 AI가 OtherObject 정보를 갖고 있을 수 있음.
            GameClientTable.Monster.Param param = GameInfo.Instance.GetMonsterData(DefenceObjTableId);
            if (param == null)
                return;

            string strModel = Utility.AppendString("Unit/", param.ModelPb, ".prefab");
            otherObject = ResourceMgr.Instance.CreateFromAssetBundle<OtherObject>("unit", strModel);

            otherObject.Init(param.ID, eCharacterType.Other, null);
            otherObject.SetInitialPosition(DefenceObjStartPos.transform.position, DefenceObjStartPos.transform.rotation);

            ShowOtherObject(true);
        }

        int totalMonsterCount = 0;

        maxMonsterCountInSpawnGroup = 0;
        for (int i = 0; i < kBattleAreaList.Count; i++)
        {
            if (kBattleAreaList[i].maxMonsterCountInSpawnGroup > maxMonsterCountInSpawnGroup)
                maxMonsterCountInSpawnGroup = kBattleAreaList[i].maxMonsterCountInSpawnGroup;

            foreach (KeyValuePair<int, int> kv in kBattleAreaList[i].dicMonsterInfo)
            {
                totalMonsterCount += kv.Value;
                AddMonsterInfo(kv.Key, kv.Value);
            }
        }

        m_listEnemy.Clear();
        foreach (KeyValuePair<int, int> kv in DicMonsterInfo)
        {
            for (int i = 0; i < kv.Value; i++)
            {
                GameSupport.CreateEnemy(kv.Key);
            }
        }

        Log.Show(name + "의 생성한 몬스터 수는 : " + m_listEnemy.Count + ", 총 몬스터 수는 " + totalMonsterCount, Log.ColorType.Blue);

        // 마지막 스폰 그룹은 스폰 타이밍이 HP일 필요가 없음.
        BattleArea lastBA = kBattleAreaList[kBattleAreaList.Count - 1];
        BattleSpawnGroup lastSG = lastBA.kSpawnGroupList[lastBA.kSpawnGroupList.Count - 1];
        lastSG.spawnTimingType = BattleSpawnGroup.eSpawnTimingType.Deactivate;

        NotifyOnce = false;
    }

	public override int GetItemHasEnvObjectCount() {
		int count = base.GetItemHasEnvObjectCount();

		for( int i = 0; i < kBattleAreaList.Count; i++ ) {
			if( kBattleAreaList[i].EnvObjects == null ) {
				continue;
			}

            for( int j = 0; j < kBattleAreaList[i].EnvObjects.Length; j++ ) {
                if( kBattleAreaList[i].EnvObjects[j] == null ) {
                    continue;
				}

                if( kBattleAreaList[i].EnvObjects[j].AllDropItemID > 0 || kBattleAreaList[i].EnvObjects[j].RandomDropItemID > 0 ) {
                    ++count;
                }
            }
		}

		return count;
	}

	public void ShowOtherObject(bool show)
    {
        if (otherObject == null)
            return;

        otherObject.gameObject.SetActive(show);
    }

    private void AddMonsterInfo(int tableId, int count)
    {
        if (DicMonsterInfo.ContainsKey(tableId) == true)
        {
            if (DicMonsterInfo[tableId] < count)
            {
                DicMonsterInfo[tableId] = count;
            }
        }
        else
        {
            DicMonsterInfo.Add(tableId, count);
        }
    }

	public override void Active() {
		base.Active();

		if ( GameInfo.Instance.ContinueStage ) {
			BattleArea battleArea = null;

			CurBattleAreaIndex = PlayerPrefs.GetInt( "SAVE_STAGE_BATTLE_AREA_ID_" );
			for ( int i = 0; i < CurBattleAreaIndex; i++ ) {
				kBattleAreaList[i].Deactivate();
			}

			battleArea = kBattleAreaList[CurBattleAreaIndex];
			battleArea.Activate();

			if ( PlayerPrefs.HasKey( "SAVE_STAGE_CAMERA_MODE_" ) ) {
				InGameCamera.EMode mode = (InGameCamera.EMode)PlayerPrefs.GetInt( "SAVE_STAGE_CAMERA_MODE_" );
				if ( mode == InGameCamera.EMode.SIDE ) {
					World.Instance.InGameCamera.SideSetting.Load();
					World.Instance.InGameCamera.SetSideMode( World.Instance.InGameCamera.SideSetting );
				}
				else if ( mode == InGameCamera.EMode.FIXED ) {
					World.Instance.InGameCamera.FixedSetting.Load();
					World.Instance.InGameCamera.SetFixedMode( World.Instance.InGameCamera.FixedSetting );
				}
			}
			else {
				World.Instance.InGameCamera.SetDefaultMode();
			}

			battleArea.ActiveArea();

			float playerHp = PlayerPrefs.GetFloat( "SAVE_STAGE_PLAYER_HP_" );
			float playerSp = PlayerPrefs.GetFloat( "SAVE_STAGE_PLAYER_SP_" );
			World.Instance.Player.ReInitPlayerUI( playerHp, playerSp );

			float supporterCoolTime = PlayerPrefs.GetFloat( "SAVE_STAGE_SUPPORTER_COOLTIME_" );
			World.Instance.UIPlay.ContinueSupporterCoolTime( supporterCoolTime );

			int usedBOSetIdCount = PlayerPrefs.GetInt( "SAVE_STAGE_USED_BOSETID_COUNT_" );
			for ( int i = 1; i <= usedBOSetIdCount; i++ ) {
				int id = PlayerPrefs.GetInt( string.Format( "SAVE_STAGE_USED_USED_BOSETID_{0}_", i ) );
				World.Instance.Player.DeleteUsedBattleOptionSet( id );
			}

			CurSpawnGroupIndex = PlayerPrefs.GetInt( "SAVE_STAGE_SPAWN_GROUP_ID_" );
			if ( CurSpawnGroupIndex > 0 ) {
				for ( int i = 0; i < CurSpawnGroupIndex; i++ ) {
					battleArea.kSpawnGroupList[i].Deactivate();
				}
			}

			CurSpawnGroupIndex -= 1;
			NextBattleSpawnGroup();

			int dieMonsterCount = PlayerPrefs.GetInt( "SAVE_STAGE_DIE_MONSTER_COUNT_" );
			if ( dieMonsterCount > 0 ) {
				World.Instance.SetDieMonsterCount( dieMonsterCount );
				World.Instance.UIPlay.UpdateMonsterCount( dieMonsterCount, World.Instance.ClearConditionValue );
			}
		}
		else {
			kBattleAreaList[0].gameObject.SetActive( true );

            if ( kBattleAreaList[0].kPathEffect ) {
                kBattleAreaList[0].kPathEffect.gameObject.SetActive( true );
            }
        }

		if ( otherObject ) {
			otherObject.OnMissionStart();
		}

		World.Instance.OnPlayerMissionStart();
	}

	public override void Deactive() {
        for ( int i = 0; i < kBattleAreaList.Count; i++ ) {
            kBattleAreaList[i].gameObject.SetActive( false );
        }
    }

    public override void GetStartPoint(out Vector3 pos, out Quaternion rot)
    {
        if (GameInfo.Instance.ContinueStage)
        {
            int battleAreaId = PlayerPrefs.GetInt("SAVE_STAGE_BATTLE_AREA_ID_");
            BattleArea battleArea = kBattleAreaList[battleAreaId];
            pos = battleArea.transform.position;

            int spawnGroupId = PlayerPrefs.GetInt("SAVE_STAGE_SPAWN_GROUP_ID_");
            BattleSpawnGroup battleSpawnGroup = battleArea.GetBattleSpawnGroup(spawnGroupId);

            if (battleSpawnGroup && battleSpawnGroup.kSpawnPointList.Count > 0)
            {
                BattleSpawnPoint battleSpawnPoint = battleSpawnGroup.kSpawnPointList[0];

                Vector3 p1 = battleArea.transform.position;
                Vector3 p2 = battleSpawnPoint.transform.position;
                p2.y = p1.y;

                rot = Quaternion.LookRotation((p2 - p1).normalized);
            }
            else
            {
                rot = battleArea.transform.rotation;
            }
        }
        else
        { 
            base.GetStartPoint(out pos, out rot);
        }
    }

    private void Update()
    {
        if (World.Instance.IsEndGame == true)
            return;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            List<Unit> list = GetActiveEnemies(null);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].curHp <= 0.0f)
                    continue;

                list[i].SetDie();
                list[i].actionSystem.CancelCurrentAction();
                list[i].CommandAction(eActionCommand.Die, null);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            base.Init();
#endif

        BattleArea BA = GetBattleArea(CurBattleAreaIndex);
        if (BA == null)
            return;

        BattleSpawnGroup SG = BA.GetBattleSpawnGroup(CurSpawnGroupIndex);
        if (SG == null)
            return;

        if (!SG.IsMonsterActive())
        {
            //다음 스폰으로
            NextBattleSpawnGroup();
            World.Instance.UIPlay.ActiveTargetGauge(false);
        }

        if (NotifyOnce == false && IsAllMonsterDie() == true)
        {
            if ( World.Instance.Boss == null || !World.Instance.Boss.HasDirector( "BossDie" ) ) {
                EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_ALL_ENEMY_DEAD, null );
                NotifyOnce = true;
            }
        }
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

    public void Clean()
    {
        for (int i = 0; i < kBattleAreaList.Count; i++)
        {
            BattleArea BA = kBattleAreaList[i];
            if (BA == null)
            {
                kBattleAreaList.RemoveAt(i);
                i -= 1;
            }
        }
    }

    public BattleArea GetCurrentBattleArea()
    {
        if (CurBattleAreaIndex < 0 || CurBattleAreaIndex >= kBattleAreaList.Count)
            return null;

        return kBattleAreaList[CurBattleAreaIndex];
    }

    public bool IsPortalOrPathOpen()
    {
        BattleArea baCur = GetCurrentBattleArea();
        if(baCur && baCur.gameObject.activeSelf && baCur.portalEntry && baCur.portalEntry.activeSelf)
        {
            return true;
        }

        BattleArea baNext = GetNextBattleAreaOrNull();
        if(baNext && baNext.gameObject.activeSelf && baNext.kPathEffect && baNext.kPathEffect.activeSelf)
        {
            return true;
        }

        return false;
    }

    public BattleArea GetNextBattleAreaOrNull()
    {
        if (CurBattleAreaIndex < 0 || (CurBattleAreaIndex + 1) >= kBattleAreaList.Count)
            return null;

        return kBattleAreaList[CurBattleAreaIndex + 1];
    }

    public BattleArea GetBattleArea(int index)
    {
        if (index < 0 || index >= kBattleAreaList.Count)
            return null;

        return kBattleAreaList[index];
    }

    public int GetBattleAreaIndex(BattleArea battleArea)
    {
        for(int i = 0; i < kBattleAreaList.Count; i++)
        {
            if(kBattleAreaList[i] == battleArea)
            {
                return i;
            }
        }

        return -1;
    }

    public void NextBattleArea()
    {
        CurBattleAreaIndex += 1;
        CurSpawnGroupIndex = -1;

        BattleArea BA = GetBattleArea(CurBattleAreaIndex);
        if (BA == null)
        {
            Debug.LogError(CurBattleAreaIndex + "번 배틀 에어리어는 존재하지 않습니다.");
        }
        else
        {
            NextBattleSpawnGroup();
        }
    }

    public void NextBattleSpawnGroup()
    {
        CurSpawnGroupIndex += 1;

        BattleArea BA = GetBattleArea(CurBattleAreaIndex);
        BattleArea nextBattleArea = GetBattleArea(CurBattleAreaIndex + 1);

        BattleSpawnGroup SG = BA.GetBattleSpawnGroup(CurSpawnGroupIndex);
        if (SG == null)
        {
            //World.Instance.playerCam.SetControlMode(PlayerCamera.eOtherControlMode.None, 15.0f);
            BA.OffBattleArea();//현재 구역 종료
            WaitForSpawn = true;

            //BA = GetBattleArea(CurBattleAreaIndex + 1);
            if (nextBattleArea != null)
            {
                nextBattleArea.gameObject.SetActive(true);

                if ( nextBattleArea.kPathEffect ) {
                    nextBattleArea.kPathEffect.gameObject.SetActive( true );
                }

                int spawnGroupIndex = 0;
                if(nextBattleArea.kSpawnGroupList == null || nextBattleArea.kSpawnGroupList.Count <= 0)
                {
                    spawnGroupIndex = -1;
                }

                FSaveData.Instance.UpdateSaveStageData(nextBattleArea.Index, spawnGroupIndex);
            }

            Vector3 pos = new Vector3(-1000.0f, -1000.0f, -1000.0f);
            if (nextBattleArea && nextBattleArea.portalEntry && nextBattleArea.portalEntry.activeSelf)
            {
                pos = nextBattleArea.portalEntry.transform.position;
            }
            else if (BA && BA.portalEntry && BA.portalEntry.activeSelf)
            {
                pos = BA.portalEntry.transform.position;
            }
            else if (nextBattleArea && nextBattleArea.kPathEffect && nextBattleArea.kPathEffect.activeSelf)
            {
                pos = nextBattleArea.kPathEffect.transform.position;
            }
            else if (nextBattleArea)
            {
                pos = nextBattleArea.transform.position;
            }

			if( pos != new Vector3( -1000.0f, -1000.0f, -1000.0f ) ) {
				World.Instance.Player.SetNextBattleAreaNavigator( pos );

				for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                    if( World.Instance.ListPlayer[i].Input.isPause ) {
                        World.Instance.ListPlayer[i].Input.Pause( false );
                    }

					World.Instance.ListPlayer[i].SetAutoMove( true );
				}

				if( AppMgr.Instance.CustomInput.InputType == BaseCustomInput.eInputType.Touch && 
                    !Utility.IsInViewPort( World.Instance.InGameCamera.MainCamera.WorldToViewportPoint( pos ) ) ) {
					World.Instance.InGameCamera.TurnToTarget( pos, GameInfo.Instance.BattleConfig.CameraTurnSpeed, false, pos );
				}
			}
		}
        else
        {
            if (CurSpawnGroupIndex == 0)
            {
                BA.OnBattleArea();
            }

            FSaveData.Instance.UpdateSaveStageData(CurBattleAreaIndex, CurSpawnGroupIndex);
            SG.Active();
        }

        CollectDropItem();
    }

    public void DiedMonster(int monsterid)
    {
        GameUIManager.Instance.Renewal("GameMainPanel");

        BattleArea BA = GetBattleArea(CurBattleAreaIndex);
        if (BA == null)
            return;

        BattleSpawnGroup SG = BA.GetBattleSpawnGroup(CurSpawnGroupIndex);
        if (SG == null)
            return;

        if (!SG.IsMonsterActive())
        {
            //다음 스폰으로
            NextBattleSpawnGroup();
        }
    }

    public BattleSpawnGroup GetCurrentSpawnGroup()
    {
        BattleArea battleArea = GetBattleArea(CurBattleAreaIndex);
        if (battleArea == null)
            return null;

        BattleSpawnGroup spawnGroup = battleArea.GetBattleSpawnGroup(CurSpawnGroupIndex);
        if (spawnGroup == null)
            return null;

        return spawnGroup;
    }

    public BattleSpawnGroup GetCurrentSpawnGroupByIndex(int battleAreaIndex, int spawnGroupIndex)
    {
        BattleArea battleArea = GetBattleArea(battleAreaIndex);
        if (battleArea == null)
            return null;

        BattleSpawnGroup spawnGroup = battleArea.GetBattleSpawnGroup(spawnGroupIndex);
        if (spawnGroup == null)
            return null;

        return spawnGroup;
    }

    public BattleSpawnGroup GetLastSpawnGroup()
    {
        BattleArea battleArea = kBattleAreaList[kBattleAreaList.Count - 1];
        if (battleArea == null || battleArea.kSpawnGroupList.Count <= 0)
            return null;

        BattleSpawnGroup spawnGroup = battleArea.kSpawnGroupList[battleArea.kSpawnGroupList.Count - 1];
        if (spawnGroup == null)
            return null;

        return spawnGroup;
    }

    public bool IsLastSpawnGroup()
    {
        BattleSpawnGroup currentSpawnGroup = GetCurrentSpawnGroup();
        BattleSpawnGroup lastSpawnGrpup = GetLastSpawnGroup();

        if (currentSpawnGroup != null && lastSpawnGrpup != null && currentSpawnGroup == lastSpawnGrpup)
            return true;

        return false;
    }

    public override bool IsAllMonsterDie()
    {
        bool allDie = true;
        for(int i = 0; i < kBattleAreaList.Count; i++)
        {
            for (int j = 0; j < kBattleAreaList[i].kSpawnGroupList.Count; j++)
            {
                if (!kBattleAreaList[i].kSpawnGroupList[j].IsAllMonsterDie())
                {
                    allDie = false;
                    break;
                }
            }
        }

        return allDie;
    }

    public int GetMaxMonsterCounter()
    {
        int compare = 0;
        for(int i = 0; i < kBattleAreaList.Count; i++)
        {
            for(int j = 0; j < kBattleAreaList[i].kSpawnGroupList.Count; j++)
            {
                int n = kBattleAreaList[i].kSpawnGroupList[j].GetTotalMonsterCount();
                if (n > compare)
                    compare = n;
            }
        }

        return compare;
    }

    /*private void CreateEnemy(int tableId)
    {
        Enemy enemy = GameSupport.CreateEnemy(tableId, false);
        if (enemy == null)
            return;

        m_listEnemy.Add(enemy);
    }*/

    public override void AddEnemy(Unit enemy)
    {
        base.AddEnemy(enemy);

        if(enemy.grade == Unit.eGrade.Boss)
            World.Instance.Player.UpdateTargetHp(enemy);
    }

    public override void Clear()
    {
        for (int i = 0; i < m_listEnemy.Count; i++)
            m_listEnemy[i].Deactivate();

        gameObject.SetActive(false);
    }

    public override void AddStageBOSet(int stageBOSetGroupId)
    {
        if (stageBOSetGroupId <= 0)
        {
            return;
        }

        List<GameClientTable.StageBOSet.Param> list = GameInfo.Instance.GameClientTable.FindAllStageBOSet(x => x.Group == stageBOSetGroupId);
        if(list == null || list.Count <= 0)
        {
            return;
        }

        for(int i = 0; i < list.Count; i++)
        {
            string str = list[i].StageBOSetList.Replace(" ", "");

			string[] split = Utility.Split(str, ','); //str.Split(',');
            for(int j = 0; j < split.Length; j++)
            {
                int id = Utility.SafeIntParse(split[j]);

                for (int k = 0; k < m_listEnemy.Count; k++)
                {
                    Enemy enemy = m_listEnemy[k] as Enemy;
                    if(enemy == null)
                    {
                        continue;
                    }

                    enemy.AddBOSet(id);
                }
            }
        }
    }

	public override void AddStageBOSet( GameClientTable.StageBOSet.Param param ) {
		string str = param.StageBOSetList.Replace(" ", "");
		string[] split = Utility.Split(str, ',');

		for( int j = 0; j < split.Length; j++ ) {
			int id = Utility.SafeIntParse(split[j]);

			for( int k = 0; k < m_listEnemy.Count; k++ ) {
				Enemy enemy = m_listEnemy[k] as Enemy;
				if( enemy == null ) {
					continue;
				}

				enemy.AddBOSet( id );

                if( enemy.ChangeEnemy ) {
                    enemy.ChangeEnemy.AddBOSet( id );
                }
            }
		}
	}

	public override InGameCamera.sDefaultSetting GetCurAreaDefaultCameraSettingOrNull()
    {
        BattleArea battleArea = GetCurrentBattleArea();
        if(battleArea == null)
        {
            return null;
        }

        BattleSpawnGroup battleSpawnGroup = battleArea.GetBattleSpawnGroup(CurSpawnGroupIndex);
        if(battleSpawnGroup == null && !battleArea.UseCameraSetting)
        {
            return null;
        }

        if(battleSpawnGroup && battleSpawnGroup.UseCameraSetting)
        {
            return battleSpawnGroup.CameraDefaultSetting;
        }

        if(battleArea.UseCameraSetting)
        {
            return battleArea.CameraDefaultSetting;
        }

        return null;
    }

    public override Transform[] GetEdgePoints()
    {
        BattleArea battleArea = GetCurrentBattleArea();
        if(battleArea == null)
        {
            return null;
        }

        return battleArea.edgePoints;
    }
}
