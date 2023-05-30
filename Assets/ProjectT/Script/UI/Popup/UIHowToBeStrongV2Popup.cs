using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHowToBeStrongV2Popup : FComponent
{  
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
    public UIButton kBackBtn;
    public UIButton kMoveBtn;
    private eSTATE _state;
    public eSTATE CurState { get { return _state; } }
    private int _selete;
    public int CurSelect { get { return _selete; } }
    private CharData _chardata;

    [SerializeField] private FList HelpCharacterList;
    [SerializeField] private FList HelpSupporterList;
    [SerializeField] private FList HelpWeaponList;
    [SerializeField] private FList HelpGemList;
    [SerializeField] private FList HelpDetailList;

    private List<GameClientTable.HowToBeStrong.Param> HTBSCharacterParams;
    private List<GameClientTable.HowToBeStrong.Param> HTBSSupporterParams;
    private List<GameClientTable.HowToBeStrong.Param> HTBSWeaponParams;
    private List<GameClientTable.HowToBeStrong.Param> HTBSGemParams;


    public override void Awake()
	{
		base.Awake();

        kHelpToggle.EventCallBack = OnHelpToggleSelect;

        HTBSCharacterParams = GameInfo.Instance.GameClientTable.FindAllHowToBeStrong(x => x.Group == (int)eSTATE.DETAIL_CHAR);
        HTBSSupporterParams = GameInfo.Instance.GameClientTable.FindAllHowToBeStrong(x => x.Group == (int)eSTATE.DETAIL_CARD);
        HTBSWeaponParams = GameInfo.Instance.GameClientTable.FindAllHowToBeStrong(x => x.Group == (int)eSTATE.DETAIL_WEAPON);
        HTBSGemParams = GameInfo.Instance.GameClientTable.FindAllHowToBeStrong(x => x.Group == (int)eSTATE.DETAIL_GEM);

        if (HelpCharacterList != null)
        {
            HelpCharacterList.EventUpdate = UpdateCharacterListSlot;
            HelpCharacterList.EventGetItemCount = () => { return HTBSCharacterParams.Count; };
            HelpCharacterList.UpdateList();
        }

        if (HelpSupporterList != null)
        {
            HelpSupporterList.EventUpdate = UpdateSupporterListSlot;
            HelpSupporterList.EventGetItemCount = () => { return HTBSSupporterParams.Count; };
            HelpSupporterList.UpdateList();
        }
        if (HelpWeaponList != null)
        {
            HelpWeaponList.EventUpdate = UpdateWeaponListSlot;
            HelpWeaponList.EventGetItemCount = () => { return HTBSWeaponParams.Count; };
            HelpWeaponList.UpdateList();
        }
        if (HelpGemList != null)
        {
            HelpGemList.EventUpdate = UpdateGemListSlot;
            HelpGemList.EventGetItemCount = () => { return HTBSGemParams.Count; };
            HelpGemList.UpdateList();
        }
        if (HelpDetailList != null)
        {
            HelpDetailList.EventUpdate = UpdateDetailListSlot;
            HelpDetailList.EventGetItemCount = () => {

                switch (_state)
                {
                    case eSTATE.DETAIL_CHAR:
                        return HTBSCharacterParams.Count;                        
                    case eSTATE.DETAIL_CARD:
                        return HTBSSupporterParams.Count;
                    case eSTATE.DETAIL_WEAPON:
                        return HTBSWeaponParams.Count;
                    case eSTATE.DETAIL_GEM:
                        return HTBSGemParams.Count;                        
                }
                return 0; 
            };
            HelpDetailList.UpdateList();
        }
    }


    private void UpdateCharacterListSlot(int index, GameObject slotObj)
    {
        UIHowToBeStrongListSlot  slot = slotObj.GetComponent<UIHowToBeStrongListSlot>();
        if (slot == null) return;

        slot.ParentGO = gameObject;
        slot.UpdateSlot(index, HTBSCharacterParams[index]);
    }

    private void UpdateSupporterListSlot(int index, GameObject slotObj)
    {
        UIHowToBeStrongListSlot slot = slotObj.GetComponent<UIHowToBeStrongListSlot>();
        if (slot == null) return;

        slot.ParentGO = gameObject;
        slot.UpdateSlot(index, HTBSSupporterParams[index]);
    }

    private void UpdateWeaponListSlot(int index, GameObject slotObj)
    {
        UIHowToBeStrongListSlot slot = slotObj.GetComponent<UIHowToBeStrongListSlot>();
        if (slot == null) return;

        slot.ParentGO = gameObject;
        slot.UpdateSlot(index, HTBSWeaponParams[index]);
    }

    private void UpdateGemListSlot(int index, GameObject slotObj)
    {
        UIHowToBeStrongListSlot slot = slotObj.GetComponent<UIHowToBeStrongListSlot>();
        if (slot == null) return;

        slot.ParentGO = gameObject;
        slot.UpdateSlot(index, HTBSGemParams[index]);
    }

    private void UpdateDetailListSlot(int index, GameObject slotObj)
    {
        UIHowToBeStrongListSlot slot = slotObj.GetComponent<UIHowToBeStrongListSlot>();
        if (slot == null) return;

        slot.ParentGO = gameObject;
        switch (_state)
        {
            case eSTATE.DETAIL_CHAR:
                slot.UpdateDetailSlot(index, HTBSCharacterParams[index]);
                break;
            case eSTATE.DETAIL_CARD:
                slot.UpdateDetailSlot(index, HTBSSupporterParams[index]);
                break;
            case eSTATE.DETAIL_WEAPON:
                slot.UpdateDetailSlot(index, HTBSWeaponParams[index]);
                break;
            case eSTATE.DETAIL_GEM: 
                slot.UpdateDetailSlot(index, HTBSGemParams[index]); 
                break;
        }
    }



    public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}    

    public override void InitComponent()
	{
        _chardata = null;
        //_weapondata = null;
        long cuid = (long)UIValue.Instance.GetValue(UIValue.EParamType.HowToBeStrongCUID);
        _chardata = GameInfo.Instance.GetCharData(cuid);

        SetState(eSTATE.MAIN, 0);        
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        if (_state == eSTATE.MAIN)
        {
            HelpCharacterList.UpdateList();
            HelpSupporterList.UpdateList();
            HelpWeaponList.UpdateList();
            HelpGemList.UpdateList();
            return;
        }   

        var data = GameInfo.Instance.GameClientTable.FindHowToBeStrong(x => x.Group == (int)_state && x.Index == _selete);
        if (data == null)
            return;

        var datalist = GameInfo.Instance.GameClientTable.FindAllHowToBeStrong(x => x.Group == (int)_state);
        if (datalist == null)
            return;

        kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(data.Name);
        kDetailLabel.textlocalize = FLocalizeString.Instance.GetText(data.Detail);
        kDescLabel.textlocalize = FLocalizeString.Instance.GetText(data.Desc);

        kHelpSpr.atlas = FLocalizeAtlas.Instance.GetUIAtlas(data.Atlas);        
        kHelpSpr.spriteName = data.Icon;        

        SetMoveButton(data);        

        kTitleLLabel.textlocalize = "";
        kTitleRLabel.textlocalize = "";
        int indexl = (int)_state - 1;
        int indexr = (int)_state + 1;
        if (indexl <= (int)eSTATE.MAIN)
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

        for (int i = 0; i < kHelpItemList.Count; i++)
        {   
            kHelpItemList[i].SetActive(false);
        }

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

        kHelpToggle.gameObject.SetActive(!(idlist.Count == 0));
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
    public void OnClick_TitleRBtn()
    {
        int index = (int)_state;
        index += 1;
        if (index >= (int)eSTATE.COUNT)
            index = 1;

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
	
    public void OnClick_MoveBtn()
    {
        if (_chardata == null)
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

        System.Action<ePANELTYPE> ActionShowUI = (ePanelType) =>
        {
            OnClickClose();
            LobbyUIManager.Instance.HideUI("MenuPopup");

            LobbyUIManager.Instance.SetPanelType(ePanelType);
            Lobby.Instance.MoveToLobby();
        };

        if (_state == eSTATE.DETAIL_CHAR)
        {
            if (_selete == (int)UICharInfoPanel.eCHARINFOTAB.SKILL)
                UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.SKILL);
            else
                UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.STATUS);

            ActionShowUI(ePANELTYPE.CHARINFO);
        }
        else if (_state == eSTATE.DETAIL_CARD)
        {
            if (_selete == 4)
            {
                //Enchant
                UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.SUPPORTER);
                ActionShowUI(ePANELTYPE.CHARINFO);
            }
            else
            {
                UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.SUPPORTER);
                ActionShowUI(ePANELTYPE.CHARINFO);
            }
        }
        else if (_state == eSTATE.DETAIL_WEAPON)
        {
            if (_selete == 4)
            {
                //Enchant
                UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.WEAPON);
                ActionShowUI(ePANELTYPE.CHARINFO);
            }
            else
            {
                UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.WEAPON);
                ActionShowUI(ePANELTYPE.CHARINFO);
            }
        }
        else if (_state == eSTATE.DETAIL_GEM)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.WEAPON);
            ActionShowUI(ePANELTYPE.CHARINFO);
        }

    }

    public void SetState(eSTATE e, int selete )
    {
        eSTATE olde = _state;
        _state = e;
        _selete = selete;

        bool activeState = _state == eSTATE.MAIN;

        kHelpMain.SetActive(activeState);
        kHelpDetail.SetActive(!activeState);
        kHelpToggle.OnClickToggle((int)eCOUNT.NONE + 1);
        kTitle.SetActive(!activeState);
        kBackBtn.gameObject.SetActive(!activeState);
        HelpDetailList.gameObject.SetActive(!activeState);

        Renewal(true);

        if (olde != e)
        {
            HelpDetailList.UpdateList();            
        }
        else
        {
            HelpDetailList.RefreshNotMove();
        }
    }

    private void SetMoveButton(GameClientTable.HowToBeStrong.Param param)
    {
        kMoveBtn.SetActive(false);

        if (GameSupport.IsHowToBeStrongNotice(_chardata, param))
        {
            kMoveBtn.SetActive(true);            
        }
   }
}
