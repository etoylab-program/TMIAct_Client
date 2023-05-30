using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDailySlot : FSlot
{
    public UIButton kDailyBtn;
    public UILabel kDailyBtnLabel;
    public List<UIItemListSlot> kItemListSlots;

    private int _index;
    private int _tableId;
    
    public void UpdateSlot(int index, int tableId, GameTable.Random.Param[] randomArray, bool buy, bool reward, bool complete)
    {
        _index = index;
        _tableId = tableId;
        
        for (int i = 0; i < kItemListSlots.Count; i++)
        {
            if (randomArray == null || randomArray.Length <= i)
            {
                kItemListSlots[i].gameObject.SetActive(false);
                continue;
            }
            
            kItemListSlots[i].ParentGO = gameObject;
            kItemListSlots[i].gameObject.SetActive(true);
            kItemListSlots[i].UpdateSlot(UIItemListSlot.ePosType.SpecialBuyPopup_ItemInfo, i, randomArray[i]);
        }

        string text = FLocalizeString.Instance.GetText(281, index + 1);
        if (buy)
        {
            text = FLocalizeString.Instance.GetText(complete ? 1685 : 1684);
        }
        
        kDailyBtnLabel.textlocalize = text;
        kDailyBtn.isEnabled = buy && reward;
    }
    
    public void OnClick_DailyBtn()
    {
        Log.Show("OnClick_DailyBtn");
        
        GameInfo.Instance.Send_ReqUnexpectedPackageDailyReward(_tableId, _index + 1, OnNetBuyDaily);
    }

    private void OnNetBuyDaily(int result, PktMsgType pktmsg)
    {
        UISpecialBuyDailyPopup popup = ParentGO.GetComponent<UISpecialBuyDailyPopup>();
        if (popup == null)
        {
            return;
        }
        
        popup.OnClick_Reward(_index);
    }
}
