using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class UIFigureRoomSaveLoadPanel : FComponent
{
	[Serializable]
	public class LocalSaveInfo {
		public int figureTableId;
		public int action1;
		public int action2;
		public byte[] detailArray;
	}

	public UILabel lbName;
    public UILabel lbDate;
    public UIFigureRoomSaveListSlot[] m_arrSavedSlot;

    [Header("Bottom UI")]
    public GameObject goSaveBtn;
    public GameObject goLoadBtn;
    public GameObject goDeleteBtn;

	[HideInInspector]
	public List<LocalSaveInfo> saveList;

	private Animation					m_ani			= null;
	private RoomThemeSlotData			m_roomData		= null;
	private UIFigureRoomSaveListSlot	m_curSlot		= null;
	private bool						m_isBottomUI	= false;


	public override void OnEnable() {
		if ( m_ani == null ) {
			m_ani = GetComponent<Animation>();
		}

		m_roomData = FigureRoomScene.Instance.RoomSlotData;
		Init();

		base.OnEnable();
	}

	public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();

		m_isBottomUI = false;
        m_curSlot = null;
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
        Init();
    }

    private void Init()
    {
        ResetBottomUI();

        for (int i = 0; i < m_arrSavedSlot.Length; i++)
        {
            m_arrSavedSlot[i].ParentGO = this.gameObject;
            m_arrSavedSlot[i].UpdateFigureRoomSlot(i);
            //  슬롯 그외 데이터 셋팅
            SetSlotData(m_arrSavedSlot[i], i);
        }
    }

    void ResetBottomUI()
    {
        m_curSlot = null;
        PlayAnimtion(3);
        m_isBottomUI = false;
    }

    public void SelectSlot(UIFigureRoomSaveListSlot slot)
    {
        // 현재 선택된 슬롯을 다시 선택했을 경우
        if (m_curSlot == slot)
        {
            m_curSlot.Select(false);
            ResetBottomUI();
            return;
        }

        // 기존의 버튼이 있는경우 선택 해제
        if (m_curSlot != null)
            m_curSlot.Select(false);
        // 신규버튼으로 설정
        m_curSlot = slot;
        m_curSlot.Select(true);

        if (false == m_isBottomUI)
        {
            PlayAnimtion(2);
            m_isBottomUI = true;
        }

        // 현재의 슬롯에 맞게끔 BottomUI의 버튼이 출력 된다
        // 선택된 슬롯에 저장 목록이 있을 경우 삭제, 불러오기, 저장
        // 선택된 슬롯에 저장 목록이 없을 경우 Save만 가능
        if (HasSaveSlotData(m_curSlot.index))
        {
            goSaveBtn.SetActive(true);
            goLoadBtn.SetActive(true);
            goDeleteBtn.SetActive(true);
        }
        else
        {
            goSaveBtn.SetActive(true);
            goLoadBtn.SetActive(false);
            goDeleteBtn.SetActive(false);
        }
    }

    private int GetFigureRoomSlotCount()
    {
        //787878    return m_roomData.listFigureRoomSave.Count;
        return 0;
    }

    private void UpdateFigureRoomSlot(int index, GameObject slotObj)
    {
        UIFigureRoomSaveListSlot slot = slotObj.GetComponent<UIFigureRoomSaveListSlot>();
        slot.ParentGO = gameObject;
    }

    public void OnBtnBack()
    {
        //OnClose();
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.FIGUREROOM);
    }

    public void OnBtnCancel()
    {
        PlayAnimtion(3);
        m_isBottomUI = false;
    }

    #region Save
    /// <summary>
    ///  저장하기 버튼
    /// </summary>
    public void OnBtnSave()
    {
        MessagePopup.CYN(eTEXTID.OK, 3044, eTEXTID.OK, eTEXTID.CANCEL, SaveRoom, null);
    }

    /// <summary>
    ///  저장하기 이벤트
    /// </summary>
    private void SaveRoom()
    {
        // RoomThemeSlotData, FigureSlotData -> 로컬에서 저장하고 있어야 함. (여기에서는 서버에서 저장되는 구간이 없다)
        // 로컬로만 저장. 서버에는 저장되지 않는다.
        SaveLocalData();

        Renewal(true);
    }

    void SaveLocalData()
    {
        RoomThemeSlotData roomThemeSlotData = FigureRoomScene.Instance.RoomSlotData.DeepCopy();

        // 스크린샷 저장.(현재 로컬 세이브 슬롯의 인덱스로 저장.)
        FigureRoomScene.Instance.SaveScreenShot(m_curSlot.index.ToString("D2"), m_curSlot.index.ToString());

        saveList = new List<LocalSaveInfo>();
        for (int i = 0; i < FigureRoomScene.Instance.ListFigureInfo.Count; i++)
        {
            FigureData _data = FigureRoomScene.Instance.ListFigureInfo[i].data;
            LocalSaveInfo _save = new LocalSaveInfo();
            /*if (_data.listFigureAction != null)
            {
                int _cnt = 0;
                _save.action1 = 101001;
                _save.action2 = 201001;
                foreach (var item in _data.listFigureAction)
                {
                    if (_cnt == 0)
                        _save.action1 = item.tableId;
                    else if (_cnt == 1)
                        _save.action2 = item.tableId;
                    else
                        break;

                    _cnt++;
                }
            }*/
            _save.figureTableId = _data.tableId;
            _save.detailArray = _data.saveData.ToBytes();
            saveList.Add(_save);
        }
        var _uid = GameInfo.Instance.UserData.UUID;
        PlayerPrefs.SetInt("RoomThemeSlotData_SlotNum_Local_" + _uid.ToString() + m_curSlot.index.ToString("D2"), m_curSlot.index);
        PlayerPrefs.SetInt("RoomThemeSlotData_TableID_Local_" + _uid.ToString() + m_curSlot.index.ToString("D2"), roomThemeSlotData.TableID);

        var _str = TransSerialize.ObjectToStringSerialize(saveList);
        PlayerPrefs.SetString("RoomThemeSlotData_Local" + _uid.ToString() + m_curSlot.index.ToString("D2"), _str);
    }

    void LoadLocalData()
    {
        var _uid = GameInfo.Instance.UserData.UUID;
        GameInfo.Instance.UseRoomDataClear();
        int _slotNum = PlayerPrefs.GetInt("RoomThemeSlotData_SlotNum_Local_" + _uid.ToString() + m_curSlot.index.ToString("D2"));
        int _tableId = PlayerPrefs.GetInt("RoomThemeSlotData_TableID_Local_" + _uid.ToString() + m_curSlot.index.ToString("D2"));
        GameInfo.Instance.UseRoomThemeData = new RoomThemeSlotData(0, _tableId);

        var data = PlayerPrefs.GetString("RoomThemeSlotData_Local" + _uid.ToString() + m_curSlot.index.ToString("D2"));
        if (!string.IsNullOrEmpty(data))
        {
            var _figureData = TransSerialize.Deserialize<List<LocalSaveInfo>>(data);

            int index = 0;
            foreach (var item in _figureData)
            {
                RoomThemeFigureSlotData _data = new RoomThemeFigureSlotData(item.figureTableId);
                _data.Action1 = item.action1;
                //_data.Action2 = item.action2;
                _data.detailarry = item.detailArray;
                _data.RoomThemeSlotNum = 0;
                _data.SlotNum = index;
                GameInfo.Instance.UseRoomThemeFigureList.Add(_data);
                index++;
            }
        }

        FigureRoomScene.Instance.LoadThemeFigureListByLocalData();

        UIFigureRoomPanel figureRoomPanel = LobbyUIManager.Instance.GetUI<UIFigureRoomPanel>("FigureRoomPanel");
        figureRoomPanel.Renewal(true);

        FigureRoomScene.Instance.DestroyFigureListInfo();
        //Lobby.Instance.MoveToRoom(GameInfo.Instance.UseRoomThemeData.TableID);

        Lobby.Instance.ForceToRoom(GameInfo.Instance.UseRoomThemeData.TableID);
        //LobbyDoorPopup.Show();
    }
    
    void ClearLocalData()
    {
        var _uid = GameInfo.Instance.UserData.UUID;
        PlayerPrefs.DeleteKey("RoomThemeSlotData_SlotNum_Local_" + _uid.ToString() + m_curSlot.index.ToString("D2"));
        PlayerPrefs.DeleteKey("RoomThemeSlotData_TableID_Local_" + _uid.ToString() + m_curSlot.index.ToString("D2"));
        PlayerPrefs.DeleteKey("RoomThemeSlotData_Local" + _uid.ToString() + m_curSlot.index.ToString("D2"));
    }

    public bool HasSaveSlotData(int p_slotNum)
    {
        bool _has = false;
        var _uid = GameInfo.Instance.UserData.UUID;
        // Index 의 이름 조합으로 만들어진 키가 있는지 없는지 체크한다.
        _has = PlayerPrefs.HasKey("RoomThemeSlotData_SlotNum_Local_" + _uid.ToString() + p_slotNum.ToString("D2"));
        return _has;
    }

    /// <summary>
    ///  통신이후 저장하기 상황 -> 여기에선 통신 하지 않고
    /// </summary>
    /// <param name="result"></param>
    /// <param name="pktMsgType"></param>
    private void OnSaveRoom(int result, PktMsgType pktMsgType)
    {
        //LoadTexture(m_curSlot.index);

        OnBtnCancel();

        Renewal(true);
    }

    #endregion

    #region Load
    /// <summary>
    ///  불러오기 버튼
    /// </summary>
    public void OnBtnLoad()
    {
        MessagePopup.CYN(eTEXTID.OK, 3044, eTEXTID.OK, eTEXTID.CANCEL, LoadRoom, null);
    }

    /// <summary>
    ///  불러오기 이벤트
    /// </summary>
    private void LoadRoom()
    {
        LoadLocalData();

        OnBtnBack();
    }

    #endregion

    #region ClearSlot
    /// <summary>
    ///  슬롯 비우기 버튼
    /// </summary>
    public void OnBtnClearSlot()
    {
        MessagePopup.CYN(eTEXTID.OK, 3044, eTEXTID.OK, eTEXTID.CANCEL, ClearRoomSlot, null);
    }

    /// <summary>
    ///  슬롯 비우기 이벤트
    /// </summary>
    private void ClearRoomSlot()
    {
        // 해당 데이터만 지워준다.
        ClearLocalData();

        // 스크린샷 파일 지워준다.
        FigureRoomScene.Instance.DeleteScreenShotFile(m_curSlot.index.ToString("D2"),
                                                            m_curSlot.index.ToString() + ".png");
        Renewal(true);
    }

    #endregion

    /// <summary>
    ///  슬롯의 나머지 데이터들 셋팅(생성시간,사진)
    /// </summary>
    /// <param name="slot"></param>
    private void SetSlotData(UIFigureRoomSaveListSlot slot, int slotNum)
    {
        System.DateTime dateTime = new System.DateTime();
        Texture2D tex =
            FigureRoomScene.Instance.LoadScreenShotTexture(slotNum.ToString("D2"),
                                                            slotNum.ToString() + ".png",
                                                            ref dateTime);

        slot.SetSavedCreateDay(dateTime);
        slot.SetSavedPicture(tex);
        //*/
    }


    public override bool IsBackButton()
    {
        if (m_isBottomUI == true)
        {
            //  하단 메뉴 비활성화
            OnBtnCancel();
        }
        else
        {
            //  테마룸으로 이동
            OnBtnBack();
        }

        return false;
    }
}
