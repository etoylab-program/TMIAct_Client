using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CodeStage.AntiCheat.ObscuredTypes;

public class UIBikeRaceModePanel : UISpecialModePanel
{
    public delegate void RaceModeDelegate();

    //private World.eGameMode m_curGameMode = World.eGameMode.None;    
    private bool _pause { get { return World.Instance.IsPause; } }
    private bool _started = false;

    public UISprite kgaugeBgSpr;
	public UISprite kgaugeSpr;
    public UISlider kgaugeSlider;
	public UISprite kicoBikeSpr;
	public UISprite kicoGoalSpr;
	public UILabel kTimeLabel;
	public UISprite kbgSpr;
	public UISprite kicoSpr;
	public UILabel kgoldLabel;
	public UIButton kBtnRightArrowBtn;
	public UIButton kBtnLeftArrowBtn;
    public UIButton kBtnPause;

    [Header("ArrowBtnEffects")]
    public ParticleSystem kRightArrowEffect;
    public ParticleSystem kLeftArrowEffect;

    [Header("Player HP")]
    public List<RaceHP> kRaceHpObjs;

    private ObscuredInt _curHp;
    public ObscuredInt CurHp { get { return _curHp; } }
    private ObscuredInt _coins;
    public ObscuredInt CurCoins { get { return _coins; } }

    private int _clearConditionValue = 0;
    public int ClearConditionValue { get { return _clearConditionValue; } set { _clearConditionValue = value; } }

    RaceModeDelegate m_RaceModeDelegate;

    float _magnetDistance = 0;

    public Vector3 _camPos = Vector3.zero;
    public Quaternion _camRot = Quaternion.identity;

    public UIBikeRace_BuffGaugeUnit kBoostGauge;
    public UIBikeRace_BuffGaugeUnit kMagnetGauge;

    private WorldBike mWorldBike = null;


	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}

	public override void InitComponent() {
		if ( mWorldBike == null ) {
			mWorldBike = World.Instance as WorldBike;
		}

		if ( _camPos == Vector3.zero ) {
			_camPos = Camera.main.transform.position;
		}

		if ( _camRot == Quaternion.identity ) {
			_camRot = Camera.main.transform.rotation;
		}

		m_RaceModeDelegate = null;
		_started = false;
		_coins = 0;
		_curHp = kRaceHpObjs.Count;
		kgoldLabel.textlocalize = string.Format( "x{0:#,##0}", _coins );

		_magnetDistance = mWorldBike.kRider.kMagnetDistance;

		if ( AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam || AppMgr.Instance.IsAndroidPC() ) {
			AppMgr.Instance.CustomInput.BindButton( AppMgr.Instance.CustomInput.GetKeyMapping( BaseCustomInput.eKeyKind.Right ).GetKeyKind(), kBtnRightArrowBtn, null, null, null, null, null, null );
			AppMgr.Instance.CustomInput.BindButton( AppMgr.Instance.CustomInput.GetKeyMapping( BaseCustomInput.eKeyKind.PadRB ).GetKeyKind(), kBtnRightArrowBtn, null, null, null, null, null, null );

			AppMgr.Instance.CustomInput.BindButton( AppMgr.Instance.CustomInput.GetKeyMapping( BaseCustomInput.eKeyKind.Left ).GetKeyKind(), kBtnLeftArrowBtn, null, null, null, null, null, null );
			AppMgr.Instance.CustomInput.BindButton( AppMgr.Instance.CustomInput.GetKeyMapping( BaseCustomInput.eKeyKind.PadLB ).GetKeyKind(), kBtnLeftArrowBtn, null, null, null, null, null, null );

			AppMgr.Instance.CustomInput.BindButton( AppMgr.Instance.CustomInput.GetKeyMapping( BaseCustomInput.eKeyKind.Pause ).GetKeyKind(), kBtnPause, null, null, null, null, null, null );

			CustomPCInput pcinput = AppMgr.Instance.CustomInput as CustomPCInput;
			UpdateKeyMappingType( !pcinput.UseGamePad );
		}
		else {
			kBtnRightArrowBtn.HideKeyMapping();
			kBtnLeftArrowBtn.HideKeyMapping();
			kBtnPause.HideKeyMapping();
		}
	}

	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}

    public float ShowUI(int aniIndex)
    {
        SetUIActive(true, false);
        float delay = PlayAnimtion(aniIndex);

        return delay;
    }

    public void UpdateTime(TimeSpan ts)
    {
        kTimeLabel.textlocalize = string.Format("{0}:{1}.{2}", ts.Minutes.ToString("D2"), ts.Seconds.ToString("D2"), (ts.Milliseconds / 10).ToString("D2"));
    }

    public void StartRaceMode(float _delay)
    {
        Camera.main.transform.SetPositionAndRotation(_camPos, _camRot);

        mWorldBike.InGameCamera.enabled = true;
        mWorldBike.InGameCamera.SetPlayer(mWorldBike.Player);
        mWorldBike.InGameCamera.SetSimpleMode(new Vector3(0, 1.2f, 1f));

        mWorldBike.kRider.StartRaceMode(_delay, StartRaceModeCallBack);
    }

    public void StartRaceModeCallBack()
    {
        //_pause = false;
        _started = true;
    }


    private void Update()
    {
        if (!_started)
            return;
        if (_pause)
            return;

        //ÁÂ, ¿ì¹öÆ°
        if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam || AppMgr.Instance.IsAndroidPC())
        {
            if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Left))
            {
                OnClick_BtnLeftArrowBtn();
                UpdateKeyMappingType(true);
            }
            else if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.PadLB))
            {
                OnClick_BtnLeftArrowBtn();
                UpdateKeyMappingType(false);
            }
            else if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Right))
            {
                OnClick_BtnRightArrowBtn();
                UpdateKeyMappingType(true);
            }
            else if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.PadRB))
            {
                OnClick_BtnRightArrowBtn();
                UpdateKeyMappingType(false);

            }

            //else
            //{
            //    float v = AppMgr.Instance.CustomInput.GetXAxis(true);
            //    if (v <= -1.0f) { OnClick_BtnLeftArrowBtn(); UpdateKeyMappingType(false); }
            //    else if (v >= 1.0f) { OnClick_BtnRightArrowBtn(); UpdateKeyMappingType(false); }
            //}
        }
        else
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                OnClick_BtnLeftArrowBtn();
            if (Input.GetKeyDown(KeyCode.RightArrow))
                OnClick_BtnRightArrowBtn();
            if (Input.GetKeyDown(KeyCode.S))
            {
                mWorldBike.kRider.EndGame();
                mWorldBike.OnRaceSuccess();
            }
