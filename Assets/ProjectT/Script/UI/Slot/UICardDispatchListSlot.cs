
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UICardDispatchListSlot : FSlot
{
    public UIItemListSlot itemSlot = null;

    public UISprite sprComplete;
    public UISprite sprDifficulty;    

    public UIGaugeUnit gaugeTime;

    public UILabel lblMissionName;
    public UILabel lblTime;
    public UILabel lblLockLv;
    public UILabel lblDispatchCondition;

    public UIGoodsUnit GoodsUnit;

    public UIButton btnConfirm;
    public UIButton btnCancel;
    public UIButton btnComplete;
    public UIButton btnOpen;
    public UIButton btnMissionChange;
    public UIButton btnAuto;
    public UIButton ImmediatelyDoneBtn;

    public TweenAlpha twAlphaOpen;

    public GameObject goRedTitleBG;
    public GameObject goYellowTitleBG;
    public GameObject goCondition;
    public GameObject goCompleteEffect;
    public GameObject goProgress;
    public GameObject goOpenEffect;
    public GameObject goLock;
    public GameObject goDispatchCondition;

    public List<UICardDispatchUnit> listCardDispatchUnit;

    private int Index;
    private GameTable.CardDispatchSlot.Param TableSlotParam;
    private Dispatch _dispatchData;    
    private GameTable.CardDispatchMission.Param TableMissionParam;

    private enum eState { NONE = 0, CLOSE, OPEN, DISPATCHING, COMPLETE, MAX }
    private eState State = eState.NONE;

    private int OpenUnitCount = 0;
    private int CompleteMissionTID = 0;

    private string strTimerFormat = "";
    public void SelfUpdate()
    {
        UpdateSlot(Index, TableSlotParam);
    }

    public void UpdateSlot(int index, GameTable.CardDispatchSlot.Param param)   //Fill parameter if you need
    {
        strTimerFormat = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);

        Index = index;
        TableSlotParam = param;

        Init();
        SetData();
        State = GetState();
        switch (State)
        {
            case eState.CLOSE: SetClose(); break;
            case eState.OPEN: SetOpen(); break;
            case eState.DISPATCHING: SetDispatching(); break;
            case eState.COMPLETE: SetComplete(); break;
        }
    }

    public void OnBtnImmediatelyDone() {
        MessagePopup.CYNItem( FLocalizeString.Instance.GetText( 1325 ), FLocalizeString.Instance.GetText( 3064 ), eTEXTID.OK, eTEXTID.CANCEL,
                              GameInfo.Instance.GameConfig.CardDispatchFastItemID, GameInfo.Instance.GameConfig.CardDispatchFastItemValue,
                              SendReqImmediatelyDone, null );
    }

    private void Init()
    {
        _dispatchData = null;
        CompleteMissionTID = 0;

        itemSlot.SetActive(false);

        sprComplete.SetActive(false);
        sprComplete.SetActive(false); 
        sprDifficulty.SetActive(false); 

        gaugeTime.SetActive(false); 

        lblMissionName.SetActive(false); 
        lblTime.SetActive(false); 
        lblLockLv.SetActive(false);

        GoodsUnit.SetActive(false);

        btnConfirm.SetActive(false); 
        btnCancel.SetActive(false); 
        btnComplete.SetActive(false); 
        btnOpen.SetActive(false); 
        btnMissionChange.SetActive(false);
        btnAuto.SetActive(false);
        ImmediatelyDoneBtn.SetActive( false );

        goRedTitleBG.SetActive(false);
        goYellowTitleBG.SetActive(false);
        goCondition.SetActive(false);
        goCompleteEffect.SetActive(false);
        goProgress.SetActive(false);
        goOpenEffect.SetActive(false);
        goLock.SetActive(false);
        goDispatchCondition.SetActive(false);

        foreach(var go in listCardDispatchUnit)
        {
            go.Init();
            go.SetActive(false);
        }
    }

    private void SetData()
    {
        _dispatchData = GameInfo.Instance.Dispatches.Find(x => x.TableID == Index + 1);
        if (_dispatchData == null)
        {
            return;
        }

        TableMissionParam = GameInfo.Instance.GameTable.CardDispatchMissions.Find(x => x.ID == _dispatchData.MissionID);
    }

    private eState GetState()
    {
        if (_dispatchData == null) return eState.CLOSE;

        if (_dispatchData.EndTime <= DateTime.MinValue || _dispatchData.EndTime >= DateTime.MaxValue || _dispatchData.EndTime.Ticks <= 0)
            return eState.OPEN;

        var diffTime = _dispatchData.EndTime - GameSupport.GetCurrentServerTime();
        if (diffTime.Ticks < 0)     //완료
        {
            return eState.COMPLETE;
        }

        return eState.DISPATCHING;
    }

    private void SetClose()
    {
        goLock.SetActive(true);

        int previousIndex = TableSlotParam.Index - 1;

        if(previousIndex <= 0)
            btnOpen.SetActive(true);
        else
        {
            if(GameInfo.Instance.Dispatches.Find(x => x.TableID == previousIndex) == null)
                btnOpen.SetActive(false);
            else
                btnOpen.SetActive(true);
        }
        lblLockLv.SetActive(true);
        lblLockLv.textlocalize = string.Format(FLocalizeString.Instance.GetText(210), TableSlotParam.NeedRank);

        GoodsUnit.SetActive(true);
        GoodsUnit.InitGoodsUnit((eGOODSTYPE)TableSlotParam.OpenGoods, TableSlotParam.OpenValue, true);
    }

    private void SetOpen()
    {
        SetCommonData(true);

        goRedTitleBG.SetActive(true);
        btnMissionChange.SetActive(true);

        //빈 슬롯이 있으면 Auto버튼 켜기
        //아니면 Confirm버튼 켜기
        if (IsAutoEnable()) btnAuto.SetActive(true);
        else btnConfirm.SetActive(true);

        if (TableMissionParam.NeedURCnt > 0)
        {
            goCondition.SetActive(true);
            lblDispatchCondition.textlocalize = string.Format("UR x{0}", TableMissionParam.NeedURCnt);
        }

        List<CardData> supports = GameInfo.Instance.CardList.FindAll(x =>
            x.PosValue == _dispatchData.TableID &&
            x.PosKind == (int)eContentsPosKind.DISPATCH);

        if (supports != null)
        {
            for (int i = 0; i < supports.Count; i++)
            {
                SetCardDispatchUnit(supports[i], true);
            }
        }
    }

    private void SetDispatching()
    {
        SetCommonData(false);
        
        goRedTitleBG.SetActive(true);
        gaugeTime.SetActive(true);
        goProgress.SetActive(true);
        ImmediatelyDoneBtn.SetActive( true );

        var anim = goProgress.GetComponent<Animation>();
        if (anim) anim.Play();

        List<CardData> supports = GameInfo.Instance.CardList.FindAll(x => 
            x.PosValue == _dispatchData.TableID && 
            x.PosKind == (int)eContentsPosKind.DISPATCH);

        for (int i = 0; i < supports.Count; i++)
        {
            SetCardDispatchUnit(supports[i], false);
        }

        btnCancel.SetActive(true);
    }

    private void SetComplete()
    {
        SetCommonData(false);

        goYellowTitleBG.SetActive(true);
        goCompleteEffect.SetActive(true);
        lblTime.SetActive(false);

        btnComplete.SetActive(true);

        List<CardData> supports = GameInfo.Instance.CardList.FindAll(x =>
            x.PosValue == _dispatchData.TableID &&
            x.PosKind == (int)eContentsPosKind.DISPATCH);

        for (int i = 0; i < supports.Count; i++)
        {
            SetCardDispatchUnit(supports[i], false);
        }
    }

    private void SetCommonData(bool isChange)
    {
        itemSlot.SetActive(true);
        lblMissionName.SetActive(true);
        sprDifficulty.SetActive(true);
        lblTime.SetActive(true);

        lblMissionName.textlocalize = FLocalizeString.Instance.GetText(TableMissionParam.Name);
        sprDifficulty.spriteName = string.Format("icondifficulty_{0:00}", TableMissionParam.Grade);

        float time = TableMissionParam.Time * 60f;
        if (IsAutoEnable())
            lblTime.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR), GameSupport.GetFacilityTimeString(time));
        else
            lblTime.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.WHITE_TEXT_COLOR), GameSupport.GetFacilityTimeString(time));

        // Reward
        itemSlot.ParentGO = this.gameObject;
        itemSlot.UpdateSlot(TableMissionParam);

        // Unit 상태 설정
        OpenUnitCount = 0;
        SetDefaultUnit(0, TableMissionParam.SocketType1, isChange);
        SetDefaultUnit(1, TableMissionParam.SocketType2, isChange);
        SetDefaultUnit(2, TableMissionParam.SocketType3, isChange);
        SetDefaultUnit(3, TableMissionParam.SocketType4, isChange);
        SetDefaultUnit(4, TableMissionParam.SocketType5, isChange);
    }

    private void SetCardDispatchUnit(CardData data, bool isChange)
    {
        if (data == null || listCardDispatchUnit.Count <= data.PosSlot)
            return;

        if (listCardDispatchUnit == null || listCardDispatchUnit.Count <= 0)
            return;

        listCardDispatchUnit[data.PosSlot].SetData(data, _dispatchData, isChange);
    }

    private void SetDefaultUnit(int index, int SokectType, bool isChange)
    {
        if (SokectType <= 0) return;
        listCardDispatchUnit[index].SetData(null, _dispatchData, isChange);
        OpenUnitCount++;
    }

    private void FixedUpdate()
    {
        if (State == eState.DISPATCHING)
        {
            var diffTime = _dispatchData.EndTime - GameSupport.GetCurrentServerTime();
            if (diffTime.Ticks < 0)     //완료
            {
                State = eState.COMPLETE;
                SelfUpdate();                
                return;
            }

            //진행중            
            float workTime = _dispatchData.TableData.Time * 60f;            
            float f = 1.0f - ((float)diffTime.TotalSeconds / (float)workTime);
            gaugeTime.InitGaugeUnit(f);
            lblTime.textlocalize = string.Format(strTimerFormat, string.Format("{0:00}:{1:00}:{2:00}", (int)diffTime.TotalHours, (int)diffTime.Minutes, (int)diffTime.Seconds));
        }
    }

