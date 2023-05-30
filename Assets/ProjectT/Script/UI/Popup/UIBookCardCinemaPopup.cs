using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public enum eScenarioType
{
    SHOW,
    CLOSE,
    CHAR_LOAD,
    CHAR_FACE_LOAD,
    CHAR_DESTROY,
    CHAR_SET,
    CHAR_FADE,
    CHAR_STATE,
    CHAR_ROT,
    CHAR_SCALE,
    CHAR_WEAPON,
    CHAR_TALK,
    CHAR_TALK_S,
    CHAR_YAXIS,
    MOVE_TO,
    EYE_EFFECT,
    EFF_PLAY,
    EFF_STOP,
    BG_COLOR,
    BG_LOAD,
    BG_INGAME_CAMERA,
    BG_INGAME_CAMERA_CHANGE,
    BGM_PLAY,
    BGM_STOP,
    BLR_ON,
    BLR_OFF,
    BG_FADE_IN,
    BG_FADE_OUT,
    SE_PLAY,
    SE_STOP,
    CHAR_SHAKE,
    SHAKE_BG,
    TALK_IN,
    TALK_OUT,
    TEXT_CENTER,
    
    POST_COLOR,
    CHAR_SHAKE_ON,
    CHAR_SHAKE_OFF,
    SEL_TALK,
    BLR_TEX_ON,
    BLR_TEX_OFF,
    MOVE_NONE_Z,
	DELAY,

    NONE,
}

public class StorySelectItemValue
{
    public int kStartIdx = 0;
    public int kEndIdx = 0;

    public StorySelectItemValue(int startIdx, int endIdx)
    {
        kStartIdx = startIdx;
        kEndIdx = endIdx;
    }
}

public class UIBookCardCinemaPopup : FComponent
{
    public enum eNEXTTYPE
    {
        NEXT = 0,
        TIMENEXT,
        WAIT,
    }


    [SerializeField] private FList _FavorabilityLogListInstance;
    [SerializeField] private FList _selListInstance;
    public UIStoryBGUnit kStoryBGUnit;
    public FToggle kAutoToggle;
    public FToggle kSkipToggle;
	public GameObject kBtnMenu;

    public GameObject kTalkNameRoot;
    public UISprite kTalkBGSprite;

    public FLabelTextShow kCharNameLabel;
	public FLabelTextShow kTalkTextLabel;
    public FLabelTextShow kCenterTextLabel;
    public UIButton kViewCinemaBtn;
    public UIButton kBackBtn;
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
    private string _spriteName = string.Empty;
    private string _bgName = string.Empty;
    private List<ScenarioParam> _scenarioList = new List<ScenarioParam>();
    private List<ScenarioParam> _logScenarioList = new List<ScenarioParam>();
    //Shake
    private Vector3 m_shakeOriginPos = Vector3.zero;
    private bool m_bIsShakeing = false;
    private Dictionary<string, UIBookCardCinemaUnit> m_dicBookCardUnits = new Dictionary<string, UIBookCardCinemaUnit>();
    private Dictionary<string, GameObject> m_dicEffects;
    private Dictionary<string, AudioClip> m_dicSoundEffects;
    private AudioClip m_prevBGM = null;

    private const int m_keyboardSoundCount = 3;
    private List<string> m_keyboardNamelist;
    private Coroutine m_keyboardPlayer = null;

    private eScenarioType m_currentType = eScenarioType.NONE;

    //BGM & SE Audios
    private AudioSource m_bgmAudioSource = null;
    private List<AudioSource> m_seAudioSources;

    private Coroutine m_bgmFadeInCoroutine = null;
    private Coroutine m_bgmFadeOutCoroutine = null;

    //StoryWord
    private List<int> m_curStoryWordIdx = new List<int>();

    //UI HIDE
    private bool m_uiHideFlag = false;

    private UIGameOffPopup uIGameOffPopup;

    //SelectTalk Values
    private int m_selectTalkCnt = 0;
    private int m_selectEndMoveIdx = 0;
    private StorySelectItemValue m_slotSelectItem = null;
    private List<StorySelectItemValue> m_selectTalkItemList;


    private bool _bSkip = false;


    public override void Awake()
	{
		base.Awake();

        if (AppMgr.Instance.WideScreen)
        {
            kBackBtn.transform.localPosition = new Vector3(kBackBtn.transform.localPosition.x + 110f, kBackBtn.transform.localPosition.y, kBackBtn.transform.localPosition.z);
            kViewCinemaBtn.transform.localPosition = new Vector3(kViewCinemaBtn.transform.localPosition.x - 110f, kViewCinemaBtn.transform.localPosition.y, kViewCinemaBtn.transform.localPosition.z);
            kLogCloseBtn.transform.localPosition = new Vector3(kLogCloseBtn.transform.localPosition.x - 70f, kLogCloseBtn.transform.localPosition.y, kLogCloseBtn.transform.localPosition.z);
        }
        kAutoToggle.EventCallBack = OnAutoToggleSelect;
        kSkipToggle.EventCallBack = OnSkipToggleSelect;
        this._FavorabilityLogListInstance.EventUpdate = this._UpdateFavorabilityLogListSlot;
        this._FavorabilityLogListInstance.EventGetItemCount = this._GetFavorabilityLogElementCount;

        this._selListInstance.EventUpdate = this._UpdateSelectListSlot;
        this._selListInstance.EventGetItemCount = this._GetSelectListElementCount;

    }

    public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		kCharNameLabel.ResetText();
		kTalkTextLabel.ResetText();

		m_slotSelectItem = null;
        _bStoryStart = false;

        _logScenarioList.Clear();

