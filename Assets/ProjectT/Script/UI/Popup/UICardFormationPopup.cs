using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICardFormationPopup : FComponent
{

	[SerializeField] private FList _CardTeamListInstance;
	public UILabel kFavorLimitLabel;
	public GameObject kCardListNoneObj;
	private List<GameTable.CardFormation.Param> _cardFormationList = new List<GameTable.CardFormation.Param>();

	private eFilterFlag _flagFilter = eFilterFlag.ALL;
	private eFilterFlag _optFilter = eFilterFlag.ALL;
	private eFilterFlag _favorFilter = eFilterFlag.ALL;

	public override void Awake()
	{
		base.Awake();

		if(this._CardTeamListInstance == null) return;
		
		this._CardTeamListInstance.EventUpdate = this._UpdateCardTeamListSlot;
		this._CardTeamListInstance.EventGetItemCount = this._GetCardTeamElementCount;
		this._CardTeamListInstance.InitBottomFixing();
	}

	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
		if (_cardFormationList != null)
			_cardFormationList = new List<GameTable.CardFormation.Param>();

		_flagFilter = eFilterFlag.ALL;
		_optFilter = eFilterFlag.ALL;
		_favorFilter = eFilterFlag.ALL;

		_cardFormationList.Clear();

		_cardFormationList.AddRange(GameInfo.Instance.GameTable.CardFormations);

		SortCardFormationList();

		this._CardTeamListInstance.UpdateList();
		int cardFormationID = GameSupport.GetSelectCardFormationID();

		//if (cardFormationID > (int)eCOUNT.NONE)
		//{
		//	for (int i = 0; i < _cardFormationList.Count; i++)
		//	{
		//		if (cardFormationID == _cardFormationList[i].ID)
		//		{
		//			if(this._CardTeamListInstance.RowCount - 1  <= i)
		//				this._CardTeamListInstance.SetFocus(i, false);
		//			break;
		//		}
		//	}
		//}

		UIItemFilterPopup filterPopup = LobbyUIManager.Instance.GetUI<UIItemFilterPopup>("ItemFilterPopup");
		if (filterPopup != null)
		{
			filterPopup.DefailtTab = true;
		}

		kCardListNoneObj.SetActive(false);
	}

	private int SortCardTeamList(GameTable.CardFormation.Param data1, GameTable.CardFormation.Param data2)
	{
        bool dimdFlag = false;
        bool newFlag = true;
        bool levelFlag = true;
        bool favorFlag = true;

		int flagCnt1 = (int)eCOUNT.NONE;
		int flagCnt2 = (int)eCOUNT.NONE;

        SetCardItem(data1.CardID1, ref dimdFlag, ref newFlag, ref levelFlag, ref favorFlag);
        SetCardItem(data1.CardID2, ref dimdFlag, ref newFlag, ref levelFlag, ref favorFlag);
        SetCardItem(data1.CardID3, ref dimdFlag, ref newFlag, ref levelFlag, ref favorFlag);

		flagCnt1 = GetFlagCount(newFlag, levelFlag, favorFlag);

		dimdFlag = false;
		newFlag = true;
		levelFlag = true;
		favorFlag = true;

		SetCardItem(data2.CardID1, ref dimdFlag, ref newFlag, ref levelFlag, ref favorFlag);
		SetCardItem(data2.CardID2, ref dimdFlag, ref newFlag, ref levelFlag, ref favorFlag);
		SetCardItem(data2.CardID3, ref dimdFlag, ref newFlag, ref levelFlag, ref favorFlag);

		flagCnt2 = GetFlagCount(newFlag, levelFlag, favorFlag);

		if (!flagCnt1.Equals(flagCnt2))
		{
            if (flagCnt1 > flagCnt2)
                return -1;

            return 1;

            //return flagCnt1.CompareTo(flagCnt2);
		}

		return data1.ID.CompareTo(data2.ID);
	}
 
	private void SetCardItem(int cardTableId, ref bool dimdFlag, ref bool newFlag, ref bool levelFlag, ref bool favorFlag)
	{
		if (cardTableId == (int)eCOUNT.NONE)
		{
			return;
		}

		bool bNew = false;
		bool bLevel = false;
		bool bFavor = false;

		GameTable.Card.Param cardTableData = GameInfo.Instance.GameTable.FindCard(x => x.ID == cardTableId);
		if (cardTableData == null)
		{
			Debug.LogError(cardTableId + " Card id NULL");
			return;
		}

		GameClientTable.Book.Param clientBook = GameInfo.Instance.GameClientTable.FindBook(x => x.Group == (int)eBookGroup.Supporter && x.ItemID == cardTableId);
		if (cardTableData == null)
		{
			Debug.LogError(cardTableId + " CardClient id NULL");
			return;
		}

		string strnum = string.Format(FLocalizeString.Instance.GetText(216), clientBook.Num);

		CardBookData cardbookdata = GameInfo.Instance.CardBookList.Find(x => x.TableID == cardTableId);
		if (cardbookdata == null)
		{
			//서포터 획득 전
			dimdFlag = true;
			newFlag = false;
		}
		else
		{
			//서포터 획득 후
			bNew = true;
			bLevel = cardbookdata.IsOnFlag(eBookStateFlag.MAX_WAKE_AND_LV);
			bFavor = cardbookdata.IsOnFlag(eBookStateFlag.MAX_FAVOR_LV);

			if (!bLevel)
				levelFlag = false;
			if (!bFavor)
				favorFlag = false;
        }

		return;
	}

	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

		kFavorLimitLabel.textlocalize = FLocalizeString.Instance.GetText(218, GameInfo.Instance.CardFormationFavorList.Count, GameInfo.Instance.GameConfig.CardFormationFavoritesCount);

		kCardListNoneObj.SetActive(_cardFormationList.Count <= (int)eCOUNT.NONE);

		this._CardTeamListInstance.RefreshNotMove();
	}
 

	
	private void _UpdateCardTeamListSlot(int index, GameObject slotObject)
	{
		do
		{
			UICardTeamSlot slot = slotObject.GetComponent<UICardTeamSlot>();
			if (null == slot) break;
			slot.ParentGO = this.gameObject;

			GameTable.CardFormation.Param data = null;
			if (0 <= index && _cardFormationList.Count > index)
				data = _cardFormationList[index];

			slot.UpdateSlot(index, data);

		}while(false);
	}
	
	private int _GetCardTeamElementCount()
	{
		return _cardFormationList.Count;
	}

	
	public void OnClick_BackBtn()
	{
		OnClickClose();
	}

    public override void OnClickClose()
    {
		LobbyUIManager.Instance.Renewal("ArmoryPopup");
		base.OnClickClose();
    }

	public void OnClick_CardFormationInfoBtn()
	{
		LobbyUIManager.Instance.ShowUI("CardFormationinfoPopup", true);
	}

	public void OnClick_FilterBtn()
	{
		UIValue.Instance.SetValue(UIValue.EParamType.FilterOpenUI, UIItemFilterPopup.eFilterOpenUI.CardFormationPopup.ToString());
		//  추후 정렬 필터로 추가됩니다.
		LobbyUIManager.Instance.ShowUI("ItemFilterPopup", true);
	}

	public void SetFilter(eFilterFlag flagFilter, eFilterFlag optFilter, eFilterFlag favorFilter, bool renewal)
	{
		Log.Show("CardFormationFilter");

		_flagFilter = flagFilter;
		_optFilter = optFilter;
		_favorFilter = favorFilter;

		_cardFormationList.Clear();

		List<GameTable.CardFormation.Param> formationDummy = GameInfo.Instance.GameTable.CardFormations;

		for (int i = 0; i < formationDummy.Count; i++)
		{
			if (_flagFilter != eFilterFlag.ALL)
			{
				bool dimdFlag = false;
				bool newFlag1 = true;
				bool levelFlag1 = true;
				bool favorFlag1 = true;

				bool newFlag2 = true;
				bool levelFlag2 = true;
				bool favorFlag2 = true;

				bool newFlag3 = true;
				bool levelFlag3 = true;
				bool favorFlag3 = true;

				SetCardItem(formationDummy[i].CardID1, ref dimdFlag, ref newFlag1, ref levelFlag1, ref favorFlag1);
				SetCardItem(formationDummy[i].CardID2, ref dimdFlag, ref newFlag2, ref levelFlag2, ref favorFlag2);
				SetCardItem(formationDummy[i].CardID3, ref dimdFlag, ref newFlag3, ref levelFlag3, ref favorFlag3);

				bool newFlag = newFlag1 && newFlag2 && newFlag3;
				

				if ((_flagFilter & eFilterFlag.Pick_1) == eFilterFlag.Pick_1)
				{
					if (!(newFlag1 && newFlag2 && (formationDummy[i].CardID3 == (int)eCOUNT.NONE ? true : newFlag3)))
						continue;
				}
				if ((_flagFilter & eFilterFlag.Pick_2) == eFilterFlag.Pick_2)
				{
					if (!(newFlag && levelFlag1 && levelFlag2 && (formationDummy[i].CardID3 == (int)eCOUNT.NONE ? true : levelFlag3)))
						continue;
				}
				if ((_flagFilter & eFilterFlag.Pick_3) == eFilterFlag.Pick_3)
				{
					if (!(newFlag && favorFlag1 && favorFlag2 && (formationDummy[i].CardID3 == (int)eCOUNT.NONE ? true : favorFlag3)))
						continue;
				}
				if ((_flagFilter & eFilterFlag.Pick_4) == eFilterFlag.Pick_4)
				{
					if (newFlag1 && newFlag2 && (formationDummy[i].CardID3 == (int)eCOUNT.NONE ? true : newFlag3))
						continue;
				}
			}

			if (_optFilter != eFilterFlag.ALL)
			{
				string[] optStr = Utility.Split(formationDummy[i].OptionKind, ','); //formationDummy[i].OptionKind.Split(',');
				bool flag = false;

				for (int j = 0; j < optStr.Length; j++)
				{
					int optValue = int.Parse(optStr[j]);
					if ((_optFilter & (eFilterFlag)(1 << optValue - 1)) == (eFilterFlag)(1 << optValue - 1))
					{
						flag = true;
						break;
					}
				}

				if (!flag)
					continue;

			}

			if (_favorFilter != eFilterFlag.ALL)
			{
				if (!GameInfo.Instance.CardFormationFavorList.Contains(formationDummy[i].ID))
					continue;
			}

			_cardFormationList.Add(formationDummy[i]);
		}

		SortCardFormationList();
		this._CardTeamListInstance.UpdateList();
		int cardFormationID = GameSupport.GetSelectCardFormationID();

		kCardListNoneObj.SetActive(_cardFormationList.Count <= (int)eCOUNT.NONE);
	}

	private void SortCardFormationList()
	{
		_cardFormationList.Sort(SortCardTeamList);

		for (int i = 0; i < _cardFormationList.Count; i++)
		{
			if (GameInfo.Instance.CardFormationFavorList.Contains(_cardFormationList[i].ID))
			{
				GameTable.CardFormation.Param temp = GameInfo.Instance.GameTable.FindCardFormation(x => x.ID == _cardFormationList[i].ID);
				if (temp != null)
				{
					_cardFormationList.Remove(temp);
					_cardFormationList.Insert((int)eCOUNT.NONE, temp);
				}
			}
		}

		int cardformationID = GameSupport.GetSelectCardFormationID();
		GameTable.CardFormation.Param equipFormation = GameInfo.Instance.GameTable.FindCardFormation(x => x.ID == cardformationID);
		if (equipFormation != null)
		{
			_cardFormationList.Remove(equipFormation);
			_cardFormationList.Insert((int)eCOUNT.NONE, equipFormation);
		}
	}

	private int GetFlagCount(bool bNew, bool bLevel, bool bFavor)
	{
		int result = (int)eCOUNT.NONE;

		if (bNew)
		{
			result++;

			if (bLevel)
				result++;

			if (bFavor)
				result++;
		}

		return result;
	}
}
