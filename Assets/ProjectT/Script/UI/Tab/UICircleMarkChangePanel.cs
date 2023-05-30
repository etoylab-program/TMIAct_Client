using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleMarkChangePanel : FComponent
{
    [Header("UICircleLobbyMarkBuyPanel")]
    [SerializeField] private FList buyFList = null;    
    [SerializeField] private UITexture flagTex = null;
    [SerializeField] private UITexture markTex = null;
    [SerializeField] private UITexture colorTex = null;
    [SerializeField] private UILabel changeLabel = null;
    [SerializeField] private GameObject buyObj = null;
    [SerializeField] private UIGoodsUnit buyGoodsUnit = null;
    [SerializeField] private List<UISprite> selSprList = null;
    [SerializeField] private UITexture selFlagTex = null;
    [SerializeField] private UITexture selMarkTex = null;
    [SerializeField] private UISprite selColorSpr = null;

    private int _selectTableId;
    private int[] _selectTableIdArray;
    private eCircleMarkType _currentFlagType;

    public override void Awake()
    {
        base.Awake();

        buyFList.EventUpdate = OnEventMarkListUpdate;
        buyFList.EventGetItemCount = OnEventMarkListCount;
        buyFList.UpdateList();

        _selectTableIdArray = new int[(int)eCircleMarkType.COLOR];
    }

    public override void InitComponent()
    {
        base.InitComponent();

        InitData(eCircleMarkType.FLAG, GameInfo.Instance.CircleData.FlagId, ref flagTex);
        InitData(eCircleMarkType.MARK, GameInfo.Instance.CircleData.MarkId, ref markTex);
        InitData(eCircleMarkType.COLOR, GameInfo.Instance.CircleData.ColorId, ref colorTex);

        selFlagTex.mainTexture = flagTex.mainTexture;
        selMarkTex.mainTexture = markTex.mainTexture;
        selColorSpr.color = colorTex.color;

        SetChangeFlagType(eCircleMarkType.FLAG);

        _selectTableId = GameInfo.Instance.CircleData.FlagId;
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        buyFList.RefreshNotMoveAllItem();

        bool isHaveMark = false;
        GameTable.CircleMark.Param circleMarkParam = GameInfo.Instance.GameTable.FindCircleMark(_selectTableId);
        if (circleMarkParam != null)
        {
            isHaveMark = GameInfo.Instance.IsCircleMark((eCircleMarkType)circleMarkParam.Marktype, circleMarkParam.ID);
            GameTable.Store.Param storeParam = GameInfo.Instance.GameTable.FindStore(circleMarkParam.StoreID);
            if (storeParam != null)
            {
                buyGoodsUnit.InitGoodsUnit(eCircleGoodsType.CIRCLE_GOLD, storeParam.PurchaseValue);
            }
        }

        changeLabel.SetActive(isHaveMark);
        buyObj.SetActive(!isHaveMark);
    }

    public void SetSeleteID(int selectTableId)
    {
        int selectIndex = selectTableId / 100 - 1;
        if (0 <= selectIndex && selectIndex < _selectTableIdArray.Length)
        {
            _selectTableIdArray[selectIndex] = _selectTableId = selectTableId;
        }

        Renewal();

        switch (_currentFlagType)
        {
            case eCircleMarkType.FLAG:
                {
                    flagTex.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture(_selectTableId);
                    selFlagTex.mainTexture = flagTex.mainTexture;
                }
                break;
            case eCircleMarkType.MARK:
                {
                    markTex.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture(_selectTableId);
                    selMarkTex.mainTexture = markTex.mainTexture;
                }
                break;
            case eCircleMarkType.COLOR:
                {
                    flagTex.color = LobbyUIManager.Instance.GetCircleMarkColor(_selectTableId);
                    selColorSpr.color = flagTex.color;
                }
                break;
        }
    }

    private void OnEventMarkListUpdate(int index, GameObject obj)
    {
        UIUserIconListSlot slot = obj.GetComponent<UIUserIconListSlot>();
        if (slot == null)
        {
            return;
        }

        if (slot.ParentGO == null)
        {
            slot.ParentGO = this.gameObject;
        }

        slot.UpdateSlot(index, GetCircleMarkParam(index), _selectTableId);
    }

    private int OnEventMarkListCount()
    {
        return GameInfo.Instance.GameTable.FindAllCircleMark(x => x.Marktype == (int)_currentFlagType).Count;
    }

    private void InitData(eCircleMarkType circleFlagType, int tableId, ref UITexture uITexture)
    {
        switch(circleFlagType)
        {
            case eCircleMarkType.FLAG:
            case eCircleMarkType.MARK:
                {
                    uITexture.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture(tableId);
                }
                break;
            case eCircleMarkType.COLOR:
                {
                    uITexture.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture(tableId, true);
                    uITexture.color = LobbyUIManager.Instance.GetCircleMarkColor(tableId);
                }
                break;
        }
        
        int circleFlagTypeIndex = (int)circleFlagType - 1;
        if (0 <= circleFlagTypeIndex && circleFlagTypeIndex < _selectTableIdArray.Length)
        {
            _selectTableIdArray[circleFlagTypeIndex] = tableId;
        }
    }

    private GameTable.CircleMark.Param GetCircleMarkParam(int index)
    {
        int tableId = index;

        GameTable.CircleMark.Param firstParam = GameInfo.Instance.GameTable.CircleMarks.Find(x => x.Marktype == (int)_currentFlagType);
        if (firstParam != null)
        {
            tableId += firstParam.ID;
        }

        return GameInfo.Instance.GameTable.FindCircleMark(tableId);
    }

    private void SetChangeFlagType(eCircleMarkType circleFlagType)
    {
        _currentFlagType = circleFlagType;

        int index = (int)circleFlagType - 1;
        for (int i = 0; i < selSprList.Count; i++)
        {
            selSprList[i].SetActive(i == index);
        }

        buyFList.Reset();

        Renewal();
    }

    public void OnClick_ChangeBtn()
    {
        bool isHaveMark = false;
        GameTable.CircleMark.Param circleMarkParam = GameInfo.Instance.GameTable.FindCircleMark(_selectTableId);
        if (circleMarkParam != null)
        {
            isHaveMark = GameInfo.Instance.IsCircleMark(_currentFlagType, circleMarkParam.ID);
        }

        if (isHaveMark)
        {
            GameInfo.Instance.Send_ReqCircleChangeMark(
                _selectTableIdArray[(int)eCircleMarkType.FLAG - 1], _selectTableIdArray[(int)eCircleMarkType.MARK - 1], _selectTableIdArray[(int)eCircleMarkType.COLOR - 1], OnNet_CircleChangeMark);
        }
        else
        {
            GameInfo.Instance.Send_ReqCircleBuyMarkItem(_selectTableId, OnNet_CircleBuyMarkItem);
        }
    }

    public void OnClick_FlagBtn()
    {
        SetChangeFlagType(eCircleMarkType.FLAG);
    }

    public void OnClick_MarkBtn()
    {
        SetChangeFlagType(eCircleMarkType.MARK);
    }

    public void OnClick_ColorBtn()
    {
        SetChangeFlagType(eCircleMarkType.COLOR);
    }

    private void OnNet_CircleBuyMarkItem(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        LobbyUIManager.Instance.Renewal("TopPanel");

        Renewal();
    }

    private void OnNet_CircleChangeMark(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        Renewal();
    }
}
