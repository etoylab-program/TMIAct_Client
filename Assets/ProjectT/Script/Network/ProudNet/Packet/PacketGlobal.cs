using Nettention.Proud;


public class PacketGlobalBase : PacketBase
{
    public delegate void Event(PacketGlobal _pktGbl);

    // 클라이언트서 서버로 요청 보내는 오브젝트
    protected GlobalC2S.Proxy m_C2SProxy = new GlobalC2S.Proxy();
    // 서버에서 응답을 받는 오브젝트
    protected GlobalS2C.Stub m_S2CStub = new GlobalS2C.Stub();

    //public GlobalC2S.Proxy Send() { return m_C2SProxy; }
    public GlobalS2C.Stub Recv() { return m_S2CStub; }

    public override void DoAttach(NetClient _netClient) {; }

    public virtual void ReqPing() { Debug.LogError("Not Make embody - ReqPing"); }                                  // 핑 요청
    public virtual void ReqLogOnCreditKey() { Debug.LogError("Not Make embody - ReqLogOnCreditKey"); }              // 크래딧 키 로그온 요청
    public virtual void ReqLogOut() { Debug.LogError("Not Make embody - ReqLogOut"); }                              // 로그아웃 요청
    public virtual void ReqGetTotlaRelocateCntToNotComplete() { Debug.LogError("Not Make embody - ReqGetTotlaRelocateCntToNotComplete"); }    // 서버 이전을 아직 진행하지 않은 유저의 수 요청
    public virtual void ReqRelocateUserInfoSet(PktInfoRelocateUser _pkt) { Debug.LogError("Not Make embody - ReqRelocateUserInfoSet"); }    // 유저 서버 이전 정보 설정 요청
    public virtual void ReqRelocateUserComplate(PktInfoRelocateUser _pkt) { Debug.LogError("Not Make embody - ReqRelocateUserComplate"); }  // 유저 서버 이전 완료 요청
    public virtual void ReqRefrashUserInfo() { Debug.LogError("Not Make embody - ReqRefrashUserInfo"); }            // 유저의 특정 정보에 대한 현재 값을 요청
    public virtual void ReqReConnectUserInfo(PktInfoReconnectUserInfoReq _pkt) { Debug.LogError("Not Make embody - ReqReConnectUserInfo"); } // 재접속 유저 정보 요청
    public virtual void ReqAccountCode() { Debug.LogError("Not Make embody - ReqAccountCode"); }                    // 이어하기 코드 요청
    public virtual void ReqAccountSetPassword(PktInfoStr _pkt) { Debug.LogError("Not Make embody - ReqAccountSetPassword"); }      // 이어하기 비밀번호 설정 요청
    public virtual void ReqAccountCodeReward() { Debug.LogError("Not Make embody - ReqAccountCodeReward"); }              // 이어하기코드 보상 요청
    public virtual void ReqAccountLinkReward() { Debug.LogError("Not Make embody - ReqAccountLinkReward"); }              // 계정연동 보상 요청
    public virtual void ReqLinkAccountList() { Debug.LogError("Not Make embody - ReqLinkAccountList"); }            // 이어하기 가능한 연결 계정 목록 요청
    public virtual void ReqAddLinkAccountAuth(PktInfoAccountLinkInfo _pkt) { Debug.LogError("Not Make embody - ReqAddLinkAccountAuth"); } // 이어하기 가능한 연결 계정 정보 추가 요청
	public virtual void ReqAccountDelete() { Debug.LogError("Not Make embody - ReqAccountDelete"); }				// 계정 삭제
    public virtual void ReqGetUserInfoFromAccountLink(PktInfoUserInfoFromLinkReq _pkt) { Debug.LogError("Not Make embody - ReqGetUserInfoFromAccountLink"); }   // 유저 정보 계정 연동 정보 요청
    public virtual void ReqPushNotifiTokenSet(PktInfoPushNotiSetToken _pkt) { Debug.LogError("Not Make embody - ReqPushNotifiTokenSet"); }  // 유저 푸시 토큰 설정 요청

