using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
public class UIStroyPopup : FComponent
{
    public enum eNEXTTYPE
    {
        NEXT = 0,
        TIMENEXT,
        WAIT,
    }

    [SerializeField] private FList _logListInstance;
    [SerializeField] private FList _selListInstance;
    public FToggle kAutoToggle;
    public FToggle kSkipToggle;
    public UIStoryBGUnit kCharBG;
    public GameObject kCharRoot;

    public GameObject kTalkNameRoot;
    public UISprite kTalkBGSprite;

    public List<GameObject> kCharPosList;

    public FLabelTextShow kCharNameLabel;
    public FLabelTextShow kTalkTextLabel;
    public FLabelTextShow kCenterTextLabel;

    public UISprite kTalkArrowSpr;
    public UIButton kNextBtn;
    public UIButton kLogCloseBtn;
    public FAniPlay kTalkAniPlay;
    public GameObject kStoryword;

    public GameObject kStorySelectListObj;
    private TweenScale m_storywordTween;

    [Header("Setting")]
    public float EndCloseTime = 0.2f;
    public float TakeWaitTime = 1.5f;

    private int _scenarioGroupID = 0;
    private bool _isMenuOpen = false;
    private int _storyTalkIndex = 0;
    private int _logIndex = 0;
    private bool _bStoryStart = false;
    private bool _bAuto = false;
    private bool _bRememberValue = false;
    private bool _waitAuto = false;
    private bool _firstFrame = false;
    public ActiveVisualNovel activevn = null;

    private List<ScenarioParam> _scenarioList = new List<ScenarioParam>();
    private List<ScenarioParam> _logScenarioList = new List<ScenarioParam>();

    private Dictionary<string, UIStoryCharUnit> m_dicStoryUnits;
    private Dictionary<string, GameObject> m_dicEffects;
    private Dictionary<string, AudioClip> m_dicSoundEffects;
    private Vector3 m_shakeOriginPos = Vector3.zero;
    private AudioClip m_prevBGM = null;

    private const int m_keyboardSoundCount = 3;
    private List<string> m_keyboardNamelist;
    private Coroutine m_keyboardPlayer = null;
    
    private eScenarioType m_currentType = eScenarioType.NONE;

    public PostProcessVolume kPostProcessVolume;

    //TestValues
    private bool m_FastKeyboardSnd = false;
    private bool m_NoneKeyboardSnd = false;
    private float m_originBGMVolume = 1f;
    private float m_talkBGMVolume = 0.5f;

    //BGM & SE Audios
    private AudioSource m_bgmAudioSource = null;
    private List<AudioSource> m_seAudioSources;

    private Coroutine m_bgmFadeInCoroutine = null;
    private Coroutine m_bgmFadeOutCoroutine = null;

    //UI HIDE
    private bool m_uiHideFlag = false;

    //StoryWord
    private List<int> m_curStoryWordIdx = new List<int>();

    UIGameOffPopup uIGameOffPopup;

    //SelectTalk Values
    private int m_selectTalkCnt = 0;
    private int m_selectEndMoveIdx = 0;
    private StorySelectItemValue m_slotSelectItem = null;
    private List<StorySelectItemValue> m_selectTalkItemList;

    private Vector3 mOriginalCamPos = Vector3.zero;
    private Vector3 mOriginalCamRot = Vector3.zero;
    private float   mOriginalFOV    = 0.0f;

    private float _fadeInVolume = 0f;
    private float _fadeOutVolume = 0f;

    private bool _bSkip = false;

    private Camera _uICamera;
    
    public override void Awake()
	{
        base.Awake();

        if (AppMgr.Instance.WideScreen)
        {
            kLogCloseBtn.transform.localPosition = new Vector3(kLogCloseBtn.transform.localPosition.x - 70f, kLogCloseBtn.transform.localPosition.y, kLogCloseBtn.transform.localPosition.z);
        }

        kAutoToggle.EventCallBack = OnAutoToggleSelect;
        kSkipToggle.EventCallBack = OnSkipToggleSelect;
        this._logListInstance.EventUpdate = this._UpdateLogListSlot;
        this._logListInstance.EventGetItemCount = this._GetFavorabilityLogElementCount;

        this._selListInstance.EventUpdate = this._UpdateSelectListSlot;
        this._selListInstance.EventGetItemCount = this._GetSelectListElementCount;

        UICamera uiCamera = GameObject.FindObjectOfType<UICamera>();
        if (uiCamera != null)
        {
            _uICamera = uiCamera.GetComponent<Camera>();

            if (_uICamera != null)
            {
                _uICamera.farClipPlane = 50;
            }
        }
    }
   
    public override void OnEnable()
	{
        InitComponent();
		base.OnEnable();
        
        if (activevn)
            activevn.director.ActiveCanvas(false);

        UIAnchor anchor = kCharRoot.GetComponent<UIAnchor>();
        if(anchor != null)
        {
            //anchor.side = UIAnchor.Side.Top;
            //anchor.pixelOffset = new Vector2(0, -NGUITools.screenSize.y);
        }

        AppMgr.Instance.CustomInput.ShowCursor(true);

        if (World.Instance.InGameCamera)
        {
            mOriginalCamPos = World.Instance.InGameCamera.transform.position;
            mOriginalCamRot = World.Instance.InGameCamera.transform.rotation.eulerAngles;
            mOriginalFOV = World.Instance.InGameCamera.MainCamera.fieldOfView;
        }
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

        if (_logListInstance != null)
        {
            _logListInstance.gameObject.SetActive(false);
        }
        
        if (kLogCloseBtn != null)
        {
            kLogCloseBtn.gameObject.SetActive(false);
        }

        activevn = null;
        _firstFrame = false;

        _logScenarioList.Clear();

        if (World.Instance.InGameCamera != null)
        {
            World.Instance.InGameCamera.RestoreCullingMask();
            //World.Instance.InGameCamera.SetFixedMode(mOriginalCamPos, mOriginalCamRot, mOriginalFOV, true);
            World.Instance.InGameCamera.SetDefaultMode();
        }

        if (m_prevBGM != null)
        {
            if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
                Log.Show( "BGM Name : " + m_prevBGM.name, Log.ColorType.Red );
                SoundManager.Instance.PlayBgm( m_prevBGM, FSaveData.Instance.GetBGVolume() );
            }
            else if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage ) {
                World.Instance.PlayBgm();
			}

