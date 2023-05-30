using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eEventStageType
{
    EVENT,
    ARENA_PROLOGUE,
}

public class BonusStageInfo
{
    public GameTable.Stage.Param    StageData;
    public bool                     IsClear;


    public BonusStageInfo(GameTable.Stage.Param stageData, bool isClear)
    {
        StageData = stageData;
        IsClear = isClear;
    }
}


public class UIEventmodeStoryStagePanel : FComponent
{
    public List<UIStoryUnit> kStoryUnitList;
    public UILabel kStoryNameLabel;
    public UILabel kChapterLabel;
    public FTab kDifficultyTab;
    public UILabel kEventStageDescLb;
    public UILabel kArenaPerDescLb;
    public UILabel kArenaPerGaugeLb;
    public UICampaignMarkUnit kCampaignMarkUnit;

    [Header("[Bonus Stage]")]
    public UIButton     BtnBonusStage;
    public UISprite     SprBonusStageClear;
    public GameObject   BonusStageLine;

    private EventSetData _eventsetdata = null;
    private int _difficulty = 1;// 1부터 시작 스테이지 난이도
    private int _chapter = 1;// 1부터 시작 챕터
    private List<bool> _isdifficultylist = new List<bool>();
    private List<GameTable.Stage.Param> _stagelist = new List<GameTable.Stage.Param>();

    private eEventStageType _popupType;

    // for Bonus Stage
    private List<BonusStageInfo>    mListBonusStageInfo     = new List<BonusStageInfo>();
    private bool                    mbAllEventMissionClear  = false;


    public override void Awake()
    {
        base.Awake();

        kDifficultyTab.EventCallBack = OnDifficultyTabSelect;
    }


    public override void OnEnable()
    {
        InitComponent();

        base.OnEnable();
    }

    public override void InitComponent()
    {
        _difficulty = 1;
        _chapter = 1;
        mbAllEventMissionClear = false;

        //가챠이벤트, 아레나 프롤로그 셋팅 해줌
        var popupType = UIValue.Instance.GetValue(UIValue.EParamType.EventStagePopupType);
        if (popupType == null)
            return;

        _popupType = (eEventStageType)popupType;

        int lastStageID = -1;
        var lastPlayStage = UIValue.Instance.GetValue(UIValue.EParamType.LastPlayStageID);
        if (lastPlayStage != null)
            lastStageID = (int)lastPlayStage;

        switch (_popupType)
        {
            case eEventStageType.EVENT:
                {
                    var eventObj = UIValue.Instance.GetValue(UIValue.EParamType.EventID);
                    if (eventObj == null)
                        return;
                    int eventId = (int)eventObj;
                    _eventsetdata = GameInfo.Instance.GetEventSetData(eventId);
                    if (_eventsetdata == null)
                        return;

                    var eventstageid = UIValue.Instance.GetValue(UIValue.EParamType.EventStageID);
                    if (eventstageid != null)
                    {
                        var stagedata = GameInfo.Instance.GameTable.FindStage(x => x.ID == (int)eventstageid);
                        if (stagedata != null)
                        {
                            if (stagedata.TypeValue == eventId)
                            {
                                if (stagedata.StageType == (int)eSTAGETYPE.STAGE_EVENT)
                                {
                                    _difficulty = stagedata.Difficulty;
                                    _chapter = stagedata.Chapter;
                                }
                                else if(stagedata.StageType == (int)eSTAGETYPE.STAGE_EVENT_BONUS)
                                {
                                    _difficulty = 3;
                                }
                            }
                        }
                    }

                    _isdifficultylist.Clear();
                    GameSupport.GetIsChapterDifficultylist(_chapter, ref _isdifficultylist, eSTAGETYPE.STAGE_EVENT, eventId);

                    GetBonusStageInfo();
                }
                break;
            case eEventStageType.ARENA_PROLOGUE:
                {
                    int prologueStageid = 1;
                    if(lastStageID != -1)
                    {
                        GameTable.Stage.Param laststagedata = GameInfo.Instance.GameTable.FindStage(x => x.ID == lastStageID);
                        if(laststagedata != null)
                        {
                            if (laststagedata.StageType == (int)eSTAGETYPE.STAGE_PVP_PROLOGUE)
                            {
                                if (laststagedata.NextStage == -1)
                                    prologueStageid = lastStageID;
                                else
                                    prologueStageid = laststagedata.NextStage;
                            }
                        }
                        else
                        {
                            prologueStageid = (int)UIValue.Instance.GetValue(UIValue.EParamType.ArenaPrologueStageID);
                        }
                    }
                    else
                    {
                        prologueStageid = (int)UIValue.Instance.GetValue(UIValue.EParamType.ArenaPrologueStageID);
                    }

                    var stagedata = GameInfo.Instance.GameTable.FindStage(x => x.ID == prologueStageid);
                    if (stagedata != null)
                    {
                        if (stagedata.StageType == (int)eSTAGETYPE.STAGE_PVP_PROLOGUE)
                        {
                            _difficulty = stagedata.Difficulty;
                            _chapter = stagedata.Chapter;
                        }
                    }

                    if (prologueStageid != null)
                    {
                        
                    }

                    _isdifficultylist.Clear();
                    GameSupport.GetIsChapterDifficultylist(_chapter, ref _isdifficultylist, eSTAGETYPE.STAGE_PVP_PROLOGUE);
                }
                break;
        }
        
        for (int i = 0; i < kDifficultyTab.kBtnList.Count; i++)
            kDifficultyTab.SetEnabled(i, _isdifficultylist[i]);
        kDifficultyTab.SetTab(_difficulty - 1, SelectEvent.Code);
        
        Renewal(true);
    }

