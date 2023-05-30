using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIStageDetailPopup : FComponent
{
    public GameObject kTitleStory;
    public GameObject kTitleDaily;
    public GameObject kChallenge;
    public GameObject CharacteristicObj;
    public GameObject kTimeAttack;
    public GameObject kTicketPopupCount;
    public GameObject kTimeAttackOn;
    public GameObject kTimeAttackOff;
    public UILabel kStoryNameLabel_00;
    public UILabel kStoryNameLabel_01;
    public UILabel kDailyNameLabel;
    public List<UILabel> kDayLabelList;
    public GameObject kDailyMark;
    public UILabel kCharLevel;
    public UISprite kCharGrade;
    
    public UISprite kCharNameSpr;
    public UITexture kCharTexture;
    public UIButton kCharChangeBtn;
    public UISprite kTicketIconSpr;
    public UILabel kTicketLabel;
    public FTab kDifficultyTab;
    public FTab TabBonusStageDifficult;
    public GameObject kChallengeClear;
    public List<UIChallengeUnit> kChallengeUnitList;
    public List<UIChallengeUnit> ListUnitCharacteristic;
    public UILabel kChallengeRewardLabel;
    public UIUserCharListSlot kUserCharListSlot;
    public UILabel kHighestScoreLabel;
    public UILabel kDateRemainLabel;
    public UILabel kTimeAttackRemainTimeDescLb;
    public UILabel kMultipleLabel;
    public UISprite kMultipleTicketIconSpr;
    public UILabel kMultipleRewardLabel;
    public UICampaignMarkUnit kCampaignMarkUnit;
    public UISprite kStorySpr;
    public UISprite kNoticeSpr;
    public UIButton kEnemyInfoBtn;
    
    [Header("Secret")]
    public GameObject kStageBoInfoObj;
    public GameObject kSecretObj;
    public FList kSecretChapterList;
    public FList kSecretItemList;
    public UIButton kCharSelectBtn;
    
    [Header("CardTypeCondi")]
    public GameObject kCardTypeCondiObj;
    public UILabel kCardTypeChallengeTitleLb;
    public UILabel kCardTypeChallengeDescLb;
    public UISprite kCardTypeSpr;
    private bool _cardEquipFlag = false;

    [Header("CardTeam")]
    public GameObject kPlayCharObj;
    public GameObject kPlaySupportObj;
    public List<GameObject> kCardTeamInfoObjList;
    public List<UILabel> kCardTeamInfoNameLabelList;
    public GameObject kCardTeamChangeBtn;
    public CardTeamToolTipPopup.eCardToolTipDir kCardTeamInfoPopupDir = CardTeamToolTipPopup.eCardToolTipDir.NONE;
    private int _cardFormationID = 0;
    
    [Header("FastQuestTicket")]
    public GameObject kStartBtnObj;
    public GameObject kOneStoreObj;
    public UILabel kSubjugationLabel;

    public GameObject kHandUnit;
    public GameObject kMultibleGC;
    public FToggle kMultiGCToggle;
    private bool _multiGCFlag = false;
    public UIButton kTicketPlusBtn;
    public UIButton kTicketMinusBtn;

    [SerializeField] private FList _ItemListInstance;

    public UIButton kPresetBtn;

    [Header( "[for Raid Prologue]" )]
    [SerializeField] private GameObject         _CharSelectObj;
    [SerializeField] private GameObject         _RaidCharSelectObj;
    [SerializeField] private UICharListSlot[]   _RaidCharListSlots;

    private List<bool> _isdifficultylist = new List<bool>();
    private List<GameTable.Random.Param> _dropitemlist = new List<GameTable.Random.Param>();
    private int _stageid = -1;
    private int _originstageid = -1;
    private GameTable.Stage.Param _stagetabledata;
    private StageClearData _stagecleardata;
    private long _seletecuid = -1;
    private List<bool> _difficultylist = new List<bool>();
    private int _difficulty;
    private bool _bmark;
    private int _multipleindex = 0;
    private int _finalticket = 0;

    private EventSetData _eventSetData;
    private TimeAttackClearData _timeattackcleardata;
    private int _stageCondiDropItemCnt = 0;

    // for Event bonus stage
    private List<BonusStageInfo>    mListEventBonusStageInfo    = new List<BonusStageInfo>();
    private UIGoodsPopup            mUIGoodsPopup               = null;

    private int _selectIndex = 0;
    private bool _secretStageInit = false;
    private List<GameTable.Stage.Param> _secretStageList = new List<GameTable.Stage.Param>();

    public List<GameTable.Random.Param> DropItemList { get { return _dropitemlist; } }

    // for Raid Prologue
    private List<long> mCharUidSortList = new List<long>();


    public override void Awake()
    {
        base.Awake();

        if (this._ItemListInstance == null) return;

        this._ItemListInstance.EventUpdate = this._UpdateItemListSlot;
        this._ItemListInstance.EventGetItemCount = this._GetItemElementCount;

        kSecretItemList.EventUpdate = _UpdateItemListSlot;
        kSecretItemList.EventGetItemCount = _GetItemElementCount;
        
        kDifficultyTab.EventCallBack = OnDifficultyTabSelect;
        TabBonusStageDifficult.EventCallBack = OnTabBonusStageDifficult;

        kMultiGCToggle.EventCallBack = OnMultibleGCToggleSelect;

        kSecretChapterList.EventUpdate = _UpdateSecretList;
        kSecretChapterList.EventGetItemCount = _GetSecretItemCount;
    }

    private void _UpdateSecretList(int index, GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        UISecretListSlot slot = obj.GetComponent<UISecretListSlot>();
        if (slot == null)
        {
            return;
        }

        if (_secretStageList.Count <= index)
        {
            return;
        }

        slot.UpdateSlot(index, _selectIndex, _secretStageList[index]);
    }

    private int _GetSecretItemCount()
    {
        return _secretStageList.Count;
    }

    public override void OnEnable()
    { 
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        _stageid = (int)UIValue.Instance.GetValue(UIValue.EParamType.StageID);
        _originstageid = _stageid;
        _stagetabledata = GameInfo.Instance.GameTable.FindStage(_stageid);
        _stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == _stageid);
        _multipleindex = 0;
        _multiGCFlag = false;

        var obj = UIValue.Instance.GetValue(UIValue.EParamType.StageCharCUID);
        if (obj == null)
            _seletecuid = GameInfo.Instance.UserData.MainCharUID;
        else
            _seletecuid = (long)obj;

        kPresetBtn.SetActive(_stagetabledata.StageType != (int)eSTAGETYPE.STAGE_SECRET);

        int nEventID = -1;
        if(_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_EVENT || _stagetabledata.StageType == (int)eSTAGETYPE.STAGE_EVENT_BONUS)
        {
            var eventId = UIValue.Instance.GetValue(UIValue.EParamType.EventID);
            if(eventId != null)
            {
                nEventID = (int)eventId;
                _eventSetData = GameInfo.Instance.GetEventSetData(nEventID);
            }
        }
        else if (_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK)
        {
            _timeattackcleardata = GameInfo.Instance.TimeAttackClearList.Find(x => x.TableID == _stagetabledata.ID);
        }
        _difficulty = _stagetabledata.Difficulty;

        mUIGoodsPopup = LobbyUIManager.Instance.GetUI<UIGoodsPopup>();
        if(mUIGoodsPopup)
        {
            mUIGoodsPopup.OnPostAnimationEvent = null;
        }

        _isdifficultylist.Clear();

        kStageBoInfoObj.SetActive(_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_SECRET);
        kSecretObj.SetActive(_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_SECRET);
        kPlayCharObj.SetActive(_stagetabledata.StageType != (int)eSTAGETYPE.STAGE_SECRET);
        kPlaySupportObj.SetActive(_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_SECRET);
        
        _ItemListInstance.gameObject.SetActive(_stagetabledata.StageType != (int)eSTAGETYPE.STAGE_SECRET);
        kSecretItemList.gameObject.SetActive(_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_SECRET);
        
        if (_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_SECRET)
        {
            kCharSelectBtn.isEnabled = true;
            kDifficultyTab.gameObject.SetActive(false);
            TabBonusStageDifficult.gameObject.SetActive(false);
        }
        else if (_stagetabledata.StageType != (int)eSTAGETYPE.STAGE_EVENT_BONUS)
        {
            kDifficultyTab.gameObject.SetActive(true);
            TabBonusStageDifficult.gameObject.SetActive(false);

            GameSupport.GetIsChapterSectionDifficultylist(_stagetabledata.Chapter, _stagetabledata.Section, ref _isdifficultylist, (eSTAGETYPE)_stagetabledata.StageType, nEventID);
            for (int i = 0; i < kDifficultyTab.kBtnList.Count; i++)
                kDifficultyTab.SetEnabled(i, _isdifficultylist[i]);

            kDifficultyTab.SetTab(_difficulty - 1, SelectEvent.Code);
        }
        else
        {
            kDifficultyTab.gameObject.SetActive(false);
            TabBonusStageDifficult.gameObject.SetActive(true);

            int possibleFirstStageIndex = 0;
            for (int i = 0; i < mListEventBonusStageInfo.Count; i++)
            {
                if(!mListEventBonusStageInfo[i].IsClear)
                {
                    possibleFirstStageIndex = i;
                    break;
                }
            }

            for (int i = 0; i < mListEventBonusStageInfo.Count; i++)
            {
                BonusStageInfo info = mListEventBonusStageInfo[i];
                TabBonusStageDifficult.SetEnabled(i, i == possibleFirstStageIndex, i < possibleFirstStageIndex);
            }

            TabBonusStageDifficult.SetTab(possibleFirstStageIndex, SelectEvent.Code);

            if (mUIGoodsPopup)
            {
                mUIGoodsPopup.OnPostAnimationEvent = HideBPTicketObj;
            }
        }

        kChallenge.SetActive(true);
        kTimeAttack.SetActive(false);
        kTicketPopupCount.SetActive(true);
        CharacteristicObj.SetActive(false);

        if (_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK)
        {
            kChallenge.SetActive(false);
        }
        else if(_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_EVENT_BONUS)
        {
            kChallenge.SetActive(false);
            kTicketPopupCount.SetActive(false);
            CharacteristicObj.SetActive(true);
        }

        if (kHandUnit != null)
        {
            kHandUnit.gameObject.SetActive(GameSupport.ShowLobbyStageHand(_stageid));
        }

		//Multible GC Check
		if ( GameInfo.Instance.MultibleGCFlag ) {
            kMultiGCToggle.SetToggle( (int)eCOUNT.NONE, SelectEvent.Code );
        }
		else {
			kMultiGCToggle.SetToggle( (int)eCOUNT.NONE + 1, SelectEvent.Code );
		}

		kMultibleGC.SetActive(false);

        GuerrillaCampData campdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Stage_Multiple, (int)eCOUNT.NONE);
        if (campdata != null)
        {
            if (kTicketPopupCount.activeSelf)
            {
                kMultibleGC.SetActive(true);
                if(GameInfo.Instance.MultibleGCFlag)
                    kMultiGCToggle.SetToggle((int)eCOUNT.NONE, SelectEvent.Code);
                else
                    kMultiGCToggle.SetToggle((int)eCOUNT.NONE + 1, SelectEvent.Code);
            }
        }

        if (kSecretObj.activeSelf)
        {
            kSecretChapterList.InitBottomFixing();
            kSecretChapterList.UpdateList();
        }
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        NotificationManager.Instance.CheckCampaignMark(kCampaignMarkUnit, _stagetabledata.StageType);

        kTitleStory.gameObject.SetActive(false);
        kTitleDaily.gameObject.SetActive(false);
        kDailyMark.gameObject.SetActive(false);
        kNoticeSpr.gameObject.SetActive(false);

        kCardTypeCondiObj.SetActive(false);

        if (_stagetabledata.StageType != (int)eSTAGETYPE.STAGE_SECRET)
        {
            kTitleStory.gameObject.SetActive(true);
            kStoryNameLabel_00.textlocalize = FLocalizeString.Instance.GetText(_stagetabledata.Desc);
            kStoryNameLabel_01.textlocalize = FLocalizeString.Instance.GetText(_stagetabledata.Name);
        }
        
        if (_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_DAILY)
        {
            _bmark = false;
            int today = (int)GameSupport.GetCurrentRealServerTime().DayOfWeek;
            if (GameSupport.IsOnDayOfWeek(_stagetabledata.TypeValue, today) == true)
                _bmark = true;
            if (_bmark)
            {
                kDailyMark.gameObject.SetActive(true);

                float posX = kCampaignMarkUnit.gameObject.activeSelf ? -270 : -190;
                kDailyMark.transform.localPosition = new Vector3(posX, kDailyMark.transform.localPosition.y, kDailyMark.transform.localPosition.z);
            }
            else
                kDailyMark.gameObject.SetActive(false);
        }

        for (int i = 0; i < kChallengeUnitList.Count; i++)
            kChallengeUnitList[i].gameObject.SetActive(false);

        eGOODSTYPE tickettype = eGOODSTYPE.AP;
        GameInfo.Instance.IsRaidPrologue = _stagetabledata.PlayerMode == 1;

        if (_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK)
        {
            tickettype = eGOODSTYPE.BP;

            kTicketIconSpr.spriteName = "Goods_TimeAttack_s";
            kMultipleTicketIconSpr.spriteName = "Goods_TimeAttack_s";

            kChallenge.SetActive(false);
            kTimeAttack.SetActive(true);

            //난이도 변경했을때 갱신.
            int timeAttackClearScore = 0;

            _timeattackcleardata = GameInfo.Instance.TimeAttackClearList.Find(x => x.TableID == _stagetabledata.ID);
            if (_timeattackcleardata == null)
            {
                kTimeAttackOn.SetActive(false);
                kTimeAttackOff.SetActive(true);

                timeAttackClearScore = 0;
            }
            else
            {
                if(_timeattackcleardata.HighestScoreRemainTime < GameSupport.GetCurrentServerTime())
                {
                    kTimeAttackOn.SetActive(false);
                    kTimeAttackOff.SetActive(true);

                    timeAttackClearScore = 0;
                }
                else
                {
                    kTimeAttackOn.SetActive(true);
                    kTimeAttackOff.SetActive(false);
                    FLocalizeString.SetLabel(kTimeAttackRemainTimeDescLb, string.Format(FLocalizeString.Instance.GetText(1044), GameInfo.Instance.GameConfig.TimeAttackModeRecordDay));
                    kUserCharListSlot.UpdateSlot(_timeattackcleardata);
                    kHighestScoreLabel.textlocalize = GameSupport.GetTimeHighestScore(_timeattackcleardata.HighestScore);
                    System.DateTime subDate = _timeattackcleardata.HighestScoreRemainTime.AddDays(-GameInfo.Instance.GameConfig.TimeAttackModeRecordDay);
                    kDateRemainLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1058), subDate.ToString());

                    timeAttackClearScore = _timeattackcleardata.HighestScore;
                }
            }

            UIValue.Instance.SetValue(UIValue.EParamType.TimeAttackClearScore, timeAttackClearScore);
        }
        else if(_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_EVENT_BONUS)
        {
            kChallenge.SetActive(false);
            kTimeAttack.SetActive(false);
            kChallengeClear.SetActive(false);

			kTicketIconSpr.spriteName = "Goods_Ticket_s";
			kMultipleTicketIconSpr.spriteName = "Goods_Ticket_s";

            RenewalCharacteristicUnit();
        }
        else if (_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_SECRET)
        {
            kChallenge.SetActive(false);
            kTicketIconSpr.spriteName = "Goods_Ticket_s";
            kMultipleTicketIconSpr.spriteName = "Goods_Ticket_s";
        }
        else
        {
            kTicketIconSpr.spriteName = "Goods_Ticket_s";
            kMultipleTicketIconSpr.spriteName = "Goods_Ticket_s";

            kChallenge.SetActive(true);
            kTimeAttack.SetActive(false);
            kChallengeClear.SetActive(false);
            
            if (_stagecleardata == null)
            {
                RenewalChallengeUnit(0, _stagetabledata.Mission_00, (int)eCOUNT.NONE);
                RenewalChallengeUnit(1, _stagetabledata.Mission_01, (int)eCOUNT.NONE);
                RenewalChallengeUnit(2, _stagetabledata.Mission_02, (int)eCOUNT.NONE);
            }
            else
            {
                RenewalChallengeUnit(0, _stagetabledata.Mission_00, _stagecleardata.Mission[0]);
                RenewalChallengeUnit(1, _stagetabledata.Mission_01, _stagecleardata.Mission[1]);
                RenewalChallengeUnit(2, _stagetabledata.Mission_02, _stagecleardata.Mission[2]);
                if( _stagecleardata.IsClearAll() )
                    kChallengeClear.SetActive(true);
            }
        }

        CharData chardata = GameInfo.Instance.GetCharData(_seletecuid);
        bool hasChars = true;

        kPresetBtn.SetActive( true );
        _CharSelectObj.SetActive( true );
        _RaidCharSelectObj.SetActive( false );
        
        if( GameInfo.Instance.IsRaidPrologue ) {
            SortCharUidByCombatPower();

            if( mCharUidSortList.Count <= 0 ) {
                hasChars = false;
            }
            else {
                kPresetBtn.SetActive( false );
                _CharSelectObj.SetActive( false );
                _RaidCharSelectObj.SetActive( true );

                string str = PlayerPrefs.GetString( "RaidPlayer1_CUID" );
                long cuid1 = Utility.SafeLongParse( str );

                str = PlayerPrefs.GetString( "RaidPlayer2_CUID" );
                long cuid2 = Utility.SafeLongParse( str );

                str = PlayerPrefs.GetString( "RaidPlayer3_CUID" );
                long cuid3 = Utility.SafeLongParse( str );

                if( cuid1 == 0 ) {
                    cuid1 = GameInfo.Instance.GetMainCharUID();
                    PlayerPrefs.SetString( "RaidPlayer1_CUID", cuid1.ToString() );
                }

                mCharUidSortList.Remove( cuid1 );

                if( cuid2 == 0 && mCharUidSortList.Count > 0 ) {
                    cuid2 = mCharUidSortList[0];
                    PlayerPrefs.SetString( "RaidPlayer2_CUID", cuid2.ToString() );
                }

                if( cuid2 > 0 ) {
                    mCharUidSortList.Remove( cuid2 );
                }

                if( cuid3 == 0 && mCharUidSortList.Count > 0 ) {
                    cuid3 = mCharUidSortList[0];
                    PlayerPrefs.SetString( "RaidPlayer3_CUID", cuid3.ToString() );
                }

                // 0
                CharData raidCharData = GameInfo.Instance.GetCharData( cuid1 );
                _RaidCharListSlots[0].UpdateSlot( UICharListSlot.ePos.RAID_PROLORGUE, 0, raidCharData );

                chardata = raidCharData;
                _seletecuid = cuid1;

                // 1
                GameInfo.Instance.RaidPrologueCharTidList.Clear();

				raidCharData = GameInfo.Instance.GetCharData( cuid2 );
				_RaidCharListSlots[1].UpdateSlot( UICharListSlot.ePos.RAID_PROLORGUE, 1, raidCharData );

                if( raidCharData != null ) {
                    GameInfo.Instance.RaidPrologueCharTidList.Add( raidCharData.TableID );
                }

				// 2
				raidCharData = GameInfo.Instance.GetCharData( cuid3 );
				_RaidCharListSlots[2].UpdateSlot( UICharListSlot.ePos.RAID_PROLORGUE, 2, raidCharData );

                if( raidCharData != null ) {
                    GameInfo.Instance.RaidPrologueCharTidList.Add( raidCharData.TableID );
                }
            }
		}

        if( !hasChars || !GameInfo.Instance.IsRaidPrologue ) {
            if( chardata != null ) {
                kCharTexture.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", "Icon/Char/ListSlot/ListSlot_" + chardata.TableData.Icon + "_" + chardata.EquipCostumeID.ToString() + ".png" );
                kCharGrade.spriteName = string.Format( "grade_{0}", chardata.Grade.ToString( "D2" ) );  //"grade_0" + chardata.Grade.ToString();
                kCharGrade.MakePixelPerfect();
                kCharGrade.transform.localScale = new Vector3( 0.5f, 0.5f, 1.0f );

                kCharLevel.textlocalize = string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.LEVEL_TXT_NOW_LV ), chardata.Level );

                kCharNameSpr.spriteName = "Name_Horizontal_" + ( (ePlayerCharType)chardata.TableData.ID ).ToString();

                if( GameSupport.CheckCharData( chardata ) ) {
                    kNoticeSpr.gameObject.SetActive( true );
                }
                else {
                    var weapondata = GameInfo.Instance.GetWeaponData(chardata.EquipWeaponUID);
                    if( weapondata != null ) {
                        if( GameSupport.CheckWeaponData( weapondata ) )
                            kNoticeSpr.gameObject.SetActive( true );
                    }

                    for( int i = 0; i < (int)eCOUNT.CARDSLOT; i++ ) {
                        var carddata = GameInfo.Instance.GetCardData(chardata.EquipCard[i]);
                        if( carddata != null ) {
                            if( GameSupport.CheckCardData( carddata ) )
                                kNoticeSpr.gameObject.SetActive( true );
                        }
                    }
                }
            }
        }
        

        _dropitemlist.Clear();

        //서포터 장착 조건 체크 (2020. 01. 28 교환형 이벤트에만 적용되어있음.)
        if (_stagetabledata.Condi_Type != (int)eSTAGE_CONDI.NONE && 
            _stagetabledata.Condi_Type != (int)eSTAGE_CONDI.NOT_CHECK_CONDI && 
            _stagetabledata.Condi_Type != (int)eSTAGE_CONDI.INFLUENCE_CONDI)
        {
            kChallenge.SetActive(false);
            kTimeAttack.SetActive(false);
            kChallengeClear.SetActive(false);

            kCardTypeCondiObj.SetActive(true);

            //장착한 서포터 갯수 체크
            _cardEquipFlag = GameSupport.GetCardCondiCheck(chardata, (eSTAGE_CONDI)_stagetabledata.Condi_Type, _stagetabledata.Condi_Value);

            kCardTypeSpr.spriteName = GameSupport.GetCardTypeSpriteName((eSTAGE_CONDI)_stagetabledata.Condi_Type);
            kCardTypeSpr.alpha = 1f;
            string strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.WHITE_TEXT_COLOR);
            if (!_cardEquipFlag)
            {
                strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR);
                kCardTypeSpr.alpha = 0.5f;
            }
            kCardTypeChallengeDescLb.textlocalize = string.Format(strHaveCntColor, string.Format(FLocalizeString.Instance.GetText(1425), _stagetabledata.Condi_Value));

            AddDropItemList(_stagetabledata.Condi_DropID);
            _stageCondiDropItemCnt = _dropitemlist.Count;
        }
        else if (_stagetabledata.Condi_Type == (int)eSTAGE_CONDI.NOT_CHECK_CONDI || _stagetabledata.Condi_Type == (int)eSTAGE_CONDI.INFLUENCE_CONDI)
        {
            AddDropItemList(_stagetabledata.Condi_DropID);
        }
            
        AddDropItemList(_stagetabledata.N_DropID);

        int ticket = _stagetabledata.Ticket;
        bool bticketcostdw = false;
        if (_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_DAILY)
        {
            if (GameSupport.IsOnDayOfWeek(_stagetabledata.TypeValue, (int)GameSupport.GetCurrentRealServerTime().DayOfWeek) == true)
            {
                ticket = (int)((float)ticket * GameInfo.Instance.GameConfig.StageDailyCostTicketRate);
                bticketcostdw = true;
            }
        }

        //캠페인 체크
        GuerrillaCampData campdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_StageClear_APRateDown, _stagetabledata.StageType);
        if (campdata != null && tickettype == eGOODSTYPE.AP)
        {
            ticket -= (int)((float)ticket * (float)((float)campdata.EffectValue / (float)eCOUNT.MAX_BO_FUNC_VALUE));
            bticketcostdw = true;
        }

        kTicketPopupCount.SetActive(false);
        kMultibleGC.SetActive(false);
        bool bclearmission = false;
        if (GameSupport.GetStageMissionCount(_stagetabledata) == 0)
            bclearmission = true;
        else if (_stagecleardata != null)
            bclearmission = _stagecleardata.IsClearAll();

        if (_stagetabledata.TicketMultiple == 1 && bclearmission)
        {
            int mult = GameInfo.Instance.GameConfig.MultipleList[_multipleindex];
            int multrate = GameInfo.Instance.GameConfig.MultipleRewardRateList[_multipleindex];

            kMultipleLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), mult);
            kMultipleLabel.color = GameInfo.Instance.GameConfig.MultipleRewardRateColor[_multipleindex];
            kMultipleRewardLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(229), multrate);
            kMultipleRewardLabel.color = GameInfo.Instance.GameConfig.MultipleRewardRateColor[_multipleindex];

            if (_stagetabledata.StageType != (int)eSTAGETYPE.STAGE_EVENT_BONUS)
            {
                kTicketPopupCount.SetActive(true);
                GuerrillaCampData multigcdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Stage_Multiple, _stagetabledata.StageType);
                if (multigcdata != null && _stagetabledata.StageType != (int)eSTAGETYPE.STAGE_TIMEATTACK)
                {
                    kMultibleGC.SetActive(true);
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
                    kMultibleGC.SetActive(false);
                    _multiGCFlag = false;

					kTicketPlusBtn.isEnabled = true;
					kTicketMinusBtn.isEnabled = true;
				}
            }

            ticket = ticket * mult;
        }
        else
        {
            kMultibleGC.SetActive(false);
            _multiGCFlag = false;
        }

        ticket = Mathf.Max( 0, ticket );

        if (GameInfo.Instance.UserData.IsGoods(tickettype, ticket))
        {
            if (bticketcostdw)
                kTicketLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), ticket);
            else
                kTicketLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), ticket);
        }
        else
            kTicketLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_R), ticket);

        kChallengeRewardLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), GameInfo.Instance.GameConfig.StageMissionClearCash);

        if (_ItemListInstance.gameObject.activeSelf)
        {
            _ItemListInstance.UpdateList();
        }
        if (kSecretItemList.gameObject.activeSelf)
        {
            kSecretItemList.UpdateList();
        }

        _finalticket = ticket;

        kStorySpr.gameObject.SetActive(false);
        if (GameSupport.IsShowStoryStage(_stagetabledata))
            kStorySpr.gameObject.SetActive(true);

        //EnemyInfoBtn Active
        int findStageType = 1;
        int findStageId = _stageid;
        if (_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_SECRET)
        {
            findStageType = 3;
            SecretQuestOptionData optionData = GameInfo.Instance.ServerData.SecretQuestOptionList.Find(x => x.GroupId == _stageid);
            if (optionData != null)
            {
                findStageId = optionData.GroupId * 100 + optionData.LevelId;
            }
        }
        GameClientTable.HelpEnemyInfo.Param enemyInfoData =
            GameInfo.Instance.GameClientTable.FindHelpEnemyInfo(x => x.StageID == findStageId && x.StageType == findStageType);
        if (enemyInfoData != null)
            kEnemyInfoBtn.gameObject.SetActive(true);
        else
            kEnemyInfoBtn.gameObject.SetActive(false);

        _cardFormationID = GameSupport.GetSelectCardFormationID();

        string cardText;
        if (_cardFormationID == (int)eCOUNT.NONE)
        {
            cardText = FLocalizeString.Instance.GetText(1617);
        }
        else
        {
            GameTable.CardFormation.Param cardFrm = GameInfo.Instance.GameTable.FindCardFormation(x => x.ID == _cardFormationID);
            cardText = FLocalizeString.Instance.GetText(cardFrm.Name);
        }
        foreach (UILabel label in kCardTeamInfoNameLabelList)
        {
            label.textlocalize = cardText;
        }

        bool bOneStoreActive = false;
        bool bUseTicket = (_stagetabledata?.UseFastQuestTicket ?? 0) == 1;
        bool bStageClearAll = _stagecleardata?.IsClearAll() ?? false;
        bool bUnlockRank = GameInfo.Instance.GameConfig.FastQuestUnlockRank <= GameInfo.Instance.UserData.Level;
        
        bOneStoreActive = bUseTicket && bStageClearAll && bUnlockRank;

        bool bStageSecret = _stagetabledata.StageType == (int)eSTAGETYPE.STAGE_SECRET;
        kStartBtnObj.SetActive(!bOneStoreActive && !bStageSecret);
        kOneStoreObj.SetActive(bOneStoreActive && !bStageSecret);
        kCharSelectBtn.SetActive(bStageSecret);
        if (bOneStoreActive)
        {
            ItemData item = GameInfo.Instance.ItemList.Find(x => x.TableID.Equals(GameInfo.Instance.GameConfig.FastQuestTicketID));
            int itemCount = item?.Count ?? 0;
            int multiple = GameInfo.Instance.GameConfig.MultipleList[_multipleindex];
            if (_multiGCFlag)
            {
                GuerrillaCampData multigcdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Stage_Multiple, _stagetabledata.StageType);
                if (multigcdata != null)
                {
                    multiple = multigcdata.EffectValue;
                }
            }
            eTEXTID textId = multiple <= itemCount ? eTEXTID.WHITE_TEXT_COLOR : eTEXTID.RED_TEXT_COLOR;
            kSubjugationLabel.textlocalize = FLocalizeString.Instance.GetText((int) textId, 
                FLocalizeString.Instance.GetText(236, itemCount, multiple));
        }

        if (_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_SECRET)
        {
            kSecretChapterList.RefreshNotMoveAllItem();
            kSecretChapterList.SpringSetFocus(_selectIndex, 0.5f);
        }
    }

    public void SetSelectSecretSlot(int index, bool disable)
    {
        if (_secretStageList.Count <= index)
        {
            return;
        }
        
        _selectIndex = index;
        _stagetabledata = _secretStageList[index];
        _stageid = _stagetabledata.ID;
        UIValue.Instance.SetValue(UIValue.EParamType.StageID, _stageid);

        kCharSelectBtn.isEnabled = !disable;

        Renewal(false);
    }

    public void SetSecretStage()
    {
        if (!_secretStageInit)
        {
            _secretStageInit = true;
            _secretStageList =
                GameInfo.Instance.GameTable.FindAllStage(x => x.StageType == (int)eSTAGETYPE.STAGE_SECRET);
        }

        _selectIndex = _secretStageList.Count - 1;
        foreach (GameTable.Stage.Param secretStage in _secretStageList)
        {
            if (GameInfo.Instance.StageClearList.Any(x => x.TableID == secretStage.ID))
            {
                continue;
            }

            _selectIndex = secretStage.Section - 1;
            break;
        }
        
        if (_selectIndex < _secretStageList.Count)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.StageID, _secretStageList[_selectIndex].ID);
        }
    }

    public void SetMultipleIndex(int multipleIndex, bool multiCGFlag)
    {
        if (GameInfo.Instance.GameConfig.MultipleList.Count <= multipleIndex)
        {
            return;
        }
        
        _multipleindex = multipleIndex;
        
        kMultiGCToggle.SetToggle(multiCGFlag ? 0 : 1, SelectEvent.Code);
    }
    
    public void SetEventBonusStageInfo(List<BonusStageInfo> listBonusStageInfo)
    {
        mListEventBonusStageInfo = listBonusStageInfo;
    }

    private void SetDayString(int index, int day)
    {
        int dow = (int)GameSupport.GetCurrentRealServerTime().DayOfWeek;
        
        kDayLabelList[index].textlocalize = FLocalizeString.Instance.GetText(190 + day);
        kDayLabelList[index].color = GameInfo.Instance.GameConfig.TextColor[0];

        if (GameSupport.IsOnDayOfWeek(_stagetabledata.TypeValue, day) == true)
        {
            kDayLabelList[index].color = GameInfo.Instance.GameConfig.TextColor[2];
            if (dow == day)
                _bmark = true;
        }
    }

    private void _UpdateItemListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIItemListSlot item = slotObject.GetComponent<UIItemListSlot>();
            if (null == item) break;
            item.ParentGO = this.gameObject;

            GameTable.Random.Param data = null;
            if (0 <= index && _dropitemlist.Count > index)
                data = _dropitemlist[index];

            if (data.ProductType == (int)eREWARDTYPE.WEAPON)         //무기
            {
                var tabledata = GameInfo.Instance.GameTable.FindWeapon(data.ProductIndex);
                if (tabledata != null)
                {
                    item.UpdateSlot(UIItemListSlot.ePosType.RewardTable, index, tabledata);
                }
            }
            else if (data.ProductType == (int)eREWARDTYPE.GEM)            //곡옥
            {
                var tabledata = GameInfo.Instance.GameTable.FindGem(data.ProductIndex);
                if (tabledata != null)
                {
                    item.UpdateSlot(UIItemListSlot.ePosType.RewardTable, index, tabledata);
                }
            }
            else if (data.ProductType == (int)eREWARDTYPE.CARD)
            {
                var tabledata = GameInfo.Instance.GameTable.FindCard(data.ProductIndex);
                if (tabledata != null)
                {
                    item.UpdateSlot(UIItemListSlot.ePosType.RewardTable, index, tabledata);
                }
            }
            else if (data.ProductType == (int)eREWARDTYPE.ITEM)
            {
                var tabledata = GameInfo.Instance.GameTable.FindItem(data.ProductIndex);
                if (tabledata != null)
                {
                    item.UpdateSlot(UIItemListSlot.ePosType.RewardTable, index, tabledata);
                    
                }
            }           
            else if (data.ProductType == (int)eREWARDTYPE.GOODS)
            {
                item.UpdateSlot(UIItemListSlot.ePosType.RewardTable, index, data);
            }

            if (_stagetabledata.Condi_Type != (int)eSTAGE_CONDI.NONE && _stagetabledata.Condi_Type != (int)eSTAGE_CONDI.NOT_CHECK_CONDI && index < _stageCondiDropItemCnt)
                item.SetCardTypeFlag((eSTAGE_CONDI)_stagetabledata.Condi_Type, _cardEquipFlag);

            string persent = FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONE_POINT_TEXT, GameSupport.GetDropPersent(data.GroupID, data.ProductType, data.ProductIndex));
            item.SetCountText(persent);
        } while (false);
    }

    private int _GetItemElementCount()
    {
        return _dropitemlist.Count; //TempValue
    }

    private bool OnDifficultyTabSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
            return false;

        var stagetabledata = (_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_EVENT) ?
            GameInfo.Instance.GameTable.FindStage(x => x.StageType == _stagetabledata.StageType && x.TypeValue == _stagetabledata.TypeValue && x.Chapter == _stagetabledata.Chapter && x.Section == _stagetabledata.Section && x.Difficulty == nSelect + 1) :
            GameInfo.Instance.GameTable.FindStage(x => x.StageType == _stagetabledata.StageType && x.Chapter == _stagetabledata.Chapter && x.Section == _stagetabledata.Section && x.Difficulty == nSelect + 1);

        if (stagetabledata == null)
            return false;

        //난이도 변경시 배수 클리어 초기화
        _multipleindex = 0;

        _difficulty = nSelect + 1;
        _stageid = stagetabledata.ID;
        _stagetabledata = stagetabledata;
        _stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == _stageid);

        UIValue.Instance.SetValue(UIValue.EParamType.StageID, _stageid);

        Renewal(true);

        return true;
    }

    private bool OnTabBonusStageDifficult(int nSelect, SelectEvent type)
    {
        if(type != SelectEvent.Code)
        {
            return false;
        }

        GameTable.Stage.Param find = GameInfo.Instance.GameTable.FindStage(x => x.StageType == (int)eSTAGETYPE.STAGE_EVENT_BONUS && 
                                                                                x.ID == _stagetabledata.ID + nSelect);
        if(find == null)
        {
            return false;
        }

        _stageid = find.ID;
        _stagetabledata = find;
        _stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == _stageid);

        Renewal(true);
        return true;
    }

    private void AddDropItemList(int dropid)
    {
        var list = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == dropid);
        for (int i = 0; i < list.Count; i++)
        {
            var data = _dropitemlist.Find(x => x.ProductType == list[i].ProductType && x.ProductIndex == list[i].ProductIndex);
            if (data == null)
                _dropitemlist.Add(list[i]);
            else
                data.ProductValue = list[i].ProductValue > data.ProductValue ? list[i].ProductValue : data.ProductValue;
        }
    }

    private void RenewalChallengeUnit(int index, int missionid, int clear)
    {
        GameClientTable.StageMission.Param missiondata = GameInfo.Instance.GameClientTable.StageMissions.Find(x => x.ID == missionid);
        if (missiondata == null)
        {
            kChallengeUnitList[index].gameObject.SetActive(false);
            return;
        }

        kChallengeUnitList[index].gameObject.SetActive(true);
        kChallengeUnitList[index].InitChallengeUnit(missiondata, clear);
    }

    private void RenewalCharacteristicUnit()
    {
        CharacteristicObj.SetActive(false);

        List<GameClientTable.StageBOSet.Param> findAll = GameInfo.Instance.GameClientTable.StageBOSets.FindAll(x => x.Group == _stagetabledata.StageBOSet);
        if(findAll == null || findAll.Count <= 0)
        {
            return;
        }

        CharacteristicObj.SetActive(true);
        
        for(int i = 0; i < ListUnitCharacteristic.Count; i++)
        {
            ListUnitCharacteristic[i].SetActive(false);
        }

        for(int i = 0; i < findAll.Count; i++)
        {
            if(i >= ListUnitCharacteristic.Count)
            {
                break;
            }

            UIChallengeUnit unit = ListUnitCharacteristic[i];
            unit.SetActive(true);

            unit.kIconSpr.gameObject.SetActive(false);
            unit.kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(findAll[i].Desc);
        }
    }

    private void HideBPTicketObj()
    {
        if(mUIGoodsPopup == null)
        {
            return;
        }

        mUIGoodsPopup.ShowBPTicket(false);
    }

    public void OnClick_BackBtn()
    {
        if (GameSupport.IsTutorial())
            return;

        if( _stagetabledata.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK && GameInfo.Instance.TimeAttackRankList.Count <= 0 ) {
            GameInfo.Instance.Send_ReqTimeAtkRankingList( OnNetTimeAtkRankingList );
        }
        else {
            OnClickClose();
        }
    }

    public void OnClick_CancleBtn()
    {
        OnClickClose();
    }

    public void OnClick_MinusBtn()
    {
        _multipleindex -= 1;
        if (_multipleindex <= 0)
            _multipleindex = 0;
        Renewal(true);
    }

    public void OnClick_PlusBtn()
    {
        _multipleindex += 1;
        if (_multipleindex >= GameInfo.Instance.GameConfig.MultipleList.Count)
            _multipleindex = GameInfo.Instance.GameConfig.MultipleList.Count-1;
        Renewal(true);
    }

    public void OnClick_StartBtn()
    {
        GameTable.Stage.Param stagedata = GameInfo.Instance.GameTable.FindStage(_stageid);
        if (stagedata == null)
            return;

        if (_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK)
        {
            if (!GameSupport.IsCheckTicketBP(_finalticket))
                return;
        }
        else
        {
            if (!GameSupport.IsCheckTicketAP(_finalticket))
                return;
        }

        if (_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_EVENT || _stagetabledata.StageType == (int)eSTAGETYPE.STAGE_EVENT_BONUS)
        {
            int eventId = _stagetabledata.TypeValue;

            int state = GameSupport.GetJoinEventState(eventId);
            if (state != (int)eEventState.EventPlaying)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3053));
                return;
            }
        }

        if (!GameSupport.IsCheckInven())
            return;

        if (GameSupport.IsEmptyInEquipMainWeapon(ePresetKind.STAGE, _seletecuid))
        {
            return;
        }

        CharData chardata = GameInfo.Instance.GetCharData(_seletecuid);
        if (chardata == null)
            return;

        UIValue.Instance.SetValue(UIValue.EParamType.StageID, _stageid);

        if (GameSupport.GetCharLastSkillSlotCheck(chardata))
        {
            chardata.EquipSkill[(int)eCOUNT.SKILLSLOT - 1] = (int)eCOUNT.NONE;
            GameInfo.Instance.Send_ReqApplySkillInChar(chardata.CUID, chardata.EquipSkill, OnSkillOutGameStart);
            return;
        }
        
        GameInfo.Instance.Send_ReqStageStart(_stageid, _seletecuid, _multipleindex, false, _multiGCFlag, null, OnNetGameStart);
    }
    
    public void OnClick_SubjugationBtn()
    {
        if (!GameSupport.IsCheckTicketAP(_finalticket) || !GameSupport.IsCheckInven())
        {
            return;
        }
        
        ItemData item = GameInfo.Instance.ItemList.Find(x => x.TableID == GameInfo.Instance.GameConfig.FastQuestTicketID);
        int count = item?.Count ?? 0;
        int maxMultiple = GameInfo.Instance.GameConfig.MultipleList[_multipleindex];
        if (_multiGCFlag)
        {
            GuerrillaCampData multigcdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Stage_Multiple, _stagetabledata.StageType);
            if (multigcdata != null)
            {
                maxMultiple = multigcdata.EffectValue;
            }
        }
        
        if (count <= (int)eCOUNT.NONE || count < maxMultiple)
        {
            ItemBuyMessagePopup.ShowItemBuyPopup(GameInfo.Instance.GameConfig.FastQuestTicketID, count, maxMultiple);
            return;
        }

        if (GameSupport.IsEmptyInEquipMainWeapon(ePresetKind.STAGE, _seletecuid))
        {
            return;
        }

        CharData charData = GameInfo.Instance.GetCharData(_seletecuid);
        if (charData == null)
        {
            return;
        }

        UISubjugationConfirmPopup popup = LobbyUIManager.Instance.GetUI("SubjugationConfirmPopup") as UISubjugationConfirmPopup;
        if (popup != null)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.StageID, _stageid);
            
            popup.SetValue(charData, _stageid, _multipleindex, _seletecuid, kTicketLabel.text, _multiGCFlag);
            popup.SetUIActive(true);
        }
    }
    
    public void OnClick_CharChangeBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.CharSeletePopupType, (int)eCharSelectFlag.STAGE);
        LobbyUIManager.Instance.ShowUI("CharSeletePopup", true);
    }

    public void OnClick_SecretQuestChangeBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.CharSeletePopupType, (int)eCharSelectFlag.SECRET_QUEST);
        LobbyUIManager.Instance.ShowUI("CharSeletePopup", true);
    }

    public void OnClick_CharInfoBtn()
    {
        CharData chardata = GameInfo.Instance.GetCharData(_seletecuid);
        if (chardata == null)
            return;

        LobbyUIManager.Instance.JoinStageID = (int)UIValue.Instance.GetValue(UIValue.EParamType.StageID);
        LobbyUIManager.Instance.JoinCharSeleteIndex = -1;

        UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, chardata.CUID);
        UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, chardata.TableData.ID);
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);

        OnClickClose();
        LobbyUIManager.Instance.HideUI("StageDetailPopup");
    }

    public void OnNetGameStart(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        int stageid = (int)UIValue.Instance.GetValue(UIValue.EParamType.StageID);
        var stagedata = GameInfo.Instance.GameTable.FindStage(stageid);

        if (!GameInfo.Instance.netFlag)
            GameInfo.Instance.MaxNormalBoxCount = UnityEngine.Random.Range(stagedata.N_DropMinCnt, stagedata.N_DropMaxCnt + 1);

        GameInfo.Instance.SelecteStageTableId = stageid;

        UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.LobbyToStage);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, stageid);
        LobbyUIManager.Instance.ShowUI("LoadingPopup", false);
        AppMgr.Instance.LoadScene(AppMgr.eSceneType.Stage, stagedata.Scene);
    }

    public void OnSkillOutGameStart(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        Log.Show("Skill Out!!!", Log.ColorType.Red);
        GameInfo.Instance.Send_ReqStageStart(_stageid, _seletecuid, _multipleindex, false, _multiGCFlag, null, OnNetGameStart);
    }

    public void OnClick_EnemyInfoBtn()
    {
        if (_stagetabledata.StageType == (int)eSTAGETYPE.STAGE_EVENT_BONUS)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.StageID, _originstageid);
        }
        else
        {
            UIValue.Instance.SetValue(UIValue.EParamType.StageID, _stageid);
        }

        int stageTypeNumber = 1;
        switch (_stagetabledata.StageType)
        {
            case (int)eSTAGETYPE.STAGE_SECRET:
                stageTypeNumber = 3;
                break;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.StageType, stageTypeNumber);
        LobbyUIManager.Instance.ShowUI("EnemyInfoPopup", true);
    }

    public void OnClick_StageBoInfoBtn()
    {
        MessagePopup.OK(FLocalizeString.Instance.GetText(1818), _stagetabledata.ID, null);
    }

    public void OnClick_CardTeamChangeBtn()
    {
        LobbyUIManager.Instance.ShowUI("ArmoryPopup", true);
    }

    public void OnClick_CardTeamInfoBtn()
    {
        if (_cardFormationID == (int)eCOUNT.NONE)
        {
            return;
        }

        int index = _stagetabledata.StageType != (int)eSTAGETYPE.STAGE_SECRET ? 0 : 1;
        if (kCardTeamInfoObjList.Count <= index)
        {
            return;
        }

        CardTeamToolTipPopup.Show(_cardFormationID, kCardTeamInfoObjList[index], kCardTeamInfoPopupDir);
    }

    private bool OnMultibleGCToggleSelect(int nSelect, SelectEvent type)
    {
		if ( type == SelectEvent.Enable ) {
			return false;
		}

		_multiGCFlag = (nSelect > (int)eCOUNT.NONE) ? false : true;
        Log.Show(_multiGCFlag);

        if (type == SelectEvent.Click)
            Renewal(true);

        return true;
    }

    public void OnClick_PresetBtn()
    {
        if (GameInfo.Instance.QuestPresetDatas != null)
        {
            OnNet_PresetList(0, null);
        }
        else
        {
            GameInfo.Instance.Send_ReqGetUserPresetList(ePresetKind.STAGE, _seletecuid, OnNet_PresetList);
        }
    }

    private void OnNet_PresetList(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        PktInfoUserPreset pktInfoUserPreset = pktmsg as PktInfoUserPreset;
        if (pktInfoUserPreset != null && pktInfoUserPreset.infos_.Count <= 0)
        {
            GameInfo.Instance.SetPresetData(ePresetKind.STAGE, -1, GameInfo.Instance.GameConfig.ContPresetSlot);
        }

        UIPresetPopup presetPopup = LobbyUIManager.Instance.GetUI<UIPresetPopup>("PresetPopup");
        if (presetPopup == null)
        {
            return;
        }

        presetPopup.SetPresetData(eCharSelectFlag.Preset, ePresetKind.STAGE, _seletecuid);
        presetPopup.SetUIActive(true);
    }

    private void OnNetTimeAtkRankingList( int result, PktMsgType pkt ) {
        LobbyUIManager.Instance.SetPanelType( ePANELTYPE.TIMEATTACK, false, true );
        OnClickClose();
	}

    public void OnClick_PresetInfoBtn()
    {
        UIBadgePresetPopup badgePresetPopup = LobbyUIManager.Instance.GetUI<UIBadgePresetPopup>("BadgePresetPopup");
        if (badgePresetPopup == null)
        {
            return;
        }

        badgePresetPopup.SetPresetData(_seletecuid);
        badgePresetPopup.SetUIActive(true);
    }

    private void SortCharUidByCombatPower() {
        mCharUidSortList.Clear();
        for( int i = 0; i < GameInfo.Instance.CharList.Count; i++ ) {
            mCharUidSortList.Add( GameInfo.Instance.CharList[i].CUID );
        }

        if( mCharUidSortList.Count > 0 ) {
            mCharUidSortList.Sort( delegate ( long lhs, long rhs ) {
                CharData lData = GameInfo.Instance.GetCharData( lhs );
                CharData rData = GameInfo.Instance.GetCharData( rhs );

                if( lData.CombatPower < rData.CombatPower ) {
                    return 1;
                }
                else if( lData.CombatPower > rData.CombatPower ) {
                    return -1;
                }

                return 0;
            } );
        }
    }
}