    public virtual void ReqReflashLoginBonus() { Debug.LogError("Not Make embody - ReqReflashLoginBonus"); }        // 로그인 보너스 요청
    public virtual void ReqRewardTakeAchieve(PktInfoTIDList _pkt) { Debug.LogError("Not Make embody - ReqRewardTakeAchieve"); }   // 유저 공적 보상 획득 요청
    public virtual void ReqRewardTakeAchieveEvent(PktInfoTIDList _pkt) { Debug.LogError("Not Make embody - ReqRewardTakeAchieveEvent"); }   // 유저 공적 이벤트 보상 획득 요청
    public virtual void ReqRewardDailyMission(PktInfoRwdDailyMissionReq _pkt) { Debug.LogError("Not Make embody - ReqRewardDailyMission"); }   // 유저 일일 미션 보상 요청
    public virtual void ReqRewardWeekMission(PktInfoComRwd _pkt) { Debug.LogError("Not Make embody - ReqRewardWeekMission"); }   // 유저 주간 미션 보상 요청
    public virtual void ReqRewardInfluMission(PktInfoComRwd _pkt) { Debug.LogError("Not Make embody - ReqRewardInfluMission"); }   // 유저 서버 달성(세력) 미션 보상 요청
    public virtual void ReqUpdateGllaMission(PktInfoUpdateGllaMission _pkt) { Debug.LogError("Not Make embody - ReqUpdateGllaMission"); }   // 유저 게릴라 미션 카운팅 요청
    public virtual void ReqRewardGllaMission(PktInfoRwdGllaMission _pkt) { Debug.LogError("Not Make embody - ReqRewardGllaMission"); }   // 유저 게릴라 미션 보상 요청
    public virtual void ReqRewardPassMission(PktInfoRwdPassMission _pkt) { Debug.LogError("Not Make embody - ReqRewardPassMission"); }   // 유저 패스 미션 보상 응답
    public virtual void ReqRewardPass(PktInfoRwdPassReq _pkt) { Debug.LogError("Not Make embody - ReqRewardPass"); }   // 유저 패스 보상 획득 응답
    public virtual void ReqEventRewardReset(System.UInt32 _eventID) { Debug.LogError("Not Make embody - ReqEventRewardReset"); }   // 유저 이벤트 보상 재설정 요청
    public virtual void ReqEventRewardTake(PktInfoEventRewardReq _pkt) { Debug.LogError("Not Make embody - ReqEventRewardTake"); } // 유저 이벤트 보상 획득 요청
    public virtual void ReqEventLgnRewardTake(PktInfoEvtLgnRwdReq _pkt) { Debug.LogError("Not Make embody - ReqEventLgnRewardTake"); } // 유저 이벤트 로그인 보상 획득 요청
    public virtual void ReqSetTutorialVal(System.UInt32 _tutoVal) { Debug.LogError("Not Make embody - ReqSetTutorialVal"); } // 유저 튜토리얼 값 변경 요청
    public virtual void ReqSetTutorialFlag(System.UInt64 _tutoFlag) { Debug.LogError("Not Make embody - ReqSetTutorialFlag"); } // 유저 튜토리얼 플래그 변경 요청

    public virtual void ReqAddCharacter(System.UInt32 _addCharTID) { Debug.LogError("Not Make embody - ReqAddCharacter"); } // 캐릭터 생성 요청

    public virtual void ReqChangePreferenceNum(PktInfoUIDList _pkt) { Debug.LogError("Not Make embody - ReqChangePreferenceNum"); }   // 캐릭터 친밀도 버프 변경 요청
    public virtual void ReqChangeMainChar(PktInfoUIDList _pkt) { Debug.LogError("Not Make embody - ReqChangeMainChar"); }   // 메인 캐릭터 변경 요청
    public virtual void ReqGradeUpChar(System.UInt64 _cuid) { Debug.LogError("Not Make embody - ReqGradeUpChar"); }    // 캐릭터 등급업 요청
    public virtual void ReqSetMainCostumeChar(PktInfoCharSetMainCostumeReq _pkt) { Debug.LogError("Not Make embody - ReqSetMainCostumeChar"); }    // 캐릭터 코스츔 설정 요청
    public virtual void ReqRandomCostumeDyeing(System.UInt32 _costumeID) { Debug.LogError("Not Make embody - ReqRandomCostumeDyeing"); }    // 코스츔 염색 랜덤색상 요청 
    public virtual void ReqSetCostumeDyeing(PktInfoSetCostumeDyeingReq _pkt) { Debug.LogError("Not Make embody - ReqSetCostumeDyeing"); }    // 코스츔 염색 색상 결정 요청
    public virtual void ReqCostumeDyeingLock(PktInfoCostumeDyeingLock _pkt) { Debug.LogError("Not Make embody - ReqCostumeDyeingLock"); }    // 코스츔 염색 부위 잠금 요청
    public virtual void ReqUserCostumeColor(System.UInt32 _costumeID) { Debug.LogError("Not Make embody - ReqUserCostumeColor"); }    // 코스츔 염색 색상 정보 요청 
    public virtual void ReqEquipWeaponChar(PktInfoCharEquipWeapon _pkt) { Debug.LogError("Not Make embody - ReqEquipWeaponChar"); } // 캐릭터 무기 장착 요청
    public virtual void ReqApplySkillInChar(PktInfoCharSlotSkill _pkt) { Debug.LogError("Not Make embody - ReqApplySkillInChar"); } // 캐릭터 스킬 슬롯 적용 요청
    public virtual void ReqLvUpSkill(PktInfoSkillLvUpReq _pkt) { Debug.LogError("Not Make embody - ReqLvUpSkill"); }   // 캐릭터 스킬 레벨업 요청
    public virtual void ReqLvUpUserSkill(PktInfoUserSklLvUpReq _pkt) { Debug.LogError("Not Make embody - ReqLvUpUserSkill"); }   // 유저 스킬 레벨업 요청
    public virtual void ReqResetUserSkill() { Debug.LogError("Not Make embody - ReqResetUserSkill"); }   // 유저 스킬 초기화 요청
    public virtual void ReqGivePresentChar(PktInfoGivePresentCharReq _pkt) { Debug.LogError("Not Make embody - ReqGivePresentChar"); }   // 캐릭터 선물 하기 요청
    public virtual void ReqResetSecretCntChar(PktInfoUIDList _pkt) { Debug.LogError("Not Make embody - ReqResetSecretCntChar"); }   // 캐릭터 시크릿 퀘스트 초기화 요청
    public virtual void ReqRaidHPRestore(PktInfoRaidRestoreHPReq _pkt) { Debug.LogError("Not Make embody - ReqRaidHPRestore"); }   // 캐릭터 레이드 hp 회복 요청
    public virtual void ReqRaidStageDrop( PktInfoRaidStageDrop _pkt ) { Debug.LogError( "Not Make embody - ReqRaidStageDrop" ); }   // 캐릭터 레이드 hp 회복 요청
    public virtual void ReqCharLvUnexpectedPackageHardOpen(System.UInt64 _cuid) { Debug.LogError("Not Make embody - ReqCharLvUnexpectedPackageHardOpen"); } // 캐릭터 레벨 돌발패키지 강제 생성 요청

