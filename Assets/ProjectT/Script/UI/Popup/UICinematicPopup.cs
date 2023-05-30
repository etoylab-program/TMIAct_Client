
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UICinematicPopup : FComponent
{
    public enum EAnchorBossName
    {
        TOP_LEFT = 0,
        TOP_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT,
    }


    [Header("[Property]")]
    public UIBossName[] BossNames;
    public UIButton     BtnSkip;
    public UIButton     BtnJumpSection;
    public UILabel      LbDialog;
    public Animation    CountdownAni;
    public GameObject   PauseObj;

    public Dictionary<string, GameObject> UIComponentDic = new Dictionary<string, GameObject>();

    private Director    mCurrentDirector    = null;
    private float       mJumpSectionTime    = 0.0f;
    

    public override void Awake()
    {
        CinematicUI[] list = transform.GetComponentsInChildren<CinematicUI>(true);
        foreach(CinematicUI item in list)
        {
            UIComponentDic.Add(item.name, item.gameObject);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        HideAll();
    }
    
    public void ShowBossName(Director director, EAnchorBossName anchor, GameClientTable.Monster.Param tableData)
    {
        mCurrentDirector = director;
        HideBossName();

		string[] split = Utility.Split(FLocalizeString.Instance.GetText(tableData.Name), '・'); //FLocalizeString.Instance.GetText(tableData.Name).Split('・');

		int index = (int)anchor;

        if (split.Length > 1)
        {
            BossNames[index].Show(split[0], split[1]);
        }
        else
        {
            BossNames[index].Show("", split[0]);
        }

        if (UIComponentDic.Count > 0)
        {
            foreach(KeyValuePair<string, GameObject> pair in UIComponentDic)
            {
                pair.Value.SetActive(false);
            }
        }

    }

    public void HideBossName()
    {
        for (int i = 0; i < BossNames.Length; i++)
        {
            BossNames[i].gameObject.SetActive(false);
        }
    }

	public void ShowSkipButton( Director director ) {
		if ( BtnSkip.gameObject.activeSelf ) {
			return;
		}

		mCurrentDirector = director;
		BtnSkip.gameObject.SetActive( true );

		AppMgr.Instance.CustomInput.ShowCursor( true );
	}

	public void HideSkipButton() {
		if ( AppMgr.Instance.SceneType == AppMgr.eSceneType.Title || BtnSkip == null || !BtnSkip.gameObject.activeSelf ) {
			return;
		}

		BtnSkip.gameObject.SetActive( false );
	}

	public void ShowJumpSection(Director director, float time)
    {
        mCurrentDirector = director;
        mJumpSectionTime = time;

        BtnJumpSection.gameObject.SetActive(true);
    }

    public void HideJumpSection()
    {
        if (BtnJumpSection == null)
        {
            return;
        }

        BtnJumpSection.gameObject.SetActive(false);
    }

    public void ShowDialog(Director director, int groupId, int num)
    {
        mCurrentDirector = director;
        LbDialog.gameObject.SetActive(true);

        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
        {
            LbDialog.textlocalize = FLocalizeString.Instance.GetText(950 + num);
        }
        else
        {
            LbDialog.textlocalize = ScenarioMgr.Instance.GetText(groupId, num);
        }
    }

    public void HideDialog()
    {
        LbDialog.gameObject.SetActive(false);
    }

    public void ShowCinematicUI(string uiName)
    {
        if (UIComponentDic.ContainsKey(uiName))
        {
            UIComponentDic[uiName].SetActive(true);
        }
        else
        {
            FComponent component = GameUIManager.Instance.GetUI("FirstCharSelectPopup");
            if (component == null)
            {
                Debug.LogError("UICinematicPopup에 " + uiName + " 가 존재하지 않습니다.");
                return;
            }
            else
            {
                component.gameObject.SetActive(true);
            }
        }
    }

    public void HideCinematicUI(string uiName)
    {
        if (UIComponentDic.ContainsKey(uiName))
        {
            UIComponentDic[uiName].SetActive(false);
        }
        else
        {
            FComponent component = GameUIManager.Instance.GetUI("FirstCharSelectPopup");
            if (component == null)
            {
                Debug.LogError("UICinematicPopup에 " + uiName + " 가 존재하지 않습니다.");
                return;
            }
            else
            {
                component.gameObject.SetActive(false);
            }
        }
    }

    private void HideAll()
    {
        HideBossName();
        HideSkipButton();
        HideDialog();

        if (PauseObj)
        {
            PauseObj.SetActive(false);
        }
    }

    public void OnBtnSkip()
    {
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
        {
            TitleUIManager.Instance.PlaySound(0);
        }

        mbShowCursor = AppMgr.Instance.CustomInput.IsShowCursor;
        OnBtnResume();

		World.Instance.SkipAllCutScene(mCurrentDirector);
    }

    public void OnBtnJumpSection()
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("SkipStartScenarioCutScene", "Tutorial", 0);

        HideJumpSection();
        mCurrentDirector.SetTime(mJumpSectionTime);
    }

    bool mbShowCursor = false;
    public void ShowPausePopup()
    {
        if(PauseObj == null)
        {
            return;
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        PauseObj.SetActive(true);

        mbShowCursor = AppMgr.Instance.CustomInput.IsShowCursor;
        AppMgr.Instance.CustomInput.ShowCursor(true);
    }

    public void OnBtnResume()
    {
        if (PauseObj == null)
        {
            return;
        }

        AppMgr.Instance.CustomInput.ShowCursor(mbShowCursor);

        if (Director.CurrentPlaying && Director.CurrentPlaying.isPause)
        {
            Director.CurrentPlaying.Resume();
        }

        PauseObj.SetActive(false);
    }

    public bool IsActiveSkipBtn() {
        return BtnSkip.gameObject.activeSelf;
	}
}
