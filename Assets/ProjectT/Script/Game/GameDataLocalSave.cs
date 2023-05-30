using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public partial class GachaCategoryData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetString("GachaCategoryData_Type_" + num.ToString(), Type);
        PlayerPrefs.SetInt("GachaCategoryData_Text_" + num.ToString(), Text);
        PlayerPrefs.SetInt("GachaCategoryData_StoreID1_" + num.ToString(), StoreID1);
        PlayerPrefs.SetInt("GachaCategoryData_StoreID2_" + num.ToString(), StoreID2);
        PlayerPrefs.SetString("GachaCategoryData_UrlBtnImage_" + num.ToString(), UrlBtnImage);
        PlayerPrefs.SetString("GachaCategoryData_UrlBGImage_" + num.ToString(), UrlBGImage);
        PlayerPrefs.SetString("GachaCategoryData_StartDate_" + num.ToString(), StartDate.ToString());
        PlayerPrefs.SetString("GachaCategoryData_EndDate_" + num.ToString(), EndDate.ToString());
    }
    public void LoadData(int num)
    {
        Type = PlayerPrefs.GetString("GachaCategoryData_Type_" + num.ToString());
        Text = PlayerPrefs.GetInt("GachaCategoryData_Text_" + num.ToString());
        StoreID1 = PlayerPrefs.GetInt("GachaCategoryData_StoreID1_" + num.ToString());
        StoreID2 = PlayerPrefs.GetInt("GachaCategoryData_StoreID2_" + num.ToString());
        UrlBtnImage = PlayerPrefs.GetString("GachaCategoryData_UrlBtnImage_" + num.ToString());
        UrlBGImage = PlayerPrefs.GetString("GachaCategoryData_UrlBGImage_" + num.ToString());

        DateTime v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("GachaCategoryData_StartDate_" + num.ToString()), out v);
        StartDate = v;

        v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("GachaCategoryData_EndDate_" + num.ToString()), out v);
        EndDate = v;
    }
}

public partial class StoreSaleData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("StoreSaleData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("StoreSaleData_LimitValue_" + num.ToString(), LimitValue);
        PlayerPrefs.SetInt("StoreSaleData_DiscountRate_" + num.ToString(), DiscountRate);
        PlayerPrefs.SetInt("StoreSaleData_LimitType_" + num.ToString(), LimitType);
    }
    public void LoadData(int num)
    {
        TableID = PlayerPrefs.GetInt("StoreSaleData_TableID_" + num.ToString());
        LimitValue = PlayerPrefs.GetInt("StoreSaleData_LimitValue_" + num.ToString());
        DiscountRate = PlayerPrefs.GetInt("StoreSaleData_DiscountRate_" + num.ToString());
        LimitType = PlayerPrefs.GetInt("StoreSaleData_LimitType_" + num.ToString());
    }

}

public partial class BannerData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetString("BannerData_UrlImage_" + num.ToString(), UrlImage);
        PlayerPrefs.SetString("BannerData_FunctionType_" + num.ToString(), FunctionType);
        PlayerPrefs.SetString("BannerData_FunctionValue1_" + num.ToString(), FunctionValue1);
        PlayerPrefs.SetString("BannerData_FunctionValue2_" + num.ToString(), FunctionValue2);
        PlayerPrefs.SetString("BannerData_StartDate_" + num.ToString(), StartDate.ToString());
        PlayerPrefs.SetString("BannerData_EndDate_" + num.ToString(), EndDate.ToString());
    }
    public void LoadData(int num)
    {
        UrlImage = PlayerPrefs.GetString("BannerData_UrlImage_" + num.ToString());
        FunctionType = PlayerPrefs.GetString("BannerData_FunctionType_" + num.ToString());
        FunctionValue1 = PlayerPrefs.GetString("BannerData_FunctionValue1_" + num.ToString());
        FunctionValue2 = PlayerPrefs.GetString("BannerData_FunctionValue2_" + num.ToString());

        DateTime v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("BannerData_StartDate_" + num.ToString()), out v);
        StartDate = v;

        //  티켓 충전 남은 시간
        v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("BannerData_EndDate_" + num.ToString()), out v);
        EndDate = v;
    }
}

public partial class GuerrillaCampData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("GuerrillaCampData_Condition_" + num.ToString(), Condition);
        PlayerPrefs.SetInt("GuerrillaCampData_EffectValue_" + num.ToString(), EffectValue);
        PlayerPrefs.SetString("GuerrillaCampData_Type_" + num.ToString(), Type);
        PlayerPrefs.SetInt("GuerrillaCampData_Name_" + num.ToString(), Name);
        PlayerPrefs.SetInt("GuerrillaCampData_Desc_" + num.ToString(), Desc);
        PlayerPrefs.SetString("GuerrillaCampData_StartDate_" + num.ToString(), StartDate.ToString());
        PlayerPrefs.SetString("GuerrillaCampData_EndDate_" + num.ToString(), EndDate.ToString());
    }
    public void LoadData(int num)
    {
        Condition = PlayerPrefs.GetInt("GuerrillaCampData_Condition_" + num.ToString(), Condition);
        EffectValue = PlayerPrefs.GetInt("GuerrillaCampData_EffectValue_" + num.ToString(), EffectValue);
        Type = PlayerPrefs.GetString("GuerrillaCampData_Type_" + num.ToString(), Type);
        Name = PlayerPrefs.GetInt("GuerrillaCampData_Name_" + num.ToString());
        Desc = PlayerPrefs.GetInt("GuerrillaCampData_Desc_" + num.ToString());
        DateTime v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("GuerrillaCampData_StartDate_" + num.ToString()), out v);
        StartDate = v;

        //  티켓 충전 남은 시간
        v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("GuerrillaCampData_EndDate_" + num.ToString()), out v);
        EndDate = v;
    }
}
//9998
public partial class GuerrillaMissionData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetString("GuerrillaMissionData_Type_" + num.ToString(), Type);
        PlayerPrefs.SetInt("GuerrillaMissionData_Name_" + num.ToString(), Name);
        PlayerPrefs.SetInt("GuerrillaMissionData_Desc_" + num.ToString(), Desc);
        PlayerPrefs.SetInt("GuerrillaMissionData_Condition_" + num.ToString(), Condition);
        PlayerPrefs.SetInt("GuerrillaMissionData_Count_" + num.ToString(), Count);
        PlayerPrefs.SetInt("GuerrillaMissionData_GroupID_" + num.ToString(), GroupID);
        PlayerPrefs.SetInt("GuerrillaMissionData_GroupOrder_" + num.ToString(), GroupOrder);
        PlayerPrefs.SetInt("GuerrillaMissionData_RewardType_" + num.ToString(), RewardType);
        PlayerPrefs.SetInt("GuerrillaMissionData_RewardIndex_" + num.ToString(), RewardIndex);
        PlayerPrefs.SetInt("GuerrillaMissionData_RewardValue_" + num.ToString(), RewardValue);
        PlayerPrefs.SetString("GuerrillaMissionData_StartDate_" + num.ToString(), StartDate.ToString());
        PlayerPrefs.SetString("GuerrillaMissionData_EndDate_" + num.ToString(), EndDate.ToString());
    }
    public void LoadData(int num)
    {
        Condition = PlayerPrefs.GetInt("GuerrillaMissionData_Condition_" + num.ToString());
        Type = PlayerPrefs.GetString("GuerrillaMissionData_Type_" + num.ToString(), Type);
        Name = PlayerPrefs.GetInt("GuerrillaMissionData_Name_" + num.ToString());
        Desc = PlayerPrefs.GetInt("GuerrillaMissionData_Desc_" + num.ToString());
        Condition = PlayerPrefs.GetInt("GuerrillaMissionData_Condition_" + num.ToString());
        Count = PlayerPrefs.GetInt("GuerrillaMissionData_Count_" + num.ToString());
        GroupID = PlayerPrefs.GetInt("GuerrillaMissionData_GroupID_" + num.ToString());
        GroupOrder = PlayerPrefs.GetInt("GuerrillaMissionData_GroupOrder_" + num.ToString());
        RewardType = PlayerPrefs.GetInt("GuerrillaMissionData_RewardType_" + num.ToString());
        RewardIndex = PlayerPrefs.GetInt("GuerrillaMissionData_RewardIndex_" + num.ToString());
        RewardValue = PlayerPrefs.GetInt("GuerrillaMissionData_RewardValue_" + num.ToString());

        DateTime v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("GuerrillaMissionData_StartDate_" + num.ToString()), out v);
        StartDate = v;

        v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("GuerrillaMissionData_EndDate_" + num.ToString()), out v);
        EndDate = v;
    }
}


