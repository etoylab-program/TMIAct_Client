
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIArenaTowerSelectFriendCharSlot : FSlot
{
    [Header("[Property]")]
    public UITexture                TexUserMark;
    public UILabel                  LbRank;
    public UILabel                  LbName;
    public UILabel                  LbId;
    public List<UIUserCharListSlot> ListCharSlot;

    public void UpdateSlot(int index, TeamData teamData, int selectedFriendCharIndex, int selectCharIndex)
    {
        LobbyUIManager.Instance.GetUserMarkIcon(ParentGO, this.gameObject, teamData.UserMark, ref TexUserMark);

        LbRank.textlocalize = string.Format(FLocalizeString.Instance.GetText(210), teamData.UserLv);
        LbName.textlocalize = teamData.GetUserNickName();
        LbId.textlocalize = teamData.UUID.ToString();

        for(int i = 0; i < ListCharSlot.Count; i++)
        {
            ListCharSlot[i].UpdateArenaTeamSlot(i, null);
        }

        int totalCharIndex = 0;
        for(int i = 0; i < teamData.charlist.Count; i++)
        {
            totalCharIndex = (index * 3) + i;

            ListCharSlot[i].ParentGO = gameObject;
            ListCharSlot[i].UpdateArenaTowerFriendCharSlot(totalCharIndex, teamData.charlist[i].CharData, totalCharIndex == selectedFriendCharIndex, selectCharIndex);
        }
    }
}
