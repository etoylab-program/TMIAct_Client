using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIFacilityCardSeletePopup : FComponent
{
    public enum eSelectState
    {
        NONE,
        FACILITY,
        TRADE_MATERIAL,
    }
    public UILabel kTimeLabel;
    public UILabel kEffectLabel;
    public UIButton kChangeBtn;
    public UILabel kChangeBtnLabel;
    public UIButton kRemoveBtn;

    [SerializeField] private FList _ItemListInstance;
    private List<WeaponData> _weaponlist = new List<WeaponData>();
    private List<CardData> _cardlist = new List<CardData>();
    private FacilityData _facilitydata;
    private CardData _carddata;
    private CardBookData _cardbookdata;

    public FacilityData Facilitydata { get { return _facilitydata; } }

    public long SeleteCardUID
    {
        get
        {
            if (_carddata == null)
                return -1;
            return _carddata.CardUID;
        }
    }

    private eSelectState _SelectState = eSelectState.NONE;

    [Header("Trade")]
    public UIButton kClearAllBtn;
    public GameObject kMaterialObj;
    public List<UIItemListSlot> kTradeItemList;
    public List<GameObject> kTradeItemEmptyList;
    public List<GameObject> kTradeItemLockList;
    public List<UILabel> kTradeItemLockLabelList;

    //public long[] TradeMaterialUIDList = new long[4];

    public override void Awake()
    {
        base.Awake();

        if (this._ItemListInstance == null) return;

        this._ItemListInstance.EventUpdate = this._UpdateItemListSlot;
        this._ItemListInstance.EventGetItemCount = this._GetItemElementCount;
        this._ItemListInstance.InitBottomFixing();
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    private int SortOptValue(CardData data1, CardData data2)
    {
        if (data1.SkillLv < data2.SkillLv) return -1;
        if (data1.SkillLv > data2.SkillLv) return 1;
        return 0;
    }

    public override void InitComponent()
    {
        _facilitydata = null;
        _cardlist.Clear();
        List<CardData> tmpOptcardlist = new List<CardData>();

        int id = (int)UIValue.Instance.GetValue(UIValue.EParamType.FacilityID);
        _facilitydata = GameInfo.Instance.GetFacilityData(id);

        _SelectState = eSelectState.FACILITY;

        SetTradeObject(false);
        var tradematerial = UIValue.Instance.GetValue(UIValue.EParamType.TradeMaterial, true);
        if (tradematerial != null)
        {
            _SelectState = eSelectState.TRADE_MATERIAL;
            InitComponent_TRADE_MATERIAL();
        }
        else
        {
            if(_facilitydata.TableData.EffectType.Equals("FAC_CARD_TRADE"))
                FacilitySupport.CopyToTempTradeMaterialUIDs(GameInfo.Instance.TradeMaterialUIDList);
        }


        for (int i = 0; i < GameInfo.Instance.CardList.Count; i++)
        {
            if (_SelectState == eSelectState.FACILITY)
            {
                if (_facilitydata.EquipCardUID != GameInfo.Instance.CardList[i].CardUID &&
                        GameSupport.IsEquipAndUsingCardData(GameInfo.Instance.CardList[i].CardUID) == false &&
                        GameInfo.Instance.GetEquiCardCharData(GameInfo.Instance.CardList[i].CardUID) == null)
                {
                    if (_facilitydata.TableData.EffectType.Equals("FAC_CARD_TRADE"))
                    {
                        if (!FacilitySupport.IsTradeSelectMaterial(GameInfo.Instance.CardList[i].CardUID))
                        {
                            if (GameSupport.GetFacilitySptOptValRate(_facilitydata, GameInfo.Instance.CardList[i]) == 0.0f)
                                _cardlist.Add(GameInfo.Instance.CardList[i]);
                            else
                                tmpOptcardlist.Add(GameInfo.Instance.CardList[i]);
                        }
                    }
                    else
                    {
                        if (GameSupport.GetFacilitySptOptValRate(_facilitydata, GameInfo.Instance.CardList[i]) == 0.0f)
                            _cardlist.Add(GameInfo.Instance.CardList[i]);
                        else
                            tmpOptcardlist.Add(GameInfo.Instance.CardList[i]);
                    }
                }
            }
            else if(_SelectState == eSelectState.TRADE_MATERIAL)
            {
                if (_facilitydata.EquipCardUID != GameInfo.Instance.CardList[i].CardUID &&
                    !GameInfo.Instance.CardList[i].Lock &&
                    GameInfo.Instance.CardList[i].TableData.Selectable == 1 &&
                    GameSupport.IsEquipAndUsingCardData(GameInfo.Instance.CardList[i].CardUID) == false &&
                    GameInfo.Instance.GetEquiCardCharData(GameInfo.Instance.CardList[i].CardUID) == null)
                {
                    if (GameSupport.GetFacilitySptOptValRate(_facilitydata, GameInfo.Instance.CardList[i]) == 0.0f)
                        _cardlist.Add(GameInfo.Instance.CardList[i]);
                    else
                        tmpOptcardlist.Add(GameInfo.Instance.CardList[i]);
                }
            }
        }

        CardData.SortUp = false;
        _cardlist.Sort(CardData.CompareFuncGradeLevel);

        tmpOptcardlist.Sort(SortOptValue);
        for (int i = 0; i < tmpOptcardlist.Count; i++)
        {
            _cardlist.Insert(0, tmpOptcardlist[i]);
        }

        kChangeBtnLabel.textlocalize = FLocalizeString.Instance.GetText(13);
        if (_SelectState == eSelectState.FACILITY)
        {   
            if (_facilitydata.EquipCardUID != (int)eCOUNT.NONE)
            {
                _cardlist.Insert(0, GameInfo.Instance.GetCardData(_facilitydata.EquipCardUID));
            }
        }
        else if (_SelectState == eSelectState.TRADE_MATERIAL)
        {
            kChangeBtnLabel.textlocalize = FLocalizeString.Instance.GetText(1);
        }

        _carddata = null;
        if (_cardlist.Count != 0)
        {
            _carddata = _cardlist[0];
            _cardbookdata = GameInfo.Instance.GetCardBookData(_carddata.TableID);
            if (_facilitydata.EquipCardUID != (int)eCOUNT.NONE)
            {
            }
        }

        _ItemListInstance.UpdateList();
    }

    private void InitComponent_TRADE_MATERIAL()
    {
        //데이터 복사
        FacilitySupport.CopyToTempTradeMaterialUIDs(GameInfo.Instance.TradeMaterialUIDList);        

        kMaterialObj.SetActive(true);
        kClearAllBtn.SetActive(true);
        kChangeBtn.SetActive(true);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        switch(_SelectState)
        {
            case eSelectState.FACILITY:
                {
                    kTimeLabel.textlocalize = "";
                    kEffectLabel.textlocalize = "";

                    kChangeBtn.gameObject.SetActive(false);
                    kRemoveBtn.gameObject.SetActive(false);
                    kTimeLabel.gameObject.SetActive(true);
                    kEffectLabel.gameObject.SetActive(false);

                    if (_carddata == null)
                        return;

                    float time = GameInfo.Instance.GameConfig.FacilityCardGradeBaseSubTimeRatio[_carddata.TableData.Grade] + (_carddata.Level * GameInfo.Instance.GameConfig.FacilityCardGradeSubTimeRatio[_carddata.TableData.Grade]);
                    kTimeLabel.textlocalize = $"{FLocalizeString.Instance.GetText(1712)} : {FLocalizeString.Instance.GetText(1057, time)}";

                    float rate = GameSupport.GetFacilitySptOptValRate(_facilitydata, _carddata);
                    if (rate != 0.0f)
                    {
                        kEffectLabel.gameObject.SetActive(true);
                        int nTextId = 1397;
                        if (_facilitydata.TableData.EffectType.Equals("FAC_ITEM_COMBINE"))
                        {
                            nTextId = 1398;
                        }
                        
                        kEffectLabel.textlocalize = $"{FLocalizeString.Instance.GetText(1713)} : {FLocalizeString.Instance.GetText(nTextId, rate * (float)eCOUNT.MAX_BO_FUNC_VALUE)}";
                    }

                    if (_facilitydata.EquipCardUID == _carddata.CardUID)
                    {
                        kChangeBtn.gameObject.SetActive(false);
                        kRemoveBtn.gameObject.SetActive(true);
                    }
                    else
                    {
                        kChangeBtn.gameObject.SetActive(true);
                        kRemoveBtn.gameObject.SetActive(false);
                    }
                }
                break;
            case eSelectState.TRADE_MATERIAL:
                Renewal_TRADE_MATERIAL();
                break;
        }

        _ItemListInstance.RefreshNotMove();
    }

    private void Renewal_TRADE_MATERIAL()
    {
        kTimeLabel.SetActive(false);
        kEffectLabel.SetActive(false);
        kRemoveBtn.SetActive(false);

        kChangeBtn.SetActive(true);

        // Empty/Lock 슬롯 상태 설정
        int itemMaxCount = kTradeItemList.Count;
        int itemEmptyCount = _facilitydata.TableData.EffectValue + (_facilitydata.Level - 1);
        int NextReqLevel = 1;

        for (int i = 0; i < itemMaxCount; i++)
        {
            kTradeItemList[i].SetActive(false);

            if (i < itemEmptyCount)
            {
                // 재료데이터 설정에 따른 Empty 슬로 상태 설정
                if (FacilitySupport.TempTradeMaterialUIDs.Length == 0)
                {
                    kTradeItemEmptyList[i].SetActive(true);
                }
                else if (i >= FacilitySupport.TempTradeMaterialUIDs.Length || FacilitySupport.TempTradeMaterialUIDs[i] <= 0)
                    kTradeItemEmptyList[i].SetActive(true);
            }
            else
            {
                kTradeItemLockList[i].SetActive(true);
                kTradeItemLockLabelList[i].textlocalize = string.Format(FLocalizeString.Instance.GetText(1365), _facilitydata.Level + NextReqLevel++);
            }
        }

        // 재료 아이템 슬롯 
        for (int i = 0; i < FacilitySupport.TempTradeMaterialUIDs.Length; i++)
        {
            if (FacilitySupport.TempTradeMaterialUIDs[i] <= 0) continue;
            if (kTradeItemList[i] == null) continue;
            UIItemListSlot slot = kTradeItemList[i].GetComponent<UIItemListSlot>();
            if (slot == null) continue;

            CardData w = GameInfo.Instance.CardList.Find(x => x.CardUID == FacilitySupport.TempTradeMaterialUIDs[i]);
            if (w != null)
            {
                kTradeItemEmptyList[i].SetActive(false);
                kTradeItemList[i].SetActive(true);
                slot.ParentGO = slot.gameObject;
                slot.UpdateSlot(UIItemListSlot.ePosType.FacilityMaterial_Card, i, w);
            }
        }
    }

    private void _UpdateItemListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIItemListSlot card = slotObject.GetComponent<UIItemListSlot>();
            if (null == card) break;

            CardData data = null;
            if (0 <= index && _cardlist.Count > index)
            {
                data = _cardlist[index];
            }

            card.ParentGO = this.gameObject;
            switch (_SelectState)
            {
                case eSelectState.FACILITY:
                    card.UpdateSlot(UIItemListSlot.ePosType.FacilityCard_SeleteList, index, data);
                    break;
                case eSelectState.TRADE_MATERIAL:
                    card.UpdateSlot(UIItemListSlot.ePosType.FacilityMaterial_Card, index, data);
                    break;
            }
            
        } while (false);
    }

    private int _GetItemElementCount()
    {
        return _cardlist.Count;
    }

    public void SetSeleteCardUID(long uid)
    {
        var carddata = GameInfo.Instance.GetCardData(uid);
        if (carddata == null)
            return;
       
        _carddata = carddata;
        _cardbookdata = GameInfo.Instance.GetCardBookData(_carddata.TableID);
        Renewal(true);
    }

    public void OnClick_BackBtn()
    {
        OnClickClose();
    }

    public void OnClick_ChangeBtn()
    {
        if (_carddata == null)
            return;

        switch (_SelectState)
        {
            case eSelectState.FACILITY:
                {
                    CharData chardata = GameInfo.Instance.GetEquiCardCharData(_carddata.CardUID);
                    if (chardata != null)
                    {
                        string strCard = FLocalizeString.Instance.GetText(1071);    //  서포터
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3025, strCard));
                        return;
                    }

                    if (GameSupport.IsEquipAndUsingCardData(_carddata.CardUID))
                    {
                        string strCard = FLocalizeString.Instance.GetText(1071);    //  서포터
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3026, strCard));
                        return;
                    }

                    GameInfo.Instance.Send_ReqFacilityCardEquip(_facilitydata.TableID, _carddata.CardUID, OnNetFacilityCardEquip);
                }
                break;
            case eSelectState.TRADE_MATERIAL:
                {
                    //재료 적용                    
                    GameInfo.Instance.SetTradeMaterialUID(FacilitySupport.TempTradeMaterialUIDs);
                    LobbyUIManager.Instance.Renewal("FacilityPanel");
                    OnClickClose();
                }
                break;
        }
    }

    public void OnClick_RemoveBtn()
    {
        if (_carddata == null)
            return;


        GameInfo.Instance.Send_ReqFacilityCardRemove(_facilitydata.EquipCardUID, OnNetFacilityCardRemove);

    }

    public void OnClick_AllClear()
    {
        if (_SelectState != eSelectState.TRADE_MATERIAL)
            return;

        for (int i = 0; i < FacilitySupport.TempTradeMaterialUIDs.Length; i++)
        {
            FacilitySupport.TempTradeMaterialUIDs[i] = 0;
        }

        Renewal(true);
    }

    public void OnNetFacilityCardEquip(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("FacilityPanel");
        LobbyUIManager.Instance.Renewal("FacilityItemPanel");
        OnClick_BackBtn();
    }

    public void OnNetFacilityCardRemove(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("FacilityPanel");
        LobbyUIManager.Instance.Renewal("FacilityItemPanel");
        OnClick_BackBtn();
    }


    private void SetTradeObject(bool state)
    {
        kClearAllBtn.SetActive(state);
        kMaterialObj.SetActive(state);
        if (kTradeItemList != null)
        {
            for (int i = 0; i < kTradeItemList.Count; i++)
            {
                if (kTradeItemList[i] != null) kTradeItemList[i].SetActive(state);
            }
        }

        if (kTradeItemEmptyList != null)
        {
            for (int i = 0; i < kTradeItemEmptyList.Count; i++)
            {
                if (kTradeItemEmptyList[i] != null) kTradeItemEmptyList[i].SetActive(state);
            }
        }

        if (kTradeItemLockList != null)
        {
            for (int i = 0; i < kTradeItemLockList.Count; i++)
            {
                if (kTradeItemLockList[i] != null) kTradeItemLockList[i].SetActive(state);
            }
        }
    }
}