#region OnClick
    public void OnClick_Open()
    {
        //랭크확인        
        if(GameInfo.Instance.UserData.Level < TableSlotParam.NeedRank)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3205));            
            return;
        }

        //재화확인
        if (!GameSupport.IsCheckGoods((eGOODSTYPE)TableSlotParam.OpenGoods, TableSlotParam.OpenValue))
            return;

        string text = FLocalizeString.Instance.GetText(3204);
        MessagePopup.CYN(eTEXTID.TITLE_BUY, text, eTEXTID.YES, eTEXTID.NO, (eGOODSTYPE)TableSlotParam.OpenGoods, TableSlotParam.OpenValue, 
            () => { GameInfo.Instance.Send_ReqDispatchOpen((uint)TableSlotParam.Index, OnNetAckDispatchOpen); });
    }

    public void OnClick_Confirm()
    {
        // 편성된 서포터 리스트 확인
        List<CardData> supports = GameInfo.Instance.CardList.FindAll(x => 
            x.PosValue == Index +1 && 
            x.PosKind == (int)eContentsPosKind.DISPATCH);

        if(supports == null || supports.Count == 0)
        {
            //편성된 서포터가 없습니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1037));            
            return;
        }

        // 서포터 편성이 필요합니다.
        if (supports.Count < OpenUnitCount)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3202));            
            return;
        }

        // UR 서포터 체크
        if (TableMissionParam.NeedURCnt > 0)
        {
            int curURCount = 0;
            for (int i = 0; i < supports.Count; i++)
            {
                if (supports[i].TableData.Grade == (int)eGRADE.GRADE_UR)
                    curURCount++;
            }

            if(TableMissionParam.NeedURCnt > curURCount)
            {
                // UR 등급의 서포터가 부족합니다.
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3203));                
                return;
            }
        }

        //PosSlot으로 정렬(오름차순)
        supports.Sort(delegate (CardData l, CardData r)
        {
            if (l.PosSlot > r.PosSlot) return 1;
            else if (l.PosSlot < r.PosSlot) return -1;
            return 0;
        });

        List<long> SelectSupportIDs = new List<long>();
        foreach (var s in supports)
            SelectSupportIDs.Add(s.CardUID);

        GameInfo.Instance.Send_ReqDispatchOper((uint)_dispatchData.TableID, SelectSupportIDs, OnNetAckDispatchOper);
    }

    public void OnClick_Complete()
    {
        CompleteMissionTID = TableMissionParam.ID;
        GameInfo.Instance.Send_ReqDispatchOperConfirm((uint)_dispatchData.TableID, false, OnNetAckDispatchOperConfirm);       
    }

    public void OnClick_Cancel()
    {
        //취소 하시겠습니까?
        MessagePopup.OKCANCEL(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3055),
            () => { GameInfo.Instance.Send_ReqDispatchOperConfirm((uint)_dispatchData.TableID, false, OnNetAckDispatchOperConfirm ); });
    }

    public void OnClick_Change()
    {
        //재화확인
        if (!GameSupport.IsCheckGoods((eGOODSTYPE)TableSlotParam.ChangeGoods, TableSlotParam.ChangeValue))
            return;

        string text = FLocalizeString.Instance.GetText(3206);
        MessagePopup.CYN(eTEXTID.TITLE_BUY, text, eTEXTID.YES, eTEXTID.NO, (eGOODSTYPE)TableSlotParam.ChangeGoods, TableSlotParam.ChangeValue,
            () => { GameInfo.Instance.Send_ReqDispatchChange((uint)_dispatchData.TableID, OnNetAckDispatchChange); });
    }

    public void OnClick_Auto()
    {
        /*
         PosValue : _dispatchData.TableID
         PosKind :(int)eContentsPosKind.DISPATCH
         PosSlot : slot
        */
        List<int> UsingCardTableID = new List<int>();

        var UnUsedSupport = GameInfo.Instance.CardList.FindAll(x =>
        {
            return x.PosKind == 0 && x.PosValue == 0 && x.PosSlot == 0;
        });

        if (UnUsedSupport == null || UnUsedSupport.Count <= 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3202));
            return;
        }

        int assignURCnt = 0;

        List<UICardDispatchUnit> EmptyUnitList = new List<UICardDispatchUnit>();
        foreach(var unit in listCardDispatchUnit)
        {
            if (!unit.IsActive) continue;
            if (!unit.IsEmpty)
            {
                if (unit.SupportCardData.TableData.Grade == (int)eGRADE.GRADE_UR)
                    assignURCnt++;

                UsingCardTableID.Add(unit.SupportCardData.TableID);
                continue;
            }

            EmptyUnitList.Add(unit);
        }

        if (EmptyUnitList.Count <= 0) return;

        //필요 속성의 오름차순 정렬 (ALL이 가장 늦게 할당되기 위함)
        EmptyUnitList.Sort(delegate (UICardDispatchUnit l, UICardDispatchUnit r)
        {
            if (l.CardType > r.CardType) return 1;
            else if (l.CardType < r.CardType) return -1;
            return 0;
        });

        // 서포터 할당 시작
        List<CardData> tmpOptcardlist = new List<CardData>();

        foreach (var unit in EmptyUnitList)
        {
            tmpOptcardlist.Clear();
            tmpOptcardlist = GameInfo.Instance.CardList.FindAll(x =>
            {
                if (x.PosKind != 0 || x.PosValue != 0 || x.PosSlot != 0)
                    return false;

                foreach (int id in UsingCardTableID)                
                    if (id == x.TableID) return false;                

                if (unit.CardType == (int)eCARDTYPE.ALL)
                    return true;

                if (unit.CardType == x.Type)
                    return true;                

                return false;
            });

            if (tmpOptcardlist == null || tmpOptcardlist.Count <= 0)
                continue;

            if (IsNeedSupport_UR()) 
                tmpOptcardlist.Sort(CompareGradeDown);
            else 
                tmpOptcardlist.Sort(CompareGradeUp);

            var select = tmpOptcardlist[0];

            //UR카드 사용 카운트 체크
            if(select.TableData.Grade == (int)eGRADE.GRADE_UR)
            {
                assignURCnt++;
            }

            select.SetPos(unit.DispatchData.TableID, (int)eContentsPosKind.DISPATCH, unit.Index);
            unit.DispatchData.RefreshData();
            SetCardDispatchUnit(select, false);

            UsingCardTableID.Add(select.TableID);
        }
        UsingCardTableID = null;

        SelfUpdate();

        if(TableMissionParam.NeedURCnt > 0 && TableMissionParam.NeedURCnt > assignURCnt)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3203));
            return;
        }

        foreach (var unit in EmptyUnitList)
        {
            if (unit.IsEmpty)
            {
                // 서포터가 부족합니다.
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3202));
                break;
            }
        }
    }