    public virtual void ReqStageStart(PktInfoStageGameStartReq _pkt) { Debug.LogError("Not Make embody - ReqStageStart"); }   // 스테이지 게임 시작 요청
    public virtual void ReqStageEnd(PktInfoStageGameResultReq _pkt) { Debug.LogError("Not Make embody - ReqStageEnd"); }    // 스테이지 게임 종료 결과 요청
    public virtual void ReqStageEndFail(PktInfoStageGameEndFail _pkt) { Debug.LogError("Not Make embody - ReqStageEndFail"); }  // 스테이지 게임 종료 실패 요청
    public virtual void ReqStageContinue() { Debug.LogError("Not Make embody - ReqStageContinue"); }    // 스테이지 이어하기 요청
    public virtual void ReqRaidStageDrop() { Debug.LogError("Not Make embody - ReqRaidStageDrop"); }    // 레이드 스테이지 포기 요청

    public virtual void ReqBookNewConfirm(PktInfoBookNewConfirm _pkt) { Debug.LogError("Not Make embody - ReqBookNewConfirm"); }  // 도감 확인 요청

    public virtual void ReqTimeAtkRankingList(PktInfoTimeAtkRankingHeader _pkt) { Debug.LogError("Not Make embody - ReqTimeAtkRankingList"); }  // 랭킹 리스트 요청
    public virtual void ReqTimeAtkRankerDetail(PktInfoTimeAtkRankerDetailReq _pkt) { Debug.LogError("Not Make embody - ReqTimeAtkRankerDetail"); }  // 랭커의 디테일 정보 요청

    public virtual void ReqArenaSeasonPlay() { Debug.LogError("Not Make embody - ReqTimeAtkRankerDetail"); }  // PVP 시즌 참가, 보상 요청
    public virtual void ReqSetArenaTeam(PktInfoUserArenaTeam _pkt) { Debug.LogError("Not Make embody - ReqTimeAtkRankerDetail"); }  // PVP 유저 팀 편성 요청
    public virtual void ReqArenaGameStart(PktInfoArenaGameStartReq _pkt) { Debug.LogError("Not Make embody - ReqTimeAtkRankerDetail"); } // PVP 대전 게임 시작 요청
    public virtual void ReqArenaGameEnd(PktInfoArenaGameEndReq _pkt) { Debug.LogError("Not Make embody - ReqTimeAtkRankerDetail"); } // PVP 대전 게임 종료 요청
    public virtual void ReqArenaEnemySearch() { Debug.LogError("Not Make embody - ReqArenaEnemySearch"); } // PVP 대전 상대 검색 요청

    public virtual void ReqArenaRankingList(System.UInt64 _updateTM) { Debug.LogError("Not Make embody - ReqArenaRankingList"); } // PVP 랭킹 리스트 요청
    public virtual void ReqArenaRankerDetail(System.UInt64 _uuid) { Debug.LogError("Not Make embody - ReqArenaRankerDetail"); } // PVP 랭킹 유저 자세한 정보 요청

    public virtual void ReqSetArenaTowerTeam(PktInfoUserArenaTeam _pkt) { Debug.LogError("Not Make embody - ReqSetArenaTowerTeam"); } // 아레나 타워 팀 편성 요청
    public virtual void ReqArenaTowerGameStart(PktInfoArenaTowerGameStartReq _pkt) { Debug.LogError("Not Make embody - ReqArenaTowerGameStart"); } // 아레나 타워 게임 시작 요청
    public virtual void ReqArenaTowerGameEnd(PktInfoArenaTowerGameEndReq _pkt) { Debug.LogError("Not Make embody - ReqArenaTowerGameEnd"); } // 아레나 타워 게임 종료 요청
    public virtual void ReqUnexpectedPackageDailyReward(PktInfoUnexpectedPackageDailyRewardReq _pkt) { Debug.LogError("Not Make embody - PktInfoUnexpectedPackageDailyRewardReq"); } // 돌발패키지 일자 보상 요청 
    public virtual void ReqGetUserPresetList(PktInfoPresetCommon _pkt) { Debug.LogError("Not Make embody - ReqGetUserPresetList"); } // 프리셋 리스트 요청
    public virtual void ReqAddOrUpdateUserPreset(PktAddOrUpdatePreset _pkt) { Debug.LogError("Not Make embody - ReqAddOrUpdateUserPreset"); } // 프리셋 등록/갱신 요청
    public virtual void ReqUserPresetLoad(PktInfoPresetCommon _pkt) { Debug.LogError("Not Make embody - ReqUserPresetLoad"); } // 프리셋 로드 요청
    public virtual void ReqUserPresetChangeName(PktInfoPresetCommon _pkt) { Debug.LogError("Not Make embody - ReqUserPresetChangeName"); } // 프리셋 이름 변경 요청

