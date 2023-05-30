
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseEnemyMgr : MonoBehaviour
{
    [Header("[Property]")]
    public int MaxSpawnMonsterCount = 0;

    [Header("[EnvObject]")]
    public GameObject m_startPoint;
    public EnvObject[] envObjects;

    [Header("[Camera Setting]")]
    [HideInInspector] public bool                               UseCameraSetting            = false;
    [HideInInspector] public InGameCamera.EMode                 CameraMode                  = InGameCamera.EMode.DEFAULT;
    [HideInInspector] public InGameCamera.sDefaultSetting       CameraDefaultSetting        = new InGameCamera.sDefaultSetting(InGameCamera.DEFAULT_CAMERA_DISTANCE, InGameCamera.DEFAULT_CAMERA_LOOKAT);
    [HideInInspector] public InGameCamera.sSideSetting          CameraSideSetting           = new InGameCamera.sSideSetting(new Vector3(0.0f, 1.5f, 4.0f), new Vector2(0.0f, 1.0f), Unit.eAxisType.Z);
    [HideInInspector] public InGameCamera.sFixedSetting         CameraFixedSetting          = new InGameCamera.sFixedSetting();
    [HideInInspector] public InGameCamera.sFollowPlayerSetting  CameraFollowPlayerSetting   = new InGameCamera.sFollowPlayerSetting();

    [System.NonSerialized]
    public int maxMonsterCountInSpawnGroup = 0;

    public List<Unit>   listEnemy       { get { return m_listEnemy; } }
    public bool         WaitForSpawn    { get; protected set; } = true;
    public OtherObject  otherObject     { get; protected set; } = null;

    protected List<Unit>        m_listEnemy                     = new List<Unit>();
    protected int               CurSpawnMonsterCount            = 0;
    protected List<Unit>        m_listAllUnit                   = new List<Unit>();
    protected List<DropItem>    m_listCurDropItem               = new List<DropItem>();
    protected List<Unit>        mListTemp                       = new List<Unit>();
    protected List<Unit>        listFindForActiveEnemies        = new List<Unit>();
    protected List<Unit>        listFindForActiveEnemies2       = new List<Unit>();
    protected List<Unit>        listFindForNearActiveEnemies    = new List<Unit>();
    protected List<Unit>        listFindForNearActiveEnemies2   = new List<Unit>();
    protected List<Unit>        listFindMarkingEnemies          = new List<Unit>();

    
	public virtual void Init()
    {
        WaitForSpawn = true;
    }

    public void InitEnvObjects()
    {
        for (int i = 0; i < envObjects.Length; i++)
        {
            if (envObjects[i] == null)
                continue;

            envObjects[i].Init(-1, eCharacterType.Other, null);
            envObjects[i].Activate();
        }
    }

	public virtual int GetItemHasEnvObjectCount() {
		if( envObjects == null ) {
			return 0;
		}

		int count = 0;

		for( int i = 0; i < envObjects.Length; i++ ) {
            if( envObjects[i] == null ) {
                continue;
			}

			if( envObjects[i].AllDropItemID > 0 || envObjects[i].RandomDropItemID > 0 ) {
				++count;
			}
		}

		return count;
	}

	public virtual void Clear()
    {
        m_listEnemy.Clear();
    }

	public virtual void Active() {
		gameObject.SetActive( true );
	}

	public virtual void Deactive() {
		gameObject.SetActive( false );
	}

	public void ClearEnvObjects()
	{
		if (envObjects != null && envObjects.Length > 0)
		{
			for (int i = 0; i < envObjects.Length; ++i)
			{
				if(envObjects[i] == null || envObjects[i].gameObject == null)
				{
					continue;
				}

				Destroy(envObjects[i].gameObject);
				Destroy(envObjects[i]);

				envObjects[i] = null;
			}
			envObjects = null;
		}

		for(int i = 0; i < m_listEnemy.Count; ++i)
		{
			if (m_listEnemy[i] == null || m_listEnemy[i].gameObject == null)
			{
				continue;
			}

			listEnemy[i].SetDie();
			listEnemy[i].Deactivate();

			Destroy(m_listEnemy[i].gameObject);
			Destroy(m_listEnemy[i]);

			m_listEnemy[i] = null;
		}
		m_listEnemy.Clear();

		for (int i = 0; i < m_listAllUnit.Count; ++i)
		{
			m_listAllUnit[i] = null;
		}
		m_listAllUnit.Clear();

		for (int i = 0; i < mListTemp.Count; ++i)
		{
			mListTemp[i] = null;
		}
		mListTemp.Clear();
	}

    public Enemy CreateEnemy(GameClientTable.Monster.Param param)
    {
        string platform = (AppMgr.Instance.ResPlatform == AppMgr.eResPlatform.aos && param.Platform == 1) ? "_aos" : "";
        string path = Utility.AppendString("Unit/", param.ModelPb + platform, ".prefab");

        Enemy enemy = ResourceMgr.Instance.CreateFromAssetBundle<Unit>("unit", path) as Enemy;//m_unitPool.AddUnit(param.ID, path) as Enemy;
        enemy.Init(param.ID, eCharacterType.Monster, null);
        enemy.Deactivate();

        if (param.Child > 0)
        {
            GameClientTable.Monster.Param paramChild = GameInfo.Instance.GetMonsterData(param.Child);
            if (paramChild == null)
            {
                Debug.LogError(paramChild.ID + "번은 존재 하지 않는 몬스터 테이블 아이디 입니다.");
                return null;
            }

            Enemy child = CreateEnemy(paramChild);
            child.SetParent(enemy);

            enemy.SetChild(child);
        }

        m_listEnemy.Add(enemy);
        return enemy;
    }

    public Enemy CreateEnemyWithoutAddEnemy(GameClientTable.Monster.Param param)
    {
        string platform = (AppMgr.Instance.ResPlatform == AppMgr.eResPlatform.aos && param.Platform == 1) ? "_aos" : "";
        string path = Utility.AppendString("Unit/", param.ModelPb + platform, ".prefab");

        Enemy enemy = ResourceMgr.Instance.CreateFromAssetBundle<Unit>("unit", path) as Enemy;////m_unitPool.AddUnit(param.ID, path) as Enemy;
        enemy.Init(param.ID, eCharacterType.Monster, null);
        enemy.Deactivate();

        if (param.Child > 0)
        {
            GameClientTable.Monster.Param paramChild = GameInfo.Instance.GetMonsterData(param.Child);
            if (paramChild == null)
            {
                Debug.LogError(paramChild.ID + "번은 존재 하지 않는 몬스터 테이블 아이디 입니다.");
                return null;
            }

            Enemy child = CreateEnemy(paramChild);
            child.SetParent(enemy);

            enemy.SetChild(child);
        }

        return enemy;
    }

    /*
    public bool IsHoldSpawn()
    {
        return m_unitPool.SpawnUnitCount >= MaxSpawnMonsterCount;
    }
    */

    public void DeactivateEnemy()//Enemy enemy)
    {
        //m_unitPool.ReturnToPool(enemy);
        CurSpawnMonsterCount = Mathf.Clamp(CurSpawnMonsterCount - 1, 0, 100);
    }
    
    public virtual void AddEnemy(Unit enemy)
    {
        Unit find = m_listEnemy.Find(x => x == enemy);
        if (find != null)
            return;

        m_listEnemy.Add(enemy);
    }

    public virtual void AddStageBOSet(int stageBOSetGroupId)
    {
    }

    public virtual void AddStageBOSet( GameClientTable.StageBOSet.Param param ) {
    }

    public void SpawnEnemy()
    {
        WaitForSpawn = false;
        ++CurSpawnMonsterCount;
    }

    public void SkipSpawnEnemy()
    {
        WaitForSpawn = false;
    }

    public bool IsHoldSpawn()
    {
        if(MaxSpawnMonsterCount <= 0)
        {
            return false;
        }

        return CurSpawnMonsterCount >= MaxSpawnMonsterCount;
    }

    public virtual bool IsEmptyEnemy(Unit mine)
    {
        List<Unit> list = GetActiveEnemies(mine);
        return list.Count <= 0;
    }

    public virtual void PauseAll(Unit exceptUnit = null)
    {
        for (int i = 0; i < m_listEnemy.Count; i++)
        {
            if (m_listEnemy[i].IsActivate() == false || exceptUnit == m_listEnemy[i])
                continue;

            m_listEnemy[i].Pause();
        }
    }

    public virtual void ResumeAll()
    {
        for (int i = 0; i < m_listEnemy.Count; i++)
        {
            if (m_listEnemy[i].IsActivate() == false)
                continue;

            m_listEnemy[i].Resume();
        }
    }

    public virtual void DieAll()
    {
        for (int i = 0; i < m_listEnemy.Count; i++)
        {
            if (m_listEnemy[i].IsActivate() == false)
                continue;

            m_listEnemy[i].actionSystem.CancelCurrentAction();
            m_listEnemy[i].CommandAction(eActionCommand.Die, null);
        }
    }

    public virtual void ShowAll(bool show)
    {
        for (int i = 0; i < m_listEnemy.Count; i++)
        {
            if (m_listEnemy[i].IsActivate() == false)
                continue;

            m_listEnemy[i].ShowMesh(show);
        }
    }

    public virtual void SetSpeedRateAll(float aniSpeedRate)
    {
        for (int i = 0; i < m_listEnemy.Count; i++)
        {
            if (m_listEnemy[i].IsActivate() == false)
                continue;

            m_listEnemy[i].SetSpeedRate(aniSpeedRate);
        }
    }

    public virtual List<Unit> GetActiveEnvObjects()
    {
        mListTemp.Clear();

        BattleAreaManager battleAreaMgr = World.Instance.EnemyMgr as BattleAreaManager;
        if(battleAreaMgr)
        {
            BattleArea battleArea = battleAreaMgr.GetCurrentBattleArea();
            if(battleArea)
            {
                return battleArea.GetActiveEnvObjects();
            }
        }

        return mListTemp;
    }

	public virtual List<Unit> GetActiveEnemies( Unit mine ) {
		listFindForActiveEnemies.Clear();

        PlayerGuardian playerGuardian = mine as PlayerGuardian;

		for ( int i = 0; i < m_listEnemy.Count; i++ ) {
			if ( m_listEnemy[i] == null ) {
				continue;
			}

			if ( playerGuardian && playerGuardian.OwnerPlayer && playerGuardian.OwnerPlayer == m_listEnemy[i] ) {
				continue;
			}

			if ( m_listEnemy[i] != mine && m_listEnemy[i].IsActivate() && m_listEnemy[i].curHp > 0.0f && !m_listEnemy[i].ignoreHit ) {
				listFindForActiveEnemies.Add( m_listEnemy[i] );

				List<PlayerMinion> list = m_listEnemy[i].GetAllActiveMinion();
				if ( list.Count > 0 ) {
					listFindForActiveEnemies.AddRange( list );
				}
			}
		}

		if ( mine && mine.isClone ) {
			listFindForActiveEnemies2.Clear();

			for ( int i = 0; i < listFindForActiveEnemies.Count; i++ ) {
				if ( listFindForActiveEnemies[i] == null ) {
					continue;
				}

				if ( listFindForActiveEnemies[i] != mine.cloneOwner ) {
					listFindForActiveEnemies2.Add( listFindForActiveEnemies[i] );

					List<PlayerMinion> list = listFindForActiveEnemies[i].GetAllActiveMinion();
					if ( list.Count > 0 ) {
						listFindForActiveEnemies2.AddRange( list );
					}
				}
			}

			return listFindForActiveEnemies2;
		}

		return listFindForActiveEnemies;
	}

	public virtual List<Unit> GetNearActiveEnemies( Unit mine, float minDist, Vector3 playerPos ) {
		listFindForNearActiveEnemies.Clear();

		for( int i = 0; i < m_listEnemy.Count; i++ ) {
			if( m_listEnemy[i] != mine && m_listEnemy[i].IsActivate() && m_listEnemy[i].curHp > 0f && !m_listEnemy[i].ignoreHit && 
                Vector3.Distance( playerPos, m_listEnemy[i].transform.position ) <= minDist ) {
				listFindForNearActiveEnemies.Add( m_listEnemy[i] );
			}
		}

		if( mine && mine.isClone ) {
			listFindForNearActiveEnemies2.Clear();

			for( int i = 0; i < listFindForNearActiveEnemies.Count; i++ ) {
				if( listFindForNearActiveEnemies[i] != mine.cloneOwner ) {
					listFindForNearActiveEnemies2.Add( listFindForNearActiveEnemies[i] );
				}
			}

			return listFindForNearActiveEnemies2;
		}

		return listFindForNearActiveEnemies;
	}

	public virtual List<Unit> GetMarkingEnemies( Unit.eMarkingType markingType ) {
		listFindMarkingEnemies.Clear();

		for( int i = 0; i < m_listEnemy.Count; i++ ) {
			Unit enemy = m_listEnemy[i];

			if( enemy == null || !enemy.IsActivate() || enemy.curHp <= 0.0f || enemy.ignoreHit || enemy.MarkingInfo.MarkingType != markingType ) {
				continue;
			}

			listFindMarkingEnemies.Add( enemy );
		}

		return listFindMarkingEnemies;
	}

	public virtual List<Unit> GetAllActiveUnit(Unit mine)
    {
        m_listAllUnit.Clear();

        m_listAllUnit.AddRange(GetActiveEnemies(mine));
        m_listAllUnit.AddRange(GetActiveEnvObjects());

        return m_listAllUnit;
    }

    public virtual List<Unit> GetAllUnit(Unit mine)
    {
        m_listAllUnit.Clear();

        for(int i = 0; i < m_listEnemy.Count; i++)
        {
            if(m_listEnemy[i] == mine)
            {
                continue;
            }

            m_listAllUnit.Add(m_listEnemy[i]);
        }

        m_listAllUnit.AddRange(GetActiveEnvObjects());
        return m_listAllUnit;
    }

    public virtual Enemy GetDeactivateEnemy(int tableId)
    {
        Unit find = m_listEnemy.Find(x => x.tableId == tableId && !x.IsActivate());// && !x.usedInPool);
        if (find == null)
        {
            Debug.LogError("풀에 대기중인 " + tableId + "번 몬스터가 존재하지 않습니다.");
            return null;
        }

        EffectManager.Instance.StopAllByParent(find.transform);
        return find as Enemy;
    }

    public virtual Unit GetNearestTarget(Unit mine, bool onlyEnemy, bool skipHasShieldTarget = false, bool onlyAir = false, float vision = 0.0f)//, bool checkPlayerAir = true)
    {
        List<Unit> list = !onlyEnemy ? GetAllActiveUnit(mine) : GetActiveEnemies(mine);
        SortListTargetByNearDistance(mine, ref list);

        Unit target = null;
        for (int i = 0; i < list.Count; i++)
        {
            Unit enemy = list[i];
            if (enemy == null || enemy.IsActivate() == false || enemy.curHp <= 0.0f || enemy.ignoreHit)
            {
                continue;
            }

            if (skipHasShieldTarget && (enemy.curShield > 0.0f || enemy.CurrentSuperArmor >= Unit.eSuperArmor.Lv1))
            {
                continue;
            }
            else if (onlyAir && enemy.isGrounded)
            {
                continue;
            }

            if(vision > 0.0f)
            {
                if (!mine.IsTargetInAttackRange(enemy.MainCollider, eAttackDirection.Front, 0.0f, vision))
                {
                    continue;
                }
            }

            target = enemy;
            break;
        }

        return target;
    }

    public virtual List<Unit> GetNearestTargetList( Unit mine, bool onlyEnemy, bool skipHasShieldTarget = false, bool onlyAir = false, float vision = 0.0f )
    {
        List<Unit> list = !onlyEnemy ? GetAllActiveUnit(mine) : GetActiveEnemies(mine);
        SortListTargetByNearDistance( mine, ref list );

        return list;
    }

    public virtual Unit GetNearestTarget(Unit unit, List<Unit> listTarget)
    {
        SortListTargetByNearDistance(unit, ref listTarget);

        Unit nearestTarget = null;
        for (int i = 0; i < listTarget.Count; i++)
        {
            Unit target = listTarget[i];
            if (target == null || target.IsActivate() == false || target.curHp <= 0.0f)
                continue;

            if (!target.alwaysKinematic && !unit.isGrounded && target.isGrounded == true)
                continue;

            nearestTarget = target;
            break;
        }

        return nearestTarget;
    }

    public virtual Unit GetNearestTarget(Unit mine, float dist, float angle)
    {
        List<Unit> list = GetActiveEnemies(mine);
        SortListTargetByNearDistance(mine, ref list);

        Unit target = null;
        for (int i = 0; i < list.Count; i++)
        {
            Unit enemy = list[i];
            if (enemy == null || enemy.IsActivate() == false || enemy.curHp <= 0.0f)
                continue;

            if (!enemy.alwaysKinematic && !mine.isGrounded && enemy.isGrounded == true)
                continue;

            float d = mine.GetDistance(enemy); //Vector3.Distance(player.transform.position, enemy.transform.position);
            float a = Vector3.Angle(mine.transform.forward, (enemy.transform.position - mine.transform.position).normalized);

            if (d > dist || a > angle)
                continue;

            target = enemy;
            break;
        }

        return target;
    }

	public virtual Unit GetNearestTargetForEnvObject( Unit mine, float dist, float angle ) {
		List<Unit> list = GetActiveEnvObjects();
		SortListTargetByNearDistance( mine, ref list );

		Unit target = null;
		for ( int i = 0; i < list.Count; i++ ) {
			Unit enemy = list[i];
			if ( enemy == null || enemy.IsActivate() == false || enemy.curHp <= 0.0f )
				continue;

			if ( !enemy.alwaysKinematic && !mine.isGrounded && enemy.isGrounded == true )
				continue;

			float d = mine.GetDistance( enemy ); //Vector3.Distance(player.transform.position, enemy.transform.position);
			float a = Vector3.Angle( mine.transform.forward, ( enemy.transform.position - mine.transform.position ).normalized );

			if ( d > dist || a > angle )
				continue;

			target = enemy;
			break;
		}

		return target;
	}

    public virtual Unit GetEvadedTarget(Unit mine)
    {
        List<Unit> list = GetActiveEnemies(mine);

        Unit target = null;
        float compare = 9999.0f;
        for (int i = 0; i < list.Count; i++)
        {
            Unit enemy = list[i];
            if (enemy == null || enemy.IsActivate() == false || enemy.curHp <= 0.0f || enemy.withoutAniEvent)
                continue;

            AniEvent.sEvent evt = enemy.aniEvent.IsPossibleToEvade(enemy.aniEvent.curAniType);
            if (evt == null)
                continue;

            float range = evt.atkRange;
            if (evt.hitBoxSize != Vector3.zero)
                range = evt.hitBoxSize.magnitude;

            bool isInEnemyAttackRange = enemy.IsTargetInAttackRange(mine.MainCollider, eAttackDirection.Front, 0.0f, range);
            if (isInEnemyAttackRange == false)
                continue;

            float dist = mine.GetDistance(enemy); //Vector3.Distance(player.transform.position, enemy.transform.position);
            if (dist < compare)
            {
                target = enemy;
                compare = dist;
            }
        }

        return target;
    }

    public bool HasFloatingEnemy()
    {
        if(World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
        {
            return true;
        }

        ActionHit actionHit = null;
        for(int i = 0; i < m_listEnemy.Count; i++)
        {
            Unit unit = m_listEnemy[i];
            if(unit.curHp <= 0.0f || !unit.IsActivate() || unit.actionSystem.currentAction == null)
            {
                continue;
            }

            actionHit = unit.actionSystem.GetCurrentAction<ActionHit>();
            if(actionHit == null)
            {
                continue;
            }

            if(actionHit.State == ActionHit.eState.Float)
            {
                return true;
            }
        }

        return false;
    }

    public Vector3 GetCenterPosOfEnemies(Unit mine)
    {
        Vector3 center = Vector3.zero;

        List<Unit> listActiveEnemy = GetActiveEnemies(mine);
        for (int i = 0; i < listActiveEnemy.Count; i++)
        {
            center += listActiveEnemy[i].transform.position;
        }

        return center /= listActiveEnemy.Count;
    }

    public void SortListTargetByNearDistance(Unit unit, ref List<Unit> listTarget)
    {
        for (int i = 0; i < listTarget.Count; i++)
        {
            Unit target = listTarget[i];
            target.compareScore = unit.GetDistance(target);

            Vector3 unitInputDir = unit.AI == null ? Vector3.zero : unit.AI.GetDirection();
            Vector3 unitDir = unitInputDir == Vector3.zero ? unit.transform.forward : unitInputDir;

            float angle = Vector3.Angle(unitDir, (target.transform.position - unit.transform.position).normalized) / 45.0f;
            target.compareScore += angle;

            if (!target.isVisible)
                target.compareScore += 10.0f;

            if (target.gameObject.layer == (int)eLayer.EnvObject)
            {
                target.compareScore += 10.0f;
            }
        }

        // 거리와 각도로 정해진 targetScore가 가장 작은 타겟으로 최종 정렬 
        listTarget.Sort(delegate (Unit lhs, Unit rhs)
        {
            if (lhs.compareScore > rhs.compareScore)
            {
                return 1;
            }
            else if (lhs.compareScore < rhs.compareScore)
            {
                return -1;
            }

            return 0;
        });

        // AggroValue가 높은 유닛으로 최종 정렬 
        listTarget.Sort(delegate (Unit lhs, Unit rhs)
        {
            if (lhs.AggroValue < rhs.AggroValue)
            {
                return 1;
            }
            else if (lhs.AggroValue > rhs.AggroValue)
            {
                return -1;
            }

            return 0;
        });
    }

    public void SortListTargetByNearDistance(Unit unit, ref List<UnitCollider> listTarget)
    {
        if(listTarget == null || listTarget.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < listTarget.Count; i++)
        {
            Unit target = listTarget[i].Owner;
            target.compareScore = unit.GetDistance(target);

            Vector3 unitInputDir = unit.AI == null ? Vector3.zero : unit.AI.GetDirection();
            Vector3 unitDir = unitInputDir == Vector3.zero ? unit.transform.forward : unitInputDir;

            float angle = Vector3.Angle(unitDir, (target.transform.position - unit.transform.position).normalized) / 45.0f;
            target.compareScore += angle;

            if (!target.isVisible)
                target.compareScore += 10.0f;

            if (target.gameObject.layer == (int)eLayer.EnvObject)
            {
                target.compareScore += 10.0f;
            }
        }

        // 거리와 각도로 정해진 targetScore가 가장 작은 타겟으로 최종 정렬 
        listTarget.Sort(delegate (UnitCollider lhs, UnitCollider rhs)
        {
            if (lhs.Owner.compareScore > rhs.Owner.compareScore)
            {
                return 1;
            }
            else if (lhs.Owner.compareScore < rhs.Owner.compareScore)
            {
                return -1;
            }

            return 0;
        });

        // AggroValue가 높은 유닛으로 최종 정렬 
        listTarget.Sort(delegate (UnitCollider lhs, UnitCollider rhs)
        {
            if (lhs.Owner.AggroValue < rhs.Owner.AggroValue)
            {
                return 1;
            }
            else if (lhs.Owner.AggroValue > rhs.Owner.AggroValue)
            {
                return -1;
            }

            return 0;
        });
    }

    public virtual void ChangeAllEnemiesLayer(eLayer layer, Unit.eGrade exceptGrade)
    {
        for (int i = 0; i < m_listEnemy.Count; i++)
        {
            if (m_listEnemy[i].grade == exceptGrade)
                continue;

            Utility.ChangeLayersRecursively(m_listEnemy[i].transform, layer);
        }
    }

    public bool IsAllMonsterDeactive()
    {
        for (int i = 0; i < m_listEnemy.Count; i++)
        {
            if (m_listEnemy[i].IsActivate() == true)
                return false;
        }

        return true;
    }

    public virtual bool IsAllMonsterDie()
    {
        for (int i = 0; i < m_listEnemy.Count; i++)
        {
            if (m_listEnemy[i].curHp > 0.0f)
            {
                return false;
            }
        }

        return true;
    }

	public bool HasAliveMonster()
	{
		List<Unit> list = GetActiveEnemies(null);
		if(list == null || list.Count <= 0)
		{
			return false;
		}

		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].curHp > 0.0f)
			{
				return true;
			}
		}

		return false;
	}

	public void AddDropItem(DropItem item)
    {
        DropItem find = m_listCurDropItem.Find(x => x == item);
        if (find)
            return;

        m_listCurDropItem.Add(item);
    }

    public void CollectDropItem()
    {
        if (m_listCurDropItem.Count <= 0)
            return;

        for (int i = 0; i < m_listCurDropItem.Count; i++)
            m_listCurDropItem[i].MoveToPlayer();

        m_listCurDropItem.Clear();
    }

    public virtual void GetStartPoint(out Vector3 pos, out Quaternion rot)
    {
        pos = m_startPoint.transform.position;
        rot = m_startPoint.transform.rotation;
    }

    public virtual InGameCamera.sDefaultSetting GetCurAreaDefaultCameraSettingOrNull()
    {
        return null;
    }

    public virtual Transform[] GetEdgePoints()
    {
        return null;
    }

    public virtual void AllEnemyRetarget()
    {
        List<Unit> list = GetActiveEnemies(null);
        for(int i = 0; i < list.Count; i++)
        {
            Enemy enemy = list[i] as Enemy;
            if(enemy == null)
            {
                continue;
            }

            enemy.Retarget();
        }
    }

    public void RemoveHitTarget( Unit target ) {
        for ( int i = 0; i < m_listEnemy.Count; i++ ) {
            if ( m_listEnemy[i].IsActivate() ) {
                m_listEnemy[i].RemoveHitTarget( target );
            }
		}
	}

#if UNITY_EDITOR
	protected virtual void OnDrawGizmos() {
		string label = string.Empty;
        Vector3 headPos = Vector3.zero;

		List<Unit> list = GetActiveEnemies(null);
		for( int i = 0; i < list.Count; i++ ) {
			if( list[i].mainTarget == null ) {
				continue;
			}

            headPos = list[i].GetHeadPos();

            // 타겟 네임
            label = "타겟 : " + ( string.IsNullOrEmpty( list[i].mainTarget.tableName ) ? list[i].mainTarget.name : list[i].mainTarget.tableName );
            UnityEditor.Handles.Label( headPos, label );

            /*/ 경화 중첩 수
            if( list[i].cmptBuffDebuff ) {
                headPos.y += 0.5f;
                label = "경화 중첩 수 : " + list[i].cmptBuffDebuff.GetStackCountByIconType( eBuffIconType.Debuff_Temp4 ).ToString();
                UnityEditor.Handles.Label( headPos, label );
            }
            */
		}
	}
#endif
}
