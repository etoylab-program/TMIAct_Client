using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBadgeLevelUpPopup : FComponent
{

	public UILabel kBeforeLvLabel;                          //���� ����
	public UILabel kAfterLvLabel;                           //���� �� ����
	public UITexture kEFFTex;                               //
	public UILabel kSuccessLabel;                           //����Ȯ��
	public UILabel kLevelupLabel;                           //���� ��ȭ Ƚ��
	public UITexture kBatchTex;                             //���� ������
    public UILabel kBadgeCurLevelLabel;                     //���� ������ ����
	public UILabel kBatchNameLabel;                         //���� �̸�

    public List<UIGaugeUnit> kBadgeOptGaugeUnitList;        //���� ������ �ɼ� ǥ��
    public List<UIItemListSlot> kUseItemSlotList;           //���� ������ ǥ��

	public UIGoodsUnit kGoldGoodsUnit;                      //���� ��ȭ ǥ��
    public UIButton kBadgeLevelBtn;

    [Header("LevelUpEffects")]
    public GameObject kLevelUpSuccessEff;                   //������ ���� ����Ʈ
    public GameObject kLevelUpSuccessAfterEff;              //������ ���� �� ��¦�̴� ����Ʈ
    public GameObject kLevelUpFailedEff;                    //������ ���� ����Ʈ

    public GameObject kLevelUpResultType;                   //������ ����/���� ����
    public GameObject kLevelUpResultSuccess;                //������ ���� �۾� �ִ�
    public GameObject kLevelUpResultFailed;                 //������ ���� �۾� �ִ�

    private long _selectBadgeUID = 0;                       //���� ������ ������ UID
    private BadgeData _badgeData;                           //���� ������ ����

    private GameTable.BadgeOpt.Param _badgeTableData;       //���� �ɼ�

    private bool _useGold = false;                          //���� ��尡 �������� üũ
    private bool _useMatItem = false;                       //���� �������� �������� üũ

    private bool _bSendLevelUp = false;                     //������ ���������� üũ
    public bool IsSendLevelUp { get { return _bSendLevelUp; } }
    private int _badgeCurLv;                                //���� ����
    private int _badgeCurLvCnt;                             //���� ��ȭ ���� Ƚ��


	public override void OnEnable()
	{
        DisableEffects();


        InitComponent();
		base.OnEnable();
	}

    public override void InitComponent()
	{
        _selectBadgeUID = (long)UIValue.Instance.GetValue(UIValue.EParamType.BadgeUID);
        _badgeData = GameInfo.Instance.GetBadgeData(_selectBadgeUID);
        _badgeTableData = GameInfo.Instance.GameTable.FindBadgeOpt(x => x.OptionID == _badgeData.OptID[0]);
        _bSendLevelUp = false;
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        SetBadgeInfo();
        UseItemCheck();
    }

    void DisableEffects()
    {
        kLevelUpSuccessEff.SetActive(false);
        kLevelUpSuccessAfterEff.SetActive(false);
        if (kLevelUpFailedEff != null)
            kLevelUpFailedEff.SetActive(false);

        kLevelUpResultType.SetActive(false);
        kLevelUpResultSuccess.SetActive(false);
        kLevelUpResultFailed.SetActive(false);
    }
 
    void SetBadgeInfo()
    {
        kBatchNameLabel.textlocalize = FLocalizeString.Instance.GetText(_badgeTableData.Name);
        kBatchTex.mainTexture = GameSupport.GetBadgeIcon(_badgeData);

        if (_badgeData.RemainLvCnt <= 0)
            kBadgeLevelBtn.isEnabled = false;
        else
            kBadgeLevelBtn.isEnabled = true;

        for (int i = 0; i < (int)eBadgeOptSlot._MAX_; i++)
        {
            GameTable.BadgeOpt.Param optData = GameInfo.Instance.GameTable.BadgeOpts.Find(x => x.OptionID == _badgeData.OptID[i]);
            if (optData == null)
            {
                kBadgeOptGaugeUnitList[i].InitGaugeUnit((float)eBadgeSlot.NONE);
                kBadgeOptGaugeUnitList[i].SetText(string.Empty);
            }
            else
            {
                float fillAmount = (float)(_badgeData.OptVal[i] / (float)GameInfo.Instance.GameConfig.BadgeMaxOptVal);
                kBadgeOptGaugeUnitList[i].InitGaugeUnit(fillAmount);

                if (fillAmount >= 1.0f)
                    kBadgeOptGaugeUnitList[i].SetColor((int)UIGemOptUnit.eGAUGESTATE.MAX);
                else
                    kBadgeOptGaugeUnitList[i].SetColor((int)UIGemOptUnit.eGAUGESTATE.NORMAL);

                float optValue = ((_badgeData.OptVal[i] + _badgeData.Level) * optData.IncEffectValue) / (float)eCOUNT.MAX_RATE_VALUE * 100.0f;

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

        kBeforeLvLabel.textlocalize = string.Empty;
        kAfterLvLabel.textlocalize = string.Empty;

        int nowLv = _badgeData.Level;
        int nextLv = _badgeData.Level + 1;

        if (nextLv >= GameInfo.Instance.GameConfig.BadgeLvCnt)
            nextLv = GameInfo.Instance.GameConfig.BadgeLvCnt;

        kBeforeLvLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(282), nowLv.ToString());
        kAfterLvLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(282), nextLv.ToString());
    }

    /// <summary>
    /// �ʿ� ������ �˻�
    /// </summary>
    void UseItemCheck()
    {
        _useGold = true;
        _useMatItem = true;

        int lvCheck = _badgeData.Level;
        if (_badgeData.Level >= GameInfo.Instance.GameConfig.BadgeLvCnt)
            lvCheck = GameInfo.Instance.GameConfig.BadgeLvCnt - 1;
        
        List<GameTable.Item.Param> itemList = GameInfo.Instance.GameTable.FindAllItem(x => x.Type == (int)eITEMTYPE.MATERIAL && x.SubType == (int)eITEMSUBTYPE.MATERIAL_BADGE_LEVELUP);

        for (int i = 0; i < kUseItemSlotList.Count; i++)
            kUseItemSlotList[i].gameObject.SetActive(false);

        for(int i = 0; i < itemList.Count; i++)
        {
            kUseItemSlotList[i].gameObject.SetActive(true);
            kUseItemSlotList[i].UpdateSlot(UIItemListSlot.ePosType.Mat, i, itemList[i]);
            Log.Show(itemList[i].ID);
            int orgcut = GameInfo.Instance.GetItemIDCount(itemList[i].ID);
            int orgmax = GameInfo.Instance.GameConfig.BadgeLvUpMatCntByLv[lvCheck];
            string strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
            if (orgcut < orgmax)
            {
                strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);
                _useMatItem = false;
            }
            string strmatcount = string.Format(strHaveCntColor, string.Format(FLocalizeString.Instance.GetText(236), orgcut, orgmax));
            kUseItemSlotList[i].SetCountLabel(strmatcount);
        }

        long needGold = kGoldGoodsUnit.MyAbs(GameInfo.Instance.GameConfig.BadgeLvUpCostByLv[lvCheck]);

        _useGold = GameInfo.Instance.UserData.IsGoods(eGOODSTYPE.GOLD, needGold);

        kGoldGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, GameInfo.Instance.GameConfig.BadgeLvUpCostByLv[lvCheck], true);

        string lvUpCnt = string.Format("{0} {1}", FLocalizeString.Instance.GetText(1463), string.Format(FLocalizeString.Instance.GetText(1443), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_MAX_TEXT_COLOR), _badgeData.RemainLvCnt)));
        kLevelupLabel.textlocalize = lvUpCnt;

        kSuccessLabel.textlocalize = string.Format("{0}{1}", FLocalizeString.Instance.GetText(1466), 
            string.Format(FLocalizeString.Instance.GetText(229), 
            string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_MAX_TEXT_COLOR), GameInfo.Instance.GameConfig.BadgeLvUpRateByLv[lvCheck])));
    }
	
	public void OnClick_BackBtn()
	{
        if (_bSendLevelUp)
            return;
        OnClickClose();
	}
	
	public void OnClick_UpgradeBtn()
	{
        if (_bSendLevelUp)
            return;
        if (!_useMatItem)
        {
            //���簡 �����մϴ�.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3003));
            return;
        }

        if(!_useGold)
        {
            //��尡 �����մϴ�.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(100));
            return;
        }

        Log.Show(_selectBadgeUID + " / " + _badgeData.BadgeUID);

        _badgeCurLv = _badgeData.Level;
        _badgeCurLvCnt = _badgeData.RemainLvCnt;
        _bSendLevelUp = true;
        GameInfo.Instance.Send_ReqUpgradeBadge(_badgeData.BadgeUID, OnNetAckUpgradeBadge);
	}

    public void OnNetAckUpgradeBadge(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            _bSendLevelUp = false;
            return;
        }

        //if (_badgeData.RemainLvCnt <= 0)
        //{
        //    OnClickClose();
        //    MessageToastPopup.Show(FLocalizeString.Instance.GetText(3004));
        //    return;
        //}

        //Renewal(true);
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        StartCoroutine(BadgeLevelUpResultCoroutine(pktmsg));
    }

    IEnumerator BadgeLevelUpResultCoroutine(PktMsgType pktmsg)
    {
        yield return null;

        kLevelUpResultType.SetActive(false);
        kLevelUpResultSuccess.SetActive(false);
        kLevelUpResultFailed.SetActive(false);
        if (_badgeCurLv < _badgeData.Level)
        {
            //����
            GameSupport.PlayParticle(kLevelUpSuccessEff);
            SoundManager.Instance.PlayUISnd(28);
            yield return new WaitForSeconds(1.5f);

            Renewal(true);

            kLevelUpResultType.SetActive(true);
            kLevelUpResultSuccess.SetActive(true);
            GameSupport.PlayParticle(kLevelUpSuccessAfterEff);

        }
        else
        {
            //����
            GameSupport.PlayParticle(kLevelUpFailedEff);
            SoundManager.Instance.PlayUISnd(73);
            yield return new WaitForSeconds(1.5f);
            Renewal(true);
            kLevelUpResultType.SetActive(true);
            kLevelUpResultFailed.SetActive(true);
        }

        yield return new WaitForSeconds(2f);

        DisableEffects();

        //��ȭȽ�� ��� ����, �ִ뷹�� ���޽�..
        if (_badgeData.RemainLvCnt <= 0 || _badgeData.Level >= GameInfo.Instance.GameConfig.BadgeLvCnt)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3164));
            yield return new WaitForSeconds(1f);
            _bSendLevelUp = false;
            OnClickClose();
        }
        else
        {
            _bSendLevelUp = false;
        }

        
    }

    public override void OnClickClose()
    {
        if (_bSendLevelUp)
            return;
        UIBadgeInfoPopup uIBadgeInfoPopup = LobbyUIManager.Instance.GetActiveUI<UIBadgeInfoPopup>("BadgeInfoPopup");
        if(uIBadgeInfoPopup != null)
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
        if(badgeSelectPopup != null)
        {
            badgeSelectPopup.InitComponent();
            badgeSelectPopup.Renewal(true);
        }

        base.OnClickClose();

    }
}
