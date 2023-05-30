using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class UIAccountLinkingPopup : FComponent
{
    [Header("[Property]")]
    public GameObject   RootBtns;
    public Vector3[]    RootBtnsPos;
    public UIButton     BtnAccountLink;
	public GameObject	CheckDeleteObj;
	public UILabel		lbDeleteCode;
	public UIInput		inputDeleteCode;

    [Header("[Reward]")]
    [SerializeField] private UISprite LinkRewardEnableSpr;
    [SerializeField] private UISprite LinkRewardDisableSpr;
    [SerializeField] private UILabel LinkRewardLabel;

    [SerializeField] private UISprite CodeRewardEnableSpr;
    [SerializeField] private UISprite CodeRewardDisableSpr;
    [SerializeField] private UILabel CodeRewardLabel;

	private StringBuilder mSbCheckDeleteCode = new StringBuilder();


    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        BtnAccountLink.gameObject.SetActive(true);
        RootBtns.transform.localPosition = RootBtnsPos[0];
		CheckDeleteObj.gameObject.SetActive(false);

		//if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
		//{
		//    BtnAccountLink.gameObject.SetActive(false);
		//    RootBtns.transform.localPosition = RootBtnsPos[1];
		//}
		//else
		//{
		//    BtnAccountLink.gameObject.SetActive(true);
		//    RootBtns.transform.localPosition = RootBtnsPos[0];
		//}
		GameInfo.Instance.Send_ReqLinkAccountList(OnNetLinkAccountList);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        SetStateReward_Link();
        SetStateReward_Code();
    }

    private void SetStateReward_Link()
    {
        LinkRewardEnableSpr.SetActive(false);
        LinkRewardDisableSpr.SetActive(false);
        LinkRewardLabel.SetActive(true);

        //AccountLinkList가 없으면 보상받기/클릭불가
        if (GameInfo.Instance.UserData.AccountLinkList.Count == 0)
        {
            LinkRewardDisableSpr.SetActive(true);
            LinkRewardLabel.textlocalize = FLocalizeString.Instance.GetText(1684); // 보상받기
            return;
        }

        //AccountLinkList 있음, 보상 수령 상태에 따라 표현
        LinkRewardEnableSpr.SetActive(!GameInfo.Instance.UserData.AccountLinkReward);
        LinkRewardDisableSpr.SetActive(GameInfo.Instance.UserData.AccountLinkReward);        

        string strBtnName = FLocalizeString.Instance.GetText(1684); // 보상받기
        if (GameInfo.Instance.UserData.AccountLinkReward)
            strBtnName = FLocalizeString.Instance.GetText(1685); // 보상받음
        LinkRewardLabel.textlocalize = strBtnName;
    }

    private void SetStateReward_Code()
    {
        CodeRewardEnableSpr.SetActive(false);
        CodeRewardDisableSpr.SetActive(false);
        CodeRewardLabel.SetActive(true);

        //Password가 설정되어 있지 않으면 보상받기/클릭불가
        if (!GameInfo.Instance.UserData.PasswordSet)
        {
            CodeRewardDisableSpr.SetActive(true);
            CodeRewardLabel.textlocalize = FLocalizeString.Instance.GetText(1684); // 보상받기
            return;
        }

        CodeRewardEnableSpr.SetActive(!GameInfo.Instance.UserData.AccountCodeReward);
        CodeRewardDisableSpr.SetActive(GameInfo.Instance.UserData.AccountCodeReward);        

        string strBtnName = FLocalizeString.Instance.GetText(1684); // 보상받기
        if (GameInfo.Instance.UserData.AccountCodeReward)
            strBtnName = FLocalizeString.Instance.GetText(1685); // 보상받음
        CodeRewardLabel.textlocalize = strBtnName;
    }
    public void OnClick_AccountLinkingBtn()
    {
        //GameInfo.Instance.Send_ReqLinkAccountList(OnNetLinkAccountList);
        LobbyUIManager.Instance.ShowUI("AccountDateConnectPopup", true);
    }
    public void OnClick_CodeBtn()
    {
        //if (GameInfo.Instance.UserData.AccountCode == "" || GameInfo.Instance.UserData.AccountCode == string.Empty )
        //    GameInfo.Instance.Send_ReqAccountCode(OnNetAccountCode);
        //else
        //    LobbyUIManager.Instance.ShowUI("AccountGetCodePopup", true);
        LobbyUIManager.Instance.ShowUI("AccountGetCodePopup", true);
    }
    public void OnClick_InitBtn()
    {
        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
        {
            MessagePopup.CYN(eTEXTID.TITLE_NOTICE, 3201, eTEXTID.OK, eTEXTID.CANCEL, OnMsg_DataInit);
        }
        else
        {
            MessagePopup.CYN(eTEXTID.TITLE_NOTICE, 3088, eTEXTID.OK, eTEXTID.CANCEL, OnMsg_DataInit);
        }
    }

    public void OnClick_LinkReward()
    {
        //AccountLinkList가 없으면 보상받기/클릭불가
        if (GameInfo.Instance.UserData.AccountLinkList.Count == 0)
            return;
        if (GameInfo.Instance.UserData.AccountLinkReward)
            return;

        GameInfo.Instance.Send_ReqAccountLinkReward(OnNetAccountLinkReward);
    }

    public void OnClick_CodeReward()
    {
        //Password가 설정되어 있지 않으면 보상받기/클릭불가
        if (!GameInfo.Instance.UserData.PasswordSet)
            return;
        if (GameInfo.Instance.UserData.AccountCodeReward)
            return;

        GameInfo.Instance.Send_ReqAccountCodeReward(OnNetAccountCodeReward);
    }

	public void OnBtnDeleteAccount()
	{
		inputDeleteCode.value = "";
		MessagePopup.CYN(eTEXTID.TITLE_NOTICE, 3279, eTEXTID.OK, eTEXTID.CANCEL, ConfirmDelete);
	}

	public void OnBtnCancelDelete()
	{
		CheckDeleteObj.SetActive(false);
	}

	public void OnBtnConfirmDelete()
	{
		if (lbDeleteCode.textlocalize.CompareTo(inputDeleteCode.value.ToUpper()) != 0)
		{
			MessagePopup.OK(eTEXTID.TITLE_NOTICE, 3284, null);
			return;
		}

		MessagePopup.CYN(FLocalizeString.Instance.GetText(3280), FLocalizeString.Instance.GetText(3285), 
						 FLocalizeString.Instance.GetText(1), FLocalizeString.Instance.GetText(2), 
						 ReqAccountDelete, OnBtnCancelDelete);
	}

    public void OnClick_BackBtn()
    {
        OnClickClose();
    }

    public void OnMsg_DataInit()
    {
#if UNITY_STANDALONE
        // 해상도 저장
        int width = PlayerPrefs.GetInt("Screen.width");
        int height = PlayerPrefs.GetInt("Screen.height");
        int fullscr = PlayerPrefs.GetInt("Screen.fullScreen");
#endif

        PlayerPrefs.DeleteAll();

#if UNITY_STANDALONE
        PlayerPrefs.SetInt("Screen.width", width);
        PlayerPrefs.SetInt("Screen.height", height);
        PlayerPrefs.SetInt("Screen.fullScreen", fullscr);
        PlayerPrefs.SetInt("InitData", 1);
#endif
        FGlobalTimer.Instance.RemoveAllTimer();
        GameInfo.Instance.SetPktInitData();
        GameInfo.Instance.ClearCallBackList();
        GameInfo.Instance.MoveLobbyToTitle();
    }

    public void OnNetLinkAccountList(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        Renewal(true);

        if (GameInfo.Instance.UserData.AccountCode == "" || GameInfo.Instance.UserData.AccountCode == string.Empty)
            GameInfo.Instance.Send_ReqAccountCode(OnNetAccountCode);        

        //LobbyUIManager.Instance.ShowUI("AccountDateConnectPopup", true);
    }

    public void OnNetAccountCode(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        Renewal(true);
        //LobbyUIManager.Instance.ShowUI("AccountGetCodePopup", true);
    }

    private void OnNetAccountLinkReward(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        PktInfoGoods pAck = (PktInfoGoods )pktmsg;
        List<RewardData> rewards = new List<RewardData>();

        //보상은 항상 대마석입니다.
        if (pAck.infos_ != null && pAck.infos_.Count > 0)
        {   
            RewardData data = new RewardData((int)eREWARDTYPE.GOODS, (int)pAck.infos_[0].type_, GameInfo.Instance.GameConfig.AccountInterlockReward);
            rewards.Add(data);
        }

        if(rewards.Count > 0)
        {
            MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText(1262), FLocalizeString.Instance.GetText(1263), rewards);
        }

        Renewal(false);

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
    }

    private void OnNetAccountCodeReward(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        PktInfoGoods pAck = (PktInfoGoods)pktmsg;
        List<RewardData> rewards = new List<RewardData>();

        //보상은 항상 대마석입니다.
        if (pAck.infos_ != null && pAck.infos_.Count > 0)
        {
            RewardData data = new RewardData((int)eREWARDTYPE.GOODS, (int)pAck.infos_[0].type_, GameInfo.Instance.GameConfig.AccountInterlockReward);
            rewards.Add(data);
        }

        if (rewards.Count > 0)
        {            
            MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText(1262), FLocalizeString.Instance.GetText(1263), rewards);
        }

        Renewal(false);

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
    }

	private void ConfirmDelete()
	{
		CheckDeleteObj.SetActive(true);

		mSbCheckDeleteCode.Clear();
		mSbCheckDeleteCode.Capacity = 4;

		for (int i = 0; i < 4; ++i)
		{
			char rand = (char)Random.Range(65, 91);
			mSbCheckDeleteCode.Append(rand);
		}

		lbDeleteCode.textlocalize = mSbCheckDeleteCode.ToString();
	}

	private void ReqAccountDelete()
	{
		GameInfo.Instance.Send_ReqAccountDelete(OnAccountDelete);
	}

	private void OnAccountDelete(int result, PktMsgType pktmsg)
	{
		if (result != 0)
		{
			return;
		}

        PlayerPrefs.DeleteKey( "User_AccountUUID" );

        FGlobalTimer.Instance.RemoveAllTimer();
        GameInfo.Instance.SetPktInitData();
        GameInfo.Instance.ClearCallBackList();
        GameInfo.Instance.MoveLobbyToTitle();
    }
}
