using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharGradeUpPopup : FComponent
{
    public UITexture kCharTexture;
    public UILabel kNameLabel;
	public UISprite kGradeNowSpr;
	public UISprite kGradeNextSpr;
    public UISprite kGradeEffSpr;
    public UIGoodsUnit kGoodsUnit;
	public UIButton kCancelBtn;
	public UIButton kUpgradeBtn;
    public List<UIItemListSlot> kMatItemList;
    public List<UILabel> kHaveCountLabel;

    public UILabel  LbAwakenPoint;
    public UILabel  LbGradeUp;

    private CharData _chardata;
    private bool _bmat = true;
    private bool _bcoroutine;
    private Vector3 m_originalEffPos;

    private bool mbAwaken = false;


	public override void Awake() {
		base.Awake();

		m_originalEffPos = kGradeEffSpr.transform.localPosition;
	}

	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
        int tableid = (int)UIValue.Instance.GetValue(UIValue.EParamType.CharSelTableID);
        _chardata = GameInfo.Instance.GetCharData(uid);
        _bmat = true;
        _bcoroutine = false;
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        _bmat = true;
        int nowgrade = _chardata.Grade;
        int nextgrade = nowgrade+1;
        kGradeNowSpr.spriteName = string.Format("grade_{0}", nowgrade.ToString("D2"));  //"grade_0" + nowgrade.ToString();
        kGradeNowSpr.MakePixelPerfect();
        kGradeNextSpr.spriteName = string.Format("grade_{0}", nextgrade.ToString("D2")); //"grade_0" + nextgrade.ToString();
        kGradeNextSpr.MakePixelPerfect();

        kCharTexture.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/Full/Full_" + _chardata.TableData.Icon + ".png"); ;

        //  진급하는 해당 캐릭의 이름 셋팅
        FLocalizeString.SetLabel(kNameLabel, _chardata.TableData.Name);

        mbAwaken = nowgrade >= GameInfo.Instance.GameConfig.CharStartAwakenGrade;
        int n = mbAwaken ? GameInfo.Instance.GameConfig.CharStartAwakenGrade : 0;

        Vector3 pos = m_originalEffPos;
        pos.x += ((float)(nowgrade - n) * 26.0f);
        kGradeEffSpr.transform.localPosition = pos;

        if(mbAwaken)
        {
            kGradeEffSpr.color = new Color(1.0f, 0.0f, 0.66f);
        }
        else
        {
            kGradeEffSpr.color = new Color(0.68f, 0.51f, 0.0f);
        }

        for (int i = 0; i < kMatItemList.Count; i++)
        {
            kMatItemList[i].gameObject.SetActive(false);
            kHaveCountLabel[i].gameObject.SetActive(false);
        }

        
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == _chardata.TableData.WakeReqGroup && x.Level == _chardata.Grade);
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
            kGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, reqdata.Gold, true);
        }

        LbAwakenPoint.gameObject.SetActive(false);
        LbGradeUp.textlocalize = FLocalizeString.Instance.GetText(1064);

        if(mbAwaken)
        {
            LbGradeUp.textlocalize = FLocalizeString.Instance.GetText(1627);

            if (nextgrade >= GameInfo.Instance.GameConfig.CharWPStartGrade)
            {
                LbAwakenPoint.gameObject.SetActive(true);
                LbAwakenPoint.textlocalize = string.Format("{0} +{1}", FLocalizeString.Instance.GetText(1628), GameInfo.Instance.GameConfig.CharAwakeGetWP);
            }
        }
    }
 
	public void OnClick_CancelBtn()
	{
        OnClickClose();
	}
	
	public void OnClick_UpgradeBtn()
	{
        if (_chardata == null)
        {
            return;
        }

        if(!GameSupport.IsMaxCharLevel(_chardata.Level, _chardata.Grade))
        {
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3146), GameSupport.GetCharMaxLevel(_chardata.Grade)));
            return;
        }

        if (GameSupport.IsMaxGrade(_chardata.Grade))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3007));
            return;
        }

        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == _chardata.TableData.WakeReqGroup && x.Level == _chardata.Grade);
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
        GameInfo.Instance.Send_ReqGradeUpChar(_chardata.CUID, OnNetCharGardeUpRequest);
    }

    public void OnNetCharGardeUpRequest(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        DirectorUIManager.Instance.PlayCharGradeUp(_chardata.TableID);


        /*
        VoiceMgr.Instance.PlayChar(eVOICECHAR.GradeUp, _chardata.TableID);
        StartCoroutine(CharGardeUpRequestCoroutine());
        */
    }
}