#endregion

#region OnNetAck
    private void OnNetAckDispatchOpen(int result, PktMsgType pktmsg)
    {
        Renewal_TopPanel();
        
        //Open 연출
        StartCoroutine(CoOpenDirection());
    }

    private void OnNetAckDispatchOper(int result, PktMsgType pktmsg)
    {
        // 보이스 재생
        PktInfoDispatchOperAck pAck = (PktInfoDispatchOperAck)pktmsg;
        List<ulong> cardUID = new List<ulong>();
        foreach(var info in pAck.cards_.infos_)        
            cardUID.Add(info.uid_);
        StartCoroutine(PlayVoice(cardUID, eVOICESUPPORTER.DispatchStart));

        Renewal_TopPanel();
        SelfUpdate();
    }

    private void OnNetAckDispatchOperConfirm(int result, PktMsgType pktmsg)
    {
        PktInfoDispatchOperConfirmAck pAck = (PktInfoDispatchOperConfirmAck)pktmsg;
        if(pAck.operEndFlag_)
        {
            // 보상 아이템 연출
            var oldMission = GameInfo.Instance.GameTable.CardDispatchMissions.Find(x => x.ID == CompleteMissionTID);
            if (oldMission != null)
            {
                List<RewardData> rewards = new List<RewardData>();
                RewardData data = new RewardData(oldMission.RewardType, oldMission.RewardIndex, oldMission.RewardValue);
                rewards.Add(data);
                
                string msg = string.Format(FLocalizeString.Instance.GetText(3207), FLocalizeString.Instance.GetText(oldMission.Name));
                MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText(1577), msg, rewards);

                // 보이스 재생
                List<ulong> cardUID = new List<ulong>();
                foreach (var info in pAck.outCards_.uids_)
                    cardUID.Add(info);
                StartCoroutine(PlayVoice(cardUID, eVOICESUPPORTER.DispatchComplete, 0.5f));
            }
        }
        else
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3208));            
        }


        Renewal_TopPanel();
        SelfUpdate();
    }

    private void OnNetAckDispatchChange(int result, PktMsgType pktmsg)
    {
        // "새로운 파견 임무를 받았습니다."
        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3209));

        Renewal_TopPanel();

        //임시 배정된 서포터 삭제
        // 편성된 서포터 리스트 확인
        List<CardData> supports = GameInfo.Instance.CardList.FindAll(x =>
            x.PosValue == Index + 1 &&
            x.PosKind == (int)eContentsPosKind.DISPATCH);

        foreach(var data in supports)
            data.InitPos();

        var fComponent = LobbyUIManager.Instance.GetUI("CardDispatchPanel");
        fComponent.Renewal();
    }
