
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public partial class FigureActionData
{
    public int                          tableId;
    public GameTable.RoomAction.Param   tableData;
    public bool                         play;


    public FigureActionData()
    {
    }

    public FigureActionData(FigureActionData data)
    {
        tableId = data.tableId;
        play = data.play;
        tableData = data.tableData;
    }
}

public partial class FigureData
{
    public int                          tableId;
    public GameTable.RoomFigure.Param   tableData;
    public bool                         placement;
    public Vector3                      pos;
    public Vector3                      rot;
    public FigureUnit.sSaveData         saveData;
    public FigureActionData             actionData;
    public int                          CostumeStateFlag;
    public int                          CostumeColor;
    public int                          roomThemeFigureSlotDataIndex;


    public FigureData()
    {
        saveData = new FigureUnit.sSaveData();
    }

    public FigureData(int tableId)
    {
        this.tableId = tableId;
        tableData = GameInfo.Instance.GameTable.FindRoomFigure(tableId);

        placement = false;
        pos = Vector3.zero;
        rot = Vector3.zero;

        saveData = new FigureUnit.sSaveData();
        actionData = null;

        CostumeStateFlag = 0;
        CostumeColor = 0;
    }

    public FigureData(FigureData data)
    {
        tableId = data.tableId;
        tableData = data.tableData;
        placement = data.placement;
        pos = data.pos;
        rot = data.rot;
        saveData = new FigureUnit.sSaveData(data.saveData);
        actionData = new FigureActionData(data.actionData);
        CostumeStateFlag = data.CostumeStateFlag;
        CostumeColor = data.CostumeColor;
    }

    public void Reset()
    {
        placement = false;
        pos = Vector3.zero;
        rot = Vector3.zero;

        saveData = new FigureUnit.sSaveData();
    }

    public void SetRoomThemeFigureSlotData(RoomThemeFigureSlotData roomfigureslotdata)
    {
        tableId = roomfigureslotdata.TableID;
        tableData = roomfigureslotdata.TableData;
        placement = true;
        saveData = new FigureUnit.sSaveData();
        if (roomfigureslotdata.detailarry != null && roomfigureslotdata.detailarry.Length > 0)
        {
            saveData.FromBytes(roomfigureslotdata.detailarry);
        }
        pos = saveData.Pos;
        rot = saveData.Rot;

        actionData = new FigureActionData();
        actionData.tableId = roomfigureslotdata.Action1;
        actionData.tableData = GameInfo.Instance.GameTable.FindRoomAction(actionData.tableId);
        actionData.play = true;

        roomThemeFigureSlotDataIndex = roomfigureslotdata.SlotNum;

        CostumeStateFlag = roomfigureslotdata.CostumeStateFlag;
        CostumeColor = roomfigureslotdata.CostumeColor;
    }

    public void ChangeFigureActionData(int tableId)
    {
        if (actionData == null)
        {
            actionData = new FigureActionData();
            actionData.tableId = tableId;
            actionData.tableData = GameInfo.Instance.GameTable.FindRoomAction(actionData.tableId);
        }
        else
        {
            actionData.tableId = tableId;
            actionData.tableData = GameInfo.Instance.GameTable.FindRoomAction(actionData.tableId);
        }
    }
}

public class sLightObjInfo
{
    public GameObject   ShowingObj;
    public Light        Light;
    public RotateAxis   RotAxis;

    private Vector3         mOriginPos          = Vector3.zero;
    private Quaternion      mOriginRot          = Quaternion.identity;
    private Color           mOriginColor        = Color.white;
    private float           mOriginIntensity    = 1.0f;
    private List<Renderer>  mListRenderer       = new List<Renderer>();


    public void Init(GameObject lightObj, Light defaultLight)
    {
        ShowingObj = lightObj;

        mOriginPos = defaultLight.transform.position;
        mOriginRot = defaultLight.transform.rotation;
        mOriginColor = defaultLight.color;
        mOriginIntensity = defaultLight.intensity;

        Light = lightObj.GetComponentInChildren<Light>();

        mListRenderer.Clear();
        mListRenderer.AddRange(lightObj.GetComponentsInChildren<Renderer>());

        // Rot Axis 오브젝트 생성
        RotAxis = ResourceMgr.Instance.CreateFromAssetBundle<RotateAxis>("unit", "Unit/FigureRotAxis.prefab");
        RotAxis.name = string.Format("RotateAxis_Light");
        RotAxis.transform.parent = ShowingObj.transform;
        Utility.InitTransform(RotAxis.gameObject, Vector3.zero, Quaternion.identity, Vector3.one * 8.0f);
        Utility.SetLayer(RotAxis.gameObject, (int)eLayer.HitBox, true, (int)eLayer.Default);

        Renderer[] renderers = RotAxis.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.renderQueue = 3002;
        }

