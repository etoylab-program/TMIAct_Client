using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChatInfo
{
    public GameObject ParentObj;
    public UITexture MarkTex;
    public UILabel NameLabel;
    public GameObject ContentObj;
    public UILabel ContentLabel;
    public UILabel ContentTimeLabel;
    public GameObject StampObj;
    public UITexture StampTex;
    public UILabel StampTimeLabel;
}

public class UICircleChatListSlot : FSlot
{
    [Header("UICircleChatListSlot")]
    [SerializeField] private ChatInfo leftChat = null;
    [SerializeField] private ChatInfo rightChat = null;

    private int _index;
    private CircleChatData _circleChatData;
    public void UpdateSlot(int index, CircleChatData circleChatData, bool isPlayer)
    {
        _index = index;
        _circleChatData = circleChatData;

        if (_circleChatData == null)
        {
            return;
        }

        leftChat.ParentObj.SetActive(!isPlayer);
        rightChat.ParentObj.SetActive(isPlayer);

        if (isPlayer)
        {
            SetChatData(ref rightChat);
        }
        else
        {
            SetChatData(ref leftChat);
        }
    }

    private void SetChatData(ref ChatInfo chatInfo)
    {
        LobbyUIManager.Instance.GetUserMarkIcon(this.gameObject, this.gameObject, _circleChatData.UserMarkId, ref chatInfo.MarkTex);
        chatInfo.NameLabel.textlocalize = _circleChatData.UserName;
        chatInfo.ContentTimeLabel.textlocalize = chatInfo.StampTimeLabel.textlocalize = _circleChatData.ChatTime.ToString("yyyy-MM-dd HH:mm");
        chatInfo.ContentLabel.textlocalize = _circleChatData.Content;

        Texture stampMainTexture = null;
        GameTable.ChatStamp.Param chatStampParam = GameInfo.Instance.GameTable.FindChatStamp(_circleChatData.StampId);
        if (chatStampParam != null)
        {
            stampMainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", Utility.AppendString("Icon/Circle/", chatStampParam.Icon, ".png")) as Texture;
        }

        chatInfo.StampTex.mainTexture = stampMainTexture;

        chatInfo.ContentObj.SetActive(chatStampParam == null);
        chatInfo.StampObj.SetActive(chatStampParam != null);
    }
}
