using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWeaponInfoPopup : FComponent
{
    public enum eWeaponInfoType
    {
        Info,
        Get,
        _MAX_,
    }

    public GameObject kInfo;
    public GameObject kAcquisition;
    public GameObject kSelBtn;
    public UIButton kAcquisitionTabBtn;
    public UIButton kInfoTabBtn;
    public UIAcquisitionInfoUnit kAcquisitionInfoUnit;
    public UITexture kWeaponTex;
    public UIButton kBackBtn;
    public FToggle kLockToggle;
    public UIButton kEquipBtn;
    public UITexture kEquipImageTex;
    public UISprite kGradeSpr;
    public UILabel kNameLabel;
    public UILabel kEnchantLabel;
    public UILabel kLevelLabel;
    public UIGaugeUnit kExpGaugeUnit;
    public UISprite kWakeSpr;
    public UILabel kSkillNameLabel;
    public UILabel kSkillLevelLabel;
    public UILabel kSkillDescLabel;
    public UISprite kSPIconSpr;
    public UILabel kSPLabel;
    public UIStatusUnit kWeaponStatusUnit_ATK;
    public UIStatusUnit kWeaponStatusUnit_CRI;
    public GameObject kSocket;
    public List<UISprite> kGemSlotLockList;
    public List<UITexture> kGemTexList;
    public List<UIButton> kGemChangeBtnList;
    public UIButton kLevelUpBtn;
    public UIButton kGradeUpBtn;
    public UIButton kSkillLevelUpBtn;
    public UISprite kGradeUpNoticeSpr;    
    public UISprite kSkillLevelUpNoticeSpr;
    public UISprite kEnchantNoticeSpr;
    public UIButton kArrow_LBtn;
    public UIButton kArrow_RBtn;

    public UILabel kCharLabel;

    public UIGrid Grid;

    public UIButton kEnchantBtn;
    public UIButton kDecompositionBtn;

    public GameObject kWeaponInfoTabRoot;
    public FTab kWeaponInfoTypeTab;

    public UITexture _TexAvailableChar;

    public OnVoidCallBack OnCloseCallBack;

    [Header("Add Gem UR Grade")]
    [SerializeField] List<UISprite> gemSetOptList;

    private int _itemlistindex = -1;
    private WeaponData _weapondata;
    private GameTable.Weapon.Param _weapontabledata;

    private eWeaponInfoType _weaponInfoType = eWeaponInfoType.Info;

    private bool mWakeMaxFromBook = false;

    public override void Awake()
    {
        base.Awake();

        kLockToggle.EventCallBack = OnLockToggleSelect;
        kWeaponInfoTypeTab.EventCallBack = OnWeaponInfoTypeTabSelect;
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        _itemlistindex = -1;
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.WeaponUID);
        if (uid != -1)
        {
            _weapondata = GameInfo.Instance.GetWeaponData(uid);
            _weapontabledata = _weapondata.TableData;

            UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
            if (itempanel != null)
            {
                for (int i = 0; i < itempanel.WeaponList.Count; i++)
                {
                    if (itempanel.WeaponList[i] != null)
                    {
                        if (_weapondata.WeaponUID == itempanel.WeaponList[i].WeaponUID)
                        {
                            _itemlistindex = i;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            _weapondata = null;
            _weapontabledata = GameInfo.Instance.GameTable.FindWeapon((int)UIValue.Instance.GetValue(UIValue.EParamType.WeaponTableID));
        }

        _weaponInfoType = eWeaponInfoType.Info;
        kWeaponInfoTypeTab.SetTab((int)_weaponInfoType, SelectEvent.Code);

        foreach (UISprite uiSprite in gemSetOptList)
        {
            uiSprite.SetActive(false);
        }

        //  UI팝업 애니메이션 과는 상관업이 랜더 타겟이 바뀌므로
        //  바뀌는 부분에 딜레이를 줍니다.
        Invoke("InvokeShowWeapon", 0.1f);
    }

    public override void OnClickClose()
    {
        if (OnCloseCallBack != null)
        {
            OnCloseCallBack();
            OnCloseCallBack = null;
        }
        base.OnClickClose();
    }

    private void InvokeShowWeapon()
    {
        RenderTargetWeapon.Instance.gameObject.SetActive(true);
        if (_weapondata != null)
            RenderTargetWeapon.Instance.InitRenderTargetWeapon(_weapondata.TableID, _weapondata.WeaponUID, true);
        else {
			RenderTargetWeapon.Instance.InitRenderTargetWeapon( _weapontabledata.ID, -1, true );
            RenderTargetWeapon.Instance.ShowWeaponEffect( mWakeMaxFromBook );
		}
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        if (_weapondata == null && _weapontabledata == null)
            return;

        int level = 1;
        int wake = 0;
        int skillLv = 1;
        int enchantLv = 0;
        float fillAmount = 0.0f;

        if (_weapondata != null)
        {
            level = _weapondata.Level;
            wake = _weapondata.Wake;
            skillLv = _weapondata.SkillLv;
            enchantLv = _weapondata.EnchantLv;
            fillAmount = GameSupport.GetWeaponLevelExpGauge(_weapondata, _weapondata.Level, _weapondata.Exp);
        }

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_weapontabledata.Name);
        kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, level, GameSupport.GetWeaponMaxLevel(_weapontabledata.Grade, wake));

        kGradeSpr.spriteName = "itemgrade_L_" + _weapontabledata.Grade.ToString();
        //kGradeSpr.MakePixelPerfect();

        // Enchant
        kEnchantNoticeSpr.SetActive(false);
        kEnchantBtn.SetActive(true);
        kEnchantBtn.isEnabled = true;
        if (_weapondata != null && _weapondata.EnchantLv >= 0)
        {   
            kEnchantLabel.SetActive(true);
            kEnchantLabel.textlocalize = string.Format("+{0}", _weapondata.EnchantLv);
            kEnchantLabel.transform.localPosition = new Vector3(kNameLabel.transform.localPosition.x + kNameLabel.printedSize.x + 10, kNameLabel.transform.localPosition.y, 0);
            kEnchantBtn.isEnabled = !GameSupport.IsMaxEnchantLevel(_weapondata.TableData.EnchantGroup, _weapondata.EnchantLv);

            kEnchantNoticeSpr.SetActive(GameSupport.IsPossibleEnchant(_weapondata.TableData.EnchantGroup, _weapondata.EnchantLv));
        }
        else
        {  
            kEnchantLabel.SetActive(false);
        }

        GameTable.Character.Param charTableData = null;
        string label = null;
        string[] split = Utility.Split( _weapontabledata.CharacterID, ',' );

        for( int i = 0; i < split.Length; i++ ) {
            int charTableId = Utility.SafeIntParse( split[i] );

            charTableData = GameInfo.Instance.GameTable.FindCharacter( x => x.ID == charTableId );
            if( charTableData == null ) {
                continue;
			}

            /*
            if( string.IsNullOrEmpty( label ) ) {
                label = "[00FFFF]";
			}
            */

            label += FLocalizeString.Instance.GetText( charTableData.Name );

            if( i < split.Length - 1 ) {
                label += ", ";
            }
        }

        if( !string.IsNullOrEmpty( label ) ) {
            label = string.Format( FLocalizeString.Instance.GetText( 1514 ), label );
            
            kCharLabel.gameObject.SetActive( true );
            kCharLabel.textlocalize = label;
        }
        else {
            kCharLabel.gameObject.SetActive( false );
        }
        /*
        GameTable.Character.Param charTableData = GameInfo.Instance.GameTable.FindCharacter(x => x.ID == _weapontabledata.CharacterID);
        if (charTableData != null)
        {
            kCharLabel.gameObject.SetActive(true);
            kCharLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1514), FLocalizeString.Instance.GetText(charTableData.Name));
        }
        else
            kCharLabel.gameObject.SetActive(false);
        */
        kWakeSpr.gameObject.SetActive(true);
        kWakeSpr.spriteName = "itemwake_0" + wake.ToString();
        kWakeSpr.MakePixelPerfect();

        kExpGaugeUnit.InitGaugeUnit(fillAmount);
        kExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), ( fillAmount * 100.0f )));

        if (_weapontabledata.SkillEffectName > 0)
        {
            kSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(_weapontabledata.SkillEffectName);
            kSkillDescLabel.textlocalize = GameSupport.GetWeaponSkillDesc(_weapontabledata, skillLv);
        }
        else
        {
            kSkillNameLabel.textlocalize = "";
            kSkillDescLabel.textlocalize = "";
        }

        kSkillNameLabel.alpha = 1.0f;
        kSkillDescLabel.alpha = 1.0f;

        kSkillLevelLabel.textlocalize = "";
        
        kWeaponStatusUnit_ATK.InitStatusUnit((int)eTEXTID.STAT_ATK, GameSupport.GetWeaponATK(level, wake, skillLv, enchantLv, _weapontabledata));
        kWeaponStatusUnit_CRI.InitStatusUnit((int)eTEXTID.STAT_CRI, GameSupport.GetWeaponCRI(level, wake, skillLv, enchantLv, _weapontabledata));

        kAcquisitionInfoUnit.UpdateSlot(_weapontabledata.AcquisitionID);
        kAcquisitionTabBtn.gameObject.SetActive(true);

        if (_weapontabledata.AcquisitionID > 0)
            kWeaponInfoTabRoot.SetActive(true);
        else
            kWeaponInfoTabRoot.SetActive(false);
            

        if (_weapontabledata.UseSP <= 0)
        {
            kSPIconSpr.gameObject.SetActive(false);
            kSPLabel.gameObject.SetActive(false);
        }
        else
        {
            kSPIconSpr.gameObject.SetActive(true);
            kSPLabel.gameObject.SetActive(true);
            kSPLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), (_weapontabledata.UseSP / 100));
        }

        kGradeUpNoticeSpr.gameObject.SetActive(false);
        kSkillLevelUpNoticeSpr.gameObject.SetActive(false);

        if (_weapondata != null)
        {
            kSocket.SetActive(true);
            kSelBtn.SetActive(true);
            if (_weapontabledata.SkillEffectName > 0)
            {
                kSkillLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.SKILL_LEVEL_TXT_NOW_LV, skillLv, GameSupport.GetMaxSkillLevelWeapon());
                kSkillLevelLabel.transform.localPosition = new Vector3(kSkillNameLabel.transform.localPosition.x + kSkillNameLabel.printedSize.x + 10, kSkillLevelLabel.transform.localPosition.y, 0);
            }
            else
            {
                kSkillLevelLabel.textlocalize = "";
            }

            for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
            {
                kGemTexList[i].gameObject.SetActive(false);
                kGemChangeBtnList[i].gameObject.SetActive(false);
            }

            int slotmaxcount = GameSupport.GetWeaponGradeSlotCount(_weapondata.TableData.Grade, 2);
            int slotcount = GameSupport.GetWeaponGradeSlotCount(_weapondata.TableData.Grade, _weapondata.Wake);

            for (int i = 0; i < slotmaxcount; i++)
            {
                if (i >= slotcount)
                    kGemSlotLockList[i].gameObject.SetActive(true);
                else
                    kGemSlotLockList[i].gameObject.SetActive(false);
                kGemChangeBtnList[i].gameObject.SetActive(true);
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
            }

            kLockToggle.gameObject.SetActive(true);
            if (_weapondata.Lock)
                kLockToggle.SetToggle(1, SelectEvent.Code);
            else
                kLockToggle.SetToggle(0, SelectEvent.Code);

            kEquipBtn.gameObject.SetActive(false);
            CharData chardata = GameInfo.Instance.GetEquipWeaponCharData(_weapondata.WeaponUID);
            if (chardata != null)
            {
                kEquipBtn.gameObject.SetActive(true);
                kEquipImageTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_Chara_" + chardata.TableData.Icon + ".png");
            }
            FacilityData facilityWeaponData = GameInfo.Instance.GetEquipWeaponFacilityData(_weapondata.WeaponUID);
            if (facilityWeaponData != null)
            {
                kEquipBtn.gameObject.SetActive(true);
                kEquipImageTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_" + GameSupport.GetFacilityIconName(facilityWeaponData));
            }
            if (GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList.Contains(_weapondata.WeaponUID))
            {
                kEquipBtn.SetActive(true);
                kEquipImageTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_Wpn_Depot.png");
            }

            if (GameSupport.IsMaxLevelWeapon(_weapondata))
            {
                kLevelUpBtn.gameObject.SetActive(false);
                kGradeUpBtn.gameObject.SetActive(true);
                if (GameSupport.IsMaxWakeWeapon(_weapondata))
                    kGradeUpBtn.isEnabled = false;
                else
                    kGradeUpBtn.isEnabled = true;
            }
            else
            {
                kLevelUpBtn.gameObject.SetActive(true);
                kGradeUpBtn.gameObject.SetActive(false);
            }

            kSkillLevelUpBtn.gameObject.SetActive(true);
            if (GameSupport.IsMaxSkillLevelWeapon(skillLv) || _weapontabledata.SkillEffectName <= 0)
                kSkillLevelUpBtn.isEnabled = false;
            else
                kSkillLevelUpBtn.isEnabled = true;

            if (GameSupport.IsWeaponWakeUp(_weapondata))
                kGradeUpNoticeSpr.gameObject.SetActive(true);
            if (GameSupport.IsWeaponSkillUp(_weapondata, true))
                kSkillLevelUpNoticeSpr.gameObject.SetActive(true);

            kArrow_LBtn.gameObject.SetActive(false);
            kArrow_RBtn.gameObject.SetActive(false);
            UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
            if (itempanel != null)
            {
                if (GameInfo.Instance.WeaponList.Count > 1)
                {
                    kArrow_LBtn.gameObject.SetActive(true);
                    kArrow_RBtn.gameObject.SetActive(true);
                }
            }

            kDecompositionBtn.SetActive(_weapondata.TableData.Decomposable == 1);
        }
        else
        {
            kArrow_LBtn.gameObject.SetActive(false);
            kArrow_RBtn.gameObject.SetActive(false);
            kSelBtn.SetActive(false);
            kWakeSpr.gameObject.SetActive(false);
            kSocket.SetActive(false);
            kLockToggle.gameObject.SetActive(false);
            kEquipBtn.gameObject.SetActive(false);
            kLevelUpBtn.gameObject.SetActive(false);
            kGradeUpBtn.gameObject.SetActive(false);
            kSkillLevelUpBtn.gameObject.SetActive(false);
            kEnchantBtn.SetActive(false);
            kDecompositionBtn.SetActive(false);
        }

        _TexAvailableChar.mainTexture = null;

        if( _weapontabledata.WpnBOActivate == 0 ) {
            _TexAvailableChar.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", "Icon/Set/Set_Chara_All.png" );
        }
        else {
            GameTable.Character.Param charParam = GameInfo.Instance.GetCharacterData( _weapontabledata.WpnBOActivate );
            if( charParam != null ) {
                _TexAvailableChar.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", "Icon/Set/Set_Chara_" + charParam.Icon + ".png" );
            }
        }

        Grid.Reposition();
    }

	public override void OnDisable() {
		base.OnDisable();

		mWakeMaxFromBook = false;
	}

    private bool OnLockToggleSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Click)
        {
            List<long> uidlist = new List<long>();
            List<bool> locklist = new List<bool>();

            uidlist.Add(_weapondata.WeaponUID);
            if (nSelect == 0)
                locklist.Add(false);
            else
                locklist.Add(true);

            GameInfo.Instance.Send_ReqSetLockWeaponList(uidlist, locklist, OnNetWeaponLock);
        }
        return true;
    }


    public void OnClick_BackBtn()
    {
        OnClickClose();
    }

    public override void OnClose()
    {
        base.OnClose();
        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.CHARINFO)
        {
            LobbyUIManager.Instance.Renewal("CharInfoPanel");
        }
    }

    public void OnClick_SellBtn()
    {
        if (IsPossibleRemove())
            return;

        UIValue.Instance.SetValue(UIValue.EParamType.SellSingleType, UISellSinglePopup.eSELLTYPE.WEAPON);
        UIValue.Instance.SetValue(UIValue.EParamType.SellSingleUID, _weapondata.WeaponUID);
        LobbyUIManager.Instance.ShowUI("SellSinglePopup", true);
    }

    public void OnClick_Decomposition()
    {
        if (IsPossibleRemove())
            return;

        List<long> _decompoitemlist = new List<long>();
        List<RewardData> _decompoRewards = new List<RewardData>();

        System.Action<int> ActionAddGroupID = (id) =>
        {
            var Randoms = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == id);
            if (Randoms == null || Randoms.Count == 0)
                return;

            for (int i = 0; i < Randoms.Count; i++)
            {
                var p1 = Randoms[i];
                var p2 = _decompoRewards.Find(x => x.Type == p1.ProductType &&
                        x.Index == p1.ProductIndex);

                if (p2 != null)
                {
                    p2.Value += p1.ProductValue;                    
                }
                else
                    _decompoRewards.Add(new RewardData(p1.ProductType, p1.ProductIndex, p1.ProductValue));
            }
        };

        _decompoitemlist.Add(_weapondata.WeaponUID);
        ActionAddGroupID(_weapondata.TableData.Decomposition);

        UIDecompositionPopup popup = LobbyUIManager.Instance.GetUI<UIDecompositionPopup>("DecompositionPopup");
        if (popup != null)
        {
            popup.SetData(_decompoitemlist, _decompoRewards, UIItemPanel.eTabType.TabType_Weapon);
            LobbyUIManager.Instance.ShowUI("DecompositionPopup", true);
        }
    }

    public void OnClick_EquipBtn()
    {
        CharData chardata = GameInfo.Instance.GetEquipWeaponCharData(_weapondata.WeaponUID);
        if (chardata != null)
        {
            OnClickClose();
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, chardata.CUID);
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, chardata.TableData.ID);
            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.WEAPON);
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);
            return;
        }
        FacilityData facilityWeaponData = GameInfo.Instance.GetEquipWeaponFacilityData(_weapondata.WeaponUID);
        if (facilityWeaponData != null)
        {
            OnClickClose();

            //뒷 Lobby BG 제거 후 이동
            LobbyUIManager.Instance.PanelBGAllHide();

            LobbyUIManager.Instance.HideUI("ItemPanel");
            UIMainPanel mainpanel = LobbyUIManager.Instance.GetUI<UIMainPanel>("MainPanel");
            if (mainpanel != null)
                mainpanel.OnClick_FacilityBtn(facilityWeaponData.TableData.ParentsID - 1);
        }

        if (GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList.Contains(_weapondata.WeaponUID))
        {
            OnClickClose();

            //뒷 Lobby BG 제거 후 이동
            LobbyUIManager.Instance.PanelBGAllHide();

            LobbyUIManager.Instance.HideUI("ItemPanel");
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN);
            UIValue.Instance.SetValue(UIValue.EParamType.UserInfoPopup, 0);
            LobbyUIManager.Instance.ShowUI("UserInfoPopup", true);
            LobbyUIManager.Instance.ShowUI("ArmoryPopup", true);
        }
    }

    public void OnClick_GemChangeBtn(int index)
    {
        int slotcount = GameSupport.GetWeaponGradeSlotCount(_weapondata.TableData.Grade, _weapondata.Wake);
        if (index >= slotcount)
        {
            int i = index - slotcount;
            if (i == 0)
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3114));
            else if (i == 1)
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3115));
            return;
        }

        if(GameInfo.Instance.GemList.Count <= 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1597));
            return;
        }

        if (GameSupport.IsWeaponEquipGemCount(_weapondata.WeaponUID) <= (int)eCOUNT.NONE)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1597));
            return;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, _weapondata.WeaponUID);
        UIValue.Instance.SetValue(UIValue.EParamType.WeaponGemIndex, index);
        LobbyUIManager.Instance.ShowUI("WeaponGemSeletePopup", true);
    }

    public void OnClick_LevelUpBtn()
    {
        if (GameSupport.IsMaxLevelWeapon(_weapondata, _weapondata.Level))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3007));
            return;
        }

        if (GameInfo.Instance.GetEquipWeaponFacilityData(_weapondata.WeaponUID) != null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3145));
            return;
        }

        LobbyUIManager.Instance.ShowUI("WeaponLevelUpPopup", true);
    }

    public void OnClick_GradeUpBtn()
    {
        if (GameSupport.IsMaxWakeWeapon(_weapondata))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3007));
            return;
        }

        if (GameInfo.Instance.GetEquipWeaponFacilityData(_weapondata.WeaponUID) != null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3145));
            return;
        }

        LobbyUIManager.Instance.ShowUI("WeaponGradeUpPopup", true);
    }

    public void OnClick_SkillLevelUpBtn()
    {
        if (GameSupport.IsMaxSkillLevelWeapon(_weapondata.SkillLv))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3007));
            return;
        }

        var list = GameInfo.Instance.WeaponList.FindAll(x => x.TableID == _weapondata.TableID && x.WeaponUID != _weapondata.WeaponUID && x.Lock == false);
        bool blevelup = false;

        for (int i = 0; i < list.Count; i++)
        {
            CharData matchardata = GameInfo.Instance.GetEquipWeaponCharData(list[i].WeaponUID);
            if (matchardata != null)
                continue;
            if (GameInfo.Instance.GetEquipWeaponFacilityData(list[i].WeaponUID) != null)
                continue;

            if (GameSupport.GetEquipWeaponDepot(list[i].WeaponUID))
                continue;

            blevelup = true;
            break;
        }

        var itemlist = GameInfo.Instance.ItemList.FindAll(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_WEAPON_SLVUP && x.TableData.Grade == _weapondata.TableData.Grade);
        if (itemlist.Count != 0)
        {
            blevelup = true;
        }

        if (!blevelup)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3126));
            return;
        }

        if (GameInfo.Instance.GetEquipWeaponFacilityData(_weapondata.WeaponUID) != null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3145));
            return;
        }

        LobbyUIManager.Instance.ShowUI("WeaponSkillLevelUpPopup", true);
    }


    public void OnClick_Arrow_LBtn()
    {
        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel == null)
            return;

        int temp = _itemlistindex;
        temp -= 1;
        if (temp < 0)
            temp = GameInfo.Instance.WeaponList.Count - 1;

        if (0 <= temp && itempanel.WeaponList.Count > temp)
        {
            var data = itempanel.WeaponList[temp];
            if (data != null)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, data.WeaponUID);
                InitComponent();
                Renewal(true);
            }
        }
    }
    public void OnClick_Arrow_RBtn()
    {
        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel == null)
            return;

        int temp = _itemlistindex;
        temp += 1;
        if (temp >= GameInfo.Instance.WeaponList.Count)
            temp = 0;

        if (0 <= temp && itempanel.WeaponList.Count > temp)
        {
            var data = itempanel.WeaponList[temp];
            if (data != null)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, data.WeaponUID);
                InitComponent();
                Renewal(true);
            }
        }
    }

    public void OnClick_EnchantPopup()
    {   
        FacilityData facilityWeaponData = GameInfo.Instance.GetEquipWeaponFacilityData(_weapondata.WeaponUID);
        if (facilityWeaponData != null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(110311));
            return;
        }
        if (GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList.Contains(_weapondata.WeaponUID))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(111114));
            return;
        }


        UIValue.Instance.SetValue(UIValue.EParamType.EnchantUID, _weapondata.WeaponUID);
        LobbyUIManager.Instance.ShowUI("WeaponEnchantPopup", true);
    }

    public void OnNetWeaponLock(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3050));

        Renewal(true);
        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
            itempanel.RefreshList();
    }

    public override bool IsBackButton()
    {
        return !Director.IsPlaying;
    }

    public void SetWakeMaxFromBook( bool isWakeMax ) {
		mWakeMaxFromBook = isWakeMax;
	}

    private bool IsPossibleRemove()
    {
        if (_weapondata == null)
            return true;

        string strWeapon = FLocalizeString.Instance.GetText(1069);
        //  잠금 상태
        if (_weapondata.Lock == true)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3057, strWeapon)); //  잠금 중인 {0}입니다.
            return true;
        }

        //  장착 상태
        if (GameInfo.Instance.GetEquipWeaponCharData(_weapondata.WeaponUID) != null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3025, strWeapon)); //  장착 중인 {0}입니다.
            return true;
        }

        //  시설 이용 상태
        if (GameInfo.Instance.GetEquipWeaponFacilityData(_weapondata.WeaponUID) != null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3026, strWeapon)); //  배치 중인 {0}입니다.
            return true;
        }

        if (GameSupport.GetEquipWeaponDepot(_weapondata.WeaponUID))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3231)); //  무기고에 배치 중인 무기입니다.
            return true;
        }

        return false;
    }

    private bool OnWeaponInfoTypeTabSelect(int nSelect, SelectEvent type)
    {
        _weaponInfoType = (eWeaponInfoType)nSelect;

        kInfo.SetActive(_weaponInfoType == eWeaponInfoType.Info);
        kAcquisition.SetActive(_weaponInfoType == eWeaponInfoType.Get);

        return true;
    }
}
