using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class UIFacilityPanel : FComponent
{
    public UILabel kLevelLabel;
    public UILabel kNameLabel;
    public UILabel kEffectDescLabel;
    public GameObject kInfo;
    public GameObject kLock;
    public UILabel kInfoEffectTitleLabel;
    public UILabel kInfoEffectDescLabel;
    public UILabel kInfoTimeTitleLabel;
    public UILabel kInfoTimeDescLabel;
    public UIGaugeUnit kTimeGaugeUnit;
    public UITexture kIconTex;
    public UISprite kIconBgSpr;
    public UIButton kActiveBtn;
    public UIButton kUpgradeBtn;
    public UIButton kCommonBtn;
    public UILabel kCommonBtnLabel;
    public UISprite kLockSpr;
    public UILabel kLockLevelLabel;
    public GameObject kChangeBtn;
    public GameObject kSpeedBtn;
    public GameObject kStopBtn;
    public GameObject kLevelUpEff;

    private int mFacilityId = 0;
    private FacilityData _facilitydata;
    private bool _bendsend;

    private string _strTimeInfoColor;       //서포터 장착 여부에 따라 보여지는 남은시간 색상 변경 변수
    
    private int _prevLv = -1;

    public List<GameObject> kFacilityBtns = new List<GameObject>();


    public GameObject kInfoNormalObj;
    [Header("Trade Card")]
    public GameObject kInfoTradeObj;
    public GameObject kTradeItemObj;
    public UILabel kTradeTimeTitleLabel;
    public UILabel kTradeTimeDescLabel;
    public UIGaugeUnit kTradeTimeGauge;
    public UITexture kTradeIconTexture;
    public UISprite kTradeIconBgSprite;
    public UISprite kTradeIconEmptySprite;
    public GameObject kTradeCompleteObj;
    public UILabel kTradeTimeResetLabel;
    public GameObject kTradeReadyObj;
    public UILabel kTradePercentLabel;

    public GameObject kTradeItemMaterialObj;
    public List<UIItemListSlot> kTradeItemList;
    public List<GameObject> kTradeItemEmptyList;
    public List<UISprite> kTradeItemEmptySprList;
    public List<GameObject> kTradeItemLockList;
    public List<UILabel> kTradeItemLockLabelList;
    
    public FToggle kFunctionSelectToggle;
    public List<GameObject> kFunctionSelectToggleList;
    public GameObject kFunctionSpr;
    public UILabel kFunctionLabel;

    public enum eTradeType { NONE = 0, CARD, WEAPON, }
    private eTradeType eCurTradeType = eTradeType.NONE;
    private string TradeTypePrefsKey = "FacilityTradeType";
    private GameTable.FacilityTradeAddon.Param _selectAddOnParam;
    
    private float FacilityDataDenominator = 0;
    private string str232 = "";
    private string str1124 = "";
    private string str1127 = "";
    private string str1654 = "";
    private string str1658 = "";

    private List<CardBookData> _precardbooklist = new List<CardBookData>();

    
	public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        var obj = UIValue.Instance.GetValue(UIValue.EParamType.FacilityID);
        if (obj == null)
            return;

        mFacilityId = (int)obj;

        if (_facilitydata != null)
        {
            if (mFacilityId != _facilitydata.TableID)
                _prevLv = -1;
        }
        _facilitydata = GameInfo.Instance.GetFacilityData(mFacilityId);

        _bendsend = false;

        if (_prevLv < 0)
            _prevLv = _facilitydata.Level;

        if (!_prevLv.Equals(_facilitydata.Level))
        {
            _prevLv = _facilitydata.Level;
            kLevelUpEff.SetActive(true);
        }
        else
        {
            kLevelUpEff.SetActive(false);
        }

        InitTradeInfo();

        FacilityDataDenominator = GameSupport.GetFacilityTime(_facilitydata) * 60;
        str232 = FLocalizeString.Instance.GetText(232);
        str1124 = FLocalizeString.Instance.GetText(1124);
        str1127 = FLocalizeString.Instance.GetText(1127);
        str1654 = FLocalizeString.Instance.GetText(1654);
        str1658 = FLocalizeString.Instance.GetText(1658);

        kFunctionSelectToggle.EventCallBack = _OnEventTabSelect;

        _selectAddOnParam = null;
    }

    private bool _OnEventTabSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Click)
        {
            if (_facilitydata.Stats == (int)eFACILITYSTATS.USE)
            {
                return false;
            }
            
            UIFacilityFunctionSelectPopup popup = LobbyUIManager.Instance.GetUI<UIFacilityFunctionSelectPopup>("FacilityFunctionSelectPopup");
            if (popup != null)
            {
                GameTable.Item.Param itemParam = null;
                if (_selectAddOnParam != null)
                {
                    itemParam = GameInfo.Instance.GameTable.FindItem(_selectAddOnParam.AddItemID);
                }
                popup.SetSelectItemData(itemParam);
                popup.SetSelectType((int)eCurTradeType);
                popup.SetUIActive(true);
            }
            return false;
        }
        
        for (int i = 0; i < nSelect; i++)
        {
            kFunctionSelectToggleList[i].SetActive(i == nSelect);
        }
        
        kFunctionSpr.SetActive(nSelect == 1);
        if (kFunctionSpr.activeSelf)
        {
            kFunctionLabel.textlocalize = FLocalizeString.Instance.GetText(_selectAddOnParam?.Name ?? 0);
        }
        
        return true;
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
        if (_facilitydata == null)
            return;

        kEffectDescLabel.textlocalize = FLocalizeString.Instance.GetText(_facilitydata.TableData.Desc);

        if (_facilitydata.Level >= _facilitydata.TableData.MaxLevel)
            kUpgradeBtn.gameObject.SetActive(false);
        else
            kUpgradeBtn.gameObject.SetActive(true);

        kInfo.gameObject.SetActive(false);
        kLock.gameObject.SetActive(false);

        for (int i = 0; i < kFacilityBtns.Count; i++)
        {
            FIndex fIdx = kFacilityBtns[i].GetComponent<FIndex>();

            if(fIdx.kIndex == (_facilitydata.TableID - 1))
            {
                kFacilityBtns[i].transform.Find("dis").gameObject.SetActive(true);
            }
            else
            {
                kFacilityBtns[i].transform.Find("dis").gameObject.SetActive(false);
            }
        }

        kInfoNormalObj.SetActive(true);
        kInfoTradeObj.SetActive(false);
        SetTradeObject(false);

        if (_facilitydata.Level == 0)
        {
            kLevelLabel.textlocalize = "";
            kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_facilitydata.TableData.Name);
            kLock.gameObject.SetActive(true);
            kInfoEffectTitleLabel.gameObject.SetActive(false);
            kInfoEffectDescLabel.gameObject.SetActive(false);
            if (GameInfo.Instance.UserData.Level < _facilitydata.TableData.FacilityOpenUserRank)
            {
                kActiveBtn.gameObject.SetActive(false);
                kLockSpr.gameObject.SetActive(true);
                kLockLevelLabel.gameObject.SetActive(true);
                kLockLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.RANK_TXT_NOW_LV), _facilitydata.TableData.FacilityOpenUserRank);
            }
            else
            {
                kActiveBtn.gameObject.SetActive(true);
                kLockSpr.gameObject.SetActive(false);
                kLockLevelLabel.gameObject.SetActive(false);
            }
        }
        else
        {
            if (_facilitydata.TableData != null && _facilitydata.TableData.EffectType == "FAC_CARD_TRADE")
            {
                Renewal_Trade();
                return;
            }

            kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_facilitydata.TableData.Name);
            kLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), _facilitydata.Level);
            kLevelLabel.transform.localPosition = new Vector3(kNameLabel.transform.localPosition.x + kNameLabel.printedSize.x + 10, kNameLabel.transform.localPosition.y, 0);
            kInfo.gameObject.SetActive(true);
            kInfoEffectTitleLabel.gameObject.SetActive(true);
            kInfoEffectDescLabel.gameObject.SetActive(true);

            //서포터 등록여부에 따라 색상 변경 - 기본 흰색 / 등록시 초록색
            _strTimeInfoColor = FLocalizeString.Instance.GetText((int)eTEXTID.WHITE_TEXT_COLOR);
            CardData carddata = GameInfo.Instance.GetCardData(_facilitydata.EquipCardUID);
            if(carddata != null)
            {
                _strTimeInfoColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
                kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Item/{0}.png", carddata.TableData.Icon)); 
                kIconTex.gameObject.SetActive(true);
                kIconBgSpr.spriteName = "itembgSlot_weapon_" + carddata.TableData.Grade.ToString();
                kIconBgSpr.gameObject.SetActive(true);
            }
            else
            {
                kIconTex.gameObject.SetActive(false);
                kIconBgSpr.gameObject.SetActive(false);
            }

            //효과 타이틀 및 적용 효과값 텍스트 적용 및 좌표 수정
            kInfoEffectTitleLabel.textlocalize = FLocalizeString.Instance.GetText(_facilitydata.TableData.EffectDesc);
            kInfoEffectDescLabel.textlocalize = string.Format("{0:#,##0}", GameSupport.GetFacilityEffectValue(_facilitydata, true));
            kInfoEffectDescLabel.transform.localPosition = new Vector3(kInfoEffectTitleLabel.transform.localPosition.x + kInfoEffectTitleLabel.printedSize.x + 10, kInfoEffectTitleLabel.transform.localPosition.y, 0);

            //필요시간
            kInfoTimeTitleLabel.textlocalize = FLocalizeString.Instance.GetText(1126);

            float time = GameSupport.GetFacilityTime(_facilitydata) * 60;

            kInfoTimeDescLabel.textlocalize = string.Format(_strTimeInfoColor, GameSupport.GetFacilityTimeString(time));

            if ( _facilitydata.Stats == (int)eFACILITYSTATS.WAIT )
            {
                FacilityButtonActivetion(true);

                kCommonBtnLabel.textlocalize = FLocalizeString.Instance.GetText(1095);
                kTimeGaugeUnit.InitGaugeUnit(0f);
                kChangeBtn.SetActive(true);
            }
            else
            {
                kInfoTimeTitleLabel.textlocalize = FLocalizeString.Instance.GetText(1127);
                kChangeBtn.SetActive(false);
                var diffTime = _facilitydata.RemainTime - GameSupport.GetCurrentServerTime();
                if (diffTime.Ticks > 0)     //진행중
                {
                    FacilityButtonActivetion(false);

                    System.DateTime NowTime = GameSupport.GetCurrentServerTime();
                    string strremain = GameSupport.GetFacilityRemainTimeString(_facilitydata.RemainTime, NowTime);
                    kInfoTimeDescLabel.textlocalize = string.Format(_strTimeInfoColor, strremain);

                    kCommonBtnLabel.textlocalize = FLocalizeString.Instance.GetText(1124);

                    kTimeGaugeUnit.InitGaugeUnit(0.0f);

                }
                else  //완료
                {
                    FacilityButtonActivetion(true);

                    kInfoTimeDescLabel.textlocalize = string.Format(_strTimeInfoColor, FLocalizeString.Instance.GetText(1125));
                    kTimeGaugeUnit.InitGaugeUnit(1.0f);
                    kCommonBtnLabel.textlocalize = FLocalizeString.Instance.GetText(1125);
                }
            }
        }

    }

    private void FixedUpdate()
    {
        FixedUpdate_Timer();
    }

    private void FixedUpdate_Timer()
    {
        if (_facilitydata == null)
            return;

        if (_facilitydata.Level == 0)
            return;

        if (_facilitydata.Stats == (int)eFACILITYSTATS.WAIT)
        {
            kChangeBtn.SetActive(true);
            return;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!GameInfo.Instance.netFlag)
                _facilitydata.RemainTime = GameSupport.GetCurrentServerTime();
        }
