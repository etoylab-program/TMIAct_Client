using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIFacilityItemCombinePopup : FComponent
{

	public FTab kMainTab;
    public List<UIItemListSlot> kMatItemList;
    public UILabel kTimeLabel;
    public UILabel kReleaseTimeLabel;
    public UILabel kItemCombineCountLabel;
	public UIGoodsUnit kGoodsUnit;
    public UIButton kSeleteBtn;

    [SerializeField] private FList _ItemListInstance;
    private FacilityData _facilitydata;
    private FacilityData _factilitdataParent;
    private List<GameTable.FacilityItemCombine.Param> _facilityitemcombinelist = new List<GameTable.FacilityItemCombine.Param>();
    [SerializeField]private int _seleteid;
    private bool _bmat;
    public int SeleteID { get { return _seleteid; } }
    private int m_OldSelectID = 0;

    int _itemCombineCount = 0;          //현재 선택된 아이템 조합 갯수
    int _itemCombineMaxCount = 0;       //현재 재료로 만들수있는 아이템조합 최대 갯수
    bool _itemCombineFlag = false;      //현재 1개이상 조합을 할수있는지 여부
    string _itemCombineCountColor;      //현재 조합이 가능 / 불가능 칼라값 저장

    GameTable.FacilityItemCombine.Param _combineParam;
    GameTable.ItemReqList.Param _itemReqlist;

	private List<int> mItemCombineMaxCountList = null;

	public override void Awake()
	{
		base.Awake();

		kMainTab.EventCallBack = OnMainTabSelect;
		if(this._ItemListInstance == null) return;
		
		this._ItemListInstance.EventUpdate = this._UpdateItemListSlot;
		this._ItemListInstance.EventGetItemCount = this._GetItemElementCount;
        this._ItemListInstance.InitBottomFixing();

    }
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        var obj = UIValue.Instance.GetValue(UIValue.EParamType.FacilityID);
        if (obj == null)
            return;
        _facilitydata = GameInfo.Instance.GetFacilityData((int)obj);
        _factilitdataParent = GameInfo.Instance.GetFacilityData((int)_facilitydata.TableData.ParentsID);
        _bmat = true;
        _seleteid = -1;

        if (_facilitydata.EquipCardUID != (int)eCOUNT.NONE)
            _itemCombineCountColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
        else
            _itemCombineCountColor = FLocalizeString.Instance.GetText((int)eTEXTID.WHITE_TEXT_COLOR);

        int tab = 0;
        if( _facilitydata.Selete != -1 )
        {
            var _facilityitemcombinedata = GameInfo.Instance.GameTable.FindFacilityItemCombine((int)_facilitydata.Selete);
            if (_facilityitemcombinedata != null)
            {
                tab = _facilityitemcombinedata.ReqFacilityLv - 1;
            }
        }

        for(int i = 0; i < kMainTab.kBtnList.Count; i++)
        {
            if (_factilitdataParent.Level - 1 >= i)
            {
                kMainTab.SetEnabled(i, true);
                kMainTab.SetTabLabelAlpha(i, 1.0f);
            }
            else
            {
                kMainTab.SetEnabled(i, true);
                kMainTab.SetTabLabelAlpha(i, 0.25f);
            }
        }

        kMainTab.SetTab(tab, SelectEvent.Code);
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        if (_seleteid == -1)
            return;

        var _facilityitemcombinedata = GameInfo.Instance.GameTable.FindFacilityItemCombine(_seleteid);
        if (_facilityitemcombinedata == null)
            return;


        for (int i = 0; i < kMatItemList.Count; i++)
        {
            kMatItemList[i].gameObject.SetActive(false);
        }

        if (_itemReqlist != null)
        {
            List<int> idlist = new List<int>();
            List<int> countlist = new List<int>();
            GameSupport.SetMatList(_itemReqlist, ref idlist, ref countlist);
            for (int i = 0; i < idlist.Count; i++)
            {
                kMatItemList[i].gameObject.SetActive(true);
                kMatItemList[i].UpdateSlot(UIItemListSlot.ePosType.Mat, i, GameInfo.Instance.GameTable.FindItem(idlist[i]));
                int orgcut = GameInfo.Instance.GetItemIDCount(idlist[i]);
                int orgmax = countlist[i] * _itemCombineCount;
                string strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
                if (orgcut < orgmax)
                {
                    _bmat = false;
                    strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);
                }
                string strmatcount = string.Format(strHaveCntColor, string.Format(FLocalizeString.Instance.GetText(236), orgcut, orgmax));
                kMatItemList[i].SetCountLabel(strmatcount);
            }
        }
        
        kGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, _itemReqlist.Gold * _itemCombineCount, true);

        kItemCombineCountLabel.textlocalize = string.Format("{0}/{1}", _itemCombineCount.ToString("D2"), _itemCombineMaxCount.ToString("D2"));

        kTimeLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.WHITE_TEXT_COLOR), GameSupport.GetFacilityTimeString(_facilityitemcombinedata.Time * 60));

        int time = (int)GameSupport.GetFacilityItemCombineTime(_facilityitemcombinedata.Time * 60 * _itemCombineCount, _facilitydata);
        kReleaseTimeLabel.textlocalize = string.Format(_itemCombineCountColor, GameSupport.GetFacilityTimeString(time));

        kSeleteBtn.gameObject.SetActive(true);
    }

    private bool OnMainTabSelect(int nSelect, SelectEvent type)
	{
        _facilityitemcombinelist = GameInfo.Instance.GameTable.FindAllFacilityItemCombine(x => x.ReqFacilityLv == kMainTab.kSelectTab + 1);
        m_OldSelectID = _seleteid;
        _seleteid = -1;
        if (_facilityitemcombinelist.Count != 0)
        {
            for( int i = 0; i < _facilityitemcombinelist.Count; i++ )
            {
                if( _facilityitemcombinelist[i].ID == (int)_facilitydata.Selete )
                    _seleteid = _facilityitemcombinelist[i].ID;
            }
            if(_seleteid == -1)
                _seleteid = _facilityitemcombinelist[0].ID;
        }

        //if (_facilitydata.Level < _facilityitemcombinelist.Find(x => x.ID == _seleteid).ReqFacilityLv)
        //{
        //    MessageToastPopup.Show(FLocalizeString.Instance.GetText(3047));
        //    _seleteid = m_OldSelectID;
        //    return false;
        //}

        _ItemListInstance.UpdateList();
        SetSeleteID(_seleteid);
        return true;
	}
	
	private void _UpdateItemListSlot(int index, GameObject slotObject)
	{
        do
        {
            UIFacilityItemCombineListSlot card = slotObject.GetComponent<UIFacilityItemCombineListSlot>();
            if (null == card) break;

            GameTable.FacilityItemCombine.Param data = null;
            if (0 <= index && _facilityitemcombinelist.Count > index)
            {
                data = _facilityitemcombinelist[index];
            }
            card.ParentGO = this.gameObject;
            card.UpdateSlot(index, data);
        } while (false);
    }
	
	private int _GetItemElementCount()
	{
		return _facilityitemcombinelist.Count;
	}

	
	public void OnClick_CloseBtn()
	{
        OnClickClose();
	}
	
	public void OnClick_SeleteBtn()
	{
        if (_facilitydata.Level < _facilityitemcombinelist.Find(x => x.ID == _seleteid).ReqFacilityLv)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3047));
            return;
        }

        if (!GameSupport.IsCheckGoods(eGOODSTYPE.GOLD, _itemReqlist.Gold * _itemCombineCount))
        {
            return;
        }

        List<int> idlist = new List<int>();
        List<int> countlist = new List<int>();
        idlist.Add(_itemReqlist.ItemID1);
        idlist.Add(_itemReqlist.ItemID2);
        idlist.Add(_itemReqlist.ItemID3);
        idlist.Add(_itemReqlist.ItemID4);
        countlist.Add(_itemReqlist.Count1);
        countlist.Add(_itemReqlist.Count2);
        countlist.Add(_itemReqlist.Count3);
        countlist.Add(_itemReqlist.Count4);

        int maxCnt = 0;

        for (int i = 0; i < idlist.Count; i++)
        {
            if (idlist[i].Equals(-1))
                continue;
            ItemData checkItem = GameInfo.Instance.GetItemData(idlist[i]);
            if (checkItem != null)
            {
                if(checkItem.Count * _itemCombineCount < countlist[i] * _itemCombineCount)
                {
                    //MessageToastPopup.Show(FLocalizeString.Instance.GetText(3003));
                    ItemBuyMessagePopup.ShowItemBuyPopup(idlist[i], checkItem.Count, countlist[i] * _itemCombineCount);
                    return;
                }
            }
            else
            {
                //재료가 존재 하지 않음
                //MessageToastPopup.Show(FLocalizeString.Instance.GetText(3003));
                ItemBuyMessagePopup.ShowItemBuyPopup(idlist[i], (int)eCOUNT.NONE, countlist[i] * _itemCombineCount);
                return;
            }
        }

        if(GameInfo.Instance.netFlag)
        {
            GameInfo.Instance.Send_FacilityItemEquip(_facilitydata.TableID, _seleteid, _itemCombineCount, OnNetFacilityUse);
        }
        else
        {
            GameInfo.Instance.Send_FacilityItemEquip(_facilitydata.TableID, _seleteid, _itemCombineCount, OnNetFacilityItemEquip);

        }
    }

    //UIFacilityItemCombineListSlot에서 아이템을 터치 했을때 들어온다.
    public void SetSeleteID(int id)
    {
        _seleteid = id;

        _ItemListInstance.RefreshNotMove();

        SetCombineDataWithMaxCountCheck();

        Renewal(false);
    }

    //보유 아이템 재화에 따른 최대갯수 계산 및 조합 가능여부 확인
    private void SetCombineDataWithMaxCountCheck()
    {
        _itemCombineCount = 1;
        _itemCombineMaxCount = 1;

        _combineParam = GameInfo.Instance.GameTable.FindFacilityItemCombine((int)_seleteid);
        if(_combineParam == null)
        {
            Debug.LogError("Table에 " + _seleteid + " 시설 아이템 조합식이 존재하지 않습니다.");
            return;
        }

        _itemReqlist = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == _combineParam.ItemReqGroup);
        if(_itemReqlist == null)
        {
            Debug.LogError("Table에 " + _seleteid + " 아이템 조합 ItemRegGroup가 존재하지 않습니다.");
            return;
        }

        _itemCombineMaxCount = Mathf.Min(GetItemCombineMaxCount(), GameInfo.Instance.GameConfig.FacCombineCntMax);
    }

	public void OnNetFacilityItemEquip(int result, PktMsgType pktmsg)
    {
        
        if (result != 0)
            return;
        GameInfo.Instance.Send_ReqFacilityOperation(_facilitydata.TableID, -1, _itemCombineCount, null, OnNetFacilityUse);
       
    }

    public void OnNetFacilityUse(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyFacility lobbyFacility = Lobby.Instance.GetLobbyFacility(_facilitydata.TableData.ParentsID);
        if (lobbyFacility != null)
        {
            lobbyFacility.InitLobbyFacility(_facilitydata);
        }

        var carddata = GameInfo.Instance.GetCardData(_facilitydata.EquipCardUID);
        
        if (carddata != null)
        {
            VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.FacilityStart, carddata.TableID);
        }

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("FacilityItemPanel");
        OnClick_CloseBtn();
    }

    public void OnNetFacilityItemRemove(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("FacilityItemPanel");
        OnClick_CloseBtn();
    }

    public void OnClickItemCombineCountPlus()
    {
        _itemCombineCount++;
        if (_itemCombineCount > _itemCombineMaxCount)
            _itemCombineCount = _itemCombineMaxCount;

        Renewal(false);
    }

    public void OnClickItemCombineCountMinus()
    {
        _itemCombineCount--;
        if (_itemCombineCount <= 0)
            _itemCombineCount = 1;

        Renewal(false);
    }

    public void OnClickItemCombineCountMax()
    {
        _itemCombineCount = _itemCombineMaxCount;
        Renewal(false);
    }

	private int GetItemCombineMaxCount() {
		List<int> idlist = new List<int>() {
			_itemReqlist.ItemID1, _itemReqlist.ItemID2, _itemReqlist.ItemID3, _itemReqlist.ItemID4,
		};

		List<int> countlist = new List<int>() {
			_itemReqlist.Count1, _itemReqlist.Count2, _itemReqlist.Count3, _itemReqlist.Count4,
		};

		if ( mItemCombineMaxCountList == null ) {
			mItemCombineMaxCountList = new List<int>();
			mItemCombineMaxCountList.Capacity = idlist.Count;
		}

		mItemCombineMaxCountList.Clear();

		for ( int i = 0; i < idlist.Count; i++ ) {
			if ( idlist[i] <= 0 ) {
				continue;
			}

			ItemData checkItem = GameInfo.Instance.GetItemData( idlist[i] );
			if ( checkItem == null ) {
				mItemCombineMaxCountList.Clear();
				break;
			}

			mItemCombineMaxCountList.Add( checkItem.Count / countlist[i] );
		}

		mItemCombineMaxCountList.Sort( ( f, b ) => {
			if ( f < b ) {
				return -1;
			}

			if ( f > b ) {
				return 1;
			}

			return 0;
		} );

		int maxCount = mItemCombineMaxCountList.FirstOrDefault();
		if ( maxCount <= 0 ) {
			maxCount = 1;
		}

		return maxCount;
	}
}
