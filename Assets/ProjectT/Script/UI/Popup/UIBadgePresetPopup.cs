using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBadgePresetPopup : FComponent
{
    [SerializeField] private PresetCharInfo presetCharInfo = null;

    private CharData _curCharData;
    private PresetCharData _curPresetCharData;

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        base.InitComponent();

        if (_curCharData != null)
        {
            SetCharacterData(ref presetCharInfo, _curCharData);
        }
        else
        {
            SetCharacterData(ref presetCharInfo, _curPresetCharData);
        }
    }

    public void SetPresetData(long charUid, PresetCharData presetCharData = null)
    {
        _curCharData = GameInfo.Instance.CharList.Find(x => x.CUID == charUid);
        _curPresetCharData = presetCharData;
    }

    private void OnClick()
    {
        SetUIActive(false);
    }

    private void SetCharacterData(ref PresetCharInfo presetCharInfo, CharData charData)
    {
        presetCharInfo.CharListSlot.UpdateSlot(UICharListSlot.ePos.Preset_Char_Info, 0, charData);

        for (int i = 0; i < presetCharInfo.SupportList.Count; i++)
        {
            presetCharInfo.SupportList[i].UpdateSlot(UIItemListSlot.ePosType.Preset_Card_Info, i, charData);
        }

        for (int i = 0; i < presetCharInfo.WeaponList.Count; i++)
        {
            if (presetCharInfo.WeaponList[i].WeaponSlot.ParentGO == null)
            {
                presetCharInfo.WeaponList[i].WeaponSlot.ParentGO = this.gameObject;
            }

            presetCharInfo.WeaponList[i].WeaponSlot.UpdateSlot(UIItemListSlot.ePosType.Preset_Weapon_Info, i, charData);

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
                presetCharInfo.WeaponList[i].GemList[j].WarningSprite.SetActive(false);
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
    }

    private void SetCharacterData(ref PresetCharInfo presetCharInfo, PresetCharData presetCharData)
    {
        if (presetCharData == null)
        {
            return;
        }

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
    }
}
