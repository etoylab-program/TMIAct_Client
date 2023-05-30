using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public partial class UIArenaBattleListSlot : FSlot
{
    
    public void Update_TowerMainPanel()
    {
        _charSelectFlag = eCharSelectFlag.ARENATOWER;

        kArenaGradeUpObj.SetActive(false);
        kArenaGradeObj.SetActive(false);
        kArenaRankingObj.SetActive(false);

        List<long> _towerTeamCharList = GameSupport.GetArenaTowerTeamCharList();

        //_badgeList = GameInfo.Instance.BadgeList;
        _badgeList = GameInfo.Instance.BadgeList.FindAll(x => x.PosKind == (int)eContentsPosKind.ARENA_TOWER);

        LobbyUIManager.Instance.GetUserMarkIcon(ParentGO, this.gameObject, GameInfo.Instance.UserData.UserMarkID, ref kIconTex);

        kNameLabel.textlocalize = GameInfo.Instance.UserData.GetNickName();
        kLevelLabel.textlocalize = GameInfo.Instance.UserData.Level.ToString();
        kCombatPowerLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(222), GameSupport.GetArenaTeamPower(eContentsPosKind.ARENA_TOWER));

        //kArenaGradeIcoSpr.spriteName = _arenaGradeTableData.Icon;
        kArenaGradeLabel.textlocalize = GameInfo.Instance.UserBattleData.Now_Score.ToString("#,##0");        
        kArenaRankingLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1448), GameInfo.Instance.UserBattleData.Now_Rank.ToString("#,##0"));


        for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
        {
            kArenaUserCharSlotList[i].CharSelectFlag = eCharSelectFlag.ARENATOWER;
            kArenaUserCharSlotList[i].UpdateArenaTeamSlot(i, null);
        }

        for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
        {
            kArenaUserCharSlotList[i].UpdateArenaTeamSlot(i, GameInfo.Instance.GetArenaTowerCharData(_towerTeamCharList[i]), true);
        }


        int mainOptID = 0;
        int BadgeSlotFirst = (int)eBadgeOptSlot.FIRST;
        int PosKind = (int)eContentsPosKind.ARENA_TOWER;
        BadgeData mainBadgeData = GameInfo.Instance.BadgeList.Find(x => x.PosSlotNum == BadgeSlotFirst && x.PosKind == PosKind);
        if (mainBadgeData != null)
        {   
            mainOptID = mainBadgeData.OptID[BadgeSlotFirst];
        }

        for (int i = 0; i < kBadgeSlotList.Count; i++)
        {
            int slotIdx = i;
            BadgeData badgedata = GameInfo.Instance.BadgeList.Find(x => x.PosSlotNum == slotIdx && x.PosKind == PosKind);
            if (badgedata != null)
            {
                kBadgeSlotList[i].UpdateSlot(slotIdx + 1, mainOptID, badgedata, UIBatchListSlotSlot.eBadgeSlotType.Equip, eContentsPosKind.ARENA_TOWER);
            }
            else
            {
                kBadgeSlotList[i].UpdateSlot(slotIdx + 1, mainOptID, null, UIBatchListSlotSlot.eBadgeSlotType.Equip, eContentsPosKind.ARENA_TOWER);
            }
        }

        SetCardTeamInfo(false);
    }

    public void Update_TowerSlot(eContentsPosKind presetPosKind)
    {
        Update_TowerMainPanel();

        kFirstCharChangeBtn.SetActive(false);
        kSecondCharChangeBtn.SetActive(false);
        kThirdCharChangeBtn.SetActive(false);
        kCardTeamChangeBtn.SetActive(false);

        _charSelectFlag = eCharSelectFlag.Preset;

        List<long> _towerTeamCharList = GameSupport.GetArenaTowerTeamCharList();
        for (int i = 0; i < kArenaUserCharSlotList.Count; i++)
        {
            CharData charData = null;
            if (i < _towerTeamCharList.Count)
            {
                charData = GameInfo.Instance.GetArenaTowerCharData(_towerTeamCharList[i]);
            }
            kArenaUserCharSlotList[i].UpdatePresetSlot(i, _charSelectFlag, charData);
        }

        int mainOptID = 0;
        int BadgeSlotFirst = (int)eBadgeOptSlot.FIRST;
        int PosKind = (int)eContentsPosKind.ARENA_TOWER;
        BadgeData mainBadgeData = GameInfo.Instance.BadgeList.Find(x => x.PosSlotNum == BadgeSlotFirst && x.PosKind == PosKind);
        if (mainBadgeData != null)
        {
            mainOptID = mainBadgeData.OptID[BadgeSlotFirst];
        }

        for (int i = 0; i < kBadgeSlotList.Count; i++)
        {
            int slotIdx = i;
            BadgeData badgedata = GameInfo.Instance.BadgeList.Find(x => x.PosSlotNum == slotIdx && x.PosKind == PosKind);
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

    public void Update_TowerSlot(PresetData presetData, eContentsPosKind presetPosKind)
    {
        if (presetData == null)
        {
            return;
        }

        Update_TowerMainPanel();

        kFirstCharChangeBtn.SetActive(false);
        kSecondCharChangeBtn.SetActive(false);
        kThirdCharChangeBtn.SetActive(false);
        kCardTeamChangeBtn.SetActive(false);

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

    public void OnClick_CardFormation()
    {
        LobbyUIManager.Instance.ShowUI("CardFormationPopup", true);
    }
}
