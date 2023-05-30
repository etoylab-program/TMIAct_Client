using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIUserInfoPopup : FComponent
{
    public List<GameObject> kAchievementList;
    public List<GameObject> kUserInfoList;
    public UITexture kCharTex;
    public UITexture kIconTex;
	public UILabel kNameLabel;
	public UILabel kLevelLabel;
    public UILabel kWordLabel;
    public UILabel kIDLabel;
    public UIGaugeUnit kExpGaugeUnit;
    public UIButton kIconEditBtn;
    public UIButton kNameEditBtn;
	public UIButton kWordEditBtn;
	public UILabel kNormalLoginCountLabel;
	public UILabel kContiLoginLabelCountLabel;
	public UILabel kMileagePointCountLabel;
	public UILabel kRoomPointCountLabel;
	public UILabel kHaveSupporterCountLabel;
	public UILabel kHaveWeaponCountLabel;
	public FToggle kAchievementToggle;
	public FTab kAchievementTab;
    public UIAchievementListSlot kAchievementScore;
    public UISprite kIconEditNoticeSpr;
    public UISprite kAchievementNoticeSpr;
    public List<UISprite> AchievementTabNoticeList;
    [SerializeField] private FList _AchievementListInstance;
    public UIButton kAllGetBtn;
    public GameObject kDisAllGet;
    public UILabel kTestTextLabel;

    public GameObject kCardTeamObj;
    public GameObject kCardTeamNoneObj;
    public UICardArmoryTeamSlot kCardTeamSlot;

    [Header("Circle")]
    [SerializeField] private UILabel circleId = null;
    [SerializeField] private UILabel circleName = null;
    [SerializeField] private UILabel circleRank = null;
    [SerializeField] private UILabel circleLeaderName = null;
    [SerializeField] private UITexture circleFlagTex = null;
    [SerializeField] private UITexture circleMarkTex = null;

    private List<int> _achievementtabnoticelist = new List<int>();
    private List<AchieveData> _achievelist = new List<AchieveData>();
    private AchieveData _achievecontribution;
    private List<RewardData> rewardList = new List<RewardData>();
    private bool _bsendtakeachieve = false;

    private bool _prevRenderCharActive = false;
    private int _prevCharTableID = -1;
    private long _prevCharUID = -1;

    public override void Awake()
	{
        
        base.Awake();

		kAchievementToggle.EventCallBack = OnAchievementToggleSelect;
		kAchievementTab.EventCallBack = OnAchievementTabSelect;
		if(this._AchievementListInstance == null) return;
		
		this._AchievementListInstance.EventUpdate = this._UpdateAchievementListSlot;
		this._AchievementListInstance.EventGetItemCount = this._GetAchievementElementCount;
        this._AchievementListInstance.InitBottomFixing();
    }

    public override void OnEnable()
	{
        _bsendtakeachieve = false;
        
        /*
        //전에 렌더 캐릭터가 켜져있다면 캐릭터 정보 저장.
        if(RenderTargetChar.Instance.gameObject.activeSelf)
        {
            if(RenderTargetChar.Instance.RenderPlayer != null)
            {
                _prevRenderCharActive = true;
                _prevCharTableID = RenderTargetChar.Instance.RenderPlayerTableID;
                _prevCharUID = RenderTargetChar.Instance.RenderPlayerUID;
            }
            
        }

        CharData chardata = GameInfo.Instance.GetMainChar();
        RenderTargetChar.Instance.gameObject.SetActive(true);
        RenderTargetChar.Instance.InitRenderTargetChar(chardata.TableID, chardata.CUID, false, eCharacterType.Character);
        */

        _achievecontribution = GameInfo.Instance.AchieveList.Find(x => x.GroupID == GameInfo.Instance.GameConfig.AchieveContributionGroupID);

        InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
		UIValue.Instance.SetValue( UIValue.EParamType.CardFormationType, eCharSelectFlag.USER_INFO );

		_achievementtabnoticelist.Clear();
        int tab = 0;
        var obj = UIValue.Instance.GetValue(UIValue.EParamType.UserInfoPopup);
        if (obj != null)
            tab = (int)obj;
        kAchievementToggle.SetToggle(tab, SelectEvent.Code);

        SaveRenderChar();

        CircleData circleData = GameInfo.Instance.CircleData;

        circleFlagTex.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture(circleData.FlagId);
        circleMarkTex.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture(circleData.MarkId);
        circleFlagTex.color = LobbyUIManager.Instance.GetCircleMarkColor(circleData.ColorId);

        circleId.textlocalize = circleData.GetStringUid();
        circleRank.textlocalize = circleData.GetStringRank();
        circleName.textlocalize = circleData.Name;
        circleLeaderName.textlocalize = circleData.Leader.Name;
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        UserData userdata = GameInfo.Instance.UserData;

        CharData chardata = GameInfo.Instance.GetMainChar();
        RenderTargetChar.Instance.gameObject.SetActive(true);
        RenderTargetChar.Instance.InitRenderTargetChar(chardata.TableID, chardata.CUID, false, eCharacterType.Character);

        LobbyUIManager.Instance.GetUserMarkIcon(this.gameObject, this.gameObject, GameInfo.Instance.UserData.UserMarkID, ref kIconTex);

        kLevelLabel.textlocalize = userdata.Level.ToString();//string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.RANK_TXT_NOW_LV), userdata.Level);
        kNameLabel.textlocalize = userdata.GetNickName();
        kIDLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1255), userdata.UUID);

        float fillAmount = GameSupport.GetAccountLevelExpGauge(userdata.Level, userdata.Exp);
        kExpGaugeUnit.InitGaugeUnit(fillAmount);
        //kExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT), (fillAmount * 100.0f)));

        float perVal = fillAmount * 100.0f;
        double truncateVal = System.Math.Truncate(perVal * 100f) / 100f;
        kExpGaugeUnit.SetText(string.Format("{0:#.##}%", truncateVal));

        if (userdata.UserWord == string.Empty || userdata.UserWord == "")
            kWordLabel.textlocalize = FLocalizeString.Instance.GetText(1260);
        else
            kWordLabel.textlocalize = userdata.UserWord;


        int countcard = GameInfo.Instance.CardList.Count;
        int countweapon = GameInfo.Instance.WeaponList.Count;

        kNormalLoginCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(281), userdata.LoginTotalCount);
        kContiLoginLabelCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(281), userdata.LoginContinuityCount);
        kMileagePointCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(270), userdata.Goods[(int)eGOODSTYPE.SUPPORTERPOINT]);
        kRoomPointCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(270), userdata.Goods[(int)eGOODSTYPE.ROOMPOINT]);
        kHaveSupporterCountLabel.textlocalize = countcard.ToString();
        kHaveWeaponCountLabel.textlocalize = countweapon.ToString();

        kAchievementScore.ParentGO = this.gameObject;
        kAchievementScore.UpdateSlot(-1, _achievecontribution);
        _AchievementListInstance.RefreshNotMove();

        if (GameInfo.Instance.NewIconList.Count != 0)
            kIconEditNoticeSpr.gameObject.SetActive(true);
        else
            kIconEditNoticeSpr.gameObject.SetActive(false);

        for( int i = 0; i < AchievementTabNoticeList.Count; i++ )
            AchievementTabNoticeList[i].gameObject.SetActive(false);

        if (NotificationManager.Instance.IsAchievementComplete)
        {
            kAchievementNoticeSpr.gameObject.SetActive(true);

            if (_achievementtabnoticelist.Count == 0)
                CheckAchievementTabNotice();

            for (int i = 0; i < AchievementTabNoticeList.Count; i++)
                if (_achievementtabnoticelist[i] != 0)
                    AchievementTabNoticeList[i].gameObject.SetActive(true);

            kAllGetBtn.gameObject.SetActive(true);
            kDisAllGet.gameObject.SetActive(false);
        }
        else
        {
            kAchievementNoticeSpr.gameObject.SetActive(false);

            kAllGetBtn.gameObject.SetActive(false);
            kDisAllGet.gameObject.SetActive(true);
        }

        if (GameInfo.Instance.UserData.CardFormationID == (int)eCOUNT.NONE)
        {
            kCardTeamNoneObj.SetActive(true);
            kCardTeamSlot.SetActive(false);
        }
        else
        {
            kCardTeamNoneObj.SetActive(false);
            kCardTeamSlot.SetActive(true);
            kCardTeamSlot.UpdateSlot();
        }

        //_AchievementListInstance.ScrollPositionSet();
        kTestTextLabel.gameObject.SetActive(false);
        if ( AppMgr.Instance.DebugInfo )
        {
            string text = string.Empty;
            string strUniqueIdentifier = Platforms.IBase.Inst.GetDeviceUniqueID();
            text += strUniqueIdentifier;
            text += "\n\n";
            text += string.Format("Svn Revision  : {0}", AppMgr.Instance.configData.m_svnrevision);
            text += "\n";
            text += string.Format("Version : {0}", AppMgr.Instance.configData.m_version);
            text += "\n";
            text += string.Format("Server Version : {0}.{1}", AppMgr.Instance.configData.m_serverversionmain, AppMgr.Instance.configData.m_serverversionsub);
            text += "\n";
            text += string.Format("Review : {0}", AppMgr.Instance.Review.ToString() );
            text += "\n";
            text += string.Format("ResPlatform : {0}", AppMgr.Instance.ResPlatform.ToString());
            text += "\n";
            text += string.Format("FCM : " + LocalPushNotificationManager.Instance.FcmToken);

            kTestTextLabel.gameObject.SetActive(true);
            kTestTextLabel.textlocalize = text;
        }
    }

    public void CheckAchievementTabNotice()
    {
        _achievementtabnoticelist.Clear();
        for (int i = 0; i < 4; i++)
            _achievementtabnoticelist.Add(0);

        for (int i = 0; i < GameInfo.Instance.AchieveList.Count; i++)
        {
            if (GameSupport.IsAchievementComplete(GameInfo.Instance.AchieveList[i]))
            {
                int kind = GameInfo.Instance.AchieveList[i].TableData.AchieveKind;
                if (0 <= kind && _achievementtabnoticelist.Count > kind)
                {
                    _achievementtabnoticelist[kind] += 1;
                }
            }
        }

    }

    private bool OnAchievementToggleSelect(int nSelect, SelectEvent type)
	{
        for (int i = 0; i < kAchievementList.Count; i++)
            kAchievementList[i].SetActive(false);
        for (int i = 0; i < kUserInfoList.Count; i++)
            kUserInfoList[i].SetActive(false);

        kCardTeamObj.SetActive(false);
        if ( nSelect == 0 )
        {
            for (int i = 0; i < kUserInfoList.Count; i++)
                kUserInfoList[i].SetActive(true);

            kCardTeamObj.SetActive(true);
        }
        else
        {
            for (int i = 0; i < kAchievementList.Count; i++)
                kAchievementList[i].SetActive(true);

            kCardTeamObj.SetActive(false);
        }

        return true;
    }

	private bool OnAchievementTabSelect( int nSelect, SelectEvent type ) {
		List<AchieveData> list = GameInfo.Instance.AchieveList.FindAll( x => x.TableData != null && 
                                                                             x.GroupID != GameInfo.Instance.GameConfig.AchieveContributionGroupID &&
                                                                             x.TableData.AchieveKind == kAchievementTab.kSelectTab );

		List<AchieveData> completelist = new List<AchieveData>();
		List<AchieveData> totalcompletelist = new List<AchieveData>();
		_achievelist.Clear();

		for ( int i = 0; i < list.Count; i++ ) {
            if ( list[i] == null ) {
                continue;
			}

			if ( list[i].bTotalComplete ) {
				totalcompletelist.Add( list[i] );
				continue;
			}

			bool b = GameSupport.IsAchievementComplete( list[i] );
            if ( b ) {
                completelist.Add( list[i] );
            }
            else {
                _achievelist.Add( list[i] );
            }
		}

        for ( int i = 0; i < completelist.Count; i++ ) {
            _achievelist.Insert( 0, completelist[i] );
        }

        for ( int i = 0; i < totalcompletelist.Count; i++ ) {
            _achievelist.Add( totalcompletelist[i] );
        }

		_AchievementListInstance.UpdateList();
		Renewal( true );

		return true;
	}

	private void _UpdateAchievementListSlot(int index, GameObject slotObject)
	{
        do
        {
            UIAchievementListSlot card = slotObject.GetComponent<UIAchievementListSlot>();
            if (null == card) break;

            AchieveData data = null;
            if (0 <= index && _achievelist.Count > index)
                data = _achievelist[index];

            card.ParentGO = this.gameObject;
            card.UpdateSlot(index, data);
        } while (false);
    }
	
	private int _GetAchievementElementCount()
	{
		return _achievelist.Count; //TempValue
	}

    public void OnClick_BackBtn()
    {
        OnClickClose();
    }

    public override void OnClickClose()
    {
        if (_prevRenderCharActive)
        {
            _prevRenderCharActive = false;
            RenderTargetChar.Instance.gameObject.SetActive(true);
            RenderTargetChar.Instance.InitRenderTargetChar(_prevCharTableID, _prevCharUID, false, eCharacterType.Character);
            _prevCharTableID = -1;
            _prevCharUID = -1;
        }

        LobbyUIManager.Instance.Renewal("MenuPopup");

        base.OnClickClose();
    }

    public void OnClick_IconEditBtn()
	{
        if (!GameInfo.Instance.IsUserMark(GameInfo.Instance.UserData.UserMarkID))
            GameInfo.Instance.Send_ReqUserMarkList(OnNetUserMarkList);
        else
            LobbyUIManager.Instance.ShowUI("UserIconSeletePopup", true);
    }
	
	public void OnClick_NameEditBtn()
	{
        InputFieldPopup.Show(FLocalizeString.Instance.GetText(3072), 
            FLocalizeString.Instance.GetText(3073), 
            GameInfo.Instance.UserData.GetRawNickName(),//kNameLabel.text, 
            GameInfo.Instance.GameConfig.NameMaxLength,
            GameInfo.Instance.GameConfig.NameAddLength, 
            false, 
            OnNickNameSubmit);
    }

    public void OnNickNameSubmit()
    {
        GameInfo.Instance.Send_ReqUserSetName(InputFieldPopup.GetInputTextWithClose(), OnNetSetUserNickName);
    }
	
	public void OnClick_WordEditBtn()
	{
        InputFieldPopup.Show(FLocalizeString.Instance.GetText(3074), 
            FLocalizeString.Instance.GetText(3075), 
            kWordLabel.text, 
            GameInfo.Instance.GameConfig.WordMaxLength,
            GameInfo.Instance.GameConfig.WordAddLength,
            true, 
            OnWordEditSubmit);
    }

    public void OnWordEditSubmit()
    {
        GameInfo.Instance.Send_ReqUserSetCommentMsg(InputFieldPopup.GetInputTextWithClose(), OnNetSetUserWord);
    }

    public void OnClick_MainCharBtn()
    {
        //UIValue.Instance.SetValue(UIValue.EParamType.CharSeletePopupType, (int)eCharSelectFlag.USER_INFO);
        //LobbyUIManager.Instance.ShowUI("CharSeletePopup", true);
        LobbyUIManager.Instance.ShowUI("MainCharSetPopup", true);
    }

    public void OnClick_AllGetBtn()
    {
        rewardList.Clear();

        List<int> completelist = new List<int>();
        for (int i = 0; i < GameInfo.Instance.AchieveList.Count; i++)
        {
            if (GameInfo.Instance.AchieveList[i].bTotalComplete)
                continue;

            bool b = GameSupport.IsAchievementComplete(GameInfo.Instance.AchieveList[i]);
            if (b)
            {
                completelist.Add(GameInfo.Instance.AchieveList[i].GroupID);

                rewardList.Add(new RewardData(
                    GameInfo.Instance.AchieveList[i].TableData.ProductType,
                    GameInfo.Instance.AchieveList[i].TableData.ProductIndex,
                    GameInfo.Instance.AchieveList[i].TableData.ProductValue));
            }
        }

        if (completelist.Count == 0)
            return;
        if (_bsendtakeachieve)
            return;

        _bsendtakeachieve = true;
        GameInfo.Instance.Send_ReqRewardTakeAchieve(completelist, OnNetRewardTakeAchieve);

    }

    public void OnClick_GetBtn( int groupid )
    {
        AchieveData data = GameInfo.Instance.AchieveList.Find(x => x.GroupID == groupid );
        if (data == null)
            return;

        if (data.bTotalComplete)
            return;
        bool b = GameSupport.IsAchievementComplete(data);
        if (!b)
            return;

        if (_bsendtakeachieve)
            return;
        List<int> completelist = new List<int>();
        completelist.Add(groupid);

        rewardList.Clear();
        rewardList.Add(new RewardData(data.TableData.ProductType, data.TableData.ProductIndex, data.TableData.ProductValue));

        _bsendtakeachieve = true;
        GameInfo.Instance.Send_ReqRewardTakeAchieve(completelist, OnNetRewardTakeAchieve);
    }
    public void OnNetRewardTakeAchieve(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
        
        string title = FLocalizeString.Instance.GetText(1142);
        string desc = FLocalizeString.Instance.GetText(1336);

        GameInfo.Instance.RewardList.Clear();
        for (int i = 0; i < rewardList.Count; i++) {
            //GameInfo.Instance.RewardList.Add(new RewardData(rewardList[i].Type, rewardList[i].Index, rewardList[i].Value));
            GameInfo.Instance.RewardList.Add(rewardList[i]);
        }

        MessageRewardListPopup.RewardListMessage(title, desc, GameInfo.Instance.RewardList);

        RenewalAchievement();
        _bsendtakeachieve = false;
    }

    public void RenewalAchievement()
    {
        NotificationManager.Instance.Init();
        CheckAchievementTabNotice();
        OnAchievementTabSelect(kAchievementTab.kSelectTab, SelectEvent.Code);
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
    }

    public void SeleteMainChar( long cuid )
    {
        GameInfo.Instance.Send_ReqChangeMainChar(cuid, OnNetCharMain);
    }

    public void OnNetCharMain(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        Lobby.Instance.ChangeMainChar();

        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3022));

        var chardata = GameInfo.Instance.GetMainChar();

        RenderTargetChar.Instance.gameObject.SetActive(true);
        RenderTargetChar.Instance.InitRenderTargetChar(chardata.TableID, chardata.CUID, false, eCharacterType.Character);

        VoiceMgr.Instance.PlayChar(eVOICECHAR.MainChar, chardata.TableID);

        Renewal(true);
    }

    public void OnNetSetUserNickName(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        Renewal(true);

        //닉네임 갱신
        UITopPanel toppanel = LobbyUIManager.Instance.GetActiveUI<UITopPanel>("TopPanel");
        toppanel.Renewal(true);
    }

    public void OnNetSetUserWord(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        Renewal(true);
    }

    public void OnNetUserMarkList(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
        LobbyUIManager.Instance.ShowUI("UserIconSeletePopup", true);
    }
        
    public void OnClick_CopyToUserUUID()
    {
        UniClipboard.SetText(GameInfo.Instance.UserData.UUID.ToString());
        MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3175), FLocalizeString.Instance.GetText(1507)));
    }
    private void SetCardTeam()
    {
        if (GameInfo.Instance.UserData.CardFormationID == (int)eCOUNT.NONE)
        {
            //장착 하지 않음

        }
        else
        {
            //장착 중
        }
    }

    public void OnClick_ChanegeBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.CardFormationType, eCharSelectFlag.USER_INFO);
        LobbyUIManager.Instance.ShowUI("ArmoryPopup", true);
    }

    public void OnBtnChangeNickNameColor() {
        LobbyUIManager.Instance.ShowUI( "UsernamecolorPopup", true );
	}

    private void SaveRenderChar()
    {
        //전에 렌더 캐릭터가 켜져있다면 캐릭터 정보 저장.
        if (RenderTargetChar.Instance.gameObject.activeSelf)
        {
            if (RenderTargetChar.Instance.RenderPlayer != null)
            {
                _prevRenderCharActive = true;
                _prevCharTableID = RenderTargetChar.Instance.RenderPlayerTableID;
                _prevCharUID = RenderTargetChar.Instance.RenderPlayerUID;
            }

        }
    }
}