        //  저장된 카드 ID 초기화
        UIValue.Instance.SetValue(UIValue.EParamType.CardTableID, 0);
        StopAllCoroutines();
        if (m_dicBookCardUnits.Count > 0)
        {
            foreach (KeyValuePair<string, UIBookCardCinemaUnit> pair in m_dicBookCardUnits)
            {
                DestroyImmediate(pair.Value.gameObject);
            }

        }
        if(m_dicBookCardUnits != null)
            m_dicBookCardUnits.Clear();
        if(m_dicSoundEffects != null)
            m_dicSoundEffects.Clear();
        if(m_dicEffects != null)
        {
            foreach(KeyValuePair<string, GameObject> effobj in m_dicEffects)
            {
                DestroyImmediate(effobj.Value);
            }
            m_dicEffects.Clear();
        }
        if (m_keyboardNamelist != null)
            m_keyboardNamelist.Clear();
        if (m_curStoryWordIdx != null)
            m_curStoryWordIdx.Clear();
        if (m_seAudioSources != null)
            m_seAudioSources.Clear();
        kStoryBGUnit.Init();

		if( m_prevBGM ) {
			if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
				SoundManager.Instance.PlayBgm( m_prevBGM, FSaveData.Instance.GetMasterVolume() );
			}
			else {
				World.Instance.PlayBgm();
			}

			m_prevBGM = null;
		}

		if (World.Instance.InGameCamera != null)
        {
            World.Instance.InGameCamera.RestoreCullingMask();
        }

        base.OnDisable();

        //  인보크 취소
        CancelInvoke("AutoNext");

#if !DISABLESTEAMWORKS
        if(AppMgr.Instance.SceneType != AppMgr.eSceneType.Lobby)
            AppMgr.Instance.CustomInput.ShowCursor(false);
