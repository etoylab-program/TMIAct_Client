
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveVisualNovel : MonoBehaviour
{
    public enum eDirectorPauseType
    {
        None = 0,

        Pause,
        Looping,
    }


    public eDirectorPauseType pauseDirector = eDirectorPauseType.None;
    public float loopStartTime = 0.0f;
    public float loopEndTime = 0.0f;
    public int ScenarioGroupID = -1;
    public bool enableCavansOnEnd = true;

    [Header("[Blur Property]")]
    public bool enableBlur = false;
    public float blurStartTime = 0.0f;

    private Director m_director = null;
    private int popuptype = 0;
    public Director director { get { return m_director; } }


    private void Awake()
    {
        m_director = GetComponentInParent<Director>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (m_director == null || pauseDirector == eDirectorPauseType.None)
            return;

        if (ScenarioGroupID == -1)
            return;

        UIValue.Instance.SetValue(UIValue.EParamType.StroyID, ScenarioGroupID);
        ScenarioMgr.Instance.Open(ScenarioGroupID);

        var data = ScenarioMgr.Instance.GetScenarioParam(0);
        if( data.Pos == 0 )
        {
            UIStroyPopup popup = GameUIManager.Instance.GetUI<UIStroyPopup>("StroyPopup");
            if (popup != null)
                popup.activevn = this;
        }
        else
        {
            UIStoryCommunicationPopup popup = GameUIManager.Instance.GetUI<UIStoryCommunicationPopup>("StoryCommunicationPopup");
            if (popup != null)
                popup.activevn = this;
        }
        popuptype = (int)data.Pos;
        /*
        UIStroyPopup popup = GameUIManager.Instance.GetUI<UIStroyPopup>("StroyPopup");
        if (popup != null)
            popup.activevn = this;

        UIValue.Instance.SetValue(UIValue.EParamType.StroyID, ScenarioGroupID);
        GameUIManager.Instance.ShowUI("StroyPopup", false);
        */
        if (pauseDirector == eDirectorPauseType.Pause)
            m_director.Pause();
        else
        {
            m_director.LockInput(true);
            m_director.SetLoop(loopStartTime, loopEndTime);
        }

        if (enableBlur)
            EnableBlur();
    }

    private void Update()
    {
        if (m_director == null || pauseDirector == eDirectorPauseType.None)
            return;

        //if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Dash"))
        if(AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Select))
        {
            if(popuptype == 0)
            {
                UIStroyPopup popup = GameUIManager.Instance.GetUI<UIStroyPopup>("StroyPopup");
                if (popup != null)
                    popup.OnClick_NextBtn();
            }
            else
            {
                UIStoryCommunicationPopup popup = GameUIManager.Instance.GetUI<UIStoryCommunicationPopup>("StoryCommunicationPopup");
                if (popup != null)
                    popup.OnClick_NextBtn();
            }
        }
        
    }

    public void EnableBlur()
    {
        //World.Instance.playerCam.EnableMotionBlur(0.0f, blurStartTime);
        World.Instance.InGameCamera.EnableMotionBlur(0.0f, blurStartTime);
    }

    public void DisableBlur()
    {
        //World.Instance.playerCam.DisableMotionBlur();
        World.Instance.InGameCamera.DisableMotionBlur();
    }

    public void EndVisualNovel()
    {
        if(enableCavansOnEnd)
            m_director.ActiveCanvas(true);

        if (pauseDirector == eDirectorPauseType.Pause)
        {
            m_director.Resume();
            DisableBlur();
        }
        else
        {
            m_director.LockInput(false);
            m_director.EndLoop();

            DisableBlur();
        }
    }
}