        Reset();
    }

    public void Destroy()
    {
        GameObject.DestroyImmediate(ShowingObj.gameObject);
    }

    public void LoadFromBinary(BinaryReader br)
    {
        float x = br.ReadSingle();
        float y = br.ReadSingle();
        float z = br.ReadSingle();

        float rx = br.ReadSingle();
        float ry = br.ReadSingle();
        float rz = br.ReadSingle();

        // 라이트 색상 값 (지금은 사용 안함)
        float r = br.ReadSingle();
        float g = br.ReadSingle();
        float b = br.ReadSingle();

        // 라이트 세기 값 (지금은 사용 안함)
        float i = br.ReadSingle();

        ShowingObj.transform.SetPositionAndRotation(new Vector3(x, y, z), Quaternion.Euler(rx, ry, rz));
        Light.color = new Color(r, g, b);
        Light.intensity = i;
    }

    public void SaveToBinary(BinaryWriter bw)
    {
        bw.Write(ShowingObj.transform.position.x);
        bw.Write(ShowingObj.transform.position.y);
        bw.Write(ShowingObj.transform.position.z);

        bw.Write(ShowingObj.transform.eulerAngles.x);
        bw.Write(ShowingObj.transform.eulerAngles.y);
        bw.Write(ShowingObj.transform.eulerAngles.z);

        bw.Write(Light.color.r);
        bw.Write(Light.color.g);
        bw.Write(Light.color.b);

        bw.Write(Light.intensity);
    }

    public void Select()
    {
        for(int i = 0; i < mListRenderer.Count; i++)
        {
            if(!mListRenderer[i].material.HasProperty("_RimColor"))
            {
                continue;
            }

            mListRenderer[i].material.SetColor("_RimColor", new Color(1.0f, 1.0f, 1.0f, 1.0f));
        }
    }

    public void Unselect()
    {
        for (int i = 0; i < mListRenderer.Count; i++)
        {
            if (!mListRenderer[i].material.HasProperty("_RimColor"))
            {
                continue;
            }

            mListRenderer[i].material.SetColor("_RimColor", new Color(1.0f, 1.0f, 1.0f, 0.0f));
        }

        ShowRotAxis(false);
    }

    public void Reset()
    {
        ShowingObj.transform.SetPositionAndRotation(mOriginPos, mOriginRot);
        Light.color = mOriginColor;
        Light.intensity = mOriginIntensity;

        Unselect();
    }

    public void Show(bool show, bool withRotAxis)
    {
        for(int i = 0; i < mListRenderer.Count; i++)
        {
            mListRenderer[i].enabled = show;
        }

        if(!show && withRotAxis)
        {
            ShowRotAxis(false);
        }
    }

    public void ShowRotAxis(bool show)
    {
        RotAxis.Show(show);
    }
}


public class FigureRoomScene : MonoSingleton<FigureRoomScene>
{
    public enum eEditType
    {
        None = 0,

        EditWaiting,

        FigurePosition,
        FigureRotation,

        FigureBonePosition,
        FigureBoneRotation,

        LightPosition,
        LightRotation,
    }

    public enum eFigureRoomMode
    {
        EditMode,
        ARMode,
    }

    public class sFigureInfo
    {
        public FigureData data;
        public FigureUnit figure;


        public sFigureInfo(FigureData data, FigureUnit figure)
        {
            this.data = data;
            this.figure = figure;
        }
    }


    private static int      VERSION                 = 1; // 1.0
    private static string   LOCAL_SCREENSHOT_PATH   = "/FigureRoom/LocalSave/{0}/{1}/";
    private static string   TEMP_SCREENSHOT_PATH    = "/FigureRoom/Temp/{0}/{1}/";

    [Header("[Proprety]")]
    public LobbyCamera  RoomCamera;
    public Light        RoomDefaultLight;
    public Vector3      LightRotate = Vector3.zero;

    public eEditType                        EditType                { get; private set; }
    public eFigureRoomMode                  FigureRoomMode          { get; private set; }
    public List<sFigureInfo>                ListFigureInfo          { get; private set; }
    public sFigureInfo                      SelectedFigureInfo      { get; private set; }
    public FigureUnit.sEditableBoneData     SelectedBoneData        { get; private set; }
    public FigureUnit.eEditableBoneType     SelectedBoneType        { get; private set; } = FigureUnit.eEditableBoneType.None;
    public RoomThemeSlotData                RoomSlotData            { get; private set; }
    public List<RoomThemeFigureSlotData>    RoomFigureSlotList      { get; private set; } = new List<RoomThemeFigureSlotData>();
    public sLightObjInfo                    SelectedLightObjInfo    { get; private set; } = null;
    public string                           ScreenShotPath          { get { return mScreenShot.SavePath;} }

    private List<sLightObjInfo>                 mListLightObj               = new List<sLightObjInfo>();
    private Vector3                             mCurTouchPos                = Vector3.zero;
    private Vector3                             mBeforeTouchPos             = Vector3.zero;

    private Dictionary<string, FigureRoomFunc>  mDicRoomEffect              = new Dictionary<string, FigureRoomFunc>();
    private GameScreenShot                      mScreenShot                 = null;
    private bool                                mMakingScreenShot           = false;
    private bool                                mShowPlacedUnselectFigure   = false;

    private System.DateTime                     mStartDateTime;  // 들어온 시간
    private System.DateTime                     mEndDateTime;    // 나간 시간

