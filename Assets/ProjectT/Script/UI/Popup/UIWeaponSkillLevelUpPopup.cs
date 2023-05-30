using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWeaponSkillLevelUpPopup : FComponent
{
    public UISprite kGradeSpr;
    public UILabel kNameLabel;
    public UITexture kWeaponTex;
    public UILabel kSkillNameLabel;
    public UILabel kSkillLevelLabel;
    public UILabel kSkillDescLabel;
    public UISprite kSPIconSpr;
    public UILabel kSPLabel;
    public UIButton kLevelUpBtn;
    public UIGoodsUnit kGoldGoodsUnit;
    public UIItemListSlot kItemListSlot;
    public UIItemListSlot kMatItemListSlot;
    public UISprite kMatEmptySpr;
    public UIStatusUnit kWeaponStatusUnit_ATK;
    public UIStatusUnit kWeaponStatusUnit_CRI;
    [SerializeField] private FList _ItemListInstance;

    private List<MatSkillData> _matlist = new List<MatSkillData>();
    private MatSkillData _selmatdata = null;
    private WeaponData _weapondata;
    private bool _bsendskilllvup = false;
    public MatSkillData SelMatData { get { return _selmatdata; } }

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
        _bsendskilllvup = false;
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.WeaponUID);
        _weapondata = GameInfo.Instance.GetWeaponData(uid);

        _selmatdata = null;

        _matlist.Clear();

        for (int i = 0; i < GameInfo.Instance.WeaponList.Count; i++)
        {
            if (GameInfo.Instance.WeaponList[i].Lock)
                continue;
            if (_weapondata.WeaponUID == GameInfo.Instance.WeaponList[i].WeaponUID)
                continue;
            CharData chardata = GameInfo.Instance.GetEquipWeaponCharData(GameInfo.Instance.WeaponList[i].WeaponUID);
            if (chardata != null)
                continue;
            if (GameInfo.Instance.GetEquipWeaponFacilityData(GameInfo.Instance.WeaponList[i].WeaponUID) != null)
                continue;
            if (GameSupport.GetEquipWeaponDepot(GameInfo.Instance.WeaponList[i].WeaponUID))
                continue;

            if (_weapondata.TableID == GameInfo.Instance.WeaponList[i].TableID)
                _matlist.Add(new MatSkillData(GameInfo.Instance.WeaponList[i]));
        }
        
        var list = GameInfo.Instance.ItemList.FindAll(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_WEAPON_SLVUP && x.TableData.Grade == _weapondata.TableData.Grade);
        for (int i = 0; i < list.Count; i++)
            _matlist.Add(new MatSkillData(list[i]));

        _ItemListInstance.UpdateList();

        Invoke("InvokeShowWeapon", 0.1f);
    }

    private void InvokeShowWeapon()
    {
        RenderTargetWeapon.Instance.gameObject.SetActive(true);

        RenderTargetWeapon.Instance.InitRenderTargetWeapon(_weapondata.TableID, _weapondata.WeaponUID, true);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_weapondata.TableData.Name);
        kGradeSpr.spriteName = "itemgrade_L_" + _weapondata.TableData.Grade.ToString();
        //kGradeSpr.MakePixelPerfect();

        int skilllevel = _weapondata.SkillLv;
        if (_selmatdata != null)
        {
            if (_selmatdata.Type == MatSkillData.eTYPE.ITEM)
                skilllevel += _selmatdata.ItemData.TableData.Value;
            else if (_selmatdata.Type == MatSkillData.eTYPE.WEAPON)
                skilllevel += _selmatdata.WeaponData.SkillLv;
            if (GameSupport.GetMaxSkillLevelWeapon() < skilllevel)
                skilllevel = GameSupport.GetMaxSkillLevelWeapon();
        }

        float addPercent = (GameInfo.Instance.GameConfig.WeaponSkillLvStatRate[skilllevel] - GameInfo.Instance.GameConfig.WeaponSkillLvStatRate[_weapondata.SkillLv]) * 100.0f;

        kWeaponStatusUnit_ATK.InitStatusUnit((int)eTEXTID.STAT_ATK, GameSupport.GetWeaponATK(_weapondata.Level, _weapondata.Wake, _weapondata.SkillLv, _weapondata.EnchantLv, _weapondata.TableData), GameSupport.GetWeaponATK(_weapondata.Level, _weapondata.Wake, skilllevel, _weapondata.EnchantLv, _weapondata.TableData), addPercent);
        kWeaponStatusUnit_CRI.InitStatusUnit((int)eTEXTID.STAT_CRI, GameSupport.GetWeaponCRI(_weapondata.Level, _weapondata.Wake, _weapondata.SkillLv, _weapondata.EnchantLv, _weapondata.TableData), GameSupport.GetWeaponCRI(_weapondata.Level, _weapondata.Wake, skilllevel, _weapondata.EnchantLv, _weapondata.TableData), addPercent);

        kSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(_weapondata.TableData.SkillEffectName);
        kSkillDescLabel.textlocalize = GameSupport.GetWeaponSkillDesc(_weapondata.TableData, skilllevel);
        kSkillLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.SKILL_LEVEL_TXT_NOW_LV, skilllevel, GameSupport.GetMaxSkillLevelWeapon());
        kSkillLevelLabel.transform.localPosition = new Vector3(kSkillNameLabel.transform.localPosition.x + kSkillNameLabel.printedSize.x + 10, kSkillLevelLabel.transform.localPosition.y, 0);

        if (_weapondata.TableData.UseSP <= 0)
        {
            kSPIconSpr.gameObject.SetActive(false);
            kSPLabel.gameObject.SetActive(false);
        }
        else
        {
            kSPIconSpr.gameObject.SetActive(true);
            kSPLabel.gameObject.SetActive(true);
            kSPLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), (_weapondata.TableData.UseSP / 100));
        }

        kItemListSlot.ParentGO = this.gameObject;
        kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, 0, _weapondata);

        int gold = 0;
        if (_selmatdata != null)
        {
            kMatEmptySpr.gameObject.SetActive(false);
            kMatItemListSlot.gameObject.SetActive(true);
            kMatItemListSlot.ParentGO = this.gameObject;

            if (_selmatdata.Type == MatSkillData.eTYPE.ITEM)
                kMatItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, 0, _selmatdata.ItemData);
            else if (_selmatdata.Type == MatSkillData.eTYPE.WEAPON)
                kMatItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, 0, _selmatdata.WeaponData);
            
            gold = GameSupport.GetWeaponSkillLevelUpCost(_weapondata, 1);

            kLevelUpBtn.isEnabled = true;
        }
        else
        {
            kMatEmptySpr.gameObject.SetActive(true);
            kMatItemListSlot.gameObject.SetActive(false);

            kLevelUpBtn.isEnabled = false;
        }

        _ItemListInstance.RefreshNotMove();

        kGoldGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, gold, true);
    }



    private void _UpdateItemListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIItemListSlot card = slotObject.GetComponent<UIItemListSlot>();
            if (null == card) break;
            card.ParentGO = this.gameObject;

            MatSkillData data = null;
            if (0 <= index && _matlist.Count > index)
                data = _matlist[index];

            card.ParentGO = this.gameObject;
            if (data.Type == MatSkillData.eTYPE.ITEM)
                card.UpdateSlot(UIItemListSlot.ePosType.Weapon_SkillLevelUpMatItemList, index, data.ItemData);
            else if (data.Type == MatSkillData.eTYPE.WEAPON)
                card.UpdateSlot(UIItemListSlot.ePosType.Weapon_SkillLevelUpMatList, index, data.WeaponData);

        } while (false);
    }

    private int _GetItemElementCount()
    {
        return _matlist.Count;
    }
    public void OnClick_ItemSlot()
    {
        _selmatdata = null;
        Renewal(true);
    }

    public void OnClick_BackBtn()
    {
        OnClickClose();
    }

    public void OnClick_CancleBtn()
    {
        OnClickClose();
    }

    public void OnClick_LevelUpBtn()
    {
        if (_selmatdata == null)
            return;
        if (_bsendskilllvup)
            return;

        int gold = GameSupport.GetWeaponSkillLevelUpCost(_weapondata, 1);

        if (!GameSupport.IsCheckGoods(eGOODSTYPE.GOLD, gold))
        {
            return;
        }

        if (GameSupport.IsMaxSkillLevelWeapon(_weapondata.SkillLv))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3059));
            return;
        }
        _bsendskilllvup = true;
        if (_selmatdata.Type == MatSkillData.eTYPE.ITEM)
            GameInfo.Instance.Send_ReqSkillLvUpWeapon(_weapondata.WeaponUID, (int)_selmatdata.Type, _selmatdata.ItemData.ItemUID, OnNetSkillLvUpWeapon);
        else if (_selmatdata.Type == MatSkillData.eTYPE.WEAPON)
            GameInfo.Instance.Send_ReqSkillLvUpWeapon(_weapondata.WeaponUID, (int)_selmatdata.Type, _selmatdata.WeaponData.WeaponUID, OnNetSkillLvUpWeapon);
    }

    public void SeletMatItem(ItemData itemdata)
    {
        if (GameSupport.IsMaxSkillLevelWeapon(_weapondata.SkillLv))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3059));
            return;
        }
        _selmatdata = new MatSkillData(itemdata);
        Renewal(true);
    }

    public void SeletMatWeapon(WeaponData weapondata)
    {
        if (GameSupport.IsMaxSkillLevelWeapon(_weapondata.SkillLv))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3059));
            return;
        }
        _selmatdata = new MatSkillData(weapondata);
        Renewal(true);
    }

    public void OnNetSkillLvUpWeapon(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        StartCoroutine(SkillLvUpWeaponResultCoroutine());
    }

    IEnumerator SkillLvUpWeaponResultCoroutine()
    {
        LobbyUIManager.Instance.ShowUI("ResultWeaponSkillLevelUpPopup", true);

        yield return new WaitForSeconds(2.0f);

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("WeaponInfoPopup");

        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
        {   
            itempanel.InitComponent();
            itempanel.Renewal(true);
        }

        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.CHARINFO)
        {
            //LobbyUIManager.Instance.Renewal("CharInfoPanel");
            LobbyUIManager.Instance.InitComponent("CharWeaponSeletePopup");
            LobbyUIManager.Instance.Renewal("CharWeaponSeletePopup");
        }


        InitComponent();
        Renewal(true);
        _bsendskilllvup = false;
    }
}
