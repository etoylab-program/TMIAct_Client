using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameInfo : FMonoSingleton<GameInfo>
{

    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  지울거
    //
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    private Dictionary<string, OnReceiveCallBack> _callbacklist = new Dictionary<string, OnReceiveCallBack>();
    public void ClearCallBackList()
    {
        _callbacklist.Clear();
    }

    private void SetCallBackList(string key, OnReceiveCallBack value)
    {
        if (_callbacklist.ContainsKey(key))
        {
            _callbacklist[key] = value;
        }
        else
        {
            _callbacklist.Add(key, value);
        }
    }

    private OnReceiveCallBack GetCallBackList(string key, bool isRemove = false)
    {
        OnReceiveCallBack outValue;
        if (_callbacklist.TryGetValue(key, out outValue))
        {
            if (isRemove)
                _callbacklist.Remove(key);
            return outValue;
        }
        return null;
    }

    private bool ContainsCallBackListKey(string key)
    {
        return _callbacklist.ContainsKey(key);
    }
    
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  지울거 끝
    //
    //----------------------------------------------------------------------------------------------------------------------------------------------------

    public bool IsProtocolData()
    {
        if(!_netflag)
        {
            ClearProtocolData();
            return false;
        }

        return _protocallist.Count != 0 ? true : false;
    }

    public void ClearProtocolData()
    {
        _protocallist.Clear();
    }
    public void AddProtocolData(ePacketType type, Nettention.Proud.RmiID id, PktMsgType pktdata, OnReceiveCallBack callback)
    {
        _protocallist.Add(new ProtocolData(type, id, pktdata, callback));
    }

    public void AddProtocolData(ePacketType type, Nettention.Proud.RmiID id, long pktnum, OnReceiveCallBack callback)
    {
        _protocallist.Add(new ProtocolData(type, id, pktnum, callback));
    }

    private void SendProtocolData( bool bwait = true, bool bresend = false )
    {
        if (_protocallist.Count == 0)
            return;
        var pdata = _protocallist[0];

        WaitPopup.Show();

        if (pdata.PacketType == ePacketType.Global)
        {
            if( pdata.PacketID == GlobalC2S.Common.ReqPing ) { }
            else if( pdata.PacketID == GlobalC2S.Common.ReqLogOnCreditKey ) { }
            else if( pdata.PacketID == GlobalC2S.Common.ReqLogOut ) { }
            else if( pdata.PacketID == GlobalC2S.Common.ReqReConnectUserInfo ) { }
            else if( pdata.PacketID == GlobalC2S.Common.ReqAccountCode ) {
                NETStatic.PktGbl.ReqAccountCode();
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqAccountSetPassword ) {
                NETStatic.PktGbl.ReqAccountSetPassword( pdata.PktMsgData as PktInfoStr );
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "AccountSetPassword" );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqLinkAccountList ) {
                NETStatic.PktGbl.ReqLinkAccountList();
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "LinkAccountList" );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqAddLinkAccountAuth ) {
                PktInfoAccountLinkInfo pktinfo = pdata.PktMsgData as PktInfoAccountLinkInfo;

                NETStatic.PktGbl.ReqAddLinkAccountAuth( pktinfo );
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "AddLinkAccountAuth", "LinkAccountType", pktinfo.type_.ToString() );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqGetUserInfoFromAccountLink ) { }
            else if( pdata.PacketID == GlobalC2S.Common.ReqPushNotifiTokenSet ) {
                NETStatic.PktGbl.ReqPushNotifiTokenSet( pdata.PktMsgData as PktInfoPushNotiSetToken );
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "PushNotifiTokenSet" );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqReflashLoginBonus ) {
                NETStatic.PktGbl.ReqReflashLoginBonus();
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "ReflashLoginBonus" );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRewardTakeAchieve ) {
                NETStatic.PktGbl.ReqRewardTakeAchieve( pdata.PktMsgData as PktInfoTIDList );

                PktInfoTIDList pktInfoTIDList = pdata.PktMsgData as PktInfoTIDList;

                for( int i = 0; i < pktInfoTIDList.tids_.Count; i++ )
                    Firebase.Analytics.FirebaseAnalytics.LogEvent( "RewardTakeAchieve", "GroupID", pktInfoTIDList.tids_[i] );

            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRewardDailyMission ) {
                NETStatic.PktGbl.ReqRewardDailyMission( pdata.PktMsgData as PktInfoRwdDailyMissionReq );

                PktInfoRwdDailyMissionReq pktInfoRewardDailyMission = pdata.PktMsgData as PktInfoRwdDailyMissionReq;

                for( int i = 0; i < pktInfoRewardDailyMission.idxs_.idxs_.vals_.Count; i++ )
                    Firebase.Analytics.FirebaseAnalytics.LogEvent( "RewardDailyMission", "DailyMissionIndex", ( (PktInfoMission.Daily.Piece.ENUM)pktInfoRewardDailyMission.idxs_.idxs_.vals_[i] ).ToString() );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRewardWeekMission ) {
                NETStatic.PktGbl.ReqRewardWeekMission( pdata.PktMsgData as PktInfoComRwd );

                PktInfoComRwd pktInfoRewardWeekMission = pdata.PktMsgData as PktInfoComRwd;

                for( int i = 0; i < pktInfoRewardWeekMission.idxs_.vals_.Count; i++ )
                    Firebase.Analytics.FirebaseAnalytics.LogEvent( "RewardWeekMission", "WeekMissionIndex", ( (PktInfoMission.Weekly.ENUM)pktInfoRewardWeekMission.idxs_.vals_[i] ).ToString() );

            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqEventRewardReset ) {
                NETStatic.PktGbl.ReqEventRewardReset( (uint)pdata.PktNumData );

                Firebase.Analytics.FirebaseAnalytics.LogEvent( "EventRewardReset", "EventTableID", (uint)pdata.PktNumData );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqEventRewardTake ) {
                NETStatic.PktGbl.ReqEventRewardTake( pdata.PktMsgData as PktInfoEventRewardReq );

                PktInfoEventRewardReq pktInfoEventRewardReq = pdata.PktMsgData as PktInfoEventRewardReq;

                Firebase.Analytics.Parameter[] parameters = {
                        new Firebase.Analytics.Parameter("EventTableID", (int)pktInfoEventRewardReq.eventID_),
                        new Firebase.Analytics.Parameter("EventRewardCount", pktInfoEventRewardReq.cnt_),
                        new Firebase.Analytics.Parameter("EventResetStep", pktInfoEventRewardReq.step_),
                    };
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "EventRewardTake", parameters );

            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRewardGllaMission ) {
                NETStatic.PktGbl.ReqRewardGllaMission( pdata.PktMsgData as PktInfoRwdGllaMission );

                PktInfoRwdGllaMission pktInfoRewardGllaMission = pdata.PktMsgData as PktInfoRwdGllaMission;

                for( int i = 0; i < pktInfoRewardGllaMission.groupIDs_.tids_.Count; i++ )
                    Firebase.Analytics.FirebaseAnalytics.LogEvent( "ReqRewardGllaMission", "GllaMissionGroupID", pktInfoRewardGllaMission.groupIDs_.tids_[i] );
                //Firebase.Analytics.FirebaseAnalytics.LogEvent("ReqRewardGllaMission", "GllaMissionGroupID", groupId.ToString());

            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqSetTutorialVal ) {
                NETStatic.PktGbl.ReqSetTutorialVal( (System.UInt32)pdata.PktNumData );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqSetTutorialFlag ) {
                NETStatic.PktGbl.ReqSetTutorialFlag( (System.UInt64)pdata.PktNumData );

                if( (System.UInt64)pdata.PktNumData != 0 )
                    Firebase.Analytics.FirebaseAnalytics.LogEvent( "TutorialFlag", "FlagIndex", pdata.PktNumData );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqAddCharacter ) {
                /*
                NETStatic.PktGbl.ReqAddCharacter((byte)pdata.PktNumData);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("AddCharacter", "CharTableID", pdata.PktNumData);
                */
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqChangeMainChar ) {
                /*
                PktInfoUIDList pkt = new PktInfoUIDList();
                pkt.uids_ = new List<System.UInt64>();
                pkt.uids_.Add((System.UInt64)pdata.PktNumData);
                pkt.uids_.Add(0);
                pkt.uids_.Add(0);
                */

                PktInfoUIDList pkt = pdata.PktMsgData as PktInfoUIDList;
                NETStatic.PktGbl.ReqChangeMainChar( pkt );

                Firebase.Analytics.FirebaseAnalytics.LogEvent( "ChangeMainChar", "CharTableID", GetCharDataTableID( (int)pdata.PktNumData ) );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqGradeUpChar ) {
                NETStatic.PktGbl.ReqGradeUpChar( (System.UInt64)pdata.PktNumData );
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "GradeUpChar", "CharTableID", GetCharDataTableID( (int)pdata.PktNumData ) );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqSetMainCostumeChar ) {
                NETStatic.PktGbl.ReqSetMainCostumeChar( pdata.PktMsgData as PktInfoCharSetMainCostumeReq );

                PktInfoCharSetMainCostumeReq pktInfoCharSetMainCostume = pdata.PktMsgData as PktInfoCharSetMainCostumeReq;

                Firebase.Analytics.Parameter[] parameters = {
                new Firebase.Analytics.Parameter("CostumeID", pktInfoCharSetMainCostume.costumeTID_),
                new Firebase.Analytics.Parameter("Costumecolor", pktInfoCharSetMainCostume.skinColor_.costumeClr_),
                new Firebase.Analytics.Parameter("Costumeflag", pktInfoCharSetMainCostume.skinColor_.skinStateFlag_)
                };
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "SetMainCostumeChar", parameters );

            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqEquipWeaponChar ) {
                //!@# 멀티 무기 개선 작업 확인 필요 #@!
                NETStatic.PktGbl.ReqEquipWeaponChar( pdata.PktMsgData as PktInfoCharEquipWeapon );

                PktInfoCharEquipWeapon pktInfoCharEquipWeapon = pdata.PktMsgData as PktInfoCharEquipWeapon;


                Firebase.Analytics.FirebaseAnalytics.LogEvent( "EquipWeaponChar", "WeaponTableID", GetWeaponData( (long)pktInfoCharEquipWeapon.wpns_[0].wpnUID_ ).TableID );


            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqApplySkillInChar ) {
                NETStatic.PktGbl.ReqApplySkillInChar( pdata.PktMsgData as PktInfoCharSlotSkill );

                PktInfoCharSlotSkill pktInfoCharSlotSkill = pdata.PktMsgData as PktInfoCharSlotSkill;

                Firebase.Analytics.Parameter[] parameters = new Firebase.Analytics.Parameter[pktInfoCharSlotSkill.skilTIDs_.Length];
                for( int i = 0; i < pktInfoCharSlotSkill.skilTIDs_.Length; i++ )
                    parameters[i] = new Firebase.Analytics.Parameter( "SkillTableID", pktInfoCharSlotSkill.skilTIDs_[i] );
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "ApplySkillInChar", parameters );


            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqLvUpSkill ) {
                NETStatic.PktGbl.ReqLvUpSkill( pdata.PktMsgData as PktInfoSkillLvUpReq );

                PktInfoSkillLvUpReq pktInfoSkillLvUpReq = pdata.PktMsgData as PktInfoSkillLvUpReq;

                Firebase.Analytics.FirebaseAnalytics.LogEvent( "LvUpSkill", "SkillTableID", pktInfoSkillLvUpReq.skillTID_ );

            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqLvUpUserSkill ) {
                NETStatic.PktGbl.ReqLvUpUserSkill( pdata.PktMsgData as PktInfoUserSklLvUpReq );

                PktInfoUserSklLvUpReq pktInfo = pdata.PktMsgData as PktInfoUserSklLvUpReq;
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "LvUpUserSkill", "SkillTableID", pktInfo.tid_ );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqResetUserSkill ) {
                NETStatic.PktGbl.ReqResetUserSkill();
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "ReqResetUserSkill" );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqStageStart ) {
                NETStatic.PktGbl.ReqStageStart( pdata.PktMsgData as PktInfoStageGameStartReq );

                PktInfoStageGameStartReq pktInfoStageGameStartReq = pdata.PktMsgData as PktInfoStageGameStartReq;

                Firebase.Analytics.Parameter[] parameters = {
                new Firebase.Analytics.Parameter("CharTableID", GetCharDataTableID((int)pktInfoStageGameStartReq.cuid_)),
                new Firebase.Analytics.Parameter("StageID", pktInfoStageGameStartReq.stageTID_),
                new Firebase.Analytics.Parameter("TicketMultiple", pktInfoStageGameStartReq.ticketMultipleIdx_),
                };
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "StageStart", parameters );

            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqStageEnd ) {
                NETStatic.PktGbl.ReqStageEnd( pdata.PktMsgData as PktInfoStageGameResultReq );

                PktInfoStageGameResultReq pktInfoStageGameResultReq = pdata.PktMsgData as PktInfoStageGameResultReq;

                Firebase.Analytics.Parameter[] parameters = {
                new Firebase.Analytics.Parameter("ClearTime", pktInfoStageGameResultReq.playData_.clearTime_Ms),
                new Firebase.Analytics.Parameter("GoldCount", pktInfoStageGameResultReq.dropGoldItemCnt_),
                new Firebase.Analytics.Parameter("TakeBoxCount", pktInfoStageGameResultReq.nTakeBoxCnt_),
                new Firebase.Analytics.Parameter("Mission1", pktInfoStageGameResultReq.mission0_.ToString()),
                new Firebase.Analytics.Parameter("Mission2", pktInfoStageGameResultReq.mission1_.ToString()),
                new Firebase.Analytics.Parameter("Mission3", pktInfoStageGameResultReq.mission2_.ToString()),
            };
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "StageEnd", parameters );

            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqStageEndFail ) {
                NETStatic.PktGbl.ReqStageEndFail( pdata.PktMsgData as PktInfoStageGameEndFail );

                PktInfoStageGameEndFail pktInfoStageGameEndFail = pdata.PktMsgData as PktInfoStageGameEndFail;

                Firebase.Analytics.FirebaseAnalytics.LogEvent( "StageEndFail", "PlayTIme", pktInfoStageGameEndFail.playTime_Ms_ );

            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqBookNewConfirm ) {
                NETStatic.PktGbl.ReqBookNewConfirm( pdata.PktMsgData as PktInfoBookNewConfirm );

                PktInfoBookNewConfirm pktInfoBookNewConfirm = pdata.PktMsgData as PktInfoBookNewConfirm;

                Firebase.Analytics.Parameter[] parameters = {
                new Firebase.Analytics.Parameter("CardBookTableID", pktInfoBookNewConfirm.bookTID_),
                new Firebase.Analytics.Parameter("CardBookType", pktInfoBookNewConfirm.bookGroup_.ToString())
            };
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "BookNewConfirm", parameters );

            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqTimeAtkRankingList ) {
                NETStatic.PktGbl.ReqTimeAtkRankingList( pdata.PktMsgData as PktInfoTimeAtkRankingHeader );
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "TimeAtkRankingList" );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqTimeAtkRankerDetail ) {
                NETStatic.PktGbl.ReqTimeAtkRankerDetail( pdata.PktMsgData as PktInfoTimeAtkRankerDetailReq );

                PktInfoTimeAtkRankerDetailReq pktInfoTimeAtkRankerDetailReq = pdata.PktMsgData as PktInfoTimeAtkRankerDetailReq;

                Firebase.Analytics.FirebaseAnalytics.LogEvent( "TimeAtkRankerDetail", "StageID", pktInfoTimeAtkRankerDetailReq.stageID_ );

            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqArenaRankingList )     //아레나
            {
                //랭킹 정보 요청
                NETStatic.PktGbl.ReqArenaRankingList( (ulong)pdata.PktNumData );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqArenaRankerDetail ) {
                //랭커 상세 정보 요청
                NETStatic.PktGbl.ReqArenaRankerDetail( (ulong)pdata.PktNumData );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqSetArenaTeam ) {
                //팀편성
                NETStatic.PktGbl.ReqSetArenaTeam( pdata.PktMsgData as PktInfoUserArenaTeam );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqArenaSeasonPlay ) {
                //시즌 참가 요청
                NETStatic.PktGbl.ReqArenaSeasonPlay();
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqArenaEnemySearch ) {
                //대전 상대 찾기
                NETStatic.PktGbl.ReqArenaEnemySearch();
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqArenaGameStart ) {
                //아레나 대전 시작
                NETStatic.PktGbl.ReqArenaGameStart( pdata.PktMsgData as PktInfoArenaGameStartReq );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqArenaGameEnd ) {
                NETStatic.PktGbl.ReqArenaGameEnd( pdata.PktMsgData as PktInfoArenaGameEndReq );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRefrashUserInfo ) {
                //특정 유저의 현재 값 요청 - 0319 추가
                NETStatic.PktGbl.ReqRefrashUserInfo();
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRewardPass ) {
                NETStatic.PktGbl.ReqRewardPass( pdata.PktMsgData as PktInfoRwdPassReq );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRewardPassMission ) {
                NETStatic.PktGbl.ReqRewardPassMission( pdata.PktMsgData as PktInfoRwdPassMission );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqUpdateGllaMission ) // 게릴라 미션 추가
            {
                NETStatic.PktGbl.ReqUpdateGllaMission( pdata.PktMsgData as PktInfoUpdateGllaMission );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqSetArenaTowerTeam ) {
                NETStatic.PktGbl.ReqSetArenaTowerTeam( pdata.PktMsgData as PktInfoUserArenaTeam );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqArenaTowerGameStart ) {
                NETStatic.PktGbl.ReqArenaTowerGameStart( pdata.PktMsgData as PktInfoArenaTowerGameStartReq );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqArenaTowerGameEnd ) {
                NETStatic.PktGbl.ReqArenaTowerGameEnd( pdata.PktMsgData as PktInfoArenaTowerGameEndReq );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRelocateUserInfoSet ) {
                if( !AppMgr.Instance.HasContentFlag( eContentType.SERVER_RELOCATE ) )
                    return;

                NETStatic.PktGbl.ReqRelocateUserInfoSet( pdata.PktMsgData as PktInfoRelocateUser );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRelocateUserComplate ) {
                if( !AppMgr.Instance.HasContentFlag( eContentType.SERVER_RELOCATE ) )
                    return;

                NETStatic.PktGbl.ReqRelocateUserComplate( pdata.PktMsgData as PktInfoRelocateUser );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRewardInfluMission ) {
                NETStatic.PktGbl.ReqRewardInfluMission( pdata.PktMsgData as PktInfoComRwd );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqGetTotlaRelocateCntToNotComplete ) {
                NETStatic.PktGbl.ReqGetTotlaRelocateCntToNotComplete();
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqAccountLinkReward ) {
                NETStatic.PktGbl.ReqAccountLinkReward();
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqAccountCodeReward ) {
                NETStatic.PktGbl.ReqAccountCodeReward();
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRandomCostumeDyeing ) {
                NETStatic.PktGbl.ReqRandomCostumeDyeing( (UInt32)pdata.PktNumData );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqSetCostumeDyeing ) {
                NETStatic.PktGbl.ReqSetCostumeDyeing( pdata.PktMsgData as PktInfoSetCostumeDyeingReq );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqCostumeDyeingLock ) {
                NETStatic.PktGbl.ReqCostumeDyeingLock( pdata.PktMsgData as PktInfoCostumeDyeingLock );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqUserCostumeColor ) {
                NETStatic.PktGbl.ReqUserCostumeColor( (UInt32)pdata.PktNumData );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqGivePresentChar ) {
                NETStatic.PktGbl.ReqGivePresentChar( pdata.PktMsgData as PktInfoGivePresentCharReq );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqUnexpectedPackageDailyReward ) {
                NETStatic.PktGbl.ReqUnexpectedPackageDailyReward( pdata.PktMsgData as PktInfoUnexpectedPackageDailyRewardReq );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqResetSecretCntChar ) {
                NETStatic.PktGbl.ReqResetSecretCntChar( pdata.PktMsgData as PktInfoUIDList );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqEventLgnRewardTake ) {
                NETStatic.PktGbl.ReqEventLgnRewardTake( pdata.PktMsgData as PktInfoEvtLgnRwdReq );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqAccountDelete ) {
                NETStatic.PktGbl.ReqAccountDelete();
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRewardTakeAchieveEvent ) {
                NETStatic.PktGbl.ReqRewardTakeAchieveEvent( pdata.PktMsgData as PktInfoTIDList );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqGetUserPresetList ) {
                NETStatic.PktGbl.ReqGetUserPresetList( pdata.PktMsgData as PktInfoPresetCommon );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqAddOrUpdateUserPreset ) {
                NETStatic.PktGbl.ReqAddOrUpdateUserPreset( pdata.PktMsgData as PktAddOrUpdatePreset );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqUserPresetLoad ) {
                NETStatic.PktGbl.ReqUserPresetLoad( pdata.PktMsgData as PktInfoPresetCommon );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqUserPresetChangeName ) {
                NETStatic.PktGbl.ReqUserPresetChangeName( pdata.PktMsgData as PktInfoPresetCommon );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqInitRaidSeasonData ) {
                NETStatic.PktGbl.ReqInitRaidSeasonData();
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRaidRankingList ) {
                NETStatic.PktGbl.ReqRaidRankingList( pdata.PktMsgData as PktInfoRaidRankingHeader );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRaidFirstRankingList ) {
                NETStatic.PktGbl.ReqRaidFirstRankingList( pdata.PktMsgData as PktInfoRaidRankingHeader );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRaidRankerDetail ) {
                NETStatic.PktGbl.ReqRaidRankerDetail( pdata.PktMsgData as PktInfoRaidRankerDetailReq );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRaidFirstRankerDetail ) {
                NETStatic.PktGbl.ReqRaidFirstRankerDetail( pdata.PktMsgData as PktInfoRaidRankerDetailReq );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqSetRaidTeam ) {
                NETStatic.PktGbl.ReqSetRaidTeam( pdata.PktMsgData as PktInfoUserRaidTeam );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRaidHPRestore ) {
                NETStatic.PktGbl.ReqRaidHPRestore( pdata.PktMsgData as PktInfoRaidRestoreHPReq );
            }
            else if( pdata.PacketID == GlobalC2S.Common.ReqRaidStageDrop ) {
                NETStatic.PktGbl.ReqRaidStageDrop( pdata.PktMsgData as PktInfoRaidStageDrop );
            }
            else if ( pdata.PacketID == GlobalC2S.Common.ReqCharLvUnexpectedPackageHardOpen)
            {
                NETStatic.PktGbl.ReqCharLvUnexpectedPackageHardOpen((System.UInt64)pdata.PktNumData);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleOpen)
            {
                NETStatic.PktGbl.ReqCircleOpen(pdata.PktMsgData as PktInfoCircleOpenReq);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqSuggestCircleList)
            {
                NETStatic.PktGbl.ReqSuggestCircleList((eLANGUAGE)pdata.PktNumData);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleJoin)
            {
                NETStatic.PktGbl.ReqCircleJoin(pdata.PktMsgData as PktInfoCircleJoinReq);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleJoinCancel)
            {
                NETStatic.PktGbl.ReqCircleJoinCancel((ulong)pdata.PktNumData);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleLobbyInfo)
            {
                NETStatic.PktGbl.ReqCircleLobbyInfo();
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleWithdrawal)
            {
                NETStatic.PktGbl.ReqCircleWithdrawal();
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleDisperse)
            {
                NETStatic.PktGbl.ReqCircleDisperse();
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqGetCircleUserList)
            {
                NETStatic.PktGbl.ReqGetCircleUserList();
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleChangeStateJoinWaitUser)
            {
                NETStatic.PktGbl.ReqCircleChangeStateJoinWaitUser(pdata.PktMsgData as PktInfoChangeStateJoinWaitUser);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleUserKick)
            {
                NETStatic.PktGbl.ReqCircleUserKick((ulong)pdata.PktNumData);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleChangeAuthLevel)
            {
                NETStatic.PktGbl.ReqCircleChangeAuthLevel(pdata.PktMsgData as PktInfoCircleChangeAuthority);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleChangeMark)
            {
                NETStatic.PktGbl.ReqCircleChangeMark(pdata.PktMsgData as PktInfoCircleMarkSet);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleChangeName)
            {
                NETStatic.PktGbl.ReqCircleChangeName(pdata.PktMsgData as PktInfoCircleChangeName);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleChangeComment)
            {
                NETStatic.PktGbl.ReqCircleChangeComment(pdata.PktMsgData as PktInfoStr);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleChangeMainLanguage)
            {
                NETStatic.PktGbl.ReqCircleChangeMainLanguage((eLANGUAGE)pdata.PktNumData);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleChangeSuggestAnotherLangOpt)
            {
                NETStatic.PktGbl.ReqCircleChangeSuggestAnotherLangOpt(pdata.PktNumData == (long)eToggleType.ServerTypeOn);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleAttendance)
            {
                NETStatic.PktGbl.ReqCircleAttendance();
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleBuyMarkItem)
            {
                NETStatic.PktGbl.ReqCircleBuyMarkItem((uint)pdata.PktNumData);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleChatSend)
            {
                NETStatic.PktGbl.ReqCircleChatSend(pdata.PktMsgData as PktInfoCircleChat.Piece);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleGetMarkList)
            {
                NETStatic.PktGbl.ReqCircleGetMarkList();
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleSearch)
            {
                NETStatic.PktGbl.ReqCircleSearch(pdata.PktMsgData as PktInfoCircleSearch);
            }
            else if (pdata.PacketID == GlobalC2S.Common.ReqCircleChatList)
            {
                NETStatic.PktGbl.ReqCircleChatList();
            }
			else if ( pdata.PacketID == GlobalC2S.Common.ReqChangePreferenceNum ) {
				NETStatic.PktGbl.ReqChangePreferenceNum( pdata.PktMsgData as PktInfoUIDList );
			}
		}
        else if (pdata.PacketType == ePacketType.Lobby)
        {
            if (pdata.PacketID == LobbyC2S.Common.ReqMoveToLogin) { }
            else if (pdata.PacketID == LobbyC2S.Common.ReqMoveToBattle) { }
            else if (pdata.PacketID == LobbyC2S.Common.ReqUserMarkList)
            {
                NETStatic.PktLby.ReqUserMarkList();
                Firebase.Analytics.FirebaseAnalytics.LogEvent("UserMarkList");
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqUserSetMark)
            {
                NETStatic.PktLby.ReqUserSetMark((uint)pdata.PktNumData);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("UserSetMark", "Mark", pdata.PktNumData);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqUserSetName)
            {
                NETStatic.PktLby.ReqUserSetName(pdata.PktMsgData as PktInfoStr);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("UserSetName");
            }
            else if( pdata.PacketID == LobbyC2S.Common .ReqUserSetNameColor ) {
                NETStatic.PktLby.ReqUserSetNameColor( (uint)pdata.PktNumData );
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "UserSetNameColor" );
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqUserSetCommentMsg)
            {
                NETStatic.PktLby.ReqUserSetCommentMsg(pdata.PktMsgData as PktInfoStr);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("UserSetCommentMsg");
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqUserPkgShowOff)
            {
                NETStatic.PktLby.ReqUserPkgShowOff();
                Firebase.Analytics.FirebaseAnalytics.LogEvent("UserPkgShowOff");
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqFacilityUpgrade)
            {
                NETStatic.PktLby.ReqFacilityUpgrade((uint)pdata.PktNumData);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("FacilityUpgrade", "FacilityTableID", pdata.PktNumData);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqFacilityOperation)
            {
                NETStatic.PktLby.ReqFacilityOperation(pdata.PktMsgData as PktInfoFacilityOperationReq);

                PktInfoFacilityOperationReq pktInfoFacilityOperationReq = pdata.PktMsgData as PktInfoFacilityOperationReq;

                FacilityData facilitydata = GetFacilityData((int)pktInfoFacilityOperationReq.facilityTID_);
                if (facilitydata != null)
                {
                    if (facilitydata.TableData.EffectType == "FAC_CHAR_EXP" || facilitydata.TableData.EffectType == "FAC_CHAR_SP")
                    {
                        Firebase.Analytics.Parameter[] parameters = {
                        new Firebase.Analytics.Parameter("FacilityTableID", (int)pktInfoFacilityOperationReq.facilityTID_),
                        new Firebase.Analytics.Parameter("CharTableID", GetCharDataTableID((long)pktInfoFacilityOperationReq.operationValue_)),
                    };
                        Firebase.Analytics.FirebaseAnalytics.LogEvent("FacilityOperation", parameters);
                    }
                    else if (facilitydata.TableData.EffectType == "FAC_WEAPON_EXP")
                    {
                        Firebase.Analytics.Parameter[] parameters = {
                        new Firebase.Analytics.Parameter("FacilityTableID", (int)pktInfoFacilityOperationReq.facilityTID_),
                        new Firebase.Analytics.Parameter("WeaponTableID", GetWeaponData((long)pktInfoFacilityOperationReq.operationValue_).TableID),
                    };
                        Firebase.Analytics.FirebaseAnalytics.LogEvent("FacilityOperation", parameters);
                    }
                    else if (facilitydata.TableData.EffectType == "FAC_ITEM_COMBINE")
                    {
                        Firebase.Analytics.Parameter[] parameters = {
                        new Firebase.Analytics.Parameter("FacilityTableID", (int)pktInfoFacilityOperationReq.facilityTID_),
                        new Firebase.Analytics.Parameter("ItemCombineGroupID", (long)pktInfoFacilityOperationReq.operationValue_),
                        new Firebase.Analytics.Parameter("ItemCombineCount", (long)pktInfoFacilityOperationReq.operationCnt_)
                    };
                        Firebase.Analytics.FirebaseAnalytics.LogEvent("FacilityOperation", parameters);
                    }
                }

            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqFacilityOperationConfirm)
            {
                NETStatic.PktLby.ReqFacilityOperationConfirm(pdata.PktMsgData as PktInfoFacilityOperConfirmReq);

                PktInfoFacilityOperConfirmReq pktInfoFacilityOperConfirmReq = pdata.PktMsgData as PktInfoFacilityOperConfirmReq;

                Firebase.Analytics.Parameter[] parameters = {
                        new Firebase.Analytics.Parameter("FacilityTableID", pktInfoFacilityOperConfirmReq.facilityTID_),
                        new Firebase.Analytics.Parameter("FacilityClearOperValFlag", pktInfoFacilityOperConfirmReq.clearOperValFlag_.ToString()),
                        new Firebase.Analytics.Parameter("FacilityImmediatelyItemTID", pktInfoFacilityOperConfirmReq.consumeVal_),
                    };
                Firebase.Analytics.FirebaseAnalytics.LogEvent("FacilityComplete", parameters);

            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqSetMainRoomTheme)
            {
                NETStatic.PktLby.ReqSetMainRoomTheme((System.Byte)pdata.PktNumData);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("SetMainRoomTheme", "RoomThemeIndex", pdata.PktNumData);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqRoomPurchase)
            {
                NETStatic.PktLby.ReqRoomPurchase((System.UInt32)pdata.PktNumData);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("RoomPurchase", "StoreRoomID", pdata.PktNumData);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqRoomThemeSlotDetailInfo)
            {
                NETStatic.PktLby.ReqRoomThemeSlotDetailInfo((System.Byte)pdata.PktNumData);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("RoomThemeSlotDetailInfo", "RoomThemeIndex", pdata.PktNumData);

            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqRoomThemeSlotSave)
            {
                //NETStatic.PktLby.ReqRoomThemeSlotSave(RoomThemeSlotDetail);
                NETStatic.PktLby.ReqRoomThemeSlotSave(pdata.PktMsgData as PktInfoRoomThemeSlotDetail);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("RoomThemeSlotSave");
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqStorePurchase)
            {
                NETStatic.PktLby.ReqStorePurchase(pdata.PktMsgData as PktInfoStorePurchaseReq);

                PktInfoStorePurchaseReq pktInfoStorePurchaseReq = pdata.PktMsgData as PktInfoStorePurchaseReq;

                Firebase.Analytics.FirebaseAnalytics.LogEvent("Purchase", "StoreTableID", pktInfoStorePurchaseReq.storeID_);

            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqStorePurchaseInApp)
            {
                NETStatic.PktLby.ReqStorePurchaseInApp(pdata.PktMsgData as PktInfoStorePurchaseInAppReq);

                PktInfoStorePurchaseInAppReq pktInfoStorePurchaseInAppReq = pdata.PktMsgData as PktInfoStorePurchaseInAppReq;

                Firebase.Analytics.Parameter[] parameters = {
                        new Firebase.Analytics.Parameter("StoreTableID", pktInfoStorePurchaseInAppReq.storeID_),
                        new Firebase.Analytics.Parameter("InappKind", pktInfoStorePurchaseInAppReq.inappKind_.ToString()),
                    };
                Firebase.Analytics.FirebaseAnalytics.LogEvent("PurchaseInApp", parameters);

            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqMailList)
            {
                NETStatic.PktLby.ReqMailList(pdata.PktMsgData as PktInfoMailListReq);
                //Firebase.Analytics.FirebaseAnalytics.LogEvent("MailList");
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqMailTakeProductList)
            {
                NETStatic.PktLby.ReqMailTakeProductList(pdata.PktMsgData as PktInfoMailProductTakeReq);
                //Firebase.Analytics.FirebaseAnalytics.LogEvent("MailTakeProductList");
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqCommunityInfoGet)
            {
                // 커뮤니티 정보 획득 요청
                NETStatic.PktLby.ReqCommunityInfoGet();
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqFriendSuggestList)
            {
                // 추천 친구 목록 요청
                NETStatic.PktLby.ReqFriendSuggestList(pdata.PktMsgData as PktInfoCommuSuggestReq);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqFriendAsk)
            {
                // 친구 신청 요청
                NETStatic.PktLby.ReqFriendAsk(pdata.PktMsgData as PktInfoCommuAskReq);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqFriendAnswer)
            {
                // 친구 신청에 대한 답변 요청
                NETStatic.PktLby.ReqFriendAnswer(pdata.PktMsgData as PktInfoCommuAnswer);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqFriendKick)
            {
                // 친구 제거 요청
                NETStatic.PktLby.ReqFriendKick(pdata.PktMsgData as PktInfoCommuKick);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqFriendPointGive)
            {
                // 친구 포인트 보내기 요청
                NETStatic.PktLby.ReqFriendPointGive();
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqFriendPointTake)
            {
                // 친구 포인트 받기 요청
                NETStatic.PktLby.ReqFriendPointTake(pdata.PktMsgData as PktInfoFriendPointTakeReq);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqFriendAskDel)
            {
                // 친구 신청 취소 요청
                NETStatic.PktLby.ReqFriendAskDel(pdata.PktMsgData as PktInfoCommuAskDel);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqFriendRoomVisitFlag)
            {
                // 친구 프라이빗룸 입장 가능 상태 변경 요청
                NETStatic.PktLby.ReqFriendRoomVisitFlag(pdata.PktMsgData as PktInfoFriendRoomFlag);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqFriendRoomInfoGet)
            {
                // 친구 프라이빗룸 정보 요청
                NETStatic.PktLby.ReqFriendRoomInfoGet(pdata.PktMsgData as PktInfoCommuRoomInfoGet);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqCommunityUserArenaInfoGet)
            {
                // 친구 아레나 정보 요청
                NETStatic.PktLby.ReqCommunityUserArenaInfoGet(pdata.PktMsgData as PktInfoCommuUserArenaInfoReq);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqUserSetCountryAndLangCode)
            {
                // 국가 및 언어 코드 변경 요청
                NETStatic.PktLby.ReqUserSetCountryAndLangCode(pdata.PktMsgData as PktInfoCountryLangCode);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqDispatchOpen)
            {
                // 파견 - 슬롯 열기 요청
                NETStatic.PktLby.ReqDispatchOpen(pdata.PktMsgData as PktInfoTIDList);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqDispatchOperation)
            {
                // 파견 - 시작 요청
                NETStatic.PktLby.ReqDispatchOperation(pdata.PktMsgData as PktInfoDispatchOperReq);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqDispatchOperationConfirm)
            {
                // 파견 - 완료 요청
                NETStatic.PktLby.ReqDispatchOperationConfirm(pdata.PktMsgData as PktInfoDispatchOperConfirmReq);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqDispatchChange)
            {
                // 파견 - (미션)교체 요청
                NETStatic.PktLby.ReqDispatchChange(pdata.PktMsgData as PktInfoTIDList);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqUserLobbyThemeList)
            {
                // 지휘관 로비 테마 목록 요청
                NETStatic.PktLby.ReqUserLobbyThemeList();
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqUserSetLobbyTheme)
            {
                // 지휘관 대표 로비 테마 설정 요청
                NETStatic.PktLby.ReqUserSetLobbyTheme((uint)pdata.PktNumData);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqUserSetMainCardFormation)
            {
                NETStatic.PktLby.ReqUserSetMainCardFormation((uint)pdata.PktNumData);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqInfluenceChoice)
            {
                // 서버 달성(세력) 선택 요청
                NETStatic.PktLby.ReqInfluenceChoice(pdata.PktMsgData as PktInfoInfluenceChoice);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqGetInfluenceInfo)
            {
                //서버 달성(세력) 정보 요청
                NETStatic.PktLby.ReqGetInfluenceInfo();
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqGetInfluenceRankInfo)
            {
                // 서버 달성(세력) 랭킹 정보 요청
                NETStatic.PktLby.ReqGetInfluenceRankInfo(pdata.PktMsgData as PktInfoInfluRankListReq);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqInfluenceTgtRwd)
            {
                // 서버 달성(세력) 보상 요청
                NETStatic.PktLby.ReqInfluenceTgtRwd(pdata.PktMsgData as PktInfoRwdInfluenceTgtReq);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqUserRotationGachaOpen)
            {
                // 로테이션 가챠
                NETStatic.PktLby.ReqUserRotationGachaOpen(pdata.PktMsgData as PktInfoTIDList);
            }
            else if( pdata.PacketID == LobbyC2S.Common.ReqRaidStoreList ) {
                // 레이드 비밀 상점 상품 목록
                NETStatic.PktLby.ReqRaidStoreList( pdata.PktMsgData as PktInfoRaidStoreListReq );
			}
            else if (pdata.PacketID == LobbyC2S.Common.ReqUserCardFormationFavi)
            {
                //서포터 진형 즐겨찾기
                NETStatic.PktLby.ReqUserCardFormationFavi(pdata.PktMsgData as PktInfoTIDList);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqBingoEventReward)
            {
                NETStatic.PktLby.ReqBingoEventReward(pdata.PktMsgData as PktInfoBingoEventRewardReq);
            }
            else if (pdata.PacketID == LobbyC2S.Common.ReqBingoNextOpen)
            {
                NETStatic.PktLby.ReqBingoNextOpen((uint)pdata.PktNumData);
            }
        }
        else if (pdata.PacketType == ePacketType.Product)
        {
            if (pdata.PacketID == ProductC2S.Common.ReqAddItemSlot)
            {
                PktInfoAddSlot pktInfoAddSlot = pdata.PktMsgData as PktInfoAddSlot;
                NETStatic.PktProd.ReqAddItemSlot(pktInfoAddSlot.nowSlotCnt_);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("AddItemSlot");
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqApplyPosCard)
            {
                NETStatic.PktProd.ReqApplyPosCard(pdata.PktMsgData as PktInfoCardApplyPos);

                PktInfoCardApplyPos pktInfoCardApplyPos = pdata.PktMsgData as PktInfoCardApplyPos;

                Firebase.Analytics.Parameter[] parameters = {
                new Firebase.Analytics.Parameter("CharTableID", GetCharDataTableID((long)pktInfoCardApplyPos.posValue_)),
                new Firebase.Analytics.Parameter("CardTableID", GetCardData((long)pktInfoCardApplyPos.cardUID_).TableID),
                new Firebase.Analytics.Parameter("CardSlotNum", pktInfoCardApplyPos.slotNum_),
            };
                Firebase.Analytics.FirebaseAnalytics.LogEvent("ApplyPosCard", parameters);

            }
            else if (pdata.PacketID == ProductC2S.Common.ReqApplyOutPosCard)
            {
                NETStatic.PktProd.ReqApplyOutPosCard((UInt64)pdata.PktNumData);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("ApplyOutPosCard", "CardTableID", GetCardData(pdata.PktNumData).TableID);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqSellCard)
            {
                NETStatic.PktProd.ReqSellCard(pdata.PktMsgData as PktInfoCardSell);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("SellCardList");
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqSetLockCard)
            {
                NETStatic.PktProd.ReqSetLockCard(pdata.PktMsgData as PktInfoCardLock);
                //Firebase.Analytics.FirebaseAnalytics.LogEvent("SetLockCardList");

                PktInfoCardLock pktInfoCardLock = pdata.PktMsgData as PktInfoCardLock;

                for (int i = 0; i < pktInfoCardLock.infos_.Count; i++)
                {
                    Firebase.Analytics.Parameter[] parameters = {
                    new Firebase.Analytics.Parameter("CardTableID", GetCardData((long)pktInfoCardLock.infos_[i].cardUID_).TableID),
                    new Firebase.Analytics.Parameter("CardLocked", pktInfoCardLock.infos_[i].lock_.ToString()),
                    };
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("LockedCard", parameters);
                }

            }
            else if (pdata.PacketID == ProductC2S.Common.ReqLvUpCard)
            {
                NETStatic.PktProd.ReqLvUpCard(pdata.PktMsgData as PktInfoProductComGrowReq);
                PktInfoProductComGrowReq pktInfoProductComGrowReq = pdata.PktMsgData as PktInfoProductComGrowReq;

                Firebase.Analytics.FirebaseAnalytics.LogEvent("LvUpCard", "CardTableID", GetCardData((long)pktInfoProductComGrowReq.targetUID_).TableID);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqSkillLvUpCard)
            {
                NETStatic.PktProd.ReqSkillLvUpCard(pdata.PktMsgData as PktInfoProductComGrowReq);

                PktInfoProductComGrowReq pktInfoProductComGrowReq = pdata.PktMsgData as PktInfoProductComGrowReq;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("SkillLvUpCard", "CardTableID", GetCardData((long)pktInfoProductComGrowReq.targetUID_).TableID);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqWakeCard)
            {
                NETStatic.PktProd.ReqWakeCard(pdata.PktMsgData as PktInfoProductComGrowReq);

                PktInfoProductComGrowReq pktInfoProductComGrowReq = pdata.PktMsgData as PktInfoProductComGrowReq;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("WakeCard", "CardTableID", GetCardData((long)pktInfoProductComGrowReq.targetUID_).TableID);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqFavorLvRewardCard)
            {
                NETStatic.PktProd.ReqFavorLvRewardCard(pdata.PktMsgData as PktInfoBookOnStateReq);

                PktInfoBookOnStateReq pktInfoBookOnStateReq = pdata.PktMsgData as PktInfoBookOnStateReq;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("FavorLvRewardCard", "CardBookTableID", pktInfoBookOnStateReq.tid_);

            }
            else if (pdata.PacketID == ProductC2S.Common.ReqSellItem)
            {
                NETStatic.PktProd.ReqSellItem(pdata.PktMsgData as PktInfoItemSell);
                /*
                Firebase.Analytics.Parameter[] parameters = {
                new Firebase.Analytics.Parameter("ItemTableID", GameInfo.Instance.GetItemData(uid).TableID),
                new Firebase.Analytics.Parameter("SellCount", count),
                };
                Firebase.Analytics.FirebaseAnalytics.LogEvent("SellItemList", parameters);
                */
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqUseItem)
            {
                // !@#~서버 아이템 사용 작업으로 클라에서 수정 및 확인할 부분~#@!
                NETStatic.PktProd.ReqUseItem(pdata.PktMsgData as PktInfoUseItemReq);
                //NETStatic.PktProd.ReqUseItem(pdata.PktMsgData as PktInfoItemCntVec.Piece);

                /*
                Firebase.Analytics.Parameter[] parameters = {
                new Firebase.Analytics.Parameter("ItemTableID", GameInfo.Instance.GetItemData(uid).TableID),
                new Firebase.Analytics.Parameter("UseCount", count),
            };
                Firebase.Analytics.FirebaseAnalytics.LogEvent("UseItem", parameters);
                */
            }
			else if ( pdata.PacketID == ProductC2S.Common.ReqUseItemGoods ) {
				NETStatic.PktProd.ReqUseItemGoods( pdata.PktMsgData as PktInfoItemCnt );
			}
			else if (pdata.PacketID == ProductC2S.Common.ReqSellGem)
            {
                NETStatic.PktProd.ReqSellGem(pdata.PktMsgData as PktInfoGemSell);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("SellGemList");
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqSetLockGem)
            {
                NETStatic.PktProd.ReqSetLockGem(pdata.PktMsgData as PktInfoGemLock);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("SetLockGemList");
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqResetOptGem)
            {
                NETStatic.PktProd.ReqResetOptGem(pdata.PktMsgData as PktInfoGemResetOptReq);

                PktInfoGemResetOptReq pktInfoGemResetOptReq = pdata.PktMsgData as PktInfoGemResetOptReq;
                Firebase.Analytics.Parameter[] parameters = {
                new Firebase.Analytics.Parameter("GemTableID", GetGemData((long)pktInfoGemResetOptReq.gemUID_).TableID),
                new Firebase.Analytics.Parameter("SlotIdx", pktInfoGemResetOptReq.slotIdx_),
            };
                Firebase.Analytics.FirebaseAnalytics.LogEvent("ResetOptGem", parameters);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqResetOptSelectGem)
            {
                NETStatic.PktProd.ReqResetOptSelectGem(pdata.PktMsgData as PktInfoGemResetOptSelect);

                PktInfoGemResetOptSelect pktInfoGemResetOptSelect = pdata.PktMsgData as PktInfoGemResetOptSelect;
                Firebase.Analytics.Parameter[] parameters = {
                new Firebase.Analytics.Parameter("GemTableID", GetGemData((long)pktInfoGemResetOptSelect.gemUID_).TableID),
                new Firebase.Analytics.Parameter("NewFlag", pktInfoGemResetOptSelect.newFlag_.ToString()),
            };
                Firebase.Analytics.FirebaseAnalytics.LogEvent("ResetOptSelectGem", parameters);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqLvUpGem)
            {
                NETStatic.PktProd.ReqLvUpGem(pdata.PktMsgData as PktInfoProductComGrowReq);

                PktInfoProductComGrowReq pktInfoProductComGrowReq = pdata.PktMsgData as PktInfoProductComGrowReq;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("LvUpGem", "GemTableID", GetGemData((long)pktInfoProductComGrowReq.targetUID_).TableID);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqWakeGem)
            {
                NETStatic.PktProd.ReqWakeGem(pdata.PktMsgData as PktInfoProductComGrowReq);

                PktInfoProductComGrowReq pktInfoProductComGrowReq = pdata.PktMsgData as PktInfoProductComGrowReq;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("WakeGem", "GemTableID", GetGemData((long)pktInfoProductComGrowReq.targetUID_).TableID);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqSellWeapon)
            {
                NETStatic.PktProd.ReqSellWeapon(pdata.PktMsgData as PktInfoWeaponSell);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("SellWeaponList");
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqSetLockWeapon)
            {
                NETStatic.PktProd.ReqSetLockWeapon(pdata.PktMsgData as PktInfoWeaponLock);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("SetLockWeaponList");
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqLvUpWeapon)
            {
                NETStatic.PktProd.ReqLvUpWeapon(pdata.PktMsgData as PktInfoProductComGrowReq);

                PktInfoProductComGrowReq pktInfoProductComGrowReq = pdata.PktMsgData as PktInfoProductComGrowReq;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("LvUpWeapon", "WeaponTableID", GetWeaponData((long)pktInfoProductComGrowReq.targetUID_).TableID);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqWakeWeapon)
            {
                NETStatic.PktProd.ReqWakeWeapon(pdata.PktMsgData as PktInfoProductComGrowReq);

                PktInfoProductComGrowReq pktInfoProductComGrowReq = pdata.PktMsgData as PktInfoProductComGrowReq;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("WakeWeapon", "WeaponTableID", GetWeaponData((long)pktInfoProductComGrowReq.targetUID_).TableID);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqSkillLvUpWeapon)
            {
                NETStatic.PktProd.ReqSkillLvUpWeapon(pdata.PktMsgData as PktInfoProductComGrowReq);

                PktInfoProductComGrowReq pktInfoProductComGrowReq = pdata.PktMsgData as PktInfoProductComGrowReq;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("SkillLvUpWeapon", "WeaponTableID", GetWeaponData((long)pktInfoProductComGrowReq.targetUID_).TableID);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqApplyGemInWeapon)
            {
                NETStatic.PktProd.ReqApplyGemInWeapon(pdata.PktMsgData as PktInfoWeaponSlotGem);

                PktInfoWeaponSlotGem pktInfoWeaponSlotGem = pdata.PktMsgData as PktInfoWeaponSlotGem;

                List<Firebase.Analytics.Parameter> par = new List<Firebase.Analytics.Parameter>();

                par.Add(new Firebase.Analytics.Parameter("WeaponTableID", GetWeaponData((long)pktInfoWeaponSlotGem.weaponUID_).TableID));
                for (int i = 0; i < pktInfoWeaponSlotGem.gemUIDs_.Length; i++)
                {
                    GemData gemdata = GetGemData((long)pktInfoWeaponSlotGem.gemUIDs_[i]);
                    if (gemdata != null)
                        par.Add(new Firebase.Analytics.Parameter("GemTableID", gemdata.TableID));
                }

                Firebase.Analytics.FirebaseAnalytics.LogEvent("ApplyGemInWeapon", par.ToArray());
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqApplyPosBadge)
            {
                NETStatic.PktProd.ReqApplyPosBadge(pdata.PktMsgData as PktInfoBadgeApplyPos);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqApplyOutPosBadge)
            {
                NETStatic.PktProd.ReqApplyOutPosBadge(pdata.PktMsgData as PktInfoBadgeComReq);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqUpgradeBadge)
            {
                NETStatic.PktProd.ReqUpgradeBadge(pdata.PktMsgData as PktInfoBadgeComReq);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqResetUpgradeBadge)
            {
                NETStatic.PktProd.ReqResetUpgradeBadge(pdata.PktMsgData as PktInfoBadgeComReq);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqSetLockBadge)
            {
                NETStatic.PktProd.ReqSetLockBadge(pdata.PktMsgData as PktInfoBadgeLock);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqSellBadge)
            {
                NETStatic.PktProd.ReqSellBadge(pdata.PktMsgData as PktInfoBadgeSell);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqAddSlotInWpnDepot)
            {
                NETStatic.PktProd.ReqAddSlotInWpnDepot((byte)pdata.PktNumData);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqApplySlotInWpnDepot)
            {
                NETStatic.PktProd.ReqApplySlotInWpnDepot(pdata.PktMsgData as PktInfoWpnDepotApply);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqChangeTypeCard)
            {
                NETStatic.PktProd.ReqChangeTypeCard(pdata.PktMsgData as PktInfoCardTypeChangeReq);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqEnchantCard)
            {
                NETStatic.PktProd.ReqEnchantCard(pdata.PktMsgData as PktInfoProductComGrowReq);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqEnchantWeapon)
            {
                NETStatic.PktProd.ReqEnchantWeapon(pdata.PktMsgData as PktInfoProductComGrowReq);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqDecomposition)
            {
                NETStatic.PktProd.ReqDecomposition(pdata.PktMsgData as PktInfoDecompositionReq);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqItemExchangeCash)
            {
                NETStatic.PktProd.ReqItemExchangeCash(pdata.PktMsgData as PktInfoItemTIDCnt);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqEvolutionGem)
            {
                NETStatic.PktProd.ReqEvolutionGem(pdata.PktMsgData as PktInfoProductComGrowReq);
            }
            else if (pdata.PacketID == ProductC2S.Common.ReqAnalyzeGem)
            {
                NETStatic.PktProd.ReqAnalyzeGem(pdata.PktMsgData as PktInfoProductComGrowReq);
            }
        }
        else if (pdata.PacketType == ePacketType.Login)
        {
            if (pdata.PacketID == LoginC2S.Common.ReqClientSecurityInfo)
            {
                PktInfoClientSecurityReq pktInfoClientSecurityReq = pdata.PktMsgData as PktInfoClientSecurityReq;
                NETStatic.PktLgn.ReqClientSecurityInfo(pktInfoClientSecurityReq);
            }
            else if (pdata.PacketID == LoginC2S.Common.ReqClientSecurityVerify)
            {
                PktInfoClientSecurityVerifyReq pktInfoClientSecurityVerifyReq = pdata.PktMsgData as PktInfoClientSecurityVerifyReq;
                NETStatic.PktLgn.ReqClientSecurityVerify(pktInfoClientSecurityVerifyReq);
            }

            //로그인서버에서는 무시
            return;
        }
        if (bwait)
        {
            StartTimeOutSend();
        }
        else
        {
            _protocallist.RemoveAt(0);
            WaitPopup.Hide();
        }
    }


    private void RecvProtocolData(PktMsgType pktInfo)
    {
        WaitPopup.Hide();
        StopTimeOut();

        if (_protocallist.Count == 0)
            return;

        var pdata = _protocallist[0];
        _protocallist.RemoveAt(0);

        if (pdata.ReceiveCallBack != null)
            pdata.ReceiveCallBack(0, pktInfo);
    }
   
    private void RecvProtocolData(long pktnum)
    {
        WaitPopup.Hide();
        StopTimeOut();

        if (_protocallist.Count == 0)
            return;

        var pdata = _protocallist[0];
        _protocallist.RemoveAt(0);

        if (pdata.ReceiveCallBack != null)
            pdata.ReceiveCallBack(0, null);

    }

    public void StartTimeOutSend()
    {
        Utility.StopCoroutine(this, ref _timeoutcoroutine);
        _timeoutcoroutine = StartCoroutine(TimeoutCheckSend());
    }
    public void StartTimeOutConnect()
    {
        Utility.StopCoroutine(this, ref _timeoutcoroutine);
        _timeoutcoroutine = StartCoroutine(TimeoutCheckConnect());
    }
    public void StopTimeOut()
    {
        Utility.StopCoroutine(this, ref _timeoutcoroutine);
    }

    private IEnumerator TimeoutCheckSend()
    {
        Debug.Log("CheckTimeout");
        yield return new WaitForSeconds(GameInfo.Instance.GameConfig.NetTimeOutSend);

        WaitPopup.Hide();
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3131), OnMsg_TitleReset, true, true);
        }
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby || AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3131), OnMsg_ToTitle, true, true);//타이틀로
        }
    }

    private IEnumerator TimeoutCheckConnect()
    {
        Debug.Log("CheckTimeout");
        yield return new WaitForSeconds(GameInfo.Instance.GameConfig.NetTimeOutSend);

        WaitPopup.Hide();
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3132), OnMsg_TitleReset, true, true);
        }
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby || AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3132), OnMsg_ToTitle, true, true);//타이틀로
        }
    }

    /// <summary>
    /// TitleSceneOnly
    /// </summary>
    /// <param name="checkTime"></param>
    /// <param name="callback"></param>
    public void StartTimeoutCheckSend(OnTimeOutCallBack timeoutCallback)
    {
        Utility.StopCoroutine(this, ref _timeoutcoroutine);
        _timeoutcoroutine = StartCoroutine(TimeoutCheckSend(timeoutCallback));
    }

    private IEnumerator TimeoutCheckSend(OnTimeOutCallBack timeoutCallback)
    {
        Debug.Log("CheckTimeout Title Scene Only");
        yield return new WaitForSeconds(GameInfo.Instance.GameConfig.NetTimeOutSend);

        if (timeoutCallback != null)
            timeoutCallback();

        WaitPopup.Hide();
    }

}
