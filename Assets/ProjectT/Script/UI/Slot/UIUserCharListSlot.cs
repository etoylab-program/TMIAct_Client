
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIUserCharListSlot : FSlot
{

    public UISprite kBgFriednSpr;
    public UISprite kBGSpr;
    public UISprite kBGMESpr;
    public UISprite kSelSpr;
	public UISprite kGradeSpr;
	public UITexture kIconTex;
	public UILabel kLevelLabel;

    public UISprite kSlotSpr;
    public UILabel kSlotLabel;
    public UISprite kSynastrySpr;

    public UILabel kArenaSlotPosLabel;

    public UISprite SprSelectedFriendChar;
    public UIButton BtnShowFriendCharDetail;

    public UISprite SprLobbyBgCharSlot;

    public GameObject kFavorObj;
    public UILabel kFavorLabel;
    public CharData CharData => _chardata;

    [SerializeField] private GameObject _HpObj;
    [SerializeField] private UISprite   _HpSpr;

    private CharData _chardata;
    private int _index;
    private bool _bCharInfo = false;
    private bool _bRankingUser = false;
    private bool _bEnemyDetail = false;
    private bool _bFacilityOperation = false;
    private long _rankerDetialUUID = 0;

    private bool    mbArenaTowerFriendChar  = false;
    private bool    mbLobbyBgChar           = false;
    private int     mLobbyBgCharSlotIndex   = -1;

    private Action<GameObject> _facilityClickAction = null;
    
    public eCharSelectFlag CharSelectFlag = eCharSelectFlag.ARENA;

    public void UpdateSlot(CharData chardata, bool bEnable = true)     //Fill parameter if you need
    {
        kFavorObj.SetActive(chardata != null && bEnable);

        if( _HpObj ) {
            _HpObj.SetActive( false );
        }

        if (chardata == null)
        {
            kBGSpr.gameObject.SetActive(true);
            kBGMESpr.gameObject.SetActive(false);
            kSelSpr.gameObject.SetActive(false);
            kGradeSpr.gameObject.SetActive(false);
            kIconTex.mainTexture = null;
            kLevelLabel.gameObject.SetActive(false);
            kSlotSpr.gameObject.SetActive(false);
            kSlotLabel.gameObject.SetActive(false);
            kSynastrySpr.gameObject.SetActive(false);
        }
        else
        {
            _bRankingUser = true;
            _chardata = chardata;
            kFavorLabel.textlocalize = chardata.FavorLevel.ToString();
            
            UpdateSlot(chardata.Level, chardata.Grade, chardata.TableID, chardata.EquipCostumeID);
        }
    }
    
    public void UpdateSlot(TimeAttackClearData timeattackcleardata)     //Fill parameter if you need
    {
        var chardata = GameInfo.Instance.GetCharData(timeattackcleardata.CharCUID);
        if (chardata == null)
            return;
        _chardata = chardata;
        kBGSpr.gameObject.SetActive(false);
        kBGMESpr.gameObject.SetActive(true);
        kGradeSpr.gameObject.SetActive(false);
        kLevelLabel.gameObject.SetActive(false);

        if( _HpObj ) {
            _HpObj.SetActive( false );
        }

        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/MainSlot/MainSlot_" + chardata.TableData.Icon + "_" + chardata.TableData.InitCostume.ToString() + ".png");
    }

	public void UpdateSlot( RaidClearData raidClearData ) {
		CharData charData = GameInfo.Instance.GetCharData( raidClearData.Cuid );
        if( charData == null ) {
            return;
        }

		_chardata = charData;

		kBGSpr.gameObject.SetActive( false );
		kBGMESpr.gameObject.SetActive( true );
		kGradeSpr.gameObject.SetActive( false );
		kLevelLabel.gameObject.SetActive( false );

		if( _HpObj ) {
			_HpObj.SetActive( false );
		}

		kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", "Icon/Char/MainSlot/MainSlot_" + charData.TableData.Icon + 
                                                                                          "_" + charData.TableData.InitCostume.ToString() + ".png" );
	}

	public void UpdateSlot( int level, int grade, int chartableid, int equipcostumeid) 	//Fill parameter if you need
	{
        var tabledata = GameInfo.Instance.GameTable.FindCharacter(x => x.ID == chartableid);
        if (tabledata == null)
            return;

        _bRankingUser = false;
        kBGSpr.gameObject.SetActive(true);
        kBGMESpr.gameObject.SetActive(false);
        kGradeSpr.gameObject.SetActive(true);
        kLevelLabel.gameObject.SetActive(true);

        if( _HpObj ) {
            _HpObj.SetActive( false );
        }

        kLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), level);
        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/MainSlot/MainSlot_" + tabledata.Icon + "_" + equipcostumeid.ToString() + ".png");
        kGradeSpr.spriteName = string.Format("grade_{0}", grade.ToString("D2"));  //"grade_0" + grade.ToString();

        //친구 배경 설정
        if (CharSelectFlag != eCharSelectFlag.ARENA)
        {
            bool isFreind = GameSupport.IsFriend(_chardata.CUID);
            kBgFriednSpr?.SetActive(isFreind);
            kBGSpr.SetActive(!isFreind);
        }
    }

	public void UpdateThreePlayerSlot( Player player ) {
        if( player == null ) {
            SetActive( false );
            return;
		}

        GameTable.Character.Param tableData = GameInfo.Instance.GameTable.FindCharacter(x => x.ID == player.tableId );
        if( tableData == null ) {
            return;
        }

        kBGSpr.SetActive( true );
        kBgFriednSpr?.SetActive( false );
        kFavorObj.SetActive( false );
        kBGSpr.gameObject.SetActive( true );
        kBGMESpr.gameObject.SetActive( false );
        kGradeSpr.gameObject.SetActive( true );
        kLevelLabel.gameObject.SetActive( true );

        if( _HpObj ) {
            _HpObj.SetActive( true );
        }

		_chardata = player.charData;
		_bRankingUser = false;
		
		kLevelLabel.textlocalize = string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.LEVEL_TXT_NOW_LV ), _chardata.Level );
		kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", "Icon/Char/MainSlot/MainSlot_" + tableData.Icon + "_" + _chardata.EquipCostumeID.ToString() + ".png" );
		kGradeSpr.spriteName = string.Format( "grade_{0}", _chardata.Grade.ToString( "D2" ) );

        if( _HpSpr ) {
            _HpSpr.fillAmount = player.curHp / player.maxHp;
        }
    }

	//아레나 슬롯
	public void UpdateArenaTeamSlot(int index, CharData chardata, bool viewInfo = false, bool rankInfo = false)
    {
        _index = index;
        _bCharInfo = viewInfo;
        kBGMESpr.gameObject.SetActive(false);
        _bRankingUser = rankInfo;
        _chardata = chardata;

        //배경은 기본으로 설정
        kBgFriednSpr?.SetActive(false);
        kBGSpr.SetActive(true);

        int slotIdx = _index + 1;
        kSlotSpr.gameObject.SetActive(true);
        kSlotLabel.textlocalize = slotIdx.ToString();
        kSynastrySpr.gameObject.SetActive(false);

        kArenaSlotPosLabel.gameObject.SetActive(false);

        if (_bRankingUser)
            kSlotSpr.gameObject.SetActive(false);

        if( _HpObj ) {
            _HpObj.SetActive( false );
        }

        if (_chardata == null || _chardata.TableID == (int)eCOUNT.NONE)
        {
            if(_bCharInfo)
            {
                kArenaSlotPosLabel.gameObject.SetActive(true);
                kArenaSlotPosLabel.textlocalize = FLocalizeString.Instance.GetText(1485 + _index);
            }

            kGradeSpr.gameObject.SetActive(false);
            kIconTex.gameObject.SetActive(false);
            kLevelLabel.gameObject.SetActive(false);
            return;
        }

        kLevelLabel.gameObject.SetActive(true);
        kLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), chardata.Level);
        kIconTex.gameObject.SetActive(true);
        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/MainSlot/MainSlot_" + chardata.TableData.Icon + "_" + chardata.EquipCostumeID.ToString() + ".png");
        kGradeSpr.gameObject.SetActive(true);
        kGradeSpr.spriteName = string.Format("grade_{0}", chardata.Grade.ToString("D2"));  //"grade_0" + chardata.Grade.ToString();

        //친구 배경 설정
        if (CharSelectFlag != eCharSelectFlag.ARENA)
        {
            bool isFreind = GameSupport.IsFriend(_chardata.CUID);
            kBgFriednSpr?.SetActive(isFreind);
            kBGSpr.SetActive(!isFreind);
        }
    }

    public void UpdateArenaEnemyDetial(int index, CharData chardata)
    {
        _bEnemyDetail = true;
        UpdateArenaTeamSlot(index, chardata);
    }

    public void UpdateLobbyBgCharSlot(int selectedSlotIndex, CharData charData, int placementSlotIndex)
    {
        mbLobbyBgChar = true;
        mLobbyBgCharSlotIndex = selectedSlotIndex;

        UpdateSlot(charData);

        kLevelLabel.gameObject.SetActive(false);
        SprLobbyBgCharSlot.gameObject.SetActive(false);

        if(placementSlotIndex >= 0)
        {
            SprLobbyBgCharSlot.gameObject.SetActive(true);
            SprLobbyBgCharSlot.spriteName = string.Format("ico_Mainchara_number{0}", placementSlotIndex + 1);
        }
    }

    public void UpdateFacilityOperationSlot(CharData charData, Action<GameObject> clickAction)
    {
        _bFacilityOperation = true;
        _facilityClickAction = clickAction;

        UpdateSlot(charData);
    }

    public void UpdatePresetSlot(int index, eCharSelectFlag charSelectFlag, CharData charData)
    {
        CharSelectFlag = charSelectFlag;

        UpdateArenaTeamSlot(index, charData, true);
    }

	public void OnClick_Slot()
	{
        if(mbLobbyBgChar)
        {
            SelectLobbyBgChar();
            return;
        }

        if(mbArenaTowerFriendChar)
        {
            SelectArenaTowerFriendChar();
            return;
        }

        if(_bCharInfo)
        {
            if (CharSelectFlag == eCharSelectFlag.Preset)
            {
                return;
            }

            if (_chardata == null)
            {
                if (CharSelectFlag == eCharSelectFlag.ARENA)
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.ArenaTeamCharSlot, (int)_index);
                    UIValue.Instance.SetValue(UIValue.EParamType.CharSeletePopupType, (int)CharSelectFlag);
                    LobbyUIManager.Instance.ShowUI("CharSeletePopup", true);
                }
                else if (CharSelectFlag == eCharSelectFlag.ARENATOWER || CharSelectFlag == eCharSelectFlag.ARENATOWER_STAGE)
                {
                    GameSupport.ShowCharSeletePopupArenaTower(_index, CharSelectFlag);
                    return;
                }

                return;
            }

            UIArenaBattleConfirmPopup uIArenaBattleConfirmPopup = LobbyUIManager.Instance.GetActiveUI<UIArenaBattleConfirmPopup>("ArenaBattleConfirmPopup");
            if (uIArenaBattleConfirmPopup != null)
            {
                LobbyUIManager.Instance.HideUI("ArenaBattleConfirmPopup");
                bool isPvpFriend = UIValue.Instance.ContainsKey(UIValue.EParamType.IsFriendPVP);
                if (isPvpFriend)
                {
                    LobbyUIManager.Instance.HideUI("FriendPopup", false);
                    UIValue.Instance.SetValue(UIValue.EParamType.ArenaCharInfoFlag, (int)eArenaToCharInfoFlag.FRIEND_BATTLE);
                }
                else
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.ArenaCharInfoFlag, (int)eArenaToCharInfoFlag.ARENA_ENEMY_SEARCH);
                }
                
            }
            else
            {
                eArenaToCharInfoFlag flag = eArenaToCharInfoFlag.ARENA_MAIN;
                if (CharSelectFlag == eCharSelectFlag.ARENATOWER) flag = eArenaToCharInfoFlag.ARENATOWER;
                else if (CharSelectFlag == eCharSelectFlag.ARENATOWER_STAGE) flag = eArenaToCharInfoFlag.ARENATOWER_STAGE;
                UIValue.Instance.SetValue(UIValue.EParamType.ArenaCharInfoFlag, (int)flag);
            }

            if (GameSupport.IsFriend(_chardata.CUID))
            {
                // 친구정보
                UIValue.Instance.SetValue(UIValue.EParamType.RankUserType, (int)eRankUserType.ARENATOWER_FRIEND);
                UIValue.Instance.SetValue(UIValue.EParamType.ArenaTeamCharSlot, (int)_index);
                UIValue.Instance.SetValue(UIValue.EParamType.ArenaTowerFriendCUID, _chardata.CUID);
                LobbyUIManager.Instance.ShowUI("UserCharDetailPopup", true);
            }
            else
            {
                LobbyUIManager.Instance.HideUI("PresetPopup");

                UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, _chardata.CUID);
                UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, _chardata.TableData.ID);
                LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);
            }
        }
        
        if(_bRankingUser)
        {
            if (_chardata == null)
                return;
            Log.Show("Ranking!!");
            if (ParentGO == null)
                return;

            UIPvpAllRewardSlot uIPvpAllRewardSlot = ParentGO.GetComponent<UIPvpAllRewardSlot>();
            if(uIPvpAllRewardSlot != null)
            {
                uIPvpAllRewardSlot.RankerDetialUUID_Check(_index);
            }
        }

        if(_bEnemyDetail)
        {
            if (_chardata == null || _chardata.TableID == (int)eCOUNT.NONE)
                return;

            UIValue.Instance.SetValue(UIValue.EParamType.RankUserType, (int)eRankUserType.ARENA_ENEMY);
            UIValue.Instance.SetValue(UIValue.EParamType.ArenaTeamCharSlot, (int)_index);
            LobbyUIManager.Instance.ShowUI("UserCharDetailPopup", true);
        }

        if (_bFacilityOperation)
        {
            _facilityClickAction?.Invoke(gameObject);
        }
    }

    public void SetSynastry(long targetCharTID, TeamCharData teamCharData = null)
    {
        if (_chardata == null)
        {
            kSynastrySpr.gameObject.SetActive(false);
            return;
        }

        int charTID = _chardata.TableID;
        long targetTID = 0;

        GameClientTable.HelpCharInfo.Param helpCharInfo = GameInfo.Instance.GameClientTable.FindHelpCharInfo(x => x.CharID == charTID);
        if (helpCharInfo == null)
            return;
        string strongIds = helpCharInfo.StrongID.Replace(" ", "");
        string weakIds = helpCharInfo.WeakID.Replace(" ", "");

		string[] strongIdArray = Utility.Split(strongIds, ','); //strongIds.Split(',');
		string[] weakIdArray = Utility.Split(weakIds, ','); //weakIds.Split(',');

		if (teamCharData != null)
        {
            targetTID = (long)teamCharData.CharData.TableID;
        }
        else
        {
            if (targetCharTID == 0)
                return;

            targetTID = targetCharTID;
        }


        kSynastrySpr.gameObject.SetActive(true);
        for (int i = 0; i < strongIdArray.Length; i++)
        {
            if (string.IsNullOrEmpty(strongIdArray[i]))
                continue;
            if(long.Parse(strongIdArray[i]) == targetTID)
            {
                kSynastrySpr.spriteName = "ico_Advantage";
                return;
            }
        }

        for(int i = 0; i < weakIdArray.Length; i++)
        {
            if (string.IsNullOrEmpty(weakIdArray[i]))
                continue;

            if(long.Parse(weakIdArray[i]) == targetTID)
            {
                kSynastrySpr.spriteName = "ico_Disadvantage";
                return;
            }
        }

        kSynastrySpr.gameObject.SetActive(false);
    }

    public void UpdateArenaTowerFriendCharSlot(int index, CharData charData, bool selected, int selectCharIndex)
    {
        mbArenaTowerFriendChar = true;

        _index = index;
        _bCharInfo = true;
        _bRankingUser = false;
        _chardata = charData;

        kBGMESpr.gameObject.SetActive(false);
        kSlotSpr.gameObject.SetActive(false);
        kSynastrySpr.gameObject.SetActive(false);
        kArenaSlotPosLabel.gameObject.SetActive(false);

        kSelSpr.gameObject.SetActive(_index == selectCharIndex);
        SprSelectedFriendChar.gameObject.SetActive(selected);

        kBGSpr.SetActive(false);
        kBgFriednSpr.SetActive(true);

        if (_chardata == null || _chardata.TableData == null)
        {
            kGradeSpr.gameObject.SetActive(false);
            kIconTex.gameObject.SetActive(false);
            kLevelLabel.gameObject.SetActive(false);

            return;
        }

        kLevelLabel.gameObject.SetActive(true);
        kLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), charData.Level);

        kIconTex.gameObject.SetActive(true);
        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/MainSlot/MainSlot_" + charData.TableData.Icon + "_" + charData.EquipCostumeID.ToString() + ".png");

        kGradeSpr.gameObject.SetActive(true);
        kGradeSpr.spriteName = string.Format("grade_{0}", charData.Grade.ToString("D2"));  //"grade_0" + chardata.Grade.ToString();

        List<long> listTowerTeamCharUid = GameSupport.GetArenaTowerTeamCharList();
        if (listTowerTeamCharUid != null && listTowerTeamCharUid.Count > 0)
        {
            for (int i = 0; i < listTowerTeamCharUid.Count; i++)
            {
                if (listTowerTeamCharUid[i] == 0)
                {
                    continue;
                }

                if (listTowerTeamCharUid[i] == _chardata.CUID)
                {
                    kSlotSpr.gameObject.SetActive(true);
                    kSlotLabel.textlocalize = string.Format("{0}", (i + 1));

                    break;
                }
            }
        }
    }

    public void OnBtnShowFriendCharDetail()
    {
        if(_chardata == null || _chardata.TableData == null)
        {
            return;
        }

        eArenaToCharInfoFlag flag = eArenaToCharInfoFlag.ARENA_MAIN;
        if (CharSelectFlag == eCharSelectFlag.ARENATOWER)
        {
            flag = eArenaToCharInfoFlag.ARENATOWER;
        }
        else if (CharSelectFlag == eCharSelectFlag.ARENATOWER_STAGE)
        {
            flag = eArenaToCharInfoFlag.ARENATOWER_STAGE;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.ArenaCharInfoFlag, (int)flag);

        // 친구정보
        UIValue.Instance.SetValue(UIValue.EParamType.RankUserType, (int)eRankUserType.ARENATOWER_FRIEND);
        UIValue.Instance.SetValue(UIValue.EParamType.ArenaTeamCharSlot, (int)_index);
        UIValue.Instance.SetValue(UIValue.EParamType.ArenaTowerFriendCUID, _chardata.CUID);

        LobbyUIManager.Instance.ShowUI("UserCharDetailPopup", true);
    }

    private void SelectArenaTowerFriendChar()
    {
        if (_chardata == null || _chardata.TableData == null)
        {
            return;
        }

        UIArenaCharSelectPopup popup = LobbyUIManager.Instance.GetUI<UIArenaCharSelectPopup>();
        if (popup)
        {
            popup.SelectChar(_index);
        }

        kSelSpr.gameObject.SetActive(true);
    }

    private void SelectLobbyBgChar()
    {
        if (_chardata == null || _chardata.TableData == null)
        {
            return;
        }

        UIMainCharSetPopup popup = LobbyUIManager.Instance.GetUI<UIMainCharSetPopup>();
        if(popup == null)
        {
            return;
        }

        popup.SelectLobbyBgChar(_chardata);
    }
}
