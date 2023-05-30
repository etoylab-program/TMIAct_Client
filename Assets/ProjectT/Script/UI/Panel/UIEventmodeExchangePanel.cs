using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEventmodeExchangePanel : FComponent
{
    public FTab kDifficultyTab;
    [SerializeField] private FList _EventExchangeRewardListInstance;
	public UITexture kBGTex;
	public UILabel kCaution_Label;

    public List<UIEventmodeExchangeMainPanel.UIGoodsTex> kGoodsUnits;

    private int _eventId;
    private EventSetData _eventSetData;
    private List<GameTable.EventExchangeReward.Param> _selEventChangeList = new List<GameTable.EventExchangeReward.Param>();

    private int _selectTabIdx = 0;

	public override void Awake()
	{
		base.Awake();

		if(this._EventExchangeRewardListInstance == null) return;
		this._EventExchangeRewardListInstance.InitBottomFixing();
		this._EventExchangeRewardListInstance.EventUpdate = this._UpdateEventExchangeRewardListSlot;
		this._EventExchangeRewardListInstance.EventGetItemCount = this._GetEventExchangeRewardElementCount;

        kDifficultyTab.EventCallBack = OnDifficultySelect;

    }
 
	public override void OnEnable()
	{
        _selectTabIdx = 1;
        InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        _eventId = (int)UIValue.Instance.GetValue(UIValue.EParamType.EventID);
        _eventSetData = GameInfo.Instance.EventSetDataList.Find(x => x.TableID == _eventId && x.RewardStep == _selectTabIdx);
        if (_eventSetData == null)
            return;

        kDifficultyTab.DisableTab();

        for(int i = 0; i < kDifficultyTab.kBtnList.Count; i++)
        {
            GameTable.EventExchangeReward.Param tabCheckData = GameInfo.Instance.GameTable.FindEventExchangeReward(x => x.EventID == _eventId && x.RewardStep == i + 1);
            if (tabCheckData == null)
                kDifficultyTab.SetEnabled(i, false);
            else
                kDifficultyTab.SetEnabled(i, true);
        }
    
        kDifficultyTab.SetTab(_selectTabIdx - 1, SelectEvent.Code);

        Renewal(true);
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        BannerData bannerdataBG = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.EVENT_MAINBG && x.BannerTypeValue == _eventSetData.TableID);
        if (bannerdataBG != null)
        {
			if (kBGTex.mainTexture != null)
			{
				DestroyImmediate(kBGTex.mainTexture, false);
				kBGTex.mainTexture = null;
			}

            kBGTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(bannerdataBG.UrlImage, true);
        }

        SetGoods();
        _EventExchangeRewardListInstance.RefreshNotMove();
    }

	private void _UpdateEventExchangeRewardListSlot(int index, GameObject slotObject)
	{
		do
		{
            UIEventExchangeRewardListSlot uIEventExchangeRewardListSlot = slotObject.GetComponent<UIEventExchangeRewardListSlot>();
            if (null == uIEventExchangeRewardListSlot) break;

            uIEventExchangeRewardListSlot.ParentGO = this.gameObject;
            uIEventExchangeRewardListSlot.UpdateSlot(index, _eventSetData.RewardItemCount[index], _selEventChangeList[index]);
        } while(false);
	}
	
	private int _GetEventExchangeRewardElementCount()
	{
        if(_selEventChangeList == null || _selEventChangeList.Count <= 0)
		    return 0; //TempValue

        return _selEventChangeList.Count;
	}

    private bool OnDifficultySelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
            return false;
        _selectTabIdx = nSelect + 1;
        _selEventChangeList = GameInfo.Instance.GameTable.FindAllEventExchangeReward(x => x.EventID == _eventId && x.RewardStep == _selectTabIdx);
        _eventSetData = GameInfo.Instance.EventSetDataList.Find(x => x.TableID == _eventId && x.RewardStep == _selectTabIdx);
        _EventExchangeRewardListInstance.UpdateList();
        return true;
        //return MultiTabCheck(kRare, ref _rareFilter, nSelect, type);
    }

    private void SetGoods()
    {
        SetGoods(kGoodsUnits[0], _eventSetData.TableData.EventItemID1);
        SetGoods(kGoodsUnits[1], _eventSetData.TableData.EventItemID2);
        SetGoods(kGoodsUnits[2], _eventSetData.TableData.EventItemID3);
        SetGoods(kGoodsUnits[3], _eventSetData.TableData.EventItemID4);
        SetGoods(kGoodsUnits[4], _eventSetData.TableData.EventItemID5);
        SetGoods(kGoodsUnits[5], _eventSetData.TableData.EventItemID6);
    }

    private void SetGoods(UIEventmodeExchangeMainPanel.UIGoodsTex uIGoodsTex, int itemTableID)
    {
        if (itemTableID == 0)
        {
            uIGoodsTex.SetActive(false);
            return;
        }

        uIGoodsTex.SetActive(true);

        string texPath = "Icon/Item/" + GameInfo.Instance.GameTable.FindItem(x => x.ID == itemTableID).Icon;

        uIGoodsTex.texIcon.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", texPath);
        uIGoodsTex.goodsUnit.kTextLabel.textlocalize = GameInfo.Instance.GetItemIDCount(itemTableID).ToString();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (!GameInfo.Instance.netFlag)
            {
                if (_eventSetData.TableData.EventItemID1 == 0)
                    return;
                NetLocalSvr.Instance.AddItem(_eventSetData.TableData.EventItemID1, 10);
                NetLocalSvr.Instance.SaveLocalData();
                Renewal(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (!GameInfo.Instance.netFlag)
            {
                if (_eventSetData.TableData.EventItemID2 == 0)
                    return;
                NetLocalSvr.Instance.AddItem(_eventSetData.TableData.EventItemID2, 10);
                NetLocalSvr.Instance.SaveLocalData();
                Renewal(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            if (!GameInfo.Instance.netFlag)
            {
                if (_eventSetData.TableData.EventItemID3 == 0)
                    return;
                NetLocalSvr.Instance.AddItem(_eventSetData.TableData.EventItemID3, 10);
                NetLocalSvr.Instance.SaveLocalData();
                Renewal(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            if (!GameInfo.Instance.netFlag)
            {
                if (_eventSetData.TableData.EventItemID4 == 0)
                    return;
                NetLocalSvr.Instance.AddItem(_eventSetData.TableData.EventItemID4, 10);
                NetLocalSvr.Instance.SaveLocalData();
                Renewal(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            if (!GameInfo.Instance.netFlag)
            {
                if (_eventSetData.TableData.EventItemID5 == 0)
                    return;
                NetLocalSvr.Instance.AddItem(_eventSetData.TableData.EventItemID5, 10);
                NetLocalSvr.Instance.SaveLocalData();
                Renewal(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            if (!GameInfo.Instance.netFlag)
            {
                if (_eventSetData.TableData.EventItemID6 == 0)
                    return;
                NetLocalSvr.Instance.AddItem(_eventSetData.TableData.EventItemID6, 10);
                NetLocalSvr.Instance.SaveLocalData();
                Renewal(true);
            }
        }
    }
#endif
}
