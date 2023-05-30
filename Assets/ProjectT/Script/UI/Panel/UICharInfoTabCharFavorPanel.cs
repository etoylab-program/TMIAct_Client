using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

public class UICharInfoTabCharFavorPanel : FComponent
{
	[Header("UICharInfoTabCharFavorPanel")]
	[SerializeField] private FList charFavorRewardList;
	[SerializeField] private FList charFavorPreferenceStepList;
	
	public UISprite kFavorInfoSpr;
	public UILabel kFavorInfoLabel;
	public GameObject kLock;
	public GameObject kFavorGift;
	public GameObject kFavor;
	public GameObject kHeart;
	public UISprite kHeartSpr;
	public UILabel kCharFavorLevelLabel;
	public UIGaugeUnit kCharFavorGaugeUnit;
	public GameObject kFavorLevel;
	public UIButton kGiftBtn;
	public UIButton kHelpBtn;

	private CharData _charData;
	private GameTable.Character.Param _tableData;
	private GameTable.LevelUp.Param _currentFavorLevelTable;
	private List<GameTable.LevelUp.Param> _charFavorLevelUpList = new List<GameTable.LevelUp.Param>();
	
	private readonly List<GameTable.Item.Param> _charPreferenceStep1List = new List<GameTable.Item.Param>();
	private readonly List<GameTable.Item.Param> _charPreferenceStep2List = new List<GameTable.Item.Param>();
	private readonly List<GameTable.Item.Param> _charPreferenceTotalList = new List<GameTable.Item.Param>();
	
	public override void Awake()
	{
		base.Awake();
		
		if (charFavorRewardList != null)
		{
			charFavorRewardList.EventUpdate = _UpdateCharFavorRewardListSlot;
			charFavorRewardList.EventGetItemCount = _GetCharFavorRewardListElementCount;
			charFavorRewardList.UpdateList();
		}

		if (charFavorPreferenceStepList != null)
		{
			charFavorPreferenceStepList.EventUpdate = _UpdateCharFavorPreferenceStepListSlot;
			charFavorPreferenceStepList.EventGetItemCount = _GetCharFavorPreferenceStepListElementCount;
			charFavorPreferenceStepList.UpdateList();
		}
	}
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
		long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
		int tableId = (int)UIValue.Instance.GetValue(UIValue.EParamType.CharSelTableID);

		_charData = GameInfo.Instance.GetCharData(uid);
		_tableData = GameInfo.Instance.GameTable.FindCharacter(tableId);
		
		_charPreferenceStep1List.Clear();
		_charPreferenceStep2List.Clear();
		_charPreferenceTotalList.Clear();
		
		_charFavorLevelUpList = GameInfo.Instance.GameTable.FindAllLevelUp(x => x.Group == _tableData.PreferenceLevelGroup && x.Value1 > (int)eCOUNT.NONE);

		SetPreferenceStep(_tableData.PreferenceStep1, true);
		SetPreferenceStep(_tableData.PreferenceStep2, false);
		
		_charPreferenceTotalList.AddRange(_charPreferenceStep1List);
		_charPreferenceTotalList.AddRange(_charPreferenceStep2List);
		_charPreferenceTotalList.Sort((x, y) => y.Grade.CompareTo(x.Grade));
		
		GameTable.Buff.Param buff = GameInfo.Instance.GameTable.FindBuff(_tableData.PreferenceBuff);
		if (buff != null)
		{
			kFavorInfoLabel.textlocalize = FLocalizeString.Instance.GetText(buff.Name);
		}