    public void Init()
    {
        EditType = eEditType.None;

        mScreenShot = GetComponent<GameScreenShot>();
        mScreenShot.onScreenShot += OnPostScreenShot;

        LoadRoomFunctions();

        ListFigureInfo = new List<sFigureInfo>();
        if (GameSupport.IsFriendRoom())
            RoomSlotData = GameInfo.Instance.FriendRoomSlotData.DeepCopy();
        else
            RoomSlotData = GameInfo.Instance.UseRoomThemeData.DeepCopy();

        for(int i = 0; i < RoomSlotData.RoomThemeFuncList.Count; i++)
        {
            RoomThemeFuncData data = RoomSlotData.RoomThemeFuncList[i];
            if(data == null)
            {
                continue;
            }

            data.TableData = GameInfo.Instance.GameTable.FindRoomFunc(data.TableID);
            ActiveRoomFunc(data);
        }

        RoomFigureSlotList.Clear();
        if(GameSupport.IsFriendRoom())
        {
            for (int i = 0; i < GameInfo.Instance.FriendRoomFigureSlotList.Count; i++)
            {
                RoomFigureSlotList.Add(GameInfo.Instance.FriendRoomFigureSlotList[i].DeepCopy());
            }
        }
        else
        {
            for (int i = 0; i < GameInfo.Instance.UseRoomThemeFigureList.Count; i++)
            {
                RoomFigureSlotList.Add(GameInfo.Instance.UseRoomThemeFigureList[i].DeepCopy());
            }
        }
        

        mMakingScreenShot = false;
        mShowPlacedUnselectFigure = false;
        mStartDateTime = GameSupport.GetCurrentServerTime();

        if (GameSupport.IsFriendRoom())
            SoundManager.Instance.PlayBgm(GameInfo.Instance.FriendRoomSlotData.TableData.Bgm, 1f, true);
        else
            SoundManager.Instance.PlayBgm(GameInfo.Instance.UseRoomThemeData.TableData.Bgm, 1.0f, true);

        CreateLight();
    }

    public IEnumerator LoadFigures()
    {
        int selectedFigureInfoIndex = -1;
        ListFigureInfo.Clear();

        for (int i = 0; i < RoomFigureSlotList.Count; i++)
        {
            FigureData data = new FigureData();
            data.roomThemeFigureSlotDataIndex = i;
            data.SetRoomThemeFigureSlotData(RoomFigureSlotList[i]);

            FigureUnit figure = null;
            if (data.tableData.ContentsType == (int)eContentsPosKind.COSTUME || data.tableData.ContentsType == (int)eContentsPosKind.MONSTER)
            {
                figure = GameSupport.CreateFigure(data.tableId, null, true);
            }
            else if (data.tableData.ContentsType == (int)eContentsPosKind.WEAPON)
            {
                figure = GameSupport.CreateFigureWeapon(data.tableId);
            }

            if (figure == null)
            {
                continue;
            }

            figure.transform.position = Vector3.zero;
            figure.IsDyeing = data.saveData.UseDye;
            figure.Deactivate();

            if (data.placement)
            {
                figure.Activate();

                if (selectedFigureInfoIndex == -1)
                {
                    selectedFigureInfoIndex = i;
                }
            }

            ListFigureInfo.Add(new sFigureInfo(data, figure));
            yield return null;
        }

        for (int i = 0; i < ListFigureInfo.Count; i++)
        {
            sFigureInfo info = ListFigureInfo[i];
            SetFigureUnit(info);
        }

        RoomCamera.SetCameraType(LobbyCamera.eType.Normal, null);
    }

    public void LoadThemeFigureListByLocalData()
    {
        mMakingScreenShot = false;
        mShowPlacedUnselectFigure = false;

        LoadRoomFunctions();
        ResetRoomCamera();
    }

    public void DestroyFigureListInfo()
    {
        for (int i = 0; i < ListFigureInfo.Count; i++)
        {
            Destroy(ListFigureInfo[i].figure.gameObject);
        }

        ListFigureInfo.Clear();
    }

    public void PlacementFigure(int tableId, bool placement)
    {
        sFigureInfo find = ListFigureInfo.Find(x => x.data.tableId == tableId);
        if (find == null)
        {
            FigureData data = new FigureData(tableId);
            data.roomThemeFigureSlotDataIndex = RoomFigureSlotList.Count;

            FigureUnit figure = null;
            if (data.tableData.ContentsType == (int)eContentsPosKind.COSTUME || data.tableData.ContentsType == (int)eContentsPosKind.MONSTER)
            {
				uint costumeFlag = 0;

				GameTable.Costume.Param findCostume = GameInfo.Instance.GameTable.FindCostume(x => x.ID == data.tableData.ContentsIndex);
				if (findCostume != null && findCostume.SubHairChange == 1)
				{
					bool isOn = GameSupport._IsOnBitIdx(costumeFlag, (int)(eCostumeStateFlag.CSF_HAIR));
					GameSupport._DoOnOffBitIdx(ref costumeFlag, (int)(eCostumeStateFlag.CSF_HAIR), true);

					data.CostumeStateFlag = (int)costumeFlag;
				}

				figure = GameSupport.CreateFigure(tableId, null, true);
            }
            else if (data.tableData.ContentsType == (int)eContentsPosKind.WEAPON)
            {
                figure = GameSupport.CreateFigureWeapon(tableId);
            }

            if (figure == null)
            {
                Debug.Log("FigureRoomScene::PlacementFigure : " + tableId + "번 피규어를 생성할 수 없습니다.");
                return;
            }
            else
            {
                find = new sFigureInfo(data, figure);
                SetFigureUnit(find);

                ListFigureInfo.Add(find);
            }
        }

        if (placement)
        {
            find.data.placement = true;
            find.figure.Activate();
        }
        else
        {
            find.data.Reset();
            find.data.ChangeFigureActionData(0);

            PlayFaceAni(eFaceAnimation.FaceIdle.ToString(), find.figure, 0, 1.0f);
            PlayFaceAni(eFaceAnimation.MouthIdle.ToString(), find.figure, 1, 0.0f);

            ResetFigureJoints(find);
            //find.figure.ResetFigure();

            find.figure.Deactivate();
        }
    }

