
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIFigureRoomSetIconListSlot : FSlot
{
    public UITexture texIcon;

    private int m_index = 0;
    private FigureData m_data = null;


    public void UpdateSlot(int index, FigureData figuredata)
    {
        m_index = index;
        m_data = figuredata;

        if (m_data.tableData.ContentsType == (int)eContentsPosKind.COSTUME)
        {
            texIcon.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Char/MainSlot/{0}", m_data.tableData.Icon)) as Texture;
        }
        else if (m_data.tableData.ContentsType == (int)eContentsPosKind.MONSTER)
        {
            texIcon.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Monster/{0}", m_data.tableData.Icon)) as Texture;
        }
        else if (m_data.tableData.ContentsType == (int)eContentsPosKind.WEAPON)
        {
            texIcon.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Item/{0}", m_data.tableData.Icon)) as Texture;
        }
    }

    public void OnBtnClick()
    {
        if (m_data == null)
        {
            Debug.LogError("피규어 데이터가 없습니다. UIFigureRoomIconListSlot::" + m_index);
            return;
        }

        UIFigureRoomPanel figureRoomPanel = LobbyUIManager.Instance.GetUI<UIFigureRoomPanel>("FigureRoomPanel");
        figureRoomPanel.SelectFigureMoveToEditMode(m_data);
    }
}