    // Raid
    public virtual void ReqInitRaidSeasonData() { Debug.LogError( "Not Make embody - ReqInitRaidSeasonData" ); }  // 레이드 시즌 초기화 요청
    public virtual void ReqRaidRankingList( PktInfoRaidRankingHeader _pkt ) { Debug.LogError( "Not Make embody - ReqRaidRankingList" ); }  // 레이드 랭킹 리스트 요청
    public virtual void ReqRaidFirstRankingList( PktInfoRaidRankingHeader _pkt ) { Debug.LogError( "Not Make embody - ReqRaidRankingList" ); }  // 레이드 랭킹 리스트 요청
    public virtual void ReqRaidRankerDetail(PktInfoRaidRankerDetailReq _pkt) { Debug.LogError("Not Make embody - ReqRaidRankerDetail"); }  // 레이드 랭킹 유저 자세한 정보 요청
    public virtual void ReqRaidFirstRankerDetail( PktInfoRaidRankerDetailReq _pkt ) { Debug.LogError( "Not Make embody - ReqRaidFirstRankerDetail" ); }  // 레이드 랭킹 유저 자세한 정보 요청
    public virtual void ReqSetRaidTeam(PktInfoUserRaidTeam _pkt) { Debug.LogError("Not Make embody - ReqSetRaidTeam"); }  // 레이드 팀 편성 요청

	// 서클관련
	public virtual void ReqCircleOpen(PktInfoCircleOpenReq _pkt) { Debug.LogError("Not Make embody - ReqCircleOpen"); }  // 서클 생성요청
	public virtual void ReqSuggestCircleList(eLANGUAGE _lang) { Debug.LogError("Not Make embody - ReqSuggestCircleList"); }  // 서클 추천리스트 요청
	public virtual void ReqCircleJoin(PktInfoCircleJoinReq _pkt) { Debug.LogError("Not Make embody - ReqCircleJoin"); }  // 서클 가입요청
	public virtual void ReqCircleJoinCancel(ulong _circleID) { Debug.LogError("Not Make embody - ReqCircleJoinCancel"); }  // 서클 가입 취소
	public virtual void ReqCircleLobbyInfo() { Debug.LogError("Not Make embody - ReqCircleLobbyInfo"); }  // 서클 로비진입 시 필요정보 요청
	public virtual void ReqCircleWithdrawal() { Debug.LogError("Not Make embody - ReqCircleWithdrawal"); }  // 가입한 서클 탈퇴 요청
	public virtual void ReqCircleDisperse() { Debug.LogError("Not Make embody - ReqCircleDisperse"); }  // 서클 해산 요청
	public virtual void ReqGetCircleUserList() { Debug.LogError("Not Make embody - ReqGetCircleUserList"); }  // 서클 가입 유저 및 가입대기 유저 리스트 요청
	public virtual void ReqCircleChangeStateJoinWaitUser(PktInfoChangeStateJoinWaitUser _pkt) { Debug.LogError("Not Make embody - ReqCircleChangeStateJoinWaitUser"); }  // 서클 가입대기 유저 상태변경 요청
	public virtual void ReqCircleUserKick(ulong _targetUuid) { Debug.LogError("Not Make embody - ReqCircleUserKick"); }  // 서클 유저 강퇴요청
	public virtual void ReqCircleChangeAuthLevel(PktInfoCircleChangeAuthority _pkt) { Debug.LogError("Not Make embody - ReqCircleChangeAuthLevel"); }  // 서클 유저 권한변경 요청
	public virtual void ReqCircleChangeMark(PktInfoCircleMarkSet _pkt) { Debug.LogError("Not Make embody - ReqCircleChangeMark"); }  // 서클 마크변경 요청
	public virtual void ReqCircleChangeName(PktInfoCircleChangeName _pkt) { Debug.LogError("Not Make embody - ReqCircleChangeName"); }  // 서클 이름변경 요청
	public virtual void ReqCircleChangeComment(PktInfoStr _pkt) { Debug.LogError("Not Make embody - ReqCircleChangeComment"); }  // 서클 알림말 변경 요청
	public virtual void ReqCircleChangeMainLanguage(eLANGUAGE _lang) { Debug.LogError("Not Make embody - ReqCircleChangeMainLanguage"); }  // 서클 주사용 언어 변경 요청
	public virtual void ReqCircleChangeSuggestAnotherLangOpt(bool _state) { Debug.LogError("Not Make embody - ReqCircleChangeSuggestAnotherLangOpt"); }  // 서클 주사용 언어 외 가입허용 옵션 변경
	public virtual void ReqCircleAttendance() { Debug.LogError("Not Make embody - ReqCircleAttendance"); }  // 서클 출석체크 요청
	public virtual void ReqCircleBuyMarkItem(uint _tid) { Debug.LogError("Not Make embody - ReqCircleBuyMarkItem"); }  // 서클 마크아이템 구매요청(테이블 아이디)
	public virtual void ReqCircleChatSend(PktInfoCircleChat.Piece _pkt) { Debug.LogError("Not Make embody - ReqCircleChatSend"); }  // 서클 채팅전송 요청
	public virtual void ReqCircleGetMarkList() { Debug.LogError("Not Make embody - ReqCircleGetMarkList"); }  // 서클 소유 마크 아이템 리스트 요청
	public virtual void ReqCircleSearch(PktInfoCircleSearch _pkt) { Debug.LogError("Not Make embody - ReqCircleSearch"); }  // 서클 검색 요청
	public virtual void ReqCircleChatList() { Debug.LogError("Not Make embody - ReqCircleChatList"); }  // 서클 채팅 리스트 요청 ( 로그인 후 한번만 호출할 수 있도록 권장)

}
public class PacketGlobal : PacketGlobalBase
{
    
