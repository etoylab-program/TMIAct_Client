using System.Collections.Generic;
using UnityEngine;

public class UIBadgeSelectPopup : FComponent
{
    [SerializeField] private FList _BatchListInstance;
	public UITexture kBatchTex;
	public UILabel kBatchNameLabel;
    public UILabel kBadgeLevelLabel;
	public UILabel kLevelupLabel;
	public UIButton kLevelupResetBtn;

    public List<UIGaugeUnit> kBadgeOptGaugeUnitList;

    public UILabel kChangeBtnLabel;
    public UILabel kSelectSlotLabel;
    public UILabel kSelectBadgeNameLabel;

    private int _selectSlotIdx = 0;
    private List<BadgeData> _badgeDataList = new List<BadgeData>();
    private BadgeData _selectBadgeData;
    public BadgeData SelectBadgeData { get { return _selectBadgeData; } }

    private long _selectBadgeUID = 0;
    public long SelectBadgeUID { get { return _selectBadgeUID; } }
    private long _selectOriginBadgeUID = 0;
    private GameTable.BadgeOpt.Param _selectBadgeBaseTableData;

    private eContentsPosKind _contentsPosKind = eContentsPosKind._NONE_;
    public override void Awake()
	{
		base.Awake();

		if(this._BatchListInstance == null) return;

        this._BatchListInstance.InitBottomFixing();
		this._BatchListInstance.EventUpdate = this._UpdateBatchListSlot;
		this._BatchListInstance.EventGetItemCount = this._GetBatchElementCount;
	}
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        //_badgeDataList = GameInfo.Instance.BadgeList;

        _selectSlotIdx = (int)UIValue.Instance.GetValue(UIValue.EParamType.BadgeSlotIndex);
        _selectOriginBadgeUID = _selectBadgeUID = (long)UIValue.Instance.GetValue(UIValue.EParamType.BadgeUID);
        _contentsPosKind = (eContentsPosKind)UIValue.Instance.GetValue(UIValue.EParamType.BadgeType);


        _badgeDataList.Clear();
        for(int i = 0; i < GameInfo.Instance.BadgeList.Count; i++)
        {
            BadgeData tempBadgeData = GameInfo.Instance.BadgeList[i];
            switch((eContentsPosKind)tempBadgeData.PosKind)
            {
                case eContentsPosKind.ARENA:
                case eContentsPosKind.ARENA_TOWER:
                    continue;                    
            }

            _badgeDataList.Add(GameInfo.Instance.BadgeList[i]);
        }

        BadgeData.SortUp = true;
        _badgeDataList.Sort(BadgeData.CompareFuncOptID);

        int PosKind = (int)eContentsPosKind.ARENA;
        if (_contentsPosKind == eContentsPosKind.ARENA_TOWER)
            PosKind = (int)eContentsPosKind.ARENA_TOWER;

//        for (int i = (int)eBadgeOptSlot.FIRST; i < (int)eBadgeOptSlot._MAX_; i++)
//        {
//#if ARENA_TOWER
//            BadgeData tempBadgData = GameInfo.Instance.BadgeList.Find(x => x.PosKind == PosKind  && x.PosSlotNum == i);
//#else
//            BadgeData tempBadgData = GameInfo.Instance.BadgeList.Find(x => x.PosKind == (int)eContentsPosKind.ARENA && x.PosSlotNum == i);
//#endif

//            if (tempBadgData != null)
//            {
//                if(_selectSlotIdx -1 != i)
//                {
//                    _badgeDataList.Insert(0, tempBadgData);
//                }
//            }
//        }

        BadgeData selBadge = GameInfo.Instance.BadgeList.Find(x => x.PosKind == PosKind && x.PosSlotNum == _selectSlotIdx -1);

        if (selBadge != null)
        {
            _badgeDataList.Insert(0, selBadge);
        }

