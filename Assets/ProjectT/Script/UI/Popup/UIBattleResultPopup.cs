using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using Holoville.HOTween;

public class UIBattleResultPopup : FComponent
{
    public class sCharExpInfo {
        public int          GageAllGainExp;
        public int          CurLevel;
        public int          CurGrade;
        public int          CurExp;
        public int          GainExp;
        public Coroutine    Coroutine;
    }

    [System.Serializable]
    public class sCharInfo {
        public GameObject   Obj;
        public UISprite     NameSpr;
        public UILabel      LevelLabel;
        public UILabel      AddExpLabel;
        public UIGaugeUnit  GaugeUnit;
        public GameObject   LevelUp;

        [System.NonSerialized]
        public sCharExpInfo ExpInfo = new sCharExpInfo();
    }


    public UILabel kStageNameLabel;
    public UILabel kStageNameLabel1;
    public UILabel kStageNameLabel2;


    public UITexture kUserIconTex;
    public UILabel kUserLevelLabel;
    public UILabel kUserAddExpLabel;
    public UIGaugeUnit kUserGaugeUnit;
    public GameObject kUserLevelUp;

    
    public UITexture kCharIconTex;
    public UISprite kCharNameSpr;
    public UILabel kCharLevelLabel;
    public UILabel kCharAddExpLabel;
    public UIGaugeUnit kCharGaugeUnit;
    public GameObject kCharLevelUp;

    public List<GameObject> kCardOn;
    public List<GameObject> kCardOff;
    public List<UISprite> kCardIconBGSpr;
    public List<UITexture> kCardIconTex;
    public List<UILabel> kCardLevelLabel;
    public List<UILabel> kCardAddExpLabel;
    public List<UIGaugeUnit> kCardGaugeUnit;
    public List<GameObject> kCardLevelUp;

    public List<UIGoodsUnit>  kGoodsUnitList;

    public UIButton kConfirmBtn;
    public UIButton kRetryBtn;

    [SerializeField] private FList _ItemListInstance;

    public int kUserGageAllGainExp = -1;
    private int _curuserlevel = -1;
    private int _curuserexp = -1;
    private int _gainuserexp = -1;
    private Coroutine m_crUser = null;

    public int kCharGageAllGainExp = -1;
    private int _curcharlevel = -1;
    private int _curchargrade = -1;
    private int _curcharexp = -1;
    private int _gaincharexp = -1;
    private Coroutine m_crChar;

    [HideInInspector] public int[] kCardGageAllGainExp = new int[(int)eCOUNT.CARDSLOT];
    private int[] _curcardlevel = new int[(int)eCOUNT.CARDSLOT];
    private int[] _curcardexp = new int[(int)eCOUNT.CARDSLOT];
    private int[] _gaincardexp = new int[(int)eCOUNT.CARDSLOT];
    private Coroutine[] m_crCard = new Coroutine[(int)eCOUNT.CARDSLOT];

    private CardData[] cardlist = new CardData[(int)eCOUNT.CARDSLOT];
    private int _type = 0;
    private int _goodscount;
    private List<int> _tableidlist = new List<int>();

    private List<UIRewardBoxListSlot> _rewardlist = new List<UIRewardBoxListSlot>();
    private List<RewardData> _rewarddatalist = new List<RewardData>();
    private bool _buserrankup; //
    private bool _buserrankuppopup;
    private bool _buserexpgaugestop;
    private Coroutine m_coroutine = null;

    private bool mRetry = false;

    private CharData _charData;
    private GameTable.Stage.Param mStageData = null;

    public bool bContinue { get; set; }
    private bool _blevelupcardvoice = false;

    //서포터 조건이 있는지 여부
    private eSTAGE_CONDI _cardCondiType = eSTAGE_CONDI.NONE;
    private bool _cardCondiMissionClear = false;

    //다시시작할때 스킬빼야하는지 체크용
    private int _stageid = -1;
    private long _seletecuid = -1;
    private int _multipleindex = 0;
    
    public GameObject kOneStoreObj;
    public UILabel kSubjugationLabel;
    public ParticleSystem kFastQuestTicketEffect;
    public UILabel kSubjugationAPTicketLabel;

    public UIButton kEnemyInfoBtn;
    public UILabel kMultipleLabel;
    public UISprite kMultipleTicketIconSpr;
    public UILabel kMultipleRewardLabel;
    public GameObject kTicketPopupCount;

    public UISprite kTicketIconSpr;
    public UILabel kTicketLabel;

    public GameObject kMultibleGC;
    public FToggle kMultiGCToggle;
    public UIButton kTicketPlusBtn;
    public UIButton kTicketMinusBtn;

    [Header("[Raid]")]
    [SerializeField] private GameObject     _NormalObj;
    [SerializeField] private GameObject     _RaidObj;
    [SerializeField] private sCharInfo[]    _CharInfos;
    [SerializeField] private UIGoodsUnit    _RaidPointGoodsUnit;
    [SerializeField] private UIGoodsUnit    _RaidGoldGoodsUnit;


    private bool _multiGCFlag = false;

    private bool _bFastQuestTicket = false;

    private bool _bPreInit = true;

    private bool _bClearmission = false;
    private eGOODSTYPE _tickettype;
    private int _ticket = (int)eCOUNT.NONE;
    bool _bTicketcostdw = false;

	private WaitForFixedUpdate mWaitForFixedUpdate = new WaitForFixedUpdate();

    
    public override void Awake()
    {
        base.Awake();

        if (this._ItemListInstance == null) return;

        this._ItemListInstance.EventUpdate = this._UpdateItemListSlot;
        this._ItemListInstance.EventGetItemCount = this._GetItemElementCount;

        kMultiGCToggle.EventCallBack = OnMultibleGCToggleSelect;
    }

    public override void OnEnable()
    {
        GameResultData gameresultdata = GameInfo.Instance.GameResultData;

        m_crUser = null;
        kUserGageAllGainExp = -1;

        m_crChar = null;
        kCharGageAllGainExp = -1;

        for( int i = 0; i < (int)eCOUNT.CARDSLOT; i++ )
            m_crCard[i] = null;
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
            kCardGageAllGainExp[i] = -1;
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
            cardlist[i] = null;
        
        
        _type = 0;

        _buserrankup = false;
        _buserrankuppopup = false;
        _buserexpgaugestop = false;

        _blevelupcardvoice = false;
        if ( GameInfo.Instance.GameResultData.UserBeforeLevel != GameInfo.Instance.GameResultData.UserAfterLevel )
            _buserrankup = true;

      
        InitResult();
        
        FastQuestTicketRenewal(true);
        
        base.OnEnable();

        m_coroutine = StartCoroutine(ResultCoroutineExp());

        if( mStageData.StageType == (int)eSTAGETYPE.STAGE_RAID ) {
            for( int i = 0; i < GameInfo.Instance.RaidUserData.CharUidList.Count; i++ ) {
                CharData charData = GameInfo.Instance.GetCharData( GameInfo.Instance.RaidUserData.CharUidList[i] );
                StartCoroutine( RaidCharResultCoroutineExp( i, charData ) );
            }
        }

        Lobby.Instance.lobbyCamera.EnableCamera(false);
        mRetry = false;
        Director.IsPlaying = false;
    }
    
    private IEnumerator ShowFastQuestEffect(System.Action action)
    {
        kFastQuestTicketEffect.gameObject.SetActive(true);
        
        while (true)
        {
            yield return null;

            if (!kFastQuestTicketEffect.isPlaying)
            {
                kFastQuestTicketEffect.gameObject.SetActive(false);
                break;
            }
        }
        
        action?.Invoke();
    }
    
    public void Rewind()
    {
        InitResult();
        FastQuestTicketRenewal(true);
        
        m_coroutine = StartCoroutine(ResultCoroutineExp());

        if( mStageData.StageType == (int)eSTAGETYPE.STAGE_RAID ) {
            for( int i = 0; i < GameInfo.Instance.RaidUserData.CharUidList.Count; i++ ) {
                CharData charData = GameInfo.Instance.GetCharData( GameInfo.Instance.RaidUserData.CharUidList[i] );
                StartCoroutine( RaidCharResultCoroutineExp( i, charData ) );
            }
        }
    }

