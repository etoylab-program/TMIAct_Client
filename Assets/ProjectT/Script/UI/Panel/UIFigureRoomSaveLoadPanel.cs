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
            //  ���� �׿� ������ ����
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
        // ���� ���õ� ������ �ٽ� �������� ���
        if (m_curSlot == slot)
        {
            m_curSlot.Select(false);
            ResetBottomUI();
            return;
        }

        // ������ ��ư�� �ִ°�� ���� ����
        if (m_curSlot != null)
            m_curSlot.Select(false);
        // �űԹ�ư���� ����
        m_curSlot = slot;
        m_curSlot.Select(true);

        if (false == m_isBottomUI)
        {
            PlayAnimtion(2);
            m_isBottomUI = true;
        }

        // ������ ���Կ� �°Բ� BottomUI�� ��ư�� ��� �ȴ�
        // ���õ� ���Կ� ���� ����� ���� ��� ����, �ҷ�����, ����
        // ���õ� ���Կ� ���� ����� ���� ��� Save�� ����
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
    ///  �����ϱ� ��ư
    /// </summary>
    public void OnBtnSave()
    {
        MessagePopup.CYN(eTEXTID.OK, 3044, eTEXTID.OK, eTEXTID.CANCEL, SaveRoom, null);
    }

    /// <summary>
    ///  �����ϱ� �̺�Ʈ
    /// </summary>
    private void SaveRoom()
    {
        // RoomThemeSlotData, FigureSlotData -> ���ÿ��� �����ϰ� �־�� ��. (���⿡���� �������� ����Ǵ� ������ ����)
        // ���÷θ� ����. �������� ������� �ʴ´�.
        SaveLocalData();

        Renewal(true);
    }

    void SaveLocalData()
    {
        RoomThemeSlotData roomThemeSlotData = FigureRoomScene.Instance.RoomSlotData.DeepCopy();

        // ��ũ���� ����.(���� ���� ���̺� ������ �ε����� ����.)
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
        // Index �� �̸� �������� ������� Ű�� �ִ��� ������ üũ�Ѵ�.
        _has = PlayerPrefs.HasKey("RoomThemeSlotData_SlotNum_Local_" + _uid.ToString() + p_slotNum.ToString("D2"));
        return _has;
    }

    /// <summary>
    ///  ������� �����ϱ� ��Ȳ -> ���⿡�� ��� ���� �ʰ�
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
    ///  �ҷ����� ��ư
    /// </summary>
    public void OnBtnLoad()
    {
        MessagePopup.CYN(eTEXTID.OK, 3044, eTEXTID.OK, eTEXTID.CANCEL, LoadRoom, null);
    }

    /// <summary>
    ///  �ҷ����� �̺�Ʈ
    /// </summary>
    private void LoadRoom()
    {
        LoadLocalData();

        OnBtnBack();
    }

    #endregion

    #region ClearSlot
    /// <summary>
    ///  ���� ���� ��ư
    /// </summary>
    public void OnBtnClearSlot()
    {
        MessagePopup.CYN(eTEXTID.OK, 3044, eTEXTID.OK, eTEXTID.CANCEL, ClearRoomSlot, null);
    }

    /// <summary>
    ///  ���� ���� �̺�Ʈ
    /// </summary>
    private void ClearRoomSlot()
    {
        // �ش� �����͸� �����ش�.
        ClearLocalData();

        // ��ũ���� ���� �����ش�.
        FigureRoomScene.Instance.DeleteScreenShotFile(m_curSlot.index.ToString("D2"),
                                                            m_curSlot.index.ToString() + ".png");
        Renewal(true);
    }

    #endregion

    /// <summary>
    ///  ������ ������ �����͵� ����(�����ð�,����)
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
            //  �ϴ� �޴� ��Ȱ��ȭ
            OnBtnCancel();
        }
        else
        {
            //  �׸������� �̵�
            OnBtnBack();
        }

        return false;
    }
}
