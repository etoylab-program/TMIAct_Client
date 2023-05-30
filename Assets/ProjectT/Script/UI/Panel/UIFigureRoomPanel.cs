
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIFigureRoomPanel : FComponent
{
    public enum eBasicMenuType
    {
        None = 0,
        Env,
        Figure,
        Equipment
    }

    public enum eEnvMenuType
    {
        Theme = 0,
        Effect,
    }


    [Header("[Basic Menu]")]
    public FTab     tabBasic;
    public FTab     tabEnv;
    public FList    listEnvSlot;
    public FTab     tabFigure;
    public FTab     tabEquipment;
    public FList    listIconSlot;
    public UILabel  kNoneLabel;

    [Header("[Setting UI]")]
    public FList    listSetting;
    public UILabel  lbSettingCount;

    [Header("[Menu Button]")]
    public UISprite sprMenuOpen;
    public UISprite sprMenuClose;
    public UIButton btnOpenSaveLoad;

    public bool                 ShowTopPanelOnDisable   { get; set; }
    public RoomThemeSlotData    Data                    { get; private set; }

    private Animation           mAni;
    private eBasicMenuType      mBasicMenuType          = eBasicMenuType.None;
    private eEnvMenuType        mEnvMenuType            = eEnvMenuType.Theme;
    private eContentsPosKind    mIconSlotType           = eContentsPosKind._NONE_;
    private int                 mSelectedWeaponCharId   = 0;
    private bool                mMenuOn                 = false;
    private int                 mBeforeBuyIconIndex     = -1;

    private FigureRoomScene mFigureRoomScene    = null;
    private LobbyCamera     mRoomCamera         = null;

    private List<GameTable.RoomFunc.Param>      mListAllRoomFunc        = new List<GameTable.RoomFunc.Param>();
    private List<GameTable.RoomFigure.Param>    mListAllFigureChar      = new List<GameTable.RoomFigure.Param>();
    private List<GameTable.RoomFigure.Param>    mListAllFigureMob       = new List<GameTable.RoomFigure.Param>();
    private List<GameTable.RoomFigure.Param>    mListAllFigureWeapon    = new List<GameTable.RoomFigure.Param>();
    private List<GameTable.RoomFigure.Param>    mListCharWeapon         = new List<GameTable.RoomFigure.Param>();
    private List<FigureData>                    mListSettingFigure      = new List<FigureData>();

    private List<GameTable.RoomTheme.Param>     mListVisibleRoomTheme   = new List<GameTable.RoomTheme.Param>();


    public override void Awake()
    {
        base.Awake();
        mAni = GetComponent<Animation>();

        tabBasic.EventCallBack = OnTabBaisc;

        tabEnv.EventCallBack = OnTabEnv;
        listEnvSlot.EventGetItemCount = GetEnvSlotItemCount;
        listEnvSlot.EventUpdate = UpdateEnvSlot;

        tabFigure.EventCallBack = OnTabFigure;
        listIconSlot.EventGetItemCount = GetIconSlotItemCount;
        listIconSlot.EventUpdate = UpdateIconSlot;

        tabEquipment.EventCallBack = OnTabEquipment;

        listSetting.EventGetItemCount = GetSettingSlotItemCount;
        listSetting.EventUpdate = UpdateSettingSlot;
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

        mFigureRoomScene.RoomCamera.LockCamera( false );

        base.OnDisable();
		mBeforeBuyIconIndex = -1;
    }

    public override void InitComponent()
    {
        LobbyUIManager.Instance.SetUICameraMultiTouch(true);

        ShowTopPanelOnDisable = true;
        LobbyUIManager.Instance.ShowUI("TopPanel", false);

        mFigureRoomScene = FigureRoomScene.Instance;
        Data = mFigureRoomScene.RoomSlotData;
        mRoomCamera = mFigureRoomScene.RoomCamera;

        InitFigureRoomPanel();
        GameSupport.ShowTutorialFlag(eTutorialFlag.PRIVATEROOM);

        mRoomCamera.SetCameraType(LobbyCamera.eType.Normal, null);
    }

    public override void Renewal(bool bChildren)
    {
        if (mFigureRoomScene == null)
            return;

        UITopPanel topPanel = LobbyUIManager.Instance.GetUI<UITopPanel>("TopPanel");
        if(topPanel)
        {
            topPanel.Renewal(true);
        }

        GetLists();
        SetRoomFunc();

        base.Renewal(bChildren);

        kNoneLabel.SetActive(false);

        if (listEnvSlot.gameObject.activeSelf)
        {
			mFigureRoomScene.RoomCamera.LockCamera(true);

			listEnvSlot.UpdateList();
            listEnvSlot.RefreshNotMove();
            listEnvSlot.ScrollPositionSet();
        }
        else if (listIconSlot.gameObject.activeSelf)
        {
			mFigureRoomScene.RoomCamera.LockCamera(true);

			listIconSlot.UpdateList();
            listIconSlot.RefreshNotMove();
            listIconSlot.ScrollPositionSet();

            if (mBeforeBuyIconIndex > 0)
            {
                listIconSlot.LoadSavedFocus();
            }

             if (mIconSlotType == eContentsPosKind.MONSTER)
            {
                if(mListAllFigureMob.Count <= 0)
                {
                    kNoneLabel.SetActive(true);
                }
            }
        }

        SettingListUpdate();
    }

    public bool PossibleToSet()
    {
        if (mListSettingFigure.Count >= Data.TableData.MaxChar)
        {
            return false;
        }

        return true;
    }

    public void SelectFigureMoveToEditMode(FigureData data)
    {
        if (mFigureRoomScene.SetEditMode(data))
        {
            MoveToEditMode();
        }
    }

    public void AllRoomThemeFuncOff()
    {
        LinkedList<CListViewItem>.Enumerator iter = listEnvSlot.ListItem.GetEnumerator();
        while (iter.MoveNext())
        {
            UIFigureRoomEnvMenuListSlot slot = iter.Current.mGameObject.GetComponent<UIFigureRoomEnvMenuListSlot>();
            if(slot == null || slot.ListType != eEnvMenuType.Effect)
            {
                continue;
            }

            slot.OnBtnOff();
        }
    }

    public void SaveIconIndex(int index)
    {
        mBeforeBuyIconIndex = index;
        listIconSlot.SaveCurrentFocus();
    }

    private void InitFigureRoomPanel()
    {
        GetLists();

        // Basic Menu
        mBasicMenuType = eBasicMenuType.None;
        mEnvMenuType = eEnvMenuType.Theme;
        mIconSlotType = eContentsPosKind._NONE_;
        mSelectedWeaponCharId = 0;

        DisableSubTabs();

        // Setting UI
        listSetting.gameObject.SetActive(true);
        SettingListUpdate();

        // Menu Button
        DisableMenuBtn();

        SetRoomFunc();

        mFigureRoomScene.ShowAllPlacedFigure(true);
        mFigureRoomScene.ResetRoomCamera();
    }

    private void Update()
    {
        if(AppMgr.Instance.CustomInput.IsOverUI())
        {
            return;
        }

        //if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Dash"))
        if(AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Select))
        {
            FigureRoomScene.sFigureInfo pickedFigureInfo = GetPickingFigureInfo();
            if (pickedFigureInfo != null)
            {
                SelectFigureMoveToEditMode(pickedFigureInfo.data);
            }
            else
            {
                tabBasic.DisableTab();

                DisableSubTabs();
                DisableMenuBtn();
            }
        }
    }

    private FigureRoomScene.sFigureInfo GetPickingFigureInfo()
    {
        if (Physics.Raycast(mRoomCamera.camera.ScreenPointToRay(AppMgr.Instance.CustomInput.GetTouchPos()), out RaycastHit hitInfo, 
            Mathf.Infinity, 1 << (int)eLayer.Figure))
        {
            FigureUnit figure = hitInfo.collider.GetComponentInParent<FigureUnit>();
            if (figure && figure.gameObject.activeSelf)
            {
                return mFigureRoomScene.ListFigureInfo.Find(x => x.figure == figure);
            }
        }

        return null;
    }

    private void MoveToEditMode()
    {
        if(mFigureRoomScene.SelectedFigureInfo == null)
        {
            Debug.LogError("선택된 피규어가 없어서 에디트 모드로 넘어갈 수 없습니다.");
            return;
        }

        ShowTopPanelOnDisable = false;

        mFigureRoomScene.ShowPlacedUnselectFigure(true);

        mRoomCamera.SetCameraType(LobbyCamera.eType.LookAtTarget, mFigureRoomScene.SelectedFigureInfo.figure.transform, false);
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.FIGUREROOM_EDITMODE);
    }

    private void SetRoomFunc()
    {
        /*for (int i = 0; i < Data.listRoomFunc.Count; i++)
        {
            mFigureRoomScene.ActivateRoomFunc(Data.listRoomFunc[i]);
        }*/
    }

    private void GetLists()
    {
        mListAllRoomFunc.Clear();
        mListAllRoomFunc.AddRange(GameInfo.Instance.GameTable.FindAllRoomFunc(x => x.RoomTheme == Data.TableID));

        mListAllFigureChar.Clear();
        List<GameTable.RoomFigure.Param> listParamFigure = GameInfo.Instance.GameTable.RoomFigures.FindAll(x => x.ContentsType == (int)eContentsPosKind.COSTUME);
        for (int i = 0; i < listParamFigure.Count; i++)
        {
            if (listParamFigure[i].StoreRoomID <= 0)
                continue;
            if (GameInfo.Instance.HasCostume(listParamFigure[i].ContentsIndex))
            {
                mListAllFigureChar.Add(listParamFigure[i]);
            }
        }

        mListAllFigureMob.Clear();
        listParamFigure = GameInfo.Instance.GameTable.RoomFigures.FindAll(x => x.ContentsType == (int)eContentsPosKind.MONSTER);
        for (int i = 0; i < listParamFigure.Count; i++)
        {
            if (listParamFigure[i].StoreRoomID <= 0)
                continue;
            if (GameInfo.Instance.GetMonsterBookData(listParamFigure[i].ContentsIndex) != null)
            {
                mListAllFigureMob.Add(listParamFigure[i]);
            }
        }

        mListAllFigureWeapon.Clear();
        listParamFigure = GameInfo.Instance.GameTable.RoomFigures.FindAll(x => x.ContentsType == (int)eContentsPosKind.WEAPON);
        for (int i = 0; i < listParamFigure.Count; i++)
        {
            if (listParamFigure[i].StoreRoomID <= 0)
                continue;
            if (GameInfo.Instance.GetWeaponBookData(listParamFigure[i].ContentsIndex) != null)
            {
                mListAllFigureWeapon.Add(listParamFigure[i]);
            }
        }

        mListCharWeapon.Clear();
        mListCharWeapon.AddRange(mListAllFigureWeapon.FindAll(x => x.CharacterID == mSelectedWeaponCharId));

        mListSettingFigure.Clear();
        var _list = mFigureRoomScene.ListFigureInfo.FindAll(x => x.data.placement == true);
        foreach (var item in _list)
        {
            mListSettingFigure.Add(item.data);
        }
    }

    private void DisableSubTabs()
    {
		mFigureRoomScene.RoomCamera.LockCamera(false);

		tabEnv.gameObject.SetActive(false);

        listEnvSlot.gameObject.SetActive(false);
        listIconSlot.gameObject.SetActive(false);

        tabFigure.gameObject.SetActive(false);
        tabEquipment.gameObject.SetActive(false);
    }

    private void DisableMenuBtn()
    {
        mMenuOn = false;

        sprMenuOpen.gameObject.SetActive(true);
        sprMenuClose.gameObject.SetActive(false);

        btnOpenSaveLoad.gameObject.SetActive(false);
    }

    private void SettingListUpdate()
    {
        listSetting.gameObject.SetActive(true);
        listSetting.UpdateList();

        lbSettingCount.textlocalize = string.Format("{0}/{1}", mListSettingFigure.Count, Data.TableData.MaxChar);
    }

    private bool OnTabBaisc(int nSelect, SelectEvent type)
    {
        DisableSubTabs();
        DisableMenuBtn();

        mBasicMenuType = (eBasicMenuType)(nSelect + 1);
        switch (mBasicMenuType)
        {
            case eBasicMenuType.Env:
                tabEnv.gameObject.SetActive(true);
                break;

            case eBasicMenuType.Figure:
                tabFigure.gameObject.SetActive(true);
                break;

            case eBasicMenuType.Equipment:
                tabEquipment.gameObject.SetActive(true);
                break;
        }

        return true;
    }

    private bool OnTabEnv(int nSelect, SelectEvent type)
    {
        mEnvMenuType = (eEnvMenuType)nSelect;
        listEnvSlot.gameObject.SetActive(true);

        if (mEnvMenuType == eEnvMenuType.Effect && mListAllRoomFunc.Count <= 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3219));
        }

        Renewal(true);
        return true;
    }

    private int GetEnvSlotItemCount()
    {
        if (mEnvMenuType == eEnvMenuType.Theme)
        {
            mListVisibleRoomTheme.Clear();

            for(int i = 0; i < GameInfo.Instance.GameTable.RoomThemes.Count; i++)
            {
                GameTable.RoomTheme.Param param = GameInfo.Instance.GameTable.RoomThemes[i];
                if (param.PreVisible == 0 && !GameInfo.Instance.IsRoomThema(param.ID))
                {
                    continue;
                }

                mListVisibleRoomTheme.Add(param);
            }

            return mListVisibleRoomTheme.Count;
        }
        else if (mEnvMenuType == eEnvMenuType.Effect)
        {
            return mListAllRoomFunc.Count;
        }

        return 0;
    }

    private void UpdateEnvSlot(int index, GameObject slotObj)
    {
        UIFigureRoomEnvMenuListSlot slot = slotObj.GetComponent<UIFigureRoomEnvMenuListSlot>();
        slot.ParentGO = gameObject;

        if (mEnvMenuType == eEnvMenuType.Theme)
        {
            slot.UpdateThemeSlot(index, mListVisibleRoomTheme[index], Data.TableID);
        }
        else
        {
            slot.UpdateEffectSlot(this, index, mListAllRoomFunc[index], Data, Data.TableID);
        }
    }

    private bool OnTabFigure(int nSelect, SelectEvent type)
    {
        if (nSelect == 0)
            mIconSlotType = eContentsPosKind.COSTUME;
        else if (nSelect == 1)
            mIconSlotType = eContentsPosKind.MONSTER;

        listIconSlot.gameObject.SetActive(true);
        
        Renewal(true);
        return true;
    }

    private bool OnTabEquipment(int nSelect, SelectEvent type)
    {
        mIconSlotType = eContentsPosKind.WEAPON;
        mSelectedWeaponCharId = nSelect + 1;

        listIconSlot.gameObject.SetActive(true);
        
        Renewal(true);
        return true;
    }

    private int GetIconSlotItemCount()
    {
        int count = 0;

        if (mIconSlotType == eContentsPosKind.COSTUME)
        {
            count = mListAllFigureChar.Count;
        }
        else if (mIconSlotType == eContentsPosKind.MONSTER)
        {
            count = mListAllFigureMob.Count;
        }
        else if (mIconSlotType == eContentsPosKind.WEAPON)
        {
            count = mListCharWeapon.Count;
        }

        return count;
    }

    private void UpdateIconSlot(int index, GameObject slotObj)
    {
        UIFigureRoomIconListSlot slot = slotObj.GetComponent<UIFigureRoomIconListSlot>();
        slot.ParentGO = gameObject;
        
        RoomThemeSlotData Data = GameInfo.Instance.GetRoomThemeSlotData(GameInfo.Instance.UserData.RoomThemeSlot);

        if (mIconSlotType == eContentsPosKind.COSTUME)
        {
            slot.UpdateSlot(index, Data, mListAllFigureChar[index]);
        }
        else if (mIconSlotType == eContentsPosKind.MONSTER)
        {
            slot.UpdateSlot(index, Data, mListAllFigureMob[index]);
        }
        else if (mIconSlotType == eContentsPosKind.WEAPON)
        {
            slot.UpdateSlot(index, Data, mListCharWeapon[index]);
        }
    }

    private int GetSettingSlotItemCount()
    {
        return mListSettingFigure.Count;
    }

    private void UpdateSettingSlot(int index, GameObject slotObj)
    {
        UIFigureRoomSetIconListSlot slot = slotObj.GetComponent<UIFigureRoomSetIconListSlot>();
        slot.ParentGO = gameObject;

        slot.UpdateSlot(index, mListSettingFigure[index]);
    }

    public void OnBtnHideUI()
    {
        LobbyUIManager.Instance.HideUI("TopPanel", false);

        LobbyUIManager.Instance.EnableBlockUI(true, mAni["RoomThemeUI_Hide"].length);
        mAni.Play("RoomThemeUI_Hide");
    }

    public void OnBtnShowUI()
    {
        LobbyUIManager.Instance.ShowUI("TopPanel", false);

        LobbyUIManager.Instance.EnableBlockUI(true, mAni["RoomThemeUI_Show"].length);
        mAni.Play("RoomThemeUI_Show");
    }

    public void OnBtnFreeLook()
    {
        ShowTopPanelOnDisable = false;
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.FIGUREROOM_FREELOOK);
    }

    public void OnBtnAlbum()
    {
        LobbyUIManager.Instance.ShowUI("FigureAlbumPopup", true);
    }

    public void OnBtnMenu()
    {
        mMenuOn = !mMenuOn;

        if (mMenuOn)
        {
            DisableSubTabs();
            tabBasic.DisableTab();

            sprMenuOpen.gameObject.SetActive(false);
            sprMenuClose.gameObject.SetActive(true);

            btnOpenSaveLoad.gameObject.SetActive(true);

            LobbyUIManager.Instance.EnableBlockUI(true, mAni["MainMenu_Show"].length);
            mAni.Play("MainMenu_Show");
        }
        else
        {
            sprMenuOpen.gameObject.SetActive(true);
            sprMenuClose.gameObject.SetActive(false);

            LobbyUIManager.Instance.EnableBlockUI(true, mAni["MainMenu_Hide"].length);
            mAni.Play("MainMenu_Hide");
        }
    }

    public void OnBtnOpenSaveLoad()
    {
        //  메뉴 닫기
        OnBtnMenu();
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.FIGUREROOM_SAVELOAD);
    }

    public void OnBtnBackToLobby()
    {
        MessagePopup.OKCANCEL(eTEXTID.OK, 3108, OnSaveAndBackToLooby, OnBackToLobby, true, true, true);
    }

    private void OnSaveAndBackToLooby()
    {
        int slotNum = 0;

        mFigureRoomScene.RoomFigureSlotList.Clear();
        for (int i = 0; i < mFigureRoomScene.ListFigureInfo.Count; i++)
        {
            var info = mFigureRoomScene.ListFigureInfo[i];
            if (info.data.placement == false)
            {
                continue;
            }

            info.figure.Save();
            var _DataArr = info.figure.SaveData.ToBytes();

            RoomThemeFigureSlotData _figureSlotData = new RoomThemeFigureSlotData(info.data.tableId);
            
            _figureSlotData.Action1 = info.data.actionData == null ? 0 : info.data.actionData.tableId;
            //_figureSlotData.Action2 = _action2;
            _figureSlotData.SlotNum = slotNum;
            _figureSlotData.RoomThemeSlotNum = mFigureRoomScene.RoomSlotData.SlotNum;
            _figureSlotData.detailarry = _DataArr;

			_figureSlotData.CostumeStateFlag = info.data.CostumeStateFlag;
			GameTable.Costume.Param find = GameInfo.Instance.GameTable.FindCostume(x => x.ID == info.data.tableData.ContentsIndex);
			if (find != null && find.SubHairChange == 1)
			{
				bool isOn = GameSupport._IsOnBitIdx((uint)_figureSlotData.CostumeStateFlag, (int)(eCostumeStateFlag.CSF_HAIR));

				uint flag = (uint)_figureSlotData.CostumeStateFlag;
				GameSupport._DoOnOffBitIdx(ref flag, (int)(eCostumeStateFlag.CSF_HAIR), !isOn);

				_figureSlotData.CostumeStateFlag = (int)flag;
			}

			_figureSlotData.CostumeColor = info.data.CostumeColor;

            mFigureRoomScene.RoomFigureSlotList.Add(_figureSlotData);

            ++slotNum;
        }

        Debug.Log(mFigureRoomScene.RoomSlotData.TableID);

        mFigureRoomScene.RoomSlotData.ArrLightInfo = mFigureRoomScene.SaveLights();
        GameInfo.Instance.UseRoomThemeData = mFigureRoomScene.RoomSlotData.DeepCopy();

        GameInfo.Instance.RoomThemeFigureSlotList.Clear();
        for (int i = 0; i < mFigureRoomScene.RoomFigureSlotList.Count; i++)
        {
            GameInfo.Instance.RoomThemeFigureSlotList.Add(mFigureRoomScene.RoomFigureSlotList[i].DeepCopy());
        }

        GameInfo.Instance.Send_RoomThemeSlotSave(GameInfo.Instance.UseRoomThemeData, GameInfo.Instance.RoomThemeFigureSlotList, onNetRoomThemeSlotSave);
    }

    private void OnBackToLobby()
    {
        //로비로 이동 연출시에 메뉴를 꺼준다.
        DisableSubTabs();
        DisableMenuBtn();

        SoundManager.Instance.StopBgm();

        mFigureRoomScene.DestroyFigureListInfo();
        LobbyDoorPopup.Show(DoorToLobby);
    }

    void onNetRoomThemeSlotSave(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        Debug.LogWarning("Save Success");
        OnBackToLobby();
    }

    public void DoorToLobby()
    {
        SoundManager.Instance.PlayBgm(Lobby.Instance.GetCurrentBGMID(), "LobbyBGM", true);

        LobbyUIManager.Instance.SetUICameraMultiTouch(false);
        LobbyUIManager.Instance.ShowUI("TopPanel", false);

        Lobby.Instance.MoveToLobby();
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN);
    }

    public override bool IsBackButton()
    {
        if ((tabEnv != null && tabEnv.isActiveAndEnabled == true) ||
            (tabFigure != null && tabFigure.isActiveAndEnabled == true) ||
            (tabEquipment != null && tabEquipment.isActiveAndEnabled == true) ||
            (listEnvSlot != null && listEnvSlot.isActiveAndEnabled == true) ||
            (listIconSlot != null && listIconSlot.isActiveAndEnabled == true))
        {
            tabBasic.DisableTab();
            DisableMenuBtn();
            DisableSubTabs();

            return false;
        }
        else
        {
            OnBtnBackToLobby();
            return true;
        }
    }
}