public partial class ServerData
{
    public void SaveData()
    {
        PlayerPrefs.SetInt("ServerData_Version", Version);
        PlayerPrefs.SetInt("ServerData_GuerrillaCampCount", GuerrillaCampList.Count);
        PlayerPrefs.SetInt("ServerData_GuerrillaMissionCount", GuerrillaMissionList.Count);
        PlayerPrefs.SetInt("ServerData_GachaCategoryCount", GachaCategoryList.Count);
        PlayerPrefs.SetInt("ServerData_StoreSaleCount", StoreSaleList.Count);
        PlayerPrefs.SetInt("ServerData_BannerCount", BannerList.Count);

        for (int i = 0; i < GuerrillaCampList.Count; i++)
            GuerrillaCampList[i].SaveData(i);
        for (int i = 0; i < GuerrillaMissionList.Count; i++)
            GuerrillaMissionList[i].SaveData(i);        
        for (int i = 0; i < GachaCategoryList.Count; i++)
            GachaCategoryList[i].SaveData(i);
        for (int i = 0; i < StoreSaleList.Count; i++)
            StoreSaleList[i].SaveData(i);
        for (int i = 0; i < BannerList.Count; i++)
            BannerList[i].SaveData(i);
    }
    public void LoadData()
    {
        Version = PlayerPrefs.GetInt("ServerData_Version");
        int GuerrillaCampCount = PlayerPrefs.GetInt("ServerData_GuerrillaCampCount", GuerrillaCampList.Count);
        int GuerrillaMissionCount = PlayerPrefs.GetInt("ServerData_GuerrillaMissionCount", GuerrillaMissionList.Count);
        int GachaCategoryCount = PlayerPrefs.GetInt("ServerData_GachaCategoryCount", GachaCategoryList.Count);
        int StoreSaleCount = PlayerPrefs.GetInt("ServerData_StoreSaleCount", StoreSaleList.Count);
        int BannerCount = PlayerPrefs.GetInt("ServerData_BannerCount", BannerList.Count);

        GuerrillaCampList.Clear();
        GuerrillaMissionList.Clear();
        GachaCategoryList.Clear();
        StoreSaleList.Clear();
        BannerList.Clear();

        for (int i = 0; i < GuerrillaCampCount; i++)
            GuerrillaCampList.Add(new GuerrillaCampData());
        for (int i = 0; i < GuerrillaMissionCount; i++)
            GuerrillaMissionList.Add(new GuerrillaMissionData());
        for (int i = 0; i < GachaCategoryCount; i++)
            GachaCategoryList.Add(new GachaCategoryData());
        for (int i = 0; i < StoreSaleCount; i++)
            StoreSaleList.Add(new StoreSaleData());
        for (int i = 0; i < BannerCount; i++)
            BannerList.Add(new BannerData());

        for (int i = 0; i < GuerrillaCampCount; i++)
            GuerrillaCampList[i].LoadData(i);
        for (int i = 0; i < GuerrillaMissionCount; i++)
            GuerrillaMissionList[i].LoadData(i);
        for (int i = 0; i < GachaCategoryCount; i++)
            GachaCategoryList[i].LoadData(i);
        for (int i = 0; i < StoreSaleCount; i++)
            StoreSaleList[i].LoadData(i);
        for (int i = 0; i < BannerCount; i++)
            BannerList[i].LoadData(i);
    }
}

