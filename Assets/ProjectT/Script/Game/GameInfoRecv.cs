using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nettention.Proud;
using System.Linq;

public partial class GameInfo : FMonoSingleton<GameInfo> {
    //--------------------------------------------------------------------------------------------------------------
    //
    //  recv
    //
    //--------------------------------------------------------------------------------------------------------------
    // login
    bool RecvAckClientSecurityInfo( HostID remote, RmiContext rmiContext, PktInfoClientSecurityAck _pkt ) {
        RecvProtocolData( _pkt );
        return true;
    }
    bool RecvAckClientSecurityVerify( HostID remote, RmiContext rmiContext, PktInfoClientSecurityVerifyAck _pkt ) {
        SetPktLoginCommonInfo( _pkt.svrInfo_ );

        RecvProtocolData( _pkt );
        return true;
    }
    bool RecvNotiFirstLogin( HostID remote, RmiContext rmiContext, PktInfoUserDataChange _pktInfo ) {
        Debug.LogWarning( string.Format( "~~~~~ RecvNotiFirstLogin ~~~~~" ) );

        SetPktUserDataChange( _pktInfo );
        SetPktInfluenceData( _pktInfo.mission_.influ_ );
        ApplyPktMissionDailyData( _pktInfo.mission_.daily_ ); //신규/복귀 유저 이벤트(Daily)

#if !DISABLESTEAMWORKS
        //Steam First Login (Auto Link)
        PktInfoAccountLinkInfo pktInfo = new PktInfoAccountLinkInfo();
        pktInfo.linkID_ = new PktInfoStr();
        pktInfo.linkID_.str_ = AppMgr.Instance.SteamId.ToString();
        pktInfo.type_ = eAccountType.STEAM;
        pktInfo.updateFlag_ = true;

        NETStatic.PktGbl.ReqAddLinkAccountAuth(pktInfo);
#else
        OnReceiveCallBack callback = GetCallBackList("Send_ReqLogin");  //<-- 직접사용
        if( callback != null ) {
            if( _charlist.Count <= 0 ) {
                //Send_ReqAddCharacter(1, callback);
                callback.Invoke( 0, null );
            }
            else {
                SvrConnect_LoginToLobby(); //callbackdo(0, loginUserInfo);
            }

        }
#endif
        return true;
    }

    bool RecvNotiUserLoginChangeData( HostID remote, RmiContext rmiContext, PktInfoUserDataChange _pktInfo ) {
        Debug.LogWarning( string.Format( "~~~~~ RecvNotiUserLoginChangeData ~~~~~" ) );
        SetPktUserDataChange( _pktInfo );

        OnReceiveCallBack callback = GetCallBackList("Send_ReqLogin");  //<-- 직접사용
        if( callback != null ) {

            if( _charlist.Count <= 0 ) {
                //Send_ReqAddCharacter(1, callback);
                callback.Invoke( 0, null );
            }
            else {
                SvrConnect_LoginToLobby(); //callback(0, loginUserInfo);
            }
        }

        return true;
    }
    bool RecvAckErrLoginUserBlock( HostID remote, RmiContext rmiContext, PktInfoTime _blockTime ) {
        //Debug.LogWarning(string.Format("~~~~~ RecvAckErrLoginUserBlock ~~~~~"));
        MessagePopup.OK( eTEXTID.TITLE_NOTICE, string.Format( FLocalizeString.Instance.GetText( 3150 ), _blockTime.GetTime() ), () => Application.Quit() );
        return true;
    }

    bool RecvAckLoginAndUserInfo( HostID remote, RmiContext rmiContext, PktInfoLoginCommon _pktInfo, PktInfoDetailUser loginUserInfo ) {
        Log.Show( loginUserInfo.userInfo_.coLangCode_.country_.str_ + " / " + loginUserInfo.userInfo_.coLangCode_.lang_.str_, Log.ColorType.Red );

        RaidRankDataList.Clear();
        _usermarklist.Clear();
        _charPresetDataDict.Clear();
        _questPresetDatas = null;
        _arenaPresetDatas = null;
        _raidPresetDatas = null;
        _arenaTowerPresetDatas = null;

        SetPktLoginCommonInfo( _pktInfo );

        _userdata.SetPktData( loginUserInfo.chatStamp_ );
        _userdata.SetPktData( loginUserInfo.userInfo_ );
        _userdata.SetPktData( loginUserInfo.goodsInfo_ );
        _userdata.SetPktData( loginUserInfo.bingoEventList_ );
        _userdata.SetPktEvtLoginData( loginUserInfo.events_.login_ );

        SetPktAchieveEvents( loginUserInfo.achieveEventList_ );
        SetPktFacilityList( loginUserInfo.facilityes_ );

        //EventMode
        SetPktEventSetDataList( loginUserInfo.events_.reward_ );

        //SetPktEventResetRewardDataList(loginUserInfo.events_.reward_);
        SetPktLobbyTheme( loginUserInfo.lobbyTheme_ );
        SetPktCostumeList( loginUserInfo.costumes_ );
        SetPktWeaponList( loginUserInfo.weapons_ );
        SetPktItemList( loginUserInfo.items_ );
        SetPktGemList( loginUserInfo.gems_ );
        SetPktStageClearList( loginUserInfo.stageClears_ );
        SetPktTimeAttackClearList( loginUserInfo.timeRecord_ );
        SetPktWeaponBookList( loginUserInfo.weaponBooks_ );
        SetPktCardBookList( loginUserInfo.cardBooks_ );
        SetPktMonsterBookList( loginUserInfo.monsterBooks_ );
        SetPktCharList( loginUserInfo.charInfo_ );
        SetPktCharSkillList( loginUserInfo.charSkls_ );
        SetPktCardList( loginUserInfo.cards_ );
        SetPktSaleStore( loginUserInfo.storeSales_ );
        SetPktAchieves( loginUserInfo.achieves_ );
        SetPktInfoRoomPurchase( loginUserInfo.roomPackage_.purchase_ );
        SetPktInfoRoomThemeSlot( loginUserInfo.roomPackage_.themeSlot_ );
        SetPktInfoRoomFigureSlotAndClear( loginUserInfo.roomPackage_.figureSlot_ );

        SetPktInfoMonthlyFee( loginUserInfo.monFees_ );
        SetPktInfoBuffEffect( loginUserInfo.effs_ );

        //  메일의 총보유 갯수(로그인시 갯수 셋팅만)
        SetPktMailCount( loginUserInfo.mails_.maxCnt_ );

        //Arena
        SetPktArenaData( loginUserInfo.arena_ );
        SetPktBadgeList( loginUserInfo.badges_ );

        //Pass
        SetPktPassSetData( loginUserInfo.pass_ );
        SetPktPassMissionData( loginUserInfo.mission_.pass_ );

        //무기고
        SetPktInfoWpnDepotSet( loginUserInfo.wnpDepot_ );

        //Dispatch
        SetPktDispatch( loginUserInfo.dispatches_ );

        SetPktArenaTowerData( loginUserInfo.arenaTower_ );

        SetPktAwakenSkillData( loginUserInfo.userSkls_ );

        //서버달성 이벤트
        SetPktInfluenceData( loginUserInfo.mission_.influ_ );

        //신규/복귀 유저 이벤트(Daily)
        ApplyPktMissionDailyData( loginUserInfo.mission_.daily_ );

        //Server Relocate
        SetPktInfoRelocateUserInfo( loginUserInfo.relocate_ );

        //로테이션 가챠
        SetPktUserRotaionGacha( loginUserInfo.rgacha_ );

        // Package Event
        SetPktInfoUnexpectedPackage( loginUserInfo.unexpectedPackage_ );

        SetCardFormationFavor( loginUserInfo.cardFormaFavi_ );

        PlayerPrefs.SetString( "User_AccountUUID", loginUserInfo.userInfo_.uuid_.ToString() );

        //서버 정보
        _serverdata.Version = 1;



        //  서버시간 차이값 기억

        //_serverdata.LoginTime = GameSupport.GetLocalTimeByServerTime(_pktInfo.svrTime_.GetTime());
        WeekMissionData.SetPktData( loginUserInfo.mission_.weekly_ );
        Debug.LogWarning( string.Format( "###주간 미션###   주간 미션 갱신 시간은 {0}", WeekMissionData.fWeekMissionResetDate.ToString() ) );

        SetPktGllaMissionData( loginUserInfo.mission_.guerrilla_ );

        //레드닷 처리
        LoadNewCard();
        LoadNewWeapon();
        LoadNewGem();
        LoadNewItem();
        LoadNewIcon();
        LoadNewBadge();

        //스페셜(바이크)모드 남은시간 처리
        _userdata.LastPlaySpecialModeTime = GameSupport.GetLocalTimeByServerTime( loginUserInfo.stageInfo_.special_.nextTime_.GetTime() );
        _userdata.NextPlaySpecialModeTableID = (int)loginUserInfo.stageInfo_.special_.tableID_;
        _userdata.ArenaPrologueValue = (int)loginUserInfo.stageInfo_.arenaProc_;

        // 캐릭터 전투력 계산
        for( int i = 0; i < _charlist.Count; i++ ) {
            _charlist[i].CombatPower = GameSupport.GetCombatPower( _charlist[i], eWeaponSlot.MAIN, eContentsPosKind._NONE_, null, null );
        }

        // 레이드
        RaidUserData.SetPktData( loginUserInfo.raid_ );
        RaidSecretStoreChangeRemainTime = GameSupport.GetLocalTimeByServerTime( loginUserInfo.raidStore_.resetTM_.GetTime() );

        // 서클
        CircleReqChatList = false;

        return true;
    }
    //--------------------------------------------------------------------------------------------------------------
    // Global
    //--------------------------------------------------------------------------------------------------------------
    bool RecvAckAccountCode( HostID remote, RmiContext rmiContext, PktInfoStr _code, PktInfoStr _password )//<-
    {
        WaitPopup.Hide();
        _userdata.AccountCode = _code.str_;
        if( _password.str_ == "" || _password.str_ == string.Empty )
            _userdata.PasswordSet = false;
        else
            _userdata.PasswordSet = true;

        OnReceiveCallBack callback = GetCallBackList("Send_ReqAccountCode");//<-- 직접사용
        if( callback != null )
            callback( 0, null );

        RecvProtocolData( null );
        return true;
    }
    bool RecvAckAccountSetPassword( HostID remote, RmiContext rmiContext ) {
        WaitPopup.Hide();

        if( !_userdata.PasswordSet ) {
            Debug.LogWarningFormat( " _userdata.PasswordSet : {0} ", _userdata.PasswordSet );
            _userdata.PasswordSet = true;
        }

        OnReceiveCallBack callback = GetCallBackList("Send_ReqAccountSetPassword");//<-- 직접사용
        if( callback != null )
            callback( 0, null );

        RecvProtocolData( null );
        return true;
    }

