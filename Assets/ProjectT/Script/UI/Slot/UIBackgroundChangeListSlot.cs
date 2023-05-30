using UnityEngine;
using System.Collections;

public class UIBackgroundChangeListSlot : FSlot 
{	
	public UILabel kNameLabel;

	private int TableIndex = 0;
	private GameTable.LobbyTheme.Param LobbyThemeParam;

	public void Init()
    {
		TableIndex = 0;
    }

	public void UpdateSlot(int Index) 	//Fill parameter if you need
	{
		if (TableIndex == Index)
			return;

		TableIndex = Index;
		if(TableIndex <= 0)
        {
			LobbyThemeParam = null;
			kNameLabel.textlocalize = string.Empty;
			return;
        }

		LobbyThemeParam = GameInfo.Instance.GameTable.LobbyThemes.Find(x => x.ID == TableIndex);
		if(LobbyThemeParam == null)
        {
			kNameLabel.textlocalize = string.Empty;
			return;
        }

		kNameLabel.textlocalize = FLocalizeString.Instance.GetText(LobbyThemeParam.Name);
	}

    public void OnClick_Slot()
	{
        if (GameInfo.Instance.UserData.UserLobbyThemeID == LobbyThemeParam.ID)
        {
			string lobbyScreenShot = PlayerPrefs.GetString("LobbyBGWithScreenShot", "false");
			if (lobbyScreenShot.ToLower().Equals("true"))
			{
				Lobby.Instance.SetBackGround(LobbyThemeParam);
				return;
			}

            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1649));
            return;
        }

		GameInfo.Instance.Send_ReqUserSetLobbyTheme((uint)LobbyThemeParam.ID, OnNetUserSetLobbyTheme);
	}

	private void OnNetUserSetLobbyTheme(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        Lobby.Instance.SetBackGround(LobbyThemeParam);
        LobbyUIManager.Instance.GetUI<UIMainPanel>().OnClick_BackgroundChange();
    }
}
