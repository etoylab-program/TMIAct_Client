using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PresetGemInfo
{
    public UITexture GemTexture;
    public GameObject LockObject;
    public UISprite WarningSprite;
    public UISprite SetOptSprite;
}

[System.Serializable]
public class PresetWeaponInfo
{
    public UIItemListSlot WeaponSlot;
    public List<PresetGemInfo> GemList;
}

[System.Serializable]
public class PresetCharInfo
{
    public UICharListSlot CharListSlot;
    public List<UIItemListSlot> SupportList;
    public List<PresetWeaponInfo> WeaponList;
    public List<UISkillListSlot> SkillList;
    public GameObject SubTeamObj;
    public UILabel SubTeamLabel;
}

public class UIPresetPopup : FComponent
{
    [Header("Current Setting Info")]
    [SerializeField] private PresetCharInfo curSettingInfo = null;
    [SerializeField] private UIArenaBattleListSlot curArenaSettingInfo = null;

    [Header("Preset Setting Info")]
    [SerializeField] private PresetCharInfo preSettingInfo = null;
    [SerializeField] private UIArenaBattleListSlot preArenaSettingInfo = null;

    [Header("Objects")]
    [SerializeField] private List<GameObject> contentsObjectList = null;
    [SerializeField] private List<GameObject> arenaObjectList = null;

    [Header("Toggles")]
    [SerializeField] private FToggle contentsToggle = null;
    [SerializeField] private FToggle arenaToggle = null;

    [Header("Other Setting")]
    [SerializeField] private UITexture arenaBGTex = null;
    [SerializeField] private UILabel presetNameLabel = null;
    [SerializeField] private UILabel arenaPresetNameLabel = null;
    [SerializeField] private UIButton arrowRightBtn = null;
    [SerializeField] private UIButton arrowLeftBtn = null;

    private CharData _curCharData;
    private PresetData _curPresetData;
    private ePresetKind _presetType;
    private bool _isNotLoadData = false;
    private int _prevCharCostumeId = 0;
    private int _prevCharCostumeColor = 0;
    private int _prevCharCostumeStateFlag = 0;

    private List<BadgeData> _checkBadgeDataList = new List<BadgeData>();
    private List<CharData> _checkCharDataList = new List<CharData>();

    public override void Awake()
    {
        base.Awake();

        contentsToggle.EventCallBack = OnEventTabSelect;
        arenaToggle.EventCallBack = OnEventTabSelect;
    }

    public override void InitComponent()
    {
        base.InitComponent();

        switch (_presetType)
        {
            case ePresetKind.CHAR:
            case ePresetKind.STAGE:
                {
                    contentsToggle.SetToggle(0, SelectEvent.Code);
                }
                break;

            case ePresetKind.ARENA:
            case ePresetKind.ARENA_TOWER:
            case ePresetKind.RAID:
                {
                    arenaToggle.SetToggle(0, SelectEvent.Code);
                }
                break;
        }
    }

    public override void OnEnable()
    {
        InitComponent();

        base.OnEnable();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        SetCheckData();

        _isNotLoadData = DataWarningCheck();

        arrowRightBtn.isEnabled = DataComparison(isSave: true);
        arrowLeftBtn.isEnabled = DataComparison(isLoad: true);

        switch (_presetType)
        {
            case ePresetKind.CHAR:
            case ePresetKind.STAGE:
                {
                    SetCharacterData(ref curSettingInfo, _curCharData);
                    SetCharacterData(ref preSettingInfo, _curPresetData);
                } break;
            case ePresetKind.ARENA:
                {
                    curArenaSettingInfo.UpdateSlot(eContentsPosKind.ARENA);
                    preArenaSettingInfo.UpdateSlot(_curPresetData, eContentsPosKind.ARENA);

                    arenaPresetNameLabel.textlocalize = _curPresetData.PresetName;
                } break;
            case ePresetKind.ARENA_TOWER:
                {
                    curArenaSettingInfo.Update_TowerSlot(eContentsPosKind.ARENA_TOWER);
                    preArenaSettingInfo.Update_TowerSlot(_curPresetData, eContentsPosKind.ARENA_TOWER);

                    arenaPresetNameLabel.textlocalize = _curPresetData.PresetName;
                } break;
            case ePresetKind.RAID:
                {
                    curArenaSettingInfo.UpdateSlot(eContentsPosKind.RAID);
                    preArenaSettingInfo.UpdateSlot(_curPresetData, eContentsPosKind.RAID);

                    arenaPresetNameLabel.textlocalize = _curPresetData.PresetName;
                } break;
        }
    }

    public override void OnClickClose()
    {
        base.OnClickClose();
    }

    private bool OnEventTabSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
        {
            return false;
        }

