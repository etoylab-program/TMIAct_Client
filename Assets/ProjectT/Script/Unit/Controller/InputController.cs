
using System;
using System.Collections.Generic;
using UnityEngine;


public class InputController : MonoBehaviour
{
    [Flags]
    public enum ELockBtnFlag
    {
        NONE        = 0,
        ATTACK      = 1 << 1,
        DASH        = 1 << 2,
        WEAPON      = 1 << 3,
        USKILL      = 1 << 4,
        SUPPORTER   = 1 << 5,
    }


    new Transform transform;

    public static float MAX_THUMB_RADIUS    = 0.25f;
    public static float MIN_MOVE_DISTANCE   = 120.0f;

    [Header("[Property]")]
    public Transform	m_root;
    public UISprite		m_imgBg;
    public UISprite		m_imgThumb;
    public int			directionCount;

    public ELockBtnFlag	LockBtn		{ get; set; }			= ELockBtnFlag.NONE;
    public bool			LockPause	{ get; set; }			= false;
    public bool			isPause		{ get; private set; }	= false;

    private Player				mOwner          = null;
    private InputEvent			mInputEvent     = new InputEvent();
    private int					mCurFingerId    = -1;
    private Vector3				mDir            = Vector3.zero;
    private bool				mStartCtrl      = false;
    private Vector3				mBeforeDir      = Vector3.zero;
    private Vector3				mOriginalPos    = Vector3.zero;
    private Vector3				mThumbPos       = Vector3.zero;
    private Vector3				mTouchPos       = Vector3.zero;
    private bool				mLockDirection  = false;
	private UIGamePlayPanel		mGameUI			= null;
	private List<EventDelegate>	mListEv			= new List<EventDelegate>();


	public bool Pause( bool pause ) {
        if( LockPause ) {
            return true;
        }

		if( pause ) {
			Debug.Log( "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@인풋컨트롤러 퍼즈" );
			InitPos();
		}
		else {
			Debug.Log( "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@어디서 인풋컨트롤러에 퍼즈를 펄스로 보내나~~" );
		}

		isPause = pause;
		ResetBeforeDir();

		if( isPause ) {
			if( mOwner.IsAutoMove ) {
				mOwner.SetAutoMove( false );
			}
			else if( !mOwner.UsingUltimateSkill ) {
				SendEvent( eEventType.EVENT_PLAYER_INPUT_DIR, true );
			}

			if( mOwner.AI ) {
				mOwner.StopBT();
			}
		}
		else {
			if( mOwner.AI ) {
				if( World.Instance.EnemyMgr.HasAliveMonster() ) {
					mOwner.ResetBT();
				}
				else {
					mOwner.SetAutoMove( true );
				}
			}
		}

		return true;
	}

	public bool HasDirection() {
		return mStartCtrl;
	}

	public Vector3 GetRawDirection() {
		if( isPause || mOwner.AI ) {
			return Vector3.zero;
		}

		return mDir;
	}

	public Vector3 GetDirection() {
		if( isPause || mOwner.AI ) {
			return Vector3.zero;
		}

		return Utility.Get3DDirFrom2DDir( mDir );
	}

	public void Init( int directionCount, UIGamePlayPanel uiGamePlayPanel ) {
		if( transform == null ) {
			transform = GetComponent<Transform>();
		}

		if( mOwner == null ) {
			SetOwner( GetComponent<Player>() );
		}

		mGameUI = uiGamePlayPanel;
		this.directionCount = directionCount;
		LockPause = false;

		if( !mOwner.IsHelper ) {
			SetInputUI();
		}

		isPause = false;
		mLockDirection = false;

		Reset();
	}

