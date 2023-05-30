
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIFigureRoomIconListSlot : FSlot
{
    public UISprite sprDisable;
    public UITexture texIcon;
    public UISprite sprSet;
    public UIButton btnBuy;
    public UIButton btnSub;

    private int m_index = 0;
    private int m_curRoomTableId = 0;
    private int m_tableId = 0;

    //private FigureData m_data = null;
    // 슬롯데이터로 일단 대체 -dc-
    private RoomThemeFigureSlotData m_data = null;

    //787878
    ///*
    public void UpdateSlot(int index, RoomThemeSlotData roomData, GameTable.RoomFigure.Param param)
    {
        m_index = index;
        m_curRoomTableId = (int)roomData.TableID;
        m_tableId = param.ID;

        sprDisable.gameObject.SetActive(true);
        sprSet.gameObject.SetActive(false);
        btnBuy.gameObject.SetActive(true);
        btnSub.gameObject.SetActive(false);

        if (param.ContentsType == (int)eContentsPosKind.COSTUME)
            texIcon.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Char/MainSlot/{0}", param.Icon)) as Texture;
        else if (param.ContentsType == (int)eContentsPosKind.MONSTER)
            texIcon.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Monster/{0}", param.Icon)) as Texture;
        else if (param.ContentsType == (int)eContentsPosKind.WEAPON)
            texIcon.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Item/{0}", param.Icon)) as Texture;

        // 구매 이력이 있으면 배치 가능.
        bool _has = GameInfo.Instance.HasFigure(m_tableId);
        if (_has)
        {
            sprDisable.gameObject.SetActive(false);
            btnBuy.gameObject.SetActive(false);

            m_data = new RoomThemeFigureSlotData(m_tableId);
            var _hasFigure = FigureRoomScene.Instance.ListFigureInfo.Find(x => x.figure.tableId == m_tableId);
            
            //해당 룸에 배치가 되어 있음.
            if (_hasFigure != null)
            {
                if (_hasFigure.data.placement)
                {
                    sprSet.gameObject.SetActive(true);
                    btnSub.gameObject.SetActive(true);
                }
            }
        }
    }

    public void OnBtnClick()
    {
        if (sprDisable.gameObject.activeSelf)
            return;

        UIFigureRoomPanel figureRoomPanel = LobbyUIManager.Instance.GetUI<UIFigureRoomPanel>("FigureRoomPanel");
        if (!figureRoomPanel.PossibleToSet())
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3045));
            return;
        }

        FigureRoomScene.Instance.PlacementFigure(m_data.TableID, true);

        figureRoomPanel.SaveIconIndex(m_index);
        figureRoomPanel.Renewal(true);
    }

    public void OnBtnBuy()
    {
        GameTable.StoreRoom.Param param = GameInfo.Instance.GameTable.FindStoreRoom(x => x.ProductType == (int)eREWARDTYPE.ROOMFIGURE && x.ProductIndex == m_tableId);
        if (param == null)
        {
            Debug.LogError("StoreRoom 테이블에 " + m_tableId + "번 상품이 없습니다.");
            return;
        }

        UIBuyRoomPopup popup = LobbyUIManager.Instance.GetUI<UIBuyRoomPopup>("BuyRoomPopup");
        popup.Show(ParentGO.GetComponent<FComponent>(), m_curRoomTableId, param);

        UIFigureRoomPanel figureRoomPanel = LobbyUIManager.Instance.GetUI<UIFigureRoomPanel>("FigureRoomPanel");
        if(figureRoomPanel)
        {
            figureRoomPanel.SaveIconIndex(m_index);
        }
    }

    public void OnBtnSub()
    {
        FigureRoomScene.Instance.PlacementFigure(m_data.TableID, false);

        UIFigureRoomPanel figureRoomPanel = LobbyUIManager.Instance.GetUI<UIFigureRoomPanel>("FigureRoomPanel");
        if (figureRoomPanel)
        {
            figureRoomPanel.SaveIconIndex(m_index);
            figureRoomPanel.Renewal(true);
        }
    }
}
