
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIFigureRoomSaveListSlot : FSlot
{
    public UILabel lbName;
    public UILabel lbSavedDay = null;
    public UISprite sprSel;
    public UITexture texSavedPicture = null;

    public int index { get; private set; }

    ///*
    public void UpdateFigureRoomSlot(int index)
    {
        this.index = index;
        Select(false);

        FLocalizeString.SetLabel(lbName, 1230, index + 1);
    }
    //*/
    public void OnBtnClick()
    {
        UIFigureRoomSaveLoadPanel popup = ParentGO.GetComponent<UIFigureRoomSaveLoadPanel>();
        if (popup != null)
        {
            popup.SelectSlot(this);
        }
    }

    public void Select(bool select)
    {
        sprSel.gameObject.SetActive(select);
    }

    public void SetSavedPicture(Texture texture)
    {
        if (texSavedPicture != null)
        {
            texSavedPicture.gameObject.SetActive(texture != null);
            texSavedPicture.mainTexture = texture;
        }
    }

    public void SetSavedCreateDay(System.DateTime day)
    {
        if (lbSavedDay != null)
        {
            if (day.Year < 2000)
            {
                lbSavedDay.gameObject.SetActive(false);
            }
            else
            {
                lbSavedDay.gameObject.SetActive(true);
                lbSavedDay.textlocalize = string.Format("{0:D4}.{1:D2}.{2:D2}", day.Year, day.Month, day.Day);
            }
        }
    }
}
