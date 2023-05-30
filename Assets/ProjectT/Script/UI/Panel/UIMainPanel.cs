using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class UIMainPanel : FComponent
{
    public UISprite kMenuSpr_open;
    public UISprite kMenuSpr_close;
    //public UIBannerSlot kEventBannerSlot;
    public UIButton kCampaignBtn;
    public GameObject kFacility;
    public List<UIButton> kFacilityListBtn;
    public UIButton kEventBoardBtn;

    public UIGrid GridIcon;

    [Header("NoticeBottom")]
    public GameObject kNoticeRoot;
    public FTab kNoticeTab;
    [SerializeField] private FList _NoticeBaseListInstance;
    [SerializeField] private FList _NoticeFacilityListInstance;
    [SerializeField] private FList _NoticeBuffListInstance;

    [Header("SpecialBuy")]
    public GameObject kSpecialBuyBtn;
    public UITexture kSpecialTex;
    public UILabel kSpecialRemainTimeLb;
    private DateTime _specialRemainTime;
    private TimeSpan _specialRemainTimeSpan;

    [Header("MonthlyPack")]
    public GameObject kMonthlyPackObj;
    public GameObject kMonthlyPackNormalObj;
    public GameObject kMonthlyPackBuyObj;
    public UILabel kMonthlyPackEndTimeLabel;
    private MonthlyData _monthlyData = null;

    [Header("BackGround Change")]
    public GameObject kBackgruondChangeOff;
    public GameObject kBackgruondChangeOn;
    [SerializeField] private FList _BackgroundList;

    [Header("User Welcome")]
    public UIButton kUserWelcomeBtn;
    public UIButton kUserComebackBtn;
    public UIButton kUserPublicBtn;

    [Header("Influence")]
    public UIButton kInfluenceBtn;

    [Header("Pass")]
    public List<UISprite> kPassSprList;

    [Header("Trade Facility")]
    public UIButton kTradeFacilityBtn;

    private bool _bnoticeopen = false;
    private bool _bmenufacility = true;
    private bool _buihide = false;
    private Coroutine playAnimQue = null;

    public bool UIHide { get { return _buihide; } }

    [Header("None Label")]
    public UILabel kNoneLabel;

    public GameObject kHandUnit;

    [Header("Special Buy Popup")]
    public UIButton kSpecialBuyDailyBtn;
    public GameObject kSpecialBuyDailyObj;
    public UIGrid kSpecialBuyGrid;
    public FToggle kSpecialBuyToggle;
    public List<UISpecialPackageBtnUnit> kSpecialBuyBtnUnitList;
    
    private bool _bNoticeLoginBonus = false;
    private bool _bEnterAnimPlayOnce;
    
    private List<BuffEffectData> _userBuffEffectList;
    private List<BuffEffectData> _favorBuffEffectList = new List<BuffEffectData>();

    private Coroutine _lobbyEnterCor = null;
    
    private UnexpectedPackageData _specialBuyDailyData;
    
    public override void Awake()
    {
        base.Awake();

        if (this._NoticeBaseListInstance == null) return;

        this._NoticeBaseListInstance.EventUpdate = this._UpdateNoticeBaseListSlot;
        this._NoticeBaseListInstance.EventGetItemCount = this._GetNoticeBaseElementCount;

        if (this._NoticeFacilityListInstance == null) return;

        this._NoticeFacilityListInstance.EventUpdate = this._UpdateNoticeFacilityListSlot;
        this._NoticeFacilityListInstance.EventGetItemCount = this._GetNoticeFacilityElementCount;

        if (this._NoticeBuffListInstance == null) return;
        this._NoticeBuffListInstance.EventUpdate = this._UpdateNoticeBuffListSlot;
        this._NoticeBuffListInstance.EventGetItemCount = this._GetNoticeBuffElementCount;

        kNoticeTab.EventCallBack = OnTabkNoticeSelect;


        if (this._BackgroundList == null) return;
        this._BackgroundList.EventUpdate = this._UpdateBackgroundListSlot;
        this._BackgroundList.EventGetItemCount = this._UpdateBackgroundCount;
        
        kSpecialBuyToggle.EventCallBack = SpecialBuyToggleCallBack;
    }

    public override void OnEnable()
    {
        LobbyUIManager.Instance.JoinStageID = -1;
        LobbyUIManager.Instance.JoinCharSeleteIndex = -1;
        UIValue.Instance.RemoveValue(UIValue.EParamType.ArenaCharInfoFlag);

        _bEnterAnimPlayOnce = false;
        _lobbyEnterCor = null;
        InitComponent();
        
        CheckPackageTime();
        
        base.OnEnable();
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		CancelInvoke("OnClick_NoticeBtn");
        kSpecialBuyToggle.SetToggle(0, SelectEvent.Code);
        base.OnDisable();
    }
    
    public override void InitComponent()
    {
        if (kNoneLabel != null)
            kNoneLabel.SetActive(false);

        bool isEventBoard = false;
        foreach (GameClientTable.EventPage.Param eventPage in GameInfo.Instance.GameClientTable.EventPages)
        {
            switch ((eLobbyEventType)eventPage.Type)
            {
                case eLobbyEventType.Bingo:
                    {
                        GameTable.BingoEvent.Param bingoEvent = LobbyUIManager.Instance.GetBingoEvent(eventPage);
                        if (bingoEvent == null)
                        {
                            continue;
                        }
                    }
                    break;
                case eLobbyEventType.Achieve:
                    {
                        GameTable.AchieveEvent.Param achieveEvent = LobbyUIManager.Instance.GetAchieveEvent(eventPage);
                        if (achieveEvent == null)
                        {
                            continue;
                        }
                    }
                    break;
            }

            isEventBoard = true;
            break;
        }

        kEventBoardBtn.SetActive(isEventBoard);

        kNoticeTab.SetTab(0, SelectEvent.Code);

        GameSupport.RemoveTimeOverUserBuffEffect();
        
        _userBuffEffectList = GameInfo.Instance.UserBuffEffectData.BuffEffectDataList.FindAll(x => x.TableData.UseType == 0);
        _favorBuffEffectList.Clear();
        List<BuffEffectData> tempFavorBuffEffectList = GameInfo.Instance.UserBuffEffectData.BuffEffectDataList.FindAll(x => x.TableData.UseType == 1);
        foreach (BuffEffectData buffEffectData in tempFavorBuffEffectList)
        {
            foreach (long uId in GameInfo.Instance.UserData.ArrFavorBuffCharUid)
            {
                if (uId <= 0)
                {
                    continue;
                }
                            
                int buffId = GameInfo.Instance.GetCharData(uId).TableData.PreferenceBuff;
                if (buffId == buffEffectData.BuffTableID)
                {
                    _favorBuffEffectList.Add(buffEffectData);
                    break;
                }
            }
        }
        
        _NoticeBaseListInstance.UpdateList();
        _NoticeFacilityListInstance.UpdateList();
        _NoticeBuffListInstance.UpdateList();

        _bmenufacility = true;
        kFacility.SetActive(false);
        kMenuSpr_open.gameObject.SetActive(true);
        kMenuSpr_close.gameObject.SetActive(false);

        RenderTargetChar.Instance.gameObject.SetActive(false);
        RenderTargetWeapon.Instance.gameObject.SetActive(false);
        _bNoticeLoginBonus = false;

        ///
        //EventBanner();
        if (!GameSupport.IsTutorial())
        {
            bool isLoginBonus = GameSupport.IsPossibleLoginBonus();
            if (isLoginBonus == true)
            {
                Log.Show("ui매니저에서 로그인 호출", Log.ColorType.Green, true, 18);
                GameInfo.Instance.Send_ReqReflashLoginBonus(OnNetReflashLoginBonus);
            }
            else if (LobbyUIManager.Instance.LoginBonusStep != eLoginBonusStep.End)
            {
                LobbyUIManager.Instance.ShowDailyLoginPopup(false);
            }
            else
            {
                if (GameInfo.Instance.UserData.ShowPkgPopup)
                {
                    GameInfo.Instance.Send_ReqUserPkgShowOff(OnNetUserPkgShowOff);
                }
                else
                {
                    if (!ReceiptChackFlag())
                    {
                        if (GameInfo.Instance.netFlag)
                            SendReqCommunityInfoGet();
                    }
                    
                    GameSupport.OpenWebView_ServerRelocate(FLocalizeString.Instance.GetText(6002), FLocalizeString.Instance.GetText(3229));
                }
            }

            SpecialBuyBtnCheck();
            SetMonthlyPack();
        }

        Transform dispatchTr = kFacility.transform.Find("Root5");

        if (dispatchTr != null)
        {
            dispatchTr.gameObject.SetActive(true);
        }

        _IsBackgroundEnable = false;
        SetStateBackgroundChangBtn();
        _BackgroundList.UpdateList();

        if (kInfluenceBtn)
        {
            if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
            {   
                kInfluenceBtn.SetActive(false);
            }
            else
            {
                if (GameSupport.GetCurrentInfluenceMissionSet(true) != null)
                    kInfluenceBtn.SetActive(true);
                else
                    kInfluenceBtn.SetActive(false);
            }
        }

        if (kTradeFacilityBtn)
        {
            kTradeFacilityBtn.transform.parent.gameObject.SetActive(true);
        }

        if (kHandUnit != null)
            kHandUnit.gameObject.SetActive(GameSupport.ShowLobbyStageHand());
        
        if (GameInfo.Instance.UnexpectedPackageDataDict.ContainsKey((int)eUnexpectedPackageType.FIRST_STAGE))
        {
            _specialBuyDailyData = GameInfo.Instance.UnexpectedPackageDataDict[(int)eUnexpectedPackageType.FIRST_STAGE].Find(x => x.IsPurchase);
        }
        
        bool specialBuyDailyActive = _specialBuyDailyData != null;
        if (specialBuyDailyActive)
        {
            int lastDay = 0;
            
            GameTable.UnexpectedPackage.Param unexpectedPackage = GameInfo.Instance.GameTable.FindUnexpectedPackage(_specialBuyDailyData.TableId);
            if (unexpectedPackage != null)
            {
                GameTable.Store.Param store = GameInfo.Instance.GameTable.FindStore(unexpectedPackage.ConnectStoreID);
                if (store != null)
                {
                    GameTable.Random.Param randomLast =
                        GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == store.ProductIndex).LastOrDefault();
                
                    if (randomLast != null)
                    {
                        lastDay = randomLast.Value;
                    }
                }
            }

            specialBuyDailyActive = false;
            for (int i = 0; i < lastDay; i++)
            {
                int flag = 1 << i;
                if ((_specialBuyDailyData.RewardBitFlag & flag) != flag)
                {
                    specialBuyDailyActive = true;
                    break;
                }
            }
        }
        
        kSpecialBuyDailyBtn.gameObject.SetActive(specialBuyDailyActive);
    }

	public bool ReceiptChackFlag() {
		string receipt = PlayerPrefs.GetString( "IAPBuyReceipt" );
		int buyID = PlayerPrefs.GetInt( "IAPBuyStoreID" );

		//Log.Show("@@#@##@#@# " + receipt + " / " + buyID);

		if ( buyID == 0 ) {
			//구매 결과 응답을 받으면 제거
			PlayerPrefs.DeleteKey( "IAPBuyReceipt" );
			PlayerPrefs.DeleteKey( "IAPBuyStoreID" );
			PlayerPrefs.Save();

			GameSupport.ShowReviewPopup();
			SendReqCommunityInfoGet();

			return false;
		}


        if ( null == receipt || string.IsNullOrEmpty( receipt ) ) {
            return false;
        }

		GameInfo.Instance.Send_ReqStorePurchaseInApp( receipt, buyID, OnRestoreIAP );
		return true;
	}

	public void OnRestoreIAP(int result, PktMsgType pktmsg)
    {
        Log.Show("Error : " + result);

        //구매 결과 응답을 받으면 제거
        PlayerPrefs.DeleteKey("IAPBuyReceipt");
        PlayerPrefs.DeleteKey("IAPBuyStoreID");
        PlayerPrefs.Save();

        GameSupport.ShowReviewPopup();
        SendReqCommunityInfoGet();
    }
    /*
    private void EventBanner()
    {
        kEventBannerSlot.gameObject.SetActive(false);
        for ( int i = 0; i < GameInfo.Instance.EventSetDataList.Count; i++ )
        {
            if( GameSupport.GetJoinEventState(GameInfo.Instance.EventSetDataList[i].TableID) == 1 )
            {
                //kEventBannerSlot.gameObject.SetActive(true);
                kEventBannerSlot.UpdateSlot(UIBannerSlot.ePosType.Event, 0, GameInfo.Instance.EventSetDataList[i]);
                return;
            }
        }

    }
    */

    public override void OnUIOpen()
    {
        base.OnUIOpen();
        if (NotificationManager.Instance.IsFailityComplete)
        {
            kNoticeTab.SetTab((int)eMainNoticeType.FACILITY, SelectEvent.Code);
            if (!_bnoticeopen)
            {
                if (IsPlayAnimtion(0))
                    Invoke("ShowNotice", GetOpenAniTime() + 0.2f);
                else
                    ShowNotice();
            }
        }
        else if (NotificationManager.Instance.NoticeBaseList.Count != 0)
        {
            kNoticeTab.SetTab((int)eMainNoticeType.BASE, SelectEvent.Code);
            if (!_bnoticeopen)
            {
                if (IsPlayAnimtion(0))
                    Invoke("ShowNotice", GetOpenAniTime() + 0.2f);
                else
                    ShowNotice();
            }
        }
        else
        {
            if (_bnoticeopen == true)
            {
                if (IsPlayAnimtion(0))
                    Invoke("HideNotice", GetOpenAniTime() + 0.2f);
                else
                    HideNotice();
            }

        }
    }



    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        bool bshow = false;

        for (int i = 0; i < GameInfo.Instance.ServerData.GuerrillaCampList.Count; i++)
        {
            if (GameSupport.IsGuerrillaCampaign(GameInfo.Instance.ServerData.GuerrillaCampList[i]))
                bshow = true;
        }

        //게릴라 미션 검사
        for (int i = 0; i < GameInfo.Instance.ServerData.GuerrillaMissionList.Count; i++)
        {
            if (GameSupport.IsGuerrillaMission(GameInfo.Instance.ServerData.GuerrillaMissionList[i]))
                bshow = true;
        }

        if (bshow)
            kCampaignBtn.gameObject.SetActive(true);
        else
            kCampaignBtn.gameObject.SetActive(false);
        
        //Pass 검사
        GameTable.PassSet.Param passSetsParam = GameInfo.Instance.GameTable.PassSets.Find(x => x.PassID == (int)ePassSystemType.Gold);
        if (passSetsParam != null)
        {
            DateTime netTime = GameInfo.Instance.GetNetworkTime();
            DateTime endTime = GameSupport.GetTimeWithString(passSetsParam.EndTime, true);
            string sprName = "ico_Pass";
            if (netTime <= endTime)
            {
                sprName = "ico_GoldenPass";
            }
            foreach (UISprite spr in kPassSprList)
            {
                spr.spriteName = sprName;
            }
        }

        SpecialBuyBtnCheck();
        SetMonthlyPack();

        _NoticeBaseListInstance.UpdateList();
        _NoticeFacilityListInstance.UpdateList();
        _NoticeBuffListInstance.UpdateList();


        if (kUserWelcomeBtn != null) kUserWelcomeBtn.SetActive(GameSupport.HasDailyEventState(eEventTarget.WELCOME));
        if (kUserComebackBtn != null) kUserComebackBtn.SetActive(GameSupport.HasDailyEventState(eEventTarget.COMEBACK));
        if (kUserPublicBtn != null) kUserPublicBtn.SetActive(GameSupport.HasDailyEventState(eEventTarget.PUBLIC));

        if (GridIcon)
        {
            GridIcon.Reposition();
        }
    }

    bool OnTabkNoticeSelect(int nSelect, SelectEvent type)
    {
        if (nSelect == (int)eMainNoticeType.BASE)
        {
            if (kNoneLabel != null)
            {
                kNoneLabel.SetActive(false);
            }

            _NoticeBaseListInstance.gameObject.SetActive(true);
            _NoticeFacilityListInstance.gameObject.SetActive(false);
            _NoticeBuffListInstance.gameObject.SetActive(false);

            if (NotificationManager.Instance.NoticeBaseList.Count <= 0)
            {
                if (kNoneLabel != null)
                {
                    kNoneLabel.SetActive(true);
                    kNoneLabel.textlocalize = FLocalizeString.Instance.GetText(1596);
                }
            }

        }
        else if (nSelect == (int)eMainNoticeType.BUFF)
        {
            if (kNoneLabel != null)
            {
                kNoneLabel.SetActive(false);
            }

            _NoticeBaseListInstance.gameObject.SetActive(false);
            _NoticeFacilityListInstance.gameObject.SetActive(false);
            _NoticeBuffListInstance.gameObject.SetActive(true);

            if (GameInfo.Instance.UserBuffEffectData.BuffEffectDataList.Count <= (int)eCOUNT.NONE ||
                GameSupport.GetActiveUserBuffEffect() <= (int)eCOUNT.NONE)
            {
                if (kNoneLabel != null)
                {
                    kNoneLabel.SetActive(true);
                    kNoneLabel.textlocalize = FLocalizeString.Instance.GetText(1596);
                }
            }
        }
        else if (nSelect == (int)eMainNoticeType.FACILITY)
        {
            if (kNoneLabel != null)
            {
                kNoneLabel.SetActive(false);
            }

            _NoticeBaseListInstance.gameObject.SetActive(false);
            _NoticeFacilityListInstance.gameObject.SetActive(true);
            _NoticeBuffListInstance.gameObject.SetActive(false);


            bool IsEnableNoneLable = false;
            IsEnableNoneLable = NotificationManager.Instance.NoticeFailityList.Count <= 0 && NotificationManager.Instance.NoticeDispatchList.Count <= 0;

            if (IsEnableNoneLable)
            {
                if (kNoneLabel != null)
                {
                    kNoneLabel.SetActive(true);
                    kNoneLabel.textlocalize = FLocalizeString.Instance.GetText(1596);
                }
            }
        }
        return true;
    }
    private void _UpdateNoticeBaseListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIMainNoticeListSlot card = slotObject.GetComponent<UIMainNoticeListSlot>();
            if (null == card) break;

            NoticeBaseData data = null;
            if (0 <= index && NotificationManager.Instance.NoticeBaseList.Count > index)
            {
                data = NotificationManager.Instance.NoticeBaseList[index];
            }
            card.ParentGO = this.gameObject;
            card.UpdateSlot(index, data);
        } while (false);
    }

    private int _GetNoticeBaseElementCount()
    {
        return NotificationManager.Instance.NoticeBaseList.Count;
    }

    private void _UpdateNoticeFacilityListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIMainNoticeFacilityListSlot card = slotObject.GetComponent<UIMainNoticeFacilityListSlot>();
            if (null == card) break;

            NoticeFacilityData data = null;
            NoticeDispatchData dispatchdata = null;

            if (0 <= index && NotificationManager.Instance.NoticeFailityList.Count > index)
            {
                data = NotificationManager.Instance.NoticeFailityList[index];

                card.ParentGO = this.gameObject;
                card.UpdateSlot(index, data);
            }
            else
            {
                int di = index - NotificationManager.Instance.NoticeFailityList.Count;
                if (di <= 0) di = 0;

                dispatchdata = NotificationManager.Instance.NoticeDispatchList[di];
                card.ParentGO = this.gameObject;
                card.UpdateSlot(index, dispatchdata);
            }


        } while (false);
    }

    private int _GetNoticeFacilityElementCount()
    {
        return NotificationManager.Instance.NoticeFailityList.Count + NotificationManager.Instance.NoticeDispatchList.Count;
    }

    private void _UpdateNoticeBuffListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIMainNoticeBuffListSlot slot = slotObject.GetComponent<UIMainNoticeBuffListSlot>();
            if (null == slot) break;


            BuffEffectData data = null;
            if (0 <= index && GameInfo.Instance.UserBuffEffectData.BuffEffectDataList.Count > index)
            {
                if (index < _userBuffEffectList.Count)
                {
                    data = _userBuffEffectList[index];
                }
                else
                {
                    if (1 == _userBuffEffectList.Count)
                    {
                        index -= _userBuffEffectList.Count;
                    }
                    else if (1 < _userBuffEffectList.Count)
                    {
                        index %= _userBuffEffectList.Count;
                    }

                    if ( index < _favorBuffEffectList.Count ) {
						data = _favorBuffEffectList[index];
					}
                }

                slot.ParentGO = this.gameObject;
                slot.UpdateSlot(data);
            }
        } while (false);
    }

    private int _GetNoticeBuffElementCount()
    {
        return GameSupport.GetActiveUserBuffEffect();
    }
    
    private bool SpecialBuyToggleCallBack(int nSelect, SelectEvent type)
    {
        if (type != SelectEvent.Click)
        {
            return true;
        }
        
        foreach (UISpecialPackageBtnUnit unit in kSpecialBuyBtnUnitList)
        {
            if (nSelect <= 0)
            {
                unit.SetActive(unit.transform.GetSiblingIndex() == 0);
            }
            else
            {
                if (_bnoticeopen)
                {
                    HideNotice();
                }
                
                unit.SetActive(unit.Rendering);
            }
        }
            
        return true;
    }
    
    private void CheckPackageTime()
    {
        foreach (UISpecialPackageBtnUnit unit in kSpecialBuyBtnUnitList)
        {
            unit.SetActive(false);
        }
        
        int siblingIndex = 0;
        List<UnexpectedPackageData> list = GameInfo.Instance.GetUnexpectedPackageFirstByType();
        
        kSpecialBuyDailyObj.SetActive(0 < list.Count);
        kSpecialBuyToggle.gameObject.SetActive(true);
        
        if (list.Count <= 1)
        {
            kSpecialBuyToggle.SetToggle(0, SelectEvent.Code);
            kSpecialBuyToggle.gameObject.SetActive(false);
        }
        
        foreach (UnexpectedPackageData data in list)
        {
            GameTable.UnexpectedPackage.Param tableData = GameInfo.Instance.GameTable.FindUnexpectedPackage(data.TableId);
            if (tableData == null)
            {
                continue;
            }

            int index = tableData.UnexpectedType - 1;
            if (kSpecialBuyBtnUnitList.Count <= index)
            {
                continue;
            }

            bool bEnable = true;
            if (kSpecialBuyToggle.kSelect == 0)
            {
                bEnable = siblingIndex == 0;
            }
            
            kSpecialBuyBtnUnitList[index].SetActive(bEnable);
            kSpecialBuyBtnUnitList[index].SetData(data, () => Invoke("CheckPackageTime", 0.1f));
            kSpecialBuyBtnUnitList[index].transform.SetSiblingIndex(siblingIndex);
            ++siblingIndex;
        }

        kSpecialBuyGrid.Reposition();
    }
    
    float _UpdateInterval = 0f;
    public void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GridIcon)
            {
                GridIcon.Reposition();
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Log.Show(GameSupport.IsEndCheckMonthly((int)eMonthlyType.NORMAL), Log.ColorType.Red);
            MessagePopup.CYN(FLocalizeString.Instance.GetText(1689), FLocalizeString.Instance.GetText(1690), FLocalizeString.Instance.GetText((int)eTEXTID.OK), FLocalizeString.Instance.GetText(915),
                () => { LobbyUIManager.Instance.ShowUI("CashBuyPopup", true); }, () =>
                {
                    MessagePopup.CYN(FLocalizeString.Instance.GetText(1691), FLocalizeString.Instance.GetText(1692), FLocalizeString.Instance.GetText((int)eTEXTID.OK), FLocalizeString.Instance.GetText(915),
                                    () => { GameSupport.PaymentAgreement_Package(2022); });
                });
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Log.Show(GameSupport.IsEndCheckMonthly((int)eMonthlyType.PREMIUM), Log.ColorType.Black);
            if (GameSupport.IsEndCheckMonthly((int)eMonthlyType.PREMIUM))
            {
                MessagePopup.OK(FLocalizeString.Instance.GetText(1691), FLocalizeString.Instance.GetText(1692), FLocalizeString.Instance.GetText((int)eTEXTID.OK),
                    () => { GameSupport.PaymentAgreement_Package(2022); });
            }
        }
