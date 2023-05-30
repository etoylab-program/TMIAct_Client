using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStoryPanel : FComponent
{
    public List<UIStoryUnit> kStoryUnitList;
    public UILabel kStoryNameLabel;
    public UILabel kChapterLabel;
    public FTab kDifficultyTab;
    [SerializeField] private FList _ChapterTglBtnListInstance;

    private int _chapter = 1;   // 0���� ���� é�͸���Ʈ�� IDX(é��ID �ƴ�)
    private int _difficulty = 1;// 1���� ���� �������� ���̵�

    private List<GameTable.Stage.Param> _stagelist = new List<GameTable.Stage.Param>();
    private List<bool> _isdifficultylist = new List<bool>();

    public override void Awake()
    {
        base.Awake();

        _ChapterTglBtnListInstance.InitBottomFixing();
        _ChapterTglBtnListInstance.EventUpdate = _UpdateChapterTglBtnListSlot;
        _ChapterTglBtnListInstance.EventGetItemCount = _GetChapterTglBtnElementCount;
        

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

        var storystageid = UIValue.Instance.GetValue(UIValue.EParamType.StoryStageID);
        if (storystageid != null)
        {
            var stagedata = GameInfo.Instance.GameTable.FindStage(x => x.ID == (int)storystageid);
            if (stagedata != null)
            {
                _chapter = stagedata.Chapter;
                _difficulty = stagedata.Difficulty;
            }
        }

       _isdifficultylist.Clear();
        GameSupport.GetIsChapterDifficultylist(_chapter, ref _isdifficultylist);
        for (int i = 0; i < kDifficultyTab.kBtnList.Count; i++)
        {
            kDifficultyTab.SetEnabled(i, _isdifficultylist[i]);

            //�������������� ���̵��� �������� Ȯ��
            if (_isdifficultylist[i])
                _difficulty = i + 1;
        }

        kDifficultyTab.SetTab(_difficulty-1, SelectEvent.Code);

        _ChapterTglBtnListInstance.UpdateList();
        //_ChapterTglBtnListInstance.ScrollPositionSet();

        if (_ChapterTglBtnListInstance.IsScroll && _chapter > 4)
        {
            _ChapterTglBtnListInstance.SpringSetFocus(_chapter - 1, 1);
        }
    }

    public override void OnUIOpen()
    {
        LobbyUIManager.Instance.kBlackScene.SetActive(false);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        //Text����
        FLocalizeString.SetLabel(kChapterLabel, string.Format("{0} {1}", FLocalizeString.Instance.GetText(1300), _chapter.ToString("D2")));
        var chapterdata = GameInfo.Instance.GameClientTable.FindChapter(x => x.ID == _chapter);
        if(chapterdata != null)
            FLocalizeString.SetLabel(kStoryNameLabel, chapterdata.Name);

        //é�� ��ư ���°���
        for (int i = 0; i < kStoryUnitList.Count; i++)
            kStoryUnitList[i].UpdateSlot(i, _stagelist[i]);
        

        //  ������ �������� ǥ�ý� ����Ʈ Ȱ��ȭ
        //kStoryUnit_EFF.gameObject.SetActive(kStoryUnitList[kStoryUnitList.Count - 1].IsActive);

        LobbyUIManager.Instance.BG_Stage(kBGIndex, _chapter, _difficulty);

        _ChapterTglBtnListInstance.RefreshNotMove();

        

    }


    private void _UpdateChapterTglBtnListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIChapterTglBtnSlot slot = slotObject.GetComponent<UIChapterTglBtnSlot>();
            if (slot == null)
                break;
            slot.ParentGO = gameObject;
            int chapter = GameInfo.Instance.GameClientTable.Chapters[index].ID;
            if (0 <= index)
                slot.UpdateSlot(UIChapterTglBtnSlot.ePos.Story, index, chapter, _chapter == chapter, GameSupport.IsMainStoryChapter(chapter, _difficulty));
        } while (false);
    }

    private int _GetChapterTglBtnElementCount()
    {
        return GameInfo.Instance.GameClientTable.Chapters.Count;
    }

    private bool OnDifficultyTabSelect(int nSelect, SelectEvent type)
    {
        Log.Show(nSelect + " / " + type);
        _difficulty = nSelect + 1;

        _stagelist.Clear();
        _stagelist = GameInfo.Instance.GameTable.FindAllStage(x => x.Difficulty == _difficulty && x.Chapter == _chapter && x.StageType == (int)eSTAGETYPE.STAGE_MAIN_STORY);
        
        Renewal(true);

        return true;
    }

    public void OnChapterSelect(int selectChapter)
    {
        //  é�� ID�� �ƴ� é�� ����Ʈ �ε���
        _chapter = selectChapter;

        //�̵��� é�Ϳ� ���̵��� ������ �ʾҴٸ� �ִ� ���̵��� ����
        _isdifficultylist.Clear();
        GameSupport.GetIsChapterDifficultylist(_chapter, ref _isdifficultylist);
        if( !_isdifficultylist[_difficulty - 1])
        {
            int temp = 0;
            for( int i = 0; i < _isdifficultylist.Count; i++ )
            {
                if (_isdifficultylist[i])
                    temp = i;
            }
            _difficulty = temp + 1;
        }

        for (int i = 0; i < kDifficultyTab.kBtnList.Count; i++)
            kDifficultyTab.SetEnabled(i, _isdifficultylist[i]);
        kDifficultyTab.SetTab(_difficulty - 1, SelectEvent.Code);

        //Log.Show(_chapter - 1);
        //_ChapterTglBtnListInstance.SetFocus(_chapter -1, false);

        //if (_ChapterTglBtnListInstance.IsScroll)
        //{
        //    Log.Show(_ChapterTglBtnListInstance.GetRealIndex + " / " + _ChapterTglBtnListInstance.GetTotalItemCount + " / " + _ChapterTglBtnListInstance.GetMaxItemCount + " / " + _chapter, Log.ColorType.Red);
        //    if (_ChapterTglBtnListInstance.GetMaxItemCount < _chapter)
        //    {
        //        _ChapterTglBtnListInstance.SetFocus(_chapter - _ChapterTglBtnListInstance.GetMaxItemCount, false);
        //    }
        //    else
        //    {
        //        _ChapterTglBtnListInstance.SetFocus(0, false);
        //    }


        //    //if (_chapter - 2 > 0)
        //    //    _ChapterTglBtnListInstance.SetFocus(_chapter - 2, false);
        //    //else
        //    //    _ChapterTglBtnListInstance.SetFocus(0, false);
        //}
        
        Renewal(true);
    }
}
