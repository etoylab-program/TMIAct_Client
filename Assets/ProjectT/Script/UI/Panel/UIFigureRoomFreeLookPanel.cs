
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIFigureRoomFreeLookPanel : FComponent
{
    public UISprite sprSpeedUp;

    [Header("Control Btns")]
    public UIButton BtnUp;
    public UIButton BtnDown;
    public UIButton BtnSpeedUp;

    [Header("AR Mode")]
    Vector3 kARObjOriginPos = Vector3.zero;
    #if AR_MODE
    public ARManager kARManager;
    public GameObject kARLeftRotBtn;
    public GameObject kARRightRotBtn;
    #endif
    float _rotFlag = 10f;

    [Header("Friend Mode")]
    public GameObject kFriendObj;
    public UILabel kFriendUserDesc;

    [Header("LobbyBG")]
    public GameObject kLobbyBGBtn;

	private Animation		m_ani				= null;
	private LobbyCamera		m_roomCamera		= null;
	private VirtualJoystick	m_joystick			= null;
	private bool			m_startVerticalMove	= false;
	private bool			m_movingUp			= false;
	private bool			m_speedUp			= false;


	public override void OnEnable() {
		if ( m_ani == null ) {
			m_ani = GetComponent<Animation>();
		}

		InitComponent();
		base.OnEnable();
	}

	public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();

		UIFigureRoomPanel figureRoomPanel = LobbyUIManager.Instance.GetUI<UIFigureRoomPanel>("FigureRoomPanel");
        figureRoomPanel.ShowTopPanelOnDisable = true;

        //FigureRoomScene.Instance.SetEditMode(UIFigureRoomEditModePanel.FigureCtrlType.None);
        m_roomCamera.SetJoystick(null);
        m_roomCamera.SetCameraType(LobbyCamera.eType.Normal, null);
    }

    public override void InitComponent()
    {
        if (m_joystick == null)
        {
            m_joystick = GetComponentInChildren<VirtualJoystick>();
        }

        m_roomCamera = FigureRoomScene.Instance.RoomCamera;

        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
        {
            m_joystick.gameObject.SetActive(false);
            BtnUp.gameObject.SetActive(false);
            BtnDown.gameObject.SetActive(false);
            BtnSpeedUp.gameObject.SetActive(false);

            m_roomCamera.SetCameraType(LobbyCamera.eType.Normal, null);
        }
        else
        {
            m_joystick.gameObject.SetActive(true);
            BtnUp.gameObject.SetActive(true);
            BtnDown.gameObject.SetActive(true);
            BtnSpeedUp.gameObject.SetActive(true);

            m_roomCamera.SetJoystick(m_joystick);
            m_roomCamera.SetCameraType(LobbyCamera.eType.Joystick, null);
        }

        m_startVerticalMove = false;
        m_speedUp = false;

        kFriendObj.SetActive(GameSupport.IsFriendRoom());
        kLobbyBGBtn.SetActive(!GameSupport.IsFriendRoom());
        if (GameSupport.IsFriendRoom())
        {
            FriendUserData friendUser = GameInfo.Instance.CommunityData.FriendList.Find(x => x.UUID == GameInfo.Instance.FriendRoomUserUUID);
            if (friendUser != null)
                kFriendUserDesc.textlocalize = string.Format(FLocalizeString.Instance.GetText(1535), friendUser.GetNickName() );
        }

    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
    }

    private void Update()
    {
        if (m_startVerticalMove)
            m_roomCamera.MoveVertical(m_movingUp);
    }

    public void OnBtnBack(bool bisHome = false)
    {
        OnClose();
        //OnARModeClose();

        if (GameSupport.IsFriendRoom())
        {
            GameInfo.Instance.FriendRoomUserUUID = 0;
            SoundManager.Instance.StopBgm();
            FigureRoomScene.Instance.DestroyFigureListInfo();

            LobbyDoorPopup.Show(DoorToLobby);
        }
        else
        {
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.FIGUREROOM);
            if (bisHome)
            {
                UIFigureRoomPanel figureRoom = LobbyUIManager.Instance.GetUI<UIFigureRoomPanel>("FigureRoomPanel");
                if (figureRoom != null)
                {
                    figureRoom.OnBtnBackToLobby();
                }
            }
        }
    }

    public void OnBtnStartMoveUp()
    {
        m_startVerticalMove = true;
        m_movingUp = true;
    }

    public void OnBtnStartMoveDown()
    {
        m_startVerticalMove = true;
        m_movingUp = false;
    }

    public void OnBtnEndMove()
    {
        m_startVerticalMove = false;
    }

    public void OnBtnSpeedUp()
    {
        m_speedUp = !m_speedUp;
        if (m_speedUp)
        {
            m_roomCamera.SpeedUp(2.0f);
            sprSpeedUp.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        else
        {
            m_roomCamera.ResetSpeed();
            sprSpeedUp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    public void OnBtnHideUI()
    {
        LobbyUIManager.Instance.EnableBlockUI(true, m_ani["RoomThemeUI_Hide"].length);
        m_ani.Play("RoomThemeUI_Hide");
    }

    public void OnBtnShowUI()
    {
        LobbyUIManager.Instance.EnableBlockUI(true, m_ani["RoomThemeUI_Show"].length);
        m_ani.Play("RoomThemeUI_Show");
    }

    public void OnBtnVR()
    {
        //OnARModeClose();
        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3001));

        //FigureRoomScene.Instance.SetMainCameraEnable(false);
    }
#if AR_MODE
    public void OnBtnAR()
    {
        if (FigureRoomScene.Instance.FigureRoomMode == FigureRoomScene.eFigureRoomMode.ARMode)
            return;
        if(FigureRoomScene.Instance.ListFigureInfo.Count <= 0)
        {
            //배치 된 피규어가 없습니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3100));
            return;
        }
        else
        {
            Log.Show(FigureRoomScene.Instance.ListFigureInfo.Count);

            //피규어를 선택해 주세요.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3101));
            //FigureRoomScene.Instance.FigureRoomMode = FigureRoomScene.eFigureRoomMode.ARMode;
            StartCoroutine(ARModeSelectFigure());
        }
 
    }

    IEnumerator ARModeSelectFigure()
    {
        FigureRoomScene.Instance.ResetSelectedFigureInfo();

        while (FigureRoomScene.Instance.SelectedFigureInfo == null)
        {
            yield return null;
        }

        LobbyUIManager.Instance.EnableBlockUI(true, m_ani["RoomThemeUI_Hide"].length);
        m_ani.Play("RoomThemeUI_Hide");
        kARObjOriginPos = FigureRoomScene.Instance.ARModeSelectFigurePos();
        FigureRoomScene.Instance.SetARMode(FigureRoomScene.eFigureRoomMode.ARMode);
        FigureRoomScene.Instance.RoomCamera.gameObject.SetActive(false);
        kARManager.SetARCamera(true);
        ARRotBtnVisible(true);
    }

    void OnARModeClose()
    {
        StopCoroutine(ARModeSelectFigure());
        if (FigureRoomScene.Instance.FigureRoomMode == FigureRoomScene.eFigureRoomMode.ARMode)
        {
            kARManager.SetARCamera(false);

            if (FigureRoomScene.Instance.SelectedFigureInfo != null)
            {
                FigureRoomScene.Instance.SelectedFigureInfo.figure.gameObject.transform.position = kARObjOriginPos;
            }

            FigureRoomScene.Instance.SetARMode(FigureRoomScene.eFigureRoomMode.EditMode);
            //FigureRoomScene.Instance.FigureRoomMode = FigureRoomScene.eFigureRoomMode.EditMode;
            FigureRoomScene.Instance.RoomCamera.gameObject.SetActive(true);
        }
        ARRotBtnVisible(false);
    }

    void ARRotBtnVisible(bool b)
    {
        kARLeftRotBtn.SetActive(b);
        kARRightRotBtn.SetActive(b);
    }
#endif
    public void OnClickARModeLeftRotBtn()
    {
        FigureRoomScene.Instance.SelectedFigureInfo.figure.transform.Rotate(0, _rotFlag, 0);
    }
    public void OnClickARModeRightRotBtn()
    {
        FigureRoomScene.Instance.SelectedFigureInfo.figure.transform.Rotate(0, -_rotFlag, 0);
    }

    public void OnBtnMovie()
    {
        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3001));
        //OnARModeClose();
    }

    public void OnBtnScreenShot()
    {
        FigureRoomScene.Instance.ScreenShot();
    }

    public void OnClick_SetLobbyBG()
    {
        Log.Show("OnClick_SetLobbyBG");
        MessagePopup.CYN(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3266), eTEXTID.OK, eTEXTID.NO, () => { FigureRoomScene.Instance.SetLobbyBGWithScreenShot(); });
        
    }

    public override bool IsBackButton()
    {
        OnBtnBack();

        return false;
    }

    public void DoorToLobby()
    {
        SoundManager.Instance.PlayBgm(Lobby.Instance.GetCurrentBGMID(), "LobbyBGM", true);

        LobbyUIManager.Instance.SetUICameraMultiTouch(false);
        LobbyUIManager.Instance.ShowUI("TopPanel", false);

        Lobby.Instance.MoveToLobby();
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN);
    }
}
