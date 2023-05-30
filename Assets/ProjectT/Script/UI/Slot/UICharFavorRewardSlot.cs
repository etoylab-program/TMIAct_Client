using UnityEngine;
using System.Collections;

public class UICharFavorRewardSlot : FSlot 
{
	public UIItemListSlot kItemListSlot;
	public UISprite kFavorLevelSpr;
	public UILabel kFavorLevelLabel;
	public UISprite kFavorSpr;
	public GameObject kGet;
	public UISprite kCompleteSpr;
	public UIButton kRewardBtn;

	private int _index;
	private GameTable.LevelUp.Param _data;
	private GameTable.Random.Param _randomData;

	public void UpdateSlot(int index, GameTable.LevelUp.Param data) 	//Fill parameter if you need
	{
		_index = index;
		_data = data;

		kFavorLevelLabel.textlocalize = FLocalizeString.Instance.GetText(211, _data.Level);
		
		_randomData = GameInfo.Instance.GameTable.FindRandom(x => x.GroupID == _data.Value1);
		
		kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, _index, _randomData);
	}

	public void SetRewardButton(bool enable)
	{
		kRewardBtn.isEnabled = enable;
	}

	public void SetReward(bool bComplete = false)
	{
		kGet.SetActive(false);
		kCompleteSpr.gameObject.SetActive(bComplete);
	}
	
	public void OnClick_Slot()
	{
		int itemId = -1;
		int lobbyAnimId = -1;
		int randomId = -1;
		switch (_randomData.ProductType)
		{
			case 1: // Diamond
				return;
			case 7: // Item
				itemId = _randomData.ProductIndex;
				break;
			case 19: // Animation
				randomId = _randomData.GroupID;
				lobbyAnimId = _randomData.ProductIndex;
				break;
		}
		
		UIValue.Instance.SetValue(UIValue.EParamType.ItemUID, (long)-1);
		UIValue.Instance.SetValue(UIValue.EParamType.ItemTableID, itemId);
		UIValue.Instance.SetValue(UIValue.EParamType.RandomTableID, randomId);
		UIValue.Instance.SetValue(UIValue.EParamType.LobbyAnimTableID, lobbyAnimId);
		UIValue.Instance.SetValue(UIValue.EParamType.ItemTableCount, _randomData.Value);
		LobbyUIManager.Instance.ShowUI("ItemInfoPopup", true);
	}
}
