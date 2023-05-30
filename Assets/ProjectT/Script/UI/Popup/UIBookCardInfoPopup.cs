using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBookCardInfoPopup : FComponent
{
    public UITexture        kCardTex;
    public UIButton         kCvPlayBtn;
    public UISprite         kGradeSpr;
    public UISprite         kCardTypeSpr;
    public UILabel          kCardTypeLabel;
    public UILabel          kNameLabel;
    public UILabel          kCvLabel;
    public UILabel          kIllustLabel;
    public UILabel          kIllust2Label;
    public UILabel          kTalkLabel;
    public UIButton         kViewBtn;
    public UIButton         kChangeBtn;
    public GameObject       LockChangeObj;
    public UIButton         kFavorBtn;
    public GameObject       LockFavorObj;
    public UITextList       kTextList;
    public UIGaugeUnit      kFavorGaugeUnit;
    public UILabel          kFavorLevelLabel;
    public List<UIButton>   kFavorRewardList;
    public List<UILabel>    kFavorRewardCountList;
    public List<UISprite>   kFavorRewardEFFList;

    public UISprite         kFavorNoticeSpr;
    public UILabel          kBookNoLabel;
    public UIButton         kArrow_RBtn;
    public UIButton         kArrow_LBtn;

    public GameObject       kGaugeFavorRoot;

    private GameTable.Card.Param                _cardtabledata;
    private CardBookData                        _cardbookdata;
    private List<GameClientTable.Book.Param>    _havebooklist   = new List<GameClientTable.Book.Param>();
    private int                                 _nowIndex       = 0;

    private bool                                _bwake;
    private bool                                _bLarge;
    private bool                                _bchange;
    private bool                                _bfavor;

    private List<byte>                          _favorRewardList = new List<byte>();


	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();

        kArrow_RBtn.gameObject.SetActive(true);
        kArrow_LBtn.gameObject.SetActive(true);

        int tableid = (int)UIValue.Instance.GetValue(UIValue.EParamType.BookItemID);

        // 보유 도감 리스트
        _havebooklist.Clear();
        _nowIndex = 0;
        _havebooklist = GameInfo.Instance.GameClientTable.FindAllBook(x => x.Group == (int)eBookGroup.Supporter);
        for (int idx = 0; idx < _havebooklist.Count; idx++)
        {
            if (_havebooklist[idx].ItemID == tableid)
            {
                _nowIndex = idx;
                break;
            }
        }
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();
        SoundManager.Instance.StopVoice();
        RenderTargetChar.Instance.DestroyRenderTarget();
    }

    public override void InitComponent()
	{
        int tableid = (int)UIValue.Instance.GetValue(UIValue.EParamType.BookItemID);
        _cardtabledata = GameInfo.Instance.GameTable.FindCard(tableid);
        _cardbookdata = GameInfo.Instance.GetCardBookData(_cardtabledata.ID);

        _bwake = false;
        _bLarge = false;

        if (_cardbookdata != null)
        {
            _bchange = _cardbookdata.IsOnFlag(eBookStateFlag.MAX_WAKE_AND_LV);
            _bfavor = _cardbookdata.IsOnFlag(eBookStateFlag.MAX_FAVOR_LV);
        }
        else
        {
            _bchange = false;
            _bfavor = false;
        }

        var data = GameInfo.Instance.GameClientTable.FindBook(x => x.Group == (int)eBookGroup.Supporter && x.ItemID == tableid);
        if (data == null)
            return;
        kBookNoLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(216), data.Num);
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        kGradeSpr.spriteName = "itemgrade_L_" + _cardtabledata.Grade.ToString();
        //kGradeSpr.MakePixelPerfect();
        kCardTypeSpr.spriteName = "SupporterType_" + _cardtabledata.Type.ToString();
        kCardTypeLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.CARDTYPE + _cardtabledata.Type);
        if ( !_bwake )
            kCardTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("card", string.Format("Card/{0}_{1}.png", _cardtabledata.Icon, 0));
        else
            kCardTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("card", string.Format("Card/{0}_{1}.png", _cardtabledata.Icon, 1));

        FLocalizeString.SetLabel(kNameLabel, _cardtabledata.Name);
        FLocalizeString.SetLabel(kCvLabel, _cardtabledata.Name + 500000);
        FLocalizeString.SetLabel(kIllustLabel, _cardtabledata.Name + 600000);
        FLocalizeString.SetLabel(kIllust2Label, _cardtabledata.Name + 700000);
        FLocalizeString.SetLabel(kTalkLabel, _cardtabledata.Name + 800000);
        kTextList.textLabel.textlocalize = "";
        kTextList.Clear();
        kTextList.Add(Utility.AppendColorBBCodeString(FLocalizeString.Instance.GetText(_cardtabledata.Name + 900000)));

        _favorRewardList.Clear();

        if (_cardbookdata != null)
        {
            kGaugeFavorRoot.SetActive(true);
            kFavorGaugeUnit.gameObject.SetActive(true);
            kFavorLevelLabel.textlocalize = _cardbookdata.FavorLevel.ToString();
            float fvalue = 1.0f / (float)GameInfo.Instance.GameConfig.CardFavorMaxLevel;
            float f = GameSupport.GetCardFavorLevelExpGauge(_cardbookdata.TableID, _cardbookdata.FavorLevel, _cardbookdata.FavorExp) * fvalue;
            f += (_cardbookdata.FavorLevel * fvalue);
            kFavorGaugeUnit.InitGaugeUnit(f);

            for (int i = 0; i < kFavorRewardList.Count; i++)
            {
                kFavorRewardList[i].isEnabled = false;
                kFavorRewardList[i].gameObject.SetActive(true);
                kFavorRewardEFFList[i].gameObject.SetActive(false);

                if (i + 1 <= _cardbookdata.FavorLevel)
                {
                    if (!_cardbookdata.IsOnFlag(eBookStateFlag.FAVOR_RWD_GET_1 + i))
                    {
                        kFavorRewardList[i].isEnabled = true;
                        kFavorRewardEFFList[i].gameObject.SetActive(true);
                        _favorRewardList.Add((byte)(eBookStateFlag.FAVOR_RWD_GET_1 + i));
                    }
                    else
                    {
                        kFavorRewardList[i].gameObject.SetActive(false);
                    }
                }
            }

            if (_cardbookdata.FavorLevel == GameInfo.Instance.GameConfig.CardFavorMaxLevel)
            {
                bool bAll = true;
                for (int i = 0; i < kFavorRewardList.Count; i++)
                {
                    if (!_cardbookdata.IsOnFlag(eBookStateFlag.FAVOR_RWD_GET_1 + i))
                    {
                        bAll = false;
                        break;
                    }
                }
                if (bAll)
                    kFavorGaugeUnit.gameObject.SetActive(false);
            }
        }
        else
        {
            kGaugeFavorRoot.SetActive(false);
        }

        if (_cardtabledata.Grade >= (int)eGRADE.GRADE_SR)
        {
            kFavorBtn.gameObject.SetActive(true);
            kCvPlayBtn.gameObject.SetActive(true);
        }
        else
        {
            kFavorBtn.gameObject.SetActive(false);
            kCvPlayBtn.gameObject.SetActive(false);
        }

        kFavorNoticeSpr.gameObject.SetActive(false);
        if (_bfavor)
        {
            if (PlayerPrefs.HasKey("NCardBook_Favor_" + _cardtabledata.ID.ToString()))
            {
                if (_cardtabledata.Grade >= (int)eGRADE.GRADE_SR)
                    kFavorNoticeSpr.gameObject.SetActive(true);
                else
                    PlayerPrefs.DeleteKey("NCardBook_Favor_" + _cardtabledata.ID.ToString());
            }
        }

        for (int i = 0; i < GameInfo.Instance.GameConfig.CardFavorMaxLevel; i++)
        {
            var data = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == _cardtabledata.FavorGroup && x.Level == i+1);
            if (data != null)
            {
                var randomdata = GameInfo.Instance.GameTable.FindRandom(x => x.GroupID == data.Value1);
                if (randomdata != null)
                {
                    if( randomdata.ProductType == (int)eREWARDTYPE.GOODS )
                        if (randomdata.ProductIndex == (int)eGOODSTYPE.CASH )
                            kFavorRewardCountList[i].textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), randomdata.ProductValue );
                }
            }
        }

        LockChangeObj.SetActive(!_bchange);
        LockFavorObj.SetActive(!_bfavor);

        if ( !AppMgr.Instance.configData.m_Network ) {
            LockChangeObj.SetActive( false );
            LockFavorObj.SetActive( false );
        }
    }

    public void OnClick_BackBtn()
	{
        OnClickClose();
    }

    public override void OnClickClose()
    {
        LobbyUIManager.Instance.Renewal("BookItemListPopup");
        base.OnClickClose();
    }

    public void OnClick_CvPlayBtn()
	{
        int randValue = Random.Range(0, (int)eVOICESUPPORTER.Count);
        VoiceMgr.Instance.PlaySupporter((eVOICESUPPORTER)randValue,_cardtabledata.ID);
    }

    public void OnClick_ViewBtn()
    {
        OnClick_CardBtn();
    }

    public void OnClick_ChangeBtn()
    {
        if (!GameInfo.Instance.GameConfig.TestMode && !AppMgr.Instance.Review)
        {
            if (!_bchange)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3062));
                return;
            }
        }

        _bwake = !_bwake;
        Renewal(true);
    }

    public void OnClick_FavorBtn()
    {
        if (!GameInfo.Instance.GameConfig.TestMode && !AppMgr.Instance.Review)
        {
            if (!_bfavor)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3070));
                return;
            }
        }

        //SoundManager.Instance.StopBgm();
            
        //  카드 ID 저장
        int scenarioGroupID = _cardtabledata.ScenarioGroupID;
        string scenariobg = _cardtabledata.Icon +"_0";
        string scenariobgSprite = _cardtabledata.SpriteIcon;
        UIValue.Instance.SetValue(UIValue.EParamType.ScenarioGroupID, scenarioGroupID);
        UIValue.Instance.SetValue(UIValue.EParamType.ScenarioFavorBGStr, scenariobg);
        UIValue.Instance.SetValue(UIValue.EParamType.ScenarioFavorBGSprite, scenariobgSprite);
        LobbyUIManager.Instance.ShowUI("BookCardCinemaPopup", true);

        PlayerPrefs.DeleteKey("NCardBook_Favor_" + _cardtabledata.ID.ToString());

        // kFavorNoticeSpr 마크 갱신을 위해 Renewal 처리
        Renewal(true);
    }

    public void OnClick_CardBtn()
    {
        //if (IsPlayAnimtion(0))
        //    return;
        //if (IsPlayAnimtion(1))
        //    return;
        //if (IsPlayAnimtion(2))
        //    return;
        //if (IsPlayAnimtion(3))
        //    return;

        //if (_bLarge)
        //{
        //    PlayAnimtion(2);
        //    _bLarge = false;
        //}
        //else
        //{
        //    PlayAnimtion(3);
        //    _bLarge = true;
        //}
        //Log.Show(_bLarge);

        //LobbyUIManager.Instance.ShowUI("CardViewerPopup", true);
        CardViewer.ShowCardPopup(_cardtabledata.ID, _bwake);
    }

    public void OnClick_FavorRewardBtn(int level)
    {
        if (_favorRewardList.Count == 0)
            return;
        // 호감도 보상 일괄 획득 수정으로 서버 패킷에는 level 필요없지만 NetLocalSvr.Proc_ReqFavorLvRewardCard 함수 기능 때문에 남겨둠
        GameInfo.Instance.Send_ReqFavorLvRewardCard(_cardbookdata.TableID, level, _favorRewardList, OnNetFavorRewardCardBook);
    }

    public void OnClick_Arrow_LBtn()
    {
        int id = GetNextBookID(true);
        if(id != -1 )
        {
            SendBookNewConfirm(id);
            UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, id);
            InitComponent();
            Renewal(true);
        }
    }

    public void OnClick_Arrow_RBtn()
    {
        int id = GetNextBookID(false);
        if (id != -1)
        {
            SendBookNewConfirm(id);
            UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, id);
            InitComponent();
            Renewal(true);
        }
    }

    public void OnClick_CardInfoBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.CardUID, (long)-1);
        UIValue.Instance.SetValue(UIValue.EParamType.CardTableID, _cardtabledata.ID);
        LobbyUIManager.Instance.ShowUI("CardInfoPopup", true);
    }

    private int GetNextBookID(bool bleft)
    {
        if (bleft)
        {
            _nowIndex -= 1;
            if (_nowIndex < 0)
                _nowIndex = _havebooklist.Count - 1;
        }
        else
        {
            _nowIndex += 1;
            if (_nowIndex >= _havebooklist.Count)
                _nowIndex = 0;
        }

        return _havebooklist[_nowIndex].ItemID;
    }

    public void OnNetFavorRewardCardBook(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        Renewal(true);

        MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TITLE), FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TEXT), GameInfo.Instance.RewardList);
        LobbyUIManager.Instance.Renewal("TopPanel");
    }

    private void SendBookNewConfirm(int id)
    {
        var cardtabledata = GameInfo.Instance.GameTable.FindCard(id);
        if (cardtabledata == null)
            return;
        var cardbookdata = GameInfo.Instance.GetCardBookData(cardtabledata.ID);
        if (cardbookdata == null)
            return;
        if (cardbookdata.IsOnFlag(eBookStateFlag.NEW_CHK))
            return;

        GameInfo.Instance.Send_ReqBookNewConfirm((int)eBookGroup.Supporter, cardbookdata.TableID, OnNetSetBookState);
    }

    public void OnNetSetBookState(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
    }

    public override bool IsBackButton()
    {
        if (IsPlayAnimtion(2))
            return false;

        if (_bLarge)
        {
            OnClick_CardBtn();
            return false;
        }
        else
            return true;
        
    }

}