            m_prevBGM = null;
        }

        base.OnDisable();
        if(AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
            AppMgr.Instance.CustomInput.ShowCursor(false);

        InitPos();
    }

    private void Update()
    {
        if (!m_uiHideFlag)
            return;

        if (m_uiHideFlag && AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Select))//Input.GetMouseButtonDown(0))
        {
            m_uiHideFlag = false;
            //ActiveNextBtn();

            _bAuto = _bRememberValue;
            if (_bAuto)
                Invoke("AutoNext", TakeWaitTime);

            float aniTime = kTalkAniPlay.GetTime(0);
            kTalkAniPlay.Play("StroyTalk_In");

            CancelInvoke("ActiveNextBtn");
            Invoke("ActiveNextBtn", 1f);
            return;
        }
    }

    public override void InitComponent()
    {
        SetAudioSources();

        _scenarioGroupID = System.Convert.ToInt32(UIValue.Instance.GetValue(UIValue.EParamType.StroyID));
        if (_scenarioGroupID == 0)
            return;

        kStoryword.SetActive(false);
        m_storywordTween = kStoryword.GetComponent<TweenScale>();
        ScenarioMgr.Instance.InitScenarioScene(_scenarioGroupID);
        _scenarioList = ScenarioMgr.Instance.Scenarios.FindAll(a => a.Group == _scenarioGroupID && a.Type == "CHAR_TALK");

        List<ScenarioParam> t = ScenarioMgr.Instance.Scenarios.FindAll(a => a.Group == _scenarioGroupID);

        kCharBG.Init();

        if (m_dicStoryUnits == null)
            m_dicStoryUnits = new Dictionary<string, UIStoryCharUnit>();
        else
            m_dicStoryUnits.Clear();

        List <ScenarioParam> charList = ScenarioMgr.Instance.Scenarios.FindAll(a => a.Group == _scenarioGroupID && a.Type == "CHAR_LOAD");
        if(charList != null)
        {
            for(int i = 0; i < charList.Count; i++)
            {
                if(!m_dicStoryUnits.ContainsKey(charList[i].Value1))
                {
                    m_dicStoryUnits.Add(charList[i].Value1, StoryCharUnit.CreateStoryUnit(this, kCharRoot.transform, charList[i].Value2, charList[i].Value3, charList[i].Value5));
                    m_dicStoryUnits[charList[i].Value1].gameObject.name = charList[i].Value1;
                    m_dicStoryUnits[charList[i].Value1].SetZAxis((float)i);
                }
            }
        }

        List<ScenarioParam> weaponList = ScenarioMgr.Instance.Scenarios.FindAll(a => a.Group == _scenarioGroupID && a.Type == "CHAR_WEAPON" && a.Value3 == "SHOW");
        if (weaponList != null)
        {
            for (int i = 0; i < weaponList.Count; i++)
            {
                if (m_dicStoryUnits.ContainsKey(weaponList[i].Value1))
                {
                    m_dicStoryUnits[weaponList[i].Value1].SetWeapon(weaponList[i].Value2);
                    m_dicStoryUnits[weaponList[i].Value1].HideWeapon();
                }
            }
        }

        kCharBG.transform.localPosition = new Vector3(0, 0, ((charList.Count + 1) + 1) * 600f);
        kCharBG.ResetPanelPos();
        if (World.Instance.InGameCamera != null)
        {
            World.Instance.InGameCamera.ExcludCullingMask((int)eLayer.TransparentFX);
            World.Instance.InGameCamera.ExcludCullingMask((int)eLayer.Player);
            World.Instance.InGameCamera.ExcludCullingMask((int)eLayer.PlayerClone);
        }

        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
        {
            if (uIGameOffPopup == null)
                uIGameOffPopup = GameUIManager.Instance.GetUI<UIGameOffPopup>("GameOffPopup");
            GameUIManager.Instance.ShowUI("GameOffPopup", false);
        }
        

        kCenterTextLabel.SetText("", false, true);
        kCharNameLabel.SetText("", false, true);
        kTalkTextLabel.SetText("", false, true);
        kNextBtn.gameObject.SetActive(false);

        _storyTalkIndex = 1;
        //_storyTalkIndex = 228;
        _logIndex = 0;
        _bAuto = false;
        _bSkip = false;
        if (m_prevBGM == null)
            m_prevBGM = SoundManager.Instance.GetBgm();
        if (m_prevBGM != null)
            SoundManager.Instance.StopBgm();

        m_keyboardNamelist = new List<string>();
        for (int i = 0; i < m_keyboardSoundCount; i++)
        {
            string fileName = "snd_story_keyboard_eff_0" + (i + 1);
            string soundPath = "Sound/Fx/Story/" + fileName + ".ogg";
            m_keyboardNamelist.Add(fileName);
            SoundManager.Instance.AddAudioClip(fileName, soundPath, FSaveData.Instance.GetSEVolume());
        }

        if(kPostProcessVolume != null)
        {
            kPostProcessVolume.weight = 0f;
            if(kPostProcessVolume.profile.HasSettings(typeof(ColorGrading)))
            {
                ColorGrading colorGrading = kPostProcessVolume.profile.GetSetting<ColorGrading>();
                colorGrading.active = false;
            }

            if(kPostProcessVolume.profile.HasSettings(typeof(Vignette)))
            {
                Vignette vignette = kPostProcessVolume.profile.GetSetting<Vignette>();
                vignette.active = false;
            }
        }

        kAutoToggle.SetToggle(0, SelectEvent.Enable);
        kSkipToggle.SetToggle(0, SelectEvent.Code);
        Invoke("Next", 0.5f);
    }

    #region Audio
    private void SetAudioSources()
    {
        if(m_bgmAudioSource == null)
        {
            m_bgmAudioSource = this.gameObject.AddComponent<AudioSource>();
            m_bgmAudioSource.playOnAwake = false;
            m_bgmAudioSource.loop = true;
            m_bgmAudioSource.volume = 0f;// SoundManager.Instance.GetVolume(SoundManager.eSoundType.Primary, FSaveData.Instance.GetBGVolume());
            m_bgmAudioSource.outputAudioMixerGroup = SoundManager.Instance.GetAudioMixer(SoundManager.eSoundType.Primary);
        }

        m_bgmAudioSource.clip = null;

        if (m_seAudioSources == null)
            m_seAudioSources = new List<AudioSource>();

        for(int i = 0; i < m_seAudioSources.Count; i++)
        {
            m_seAudioSources[i].playOnAwake = false;
            m_seAudioSources[i].loop = false;
            m_seAudioSources[i].clip = null;
            m_seAudioSources[i].volume = 0f;// SoundManager.Instance.GetVolume(SoundManager.eSoundType.FX, FSaveData.Instance.GetSEVolume());
            m_seAudioSources[i].outputAudioMixerGroup = SoundManager.Instance.GetAudioMixer(SoundManager.eSoundType.FX);
        }
    }
 
    AudioSource GetSEAudiosouce(AudioClip ac)
    {
        for(int i = 0; i < m_seAudioSources.Count; i++)
        {
            if (m_seAudioSources[i].isPlaying)
            {
                if(m_seAudioSources[i].clip.Equals(ac))
                return m_seAudioSources[i];
            }
        }

        AudioSource ao = this.gameObject.AddComponent<AudioSource>();
        ao.playOnAwake = false;
        ao.loop = false;
        ao.clip = ac;
        ao.volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.FX, FSaveData.Instance.GetSEVolume());
        ao.outputAudioMixerGroup = SoundManager.Instance.GetAudioMixer(SoundManager.eSoundType.FX);

        m_seAudioSources.Add(ao);

        return ao;
    }

    void PlayBGM(AudioClip ac, float volume, bool bFadeIn = false, float duration = 1f)
    {
        if (bFadeIn)
        {
            if (null == m_bgmAudioSource.clip)
                m_bgmAudioSource.clip = ac;
            else if (!m_bgmAudioSource.clip.Equals(ac))
                m_bgmAudioSource.clip = ac;

            m_bgmFadeInCoroutine = StartCoroutine(FadeINBGMAudio(volume, duration));
        }
        else
        {
            m_bgmAudioSource.volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.Primary, volume);

            if (null == m_bgmAudioSource.clip)
            {
                m_bgmAudioSource.clip = ac;
                m_bgmAudioSource.Play();
            }
            else 
            {
                if (!m_bgmAudioSource.clip.Equals(ac))
                {
                    m_bgmAudioSource.clip = ac;
                    m_bgmAudioSource.Play();
                }
            }
        }
    }

    void StopBGM(float volume,bool bFadeOut, float duration = 1f)
    {
        if (bFadeOut)
            m_bgmFadeOutCoroutine = StartCoroutine(FadeOutBGMAudio(volume));
        else
            m_bgmAudioSource.Stop();
    }

    private IEnumerator FadeOutBGMAudio(float target = 0f, float duration = 1f)
    {
        Debug.Log("FadeOutBGMAudio");

        _fadeOutVolume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.Primary, target);

        while (m_bgmAudioSource.volume > SoundManager.Instance.GetVolume(SoundManager.eSoundType.Primary, target))
        {
            m_bgmAudioSource.volume -= (Time.deltaTime * 0.3f);
            if (m_bgmAudioSource.volume <= 0.05f)
                break;
            yield return null;
        }

        FadeOutVolumeEnded();

        Debug.Log("FadeOutBGMAudio_END");
    }

    private IEnumerator FadeINBGMAudio(float target = 0f, float duration = 1f)
    {
        Debug.Log("FadeINBGMAudio");

        if (!m_bgmAudioSource.isPlaying)
            m_bgmAudioSource.Play();

        _fadeInVolume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.Primary, target);
        
        while (m_bgmAudioSource.volume < SoundManager.Instance.GetVolume(SoundManager.eSoundType.Primary, target))
        {
            m_bgmAudioSource.volume += (Time.deltaTime * 0.3f);
            yield return null;
        }

        FadeInVolumeEnded();
    }

    private void FadeInVolumeEnded()
    {
        m_bgmAudioSource.Play();
        m_bgmAudioSource.volume = _fadeInVolume;
        _fadeInVolume = 0f;
        m_bgmFadeInCoroutine = null;
    }

    private void FadeOutVolumeEnded()
    {
        m_bgmAudioSource.volume = _fadeOutVolume;
        _fadeOutVolume = 0f;
        m_bgmAudioSource.Stop();
        m_bgmFadeOutCoroutine = null;
    }

    void PlaySE(AudioClip ac, float volume, bool bFadeIn, bool bLoop = false)
    {
        AudioSource ao = GetSEAudiosouce(ac);

        ao.loop = bLoop;
        if (bFadeIn)
            StartCoroutine(FadeInSEAudio(ao, volume));
        else
            ao.Play();
    }

    void StopSE(AudioClip ac, float volume, bool bFadeOut = false)
    {
        if (null == m_seAudioSources || m_seAudioSources.Count <= 0)
            return;

        for(int i = 0; i < m_seAudioSources.Count; i++)
        {
            if (null != m_seAudioSources[i].clip)
            {
                if(m_seAudioSources[i].clip.Equals(ac))
                {
                    if (bFadeOut)
                        StartCoroutine(FadeOutSEAudio(m_seAudioSources[i], volume));
                    else
                        m_seAudioSources[i].Stop();
                }
            }
        }
    }

    private IEnumerator FadeOutSEAudio(AudioSource ao, float target = 0f)
    {
        while(ao.volume > SoundManager.Instance.GetVolume(SoundManager.eSoundType.FX, target))
        {
            ao.volume -= (Time.deltaTime * 0.2f);
            yield return null;
        }

        ao.volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.FX, target);
    }

    private IEnumerator FadeInSEAudio(AudioSource ao, float target = 0f)
    {
        if (!ao.isPlaying)
            ao.Play();
        while (ao.volume < SoundManager.Instance.GetVolume(SoundManager.eSoundType.FX, target))
        {
            ao.volume += (Time.deltaTime * 0.2f);
            yield return null;
        }

        ao.volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.FX, target);
    }
    #endregion
    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}
    
    private void _UpdateLogListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIStoryTalkLogSlot talkLogSlot = slotObject.GetComponent<UIStoryTalkLogSlot>();
            if (talkLogSlot == null)
                return;

            talkLogSlot.ParentGO = this.gameObject;
            Debug.Log(index + " / " + _logIndex);
            //  현재까지 등장한 idx보다 슬롯 idx 작은 경우만 활성화
            if (index < _logScenarioList.Count && _logScenarioList[index].Type == "CHAR_TALK")
            {
                talkLogSlot.UpdateSlot(_logScenarioList[index]);
                talkLogSlot.gameObject.SetActive(true);
            }
            else
            {
                talkLogSlot.gameObject.SetActive(false);
            }
        } while (false);
    }

    private int _GetFavorabilityLogElementCount()
    {
        if (null == _logScenarioList || _logScenarioList.Count <= 0)
            return 0;
        return _logScenarioList.Count; //TempValue
    }

    private void _UpdateSelectListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIStorySelectListSlot slot = slotObject.GetComponent<UIStorySelectListSlot>();
            if (slot == null)
                return;

            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(ScenarioMgr.Instance.GetScenarioParam(m_selectTalkItemList[index].kStartIdx), m_selectTalkItemList[index]);
        } while (false);

    }

    private int _GetSelectListElementCount()
    {
        return m_selectTalkCnt;
    }

    public void OnClick_NextBtn()
    {
        if (m_uiHideFlag)
            return;
        if (_waitAuto)
            return;
        if (kLogCloseBtn.gameObject.activeSelf)
            return;
        var param = ScenarioMgr.Instance.GetScenarioParam(_storyTalkIndex);
        if (param == null)
            return;

        if (_bSkip)
        {
            kSkipToggle.SetToggle((int)eCOUNT.NONE + 1, SelectEvent.Code);
            return;
        }

        StopKeyboardSound();
        

        if (param.Type == "CHAR_TALK")
        {
            CancelInvoke("AutoNext");

            if (kTalkTextLabel.TextScroll == true)
            {
                //  모든 텍스트 표시
                kTalkTextLabel.SetText(param.Value2, false, true);

                if (_bAuto == true)
                    Invoke("AutoNext", TakeWaitTime);

                return;
            }
            else
            {
                if (kStoryword.activeSelf)
                    kStoryword.SetActive(false);

                SoundManager.Instance.StopVoice();
                foreach (KeyValuePair<string, UIStoryCharUnit> pair in m_dicStoryUnits)
                {
                    pair.Value.StopVoice();
                    pair.Value.StopTextOnly();
                }
            }
        }

        _storyTalkIndex += 1;
        Next();
    }

    private void SkipNext()
    {
        var param = ScenarioMgr.Instance.GetScenarioParam(_storyTalkIndex);
        if (param == null)
            return;

        if (m_currentType == eScenarioType.SEL_TALK)
            return;
        
        if (kStoryword.activeSelf)
            kStoryword.SetActive(false);

        SoundManager.Instance.StopVoice();

        _storyTalkIndex += 1;
        Next();
    }


    public void OnClick_LogBtn()
    {
        if (_logIndex <= 0)
            return;

        if (kStorySelectListObj.activeSelf)
            return;

        _bRememberValue = _bAuto;
        _bAuto = false;
        CancelInvoke("AutoNext");
        kNextBtn.gameObject.SetActive(false);
        _logListInstance.gameObject.SetActive(true);
        kLogCloseBtn.gameObject.SetActive(true);
        _logListInstance.UpdateList();
        SetSpringScroll(_logIndex);
        
    }

    /// <summary>
    ///  로그 보기 창 닫기
    /// </summary>
    public void OnClick_HideLog()
    {
        _waitAuto = false;
        _bAuto = _bRememberValue;
        if(_bAuto)
            Invoke("AutoNext", TakeWaitTime);

        _logListInstance.gameObject.SetActive(false);
        kLogCloseBtn.gameObject.SetActive(false);
        ActiveNextBtn();
    }

    /// <summary>
    ///  스킵 버튼
    /// </summary>
    public void OnClick_ExitBtn()
    {
        if (kStorySelectListObj.activeSelf)
            return;

        CancelInvoke("AutoNext");
        CancelInvoke("SkipNext");

        int stageTableId = -1;
        if (World.Instance.StageInfoType == World.eStageInfoType.New)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("SkipStory", new Firebase.Analytics.Parameter("StageId", stageTableId),
                                                                       new Firebase.Analytics.Parameter("StoryId", _scenarioGroupID));
        }

        SoundManager.Instance.StopVoice();
        foreach (KeyValuePair<string, UIStoryCharUnit> pair in m_dicStoryUnits)
        {
            pair.Value.StopVoice();
        }

        if (kTalkAniPlay.kIndex == 0)
            kTalkAniPlay.Play(1);
        kTalkAniPlay.gameObject.SetActive(false);
        CharListRemove();
        EffListRemove();
        //OnClickClose();
        if(AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
        {
            GameUIManager.Instance.ShowUI("GameOffPopup", false);
            SetUIActive(false, false);
            return;
        }

        //끝나고 애니메이션
        SetUIActive(false, true);
    }

    public void OnClick_HideUIBtn()
    {
        if (kStorySelectListObj.activeSelf)
            return;
        if (_logListInstance.gameObject.activeSelf)
            return;
        //OnAutoToggleSelect(0, SelectEvent.Click);
        kNextBtn.gameObject.SetActive(false);
        m_uiHideFlag = true;

        _bRememberValue = _bAuto;
        _bAuto = false;
        CancelInvoke("AutoNext");

        kTalkAniPlay.Play("StroyTalk_Out");
    }

    public void OnClick_StorywordBtn()
    {
        if (kStorySelectListObj.activeSelf)
            return;
        _bRememberValue = _bAuto;
        _bAuto = false;
        CancelInvoke("AutoNext");

        MessagePopup.TextLong(FLocalizeString.Instance.GetText(1368), GetStoryTream(), true, OnStorywordClose);
    }

    public void OnStorywordClose()
    {
        _bAuto = _bRememberValue;
        if (_bAuto)
            Invoke("AutoNext", TakeWaitTime);
    }

    string GetStoryTream()
    {
        StringBuilder str = new StringBuilder();

        for(int i = 0; i < m_curStoryWordIdx.Count; i++)
        {
            TremParam param = ScenarioMgr.Instance.GetScenarioTremParam(m_curStoryWordIdx[i]);
            str.Append(string.Format("{0}\n\n", param.Title));
            str.Append(param.Desc);
            str.Append("\n\n\n");
        }

        return str.ToString();
    }

    //public void OnClick_AutoBtn()
    //{
    //    _bAuto = !_bAuto;
    //    Next();
    //    kMenuToggle.SetToggle(0, SelectEvent.Code);
    //}

    private bool OnAutoToggleSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
            return true;
        if (kStorySelectListObj.activeSelf)
            return false;

        _bAuto = System.Convert.ToBoolean(nSelect);
        if(_bSkip && _bAuto)
            kSkipToggle.SetToggle((int)eCOUNT.NONE, SelectEvent.Code);

        if (type == SelectEvent.Code)
        {
            return true;
        }


        OnClick_NextBtn();
        if (!_bAuto)
            ActiveNextBtn();
        return true;
    }

    private bool OnSkipToggleSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
            return false;
        if (kStorySelectListObj.activeSelf)
            return false;

        _bSkip = System.Convert.ToBoolean(nSelect);
        if(_bAuto && _bSkip)
            kAutoToggle.SetToggle((int)eCOUNT.NONE, SelectEvent.Code);

        if (type == SelectEvent.Code)
        {

            return true;
        }

        if (_bSkip)
        {
            StopKeyboardSound();

            SoundManager.Instance.StopVoice();
            foreach (var storyUnit in m_dicStoryUnits)
            {
                storyUnit.Value.StopVoice();
            }

            foreach (var audioSource in m_seAudioSources)
            {
                audioSource.Stop();
            }
        }
        SkipNext();
        ActiveNextBtn();

        return true;
    }

    public void Next()
    {
        if (m_slotSelectItem != null)
        {
            if (m_slotSelectItem.kEndIdx < _storyTalkIndex)
            {
                _storyTalkIndex = m_selectEndMoveIdx;
                m_slotSelectItem = null;
            }

        }

        var param = ScenarioMgr.Instance.GetScenarioParam(_storyTalkIndex);
        if (param == null)
            return;

        //Debug.Log(param.Value2);
        kNextBtn.gameObject.SetActive(false);
        float fwait = 0.0f;
        float ftimescale = 1.0f;
        //if (_bAuto)
        //    ftimescale = 0.2f;

        float ftime = param.Time * ftimescale;

        int unitcount = 0;

        m_currentType = (eScenarioType)System.Enum.Parse(typeof(eScenarioType), param.Type);
        
        if(m_currentType != eScenarioType.CHAR_TALK && m_currentType != eScenarioType.CHAR_TALK_S)
            param.Value1 = param.Value1.Replace(" ", "");

        param.Value3 = param.Value3.Replace(" ", "");
        param.Value4 = param.Value4.Replace(" ", "");
        param.Value5 = param.Value5.Replace(" ", "");
        param.Voice = param.Voice.Replace(" ", "");

        Log.Show(m_currentType);


        switch (m_currentType)
        {
            case eScenarioType.CHAR_LOAD:
                GameOffPopupHide();
                break;
            case eScenarioType.CHAR_DESTROY:
                break;
            case eScenarioType.CHAR_SET:
                {
                    if (!m_dicStoryUnits.ContainsKey(param.Value1))
                    {
                        Debug.LogError("Animation Target is NULL");
                        _storyTalkIndex += 1;
                        Next();
                        return;
                    }

                    m_dicStoryUnits[param.Value1].SetAnimation(param.Value2, param.Value3, param.Value4);

                    if (!string.IsNullOrEmpty(param.Value4))
                    {
                        if (param.Value4.Equals("SHY_ON"))
                            m_dicStoryUnits[param.Value1].SetShyFace(true);
                        else if (param.Value4.Equals("SHY_OFF"))
                            m_dicStoryUnits[param.Value1].SetShyFace(false);
                    }

                    if (param.Value5.Equals("LEFT"))
                    {
                        m_dicStoryUnits[param.Value1].SetPos(-5f);
                        m_dicStoryUnits[param.Value1].MovePos(param.Pos, _bSkip ? 0f : param.Time);
                    }
                    else if (param.Value5.Equals("RIGHT"))
                    {
                        m_dicStoryUnits[param.Value1].SetPos(5f);
                        m_dicStoryUnits[param.Value1].MovePos(param.Pos, _bSkip ? 0f : param.Time);
                    }
                    else if (param.Value5.Equals("BLANK"))
                    {
                        m_dicStoryUnits[param.Value1].SetPos(param.Pos);
                    }
                    else if (param.Value5.Equals("ZAXIS"))
                    {
                        m_dicStoryUnits[param.Value1].SetZAxis(param.Pos);
                    }
                    else if (param.Value5.Equals("ROTATION"))
                    {
                        m_dicStoryUnits[param.Value1].SetCharRotation(param.Pos, _bSkip ? 0f : ftime);
                    }
                    fwait = ftime;
                    if (_bSkip)
                        fwait = 0f;
                }
                break;
            case eScenarioType.CHAR_ROT:
                {
                    m_dicStoryUnits[param.Value1].SetCharRotation(param.Pos, _bSkip ? 0f : ftime);
                    fwait = ftime;
                }
                break;
            case eScenarioType.CHAR_SCALE:
                {
                    m_dicStoryUnits[param.Value1].SetCharLocalScale(param.Pos);

                }
                break;
            case eScenarioType.CHAR_FADE:
                {
                    if (param.Value4.Equals("FADE_IN"))
                    {
                        m_dicStoryUnits[param.Value1].SetPos(param.Pos);
                        m_dicStoryUnits[param.Value1].SetCharAlpha(false, _bSkip ? 0f : ftime);
                    }
                    else if (param.Value4.Equals("FADE_OUT"))
                    {
                        m_dicStoryUnits[param.Value1].SetCharAlpha(true, param.Pos, _bSkip ? 0f : ftime);
                    }
                    fwait = ftime;
                }
                break;
            case eScenarioType.MOVE_TO:
                {
                    //param.Next = (int)eNEXTTYPE.TIMENEXT;
                    m_dicStoryUnits[param.Value1].MovePos(param.Pos, _bSkip ? 0f : param.Time);
                    fwait = ftime;
                }
                break;
            case eScenarioType.MOVE_NONE_Z:
                {
                    m_dicStoryUnits[param.Value1].MovePos(param.Pos, _bSkip ? 0f : param.Time, true);
                    fwait = ftime;
                }
                break;
            case eScenarioType.BG_LOAD:
                {
                    param.Next = (int)eNEXTTYPE.TIMENEXT;
                    if (kTalkAniPlay.kIndex == 0)
                        kTalkAniPlay.Play("StroyTalk_Out");

                    if (uIGameOffPopup != null && uIGameOffPopup.gameObject.activeSelf)
                    {
                        CancelInvoke("GameOffPopupHide");
                        Invoke("GameOffPopupHide", param.Time);
                    }

                    fwait = ftime = param.Time;
                    if (string.IsNullOrEmpty(param.Value2))
                    {
                        if (param.Value4.Equals("FADE_OUT"))
                        {
                            kCharBG.FadeOut(_bSkip ? 0f : param.Time);
                        }
                        else
                        {
                            Debug.LogError("바꿀 배경이미지의 경로가 없습니다.");
                        }
                    }
                    else
                    {
						string[] path = Utility.Split(param.Value2, '/'); //param.Value2.Split('/');
						if (param.Value1.Equals("CHANGE"))
                        {
                            if (param.Value4.Equals("CROSS"))
                                kCharBG.ChangeBG(path[0].ToLower(), param.Value2, _bSkip ? 0f : param.Time);
                            else
                                kCharBG.SetTexture(path[0].ToLower(), param.Value2);
                        }
                        else if (param.Value1.Equals("FADE_OUT"))
                        {
                            kCharBG.FadeOut(_bSkip ? 0f : param.Time);
                        }
                        else if (param.Value1.Equals("FADE_IN"))
                        {
                            kCharBG.FadeIn(path[0].ToLower(), param.Value2, _bSkip ? 0f : param.Time);
                        }
                        else if (param.Value1.Equals("ADD"))
                        {
                            Debug.LogError("지원하지 않는 기능입니다.");
                        }
                        else if (param.Value1.Equals("ROTATE"))
                        {
                            if (param.Value4.Equals("FORWARD"))      //시계방향 회전
                            {
                                kCharBG.RotateChangeBG(path[0].ToLower(), param.Value2, "", param.Value5, true, _bSkip ? 0f : (param.Time >= 1f) ? param.Time : 2f, 
                                    () => {
                                        foreach (KeyValuePair<string, UIStoryCharUnit> pair in m_dicStoryUnits)
                                            pair.Value.ReSetPosistion();
                                    });
                            }
                            else if (param.Value4.Equals("REVERSE"))     //시계 반대방향 회전
                            {
                                kCharBG.RotateChangeBG(path[0].ToLower(), param.Value2, "", param.Value5, false, _bSkip ? 0f : (param.Time >= 1f) ? param.Time : 2f,
                                    () => {
                                        foreach (KeyValuePair<string, UIStoryCharUnit> pair in m_dicStoryUnits)
                                            pair.Value.ReSetPosistion();
                                    });
                            }
                        }
                        else if (param.Value1.Equals("MOVE"))
                        {
                            kCharBG.MoveChangeBG(param.Value4, param.Value5, path[0].ToLower(), param.Value2, "", _bSkip ? 0f : (param.Time >= 1f) ? param.Time : 2f,
                                () => {
                                    foreach (KeyValuePair<string, UIStoryCharUnit> pair in m_dicStoryUnits)
                                        pair.Value.ReSetPosistion();
                                });
                        }
                        else if (param.Value1.Equals("COLOR"))
                        {
                            //if (_bSkip)
                            //    break;

                            if (string.IsNullOrEmpty(param.Value4))
                            {
                                Debug.LogError("배경 색상이 빈 칸입니다.");
                            }
                            else
                            {
                                param.Value4 = param.Value4.Replace(" ", "");
								string[] colors = Utility.Split(param.Value4, ','); //param.Value4.Split(',');
                                if (colors.Length < 4)
                                    Debug.LogError("색상 값이 잘못 들어갔습니다.");
                                else
                                {
                                    Color color = new Color(Utility.SafeParse(colors[0]), Utility.SafeParse(colors[1]), Utility.SafeParse(colors[2]), 1f);
                                    kCharBG.ColorOutChangeBG(path[0].ToLower(), param.Value2, "", color, _bSkip ? 0f : (param.Time >= 1f) ? param.Time : 2f);
                                }
                            }
                        }
                    }
                    //ftime += 0.5f;
                }
                break;
            case eScenarioType.BG_INGAME_CAMERA:
                {
                    if (AppMgr.Instance.SceneType != AppMgr.eSceneType.Stage)
                        break;

                    param.Next = (int)eNEXTTYPE.TIMENEXT;
                    if (kTalkAniPlay.kIndex == 0)
                        kTalkAniPlay.Play("StroyTalk_Out");

                    if (uIGameOffPopup != null && uIGameOffPopup.gameObject.activeSelf)
                    {
                        CancelInvoke("GameOffPopupHide");
                        Invoke("GameOffPopupHide", param.Time);
                    }

                    fwait = ftime = param.Time;
                    if(string.IsNullOrEmpty(param.Value1))
                    {
                        if (string.IsNullOrEmpty(param.Value2))
                        {
                            if (param.Value4.Equals("FADE_OUT"))
                            {
                                kCharBG.FadeOut(_bSkip ? 0f : param.Time);
                            }
                            else
                            {
                                Debug.LogError("인게임 카메라의 정보가 없습니다.");
                            }
                        }
                        else
                        {
                            SetInGameCameraPos(param.Value2);
                        }
                    }
                    else if(param.Value1.Equals("MOVE"))
                    {
                        kCharBG.MoveChangeBG(param.Value4, "0,0,0", string.Empty, string.Empty, "", _bSkip ? 0f : (param.Time >= 1f) ? param.Time : 2f,
                                () => {
                                    kCharBG.DisableTexture();
                                    SetInGameCameraPos(param.Value2);

                                    foreach (KeyValuePair<string, UIStoryCharUnit> pair in m_dicStoryUnits)
                                        pair.Value.ReSetPosistion();
                                });
                    }
                    //ftime += 0.5f;
                }
                break;
            case eScenarioType.EYE_EFFECT:
                {
                    //if (kTalkAniPlay.kIndex == 0)
                    //    kTalkAniPlay.Play("StroyTalk_Out");
                    //param.Next = (int)eNEXTTYPE.TIMENEXT;
                    //fwait = ftime;
                    //string[] path = param.Value2.Split('/');
                    //if (param.Value1.Equals("OPEN"))
                    //{
                    //    kCharBG.EyeChangeBG(path[0].ToLower(), param.Value2, "", true, ftime);
                    //}
                    //else if (param.Value1.Equals("CLOSE"))
                    //{
                    //    kCharBG.EyeChangeBG(path[0].ToLower(), param.Value2, "", false, ftime);
                    //}
                    //else if (param.Value1.Equals("ONCE"))
                    //{
                    //    kCharBG.EyeChangeBG(path[0].ToLower(), param.Value2, "", ftime);
                    //    fwait = ftime = ftime + (ftime * 2);
                    //}
                }
                break;
            case eScenarioType.BG_COLOR:
                {
                    if (_bSkip)
                        break;

                    if (!string.IsNullOrEmpty(param.Value3))
                    {
                        if (kTalkAniPlay.kIndex == 0)
                            kTalkAniPlay.Play("StroyTalk_Out");
                        param.Next = (int)eNEXTTYPE.TIMENEXT;
                        fwait = ftime;

						string[] colors = Utility.Split(param.Value3, ','); //param.Value3.Split(',');
                        if (colors.Length < 4)
                            Debug.LogError("색상 값이 잘못들어갔습니다.");
                        else
                        {
                            Color color = new Color(Utility.SafeParse(colors[0]), Utility.SafeParse(colors[1]), Utility.SafeParse(colors[2]), Utility.SafeParse(colors[3]));
                            if (string.IsNullOrEmpty(param.Value4) || param.Value4.Equals("NONE"))
                                kCharBG.SetBGColor(color, 0f);
                            else if (param.Value4.Equals("FADE"))
                                kCharBG.SetBGColor(color, _bSkip ? 0f : param.Time);
                        }
                    }
                }
                break;
            case eScenarioType.BGM_PLAY:
                {
                    if(!string.IsNullOrEmpty(param.Value3))
                    {
                        param.Value3 = param.Value3.Replace(" ", "");
						string[] volumes = Utility.Split(param.Value3, ','); //param.Value3.Split(',');
						if (volumes.Length >= 2)
                        {
                            m_originBGMVolume = Utility.SafeParse( volumes[0]);
                            m_talkBGMVolume = Utility.SafeParse(volumes[1]);
                        }
                    }
                    else
                    {
                        m_originBGMVolume = 0f;
                        m_talkBGMVolume = 0f;
                    }

                    if (m_bgmFadeInCoroutine != null)
                    {
                        StopCoroutine(m_bgmFadeInCoroutine);
                        FadeInVolumeEnded();
                    }
                        
                    if (m_bgmFadeOutCoroutine != null)
                    {
                        StopCoroutine(m_bgmFadeOutCoroutine);
                        FadeOutVolumeEnded();
                    }
                        

                    if (param.Value1.Equals("PLAY"))
                    {
                        AudioClip ac = ResourceMgr.Instance.LoadFromAssetBundle("sound", param.Value2) as AudioClip;
                        PlayBGM(ac, param.Pos, false);
                    }
                    else if (param.Value1.Equals("FADE_IN"))
                    {
                        AudioClip ac = ResourceMgr.Instance.LoadFromAssetBundle("sound", param.Value2) as AudioClip;
                        PlayBGM(ac, param.Pos, true, _bSkip ? 0f : param.Time);
                    }
                    else if(param.Value1.Equals("FADE_OUT"))
                    {
                        StopBGM(param.Pos, true, _bSkip ? 0f : 1f);
                    }
                }
                break;
            case eScenarioType.BGM_STOP:
                {
                    if(param.Value1.Equals("FADE_OUT"))
                    {
                        StopBGM(param.Pos, true, _bSkip ? 0f : 1f);
                    }
                    else
                    {
                        StopBGM(param.Pos, false, _bSkip ? 0f : 1f);
                    }
                }
                break;
            case eScenarioType.SE_PLAY:
                {
                    if (m_dicSoundEffects == null)
                        m_dicSoundEffects = new Dictionary<string, AudioClip>();

                    if (_bSkip)
                        break;

                    if (param.Value3.Equals("LOOP"))
                    {
                        if (!m_dicSoundEffects.ContainsKey(param.Value1))
                        {
                            AudioClip ac = ResourceMgr.Instance.LoadFromAssetBundle("sound", param.Value2) as AudioClip;
                            m_dicSoundEffects.Add(param.Value1, ac);

                            if(param.Value4.Equals("FADE_OUT"))
                            {
                                StopSE(ac, param.Pos, true);
                            }
                            else if(param.Value4.Equals("FADE_IN"))
                            {
                                PlaySE(ac, param.Pos, true, true);
                            }
                            else
                            {
                                PlaySE(ac, param.Pos, false, true);
                            }
                        }
                    }
                    else
                    {
                        AudioClip ac = ResourceMgr.Instance.LoadFromAssetBundle("sound", param.Value2) as AudioClip;
                        if(!m_dicSoundEffects.ContainsKey(param.Value1))
                            m_dicSoundEffects.Add(param.Value1, ac);

                        if (param.Value4.Equals("FADE_OUT"))
                        {
                            StopSE(ac, param.Pos, true);
                        }
                        else if (param.Value4.Equals("FADE_IN"))
                        {
                            PlaySE(ac, param.Pos, true, false);
                        }
                        else
                        {
                            PlaySE(ac, param.Pos, false, false);
                        }
                    }
                }
                break;
            case eScenarioType.SE_STOP:
                {
                    if (_bSkip)
                        break;

                    if (null != m_dicSoundEffects)
                    {
                        AudioClip ac = m_dicSoundEffects[param.Value1];
                        if (param.Value4.Equals("FADE_OUT"))
                        {
                            StopSE(ac, param.Pos, true);
                        }
                        else
                        {
                            StopSE(ac, param.Pos);
                        }
                    }
                }
                break;
            case eScenarioType.CHAR_SHAKE:
                {
                    if (_bSkip)
                        break;

                    if (m_dicStoryUnits.ContainsKey(param.Value1))
                    {
                        SkakeTalkObject(m_dicStoryUnits[param.Value1].gameObject, _bSkip ? 0f : param.Time);
                    }
                }
                break;
            case eScenarioType.CHAR_SHAKE_ON:
                {
                    if (_bSkip)
                        break;

                    if (m_dicStoryUnits.ContainsKey(param.Value1))
                    {
                        m_dicStoryUnits[param.Value1].PlayShakeChar(param.Pos);
                    }
                }
                break;
            case eScenarioType.CHAR_SHAKE_OFF:
                {
                    if (_bSkip)
                        break;

                    if (m_dicStoryUnits.ContainsKey(param.Value1))
                    {
                        m_dicStoryUnits[param.Value1].StopShakeChar();
                    }
                }
                break;
            case eScenarioType.SHAKE_BG:
                {
                    if (param.Value1.Equals("BG"))
                    {
                        SkakeTalkObject(kCharBG.gameObject, _bSkip ? 0f : param.Time);
                    }
                    else if (param.Value1.Equals("TALK"))
                    {
                        SkakeTalkObject(kTalkAniPlay.gameObject, _bSkip ? 0f : param.Time);
                    }
                    else if (param.Value1.Equals("ALL"))
                    {
                        ShakeAll(_bSkip ? 0f : param.Time);
                    }
                }
                break;
            case eScenarioType.CHAR_STATE:
                {
					string[] talks = Utility.Split(param.Value1, ','); //param.Value1.Split(',');
                    string[] listen = Utility.Split(param.Value2, ','); //param.Value2.Split(',');
					string[] stand = Utility.Split(param.Value3, ','); //param.Value3.Split(',');

					for (int i = 0; i < talks.Length; i++)
                    {

                        if (!string.IsNullOrEmpty(talks[i]))
                        {
                            m_dicStoryUnits[talks[i]].Talk(_bSkip ? 0f : param.Time);
                        }
                    }

                    if (param.Value2.Equals("ALL"))
                    {
                        foreach (KeyValuePair<string, UIStoryCharUnit> pair in m_dicStoryUnits)
                        {
                            bool checker = false;
                            for (int i = 0; i < talks.Length; i++)
                            {
                                if (talks[i].Equals(pair.Key))
                                    checker = true;
                            }
                            if (!checker)
                                pair.Value.Listen(_bSkip ? 0f : param.Time);
                        }
                    }
                    else
                    {

                        for (int i = 0; i < listen.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(listen[i]))
                                m_dicStoryUnits[listen[i]].Listen(_bSkip ? 0f : param.Time);
                        }
                    }

                    if (param.Value3.Equals("ALL"))
                    {
                        foreach (KeyValuePair<string, UIStoryCharUnit> pair in m_dicStoryUnits)
                        {
                            bool checker = false;
                            for (int i = 0; i < talks.Length; i++)
                            {
                                if (talks[i].Equals(pair.Key))
                                    checker = true;
                            }
                            for (int i = 0; i < listen.Length; i++)
                            {
                                if (listen[i].Equals(pair.Key))
                                    checker = true;
                            }
                            if (!checker)
                                pair.Value.Stand(_bSkip ? 0f : param.Time);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < stand.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(stand[i]))
                            {
                                m_dicStoryUnits[stand[i]].Stand(_bSkip ? 0f : param.Time);
                            }
                        }
                    }

                    
                    if(!param.Value5.Equals("FIXED"))
                        RePosition();
                }
                break;
            case eScenarioType.CHAR_TALK:
                {
                    _logScenarioList.Add(param);

                    if (!kTalkAniPlay.gameObject.activeSelf)
                        kTalkAniPlay.gameObject.SetActive(true);
                    

                    if(!string.IsNullOrEmpty(param.Value1) && param.Value1 != "")
                    {
                        kTalkNameRoot.SetActive(true);
                        kTalkTextLabel.kLabel.color = Color.white;
                        kTalkBGSprite.spriteName = "frm_Story_C";
                    }
                    else
                    {
                        kTalkNameRoot.SetActive(false);
                        kTalkTextLabel.kLabel.color = new Color(0.9333333f, 0.8745098f, 0.4666667f, 1f);
                        kTalkBGSprite.spriteName = "frm_Story_Noname";
                    }

                    if (kTalkAniPlay.kIndex == 1)
                        kTalkAniPlay.Play("StroyTalk_In");

                    SetText(kCharNameLabel, param.Value1, false);
                    SetText(kTalkTextLabel, param.Value2, _bSkip ? false : true);

                    if(!string.IsNullOrEmpty(param.TremIndex))
                    {
                        m_curStoryWordIdx.Clear();
						string[] words = Utility.Split(param.TremIndex, ','); //param.TremIndex.Split(',');
						for (int i = 0; i < words.Length; i++)
                            m_curStoryWordIdx.Add(int.Parse(words[i]));

                        kStoryword.SetActive(true);
                        TweenScale.SetTweenScale(m_storywordTween, UITweener.Style.Once, new Vector3(0.01f, 0.01f, 0.01f), Vector3.one, 0.5f, 0f, null);
                    }

                    if (_bSkip)
                        break;

                    float voiceTotalTime = 0f;
                    if (param.Voice != "" && param.Voice != string.Empty)
                    {
						string[] voices = Utility.Split(param.Voice, ','); //param.Voice.Split(',');
						if (!string.IsNullOrEmpty(param.Value3))
                        {
							string[] charVoices = Utility.Split(param.Value3, ','); //param.Value3.Split(',');
                            Debug.Log("voice : " + charVoices.Length + " / " + voices.Length);
                            for (int i = 0; i < voices.Length; i++)
                            {
                                if (m_dicStoryUnits.ContainsKey(charVoices[i]))
                                {
                                    voiceTotalTime += m_dicStoryUnits[charVoices[i]].PlayVoice(param.BundlePath, voices[i]);
                                }
                                else
                                {
                                    //SoundManager.sSoundInfo voice = VoiceMgr.Instance.PlayStory(param.BundlePath, voices[i], FSaveData.Instance.GetVoiceVolume().Equals(0f) ? 0f : 1f);
                                    SoundManager.sSoundInfo voice = VoiceMgr.Instance.PlayStory(param.BundlePath, voices[i], FSaveData.Instance.GetVoiceVolume());
                                    voiceTotalTime += voice.clip.length;
                                }
                            }

                            if (param.Value4.Equals("TEXT_ONLY"))
                            {
								string[] mouthValue = Utility.Split(param.Value5, ','); //param.Value5.Split(',');
                                for(int i = 0; i < charVoices.Length; i++)
                                {
                                    m_dicStoryUnits[charVoices[i]].PlayTextOnly(kTalkTextLabel, Utility.SafeParse(mouthValue[0]), Utility.SafeParse(mouthValue[1]));
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < voices.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(voices[i]))
                                {
                                    //SoundManager.sSoundInfo voice = VoiceMgr.Instance.PlayStory(param.BundlePath, voices[i], FSaveData.Instance.GetVoiceVolume().Equals(0f) ? 0f : 1f);
                                    SoundManager.sSoundInfo voice = VoiceMgr.Instance.PlayStory(param.BundlePath, voices[i], FSaveData.Instance.GetVoiceVolume());
                                    voiceTotalTime += voice.clip.length;
                                }

                            }
                        }
                        //SEPlayCheck(true);
                        if(m_bgmAudioSource.volume > m_talkBGMVolume)
                        {
                            if (m_bgmFadeInCoroutine != null)
                            {
                                StopCoroutine(m_bgmFadeInCoroutine);
                                FadeInVolumeEnded();
                            }
                            if (m_bgmFadeOutCoroutine != null)
                            {
                                StopCoroutine(m_bgmFadeOutCoroutine);
                                FadeOutVolumeEnded();
                            }
                                

                            m_bgmAudioSource.volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.Primary, m_talkBGMVolume);
                        }

                       

                        if (_bAuto)
                        {
                            ftime = (voiceTotalTime / voices.Length);
                            if (param.Value4.Equals("TEXT_ONLY"))
                                ftime = param.Value2.Length * kTalkTextLabel.kCharSpeed;

                            ftime += GetAutoCharVoice();
                        }
                        //else
                        //{
                        //    ftime = (voiceTotalTime / voices.Length) + 0.5f;
                        //}
                    }
                    else
                    {
                        if (param.Value4.Equals("TEXT_ONLY"))
                        {
							string[] mouthValue = Utility.Split(param.Value5, ','); //param.Value5.Split(',');
							string[] charVoices = Utility.Split(param.Value3, ','); //param.Value3.Split(',');
							for (int i = 0; i < charVoices.Length; i++)
                            {
                                m_dicStoryUnits[charVoices[i]].PlayTextOnly(kTalkTextLabel, Utility.SafeParse(mouthValue[0]), Utility.SafeParse(mouthValue[1]));
                            }
                        }
                        else
                        {
                            if (!m_NoneKeyboardSnd)
                            {
                                ftime = param.Value2.Length * (kTalkTextLabel.kCharSpeed * (m_FastKeyboardSnd ? 0.5f : 1));
                                PlayKeyboardSound(ftime);
                                ftime += 1.5f;
                            }
                        }
                        

                        if(m_bgmAudioSource.volume < m_originBGMVolume)
                        {
                            if (m_bgmFadeInCoroutine != null)
                            {
                                StopCoroutine(m_bgmFadeInCoroutine);
                                FadeInVolumeEnded();
                            }

                            if (m_bgmFadeOutCoroutine != null)
                            {
                                StopCoroutine(m_bgmFadeOutCoroutine);
                                FadeOutVolumeEnded();
                            }

                            m_bgmAudioSource.volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.Primary, m_originBGMVolume);

                            //StartCoroutine(FadeINBGMAudio(m_talkBGMVolume));
                        }
                        if (_bAuto)
                        {
                            if (param.Value4.Equals("TEXT_ONLY"))
                                ftime = param.Value2.Length * kTalkTextLabel.kCharSpeed;
                            ftime += GetAutoNarration();
                        }
                        //SEPlayCheck(false);
                    }
                    //RePosition();

                    

                    _logIndex++;
                }
                break;
            case eScenarioType.CHAR_WEAPON:
                {
                    if (m_dicStoryUnits.ContainsKey(param.Value1))
                    {
                        if (param.Value3.Equals("SHOW"))
                        {
                            m_dicStoryUnits[param.Value1].ShowWeapon(param.Value2);

						}
                        else if (param.Value3.Equals("HIDE"))
                        {
                            m_dicStoryUnits[param.Value1].HideWeapon();
                        }
                    }
                }
                break;
            case eScenarioType.CHAR_YAXIS:
                {
                    if(m_dicStoryUnits != null)
                    {
                        foreach(KeyValuePair<string, UIStoryCharUnit> pair in m_dicStoryUnits)
                        {
                            pair.Value.SetRootPosY(param.Pos);
                        }
                    }
                }
                break;
            case eScenarioType.EFF_PLAY:
                {
                    if (_bSkip)
                        break;

                    GameObject effObj = AddEffect(param.Value1, param.Value2);
                    if (param.Value3.Equals("SCREEN_UP"))
                    {
                        Debug.Log(Screen.height + " / " + Screen.currentResolution.height);
                        effObj.transform.localPosition = new Vector3(0, (Screen.currentResolution.height), 0);
                    }
                    else
                    {
                        effObj.transform.localPosition = GetPosition(param.Value4);
                        effObj.transform.localRotation = GetRotation(param.Value5);
                    }
                    effObj.SetActive(false);
                    effObj.SetActive(true);
                }
                break;
            case eScenarioType.EFF_STOP:
                {
                    if (null != m_dicEffects && m_dicEffects.ContainsKey(param.Value1))
                    {
                        m_dicEffects[param.Value1].SetActive(false);
                    }
                }
                break;
            case eScenarioType.TALK_IN:
                {
                    //kTalkAniPlay.Play("StroyTalk_In");
                    fwait = ftime;
                }
                break;
            case eScenarioType.TALK_OUT:
                {
                    kTalkAniPlay.Play("StroyTalk_Out");
                    fwait = ftime;

                    Invoke("InitPos", fwait);
                }
                break;
            case eScenarioType.TEXT_CENTER:
                {
                    if (_bAuto || param.Value1 == "" || param.Value1 == string.Empty)
                        kCenterTextLabel.SetText(param.Value1, false, true);
                    else
                        kCenterTextLabel.SetText(param.Value1, true, true);
                }
                break;
            case eScenarioType.POST_COLOR:
                {
                    if (_bSkip)
                        break;

                    param.Next = (int)eNEXTTYPE.TIMENEXT;
					//추후 적용 현재는 사이렌 기능만 적용
					string[] colors = Utility.Split(param.Value1, ','); //param.Value1.Split(',');
                    Color color = new Color(Utility.SafeParse(colors[0]), Utility.SafeParse(colors[1]), Utility.SafeParse(colors[2]), (colors.Length == 3) ? 1f : Utility.SafeParse(colors[3])); ;
                    

                    if(kPostProcessVolume.profile.HasSettings(typeof(ColorGrading)))
                    {
                        Debug.Log("color");
                        ColorGrading colorSettings = kPostProcessVolume.profile.GetSetting<ColorGrading>();
                        colorSettings.colorFilter.value = color;
                        StartCoroutine(TweenPostProcessColorGrading(param.Time, (int)param.Pos));
                    }

                   
                    ftime = param.Time;
                }
                break;
            case eScenarioType.BLR_ON:      //3D Blur
                {
                    if (_bSkip)
                        break;

                    if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
                        World.Instance.InGameCamera.EnableMotionBlur(0.0f, param.Pos);
                }
                break;
            case eScenarioType.BLR_OFF:     //3D Blur
                {
                    if (_bSkip)
                        break;

                    if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
                        World.Instance.InGameCamera.DisableMotionBlur();
                }
                break;
            case eScenarioType.BLR_TEX_ON:  //2D Blur
                {
                    kCharBG.BlurOn(param.Pos);
                }
                break;
            case eScenarioType.BLR_TEX_OFF:  //2D Blur
                {
                    kCharBG.BlurOff();
                }
                break;
            case eScenarioType.BG_FADE_IN:
                {
                    if (kTalkAniPlay.kIndex == 0)
                        kTalkAniPlay.Play("StroyTalk_Out");
                    kCharBG.FadeIn(param.Value1, _bSkip ? 0f : ftime);
                    fwait = ftime;
                }
                break;
            case eScenarioType.BG_FADE_OUT:
                {
                    if (kTalkAniPlay.kIndex == 0)
                        kTalkAniPlay.Play("StroyTalk_Out");
                    kCharBG.FadeOut(_bSkip ? 0f : ftime);
                    fwait = ftime;
                }
                break;
            case eScenarioType.SEL_TALK:
                {
                    CancelInvoke("SkipNext");
                    CancelInvoke("AutoNext");
					string[] selTalks = Utility.Split(param.Value1, ','); //param.Value1.Split(',');
					string[] selEndTalks = Utility.Split(param.Value2, ','); //param.Value2.Split(',');

					m_selectEndMoveIdx = int.Parse(param.Value3) - 1;

                    m_selectTalkCnt = selTalks.Length;

                    kStorySelectListObj.SetActive(true);

                    if (m_selectTalkItemList == null)
                        m_selectTalkItemList = new List<StorySelectItemValue>();

                    m_selectTalkItemList.Clear();

                    for (int i = 0; i < selTalks.Length; i++)
                    {
                        m_selectTalkItemList.Add(new StorySelectItemValue(int.Parse(selTalks[i]) - 1, int.Parse(selEndTalks[i]) - 1));
                    }

                    _selListInstance.UpdateList();

                    return;
                }
                break;
            case eScenarioType.CLOSE:
                {
                    if (activevn != null)
                        activevn.EndVisualNovel();
                    if (kTalkAniPlay.kIndex == 0)
                        kTalkAniPlay.Play(1);
                    kTalkAniPlay.gameObject.SetActive(false);
                    CharListRemove();
                    EffListRemove();
                    kCharBG.BlurOff();
                    if(AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
                    {
                        World.Instance.InGameCamera.DisableMotionBlur();
                        GameUIManager.Instance.ShowUI("GameOffPopup", false);
                        SetUIActive(false, false);
                        return;
                    }

                    //끝나고 애니메이션
                    SetUIActive(false, true);

                    return;
                }
                break;
			case eScenarioType.DELAY: {
				ftime = param.Time;
			}
			break;

            default:
                {
                    Log.Show(m_currentType.ToString() + " 해당하는 액션이 없습니다.", Log.ColorType.Red);
                }
                break;
        }

        if (_bSkip)
        {
            Invoke("SkipNext", 0.05f);
        }
        else
        {
            if (param.Next == (int)eNEXTTYPE.NEXT)
            {
                _storyTalkIndex += 1;
                Next();
            }
            else if (param.Next == (int)eNEXTTYPE.TIMENEXT)
            {
                _waitAuto = true;
                Invoke("AutoNext", ftime);
                return;
            }
            else if (param.Next == (int)eNEXTTYPE.WAIT)
            {
                if (_bAuto)
                {
                    Invoke("AutoNext", ftime);
                }
                else
                {
                    if (fwait == 0.0f)
                        ActiveNextBtn();
                    else
                        Invoke("ActiveNextBtn", ftime);
                }
            }
        }

        
    }

    public void AutoNext()
    {
        _waitAuto = false;
        OnClick_NextBtn();
    }

    public void ActiveNextBtn()
    {
        kNextBtn.gameObject.SetActive(true);
    }

    private void InitPos()
    {
        transform.localPosition = Vector3.zero;
    }
    
    /// <summary>
    ///  백버튼 처리
    /// </summary>
    /// <returns></returns>
    public override bool IsBackButton()
    {
        return false;
    }

    public override void OnClose()
    {
        base.OnClose();
        ScenarioMgr.Instance.OnPopupEnd();
    }

    /// 전체화면 흔들기
    public void ShakeAll(float duration = 1)
    {
        StartCoroutine(ShakeObject(this.gameObject, duration));
    }

    public void SkakeTalkObject(GameObject target, float duration, float amount = 1f)
    {
        StartCoroutine(ShakeObject(target, duration));
    }

    IEnumerator ShakeObject(GameObject target, float duration, float amount = 1f)
    {
        if (kPostProcessVolume.profile.HasSettings(typeof(Vignette)))
        {
            kPostProcessVolume.weight = 1f;
            Vignette vignette = kPostProcessVolume.profile.GetSetting<Vignette>();
            vignette.active = true;
        }
        

        m_shakeOriginPos = this.transform.localPosition;
        float timer = 0;
        while (timer <= duration)
        {
            target.transform.localPosition = (Vector3)UnityEngine.Random.insideUnitCircle * amount + m_shakeOriginPos;

            timer += Time.deltaTime;
            yield return null;
        }
        target.transform.localPosition = m_shakeOriginPos;
        if (kPostProcessVolume.profile.HasSettings(typeof(Vignette)))
        {
            kPostProcessVolume.weight = 0f;
            Vignette vignette = kPostProcessVolume.profile.GetSetting<Vignette>();
            vignette.active = false;
        }
        
    }

    void CharListRemove()
    {
        if (m_dicStoryUnits.Count > 0)
        {
            foreach (KeyValuePair<string, UIStoryCharUnit> pair in m_dicStoryUnits)
            {
                pair.Value.StopVoice();
                DestroyImmediate(pair.Value.gameObject);
            }

            m_dicStoryUnits.Clear();
        }
    }

    void CharListHide()
    {
        if (m_dicStoryUnits.Count > 0)
        {
            foreach (KeyValuePair<string, UIStoryCharUnit> pair in m_dicStoryUnits)
            {
                pair.Value.gameObject.SetActive(false);
            }

            m_dicStoryUnits.Clear();
        }
    }

    void EffListRemove()
    {
        if (m_dicEffects == null)
            return;
        if(m_dicEffects.Count > 0)
        {
            foreach(KeyValuePair<string, GameObject> pair in m_dicEffects)
            {
                pair.Value.SetActive(false);
                DestroyImmediate(pair.Value.gameObject);
            }

            m_dicEffects.Clear();
        }
    }

    void RePosition()
    {
        int frontIdx = 2;
        if (m_dicStoryUnits.Count > 0)
        {
            foreach (KeyValuePair<string, UIStoryCharUnit> pair in m_dicStoryUnits)
            {
                if(pair.Value.UnitState == UIStoryCharUnit.eStoryCharUnitState.Talk)
                {
                    pair.Value.SetZAxis((float)frontIdx);
                    frontIdx++;
                }
            }

            foreach (KeyValuePair<string, UIStoryCharUnit> pair in m_dicStoryUnits)
            {
                if (pair.Value.UnitState != UIStoryCharUnit.eStoryCharUnitState.Talk)
                {
                    pair.Value.SetZAxis((float)frontIdx);
                    frontIdx++;
                }
            }
        }
    }

    GameObject AddEffect(string effName, string bundlePath)
    {
        if (m_dicEffects == null)
            m_dicEffects = new Dictionary<string, GameObject>();

        if(!m_dicEffects.ContainsKey(effName))
        {
			string[] path = Utility.Split(bundlePath, '/'); //bundlePath.Split('/');
			GameObject obj = ResourceMgr.Instance.CreateFromAssetBundle(path[0].ToLower(), bundlePath + ".prefab");
            obj.SetActive(false);
            obj.transform.parent = this.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            obj.transform.SetChildLayer(5);
            //Utility.SetLayer(_leftThrower.gameObject, (int)eLayer.Default, true);
            m_dicEffects.Add(effName, obj);
        }

        if (m_dicEffects.ContainsKey(effName))
            return m_dicEffects[effName];
        else
            return null;
    }

    Vector3 GetPosition(string pos)
    {
		string[] vec = Utility.Split(pos, ','); //pos.Split(',');

		if (vec.Length == 3)
        {
            return new Vector3(Utility.SafeParse(vec[0]), Utility.SafeParse(vec[1]), Utility.SafeParse(vec[2]));
        }
        else
        {
            return Vector3.zero;
        }
    }

    Quaternion GetRotation(string rot)
    {
        if (string.IsNullOrEmpty(rot))
            return Quaternion.identity;

        rot = rot.Replace(" ", "");
		string[] vec = Utility.Split(rot, ','); //rot.Split(',');

		if (vec.Length == 3)
        {
            return Quaternion.Euler(Utility.SafeParse(vec[0]), Utility.SafeParse(vec[1]), Utility.SafeParse(vec[2]));
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public void SetSpringScroll(int number)
    {
        float itemHeight = _logListInstance.ItemSize.y * _logListInstance.ItemScale.y;
        float penddingY = _logListInstance.Padding.y;
        float sizeY = _logListInstance.Panel.GetViewSize().y;

        //  스크롤 스프링 컴포넌트
        SpringPanel spring = _logListInstance.GetComponentInChildren<SpringPanel>(true);
        if (spring == null)
            spring = _logListInstance.Panel.gameObject.AddComponent<SpringPanel>();

        number = _logIndex;
        //  가로 슬롯 넓이       //  펜딩 넓이                //  기본으로 보여주는 사이즈
        float y = ((itemHeight + penddingY) * number) - sizeY;

        if(y > 0)
        {
            spring.target = new Vector3(0, y, 0);

            spring.enabled = true;

        }
    }

    void PlayKeyboardSound(float ftime)
    {
        m_keyboardPlayer = StartCoroutine(PlayKeyboard(ftime));
    }

    IEnumerator PlayKeyboard(float ftime)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(kTalkTextLabel.kCharSpeed * 0.5f);
        while (kTalkTextLabel.TextScroll)
        {
            SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, m_keyboardNamelist[UnityEngine.Random.Range(0, m_keyboardSoundCount)], FSaveData.Instance.GetSEVolume());
            yield return waitForSeconds;
        }
    }

    void StopKeyboardSound()
    {
        if (m_keyboardPlayer != null)
            StopCoroutine(m_keyboardPlayer);
    }

    IEnumerator TweenPostProcessColorGrading(float ftime, int loopIdx)
    {
        ColorGrading colorSettings = kPostProcessVolume.profile.GetSetting<ColorGrading>();
        colorSettings.active = true;

        float onceTime = ftime / loopIdx;
        float pingpongTime = onceTime * 0.5f;
        float endtime = Time.time + pingpongTime;
        float clampValue = 0f;
        for(int i = 0; i < loopIdx; i++)
        {
            endtime = Time.time + pingpongTime;
            while (this.gameObject.activeSelf)
            {
                clampValue = Mathf.Clamp((endtime - Time.time) / pingpongTime, 0f, 1f);
                kPostProcessVolume.weight = 1f - clampValue;
                if (endtime < Time.time)
                {
                    break;
                }
                yield return null;
            }

            clampValue = 0f;
            endtime = Time.time + pingpongTime;
            while (this.gameObject.activeSelf)
            {
                clampValue = Mathf.Clamp((endtime - Time.time) / pingpongTime, 0f, 1f);
                kPostProcessVolume.weight = clampValue;
                if (endtime < Time.time)
                {
                    break;
                }
                yield return null;
            }
        }
        colorSettings.active = false;
        kPostProcessVolume.weight = 0f;
    }

    void SetText(FLabelTextShow fLabelTextShow, string text, bool banim)
    {
        float charSpeed = 0.05f;
        if (FSaveData.Instance.VLText.Equals(0))
            charSpeed = 0.15f;
        else if (FSaveData.Instance.VLText.Equals(1))
            charSpeed = 0.05f;
        else if (FSaveData.Instance.VLText.Equals(2))
            charSpeed = 0.01f;
        else if(FSaveData.Instance.VLText.Equals(3))
        {
            fLabelTextShow.SetText(text, false, true);
            return;
        }

        fLabelTextShow.kCharSpeed = charSpeed;
        fLabelTextShow.SetText(text, banim, true);
    }

    float GetAutoNarration()
    {
        if (FSaveData.Instance.VLAuto.Equals(0))
            return 3f;
        else if (FSaveData.Instance.VLAuto.Equals(1))
            return 2f;
        else if (FSaveData.Instance.VLAuto.Equals(2))
            return 1f;
        else if (FSaveData.Instance.VLAuto.Equals(3))
            return 0f;

        return 0f;
    }

    float GetAutoCharVoice()
    {
        if (FSaveData.Instance.VLAuto.Equals(0))
            return 1.5f;
        else if (FSaveData.Instance.VLAuto.Equals(1))
            return 1f;
        else if (FSaveData.Instance.VLAuto.Equals(2))
            return 0.5f;
        else if (FSaveData.Instance.VLAuto.Equals(3))
            return 0f;

        return 0f;
    }

    void GameOffPopupHide()
    {
        if (uIGameOffPopup != null && uIGameOffPopup.gameObject.activeSelf)
            uIGameOffPopup.SetUIActive(false, false);
    }

    public void OnClick_SelectSlot(StorySelectItemValue selectItemValue)
    {
        kStorySelectListObj.gameObject.SetActive(false);
        m_slotSelectItem = selectItemValue;
        _storyTalkIndex = m_slotSelectItem.kStartIdx;
        Next();
    }

    private void SetInGameCameraPos(string posValue)
    {
		string[] split = Utility.Split(posValue, ','); //posValue.Split(',');
		if (split.Length < 7)
        {
            Debug.LogError("인게임 카메라의 정보는 포지션x,포지션y,포지션z,회전x,회전y,회전z,FOV로 입력해주세요.");
        }
        else
        {
            Vector3 pos, rot;

            pos.x = Utility.SafeParse(split[0]);
            pos.y = Utility.SafeParse(split[1]);
            pos.z = Utility.SafeParse(split[2]);

            rot.x = Utility.SafeParse(split[3]);
            rot.y = Utility.SafeParse(split[4]);
            rot.z = Utility.SafeParse(split[5]);

            float fov = Utility.SafeParse(split[6]);

            World.Instance.InGameCamera.SetFixedMode(pos, rot, fov, true);
        }
    }

    /*
    void SEPlayCheck(bool b)
    {
        if (m_dicSoundEffects == null || m_dicSoundEffects.Count <= 0)
            return;
        foreach(KeyValuePair<string, AudioClip> pair in m_dicSoundEffects)
        {
            AudioSource ao = SoundManager.Instance.GetFxLoopSndSrc(pair.Key);
            if(ao != null)
            {
                if(ao.isPlaying)
                {
                    ao.volume = b ? FSaveData.Instance.GetSEVolume() * 0.3f : FSaveData.Instance.GetSEVolume();
                }
            }
        }
    }
    */
}