        if (_badgeDataList.Count > 0)
        {
            if (_selectBadgeUID == (int)eCOUNT.NONE)
            {
                _selectBadgeUID = _badgeDataList[(int)eCOUNT.NONE].BadgeUID;
                _selectOriginBadgeUID = (long)eCOUNT.NONE;
                _selectBadgeData = GameInfo.Instance.BadgeList.Find(x => x.BadgeUID == _selectBadgeUID);
            }
            else
            {
                _selectBadgeData = GameInfo.Instance.BadgeList.Find(x => x.BadgeUID == _selectBadgeUID);
                if (_selectBadgeData == null)
                {
                    _selectBadgeUID = _badgeDataList[(int)eCOUNT.NONE].BadgeUID;
                    _selectOriginBadgeUID = (long)eCOUNT.NONE;
                    _selectBadgeData = GameInfo.Instance.BadgeList.Find(x => x.BadgeUID == _selectBadgeUID);
                }
            }
        }

        kSelectSlotLabel.textlocalize = _selectSlotIdx.ToString();
        _BatchListInstance.UpdateList();
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        if (_badgeDataList.Count <= 0)
        {
            return;
        }

        _selectBadgeData = GameInfo.Instance.BadgeList.Find(x => x.BadgeUID == _selectBadgeUID);
        SetSelectBadgeInfo();

        
        if (_selectBadgeData.Level == (int)eCOUNT.NONE)      //강화를 안한 레벨이 0일때
        {
            kBadgeLevelLabel.gameObject.SetActive(false);
        }
        else
        {
            kBadgeLevelLabel.gameObject.SetActive(true);
            kBadgeLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(282), _selectBadgeData.Level);
        }
        

        if(_selectOriginBadgeUID == _selectBadgeUID)
        {
            kChangeBtnLabel.textlocalize = FLocalizeString.Instance.GetText(14);
        }
        else
        {
            kChangeBtnLabel.textlocalize = FLocalizeString.Instance.GetText(13);
        }

        string lvUpCnt = string.Format("{0} {1}", FLocalizeString.Instance.GetText(1463), string.Format(FLocalizeString.Instance.GetText(1443), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_MAX_TEXT_COLOR), _selectBadgeData.RemainLvCnt)));
        kLevelupLabel.textlocalize = lvUpCnt;

        _BatchListInstance.RefreshNotMove();

    }

    private void _UpdateBatchListSlot(int index, GameObject slotObject)
	{
		do
		{
            UIBatchListSlotSlot slot = slotObject.GetComponent<UIBatchListSlotSlot>();
            if (null == slot) break;

            BadgeData data = null;
            if (0 <= index && _badgeDataList.Count > index)
            {
                data = _badgeDataList[index];
            }

            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(index, (int)eCOUNT.NONE, data, UIBatchListSlotSlot.eBadgeSlotType.Slot, _contentsPosKind, _selectBadgeUID == data.BadgeUID);
        } while(false);
	}
	
	private int _GetBatchElementCount()
	{
        if (_badgeDataList == null || _badgeDataList.Count <= 0)
            return 0;
        return _badgeDataList.Count;
    }

    private void SetSelectBadgeInfo()
    {
        if(_selectBadgeData == null)
        {
            kBatchTex.gameObject.SetActive(false);
            kBatchNameLabel.textlocalize = string.Empty;
            kSelectBadgeNameLabel.textlocalize = string.Empty;
            for (int i = 0; i < (int)eBadgeOptSlot._MAX_; i++)
            {
                kBadgeOptGaugeUnitList[i].InitGaugeUnit((float)eCOUNT.NONE);
                kBadgeOptGaugeUnitList[i].SetText(string.Empty);
            }
        }
        else
        {
            kBatchTex.gameObject.SetActive(true);
            kBatchTex.mainTexture = GameSupport.GetBadgeIcon(_selectBadgeData);

            for (int i = 0; i < (int)eBadgeOptSlot._MAX_; i++)
            {
                GameTable.BadgeOpt.Param optData = GameInfo.Instance.GameTable.BadgeOpts.Find(x => x.OptionID == _selectBadgeData.OptID[i]);
                if (i == (int)eBadgeOptSlot.FIRST)
                {
                    kBatchNameLabel.textlocalize = FLocalizeString.Instance.GetText(optData.Name);
                    kSelectBadgeNameLabel.textlocalize = FLocalizeString.Instance.GetText(optData.Name);
                }


                if (optData == null)
                {
                    kBadgeOptGaugeUnitList[i].InitGaugeUnit((float)eCOUNT.NONE);
                    kBadgeOptGaugeUnitList[i].SetText(string.Empty);
                }
                else
                {
                    float fillAmount = (float)(_selectBadgeData.OptVal[i] / (float)GameInfo.Instance.GameConfig.BadgeMaxOptVal);
                    kBadgeOptGaugeUnitList[i].InitGaugeUnit(fillAmount);

                    if (fillAmount >= 1.0f)
                        kBadgeOptGaugeUnitList[i].SetColor((int)UIGemOptUnit.eGAUGESTATE.MAX);
                    else
                        kBadgeOptGaugeUnitList[i].SetColor((int)UIGemOptUnit.eGAUGESTATE.NORMAL);

                    float optValue = ((_selectBadgeData.OptVal[i] + _selectBadgeData.Level) * optData.IncEffectValue) / (float)eCOUNT.MAX_RATE_VALUE * 100.0f;

                    string optDesc = string.Format(FLocalizeString.Instance.GetText(optData.Desc),
                        string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT, optValue))));

                    if(i == (int)eBadgeOptSlot.FIRST)
                    {
                        kBadgeOptGaugeUnitList[i].SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.YELLOW_TEXT_COLOR), optDesc));
                    }
                    else
                    {
                        kBadgeOptGaugeUnitList[i].SetText(optDesc);
                    }
                    
                }

            }
        }

    }
	
	public void OnClick_BackBtn()
	{
        OnClickClose();
	}
	
	public void OnClick_ChangeBtn()
	{
        if (_selectOriginBadgeUID == _selectBadgeUID)
        {
            //해제
            GameInfo.Instance.Send_ReqApplyOutPosBadge(_selectBadgeUID, OnNet_AckApplyPosBadge);
        }
        else
        {
            //변경
            GameInfo.Instance.Send_ReqApplyPosBadge(_selectBadgeUID, _selectSlotIdx - 1, _contentsPosKind, OnNet_AckApplyPosBadge);
        }
    }

    public void OnNet_AckApplyPosBadge(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        if(_contentsPosKind == eContentsPosKind.ARENA_TOWER)
        {
            LobbyUIManager.Instance.Renewal("ArenaTowerMainPanel");
            LobbyUIManager.Instance.Renewal("ArenaTowerStagePanel");
        }

        OnClickClose();
    }
	
    public void OnClick_BadgeSlot(long uid)
    {
        _selectBadgeUID = uid;
        _selectBadgeData = GameInfo.Instance.BadgeList.Find(x => x.BadgeUID == _selectBadgeUID);
        if(_selectBadgeData == null)
        {
            Log.Show("Badge is NULL");
            return;
        }
        //EquipBadgeWithSlot();
        Renewal(true);
        
    }

    public void OnClick_EquipBadgeSlot(int idx)
    {
        if(_selectSlotIdx != idx)
        {
            _selectSlotIdx = idx;
            BadgeData tempdata = _badgeDataList.Find(x => x.PosSlotNum == idx);
            if (tempdata != null)
            {
                _selectBadgeUID = tempdata.BadgeUID;
            }
        }
        else
        {
            BadgeData tempdata = _badgeDataList.Find(x => x.PosSlotNum == idx);
            if (tempdata != null)
            {
                tempdata.PosSlotNum = (int)eBadgeSlot.NONE;
                _selectBadgeUID = (long)eBadgeSlot.NONE;
            }
        }
        Renewal(true);
    }

    private void EquipBadgeWithSlot()
    {
        if(_selectBadgeData == null)
        {
            Log.Show("선택 된 문양이 없습니다!!!");
            return;
        }

        
        if(_selectBadgeData.PosSlotNum == _selectSlotIdx)
        {
            _selectBadgeData.PosSlotNum = (int)eBadgeSlot.NONE;
            _selectBadgeUID = (long)eCOUNT.NONE;
        }
        else
        {
            BadgeData tempdata = _badgeDataList.Find(x => x.PosSlotNum == _selectSlotIdx);
            if (tempdata != null)
            {
                tempdata.PosSlotNum = (int)eBadgeSlot.NONE;
            }

            _selectBadgeData.PosSlotNum = _selectSlotIdx;
        }
    }

    public override void OnClickClose()
    {
        LobbyUIManager.Instance.Renewal("PresetPopup");
        LobbyUIManager.Instance.Renewal("ArenaMainPanel");
        LobbyUIManager.Instance.Renewal("ArenaBattleConfirmPopup");
        base.OnClickClose();
    }
}
