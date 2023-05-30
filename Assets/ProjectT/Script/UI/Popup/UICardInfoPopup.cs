using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICardInfoPopup : FComponent
{
    public enum eCardInfoType
    {
        Info,
        Get,
        Formation,
        _MAX_,
    }

    public GameObject kInfo;
    public GameObject kAcquisition;
    public GameObject kFormation;
    public GameObject kFormationGoBtnObj;
    public GameObject kSelBtn;
    public UIButton kAcquisitionTabBtn;
    public UIButton kInfoTabBtn;
    public UIButton kCardFormationBtn;
    public UIAcquisitionInfoUnit kAcquisitionInfoUnit;
    public UIButton kBackBtn;
	public FToggle kLockToggle;
    public UIButton kEquipBtn;
	public UITexture kEquipImageTex;
	public UITexture kCardTex;
	public UISprite kGradeSpr;
    public UILabel kNameLabel;
    public UILabel kEnchantLabel;
    public UILabel kLevelLabel;
    public UILabel kLuckLabel;
    public UIGaugeUnit kExpGaugeUnit;
    public UISprite kWakeSpr;
    public UIStatusUnit kSupporterStatusUnit_HP;
    public UIStatusUnit kSupporterStatusUnit_DEF;
    public UISprite kTypeSpr;
    public UILabel kTypeLabel;
    public UILabel kSkillNameLabel;
	public UILabel kSkillLevelLabel;
	public UILabel kSkillDesceLabel;
    public GameObject kMainSkill;
    public UILabel kMainSkillNameLabel;
    public UILabel kMainSkillLevelLabel;
    public UILabel kMainSkillTimeLabel;
    public UILabel kMainSkillDesceLabel;
    public GameObject kFavor;
    public UILabel kFavorLevelLabel;
    public UISprite kFavorExpSpr;
    public UIButton kLevelUpBtn;
    public UIButton kGradeUpBtn;
    public UIButton kSkillLevelUpBtn;
    public UISprite kGradeUpNoticeSpr;
    public UISprite kSkillLevelUpNoticeSpr;
    public UISprite kEnchantNoticeSpr;
    public UIButton kArrow_LBtn;
    public UIButton kArrow_RBtn;

    public UIButton BtnChangeAttr;
    public UIGrid   Grid;

    public UIButton kEnchantBtn;
    public UIButton kDecompositionBtn;

    public UISprite kCardTypeChangeSpr;
    public GameObject kCardInfoTypeTabRoot;
    public FTab kCardInfoTypeTab;
    public FList kCardFormationListInstance;
    public GameObject kGoToFormationBtn;
    public OnVoidCallBack OnCloseCallBack;

    private CardData _carddata;
    private CardBookData _cardbookdata;
    private GameTable.Card.Param _cardtabledata;
    private int _itemlistindex = -1;
    private bool _bLarge = false;

    private eCardInfoType _cardInfoType = eCardInfoType.Info;
    private List<GameTable.CardFormation.Param> _cardFormationList = new List<GameTable.CardFormation.Param>();

    public override void Awake()
	{
		base.Awake();

        kLockToggle.EventCallBack = OnLockToggleSelect;
        kCardInfoTypeTab.EventCallBack = OnCardInfoTypeTabSelect;
        kCardFormationListInstance.EventUpdate = _UpdateCardFormationListSlot;
        kCardFormationListInstance.EventGetItemCount = _GetCardFormationElementCount;
        kCardFormationListInstance.InitBottomFixing();
    }

    public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}

    public override void InitComponent()
	{
        _itemlistindex = -1;
        long carduid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CardUID);
        if (carduid != -1)
        {
            _carddata = GameInfo.Instance.GetCardData(carduid);
            _cardbookdata = GameInfo.Instance.GetCardBookData(_carddata.TableID);
            _cardtabledata = _carddata.TableData;

            UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
            if (itempanel != null)
            {
                for( int i = 0; i < itempanel.CardList.Count; i++ )
                {
                    if (itempanel.CardList[i] != null)
                    {
                        if (_carddata.CardUID == itempanel.CardList[i].CardUID)
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
            _carddata = null;
            _cardbookdata = null;
            _cardtabledata = GameInfo.Instance.GameTable.FindCard((int)UIValue.Instance.GetValue(UIValue.EParamType.CardTableID));
        }

        _bLarge = false;

        //211221
        UIValue.Instance.SetValue(UIValue.EParamType.CardFormationType, eCharSelectFlag.USER_INFO);
        _cardFormationList.Clear();
        _cardFormationList = GameInfo.Instance.GameTable.FindAllCardFormation(x => x.CardID1 == _cardtabledata.ID || x.CardID2 == _cardtabledata.ID || x.CardID3 == _cardtabledata.ID);

        kCardFormationListInstance.UpdateList();

        _cardInfoType = eCardInfoType.Info;
        kCardInfoTypeTab.SetTab((int)_cardInfoType, SelectEvent.Code);
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        if (_carddata == null && _cardtabledata == null)
            return;


        kCardTypeChangeSpr.SetActive(false);
        int level = 1;
        int seleteimage = 0;
        int luck = 0;
        int skilllevel = 1;
        int enchantLv = 0;
        int wake = 0;
        float fillAmount = 0.0f;

        if (_carddata != null )
        {
            level = _carddata.Level;
            skilllevel = _carddata.SkillLv;
            wake = _carddata.Wake;
            enchantLv = _carddata.EnchantLv;
            fillAmount = GameSupport.GetCardLevelExpGauge(_carddata, _carddata.Level, _carddata.Exp);
            seleteimage = GameSupport.GetCardImageNum(_carddata);
        }

        kCardTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("card", string.Format("Card/{0}_{1}.png", _cardtabledata.Icon, seleteimage) );

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_cardtabledata.Name);
        kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, level, GameSupport.GetMaxLevelCard(_cardtabledata.Grade, wake));
        kLuckLabel.textlocalize = luck.ToString();

        // Enchant
        kEnchantNoticeSpr.SetActive(false);
        kEnchantBtn.SetActive(true);
        kEnchantBtn.isEnabled = true;
        if (_carddata != null && _carddata.EnchantLv >= 0)
        {   
            kEnchantLabel.SetActive(true);            
            kEnchantLabel.textlocalize = string.Format("+{0}", _carddata.EnchantLv);
            kEnchantLabel.transform.localPosition = new Vector3(kNameLabel.transform.localPosition.x + kNameLabel.printedSize.x + 10, kNameLabel.transform.localPosition.y, 0);
            kEnchantBtn.isEnabled = !GameSupport.IsMaxEnchantLevel(_carddata.TableData.EnchantGroup, _carddata.EnchantLv);

            kEnchantNoticeSpr.SetActive(GameSupport.IsPossibleEnchant(_carddata.TableData.EnchantGroup, _carddata.EnchantLv));
        }
        else
        {   
            kEnchantLabel.SetActive(false);
        }

        kGradeSpr.spriteName = "itemgrade_L_" + _cardtabledata.Grade.ToString();
        //kGradeSpr.MakePixelPerfect();

        kWakeSpr.spriteName = "itemwake_0" + wake.ToString();
        kWakeSpr.MakePixelPerfect();

        kExpGaugeUnit.InitGaugeUnit(fillAmount);
        kExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (fillAmount * 100.0f)));

        kSupporterStatusUnit_HP.InitStatusUnit((int)eTEXTID.STAT_HP, GameSupport.GetCardHP(level, wake, skilllevel, enchantLv, _cardtabledata));
        kSupporterStatusUnit_DEF.InitStatusUnit((int)eTEXTID.STAT_DEF, GameSupport.GetCardDEF(level, wake, skilllevel, enchantLv, _cardtabledata));

        int cardType = 0;
        if(_carddata != null)
        {
            cardType = _carddata.Type;
        }
        else
        {
            cardType = _cardtabledata.Type;
        }

        kTypeSpr.spriteName = "SupporterType_" + cardType.ToString();
        kTypeLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.CARDTYPE + cardType);

        kSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(_cardtabledata.SkillEffectName);
        kSkillDesceLabel.textlocalize = GameSupport.GetCardSubSkillDesc(_cardtabledata, skilllevel);

        kSkillLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.SKILL_LEVEL_TXT_NOW_LV, skilllevel, GameSupport.GetMaxSkillLevelCard());
        kSkillLevelLabel.transform.localPosition = new Vector3(kSkillNameLabel.transform.localPosition.x + kSkillNameLabel.printedSize.x + 10, kSkillLevelLabel.transform.localPosition.y, 0);

        if (_cardtabledata.MainSkillEffectName > 0)
        {
            kMainSkill.SetActive(true);
            kMainSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(_cardtabledata.MainSkillEffectName);
            kMainSkillDesceLabel.textlocalize = GameSupport.GetCardMainSkillDesc(_cardtabledata, wake);
            if (wake == 0)
                kMainSkillLevelLabel.textlocalize = "";
            else
                kMainSkillLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_NOW_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PLUS_TEXT), wake));
            kMainSkillLevelLabel.transform.localPosition = new Vector3(kMainSkillNameLabel.transform.localPosition.x + kMainSkillNameLabel.printedSize.x + 10, kMainSkillNameLabel.transform.localPosition.y, 0);

            if (_cardtabledata.CoolTime == 0)
            {
                kMainSkillTimeLabel.gameObject.SetActive(false);
            }
            else
            {
                kMainSkillTimeLabel.gameObject.SetActive(true);
                kMainSkillTimeLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(263), _cardtabledata.CoolTime);
            }
        }
        else
            kMainSkill.SetActive(false);

        kAcquisitionInfoUnit.UpdateSlot(_cardtabledata.AcquisitionID);

        bool acquisitionDimd = false;
        kAcquisitionTabBtn.gameObject.SetActive(true);
        if (_cardtabledata.AcquisitionID > 0)
            kAcquisitionTabBtn.isEnabled = true;
        else
        {
            kAcquisitionTabBtn.isEnabled = false;
            acquisitionDimd = true;
        }

        bool cardFormationDimd = false;
        kCardFormationBtn.gameObject.SetActive(true);

        if (_cardFormationList.Count <= (int)eCOUNT.NONE)
        {
            kCardFormationBtn.isEnabled = false;
            cardFormationDimd = true;
        }
        else
        {
            if (LobbyUIManager.Instance.IsActiveUI("ItemPanel"))
                kGoToFormationBtn.SetActive(true);
            else
                kGoToFormationBtn.SetActive(false);

            kCardFormationBtn.isEnabled = true;
        }

        kCardInfoTypeTabRoot.SetActive(cardFormationDimd && acquisitionDimd ? false : true);

        kGradeUpNoticeSpr.gameObject.SetActive(false);
        kSkillLevelUpNoticeSpr.gameObject.SetActive(false);
        if (_carddata != null)
        {
            kSelBtn.SetActive(true);
            kLockToggle.gameObject.SetActive(true);
            if (_carddata.Lock)
                kLockToggle.SetToggle(1, SelectEvent.Code);
            else
                kLockToggle.SetToggle(0, SelectEvent.Code);

            kEquipBtn.gameObject.SetActive(false);
            CharData chardata = GameInfo.Instance.GetEquiCardCharData(_carddata.CardUID);
            if (chardata != null)
            {
                kEquipBtn.gameObject.SetActive(true);
                kEquipImageTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_Chara_" + chardata.TableData.Icon + ".png");
            }
            FacilityData facilityCardData = GameInfo.Instance.GetEquiCardFacilityData(_carddata.CardUID);
            if (facilityCardData != null)
            {
                kEquipBtn.gameObject.SetActive(true);
                kEquipImageTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_" + GameSupport.GetFacilityIconName(facilityCardData));
            }
            if (GameInfo.Instance.GetUsingDispatchData(_carddata.CardUID) != null)
            {
                kEquipBtn.gameObject.SetActive(true);                
                kEquipImageTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_Facility_SptDispatch.png");
            }
            if (GameSupport.IsMaxLevelCard(_carddata))
            {
                kLevelUpBtn.gameObject.SetActive(false);
                kGradeUpBtn.gameObject.SetActive(true);
                if (GameSupport.IsMaxWakeCard(_carddata))
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
            if (GameSupport.IsMaxSkillLevelCard(_carddata))
                kSkillLevelUpBtn.isEnabled = false;
            else
                kSkillLevelUpBtn.isEnabled = true;

            kFavorExpSpr.fillAmount = GameSupport.GetCardFavorLevelExpGauge(_cardbookdata.TableID, _cardbookdata.FavorLevel, _cardbookdata.FavorExp);
            kFavorLevelLabel.textlocalize = _cardbookdata.FavorLevel.ToString();
            kFavor.SetActive(true);

            if(GameSupport.IsCardWakeUp(_carddata))
                kGradeUpNoticeSpr.gameObject.SetActive(true);
            if (GameSupport.IsCardSkillUp(_carddata, true))
                kSkillLevelUpNoticeSpr.gameObject.SetActive(true);

            /*
            if (GameSupport.IsMaxWakeCard(_carddata.TableData.Grade, _carddata.Wake))
                kPotentialLockSpr.gameObject.SetActive(false);
            else
                kPotentialLockSpr.gameObject.SetActive(true);
            */

            kArrow_LBtn.gameObject.SetActive(false);
            kArrow_RBtn.gameObject.SetActive(false);
            UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
            if (itempanel != null)
            {
                if (GameInfo.Instance.CardList.Count > 1)
                {
                    kArrow_LBtn.gameObject.SetActive(true);
                    kArrow_RBtn.gameObject.SetActive(true);
                }
            }

            BtnChangeAttr.gameObject.SetActive(_carddata.TableData.Changeable == 1);

            kDecompositionBtn.SetActive(true);
        }
        else
        {
            kArrow_LBtn.gameObject.SetActive(false);
            kArrow_RBtn.gameObject.SetActive(false);
            kSelBtn.SetActive(false);
            kLockToggle.gameObject.SetActive(false);
            kEquipBtn.gameObject.SetActive(false);
            kLevelUpBtn.gameObject.SetActive(false);
            kGradeUpBtn.gameObject.SetActive(false);
            kSkillLevelUpBtn.gameObject.SetActive(false);
            BtnChangeAttr.gameObject.SetActive(false);

            kFavor.SetActive(false);
            if (_cardtabledata.Changeable > (int)eCOUNT.NONE)
                kCardTypeChangeSpr.SetActive(true);
            kEnchantBtn.SetActive(false);
            kDecompositionBtn.SetActive(false);
        }

        Grid.Reposition();
    }
 

	
	private bool OnLockToggleSelect(int nSelect, SelectEvent type)
	{
        if (type == SelectEvent.Click)
        {
            List<long> uidlist = new List<long>();
            List<bool> locklist = new List<bool>();

            uidlist.Add(_carddata.CardUID);
            if (nSelect == 0)
                locklist.Add(false);
            else
                locklist.Add(true);

            GameInfo.Instance.Send_ReqSetLockCardList(uidlist, locklist, OnNetCardLock);
        }
        return true;
	}

    public void OnClick_BackBtn()
	{
        OnClickClose();
	}

    public override void OnClickClose()
    {
        if (OnCloseCallBack != null)
        {
            OnCloseCallBack();
            OnCloseCallBack = null;
        }

#if DISABLESTEAMWORKS
        LobbyUIManager.Instance.Renewal("CharInfoPanel");
        base.OnClickClose();        
#else
        if (UIAni.isPlaying) return;

        LobbyUIManager.Instance.Renewal("CharInfoPanel");

        if (!_bLarge)
            base.OnClickClose();
        else
            StartCoroutine(CoClickClose());
#endif
    }

    private IEnumerator CoClickClose()
    {
        float fTime = PlayAnimtion(2);
        _bLarge = false;

        yield return new WaitForSeconds(fTime);

        base.OnClickClose();
    }

    public void OnClick_SellBtn()
    {
        if (IsPossibleRemove())
            return;

        UIValue.Instance.SetValue(UIValue.EParamType.SellSingleType, UISellSinglePopup.eSELLTYPE.CARD);
        UIValue.Instance.SetValue(UIValue.EParamType.SellSingleUID, _carddata.CardUID);
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
        
        _decompoitemlist.Add(_carddata.CardUID);
        ActionAddGroupID(_carddata.TableData.Decomposition);

        UIDecompositionPopup popup = LobbyUIManager.Instance.GetUI<UIDecompositionPopup>("DecompositionPopup");
        if (popup != null)
        {
            popup.SetData(_decompoitemlist, _decompoRewards, UIItemPanel.eTabType.TabType_Card);
            LobbyUIManager.Instance.ShowUI("DecompositionPopup", true);
        }
    }

    public void OnClick_CardBtn()
    {
        if(_carddata != null)
            CardViewer.ShowCardPopup(_cardtabledata.ID, GameSupport.IsMaxWakeCard(_carddata) && GameSupport.IsMaxLevelCard(_carddata));
        else
            CardViewer.ShowCardPopup(_cardtabledata.ID, false);
    }

    public void OnClick_EquipBtn()
	{
        CharData chardata = GameInfo.Instance.GetEquiCardCharData(_carddata.CardUID);
        if (chardata != null)
        {
            base.OnClickClose();
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, chardata.CUID);
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, chardata.TableData.ID);
            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.SUPPORTER);

            LobbyUIManager.Instance.SetPanelType( ePANELTYPE.CHARINFO, false, true );
            return;
        }
        FacilityData facilityCardData = GameInfo.Instance.GetEquiCardFacilityData(_carddata.CardUID);
        if (facilityCardData != null)
        {
            base.OnClickClose();
            
            //뒷 Lobby BG 제거 후 이동
            LobbyUIManager.Instance.PanelBGAllHide();

            LobbyUIManager.Instance.HideUI("ItemPanel");
            UIMainPanel mainpanel = LobbyUIManager.Instance.GetUI<UIMainPanel>("MainPanel");
            if (mainpanel != null)
            {
                mainpanel.OnClick_FacilityBtn(facilityCardData.TableData.ParentsID - 1);
                return;
            }
        }
        if (GameInfo.Instance.GetUsingDispatchData(_carddata.CardUID) != null)
        {
            base.OnClickClose();

            LobbyUIManager.Instance.HideUI("ItemPanel");
            UIMainPanel mainpanel = LobbyUIManager.Instance.GetUI<UIMainPanel>("MainPanel");
            if (mainpanel != null)
            {
                mainpanel.OnClick_CardDispatch();
                return;
            }
        }
    }

    public void OnClick_LevelUpBtn()
	{
        if (GameSupport.IsMaxLevelCard(_carddata))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3004));
            return;
        }

        if (GameInfo.Instance.GetEquiCardFacilityData(_carddata.CardUID) != null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3145));
            return;
        }

        if (GameInfo.Instance.GetUsingDispatchData(_carddata.CardUID) != null)
        {
            // 파견중인 서포터 입니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3218));
            return;
        }
        LobbyUIManager.Instance.ShowUI("CardLevelUpPopup", true);
    }

    public void OnClick_GradeUpBtn()
    {
        if (GameSupport.IsMaxWakeCard(_carddata))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3007));
            return;
        }

        if (GameInfo.Instance.GetEquiCardFacilityData(_carddata.CardUID) != null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3145));
            return;
        }

        if (GameInfo.Instance.GetUsingDispatchData(_carddata.CardUID) != null)
        {
            // 파견중인 서포터 입니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3218));
            return;
        }

        LobbyUIManager.Instance.ShowUI("CardGradeUpPopup", true);
    }

    public void OnClick_SkillLevelUpBtn()
    {
        if (GameSupport.IsMaxSkillLevelCard(_carddata))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3007));
            return;
        }

        bool blevelup = false;
        var list = GameInfo.Instance.CardList.FindAll(x => x.TableID == _carddata.TableID && x.CardUID != _carddata.CardUID && x.Lock == false);
        for (int i = 0; i < list.Count; i++)
        {
            CharData matchardata = GameInfo.Instance.GetEquiCardCharData(list[i].CardUID);
            if (matchardata != null)
                continue;
            if (GameSupport.IsEquipAndUsingCardData(list[i].CardUID))
                continue;
            blevelup = true;
            break;
        }
      
        var itemlist = GameInfo.Instance.ItemList.FindAll(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_CARD_SLVUP && x.TableData.Grade == _carddata.TableData.Grade);
        if (itemlist.Count != 0)
        {
            blevelup = true;
        }
         
        if (!blevelup)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3125));
            return;
        }

        if (GameInfo.Instance.GetEquiCardFacilityData(_carddata.CardUID) != null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3145));
            return;
        }

        if (GameInfo.Instance.GetUsingDispatchData(_carddata.CardUID) != null)
        {
            // 파견중인 서포터 입니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3218));
            return;
        }

        LobbyUIManager.Instance.ShowUI("CardSkillLevelUpPopup", true);
    }
    /*
    public void OnClick_PotentialBtn()
    {
        LobbyUIManager.Instance.ShowUI("CardPotentialPopup",true);
    }
    */
    public void OnClick_Arrow_LBtn()
    {
        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel == null)
            return;

        int temp = _itemlistindex;
        temp -= 1;
        if (temp < 0)
            temp = GameInfo.Instance.CardList.Count - 1;

        if (0 <= temp && itempanel.CardList.Count > temp)
        {
            var data = itempanel.CardList[temp];
            if (data != null)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.CardUID, data.CardUID);
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
        if (temp >= GameInfo.Instance.CardList.Count)
            temp = 0;

        if (0 <= temp && itempanel.CardList.Count > temp)
        {
            var data = itempanel.CardList[temp];
            if (data != null)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.CardUID, data.CardUID);
                InitComponent();
                Renewal(true);
            }
        }
    }

    public void OnClick_EnchantPopup()
    {       
        FacilityData facilityCardData = GameInfo.Instance.GetEquiCardFacilityData(_carddata.CardUID);
        if (facilityCardData != null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(110806));
            return;
        }

        if (GameInfo.Instance.GetUsingDispatchData(_carddata.CardUID) != null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3240));
            return;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.EnchantUID, _carddata.CardUID);
        LobbyUIManager.Instance.ShowUI("CardEnchantPopup", true);
    }

    public void OnBtnChangeAttr()
    {
        string strCard = FLocalizeString.Instance.GetText(1071);    //  서포터
        //  잠금 상태, 장착 상태, 시설 이용 상태의 경우
        if (_carddata.Lock == true)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3057, strCard)); //  잠금 중인 {0}입니다.
            return;
        }

        if (GameInfo.Instance.GetEquiCardCharData(_carddata.CardUID) != null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3026, strCard)); //  배치 중인 {0}입니다.
            return;
        }

        if (GameSupport.IsEquipAndUsingCardData(_carddata.CardUID))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3026, strCard)); //  배치 중인 {0}입니다.
            return;
        }

        UICardTypeChangePopup popup = LobbyUIManager.Instance.GetUI<UICardTypeChangePopup>("CardTypeChangePopup");
        popup.SetCardData(_carddata);

        LobbyUIManager.Instance.ShowUI("CardTypeChangePopup", true);
    }

    public void OnNetCardLock(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3050));

        Renewal(true);
        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
            itempanel.RefreshList();
    }

    public void OnNetCardSeleteImage(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        Renewal(true);
        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
            itempanel.RefreshList();
    }

    private bool IsPossibleRemove()
    {
        string strCard = FLocalizeString.Instance.GetText(1071);    //  서포터
        //  잠금 상태, 장착 상태, 시설 이용 상태의 경우
        if (_carddata.Lock == true)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3057, strCard)); //  잠금 중인 {0}입니다.
            return true;
        }

        if (GameInfo.Instance.GetEquiCardCharData(_carddata.CardUID) != null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3026, strCard)); //  배치 중인 {0}입니다.
            return true;
        }

        if (GameSupport.IsEquipAndUsingCardData(_carddata.CardUID))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3026, strCard)); //  배치 중인 {0}입니다.
            return true;
        }
        return false;
    }

    private bool OnCardInfoTypeTabSelect(int nSelect, SelectEvent type)
    {
        _cardInfoType = (eCardInfoType)nSelect;

        kInfo.SetActive(_cardInfoType == eCardInfoType.Info);
        kAcquisition.SetActive(_cardInfoType == eCardInfoType.Get);
        kFormation.SetActive(_cardInfoType == eCardInfoType.Formation);
        kFormationGoBtnObj.SetActive(_cardInfoType == eCardInfoType.Formation);

        kNameLabel.SetActive(_cardInfoType != eCardInfoType.Formation);
        kEnchantLabel.SetActive(_carddata != null && _carddata.EnchantLv >= 0 && _cardInfoType != eCardInfoType.Formation);

        return true;
    }

    private void _UpdateCardFormationListSlot(int index, GameObject slotObject)
    {
        do
        {
            UICardTeamSlot slot = slotObject.GetComponent<UICardTeamSlot>();
            if (null == slot) break;

            GameTable.CardFormation.Param data = null;
            if (0 <= index && _cardFormationList.Count > index)
                data = _cardFormationList[index];

            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(index, data);
        } while (false);
    }

    private int _GetCardFormationElementCount()
    {
        return _cardFormationList.Count;
    }

    public void OnClick_CardFormation()
    {
        LobbyUIManager.Instance.ShowUI("ArmoryPopup", true);

        if (LobbyUIManager.Instance.IsActiveUI("BookCardInfoPopup"))
            LobbyUIManager.Instance.HideUI("BookCardInfoPopup", false);

        OnClickClose();
    }
}
