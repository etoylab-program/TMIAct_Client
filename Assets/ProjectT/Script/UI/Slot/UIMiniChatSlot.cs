using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMiniChatSlot : FSlot
{
    [Header("UIMiniChatSlot")]
    [SerializeField] private UILabel nameLabel = null;
    [SerializeField] private UILabel chatLabel = null;

    public void UpdateSlot(CircleChatData circleChatData)
    {
        if (circleChatData == null)
        {
            return;
        }

        nameLabel.textlocalize = circleChatData.UserName;
        chatLabel.textlocalize = circleChatData.Content;
    }
}
