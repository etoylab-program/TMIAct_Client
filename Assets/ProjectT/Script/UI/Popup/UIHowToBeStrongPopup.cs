using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHowToBeStrongPopup : FComponent
{
    public enum eNOTICETYPE
    {
        NONE = -1,
        CharLevelUp,
        CharGradeUp,
        CharSkillGet,
        SupporterLevelGradeUp,
        SupporterSkillLevelUp,
        SupporterDeploy,
        WeaponLevelUp,
        WeaponGradeUp,
        WeaponSkillLevelUp,
        GemLevelUp,
        GemGradeUp,
        GemOptChange,
    }
    public enum eSTATE
    {
        MAIN = 0,
        DETAIL_CHAR,
        DETAIL_CARD,
        DETAIL_WEAPON,
        DETAIL_GEM,
        COUNT,
    }

    

    public GameObject kHelpMain;
	public GameObject kHelpDetail;
	public UILabel kDetailLabel;
	public FToggle kHelpToggle;
	public UISprite kHelpSpr;
	public UILabel kDescLabel;
	public GameObject kHelpItem;
    public List<UIItemListSlot> kHelpItemList;
    public GameObject kTitle;
    public UILabel kTitleLabel;
	public UILabel kTitleLLabel;
	public UILabel kTitleRLabel;
	public FTab kHelpTab;
    public UIButton kBackBtn;
    public UIButton kMoveBtn;
    //public UISprite kMoveIconSpr;
    public List<UISprite> kNoticeList;
    public List<UISprite> kBTabNoticeList;
    private eSTATE _state;
    private int _selete;
    private CharData _chardata;
    //private CardData _carddata;
    private WeaponData _weapondata;
    public override void Awake()
	{
		base.Awake();

		kHelpToggle.EventCallBack = OnHelpToggleSelect;
		kHelpTab.EventCallBack = OnHelpTabSelect;
	}
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}    

    public override void InitComponent()
	{
        _chardata = null;
        _weapondata = null;
        long cuid = (long)UIValue.Instance.GetValue(UIValue.EParamType.HowToBeStrongCUID);
        _chardata = GameInfo.Instance.GetCharData(cuid);
        
        for (int i = 0; i < kNoticeList.Count; i++)
            kNoticeList[i].gameObject.SetActive(false);

        if(_chardata != null)
        {
            //캐릭터 진급
            if (GameSupport.IsCharGradeUp(_chardata))
            {
                kNoticeList[(int)eNOTICETYPE.CharGradeUp].gameObject.SetActive(true);
            }
            //캐릭터 스킬 패시브 스킬 강화 가능, 스킬 획득 가능
            if (GameSupport.IsCharSkillUp(_chardata) || GameSupport.IsCharPassiveSkillUp(_chardata))
            {
                kNoticeList[(int)eNOTICETYPE.CharSkillGet].gameObject.SetActive(true);
            }
            
            for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
            {
                var carddata = GameInfo.Instance.GetCardData(_chardata.EquipCard[i]);
                if (carddata != null)
                {
                    //서포터 강화/각성
                    var list = GameInfo.Instance.ItemList.FindAll(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_CARD_LEVELUP);
                    if (list.Count != 0 && !GameSupport.IsMaxLevelCard(carddata) || GameSupport.IsCardWakeUp(carddata))
                    {
                        kNoticeList[(int)eNOTICETYPE.SupporterLevelGradeUp].gameObject.SetActive(true);
                    }
                    //서포터 스킬 강화
                    if (GameSupport.IsCardSkillUp(carddata))
                    {
                        kNoticeList[(int)eNOTICETYPE.SupporterSkillLevelUp].gameObject.SetActive(true);
                    }
                }
            }
            
            _weapondata = GameInfo.Instance.GetWeaponData(_chardata.EquipWeaponUID);
            if (_weapondata != null)
            {
                //무기 강화
                var list = GameInfo.Instance.ItemList.FindAll(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_WEAPON_LEVELUP);
                if (list.Count != 0 && !GameSupport.IsMaxLevelWeapon(_weapondata))
                {
                    kNoticeList[(int)eNOTICETYPE.WeaponLevelUp].gameObject.SetActive(true);
                }
                //무기 제련
                if (GameSupport.IsWeaponWakeUp(_weapondata))
                {
                    kNoticeList[(int)eNOTICETYPE.WeaponGradeUp].gameObject.SetActive(true);
                }
                //무기 스킬 강화
                if (GameSupport.IsWeaponSkillUp(_weapondata))
                {
                    kNoticeList[(int)eNOTICETYPE.WeaponSkillLevelUp].gameObject.SetActive(true);
                }

                for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
                {
                    if (_weapondata.SlotGemUID[i] != (int)eCOUNT.NONE)
                    {
                        GemData gemdata = GameInfo.Instance.GetGemData(_weapondata.SlotGemUID[i]);
                        if (gemdata != null)
                        {
                            //곡옥 강화
                            if (GameSupport.IsGemLevelUp() && !GameSupport.IsMaxLevelGem(gemdata))
                            {
                                kNoticeList[(int)eNOTICETYPE.GemLevelUp].gameObject.SetActive(true);
                            }
                            //곡옥 연마
                            if (GameSupport.IsGemWakeUp(gemdata))
                            {
                                kNoticeList[(int)eNOTICETYPE.GemGradeUp].gameObject.SetActive(true);
                            }
                            //재설정
                            if (GameSupport.IsGemOptChange(gemdata))
                            {
                                kNoticeList[(int)eNOTICETYPE.GemOptChange].gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }
        }

        SetState(eSTATE.MAIN, 0);
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        if (_state == eSTATE.MAIN)
            return;

        var data = GameInfo.Instance.GameClientTable.FindHowToBeStrong(x => x.Group == (int)_state && x.Index == _selete);
        if (data == null)
            return;

        var datalist = GameInfo.Instance.GameClientTable.FindAllHowToBeStrong(x => x.Group == (int)_state);
        if (datalist == null)
            return;

        kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(data.Name);
        kDetailLabel.textlocalize = FLocalizeString.Instance.GetText(data.Detail);
        kDescLabel.textlocalize = FLocalizeString.Instance.GetText(data.Desc);
        kHelpSpr.spriteName = data.Icon;
        //kHelpTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Help/" + data.Icon);
        int index = ((int)(_state - 1) * 3) + _selete;

        kMoveBtn.gameObject.SetActive(false);
        if (0 <= index && kNoticeList.Count > index)
        {
            if (kNoticeList[index].gameObject.activeSelf)
                kMoveBtn.gameObject.SetActive(true);
        }

        /*
        kMoveIconSpr.gameObject.SetActive(false);
        if (0 <= index && kNoticeList.Count > index)
        {
            if (kNoticeList[index].gameObject.activeSelf)
                kMoveIconSpr.gameObject.SetActive(true);
        }
        */
        for (int i = 0; i < kBTabNoticeList.Count; i++)
            kBTabNoticeList[i].gameObject.SetActive(false);

        
        for (int i = 0; i < datalist.Count; i++)
        {
            kHelpTab.SetTabLabel(i, FLocalizeString.Instance.GetText(datalist[i].BtnText));

            if (kNoticeList[((int)(_state - 1) * 3) + i].gameObject.activeSelf)
                kBTabNoticeList[i].gameObject.SetActive(true);
        }

        kTitleLLabel.textlocalize = "";
        kTitleRLabel.textlocalize = "";
        int indexl = (int)_state - 1;
        int indexr = (int)_state + 1;
        if (indexl <= 1)
            indexl = (int)eSTATE.COUNT - 1;
        if (indexr >= (int)eSTATE.COUNT)
            indexr = 1;
        var datal = GameInfo.Instance.GameClientTable.FindHowToBeStrong(x => x.Group == (int)indexl && x.Index == 0);
        if (datal != null)
            kTitleLLabel.textlocalize = FLocalizeString.Instance.GetText(datal.Name);
        var datar = GameInfo.Instance.GameClientTable.FindHowToBeStrong(x => x.Group == (int)indexr && x.Index == 0);
        if (datar != null)
            kTitleRLabel.textlocalize = FLocalizeString.Instance.GetText(datar.Name);

        if( kHelpToggle.kSelect == 0 )
        {
            kDescLabel.gameObject.SetActive(true);
            kHelpItem.SetActive(false);
        }
        else
        {
            kDescLabel.gameObject.SetActive(false);
            kHelpItem.SetActive(true);
        }

        

        List<int> idlist = new List<int>();
        if (data.ItemID1 != -1)
            idlist.Add(data.ItemID1);
        if (data.ItemID2 != -1)
            idlist.Add(data.ItemID2);
        if (data.ItemID3 != -1)
            idlist.Add(data.ItemID3);
        if (data.ItemID4 != -1)
            idlist.Add(data.ItemID4);
        if (data.ItemID5 != -1)
            idlist.Add(data.ItemID5);

        for ( int i = 0; i < kHelpItemList.Count; i++ )
            kHelpItemList[i].gameObject.SetActive(false);

        for (int i = 0; i < idlist.Count; i++)
        {
            kHelpItemList[i].gameObject.SetActive(true);
            kHelpItemList[i].UpdateSlot(UIItemListSlot.ePosType.Mat, i, GameInfo.Instance.GameTable.FindItem(idlist[i]));
            int orgcut = GameInfo.Instance.GetItemIDCount(idlist[i]);
            if (orgcut > 0)
                kHelpItemList[i].SetCountLabel(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), orgcut));
            else
                kHelpItemList[i].SetCountLabel(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_R), orgcut));
        }

        if (idlist.Count == 0)
            kHelpToggle.gameObject.SetActive(false);
        else
            kHelpToggle.gameObject.SetActive(true);


    }

    private bool OnHelpToggleSelect(int nSelect, SelectEvent type)
	{
        if (type == SelectEvent.Enable)
            return false;
        if (type == SelectEvent.Code)
            return true;

        Renewal(true);

        return true;
	}
	
	private bool OnHelpTabSelect(int nSelect, SelectEvent type)
	{
        if (type == SelectEvent.Enable)
            return false;
        if (type == SelectEvent.Code)
            return true;

        _selete = nSelect;
        kHelpToggle.SetToggle(0, SelectEvent.Code);
        Renewal(true);

        return true;
	}

	
	public void OnClick_HelpCharLevelUpBtn()
	{
        SetState(eSTATE.DETAIL_CHAR, 0);
    }
	
	public void OnClick_HelpCharGradeUpBtn()
	{
        SetState(eSTATE.DETAIL_CHAR, 1);
    }
	
	public void OnClick_HelpCharSkillGetBtn()
	{
        SetState(eSTATE.DETAIL_CHAR, 2);
    }
	
	public void OnClick_HelpSupporterLevelUpBtn()
	{
        SetState(eSTATE.DETAIL_CARD, 0);
    }
	
	public void OnClick_HelpSupporterGradeUpBtn()
	{
        SetState(eSTATE.DETAIL_CARD, 1);
    }
	
	public void OnClick_HelpSupporterSkillLevelUpBtn()
	{
        SetState(eSTATE.DETAIL_CARD, 2);
    }
	
	public void OnClick_HelpWeaponLevelUpBtn()
	{
        SetState(eSTATE.DETAIL_WEAPON, 0);
    }
	
	public void OnClick_HelpWeaponGradeUpBtn()
	{
        SetState(eSTATE.DETAIL_WEAPON, 1);
    }
	
	public void OnClick_HelpWeaponSkillLevelUpBtn()
	{
        SetState(eSTATE.DETAIL_WEAPON, 2);
    }
	
	public void OnClick_HelpGemLevelUpBtn()
	{
        SetState(eSTATE.DETAIL_GEM, 0);
    }
	
	public void OnClick_HelpGemGradeUpBtn()
	{
        SetState(eSTATE.DETAIL_GEM, 1);
    }
	
	public void OnClick_HelpGemOptChangeBtn()
	{
        SetState(eSTATE.DETAIL_GEM, 2);
    }
	
	public void OnClick_CloseBtn()
	{
        if(GameInfo.Instance.bStageFailure)
        {
            LobbyUIManager.Instance.ShowAddSpecialPopup();
        }
        OnClickClose();
	}

    public override void OnClickClose()
    {
        GameInfo.Instance.bStageFailure = false;
        base.OnClickClose();
    }

    public void OnClick_BackBtn()
    {
        SetState(eSTATE.MAIN, 0);
    }
    
    public void OnClick_TitleLBtn()
	{
        int index = (int)_state;
        index -= 1;
        if (index <= 0)
            index = (int)eSTATE.COUNT - 1;

        SetState((eSTATE)index, 0);        
    }

    public void OnClick_StorePackBtn()
    {
        GameSupport.PaymentAgreement_Package();
    }

    public void OnClick_StorePackWithStoreIDBtn(int storeId)
    {
        GameSupport.PaymentAgreement_Package(storeId);
    }
	
	public void OnClick_TitleRBtn()
	{
        int index = (int)_state;
        index += 1;
        if (index >= (int)eSTATE.COUNT)
            index = 1;

        SetState((eSTATE)index, 0);
    }

    public void OnClick_MoveBtn()
    {
        if (_chardata == null)
            return;
        //if( !kMoveIconSpr.gameObject.activeSelf )
        //    return;

        int index = ((int)(_state - 1) * 3) + _selete;
        if (0 <= index && kNoticeList.Count > index)
        {
            if (!kNoticeList[index].gameObject.activeSelf)
                return;
        }


        bool bnoti = false;
        for (int i = 0; i < kNoticeList.Count; i++)
            if( kNoticeList[i].gameObject.activeSelf )
                bnoti = true;

        if (!bnoti)
            return;

        if (_state == eSTATE.MAIN)
            return;

        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.CHARINFO)
        {
            Log.Show("LobbyUIManager panelType is CHARINFO");
            OnClickClose();
            LobbyUIManager.Instance.HideUI("MenuPopup");
            return;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, _chardata.CUID);
        UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, _chardata.TableID);


        if (_state == eSTATE.DETAIL_CHAR)
        {
            if(_selete == (int)UICharInfoPanel.eCHARINFOTAB.SKILL)
                UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.SKILL);
            else
                UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.STATUS);
            //if( kNoticeList[(int)eNOTICETYPE.CharGradeUp].gameObject.activeSelf )
            //    UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.STATUS);
            //else
            //    UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.SKILL);
        }
        else if (_state == eSTATE.DETAIL_CARD)
            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.SUPPORTER);
        else if (_state == eSTATE.DETAIL_WEAPON)
            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.WEAPON);
        else if (_state == eSTATE.DETAIL_GEM)
            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.WEAPON);

        OnClickClose();
        LobbyUIManager.Instance.HideUI("MenuPopup");
        
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);
        Lobby.Instance.MoveToLobby();
    }

    private void SetState(eSTATE e, int selete )
    {
        eSTATE olde = _state;
        _state = e;
        _selete = selete;

        if(_state == eSTATE.MAIN )
        {
            kHelpMain.SetActive(true);
            kHelpDetail.SetActive(false);

            kTitle.SetActive(false);
            kBackBtn.gameObject.SetActive(false);
            kHelpTab.gameObject.SetActive(false);
        }
        else
        {
            kHelpMain.SetActive(false);
            kHelpDetail.SetActive(true);

            kTitle.SetActive(true);
            kBackBtn.gameObject.SetActive(true);
            kHelpTab.gameObject.SetActive(true);

            kHelpTab.SetTab(_selete, SelectEvent.Code);
            kHelpToggle.SetToggle(0, SelectEvent.Code);
        }

        Renewal(true);
    }

}
