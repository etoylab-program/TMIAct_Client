using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBookItemListPopup : FComponent
{
    [SerializeField] private FList _ItemListInstance;
    public UILabel kHaveLabel;
    public UILabel kHaveLimitedLabel;
    private int _booklisttype = (int)eBookGroup.None;
    private List<GameClientTable.Book.Param> _booklist = new List<GameClientTable.Book.Param>();

    public override void Awake()
    {
        base.Awake();

        if (this._ItemListInstance == null) return;

        this._ItemListInstance.EventUpdate = this._UpdateItemListSlot;
        this._ItemListInstance.EventGetItemCount = this._GetItemElementCount;
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        _booklisttype = (int)UIValue.Instance.GetValue(UIValue.EParamType.BookItemListType);
        _booklist = GameInfo.Instance.GameClientTable.FindAllBook(x => x.Group == _booklisttype);

        kHaveLimitedLabel.textlocalize = string.Format("/{0:#,##0}", _booklist.Count);

        int listmaxcount = 8 * 4;
        int count = 0;
        if (_booklist.Count < listmaxcount)
            count = listmaxcount - _booklist.Count;
        else
            count = 8 - (_booklist.Count % 8);

        if (count < 8)
        {
            for (int i = 0; i < count; i++)
                _booklist.Add(null);
        }

        this._ItemListInstance.UpdateList();
        _ItemListInstance.ScrollPositionSet();
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        int now = 0;
        if (_booklisttype == (int)eBookGroup.Weapon)
            now = GameInfo.Instance.WeaponBookList.Count;
        else if (_booklisttype == (int)eBookGroup.Supporter)
            now = GameInfo.Instance.CardBookList.Count;
        else if (_booklisttype == (int)eBookGroup.Monster)
            now = GameInfo.Instance.MonsterBookList.Count;

        kHaveLabel.textlocalize = string.Format("{0:#,##0}", now);
        this._ItemListInstance.RefreshNotMove();

        
    }

    private void _UpdateItemListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIBookListSlot card = slotObject.GetComponent<UIBookListSlot>();
            if (null == card) break;

            GameClientTable.Book.Param data = null;
            if (0 <= index && _booklist.Count > index)
            {
                data = _booklist[index];
            }
            card.ParentGO = this.gameObject;
            card.UpdateSlot(index, data, _booklisttype);
        } while (false);
    }

    private int _GetItemElementCount()
    {
        return _booklist.Count; //TempValue
    }

    public void OnClick_BackBtn()
    {
        OnClickClose();
    }

    public override void OnClickClose()
    {
        NotificationManager.Instance.Init();
        LobbyUIManager.Instance.Renewal("BookMainPopup");
        base.OnClickClose();
    }

}
