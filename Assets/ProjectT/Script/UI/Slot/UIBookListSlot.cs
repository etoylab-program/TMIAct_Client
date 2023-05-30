using UnityEngine;
using System.Collections;

public class UIBookListSlot : FSlot
{
    public UISprite kBGSpr;
    public UILabel kCountLabel;
    public UIItemListSlot kItemListSlot;
    private int _index;
    private GameClientTable.Book.Param _tabledata;

    public void UpdateSlot(int index, GameClientTable.Book.Param tabledata, int group)  //Fill parameter if you need
    {
        _index = index;
        _tabledata = tabledata;
        
        kItemListSlot.gameObject.SetActive(false);
        kBGSpr.gameObject.SetActive(true);
        if (_tabledata == null)
        {
            kBGSpr.spriteName = "itembgSlot_binitem";
            kCountLabel.textlocalize = "";
            return;
        }

        kBGSpr.gameObject.SetActive(false);
        kBGSpr.spriteName = "itembgSlot_bookitem";

        string strnum = string.Format(FLocalizeString.Instance.GetText(216), _tabledata.Num);

        if (_tabledata.Group == (int)eBookGroup.Weapon)
        {
            GameTable.Weapon.Param data = GameInfo.Instance.GameTable.FindWeapon(x => x.ID == _tabledata.ItemID);
            if (data != null)
            {
                kItemListSlot.gameObject.SetActive(true);
                kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Book, index, data);
                kItemListSlot.SetCountLabel(strnum);
            }

            WeaponBookData bookdata = GameInfo.Instance.WeaponBookList.Find(x => x.TableID == _tabledata.ItemID);
            if (bookdata != null)
            {
                bool bnew = bookdata.IsOnFlag(eBookStateFlag.NEW_CHK);
                kItemListSlot.SetActiveNew(!bnew);
            }
            else
            {
                kItemListSlot.SetActiveNew(false);
                kItemListSlot.kInactiveSpr.SetActive(true);
            }
        }
        else if (_tabledata.Group == (int)eBookGroup.Supporter)
        {
            GameTable.Card.Param data = GameInfo.Instance.GameTable.FindCard(x => x.ID == _tabledata.ItemID);
            if (data != null)
            {
                kItemListSlot.gameObject.SetActive(true);
                kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Book, index, data);
                kItemListSlot.SetCountLabel(strnum);
            }

            CardBookData bookdata = GameInfo.Instance.CardBookList.Find(x => x.TableID == _tabledata.ItemID);
            if (bookdata != null)
            {
                // State 가 0일 경우 new
                bool bnew = bookdata.IsOnFlag(eBookStateFlag.NEW_CHK);
                kItemListSlot.SetActiveNew(!bnew);

                bool bfavor = bookdata.IsOnFlag(eBookStateFlag.MAX_FAVOR_LV);
                if (bfavor)
                {
                    if (PlayerPrefs.HasKey("NCardBook_Favor_" + bookdata.TableID.ToString()) && data.Grade >= (int)eGRADE.GRADE_SR)
                        kItemListSlot.SetActiveNotice(true);
                }
            }
            else
            {
                kItemListSlot.SetActiveNew(false);
                kItemListSlot.kInactiveSpr.SetActive(true);
            }
        }
        else if (_tabledata.Group == (int)eBookGroup.Monster)
        {
            GameClientTable.BookMonster.Param data = GameInfo.Instance.GameClientTable.FindBookMonster(x => x.ID == _tabledata.ItemID);
            if (data != null)
            {
                kItemListSlot.gameObject.SetActive(true);
                kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Book, index, data);
                kItemListSlot.SetCountLabel(strnum);
            }

            MonsterBookData bookdata = GameInfo.Instance.MonsterBookList.Find(x => x.TableID == _tabledata.ItemID);
            if (bookdata != null)
            {
                // State 가 0일 경우 new
                bool bnew = bookdata.IsOnFlag(eBookStateFlag.NEW_CHK);
                kItemListSlot.SetActiveNew(!bnew);
            }
            else
            {
                kItemListSlot.SetActiveNew(false);
                kItemListSlot.kInactiveSpr.SetActive(true);
            }

        }
    }

    public void OnClick_Slot()
    {
        bool bnewclick = false;

        if (_tabledata == null)
            return;

        Log.Show(_tabledata.ItemID);

        if (_tabledata.Group == (int)eBookGroup.Weapon)
        {
            WeaponBookData bookdata = GameInfo.Instance.WeaponBookList.Find(x => x.TableID == _tabledata.ItemID);
            if (bookdata != null)
            {
                if (!bookdata.IsOnFlag(eBookStateFlag.NEW_CHK))
                    bnewclick = true;
            }

            UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, _tabledata.ItemID);
            LobbyUIManager.Instance.ShowUI("BookWeaponInfoPopup", true);
        }
        else if (_tabledata.Group == (int)eBookGroup.Supporter)
        {
            CardBookData bookdata = GameInfo.Instance.CardBookList.Find(x => x.TableID == _tabledata.ItemID);
            if (bookdata != null)
            {
                if (!bookdata.IsOnFlag(eBookStateFlag.NEW_CHK))
                    bnewclick = true;
            }

            UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, _tabledata.ItemID);
            LobbyUIManager.Instance.ShowUI("BookCardInfoPopup", true);
        }
        else if (_tabledata.Group == (int)eBookGroup.Monster)
        {
            MonsterBookData bookdata = GameInfo.Instance.MonsterBookList.Find(x => x.TableID == _tabledata.ItemID);
            if (bookdata != null)
            {
                if (!bookdata.IsOnFlag(eBookStateFlag.NEW_CHK))
                    bnewclick = true;
            }

            UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, _tabledata.ItemID);
            LobbyUIManager.Instance.ShowUI("BookMonsterInfoPopup", true);
        }


        if (bnewclick)
            GameInfo.Instance.Send_ReqBookNewConfirm(_tabledata.Group, _tabledata.ItemID, OnNetSetBookState);
            

    }

    public void OnNetSetBookState(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        // 결과가 성공했으므로 false 시켜버린다. (New 버튼의 활성화 비활성화만 갱신하면 됨.
        // 해당 함수에 들어왔음은 state 변화가 정상적으로 처리 됐음을 의미하므로 Active를 비활성화
        kItemListSlot.SetActiveNew(false);

        // Data 유효성을 확인하고 변경할 경우. (해당 slot 의 data가 어딘가에서 변경될수도 있다고 하면) 아래코드사용
        /*
        if (_tabledata.Group == (int)eBookGroup.Weapon)
        {
            var bookdata = GameInfo.Instance.WeaponBookList.Find(x => x.TableID == _tabledata.ItemID);
            if (bookdata == null)
                return;

            kItemListSlot.SetActiveNew(bookdata.State == 0);
        }
        else if (_tabledata.Group == (int)eBookGroup.Supporter)
        {
            var bookdata = GameInfo.Instance.CardBookList.Find(x => x.TableID == _tabledata.ItemID);
            if (bookdata == null)
                return;

            kItemListSlot.SetActiveNew(bookdata.State == 0);
        }
        else if (_tabledata.Group == (int)eBookGroup.Monster)
        {
            var bookdata = GameInfo.Instance.MonsterBookList.Find(x => x.TableID == _tabledata.ItemID);
            if (bookdata == null)
                return;

            kItemListSlot.SetActiveNew(bookdata.State == 0);
        }
        //*/


        // 해당 슬롯만 갱신해주면 되므로 전체 Popup의 Renewal 은 진행하지 않는다.
        // LobbyUIManager.Instance.Renewal("BookItemListPopup");
    }
}
