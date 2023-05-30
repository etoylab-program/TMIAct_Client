using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GemOption
{
    public UILabel NameLabel = null;
    public UILabel LevelLabel = null;
    public UISprite WakeSpr = null;
    public UISprite GradeSpr = null;
    public UIGaugeUnit ExpGaugeUnit = null;
    public UIStatusUnit StatusUnit00 = null;
    public UIStatusUnit StatusUnit01 = null;
    public List<UIGemOptUnit> OptUnitList = new List<UIGemOptUnit>();
}

public class UIGemGradeUpPopup : FComponent
{
	public UILabel kNameLabel;
	public UITexture kGemTex;
	public UILabel kLevelLabel;
    public UISprite kGradeSpr;
    public UISprite kOptEFFSpr;

    public List<UIItemListSlot> kMatItemList;
    public List<UILabel> kHaveCountLabel;
    public List<UIGemOptUnit> kGemOptList;
    public UIGoodsUnit kGoldGoodsUnit;
    public UIButton kCancleBtn;
    public UIButton kLevelUpBtn;

    [Header("Add Gem UR Grade")]
    [SerializeField] private GameObject optObj = null;
    [SerializeField] private UIButton gemUpBtn = null;
    [SerializeField] private GameObject prevGemInfoObj = null;
    [SerializeField] private GameObject nextGemInfoObj = null;
    [SerializeField] private GemOption prevGemInfo = null;
    [SerializeField] private GemOption nextGemInfo = null;

    private GemData _gemdata;
    private bool _bmat = true;
    private Vector3 m_originalEffPos;

	public override void Awake() {
		base.Awake();

		m_originalEffPos = kOptEFFSpr.transform.localPosition;
	}

	public override void OnEnable()
	{
		long uid = (long)UIValue.Instance.GetValue( UIValue.EParamType.GemUID );
		_gemdata = GameInfo.Instance.GetGemData( uid );
		_bmat = true;

		base.OnEnable();
	}
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        bool isEvolution = GameSupport.IsMaxWakeGem(_gemdata) && _gemdata.TableData.Grade == (int)eGRADE.GRADE_SR;

        kNameLabel.SetActive(!isEvolution);
        optObj.SetActive(!isEvolution);
        kLevelUpBtn.SetActive(!isEvolution);

        prevGemInfoObj.SetActive(isEvolution);
        nextGemInfoObj.SetActive(isEvolution);
        gemUpBtn.SetActive(isEvolution);

