using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharSeletePopup : FComponent
{
    public enum eCharSortType
    {
        NONE,
        Get,        //ȹ��
        Level,      //����
        MainStat,   //������
        SecretQuestCount ,//����(����ӹ�)
        _MAX_,
    }

    public enum eCharSortOrder
    {
        SortOrder_Up,
        SortOrder_Down,
    }

	[SerializeField] private FList _CharListInstance;
    [SerializeField] private FList _SmallCharListInstance;
    [SerializeField] private FToggle _ToggleAlignment;
	public UIButton kHPFacilityBtn;
	public UIButton kHPItemBtn;
    public UIButton kCharInfoBtn;
    public UILabel kFacilitySelectDescLb;

    private List<CharData> _charlist = new List<CharData>();
    private int _type;
    private int _seleteindex;

    public int SeleteIndex { get { return _seleteindex; } }

    private FacilityData _facilitydata;

    public GameObject kSeleteBtn;

    public GameObject kCombatPowerObj;
    public UILabel kCombatPowerLb;

    public UIButton kArenaSelectBtn;
    public UILabel kArenaSelectLb;
    public UISprite kArenaSelectSpr;

	[Header("[Use Char GradeUp Item]")]
	public UISprite	SprGradeIcon;
	public UILabel	LbUseCharGradeUpItem;

    [Header("Sort/Order")]
    public UILabel kSortLabel;
    public UILabel kOrderLabel;
    public UISprite kOrderSpr;

    [Header("Secret Quest")]
    public GameObject _SecretObj;
    public UIButton _SecretCharInfoBtn;
    //�Ѹ� �⵿�Ҷ�
    public GameObject _SecretOneCharObj;
    public UIButton _SecretOneCharStartBtn;
    public UILabel _SecretOneCharApLabel;
    //���� ���� ���� �Ҷ�
    public GameObject _SecretMultiCharObj;
    public UIButton _SecretMultiCharStartBtn;
    public UILabel _SecretMultiCharApLabel;    
    public UILabel _SecretMultiCharSelectCharCountLabel;

    //�����ӹ�(��������) ���� �Ҷ�
    public GameObject _SecretMultiCharQuickObj;
    public UIButton _SecretMultiCharQuickStartBtn;
    public UIButton _SecretMultiCharQuickSubjugationBtn;
    public UILabel _SecretMultiCharAutoItemCountLabel;

    //��� �ӹ� ���� �߰�
    private List<int> seleteSubIndexList = new List<int>();
    public List<int> SeleteSubIndexList { get { return seleteSubIndexList; } }
    private int lastSelectIndex;
    private bool isClearSecret;
    public bool IsClearSecret { get { return isClearSecret; } }
    private int currentSelectCharacterCount;
    private int maxSelectCharacter;
    private int secretNeedAp;
    private List<long> selectSubCharCuidList = new List<long>();

    [Header("Favor Buff Char")]
    public UIButton kUndeployBtn;

    private int _arenaSelectSlotIdx;

    private bool _initSmall = false;
    private bool _initLarge = false;

    private eCharSortType _sortType = eCharSortType.Get;
    private eCharSortOrder _sortOrder = eCharSortOrder.SortOrder_Up;

    private readonly string[] m_sortIcon = { "ico_Filter1", "ico_Filter2" };

	private UIPanel		mPanel				= null;
	private UIPanel[]	mArrCharListPanel	= null;
	private int			mOriginalDepth		= 0;

    private int _stageId = 0;
    private long _selectCuid = 0;

    private int     mSelectedRaidCharIndex  = -1;
    private long    mSelectedRaidCuid       = -1;

    private int mSelectFavorBuffCharIndex = -1;
    private long mSelectFavorBuffCuid = -1;

    private Dictionary<long, int> mDicCombat = null;

	public override void Awake()
	{
		base.Awake();

		mPanel = GetComponent<UIPanel>();
		mArrCharListPanel = GetComponentsInChildren<UIPanel>(true);
		mOriginalDepth = mPanel.depth;

		if (this._CharListInstance == null) return;
		
		this._CharListInstance.EventUpdate = this._UpdateCharListSlot;
		this._CharListInstance.EventGetItemCount = this._GetCharElementCount;

        if (this._SmallCharListInstance == null) return;
        this._SmallCharListInstance.EventUpdate = this._UpdateCharListSlot;
        this._SmallCharListInstance.EventGetItemCount = this._GetCharElementCount;

        this._ToggleAlignment.EventCallBack = OnToggleAlignment;
    }
 
	public override void OnEnable()
	{

        InitComponent();
		base.OnEnable();
	}
    public override void InitComponent()
	{
        var value = UIValue.Instance.GetValue(UIValue.EParamType.CharSeletePopupType);
        if (value == null)
            return;
        _type = (int)value;

        object obj = UIValue.Instance.GetValue( UIValue.EParamType.SelectedRaidCharIndex );
        if( obj != null ) {
            mSelectedRaidCharIndex = (int)obj;
        }

        obj = UIValue.Instance.GetValue( UIValue.EParamType.SelectedRaidCuid );
        if( obj != null ) {
            mSelectedRaidCuid = (long)obj;
		}

        kSeleteBtn.SetActive(true);
        _SecretObj.SetActive(false);

        kHPFacilityBtn.gameObject.SetActive(false);
        kHPItemBtn.gameObject.SetActive(false);

		if( _type == (int)eCharSelectFlag.STAGE || _type == (int)eCharSelectFlag.RAID || _type == (int)eCharSelectFlag.RAID_PROLOGUE ) {
			kCharInfoBtn.SetActive( true );
		}
        else {
            kCharInfoBtn.SetActive( false );
        }

		kFacilitySelectDescLb.gameObject.SetActive(false);
        kCombatPowerObj.SetActive(false);
        kArenaSelectBtn.gameObject.SetActive(false);

		SprGradeIcon.gameObject.SetActive(false);
		LbUseCharGradeUpItem.gameObject.SetActive(false);

        kUndeployBtn.SetActive(false);

        _charlist.Clear();

        _initLarge = false;
        _initSmall = false;

        //���� ������ �ش� UI�� Sort �ִ�ġ�� �Ѿ����� Ȯ�� �� �����Ѵ�.
        CheckSortMax();

		switch ((eCharSelectFlag)_type)
        {
            case eCharSelectFlag.FACILITY:
				InitComponent_FACILITY();
				break;

            case eCharSelectFlag.ARENA:
				InitComponent_ARENA();
				break;

            case eCharSelectFlag.ARENATOWER:
            case eCharSelectFlag.ARENATOWER_STAGE:
                InitComponent_ARENATOWER();
				break;

			case eCharSelectFlag.USE_CHAR_GRADE_UP_ITEM:
				InitComponent_UseCharGradeUpItem();
				break;

            case eCharSelectFlag.SECRET_QUEST:
                InitComponent_SecretQuest();
                break;

            case eCharSelectFlag.RAID_PROLOGUE:
            case eCharSelectFlag.RAID:
                InitComponent_RaidPrologue();
                break;
            case eCharSelectFlag.FAVOR_BUFF_CHAR:
                InitComponent_FavorBuffChar();
                break;
            default:
				InitComponent_DEFAULT();
				break;
        }

        LateSortOrder();

		if ( mDicCombat == null ) {
			mDicCombat = new Dictionary<long, int>();
		}

		mDicCombat.Clear();
		mDicCombat.EnsureCapacity( _charlist.Count );
		for ( int i = 0; i < _charlist.Count; i++ ) {
			mDicCombat.Add( _charlist[i].CUID, GameSupport.GetCombatPower( _charlist[i], eWeaponSlot.MAIN ) );
		}

		if (GameSupport.IsTutorial())
        {
            _ToggleAlignment.SetToggle(1, SelectEvent.Code);
        }
        else
        {
            if (!PlayerPrefs.HasKey("CharSeletePopup_Alignment"))
            {
                _ToggleAlignment.SetToggle(1, SelectEvent.Code);
            }
            else
            {
                int select = PlayerPrefs.GetInt("CharSeletePopup_Alignment");
                _ToggleAlignment.SetToggle(select, SelectEvent.Code);
            }
        }
    }

    public override void OnClickClose() {
        base.OnClickClose();

        if (_type == (int)eCharSelectFlag.SECRET_QUEST) {
            ClearAllSelect();
        }
    }

    public void ClearAllSelect() {
        _selectCuid = 0;
        selectSubCharCuidList.Clear();
    }

    private void InitComponent_FACILITY()
    {
        int id = (int)UIValue.Instance.GetValue(UIValue.EParamType.FacilityID);
        _facilitydata = GameInfo.Instance.GetFacilityData(id);
        if (_facilitydata != null)
        {
            kFacilitySelectDescLb.gameObject.SetActive(true);
            for (int i = 0; i < GameInfo.Instance.CharList.Count; i++)
            {
                if (GameInfo.Instance.GetEquipCharFacilityData(GameInfo.Instance.CharList[i].CUID) == null)
                {
                    _charlist.Add(GameInfo.Instance.CharList[i]);
                }
            }
        }

        SetSortOrder();
    }

    private void InitComponent_ARENA()
    {
        kCharInfoBtn.gameObject.SetActive(false);
        kSeleteBtn.SetActive(false);

        GameInfo.Instance.GetCharList(ref _charlist);
        _seleteindex = 0;
        _arenaSelectSlotIdx = (int)UIValue.Instance.GetValue(UIValue.EParamType.ArenaTeamCharSlot);

        if (0 <= LobbyUIManager.Instance.JoinCharSeleteIndex && _charlist.Count > LobbyUIManager.Instance.JoinCharSeleteIndex)
            _seleteindex = LobbyUIManager.Instance.JoinCharSeleteIndex;

        if (GameInfo.Instance.TeamcharList[_arenaSelectSlotIdx] != 0)
        {
            for (int i = 0; i < _charlist.Count; i++)
            {
                if (_charlist[i].CUID == GameInfo.Instance.TeamcharList[_arenaSelectSlotIdx])
                {
                    _seleteindex = i;
                    break;
                }
            }
        }

        kCombatPowerObj.SetActive(true);
        kArenaSelectBtn.gameObject.SetActive(true);
        SetSeleteIndex(_seleteindex);
    }

    private void InitComponent_ARENATOWER()
    {
        kCharInfoBtn.gameObject.SetActive(false);
        kSeleteBtn.SetActive(false);

        GameInfo.Instance.GetArenaTowerCharList(ref _charlist);
        if (_charlist.Count > 0)
        {
            _charlist.RemoveAll(x => x.TableData == null);
        }

        _seleteindex = 0;
        _arenaSelectSlotIdx = (int)UIValue.Instance.GetValue(UIValue.EParamType.ArenaTowerTeamCharSlot);

        //if (0 <= LobbyUIManager.Instance.JoinCharSeleteIndex && _charlist.Count > LobbyUIManager.Instance.JoinCharSeleteIndex)
        //    _seleteindex = LobbyUIManager.Instance.JoinCharSeleteIndex;        
        List<long> _towerTeamCharList = GameSupport.GetArenaTowerTeamCharList();

        if (_towerTeamCharList[_arenaSelectSlotIdx] != 0)
        {
            for (int i = 0; i < _charlist.Count; i++)
            {   
                if (_charlist[i].CUID == _towerTeamCharList[_arenaSelectSlotIdx])
                {
                    _seleteindex = i;
                    break;
                }
            }
        }

        kCombatPowerObj.SetActive(true);
        kArenaSelectBtn.gameObject.SetActive(true);
        SetSeleteIndex(_seleteindex);
    }

    private void InitComponent_SecretQuest()
    {
        SetAllChar();

        lastSelectIndex = 0;

        //����� CUID ������ �ε����� �����Ѵ�.
        SetIndexFromCUID();

        int stageId = (int)UIValue.Instance.GetValue(UIValue.EParamType.StageID);
        isClearSecret = GameInfo.Instance.StageClearList.Exists(x => x.TableID == stageId);

        maxSelectCharacter = GameInfo.Instance.GameConfig.MultipleList.Count;

        kCombatPowerObj.SetActive(true);

        kSeleteBtn.SetActive(false);
        //����ӹ� ������Ʈ Ȱ��ȭ
        _SecretObj.SetActive(true);

        //���� ������Ʈ ����
        _SecretCharInfoBtn.isEnabled = false;

        _SecretOneCharObj.SetActive(!isClearSecret);
        _SecretOneCharStartBtn.isEnabled = false;

        _SecretMultiCharObj.SetActive(isClearSecret);
        _SecretMultiCharStartBtn.isEnabled = false;
        _SecretMultiCharQuickStartBtn.isEnabled = false;
        _SecretMultiCharQuickSubjugationBtn.isEnabled = false;

    }

    private void InitComponent_DEFAULT()
    {
        GameInfo.Instance.GetCharList(ref _charlist);
        _seleteindex = 0;

        if (0 <= LobbyUIManager.Instance.JoinCharSeleteIndex && _charlist.Count > LobbyUIManager.Instance.JoinCharSeleteIndex)
            _seleteindex = LobbyUIManager.Instance.JoinCharSeleteIndex;

        //ĳ���� ������ ǥ��
        kCombatPowerObj.SetActive(true);
        kCombatPowerLb.textlocalize = GameSupport.GetCombatPowerString(_charlist[_seleteindex], eWeaponSlot.MAIN);
    }

    private void InitComponent_RaidPrologue() {

        List<long> raidCharUidList = GameInfo.Instance.RaidUserData.CharUidList;
        foreach (long uid in raidCharUidList)
        {
            CharData charData = GameInfo.Instance.GetCharData(uid);
            if (charData == null)
            {
                continue;
            }

            if (mSelectedRaidCuid == uid)
            {
                _charlist.Insert(0, charData);
            }
            else
            {
                _charlist.Add(charData);
            }
        }

        List<CharData> charDataList = GameInfo.Instance.CharList;
        foreach (CharData charData in charDataList)
        {
            if (raidCharUidList.Exists(x => x == charData.CUID))
            {
                continue;
            }

            _charlist.Add(charData);
        }

        if( LobbyUIManager.Instance.JoinCharSeleteIndex >= 0 && LobbyUIManager.Instance.JoinCharSeleteIndex < _charlist.Count  ) {
            _seleteindex = LobbyUIManager.Instance.JoinCharSeleteIndex;
        }
        else {
            _seleteindex = -1;
            for( int i = 0; i < _charlist.Count; i++ ) {
                if( mSelectedRaidCuid == _charlist[i].CUID ) {
                    _seleteindex = i;
                    break;
                }
            }
        }

        //ĳ���� ������ ǥ��
        kCombatPowerObj.SetActive( true );
        kCombatPowerLb.textlocalize = GameSupport.GetCombatPowerString( _charlist[_seleteindex], eWeaponSlot.MAIN );
    }

	private void InitComponent_FavorBuffChar() {
		object cuidObj = UIValue.Instance.GetValue( UIValue.EParamType.SelectFavorBuffCuid );
		if ( cuidObj == null ) {
			return;
		}

		object buffCharIndexObj = UIValue.Instance.GetValue( UIValue.EParamType.SelectFavorBuffCharIndex );
		if ( buffCharIndexObj == null ) {
			return;
		}

		LbUseCharGradeUpItem.SetActive( true );

		mSelectFavorBuffCuid = (long)cuidObj;
		mSelectFavorBuffCharIndex = (int)buffCharIndexObj;

		List<CharData> charDataList = new List<CharData>();

		for ( int i = 0; i < GameInfo.Instance.CharList.Count; i++ ) {
			CharData charData = GameInfo.Instance.CharList[i];
			if ( charData == null ) {
				continue;
			}

			GameTable.LevelUp.Param levelUpParam =
				GameInfo.Instance.GameTable.FindLevelUp( x => x.Group == charData.TableData.PreferenceLevelGroup && x.Level == charData.FavorLevel );
			if ( levelUpParam == null ) {
				continue;
			}

			if ( 0 <= levelUpParam.Exp ) {
				continue;
			}

			charDataList.Add( charData );
		}

		foreach ( CharData charData in charDataList ) {
			if ( charData.CUID == mSelectFavorBuffCuid ) {
				_charlist.Insert( 0, charData );
			}
			else {
				_charlist.Add( charData );
			}
		}

		_seleteindex = 0;
		SetSeleteIndex( _seleteindex );
	}

	private void InitComponent_UseCharGradeUpItem()
	{
		SprGradeIcon.gameObject.SetActive(true);
		LbUseCharGradeUpItem.gameObject.SetActive(true);
		
		mPanel.depth = 100;

		for (int i = 0; i < mArrCharListPanel.Length; ++i)
		{
			if(mArrCharListPanel[i] == mPanel)
			{
				continue;
			}

			mArrCharListPanel[i].depth = 105;
		}

		int value = (int)UIValue.Instance.GetValue(UIValue.EParamType.GradeUpValue);
		LbUseCharGradeUpItem.textlocalize = string.Format(FLocalizeString.Instance.GetText(3271), value);

		_charlist.Clear();
		for (int i = 0; i < GameInfo.Instance.CharList.Count; i++)
		{
			if(GameInfo.Instance.CharList[i].Grade >= value)
			{
				continue;
			}

			_charlist.Add(GameInfo.Instance.CharList[i]);
		}
	}

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        switch (_sortType)
        {
            case eCharSortType.Get:
                FLocalizeString.SetLabel(kOrderLabel, 1250);
                break;
            case eCharSortType.Level:
                FLocalizeString.SetLabel(kOrderLabel, 1247);
                break;
            case eCharSortType.MainStat:
                FLocalizeString.SetLabel(kOrderLabel, 320);
                break;
            case eCharSortType.SecretQuestCount:
                FLocalizeString.SetLabel(kOrderLabel, 1793);
                break;
        }

        kOrderSpr.spriteName = _sortOrder == eCharSortOrder.SortOrder_Up ? m_sortIcon[0] : m_sortIcon[1];

        if (bChildren)
        {
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

                if (_type == (int) eCharSelectFlag.SECRET_QUEST)
                {
                    if (lastSelectIndex != -1)
                        _CharListInstance.SpringSetFocus(lastSelectIndex, 0.5f);
                }
            }
            else
            {
                if (!_initSmall)
                {
                    _SmallCharListInstance.UpdateList();
                    _initSmall = true;
                }
                else
                {
                    _SmallCharListInstance.RefreshNotMove();
                }
                
                if (_type == (int) eCharSelectFlag.SECRET_QUEST)
                {
                    if (lastSelectIndex != -1)
                        _SmallCharListInstance.SpringSetFocus(lastSelectIndex, 0.5f);
                }
            }
        }

        //�ε��� �ʱ�ȭ
        lastSelectIndex = -1;

        if (_type == (int)eCharSelectFlag.FACILITY)
        {

        }
        else if (_type == (int)eCharSelectFlag.ARENA)
        {
            //ĳ���� ������ ���
            kCombatPowerLb.textlocalize = GameSupport.GetCombatPowerString(_charlist[_seleteindex], eWeaponSlot.MAIN);

            long uid = _charlist[_seleteindex].CUID;
            long arenaSlotCharUID = GameInfo.Instance.TeamcharList[_arenaSelectSlotIdx];

            bool clearFlag = false;

            if (arenaSlotCharUID == uid)
                clearFlag = true;

            if (clearFlag)
            {
                kArenaSelectLb.textlocalize = FLocalizeString.Instance.GetText(1121);
                kArenaSelectBtn.SetButtonSpriteName("btn_Default_R_Red");
            }
            else
            {
                kArenaSelectLb.textlocalize = FLocalizeString.Instance.GetText(1095);
                kArenaSelectBtn.SetButtonSpriteName("btn_Default_R_Yellow");
            }
        }
        else if (_type == (int)eCharSelectFlag.ARENATOWER || _type == (int)eCharSelectFlag.ARENATOWER_STAGE)
        {
            //ĳ���� ������ ���
            bool IsFriend = GameSupport.IsFriend(_charlist[_seleteindex].CUID);
            TeamCharData choiceData = null;

            if (IsFriend)
            {
                var iterA = GameInfo.Instance.TowerFriendTeamData.GetEnumerator();
                while (iterA.MoveNext())
                {
                    var iterB = iterA.Current.charlist.GetEnumerator();
                    while (iterB.MoveNext())
                    {
                        if (iterB.Current.CharData.CUID == _charlist[_seleteindex].CUID)
                        {
                            choiceData = iterB.Current;
                            break;
                        }
                    }
                    if (choiceData != null)
                        break;
                }
                kCombatPowerLb.textlocalize = GameSupport.GetCombatPowerString(choiceData);
            }
            else
                kCombatPowerLb.textlocalize = GameSupport.GetCombatPowerString(_charlist[_seleteindex], eWeaponSlot.MAIN, eContentsPosKind.ARENA_TOWER);

            long uid = _charlist[_seleteindex].CUID;
            List<long> _towerTeamCharList = GameSupport.GetArenaTowerTeamCharList();
            long arenaSlotCharUID = _towerTeamCharList[_arenaSelectSlotIdx];

            bool clearFlag = false;

            if (arenaSlotCharUID == uid)
                clearFlag = true;

            if (clearFlag)
            {
                kArenaSelectLb.textlocalize = FLocalizeString.Instance.GetText(1121);
                kArenaSelectBtn.SetButtonSpriteName("btn_Default_R_Red");
            }
            else
            {
                kArenaSelectLb.textlocalize = FLocalizeString.Instance.GetText(1095);
                kArenaSelectBtn.SetButtonSpriteName("btn_Default_R_Yellow");
            }
        }
        else if (_type == (int) eCharSelectFlag.SECRET_QUEST)
        {
            //������ ���� üũ
            int mainCount = _seleteindex == -1 ? 0 : 1;
            currentSelectCharacterCount = mainCount + seleteSubIndexList.Count;

            //�Ҹ� AP��
            object stageIdObj = UIValue.Instance.GetValue(UIValue.EParamType.StageID);
            int.TryParse(stageIdObj.ToString(), out _stageId);

            int oneCharAp = 0;
            secretNeedAp = 0;
            int textId = (int)eTEXTID.RED_TEXT_COLOR;
            if (0 <= _stageId)
            {
                GameTable.Stage.Param stageParam = GameInfo.Instance.GameTable.Stages.Find(x => x.ID == _stageId);

                if (stageParam != null)
                {
                    oneCharAp = stageParam.Ticket;
                    
                    //ķ���� üũ
                    GuerrillaCampData guerrillaCampData = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_StageClear_APRateDown, stageParam.StageType);
                    if (guerrillaCampData != null)
                    {
                        oneCharAp -= (int)((float)oneCharAp * (float)((float)guerrillaCampData.EffectValue / (float)eCOUNT.MAX_BO_FUNC_VALUE));
                    }

                    //���� ĳ���� �� ��ŭ Ƽ�� ��뷮 ����
                    if (isClearSecret == true) {
                        secretNeedAp = oneCharAp * currentSelectCharacterCount;
                    }
                    else {
                        secretNeedAp = oneCharAp;
                    }

                    if (GameInfo.Instance.UserData.IsGoods(eGOODSTYPE.AP, secretNeedAp))
                    {
                        textId = guerrillaCampData == null ? (int) eTEXTID.WHITE_TEXT_COLOR : (int)eTEXTID.GREEN_TEXT_COLOR;
                    }
                }


                //���� ���� ���� ����?
                bool isQuick = stageParam.UseFastQuestTicket == 1 && GameInfo.Instance.GameConfig.FastQuestUnlockRank <= GameInfo.Instance.UserData.Level;
                
                _SecretMultiCharStartBtn.gameObject.SetActive(!isQuick);
                _SecretMultiCharQuickObj.SetActive(isQuick);
            }

            //������
            string powerSrt = "0";
            if (0 <= _seleteindex) {
                powerSrt = GameSupport.GetCombatPowerString(_charlist[_seleteindex], eWeaponSlot.MAIN);
            }
            kCombatPowerLb.textlocalize = powerSrt;

            _SecretCharInfoBtn.isEnabled = _seleteindex != -1;


            if (isClearSecret == false) {
                _SecretOneCharApLabel.textlocalize = FLocalizeString.Instance.GetText(textId, secretNeedAp);

                _SecretOneCharStartBtn.isEnabled = _seleteindex != -1;
            }
            else {
                //������ ĳ���Ͱ� ���� ��, �ּ��� 1���� AP�� ǥ�� �ϱ� ���� ���� ó��
                if (currentSelectCharacterCount == 0) 
                    _SecretMultiCharApLabel.textlocalize = FLocalizeString.Instance.GetText(textId, oneCharAp);
                else
                    _SecretMultiCharApLabel.textlocalize = FLocalizeString.Instance.GetText(textId, secretNeedAp);

                _SecretMultiCharStartBtn.isEnabled = _seleteindex != -1;
                _SecretMultiCharQuickStartBtn.isEnabled = _seleteindex != -1;
                _SecretMultiCharQuickSubjugationBtn.isEnabled = _seleteindex != -1;

                //���� �ӹ� Ƽ�� ��� ��Ȳ
                ItemData item = GameInfo.Instance.ItemList.Find(x => x.TableID.Equals(GameInfo.Instance.GameConfig.FastQuestTicketID));
                int itemCount = item?.Count ?? 0;
                
                //���� �����̼� Ƽ�� ����
                eTEXTID haveTicketColorID = currentSelectCharacterCount <= itemCount ? eTEXTID.WHITE_TEXT_COLOR : eTEXTID.RED_TEXT_COLOR;
                _SecretMultiCharAutoItemCountLabel.textlocalize = FLocalizeString.Instance.GetText((int)haveTicketColorID,
                    FLocalizeString.Instance.GetText(236, itemCount, currentSelectCharacterCount));

                //������ ���� �� ǥ��
                eTEXTID selectChrColorID = eTEXTID.RED_TEXT_COLOR;
                if (currentSelectCharacterCount > 0) {
                    selectChrColorID = currentSelectCharacterCount < maxSelectCharacter ? eTEXTID.WHITE_TEXT_COLOR : eTEXTID.GREEN_TEXT_COLOR;
                }

                string countText = FLocalizeString.Instance.GetText((int)selectChrColorID, currentSelectCharacterCount);
                _SecretMultiCharSelectCharCountLabel.textlocalize = FLocalizeString.Instance.GetText(236, countText, maxSelectCharacter);
            }
        }
        else
        {
			//ĳ���� ������ ���
			if ( _seleteindex < _charlist.Count ) {
				kCombatPowerLb.textlocalize = GameSupport.GetCombatPowerString( _charlist[_seleteindex], eWeaponSlot.MAIN );
			}
            else {
                kCombatPowerLb.textlocalize = string.Empty;
            }
		}

    }

    //�����߾��� ĳ���� id�� ����ĳ���� id�� �����Ѵ�. ����ӹ����� ĳ���� ���� �� �׸��� �����ϱ� ����.
    void SetSelectCharacterCuid() {
        if (_type != (int)eCharSelectFlag.SECRET_QUEST)
            return;

        if (0 <= _seleteindex && _seleteindex < _charlist.Count)
            _selectCuid = _charlist[_seleteindex].CUID;
        else
            _selectCuid = 0;

        selectSubCharCuidList.Clear();
        int charIndex = 0;
        for (int i = 0; i < seleteSubIndexList.Count; i++) {
            charIndex = seleteSubIndexList[i];
            if (charIndex < 0 || charIndex >= _charlist.Count) {
                Debug.LogError("Error : Secret Sub Character Index out of Range");
                return;
            }

            selectSubCharCuidList.Add(_charlist[charIndex].CUID);
        }

    }

    //cuid ����� �׸��� ������ index�� �ٽ� �����Ѵ�.
    void SetIndexFromCUID() {
        //����ӹ����� �÷��� �ȿ��� ���Ǳ� ������ �߰��� ����ó���� ����.
        _seleteindex = -1;
        seleteSubIndexList.Clear();
        //���� ������ ĳ���� ã�Ƽ� �ٽ� ����
        for (int i = 0; i < _charlist.Count; i++) {

            if (_charlist[i].CUID == _selectCuid) {
                _seleteindex = i;
            }
            else if (selectSubCharCuidList.Exists(r => r == _charlist[i].CUID) == true) {
                seleteSubIndexList.Add(i);
            }
        }
    }

    private void _UpdateCharListSlot(int index, GameObject slotObject)
	{
        do
        {
            UICharListSlot card = slotObject.GetComponent<UICharListSlot>();
            if (null == card) break;

            CharData data = null;
            if (0 <= index && _charlist.Count > index)
            {
                data = _charlist[index];
            }
            card.ParentGO = this.gameObject;
            if(_type == (int)eCharSelectFlag.ARENA)
                card.UpdateSlot(UICharListSlot.ePos.SeleteArenaPopup, index, data.TableData);
            else if (_type == (int)eCharSelectFlag.ARENATOWER || _type == (int)eCharSelectFlag.ARENATOWER_STAGE)
                card.UpdateSlot(UICharListSlot.ePos.SeleteArenaTowerPopup, index, data, _seleteindex);
            else if (_type == (int)eCharSelectFlag.SECRET_QUEST)
                card.UpdateSlot(UICharListSlot.ePos.SecretQuest, index, data.TableData);
            else if( _type == (int)eCharSelectFlag.RAID_PROLOGUE || _type == (int)eCharSelectFlag.RAID ) {
                card.UpdateSlot( UICharListSlot.ePos.SELECT_RAID_CHAR, index, data.TableData );
            }
			else if ( _type == (int)eCharSelectFlag.FAVOR_BUFF_CHAR ) {
				card.UpdateSlotFavorBuffChar( UICharListSlot.ePos.SeletePopup, index, data.TableData );
			}
			else
                card.UpdateSlot(UICharListSlot.ePos.SeletePopup, index, data.TableData);

        } while (false);
    }
	
	private int _GetCharElementCount()
	{
		return _charlist.Count; //TempValue
	}

	public void SetSeleteIndex( int index ) {

        //��� �ӹ��϶�
        if ( _type == (int)eCharSelectFlag.SECRET_QUEST) {
            //_lastSelectIndex - �����ε����� ���� ��ũ�Ѻ� �̵�, �ε����� ������ ���õ� �׸� ��Ŀ�� ����

            if (isClearSecret == true) {
                //������ �ƹ��͵� �����Ѱ� ��������
                if (_seleteindex == -1) {
                    //���� ������ �ε����� ���� �ε������ٸ� ����
                    if (seleteSubIndexList.Exists(r => r == index) == true) {
                        seleteSubIndexList.Remove(index);
                    }

                    _seleteindex = index;
                    lastSelectIndex = -1;
                }
                //���ΰ� ������ ������ ��
                else if (_seleteindex == index) {
                    //������ ���� ĳ���Ͱ� ������ �ִ���?
                    if (seleteSubIndexList.Count > 0) {
                        //0�� ���� ĳ���͸� �������� ���� �� �ش�.
                        _seleteindex = seleteSubIndexList[0];
                        seleteSubIndexList.RemoveAt(0);
                    }
                    else {
                        _seleteindex = -1;
                    }

                    lastSelectIndex = -1;
                }
                //���ΰ� �ٸ��� ������.
                else {

                    //������ ���꿡 ��ϵ� �׸��� ����
                    if (seleteSubIndexList.Exists(r => r == index) == true) {
                        seleteSubIndexList.Remove(index);
                        lastSelectIndex = -1;
                    }
                    //������ ���꿡 ����, �ִ� ��� ���� ���� �϶� (������ ���� �����ϹǷ� -1)
                    else if (seleteSubIndexList.Count < maxSelectCharacter - 1) {
                        seleteSubIndexList.Add(index);
                        lastSelectIndex = -1;
                    }
                    //�ִ� ��� ���� �϶�
                    else {
                        //�ƹ��͵� ���� �ʰ� ������ �Ѵ�.(�Է� ����)
                        return;
                    }
                }
            }
            else {
                if (_seleteindex == index)
                    _seleteindex = -1;
                else
                    _seleteindex = index;
            }
            
        }
        else {
            _seleteindex = index;

            if (_type == (int)eCharSelectFlag.FAVOR_BUFF_CHAR) {
                if (_seleteindex < _charlist.Count) {
                    kUndeployBtn.SetActive(_charlist[_seleteindex].CUID == mSelectFavorBuffCuid);

                    GameTable.Buff.Param buffParam = GameInfo.Instance.GameTable.FindBuff(_charlist[_seleteindex].TableData.PreferenceBuff);
                    if (buffParam != null) {
                        LbUseCharGradeUpItem.textlocalize = FLocalizeString.Instance.GetText(buffParam.Name);
                    }
                    else {
                        LbUseCharGradeUpItem.textlocalize = string.Empty;
                    }
                }
            }
        }

        Renewal(true);
    }

    public void SetAllChar()
    {
        _charlist.Clear();
        _charlist.AddRange(GameInfo.Instance.CharList);
        SetSortOrder();
    }

    public void CheckSortMax() {
        //LHK - ���� ���͸� ������ ���������� �ʰ� �Ǹ�, �����ؾߵȴ�.
        //���� �����϶� ������ ������ ������ �ʾƾ� �Ѵ�.
        if (_type == (int)eCharSelectFlag.FAVOR_BUFF_CHAR) {
            if ((int)_sortType >= (int)eCharSortType.MainStat)
                _sortType = eCharSortType.Get;
        }
        //��� �ӹ� �϶� �� ���´�.
        else if (_type == (int)eCharSelectFlag.SECRET_QUEST) {
            if ((int)_sortType >= (int)eCharSortType._MAX_)
                _sortType = eCharSortType.Get;
        }
        //�Ϲ� ������ Get, Level, MainStat ������ ���´�.
        else {
            if ((int)_sortType >= (int)eCharSortType.SecretQuestCount)
                _sortType = eCharSortType.Get;
        }
    }
    
	public void OnClick_HPFacilityBtn()
	{
	
	}
	
	public void OnClick_HPItemBtn()
	{
	
	}

	
	public void OnClick_CloseBtn()
	{
		mPanel.depth = mOriginalDepth;

		for (int i = 0; i < mArrCharListPanel.Length; ++i)
		{
			if (mArrCharListPanel[i] == mPanel)
			{
				continue;
			}

			mArrCharListPanel[i].depth = mOriginalDepth + 5;
		}

		OnClickClose();
    }

    public void OnClick_SeleteBtn() {
        if( 0 > _seleteindex || _charlist.Count <= _seleteindex )
            return;

        long uid = _charlist[_seleteindex].CUID;

        if( _type == (int)eCharSelectFlag.FACILITY ) {
            // �ִ� ���� ���� �ü� �̿� ��ü�� �����ϵ��� ó��
            //if (_facilitydata.TableData.EffectType == "FAC_CHAR_EXP")
            //{
            //    if (_charlist[_seleteindex].Level >= GameInfo.Instance.GameConfig.CharMaxLevel[_charlist[_seleteindex].Grade])
            //    {
            //        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3007));

            //        return;
            //    }
            //}
            //else if (_facilitydata.TableData.EffectType == "FAC_CHAR_SP")
            //{
            //    if (_charlist[_seleteindex].PassviePoint >= GameInfo.Instance.GameConfig.LimitMaxSkillPT)
            //    {
            //        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3041));
            //        return;
            //    }
            //}
            if( _facilitydata.TableData.EffectType == "FAC_CHAR_EXP" ) {
                if( GameSupport.IsMaxCharLevel( _charlist[_seleteindex].Level, _charlist[_seleteindex].Grade ) ) {
                    MessagePopup.CYN( eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText( 1699 ), eTEXTID.YES, eTEXTID.NO,
                    () => {
                        UIFacilityPanel panel = LobbyUIManager.Instance.GetActiveUI<UIFacilityPanel>("FacilityPanel");
                        if( panel != null )
                            panel.SendFacilityUse( uid );
                    }, null );
                }
                else {
                    UIFacilityPanel panel = LobbyUIManager.Instance.GetActiveUI<UIFacilityPanel>("FacilityPanel");
                    if( panel != null )
                        panel.SendFacilityUse( uid );
                }
            }
            else {
                UIFacilityPanel panel = LobbyUIManager.Instance.GetActiveUI<UIFacilityPanel>("FacilityPanel");
                if( panel != null )
                    panel.SendFacilityUse( uid );
            }

        }
        else if( _type == (int)eCharSelectFlag.STAGE ) {
            UIValue.Instance.SetValue( UIValue.EParamType.StageCharCUID, uid );

            OnClickClose();

            CharData chardata = GameInfo.Instance.GetCharData(uid);
            if( chardata != null )
                VoiceMgr.Instance.PlayChar( eVOICECHAR.PlayStageSel, chardata.TableID );

            LobbyUIManager.Instance.InitComponent( "StageDetailPopup" );
            LobbyUIManager.Instance.Renewal( "StageDetailPopup" );
        }
        else if( _type == (int)eCharSelectFlag.USER_INFO ) {
            var popup = LobbyUIManager.Instance.GetActiveUI<UIUserInfoPopup>("UserInfoPopup");
            if( popup != null ) {
                if( GameInfo.Instance.UserData.MainCharUID != uid ) {
                    popup.SeleteMainChar( uid );
                }

                OnClickClose();
            }
        }
        else if( _type == (int)eCharSelectFlag.ARENA ) {
            //int teamSlot = (int)UIValue.Instance.GetValue(UIValue.EParamType.ArenaTeamCharSlot);

            int cardFormationId = 0;
            cardFormationId = GameInfo.Instance.UserData.ArenaCardFormationID;

            GameInfo.Instance.Send_ReqSetArenaTeam( GameSupport.ArenaCharChange( uid, _arenaSelectSlotIdx ), (uint)cardFormationId, OnNet_AckSetArenaTeam );
        }
        else if( _type == (int)eCharSelectFlag.USE_CHAR_GRADE_UP_ITEM ) {
            UIItemInfoPopup itemInfoPopup = LobbyUIManager.Instance.GetActiveUI<UIItemInfoPopup>("ItemInfoPopup");
            if( itemInfoPopup ) {
                CharData chardata = GameInfo.Instance.GetCharData(uid);
                itemInfoPopup.SetGradeUpCharData( chardata );

                OnClickClose();
            }
        }
        else if( _type == (int)eCharSelectFlag.Preset ) {
            UIPresetPopup presetPopup = LobbyUIManager.Instance.GetActiveUI<UIPresetPopup>("PresetPopup");
            if( presetPopup ) {
                presetPopup.SetCharData( uid );
                presetPopup.Renewal( true );
            }

            UICharInfoPanel charInfoPanel = LobbyUIManager.Instance.GetActiveUI<UICharInfoPanel>("CharInfoPanel");
            if( charInfoPanel ) {
                UIValue.Instance.SetValue( UIValue.EParamType.CharSelUID, uid );
                CharData chardata = GameInfo.Instance.GetCharData(uid);
                if( chardata != null ) {
                    UIValue.Instance.SetValue( UIValue.EParamType.CharSelTableID, chardata.TableID );
                }

                charInfoPanel.InitComponent();
                charInfoPanel.Renewal( true );
            }

            UIStageDetailPopup stageDetailPopup = LobbyUIManager.Instance.GetActiveUI<UIStageDetailPopup>("StageDetailPopup");
            if( stageDetailPopup ) {
                UIValue.Instance.SetValue( UIValue.EParamType.StageCharCUID, uid );

                stageDetailPopup.InitComponent();
                stageDetailPopup.Renewal( true );
            }

            OnClickClose();
        }
        else if( _type == (int)eCharSelectFlag.RAID_PROLOGUE ) {
            OnClickClose();

            CharData chardata = GameInfo.Instance.GetCharData( uid );
            if( chardata != null ) {
                VoiceMgr.Instance.PlayChar( eVOICECHAR.PlayStageSel, chardata.TableID );
            }

            string str = PlayerPrefs.GetString( "RaidPlayer1_CUID" );
            long cuid1 = Utility.SafeLongParse( str );

            str = PlayerPrefs.GetString( "RaidPlayer2_CUID" );
            long cuid2 = Utility.SafeLongParse( str );

            str = PlayerPrefs.GetString( "RaidPlayer3_CUID" );
            long cuid3 = Utility.SafeLongParse( str );

            if( mSelectedRaidCharIndex == 0 ) {
                UIValue.Instance.SetValue( UIValue.EParamType.StageCharCUID, uid );

                if( uid == cuid2 ) {
                    PlayerPrefs.SetString( "RaidPlayer2_CUID", cuid1.ToString() );
                }
                else if( uid == cuid3 ) {
                    PlayerPrefs.SetString( "RaidPlayer3_CUID", cuid1.ToString() );
                }

                PlayerPrefs.SetString( "RaidPlayer1_CUID", uid.ToString() );
            }
            else if( mSelectedRaidCharIndex == 1 ) {
                if( uid == cuid1 ) {
                    PlayerPrefs.SetString( "RaidPlayer1_CUID", cuid2.ToString() );
                }
                else if( uid == cuid3 ) {
                    PlayerPrefs.SetString( "RaidPlayer3_CUID", cuid2.ToString() );
                }

                PlayerPrefs.SetString( "RaidPlayer2_CUID", uid.ToString() );
            }
            else if( mSelectedRaidCharIndex == 2 ) {
                if( uid == cuid1 ) {
                    PlayerPrefs.SetString( "RaidPlayer1_CUID", cuid3.ToString() );
                }
                else if( uid == cuid2 ) {
                    PlayerPrefs.SetString( "RaidPlayer2_CUID", cuid3.ToString() );
                }

                PlayerPrefs.SetString( "RaidPlayer3_CUID", uid.ToString() );
            }

            LobbyUIManager.Instance.InitComponent( "StageDetailPopup" );
            LobbyUIManager.Instance.Renewal( "StageDetailPopup" );
        }
        else if( _type == (int)eCharSelectFlag.RAID ) {
            OnClickClose();

            UIRaidDetailPopup raidDetailPopup = LobbyUIManager.Instance.GetActiveUI<UIRaidDetailPopup>("RaidDetailPopup");
            if( raidDetailPopup ) {
                raidDetailPopup.ChangeRaidChar( mSelectedRaidCharIndex, uid );
			}
        }
		else if ( _type == (int)eCharSelectFlag.FAVOR_BUFF_CHAR ) {
			OnClickClose();

			if ( GameInfo.Instance.UserData.ArrFavorBuffCharUid[mSelectFavorBuffCharIndex] == uid ) {
				return;
			}

			GameInfo.Instance.Send_ReqChangePreferenceNum( uid, mSelectFavorBuffCharIndex, OnNet_FavorBuffChar );
		}
	}

    public void OnClick_ArenaCharSelectBtn()
    {
        if (_type == (int)eCharSelectFlag.ARENA)
        {

            if (0 > _seleteindex || _charlist.Count <= _seleteindex)
                return;

            long uid = _charlist[_seleteindex].CUID;

            //int teamSlot = (int)UIValue.Instance.GetValue(UIValue.EParamType.ArenaTeamCharSlot);

            int cardFormationId = 0;
            cardFormationId = GameInfo.Instance.UserData.ArenaCardFormationID;

            GameInfo.Instance.Send_ReqSetArenaTeam(GameSupport.ArenaCharChange(uid, _arenaSelectSlotIdx), (uint)cardFormationId, OnNet_AckSetArenaTeam);
        }
        else if (_type == (int)eCharSelectFlag.ARENATOWER || _type == (int)eCharSelectFlag.ARENATOWER_STAGE)
        {
            if (0 > _seleteindex || _charlist.Count <= _seleteindex)
                return;

            long selectCUID = _charlist[_seleteindex].CUID;
            List<long> _newTeamCharList = new List<long>();
            if (GameSupport.IsFriend(selectCUID))
            {
                int CurSlotIdx = GameInfo.Instance.ArenaTowerFriendContainer.GetKeyByValue(selectCUID);

                // �����Ϸ��� ���Կ� �̹� ģ�� ĳ���Ͱ� ���� ���
                if (CurSlotIdx < 0 && GameInfo.Instance.ArenaTowerFriendContainer.TryGetValue(_arenaSelectSlotIdx, out long uid) && GameSupport.IsFriend(uid))
                {
                    GameInfo.Instance.ArenaTowerFriendContainer.Remove(_arenaSelectSlotIdx);
                }

                if (CurSlotIdx >= 0)
                {
                    //���Կ� �Ҵ�Ǿ� �ִ� ģ�� ĳ����
                    if (CurSlotIdx == _arenaSelectSlotIdx)
                    {   
                        //������ ��ġ�� ������ ���Կ��� ����
                        GameInfo.Instance.ArenaTowerFriendContainer.Remove(_arenaSelectSlotIdx);
                        selectCUID = 0;
                    }
                    else
                    {
                        //������ ��ġ�� �ٸ��� ����
                        GameInfo.Instance.ArenaTowerFriendContainer.SwapReplace(_arenaSelectSlotIdx, selectCUID);

                        selectCUID = 0;

                        //���� �÷��̾� �� ��ġ ����
                        if (GameInfo.Instance.TowercharList != null && GameInfo.Instance.TowercharList.Count > _arenaSelectSlotIdx)
                        {
                            selectCUID = GameInfo.Instance.TowercharList[_arenaSelectSlotIdx];
                            _arenaSelectSlotIdx = CurSlotIdx;
                        }
                    }
                }
                else
                {
                    if(GameInfo.Instance.ArenaTowerFriendContainer.ValidCount() >= 1)
                    {
                        //ģ���� 1�� ���Կ� ������ �� ����                        
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(1626));
                        return;
                    }

                    //���Կ� ó���Ҵ� �ϴ� ģ�� ĳ����
                    GameInfo.Instance.ArenaTowerFriendContainer.Assign(_arenaSelectSlotIdx, selectCUID);

                    selectCUID = 0;
                }

                //�޸𸮿� ����� �� �÷��̾�� ĳ���� ���� ���� ���� üũ
                List<long> _oldTeamCharList = new List<long>();
                for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
                    _oldTeamCharList.Add(GameInfo.Instance.TowercharList[i]);

                
                _newTeamCharList = GameSupport.ArenaTowerCharChange(selectCUID, _arenaSelectSlotIdx);

                bool IsEqual = true;
                for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
                {
                    if (i >= _newTeamCharList.Count)
                        continue;

                    if (_oldTeamCharList[i] != _newTeamCharList[i])
                    {
                        IsEqual = false;
                        break;
                    }
                }

                if (IsEqual)
                {
                    OnNet_AckSetArenaTowerTeam(0, null);
                    return;
                }
            }
            else
            {   
                long curFriendCUID = 0;
                GameInfo.Instance.ArenaTowerFriendContainer.TryGetValue(_arenaSelectSlotIdx, out curFriendCUID);
                if (curFriendCUID > 0)
                {
                    //ģ�� ĳ������ ���� ��ġ�� �ٲ���� �Ѵ�.
                    int tmpidx = -1;
                    for (int i = 0; i < GameInfo.Instance.TowercharList.Count; i++)
                    {
                        if (GameInfo.Instance.TowercharList[i] == selectCUID)
                        {
                            tmpidx = i;
                            break;
                        }
                    }
                    if (tmpidx >= 0)
                        GameInfo.Instance.ArenaTowerFriendContainer.SwapReplace(tmpidx, curFriendCUID);
                    else
                        GameInfo.Instance.ArenaTowerFriendContainer.Remove(_arenaSelectSlotIdx);
                }
                else                
                {
                    //���õ� ���Կ� ģ���� ������ ����
                    GameInfo.Instance.ArenaTowerFriendContainer.Remove(_arenaSelectSlotIdx);
                }
                _newTeamCharList = GameSupport.ArenaTowerCharChange(selectCUID, _arenaSelectSlotIdx);
            }
            GameInfo.Instance.Send_ReqSetArenaTowerTeam(_newTeamCharList, (uint)GameInfo.Instance.UserData.ArenaTowerCardFormationID, OnNet_AckSetArenaTowerTeam);
        }
    }

    public void OnClick_CharInfoBtn()
    {
        if (0 > _seleteindex || _charlist.Count <= _seleteindex)
            return;

        long uid = _charlist[_seleteindex].CUID;
        int tableid = _charlist[_seleteindex].TableID;

        
        LobbyUIManager.Instance.JoinStageID = (int)UIValue.Instance.GetValue(UIValue.EParamType.StageID);
        LobbyUIManager.Instance.JoinCharSeleteIndex = _seleteindex;

        UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, uid);
        UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, tableid);
        
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);

        OnClickClose();

        //â�� �ݱ� ���� index ���ÿ� ����Ͽ� ������ ���� ĳ���� CUID ���� �������ش�.
        SetSelectCharacterCuid();

        GameTable.Stage.Param find = GameInfo.Instance.GameTable.FindStage( LobbyUIManager.Instance.JoinStageID );
        if( find != null && find.StageType == (int)eSTAGETYPE.STAGE_RAID ) {
            LobbyUIManager.Instance.HideUI( "RaidDetailPopup" );
        }
        else {
            LobbyUIManager.Instance.HideUI( "StageDetailPopup" );
        }
        
    }

	public void OnClick_UndeployBtn() {
		if ( _type == (int)eCharSelectFlag.FAVOR_BUFF_CHAR ) {
			OnClickClose();

			if ( _charlist.Count <= _seleteindex ) {
				return;
			}

			if ( _charlist[_seleteindex] == null ) {
				return;
			}

			GameInfo.Instance.Send_ReqChangePreferenceNum( 0, mSelectFavorBuffCharIndex, OnNet_FavorBuffChar );
		}
	}

	public void OnNet_FavorBuffChar(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        LobbyUIManager.Instance.Renewal("ArmoryPopup");
        LobbyUIManager.Instance.Renewal("TopPanel");
    }

    public void OnNet_AckSetArenaTeam(int result, PktMsgType pktMsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("PresetPopup");
        LobbyUIManager.Instance.Renewal("ArenaMainPanel");
        LobbyUIManager.Instance.Renewal("ArenaBattleConfirmPopup");

        OnClickClose();
    }

    public void OnNet_AckSetArenaTowerTeam(int result, PktMsgType pktMsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("ArenaTowerMainPanel");
        LobbyUIManager.Instance.Renewal("ArenaTowerStagePanel");
        OnClickClose();
    }

    private bool OnToggleAlignment(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
        {
            return false;
        }

        if (nSelect == 0) // ����������
        {
            _CharListInstance.gameObject.SetActive(false);

            _SmallCharListInstance.gameObject.SetActive(true);
            if (!_initSmall)
            {
                _SmallCharListInstance.UpdateList();
                _initSmall = true;
            }
            else
            {
                _SmallCharListInstance.RefreshNotMove();
            }

            if (_type == (int) eCharSelectFlag.SECRET_QUEST)
            {
                _SmallCharListInstance.SpringSetFocus(_seleteindex == -1 ? 0 : _seleteindex, 0.5f, true);
            }

            PlayerPrefs.SetInt("CharSeletePopup_Alignment", 0);
        }
        else // ��������
        {
            _SmallCharListInstance.gameObject.SetActive(false);

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

            if (_type == (int) eCharSelectFlag.SECRET_QUEST)
            {
                _CharListInstance.SpringSetFocus(_seleteindex == -1 ? 0 : _seleteindex, 0.5f, true);
            }

            PlayerPrefs.SetInt("CharSeletePopup_Alignment", 1);
        }

        return true;
    }

    public void OnClick_StartBtn()
    {
        if (_seleteindex < 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3006));
            return;
        }
        
        object stageIdObj = UIValue.Instance.GetValue(UIValue.EParamType.StageID);
        int.TryParse(stageIdObj.ToString(), out _stageId);
        if (_stageId < 0)
        {
            return;
        }

        // Game Table Check
        GameTable.Stage.Param stageParam = GameInfo.Instance.GameTable.FindStage(_stageId);
        if (stageParam == null)
        {
            return;
        }

        // AP Check
        int ticket = stageParam.Ticket;

        //����ӹ� �϶� �̸� ����ص� ���� ����Ѵ�. -���� �ڵ� ó�� �Ź� ����ϴ°� �� �������� ��.
        if (_type == (int)eCharSelectFlag.SECRET_QUEST) {
            ticket = secretNeedAp;
        }
        else {
            //ķ���� üũ
            GuerrillaCampData campdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_StageClear_APRateDown, stageParam.StageType);
            if (campdata != null) {
                ticket -= (int)((float)ticket * (float)((float)campdata.EffectValue / (float)eCOUNT.MAX_BO_FUNC_VALUE));
            }
        }

        if (!GameSupport.IsCheckTicketAP(ticket))
        {
            return;
        }
        
        // Inven Check
        if (!GameSupport.IsCheckInven())
        {
            return;
        }
        
        // Select Char Check
        CharData charData = _charlist[_seleteindex];
        if (charData == null)
        {
            return;
        }

        if (GameSupport.IsEmptyInEquipMainWeapon(ePresetKind.STAGE, charData.CUID))
        {
            return;
        }

        if (GameSupport.GetCharLastSkillSlotCheck(charData))
        {
            charData.EquipSkill[(int)eCOUNT.SKILLSLOT - 1] = (int)eCOUNT.NONE;
            GameInfo.Instance.Send_ReqApplySkillInChar(charData.CUID, charData.EquipSkill, OnSkillOutGameStart);
            return;
        }

        //��� �ӹ����� 2�� �̻� ���� �ҷ��� �� ��, ��� ������ ����ش�
        if (_type == (int)eCharSelectFlag.SECRET_QUEST && currentSelectCharacterCount > 1) {
            string textMessage = string.Format(FLocalizeString.Instance.GetText(1883), FLocalizeString.Instance.GetText(1884));
            MessagePopup.YN(eTEXTID.TITLE_NOTICE, textMessage, Send_StageStart);
        }
        else {
            Send_StageStart();
        }
    }


    public void OnSkillOutGameStart(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }
        
        Log.Show("Skill Out!!!", Log.ColorType.Red);
        Send_StageStart();
    }

    void Send_StageStart() {

        //����ӹ� �϶�, �߰� ������ ĳ���� ����Ʈ ����
        SetSelectCharacterCuid();

        //�Ϲ� ����
        if (selectSubCharCuidList.Count == 0) {
            GameInfo.Instance.Send_ReqStageStart(_stageId, _selectCuid, 0, false, false, null, OnNetGameStart);
        }
        //��� ����
        else {
            int multipleIndex = currentSelectCharacterCount - 1;
            GameInfo.Instance.Send_ReqStageStart(_stageId, _selectCuid, multipleIndex, false, false, selectSubCharCuidList, OnNetGameStart);
        }
    }

    public void OnNetGameStart(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        SecretQuestOptionData optionData = GameInfo.Instance.ServerData.SecretQuestOptionList.Find(x => x.GroupId == _stageId);
        if (optionData == null)
        {
            return;
        }

        GameTable.SecretQuestLevel.Param sqLevelParam = 
            GameInfo.Instance.GameTable.SecretQuestLevels.Find(x => x.GroupID == _stageId && x.No == optionData.LevelId);
        if (sqLevelParam == null)
        {
            return;
        }

		GameInfo.Instance.SelecteStageTableId = _stageId;
		GameInfo.Instance.SelecteSecretQuestLevelId = sqLevelParam.No;
		GameInfo.Instance.SelecteSecretQuestBOSetId = optionData.BoSetId;

		UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.LobbyToStage);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, _stageId);
        LobbyUIManager.Instance.ShowUI("LoadingPopup", false);
        AppMgr.Instance.LoadScene(AppMgr.eSceneType.Stage, sqLevelParam.Scene);
    }

    //��� �ӹ� ���� ��ư
    public void OnClick_SubjugationBtn() {
        
        //AP üũ
        if (!GameSupport.IsCheckTicketAP(secretNeedAp) || !GameSupport.IsCheckInven()) {
            return;
        }

        int multipleIndex = currentSelectCharacterCount - 1;

        ItemData item = GameInfo.Instance.ItemList.Find(x => x.TableID == GameInfo.Instance.GameConfig.FastQuestTicketID);
        int count = item?.Count ?? 0;
        int maxMultiple = GameInfo.Instance.GameConfig.MultipleList[multipleIndex];

        if (count <= (int)eCOUNT.NONE || count < maxMultiple) {
            ItemBuyMessagePopup.ShowItemBuyPopup(GameInfo.Instance.GameConfig.FastQuestTicketID, count, maxMultiple);
            return;
        }

        // Select Char Check
        CharData charData = _charlist[_seleteindex];
        if (charData == null) {
            return;
        }

        if (GameSupport.IsEmptyInEquipMainWeapon(ePresetKind.STAGE, charData.CUID)) {
            return;
        }

        UISubjugationConfirmPopup popup = LobbyUIManager.Instance.GetUI("SubjugationConfirmPopup") as UISubjugationConfirmPopup;
        if (popup != null) {
            UIValue.Instance.SetValue(UIValue.EParamType.StageID, _stageId);

            SetSelectCharacterCuid();

            popup.SetValue(charData, _stageId, multipleIndex, _selectCuid, _SecretMultiCharApLabel.text, selectSubCharCuidList);
            popup.SetUIActive(true);
        }
        
    }

    public void OnClick_SortBtn()
    {
        _sortType++;

        CheckSortMax();

        SetSelectCharacterCuid();

        SetSortOrder(true);
    }

    public void OnClick_OrdrtBtn()
    {
        _sortOrder = _sortOrder == eCharSortOrder.SortOrder_Up ? eCharSortOrder.SortOrder_Down : eCharSortOrder.SortOrder_Up;
        
        SetSelectCharacterCuid();

        SetSortOrder(true);
    }

    private void SetSortOrder(bool renewal = false) {

        switch (_sortType)
        {
            case eCharSortType.Get:
                _charlist.Sort(SortGet_Char);
                break;
            case eCharSortType.Level:
                _charlist.Sort(SortLevel_Char);
                break;
            case eCharSortType.MainStat:
                _charlist.Sort(SortMainStat_Char);
                break;
            case eCharSortType.SecretQuestCount:
                _charlist.Sort(SortSecretQuestCount_Char);
                break;
        }

        if (renewal)
        {
            _seleteindex = (int)eCOUNT.NONE;
            if (_type == (int)eCharSelectFlag.SECRET_QUEST)
            {
                lastSelectIndex = 0;
                //������ ���� �ߴ� ĳ���� CUID �������� ���� �Ŀ��� ���� �ε����� ����
                SetIndexFromCUID();
            }
            Renewal(true);
        }
    }

    private int SortGet_Char(CharData data1, CharData data2)
    {
        CharData char1 = data1;
        CharData char2 = data2;

        if (_sortOrder == eCharSortOrder.SortOrder_Down)
        {
            char1 = data2;
            char2 = data1;
        }

        return char1.CUID.CompareTo(char2.CUID);
    }

    private int SortLevel_Char(CharData data1, CharData data2)
    {
        CharData char1 = data1;
        CharData char2 = data2;

        if (_sortOrder == eCharSortOrder.SortOrder_Down)
        {
            char1 = data2;
            char2 = data1;
        }

		int result = char1.Level.CompareTo( char2.Level );
		if ( result == 0 ) {
			result = char1.TableID.CompareTo( char2.TableID );
        }

        return result;
    }

    private int SortMainStat_Char(CharData data1, CharData data2)
    {
        CharData char1 = data1;
        CharData char2 = data2;

        if (_sortOrder == eCharSortOrder.SortOrder_Down)
        {
            char1 = data2;
            char2 = data1;
        }

		mDicCombat.TryGetValue( char1.CUID, out int char1CombatPower );
		mDicCombat.TryGetValue( char2.CUID, out int char2CombatPower );

		return char1CombatPower.CompareTo( char2CombatPower );
	}

    private int SortSecretQuestCount_Char(CharData data1, CharData data2) {
        CharData char1 = data1;
        CharData char2 = data2;

        if (_sortOrder == eCharSortOrder.SortOrder_Down) {
            char1 = data2;
            char2 = data1;
        }

        int result = char1.SecretQuestCount.CompareTo(char2.SecretQuestCount);

        if (result == 0) {
            result = SortLevel_Char(data1, data2);
            if (result == 0) {
                result = SortMainStat_Char(data1, data2);
                if (result == 0) {
                    result = SortGet_Char(data1, data2);
                }
            }
        }

        return result;
    }
    
    private void LateSortOrder() {
		switch ( (eCharSelectFlag)_type ) {
			case eCharSelectFlag.ARENA:
			case eCharSelectFlag.ARENATOWER:
			case eCharSelectFlag.ARENATOWER_STAGE: {
				
			}
			break;

			default: {
				SetSortOrder();
			}
			break;
		}
	}
}
