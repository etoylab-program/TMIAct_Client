
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIFigureRoomEnvMenuListSlot : FSlot
{
    public UILabel lbName;
    public UISprite sprDisable;
    public UISprite sprSet;
    public UIButton btnBuy;
    public UIButton btnOff;

    public UIFigureRoomPanel.eEnvMenuType ListType { get; private set; } = UIFigureRoomPanel.eEnvMenuType.Theme;

    private int m_curRoomTableId = 0;
    private int m_tableId = 0;

    private UIFigureRoomPanel           mFigureRoomPanel    = null;
    private GameTable.RoomTheme.Param   mParamRoomTheme     = null;
    private RoomThemeFuncData           mRoomThemeFuncData  = null;

    //787878    private RoomThemeData m_roomData = null;
    //787878    private RoomThemeRoomFuncData m_funcData = null;


    public void UpdateThemeSlot(int index, GameTable.RoomTheme.Param param, int curRoomTableId)
    {
        ListType = UIFigureRoomPanel.eEnvMenuType.Theme;
        m_tableId = param.ID;
        m_curRoomTableId = curRoomTableId;
        mParamRoomTheme = param;

        sprDisable.gameObject.SetActive(true);
        sprSet.gameObject.SetActive(false);
        btnBuy.gameObject.SetActive(true);
        btnOff.gameObject.SetActive(false);

        bool b = GameInfo.Instance.IsRoomThema(param.ID);
        if (b)
        {
            sprDisable.gameObject.SetActive(false);
            btnBuy.gameObject.SetActive(false);

            if (param.ID == FigureRoomScene.Instance.RoomSlotData.TableID)
                sprSet.gameObject.SetActive(true);
        }
        else if(param.PreVisible == 0)
        {
            btnBuy.gameObject.SetActive(false);
        }

        lbName.textlocalize = FLocalizeString.Instance.GetText(param.Name);
    }

    public void UpdateEffectSlot(UIFigureRoomPanel figureRoomPanel, int index, GameTable.RoomFunc.Param param, RoomThemeSlotData roomData, int curRoomTableId)
    {
        mFigureRoomPanel = figureRoomPanel;
        ListType = UIFigureRoomPanel.eEnvMenuType.Effect;
        m_tableId = param.ID;
        m_curRoomTableId = curRoomTableId;

        sprDisable.gameObject.SetActive(true);
        sprSet.gameObject.SetActive(false);
        btnBuy.gameObject.SetActive(true);
        btnOff.gameObject.SetActive(false);

        int boughtFuncTableId = GameInfo.Instance.RoomFuncList.Find(x => x == param.ID);
        if (boughtFuncTableId > 0)
        {
            sprDisable.gameObject.SetActive(false);
            btnBuy.gameObject.SetActive(false);

            mRoomThemeFuncData = roomData.RoomThemeFuncList.Find(x => x.TableID == param.ID);
            if (mRoomThemeFuncData != null)
            {
                if (mRoomThemeFuncData.On)
                {
                    sprSet.gameObject.SetActive(true);
                    btnOff.gameObject.SetActive(true);
                }
            }
            else
            {
                mRoomThemeFuncData = new RoomThemeFuncData(boughtFuncTableId);
                roomData.RoomThemeFuncList.Add(mRoomThemeFuncData);
            }
        }

        /*
        m_funcData = roomData.listRoomFunc.Find(x => x.tableId == param.ID);
        if (m_funcData != null)
        {
            sprDisable.gameObject.SetActive(false);
            btnBuy.gameObject.SetActive(false);

            if (m_funcData.on)
            {
                sprSet.gameObject.SetActive(true);
                btnOff.gameObject.SetActive(true);
            }

            FigureRoomScene.Instance.ActivateRoomFunc(m_funcData);
        }
        */

        lbName.textlocalize = FLocalizeString.Instance.GetText(param.Name);
    }

    public void OnBtnClick()
    {
        if (sprDisable.gameObject.activeSelf)
            return;

        if (ListType == UIFigureRoomPanel.eEnvMenuType.Theme && !Lobby.Instance.IsSameRoom(mParamRoomTheme.ID))
        {
            FigureRoomScene.Instance.DestroyFigureListInfo();

            GameInfo.Instance.UseRoomDataMoveRoom(mParamRoomTheme.ID);//787878
            Lobby.Instance.MoveToRoom(mParamRoomTheme.ID);
        }
        else if (ListType == UIFigureRoomPanel.eEnvMenuType.Effect && mRoomThemeFuncData != null && !mRoomThemeFuncData.On)
        {
            if (mRoomThemeFuncData.TableData.GroupID > 0)
            {
                mFigureRoomPanel.AllRoomThemeFuncOff();
            }

            mRoomThemeFuncData.On = true;
            FigureRoomScene.Instance.ActiveRoomFunc(mRoomThemeFuncData);

            btnOff.gameObject.SetActive(true);
            sprSet.gameObject.SetActive(true);
        }
    }

    public void DoorToTheme()
    {
        LobbyUIManager.Instance.ShowUI("TopPanel", false);
        Lobby.Instance.ForceToRoom(GameInfo.Instance.UseRoomThemeData.TableID);
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN);
    }

    public void OnBtnBuy()
    {
        if (ListType == UIFigureRoomPanel.eEnvMenuType.Theme)
        {
            GameTable.StoreRoom.Param param = GameInfo.Instance.GameTable.FindStoreRoom(x => x.ProductType == (int)eREWARDTYPE.ROOMTHEME && x.ProductIndex == m_tableId);
            if (param == null)
            {
                Debug.LogError("StoreRoom 테이블에 " + m_tableId + "번 상품이 없습니다.");
                return;
            }

            UIBuyRoomPopup popup = LobbyUIManager.Instance.GetUI<UIBuyRoomPopup>("BuyRoomPopup");
            popup.Show(ParentGO.GetComponent<FComponent>(), m_curRoomTableId, param, RoomThemeBuySuccess);
        }
        else if (ListType == UIFigureRoomPanel.eEnvMenuType.Effect)
        {
            GameTable.StoreRoom.Param param = GameInfo.Instance.GameTable.FindStoreRoom(x => x.ProductType == (int)eREWARDTYPE.ROOMFUNC && x.ProductIndex == m_tableId);
            if (param == null)
            {
                Debug.LogError("StoreRoom 테이블에 " + m_tableId + "번 상품이 없습니다.");
                return;
            }

            UIBuyRoomPopup popup = LobbyUIManager.Instance.GetUI<UIBuyRoomPopup>("BuyRoomPopup");
            popup.Show(ParentGO.GetComponent<FComponent>(), m_curRoomTableId, param);
        }
    }

    public void RoomThemeBuySuccess()
    {
        GameTable.StoreRoom.Param param = GameInfo.Instance.GameTable.FindStoreRoom(x => x.ProductType == (int)eREWARDTYPE.ROOMTHEME && x.ProductIndex == m_tableId);
        if (param == null)
            return;

        if (param.ProductType == (int)eREWARDTYPE.ROOMTHEME)
        {
            GameTable.RoomTheme.Param paramRoom = GameInfo.Instance.GameTable.FindRoomTheme(param.ProductIndex);
            if (paramRoom == null)
                return;

            string themename = FLocalizeString.Instance.GetText(paramRoom.Name);

            MessagePopup.OKCANCEL(eTEXTID.APPLY, FLocalizeString.Instance.GetText(3268, themename), () =>
            {
                FigureRoomScene.Instance.DestroyFigureListInfo();

                GameInfo.Instance.UseRoomDataMoveRoom(mParamRoomTheme.ID);//787878
                Lobby.Instance.MoveToRoom(mParamRoomTheme.ID);
            });
        }

        
        
    }

    public void OnBtnOff()
    {
        if(mRoomThemeFuncData == null)
        {
            return;
        }

        mRoomThemeFuncData.On = false;
        FigureRoomScene.Instance.ActiveRoomFunc(mRoomThemeFuncData);

        btnOff.gameObject.SetActive(false);
        sprSet.gameObject.SetActive(false);
    }
}
