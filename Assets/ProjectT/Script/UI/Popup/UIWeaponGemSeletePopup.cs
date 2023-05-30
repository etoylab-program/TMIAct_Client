using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWeaponGemSeletePopup : FComponent
{
	public UIButton kBackBtn;
	public UIStatusUnit kCharStatusUnit_HP;
	public UIStatusUnit kCharStatusUnit_ATK;
	public UIStatusUnit kCharStatusUnit_DEF;
	public UIStatusUnit kCharStatusUnit_CRI;
    public GameObject kInfo;
    public UILabel kNameLabel;
	public UILabel kLevelLabel;
	public UIStatusUnit kWeaponStatusUnit_00;
	public UIStatusUnit kWeaponStatusUnit_01;
    public List<UIGemOptUnit> kGemOptList;
    public List<GameObject> kEmptyList;

    public List<UISprite> kEmptySlotColorList;
    public List<UISprite> kLockList;
    public List<UITexture> kGemTexList;
    public List<UIButton> kGemSeleteBtnList;

    public UISprite kSelEquipSpr;
    public UIButton kChangeBtn;
    [SerializeField] private FList _ItemListInstance;

    [Header("Add Gem UR Grade")]
    [SerializeField] private GameObject gemTooltipObj = null;
    [SerializeField] private UILabel gemTooltipLabel = null;
    [SerializeField] private List<UISprite> gemSetOptList = null;
    [SerializeField] private FToggle gemInfoToggle = null;

    private WeaponData _weapondata;
    private List<GemData> _gemlist = new List<GemData>();
    private GemData _gemdata;
    private long[] _slotgemuid = new long[(int)eCOUNT.WEAPONGEMSLOT];
    private int[] _statusAry = new int[(int)eCHARABILITY._MAX_];
    private int _equipslot;
    private string _setOptStr;

    public long SeleteGemUID
    {
        get
        {
            if (_gemdata == null)
                return -1;
            return _gemdata.GemUID;
        }
    }

    public override void Awake()
	{
		base.Awake();

		if(this._ItemListInstance == null) return;

        
        this._ItemListInstance.EventUpdate = this._UpdateItemListSlot;
		this._ItemListInstance.EventGetItemCount = this._GetItemElementCount;
        this._ItemListInstance.InitBottomFixing();

        gemInfoToggle.EventCallBack = OnEventGemTabSelect;
    }

    public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
    }

    public override void InitComponent()
	{
        gemTooltipObj.SetActive(false);

        long weaponuid = (long)UIValue.Instance.GetValue(UIValue.EParamType.WeaponUID);
        _weapondata = GameInfo.Instance.GetWeaponData(weaponuid);

        _gemlist.Clear();
        for (int i = 0; i < GameInfo.Instance.GemList.Count; i++)
        {
            WeaponData tempWeaponData = GameInfo.Instance.GetEquipGemWeaponData(GameInfo.Instance.GemList[i].GemUID);
            if (tempWeaponData != null)
            {
                if (weaponuid == tempWeaponData.WeaponUID)
                    _gemlist.Add(GameInfo.Instance.GemList[i]);
            }
            else
                _gemlist.Add(GameInfo.Instance.GemList[i]);
        }
        _gemlist.Sort(GemData.CompareFuncGradeLevel);

        _gemdata = null;
        
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
            _slotgemuid[i] = _weapondata.SlotGemUID[i];

        _equipslot = -1;

        for (int i = 0; i < (int)eCHARABILITY._MAX_; i++)
            _statusAry[i] = 0;

        GameSupport.GetGemTotalStat(_slotgemuid, ref _statusAry);

        _ItemListInstance.UpdateList();

        int slot = (int)UIValue.Instance.GetValue(UIValue.EParamType.WeaponGemIndex);
        if (slot != -1)
            SetEquipSlot(slot);

        foreach (UISprite uiSprite in gemSetOptList)
        {
            uiSprite.SetActive(false);
        }
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        int[] nowStatusAry = new int[(int)eCHARABILITY._MAX_];

        GameSupport.GetGemTotalStat(_weapondata.SlotGemUID, ref nowStatusAry);

        kCharStatusUnit_HP.InitStatusUnit((int)eTEXTID.STAT_HP, _statusAry[(int)eCHARABILITY.HP], nowStatusAry[(int)eCHARABILITY.HP]);
        kCharStatusUnit_ATK.InitStatusUnit((int)eTEXTID.STAT_ATK, _statusAry[(int)eCHARABILITY.ATK], nowStatusAry[(int)eCHARABILITY.ATK]);
        kCharStatusUnit_DEF.InitStatusUnit((int)eTEXTID.STAT_DEF, _statusAry[(int)eCHARABILITY.DEF], nowStatusAry[(int)eCHARABILITY.DEF]);
        kCharStatusUnit_CRI.InitStatusUnit((int)eTEXTID.STAT_CRI, _statusAry[(int)eCHARABILITY.CRI], nowStatusAry[(int)eCHARABILITY.CRI]);

        if(_gemdata == null )
        {
            kInfo.SetActive(false);
        }
        else
        {
            kInfo.SetActive(true);
            kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_gemdata.TableData.Name);

            kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _gemdata.Level, GameSupport.GetGemMaxLevel());

            int statusmain = _gemdata.GetTypeStatus(_gemdata.TableData.MainType);
            int statussub = _gemdata.GetTypeStatus(_gemdata.TableData.SubType);
            kWeaponStatusUnit_00.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + _gemdata.TableData.MainType, statusmain);

            /*
            if (_gemdata.TableData.MainType == _gemdata.TableData.SubType)
            {
                kWeaponStatusUnit_01.gameObject.SetActive(false);
            }
            else
            {
                kWeaponStatusUnit_01.gameObject.SetActive(true);
                kWeaponStatusUnit_01.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + _gemdata.TableData.SubType, statussub);
            }
            */
            for (int i = 0; i < kGemOptList.Count; i++)
            {
                kGemOptList[i].gameObject.SetActive(true);
                kGemOptList[i].Lock();
            }

            for (int i = 0; i < _gemdata.Wake; i++)
                kGemOptList[i].Opt(_gemdata, i);
        }

        //kWeaponItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Mat, 0, _weapondata);
        //kWeaponItemListSlot.SetActiveItemStateUnit(false);

        if (_equipslot == -1)
            kSelEquipSpr.gameObject.SetActive(false);
        else
            kSelEquipSpr.gameObject.SetActive(true);


        int slotmaxcount = GameSupport.GetWeaponGradeSlotCount(_weapondata.TableData.Grade, 2);
        int slotcount = GameSupport.GetWeaponGradeSlotCount(_weapondata.TableData.Grade, _weapondata.Wake);


        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {
            if (i >= slotcount)
                kLockList[i].gameObject.SetActive(true);
            else
                kLockList[i].gameObject.SetActive(false);

            kGemTexList[i].gameObject.SetActive(false);
            kGemSeleteBtnList[i].gameObject.SetActive(false);
            kEmptyList[i].transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
        }

        _setOptStr = string.Empty;
        if (_weapondata != null)
        {
            _setOptStr = LobbyUIManager.Instance.GetGemSetOpt(_weapondata.SlotGemUID);
        }
        gemInfoToggle.SetToggle(string.IsNullOrEmpty(_setOptStr) ? 1 : 0, SelectEvent.Code);

        for (int i = 0; i < slotcount; i++)
        {
            kGemSeleteBtnList[i].gameObject.SetActive(true);
            GemData gemdata = GameInfo.Instance.GetGemData(_weapondata.SlotGemUID[i]);
            if (gemdata != null)
            {
                kGemTexList[i].gameObject.SetActive(true);
                kGemTexList[i].mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + gemdata.TableData.Icon);
                GameTable.GemSetType.Param gemSetTypeParam = GameInfo.Instance.GameTable.FindGemSetType(gemdata.SetOptID);
                gemSetOptList[i].SetActive(gemSetTypeParam != null);
                if (gemSetOptList[i].gameObject.activeSelf)
                {
                    gemSetOptList[i].spriteName = gemSetTypeParam.Icon;
                }
            }
            else
            {
                gemSetOptList[i].SetActive(false);
            }

            if (_equipslot == i)
            {
                kEmptyList[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                kSelEquipSpr.transform.localPosition = kEmptyList[i].transform.localPosition;
            }
            else
            {
                kEmptyList[i].transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
            }
        }

        _ItemListInstance.RefreshNotMove();
    }
 	
	private void _UpdateItemListSlot(int index, GameObject slotObject)
	{
        do
        {
            UIItemListSlot card = slotObject.GetComponent<UIItemListSlot>();
            if (null == card) break;

            GemData data = null;
            if (0 <= index && _gemlist.Count > index)
            {
                data = _gemlist[index];
            }

            card.ParentGO = this.gameObject;
            card.UpdateSlot(UIItemListSlot.ePosType.Gem_SeleteList, index, data);
        } while (false);
    }
	
	private int _GetItemElementCount()
	{
        if (_gemlist == null || _gemlist.Count <= 0)
            return 0;

		return _gemlist.Count;
	}

    private bool OnEventGemTabSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
        {
            return false;
        }

        if (type == SelectEvent.Click)
        {
            if (nSelect == 1)
            {
                gemTooltipObj.SetActive(!gemTooltipObj.activeSelf);
                gemTooltipLabel.textlocalize = _setOptStr;
            }

            return false;
        }

        return true;
    }

    public void SetEquipSlot(int slot)
    {
        _equipslot = slot;

        var gemdata = GameInfo.Instance.GetGemData(_weapondata.SlotGemUID[_equipslot]);
        if (gemdata != null)
        {
            _gemdata = gemdata;
            int gemIdx = -1;
            for (int i = 0; i < _gemlist.Count; i++)
            {
                if (gemdata.GemUID == _gemlist[i].GemUID)
                {
                    gemIdx = i;
                    break;
                }
            }
            bool bIsScroll = _ItemListInstance.IsScroll;
            if (bIsScroll)
            {
                Log.Show("Scroll Focus");

                _ItemListInstance.SpringSetFocus(gemIdx);
            }
        }

        Renewal(true);
    }
    
    public void SetSeleteGemUID(long uid)
    {
        Debug.Log("SetSeleteGemUID" + uid.ToString() );
        var gemdata = GameInfo.Instance.GetGemData(uid);
        if (gemdata == null)
            return;

        if (_equipslot != -1)
        {
            if (_weapondata.SlotGemUID[_equipslot] == uid)
            {
                _weapondata.SlotGemUID[_equipslot] = (int)eCOUNT.NONE;
                _gemdata = null;
            }
            else
            {
                for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
                {
                    if (_weapondata.SlotGemUID[i] == uid)
                    {
                        _weapondata.SlotGemUID[i] = (int)eCOUNT.NONE;
                        break;
                    }
                }
                _weapondata.SlotGemUID[_equipslot] = uid;
                _gemdata = gemdata;
            }
        }

        Renewal(true);
    }

    public void OnClick_SeleteBtn(int index)
    {
        if (_equipslot == index)
        {
            SetSeleteGemUID(_weapondata.SlotGemUID[index]);
        }
        else
        {
            SetEquipSlot(index);
        }
    }

    public void OnClick_BackBtn()
	{
        OnClickClose();
	}

    public override void OnClickClose()
    {
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
            _weapondata.SlotGemUID[i] = _slotgemuid[i];

        base.OnClickClose();
    }

    public void OnClick_ChangeBtn()
	{
        bool bsame = true;


        long[] gemlist = new long[(int)eCOUNT.WEAPONGEMSLOT];
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
            gemlist[i] = -1;

        int[] slotlist = new int[(int)eCOUNT.WEAPONGEMSLOT];
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
            slotlist[i] = -1;

        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {
            if (_slotgemuid[i] != _weapondata.SlotGemUID[i])
            {
                bsame = false;
            }
            gemlist[i] = _weapondata.SlotGemUID[i];
            slotlist[i] = i;
        }

        if (bsame)
        {
            OnClick_BackBtn();
            return;
        }

        GameInfo.Instance.Send_ReqApplyGemInWeapon(_weapondata.WeaponUID, gemlist, slotlist, OnNetGemWeaponEquip);
    }

    public void OnClick_GemAutoEpuipBtn()
    {
        
        if (_weapondata == null)
            return;

        List<long> gemlist = new List<long>();
        List<int> slotlist = new List<int>();

        int count = GameSupport.GetGemAutoEpuipList(_weapondata, ref gemlist, ref slotlist);
        if (count == 0)
            return;
        
        for (int i = 0; i < gemlist.Count; i++)
        {
            _weapondata.SlotGemUID[slotlist[i]] = gemlist[i];
        }

        Renewal(true);
    }

    public void OnClick_GemRemoveBtn()
    {
        if (_weapondata == null)
            return;

        if (GameSupport.IsWeaponGemSlotAllEmpty(_weapondata) == true)
            return;

        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
            _weapondata.SlotGemUID[i] = (int)eCOUNT.NONE;

        _gemdata = null;

        Renewal(true);
    }

    public void OnClick_BgCloseBtn()
    {
        gemTooltipObj.SetActive(false);
    }

    public void OnNetGemWeaponEquip(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.InitComponent("WeaponInfoPopup");
        LobbyUIManager.Instance.Renewal("WeaponInfoPopup");
        LobbyUIManager.Instance.Renewal("CharInfoPanel");
        LobbyUIManager.Instance.Renewal("CharWeaponSeletePopup");
        LobbyUIManager.Instance.Renewal("PresetPopup");

        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
            itempanel.RefreshList();

        base.OnClickClose();
    }
}
