using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEventmodeExchangeStagePanel : FComponent
{

	public FTab kDifficultyTab;
	public UITexture kBGTex;
	public UIButton kEventRuleBtn;
	public UILabel kRuleLabel;
    public UILabel kStoryNameLabel;
    public UILabel kChapterLabel;

    public List<UIStoryUnit> kEventStageUnitList;

    private EventSetData _eventSetData;

    private List<bool> _isdifficultylist = new List<bool>();
    private int _difficulty = 1;

    public override void Awake()
	{
		base.Awake();

		kDifficultyTab.EventCallBack = OnDifficultyTabSelect;
	}
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();

		//kDifficultyTab.SetTab(int, SelectEvent) 	//Set Parameter Value
	}
 
	public override void InitComponent()
	{
        int eventId = (int)UIValue.Instance.GetValue(UIValue.EParamType.EventID);
        _eventSetData = GameInfo.Instance.GetEventSetData(eventId);
        if (_eventSetData == null)
            return;

        FLocalizeString.SetLabel(kStoryNameLabel, _eventSetData.TableData.Name);
        FLocalizeString.SetLabel(kChapterLabel, _eventSetData.TableData.Desc);

        _difficulty = 1;

        BannerData bannerdataBG = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.EVENT_MAINBG && x.BannerTypeValue == _eventSetData.TableID);
        if (bannerdataBG != null)
        {
			if (kBGTex.mainTexture != null)
			{
				DestroyImmediate(kBGTex.mainTexture, false);
				kBGTex.mainTexture = null;
			}

            kBGTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(bannerdataBG.UrlImage, true, bannerdataBG.Localizes[(int)eBannerLocalizeType.Url]);
        }

        var eventstageid = UIValue.Instance.GetValue(UIValue.EParamType.EventStageID);
        if (eventstageid != null)
        {
            var stagedata = GameInfo.Instance.GameTable.FindStage(x => x.ID == (int)eventstageid);
            if (stagedata != null)
            {
                if (stagedata.StageType == (int)eSTAGETYPE.STAGE_EVENT)
                {
                    _difficulty = stagedata.Difficulty;
                }
            }
        }

        int maxDiffculty = _difficulty;
        for (int i = 0; i < kEventStageUnitList.Count; i++)
            maxDiffculty = Mathf.Max(maxDiffculty, GetMaximumDifficulty(kEventStageUnitList[i].kNum, eventId));

        for (int i = 0; i < kDifficultyTab.kBtnList.Count; i++)
        {
            kDifficultyTab.SetEnabled(i, i < maxDiffculty);
        }
        kDifficultyTab.SetTab(_difficulty - 1, SelectEvent.Code);

        Renewal(true);
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        

        if (_eventSetData == null)
            return;

        for(int i = 0; i < kEventStageUnitList.Count; i++)
        {
            SetEventStageUnit(kEventStageUnitList[i]);
        }
	}

    public override void OnUIOpen() {
        LobbyUIManager.Instance.kBlackScene.SetActive( false );
    }

    private bool OnDifficultyTabSelect(int nSelect, SelectEvent type)
	{
        _difficulty = nSelect + 1;

        Renewal(true);

        return true;
	}
	
    private void SetEventStageUnit(UIStoryUnit eventUnit)
    {
        int chapter = eventUnit.kNum;
        GameTable.Stage.Param eventStageTableData = GameInfo.Instance.GameTable.FindStage(x => x.StageType == (int)eSTAGETYPE.STAGE_EVENT &&
        x.TypeValue == _eventSetData.TableID && x.Difficulty == _difficulty && x.Chapter == chapter);

        if(eventStageTableData == null)
        {
            Log.Show("EventStageTable is null", Log.ColorType.Red);
            return;
        }
        eventUnit.UpdateSlot(chapter, eventStageTableData);
    }

	public void OnClick_EventStageUnit0()
	{
	}
	
	public void OnClick_EventStageUnit1()
	{
	}
	
	public void OnClick_EventStageUnit2()
	{
	}

    public void OnClick_EventRuleBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.RulePopupType, (int)UIEventRulePopup.eRulePopupType.EVENT_RULE);
        LobbyUIManager.Instance.ShowUI("EventRulePopup", true);
    }

    private int GetMaximumDifficulty(int chapter, int eventId)
    {
        for (int i = 2; i >= 0; i--)
        {
            GameTable.Stage.Param tabledata = GameInfo.Instance.GameTable.FindStage(x => x.StageType == (int)eSTAGETYPE.STAGE_EVENT && x.TypeValue == eventId && x.Chapter == chapter && x.Difficulty == i + 1);
            if (tabledata == null)
                continue;
            Log.Show(tabledata.ID, Log.ColorType.Red);
            if (tabledata.LimitStage != -1)
            {
                var stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == tabledata.ID);
                if (stagecleardata == null)
                    continue;

                if(tabledata.NextStage != -1)
                {
                    GameTable.Stage.Param nextStage = GameInfo.Instance.GameTable.FindStage(x => x.ID == tabledata.NextStage);
                    if (nextStage != null)
                        return nextStage.Difficulty;
                }
                else
                {
                    return tabledata.Difficulty;
                }
                
            }
            
            return i + 1;
        }

        return 1;
    }
}
