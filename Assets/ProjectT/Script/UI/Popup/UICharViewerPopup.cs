using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharViewer
{
    public static UICharViewerPopup ShowCharPopup(string prevUI, GameObject originTexObj, Transform originTexParent)
    {
        UICharViewerPopup popup = null;

        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
        {
            popup = LobbyUIManager.Instance.GetUI<UICharViewerPopup>("CharViewerPopup");
        }
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
        {
            popup = GameUIManager.Instance.GetUI<UICharViewerPopup>("CharViewerPopup");
        }

        
        popup.SetTaimanin(prevUI.Equals("BookCharInfoPopup") || prevUI.Equals("CharInfoPanel"));
        popup.InitViewer(prevUI, originTexObj, originTexParent);
        return popup;
    }

    public static UICharViewerPopup CreateCharPopup(RewardData rewardData)
    {
        UICharViewerPopup popup = null;
        popup = LobbyUIManager.Instance.GetUI<UICharViewerPopup>("CharViewerPopup");
        popup.SetTaimanin(false);
        popup.CreateCharViewer(rewardData);
        return popup;
    }

    public static UICharViewerPopup ShowWeaponViewPopup(string prevUI, GameObject originTexObj, Transform originTexParent)
    {
        UICharViewerPopup popup = null;
        popup = LobbyUIManager.Instance.GetUI<UICharViewerPopup>("CharViewerPopup");
        popup.SetTaimanin(false);
        popup.InitWeaponViewer(prevUI, originTexObj, originTexParent);
        return popup;
    }
}

public class UICharViewerPopup : FComponent
{
    enum eViewerType
    {
        Horizontal,
        Vertical,
    }

    enum eVIEWERSTAGE
    {
        Rotation,
        CamMove,
        Zoom,
        None,
    }

	public UIButton kBackBtn;
    public GameObject VerticalBackBtnPos;
    public GameObject HorizontalBackBtnPos;
    
    public UIButton kRefreshBtn;
    public GameObject VerticalRefreshBtnPos;
    public GameObject HorizontalRefreshBtnPos;
    
    public UISprite VerticalRefreshBtnSpr;
    public UISprite HorizontalRefreshBtnSpr;
    
    public GameObject kScrolObj;
    public GameObject VerticalScrollBtnPos;
    public GameObject HorizontalScrollBtnPos;

    public FList kScrollList;
    
    public GameObject kCharTexRoot;

	private UITexture m_charTex;
    private Transform m_originParent;
    private string m_prevUI;
    private Vector3 m_originPos;
    private Quaternion m_originRot;

    private float m_tweenTime = 0.4f;
    private TweenRotation m_tweenRotation;
    private TweenPosition m_tweenPosition;
    private TweenScale m_tweenScale;

    private bool m_actionFlag = false;
    private eVIEWERSTAGE m_viewerState = eVIEWERSTAGE.None;

    private float m_charCameraSize;
    private Vector3 m_charCameraPos;
    private Quaternion m_charRotation;
    private Vector3 m_charOriginPos;

    private float m_orthoZoomSpeed = 0.01f;
    private float m_moveSpeed = 0.01f;
    private float m_rotSpeed = 10f;

    //private Vector2 m_srcMoveValue = Vector2.zero;
    //private Vector2 m_destMoveValue = Vector2.zero;

    private float m_srcRotValue = 0f;
    private float m_destRotValue = 0f;

    private Unit m_rotTarget;
    
    //Weapon Values
    private Vector3 m_originFrom;
    private Vector3 m_originTo;
    private GameObject m_rotWeaponTarget;

    //RenderTexture Size
    private Vector2 m_originRenderTextureSize = Vector2.zero;
    private const int m_viewerDefaultsize = 1024;

    private bool m_weaponViewer = false;

    private Vector3 m_figureGroundPos = new Vector3(0f, 0.2f, 0f);
    private const float m_figureHeight = -0.25f;

