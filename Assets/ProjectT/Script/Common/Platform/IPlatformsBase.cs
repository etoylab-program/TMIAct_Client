//using System;
using System.Net.NetworkInformation;
using System.IO;
using UnityEngine;

/// <summary>
/// Platforms 관련
/// </summary>
namespace Platforms
{
    public enum Kind
    {
        AOS = 0,
        IOS,
        WINOS,

        _MAX_
    }

    static public class Factory
    {
        static public void CreateStart(Kind kind = Kind._MAX_)
        {
            // 두번 생성할 필요가 없어서 이미 생성된 상태면 처리하지 않습니다.
            if (null != Platforms.IBase.Inst)
                return;

            // 아래 구문으로 호출 할수 있게 하는 것이 좋으나 아직 관련 처리가 안됀 상태여서 Application.platform 값에 따라서 동작하도록 우선 처리함.
            //Platforms.Factory.Create(kind);

            if (Application.platform == RuntimePlatform.Android)
                Platforms.Factory.Create(Platforms.Kind.AOS);
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                Platforms.Factory.Create(Platforms.Kind.IOS);
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
                Platforms.Factory.Create(Platforms.Kind.WINOS);
            else
                Platforms.Factory.Create(Platforms.Kind._MAX_);
        }

        static IBase Create(Kind kind)
        {
            Debug.Log(string.Format("===== Platform.Create(Kind = {0}) =====", kind));
            IBase obj = null;
            switch (kind)
            {
                case Kind.AOS: { obj = new PlatformsAos(); } break;
                case Kind.IOS: { obj = new PlatformsIos(); } break;
                case Kind.WINOS: { obj = new PlatformsWinos(); } break;
                default: { obj = new PlatformsDefault(); } break;
            }
            obj.Init();
            return obj;
        }
    }

    /// <summary>
    /// Platform 별 기능 처리를 담당할 관련 인터페이스 클래스입니다.
    /// </summary>
    public class IBase
    {
        static IBase s_Inst = null;
        static public IBase Inst { get { return s_Inst; } }

        public delegate void CB_Param1(object p1);
        public delegate void CB_Param2(object p1, object p2);

        public void Init()
        {
            s_Inst = this;

            _DoInit();
        }

        public string GetMACAddr()
        {
            string MAC = "";

            NetworkInterface[] netInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            if(netInterfaces.Length > 0)
            {
                PhysicalAddress addr = netInterfaces[0].GetPhysicalAddress();

                byte[] bytes = addr.GetAddressBytes();
                for (int j = 0; j < bytes.Length; j++)
                {
                    MAC += bytes[j].ToString("X2");
                }

                /*for (int i = 0; i < netInterfaces.Length; i++)
                {
                    PhysicalAddress addr = netInterfaces[i].GetPhysicalAddress();

                    byte[] bytes = addr.GetAddressBytes();
                    for(int j = 0; j < bytes.Length; j++)
                    {
                        MAC += bytes[j].ToString("X2");
                    }
                }*/
            }

            return MAC;
        }

        public virtual System.String GetDeviceUniqueID() { return SystemInfo.deviceUniqueIdentifier; }

        public virtual void DoLogNotiCommonErr(System.UInt64 _errNum) {; }
        public virtual void DoLogAckErrLoginUserBlock(PktInfoTime _blockTime) {; }
        public virtual void DoNotiMoveUserInSvrToSvr(System.UInt64 _errNum, System.Guid _creditKey, PktInfoSimpleSvr _pktInfo) {; }
        public virtual void DoTestNotiUpdateTicket(PktInfoUpdateTicketUserNoti _pktInfo) {; }
        public virtual void DoTestNotiAddMail(PktInfoMail _pktInfo) {; }
        public virtual void DoTestNotiResetWeekMission(PktInfoMission.Weekly _pktInfo) {; }
        public virtual void DoTestNotiUpdateWeekMission(PktInfoMission.Weekly _pktInfo) {; }
        public virtual void DoTestNotiUserEventChange(PktInfoEventChangeList.Piece _pktInfo) {; }
        public virtual void DoTestAckLogOut() {; }
        public virtual void DoLogAckLogOnCreditKey(System.UInt64 _errNum) {; }
        public virtual void DoLogAckBookNewConfirm(PktInfoBookNewConfirm _pktInfo) {; }

        protected virtual void _DoInit() {; }
    }

    ///// <summary>
    ///// Service를 담당 업체 관련 공통 구현을 위한 클래스입니다.
    ///// </summary>
    //public class Common : IBase
    //{
    //    protected CB_Param1 m_CB_Param1;
    //    protected CB_Param2 m_CB_Param2;


    //    sealed override protected void _DoInit()
    //    {
    //        _DoInit_Ex();
    //    }

    //    protected void _DoCallBack_Param1(object p1)
    //    {
    //        if (null == m_CB_Param1)    return ;

    //        m_CB_Param1(p1);
    //    }
    //    protected void _DoCallBack_Param2(object p1, object p2)
    //    {
    //        if (null == m_CB_Param2)    return ;

    //        m_CB_Param2(p1, p2);
    //    }

    //    protected virtual void _DoInit_Ex()
    //    {
    //        ;
    //    }
    //}
}