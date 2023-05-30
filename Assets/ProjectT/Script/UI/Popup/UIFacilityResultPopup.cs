using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Holoville.HOTween;

public class UIFacilityResultPopup : FComponent
{
    private enum eRestartType
    {
        NONE = 0,
        ReStart,
        GradeUp,
        AwakenGradeUp,
    }

	public UILabel kTitleLabel;

    public GameObject kChar;
    public GameObject kItem;
    public GameObject kWeapon;
    public GameObject kCharSkillPoint;
    public GameObject kSupporter;

    public UITexture kCharIconTex;
	public UILabel kCharNameLabel;
	public UILabel kCharLevelLabel;
	public UILabel kCharAddExpLabel;
	public UIGaugeUnit kCharEXPGaugeUnit;
    public GameObject kCharLevelUp;

    public UIItemListSlot kItemListSlot;
    public UILabel kItemNameLabel;

    public UITexture kSkillPointCharIconTex;
    public UILabel kSkillPointCharNameLabel;
    public UILabel kSkillPointLabel;

    public UITexture kWeaponIconTex;
    public UILabel kWeaponNameLabel;
    public UILabel kWeaponLevelLabel;
    public UILabel kWeaponAddExpLabel;
    public UIGaugeUnit kWeaponEXPGaugeUnit;
    public GameObject kWeaponLevelUp;

    public GameObject kCloseBtn;

    private FacilityData _facilitydata;
    private CharData _chardata;
    private WeaponData _weapondata;
    private ItemData _itemdata;
    private CardBookData _cardbookdata;
    private CardData _carddata;

    public int kCharGageAllGainExp = -1;
    private int _curcharlevel = -1;
    private int _curcharexp = -1;
    private int _gaincharexp = -1;
    private Coroutine m_crCharExp;


    public int kWeaponGageAllGainExp = -1;
    private int _curweaponlevel = -1;
    private int _curweaponexp = -1;
    private int _gainweaponexp = -1;
    private Coroutine m_crWeaponExp;


    private int _nowhp;
    private int _maxhp;

    public bool bContinue { get; set; }

    //Supporter
    public GameObject kSupporterOn;
    public GameObject kSupporterOff;
    public GameObject kSupporterLevelUp;
    public UITexture kSupporterTexIcon;
    public UILabel kSupporterFavorLv;
    public UIGaugeUnit kSupporterFavorGauge;

    public int kSupporterAllGainExp = -1;
    private int _curSupporterlevel = -1;
    private int _curSupporterexp = -1;
    private int _gainSupporterexp = -1;
    private Coroutine m_crSupporterFavorExp;


    [Header("Facility Trade")]
    public GameObject kGUID;

    [Header("ReStart")]
    public GameObject kReStartBtn;
    public UILabel kReStartLabel;
    public GameObject kOkBtn;
    private eRestartType _restartType = eRestartType.NONE;

 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();