public partial class UserData
{
    public void SaveData()
    {
        PlayerPrefs.SetInt("UserData_PlatformType", PlatformType);
        PlayerPrefs.SetString("UserData_PlatformID", PlatformID);
        PlayerPrefs.SetString("UserData_PID", PID);
        PlayerPrefs.SetString("UserData_NickName", mNickName);
        PlayerPrefs.SetInt("UserData_UUID", (int)UUID);
        PlayerPrefs.SetInt("UserData_MainCharUID", (int)MainCharUID);
        PlayerPrefs.SetInt("UserData_RoomThemeSlot", RoomThemeSlot);
        PlayerPrefs.SetInt("UserData_Level", Level);
        PlayerPrefs.SetInt("UserData_Exp", Exp);
        for (int i = 0; i < (int)eGOODSTYPE.COUNT; i++)
            PlayerPrefs.SetInt("UserData_Goods" + i.ToString(), (int)Goods[i]);
        PlayerPrefs.SetInt("UserData_HardCash", (int)HardCash);
        PlayerPrefs.SetString("UserData_UserWord", UserWord);
        PlayerPrefs.SetInt("UserData_UserMarkID", UserMarkID);
        PlayerPrefs.SetInt("UserData_ItemSlotCnt", ItemSlotCnt);
        PlayerPrefs.SetInt("UserData_LoginTotalCount", LoginTotalCount);
        PlayerPrefs.SetInt("UserData_LoginContinuityCount", LoginContinuityCount);
        PlayerPrefs.SetInt("UserData_TutorialNum", TutorialNum);
        PlayerPrefs.SetInt("UserData_TutorialFlag", (int)TutorialFlag);
        
        PlayerPrefs.SetInt("UserData_LoginBonusGroupID", (int)LoginBonusGroupID);
        PlayerPrefs.SetInt("UserData_LoginBonusGroupCnt", LoginBonusGroupCnt);
        PlayerPrefs.SetString("UserData_LoginBonusRecentDate", LoginBonusRecentDate.ToString());

        PlayerPrefs.SetString("UserData_APRemainTime", APRemainTime.ToString());
        PlayerPrefs.SetString("UserData_BPRemainTime", BPRemainTime.ToString());

        PlayerPrefs.SetString("UserData_LastPlaySpecialModeTime", LastPlaySpecialModeTime.ToString());
        PlayerPrefs.SetInt("UserData_NextPlaySpecialModeTableID", NextPlaySpecialModeTableID);

        //  아레나 프롤로그 게이지
        PlayerPrefs.SetInt("UserData_ArenaPrologueValue", ArenaPrologueValue);
        //       PlayerPrefs.SetString("UserData_ExpMissionRemainTime", ExpMissionRemainTime.ToString());
        //       PlayerPrefs.SetString("UserData_GoodsMissionRemainTime", GoodsMissionRemainTime.ToString());

    }
    public void LoadData()
    {
        PlatformType = PlayerPrefs.GetInt("UserData_PlatformType");
        PlatformID = PlayerPrefs.GetString("UserData_PlatformID");
        PID = PlayerPrefs.GetString("UserData_PID");
        mNickName = PlayerPrefs.GetString("UserData_NickName");
        UUID = (long)PlayerPrefs.GetInt("UserData_UUID");
        MainCharUID = PlayerPrefs.GetInt("UserData_MainCharUID");
        RoomThemeSlot = PlayerPrefs.GetInt("UserData_RoomThemeSlot");
        Level = PlayerPrefs.GetInt("UserData_Level");
        Exp = PlayerPrefs.GetInt("UserData_Exp");
        for (int i = 0; i < (int)eGOODSTYPE.COUNT; i++)
            Goods[i] = (long)PlayerPrefs.GetInt("UserData_Goods" + i.ToString());
        HardCash = (long)PlayerPrefs.GetInt("UserData_HardCash");

        UserWord = PlayerPrefs.GetString("UserData_UserWord");
        UserMarkID = PlayerPrefs.GetInt("UserData_UserMarkID");
        ItemSlotCnt = PlayerPrefs.GetInt("UserData_ItemSlotCnt");
        LoginTotalCount = PlayerPrefs.GetInt("UserData_LoginTotalCount");
        LoginContinuityCount = PlayerPrefs.GetInt("UserData_LoginContinuityCount");
        TutorialNum = PlayerPrefs.GetInt("UserData_TutorialNum");
        TutorialFlag = PlayerPrefs.GetInt("UserData_TutorialFlag");

        LoginBonusGroupID = PlayerPrefs.GetInt("UserData_LoginBonusGroupID");
        LoginBonusGroupCnt = PlayerPrefs.GetInt("UserData_LoginBonusGroupCnt");

        //  마지막 로그인 보너스 시간
        DateTime v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("UserData_LoginBonusRecentDate"), out v);
        LoginBonusRecentDate = v;

        //  티켓 충전 남은 시간
        v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("UserData_APRemainTime"), out v);
        APRemainTime = v;

        v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("UserData_BPRemainTime"), out v);
        BPRemainTime = v;

        //  스페셜모드 마지막 타임 및 스테이지 번호
        v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("UserData_LastPlaySpecialModeTime"), out v);
        LastPlaySpecialModeTime = v;
        NextPlaySpecialModeTableID = PlayerPrefs.GetInt("UserData_NextPlaySpecialModeTableID");

        //  아레나 프롤로그 게이지
        ArenaPrologueValue =  PlayerPrefs.GetInt("UserData_ArenaPrologueValue");
        /*
        v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("UserData_ExpMissionRemainTime"), out v);
        ExpMissionRemainTime = v;

        v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("UserData_GoodsMissionRemainTime"), out v);
        GoodsMissionRemainTime = v;
        */

        ArrLobbyBgCharUid = new long[GameInfo.Instance.GameConfig.MaxLobbyBgCharCount];
    }
}

public partial class StoreData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("StoreData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("StoreData_TypeVal_" + num.ToString(), (int)TypeVal);
    }
    public void LoadData(int num)
    {
        TableID = PlayerPrefs.GetInt("StoreData_TableID_" + num.ToString());
        TypeVal = (long)PlayerPrefs.GetInt("StoreData_TypeVal_" + num.ToString());
    }
}

public partial class AchieveData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("AchieveData_GroupID_" + num.ToString(), GroupID);
        PlayerPrefs.SetInt("AchieveData_GroupOrder_" + num.ToString(), GroupOrder);
        PlayerPrefs.SetInt("AchieveData_Value_" + num.ToString(), Value);
    }
    public void LoadData(int num)
    {
        GroupID = PlayerPrefs.GetInt("AchieveData_GroupID_" + num.ToString());
        GroupOrder = PlayerPrefs.GetInt("AchieveData_GroupOrder_" + num.ToString());
        Value = PlayerPrefs.GetInt("AchieveData_Value_" + num.ToString());
        SetTable();
    }
}