    public override void DoAttach(NetClient _netClient)
    {
        _netClient.AttachProxy(m_C2SProxy);
        _netClient.AttachStub(m_S2CStub);

        __DoRegistRecv();
        return;
    }

    public override void ReqPing() {
        m_C2SProxy.ReqPing(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqLogOnCreditKey() {
        m_C2SProxy.ReqLogOnCreditKey(HostID.HostID_Server, RmiContext.SecureReliableSend, NETStatic.creditKey);
    }
    public override void ReqLogOut() {
        m_C2SProxy.ReqLogOut(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqRefrashUserInfo()
    {
        m_C2SProxy.ReqRefrashUserInfo(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqGetTotlaRelocateCntToNotComplete() {
        m_C2SProxy.ReqGetTotlaRelocateCntToNotComplete(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqRelocateUserInfoSet(PktInfoRelocateUser _pkt)
    {
        m_C2SProxy.ReqRelocateUserInfoSet(HostID.HostID_Server, RmiContext.SecureReliableSend, _pkt);
    }
    public override void ReqRelocateUserComplate(PktInfoRelocateUser _pkt)
    {
        m_C2SProxy.ReqRelocateUserComplate(HostID.HostID_Server, RmiContext.SecureReliableSend, _pkt);
    }
    public override void ReqReConnectUserInfo(PktInfoReconnectUserInfoReq _pkt) 
    {
        m_C2SProxy.ReqReConnectUserInfo(HostID.HostID_Server, RmiContext.SecureReliableSend, _pkt);
    }
    public override void ReqAccountCode() {
        m_C2SProxy.ReqAccountCode(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqAccountSetPassword(PktInfoStr _pkt) {
        m_C2SProxy.ReqAccountSetPassword(HostID.HostID_Server, RmiContext.SecureReliableSend, _pkt);
    }
    public override void ReqAccountCodeReward()
    {
        m_C2SProxy.ReqAccountCodeReward(HostID.HostID_Server, RmiContext.SecureReliableSend);
    }
    public override void ReqAccountLinkReward()
    {
        m_C2SProxy.ReqAccountLinkReward(HostID.HostID_Server, RmiContext.SecureReliableSend);
    }
    public override void ReqLinkAccountList() {
        m_C2SProxy.ReqLinkAccountList(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqAddLinkAccountAuth(PktInfoAccountLinkInfo _pkt)
    {
        m_C2SProxy.ReqAddLinkAccountAuth(HostID.HostID_Server, RmiContext.SecureReliableSend, _pkt);
    }
	public override void ReqAccountDelete()
	{
		m_C2SProxy.ReqAccountDelete(HostID.HostID_Server, RmiContext.SecureReliableSend);
	}
	public override void ReqGetUserInfoFromAccountLink(PktInfoUserInfoFromLinkReq _pkt) {
        m_C2SProxy.ReqGetUserInfoFromAccountLink(HostID.HostID_Server, RmiContext.SecureReliableSend, _pkt);
    }
    public override void ReqPushNotifiTokenSet(PktInfoPushNotiSetToken _pkt) {
        m_C2SProxy.ReqPushNotifiTokenSet(HostID.HostID_Server, RmiContext.SecureReliableSend, _pkt);
    }

    public override void ReqReflashLoginBonus() {
        m_C2SProxy.ReqReflashLoginBonus(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqRewardTakeAchieve(PktInfoTIDList _pkt) {
        m_C2SProxy.ReqRewardTakeAchieve(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqRewardTakeAchieveEvent(PktInfoTIDList _pkt)
    {
        m_C2SProxy.ReqRewardTakeAchieveEvent(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqRewardDailyMission(PktInfoRwdDailyMissionReq _pkt) {
        m_C2SProxy.ReqRewardDailyMission(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqRewardWeekMission(PktInfoComRwd _pkt) {
        m_C2SProxy.ReqRewardWeekMission(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqRewardInfluMission(PktInfoComRwd _pkt) {
        m_C2SProxy.ReqRewardInfluMission(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqUpdateGllaMission(PktInfoUpdateGllaMission _pkt) {
        m_C2SProxy.ReqUpdateGllaMission(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqRewardGllaMission(PktInfoRwdGllaMission _pkt) {
        m_C2SProxy.ReqRewardGllaMission(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqRewardPassMission(PktInfoRwdPassMission _pkt) {
        m_C2SProxy.ReqRewardPassMission(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqRewardPass(PktInfoRwdPassReq _pkt) {
        m_C2SProxy.ReqRewardPass(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqEventRewardReset(System.UInt32 _eventID) {
        m_C2SProxy.ReqEventRewardReset(HostID.HostID_Server, RmiContext.ReliableSend, _eventID);
    }
    public override void ReqEventRewardTake(PktInfoEventRewardReq _pkt) {
        m_C2SProxy.ReqEventRewardTake(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqEventLgnRewardTake(PktInfoEvtLgnRwdReq _pkt) {
        m_C2SProxy.ReqEventLgnRewardTake(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqSetTutorialVal(System.UInt32 _tutoVal) {
        m_C2SProxy.ReqSetTutorialVal(HostID.HostID_Server, RmiContext.ReliableSend, _tutoVal);
    }
    public override void ReqSetTutorialFlag(System.UInt64 _tutoFlag) {
        m_C2SProxy.ReqSetTutorialFlag(HostID.HostID_Server, RmiContext.ReliableSend, _tutoFlag);
    }

    public override void ReqAddCharacter(System.UInt32 _addCharTID) {
        m_C2SProxy.ReqAddCharacter(HostID.HostID_Server, RmiContext.ReliableSend, _addCharTID);
    }
    public override void ReqChangePreferenceNum(PktInfoUIDList _pkt) {
        m_C2SProxy.ReqChangePreferenceNum(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqChangeMainChar(PktInfoUIDList _pkt) {
        m_C2SProxy.ReqChangeMainChar(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    } 
    public override void ReqGradeUpChar(System.UInt64 _cuid) {
        m_C2SProxy.ReqGradeUpChar(HostID.HostID_Server, RmiContext.ReliableSend, _cuid);
    }
    public override void ReqSetMainCostumeChar(PktInfoCharSetMainCostumeReq _pkt) {
        m_C2SProxy.ReqSetMainCostumeChar(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqRandomCostumeDyeing(System.UInt32 _costumeID)
    {
        m_C2SProxy.ReqRandomCostumeDyeing(HostID.HostID_Server, RmiContext.ReliableSend, _costumeID);
    }
    public override void ReqSetCostumeDyeing(PktInfoSetCostumeDyeingReq _pkt)
    {
        m_C2SProxy.ReqSetCostumeDyeing(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqCostumeDyeingLock(PktInfoCostumeDyeingLock _pkt)
    {
        m_C2SProxy.ReqCostumeDyeingLock(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqUserCostumeColor(System.UInt32 _costumeID)
    {
        m_C2SProxy.ReqUserCostumeColor(HostID.HostID_Server, RmiContext.ReliableSend, _costumeID);
    }
    public override void ReqEquipWeaponChar(PktInfoCharEquipWeapon _pkt) {
        m_C2SProxy.ReqEquipWeaponChar(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqApplySkillInChar(PktInfoCharSlotSkill _pkt) {
        m_C2SProxy.ReqApplySkillInChar(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqLvUpSkill(PktInfoSkillLvUpReq _pkt) {
        m_C2SProxy.ReqLvUpSkill(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqLvUpUserSkill(PktInfoUserSklLvUpReq _pkt) {
        m_C2SProxy.ReqLvUpUserSkill(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqResetUserSkill() {
        m_C2SProxy.ReqResetUserSkill(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqGivePresentChar(PktInfoGivePresentCharReq _pkt) {
        m_C2SProxy.ReqGivePresentChar(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqResetSecretCntChar(PktInfoUIDList _pkt) {
        m_C2SProxy.ReqResetSecretCntChar(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqRaidHPRestore(PktInfoRaidRestoreHPReq _pkt) {
        m_C2SProxy.ReqRaidHPRestore(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqRaidStageDrop( PktInfoRaidStageDrop _pkt ) {
        m_C2SProxy.ReqRaidStageDrop( HostID.HostID_Server, RmiContext.ReliableSend, _pkt );
    }
    public override void ReqCharLvUnexpectedPackageHardOpen(System.UInt64 _cuid) {
        m_C2SProxy.ReqCharLvUnexpectedPackageHardOpen(HostID.HostID_Server, RmiContext.ReliableSend, _cuid);
    }

    public override void ReqStageStart(PktInfoStageGameStartReq _pkt) {
        m_C2SProxy.ReqStageStart(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqStageEnd(PktInfoStageGameResultReq _pkt) {
        m_C2SProxy.ReqStageEnd(HostID.HostID_Server, RmiContext.SecureReliableSend, _pkt);
    }
    public override void ReqStageEndFail(PktInfoStageGameEndFail _pkt) {
        m_C2SProxy.ReqStageEndFail(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqStageContinue() {
        m_C2SProxy.ReqStageContinue(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqBookNewConfirm(PktInfoBookNewConfirm _pkt) {
        m_C2SProxy.ReqBookNewConfirm(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqTimeAtkRankingList(PktInfoTimeAtkRankingHeader _pkt) {
        m_C2SProxy.ReqTimeAtkRankingList(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqTimeAtkRankerDetail(PktInfoTimeAtkRankerDetailReq _pkt) {
        m_C2SProxy.ReqTimeAtkRankerDetail(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqArenaSeasonPlay() {
        m_C2SProxy.ReqArenaSeasonPlay(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void ReqSetArenaTeam(PktInfoUserArenaTeam _pkt) {
        m_C2SProxy.ReqSetArenaTeam(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqArenaGameStart(PktInfoArenaGameStartReq _pkt) {
        m_C2SProxy.ReqArenaGameStart(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqArenaGameEnd(PktInfoArenaGameEndReq _pkt) {
        m_C2SProxy.ReqArenaGameEnd(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqArenaEnemySearch() {
        m_C2SProxy.ReqArenaEnemySearch(HostID.HostID_Server, RmiContext.ReliableSend);
    }
    public override void  ReqArenaRankingList(System.UInt64 _updateTM) {
        m_C2SProxy.ReqArenaRankingList(HostID.HostID_Server, RmiContext.ReliableSend, _updateTM);
    }
    public override void ReqArenaRankerDetail(System.UInt64 _uuid) {
        m_C2SProxy.ReqArenaRankerDetail(HostID.HostID_Server, RmiContext.ReliableSend, _uuid);
    }
    public override void ReqSetArenaTowerTeam(PktInfoUserArenaTeam _pkt) {
        m_C2SProxy.ReqSetArenaTowerTeam(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqArenaTowerGameStart(PktInfoArenaTowerGameStartReq _pkt) {
        m_C2SProxy.ReqArenaTowerGameStart(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqArenaTowerGameEnd(PktInfoArenaTowerGameEndReq _pkt) {
        m_C2SProxy.ReqArenaTowerGameEnd(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqUnexpectedPackageDailyReward(PktInfoUnexpectedPackageDailyRewardReq _pkt)
    {
        m_C2SProxy.ReqUnexpectedPackageDailyReward(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqGetUserPresetList(PktInfoPresetCommon _pkt)
    {
        m_C2SProxy.ReqGetUserPresetList(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqAddOrUpdateUserPreset(PktAddOrUpdatePreset _pkt)
    {
        m_C2SProxy.ReqAddOrUpdateUserPreset(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqUserPresetLoad(PktInfoPresetCommon _pkt)
    {
        m_C2SProxy.ReqUserPresetLoad(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }
    public override void ReqUserPresetChangeName(PktInfoPresetCommon _pkt)
    {
        m_C2SProxy.ReqUserPresetChangeName(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqInitRaidSeasonData() {
        m_C2SProxy.ReqInitRaidSeasonData( HostID.HostID_Server, RmiContext.ReliableSend );
    }

    public override void ReqRaidRankingList( PktInfoRaidRankingHeader _pkt ) {
        m_C2SProxy.ReqRaidRankingList( HostID.HostID_Server, RmiContext.ReliableSend, _pkt );
    }

    public override void ReqRaidFirstRankingList( PktInfoRaidRankingHeader _pkt ) {
        m_C2SProxy.ReqRaidFirstRankingList( HostID.HostID_Server, RmiContext.ReliableSend, _pkt );
    }

    public override void ReqRaidRankerDetail(PktInfoRaidRankerDetailReq _pkt)
    {
        m_C2SProxy.ReqRaidRankerDetail(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqRaidFirstRankerDetail( PktInfoRaidRankerDetailReq _pkt ) {
        m_C2SProxy.ReqRaidFirstRankerDetail( HostID.HostID_Server, RmiContext.ReliableSend, _pkt );
    }

    public override void ReqSetRaidTeam(PktInfoUserRaidTeam _pkt)
    {
        m_C2SProxy.ReqSetRaidTeam(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqCircleOpen(PktInfoCircleOpenReq _pkt)
    {
        m_C2SProxy.ReqCircleOpen(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqSuggestCircleList(eLANGUAGE _lang)
    {
        m_C2SProxy.ReqSuggestCircleList(HostID.HostID_Server, RmiContext.ReliableSend, _lang);
    }

    public override void ReqCircleJoin(PktInfoCircleJoinReq _pkt)
    {
        m_C2SProxy.ReqCircleJoin(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqCircleJoinCancel(ulong _circleID)
    {
        m_C2SProxy.ReqCircleJoinCancel(HostID.HostID_Server, RmiContext.ReliableSend, _circleID);
    }

    public override void ReqCircleLobbyInfo()
    {
        m_C2SProxy.ReqCircleLobbyInfo(HostID.HostID_Server, RmiContext.ReliableSend);
    }

    public override void ReqCircleWithdrawal()
    {
        m_C2SProxy.ReqCircleWithdrawal(HostID.HostID_Server, RmiContext.ReliableSend);
    }

    public override void ReqCircleDisperse()
    {
        m_C2SProxy.ReqCircleDisperse(HostID.HostID_Server, RmiContext.ReliableSend);
    }

    public override void ReqGetCircleUserList()
    {
        m_C2SProxy.ReqGetCircleUserList(HostID.HostID_Server, RmiContext.ReliableSend);
    }

    public override void ReqCircleChangeStateJoinWaitUser(PktInfoChangeStateJoinWaitUser _pkt)
    {
        m_C2SProxy.ReqCircleChangeStateJoinWaitUser(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqCircleUserKick(ulong _targetUuid)
    {
        m_C2SProxy.ReqCircleUserKick(HostID.HostID_Server, RmiContext.ReliableSend, _targetUuid);
    }

    public override void ReqCircleChangeAuthLevel(PktInfoCircleChangeAuthority _pkt)
    {
        m_C2SProxy.ReqCircleChangeAuthLevel(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqCircleChangeMark(PktInfoCircleMarkSet _pkt)
    {
        m_C2SProxy.ReqCircleChangeMark(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqCircleChangeName(PktInfoCircleChangeName _pkt)
    {
        m_C2SProxy.ReqCircleChangeName(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqCircleChangeComment(PktInfoStr _pkt)
    {
        m_C2SProxy.ReqCircleChangeComment(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqCircleChangeMainLanguage(eLANGUAGE _lang)
    {
        m_C2SProxy.ReqCircleChangeMainLanguage(HostID.HostID_Server, RmiContext.ReliableSend, _lang);
    }

    public override void ReqCircleChangeSuggestAnotherLangOpt(bool _state)
    {
        m_C2SProxy.ReqCircleChangeSuggestAnotherLangOpt(HostID.HostID_Server, RmiContext.ReliableSend, _state);
    }

    public override void ReqCircleAttendance()
    {
        m_C2SProxy.ReqCircleAttendance(HostID.HostID_Server, RmiContext.ReliableSend);
    }

    public override void ReqCircleBuyMarkItem(uint _tid)
    {
        m_C2SProxy.ReqCircleBuyMarkItem(HostID.HostID_Server, RmiContext.ReliableSend, _tid);
    }

    public override void ReqCircleChatSend(PktInfoCircleChat.Piece _pkt)
    {
        m_C2SProxy.ReqCircleChatSend(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqCircleGetMarkList()
    {
        m_C2SProxy.ReqCircleGetMarkList(HostID.HostID_Server, RmiContext.ReliableSend);
    }

    public override void ReqCircleSearch(PktInfoCircleSearch _pkt)
    {
        m_C2SProxy.ReqCircleSearch(HostID.HostID_Server, RmiContext.ReliableSend, _pkt);
    }

    public override void ReqCircleChatList()
    {
        m_C2SProxy.ReqCircleChatList(HostID.HostID_Server, RmiContext.ReliableSend);
    }

    void __DoRegistRecv()
    {
        m_S2CStub.NotiMoveUserInSvrToSvr    = __NotiMoveUserInSvrToSvr;
        m_S2CStub.AckLogOnCreditKey         = __AckLogOnCreditKey;
        m_S2CStub.AckLogOut                 = __AckLogOut;
    }

    bool __NotiMoveUserInSvrToSvr(HostID remote, RmiContext rmiContext, System.UInt64 _errNum, System.Guid _creditKey, PktInfoSimpleSvr _tgtSvrInfo)
    {
        Platforms.IBase.Inst.DoNotiMoveUserInSvrToSvr(_errNum, _creditKey, _tgtSvrInfo);

        if (eTMIErrNum.SUCCESS_OK != (eTMIErrNum)_errNum)
            return false;

        NetConnectInfo info;
        info.ipAddr_ = _tgtSvrInfo.addr_;
        info.port_ = _tgtSvrInfo.startPort_;

        NETStatic.DoSetCreditKey(_creditKey);
        NETStatic.Mgr.DoAllInOneConnect((eServerType)_tgtSvrInfo.svrTP_, ref info);

        return true;
    }

    bool __AckLogOnCreditKey(HostID remote, RmiContext rmiContext, System.UInt64 _errNum)
    {
        Platforms.IBase.Inst.DoLogAckLogOnCreditKey(_errNum);

        if (eTMIErrNum.SUCCESS_OK != (eTMIErrNum)_errNum)
            NETStatic.Mgr.DoAllInOneConnect(eServerType.LOGIN);

        NETStatic.DoClearCreditKey();
        return true;
    }
    bool __AckLogOut(HostID remote, RmiContext rmiContext)
    {
        Platforms.IBase.Inst.DoTestAckLogOut();
        return true;
    }
    
}