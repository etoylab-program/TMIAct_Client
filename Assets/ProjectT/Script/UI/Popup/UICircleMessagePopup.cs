using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleMessagePopup : FComponent
{
    [Header("UICircleMessagePopup")]
    [SerializeField] private List<GameObject> sequenceList = null;
    [SerializeField] private GameObject chatObj = null;
    [SerializeField] private GameObject managementObj = null;
    [SerializeField] private GameObject memberObj = null;
    [SerializeField] private UILabel titleLabel = null;
    [SerializeField] private UILabel yesLabel = null;
    [SerializeField] private UIButton yesBtn = null;
    [SerializeField] private UIButton noBtn = null;
    [SerializeField] private UIButton closeBtn = null;
    [SerializeField] private UIButton confirmBtn = null;
    [SerializeField] private UIGoodsUnit infoGoodsUnit = null;
    [SerializeField] private UILabel managementNoticeLabel = null;
    [SerializeField] private UIInput nameInput = null;
    [SerializeField] private UIInput contentInput = null;
    [SerializeField] private UILabel mainLangLabel = null;
    [SerializeField] private FToggle otherLangFToggle = null;
    [SerializeField] private UITexture stampBuyTex = null;
    [SerializeField] private UIGoodsUnit stampBuyGoodsUnit = null;
    [SerializeField] private UICircleMemberListSlot circleMemberListSlot = null;
    [SerializeField] private List<FToggle> alramFToggleList = null;

    private eLANGUAGE _currentLang;
    private eCircleSequenceType _currentSequenceType;
    private System.Text.StringBuilder _stringBuilder;
    private FriendUserData _circleUserData;
    private int _stampTableId;

    public override void Awake()
    {
        base.Awake();

        _stringBuilder = new System.Text.StringBuilder();

        for (int i = 0; i < alramFToggleList.Count; i++)
        {
            alramFToggleList[i].EventCallBackToggle = OnEventAlramToggleSelectComponent;
        }
    }

    public override void InitComponent()
    {
        base.InitComponent();

        _currentLang = GameInfo.Instance.CircleData.MainLanguage;
        otherLangFToggle.SetToggle(GameInfo.Instance.CircleData.GetIntOtherLang(), SelectEvent.Code);

        int on = (int)eToggleType.On;
        int off = (int)eToggleType.Off;
        for (int i = 0; i < alramFToggleList.Count; i++)
        {
            if (i < 0 || (int)eCircleChatAlramType.WeaponEnhance < i)
            {
                continue;
            }

            bool isOn = true;
            switch ((eCircleChatAlramType)i)
            {
                case eCircleChatAlramType.All:           { isOn = FSaveData.Instance.CircleChatAlramAll; } break;
                case eCircleChatAlramType.Arena:         { isOn = FSaveData.Instance.CircleChatAlramArena; } break;
                case eCircleChatAlramType.Gacha:         { isOn = FSaveData.Instance.CircleChatAlramGacha; } break;
                case eCircleChatAlramType.TimeAttack:    { isOn = FSaveData.Instance.CircleChatAlramTimeAttack; } break;
                case eCircleChatAlramType.WeaponEnhance: { isOn = FSaveData.Instance.CircleChatAlramWeaponEnhance; } break;
            }
            alramFToggleList[i].SetToggle(isOn ? on : off, SelectEvent.Code);
        }
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        int activeIndex = (int)_currentSequenceType;
        for (int i = 0; i < sequenceList.Count; i++)
        {
            sequenceList[i].SetActive(i == activeIndex);
        }

        bool isYesBtnActive = true;
        bool isNoBtnActive = true;
        bool isConfirmBtnActive = false;
        bool isMemberSlotActive = false;
        bool isChatActive = false;
        bool isManagementActive = false;
        bool isMemberActive = false;

        switch (_currentSequenceType)
        {
            case eCircleSequenceType.Info:
                {
                    // Test - LeeSeungJin - Change String Start
                    titleLabel.textlocalize = "��Ŭ�� ����";
                    managementNoticeLabel.textlocalize = "������\n�ִ� 16�ڱ��� �Է��� �� �ֽ��ϴ�.\n�̸�Ƽ���� ����� �� �����ϴ�.";
                    // Test - LeeSeungJin - Change String End
                    infoGoodsUnit.InitGoodsUnit(eGOODSTYPE.CASH, GameInfo.Instance.GameConfig.CircleNameChangeCost);
                    nameInput.defaultText = GameInfo.Instance.CircleData.Name;
                    isManagementActive = true;
                    isYesBtnActive = false;
                    isConfirmBtnActive = true;
                }
                break;
            case eCircleSequenceType.Content:
                {
                    // Test - LeeSeungJin - Change String Start
                    titleLabel.textlocalize = "��Ŭ �Ұ��� ����";
                    managementNoticeLabel.textlocalize = "������\n�ִ� 100�ڱ��� �Է��� �� �ֽ��ϴ�.\n�̸�Ƽ���� ����� �� �����ϴ�.\n�Ǹ�, �̸��� �ּ�, ��ȭ��ȣ �� ���� ������ �Է��� �ﰡ�ּ���.";
                    // Test - LeeSeungJin - Change String End
                    contentInput.defaultText = GameInfo.Instance.CircleData.Content;
                    isManagementActive = true;
                }
                break;
            case eCircleSequenceType.MainLang:
                {
                    // Test - LeeSeungJin - Change String Start
                    titleLabel.textlocalize = "��Ŭ �� ��� ��� ����";
                    managementNoticeLabel.textlocalize = "��Ŭ �� ��� �� �������ּ���.";
                    // Test - LeeSeungJin - Change String End
                    mainLangLabel.textlocalize = FLocalizeString.Instance.GetText((int)_currentLang + 601);
                    isManagementActive = true;
                }
                break;
            case eCircleSequenceType.OtherLang:
                {
                    // Test - LeeSeungJin - Change String End
                    titleLabel.textlocalize = "��Ŭ �� ��� �� ����\n���� �㰡 ����";
                    managementNoticeLabel.textlocalize = "��Ŭ �� ��� ���� ���� ���θ�\n�������ּ���.";
                    // Test - LeeSeungJin - Change String End
                    isManagementActive = true;
                }
                break;
            case eCircleSequenceType.BuyStamp:
                {
                    titleLabel.textlocalize = "������ ����"; // Test - LeeSeungJin - Change String
                    stampBuyGoodsUnit.InitGoodsUnit(eGOODSTYPE.CIRCLEPOINT, 0);
                    // �ʿ䷮ / ������
                    long needCount = 0;
                    long circlePoint = 0;

                    stampBuyTex.mainTexture = null;
                    GameTable.ChatStamp.Param chatStampParam = GameInfo.Instance.GameTable.FindChatStamp(_stampTableId);
                    if (chatStampParam != null)
                    {
                        GameTable.Store.Param storeParam = GameInfo.Instance.GameTable.FindStore(chatStampParam.StoreID);
                        if (storeParam != null)
                        {
                            needCount = storeParam.PurchaseValue;
                            circlePoint = GameInfo.Instance.UserData.GetGoods((eGOODSTYPE)storeParam.PurchaseIndex);
                        }
                        stampBuyTex.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", Utility.AppendString("Icon/Circle/", chatStampParam.Icon, ".png")) as Texture;
                    }

                    eTEXTID textId = eTEXTID.WHITE_TEXT_COLOR;
                    if (circlePoint < needCount)
                    {
                        textId = eTEXTID.RED_TEXT_COLOR;
                    }

                    stampBuyGoodsUnit.SetText(FLocalizeString.Instance.GetText((int)textId, FLocalizeString.Instance.GetText(278, needCount, circlePoint)));
                    isChatActive = true;
                }
                break;
            case eCircleSequenceType.ChatAlramSet:
                {
                    titleLabel.textlocalize = "ä�� �˸� ����"; // Test - LeeSeungJin - Change String
                    isYesBtnActive = isNoBtnActive = false;
                    isChatActive = true;
                }
                break;
            case eCircleSequenceType.ViceManagement:
                {
                    titleLabel.textlocalize = "�ο� ����"; // Test - LeeSeungJin - Change String
                    isMemberSlotActive = true;
                    isYesBtnActive = isNoBtnActive = false;
                    isMemberActive = true;
                }
                break;
            case eCircleSequenceType.MemberManagement:
                {
                    titleLabel.textlocalize = "�ο� ����"; // Test - LeeSeungJin - Change String
                    isMemberSlotActive = true;
                    isYesBtnActive = isNoBtnActive = false;
                    isMemberActive = true;
                }
                break;
            case eCircleSequenceType.Expulsion:
                {
                    titleLabel.textlocalize = "�ȳ�"; // Test - LeeSeungJin - Change String
                    isMemberActive = true;
                }
                break;
            case eCircleSequenceType.Dissolution:
                {
                    titleLabel.textlocalize = "��Ŭ �ػ�"; // Test - LeeSeungJin - Change String
                }
                break;
            case eCircleSequenceType.Withdrawal:
                {
                    titleLabel.textlocalize = "��Ŭ Ż��"; // Test - LeeSeungJin - Change String
                }
                break;
        }

        chatObj.SetActive(isChatActive);
        managementObj.SetActive(isManagementActive);
        memberObj.SetActive(isMemberActive);

        yesBtn.SetActive(isYesBtnActive);
        noBtn.SetActive(isNoBtnActive);
        closeBtn.SetActive(!isNoBtnActive);
        confirmBtn.SetActive(isConfirmBtnActive);

        circleMemberListSlot.SetActive(isMemberSlotActive);
        if (circleMemberListSlot.gameObject.activeSelf)
        {
            circleMemberListSlot.UpdateSlot(0, _circleUserData, eCircleUserSlotType.Other);
        }
    }

    private string EnterSplitString(string value, int limitLine = 1)
    {
        _stringBuilder.Clear();
        string[] splits = value.Split('\n');
        for (int i = 0; i < limitLine; i++)
        {
            if (splits.Length <= i)
            {
                break;
            }
            _stringBuilder.Append(splits[i]);

            if (i < (limitLine - 1) && i < splits.Length - 1)
            {
                _stringBuilder.Append("\n");
            }
        }
        return _stringBuilder.ToString();
    }

    private bool OnEventAlramToggleSelectComponent(int nSelect, SelectEvent type, FToggle toggle)
    {
        if (type == SelectEvent.Click)
        {
            int index = alramFToggleList.FindIndex(x => x == toggle);
            if (index < 0 || (int)eCircleChatAlramType.WeaponEnhance < index)
            {
                return false;
            }

            FSaveData.Instance.SaveCircleChatData((eCircleChatAlramType)index, nSelect);
        }

        return true;
    }

    public void SetData(eCircleSequenceType sequenceType, string yesBtnStr = "", FriendUserData circleUserData = null, int stampTableId = -1)
    {
        _currentSequenceType = sequenceType;
        _circleUserData = circleUserData;
        _stampTableId = stampTableId;

        yesLabel.textlocalize = yesBtnStr;
        if (string.IsNullOrEmpty(yesBtnStr))
        {
            yesLabel.textlocalize = FLocalizeString.Instance.GetText(1);
        }
    }

    public void OnChangeInfo()
    {
        nameInput.label.textlocalize = nameInput.value = EnterSplitString(nameInput.value);
    }

    public void OnChangeContent()
    {
        contentInput.label.textlocalize = contentInput.value = EnterSplitString(contentInput.value, 5);
    }

    public void OnClick_YesBtn()
    {
        switch (_currentSequenceType)
        {
            case eCircleSequenceType.Info:
                {
                    if (string.IsNullOrEmpty(nameInput.value))
                    {
                        MessageToastPopup.Show("��Ŭ���� ��� �ֽ��ϴ�."); // Test - LeeSeungJin - Change String
                        return;
                    }

                    if (Utility.IsCommandCheck(nameInput.value))
                    {
                        MessagePopup.OK(eTEXTID.OK, "��Ŭ�� ��Ӿ ���ԵǾ� �ֽ��ϴ�.", null); // Test - LeeSeungJin - Change String
                        return;
                    }

                    if (!GameInfo.Instance.UserData.IsGoods(eGOODSTYPE.CASH, GameInfo.Instance.GameConfig.CircleNameChangeCost))
                    {
                        MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(101), null);
                        return;
                    }

                    GameInfo.Instance.Send_ReqCircleChangeName(nameInput.value, OnNet_CircleChangeInfo);
                }
                break;
            case eCircleSequenceType.Content:
                {
                    if (string.IsNullOrEmpty(contentInput.value))
                    {
                        MessageToastPopup.Show("��Ŭ �Ұ����� ��� �ֽ��ϴ�."); // Test - LeeSeungJin - Change String
                        return;
                    }

                    if (Utility.IsCommandCheck(contentInput.value))
                    {
                        MessagePopup.OK(eTEXTID.OK, "��Ŭ �Ұ����� ��Ӿ ���ԵǾ� �ֽ��ϴ�.", null); // Test - LeeSeungJin - Change String
                        return;
                    }

                    GameInfo.Instance.Send_ReqCircleChangeComment(contentInput.value, OnNet_CircleChangeInfo);
                }
                break;
            case eCircleSequenceType.MainLang:
                {
                    GameInfo.Instance.Send_ReqCircleChangeMainLanguage(_currentLang, OnNet_CircleChangeInfo);
                }
                break;
            case eCircleSequenceType.OtherLang:
                {
                    GameInfo.Instance.Send_ReqCircleChangeSuggestAnotherLangOpt(otherLangFToggle.kSelect == (int)eToggleType.On, OnNet_CircleChangeInfo);
                }
                break;
            case eCircleSequenceType.Dissolution:
                {
                    GameInfo.Instance.Send_ReqCircleDisperse(OnNet_GoToMain);
                }
                break;
            case eCircleSequenceType.Withdrawal:
                {
                    GameInfo.Instance.Send_ReqCircleWithdrawal(OnNet_GoToMain);
                }
                break;
            case eCircleSequenceType.BuyStamp:
                {
                    GameTable.ChatStamp.Param chatStampParam = GameInfo.Instance.GameTable.FindChatStamp(_stampTableId);
                    if (chatStampParam != null)
                    {
                        GameInfo.Instance.Send_ReqStorePurchase(chatStampParam.StoreID, false, 1, OnNet_CircleChangeInfo);
                    }
                }
                break;
            case eCircleSequenceType.Expulsion:
                {
                    if (GameInfo.Instance.UserData.CircleAuthLevel.IsLessThan(_circleUserData.CircleAuthLevel.AuthLevel))
                    {
                        MessageToastPopup.Show("������ �����մϴ�."); // Test - LeeSeungJin - Change String
                    }

                    GameInfo.Instance.Send_ReqCircleUserKick(_circleUserData.UUID, OnNet_CircleChangeInfo);
                }
                break;
        }
    }

    public void OnClick_NoBtn()
    {
        OnClickClose();
    }

    public void OnClick_LeftBtn()
    {
        if (_currentLang == eLANGUAGE.KOR)
        {
            return;
        }

        --_currentLang;

        Renewal();
    }

    public void OnClick_RightBtn()
    {
        if (_currentLang == eLANGUAGE.ESP)
        {
            return;
        }

        ++_currentLang;

        Renewal();
    }

    public void OnClick_DelegationBtn()
    {
        if (GameInfo.Instance.UserData.CircleAuthLevel.IsNotEqual(eCircleAuthLevel.MASTER))
        {
            MessageToastPopup.Show("������ �����մϴ�."); // Test - LeeSeungJin - Change String
            return;
        }

        if (_circleUserData.CircleAuthLevel.IsNotEqual(eCircleAuthLevel.DEPUTY))
        {
            MessagePopup.OK(eTEXTID.OK, "����� �κ����� �ƴմϴ�.", null); // Test - LeeSeungJin - Change String
            return;
        }

        GameInfo.Instance.Send_ReqCircleChangeAuthLevel(_circleUserData.UUID, eCircleAuthLevel.MASTER, OnNet_CircleChangeInfo);
    }

    public void OnClick_DismissalBtn()
    {
        if (GameInfo.Instance.UserData.CircleAuthLevel.IsNotEqual(eCircleAuthLevel.MASTER))
        {
            MessageToastPopup.Show("������ �����մϴ�."); // Test - LeeSeungJin - Change String
            return;
        }

        if (_circleUserData.CircleAuthLevel.IsNotEqual(eCircleAuthLevel.DEPUTY))
        {
            MessagePopup.OK(eTEXTID.OK, "����� �κ����� �ƴմϴ�.", null); // Test - LeeSeungJin - Change String
            return;
        }

        GameInfo.Instance.Send_ReqCircleChangeAuthLevel(_circleUserData.UUID, eCircleAuthLevel.MEMBER, OnNet_CircleChangeInfo);
    }

    public void OnClick_ExpulsionBtn()
    {
        if (GameInfo.Instance.UserData.CircleAuthLevel.IsLessThan(_circleUserData.CircleAuthLevel.AuthLevel))
        {
            MessageToastPopup.Show("������ �����մϴ�."); // Test - LeeSeungJin - Change String
            return;
        }

        SetData(eCircleSequenceType.Expulsion, circleUserData: _circleUserData);
        Renewal();
    }

    public void OnClick_AppointmentBtn()
    {
        if (GameInfo.Instance.UserData.CircleAuthLevel.IsNotEqual(eCircleAuthLevel.MASTER))
        {
            MessageToastPopup.Show("������ �����մϴ�."); // Test - LeeSeungJin - Change String
            return;
        }

        if (GameInfo.Instance.CircleData.SubLeaderMaxCount <= GameInfo.Instance.CircleData.SubLeaderCount)
        {
            MessagePopup.OK(eTEXTID.OK, "�κ��� �Ӹ� �� �ʰ�", null); // Test - LeeSeungJin - Change String
            return;
        }

        if (_circleUserData.CircleAuthLevel.IsNotEqual(eCircleAuthLevel.MEMBER))
        {
            MessagePopup.OK(eTEXTID.OK, "����� �ο��� �ƴմϴ�.", null); // Test - LeeSeungJin - Change String
            return;
        }

        GameInfo.Instance.Send_ReqCircleChangeAuthLevel(_circleUserData.UUID, eCircleAuthLevel.DEPUTY, OnNet_CircleChangeInfo);
    }

    private void OnNet_CircleChangeInfo(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        LobbyUIManager.Instance.Renewal("CircleLobbyPanel");
        LobbyUIManager.Instance.Renewal("TopPanel");

        OnClickClose();
    }

    private void OnNet_GoToMain(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN);

        OnClickClose();
    }
}