public partial class CharData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("CharData_UID_" + num.ToString(), (int)CUID);
        PlayerPrefs.SetInt("CharData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("CharData_Grade_" + num.ToString(), Grade);
        PlayerPrefs.SetInt("CharData_Level_" + num.ToString(), Level);
        PlayerPrefs.SetInt("CharData_Exp_" + num.ToString(), Exp);
        PlayerPrefs.SetInt("CharData_PassviePoint_" + num.ToString(), PassviePoint);
        PlayerPrefs.SetInt("CharData_EquipCostumeID_" + num.ToString(), EquipCostumeID);
        PlayerPrefs.SetInt("CharData_EquipWeaponUID_" + num.ToString(), (int)EquipWeaponUID);
        PlayerPrefs.SetInt("CharData_EquipWeapon2UID_" + num.ToString(), (int)EquipWeapon2UID);

        //SkinID
        PlayerPrefs.SetInt("CharData_EquipWeaponSkinTID_" + num.ToString(), (int)EquipWeaponSkinTID);
        PlayerPrefs.SetInt("CharData_EquipWeapon2SkinTID_" + num.ToString(), (int)EquipWeapon2SkinTID);

        PlayerPrefs.SetInt("CharData_CostumeStateFlag_" + num.ToString(), CostumeStateFlag);
        PlayerPrefs.SetInt("CharData_CostumeColor_" + num.ToString(), CostumeColor);
        
            
        PlayerPrefs.SetInt("CharData_PassvieLevelCount_" + num.ToString(), PassvieList.Count);


        for (int i = 0; i < PassvieList.Count; i++)
            PlayerPrefs.SetInt("CharData_PassvieIDList_" + num.ToString() + "_" + i.ToString(), PassvieList[i].SkillID);
        for (int i = 0; i < PassvieList.Count; i++)
            PlayerPrefs.SetInt("CharData_PassvieLevelList_" + num.ToString() + "_" + i.ToString(), PassvieList[i].SkillLevel);

        for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
            PlayerPrefs.SetInt("CharData_EquipSkill_" + num.ToString() + "_" + i.ToString(), EquipSkill[i]);

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
            PlayerPrefs.SetInt("CharData_CardSkill_" + num.ToString() + "_" + i.ToString(), (int)EquipCard[i]);
    }

    public void LoadData(int num)
    {
        CUID = (long)PlayerPrefs.GetInt("CharData_UID_" + num.ToString());
        TableID = PlayerPrefs.GetInt("CharData_TableID_" + num.ToString());
        Grade = PlayerPrefs.GetInt("CharData_Grade_" + num.ToString());
        Level = PlayerPrefs.GetInt("CharData_Level_" + num.ToString());
        Exp = PlayerPrefs.GetInt("CharData_Exp_" + num.ToString());
        PassviePoint = PlayerPrefs.GetInt("CharData_PassviePoint_" + num.ToString());
        EquipCostumeID = PlayerPrefs.GetInt("CharData_EquipCostumeID_" + num.ToString());

        EquipWeaponUID = PlayerPrefs.GetInt("CharData_EquipWeaponUID_" + num.ToString());
        EquipWeapon2UID = PlayerPrefs.GetInt("CharData_EquipWeapon2UID_" + num.ToString());

        //SkinID
        EquipWeaponSkinTID = PlayerPrefs.GetInt("CharData_EquipWeaponSkinTID_" + num.ToString());
        EquipWeapon2SkinTID = PlayerPrefs.GetInt("CharData_EquipWeapon2SkinTID_" + num.ToString());

        CostumeStateFlag = PlayerPrefs.GetInt("CharData_CostumeStateFlag_" + num.ToString());
        CostumeColor = PlayerPrefs.GetInt("CharData_CostumeColor_" + num.ToString());

        int PassvieLevelCount = PlayerPrefs.GetInt("CharData_PassvieLevelCount_" + num.ToString());
        PassvieList.Clear();

        for (int i = 0; i < PassvieLevelCount; i++)
            PassvieList.Add(new PassiveData(0, 0));
        for (int i = 0; i < PassvieLevelCount; i++)
            PassvieList[i].SkillID = PlayerPrefs.GetInt("CharData_PassvieIDList_" + num.ToString() + "_" + i.ToString());
        for (int i = 0; i < PassvieLevelCount; i++)
            PassvieList[i].SkillLevel = PlayerPrefs.GetInt("CharData_PassvieLevelList_" + num.ToString() + "_" + i.ToString());
        for (int i = 0; i < PassvieLevelCount; i++)
            PassvieList[i].Init();

        for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
            EquipSkill[i] = PlayerPrefs.GetInt("CharData_EquipSkill_" + num.ToString() + "_" + i.ToString());

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
            EquipCard[i] = PlayerPrefs.GetInt("CharData_CardSkill_" + num.ToString() + "_" + i.ToString());


        TableData = GameInfo.Instance.GameTable.FindCharacter(x => x.ID == TableID);

    }
}

public partial class WeaponData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("WeaponData_UID_" + num.ToString(), (int)WeaponUID);
        PlayerPrefs.SetInt("WeaponData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("WeaponData_Level_" + num.ToString(), Level);
        PlayerPrefs.SetInt("WeaponData_Exp_" + num.ToString(), Exp);
        PlayerPrefs.SetInt("WeaponData_Wake_" + num.ToString(), Wake);
        PlayerPrefs.SetInt("WeaponData_SkillLv_" + num.ToString(), SkillLv);
        
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
            PlayerPrefs.SetInt("WeaponData_SlotGemUID_" + num.ToString() + "_" + i.ToString(), (int)SlotGemUID[i]);

        PlayerPrefs.SetString("WeaponData_Lock_" + num.ToString(), Lock.ToString());
    }

    public void LoadData(int num)
    {
        WeaponUID = PlayerPrefs.GetInt("WeaponData_UID_" + num.ToString());
        TableID = PlayerPrefs.GetInt("WeaponData_TableID_" + num.ToString());
        Level = PlayerPrefs.GetInt("WeaponData_Level_" + num.ToString());
        Exp = PlayerPrefs.GetInt("WeaponData_Exp_" + num.ToString());
        Wake = PlayerPrefs.GetInt("WeaponData_Wake_" + num.ToString());
        SkillLv = PlayerPrefs.GetInt("WeaponData_SkillLv_" + num.ToString());

        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
            SlotGemUID[i] = PlayerPrefs.GetInt("WeaponData_SlotGemUID_" + num.ToString() + "_" + i.ToString());

        if (PlayerPrefs.GetString("WeaponData_Lock_" + num.ToString()).ToLower() == "true")
            Lock = true;
        else
            Lock = false;

        TableData = GameInfo.Instance.GameTable.FindWeapon(x => x.ID == TableID);
        AddCharacterId();
    }
}

public partial class GemData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("GemData_UID_" + num.ToString(), (int)GemUID);
        PlayerPrefs.SetInt("GemData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("GemData_Level_" + num.ToString(), Level);
        PlayerPrefs.SetInt("GemData_Exp_" + num.ToString(), Exp);
        PlayerPrefs.SetInt("GemData_Wake_" + num.ToString(), Wake);
        PlayerPrefs.SetInt("GemData_TempOptIndex_" + num.ToString(), TempOptIndex);
        PlayerPrefs.SetInt("GemData_TempOptID_" + num.ToString(), TempOptID);
        PlayerPrefs.SetInt("GemData_TempOptValue_" + num.ToString(), TempOptValue);

        for (int i = 0; i < (int)eCOUNT.GEMRANDOPT; i++)
            PlayerPrefs.SetInt("GemData_RandOptID_" + num.ToString() + "_" + i.ToString(), RandOptID[i]);
        for (int i = 0; i < (int)eCOUNT.GEMRANDOPT; i++)
            PlayerPrefs.SetInt("GemData_RandOptValue_" + num.ToString() + "_" + i.ToString(), RandOptValue[i]);

        PlayerPrefs.SetString("GemData_Lock_" + num.ToString(), Lock.ToString());
    }

    public void LoadData(int num)
    {
        GemUID = PlayerPrefs.GetInt("GemData_UID_" + num.ToString());
        TableID = PlayerPrefs.GetInt("GemData_TableID_" + num.ToString());
        Level = PlayerPrefs.GetInt("GemData_Level_" + num.ToString());
        Exp = PlayerPrefs.GetInt("GemData_Exp_" + num.ToString());
        Wake = PlayerPrefs.GetInt("GemData_Wake_" + num.ToString());
        TempOptIndex = PlayerPrefs.GetInt("GemData_TempOptIndex_" + num.ToString());
        TempOptID = PlayerPrefs.GetInt("GemData_TempOptID_" + num.ToString());
        TempOptValue = PlayerPrefs.GetInt("GemData_TempOptValue_" + num.ToString());


        for (int i = 0; i < (int)eCOUNT.GEMRANDOPT; i++)
            RandOptID[i] = PlayerPrefs.GetInt("GemData_RandOptID_" + num.ToString() + "_" + i.ToString());
        for (int i = 0; i < (int)eCOUNT.GEMRANDOPT; i++)
            RandOptValue[i] = PlayerPrefs.GetInt("GemData_RandOptValue_" + num.ToString() + "_" + i.ToString());

        if (PlayerPrefs.GetString("GemData_Lock_" + num.ToString()).ToLower() == "true")
            Lock = true;
        else
            Lock = false;

        TableData = GameInfo.Instance.GameTable.FindGem(x => x.ID == TableID);
    }
}


