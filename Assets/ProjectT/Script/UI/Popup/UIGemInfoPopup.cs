using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGemInfoPopup : FComponent
{
    public enum eGemInfoType
    {
        Info,
        Get,
        Set,
        _MAX_,
    }

    public GameObject kInfo;
    public GameObject kAcquisition;
    public GameObject kSelBtn;
    public UIButton kInfoTabBtn;
    public UIButton kAcquisitionTabBtn;
    public UIAcquisitionInfoUnit kAcquisitionInfoUnit;
    public UIButton kBackBtn;
    public UITexture kGemTex;
    public UILabel kNameLabel;
    public UILabel kLevelLabel;
    public UISprite kWakeSpr;
    public UISprite kGradeSpr;
    public UIGaugeUnit kExpGaugeUnit;
	public UIStatusUnit kGemStatusUnit_00;
	public UIStatusUnit kGemStatusUnit_01;
	public FToggle kLockToggle;
	public UIButton kEquipBtn;
	public UITexture kEquipImageTex;
	public UIButton kSellBtn;
	public UIButton kLevelUpBtn;
	public UIButton kGradeUpBtn;
	public UIButton kOptChangeBtn;
    public GameObject kOpt;
    public List<UIGemOptUnit> kGemOptList;
    public UIButton kArrow_LBtn;
    public UIButton kArrow_RBtn;
    public GameObject kGemInfoTypeTabRoot;
    public FTab kGemInfoTypeTab;

    [Header("Add Gem UR Grade")]
    [SerializeField] private UIButton gemUpBtn = null;
    [SerializeField] private UIButton setBtn = null;
    [SerializeField] private UILabel setLabel = null;
    [SerializeField] private UISprite gemTypeSpr = null;
    [SerializeField] private UIButton questionBtn = null;
    [SerializeField] private GameObject identifyObj = null;
    [SerializeField] private UIGrid tabGrid = null;

    private int _itemlistindex = -1;
    
    private GemData _gemdata;
    private GameTable.Gem.Param _gemtabledata;

    private eGemInfoType _gemInfoType = eGemInfoType.Info;

    public override void Awake()
	{
		base.Awake();

		kLockToggle.EventCallBack = OnLockToggleSelect;
        kGemInfoTypeTab.EventCallBack = OnGemInfoTypeTabSelect;

    }
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        _itemlistindex = -1;
        long gemuid = (long)UIValue.Instance.GetValue(UIValue.EParamType.GemUID);
        if (gemuid != -1)
        {
            _gemdata = GameInfo.Instance.GetGemData(gemuid);
            _gemtabledata = _gemdata.TableData;

            UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
            if (itempanel != null)
            {
                for (int i = 0; i < itempanel.GemList.Count; i++)
                {
                    if (itempanel.GemList[i] != null)
                    {
                        if (_gemdata.GemUID == itempanel.GemList[i].GemUID)
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
            _gemdata = null;
            _gemtabledata = GameInfo.Instance.GameTable.FindGem((int)UIValue.Instance.GetValue(UIValue.EParamType.GemTableID));
        }

        _gemInfoType = eGemInfoType.Info;
        kGemInfoTypeTab.SetTab((int)_gemInfoType, SelectEvent.Code);
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        if (_gemdata == null && _gemtabledata == null)
            return;

        questionBtn.SetActive( false );
        gemTypeSpr.SetActive( false );
        setBtn.SetActive( false );
        gemUpBtn.SetActive( false );

        int level = 1;
        int wake = 0;
        float fillAmount = 0.0f;
        if (_gemdata != null)
        {
            level = _gemdata.Level;
            wake = _gemdata.Wake;

            fillAmount = GameSupport.GetGemLevelExpGauge(_gemdata, _gemdata.Level, _gemdata.Exp);
            /*
            for (int i = 0; i < (int)eCOUNT.GEMRANDOPT; i++)
            {
                var optdata = GameInfo.Instance.GameTable.FindGemRandOpt(x => x.GroupID == _gemdata.TableData.RandOptGroup && x.ID == _gemdata.RandOptID[i]);
                if (optdata != null)
                {
                    int value = optdata.Min + (int)((float)_gemdata.RandOptValue[i] * optdata.Value);
                    kOptList[i].SetActive(true);
                    kOptStartLabelList[i].textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONE_POINT_TEXT), optdata.Min / (float)eCOUNT.MAX_RATE_VALUE * 100.0f);
                    kOptEndLabelList[i].textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONE_POINT_TEXT), optdata.Max / (float)eCOUNT.MAX_RATE_VALUE * 100.0f);
                    kOptTextLabelList[i].textlocalize = string.Format(FLocalizeString.Instance.GetText(optdata.Desc), value / (float)eCOUNT.MAX_RATE_VALUE * 100.0f);

                    float f = (float)_gemdata.RandOptValue[i] / (float)optdata.RndStep;
                    kOptGaugeUnitList[i].InitGaugeUnit(f);
                }
                else
                {
                    kOptList[i].SetActive(false);
                }
            }
            */

            bool isEnableSet = _gemtabledata.Grade == GameInfo.Instance.GameConfig.GemSetGrade;
            bool isHaveSetOpt = _gemdata.SetOptID != 0;

            gemTypeSpr.SetActive(isEnableSet && isHaveSetOpt);
            questionBtn.SetActive(isEnableSet && !isHaveSetOpt);
            setBtn.SetActive(isEnableSet);

            GameTable.GemSetType.Param gemSetTypeParam = GameInfo.Instance.GameTable.FindGemSetType(_gemdata.SetOptID);
            if (gemSetTypeParam != null)
            {
                gemTypeSpr.spriteName = gemSetTypeParam.Icon;
            }

            if (_gemInfoType == eGemInfoType.Info)
            {
                kLockToggle.gameObject.SetActive(true);
                if (_gemdata.Lock)
                {
                    kLockToggle.SetToggle(1, SelectEvent.Code);
                }
                else
                {
                    kLockToggle.SetToggle(0, SelectEvent.Code);
                }

                WeaponData weapondata = GameInfo.Instance.GetEquipGemWeaponData(_gemdata.GemUID);
                if (weapondata == null)
                {
                    kEquipBtn.gameObject.SetActive(false);
                }
                else
                {
                    kEquipBtn.gameObject.SetActive(true);
                    kEquipImageTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_" + weapondata.TableData.Icon);
                }

                kOpt.SetActive(true);
                for (int i = 0; i < kGemOptList.Count; i++)
                {
                    kGemOptList[i].gameObject.SetActive(true);
                    kGemOptList[i].Lock();
                }

                for (int i = 0; i < _gemdata.Wake; i++)
                    kGemOptList[i].Opt(_gemdata, i);
            }
            else if (_gemInfoType == eGemInfoType.Set)
            {
                setLabel.SetActive(isEnableSet && isHaveSetOpt);
                identifyObj.SetActive(isEnableSet && !isHaveSetOpt);

                if (isEnableSet && isHaveSetOpt)
                {
                    System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

                    if (gemSetTypeParam != null)
                    {
                        string titleStr = string.Empty;

                        List<GameClientTable.GemSetOpt.Param> gemSetOptParamList = GameInfo.Instance.GameClientTable.FindAllGemSetOpt(x => x.GroupID == gemSetTypeParam.ID);
                        foreach (GameClientTable.GemSetOpt.Param gemSetOptParam in gemSetOptParamList)
                        {
                            if (string.IsNullOrEmpty(titleStr))
                            {
                                titleStr = FLocalizeString.Instance.GetText(3312, FLocalizeString.Instance.GetText(gemSetOptParam.Name));
                            }

                            stringBuilder.Append(FLocalizeString.Instance.GetText(3313, gemSetOptParam.SetCount)).Append("\n");

                            GameClientTable.BattleOptionSet.Param battleOptionSetParam = GameInfo.Instance.GameClientTable.FindBattleOptionSet(gemSetOptParam.GemBOSetID1);
                            if (battleOptionSetParam != null)
                            {
                                stringBuilder.Append(FLocalizeString.Instance.GetText(gemSetOptParam.Desc, battleOptionSetParam.BOFuncValue)).Append("\n\n");
                            }
                        }

                        stringBuilder.Insert(0, titleStr).Insert(titleStr.Length, "\n\n");
                    }
                    setLabel.textlocalize = stringBuilder.ToString();
                }
            }

            if (GameSupport.IsMaxLevelGem(_gemdata))
            {
                kLevelUpBtn.gameObject.SetActive(false);

                if (GameSupport.IsMaxWakeGem(_gemdata))
                {
                    kGradeUpBtn.isEnabled = false;
                }
                else
                {
                    kGradeUpBtn.isEnabled = true;
                }

                bool isEvolution = _gemtabledata.Grade == (int)eGRADE.GRADE_SR && kGradeUpBtn.isEnabled == false && _gemtabledata.EvReqGroup != 0;
                kGradeUpBtn.SetActive(!isEvolution);
                gemUpBtn.SetActive(isEvolution);
            }
            else
            {
                kLevelUpBtn.gameObject.SetActive(true);
                kGradeUpBtn.gameObject.SetActive(false);
                gemUpBtn.SetActive(false);
            }

            if (_gemdata.Wake == 0)
                kOptChangeBtn.isEnabled = false;
            else
                kOptChangeBtn.isEnabled = true;

            kOptChangeBtn.gameObject.SetActive(true);

            kArrow_LBtn.gameObject.SetActive(false);
            kArrow_RBtn.gameObject.SetActive(false);

            UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
            if(itempanel != null )
            {
                if(GameInfo.Instance.GemList.Count > 1 )
                {
                    kArrow_LBtn.gameObject.SetActive(true);
                    kArrow_RBtn.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            kArrow_LBtn.gameObject.SetActive(false);
            kArrow_RBtn.gameObject.SetActive(false);
            kLockToggle.gameObject.SetActive(false);
            kEquipBtn.gameObject.SetActive(false);
            kOpt.SetActive(false);

            kLevelUpBtn.gameObject.SetActive(false);
            kGradeUpBtn.gameObject.SetActive(false);
            kOptChangeBtn.gameObject.SetActive(false);

            for (int i = 0; i < kGemOptList.Count; i++)
                kGemOptList[i].gameObject.SetActive(false);
        }

        kGemTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + _gemtabledata.Icon);
        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_gemtabledata.Name);
        kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, level, GameSupport.GetGemMaxLevel());
        kWakeSpr.spriteName = "itemGemwake_0" + wake.ToString();
        kGradeSpr.spriteName = "itemgrade_L_" + _gemtabledata.Grade.ToString();
        //kGradeSpr.MakePixelPerfect();

        kAcquisitionInfoUnit.UpdateSlot(_gemtabledata.AcquisitionID);
        kAcquisitionTabBtn.SetActive(_gemtabledata.AcquisitionID > 0);

        int statusmain = GameSupport.GetTypeStatusGem(_gemtabledata.MainType, level, wake, _gemtabledata);
        int statussub = GameSupport.GetTypeStatusGem(_gemtabledata.SubType, level, wake, _gemtabledata);
        kGemStatusUnit_00.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + _gemtabledata.MainType, statusmain);
        if (_gemtabledata.MainType == _gemtabledata.SubType)
        {
            kGemStatusUnit_01.gameObject.SetActive(false);
        }
        else
        {
            kGemStatusUnit_01.gameObject.SetActive(true);
            kGemStatusUnit_01.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + _gemtabledata.SubType, statussub);
        }

        kExpGaugeUnit.InitGaugeUnit(fillAmount);
        kExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (fillAmount * 100.0f)));

        kGemInfoTypeTabRoot.SetActive(kAcquisitionTabBtn.gameObject.activeSelf || setBtn.gameObject.activeSelf);
        tabGrid.Reposition();
    }

    public void ReSetTableData()
    {
        _gemtabledata = _gemdata.TableData;
    }

    private bool OnLockToggleSelect(int nSelect, SelectEvent type)
	{
        if (type == SelectEvent.Click)
        {
            List<long> uidlist = new List<long>();
            List<bool> locklist = new List<bool>();

            uidlist.Add(_gemdata.GemUID);
            if (nSelect == 0)
                locklist.Add(false);
            else
                locklist.Add(true);

            GameInfo.Instance.Send_ReqSetLockGemList(uidlist, locklist, OnNetGemLock);
        }
        return true;
	}
	
	public void OnClick_BackBtn()
	{
        OnClickClose();
	}

    public override void OnClickClose()
    {
        LobbyUIManager.Instance.InitComponent("WeaponGemSeletePopup");
        base.OnClickClose();
    }

    public void OnClick_EquipBtn()
	{
        WeaponData weapondata = GameInfo.Instance.GetEquipGemWeaponData(_gemdata.GemUID);
        if (weapondata == null)
            return;

        OnClickClose();

        UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, weapondata.WeaponUID);
        LobbyUIManager.Instance.ShowUI("WeaponInfoPopup", true);
    }
	
	public void OnClick_SellBtn()
	{
        if (_gemdata == null)
            return;

        string strGem = FLocalizeString.Instance.GetText(1070); //  곡옥
        //  잠금 상태거나 장착 상태의 경우
        if (_gemdata.Lock == true)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3057, strGem)); //  잠금 중인 {0}입니다.
            return;
        }

        if (GameInfo.Instance.GetEquipGemWeaponData(_gemdata.GemUID) != null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3025, strGem)); //  장착 중인 {0}입니다.
            return;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.SellSingleType, UISellSinglePopup.eSELLTYPE.GEM);
        UIValue.Instance.SetValue(UIValue.EParamType.SellSingleUID, _gemdata.GemUID);
        LobbyUIManager.Instance.ShowUI("SellSinglePopup", true);
    }

	public void OnClick_LevelUpBtn()
	{
        if (GameSupport.IsMaxLevelGem(_gemdata))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3007));
            return;
        }

        LobbyUIManager.Instance.ShowUI("GemLevelUpPopup", true);
    }
	
	public void OnClick_GradeUpBtn()
	{
        if (GameSupport.IsMaxWakeGem(_gemdata))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3007));
            return;
        }

        LobbyUIManager.Instance.ShowUI("GemGradeUpPopup", true);
    }
	
	public void OnClick_OptChangeBtn()
	{
        LobbyUIManager.Instance.ShowUI("GemOptChangePopup", true);
    }

    public void OnClick_Arrow_LBtn()
    {
        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel == null)
            return;

        int temp = _itemlistindex;
        temp -= 1;
        if (temp < 0)
            temp = GameInfo.Instance.GemList.Count - 1;

        if (0 <= temp && itempanel.GemList.Count > temp)
        {
            var data = itempanel.GemList[temp];
            if (data != null)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.GemUID, data.GemUID);
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
        if (temp >= GameInfo.Instance.GemList.Count)
            temp = 0;

        if (0 <= temp && itempanel.GemList.Count > temp)
        {
            var data = itempanel.GemList[temp];
            if (data != null)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.GemUID, data.GemUID);
                InitComponent();
                Renewal(true);
            }
        }
    }

    public void OnClick_IdentifyBtn()
    {
        MessagePopup.YNGemAnalyzed(FLocalizeString.Instance.GetText(3309), yesCallback: GemAnalyzedCallback);
        MessagePopup.ShowMessagePopup();
    }

    public void OnClick_GemUpBtn()
    {
        if (_gemdata.TableData.EvReqGroup <= 0)
        {
            return;
        }

        LobbyUIManager.Instance.ShowUI("GemGradeUpPopup", true);
    }

    public void OnNetGemLock(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3050));

        Renewal(true);
        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
            itempanel.RefreshList();
    }

    private bool OnGemInfoTypeTabSelect(int nSelect, SelectEvent type)
    {
        ChangeTabInfo(nSelect);

        if (type == SelectEvent.Click)
        {
            Renewal(false);
        }

        return true;
    }

    private void OnNetGemAnalyzed(int result, PktMsgType pktMsgType)
    {
        if (_gemInfoType != eGemInfoType.Set)
        {
            kGemInfoTypeTab.SetTab((int)eGemInfoType.Set, SelectEvent.Code);
        }

        SoundManager.Instance.PlayUISnd(16);

        ReSetTableData();
        Renewal(false);

        UIItemPanel itemPanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itemPanel != null)
        {
            itemPanel.RefreshList();
            itemPanel.Renewal(false);
        }

        LobbyUIManager.Instance.Renewal("CharInfoPanel");

        string spriteName = string.Empty;
        GameTable.GemSetType.Param gemSetTypeParam = GameInfo.Instance.GameTable.FindGemSetType(_gemdata.SetOptID);
        if (gemSetTypeParam != null)
        {
            spriteName = gemSetTypeParam.Icon;
        }

        MessagePopup.YNGemAnalyzed(FLocalizeString.Instance.GetText(3309), gemSpriteName: spriteName);
        MessagePopup.ShowMessagePopup();
    }

    private void GemAnalyzedCallback()
    {
        int costIndex = GameInfo.Instance.GameConfig.GemAnalyzeCostIndex;
        GameTable.Item.Param itemTableParam = GameInfo.Instance.GameTable.FindItem(costIndex);

        int haveItemCount = 0;
        ItemData itemData = GameInfo.Instance.ItemList.Find(x => x.TableID == costIndex);
        if (itemData != null)
        {
            haveItemCount = itemData.Count;
        }

        if (haveItemCount < GameInfo.Instance.GameConfig.GemAnalyzeCostValue)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3042));
            return;
        }

        GameInfo.Instance.Send_ReqAnalyzeGem(_gemdata.GemUID, OnNetGemAnalyzed);
    }

    private void ChangeTabInfo(int selectIdx)
    {
        _gemInfoType = (eGemInfoType)selectIdx;

        kInfo.SetActive(_gemInfoType != eGemInfoType.Get);
        kAcquisition.SetActive(_gemInfoType == eGemInfoType.Get);

        // Info
        bool isActive = ( _gemInfoType == eGemInfoType.Info ) && ( _gemdata != null );
        kOpt.SetActive(isActive);
        kLockToggle.gameObject.SetActive(isActive);
        kEquipBtn.SetActive(isActive);
        kSellBtn.SetActive(isActive);

        // Set
        isActive = _gemInfoType == eGemInfoType.Set;
        setLabel.SetActive(isActive);
        identifyObj.SetActive(isActive);
    }
}
