using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NotificationManager : FMonoSingleton<NotificationManager>
{
    public enum eTYPE
    {
        MENU = 0,
        CHAR,
        ITEM,
        GACHA,
        SHOP,
        BATTLE,
        MAIL,
        WMISSION,
        FACILITY,
        FAC_WEAPON_EXP,
        FAC_ITEM_COMBINE,
        FAC_CHAR_EXP,
        FAC_CHAR_SP,       
        PRIVATEROOM,
        ITEM_TAB_WEAPON,
        ITEM_TAB_GEM,
        ITEM_TAB_CARD,
        ITEM_TAB_ITEM,
        SHOP_MPOINT,
        USERMARK,
        ACHIEVEMENT,
        ARENA,
        CAMPAIGN,
        ITEM_TAB_BADGE,
        PASS,               //Lobby Pass Btn
        PASS_TAB_REWARD,
        PASS_TAB_MISSION,
        FRIEND,
        FRIEND_LIST,
        FRIEND_ADD,
        FRIEND_APPLY,
        FRIEND_ALL_CONFIRM,
        FRIEND_ALL_GIVE,
        DISPATCH,
        INFLUENCE,
        FAC_TRADE,
        WELCOME_EVENT,
        COMEBACK_EVENT,
        PUBLIC_EVENT,
        ITEM_TAB_FAVOR,
        SPECIAL_BUY_DAILY_POPUP,
        BOOK,
        FAC_OPERATION_ROOM,
        PASS_TAB_GOLD,
        PASS_TAB_RANK,
        PASS_TAB_STORY,
        EVENT_BOARD,
        NOTICE,
    }

    public List<UISprite> kNoticeSprList;
    public List<UICampaignMarkUnit> kCampaignMarkUnitList;

    private int _newcharbookcount = 0;
    private int _newcostumebookcount = 0;
    private int _newcardbookcount = 0;
    private int _newweaponbookcount = 0;
    private int _newmonsterbookcount = 0;
    private int _newfavorcharbookcount = 0;
    private int _newfavorcardbookcount = 0;
    private bool _bachievementcomplete = false;
    private bool _bfailitycomplete = false;

    private List<NoticeBaseData> _noticebaselist = new List<NoticeBaseData>();
    private List<NoticeFacilityData> _noticefailitylist = new List<NoticeFacilityData>();
    private List<NoticeDispatchData> _noticedispatchlist = new List<NoticeDispatchData>();

    public int NewCharBookCount { get { return _newcharbookcount;  } }
    public int NewCostumeBookCount { get { return _newcostumebookcount; } }
    public int NewCardBookCount { get { return _newcardbookcount; } }
    public int NewWeaponBookCount { get { return _newweaponbookcount; } }
    public int NewMonsterBookCount { get { return _newmonsterbookcount; } }
    public int NewFavorCharBookCount { get { return _newfavorcharbookcount; } }
    public int NewFavorCardBookCount { get { return _newfavorcardbookcount; } }
    public bool IsAchievementComplete { get { return _bachievementcomplete; } }
    public bool IsFailityComplete { get { return _bfailitycomplete; } }
    public List<NoticeBaseData> NoticeBaseList { get { return _noticebaselist; } }
    public List<NoticeFacilityData> NoticeFailityList { get { return _noticefailitylist; } }
    public List<NoticeDispatchData> NoticeDispatchList { get { return _noticedispatchlist; } }
    public int NewBookTotal { get { return _newcharbookcount + _newcostumebookcount + _newcardbookcount + _newweaponbookcount + _newmonsterbookcount + _newfavorcharbookcount + _newfavorcardbookcount; } }

    public void Init()
    {
        
        GameInfo.Instance.CheckAllRedDotCard();
        GameInfo.Instance.CheckAllRedDotWeapon();

        _newcharbookcount = GetNewCharBookCount();
        _newcostumebookcount = GetNewCostumeBookCount();
        _newcardbookcount = GetNewCardBookCount();
        _newweaponbookcount = GetNewWeaponBookCount();
        _newmonsterbookcount = GetNewMonsterBookCount();
        _newfavorcharbookcount = GetNewFavorCharBookCount();
        _newfavorcardbookcount = GetNewFavorCardBookCount();
        _bachievementcomplete = CheckAchievement();
        CheckNotification();
        CheckSituationBoard();
        CheckCampaignMark();
    }

    public void CheckNotification()
    {
        for (int i = 0; i < kNoticeSprList.Count; i++)
            SetActiveNoticeSpr((eTYPE)i, false);

        CheckMENU();
        CheckCHAR();
        CheckITEM();
        CheckGACHA();
        CheckSHOP();
        CheckBATTLE();
        CheckMAIL();
        CheckWMISSION();
        CheckFACILITY();
        CheckITEM_TAB_WEAPON();
        CheckITEM_TAB_GEM();
        CheckITEM_TAB_CARD();
        CheckITEM_TAB_ITEM();
        CheckSHOP_MPOINT();
        CheckUSERMARK();
        CheckACHIEVEMENT();
        CheckARENA();
        CheckCAMPAIGN();
        CheckITEM_TAB_BADGE();
        CheckPASS();
        CheckPASS_TAB_REWARD(eTYPE.PASS_TAB_REWARD,ePassSystemType.Gold);
        CheckPASS_TAB_MISSION();
        CheckFRIEND();
        CheckFRIEND_LIST();
        CheckFRIEND_ADD();
        CheckFRIEND_APPLY();
        CheckDispatch();
        CheckInfluence();
        CheckDailyEvent(eTYPE.WELCOME_EVENT, eEventTarget.WELCOME);
        CheckDailyEvent(eTYPE.COMEBACK_EVENT, eEventTarget.COMEBACK);
        CheckDailyEvent(eTYPE.PUBLIC_EVENT, eEventTarget.PUBLIC);
        CheckNotification(eTYPE.SPECIAL_BUY_DAILY_POPUP);
        CheckBook();
        CheckPASS_TAB_REWARD(eTYPE.PASS_TAB_GOLD, ePassSystemType.Gold);
        CheckPASS_TAB_REWARD(eTYPE.PASS_TAB_RANK, ePassSystemType.Rank);
        CheckPASS_TAB_REWARD(eTYPE.PASS_TAB_STORY, ePassSystemType.Story);
        CheckEventBoard();
    }

    public void CheckNotification(eTYPE type)
    {
        switch (type)
        {
            case eTYPE.MENU: CheckMENU(); break;
            case eTYPE.CHAR: CheckCHAR(); break;
            case eTYPE.ITEM: CheckITEM(); break;
            case eTYPE.GACHA: CheckGACHA(); break;
            case eTYPE.SHOP: CheckSHOP(); break;
            case eTYPE.BATTLE: CheckBATTLE(); break;
            case eTYPE.MAIL: CheckMAIL(); break;
            case eTYPE.WMISSION: CheckWMISSION(); break;
            case eTYPE.FACILITY: CheckFACILITY(); break;
            case eTYPE.FAC_CHAR_EXP: CheckFAC_CHAR_EXP(); break;
            case eTYPE.FAC_CHAR_SP: CheckFAC_CHAR_SP(); break;
            case eTYPE.FAC_WEAPON_EXP: CheckFAC_WEAPON_EXP(); break;
            case eTYPE.FAC_ITEM_COMBINE: CheckFAC_ITEM_COMBINE(); break;
            case eTYPE.PRIVATEROOM: CheckPRIVATEROOM(); break;
            case eTYPE.ITEM_TAB_WEAPON: CheckITEM_TAB_WEAPON(); break;
            case eTYPE.ITEM_TAB_GEM: CheckITEM_TAB_GEM(); break;
            case eTYPE.ITEM_TAB_CARD: CheckITEM_TAB_CARD(); break;
            case eTYPE.ITEM_TAB_ITEM: CheckITEM_TAB_ITEM(); break;
            case eTYPE.SHOP_MPOINT: CheckSHOP_MPOINT(); break;
            case eTYPE.USERMARK: CheckUSERMARK(); break;
            case eTYPE.ACHIEVEMENT: CheckACHIEVEMENT(); break;
            case eTYPE.ARENA: CheckARENA(); break;
            case eTYPE.CAMPAIGN: CheckCAMPAIGN(); break;
            case eTYPE.ITEM_TAB_BADGE: CheckITEM_TAB_BADGE(); break;
            case eTYPE.PASS: CheckPASS(); break;
            case eTYPE.PASS_TAB_REWARD: CheckPASS_TAB_REWARD(eTYPE.PASS_TAB_REWARD, ePassSystemType.Gold); break;
            case eTYPE.PASS_TAB_MISSION: CheckPASS_TAB_MISSION(); break;
            case eTYPE.FRIEND: CheckFRIEND(); break;
            case eTYPE.FRIEND_LIST: CheckFRIEND_LIST(); break;
            case eTYPE.FRIEND_ADD: CheckFRIEND_ADD(); break;
            case eTYPE.FRIEND_APPLY: CheckFRIEND_APPLY(); break;
            case eTYPE.FRIEND_ALL_CONFIRM: CheckFRIEND_ALL_CONFIRM(); break;
            case eTYPE.DISPATCH: CheckDispatch(); break;
            case eTYPE.INFLUENCE: CheckInfluence(); break;
            case eTYPE.WELCOME_EVENT: CheckDailyEvent(eTYPE.WELCOME_EVENT, eEventTarget.WELCOME); break;
            case eTYPE.COMEBACK_EVENT: CheckDailyEvent(eTYPE.COMEBACK_EVENT, eEventTarget.COMEBACK); break;
            case eTYPE.PUBLIC_EVENT: CheckDailyEvent(eTYPE.PUBLIC_EVENT, eEventTarget.PUBLIC); break;
            case eTYPE.ITEM_TAB_FAVOR: CheckITEM_TAB_FAVOR(); break;
            case eTYPE.SPECIAL_BUY_DAILY_POPUP: CheckSPECIAL_BUY_DAILY_POPUP(); break;
            case eTYPE.FAC_OPERATION_ROOM: CheckFAC_OPERATION_ROOM(); break;
            case eTYPE.PASS_TAB_GOLD: CheckPASS_TAB_REWARD(eTYPE.PASS_TAB_GOLD, ePassSystemType.Gold); break;
            case eTYPE.PASS_TAB_STORY: CheckPASS_TAB_REWARD(eTYPE.PASS_TAB_STORY, ePassSystemType.Story); break;
            case eTYPE.PASS_TAB_RANK: CheckPASS_TAB_REWARD(eTYPE.PASS_TAB_RANK, ePassSystemType.Rank); break;
            case eTYPE.EVENT_BOARD: CheckEventBoard(); break;
        }
    }

    private void CheckMENU()
    {
        if (_bachievementcomplete || GameInfo.Instance.IsNewYouTubeLink())
        {
            SetActiveNoticeSpr(eTYPE.MENU, true);
            return;
        }

        SetActiveNoticeSpr(eTYPE.MENU, false);
    }

    private void CheckCHAR()
    {
        ItemData presentItemData = GameInfo.Instance.ItemList.Find(x =>
            x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_PREFERENCE_PRESENT);
        
        for( int i = 0; i < GameInfo.Instance.CharList.Count; i++ )
        {
            var chardata = GameInfo.Instance.CharList[i];
            if (GameSupport.CheckCharData(chardata) )
            {
                SetActiveNoticeSpr(eTYPE.CHAR, true);
                return;
            }
            else
            {
                var weapondata = GameInfo.Instance.GetWeaponData(chardata.EquipWeaponUID);
                if (weapondata != null)
                {
                    if (GameSupport.CheckWeaponData(weapondata))
                    {
                        SetActiveNoticeSpr(eTYPE.CHAR, true);
                        return;
                    }
                }

                for (int j = 0; j < (int)eCOUNT.CARDSLOT; j++)
                {
                    var carddata = GameInfo.Instance.GetCardData(chardata.EquipCard[j]);
                    if (carddata != null)
                    {
                        if (GameSupport.CheckCardData(carddata))
                        {
                            SetActiveNoticeSpr(eTYPE.CHAR, true);
                            return;
                        }
                    }
                }
                
                GameTable.LevelUp.Param levelUpTableData = 
                    GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == chardata.TableData.PreferenceLevelGroup && x.Level == chardata.FavorLevel);
                
                if (presentItemData != null && 0 < chardata.FavorPreCnt && 0 < levelUpTableData.Exp)
                {
                    SetActiveNoticeSpr(eTYPE.CHAR, true);
                    return;
                }
            }

        }
        SetActiveNoticeSpr(eTYPE.CHAR, false);
    }
    private void CheckITEM()
    {
        if (GameInfo.Instance.NewCardList.Count != 0)
        {
            SetActiveNoticeSpr(eTYPE.ITEM, true);
            return;
        }
        if (GameInfo.Instance.NewWeaponList.Count != 0)
        {
            SetActiveNoticeSpr(eTYPE.ITEM, true);
            return;
        }
        if (GameInfo.Instance.NewGemList.Count != 0)
        {
            SetActiveNoticeSpr(eTYPE.ITEM, true);
            return;
        }
        if (GameInfo.Instance.NewItemList.Count != 0)
        {
            SetActiveNoticeSpr(eTYPE.ITEM, true);
            return;
        }

        if(GameInfo.Instance.NewBadgeList.Count != 0)
        {
            SetActiveNoticeSpr(eTYPE.ITEM, true);
            return;
        }

        /*
        //스킬 강화가능시
        var cardlist = GameInfo.Instance.CardList.FindAll(x => x.RedDot == true);
        if( cardlist.Count != 0 )
        {
            SetActiveNoticeSpr(eTYPE.ITEM, true);
            return;
        }
        var weaponlist = GameInfo.Instance.WeaponList.FindAll(x => x.RedDot == true);
        if (weaponlist.Count != 0)
        {
            SetActiveNoticeSpr(eTYPE.ITEM, true);
            return;
        }
        */
        SetActiveNoticeSpr(eTYPE.ITEM, false);
    }
    private void CheckGACHA()
    {
        //무료 가챠
        if(GameSupport.IsStoreSaleApply(GameInfo.Instance.GameConfig.FreeGachaStoreID) || GameSupport.IsStoreSaleApply(GameInfo.Instance.GameConfig.SalePremiumGachaStoreID) ||
            GameSupport.IsStoreSaleApply(GameInfo.Instance.GameConfig.SaleGoldGachaStoreID))
            SetActiveNoticeSpr(eTYPE.GACHA, true);
        else
            SetActiveNoticeSpr(eTYPE.GACHA, false);
    }
    private void CheckSHOP()
    {
        /*
        //무료 골드바
        if (CheckFreeStroe(1))
            SetActiveNoticeSpr(eTYPE.SHOP, true);
        else
            SetActiveNoticeSpr(eTYPE.SHOP, false);
        */
    }
    private void CheckBATTLE()
    {
        //특별모드
    }
    private void CheckMAIL()
    {
        if (GameInfo.Instance.MailTotalCount > 0)
            SetActiveNoticeSpr(eTYPE.MAIL, true);
        else
            SetActiveNoticeSpr(eTYPE.MAIL, false);
    }
    private void CheckWMISSION()
    {
        if (GameSupport.GetPossibleReciveMissionRewardCount() > 0)
            SetActiveNoticeSpr(eTYPE.WMISSION, true);
        else
            SetActiveNoticeSpr(eTYPE.WMISSION, false);
    }
    private void CheckFACILITY()
    {
        CheckFAC_CHAR_EXP();
        CheckFAC_CHAR_SP();
        CheckFAC_WEAPON_EXP();
        CheckFAC_ITEM_COMBINE();
        CheckPRIVATEROOM();
        CheckFAC_TRADE();
        CheckFAC_OPERATION_ROOM();

        if( kNoticeSprList[(int)eTYPE.FAC_CHAR_EXP].gameObject.activeSelf )
        {
            SetActiveNoticeSpr(eTYPE.FACILITY, true);
            return;
        }
        if (kNoticeSprList[(int)eTYPE.FAC_CHAR_SP].gameObject.activeSelf)
        {
            SetActiveNoticeSpr(eTYPE.FACILITY, true);
            return;
        }
        if (kNoticeSprList[(int)eTYPE.FAC_WEAPON_EXP].gameObject.activeSelf)
        {
            SetActiveNoticeSpr(eTYPE.FACILITY, true);
            return;
        }
        if (kNoticeSprList[(int)eTYPE.FAC_ITEM_COMBINE].gameObject.activeSelf)
        {
            SetActiveNoticeSpr(eTYPE.FACILITY, true);
            return;
        }
        if (kNoticeSprList[(int)eTYPE.PRIVATEROOM].gameObject.activeSelf)
        {
            SetActiveNoticeSpr(eTYPE.FACILITY, true);
            return;
        }
        if (kNoticeSprList[(int)eTYPE.FAC_TRADE].gameObject.activeSelf)
        {
            SetActiveNoticeSpr(eTYPE.FACILITY, true);
            return;
        }
        if (kNoticeSprList[(int)eTYPE.FAC_OPERATION_ROOM].gameObject.activeSelf)
        {
            SetActiveNoticeSpr(eTYPE.FACILITY, true);
            return;
        }

        SetActiveNoticeSpr(eTYPE.FACILITY, false);
    }
    private void CheckFAC_CHAR_EXP()
    {
        if( CheckFacility(0) )
            SetActiveNoticeSpr(eTYPE.FAC_CHAR_EXP, true);
        else
            SetActiveNoticeSpr(eTYPE.FAC_CHAR_EXP, false);
    }
    private void CheckFAC_CHAR_SP()
    {
        if( CheckFacility(1))
            SetActiveNoticeSpr(eTYPE.FAC_CHAR_SP, true);
        else
            SetActiveNoticeSpr(eTYPE.FAC_CHAR_SP, false);
    }
    private void CheckFAC_WEAPON_EXP()
    {
        if( CheckFacility(2))
            SetActiveNoticeSpr(eTYPE.FAC_WEAPON_EXP, true);
        else
            SetActiveNoticeSpr(eTYPE.FAC_WEAPON_EXP, false);
    }
    private void CheckFAC_ITEM_COMBINE()
    {
        if( CheckFacility(5))
            SetActiveNoticeSpr(eTYPE.FAC_ITEM_COMBINE, true);
        else
            SetActiveNoticeSpr(eTYPE.FAC_ITEM_COMBINE, false);
    }
    private void CheckFAC_TRADE()
    {
        if (CheckFacility(8))
            SetActiveNoticeSpr(eTYPE.FAC_TRADE, true);
        else
            SetActiveNoticeSpr(eTYPE.FAC_TRADE, false);
    }

    private void CheckFAC_OPERATION_ROOM()
    {
        if (CheckFacility(9))
            SetActiveNoticeSpr(eTYPE.FAC_OPERATION_ROOM, true);
        else
            SetActiveNoticeSpr(eTYPE.FAC_OPERATION_ROOM, false);
    }
    
    private void CheckPRIVATEROOM()
    {
    }
    private void CheckITEM_TAB_WEAPON()
    {
        if (GameInfo.Instance.NewWeaponList.Count != 0)
        {
            SetActiveNoticeSpr(eTYPE.ITEM_TAB_WEAPON, true);
            return;
        }
        /*
        //스킬 강화가능시
        var weaponlist = GameInfo.Instance.WeaponList.FindAll(x => x.RedDot == true);
        if (weaponlist.Count != 0)
        {
            SetActiveNoticeSpr(eTYPE.ITEM_TAB_WEAPON, true);
            return;
        }
        */
        SetActiveNoticeSpr(eTYPE.ITEM_TAB_WEAPON, false);
    }
    private void CheckITEM_TAB_GEM()
    {
        if (GameInfo.Instance.NewGemList.Count != 0)
        {
            SetActiveNoticeSpr(eTYPE.ITEM_TAB_GEM, true);
            return;
        }
        SetActiveNoticeSpr(eTYPE.ITEM_TAB_GEM, false);
    }
    private void CheckITEM_TAB_CARD()
    {
        if (GameInfo.Instance.NewCardList.Count != 0)
        {
            SetActiveNoticeSpr(eTYPE.ITEM_TAB_CARD, true);
            return;
        }
        /*
        //스킬 강화가능시
        var cardlist = GameInfo.Instance.CardList.FindAll(x => x.RedDot == true);
        if (cardlist.Count != 0)
        {
            SetActiveNoticeSpr(eTYPE.ITEM_TAB_CARD, true);
            return;
        }
        */
        SetActiveNoticeSpr(eTYPE.ITEM_TAB_CARD, false);
    }
    private void CheckITEM_TAB_ITEM()
    {
        if (GameInfo.Instance.NewItemList.Count != 0)
        {
            SetActiveNoticeSpr(eTYPE.ITEM_TAB_ITEM, true);
            return;
        }
        SetActiveNoticeSpr(eTYPE.ITEM_TAB_ITEM, false);
    }
    private void CheckITEM_TAB_FAVOR()
    {
        CharData charData = GameInfo.Instance.GetCharDataByTableID(RenderTargetChar.Instance.RenderPlayer.tableId);
        ItemData itemData = GameInfo.Instance.ItemList.Find(x =>
            x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_PREFERENCE_PRESENT);

        GameTable.LevelUp.Param levelUpTableData = 
            GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == charData.TableData.PreferenceLevelGroup && x.Level == charData.FavorLevel);

        if (itemData != null && levelUpTableData != null)
        {
            if (0 < charData.FavorPreCnt && 0 < levelUpTableData.Exp)
            {
                SetActiveNoticeSpr(eTYPE.ITEM_TAB_FAVOR, true);
                return;
            }
        }

        SetActiveNoticeSpr(eTYPE.ITEM_TAB_FAVOR, false);
    }

    private void CheckSPECIAL_BUY_DAILY_POPUP()
    {
        if (GameInfo.Instance.UnexpectedPackageDataDict.ContainsKey((int)eUnexpectedPackageType.FIRST_STAGE))
        {
            foreach (UnexpectedPackageData data in GameInfo.Instance.UnexpectedPackageDataDict[(int) eUnexpectedPackageType.FIRST_STAGE])
            {
                GameTable.UnexpectedPackage.Param unexpectedPackage = GameInfo.Instance.GameTable.FindUnexpectedPackage(data.TableId);
                if (unexpectedPackage == null)
                {
                    break;
                }
                
                GameTable.Store.Param store = GameInfo.Instance.GameTable.FindStore(unexpectedPackage.ConnectStoreID);
                if (store == null)
                {
                    break;
                }
                
                System.TimeSpan timeSpan = GameInfo.Instance.GetNetworkTime() - data.EndTime;
                int pastDay = timeSpan.Days + 1;

                GameTable.Random.Param randomLast =
                    GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == store.ProductIndex).LastOrDefault();

                int lastDay = 0;
                if (randomLast != null)
                {
                    lastDay = randomLast.Value;
                }
                
                for (int i = 0; i < pastDay; i++)
                {
                    if (lastDay <= i)
                    {
                        break;
                    }
                    
                    int flag = 1 << i;
                    if ((data.RewardBitFlag & flag) != flag)
                    {
                        SetActiveNoticeSpr(eTYPE.SPECIAL_BUY_DAILY_POPUP, true);
                        return;
                    }
                }
            }
        }
        
        SetActiveNoticeSpr(eTYPE.SPECIAL_BUY_DAILY_POPUP, false);
    }
    
    private void CheckSHOP_MPOINT()
    {
    }
    private void CheckUSERMARK()
    {
        int total = GameInfo.Instance.NewIconList.Count;
        if (total != 0 || _bachievementcomplete == true)
        {
            SetActiveNoticeSpr(eTYPE.USERMARK, true);
            return;
        }
        SetActiveNoticeSpr(eTYPE.USERMARK, false);
    }
    private void CheckACHIEVEMENT()
    {
        if( _bachievementcomplete )
        {
            SetActiveNoticeSpr(eTYPE.ACHIEVEMENT, true);
            return;
        }
        SetActiveNoticeSpr(eTYPE.ACHIEVEMENT, false);
    }
    private void CheckARENA()
    {
    }
    private void CheckCAMPAIGN()
    {
        for (int i = 0; i < GameInfo.Instance.ServerData.GuerrillaMissionList.Count; i++)
        {
            if (!GameSupport.IsGuerrillaMissionTimeCheck(GameInfo.Instance.ServerData.GuerrillaMissionList[i]))
            {
                continue;
            }

            if (GameSupport.IsGuerrillaMissionComplete(GameInfo.Instance.ServerData.GuerrillaMissionList[i]))
            {
                SetActiveNoticeSpr(eTYPE.CAMPAIGN, true);
                return;
            }
        }

        SetActiveNoticeSpr(eTYPE.CAMPAIGN, false);
    }

    public void SetActiveNoticeSpr(eTYPE type, bool b)
    {
        int index = (int)type;
        if (0 <= index && kNoticeSprList.Count > index)
        {
            kNoticeSprList[index].gameObject.SetActive(b);
        }
    }

    public void CheckITEM_TAB_BADGE()
    {
        if(GameInfo.Instance.NewBadgeList.Count != 0)
        {
            SetActiveNoticeSpr(eTYPE.ITEM_TAB_BADGE, true);
            return;
        }
        SetActiveNoticeSpr(eTYPE.ITEM_TAB_BADGE, false);
    }

    public void CheckPASS()
    {
        bool bRewardFlag = false;
        foreach (PassSetData data in GameInfo.Instance.UserData.PassSetDataList)
        {
            bRewardFlag = GameSupport.GetPassRewardFlag(data.PassID);
            if (bRewardFlag)
            {
                break;
            }
        }

        if (bRewardFlag || GameSupport.GetPassMissionFlag())
        {
            SetActiveNoticeSpr(eTYPE.PASS, true);
        }
        else
        {
            SetActiveNoticeSpr(eTYPE.PASS, false);
        }
    }

    public void CheckPASS_TAB_REWARD(eTYPE tabType, ePassSystemType type)
    {
        SetActiveNoticeSpr(tabType, GameSupport.GetPassRewardFlag((int)type));
    }

    public void CheckPASS_TAB_MISSION()
    {
        SetActiveNoticeSpr(eTYPE.PASS_TAB_MISSION, GameSupport.GetPassMissionFlag());
    }

    public void CheckALLFriend()
    {
        CheckFRIEND();
        CheckFRIEND_LIST();
        CheckFRIEND_ADD();
        CheckFRIEND_APPLY();
    }

    public void CheckFRIEND()
    {
        if ((GameInfo.Instance.CommunityData.FriendList.Count > 0 && (GameInfo.Instance.UserData.NextFrientPointGiveTime < GameSupport.GetCurrentServerTime() || GameSupport.GetFriendPointTakeCheck())) || 
            GameInfo.Instance.CommunityData.FriendAskFromUserList.Count > 0)
        {
            SetActiveNoticeSpr(eTYPE.FRIEND, true);
        }
        else
        {
            SetActiveNoticeSpr(eTYPE.FRIEND, false);
        }
    }

    public void CheckFRIEND_LIST()
    {
        if (GameInfo.Instance.CommunityData.FriendList.Count > 0 &&
            (GameInfo.Instance.UserData.NextFrientPointGiveTime < GameSupport.GetCurrentServerTime() || GameSupport.GetFriendPointTakeCheck()))
        {
            SetActiveNoticeSpr(eTYPE.FRIEND_LIST, true);
        }
        else
        {
            SetActiveNoticeSpr(eTYPE.FRIEND_LIST, false);
        }

        CheckFRIEND_ALL_CONFIRM();
        CheckFRIEND_ALL_GIVE();
    }

    public void CheckFRIEND_ADD()
    {
        SetActiveNoticeSpr(eTYPE.FRIEND_APPLY, false);
    }

    public void CheckFRIEND_APPLY()
    {
        if (GameInfo.Instance.CommunityData.FriendAskFromUserList.Count > 0)
            SetActiveNoticeSpr(eTYPE.FRIEND_APPLY, true);
        else
            SetActiveNoticeSpr(eTYPE.FRIEND_APPLY, false);
    }

    public void CheckFRIEND_ALL_CONFIRM()
    {
        if (GameSupport.GetFriendPointTakeCheck())
            SetActiveNoticeSpr(eTYPE.FRIEND_ALL_CONFIRM, true);
        else
            SetActiveNoticeSpr(eTYPE.FRIEND_ALL_CONFIRM, false);
    }

    public void CheckFRIEND_ALL_GIVE()
    {
        if (GameInfo.Instance.CommunityData.FriendList.Count > 0 && GameInfo.Instance.UserData.NextFrientPointGiveTime < GameSupport.GetCurrentServerTime())
            SetActiveNoticeSpr(eTYPE.FRIEND_ALL_GIVE, true);
        else
            SetActiveNoticeSpr(eTYPE.FRIEND_ALL_GIVE, false);
    }

    public void CheckDispatch()
    {
        bool state = false;
        SetActiveNoticeSpr(eTYPE.DISPATCH, state);

        var Dispatches = GameInfo.Instance.Dispatches;

        if (Dispatches != null && Dispatches.Count > 0)
        {   
            //0.완료 상태 체크        
            for (int i = 0; i < Dispatches.Count; i++)
            {
                var dis = Dispatches[i];
                if (dis == null) continue;
                if (dis.EndTime <= System.DateTime.MinValue || dis.EndTime >= System.DateTime.MaxValue || dis.EndTime.Ticks <= 0)
                    continue;
                var diffTime = dis.EndTime - GameSupport.GetCurrentServerTime();
                if (diffTime.Ticks < 0)
                {
                    state = true;
                    SetActiveNoticeSpr(eTYPE.DISPATCH, state);
                    return;
                }
            }
        }

        //1.Open 가능 상태 체크
        for (int i = 0; i < GameInfo.Instance.GameTable.CardDispatchSlots.Count; i++)
        {
            if (GameInfo.Instance.UserData.Level >= GameInfo.Instance.GameTable.CardDispatchSlots[i].NeedRank)
            {
                if (Dispatches == null || Dispatches.Count == 0)
                    state = true;
                else if (i >= Dispatches.Count)
                    state = true;

                if (state)
                {
                    SetActiveNoticeSpr(eTYPE.DISPATCH, state);
                    return;
                }
            }
        }
    }

    public void CheckInfluence()
    {   
        if(!AppMgr.Instance.configData.m_Network)
        {
            return;
        }

        SetActiveNoticeSpr(eTYPE.INFLUENCE, false);

        int SelectGroupID = (int)GameInfo.Instance.InfluenceMissionData.GroupID;
        GameTable.InfluenceMissionSet.Param InfMissionSetTable = GameInfo.Instance.GameTable.FindInfluenceMissionSet(SelectGroupID);

        //MissionSet 체크
        if (InfMissionSetTable != null)
        {
            List<GameTable.Random.Param> reward = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == InfMissionSetTable.RewardGroupID);

            if (reward != null && reward.Count > 0)
            {
                for (int i = 0; i < reward.Count; i++)
                {
                    if (GameInfo.Instance.InfluenceData.TotalPoint < (ulong)reward[i].Value) break;

                    byte ChoiceBit = (byte)((int)PktInfoMission.Influ.TARGET._ALL_START_ + i);
                    if (!GameSupport.IsComplateMissionRecive(GameInfo.Instance.InfluenceMissionData.TgtRwdFlag, ChoiceBit))
                    {
                        SetActiveNoticeSpr(eTYPE.INFLUENCE, true);
                        return;
                    }
                }
            }

            reward.Clear();
            reward = null;

            // InfluenceInfo 체크
            int InfluenceInfoNo = (int)GameInfo.Instance.InfluenceMissionData.InfluID;
            GameTable.InfluenceInfo.Param InfluenceInfoTable = GameInfo.Instance.GameTable.FindInfluenceInfo(x => x.GroupID == SelectGroupID && x.No == InfluenceInfoNo);
            if (InfluenceInfoTable != null)
            {
                reward = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == InfluenceInfoTable.RewardGroupID);

                if (reward != null && reward.Count > 0)
                {   
                    ItemData item = GameInfo.Instance.ItemList.Find(x => x.TableID == GameSupport.GetCurrentInfluenceEventItemID());
                    int InfluenceInfoItemCount = 0;
                    if (item != null) InfluenceInfoItemCount = item.Count;

                    for (int i = 0; i < reward.Count; i++)
                    {
                        if (InfluenceInfoItemCount < reward[i].Value) break;

                        byte ChoiceBit = (byte)((int)PktInfoMission.Influ.TARGET._SINGLE_START_ + i);
                        if (!GameSupport.IsComplateMissionRecive(GameInfo.Instance.InfluenceMissionData.TgtRwdFlag, ChoiceBit))
                        {
                            SetActiveNoticeSpr(eTYPE.INFLUENCE, true);
                            return;
                        }
                    }
                }
            }
        }

        if (GameSupport.IsComplateMissionRecive(GameInfo.Instance.InfluenceMissionData.TgtRwdFlag, (byte)PktInfoMission.Influ.TARGET._RANK_START_))
        {
            return;
        }

        // InfluenceMission 체크
        List<GameTable.InfluenceMission.Param> InfluenceMissionTables = GameInfo.Instance.GameTable.FindAllInfluenceMission(x => x.GroupID == SelectGroupID);
        if (InfluenceMissionTables != null && InfluenceMissionTables.Count > 0)
        {
            InfluenceMissionTables.Sort((l, r) =>
            {
                if (l.No > r.No) return 1;
                else if (l.No < r.No) return -1;
                return 0;
            });

            for (int i = 0; i < InfluenceMissionTables.Count; i++)
            {  
                if (GameInfo.Instance.InfluenceMissionData.Val[i] > 0) continue;

                byte ChoiceBit = (byte)((int)PktInfoMission.Influ.ENUM._NO_START_ + i);

                if(!GameSupport.IsComplateMissionRecive(GameInfo.Instance.InfluenceMissionData.RwdFlag, ChoiceBit))
                {
                    SetActiveNoticeSpr(eTYPE.INFLUENCE, true);
                    return;
                }
            }
        }
    }

    private void CheckDailyEvent(eTYPE eType, eEventTarget eTarget)
    {
        //SetActiveNoticeSpr(eType, GameSupport.IsValidDailyEventState(eTarget));        
        SetActiveNoticeSpr(eType, GameSupport.IsValidDailyEventState(eTarget));
    }

    private void CheckBook()
    {
        SetActiveNoticeSpr(eTYPE.BOOK, NewBookTotal != 0);
    }

    private void CheckEventBoard()
    {
        bool isNoticeActive = false;

        foreach (GameClientTable.EventPage.Param eventPage in GameInfo.Instance.GameClientTable.EventPages)
        {
            switch ((eLobbyEventType)eventPage.Type)
            {
                case eLobbyEventType.Bingo:
                    {
                        isNoticeActive = IsBingoEventRewardComplete(eventPage);
                    } break;
                case eLobbyEventType.Achieve:
                    {
                        isNoticeActive = IsAchieveEventRewardComplete(eventPage);
                    } break;
            }

            if (isNoticeActive)
            {
                break;
            }
        }

        SetActiveNoticeSpr(eTYPE.EVENT_BOARD, isNoticeActive);
    }

    private bool IsBingoEventRewardComplete(GameClientTable.EventPage.Param eventPage)
    {
        GameTable.BingoEvent.Param bingoEvent = LobbyUIManager.Instance.GetBingoEvent(eventPage);
        if (bingoEvent == null)
        {
            return false;
        }

        int bingoNo = 1;
        int bingoRwdFlag = 0;
        if (GameInfo.Instance.EventDataDict.ContainsKey(eventPage.Type))
        {
            EventData eventData = GameInfo.Instance.EventDataDict[eventPage.Type].Find(x => x.GroupID == bingoEvent.ID);
            if (eventData != null)
            {
                bingoNo = eventData.No;
                bingoRwdFlag = eventData.RwdFlag;
            }
        }

        GameTable.BingoEventData.Param bingoEventData = GameInfo.Instance.GameTable.FindBingoEventData(x => x.GroupID == bingoEvent.ID && x.No == bingoNo);
        if (bingoEventData == null)
        {
            bingoEventData = GameInfo.Instance.GameTable.FindAllBingoEventData(x => x.GroupID == bingoEvent.ID).LastOrDefault();
            if (bingoEventData == null) {
                return false;
            }
        }

        bool[] bingoClearArray = new bool[16];
        int maxNumber = bingoClearArray.Length + 1;
        for (int n = 1; n < maxNumber; n++)
        {
            int itemId = (int)bingoEventData.GetType().GetField($"ItemID{n}").GetValue(bingoEventData);
            int itemCount = (int)bingoEventData.GetType().GetField($"ItemCount{n}").GetValue(bingoEventData);

            ItemData itemData = GameInfo.Instance.ItemList.Find(x => x.TableID == itemId);
            if (itemData != null)
            {
                bingoClearArray[n - 1] = itemCount <= itemData.Count;
            }
        }

        int bingoRewardCount = 0;
        int bingoClearLineCount = 0;
        maxNumber = 11;
        for (int n = 1; n < maxNumber; n++)
        {
            if ((bingoRwdFlag & (1 << n)) != 0)
            {
                ++bingoRewardCount;
            }

            string clearStr = (string)bingoEventData.GetType().GetField($"Clear{n}").GetValue(bingoEventData);
            string[] splits = clearStr.Split(',');
            bool isClearLine = true;

            for (int i = 0; i < splits.Length; i++)
            {
                if (int.TryParse(splits[i], out int result))
                {
                    int index = result - 1;
                    if (index < 0 || bingoClearArray.Length <= index)
                    {
                        isClearLine = false;
                        break;
                    }

                    if (bingoClearArray[index] == false)
                    {
                        isClearLine = false;
                        break;
                    }
                }
            }

            if (isClearLine)
            {
                ++bingoClearLineCount;
            }
        }

        bool isResult = false;

        if (10 <= bingoClearLineCount)
        {
            isResult = (bingoRwdFlag & (1 << 0)) == 0;
        }
        else
        {
            isResult = bingoRewardCount < bingoClearLineCount;
        }

        return isResult;
    }

    private bool IsAchieveEventRewardComplete(GameClientTable.EventPage.Param eventPage)
    {
        GameTable.AchieveEvent.Param achieveEvent = LobbyUIManager.Instance.GetAchieveEvent(eventPage);
        if (achieveEvent == null)
        {
            return false;
        }

        bool isResult = false;

        List<AchieveEventData> achieveEventList = GameInfo.Instance.AchieveEventList.FindAll(x => x.GroupId == achieveEvent.ID);
        foreach (AchieveEventData achieveEventData in achieveEventList)
        {
            if (GameSupport.IsAchieveEventComplete(achieveEventData) == eAchieveEventType.Reward)
            {
                isResult = true;
                break;
            }
        }

        return isResult;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  New 관련
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public int GetNewCharBookCount()
    {
        int total = 0;
        for (int i = 0; i < GameInfo.Instance.CharList.Count; i++)
            if (FSaveData.Instance.HasKey("NCharBook_" + GameInfo.Instance.CharList[i].TableID.ToString()))
                total += 1;
        

        return total;
    }

    public int GetNewCostumeBookCount()
    {
        int total = 0;

        for (int i = 0; i < GameInfo.Instance.CostumeList.Count; i++)
            if (FSaveData.Instance.HasKey("NCostumeBook_" + GameInfo.Instance.CostumeList[i]))
                total += 1;

        return total;
    }

    public int GetNewCardBookCount()
    {
        int total = 0;
        for (int i = 0; i < GameInfo.Instance.CardBookList.Count; i++)
            if (!GameInfo.Instance.CardBookList[i].IsOnFlag(eBookStateFlag.NEW_CHK))
                total += 1;
        return total;
    }

    public int GetNewWeaponBookCount()
    {
        int total = 0;
        for (int i = 0; i < GameInfo.Instance.WeaponBookList.Count; i++)
            if (!GameInfo.Instance.WeaponBookList[i].IsOnFlag(eBookStateFlag.NEW_CHK))
                total += 1;
        return total;
    }

    public int GetNewMonsterBookCount()
    {
        int total = 0;
        for (int i = 0; i < GameInfo.Instance.MonsterBookList.Count; i++)
            if (!GameInfo.Instance.MonsterBookList[i].IsOnFlag(eBookStateFlag.NEW_CHK))
                total += 1;
        return total;
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  호감도 관련
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public int GetNewFavorCharBookCount()
    {
        int total = 0;
        for (int i = 0; i < GameInfo.Instance.CharList.Count; i++)
        {
            if (GameSupport.IsCharOpenTerms_Favor(GameInfo.Instance.CharList[i]))
            {
                if (PlayerPrefs.HasKey("NCharBook_Favor_" + GameInfo.Instance.CharList[i].TableID.ToString()))
                    total += 1;
            }
        }
        return total;
    }
    public int GetNewFavorCardBookCount()
    {
        int total = 0;
        for (int i = 0; i < GameInfo.Instance.CardBookList.Count; i++)
        {
            bool bfavor = GameInfo.Instance.CardBookList[i].IsOnFlag(eBookStateFlag.MAX_FAVOR_LV);
            if (bfavor)
            {
                if (PlayerPrefs.HasKey("NCardBook_Favor_" + GameInfo.Instance.CardBookList[i].TableID.ToString()))
                    total += 1;
            }
        }
        return total;
    }


    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  아이템 관련
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private bool CheckFacility(int index)
    {
        if (GameInfo.Instance.FacilityList.Count == 0)
            return false;
        var data = GameInfo.Instance.FacilityList[index];
        if (data == null)
            return false;

        var facilitylist = GameInfo.Instance.FacilityList.FindAll(x => x.TableData.ParentsID == data.TableData.ParentsID);
        if (facilitylist == null)
            return false;

        if (facilitylist.Count == 0)
            return false;

        if( data.Level == 0 )
        {
            //활성화 가능
            if (GameSupport.IsFacilityActive(facilitylist[0]))
                return true;
        }
        else
        {
            //레벨업 가능
            if (GameSupport.IsFacilityLevelUp(facilitylist[0]))
                return true;

            //완료
            for (int i = 0; i < facilitylist.Count; i++)
            {
                if (GameSupport.IsFacilityComplete(facilitylist[i]))
                    return true;
            }
        }

       
        return false;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  업적 관련
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public bool CheckAchievement()
    {
        for ( int i = 0; i < GameInfo.Instance.AchieveList.Count; i++ )
        {
            if( GameSupport.IsAchievementComplete(GameInfo.Instance.AchieveList[i]) )
                return true;
        }
        return false;
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  상점 관련
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private bool CheckFreeStroe(int paneltype)
    {
        return GameSupport.IsStoreSaleApply(GameInfo.Instance.GameConfig.FreeGachaStoreID);
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  상점 관련
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void CheckSituationBoard()
    {
        _noticebaselist.Clear();
        _noticefailitylist.Clear();
        _noticedispatchlist.Clear();

        _bfailitycomplete = false;
        //공적 달성
        if (_bachievementcomplete)
        {
            for (int i = 0; i < GameInfo.Instance.AchieveList.Count; i++)
            {
                var achievedata = GameInfo.Instance.AchieveList[i];
                if (GameSupport.IsAchievementComplete(achievedata))
                {
                    _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.USER_ACHIEVEMENT, string.Format(FLocalizeString.Instance.GetText(achievedata.TableData.Name), achievedata.GroupOrder), achievedata.GroupID, achievedata.GroupOrder));
                }
            }
        }

        //캐릭터
        for (int i = 0; i < GameInfo.Instance.CharList.Count; i++)
        {
            CharData chardata = GameInfo.Instance.CharList[i];
            //캐릭터호감도
            if (GameSupport.IsCharOpenTerms_Favor(chardata))
            {
                if (PlayerPrefs.HasKey("NCharBook_Favor_" + chardata.TableID.ToString()))
                {
                    _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.CHAR_FAVOR, FLocalizeString.Instance.GetText(chardata.TableData.Name), chardata.TableID, -1));
                }
            }

            //캐릭터진급
            if (GameSupport.IsCharGradeUp(chardata))
            {
                NoticeBaseData.eTYPE type = NoticeBaseData.eTYPE.CHAR_GRADEUP;
                if(chardata.Grade >= GameInfo.Instance.GameConfig.CharStartAwakenGrade)
                {
                    type = NoticeBaseData.eTYPE.CHAR_AWAKEN;
                }

                _noticebaselist.Add(new NoticeBaseData(type, FLocalizeString.Instance.GetText(chardata.TableData.Name), chardata.TableID, -1));
                _noticebaselist[_noticebaselist.Count - 1].uuid = chardata.CUID;
            }

            //캐릭터기술 개방
            if (GameSupport.IsCharSkillUp(chardata))
            {
                var seleteskilllist = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.CharacterID == chardata.TableID && (x.Type == (int)eCHARSKILLPASSIVETYPE.SELECT_NORMAL));
                for (int j = 0; j < seleteskilllist.Count; j++)
                {
                    var passviedata = chardata.PassvieList.Find(x => x.SkillID == seleteskilllist[j].ID);
                    if (passviedata == null)
                    {
                        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == seleteskilllist[j].ItemReqListID);
                        if (reqdata != null)
                        {
                            if (chardata.Level >= reqdata.LimitLevel)
                            {   // 신규 스킬 개방의 경우 캐릭터명: 스킬명 형태로 출력하기 때문에 글자수가 길어 풀네임이 아닌 이름만 출력 (Name + 10000)
                                _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.CHAR_SKILLOPEN, FLocalizeString.Instance.GetText(chardata.TableData.Name + 10000), FLocalizeString.Instance.GetText(seleteskilllist[j].Name), chardata.TableID, -1));
                                _noticebaselist[_noticebaselist.Count - 1].uuid = chardata.CUID;
                            }
                        }
                    }
                }
            }
            //캐릭터 스킬 슬롯 개방
            for (int k = 1; k < (int)eCOUNT.SKILLSLOT; k++)
            {
                if (chardata.Level >= GameInfo.Instance.GameConfig.CharSkillSlotLimitLevel[k]) // 레벨제한
                {
                    string str = string.Format("UserData_SlotEffect_{0}_{1}", chardata.TableData.ID.ToString(), k);
                    if (PlayerPrefs.HasKey(str))
                    {
                        _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.CHAR_SkLLSLOTOPEN, FLocalizeString.Instance.GetText(chardata.TableData.Name), chardata.TableID, -1));
                        _noticebaselist[_noticebaselist.Count - 1].uuid = chardata.CUID;
                    }
                }
            }        
            
            //캐릭터 서포터 슬롯 개방


        }

        //장착
        for (int i = 0; i < GameInfo.Instance.CharList.Count; i++)
        {
            var chardata = GameInfo.Instance.CharList[i];
            for (int j = 0; j < (int)eCOUNT.CARDSLOT; j++)
            {
                var carddata = GameInfo.Instance.GetCardData(chardata.EquipCard[j]);
                if (carddata != null)
                {
                    var cardbookdata = GameInfo.Instance.GetCardBookData(carddata.TableID);
                    if (cardbookdata != null)
                    {
                        bool bfavor = cardbookdata.IsOnFlag(eBookStateFlag.MAX_FAVOR_LV);
                        if (bfavor)
                        {
                            if (PlayerPrefs.HasKey("NCardBook_Favor_" + cardbookdata.TableID.ToString()))
                            {
                                _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.CARD_FAVOR, FLocalizeString.Instance.GetText(carddata.TableData.Name), (int)eBookGroup.Supporter, carddata.TableID));
                                _noticebaselist[_noticebaselist.Count - 1].uuid = carddata.CardUID;
                            }
                        }
                    }

                    if (GameSupport.IsCardWakeUp(carddata))
                    {
                        _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.CARD_WAKE, FLocalizeString.Instance.GetText(carddata.TableData.Name), carddata.TableID, -1));
                        _noticebaselist[_noticebaselist.Count - 1].uuid = carddata.CardUID;
                    }
                    if (GameSupport.IsCardSkillUp(carddata))
                    {
                        _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.CARD_SKILLLEVELUP, FLocalizeString.Instance.GetText(carddata.TableData.Name), carddata.TableID, -1));
                        _noticebaselist[_noticebaselist.Count - 1].uuid = carddata.CardUID;
                    }
                }
            }
            var weapondata = GameInfo.Instance.GetWeaponData(chardata.EquipWeaponUID);
            if (weapondata != null)
            {
                if (GameSupport.IsWeaponWakeUp(weapondata))
                {
                    _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.WEAPON_WAKE, FLocalizeString.Instance.GetText(weapondata.TableData.Name), weapondata.TableID, -1));
                    _noticebaselist[_noticebaselist.Count - 1].uuid = weapondata.WeaponUID;
                }
                if (GameSupport.IsWeaponSkillUp(weapondata))
                {
                    _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.WEAPON_SKILLLEVELUP, FLocalizeString.Instance.GetText(weapondata.TableData.Name), weapondata.TableID, -1));
                    _noticebaselist[_noticebaselist.Count - 1].uuid = weapondata.WeaponUID;
                }
            }
        }

        //시설 개방
        for (int i = 0; i < GameInfo.Instance.FacilityList.Count; i++)
        {
            var data = GameInfo.Instance.FacilityList[i];
            var facilitylist = GameInfo.Instance.FacilityList.FindAll(x => x.TableData.ParentsID == data.TableData.ParentsID);
            if (facilitylist == null)
                continue;
            if (facilitylist.Count == 0)
                continue;

            if (data.TableID == data.TableData.ParentsID)
            {
                //활성화 가능
                if (GameSupport.IsFacilityActive(facilitylist[0]))
                {
                    _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.FACILITY_OPEN, FLocalizeString.Instance.GetText(facilitylist[0].TableData.Name), facilitylist[0].TableID - 1, -1));
                }
            }
        }

        //서포터 파견 슬롯 개방 가능
        for (int i = 0; i < GameInfo.Instance.GameTable.CardDispatchSlots.Count; i++)
        {
            if (i < GameInfo.Instance.Dispatches.Count) continue;

            if(GameInfo.Instance.UserData.Level >= GameInfo.Instance.GameTable.CardDispatchSlots[i].NeedRank)
            {
                string msg = string.Format("{0}", GameInfo.Instance.GameTable.CardDispatchSlots[i].Index);
                _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.DISPATCH_OPEN, msg, -1, -1));
            }
        }

        /*
        //신규 도감 오픈
        if (_newcardbookcount != 0)
        {
            for (int i = 0; i < GameInfo.Instance.CardBookList.Count; i++)
            {
                if (!GameInfo.Instance.CardBookList[i].IsOnFlag(eBookStateFlag.NEW_CHK))
                {
                    var tabledata = GameInfo.Instance.GameTable.FindCard(x => x.ID == GameInfo.Instance.CardBookList[i].TableID);
                    if (tabledata != null)
                        _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.BOOK_OPEN, FLocalizeString.Instance.GetText(tabledata.Name), (int)eBookGroup.Supporter, tabledata.ID));
                }
            }

        }
        if (_newweaponbookcount != 0)
        {
            for (int i = 0; i < GameInfo.Instance.WeaponBookList.Count; i++)
            {
                if (!GameInfo.Instance.WeaponBookList[i].IsOnFlag(eBookStateFlag.NEW_CHK))
                {
                    var tabledata = GameInfo.Instance.GameTable.FindWeapon(x => x.ID == GameInfo.Instance.WeaponBookList[i].TableID);
                    if (tabledata != null)
                        _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.BOOK_OPEN, FLocalizeString.Instance.GetText(tabledata.Name), (int)eBookGroup.Weapon, tabledata.ID));
                }
            }
        }
        if (_newmonsterbookcount != 0)
        {
            for (int i = 0; i < GameInfo.Instance.MonsterBookList.Count; i++)
            {
                if (!GameInfo.Instance.MonsterBookList[i].IsOnFlag(eBookStateFlag.NEW_CHK))
                {
                    var tabledata = GameInfo.Instance.GameClientTable.FindBookMonster(x => x.ID == GameInfo.Instance.MonsterBookList[i].TableID);
                    if (tabledata != null)
                        _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.BOOK_OPEN, FLocalizeString.Instance.GetText(tabledata.Name), (int)eBookGroup.Monster, tabledata.ID));
                }
            }
        }
        */

        //주간 미션 완료
        var weekquesttabledata = GameInfo.Instance.GameTable.FindWeeklyMissionSet((int)GameInfo.Instance.WeekMissionData.fWeekMissionSetID);
        List<eMISSIONTYPE> weekquesttype = new List<eMISSIONTYPE>();
        List<int> weekquestcntmax = new List<int>();
        weekquesttype.Add((eMISSIONTYPE)GameSupport.CompareMissionString(weekquesttabledata.WMCon0));
        weekquesttype.Add((eMISSIONTYPE)GameSupport.CompareMissionString(weekquesttabledata.WMCon1));
        weekquesttype.Add((eMISSIONTYPE)GameSupport.CompareMissionString(weekquesttabledata.WMCon2));
        weekquesttype.Add((eMISSIONTYPE)GameSupport.CompareMissionString(weekquesttabledata.WMCon3));
        weekquesttype.Add((eMISSIONTYPE)GameSupport.CompareMissionString(weekquesttabledata.WMCon4));
        weekquesttype.Add((eMISSIONTYPE)GameSupport.CompareMissionString(weekquesttabledata.WMCon5));
        weekquesttype.Add((eMISSIONTYPE)GameSupport.CompareMissionString(weekquesttabledata.WMCon6));
        weekquestcntmax.Add(weekquesttabledata.WMCnt0);
        weekquestcntmax.Add(weekquesttabledata.WMCnt1);
        weekquestcntmax.Add(weekquesttabledata.WMCnt2);
        weekquestcntmax.Add(weekquesttabledata.WMCnt3);
        weekquestcntmax.Add(weekquesttabledata.WMCnt4);
        weekquestcntmax.Add(weekquesttabledata.WMCnt5);
        weekquestcntmax.Add(weekquesttabledata.WMCnt6);
        for (int i = 0; i < (int)eCOUNT.WEEKMISSIONCOUNT; i++)
        {
            //  수령 가능 갯수
            if (GameSupport.IsComplateMissionRecive(GameInfo.Instance.WeekMissionData.fMissionRewardFlag, i) != true && GameInfo.Instance.WeekMissionData.fMissionRemainCntSlot[i] == 0)
            {
                string text = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.WEEKLY_MISSION_TEXT_START + (int)weekquesttype[i]), weekquestcntmax[i]);
                _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.WMISSION_COMPLATE, text, -1, -1));
            }

        }
        
        

        if (_newcharbookcount != 0)
            _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.BOOK_OPEN_COUNT_CHAR, string.Format("{0:#,##0}", _newcharbookcount), 0, 0));
        if (_newcardbookcount != 0)
            _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.BOOK_OPEN_COUNT_CARD, string.Format("{0:#,##0}", _newcardbookcount), 0, 0));
        if (_newweaponbookcount != 0)
            _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.BOOK_OPEN_COUNT_WEAPON, string.Format("{0:#,##0}", _newweaponbookcount), 0, 0));
        if (_newmonsterbookcount != 0)
            _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.BOOK_OPEN_COUNT_MONSTER, string.Format("{0:#,##0}", _newmonsterbookcount), 0, 0));
        if(_newcostumebookcount != 0)
            _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.BOOK_OPEN_COUNT_COSTUME, string.Format("{0:#,##0}", _newcostumebookcount), 0, 0));

        //우편
        if (GameInfo.Instance.MailTotalCount > 0)
        {
            _noticebaselist.Add(new NoticeBaseData(NoticeBaseData.eTYPE.MAIL, string.Format("{0:#,##0}", GameInfo.Instance.MailTotalCount), -1, -1));
        }

        //시설 상황
        for (int i = 0; i < GameInfo.Instance.FacilityList.Count; i++)
        {
            var facilitydata = GameInfo.Instance.FacilityList[i];
            if (facilitydata.Stats != (int)eFACILITYSTATS.WAIT)
            {
                var diffTime = facilitydata.RemainTime - GameSupport.GetCurrentServerTime();
                long remaintime = diffTime.Ticks;
                _noticefailitylist.Add(new NoticeFacilityData(facilitydata.TableID, remaintime));

                if (diffTime.Ticks <= 0)
                {
                    _bfailitycomplete = true;
                }
            }
        }

        if( _noticefailitylist.Count != 0 )
        {
            _noticefailitylist.Sort(NoticeFacilityData.CompareFuncTick);
        }

        //서포터 파견 상황
        for (int i = 0; i < GameInfo.Instance.Dispatches.Count; i++)
        {
            var data = GameInfo.Instance.Dispatches[i];
            if (data.EndTime <= System.DateTime.MinValue || data.EndTime >= System.DateTime.MaxValue || data.EndTime.Ticks <= 0)
                continue;

            var diffTime = data.EndTime - GameSupport.GetCurrentServerTime();
            long remaintime = diffTime.Ticks;
            _noticedispatchlist.Add(new NoticeDispatchData(data.TableID, data.MissionID, remaintime));
        }

        if (_noticedispatchlist.Count != 0)
        {
            _noticedispatchlist.Sort(NoticeDispatchData.CompareFuncTick);
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  캠페인 마크
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void CheckCampaignMark()
    {
        for (int i = 0; i < kCampaignMarkUnitList.Count; i++)
            kCampaignMarkUnitList[i].gameObject.SetActive(false);

        for (int i = 0; i < GameInfo.Instance.ServerData.GuerrillaCampList.Count; i++)
        {
            if (!GameSupport.IsGuerrillaCampaign(GameInfo.Instance.ServerData.GuerrillaCampList[i]))
                continue;

            GuerrillaCampData campdata = GameInfo.Instance.ServerData.GuerrillaCampList[i];

            eGuerrillaCampaignType etype = GameSupport.GetGuerrillaCampaignType(campdata.Type);

            for (int j = 0; j < kCampaignMarkUnitList.Count; j++)
            {
                if (!kCampaignMarkUnitList[j].gameObject.active)
                {
                    if (kCampaignMarkUnitList[j].IsCampaignType(etype))
                    {
                        kCampaignMarkUnitList[j].UpdateSlot(campdata);
                    }
                }
            }
        }
    }

    public void CheckCampaignMark(UICampaignMarkUnit markunit, int condi)
    {
        markunit.gameObject.SetActive(false);

        for (int i = 0; i < GameInfo.Instance.ServerData.GuerrillaCampList.Count; i++)
        {
            if (!GameSupport.IsGuerrillaCampaign(GameInfo.Instance.ServerData.GuerrillaCampList[i]))
                continue;

            GuerrillaCampData campdata = GameInfo.Instance.ServerData.GuerrillaCampList[i];

            for (int j = 0; j < kCampaignMarkUnitList.Count; j++)
            {
                if (!markunit.gameObject.active)
                {
                    if (markunit.IsCampaignTypeCondition(campdata, condi))
                    {
                        markunit.UpdateSlot(campdata, true);
                    }
                }
            }
        }
    }
}