public partial class CardData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("CardData_UID_" + num.ToString(), (int)CardUID);
        PlayerPrefs.SetInt("CardData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("CardData_Level_" + num.ToString(), Level);
        PlayerPrefs.SetInt("CardData_Exp_" + num.ToString(), Exp);
        PlayerPrefs.SetInt("CardData_Wake_" + num.ToString(), Wake);
        PlayerPrefs.SetInt("CardData_SkillLv_" + num.ToString(), SkillLv);
        PlayerPrefs.SetString("CardData_Lock_" + num.ToString(), Lock.ToString());
    }
    public void LoadData(int num)
    {
        CardUID = PlayerPrefs.GetInt("CardData_UID_" + num.ToString());
        TableID = PlayerPrefs.GetInt("CardData_TableID_" + num.ToString());
        Level = PlayerPrefs.GetInt("CardData_Level_" + num.ToString());
        Exp = PlayerPrefs.GetInt("CardData_Exp_" + num.ToString());
        Wake = PlayerPrefs.GetInt("CardData_Wake_" + num.ToString());
        SkillLv = PlayerPrefs.GetInt("CardData_SkillLv_" + num.ToString());
        if (PlayerPrefs.GetString("CardData_Lock_" + num.ToString()).ToLower() == "true")
            Lock = true;
        else
            Lock = false;
        TableData = GameInfo.Instance.GameTable.FindCard(x => x.ID == TableID);
    }
}



public partial class ItemData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("ItemData_UID_" + num.ToString(), (int)ItemUID);
        PlayerPrefs.SetInt("ItemData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("ItemData_Count_" + num.ToString(), Count);

    }
    public void LoadData(int num)
    {
        ItemUID = PlayerPrefs.GetInt("ItemData_UID_" + num.ToString());
        TableID = PlayerPrefs.GetInt("ItemData_TableID_" + num.ToString());
        Count = PlayerPrefs.GetInt("ItemData_Count_" + num.ToString());
        TableData = GameInfo.Instance.GameTable.FindItem(x => x.ID == TableID);
    }

}


public partial class FacilityData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("FacilityData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("FacilityData_Level_" + num.ToString(), Level);
        PlayerPrefs.SetInt("FacilityData_Stats_" + num.ToString(), Stats);
        PlayerPrefs.SetInt("FacilityData_EquipCardUID_" + num.ToString(), (int)EquipCardUID);
        PlayerPrefs.SetInt("FacilityData_Selete_" + num.ToString(), (int)Selete);
        PlayerPrefs.SetString("FacilityData_RemainTime_" + num.ToString(), RemainTime.ToString());
        PlayerPrefs.SetInt("FacilityData_OperationCnt_" + num.ToString(), (int)OperationCnt);

    }
    public void LoadData(int num)
    {
        TableID = PlayerPrefs.GetInt("FacilityData_TableID_" + num.ToString());
        Level = PlayerPrefs.GetInt("FacilityData_Level_" + num.ToString());
        Stats = PlayerPrefs.GetInt("FacilityData_Stats_" + num.ToString());
        EquipCardUID = PlayerPrefs.GetInt("FacilityData_EquipCardUID_" + num.ToString());
        Selete = PlayerPrefs.GetInt("FacilityData_Selete_" + num.ToString());

        DateTime v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("FacilityData_RemainTime_") + num.ToString(), out v);
        RemainTime = v;
        TableData = GameInfo.Instance.GameTable.FindFacility(x => x.ID == TableID);

        OperationCnt = PlayerPrefs.GetInt("FacilityData_OperationCnt_" + num.ToString());
    }
}

public partial class RoomThemeFuncData
{
    public void SaveData(int i, int num)
    {
        PlayerPrefs.SetInt("RoomThemeFuncData_TableID_" + i.ToString() + "_" + num.ToString(), TableID);
        PlayerPrefs.SetString("RoomThemeFuncData_On_" + num.ToString(), On.ToString());
    }

    public void LoadData(int i, int num)
    {
        TableID = PlayerPrefs.GetInt("RoomThemeFuncData_TableID_" + i.ToString() + "_" + num.ToString());
        if (PlayerPrefs.GetString("RoomThemeFuncData_On_" + num.ToString()).ToLower() == "true")
            On = true;
        else
            On = false;
        TableData = GameInfo.Instance.GameTable.FindRoomFunc(TableID);
    }

}
public partial class RoomThemeSlotData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("RoomThemeSlotData_SlotNum_" + num.ToString(), SlotNum);
        PlayerPrefs.SetInt("RoomThemeSlotData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("RoomThemeSlotData_RoomThemeFuncListCount_" + num.ToString(), RoomThemeFuncList.Count);
        for (int i = 0; i < RoomThemeFuncList.Count; i++)
            RoomThemeFuncList[i].SaveData(num,i);

        if (ArrLightInfo != null && ArrLightInfo.Length > 0)
        {
            string str = Convert.ToBase64String(ArrLightInfo);
            PlayerPrefs.SetString("RoomThemeSlotData_LightInfo_" + num.ToString(), str);
        }
    }
    public void LoadData(int num)
    {
        SlotNum = PlayerPrefs.GetInt("RoomThemeSlotData_SlotNum_" + num.ToString());
        TableID = PlayerPrefs.GetInt("RoomThemeSlotData_TableID_" + num.ToString());

        int count = PlayerPrefs.GetInt("RoomThemeSlotData_RoomThemeFuncListCount_" + num.ToString());
        RoomThemeFuncList.Clear();

        for (int i = 0; i < count; i++)
            RoomThemeFuncList.Add(new RoomThemeFuncData());

        for (int i = 0; i < count; i++)
            RoomThemeFuncList[i].LoadData(num, i);

        TableData = GameInfo.Instance.GameTable.FindRoomTheme(TableID);

        string str = PlayerPrefs.GetString("RoomThemeSlotData_LightInfo_" + num.ToString());
        if (!string.IsNullOrEmpty(str))
        {
            ArrLightInfo = Convert.FromBase64String(str);
        }
    }
}

