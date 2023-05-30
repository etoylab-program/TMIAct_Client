
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HUDResMgr : MonoSingleton<HUDResMgr>
{
	private List<UIDamageText>		mListDamageText		= new List<UIDamageText>();
	private List<UIRecoveryText>	mListRecoveryText	= new List<UIRecoveryText>();


	public void LoadHUD()
	{
		if(World.Instance.UIPlay == null || World.Instance.EnemyMgr == null)
		{
			return;
		}

		int count = 1;
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

		// Damage Text
		mListDamageText.Clear();
		mListDamageText.Capacity = count * 5;

		for (int i = 0; i < mListDamageText.Capacity; ++i)
		{
			UIDamageText ui = ResourceMgr.Instance.CreateFromAssetBundle<UIDamageText>("ui", "UI/DamageText.prefab");
			if (ui == null)
			{
				continue;
			}

			ui.name = "DamageText_" + i.ToString();
			ui.transform.SetParent(World.Instance.StageType != eSTAGETYPE.STAGE_PVP ? World.Instance.UIPlay.transform : World.Instance.UIPVP.transform);
			Utility.InitTransform(ui.gameObject);

			mListDamageText.Add(ui);
		}

		// Recovery Text
		mListRecoveryText.Clear();
		mListRecoveryText.Capacity = count * 5;

		for (int i = 0; i < mListRecoveryText.Capacity; ++i)
		{
			UIRecoveryText ui = ResourceMgr.Instance.CreateFromAssetBundle<UIRecoveryText>("ui", "UI/RecoveryText.prefab");
			if (ui == null)
			{
				continue;
			}

			ui.name = "RecoveryText_" + i.ToString();
			ui.transform.SetParent(World.Instance.StageType != eSTAGETYPE.STAGE_PVP ? World.Instance.UIPlay.transform : World.Instance.UIPVP.transform);
			Utility.InitTransform(ui.gameObject);

			mListRecoveryText.Add(ui);
		}
	}

	public void ShowDamage(Unit owner, bool isPlayer, bool isCritical, int damage)
	{
		UIDamageText find = mListDamageText.Find(x => x.IsHide);
		if(find == null)
		{
			find = mListDamageText.Find(x => x.Parent == owner);
			if(find == null)
			{
				return;
			}
		}

		find.ShowDamage(owner, isPlayer, isCritical, damage);
	}

	public void HideAllDamageText(Unit owner)
	{
		List<UIDamageText> findAll = mListDamageText.FindAll(x => x.Parent == owner);
		for(int i = 0; i < findAll.Count; ++i)
		{
			findAll[i].HideDamage();
		}
	}

	public void ShowRecovery(Unit owner, bool isPlayer, int recovery)
	{
		UIRecoveryText find = mListRecoveryText.Find(x => x.IsHide);
		if (find == null)
		{
			find = mListRecoveryText.Find(x => x.Parent == owner);
			if (find == null)
			{
				return;
			}
		}

		find.ShowRecovery(owner, isPlayer, recovery);
	}

	public void ChangeParent( Unit owner, Unit newOwner ) {
		for ( int i = 0; i < mListDamageText.Count; i++ ) {
			if ( mListDamageText[i].Parent == owner ) {
				mListDamageText[i].SetParent( newOwner );
			}
		}
	}
}
