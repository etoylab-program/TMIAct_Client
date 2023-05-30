using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITimeAttackPanel : FComponent
{
    public UILabel kChapterNumber;
	public UILabel kStoryNameLabel;
	public FTab kDifficultyTab;
	public GameObject koff;
	public GameObject kon;
    public UIUserCharListSlot kUserCharListSlot;
    public UILabel kHighestScoreLabel;
	public UILabel kDateRemainLabel;
	public UIButton kEnterBtn;
    public UILabel kTimeAttackRemainTimeDescLb;
    [SerializeField] private FList _ChapterListInstance;
    [SerializeField] private FList _RankingListInstance;

    private int _chapter = 1;   // 0부터 시작 챕터리스트의 IDX(챕터ID 아님)
    private int _difficulty = 1;// 1부터 시작 스테이지 난이도
    private int _stageid = -1;
    private List<bool> _isdifficultylist = new List<bool>();

    private GameTable.Stage.Param _stagetabledata;
    private TimeAttackClearData _timeattackcleardata;
    private TimeAttackRankData _timeattackrankdata;

    public override void Awake()
	{
		base.Awake();

		kDifficultyTab.EventCallBack = OnDifficultyTabSelect;
		if(this._ChapterListInstance == null) return;
		
		this._ChapterListInstance.EventUpdate = this._UpdateChapterListSlot;
		this._ChapterListInstance.EventGetItemCount = this._GetChapterElementCount;
        this._ChapterListInstance.InitBottomFixing();

        if (this._RankingListInstance == null) return;
		
		this._RankingListInstance.EventUpdate = this._UpdateRankingListSlot;
		this._RankingListInstance.EventGetItemCount = this._GetRankingElementCount;
        this._RankingListInstance.InitBottomFixing();
    }
 
	public override void OnEnable()
	{
        InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        _chapter = 1;
        _difficulty = 1;

        var timeStageID =  UIValue.Instance.GetValue(UIValue.EParamType.StageID);
        if (timeStageID != null)
        {
            GameTable.Stage.Param timeStageData = GameInfo.Instance.GameTable.FindStage(x => x.ID == (int)timeStageID && x.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK);
            if (timeStageData != null)
            {
                _chapter = timeStageData.Chapter;
                _difficulty = timeStageData.Difficulty;
            }
        }

        OnChapterSelect(_chapter);

        this._ChapterListInstance.UpdateList();
        this._RankingListInstance.UpdateList();

        if (_ChapterListInstance.IsScroll && _chapter > 1)
        {
            _ChapterListInstance.SpringSetFocus(_chapter - 1, 1);
        }

        GameSupport.ShowTutorialFlag(eTutorialFlag.TIMEATTACK);
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        kChapterNumber.textlocalize = string.Format("{0} {1}", FLocalizeString.Instance.GetText(1300), _chapter.ToString("D2"));
        kStoryNameLabel.textlocalize = "";
        if (_stagetabledata != null)
            kStoryNameLabel.textlocalize = FLocalizeString.Instance.GetText(_stagetabledata.Name);

        if ( _timeattackcleardata != null )
        {
            if (_timeattackcleardata.HighestScoreRemainTime < GameSupport.GetCurrentServerTime())
            {
                kon.SetActive(false);
                koff.SetActive(true);
            }
            else
            {
                kon.SetActive(true);
                koff.SetActive(false);

                FLocalizeString.SetLabel(kTimeAttackRemainTimeDescLb, string.Format(FLocalizeString.Instance.GetText(1044), GameInfo.Instance.GameConfig.TimeAttackModeRecordDay));
                kUserCharListSlot.UpdateSlot(_timeattackcleardata);
                kHighestScoreLabel.textlocalize = GameSupport.GetTimeHighestScore(_timeattackcleardata.HighestScore);

                System.DateTime subDate = _timeattackcleardata.HighestScoreRemainTime.AddDays(-GameInfo.Instance.GameConfig.TimeAttackModeRecordDay);
                kDateRemainLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1058), subDate.ToString());
            }
        }
        else
        {
            kon.SetActive(false);
            koff.SetActive(true);
        }       

        this._ChapterListInstance.RefreshNotMove();
        //this._ChapterListInstance.ScrollPositionSet();

        this._RankingListInstance.RefreshNotMove();

        LobbyUIManager.Instance.BG_Stage(kBGIndex, _chapter, _difficulty);
    }
    
    public override void OnUIOpen()
    {
        LobbyUIManager.Instance.kBlackScene.SetActive(false);
    }
    
    private bool OnDifficultyTabSelect(int nSelect, SelectEvent type)
	{
        _difficulty = nSelect + 1;

        SetStageData();

        Renewal(true);
        return true;
	}
	
	private void _UpdateChapterListSlot(int index, GameObject slotObject)
	{
        do
        {
            UIChapterTglBtnSlot slot = slotObject.GetComponent<UIChapterTglBtnSlot>();
            if (slot == null)
                break;
            slot.ParentGO = gameObject;
            int chapter = GameInfo.Instance.GameClientTable.Chapters[index].ID;
            if (0 <= index)
                slot.UpdateSlot(UIChapterTglBtnSlot.ePos.TimeAttack, index, chapter, _chapter == chapter, GameSupport.IsMainStoryChapter(chapter, _difficulty, eSTAGETYPE.STAGE_TIMEATTACK));
        } while (false);
    }
	
	private int _GetChapterElementCount()
	{
        return GameInfo.Instance.GameClientTable.Chapters.Count;
    }
	
	private void _UpdateRankingListSlot(int index, GameObject slotObject)
	{
		do
		{
            if (_timeattackrankdata != null)
            {
                UIRankingListSlot slot = slotObject.GetComponent<UIRankingListSlot>();
                if (slot == null)
                    break;

                TimeAttackRankUserData data = null;
                if (0 <= index && _timeattackrankdata.RankUserList.Count > index)
                {
                    data = _timeattackrankdata.RankUserList[index];
                }

                slot.ParentGO = gameObject;
                slot.UpdateSlot(index, data);
            }
        } while(false);
	}
	
	private int _GetRankingElementCount()
	{
        if(_timeattackrankdata != null)
		    return _timeattackrankdata.RankUserList.Count; 
        else
            return 0; 
    }

    public void OnChapterSelect(int selectChapter)
    {
        //  챕터 ID가 아닌 챕터 리스트 인덱스
        _chapter = selectChapter;

        //이동할 챕터에 난이도가 열리지 않았다면 최대 난이도로 변경
        _isdifficultylist.Clear();
        GameSupport.GetIsChapterDifficultylist(_chapter, ref _isdifficultylist, eSTAGETYPE.STAGE_TIMEATTACK);
        if (!_isdifficultylist[_difficulty - 1])
        {
            int temp = 0;
            for (int i = 0; i < _isdifficultylist.Count; i++)
            {
                if (_isdifficultylist[i])
                    temp = i;
            }
            _difficulty = temp + 1;
        }

        for (int i = 0; i < kDifficultyTab.kBtnList.Count; i++)
            kDifficultyTab.SetEnabled(i, _isdifficultylist[i]);
        kDifficultyTab.SetTab(_difficulty - 1, SelectEvent.Code);

        SetStageData();

        Renewal(true);
    }

    private void SetStageData()
    {
        _timeattackcleardata = null;
        _timeattackrankdata = null;
        _stagetabledata = GameInfo.Instance.GameTable.FindStage(x => x.Difficulty == _difficulty && x.Chapter == _chapter && x.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK);
        if (_stagetabledata != null)
        {
            _stageid = _stagetabledata.ID;
            _timeattackcleardata = GameInfo.Instance.GetTimeAttackClearData(_stagetabledata.ID);

            _timeattackrankdata = GameInfo.Instance.TimeAttackRankList.Find(x => x.TableData.Chapter == _stagetabledata.Chapter);
            
            //_timeattackrankdata = GameInfo.Instance.TimeAttackRankList.Find(x => x.TableData.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK && x.TableData.Chapter == _chapter && x.TableData.TypeValue == 1);
        }

        this._RankingListInstance.UpdateList();
    }

    public void OnClick_EnterBtn()
	{
        if (_stagetabledata == null)
            return;

        UIValue.Instance.SetValue(UIValue.EParamType.StageID, _stagetabledata.ID);
        LobbyUIManager.Instance.ShowUI("StageDetailPopup", true);
    }

    public void ClickRankUser(int index)
    {
        if (_timeattackrankdata == null)
            return;

        if (0 <= index && _timeattackrankdata.RankUserList.Count > index)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.RankUserType, (int)eRankUserType.TIMEATTACK);
            UIValue.Instance.SetValue(UIValue.EParamType.TimeAttackRankStageID, _timeattackrankdata.TableID);
            UIValue.Instance.SetValue(UIValue.EParamType.TimeAttackRankUUID, _timeattackrankdata.RankUserList[index].UUID);

            if (_timeattackrankdata.RankUserList[index].bDetail)
                LobbyUIManager.Instance.ShowUI("UserCharDetailPopup", true);
            else
                GameInfo.Instance.Send_ReqTimeAtkRankerDetail(_timeattackrankdata.TableID, _timeattackrankdata.RankUserList[index].UUID, OnNetTimeAtkRankerDetail);
        }
    }

    public void OnNetTimeAtkRankerDetail(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
        LobbyUIManager.Instance.ShowUI("UserCharDetailPopup", true);
    }

}
