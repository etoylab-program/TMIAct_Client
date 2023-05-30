using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBadgeOptionResetPopup : FComponent
{

	public UILabel kLevelupLabel;
	public UITexture kBatchTex;

	public UILabel kAfterLevelupLabel;
	public UITexture kAfterBatchTex;

    public List<UIGaugeUnit> kBeforeOptUnitList;
    public List<UIGaugeUnit> kAfterOptUnitList;

    public List<UIItemListSlot> kUseItemSlotList;
    public UIGoodsUnit kGoldGoodsUnit;

    public GameObject kFadeEffObj;
    public TweenAlpha kResetFadeEff;
    public float kTweenAlphaFadeTime;
    public AnimationCurve kResetFadeEffAniCurve;

    public UILabel kCurLevelLabel;
    public UILabel kAfterLevelLabel;

    private long _selectBadgeUID = 0;
    private BadgeData _badgeData;

    private GameTable.BadgeOpt.Param _badgeTableData;

    private bool _useGold = false;
    private bool _useMatItem = false;

    private bool _bSendReset = false;

 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        _bSendReset = false;

        kFadeEffObj.SetActive(false);

        _selectBadgeUID = (long)UIValue.Instance.GetValue(UIValue.EParamType.BadgeUID);
        _badgeData = GameInfo.Instance.GetBadgeData(_selectBadgeUID);
        _badgeTableData = GameInfo.Instance.GameTable.FindBadgeOpt(x => x.OptionID == _badgeData.OptID[0]);
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        kCurLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(282), _badgeData.Level);
        kAfterLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(282), 0);

        SetBadgeInfo(kBatchTex, kBeforeOptUnitList, kLevelupLabel, _badgeData.RemainLvCnt);
        SetBadgeInfo(kAfterBatchTex, kAfterOptUnitList, kAfterLevelupLabel, GameInfo.Instance.GameConfig.BadgeLvCnt, true);
        UseItemCheck();

    }

    void SetBadgeInfo(UITexture texIcon, List<UIGaugeUnit> gaugeUnitList, UILabel lvCntLb, int lvRemainCnt, bool bResetFlag = false)
    {
        texIcon.mainTexture = GameSupport.GetBadgeIcon(_badgeData);

        for (int i = 0; i < (int)eBadgeOptSlot._MAX_; i++)
        {
            GameTable.BadgeOpt.Param optData = GameInfo.Instance.GameTable.BadgeOpts.Find(x => x.OptionID == _badgeData.OptID[i]);
            if (optData == null)
            {
                gaugeUnitList[i].InitGaugeUnit((float)eBadgeSlot.NONE);
                gaugeUnitList[i].SetText(string.Empty);
            }
            else
            {
                float fillAmount = (float)(_badgeData.OptVal[i] / (float)GameInfo.Instance.GameConfig.BadgeMaxOptVal);
                gaugeUnitList[i].InitGaugeUnit(fillAmount);

                if (fillAmount >= 1.0f)
                    gaugeUnitList[i].SetColor((int)UIGemOptUnit.eGAUGESTATE.MAX);
                else
                    gaugeUnitList[i].SetColor((int)UIGemOptUnit.eGAUGESTATE.NORMAL);

                if(bResetFlag)
                {
                    float optValue = ((_badgeData.OptVal[i]) * optData.IncEffectValue) / (float)eCOUNT.MAX_RATE_VALUE * 100.0f;
                    
                    string optDesc = string.Format(FLocalizeString.Instance.GetText(optData.Desc),
                        string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT), optValue)));

                    if (i == (int)eBadgeOptSlot.FIRST)
                    {
                        gaugeUnitList[i].SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.YELLOW_TEXT_COLOR), optDesc));
                    }
                    else
                    {
                        gaugeUnitList[i].SetText(optDesc);
                    }
                }
                else
                {
                    float optValue = ((_badgeData.OptVal[i] + _badgeData.Level) * optData.IncEffectValue) / (float)eCOUNT.MAX_RATE_VALUE * 100.0f;
                    
                    string optDesc = string.Format(FLocalizeString.Instance.GetText(optData.Desc),
                        string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT, optValue))));

                    if (i == (int)eBadgeOptSlot.FIRST)
                    {
                        gaugeUnitList[i].SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.YELLOW_TEXT_COLOR), optDesc));
                    }
                    else
                    {
                        gaugeUnitList[i].SetText(optDesc);
                    }
                }
            }
        }

        string lvUpCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
        

        if (bResetFlag)
        {
            lvUpCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_MAX_TEXT_COLOR);
        }

        string lvUpCnt = string.Format("{0} {1}", FLocalizeString.Instance.GetText(1463), string.Format(FLocalizeString.Instance.GetText(1443), string.Format(lvUpCntColor, lvRemainCnt)));
        lvCntLb.textlocalize = lvUpCnt;
    }

    void UseItemCheck()
    {
        _useGold = true;
        _useMatItem = true;

        List<GameTable.Item.Param> itemList = GameInfo.Instance.GameTable.FindAllItem(x => x.Type == (int)eITEMTYPE.MATERIAL && x.SubType == (int)eITEMSUBTYPE.MATERIAL_BADGE_INIT_LV);

        for (int i = 0; i < kUseItemSlotList.Count; i++)
            kUseItemSlotList[i].gameObject.SetActive(false);

        for(int i = 0; i < itemList.Count; i++)
        {
            Log.Show("Mat ItemTID : " + itemList[i].ID);
            kUseItemSlotList[i].gameObject.SetActive(true);
            kUseItemSlotList[i].UpdateSlot(UIItemListSlot.ePosType.Mat, i, itemList[i]);

            int orgcut = GameInfo.Instance.GetItemIDCount(itemList[i].ID);
            //int orgmax = GameInfo.Instance.GameConfig.BadgeLvUpMatCntByLv[_badgeData.Level];
            int orgmax = GameInfo.Instance.GameConfig.BadgeLvInitMatCnt; //재료 1개 고정
            string strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
            if (orgcut < orgmax)
            {
                strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);
                _useMatItem = false;
            }
            string strmatcount = string.Format(strHaveCntColor, string.Format(FLocalizeString.Instance.GetText(236), orgcut, orgmax));
            kUseItemSlotList[i].SetCountLabel(strmatcount);
        }
        int lvCheck = _badgeData.Level -1;
        if (lvCheck < (int)eCOUNT.NONE)
            lvCheck = _badgeData.Level;

        long needGold = kGoldGoodsUnit.MyAbs(GameInfo.Instance.GameConfig.BadgeLvInitCost);

        _useGold = GameInfo.Instance.UserData.IsGoods(eGOODSTYPE.GOLD, needGold);

        kGoldGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, GameInfo.Instance.GameConfig.BadgeLvInitCost, true);
    }

	
	public void OnClick_BackBtn()
	{
        if (_bSendReset)
            return;

        OnClickClose();
	}
	
	public void OnClick_UpgradeBtn()
	{
        if (_bSendReset)
            return;

        if (!_useMatItem)
        {
            //소재가 부족합니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3003));
            return;
        }

        if (!_useGold)
        {
            //골드가 부족합니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(100));
            return;
        }

        _bSendReset = true;
        GameInfo.Instance.Send_ReqResetUpgradeBadge(_badgeData.BadgeUID, OnNetAckResetUpgradeBadge);
    }

    public void OnNetAckResetUpgradeBadge(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        StartCoroutine(BadgeOptionResetCoroutine(pktmsg));
    }

    IEnumerator BadgeOptionResetCoroutine(PktMsgType pktmsg)
    {
        kFadeEffObj.SetActive(true);
        SoundManager.Instance.PlayUISnd(71);
        TweenAlpha.SetTweenAlpha(kResetFadeEff, kTweenAlphaFadeTime, 0f, 1f, kResetFadeEffAniCurve);
        yield return new WaitForSeconds(kTweenAlphaFadeTime);
        TweenAlpha.SetTweenAlpha(kResetFadeEff, kTweenAlphaFadeTime, 1f, 0f, kResetFadeEffAniCurve);
        Renewal(true);
        yield return new WaitForSeconds(kTweenAlphaFadeTime);
        //SoundManager.Instance.PlayUISnd(72);  //MessageToastPopup OpenSound로 대체
        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3163));
        yield return new WaitForSeconds(kTweenAlphaFadeTime);
        OnClickClose();
    }

    public override void OnClickClose()
    {
        UIBadgeInfoPopup uIBadgeInfoPopup = LobbyUIManager.Instance.GetActiveUI<UIBadgeInfoPopup>("BadgeInfoPopup");
        if (uIBadgeInfoPopup != null)
        {
            uIBadgeInfoPopup.Renewal(true);
        }

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");

        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
        {
            itempanel.InitComponent();
            itempanel.Renewal(true);
        }

        UIBadgeSelectPopup badgeSelectPopup = LobbyUIManager.Instance.GetActiveUI<UIBadgeSelectPopup>("BadgeSelectPopup");
        if (badgeSelectPopup != null)
        {
            badgeSelectPopup.InitComponent();
            badgeSelectPopup.Renewal(true);
        }

        base.OnClickClose();

    }

}
