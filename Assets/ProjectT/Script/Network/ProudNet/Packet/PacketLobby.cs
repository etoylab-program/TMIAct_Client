using Nettention.Proud;


public class PacketLobbyBase : PacketBase
{
    // 클라이언트서 서버로 요청 보내는 오브젝트
    protected LobbyC2S.Proxy m_C2SProxy = new LobbyC2S.Proxy();
    // 서버에서 응답을 받는 오브젝트
    protected LobbyS2C.Stub m_S2CStub = new LobbyS2C.Stub();

    public LobbyS2C.Stub Recv() { return m_S2CStub; }

    public override void DoAttach(NetClient _netClient) { return; }

    public virtual void ReqMoveToLogin() { Debug.LogError("Not Make embody - ReqMoveToLogin"); }
    public virtual void ReqMoveToBattle() { Debug.LogError("Not Make embody - ReqMoveToBattle"); }

    public virtual void ReqUserMarkList() { Debug.LogError("Not Make embody - ReqUserMarkList"); }
    public virtual void ReqUserSetMark(System.UInt32 _userMarkTID) { Debug.LogError("Not Make embody - ReqUserSetMark"); }
    public virtual void ReqUserLobbyThemeList() { Debug.LogError("Not Make embody - ReqUserLobbyThemeList"); }
    public virtual void ReqUserSetLobbyTheme(System.UInt32 _lobbyThemeTID) { Debug.LogError("Not Make embody - ReqUserSetLobbyTheme"); }
    public virtual void ReqUserSetMainCardFormation(System.UInt32 _cardFrmtID) { Debug.LogError("Not Make embody - ReqUserSetMainCardFormation"); }
    public virtual void ReqUserCardFormationFavi(PktInfoTIDList _pkt) { Debug.LogError("Not Make embody - ReqUserCardFormationFavi"); }
    public virtual void ReqUserSetName(PktInfoStr _str) { Debug.LogError("Not Make embody - ReqUserSetName"); }
    public virtual void ReqUserSetCommentMsg(PktInfoStr _str) { Debug.LogError("Not Make embody - ReqUserSetCommentMsg"); }
    public virtual void ReqUserSetCountryAndLangCode(PktInfoCountryLangCode _str) { Debug.LogError("Not Make embody - ReqUserSetCountryAndLangCode"); }
    public virtual void ReqUserPkgShowOff() { Debug.LogError("Not Make embody - ReqUserPkgShowOff"); }

    public virtual void ReqFacilityUpgrade(System.UInt32 _facilityTID) { Debug.LogError("Not Make embody - ReqFacilityUpgrade"); }
    public virtual void ReqFacilityOperation(PktInfoFacilityOperationReq _pkt) { Debug.LogError("Not Make embody - ReqFacilityOperation"); }
    public virtual void ReqFacilityOperationConfirm(PktInfoFacilityOperConfirmReq _pkt) { Debug.LogError("Not Make embody - ReqFacilityOperationConfirm"); }

    public virtual void ReqDispatchOpen(PktInfoTIDList _pkt) { Debug.LogError("Not Make embody - ReqDispatchOpen"); }
    public virtual void ReqDispatchChange(PktInfoTIDList _pkt) { Debug.LogError("Not Make embody - ReqDispatchChange"); }
    public virtual void ReqDispatchOperation(PktInfoDispatchOperReq _pkt) { Debug.LogError("Not Make embody - ReqDispatchOperation"); }
    public virtual void ReqDispatchOperationConfirm(PktInfoDispatchOperConfirmReq _pkt) { Debug.LogError("Not Make embody - ReqDispatchOperationConfirm"); }

    public virtual void ReqSetMainRoomTheme(System.Byte _roomThemeSlotNum) { Debug.LogError("Not Make embody - ReqSetMainRoomTheme"); }
    public virtual void ReqRoomPurchase(System.UInt32 _storeRoomTID) { Debug.LogError("Not Make embody - ReqRoomPurchase"); }
    public virtual void ReqRoomThemeSlotDetailInfo(System.Byte _roomThemeSlotNum) { Debug.LogError("Not Make embody - ReqRoomThemeSlotDetailInfo"); }
    public virtual void ReqRoomThemeSlotSave(PktInfoRoomThemeSlotDetail _pkt) { Debug.LogError("Not Make embody - ReqRoomThemeSlotSave"); }

