
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nettention.Proud;

public partial class GameInfo : FMonoSingleton<GameInfo>
{
    //--------------------------------------------------------------------------------------------------------------
    //
    //  Send
    //
    //--------------------------------------------------------------------------------------------------------------
    // login
    public void Send_ReqLogin(OnReceiveCallBack ReceiveCallBack)
    {
        ClearProtocolData();
        SetCallBackList("Send_ReqLogin", ReceiveCallBack);

        PlayerPrefs.DeleteKey( "RaidPlayer1_CUID" );
        PlayerPrefs.DeleteKey( "RaidPlayer2_CUID" );
        PlayerPrefs.DeleteKey( "RaidPlayer3_CUID" );

        if (_netflag)
        {
            string strUniqueIdentifier = string.Empty;
#if DISABLESTEAMWORKS
            strUniqueIdentifier = Platforms.IBase.Inst.GetDeviceUniqueID();
#else // 스팀에선 디바이스 ID를 스팀아이디로 설정
            strUniqueIdentifier = string.Format("{0}_Steam", AppMgr.Instance.SteamId);
#endif

            if (AppMgr.Instance.configData.m_NetworkTest)
            {
                _accountUUID = AppMgr.Instance.configData.m_TestAccountUUID;
                strUniqueIdentifier = AppMgr.Instance.configData.m_TestDeviceUniqueIdentifier;
            }
            else
            {
                if (IsAccount())
                {
                    _accountUUID = long.Parse(PlayerPrefs.GetString("User_AccountUUID"));
                }
                else
                {
                    _accountUUID = 0;
                    GameSupport.SendFireBaseLogEvent(eFireBaseLogType._Login);
                }
            }

            PktInfoAuthLogin pktInfo = new PktInfoAuthLogin();
            pktInfo.ver_ = new PktInfoVersion();
            pktInfo.accountID_ = new PktInfoStr();

            pktInfo.ver_.verMain_ = (System.UInt32)AppMgr.Instance.configData.m_serverversionmain;
            pktInfo.ver_.verSub_ = (System.UInt32)AppMgr.Instance.configData.m_serverversionsub;

            pktInfo.accountID_.str_ = strUniqueIdentifier;
            pktInfo.uuid_ = (System.UInt64)_accountUUID;

            //국가 코드는 현재 시스템 언어로
            pktInfo.country_ = new PktInfoStr();
            
            //LIAPP 관련
            pktInfo.secuKind_ = eSecurityKind._NONE_;

#if UNITY_ANDROID || UNITY_IOS
#if !UNITY_EDITOR
            pktInfo.secuKind_ = eSecurityKind._NONE_;
#endif
#endif

            //현재 클라이언트 언어
            pktInfo.lang_ = new PktInfoStr();
            if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
            {
                pktInfo.lang_.str_ = eLANGUAGE.JPN.ToString();
            }
            else //if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Global)
            {
                pktInfo.lang_.str_ = FLocalizeString.Language.ToString();
            }

            // 현재 국가 코드
            pktInfo.country_.str_ = System.Globalization.RegionInfo.CurrentRegion.Name;//pktInfo.lang_.str_;

            if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
            {
                pktInfo.platform_ = ePlatformKind.STEAM;
            }
            else
            {
                pktInfo.platform_ = (ePlatformKind)((int)AppMgr.Instance.configData.ResLoadPlatformType + 1);
            }

            StartTimeOutConnect();

            NETStatic.PktLgn.ReqAccountAuthLogin(pktInfo);
            Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin);
        }
        else
        {
            NetLocalSvr.Instance.Proc_Login(ReceiveCallBack);
        }
    }

    //보안 정보얻기 요청
    public void Send_ReqClientSecurityInfo(OnReceiveCallBack ReceiveCallBack)
    {
        //if(!_netflag)
        //{
        //    ReceiveCallBack(0, null);
        //    return;
        //}

        PktInfoClientSecurityReq pktinfo = new PktInfoClientSecurityReq();

        pktinfo.secuKind_ = eSecurityKind._NONE_;
#if UNITY_ANDROID
        pktinfo.secuKind_ = eSecurityKind._NONE_;
#elif UNITY_IOS
#if LIAPP_FOR_DISTRIBUTE
            pktinfo.secuKind_ = eSecurityKind._NONE_;    
#else
            pktinfo.secuKind_ = eSecurityKind._NONE_;
#endif
#endif

        Log.Show("Send_ReqClientSecurityInfo : " + pktinfo.secuKind_);

        AddProtocolData(ePacketType.Login, LoginC2S.Common.ReqClientSecurityInfo, pktinfo, ReceiveCallBack);
        SendProtocolData();
    }

    // 클라이언트 보안 검증 요청 및 서버 정보 얻기
    public void Send_ReqClientSecurityVerify(string token, OnReceiveCallBack ReceiveCallBack)
    {
        //if (!_netflag)
        //{
        //    ReceiveCallBack(0, null);
        //    return;
        //}

        PktInfoClientSecurityVerifyReq pktinfo = new PktInfoClientSecurityVerifyReq();
        pktinfo.token_ = new PktInfoStr();
        pktinfo.token_.str_ = token;
#if UNITY_ANDROID
        pktinfo.secuKind_ = eSecurityKind._NONE_;
#elif UNITY_IOS
        pktinfo.secuKind_ = eSecurityKind._NONE_;
#else
    #if !DISABLESTEAMWORKS
            pktinfo.secuKind_ = eSecurityKind.STEAM_VAC;
            pktinfo.secuUserID_ = AppMgr.Instance.SteamId;
    #endif
#endif

        Log.Show("Send_ReqClientSecurityVerify : " + token);

        AddProtocolData(ePacketType.Login, LoginC2S.Common.ReqClientSecurityVerify, pktinfo, ReceiveCallBack);
        SendProtocolData();
    }

    //--------------------------------------------------------------------------------------------------------------
    // Global
    //--------------------------------------------------------------------------------------------------------------
    public void Send_ReqAddLinkAccountAuth(eAccountType type, string linkid, OnReceiveCallBack ReceiveCallBack)
    {
        if (!_netflag)
            return;

        //CurAccountType = type;
        PktInfoAccountLinkInfo pktinfo = new PktInfoAccountLinkInfo();
        pktinfo.linkID_ = new PktInfoStr();
        pktinfo.type_ = type;
        pktinfo.linkID_.str_ = linkid;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqAddLinkAccountAuth, pktinfo, ReceiveCallBack);
        SendProtocolData();
    }
    public void Send_ReqAccountCode(OnReceiveCallBack ReceiveCallBack)
    {
        if (!_netflag)
        {
            ReceiveCallBack(0, null);
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqAccountCode, null, ReceiveCallBack);
        SendProtocolData();
    }
    public void Send_ReqAccountSetPassword(string password, OnReceiveCallBack ReceiveCallBack)
    {
        if (!_netflag)
            return;

        PktInfoStr pktinfo = new PktInfoStr();
        pktinfo.str_ = password;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqAccountSetPassword, pktinfo, ReceiveCallBack);
        SendProtocolData();
    }
    public void Send_ReqLinkAccountList(OnReceiveCallBack ReceiveCallBack)
    {
        if (!_netflag)
        {
            ReceiveCallBack(0, null);
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqLinkAccountList, null, ReceiveCallBack);
        SendProtocolData();
    }

    //Title Only
    public void Send_ReqGetUserInfoFromAccountLink(eAccountType type, string linkid, string password, OnReceiveCallBack ReceiveCallBack)
    {
        if (!_netflag)
            return;
        SetCallBackList("Send_ReqGetUserInfoFromAccountLink", ReceiveCallBack);

        string strUniqueIdentifier = string.Empty;
#if DISABLESTEAMWORKS
        strUniqueIdentifier = Platforms.IBase.Inst.GetDeviceUniqueID();
#else // 스팀에선 디바이스 ID를 스팀아이디로 설정
        strUniqueIdentifier = string.Format("{0}_Steam", AppMgr.Instance.SteamId);
#endif

        PktInfoUserInfoFromLinkReq pktinfo = new PktInfoUserInfoFromLinkReq();

        pktinfo.linkID_ = new PktInfoStr();
        pktinfo.password_ = new PktInfoStr();
        pktinfo.accountID_ = new PktInfoStr();

        pktinfo.type_ = type;
        pktinfo.linkID_.str_ = linkid;
        pktinfo.password_.str_ = password;
        pktinfo.accountID_.str_ = strUniqueIdentifier;
        NETStatic.PktGbl.ReqGetUserInfoFromAccountLink(pktinfo);

        UsedAccountTypeInGetLink = type;
        Debug.Log(string.Format("Send_ReqGetUserInfoFromAccountLink : {0}", type.ToString()));

        Firebase.Analytics.FirebaseAnalytics.LogEvent("GetUserInfoFromAccountLink", "AccountType", type.ToString());
    }


    public void Send_ReqReConnectUserInfo(OnReceiveCallBack ReceiveCallBack)
    {
        SetCallBackList("Send_ReqReConnectUserInfo", ReceiveCallBack);
        if (_netflag)
        {
            string strUniqueIdentifier = Platforms.IBase.Inst.GetDeviceUniqueID();
            if (AppMgr.Instance.configData.m_NetworkTest)
                strUniqueIdentifier = AppMgr.Instance.configData.m_TestDeviceUniqueIdentifier;

            PktInfoReconnectUserInfoReq ptkInfo = new PktInfoReconnectUserInfoReq();
            ptkInfo.ver_ = new PktInfoVersion();
            ptkInfo.accountID_ = new PktInfoStr();
            ptkInfo.ver_.verMain_ = (System.UInt32)AppMgr.Instance.configData.m_serverversionmain;
            ptkInfo.ver_.verSub_ = (System.UInt32)AppMgr.Instance.configData.m_serverversionsub;
            ptkInfo.accountID_.str_ = strUniqueIdentifier;
            ptkInfo.uuid_ = (UInt64)GameInfo.Instance.UserData.UUID;

            NETStatic.PktGbl.ReqReConnectUserInfo(ptkInfo);
        }
        else
        {

        }
    }

    public void Send_ReqAddCharacter(int tableid, OnReceiveCallBack ReceiveCallBack)
    {
        if (_netflag)
        {
            if (_bconnect == false)
            {
                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3131), OnMsg_ToTitle, true, true);//타이틀로
                return;
            }
        }

        if (!CheckConnect())
            return;

        WaitPopup.Show();
        SetCallBackList("Send_ReqAddCharacter", ReceiveCallBack);

        if (_netflag)
        {
            NETStatic.PktGbl.ReqAddCharacter((byte)tableid);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("AddCharacter", "CharTableID", tableid);
        }
        else
        {
            NetLocalSvr.Instance.AddChar(tableid);
            _userdata.MainCharUID = _charlist[0].CUID;
            ReceiveCallBack?.Invoke(0, null);
        }
        /*
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqAddCharacter, tableid, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
        {
            NetLocalSvr.Instance.AddChar(tableid);
            _userdata.MainCharUID = _charlist[0].CUID;
            ReceiveCallBack?.Invoke(0, null);
        }
        */
    }

    public void Send_ReqPing()
    {
        if (_netflag)
        {
            NETStatic.PktGbl.ReqPing();
        }
        else
        {

        }
    }

    public void Send_ReqRewardTakeAchieve(List<int> list, OnReceiveCallBack ReceiveCallBack)// 유저 공적 보상 획득 요청
    {
        if (IsProtocolData())
            return;

        PktInfoTIDList ptkInfo = new PktInfoTIDList();
        ptkInfo.tids_ = new List<System.UInt32>();
        for (int i = 0; i < list.Count; i++)
            ptkInfo.tids_.Add((uint)list[i]);
        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqRewardTakeAchieve, ptkInfo, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqRewardTakeAchieve(list, ReceiveCallBack);
    }

    public void Send_ReqUserMarkList(OnReceiveCallBack ReceiveCallBack) // 지휘관 마크 목록 요청
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqUserMarkList, null, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqUserMarkList(ReceiveCallBack);
    }

    public void Send_ReqUserSetMark(int mark, OnReceiveCallBack ReceiveCallBack)// 지휘관 대표 이미지 설정 요청
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqUserSetMark, mark, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqUserSetMark(mark, ReceiveCallBack);
    }

    public void Send_ReqUserSetName(string nickname, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoStr pktStr = new PktInfoStr();
        pktStr.str_ = nickname;
        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqUserSetName, pktStr, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqUserSetName(nickname, ReceiveCallBack);

    }

    public void Send_ReqUserSetNameColor( int colorId, OnReceiveCallBack ReceiveCallBack ) {
        if( IsProtocolData() ) {
            return;
        }

        AddProtocolData( ePacketType.Lobby, LobbyC2S.Common.ReqUserSetNameColor, colorId, ReceiveCallBack );

        if( !CheckConnect() ) {
            return;
        }

        if( _netflag ) {
            SendProtocolData();
        }
    }

    public void Send_ReqUserSetCommentMsg(string word, OnReceiveCallBack ReceiveCallBack)  // 지휘관 인사말 변경 요청
    {
        if (IsProtocolData())
            return;

        PktInfoStr pktStr = new PktInfoStr();
        pktStr.str_ = word;
        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqUserSetCommentMsg, pktStr, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqUserSetCommentMsg(word, ReceiveCallBack);
    }

    public void Send_ReqUserPkgShowOff(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqUserPkgShowOff, null, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    // addCount는 5의 배수
    public void Send_ReqAddItemSlot(int addCount, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoAddSlot pktInfoAddSlot = new PktInfoAddSlot();
        pktInfoAddSlot.nowSlotCnt_ = (ushort)addCount;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqAddItemSlot, pktInfoAddSlot, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqAddItemSlot(ReceiveCallBack);
    }
    
    public void Send_ReqGivePresentChar(long charUid, ItemData[] itemDatas, OnReceiveCallBack receiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        PktInfoGivePresentCharReq pktData = new PktInfoGivePresentCharReq
        {
            cuid_ = (ulong)charUid
        };
        
        pktData.maters_ = new PktInfoItemCnt
        {
            infos_ = new List<PktInfoItemCnt.Piece>()
        };

        foreach (ItemData itemData in itemDatas)
        {
            pktData.maters_.infos_.Add(new PktInfoItemCnt.Piece
            {
                uid_ = (ulong)itemData.ItemUID,
                cnt_ = (ushort)itemData.Count,
            });
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqGivePresentChar, pktData, receiveCallBack);
        
        if (!CheckConnect())
        {
            return;
        }
        
        if (_netflag)
        {
            SendProtocolData();
        }
    }
    
    //--------------------------------------------------------------------------------------------------------------
    // Global
    //--------------------------------------------------------------------------------------------------------------
    //캐릭터
    public void Send_ReqChangeMainChar(long charuid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        int changeSlotIndex = -1;
        for (int i = 0; i < _userdata.ArrLobbyBgCharUid.Length; i++)
        {
            if(_userdata.ArrLobbyBgCharUid[i] == charuid)
            {
                changeSlotIndex = i;
                break;
            }
        }

        PktInfoUIDList pkt = new PktInfoUIDList();

        pkt.uids_ = new List<ulong>();
        pkt.uids_.Add((ulong)charuid);

        for (int i = 1; i < _userdata.ArrLobbyBgCharUid.Length; i++)
        {
            if (changeSlotIndex == i)
            {
                pkt.uids_.Add((ulong)_userdata.ArrLobbyBgCharUid[0]);
            }
            else
            {
                pkt.uids_.Add((ulong)_userdata.ArrLobbyBgCharUid[i]);
            }
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqChangeMainChar, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_CharMain(charuid, ReceiveCallBack);
    }

    public void Send_ReqChangeLobbyBgChar(long[] arrBgChar, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        PktInfoUIDList pkt = new PktInfoUIDList();
        pkt.uids_ = new List<ulong>();

        for (int i = 0; i < arrBgChar.Length; i++)
        {
            pkt.uids_.Add((ulong)arrBgChar[i]);
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqChangeMainChar, pkt, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    public void Send_ReqGradeUpChar(long charuid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqGradeUpChar, charuid, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_CharGardeUpRequest(charuid, ReceiveCallBack);
    }

    public void Send_ReqSetMainCostumeChar(long charuid, int costumeid, int costumecolor, int costumeflag, bool bWait, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;
        
        PktInfoCharSetMainCostumeReq pkt = new PktInfoCharSetMainCostumeReq();
        pkt.skinColor_ = new PktInfoCharSkinColor();
        pkt.skinColor_.cuid_ = (UInt64)charuid;
        pkt.skinColor_.costumeClr_ = (Byte)costumecolor;
        pkt.skinColor_.skinStateFlag_ = (UInt32)costumeflag;
        pkt.costumeTID_ = (UInt32)costumeid;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqSetMainCostumeChar, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_CharCostumeEquip(charuid, costumeid, costumecolor, costumeflag, ReceiveCallBack);
    }
    
    public void Send_ReqUserCostumeColor(long costumeId, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;
        
        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqUserCostumeColor, costumeId, callback);
        
        if (_netflag)
            SendProtocolData();
    }
    
    public void Send_ReqRandomCostumeDyeing(long costumeId, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;
        
        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqRandomCostumeDyeing, costumeId, callback);
        
        if (!CheckConnect())
            return;
        
        if (_netflag)
            SendProtocolData();
    }

    public void Send_ReqSetCostumeDyeing(int[] selectList, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        PktInfoSetCostumeDyeingReq pkt = new PktInfoSetCostumeDyeingReq();
        pkt.infos_ = new List<PktInfoSetCostumeDyeingReq.Piece>();
        
        for (byte i = 0; i < selectList.Length; i++)
        {
            if (selectList[i] < 0)
                continue;

            PktInfoSetCostumeDyeingReq.Piece piece = new PktInfoSetCostumeDyeingReq.Piece();
            piece.partIndex_ = i;
            piece.colorIndex_ = (byte)selectList[i];
            
            pkt.infos_.Add(piece);
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqSetCostumeDyeing, pkt, callback);
        
        if (!CheckConnect())
            return;
        
        if (_netflag)
            SendProtocolData();
    }

    public void Send_ReqCostumeDyeingLock(int costumeTableId, byte lockFlag, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;
        
        PktInfoCostumeDyeingLock pkt = new PktInfoCostumeDyeingLock();
        pkt.costumeID_ = (uint)costumeTableId;
        pkt.lockFlag_ = lockFlag;
        
        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCostumeDyeingLock, pkt, callback);
        
        if (!CheckConnect())
            return;
        
        if (_netflag)
            SendProtocolData();
    }
    
    public void Send_ReqEquipWeaponChar(long charuid, int costumeColor, int skinstateFlag, long mainWeaponUID, long subWeaponUID, int mainSkinTID, int subskinTID, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoCharEquipWeapon pkt = new PktInfoCharEquipWeapon();
        pkt.skinColor_ = new PktInfoCharSkinColor();
        pkt.skinColor_.cuid_ = (UInt64)charuid;
        pkt.skinColor_.costumeClr_ = (Byte)costumeColor;
        pkt.skinColor_.skinStateFlag_ = (UInt32)skinstateFlag;

        pkt.wpns_[(int)eWeaponSlot.MAIN] = new PktInfoChar.WpnSlot();
        pkt.wpns_[(int)eWeaponSlot.MAIN].wpnUID_ = (UInt64)mainWeaponUID;
        pkt.wpns_[(int)eWeaponSlot.MAIN].wpnSkin_ = (UInt32)mainSkinTID;

        pkt.wpns_[(int)eWeaponSlot.SUB] = new PktInfoChar.WpnSlot();
        pkt.wpns_[(int)eWeaponSlot.SUB].wpnUID_ = (UInt64)subWeaponUID;
        pkt.wpns_[(int)eWeaponSlot.SUB].wpnSkin_ = (UInt32)subskinTID;
        Log.Show(mainWeaponUID + " / " + mainSkinTID + " / " + subWeaponUID + " / " + subskinTID);
        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqEquipWeaponChar, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_WeaponCharEquip(charuid, mainWeaponUID, subWeaponUID, mainSkinTID, subskinTID, ReceiveCallBack);
    }

    public void Send_ReqLvUpSkill(long charuid, int skillid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        int tutoValue = 0;
        if (GameSupport.IsTutorial())
        {
            tutoValue = GameInfo.Instance.UserData.TutorialNum;
            Firebase.Analytics.FirebaseAnalytics.LogEvent("TutorialStep", "StepIndex", tutoValue);
        }

        PktInfoSkillLvUpReq pktInfo = new PktInfoSkillLvUpReq();
        pktInfo.cuid_ = (UInt64)charuid;
        pktInfo.skillTID_ = (uint)skillid;
        pktInfo.tutoVal_ = (uint)tutoValue;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqLvUpSkill, pktInfo, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_CharSkillPassiveLevelUp(charuid, skillid, ReceiveCallBack);
    }
    public void Send_ReqApplySkillInChar(long charuid, int[] skills, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        int tutoValue = 0;
        if (GameSupport.IsTutorial())
        {
            tutoValue = GameInfo.Instance.UserData.TutorialNum;
            Firebase.Analytics.FirebaseAnalytics.LogEvent("TutorialStep", "StepIndex", tutoValue);
        }
        var pkt = new PktInfoCharSlotSkill();
        var chardata = GetCharData(charuid);
        pkt.cuid_ = (UInt64)charuid;
        pkt.tutoVal_ = (uint)tutoValue;
        for (int i = 0; i < (int)PktInfoChar.SkillSlot._MAX_; i++)
            pkt.skilTIDs_[i] = (uint)chardata.EquipSkill[i];

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqApplySkillInChar, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_CharSkillUpdate(charuid, skills, ReceiveCallBack);
    }

    // 게임 시작
    public void Send_ReqStageStart(int stageid, long charuid, int _multipleindex, bool bFastQuestTicket, bool bMultiGCFlag, List<long> subCharCuidList, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        IsReqStageEndReConnect = false;
        SelectMultipleIndex = _multipleindex;
        bFastStageClear = false;
        var cleardata = _stageclearlist.Find(x => x.TableID == stageid);
        if (cleardata == null)
            bFastStageClear = true;

        FSaveData.Instance.SaveStageData(charuid, stageid, _multipleindex, bMultiGCFlag);
        UIValue.Instance.SetValue(UIValue.EParamType.LastPlayStageID, stageid);
        SeleteCharUID = charuid;
        SelecteStageTableId = stageid;

        CharData chardata = GetCharData(charuid);

        PktInfoStageGameStartReq pkt = new PktInfoStageGameStartReq();
        pkt.secretSubCuids_ = new PktInfoUIDList();
        pkt.secretSubCuids_.uids_ = new List<ulong>();
        if (subCharCuidList != null) {
            for (int i = 0; i < subCharCuidList.Count; i++) {
                pkt.secretSubCuids_.uids_.Add((ulong)subCharCuidList[i]);
            }
        }
        pkt.stageTID_ = (UInt32)stageid;
        pkt.cuid_ = (UInt64)charuid;
        pkt.useItemUIDs_ = new List<ulong>();
        pkt.ticketMultipleIdx_ = (byte)_multipleindex;
        if (true == GameSupport.IsActiveUserBuff(eBuffEffectType.Buff_SkillSlot))
            pkt.DoOnOpt(PktInfoStageGameStartReq.OPT.SK_BUF);
        if (true == bFastQuestTicket)
            pkt.DoOnOpt(PktInfoStageGameStartReq.OPT.FAST_QUEST);
        if (true == bMultiGCFlag)
            pkt.DoOnOpt(PktInfoStageGameStartReq.OPT.MULTI_AP);

        GameInfo.Instance.MultibleGCFlag = bMultiGCFlag;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqStageStart, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqStageStart(stageid, charuid, ReceiveCallBack);
    }

    public void Send_ReqStageEnd(int stageid, long charuid, int clearTime, int goldCnt, int box1, bool mission1, bool mission2, bool mission3, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        int tutoValue = 0;
        if (GameSupport.IsTutorial())
        {
            if (stageid == 1)
                GameInfo.Instance.UserData.SetTutorial((int)eTutorialState.TUTORIAL_STATE_CardEquip, 1);
            else if (stageid == 2)
                GameInfo.Instance.UserData.SetTutorial((int)eTutorialState.TUTORIAL_STATE_SkillSpt, 1);
            else if (stageid == 3)
                GameInfo.Instance.UserData.SetTutorial((int)eTutorialState.TUTORIAL_STATE_Gacha, 1);

            tutoValue = GameInfo.Instance.UserData.TutorialNum;
        }
        PktInfoStageGameResultReq pkt = new PktInfoStageGameResultReq();
		pkt.playData_ = new PktInfoIngamePlayData();
        pkt.playData_.clearTime_Ms = (UInt32)clearTime;
		pkt.dropGoldItemCnt_ = (ushort)goldCnt;
        pkt.nTakeBoxCnt_ = (byte)box1;
        pkt.mission0_ = mission1;
        pkt.mission1_ = mission2;
        pkt.mission2_ = mission3;
        pkt.tutoVal_ = (UInt32)tutoValue;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqStageEnd, pkt, ReceiveCallBack);

        if (!CheckConnect())
        {
            IsReqStageEndReConnect = true;
            return;
        }

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqStageEnd(stageid, charuid, clearTime, goldCnt, box1, mission1, mission2, mission3, ReceiveCallBack);
    }

    public void Send_ReqStageEndFail(int clearTime, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        bStageFailure = true;
        PktInfoStageGameEndFail pkt = new PktInfoStageGameEndFail();
        pkt.playTime_Ms_ = (UInt32)clearTime;
        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqStageEndFail, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData(false);
        else
            NetLocalSvr.Instance.Proc_ReqStageEndFail(clearTime, ReceiveCallBack);
    }
    public void Send_ReqTimeAtkRankingList(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoTimeAtkRankingHeader pkt = new PktInfoTimeAtkRankingHeader();
        pkt.infos_ = new List<PktInfoTimeAtkRankingHeader.Piece>();

        if (_timeattackranklist.Count == 0)
        {
            var list = _gametable.FindAllStage(x => x.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK && x.TypeValue > 0);
            for (int i = 0; i < list.Count; i++)
            {
                var piece = new PktInfoTimeAtkRankingHeader.Piece();
                piece.stageID_ = (UInt32)list[i].ID;
                piece.updateTM_ = (UInt64)0;
                pkt.infos_.Add(piece);
            }
        }
        else
        {
            for (int i = 0; i < _timeattackranklist.Count; i++)
            {
                var piece = new PktInfoTimeAtkRankingHeader.Piece();
                piece.stageID_ = (UInt32)_timeattackranklist[i].TableID;
                piece.updateTM_ = (UInt64)_timeattackranklist[i].UpdateTM;
                pkt.infos_.Add(piece);
            }
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqTimeAtkRankingList, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqTimeAtkRankingList(ReceiveCallBack);
    }
    public void Send_ReqTimeAtkRankerDetail(int stageid, long uuid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoTimeAtkRankerDetailReq pkt = new PktInfoTimeAtkRankerDetailReq();
        pkt.stageID_ = (UInt32)stageid;
        pkt.uuid_ = (UInt64)uuid;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqTimeAtkRankerDetail, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqTimeAtkRankerDetail(ReceiveCallBack);

    }
    //--------------------------------------------------------------------------------------------------------------
    // Product
    //--------------------------------------------------------------------------------------------------------------
    //아이템
    public void Send_ReqSellItemList(long uid, int count, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoItemSell();
        pkt.infos_ = new List<PktInfoItemSell.Piece>();
        var itemsell = new PktInfoItemSell.Piece();
        itemsell.itemUID_ = (UInt64)uid;
        itemsell.sellCnt_ = (UInt32)count;
        pkt.infos_.Add(itemsell);


        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqSellItem, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqSellItemList(uid, count, ReceiveCallBack);
    }

	public void Send_ReqUseItem( long uid, int count, long value, OnReceiveCallBack receiveCallBack ) {
		if ( IsProtocolData() ) {
			return;
		}

		bool isGoods = false;
		ItemData itemData = ItemList.Find( x => x.ItemUID == uid );
		if ( itemData != null ) {
			switch ( (eITEMSUBTYPE)itemData.TableData.SubType ) {
				case eITEMSUBTYPE.USE_AP_CHARGE:
				case eITEMSUBTYPE.USE_BP_CHARGE:
				case eITEMSUBTYPE.USE_AP_CHARGE_NUM:
				case eITEMSUBTYPE.USE_BP_CHARGE_NUM: {
					isGoods = true;
				}
				break;

				default: {

				}
				break;
			}
		}

		if ( isGoods ) {
			PktInfoItemCnt pktInfoItemCnt = new PktInfoItemCnt();
			pktInfoItemCnt.infos_ = new List<PktInfoItemCnt.Piece>() {
                new PktInfoItemCnt.Piece() {
                    uid_ = (ulong)uid,
                    cnt_ = (ushort)count,
                },
            };

			AddProtocolData( ePacketType.Product, ProductC2S.Common.ReqUseItemGoods, pktInfoItemCnt, receiveCallBack );
		}
		else {
			PktInfoUseItemReq pkt = new PktInfoUseItemReq() {
                item_ = new PktInfoItemCnt.Piece() {
                    uid_ = (ulong)uid,
					cnt_ = (ushort)count,
				},
				value1_ = (ulong)value,
			};

			AddProtocolData( ePacketType.Product, ProductC2S.Common.ReqUseItem, pkt, receiveCallBack );
		}

		if ( !CheckConnect() ) {
			return;
		}

		if ( _netflag ) {
			SendProtocolData();
		}
	}

	public void Send_ReqUseItemForArray( RecoverySelectData[] datas, OnReceiveCallBack receiveCallBack ) {
		if ( IsProtocolData() ) {
			return;
		}

		PktInfoItemCnt pktInfoItemCnt = new PktInfoItemCnt();
        pktInfoItemCnt.infos_ = new List<PktInfoItemCnt.Piece>();

        for ( int i = 0; i < datas.Length; i++) {
			pktInfoItemCnt.infos_.Add(new PktInfoItemCnt.Piece() {
                uid_ = (ulong)datas[i].UID,
                cnt_ = (ushort)datas[i].Count,
            } );
		}

		AddProtocolData( ePacketType.Product, ProductC2S.Common.ReqUseItemGoods, pktInfoItemCnt, receiveCallBack );

		if ( !CheckConnect() ) {
			return;
		}

		if ( _netflag ) {
			SendProtocolData();
		}
	}

	//무기
	public void Send_ReqSellWeaponList(List<long> uidlist, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoWeaponSell();
        pkt.infos_ = new List<PktInfoWeaponSell.Piece>();
        for (int i = 0; i < uidlist.Count; i++)
        {
            var data = new PktInfoWeaponSell.Piece();
            data.weaponUID_ = (UInt64)uidlist[i];
            pkt.infos_.Add(data);
        }

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqSellWeapon, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_WeaponSell(uidlist, ReceiveCallBack);
    }
    public void Send_ReqSetLockWeaponList(List<long> weaponuidlist, List<bool> locklist, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoWeaponLock();
        pkt.infos_ = new List<PktInfoWeaponLock.Piece>();
        for (int i = 0; i < weaponuidlist.Count; i++)
        {
            var data = new PktInfoWeaponLock.Piece();
            data.weaponUID_ = (UInt64)weaponuidlist[i];
            data.lock_ = (Boolean)locklist[i];
            pkt.infos_.Add(data);
        }
        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqSetLockWeapon, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_WeaponLock(weaponuidlist, locklist, ReceiveCallBack);
    }
    public void Send_ReqLvUpWeapon(long weaponuid, bool bmatitem, List<long> matlist, List<MatItemData> matitemlist, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoProductComGrowReq();
        pkt.targetUID_ = (UInt64)weaponuid;
        pkt.maters_ = new PktInfoUIDList();
        pkt.maters_.uids_ = new List<System.UInt64>();
        for (int i = 0; i < matlist.Count; i++)
            pkt.maters_.uids_.Add((UInt64)matlist[i]);
        pkt.materItems_ = new PktInfoItemCnt();
        pkt.materItems_.infos_ = new List<PktInfoItemCnt.Piece>();
        for (int i = 0; i < matitemlist.Count; i++)
        {
            var addItem = new PktInfoItemCnt.Piece();
            addItem.uid_ = (System.UInt64)matitemlist[i].ItemData.ItemUID;
            addItem.cnt_ = (System.UInt16)matitemlist[i].Count;
            pkt.materItems_.infos_.Add(addItem);
        }

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqLvUpWeapon, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqLvUpWeapon(weaponuid, bmatitem, matlist, matitemlist, ReceiveCallBack);
    }
    public void Send_ReqWakeWeapon(long weaponuid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoProductComGrowReq();
        pkt.materItems_ = new PktInfoItemCnt();
        pkt.materItems_.infos_ = new List<PktInfoItemCnt.Piece>();
        pkt.maters_ = new PktInfoUIDList();
        pkt.maters_.uids_ = new List<System.UInt64>();
        pkt.targetUID_ = (UInt64)weaponuid;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqWakeWeapon, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqWakeWeapon(weaponuid, ReceiveCallBack);
    }

    public void Send_ReqSkillLvUpWeapon(long weaponuid, int mattype, long matuid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoProductComGrowReq();
        pkt.materItems_ = new PktInfoItemCnt();
        pkt.materItems_.infos_ = new List<PktInfoItemCnt.Piece>();
        pkt.maters_ = new PktInfoUIDList();
        pkt.maters_.uids_ = new List<System.UInt64>();
        pkt.targetUID_ = (UInt64)weaponuid;
        if (mattype == (int)MatSkillData.eTYPE.ITEM)
        {
            var addItem = new PktInfoItemCnt.Piece();
            addItem.uid_ = (System.UInt64)matuid;
            addItem.cnt_ = (System.UInt16)1;
            pkt.materItems_.infos_.Add(addItem);
        }
        else
        {
            pkt.maters_.uids_.Add((UInt64)matuid);
        }

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqSkillLvUpWeapon, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqSkillLvUpWeapon(weaponuid, mattype, matuid, ReceiveCallBack);
    }


    public void Send_ReqApplyGemInWeapon(long weaponuid, long[] gemlist, int[] slotlist, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var weapondata = GetWeaponData(weaponuid);
        var pkt = new PktInfoWeaponSlotGem();
        pkt.weaponUID_ = (UInt64)weaponuid;
        for (int i = 0; i < (int)PktInfoWeapon.Slot.ENUM._MAX_; i++)
            pkt.gemUIDs_[i] = (UInt64)weapondata.SlotGemUID[i];

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqApplyGemInWeapon, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_GemWeaponEquip(weaponuid, gemlist, slotlist, ReceiveCallBack);
    }
    //곡옥
    public void Send_ReqSellGemList(List<long> uidlist, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoGemSell();
        pkt.infos_ = new List<PktInfoGemSell.Piece>();
        for (int i = 0; i < uidlist.Count; i++)
        {
            var data = new PktInfoGemSell.Piece();
            data.gemUID_ = (UInt64)uidlist[i];
            pkt.infos_.Add(data);
        }

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqSellGem, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqSellGemList(uidlist, ReceiveCallBack);
    }


    public void Send_ReqSetLockGemList(List<long> gemuidlist, List<bool> locklist, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoGemLock();
        pkt.infos_ = new List<PktInfoGemLock.Piece>();
        for (int i = 0; i < gemuidlist.Count; i++)
        {
            var data = new PktInfoGemLock.Piece();
            data.gemUID_ = (UInt64)gemuidlist[i];
            data.lock_ = (Boolean)locklist[i];
            pkt.infos_.Add(data);
        }

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqSetLockGem, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqSetLockGemList(gemuidlist, locklist, ReceiveCallBack);
    }

    public void Send_ReqLvUpGem(long gemuid, List<long> matlist, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoProductComGrowReq();
        pkt.targetUID_ = (UInt64)gemuid;
        pkt.materItems_ = new PktInfoItemCnt();
        pkt.materItems_.infos_ = new List<PktInfoItemCnt.Piece>();
        pkt.maters_ = new PktInfoUIDList();
        pkt.maters_.uids_ = new List<System.UInt64>();
        for (int i = 0; i < matlist.Count; i++)
            pkt.maters_.uids_.Add((UInt64)matlist[i]);

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqLvUpGem, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqLvUpGem(gemuid, matlist, ReceiveCallBack);
    }

    public void Send_ReqWakeGem(long gemuid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoProductComGrowReq();
        pkt.maters_ = new PktInfoUIDList();
        pkt.maters_.uids_ = new List<System.UInt64>();
        pkt.materItems_ = new PktInfoItemCnt();
        pkt.materItems_.infos_ = new List<PktInfoItemCnt.Piece>();
        pkt.targetUID_ = (UInt64)gemuid;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqWakeGem, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqWakeGem(gemuid, ReceiveCallBack);
    }

    public void Send_ReqEvolutionGem(long gemuid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoProductComGrowReq();
        pkt.maters_ = new PktInfoUIDList();
        pkt.maters_.uids_ = new List<System.UInt64>();
        pkt.materItems_ = new PktInfoItemCnt();
        pkt.materItems_.infos_ = new List<PktInfoItemCnt.Piece>();
        pkt.targetUID_ = (UInt64)gemuid;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqEvolutionGem, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqWakeGem(gemuid, ReceiveCallBack);
    }

    public void Send_ReqAnalyzeGem(long gemuid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoProductComGrowReq();
        pkt.maters_ = new PktInfoUIDList();
        pkt.maters_.uids_ = new List<System.UInt64>();
        pkt.materItems_ = new PktInfoItemCnt();
        pkt.materItems_.infos_ = new List<PktInfoItemCnt.Piece>();
        pkt.targetUID_ = (UInt64)gemuid;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqAnalyzeGem, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqWakeGem(gemuid, ReceiveCallBack);
    }

    public void Send_ReqResetOptGem(long gemuid, int slot, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoGemResetOptReq pkt = new PktInfoGemResetOptReq();
        pkt.gemUID_ = (UInt64)gemuid;
        pkt.slotIdx_ = (Byte)slot;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqResetOptGem, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqResetOptGem(gemuid, slot, ReceiveCallBack);
    }

    public void Send_ReqResetOptSelectGem(long gemuid, bool bnew, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoGemResetOptSelect pkt = new PktInfoGemResetOptSelect();
        pkt.gemUID_ = (UInt64)gemuid;
        pkt.newFlag_ = (Boolean)bnew;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqResetOptSelectGem, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqResetOptSelectGem(gemuid, bnew, ReceiveCallBack);
    }


    //카드
    public void Send_ReqApplyPosCard(long carduid, long charuid, int slot, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        int tutoValue = 0;
        if (GameSupport.IsTutorial())
        {
            tutoValue = GameInfo.Instance.UserData.TutorialNum;
        }
        PktInfoCardApplyPos pkt = new PktInfoCardApplyPos();
        pkt.cardUID_ = (UInt64)carduid;
        pkt.posValue_ = (UInt64)charuid;
        pkt.posKind_ = eContentsPosKind.CHAR;
        pkt.slotNum_ = (Byte)slot;
        pkt.tutoVal_ = (UInt32)tutoValue;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqApplyPosCard, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_CardCharEquip(carduid, charuid, slot, ReceiveCallBack);
    }

    public void Send_ReqApplyOutPosCard(long carduid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqApplyOutPosCard, carduid, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_CardCharRemove(carduid, ReceiveCallBack);
    }

    public void Send_ReqSellCardList(List<long> uidlist, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoCardSell();
        pkt.infos_ = new List<PktInfoCardSell.Piece>();
        for (int i = 0; i < uidlist.Count; i++)
        {
            var data = new PktInfoCardSell.Piece();
            data.cardUID_ = (UInt64)uidlist[i];
            pkt.infos_.Add(data);
        }
        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqSellCard, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_CardSell(uidlist, ReceiveCallBack);
    }

    public void Send_ReqSetLockCardList(List<long> carduidlist, List<bool> locklist, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoCardLock();
        pkt.infos_ = new List<PktInfoCardLock.Piece>();
        for (int i = 0; i < carduidlist.Count; i++)
        {
            var data = new PktInfoCardLock.Piece();
            data.cardUID_ = (UInt64)carduidlist[i];
            data.lock_ = (Boolean)locklist[i];
            pkt.infos_.Add(data);
        }

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqSetLockCard, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_CardLock(carduidlist, locklist, ReceiveCallBack);
    }

    public void Send_ReqLvUpCard(long carduid, bool bmatitem, List<long> matlist, List<MatItemData> matitemlist, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        int tutoValue = 0;
        if (GameSupport.IsTutorial())
        {
            tutoValue = GameInfo.Instance.UserData.TutorialNum;
        }
        var pkt = new PktInfoProductComGrowReq();
        pkt.targetUID_ = (UInt64)carduid;
        pkt.tutoVal_ = (UInt32)tutoValue;
        pkt.maters_ = new PktInfoUIDList();
        pkt.maters_.uids_ = new List<System.UInt64>();
        for (int i = 0; i < matlist.Count; i++)
            pkt.maters_.uids_.Add((UInt64)matlist[i]);
        pkt.materItems_ = new PktInfoItemCnt();
        pkt.materItems_.infos_ = new List<PktInfoItemCnt.Piece>();
        for (int i = 0; i < matitemlist.Count; i++)
        {
            var addItem = new PktInfoItemCnt.Piece();
            addItem.uid_ = (System.UInt64)matitemlist[i].ItemData.ItemUID;
            addItem.cnt_ = (System.UInt16)matitemlist[i].Count;
            pkt.materItems_.infos_.Add(addItem);
        }

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqLvUpCard, pkt, ReceiveCallBack);


        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqLvUpCard(carduid, bmatitem, matlist, matitemlist, ReceiveCallBack);
    }

    public void Send_ReqWakeCard(long carduid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoProductComGrowReq();
        pkt.targetUID_ = (UInt64)carduid;
        pkt.maters_ = new PktInfoUIDList();
        pkt.maters_.uids_ = new List<System.UInt64>();
        pkt.materItems_ = new PktInfoItemCnt();
        pkt.materItems_.infos_ = new List<PktInfoItemCnt.Piece>();

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqWakeCard, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqWakeCard(carduid, ReceiveCallBack);
    }

    public void Send_ReqSkillLvUpCard(long carduid, int mattype, long matuid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoProductComGrowReq();
        pkt.targetUID_ = (UInt64)carduid;
        pkt.materItems_ = new PktInfoItemCnt();
        pkt.materItems_.infos_ = new List<PktInfoItemCnt.Piece>();
        pkt.maters_ = new PktInfoUIDList();
        pkt.maters_.uids_ = new List<System.UInt64>();
        if (mattype == (int)MatSkillData.eTYPE.ITEM)
        {
            var addItem = new PktInfoItemCnt.Piece();
            addItem.uid_ = (System.UInt64)matuid;
            addItem.cnt_ = (System.UInt16)1;
            pkt.materItems_.infos_.Add(addItem);
        }
        else
        {
            pkt.maters_.uids_.Add((UInt64)matuid);
        }

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqSkillLvUpCard, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqSkillLvUpCard(carduid, mattype, matuid, ReceiveCallBack);
    }

    public void Send_ReqFavorLvRewardCard(int tableid, int level, List<byte> favorList, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoBookOnStateReq pkt = new PktInfoBookOnStateReq();
        pkt.idxs_ = new PktInfoComRwd();
        pkt.idxs_.idxs_ = new PktInfoUInt8List();
        pkt.idxs_.idxs_.vals_ = new List<byte>();
        for (int i = 0; i < favorList.Count; i++)
            pkt.idxs_.idxs_.vals_.Add(favorList[i]);
        pkt.tid_ = (UInt32)tableid;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqFavorLvRewardCard, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqFavorLvRewardCard(tableid, level, ReceiveCallBack); // 서버 패킷에는 level 필요없지만 해당 로컬 함수 때문에 남겨둠
    }
    public void Send_ReqBookNewConfirm(int type, int tableid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoBookNewConfirm pkt = new PktInfoBookNewConfirm();
        pkt.bookTID_ = (UInt32)tableid;
        pkt.bookGroup_ = (eBookGroup)type;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqBookNewConfirm, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqBookNewConfirm(type, tableid, ReceiveCallBack);
    }

    public void Send_ReqMailTakeProductList(List<ulong> reciveMailUIDs, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoUIDList pktInfoUIDList = new PktInfoUIDList();
        pktInfoUIDList.uids_ = new List<ulong>();
        for (int i = 0; i < reciveMailUIDs.Count; i++)
            pktInfoUIDList.uids_.Add(reciveMailUIDs[i]);
        PktInfoMailProductTakeReq pkt = new PktInfoMailProductTakeReq();
        pkt.mailIDs_ = pktInfoUIDList;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqMailTakeProductList, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqMailTakeProductList(reciveMailUIDs, ReceiveCallBack);
    }

    public void Send_ReqMailList(ulong reciveStartIDX, uint viewCount, bool isRefresh, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        int tutoValue = 0;
        if (GameSupport.IsTutorial())
        {
            GameInfo.Instance.UserData.SetTutorial((int)eTutorialState.TUTORIAL_STATE_EndTutorial, 1);
            tutoValue = GameInfo.Instance.UserData.TutorialNum;
            GameInfo.Instance.UserData.SetTutorial((int)eTutorialState.TUTORIAL_STATE_Mail, 1);
        }
        PktInfoMailListReq pkt = new PktInfoMailListReq();
        pkt.startIdx_ = (byte)reciveStartIDX;
        pkt.cnt_ = (byte)viewCount;
        pkt.tutoVal_ = (UInt32)tutoValue;
        pkt.isRefresh_ = isRefresh;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqMailList, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqMailList(reciveStartIDX, viewCount, ReceiveCallBack);
    }

    public void Send_ReqReflashLoginBonus(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqReflashLoginBonus, null, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqReflashLoginBonus(ReceiveCallBack);
    }
    public void Send_ReqRewardDailyMission(List<int> indexlist, int groupID, int day, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoRwdDailyMissionReq pkt = new PktInfoRwdDailyMissionReq();
        pkt.idxs_ = new PktInfoComRwd();
        pkt.idxs_.idxs_ = new PktInfoUInt8List();
        pkt.idxs_.idxs_.vals_ = new List<byte>();
        for (int i = 0; i < indexlist.Count; i++)
            pkt.idxs_.idxs_.vals_.Add((System.Byte)indexlist[i]);

        pkt.groupID_ = (uint)groupID;
        pkt.day_ = (byte)day;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqRewardDailyMission, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    public void Send_ReqRewardWeekMission(List<int> indexlist, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoComRwd pkt = new PktInfoComRwd();
        pkt.idxs_ = new PktInfoUInt8List();
        pkt.idxs_.vals_ = new List<byte>();
        for (int i = 0; i < indexlist.Count; i++)
            pkt.idxs_.vals_.Add((System.Byte)indexlist[i]);

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqRewardWeekMission, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqRewardWeekMission(indexlist, ReceiveCallBack);
    }

    public void Send_ReqRewardGllaMission(int groupId, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoRwdGllaMission pkt = new PktInfoRwdGllaMission();
        pkt.groupIDs_ = new PktInfoTIDList();
        pkt.groupIDs_.tids_ = new List<uint>();
        pkt.groupIDs_.tids_.Add((uint)groupId);

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqRewardGllaMission, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            Log.Show("Send_ReqRewardGllaMission은 로컬로 동작하지 않습니다.", Log.ColorType.Red);
    }

    //직접호출 - 서버에서 락?을 안걸기 때문에 GameInfoProtocol 안타고 그냥 보냄
    // -> 프로토콜 타게끔 수정
    public void Send_ReqUpdateGllaMission(OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        PktInfoUpdateGllaMission pkt = new PktInfoUpdateGllaMission();
        pkt.infos_ = new List<PktInfoUpdateGllaMission.Piece>();

        for (int i = 0; i < ServerData.GuerrillaMissionList.Count; i++)
        {
            if (ServerData.GuerrillaMissionList[i].Type != eGuerrillaMissionType.GM_MailReward.ToString())
                continue;

            if (GameSupport.IsGuerrillaMission(ServerData.GuerrillaMissionList[i], true))
            {
                PktInfoUpdateGllaMission.Piece piece = new PktInfoUpdateGllaMission.Piece();
                eGuerrillaMissionType missionType = (eGuerrillaMissionType)Enum.Parse(typeof(eGuerrillaMissionType), ServerData.GuerrillaMissionList[i].Type);

                piece.type_ = (byte)missionType;
                piece.count_ = 1;
                pkt.infos_.Add(piece);
            }
        }

        if (pkt.infos_.Count <= 0)
        {
            if (callback != null)
                callback(0, null);
            return;
        }
            

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqUpdateGllaMission, pkt, callback);

        if (!CheckConnect())
            return;

        if (_netflag)
        {
            SendProtocolData();
            //NETStatic.PktGbl.ReqUpdateGllaMission(pkt);
        }
    }

    //--------------------------------------------------------------------------------------------------------------
    public void Send_ReqFacilityOperation_Trade(int tableid, long selete, int itemcnt, List<ulong> uids, eContentsPosKind kind, int addOnTableId, OnReceiveCallBack ReceiveCallBack)   //시설 작동-교환
    {
        if (IsProtocolData())
            return;

        PktInfoFacilityOperationReq pkt = new PktInfoFacilityOperationReq();
        pkt.facilityTID_ = (uint)tableid;
        pkt.operationValue_ = (ulong)selete;
        pkt.operationCnt_ = (ushort)itemcnt;
        pkt.maters_ = new List<PktInfoFacilityOperationReq.Mater>();
        pkt.operationRoomChar_ = new PktInfoUIDList();
        pkt.operationRoomChar_.uids_ = new List<ulong>();
        pkt.tradeAddonTID_ = (uint)addOnTableId;
        
        for (int i = 0; i < uids.Count; i++)
        {
            if (uids[i] == 0) continue;

            PktInfoFacilityOperationReq.Mater m = new PktInfoFacilityOperationReq.Mater();
            m.uid_ = uids[i];
            m.kind_ = kind;
            pkt.maters_.Add(m);
        }

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqFacilityOperation, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqFacilityOperation(tableid, selete, itemcnt, ReceiveCallBack);
    }

    public void Send_ReqFacilityOperation(int tableid, long selete, int itemcnt, List<long> uids, OnReceiveCallBack ReceiveCallBack)   //시설 작동
    {
        if (IsProtocolData())
            return;

        PktInfoFacilityOperationReq pkt = new PktInfoFacilityOperationReq();
        pkt.facilityTID_ = (uint)tableid;
        pkt.operationValue_ = (ulong)selete;
        pkt.operationCnt_ = (ushort)itemcnt;
        pkt.maters_ = new List<PktInfoFacilityOperationReq.Mater>();
        pkt.operationRoomChar_ = new PktInfoUIDList();
        pkt.operationRoomChar_.uids_ = new List<ulong>();
        if (uids != null)
        {
            foreach (long uid in uids)
            {
                pkt.operationRoomChar_.uids_.Add((UInt64)uid);
            }
        }

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqFacilityOperation, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqFacilityOperation(tableid, selete, itemcnt, ReceiveCallBack);
    }

    public void Send_ReqFacilityComplete(int tableid, int speedItemTid, PktInfoFacilityOperConfirmReq.TYPE consumeType, OnReceiveCallBack ReceiveCallBack)           //시설 완료
    {
        if (IsProtocolData())
            return;

        PktInfoFacilityOperConfirmReq pkt = new PktInfoFacilityOperConfirmReq();
        pkt.facilityTID_ = (uint)tableid;
        pkt.clearOperValFlag_ = true;
        pkt.consumeVal_ = (uint)speedItemTid;           //가속장치 테이블 ID
        pkt.consumeTP_ = (byte)consumeType;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqFacilityOperationConfirm, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqFacilityComplete(tableid, speedItemTid, ReceiveCallBack);
    }
    public void Send_ReqFacilityCancel(int tableid, OnReceiveCallBack ReceiveCallBack)             //시설 취소
    {
        //중복 되는 프로토콜
        if (IsProtocolData())
            return;

        PktInfoFacilityOperConfirmReq pkt = new PktInfoFacilityOperConfirmReq();
        pkt.facilityTID_ = (uint)tableid;
        pkt.clearOperValFlag_ = true;
        pkt.consumeTP_ = (System.Byte)PktInfoFacilityOperConfirmReq.TYPE._NONE_;
        pkt.consumeVal_ = 0;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqFacilityOperationConfirm, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqFacilityCancel(tableid, ReceiveCallBack);
    }
    public void Send_ReqFacilityCardEquip(int tableid, long carduid, OnReceiveCallBack ReceiveCallBack)        //시설 서포터 장착
    {
        //중복 되는 프로토콜
        if (IsProtocolData())
            return;

        int tutoValue = 0;
        if (GameSupport.IsTutorial())
        {
            tutoValue = GameInfo.Instance.UserData.TutorialNum;
        }
        PktInfoCardApplyPos pkt = new PktInfoCardApplyPos();
        pkt.cardUID_ = (UInt64)carduid;
        pkt.posValue_ = (UInt64)tableid;
        pkt.posKind_ = eContentsPosKind.FACILITY;
        pkt.slotNum_ = (Byte)0;
        pkt.tutoVal_ = (UInt32)tutoValue;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqApplyPosCard, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqFacilityCardEquip(carduid, tableid, eContentsPosKind.FACILITY, ReceiveCallBack);
    }
    public void Send_ReqFacilityCardRemove(long carduid, OnReceiveCallBack ReceiveCallBack)                     //시설 서포터 해제
    {
        //중복 되는 프로토콜
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqApplyOutPosCard, carduid, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqFacilityCardRemove(carduid, ReceiveCallBack);
    }
    public void Send_FacilityItemEquip(int tableid, long itemid, int itemcnt, OnReceiveCallBack ReceiveCallBack)
    {
        //중복 되는 프로토콜
        if (IsProtocolData())
            return;

        PktInfoFacilityOperationReq pkt = new PktInfoFacilityOperationReq();
        pkt.facilityTID_ = (uint)tableid;
        pkt.operationValue_ = (ulong)itemid;
        pkt.operationCnt_ = (ushort)itemcnt;
        pkt.maters_ = new List<PktInfoFacilityOperationReq.Mater>();
        pkt.operationRoomChar_ = new PktInfoUIDList();
        pkt.operationRoomChar_.uids_ = new List<ulong>();

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqFacilityOperation, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_FacilityItemEquip(tableid, itemid, itemcnt, ReceiveCallBack);
    }

    public void Send_ReqFacilityUpgrade(int tableid, OnReceiveCallBack ReceiveCallBack) //시설 업그레이드
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqFacilityUpgrade, tableid, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqFacilityUpgrade(tableid, ReceiveCallBack);
    }

    public void Send_ReqStorePurchase(int storeid, bool bfree, int count, OnReceiveCallBack ReceiveCallBack)
    {
        //if (IsProtocolData())
        //    return;

        int tutoValue = 0;
        if (GameSupport.IsTutorial())
        {
            tutoValue = GameInfo.Instance.UserData.TutorialNum;
            storeid = GameInfo.Instance.GameConfig.TutorialGachaStoreID;
        }

        PktInfoStorePurchaseReq pkt = new PktInfoStorePurchaseReq();
        pkt.storeID_ = (uint)storeid;
        pkt.tutoVal_ = (UInt32)tutoValue;
        pkt.purchaseCnt_ = (byte)count;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqStorePurchase, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_Purchase(storeid, bfree, ReceiveCallBack);
    }

    public void Send_ReqStorePurchaseInApp(string receipt, int storeid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

#if !DISABLESTEAMWORKS
        ReceiveCallBack = GetCallBackList("Send_ReqSteamPurchase");
#endif

        PktInfoStorePurchaseInAppReq pkt = new PktInfoStorePurchaseInAppReq();
        pkt.receipt_ = new PktInfoStr();
        pkt.receipt_.str_ = receipt;
        pkt.storeID_ = (uint)storeid;
        pkt.inappKind_ = eInAppKind.PLAY_STORE;
#if UNITY_IOS
        pkt.inappKind_ = eInAppKind.APP_STORE;
#elif !DISABLESTEAMWORKS
        pkt.inappKind_ = eInAppKind.STEAM_STORE;
#endif
        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqStorePurchaseInApp, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_Purchase(storeid, false, ReceiveCallBack);
    }