		charFavorPreferenceStepList.RefreshNotMoveAllItem();
		charFavorPreferenceStepList.SpringSetFocus(0, isImmediate: true);
		charFavorPreferenceStepList.ScrollViewCheck();
	}

	public override void Renewal(bool bChildren = false)
	{
		base.Renewal(bChildren);

		RenderTargetChar.Instance.SetCostumeWeapon( _charData, false );
		RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject( RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.InGameOnly, false );
		RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject( RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.LobbyOnly, true );

		kCharFavorLevelLabel.textlocalize = _charData.FavorLevel.ToString();
		_currentFavorLevelTable = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == _tableData.PreferenceLevelGroup && x.Level == _charData.FavorLevel);
		
		float favorMaxExp = _charData.FavorExp;
		float exp = _charData.FavorExp;
		if (_currentFavorLevelTable != null && 0 < _currentFavorLevelTable.Exp)
		{
			GameTable.LevelUp.Param prevFavorLevelTable = null;
			favorMaxExp = _currentFavorLevelTable.Exp;
			if (0 < _currentFavorLevelTable.Level)
			{
				prevFavorLevelTable =
					GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == _currentFavorLevelTable.Group &&
					                                             x.Level == _currentFavorLevelTable.Level - 1);
				favorMaxExp = _currentFavorLevelTable.Exp - prevFavorLevelTable.Exp;
				if (prevFavorLevelTable.Exp <= exp)
				{
					exp -= prevFavorLevelTable.Exp;
				}
			}
		}
		
		float rate = exp / (favorMaxExp <= 0 ? 1 : favorMaxExp);
		string text = FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT, rate * 100);
		
		kLock.SetActive(0 <= _currentFavorLevelTable?.Exp);
		if (!kLock.activeSelf)
		{
			rate = 1;
			text = FLocalizeString.Instance.GetText(221);
		}
		
		kCharFavorGaugeUnit.InitGaugeUnit(rate);
		kCharFavorGaugeUnit.SetText(text);
		
		int focusIndex = _charData.FavorLevel - 1;
		charFavorRewardList.RefreshNotMoveAllItem();
		charFavorRewardList.SpringSetFocus(focusIndex < 0 ? 0 : focusIndex, isImmediate: true);
	}

	private void _UpdateCharFavorRewardListSlot(int index, GameObject slotObject)
	{
		UICharFavorRewardSlot slot = slotObject.GetComponent<UICharFavorRewardSlot>();
		if (null == slot)
		{
			return;
		}
		
		GameTable.LevelUp.Param data = null;
		if (0 <= index && _charFavorLevelUpList.Count > index)
		{
			data = _charFavorLevelUpList[index];
			slot.SetReward(data.Level <= _charData.FavorLevel);
		}
		
		slot.ParentGO = gameObject;
		slot.UpdateSlot(index, data);
	}

	private int _GetCharFavorRewardListElementCount()
	{
		return _charFavorLevelUpList.Count;
	}

	private void _UpdateCharFavorPreferenceStepListSlot(int index, GameObject slotObject)
	{
		UIItemListSlot slot = slotObject.GetComponent<UIItemListSlot>();
		if (null == slot)
		{
			return;
		}
		
		GameTable.Item.Param data = null;
		
		if (0 <= index && index < _charPreferenceTotalList.Count)
		{
			data = _charPreferenceTotalList[index];

			int number = (int)eCOUNT.FAVOR_ITEM_GOOD;
			if (_charPreferenceStep1List.Find(x => x.ID == data.ID) != null)
			{
				number = (int)eCOUNT.FAVOR_ITEM_VERY_GOOD;
			}
			
			slot.SetHeart(number, true);
		}
		
		slot.ParentGO = this.gameObject;
		slot.UpdateSlot(UIItemListSlot.ePosType.Char_FavorPanelSelectItem, index, data);
	}
	
	private int _GetCharFavorPreferenceStepListElementCount()
	{
		return _charPreferenceStep1List.Count + _charPreferenceStep2List.Count;
	}
	
	private void SetPreferenceStep(string strStep, bool bStep1)
	{
		string[] stepArr = Utility.Split(strStep.Replace(" ", ""), ','); //strStep.Replace(" ", "").Split(',');
		for (int i = 0; i < stepArr.Length; i++)
		{
			GameTable.Item.Param itemdata = GameInfo.Instance.GameTable.FindItem(x => x.ID == int.Parse(stepArr[i]));
			if (itemdata == null)
			{
				continue;
			}

			if (bStep1)
			{
				_charPreferenceStep1List.Add(itemdata);
			}
			else
			{
				_charPreferenceStep2List.Add(itemdata);
			}
		}
	}
	
	public void OnClick_GiftBtn()
	{
        Log.Show("OnClick_GiftBtn");

		ItemData itemData = GameInfo.Instance.ItemList.Find(x => x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_PREFERENCE_PRESENT && 
		                                                                      x.Count > (int)eCOUNT.NONE);
		if (itemData != null)
		{
			UICharGiveGiftPopup popup = LobbyUIManager.Instance.GetUI("CharGiveGiftPopup") as UICharGiveGiftPopup;
			if (popup != null)
			{
				popup.SetData(_charData, _tableData, _charPreferenceStep1List, _charPreferenceStep2List);
				popup.SetUIActive(true);
			}
		}
		else
		{
			MessageToastPopup.Show(FLocalizeString.Instance.GetText(3262));
		}
	}
	
	public void OnClick_HelpBtn()
	{
		Log.Show("OnClick_HelpBtn");
		
		UIValue.Instance.SetValue(UIValue.EParamType.RulePopupType, (int)UIEventRulePopup.eRulePopupType.FAVOR_RULE);
		LobbyUIManager.Instance.ShowUI("EventRulePopup", true);
	}
}