public partial class RoomThemeFigureSlotData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("RoomThemeFigureSlotData_SlotNum_" + num.ToString(), SlotNum);
        PlayerPrefs.SetInt("RoomThemeFigureSlotData_RoomThemeSlotNum_" + num.ToString(), RoomThemeSlotNum);
        PlayerPrefs.SetInt("RoomThemeFigureSlotData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("RoomThemeFigureSlotData_Action1_" + num.ToString(), Action1);
        //PlayerPrefs.SetInt("RoomThemeFigureSlotData_Action2_" + num.ToString(), Action2);
        PlayerPrefs.SetInt("RoomThemeFigureSlotData_CostumeStateFlag_" + num.ToString(), CostumeStateFlag);
        PlayerPrefs.SetInt("RoomThemeFigureSlotData_CostumeColor_" + num.ToString(), CostumeColor);

        //77 혁팀장 detailattr 저장
        string str = System.Convert.ToBase64String(detailarry);
        PlayerPrefs.SetString("RoomThemeFigureSlotData_DetailArr_" + num.ToString(), str);
    }
    public void LoadData(int num)
    {
        SlotNum = PlayerPrefs.GetInt("RoomThemeFigureSlotData_SlotNum_" + num.ToString());
        RoomThemeSlotNum = PlayerPrefs.GetInt("RoomThemeFigureSlotData_RoomThemeSlotNum_" + num.ToString());
        TableID = PlayerPrefs.GetInt("RoomThemeFigureSlotData_TableID_" + num.ToString());
        Action1 = PlayerPrefs.GetInt("RoomThemeFigureSlotData_Action1_" + num.ToString());
        //Action2 = PlayerPrefs.GetInt("RoomThemeFigureSlotData_Action2_" + num.ToString());
        CostumeStateFlag = PlayerPrefs.GetInt("RoomThemeFigureSlotData_CostumeStateFlag_" + num.ToString());
        CostumeColor = PlayerPrefs.GetInt("RoomThemeFigureSlotData_CostumeColor_" + num.ToString());

        //77 혁팀장 detailattr 로드
        string str = PlayerPrefs.GetString("RoomThemeFigureSlotData_DetailArr_" + num.ToString());
        detailarry = System.Convert.FromBase64String(str);

        TableData = GameInfo.Instance.GameTable.FindRoomFigure(TableID);
    }
}


public partial class StageClearData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("StageClearData_TableID_" + num.ToString(), TableID);

        for (int i = 0; i < (int)eCOUNT.STAGEMISSION; i++)
            PlayerPrefs.SetInt("StageClearData_Mission_" + num.ToString() + "_" + i.ToString(), Mission[i]);
    }
    public void LoadData(int num)
    {
        TableID = PlayerPrefs.GetInt("StageClearData_TableID_" + num.ToString());

        for (int i = 0; i < (int)eCOUNT.STAGEMISSION; i++)
            Mission[i] = PlayerPrefs.GetInt("StageClearData_Mission_" + num.ToString() + "_" + i.ToString());

        TableData = GameInfo.Instance.GameTable.FindStage(TableID);
    }
}

public partial class TimeAttackClearData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("TimeAttackClearData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("TimeAttackClearData_HighestScore_" + num.ToString(), HighestScore);
        PlayerPrefs.SetInt("TimeAttackClearData_CharCUID_" + num.ToString(), (int)CharCUID);
        PlayerPrefs.SetString("TimeAttackClearData_HighestScoreRemainTime_" + num.ToString(), HighestScoreRemainTime.ToString());
    }
    public void LoadData(int num)
    {
        TableID = PlayerPrefs.GetInt("TimeAttackClearData_TableID_" + num.ToString());
        HighestScore = PlayerPrefs.GetInt("TimeAttackClearData_HighestScore_" + num.ToString());
        CharCUID = (int)PlayerPrefs.GetInt("TimeAttackClearData_CharCUID_" + num.ToString());
        DateTime v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("TimeAttackClearData_HighestScoreRemainTime_" + num.ToString()), out v);
        HighestScoreRemainTime = v;
        TableData = GameInfo.Instance.GameTable.FindStage(TableID);
    }
}

public partial class TimeAttackRankUserData
{
    public void SaveData(int tableid, int num)
    {
        PlayerPrefs.SetInt("TimeAttackRankUserData_TableID_" + (tableid * 100).ToString() + "_" + num.ToString(), Rank);
        PlayerPrefs.SetInt("TimeAttackRankUserData_HighestScore_" + (tableid * 100).ToString() + "_" + num.ToString(), HighestScore);
        PlayerPrefs.SetString("TimeAttackRankUserData_UserNickName_" + (tableid * 100).ToString() + "_" + num.ToString(), mUserNickName.ToString());
        PlayerPrefs.SetInt("TimeAttackRankUserData_UserMark_" + (tableid * 100).ToString() + "_" + num.ToString(), UserMark);
        PlayerPrefs.SetInt("TimeAttackRankUserData_UserRank_" + (tableid * 100).ToString() + "_" + num.ToString(), UserRank);
        PlayerPrefs.SetInt("TimeAttackRankUserData_CharData_Is_" + (tableid * 100).ToString() + "_" + num.ToString(), CharData == null ? 0 : 1 );
        PlayerPrefs.SetInt("TimeAttackRankUserData_WeaponData_Is_" + (tableid * 100).ToString() + "_" + num.ToString(), WeaponData == null ? 0 : 1);
        PlayerPrefs.SetInt("TimeAttackRankUserData_CardData_Count_" + (tableid * 100).ToString() + "_" + num.ToString(), CardList.Count);
        
        //PlayerPrefs.SetInt("TimeAttackRankUserData_CardData_Is_" + (tableid * 100).ToString() + "_" + num.ToString(), CardData == null ? 0 : 1);

        if (CharData != null)
            CharData.SaveData((tableid * 100) + num);
        if (WeaponData != null)
            WeaponData.SaveData((tableid * 100) + num);

        for( int i = 0; i < CardList.Count; i++ )
            CardList[i].SaveData((tableid * 100) + num + i);
    }
    public void LoadData(int tableid, int num)
    {
        Rank = PlayerPrefs.GetInt("TimeAttackRankUserData_TableID_" + (tableid * 100).ToString() + "_" + num.ToString() );
        HighestScore = PlayerPrefs.GetInt("TimeAttackRankUserData_HighestScore_" + (tableid * 100).ToString() + "_" + num.ToString() );
        mUserNickName = PlayerPrefs.GetString("TimeAttackRankUserData_UserNickName_" + (tableid * 100).ToString() + "_" + num.ToString());
        UserMark = PlayerPrefs.GetInt("TimeAttackRankUserData_UserMark_" + (tableid * 100).ToString() + "_" + num.ToString() );
        UserRank = PlayerPrefs.GetInt("TimeAttackRankUserData_UserRank_" + (tableid * 100).ToString() + "_" + num.ToString() );
        int ischardata = PlayerPrefs.GetInt("TimeAttackRankUserData_CharData_Is_" + (tableid * 100).ToString() + "_" + num.ToString());
        int isweapondata = PlayerPrefs.GetInt("TimeAttackRankUserData_WeaponData_Is_" + (tableid * 100).ToString() + "_" + num.ToString());
        int cardlistcount = PlayerPrefs.GetInt("TimeAttackRankUserData_CardData_Count_" + (tableid * 100).ToString() + "_" + num.ToString());
       
        if (ischardata == 1)
        {
            CharData = new CharData();
            CharData.LoadData((tableid * 100) + num);
        }
        if (isweapondata == 1)
        {
            WeaponData = new WeaponData();
            WeaponData.LoadData((tableid * 100) + num);
        }

        CardList.Clear();
        for ( int i = 0; i < cardlistcount; i++ )
        {
            var CardData = new CardData();
            CardList.Add(CardData);
            CardData.LoadData((tableid * 100) + num + i);
        }

    }
}

