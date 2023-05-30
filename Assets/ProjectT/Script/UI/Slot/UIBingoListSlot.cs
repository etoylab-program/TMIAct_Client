using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBingoListSlot : FSlot
{
    [SerializeField] private UIItemListSlot itemListSlot = null;
    [SerializeField] private UISprite clearSpr = null;

    private bool _isClear = false;
    public bool IsClear => _isClear;

    public void UpdateSlot(GameTable.Item.Param itemParam, bool bingoClear)
    {
        _isClear = bingoClear;

        itemListSlot.UpdateSlot(UIItemListSlot.ePosType.ItemInfo, 0, itemParam);

        clearSpr.SetActive(_isClear);
    }

    public void SetCountLabel(string text)
    {
        itemListSlot.SetCountLabel(text);
    }
}
