using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIDailyUnit : FUnit
{
    public int kChapter;
    public UISprite kBGSpr;
    public UISprite kIconSpr;
    public UILabel kNameLabel;
    public List<UILabel> kDayLabelList;
    public GameObject kMark;
    public List<UISprite> kStarBGList;
    public List<UISprite> kStarList;

    private GameTable.Stage.Param _stagetabledata;
    private bool _bmark;
    private int _difficulty = 1;
    public void UpdateSlot( int difficulty )    //Fill parameter if you need
    {
        _difficulty = difficulty;

        _stagetabledata = GameInfo.Instance.GameTable.FindStage(x => x.StageType == (int)eSTAGETYPE.STAGE_DAILY &&x.Chapter == kChapter && x.Difficulty == _difficulty);
        _bmark = false;
        kMark.gameObject.SetActive(false);
        //kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_stagetabledata.Name);


        SetDayString(0, 1);
        SetDayString(1, 2);
        SetDayString(2, 3);
        SetDayString(3, 4);
        SetDayString(4, 5);
        SetDayString(5, 6);
        SetDayString(6, 0);

        if(_bmark)
        {
            kMark.gameObject.SetActive(true);
        }
        else
        {
            kMark.gameObject.SetActive(false);
        }

        kStarBGList[0].gameObject.SetActive(_stagetabledata.Mission_00 == -1 ? false : true);
        kStarBGList[1].gameObject.SetActive(_stagetabledata.Mission_01 == -1 ? false : true);
        kStarBGList[2].gameObject.SetActive(_stagetabledata.Mission_02 == -1 ? false : true);
        //미션 클리어 오브젝트 일단 모두 끈다..
        for (int i = 0; i < kStarList.Count; i++)
            kStarList[i].gameObject.SetActive(false);

        kBGSpr.spriteName = "btn_daily2";
        //btn_daily_off
        var clearStage = GameInfo.Instance.StageClearList.Find(x => x.TableID == _stagetabledata.ID);
        if (clearStage == null)
        {
            if (_stagetabledata.LimitStage != -1)
                kBGSpr.spriteName = "btn_daily2_off";
        }
        else
        {
            for (int i = 0; i < clearStage.GetClearCount(); i++)
                kStarList[i].gameObject.SetActive(true);
        }
    }

    private void SetDayString(int index, int day)
    {
        //int dow = (int)GameInfo.Instance.ServerData.LoginTime.DayOfWeek;
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

    public void OnClick_Slot()
    {
        var tabledata = GameInfo.Instance.GameTable.FindStage(x => x.StageType == (int)eSTAGETYPE.STAGE_DAILY && x.Chapter == kChapter && x.Difficulty == _difficulty );
        if (tabledata == null)
            return;

        if( !AppMgr.Instance.Review )
        {
            if (tabledata.LimitStage != -1)
            {
                var stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == tabledata.LimitStage);
                if (stagecleardata == null)
                {
                    MessageToastPopup.Show(FLocalizeString.Instance.GetText(3085));
                    return;
                }
            }
        }
        


        UIValue.Instance.SetValue(UIValue.EParamType.StageID, tabledata.ID);
        LobbyUIManager.Instance.ShowUI("StageDetailPopup", true);

        
    }

}

