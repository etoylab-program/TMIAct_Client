
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIFigureRoomJointLockListSlot : FSlot
{
    public UILabel lbName;
    public UISprite sprSel;
    public UIButton btnSub;

    private UIFigureRoomEditModePanel m_figureRoomEditModePanel = null;

    public int index { get; private set; }
    public FigureUnit.sEditableBoneData boneData { get; private set; }


    public void UpdateSlot(int index, FigureUnit.sEditableBoneData boneData, UIFigureRoomEditModePanel figureRoomEditModePanel)
    {
        m_figureRoomEditModePanel = figureRoomEditModePanel;

        this.index = index;
        this.boneData = boneData;

        //lbName.textlocalize = ikObj.name;

        if (figureRoomEditModePanel.SelectedJoinHoldSlot == null || figureRoomEditModePanel.SelectedJoinHoldSlot.index != index)
            Unselect();
        else
            Select();
    }

    public void Select()
    {
        sprSel.gameObject.SetActive(true);
        btnSub.gameObject.SetActive(true);
    }

    public void Unselect()
    {
        sprSel.gameObject.SetActive(false);
        btnSub.gameObject.SetActive(false);
    }

    public void OnBtnClick()
    {
        Select();
    }

    public void OnBtnSub()
    {
    }
}