#if !DISABLESTEAMWORKS
    public void Send_ReqSteamPurchase(int storeid, OnReceiveCallBack ReceiveCallBack)
    {
        if (AppMgr.Instance.configData.m_ServiceType != Config.eServiceType.Steam)
            return;

        SetCallBackList("Send_ReqSteamPurchase", ReceiveCallBack);

        if (_netflag)
        {
            if (!CheckConnect())
                return;

            var item = GameTable.Stores.Find(o => o.ID == storeid);
            if (item == null)
            {
                Debug.Log("Stores Not Find Item : " + storeid);
                return;
            }

            string itemDesc = "";
            var sdg = GameClientTable.StoreDisplayGoodss.Find(o => o.StoreID == storeid && o.PackageUIType == (int)eCOUNT.NONE);

            if (sdg == null)
            {
                sdg = GameClientTable.StoreDisplayGoodss.Find(o => o.StoreID == storeid);
            }


            if (sdg == null)
            {
                Debug.Log("StoreDisplayGoods Not Find Item : " + storeid);
            }
            else
            {
                itemDesc = FLocalizeString.Instance.GetText(sdg.Name);
                while(true)
                {
                    int start = itemDesc.IndexOf("[");
                    if (start < 0)
                        break;

                    int end = itemDesc.IndexOf("]");
                    if (end < 0)
                        end = itemDesc.Length - 1;

                    itemDesc = itemDesc.Remove(start, end - start + 1);
                }

            }

            WaitPopup.Show();

            PktInfoSteamPurchaseReq pktInfo = new PktInfoSteamPurchaseReq();
            pktInfo.languageCode_ = AppMgr.Instance.SteamLanguage;
            pktInfo.storeID_ = (uint)storeid;            
            pktInfo.steamID_ = AppMgr.Instance.SteamId;            
            pktInfo.itemDesc_ = itemDesc;

            NETStatic.PktLby.ReqSteamPurchase(pktInfo);
        }
    }