#endregion

    private void Renewal_TopPanel()
    {
        UITopPanel topPanel = LobbyUIManager.Instance.GetUI<UITopPanel>("TopPanel");
        if (topPanel)
        {
            topPanel.Renewal(true);
        }
    }

    private IEnumerator CoOpenDirection()
    {
        var fComponent = LobbyUIManager.Instance.GetUI("CardDispatchPanel");
        fComponent.Renewal();

        goOpenEffect.SetActive(true);
        twAlphaOpen.PlayForward();
        while (twAlphaOpen.tweenFactor < 1f)
            yield return null;
        goOpenEffect.SetActive(false);
    }

    private IEnumerator PlayVoice(List<ulong> carduids, eVOICESUPPORTER kind, float delay = 0f)
    {
        if (carduids == null || carduids.Count == 0)
            yield break;

        /*
        - 파견 시작, 파견 완료 시 음성 재생
        - SR, UR만 존재.
        - UR, SR 중 랜덤 플레이(R은 리소스도 없음)  	  
        */
        Func<ulong, GameTable.Card.Param> IsVoiceCard = (carduid) =>
        {
            for (int i = 0; i < GameInfo.Instance.CardList.Count; i++)
            {
                if(GameInfo.Instance.CardList[i].CardUID == (long)carduid &&
                   GameInfo.Instance.CardList[i].TableData.Grade >= (int)eGRADE.GRADE_SR)
                {
                    return GameInfo.Instance.CardList[i].TableData;
                }
            }
            return null;
        };

        List<GameTable.Card.Param> list = new List<GameTable.Card.Param>();
        for (int i = 0; i < carduids.Count; i++)
        {
            GameTable.Card.Param p = IsVoiceCard(carduids[i]);
            if (p != null)
                list.Add(p);
        }

        if (list == null || list.Count == 0)
            yield break;

        GameTable.Card.Param playCard = list[UnityEngine.Random.Range(0, list.Count)];
        if (playCard == null) yield break;

        if(delay > 0f)
            yield return new WaitForSeconds(delay);

        VoiceMgr.Instance.PlaySupporter(kind, playCard.ID);
    }

    private bool IsAutoEnable()
    {
        List<CardData> supports = GameInfo.Instance.CardList.FindAll(x =>
           x.PosValue == _dispatchData.TableID &&
           x.PosKind == (int)eContentsPosKind.DISPATCH);

        if (supports != null && supports.Count < OpenUnitCount)
        {
            return true;
        }

        return false;
    }

    private bool IsNeedSupport_UR()
    {
        if (TableMissionParam.NeedURCnt <= 0)
            return false;

        // UR 서포터 체크
        int curURCount = 0;
        for (int i = 0; i < listCardDispatchUnit.Count; i++)
        {
            if (listCardDispatchUnit[i].SupportCardData == null) continue;

            if (listCardDispatchUnit[i].SupportCardData.TableData.Grade == (int)eGRADE.GRADE_UR)
                curURCount++;
        }

        if (TableMissionParam.NeedURCnt > curURCount)
            return true;

        return false;
    }

    private int CompareGradeUp(CardData l, CardData r)
    {
        if (l.TableData.Grade > r.TableData.Grade) return 1;
        else if (l.TableData.Grade < r.TableData.Grade) return -1;

        return 0;
    }

    private int CompareGradeDown(CardData l, CardData r)
    {
        if (l.TableData.Grade > r.TableData.Grade) return -1;
        else if (l.TableData.Grade < r.TableData.Grade) return 1;

        return 0;
    }

    private void SendReqImmediatelyDone() {
        List<ItemData> list = GameInfo.Instance.ItemList.FindAll( x => x.TableID == GameInfo.Instance.GameConfig.CardDispatchFastItemID );
        if( list == null || list.Count <= 0 ) {
            MessagePopup.OK( eTEXTID.OK, 3003, null );
            return;
		}

        CompleteMissionTID = TableMissionParam.ID;
        GameInfo.Instance.Send_ReqDispatchOperConfirm( (uint)_dispatchData.TableID, true, OnNetAckDispatchOperConfirm );
    }
}