#endif


        //if (Input.GetMouseButtonDown(0))
        if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Select))
        {
#if UNITY_EDITOR
            if (!UICamera.isOverUI)
            {
                if (_bmenufacility)
                    HideFacilityMenu();
            }
#else
            if(Input.touchCount > 0 && !UICamera.Raycast(Input.GetTouch(0).position))
            {
                if (_bmenufacility)
                    HideFacilityMenu();
            }
#endif
            /*if (!UICamera.isOverUI)
            {
                if (_bmenufacility)
                    HideFacilityMenu();
            }*/
        }

        if (_UpdateInterval <= 0f)
        {
            if (_specialRemainTimeSpan != null)
            {
                if (_specialRemainTimeSpan.Ticks > 0)
                    SetSpecialRemainTime();
                else
                    SpecialBuyBtnCheck();
            }

            SetMonthlyPack();
            _UpdateInterval = 1f;
        }
        else
        {
            _UpdateInterval -= Time.deltaTime;
        }
    }
    //-------------------------------------------------------------------------------------------------------------
    //
    //
    //
    //-------------------------------------------------------------------------------------------------------------
    public void OnClick_CharBtn()
    {
        UICharMainPanel charmainpanel = LobbyUIManager.Instance.GetUI<UICharMainPanel>("CharMainPanel");
        if (charmainpanel != null)
            charmainpanel.SetCharDefaultFilter();

        VoiceMgr.Instance.PlayChar(eVOICECHAR.UIChar, GameInfo.Instance.GetMainChar().TableID);
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARMAIN);
    }
    public void OnClick_ItemBtn()
    {
        VoiceMgr.Instance.PlayChar(eVOICECHAR.UIInven, GameInfo.Instance.GetMainChar().TableID);
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.ITEM);
    }
    public void OnClick_GachaBtn()
    {
        VoiceMgr.Instance.PlayChar(eVOICECHAR.UIGacha, GameInfo.Instance.GetMainChar().TableID);
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.GACHA);
    }
    public void OnClick_StoreBtn()
    {
        UIStorePanel storePanel = LobbyUIManager.Instance.GetUI<UIStorePanel>("StorePanel");
        if (storePanel != null)
        {
            storePanel.ResetData();
        }

        VoiceMgr.Instance.PlayChar(eVOICECHAR.UIStore, GameInfo.Instance.GetMainChar().TableID);
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.STORE);
    }
    public void OnClick_StoryBtn()
    {
        VoiceMgr.Instance.PlayChar(eVOICECHAR.UIMission, GameInfo.Instance.GetMainChar().TableID);
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.STORYMAIN);
    }

    public void OnClick_BookBtn()
    {
        Log.Show("OnClick_BookBtn");
        LobbyUIManager.Instance.ShowUI("BookMainPopup", true);
    }
    
    public void OnClick_SpecialBuyDailyPackageBtn()
    {
        UISpecialBuyDailyPopup specialBuyDailyPopup = LobbyUIManager.Instance.GetUI("SpecialBuyDailyPopup") as UISpecialBuyDailyPopup;
        if (specialBuyDailyPopup != null)
        {
            specialBuyDailyPopup.SetGameTable(_specialBuyDailyData.TableId);
            specialBuyDailyPopup.SetUIActive(true);
        }
    }

    public void OnClick_EventBoardBtn()
    {
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.EVENT_BOARD_MAIN);
    }

    //-------------------------------------------------------------------------------------------------------------
    //
    //
    //
    //-------------------------------------------------------------------------------------------------------------
    public void OnClick_MenuBtn()
    {
        if (_bmenufacility)
        {
            if (kFacility.activeSelf == true && kMenuSpr_close.gameObject.activeSelf)
            {
                HideFacilityMenu();
            }
            else
            {
                ShowFacilityMenu();
            }
        }
    }

    public void OnClick_NoticeBtn()
    {
        if (IsPlayAnimtion())
            return;

        StopCoroutineAni();

        if (_bnoticeopen == true)
        {
            HideNotice();
        }
        else
        {
            ShowNotice();
        }
    }

    public void ShowNotice()
    {
        if (kFacility.activeSelf == true)
        {
            playAnimQue = StartCoroutine(PlayAnimQue(null, 4, 3));
            kMenuSpr_close.gameObject.SetActive(false);
        }
        else
            PlayAnimtion(4);

        _bnoticeopen = true;
    }

    public void HideNotice()
    {
        PlayAnimtion(5);
        _bnoticeopen = false;
    }

    public void ShowFacilityMenu()
    {
        if (IsPlayAnimtion())
            return;

        StopCoroutineAni();

        if (_bnoticeopen == true)
        {
            playAnimQue = StartCoroutine(PlayAnimQue(kFacility, 2, 5));
            _bnoticeopen = false;
        }
        else
        {
            PlayAnimtion(2);
            kFacility.SetActive(true);
        }

        kMenuSpr_close.gameObject.SetActive(true);
    }
    public void HideFacilityMenu()
    {
        if (!kFacility.activeSelf)
            return;
        if (IsPlayAnimtion())
            return;

        PlayAnimtion(3);

        Invoke("ActiveFacilityHide", 0.1f);
    }


    public void ActiveFacilityHide()
    {
        kFacility.SetActive(false);
        kMenuSpr_close.gameObject.SetActive(false);
    }

    public override bool IsBackButton()
    {
        bool isBack = true;
        if (IsPlayAnimtion() == true)
        {
            isBack = false;
        }
        if (kFacility.activeSelf == true)
        {
            HideFacilityMenu();

            isBack = false;
        }
        if (_bnoticeopen == true)
        {
            PlayAnimtion(5);
            _bnoticeopen = false;

            isBack = false;
        }
        return isBack;
    }

    //-------------------------------------------------------------------------------------------------------------
    //
    //
    //
    //-------------------------------------------------------------------------------------------------------------
    public void OnClick_FacilityBtn(int index)
    {
        var data = GameInfo.Instance.FacilityList[index];
        //data == null 일때는 활성화된게 없을때..

        UIValue.Instance.SetValue(UIValue.EParamType.FacilityID, data.TableID);

        HideFacilityMenu();

        var obj = UIValue.Instance.GetValue(UIValue.EParamType.FacilityID);

        if (obj == null)
            return;

        var facilitydata = GameInfo.Instance.GetFacilityData((int)obj);
        var list = GameInfo.Instance.FacilityList.FindAll(x => x.TableData.ParentsID == facilitydata.TableID);

        LobbyDoorPopup.Show(MoveToFacilitys);
    }

    public void MoveToFacilitys()
    {
        var obj = UIValue.Instance.GetValue(UIValue.EParamType.FacilityID);
        if (obj == null)
        {
            Lobby.Instance.MoveToLobby();
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN);
        }
        else
        {
            var facilitydata = GameInfo.Instance.GetFacilityData((int)obj);
            var list = GameInfo.Instance.FacilityList.FindAll(x => x.TableData.ParentsID == facilitydata.TableID);
            var data = GameInfo.Instance.FacilityList[(int)obj - 1];
            Lobby.Instance.MoveToFacility(data.TableID);

            if (list.Count.Equals(1))
                LobbyUIManager.Instance.SetPanelType(ePANELTYPE.FACILITY);
            else
                LobbyUIManager.Instance.SetPanelType(ePANELTYPE.FACILITYITEM);
        }
    }

    public void OnClick_FacilityMoveRoomBtn()
    {
        /*
        RoomThemeSlotData roomslotdata = GameInfo.Instance.GetRoomThemeSlotData(GameInfo.Instance.UserData.RoomThemeSlot);
        // 테마슬롯이 없는 경우. 초기 Detail 정보를 저장해서 서버에 요청.
        if (roomslotdata == null)
        {
            //Debug.LogError(GameInfo.Instance.RoomThemaList[GameInfo.Instance.UserData.RoomThemeSlot]);
            int _tableId = GameInfo.Instance.RoomThemaList[GameInfo.Instance.UserData.RoomThemeSlot];
            roomslotdata = new RoomThemeSlotData(GameInfo.Instance.UserData.RoomThemeSlot, _tableId); 
            GameInfo.Instance.RoomThemeSlotList.Add(roomslotdata);
            ///Debug.LogError(GameInfo.Instance.RoomThemeSlotList.Count);
            GameInfo.Instance.Send_RoomThemeSlotSave(roomslotdata, GameInfo.Instance.RoomThemeFigureSlotList, OnNetRoomThemeSlotSave);
            return;
        }
        // 테마슬롯이 있는 경우 해당 테마슬롯의 디테일 정보를 서버에 요청.
        else
        {
            GameInfo.Instance.Send_RoomThemeSlotDetailInfo(GameInfo.Instance.UserData.RoomThemeSlot, OnNetRoomThemeSlotDetailInfo);
        }
        */

        // 무조건 테마슬롯의 디테일 정보를 받음
        if (!AppMgr.Instance.configData.m_Network && GameInfo.Instance.RoomThemeFigureDetailInfo)
        {
            JoinRoomTheme();
        }
        else
        {
            GameInfo.Instance.Send_RoomThemeSlotDetailInfo(GameInfo.Instance.UserData.RoomThemeSlot, OnNetRoomThemeSlotDetailInfo);
        }

    }

    public void OnClick_MailBox()
    {
        GameInfo.Instance.Send_ReqMailList(0, (uint)GameInfo.Instance.GameConfig.MaxMailCnt, false, OnShowMailPopup);
    }

    long _seletecuid = -1;
    public void OnClick_Friend()
    {
        Log.Show("Onclick_Friend");
        GameSupport.ShowFriendPopupWithSerachFriendList(string.Empty);
    }

    private void OnStartSpecialMode(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        int stageid = (int)UIValue.Instance.GetValue(UIValue.EParamType.StageID);
        var stagedata = GameInfo.Instance.GameTable.FindStage(stageid);

        if (!GameInfo.Instance.netFlag)
            GameInfo.Instance.MaxNormalBoxCount = UnityEngine.Random.Range(stagedata.N_DropMinCnt, stagedata.N_DropMaxCnt + 1);

        //  스테이지 패킷에서 셋팅된 값을 변경합니다.
        GameInfo.Instance.SeleteCharUID = _seletecuid;
        GameInfo.Instance.SelecteStageTableId = stageid;

        UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.LobbyToStage);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, stageid);
        LobbyUIManager.Instance.ShowUI("LoadingPopup", false);
        AppMgr.Instance.LoadScene(AppMgr.eSceneType.Stage, "event_school_room");
    }

    public void OnClick_WeeklyMission()
    {
        LobbyUIManager.Instance.ShowUI("WeeklyMissionPopup", true);
    }

    public void OnClick_AchievementBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.UserInfoPopup, 1);
        LobbyUIManager.Instance.ShowUI("UserInfoPopup", true);
    }

    public void OnClick_PackageBtn()
    {
        GameSupport.PaymentAgreement_Package();
    }

    public void OnClick_CampaignBtn()
    {
        LobbyUIManager.Instance.ShowUI("GuerrillaCampaignPopup", true);
    }

    public void OnClick_PVPArenaBtn()
    {
        Log.Show("OnClick_PVPArenaBtn");
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.ARENA);
    }

    public void OnClick_UiHideBtn()
    {
        if (_buihide == true)
            return;
        SetUIHide();
    }

    public void OnClick_CardDispatch()
    {   
        var data = GameInfo.Instance.GameTable.CardDispatchSlots.Find(x => x.Index == 1);
        if (data == null)
            return;

        if (!AppMgr.Instance.Review)
        {
            UserData userdata = GameInfo.Instance.UserData;
            if (userdata.Level < data.NeedRank)
                return;
        }

        HideFacilityMenu();
        //UICardDispatch 열기
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CARD_DISPATCH);
    }

    public void SetUIShow()
    {
        UITopPanel toppanel = LobbyUIManager.Instance.GetActiveUI<UITopPanel>("TopPanel");
        if (toppanel == null)
            return;

        if (toppanel.IsPlayAnimtion())
            return;
        if (IsPlayAnimtion())
            return;

        toppanel.PlayAnimtion(0);
        PlayAnimtion(0);
        //LobbyUIManager.Instance.ShowTemporaryActiveUI();
        _buihide = false;
    }
    public void SetUIHide()
    {
        UITopPanel toppanel = LobbyUIManager.Instance.GetActiveUI<UITopPanel>("TopPanel");
        if (toppanel == null)
            return;

        if (toppanel.IsPlayAnimtion())
            return;
        if (IsPlayAnimtion())
            return;

        toppanel.PlayAnimtion(1);
        PlayAnimtion(1);
        //LobbyUIManager.Instance.HideTemporaryActiveUI(null);
        _buihide = true;
    }


    void OnNetRoomThemeSlotDetailInfo(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        JoinRoomTheme();
    }

    void OnNetRoomThemeSlotSave(int result, PktMsgType pktmsg)
    {
        //Debug.LogError(result);
        if (result != 0)
            return;
    }


    private void JoinRoomTheme()
    {
        GameInfo.Instance.UseRoomDataCopySelRoomSlot();
        if (GameInfo.Instance.UseRoomThemeData.TableData == null)
        {
            Debug.LogError("Can not find RoomSlotData");
            return;
        }
        Lobby.Instance.MoveToRoom(GameInfo.Instance.UseRoomThemeData.TableID);
    }

    public void OnClick_PassBtn()
    {
        LobbyUIManager.Instance.ShowUI("PassMissionPopup", true);
    }

    //-------------------------------------------------------------------------------------------------------------
    //
    //
    //
    //-------------------------------------------------------------------------------------------------------------


    /// <summary>
    ///  19/03/12 윤주석
    ///  연출을 순차적으로 애니메이션 되도록 합니다.
    /// </summary>
    /// <param name="activeObj"></param>
    /// <param name="NowPlayAniNum"></param>
    /// <param name="BeforePlayAniNum"></param>
    /// <returns></returns>
    private IEnumerator PlayAnimQue(GameObject activeObj, int NowPlayAniNum, int BeforePlayAniNum = 0)
    {
        if (BeforePlayAniNum != 0)
        {
            float waitTime = PlayAnimtion(BeforePlayAniNum) + 0.1f;

            yield return new WaitForSeconds(waitTime);

            PlayAnimtion(NowPlayAniNum);
        }

        if (activeObj != null)
            activeObj.SetActive(true);
    }

    /// <summary>
    ///  19/03/12 윤주석
    ///  코루틴을 종료 합니다.
    /// </summary>
    private void StopCoroutineAni()
    {
        if (playAnimQue != null)
        {
            StopCoroutine(playAnimQue);
            playAnimQue = null;
        }
    }

    private void OnShowMailPopup(int result, PktMsgType pktmsg)
    {
        LobbyUIManager.Instance.ShowUI("MailBoxPopup", true);

        if (GameSupport.IsTutorial())
        {
            // 메일 버튼을 누르는 시점에서 튜토리얼은 끝난거
            UITutorialPopup popup = LobbyUIManager.Instance.GetActiveUI<UITutorialPopup>("TutorialPopup");
            if (popup)
            {
                popup.HideTutorialSkipButton();
            }

            GameSupport.TutorialNext();
        }
    }

    public void OnNetReflashLoginBonus(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        Debug.LogWarning(string.Format("###로그인###   로그인 그룹은 {0} | 해당 로그인 카운트는 {1}",
                         GameInfo.Instance.UserData.LoginBonusGroupID,
                         GameInfo.Instance.UserData.LoginBonusGroupCnt));

        /*
        //미보유 캐릭터 로비에서 한번 보여주는 기능
        List<BannerData> bannerList = GameInfo.Instance.ServerData.BannerList.FindAll(x => x.BannerType == (int)eBannerType.PACKAGE_BG);
        if(bannerList != null)
        {
            int charID = -1;
            for(int i = 0; i < bannerList.Count; i++)
            {
                GameClientTable.StoreDisplayGoods.Param clientTable = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == bannerList[i].BannerTypeValue);
                if (clientTable == null)
                    continue;

                if (!GameSupport.IsShowStoreDisplay(clientTable))
                    continue;

                if (clientTable.HideConType != "HCT_HaveChar")
                    continue;

                string haskey = "PACKAGE_CHAR_DIRECTOR_" + clientTable.HideConValue;

                if (PlayerPrefs.HasKey(haskey))
                    continue;

                charID = clientTable.HideConValue;
                PlayerPrefs.SetInt(haskey, charID);
                break;
            }

            if(charID > -1)
            {
                DirectorUIManager.Instance.PlayCharBuy(charID, EndCharInfoShowDailyLoginPopup);
                return;
            }
        }
        */
        _bNoticeLoginBonus = true;
        LobbyUIManager.Instance.LoginBonusStep = eLoginBonusStep.Step01;
        LobbyUIManager.Instance.IsOnceLoginBonusPopup = true;
        LobbyUIManager.Instance.ShowDailyLoginPopup();
        LobbyUIManager.Instance.Renewal("MainPanel");
        //SendReqCommunityInfoGet();
    }

    public void OnNetUserPkgShowOff(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        //
        GameSupport.IsShowBannerPopup(_bNoticeLoginBonus);
        //인앱구매 내역이 남아있는지 확인.
        if (!ReceiptChackFlag())
        {
            SendReqCommunityInfoGet();
        }
    }

    public void EndCharInfoShowDailyLoginPopup()
    {
        LobbyUIManager.Instance.ShowDailyLoginPopup();
        SendReqCommunityInfoGet();
    }


    /// <summary>
    /// 커뮤니티 정보 요청 - 친구 관련 정보 요청
    /// </summary>
    private void SendReqCommunityInfoGet()
    {
        Log.Show("##### Call SendReqCommunityInfoGet", Log.ColorType.Red);
        GameInfo.Instance.Send_ReqCommunityInfoGet(OnNetAckCommunityInfoGet);
    }

    private void OnNetAckCommunityInfoGet(int result, PktMsgType pktMsg)
    {
        if (result != 0)
            return;

        Log.Show("#### Call Send_ReqUpdateGllaMission", Log.ColorType.Red);
        GameInfo.Instance.Send_ReqUpdateGllaMission(OnNetAckUpdateGllaMission);
    }

    private void OnNetAckUpdateGllaMission(int result, PktMsgType pktMsg)
    {
        Log.Show("#### LocalPushNotificationManager.Instance.SendPushToken", Log.ColorType.Red);

		if (LocalPushNotificationManager.Instance)
		{
			LocalPushNotificationManager.Instance.SendPushToken();
		}

        if (FSaveData.Instance.IsContinueStage())
            MessagePopup.OKCANCEL(eTEXTID.OK, 3118, ContinueStage, CheckNickName );
        else
            CheckNickName();
    }

    private void CheckNickName() {
        char[] c = GameInfo.Instance.UserData.GetRawNickNameCharArr();

        CheckUnicodePosition( c );

        for( int i = 0; i < c.Length; i++ ) {
            if( c[i] == '[' ) {
                Utility.ChangeBBCodeCharToZero( c, i );
            }
        }

        string newNickName = "";
        for( int i = 0; i < c.Length; i++ ) {
            if( c[i] != '\0' ) {
                newNickName += c[i];
            }
        }

        if( string.IsNullOrEmpty( newNickName ) ) {
            newNickName = GameInfo.Instance.GameConfig.InitAccountNickName + UnityEngine.Random.Range( 1000000, 10000000 );
		}

        if( newNickName != GameInfo.Instance.UserData.GetRawNickName() ) {
            GameInfo.Instance.Send_ReqUserSetName( newNickName, OnNetSetUserNickName );
        }
        else {
            BannerAndDyeingCheck();
        }
    }

    private bool CheckUnicodePosition( char[] c ) {
        string singleChar;
        byte[] unicodeBytes;

        for( int i = 0; i < GameInfo.Instance.UnicodeCheckTable.Infos.Count; i++ ) {
            for( int j = 0; j < c.Length; j++ ) {
                singleChar = c[j].ToString();
                unicodeBytes = System.Text.Encoding.Unicode.GetBytes( singleChar );

                int position = 0;
                for( int k = unicodeBytes.Length - 1; k >= 0; k-- ) {
                    position |= ( unicodeBytes[k] << ( k * 8 ) );
                }

                if( position >= GameInfo.Instance.UnicodeCheckTable.Infos[i].Min && position <= GameInfo.Instance.UnicodeCheckTable.Infos[i].Max ) {
                    c[j] = '\0';
                }
            }
		}

        return false;
    }

    private void OnNetSetUserNickName( int result, PktMsgType pktmsg ) {
        if( result != 0 ) {
            return;
        }

        //닉네임 갱신
        UITopPanel toppanel = LobbyUIManager.Instance.GetActiveUI<UITopPanel>("TopPanel");
        toppanel.Renewal( true );

        BannerAndDyeingCheck();
    }

    private void BannerAndDyeingCheck()
    {
        FSaveData.Instance.RemoveStageData();

        if (_bNoticeLoginBonus)
            GameSupport.IsShowBannerPopup(_bNoticeLoginBonus);
        else
        {
            int popupCount = LobbyUIManager.Instance.ActivePopupList.Count;
            if (popupCount <= 0)
            {
                int costumeId = (int)GameInfo.Instance.UserData.DyeingCostumeId;
                if (0 < costumeId)
                {
                    UIMessagePopup.OnClickOKCallBack callBack = () =>
                    {
                        GameTable.Costume.Param costume = GameInfo.Instance.GameTable.FindCostume(costumeId);
                        if (costume == null)
                            return;
        
                        CharData charData = GameInfo.Instance.GetCharDataByTableID(costume.CharacterID);
                        if (charData == null)
                            return;
                            
                        UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, charData.CUID);
                        UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, charData.TableData.ID);
                        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);
                    };
                    
                    MessagePopup.OK(FLocalizeString.Instance.GetText((int)eTEXTID.TITLE_NOTICE), 
                        FLocalizeString.Instance.GetText(1743), 
                        FLocalizeString.Instance.GetText((int)eTEXTID.OK), callBack, callBack);
                }
                else if (!_bEnterAnimPlayOnce)
                {
                    _bEnterAnimPlayOnce = true;
                    if (_lobbyEnterCor != null)
                    {
                        StopCoroutine(_lobbyEnterCor);
                        _lobbyEnterCor = null;
                    }
                    _lobbyEnterCor = StartCoroutine(WaitLobbyEnterAnimation());
                }
            }
        }
    }

    IEnumerator WaitLobbyEnterAnimation()
    {
        while (!Lobby.Instance.LobbyPlayer.gameObject.activeSelf)
            yield return null;
        
        if (!Lobby.Instance.LobbyPlayer.EnterAnimation())
        {
            if (!GameInfo.Instance.Greetings)
            {
                VoiceMgr.Instance.PlayChar(eVOICECHAR.Greetings, GameInfo.Instance.GetMainChar().TableID);
                GameInfo.Instance.Greetings = true;
            }
        }
        
        _lobbyEnterCor = null;
    }

    private void ContinueStage()
    {
        int stageId = PlayerPrefs.GetInt("SAVE_STAGE_ID_");

        GameTable.Stage.Param param = GameInfo.Instance.GameTable.FindStage(stageId);
        if (param == null)
        {
            FSaveData.Instance.RemoveStageData();
            return;
        }

        var stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == stageId);

        int ticket = param.Ticket;
        int multipleIndex = PlayerPrefs.GetInt("SAVE_STAGE_MULTIPLE_INDEX_");

        bool bmultiGCFlag = false;
        string multigcdata = PlayerPrefs.GetString("SAVE_STAGE_MULTI_GC_FLAG_");
        bmultiGCFlag = bool.Parse(multigcdata);

        bool bclearmission = false;
        if (GameSupport.GetStageMissionCount(param) == 0)
            bclearmission = true;
        else if (stagecleardata != null)
            bclearmission = stagecleardata.IsClearAll();

        if (param.TicketMultiple == 1 && bclearmission)
        {
            int mult = GameInfo.Instance.GameConfig.MultipleList[multipleIndex];
            int multrate = GameInfo.Instance.GameConfig.MultipleRewardRateList[multipleIndex];
            ticket = ticket * mult;
        }

        if (param.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK)
        {
            if (!GameSupport.IsCheckTicketBP(ticket))
            {
                FSaveData.Instance.RemoveStageData();
                return;
            }
        }
        else
        {
            if (param.StageType == (int)eSTAGETYPE.STAGE_DAILY)
            {
                //if (GameSupport.IsOnDayOfWeek(param.TypeValue, (int)GameInfo.Instance.ServerData.LoginTime.DayOfWeek) == true)
                if (GameSupport.IsOnDayOfWeek(param.TypeValue, (int)GameSupport.GetCurrentRealServerTime().DayOfWeek) == true)
                {
                    ticket = (int)((float)ticket * GameInfo.Instance.GameConfig.StageDailyCostTicketRate);
                }
            }

            //캠페인 체크
            GuerrillaCampData campdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_StageClear_APRateDown, param.StageType);
            if (campdata != null)
            {
                ticket -= (int)((float)ticket * (float)((float)campdata.EffectValue / (float)eCOUNT.MAX_BO_FUNC_VALUE));
            }

            if (!GameSupport.IsCheckTicketAP(ticket))
            {
                FSaveData.Instance.RemoveStageData();
                return;
            }
        }

        if (param.StageType == (int)eSTAGETYPE.STAGE_EVENT)
        {
            int eventId = param.TypeValue;

            int state = GameSupport.GetJoinEventState(eventId);
            if (state != (int)eEventState.EventPlaying)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3053));
                FSaveData.Instance.RemoveStageData();
                return;
            }
        }

        if (!GameSupport.IsCheckInven())
        {
            FSaveData.Instance.RemoveStageData();
            return;
        }

        string str = PlayerPrefs.GetString("SAVE_STAGE_CHAR_UID_");
        long charUID = long.Parse(str);

        CharData chardata = GameInfo.Instance.GetCharData(charUID);
        if (chardata == null)
        {
            FSaveData.Instance.RemoveStageData();
            return;
        }

        GameInfo.Instance.SeleteCharUID = charUID;

        UIValue.Instance.SetValue(UIValue.EParamType.StageID, stageId);
        GameInfo.Instance.Send_ReqStageStart(stageId, charUID, multipleIndex, false, bmultiGCFlag, null, OnContinueStage);
    }

    private void OnContinueStage(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        int stageId = (int)UIValue.Instance.GetValue(UIValue.EParamType.StageID);
        GameTable.Stage.Param param = GameInfo.Instance.GameTable.FindStage(stageId);

        if (!GameInfo.Instance.netFlag)
            GameInfo.Instance.MaxNormalBoxCount = UnityEngine.Random.Range(param.N_DropMinCnt, param.N_DropMaxCnt + 1);

        GameInfo.Instance.SelecteStageTableId = stageId;
        GameInfo.Instance.ContinueStage = true;

        UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.LobbyToStage);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, stageId);
        LobbyUIManager.Instance.ShowUI("LoadingPopup", false);
        AppMgr.Instance.LoadScene(AppMgr.eSceneType.Stage, param.Scene);
    }

    public void OnClick_SpecialBuyBtn()
    {
        UISpecialBuyPopup specialBuyPopup = LobbyUIManager.Instance.GetUI("SpecialBuyPopup") as UISpecialBuyPopup;
        if (specialBuyPopup != null)
        {
            specialBuyPopup.SetChmuki();
            specialBuyPopup.SetUIActive(true);
        }
    }

    public void OnClick_MonthlyPackBtn()
    {
        if (GameSupport.PremiumMonthlyDateFlag((int)eMonthlyType.PREMIUM))
        {
            GameSupport.PaymentAgreement_Package(2023);
        }
    }

    private void SpecialBuyBtnCheck()
    {
        if (_specialRemainTimeSpan == null)
            _specialRemainTimeSpan = new TimeSpan(0);

        if (GameSupport.SpecialBuyPopupWithRemainTime())
        {
            kSpecialBuyBtn.SetActive(true);
            string remainTimeStr = PlayerPrefs.GetString(eSpecialLocalData.SPECIAL_BUY_POPUP_REMAIN_TIME.ToString());
            _specialRemainTime = new DateTime(long.Parse(remainTimeStr));

            BannerData bannerdata = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.SPECIAL_BUY);
            if (bannerdata == null)
                return;

            int storeID = bannerdata.BannerTypeValue;

            GameTable.Store.Param storeData = GameInfo.Instance.GameTable.FindStore(x => x.ID == storeID);
            if (storeData != null)
            {
                GameTable.Random.Param randomData = GameInfo.Instance.GameTable.FindRandom(x => x.GroupID == storeData.ProductIndex);
                RewardData rewarddata = new RewardData(randomData.ProductType, randomData.ProductIndex, randomData.ProductValue);

                if (rewarddata.Type == (int)eREWARDTYPE.WEAPON)
                {

                }
                else if (rewarddata.Type == (int)eREWARDTYPE.GEM)
                {

                }
                else if (rewarddata.Type == (int)eREWARDTYPE.CARD)
                {
                    GameTable.Card.Param carddata = GameInfo.Instance.GameTable.FindCard(rewarddata.Index);
                    if (carddata != null)
                    {
                        string fileName = carddata.Icon + ".png";
                        kSpecialTex.mainTexture = (Texture2D)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + fileName);
                    }

                }
                else if (rewarddata.Type == (int)eREWARDTYPE.ITEM)
                {

                }
                else if (rewarddata.Type == (int)eREWARDTYPE.COSTUME)
                {

                }
                else
                {
                    Debug.LogError((eREWARDTYPE)rewarddata.Type + " / 정의되지 않음");
                }

                SetSpecialRemainTime();
            }
        }
        else
        {
            kSpecialBuyBtn.SetActive(false);
        }
    }

    string _str231 = string.Empty;
    private void SetSpecialRemainTime()
    {
        _specialRemainTimeSpan = _specialRemainTime - DateTime.Now;

        if (_specialRemainTimeSpan.Ticks > 0)
        {
            if (string.IsNullOrEmpty(_str231))
                _str231 = FLocalizeString.Instance.GetText(231);

            //kSpecialRemainTimeLb.textlocalize = FLocalizeString.Instance.GetText(231, _specialRemainTimeSpan.Minutes, _specialRemainTimeSpan.Seconds);
            kSpecialRemainTimeLb.textlocalize = string.Format(_str231, _specialRemainTimeSpan.Minutes, _specialRemainTimeSpan.Seconds);            
        }
        else
        {
            SpecialBuyBtnCheck();
        }

    }

    private void SetMonthlyPack()
    {
        if (kMonthlyPackObj == null)
            return;

        if (GameSupport.IsHaveMonthlyData((int)eMonthlyType.PREMIUM))
        {
            kMonthlyPackObj.SetActive(true);
            if (GameSupport.PremiumMonthlyDateFlag((int)eMonthlyType.PREMIUM))
            {
                kMonthlyPackBuyObj.SetActive(true);
                kMonthlyPackNormalObj.SetActive(false);
            }
            else
            {
                kMonthlyPackBuyObj.SetActive(false);
                kMonthlyPackNormalObj.SetActive(true);
            }

            if (_monthlyData == null)
                _monthlyData = GameInfo.Instance.UserMonthlyData.GetMonthlyDataWithStoreID((int)eMonthlyType.PREMIUM);

            if (_monthlyData != null)
            {
                kMonthlyPackEndTimeLabel.textlocalize = GameSupport.GetRemainTimeString(_monthlyData.MonthlyEndTime, GameSupport.GetCurrentServerTime());
            }

        }
        else
        {
            if (_monthlyData != null)
                _monthlyData = null;
            kMonthlyPackObj.SetActive(false);
        }
    }

