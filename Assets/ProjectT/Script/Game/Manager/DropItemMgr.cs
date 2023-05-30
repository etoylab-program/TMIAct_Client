
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DropItemMgr : MonoSingleton<DropItemMgr>
{
	private List<DropItem> mListDropItem = new List<DropItem>();


	public void LoadDropItems()
	{
		int count = 1;
		if (World.Instance.EnemyMgr)
		{
			if (World.Instance.EnemyMgr.MaxSpawnMonsterCount > 0)
			{
				count = Mathf.Min(World.Instance.EnemyMgr.MaxSpawnMonsterCount, World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup);
			}
			else
			{
				count = World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup;
			}

			++count; // 플레이어
			if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
			{
				++count;
			}

			if (World.Instance.EnemyMgr)
			{
				count += World.Instance.EnemyMgr.GetItemHasEnvObjectCount();
			}
		}

		List<GameClientTable.DropItem.Param> listParamDropItem = GameInfo.Instance.GameClientTable.DropItems;//.FindAll( x => x.ID < 101 );

		mListDropItem.Clear();
		mListDropItem.Capacity = count * listParamDropItem.Count * GameInfo.Instance.BattleConfig.MaxDropItemNum;
		
		for (int i = 0; i < listParamDropItem.Count; ++i)
		{
			for (int j = 0; j < GameInfo.Instance.BattleConfig.MaxDropItemNum; ++j)
			{
				for (int k = 0; k < count; ++k)
				{
					DropItem dropItem = ResourceMgr.Instance.CreateFromAssetBundle<DropItem>("item", listParamDropItem[i].ModelPb + ".prefab");
					dropItem.Init(listParamDropItem[i]);
					dropItem.gameObject.SetActive(false);

					mListDropItem.Add(dropItem);
				}
			}
		}

		Log.Show("생성한 총 드랍 아이템 개수는 " + mListDropItem.Count, Log.ColorType.Blue);
	}
	
	public void DropItem(Unit owner)
	{
		Random.InitState(owner.GetInstanceID());

		int allDropItemId = 0;
		int allDropItemValue = 0;
		int randomDropItemId = 0;
		int randomItemValue = 0;

		Enemy enemy = owner as Enemy;
		if (enemy)
		{
			allDropItemId = enemy.data.AllDropItemID;
			allDropItemValue = enemy.data.AllDropItemValue;

			randomDropItemId = enemy.data.RandomDropItemID;
			randomItemValue = enemy.data.RandomItemValue;
		}
		else
		{
			EnvObject envObject = owner as EnvObject;
			if (envObject)
			{
				allDropItemId = envObject.AllDropItemID;
				allDropItemValue = envObject.AllDropItemValue;

				randomDropItemId = envObject.RandomDropItemID;
				randomItemValue = envObject.RandomItemValue;
			}
		}

		// All Drop
		List<GameClientTable.RandomDrop.Param> findAll = GameInfo.Instance.GameClientTable.RandomDrops.FindAll(x => x.GroupID == allDropItemId);
		if (findAll != null && findAll.Count > 0)
		{
			for (int i = 0; i < findAll.Count; ++i)
			{
				for (int j = 0; j < allDropItemValue; ++j)
				{
					int min = findAll[i].DropItemValueMin;
					int max = Mathf.Min(findAll[i].DropItemValueMax, GameInfo.Instance.BattleConfig.MaxDropItemNum);
					int count = Random.Range(min, max + 1);

					List<DropItem> listDropItem = mListDropItem.FindAll(x => !x.IsActive && x.data.ID == findAll[i].DropItemIndex);
					if (listDropItem != null && listDropItem.Count >= count)
					{
						for (int k = 0; k < count; ++k)
						{
							listDropItem[k].Drop(null, owner.GetCenterPos());
						}
					}
				}
			}
		}

		// Random Drop
		findAll = GameInfo.Instance.GameClientTable.RandomDrops.FindAll(x => x.GroupID == randomDropItemId);
		if (findAll != null && findAll.Count > 0)
		{
			for (int i = 0; i < findAll.Count; ++i)
			{
				for (int j = 0; j < randomItemValue; ++j)
				{
					int rand = Random.Range(0, 100);
					int prob = (int)((float)findAll[i].Prob * 0.7f);

					if (rand >= prob)
					{
						continue;
					}

					int min = findAll[i].DropItemValueMin;
					int max = Mathf.Min(findAll[i].DropItemValueMax, GameInfo.Instance.BattleConfig.MaxDropItemNum);
					int count = Random.Range(min, max + 1);

					Log.Show(string.Format("[{0}{1}] 드랍확률 : {2}, 랜덤값 : {3}, 떨군 아이템 ID : {4}({5})",
											owner.tableName, owner.GetInstanceID(), prob, rand, findAll[i].DropItemIndex, count), Log.ColorType.Navy);

					List<DropItem> listDropItem = mListDropItem.FindAll(x => !x.IsActive && x.data.ID == findAll[i].DropItemIndex);
					if (listDropItem != null && listDropItem.Count >= count)
					{
						for (int k = 0; k < count; ++k)
						{
							listDropItem[k].Drop(null, owner.GetCenterPos());
						}
					}
				}
			}
		}
	}

	public void DropItemBySkill(Unit owner, Vector3 pos, int dropItemId, int dropCnt)
	{
		List<DropItem> findAll = mListDropItem.FindAll(x => !x.IsActive && x.data.ID == dropItemId);
		if(findAll == null || findAll.Count <= 0)
		{
			return;
		}

		int max = Mathf.Min(dropCnt, GameInfo.Instance.BattleConfig.MaxDropItemNum);
		if (findAll.Count >= max)
		{
			for (int i = 0; i < max; ++i)
			{
				findAll[i].Drop(owner, pos);
			}
		}
	}
}