	public void SetInputUI() {
		m_root = mGameUI.joystick;
		m_imgBg = mGameUI.joystickBg;
		m_imgThumb = mGameUI.joystickThumb;

		mGameUI.btnAtk.SetPlayer( mOwner );

		if( AppMgr.Instance.CustomInput.InputType == BaseCustomInput.eInputType.KMGP ) {
			m_root.gameObject.SetActive( false );

			AppMgr.Instance.CustomInput.BindButton( BaseCustomInput.eKeyKind.Dash, World.Instance.UIPlay.btnDash, OnBtnDash, OnBtnJump, null, null, null, null );

			if( AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam || AppMgr.Instance.IsAndroidPC() ) {
				AppMgr.Instance.CustomInput.BindButton( BaseCustomInput.eKeyKind.Attack, World.Instance.UIPlay.btnAtk, OnBtnAttack, OnBtnGestureSkill, OnBtnChargeAttackStart, OnBtnChargeAttackEnd, OnBtnAtkPressStart, OnBtnAtkTouchEnd );
				AppMgr.Instance.CustomInput.BindButton( BaseCustomInput.eKeyKind.WeaponSkill, World.Instance.UIPlay.btnWpnSkill, World.Instance.UIPlay.OnBtnWpnSkill, null, null, null, null, null );
				AppMgr.Instance.CustomInput.BindButton( BaseCustomInput.eKeyKind.SupporterSkill, World.Instance.UIPlay.btnSupporter, World.Instance.UIPlay.OnBtnSupporter, null, null, null, null, null );
				AppMgr.Instance.CustomInput.BindButton( BaseCustomInput.eKeyKind.USkill, World.Instance.UIPlay.btnUSkill, OnBtnUltimateSkill, null, null, null, null, null );
				AppMgr.Instance.CustomInput.BindButton( BaseCustomInput.eKeyKind.ChangeWeapon, World.Instance.UIPlay.BtnWpnChange, World.Instance.UIPlay.OnBtnChangeWeapon, null, null, null, null, null );

				UISubCharacterUnit subCharBtn = World.Instance.UIPlay.GetSubCharUnitByIndexOrNull( 0 );
				AppMgr.Instance.CustomInput.BindButton( BaseCustomInput.eKeyKind.ChangeSubChar01, subCharBtn.Btn, subCharBtn.OnBtnChangeChar, null, null, null, null, null );

				subCharBtn = World.Instance.UIPlay.GetSubCharUnitByIndexOrNull( 1 );
				AppMgr.Instance.CustomInput.BindButton( BaseCustomInput.eKeyKind.ChangeSubChar02, subCharBtn.Btn, subCharBtn.OnBtnChangeChar, null, null, null, null, null );
			}
		}
		else {
			m_root.gameObject.SetActive( true );

		#if UNITY_EDITOR
			AppMgr.Instance.CustomInput.BindButton( BaseCustomInput.eKeyKind.Dash, World.Instance.UIPlay.btnDash, OnBtnDash, OnBtnJump, null, null, null, null );
		#endif
		}
		if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP )
			AppMgr.Instance.CustomInput.BindButton( BaseCustomInput.eKeyKind.Pause, World.Instance.UIPlay.BtnPause, null, null, null, null, null, null );
		else
			AppMgr.Instance.CustomInput.BindButton( BaseCustomInput.eKeyKind.Pause, World.Instance.UIPlay.BtnPause, null, null, null, null, null, null );

		mOriginalPos = m_root.transform.localPosition;

		mGameUI.btnAtk.m_onPressed.Clear();
		mGameUI.btnAtk.m_onPressed.Add( new EventDelegate( this, "OnBtnAttack" ) );

		EventDelegate eventDelegate = new EventDelegate( this, "OnBtnGestureSkill" );
		mGameUI.btnAtk.m_onGestureUp.Clear();
		mGameUI.btnAtk.m_onGestureUp.Add( eventDelegate );

		mGameUI.btnAtk.m_onPressing.Clear();
		mGameUI.btnAtk.m_onPressing.Add( eventDelegate );

		eventDelegate = new EventDelegate( this, "OnBtnJump" );
		mGameUI.btnDash.m_onPressing.Clear();
		mGameUI.btnDash.m_onPressing.Add( eventDelegate );

		mGameUI.btnDash.m_onGestureUp.Clear();
		mGameUI.btnDash.m_onGestureUp.Add( eventDelegate );

		mGameUI.btnDash.m_onPressed.Clear();
		mGameUI.btnDash.m_onPressed.Add( new EventDelegate( this, "OnBtnDash" ) );

		mGameUI.btnUSkill.onClick.Clear();
		mGameUI.btnUSkill.onClick.Add( new EventDelegate( this, "OnBtnUltimateSkill" ) );

		mGameUI.btnAtk.m_onChargeStart.Clear();
		mGameUI.btnAtk.m_onChargeStart.Add( new EventDelegate( this, "OnBtnChargeAttackStart" ) );

