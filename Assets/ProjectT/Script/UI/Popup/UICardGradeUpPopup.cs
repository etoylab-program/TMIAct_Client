using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICardGradeUpPopup : FComponent
{
	public UISprite kGradeSpr;
	public UILabel kNameLabel;
	public UILabel kLevelNextLabel;
	public UISprite kWakeNextSpr;
	public UISprite kWakeEff;
	public UITexture kCardTex;
    public GameObject kMainSkill;
    public UILabel kMainSkillDescLabel;
    public UILabel kMainSkillNameLabel;
    public UILabel kMainSkillLevelLabel;
    public UILabel kMainSkillTimeLabel;
    public List<UIItemListSlot> kMatItemList;
    public List<UILabel> kHaveCountLabel;
    public UIGoodsUnit kGoldGoodsUnit;
    public UIButton kCancleBtn;
	public UIButton kLevelUpBtn;
    private CardData _carddata;
    private bool _bmat = true;
    private Vector3 m_originalEffPos;

	public override void Awake() {
		base.Awake();

		m_originalEffPos = kWakeEff.transform.localPosition;
	}

	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CardUID);
        _carddata = GameInfo.Instance.GetCardData(uid);
        _bmat = true;
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
        _bmat = true;
        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_carddata.TableData.Name);
        kGradeSpr.spriteName = "itemgrade_L_" + _carddata.TableData.Grade.ToString();
        //kGradeSpr.MakePixelPerfect();

        kCardTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("card", string.Format("Card/{0}_{1}.png", _carddata.TableData.Icon, GameSupport.GetCardImageNum(_carddata)));

        int nextWake = _carddata.Wake + 1;

        // 각성 시 레벨이 초기화된다는 정보 전달을 위해 레벨 1로 표기
        kLevelNextLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, 1, GameSupport.GetMaxLevelCard(_carddata, nextWake), false, true);

        kWakeNextSpr.spriteName = "itemwake_0" + nextWake.ToString();
        kWakeNextSpr.MakePixelPerfect();
        Vector3 pos = m_originalEffPos;
        pos.y -= ((float)_carddata.Wake * 22.0f);
        kWakeEff.transform.localPosition = pos;

        if (_carddata.TableData.MainSkillEffectName > 0)
        {
            kMainSkill.SetActive(true);
            kMainSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(_carddata.TableData.MainSkillEffectName);
            kMainSkillDescLabel.textlocalize = GameSupport.GetCardMainSkillDesc(_carddata.TableData, nextWake);
            if (nextWake == 0)
                kMainSkillLevelLabel.textlocalize = "";
            else
                kMainSkillLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_NOW_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PLUS_TEXT), nextWake));
            kMainSkillLevelLabel.transform.localPosition = new Vector3(kMainSkillNameLabel.transform.localPosition.x + kMainSkillNameLabel.printedSize.x + 10, kMainSkillNameLabel.transform.localPosition.y, 0);

            if (_carddata.TableData.CoolTime == 0)
            {
                kMainSkillTimeLabel.gameObject.SetActive(false);
            }
            else
            {
                kMainSkillTimeLabel.gameObject.SetActive(true);
                kMainSkillTimeLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(263), _carddata.TableData.CoolTime);
            }
        }
        else
            kMainSkill.SetActive(false);

        for (int i = 0; i < kMatItemList.Count; i++)
        {
            kMatItemList[i].gameObject.SetActive(false);
            kHaveCountLabel[i].gameObject.SetActive(false);
        }
        //2013
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == _carddata.TableData.WakeReqGroup && x.Level == _carddata.Wake);
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
                //kHaveCountLabel[i].gameObject.SetActive(true);
                //string strhavecount = string.Format(strHaveCntColor, string.Format(FLocalizeString.Instance.GetText(279), orgcut));
                //kHaveCountLabel[i].textlocalize = strhavecount;
            }
            kGoldGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, reqdata.Gold, true);
        }
    }

    public void OnClick_BackBtn()
    {
        OnClickClose();
    }

	public void OnClick_LevelUpBtn()
	{
        if (GameSupport.GetCardMaxWake(_carddata) == 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3061));
            return;
        }
        if (GameSupport.IsMaxWakeCard(_carddata))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3060));
            return;
        }
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == _carddata.TableData.WakeReqGroup && x.Level == _carddata.Wake);
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
            //재료 부족 
            if (reqdata != null)
            {
                GameSupport.ShowBuyMatItemPopup(reqdata);
            }
            return;
        }
        Director.IsPlaying = true;
        GameInfo.Instance.Send_ReqWakeCard(_carddata.CardUID, OnNetCardWake);
    }

    public void OnNetCardWake(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        StartCoroutine(CardWakeResultCoroutine());
    }

    IEnumerator CardWakeResultCoroutine()
    {
        DirectorUIManager.Instance.PlayCardWakeUp(_carddata);

        yield return new WaitForSeconds(1.0f);

        OnClickClose();
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("CardInfoPopup");
        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
        {
            itempanel.InitComponent();
            itempanel.Renewal(true);
        }
        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.CHARINFO)
        {
            LobbyUIManager.Instance.Renewal("CharInfoPanel");
            LobbyUIManager.Instance.InitComponent("CharCardSeletePopup");
            LobbyUIManager.Instance.Renewal("CharCardSeletePopup");

        }
    }
}
