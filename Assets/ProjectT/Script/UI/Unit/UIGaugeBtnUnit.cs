using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGaugeBtnUnit : FUnit
{
	public List<UISprite> kGaugeSprList;
	public UILabel kTextLLabel;
	public UILabel kTextRLabel;
    public bool bValueShow;
    public int Value;
    private int _type;
	public void InitUIGaugeBtnUnit( string strl, string strr, int value, int type, bool bvalueshow = false )
	{
        kTextLLabel.textlocalize = strl;
        kTextRLabel.textlocalize = strr;
        bValueShow = bvalueshow;
        SetValue(value);
        _type = type;
    }

    public void SetValue( int value)
    {
        SetGaugeSpr(value, false );
    }

    private void SetGaugeSpr( int value, bool click )
    {
        Value = value;

        for (int i = 0; i < kGaugeSprList.Count; i++)
            kGaugeSprList[i].enabled = false;

        for (int i = 0; i < value; i++)
            kGaugeSprList[i].enabled = true;

        if( bValueShow )
            kTextRLabel.textlocalize = Value.ToString();

        if(click)
        {
			UIOptionPopup popup = null;
			if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
			{
				popup = LobbyUIManager.Instance.GetUI<UIOptionPopup>("OptionPopup");
			}
			else
			{
				popup = GameUIManager.Instance.GetUI<UIOptionPopup>("OptionPopup");
			}

            if (popup != null)
                popup.OnClickGauge(_type);
        }
    }

    public void OnClick_GaugeBtnBtn( int i )
	{
        if(kGaugeSprList[i].enabled == false)
            SetGaugeSpr(i+1, true);
        else
            SetGaugeSpr(i, true);



    }
}
