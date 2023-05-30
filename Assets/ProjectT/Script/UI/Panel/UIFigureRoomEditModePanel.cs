using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIFigureRoomEditModePanel : FComponent
{
    public enum eAniType
    {
        Expression = 0,
        Motion,
    }


    private static float SLIDER_VALUE_INTERVAL_BY_BTN = 0.01f;

    [Header("[Animation Menu]")]
    public FTab     tabAni;
    public FList    listAni;

    [Header("[Edit Menu]")]
    public UIButton         BtnRotate;
    public UIButton         BtnCloseRotate;
    public GameObject       RotateSliderObject;
    public TweenPosition    TweenPositionSlider = null;
    public UISlider         SliderXRotate;
    public UISlider         SliderYRotate;
    public UISlider         SliderZRotate;
    public UIButton         BtnLoadCamera;
    public UIButton         BtnReset;

    [Header("[Costume]")]
    public UIGrid       GridCostume;
    public UIButton     BtnHair;
    public UISprite     SprHairOn;
    public UIButton     BtnAccessory;
    public UISprite     SprAccessoryOn;
    public UIButton     BtnOtherParts;
    public UISprite     SprOtherPartsOn;
    public UISprite     SprOtherParts;
    public UIButton     BtnColor;
    public UILabel      LbColorCount;
    public UISprite     SprColorIcon;
    public GameObject   DyeObj;

    public UIFigureRoomJointLockListSlot    SelectedJoinHoldSlot        { get; private set; }
    public UIFigureRoomAniListSlot          SelectedFigureActionSlot    { get; private set; }

    private Animation   mAni        = null;
    private eAniType    mAniType    = eAniType.Expression;

    private FigureRoomScene                     mFigureRoomScene    = null;
    private FigureData                          mFigureData         = null;
    private FigureRoomScene.sFigureInfo         mSelectedFigureInfo = null;
    private List<GameTable.RoomAction.Param>    mListFigureAction   = new List<GameTable.RoomAction.Param>();

    private float       mBeforeXSliderValue = 0.0f;
    private float       mBeforeYSliderValue = 0.0f;
    private float       mBeforeZSliderValue = 0.0f;
    private bool        mbOpenSlider        = false;
    private Vector3     mInitSliderPosition = Vector3.zero;
    private Vector3     mOpenSliderPosition = Vector3.zero;
    private Vector3     mSaveCameraPosition = Vector3.zero;
    private Quaternion  mSaveCameraRotation = Quaternion.identity;

    private bool    mChangeHair         = false;
    private bool    mAttachAccessory    = false;
    private bool    mAttachOtherParts   = false;
    private int     mCostumeColorCount  = 1;
    private int     mCostumeColorIndex  = 0;
    private bool    mUseDye             = false;


    public override void Awake()
    {
        base.Awake();

        mAni = GetComponent<Animation>();

        tabAni.EventCallBack = OnTabAni;

        listAni.EventGetItemCount = GetAniListSlotCount;
        listAni.EventUpdate = UpdateAniSlot;

        mInitSliderPosition = RotateSliderObject.transform.localPosition;
        mOpenSliderPosition = mInitSliderPosition;
        mOpenSliderPosition.x = -mOpenSliderPosition.x;

        mSaveCameraPosition = Vector3.zero;
        mSaveCameraRotation = Quaternion.identity;
        BtnLoadCamera.gameObject.SetActive(false);
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

		UIFigureRoomPanel figureRoomPanel = LobbyUIManager.Instance.GetUI<UIFigureRoomPanel>("FigureRoomPanel");
        figureRoomPanel.ShowTopPanelOnDisable = true;
    }

    public override void InitComponent()
    {
        mFigureRoomScene = FigureRoomScene.Instance;
        mFigureData = mFigureRoomScene.SelectedFigureInfo.data;

        DisableAniUIs();

        if (mFigureData == null || mFigureData.tableData == null)
        {
            Debug.LogError("data is null");
        }

        mAniType = eAniType.Expression;
        if (mFigureData.tableData.ContentsType != (int)eContentsPosKind.COSTUME)
        {
            tabAni.gameObject.SetActive(false);
        }
        else
        {
            tabAni.gameObject.SetActive(true);
        }
            
        SelectedFigureActionSlot = null;

        mSelectedFigureInfo = mFigureRoomScene.SelectedFigureInfo;
        ShowRotateSlideBar();

        SelectedJoinHoldSlot = null;

        //DisableRotateUI();
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        mListFigureAction.Clear();
        mListFigureAction.AddRange(GameInfo.Instance.GameTable.FindAllRoomAction(x => x.CharacterID == mFigureData.tableData.CharacterID && x.Type == 1));

        if (listAni.gameObject.activeSelf)
        {
			listAni.UpdateList();
            listAni.RefreshNotMove();
            listAni.ScrollPositionSet();
        }

        bool hasHair = false, hasAccessory = false, hasOtherParts = false, hasColor = false;
        mUseDye = mSelectedFigureInfo.figure.GetAttachObjectsInfo(ref hasHair, ref hasAccessory, ref hasOtherParts, ref hasColor, ref mCostumeColorCount);

        BtnHair.gameObject.SetActive(hasHair);
        BtnAccessory.gameObject.SetActive(hasAccessory);
        BtnOtherParts.gameObject.SetActive(hasOtherParts);

        if( mFigureData.tableData.CharacterID == (int)ePlayerCharType.Sora ) {
            SprOtherParts.spriteName = "ico_Costume_Fox";
        }
        else {
            SprOtherParts.spriteName = "ico_FigureRoom_Fig";
        }

        mChangeHair = IsOnCostumeStateFlag(mFigureData.CostumeStateFlag, eCostumeStateFlag.CSF_HAIR);
        mAttachAccessory = IsOnCostumeStateFlag(mFigureData.CostumeStateFlag, eCostumeStateFlag.CSF_ATTACH_1);
        mAttachOtherParts = IsOnCostumeStateFlag(mFigureData.CostumeStateFlag, eCostumeStateFlag.CSF_ATTACH_2);

        if (hasHair)
        {
            SprHairOn.gameObject.SetActive(!mChangeHair);
        }

        if (hasAccessory)
        {
            SprAccessoryOn.gameObject.SetActive(!mAttachAccessory);
        }

        if(hasOtherParts)
        {
            SprOtherPartsOn.gameObject.SetActive(mAttachOtherParts);
        }

        BtnColor.gameObject.SetActive(hasColor);
        mCostumeColorIndex = mFigureData.CostumeColor;
        LbColorCount.textlocalize = (mCostumeColorIndex + 1).ToString();

        bool activeDyeObj = mUseDye && mCostumeColorIndex >= (mCostumeColorCount - 1);
        SprColorIcon.gameObject.SetActive(!activeDyeObj);
        DyeObj.gameObject.SetActive(activeDyeObj);

        GridCostume.Reposition();

        mSelectedFigureInfo.figure.SetShadowCommandBufferDirty();
        mSelectedFigureInfo.figure.IsDyeing = activeDyeObj;
    }

    /*public void DisableRotateUI()
    {
        BtnRotate.gameObject.SetActive(false);
        RotateSliderObject.SetActive(false);

        BtnCloseRotate.gameObject.SetActive(false);
    }*/

    public void SetRotateMode()
    {
        BtnRotate.gameObject.SetActive(false);
        RotateSliderObject.SetActive(true);

        //BtnCloseRotate.gameObject.SetActive(true);

        mBeforeXSliderValue = 0.5f;
        SliderXRotate.value = mBeforeXSliderValue;

        mBeforeYSliderValue = 0.5f;
        SliderYRotate.value = mBeforeYSliderValue;

        mBeforeZSliderValue = 0.5f;
        SliderZRotate.value = mBeforeZSliderValue;
    }

    public void CloseRotateMode()
    {
        BtnRotate.gameObject.SetActive(true);

        RotateSliderObject.transform.localPosition = mInitSliderPosition;
        TweenPositionSlider.ResetToBeginning();
        TweenPositionSlider.gameObject.SetActive(false);
        mbOpenSlider = false;

        RotateSliderObject.SetActive(false);

        BtnCloseRotate.gameObject.SetActive(false);
    }

    public void OnSelectFigureActionSlot(UIFigureRoomAniListSlot slot)
    {
        if (SelectedFigureActionSlot != null)
        {
            SelectedFigureActionSlot.Unselect();
        }

        SelectedFigureActionSlot = slot;
        SelectedFigureActionSlot.Select();

        mSelectedFigureInfo.data.ChangeFigureActionData(SelectedFigureActionSlot.param.ID);

        if (SelectedFigureActionSlot.param.Type == 1)
        {
            mFigureRoomScene.PlayFaceAni(SelectedFigureActionSlot.param.Action, mSelectedFigureInfo.figure, 0, SelectedFigureActionSlot.param.Weight);
            mFigureRoomScene.PlayFaceAni(SelectedFigureActionSlot.param.Action2, mSelectedFigureInfo.figure, 1, SelectedFigureActionSlot.param.Weight2);
        }
    }

    public void OnUnselectFigureActionSlot(UIFigureRoomAniListSlot slot)
    {
        slot.Unselect();
        mSelectedFigureInfo.data.ChangeFigureActionData(0);

        if (slot.param.Type == 1)
        {
            mFigureRoomScene.PlayFaceAni(eFaceAnimation.FaceIdle.ToString(), mSelectedFigureInfo.figure, 0, 1.0f);
            mFigureRoomScene.PlayFaceAni(eFaceAnimation.MouthIdle.ToString(), mSelectedFigureInfo.figure, 1, 0.0f);
        }

        SelectedFigureActionSlot = null;
    }

    public void OnBtnBack(bool bisHome = false)
    {
        OnClose();

        mFigureRoomScene.SaveFigureData();
        mFigureRoomScene.EndEditMode();
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

    public void OnBtnScreenShot()
    {
        DisableAniUIs();
        //DisableRotateUI();

        mFigureRoomScene.ShowAllPlacedSelectObject(false);
        mFigureRoomScene.ShowAllPlacedFigure(true);
        mFigureRoomScene.ShowLights(false, false);

        mFigureRoomScene.ScreenShot();
    }

    public void OnPostScreenShot()
    {
        if (mSelectedFigureInfo != null)
        {
            mSelectedFigureInfo.figure.ShowAllSelectObject(true);
            mFigureRoomScene.ShowPlacedUnselectFigure(true);
        }

        mFigureRoomScene.ShowLights(true, true);
    }

    public void OnBtnReset()
    {
        if (SelectedFigureActionSlot)
        {
            SelectedFigureActionSlot.Unselect();
            SelectedFigureActionSlot = null;
        }

        mSelectedFigureInfo.data.ChangeFigureActionData(0);

        mFigureRoomScene.PlayFaceAni(eFaceAnimation.FaceIdle.ToString(), mSelectedFigureInfo.figure, 0, 1.0f);
        mFigureRoomScene.PlayFaceAni(eFaceAnimation.MouthIdle.ToString(), mSelectedFigureInfo.figure, 1, 0.0f);

        mFigureRoomScene.ResetSelectedFigureJoints();

        mFigureRoomScene.ResetAllLights();

        DisableAniUIs();
        //DisableRotateUI();
    }

    public void OnBtnSaveCamera()
    {
        mSaveCameraPosition = mFigureRoomScene.RoomCamera.transform.position;
        mSaveCameraRotation = mFigureRoomScene.RoomCamera.transform.rotation;

        BtnLoadCamera.gameObject.SetActive(true);
    }

    public void OnBtnLoadCamera()
    {
        if (mSaveCameraPosition == Vector3.zero)
        {
            return;
        }

        mFigureRoomScene.RoomCamera.transform.SetPositionAndRotation(mSaveCameraPosition, mSaveCameraRotation);
    }

    public void OnBtnHair()
    {
        mChangeHair = !mChangeHair;
        SetCostumeBody();
    }

    public void OnBtnAccessory()
    {
        mAttachAccessory = !mAttachAccessory;
        SetCostumeBody();
    }

    public void OnBtnOtherParts()
    {
        mAttachOtherParts = !mAttachOtherParts;
        SetCostumeBody();
    }

    public void OnBtnColor()
    {
        ++mCostumeColorIndex;
        if(mCostumeColorIndex >= mCostumeColorCount)
        {
            mCostumeColorIndex = 0;
        }

        bool activeDyeObj = mUseDye && mCostumeColorIndex >= (mCostumeColorCount - 1);
        mSelectedFigureInfo.figure.IsDyeing = activeDyeObj;

        SetCostumeBody();
        LbColorCount.textlocalize = (mCostumeColorIndex + 1).ToString();

        SprColorIcon.gameObject.SetActive(!activeDyeObj);
        DyeObj.gameObject.SetActive(activeDyeObj);
    }

    public void OnBtnSave()
    {
        mFigureRoomScene.SaveSelectedFigureJoints();
    }

    public void OnBtnLoad()
    {
        mFigureRoomScene.LoadSelectedFigureJoints();
    }

    public void ShowRotateSlideBar(bool resetSliderValue = true)
    {
        if (resetSliderValue)
        {
            OnBtnRotate();
        }
        else
        {
            if (mFigureRoomScene.SelectedLightObjInfo != null)
            {
                mFigureRoomScene.SelectedLightObjInfo.ShowRotAxis(true);
            }
            else
            {
                if (mFigureRoomScene.SelectedBoneData != null)
                {
                    mFigureRoomScene.SelectedBoneData.SelectRotAxis();
                }
                else
                {
                    mFigureRoomScene.SelectedFigureInfo.figure.SelectBodyRotateAxis();
                }
            }
        }

        mbOpenSlider = false;
        OnBtnOpenSlider();

        BtnRotate.gameObject.SetActive(false);
        BtnCloseRotate.gameObject.SetActive(false);
    }

    /*public void ShowBtnRotate()
    {
        BtnRotate.gameObject.SetActive(true);
        CloseRotateMode();
    }*/

    public void OnBtnRotate()
    {
        SetRotateMode();

        if (mFigureRoomScene.SelectedLightObjInfo != null)
        {
            mFigureRoomScene.SelectedLightObjInfo.ShowRotAxis(true);
        }
        else
        {
            if (mFigureRoomScene.SelectedBoneData != null)
            {
                mFigureRoomScene.SelectedBoneData.SelectRotAxis();
            }
            else if (mFigureRoomScene.SelectedFigureInfo != null)
            {
                mFigureRoomScene.SelectedFigureInfo.figure.SelectBodyRotateAxis();
            }
        }
    }

    public void OnBtnCloseRotate()
    {
        ShowRotateSlideBar();
        /*CloseRotateMode();

        if (mFigureRoomScene.SelectedBoneData != null)
        {
            mFigureRoomScene.SelectedBoneData.Select();
        }
        else
        {
            mFigureRoomScene.SelectedFigureInfo.figure.UnselectBodyRotateAxis();
        }*/
    }

    public void OnSliderXRotate()
    {
        if (mFigureRoomScene.SelectedLightObjInfo != null)
        {
            mFigureRoomScene.RotateLightObj(Vector3.up, (SliderXRotate.value - mBeforeXSliderValue) * 360.0f);
        }
        else
        {
            if (mFigureRoomScene.SelectedBoneData != null)
            {
                mFigureRoomScene.RotateSelectedBone(Vector3.up, (SliderXRotate.value - mBeforeXSliderValue) * 360.0f);
                mFigureRoomScene.SelectedBoneData.BoneSaveData.RotY = mFigureRoomScene.SelectedBoneData.Bone.rotation.eulerAngles.y;
            }
            else
            {
                mFigureRoomScene.RotateBody(Vector3.right, (SliderXRotate.value - mBeforeXSliderValue) * 360.0f);
            }
        }

        mBeforeXSliderValue = SliderXRotate.value;
    }

    public void OnBtnXRotateLeft()
    {
        SliderXRotate.value = Mathf.Clamp(SliderXRotate.value - SLIDER_VALUE_INTERVAL_BY_BTN, 0.0f, 1.0f);
    }

    public void OnBtnXRotateRight()
    {
        SliderXRotate.value = Mathf.Clamp(SliderXRotate.value + SLIDER_VALUE_INTERVAL_BY_BTN, 0.0f, 1.0f);
    }

    public void OnSliderYRotate()
    {
        if (mFigureRoomScene.SelectedLightObjInfo != null)
        {
            mFigureRoomScene.RotateLightObj(Vector3.right, -(SliderYRotate.value - mBeforeYSliderValue) * 360.0f);
        }
        else
        {
            if (mFigureRoomScene.SelectedBoneData != null)
            {
                mFigureRoomScene.RotateSelectedBone(Vector3.right, -(SliderYRotate.value - mBeforeYSliderValue) * 360.0f);
                mFigureRoomScene.SelectedBoneData.BoneSaveData.RotX = mFigureRoomScene.SelectedBoneData.Bone.rotation.eulerAngles.x;
            }
            else
            {
                mFigureRoomScene.RotateBody(Vector3.up, -(SliderYRotate.value - mBeforeYSliderValue) * 360.0f);
            }
        }

        mBeforeYSliderValue = SliderYRotate.value;
    }

    public void OnBtnYRotateLeft()
    {
        SliderYRotate.value = Mathf.Clamp(SliderYRotate.value - SLIDER_VALUE_INTERVAL_BY_BTN, 0.0f, 1.0f);
    }

    public void OnBtnYRotateRight()
    {
        SliderYRotate.value = Mathf.Clamp(SliderYRotate.value + SLIDER_VALUE_INTERVAL_BY_BTN, 0.0f, 1.0f);
    }

    public void OnSliderZRotate()
    {
        if (mFigureRoomScene.SelectedLightObjInfo != null)
        {
            mFigureRoomScene.RotateLightObj(Vector3.forward, (SliderZRotate.value - mBeforeZSliderValue) * 360.0f);
        }
        else
        {
            if (mFigureRoomScene.SelectedBoneData != null)
            {
                mFigureRoomScene.RotateSelectedBone(Vector3.forward, (SliderZRotate.value - mBeforeZSliderValue) * 360.0f);
                mFigureRoomScene.SelectedBoneData.BoneSaveData.RotZ = mFigureRoomScene.SelectedBoneData.Bone.rotation.eulerAngles.z;
            }
            else
            {
                mFigureRoomScene.RotateBody(Vector3.forward, (SliderZRotate.value - mBeforeZSliderValue) * 360.0f);
            }
        }

        mBeforeZSliderValue = SliderZRotate.value;
    }

    public void OnBtnZRotateLeft()
    {
        SliderZRotate.value = Mathf.Clamp(SliderZRotate.value - SLIDER_VALUE_INTERVAL_BY_BTN, 0.0f, 1.0f);
    }

    public void OnBtnZRotateRight()
    {
        SliderZRotate.value = Mathf.Clamp(SliderZRotate.value + SLIDER_VALUE_INTERVAL_BY_BTN, 0.0f, 1.0f);
    }

    public void OnBtnOpenSlider()
    {
        mbOpenSlider = !mbOpenSlider;

        if (mbOpenSlider)
        {
            TweenPositionSlider.gameObject.SetActive(true);
            TweenPositionSlider.PlayForward();

            if (mFigureRoomScene.SelectedLightObjInfo != null)
            {
                mFigureRoomScene.SelectedLightObjInfo.ShowRotAxis(false);
            }
            else
            {
                if (mFigureRoomScene.SelectedBoneData != null)
                {
                    mFigureRoomScene.SelectedBoneData.Select();
                }
                else if (mFigureRoomScene.SelectedFigureInfo != null)
                {
                    mFigureRoomScene.SelectedFigureInfo.figure.UnselectBodyRotateAxis();
                }
            }
        }
        else
        {
            //BtnCloseRotate.gameObject.SetActive(true);
            TweenPositionSlider.PlayReverse();

            if (mFigureRoomScene.SelectedLightObjInfo != null)
            {
                mFigureRoomScene.SelectedLightObjInfo.ShowRotAxis(true);
            }
            else
            {
                if (mFigureRoomScene.SelectedBoneData != null)
                {
                    mFigureRoomScene.SelectedBoneData.SelectRotAxis();
                }
                else
                {
                    mFigureRoomScene.SelectedFigureInfo.figure.SelectBodyRotateAxis();
                }
            }
        }
    }

    private void Update()
    {
        if(AppMgr.Instance.CustomInput.IsOverUI() || mFigureRoomScene.EditType == FigureRoomScene.eEditType.None)
        {
            return;
        }

        if (mFigureRoomScene.EditType == FigureRoomScene.eEditType.EditWaiting)
        {
            if(AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Select))
            {
                RaycastHit hitInfo;

                Ray ray = mFigureRoomScene.RoomCamera.camera.ScreenPointToRay(AppMgr.Instance.CustomInput.GetTouchPos());
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, (1 << (int)eLayer.Pick)))
                {
                    mFigureRoomScene.UnselectLightObj();

                    RotateAxis selectedRotAxis = hitInfo.transform.GetComponentInParent<RotateAxis>();
                    if (selectedRotAxis)
                    {
                        if (!selectedRotAxis.name.Contains("_Body"))
                        {
                            mFigureRoomScene.SelectRotAxis(selectedRotAxis, hitInfo.transform);
                        }
                        else
                        {
                            mFigureRoomScene.SelectFigureRotAxis(selectedRotAxis, hitInfo.transform);
                        }
                    }
                    else
                    {
						string[] split = Utility.Split(hitInfo.transform.name, '_'); //hitInfo.transform.name.Split('_');
						if (split.Length < 2)
                        {
                            return;
                        }

                        FigureUnit.eEditableBoneType selectBoneType = mFigureRoomScene.SelectedFigureInfo.figure.FindBoneTypeByName(split[1]);
                        if (selectBoneType != FigureUnit.eEditableBoneType.None)
                        {
                            bool resetSliderValue = mFigureRoomScene.SelectedBoneType != selectBoneType;
                            if (mFigureRoomScene.SelectBone(selectBoneType))
                            {
                                ShowRotateSlideBar(resetSliderValue);
                            }
                        }
                    }
                }
                else if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, (1 << (int)eLayer.Figure)))
                {
                    mFigureRoomScene.UnselectLightObj();

                    FigureUnit figure = hitInfo.collider.GetComponentInParent<FigureUnit>();

                    if (figure == mSelectedFigureInfo.figure)
                    {
                        bool resetSliderValue = mFigureRoomScene.SelectedBoneData != null;

                        mFigureRoomScene.SelectFigure();
                        ShowRotateSlideBar(resetSliderValue);
                    }
                    else
                    {
                        FigureRoomScene.sFigureInfo figureInfo = mFigureRoomScene.ListFigureInfo.Find(x => x.figure == figure);
                        if (figureInfo != null)
                        {
                            mSelectedFigureInfo = figureInfo;
                            mFigureRoomScene.SetEditMode(figureInfo.data);

                            mFigureRoomScene.ShowPlacedUnselectFigure(true);
                            ShowRotateSlideBar();

                            InitComponent();
                            Renewal(true);
                        }
                    }
                }
                else
                {
                    DisableAniUIs();

                    if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, (1 << (int)eLayer.HitBox))) // 라이트 오브젝트
                    {
                        RotateAxis selectedRotAxis = hitInfo.transform.GetComponentInParent<RotateAxis>();
                        if (selectedRotAxis)
                        {
                            mFigureRoomScene.SelectLightRotAxis(selectedRotAxis, hitInfo.transform);
                        }
                        else
                        {
                            if (mFigureRoomScene.SelectedBoneData != null)
                            {
                                mFigureRoomScene.SelectedBoneData.Unselect();
                            }
                            else if (mFigureRoomScene.SelectedFigureInfo != null)
                            {
                                mFigureRoomScene.SelectedFigureInfo.figure.UnselectAll();
                            }

                            mFigureRoomScene.SelectLightObj(hitInfo.collider.gameObject);
                            ShowRotateSlideBar();
                        }
                    }
                }
            }
        }
        else if(mFigureRoomScene.EditType == FigureRoomScene.eEditType.FigurePosition)
        {
            if (AppMgr.Instance.CustomInput.GetButtonUp(BaseCustomInput.eKeyKind.Select))
            {
                mFigureRoomScene.SetEditType(FigureRoomScene.eEditType.EditWaiting);
            }
            else
            {
                mFigureRoomScene.EditingPosition();
            }
        }
        else if (mFigureRoomScene.EditType == FigureRoomScene.eEditType.FigureRotation)
        {
            if (AppMgr.Instance.CustomInput.GetButtonUp(BaseCustomInput.eKeyKind.Select))
            {
                mFigureRoomScene.SetEditType(FigureRoomScene.eEditType.EditWaiting);
            }
            else
            {
                mFigureRoomScene.EditingRotation();
            }
        }
        else if (mFigureRoomScene.EditType == FigureRoomScene.eEditType.FigureBonePosition)
        {
            if (AppMgr.Instance.CustomInput.GetButtonUp(BaseCustomInput.eKeyKind.Select))
            {
                mFigureRoomScene.SetEditType(FigureRoomScene.eEditType.EditWaiting);
            }
            else
            {
                mFigureRoomScene.EditingBonePosition();
            }
        }
        else if (mFigureRoomScene.EditType == FigureRoomScene.eEditType.FigureBoneRotation)
        {
            if (AppMgr.Instance.CustomInput.GetButtonUp(BaseCustomInput.eKeyKind.Select))
            {
                mFigureRoomScene.SetEditType(FigureRoomScene.eEditType.EditWaiting);
            }
            else
            {
                mFigureRoomScene.EditingBoneRotation();
            }
        }
        else if(mFigureRoomScene.EditType == FigureRoomScene.eEditType.LightPosition)
        {
            if (AppMgr.Instance.CustomInput.GetButtonUp(BaseCustomInput.eKeyKind.Select))
            {
                mFigureRoomScene.SetEditType(FigureRoomScene.eEditType.EditWaiting);
            }
            else
            {
                mFigureRoomScene.EditingLightPosition();
            }
        }
        else if (mFigureRoomScene.EditType == FigureRoomScene.eEditType.LightRotation)
        {
            if (AppMgr.Instance.CustomInput.GetButtonUp(BaseCustomInput.eKeyKind.Select))
            {
                mFigureRoomScene.SetEditType(FigureRoomScene.eEditType.EditWaiting);
            }
            else
            {
                mFigureRoomScene.EditingLightRotation();
            }
        }
        else if (AppMgr.Instance.CustomInput.GetButtonUp(BaseCustomInput.eKeyKind.Select))
        {
            DisableAniUIs();
        }

        float y = AppMgr.Instance.CustomInput.GetYAxis();
        AppMgr.Instance.CustomInput.LockCursor = false;

        if (y != 0.0f)
        {
            if (AppMgr.Instance.CustomInput.GetButton(BaseCustomInput.eKeyKind.FigureRotX))
            {
                AppMgr.Instance.CustomInput.LockCursor = true;

                if (y > 0.0f)
                {
                    OnBtnXRotateLeft();
                }
                else
                {
                    OnBtnXRotateRight();
                }
            }
            else if (AppMgr.Instance.CustomInput.GetButton(BaseCustomInput.eKeyKind.FigureRotY))
            {
                AppMgr.Instance.CustomInput.LockCursor = true;

                if (y > 0.0f)
                {
                    OnBtnYRotateLeft();
                }
                else
                {
                    OnBtnYRotateRight();
                }
            }
            else if (AppMgr.Instance.CustomInput.GetButton(BaseCustomInput.eKeyKind.FigureRotZ))
            {
                AppMgr.Instance.CustomInput.LockCursor = true;

                if (y > 0.0f)
                {
                    OnBtnZRotateLeft();
                }
                else
                {
                    OnBtnZRotateRight();
                }
            }
        }
    }

    private void DisableAniUIs()
    {
		mFigureRoomScene.RoomCamera.LockCamera(false);
	
		tabAni.DisableTab();
        listAni.gameObject.SetActive(false);
	}

    private int GetAniListSlotCount()
    {
        return mListFigureAction.Count;
    }

    private void UpdateAniSlot(int index, GameObject slotObj)
    {
        UIFigureRoomAniListSlot slot = slotObj.GetComponent<UIFigureRoomAniListSlot>();
        slot.ParentGO = gameObject;

        if (mFigureRoomScene.SelectedFigureInfo.data.actionData != null && mFigureRoomScene.SelectedFigureInfo.data.actionData.tableId == mListFigureAction[index].ID)
        {
            SelectedFigureActionSlot = slot;
        }
            
        slot.UpdateSlot(index, mFigureRoomScene.RoomSlotData, mFigureRoomScene.SelectedFigureInfo.data, mListFigureAction[index]);
    }

    private bool OnTabAni(int nSelect, SelectEvent type)
    {
        if (!GameInfo.Instance.HasFigure(mFigureData.tableId))
            return false;

        mAniType = (eAniType)nSelect;

        mListFigureAction.Clear();
        mListFigureAction.AddRange(GameInfo.Instance.GameTable.FindAllRoomAction(x => x.CharacterID == mFigureData.tableData.CharacterID && x.Type == nSelect + 1));

		mFigureRoomScene.RoomCamera.LockCamera(true);

		listAni.gameObject.SetActive(true);
        listAni.UpdateList();

        return true;
    }

    private void SetCostumeBody()
    {
        uint flag = 0;

        GameSupport._DoOnOffBitIdx(ref flag, (int)eCostumeStateFlag.CSF_HAIR, mChangeHair);
        GameSupport._DoOnOffBitIdx(ref flag, (int)eCostumeStateFlag.CSF_ATTACH_1, mAttachAccessory);
        GameSupport._DoOnOffBitIdx(ref flag, (int)eCostumeStateFlag.CSF_ATTACH_2, mAttachOtherParts);

        mSelectedFigureInfo.figure.SetCostumeBody(mCostumeColorIndex, (int)flag);

        mSelectedFigureInfo.data.CostumeColor = mCostumeColorIndex;
        mSelectedFigureInfo.data.CostumeStateFlag = (int)flag;

        Renewal(true);
    }

    private bool IsOnCostumeStateFlag(int costumeStateFlag, eCostumeStateFlag flag)
    {
        int _flagIdx = (int)flag;
        if (32 <= _flagIdx)
            return false;
        int checkFlag = 0x00000001 << _flagIdx;
        return (checkFlag == (costumeStateFlag & (System.Int32)checkFlag));
    }
}
