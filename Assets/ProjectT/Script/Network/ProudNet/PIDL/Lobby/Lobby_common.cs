﻿ 





// Generated by PIDL compiler.
// Do not modify this file, but modify the source .pidl file.

using System;
namespace LobbyC2S
{
	public class Common
	{
		// Message ID that replies to each RMI method. 
			public const Nettention.Proud.RmiID ReqMoveToLogin = (Nettention.Proud.RmiID)20200+1;
			public const Nettention.Proud.RmiID ReqMoveToBattle = (Nettention.Proud.RmiID)20200+2;
			public const Nettention.Proud.RmiID ReqUserMarkList = (Nettention.Proud.RmiID)20200+3;
			public const Nettention.Proud.RmiID ReqUserSetMark = (Nettention.Proud.RmiID)20200+4;
			public const Nettention.Proud.RmiID ReqUserLobbyThemeList = (Nettention.Proud.RmiID)20200+5;
			public const Nettention.Proud.RmiID ReqUserSetLobbyTheme = (Nettention.Proud.RmiID)20200+6;
			public const Nettention.Proud.RmiID ReqUserSetMainCardFormation = (Nettention.Proud.RmiID)20200+7;
			public const Nettention.Proud.RmiID ReqUserCardFormationFavi = (Nettention.Proud.RmiID)20200+8;
			public const Nettention.Proud.RmiID ReqUserSetName = (Nettention.Proud.RmiID)20200+9;
			public const Nettention.Proud.RmiID ReqUserSetNameColor = (Nettention.Proud.RmiID)20200+10;
			public const Nettention.Proud.RmiID ReqUserSetCommentMsg = (Nettention.Proud.RmiID)20200+11;
			public const Nettention.Proud.RmiID ReqUserSetCountryAndLangCode = (Nettention.Proud.RmiID)20200+12;
			public const Nettention.Proud.RmiID ReqUserPkgShowOff = (Nettention.Proud.RmiID)20200+13;
			public const Nettention.Proud.RmiID ReqFacilityUpgrade = (Nettention.Proud.RmiID)20200+14;
			public const Nettention.Proud.RmiID ReqFacilityOperation = (Nettention.Proud.RmiID)20200+15;
			public const Nettention.Proud.RmiID ReqFacilityOperationConfirm = (Nettention.Proud.RmiID)20200+16;
			public const Nettention.Proud.RmiID ReqDispatchOpen = (Nettention.Proud.RmiID)20200+17;
			public const Nettention.Proud.RmiID ReqDispatchChange = (Nettention.Proud.RmiID)20200+18;
			public const Nettention.Proud.RmiID ReqDispatchOperation = (Nettention.Proud.RmiID)20200+19;
			public const Nettention.Proud.RmiID ReqDispatchOperationConfirm = (Nettention.Proud.RmiID)20200+20;
			public const Nettention.Proud.RmiID ReqSetMainRoomTheme = (Nettention.Proud.RmiID)20200+21;
			public const Nettention.Proud.RmiID ReqRoomPurchase = (Nettention.Proud.RmiID)20200+22;
			public const Nettention.Proud.RmiID ReqRoomThemeSlotDetailInfo = (Nettention.Proud.RmiID)20200+23;
			public const Nettention.Proud.RmiID ReqRoomThemeSlotSave = (Nettention.Proud.RmiID)20200+24;
			public const Nettention.Proud.RmiID ReqStorePurchase = (Nettention.Proud.RmiID)20200+25;
			public const Nettention.Proud.RmiID ReqStorePurchaseInApp = (Nettention.Proud.RmiID)20200+26;
			public const Nettention.Proud.RmiID ReqSteamPurchase = (Nettention.Proud.RmiID)20200+27;
			public const Nettention.Proud.RmiID ReqUserRotationGachaOpen = (Nettention.Proud.RmiID)20200+28;
			public const Nettention.Proud.RmiID ReqRaidStoreList = (Nettention.Proud.RmiID)20200+29;
			public const Nettention.Proud.RmiID ReqMailList = (Nettention.Proud.RmiID)20200+30;
			public const Nettention.Proud.RmiID ReqMailTakeProductList = (Nettention.Proud.RmiID)20200+31;
			public const Nettention.Proud.RmiID ReqCommunityInfoGet = (Nettention.Proud.RmiID)20200+32;
			public const Nettention.Proud.RmiID ReqCommunityUserArenaInfoGet = (Nettention.Proud.RmiID)20200+33;
			public const Nettention.Proud.RmiID ReqCommunityUserUseCallCnt = (Nettention.Proud.RmiID)20200+34;
			public const Nettention.Proud.RmiID ReqFriendSuggestList = (Nettention.Proud.RmiID)20200+35;
			public const Nettention.Proud.RmiID ReqFriendAsk = (Nettention.Proud.RmiID)20200+36;
			public const Nettention.Proud.RmiID ReqFriendAskDel = (Nettention.Proud.RmiID)20200+37;
			public const Nettention.Proud.RmiID ReqFriendAnswer = (Nettention.Proud.RmiID)20200+38;
			public const Nettention.Proud.RmiID ReqFriendKick = (Nettention.Proud.RmiID)20200+39;
			public const Nettention.Proud.RmiID ReqFriendPointGive = (Nettention.Proud.RmiID)20200+40;
			public const Nettention.Proud.RmiID ReqFriendPointTake = (Nettention.Proud.RmiID)20200+41;
			public const Nettention.Proud.RmiID ReqFriendRoomVisitFlag = (Nettention.Proud.RmiID)20200+42;
			public const Nettention.Proud.RmiID ReqFriendRoomInfoGet = (Nettention.Proud.RmiID)20200+43;
			public const Nettention.Proud.RmiID ReqInfluenceChoice = (Nettention.Proud.RmiID)20200+44;
			public const Nettention.Proud.RmiID ReqGetInfluenceInfo = (Nettention.Proud.RmiID)20200+45;
			public const Nettention.Proud.RmiID ReqGetInfluenceRankInfo = (Nettention.Proud.RmiID)20200+46;
			public const Nettention.Proud.RmiID ReqInfluenceTgtRwd = (Nettention.Proud.RmiID)20200+47;
			public const Nettention.Proud.RmiID ReqBingoEventReward = (Nettention.Proud.RmiID)20200+48;
			public const Nettention.Proud.RmiID ReqBingoNextOpen = (Nettention.Proud.RmiID)20200+49;
		// List that has RMI ID.
		public static Nettention.Proud.RmiID[] RmiIDList = new Nettention.Proud.RmiID[] {
			ReqMoveToLogin,
			ReqMoveToBattle,
			ReqUserMarkList,
			ReqUserSetMark,
			ReqUserLobbyThemeList,
			ReqUserSetLobbyTheme,
			ReqUserSetMainCardFormation,
			ReqUserCardFormationFavi,
			ReqUserSetName,
			ReqUserSetNameColor,
			ReqUserSetCommentMsg,
			ReqUserSetCountryAndLangCode,
			ReqUserPkgShowOff,
			ReqFacilityUpgrade,
			ReqFacilityOperation,
			ReqFacilityOperationConfirm,
			ReqDispatchOpen,
			ReqDispatchChange,
			ReqDispatchOperation,
			ReqDispatchOperationConfirm,
			ReqSetMainRoomTheme,
			ReqRoomPurchase,
			ReqRoomThemeSlotDetailInfo,
			ReqRoomThemeSlotSave,
			ReqStorePurchase,
			ReqStorePurchaseInApp,
			ReqSteamPurchase,
			ReqUserRotationGachaOpen,
			ReqRaidStoreList,
			ReqMailList,
			ReqMailTakeProductList,
			ReqCommunityInfoGet,
			ReqCommunityUserArenaInfoGet,
			ReqCommunityUserUseCallCnt,
			ReqFriendSuggestList,
			ReqFriendAsk,
			ReqFriendAskDel,
			ReqFriendAnswer,
			ReqFriendKick,
			ReqFriendPointGive,
			ReqFriendPointTake,
			ReqFriendRoomVisitFlag,
			ReqFriendRoomInfoGet,
			ReqInfluenceChoice,
			ReqGetInfluenceInfo,
			ReqGetInfluenceRankInfo,
			ReqInfluenceTgtRwd,
			ReqBingoEventReward,
			ReqBingoNextOpen,
		};
	}
}
namespace LobbyS2C
{
	public class Common
	{
		// Message ID that replies to each RMI method. 
			public const Nettention.Proud.RmiID AckUserMarkList = (Nettention.Proud.RmiID)20250+1;
			public const Nettention.Proud.RmiID AckUserSetMark = (Nettention.Proud.RmiID)20250+2;
			public const Nettention.Proud.RmiID AckUserLobbyThemeList = (Nettention.Proud.RmiID)20250+3;
			public const Nettention.Proud.RmiID AckUserSetLobbyTheme = (Nettention.Proud.RmiID)20250+4;
			public const Nettention.Proud.RmiID AckUserSetMainCardFormation = (Nettention.Proud.RmiID)20250+5;
			public const Nettention.Proud.RmiID AckUserCardFormationFavi = (Nettention.Proud.RmiID)20250+6;
			public const Nettention.Proud.RmiID AckUserSetName = (Nettention.Proud.RmiID)20250+7;
			public const Nettention.Proud.RmiID AckUserSetNameColor = (Nettention.Proud.RmiID)20250+8;
			public const Nettention.Proud.RmiID AckUserSetCommentMsg = (Nettention.Proud.RmiID)20250+9;
			public const Nettention.Proud.RmiID AckUserSetCountryAndLangCode = (Nettention.Proud.RmiID)20250+10;
			public const Nettention.Proud.RmiID AckUserPkgShowOff = (Nettention.Proud.RmiID)20250+11;
			public const Nettention.Proud.RmiID AckFacilityUpgrade = (Nettention.Proud.RmiID)20250+12;
			public const Nettention.Proud.RmiID AckFacilityOperation = (Nettention.Proud.RmiID)20250+13;
			public const Nettention.Proud.RmiID AckFacilityOperationConfirm = (Nettention.Proud.RmiID)20250+14;
			public const Nettention.Proud.RmiID AckDispatchOpen = (Nettention.Proud.RmiID)20250+15;
			public const Nettention.Proud.RmiID AckDispatchChange = (Nettention.Proud.RmiID)20250+16;
			public const Nettention.Proud.RmiID AckDispatchOperation = (Nettention.Proud.RmiID)20250+17;
			public const Nettention.Proud.RmiID AckDispatchOperationConfirm = (Nettention.Proud.RmiID)20250+18;
			public const Nettention.Proud.RmiID AckSetMainRoomTheme = (Nettention.Proud.RmiID)20250+19;
			public const Nettention.Proud.RmiID AckRoomPurchase = (Nettention.Proud.RmiID)20250+20;
			public const Nettention.Proud.RmiID AckRoomThemeSlotDetailInfo = (Nettention.Proud.RmiID)20250+21;
			public const Nettention.Proud.RmiID AckRoomThemeSlotSave = (Nettention.Proud.RmiID)20250+22;
			public const Nettention.Proud.RmiID AckStorePurchase = (Nettention.Proud.RmiID)20250+23;
			public const Nettention.Proud.RmiID AckStorePurchaseInApp = (Nettention.Proud.RmiID)20250+24;
			public const Nettention.Proud.RmiID AckUserRotationGachaOpen = (Nettention.Proud.RmiID)20250+25;
			public const Nettention.Proud.RmiID AckRaidStoreList = (Nettention.Proud.RmiID)20250+26;
			public const Nettention.Proud.RmiID AckMailList = (Nettention.Proud.RmiID)20250+27;
			public const Nettention.Proud.RmiID AckMailTakeProductList = (Nettention.Proud.RmiID)20250+28;
			public const Nettention.Proud.RmiID AckCommunityInfoGet = (Nettention.Proud.RmiID)20250+29;
			public const Nettention.Proud.RmiID AckCommunityUserArenaInfoGet = (Nettention.Proud.RmiID)20250+30;
			public const Nettention.Proud.RmiID AckCommunityUserUseCallCnt = (Nettention.Proud.RmiID)20250+31;
			public const Nettention.Proud.RmiID AckFriendSuggestList = (Nettention.Proud.RmiID)20250+32;
			public const Nettention.Proud.RmiID AckFriendAsk = (Nettention.Proud.RmiID)20250+33;
			public const Nettention.Proud.RmiID AckFriendAskDel = (Nettention.Proud.RmiID)20250+34;
			public const Nettention.Proud.RmiID AckFriendAnswer = (Nettention.Proud.RmiID)20250+35;
			public const Nettention.Proud.RmiID AckFriendKick = (Nettention.Proud.RmiID)20250+36;
			public const Nettention.Proud.RmiID AckFriendPointGive = (Nettention.Proud.RmiID)20250+37;
			public const Nettention.Proud.RmiID AckFriendPointTake = (Nettention.Proud.RmiID)20250+38;
			public const Nettention.Proud.RmiID AckFriendRoomVisitFlag = (Nettention.Proud.RmiID)20250+39;
			public const Nettention.Proud.RmiID AckFriendRoomInfoGet = (Nettention.Proud.RmiID)20250+40;
			public const Nettention.Proud.RmiID AckInfluenceChoice = (Nettention.Proud.RmiID)20250+41;
			public const Nettention.Proud.RmiID AckGetInfluenceInfo = (Nettention.Proud.RmiID)20250+42;
			public const Nettention.Proud.RmiID AckGetInfluenceRankInfo = (Nettention.Proud.RmiID)20250+43;
			public const Nettention.Proud.RmiID AckInfluenceTgtRwd = (Nettention.Proud.RmiID)20250+44;
			public const Nettention.Proud.RmiID AckBingoNextOpen = (Nettention.Proud.RmiID)20250+45;
			public const Nettention.Proud.RmiID AckBingoEventReward = (Nettention.Proud.RmiID)20250+46;
		// List that has RMI ID.
		public static Nettention.Proud.RmiID[] RmiIDList = new Nettention.Proud.RmiID[] {
			AckUserMarkList,
			AckUserSetMark,
			AckUserLobbyThemeList,
			AckUserSetLobbyTheme,
			AckUserSetMainCardFormation,
			AckUserCardFormationFavi,
			AckUserSetName,
			AckUserSetNameColor,
			AckUserSetCommentMsg,
			AckUserSetCountryAndLangCode,
			AckUserPkgShowOff,
			AckFacilityUpgrade,
			AckFacilityOperation,
			AckFacilityOperationConfirm,
			AckDispatchOpen,
			AckDispatchChange,
			AckDispatchOperation,
			AckDispatchOperationConfirm,
			AckSetMainRoomTheme,
			AckRoomPurchase,
			AckRoomThemeSlotDetailInfo,
			AckRoomThemeSlotSave,
			AckStorePurchase,
			AckStorePurchaseInApp,
			AckUserRotationGachaOpen,
			AckRaidStoreList,
			AckMailList,
			AckMailTakeProductList,
			AckCommunityInfoGet,
			AckCommunityUserArenaInfoGet,
			AckCommunityUserUseCallCnt,
			AckFriendSuggestList,
			AckFriendAsk,
			AckFriendAskDel,
			AckFriendAnswer,
			AckFriendKick,
			AckFriendPointGive,
			AckFriendPointTake,
			AckFriendRoomVisitFlag,
			AckFriendRoomInfoGet,
			AckInfluenceChoice,
			AckGetInfluenceInfo,
			AckGetInfluenceRankInfo,
			AckInfluenceTgtRwd,
			AckBingoNextOpen,
			AckBingoEventReward,
		};
	}
}

				 
