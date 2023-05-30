using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UIGoodsPopup : FComponent
{
    public GameObject kGoodsRoot;
    public UIGoodsUnit kSkillPointGoodsUnit;
    public UIGoodsUnit kRoomPointGoodsUnit;
    public UIGoodsUnit kPointGoodsUnit;
    public UIGoodsUnit kCashGoodsUnit;
    public UIGoodsUnit kGoldGoodsUnit;
    public UIGoodsUnit kTicketAPGoodsUnit;
    public UIGoodsUnit kArenaGoldGoodsUnit;
    public UIGoodsUnit kFriendPointGoodsUnit;
    public UIGoodsUnit kDesireGoodsUnit;
    public UIGoodsUnit RaidGoodsUnit;
    public UIGoodsUnit CircleGoodsUnit;
    public UISprite CircleGoldSpr;
    public UISprite CirclePointSpr;
    public UILabel kTicketAPRemainLabel;
    public UIGoodsUnit kTicketBPGoodsUnit;
    public UILabel kTicketBPRemainLabel;
    public GameObject kTicketAP;
    public GameObject kTicketBP;
    public List<int> kXPosList;

    public UIGoodsUnit WPGoodsUnit;
    public UIGoodsUnit kEventStorePointGoodsUnit;

    //PlusBtns
    public GameObject kCashPlusBtnObj;
    public GameObject kTicketAPPlusBtnObj;
    public GameObject kTicketBPPlusBtnObj;

    public System.Action OnPostAnimationEvent { get; set; } = null;

    [Flags]
    public enum eButtonPauseType {
        NONE = 0,
        AP = 1 << 0,
        CASH = 1 << 1,
    }

    private eButtonPauseType mPauseType;

    private string mPauseMessage;

	private ePOPUPGOODSTYPE _goodstype = ePOPUPGOODSTYPE.NONE;
    private int _setdepth;
    private int _tickettimeAPid = 0;
    private int _tickettimeBPid = 0;


	public override void OnEnable() {
		mPauseType = eButtonPauseType.NONE;
		mPauseMessage = string.Empty;

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

        UserData userdata = GameInfo.Instance.UserData;
        if (userdata.Level == 0)
            return;

        //상단에 선택한 캐릭터의 스킬포인트가 노출되도록 변경
        //CharData chardata = GameInfo.Instance.GetMainChar();
    
        CharData chardata = null;
        var obj = UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
        if( obj!= null )
        {
            chardata = GameInfo.Instance.GetCharData((long)obj);
        }
        if (chardata != null)
            kSkillPointGoodsUnit.kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), chardata.PassviePoint);
        else
            kSkillPointGoodsUnit.kTextLabel.textlocalize = "";

        kRoomPointGoodsUnit.InitGoodsUnit(eGOODSTYPE.ROOMPOINT, userdata.Goods[(int)eGOODSTYPE.ROOMPOINT]);
        kPointGoodsUnit.InitGoodsUnit(eGOODSTYPE.SUPPORTERPOINT, userdata.Goods[(int)eGOODSTYPE.SUPPORTERPOINT]);
        kGoldGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, userdata.Goods[(int)eGOODSTYPE.GOLD]);
        kCashGoodsUnit.InitGoodsUnit(eGOODSTYPE.CASH, userdata.Goods[(int)eGOODSTYPE.CASH]);
        kArenaGoldGoodsUnit.InitGoodsUnit(eGOODSTYPE.BATTLECOIN, userdata.Goods[(int)eGOODSTYPE.BATTLECOIN]);
        kFriendPointGoodsUnit.InitGoodsUnit(eGOODSTYPE.FRIENDPOINT, userdata.Goods[(int)eGOODSTYPE.FRIENDPOINT]);
        kDesireGoodsUnit.InitGoodsUnit(eGOODSTYPE.DESIREPOINT, userdata.Goods[(int)eGOODSTYPE.DESIREPOINT]);
        kDesireGoodsUnit.SetTextNowMax(userdata.Goods[(int)eGOODSTYPE.DESIREPOINT], GameInfo.Instance.GameConfig.LimitMaxDP);
        WPGoodsUnit.InitGoodsUnit(eGOODSTYPE.AWAKEPOINT, userdata.Goods[(int)eGOODSTYPE.AWAKEPOINT]);
        RaidGoodsUnit.InitGoodsUnit( eGOODSTYPE.RAIDPOINT, userdata.Goods[(int)eGOODSTYPE.RAIDPOINT] );
        
        if (_goodstype == ePOPUPGOODSTYPE.CIRCLE_GOLD)
        {
            CircleGoodsUnit.InitGoodsUnit(eCircleGoodsType.CIRCLE_GOLD, GameInfo.Instance.CircleData.Goods[(int)eCircleGoodsType.CIRCLE_GOLD], isSmall: true);
        }
        else
        {
            CircleGoodsUnit.InitGoodsUnit(eGOODSTYPE.CIRCLEPOINT, userdata.Goods[(int)eGOODSTYPE.CIRCLEPOINT]);
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
    }

	public void SetPauseMessage( string pauseMessage ) {
		mPauseMessage = pauseMessage;
	}

	public void PlayButton( params eButtonPauseType[] puaseTypes ) {
		for ( int i = 0; i < puaseTypes.Length; i++ ) {
			mPauseType &= ~puaseTypes[i];
		}
	}

    public void PauseButton( params eButtonPauseType[] puaseTypes ) {
		for ( int i = 0; i < puaseTypes.Length; i++ ) {
			mPauseType |= puaseTypes[i];
		}
	}

    public bool IsPauseButton( eButtonPauseType pauseType ) {
        return (mPauseType & pauseType) != eButtonPauseType.NONE;
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
        if (GameSupport.IsTutorial())
            return;

        if ( IsPauseButton( eButtonPauseType.CASH ) ) {
            MessageToastPopup.Show( mPauseMessage );
            return;
        }

        GameSupport.PaymentAgreement_Cash();
    }

	public void OnClick_TicketAPPlusBtn() {
		if ( GameSupport.IsTutorial() ) {
			return;
		}

		if ( IsPauseButton( eButtonPauseType.AP ) ) {
			MessageToastPopup.Show( mPauseMessage );
			return;
		}

		UIRecoverySelectPopup uIRecoverySelectPopup = LobbyUIManager.Instance.GetUI<UIRecoverySelectPopup>( "RecoverySelectPopup" );
		if ( uIRecoverySelectPopup != null ) {
			uIRecoverySelectPopup.SetGoodsType( eGOODSTYPE.AP );
			uIRecoverySelectPopup.SetUIActive( true );
		}
	}

	public void OnClick_TicketBPPlusBtn() {
		if ( GameSupport.IsTutorial() ) {
			return;
		}

		UIRecoverySelectPopup uIRecoverySelectPopup = LobbyUIManager.Instance.GetUI<UIRecoverySelectPopup>( "RecoverySelectPopup" );
		if ( uIRecoverySelectPopup != null ) {
			uIRecoverySelectPopup.SetGoodsType( eGOODSTYPE.BP );
			uIRecoverySelectPopup.SetUIActive( true );
		}
	}

    public void OnAnimationEvent()
    {
        SetPanelDepth(_setdepth);

        kTicketAPGoodsUnit.transform.parent.gameObject.SetActive(true);
        kTicketBPGoodsUnit.transform.parent.gameObject.SetActive(false);

        kSkillPointGoodsUnit.transform.parent.gameObject.SetActive(false);
        kRoomPointGoodsUnit.transform.parent.gameObject.SetActive(false);
        kPointGoodsUnit.transform.parent.gameObject.SetActive(false);
        kArenaGoldGoodsUnit.transform.parent.gameObject.SetActive(false);
        kFriendPointGoodsUnit.transform.parent.gameObject.SetActive(false);
        kDesireGoodsUnit.transform.parent.gameObject.SetActive(false);
        WPGoodsUnit.transform.parent.gameObject.SetActive(false);        
        kEventStorePointGoodsUnit.transform.parent.gameObject.SetActive(false);
        RaidGoodsUnit.transform.parent.gameObject.SetActive( false );
        
        CircleGoodsUnit.transform.parent.gameObject.SetActive(false);
        CircleGoldSpr.SetActive(_goodstype == ePOPUPGOODSTYPE.CIRCLE_GOLD);
        CirclePointSpr.SetActive(_goodstype == ePOPUPGOODSTYPE.CIRCLE_POINT);
        
        kCashPlusBtnObj.SetActive(true);
        kTicketAPPlusBtnObj.SetActive(true);
        kTicketBPPlusBtnObj.SetActive(true);

        switch (_goodstype)
        {
            case ePOPUPGOODSTYPE.BASE:
                break;
            case ePOPUPGOODSTYPE.SKILLPOINT:
                {
                    kSkillPointGoodsUnit.transform.parent.gameObject.SetActive(true);
                }
                break;
            case ePOPUPGOODSTYPE.ROOMPOINT:
                {
                    kRoomPointGoodsUnit.transform.parent.gameObject.SetActive(true);
                }
                break;
            case ePOPUPGOODSTYPE.MPOINT:
                {
                    kPointGoodsUnit.transform.parent.gameObject.SetActive(true);
                }
                break;
            case ePOPUPGOODSTYPE.BASEBP:
                {
                    kTicketBPGoodsUnit.transform.parent.gameObject.SetActive(true);
                }
                break;
            case ePOPUPGOODSTYPE.BASE_NONE_BTN:
                {
                    kCashPlusBtnObj.SetActive(false);
                }
                break;
            case ePOPUPGOODSTYPE.SKILLPOINT_NONE_BTN:
                {
                    kSkillPointGoodsUnit.transform.parent.gameObject.SetActive(true);
                    kCashPlusBtnObj.SetActive(false);
                }
                break;
            case ePOPUPGOODSTYPE.ROOMPOINT_NONE_BTN:
                {
                    kRoomPointGoodsUnit.transform.parent.gameObject.SetActive(true);
                    kCashPlusBtnObj.SetActive(false);
                }
                break;
            case ePOPUPGOODSTYPE.MPOINT_NONE_BTN:
                {
                    kPointGoodsUnit.transform.parent.gameObject.SetActive(true);
                    kCashPlusBtnObj.SetActive(false);
                }
                break;
            case ePOPUPGOODSTYPE.BASEBP_NONE_BTN:
                {
                    kTicketBPGoodsUnit.transform.parent.gameObject.SetActive(true);
                    kCashPlusBtnObj.SetActive(false);
                }
                break;
            case ePOPUPGOODSTYPE.ARENAPOINT:
                {
                    kArenaGoldGoodsUnit.transform.parent.gameObject.SetActive(true);
                }
                break;
            case ePOPUPGOODSTYPE.ARENAPOINT_NONE_BTN:
                {
                    kArenaGoldGoodsUnit.transform.parent.gameObject.SetActive(true);
                    kCashPlusBtnObj.SetActive(false);
                }
                break;
            case ePOPUPGOODSTYPE.FRIENDPOINT:
                {
                    kFriendPointGoodsUnit.transform.parent.gameObject.SetActive(true);
                }
                break;
            case ePOPUPGOODSTYPE.FRIENDPOINT_NONE_BTN:
                {
                    kFriendPointGoodsUnit.transform.parent.gameObject.SetActive(true);
                    kCashPlusBtnObj.SetActive(false);
                }
                break;
            case ePOPUPGOODSTYPE.DESIREPOINT:
                {
                    kPointGoodsUnit.transform.parent.gameObject.SetActive(true);
                    kDesireGoodsUnit.transform.parent.gameObject.SetActive(true);
                    kTicketAPGoodsUnit.transform.parent.gameObject.SetActive(false);
                }
                break;
            case ePOPUPGOODSTYPE.DESIREPOINT_NONE_BTN:
                {
                    kPointGoodsUnit.transform.parent.gameObject.SetActive(true);
                    kDesireGoodsUnit.transform.parent.gameObject.SetActive(true);
                    kTicketAPGoodsUnit.transform.parent.gameObject.SetActive(false);
                    kCashPlusBtnObj.SetActive(false);
                }
                break;
            case ePOPUPGOODSTYPE.AWAKEN_SKILL_POINT:
                {
                    kSkillPointGoodsUnit.transform.parent.gameObject.SetActive(false);
                    WPGoodsUnit.transform.parent.gameObject.SetActive(true);
                }
                break;
            case ePOPUPGOODSTYPE.EVENTSTORE_POINT:
                {
                    kEventStorePointGoodsUnit.transform.parent.gameObject.SetActive(true);
                }
                break;
            case ePOPUPGOODSTYPE.RAID_POINT:
                RaidGoodsUnit.transform.parent.gameObject.SetActive( true );
                break;
            case ePOPUPGOODSTYPE.CIRCLE_POINT:
            case ePOPUPGOODSTYPE.CIRCLE_GOLD:
                {
                    CircleGoodsUnit.transform.parent.gameObject.SetActive(true);
                }
                break;
            case ePOPUPGOODSTYPE.NONE_BTN:
                {
                    kCashPlusBtnObj.SetActive(false);
                    kTicketAPPlusBtnObj.SetActive(false);
                    kTicketBPPlusBtnObj.SetActive(false);
                }
                break;
            default:
                break;
        }
        
        Renewal(true);
        OnPostAnimationEvent?.Invoke();
    }

    public void ShowGoodsStatePlay(ePOPUPGOODSTYPE e, int paneldepth)
    {
        _goodstype = e;
        _setdepth = paneldepth;
        if (!this.gameObject.activeSelf)
        {
            SetUIActive(true, true);
        }
        else
        {
            PlayAnimtion(2);
        }
    }

    public void HideGoodsStatePlay()
    {
        _goodstype = ePOPUPGOODSTYPE.BASE;
        SetUIActive(false, true);
    }

    public void ShowBPTicket(bool show)
    {
        kTicketBP.SetActive(show);
    }

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
}