    public override void OnUIOpen()
    {
        LobbyUIManager.Instance.kBlackScene.SetActive(false);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
       
        kEventStageDescLb.gameObject.SetActive(false);
        kArenaPerDescLb.gameObject.SetActive(false);
        kArenaPerGaugeLb.gameObject.SetActive(false);
        kStoryNameLabel.gameObject.SetActive(false);
        kChapterLabel.gameObject.SetActive(false);

        BtnBonusStage.gameObject.SetActive(false);
        BonusStageLine.gameObject.SetActive(false);

        switch (_popupType)
        {
            case eEventStageType.EVENT:
                {
                    NotificationManager.Instance.CheckCampaignMark(kCampaignMarkUnit, (int)eSTAGETYPE.STAGE_EVENT);

                    if (_eventsetdata == null)
                        return;

                    kEventStageDescLb.gameObject.SetActive(true);
                    kStoryNameLabel.gameObject.SetActive(true);
                    kChapterLabel.gameObject.SetActive(true);

                    FLocalizeString.SetLabel(kStoryNameLabel, _eventsetdata.TableData.Name);
                    FLocalizeString.SetLabel(kChapterLabel, _eventsetdata.TableData.Desc);

                    for (int i = 0; i < kStoryUnitList.Count; i++)
                    {
                        UIStoryUnit unit = kStoryUnitList[i];
                        unit.UpdateSlot(i, _stagelist[i]);
                    }

                    LobbyUIManager.Instance.BG_Event(kBGIndex, _eventsetdata.TableID, 1);

                    if(_difficulty >= 3 && mbAllEventMissionClear)
                    {
                        BtnBonusStage.SetActive(true);
                        BonusStageLine.gameObject.SetActive(true);
                    }
                }
                break;

            case eEventStageType.ARENA_PROLOGUE:
                {
                    NotificationManager.Instance.CheckCampaignMark(kCampaignMarkUnit, (int)eSTAGETYPE.STAGE_PVP_PROLOGUE);

                    kArenaPerGaugeLb.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (GameInfo.Instance.UserData.ArenaPrologueValue));

                    if (GameInfo.Instance.UserData.ArenaPrologueValue < 100)
                    {
                        kArenaPerDescLb.gameObject.SetActive(true);
                        kArenaPerGaugeLb.gameObject.SetActive(true);
                    }

                    for (int i = 0; i < kStoryUnitList.Count; i++)
                        kStoryUnitList[i].UpdateSlot(i, _stagelist[i]);

                    LobbyUIManager.Instance.BG_Arena(kBGIndex, "Story/BG/TA0_NC016b.png");
                }
                break;
        }
    }

    public void OnBtnBonusStage()
    {
        if(mListBonusStageInfo.Count <= 0)
        {
            return;
        }

        BonusStageInfo find = mListBonusStageInfo.Find(x => !x.IsClear);
        if(find == null) // 보너스 스테이지 다 깼으면 못들어감
        {
            return;
        }

        UIStageDetailPopup stageDetailPopup = LobbyUIManager.Instance.GetUI<UIStageDetailPopup>("StageDetailPopup");
        stageDetailPopup.SetEventBonusStageInfo(mListBonusStageInfo);

        UIValue.Instance.SetValue(UIValue.EParamType.StageID, (int)mListBonusStageInfo[0].StageData.ID);
        LobbyUIManager.Instance.ShowUI("StageDetailPopup", true);
    }

    private bool OnDifficultyTabSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
            return true;

        _difficulty = nSelect + 1;
        Log.Show("_difficulty : " + _difficulty, Log.ColorType.Red);
        _stagelist.Clear();
        if(_popupType == eEventStageType.EVENT)
            _stagelist = GameInfo.Instance.GameTable.FindAllStage(x => x.StageType == (int)eSTAGETYPE.STAGE_EVENT && x.Difficulty == _difficulty && x.TypeValue == _eventsetdata.TableID);
        else if(_popupType == eEventStageType.ARENA_PROLOGUE)
            _stagelist = GameInfo.Instance.GameTable.FindAllStage(x => x.StageType == (int)eSTAGETYPE.STAGE_PVP_PROLOGUE && x.Difficulty == _difficulty);

        Renewal(true);
        return true;
    }

    private void GetBonusStageInfo()
    {
        mListBonusStageInfo.Clear();

        List<GameTable.Stage.Param> findAll = GameInfo.Instance.GameTable.FindAllStage(x => x.StageType == (int)eSTAGETYPE.STAGE_EVENT_BONUS &&
                                                                                            x.TypeValue == _eventsetdata.TableID);

        if (findAll == null || findAll.Count <= 0)
        {
            return;
        }

        bool isAllBonusStageClear = true;
        for(int i = 0; i < findAll.Count; i++)
        {
            GameTable.Stage.Param stageData = findAll[i];
            StageClearData stageClearData = GameInfo.Instance.StageClearList.Find(x => x.TableID == stageData.ID);

            BonusStageInfo info = new BonusStageInfo(findAll[i], stageClearData != null);
            if(!info.IsClear)
            {
                isAllBonusStageClear = false;
            }

            mListBonusStageInfo.Add(info);
        }

        SprBonusStageClear.gameObject.SetActive(isAllBonusStageClear);
        mbAllEventMissionClear = GameSupport.IsAllEventMissionClear(_eventsetdata.TableID);
    }
}
