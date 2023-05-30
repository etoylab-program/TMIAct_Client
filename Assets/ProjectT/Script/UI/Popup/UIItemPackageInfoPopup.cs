using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIItemPackageInfoPopup  : FComponent
{

	public UILabel kNameLabel;
	public UILabel kDescLabel;
	public UIItemListSlot kItemListSlot;
    public GameObject kRunBtnObj;

    public GameObject kRunCntBtnObj;
    public UILabel kRunCntLabel;

    [SerializeField] private FList _PackageListInstance;

    private long _itemUid;
    private ItemData _itemData = null;
    private GameTable.Item.Param _itemTableData;
    private List<GameTable.Random.Param> _packageInItems;
    private List<CardBookData> _precardbooklist = new List<CardBookData>();

    private int _selectItemUseCnt = 1;

    public override void Awake()
	{
		base.Awake();

        if (this._PackageListInstance == null) return;

        this._PackageListInstance.EventUpdate = this._UpdatePackageListSlot;
        this._PackageListInstance.EventGetItemCount = this._GetPackageElementCount;
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        kRunBtnObj.SetActive(false);
        kRunCntBtnObj.SetActive(false);
        _itemTableData = null;

        _itemUid = (long)UIValue.Instance.GetValue(UIValue.EParamType.ItemUID);

        if (_itemUid != -1)
        {
            _itemData = GameInfo.Instance.GetItemData(_itemUid);
            _itemTableData = _itemData.TableData;
            if(_itemTableData.Type == (int)eITEMTYPE.USE && _itemTableData.SubType == (int)eITEMSUBTYPE.USE_RANDOM_ITEM)
            {
                kRunCntBtnObj.SetActive(true);
                _selectItemUseCnt = 1;
                kRunCntLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(222), _selectItemUseCnt);
            }
            else
            {
                kRunBtnObj.SetActive(true);
            }
        }
        else
        {
            _itemData = GameInfo.Instance.GetItemData((int)UIValue.Instance.GetValue(UIValue.EParamType.ItemTableID));
            _itemTableData = GameInfo.Instance.GameTable.FindItem((int)UIValue.Instance.GetValue(UIValue.EParamType.ItemTableID));
        }

        if (_itemTableData == null)
            return;

        if (_itemTableData.SubType == (int)eITEMSUBTYPE.USE_STORE_PACKAGE)
        {
            GameTable.Store.Param storeTableData = GameInfo.Instance.GameTable.FindStore(_itemTableData.Value);
            if(storeTableData != null)
                _packageInItems = GameInfo.Instance.GameTable.Randoms.FindAll(x => x.GroupID == storeTableData.ProductIndex);
        }
        else
        {
            _packageInItems = GameInfo.Instance.GameTable.Randoms.FindAll(x => x.GroupID == _itemTableData.Value);
        }

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_itemTableData.Name);
        kDescLabel.textlocalize = FLocalizeString.Instance.GetText(_itemTableData.Desc);

        if (_itemData != null)
        {
            kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, -1, _itemData);
        }
        else
        {
            kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, -1, _itemTableData);
            kItemListSlot.SetCountText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), 0)));
        }

        Log.Show("ItemTableID : " + _itemTableData.ID, Log.ColorType.Red);
        Log.Show("PackageInItems Count : " + _packageInItems.Count);

        this._PackageListInstance.UpdateList();
    }
 	
	private void _UpdatePackageListSlot(int index, GameObject slotObject)
	{
		do
		{
            UIPackageItemListSlot slot = slotObject.GetComponent<UIPackageItemListSlot>();
            if (slot == null) break;
            if (index > _packageInItems.Count) break;

            slot.ParentGO = this.gameObject;

            slot.UpdateSlot(_packageInItems[index]);
        
		}while(false);
	}
	
	private int _GetPackageElementCount()
	{
        if (_packageInItems == null)
            return 0;
		return _packageInItems.Count; //TempValue
	}

	
	public void OnClick_BackBtn()
	{
        OnClickClose();
	}
	
	public void OnClick_RunBtn()
	{
        if (_itemTableData == null)
            return;

        if (!GameSupport.IsCheckInven())
            return;

        _precardbooklist.Clear();
        _precardbooklist.AddRange(GameInfo.Instance.CardBookList);

        if (_itemTableData.SubType == (int)eITEMSUBTYPE.USE_STORE_PACKAGE)
        {
            MessagePopup.CYN(eTEXTID.TITLE_NOTICE, string.Format(FLocalizeString.Instance.GetText(115), FLocalizeString.Instance.GetText(_itemTableData.Name)), eTEXTID.YES, eTEXTID.NO, StorePackageItemOpen, null);
        }
        else if(_itemTableData.SubType == (int)eITEMSUBTYPE.USE_SELECT_ITEM)
        {
            //아이템 정보팝업 갔다오면 itemuid 가 변경되기 때문에 다시한번 저장
            UIValue.Instance.SetValue(UIValue.EParamType.ItemUID, _itemUid);

            UIValue.Instance.SetValue(UIValue.EParamType.ChoicePopupType, (int)eChoicePopupType.SELECT_ITEM);
            LobbyUIManager.Instance.ShowUI("ChoiceitemPopup", true);
        }
        else if (_itemTableData.SubType == (int)eITEMSUBTYPE.USE_PACKAGE_ITEM)
        {
            MessagePopup.CYN(eTEXTID.TITLE_NOTICE, string.Format(FLocalizeString.Instance.GetText(115), FLocalizeString.Instance.GetText(_itemTableData.Name)), eTEXTID.YES, eTEXTID.NO, PackageItemOpen, null);
        }
    }

    public void StorePackageItemOpen()
    {
        GameTable.Store.Param storeTableData = GameInfo.Instance.GameTable.FindStore(_itemTableData.Value);
        if (storeTableData == null)
            return;

        GameInfo.Instance.Send_ReqStorePurchase(storeTableData.ID, false, 1, OnNetEventPackageItemOpen);
    }

    public void PackageItemOpen()
    {
        GameInfo.Instance.Send_ReqUseItem(_itemUid, 1, 0, OnNetEventPackageItemOpen);
    }

    public void OnNetEventPackageItemOpen(int _result, PktMsgType pktmsg)
    {
        if (_result != 0)
            return;

        for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++)
        {
            if (GameInfo.Instance.RewardList[i].Type == (int)eREWARDTYPE.CHAR)
            {
                DirectorUIManager.Instance.PlayCharBuy(GameInfo.Instance.RewardList[i].Index, OnDirectorUIPlayCharBuy);
                return;
            }
        }

        OnDirectorUIPlayCharBuy();
    }

    public void OnDirectorUIPlayCharBuy()
    {
        MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TITLE), FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TEXT), GameInfo.Instance.RewardList, OnMessageRewardCallBack);

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.InitComponent("ItemPanel");
        OnClickClose();
    }

    public void OnNetEventRandomItemOpen(int _result, PktMsgType pktmsg)
    {
        if (_result != 0)
            return;

        DirectorUIManager.Instance.PlayRewardOpen(FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TITLE), FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TEXT), GameInfo.Instance.RewardList, OnMessageRewardCallBack);

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.InitComponent("ItemPanel");
        OnClickClose();
    }

    public void OnMessageRewardCallBack()
    {
        DirectorUIManager.Instance.PlayNewCardGreeings(_precardbooklist);
    }

    public void OnClick_BGBtn()
    {
        OnClickClose();
    }

    public void OnClick_ItemUseCntPlus()
    {
        _selectItemUseCnt++;
        if (_selectItemUseCnt >= _itemData.Count)
            _selectItemUseCnt = _itemData.Count;

        if (_selectItemUseCnt >= GameInfo.Instance.GameConfig.MatCount)
            _selectItemUseCnt = GameInfo.Instance.GameConfig.MatCount;

        kRunCntLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(222), _selectItemUseCnt);
    }

    public void OnClick_ItemUseCntMinus()
    {
        _selectItemUseCnt--;
        if (_selectItemUseCnt <= 0)
            _selectItemUseCnt = 1;

        kRunCntLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(222), _selectItemUseCnt);
    }

    public void OnClick_ItemUseCntMax()
    {
        _selectItemUseCnt = _itemData.Count;

        if (_selectItemUseCnt >= GameInfo.Instance.GameConfig.MatCount)
            _selectItemUseCnt = GameInfo.Instance.GameConfig.MatCount;

        kRunCntLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(222), _selectItemUseCnt);
    }

    public void OnClick_ItemUseCnt()
    {
        if (_itemTableData.SubType == (int)eITEMSUBTYPE.USE_RANDOM_ITEM)
        {
            MessagePopup.CYN(eTEXTID.TITLE_NOTICE, string.Format(FLocalizeString.Instance.GetText(115), FLocalizeString.Instance.GetText(_itemTableData.Name)), eTEXTID.YES, eTEXTID.NO, OnMsg_UseItemCnt, null);
            return;
        }
        OnMsg_UseItemCnt();
    }

    public void OnMsg_UseItemCnt()
    {
        if (_selectItemUseCnt >= GameInfo.Instance.GameConfig.MatCount)
            _selectItemUseCnt = GameInfo.Instance.GameConfig.MatCount;

        _precardbooklist.Clear();
        _precardbooklist.AddRange(GameInfo.Instance.CardBookList);

        GameInfo.Instance.Send_ReqUseItem(_itemUid, _selectItemUseCnt, 0, OnNetEventRandomItemOpen);
    }
}
