
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleSpawnPoint : MonoBehaviour
{
    [Header("[Monster]")]
    public float    startDealy          = 0.0f;
    public float    spawnIntervalTime   = 0.0f;
    public int      maxMonsterCount     = 1;
    public int      kMonsterID          = 1;

    [Header("[Gate]")]
    public int          GateObjTableId  = 0;
    public Transform    GateObjPos      = null;

    public Enemy        kMonsterObj     { get; private set; }   = null;
    public List<Enemy>  ListSpawnEnemy  { get; private set; }   = new List<Enemy>();
    public EnemyGate    GateObj         { get; private set; }   = null;

    private bool mbSpawned = false;


    public bool Init()
    {
        kMonsterObj = World.Instance.EnemyMgr.GetDeactivateEnemy(kMonsterID);
		if(kMonsterObj == null)
		{
			return false;
		}

        kMonsterObj.SetData(kMonsterID);
        kMonsterObj.Activate();

        ListSpawnEnemy.Clear();

        CreateGateObj();
		return true;
    }

    public void Spawn()
    {
        if (World.Instance.IsEndGame == true)
        {
            return;
        }

        if(GateObj)
        {
            GateObj.Deactivate();
        }

        StartCoroutine("DelaySpawn", startDealy);
    }

    public int GetMaxMonsterCount()
    {
        if (spawnIntervalTime <= 0.0f)
        {
            return 1;
        }

        return maxMonsterCount == -1 ? World.Instance.EnemyMgr.MaxSpawnMonsterCount : maxMonsterCount;
    }

    public bool IsAllMonsterDie()
    {
        /*
        if (maxMonsterCount == -1 && GateObj.curHp <= 0.0f)
        {
            return true;
        }
        
        if (ListSpawnEnemy.Count < GetMaxMonsterCount())
        {
            return false;
        }
        */

        if(!mbSpawned)
        {
            return false;
        }

        bool isAllDieSpawnEnemy = true;
        for (int i = 0; i < ListSpawnEnemy.Count; i++)
        {
            ActionEnemyBase action = null;
            if(ListSpawnEnemy[i].actionSystem && ListSpawnEnemy[i].actionSystem.currentAction)
            {
                action = ListSpawnEnemy[i].actionSystem.GetCurrentAction<ActionEnemyBase>();
            }

            if ((ListSpawnEnemy[i].curHp > 0.0f && ListSpawnEnemy[i].IsActivate()) || action != null)
            {
                isAllDieSpawnEnemy = false;
                break;
            }
        }

        if (isAllDieSpawnEnemy && ListSpawnEnemy.Count > 0)
        {
            if ((GateObj && GateObj.curHp > 0.0f) || (GateObj == null && maxMonsterCount == -1))
            {
                isAllDieSpawnEnemy = false;
            }
        }

        return isAllDieSpawnEnemy;
    }

	public bool IsAllMonsterHpIsZero() {
		if( !mbSpawned ) {
			return false;
		}

		bool isAllDieSpawnEnemy = true;

		for( int i = 0; i < ListSpawnEnemy.Count; i++ ) {
			if( ListSpawnEnemy[i].curHp > 0.0f ) {
				isAllDieSpawnEnemy = false;
				break;
			}
		}

		if( isAllDieSpawnEnemy && ListSpawnEnemy.Count > 0 ) {
			if( ( GateObj && GateObj.curHp > 0.0f ) || ( GateObj == null && maxMonsterCount == -1 ) ) {
				isAllDieSpawnEnemy = false;
			}
		}

		return isAllDieSpawnEnemy;
	}

	public void ChangeEnemy(Enemy enemy)
    {
        kMonsterObj = enemy;
    }

    public void SetSpawned()
    {
        mbSpawned = true;
    }

    private void CreateGateObj()
    {
        if(GateObjTableId <= 0)
        {
            return;
        }

        GateObj = World.Instance.EnemyMgr.GetDeactivateEnemy(GateObjTableId) as EnemyGate;
        GateObj.SetData(GateObjTableId);
        GateObj.SetSpawnPoint(this);
        GateObj.Activate();

        if(GateObjPos)
        {
            GateObj.SetInitialPosition(GateObjPos.position, GateObjPos.rotation);
        }
        else
        {
            GateObj.SetInitialPosition(transform.position, transform.rotation);
        }
    }

    private IEnumerator DelaySpawn(float delay)
    {
        float checkTime = 0.0f;

        if (GateObj && !GateObj.IsActivate())
        {
            GateObj.Activate();
            float aniLength = GateObj.PlayAniImmediate(eAnimation.Appear);

            while (checkTime < aniLength)
            {
                if (!Director.IsPlaying)
                {
                    checkTime += Time.deltaTime;
                }

                yield return null;
            }

            GateObj.PlayAni(eAnimation.Idle01);
        }

        checkTime = 0.0f;
        while(checkTime < delay)
        {
            if (!World.Instance.EnemyMgr.IsHoldSpawn())
            {
                checkTime += Time.deltaTime;
            }

            yield return null;
        }

        StartSpawn();
    }

    private void StartSpawn()
    {
        if (World.Instance.IsEndGame || (World.Instance.ClearMode == World.eClearMode.BossDie && World.Instance.IsBossDead))
        {
            return;
        }

        kMonsterObj = World.Instance.EnemyMgr.GetDeactivateEnemy(kMonsterID);
		if(kMonsterObj == null)
		{
			Debug.LogError("비활성화된 " + kMonsterID + "번 몬스터가 없어서 스폰 못함.");
			return;
		}

        kMonsterObj.SetData(kMonsterID);
        kMonsterObj.ShowMesh(true);
        kMonsterObj.myBattleSpawnPoint = this;
        kMonsterObj.SetGroundedRigidBody();

        Vector3 startPos = transform.position;
        if(spawnIntervalTime > 0.0f)
        {
            Vector2 rand = Vector2.zero;
            rand.x = ((float)Random.Range(0, 5) * 0.5f);
            if(Random.Range(0, 2) == 0)
            {
                rand.x *= -1.0f;
            }

            rand.y = (float)Random.Range(0, 5) * 0.5f;
            if (Random.Range(0, 2) == 0)
            {
                rand.y *= -1.0f;
            }

            startPos.x += rand.x;
            startPos.z += rand.y;
        }

        kMonsterObj.SetInitialPosition(startPos, transform.rotation);

        Activate();
        ListSpawnEnemy.Add(kMonsterObj);

        World.Instance.EnemyMgr.SpawnEnemy();

        if (spawnIntervalTime > 0.0f && !mbSpawned)
        {
            StartCoroutine("ReSpawn");
        }

        mbSpawned = true;
    }

	private void Activate() {
		Unit enemyTarget = null;
		
        BTBase.eTargetOnStart targetOnStart = kMonsterObj.GetTargetOnStart();
		if( targetOnStart == BTBase.eTargetOnStart.Player ) {
			UnitCollider targetCollider = kMonsterObj.GetMainTargetCollider( true );
			enemyTarget = ( targetCollider == null || targetCollider.Owner == null ) ? World.Instance.Player : targetCollider.Owner;

			kMonsterObj.LockTarget = false;
		}
		else if( targetOnStart == BTBase.eTargetOnStart.OtherObject ) {
			enemyTarget = World.Instance.EnemyMgr.otherObject;
			kMonsterObj.LockTarget = true;
		}

		kMonsterObj.SetMainTarget( enemyTarget );
		kMonsterObj.Activate();
		kMonsterObj.actionSystem.CancelCurrentAction();
		kMonsterObj.CommandAction( eActionCommand.Appear, null );

		World.Instance.Player.SetEnemyNavigatorTarget( kMonsterObj );
	}

	private IEnumerator ReSpawn()
    {
        float checkTime = 0.0f;
        while (true)
        {
            if (checkTime < spawnIntervalTime)
            {
                if (!World.Instance.EnemyMgr.IsHoldSpawn())
                {
                    checkTime += Time.fixedDeltaTime;
                }
            }
            else
            {
                checkTime = 0.0f;

                if(maxMonsterCount == -1 && GateObj && (!GateObj.IsActivate() || GateObj.curHp <= 0.0f))
                {
                    break;
                }

                if ((maxMonsterCount > 0 && ListSpawnEnemy.Count >= GetMaxMonsterCount()) || World.Instance.IsEndGame)
                {
                    break;
                }

                if (World.Instance.ClearMode == World.eClearMode.BossDie && World.Instance.IsBossDead)
                {
                    break;
                }

                StartCoroutine("DelaySpawn", 0.0f);
            }

            yield return null;
        }
    }
}
