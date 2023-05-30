using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBadgeInfoPopup : FComponent
{

	public UILabel kNameLabel;
	public UITexture kBadgeTex;
    public UILabel kBadgeLvLabel;
    public List<UIGaugeUnit> kBadgeOptGaugeUnitList;

	public UILabel kLevelupLabel;

    public UIButton kLevelUpBtn;
    public UIButton kLevelUpResetBtn;

    public FToggle kLockToggle;
    public UIButton kEquipBtn;

    public GameObject kAcquisition;
    public GameObject kAcquisitionTabBtn;

    public GameObject kInfo;
    public GameObject kInfoBtn;

    public UIAcquisitionInfoUnit kAcquisitionInfoUnit;

    private long _selectBadgeUID = 0;

    private BadgeData _badgeData;
    private GameTable.BadgeOpt.Param _badgeTableData;

    private bool _info = true;

    public override void Awake()
	{
		base.Awake();
        kLockToggle.EventCallBack = OnLockToggleSelect;
    }
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}

    public override void InitComponent()
	{
        _selectBadgeUID = (long)UIValue.Instance.GetValue(UIValue.EParamType.BadgeUID);

        if(_selectBadgeUID == (int)eCOUNT.NONE)
        {
            OnClickClose();
            return;
        }

        _badgeData = GameInfo.Instance.GetBadgeData(_selectBadgeUID);
        _badgeTableData = GameInfo.Instance.GameTable.FindBadgeOpt(x => x.OptionID == _badgeData.OptID[0]);

    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_badgeTableData.Name);
        kBadgeTex.mainTexture = GameSupport.GetBadgeIcon(_badgeData);
        kBadgeLvLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(282), _badgeData.Level.ToString());
        kLockToggle.gameObject.SetActive(true);
        if (_badgeData.Lock)
            kLockToggle.SetToggle(1, SelectEvent.Code);
        else
            kLockToggle.SetToggle(0, SelectEvent.Code);


        kEquipBtn.gameObject.SetActive(false);
        if (_badgeData.PosKind != (int)eContentsPosKind._NONE_)
            kEquipBtn.gameObject.SetActive(true);

        //강화 횟수 체크
        if (_badgeData.RemainLvCnt <= (int)eCOUNT.NONE || _badgeData.Level >= GameInfo.Instance.GameConfig.BadgeLvCnt)
        {
            kLevelUpBtn.isEnabled = false;
        }
        else
        {
            kLevelUpBtn.isEnabled = true;
        }

        //강화 초기화 가능 체크
        if(_badgeData.RemainLvCnt >= GameInfo.Instance.GameConfig.BadgeLvCnt)
        {
            kLevelUpResetBtn.isEnabled = false;
        }
        else
        {
            kLevelUpResetBtn.isEnabled = true;
        }

        string lvUpCnt = string.Format("{0} {1}", FLocalizeString.Instance.GetText(1463), string.Format(FLocalizeString.Instance.GetText(1443), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_MAX_TEXT_COLOR), _badgeData.RemainLvCnt)));
        kLevelupLabel.textlocalize = lvUpCnt;

        for(int i = 0; i < (int)eBadgeOptSlot._MAX_; i++)
        {
            GameTable.BadgeOpt.Param optData = GameInfo.Instance.GameTable.BadgeOpts.Find(x => x.OptionID == _badgeData.OptID[i]);
            if(optData == null)
            {
                kBadgeOptGaugeUnitList[i].InitGaugeUnit((float)eBadgeSlot.NONE);
                kBadgeOptGaugeUnitList[i].SetText(string.Empty);
            }
            else
            {
                //float fillAmount = (float)(_badgeData.OptVal[i] * 0.1f);
                float fillAmount = (float)(_badgeData.OptVal[i] / (float)GameInfo.Instance.GameConfig.BadgeMaxOptVal);
                Log.Show(_badgeData.OptVal[i] + " / " + fillAmount);
                kBadgeOptGaugeUnitList[i].InitGaugeUnit(fillAmount);

                if (fillAmount >= 1.0f)
                    kBadgeOptGaugeUnitList[i].SetColor((int)UIGemOptUnit.eGAUGESTATE.MAX);
                else
                    kBadgeOptGaugeUnitList[i].SetColor((int)UIGemOptUnit.eGAUGESTATE.NORMAL);

                float optValue = (float)((_badgeData.OptVal[i] + _badgeData.Level) * optData.IncEffectValue) / (float)eCOUNT.MAX_RATE_VALUE * 100.0f;

                string optDesc = string.Format(FLocalizeString.Instance.GetText(optData.Desc),
                        string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT, optValue))));

                if (i == (int)eBadgeOptSlot.FIRST)
                {
                    kBadgeOptGaugeUnitList[i].SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.YELLOW_TEXT_COLOR), optDesc));
                }
                else
                {
                    kBadgeOptGaugeUnitList[i].SetText(optDesc);
                }
            }
        }

        //문양 테이블에 AcquisitionID 추가되면 ItemInfoPopup참고해서 구현하기
        kAcquisitionInfoUnit.UpdateSlot(0);
        kAcquisitionTabBtn.gameObject.SetActive(false);
    }
	
	public void OnClick_BackBtn()
	{
        OnClickClose();
	}
	
	public void OnClick_InfoTabBtn()
	{
        Log.Show("OnClick_InfoTabBtn");
        SetToggleInfo(true);
    }
	
	public void OnClick_SellBtn()
	{
        if (_badgeData == null)
            return;

        string strBadge = FLocalizeString.Instance.GetText(1464);       //문양
        if(_badgeData.Lock == true)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3057, strBadge)); //  잠금 중인 {0}입니다.
            return;
        }

        if(_badgeData.PosKind != (int)eContentsPosKind._NONE_)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3025, strBadge)); //  장착 중인 {0}입니다.
            return;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.SellSingleType, UISellSinglePopup.eSELLTYPE.BADGE);
        UIValue.Instance.SetValue(UIValue.EParamType.SellSingleUID, _badgeData.BadgeUID);

        LobbyUIManager.Instance.ShowUI("SellSinglePopup", true);
    }
	
	public void OnClick_AcquisitionTabBtn()
	{
        Log.Show("OnClick_AcquisitionTabBtn");
        SetToggleInfo(false);
    }

    private void SetToggleInfo(bool info)
    {
        _info = info;
        if(_info)
        {
            kInfo.SetActive(true);
            kAcquisition.SetActive(false);
        }
        else
        {
            kInfo.SetActive(false);
            kAcquisition.SetActive(true);
        }
    }
	
    public void OnClick_EquipBtn()
    {
        Log.Show("OnClick_EquipBtn");
        if (_badgeData == null)
            return;

        if(_badgeData.PosKind == (int)eContentsPosKind.ARENA)
        {
            
            //OnClickClose();
            //LobbyUIManager.Instance.PanelBGAllHide();
            //LobbyUIManager.Instance.HideUI("ItemPanel");
        }
    }

	public void OnClick_LevelUpBtn()
	{
        LobbyUIManager.Instance.ShowUI("BadgeLevelUpPopup", true);
	}
	
	public void OnClick_LevelUpResetBtn()
	{
        LobbyUIManager.Instance.ShowUI("BadgeOptionResetPopup", true);
    }

    private bool OnLockToggleSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Click)
        {
            List<long> uidlist = new List<long>();
            List<bool> locklist = new List<bool>();

            uidlist.Add(_badgeData.BadgeUID);
            if (nSelect == 0)
                locklist.Add(false);
            else
                locklist.Add(true);

            GameInfo.Instance.Send_ReqSetLockBadge(uidlist, locklist, OnNetGemLock);
        }
        return true;
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

}