        switch (_presetType)
        {
            case ePresetKind.CHAR:
                {
                    if (_curCharData != null && GameInfo.Instance.CharPresetDataDict.ContainsKey(_curCharData.TableID))
                    {
                        if (nSelect < GameInfo.Instance.CharPresetDataDict[_curCharData.TableID].Length)
                        {
                            _curPresetData = GameInfo.Instance.CharPresetDataDict[_curCharData.TableID][nSelect];
                        }
                    }
                }
                break;

            case ePresetKind.STAGE:
                {
                    if (GameInfo.Instance.QuestPresetDatas != null && nSelect < GameInfo.Instance.QuestPresetDatas.Length)
                    {
                        _curPresetData = GameInfo.Instance.QuestPresetDatas[nSelect];
                    }
                }
                break;

            case ePresetKind.ARENA:
                {
                    if (GameInfo.Instance.ArenaPresetDatas != null && nSelect < GameInfo.Instance.ArenaPresetDatas.Length)
                    {
                        _curPresetData = GameInfo.Instance.ArenaPresetDatas[nSelect];
                    }
                }
                break;
            case ePresetKind.ARENA_TOWER:
                {
                    if (GameInfo.Instance.ArenaTowerPresetDatas != null && nSelect < GameInfo.Instance.ArenaTowerPresetDatas.Length)
                    {
                        _curPresetData = GameInfo.Instance.ArenaTowerPresetDatas[nSelect];
                    }
                }
                break;
            case ePresetKind.RAID:
                {
                    if (GameInfo.Instance.RaidPresetDatas != null && nSelect < GameInfo.Instance.RaidPresetDatas.Length)
                    {
                        _curPresetData = GameInfo.Instance.RaidPresetDatas[nSelect];
                    }
                }
                break;
        }

        if (_curPresetData != null)
        {
            if (string.IsNullOrEmpty(_curPresetData.PresetName))
            {
                switch (_presetType)
                {
                    case ePresetKind.CHAR:
                    case ePresetKind.STAGE:
                        {
                            _curPresetData.PresetName = FLocalizeString.Instance.GetText(1838, contentsToggle.kSelect + 1);
                        }
                        break;
                    case ePresetKind.ARENA:
                    case ePresetKind.ARENA_TOWER:
                    case ePresetKind.RAID:
                        {
                            _curPresetData.PresetName = FLocalizeString.Instance.GetText(1838, arenaToggle.kSelect + 1);
                        }
                        break;
                }
            }
        }

        if (type == SelectEvent.Click)
        {
            Renewal();
        }

