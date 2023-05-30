using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEventExchangeRewardListSlot : FSlot {

    public UIRewardListSlot kRewardListSlot;
	public UISprite kCardTypeSpr;
	public UISprite kShortfallSpr;
	public UILabel kNameLabel;
	public UIButton kExchangeBtn;
	public UITexture kItem0Tex;
    public UILabel kItem0Count;
	public UITexture kItem1Tex;
    public UILabel kItem1Count;
    public UITexture kItem2Tex;
    public UILabel kItem2Count;
    public UISprite kLineSpr;
	public UISprite kBGSpr;
	public UILabel kCountLabel;
	public UISprite kInfiniteSpr;
	public GameObject kComplete;

    public GameObject kExchangeDisableBtnSpr;
    public GameObject kExchangeDisableBtnLb;
    public GameObject kExchangeBtnEffObj;

    GameTable.EventExchangeReward.Param m_exchangeRewardTable;

    private int _index;

    private int m_Item0Count;
    private int m_Item1Count;
    private int m_Item2Count;
    private List<CardBookData> _precardbooklist = new List<CardBookData>();

    public void UpdateSlot(int idx, int cnt, GameTable.EventExchangeReward.Param tableData) 	//Fill parameter if you need
	{
        _index = idx;

        kComplete.SetActive(false);
        m_exchangeRewardTable = tableData;
        eREWARDTYPE rewardType = (eREWARDTYPE)tableData.ProductType;

        RewardData rewardData = new RewardData(tableData.ProductType, tableData.ProductIndex, tableData.ProductValue);

        kRewardListSlot.UpdateSlot(rewardData, true);

        m_Item0Count = SetNeedItem(kItem0Tex, kItem0Count, tableData.ReqItemID1, tableData.ReqItemCnt1);
        m_Item1Count = SetNeedItem(kItem1Tex, kItem1Count, tableData.ReqItemID2, tableData.ReqItemCnt2);
        m_Item2Count = SetNeedItem(kItem2Tex, kItem2Count, tableData.ReqItemID3, tableData.ReqItemCnt3);

        kInfiniteSpr.gameObject.SetActive((tableData.ExchangeCnt == 0));
        kCountLabel.gameObject.SetActive(!(tableData.ExchangeCnt == 0));
        kCountLabel.textlocalize = string.Format("{0}/{1}", cnt, tableData.ExchangeCnt);

        FLocalizeString.SetLabel(kNameLabel, GameSupport.GetProductName(rewardData));

        
        if (tableData.ExchangeCnt != 0 && cnt == 0)
        {
            kComplete.SetActive(true);
            kRewardListSlot.IsInActive(true);
        }

        ChackExChangeBtnActive();
    }
 
	public void OnClick_Slot()
	{
        //Log.Show("OnClick_Slot");
    }
 
	public void OnClick_ExchangeBtn()
	{
        if(kExchangeDisableBtnSpr != null)
        {
            if (kExchangeDisableBtnSpr.activeSelf)
                return;
        }

        if (kComplete.activeSelf)
            return;

        Log.Show("OnClick_ExchangeBtn : " + m_Item0Count + " / " + m_Item1Count + " / " + m_Item2Count);

        _precardbooklist.Clear();
        _precardbooklist.AddRange(GameInfo.Instance.CardBookList);

        GameInfo.Instance.Send_ReqEventRewardTake(m_exchangeRewardTable.EventID, 1, m_exchangeRewardTable.RewardStep, _index, OnNetEventRewardTake);
    }

    public void OnNetEventRewardTake(int _result, PktMsgType pktmsg)
    {
        if (_result != 0)
            return;

        MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TITLE), FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TEXT), GameInfo.Instance.RewardList, OnMessageRewardCallBack);

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");

        UIEventmodeExchangePanel uIEventmodeExchangePanel = LobbyUIManager.Instance.GetActiveUI<UIEventmodeExchangePanel>("EventmodeExchangePanel");
        if (uIEventmodeExchangePanel != null)
            uIEventmodeExchangePanel.Renewal(true);
    }

    public void OnMessageRewardCallBack()
    {
        DirectorUIManager.Instance.PlayNewCardGreeings(_precardbooklist);
    }

    public void OnClick_ItemListSlot()
	{
        Log.Show("OnClick_ItemListSlot");
    }

    private int SetNeedItem(UITexture iconTex, UILabel cntLb, int itemId, int itemCnt)
    {
        if(itemId != 0)
        {
            string texPath = "Icon/Item/" + GameInfo.Instance.GameTable.FindItem(x => x.ID == itemId).Icon;
            iconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", texPath);

            iconTex.gameObject.SetActive(true);
            cntLb.gameObject.SetActive(true);

            string strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.WHITE_TEXT_COLOR);
            int useItemCnt = GameInfo.Instance.GetItemIDCount(itemId);
            if(useItemCnt < itemCnt)
                strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);

            cntLb.textlocalize = string.Format(strHaveCntColor, string.Format("{0}", itemCnt));
        }
        else
        {
            iconTex.gameObject.SetActive(false);
            cntLb.gameObject.SetActive(false);
        }

        return itemCnt;
    }

    private void ChackExChangeBtnActive()
    {
        if (m_exchangeRewardTable == null)
            return;

        bool needChecker = NeedItemChech(m_exchangeRewardTable.ReqItemID1, m_exchangeRewardTable.ReqItemCnt1) && NeedItemChech(m_exchangeRewardTable.ReqItemID2, m_exchangeRewardTable.ReqItemCnt2) && NeedItemChech(m_exchangeRewardTable.ReqItemID3, m_exchangeRewardTable.ReqItemCnt3);

        if (kComplete.activeSelf)
        {
            needChecker = false;
            kExchangeBtn.gameObject.SetActive(false);
        }
        else
        {
            kExchangeBtn.gameObject.SetActive(true);
            kExchangeDisableBtnSpr.SetActive(!needChecker);
            kExchangeDisableBtnLb.SetActive(!needChecker);
            kExchangeBtnEffObj.SetActive(needChecker);
        }
        kExchangeBtn.isEnabled = needChecker;
    }

    //아이템 아이디, 아이템 갯수로 교환 가능 여부 반환
    private bool NeedItemChech(int itemId, int itemCnt)
    {
        if (itemId == 0)
            return true;

        int userHaveItemCnt = GameInfo.Instance.GetItemIDCount(itemId);

        if (userHaveItemCnt < itemCnt)
            return false;


        return true;
    }
}