    private UITexture m_rewardCharTex;
    private bool m_rewardChar = false;

    private eViewerType _viewerType = eViewerType.Horizontal;
    private Vector2 _originCharTexSize;

    private List<int> _lobbyAnimLockList = new List<int>();
    private List<GameTable.LobbyAnimation.Param> _lobbyAllAnimParamList = new List<GameTable.LobbyAnimation.Param>();

    private bool _bTaimanin = false;
    private CharData _charData;
    private Coroutine _animCoroutine = null;
    private Coroutine _backAnimCoroutine = null;
    private eAnimation _curSelectAnimation = eAnimation.None;

    private AudioSource mVoiceAudioSrc  = null;
    private float[]     mSamples        = new float[128];


    public override void Awake()
	{
		base.Awake();
        
        if (kScrollList != null)
        {
            kScrollList.EventUpdate = _UpdateScrollListSlot;
            kScrollList.EventGetItemCount = _GetScrollListElementCount;
        }
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

		base.OnDisable();
        m_weaponViewer = false;
        m_rewardChar = false;
        _viewerType = eViewerType.Horizontal;
        
        if (_bTaimanin && RenderTargetChar.Instance.RenderPlayer != null)
        {
            RenderTargetChar.Instance.RenderPlayer.aniEvent.aniFace.SetLayerWeight( 1, 0f );
            RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Idle01, 0, eFaceAnimation.Idle01, 0);
            RenderTargetChar.Instance.ShowAttachedObject(false);
            RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject(RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.InGameOnly, false);
            RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject(RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.LobbyOnly, true);
        }
        
