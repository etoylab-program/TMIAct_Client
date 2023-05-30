/// <summary>
/// Platform 관련 디폴트 관련 작업 구현 클래스
/// </summary>
namespace Platforms
{
    public class PlatformsDefault : IBase
    {
        public override void DoLogNotiCommonErr(System.UInt64 _errNum)
        {
            Debug.LogError(string.Format("Server -> NotiCommonErr() eTMIErrNum[{0}]", _errNum));
        }
        public override void DoLogAckErrLoginUserBlock(PktInfoTime _blockTime)
        {
            System.DateTime endTime = _blockTime.GetTime();
            System.String time = endTime.ToLongDateString() + endTime.ToLongTimeString();
            Debug.LogWarning(string.Format("Server -> AckErrLoginUserBlock() blockEndTime:[{0}]", time));
        }
        public override void DoNotiMoveUserInSvrToSvr(System.UInt64 _errNum, System.Guid _creditKey, PktInfoSimpleSvr _pktInfo)
        {
            Debug.Log(string.Format("Server -> NotiMoveUserInSvrToSvr() Err[{0}] key:[{1}]", _errNum, _creditKey.ToString()));
        }
        public override void DoTestNotiUpdateTicket(PktInfoUpdateTicketUserNoti _pktInfo)
        {
            System.DateTime nowDateTime = System.DateTime.Now;
            System.DateTime nextDateTime = _pktInfo.nextTime_.GetTime();
            System.String nowTime = nowDateTime.ToLongDateString() + nowDateTime.ToLongTimeString();
            System.String nextTime = nextDateTime.ToLongDateString() + nextDateTime.ToLongTimeString();
            Debug.LogWarning(string.Format("Server -> NotiUpdateTicket() Ticket[{0}] NowTime:[{1}] NextTime:[{2}]", _pktInfo.resultTicket_, nowTime, nextTime));
        }
        public override void DoTestNotiAddMail(PktInfoMail _pktInfo) {
            Debug.LogWarning(string.Format("Server -> NotiAddMail() MaxCnt[{0}]", _pktInfo.maxCnt_));
        }
        public override void DoTestNotiResetWeekMission(PktInfoMission.Weekly _pktInfo) {
            Debug.LogWarning(string.Format("Server -> NotiResetWeekMission() {0}", _pktInfo.GetStr()));
        }
        public override void DoTestNotiUpdateWeekMission(PktInfoMission.Weekly _pktInfo) {
            Debug.LogWarning(string.Format("Server -> NotiUpdateWeekMission() {0}", _pktInfo.GetStr()));
        }
        public override void DoTestNotiUserEventChange(PktInfoEventChangeList.Piece _pktInfo) {
            Debug.LogWarning(string.Format("Server -> DoTestNotiUserEventChange() {0}", _pktInfo.GetStr()));
        }
        public override void DoLogAckLogOnCreditKey(System.UInt64 _errNum)
        {
            if (eTMIErrNum.SUCCESS_OK != (eTMIErrNum)_errNum)
                Debug.Log(string.Format("Server -> AckLogOnCreditKey() Err[{0}]", _errNum));
            else
                Debug.Log(string.Format("Server -> AckLogOnCreditKey() OK"));
        }
        public override void DoTestAckLogOut()
        {
            NETStatic.Mgr.DoRelease(eConnectKind.DISCONNECT_LOGOUT);
            AppMgr.Instance.LoadScene(AppMgr.eSceneType.Title, "NetworkTest");
        }
        public override void DoLogAckBookNewConfirm(PktInfoBookNewConfirm _pktInfo)
        {
            Debug.Log(string.Format("Server -> AckBookNewConfirm() PosKind[{0}] BookTID:[{1}]", _pktInfo.bookGroup_, _pktInfo.bookTID_));
        }
    }
}