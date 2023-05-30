using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBookCharListPopup : FComponent
{
    public UILabel kHaveLabel;
    public UILabel kHaveLimitedLabel;

    [SerializeField] private FList      _CharListInstance;
    [SerializeField] private FList      SmallCharSlotList;
    [SerializeField] private FToggle    ToggleAlignment;
    public GameObject                   kCharListNoneObj;

    private List<GameTable.Character.Param> _charlist = new List<GameTable.Character.Param>();
    public List<GameTable.Character.Param> CharList { get { return _charlist; } }

    private bool _initSmall = false;
    private bool _initLarge = false;

    private eFilterFlag _charTypeFilter = eFilterFlag.ALL;
    private eFilterFlag _charMonTypeFilter = eFilterFlag.ALL;

    public override void Awake()
    {
        base.Awake();

        if (this._CharListInstance == null) return;

        this._CharListInstance.EventUpdate = this._UpdateCharListSlot;
        this._CharListInstance.EventGetItemCount = this._GetCharElementCount;

        SmallCharSlotList.EventUpdate = _UpdateCharListSlot;
        SmallCharSlotList.EventGetItemCount = _GetCharElementCount;

        ToggleAlignment.EventCallBack = OnToggleAlignment;
    }

    public override void OnEnable()
    {
        SetFilterChar(_charTypeFilter, _charMonTypeFilter);

        if (!PlayerPrefs.HasKey("BookCharList_Alignment"))
        {
            ToggleAlignment.SetToggle(1, SelectEvent.Code);
        }
        else
        {
            int select = PlayerPrefs.GetInt("BookCharList_Alignment");
            ToggleAlignment.SetToggle(select, SelectEvent.Code);
        }

        kCharListNoneObj.SetActive(false);

        base.OnEnable();
    }

    public void SetCharDefaultFilter()
    {
        UIItemFilterPopup filterPopup = LobbyUIManager.Instance.GetUI<UIItemFilterPopup>("ItemFilterPopup");
        if (filterPopup != null)
        {
            filterPopup.DefailtTab = true;
        }

        _charTypeFilter = eFilterFlag.ALL;
        _charMonTypeFilter = eFilterFlag.ALL;

        _initSmall = false;
        _initLarge = false;
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        if (!bChildren)
            return;

        for (int i = GameInfo.Instance.UserData.ArrLobbyBgCharUid.Length - 1; i >= 0; --i)
        {
            long uid = GameInfo.Instance.UserData.ArrLobbyBgCharUid[i];

            CharData charData = GameInfo.Instance.GetCharData(uid);
            if (charData == null)
            {
                continue;
            }

            GameTable.Character.Param find = _charlist.Find(x => x.ID == charData.TableID);
            if (find != null)
            {
                _charlist.Remove(find);
                _charlist.Insert(0, find);
            }
        }

        if (_CharListInstance.gameObject.activeSelf)
        {
            if (!_initLarge)
            {
                _CharListInstance.UpdateList();
                _initLarge = true;
            }
            else
            {
                _CharListInstance.RefreshNotMove();
            }
            
        }
        else
        {
            if (!_initSmall)
            {
                SmallCharSlotList.UpdateList();
                _initSmall = true;
            }
            else
            {
                SmallCharSlotList.RefreshNotMove();
            }
            
        }

        int now = GameInfo.Instance.CharList.Count;
        int max = GameInfo.Instance.GameTable.Characters.Count;

        kHaveLabel.textlocalize = string.Format("{0:#,##0}", now);
        kHaveLimitedLabel.textlocalize = string.Format("/{0:#,##0}", max);

        kCharListNoneObj.SetActive(_charlist.Count <= (int)eCOUNT.NONE);
    }

    private void _UpdateCharListSlot(int index, GameObject slotObject)
    {
        do
        {
            UICharListSlot card = slotObject.GetComponent<UICharListSlot>();
            if (null == card) break;

            GameTable.Character.Param data = null;
            if (0 <= index && _charlist.Count > index)
            {
                data = _charlist[index];
            }
            card.ParentGO = this.gameObject;
            card.UpdateSlot(UICharListSlot.ePos.BookCharList, index, data);

        } while (false);
    }

    private int _GetCharElementCount()
    {
        return _charlist.Count; //TempValue
    }

    public void SetFilterChar(eFilterFlag charTypeFilter, eFilterFlag charMonTypeFilter, bool refesh = false)
    {
        if (_charTypeFilter == charTypeFilter && _charMonTypeFilter == charMonTypeFilter)
        {
            if (refesh)
            {
                refesh = false;
            }
        }

        _charTypeFilter = charTypeFilter;
        _charMonTypeFilter = charMonTypeFilter;

        _charlist.Clear();

        List<GameTable.Character.Param> charDummy = GameInfo.Instance.GameTable.Characters.FindAll(x =>
            ((_charTypeFilter == eFilterFlag.ALL) ? true : (_charTypeFilter & (eFilterFlag)(1 << x.Type - 1)) == (eFilterFlag)(1 << x.Type - 1)) &&
            ((_charMonTypeFilter == eFilterFlag.ALL) ? true : (_charMonTypeFilter & (eFilterFlag)(1 << x.MonType - 1)) == (eFilterFlag)(1 << x.MonType - 1))
        );

        for (int i = 0; i < charDummy.Count; i++)
        {
            CharData carddata = GameInfo.Instance.GetCharDataByTableID(charDummy[i].ID);
            if (carddata != null)
                _charlist.Add(charDummy[i]);
        }

        for (int i = 0; i < charDummy.Count; i++)
        {
            CharData carddata = GameInfo.Instance.GetCharDataByTableID(charDummy[i].ID);
            if (carddata == null)
                _charlist.Add(charDummy[i]);
        }



        if (refesh)
        {
            _initLarge = false;
            _initSmall = false;
        }

        Renewal(true);
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

    private bool OnToggleAlignment(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
        {
            return false;
        }

        if (nSelect == 0)
        {
            _CharListInstance.gameObject.SetActive(false);

            SmallCharSlotList.gameObject.SetActive(true);
            if (!_initSmall)
            {
                SmallCharSlotList.UpdateList();
                _initSmall = true;
            }
            else
            {
                SmallCharSlotList.RefreshNotMove();
            }
            

            PlayerPrefs.SetInt("BookCharList_Alignment", 0);
        }
        else
        {
            SmallCharSlotList.gameObject.SetActive(false);

            _CharListInstance.gameObject.SetActive(true);

            if (!_initLarge)
            {
                _CharListInstance.UpdateList();
                _initLarge = true;
            }
            else
            {
                _CharListInstance.RefreshNotMove();
            }
            

            PlayerPrefs.SetInt("BookCharList_Alignment", 1);
        }

        return true;
    }

    public void OnClick_FilterBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.FilterOpenUI, UIItemFilterPopup.eFilterOpenUI.BookCharListPopup.ToString());
        //  추후 정렬 필터로 추가됩니다.
        LobbyUIManager.Instance.ShowUI("ItemFilterPopup", true);
    }
}