#endif

    //=============================================================================================
    // 피규어 룸
    //=============================================================================================

    public void Send_SetMainRoomTheme(int roomthemeslotnum, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqSetMainRoomTheme, roomthemeslotnum, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_SetMainRoomTheme(roomthemeslotnum, ReceiveCallBack);
    }

    public void Send_RoomPurchase(int storeroomtid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqRoomPurchase, storeroomtid, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_RoomPurchase(storeroomtid, ReceiveCallBack);
    }

    public void Send_RoomThemeSlotDetailInfo(int roomthemeslotnum, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqRoomThemeSlotDetailInfo, roomthemeslotnum, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_RoomThemeSlotDetailInfo(roomthemeslotnum, ReceiveCallBack);
    }

    public void Send_RoomThemeSlotSave(RoomThemeSlotData roomslotdata, List<RoomThemeFigureSlotData> roomfigureslotlist, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        RoomThemeSlotDetail.DoClear();
        if (RoomThemeSlotDetail.figureSlot_ == null)
        {
            RoomThemeSlotDetail.figureSlot_ = new PktInfoRoomFigureSlot();
            RoomThemeSlotDetail.figureSlot_.infos_ = new List<PktInfoRoomFigureSlot.Piece>();
        }

        RoomThemeSlotDetail.themeSlot_.slotNum_ = (uint)roomslotdata.SlotNum;
        RoomThemeSlotDetail.themeSlot_.tableID_ = (uint)roomslotdata.TableID;
		RoomThemeSlotDetail.themeSlot_.funcStateFlag_ = 0;


		for (int i = 0; i < roomslotdata.RoomThemeFuncList.Count; i++)
        {
            if (true == roomslotdata.RoomThemeFuncList[i].On)
                RoomThemeSlotDetail.themeSlot_.DoOnFunc((ulong)i);
            else
                RoomThemeSlotDetail.themeSlot_.DoOffFunc((ulong)i);
        }

        RoomThemeSlotDetail.themeSlot_.data_ = new PktInfoStream();
        RoomThemeSlotDetail.themeSlot_.data_.stm_ = new List<Byte>();
        RoomThemeSlotDetail.themeSlot_.data_.DoAdd(roomslotdata.ArrLightInfo);

        for (int i = 0; i < roomfigureslotlist.Count; i++)
        {
            var piece = new PktInfoRoomFigureSlot.Piece();
            piece.actionID1_ = (uint)roomfigureslotlist[i].Action1;
            piece.actionID2_ = 0;//(uint)roomfigureslotlist[i].Action2;
            piece.figureSlotNum_ = (byte)roomfigureslotlist[i].SlotNum;
            piece.themeSlotNum_ = (byte)roomfigureslotlist[i].RoomThemeSlotNum;
            piece.tableID_ = (uint)roomfigureslotlist[i].TableID;

            piece.detail_ = new PktInfoStream();
            piece.detail_.stm_ = new List<System.Byte>();
            piece.detail_.DoAdd(roomfigureslotlist[i].detailarry);

            piece.skinStateFlag_ = (uint)roomfigureslotlist[i].CostumeStateFlag;
            piece.costumeClr_ = (byte)roomfigureslotlist[i].CostumeColor;

            RoomThemeSlotDetail.figureSlot_.infos_.Add(piece);
        }

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqRoomThemeSlotSave, RoomThemeSlotDetail, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
        {
            // 오픈스펙엔 슬롯 0번만 사용 (슬롯 저장/로드 기능은 사용안함)
            _roomthemeslotlist.Clear();
            _roomthemeslotlist.Add(roomslotdata);
            NetLocalSvr.Instance.Proc_RoomThemeSlotSave(roomslotdata, roomfigureslotlist, ReceiveCallBack);
        }
    }



    //=============================================================================================
    // 이벤트 모드
    //=============================================================================================
    public void Send_ReqEventRewardReset(int eventid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqEventRewardReset, eventid, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqEventRewardReset(eventid, ReceiveCallBack);
    }

    public void Send_ReqEventRewardTake(int eventid, int cnt, int eventresetstep, int idx, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        EventSetData eventSetData = GameInfo.Instance.GetEventSetData(eventid);
        PktInfoEventRewardReq pkt = new PktInfoEventRewardReq();
        pkt.eventID_ = (uint)eventid;
        pkt.cnt_ = (byte)cnt;
        pkt.step_ = (byte)eventresetstep;
        pkt.idx_ = (byte)idx;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqEventRewardTake, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqEventRewardTake(eventid, cnt, eventresetstep, idx, ReceiveCallBack);
    }

    public void Send_ReqSetTutorialVal(int state, int step, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        byte[] bytes = new byte[(int)eTutorial.Count];
        bytes[0] = (byte)state;
        bytes[1] = (byte)step;
        bytes[2] = 0;
        bytes[3] = 0;
        int TutorialNum = BitConverter.ToInt32(bytes, 0);

        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
        {
            eTutorialState tutorialState = (eTutorialState)GameInfo.Instance.UserData.GetTutorialState();
            if (tutorialState == eTutorialState.TUTORIAL_STATE_Init)
                GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_1_Tutorial_Skip);
            else if (tutorialState == eTutorialState.TUTORIAL_STATE_Stage2Clear)
                GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_2_Tutorial_Skip);
            else if (tutorialState == eTutorialState.TUTORIAL_STATE_Stage3Clear)
                GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_3_Tutorial_Skip);
        }

        if (state == (int)eTutorialState.TUTORIAL_STATE_EndTutorial)
        {
            CharSelete = false;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqSetTutorialVal, TutorialNum, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqSetTutorialVal(state, step, ReceiveCallBack);
    }

    public void Send_ReqSetTutorialFlag(int flag, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        UserData.DoOnTutorialFlag((eTutorialFlag)flag);
        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqSetTutorialFlag, GameInfo.Instance.UserData.TutorialFlag, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqSetTutorialFlag(GameInfo.Instance.UserData.TutorialFlag, ReceiveCallBack);
    }

    public void Send_ReqEventLgnRewardTake(int pTableId, byte[] pDays, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        if (!CheckConnect())
        {
            return;
        }

        PktInfoEvtLgnRwdReq pktInfoEvtLgnRwdReq = new PktInfoEvtLgnRwdReq
        {
            evtLgnTID_ = (uint)pTableId,
            rwdDays_ = new PktInfoUInt8List
            {
                vals_ = new List<byte>()
            }
        };
        
        pktInfoEvtLgnRwdReq.rwdDays_.vals_.AddRange(pDays);

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqEventLgnRewardTake, pktInfoEvtLgnRwdReq, ReceiveCallBack);
        
        SendProtocolData();
    }
    //=============================================================================================
    // PushNotification(서버 푸쉬토큰 등록)
    //=============================================================================================
    public void Send_ReqPushNotifiTokenSet(string token, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoPushNotiSetToken pkt = new PktInfoPushNotiSetToken();
        pkt.token_ = new PktInfoStr();
        pkt.token_.str_ = token;
        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqPushNotifiTokenSet, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    //=============================================================================================
    // 아레나
    //=============================================================================================

    /// <summary>
    /// 시즌 참가 요청
    /// </summary>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqArenaSeasonPlay(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqArenaSeasonPlay, 0, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqArenaSeasonPlay(ReceiveCallBack);
    }

    /// <summary>
    /// 아레나 랭킹 요청
    /// </summary>
    /// <param name="updateTM"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqArenaRankingList(long updateTM, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqArenaRankingList, updateTM, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqArenaRankingList(ReceiveCallBack);
    }

    /// <summary>
    /// 랭커 상세정보 요청
    /// </summary>
    /// <param name="rankUID"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqArenaRankerDetail(long rankUID, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqArenaRankerDetail, rankUID, ReceiveCallBack);

        if (!CheckConnect())
            return;
        if (_netflag)
            SendProtocolData();
    }

    /// <summary>
    /// 아레나 팀편성 요청
    /// </summary>
    /// <param name="teamCharList"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqSetArenaTeam(List<long> teamCharList, uint arenaCardFrmtID, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoUserArenaTeam pkt = new PktInfoUserArenaTeam();
        for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
        {
            pkt.CUIDs_[i] = (ulong)teamCharList[i];
        }
        pkt.cardFrmtID_ = (uint)arenaCardFrmtID;
        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqSetArenaTeam, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqSetArenaTeam(teamCharList, ReceiveCallBack);
    }

    /// <summary>
    /// 대전 상대 찾기
    /// </summary>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqArenaEnemySearch(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqArenaEnemySearch, null, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    /// <summary>
    /// 아레나 에서 문양 장착
    /// </summary>
    /// <param name="badgeUID">장착할 문양 UID</param>
    /// <param name="slotNum">장착할 슬롯 번호</param>
    /// <param name="ReceiveCallBack">콜백 함수</param>
    public void Send_ReqApplyPosBadge(long badgeUID, int slotNum, eContentsPosKind posKind, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoBadgeApplyPos pkt = new PktInfoBadgeApplyPos();
        pkt.badgeUID_ = (UInt64)badgeUID;
        pkt.posValue_ = (UInt64)GameInfo.Instance.UserData.UUID;
        pkt.posKind_ = posKind;
        pkt.slotNum_ = (Byte)slotNum;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqApplyPosBadge, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqApplyPosBadge(badgeUID, slotNum, ReceiveCallBack);
    }

    /// <summary>
    /// 문양 장착 해제
    /// </summary>
    /// <param name="badgeUID">해제할 문양 UID</param>
    /// <param name="ReceiveCallBack">콜백 함수</param>
    public void Send_ReqApplyOutPosBadge(long badgeUID, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoBadgeComReq pkt = new PktInfoBadgeComReq();
        pkt.badgeUID_ = (UInt64)badgeUID;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqApplyOutPosBadge, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqApplyOutPosBadge(badgeUID, ReceiveCallBack);

    }

    /// <summary>
    /// 문양 강화
    /// </summary>
    /// <param name="badgeUID"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqUpgradeBadge(long badgeUID, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoBadgeComReq pkt = new PktInfoBadgeComReq();
        pkt.badgeUID_ = (UInt64)badgeUID;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqUpgradeBadge, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqUpgradeBadge(badgeUID, ReceiveCallBack);
    }

    /// <summary>
    /// 문양 강화 레벨 초기화
    /// </summary>
    /// <param name="badgeUID"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqResetUpgradeBadge(long badgeUID, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        PktInfoBadgeComReq pkt = new PktInfoBadgeComReq();
        pkt.badgeUID_ = (UInt64)badgeUID;

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqResetUpgradeBadge, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqResetUpgradeBadge(badgeUID, ReceiveCallBack);
    }

    /// <summary>
    /// 문양 잠금 상태 변경
    /// </summary>
    /// <param name="badgeuidlist"></param>
    /// <param name="locklist"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqSetLockBadge(List<long> badgeuidlist, List<bool> locklist, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoBadgeLock();
        pkt.infos_ = new List<PktInfoBadgeLock.Piece>();
        for (int i = 0; i < badgeuidlist.Count; i++)
        {
            var data = new PktInfoBadgeLock.Piece();
            data.badgeUID_ = (UInt64)badgeuidlist[i];
            data.lock_ = (Boolean)locklist[i];
            pkt.infos_.Add(data);
        }

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqSetLockBadge, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqSetLockBadge(badgeuidlist, locklist, ReceiveCallBack);
    }

    /// <summary>
    /// 문양 판매
    /// </summary>
    /// <param name="uidlist"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqSellBadge(List<long> uidlist, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoBadgeSell();
        pkt.infos_ = new List<PktInfoBadgeSell.Piece>();
        for (int i = 0; i < uidlist.Count; i++)
        {
            var data = new PktInfoBadgeSell.Piece();
            data.badgeUID_ = (UInt64)uidlist[i];
            pkt.infos_.Add(data);
        }

        AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqSellBadge, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
        else
            NetLocalSvr.Instance.Proc_ReqSellBadge(uidlist, ReceiveCallBack);
    }

    public void Send_ReqArenaGameStart(List<long> buffItemUIDList, bool buffBattleCoin, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        var pkt = new PktInfoArenaGameStartReq();
        pkt.useItemUIDs_ = new List<ulong>();
        for (int i = 0; i < buffItemUIDList.Count; i++)
            pkt.useItemUIDs_.Add((UInt64)buffItemUIDList[i]);

        pkt.useBattleCoinBuff_ = buffBattleCoin;
        pkt.upCharSKBuffFlag_ = GameSupport.IsActiveUserBuff(eBuffEffectType.Buff_SkillSlot);

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqArenaGameStart, pkt, ReceiveCallBack);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    public void Send_ReqArenaGameEnd(byte result, uint playTime, bool isFriendPVP, UInt32 opponentScore, UInt32 maxDamage, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
        {
            return;
        }

        bArenaLose = false;
        if ((int)result == 0 && !isFriendPVP)
            bArenaLose = true;

        PktInfoArenaGameEndReq pkt = new PktInfoArenaGameEndReq();
        pkt.enemyScore_ = (uint)MatchTeam.Score;
        pkt.enemyTeamPower_ = (uint)GameSupport.GetArenaEnemyTeamPower();
        pkt.result_ = result;
        pkt.playTime_Ms_ = playTime;

        pkt.teamHP_ = GameSupport.GetTotalCardFormationEffectValue();
        pkt.teamAtk_ = GameSupport.GetTotalWeaponDepotEffectValue();

        pkt.teamPower_ = (uint)GameSupport.GetArenaTeamPower(eContentsPosKind.ARENA);
        if (PktArenaGameStart != null)
        {
            pkt.certifyKey_ = PktArenaGameStart.certifyKey_;
        }

        pkt.enemyScore_ = opponentScore;
        pkt.maxDamage_ = maxDamage;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqArenaGameEnd, pkt, callback);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    public void Send_ReqRefrashUserInfo(OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqRefrashUserInfo, null, callback);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    //Pass
    public void Send_ReqRewardPassMission(List<int> passMissions, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        PktInfoRwdPassMission pkt = new PktInfoRwdPassMission();

        pkt.ids_ = new PktInfoTIDList();
        pkt.ids_.tids_ = new List<System.UInt32>();
        for (int i = 0; i < passMissions.Count; i++)
            pkt.ids_.tids_.Add((uint)passMissions[i]);

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqRewardPassMission, pkt, callback);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    public void Send_ReqRewardPass(int passid, int rewardCnt, int rewardSPCnt, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        PktInfoRwdPassReq pkt = new PktInfoRwdPassReq();
        pkt.passID_ = (uint)passid;
        pkt.rwdEndPT_N_ = (Byte)rewardCnt;
        pkt.rwdEndPT_S_ = (Byte)rewardSPCnt;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqRewardPass, pkt, callback);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    //친구 관련 통신
    // 커뮤니티 정보 획득 요청
    public void Send_ReqCommunityInfoGet(OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqCommunityInfoGet, null, callback);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    // 추천 친구 목록 요청
    public void Send_ReqFriendSuggestList(string serachUUID, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        PktInfoCommuSuggestReq pkt = new PktInfoCommuSuggestReq();
        pkt.sugUids_ = new PktInfoUIDList();
        pkt.sugUids_.uids_ = new List<ulong>();
        if (serachUUID != string.Empty)
        {
            pkt.sugUids_.uids_.Add(ulong.Parse(serachUUID));
        }

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqFriendSuggestList, pkt, callback);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    // 친구 신청 요청
    public void Send_ReqFriendAsk(long friendAskUUID, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        PktInfoCommuAskReq pkt = new PktInfoCommuAskReq();
        //pkt.reqUuid_ = (ulong)friendAskUUID;
        pkt.tgtUids_ = new PktInfoUIDList();
        pkt.tgtUids_.uids_ = new List<ulong>();
        pkt.tgtUids_.uids_.Add((ulong)friendAskUUID);

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqFriendAsk, pkt, callback);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    // 친구 신청에 대한 답변 요청 - 나한테 온 목록 중에 승인, 미승인
    public void Send_ReqFriendAnswer(long friendUUID, bool addflag, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        PktInfoCommuAnswer pkt = new PktInfoCommuAnswer();
        pkt.accept_ = addflag;
        pkt.tgtUids_ = new PktInfoUIDList();
        pkt.tgtUids_.uids_ = new List<ulong>();
        pkt.tgtUids_.uids_.Add((ulong)friendUUID);

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqFriendAnswer, pkt, callback);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    // 친구 제거 요청
    public void Send_ReqFriendKick(List<long> kickUUID_List, OnReceiveCallBack callback) {
        if (IsProtocolData())
            return;

        PktInfoCommuKick pkt = new PktInfoCommuKick();
        pkt.tgtUids_ = new PktInfoUIDList();
        pkt.tgtUids_.uids_ = new List<ulong>();

        for (int i = 0; i < kickUUID_List.Count; i++) {
            pkt.tgtUids_.uids_.Add((ulong)kickUUID_List[i]);
        }

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqFriendKick, pkt, callback);


        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    // 친구 포인트 보내기 요청
    public void Send_ReqFriendPointGive(OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqFriendPointGive, null, callback);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    // 친구 포인트 받기 요청
    public void Send_ReqFriendPointTake(List<long> uuidList, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        PktInfoFriendPointTakeReq pkt = new PktInfoFriendPointTakeReq();
        pkt.tgtUids_ = new PktInfoUIDList();
        pkt.tgtUids_.uids_ = new List<ulong>();

        for (int i = 0; i < uuidList.Count; i++)
            pkt.tgtUids_.uids_.Add((ulong)uuidList[i]);

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqFriendPointTake, pkt, callback);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    // 친구 신청 취소 요청
    public void Send_ReqFriendAskDel(List<long> uuidList, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        PktInfoCommuAskDel pkt = new PktInfoCommuAskDel();
        pkt.tgtUids_ = new PktInfoUIDList();
        pkt.tgtUids_.uids_ = new List<ulong>();

        for (int i = 0; i < uuidList.Count; i++)
            pkt.tgtUids_.uids_.Add((ulong)uuidList[i]);

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqFriendAskDel, pkt, callback);


        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    // 친구 프라이빗룸 입장 가능 상태 변경 요청
    public void Send_ReqFriendRoomVisitFlag(List<long> uuidList, bool roomAccept, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        PktInfoFriendRoomFlag pkt = new PktInfoFriendRoomFlag();
        //pkt.accept_ = roomAccept;

        pkt.tgtUids_ = new PktInfoUIDList();
        pkt.tgtUids_.uids_ = new List<ulong>();

        for (int i = 0; i < uuidList.Count; i++)
            pkt.tgtUids_.uids_.Add((ulong)uuidList[i]);

        pkt.accept_ = roomAccept;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqFriendRoomVisitFlag, pkt, callback);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    // 친구 프라이빗룸 정보 요청
    public void Send_ReqFriendRoomInfoGet(FriendUserData friendUser, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        PktInfoCommuRoomInfoGet pkt = new PktInfoCommuRoomInfoGet();
        pkt.tgtUuid_ = (ulong)friendUser.UUID;
        pkt.roomSlotNum_ = (byte)friendUser.RoomSlotNum;
        pkt.dbID_ = (ulong)friendUser.DBID;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqFriendRoomInfoGet, pkt, callback);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    // 친구 아레나 정보 요청
    public void Send_ReqCommunityUserArenaInfoGet(eCommunityUserInfoGetType getType, List<ulong> friendAskUUID, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
        {
            return;
        }

        CommunityUserInfoGetType = getType;

        PktInfoCommuUserArenaInfoReq pkt = new PktInfoCommuUserArenaInfoReq();
        pkt.uids_ = new PktInfoUIDList();
        pkt.uids_.uids_ = new List<ulong>();
        var iter = friendAskUUID.GetEnumerator();
        while(iter.MoveNext())
        {
            pkt.uids_.uids_.Add(iter.Current);
        }
        pkt.reqUuid_ = (ulong)_userdata.UUID;

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqCommunityUserArenaInfoGet, pkt, callback);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    public void Send_ReqUserSetCountryAndLangCode(OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        PktInfoCountryLangCode pkt = new PktInfoCountryLangCode();
        pkt.country_ = new PktInfoStr();
        pkt.lang_ = new PktInfoStr();

        //System.Globalization.RegionInfo.CurrentRegion.Name
        string countryCode = System.Globalization.RegionInfo.CurrentRegion.Name;

        pkt.country_.str_ = countryCode;
        pkt.lang_.str_ = FLocalizeString.Language.ToString();

        AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqUserSetCountryAndLangCode, pkt, callback);

        if (!CheckConnect())
            return;

        if (_netflag)
            SendProtocolData();
    }

    // 파견-슬롯 열기
    public void Send_ReqDispatchOpen(uint tableid, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        PktInfoTIDList pkt = new PktInfoTIDList();

        pkt.tids_ = new List<uint>();
        pkt.tids_.Add(tableid);

        if (_netflag)
        {
            AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqDispatchOpen, pkt, callback);
            SendProtocolData();
        }
    }

    // 파견-시작
    public void Send_ReqDispatchOper(uint dispatchTID, List<long> CUIDs, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        PktInfoDispatchOperReq pkt = new PktInfoDispatchOperReq();
        pkt.cardUIDs_ = new PktInfoUIDList();
        pkt.cardUIDs_.uids_ = new List<ulong>();
        foreach (var id in CUIDs)
        {
            pkt.cardUIDs_.uids_.Add((ulong)id);
        }

        pkt.dispatchTID_ = dispatchTID;

        if (_netflag)
        {
            AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqDispatchOperation, pkt, callback);
            SendProtocolData();
        }
    }

    // 파견-완료
    public void Send_ReqDispatchOperConfirm(uint dispatchTID, bool isImmediateDone, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        PktInfoDispatchOperConfirmReq pkt = new PktInfoDispatchOperConfirmReq();
        pkt.dispatchTID_ = dispatchTID;
        pkt.isImmediateComplete_ = isImmediateDone;

        if (_netflag)
        {
            AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqDispatchOperationConfirm, pkt, callback);
            SendProtocolData();
        }
    }

    // 파견-(미션)교체
    public void Send_ReqDispatchChange(uint tableid, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        PktInfoTIDList pkt = new PktInfoTIDList();

        pkt.tids_ = new List<uint>();
        pkt.tids_.Add(tableid);

        if (_netflag)
        {
            AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqDispatchChange, pkt, callback);
            SendProtocolData();
        }
    }

    public void Send_ReqUserLobbyThemeList(OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        if (_netflag)
        {
            AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqUserLobbyThemeList, null, callback);
            SendProtocolData();
        }
    }

    public void Send_ReqUserSetLobbyTheme(uint tableid, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        if (_netflag)
        {
            AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqUserSetLobbyTheme, tableid, callback);
            SendProtocolData();
        }
    }

    //지원부대
    public void Send_ReqUserSetMainCardFormation(uint tableid, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        if (_netflag)
        {
            AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqUserSetMainCardFormation, tableid, callback);
            SendProtocolData();
        }
    }

    //무기고
    public void Send_ReqAddSlotInWpnDepot(int addCnt, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        if (_netflag)
        {
            AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqAddSlotInWpnDepot, addCnt, callback);
            SendProtocolData();
        }
    }

    public void Send_ReqApplySlotInWpnDepot(long originUID, long weaponUUID, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        if (_netflag)
        {
            List<long> weaponSlotList = new List<long>();

            weaponSlotList.AddRange(GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList);

            if (originUID != (int)eCOUNT.NONE)
            {
                if (originUID == weaponUUID)
                {
                    if (weaponSlotList.Contains(weaponUUID))
                        weaponSlotList.Remove(weaponUUID);
                }
                else
                {
                    if (weaponSlotList.Contains(weaponUUID))
                        weaponSlotList.Remove(weaponUUID);
                    else
                    {
                        if (weaponSlotList.Contains(originUID))
                            weaponSlotList.Remove(originUID);

                        weaponSlotList.Add(weaponUUID);
                    }
                        
                }
            }
            else
            {
                if (weaponSlotList.Contains(weaponUUID))
                    weaponSlotList.Remove(weaponUUID);
                else
                    weaponSlotList.Add(weaponUUID);
            }

            PktInfoWpnDepotApply pktInfoWpnDepotApply = new PktInfoWpnDepotApply();

            pktInfoWpnDepotApply.slots_ = new List<PktInfoWpnDepotSet.Piece>();

            for (int i = 0; i < weaponSlotList.Count; i++)
            {
                PktInfoWpnDepotSet.Piece piece = new PktInfoWpnDepotSet.Piece();
                piece.wnpUID_ = (ulong)weaponSlotList[i];

                pktInfoWpnDepotApply.slots_.Add(piece);
            }

            AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqApplySlotInWpnDepot, pktInfoWpnDepotApply, callback);
            SendProtocolData();
        }
    }

    /// <summary>
    /// 아레나 타워 팀편성 요청
    /// </summary>
    /// <param name="teamCharList"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqSetArenaTowerTeam(List<long> teamCharList, uint cardFrmtID, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        PktInfoUserArenaTeam pkt = new PktInfoUserArenaTeam();
        for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
        {
            pkt.CUIDs_[i] = (ulong)teamCharList[i];
        }
        pkt.cardFrmtID_ = cardFrmtID;

        if (_netflag)
        {
            AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqSetArenaTowerTeam, pkt, ReceiveCallBack);            
            SendProtocolData();
        }
    }

    /// <summary>
    /// 아레나 타워 시작
    /// </summary>
    /// <param name="teamCharList"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqArenaTowerGameStart(ulong uuid, uint stageid, bool charskillbuffFlag,  OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

		IsReqStageEndReConnect = false;

		if (!CheckConnect())
            return;

        PktInfoArenaTowerGameStartReq pkt = new PktInfoArenaTowerGameStartReq();
        pkt.useCommuUuid_ = uuid;
        pkt.towerID_ = stageid;
        pkt.upCharSKBuffFlag_ = charskillbuffFlag;

        if (_netflag)
        {
            AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqArenaTowerGameStart , pkt, ReceiveCallBack);
            SendProtocolData();
        }
    }

    /// <summary>
    /// 아레나 타워 종료
    /// </summary>
    /// <param name="teamCharList"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_PktInfoArenaTowerGameEndReq(uint playTime, bool bSuccess, OnReceiveCallBack ReceiveCallBack)
    {
        if ( GameInfo.Instance.IsTowerStageTestPlay ) {
            return;
		}

        if (IsProtocolData())
            return;

		if (!CheckConnect())
		{
			IsReqStageEndReConnect = true;
			return;
		}

        PktInfoArenaTowerGameEndReq pkt = new PktInfoArenaTowerGameEndReq();
        pkt.playTime_Ms_ = playTime;
        pkt.successFlag_ = bSuccess;        

        if (_netflag)
        {
            AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqArenaTowerGameEnd, pkt, ReceiveCallBack);
            SendProtocolData();
        }
    }

    // 유저 서버 이전 정보 설정 요청 
    public void Send_ReqRelocateUserInfoSet(string MoveID, string MovePW, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        if (AppMgr.Instance.configData == null)
            return;

        if (AppMgr.Instance.configData.m_ServiceType != Config.eServiceType.Japan)
            return;        

        PktInfoRelocateUser pkt = new PktInfoRelocateUser();

        pkt.accountID_ = new PktInfoStr();
        pkt.id_ = new PktInfoStr();
        pkt.pw_ = new PktInfoStr();

        pkt.accountID_.str_ = string.Empty;
        pkt.id_.str_ = MoveID;
        pkt.pw_.str_ = MovePW;        

        if (_netflag)
        {
            AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqRelocateUserInfoSet, pkt, callback);
            SendProtocolData();
        }
    }

    // 유저 서버 이전 완료 요청
    public void Send_ReqRelocateUserComplate(string MoveID, string MovePW, OnReceiveCallBack callback)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        if (AppMgr.Instance.configData == null)
            return;

        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
            return;

        SetCallBackList("Send_ReqRelocateUserComplate", callback);


        PktInfoRelocateUser pkt = new PktInfoRelocateUser();

        pkt.accountID_ = new PktInfoStr();
        pkt.id_ = new PktInfoStr();
        pkt.pw_ = new PktInfoStr();

        //Send_ReqLogin()과 같은 코드        
