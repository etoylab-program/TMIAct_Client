using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITopPanel : FComponent
{
    public enum eTOPSTATE
    {
        MAIN = 0,
        NORMAL,
        CHAR,
        SUPPORTER,
        STORE,
        ITEM,
        STAGE,
        GACHA,
        ROOM,
        OUT,
        FACILITY_IN,
        FACILITY_OUT,
        AWAKEN_SKILL,
        ARENA_TOWER,
        CIRCLE_GOLD,
        CIRCLE_POINT,
    };

    public enum eTOPANIM
    {
        IN_TOPPANEL = 0,
        OUT_TOPPANEL,
        USERINFO,
        BACK,
        GACHA,
        FACILITY_IN,
        FACILITY_OUT,
    }

    public GameObject kGoodsRoot;
    public UIGoodsUnit kSkillPointGoodsUnit;
    public UIGoodsUnit kRoomPointGoodsUnit;
    public UIGoodsUnit kPointGoodsUnit;
    public UIGoodsUnit kCashGoodsUnit;
    public UIGoodsUnit kGoldGoodsUnit;
    public UIGoodsUnit kDesireGoodsUnit;
    public UIGoodsUnit kTicketAPGoodsUnit;
    public UILabel kTicketAPRemainLabel;
    public UIGoodsUnit kTicketBPGoodsUnit;
    public UILabel kTicketBPRemainLabel;
    public UIButton kOptionBtn;
    public UITexture kIconTex;
    public UILabel kLevelLabel;
    public UILabel kNameLabel;
    public UIGaugeUnit kGaugeUnit;
    public UIButton kBackBtn;
    public List<GameObject> kSvrOnList;
    public List<GameObject> kSvrOffList;
    public List<int> kXPosList;

    public UIGoodsUnit kFriendPointGoodsUnit;
    public UIGoodsUnit kEventStorePointGoodsUnit;

    public UIGoodsUnit WPGoodsUnit;
    public UIGoodsUnit DirectiveGoodsUnit;

    public GameObject kServerRelocateObj;
    public GameObject kServerRelocateBefore;
    public GameObject kServerRelocateAfter;

	[Header("[New/Comeback User]")]
	public UISprite SprNewUser;
	public UISprite SprComebackUser;

    [Header("[Blacklist]")]
    public UISprite SprBlacklist;

    [Header("Circle")]
    public UIGoodsUnit CircleGoodsUnit;
    public UISprite CircleGoldSpr;
    public UISprite CirclePointSpr;

    private eTOPSTATE _topstate = eTOPSTATE.MAIN;
    public eTOPSTATE TopState { get { return _topstate; } }

    private eTOPSTATE _prevState = eTOPSTATE.MAIN;
    private int _tickettimeAPid = 0;
    private int _tickettimeBPid = 0;

    private int mPrevAnim = -1;
    

    public override void OnEnable()
    {
		if (SprNewUser)
		{
			SprNewUser.SetActive(false);
		}

		if (SprComebackUser)
		{
			SprComebackUser.SetActive(false);
		}

		mPrevAnim = -1;

		OnAnimationEvent();
        base.OnEnable();
	}

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (_tickettimeAPid != 0)
            FGlobalTimer.Instance.RemoveTimer(_tickettimeAPid);

        if (_tickettimeBPid != 0)
            FGlobalTimer.Instance.RemoveTimer(_tickettimeBPid);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        //서버이전 오브젝트 초기화
        InitServerRelocate();


        UserData userdata = GameInfo.Instance.UserData;
        if (userdata.Level == 0)
            return;

        LobbyUIManager.Instance.GetUserMarkIcon(this.gameObject, this.gameObject, GameInfo.Instance.UserData.UserMarkID, ref kIconTex);

        kLevelLabel.textlocalize = userdata.Level.ToString();//string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.RANK_TXT_NOW_LV), userdata.Level);
        kNameLabel.textlocalize = userdata.GetNickName();
        //  레벨과 이름이 한줄에 있는 경우 필요
        //  2줄로 있는 지금 불필요한 코드로 주석처리합니다.
        //kNameLabel.transform.localPosition = new Vector3(kLevelLabel.printedSize.x + 10, kNameLabel.transform.localPosition.y, 0);

        float fillAmount = GameSupport.GetAccountLevelExpGauge(userdata.Level, userdata.Exp);
        kGaugeUnit.InitGaugeUnit(fillAmount);

        float perVal = fillAmount * 100.0f;
        double truncateVal = System.Math.Truncate(perVal * 100f) / 100f;
        kGaugeUnit.SetText(string.Format("{0:#.##}%", truncateVal));

        //상단에 선택한 캐릭터의 스킬포인트가 노출되도록 변경
        //CharData chardata = GameInfo.Instance.GetMainChar();
        CharData chardata = null;
        var selCharID = UIValue.Instance.GetValue(UIValue.EParamType.CharSelTableID);
        if (selCharID != null)
            chardata = GameInfo.Instance.GetCharDataByTableID((int)selCharID);
        else
            chardata = GameInfo.Instance.GetMainChar();

        if (chardata != null)
            kSkillPointGoodsUnit.kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), chardata.PassviePoint);

        kGoldGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, userdata.Goods[(int)eGOODSTYPE.GOLD]);
        kCashGoodsUnit.InitGoodsUnit(eGOODSTYPE.CASH, userdata.Goods[(int)eGOODSTYPE.CASH]);
        kPointGoodsUnit.InitGoodsUnit(eGOODSTYPE.SUPPORTERPOINT, userdata.Goods[(int)eGOODSTYPE.SUPPORTERPOINT]);
        kRoomPointGoodsUnit.InitGoodsUnit(eGOODSTYPE.ROOMPOINT, userdata.Goods[(int)eGOODSTYPE.ROOMPOINT]);
        kFriendPointGoodsUnit.InitGoodsUnit(eGOODSTYPE.FRIENDPOINT, userdata.Goods[(int)eGOODSTYPE.FRIENDPOINT]);
        kDesireGoodsUnit.InitGoodsUnit(eGOODSTYPE.DESIREPOINT, userdata.Goods[(int)eGOODSTYPE.DESIREPOINT]);
        kDesireGoodsUnit.SetTextNowMax(userdata.Goods[(int)eGOODSTYPE.DESIREPOINT], GameInfo.Instance.GameConfig.LimitMaxDP);
        WPGoodsUnit.InitGoodsUnit(eGOODSTYPE.AWAKEPOINT, userdata.Goods[(int)eGOODSTYPE.AWAKEPOINT]);
        
        if (_topstate == eTOPSTATE.CIRCLE_GOLD)
        {
            CircleGoodsUnit.InitGoodsUnit(eCircleGoodsType.CIRCLE_GOLD, GameInfo.Instance.CircleData.Goods[(int)eCircleGoodsType.CIRCLE_GOLD], isSmall: true);
        }
        else
        {
            CircleGoodsUnit.InitGoodsUnit(eGOODSTYPE.CIRCLEPOINT, userdata.Goods[(int)eGOODSTYPE.CIRCLEPOINT]);
        }
        
        // 특파 지령서
        int count = 0;
        ItemData itemData = GameInfo.Instance.GetItemData(10052);
        if (itemData != null)
        {
            count = itemData.Count;
        }

        if (DirectiveGoodsUnit)
        {
            DirectiveGoodsUnit.InitGoodsUnit(eGOODSTYPE.NONE, count);
        }

        //이벤트 스토어 재화(아이템) 초기화
        kEventStorePointGoodsUnit.InitGoodsUnit_EventStore();


        int ticketnow = (int)userdata.Goods[(int)eGOODSTYPE.AP];
        int ticketmax = GameSupport.GetMaxAP();
        kTicketAPGoodsUnit.InitGoodsUnit(eGOODSTYPE.AP, ticketnow, ticketmax);
        if (ticketnow < ticketmax)
        {
            kTicketAPGoodsUnit.kTextLabel.transform.localPosition = new Vector3(kTicketAPGoodsUnit.kTextLabel.transform.localPosition.x, 6, 0);
            kTicketAPRemainLabel.gameObject.SetActive(true);
            kTicketAPRemainLabel.textlocalize = GameSupport.GetRemainTimeString(GameInfo.Instance.UserData.APRemainTime, GameSupport.GetCurrentServerTime());

            if (_tickettimeAPid == 0)
                _tickettimeAPid = FGlobalTimer.Instance.AddTimer(1, OnTimeTicketAPTime, true);
        }
        else
        {
            kTicketAPGoodsUnit.kTextLabel.transform.localPosition = new Vector3(kTicketAPGoodsUnit.kTextLabel.transform.localPosition.x, 0, 0);
            kTicketAPRemainLabel.gameObject.SetActive(false);
        }

        ticketnow = (int)userdata.Goods[(int)eGOODSTYPE.BP];
        ticketmax = GameInfo.Instance.GameConfig.BPMaxCount; 
        kTicketBPGoodsUnit.InitGoodsUnit(eGOODSTYPE.BP, ticketnow, ticketmax);
        if (ticketnow < ticketmax)
        {
            kTicketBPGoodsUnit.kTextLabel.transform.localPosition = new Vector3(kTicketBPGoodsUnit.kTextLabel.transform.localPosition.x, 6, 0);
            kTicketBPRemainLabel.gameObject.SetActive(true);
            kTicketBPRemainLabel.textlocalize = GameSupport.GetRemainTimeString(GameInfo.Instance.UserData.BPRemainTime, GameSupport.GetCurrentServerTime());

            if (_tickettimeBPid == 0)
                _tickettimeBPid = FGlobalTimer.Instance.AddTimer(1, OnTimeTicketBPTime, true);
        }
        else
        {
            kTicketBPGoodsUnit.kTextLabel.transform.localPosition = new Vector3(kTicketBPGoodsUnit.kTextLabel.transform.localPosition.x, 0, 0);
            kTicketBPRemainLabel.gameObject.SetActive(false);
        }

        for (int i = 0; i < kSvrOnList.Count; i++)
            kSvrOnList[i].SetActive(false);
        for (int i = 0; i < kSvrOffList.Count; i++)
            kSvrOffList[i].SetActive(false);

        if ( AppMgr.Instance.DebugInfo )
        {
            if (GameInfo.Instance.IsConnect)
            {
                for (int i = 0; i < kSvrOnList.Count; i++)
                    kSvrOnList[i].SetActive(true);
            }
            else
            {
                for (int i = 0; i < kSvrOffList.Count; i++)
                    kSvrOffList[i].SetActive(true);
            }
        }

        kEventStorePointGoodsUnit.transform.parent.gameObject.SetActive(false);
        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.STORE)
        {
            UIStorePanel storepanel = LobbyUIManager.Instance.GetActiveUI<UIStorePanel>("StorePanel");
            if(storepanel != null)
            {
                if (storepanel.StorePanelType == eStorePanelType.Etc || storepanel.StorePanelType == eStorePanelType.Etc_Event)
                {
                    switch(storepanel.EtcTabSelect)
                    {
                        case UIStorePanel.eStoreTabType.STORE_FRIENDPOINT:
                            {
                                kFriendPointGoodsUnit.transform.parent.gameObject.SetActive(true);
                                kPointGoodsUnit.transform.parent.gameObject.SetActive(false);
                            } break;
                        case UIStorePanel.eStoreTabType.STORE_EVENT:
                            {
                                kFriendPointGoodsUnit.transform.parent.gameObject.SetActive(false);
                                kPointGoodsUnit.transform.parent.gameObject.SetActive(false);
                                kEventStorePointGoodsUnit.transform.parent.gameObject.SetActive(true);
                            } break;
                        default:
                            {
                                kFriendPointGoodsUnit.transform.parent.gameObject.SetActive(false);
                                kPointGoodsUnit.transform.parent.gameObject.SetActive(true);
                            } break;
                    }
                }
                else
                {
                    kFriendPointGoodsUnit.transform.parent.gameObject.SetActive(false);
                    kPointGoodsUnit.transform.parent.gameObject.SetActive(true);
                }
            }
        }
    }

    private void OnTimeTicketAPTime()
    {
        UserData userdata = GameInfo.Instance.UserData;
        int ticketnow = (int)userdata.Goods[(int)eGOODSTYPE.AP];
        int ticketmax = GameSupport.GetMaxAP();

        kTicketAPGoodsUnit.InitGoodsUnit(eGOODSTYPE.AP, ticketnow, ticketmax);

        if (ticketnow < ticketmax)
        {
            kTicketAPGoodsUnit.kTextLabel.transform.localPosition = new Vector3(kTicketAPGoodsUnit.kTextLabel.transform.localPosition.x, 6, 0);
            kTicketAPRemainLabel.gameObject.SetActive(true);
            kTicketAPRemainLabel.textlocalize = GameSupport.GetRemainTimeString(GameInfo.Instance.UserData.APRemainTime, GameSupport.GetCurrentServerTime());
        }
        else
        {
            kTicketAPGoodsUnit.kTextLabel.transform.localPosition = new Vector3(kTicketAPGoodsUnit.kTextLabel.transform.localPosition.x, 0, 0);
            kTicketAPRemainLabel.gameObject.SetActive(false);

            FGlobalTimer.Instance.RemoveTimer(_tickettimeAPid);
            _tickettimeAPid = 0;
            Renewal(true);
        }
    }

    private void OnTimeTicketBPTime()
    {
        UserData userdata = GameInfo.Instance.UserData;
        int ticketnow = (int)userdata.Goods[(int)eGOODSTYPE.BP];
        int ticketmax = GameInfo.Instance.GameConfig.BPMaxCount;

        kTicketBPGoodsUnit.InitGoodsUnit(eGOODSTYPE.BP, ticketnow, ticketmax);

        if (ticketnow < ticketmax)
        {
            kTicketBPGoodsUnit.kTextLabel.transform.localPosition = new Vector3(kTicketBPGoodsUnit.kTextLabel.transform.localPosition.x, 6, 0);
            kTicketBPRemainLabel.gameObject.SetActive(true);
            kTicketBPRemainLabel.textlocalize = GameSupport.GetRemainTimeString(GameInfo.Instance.UserData.BPRemainTime, GameSupport.GetCurrentServerTime());
        }
        else
        {
            kTicketBPGoodsUnit.kTextLabel.transform.localPosition = new Vector3(kTicketBPGoodsUnit.kTextLabel.transform.localPosition.x, 0, 0);
            kTicketBPRemainLabel.gameObject.SetActive(false);

            FGlobalTimer.Instance.RemoveTimer(_tickettimeBPid);
            _tickettimeBPid = 0;
            Renewal(true);
        }
    }

    public void OnClick_CashPlusBtn()
    {
        GameSupport.PaymentAgreement_Cash();
    }

    public void OnClick_MailBtn()
    {
        LobbyUIManager.Instance.ShowUI("MailBoxPopup", true);
    }

    public void OnClick_OptionBtn()
    {
        LobbyUIManager.Instance.ShowUI("MenuPopup", true);
    }

	public void OnClick_TicketAPPlusBtn() {
		UIRecoverySelectPopup uIRecoverySelectPopup = LobbyUIManager.Instance.GetUI<UIRecoverySelectPopup>( "RecoverySelectPopup" );
		if ( uIRecoverySelectPopup != null ) {
			uIRecoverySelectPopup.SetGoodsType( eGOODSTYPE.AP );
			uIRecoverySelectPopup.SetUIActive( true );
		}
	}

	public void OnClick_TicketBPPlusBtn() {
		UIRecoverySelectPopup uIRecoverySelectPopup = LobbyUIManager.Instance.GetUI<UIRecoverySelectPopup>( "RecoverySelectPopup" );
		if ( uIRecoverySelectPopup != null ) {
			uIRecoverySelectPopup.SetGoodsType( eGOODSTYPE.BP );
			uIRecoverySelectPopup.SetUIActive( true );
		}
	}

    public void OnClick_BackBtn()
    {
        LobbyUIManager.Instance.BackBtnEvent();
    }

    public void OnClick_UserInfoBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.CardFormationType, eCharSelectFlag.USER_INFO);
        UIValue.Instance.SetValue(UIValue.EParamType.UserInfoPopup, 0);
        LobbyUIManager.Instance.ShowUI("UserInfoPopup", true);
    }

    public void OnClick_ServerRelocate()
    {
        if(GameInfo.Instance.IsServerRelocate)
        {
            //서버이전 완료
            //Debug.Log(string.Format("[서버이전 완료] ID : {0}", ));
            MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(6006), GameInfo.Instance.UserData.UUID, GameInfo.Instance.ServerRelocateID), null);
        }
        else
        {
            GameInfo.Instance.Send_ReqGetTotalRelocateCntToNotComplete(ShowServerAgreePopup);
        }
    }

    private void ShowServerAgreePopup(int result, PktMsgType pktMsg)
    {
        if (result != 0)
        {
            return;
        }

        //서버이전 팝업 띄우기
        Debug.Log(string.Format("[서버이전] State : {0}", GameInfo.Instance.IsServerRelocate));
        LobbyUIManager.Instance.ShowUI("ServerAgreePopup", true);
    }

    public void OnAnimationEvent()
    {
        kGoodsRoot.transform.localPosition = new Vector3(kXPosList[(int)_topstate], 0, 0);

        // 과반 이상 표시하는 정보
        kCashGoodsUnit.transform.parent.gameObject.SetActive(true);
        kGoldGoodsUnit.transform.parent.gameObject.SetActive(true);
        kTicketAPGoodsUnit.transform.parent.gameObject.SetActive(true);
        kOptionBtn.gameObject.SetActive(true);

        // 과반 이상 표시하지 않는 정보
        kSkillPointGoodsUnit.transform.parent.gameObject.SetActive(false);
        kRoomPointGoodsUnit.transform.parent.gameObject.SetActive(false);
        kPointGoodsUnit.transform.parent.gameObject.SetActive(false);
        kFriendPointGoodsUnit.transform.parent.gameObject.SetActive(false);
        kDesireGoodsUnit.transform.parent.gameObject.SetActive(false);
        kTicketBPGoodsUnit.transform.parent.gameObject.SetActive(false);
        WPGoodsUnit.transform.parent.gameObject.SetActive(false);
        
        CircleGoodsUnit.transform.parent.gameObject.SetActive(false);
        CircleGoldSpr.SetActive(_topstate == eTOPSTATE.CIRCLE_GOLD);
        CirclePointSpr.SetActive(_topstate == eTOPSTATE.CIRCLE_POINT);
        
        if (DirectiveGoodsUnit)
        {
            DirectiveGoodsUnit.transform.parent.gameObject.SetActive(false);
        }

        if (_topstate == eTOPSTATE.CHAR)
        {
            kSkillPointGoodsUnit.transform.parent.gameObject.SetActive(true);
        }
        else if (_topstate == eTOPSTATE.STORE)
        {
            kPointGoodsUnit.transform.parent.gameObject.SetActive(true);
            kOptionBtn.gameObject.SetActive(false);
        }
        else if (_topstate == eTOPSTATE.ITEM)
        {
            kPointGoodsUnit.transform.parent.gameObject.SetActive(true);
        }
        else if (_topstate == eTOPSTATE.STAGE)
        {
            kTicketBPGoodsUnit.transform.parent.gameObject.SetActive(true);
        }
        else if (_topstate == eTOPSTATE.GACHA)
        {
            kPointGoodsUnit.transform.parent.gameObject.SetActive(true);
            kDesireGoodsUnit.transform.parent.gameObject.SetActive(true);
            kTicketAPGoodsUnit.transform.parent.gameObject.SetActive(false);
            kOptionBtn.gameObject.SetActive(false);
        }
        else if (_topstate == eTOPSTATE.ROOM)
        {
            kOptionBtn.gameObject.SetActive(false);
        }
        else if(_topstate == eTOPSTATE.AWAKEN_SKILL)
        {
            kSkillPointGoodsUnit.transform.parent.gameObject.SetActive(false);
            WPGoodsUnit.transform.parent.gameObject.SetActive(true);
        }
        else if(_topstate == eTOPSTATE.ARENA_TOWER)
        {
            DirectiveGoodsUnit.transform.parent.gameObject.SetActive(true);
        }
        else if (_topstate == eTOPSTATE.CIRCLE_GOLD || _topstate == eTOPSTATE.CIRCLE_POINT)
        {
            kTicketAPGoodsUnit.transform.parent.gameObject.SetActive(false);
            CircleGoodsUnit.transform.parent.gameObject.SetActive(true);
        }
        
        // Blacklist icon
        SprBlacklist.gameObject.SetActive( false );

        if( GameInfo.Instance.UserData.BlacklistLevel > 0 ) {
            SprBlacklist.gameObject.SetActive( true );

            SprNewUser.SetActive( false );
            SprComebackUser.SetActive( false );
        }
        else {
            if( SprNewUser && GameInfo.Instance.IsNewUser() ) {
                SprNewUser.SetActive( true );
            }
            else if( SprComebackUser && GameInfo.Instance.IsComebackUser() ) {
                SprComebackUser.SetActive( true );
            }
        }

        //  19/03/12 윤주석
        //  애니메이션을 끝낸이후 이전state로 저장합니다.
        _prevState = _topstate;
        Renewal(true);
    }

    public void SetTopStatePlay(eTOPSTATE e)
    {
        //if (_topstate == e)
        //    return;

        _topstate = e;

        int animNum = 0;

        //  19/03/11 윤주석
        //  상단 탭 애니메이션중 특정 패널의 경우
        //  오른쪽 상단만 움직이는 애니메이션을 실행합니다.
        //  애니메이션 실행중에는 rewind 하여 play합니다.
        switch (_topstate)
        {
            case eTOPSTATE.MAIN:
                {
                    animNum = (int)eTOPANIM.USERINFO;
                    //if (_prevState == eTOPSTATE.NORMAL)
                    //{
                    //    animNum = (int)eTOPANIM.IN_TOPPANEL;
                    //}
                    //else
                    //{
                    //    animNum = (int)eTOPANIM.USERINFO;
                    //}
                    
                }
                break;

            case eTOPSTATE.GACHA:
                {
                    if (_prevState == eTOPSTATE.MAIN)
                    {
                        animNum = (int)eTOPANIM.BACK;
                    }
                    else
                    {
                        animNum = (int)eTOPANIM.GACHA;

                        if (IsPlayAnimtion(animNum) == true)
                        {
                            RewindAnimation(animNum);
                            return;
                        }
                    }
                }
                break;
            case eTOPSTATE.OUT:
                {
                    animNum = (int)eTOPANIM.OUT_TOPPANEL;
                }
                break;
            case eTOPSTATE.FACILITY_IN:
                {
                    animNum = (int)eTOPANIM.FACILITY_IN;
                }
                break;
            case eTOPSTATE.FACILITY_OUT:
                {
                    animNum = (int)eTOPANIM.FACILITY_OUT;
                }
                break;
            default:
                {
                    animNum = (int)eTOPANIM.BACK;
                }
                break;

        }

        if ( mPrevAnim == animNum) {
			return;
		}

		mPrevAnim = animNum;
		PlayAnimtion( animNum );
	}

    /// <summary>
    /// 19/03/12 윤주석
    /// 해당 재화를 비활성화 표시 할경우 
    /// 컬러값을 변경 합니다.
    /// </summary>
    private void SetGoodsColor(UIGoodsUnit goods, bool isActive)
    {
        Color activeColor = new Color(1, 1, 1, 1);
        Color inactiveColor = new Color(0.4f, 0.4f, 0.4f, 0.5f);

        if (goods != null)
        {
            goods.kIconSpr.color = isActive == true ? activeColor : inactiveColor;
            goods.kTextLabel.color = isActive == true ? activeColor : inactiveColor;
        }
    }

    public void DoorToLobby()
    {
        Lobby.Instance.MoveToLobby();
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN);
    }


    private void InitServerRelocate()
    {
        kServerRelocateObj.SetActive(false);

        if (LobbyUIManager.Instance.PanelType != ePANELTYPE.MAIN)
            return;

        if (AppMgr.Instance.configData == null || AppMgr.Instance.configData.m_ServiceType != Config.eServiceType.Japan)
            return;

        if (!AppMgr.Instance.HasContentFlag(eContentType.SERVER_RELOCATE))
            return;

        kServerRelocateObj.SetActive(true);
        kServerRelocateBefore.SetActive(!GameInfo.Instance.IsServerRelocate);
        kServerRelocateAfter.SetActive(GameInfo.Instance.IsServerRelocate);
    }
    
    public void SetBackBtnActive(bool active)
    {
        kBackBtn.gameObject.SetActive(active);
    }
}
