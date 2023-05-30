using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIDailyPanel : FComponent
{

	public GameObject kCloseObj;
	public UILabel kDayLabel;
	public UILabel kRemainTimeLabel;
    public List<UIDailyUnit> kDailyList;
    public FTab kDifficultyTab;
    public List<GameObject> kSecretQuestLockList;
    public UIButton kSecretQuestBtn;

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
	}
 
	public override void InitComponent()
	{
        for( int i = 0; i < kDailyList.Count; i++ )
            _difficulty = Mathf.Max(_difficulty, GetMaximumDifficulty(kDailyList[i].kChapter));

        kDifficultyTab.SetTab(_difficulty-1, SelectEvent.Code);

        GameSupport.ShowTutorialFlag(eTutorialFlag.DAILY);

        bool bEnableSecretQuestBtn = true;
        for (int i = 0; i < GameInfo.Instance.GameConfig.SecretQuestOpenKind.Count; i++)
        {
            bool bClear = GameInfo.Instance.StageClearList.Any(x => x.TableID == GameInfo.Instance.GameConfig.SecretQuestOpenKind[i]);
            if (i < kSecretQuestLockList.Count)
            {
                kSecretQuestLockList[i].SetActive(!bClear);
            }

            if (!bClear && bEnableSecretQuestBtn)
            {
                bEnableSecretQuestBtn = false;
            }
        }
        kSecretQuestBtn.isEnabled = bEnableSecretQuestBtn;
        kCloseObj.SetActive(!bEnableSecretQuestBtn);
    }

    public override void OnUIOpen()
    {
        LobbyUIManager.Instance.kBlackScene.SetActive(false);
    }
    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        for( int i = 0; i < kDailyList.Count; i++ )
        {
            kDailyList[i].UpdateSlot(_difficulty);
        }

        kDayLabel.textlocalize = FLocalizeString.Instance.GetText(200 + (int)GameSupport.GetCurrentServerTime().DayOfWeek);
        
        string strRemainTime = GameSupport.GetRemainTimeString(GameInfo.Instance.ServerData.DayRemainTime, System.DateTime.UtcNow.AddSeconds(GameInfo.Instance.ServerData.ServerTimeGap));
        if (strRemainTime.Equals("-"))
        {
            System.DateTime tomorrowTime = GameInfo.Instance.UserData.LoginBonusRecentDate.AddDays(1);
            GameInfo.Instance.ServerData.DayRemainTime = new System.DateTime(tomorrowTime.Year, tomorrowTime.Month, tomorrowTime.Day);
            strRemainTime = GameSupport.GetRemainTimeString(GameInfo.Instance.ServerData.DayRemainTime, System.DateTime.UtcNow.AddSeconds(GameInfo.Instance.ServerData.ServerTimeGap));
        }
        
        kRemainTimeLabel.textlocalize = strRemainTime;
    }

    private bool OnDifficultyTabSelect(int nSelect, SelectEvent type)
    {
        _difficulty = nSelect + 1;
        
        Renewal(true);

        return true;
    }


    int GetMaximumDifficulty( int chapter )
    {
        for (int i = 2; i >= 0; i--)
        {
            var tabledata = GameInfo.Instance.GameTable.FindStage(x => x.StageType == (int)eSTAGETYPE.STAGE_DAILY && x.Chapter == chapter && x.Difficulty == i + 1);
            if (tabledata == null)
                continue;

            if (tabledata.LimitStage != -1)
            {
                var stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == tabledata.ID);
                if (stagecleardata == null)
                    continue;
            }

            return i + 1;
        }

        return 1;
    }

    public void OnClick_SecretQuestBtn()
    {
        UIStageDetailPopup popup = LobbyUIManager.Instance.GetUI<UIStageDetailPopup>("StageDetailPopup");
        if (popup != null)
        {
            popup.SetSecretStage();
            popup.SetUIActive(true);
        }
    }

    public void OnClick_LockBtn(int index)
    {
        if (GameInfo.Instance.GameConfig.SecretQuestOpenKind.Count <= index)
        {
            return;
        }
        
        GameTable.Stage.Param stageParam = GameInfo.Instance.GameTable.FindStage(GameInfo.Instance.GameConfig.SecretQuestOpenKind[index]);
        if (stageParam == null)
        {
            return;
        }

        MessageToastPopup.Show(FLocalizeString.Instance.GetText(1820, FLocalizeString.Instance.GetText(stageParam.Name)));
    }
}