    public void ShowAllPlacedSelectObject(bool show)
    {
        for (int i = 0; i < ListFigureInfo.Count; i++)
        {
            sFigureInfo info = ListFigureInfo[i];
            if (!info.data.placement)
            {
                continue;
            }

            info.figure.UnselectAll();
            info.figure.ShowAllSelectObject(show);
        }
    }

    public void ShowAllPlacedFigure(bool show)
    {
        for (int i = 0; i < ListFigureInfo.Count; i++)
        {
            sFigureInfo info = ListFigureInfo[i];
            if (!info.data.placement)
            {
                continue;
            }

            info.figure.SetColor(1.0f);

            if (show)
            {
                info.figure.Activate();
            }
            else
            {
                info.figure.Deactivate();
            }
        }
    }

    public void ShowPlacedUnselectFigure(bool show)
    {
        mShowPlacedUnselectFigure = show;

        for (int i = 0; i < ListFigureInfo.Count; i++)
        {
            sFigureInfo info = ListFigureInfo[i];
            if (info == SelectedFigureInfo || !info.data.placement)
            {
                continue;
            }

            if (show)
            {
                info.figure.SetColor(0.45f);
                info.figure.Activate();
            }
            else
            {
                info.figure.SetColor(1.0f);
                info.figure.Deactivate();
            }
        }
    }

    public bool SelectFigureInfo(FigureData figureData)
    {
        sFigureInfo find = ListFigureInfo.Find(x => x.data.tableId == figureData.tableId);
        if(find == null)
        {
            Debug.LogError("리스트에 없는 선택된 피규어입니다.");
            return false;
        }

        if(SelectedBoneData != null)
        {
            SelectedBoneData = null;
        }

        if(SelectedFigureInfo != null)
        {
            SelectedFigureInfo.figure.UnselectAll();
            SelectedFigureInfo.figure.ShowAllSelectObject(false);
        }

        SelectedFigureInfo = find;
        SelectedFigureInfo.figure.SetColor(1.0f);

        return true;
    }

    public void PlayFaceAni(string ani, FigureUnit unit, int layer, float weight)
    {
        if (unit == null)
            return;

        unit.PlayFigureFaceAni(Utility.GetFaceAnimationByString(ani), layer, weight);
    }

    public void ResetSelectedFigureJoints(bool resetRoom = true)
    {
        if (SelectedFigureInfo != null)
        {
            SelectedFigureInfo.figure.UnselectAll();
            ResetFigureJoints(SelectedFigureInfo);

            if (resetRoom)
            {
                ResetRoomCamera();
            }
        }
    }

    public void ResetFigureJoints(sFigureInfo figureInfo)
    {
        if (figureInfo == null)
        {
            return;
        }

        figureInfo.data.pos = Vector3.zero;
        figureInfo.figure.transform.position = Vector3.zero;

        figureInfo.data.rot = Vector3.zero;
        figureInfo.figure.transform.rotation = Quaternion.identity;

        figureInfo.figure.ResetFigure();
    }

    public void EndEditMode()
    {
        for (int i = 0; i < ListFigureInfo.Count; i++)
        {
            sFigureInfo info = ListFigureInfo[i];
            info.figure.ShowAllSelectObject(false);
        }

        ShowLights(false, true);
    }

    public void SaveFigureData()
    {
        if (SelectedFigureInfo == null)
        {
            return;
        }

        SelectedFigureInfo.data.pos = SelectedFigureInfo.figure.transform.position;
        SelectedFigureInfo.data.rot = SelectedFigureInfo.figure.transform.rotation.eulerAngles;

        SelectedFigureInfo.figure.Save();
        SelectedFigureInfo.data.saveData = SelectedFigureInfo.figure.SaveData;
    }

    public void ResetRoomCamera()
    {
        if (ListFigureInfo.Count > 0)
        {
            FigureUnit target = ListFigureInfo[0].figure;
            if (SelectedFigureInfo != null)
                target = SelectedFigureInfo.figure;
        }

        RoomCamera.InitCameraTransform();
    }

    public Vector3 GetTouchPos(ref Vector3 beforeTouchPos, bool updateBeforeTouchPos)
    {
        Vector3 touchPos = Utility.Get3DPos(RoomCamera.camera, AppMgr.Instance.CustomInput.GetTouchPos(), RoomCamera.distance.z);

        if (updateBeforeTouchPos)
            beforeTouchPos = touchPos;

        return touchPos;
    }

    public void SaveSelectedFigureJoints()
    {
        if (SelectedFigureInfo == null)
        {
            return;
        }

        SelectedFigureInfo.figure.Save();
    }

    public void LoadSelectedFigureJoints()
    {
        if (SelectedFigureInfo == null)
        {
            return;
        }

        SelectedFigureInfo.figure.Load(SelectedFigureInfo.figure.SaveData);
    }

    public Texture2D TakeScreenShot()
    {
        return mScreenShot.TakeScreenShot();
    }

    public void SaveScreenShotToFile(string path, Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
    }

    public void ScreenShot()
    {
        if (mMakingScreenShot)
            return;

        UIFigureAlbumPopup popup = LobbyUIManager.Instance.GetUI<UIFigureAlbumPopup>("FigureAlbumPopup");
        if(popup && popup.IsFull())
        {
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(3154), OpenAlbumPopup);
            return;
        }