    private void FastQuestTicketRenewal(bool isRewindAnim = false)
    {
        if (_bFastQuestTicket)
        {
            kSubjugationAPTicketLabel.textlocalize = kTicketLabel.textlocalize;
            
            ItemData item = GameInfo.Instance.ItemList.Find(x => x.TableID.Equals(GameInfo.Instance.GameConfig.FastQuestTicketID));
            int itemCount = item?.Count ?? 0;
            int multiple = GameInfo.Instance.GameConfig.MultipleList[_multipleindex];
            if (_multiGCFlag)
            {
                GuerrillaCampData multigcdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Stage_Multiple, mStageData.StageType);
                if (multigcdata != null)
                {
                    multiple = multigcdata.EffectValue;
                }
            }
            eTEXTID textId = multiple <= itemCount ? eTEXTID.WHITE_TEXT_COLOR : eTEXTID.RED_TEXT_COLOR;
            kSubjugationLabel.textlocalize = FLocalizeString.Instance.GetText((int) textId, 
                FLocalizeString.Instance.GetText(236, itemCount, multiple));

            if (isRewindAnim)
            {
                PlayAnimtion(1);
                StartCoroutine(ShowFastQuestEffect(() => { PlayAnimtion(0); }));
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

        //결과창이 닫히면 클리어 정보는 필요없기 때문에 초기화 해줍니다
        //이후 스킬 훈련장에 갔다와도 활성화 안되도록 설정
        GameInfo.Instance.StageClearID = -1;

        if (!mRetry)
        {
			if (Lobby.Instance && Lobby.Instance.lobbyCamera)
			{
				Lobby.Instance.lobbyCamera.EnableCamera(true);
			}
        }
    }

    void InitResult()
    {
        _NormalObj.SetActive( true );
        _RaidObj.SetActive( false );

        GameResultData gameresultdata = GameInfo.Instance.GameResultData;
        UserData userdata = GameInfo.Instance.UserData;
        _charData = GameInfo.Instance.GetCharData(gameresultdata.CharUID);

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
            cardlist[i] = GameInfo.Instance.GetCardData(gameresultdata.CardUID[i]);

        bContinue = true;

        mStageData = GameInfo.Instance.GetStageData( gameresultdata.StageID ); //GameInfo.Instance.RaidUserData.CurStageParam;
        if (mStageData == null)
        {
            return;
        }

        if (mStageData.ID <= (int)eFireBaseLogType._STAGE_END_)
        {
            GameSupport.SendFireBaseLogEvent((eFireBaseLogType)mStageData.ID);
        }

        //조건 드랍 체크
        _cardCondiType = (eSTAGE_CONDI)mStageData.Condi_Type;
        _cardCondiMissionClear = false;
        if(_cardCondiType != eSTAGE_CONDI.NONE && _cardCondiType != eSTAGE_CONDI.NOT_CHECK_CONDI && _cardCondiType != eSTAGE_CONDI.INFLUENCE_CONDI)
        {
            _cardCondiMissionClear = GameSupport.GetCardCondiCheck(_charData, (eSTAGE_CONDI)mStageData.Condi_Type, mStageData.Condi_Value);
        }

        kUserLevelUp.SetActive(false);
        kCharLevelUp.SetActive(false);
        for( int i = 0; i < (int)eCOUNT.CARDSLOT; i++ )
            kCardLevelUp[i].SetActive(false);


        kStageNameLabel1.textlocalize = kStageNameLabel2.textlocalize = kStageNameLabel.textlocalize = FLocalizeString.Instance.GetText(mStageData.Name);

        float fillAmount = 0.0f;

        _curuserlevel = gameresultdata.UserBeforeLevel;
        _curuserexp = gameresultdata.UserBeforeExp;
        _gainuserexp = gameresultdata.UserBeforeExp;
        kUserGageAllGainExp = -1;

        fillAmount = GameSupport.GetAccountLevelExpGauge(_curuserlevel, _curuserexp);
        kUserGaugeUnit.InitGaugeUnit(fillAmount);
        if (gameresultdata.UserBeforeLevel == gameresultdata.UserAfterLevel && gameresultdata.UserBeforeExp == gameresultdata.UserAfterExp)
            kUserGaugeUnit.InitGaugeUnit(1.0f);

        if (GameSupport.IsMaxAccountLevel(_curuserlevel))
            kUserGaugeUnit.SetText(FLocalizeString.Instance.GetText(221));
        else
            kUserGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT), (fillAmount * 100.0f)));

        LobbyUIManager.Instance.GetUserMarkIcon(this.gameObject, this.gameObject, GameInfo.Instance.UserData.UserMarkID, ref kUserIconTex);

        kUserLevelLabel.textlocalize = _curuserlevel.ToString();//string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.RANK_TXT_NOW_LV), _curuserlevel);
        kUserAddExpLabel.textlocalize = "";
        kUserLevelUp.SetActive(false);

        if (_charData != null)
        {
            kCharIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/Full/Full_" + _charData.TableData.Icon + ".png");
            kCharNameSpr.spriteName = "Name_Horizontal_" + ((ePlayerCharType)_charData.TableData.ID).ToString();

            _curcharlevel = gameresultdata.CharBeforeLevel;
            _curchargrade = gameresultdata.CharBeforeGrade;
            _curcharexp = gameresultdata.CharBeforeExp;
            _gaincharexp = gameresultdata.CharBeforeExp;
            kCharGageAllGainExp = -1;
            fillAmount = GameSupport.GetCharacterLevelExpGauge(_curcharlevel, _curchargrade, _curcharexp);
            kCharGaugeUnit.InitGaugeUnit(fillAmount);

            if (GameSupport.IsMaxCharLevel(_curcharlevel, _curchargrade))
                kCharGaugeUnit.SetText(FLocalizeString.Instance.GetText(221));
            else
                kCharGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT), (fillAmount * 100.0f)));

            kCharLevelLabel.textlocalize = _curcharlevel.ToString();//GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curcharlevel);
            kCharAddExpLabel.textlocalize = "";
            kCharLevelUp.SetActive(false);
        }

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
            kCardAddExpLabel[i].textlocalize = "";
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            if (cardlist[i] == null)
            {

                kCardOn[i].SetActive(false);
                kCardOff[i].SetActive(true);

            }
            else
            {

                kCardOn[i].SetActive(true);
                kCardOff[i].SetActive(false);

                kCardIconBGSpr[i].spriteName = "itembgSlot_weapon_" + cardlist[i].TableData.Grade.ToString();
                kCardIconTex[i].mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Item/{0}.png", cardlist[i].TableData.Icon));
                //kCardIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("card", string.Format("Card/{0}_{1}.png", _carddata.TableData.Icon, GameSupport.GetCardImageNum(_carddata)));

                _curcardlevel[i] = gameresultdata.CardFavorBeforeLevel[i];
                _curcardexp[i] = gameresultdata.CardFavorBeforeExp[i];
                _gaincardexp[i] = gameresultdata.CardFavorBeforeExp[i];
                kCardGageAllGainExp[i] = -1;

                fillAmount = GameSupport.GetCardFavorLevelExpGauge(cardlist[i].TableID, _curcardlevel[i], _curcardexp[i]);
                kCardGaugeUnit[i].InitGaugeUnit(fillAmount);
                kCardLevelLabel[i].textlocalize = _curcardlevel[i].ToString();
                kCardLevelUp[i].SetActive(false);

            }
        }


        for (int i = 0; i < kGoodsUnitList.Count; i++)
            kGoodsUnitList[i].gameObject.SetActive(false);

        //레이드 재화 off
        _RaidPointGoodsUnit.gameObject.SetActive(false);
        _RaidGoldGoodsUnit.gameObject.SetActive(false);

        if ( mStageData.StageType != (int)eSTAGETYPE.STAGE_RAID ) {
            kGoodsUnitList[0].InitGoodsUnit( eGOODSTYPE.GOLD, gameresultdata.Goods[(int)eGOODSTYPE.GOLD] );
            kGoodsUnitList[1].InitGoodsUnit( eGOODSTYPE.ROOMPOINT, gameresultdata.Goods[(int)eGOODSTYPE.ROOMPOINT] );
            kGoodsUnitList[0].kIconSpr.spriteName = GameSupport.GetGoodsIconName( eGOODSTYPE.GOLD );
            kGoodsUnitList[1].kIconSpr.spriteName = GameSupport.GetGoodsIconName( eGOODSTYPE.ROOMPOINT );
        }

        _goodscount = 1;

        _rewarddatalist.Clear();

        for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++)
            _rewarddatalist.Add(GameInfo.Instance.RewardList[i]);

        for (int i = 0; i < _rewarddatalist.Count; i++)
            _rewarddatalist[i].bNew = false;

        

        _rewardlist.Clear();
        this._ItemListInstance.UpdateList(true, _bPreInit);
        if (_bPreInit)
            _bPreInit = false;

        kConfirmBtn.gameObject.SetActive(false);
        kRetryBtn.gameObject.SetActive(false);
        
        kOneStoreObj.SetActive(false);
        kEnemyInfoBtn.gameObject.SetActive(false);

        

        _multipleindex = GameInfo.Instance.SelectMultipleIndex;
        _multiGCFlag = GameInfo.Instance.MultibleGCFlag;
        SetTicketInfo();

        
        //결과창이 뜰때 까만화면을 꺼줍니다.
        LobbyUIManager.Instance.kBlackScene.SetActive(false);

        // 레이드 전용
        if( mStageData.StageType == (int)eSTAGETYPE.STAGE_RAID ) {
            _NormalObj.SetActive( false );
            _RaidObj.SetActive( true );

            _RaidPointGoodsUnit.InitGoodsUnit( eGOODSTYPE.RAIDPOINT, gameresultdata.Goods[(int)eGOODSTYPE.RAIDPOINT] );
            _RaidPointGoodsUnit.kIconSpr.spriteName = GameSupport.GetGoodsIconName( eGOODSTYPE.RAIDPOINT );

            _RaidGoldGoodsUnit.InitGoodsUnit( eGOODSTYPE.GOLD, gameresultdata.Goods[(int)eGOODSTYPE.GOLD] );
            _RaidGoldGoodsUnit.kIconSpr.spriteName = GameSupport.GetGoodsIconName( eGOODSTYPE.GOLD );

            for ( int i = 0; i < _CharInfos.Length; i++ ) {
                _CharInfos[i].Obj.SetActive( false );
            }

			for( int i = 0; i < GameInfo.Instance.RaidUserData.CharUidList.Count; i++ ) {
                _CharInfos[i].Obj.SetActive( true );

                CharData charData = GameInfo.Instance.GetCharData( GameInfo.Instance.RaidUserData.CharUidList[i] );
				InitCharExp( i, charData );
			}
		}
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
        SetTicketMultiple();
        FastQuestTicketRenewal();
    }

    private void InvokeShowUserRankUp()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.UserLevel, _curuserlevel);
        LobbyUIManager.Instance.ShowUI("UserRankUpPopup", true);
    }
    void FixedUpdate()
    {
        if (kFastQuestTicketEffect.isPlaying)
            return;
        
        if (kUserGageAllGainExp != -1 && bContinue == true)
        {
            int nowexp = _curuserexp + kUserGageAllGainExp;
            int level = GameSupport.GetAccountExpLevel(nowexp);

            float fillAmount = GameSupport.GetAccountLevelExpGauge(level, nowexp);
            kUserGaugeUnit.InitGaugeUnit(fillAmount);

            if (GameSupport.IsMaxAccountLevel(_curuserlevel))
                kUserGaugeUnit.SetText(FLocalizeString.Instance.GetText(221));
            else
                kUserGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT), (fillAmount * 100.0f)));

            kUserAddExpLabel.textlocalize = string.Format("+{0:#,##0}", kUserGageAllGainExp);
            kUserLevelLabel.textlocalize = _curuserlevel.ToString();//string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.RANK_TXT_NOW_LV), _curuserlevel);

            if (_curuserlevel != level)
            {
                bContinue = false;
                _curuserlevel = level;
                kUserLevelUp.SetActive(true);
                float delayTime = GameSupport.TweenerPlay(kUserLevelLabel.transform.parent.gameObject);
                //  랭크업 연출을 해야하는 상황
                kUserGaugeUnit.InitGaugeUnit(0);
                //  랭크업 표시만큼 딜레이
                Invoke("InvokeShowUserRankUp", delayTime);
                //  랭크업 팝업 활성화 되는 동안 연출을 일시정지
            }
        }

        if (kCharGageAllGainExp != -1)
        {
            int nowexp = _curcharexp + kCharGageAllGainExp;
            int level = GameSupport.GetCharacterExpLevel(nowexp, _curchargrade);

            float fillAmount = GameSupport.GetCharacterLevelExpGauge(level, _curchargrade, nowexp);
            kCharGaugeUnit.InitGaugeUnit(fillAmount);

            if (GameSupport.IsMaxCharLevel(_curcharlevel, _curchargrade))
                kCharGaugeUnit.SetText(FLocalizeString.Instance.GetText(221));
            else
                kCharGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT), (fillAmount * 100.0f)));

            kCharAddExpLabel.textlocalize = string.Format("+{0:#,##0}", kCharGageAllGainExp);
            kCharLevelLabel.textlocalize = _curcharlevel.ToString();//GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curcharlevel);

            if (_curcharlevel != level)
            {
                if(_charData != null)
                    GameSupport.IsCharOpenSkillSlot(_charData, _curcharlevel, level);

                SoundManager.Instance.PlayUISnd(34);
                _curcharlevel = level;
                kCharLevelUp.SetActive(true);
                
                GameSupport.TweenerPlay(kCharLevelLabel.transform.parent.gameObject);
            }
        }

        if( mStageData.StageType == (int)eSTAGETYPE.STAGE_RAID ) {
            for( int i = 0; i < GameInfo.Instance.RaidUserData.CharUidList.Count; i++ ) {
                CharData charData = GameInfo.Instance.GetCharData( GameInfo.Instance.RaidUserData.CharUidList[i] );

                if( _CharInfos[i].ExpInfo.GageAllGainExp != -1 ) {
                    int exp = _CharInfos[i].ExpInfo.CurExp + _CharInfos[i].ExpInfo.GageAllGainExp;
                    int level = GameSupport.GetCharacterExpLevel( exp, _CharInfos[i].ExpInfo.CurGrade );

                    float fillAmount = GameSupport.GetCharacterLevelExpGauge( level, _CharInfos[i].ExpInfo.CurGrade, exp );
                    _CharInfos[i].GaugeUnit.InitGaugeUnit( fillAmount );

                    if( GameSupport.IsMaxCharLevel( _CharInfos[i].ExpInfo.CurLevel, _CharInfos[i].ExpInfo.CurGrade ) ) {
                        _CharInfos[i].GaugeUnit.SetText( FLocalizeString.Instance.GetText( 221 ) );
                    }
                    else {
                        _CharInfos[i].GaugeUnit.SetText( string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.PERCENT_ONETWO_POINT_TEXT ), ( fillAmount * 100.0f ) ) );
                    }

                    _CharInfos[i].AddExpLabel.textlocalize = string.Format( "+{0:#,##0}", _CharInfos[i].ExpInfo.GageAllGainExp );
                    _CharInfos[i].LevelLabel.textlocalize = _CharInfos[i].ExpInfo.CurLevel.ToString();

                    if( _CharInfos[i].ExpInfo.CurLevel != level ) {
                        if( charData != null ) {
                            GameSupport.IsCharOpenSkillSlot( charData, _CharInfos[i].ExpInfo.CurLevel, level );
                        }

                        SoundManager.Instance.PlayUISnd( 34 );

                        _CharInfos[i].ExpInfo.CurLevel = level;
                        _CharInfos[i].LevelUp.SetActive( true );

                        GameSupport.TweenerPlay( _CharInfos[i].LevelLabel.transform.parent.gameObject );
                    }
                }
            }
        }
        
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            if (cardlist[i] != null)
            {
                if (kCardGageAllGainExp[i] != -1)
                {
                    int nowexp = _curcardexp[i] + kCardGageAllGainExp[i];
                    int level = GameSupport.GetCardFavorLevel(cardlist[i].TableID, nowexp);

                    float fillAmount = GameSupport.GetCardFavorLevelExpGauge(cardlist[i].TableID, level, nowexp);
                    kCardGaugeUnit[i].InitGaugeUnit(fillAmount);
                    kCardAddExpLabel[i].textlocalize = string.Format("+{0:#,##0}", kCardGageAllGainExp[i]);
                    kCardLevelLabel[i].textlocalize = _curcardlevel[i].ToString();

                    if (_curcardlevel[i] != level)
                    {
                        _curcardlevel[i] = level;

                        kCardLevelUp[i].SetActive(true);
                        GameSupport.TweenerPlay(kCardLevelLabel[i].transform.parent.gameObject);
                        if (!_blevelupcardvoice)
                        {
                            CardBookData cardbookdata = GameInfo.Instance.GetCardBookData(cardlist[i].TableID);
                            if (cardbookdata != null)
                            {
                                if (GameSupport.IsCardOpenTerms_Favor(cardbookdata))
                                    VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.FavorMax, cardlist[i].TableID);
                                else
                                    VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.FavorUp, cardlist[i].TableID);

                                _blevelupcardvoice = true;
                            }
                        }

                    }
                }
            }
        }
        
    }

    public void OnClick_Retry()
    {
        /*
        int stageId = GameInfo.Instance.SelecteStageTableId;

        GameTable.Stage.Param stagetabledata = GameInfo.Instance.GameTable.FindStage(stageId);
        if (stagetabledata == null)
        {
            return;
        }
        */

        if (mStageData == null)
        {
            return;
        }

        int stageId = mStageData.ID;

        var stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == stageId);

        int ticket = mStageData.Ticket;
        //int multipleindex = GameInfo.Instance.SelectMultipleIndex;

        bool bclearmission = false;
        if (GameSupport.GetStageMissionCount(mStageData) == 0)
            bclearmission = true;
        else if (stagecleardata != null)
            bclearmission = stagecleardata.IsClearAll();

		if ( mStageData.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK ) {
			ticket *= GameInfo.Instance.GameConfig.MultipleList[_multipleindex];
			if ( !GameSupport.IsCheckTicketBP( ticket ) ) {
				return;
			}
		}
		else
        {
            if (mStageData.StageType == (int)eSTAGETYPE.STAGE_DAILY)
            {
                if (GameSupport.IsOnDayOfWeek(mStageData.TypeValue, (int)GameSupport.GetCurrentRealServerTime().DayOfWeek) == true)
                {
                    ticket = (int)((float)ticket * GameInfo.Instance.GameConfig.StageDailyCostTicketRate);
                }
            }

            //캠페인 체크
            GuerrillaCampData campdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_StageClear_APRateDown, mStageData.StageType);
            if (campdata != null)
            {
                ticket -= (int)((float)ticket * (float)((float)campdata.EffectValue / (float)eCOUNT.MAX_BO_FUNC_VALUE));
            }

            if (mStageData.TicketMultiple == 1 && bclearmission)
            {
                int mult = GameInfo.Instance.GameConfig.MultipleList[_multipleindex];
                int multrate = GameInfo.Instance.GameConfig.MultipleRewardRateList[_multipleindex];

                if (mStageData.StageType != (int)eSTAGETYPE.STAGE_EVENT_BONUS)
                {
                    GuerrillaCampData multigcdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Stage_Multiple, mStageData.StageType);
                    if (multigcdata != null)
                    {
                        if (_multiGCFlag)
                        {
                            mult = multigcdata.EffectValue;
                            _multipleindex = (int)eCOUNT.NONE;
                        }
                    }
                }

                ticket = ticket * mult;
            }

            if (!GameSupport.IsCheckTicketAP(ticket))
            {
                return;
            }
        }

        if (mStageData.StageType == (int)eSTAGETYPE.STAGE_EVENT)
        {
            int eventId = mStageData.TypeValue;

            int state = GameSupport.GetJoinEventState(eventId);
            if (state != (int)eEventState.EventPlaying)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3053));
                return;
            }
        }

        if (!GameSupport.IsCheckInven())
        {
            return;
        }

        long charUID = GameInfo.Instance.SeleteCharUID;

        CharData chardata = GameInfo.Instance.GetCharData(charUID);
        if (chardata == null)
        {
            return;
        }

        if (GameSupport.GetCharLastSkillSlotCheck(chardata))
        {
            _stageid = stageId;
            _seletecuid = charUID;
            //_multipleindex = multipleindex;

            chardata.EquipSkill[(int)eCOUNT.SKILLSLOT - 1] = (int)eCOUNT.NONE;
            GameInfo.Instance.Send_ReqApplySkillInChar(chardata.CUID, chardata.EquipSkill, OnSkillOutGameStart);
            return;
        }

        GameInfo.Instance.Send_ReqStageStart(stageId, charUID, _multipleindex, false, _multiGCFlag, null, OnNetGameStart);
    }

    public void OnNetGameStart(int result, PktMsgType pktmsg)
    {
        if (result != 0 || mStageData == null)
        {
            return;
        }

        mRetry = true;

        //int stageId = GameInfo.Instance.SelecteStageTableId;
        //GameTable.Stage.Param param = GameInfo.Instance.GameTable.FindStage(stageId);

        int stageId = mStageData.ID;

        if (!GameInfo.Instance.netFlag)
            GameInfo.Instance.MaxNormalBoxCount = Random.Range(mStageData.N_DropMinCnt, mStageData.N_DropMaxCnt + 1);

        UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.LobbyToStage);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, stageId);
        LobbyUIManager.Instance.ShowUI("LoadingPopup", false);
        AppMgr.Instance.LoadScene(AppMgr.eSceneType.Stage, mStageData.Scene);
    }
    
    public void OnClick_SubjugationBtn()
    {
        if (mStageData == null)
            return;

        int stageId = mStageData.ID;
        bool bclearmission = false;
        StageClearData stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == stageId);
        
        if (GameSupport.GetStageMissionCount(mStageData) == 0)
            bclearmission = true;
        else if (stagecleardata != null)
            bclearmission = stagecleardata.IsClearAll();
        
        int ticket = mStageData.Ticket;
        //int multipleindex = GameInfo.Instance.SelectMultipleIndex;
        // if (mStageData.TicketMultiple == 1 && bclearmission)
        //     ticket *= GameInfo.Instance.GameConfig.MultipleList[_multipleindex];

        int multiValue = GameInfo.Instance.GameConfig.MultipleList[_multipleindex];
        if (_multiGCFlag)
        {
            GuerrillaCampData multigcdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Stage_Multiple, mStageData.StageType);
            if (multigcdata != null)
            {
                multiValue = multigcdata.EffectValue;
            }
        }
        ticket *= multiValue;

        if (mStageData.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK)
        {
            if (!GameSupport.IsCheckTicketBP(ticket))
                return;
        }
        else
        {
            if (mStageData.StageType == (int)eSTAGETYPE.STAGE_DAILY)
            {
                if (GameSupport.IsOnDayOfWeek(mStageData.TypeValue, (int)GameSupport.GetCurrentRealServerTime().DayOfWeek))
                    ticket = (int)((float)ticket * GameInfo.Instance.GameConfig.StageDailyCostTicketRate);
            }

            //캠페인 체크
            GuerrillaCampData campdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_StageClear_APRateDown, mStageData.StageType);
            if (campdata != null)
                ticket -= (int)(ticket * (campdata.EffectValue / (float)eCOUNT.MAX_BO_FUNC_VALUE));

            if (!GameSupport.IsCheckTicketAP(ticket))
                return;
        }

        if (mStageData.StageType == (int)eSTAGETYPE.STAGE_EVENT)
        {
            if (GameSupport.GetJoinEventState(mStageData.TypeValue) != (int)eEventState.EventPlaying)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3053));
                return;
            }
        }
        
        if (!GameSupport.IsCheckInven())
            return;
        
        long charUid = GameInfo.Instance.SeleteCharUID;
        CharData chardata = GameInfo.Instance.GetCharData(charUid);
        if (chardata == null)
            return;

        ItemData item = GameInfo.Instance.ItemList.Find(x => x.TableID == GameInfo.Instance.GameConfig.FastQuestTicketID);
        int count = item == null ? 0 : -1;
        int maxMultiple = GameInfo.Instance.GameConfig.MultipleList[_multipleindex];
        if (_multiGCFlag)
        {
            GuerrillaCampData multigcdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Stage_Multiple, mStageData.StageType);
            if (multigcdata != null)
            {
                maxMultiple = multigcdata.EffectValue;
            }
        }
        
        if (item != null && item.Count < maxMultiple)
            count = item.Count;

        if (0 <= count)
        {
            ItemBuyMessagePopup.ShowItemBuyPopup(GameInfo.Instance.GameConfig.FastQuestTicketID, count, maxMultiple);
            return;
        }

        UISubjugationConfirmPopup popup = LobbyUIManager.Instance.GetUI("SubjugationConfirmPopup") as UISubjugationConfirmPopup;
        if (popup != null)
        {
            popup.SetValue(_multipleindex, kTicketLabel.text, _multiGCFlag);
            popup.SetUIActive(true);
        }
    }

    public void EnableFastQuestTicket()
    {
        _bFastQuestTicket = true;
    }
    
    public void OnSkillOutGameStart(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        Log.Show("Skill Out!!!", Log.ColorType.Red);
        GameInfo.Instance.Send_ReqStageStart(_stageid, _seletecuid, _multipleindex, false, _multiGCFlag, null, OnNetGameStart);
    }

    public void OnClick_Confirm()
    {
        if (kFastQuestTicketEffect.isPlaying)
            return;
        
        if (IsPlayAnimtion())
            return;

        if (m_coroutine == null)
        {
            if (_bFastQuestTicket) {
                LobbyUIManager.Instance.Renewal("TopPanel");

                //비밀 임무 빠른 티켓 이였다면, 비밀임무 캐릭터 선택 창 까지 띄워줌.
                if (mStageData.StageType == (int)eSTAGETYPE.STAGE_SECRET) {
                    UIStageDetailPopup popup = LobbyUIManager.Instance.GetUI("StageDetailPopup") as UIStageDetailPopup;
                    if (popup != null) {
                        popup.OnClick_SecretQuestChangeBtn();
                    }
                }
            }

            OnClickClose();

            if (GameSupport.IsInLobbyTutorial())
                GameSupport.ShowTutorial();
        }
        else
        {
            if (GameInfo.Instance.GameResultData.UserBeforeLevel != GameInfo.Instance.GameResultData.UserAfterLevel && GameInfo.Instance.GameResultData.UserAfterLevel != _curuserlevel)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.UserLevel, GameInfo.Instance.GameResultData.UserAfterLevel);
                LobbyUIManager.Instance.ShowUI("UserRankUpPopup", true);
                bContinue = false;
                _curuserlevel = GameInfo.Instance.GameResultData.UserAfterLevel;
                _buserexpgaugestop = true;
                //유저레벨만 정리
                EndResult(true);
            }
            else
            {
                EndResult(false);
            }
        }
    }

	IEnumerator ResultCoroutineExp() {
		GameResultData gameresultdata = GameInfo.Instance.GameResultData;

		float fopentime = GetOpenAniTime();
		if ( _bFastQuestTicket ) {
			SoundManager.Instance.PlayUISnd( 88 );
			fopentime += kFastQuestTicketEffect.main.duration;
		}

		yield return new WaitForSeconds( fopentime );

		SoundManager.Instance.PlayUISnd( 58 );

		if ( gameresultdata.UserBeforeLevel != gameresultdata.UserAfterLevel || gameresultdata.UserBeforeExp != gameresultdata.UserAfterExp ) {
			float t = 0.0f;
			float expValue = kUserGageAllGainExp;

			//WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
			while ( t < 1.0f ) {
				yield return mWaitForFixedUpdate;

                //  게이지바 연출중 연출을 멈추는 상황
                if ( bContinue == false ) {
                    continue;
                }

				if ( _buserexpgaugestop ) {
					t = 1.0f;
					break;
				}
				else {
					t += Time.fixedDeltaTime / GameInfo.Instance.GameConfig.ResultGaugeDuration;
					_gainuserexp = gameresultdata.UserAfterExp - gameresultdata.UserBeforeExp;
					kUserGageAllGainExp = (int)Mathf.Lerp( expValue, _gainuserexp, t );
				}
			}

			while ( GameInfo.Instance.GameResultData.UserAfterLevel != _curuserlevel || bContinue == false ) {
				yield return mWaitForFixedUpdate;
			}
		}


		//캐릭터 경험치
		CharData chardata = GameInfo.Instance.GetCharData( gameresultdata.CharUID );
		if ( chardata != null ) {
			if ( gameresultdata.CharBeforeLevel != gameresultdata.CharAfterLevel || gameresultdata.CharBeforeExp != gameresultdata.CharAfterExp ) {
				SoundManager.Instance.PlayUISnd( 58 );
				SetExpGainCharTween( GameInfo.Instance.GameConfig.ResultGaugeDuration );
				yield return new WaitForSeconds( GameInfo.Instance.GameConfig.ResultGaugeDuration );

				if ( gameresultdata.CharBeforeLevel != gameresultdata.CharAfterLevel ) {
					VoiceMgr.Instance.PlayChar( eVOICECHAR.GradeUp, chardata.TableID );
				}
			}
		}

		//카드 경험치
		WaitForSeconds waitForSeconds = new WaitForSeconds( GameInfo.Instance.GameConfig.ResultGaugeDuration );
		for ( int i = 0; i < (int)eCOUNT.CARDSLOT; i++ ) {
			CardData carddata = GameInfo.Instance.GetCardData( chardata.EquipCard[i] );
			if ( carddata != null ) {
				if ( gameresultdata.CardFavorBeforeLevel[i] != gameresultdata.CardFavorAfterLevel[i] || gameresultdata.CardFavorBeforeExp[i] != gameresultdata.CardFavorAfterExp[i] ) {
					SetExpGainCardTween( i, GameInfo.Instance.GameConfig.ResultGaugeDuration );
					yield return waitForSeconds;
				}
			}
		}

		//재화표시
		waitForSeconds = new WaitForSeconds( GameInfo.Instance.GameConfig.ResultGoodsDuration );

        if ( mStageData.StageType != (int)eSTAGETYPE.STAGE_RAID ) {
            for ( int i = 0; i < _goodscount; i++ ) {
                kGoodsUnitList[i].gameObject.SetActive( true );
                GameSupport.TweenerPlay( kGoodsUnitList[i].gameObject );
                SoundManager.Instance.PlayUISnd( 37 );
                yield return waitForSeconds;
            }
        }
        else {
            _RaidPointGoodsUnit.gameObject.SetActive( true );
            GameSupport.TweenerPlay( _RaidPointGoodsUnit.gameObject );

            _RaidGoldGoodsUnit.gameObject.SetActive( true );
            GameSupport.TweenerPlay( _RaidGoldGoodsUnit.gameObject );
            
            SoundManager.Instance.PlayUISnd( 37 );
            yield return waitForSeconds;
        }

		waitForSeconds = new WaitForSeconds( GameInfo.Instance.GameConfig.ResultItemDuration );
		for ( int i = 0; i < _rewardlist.Count; i++ ) {
			_rewardlist[i].Open();
			SoundManager.Instance.PlayUISnd( 39 );
			yield return waitForSeconds;
		}

        for ( int i = 0; i < _rewarddatalist.Count; i++ ) {
            _rewarddatalist[i].bNew = true;
        }

		_ItemListInstance.RefreshNotMove();

		LobbyUIManager.Instance.CheckAddSpecialPopup();
		LobbyUIManager.Instance.ShowAddSpecialPopup();

        //비밀임무 결과에서는 확인 창만 나온다.
        if (mStageData.StageType == (int)eSTAGETYPE.STAGE_SECRET) {
            kConfirmBtn.gameObject.SetActive(true);
            kOneStoreObj.SetActive(false);
        }
        else {
            kConfirmBtn.gameObject.SetActive(!_bFastQuestTicket);
            kOneStoreObj.SetActive(_bFastQuestTicket);
        }

		if ( !GameSupport.IsTutorial() && ( mStageData != null ) && ( mStageData.StageType != (int)eSTAGETYPE.STAGE_EVENT_BONUS ) ) {
			kRetryBtn.gameObject.SetActive( !_bFastQuestTicket && mStageData.StageType != (int)eSTAGETYPE.STAGE_SECRET && mStageData.StageType != (int)eSTAGETYPE.STAGE_RAID );
			kTicketPopupCount.SetActive( _bClearmission && mStageData.TicketMultiple == 1 );

			GuerrillaCampData guerrillaCampData = GameSupport.GetOnGuerrillaCampaignType( eGuerrillaCampaignType.GC_Stage_Multiple, mStageData.StageType );

			bool stageCheckFlag = mStageData.StageType != (int)eSTAGETYPE.STAGE_TIMEATTACK &&
								  mStageData.StageType != (int)eSTAGETYPE.STAGE_RAID &&
								  mStageData.StageType != (int)eSTAGETYPE.STAGE_SECRET;

			kMultibleGC.SetActive( _bClearmission && guerrillaCampData != null && stageCheckFlag );

			//EnemyInfoBtn Active
			var enemyInfoData = GameInfo.Instance.GameClientTable.FindHelpEnemyInfo( x => x.StageID == mStageData.ID && x.StageType == 1 );
			if ( enemyInfoData != null )
				kEnemyInfoBtn.gameObject.SetActive( true );
			else
				kEnemyInfoBtn.gameObject.SetActive( false );

			SetTicketMultiple();
			FastQuestTicketRenewal();
		}

		NotiCheck();

		m_coroutine = null;
		yield return null;
	}

	IEnumerator RaidCharResultCoroutineExp( int index, CharData charData ) {
        float aniTime = GetOpenAniTime();

        if( _bFastQuestTicket ) {
            SoundManager.Instance.PlayUISnd( 88 );
            aniTime += kFastQuestTicketEffect.main.duration;
        }

        yield return new WaitForSeconds( aniTime );
        SoundManager.Instance.PlayUISnd( 58 );

        // 캐릭터 경험치
        if( GameInfo.Instance.GameResultData.RaidCharExpInfoArr[index].BeforeLevel != GameInfo.Instance.GameResultData.RaidCharExpInfoArr[index].AfterLevel ||
            GameInfo.Instance.GameResultData.RaidCharExpInfoArr[index].BeforeExp != GameInfo.Instance.GameResultData.RaidCharExpInfoArr[index].AfterExp ) {

			SoundManager.Instance.PlayUISnd( 58 );
            Utility.StopCoroutine( this, ref _CharInfos[index].ExpInfo.Coroutine );

            float gaugeDuration = GameInfo.Instance.GameConfig.ResultGaugeDuration;

            _CharInfos[index].ExpInfo.GainExp = GameInfo.Instance.GameResultData.RaidCharExpInfoArr[index].AfterExp - GameInfo.Instance.GameResultData.RaidCharExpInfoArr[index].BeforeExp;
            _CharInfos[index].ExpInfo.Coroutine = StartCoroutine( Utility.UpdateCoroutineValue( ( x ) => _CharInfos[index].ExpInfo.GageAllGainExp = (int)x, _CharInfos[index].ExpInfo.GageAllGainExp, _CharInfos[index].ExpInfo.GainExp, gaugeDuration ) );

            yield return new WaitForSeconds( gaugeDuration );

			if( GameInfo.Instance.GameResultData.RaidCharExpInfoArr[index].BeforeLevel != GameInfo.Instance.GameResultData.RaidCharExpInfoArr[index].AfterLevel ) {
				VoiceMgr.Instance.PlayChar( eVOICECHAR.GradeUp, charData.TableID );
			}
		}

        yield return null;
    }

    private void SetExpGainUserTween(float ftime)
    {
        GameResultData gameresultdata = GameInfo.Instance.GameResultData;

        Utility.StopCoroutine(this, ref m_crUser);

        _gainuserexp = gameresultdata.UserAfterExp - gameresultdata.UserBeforeExp;
        m_crUser = StartCoroutine(Utility.UpdateCoroutineValue((x) => kUserGageAllGainExp = (int)x, kUserGageAllGainExp, _gainuserexp, ftime));
    }

    private void SetExpGainCharTween(float ftime)
    {
        GameResultData gameresultdata = GameInfo.Instance.GameResultData;

        Utility.StopCoroutine(this, ref m_crChar);
        
        _gaincharexp = gameresultdata.CharAfterExp - gameresultdata.CharBeforeExp;
        m_crChar = StartCoroutine(Utility.UpdateCoroutineValue((x) => kCharGageAllGainExp = (int)x, kCharGageAllGainExp, _gaincharexp, ftime));
    }

    private void SetExpGainCardTween(int i, float ftime)
    {
        GameResultData gameresultdata = GameInfo.Instance.GameResultData;

        Utility.StopCoroutine(this, ref m_crCard[i]);

        _gaincardexp[i] = gameresultdata.CardFavorAfterExp[i] - gameresultdata.CardFavorBeforeExp[i];
        m_crCard[i] = StartCoroutine(Utility.UpdateCoroutineValue((x) => kCardGageAllGainExp[i] = (int)x, kCardGageAllGainExp[i], _gaincardexp[i], ftime));
    }


	private void _UpdateItemListSlot( int index, GameObject slotObject ) {
		UIRewardBoxListSlot card = slotObject.GetComponent<UIRewardBoxListSlot>();
        if ( null == card ) {
            return;
        }

		RewardData data = null;
        if ( 0 <= index && _rewarddatalist.Count > index ) {
            data = _rewarddatalist[index];
        }

        if ( data == null ) {
            return;
		}

		card.ParentGO = this.gameObject;
		card.UpdateSlot( index, 0, data );

		if ( _cardCondiMissionClear ) {
			if ( index < GameInfo.Instance.GameConfig.MultipleList[GameInfo.Instance.SelectMultipleIndex] ) {
				card.kItemListSlot.SetCardTypeFlag( _cardCondiType, true );
			}
		}

        if ( data.bNew == false ) {
            _rewardlist.Add( card );
        }
	}

	private int _GetItemElementCount()
    {
        return _rewarddatalist.Count; //TempValue
    }

	private void EndResult( bool buser ) {
		GameResultData gameresultdata = GameInfo.Instance.GameResultData;

		Utility.StopCoroutine( this, ref m_crUser );
		kUserGageAllGainExp = -1;

		float fillAmount = GameSupport.GetAccountLevelExpGauge( gameresultdata.UserAfterLevel, gameresultdata.UserAfterExp );
		kUserGaugeUnit.InitGaugeUnit( fillAmount );

        if ( GameSupport.IsMaxAccountLevel( gameresultdata.UserAfterLevel ) ) {
            kUserGaugeUnit.SetText( FLocalizeString.Instance.GetText( 221 ) );
        }
        else {
            kUserGaugeUnit.SetText( string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.PERCENT_ONETWO_POINT_TEXT ), ( fillAmount * 100.0f ) ) );
        }
		
        kUserAddExpLabel.textlocalize = string.Format( "+{0:#,##0}", gameresultdata.UserAfterExp - gameresultdata.UserBeforeExp );
		kUserLevelLabel.textlocalize = gameresultdata.UserAfterLevel.ToString();

        if ( gameresultdata.UserAfterLevel != gameresultdata.UserBeforeLevel ) {
            kUserLevelUp.SetActive( true );
        }

        if ( buser ) {
            return;
        }

		kCharGageAllGainExp = -1;
        for ( int i = 0; i < (int)eCOUNT.CARDSLOT; i++ ) {
            kCardGageAllGainExp[i] = -1;
        }

		Utility.StopCoroutine( this, ref m_coroutine );
		Utility.StopCoroutine( this, ref m_crChar );

		if ( mStageData.StageType == (int)eSTAGETYPE.STAGE_RAID ) {
			for ( int i = 0; i < _CharInfos.Length; i++ ) {
				Utility.StopCoroutine( this, ref _CharInfos[i].ExpInfo.Coroutine );
			}
		}

		NotiCheck();

		CharData chardata = GameInfo.Instance.GetCharData( gameresultdata.CharUID );

		fillAmount = GameSupport.GetCharacterLevelExpGauge( gameresultdata.CharAfterLevel, chardata.Grade, gameresultdata.CharAfterExp );
		kCharGaugeUnit.InitGaugeUnit( fillAmount );
        
        if ( GameSupport.IsMaxCharLevel( gameresultdata.CharAfterLevel, chardata.Grade ) ) {
            kCharGaugeUnit.SetText( FLocalizeString.Instance.GetText( 221 ) );
        }
        else {
            kCharGaugeUnit.SetText( string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.PERCENT_ONETWO_POINT_TEXT ), ( fillAmount * 100.0f ) ) );
        }

		kCharAddExpLabel.textlocalize = string.Format( "+{0:#,##0}", gameresultdata.CharAfterExp - gameresultdata.CharBeforeExp );
		kCharLevelLabel.textlocalize = gameresultdata.CharAfterLevel.ToString();//GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curcharlevel);

        if ( gameresultdata.CharAfterLevel != gameresultdata.CharBeforeLevel ) {
            kCharLevelUp.SetActive( true );
        }

		for ( int i = 0; i < (int)eCOUNT.CARDSLOT; i++ ) {
			if ( cardlist[i] != null ) {
				Utility.StopCoroutine( this, ref m_crCard[i] );

				fillAmount = GameSupport.GetCardFavorLevelExpGauge( cardlist[i].TableID, gameresultdata.CardFavorAfterLevel[i], gameresultdata.CardFavorAfterExp[i] );
				kCardGaugeUnit[i].InitGaugeUnit( fillAmount );
				kCardAddExpLabel[i].textlocalize = string.Format( "+{0:#,##0}", gameresultdata.CardFavorAfterExp[i] - gameresultdata.CardFavorBeforeExp[i] );
				kCardLevelLabel[i].textlocalize = gameresultdata.CardFavorAfterLevel[i].ToString();

				if ( gameresultdata.CardFavorAfterLevel[i] != gameresultdata.CardFavorBeforeLevel[i] )
					kCardLevelUp[i].SetActive( true );
			}
		}

        //재화표시
        if ( mStageData.StageType != (int)eSTAGETYPE.STAGE_RAID ) {
            for ( int i = 0; i < _goodscount; i++ ) {
                if ( !kGoodsUnitList[i].gameObject.active ) {
                    kGoodsUnitList[i].gameObject.SetActive( true );
                    GameSupport.TweenerPlay( kGoodsUnitList[i].transform.parent.gameObject );
                }
            }
        }
        else {
            _RaidPointGoodsUnit.gameObject.SetActive( true );
            GameSupport.TweenerPlay( _RaidPointGoodsUnit.transform.parent.gameObject );

            _RaidGoldGoodsUnit.gameObject.SetActive( true );
            GameSupport.TweenerPlay( _RaidGoldGoodsUnit.transform.parent.gameObject );
        }

		for ( int i = 0; i < _rewardlist.Count; i++ ) {
            if ( !_rewardlist[i].IsOpen ) {
                _rewardlist[i].Open();
            }
		}

		SoundManager.Instance.PlayUISnd( 39 );

        for ( int i = 0; i < _rewarddatalist.Count; i++ ) {
            _rewarddatalist[i].bNew = true;
        }

		_ItemListInstance.RefreshNotMove();

		LobbyUIManager.Instance.CheckAddSpecialPopup();
		LobbyUIManager.Instance.ShowAddSpecialPopup();

        //비밀임무 결과에서는 확인 창만 나온다.
        if (mStageData.StageType == (int)eSTAGETYPE.STAGE_SECRET) {
            kConfirmBtn.gameObject.SetActive(true);
            kOneStoreObj.SetActive(false);
        }
        else {
            kConfirmBtn.gameObject.SetActive(!_bFastQuestTicket);
            kOneStoreObj.SetActive(_bFastQuestTicket);
        }

		if ( !GameSupport.IsInLobbyTutorial() && ( mStageData != null ) && ( mStageData.StageType != (int)eSTAGETYPE.STAGE_EVENT_BONUS ) ) {
			kRetryBtn.gameObject.SetActive( !_bFastQuestTicket && mStageData.StageType != (int)eSTAGETYPE.STAGE_SECRET && mStageData.StageType != (int)eSTAGETYPE.STAGE_RAID );
			kTicketPopupCount.SetActive( _bClearmission && mStageData.TicketMultiple == 1 );

			GuerrillaCampData guerrillaCampData = GameSupport.GetOnGuerrillaCampaignType( eGuerrillaCampaignType.GC_Stage_Multiple, mStageData.StageType );

			bool stageCheckFlag = mStageData.StageType != (int)eSTAGETYPE.STAGE_TIMEATTACK &&
								  mStageData.StageType != (int)eSTAGETYPE.STAGE_RAID &&
								  mStageData.StageType != (int)eSTAGETYPE.STAGE_SECRET;

			kMultibleGC.SetActive( _bClearmission && guerrillaCampData != null && stageCheckFlag );

			//EnemyInfoBtn Active
			var enemyInfoData = GameInfo.Instance.GameClientTable.FindHelpEnemyInfo( x => x.StageID == mStageData.ID && x.StageType == 1 );
            if ( enemyInfoData != null ) {
                kEnemyInfoBtn.gameObject.SetActive( true );
            }
            else {
                kEnemyInfoBtn.gameObject.SetActive( false );
            }

			SetTicketMultiple();
			FastQuestTicketRenewal();
		}
	}

	private void NotiCheck()
    {
        GameResultData gameresultdata = GameInfo.Instance.GameResultData;
        UserData userdata = GameInfo.Instance.UserData;
        CharData chardata = GameInfo.Instance.GetCharData(gameresultdata.CharUID);

        if (GameInfo.Instance.bFastStageClear)
        {
            if (GameInfo.Instance.GameConfig.StageTypeOpenIDList[(int)eSTAGETYPE.STAGE_DAILY] == gameresultdata.StageID)
                NoticePopup.ShowText(UINoticePopup.eTYPE.TEXT, FLocalizeString.Instance.GetText(4104), "", UINoticePopup.eRUNTYPE.NONE, 0, 0);

            if (GameInfo.Instance.GameConfig.StageTypeOpenIDList[(int)eSTAGETYPE.STAGE_EVENT] == gameresultdata.StageID)
                NoticePopup.ShowText(UINoticePopup.eTYPE.TEXT, FLocalizeString.Instance.GetText(4105), "", UINoticePopup.eRUNTYPE.NONE, 0, 0);
            
            if (GameInfo.Instance.GameConfig.StageTypeOpenIDList[(int)eSTAGETYPE.STAGE_SPECIAL] == gameresultdata.StageID)
                NoticePopup.ShowText(UINoticePopup.eTYPE.TEXT, FLocalizeString.Instance.GetText(4103), "", UINoticePopup.eRUNTYPE.NONE, 0, 0);

            if (GameInfo.Instance.GameConfig.StageTypeOpenIDList[(int)eSTAGETYPE.STAGE_TIMEATTACK] == gameresultdata.StageID)
                NoticePopup.ShowText(UINoticePopup.eTYPE.TEXT, FLocalizeString.Instance.GetText(4106), "", UINoticePopup.eRUNTYPE.NONE, 0, 0);
        }

        string icoNoticeSpriteName = "ico_Notice";

        if (gameresultdata.CharBeforeLevel != gameresultdata.CharAfterLevel)
        {
            //캐릭터 기술 개방
            List<GameTable.CharacterSkillPassive.Param> characterSkillPassiveParamList = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.CharacterID == chardata.TableID && (x.Type == (int)eCHARSKILLPASSIVETYPE.SELECT_NORMAL));
            characterSkillPassiveParamList.Sort((f, b) =>
            {
                if (f.ItemReqListID < b.ItemReqListID)
                {
                    return -1;
                }

                if (f.ItemReqListID > b.ItemReqListID)
                {
                    return 1;
                }

                return 0;
            });

            foreach (GameTable.CharacterSkillPassive.Param characterSkillPassiveParam in characterSkillPassiveParamList)
            {
                GameTable.ItemReqList.Param itemReqListParam = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == characterSkillPassiveParam.ItemReqListID);
                if (itemReqListParam == null)
                {
                    continue;
                }

                if (gameresultdata.CharBeforeLevel < itemReqListParam.LimitLevel && itemReqListParam.LimitLevel <= gameresultdata.CharAfterLevel)
                {
                    string text = string.Format(FLocalizeString.Instance.GetText(4108), FLocalizeString.Instance.GetText(chardata.TableData.Name), FLocalizeString.Instance.GetText(characterSkillPassiveParam.Name));
                    NoticePopup.ShowText(UINoticePopup.eTYPE.SPR, text, icoNoticeSpriteName, UINoticePopup.eRUNTYPE.CHARSKILL, (int)chardata.CUID, chardata.TableID);
                }
            }     

            //캐릭터 스킬 슬롯 개방
            for (int k = 1; k < (int)eCOUNT.SKILLSLOT; k++)
            {
                for (int lvCheck = gameresultdata.CharBeforeLevel; lvCheck <= gameresultdata.CharAfterLevel; lvCheck++)
                {
                    if (lvCheck == GameInfo.Instance.GameConfig.CharSkillSlotLimitLevel[k])
                    {
                        string str = string.Format("UserData_SlotEffect_{0}_{1}", chardata.TableData.ID.ToString(), k);
                        if (PlayerPrefs.HasKey(str))
                        {
                            string text = string.Format(FLocalizeString.Instance.GetText(4107), FLocalizeString.Instance.GetText(chardata.TableData.Name));
                            NoticePopup.ShowText(UINoticePopup.eTYPE.SPR, text, icoNoticeSpriteName, UINoticePopup.eRUNTYPE.CHARSKILL, (int)chardata.CUID, chardata.TableID);
                        }
                    }
                }
                    
            }
            //캐릭터 서포터 슬롯 개방
            for (int idx = 1; idx < (int)eCOUNT.CARDSLOT; idx++)
            {
                for (int lvCheck = gameresultdata.CharBeforeLevel; lvCheck <= gameresultdata.CharAfterLevel; lvCheck++)
                {
                    if (lvCheck == GameInfo.Instance.GameConfig.CharCardSlotLimitLevel[idx])
                    {
                        string text = string.Format(FLocalizeString.Instance.GetText(4109), FLocalizeString.Instance.GetText(chardata.TableData.Name));
                        NoticePopup.ShowText(UINoticePopup.eTYPE.SPR, text, icoNoticeSpriteName, UINoticePopup.eRUNTYPE.CHARCARD, (int)chardata.CUID, chardata.TableID);
                    }
                }
            }
            //캐릭터 서브 무기 슬롯 개방
            for (int idx = 1; idx < (int)eCOUNT.WEAPONSLOT; idx++)
            {
                for (int lvCheck = gameresultdata.CharBeforeLevel; lvCheck <= gameresultdata.CharAfterLevel; lvCheck++)
                {
                    if (lvCheck == GameInfo.Instance.GameConfig.CharWeaponSlotLimitLevel[idx])
                    {
                        string text = string.Format(FLocalizeString.Instance.GetText(4111), FLocalizeString.Instance.GetText(chardata.TableData.Name));
                        NoticePopup.ShowText(UINoticePopup.eTYPE.SPR, text, icoNoticeSpriteName, UINoticePopup.eRUNTYPE.CHARWEAPON, (int)chardata.CUID, chardata.TableID);
                    }
                }
            }
        }
    }

    public override bool IsBackButton()
    {
        if (LobbyUIManager.Instance.GetUI("UserRankUpPopup").gameObject.activeSelf)
            return false;

        OnClick_Confirm();
        return false;
    }

    public void OnClick_EnemyInfoBtn()
    {
        if (mStageData.StageType == (int)eSTAGETYPE.STAGE_EVENT_BONUS)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.StageID, mStageData.ID);
        }
        else
        {
            UIValue.Instance.SetValue(UIValue.EParamType.StageID, mStageData.ID);
        }

        UIValue.Instance.SetValue(UIValue.EParamType.StageType, 1);
        LobbyUIManager.Instance.ShowUI("EnemyInfoPopup", true);
    }

    public void OnClick_MinusBtn()
    {
        _multipleindex -= 1;
        if (_multipleindex <= 0)
            _multipleindex = 0;
        SetTicketMultiple();
        FastQuestTicketRenewal();
    }

    public void OnClick_PlusBtn()
    {
        _multipleindex += 1;
        if (_multipleindex >= GameInfo.Instance.GameConfig.MultipleList.Count)
            _multipleindex = GameInfo.Instance.GameConfig.MultipleList.Count - 1;
        SetTicketMultiple();
        FastQuestTicketRenewal();
    }

    private void SetTicketInfo()
    {
        kTicketIconSpr.spriteName = "Goods_Ticket_s";
        kMultipleTicketIconSpr.spriteName = "Goods_Ticket_s";

        _tickettype = eGOODSTYPE.AP;
        if (mStageData.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK)
        {
            _tickettype = eGOODSTYPE.BP;
            kTicketIconSpr.spriteName = "Goods_TimeAttack_s";
            kMultipleTicketIconSpr.spriteName = "Goods_TimeAttack_s";
        }

        TicketCheck();

        var stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == mStageData.ID);

        kTicketPopupCount.SetActive(false);
        kMultibleGC.SetActive(false);
        if (GameInfo.Instance.MultibleGCFlag)
            kMultiGCToggle.SetToggle((int)eCOUNT.NONE, SelectEvent.Code);
        else
            kMultiGCToggle.SetToggle((int)eCOUNT.NONE + 1, SelectEvent.Code);

        _bClearmission = false;
        if (GameSupport.GetStageMissionCount(mStageData) == 0)
            _bClearmission = true;
        else if (stagecleardata != null)
            _bClearmission = stagecleardata.IsClearAll();
    }

    private void TicketCheck()
    {
        _ticket = mStageData.Ticket;
        _bTicketcostdw = false;
        if (mStageData.StageType == (int)eSTAGETYPE.STAGE_DAILY)
        {
            if (GameSupport.IsOnDayOfWeek(mStageData.TypeValue, (int)GameSupport.GetCurrentRealServerTime().DayOfWeek) == true)
            {
                _ticket = (int)((float)_ticket * GameInfo.Instance.GameConfig.StageDailyCostTicketRate);
                _bTicketcostdw = true;
            }
        }

        //캠페인 체크
        GuerrillaCampData campdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_StageClear_APRateDown, mStageData.StageType);
        if (campdata != null && _tickettype == eGOODSTYPE.AP)
        {
            _ticket -= (int)((float)_ticket * (float)((float)campdata.EffectValue / (float)eCOUNT.MAX_BO_FUNC_VALUE));
            _bTicketcostdw = true;
        }
    }

    private void SetTicketMultiple()
    {
        TicketCheck();

        if (mStageData.TicketMultiple == 1 && _bClearmission)
        {
            int mult = GameInfo.Instance.GameConfig.MultipleList[_multipleindex];
            int multrate = GameInfo.Instance.GameConfig.MultipleRewardRateList[_multipleindex];

            kMultipleLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), mult);
            kMultipleLabel.color = GameInfo.Instance.GameConfig.MultipleRewardRateColor[_multipleindex];
            kMultipleRewardLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(229), multrate);
            kMultipleRewardLabel.color = GameInfo.Instance.GameConfig.MultipleRewardRateColor[_multipleindex];

            if (mStageData.StageType != (int)eSTAGETYPE.STAGE_EVENT_BONUS)
            {
                GuerrillaCampData multigcdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Stage_Multiple, mStageData.StageType);
                if (multigcdata != null)
                {
                    if (_multiGCFlag)
                    {
                        kMultipleLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), multigcdata.EffectValue);
                        kMultipleLabel.color = GameInfo.Instance.GameConfig.MultipleRewardRateColor[GameInfo.Instance.GameConfig.MultipleRewardRateColor.Count - 1];
                        kMultipleRewardLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(229), GameInfo.Instance.GameConfig.MultipleRewardRateList[(int)eCOUNT.NONE] * multigcdata.EffectValue);
                        kMultipleRewardLabel.color = GameInfo.Instance.GameConfig.MultipleRewardRateColor[GameInfo.Instance.GameConfig.MultipleRewardRateColor.Count - 1];

                        mult = multigcdata.EffectValue;
                        _multipleindex = (int)eCOUNT.NONE;
                        kTicketPlusBtn.isEnabled = false;
                        kTicketMinusBtn.isEnabled = false;
                    }
                    else
                    {
                        kTicketPlusBtn.isEnabled = true;
                        kTicketMinusBtn.isEnabled = true;
                    }
                }
                else
                {
                    _multiGCFlag = false;
                }
            }

            _ticket = _ticket * mult;
        }
        else
        {
            _multiGCFlag = false;
        }

        if (GameInfo.Instance.UserData.IsGoods(_tickettype, _ticket))
        {
            if (_bTicketcostdw)
                kTicketLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), _ticket);
            else
                kTicketLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), _ticket);
        }
        else
            kTicketLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_R), _ticket);
    }

    private bool OnMultibleGCToggleSelect(int nSelect, SelectEvent type)
    {
        _multiGCFlag = (nSelect > (int)eCOUNT.NONE) ? false : true;
        Log.Show(_multiGCFlag);

        if (type == SelectEvent.Click)
        {
            SetTicketMultiple();
            FastQuestTicketRenewal();
        }

        return true;
    }

    private void InitCharExp( int index, CharData charData ) {
        _CharInfos[index].NameSpr.spriteName = "Name_Horizontal_" + ( (ePlayerCharType)charData.TableData.ID ).ToString();

        _CharInfos[index].ExpInfo.CurLevel = GameInfo.Instance.GameResultData.RaidCharExpInfoArr[index].BeforeLevel;
        _CharInfos[index].ExpInfo.CurGrade = GameInfo.Instance.GameResultData.RaidCharExpInfoArr[index].BeforeGrade;
        _CharInfos[index].ExpInfo.CurExp = GameInfo.Instance.GameResultData.RaidCharExpInfoArr[index].BeforeExp;
        _CharInfos[index].ExpInfo.GainExp = GameInfo.Instance.GameResultData.RaidCharExpInfoArr[index].BeforeExp;
        _CharInfos[index].ExpInfo.GageAllGainExp = -1;

        float fillAmount = GameSupport.GetCharacterLevelExpGauge( _CharInfos[index].ExpInfo.CurLevel, _CharInfos[index].ExpInfo.CurGrade, _CharInfos[index].ExpInfo.CurExp );
        _CharInfos[index].GaugeUnit.InitGaugeUnit( fillAmount );

        if( GameSupport.IsMaxCharLevel( _CharInfos[index].ExpInfo.CurLevel, _CharInfos[index].ExpInfo.CurGrade ) ) {
            _CharInfos[index].GaugeUnit.SetText( FLocalizeString.Instance.GetText( 221 ) );
        }
        else {
            _CharInfos[index].GaugeUnit.SetText( string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.PERCENT_ONETWO_POINT_TEXT ), ( fillAmount * 100.0f ) ) );
        }

        _CharInfos[index].LevelLabel.textlocalize = _CharInfos[index].ExpInfo.CurLevel.ToString();
        _CharInfos[index].AddExpLabel.textlocalize = "";

        _CharInfos[index].LevelUp.SetActive( false );
    }
}