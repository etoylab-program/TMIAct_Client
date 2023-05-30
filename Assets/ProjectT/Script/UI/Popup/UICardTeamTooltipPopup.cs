using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardTeamToolTipPopup
{
	public enum eCardToolTipDir
	{
		LEFT,
		RIGHT,
		NONE
	}

	public static UICardTeamTooltipPopup GetCardTeamToolTipPopup()
	{
		UICardTeamTooltipPopup popup = null;
		if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
			popup = LobbyUIManager.Instance.GetUI<UICardTeamTooltipPopup>("CardTeamTooltipPopup");

		return popup;
	}

	public static void Show(int cardFormationTID, GameObject target, eCardToolTipDir dir)
	{
		UICardTeamTooltipPopup popup = GetCardTeamToolTipPopup();
		if (popup == null)
			return;

		popup.InitCardTeamToolTip(cardFormationTID, target, dir);
	}
}

public class UICardTeamTooltipPopup : FComponent
{
	public UIItemListSlot kCardTeamSlot00;
	public UIItemListSlot kCardTeamSlot01;
	public UIItemListSlot kCardTeamSlot02;

	public UILabel kCardTeamNameLabel;
	public UILabel kValueLabel;
	public GameObject kRootObj;
	public GameObject kArrow_R;
	public GameObject kArrow_L;
 

	void OnClick()
	{
		SetUIActive(false);
	}

	public void InitCardTeamToolTip(int cardFormationTID, GameObject target, CardTeamToolTipPopup.eCardToolTipDir dir)
	{
		SetUIActive(true);

		kArrow_L.SetActive(dir == CardTeamToolTipPopup.eCardToolTipDir.LEFT);
		kArrow_R.SetActive(dir == CardTeamToolTipPopup.eCardToolTipDir.RIGHT);

		Vector3 targetPos = UICamera.mainCamera.ViewportToScreenPoint(target.transform.localPosition);

		kRootObj.transform.localPosition = (dir == CardTeamToolTipPopup.eCardToolTipDir.LEFT) ?
			(new Vector3(target.transform.localPosition.x + 400f, 0, 0)) :
			(new Vector3(target.transform.localPosition.x + 200f, 0, 0));

		GameTable.CardFormation.Param cardFormtationData = GameInfo.Instance.GameTable.FindCardFormation(x => x.ID == cardFormationTID);

		SetCardItem(kCardTeamSlot00, cardFormtationData.CardID1);
		SetCardItem(kCardTeamSlot01, cardFormtationData.CardID2);
		SetCardItem(kCardTeamSlot02, cardFormtationData.CardID3);

		kCardTeamNameLabel.textlocalize = FLocalizeString.Instance.GetText(cardFormtationData.Name);

		GameClientTable.BattleOptionSet.Param formationBOSet1 = GameInfo.Instance.GameClientTable.FindBattleOptionSet(x => x.ID == cardFormtationData.FormationBOSetID1);
		GameClientTable.BattleOptionSet.Param formationBOSet2 = GameInfo.Instance.GameClientTable.FindBattleOptionSet(x => x.ID == cardFormtationData.FormationBOSetID2);

		if(formationBOSet2 != null)
			kValueLabel.textlocalize = FLocalizeString.Instance.GetText(cardFormtationData.Desc, formationBOSet1.BOFuncValue, formationBOSet2.BOFuncValue);
		else
			kValueLabel.textlocalize = FLocalizeString.Instance.GetText(cardFormtationData.Desc, formationBOSet1.BOFuncValue);
	}

	private void SetCardItem(UIItemListSlot cardSlot, int cardTableId)
	{
		if (cardTableId == (int)eCOUNT.NONE)
		{
			cardSlot.SetActive(false);
			return;
		}

		cardSlot.SetActive(true);

        GameTable.Card.Param cardTableData = GameInfo.Instance.GameTable.FindCard(x => x.ID == cardTableId);
        if (cardTableData == null)
        {
            Debug.LogError(cardTableId + " Card id NULL");
            cardSlot.SetActive(false);
            return;
        }

        GameClientTable.Book.Param clientBook = GameInfo.Instance.GameClientTable.FindBook(x => x.Group == (int)eBookGroup.Supporter && x.ItemID == cardTableId);
		if (clientBook == null)
		{
			Debug.LogError(cardTableId + " CardClient id NULL");
			return;
		}

		cardSlot.UpdateSlot(UIItemListSlot.ePosType.Book, 0, cardTableData, false);

		//CardBookData cardbookdata = GameInfo.Instance.CardBookList.Find(x => x.TableID == cardTableId);
		//if (cardbookdata == null)
		//{
		//	cardSlot.SetActive(false);
		//	return;
		//}
		//else
		//{
			

		//}
		string strnum = string.Format(FLocalizeString.Instance.GetText(216), clientBook.Num);
		cardSlot.SetCountLabel(strnum);
	}
}