#endif
        }

        /*/ÀÏ½ÃÁ¤Áö
        if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Pause) && !World.Instance.IsEndGame)
        {
            UIGamePausePopup ui = GameUIManager.Instance.GetUI<UIGamePausePopup>("GamePausePopup");
            if (ui == null)
                return;

            if (ui.isActiveAndEnabled == true)
            {
                GameUIManager.Instance.HideUI("GamePausePopup");
                mWorldBike.Pause(false);
            }
            else
            {
                OnBtnPause();
            }
        }
        */

        if (kgaugeSlider != null)
        {
            kgaugeSlider.value = mWorldBike.kRider.transform.position.z / ClearConditionValue;
            if(kgaugeSlider.value >= 1f)
            {
                mWorldBike.kRider.EndGame();
                mWorldBike.OnRaceSuccess();
            }
        }
    }

    public override void OnClickClose()
    {
        GameUIManager.Instance.HideUI("GamePausePopup");
        mWorldBike.Pause(false);
    }

    //ÄÚÀÎ È¹µæ
    public void AddCoin()
    {
        _coins++;
        kgoldLabel.textlocalize = string.Format("x{0:#,##0}", _coins);
    }

    //È¸º¹ ¾ÆÀÌÅÛ È¹µæ
    public void AddRiderHP()
    {
        _curHp++;
        if(_curHp > kRaceHpObjs.Count)
        {
            _curHp = kRaceHpObjs.Count;
        }

        for(int i = 0; i < _curHp; i++)
        {
            kRaceHpObjs[i].SetHpState(true);
        }
    }
 
    //Àå¾Ö¹°¿¡ ºÎµúÇô¼­ Ã¼·Â °¨¼Ò
    public bool DamageRiderHP()
    {
        _curHp--;
        kRaceHpObjs[_curHp].SetHpState(false);
        if(_curHp <= 0)
        {
            mWorldBike.kRider.EndGame();

            mWorldBike.OnRaceFailed();
            return false;
        }
        return true;
    }
    
#region OnClick_Btns
    public void OnClick_BtnRightArrowBtn()
	{
        if (!_started)
            return;
        if (_pause)
            return;
        mWorldBike.kRider.BikeMoveHorizontal(1);
        SetOnClickBtn(1);        
    }
	
	public void OnClick_BtnLeftArrowBtn()
	{
        if (!_started)
            return;
        if (_pause)
            return;
        mWorldBike.kRider.BikeMoveHorizontal(-1);
        SetOnClickBtn(-1);        
    }

    void SetOnClickBtn(int _dir)
    {
        if(_dir.Equals(1))
        {
            //if(kRightArrowEffect != null)
            //{
            //    kRightArrowEffect.Play();
            //}
        }
        if(_dir.Equals(-1))
        {
            //if(kLeftArrowEffect != null)
            //{
            //    kLeftArrowEffect.Play();
            //}
        }
    }

    public override void OnBtnPause()
    {
        if (_pause == true)
            return;

        FComponent f = GameUIManager.Instance.GetUI("GamePausePopup");
        if (f == null || f.gameObject.activeSelf)
            return;

        GameUIManager.Instance.ShowUI("GamePausePopup", true);
    }

    private void UpdateKeyMappingType(bool isKeyBoard)
    {
        AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Pause, isKeyBoard);

        if(isKeyBoard)
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
#endregion
}
