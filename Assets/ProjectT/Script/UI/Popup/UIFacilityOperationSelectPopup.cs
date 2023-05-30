using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIFacilityOperationSelectPopup : FComponent
{
    [Header("Anchor_L")]
    [SerializeField] private UIItemListSlot itemListSlot = null;
    [SerializeField] private UILabel itemLabel = null;
    [SerializeField] private UILabel timeLabel = null;
    [SerializeField] private UILabel quantityLabel = null;

    [Header("Anchor_TR")]
    [SerializeField] private FList itemList = null;
    [SerializeField] private UILabel countLabel = null;

    private FacilityData _facilityData;
    
    private readonly List<CharData> _charDataList = new List<CharData>();
    private readonly Dictionary<int, GameTable.FacilityOperationRoom.Param> _facilityOperationRooms = new Dictionary<int, GameTable.FacilityOperationRoom.Param>();
    private readonly Dictionary<GameObject, UIUserCharListSlot> _userCharListSlots = new Dictionary<GameObject, UIUserCharListSlot>();

    private int _selectCount = 0;
    private int _maxCount = 0;

    public override void Awake()
    {
        base.Awake();

        itemList.EventUpdate = _UpdateItem;
        itemList.EventGetItemCount = _ItemCount;
    }
    
    public override void OnEnable()
    {
        _userCharListSlots.Clear();
        _charDataList.Clear();
        
        foreach (CharData charData in GameInfo.Instance.CharList)
        {
            if (0 < charData.OperationRoomTID)
            {
                continue;
            }
            
            _charDataList.Add(charData);
        }

        List<CharData> charDataList = GameInfo.Instance.CharList.FindAll(x => x.OperationRoomTID > 0);
        int max = GameInfo.Instance.GameConfig.ParticipantMaxCount - charDataList.Count;
        
        _SetCountLabel((int)eCOUNT.NONE, max);
        
        itemList.InitBottomFixing();
        itemList.UpdateList();

        base.OnEnable();
    }
    
    private void _UpdateSelectSlot(GameObject obj)
    {
        if (!_userCharListSlots.ContainsKey(obj))
        {
            return;
        }
        
        UIUserCharListSlot slot = _userCharListSlots[obj];
        bool active = slot.kSelSpr.gameObject.activeSelf;
        int tempSelectCount = _selectCount;
        if (active)
        {
            --tempSelectCount;
        }
        else
        {
            ++tempSelectCount;
        }
        
        if (_maxCount < tempSelectCount)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3056));
            return;
        }
        
        _SetCountLabel(tempSelectCount);
        
        slot.kSelSpr.SetActive(!active);
    }
    
    private void _UpdateItem(int index, GameObject obj)
    {
        if (_charDataList.Count <= index)
        {
            return;
        }
        
        UIUserCharListSlot slot;
        if (_userCharListSlots.ContainsKey(obj))
        {
            slot = _userCharListSlots[obj];
        }
        else
        {
            slot = obj.GetComponent<UIUserCharListSlot>();
            _userCharListSlots.Add(obj, slot);
        }
        
        slot.UpdateFacilityOperationSlot(_charDataList[index], _UpdateSelectSlot);
    }
    
    private int _ItemCount()
    {
        return _charDataList.Count;
    }
    
    private void _SetCountLabel(int selectCount, int maxCount = -1)
    {
        _selectCount = selectCount;
        if (0 <= maxCount)
        {
            _maxCount = maxCount;
        }
        
        countLabel.textlocalize = FLocalizeString.Instance.GetText(218, _selectCount, _maxCount);
        
        string itemRewardCount = FLocalizeString.Instance.GetText(1792);
        if (_facilityOperationRooms.TryGetValue(_selectCount, out GameTable.FacilityOperationRoom.Param value))
        {
            itemRewardCount = FLocalizeString.Instance.GetText(306, value.ProductValueMin, value.ProductValueMax);
        }
        itemListSlot.SetCountText(itemRewardCount);

        var labelName = FLocalizeString.Instance.GetText(1795);
        var labelValue = FLocalizeString.Instance.GetText(280, _facilityData.TableData.Time / 60);
        timeLabel.textlocalize = FLocalizeString.Instance.GetText(220, labelName, labelValue);

        labelName = FLocalizeString.Instance.GetText(1796);
        labelValue = itemRewardCount;
        quantityLabel.textlocalize = FLocalizeString.Instance.GetText(220, labelName, labelValue);
    }
    
    public void SetItem(FacilityData facilityData, GameTable.Item.Param itemParam)
    {
        itemListSlot.UpdateSlot(UIItemListSlot.ePosType.Mat, (int)eCOUNT.NONE, itemParam);

        _facilityData = facilityData;
        _facilityOperationRooms.Clear();

        if (itemParam == null)
        {
            return;
        }

        itemLabel.textlocalize = FLocalizeString.Instance.GetText(itemParam.Name);

        foreach(GameTable.FacilityOperationRoom.Param param in
                GameInfo.Instance.GameTable.FacilityOperationRooms.FindAll(x => x.ProductIndex == itemParam.ID))
        {
            _facilityOperationRooms.Add(param.ParticipantCount, param);
        }
    }
    
    public void OnClick_AutoMatBtn()
    {
        if (_maxCount <= _selectCount)
        {
            return;
        }
        
        int tempSelectCount = _selectCount;
        foreach (KeyValuePair<GameObject, UIUserCharListSlot> slot in _userCharListSlots)
        {
            if (slot.Value.kSelSpr.gameObject.activeSelf)
            {
                continue;
            }
            
            if (_maxCount <= tempSelectCount)
            {
                break;
            }
            
            slot.Value.kSelSpr.SetActive(true);
            ++tempSelectCount;
        }
        
        _SetCountLabel(tempSelectCount);
    }
    
    public void OnClick_ResetBtn()
    {
        foreach (KeyValuePair<GameObject, UIUserCharListSlot> slot in _userCharListSlots)
        {
            slot.Value.kSelSpr.SetActive(false);
        }
        
        _SetCountLabel(0);
    }
    
    public void OnClick_StartBtn()
    {
        if (_selectCount <= 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1324));
            return;
        }

        List<long> selectCharUidList = new List<long>();
        foreach(KeyValuePair<GameObject, UIUserCharListSlot> pair in _userCharListSlots)
        {
            if (!pair.Value.kSelSpr.gameObject.activeSelf)
            {
                continue;
            }
            
            selectCharUidList.Add(pair.Value.CharData.CUID);
        }

        if (selectCharUidList.Count <= 0)
        {
            return;
        }
        
        int rnd = UnityEngine.Random.Range(0, selectCharUidList.Count);
        
        GameInfo.Instance.Send_ReqFacilityOperation(_facilityData.TableID, selectCharUidList[rnd], 1, selectCharUidList, OnNetOperationStart);
    }

    private void OnNetOperationStart(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        LobbyFacility lobbyFacility = Lobby.Instance.GetLobbyFacility(_facilityData.TableData.ParentsID);
        if (lobbyFacility != null)
        {
            lobbyFacility.InitLobbyFacility(_facilityData);
        }
        
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("FacilityItemPanel");
        
        OnClickClose();
    }
}