        Utility.StopCoroutine(this, ref _backAnimCoroutine);
        Utility.StopCoroutine(this, ref _animCoroutine);
    }

    public override void InitComponent()
	{
        kBackBtn.isEnabled = false;

        SetViewerType();

        if (m_rewardCharTex != null && !m_rewardChar)
            m_rewardCharTex.gameObject.SetActive(false);
        
        _lobbyAnimLockList.Clear();

        if (_bTaimanin)
        {
            if (RenderTargetChar.Instance.RenderPlayer != null)
            {
                _charData = GameInfo.Instance.GetCharDataByTableID(RenderTargetChar.Instance.RenderPlayer.tableId);
            }
        
            if (_charData != null)
            {
                _lobbyAllAnimParamList =
                    GameInfo.Instance.GameTable.FindAllLobbyAnimation(x => x.Character == _charData.TableID);

                for (int i = 0; i < _lobbyAllAnimParamList.Count; i++)
                {
                    int flag = 1 << _lobbyAllAnimParamList[i].No;
                    if ((_charData.CharAniFlag & flag) == flag)
                    {
                        _lobbyAnimLockList.Add(0);
                    }
                    else
                    {
                        if (_lobbyAllAnimParamList[i].LockType <= 0)
                        {
                            _lobbyAnimLockList.Add(0);
                        }
                        else
                        {
                            _lobbyAnimLockList.Add(1);
                        }
                    }
                }
            
                RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Idle01, 0, eFaceAnimation.Idle01, 0);
            }

            if (_charData == null)
            {
                _bTaimanin = false;
            }
        }
        
        kScrolObj.SetActive(_bTaimanin);
        kScrollList.UpdateList(preInit: false);
        _curSelectAnimation = eAnimation.None;

        mVoiceAudioSrc = SoundManager.Instance.GetAudioSource(SoundManager.eSoundType.Voice);
    }

    private void _UpdateScrollListSlot(int index, GameObject slotObject)
    {
        UICharViewerListSlot slot = slotObject.GetComponent<UICharViewerListSlot>();
        if (null == slot)
        {
            return;
        }
		
        GameTable.LobbyAnimation.Param data = null;
        
        if (0 <= index && _lobbyAllAnimParamList.Count > index)
        {
            data = _lobbyAllAnimParamList[index];
        }

        int lockFlag = 1;
        if (0 <= index && _lobbyAnimLockList.Count > index)
        {
            lockFlag = _lobbyAnimLockList[index];
        }

        if ( !AppMgr.Instance.configData.m_Network ) {
            lockFlag = 0;
		}
		
        slot.ParentGO = this.gameObject;
        slot.UpdateSlot(index, lockFlag, data);
    }
    
    private int _GetScrollListElementCount()
    {
        return _lobbyAllAnimParamList.Count;
    }

    private void SetViewerType()
    {
        switch (_viewerType)
        {
            case eViewerType.Horizontal:
                {
                    kBackBtn.transform.SetParent(HorizontalBackBtnPos.transform);
                    kRefreshBtn.transform.SetParent(HorizontalRefreshBtnPos.transform);
                    kScrolObj.transform.SetParent(HorizontalScrollBtnPos.transform, false);
                    VerticalRefreshBtnSpr.gameObject.SetActive(true);
                    HorizontalRefreshBtnSpr.gameObject.SetActive(false);
                }
                break;
            case eViewerType.Vertical:
                {
                    kBackBtn.transform.SetParent(VerticalBackBtnPos.transform);
                    kRefreshBtn.transform.SetParent(VerticalRefreshBtnPos.transform);
                    kScrolObj.transform.SetParent(VerticalScrollBtnPos.transform, false);
                    VerticalRefreshBtnSpr.gameObject.SetActive(false);
                    HorizontalRefreshBtnSpr.gameObject.SetActive(true);
                }
                break;
        }

        Utility.InitTransform(kBackBtn.gameObject);
        Utility.InitTransform(kRefreshBtn.gameObject);
    }

    private void ChangeViewerType()
    {
        m_tweenRotation = null;
        m_tweenScale = null;

        m_tweenRotation = m_charTex.gameObject.GetComponent<TweenRotation>();
        if (m_tweenRotation == null)
        {
            m_tweenRotation = m_charTex.gameObject.AddComponent<TweenRotation>();
        }

        m_tweenScale = m_charTex.gameObject.GetComponent<TweenScale>();
        if (m_tweenScale == null)
        {
            m_tweenScale = m_charTex.gameObject.AddComponent<TweenScale>();
        }

        if (_viewerType == eViewerType.Horizontal)
        {
            _viewerType = eViewerType.Vertical;

            if (m_tweenRotation)
                TweenRotation.SetTweenRotation(m_tweenRotation, UITweener.Style.Once, m_charTex.transform.localRotation.eulerAngles, new Vector3(0, 0, 90f), m_tweenTime, 0f, null);

            if (m_tweenScale)
                TweenScale.SetTweenScale(m_tweenScale, UITweener.Style.Once, m_charTex.transform.localScale, new Vector3(1.5f, 1.5f, 1.5f), m_tweenTime, 0f, null);
        }
        else
        {
            _viewerType = eViewerType.Horizontal;

            if (m_tweenRotation)
                TweenRotation.SetTweenRotation(m_tweenRotation, UITweener.Style.Once, m_charTex.transform.localRotation.eulerAngles, new Vector3(0, 0, 0f), m_tweenTime, 0f, null);

            if (m_tweenScale)
                TweenScale.SetTweenScale(m_tweenScale, UITweener.Style.Once, m_charTex.transform.localScale, new Vector3(1f, 1f, 1f), m_tweenTime, 0f, null);
        }
            

        SetViewerType();
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}

    public void CreateCharViewer(RewardData reward)
    {
        m_rewardChar = true;

        if(m_rewardCharTex == null)
        {
            GameObject child = new GameObject("Reward_RenderTex");
            child.transform.parent = this.transform;
            child.transform.localScale = Vector3.one;
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            m_rewardCharTex = child.AddComponent<UITexture>();

            m_rewardCharTex.mainTexture = (RenderTexture)ResourceMgr.Instance.LoadFromAssetBundle("ui", "UI/CharRenderTexture.renderTexture");
            m_rewardCharTex.width = 1024;
            m_rewardCharTex.height = 1024;
        }
        
        m_rewardCharTex.gameObject.SetActive(true);

        GameTable.Costume.Param costumeTabledata = GameInfo.Instance.GameTable.FindCostume(x => x.ID == reward.Index);
        if(costumeTabledata == null)
        {
            Log.Show(reward.Index + " / CostumeTableData is NULL", Log.ColorType.Red);
        }
        CharData tempChardata = new CharData();
        tempChardata.CUID = 0;
        tempChardata.TableData = GameInfo.Instance.GameTable.FindCharacter(x => x.ID == costumeTabledata.CharacterID);
        tempChardata.TableID = tempChardata.TableData.ID;
        tempChardata.EquipCostumeID = costumeTabledata.ID;

        RenderTargetChar.Instance.gameObject.SetActive(true);
        RenderTargetChar.Instance.InitRenderTargetChar(tempChardata.TableID, GameInfo.Instance.UserData.UUID, tempChardata, false, eCharacterType.Figure);

        InitViewer("Reward", m_rewardCharTex.gameObject, m_rewardCharTex.transform.parent);
    }

    public void InitViewer(string prevUI, GameObject originTexObj, Transform originTexParent)
    {
        m_originParent = originTexParent;
        m_originPos = originTexObj.transform.localPosition;
        Log.Show(m_originPos);
        m_prevUI = prevUI;
        originTexObj.transform.parent = kCharTexRoot.transform;
        originTexObj.transform.localPosition = Vector3.zero;
        originTexObj.transform.localScale = Vector3.one;
        m_charTex = originTexObj.GetComponent<UITexture>();
        m_originRenderTextureSize = new Vector2(m_charTex.width, m_charTex.height);

        m_charTex.width = m_viewerDefaultsize;
        m_charTex.height = m_viewerDefaultsize;

        m_charCameraSize = RenderTargetChar.Instance.kRenderTargetCharCamera.orthographicSize;
        m_charCameraPos = RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition;

        if(RenderTargetChar.Instance.Figure != null)
        {
            m_charRotation = RenderTargetChar.Instance.Figure.transform.localRotation;
            m_rotTarget = RenderTargetChar.Instance.Figure;

            m_charOriginPos = m_rotTarget.transform.localPosition;
            m_figureGroundPos = new Vector3(0, m_charOriginPos.y + m_figureHeight, 0f);
            m_rotTarget.transform.localPosition = m_figureGroundPos;
            RenderTargetChar.Instance.kRenderTargetCharCamera.orthographicSize = 1.05f;
            RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition = new Vector3(0f, 0.825f, 3.7f);
        }
        if (RenderTargetChar.Instance.RenderPlayer != null)
        {
            m_charRotation = RenderTargetChar.Instance.RenderPlayer.transform.localRotation;
            m_rotTarget = RenderTargetChar.Instance.RenderPlayer;

            RenderTargetChar.Instance.kRenderTargetCharCamera.orthographicSize = 1.6f;
            RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition = new Vector3(0f, 0.825f, 3.7f);
        }
        if (RenderTargetChar.Instance.RenderEnemy != null)
        {
            m_charRotation = RenderTargetChar.Instance.RenderEnemy.transform.localRotation;
            m_rotTarget = RenderTargetChar.Instance.RenderEnemy;
            
            
            RenderTargetChar.Instance.kRenderTargetCharCamera.orthographicSize = 0.7f;
            RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition = new Vector3(0f, 1.1f, 3.7f);
        }

        SetRenderTextureTween();

        SetUIActive(true);
    }

    public void InitWeaponViewer(string prevUI, GameObject originTexObj, Transform originTexParent)
    {
        m_weaponViewer = true;

        m_originParent = originTexParent;
        m_originPos = originTexObj.transform.localPosition;
        m_prevUI = prevUI;
        originTexObj.transform.parent = kCharTexRoot.transform;
        originTexObj.transform.localPosition = Vector3.zero;
        originTexObj.transform.localScale = Vector3.one;
        m_charTex = originTexObj.GetComponent<UITexture>();

        m_originRenderTextureSize = new Vector2(m_charTex.width, m_charTex.height);

        m_charTex.width = m_viewerDefaultsize;
        m_charTex.height = m_viewerDefaultsize;

        m_charCameraSize = RenderTargetWeapon.Instance.camDefault.orthographicSize;
        m_charCameraPos = RenderTargetWeapon.Instance.camDefault.transform.localPosition;

        m_rotWeaponTarget = RenderTargetWeapon.Instance.kWeaponPosistionObj;

        SetRenderTextureTween();

        SetUIActive(true);
    }

    void SetRenderTextureTween()
    {
        m_actionFlag = false;
        m_viewerState = eVIEWERSTAGE.None;

        if (!m_weaponViewer)
        {
            m_tweenPosition = m_charTex.gameObject.GetComponent<TweenPosition>();
            if (m_tweenPosition == null)
                m_tweenPosition = m_charTex.gameObject.AddComponent<TweenPosition>();
        }
        else
        {
            m_tweenPosition = m_charTex.gameObject.GetComponent<TweenPosition>();
            if(m_tweenPosition != null)
            {
                m_originFrom = m_tweenPosition.from;
                m_originTo = m_tweenPosition.to;

                m_tweenPosition.from = new Vector3(10, 0, 0);
                m_tweenPosition.to = new Vector3(-10, 0, 0);
            }
        }

        m_tweenRotation = null;
        m_tweenScale = null;

        if (_viewerType == eViewerType.Vertical)
        {
            m_tweenRotation = m_charTex.gameObject.GetComponent<TweenRotation>();
            if (m_tweenRotation == null)
            {
                m_tweenRotation = m_charTex.gameObject.AddComponent<TweenRotation>();
            }

            m_tweenScale = m_charTex.gameObject.GetComponent<TweenScale>();
            if (m_tweenScale == null)
            {
                m_tweenScale = m_charTex.gameObject.AddComponent<TweenScale>();
            }
        }

        if (m_tweenRotation)
        {
            TweenRotation.SetTweenRotation(m_tweenRotation, UITweener.Style.Once, m_charTex.transform.localRotation.eulerAngles, new Vector3(0, 0, 90f), m_tweenTime, 0f, null);
        }

        if (m_tweenScale)
        {
            TweenScale.SetTweenScale(m_tweenScale, UITweener.Style.Once, m_charTex.transform.localScale, new Vector3(1.5f, 1.5f, 1.5f), m_tweenTime, 0f, null);
        }

        if (m_tweenRotation || m_tweenScale)
        {
            Invoke("ActionFlagDelay", m_tweenTime + 0.1f);
        }
        else
        {
            Invoke("ActionFlagDelay", 0.1f);
        }
    }

    public void SetTaimanin(bool bTaimanin)
    {
        _bTaimanin = bTaimanin;
    }

    void ActionFlagDelay()
    {
        m_actionFlag = true;
        ActiveBackBtnCollider();
    }
 
	public void OnClick_BackBtn()
	{
        OnClickClose();
    }

    public override void OnClickClose()
    {
        if (!m_actionFlag)
            return;

        m_actionFlag = false;

        m_charTex.transform.parent = m_originParent;

        //한번 껏다켜줘야 나와서 임시로 적용
        m_charTex.gameObject.SetActive(false);
        m_charTex.gameObject.SetActive(true);

        m_charTex.width = (int)m_originRenderTextureSize.x;
        m_charTex.height = (int)m_originRenderTextureSize.y;
        m_originRenderTextureSize = Vector2.zero;

        if (!m_weaponViewer)
        {
            TweenPosition.SetTweenPosition(m_tweenPosition, UITweener.Style.Once, m_charTex.transform.localPosition, m_originPos, m_tweenTime, 0f, null);
            
        }
        else
        {
            m_tweenPosition.from = m_originFrom;
            m_tweenPosition.to = m_originTo;
        }

        

        if (m_tweenRotation)
        {
            TweenRotation.SetTweenRotation(m_tweenRotation, UITweener.Style.Once, m_charTex.transform.localRotation.eulerAngles, Vector3.zero, m_tweenTime, 0f, null);
        }

        if (m_tweenScale)
        {
            TweenScale.SetTweenScale(m_tweenScale, UITweener.Style.Once, m_charTex.transform.localScale, Vector3.one, m_tweenTime, 0f, null);
        }

        Log.Show(m_charTex.width + " / " + m_charTex.height);

        m_charTex = null;

        if (!m_weaponViewer)
        {
            RenderTargetChar.Instance.kRenderTargetCharCamera.orthographicSize = m_charCameraSize;
            RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition = m_charCameraPos;
            if (RenderTargetChar.Instance.Figure != null)
                m_rotTarget.transform.localPosition = m_charOriginPos;
        }
        else
        {
            RenderTargetWeapon.Instance.camDefault.orthographicSize = m_charCameraSize;
            RenderTargetWeapon.Instance.camDefault.transform.localPosition = m_charCameraPos;
        }


        if (m_rotTarget != null)
        {
            m_rotTarget.aniEvent.transform.localRotation = m_charRotation;
            m_rotTarget = null;
        }

        if (m_rotWeaponTarget != null)
        {
            m_rotWeaponTarget.transform.localRotation = Quaternion.identity;
            m_rotWeaponTarget = null;
        }

        SetUIActive(false);
    }

    private void Update()
    {
        if (!m_actionFlag)
            return;

        if (Mathf.Abs(AppMgr.Instance.CustomInput.Delta) > 0.1f)
        {
            if (!m_weaponViewer)
            {
                RenderTargetChar.Instance.kRenderTargetCharCamera.orthographicSize -= AppMgr.Instance.CustomInput.Delta * Time.deltaTime;

                if (RenderTargetChar.Instance.kRenderTargetCharCamera.orthographicSize >= 2f)
                {
                    RenderTargetChar.Instance.kRenderTargetCharCamera.orthographicSize = 2f;
                }

                if (RenderTargetChar.Instance.kRenderTargetCharCamera.orthographicSize <= 0.2f)
                {
                    RenderTargetChar.Instance.kRenderTargetCharCamera.orthographicSize = 0.2f;
                }
            }
            else
            {
                RenderTargetWeapon.Instance.camDefault.orthographicSize -= AppMgr.Instance.CustomInput.Delta * Time.deltaTime;

                if (RenderTargetWeapon.Instance.camDefault.orthographicSize >= 2f)
                {
                    RenderTargetWeapon.Instance.camDefault.orthographicSize = 2f;
                }

                if (RenderTargetWeapon.Instance.camDefault.orthographicSize <= 0.2f)
                {
                    RenderTargetWeapon.Instance.camDefault.orthographicSize = 0.2f;
                }
            }

            return;
        }

        CharRotation(Vector3.zero);
        MoveCamera(Vector3.zero);
    }

    void MoveCamera(Vector2 moveTouch)
    {
        Vector3 v = AppMgr.Instance.CustomInput.MultiTouchDeltaPos;
        if(v == Vector3.zero)
        {
            return;
        }

        if(_viewerType == eViewerType.Horizontal)
        {
            v.x = -AppMgr.Instance.CustomInput.MultiTouchDeltaPos.y;
            v.y = AppMgr.Instance.CustomInput.MultiTouchDeltaPos.x;
        }

        v *= AppMgr.Instance.CustomInput.Sensitivity;

        if (!m_weaponViewer)
        {
            RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition = new Vector3(
                RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition.x + (v.y * m_moveSpeed * Time.deltaTime),
             RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition.y + (v.x * m_moveSpeed * Time.deltaTime),
              RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition.z);

            if(RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition.x >= 0.5f)
            {
                RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition = new Vector3(0.5f,
                    RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition.y,
                    RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition.z);
            }
            if(RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition.x <= -0.5f)
            {
                RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition = new Vector3(-0.5f,
                    RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition.y,
                    RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition.z);
            }
            if(RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition.y >= 2f)
            {
                RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition = new Vector3(
                    RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition.x,
                    2f,
                    RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition.z);
            }
            if(RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition.y <= -0.5f)
            {
                RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition = new Vector3(
                    RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition.x,
                    -0.5f,
                    RenderTargetChar.Instance.kRenderTargetCharCamera.transform.localPosition.z);
            }
        }
        else
        {
            RenderTargetWeapon.Instance.camDefault.transform.localPosition = new Vector3(
                RenderTargetWeapon.Instance.camDefault.transform.localPosition.x + (v.y * m_moveSpeed * Time.deltaTime),
                 RenderTargetWeapon.Instance.camDefault.transform.localPosition.y + (v.x * m_moveSpeed * Time.deltaTime),
              RenderTargetWeapon.Instance.camDefault.transform.localPosition.z);

            if (RenderTargetWeapon.Instance.camDefault.transform.localPosition.x >= 0.5f)
            {
                RenderTargetWeapon.Instance.camDefault.transform.localPosition = new Vector3(0.5f,
                    RenderTargetWeapon.Instance.camDefault.transform.localPosition.y,
                    RenderTargetWeapon.Instance.camDefault.transform.localPosition.z);
            }
            if (RenderTargetWeapon.Instance.camDefault.transform.localPosition.x <= -0.5f)
            {
                RenderTargetWeapon.Instance.camDefault.transform.localPosition = new Vector3(-0.5f,
                    RenderTargetWeapon.Instance.camDefault.transform.localPosition.y,
                    RenderTargetWeapon.Instance.camDefault.transform.localPosition.z);
            }
            if (RenderTargetWeapon.Instance.camDefault.transform.localPosition.y >= 0.5f)
            {
                RenderTargetWeapon.Instance.camDefault.transform.localPosition = new Vector3(
                    RenderTargetWeapon.Instance.camDefault.transform.localPosition.x,
                    0.5f,
                    RenderTargetWeapon.Instance.camDefault.transform.localPosition.z);
            }
            if (RenderTargetWeapon.Instance.camDefault.transform.localPosition.y <= -0.5f)
            {
                RenderTargetWeapon.Instance.camDefault.transform.localPosition = new Vector3(
                    RenderTargetWeapon.Instance.camDefault.transform.localPosition.x,
                    -0.5f,
                    RenderTargetWeapon.Instance.camDefault.transform.localPosition.z);
            }
        }
        
    }

	private void CharRotation( Vector2 pos ) {
		if ( AppMgr.Instance.CustomInput.DeltaPos == Vector3.zero ) {
			return;
		}

		float deltaPos = 0f;

		switch ( _viewerType ) {
			case eViewerType.Horizontal: {
                deltaPos = AppMgr.Instance.CustomInput.DeltaPos.x;
            }
            break;
			case eViewerType.Vertical: {
                deltaPos = AppMgr.Instance.CustomInput.DeltaPos.y;
            }
            break;
		}

        Vector3 rot;

		if ( !m_weaponViewer ) {
            rot.x = m_rotTarget.aniEvent.transform.localRotation.x;
            rot.y = m_rotTarget.aniEvent.transform.localRotation.eulerAngles.y - ( deltaPos * m_rotSpeed * Time.deltaTime );
            rot.z = m_rotTarget.aniEvent.transform.localRotation.z;

            m_rotTarget.aniEvent.transform.localRotation = Quaternion.Euler( rot );

			if ( RenderTargetChar.Instance.AddedWeapon != null && RenderTargetChar.Instance.AddedWeapon is PlayerGuardian ) {
				RenderTargetChar.Instance.AddedWeapon.aniEvent.transform.localRotation = m_rotTarget.aniEvent.transform.localRotation;
			}
		}
		else {
            rot.x = m_rotWeaponTarget.transform.localRotation.x;
            rot.y = m_rotWeaponTarget.transform.localRotation.eulerAngles.y - ( deltaPos * m_rotSpeed * Time.deltaTime );
            rot.z = m_rotWeaponTarget.transform.localRotation.z;

            m_rotWeaponTarget.transform.localRotation = Quaternion.Euler( rot );
		}

		m_destRotValue = m_srcRotValue;

	}

	void ActiveBackBtnCollider()
    {
        kBackBtn.isEnabled = true;
    }

    public override bool IsBackButton()
    {
        OnClick_BackBtn();
        return false;
    }

    public void OnClick_RefeshDir()
    {
        ChangeViewerType();
    }

    public void OnClick_ScrollBtn()
    {
        kScrollList.Panel.gameObject.SetActive(!kScrollList.Panel.gameObject.activeSelf);
    }
    
    public void PlayAnimation(eAnimation bodyAnim, eFaceAnimation faceAnim)
    {
        if (_curSelectAnimation == bodyAnim && RenderTargetChar.Instance.RenderPlayer.aniEvent.IsAniPlaying(_curSelectAnimation) == eAniPlayingState.Playing)
        {
            return;
        }

        _curSelectAnimation = bodyAnim;
        
        RenderTargetChar.Instance.RenderPlayer.PlayAniImmediate(bodyAnim, faceAnim, 0);

        Utility.StopCoroutine(this, ref _backAnimCoroutine);
        Utility.StopCoroutine(this, ref _animCoroutine);
        
        float delay = 0;
        if (bodyAnim == eAnimation.Lobby_Weapon || bodyAnim == eAnimation.Lobby_Weapon_Idle)
        {
            delay = RenderTargetChar.Instance.RenderPlayer.aniEvent.GetCutFrameLength(bodyAnim);
            _animCoroutine = StartCoroutine(ShowAttachedObject(delay));
            
        }
        else
        {
            RenderTargetChar.Instance.ShowAttachedObject(false);
            RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject(RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.InGameOnly, false);
            RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject(RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.LobbyOnly, true);

            switch (bodyAnim)
            {
                case eAnimation.Idle02: case eAnimation.Idle03: case eAnimation.Enter01:
                case eAnimation.LobbyTouchMotion01: case eAnimation.LobbyTouchMotion02:
                case eAnimation.LobbyTouchMotion04: case eAnimation.LobbyTouchMotion05: case eAnimation.LobbyTouchMotion06:
                case eAnimation.Accept01: case eAnimation.Accept02: case eAnimation.Accept03:
                {
                    delay = RenderTargetChar.Instance.RenderPlayer.aniEvent.GetAniLength(bodyAnim);
                    _backAnimCoroutine = StartCoroutine(BackIdle(delay));
                }
                break;
            }
        }
    }
    
    private IEnumerator ShowAttachedObject(float delay)
    {
        if (0 < delay)
        {
            RenderTargetChar.Instance.ShowAttachedObject(false);
        }
        
        yield return new WaitForSeconds(delay);

		RenderTargetChar.Instance.SetCostumeWeapon( _charData, !_charData.IsHideWeapon );
		RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject( RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.InGameOnly, !_charData.IsHideWeapon );
		RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject( RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.LobbyOnly, _charData.IsHideWeapon );
	}

    private IEnumerator BackIdle(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Idle01, 0, eFaceAnimation.Idle01, 0);
    }

    private void FixedUpdate()
    {
        if(RenderTargetChar.Instance.RenderPlayer == null)
        {
            return;
        }

        RenderTargetChar.Instance.RenderPlayer.UpdateLipSync();
    }
}
