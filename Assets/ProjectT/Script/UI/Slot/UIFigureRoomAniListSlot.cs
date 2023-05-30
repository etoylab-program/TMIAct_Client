
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIFigureRoomAniListSlot : FSlot
{
    public UISprite sprDisable;
    public UILabel lbName;
    public UISprite sprSet;
    public UIButton btnBuy;
    public UIButton btnSub;

    public GameTable.RoomAction.Param param { get; private set; }

    private int         mIndex          = 0;
    private int         mCurRoomTableId = 0;
    private FigureData  mFigureData     = null;

    
    public void UpdateSlot(int index, RoomThemeSlotData roomData, FigureData figureData, GameTable.RoomAction.Param param)
    {
        mIndex = index;
        mCurRoomTableId = roomData.TableID;
        mFigureData = figureData;
        this.param = param;

        sprDisable.gameObject.SetActive(true);
        sprSet.gameObject.SetActive(false);
        btnBuy.gameObject.SetActive(true);
        btnSub.gameObject.SetActive(false);

        int boughtActionTableId = GameInfo.Instance.RoomActionList.Find(x => x == param.ID);
        if(boughtActionTableId > 0)
        {
            sprDisable.gameObject.SetActive(false);
            btnBuy.gameObject.SetActive(false);
        }

        if (mFigureData.actionData != null && mFigureData.actionData.tableId == param.ID)
        {
            sprSet.gameObject.SetActive(true);
            btnSub.gameObject.SetActive(true);
        }

        lbName.textlocalize = FLocalizeString.Instance.GetText(param.Name);
    }
    
    public void Select()
    {
        sprSet.gameObject.SetActive(true);
        btnSub.gameObject.SetActive(true);
    }

    public void Unselect()
    {
        sprSet.gameObject.SetActive(false);
        btnSub.gameObject.SetActive(false);
    }

    public void OnBtnClick()
    {
        UIFigureRoomEditModePanel panel = ParentGO.GetComponent<UIFigureRoomEditModePanel>();
        panel.OnSelectFigureActionSlot (this);
    }

    public void OnBtnBuy()
    {
        GameTable.StoreRoom.Param paramStoreRoom = GameInfo.Instance.GameTable.FindStoreRoom(x => x.ID == param.StoreRoomID);
        if (paramStoreRoom == null)
        {
            Debug.LogError("StoreRoom 테이블에 " + param.StoreRoomID + "번 상품이 없습니다.");
            return;
        }

        UIBuyRoomPopup popup = LobbyUIManager.Instance.GetUI<UIBuyRoomPopup>("BuyRoomPopup");
        popup.Show(ParentGO.GetComponent<FComponent>(), mCurRoomTableId, paramStoreRoom);
    }

    public void OnBtnSub()
    {
        UIFigureRoomEditModePanel panel = ParentGO.GetComponent<UIFigureRoomEditModePanel>();
        panel.OnUnselectFigureActionSlot(this);
    }
}
