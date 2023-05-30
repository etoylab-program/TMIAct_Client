using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIShootingModePanel : UISpecialModePanel
{
    

    private bool _bisGameStart = false;

    

	public UILabel kScoreLabel;
    public UILabel kQualifyLabel;
	public UILabel kTimeLabel;
	public UIButton kBtnLeftShootingBtn;
	public UIButton kBtnRightShootingBtn;
    public UIButton kBtnPause;
	public UIThrowScope kLeftScope;
	public UIThrowScope kRightScope;

    public List<float> kScopeLv = new List<float>();
    public int _currentScore { get; private set; }

    [Header("Score HUD")]
    public GameObject kScoreHUDRootObj;
    private List<UIScoreText> kScoreTextList;

    private Camera _uiCamera;

    private int _prevScore;
    private int _prevAddScore;
    private Coroutine _currentScoreCor = null;

    private int _currentScopeLevel = 0;

    private float _leftBtnTimer = 0f;
    private float _rightBtnTimer = 0f;

    public float _btnCheckTime = 0.25f;

    private const int _damageTextCnt = 10;

 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
		if(_uiCamera == null)
        {
            UIRoot uiroot = GameObject.Find("UI Root").GetComponent<UIRoot>();
            _uiCamera = uiroot.GetComponentInChildren<Camera>();

            kLeftScope.uiCamera = _uiCamera;
            kRightScope.uiCamera = _uiCamera;

            //kLeftScope.GetPlayerIKObject();
            //kRightScope.GetPlayerIKObject();
        }

        _bisGameStart = false;
        
        _leftBtnTimer = _btnCheckTime;
        _rightBtnTimer = _btnCheckTime;

        _currentScore = 0;
        _prevScore = _currentScore;
        _prevAddScore = 0;
        kScoreLabel.textlocalize = string.Format("{0:#,##0}", _currentScore);

        _currentScopeLevel = 0;

        Object damageObj = ResourceMgr.Instance.LoadFromAssetBundle("ui", "UI/ScoreText.prefab");
        kScoreTextList = new List<UIScoreText>();
        if (damageObj != null)
        {
            for (int i = 0; i < _damageTextCnt; i++ )
            {
                GameObject damageGO = GameObject.Instantiate(damageObj) as GameObject;
                damageGO.transform.parent = kScoreHUDRootObj.transform;
                damageGO.transform.localPosition = Vector3.zero;
                damageGO.transform.localRotation = Quaternion.identity;
                damageGO.transform.localScale = Vector3.one;

                kScoreTextList.Add(damageGO.GetComponent<UIScoreText>());
            }
        }

        if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam || AppMgr.Instance.IsAndroidPC())
        {
            AppMgr.Instance.CustomInput.BindButton(AppMgr.Instance.CustomInput.GetKeyMapping(BaseCustomInput.eKeyKind.Right).GetKeyKind(), kBtnRightShootingBtn, null, null, null, null, null, null);
            AppMgr.Instance.CustomInput.BindButton(AppMgr.Instance.CustomInput.GetKeyMapping(BaseCustomInput.eKeyKind.PadRB).GetKeyKind(), kBtnRightShootingBtn, null, null, null, null, null, null);

            AppMgr.Instance.CustomInput.BindButton(AppMgr.Instance.CustomInput.GetKeyMapping(BaseCustomInput.eKeyKind.Left).GetKeyKind(), kBtnLeftShootingBtn, null, null, null, null, null, null);
            AppMgr.Instance.CustomInput.BindButton(AppMgr.Instance.CustomInput.GetKeyMapping(BaseCustomInput.eKeyKind.PadLB).GetKeyKind(), kBtnLeftShootingBtn, null, null, null, null, null, null);

            AppMgr.Instance.CustomInput.BindButton(AppMgr.Instance.CustomInput.GetKeyMapping(BaseCustomInput.eKeyKind.Pause).GetKeyKind(), kBtnPause, null, null, null, null, null, null);

            CustomPCInput pcinput = AppMgr.Instance.CustomInput as CustomPCInput;
            UpdateKeyMappingType(!pcinput.UseGamePad);
        }
        else
        {
            kBtnRightShootingBtn.HideKeyMapping();
            kBtnLeftShootingBtn.HideKeyMapping();
            kBtnPause.HideKeyMapping();
        }

        
        UpdateScoreLevel(kScopeLv.Count - 1);
    }

	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}

    public void ShootingGameStart()
    {
        _bisGameStart = true;
        kQualifyLabel.textlocalize = string.Format("Qualify : {0:#,##0}", World.Instance.StageData.ConditionValue);
    }

    private void Update()
    {
        if (!_bisGameStart)
            return;

        if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam || AppMgr.Instance.IsAndroidPC())
        {
            if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Left))
            {
                OnClick_BtnLeftShootingBtn(); 
                UpdateKeyMappingType(true);
            }
            else if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.PadLB))
            {                
                OnClick_BtnLeftShootingBtn();
                UpdateKeyMappingType(false);                
            }
            else if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Right))
            {
                OnClick_BtnRightShootingBtn(); 
                UpdateKeyMappingType(true);
            }
            else if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.PadRB))
            {
                OnClick_BtnRightShootingBtn();
                UpdateKeyMappingType(false);
            }

            //else
            //{
            //    float v = AppMgr.Instance.CustomInput.GetXAxis(true);
            //    if (v <= -1.0f) { OnClick_BtnLeftShootingBtn(); UpdateKeyMappingType(false); }
            //    else if (v >= 1.0f) { OnClick_BtnRightShootingBtn(); UpdateKeyMappingType(false); }
            //}
        }
        else
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                OnClick_BtnLeftShootingBtn();
            if (Input.GetKeyDown(KeyCode.RightArrow))
                OnClick_BtnRightShootingBtn();