    bool RecvAckAccountCodeReward( HostID remote, RmiContext rmiContext, PktInfoGoods _pktInfo ) {
        _userdata.AccountCodeReward = true;

        //보상은 항상 대마석입니다.(총보유량)
        if( _pktInfo.infos_ != null && _pktInfo.infos_.Count > 0 )
            _userdata.SetGoods( eGOODSTYPE.CASH, (long)_pktInfo.infos_[0].value_ );


        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckAccountLinkReward( HostID remote, RmiContext rmiContext, PktInfoGoods _pktInfo ) {
        _userdata.AccountLinkReward = true;

        //보상은 항상 대마석입니다.(총보유량)
        if( _pktInfo.infos_ != null && _pktInfo.infos_.Count > 0 )
            _userdata.SetGoods( eGOODSTYPE.CASH, (long)_pktInfo.infos_[0].value_ );

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckLinkAccountList( HostID remote, RmiContext rmiContext, PktInfoAccountLinkList _pktInfo ) {
        WaitPopup.Hide();
        _userdata.AccountLinkList.Clear();
        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            var data = _pktInfo.infos_[i];
            _userdata.AccountLinkList.Add( new AccounLinkData( data.type_, data.linkID_.str_ ) );
        }
        OnReceiveCallBack callback = GetCallBackList("Send_ReqLinkAccountList");//<-- 직접사용
        if( callback != null )
            callback( 0, null );

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckAddLinkAccountAuth( HostID remote, RmiContext rmiContext, PktInfoAccountLinkInfo _pktInfo ) {
        OnReceiveCallBack callback = null;
        if( _pktInfo.type_ == eAccountType.STEAM ) // AddLink에 타입이 스팀으로 오는 경우는 스팀 버전에서 최초 로그인 시(RecvNotiFirstLogin)밖에 없음
        {
            callback = GetCallBackList( "Send_ReqLogin" );  //<-- 직접사용
            if( callback != null )
                callback( 0, null );
        }
        else {
            WaitPopup.Hide();
            callback = GetCallBackList( "Send_ReqAddLinkAccountAuth" ); //<-- 직접사용
            if( callback != null )
                callback( 0, null );

            RecvProtocolData( null );
        }

        return true;
    }
    bool RecvAckGetUserInfoFromAccountLink( HostID remote, RmiContext rmiContext, PktInfoUserInfoFromLinkAck _pktInfo )//<-
    {
        WaitPopup.Hide();

        //패치 정보등 전부다 날아가서 주석처리
        //PlayerPrefs.DeleteAll();
        UIValue.Instance.ClearParameter();

    #if !DISABLESTEAMWORKS
        PlayerPrefs.SetString( "User_SteamID", AppMgr.Instance.SteamId.ToString() );
    #endif

		PlayerPrefs.SetString( "User_AccountUUID", _pktInfo.uuid_.ToString() );
        Log.Show( "Send_ReqGetUserInfoFromAccountLink ####" );
        OnReceiveCallBack callback = GetCallBackList("Send_ReqGetUserInfoFromAccountLink");//<-- 직접사용
        if( callback != null )
            callback( 0, null );

        PlayerPrefs.Save();
        return true;
    }
    bool RecvAckPushNotifiTokenSet( HostID remote, RmiContext rmiContext, PktInfoPushNotiSetToken _pktInfo ) {
        PlayerPrefs.SetString( "FCM_PUSH_TOKEN_SERVER", _pktInfo.token_.str_ );

        RecvProtocolData( _pktInfo );
        return true;
    }

    bool RecvAckPing( HostID remote, RmiContext rmiContext, PktInfoTime _pktInfo ) {
        return true;
    }

    bool RecvAckGetTotlaRelocateCntToNotComplete( HostID remote, RmiContext rmiContext, System.UInt32 _pkt ) {
        ServerMigrationCountOnWeek = (int)_pkt;
        RecvProtocolData( (long)_pkt );

        return true;
    }

    bool RecvAckRelocateUserInfoSet( HostID remote, RmiContext rmiContext, PktInfoRelocateUser _pktInfo ) {
        SetPktInfoRelocateUserInfo( _pktInfo );
        RecvProtocolData( _pktInfo );


        return true;
    }
    bool RecvAckRelocateUserComplate( HostID remote, RmiContext rmiContext, PktInfoRelocateUser _pktInfo ) {
        //SetPktInfoRelocateUserInfo(_pktInfo);
        //RecvProtocolData(_pktInfo);

        WaitPopup.Hide();

        PlayerPrefs.SetString( "User_AccountUUID", _pktInfo.uuid_.ToString() );

        OnReceiveCallBack callback = GetCallBackList("Send_ReqRelocateUserComplate");//<-- 직접사용
        if( callback != null )
            callback( 0, null );

        PlayerPrefs.Save();

        return true;
    }

    bool RecvAckRefrashUserInfo( HostID remote, RmiContext rmiContext, PktInfoRefreahUserInfo _pktInfo ) //<--
    {
        _userdata.SetPktData( _pktInfo.nowGoods_ );

        Log.Show( "RecvAckRefrashUserInfo", Log.ColorType.Red );
        for( int i = 0; i < _pktInfo.nowGoods_.goodsValues_.Length; i++ ) {
            Log.Show( i + " / " + _pktInfo.nowGoods_.goodsValues_[i] );
        }

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckReConnectUserInfo( HostID remote, RmiContext rmiContext, PktInfoReconnectUserInfoAck _pktInfo ) //<--
    {
        _userdata.SetPktData( _pktInfo.nowGoods_ );

        WaitPopup.Hide();
        OnReceiveCallBack callback = GetCallBackList("Send_ReqReConnectUserInfo");//<-- 직접사용
        if( callback != null )
            callback( 0, null );
        return true;
    }
    //캐릭터
    bool RecvAckAddCharacter( HostID remote, RmiContext rmiContext, PktInfoAddChar _pktInfo ) {
        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            AddPktCharList( _pktInfo.infos_[i].char_ );
            AddPktCharSkillList( _pktInfo.infos_[i].charSkills_ );
            AddPktCostumeList( _pktInfo.infos_[i].costumes_ );
            AddPktWeaponList( _pktInfo.infos_[i].weapons_ );
            AddPktWeaponBookList( _pktInfo.infos_[i].weaponBooks_ );

            if( 1 == _pktInfo.infos_[i].char_.selNum_ ) {
                _userdata.MainCharUID = (long)_pktInfo.infos_[i].char_.cuid_;
                _userdata.ArrLobbyBgCharUid[0] = _userdata.MainCharUID;
            }
        }

        OnReceiveCallBack callback = GetCallBackList("Send_ReqAddCharacter");
        if( callback != null )
            callback( 0, null );

        //SvrConnect_LoginToLobby(); 
        return true;
    }

    bool RecvAckChangeMainChar( HostID remote, RmiContext rmiContext, PktInfoUIDValue _pkt ) {
        if( !_userdata.SetPktData( _pkt ) ) {
            return false;
        }

        RecvProtocolData( _pkt );

        //var chardata = GetCharData((long)_cuid);
        //if (chardata == null)
        //    return false;

        //_userdata.MainCharUID = (long)_cuid;

        //RecvProtocolData((long)_cuid);
        return true;
    }
    bool RecvAckGradeUpChar( HostID remote, RmiContext rmiContext, PktInfoCharGradeUp _pkt ) {
        var chardata = GetCharData((long)_pkt.cuid_);
        if( chardata == null )
            return false;

        //_userdata.SetPktData(_pktInfo.retGoods_);
        //ApplyPktInfoMaterItem(_pktInfo.maters_);
        ApplyPktInfoMaterItemGoods( _pkt.retItemGoods_ );
        chardata.Grade = _pkt.resultGrade_;

        RecvProtocolData( _pkt );
        return true;
    }

    bool RecvAckSetGradeLvChar( HostID remote, RmiContext rmiContext, PktInfoCharGradeExpLv _pkt ) {
        CharData chardata = GetCharData((long)_pkt.gradeUp_.cuid_);
        if( chardata == null ) {
            return false;
        }

        AddPktUnexpectedPackage( _pkt.pkg_ );
        ApplyPktInfoMaterItemGoods( _pkt.gradeUp_.retItemGoods_ );

        chardata.Grade = _pkt.gradeUp_.resultGrade_;
        chardata.Level = _pkt.expLv_.lv_;
        chardata.Exp = (int)_pkt.expLv_.exp_;

        RecvProtocolData( _pkt );
        return true;
    }

    bool RecvAckSetMainCostumeChar( HostID remote, RmiContext rmiContext, PktInfoCharSetMainCostumeAck _pkt ) {
        var chardata = GetCharData((long)_pkt.skinColor_.cuid_);
        if( chardata == null )
            return false;

        chardata.EquipCostumeID = (int)_pkt.costumeTID_;
        chardata.SetPktSkinColorData( _pkt.skinColor_ );

        RecvProtocolData( _pkt );
        return true;
    }

    bool RecvAckRandomCostumeDyeing( HostID remote, RmiContext rmiContext, PktInfoRandomCostumeDyeing _pktInfo ) {
        ApplyPktInfoMaterItem( _pktInfo.useItem_ );

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckSetCostumeDyeing( HostID remote, RmiContext rmiContext, PktInfoCostume.Piece _pktInfo ) {
        MyDyeingData dyeingData = GetDyeingData((int)_pktInfo.tableID_);
        if( dyeingData != null ) {
            dyeingData.ChangeData( _pktInfo );
        }

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckCostumeDyeingLock( HostID remote, RmiContext rmiContext, PktInfoCostumeDyeingLock _pktInfo ) {
        MyDyeingData dyeingData = GetDyeingData((int)_pktInfo.costumeID_);
        if( dyeingData != null ) {
            dyeingData.LockFlag = _pktInfo.lockFlag_;
        }

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckUserCostumeColor( HostID remote, RmiContext rmiContext, PktInfoUserCostumeColor _pktInfo ) {
        RecvProtocolData( _pktInfo );
        return true;
    }

    bool RecvAckEquipWeaponChar( HostID remote, RmiContext rmiContext, PktInfoCharEquipWeapon _pktInfo ) {
        //!@# 멀티 무기 개선 작업 확인 필요 #@!
        var chardata = GetCharData((long)_pktInfo.skinColor_.cuid_);
        if( chardata == null )
            return false;

        chardata.EquipWeaponUID = (long)_pktInfo.wpns_[(int)eWeaponSlot.MAIN].wpnUID_;
        chardata.EquipWeaponSkinTID = (int)_pktInfo.wpns_[(int)eWeaponSlot.MAIN].wpnSkin_;

        chardata.EquipWeapon2UID = (long)_pktInfo.wpns_[(int)eWeaponSlot.SUB].wpnUID_;
        chardata.EquipWeapon2SkinTID = (int)_pktInfo.wpns_[(int)eWeaponSlot.SUB].wpnSkin_;

        chardata.SetPktSkinColorData( _pktInfo.skinColor_ );

        RecvProtocolData( _pktInfo );
        return true;
    }

    bool RecvAckLvUpSkill( HostID remote, RmiContext rmiContext, PktInfoSkillLvUp _pktInfo ) {
        var chardata = GetCharData((long)_pktInfo.cuid_);
        if( chardata == null )
            return false;

        _userdata.Goods[(int)eGOODSTYPE.GOLD] = (long)_pktInfo.userGold_;
        chardata.PassviePoint = (int)_pktInfo.skillPT_;

        var skilldata = chardata.PassvieList.Find(x => x.SkillID == (int)_pktInfo.skillTID_);
        if( skilldata == null ) {
            chardata.PassvieList.Add( new PassiveData( (int)_pktInfo.skillTID_, (int)_pktInfo.resultLv_ ) );
        }
        else {
            skilldata.SkillLevel = (int)_pktInfo.resultLv_;
        }

        ApplyPktInfoMaterItem( _pktInfo.maters_ );


        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckLvUpUserSkill( HostID remote, RmiContext rmiContext, PktInfoUserSklLvUpAck _pktInfo ) {
        _userdata.SetPktData( _pktInfo.goods_ );
        _userdata.SetPktData( _pktInfo );

        ApplyPktInfoMaterItem( _pktInfo.maters_ );

        RecvProtocolData( _pktInfo );
        return true;
    }

    bool RecvAckResetUserSkill( HostID remote, RmiContext rmiContext, PktInfoUserSklReset _pktInfo ) {
        ApplyPktInfoMaterItemGoods( _pktInfo.consume_ );
        _userdata.SetPktData( _pktInfo.tids_ );

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckGivePresentChar( HostID remote, RmiContext rmiContext, PktInfoGivePresentCharAck _pktInfo ) {
        CharData charData = GetCharData((long)_pktInfo.cuid_);
        if( charData == null ) {
            return false;
        }

        charData.FavorPreCnt = _pktInfo.preCnt_;
        charData.FavorExp = (int)_pktInfo.expLv_.exp_;
        charData.FavorLevel = _pktInfo.expLv_.lv_;

        ApplyProduct( _pktInfo.products_, false, false );
        ApplyPktInfoMaterItem( _pktInfo.maters_ );

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckResetSecretCntChar( HostID remote, RmiContext rmiContext, PktInfoCharSecretCntRst _pktInfo ) {
        ApplyPktInfoMaterItemGoods( _pktInfo.consume_ );

        foreach( ulong uid in _pktInfo.chars_.uids_ ) {
            for( int i = 0; i < _charlist.Count; i++ ) {
                if( _charlist[i].CUID == (long)uid ) {
                    _charlist[i].SecretQuestCount = 1;
                    break;
                }
            }
        }

        RecvProtocolData( _pktInfo );
        return true;
    }

    bool RecvAckApplySkillInChar( HostID remote, RmiContext rmiContext, PktInfoCharSlotSkill _pktInfo ) {
        var chardata = GetCharData((long)_pktInfo.cuid_);
        if( chardata == null )
            return false;

        for( int i = 0; i < _pktInfo.skilTIDs_.Length; i++ ) {
            chardata.EquipSkill[i] = (int)_pktInfo.skilTIDs_[i];
        }

        RecvProtocolData( _pktInfo );
        return true;
    }

    bool RecvAckStageStart( HostID remote, RmiContext rmiContext, PktInfoStageGameStartAck _pktInfo ) {
        _pktstagegamestart = _pktInfo;
        StageClearID = -1;
        bStageFailure = false;
        GameResultData.Init();

        MaxNormalBoxCount = _pktInfo.nBoxMaxCnt_;

        if (0 < _pktInfo.ticketNextTime_.time_)
        {
            _userdata.BPRemainTime = _pktInfo.ticketNextTime_.GetTime();
        }

        GameTable.Stage.Param stageParam = _gametable.FindStage( x => x.ID == _pktInfo.stageTID_ );
        if( stageParam != null && stageParam.StageType == (int)eSTAGETYPE.STAGE_RAID ) {
            ApplyPktInfoMaterItemGoods( _pktInfo.consume_ );
        }

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckStageEnd( HostID remote, RmiContext rmiContext, PktInfoStageGameResultAck _pktInfo, PktInfoProductPack _pktProduct ) {
        StageClearID = (int)_pktInfo.clearStageTID_;
        bStageFailure = false;

        GameTable.Stage.Param stageParam = _gametable.FindStage( x => x.ID == StageClearID );
        bool isRaid = ( stageParam != null && stageParam.StageType == (int)eSTAGETYPE.STAGE_RAID );

        _pktstagegamestartresult = _pktInfo;
        _pktproduct = _pktProduct;

        ApplyStageGameResult( _pktstagegamestartresult, isRaid );
        ApplyProduct( _pktproduct, true, isRaid );
        ApplyPktTimeAttackClearList( _pktInfo.timeRecord_ );

        if( isRaid ) {
            ApplyPktRaidClearList( _pktInfo.raidSeasonRecord_ );

            for( int i = 0; i < RaidUserData.CharUidList.Count; i++ ) {
                CharData charData = GetCharData( RaidUserData.CharUidList[i] );
                charData.RaidHpPercentage = (float)_pktInfo.raidCharHPs_[i] / 100.0f;
            }

            IsNewRaidReocrd = ( _pktInfo.raidBestOpenLevel_ > 0 );
            if( IsNewRaidReocrd ) {
                RaidUserData.SetCurStep( _pktInfo.raidBestOpenLevel_ );
			}

            if( _pktInfo.dailyLimitPoint_ > 0 ) {
                RaidUserData.DailyRaidPoint = _pktInfo.dailyLimitPoint_;
            }
        }

        if( GameResultData.UserBeforeLevel == GameResultData.UserAfterLevel ) {
            Firebase.Analytics.FirebaseAnalytics.LogEvent( "RankUp", "Rank", GameResultData.UserAfterLevel );
        }

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckStageEndFail( HostID remote, RmiContext rmiContext, PktInfoStageGameEndFail _pkt ) {
        bStageFailure = true;
        ApplyRaidFailResult( _pkt );

		RecvProtocolData( _pkt );
        return true;
    }
    bool RecvAckStageContinue( HostID remote, RmiContext rmiContext, PktInfoConsumeItemAndGoods _pktInfo ) {
        return true;
    }
    bool RecvAckTimeAtkRankingList( HostID remote, RmiContext rmiContext, PktInfoTimeAtkRankStageList _pktInfo ) {
        ApplyPktInfoTimeAtkRankStageList( _pktInfo );

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckTimeAtkRankerDetail( HostID remote, RmiContext rmiContext, PktInfoTimeAtkRankerDetailAck _pktInfo ) {
        ApplyPktInfoTimeAtkRankerDetailAck( _pktInfo );

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckArenaSeasonPlay( HostID remote, RmiContext rmiContext, PktInfoUserArenaRec _pktInfo ) {
        _userbattledata.SetPktData( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckSetArenaTeam( HostID remote, RmiContext rmiContext, PktInfoUserArenaTeam _pktInfo ) {
        SetPktInfoUserArenaTeam( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckArenaGameStart( HostID remote, RmiContext rmiContext, PktInfoArenaGameStartAck _pktInfo ) {
        PktArenaGameStart = _pktInfo;

        _userdata.Goods[(int)eGOODSTYPE.GOLD] = (long)_pktInfo.userGold_;
        ApplyPktInfoMaterItem( _pktInfo.useItemInfos_ );

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckArenaGameEnd( HostID remote, RmiContext rmiContext, PktInfoArenaGameEndAck _pktInfo ) {
        _userdata.SetPktData( _pktInfo.userGoods_ );

        if( 0 != _pktInfo.BPNextTime_.time_ )
            _userdata.BPRemainTime = _pktInfo.BPNextTime_.GetTime();

        _userbattledata.SetPktData( _pktInfo.record_ );
        ArenaGameEnd_Flag = true;
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckArenaEnemySearch( HostID remote, RmiContext rmiContext, PktInfoArenaSearchEnemyAck _pktInfo ) {
        _userdata.Goods[(int)eGOODSTYPE.GOLD] = (long)_pktInfo.userGold_;

        MatchTeam.SetPktData( _pktInfo.enemyInfo_ );

        GetArenaEnemyRandomTeam( _pktInfo );

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckArenaRankingList( HostID remote, RmiContext rmiContext, PktInfoArenaRankList _pktInfo ) {
        ArenaRankingList.SetPktData( _pktInfo );

        //랭킹 리스트 받아서 자기자신이 있는지 확인.
        if( null != ArenaRankingList.RankingSimpleList && ArenaRankingList.RankingSimpleList.Count > 0 ) {
            TeamData rankdata = GameInfo.Instance.ArenaRankingList.RankingSimpleList.Find(x => x.UUID == GameInfo.Instance.UserData.UUID);
            if( rankdata != null )
                _userbattledata.Now_Rank = rankdata.Rank;
        }

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckArenaRankerDetail( HostID remote, RmiContext rmiContext, PktInfoArenaDetail _pktInfo ) {
        _arenaRankerDetialData.SetPktData( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckSetArenaTowerTeam( HostID remote, RmiContext rmiContext, PktInfoUserArenaTeam _pktInfo ) {
        SetPktInfoUserArenaTowerTeam( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckArenaTowerGameStart( HostID remote, RmiContext rmiContext, PktInfoArenaTowerGameStartAck _pktInfo ) {
        ApplyPktInfoMaterItem( _pktInfo.useItem_ );
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckArenaTowerGameEnd( HostID remote, RmiContext rmiContext, PktInfoArenaTowerGameEndAck _pktInfo ) {
        RecvProtocolData( _pktInfo );
        return true;
    }

    bool RecvAckUnexpectedPackageDailyReward( HostID remote, RmiContext rmiContext, PktInfoUnexpectedPackageDailyRewardAck _pktInfo ) {
        if( AddPktUnexpectedPackagePiece( _pktInfo.piece_ ) ) {
            // Sort
            foreach( KeyValuePair<int, List<UnexpectedPackageData>> pair in _unexpectedPackageDataDict ) {
                pair.Value.Sort( ( x, y ) => x.EndTime < y.EndTime ? -1 : 1 );
            }
        }

        ApplyProduct( _pktInfo.productReward_, false );
        RecvProtocolData( _pktInfo );
        return true;
    }
    //--------------------------------------------------------------------------------------------------------------
    // Product
    //--------------------------------------------------------------------------------------------------------------
    //아이템
    bool RecvAckSellItem( HostID remote, RmiContext rmiContext, PktInfoItemSell _pktInfo ) {
        _userdata.Goods[(int)eGOODSTYPE.GOLD] = (long)_pktInfo.userGold_;

        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            var info = _pktInfo.infos_[i];
            var itemdata = GetItemData((long)info.itemUID_);
            if( itemdata != null ) {
                if( info.delFlag_ != 0 ) {
                    _itemlist.Remove( itemdata );
                }
                else {
                    itemdata.Count = (int)info.cnt_;
                }
            }

        }
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckItemExchangeCash( HostID remote, RmiContext rmiContext, PktInfoRwdItemGoods _pktInfo ) {
        _userdata.SetPktData( _pktInfo.goods_ );
        AddPktItemList( _pktInfo.items_.rets_ );
        RewardList.Clear();
        for( int i = 0; i < _pktInfo.items_.adds_.infos_.Count; i++ ) {

            RewardData reward = new RewardData((int)eREWARDTYPE.ITEM, (int)_pktInfo.items_.adds_.infos_[i].tid_, (int)_pktInfo.items_.adds_.infos_[i].addCnt_);
            RewardList.Add( reward );
        }

        RecvProtocolData( _pktInfo );

        return true;
    }
    bool RecvAckUseItemGoods( HostID remote, RmiContext rmiContext, PktInfoUseItemGoodsAck _pktInfo ) {
        ApplyPktInfoMaterItem( _pktInfo.useItem_ );
        _userdata.Goods[(int)_pktInfo.type_] = (long)_pktInfo.userVal_;

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckUseItemProduct( HostID remote, RmiContext rmiContext, PktInfoUseItemProductAck _pktInfo ) {
        // !@#~서버 아이템 사용 작업으로 클라에서 수정 및 확인할 부분~#@!
        ApplyPktInfoMaterItem( _pktInfo.useItem_ );
        ApplyProduct( _pktInfo.products_, false, false );

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckUseItemCode( HostID remote, RmiContext rmiContext, PktInfoUseItemCodeAck _pktInfo ) {
        ApplyPktInfoMaterItem( _pktInfo.useItem_ );

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckUseItemStageSpecial( HostID remote, RmiContext rmiContext, PktInfoUseItemStageSpecialAck _pktInfo ) {
        ApplyPktInfoMaterItem( _pktInfo.useItem_ );

        _userdata.LastPlaySpecialModeTime = GameSupport.GetLocalTimeByServerTime( _pktInfo.special_.nextTime_.GetTime() );
        _userdata.NextPlaySpecialModeTableID = (int)_pktInfo.special_.tableID_;

        if( _pktInfo.subItemType_ == (int)eITEMSUBTYPE.MATERIAL_SPECIAL_CHANGE ) {

        }
        else if( _pktInfo.subItemType_ == (int)eITEMSUBTYPE.MATERIAL_SPECIAL_RESET ) {

        }

        RecvProtocolData( _pktInfo );

        return true;
    }

    //무기
    bool RecvAckSellWeapon( HostID remote, RmiContext rmiContext, PktInfoWeaponSell _pktInfo ) {
        _userdata.Goods[(int)eGOODSTYPE.GOLD] = (long)_pktInfo.userGold_;
        _userdata.Goods[(int)eGOODSTYPE.SUPPORTERPOINT] = (long)_pktInfo.userSP_;

        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            var info = _pktInfo.infos_[i];
            var weapondata = GetWeaponData((long)info.weaponUID_);
            if( weapondata != null ) {
                _weaponlist.Remove( weapondata );
            }
        }

        CheckAllRedDotWeapon();

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckSetLockWeapon( HostID remote, RmiContext rmiContext, PktInfoWeaponLock _pktInfo ) {
        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            var info = _pktInfo.infos_[i];
            var weapondata = GetWeaponData((long)info.weaponUID_);
            if( weapondata != null ) {
                weapondata.Lock = (bool)info.lock_;
            }
        }

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckLvUpWeapon( HostID remote, RmiContext rmiContext, PktInfoWeaponGrow _pktInfo ) {
        __DoApplyCommonWeaponGrow( _pktInfo );
        RecvProtocolData( _pktInfo );

        return true;
    }
    bool RecvAckWakeWeapon( HostID remote, RmiContext rmiContext, PktInfoWeaponGrow _pktInfo ) {
        __DoApplyCommonWeaponGrow( _pktInfo );
        RecvProtocolData( _pktInfo );

        return true;
    }
    bool RecvAckEnchantWeapon( HostID remote, RmiContext rmiContext, PktInfoWeaponGrow _pktInfo ) {
        __DoApplyCommonWeaponGrow( _pktInfo );
        RecvProtocolData( _pktInfo );

        return true;
    }
    bool RecvAckSkillLvUpWeapon( HostID remote, RmiContext rmiContext, PktInfoWeaponGrow _pktInfo ) {
        __DoApplyCommonWeaponGrow( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool __DoApplyCommonWeaponGrow( PktInfoWeaponGrow _pktInfo ) {
        PktInfoProductComGrowAck comGrow = _pktInfo.comGrow_;
        var data = GetWeaponData((long)comGrow.targetUID_);
        if( data == null )
            return false;

        data.Exp = (int)comGrow.expLv_.exp_;
        data.Level = comGrow.expLv_.lv_;
        data.Wake = _pktInfo.retWake_;
        data.SkillLv = _pktInfo.retSkillLv_;
        data.EnchantLv = _pktInfo.retEnc_;

        for( int i = 0; i < _pktInfo.bookStates_.infos_.Count; i++ ) {
            WeaponBookData _bookdata = GetWeaponBookData((int)_pktInfo.bookStates_.infos_[i].tableID_);
            _bookdata.StateFlag = (int)_pktInfo.bookStates_.infos_[i].stateFlag_;
        }

        this.ApplyPktInfoMaterWeapon( comGrow.maters_ );
        // 재료 아이템 소모 처리
        this.ApplyPktInfoMaterItem( comGrow.materItems_ );

        _userdata.Goods[(int)eGOODSTYPE.GOLD] = (long)comGrow.userGold_;

        CheckAllRedDotWeapon();
        return true;
    }
    bool RecvAckApplyGemInWeapon( HostID remote, RmiContext rmiContext, PktInfoWeaponSlotGem _pktInfo ) {
        var weapondata = GetWeaponData((long)_pktInfo.weaponUID_);
        if( weapondata == null )
            return false;

        for( int i = 0; i < _pktInfo.gemUIDs_.Length; i++ )
            weapondata.SlotGemUID[i] = (long)_pktInfo.gemUIDs_[i];

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckAddSlotInWpnDepot( HostID remote, RmiContext rmiContext, PktInfoWpnDepotSlotAdd _pktInfo ) {
        this.ApplyPktInfoMaterItemGoods( _pktInfo.comsume_ );
        WeaponArmoryData.ArmorySlotCnt = (int)_pktInfo.value_.maxCnt_;
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckApplySlotInWpnDepot( HostID remote, RmiContext rmiContext, PktInfoWpnDepotApply _pktInfo ) {
        WeaponArmoryData.SetWeaponArmorySlot( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }



    //곡옥
    bool RecvAckSellGem( HostID remote, RmiContext rmiContext, PktInfoGemSell _pktInfo ) {
        _userdata.Goods[(int)eGOODSTYPE.GOLD] = (long)_pktInfo.userGold_;

        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            var info = _pktInfo.infos_[i];
            var gemdata = GetGemData((long)info.gemUID_);
            if( gemdata != null ) {
                _gemlist.Remove( gemdata );
            }

        }

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckSetLockGem( HostID remote, RmiContext rmiContext, PktInfoGemLock _pktInfo ) {
        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            var info = _pktInfo.infos_[i];
            var gemdata = GetGemData((long)info.gemUID_);
            if( gemdata != null ) {
                gemdata.Lock = (bool)info.lock_;
            }
        }

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckLvUpGem( HostID remote, RmiContext rmiContext, PktInfoGemGrow _pktInfo ) {
        __DoApplyCommonGemGrow( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;

    }
    bool RecvAckWakeGem( HostID remote, RmiContext rmiContext, PktInfoGemGrow _pktInfo ) {
        __DoApplyCommonGemGrow( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckEvolutionGem( HostID remote, RmiContext rmiContext, PktInfoGemGrow _pktInfo ) {
        __DoApplyCommonGemGrow( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckAnalyzeGem( HostID remote, RmiContext rmiContext, PktInfoGemGrow _pktInfo ) {
        __DoApplyCommonGemGrow( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool __DoApplyCommonGemGrow( PktInfoGemGrow _pktInfo ) {
        PktInfoProductComGrowAck comGrow = _pktInfo.comGrow_;
        //Send_GemLevelUp <=> ProductC2S.Proxy.ReqLvUpGem -> ProductS2C.Stub.AckLvUpGem
        GemData gemdata = GetGemData((long)comGrow.targetUID_);
        if( gemdata == null )
            return false;

        gemdata.SetPktData( _pktInfo.gem_ );

        // 재료 소모 처리
        this.ApplyPktInfoMaterGem( comGrow.maters_ );
        // 재료 아이템 소모 처리
        this.ApplyPktInfoMaterItem( comGrow.materItems_ );

        _userdata.Goods[(int)eGOODSTYPE.GOLD] = (long)comGrow.userGold_;
        return true;
    }
    bool RecvAckResetOptGem( HostID remote, RmiContext rmiContext, PktInfoGemResetOptAck _pktInfo ) {
        GemData gemdata = GetGemData((long)_pktInfo.req_.gemUID_);
        if( gemdata == null )
            return false;

        gemdata.TempOptIndex = (int)_pktInfo.req_.slotIdx_;
        gemdata.TempOptID = (int)_pktInfo.opt_.optID_;
        gemdata.TempOptValue = (int)_pktInfo.opt_.value_;

        // 재료 아이템 소모 처리
        this.ApplyPktInfoMaterItemGoods( _pktInfo.consume_ );

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckResetOptSelectGem( HostID remote, RmiContext rmiContext, PktInfoGemResetOptSelect _pktInfo ) {
        GemData gemdata = GetGemData((long)_pktInfo.gemUID_);
        if( gemdata == null )
            return false;

        if( _pktInfo.newFlag_ ) {
            gemdata.RandOptID[gemdata.TempOptIndex] = gemdata.TempOptID;
            gemdata.RandOptValue[gemdata.TempOptIndex] = gemdata.TempOptValue;
        }

        gemdata.TempOptIndex = -1;
        gemdata.TempOptID = -1;
        gemdata.TempOptValue = 0;

        RecvProtocolData( _pktInfo );
        return true;
    }

    // 인벤토리 확장
    bool RecvAckAddItemSlot( HostID remote, RmiContext rmiContext, PktInfoAddSlot _pktInfo ) {
        _userdata.ItemSlotCnt = (int)_pktInfo.nowSlotCnt_;
        // TODO 황소현 인벤토리확장 임시로 수정
        this.ApplyPktInfoMaterItemGoods( _pktInfo.comsume_ );

        RecvProtocolData( _pktInfo );

        return true;
    }

    //카드
    bool RecvAckApplyPosCard( HostID remote, RmiContext rmiContext, PktInfoCardApplyPos _pktInfo ) {
        CardData carddata = GetCardData((long)_pktInfo.cardUID_);
        if( carddata != null ) {
            CardData oldcarddata = GetCardData((long)_pktInfo.oldCardUID_);
            if( oldcarddata != null ) {
                if( _pktInfo.oldCardChangeSlotNum_ == (int)eCardSlotPosMax._SLOT_MAX_ ) {
                    oldcarddata.PosKind = (int)eContentsPosKind._NONE_;
                    oldcarddata.PosValue = 0;
                    oldcarddata.PosSlot = 0;
                }
                else {
                    oldcarddata.PosKind = (int)_pktInfo.posKind_;
                    oldcarddata.PosValue = (long)_pktInfo.posValue_;
                    oldcarddata.PosSlot = (int)_pktInfo.oldCardChangeSlotNum_;
                }
            }

            carddata.PosKind = (int)_pktInfo.posKind_;
            carddata.PosValue = (long)_pktInfo.posValue_;
            carddata.PosSlot = (int)_pktInfo.slotNum_;

            ApplySetCardPos();
        }

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckApplyOutPosCard( HostID remote, RmiContext rmiContext, PktInfoCardApplyOutPos _pktInfo ) {
        CardData carddata = GetCardData((long)_pktInfo.cardUID_);
        if( carddata != null ) {
            carddata.InitPos();
            ApplySetCardPos();
        }

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckSellCard( HostID remote, RmiContext rmiContext, PktInfoCardSell _pktInfo ) {
        _userdata.Goods[(int)eGOODSTYPE.GOLD] = (long)_pktInfo.userGold_;
        _userdata.Goods[(int)eGOODSTYPE.SUPPORTERPOINT] = (long)_pktInfo.userSP_;

        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            var info = _pktInfo.infos_[i];
            var carddata = GetCardData((long)info.cardUID_);
            if( carddata != null ) {
                _cardlist.Remove( carddata );
            }
        }

        CheckAllRedDotCard();

        RecvProtocolData( _pktInfo );
        return true;
    }

    bool RecvAckSetLockCard( HostID remote, RmiContext rmiContext, PktInfoCardLock _pktInfo ) {
        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            var info = _pktInfo.infos_[i];
            var carddata = GetCardData((long)info.cardUID_);
            if( carddata != null ) {
                carddata.Lock = (bool)info.lock_;
            }
        }

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckChangeTypeCard( HostID remote, RmiContext rmiContext, PktInfoCardTypeChangeAck _pktInfo ) {
        _userdata.SetPktData( _pktInfo.goods_ );

        CardData cardData = GetCardData((long)_pktInfo.cardUID_);
        cardData.Type = _pktInfo.type_;

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckLvUpCard( HostID remote, RmiContext rmiContext, PktInfoCardGrow _pktInfo ) {
        __DoApplyCommonCardGrow( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckWakeCard( HostID remote, RmiContext rmiContext, PktInfoCardGrow _pktInfo ) {
        __DoApplyCommonCardGrow( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckEnchantCard( HostID remote, RmiContext rmiContext, PktInfoCardGrow _pktInfo ) {
        __DoApplyCommonCardGrow( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckSkillLvUpCard( HostID remote, RmiContext rmiContext, PktInfoCardGrow _pktInfo ) {
        __DoApplyCommonCardGrow( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }

    bool __DoApplyCommonCardGrow( PktInfoCardGrow _pktInfo ) {
        PktInfoProductComGrowAck comGrow = _pktInfo.comGrow_;
        CardData carddata = GetCardData((long)comGrow.targetUID_);
        if( carddata == null )
            return false;

        _userdata.Goods[(int)eGOODSTYPE.GOLD] = (long)comGrow.userGold_;
        carddata.Level = comGrow.expLv_.lv_;
        carddata.Exp = (int)comGrow.expLv_.exp_;
        carddata.Wake = _pktInfo.retWake_;
        carddata.SkillLv = _pktInfo.retSkillLv_;
        carddata.EnchantLv = _pktInfo.retEnc_;

        for( int i = 0; i < _pktInfo.bookStates_.infos_.Count; i++ ) {
            CardBookData _cardbookdata = GetCardBookData((int)_pktInfo.bookStates_.infos_[i].tableID_);
            _cardbookdata.StateFlag = (int)_pktInfo.bookStates_.infos_[i].stateFlag_;
        }

        //재료 카드 소모 처리
        this.ApplyPktInfoMaterCard( comGrow.maters_ );
        // 재료 아이템 소모 처리
        this.ApplyPktInfoMaterItem( comGrow.materItems_ );

        _userdata.Goods[(int)eGOODSTYPE.GOLD] = (long)comGrow.userGold_;

        CheckAllRedDotCard();

        return true;
    }
    bool RecvAckFavorLvRewardCard( HostID remote, RmiContext rmiContext, PktInfoBookStateReward _pktInfo ) {
        ApplyProduct( _pktInfo.products_, false, false );

        CardBookData _cardbookdata = GetCardBookData((int)_pktInfo.bookState_.tid_);
        if( _cardbookdata != null )
            _cardbookdata.StateFlag = (int)_pktInfo.bookState_.stateFlag_;

        RecvProtocolData( _pktInfo );

        return true;
    }
    bool RecvAckDecomposition( HostID remote, RmiContext rmiContext, PktInfoDecompositionAck _pktInfo ) {
        if( _pktInfo.kind_ == eContentsPosKind.CARD ) {
            ApplyPktInfoMaterCard( _pktInfo.decompositionList_ );
        }
        else if( _pktInfo.kind_ == eContentsPosKind.WEAPON ) {
            ApplyPktInfoMaterWeapon( _pktInfo.decompositionList_ );
        }

        ApplyProduct( _pktInfo.takeProduct_, false );

        RecvProtocolData( _pktInfo );

        return true;
    }
    //문양
    bool RecvAckApplyPosBadge( HostID remote, RmiContext rmiContext, PktInfoBadgeApplyPos _pktInfo ) {
        BadgeData badgeData = GetBadgeData((long)_pktInfo.badgeUID_);
        if( badgeData != null ) {
            BadgeData oldBadgeData = GetBadgeData((long)_pktInfo.oldBadgeUID_);
            if( oldBadgeData != null ) {
                if( _pktInfo.oldBadgeChangeSlotNum_ == (int)eBadgeSlotPosMax._SLOT_MAX_ ) {
                    oldBadgeData.PosKind = (int)eContentsPosKind._NONE_;
                    oldBadgeData.PosValue = 0;
                    oldBadgeData.PosSlotNum = (int)eBadgeSlot.NONE;
                }
                else {
                    oldBadgeData.PosKind = (int)_pktInfo.posKind_;
                    oldBadgeData.PosValue = (long)_pktInfo.posValue_;
                    oldBadgeData.PosSlotNum = (int)_pktInfo.oldBadgeChangeSlotNum_;
                }
            }
            badgeData.PosKind = (int)_pktInfo.posKind_;
            badgeData.PosValue = (long)_pktInfo.posValue_;
            badgeData.PosSlotNum = (int)_pktInfo.slotNum_;
        }
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckApplyOutPosBadge( HostID remote, RmiContext rmiContext, PktInfoBadgeApplyOutPos _pktInfo ) {
        BadgeData badgeData = GetBadgeData((long)_pktInfo.badgeUID_);
        if( badgeData != null ) {
            badgeData.PosValue = 0;
            badgeData.PosKind = (int)eContentsPosKind._NONE_;
            badgeData.PosSlotNum = (int)eBadgeSlot.NONE;
        }
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckSellBadge( HostID remote, RmiContext rmiContext, PktInfoBadgeSell _pktInfo ) {
        _userdata.Goods[(int)eGOODSTYPE.BATTLECOIN] = (long)_pktInfo.userBTCoin_;

        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            var info = _pktInfo.infos_[i];
            var badgedata = GetBadgeData((long)info.badgeUID_);
            if( badgedata != null ) {
                _badgelist.Remove( badgedata );
            }
        }

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckSetLockBadge( HostID remote, RmiContext rmiContext, PktInfoBadgeLock _pktInfo ) {
        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            var info = _pktInfo.infos_[i];
            var badgedata = GetBadgeData((long)info.badgeUID_);
            if( badgedata != null ) {
                badgedata.Lock = (bool)info.lock_;
            }
        }

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckUpgradeBadge( HostID remote, RmiContext rmiContext, PktInfoBadgeUpgrade _pktInfo ) {
        BadgeData badgedata = GetBadgeData((long)_pktInfo.badgeUID_);
        badgedata.Level = (int)_pktInfo.retLv_;
        badgedata.RemainLvCnt = (int)_pktInfo.retRemainLvUpCnt_;

        ApplyPktInfoMaterItem( _pktInfo.maters_ );
        _userdata.Goods[(int)eGOODSTYPE.GOLD] = (long)_pktInfo.userGold_;

        RecvProtocolData( _pktInfo );

        return true;
    }
    bool RecvAckResetUpgradeBadge( HostID remote, RmiContext rmiContext, PktInfoBadgeUpgrade _pktInfo ) {
        BadgeData badgedata = GetBadgeData((long)_pktInfo.badgeUID_);
        badgedata.Level = (int)_pktInfo.retLv_;
        badgedata.RemainLvCnt = (int)_pktInfo.retRemainLvUpCnt_;

        ApplyPktInfoMaterItem( _pktInfo.maters_ );
        _userdata.Goods[(int)eGOODSTYPE.GOLD] = (long)_pktInfo.userGold_;

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckStorePurchase( HostID remote, RmiContext rmicontext, PktInfoStorePurchase _pktInfo ) {
        ApplyPktInfoMaterItem( _pktInfo.consumeItem_ );
        ApplyProduct( _pktInfo.products_, false, false );
        ApplyPktSaleStore( _pktInfo.addSaleInfo_ );
        ApplyPktSaleStore( _pktInfo.updateSaleInfo_ );

        ApplyItemWithStoreID( (int)_pktInfo.storeID_ );

        RecvProtocolData( _pktInfo );

        return true;
    }
    bool RecvAckStorePurchaseInApp( HostID remote, RmiContext rmicontext, PktInfoStorePurchase _pktInfo ) {
        ApplyProduct( _pktInfo.products_, false, false );
        ApplyPktSaleStore( _pktInfo.addSaleInfo_ );
        ApplyPktSaleStore( _pktInfo.updateSaleInfo_ );

        ApplyItemWithStoreID( (int)_pktInfo.storeID_ );

        RecvProtocolData( _pktInfo );

        return true;
    }
    bool RecvAckUserRotationGachaOpen( HostID remote, RmiContext rmicontext, PktInfoUserRGachaOpenAck _pktInfo ) {
        Log.Show( "############# AckUserRotationGachaOpen", Log.ColorType.Red );
        _userdata.SetPktData( _pktInfo.goods_ );
        UserRotationGachaData.UpdatePktData( _pktInfo.infos_ );

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckRaidStoreList( HostID remote, RmiContext rmicontext, PktInfoRaidStoreListAck _pktInfo ) {
        ApplyPktInfoMaterItemGoods( _pktInfo.consume_ );
        RaidSecretStoreChangeRemainTime = GameSupport.GetLocalTimeByServerTime( _pktInfo.info_.resetTM_.GetTime() );

        RecvProtocolData( _pktInfo );
        return true;
    }

    bool RecvAckSetMainRoomTheme( HostID remote, RmiContext rmiContext, System.Byte _roomThemeSlotNum ) {
        _userdata.RoomThemeSlot = _roomThemeSlotNum;

        RecvProtocolData( (long)_roomThemeSlotNum );
        return true;
    }

    bool RecvAckRoomPurchase( HostID remote, RmiContext rmiContext, PktInfoRoomStorePurchase _pktInfo ) {
        for( int loop = 0; loop < _pktInfo.infos_.Count; loop++ ) {
            SetPktInfoRoomPurchaseOne( _pktInfo.infos_[loop].purchase_ );
        }

        this.ApplyPktInfoMaterItemGoods( _pktInfo.retConsume_ );

        //_userdata.Goods[(int)_pktInfo.goodsTp_] = (long)_pktInfo.userGoodsVal_;
        //_userdata.Goods[(int)eGOODSTYPE.ROOMPOINT] = (long)_pktInfo.userRoomPT_;
        //_userdata.HardCash = (long)_pktInfo.nowHardCash_;

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckRoomThemeSlotDetailInfo( HostID remote, RmiContext rmiContext, PktInfoRoomThemeSlotDetail _pktInfo ) {
        SetPktInfoRoomThemeSlotDetail( _pktInfo );

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckRoomThemeSlotSave( HostID remote, RmiContext rmiContext ) {
        SetPktInfoRoomThemeSlotDetail( RoomThemeSlotDetail );

        RecvProtocolData( null );
        return true;
    }

    bool RecvAckBookNewConfirm( HostID remote, RmiContext rmicontext, PktInfoBookNewConfirm _pktInfo ) {
        switch( _pktInfo.bookGroup_ ) {
            case eBookGroup.Weapon:
                WeaponBookData bookData = _weaponbooklist.Find(x => x.TableID == _pktInfo.bookTID_);
                bookData.StateFlag = (int)_pktInfo.retStateFlag_;
                break;

            case eBookGroup.Supporter:
                CardBookData cardData = _cardbooklist.Find(x => x.TableID == _pktInfo.bookTID_);
                cardData.StateFlag = (int)_pktInfo.retStateFlag_;
                break;

            case eBookGroup.Monster:
                MonsterBookData monsterData = _monsterbooklist.Find(x => x.TableID == _pktInfo.bookTID_);
                monsterData.StateFlag = (int)_pktInfo.retStateFlag_;
                break;
        }

        RecvProtocolData( _pktInfo );

        return true;
    }

    bool RecvNotiSvrReloadTableInfo( HostID remote, RmiContext rmiContext, PktInfoUserReflash _pktInfo ) {
        SetPktInfoUserReflashData( _pktInfo );
        //GameInfo.Instance.Send_ReqUpdateGllaMission();
        return true;
    }
    bool RecvNotiCloseServTime( HostID remote, RmiContext rmiContext, System.UInt32 _leftTime_Sec ) {
        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Title )
            return true;

        int timesec = (int)_leftTime_Sec;
        int hor = timesec / 3600;
        timesec = timesec % 3600;
        int min = timesec / 60;
        int sec = timesec % 60;
        string text = string.Empty;
        if( hor != 0 )
            text = string.Format( FLocalizeString.Instance.GetText( 3110 ), hor );
        else if( min != 0 )
            text = string.Format( FLocalizeString.Instance.GetText( 3111 ), min );
        else
            text = string.Format( FLocalizeString.Instance.GetText( 3112 ), sec );

        if( timesec > 0 ) {
            MessageNotifyPopup.Show( text, GameInfo.Instance.GameConfig.MessageNotifyDuration );
        }
        else {
            MessagePopup.OK( eTEXTID.TITLE_NOTICE, 3151, () => Application.Quit() );
        }

        return true;
    }
    bool RecvNotiCommonErr( HostID remote, RmiContext rmiContext, System.UInt64 _errNum ) {
        NotiCommonErr( remote, rmiContext, _errNum );
        return true;
    }
    bool RecvNotiEmbargoWordErr( HostID remote, RmiContext rmiContext, PktInfoStr _pktInfo ) {
        WaitPopup.Hide();
        ClearProtocolData();
        StopTimeOut();

        string str = _pktInfo.str_;

        MessagePopup.OK( eTEXTID.SERVERERROR, string.Format( FLocalizeString.Instance.GetText( 3147 ), str ), null );
        return true;
    }
    bool RecvNotiCheckVersionErr( HostID remote, RmiContext rmiContext, PktInfoVersion _pktInfo ) {
        NotiCheckVersionErr( remote, rmiContext, _pktInfo );
        return true;
    }

    bool RecvNotiUpdateTicket( HostID remote, RmiContext rmiContext, PktInfoUpdateTicketUserNoti _pktInfo ) {
        //Platforms.IBase.Inst.DoTestNotiUpdateTicket(_pktInfo);
        if( _pktInfo.ticketType_ == eGOODSTYPE.AP ) {
            _userdata.UpdateAP( (long)_pktInfo.resultTicket_, _pktInfo.nextTime_.GetTime() );
            Log.Show( "RecvNotiUpdateTicket AP", Log.ColorType.Red );

        }

        if( _pktInfo.ticketType_ == eGOODSTYPE.BP ) {
            _userdata.UpdateBP( (long)_pktInfo.resultTicket_, _pktInfo.nextTime_.GetTime() );
            Log.Show( "RecvNotiUpdateTicket BP", Log.ColorType.Red );
        }

        return true;
    }

    // 유저 마크 획득 알림
    bool RecvNotiUserMarkTake( HostID remote, RmiContext rmiContext, PktInfoUserMarkTake _pktInfo ) {
        AddPktUserMarkList( _pktInfo.tids_ );
        return true;
    }
    bool RecvNotiAddMail( HostID remote, RmiContext rmiContext, PktInfoMail _pktInfo ) {
        //Platforms.IBase.Inst.DoTestNotiAddMail(_pktInfo);
        SetPktMailCount( _pktInfo.maxCnt_ );

        return true;
    }
    bool RecvNotiUpdateAchieve( HostID remote, RmiContext rmiContext, PktInfoAchieve _pktInfo ) {
        ApplyPktInfoAchieve( _pktInfo );
        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            NotificationManager.Instance.CheckAchievement();
            NotificationManager.Instance.CheckNotification( NotificationManager.eTYPE.MENU );
            NotificationManager.Instance.CheckNotification( NotificationManager.eTYPE.USERMARK );
        }

        return true;
    }
    bool RecvNotiUpdateAchieveEvent( HostID remote, RmiContext rmiContext, PktInfoAchieveEvent _pktInfo ) {
        ApplyPktAchieveEvents( _pktInfo );

        return true;
    }

    bool RecvNotiSetSvrRotationGacha( HostID remote, RmiContext rmiContext, PktInfoComTimeAndTID _pktInfo ) {
        //시간 지났는지 확인후 제거
        ServerRotationGachaData.ResetPktData();
        UserRotationGachaData.ResetPktData();

        SetPktServerRotaionGacha( _pktInfo );
        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby )
            LobbyUIManager.Instance.Renewal( "GachaPanel" );
        return true;
    }
    bool RecvNotiSetSvrSecretQuestOpt( HostID remote, RmiContext rmiContext, PktInfoSecretQuestOpt _pktInfo ) {
        _serverdata.SetPktSecretQuestData( _pktInfo );
        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            LobbyUIManager.Instance.Renewal( "StageDetailPopup" );
        }
        return true;
    }
    bool RecvNotiUpdateDailyMission( HostID remote, RmiContext rmiContext, PktInfoMission.Daily _pktInfo ) {
        ApplyPktMissionDailyData( _pktInfo );
        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            if( LobbyUIManager.Instance.IsActiveUI( "UserWelcomeEventPopup" ) )
                LobbyUIManager.Instance.Renewal( "UserWelcomeEventPopup" );
        }

        return true;
    }
    bool RecvNotiResetWeekMission( HostID remote, RmiContext rmiContext, PktInfoMission.Weekly _pktInfo ) {
        Platforms.IBase.Inst.DoTestNotiResetWeekMission( _pktInfo );

        WeekMissionData.SetPktData( _pktInfo );

        return true;
    }
    bool RecvNotiUpdateWeekMission( HostID remote, RmiContext rmiContext, PktInfoMission.Weekly _pktInfo ) {
        Platforms.IBase.Inst.DoTestNotiUpdateWeekMission( _pktInfo );

        WeekMissionData.SetPktData( _pktInfo );

        return true;
    }
    bool RecvNotiUpdateInfluMission( HostID remote, RmiContext rmicontext, PktInfoMission.Influ _pktInfo ) {
        // 유저 서버 달성(세력) 미션 갱신 알림
        InfluenceMissionData.SetPktData( _pktInfo );
        return true;
    }
    bool RecvNotiUserInfluMissionChange( HostID remote, RmiContext rmicontext, PktInfoInfluChangeList.Piece _pktInfo ) {
        // 유저 서버 달성(세력) 이벤트 변경 알림
        if( UserData.UUID == (long)_pktInfo.uuid_ ) {
            InfluenceMissionData.SetPktData( _pktInfo.addInfo_ );
        }
        return true;
    }

    bool RecvNotiUpdateGllaMission( HostID remote, RmiContext rmiContext, PktInfoMission.Guerrilla _pktInfo ) {
        AddPktMissionGuerrilla( _pktInfo );

        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            if( GameInfo.Instance.ServerData.GuerrillaMissionList.Find( x => x.Type == "GM_LoginBonus" && x.GroupID == _pktInfo.infos_[i].groupID_ ) != null ) {
                GllaMissionData userMissionData = GllaMissionList.Find(x => x.GroupID == _pktInfo.infos_[i].groupID_);
                if( userMissionData != null ) {
                    userMissionData.LoginBonusDisplayFlag = true;
                    break;
                }
            }
        }

        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            Send_ReqUpdateGllaMission( null );
            NotificationManager.Instance.CheckNotification( NotificationManager.eTYPE.CAMPAIGN );
        }

        return true;
    }
    bool RecvNotiUpdatePassMission( HostID remote, RmiContext rmiContext, PktInfoMission.Pass _pktInfo ) {
        UpdatePktPassMissionData( _pktInfo );

        return true;
    }
    bool RecvNotiUserPassChange( HostID remote, RmiContext rmiContext, PktInfoPassChangeList.Piece _pktInfo ) {
        EndPktPassMissions( _pktInfo.endMsionList_ );
        SetPktPassSetData( _pktInfo.pass_ );
        SetPktPassMissionData( _pktInfo.addMsion_ );

        return true;
    }
    bool RecvNotiUserEventChange( HostID remote, RmiContext rmiContext, PktInfoEventChangeList.Piece _pktInfo ) {
        Platforms.IBase.Inst.DoTestNotiUserEventChange( _pktInfo );

        // (클라이언트)
        if( _pktInfo.addEvtRwds_ != null ) {
            UpdatePktEventSetDataList( _pktInfo.addEvtRwds_ );
        }

        return true;
    }
    //9998
    bool RecvNotiUpdateArenaTime( HostID remote, RmiContext rmiContext, PktInfoArenaSeasonTime _pktInfo ) {
        if( _pktInfo != null ) {
            _serverdata.ArenaSeasonEndTime = GameSupport.GetLocalTimeByServerTime( _pktInfo.endTime_.GetTime() );
            _serverdata.ArenaNextSeasonStartTime = GameSupport.GetLocalTimeByServerTime( _pktInfo.nextStartTime_.GetTime() );

            _serverdata.ArenaState = (int)_pktInfo.seasonState_;
        }

        return true;
    }
    // 커뮤니티 유저 아레나 정보 활성화 알림
    bool RecvNotiCommunityUserArenaOn( HostID remote, RmiContext rmiContext, System.UInt64 _onArenaUuid ) {
        FriendUserData find = CommunityData.FriendList.Find(x => x.UUID == (long)_onArenaUuid);
        if( find == null ) {
            return false;
        }

        find.HasArenaInfo = true;
        return true;
    }
    // 커뮤니티 유저 호출 횟수 변경 알림
    bool RecvNotiCommunityUserCallCnt( HostID remote, RmiContext rmiContext, PktInfoCommuCallCntNoti _pktInfo ) {
        return true;
    }
    bool RecvNotiCommunitySetArenaTowerID( HostID remote, RmiContext rmiContext, PktInfoCommuArenaTowerIDNoti _pktInfo ) {
        return true;
    }

    // 친구 승인 목록 추가 알림
    bool RecvNotiFriendFromAdd( HostID remote, RmiContext rmiContext, PktInfoFriend _pktInfo ) {
        CommunityData.UpdateFriendAskFromUserList( _pktInfo );

        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            NotificationManager.Instance.CheckALLFriend();
        }

        return true;
    }
    // 친구 승인 목록 제거 알림
    bool RecvNotiFriendFromDel( HostID remote, RmiContext rmiContext, PktInfoUIDList _pktInfo ) {
        CommunityData.RemoveFriendToAskList( _pktInfo );
        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            NotificationManager.Instance.CheckALLFriend();
        }
        return true;
    }
    // 친구 신청 답변 알림
    bool RecvNotiFriendAnswer( HostID remote, RmiContext rmiContext, PktInfoCommuAnswerNoti _pktInfo ) {
        FriendUserData friendUserData = CommunityData.FriendToAskList.Find(x => x.UUID == (long)_pktInfo.uuidInAsk_);

        if( friendUserData != null ) {
            if( _pktInfo.accept_ == true ) {
                CommunityData.FriendList.Add( friendUserData );
            }

            CommunityData.FriendToAskList.Remove( friendUserData );
        }

        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            NotificationManager.Instance.CheckALLFriend();
        }

        return true;
    }
    // 친구 제거 알림
    bool RecvNotiFriendKick( HostID remote, RmiContext rmiContext, PktInfoCommuKickNoti _pktInfo ) {
        FriendUserData friendUserData = CommunityData.FriendList.Find(x => x.UUID == (long)_pktInfo.reqUuid_);
        if( friendUserData != null )
            CommunityData.FriendList.Remove( friendUserData );

        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            NotificationManager.Instance.CheckALLFriend();
        }

        return true;
    }
    // 친구 기능 플래그 갱신 알림
    bool RecvNotiFriendFlagUpdate( HostID remote, RmiContext rmiContext, PktInfoFriendFlagUpdateNoti _pktInfo ) {

        FriendUserData friendUserData = CommunityData.FriendList.Find(x => x.UUID == (long)_pktInfo.reqUuid_);
        if( friendUserData != null ) {
            GameSupport._DoOnOffBitIdx( ref friendUserData.FriendTotalFlag, (int)_pktInfo.flag_.flagIdx_, _pktInfo.flag_.onFlag_ );
            friendUserData.UpdateBitFlag();
        }

        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            NotificationManager.Instance.CheckALLFriend();
        }

        return true;
    }

    //시설
    // 시설 레벨업(활성화) 응답
    bool RecvAckFacilityUpgrade( HostID remote, RmiContext rmicontext, PktInfoFacilityUpgrade _pktInfo ) {
        _userdata.Goods[(int)eGOODSTYPE.GOLD] = (long)_pktInfo.userGold_;
        ApplyPktInfoMaterItem( _pktInfo.maters_ );
        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            FacilityData fd = GetFacilityData((int)_pktInfo.infos_[i].facilityTID_);
            if( fd != null )
                fd.Level = _pktInfo.infos_[i].resultLv_;
        }

        RecvProtocolData( _pktInfo );


        return true;
    }

    // 시설 작동 응답
    bool RecvAckFacilityOperation( HostID remote, RmiContext rmicontext, PktInfoFacilityOperationAck _pktInfo ) {
        FacilityData facilitydata = GetFacilityData((int)_pktInfo.facilityTID_);
        if( facilitydata != null ) {
            //_userdata.Goods[(int)eGOODSTYPE.GOLD] = (long)_pktInfo.userGold_; <-- 제거됨
            ApplyPktInfoMaterItem( _pktInfo.itemMaters_ );

            ApplyPktInfoFacilityOperationAckMater( _pktInfo.maters_ );
            ApplyProduct( _pktInfo.products_, false );

            if( _pktInfo.operationRoomChar_ != null ) {
                foreach( ulong uid in _pktInfo.operationRoomChar_.uids_ ) {
                    CharData charData = _charlist.Find(x => x.CUID == (long) uid);
                    if( charData != null ) {
                        charData.OperationRoomTID = facilitydata.TableID;
                    }
                }
            }

            Log.Show( _userdata.Goods[(int)eGOODSTYPE.GOLD] );

            if( facilitydata.TableData.EffectType.Equals( "FAC_CHAR_EXP" ) ) {
                facilitydata.Selete = (long)_pktInfo.operationValue_;
            }
            else if( facilitydata.TableData.EffectType.Equals( "FAC_CHAR_SP" ) ) {
                facilitydata.Selete = (long)_pktInfo.operationValue_;
            }
            else if( facilitydata.TableData.EffectType.Equals( "FAC_ITEM_COMBINE" ) ) {
                facilitydata.Selete = (long)_pktInfo.operationValue_;
                facilitydata.OperationCnt = (int)_pktInfo.operationCnt_;
            }
            else if( facilitydata.TableData.EffectType.Equals( "FAC_WEAPON_EXP" ) ) {
                facilitydata.Selete = (long)_pktInfo.operationValue_;
            }
            else if( facilitydata.TableData.EffectType.Equals( "FAC_OPERATION_ROOM" ) ) {
                facilitydata.Selete = (long)_pktInfo.operationValue_;
                facilitydata.OperationCnt = (int)_pktInfo.operationCnt_;
            }

            facilitydata.Stats = (int)eFACILITYSTATS.USE;
            facilitydata.RemainTime = GameSupport.GetLocalTimeByServerTime( _pktInfo.endTime_.GetTime() );
        }


        RecvProtocolData( _pktInfo );

        return true;
    }
    // 시설 작동 완료 응답
    bool RecvAckFacilityOperationConfirm( HostID remote, RmiContext rmicontext, PktInfoFacilityOperConfirmAck _pktInfo ) {
        Debug.Log( "RecvAckFacilityOperationConfirm" );

        FacilityData facilitydata = GetFacilityData((int)_pktInfo.facilityTID_);
        if( facilitydata != null ) {
            // 즉시 완료 아이템 및 재화 적용
            ApplyPktInfoMaterItemGoods( _pktInfo.consumeAndGoods_ );
            ApplyProduct( _pktInfo.products_, false );
            AddPktItemList( _pktInfo.items_ );
            AddPktUnexpectedPackage( _pktInfo.unexpectedPackage_ );

            if( _pktInfo.operationRoomChar_ != null ) {
                foreach( ulong uid in _pktInfo.operationRoomChar_.uids_ ) {
                    CharData charData = _charlist.Find(x => x.CUID == (long) uid);
                    if( charData != null ) {
                        charData.OperationRoomTID = 0;
                    }
                }
            }

            if( !_pktInfo.operEndFlag_ )            //완료시간 이전에 취소했을 경우
            {
                facilitydata.Stats = (int)eFACILITYSTATS.WAIT;
                facilitydata.Selete = 0;

                RecvProtocolData( _pktInfo );

                return true;

            }
            else {
                //시설 완료
                FacilityResultData.Init();
                FacilityResultData.FacilityID = (int)_pktInfo.facilityTID_;
                FacilityResultData.UserBeforeLevel = GameInfo.Instance.UserData.Level;
                FacilityResultData.UserBeforeExp = GameInfo.Instance.UserData.Exp;

                if( facilitydata.EquipCardUID != (int)eCOUNT.NONE ) {
                    CardData cardData = GameInfo.Instance.GetCardData(facilitydata.EquipCardUID);
                    CardBookData cardBookData = GameInfo.Instance.GetCardBookData(cardData.TableID);
                    if( cardData != null && cardBookData != null ) {
                        GameInfo.Instance.FacilityResultData.CardUID = cardData.CardUID;
                        GameInfo.Instance.FacilityResultData.CardBeforeLevel = cardBookData.FavorLevel;
                        GameInfo.Instance.FacilityResultData.CardBeforeExp = cardBookData.FavorExp;

                        UpdatePktCardBookList( _pktInfo.cardBook_ );

                        GameInfo.Instance.FacilityResultData.CardAfterLevel = cardBookData.FavorLevel;
                        GameInfo.Instance.FacilityResultData.CardAfterExp = cardBookData.FavorExp;
                    }
                }

                GameInfo.Instance.FacilityResultData.UserAfterLevel = GameInfo.Instance.UserData.Level;
                GameInfo.Instance.FacilityResultData.UserAfterExp = GameInfo.Instance.UserData.Exp;

                if( facilitydata.TableData.EffectType.Equals( "FAC_CHAR_EXP" ) ) {
                    CharData charData = GetCharData(facilitydata.Selete);
                    if( charData != null ) {
                        GameInfo.Instance.FacilityResultData.TargetUID = facilitydata.Selete;
                        GameInfo.Instance.FacilityResultData.TargetBeforeLevel = charData.Level;
                        GameInfo.Instance.FacilityResultData.TargetBeforeExp = charData.Exp;

                        charData.Level = _pktInfo.operEffExpLv_.lv_;
                        charData.Exp = (int)_pktInfo.operEffExpLv_.exp_;

                        GameInfo.Instance.FacilityResultData.TargetAfterLevel = charData.Level;
                        GameInfo.Instance.FacilityResultData.TargetAfterExp = charData.Exp;
                    }
                }
                else if( facilitydata.TableData.EffectType.Equals( "FAC_CHAR_SP" ) ) {
                    CharData charData = GetCharData(facilitydata.Selete);
                    if( charData != null ) {
                        GameInfo.Instance.FacilityResultData.TargetUID = facilitydata.Selete;
                        GameInfo.Instance.FacilityResultData.TargetBeforeLevel = charData.PassviePoint;

                        //스킬 포인트 적용 - 더해진 값이 온다.
                        charData.PassviePoint = (int)_pktInfo.retValue1_;

                        //결과창에 얻은 걸 보여주기때문에 계산해준다.
                        GameInfo.Instance.FacilityResultData.TargetAfterLevel = charData.PassviePoint - GameInfo.Instance.FacilityResultData.TargetBeforeLevel;
                    }
                }
                else if( facilitydata.TableData.EffectType.Equals( "FAC_WEAPON_EXP" ) ) {
                    WeaponData weapondata = GetWeaponData(facilitydata.Selete);
                    if( weapondata != null ) {
                        GameInfo.Instance.FacilityResultData.TargetUID = facilitydata.Selete;
                        GameInfo.Instance.FacilityResultData.TargetBeforeLevel = weapondata.Level;
                        GameInfo.Instance.FacilityResultData.TargetBeforeExp = weapondata.Exp;

                        weapondata.Level = _pktInfo.operEffExpLv_.lv_;
                        weapondata.Exp = (int)_pktInfo.operEffExpLv_.exp_;

                        UpdatePktWeaponBookList( _pktInfo.weaponBook_ );

                        GameInfo.Instance.FacilityResultData.TargetAfterLevel = weapondata.Level;
                        GameInfo.Instance.FacilityResultData.TargetAfterExp = weapondata.Exp;
                    }
                }
                else if( facilitydata.TableData.EffectType.Equals( "FAC_ITEM_COMBINE" ) ) {
                    var combinedata = GameTable.FindFacilityItemCombine((int)facilitydata.Selete);
                    if( combinedata != null ) {
                        if( _pktInfo.logAddItem_.infos_.Count == 1 )      //현재 1개만 오게 되어있음.
                        {
                            for( int i = 0; i < _pktInfo.logAddItem_.infos_.Count; i++ ) {
                                GameInfo.Instance.FacilityResultData.TargetUID = _pktInfo.logAddItem_.infos_[i].tid_;
                                GameInfo.Instance.FacilityResultData.TargetAfterLevel = (int)_pktInfo.logAddItem_.infos_[i].addCnt_;
                                GameInfo.Instance.FacilityResultData.ItemUID = _pktInfo.logAddItem_.infos_[i].tid_;
                                GameInfo.Instance.FacilityResultData.ItemCount = (int)_pktInfo.logAddItem_.infos_[i].addCnt_;
                            }
                        }
                    }
                }
                else if( facilitydata.TableData.EffectType.Equals( "FAC_OPERATION_ROOM" ) ) {
                    foreach( var piece in _pktInfo.products_.addItemInfos_.infos_ ) {
                        FacilityResultData.ItemUID = piece.tid_;
                        FacilityResultData.ItemCount = (int)piece.addCnt_;
                    }
                }

                facilitydata.Stats = (int)eFACILITYSTATS.WAIT;
                facilitydata.Selete = (int)eCOUNT.NONE;
            }
        }

        RecvProtocolData( _pktInfo );

        return true;
    }

    // 파견 슬롯 열기 응답
    bool RecvAckDispatchOpen( HostID remote, RmiContext rmicontext, PktInfoDispatchOpen _pktInfo ) {
        SetPktDispatch( _pktInfo.opens_ );
        _userdata.SetPktData( _pktInfo.goods_ );
        RecvProtocolData( _pktInfo );
        return true;
    }
    // 파견 임무 교체 응답
    bool RecvAckDispatchChange( HostID remote, RmiContext rmicontext, PktInfoDispatchChange _pktInfo ) {
        SetPktDispatch( _pktInfo.changes_ );
        _userdata.SetPktData( _pktInfo.goods_ );
        RecvProtocolData( _pktInfo );
        return true;
    }
    // 파견 임무 실행 응답
    bool RecvAckDispatchOperation( HostID remote, RmiContext rmicontext, PktInfoDispatchOperAck _pktInfo ) {
        SetPktDispatch( _pktInfo.cards_ );
        SetPktDispatch( _pktInfo.opers_ );
        RecvProtocolData( _pktInfo );
        return true;
    }
    // 파견 임무 실행 완료 응답
    bool RecvAckDispatchOperationConfirm( HostID remote, RmiContext rmicontext, PktInfoDispatchOperConfirmAck _pktInfo ) {
        SetPktDispatch( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }

    // 메일 리스트 확인
    bool RecvAckMailList( HostID remote, RmiContext rmicontext, PktInfoMail _pktInfo ) {
        MailList.Clear();
        SetPktMailCount( _pktInfo.maxCnt_ );
        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            MailData mailData = new MailData();
            mailData.SetPktData( _pktInfo.infos_[i] );

            MailList.Add( mailData );
        }

        RecvProtocolData( _pktInfo );

        return true;
    }

    // 메일 수령 확인
    bool RecvAckMailTakeProductList( HostID remote, RmiContext rmicontext, PktInfoMailProductTake _pktInfo ) {
        RewardList.Clear();
        for( int i = 0; i < _pktInfo.delList_.uids_.Count; i++ ) {
            MailData mailData = GetMailData(_pktInfo.delList_.uids_[i]);
            RewardData reward = new RewardData(mailData.ProductType,(int)mailData.ProductIndex, (int)mailData.ProductValue);
            RewardList.Add( reward );
        }

        //  메일 리스트 갱신
        MailList.Clear();
        SetPktMailCount( _pktInfo.mails_.maxCnt_ );
        for( int i = 0; i < _pktInfo.mails_.infos_.Count; i++ ) {
            MailData mailData = new MailData();
            mailData.SetPktData( _pktInfo.mails_.infos_[i] );

            MailList.Add( mailData );
        }
        //  받은 보상들 갱신
        ApplyProduct( _pktInfo.takeProduct_, false );

        RecvProtocolData( _pktInfo );

        return true;
    }
    // 커뮤니티 정보 획득 응답
    bool RecvAckCommunityInfoGet( HostID remote, RmiContext rmicontext, PktInfoCommunity _pktInfo ) {
        Log.Show( "### RecvAckCommunityInfoGet" );
        CommunityData.SetCommunityData( _pktInfo );

        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            NotificationManager.Instance.CheckALLFriend();
        }

        _circleData.SetPktInfoCircleSimple(_pktInfo.circleSimple_);

        RecvProtocolData( _pktInfo );

        return true;
    }

    // 커뮤니티 유저 아레나 정보 획득 응답
    bool RecvAckCommunityUserArenaInfoGet( HostID remote, RmiContext rmicontext, PktInfoCommuUserArenaInfoAck _pktInfo ) {
        if( CommunityUserInfoGetType == eCommunityUserInfoGetType.ARENA ) {
            MatchTeam.SetPktData( _pktInfo.infos_[0] );

            //문양 데이터 보강
            if( MatchTeam.badgelist != null ) {
                for( int i = 0; i < MatchTeam.badgelist.Count; i++ ) {
                    if( MatchTeam.badgelist[i].PosKind == (int)eContentsPosKind._NONE_ ) {
                        MatchTeam.badgelist[i].PosSlotNum = i;
                        MatchTeam.badgelist[i].PosKind = (int)eContentsPosKind.ARENA;
                    }
                }
            }

            for( int i = 0; i < _matchteam.charlist.Count; i++ ) {
                if( _matchteam.charlist[i] == null ) {
                    continue;
                }

                //패시브 스킬 캐릭터에 적용
                List<int> skillIds = GameSupport.CreateArenaOpponentCharPassiveSkillList(_matchteam.charlist[i].CharData.TableID);
                if( skillIds.Count > 0 ) {
                    for( int idx = 0; idx < skillIds.Count; idx++ ) {
                        _matchteam.charlist[i].CharData.PassvieList.Add( new PassiveData( skillIds[idx], 1 + (int)( (float)_matchteam.charlist[i].CharData.Level * GameInfo.Instance.BattleConfig.ArenaEnemyCharPassvieRate ) ) );
                    }
                }
            }
        }
        else if( CommunityUserInfoGetType == eCommunityUserInfoGetType.ARENATOWER ) {
            var iter = _pktInfo.infos_.GetEnumerator();
            while( iter.MoveNext() ) {
                TeamData t = TowerFriendTeamData.Find(x => x.UUID == (long)iter.Current.userInfo_.uuid_);
                if( t != null )
                    continue;

                TeamData d = new TeamData();
                d.SetPktData( iter.Current );
                TowerFriendTeamData.Add( d );
            }
        }

        RecvProtocolData( _pktInfo );
        return true;
    }

    // 커뮤니티 유저 호출 횟수 사용 응답
    bool RecvAckCommunityUserUseCallCnt( HostID remote, RmiContext rmicontext, PktInfoCommuUseCallCntAck _pktInfo ) {
        return true;
    }

    // 추천 친구 목록 응답
    bool RecvAckFriendSuggestList( HostID remote, RmiContext rmicontext, PktInfoCommuSuggestAck _pktInfo ) {
        GameInfo.Instance.CommunityData.FriendSerachTime = GameSupport.GetCurrentServerTime();
        CommunityData.SetFriendSuggestList( _pktInfo );

        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            NotificationManager.Instance.CheckALLFriend();
        }

        RecvProtocolData( _pktInfo );

        return true;
    }
    // 친구 신청 응답
    bool RecvAckFriendAsk( HostID remote, RmiContext rmicontext, PktInfoFriendAsk _pktInfo ) {
        CommunityData.UpdateFriendToAskList( _pktInfo.addAsk_ );

        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            NotificationManager.Instance.CheckALLFriend();
        }

        RecvProtocolData( _pktInfo );

        return true;
    }
    // 친구 신청 취소 응답
    bool RecvAckFriendAskDel( HostID remote, RmiContext rmicontext ) {

        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            NotificationManager.Instance.CheckALLFriend();
        }

        RecvProtocolData( null );

        return true;
    }
    // 친구 신청에 대한 답변 응답
    bool RecvAckFriendAnswer( HostID remote, RmiContext rmicontext, PktInfoCommuAnswer _pktInfo ) {
        if( _pktInfo.accept_ ) {
            //요청 승낙 - 친구리스트에 넣어준다.
            for( int i = 0; i < _pktInfo.tgtUids_.uids_.Count; i++ ) {
                FriendUserData friendUserData = CommunityData.FriendAskFromUserList.Find(x => x.UUID == (long)_pktInfo.tgtUids_.uids_[i]);
                if( friendUserData != null ) {
                    CommunityData.FriendList.Add( friendUserData );
                    CommunityData.FriendAskFromUserList.Remove( friendUserData );
                }
            }
        }
        else {
            //요청 제거
            for( int i = 0; i < _pktInfo.tgtUids_.uids_.Count; i++ ) {
                FriendUserData friendTo = CommunityData.FriendToAskList.Find(x => x.UUID == (long)_pktInfo.tgtUids_.uids_[i]);
                if( friendTo != null )
                    CommunityData.FriendToAskList.Remove( friendTo );

                FriendUserData friendFrom = CommunityData.FriendAskFromUserList.Find(x => x.UUID == (long)_pktInfo.tgtUids_.uids_[i]);
                if( friendFrom != null )
                    CommunityData.FriendAskFromUserList.Remove( friendFrom );
            }
        }

        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            NotificationManager.Instance.CheckALLFriend();
        }

        RecvProtocolData( _pktInfo );

        return true;
    }
    // 친구 제거 응답
    bool RecvAckFriendKick( HostID remote, RmiContext rmicontext, PktInfoCommuKick _pktInfo ) {
        for( int i = 0; i < _pktInfo.tgtUids_.uids_.Count; i++ ) {
            FriendUserData friendUserData = CommunityData.FriendList.Find(x => x.UUID == (long)_pktInfo.tgtUids_.uids_[i]);
            if( friendUserData != null ) {
                CommunityData.FriendList.Remove( friendUserData );
            }
        }

        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            NotificationManager.Instance.CheckALLFriend();
        }

        RecvProtocolData( _pktInfo );

        return true;
    }
    // 친구 포인트 보내기 응답
    bool RecvAckFriendPointGive( HostID remote, RmiContext rmicontext, PktInfoFriendPointGive _pktInfo ) {
        _userdata.Goods[(int)eGOODSTYPE.FRIENDPOINT] = (int)_pktInfo.userFriPoint_;
        _userdata.NextFrientPointGiveTime = GameSupport.GetLocalTimeByServerTime( _pktInfo.nextFriPTGiveTM_.GetTime() );

        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            NotificationManager.Instance.CheckALLFriend();
        }

        RecvProtocolData( _pktInfo );

        return true;
    }
    // 친구 포인트 받기 응답
    bool RecvAckFriendPointTake( HostID remote, RmiContext rmicontext, PktInfoFriendPointTakeAck _pktInfo ) {
        long addPoint = (long)_pktInfo.userFriPoint_ - _userdata.Goods[(int)eGOODSTYPE.FRIENDPOINT];

        _userdata.Goods[(int)eGOODSTYPE.FRIENDPOINT] = (long)_pktInfo.userFriPoint_;

        //3185 {0} 친구 포인트를 획득했습니다.
        MessageToastPopup.Show( string.Format( FLocalizeString.Instance.GetText( 3185 ), addPoint ) );

        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            NotificationManager.Instance.CheckALLFriend();
        }

        RecvProtocolData( _pktInfo );

        return true;
    }
    // 친구 프라이빗룸 입장 가능 상태 변경 응답
    bool RecvAckFriendRoomVisitFlag( HostID remote, RmiContext rmicontext, PktInfoFriendRoomFlag _pktInfo ) {
        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
            NotificationManager.Instance.CheckALLFriend();
        }

        //친구 브라이빗룸 입장 가능 상태 변경요청한 곳에서 uuid저장한걸로 갱신
        RecvProtocolData( _pktInfo );

        return true;
    }
    // 친구 프라이빗룸 정보 응답
    bool RecvAckFriendRoomInfoGet( HostID remote, RmiContext rmicontext, PktInfoRoomThemeSlotDetail _pktInfo ) {
        _friendRoomSlotData.SetPktData( _pktInfo.themeSlot_ );

        _friendRoomFigureSlotList.Clear();
        for( int i = 0; i < _pktInfo.figureSlot_.infos_.Count; i++ ) {
            RoomThemeFigureSlotData roomFigure = new RoomThemeFigureSlotData();
            roomFigure.SetPktData( _pktInfo.figureSlot_.infos_[i] );
            _friendRoomFigureSlotList.Add( roomFigure );
        }

        RecvProtocolData( _pktInfo );

        return true;
    }

    // 서버 달성(세력) 선택 응답
    bool RecvAckInfluenceChoice( HostID remote, RmiContext rmicontext, PktInfoInfluenceChoice _pktInfo ) {
        InfluenceMissionData.InfluID = _pktInfo.tid_;
        RecvProtocolData( _pktInfo );
        return true;
    }
    // 서버 달성(세력) 정보 응답
    bool RecvAckGetInfluenceInfo( HostID remote, RmiContext rmicontext, PktInfoInfluence _pktInfo ) {
        InfluenceData.SetPktData( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }
    // 서버 달성(세력) 랭킹 정보 응답
    bool RecvAckGetInfluenceRankInfo( HostID remote, RmiContext rmicontext, PktInfoRankInfluence _pktInfo ) {
        InfluenceRankData.SetPktData( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }
    // 서버 달성(세력) 보상 응답
    bool RecvAckInfluenceTgtRwd( HostID remote, RmiContext rmicontext, PktInfoRwdInfluenceTgtAck _pktInfo ) {
        ApplyProduct( _pktInfo.products_, false, false );
        InfluenceMissionData.TgtRwdFlag = _pktInfo.retTgtRwdFlag_;
        RecvProtocolData( _pktInfo );
        return true;
    }

    // 유저 공적 보상 획득
    bool RecvAckRewardTakeAchieve( HostID remote, RmiContext rmicontext, PktInfoAchieveReward _pktInfo ) {
        ApplyProduct(_pktInfo.products_, false, false);
        ApplyPktInfoAchieve( _pktInfo.achieve_ );
        RecvProtocolData( _pktInfo );

        return true;
    }
    // 지휘관 마크 목록
    bool RecvAckUserMarkList( HostID remote, RmiContext rmicontext, PktInfoTIDList _pktInfo ) {
        _usermarklist.Clear();
        AddPktUserMarkList( _pktInfo );

        RecvProtocolData( _pktInfo );

        return true;
    }
    // 지휘관 마크 설정
    bool RecvAckUserSetMark( HostID remote, RmiContext rmicontext, System.UInt32 _userMarkTID ) {
        _userdata.UserMarkID = (int)_userMarkTID;

        RecvProtocolData( (long)_userMarkTID );

        return true;
    }
    // 지휘관 로비 테마 목록
    bool RecvAckUserLobbyThemeList( HostID remote, RmiContext rmicontext, PktInfoTIDList _pktInfo ) {
        _UserLobbyThemeList.Clear();
        AddUserLobbyThemeList( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }
    // 지휘관 로비 테마 설정
    bool RecvAckUserSetLobbyTheme( HostID remote, RmiContext rmicontext, System.UInt32 _lobbyThemeTID ) {
        _userdata.UserLobbyThemeID = (int)_lobbyThemeTID;
        RecvProtocolData( (long)_lobbyThemeTID );
        return true;
    }
    // 지휘관 대표 카드(서포터) 진형 설정
    bool RecvAckUserSetMainCardFormation( HostID remote, RmiContext rmicontext, System.UInt32 _cardFrmtID ) {
        UserData.CardFormationID = (int)_cardFrmtID;
        RecvProtocolData( null );
        return true;
    }
    // 카드(서포터) 진형 즐겨찾기 설정
    bool RecvAckUserCardFormationFavi( HostID remote, RmiContext rmicontext, PktInfoCardFormaFavi _pkt ) {
        SetCardFormationFavor( _pkt );
        RecvProtocolData( _pkt );
        return true;
    }

    /// <summary>
    /// 지휘관 이름 설정
    /// </summary>
    bool RecvAckUserSetName( HostID remote, RmiContext rmicontext, PktInfoStr _pktInfo ) {
        _userdata.SetNickName( _pktInfo.str_ );
        RecvProtocolData( _pktInfo );

        return true;
    }

    bool RecvAckUserSetNameColor( HostID remote, RmiContext rmicontext, UInt32 colorId ) {
        _userdata.NickNameColorId = (int)colorId;
        RecvProtocolData( colorId );

        return true;
    }

    // 지휘관 인사말 설정
    bool RecvAckUserSetCommentMsg( HostID remote, RmiContext rmicontext, PktInfoStr _pktInfo ) {
        _userdata.UserWord = _pktInfo.str_;

        RecvProtocolData( _pktInfo );
        return true;
    }
    // 국가 및 언어 설정
    bool RecvAckUserSetCountryAndLangCode( HostID remote, RmiContext rmicontext, PktInfoCountryLangCode _pktInfo ) {
        Log.Show( _pktInfo.country_.str_ + " / " + _pktInfo.lang_.str_ );

        RecvProtocolData( _pktInfo );

        return true;
    }

    bool RecvAckUserPkgShowOff( HostID remote, RmiContext rmicontext ) {
        _userdata.ShowPkgPopup = false;
        RecvProtocolData( null );

        return true;
    }

    //  로그인 보너스 확인
    bool RecvAckReflashLoginBonus( HostID remote, RmiContext rmicontext, PktInfoLoginBonus _pktInfo ) {
        UserData.LoginBonusGroupID = (int)_pktInfo.lgnGID_;
        UserData.LoginBonusGroupCnt = _pktInfo.lgnGroupCnt_;

        UserData.LoginBonusRecentDate = _pktInfo.lastBonusLgnTM_.GetTime();
        UserData.LoginTotalCount = _pktInfo.totalLgnCnt_;
        UserData.LoginContinuityCount = _pktInfo.continueLgnCnt_;

        // 서버에서 끝나는 기간이 더 길게 세팅되어 있어서 내가 보상을 다 받았어도 로그인 이벤트 정보가 들어올 수 있음.
        // 보상 다 받고 나면 로그인 이벤트 창 안보여 줄건지는 기획팀장님과 상의
        UserData.SetPktEvtLoginData( _pktInfo.evtLgn_ );

        foreach( CharData data in CharList ) {
            if( 0 < _pktInfo.preCnt_ ) {
                data.FavorPreCnt = _pktInfo.preCnt_;
            }

            if( 0 < _pktInfo.scrCnt_ ) {
                data.SecretQuestCount = _pktInfo.scrCnt_;
            }
        }

        //요일던전 내일 시간 갱신
        DateTime tomorrow = UserData.LoginBonusRecentDate.AddDays(1);
        ServerData.DayRemainTime = new DateTime( tomorrow.Year, tomorrow.Month, tomorrow.Day );

        ApplyPktMission( _pktInfo.mission_ );

        if( _pktInfo.raidHP_ > 0 ) {
            ResetRaidCharHp();
		}

        RaidUserData.DailyRaidPoint = _pktInfo.dailyLimitPoint_;
        _userdata.LoginBonusMonthlyCount = _pktInfo.nowLgnBonusMonthlyCnt_;

        RecvProtocolData( _pktInfo );
        return true;
    }

    //  일일 미션 보상 받기
    bool RecvAckRewardDailyMission( HostID remote, RmiContext rmicontext, PktInfoRwdDailyMissionAck _pkt ) {
        ApplyProduct( _pkt.products_, false, true );
        for( int i = 0; i < DailyMissionData.Infos.Count; i++ ) {
            if( DailyMissionData.Infos[i].GroupID == _pkt.groupID_ && DailyMissionData.Infos[i].Day == _pkt.day_ ) {
                DailyMissionData.Infos[i].RwdFlag = _pkt.retRwdFlag_;
                break;
            }
        }
        RecvProtocolData( _pkt );
        return true;
    }
    //  주간 미션 보상 받기
    bool RecvAckRewardWeekMission( HostID remote, RmiContext rmicontext, uint rewardFlag ) {
        var weeklyMission = GameTable.FindWeeklyMissionSet((int)WeekMissionData.fWeekMissionSetID);
        var rewardGroup = GameTable.FindAllRandom(a => a.GroupID == weeklyMission.RewardGroupID);

        uint oldFlag = WeekMissionData.fMissionRewardFlag;

        RewardList.Clear();

        for( int i = 0; i < 10; i++ ) {
            if( GameSupport.IsComplateMissionRecive( rewardFlag, i ) == true && GameSupport.IsComplateMissionRecive( oldFlag, i ) == false ) {
                RewardList.Add( new RewardData( rewardGroup[i].ProductType, rewardGroup[i].ProductIndex, rewardGroup[i].ProductValue ) );
            }
        }

        WeekMissionData.fMissionRewardFlag = rewardFlag;

        RecvProtocolData( (long)rewardFlag );

        return true;
    }

    // 유저 서버 달성(세력) 미션 보상 응답
    bool RecvAckRewardInfluMission( HostID remote, RmiContext rmicontext, PktInfoRwdInfluMissionAck _pktInfo ) {
        InfluenceMissionData.RwdFlag = _pktInfo.retRwdFlag_;
        AddPktItemList( _pktInfo.item_.rets_ );

        RecvProtocolData( _pktInfo );
        return true;
    }

    bool RecvAckUpdateGllaMission( HostID remote, RmiContext rmicontext, PktInfoUpdateGllaMission _pktInfo ) {
        RecvProtocolData( _pktInfo );

        Log.Show( _pktInfo );
        return true;
    }
    bool RecvAckRewardGllaMission( HostID remote, RmiContext rmicontext, PktInfoMission.Guerrilla _pktInfo ) {
        AddPktMissionGuerrilla( _pktInfo );
        //GameInfo.Instance.Send_ReqUpdateGllaMission();
        RecvProtocolData( _pktInfo );
        return true;
    }

    bool RecvAckRewardPassMission( HostID remote, RmiContext rmicontext, PktInfoMission.Pass _pktInfo ) {
        UpdatePktPassMissionData( _pktInfo );
        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckRewardPass( HostID remote, RmiContext rmicontext, PktInfoRwdPassAck _pktInfo ) {
        SetPktPassSetData( _pktInfo.pass_ );
        RecvProtocolData( _pktInfo );
        return true;
    }

    bool RecvAckEventRewardReset( HostID remote, RmiContext rmiContext, PktInfoEventRewardReset _pktInfo ) {
        if( _pktInfo.add_ != null && _pktInfo.add_.infos_ != null && _pktInfo.add_.infos_.Count > 0 ) {
            //EventSetData ed = _eventSetDataList.Find(x => x.TableData.EventID == _pktInfo.eventID_);
            //if(ed != null)
            //{
            //    ed.RewardStep = (int)(_pktInfo.add_.infos_[0].value_ + 1);
            //}

            //for (int i = 0; i < _pktInfo.add_.infos_.Count; i++)
            //{
            //    List<EventResetRewardData> rd = _eventResetRewardDataList.FindAll(x => x.TableID == _pktInfo.add_.infos_[i].tableID_);
            //    if(rd != null)
            //    {
            //        rd = rd.FindAll(x => x.RewardStep == _pktInfo.add_.infos_[i].step_);

            //        for (int j = 0; j < rd.Count; j++)
            //        {
            //            rd[j].SetPktData(_pktInfo.add_.infos_[i]);
            //        }
            //    }
            //}

            EventSetData eventRewardData = _eventSetDataList.Find(x => x.TableID == _pktInfo.eventID_);
            if( eventRewardData != null ) {
                eventRewardData.SetPktData( _pktInfo.add_.infos_[0] );
            }
        }

        if( _pktInfo.update_ != null && _pktInfo.update_.infos_ != null && _pktInfo.update_.infos_.Count > 0 ) {
            //EventSetData ed = _eventSetDataList.Find(x => x.TableData.EventID == _pktInfo.eventID_);
            //if(ed != null)
            //{
            //    ed.RewardStep = (int)(_pktInfo.update_.infos_[0].value_ + 1);
            //}


            //for (int i = 0; i < _pktInfo.update_.infos_.Count; i++)
            //{
            //    List<EventResetRewardData> rd = _eventResetRewardDataList.FindAll(x => x.TableID == _pktInfo.update_.infos_[i].tableID_);
            //    if (rd != null)
            //    {
            //        rd = rd.FindAll(x => x.RewardStep == _pktInfo.update_.infos_[i].step_);
            //        for (int j = 0; j < rd.Count; j++)
            //        {
            //            rd[j].SetPktData(_pktInfo.update_.infos_[i]);
            //        }
            //    }

            //}

            EventSetData eventRewardData = _eventSetDataList.Find(x => x.TableID == _pktInfo.eventID_);
            if( eventRewardData != null ) {
                eventRewardData.UpdatePktData( _pktInfo.update_.infos_[0] );
            }
        }

        RecvProtocolData( _pktInfo );
        return true;
    }
    bool RecvAckEventRewardTake( HostID remote, RmiContext rmiContext, PktInfoEventRewardTake _pktInfo ) {
        OnReceiveCallBack callback = GetCallBackList("Send_ReqEventRewardTake");

        //소모아이템 감소(이벤트 티켓 등)
        ApplyPktInfoMaterItem( _pktInfo.maters_ );

        //재화 보상
        ApplyProduct( _pktInfo.products_, false, false );

        //가챠 아이템별 남은갯수 최신화
        for( int i = 0; i < _pktInfo.reward_.infos_.Count; i++ ) {
            EventSetData eventRewardData = _eventSetDataList.Find(x => x.TableID == _pktInfo.reward_.infos_[i].tableID_);
            if( eventRewardData != null ) {
                GameTable.EventSet.Param eventTableData = GameInfo.Instance.GameTable.FindEventSet(x => x.EventID == eventRewardData.TableID);
                if( eventTableData.EventType == (int)eEventRewardKind.RESET_LOTTERY ) {
                    eventRewardData.SetPktData( _pktInfo.reward_.infos_[i] );
                }
                else if( eventTableData.EventType == (int)eEventRewardKind.EXCHANGE ) {
                    eventRewardData = _eventSetDataList.Find( x => x.TableID == _pktInfo.reward_.infos_[i].tableID_ && x.RewardStep == (int)_pktInfo.reward_.infos_[i].step_ );
                    if( eventRewardData != null )
                        eventRewardData.UpdatePktData( _pktInfo.reward_.infos_[i] );
                }
                else if( eventTableData.EventType == (int)eEventRewardKind.MISSION ) {

                }

            }
        }

        RecvProtocolData( _pktInfo );

        return true;
    }
    bool RecvAckEventLgnRewardTake( HostID remote, RmiContext rmiContext, PktInfoEvtLgnRwdAck _pkt ) {
        ApplyPktInfoMaterItemGoods( _pkt.consume_ );
        _userdata.UpdatePktEvtLoginData( _pkt.evtLgn_ );

        RecvProtocolData( _pkt );

        return true;
    }
    // 튜토리얼 값 변경 응답
    bool RecvAckSetTutorialVal( HostID remote, RmiContext rmiContext, System.UInt32 _tutoVal ) {
        GameInfo.Instance.UserData.TutorialNum = (int)_tutoVal;

        RecvProtocolData( (long)_tutoVal );
        return true;
    }
    // 튜토리얼 플래그 변경 응답
    bool RecvAckSetTutorialFlag( HostID remote, RmiContext rmiContext, System.UInt64 _tutoFlag ) {
        GameInfo.Instance.UserData.TutorialFlag = (long)_tutoFlag;

        RecvProtocolData( (long)_tutoFlag );

        return true;
    }

    bool RecvGetTimeRecordList( HostID remote, RmiContext rmiContext, PktInfoTimeAtkStageRec _pktInfo ) {
        RecvProtocolData( _pktInfo );

        return true;
    }

    public bool RecvAccountDelete( HostID remote, RmiContext rmiContext, PktInfoAccountDelete pkt ) {
        RecvProtocolData( pkt );
        return true;
    }

    public bool RecvAckBingoEventReward( HostID remote, RmiContext rmiContext, PktInfoBingoEventRewardAck pkt ) {
        ApplyProduct( pkt.products_, false );
        ApplyPktInfoUserBingoEvent( pkt.bingoInfo_ );

        RecvProtocolData( pkt );
        return true;
    }

    public bool RecvAckBingoNextOpen( HostID remote, RmiContext rmiContext, PktInfoBingoEvtNextRoundOpen pkt ) {
        ApplyPktInfoMaterItemGoods( pkt.consume_ );
        ApplyPktInfoUserBingoEvent( pkt.evtBingo_ );

        RecvProtocolData( pkt );
        return true;
    }

    public bool RecvAckRewardTakeAchieveEvent( HostID remote, RmiContext rmiContext, PktInfoAchieveEventReward pkt ) {
        ApplyProduct( pkt.products_, false, false );
        ApplyPktAchieveEvents( pkt.achieveEvent_ );

        RecvProtocolData( pkt );
        return true;
    }

    public bool RecvAckGetUserPresetList( HostID remote, RmiContext rmiContext, PktInfoUserPreset pkt ) {
        foreach( PktInfoUserPreset.Piece piece in pkt.infos_ ) {
            ApplyPresetData( piece );
        }

        RecvProtocolData( pkt );
        return true;
    }

    public bool RecvAckAddOrUpdateUserPreset( HostID remote, RmiContext rmiContext, PktInfoUserPreset.Piece pkt ) {
        ApplyPresetData( pkt );

        RecvProtocolData( pkt );
        return true;
    }

    public bool RecvAckUserPresetLoad( HostID remote, RmiContext rmiContext, PktInfoUserPresetLoad pkt ) {
        //!@# 멀티 무기 개선 작업 확인 필요 #@!
        foreach( PktInfoCharEquipWeapon charEquipWeapon in pkt.affectOtherChars_ ) {
            var chardata = GetCharData((long)charEquipWeapon.skinColor_.cuid_);
            if( chardata == null ) {
                continue;
            }

            chardata.EquipWeaponUID = (long)charEquipWeapon.wpns_[(int)eWeaponSlot.MAIN].wpnUID_;
            chardata.EquipWeaponSkinTID = (int)charEquipWeapon.wpns_[(int)eWeaponSlot.MAIN].wpnSkin_;

            chardata.EquipWeapon2UID = (long)charEquipWeapon.wpns_[(int)eWeaponSlot.SUB].wpnUID_;
            chardata.EquipWeapon2SkinTID = (int)charEquipWeapon.wpns_[(int)eWeaponSlot.SUB].wpnSkin_;

            chardata.SetPktSkinColorData( charEquipWeapon.skinColor_ );
        }

        foreach( PktInfoUserPresetLoad.DetachGem detachGem in pkt.detachGemInfo_ ) {
            WeaponData weaponData = GetWeaponData((long)detachGem.wpnUID_);
            if( weaponData == null ) {
                continue;
            }

            for( int i = 0; i < detachGem.gemSlotInfo_.Length; i++ ) {
                if( i < weaponData.SlotGemUID.Length ) {
                    weaponData.SlotGemUID[i] = (long)detachGem.gemSlotInfo_[i].gemUID_;
                }
            }
        }

        foreach( long uid in pkt.detachCards_.uids_ ) {
            CardData cardData = GetCardData(uid);
            if( cardData == null ) {
                continue;
            }

            CharData charData = GetCharData(cardData.PosValue);
            if( charData == null ) {
                continue;
            }

            cardData.InitPos();

            for( int i = 0; i < charData.EquipCard.Length; i++ ) {
                if( charData.EquipCard[i] == uid ) {
                    charData.EquipCard[i] = 0;
                    break;
                }
            }
        }

        foreach( long uid in pkt.detachBadge_.uids_ ) {
            BadgeData badgeData = GetBadgeData(uid);
            if( badgeData == null ) {
                continue;
            }

            badgeData.PosKind = 0;
            badgeData.PosSlotNum = 0;
            badgeData.PosValue = 0;
        }

        ApplyPresetDataInCharData( pkt.loadInfo_ );

        RecvProtocolData( pkt );
        return true;
    }

    public bool RecvAckUserPresetChangeName( HostID remote, RmiContext rmiContext, PktInfoUserPreset.Piece pkt ) {
        ApplyPresetData( pkt );

        RecvProtocolData( pkt );
        return true;
    }

    public bool RecvInitRaidSeasonData( HostID remote, RmiContext rmiContext, PktInfoInitRaidSeasonData pkt ) {
        ApplyPktInitRaidSeasonData( pkt );
        RecvProtocolData( pkt );

        return true;
    }

    public bool RecvAckRaidRankingList( HostID remote, RmiContext rmiContext, PktInfoRaidRankStageList pkt ) {
        ApplyPktInfoRaidRankStageList( pkt );
        RecvProtocolData( pkt );

        return true;
    }

    public bool RecvAckRaidFirstRankingList( HostID remote, RmiContext rmiContext, PktInfoRaidRankStageList pkt ) {
        ApplyPktInfoRaidFirstRankStageList( pkt );
        RecvProtocolData( pkt );

        return true;
    }

    public bool RecvAckRaidRankerDetail( HostID remote, RmiContext rmiContext, PktInfoRaidRankerDetailAck pkt ) {
        ApplyPktInfoRaidRankerDetailAck( pkt, false );
        RecvProtocolData( pkt );

        return true;
    }

    public bool RecvAckFirstRaidRankerDetail( HostID remote, RmiContext rmiContext, PktInfoRaidRankerDetailAck pkt ) {
        ApplyPktInfoRaidRankerDetailAck( pkt, true );
        RecvProtocolData( pkt );

        return true;
    }

    public bool RecvAckSetRaidTeam( HostID remote, RmiContext rmiContext, PktInfoUserRaidTeam pkt ) {
        RaidUserData.SetPktTeamInfo( pkt );
        RecvProtocolData( pkt );

        return true;
    }

    public bool RecvRaidHPRestore( HostID remote, RmiContext rmiContext, PktInfoRaidRestoreHPAck pkt ) {
        ApplyPktInfoMaterItem( pkt.consume_ );

        CharData charData = GetCharData( (long)pkt.cuid_ );
        charData.RaidHpPercentage = (float)pkt.raidHP_ / 100.0f;

        RecvProtocolData( pkt );
        return true;
    }

    public bool RecvRaidStageDrop( HostID remote, RmiContext rmiContext, PktInfoRaidStageDrop pkt ) {
        if( pkt.raidCUIDs_ != null && pkt.raidCUIDs_.Length > 0 ) {
            for( int i = 0; i < pkt.raidCUIDs_.Length; i++ ) {
                CharData charData = GetCharData( (long)pkt.raidCUIDs_[i] );
                charData.RaidHpPercentage = (float)pkt.raidCharHPs_[i] / 100.0f;
            }
        }

        RecvProtocolData( pkt );
        return true;
    }

    /// <summary>
    /// 레이드 초기화 종료 시 알림
    /// </summary>
    public bool RecvNotiUpdateRaidTimeEnd( HostID remote, RmiContext rmiContext, PktInfoRaidSeasonTime pkt, UInt32 openValueType ) {
        _serverdata.RaidSeasonEndTime = GameSupport.GetLocalTimeByServerTime( pkt.endTime_.GetTime() );
        _serverdata.RaidCurrentSeason = (int)openValueType;

        return true;
    }
    
    /// <summary>
    /// 레이드 초기화 시작 시 알림
    /// </summary>
    public bool RecvNotiUpdateRaidInitStart(HostID remote, RmiContext rmiContext)
    {
        if( GameSupport.IsRaidEnd() ) {
            if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage && World.Instance.StageType == eSTAGETYPE.STAGE_RAID ) {
                World.Instance.Pause( true, false );
                MessagePopup.OK( eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText( 3339 ), World.Instance.ForceQuit );
            }
            else if( LobbyUIManager.Instance.PanelType == ePANELTYPE.RAID_MAIN || LobbyUIManager.Instance.PanelType == ePANELTYPE.RAID ) {
                LobbyUIManager.Instance.HomeBtnEvent();
            }
        }

        return true;
    }

    public bool RecvAckCharLvUnexpectedPackageHardOpen(HostID remote, RmiContext rmiContext, PktInfoUnexpectedPackage  pkt)
    {
        AddPktUnexpectedPackage(pkt);
        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클(길드) 개설 요청 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleOpen(HostID remote, RmiContext rmiContext, PktInfoCircleOpenAck pkt)
    {
        _circleData.SetPktInfoCircleSimple(pkt.simpleInfo_);
        _circleData.SetPktInfoCircle(pkt.info_);

        _circleData.Uid = (long)pkt.authInfo_.ID_;
        _userdata.CircleAuthLevel.AuthLevel = pkt.authInfo_.authLv_;

        ApplyPktInfoMaterItemGoods(pkt.consume_);

        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 추천 리스트 요청에 대한 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckSuggestCircleList(HostID remote, RmiContext rmiContext, PktInfoGetSuggestCircleAck pkt)
    {
        CircleRecommendList.Clear();
        foreach (PktInfoCircleSimple.Piece info in pkt.list_.infos_)
        {
            CircleData circleData = new CircleData();
            circleData.SetPktInfoCircleSimple(info);
            CircleRecommendList.Add(circleData);
        }

        CircleJoinList.Clear();
        foreach (PktInfoCircleSimple.Piece info in pkt.joinReqList_.infos_)
        {
            CircleData circleData = new CircleData();
            circleData.SetPktInfoCircleSimple(info);
            CircleJoinList.Add(circleData);
        }

        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 가입신청 요청에 대한 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleJoin(HostID remote, RmiContext rmiContext, PktInfoGetSuggestCircleAck pkt)
    {
        CircleRecommendList.Clear();
        foreach (PktInfoCircleSimple.Piece info in pkt.list_.infos_)
        {
            CircleData circleData = new CircleData();
            circleData.SetPktInfoCircleSimple(info);
            CircleRecommendList.Add(circleData);
        }

        CircleJoinList.Clear();
        foreach (PktInfoCircleSimple.Piece info in pkt.joinReqList_.infos_)
        {
            CircleData circleData = new CircleData();
            circleData.SetPktInfoCircleSimple(info);
            CircleJoinList.Add(circleData);
        }

        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 가입신청 취소 요청에 대한 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="ccuid"></param>
    /// <returns></returns>
    public bool RecvAckCircleJoinCancel(HostID remote, RmiContext rmiContext, ulong ccuid)
    {
        CircleData circleData = CircleJoinList.Find(x => x.Uid == (long)ccuid);
        if (circleData != null)
        {
            CircleJoinList.Remove(circleData);
        }

        RecvProtocolData((long)ccuid);
        return true;
    }

    /// <summary>
    /// 서클 로비정보 요청 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleLobbyInfo(HostID remote, RmiContext rmiContext, PktInfoCircleLobby pkt)
    {
        _circleData.LobbySetId = (int)pkt.lobbySet_;
        _circleData.AttendanceCount = pkt.attendenceCnt_;

        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 탈퇴 요청에 대한 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleWithdrawal(HostID remote, RmiContext rmiContext, PktInfoCircleWithdrawalAck pkt)
    {
        _circleData.Uid = (long)pkt.authInfo_.ID_;
        _userdata.CircleAuthLevel.AuthLevel = pkt.authInfo_.authLv_;
        _userdata.CirclePossibleJoinTime = pkt.possibleCircleJoinDate_.GetTime();

        RecvProtocolData(pkt);
        return true;
    }
    
    /// <summary>
    /// 서클 해산에 대한 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleDisperse(HostID remote, RmiContext rmiContext, PktInfoTime pkt)
    {
        // Test - LeeSeungJin - Pkt
        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 유저리스트 조회 요청에 대한 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckGetCircleUserList(HostID remote, RmiContext rmiContext, PktInfoCircleUserList pkt)
    {
        _circleData.MemberList.Clear();
        foreach (PktInfoComCommuUser.Piece piece in pkt.userList_.infos_)
        {
            FriendUserData userData = new FriendUserData();
            userData.SetFriendUserData(piece);
            _circleData.MemberList.Add(userData);
        }

        _circleData.JoinWaitList.Clear();
        foreach (PktInfoComCommuUser.Piece piece in pkt.joinWaitList_.infos_)
        {
            FriendUserData userData = new FriendUserData();
            userData.SetFriendUserData(piece);
            _circleData.JoinWaitList.Add(userData);
        }

        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 가입대기 유저 상태 변경요청에 대한 응답 (서클 특정 권한 유저의 요청)
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleChangeStateJoinWaitUser(HostID remote, RmiContext rmiContext, PktInfoCircleUserList pkt)
    {
        _circleData.MemberList.Clear();
        foreach (PktInfoComCommuUser.Piece piece in pkt.userList_.infos_)
        {
            FriendUserData userData = new FriendUserData();
            userData.SetFriendUserData(piece);
            _circleData.MemberList.Add(userData);
        }

        _circleData.JoinWaitList.Clear();
        foreach (PktInfoComCommuUser.Piece piece in pkt.joinWaitList_.infos_)
        {
            FriendUserData userData = new FriendUserData();
            userData.SetFriendUserData(piece);
            _circleData.JoinWaitList.Add(userData);
        }

        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 유저 추방에 대한 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleUserKick(HostID remote, RmiContext rmiContext, PktInfoCircleUserKickAck pkt)
    {
        _circleData.NextUserKickPossibleTime = pkt.nextUserKickPossibleTime_.GetTime();

        _circleData.MemberList.Clear();
        foreach (PktInfoComCommuUser.Piece piece in pkt.userList_.userList_.infos_)
        {
            FriendUserData userData = new FriendUserData();
            userData.SetFriendUserData(piece);
            _circleData.MemberList.Add(userData);
        }

        _circleData.JoinWaitList.Clear();
        foreach (PktInfoComCommuUser.Piece piece in pkt.userList_.joinWaitList_.infos_)
        {
            FriendUserData userData = new FriendUserData();
            userData.SetFriendUserData(piece);
            _circleData.JoinWaitList.Add(userData);
        }

        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 유저 권한변경 요청에 대한 응답 
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleChangeAuthLevel(HostID remote, RmiContext rmiContext, PktInfoCircleChangeAuthority pkt)
    {
        if (_userdata.UUID == (long)pkt.targetUser_.uuid_)
        {
            _userdata.CircleAuthLevel.AuthLevel = pkt.targetUser_.authLevel_;
        }

        FriendUserData data = _circleData.MemberList.Find(x => x.UUID == (long)pkt.targetUser_.uuid_);
        if (data != null)
        {
            data.CircleAuthLevel.AuthLevel = pkt.targetUser_.authLevel_;
        }

        if (_userdata.UUID == (long)pkt.affectedUser_.uuid_)
        {
            _userdata.CircleAuthLevel.AuthLevel = pkt.affectedUser_.authLevel_;
        }

        data = _circleData.MemberList.Find(x => x.UUID == (long)pkt.affectedUser_.uuid_);
        if (data != null)
        {
            data.CircleAuthLevel.AuthLevel = pkt.affectedUser_.authLevel_;
        }

        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 마크변경 요청
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleChangeMark(HostID remote, RmiContext rmiContext, PktInfoCircleMarkSet pkt)
    {
        _circleData.FlagId = (int)pkt.flagTID_;
        _circleData.MarkId = (int)pkt.markTID_;
        _circleData.ColorId = (int)pkt.colorTID_;

        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 이름변경 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleChangeName(HostID remote, RmiContext rmiContext, PktInfoCircleChangeName pkt)
    {
        _circleData.Name = pkt.changeName_.str_;

        _userdata.SetPktData(pkt.consume_);

        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 소개문 변경 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleChangeComment(HostID remote, RmiContext rmiContext, PktInfoStr pkt)
    {
        _circleData.Content = pkt.str_;

        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 주사용 언어 변경 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="mlang"></param>
    /// <returns></returns>
    public bool RecvAckCircleChangeMainLanguage(HostID remote, RmiContext rmiContext, eLANGUAGE mlang)
    {
        _circleData.MainLanguage = mlang;

        RecvProtocolData((long)mlang);
        return true;
    }

    /// <summary>
    /// 서클 주사용 언어 외 가입옵션 변경응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public bool RecvAckCircleChangeSuggestAnotherLangOpt(HostID remote, RmiContext rmiContext, bool state)
    {
        _circleData.IsOtherLanguage = state;

        RecvProtocolData((long)(state ? eToggleType.On : eToggleType.Off));
        return true;
    }

    /// <summary>
    /// 서클 출석체크 요청 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleAttendance(HostID remote, RmiContext rmiContext, PktInfoCircleAttendanceRwd pkt)
    {
        _userdata.CircleAttendance.LastCheckDate = pkt.lastCircleAttendTM_.GetTime();
        _userdata.CircleAttendance.RewardGroupId = (int)pkt.circleAttendGroupID_;
        _userdata.CircleAttendance.RewardCount = pkt.circleAttendGroupCnt_;

        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 마크 구매요청 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleBuyMarkItem(HostID remote, RmiContext rmiContext, PktInfoCircleBuyMarkItem pkt)
    {
        _circleData.SetGoods(pkt.circleGoods_);
        _circleData.SetPktInfoCircleMark(pkt.ownedMark_);

        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 마크 리스트 요청 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleGetMarkList(HostID remote, RmiContext rmiContext, PktInfoGetCircleMarkList pkt)
    {
        _circleData.SetGoods(pkt.goods_);
        _circleData.SetPktInfoCircleMark(pkt.list_);

        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 검색요청 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleSearch(HostID remote, RmiContext rmiContext, PktInfoCircleSimple.Piece pkt)
    {
        CircleRecommendList.Clear();

        CircleData circleData = new CircleData();
        circleData.SetPktInfoCircleSimple(pkt);
        CircleRecommendList.Add(circleData);

        RecvProtocolData(pkt);
        return true;
    }

    /// <summary>
    /// 서클 채팅 리스트 요청 응답
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="rmiContext"></param>
    /// <param name="pkt"></param>
    /// <returns></returns>
    public bool RecvAckCircleChatList(HostID remote, RmiContext rmiContext, PktInfoCircleChat pkt)
    {
        CircleChatList.Clear();
        foreach (PktInfoCircleChat.Piece piece in pkt.infos_)
        {
            CircleChatData chatData = new CircleChatData();
            chatData.SetPktInfoCircleChat(piece);
            CircleChatList.Add(chatData);
        }

        CircleNotiList.Clear();
        foreach (PktInfoCircleNotification.Piece piece in pkt.notiMessage_.infos_)
        {
            CircleNotiData notiData = new CircleNotiData();
            notiData.SetPktInfoCircleNotification(piece);
            CircleNotiList.Add(notiData);
        }
        CircleNotiList.Reverse();

        RecvProtocolData(pkt);
        return true;
    }

	/// <summary>
	/// 서클 가입수락에 대한 알림 (알림 대상은 접속중인 유저에 한함)
	/// </summary>
	/// <param name="pkt"></param>
	/// <returns></returns>
	public bool RecvNotiCircleAcceptJoin( HostID remote, RmiContext rmiContext, PktInfoCircleAuthority pkt ) {
		_circleData.Uid = (long)pkt.ID_;
		_userdata.CircleAuthLevel.AuthLevel = pkt.authLv_;

		if ( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
			UICircleJoinPanel circleJoinPanel = LobbyUIManager.Instance.GetActiveUI<UICircleJoinPanel>( "CircleJoinPanel" );
			if ( circleJoinPanel != null ) {
				LobbyUIManager.Instance.SetPanelType( ePANELTYPE.MAIN );
				MessagePopup.OK( eTEXTID.OK, "서클 가입 되어 로비로 이동 하였습니다.", null ); // Test - LeeSeungJin - Change String
			}
		}

		return true;
	}

	/// <summary>
	/// 서클 유저 추방에 대한 알림 (알림 대상은 접속중인 유저에 한함)
	/// </summary>
	/// <param name="pkt"></param>
	/// <returns></returns>
	public bool RecvNotiCircleUserKick( HostID remote, RmiContext rmiContext, PktInfoCircleWithdrawalAck pkt ) {
		_userdata.CirclePossibleJoinTime = pkt.possibleCircleJoinDate_.GetTime();
		_circleData.Uid = (long)pkt.authInfo_.ID_;
		_userdata.CircleAuthLevel.AuthLevel = pkt.authInfo_.authLv_;

		return true;
	}

	/// <summary>
	/// 서클 유저 권한변경 알림(접속중)
	/// </summary>
	/// <param name="authLevel"></param>
	/// <returns></returns>
	public bool RecvNotiCircleUserChangeAuth( HostID remote, RmiContext rmiContext, eCircleAuthLevel authLevel ) {
		_userdata.CircleAuthLevel.AuthLevel = authLevel;

		return true;
	}

	/// <summary>
	/// 서클 새로운 채팅 메시지 알림(접속중 유저)
	/// </summary>
	/// <param name="pkt"></param>
	/// <returns></returns>
	public bool RecvNotiCircleChat( HostID remote, RmiContext rmiContext, PktInfoCircleChat.Piece pkt ) {
		CircleChatData chatData = null;
		if ( CircleChatList.Count <= CircleChatList.Capacity ) {
			chatData = CircleChatList.FirstOrDefault();
			if ( chatData != null ) {
				CircleChatList.RemoveAt( 0 );
			}
		}

		if ( chatData == null ) {
			chatData = new CircleChatData();
		}

		chatData.Uid = (long)pkt.uuid_;
		chatData.UserName = pkt.nickName_.str_;
		chatData.UserMarkId = (int)pkt.mark_;
		chatData.StampId = (int)pkt.stampID_;
		chatData.Content = pkt.msg_.str_;
		chatData.ChatTime = pkt.sendTm_.GetTime();

		CircleChatList.Add( chatData );

		if ( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
			LobbyUIManager.Instance.Renewal( "CircleLobbyPanel" );
		}

		return true;
	}

	/// <summary>
	/// 서클 전체 메시지 알림
	/// </summary>
	/// <param name="pkt"></param>
	/// <returns></returns>
	public bool RecvNotiCircleChatNotimessage( HostID remote, RmiContext rmiContext, PktInfoCircleNotification.Piece pkt ) {
		CircleNotiData notiData = new CircleNotiData();
		notiData.SetPktInfoCircleNotification( pkt );
		CircleNotiList.Insert( 0, notiData );

		return true;
	}

	/// <summary>
	/// 캐릭터 친밀도 버프 변경 응답
	/// </summary>
	/// <param name="remote"></param>
	/// <param name="rmiContext"></param>
	/// <param name="pktInfoUIDValue"></param>
	/// <returns></returns>
	public bool RecvAckChangePreferenceNum( HostID remote, RmiContext rmiContext, PktInfoUIDValue pktInfoUIDValue ) {
		for ( int i = 0; i < _userdata.ArrFavorBuffCharUid.Length; i++ ) {
			_userdata.ArrFavorBuffCharUid[i] = 0;
		}

		for ( int i = 0; i < pktInfoUIDValue.infos_.Count; i++ ) {
			if ( pktInfoUIDValue.infos_[i].val_ <= 0 ) {
				continue;
			}

			_userdata.ArrFavorBuffCharUid[pktInfoUIDValue.infos_[i].val_ - 1] = (long)pktInfoUIDValue.infos_[i].uid_;
		}

		RecvProtocolData( pktInfoUIDValue );
		return true;
	}
}
