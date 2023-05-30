using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICombo : MonoBehaviour
{
    public UISprite kBG;
    public UISprite kComboText;

    public int comboType;

    private void Awake()
    {
        SetCombo(comboType);
    }

    public void SetCombo(int type)
    {
        //type 0: 일반초록색 type1: 특수??빨간색

        switch(type)
        {
            case 0:
                kBG.spriteName = "frm_combo_normal";
                kComboText.color = new Color(199f, 255f, 200f, 255f);
                break;
            case 1:
                kBG.spriteName = "frm_combo_special";
                kComboText.color = new Color(255, 228, 199, 255);
                break;
        }
    }
}