#region Backgruond Change
    private bool _IsBackgroundEnable = false;
    public void OnClick_BackgroundChange()
    {
        if (_IsBackgroundEnable == false)
        {
            // 서버에 리스트 요청
            GameInfo.Instance.Send_ReqUserLobbyThemeList(OnNetUserLobbyThemeList);
        }
        else
        {
            _IsBackgroundEnable = false;
            SetStateBackgroundChangBtn();
        }
    }

    private void SetStateBackgroundChangBtn()
    {
        kBackgruondChangeOn.SetActive(_IsBackgroundEnable);
        kBackgruondChangeOff.SetActive(!_IsBackgroundEnable);
        _BackgroundList.gameObject.SetActive(_IsBackgroundEnable);
    }

    private void _UpdateBackgroundListSlot(int index, GameObject slotObject)
    {
        UIBackgroundChangeListSlot slot = slotObject.GetComponent<UIBackgroundChangeListSlot>();
        if (null == slot) return;

        slot.ParentGO = this.gameObject;
        slot.UpdateSlot(GameInfo.Instance.UserLobbyThemeList[index]);
    }

    private int _UpdateBackgroundCount()
    {   
        return GameInfo.Instance.UserLobbyThemeList.Count;
    }

    private void OnNetUserLobbyThemeList(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        _IsBackgroundEnable = true;
        SetStateBackgroundChangBtn();
        // 보유한 Background List 갱신
        _BackgroundList.UpdateList();
    }