#if UNITY_EDITOR
        pkt.accountID_.str_ = Platforms.IBase.Inst.GetDeviceUniqueID();
#else
#if DISABLESTEAMWORKS
            pkt.accountID_.str_ = Platforms.IBase.Inst.GetDeviceUniqueID();
#else // 스팀에선 디바이스 ID를 스팀아이디로 설정
            pkt.accountID_.str_ = string.Format("{0}_Steam", AppMgr.Instance.SteamId);
#endif
#endif
        pkt.id_.str_ = MoveID;
        pkt.pw_.str_ = MovePW;        

        if (_netflag)
        {
            AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqRelocateUserComplate, pkt, callback);
            SendProtocolData();
        }
    }

    // 속성 변경 요청
    public void Send_ReqChangeCardType(long cardUid, int type, eGOODSTYPE goodsType, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        if (!CheckConnect())
        {
            return;
        }

        PktInfoCardTypeChangeReq pkt = new PktInfoCardTypeChangeReq();
        pkt.cardUID_ = (ulong)cardUid;
        pkt.type_ = (byte)type;
        pkt.goodsTP_ = goodsType;

        if (_netflag)
        {
            AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqChangeTypeCard, pkt, ReceiveCallBack);
            SendProtocolData();
        }
    }

    public void Send_ReqLvUpUserSkill(int awakenSkillTid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        PktInfoUserSklLvUpReq pktInfo = new PktInfoUserSklLvUpReq();
        pktInfo.tid_ = (uint)awakenSkillTid;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqLvUpUserSkill, pktInfo, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    public void Send_ReqResetUserSkill(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqResetUserSkill, null, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

#region Influence

    // 유저 서버 달성(세력) 미션 보상 요청
    public void Send_ReqRewardInfluMission(List<byte> datas, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        PktInfoComRwd pkt = new PktInfoComRwd();
        pkt.idxs_ = new PktInfoUInt8List();
        pkt.idxs_.vals_ = new List<byte>();
        for (int i = 0; i < datas.Count; i++)
        {
            pkt.idxs_.vals_.Add(datas[i]);
        }

        if (_netflag)
        {
            AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqRewardInfluMission, pkt, ReceiveCallBack);
            SendProtocolData();
        }
    }

    // 서버 달성(세력) 선택 요청
    public void Send_ReqInfluenceChoice(uint InfluenceID, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;
        
        PktInfoInfluenceChoice pkt = new PktInfoInfluenceChoice();
        pkt.tid_ = InfluenceID;
        
        if (_netflag)
        {
            AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqInfluenceChoice, pkt, ReceiveCallBack);
            SendProtocolData();
        }
    }

    // 서버 달성(세력) 정보 요청
    public void Send_ReqGetInfluenceInfo(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        if (_netflag)
        {
            AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqGetInfluenceInfo, null, ReceiveCallBack);
            SendProtocolData();
        }
    }

    // 서버 달성(세력) 랭킹 정보 요청
    public void Send_ReqGetInfluenceRankInfo(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        PktInfoInfluRankListReq pkt = new PktInfoInfluRankListReq();
        pkt.lastUpTM_ = new PktInfoTime();
        pkt.lastUpTM_.time_ = InfluenceRankData.LaseUpdateTime.time_;
        pkt.reqUuid_ = (ulong)UserData.UUID;
        
        if (_netflag)
        {
            AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqGetInfluenceRankInfo, pkt, ReceiveCallBack);
            SendProtocolData();
        }
    }

    // 서버 달성(세력) 보상 요청
    public void Send_ReqInfluenceTgtRwd(List<byte> datas, byte rewardType, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        PktInfoRwdInfluenceTgtReq pkt = new PktInfoRwdInfluenceTgtReq();
        pkt.idxs_ = new PktInfoUInt8List();
        pkt.idxs_.vals_ = new List<byte>();
        for (int i = 0; i < datas.Count; i++)
        {
            pkt.idxs_.vals_.Add(datas[i]);
        }
        pkt.rwdTP_ = rewardType;

        if (_netflag)
        {
            AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqInfluenceTgtRwd, pkt, ReceiveCallBack);
            SendProtocolData();
        }
    }