        mMakingScreenShot = true;
        StartCoroutine("StartScreenShot");
    }

    public void SetLobbyBGWithScreenShot()
    {
        if (mMakingScreenShot)
            return;

        mMakingScreenShot = true;
        StartCoroutine("CoSetLobbyBGWithScreenShot");
    }

    private IEnumerator CoSetLobbyBGWithScreenShot()
    {
        yield return new WaitForEndOfFrame();
        mScreenShot.OnSaveScreenShot();
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceUpdate);
#endif
        yield return new WaitForEndOfFrame();
        PlayerPrefs.SetString("LobbyBGWithScreenShot", "true");
        Lobby.Instance.SetBackGroundWithScreenShot();

        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3267));
    }

    private void OpenAlbumPopup()
    {
        LobbyUIManager.Instance.ShowUI("FigureAlbumPopup", true);
    }

    public void SaveScreenShot(string p_Path, string p_FileName, bool p_Local = true)
    {
        var uid = GameInfo.Instance.UserData.UUID;
        string path = Path.Combine(Application.persistentDataPath, string.Format(TEMP_SCREENSHOT_PATH, uid, p_Path));

        path = Path.Combine(path, p_FileName + ".png");

        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));

        Texture2D texScreen = FigureRoomScene.Instance.TakeScreenShot();
        FigureRoomScene.Instance.SaveScreenShotToFile(path, texScreen);
    }

    public Texture2D LoadScreenShotTexture(string p_Path, string p_FileName, ref System.DateTime p_DateTime, bool p_Local = true)
    {
        var uid = GameInfo.Instance.UserData.UUID;
        string path = Application.persistentDataPath + string.Format(TEMP_SCREENSHOT_PATH, uid, p_Path);
        if (p_Local)
            path = Application.persistentDataPath + string.Format(LOCAL_SCREENSHOT_PATH, uid, p_Path);

        path = path + p_FileName;
        if (System.IO.File.Exists(path))
        {
            Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            tex.LoadImage(System.IO.File.ReadAllBytes(path));
            p_DateTime = System.IO.File.GetCreationTime(path);
            return tex;
        }

        return null;
    }

    public void DeleteScreenShotFile(string p_Path, string p_FileName, bool p_Local = true)
    {
        var uid = GameInfo.Instance.UserData.UUID;
        string path = Application.persistentDataPath + string.Format(TEMP_SCREENSHOT_PATH, uid, p_Path);
        if (p_Local)
            path = Application.persistentDataPath + string.Format(LOCAL_SCREENSHOT_PATH, uid, p_Path);

        path = path + p_FileName;
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
    }

    private void LoadRoomFunctions()
    {
        mDicRoomEffect.Clear();

        FigureRoomFunc[] roomEffs = GetComponentsInChildren<FigureRoomFunc>(true);
        for(int i = 0; i < roomEffs.Length; i++)
        {
            roomEffs[i].Init();
            mDicRoomEffect.Add(roomEffs[i].name, roomEffs[i]);
        }
    }

    public void ActiveRoomFunc(RoomThemeFuncData data)
    {
        string key = data.TableData.Function;

        if (!mDicRoomEffect.ContainsKey(key))
        {
            return;
        }

        if (data.On)
        {
            if (data.TableData.GroupID > 0)
            {
                foreach (KeyValuePair<string, FigureRoomFunc> kv in mDicRoomEffect)
                {
                    if (kv.Key == key)
                    {
                        continue;
                    }

                    if (kv.Value.Param.GroupID == data.TableData.GroupID)
                    {
                        kv.Value.Deactivate();
                    }
                }
            }

            mDicRoomEffect[key].Activate();
        }
        else
        {
            mDicRoomEffect[key].Deactivate();
        }
    }

    private void SetFigureUnit(sFigureInfo info)
    {
        if (RoomFigureSlotList.Count > 0 && info.data.roomThemeFigureSlotDataIndex < RoomFigureSlotList.Count)
        {
            if (RoomFigureSlotList[info.data.roomThemeFigureSlotDataIndex].detailarry == null)
            {
                info.figure.Save();

                byte[] bytes = info.figure.SaveData.ToBytes();
                RoomFigureSlotList[info.data.roomThemeFigureSlotDataIndex].detailarry = bytes;
            }
            else
            {
                info.figure.Save();

                if (RoomFigureSlotList[info.data.roomThemeFigureSlotDataIndex].detailarry != null &&
                    RoomFigureSlotList[info.data.roomThemeFigureSlotDataIndex].detailarry.Length > 0)
                {
                    info.figure.SaveData.FromBytes(RoomFigureSlotList[info.data.roomThemeFigureSlotDataIndex].detailarry);
                }

                info.figure.Load(info.figure.SaveData);
            }
        }

        info.figure.transform.position = info.data.pos;
        info.figure.transform.rotation = Quaternion.Euler(info.data.rot);

        info.figure.InitCostume(info.data);

        if (info.data.actionData == null || info.data.actionData.tableId <= 0)
        {
            PlayFaceAni(eFaceAnimation.FaceIdle.ToString(), info.figure, 0, 1.0f);
            PlayFaceAni(eFaceAnimation.MouthIdle.ToString(), info.figure, 1, 0.0f);
        }
        else
        {
            PlayFaceAni(info.data.actionData.tableData.Action, info.figure, 0, info.data.actionData.tableData.Weight);
            PlayFaceAni(info.data.actionData.tableData.Action2, info.figure, 1, info.data.actionData.tableData.Weight2);
        }

        info.figure.Activate();
        info.figure.SetShadowPosByLightInPrivateRoom();
    }

    public bool SetEditMode(FigureData selectedFigureData)
    {
        if(SelectFigureInfo(selectedFigureData))
        {
            SetEditType(eEditType.EditWaiting);
            SelectedFigureInfo.figure.ShowAllSelectObject(true);

            ShowLights(true, true);
            return true;
        }

        return false;
    }

    public void SetEditType(eEditType type)
    {
        if (EditType == type)
        {
            return;
        }

        EditType = type;

        if (EditType == eEditType.EditWaiting)
        {
            RoomCamera.SetCameraType(LobbyCamera.eType.LookAtTarget, SelectedFigureInfo != null ? SelectedFigureInfo.figure.transform : null, false);
        }
        else
        {
            RoomCamera.SetCameraType(LobbyCamera.eType.FixedTarget, null);
        }
    }

    public void SelectFigure()
    {
        SetEditType(eEditType.FigurePosition);

        SelectedFigureInfo.figure.UnselectAll();
        SelectedBoneData = null;

        mCurTouchPos = mBeforeTouchPos = Get3DPosFromMousePosition(SelectedFigureInfo.figure.transform);
    }

    public void SelectFigureRotAxis(RotateAxis selectedRotAxis, Transform selectedAxis)
    {
        SetEditType(eEditType.FigureRotation);

        selectedRotAxis.SelectAxis(selectedAxis);
        mCurTouchPos = mBeforeTouchPos = AppMgr.Instance.CustomInput.GetTouchPos();
    }

    //Quaternion q = Quaternion.identity;
    public bool SelectBone(FigureUnit.eEditableBoneType boneType)
    {
        SetEditType(eEditType.FigureBonePosition);
        SelectedFigureInfo.figure.UnselectBodyRotateAxis();

        if (SelectedBoneData != null)
        {
            SelectedBoneData.Unselect();
        }

        SelectedBoneType = boneType;
        SelectedBoneData = SelectedFigureInfo.figure.GetSelectedBoneDataOrNull(boneType);

        if (SelectedBoneData == null)
        {
            return false;
        }
         
        SelectedBoneData.Select();
        mCurTouchPos = mBeforeTouchPos = Get3DPosFromMousePosition(SelectedBoneData.Target);

        //q = SelectedBoneData.Parent.OriginalRot;
        return true;
    }

    public void SelectRotAxis(RotateAxis selectedRotAxis, Transform selectedAxis)
    {
        SetEditType(eEditType.FigureBoneRotation);

        selectedRotAxis.SelectAxis(selectedAxis);
        mCurTouchPos = mBeforeTouchPos = AppMgr.Instance.CustomInput.GetTouchPos();
    }

    public void EditingPosition()
    {
        if(SelectedFigureInfo == null)
        {
            return;
        }

        mCurTouchPos = Get3DPosFromMousePosition(SelectedFigureInfo.figure.transform);
        Vector3 deltaPos = mCurTouchPos - mBeforeTouchPos;

        SelectedFigureInfo.figure.transform.position += deltaPos;
        mBeforeTouchPos = mCurTouchPos;
    }

    public void EditingRotation()
    {
        if (SelectedFigureInfo == null)
        {
            return;
        }

        mCurTouchPos = AppMgr.Instance.CustomInput.GetTouchPos();
        float angle = (((mCurTouchPos.x - mBeforeTouchPos.x) + (mCurTouchPos.y - mBeforeTouchPos.y))) * 0.35f;

        Vector3 axis = Vector3.zero;
        if (SelectedFigureInfo.figure.BodyRotateAxis.CurrentType == RotateAxis.eType.X)
        {
            axis = Vector3.up;
        }
        else if (SelectedFigureInfo.figure.BodyRotateAxis.CurrentType == RotateAxis.eType.Y)
        {
            axis = Vector3.right;
        }
        else if (SelectedFigureInfo.figure.BodyRotateAxis.CurrentType == RotateAxis.eType.Z)
        {
            axis = Vector3.forward;
        }

        RotateBody(axis, angle);
        mBeforeTouchPos = mCurTouchPos;
    }

    public void RotateBody(Vector3 axis, float angle)
    {
        if (SelectedFigureInfo == null)
        {
            return;
        }

        SelectedFigureInfo.figure.transform.Rotate(axis, angle);
    }

    public void EditingBonePosition()
    {
        if (SelectedBoneData == null || SelectedBoneData.Parent == null)
        {
            return;
        }

        mCurTouchPos = Get3DPosFromMousePosition(SelectedBoneData.Target);
        Vector3 deltaPos = mCurTouchPos - mBeforeTouchPos;

        Vector3 v1 = (mBeforeTouchPos - SelectedBoneData.Parent.Bone.position).normalized;
        Vector3 v2 = (mCurTouchPos - SelectedBoneData.Parent.Bone.position).normalized;
        Vector3 c = Vector3.Cross(v1, v2);

        Vector3 axis = SelectedBoneData.Parent.Bone.InverseTransformDirection(c);
        float angle = Vector3.SignedAngle(v1, v2, c);

        /*/==
        q *= Quaternion.AngleAxis(angle, c);

        float checkAngle = Quaternion.Angle(SelectedBoneData.Parent.OriginalRot, q);
        Log.Show("Check Angle : " + checkAngle, Log.ColorType.Red);
        
        if (checkAngle >= 15.0f)
        {
            mBeforeTouchPos = mCurTouchPos;
            return;
        }
        //==*/

        SelectedBoneData.Target.position += deltaPos;

        SelectedBoneData.Parent.Bone.Rotate(axis, angle);
        mBeforeTouchPos = mCurTouchPos;
    }

    public void EditingBoneRotation()
    {
        if (SelectedBoneData == null || SelectedBoneData.Bone == null || SelectedBoneData.RotAxis.CurrentType == RotateAxis.eType.None)
        {
            return;
        }

        mCurTouchPos = AppMgr.Instance.CustomInput.GetTouchPos();
        float angle = (((mCurTouchPos.x - mBeforeTouchPos.x) + (mCurTouchPos.y - mBeforeTouchPos.y))) * 0.35f;

        Vector3 axis = Vector3.zero;
        if (SelectedBoneData.RotAxis.CurrentType == RotateAxis.eType.X)
        {
            axis = Vector3.up;
        }
        else if (SelectedBoneData.RotAxis.CurrentType == RotateAxis.eType.Y)
        {
            axis = Vector3.right;
        }
        else if (SelectedBoneData.RotAxis.CurrentType == RotateAxis.eType.Z)
        {
            axis = Vector3.forward;
        }

        RotateSelectedBone(axis, angle);
        mBeforeTouchPos = mCurTouchPos;
    }

    public void RotateSelectedBone(Vector3 axis, float angle)
    {
        if(SelectedBoneData == null || SelectedBoneData.Bone == null)
        {
            return;
        }

        SelectedBoneData.Bone.Rotate(axis, angle);
    }

    public void EditingLightPosition()
    {
        mCurTouchPos = Get3DPosFromMousePosition(SelectedLightObjInfo.ShowingObj.transform);
        Vector3 deltaPos = mCurTouchPos - mBeforeTouchPos;

        SelectedLightObjInfo.ShowingObj.transform.position += deltaPos;
        mBeforeTouchPos = mCurTouchPos;
    }

    public void SelectLightRotAxis(RotateAxis selectedRotAxis, Transform selectedAxis)
    {
        SetEditType(eEditType.LightRotation);

        selectedRotAxis.SelectAxis(selectedAxis);
        mCurTouchPos = mBeforeTouchPos = AppMgr.Instance.CustomInput.GetTouchPos();
    }

    public void EditingLightRotation()
    {
        if (SelectedLightObjInfo == null)
        {
            return;
        }

        mCurTouchPos = AppMgr.Instance.CustomInput.GetTouchPos();
        float angle = (((mCurTouchPos.x - mBeforeTouchPos.x) + (mCurTouchPos.y - mBeforeTouchPos.y))) * 0.35f;

        Vector3 axis = Vector3.zero;
        if (SelectedLightObjInfo.RotAxis.CurrentType == RotateAxis.eType.X)
        {
            axis = Vector3.up;
        }
        else if (SelectedLightObjInfo.RotAxis.CurrentType == RotateAxis.eType.Y)
        {
            axis = Vector3.right;
        }
        else if (SelectedLightObjInfo.RotAxis.CurrentType == RotateAxis.eType.Z)
        {
            axis = Vector3.forward;
        }

        RotateLightObj(axis, angle);
        mBeforeTouchPos = mCurTouchPos;
    }

    public void RotateLightObj(Vector3 axis, float angle)
    {
        if (SelectedLightObjInfo == null)
        {
            return;
        }

        SelectedLightObjInfo.ShowingObj.transform.Rotate(axis, angle);
    }

    private void Update()
    {
        if(EditType == eEditType.None || AppMgr.Instance.CustomInput.IsOverUI() || MessagePopup.IsActive)
        {
            return;
        }

        RoomCamera.RigidBody.velocity = Vector3.zero;

        if (EditType == eEditType.EditWaiting)
        {
            if(AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Select))
            { 
                Ray ray = RoomCamera.camera.ScreenPointToRay(AppMgr.Instance.CustomInput.GetTouchPos());
                if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, (1 << (int)eLayer.Pick)))
                {
                    RotateAxis selectedRotAxis = hitInfo.transform.GetComponentInParent<RotateAxis>();
                    if (selectedRotAxis)
                    {
                    }
                    else
                    {
						string[] split = Utility.Split(hitInfo.transform.name, '_'); //hitInfo.transform.name.Split('_');
						if (split.Length < 2)
                        {
                            return;
                        }

                        FigureUnit.eEditableBoneType selectBoneType = SelectedFigureInfo.figure.FindBoneTypeByName(split[1]);
                        if (selectBoneType != FigureUnit.eEditableBoneType.None)
                        {
                        }
                    }
                }
            }
        }
        else if(EditType == eEditType.FigureBonePosition)
        {
            if (AppMgr.Instance.CustomInput.GetButtonUp(BaseCustomInput.eKeyKind.Select))
            {
                SetEditType(eEditType.EditWaiting);
            }
            else
            {
                EditingBonePosition();
            }
        }
    }

    private sFigureInfo GetPickingFigureInfo()
    {
        if (Physics.Raycast(RoomCamera.camera.ScreenPointToRay(AppMgr.Instance.CustomInput.GetTouchPos()), out RaycastHit hitInfo, Mathf.Infinity, 1 << (int)eLayer.Figure))
        {
            FigureUnit figure = hitInfo.collider.GetComponent<FigureUnit>();
            if (figure && figure.gameObject.activeSelf)
            {
                return ListFigureInfo.Find(x => x.figure == figure);
            }
        }

        return null;
    }

    private Vector3 Get3DPosFromMousePosition(Transform bone)
    {
        Vector3 v = RoomCamera.camera.WorldToScreenPoint(bone.position);
        return Utility.Get3DPos(RoomCamera.camera, AppMgr.Instance.CustomInput.GetTouchPos(), v.z);
    }

    private IEnumerator StartScreenShot()
    {
        yield return new WaitForEndOfFrame();
        mScreenShot.OnClick_ScreenShot();
    }

    private void OnPostScreenShot()
    {
        mMakingScreenShot = false;

        UIFigureRoomEditModePanel editModePanel = LobbyUIManager.Instance.GetActiveUI<UIFigureRoomEditModePanel>("FigureRoomEditModePanel");
        if (editModePanel)
        {
            editModePanel.OnPostScreenShot();
        }
    }

    public void SetARMode(eFigureRoomMode roomMode)
    {
        FigureRoomMode = roomMode;
        switch(roomMode)
        {
            case eFigureRoomMode.EditMode:
                for (int i = 0; i < ListFigureInfo.Count; i++)
                    ListFigureInfo[i].figure.gameObject.SetActive(true);
                break;
            case eFigureRoomMode.ARMode:
                for (int i = 0; i < ListFigureInfo.Count; i++)
                    ListFigureInfo[i].figure.gameObject.SetActive(false);
                break;
        }
    }

    public void ResetSelectedFigureInfo()
    {
        SelectedFigureInfo = null;
    }

    public Vector3 ARModeSelectFigurePos()
    {
        return SelectedFigureInfo.figure.transform.position;
    }

    public void SelectLightObj(GameObject lightObj)
    {
        sLightObjInfo find = mListLightObj.Find(x => x.ShowingObj == lightObj);
        if(find == null)
        {
            return;
        }

        SelectedLightObjInfo = find;
        SelectedLightObjInfo.Select();

        mCurTouchPos = mBeforeTouchPos = Get3DPosFromMousePosition(SelectedLightObjInfo.ShowingObj.transform);
        SetEditType(eEditType.LightPosition);
    }

    public void UnselectLightObj()
    {
        if(SelectedLightObjInfo == null)
        {
            return;
        }

        SelectedLightObjInfo.Unselect();
        SelectedLightObjInfo = null;

        SetEditType(eEditType.EditWaiting);
    }

    public void ShowLights(bool show, bool withRotAxis)
    {
        for (int i = 0; i < mListLightObj.Count; i++)
        {
            mListLightObj[i].Show(show, withRotAxis);
        }
    }

    public void ResetAllLights()
    {
        for(int i = 0; i < mListLightObj.Count; i++)
        {
            mListLightObj[i].Reset();
        }
    }

    public byte[] SaveLights()
    {
        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);

        bw.Write(VERSION);
        bw.Write(mListLightObj.Count);

        for(int i = 0; i < mListLightObj.Count; i++)
        {
            mListLightObj[i].SaveToBinary(bw);
        }

        bw.Close();
        return ms.ToArray();
    }

    protected override void OnDestroy()
    {
        mEndDateTime = GameSupport.GetCurrentServerTime();

        System.TimeSpan timeCal = mEndDateTime - mStartDateTime;

        Log.Show(timeCal.ToString());
        Firebase.Analytics.FirebaseAnalytics.LogEvent("FigureRoomOut", "FigureRoomTickTime", timeCal.ToString());

        for(int i = 0; i < mListLightObj.Count; i++)
        {
            mListLightObj[i].Destroy();
        }
        mListLightObj.Clear();

        base.OnDestroy();
    }

    private void CreateLight()
    {
        // 씬에 박아둔 라이트는 비활성화
        RoomDefaultLight.gameObject.SetActive(false);

        // 카메라에 라이트 오브젝트 레이어 추가
        RoomCamera.camera.cullingMask |= (1 << (int)eLayer.HitBox);

        if (RoomSlotData.ArrLightInfo != null && RoomSlotData.ArrLightInfo.Length > 0 && RoomSlotData.ArrLightInfo[0] > 0)
        {
            LoadLights();
        }
        else
        {
            GameObject lightObj = ResourceMgr.Instance.CreateFromAssetBundle("unit", "Unit/FigureLight.prefab");
            if (lightObj == null)
            {
                Debug.LogError("프라이빗 룸에 라이트를 생성할 수 없습니다.");
                return;
            }

            sLightObjInfo lightObjInfo = new sLightObjInfo();
            lightObjInfo.Init(lightObj, RoomDefaultLight);

            mListLightObj.Add(lightObjInfo);
        }

        SelectedLightObjInfo = null;
        ShowLights(false, true);
    }

    private void LoadLights()
    {
        MemoryStream ms = new MemoryStream(RoomSlotData.ArrLightInfo);
        BinaryReader br = new BinaryReader(ms);

        int version = br.ReadInt32();
        int count = br.ReadInt32();

        if (count == 0)
        {
            GameObject lightObj = ResourceMgr.Instance.CreateFromAssetBundle("unit", "Unit/FigureLight.prefab");
            if (lightObj == null)
            {
                Debug.LogError("프라이빗 룸에 라이트를 생성할 수 없습니다.");
                return;
            }

            sLightObjInfo lightObjInfo = new sLightObjInfo();
            lightObjInfo.Init(lightObj, RoomDefaultLight);

            mListLightObj.Add(lightObjInfo);
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                GameObject lightObj = ResourceMgr.Instance.CreateFromAssetBundle("unit", "Unit/FigureLight.prefab");
                if (lightObj == null)
                {
                    Debug.LogError("프라이빗 룸에 라이트를 생성할 수 없습니다.");
                    return;
                }

                sLightObjInfo lightObjInfo = new sLightObjInfo();

                lightObjInfo.Init(lightObj, RoomDefaultLight);
                lightObjInfo.LoadFromBinary(br);

                mListLightObj.Add(lightObjInfo);
            }
        }

        br.Close();
    }
}
