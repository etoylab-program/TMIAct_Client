using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFacilityWeaponSeletePopup : FComponent
{
    public enum eWeaponSeleteState
    {
        NONE,
        FACILITY,
        ARMORY,        
        TRADE_MATERIAL,
    }

    public UIButton kChangeBtn;
    public UILabel kChangeBtnLabel;
    [SerializeField] private FList _ItemListInstance;
    private List<WeaponData> _weaponlist = new List<WeaponData>();
    private FacilityData _facilitydata;
    private WeaponData _weapondata;
    private CardBookData _cardbookdata;

    private eWeaponSeleteState _weaponSeleteState = eWeaponSeleteState.NONE;
    private long _armoryWeaponUID = 0;
    private int _tradeMaterialSlotIndex = 0;

    [Header("Trade")]
    public UIButton kClearAllBtn;
    public GameObject kMaterialObj;
    public List<UIItemListSlot> kTradeItemList;
    public List<GameObject> kTradeItemEmptyList;    
    public List<GameObject> kTradeItemLockList;
    public List<UILabel> kTradeItemLockLabelList;

    public FacilityData Facilitydata { get { return _facilitydata; } }
    //public long[] TradeMaterialUIDList = new long[4];

    public long SeleteWeaponUID
    {
        get
        {
            if (_weapondata == null)
                return -1;
            return _weapondata.WeaponUID;
        }
    }

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

    public override void InitComponent()
    {
        _facilitydata = null;
        _weaponlist.Clear();

        _weaponSeleteState = eWeaponSeleteState.NONE;

        var facilityid = UIValue.Instance.GetValue(UIValue.EParamType.FacilityID, true);
        if (facilityid != null)
        {
            _weaponSeleteState = eWeaponSeleteState.FACILITY;
            _facilitydata = GameInfo.Instance.GetFacilityData((int)facilityid);
        }
        
        if (LobbyUIManager.Instance.IsActiveUI("ArmoryPopup"))
        {
            _weaponSeleteState = eWeaponSeleteState.ARMORY;
        }

        SetTradeObject(false);
        var tradematerial = UIValue.Instance.GetValue(UIValue.EParamType.TradeMaterial, true);
        if (tradematerial != null)
        {
            _weaponSeleteState = eWeaponSeleteState.TRADE_MATERIAL;
            _tradeMaterialSlotIndex = (int)tradematerial;
            InitComponent_TRADE_MATERIAL();
        }


        for (int i = 0; i < GameInfo.Instance.WeaponList.Count; i++)
        {
            switch (_weaponSeleteState)
            {
                case eWeaponSeleteState.FACILITY:
                
                    {
                        if (_facilitydata.Selete != GameInfo.Instance.WeaponList[i].WeaponUID &&
                            GameInfo.Instance.GetEquipWeaponFacilityData(GameInfo.Instance.WeaponList[i].WeaponUID) == null &&
                            GameInfo.Instance.GetEquipWeaponCharData(GameInfo.Instance.WeaponList[i].WeaponUID) == null &&
                            !GameSupport.GetEquipWeaponDepot(GameInfo.Instance.WeaponList[i].WeaponUID))
                        {
                            _weaponlist.Add(GameInfo.Instance.WeaponList[i]);
                        }
                    }
                    break;
                case eWeaponSeleteState.ARMORY:
                    {
                        if (GameInfo.Instance.GetEquipWeaponFacilityData(GameInfo.Instance.WeaponList[i].WeaponUID) == null &&
                            GameInfo.Instance.GetEquipWeaponCharData(GameInfo.Instance.WeaponList[i].WeaponUID) == null &&
                            !GameSupport.GetEquipWeaponDepot(GameInfo.Instance.WeaponList[i].WeaponUID))
                        {
                            _weaponlist.Add(GameInfo.Instance.WeaponList[i]);
                        }

                        var armoryUID = UIValue.Instance.GetValue(UIValue.EParamType.ArmoryWeaponUID);
                        if (armoryUID == null)
                            _armoryWeaponUID = (int)eCOUNT.NONE;
                        else
                            _armoryWeaponUID = (long)armoryUID;

                        

                        Log.Show(_armoryWeaponUID);
                    }
                    break;
                    
                case eWeaponSeleteState.TRADE_MATERIAL:
                    {
                        if (GameInfo.Instance.WeaponList[i].TableData.Selectable == 1 &&
                            !GameInfo.Instance.WeaponList[i].Lock &&
                            GameInfo.Instance.GetEquipWeaponFacilityData(GameInfo.Instance.WeaponList[i].WeaponUID) == null &&
                            GameInfo.Instance.GetEquipWeaponCharData(GameInfo.Instance.WeaponList[i].WeaponUID) == null &&
                            !GameSupport.GetEquipWeaponDepot(GameInfo.Instance.WeaponList[i].WeaponUID))
                        {   
                            _weaponlist.Add(GameInfo.Instance.WeaponList[i]);
                        }
                    }
                    break;
            }
        }

        WeaponData.SortUp = false;
        _weaponlist.Sort(WeaponData.CompareFuncGradeLevel);
        if (_weaponSeleteState == eWeaponSeleteState.ARMORY)
        {
            if (_armoryWeaponUID != (int)eCOUNT.NONE)
            {
                WeaponData armoryWeapon = GameInfo.Instance.GetWeaponData(_armoryWeaponUID);
                _weaponlist.Insert(0, armoryWeapon);
            }
        }


        _weapondata = null;
        if (_weaponlist.Count != 0)
        {
            _weapondata = _weaponlist[0];
        }

        _ItemListInstance.UpdateList();

        Renewal(true);
    }

    private void InitComponent_TRADE_MATERIAL()
    {
        //데이터 복사
        FacilitySupport.CopyToTempTradeMaterialUIDs(GameInfo.Instance.TradeMaterialUIDList);
        
        kMaterialObj.SetActive(true);
        kClearAllBtn.SetActive(true);
        kChangeBtn.SetActive(true);

        kChangeBtnLabel.textlocalize = FLocalizeString.Instance.GetText(1);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        switch (_weaponSeleteState)
        {
            case eWeaponSeleteState.FACILITY:
                {
                    kChangeBtn.gameObject.SetActive(false);

                    if (_weapondata == null)
                        return;
                    kChangeBtnLabel.textlocalize = FLocalizeString.Instance.GetText(1118);
                    kChangeBtn.gameObject.SetActive(true);
                }
                break;
            case eWeaponSeleteState.ARMORY:
                {
                    kChangeBtn.gameObject.SetActive(false);

                    if (_weapondata == null)
                        return;

                    if (_armoryWeaponUID == (int)eCOUNT.NONE)
                    {
                        if (GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList.Contains(_weapondata.WeaponUID))
                        {
                            //해제
                            kChangeBtnLabel.textlocalize = FLocalizeString.Instance.GetText(1121);
                        }
                        else
                        {
                            //빈슬롯 등록
                            kChangeBtnLabel.textlocalize = FLocalizeString.Instance.GetText(1);
                        }
                        
                    }
                    else
                    {
                        if (_weapondata.WeaponUID == _armoryWeaponUID)
                        {
                            //해제
                            kChangeBtnLabel.textlocalize = FLocalizeString.Instance.GetText(1121);
                        }
                        else
                        {
                            //교체
                            kChangeBtnLabel.textlocalize = FLocalizeString.Instance.GetText(6);
                        }
                    }

                    kChangeBtn.gameObject.SetActive(true);
                }
                break;            
            case eWeaponSeleteState.TRADE_MATERIAL:
                {
                    Renewal_TRADE_MATERIAL();
                }
                break;
        }
        

        _ItemListInstance.RefreshNotMove();
    }

    private void Renewal_TRADE_MATERIAL()
    {
        kChangeBtn.gameObject.SetActive(true);

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

            WeaponData w = GameInfo.Instance.WeaponList.Find(x => x.WeaponUID == FacilitySupport.TempTradeMaterialUIDs[i]);
            if (w != null)
            {
                kTradeItemEmptyList[i].SetActive(false);
                kTradeItemList[i].SetActive(true);
                slot.ParentGO = slot.gameObject;
                slot.UpdateSlot(UIItemListSlot.ePosType.FacilityMaterial_Weapon, i, w);
            }
        }
    }

    private void _UpdateItemListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIItemListSlot card = slotObject.GetComponent<UIItemListSlot>();
            if (null == card) break;

            WeaponData data = null;
            if (0 <= index && _weaponlist.Count > index)
            {
                data = _weaponlist[index];
            }

            card.ParentGO = this.gameObject;
            switch (_weaponSeleteState)
            {
                case eWeaponSeleteState.FACILITY:                                
                    card.UpdateSlot(UIItemListSlot.ePosType.FacilityWeapon_SeleteList, index, data);
                    break;
                case eWeaponSeleteState.ARMORY:
                    card.UpdateSlot(UIItemListSlot.ePosType.ArmoryWeapon_SeleteList, index, data);
                    break;
                case eWeaponSeleteState.TRADE_MATERIAL:
                    card.UpdateSlot(UIItemListSlot.ePosType.FacilityMaterial_Weapon, index, data);
                    break;
            }
            
        } while (false);
    }

    private int _GetItemElementCount()
    {
        return _weaponlist.Count;
    }

    public void SetSeleteWeaponUID(long uid)
    {
        var weapondata = GameInfo.Instance.GetWeaponData(uid);
        if (weapondata == null)
            return;
        
        _weapondata = weapondata;
        
        Renewal(true);
    }

    public void OnClick_BackBtn()
    {
        OnClickClose();
    }

    public void OnClick_ChangeBtn()
    {
        if (_weapondata == null)
            return;

        // 최대 조건 무관 시설 이용 자체는 가능하도록 처리
        //if (GameSupport.IsMaxLevelWeapon(_weapondata))
        //{
        //    //최대 레벨입니다.
        //    MessageToastPopup.Show(FLocalizeString.Instance.GetText(3007));
        //    return;
        //}

        switch (_weaponSeleteState)
        {
            case eWeaponSeleteState.FACILITY:
                {
                    if (GameInfo.Instance.netFlag)
                    {
                        GameInfo.Instance.Send_FacilityItemEquip(_facilitydata.TableID, _weapondata.WeaponUID, 1, OnNetFacilityUse);
                    }
                    else
                    {
                        GameInfo.Instance.Send_FacilityItemEquip(_facilitydata.TableID, _weapondata.WeaponUID, 1, OnNetFacilityItemEquip);
                    }
                }
                break;
            case eWeaponSeleteState.ARMORY:
                {
                    if (_armoryWeaponUID != _weapondata.WeaponUID)
                    {
                        if (GameSupport.GetEquipWeaponDepotTableID(_weapondata.TableID))
                        {
                            if (_armoryWeaponUID != (int)eCOUNT.NONE)
                            {
                                WeaponData equipWeapon = GameInfo.Instance.GetWeaponData(_armoryWeaponUID);
                                if (equipWeapon != null)
                                {
                                    if (equipWeapon.TableID == _weapondata.TableID)
                                    {
                                        GameInfo.Instance.Send_ReqApplySlotInWpnDepot(_armoryWeaponUID, _weapondata.WeaponUID, OnNet_AckApplySlotInWpnDepot);
                                        return;
                                    }
                                }
                            }

                            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3157));
                            return;
                        }
                    }
                    

                    GameInfo.Instance.Send_ReqApplySlotInWpnDepot(_armoryWeaponUID, _weapondata.WeaponUID, OnNet_AckApplySlotInWpnDepot);
                }
                break;

            case eWeaponSeleteState.TRADE_MATERIAL:
                {
                    //무기 재료 적용                    
                    GameInfo.Instance.SetTradeMaterialUID(FacilitySupport.TempTradeMaterialUIDs);
                    LobbyUIManager.Instance.Renewal("FacilityPanel");
                    OnClickClose();
                }
                break;
        }

        
    }

    public void OnClick_AllClear()
    {
        if (_weaponSeleteState != eWeaponSeleteState.TRADE_MATERIAL)
            return;

        for (int i = 0; i < FacilitySupport.TempTradeMaterialUIDs.Length; i++)
        {
            FacilitySupport.TempTradeMaterialUIDs[i] = 0;
        }

        Renewal(true);
    }

    public void OnNetFacilityItemEquip(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        GameInfo.Instance.Send_ReqFacilityOperation(_facilitydata.TableID, -1, 1, null, OnNetFacilityUse);
    }

    public void OnNet_AckApplySlotInWpnDepot(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("ArmoryPopup");

        OnClick_BackBtn();
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

        LobbyUIManager.Instance.Renewal("FacilityItemPanel");
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");

        OnClick_BackBtn();
    }

    public void OnNetFacilityItemRemove(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
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
