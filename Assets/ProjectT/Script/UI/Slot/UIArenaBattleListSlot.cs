using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public partial class UIArenaBattleListSlot : FSlot
{

    [System.Serializable]
    public class ArenaGradeUpSpr
    {
        public UISprite winSpr;
        public UISprite loseSpr;

        public void InitArenaGradeUpSpr()
        {
            winSpr.gameObject.SetActive(false);
            loseSpr.gameObject.SetActive(false);
        }

        public void SetWinLoseFlag(bool bWin)
        {
            winSpr.gameObject.SetActive(bWin);
            loseSpr.gameObject.SetActive(!bWin);
        }
    }

    public UITexture kIconTex;                  //유저가 설정한 아이콘
    public UILabel kNameLabel;                  //유저 닉네임
    public UILabel kLevelLabel;                 //지휘관 랭크
    public UILabel kCombatPowerLabel;           //팀 전투력
    public UILabel kGradeupLabel;               
    public UIButton kFirstCharChangeBtn;
	public UIButton kSecondCharChangeBtn;
	public UIButton kThirdCharChangeBtn;
	public UIButton kBatchDetailBtn;
    public UISprite kDekoSpr;

    public List<UIUserCharListSlot> kArenaUserCharSlotList;
    public List<UIBatchListSlotSlot> kBadgeSlotList;

    public GameObject kBadgeInfoTarget;
    public BadgeToolTipPopup.eToolTipDir kInfoPopupDir = BadgeToolTipPopup.eToolTipDir.NONE;

    public GameObject kArenaGradeObj;
    public UILabel kArenaGradeLabel;                    //아레나 점수
    public UISprite kArenaGradeIcoSpr;                  //아레나 등급 아이콘

    public GameObject kArenaRankingObj;
    public UILabel kArenaRankingLabel;                  //아레나 순위

    [Header("Grade Up Values")]
    public GameObject kArenaGradeUpObj;
    public UILabel kArenaGradeUpLabel;                  //승급전 진행중 텍스트
    public List<ArenaGradeUpSpr> kArenaGradeUpSprList;  //승급전 진행 상태

    [Header("CardTeam")]
    public GameObject kCardTeamInfoObj;
    public UILabel kCardTeamInfoNameLabel;
    public GameObject kCardTeamChangeBtn;
    public CardTeamToolTipPopup.eCardToolTipDir kCardTeamInfoPopupDir = CardTeamToolTipPopup.eCardToolTipDir.NONE;

    private bool _bEnemy = false;
    private TeamData _matchTeam;
    private GameTable.ArenaGrade.Param _arenaGradeTableData;
    private List<BadgeData> _badgeList;

    private eCharSelectFlag _charSelectFlag = eCharSelectFlag.ARENA;
    private eContentsPosKind _presetPosKind = eContentsPosKind._NONE_;

    private int _cardFormationID = 0;

    private bool mbFriendPVP = false;

    public void UpdateSlot(bool isFriendPVP, TeamData teamdata, bool bEnemy = true)
	{
        kDekoSpr.SetActive(true);

        mbFriendPVP = isFriendPVP;

        _charSelectFlag = eCharSelectFlag.ARENA;
        _presetPosKind = eContentsPosKind._NONE_;
        _matchTeam = teamdata;
        _bEnemy = bEnemy;
        if (_bEnemy)
        {
            SetEnemySlot();
        }
        else
        {
            SetUserSlot();

            _cardFormationID = GameSupport.GetSelectCardFormationID();
        }
        SetCardTeamInfo(_bEnemy);
    }

    public void UpdateSlot(eContentsPosKind presetPosKind)
    {
        UpdateSlot(false, null, false);

        kFirstCharChangeBtn.SetActive(false);
        kSecondCharChangeBtn.SetActive(false);
        kThirdCharChangeBtn.SetActive(false);
        kCardTeamChangeBtn.SetActive(false);
        kArenaGradeObj.SetActive(false);
        kArenaRankingObj.SetActive(false);

        _charSelectFlag = eCharSelectFlag.Preset;

        List<long> charUidList = null;
        if (presetPosKind == eContentsPosKind.RAID)
        {
            charUidList = GameInfo.Instance.RaidUserData.CharUidList;
        }
        else
        {
            charUidList = GameInfo.Instance.TeamcharList;
        }

        for (int i = 0; i < kArenaUserCharSlotList.Count; i++)
        {
            CharData charData = null;
            if (i < charUidList.Count)
            {
                charData = GameInfo.Instance.CharList.Find(x => x.CUID == charUidList[i]);
            }
            kArenaUserCharSlotList[i].UpdatePresetSlot(i, _charSelectFlag, charData);
        }

        bool isBatchActive = presetPosKind != eContentsPosKind.RAID;
        kDekoSpr.SetActive(isBatchActive);
        kBatchDetailBtn.SetActive(isBatchActive);
        for (int i = 0; i < kBadgeSlotList.Count; i++)
        {
            kBadgeSlotList[i].SetActive(isBatchActive);
        }

        if (isBatchActive)
        {
            int mainOptID = 0;
            BadgeData mainBadgeData = GameInfo.Instance.BadgeList.Find(x => x.PosSlotNum == ((int)eBadgeSlot.FIRST - 1) && x.PosKind == (int)eContentsPosKind.ARENA);
            if (mainBadgeData != null)
            {
                mainOptID = mainBadgeData.OptID[(int)eBadgeOptSlot.FIRST];
            }

            for (int i = 0; i < kBadgeSlotList.Count; i++)
            {
                int slotIdx = i;
                BadgeData badgedata = GameInfo.Instance.BadgeList.Find(x => x.PosSlotNum == slotIdx && x.PosKind == (int)eContentsPosKind.ARENA);
                if (badgedata != null)
                {
                    kBadgeSlotList[i].UpdateSlot(slotIdx + 1, mainOptID, badgedata, UIBatchListSlotSlot.eBadgeSlotType.Info, eContentsPosKind.PRESET);
                }
                else
                {
                    kBadgeSlotList[i].UpdateSlot(slotIdx + 1, mainOptID, null, UIBatchListSlotSlot.eBadgeSlotType.Info, eContentsPosKind.PRESET);
                }
            }
        }

        kCombatPowerLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(222), GameSupport.GetArenaTeamPower(presetPosKind));
    }

    public void UpdateSlot(PresetData presetData, eContentsPosKind presetPosKind)
    {
        if (presetData == null)
        {
            return;
        }

        UpdateSlot(false, null, false);

        kFirstCharChangeBtn.SetActive(false);
        kSecondCharChangeBtn.SetActive(false);
        kThirdCharChangeBtn.SetActive(false);
        kCardTeamChangeBtn.SetActive(false);
        kArenaGradeObj.SetActive(false);
        kArenaRankingObj.SetActive(false);

        kCombatPowerLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(222), GameSupport.GetArenaTeamPower(eContentsPosKind.PRESET, presetData, presetPosKind));

        _charSelectFlag = eCharSelectFlag.Preset;
        _presetPosKind = presetPosKind;

        for (int i = 0; i < kArenaUserCharSlotList.Count; i++)
        {
            CharData charData = null;
            if (i < presetData.CharDatas.Length)
            {
                if (presetData.CharDatas[i] != null)
                {
                    CharData originCharData = GameInfo.Instance.CharList.Find(x => x.CUID == presetData.CharDatas[i].CharUid);
                    if (originCharData != null)
                    {
                        charData = originCharData.PresetDataClone(presetData.CharDatas[i]);
                    }
                }
            }
            kArenaUserCharSlotList[i].UpdatePresetSlot(i, _charSelectFlag, charData);
        }

        bool isBatchActive = presetPosKind != eContentsPosKind.RAID;
        kDekoSpr.SetActive(isBatchActive);
        kBatchDetailBtn.SetActive(isBatchActive);
        for (int i = 0; i < kBadgeSlotList.Count; i++)
        {
            kBadgeSlotList[i].SetActive(isBatchActive);
        }

        if (isBatchActive)
        {
            int mainOptID = 0;
            BadgeData mainBadgeData = GameInfo.Instance.BadgeList.Find(x => x.BadgeUID == presetData.BadgeUdis[(int)eBadgeSlot.FIRST - 1]);
            if (mainBadgeData != null)
            {
                mainOptID = mainBadgeData.OptID[(int)eBadgeOptSlot.FIRST];
            }

            _badgeList.Clear();
            for (int i = 0; i < kBadgeSlotList.Count; i++)
            {
                BadgeData badgedata = GameInfo.Instance.BadgeList.Find(x => x.BadgeUID == presetData.BadgeUdis[i]);
                kBadgeSlotList[i].UpdateSlot(i + 1, mainOptID, badgedata, UIBatchListSlotSlot.eBadgeSlotType.Info, eContentsPosKind.PRESET, badgeUid: presetData.BadgeUdis[i]);
                _badgeList.Add(badgedata);
            }
        }

        _cardFormationID = presetData.CardTeamId;
        if (_cardFormationID == (int)eCOUNT.NONE)
        {
            kCardTeamInfoNameLabel.textlocalize = FLocalizeString.Instance.GetText(1617);
        }
        else
        {
            GameTable.CardFormation.Param cardFrm = GameInfo.Instance.GameTable.FindCardFormation(x => x.ID == _cardFormationID);
            kCardTeamInfoNameLabel.textlocalize = FLocalizeString.Instance.GetText(cardFrm.Name);
        }
    }

    private void SetUserSlot()
    {
        List<long> _teamCharList = GameInfo.Instance.TeamcharList;

        _arenaGradeTableData = GetArenaGradeTableWithGradeID(GameInfo.Instance.UserBattleData.Now_GradeId);
        //_badgeList = GameInfo.Instance.BadgeList;
        _badgeList = GameInfo.Instance.BadgeList.FindAll(x => x.PosKind == (int)eContentsPosKind.ARENA);
        
        LobbyUIManager.Instance.GetUserMarkIcon(ParentGO, this.gameObject, GameInfo.Instance.UserData.UserMarkID, ref kIconTex);

        kNameLabel.textlocalize = GameInfo.Instance.UserData.GetNickName();
        kLevelLabel.textlocalize = GameInfo.Instance.UserData.Level.ToString();
        kCombatPowerLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(222), GameSupport.GetArenaTeamPower(eContentsPosKind.ARENA));

        kArenaGradeIcoSpr.spriteName = _arenaGradeTableData.Icon;
        kArenaGradeLabel.textlocalize = GameInfo.Instance.UserBattleData.Now_Score.ToString("#,##0");
        //kArenaRankingLabel.textlocalize = GameInfo.Instance.UserBattleData.Now_Rank.ToString("#,##0");
        kArenaRankingLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1448), GameInfo.Instance.UserBattleData.Now_Rank.ToString("#,##0"));

        if (GameInfo.Instance.ArenaRankingList != null)
        {
            TeamData rankdata = GameInfo.Instance.ArenaRankingList.RankingSimpleList.Find(x => x.UUID == GameInfo.Instance.UserData.UUID);
            if (rankdata == null)
            {
                //kArenaRankingLabel.textlocalize = "-";
                kArenaRankingLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1448), "-");
            }
            else
            {
                //kArenaRankingLabel.textlocalize = rankdata.Rank.ToString("#,##0");
                kArenaRankingLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1448), rankdata.Rank.ToString("#,##0"));
            }
        }

        //전부 다 비활성화 후 적용
        for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
        {
            kArenaUserCharSlotList[i].CharSelectFlag = eCharSelectFlag.ARENA;
            kArenaUserCharSlotList[i].UpdateArenaTeamSlot(i, null);
        }

        for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
        {
            kArenaUserCharSlotList[i].UpdateArenaTeamSlot(i, GameInfo.Instance.GetCharData(_teamCharList[i]), true);
        }

        int mainOptID = 0;
        BadgeData mainBadgeData = GameInfo.Instance.BadgeList.Find(x => x.PosSlotNum == ((int)eBadgeSlot.FIRST - 1) && x.PosKind == (int)eContentsPosKind.ARENA);
        if(mainBadgeData != null)
        {
            mainOptID = mainBadgeData.OptID[(int)eBadgeOptSlot.FIRST];
        }

        for (int i = 0; i < kBadgeSlotList.Count; i++)
        {
            int slotIdx = i;
            BadgeData badgedata = GameInfo.Instance.BadgeList.Find(x => x.PosSlotNum == slotIdx && x.PosKind == (int)eContentsPosKind.ARENA);
            if (badgedata != null)
            {
                kBadgeSlotList[i].UpdateSlot(slotIdx + 1, mainOptID, badgedata, UIBatchListSlotSlot.eBadgeSlotType.Equip, eContentsPosKind.ARENA);
            }
            else
            {
                kBadgeSlotList[i].UpdateSlot(slotIdx + 1, mainOptID, null, UIBatchListSlotSlot.eBadgeSlotType.Equip, eContentsPosKind.ARENA);
            }
        }

        ArenaGradeUpCheckFlag();
    }
    private void SetEnemySlot()
    {
        kFirstCharChangeBtn.gameObject.SetActive(false);
        kSecondCharChangeBtn.gameObject.SetActive(false);
        kThirdCharChangeBtn.gameObject.SetActive(false);

        kArenaGradeObj.SetActive(true);
        kArenaRankingObj.SetActive(true);
        kArenaGradeUpObj.SetActive(false);

        int gradeId = GameSupport.GetArenaGradeWithNowPoint((int)_matchTeam.Score, eArenaGradeFlag.GRADE_ID);
        _arenaGradeTableData = GetArenaGradeTableWithGradeID(gradeId);
        _badgeList = _matchTeam.badgelist;

        LobbyUIManager.Instance.GetUserMarkIcon(ParentGO, this.gameObject, _matchTeam.UserMark, ref kIconTex);

        kNameLabel.textlocalize = _matchTeam.GetUserNickName();
        kLevelLabel.textlocalize = _matchTeam.UserLv.ToString();
        kCombatPowerLabel.textlocalize = _matchTeam.TeamPower.ToString("#,##0");

        if (_matchTeam.Rank == (int)eCOUNT.NONE)
            kArenaRankingLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1448), "-");
        else
            kArenaRankingLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1448), _matchTeam.Rank.ToString("#,##0"));

        kArenaGradeLabel.textlocalize = _matchTeam.Score.ToString("#,##0");

        kArenaGradeIcoSpr.spriteName = _arenaGradeTableData.Icon;

        //전부 다 비활성화 후 적용
        for (int i = 0; i < kArenaUserCharSlotList.Count; i++)
        {
            kArenaUserCharSlotList[i].UpdateArenaTeamSlot(i, null);
        }

        for (int i = 0; i < _matchTeam.charlist.Count; i++)
        {
            if(_matchTeam.charlist[i] != null)
                kArenaUserCharSlotList[i].UpdateArenaEnemyDetial(i, _matchTeam.charlist[i].CharData);
            else
                kArenaUserCharSlotList[i].UpdateArenaEnemyDetial(i, null);
        }

        int mainOptID = 0;
        
        BadgeData mainBadgeData = _matchTeam.badgelist.Find(x => x.PosSlotNum == ((int)eBadgeSlot.FIRST - 1) && x.PosKind == (int)eContentsPosKind.ARENA);
        if (mainBadgeData != null)
        {
            mainOptID = mainBadgeData.OptID[(int)eBadgeOptSlot.FIRST];
        }

        for (int i = 0; i < kBadgeSlotList.Count; i++)
        {
            int slotIdx = i;
            BadgeData badgedata = _matchTeam.badgelist.Find(x => x.PosSlotNum == slotIdx && x.PosKind == (int)eContentsPosKind.ARENA);
            if (badgedata != null)
            {
                kBadgeSlotList[i].UpdateSlot(slotIdx + 1, mainOptID, badgedata, UIBatchListSlotSlot.eBadgeSlotType.Info, eContentsPosKind.ARENA);
            }
            else
            {
                kBadgeSlotList[i].UpdateSlot(slotIdx + 1, mainOptID, null, UIBatchListSlotSlot.eBadgeSlotType.Info, eContentsPosKind.ARENA);
            }
        }
    }

    private GameTable.ArenaGrade.Param GetArenaGradeTableWithGradeID(int gradeId)
    {
        return GameInfo.Instance.GameTable.ArenaGrades.Find(x => x.GradeID == gradeId);
    }

    public void OnClick_Slot()
	{
	}

    //캐릭터 선택 CharSeletePopup
    public void OnClick_CharChangeBtn(int slotIdx)
    {
        //선택한 슬롯 인덱스 - 교체할때를 대비하여 저장
        Log.Show(slotIdx);
       

        if (_charSelectFlag == eCharSelectFlag.ARENA)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.ArenaTeamCharSlot, (int)slotIdx);
            UIValue.Instance.SetValue(UIValue.EParamType.CharSeletePopupType, (int)_charSelectFlag);
            LobbyUIManager.Instance.ShowUI("CharSeletePopup", true);
        }
        else if (_charSelectFlag == eCharSelectFlag.ARENATOWER || _charSelectFlag == eCharSelectFlag.ARENATOWER_STAGE)
        {   
            GameSupport.ShowCharSeletePopupArenaTower(slotIdx, _charSelectFlag);            
        }
    }

    public void OnClick_BatchDetailBtn()
	{
        if (kInfoPopupDir != BadgeToolTipPopup.eToolTipDir.NONE)
        {
            if (_presetPosKind == eContentsPosKind._NONE_)
            {
                BadgeToolTipPopup.Show(_badgeList, kBadgeInfoTarget, kInfoPopupDir, _charSelectFlag, _charSelectFlag == eCharSelectFlag.ARENA ? eContentsPosKind.ARENA : eContentsPosKind.ARENA_TOWER);
            }
            else
            {
                BadgeToolTipPopup.Show(_badgeList, kBadgeInfoTarget, kInfoPopupDir, _charSelectFlag, _presetPosKind);
            }            
        }
    }

    /// <summary>
    /// 팀 전투력 ( 유저 본인 )
    /// </summary>
    public void SetUserTeamPower(bool bAtkBuff, bool bDefBuff)
    {
        string strColor = FLocalizeString.Instance.GetText((int)eTEXTID.WHITE_TEXT_COLOR);
        if (bAtkBuff || bDefBuff)
        {
            float atkValue = bAtkBuff ? GameInfo.Instance.BattleConfig.ArenaAtkBuffRate : 0f;
            float defValue = bDefBuff ? GameInfo.Instance.BattleConfig.ArenaDefBuffRate : 0f;

            strColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);

            int userTeamPower = GameSupport.GetArenaTeamPower(eContentsPosKind.ARENA);
            int temp = (int)(userTeamPower + (userTeamPower * ((atkValue + defValue) * 0.5f)));

            kCombatPowerLabel.textlocalize = string.Format(strColor, temp);
        }
        else
        {
            kCombatPowerLabel.textlocalize = string.Format(strColor, GameSupport.GetArenaTeamPower(eContentsPosKind.ARENA));
        }
    }

    public void SetSynastryTeam(bool bEnemy)
    {
        if(bEnemy)
        {
            //상대가 유저 본인
            for(int i = 0; i < (int)eArenaTeamSlotPos._MAX_; i++)
            {
                if(GameInfo.Instance.TeamcharList[i] == 0)
                    kArenaUserCharSlotList[i].SetSynastry((long)GameInfo.Instance.TeamcharList[i]);
                else
                {
                    kArenaUserCharSlotList[i].SetSynastry(GameInfo.Instance.GetCharData(GameInfo.Instance.TeamcharList[i]).TableID);
                }
            }
        }
        else
        {
            //상대유저
            //상대가 유저 본인
            for (int i = 0; i < (int)eArenaTeamSlotPos._MAX_; i++)
            {
                kArenaUserCharSlotList[i].SetSynastry(0, GameInfo.Instance.MatchTeam.charlist[i]);
            }
        }
    }

    /// <summary>
    /// 남은 승급전 횟수가 남아있으면 승급전 진행중
    /// </summary>
    /// <returns></returns>
    private bool ArenaGradeUpCheckFlag()
    {
        /*
        0000 = 0
        0001 = 1
        0010 = 2
        0011 = 3
        ...
         */
        //GameInfo.Instance.UserBattleData.Now_PromotionRemainCnt = 1;
        //GameInfo.Instance.UserBattleData.Now_PromotionWinCnt = 2;
        if (!mbFriendPVP && GameInfo.Instance.UserBattleData.Now_PromotionRemainCnt > 0)
        {
            kArenaGradeObj.SetActive(false);
            kArenaRankingObj.SetActive(false);
            kArenaGradeUpObj.SetActive(true);

            List<eArenaGradeUpFlag> arenaGradeUpFlag = GameSupport.IsArenaGradeUpWinLoseFlags();

            for (int i = 0; i < arenaGradeUpFlag.Count; i++)
            {
                switch (arenaGradeUpFlag[i])
                {
                    case eArenaGradeUpFlag.NONE:
                        kArenaGradeUpSprList[i].InitArenaGradeUpSpr();
                        break;
                    case eArenaGradeUpFlag.WIN:
                        kArenaGradeUpSprList[i].SetWinLoseFlag(true);
                        break;
                    case eArenaGradeUpFlag.LOSE:
                        kArenaGradeUpSprList[i].SetWinLoseFlag(false);
                        break;
                }

            }

            return true;
        }
        else
        {
            kArenaGradeObj.SetActive(true);
            kArenaRankingObj.SetActive(true);
            kArenaGradeUpObj.SetActive(false);

            return false;
        }
        //return false;
    }

    private void SetCardTeamInfo(bool benemy)
    {
        kCardTeamChangeBtn.SetActive(!benemy);
        kCardTeamInfoObj.SetActive(true);

        if (benemy)
        {
            if (_matchTeam.CardFormtaionID == (int)eCOUNT.NONE)
            {
                kCardTeamInfoNameLabel.textlocalize = FLocalizeString.Instance.GetText(1617);
            }
            else
            {
                GameTable.CardFormation.Param cardFrm = GameInfo.Instance.GameTable.FindCardFormation(x => x.ID == _matchTeam.CardFormtaionID);
                kCardTeamInfoNameLabel.textlocalize = FLocalizeString.Instance.GetText(cardFrm.Name);
            }
        }
        else
        {
            _cardFormationID = GameSupport.GetSelectCardFormationID();
            if (_cardFormationID == (int)eCOUNT.NONE)
            {
                kCardTeamInfoNameLabel.textlocalize = FLocalizeString.Instance.GetText(1617);
            }
            else
            {
                GameTable.CardFormation.Param cardFrm = GameInfo.Instance.GameTable.FindCardFormation(x => x.ID == _cardFormationID);
                kCardTeamInfoNameLabel.textlocalize = FLocalizeString.Instance.GetText(cardFrm.Name);
            }
        }
    }


    public void OnClick_CardTeamInfoBtn()
    {
        if (_bEnemy)
        {
            if (_matchTeam.CardFormtaionID == (int)eCOUNT.NONE)
            {
                return;
            }
            CardTeamToolTipPopup.Show(_matchTeam.CardFormtaionID, kCardTeamInfoObj, kCardTeamInfoPopupDir);
        }
        else
        {
            if (_cardFormationID == (int)eCOUNT.NONE)
            {
                return;
            }
            CardTeamToolTipPopup.Show(_cardFormationID, kCardTeamInfoObj, kCardTeamInfoPopupDir);
        }
    }

    public void OnClick_CardTeamChangeBtn()
    {
        LobbyUIManager.Instance.ShowUI("ArmoryPopup", true);
    }

}
