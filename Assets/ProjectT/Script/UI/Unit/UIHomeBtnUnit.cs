using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHomeBtnUnit : FUnit
{
    public void OnClick_HomeBtn()
    {
        Log.Show("OnClick_HomeBtn");

        LobbyUIManager.Instance.HomeBtnEvent();
    }
}