#endregion

    public void Send_ReqGetTotalRelocateCntToNotComplete(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqGetTotlaRelocateCntToNotComplete, null, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    public void Send_ReqEnchantCard(long TargetUID,  OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        PktInfoProductComGrowReq pkt = new PktInfoProductComGrowReq();
        int tutoValue = 0;

        pkt.maters_ = new PktInfoUIDList();
        pkt.maters_.uids_ = new List<ulong>();
        pkt.materItems_ = new PktInfoItemCnt();
        pkt.materItems_.infos_ = new List<PktInfoItemCnt.Piece>();       

        pkt.targetUID_ = (ulong)TargetUID;
        pkt.tutoVal_ = (uint)tutoValue;
        if (_netflag)
        {
            AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqEnchantCard , pkt, ReceiveCallBack);
            SendProtocolData();
        }
    }

    public void Send_ReqEnchantWeapon(long TargetUID, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        PktInfoProductComGrowReq pkt = new PktInfoProductComGrowReq();
        int tutoValue = 0;

        pkt.maters_ = new PktInfoUIDList();
        pkt.maters_.uids_ = new List<ulong>();
        pkt.materItems_ = new PktInfoItemCnt();
        pkt.materItems_.infos_ = new List<PktInfoItemCnt.Piece>();

        pkt.targetUID_ = (ulong)TargetUID;
        pkt.tutoVal_ = (uint)tutoValue;
        if (_netflag)
        {
            AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqEnchantWeapon, pkt, ReceiveCallBack);
            SendProtocolData();
        }
    }

    public void Send_ReqDecomposition(eContentsPosKind kind, List<ulong> uids, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        PktInfoDecompositionReq pkt = new PktInfoDecompositionReq();
        pkt.decompositionList_ = new PktInfoUIDList();
        pkt.decompositionList_.uids_ = new List<ulong>();

        pkt.kind_ = kind;
        pkt.decompositionList_.uids_.AddRange(uids);

        if (_netflag)
        {   
            AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqDecomposition, pkt, ReceiveCallBack);
            SendProtocolData();
        }

    }

    public void Send_ReqUserRotationGachaOpen(List<uint>tableIDs, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        PktInfoTIDList pkt = new PktInfoTIDList();
        pkt.tids_ = new List<uint>();
        pkt.tids_.AddRange(tableIDs);

        if (_netflag)
        {
            AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqUserRotationGachaOpen, pkt, ReceiveCallBack);
            SendProtocolData();
        }
    }

    public void Send_ReqChangeRaidAllStoreList( List<byte> raidStoreNumberList, OnReceiveCallBack callBack ) {
        if( IsProtocolData() || !CheckConnect() ) {
            return;
        }

        PktInfoRaidStoreListReq pkt = new PktInfoRaidStoreListReq();
        pkt.resetNums_ = new PktInfoUInt8List();
        pkt.resetNums_.vals_ = new List<byte>( raidStoreNumberList.Count );

        for( int i = 0; i < raidStoreNumberList.Count; i++ ) {
            pkt.resetNums_.vals_.Add( (byte)raidStoreNumberList[i] );
        }

        AddProtocolData( ePacketType.Lobby, LobbyC2S.Common.ReqRaidStoreList, pkt, callBack );
        SendProtocolData();
    }

    public void Send_ReqRaidStoreList( byte raidStoreNumber, OnReceiveCallBack callBack ) {
        if( IsProtocolData() || !CheckConnect() ) {
            return;
        }

        PktInfoRaidStoreListReq pkt = new PktInfoRaidStoreListReq();
        pkt.resetNums_ = new PktInfoUInt8List();
        pkt.resetNums_.vals_ = new List<byte>();

        if( raidStoreNumber > 0 ) {
            pkt.resetNums_.vals_.Add( raidStoreNumber );
        }

        AddProtocolData( ePacketType.Lobby, LobbyC2S.Common.ReqRaidStoreList, pkt, callBack );
        SendProtocolData();
    }

    public void Send_ReqAccountLinkReward(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        if (_netflag)
        {            
            AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqAccountLinkReward, null, ReceiveCallBack);
            SendProtocolData();
        }
    }

    public void Send_ReqAccountCodeReward(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        if (_netflag)
        {
            AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqAccountCodeReward, null, ReceiveCallBack);
            SendProtocolData();
        }
    }

    // 캐시로 구매 가능한 아이템 구입 요청
    public void Send_ReqItemExchangeCash(int itemTableID, int itemCnt, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        if (!CheckConnect())
            return;

        PktInfoItemTIDCnt pktInfoItemTIDCnt = new PktInfoItemTIDCnt();
        pktInfoItemTIDCnt.infos_ = new List<PktInfoItemTIDCnt.Piece>();
        PktInfoItemTIDCnt.Piece exchangeItem = new PktInfoItemTIDCnt.Piece();
        exchangeItem.tid_ = (uint)itemTableID;
        exchangeItem.cnt_ = (ushort)itemCnt;

        pktInfoItemTIDCnt.infos_.Add(exchangeItem);

        if (_netflag)
        {
            AddProtocolData(ePacketType.Product, ProductC2S.Common.ReqItemExchangeCash, pktInfoItemTIDCnt, ReceiveCallBack);
            SendProtocolData();
        }
    }

    public void Send_ReqUnexpectedPackageDailyReward(int tableId, int dayNumber, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }
        
        if (!CheckConnect())
        {
            return;
        }
        
        PktInfoUnexpectedPackageDailyRewardReq pktInfoUnexpectedPackageDailyRewardReq =
            new PktInfoUnexpectedPackageDailyRewardReq();
        
        pktInfoUnexpectedPackageDailyRewardReq.unexpectedPackageTID_ = (uint)tableId;
        pktInfoUnexpectedPackageDailyRewardReq.dayNumber_ = (Byte)dayNumber;
        
        if (_netflag)
        {
            AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqUnexpectedPackageDailyReward, pktInfoUnexpectedPackageDailyRewardReq, ReceiveCallBack);
            SendProtocolData();
        }
    }

    //서포터 진형 즐겨찾기
    public void Send_ReqUserCardFormationFavi(int tableId, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
            return;

        if(!CheckConnect())
            return;

        PktInfoTIDList pktTIDList = new PktInfoTIDList();
        pktTIDList.tids_ = new List<uint>();

        for (int i = 0; i < _CardFormationFavorList.Count; i++)
        {
            pktTIDList.tids_.Add((uint)_CardFormationFavorList[i]);
        }

        if (_CardFormationFavorList.Contains(tableId))
            pktTIDList.tids_.Remove((uint)tableId);
        else
            pktTIDList.tids_.Add((uint)tableId);

        if (_netflag)
        {
            AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqUserCardFormationFavi, pktTIDList, ReceiveCallBack);
            SendProtocolData();
        }
    }

    public void Send_ReqResetSecretCntChar(List<long> charUidList, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        if (!CheckConnect())
        {
            return;
        }

        PktInfoUIDList pkt = new PktInfoUIDList();
        pkt.uids_ = new List<ulong>();
        foreach (long cuid in charUidList)
        {
            pkt.uids_.Add((ulong)cuid);
        }

        if (_netflag)
        {
            AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqResetSecretCntChar, pkt, ReceiveCallBack);
            SendProtocolData();
        }
    }

	public void Send_ReqAccountDelete(OnReceiveCallBack ReceiveCallBack)
	{
		if (IsProtocolData())
		{
			return;
		}

		if (!CheckConnect())
		{
			return;
		}

		if (_netflag)
		{
			AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqAccountDelete, null, ReceiveCallBack);
			SendProtocolData();
		}
		else
		{
		}
	}

    public void Send_ReqBingoEventReward(int groupId, int no, List<int> rewardLineCount, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        if (!CheckConnect())
        {
            return;
        }

		PktInfoUInt8List rwdLines_ = new PktInfoUInt8List();
		rwdLines_.vals_ = new List<byte>();
		for (int i=0; i<rewardLineCount.Count; ++i)
		{
			rwdLines_.vals_.Add((byte)rewardLineCount[i]);
		}
        PktInfoBingoEventRewardReq pktInfoBingoEventRewardReq = new PktInfoBingoEventRewardReq()
        {
            groupID = (uint)groupId,
            no_ = (byte)no,
            rwdLine_ = rwdLines_,
        };
        
        if (_netflag)
        {
            AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqBingoEventReward, pktInfoBingoEventRewardReq, ReceiveCallBack);
            SendProtocolData();
        }
    }

    public void Send_ReqBingoNextOpen(int groupId, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            AddProtocolData(ePacketType.Lobby, LobbyC2S.Common.ReqBingoNextOpen, (uint)groupId, ReceiveCallBack);
            SendProtocolData();
        }
    }

    public void Send_ReqRewardTakeAchieveEvent(List<int> tidList, OnReceiveCallBack receiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        if (!CheckConnect())
        {
            return;
        }

        PktInfoTIDList pktInfoTIDList = new PktInfoTIDList();
        pktInfoTIDList.tids_ = new List<uint>();
        foreach(int tid in tidList)
        {
            pktInfoTIDList.tids_.Add((uint)tid);
        }

        if (_netflag)
        {
            AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqRewardTakeAchieveEvent, pktInfoTIDList, receiveCallBack);
            SendProtocolData();
        }
    }

    public void Send_ReqGetUserPresetList(ePresetKind presetKind, long cuid, OnReceiveCallBack receiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        if (!CheckConnect())
        {
            return;
        }

        PktInfoPresetCommon pktInfoPresetCommon = new PktInfoPresetCommon();
        pktInfoPresetCommon.kind_ = presetKind;
        pktInfoPresetCommon.cuid_ = (ulong)cuid;
        pktInfoPresetCommon.name_ = new PktInfoStr();

        if (_netflag)
        {
            AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqGetUserPresetList, pktInfoPresetCommon, receiveCallBack);
            SendProtocolData();
        }
    }

    public void Send_ReqAddOrUpdateUserPreset(ePresetKind presetKind, long cuid, int slotNum, OnReceiveCallBack receiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        if (!CheckConnect())
        {
            return;
        }

        PktAddOrUpdatePreset pktAddOrUpdate = new PktAddOrUpdatePreset();
        pktAddOrUpdate.kind_ = presetKind;
        pktAddOrUpdate.cuid_ = (ulong)cuid;
        pktAddOrUpdate.slotNum_ = (byte)slotNum;

        if (_netflag)
        {
            AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqAddOrUpdateUserPreset, pktAddOrUpdate, receiveCallBack);
            SendProtocolData();
        }
    }

    public void Send_ReqUserPresetLoad(ePresetKind presetKind, int slotNum, long cuid, OnReceiveCallBack receiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        if (!CheckConnect())
        {
            return;
        }

        PktInfoPresetCommon pktInfoPresetCommon = new PktInfoPresetCommon();
        pktInfoPresetCommon.kind_ = presetKind;
        pktInfoPresetCommon.cuid_ = (ulong)cuid;
        pktInfoPresetCommon.slotNum_ = (byte)slotNum;
        pktInfoPresetCommon.name_ = new PktInfoStr();

        if (_netflag)
        {
            AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqUserPresetLoad, pktInfoPresetCommon, receiveCallBack);
            SendProtocolData();
        }
    }

    public void Send_ReqUserPresetChangeName(ePresetKind presetKind, int slotNum, long cuid, string presetName, OnReceiveCallBack receiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        if (!CheckConnect())
        {
            return;
        }

        PktInfoPresetCommon pktInfoPresetCommon = new PktInfoPresetCommon();
        pktInfoPresetCommon.kind_ = presetKind;
        pktInfoPresetCommon.cuid_ = (ulong)cuid;
        pktInfoPresetCommon.slotNum_ = (byte)slotNum;
        pktInfoPresetCommon.name_ = new PktInfoStr() { str_ = presetName };

        if (_netflag)
        {
            AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqUserPresetChangeName, pktInfoPresetCommon, receiveCallBack);
            SendProtocolData();
        }
    }

    public void Send_ReqInitRaidSeasonData( OnReceiveCallBack callBack ) {
        if( IsProtocolData() ) {
            return;
        }

        AddProtocolData( ePacketType.Global, GlobalC2S.Common.ReqInitRaidSeasonData, null, callBack );

        if( !CheckConnect() || !_netflag ) {
            return;
        }

        SendProtocolData();
    }

    public void Send_ReqRaidRankingList( OnReceiveCallBack callBack ) {
        if( IsProtocolData() ) {
            return;
        }

        PktInfoRaidRankingHeader pkt = new PktInfoRaidRankingHeader();
        pkt.infos_ = new List<PktInfoRaidRankingHeader.Piece>();

        GameTable.Stage.Param stageParam = _gametable.FindStage( x => x.StageType == (int)eSTAGETYPE.STAGE_RAID && 
                                                                      x.TypeValue == _serverdata.RaidCurrentSeason );

        if( RaidRankDataList.Count == 0 ) {
            RaidRankDataList.Capacity = RaidUserData.CurStep;

            for( int i = 1; i <= RaidUserData.CurStep; i++ ) {
                PktInfoRaidRankingHeader.Piece piece = new PktInfoRaidRankingHeader.Piece();

                piece.stageID_ = (UInt32)stageParam.ID;
                piece.updateTM_ = 0;
                piece.raidLevel_ = (ushort)i;

                pkt.infos_.Add( piece );

                RaidRankDataList.Add( new RaidRankData( piece ) );
            }
		}
		else {
            if( RaidRankDataList.Count < RaidUserData.CurStep ) {
                RaidRankDataList.Capacity = RaidUserData.CurStep;
                int n = RaidRankDataList.Count;

                for( int i = 1; i <= RaidUserData.CurStep - n; i++ ) {
                    PktInfoRaidRankingHeader.Piece piece = new PktInfoRaidRankingHeader.Piece();

                    piece.stageID_ = (UInt32)stageParam.ID;
                    piece.updateTM_ = 0;
                    piece.raidLevel_ = (ushort)( n + i );

                    RaidRankDataList.Add( new RaidRankData( piece ) );
                }
			}

            for( int i = 0; i < RaidRankDataList.Count; i++ ) {
                PktInfoRaidRankingHeader.Piece piece = new PktInfoRaidRankingHeader.Piece();

                piece.stageID_ = (UInt32)RaidRankDataList[i].StageTableID;
                piece.updateTM_ = (UInt64)RaidRankDataList[i].UpdateTM;
                piece.raidLevel_ = (ushort)RaidRankDataList[i].Step;

                pkt.infos_.Add( piece );
            }
        }

        AddProtocolData( ePacketType.Global, GlobalC2S.Common.ReqRaidRankingList, pkt, callBack );

        if( !CheckConnect() || !_netflag ) {
            return;
        }

        SendProtocolData();
    }

    public void Send_ReqRaidFirstRankingList( OnReceiveCallBack callBack ) {
        if( IsProtocolData() ) {
            return;
        }

        PktInfoRaidRankingHeader pkt = new PktInfoRaidRankingHeader();
        pkt.infos_ = new List<PktInfoRaidRankingHeader.Piece>();

        GameTable.Stage.Param stageParam = _gametable.FindStage( x => x.StageType == (int)eSTAGETYPE.STAGE_RAID && x.TypeValue == _serverdata.RaidCurrentSeason );

        for( int i = 1; i <= RaidUserData.CurStep; i++ ) {
            PktInfoRaidRankingHeader.Piece piece = new PktInfoRaidRankingHeader.Piece();

            piece.stageID_ = (UInt32)stageParam.ID;
            piece.updateTM_ = 0;
            piece.raidLevel_ = (ushort)i;

            pkt.infos_.Add( piece );
        }

        AddProtocolData( ePacketType.Global, GlobalC2S.Common.ReqRaidFirstRankingList, pkt, callBack );

        if( !CheckConnect() || !_netflag ) {
            return;
        }

        SendProtocolData();
    }

    public void Send_ReqRaidRankerDetail( int stageTid, long uuid, int step, OnReceiveCallBack callBack ) {
        if( IsProtocolData() ) {
            return;
        }

        PktInfoRaidRankerDetailReq pkt = new PktInfoRaidRankerDetailReq();
        pkt.stageID_ = (UInt32)stageTid;
        pkt.uuid_ = (UInt64)uuid;
        pkt.raidLevel_ = (ushort)step;

        AddProtocolData( ePacketType.Global, GlobalC2S.Common.ReqRaidRankerDetail, pkt, callBack );

        if( !CheckConnect() ) {
            return;
        }

        if( _netflag ) {
            SendProtocolData();
        }
    }

    public void Send_ReqFirstRaidRankerDetail( int stageTid, long uuid, int step, OnReceiveCallBack callBack ) {
        if( IsProtocolData() ) {
            return;
        }

        PktInfoRaidRankerDetailReq pkt = new PktInfoRaidRankerDetailReq();
        pkt.stageID_ = (UInt32)stageTid;
        pkt.uuid_ = (UInt64)uuid;
        pkt.raidLevel_ = (ushort)step;

        AddProtocolData( ePacketType.Global, GlobalC2S.Common.ReqRaidFirstRankerDetail, pkt, callBack );

        if( !CheckConnect() ) {
            return;
        }

        if( _netflag ) {
            SendProtocolData();
        }
    }

    public void Send_ReqSetRaidTeam( List<long> charUidList, uint cardFormationId, OnReceiveCallBack callBack ) {
        if( IsProtocolData() ) {
            return;
        }

        PktInfoUserRaidTeam pkt = new PktInfoUserRaidTeam();
        
        pkt.CUIDs_ = new ulong[charUidList.Count];
        for( int i = 0; i < pkt.CUIDs_.Length; i++ ) {
            pkt.CUIDs_[i] = (ulong)charUidList[i];
        }

        pkt.cardFrmtID_ = cardFormationId;

        AddProtocolData( ePacketType.Global, GlobalC2S.Common.ReqSetRaidTeam, pkt, callBack );

        if( !CheckConnect() ) {
            return;
        }

        if( _netflag ) {
            SendProtocolData();
        }
    }

    public void Send_ReqRaidHpRestore( long charUid, List<ItemData> itemDataList, OnReceiveCallBack callBack ) {
        if( IsProtocolData() ) {
            return;
        }

        PktInfoRaidRestoreHPReq pkt = new PktInfoRaidRestoreHPReq();
        pkt.cuid_ = (ulong)charUid;

        pkt.items_ = new PktInfoItemCnt();
        pkt.items_.infos_ = new List<PktInfoItemCnt.Piece>( itemDataList.Count );

        for( int i = 0; i < itemDataList.Count; i++ ) {
            PktInfoItemCnt.Piece piece = new PktInfoItemCnt.Piece();
            piece.uid_ = (ulong)itemDataList[i].ItemUID;
            piece.cnt_ = 1;

            pkt.items_.infos_.Add( piece );
        }

        AddProtocolData( ePacketType.Global, GlobalC2S.Common.ReqRaidHPRestore, pkt, callBack );

        if( !CheckConnect() || !_netflag ) {
            return;
        }

        SendProtocolData();
    }

    public void Send_ReqRaidStageStart( int stageTid, int step, long atkBuffItemUid, long hpBuffItemUid, OnReceiveCallBack callBack ) {
        if( IsProtocolData() ) {
            return;
        }

        IsReqStageEndReConnect = false;

        UIValue.Instance.SetValue( UIValue.EParamType.LastPlayStageID, stageTid );
        SeleteCharUID = RaidUserData.CharUidList[0];
        SelecteStageTableId = stageTid;

        PktInfoStageGameStartReq pkt = new PktInfoStageGameStartReq();
        pkt.secretSubCuids_ = new PktInfoUIDList();
        pkt.secretSubCuids_.uids_ = new List<ulong>();
        pkt.stageTID_ = (UInt32)stageTid;
        pkt.raidLevel_ = (ushort)step;
        pkt.cuid_ = (UInt64)SeleteCharUID;

        pkt.raidCUIDs_ = new ulong[RaidUserData.CharUidList.Count];
        for( int i = 0; i < pkt.raidCUIDs_.Length; i++ ) {
            pkt.raidCUIDs_[i] = (ulong)RaidUserData.CharUidList[i];
        }

        pkt.useItemUIDs_ = new List<ulong>();
        pkt.useItemUIDs_.Capacity = 2;

        if( atkBuffItemUid > 0 ) {
            pkt.useItemUIDs_.Add( (ulong)atkBuffItemUid );
		}

        if( hpBuffItemUid > 0 ) {
            pkt.useItemUIDs_.Add( (ulong)hpBuffItemUid );
        }

        AddProtocolData( ePacketType.Global, GlobalC2S.Common.ReqStageStart, pkt, callBack );

        if( !CheckConnect() )
            return;

        if( _netflag ) {
            SendProtocolData();
        }
    }

    public void Send_ReqRaidStageEnd( int clearTime, int goldCnt, int box, OnReceiveCallBack callBack ) {
        if( IsProtocolData() ) {
            return;
        }

        GameInfo.Instance.IsNewRaidReocrd = false;

        PktInfoStageGameResultReq pkt = new PktInfoStageGameResultReq();
		pkt.playData_ = new PktInfoIngamePlayData();
        pkt.playData_.clearTime_Ms = (UInt32)clearTime;
        pkt.dropGoldItemCnt_ = (ushort)goldCnt;
        pkt.nTakeBoxCnt_ = (byte)box;

        pkt.raidCharHP_ = new ushort[World.Instance.ListPlayer.Count];

        float maxDamage = 0.0f;
        float maxDps = 0.0f;
        float avgDps = 0.0f;
        float suffDmg = 0.0f;
        float totalRecovery = 0.0f;
        float validRecovery = 0.0f;

        for ( int i = 0; i < RaidUserData.CharUidList.Count; i++ ) {
            Player player = World.Instance.ListPlayer.Find( x => x.charData.CUID == RaidUserData.CharUidList[i] );
            pkt.raidCharHP_[i] = (ushort)( ( player.curHp / player.maxHp ) * 10000.0f );

            if( player.MaxAttackPower > maxDamage ) {
                maxDamage = player.MaxAttackPower;
            }

            avgDps += player.DpsAverage;
            maxDps += player.MaxDamagePerSecond;
            suffDmg += player.Damaged;
            totalRecovery += player.HealedWithOverHeal;
            validRecovery += player.Healed;
        }

        pkt.playData_.maxDamage_ = (UInt32)maxDamage;
        pkt.playData_.teamPower_ = (uint)GameSupport.GetArenaTeamPower( eContentsPosKind.RAID );
        pkt.playData_.avgDPS_ = (UInt32)avgDps;
        pkt.playData_.maxDPS_ = (UInt32)maxDps;
        pkt.playData_.sufferDamage_ = (UInt32)suffDmg;
        pkt.playData_.totalRecovery_ = (UInt32)totalRecovery;
        pkt.playData_.validRecovery_ = (UInt32)validRecovery;
        pkt.playData_.armoryRatio_ = GameSupport.GetTotalWeaponDepotEffectValue();

        AddProtocolData( ePacketType.Global, GlobalC2S.Common.ReqStageEnd, pkt, callBack );

        if( !CheckConnect() ) {
            IsReqStageEndReConnect = true;
            return;
        }

        if( _netflag ) {
            SendProtocolData();
        }
    }

    public void Send_ReqRaidStageEndFail( int clearTime, OnReceiveCallBack ReceiveCallBack ) {
        if( IsProtocolData() ) {
            return;
        }

        bStageFailure = true;
        
        PktInfoStageGameEndFail pkt = new PktInfoStageGameEndFail();
        pkt.playTime_Ms_ = (uint)clearTime;

        pkt.raidCUIDs_ = new ulong[3];
        for( int i = 0; i < pkt.raidCUIDs_.Length; i++ ) {
            pkt.raidCUIDs_[i] = (ulong)World.Instance.ListPlayer[i].charData.CUID;
		}

        pkt.raidCharHPs_ = new ushort[3];
        for( int i = 0; i < pkt.raidCharHPs_.Length; i++ ) {
            pkt.raidCharHPs_[i] = (ushort)( ( World.Instance.ListPlayer[i].curHp / World.Instance.ListPlayer[i].maxHp ) * 10000.0f );
		}

        AddProtocolData( ePacketType.Global, GlobalC2S.Common.ReqStageEndFail, pkt, ReceiveCallBack );

        if( !CheckConnect() ) {
            return;
        }

        if( _netflag ) {
            SendProtocolData();
        }
    }

    public void Send_ReqRaidStageGiveUp( OnReceiveCallBack ReceiveCallBack ) {
        if( IsProtocolData() ) {
            return;
        }

        PktInfoRaidStageDrop pkt = new PktInfoRaidStageDrop();

        pkt.raidCUIDs_ = new ulong[3];
        for( int i = 0; i < pkt.raidCUIDs_.Length; i++ ) {
            pkt.raidCUIDs_[i] = (ulong)World.Instance.ListPlayer[i].charData.CUID;
        }

        pkt.raidCharHPs_ = new ushort[3];
        for( int i = 0; i < pkt.raidCharHPs_.Length; i++ ) {
            pkt.raidCharHPs_[i] = (ushort)( ( World.Instance.ListPlayer[i].curHp / World.Instance.ListPlayer[i].maxHp ) * 10000.0f );
        }

        AddProtocolData( ePacketType.Global, GlobalC2S.Common.ReqRaidStageDrop, pkt, ReceiveCallBack );

        if( !CheckConnect() ) {
            return;
        }

        if( _netflag ) {
            SendProtocolData();
        }
    }

    public void Send_ReqCharLvUnexpectedPackageHardOpen(long cuid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCharLvUnexpectedPackageHardOpen, cuid, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클(길드) 개설 요청
    /// </summary>
    /// <param name="name"></param>
    /// <param name="comment"></param>
    /// <param name="lang"></param>
    /// <param name="state"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleOpen(string name, string comment, eLANGUAGE lang, bool state, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        PktInfoCircleOpenReq pkt = new PktInfoCircleOpenReq()
        {
            name_ = new PktInfoStr(),
            comment_ = new PktInfoStr(),
            lang_ = lang,
            suggestAnotherLang_ = state,
        };

        pkt.name_.str_ = name;
        pkt.comment_.str_ = comment;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleOpen, pkt, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 추천 리스트 요청
    /// </summary>
    /// <param name="lang"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqSuggestCircleList(eLANGUAGE lang, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqSuggestCircleList, (long)lang, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 가입 요청
    /// </summary>
    /// <param name="cuid"></param>
    /// <param name="lang"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleJoin(long cuid, eLANGUAGE lang, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        PktInfoCircleJoinReq pkt = new PktInfoCircleJoinReq()
        {
            joinReqCircleID_ = (ulong)cuid,
            suggestLang_ = lang,
        };

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleJoin, pkt, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 가입신청 취소 요청
    /// </summary>
    /// <param name="cuid"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleJoinCancel(long cuid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleJoinCancel, cuid, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 로비정보 요청
    /// </summary>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleLobbyInfo(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleLobbyInfo, null, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 탈퇴요청
    /// </summary>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleWithdrawal(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleWithdrawal, null, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 해산요청
    /// </summary>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleDisperse(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleDisperse, null, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 유저정보 리스트 요청 (가입유저 및 가입 대기유저 정보)
    /// </summary>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqGetCircleUserList(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqGetCircleUserList, null, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 가입대기 유저 상태 변경요청 (서클 특정 권한 유저의 요청)
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="state"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleChangeStateJoinWaitUser(long uuid, bool state, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        PktInfoChangeStateJoinWaitUser pkt = new PktInfoChangeStateJoinWaitUser()
        {
            tagetUUID_ = (ulong)uuid,
            state_ = state,
        };

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleChangeStateJoinWaitUser, pkt, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 유저 추방 요청
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleUserKick(long uuid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleUserKick, uuid, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 유저 권한변경 요청
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="level"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleChangeAuthLevel(long uuid, eCircleAuthLevel level, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        PktInfoCircleChangeAuthority pkt = new PktInfoCircleChangeAuthority()
        {
            targetUser_ = new PktInfoCircleChangeAuthority.Piece()
            {
                uuid_ = (ulong)uuid,
                authLevel_ = level,
            },

            affectedUser_ = new PktInfoCircleChangeAuthority.Piece(),
        };

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleChangeAuthLevel, pkt, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 마크변경 요청
    /// </summary>
    /// <param name="markTid"></param>
    /// <param name="flagTid"></param>
    /// <param name="colorTid"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleChangeMark(int markTid, int flagTid, int colorTid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        PktInfoCircleMarkSet pkt = new PktInfoCircleMarkSet()
        {
            markTID_ = (uint)markTid,
            flagTID_ = (uint)flagTid,
            colorTID_ = (uint)colorTid,
        };

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleChangeMark, pkt, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 이름변경 요청
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleChangeName(string name, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        PktInfoCircleChangeName pkt = new PktInfoCircleChangeName()
        {
            changeName_ = new PktInfoStr(),
        };

        pkt.changeName_.str_ = name;

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleChangeName, pkt, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 소개문 변경요청
    /// </summary>
    /// <param name="comment"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleChangeComment(string comment, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        PktInfoStr pkt = new PktInfoStr()
        {
            str_ = comment,
        };

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleChangeComment, pkt, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 주사용 언어 변경요청
    /// </summary>
    /// <param name="lang"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleChangeMainLanguage(eLANGUAGE lang, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleChangeMainLanguage, (long)lang, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 주사용 언어 외 가입옵션 변경요청
    /// </summary>
    /// <param name="state"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleChangeSuggestAnotherLangOpt(bool state, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleChangeSuggestAnotherLangOpt, (long)(state ? eToggleType.ServerTypeOn : eToggleType.ServerTypeOff), ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 출석체크 요청
    /// </summary>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleAttendance(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleAttendance, null, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 마크 구매요청
    /// </summary>
    /// <param name="tid"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleBuyMarkItem(int tid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleBuyMarkItem, tid, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 채팅 전송요청
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="stampTid"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleChatSend(string msg, int stampTid, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        PktInfoCircleChat.Piece pkt = new PktInfoCircleChat.Piece()
        {
            nickName_ = new PktInfoStr(),
            msg_ = new PktInfoStr() { str_ = msg, },
            stampID_ = (uint)stampTid,
            sendTm_ = new PktInfoTime(),
        };

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleChatSend, pkt, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData(bwait: false);
        }
    }

    /// <summary>
    /// 서클 소유한 마크 리스트 요청
    /// </summary>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleGetMarkList(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleGetMarkList, null, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 검색요청
    /// </summary>
    /// <param name="ccuid"></param>
    /// <param name="name"></param>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleSearch(long ccuid, string name, OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        PktInfoCircleSearch pkt = new PktInfoCircleSearch()
        {
            circleID_ = (ulong)ccuid,
            circleName_ = new PktInfoStr() { str_ = name },
        };

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleSearch, pkt, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

    /// <summary>
    /// 서클 채팅 리스트 요청
    /// </summary>
    /// <param name="ReceiveCallBack"></param>
    public void Send_ReqCircleChatList(OnReceiveCallBack ReceiveCallBack)
    {
        if (IsProtocolData())
        {
            return;
        }

        AddProtocolData(ePacketType.Global, GlobalC2S.Common.ReqCircleChatList, null, ReceiveCallBack);

        if (!CheckConnect())
        {
            return;
        }

        if (_netflag)
        {
            SendProtocolData();
        }
    }

	/// <summary>
	/// 캐릭터 친밀도 버프 변경 요청
	/// </summary>
	/// <param name="cUID"></param>
	/// <param name="selectIndex"></param>
	/// <param name="onReceiveCallBack"></param>
	public void Send_ReqChangePreferenceNum( long cUID, int selectIndex, OnReceiveCallBack onReceiveCallBack ) {
		if ( IsProtocolData() ) {
			return;
		}

		PktInfoUIDList pktInfoUIDList = new PktInfoUIDList() {
			uids_ = new List<ulong>(),
		};

		int prevIndex = -1;
		ulong prevCUID = 0;
		for ( int i = 0; i < _userdata.ArrFavorBuffCharUid.Length; i++ ) {
			long cuid = _userdata.ArrFavorBuffCharUid[i];
			if ( 0 < cUID ) {
				if ( cuid == cUID ) {
					prevIndex = i;
				}

				if ( selectIndex == i ) {
					prevCUID = (ulong)cuid;
				}
			}
			pktInfoUIDList.uids_.Add( (ulong)cuid );
		}

		if ( 0 <= prevIndex ) {
			pktInfoUIDList.uids_.RemoveAt( prevIndex );
			pktInfoUIDList.uids_.Insert( prevIndex, prevCUID );
		}

		pktInfoUIDList.uids_.RemoveAt( selectIndex );
		pktInfoUIDList.uids_.Insert( selectIndex, (ulong)cUID );

		AddProtocolData( ePacketType.Global, GlobalC2S.Common.ReqChangePreferenceNum, pktInfoUIDList, onReceiveCallBack );

		if ( !CheckConnect() ) {
			return;
		}

		if ( _netflag ) {
			SendProtocolData();
		}
	}
}