#endif
    }

    public override void OnClose()
    {
        base.OnClose();

        ScenarioMgr.Instance.OnPopupEnd();
        //if (AppMgr.Instance.SceneType != AppMgr.eSceneType.Lobby)
            
    }

    void Update()
    {
        if (!m_uiHideFlag)
            return;

        if (m_uiHideFlag && AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Select))//Input.GetMouseButtonDown(0))
        {
            m_uiHideFlag = false;
            //ActiveNextBtn();
            kTalkAniPlay.Play("StroyTalk_In");

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
        kStoryword.SetActive(false);
        m_storywordTween = kStoryword.GetComponent<TweenScale>();

        SetAudioSources();
        _scenarioGroupID = (int)UIValue.Instance.GetValue(UIValue.EParamType.ScenarioGroupID, true);

        _bgName = string.Empty;
        var bgname = UIValue.Instance.GetValue(UIValue.EParamType.ScenarioFavorBGStr, true);
        if (bgname != null)
            _bgName = (string)bgname;

        _spriteName = string.Empty;
        var spritename = UIValue.Instance.GetValue(UIValue.EParamType.ScenarioFavorBGSprite, true);
        if (spritename != null)
            _spriteName = (string)spritename;

        ScenarioMgr.Instance.InitScenarioScene(_scenarioGroupID);

        _storyTalkIndex = 1;
        _logIndex = 0;
        _bAuto = false;
        _scenarioList = ScenarioMgr.Instance.Scenarios.FindAll(a => a.Group == _scenarioGroupID && a.Type == "CHAR_TALK");
        //_scenarioList = ScenarioMgr.Instance.Scenarios.FindAll(a => a.Group == _scenarioGroupID);

        //  배경 텍스쳐 셋팅
        kStoryBGUnit.Init();
        if(!string.IsNullOrEmpty(_spriteName))
            kStoryBGUnit.SetSprite("favor", "Favor/" + _spriteName, _bgName);

        m_keyboardNamelist = new List<string>();
        for (int i = 0; i < m_keyboardSoundCount; i++)
        {
            string fileName = "snd_story_keyboard_eff_0" + (i + 1);
            string soundPath = "Sound/Fx/Story/" + fileName + ".ogg";
            m_keyboardNamelist.Add(fileName);
            SoundManager.Instance.AddAudioClip(fileName, soundPath, FSaveData.Instance.GetSEVolume());
        }

        if(AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
        {
            var flag = UIValue.Instance.GetValue(UIValue.EParamType.SkipCinemaPictureMode, true);
            if(flag != null)
            {
                kBackBtn.gameObject.SetActive(false);
                SetStoryTalkMode();
            }
            else
            {
                kBackBtn.gameObject.SetActive(true);
                //  그림 모드
                SetPictureMode();

                //처음 호감도 이미지 볼때 로그 남기기
                Firebase.Analytics.FirebaseAnalytics.LogEvent("Favor_Img", "Favor_ID", _spriteName);
            }
            
        }
        else
        {
            kBackBtn.gameObject.SetActive(false);
            //로비씬이 아닌 곳에서 시나리오 실행 시 그림모드 스킵하고 바로 실행
            if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
            {
                uIGameOffPopup = GameUIManager.Instance.GetUI<UIGameOffPopup>("GameOffPopup");
                GameUIManager.Instance.ShowUI("GameOffPopup", false);
            }

            SetStoryTalkMode();
        }

#if !DISABLESTEAMWORKS
        if (AppMgr.Instance.SceneType != AppMgr.eSceneType.Lobby)
            AppMgr.Instance.CustomInput.ShowCursor(true);
#endif
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
    }
 

	
	private bool OnAutoToggleSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
            return true;
        _bAuto = System.Convert.ToBoolean(nSelect);

        if (_bSkip && _bAuto)
            kSkipToggle.SetToggle((int)eCOUNT.NONE, SelectEvent.Code);

        if (type == SelectEvent.Code)
        {
            return true;
        }

        //if(!_firstFrame)
        //{
        //    _firstFrame = true;
        //    return true;
        //}
        OnClick_NextBtn();
        if (!_bAuto)
            ActiveNextBtn();
        return true;
	}

    private bool OnSkipToggleSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
            return true;

        _bSkip = System.Convert.ToBoolean(nSelect);

        if (_bAuto && _bSkip)
            kAutoToggle.SetToggle((int)eCOUNT.NONE, SelectEvent.Code);

        if (type == SelectEvent.Code)
        {

            return true;
        }

        if (_bSkip)
        {
            StopKeyboardSound();
        }

        SkipNext();
        ActiveNextBtn();
        
        Log.Show("Skip Flag : " + _bSkip);

        return true;
    }


    private void _UpdateFavorabilityLogListSlot(int index, GameObject slotObject)
	{
		do
        {
            UIStoryTalkLogSlot talkLogSlot = slotObject.GetComponent<UIStoryTalkLogSlot>();
            if (talkLogSlot == null)
                return;

            talkLogSlot.ParentGO = this.gameObject;

            //  현재까지 등장한 idx보다 슬롯 idx 작은 경우만 활성화
            if (index < _logScenarioList.Count && _logScenarioList[index].Type == "CHAR_TALK" )
            {
                talkLogSlot.UpdateSlot(_logScenarioList[index]);
                talkLogSlot.gameObject.SetActive(true);
            }
            else
            {
                talkLogSlot.gameObject.SetActive(false);
            }
            //Do UpdateListSlot
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

    /// <summary>
    ///  시네마 스토리 진행 버튼
    /// </summary>
    public void OnClick_ViewCinemaBtn()
    {
        //Debug.LogError("OnClick_ViewCinemaBtn");
        SetStoryTalkMode();
    }
	
    /// <summary>
    ///  back버튼
    /// </summary>
	public void OnClick_BackBtn()
	{
        OnClickClose();
	}
       
    /// <summary>
    ///  로그 보기 버튼
    /// </summary>
    public void OnClick_LogBtn()
    {
        if (_logScenarioList == null)
            return;
        if (_logScenarioList.Count <= 0)
            return;
        _bRememberValue = _bAuto;
        _bAuto = false;
        CancelInvoke("AutoNext");

        _FavorabilityLogListInstance.gameObject.SetActive(true);
        kLogCloseBtn.gameObject.SetActive(true);
        _FavorabilityLogListInstance.UpdateList();

        if (_FavorabilityLogListInstance.IsScroll)
        {
            _FavorabilityLogListInstance.SpringSetFocus(_logScenarioList.Count - 3);
        }
    }

    /// <summary>
    ///  스킵 버튼
    /// </summary>
    public void OnClick_SkipBtn()
    {
        
        OnClickClose();
    }

    public void OnClick_ExitBtn()
    {
        

        OnClickClose();
    }

    public void OnClick_HideUIBtn()
    {
        if (_FavorabilityLogListInstance.gameObject.activeSelf)
            return;
        //OnAutoToggleSelect(0, SelectEvent.Click);

        _waitAuto = false;
        kNextBtn.enabled = false;
        //kNextBtn.gameObject.SetActive(false);
        m_uiHideFlag = true;

        _bRememberValue = _bAuto;
        _bAuto = false;
        CancelInvoke("AutoNext");

        kTalkAniPlay.Play("StroyTalk_Out");
    }

    public void OnClick_StorywordBtn()
    {
        Log.Show("OnClick_StorywordBtn");
        _bRememberValue = _bAuto;
        _bAuto = false;
        CancelInvoke("AutoNext");
        MessagePopup.TextLong(FLocalizeString.Instance.GetText(1368), GetStoryTream(), true,  OnStorywordClose);
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

        for (int i = 0; i < m_curStoryWordIdx.Count; i++)
        {
            TremParam param = ScenarioMgr.Instance.GetScenarioTremParam(m_curStoryWordIdx[i]);
            str.Append(string.Format("{0}\n\n", param.Title));
            str.Append(param.Desc);
            str.Append("\n\n\n");
        }

        return str.ToString();
    }

    ///// <summary>
    /////  오토 진행 버튼
    ///// </summary>
    //public void OnClick_AutoBtn()
    //{
    //    _bAuto = !_bAuto;
    //    OnClick_NextBtn();
    //}

    /// <summary>
    ///  로그 보기 창 닫기
    /// </summary>
    public void OnClick_HideLog()
    {
        _waitAuto = false;
        _bAuto = _bRememberValue;
        if (_bAuto == true)
            Invoke("AutoNext", TakeWaitTime);

        _FavorabilityLogListInstance.gameObject.SetActive(false);
        kLogCloseBtn.gameObject.SetActive(false);
        ActiveNextBtn();
    }

    /// <summary>
    ///  풀스크린 버튼
    /// </summary>
    public void OnClick_NextBtn()
    {
        if (_bStoryStart == true)
        {
            if (_waitAuto)
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
                    if (param.Voice != "" && param.Voice != string.Empty)
                        SoundManager.Instance.StopVoice();
                }
            }

            _storyTalkIndex += 1;
            Next();

            //else if (param.Type == "TALK_OUT")
            //{
            //    _storyTalkIndex += 1;
            //    Next();
            //}
            //else
            //{
                
            //}
        }
        else
        {
            //  대사 전의 next버튼
            bool isCinemaActive = kViewCinemaBtn.gameObject.activeSelf;
            kViewCinemaBtn.gameObject.SetActive(!isCinemaActive);

            bool isBackActive = kBackBtn.gameObject.activeSelf;
            kBackBtn.gameObject.SetActive(!isBackActive);
        }
    }

    private void SkipNext()
    {
        var param = ScenarioMgr.Instance.GetScenarioParam(_storyTalkIndex);
        if (param == null)
            return;

        if (kStoryword.activeSelf)
            kStoryword.SetActive(false);
        if (param.Voice != "" && param.Voice != string.Empty)
            SoundManager.Instance.StopVoice();

        _storyTalkIndex += 1;
        Next();
    }

    /// <summary>
    ///  그림 모드
    /// </summary>
    private void SetPictureMode()
    {
        kViewCinemaBtn.gameObject.SetActive(true);
        kBackBtn.gameObject.SetActive(true);

        kTalkAniPlay.gameObject.SetActive(false);
        _FavorabilityLogListInstance.gameObject.SetActive(false);

        _bStoryStart = false;
    }

    /// <summary>
    ///  대화 진행 모드
    /// </summary>
    private void SetStoryTalkMode()
    {
        m_prevBGM = SoundManager.Instance.GetBgm();
        SoundManager.Instance.StopBgm();

        _bStoryStart = true;
        _firstFrame = false;
        Animation talkAni = kTalkAniPlay.GetComponent<Animation>();
        talkAni.enabled = false;
        talkAni.enabled = true;

        //kTalkAniPlay.gameObject.SetActive(true);

        //  토글 버튼 셋팅
        kAutoToggle.SetToggle(0, SelectEvent.Enable);
        kSkipToggle.SetToggle(0, SelectEvent.Code);
        //Next();

        kViewCinemaBtn.gameObject.SetActive(false);
        kBackBtn.gameObject.SetActive(false);

        //if (kAutoToggle.EventCallBack == null)
            


        Next();

        //대화 모드 들어갈때 로그 남기기.
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Favor_Story", "Favor_ID", _spriteName);

    }

    /// <summary>
    ///  대화창 메뉴
    /// </summary>
    /// <param name="isOpen"></param>
    private void ShowMenu(bool isOpen)
    {
        if (kBtnMenu != null)
        {
            kBtnMenu.SetActive(isOpen);
        }
    }
    
    public void Next()
    {
        if(m_slotSelectItem != null)
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
        Log.Show(_storyTalkIndex, Log.ColorType.Red);
        //kNextBtn.gameObject.SetActive(false);
        kNextBtn.enabled = false;
        float fwait = 0.0f;
        float ftimescale = 1.0f;
        //if (_bAuto)
        //    ftimescale = 1.5f;  //  추후 타임 스케일 값을 대입

        float ftime = param.Time * ftimescale;

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
            case eScenarioType.CHAR_FACE_LOAD:
                {
                    UIBookCardCinemaUnit.eBookUnitTex target = (param.Type.Equals("CHAR_FACE_LOAD") ? UIBookCardCinemaUnit.eBookUnitTex.FACE : UIBookCardCinemaUnit.eBookUnitTex.BODY);

                    if (!m_dicBookCardUnits.ContainsKey(param.Value1))
                    {
                        if (string.IsNullOrEmpty(param.Voice))
                            m_dicBookCardUnits.Add(param.Value1, BookCardUnit.CreateBookCardUnit(param.Value2, param.Value2, this.transform));
                        else
                        {
                            m_dicBookCardUnits.Add(param.Value1, BookCardUnit.CreateBookCardUnitWithSprite(param.Value2, param.Value2, param.Voice, this.transform));
                        }
                    }
                    UIBookCardCinemaUnit targetUnit = null;

                    targetUnit = m_dicBookCardUnits[param.Value1];
                    targetUnit.SetUnitPos(param.Pos);

                    if (!string.IsNullOrEmpty(param.Value3))
                    {
						string[] pos = Utility.Split(param.Value3, ','); //param.Value3.Split(',');
                        targetUnit.SetPosistion(param.Voice, new Vector3(Utility.SafeParse(pos[0]), Utility.SafeParse(pos[1]), Utility.SafeParse(pos[2])), target);
                    }
                    if (!string.IsNullOrEmpty(param.Value4))
                    {
                        if (param.Value4.ToLower().Equals("in"))
                        {
                            targetUnit.SetBookUnitTexture(param.Value2, false, target);
                            targetUnit.FadeIn(param.Voice, _bSkip ? 0f : param.Time);
                        }
                        else if (param.Value4.ToLower().Equals("out"))
                        {
                            targetUnit.SetBookUnitTexture(param.Value2, false, target);
                            targetUnit.FadeOut(param.Voice, _bSkip ? 0f : param.Time);
                        }
                        else if (param.Value4.ToLower().Equals("cross"))
                        {
                            targetUnit.SetBookUnitTexture(param.Value2, _bSkip ? false : true, target);
                        }
                    }
                }
                break;
            case eScenarioType.CHAR_SET:
                {
                    if (!string.IsNullOrEmpty(param.Value5) && !param.Value5.Equals("NONE"))
                    {
                        if (param.Value5.Equals("LEFT"))
                        {
                            m_dicBookCardUnits[param.Value1].SetUnitPos(-5);
                            m_dicBookCardUnits[param.Value1].MoveTo(param.Pos, _bSkip ? 0f : param.Time);
                        }
                        else if (param.Value5.Equals("RIGHT"))
                        {
                            m_dicBookCardUnits[param.Value1].SetUnitPos(5);
                            m_dicBookCardUnits[param.Value1].MoveTo(param.Pos, _bSkip ? 0f : param.Time);
                        }
                        else if (param.Value5.Equals("BLANK"))
                        {
                            m_dicBookCardUnits[param.Value1].SetUnitPos(param.Pos);
                        }
                    }

                    fwait = ftime;

                }
                break;
            case eScenarioType.CHAR_FADE:
                {
                    if (param.Value4.Equals("FADE_IN"))
                    {
                        m_dicBookCardUnits[param.Value1].SetUnitPos(param.Pos);
                        m_dicBookCardUnits[param.Value1].FadeIn(_bSkip ? 0f : ftime);
                    }
                    else if (param.Value4.Equals("FADE_OUT"))
                    {
                        m_dicBookCardUnits[param.Value1].FadeOut(_bSkip ? 0f : ftime);
                    }
                    param.Next = (int)eNEXTTYPE.TIMENEXT;
                    fwait = ftime;
                }
                break;
            case eScenarioType.MOVE_TO:
                {
                    if (m_dicBookCardUnits.ContainsKey(param.Value1))
                    {
                        m_dicBookCardUnits[param.Value1].MoveTo(param.Pos, _bSkip ? 0f : param.Time);
                    }
                }
                break;
            case eScenarioType.BG_LOAD:
                {
                    if (param.Next == (int)eNEXTTYPE.TIMENEXT)
                    {
                        if (kTalkAniPlay.kIndex == 0)
                            kTalkAniPlay.Play("StroyTalk_Out");
                    }
                    fwait = ftime;
                    
                    if(null != uIGameOffPopup && uIGameOffPopup.gameObject.activeSelf)
                    {
                        CancelInvoke("GameOffPopupHide");
                        Invoke("GameOffPopupHide", param.Time + (param.Time * 0.2f));
                    }


                    if (string.IsNullOrEmpty(param.Value2))
                    {
                        Debug.Log("이미지 경로가 존재 하지않습니다.");
                        if (param.Value1.Equals("FADE_OUT"))
                        {
                            kStoryBGUnit.FadeOut(_bSkip ? 0f : ftime);
                        }
                    }
                    else
                    {
						string[] path = Utility.Split(param.Value2, '/'); //param.Value2.Split('/');
						if (path.Length >= 2)
                        {
                            if (param.Value1.Equals("ADD"))
                            {
                                if (param.Value4.ToLower().Equals("in"))
                                {
                                    if (!string.IsNullOrEmpty(param.Value3))
                                    {
										string[] texPos = Utility.Split(param.Value3, ','); //param.Value3.Split(',');
										Vector2 pos = new Vector3(Utility.SafeParse(texPos[0]), Utility.SafeParse(texPos[1]), Utility.SafeParse(texPos[2]));
                                        if (string.IsNullOrEmpty(param.Voice))
                                            kStoryBGUnit.FadeInWithAddTex(path[0].ToLower(), param.Value2, _bSkip ? 0f : ftime, pos);
                                        else
                                            kStoryBGUnit.FadeInWithAddSprite(path[0].ToLower(), param.Value2, param.Voice, _bSkip ? 0f : ftime, pos);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(param.Voice))
                                            kStoryBGUnit.FadeInWithAddTex(path[0].ToLower(), param.Value2, _bSkip ? 0f : ftime, Vector3.zero);
                                        else
                                            kStoryBGUnit.FadeInWithAddSprite(path[0].ToLower(), param.Value2, param.Voice, _bSkip ? 0f : ftime, Vector3.zero);
                                    }
                                }
                                else if (param.Value4.ToLower().Equals("out"))
                                {
                                    kStoryBGUnit.FadeOutWithAddTex(_bSkip ? 0f : ftime);
                                    kStoryBGUnit.FadeOutWithAddTexSprite(_bSkip ? 0f : ftime);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(param.Value3))
                                    {
										string[] texPos = Utility.Split(param.Value3, ','); //param.Value3.Split(',');
										Vector2 pos = new Vector3(Utility.SafeParse(texPos[0]), Utility.SafeParse(texPos[1]), Utility.SafeParse(texPos[2]));
                                        if (string.IsNullOrEmpty(param.Voice))
                                            kStoryBGUnit.SetTextureWithAddTex(path[0].ToLower(), param.Value2, pos);
                                        else
                                            kStoryBGUnit.SetSpriteWithAddSprite(path[0].ToLower(), param.Value2, param.Voice, pos);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(param.Voice))
                                            kStoryBGUnit.SetTextureWithAddTex(path[0].ToLower(), param.Value2, Vector3.zero);
                                        else
                                            kStoryBGUnit.SetSpriteWithAddSprite(path[0].ToLower(), param.Value2, param.Voice, Vector3.zero);
                                    }
                                }
                            }
                            else if (param.Value1.Equals("CHANGE"))
                            {
                                if (!string.IsNullOrEmpty(param.Value4) && param.Value4.Equals("CROSS"))
                                {
                                    if (string.IsNullOrEmpty(param.Voice))
                                        kStoryBGUnit.ChangeBG(path[0].ToLower(), param.Value2, _bSkip ? 0f : ftime);
                                    else
                                        kStoryBGUnit.ChangeBGSprite(path[0].ToLower(), param.Value2, param.Voice, _bSkip ? 0f : ftime);
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(param.Voice))
                                        kStoryBGUnit.SetTexture(path[0].ToLower(), param.Value2);
                                    else
                                        kStoryBGUnit.SetSprite(path[0].ToLower(), param.Value2, param.Voice);
                                }
                            }
                            else if (param.Value1.Equals("FADE_IN"))
                            {
                                if (string.IsNullOrEmpty(param.Voice))
                                {
                                    Log.Show("BG_FADE_IN", Log.ColorType.Blue);
                                    kStoryBGUnit.FadeIn(path[0].ToLower(), param.Value2, _bSkip ? 0f : ftime);
                                }
                                else
                                    kStoryBGUnit.FadeInSprite(path[0].ToLower(), param.Value2, param.Voice, _bSkip ? 0f : ftime);
                            }
                            else if (param.Value1.Equals("FADE_OUT"))
                            {
                                kStoryBGUnit.FadeOut(_bSkip ? 0f : ftime);
                                kStoryBGUnit.FadeOutSprite(_bSkip ? 0f : ftime);
                            }
                            else if (param.Value1.Equals("ROTATE"))
                            {
                                if (param.Value4.Equals("FORWARD"))      //시계방향 회전
                                {
                                    //kStoryBGUnit.RotateChangeBG(path[0].ToLower(), param.Value2, param.Voice, Color.white, true, _bSkip ? 0f : (param.Time >= 1f) ? param.Time : 2f);
                                    kStoryBGUnit.RotateChangeBG(path[0].ToLower(), param.Value2, param.Voice, param.Value5, true, _bSkip ? 0f : (param.Time >= 1f) ? param.Time : 2f);
                                }
                                else if (param.Value4.Equals("REVERSE"))     //시계 반대방향 회전
                                {
                                    //kStoryBGUnit.RotateChangeBG(path[0].ToLower(), param.Value2, param.Voice, Color.white, false, _bSkip ? 0f : (param.Time >= 1f) ? param.Time : 2f);
                                    kStoryBGUnit.RotateChangeBG(path[0].ToLower(), param.Value2, param.Voice, param.Value5, false, _bSkip ? 0f : (param.Time >= 1f) ? param.Time : 2f);
                                }
                            }
                            else if (param.Value1.Equals("MOVE"))
                            {
                                kStoryBGUnit.MoveChangeBG(param.Value4, param.Value5, path[0].ToLower(), param.Value2, param.Voice, _bSkip ? 0f : (param.Time >= 1f) ? param.Time : 2f);
                            }
                            else if (param.Value1.Equals("COLOR"))
                            {
                                if (string.IsNullOrEmpty(param.Value4))
                                {
                                    Debug.LogError("배경 색상이 빈 칸입니다.");
                                }
                                else
                                {
									string[] colors = Utility.Split(param.Value4, ','); //param.Value4.Split(',');
									if (colors.Length < 4)
                                        Debug.LogError("색상 값이 잘못 들어갔습니다.");
                                    else
                                    {
                                        Color color = new Color(Utility.SafeParse(colors[0]), Utility.SafeParse(colors[1]), Utility.SafeParse(colors[2]), 1f);
                                        kStoryBGUnit.ColorOutChangeBG(path[0].ToLower(), param.Value2, param.Voice, color, _bSkip ? 0f : (param.Time >= 1f) ? param.Time : 2f);
                                    }
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(param.Voice))
                                    kStoryBGUnit.SetTexture(path[0].ToLower(), param.Value2, true);
                                else
                                    kStoryBGUnit.SetSprite(path[0].ToLower(), param.Value2, param.Voice, true);
                            }
                        }
                        else
                        {
                            Debug.LogError(param.Value2 + " / 잘못된 경로 입니다.");
                        }
                    }
                }
                break;
            case eScenarioType.EYE_EFFECT:
                {
                    //if (kTalkAniPlay.kIndex == 0)
                    //    kTalkAniPlay.Play("StroyTalk_Out");
                    //param.Next = (int)eNEXTTYPE.TIMENEXT;
                    //ftime = 2f;
                    //fwait = ftime;
                    //string[] path = param.Value2.Split('/');
                    //if (param.Value1.Equals("OPEN"))
                    //{
                    //    kStoryBGUnit.EyeChangeBG(path[0].ToLower(), param.Value2, param.Voice, true, ftime);
                    //}
                    //else if (param.Value1.Equals("CLOSE"))
                    //{
                    //    kStoryBGUnit.EyeChangeBG(path[0].ToLower(), param.Value2, param.Voice, false, ftime);
                    //}
                    //else if (param.Value1.Equals("ONCE"))
                    //{
                    //    kStoryBGUnit.EyeChangeBG(path[0].ToLower(), param.Value2, param.Voice, ftime);
                    //    fwait = ftime = ftime + (ftime * 2);
                    //}
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

                    effObj.SetActive(true);
                }
                break;
            case eScenarioType.EFF_STOP:
                {
                    if (_bSkip)
                        break;

                    if (null != m_dicEffects && m_dicEffects.ContainsKey(param.Value1))
                    {
                        m_dicEffects[param.Value1].SetActive(false);
                    }
                }
                break;
            case eScenarioType.BG_COLOR:
                {
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
                                kStoryBGUnit.SetBGColor(color, 0f);
                            else if (param.Value4.Equals("FADE"))
                                kStoryBGUnit.SetBGColor(color, _bSkip ? 0f : param.Time);
                        }
                    }
                }
                break;
            case eScenarioType.BGM_PLAY:
                {
                    if (m_bgmFadeInCoroutine != null)
                        StopCoroutine(m_bgmFadeInCoroutine);
                    if (m_bgmFadeOutCoroutine != null)
                        StopCoroutine(m_bgmFadeOutCoroutine);

                    if (param.Value1.Equals("PLAY"))
                    {
                        AudioClip ac = ResourceMgr.Instance.LoadFromAssetBundle("sound", param.Value2) as AudioClip;
                        PlayBGM(ac, param.Pos, false);
                    }
                    else if (param.Value1.Equals("FADE_IN"))
                    {
                        AudioClip ac = ResourceMgr.Instance.LoadFromAssetBundle("sound", param.Value2) as AudioClip;
                        PlayBGM(ac, param.Pos, _bSkip ? false : true);
                    }
                    else if (param.Value1.Equals("FADE_OUT"))
                    {
                        StopBGM(param.Pos, _bSkip ? false : true);
                    }
                }
                break;
            case eScenarioType.BGM_STOP:
                {
                    if (param.Value1.Equals("FADE_OUT"))
                    {
                        StopBGM(param.Pos, _bSkip ? false : true);
                    }
                    else
                    {
                        StopBGM(param.Pos, _bSkip ? false : false);
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

                            if (param.Value4.Equals("FADE_OUT"))
                            {
                                StopSE(ac, param.Pos, true);
                            }
                            else if (param.Value4.Equals("FADE_IN"))
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
                        if (!m_dicSoundEffects.ContainsKey(param.Value1))
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
                    if (m_dicBookCardUnits.ContainsKey(param.Value1))
                    {
                        SkakeTalkObject(m_dicBookCardUnits[param.Value1].gameObject, _bSkip ? 0f : param.Time);
                    }
                }
                break;
            case eScenarioType.CHAR_SHAKE_ON:
                {
                    if(m_dicBookCardUnits.ContainsKey(param.Value1))
                    {
                        m_dicBookCardUnits[param.Value1].PlayShakeCard(param.Pos);
                    }
                    fwait = ftime;

                    if (_bSkip)
                        fwait = 0.1f;
                }
                break;
            case eScenarioType.CHAR_SHAKE_OFF:
                {
                    if(m_dicBookCardUnits.ContainsKey(param.Value1))
                    {
                        m_dicBookCardUnits[param.Value1].StopShakeCard();
                    }
                    fwait = ftime;
                    if (_bSkip)
                        fwait = 0.1f;
                }
                break;
            case eScenarioType.SHAKE_BG:
                {
                    if (param.Value1.Equals("BG"))
                    {
                        SkakeTalkObject(kStoryBGUnit.gameObject, _bSkip ? 0f : param.Time);
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
                        m_dicBookCardUnits[talks[i]].Talk(param.Voice, _bSkip ? 0f : param.Time);
                    }

                    if (param.Value2.Equals("ALL"))
                    {
                        foreach (KeyValuePair<string, UIBookCardCinemaUnit> pair in m_dicBookCardUnits)
                        {
                            bool checker = false;
                            for (int i = 0; i < talks.Length; i++)
                            {
                                if (talks[i].Equals(pair.Key))
                                    checker = true;
                            }
                            if (!checker)
                                pair.Value.Listen(param.Voice, _bSkip ? 0f : param.Time);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < listen.Length; i++)
                        {
                            m_dicBookCardUnits[listen[i]].Listen(param.Voice, _bSkip ? 0f : param.Time);
                        }
                    }

                    if (param.Value3.Equals("ALL"))
                    {
                        foreach (KeyValuePair<string, UIBookCardCinemaUnit> pair in m_dicBookCardUnits)
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
                                pair.Value.Stand(param.Voice, _bSkip ? 0f : param.Time);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < stand.Length; i++)
                        {
                            m_dicBookCardUnits[stand[i]].Stand(param.Voice, _bSkip ? 0f : param.Time);
                        }
                    }
                }
                break;
            case eScenarioType.CHAR_SCALE:
                {
                    Log.Show("CHAR_SCALE :" + param.Pos);
                    m_dicBookCardUnits[param.Value1].SetCharLocalScale(param.Pos);
                }
                break;
            case eScenarioType.TALK_IN:
                {
                    kTalkAniPlay.Play("StroyTalk_In");
                    fwait = ftime;
                }
                break;
            case eScenarioType.TALK_OUT:
                {
                    kTalkAniPlay.Play("StroyTalk_Out");
                    fwait = ftime;

                    if(AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
                        Invoke("OnClickClose", EndCloseTime);
                }
                break;
            case eScenarioType.CHAR_TALK:
                {

                    _logScenarioList.Add(param);

                    if (!kTalkAniPlay.gameObject.activeSelf)
                    {
                        _bAuto = false;
                        kTalkAniPlay.gameObject.SetActive(true);
                        kTalkAniPlay.Play("StroyTalk_In");
                    }

                    if (!string.IsNullOrEmpty(param.Value1) && param.Value1 != "")
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

                    if (!string.IsNullOrEmpty(param.TremIndex))
                    {
                        m_curStoryWordIdx.Clear();
						string[] words = Utility.Split(param.TremIndex, ','); //param.TremIndex.Split(',');
						for (int i = 0; i < words.Length; i++)
                            m_curStoryWordIdx.Add(int.Parse(words[i]));

                        kStoryword.SetActive(true);
                        TweenScale.SetTweenScale(m_storywordTween, UITweener.Style.Once, new Vector3(0.01f, 0.01f, 0.01f), Vector3.one, 0.5f, 0f, null);
                    }


                    if (kTalkAniPlay.kIndex == 1)
                        kTalkAniPlay.Play("StroyTalk_In");
                    //kCharNameLabel.SetText(param.Value1, false, true);
                    //kTalkTextLabel.SetText(param.Value2, true, true);

                    SetText(kCharNameLabel, param.Value1, false);
                    SetText(kTalkTextLabel, param.Value2, _bSkip ? false : true);

                    Log.Show("Voice : " + param.Voice + " / " + param.Value2);

                    if (param.Voice != "" && param.Voice != string.Empty)
                    {
						string[] voices = Utility.Split(param.Voice, ','); //param.Voice.Split(',');
						float voiceTotalTime = 0f;
                        for (int i = 0; i < voices.Length; i++)
                        {
                            SoundManager.sSoundInfo voice = VoiceMgr.Instance.PlayStory(param.BundlePath, voices[i]);
                            if(voice.clip == null)
                            {
                                Debug.LogError(voices[i] + " 파일이 없습니다");
                            }
                            else
                            {
                                voiceTotalTime += voice.clip.length;
                            }
                        }

                        if (_bAuto)
                        {
                            ftime = (voiceTotalTime / voices.Length);
                            ftime += GetAutoCharVoice();
                        }
                    }
                    else
                    {
                        ftime = param.Value2.Length * kTalkTextLabel.kCharSpeed;
                        PlayKeyboardSound(ftime);

                        if (_bAuto)
                        {
                            ftime += GetAutoNarration();
                        }
                    }
                    _logIndex++;
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
            case eScenarioType.BLR_TEX_ON:
                {
                    kStoryBGUnit.BlurOn(param.Pos);
                }
                break;
            case eScenarioType.BLR_TEX_OFF:
                {
                    kStoryBGUnit.BlurOff();
                }
                break;
            case eScenarioType.SEL_TALK:
                {
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
                    //if (activevn != null)
                    //    activevn.EndVisualNovel();
                    kStoryBGUnit.BlurOff();
                    if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
                        SetUIActive(false, true);
                    else
                    {
                        if (kTalkAniPlay.kIndex == 0)
                            kTalkAniPlay.Play("StroyTalk_Out");

                        GameUIManager.Instance.ShowUI("GameOffPopup", false);

                        SetUIActive(false, true);
                    }

                    return;
                }
                break;
			case eScenarioType.DELAY: {
				ftime = param.Time;
			}
			break;

            default:
                {
                    Log.Show(m_currentType.ToString() + " 해당하는 액션이 없습니다.");
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

    /// <summary>
    ///  다음 바로 진행
    /// </summary>
    public void AutoNext()
    {
        _waitAuto = false;
        OnClick_NextBtn();
    }

    public void ActiveNextBtn()
    {
        //kNextBtn.gameObject.SetActive(true);
        kNextBtn.enabled = true;
    }

    /// <summary>
    ///  백버튼 처리
    /// </summary>
    /// <returns></returns>
    public override bool IsBackButton()
    {
        if (_FavorabilityLogListInstance.gameObject.activeSelf)
        {
            OnClick_HideLog();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// 전체화면 흔들기
    public void ShakeAll(float duration = 1)
    {
        StartCoroutine(ShakeObject(this.gameObject, duration));
    }

    public void SkakeTalkObject(GameObject target, float duration, float amount = 2)
    {
        StartCoroutine(ShakeObject(target, duration));
    }

    IEnumerator ShakeObject(GameObject target, float duration, float amount = 2)
    {
        m_shakeOriginPos = this.transform.localPosition;
        float timer = 0;
        while (timer <= duration)
        {
            target.transform.localPosition = (Vector3)Random.insideUnitCircle * amount + m_shakeOriginPos;

            timer += Time.deltaTime;
            yield return null;
        }
        target.transform.localPosition = m_shakeOriginPos;
    }

    GameObject AddEffect(string effName, string bundlePath)
    {
        if (m_dicEffects == null)
            m_dicEffects = new Dictionary<string, GameObject>();

        if (!m_dicEffects.ContainsKey(effName))
        {
            bundlePath = bundlePath.Replace(" ", "");
			string[] path = Utility.Split(bundlePath, '/'); //bundlePath.Split('/');
            GameObject obj = ResourceMgr.Instance.CreateFromAssetBundle(path[0].ToLower(), bundlePath + ".prefab");
            obj.SetActive(false);
            obj.transform.parent = this.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            obj.transform.SetChildLayer(5);
            m_dicEffects.Add(effName, obj);
        }

        if (m_dicEffects.ContainsKey(effName))
            return m_dicEffects[effName];
        else
            return null;
    }

    Vector3 GetPosition(string pos)
    {
        pos = pos.Replace(" ", "");
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

    void PlayKeyboardSound(float ftime)
    {
        if (_bSkip)
            return;
        m_keyboardPlayer = StartCoroutine(PlayKeyboard(ftime));
    }

    IEnumerator PlayKeyboard(float ftime)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(kTalkTextLabel.kCharSpeed);
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


    #region Audio
    private void SetAudioSources()
    {
        if (m_bgmAudioSource == null)
        {
            m_bgmAudioSource = this.gameObject.AddComponent<AudioSource>();
            m_bgmAudioSource.playOnAwake = false;
            m_bgmAudioSource.loop = true;
            m_bgmAudioSource.volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.Ambience, FSaveData.Instance.GetBGVolume());
            m_bgmAudioSource.outputAudioMixerGroup = SoundManager.Instance.GetAudioMixer(SoundManager.eSoundType.Ambience);
        }

        m_bgmAudioSource.clip = null;

        if (m_seAudioSources == null)
            m_seAudioSources = new List<AudioSource>();

        for (int i = 0; i < m_seAudioSources.Count; i++)
        {
            m_seAudioSources[i].playOnAwake = false;
            m_seAudioSources[i].loop = false;
            m_seAudioSources[i].clip = null;
            m_seAudioSources[i].volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.FX, FSaveData.Instance.GetSEVolume());
            m_seAudioSources[i].outputAudioMixerGroup = SoundManager.Instance.GetAudioMixer(SoundManager.eSoundType.FX);
        }
    }

    AudioSource GetSEAudiosouce(AudioClip ac)
    {
        for (int i = 0; i < m_seAudioSources.Count; i++)
        {
            if (m_seAudioSources[i].isPlaying)
            {
                if (m_seAudioSources[i].clip.Equals(ac))
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

    void PlayBGM(AudioClip ac, float volume, bool bFadeIn = false)
    {
        if (null == m_bgmAudioSource.clip)
            m_bgmAudioSource.clip = ac;
        else if (!m_bgmAudioSource.clip.Equals(ac))
            m_bgmAudioSource.clip = ac;

        if (bFadeIn)
        {
            m_bgmFadeInCoroutine = StartCoroutine(FadeINBGMAudio(volume));
        }
        else
        {
            m_bgmAudioSource.volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.Ambience, volume);
            m_bgmAudioSource.Play();
        }
    }

    void StopBGM(float volume, bool bFadeOut)
    {
        if (bFadeOut)
            m_bgmFadeOutCoroutine = StartCoroutine(FadeOutBGMAudio(volume));
        else
            m_bgmAudioSource.Stop();
    }

    private IEnumerator FadeOutBGMAudio(float target = 0f)
    {
        while (m_bgmAudioSource.volume > SoundManager.Instance.GetVolume(SoundManager.eSoundType.Ambience, target))
        {
            m_bgmAudioSource.volume -= (Time.deltaTime * 0.1f);
            yield return null;
        }
        m_bgmAudioSource.volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.Ambience, target);
    }

    private IEnumerator FadeINBGMAudio(float target = 0f)
    {
        if (!m_bgmAudioSource.isPlaying)
            m_bgmAudioSource.Play();
        while (m_bgmAudioSource.volume < SoundManager.Instance.GetVolume(SoundManager.eSoundType.Ambience, target))
        {
            m_bgmAudioSource.volume += (Time.deltaTime * 0.1f);
            yield return null;
        }
        m_bgmAudioSource.volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.Ambience, target);
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

        for (int i = 0; i < m_seAudioSources.Count; i++)
        {
            if (null != m_seAudioSources[i].clip)
            {
                if (m_seAudioSources[i].clip.Equals(ac))
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
        while (ao.volume > SoundManager.Instance.GetVolume(SoundManager.eSoundType.FX, target))
        {
            ao.volume -= (Time.deltaTime * 0.1f);
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
            ao.volume += (Time.deltaTime * 0.1f);
            yield return null;
        }

        ao.volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.FX, target);
    }
    #endregion

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

    public override void OnClickClose()
    {
        CancelInvoke( "AutoNext" );
        CancelInvoke( "SkipNext" );

        SoundManager.Instance.StopVoice();
        base.OnClickClose();
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
        else if (FSaveData.Instance.VLText.Equals(3))
        {
            fLabelTextShow.SetText(text, false, true);
            return;
        }

        Log.Show("Text Speed : " + FSaveData.Instance.VLText);

        fLabelTextShow.kCharSpeed = charSpeed;
        fLabelTextShow.SetText(text, banim, true);
    }
}