public partial class TimeAttackRankData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("TimeAttackRankData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("TimeAttackRankData_RankUserList_Count" + num.ToString(), RankUserList.Count);
        for (int i = 0; i < RankUserList.Count; i++)
            RankUserList[i].SaveData(TableID, i);
    }
    public void LoadData(int num)
    {
        TableID = PlayerPrefs.GetInt("TimeAttackRankData_TableID_" + num.ToString());
        int Count = PlayerPrefs.GetInt("TimeAttackRankData_RankUserList_Count" + num.ToString());
        RankUserList.Clear();
        for ( int i = 0; i < Count; i++)
            RankUserList.Add(new TimeAttackRankUserData());

        for (int i = 0; i < Count; i++)
            RankUserList[i].LoadData(TableID, i);


        TableData = GameInfo.Instance.GameTable.FindStage(TableID);
    }
}


public partial class WeaponBookData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("WeaponBookData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("WeaponBookData_StateFlag_" + num.ToString(), StateFlag);
    }
    public void LoadData(int num)
    {
        TableID = PlayerPrefs.GetInt("WeaponBookData_TableID_" + num.ToString());
        StateFlag = PlayerPrefs.GetInt("WeaponBookData_StateFlag_" + num.ToString());
    }
}


public partial class CardBookData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("CardBookData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("CardBookData_StateFlag_" + num.ToString(), StateFlag);
        PlayerPrefs.SetInt("CardBookData_FavorLevel_" + num.ToString(), FavorLevel);
        PlayerPrefs.SetInt("CardBookData_FavorExp_" + num.ToString(), FavorExp);
    }
    public void LoadData(int num)
    {
        TableID = PlayerPrefs.GetInt("CardBookData_TableID_" + num.ToString());
        StateFlag = PlayerPrefs.GetInt("CardBookData_StateFlag_" + num.ToString());
        FavorLevel = PlayerPrefs.GetInt("CardBookData_FavorLevel_" + num.ToString());
        FavorExp = PlayerPrefs.GetInt("CardBookData_FavorExp_" + num.ToString());
    }
}

public partial class MonsterBookData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("MonsterBookData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("MonsterBookData_StateFlag_" + num.ToString(), StateFlag);
    }
    public void LoadData(int num)
    {
        TableID = PlayerPrefs.GetInt("MonsterBookData_TableID_" + num.ToString());
        StateFlag = PlayerPrefs.GetInt("MonsterBookData_StateFlag_" + num.ToString());
    }
}

public partial class MailData
{

    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("MailData_MailUID_" + num.ToString(), (int)MailUID);             //  우편식별ID
        PlayerPrefs.SetInt("MailData_ProductType_" + num.ToString(), ProductType);          //  상품타입
        PlayerPrefs.SetInt("MailData_ProductIndex_" + num.ToString(), (int)ProductIndex);   //  상품인덱스
        PlayerPrefs.SetInt("MailData_ProductValue_" + num.ToString(), (int)ProductValue);   //  상품수량

        PlayerPrefs.SetString("MailData_RemainTime_" + num.ToString(), RemainTime.ToString());   //  삭제예정시간(남은시간 60초 1틱)

        PlayerPrefs.SetInt("MailData_MailType_" + num.ToString(), (int)MailType);        //  우편타입ID          
        PlayerPrefs.SetString("MailData_MailTypeValue_" + num.ToString(), MailTypeValue);   //  우편타입값
    }

    public void LoadData(int num)
    {
        MailUID = (ulong)PlayerPrefs.GetInt("MailData_MailUID_" + num.ToString());      //  우편식별ID
        ProductType = PlayerPrefs.GetInt("MailData_ProductType_" + num.ToString());     //  상품타입
        ProductIndex = (uint)PlayerPrefs.GetInt("MailData_ProductIndex_" + num.ToString());    //  상품인덱스
        ProductValue = (uint)PlayerPrefs.GetInt("MailData_ProductValue_" + num.ToString());    //  상품수량

        DateTime v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("MailData_RemainTime_" + num.ToString()), out v);
        RemainTime = v;                                                                   //  삭제예정시간(남은시간 60초 1틱)

        MailType = (eMailType)PlayerPrefs.GetInt("MailData_MailType_" + num.ToString());       //  우편타입ID          
        MailTypeValue = PlayerPrefs.GetString("MailData_MailTypeValue_" + num.ToString());   //  우편타입값

    }
}

public partial class WeekMissionData
{
    public void SaveData()
    {
        PlayerPrefs.SetInt("WeeklyMisstionData_fWeeklyMissionSetID", (int)fWeekMissionSetID);     //  주간 미션 Set ID
        PlayerPrefs.SetInt("WeeklyMisstionData_fMissionRewardFlag", (int)fMissionRewardFlag);       //  주간 미션 보상 획득 비트 플래그

        for (int i = 0; i < (int)eCOUNT.WEEKMISSIONCOUNT; i++)
            PlayerPrefs.SetInt("WeeklyMisstionData_fMissionRemainCntSlot_" + i.ToString(), (int)fMissionRemainCntSlot[i]);  //  주간 미션 슬롯 0 ~ 6 남은 목표 횟수

        PlayerPrefs.SetString("WeeklyMisstionData_fWeeklyMissionResetDate", fWeekMissionResetDate.ToString());   //  주간 미션 Set ID 초기화 예정 시간(남은시간 60초 1틱)
    }

    public void LoadData()
    {
        fWeekMissionSetID = (uint)PlayerPrefs.GetInt("WeeklyMisstionData_fWeeklyMissionSetID");   //  주간 미션 Set ID
        fMissionRewardFlag = (uint)PlayerPrefs.GetInt("WeeklyMisstionData_fMissionRewardFlag");     //  주간 미션 보상 획득 비트 플래그

        for (int i = 0; i < (int)eCOUNT.WEEKMISSIONCOUNT; i++)
            fMissionRemainCntSlot[i] = (uint)PlayerPrefs.GetInt("WeeklyMisstionData_fMissionRemainCntSlot_" + i.ToString());  //  주간 미션 슬롯 0 ~ 6 남은 목표 횟수

        DateTime v = DateTime.MinValue;
        DateTime.TryParse(PlayerPrefs.GetString("WeeklyMisstionData_fWeeklyMissionResetDate"), out v);
        fWeekMissionResetDate = v;

    }
}

    /*
    //로컬 구현 없음
    public partial class GllaMissionData
    {
    
    }
    */

public partial class EventSetData
{
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("EventSetData_TableID_" + num.ToString(), TableID);
        PlayerPrefs.SetInt("EventSetData_RewardStep_" + num.ToString(), RewardStep);
        PlayerPrefs.SetInt("EventSetData_Count_" + num.ToString(), Count);
        PlayerPrefs.SetInt("EventSetData_ItemCount_" + num.ToString(), RewardItemCount.Count);
        for(int i = 0; i < RewardItemCount.Count; i++)
        {
            PlayerPrefs.SetInt("EventSetData_ItemCount_" + num.ToString() + "_" + i.ToString(), RewardItemCount[i]);
        }

