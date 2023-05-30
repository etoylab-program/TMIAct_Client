using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGamePausePopup : FComponent
{
    public UILabel lbChapter;
    public UILabel lbTitle;


    public override void OnEnable()
    {
        World.Instance.Pause(true);
        base.OnEnable();

        AppMgr.Instance.CustomInput.ShowCursor(true);
    }

    private void SetText()
    {
        if (World.Instance.StageData != null)
        {
            //lbChapter.textlocalize = string.Format(FLocalizeString.Instance.GetText(1226), World.Instance.stageData.Chapter, World.Instance.stageData.Section);
            lbChapter.textlocalize = FLocalizeString.Instance.GetText(World.Instance.StageData.Desc);
            lbTitle.textlocalize = FLocalizeString.Instance.GetText(World.Instance.StageData.Name);
        }
        else
        {
            lbChapter.textlocalize = "";
            lbTitle.textlocalize = "Test Scene";
        }
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();
        AppMgr.Instance.CustomInput.ShowCursor(false);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
        SetText();
    }

    public void OnBtnHelp()
    {
        GameUIManager.Instance.ShowUI("GamePlayHelpPopup", true);
    }

    public override void OnClickClose()
    {
        OnClick_Close();
    }

    public void OnClick_ToLobby()
    {
        if (GameSupport.IsInGameTutorial())
        {
            MessagePopup.OK(eTEXTID.OK, 3127, OnClick_Close, false);
        }
        else
        {
            MessagePopup.OKCANCEL(eTEXTID.OK, 3123, GoToLobby, null, false);
        }
    }

    void GoToLobby()
    {
        FSaveData.Instance.RemoveStageData();
        World.Instance.Pause(true, false);

        GameUIManager.Instance.HideUI("GameFailPanel", false);
        GameUIManager.Instance.HideUI("GamePlayPanel", false);

        SetUIActive(false, false);

        UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.StageToLobby);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, -1);
        GameUIManager.Instance.ShowUI("LoadingPopup", false);
        AppMgr.Instance.LoadScene(AppMgr.eSceneType.Lobby, "Lobby");
    }

    public void OnClick_Close()
    {
        World.Instance.Pause(false);
        SetUIActive(false, false);
    }

    public void OnClick_HelpBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.ShowHelpPopupInPausePopup, true);
        GameUIManager.Instance.ShowUI("GamePlayHelpPopup", false);
    }

    /*
    private void Update()
    {
        if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Pause))
        {
            OnClick_Close();
        }
    }
    */
}