#endif

        kChangeBtn.SetActive(false);
        var diffTime = _facilitydata.RemainTime - GameSupport.GetCurrentServerTime();

        if (diffTime.Ticks < 0)
        {
            FacilityButtonActivetion(true);
            kInfoTimeDescLabel.textlocalize = string.Format(_strTimeInfoColor, FLocalizeString.Instance.GetText(1125));
            kTradeTimeDescLabel.textlocalize = string.Format(_strTimeInfoColor, FLocalizeString.Instance.GetText(1125));

            kTimeGaugeUnit.InitGaugeUnit(1f);
            kCommonBtnLabel.textlocalize = FLocalizeString.Instance.GetText(1125);
            kTradeTimeResetLabel.textlocalize = FLocalizeString.Instance.GetText(1659);

            Lobby.Instance.CompleteFacility(_facilitydata.TableID);
            return;
        }
        
        float f = 1.0f - ((float)diffTime.TotalSeconds / FacilityDataDenominator);

        System.DateTime NowTime = GameSupport.GetCurrentServerTime();
        string strremain = GameSupport.GetFacilityRemainTimeString(_facilitydata.RemainTime, NowTime, str232);

        if (_facilitydata.TableData != null && _facilitydata.TableData.EffectType == "FAC_CARD_TRADE")
        {
            kTradeTimeDescLabel.textlocalize = string.Format(_strTimeInfoColor, strremain);
            kTradeTimeTitleLabel.textlocalize = str1654;
            kTradeTimeGauge.InitGaugeUnit(f);

            kTradeTimeResetLabel.textlocalize = str1658;
        }
        else
        {
            kInfoTimeDescLabel.textlocalize = string.Format(_strTimeInfoColor, strremain);

            kInfoTimeTitleLabel.textlocalize = str1127;
            kCommonBtnLabel.textlocalize = str1124;
            kTimeGaugeUnit.InitGaugeUnit(f);
        }
    }
    
    public void OnClick_Upgrade()
    {
        if (_facilitydata == null)
            return;
        if (_facilitydata.Level >= _facilitydata.TableData.MaxLevel)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3007));
            return;
        }

        if( _facilitydata.Stats != (int)eFACILITYSTATS.WAIT )
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3024));
            return;
        }

        UIFacilityUpGradePopup popup = LobbyUIManager.Instance.GetUI<UIFacilityUpGradePopup>("FacilityUpGradePopup");
        if (!popup)
        {
            return;
        }

        popup.FacilityId = mFacilityId;
        LobbyUIManager.Instance.ShowUI("FacilityUpGradePopup", true);
        /*
        LobbyUIManager.Instance.HideUI("TopPanel", false);
        LobbyUIManager.Instance.HideUI("FacilityPanel", false);
        LobbyUIManager.Instance.HideUI("FacilityItemPanel", false);
        */
    }

    public void OnClick_Active()
    {
        if (GameInfo.Instance.UserData.Level < _facilitydata.TableData.FacilityOpenUserRank)
        {
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3012), _facilitydata.TableData.FacilityOpenUserRank));
            return;
        }

        UIFacilityUpGradePopup popup = LobbyUIManager.Instance.GetUI<UIFacilityUpGradePopup>("FacilityUpGradePopup");
        if (!popup)
        {
            return;
        }

        popup.FacilityId = mFacilityId;
        LobbyUIManager.Instance.ShowUI("FacilityUpGradePopup", true);


        /*
        LobbyUIManager.Instance.HideUI("TopPanel", false);
        LobbyUIManager.Instance.HideUI("FacilityPanel", false);
        LobbyUIManager.Instance.HideUI("FacilityItemPanel", false);
        */
    }

    public void OnClick_Confirm()
    {
        if (_facilitydata == null)
            return;
        if (_facilitydata.Level == 0)
            return;

        FacilityDataDenominator = GameSupport.GetFacilityTime(_facilitydata) * 60;

        if (_facilitydata.Stats == (int)eFACILITYSTATS.WAIT)
        {
            kChangeBtn.SetActive(true);
            if (_facilitydata.TableData.EffectType == "FAC_ITEM_COMBINE")
            {
                SendFacilityUse(GameInfo.Instance.GetMainCharUID());
            }
            else
            {
                bool charFlag = false;
                for (int i = 0; i < GameInfo.Instance.CharList.Count; i++)
                {
                    if (GameInfo.Instance.GetEquipCharFacilityData(GameInfo.Instance.CharList[i].CUID) == null)
                    {
                        charFlag = true;
                        break;
                    }
                }

                if(!charFlag)
                {
                    MessageToastPopup.Show(FLocalizeString.Instance.GetText(3037));
                    return;
                }

                UIValue.Instance.SetValue(UIValue.EParamType.CharSeletePopupType, (int)eCharSelectFlag.FACILITY);
                LobbyUIManager.Instance.ShowUI("CharSeletePopup", true);
            }
        }
        else
        {
            kChangeBtn.SetActive(false);
            var diffTime = _facilitydata.RemainTime - GameSupport.GetCurrentServerTime();
            if (diffTime.Ticks > 0)     //진행중
            {
                //  진행중을 취소시 확인 메세지
                MessagePopup.CYN(eTEXTID.OK,
                                 3055,      //  취소하시겠습니까?
                                 eTEXTID.OK, 
                                 eTEXTID.CANCEL, 
                                 () => { GameInfo.Instance.Send_ReqFacilityCancel(_facilitydata.TableID, OnNetFacilityCancel); }, 
                                 null);
            }
            else  //완료
            {
                GameInfo.Instance.Send_ReqFacilityComplete(_facilitydata.TableID, 0, PktInfoFacilityOperConfirmReq.TYPE._NONE_, OnNetFacilityComplete);
            }
        }
    }

    public void OnClick_SuppoterChanegeBtn()
    {
        if (_facilitydata == null)
            return;
        if (_facilitydata.Level == 0)
            return;

        if (_facilitydata.Stats != (int)eFACILITYSTATS.WAIT)
            return;

        if (GameInfo.Instance.CardList.Count == 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3020));
            return;
        }

        //서포터 목록에 표시할게 있는지 체크
        bool cardFlag = false;
        for (int i = 0; i < GameInfo.Instance.CardList.Count; i++)
        {
             if (_facilitydata.EquipCardUID == GameInfo.Instance.CardList[i].CardUID ||
                (GameSupport.IsEquipAndUsingCardData(GameInfo.Instance.CardList[i].CardUID) == false &&
                GameInfo.Instance.GetEquiCardCharData(GameInfo.Instance.CardList[i].CardUID) == null))
            {
                cardFlag = true;
                break;
            }
        }

        if(!cardFlag)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3038));
            return;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.FacilityID, _facilitydata.TableID);
        LobbyUIManager.Instance.ShowUI("FacilityCardSeletePopup", true);
    }

    public void SendFacilityUse( long uid )
    {
        CharData chardata = GameInfo.Instance.GetCharData(uid);
        if (chardata == null)
            return;

        //190820
        //아이템 갯수가 추가되어 서버에 문의
        //아이템 조합이 아닐경우 1로 넘겨달라고 해서 강제로 1로 밖아둡니다.
        GameInfo.Instance.Send_ReqFacilityOperation(_facilitydata.TableID, uid, 1, null, OnNetFacilityUse);
    }


    public void OnNetFacilityUse(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("FacilityPanel");
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.HideUI("CharSeletePopup");

        Lobby.Instance.ActivationRoomEffect(_facilitydata.TableID);

        var carddata = GameInfo.Instance.GetCardData(_facilitydata.EquipCardUID);
        if(carddata != null)
        {
            VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.FacilityStart, carddata.TableID);
        }
        
        Renewal(true);
    }

    public void OnNetFacilityCancel(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
        Lobby.Instance.DisabledRoomEffect(_facilitydata.TableID);
        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3027));
        Renewal(true);
    }

    public void OnNetFacilityComplete(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");

        PktInfoFacilityOperConfirmAck pktInfo = pktmsg as PktInfoFacilityOperConfirmAck;
        if(pktInfo != null)
        {
            if(!pktInfo.operEndFlag_)
            {
                OnNetFacilityCancel(0, pktmsg);
                return;
            }
        }

        Renewal(true);
                
        Lobby.Instance.DisabledRoomEffect(_facilitydata.TableID);
        LobbyUIManager.Instance.ShowUI("FacilityResultPopup", true);
        
        LobbyUIManager.Instance.CheckAddSpecialPopup();
        LobbyUIManager.Instance.ShowAddSpecialPopup();

        LocalPushNotificationManager.Instance.RemoveFacilityNotification(_facilitydata.TableID);
    }

    public override bool IsBackButton()
    {
        return true;
    }

    public void OnClick_MoveFacility(int idx)
    {
        if (idx == (_facilitydata.TableID - 1))
        {
            return;
        }

        InitTradeInfo();

        var data = GameInfo.Instance.FacilityList[idx];
        //LobbyUIManager.Instance.ChangeFacility("FAC_WEAPON_EXP");
        LobbyUIManager.Instance.ChangeFacility(data.TableData.EffectType);
    }
    
    public void OnClick_SpeedUpBtn()
    {
        if (_facilitydata.TableData.EffectType == "FAC_CARD_TRADE")
        {
            var diffTime = _facilitydata.RemainTime - GameSupport.GetCurrentServerTime();
            if (diffTime.Ticks < 0)     //완료
            {
                GameInfo.Instance.Send_ReqFacilityComplete(_facilitydata.TableID, 0, PktInfoFacilityOperConfirmReq.TYPE._NONE_, OnNetFacilityComplete);
                return;
            }
        }

        int needItemCnt = GameSupport.GetFacilityNeedSpeedItem(_facilitydata);

        if(GameInfo.Instance.GetItemIDCount(GameSupport.GetFacilitySpeedItemTableID()) < needItemCnt)
        {
            Log.Show("대마석 구입 팝업!!!");
            //10021 시설 시간 단축 아이템
            MessagePopup.CYNItemCash(
                FLocalizeString.Instance.GetText(1325),
                FLocalizeString.Instance.GetText(3064),
                eTEXTID.OK,
                eTEXTID.CANCEL,
                GameSupport.GetFacilitySpeedItemTableID(),
                needItemCnt,
                OnClickSpeedUpMessageOk,
                null);
        }
        else
        {
            //10021 시설 시간 단축 아이템
            MessagePopup.CYNItem(
                FLocalizeString.Instance.GetText(1325),
                FLocalizeString.Instance.GetText(3064),
                eTEXTID.OK,
                eTEXTID.CANCEL,
                GameSupport.GetFacilitySpeedItemTableID(),
                needItemCnt,
                OnClickSpeedUpMessageOk,
                null);
        }


        
    }

    public void OnClickSpeedUpMessageOk()
    {
        int needItemCnt = GameSupport.GetFacilityNeedSpeedItem(_facilitydata);

        GameTable.Item.Param speedItem = GameInfo.Instance.GameTable.FindItem(x => x.Type == (int)eITEMTYPE.MATERIAL && x.SubType == (int)eITEMSUBTYPE.MATERIAL_FACILITY_TIME);
        if (speedItem == null)
            return;

        int userItemCnt = GameInfo.Instance.GetItemIDCount(speedItem.ID);

        //즉시완료 성공
        if(userItemCnt >= needItemCnt)
        {
            var userItem = GameInfo.Instance.GetItemData(speedItem.ID);
            if(userItem != null)
            {
                GameInfo.Instance.Send_ReqFacilityComplete(_facilitydata.TableID, userItem.TableID, PktInfoFacilityOperConfirmReq.TYPE.ITEM, OnNetFacilityComplete);
            }
        }
        else //대마석으로 구입
        {
            //소재가 부족합니다.
            //MessageToastPopup.Show(FLocalizeString.Instance.GetText(3003));
            if (!GameSupport.IsCheckGoods(eGOODSTYPE.CASH, needItemCnt))
                return;

            GameInfo.Instance.Send_ReqFacilityComplete(_facilitydata.TableID, speedItem.ID, PktInfoFacilityOperConfirmReq.TYPE.CASH, OnNetFacilityComplete);
        }
    }

    public void SetAddOnItem(GameTable.FacilityTradeAddon.Param param)
    {
        _selectAddOnParam = param;
    }

    private void FacilityButtonActivetion(bool enable)
    {
        kCommonBtn.gameObject.SetActive(enable);
        kSpeedBtn.gameObject.SetActive(!enable);
        kStopBtn.gameObject.SetActive(!enable);
    }
}