    public virtual void ReqStorePurchase(PktInfoStorePurchaseReq _pkt) { Debug.LogError("Not Make embody - ReqStorePurchase"); }
    public virtual void ReqStorePurchaseInApp(PktInfoStorePurchaseInAppReq _pkt) { Debug.LogError("Not Make embody - ReqStorePurchaseInApp"); }
    public virtual void ReqSteamPurchase(PktInfoSteamPurchaseReq _pkt) { Debug.LogError("Not Make embody - ReqSteamPurchase"); }
    public virtual void ReqUserRotationGachaOpen(PktInfoTIDList _pkt) { Debug.LogError("Not Make embody - ReqUserRotationGachaOpen"); }
    public virtual void ReqRaidStoreList(PktInfoRaidStoreListReq _pkt) { Debug.LogError("Not Make embody - ReqRaidStoreList"); }

    public virtual void ReqMailList(PktInfoMailListReq _pkt) { Debug.LogError("Not Make embody - ReqMailList"); }
    public virtual void ReqMailTakeProductList(PktInfoMailProductTakeReq _pkt) { Debug.LogError("Not Make embody - ReqMailTakeProductList"); }

    
    public virtual void ReqCommunityInfoGet() { Debug.LogError("Not Make embody - ReqCommunityInfoGet"); }
    public virtual void ReqCommunityUserArenaInfoGet(PktInfoCommuUserArenaInfoReq _pkt) { Debug.LogError("Not Make embody - ReqCommunityUserArenaInfoGet"); }
    public virtual void ReqCommunityUserUseCallCnt(PktInfoCommuUseCallCntReq _pkt) { Debug.LogError("Not Make embody - ReqCommunityUserUseCallCnt"); }
    public virtual void ReqFriendSuggestList(PktInfoCommuSuggestReq _pkt) { Debug.LogError("Not Make embody - ReqFriendSuggestList"); }
    public virtual void ReqFriendAsk(PktInfoCommuAskReq _pkt) { Debug.LogError("Not Make embody - ReqFriendAsk"); }
    public virtual void ReqFriendAskDel(PktInfoCommuAskDel _pkt) { Debug.LogError("Not Make embody - ReqFriendAskDel"); }
    public virtual void ReqFriendAnswer(PktInfoCommuAnswer _pkt) { Debug.LogError("Not Make embody - ReqFriendAnswer"); }
    public virtual void ReqFriendKick(PktInfoCommuKick _pkt) { Debug.LogError("Not Make embody - ReqFriendKick"); }
    public virtual void ReqFriendPointGive() { Debug.LogError("Not Make embody - ReqFriendPointGive"); }
    public virtual void ReqFriendPointTake(PktInfoFriendPointTakeReq _pkt) { Debug.LogError("Not Make embody - ReqFriendPointTake"); }
    public virtual void ReqFriendRoomVisitFlag(PktInfoFriendRoomFlag _pkt) { Debug.LogError("Not Make embody - ReqFriendRoomVisitFlag"); }
    public virtual void ReqFriendRoomInfoGet(PktInfoCommuRoomInfoGet _pkt) { Debug.LogError("Not Make embody - ReqFriendRoomInfoGet"); }

    public virtual void ReqInfluenceChoice(PktInfoInfluenceChoice _pkt) { Debug.LogError("Not Make embody - ReqInfluenceChoice"); }
    public virtual void ReqGetInfluenceInfo() { Debug.LogError("Not Make embody - ReqGetInfluenceInfo"); }
    public virtual void ReqGetInfluenceRankInfo(PktInfoInfluRankListReq _pkt) { Debug.LogError("Not Make embody - ReqGetInfluenceRankInfo"); }
    public virtual void ReqInfluenceTgtRwd(PktInfoRwdInfluenceTgtReq _pkt) { Debug.LogError("Not Make embody - ReqInfluenceTgtRwd"); }

    public virtual void ReqBingoEventReward(PktInfoBingoEventRewardReq _pkt) { Debug.LogError("Not Make embody - ReqBingoEventReward"); }
    public virtual void ReqBingoNextOpen(System.UInt32 _groupId) { Debug.LogError("Not Make embody - ReqBingoNextOpen"); }
	public virtual void ReqUserSetNameColor(System.UInt32 _colorID) { Debug.LogError("Not Make embody - ReqUserSetNameColor"); }
}
public class PacketLobby : PacketLobbyBase
{
    RmiContext m_ZipSend = new RmiContext();

    public override void DoAttach(NetClient _netClient)
    {
        m_ZipSend.reliability = MessageReliability.MessageReliability_Reliable;
        m_ZipSend.compressMode = CompressMode.CM_Zip;

        _netClient.AttachProxy(m_C2SProxy);
        _netClient.AttachStub(m_S2CStub);
        return;
    }