        StartCoroutine(ResultCoroutine());
    }
 
	public override void InitComponent()
	{
        FacilityResultData resultdata = GameInfo.Instance.FacilityResultData;
        _facilitydata = GameInfo.Instance.GetFacilityData(resultdata.FacilityID);
        _chardata = null;
        _weapondata = null;
        _itemdata = null;
        kCharGageAllGainExp = -1;
        kWeaponGageAllGainExp = -1;
        bContinue = true;
        _cardbookdata = null;
        kSupporterAllGainExp = -1;

        _restartType = eRestartType.NONE;

        var carddata = GameInfo.Instance.GetCardData(_facilitydata.EquipCardUID);
        if (carddata != null)
        {
            VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.FacilityComplete, carddata.TableID);
        }
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        if (_facilitydata == null)
            return;

        FacilityResultData resultdata = GameInfo.Instance.FacilityResultData;
        UserData userdata = GameInfo.Instance.UserData;

        kCloseBtn.SetActive(false);

        kChar.SetActive(false);
        kItem.SetActive(false);
        kWeapon.SetActive(false);
        kCharSkillPoint.SetActive(false);
        kSupporter.SetActive(true);

        kCharLevelUp.SetActive(false);
        kWeaponLevelUp.SetActive(false);

        kSupporterOn.SetActive(false);
        kSupporterOff.SetActive(false);
        kSupporterLevelUp.SetActive(false);

        kGUID.SetActive(false);
        kTitleLabel.SetActive(true);

        if (_facilitydata.TableData.EffectType == "FAC_CHAR_EXP")
        {
            kChar.SetActive(true);

            _chardata = GameInfo.Instance.GetCharData(resultdata.TargetUID);

            kCharIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/MainSlot/MainSlot_" + _chardata.TableData.Icon + "_" + _chardata.EquipCostumeID.ToString() + ".png");
            kCharNameLabel.textlocalize = FLocalizeString.Instance.GetText(_chardata.TableData.Name);
            kCharAddExpLabel.textlocalize = string.Format("+{0:#,##0}", _chardata.Exp - resultdata.TargetBeforeExp);

            _curcharlevel = resultdata.TargetBeforeLevel;
            _curcharexp = resultdata.TargetBeforeExp;
            _gaincharexp = resultdata.TargetBeforeExp;

            kCharGageAllGainExp = -1;

            float fillAmount = GameSupport.GetCharacterLevelExpGauge(_curcharlevel, _chardata.Grade, _curcharexp);
            kCharEXPGaugeUnit.InitGaugeUnit(fillAmount);

            if (GameSupport.IsMaxCharLevel(_curcharlevel, _chardata.Grade))
                kCharEXPGaugeUnit.SetText(FLocalizeString.Instance.GetText(221));
            else
                kCharEXPGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT), (fillAmount * 100.0f)));

            kCharLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curcharlevel);

            SetExpGainCharTween(_chardata.Exp - resultdata.TargetBeforeExp, GameInfo.Instance.GameConfig.ResultGaugeDuration);
            NotiCheck();
        }
        else if (_facilitydata.TableData.EffectType == "FAC_CHAR_SP")
        {
            kCharSkillPoint.SetActive(true);

            _chardata = GameInfo.Instance.GetCharData(resultdata.TargetUID);

            kSkillPointCharIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/MainSlot/MainSlot_" + _chardata.TableData.Icon + "_" + _chardata.EquipCostumeID.ToString() + ".png");
            kSkillPointCharNameLabel.textlocalize = FLocalizeString.Instance.GetText(_chardata.TableData.Name);
            kSkillPointLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(284), _chardata.PassviePoint, resultdata.TargetAfterLevel);
        }
        else if (_facilitydata.TableData.EffectType == "FAC_WEAPON_EXP")
        {
            kWeapon.SetActive(true);

            _weapondata = GameInfo.Instance.GetWeaponData(resultdata.TargetUID);
            kWeaponIconTex.gameObject.SetActive(true);
            kWeaponIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + _weapondata.TableData.Icon);
            kWeaponNameLabel.textlocalize = FLocalizeString.Instance.GetText(_weapondata.TableData.Name);
            kWeaponAddExpLabel.textlocalize = string.Format("+{0:#,##0}", _weapondata.Exp - resultdata.TargetBeforeExp);

            _curweaponlevel = resultdata.TargetBeforeLevel;
            _curweaponexp = resultdata.TargetBeforeExp;
            _gainweaponexp = resultdata.TargetBeforeExp;

            kWeaponGageAllGainExp = -1;

            float fillAmount = GameSupport.GetWeaponLevelExpGauge(_weapondata, _curcharlevel, _curcharexp);
            kWeaponEXPGaugeUnit.InitGaugeUnit(fillAmount);

            if (GameSupport.IsMaxLevelWeapon(_weapondata, _curweaponlevel))
                kWeaponEXPGaugeUnit.SetText(FLocalizeString.Instance.GetText(221));
            else
                kWeaponEXPGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (fillAmount * 100.0f)));

            kWeaponLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curweaponlevel);

            SetExpGainWeaponTween(_weapondata.Exp - resultdata.TargetBeforeExp, GameInfo.Instance.GameConfig.ResultGaugeDuration);
        }
        else if (_facilitydata.TableData.EffectType == "FAC_ITEM_COMBINE")
        {
            kItem.SetActive(true);

            if(GameInfo.Instance.netFlag)
                _itemdata = GameInfo.Instance.GetItemData((int)resultdata.TargetUID);
            else
                _itemdata = GameInfo.Instance.GetItemData(resultdata.TargetUID);
            if (_itemdata == null)
                Debug.LogError("ItemData is NULL : " + (int)resultdata.TargetUID);
            kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, -1, _itemdata, resultdata.TargetAfterLevel);
            kItemNameLabel.textlocalize = FLocalizeString.Instance.GetText(_itemdata.TableData.Name);
        }
        else if (_facilitydata.TableData.EffectType == "FAC_CARD_TRADE")
        {
            kGUID.SetActive(true);
            kTitleLabel.SetActive(false);
        }
        else if (_facilitydata.TableData.EffectType == "FAC_OPERATION_ROOM")
        {
            kSupporter.SetActive(false);
            kItem.SetActive(true);
            _itemdata = GameInfo.Instance.GetItemData((int)resultdata.ItemUID);

            if (_itemdata != null)
            {
                kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, -1, _itemdata, resultdata.ItemCount);
                kItemNameLabel.textlocalize = FLocalizeString.Instance.GetText(_itemdata.TableData.Name);
            }
        }

        if(_facilitydata.EquipCardUID != (int)eCOUNT.NONE)
        {
            _carddata = GameInfo.Instance.GetCardData(_facilitydata.EquipCardUID);
            if (_carddata == null)
            {
                Debug.LogError("CardData is NULL : " + (int)_facilitydata.EquipCardUID);
                return;
            }
                
            _cardbookdata = GameInfo.Instance.GetCardBookData(_carddata.TableID);

            _curSupporterlevel = resultdata.CardBeforeLevel;
            _curSupporterexp = resultdata.CardBeforeExp;
            _gainSupporterexp = resultdata.CardBeforeExp;
            kSupporterAllGainExp = -1;

            float fillAmount = GameSupport.GetCardFavorLevelExpGauge(_cardbookdata.TableID, _curSupporterlevel, _curSupporterexp);
            kSupporterFavorGauge.InitGaugeUnit(fillAmount);
            
            kSupporterFavorLv.textlocalize = _curSupporterlevel.ToString();
            kSupporterTexIcon.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Item/{0}.png", _carddata.TableData.Icon));
        }

        SetReStartBtn();
    }

    private void NotiCheck()
    {
        FacilityResultData facilityresultdata = GameInfo.Instance.FacilityResultData;
        //UserData userdata = GameInfo.Instance.UserData;
        FacilityData facilitydata = GameInfo.Instance.GetFacilityData(facilityresultdata.FacilityID);
        CharData chardata = GameInfo.Instance.GetCharData(facilityresultdata.TargetUID);

        string icoNoticeSpriteName = "ico_Notice";

        if (facilityresultdata.TargetBeforeLevel != facilityresultdata.TargetAfterLevel)
        {
            //캐릭터 기술 개방
            var seleteskilllist = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.CharacterID == chardata.TableID && (x.Type == (int)eCHARSKILLPASSIVETYPE.SELECT_NORMAL));
            for (int j = 0; j < seleteskilllist.Count; j++)
            {
                var passviedata = chardata.PassvieList.Find(x => x.SkillID == seleteskilllist[j].ID);
                if (passviedata == null)
                {
                    var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == seleteskilllist[j].ItemReqListID);
                    if (reqdata != null)
                    {
                        for (int lvCheck = facilityresultdata.TargetBeforeLevel + 1; lvCheck <= facilityresultdata.TargetAfterLevel; lvCheck++)
                        {
                            if (lvCheck == reqdata.LimitLevel)
                            {
                                string text = string.Format(FLocalizeString.Instance.GetText(4108), FLocalizeString.Instance.GetText(chardata.TableData.Name), FLocalizeString.Instance.GetText(seleteskilllist[j].Name));
                                //string ico = string.Format("Icon/Skill/{0}", seleteskilllist[j].Icon);
                                NoticePopup.ShowText(UINoticePopup.eTYPE.SPR, text, icoNoticeSpriteName, UINoticePopup.eRUNTYPE.CHARSKILL, (int)chardata.CUID, chardata.TableID);
                            }
                        }
                    }
                }
            }
            //캐릭터 스킬 슬롯 개방
            for (int k = 1; k < (int)eCOUNT.SKILLSLOT; k++)
            {
                for (int lvCheck = facilityresultdata.TargetBeforeLevel; lvCheck <= facilityresultdata.TargetAfterLevel; lvCheck++)
                {
                    if (lvCheck == GameInfo.Instance.GameConfig.CharSkillSlotLimitLevel[k])
                    {
                        string str = string.Format("UserData_SlotEffect_{0}_{1}", chardata.TableData.ID.ToString(), k);
                        if (PlayerPrefs.HasKey(str))
                        {
                            string text = string.Format(FLocalizeString.Instance.GetText(4107), FLocalizeString.Instance.GetText(chardata.TableData.Name));
                            NoticePopup.ShowText(UINoticePopup.eTYPE.SPR, text, icoNoticeSpriteName, UINoticePopup.eRUNTYPE.CHARSKILL, (int)chardata.CUID, chardata.TableID);
                        }
                    }
                }                
            }
            //캐릭터 서포터 슬롯 개방
            for (int idx = 1; idx < (int)eCOUNT.CARDSLOT; idx++)
            {
                for (int lvCheck = facilityresultdata.TargetBeforeLevel; lvCheck <= facilityresultdata.TargetAfterLevel; lvCheck++)
                {
                    if (lvCheck == GameInfo.Instance.GameConfig.CharCardSlotLimitLevel[idx])
                    {
                        string text = string.Format(FLocalizeString.Instance.GetText(4109), FLocalizeString.Instance.GetText(chardata.TableData.Name));
                        NoticePopup.ShowText(UINoticePopup.eTYPE.SPR, text, icoNoticeSpriteName, UINoticePopup.eRUNTYPE.CHARCARD, (int)chardata.CUID, chardata.TableID);
                    }
                }
            }
            //캐릭터 서브 무기 슬롯 개방
            for (int idx = 1; idx < (int)eCOUNT.WEAPONSLOT; idx++)
            {
                for (int lvCheck = facilityresultdata.TargetBeforeLevel; lvCheck <= facilityresultdata.TargetAfterLevel; lvCheck++)
                {
                    if (lvCheck == GameInfo.Instance.GameConfig.CharWeaponSlotLimitLevel[idx])
                    {
                        string text = string.Format(FLocalizeString.Instance.GetText(4111), FLocalizeString.Instance.GetText(chardata.TableData.Name));
                        NoticePopup.ShowText(UINoticePopup.eTYPE.SPR, text, icoNoticeSpriteName, UINoticePopup.eRUNTYPE.CHARWEAPON, (int)chardata.CUID, chardata.TableID);
                    }
                }
            }
        }
    }
    void FixedUpdate()
    {
        if (kCharGageAllGainExp != -1)
        {
            int nowexp = _curcharexp + kCharGageAllGainExp;
            int level = GameSupport.GetCharacterExpLevel(nowexp, _chardata.Grade);

            float fillAmount = GameSupport.GetCharacterLevelExpGauge(level, _chardata.Grade, nowexp);
            kCharEXPGaugeUnit.InitGaugeUnit(fillAmount);

            if (GameSupport.IsMaxCharLevel(_curcharlevel, _chardata.Grade))
                kCharEXPGaugeUnit.SetText(FLocalizeString.Instance.GetText(221));
            else
                kCharEXPGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT), (fillAmount * 100.0f)));

            if (_curcharlevel != level)
            {
                if (_chardata != null)
                    GameSupport.IsCharOpenSkillSlot(_chardata, _curcharlevel, level);
                _curcharlevel = level;
                kCharLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curcharlevel);
                kCharLevelUp.SetActive(false);
                kCharLevelUp.SetActive(true);
            }
        }

        if (kWeaponGageAllGainExp != -1)
        {
            int nowexp = _curweaponexp + kWeaponGageAllGainExp;
            int level = GameSupport.GetWeaponExpLevel(_weapondata,nowexp);

            float fillAmount = GameSupport.GetWeaponLevelExpGauge(_weapondata, level, nowexp);
            kWeaponEXPGaugeUnit.InitGaugeUnit(fillAmount);

            if (GameSupport.IsMaxLevelWeapon(_weapondata, _curweaponlevel))
                kWeaponEXPGaugeUnit.SetText(FLocalizeString.Instance.GetText(221));
            else
                kWeaponEXPGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (fillAmount * 100.0f)));

            if (_curweaponlevel != level)
            {
                _curweaponlevel = level;
                kWeaponLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curweaponlevel);
                kWeaponLevelUp.SetActive(false);
                kWeaponLevelUp.SetActive(true);
            }

            
        }

        if(_carddata != null && _cardbookdata != null)
        {
            if (kSupporterAllGainExp != -1)
            {
                int nowexp = _curSupporterexp + kSupporterAllGainExp;
                int level = GameSupport.GetCardFavorLevel(_carddata.TableID, nowexp);

                float fillAmount = GameSupport.GetCardFavorLevelExpGauge(_cardbookdata.TableID, level, nowexp);
                kSupporterFavorGauge.InitGaugeUnit(fillAmount);
                kSupporterFavorLv.textlocalize = _curSupporterlevel.ToString();
                if(_curSupporterlevel != level)
                {
                    kSupporterLevelUp.SetActive(true);
                    _curSupporterlevel = level;
                    GameSupport.TweenerPlay(kSupporterFavorLv.transform.parent.gameObject);
                }
            }
        }
        
    }
    private void SetExpGainCharTween(int gainexp, float ftime)
    {
        //GameResultData gameresultdata = GameInfo.Instance.GameResultData;
        
        Utility.StopCoroutine(this, ref m_crCharExp);

        _gaincharexp = gainexp;
        m_crCharExp = StartCoroutine(Utility.UpdateCoroutineValue((x) => kCharGageAllGainExp = (int)x, kCharGageAllGainExp, _gaincharexp, ftime));
    }
    private void SetExpGainWeaponTween( int gainexp, float ftime)
    {
        Utility.StopCoroutine(this, ref m_crWeaponExp);

        _gainweaponexp = gainexp;
        m_crWeaponExp = StartCoroutine(Utility.UpdateCoroutineValue((x) => kWeaponGageAllGainExp = (int)x, kWeaponGageAllGainExp, _gainweaponexp, ftime));
    }
    private void SetExpGainSupporterFavorTween(int gainexp, float ftime)
    {
        Utility.StopCoroutine(this, ref m_crSupporterFavorExp);

        _gainSupporterexp = gainexp;
        m_crSupporterFavorExp = StartCoroutine(Utility.UpdateCoroutineValue((x) => kSupporterAllGainExp = (int)x, kSupporterAllGainExp, _gainSupporterexp, ftime));
    }
    public void OnClick_CloseBtn()
	{
        OnClickClose();
    }
    private void SetReStartBtn()
    {
        if (_facilitydata.TableData.EffectType == "FAC_CHAR_EXP")
        {
            //조건 추가 할것
            _restartType = eRestartType.ReStart;
            if (_chardata.Grade >= GameInfo.Instance.GameConfig.CharStartAwakenGrade)
            {
                if (!GameSupport.IsMaxGrade(_chardata.Grade) && GameSupport.IsMaxCharLevel(_chardata.Level, _chardata.Grade))
                    _restartType = eRestartType.AwakenGradeUp;
            }
            else
            {
                if(GameSupport.IsMaxCharLevel(_chardata.Level, _chardata.Grade))
                    _restartType = eRestartType.GradeUp;
            }

            if(GameSupport.IsMaxGrade(_chardata.Grade) && GameSupport.IsMaxCharLevel(_chardata.Level, _chardata.Grade))
            {
                _restartType = eRestartType.NONE;
            }
        }
        else if (_facilitydata.TableData.EffectType == "FAC_CHAR_SP")
        {
            _restartType = eRestartType.ReStart;
        }
        else if (_facilitydata.TableData.EffectType == "FAC_WEAPON_EXP")
        {
            //조건 추가 할것
            _restartType = eRestartType.ReStart;
            if(GameSupport.IsMaxLevelWeapon(_weapondata))
            {
                if (GameSupport.IsMaxWakeWeapon(_weapondata))
                    _restartType = eRestartType.NONE;
                else
                    _restartType = eRestartType.GradeUp;
            }
        }
        else if (_facilitydata.TableData.EffectType == "FAC_ITEM_COMBINE")
        {
            _restartType = eRestartType.NONE;
        }
        else if (_facilitydata.TableData.EffectType == "FAC_CARD_TRADE")
        {
            _restartType = eRestartType.NONE;
        }
        else
        {
            _restartType = eRestartType.NONE;
        }

        switch(_restartType)
        {
            case eRestartType.ReStart:
                {
                    kReStartBtn.SetActive(true);
                    kOkBtn.SetActive(true);
                    kReStartLabel.textlocalize = FLocalizeString.Instance.GetText(1693);
                }
                break;
            case eRestartType.GradeUp:
                {
                    kReStartBtn.SetActive(true);
                    kOkBtn.SetActive(true);
                    if (_facilitydata.TableData.EffectType == "FAC_CHAR_EXP")
                        kReStartLabel.textlocalize = FLocalizeString.Instance.GetText(1694);
                    else if (_facilitydata.TableData.EffectType == "FAC_WEAPON_EXP")
                        kReStartLabel.textlocalize = FLocalizeString.Instance.GetText(1696);

                }
                break;
            case eRestartType.AwakenGradeUp:
                {
                    kReStartBtn.SetActive(true);
                    kOkBtn.SetActive(true);
                    kReStartLabel.textlocalize = FLocalizeString.Instance.GetText(1695);
                }
                break;
            default:
                {
                    kReStartBtn.SetActive(false);
                    kOkBtn.SetActive(false);
                }
                break;
        }
    }
    public void OnClick_ReStartBtn()
    {
        switch(_restartType)
        {
            case eRestartType.ReStart:
                {
                    if (_facilitydata.TableData.EffectType == "FAC_CHAR_EXP" || _facilitydata.TableData.EffectType == "FAC_CHAR_SP")
                    {
                        if (_chardata == null)
                        {
                            OnClickClose();
                            return;
                        }

                        GameInfo.Instance.Send_ReqFacilityOperation(_facilitydata.TableID, _chardata.CUID, 1, null, OnNetFacilityUse);
                    }
                    else if (_facilitydata.TableData.EffectType == "FAC_WEAPON_EXP")
                    {
                        GameInfo.Instance.Send_FacilityItemEquip(_facilitydata.TableID, _weapondata.WeaponUID, 1, OnNetFacilityUse);
                    }
                }
                break;
            case eRestartType.GradeUp:
                {
                    if (_facilitydata.TableData.EffectType == "FAC_CHAR_EXP")
                    {
                        UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, _chardata.CUID);
                        UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, _chardata.TableID);

                        GameSupport.MoveUI("UIPANEL", "CHARINFO", "CharGradeUpPopup", "");
                    }
                    else if (_facilitydata.TableData.EffectType == "FAC_WEAPON_EXP")
                    {
                        UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, _weapondata.WeaponUID);
                        GameSupport.MoveUI("UIPANEL", "ITEM", "0", "WeaponGradeUpPopup");
                    }

                    OnClickClose();
                }
                break;
            case eRestartType.AwakenGradeUp:
                {
                    if (_facilitydata.TableData.EffectType == "FAC_CHAR_EXP")
                    {
                        UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, _chardata.CUID);
                        UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, _chardata.TableID);

                        GameSupport.MoveUI("UIPANEL", "CHARINFO", "CharGradeUpPopup", "");
                    }
                    OnClickClose();
                }
                break;
            default:
                OnClickClose();
                break;
        }
    }
    public void OnNetFacilityUse(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("FacilityItemPanel");
        LobbyUIManager.Instance.Renewal("FacilityPanel");
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.HideUI("CharSeletePopup");

        Lobby.Instance.ActivationRoomEffect(_facilitydata.TableID);

        var carddata = GameInfo.Instance.GetCardData(_facilitydata.EquipCardUID);
        if (carddata != null)
        {
            VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.FacilityStart, carddata.TableID);
        }

        OnClickClose();
    }
    public void OnClick_OkBtn()
    {
        OnClickClose();
    }
    IEnumerator ResultCoroutine()
    {
        FacilityResultData resultdata = GameInfo.Instance.FacilityResultData;
        UserData userdata = GameInfo.Instance.UserData;
        
        if (_facilitydata.TableData.EffectType == "FAC_CHAR_EXP")
        {
            SetExpGainCharTween(_chardata.Exp - resultdata.TargetBeforeExp, GameInfo.Instance.GameConfig.ResultGaugeDuration);
        }
        else if (_facilitydata.TableData.EffectType == "FAC_CHAR_SP")
        {
            
        }
        else if (_facilitydata.TableData.EffectType == "FAC_WEAPON_EXP")
        {
            SetExpGainWeaponTween(_weapondata.Exp - resultdata.TargetBeforeExp, GameInfo.Instance.GameConfig.ResultGaugeDuration);
        }
        else if (_facilitydata.TableData.EffectType == "FAC_ITEM_COMBINE")
        {

        }

        if (_facilitydata.EquipCardUID != (int)eCOUNT.NONE)
        {
            kSupporterOn.SetActive(true);
            
            if(_carddata != null)
            {
                _cardbookdata = GameInfo.Instance.GetCardBookData(_carddata.TableID);
                SetExpGainSupporterFavorTween(_cardbookdata.FavorExp - resultdata.CardBeforeExp, GameInfo.Instance.GameConfig.ResultGaugeDuration);
            }
        }
        else
        {
            kSupporterOff.SetActive(true);
        }

        yield return new WaitForSeconds(GameInfo.Instance.GameConfig.ResultGaugeDuration);

        if (_facilitydata.TableData.EffectType == "FAC_CHAR_EXP")
        {
            
        }
        else if (_facilitydata.TableData.EffectType == "FAC_CHAR_SP")
        {

        }
        else if (_facilitydata.TableData.EffectType == "FAC_WEAPON_EXP")
        {
            if (GameSupport.IsWeaponOpenTerms_Effect(_weapondata))
            {
                if (_weapondata.Exp != resultdata.TargetBeforeExp)
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.SelectRewardType, eREWARDTYPE.WEAPON);
                    UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, _weapondata.WeaponUID);
                    DirectorUIManager.Instance.PlayMaxLevel();
                    RenderTargetWeapon.Instance.ShowWeaponEffect(true);
                }
            }
        }
        else if (_facilitydata.TableData.EffectType == "FAC_ITEM_COMBINE")
        {

        }

        

        kCloseBtn.SetActive(true);
    }
}
