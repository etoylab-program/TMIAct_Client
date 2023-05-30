using UnityEngine;
using System.Collections;

public class UICardArmoryTeamSlot : FSlot 
{
	public UISprite kSetSpr;

	public UILabel kCardTeamNameLabel;
	public UILabel kCardTeamValueLabel;

	public UIItemListSlot kCardItemListSlot00;
	public UIItemListSlot kCardItemListSlot01;
	public UIItemListSlot kCardItemListSlot02;
	private GameTable.CardFormation.Param _cardFormationData = null;

	public void UpdateSlot() 	//Fill parameter if you need
	{
		_cardFormationData = GameInfo.Instance.GameTable.FindCardFormation(x => x.ID == GameSupport.GetSelectCardFormationID());
		if(_cardFormationData == null)
		{
			return;
		}

		SetCardItem(kCardItemListSlot00, _cardFormationData.CardID1);
		SetCardItem(kCardItemListSlot01, _cardFormationData.CardID2);
		SetCardItem(kCardItemListSlot02, _cardFormationData.CardID3);

		GameClientTable.BattleOptionSet.Param formationBOSet1 = GameInfo.Instance.GameClientTable.FindBattleOptionSet(x => x.ID == _cardFormationData.FormationBOSetID1);
		if(formationBOSet1 == null)
		{
			return;
		}

		if (kCardTeamNameLabel)
		{
			kCardTeamNameLabel.textlocalize = FLocalizeString.Instance.GetText(_cardFormationData.Name);
		}

		//지원부대 옵션이 두개거나 한개
		GameClientTable.BattleOptionSet.Param formationBOSet2 = GameInfo.Instance.GameClientTable.FindBattleOptionSet(x => x.ID == _cardFormationData.FormationBOSetID2);
		if (formationBOSet2 != null)
		{
			if (kCardTeamValueLabel)
			{
				kCardTeamValueLabel.textlocalize = FLocalizeString.Instance.GetText(_cardFormationData.Desc, formationBOSet1.BOFuncValue, formationBOSet2.BOFuncValue);
			}
		}
		else
		{
			if (kCardTeamValueLabel)
			{
				kCardTeamValueLabel.textlocalize = FLocalizeString.Instance.GetText(_cardFormationData.Desc, formationBOSet1.BOFuncValue);
			}
		}
	}
 
	public void OnClick_Slot()
	{
	}

	private void SetCardItem(UIItemListSlot cardSlot, int cardTableId)
	{
		if (cardTableId == (int)eCOUNT.NONE)
		{
			cardSlot.SetActive(false);
			return;
		}
		cardSlot.ParentGO = this.gameObject;
		cardSlot.SetActive(true);

		GameTable.Card.Param cardTableData = GameInfo.Instance.GameTable.FindCard(x => x.ID == cardTableId);
		if (cardTableData == null)
		{
			Debug.LogError(cardTableId + " Card id NULL");
			cardSlot.SetActive(false);
			return;
		}

		GameClientTable.Book.Param clientBook = GameInfo.Instance.GameClientTable.FindBook(x => x.Group == (int)eBookGroup.Supporter && x.ItemID == cardTableId);
		if (cardTableData == null)
		{
			Debug.LogError(cardTableId + " CardClient id NULL");
			return;
		}

		CardBookData cardbookdata = GameInfo.Instance.CardBookList.Find(x => x.TableID == cardTableId);
		if (cardbookdata == null)
		{
			cardSlot.SetActive(false);
			return;
		}
		else
		{
			cardSlot.UpdateSlot(UIItemListSlot.ePosType.Book, 0, cardTableData, false);
			
		}
		string strnum = string.Format(FLocalizeString.Instance.GetText(216), clientBook.Num);
		cardSlot.SetCountLabel(strnum);
	}
}
