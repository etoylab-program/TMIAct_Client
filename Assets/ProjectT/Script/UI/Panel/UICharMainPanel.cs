using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharMainPanel : FComponent
{
    public UILabel kCharLabel;
    public UILabel kAPLabel;

    [SerializeField] private FList      _CharListInstance;
    [SerializeField] private FList      SmallCharSlotList;
    [SerializeField] private FToggle    ToggleAlignment;

    [Header("CardTeam")]
    public GameObject                           kCardTeamInfoObj;
    public UILabel                              kCardTeamInfoNameLabel;
    public GameObject                           kCardTeamChangeBtn;
    public CardTeamToolTipPopup.eCardToolTipDir kCardTeamInfoPopupDir = CardTeamToolTipPopup.eCardToolTipDir.NONE;
    public GameObject                           kCharListNoneObj;

    private List<GameTable.Character.Param> _ListChar           = new List<GameTable.Character.Param>();
    private int                             mCardFormationId    = 0;

    private bool _initSmall = false;
    private bool _initLarge = false;

    private eFilterFlag _charTypeFilter = eFilterFlag.ALL;
    private eFilterFlag _charMonTypeFilter = eFilterFlag.ALL;

    public override void Awake()
	{
		base.Awake();

		if(this._CharListInstance == null) return;
		
		this._CharListInstance.EventUpdate = this._UpdateCharListSlot;
		this._CharListInstance.EventGetItemCount = this._GetCharElementCount;

        SmallCharSlotList.EventUpdate = _UpdateCharListSlot;
        SmallCharSlotList.EventGetItemCount = _GetCharElementCount;

        ToggleAlignment.EventCallBack = OnToggleAlignment;
    }

    public override void OnEnable()
    {
        kCharListNoneObj.SetActive(false);
        SetFilterChar(_charTypeFilter, _charMonTypeFilter);
        if (GameSupport.IsTutorial())
        {
            ToggleAlignment.SetToggle(1, SelectEvent.Code);
        }
        else
        {
            if (!PlayerPrefs.HasKey("CharMainPanel_Alignment"))
            {
                ToggleAlignment.SetToggle(1, SelectEvent.Code);
            }
            else
            {
                int select = PlayerPrefs.GetInt("CharMainPanel_Alignment");
                ToggleAlignment.SetToggle(select, SelectEvent.Code);
            }
        }

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

            GameTable.Character.Param find = _ListChar.Find(x => x.ID == charData.TableID);
            if (find != null)
            {
                _ListChar.Remove(find);
                _ListChar.Insert(0, find);
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
                SmallCharSlotList.RefreshNotMove();
        }

        kCharLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1578), GameInfo.Instance.CharList.Count, GameInfo.Instance.GameTable.Characters.Count);
        kAPLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1579), GameSupport.GetMaxAP(), GameSupport.GetMaxAP(false), GameSupport.GetMaxAP() - GameSupport.GetMaxAP(false));

        mCardFormationId = GameSupport.GetSelectCardFormationID();
        if (mCardFormationId == (int)eCOUNT.NONE)
        {
            kCardTeamInfoNameLabel.textlocalize = FLocalizeString.Instance.GetText(1617);
        }
        else
        {
            GameTable.CardFormation.Param cardFrm = GameInfo.Instance.GameTable.FindCardFormation(x => x.ID == mCardFormationId);
            kCardTeamInfoNameLabel.textlocalize = FLocalizeString.Instance.GetText(cardFrm.Name);
        }

        kCharListNoneObj.SetActive(_ListChar.Count <= (int)eCOUNT.NONE);
    }

    private void _UpdateCharListSlot(int index, GameObject slotObject)
	{
        do
        {
            UICharListSlot card = slotObject.GetComponent<UICharListSlot>();
            if (null == card) break;

            GameTable.Character.Param data = null;
            if (0 <= index && _ListChar.Count > index)
            {
                data = _ListChar[index];
            }
            card.ParentGO = this.gameObject;
            card.UpdateSlot( UICharListSlot.ePos.Main, index, data);

        } while (false);
    }

    private int _GetCharElementCount()
	{
		return _ListChar.Count; //TempValue
	}

    public void OnClick_CardTeamChangeBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.CardFormationType, eCharSelectFlag.USER_INFO);
        LobbyUIManager.Instance.ShowUI("ArmoryPopup", true);
    }

    public void OnClick_CardTeamInfoBtn()
    {
        if (mCardFormationId == (int)eCOUNT.NONE)
        {
            return;
        }
        CardTeamToolTipPopup.Show(mCardFormationId, kCardTeamInfoObj, kCardTeamInfoPopupDir);
    }

    public void OnBtnChangeMainChar()
    {
        LobbyUIManager.Instance.ShowUI("MainCharSetPopup", true);
    }

    public void OnClick_FilterBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.FilterOpenUI, UIItemFilterPopup.eFilterOpenUI.CharMainPanel.ToString());
        //  추후 정렬 필터로 추가됩니다.
        LobbyUIManager.Instance.ShowUI("ItemFilterPopup", true);
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

        _ListChar.Clear();

        List<GameTable.Character.Param> charDummy = GameInfo.Instance.GameTable.Characters.FindAll(x =>
            ((_charTypeFilter == eFilterFlag.ALL) ? true : (_charTypeFilter & (eFilterFlag)(1 << x.Type - 1)) == (eFilterFlag)(1 << x.Type - 1)) &&
            ((_charMonTypeFilter == eFilterFlag.ALL) ? true : (_charMonTypeFilter & (eFilterFlag)(1 << x.MonType - 1)) == (eFilterFlag)(1 << x.MonType - 1))
        );

        for (int i = 0; i < charDummy.Count; i++)
        {
            CharData carddata = GameInfo.Instance.GetCharDataByTableID(charDummy[i].ID);
            if (carddata != null)
                _ListChar.Add(charDummy[i]);
        }

        for (int i = 0; i < charDummy.Count; i++)
        {
            CharData carddata = GameInfo.Instance.GetCharDataByTableID(charDummy[i].ID);
            if (carddata == null)
                _ListChar.Add(charDummy[i]);
        }

        

        if (refesh)
        {
            _initLarge = false;
            _initSmall = false;
        }

        Renewal(true);
    }
    private bool OnToggleAlignment(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
        {
            return false;
        }

        if(nSelect == 0) // 반절사이즈
        {
            _CharListInstance.gameObject.SetActive(false);

            SmallCharSlotList.gameObject.SetActive(true);
            if (!_initSmall)
            {
                SmallCharSlotList.UpdateList();
                _initSmall = true;
            }
            else
                SmallCharSlotList.RefreshNotMove();
                
            

            PlayerPrefs.SetInt("CharMainPanel_Alignment", 0);
        }
        else // 원사이즈
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
            

            PlayerPrefs.SetInt("CharMainPanel_Alignment", 1);
        }

        return true;
    }
}