#endif
        }

        /*
        if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Pause) && !World.Instance.IsEndGame)
        {
            UIGamePausePopup ui = GameUIManager.Instance.GetUI<UIGamePausePopup>("GamePausePopup");
            if (ui == null)
                return;

            if (ui.isActiveAndEnabled == true)
            {
                GameUIManager.Instance.HideUI("GamePausePopup");
                World.Instance.Pause(false);
            }
            else
            {
                OnBtnPause();
            }
        }
        */

        kScoreLabel.textlocalize = string.Format("{0:#,##0}", _currentScore);
        _leftBtnTimer += Time.deltaTime * Time.timeScale;
        _rightBtnTimer += Time.deltaTime * Time.timeScale;
    }

    public override void OnClickClose()
    {
        GameUIManager.Instance.HideUI("GamePausePopup");
        World.Instance.Pause(false);
    }

    public void OnClick_BtnLeftShootingBtn()
	{
        if (!_bisGameStart)
            return;

        if (World.Instance.IsPause)
            return;

        if (_leftBtnTimer < _btnCheckTime)
            return;
        //_leftBtnTimer = 0;

        ThrowingManager.Instance.PlayShooting(eScopeDir.Left);
        bool flag = kLeftScope.kThrowScope.ShotThrowTarget();
        if (!flag)
            _leftBtnTimer = 0f;
    }
	
	public void OnClick_BtnRightShootingBtn()
	{
        if (!_bisGameStart)
            return;

        if (World.Instance.IsPause)
            return;

        if (_rightBtnTimer < _btnCheckTime)
            return;
        //_rightBtnTimer = 0f;

        ThrowingManager.Instance.PlayShooting(eScopeDir.Right);
        bool flag = kRightScope.kThrowScope.ShotThrowTarget();
        if (!flag)
            _rightBtnTimer = 0f;
    }

    public override void OnBtnPause()
    {
        FComponent f = GameUIManager.Instance.GetUI("GamePausePopup");
        if (f == null || f.gameObject.activeSelf)
            return;
        World.Instance.Pause(true);
        GameUIManager.Instance.ShowUI("GamePausePopup", true);
    }

    public void AddScore(Vector3 pos,int addScore, int combo)
    {
        _prevScore = _prevScore + _prevAddScore;
        _currentScore = _prevScore;
        _prevAddScore = addScore;

        Utility.StopCoroutine(this, ref _currentScoreCor);

        for(int i = 0; i < kScoreTextList.Count; i++)
        {
            if(kScoreTextList[i].IsHide)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
                kScoreTextList[i].ShowScore(addScore, combo, _uiCamera.ScreenToWorldPoint(screenPos));
                break;
            }
        }
            

        _currentScoreCor = StartCoroutine(Utility.UpdateCoroutineValue((x) => _currentScore = (int)x, _currentScore, _currentScore + addScore, 0.2f));

        //UpdateScoreLevel(_currentScopeLevel + 1);
    }

    public void DamageHp()
    {
        //ThrowingManager.Instance.ShootingFailed();
        //UpdateScoreLevel(0);
    }

    private void UpdateScoreLevel(int lv)
    {
        if (_currentScopeLevel == lv)
            return;

        _currentScopeLevel = lv;
        if (_currentScopeLevel >= kScopeLv.Count)
            _currentScopeLevel = kScopeLv.Count - 1;

        kLeftScope.SetScopeScale(kScopeLv[_currentScopeLevel]);
        kRightScope.SetScopeScale(kScopeLv[_currentScopeLevel]);
    }

    public void UpdateTime(System.TimeSpan ts)
    {
        kTimeLabel.textlocalize = string.Format("{0}:{1}.{2}", ts.Minutes.ToString("D2"), ts.Seconds.ToString("D2"), (ts.Milliseconds / 10).ToString("D2"));
    }

    private void UpdateKeyMappingType(bool isKeyBoard)
    {
        AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Pause, isKeyBoard);

        if (isKeyBoard)
        {
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Left, isKeyBoard);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Right, isKeyBoard);
        }
        else
        {
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.PadLB, isKeyBoard);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.PadRB, isKeyBoard);
        }
    }

    //private Vector3 GetWorldToUIPos(Vector3 position)
    //{
    //    Vector3 result = Vector3.zero;
    //    Vector3 screenPos = Camera.main.WorldToViewportPoint(position);

    //    result = _uiCamera.ViewportToScreenPoint(screenPos);


    //}
}