        if (isEvolution)
        {
            SetEvolutionData();
        }
        else
        {
            SetOptData();
        }
    }

    public void OnClick_BackBtn()
	{
        OnClickClose();
	}
	
	public void OnClick_LevelUpBtn()
	{
        if(!GameSupport.IsMaxLevelGem(_gemdata))
        {
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3012), GameInfo.Instance.GameConfig.GemMaxLevel));
            return;
        }

        if (GameSupport.IsMaxWakeGem(_gemdata))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3014));
            return;
        }
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == _gemdata.TableData.WakeReqGroup && x.Level == _gemdata.Wake);
        if (reqdata == null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3002));
            return;
        }
        if (!GameSupport.IsCheckGoods(eGOODSTYPE.GOLD, reqdata.Gold))
        {
            return;
        }
        if (!_bmat)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3003));
            return; //재료 부족 
        }
        Director.IsPlaying = true;
        GameInfo.Instance.Send_ReqWakeGem(_gemdata.GemUID, OnNetGemWake);
    }

    public void OnClick_GemUpBtn()
    {
        if (!GameSupport.IsMaxLevelGem(_gemdata))
        {
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3012), GameInfo.Instance.GameConfig.GemMaxLevel));
            return;
        }

        if (!GameSupport.IsMaxWakeGem(_gemdata))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3314));
            return;
        }
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == _gemdata.TableData.EvReqGroup);
        if (reqdata == null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3002));
            return;
        }
        if (!GameSupport.IsCheckGoods(eGOODSTYPE.GOLD, reqdata.Gold))
        {
            return;
        }
        if (!_bmat)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3003));
            return; //재료 부족 
        }
        Director.IsPlaying = true;
        GameInfo.Instance.Send_ReqEvolutionGem(_gemdata.GemUID, OnNetGemEvolution);
    }

    public void OnNetGemWake(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        StartCoroutine(GemWakeResultCoroutine());
    }


    IEnumerator GemWakeResultCoroutine()
    {
        UIResultGemGradeUpPopup resultGemGradeUpPopup = LobbyUIManager.Instance.GetUI<UIResultGemGradeUpPopup>("ResultGemGradeUpPopup");
        if (resultGemGradeUpPopup != null)
        {
            resultGemGradeUpPopup.SetEvolution(false);
        }

        DirectorUIManager.Instance.PlayGemWakeUp(_gemdata);
        yield return new WaitForSeconds(1.0f);

        OnClickClose();
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("GemInfoPopup");
        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
        {
            itempanel.InitComponent();
            itempanel.Renewal(true);
        }
        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.CHARINFO)
        {
            LobbyUIManager.Instance.Renewal("CharInfoPanel");
            LobbyUIManager.Instance.InitComponent("WeaponGemSeletePopup");
            LobbyUIManager.Instance.Renewal("WeaponGemSeletePopup");
        }
    }

    public void OnNetGemEvolution(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        StartCoroutine(GemEvolutionResultCoroutine());
    }

    private IEnumerator GemEvolutionResultCoroutine()
    {
        UIResultGemGradeUpPopup resultGemGradeUpPopup = LobbyUIManager.Instance.GetUI<UIResultGemGradeUpPopup>("ResultGemGradeUpPopup");
        if (resultGemGradeUpPopup != null)
        {
            resultGemGradeUpPopup.SetEvolution(true);
        }

        DirectorUIManager.Instance.PlayGemWakeUp(_gemdata);
        yield return new WaitForSeconds(1.0f);

        OnClickClose();
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");

        UIGemInfoPopup gemInfoPopup = LobbyUIManager.Instance.GetActiveUI<UIGemInfoPopup>("GemInfoPopup");
        if (gemInfoPopup != null)
        {
            gemInfoPopup.kGemInfoTypeTab.SetTab((int)UIGemInfoPopup.eGemInfoType.Info, SelectEvent.Code);
            gemInfoPopup.ReSetTableData();
            gemInfoPopup.Renewal(false);
        }

        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
        {
            itempanel.InitComponent();
            itempanel.Renewal(true);
        }
        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.CHARINFO)
        {
            LobbyUIManager.Instance.Renewal("CharInfoPanel");
            LobbyUIManager.Instance.InitComponent("WeaponGemSeletePopup");
            LobbyUIManager.Instance.Renewal("WeaponGemSeletePopup");
        }
    }

    private void SetOptData()
    {
        int wake = _gemdata.Wake;
        kGemTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + _gemdata.TableData.Icon);
        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_gemdata.TableData.Name);
        kGradeSpr.spriteName = "grade_0" + wake.ToString();

        // 연마 시 레벨이 초기화된다는 정보 전달을 위해 레벨 1로 표기
        kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, 1, GameSupport.GetGemMaxLevel());

        for (int i = 0; i < kMatItemList.Count; i++)
        {
            kMatItemList[i].gameObject.SetActive(false);
            kHaveCountLabel[i].gameObject.SetActive(false);
        }

        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == _gemdata.TableData.WakeReqGroup && x.Level == _gemdata.Wake);
        if (reqdata != null)
        {
            List<int> idlist = new List<int>();
            List<int> countlist = new List<int>();
            GameSupport.SetMatList(reqdata, ref idlist, ref countlist);
            for (int i = 0; i < idlist.Count; i++)
            {
                kMatItemList[i].gameObject.SetActive(true);
                kMatItemList[i].UpdateSlot(UIItemListSlot.ePosType.Mat, i, GameInfo.Instance.GameTable.FindItem(idlist[i]));
                int orgcut = GameInfo.Instance.GetItemIDCount(idlist[i]);
                int orgmax = countlist[i];
                string strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
                if (orgcut < orgmax)
                {
                    _bmat = false;
                    strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);
                }
                string strmatcount = string.Format(strHaveCntColor, string.Format(FLocalizeString.Instance.GetText(236), orgcut, orgmax));
                kMatItemList[i].SetCountLabel(strmatcount);
            }
            kGoldGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, reqdata.Gold, true);
        }

        for (int i = 0; i < kGemOptList.Count; i++)
            kGemOptList[i].Lock();

        int optcount = _gemdata.Wake + 1;
        for (int i = 0; i < optcount; i++)
        {
            if (i == _gemdata.Wake)
                kGemOptList[i].Open();
            else
                kGemOptList[i].Opt(_gemdata, i);
        }

        kOptEFFSpr.gameObject.SetActive(true);
        Vector3 pos = m_originalEffPos;
        pos.y -= ((float)_gemdata.Wake * 40.0f);
        kOptEFFSpr.transform.localPosition = pos;
    }

    private void SetEvolutionData()
    {
        for (int i = 0; i < kMatItemList.Count; i++)
        {
            kMatItemList[i].gameObject.SetActive(false);
            kHaveCountLabel[i].gameObject.SetActive(false);
        }

        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == _gemdata.TableData.EvReqGroup);
        if (reqdata != null)
        {
            List<int> idlist = new List<int>();
            List<int> countlist = new List<int>();
            GameSupport.SetMatList(reqdata, ref idlist, ref countlist);
            for (int i = 0; i < idlist.Count; i++)
            {
                kMatItemList[i].gameObject.SetActive(true);
                kMatItemList[i].UpdateSlot(UIItemListSlot.ePosType.Mat, i, GameInfo.Instance.GameTable.FindItem(idlist[i]));
                int orgcut = GameInfo.Instance.GetItemIDCount(idlist[i]);
                int orgmax = countlist[i];
                string strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
                if (orgcut < orgmax)
                {
                    _bmat = false;
                    strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);
                }
                string strmatcount = string.Format(strHaveCntColor, string.Format(FLocalizeString.Instance.GetText(236), orgcut, orgmax));
                kMatItemList[i].SetCountLabel(strmatcount);
            }
            kGoldGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, reqdata.Gold, true);
        }

        // Prev
        {
            prevGemInfo.NameLabel.textlocalize = FLocalizeString.Instance.GetText(_gemdata.TableData.Name);
            prevGemInfo.LevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _gemdata.Level, GameSupport.GetGemMaxLevel());
            prevGemInfo.WakeSpr.spriteName = "itemGemwake_0" + _gemdata.Wake.ToString();
            prevGemInfo.GradeSpr.spriteName = "itemgrade_L_" + _gemdata.TableData.Grade.ToString();

            float fillAmount = GameSupport.GetGemLevelExpGauge(_gemdata, _gemdata.Level, _gemdata.Exp);
            prevGemInfo.ExpGaugeUnit.InitGaugeUnit(fillAmount);
            prevGemInfo.ExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), fillAmount * 100.0f));

            int statusMain = GameSupport.GetTypeStatusGem(_gemdata.TableData.MainType, _gemdata.Level, _gemdata.Wake, _gemdata.TableData);
            int statusSub = GameSupport.GetTypeStatusGem(_gemdata.TableData.SubType, _gemdata.Level, _gemdata.Wake, _gemdata.TableData);

            prevGemInfo.StatusUnit00.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + _gemdata.TableData.MainType, statusMain);

            if (_gemdata.TableData.MainType == _gemdata.TableData.SubType)
            {
                prevGemInfo.StatusUnit01.gameObject.SetActive(false);
            }
            else
            {
                prevGemInfo.StatusUnit01.gameObject.SetActive(true);
                prevGemInfo.StatusUnit01.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + _gemdata.TableData.SubType, statusSub);
            }

            for (int i = 0; i < prevGemInfo.OptUnitList.Count; i++)
            {
                prevGemInfo.OptUnitList[i].Lock();
            }

            int optcount = _gemdata.Wake;
            for (int i = 0; i < optcount; i++)
            {
                if (i == _gemdata.Wake)
                {
                    prevGemInfo.OptUnitList[i].Open();
                }
                else
                {
                    prevGemInfo.OptUnitList[i].Opt(_gemdata, i);
                }
            }
        }

        // Next
        GameTable.Gem.Param nextGemParam = GameInfo.Instance.GameTable.FindGem(_gemdata.TableData.EvolutionResult);
        if (nextGemParam != null)
        {
            nextGemInfo.NameLabel.textlocalize = FLocalizeString.Instance.GetText(nextGemParam.Name);
            nextGemInfo.LevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, 1, GameSupport.GetGemMaxLevel());
            nextGemInfo.WakeSpr.spriteName = "itemGemwake_0" + _gemdata.Wake.ToString();
            nextGemInfo.GradeSpr.spriteName = "itemgrade_L_" + nextGemParam.Grade.ToString();

            nextGemInfo.ExpGaugeUnit.InitGaugeUnit(0);
            nextGemInfo.ExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), 0));

            int statusMain = GameSupport.GetTypeStatusGem(nextGemParam.MainType, 1, _gemdata.Wake, nextGemParam);
            int statusSub = GameSupport.GetTypeStatusGem(nextGemParam.SubType, 1, _gemdata.Wake, nextGemParam);

            nextGemInfo.StatusUnit00.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + nextGemParam.MainType, statusMain);

            if (nextGemParam.MainType == nextGemParam.SubType)
            {
                nextGemInfo.StatusUnit01.gameObject.SetActive(false);
            }
            else
            {
                nextGemInfo.StatusUnit01.gameObject.SetActive(true);
                nextGemInfo.StatusUnit01.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + nextGemParam.SubType, statusSub);
            }

            for (int i = 0; i < nextGemInfo.OptUnitList.Count; i++)
            {
                nextGemInfo.OptUnitList[i].Lock();
            }

            int optcount = _gemdata.Wake;
            for (int i = 0; i < optcount; i++)
            {
                if (i == _gemdata.Wake)
                {
                    nextGemInfo.OptUnitList[i].Open();
                }
                else
                {
                    nextGemInfo.OptUnitList[i].Opt(_gemdata, i);
                }
            }
        }
    }
}