        return true;
    }

    private void SetCheckData()
    {
        _checkBadgeDataList.Clear();
        _checkCharDataList.Clear();

        switch (_presetType)
        {
            case ePresetKind.CHAR:
            case ePresetKind.STAGE:
                {
                    _checkCharDataList.Add(_curCharData);
                }
                break;
            case ePresetKind.ARENA:
                {
                    List<long> arenaTeamList = GameInfo.Instance.TeamcharList;
                    foreach (long uid in arenaTeamList)
                    {
                        CharData charData = GameInfo.Instance.CharList.Find(x => x.CUID == uid);
                        _checkCharDataList.Add(charData);
                    }

                    _checkBadgeDataList = GameInfo.Instance.BadgeList.FindAll(x => x.PosKind == (int)eContentsPosKind.ARENA);
                }
                break;
            case ePresetKind.ARENA_TOWER:
                {
                    List<long> arenaTowerTeamList = GameSupport.GetArenaTowerTeamCharList();
                    foreach (long uid in arenaTowerTeamList)
                    {
                        CharData charData = GameInfo.Instance.CharList.Find(x => x.CUID == uid);
                        _checkCharDataList.Add(charData);
                    }

                    _checkBadgeDataList = GameInfo.Instance.BadgeList.FindAll(x => x.PosKind == (int)eContentsPosKind.ARENA_TOWER);
                }
                break;
            case ePresetKind.RAID:
                {
                    List<long> raidTeamList = GameInfo.Instance.RaidUserData.CharUidList;
                    foreach (long uid in raidTeamList)
                    {
                        CharData charData = GameInfo.Instance.GetCharData(uid);
                        _checkCharDataList.Add(charData);
                    }

                    _checkBadgeDataList = GameInfo.Instance.BadgeList.FindAll(x => x.PosKind == (int)eContentsPosKind.RAID);
                }
                break;
        }
    }

    private bool DataWarningCheck()
    {
        for (int c = 0; c < _checkCharDataList.Count; c++)
        {
            CharData charData = _checkCharDataList[c];
            if (charData == null)
            {
                continue;
            }

            PresetCharData presetCharData = _curPresetData.CharDatas[c];

            for (int i = 0; i < charData.EquipCard.Length; i++)
            {
                if (charData.EquipCard[i] != presetCharData.CardUids[i])
                {
                    CardData cardData = GameInfo.Instance.GetCardData(presetCharData.CardUids[i]);
                    if (0 < presetCharData.CardUids[i] && cardData == null)
                    {
                        return true;
                    }
                    else if (cardData != null && (int)eContentsPosKind.CHAR < cardData.PosKind)
                    {
                        return true;
                    }
                }
            }

            if (charData.EquipWeaponUID != presetCharData.WeaponDatas[(int)eWeaponSlot.MAIN].Uid)
            {
                WeaponData weaponData = GameInfo.Instance.GetWeaponData(presetCharData.WeaponDatas[(int)eWeaponSlot.MAIN].Uid);
                if (0 < presetCharData.WeaponDatas[(int)eWeaponSlot.MAIN].Uid && weaponData == null)
                {
                    return true;
                }
            }

            if (charData.EquipWeapon2UID != presetCharData.WeaponDatas[(int)eWeaponSlot.SUB].Uid)
            {
                WeaponData weaponData = GameInfo.Instance.GetWeaponData(presetCharData.WeaponDatas[(int)eWeaponSlot.SUB].Uid);
                if (0 < presetCharData.WeaponDatas[(int)eWeaponSlot.SUB].Uid && weaponData == null)
                {
                    return true;
                }
            }

            foreach (PresetWeaponData presetWeaponData in presetCharData.WeaponDatas)
            {
                if (presetWeaponData.Uid <= 0)
                {
                    continue;
                }

                WeaponData weaponData = GameInfo.Instance.WeaponList.Find(x => x.WeaponUID == presetWeaponData.Uid);
                if (weaponData != null)
                {
                    for (int i = 0; i < weaponData.SlotGemUID.Length; i++)
                    {
                        if (weaponData.SlotGemUID[i] != presetWeaponData.GemUids[i])
                        {
                            GemData gemData = GameInfo.Instance.GetGemData(presetWeaponData.GemUids[i]);
                            if (presetWeaponData.GemUids[i] != 0 && gemData == null)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < _curPresetData.BadgeUdis.Length; i++)
        {
            BadgeData badgeData = GameInfo.Instance.BadgeList.Find(x => x.BadgeUID == _curPresetData.BadgeUdis[i]);
            if (badgeData == null && 0 < _curPresetData.BadgeUdis[i])
            {
                return true;
            }
        }

        return false;
    }

    private bool DataComparison(bool isSave = false, bool isLoad = false)
    {
        if (isSave)
        {
            if (_checkCharDataList.Count <= _checkCharDataList.FindAll(x => x == null).Count)
            {
                return false;
            }

            for (int i = 0; i < _checkCharDataList.Count; i++)
            {
                if (i < _curPresetData.CharDatas.Length)
                {
                    if (_checkCharDataList[i] != null && _curPresetData.CharDatas[i] == null)
                    {
                        return true;
                    }
                }
            }
        }

        if (isLoad)
        {
            int emptyCount = 0;
            foreach (PresetCharData presetCharData in _curPresetData.CharDatas)
            {
                if (presetCharData == null || (presetCharData != null && presetCharData.CharUid <= 0))
                {
                    ++emptyCount;
                }
            }

            if (_curPresetData.CharDatas.Length <= emptyCount)
            {
                return false;
            }
        }

        for (int c = 0; c < _checkCharDataList.Count; c++)
        {
            PresetCharData presetCharData = _curPresetData.CharDatas[c];
            CharData charData = _checkCharDataList[c];
            if (charData == null)
            {
                if (0 < presetCharData.CharUid)
                {
                    return true;
                }
                else
                {
                    continue;
                }
            }

            if (charData.CUID != presetCharData.CharUid)
            {
                return true;
            }

            if (charData.EquipCostumeID != presetCharData.CostumeId)
            {
                return true;
            }

            uint flag = (uint)charData.CostumeStateFlag;
            GameTable.Costume.Param costumeParam = GameInfo.Instance.GameTable.FindCostume(charData.EquipCostumeID);
            if (costumeParam != null && costumeParam.SubHairChange == 1)
            {
                bool isOn = GameSupport._IsOnBitIdx(flag, (int)eCostumeStateFlag.CSF_HAIR);
                GameSupport._DoOnOffBitIdx(ref flag, (int)eCostumeStateFlag.CSF_HAIR, !isOn);
            }

            if ((int)flag != presetCharData.CostumeStateFlag)
            {
                return true;
            }

            if (charData.CostumeColor != presetCharData.CostumeColor)
            {
                return true;
            }

            for (int i = 0; i < charData.EquipCard.Length; i++)
            {
                if (charData.EquipCard[i] != presetCharData.CardUids[i])
                {
                    return true;
                }
            }

            if (charData.EquipWeaponUID != presetCharData.WeaponDatas[(int)eWeaponSlot.MAIN].Uid)
            {
                return true;
            }

            if (charData.EquipWeapon2UID != presetCharData.WeaponDatas[(int)eWeaponSlot.SUB].Uid)
            {
                return true;
            }

            if (charData.EquipWeaponSkinTID != presetCharData.WeaponDatas[(int)eWeaponSlot.MAIN].SkinTid)
            {
                return true;
            }

            if (charData.EquipWeapon2SkinTID != presetCharData.WeaponDatas[(int)eWeaponSlot.SUB].SkinTid)
            {
                return true;
            }

            foreach (PresetWeaponData presetWeaponData in presetCharData.WeaponDatas)
            {
                if (presetWeaponData.Uid <= 0)
                {
                    continue;
                }

                WeaponData weaponData = GameInfo.Instance.WeaponList.Find(x => x.WeaponUID == presetWeaponData.Uid);
                if (weaponData != null)
                {
                    for (int i = 0; i < weaponData.SlotGemUID.Length; i++)
                    {
                        if (weaponData.SlotGemUID[i] != presetWeaponData.GemUids[i])
                        {
                            return true;
                        }
                    }
                }
            }

            for (int i = 0; i < charData.EquipSkill.Length; i++)
            {
                if (charData.EquipSkill[i] != presetCharData.SkillIds[i])
                {
                    return true;
                }
            }
        }

        for (int i = 0; i < _curPresetData.BadgeUdis.Length; i++)
        {
            BadgeData badgeData = _checkBadgeDataList.Find(x => x.PosSlotNum == i);
            if (badgeData != null)
            {
                if (badgeData.BadgeUID != _curPresetData.BadgeUdis[i])
                {
                    return true;
                }
            }
            else
            {
                if (0 < _curPresetData.BadgeUdis[i])
                {
                    return true;
                }
            }
        }

        if (_presetType != ePresetKind.CHAR && GameSupport.GetSelectCardFormationID() != _curPresetData.CardTeamId)
        {
            return true;
        }

        return false;
    }

    private void SetCharacterData(ref PresetCharInfo presetCharInfo, CharData charData)
    {
        UICharListSlot.ePos charListSlotPos = _presetType == ePresetKind.CHAR ? UICharListSlot.ePos.Preset_Char : UICharListSlot.ePos.Preset_Stage;
        presetCharInfo.CharListSlot.UpdateSlot(charListSlotPos, 0, charData);

        for (int i = 0; i < presetCharInfo.SupportList.Count; i++)
        {
            presetCharInfo.SupportList[i].UpdateSlot(UIItemListSlot.ePosType.Preset_Card, i, charData);
        }

        for (int i = 0; i < presetCharInfo.WeaponList.Count; i++)
        {
            if (presetCharInfo.WeaponList[i].WeaponSlot.ParentGO == null)
            {
                presetCharInfo.WeaponList[i].WeaponSlot.ParentGO = this.gameObject;
            }

            presetCharInfo.WeaponList[i].WeaponSlot.UpdateSlot(UIItemListSlot.ePosType.Preset_Weapon, i, charData);

            long equipWeaponUid = 0;
            if (charData != null)
            {
                if (i == (int)eWeaponSlot.MAIN)
                {
                    equipWeaponUid = charData.EquipWeaponUID;
                }
                else
                {
                    equipWeaponUid = charData.EquipWeapon2UID;
                }
            }

            WeaponData weaponData = GameInfo.Instance.WeaponList.Find(x => x.WeaponUID == equipWeaponUid);

            int gemSlotMaxOpenCount = 0;
            if (weaponData != null)
            {
                gemSlotMaxOpenCount = GameSupport.GetWeaponGradeSlotCount(weaponData.TableData.Grade, weaponData.Wake);
            }

            for (int j = 0; j < presetCharInfo.WeaponList[i].GemList.Count; j++)
            {
                presetCharInfo.WeaponList[i].GemList[j].LockObject.SetActive(gemSlotMaxOpenCount <= j);
                presetCharInfo.WeaponList[i].GemList[j].SetOptSprite.SetActive(false);

                if (gemSlotMaxOpenCount <= j)
                {
                    continue;
                }

                if (weaponData == null || weaponData.SlotGemUID.Length <= j)
                {
                    continue;
                }

                GemData gemData = GameInfo.Instance.GemList.Find(x => x.GemUID == weaponData.SlotGemUID[j]);
                presetCharInfo.WeaponList[i].GemList[j].GemTexture.SetActive(gemData != null);
                if (gemData != null)
                {
                    presetCharInfo.WeaponList[i].GemList[j].GemTexture.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + gemData.TableData.Icon);
                    GameTable.GemSetType.Param gemSetTypeParam = GameInfo.Instance.GameTable.FindGemSetType(gemData.SetOptID);
                    presetCharInfo.WeaponList[i].GemList[j].SetOptSprite.SetActive(gemSetTypeParam != null);
                    if (presetCharInfo.WeaponList[i].GemList[j].SetOptSprite.gameObject.activeSelf)
                    {
                        presetCharInfo.WeaponList[i].GemList[j].SetOptSprite.spriteName = gemSetTypeParam.Icon;
                    }
                }
            }
        }

        List<CardData> equipCardList = new List<CardData>();
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            if (charData != null)
            {
                equipCardList.Add(GameInfo.Instance.GetCardData(charData.EquipCard[i]));
            }
            else
            {
                equipCardList.Add(null);
            }
        }

        for (int i = 0; i < presetCharInfo.SkillList.Count; i++)
        {
            if (presetCharInfo.SkillList[i].ParentGO == null)
            {
                presetCharInfo.SkillList[i].ParentGO = this.gameObject;
            }

            PassiveData passiveData = null;
            if (charData != null)
            {
                passiveData = charData.PassvieList.Find(x => x.SkillID == charData.EquipSkill[i]);
            }

            presetCharInfo.SkillList[i].UpdateSlot(UISkillListSlot.ePOS.VIEW, i, passiveData, charData, equipCardList);
        }

        presetCharInfo.SubTeamObj.SetActive(_presetType != ePresetKind.CHAR);
        if (!presetCharInfo.SubTeamObj.activeSelf)
        {
            return;
        }

        int cardFormationId = GameSupport.GetSelectCardFormationID();
        string cardFormationText;
        if (cardFormationId == (int)eCOUNT.NONE)
        {
            cardFormationText = FLocalizeString.Instance.GetText(1617);
        }
        else
        {
            GameTable.CardFormation.Param cardFormationParam = GameInfo.Instance.GameTable.FindCardFormation(x => x.ID == cardFormationId);
            cardFormationText = FLocalizeString.Instance.GetText(cardFormationParam.Name);
        }

        presetCharInfo.SubTeamLabel.textlocalize = cardFormationText;
    }

    private void SetCharacterData(ref PresetCharInfo presetCharInfo, PresetData presetData)
    {
        presetNameLabel.textlocalize = presetData.PresetName;

        PresetCharData presetCharData = presetData.CharDatas.FirstOrDefault();

        CharData charData = null;
        CharData sourCharData = GameInfo.Instance.CharList.Find(x => x.CUID == presetCharData.CharUid);
        if (sourCharData != null)
        {
            charData = sourCharData.PresetDataClone(presetCharData);
        }

        int costumeId = charData == null ? -1 : charData.EquipCostumeID;
        presetCharInfo.CharListSlot.UpdateSlot(UICharListSlot.ePos.Preset_Char_Info, 0, charData, costumeId);

        List<CardData> equipCardList = new List<CardData>();
        for (int i = 0; i < presetCharInfo.SupportList.Count; i++)
        {
            UIItemListSlot.ePosType cardPosType = UIItemListSlot.ePosType.Preset_Card_Info;
            CardData cardData = null;
            if (charData != null)
            {
                cardData = GameInfo.Instance.CardList.Find(x => x.CardUID == charData.EquipCard[i]);

                bool isWarning = false;
                if (0 < charData.EquipCard[i] && cardData == null)
                {
                    isWarning = true;
                }
                else if (cardData != null && (int)eContentsPosKind.CHAR < cardData.PosKind)
                {
                    isWarning = true;
                }

                if (isWarning)
                {
                    cardPosType = UIItemListSlot.ePosType.Preset_Card_Warning;
                }
            }
            presetCharInfo.SupportList[i].UpdateSlot(cardPosType, i, cardData);
            equipCardList.Add(cardData);
        }

        for (int i = 0; i < presetCharInfo.WeaponList.Count; i++)
        {
            if (presetCharInfo.WeaponList[i].WeaponSlot.ParentGO == null)
            {
                presetCharInfo.WeaponList[i].WeaponSlot.ParentGO = this.gameObject;
            }

            long weaponUid = 0;
            if (charData != null)
            {
                weaponUid = i == (int)eWeaponSlot.MAIN ? charData.EquipWeaponUID : charData.EquipWeapon2UID;
            }

            WeaponData weaponData = null;
            WeaponData originWeaponData = GameInfo.Instance.WeaponList.Find(x => x.WeaponUID == weaponUid);
            if (originWeaponData != null)
            {
                weaponData = originWeaponData.PresetDataClone(presetCharData.WeaponDatas[i]);
            }

            UIItemListSlot.ePosType weaponPosType = UIItemListSlot.ePosType.Preset_Weapon_Info;
            if (0 < weaponUid)
            {
                if (weaponData == null ||
                    GameInfo.Instance.GetEquipWeaponFacilityData(weaponUid) != null ||
                    GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList.Any(x => x == weaponUid))
                {
                    weaponPosType = UIItemListSlot.ePosType.Preset_Weapon_Warning;
                }
            }

            presetCharInfo.WeaponList[i].WeaponSlot.UpdateSlot(weaponPosType, i, weaponData);

            int gemSlotMaxOpenCount = 0;
            if (weaponData != null)
            {
                gemSlotMaxOpenCount = GameSupport.GetWeaponGradeSlotCount(weaponData.TableData.Grade, weaponData.Wake);
            }

            for (int j = 0; j < presetCharInfo.WeaponList[i].GemList.Count; j++)
            {
                GemData gemData = null;
                if (weaponData != null)
                {
                    gemData = GameInfo.Instance.GemList.Find(x => x.GemUID == weaponData.SlotGemUID[j]);
                }

                bool isLock = gemSlotMaxOpenCount <= j;
                presetCharInfo.WeaponList[i].GemList[j].LockObject.SetActive(isLock);
                presetCharInfo.WeaponList[i].GemList[j].WarningSprite.SetActive(!isLock && gemData == null && weaponData.SlotGemUID[j] != 0);
                presetCharInfo.WeaponList[i].GemList[j].SetOptSprite.SetActive(false);

                if (gemSlotMaxOpenCount <= j)
                {
                    continue;
                }

                presetCharInfo.WeaponList[i].GemList[j].GemTexture.SetActive(gemData != null);
                if (gemData != null)
                {
                    presetCharInfo.WeaponList[i].GemList[j].GemTexture.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + gemData.TableData.Icon);
                    GameTable.GemSetType.Param gemSetTypeParam = GameInfo.Instance.GameTable.FindGemSetType(gemData.SetOptID);
                    presetCharInfo.WeaponList[i].GemList[j].SetOptSprite.SetActive(gemSetTypeParam != null);
                    if (presetCharInfo.WeaponList[i].GemList[j].SetOptSprite.gameObject.activeSelf)
                    {
                        presetCharInfo.WeaponList[i].GemList[j].SetOptSprite.spriteName = gemSetTypeParam.Icon;
                    }
                }
            }
        }

        for (int i = 0; i < presetCharInfo.SkillList.Count; i++)
        {
            if (presetCharInfo.SkillList[i].ParentGO == null)
            {
                presetCharInfo.SkillList[i].ParentGO = this.gameObject;
            }

            PassiveData passiveData = null;
            if (charData != null)
            {
                passiveData = charData.PassvieList.Find(x => x.SkillID == charData.EquipSkill[i]);
            }

            presetCharInfo.SkillList[i].UpdateSlot(UISkillListSlot.ePOS.VIEW, i, passiveData, charData, equipCardList);
        }

        presetCharInfo.SubTeamObj.SetActive(_presetType != ePresetKind.CHAR);
        if (!presetCharInfo.SubTeamObj.activeSelf)
        {
            return;
        }

        string cardFormationText;
        if (presetData.CardTeamId == (int)eCOUNT.NONE)
        {
            cardFormationText = FLocalizeString.Instance.GetText(1617);
        }
        else
        {
            GameTable.CardFormation.Param cardFormationParam = GameInfo.Instance.GameTable.FindCardFormation(x => x.ID == presetData.CardTeamId);
            cardFormationText = FLocalizeString.Instance.GetText(cardFormationParam.Name);
        }

        presetCharInfo.SubTeamLabel.textlocalize = cardFormationText;
    }

    private void ObjectsIsActive()
    {
        bool isActiveContents = false;
        bool isActiveArena = false;

        switch (_presetType)
        {
            case ePresetKind.CHAR:
            case ePresetKind.STAGE:
                {
                    isActiveContents = true;
                } break;
            case ePresetKind.ARENA:
            case ePresetKind.ARENA_TOWER:
            case ePresetKind.RAID:
                {
                    GameObject panelBgObj = LobbyUIManager.Instance.GetPanelBG(kBGIndex);
                    if (panelBgObj != null)
                    {
                        UIBGUnit bgUnit = panelBgObj.GetComponent<UIBGUnit>();
                        if (bgUnit != null)
                        {
                            arenaBGTex.mainTexture = bgUnit.texBg.mainTexture;
                        }
                    }

                    isActiveArena = true;
                } break;
        }

        foreach (GameObject gObj in contentsObjectList)
        {
            gObj.SetActive(isActiveContents);

            if (_presetType == ePresetKind.ARENA_TOWER || _presetType == ePresetKind.RAID)
            {
                if (gObj.name.Equals("BG"))
                {
                    gObj.SetActive(true);
                }
            }
        }

        foreach (GameObject gObj in arenaObjectList)
        {
            gObj.SetActive(isActiveArena);

            if (_presetType == ePresetKind.ARENA_TOWER || _presetType == ePresetKind.RAID)
            {
                if (gObj.name.Equals("BG"))
                {
                    gObj.SetActive(false);
                }
            }
        }
    }

    public void SetPresetData(eCharSelectFlag charSelectFlag, ePresetKind presetType, long charUid = -1)
    {
        _curCharData = null;
        _curPresetData = null;

        switch(presetType)
        {
            case ePresetKind.CHAR:
            case ePresetKind.STAGE:
                {
                    SetCharData(charUid);
                } break;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.CardFormationType, charSelectFlag);

        _presetType = presetType;

        ObjectsIsActive();
    }

    public void SetCharData(long charUid)
    {
        _curCharData = GameInfo.Instance.CharList.Find(x => x.CUID == charUid);
    }

    public void OnClick_NameEditBtn()
    {
        UILabel presetLabel = presetNameLabel;
        switch(_presetType)
        {
            case ePresetKind.ARENA:
            case ePresetKind.ARENA_TOWER:
            case ePresetKind.RAID:
                {
                    presetLabel = arenaPresetNameLabel;
                } break;
        }

        InputFieldPopup.Show(FLocalizeString.Instance.GetText(3296), FLocalizeString.Instance.GetText(3297), presetLabel.textlocalize, 8, 0, false, () =>
        {
            int slotNum = 0;
            long charUid = 0;
            switch(_presetType)
            {
                case ePresetKind.CHAR:
                    {
                        charUid = _curCharData.CUID;
                        slotNum = contentsToggle.kSelect + 1;
                    } break;
                case ePresetKind.STAGE:
                    {
                        slotNum = contentsToggle.kSelect + 1;
                    } break;
                case ePresetKind.ARENA:
                case ePresetKind.ARENA_TOWER:
                case ePresetKind.RAID:
                    {
                        slotNum = arenaToggle.kSelect + 1;
                    } break;
            }
            GameInfo.Instance.Send_ReqUserPresetChangeName(_presetType, slotNum, charUid, InputFieldPopup.GetInputTextWithClose(), OnNet_Rename);
        });
    }

    private void OnNet_Rename(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        switch (_presetType)
        {
            case ePresetKind.CHAR:
            case ePresetKind.STAGE:
                {
                    contentsToggle.SetToggle(contentsToggle.kSelect, SelectEvent.Code);
                }
                break;
            case ePresetKind.ARENA:
            case ePresetKind.ARENA_TOWER:
            case ePresetKind.RAID:
                {
                    arenaToggle.SetToggle(arenaToggle.kSelect, SelectEvent.Code);
                }
                break;
        }

        Renewal();
    }

    public void OnClick_ArmoryBtn()
    {
        LobbyUIManager.Instance.ShowUI("ArmoryPopup", true);
    }

    public void OnClick_ArrowRightBtn()
    {
        int slotNum = 0;
        long charUid = 0;
        switch (_presetType)
        {
            case ePresetKind.CHAR:
            case ePresetKind.STAGE:
                {
                    charUid = _curCharData.CUID;
                    slotNum = contentsToggle.kSelect + 1;
                }
                break;
            case ePresetKind.ARENA:
            case ePresetKind.ARENA_TOWER:
            case ePresetKind.RAID:
                {
                    slotNum = arenaToggle.kSelect + 1;
                }
                break;
        }
        GameInfo.Instance.Send_ReqAddOrUpdateUserPreset(_presetType, charUid, slotNum, OnNet_PresetDataSave);
    }

    private void OnNet_PresetDataSave(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        switch(_presetType)
        {
            case ePresetKind.CHAR:
            case ePresetKind.STAGE:
                {
                    contentsToggle.SetToggle(contentsToggle.kSelect, SelectEvent.Code);
                } break;

            case ePresetKind.ARENA:
            case ePresetKind.ARENA_TOWER:
            case ePresetKind.RAID:
                {
                    arenaToggle.SetToggle(arenaToggle.kSelect, SelectEvent.Code);
                } break;
        }
        
        Renewal();

        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3299));
    }

    public void OnClick_ArrowLeftBtn()
    {
        string charNameStr = string.Empty;
        bool isEmptyMainWeapon = false;
        int mainIndex = (int)eWeaponSlot.MAIN;
        foreach (PresetCharData presetCharData in _curPresetData.CharDatas)
        {
            if (presetCharData.CharUid <= 0)
            {
                continue;
            }

            if (mainIndex < presetCharData.WeaponDatas.Length)
            {
                long weaponUid = presetCharData.WeaponDatas[mainIndex].Uid;
                WeaponData weaponData = GameInfo.Instance.GetWeaponData(weaponUid);
                if (weaponData == null ||
                    GameInfo.Instance.GetEquipWeaponFacilityData(weaponUid) != null ||
                    GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList.Any(x => x == weaponUid))
                {
                    CharData charData = GameInfo.Instance.GetCharData(presetCharData.CharUid);
                    if (charData != null)
                    {
                        charNameStr = FLocalizeString.Instance.GetText(charData.TableData.Name);
                    }

                    isEmptyMainWeapon = true;
                    break;
                }
            }
        }

        if (isEmptyMainWeapon)
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3307, charNameStr), null);
            return;
        }

        if (_curCharData != null)
        {
            _prevCharCostumeId = _curCharData.EquipCostumeID;
            _prevCharCostumeColor = _curCharData.CostumeColor;
            _prevCharCostumeStateFlag = _curCharData.CostumeStateFlag;
        }

        int slotNum = 0;
        long charUid = 0;
        switch (_presetType)
        {
            case ePresetKind.CHAR:
                {
                    charUid = _curCharData.CUID;
                    slotNum = contentsToggle.kSelect + 1;
                }
                break;
            case ePresetKind.STAGE:
                {
                    slotNum = contentsToggle.kSelect + 1;
                }
                break;
            case ePresetKind.ARENA:
            case ePresetKind.ARENA_TOWER:
            case ePresetKind.RAID:
                {
                    slotNum = arenaToggle.kSelect + 1;
                }
                break;
        }
        GameInfo.Instance.Send_ReqUserPresetLoad(_presetType, slotNum, charUid, OnNet_PresetDataLoad);
    }

    private void OnNet_PresetDataLoad(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        if (_isNotLoadData)
        {
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(3298), null);
        }

        List<long> refreshCharList = new List<long>();
        switch (_presetType)
        {
            case ePresetKind.CHAR:
                {
                    bool isChangeCostume = _prevCharCostumeId != _curCharData.EquipCostumeID;
                    bool isChangeCostumeColor = _prevCharCostumeColor != _curCharData.CostumeColor;
                    bool isChangeCostumeStateFlag = _prevCharCostumeStateFlag != _curCharData.CostumeStateFlag;

                    if (isChangeCostume)
                    {
                        RenderTargetChar.Instance.InitRenderTargetChar(_curCharData.TableID, _curCharData.CUID, true, eCharacterType.Character);
                    }

                    if (isChangeCostumeColor || isChangeCostumeStateFlag)
                    {
                        RenderTargetChar.Instance.SetCostumeBody(_curCharData.EquipCostumeID, _curCharData.CostumeColor, _curCharData.CostumeStateFlag, _curCharData.DyeingData);
                    }

                    if (isChangeCostume || isChangeCostumeColor)
                    {
                        refreshCharList.Add(_curCharData.CUID);
                    }

                    UICharInfoPanel charInfoPanel = LobbyUIManager.Instance.GetActiveUI<UICharInfoPanel>("CharInfoPanel");
                    if (charInfoPanel != null)
                    {
                        UICharInfoTabCostumePanel charInfoTabCostumePanel = charInfoPanel.kTabList[(int)UICharInfoPanel.eCHARINFOTAB.COSTUME] as UICharInfoTabCostumePanel;
                        if (charInfoTabCostumePanel.gameObject.activeSelf)
                        {
                            charInfoTabCostumePanel.SetSeleteCostumeID(_curCharData.EquipCostumeID);
                        }

                        charInfoPanel.Renewal(true);
                    }
                } break;
            case ePresetKind.STAGE:
                {
                    PresetCharData presetCharData = _curPresetData.CharDatas.FirstOrDefault();
                    if (presetCharData != null)
                    {
                        if (_curCharData.CUID != presetCharData.CharUid)
                        {
                            SetCharData(presetCharData.CharUid);
                        }
                    }

                    refreshCharList.Add(_curCharData.CUID);

                    UIStageDetailPopup stageDetailPopup = LobbyUIManager.Instance.GetActiveUI<UIStageDetailPopup>("StageDetailPopup");
                    if (stageDetailPopup != null)
                    {
                        UIValue.Instance.SetValue(UIValue.EParamType.StageCharCUID, _curCharData.CUID);
                        stageDetailPopup.InitComponent();
                        stageDetailPopup.Renewal(true);
                    }
                } break;
            case ePresetKind.ARENA:
                {
                    GameInfo.Instance.TeamcharList.Clear();
                    foreach (PresetCharData presetCharData in _curPresetData.CharDatas)
                    {
                        GameInfo.Instance.TeamcharList.Add(presetCharData.CharUid);
                    }

                    refreshCharList = GameInfo.Instance.TeamcharList;

                    UIArenaMainPanel arenaMainPanel = LobbyUIManager.Instance.GetActiveUI<UIArenaMainPanel>("ArenaMainPanel");
                    if (arenaMainPanel != null)
                    {
                        arenaMainPanel.Renewal(true);
                    }
                } break;
            case ePresetKind.ARENA_TOWER:
                {
                    GameInfo.Instance.TowercharList.Clear();
                    foreach (PresetCharData presetCharData in _curPresetData.CharDatas)
                    {
                        GameInfo.Instance.TowercharList.Add(presetCharData.CharUid);
                    }

                    refreshCharList = GameSupport.GetArenaTowerTeamCharList();

                    UIArenaTowerMainPanel arenaTowerMainPanel = LobbyUIManager.Instance.GetActiveUI<UIArenaTowerMainPanel>("ArenaTowerMainPanel");
                    if (arenaTowerMainPanel != null)
                    {
                        arenaTowerMainPanel.Renewal(true);
                    }
                } break;
            case ePresetKind.RAID:
                {
                    GameInfo.Instance.RaidUserData.CharUidList.Clear();
                    foreach (PresetCharData presetCharData in _curPresetData.CharDatas)
                    {
                        GameInfo.Instance.RaidUserData.CharUidList.Add(presetCharData.CharUid);
                    }

                    refreshCharList = GameInfo.Instance.RaidUserData.CharUidList;

                    UIRaidDetailPopup raidDetailPopup = LobbyUIManager.Instance.GetActiveUI<UIRaidDetailPopup>("RaidDetailPopup");
                    if (raidDetailPopup != null)
                    {
                        raidDetailPopup.Renewal(true);
                    }
                } break;
        }

        bool refreshLobbyChar = false;
        foreach (long cuid in refreshCharList)
        {
            for (int i = 0; i < GameInfo.Instance.UserData.ArrLobbyBgCharUid.Length; i++)
            {
                if (GameInfo.Instance.UserData.ArrLobbyBgCharUid[i] == cuid)
                {
                    refreshLobbyChar = true;
                    break;
                }
            }
        }

        if (refreshLobbyChar)
        {
            Lobby.Instance.ChangeMainChar();
            Lobby.Instance.SetShowLobbyPlayer();
        }

        Renewal();

        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3300));
    }

    public void OnClick_CharInfoBtn(int index)
    {
        long charUid = 0;
        List<long> charList = new List<long>();
        switch(_presetType)
        {
            case ePresetKind.ARENA:
                {
                    charList = GameInfo.Instance.TeamcharList;
                } break;
            case ePresetKind.ARENA_TOWER:
                {
                    charList = GameSupport.GetArenaTowerTeamCharList();
                } break;
            case ePresetKind.RAID:
                {
                    charList = GameInfo.Instance.RaidUserData.CharUidList;
                } break;
        }

        if (index < charList.Count)
        {
            charUid = charList[index];
        }

        if (charUid <= 0)
        {
            return;
        }

        UIBadgePresetPopup badgePresetPopup = LobbyUIManager.Instance.GetUI<UIBadgePresetPopup>("BadgePresetPopup");
        if (badgePresetPopup != null)
        {
            badgePresetPopup.SetPresetData(charUid);
            badgePresetPopup.SetUIActive(true);
        }
    }

    public void OnClick_PresetInfoBtn(int index)
    {
        PresetCharData presetCharData = null;
        if (index < _curPresetData.CharDatas.Length)
        {
            presetCharData = _curPresetData.CharDatas[index];
        }

        if (presetCharData == null || presetCharData.CharUid <= 0)
        {
            return;
        }

        UIBadgePresetPopup badgePresetPopup = LobbyUIManager.Instance.GetUI<UIBadgePresetPopup>("BadgePresetPopup");
        if (badgePresetPopup != null)
        {
            badgePresetPopup.SetPresetData(0, presetCharData);
            badgePresetPopup.SetUIActive(true);
        }
    }
}