		mGameUI.btnAtk.m_onChargeEnd.Clear();
		mGameUI.btnAtk.m_onChargeEnd.Add( new EventDelegate( this, "OnBtnChargeAttackEnd" ) );

		mGameUI.btnAtk.m_onPressStart.Clear();
		mGameUI.btnAtk.m_onPressStart.Add( new EventDelegate( this, "OnBtnAtkPressStart" ) );

		mGameUI.btnAtk.m_onTouchEnd.Clear();
		mGameUI.btnAtk.m_onTouchEnd.Add( new EventDelegate( this, "OnBtnAtkTouchEnd" ) );

		/*
		EventDelegate eventDelegate = new EventDelegate(this, "OnBtnAttack");
		EventDelegate.Add( mGameUI.btnAtk.m_onPressed, eventDelegate );

		eventDelegate = new EventDelegate( this, "OnBtnGestureSkill" );
		EventDelegate.Add( mGameUI.btnAtk.m_onGestureUp, eventDelegate );
		EventDelegate.Add( mGameUI.btnAtk.m_onPressing, eventDelegate );

		eventDelegate = new EventDelegate( this, "OnBtnJump" );
		EventDelegate.Add( mGameUI.btnDash.m_onPressing, eventDelegate );
		EventDelegate.Add( mGameUI.btnDash.m_onGestureUp, eventDelegate );

		eventDelegate = new EventDelegate( this, "OnBtnDash" );
		EventDelegate.Add( mGameUI.btnDash.m_onPressed, eventDelegate );

		eventDelegate = new EventDelegate( this, "OnBtnUltimateSkill" );
		EventDelegate.Add( mGameUI.btnUSkill.onClick, eventDelegate );

		eventDelegate = new EventDelegate( this, "OnBtnChargeAttackStart" );
		EventDelegate.Add( mGameUI.btnAtk.m_onChargeStart, eventDelegate );

		eventDelegate = new EventDelegate( this, "OnBtnChargeAttackEnd" );
		EventDelegate.Add( mGameUI.btnAtk.m_onChargeEnd, eventDelegate );

		eventDelegate = new EventDelegate( this, "OnBtnAtkPressStart" );
		EventDelegate.Add( mGameUI.btnAtk.m_onPressStart, eventDelegate );

		eventDelegate = new EventDelegate( this, "OnBtnAtkTouchEnd" );
		EventDelegate.Add( mGameUI.btnAtk.m_onTouchEnd, eventDelegate );
		*/
	}

	public void SetOwner( Player player ) {
		if( player == null ) {
			return;
		}

		mOwner = player;
		mInputEvent.sender = mOwner;
	}

	public void ResetBeforeDir() {
		mBeforeDir = Vector3.zero;
	}

	public void CommandDirection( Vector3 dir ) {
		if( mLockDirection ) {
			return;
		}

		mDir = dir;

		if( mDir != mBeforeDir ) {
			SendEvent( eEventType.EVENT_PLAYER_INPUT_DIR, false );
			mBeforeDir = mDir;
		}
	}

	public void SendEvent( eEventType type, bool skipRunStop, System.Action callback = null, IActionBaseParam param = null ) {
		if( mOwner == null || !mOwner.IsActivate() ) {
			return;
		}

		Debug.Log( "InputController::SendEvent : " + type + " Pause : " + isPause );

		if( World.Instance.InGameCamera.Mode == InGameCamera.EMode.SIDE ) {
			if( World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.Z ) {
				mDir.y = 0.0f;
			}
			else if( World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.X ) {
				mDir.x = 0.0f;
			}
		}

		mInputEvent.eventType = type;
		mInputEvent.dir = mDir;
		mInputEvent.beforeDir = mBeforeDir;
		mInputEvent.SkipRunStop = skipRunStop;

		EventMgr.Instance.SendEvent( mInputEvent, callback, param );
	}

	public void OnBtnAttack() {
        if( World.Instance.IsEndGame || isPause || World.Instance.IsPause ) {
            return;
        }

		if( ( LockBtn & ELockBtnFlag.ATTACK ) != 0 ) {
			return;
		}

		SendEvent( eEventType.EVENT_PLAYER_INPUT_ATK, false );
	}

	public void OnBtnJump() {
        if( World.Instance.IsEndGame || isPause || World.Instance.IsPause ) {
            return;
        }

		if( ( LockBtn & ELockBtnFlag.DASH ) != 0 ) {
			return;
		}

		SendEvent( eEventType.EVENT_PLAYER_INPUT_JUMP, false );
	}

	public void OnBtnDash() {
        if( World.Instance.IsEndGame || isPause || World.Instance.IsPause ) {
            return;
        }

		if( ( LockBtn & ELockBtnFlag.DASH ) != 0 ) {
			return;
		}

		SendEvent( eEventType.EVENT_PLAYER_INPUT_DEFENCE, false );
	}

	public void OnBtnChargeAttackStart() {
		if( mOwner.IsHelper || World.Instance.IsEndGame || isPause || World.Instance.IsPause ) {
			return;
		}

		if( ( LockBtn & ELockBtnFlag.ATTACK ) != 0 ) {
			return;
		}

		SendEvent( eEventType.EVENT_PLAYER_INPUT_CHARGE_ATK_START, false );
	}

	public void OnBtnChargeAttackEnd() {
		if( mOwner.IsHelper || World.Instance.IsEndGame || isPause || World.Instance.IsPause ) {
			return;
		}

		if( ( LockBtn & ELockBtnFlag.ATTACK ) != 0 ) {
			return;
		}

		SendEvent( eEventType.EVENT_PLAYER_INPUT_CHARGE_ATK_END, false );
	}

	public void OnBtnGestureSkill() {
        if( World.Instance.IsEndGame || isPause || World.Instance.IsPause ) {
            return;
        }

		if( ( LockBtn & ELockBtnFlag.DASH ) != 0 ) {
			return;
		}

		SendEvent( eEventType.EVENT_PLAYER_INPUT_SPECIAL_ATK, false );
	}

	public void OnBtnAtkPressStart() {
		if( World.Instance.IsEndGame || isPause || World.Instance.IsPause ) {
			return;
		}

		SendEvent( eEventType.EVENT_PLAYER_INPUT_ATK_PRESS_START, false );
	}

	public void OnBtnAtkTouchEnd() {
		SendEvent( eEventType.EVENT_PLAYER_INPUT_ATK_TOUCH_END, false );
	}

	public void OnBtnUltimateSkill() {
		if( World.Instance.IsEndGame || isPause || World.Instance.IsPause ) {
			return;
		}

		if( ( LockBtn & ELockBtnFlag.USKILL ) != 0 ) {
			return;
		}

		SendEvent( eEventType.EVENT_PLAYER_INPUT_ULTIMATE_SKILL, false );
	}

	public void LockDirection( bool lockDirection ) {
		mLockDirection = lockDirection;
	}

	public void LockBtnFlag( ELockBtnFlag flag ) {
		LockBtn = flag;
	}

	private void Reset() {
		mCurFingerId = -1;
		mStartCtrl = false;

		InitPos();
	}

	private void InitPos() {
		if( m_imgBg ) {
			m_imgBg.transform.localPosition = Vector3.zero;
		}

		if( m_imgThumb ) {
			m_imgThumb.transform.localPosition = Vector3.zero;
		}

		mThumbPos = Vector3.zero;
		mDir = Vector3.zero;
	}

	private void Update() {
		if( World.Instance.IsEndGame || mOwner == null || !mOwner.IsActivate() || mOwner.AI || isPause || World.Instance.IsPause ) {
			return;
		}

		bool input = false;
		if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam || AppMgr.Instance.IsAndroidPC()) {
			input = UpdatePC();
		}
		else {
			input = UpdateTouch();
		}

		if( !input ) {
			return;
		}

		mDir = ( m_imgThumb.transform.localPosition - m_imgBg.transform.localPosition ) / MAX_THUMB_RADIUS;
		mDir.x *= mOwner.InverseXAxis ? -1.0f : 1.0f;
		mDir.y *= mOwner.InverseYAxis ? -1.0f : 1.0f;

		if( Vector3.Magnitude( mDir ) <= MIN_MOVE_DISTANCE ) {
			mDir = Vector3.zero;
		}
		else if( mDir != mBeforeDir ) {
			Vector3 v1 = Vector3.Normalize(mDir);
			Vector3 v2 = Vector3.Normalize(mBeforeDir);
			float dot = Vector3.Dot(v1, v2);

			float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
			if( angle >= ( 360.0f / (float)directionCount ) ) {
				SendEvent( eEventType.EVENT_PLAYER_INPUT_DIR, false );
				mBeforeDir = mDir;
			}
		}
	}

	private bool UpdateTouch() {
	#if UNITY_EDITOR
		if( !AppMgr.Instance.configData.RemoteTest ) {
			float h = AppMgr.Instance.CustomInput.GetXAxis();
			float v = AppMgr.Instance.CustomInput.GetYAxis();

			if( ( h == 0.0f && v == 0.0f ) || UICamera.mainCamera == null ) {
				EndInput();
				return false;
			}

			m_imgThumb.transform.localPosition = new Vector3( h * MIN_MOVE_DISTANCE, v * MIN_MOVE_DISTANCE, 0.0f );

			if( m_imgThumb.transform.localPosition == Vector3.zero ) {
				m_imgThumb.alpha = 0.4f;
				EndInput();
			}
			else {
				m_imgThumb.alpha = 0.8f;
			}

			mStartCtrl = false;
			if( v != 0.0f || h != 0.0f ) {
				mStartCtrl = true;
			}
		}
	#endif

		if( AppMgr.Instance.CustomInput.IsNoTouch() || UICamera.mainCamera == null ) {
			if( mStartCtrl ) {
				return true;
			}
			else {
				EndInput();
				return false;
			}
		}

		Touch touch = new Touch();
		if( mCurFingerId == -1 ) {
			touch = AppMgr.Instance.CustomInput.GetTouchInJoystickArea();
			mCurFingerId = touch.fingerId;
		}
		else {
			touch = AppMgr.Instance.CustomInput.GetTouchByFingerId( mCurFingerId );
		}

		if( touch.fingerId < 0 ) {
			mCurFingerId = -1;
			return mStartCtrl;
		}

		if( AppMgr.Instance.CustomInput.IsOverUI( touch.position ) ) {
			EndInput();
			return false;
		}

		Vector3 uiTouchPos = UICamera.mainCamera.ScreenToWorldPoint(touch.position);

		if( touch.phase == TouchPhase.Began ) {
			mStartCtrl = true;
			InitPos();
		}
		else if( touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled ) {
			EndInput();
		}

		if( isPause || !mStartCtrl || mLockDirection || World.Instance.IsPause ) {
			return false;
		}

		Vector3 relativeMousePos = uiTouchPos - m_root.transform.position;

		float rad = Mathf.Atan2(relativeMousePos.y, relativeMousePos.x);
		float x = Mathf.Cos(rad) * MAX_THUMB_RADIUS;
		float y = Mathf.Sin(rad) * MAX_THUMB_RADIUS;

		mThumbPos.x = Mathf.Abs( relativeMousePos.x ) < Mathf.Abs( x ) ? relativeMousePos.x : x;
		mThumbPos.y = Mathf.Abs( relativeMousePos.y ) < Mathf.Abs( y ) ? relativeMousePos.y : y;
		mThumbPos.z = 0.0f;

		m_imgThumb.transform.position = m_root.transform.position + mThumbPos;
		if( m_imgThumb.transform.position == Vector3.zero ) {
			m_imgThumb.alpha = 0.4f;
		}
		else {
			m_imgThumb.alpha = 0.8f;
		}

		return true;
	}

	private bool UpdatePC() {
		mTouchPos = AppMgr.Instance.CustomInput.GetTouchPos();

		float h = AppMgr.Instance.CustomInput.GetXAxis();
		float v = AppMgr.Instance.CustomInput.GetYAxis();

		if( ( h == 0.0f && v == 0.0f ) || UICamera.mainCamera == null ) {
			EndInput();
			return false;
		}

		if( isPause || mLockDirection || World.Instance.IsPause ) {
			return false;
		}

		m_imgThumb.transform.localPosition = new Vector3( h * MIN_MOVE_DISTANCE, v * MIN_MOVE_DISTANCE, 0.0f );
		if( m_imgThumb.transform.localPosition == Vector3.zero ) {
			m_imgThumb.alpha = 0.4f;
			EndInput();
		}
		else {
			m_imgThumb.alpha = 0.8f;
		}

		mStartCtrl = false;
		if( v != 0.0f || h != 0.0f ) {
			mStartCtrl = true;
		}

		return mStartCtrl;
	}

	private void EndInput() {
		Reset();

		if( mBeforeDir != mDir ) {
			SendEvent( eEventType.EVENT_PLAYER_INPUT_DIR, false );
			ResetBeforeDir();
		}
	}
}