    public override void ReqMoveToLogin() {
        m_C2SProxy.ReqMoveToLogin(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqMoveToBattle() {
        m_C2SProxy.ReqMoveToBattle(HostID.HostID_Server, RmiContext.ReliableSend);
    }

    public override void ReqUserMarkList() {
        m_C2SProxy.ReqUserMarkList(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqUserSetMark(System.UInt32 _userMarkTID) {
        m_C2SProxy.ReqUserSetMark(HostID.HostID_Server, RmiContext.ReliableSend, _userMarkTID);
    }
    public override void ReqUserLobbyThemeList() {
        m_C2SProxy.ReqUserLobbyThemeList(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqUserSetLobbyTheme(System.UInt32 _lobbyThemeTID) {
        m_C2SProxy.ReqUserSetLobbyTheme(HostID.HostID_Server, RmiContext.ReliableSend, _lobbyThemeTID);
    }
    public override void ReqUserSetMainCardFormation(System.UInt32 _cardFrmtID) {
        m_C2SProxy.ReqUserSetMainCardFormation(HostID.HostID_Server, RmiContext.ReliableSend, _cardFrmtID);
    }
    public override void ReqUserCardFormationFavi(PktInfoTIDList _pkt) {
        m_C2SProxy.ReqUserCardFormationFavi(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqUserSetName(PktInfoStr _str) 
    {
        m_C2SProxy.ReqUserSetName(HostID.HostID_Server, RmiContext.ReliableSend, _str);
    }
    public override void ReqUserSetCommentMsg(PktInfoStr _str) 
    {
        m_C2SProxy.ReqUserSetCommentMsg(HostID.HostID_Server, RmiContext.ReliableSend, _str);
    }
    public override void ReqUserSetCountryAndLangCode(PktInfoCountryLangCode _str)
    {
        m_C2SProxy.ReqUserSetCountryAndLangCode(HostID.HostID_Server, RmiContext.ReliableSend, _str);
    }
    public override void ReqUserPkgShowOff()
    {
        m_C2SProxy.ReqUserPkgShowOff(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqFacilityUpgrade(System.UInt32 _facilityTID) {
        m_C2SProxy.ReqFacilityUpgrade(HostID.HostID_Server, RmiContext.ReliableSend, _facilityTID);
    }
    public override void ReqFacilityOperation(PktInfoFacilityOperationReq pktInfo) 
    {
        m_C2SProxy.ReqFacilityOperation(HostID.HostID_Server, RmiContext.ReliableSend, pktInfo);
    }
    public override void ReqFacilityOperationConfirm(PktInfoFacilityOperConfirmReq pktInfo) 
    {
        m_C2SProxy.ReqFacilityOperationConfirm(HostID.HostID_Server, RmiContext.ReliableSend, pktInfo);
    }

    public override void ReqDispatchOpen(PktInfoTIDList _pkt) {
        m_C2SProxy.ReqDispatchOpen(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqDispatchChange(PktInfoTIDList _pkt) {
        m_C2SProxy.ReqDispatchChange(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqDispatchOperation(PktInfoDispatchOperReq _pkt) {
        m_C2SProxy.ReqDispatchOperation(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqDispatchOperationConfirm(PktInfoDispatchOperConfirmReq _pkt) {
        m_C2SProxy.ReqDispatchOperationConfirm(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqSetMainRoomTheme(System.Byte _roomThemeSlotNum) {
        m_C2SProxy.ReqSetMainRoomTheme(HostID.HostID_Server, RmiContext.ReliableSend, _roomThemeSlotNum);
    }
    public override void ReqRoomPurchase(System.UInt32 _storeRoomTID) {
        m_C2SProxy.ReqRoomPurchase(HostID.HostID_Server, RmiContext.ReliableSend, _storeRoomTID);
    }
    public override void ReqRoomThemeSlotDetailInfo(System.Byte _roomThemeSlotNum) {
        m_C2SProxy.ReqRoomThemeSlotDetailInfo(HostID.HostID_Server, RmiContext.ReliableSend, _roomThemeSlotNum);
    }
    public override void ReqRoomThemeSlotSave(PktInfoRoomThemeSlotDetail _pkt) {
        m_C2SProxy.ReqRoomThemeSlotSave(HostID.HostID_Server, m_ZipSend, _pkt);
    }
    //public override void ReqRoomThemeSlotSetChar(System.Byte _slotIdx, System.UInt64 _cuid) {
    //    m_pktRoomThemeSlotSetChar.slotIdx_ = _slotIdx;
    //    m_pktRoomThemeSlotSetChar.applyCuid_ = _cuid;
    //    m_C2SProxy.ReqRoomThemeSlotSetChar(HostID.HostID_Server, RmiContext.ReliableSend, m_pktRoomThemeSlotSetChar);
    //}
    //public override void ReqRoomThemeSlotSetAction(System.Byte _slotIdx, System.UInt32 _actionID, System.UInt32 _value) {
    //    m_pktRoomThemeSlotSetAction.slotIdx_ = _slotIdx;
    //    m_pktRoomThemeSlotSetAction.actionID_ = _actionID;
    //    m_pktRoomThemeSlotSetAction.value_ = _value;
    //    m_C2SProxy.ReqRoomThemeSlotSetAction(HostID.HostID_Server, RmiContext.ReliableSend, m_pktRoomThemeSlotSetAction);
    //}

    public override void ReqStorePurchase(PktInfoStorePurchaseReq _pkt) {
        m_C2SProxy.ReqStorePurchase(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqStorePurchaseInApp(PktInfoStorePurchaseInAppReq _pkt) {
        RmiContext tmpRmi = new RmiContext();
        tmpRmi              = RmiContext.SecureReliableSend;
        tmpRmi.compressMode = CompressMode.CM_Zip;
        m_C2SProxy.ReqStorePurchaseInApp(HostID.HostID_Server, RmiContext.SecureReliableSend, _pkt);
    }
    public override void ReqSteamPurchase(PktInfoSteamPurchaseReq _pkt) {
        m_C2SProxy.ReqSteamPurchase(HostID.HostID_Server, RmiContext.SecureReliableSend, _pkt);
    }
    public override void ReqUserRotationGachaOpen(PktInfoTIDList _pkt) {
        m_C2SProxy.ReqUserRotationGachaOpen(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqRaidStoreList(PktInfoRaidStoreListReq _pkt) {
        m_C2SProxy.ReqRaidStoreList(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqMailList(PktInfoMailListReq _pkt) {
        m_C2SProxy.ReqMailList(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqMailTakeProductList(PktInfoMailProductTakeReq _pkt) {
        m_C2SProxy.ReqMailTakeProductList(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqCommunityInfoGet() {
        m_C2SProxy.ReqCommunityInfoGet(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqCommunityUserArenaInfoGet(PktInfoCommuUserArenaInfoReq _pkt) {
        m_C2SProxy.ReqCommunityUserArenaInfoGet(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqCommunityUserUseCallCnt(PktInfoCommuUseCallCntReq _pkt) {
        m_C2SProxy.ReqCommunityUserUseCallCnt(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqFriendSuggestList(PktInfoCommuSuggestReq _pkt) {
        m_C2SProxy.ReqFriendSuggestList(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqFriendAsk(PktInfoCommuAskReq _pkt) {
        m_C2SProxy.ReqFriendAsk(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqFriendAskDel(PktInfoCommuAskDel _pkt) {
        m_C2SProxy.ReqFriendAskDel(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqFriendAnswer(PktInfoCommuAnswer _pkt) {
        m_C2SProxy.ReqFriendAnswer(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqFriendKick(PktInfoCommuKick _pkt) {
        m_C2SProxy.ReqFriendKick(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqFriendPointGive() {
        m_C2SProxy.ReqFriendPointGive(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqFriendPointTake(PktInfoFriendPointTakeReq _pkt) {
        m_C2SProxy.ReqFriendPointTake(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqFriendRoomVisitFlag(PktInfoFriendRoomFlag _pkt) {
        m_C2SProxy.ReqFriendRoomVisitFlag(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqFriendRoomInfoGet(PktInfoCommuRoomInfoGet _pkt) {
        m_C2SProxy.ReqFriendRoomInfoGet(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqInfluenceChoice(PktInfoInfluenceChoice _pkt) {
        m_C2SProxy.ReqInfluenceChoice(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqGetInfluenceInfo() {
        m_C2SProxy.ReqGetInfluenceInfo(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqGetInfluenceRankInfo(PktInfoInfluRankListReq _pkt) {
        m_C2SProxy.ReqGetInfluenceRankInfo(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqInfluenceTgtRwd(PktInfoRwdInfluenceTgtReq _pkt) {
        m_C2SProxy.ReqInfluenceTgtRwd(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqBingoEventReward(PktInfoBingoEventRewardReq _pkt) {
        m_C2SProxy.ReqBingoEventReward(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqBingoNextOpen(System.UInt32 _groupId) {
        m_C2SProxy.ReqBingoNextOpen(HostID.HostID_Server, RmiContext.ReliableSend, _groupId);
    }
	public override void ReqUserSetNameColor(System.UInt32 _colorID)
	{
		m_C2SProxy.ReqUserSetNameColor(HostID.HostID_Server, RmiContext.ReliableSend, _colorID);
	}
}