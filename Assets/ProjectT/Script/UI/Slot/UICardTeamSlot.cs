using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICardTeamSlot : FSlot 
{
	public UISprite kSelSpr;
	public UIItemListSlot kCardItemListSlot00;
	public UIItemListSlot kCardItemListSlot01;
	public UIItemListSlot kCardItemListSlot02;
	public UICardTeamStateUnit kCardTeamStateUnit00;
	public UICardTeamStateUnit kCardTeamStateUnit01;
	public UICardTeamStateUnit kCardTeamStateUnit02;
	public UIButton kConfirmBtn;
	public UIButton kCancelBtn;
	public UIButton kDimdBtn;
	public GameObject kLastingeffecton;
	public UISprite kGet_HPSpr;
	public UISprite kLevel_HPSpr;
	public UISprite kFavor_HPSpr;
	public UILabel kGetHPValue;
	public UILabel kLevelHPValue;
	public UILabel kFavorHPValue;
	public GameObject kAchievement;
	public UISprite kNewSpr;
	public UISprite kLevelSpr;
	public UISprite kFaverSpr;
	public UISprite kConfirmBGSpr;
	public UISprite kNormalBGSpr;
	public UISprite kDimdBGSpr;
	public UILabel kEffectDescLabel;
	public UILabel kCardFormationNameLabel;
	public FToggle kFavorToggle;

	private int _index;
	private GameTable.CardFormation.Param _cardFormationData = null;
	private int _cardFormationID = 0;
	private eCharSelectFlag _selectFlag = eCharSelectFlag.USER_INFO;
	[SerializeField] private Color _descOriginColor;

    private void Awake()
    {
		kFavorToggle.EventCallBack = CardFavorToggleCallBack;
	}

    public void UpdateSlot()
    {
        if (!UIValue.Instance.ContainsKey(UIValue.EParamType.CardFormationType))
        {
            return;
        }

        _cardFormationID = GameSupport.GetSelectCardFormationID();
		_selectFlag = (eCharSelectFlag)UIValue.Instance.GetValue(UIValue.EParamType.CardFormationType);
		_cardFormationData = GameInfo.Instance.GameTable.FindCardFormation(x => x.ID == _cardFormationID);
		if (_cardFormationData == null)
		{
			Debug.LogError("CardFormation Table Data is NULL");
			return;
		}

		bool dimdFlag = false;
		bool newFlag = true;
		bool levelFlag = true;
		bool favorFlag = true;

		kCardFormationNameLabel.textlocalize = FLocalizeString.Instance.GetText(_cardFormationData.Name);

		GameClientTable.BattleOptionSet.Param formationBOSet1 = GameInfo.Instance.GameClientTable.FindBattleOptionSet(x => x.ID == _cardFormationData.FormationBOSetID1);
		GameClientTable.BattleOptionSet.Param formationBOSet2 = GameInfo.Instance.GameClientTable.FindBattleOptionSet(x => x.ID == _cardFormationData.FormationBOSetID2);

		string descStr = string.Empty;

		//지원부대 옵션이 두개거나 한개
		if(formationBOSet2 != null)
			descStr = FLocalizeString.Instance.GetText(_cardFormationData.Desc, formationBOSet1.BOFuncValue, formationBOSet2.BOFuncValue);
		else
			descStr = FLocalizeString.Instance.GetText(_cardFormationData.Desc, formationBOSet1.BOFuncValue);

		string[] splitStr = Utility.Split(descStr, '['); //descStr.Split('[');
		for (int i = 0; i < splitStr.Length; i++)
		{
			int idx = descStr.IndexOf('[');
			int size = descStr.IndexOf(']') - idx;
			descStr = descStr.Remove(idx, size);
		}

		kEffectDescLabel.textlocalize = descStr;

		CardBookData cardBookData00 = SetCardItem(_cardFormationData.CardID1, kCardItemListSlot00, kCardTeamStateUnit00, ref dimdFlag, ref newFlag, ref levelFlag, ref favorFlag);
		CardBookData cardBookData01 = SetCardItem(_cardFormationData.CardID2, kCardItemListSlot01, kCardTeamStateUnit01, ref dimdFlag, ref newFlag, ref levelFlag, ref favorFlag);
		CardBookData cardBookData02 = SetCardItem(_cardFormationData.CardID3, kCardItemListSlot02, kCardTeamStateUnit02, ref dimdFlag, ref newFlag, ref levelFlag, ref favorFlag);

		kSelSpr.SetActive(true);
		kNormalBGSpr.SetActive(true);

		kDimdBGSpr.SetActive(false);
		kConfirmBGSpr.SetActive(false);
		kConfirmBtn.SetActive(false);
		kCancelBtn.SetActive(false);

		if (newFlag)
		{
			kGetHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.GetHP.ToString("F2")));
			if (levelFlag)
				kLevelHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.LevelHP.ToString("F2")));
			else
				kLevelHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.LevelHP.ToString("F2")));

			if (favorFlag)
				kFavorHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.FavorHP.ToString("F2")));
			else
				kFavorHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.FavorHP.ToString("F2")));
		}
		else 
		{
			kGetHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.GetHP.ToString("F2")));
			kLevelHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.LevelHP.ToString("F2")));
			kFavorHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.FavorHP.ToString("F2")));
		}
			

		
	}

	public void UpdateSlot(int index, GameTable.CardFormation.Param data) 	//Fill parameter if you need
	{
		_cardFormationID = GameSupport.GetSelectCardFormationID();
		_selectFlag = (eCharSelectFlag)UIValue.Instance.GetValue(UIValue.EParamType.CardFormationType);
		_index = index;
		_cardFormationData = data;
		if (_cardFormationData == null)
			return;

		bool dimdFlag = false;
		bool newFlag = true;
		bool levelFlag = true;
		bool favorFlag = true;

		kSelSpr.SetActive(false);
		kDimdBGSpr.SetActive(false);
		kConfirmBGSpr.SetActive(false);
		kNormalBGSpr.SetActive(false);
		kConfirmBtn.SetActive(false);
		kCancelBtn.SetActive(false);
		kDimdBtn.SetActive(false);
		kCardFormationNameLabel.textlocalize = FLocalizeString.Instance.GetText(_cardFormationData.Name);
		GameClientTable.BattleOptionSet.Param formationBOSet1 = GameInfo.Instance.GameClientTable.FindBattleOptionSet(x => x.ID == _cardFormationData.FormationBOSetID1);
		GameClientTable.BattleOptionSet.Param formationBOSet2 = GameInfo.Instance.GameClientTable.FindBattleOptionSet(x => x.ID == _cardFormationData.FormationBOSetID2);
		string descStr = string.Empty;

		//지원부대 옵션이 두개거나 한개
		if (formationBOSet2 != null)
			descStr = FLocalizeString.Instance.GetText(_cardFormationData.Desc, formationBOSet1.BOFuncValue, formationBOSet2.BOFuncValue);
		else
			descStr = FLocalizeString.Instance.GetText(_cardFormationData.Desc, formationBOSet1.BOFuncValue);

		CardBookData cardBookData00 = SetCardItem(_cardFormationData.CardID1, kCardItemListSlot00, kCardTeamStateUnit00, ref dimdFlag, ref newFlag, ref levelFlag, ref favorFlag);
		CardBookData cardBookData01 = SetCardItem(_cardFormationData.CardID2, kCardItemListSlot01, kCardTeamStateUnit01, ref dimdFlag, ref newFlag, ref levelFlag, ref favorFlag);
		CardBookData cardBookData02 = SetCardItem(_cardFormationData.CardID3, kCardItemListSlot02, kCardTeamStateUnit02, ref dimdFlag, ref newFlag, ref levelFlag, ref favorFlag);

		string strColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);

		if (newFlag && levelFlag && favorFlag)
		{
			kFavorToggle.gameObject.SetActive(true);

			if (GameInfo.Instance.CardFormationFavorList.Contains(_cardFormationData.ID))
			{
				kFavorToggle.SetToggle((int)eCOUNT.NONE, SelectEvent.Code);
			}
			else
			{
				kFavorToggle.SetToggle((int)eCOUNT.NONE + 1, SelectEvent.Code);
			}

			if (_cardFormationData.ID == _cardFormationID)
			{
				//장착중
				kSelSpr.SetActive(true);
				kConfirmBGSpr.SetActive(true);
				kCancelBtn.SetActive(true);
			}
			else
			{
				//장착안한것들
				kNormalBGSpr.SetActive(true);
				kConfirmBtn.SetActive(true);
			}

			kEffectDescLabel.color = _descOriginColor;
		}
		else
		{
			kFavorToggle.gameObject.SetActive(false);
			kDimdBtn.SetActive(true);
			kDimdBGSpr.SetActive(true);
			strColor = FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR);
			kEffectDescLabel.color = Color.gray;
			//BBCode 제거
			string[] splitStr = Utility.Split(descStr, '['); //descStr.Split('[');
			for (int i = 0; i < splitStr.Length; i++)
			{
				int idx = descStr.IndexOf('[');
				if (idx < (int)eCOUNT.NONE)
					break;
				int size = (descStr.IndexOf(']') - idx) + 1;
				descStr = descStr.Remove(idx, size);
			}
		}

		kEffectDescLabel.textlocalize = descStr;

		if (newFlag)
		{
			kGetHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.GetHP.ToString("F2")));
			if (levelFlag)
				kLevelHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.LevelHP.ToString("F2")));
			else
				kLevelHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.LevelHP.ToString("F2")));

			if (favorFlag)
				kFavorHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.FavorHP.ToString("F2")));
			else
				kFavorHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.FavorHP.ToString("F2")));
		}
		else
		{
			kGetHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.GetHP.ToString("F2")));
			kLevelHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.LevelHP.ToString("F2")));
			kFavorHPValue.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, _cardFormationData.FavorHP.ToString("F2")));
		}
	}

	private CardBookData SetCardItem(int cardTableId, UIItemListSlot cardItemListSlot, UICardTeamStateUnit cardStateUnit, ref bool dimdFlag, ref bool newFlag, ref bool levelFlag, ref bool favorFlag)
	{
		if (cardTableId == (int)eCOUNT.NONE)
		{
			cardItemListSlot.SetActive(false);
			cardStateUnit.SetActive(false);
			return null;
		}
		cardItemListSlot.ParentGO = this.gameObject;
		cardItemListSlot.SetActive(true);
		cardStateUnit.SetActive(true);

		bool bNew = false;
		bool bLevel = false;
		bool bFavor = false;

		GameTable.Card.Param cardTableData = GameInfo.Instance.GameTable.FindCard(x => x.ID == cardTableId);
		if (cardTableData == null)
		{
			Debug.LogError(cardTableId + " Card id NULL");
			return null;
		}

		GameClientTable.Book.Param clientBook = GameInfo.Instance.GameClientTable.FindBook(x => x.Group == (int)eBookGroup.Supporter && x.ItemID == cardTableId);
		if (cardTableData == null)
		{
			Debug.LogError(cardTableId + " CardClient id NULL");
			return null;
		}

		string strnum = string.Format(FLocalizeString.Instance.GetText(216), clientBook.Num);

		CardBookData cardbookdata = GameInfo.Instance.CardBookList.Find(x => x.TableID == cardTableId);
		if (cardbookdata == null)
		{
			//서포터 획득 전
			cardStateUnit.UpdateSlot(bNew, bLevel, bFavor);
			cardItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Book, 0, cardTableData, true);
			dimdFlag = true;
			newFlag = false;
		}
		else
		{
			//서포터 획득 후
			bNew = true;
			bLevel = cardbookdata.IsOnFlag(eBookStateFlag.MAX_WAKE_AND_LV);
			bFavor = cardbookdata.IsOnFlag(eBookStateFlag.MAX_FAVOR_LV);

			cardStateUnit.UpdateSlot(bNew, bLevel, bFavor);
			cardItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Book, 0, cardTableData, false);

			if (!bLevel)
				levelFlag = false;
			if (!bFavor)
				favorFlag = false;
		}
		cardItemListSlot.SetCountLabel(strnum);

		return cardbookdata;
	}


	public void OnClick_Slot()
    {

    }

	public void OnClick_ConfirmBtn() {
		if( _selectFlag == eCharSelectFlag.ARENA ) {
			GameInfo.Instance.Send_ReqSetArenaTeam( GameInfo.Instance.TeamcharList, (uint)_cardFormationData.ID, OnNet_UserSetMainCardFormation );
		}
		else if( _selectFlag == eCharSelectFlag.ARENATOWER ) {
			GameInfo.Instance.Send_ReqSetArenaTowerTeam( GameInfo.Instance.TowercharList, (uint)_cardFormationData.ID, OnNet_UserSetMainCardFormation );
		}
		else if( _selectFlag == eCharSelectFlag.RAID ) {
			GameInfo.Instance.Send_ReqSetRaidTeam( GameInfo.Instance.RaidUserData.CharUidList, (uint)_cardFormationData.ID, OnNet_UserSetMainCardFormation );
		}
		else {
			GameInfo.Instance.Send_ReqUserSetMainCardFormation( (uint)_cardFormationData.ID, OnNet_UserSetMainCardFormation );
		}
	}

	public void OnNet_UserSetMainCardFormation( int result, PktMsgType pktmsg ) {
		if( result != 0 ) {
			return;
		}

		LobbyRenewalUI();
		MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3224, FLocalizeString.Instance.GetText( _cardFormationData.Name ) ) );
	}

	public void OnClick_CancelBtn()
	{
		int cancelId = (int)eCOUNT.NONE;
		//해제
		if( _selectFlag == eCharSelectFlag.ARENA ) {
			GameInfo.Instance.Send_ReqSetArenaTeam( GameInfo.Instance.TeamcharList, (uint)cancelId, OnNet_CancelUserSetMainCardFormation );
		}
		else if( _selectFlag == eCharSelectFlag.ARENATOWER ) {
			GameInfo.Instance.Send_ReqSetArenaTowerTeam( GameInfo.Instance.TowercharList, (uint)cancelId, OnNet_CancelUserSetMainCardFormation );
		}
		else if( _selectFlag == eCharSelectFlag.RAID ) {
			GameInfo.Instance.Send_ReqSetRaidTeam( GameInfo.Instance.RaidUserData.CharUidList, (uint)cancelId, OnNet_CancelUserSetMainCardFormation );
		}
		else {
			GameInfo.Instance.Send_ReqUserSetMainCardFormation( (uint)cancelId, OnNet_CancelUserSetMainCardFormation );
		}
	}

	public void OnNet_CancelUserSetMainCardFormation(int result, PktMsgType pktmsg)
	{
		if (result != 0)
			return;

		LobbyRenewalUI();

		MessageToastPopup.Show(FLocalizeString.Instance.GetText(3225, FLocalizeString.Instance.GetText(_cardFormationData.Name)));
	}

	public void OnClick_DimdBtn()
	{
		MessageToastPopup.Show(FLocalizeString.Instance.GetText(3239, FLocalizeString.Instance.GetText(_cardFormationData.Name)));
	}

	private void LobbyRenewalUI()
    {
        LobbyUIManager.Instance.Renewal("CardFormationPopup");
        LobbyUIManager.Instance.Renewal("ArenaTowerMainPanel");
        LobbyUIManager.Instance.Renewal("ArenaTowerStagePanel");
		LobbyUIManager.Instance.Renewal("PresetPopup");
	}

	private bool CardFavorToggleCallBack(int nSelect, SelectEvent type)
	{
		if (type == SelectEvent.Click)
		{
			if (GameInfo.Instance.CardFormationFavorList.Count >= GameInfo.Instance.GameConfig.CardFormationFavoritesCount)
			{
				if (GameInfo.Instance.CardFormationFavorList.Contains(_cardFormationData.ID))
				{
					GameInfo.Instance.Send_ReqUserCardFormationFavi(_cardFormationData.ID, OnNet_UserCardFormationFavi);
				}
				else
				{
					MessageToastPopup.Show(FLocalizeString.Instance.GetText(3269));
				}
			}
			else
			{
				GameInfo.Instance.Send_ReqUserCardFormationFavi(_cardFormationData.ID, OnNet_UserCardFormationFavi);
			}
			return false;
		}

		return true;
	}

	public void OnNet_UserCardFormationFavi(int result, PktMsgType pktmsg)
	{
		if (result != 0)
			return;

		LobbyUIManager.Instance.Renewal("CardFormationPopup");
	}

}