        PlayerPrefs.Save();
    }
    public void LoadData(int num)
    {
        TableID = PlayerPrefs.GetInt("EventSetData_TableID_" + num.ToString());
        TableData = GameInfo.Instance.GameTable.FindEventSet(x => x.EventID == TableID);
        RewardStep = PlayerPrefs.GetInt("EventSetData_RewardStep_" + num.ToString(), 1);

        Count = PlayerPrefs.GetInt("EventSetData_Count_" + num.ToString(), RewardStep - 1);

        List<GameTable.EventResetReward.Param> tempRewardList = GameInfo.Instance.GameTable.EventResetRewards.FindAll(x => x.EventID == TableID && x.RewardStep == RewardStep);

        if (RewardItemCount == null)
            RewardItemCount = new List<int>();

        RewardItemCount.Clear();

        for(int i = 0; i < tempRewardList.Count; i++)
        {
            RewardItemCount.Add(PlayerPrefs.GetInt("EventSetData_ItemCount_" + num.ToString() + "_" + i.ToString(), tempRewardList[i].RewardCnt));
        }
    }
}

//public partial class EventResetRewardData
//{
//    public void SaveData(int num)
//    {
//        PlayerPrefs.SetInt("EventResetRewardData_TableID_" + num.ToString(), TableID);
//        //PlayerPrefs.SetInt("EventResetRewardData_RewardCnt_" + num.ToString(), RewardCnt.ToString());
//        PlayerPrefs.SetInt("EventResetRewardData_ResetFlag_" + num.ToString(), ResetFlag ? 1 : 0);

//        for(int i = 0; i < RewardTableData.Count; i++)
//        {
//            PlayerPrefs.SetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_EventID_" + i, RewardTableData[i].EventID);
//            PlayerPrefs.SetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_RewardStep_" + i, RewardTableData[i].RewardStep);
//            PlayerPrefs.SetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_ProductType_" + i, RewardTableData[i].ProductType);
//            PlayerPrefs.SetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_ProductIndex_" + i, RewardTableData[i].ProductIndex);
//            PlayerPrefs.SetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_ProductValue_" + i, RewardTableData[i].ProductValue);
//            PlayerPrefs.SetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_RewardRate_" + i, RewardTableData[i].RewardRate);
//            PlayerPrefs.SetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_RewardCnt_" + i, RewardTableData[i].RewardCnt);
//            PlayerPrefs.SetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_ResetFlag_" + i, RewardTableData[i].ResetFlag);
//        }

//        for (int i = 0; i < OriginRewardTableData.Count; i++)
//        {
//            PlayerPrefs.SetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_EventID_" + i, OriginRewardTableData[i].EventID);
//            PlayerPrefs.SetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_RewardStep_" + i, OriginRewardTableData[i].RewardStep);
//            PlayerPrefs.SetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_ProductType_" + i, OriginRewardTableData[i].ProductType);
//            PlayerPrefs.SetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_ProductIndex_" + i, OriginRewardTableData[i].ProductIndex);
//            PlayerPrefs.SetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_ProductValue_" + i, OriginRewardTableData[i].ProductValue);
//            PlayerPrefs.SetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_RewardRate_" + i, OriginRewardTableData[i].RewardRate);
//            PlayerPrefs.SetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_RewardCnt_" + i, OriginRewardTableData[i].RewardCnt);
//            PlayerPrefs.SetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_ResetFlag_" + i, OriginRewardTableData[i].ResetFlag);
//        }
        
//        TotalRewardCnt = RewardTableData.Count;
//        PlayerPrefs.SetInt("EventResetRewardData_TotalRewardCnt_" + num.ToString(), TotalRewardCnt);
//        PlayerPrefs.SetInt("EventResetRewardData_RewardStep_" + num.ToString(), RewardStep);
//    }

//    public void LoadData(int num)
//    {
//        TableID = PlayerPrefs.GetInt("EventResetRewardData_TableID_" + num.ToString());
//        //RewardCnt = PlayerPrefs.GetInt("EventResetRewardData_RewardCnt_" + num.ToString());
//        ResetFlag = PlayerPrefs.GetInt("EventResetRewardData_ResetFlag_" + num.ToString()) == 1 ? true : false;
//        //TableData = GameInfo.Instance.GameTable.EventResetRewards[num];

//        TotalRewardCnt = PlayerPrefs.GetInt("EventResetRewardData_TotalRewardCnt_" + num.ToString());
//        if (RewardTableData == null)
//            RewardTableData = new List<GameTable.EventResetReward.Param>();

//        RewardTableData.Clear();
//        for (int i = 0; i < TotalRewardCnt; i++)
//        {
//            GameTable.EventResetReward.Param param = new GameTable.EventResetReward.Param();
//            param.EventID       = PlayerPrefs.GetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_EventID_" + i);
//            param.RewardStep    = PlayerPrefs.GetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_RewardStep_" + i);
//            param.ProductType   = PlayerPrefs.GetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_ProductType_" + i);
//            param.ProductIndex  = PlayerPrefs.GetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_ProductIndex_" + i);
//            param.ProductValue  = PlayerPrefs.GetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_ProductValue_" + i);
//            param.RewardRate    = PlayerPrefs.GetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_RewardRate_" + i);
//            param.RewardCnt     = PlayerPrefs.GetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_RewardCnt_" + i);
//            param.ResetFlag     = PlayerPrefs.GetInt("EventResetRewardData_RewardTableData_" + num.ToString() + "_ResetFlag_" + i);

//            RewardTableData.Add(param);
//        }

//        if (OriginRewardTableData == null)
//            OriginRewardTableData = new List<GameTable.EventResetReward.Param>();

//        OriginRewardTableData.Clear();

//        for (int i = 0; i < TotalRewardCnt; i++)
//        {
//            GameTable.EventResetReward.Param param = new GameTable.EventResetReward.Param();
//            param.EventID        = PlayerPrefs.GetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_EventID_" + i);
//            param.RewardStep     = PlayerPrefs.GetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_RewardStep_" + i);
//            param.ProductType    = PlayerPrefs.GetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_ProductType_" + i);
//            param.ProductIndex   = PlayerPrefs.GetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_ProductIndex_" + i);
//            param.ProductValue   = PlayerPrefs.GetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_ProductValue_" + i);
//            param.RewardRate     = PlayerPrefs.GetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_RewardRate_" + i);
//            param.RewardCnt      = PlayerPrefs.GetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_RewardCnt_" + i);
//            param.ResetFlag      = PlayerPrefs.GetInt("EventResetRewardData_OriginRewardTableData_" + num.ToString() + "_ResetFlag_" + i);

//            OriginRewardTableData.Add(param);
//        }

//        RewardStep = PlayerPrefs.GetInt("EventResetRewardData_RewardStep_" + num.ToString());
//    }
//}