#endregion

#region UserWelcome
    public void OnClick_WelcomeEvent()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.DailyEventType, eEventTarget.WELCOME);
        LobbyUIManager.Instance.ShowUI("UserWelcomeEventPopup", true);
    }

    public void OnClick_ComebackEvent()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.DailyEventType, eEventTarget.COMEBACK);
        LobbyUIManager.Instance.ShowUI("UserWelcomeEventPopup", true);
    }

    public void OnClick_PublicEvent()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.DailyEventType, eEventTarget.PUBLIC);
        LobbyUIManager.Instance.ShowUI("UserWelcomeEventPopup", true);
    }
#endregion

#region Influence
    public void OnClick_InfluencePopup()
    {
        if (GameInfo.Instance.InfluenceMissionData.InfluID == 0)
        {  
            if (GameSupport.GetCurrentInfluenceMissionSet() != null)
                LobbyUIManager.Instance.ShowUI("InfluenceSelectionPopup", true);
            else            
                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(100012), null);            
        }
        else
        {
            GameInfo.Instance.Send_ReqGetInfluenceInfo((int result, PktMsgType pktmsg) => { LobbyUIManager.Instance.ShowUI("InfluenceMainPopup", true); });
        }
    }
#endregion

    public void OnClick_CircleBtn()
    {
        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3001));
        return;

        //if (GameInfo.Instance.CircleData.Uid <= 0)
        //{
        //    GameInfo.Instance.Send_ReqSuggestCircleList(FLocalizeString.Language, OnNet_SuggestCircleList);
        //}
        //else
        //{
        //    if (GameInfo.Instance.CircleReqChatList)
        //    {
        //        OnNet_CircleChatList(0, null);
        //    }
        //    else
        //    {
        //        GameInfo.Instance.Send_ReqCircleChatList(OnNet_CircleChatList);
        //    }
        //}
    }

    private void OnNet_SuggestCircleList(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CircleJoin);
    }

    private void OnNet_CircleChatList(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        GameInfo.Instance.Send_ReqCircleLobbyInfo(OnNet_CircleLobbyInfo);
    }

    private void OnNet_CircleLobbyInfo(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        GameInfo.Instance.CircleReqChatList = true;
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CircleLobby);
    }
}
