using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharListSlot : FSlot
{
    public enum ePos
    {
        Main = 0,
        SeletePopup,
        BookCharList,
        SeleteArenaPopup,
        SeleteArenaTowerPopup,
        SecretQuest,
        Preset_Stage,
        Preset_Char,
        Preset_Char_Info,
        Store,
        RAID,
        RAID_PROLORGUE,
        SELECT_RAID_CHAR,
    }

    public UISprite kBGFriend;
    public UISprite kBGSpr;
    public UISprite kBGDisSpr;
    public UISprite kSelSpr;
    public UITexture kIconSpr;
    public UISprite kGradeSpr;
    public UILabel kLevelLabel;
    public UISprite kMainCharSpr;
    public GameObject kGet;
    public UILabel kGetLabel;
    public UISprite kNoticeSpr;

    public UISprite kSlotbgSpr;
    public UILabel kSlotLabel;

    public UISprite kNameSpr;
    public GameObject kFavorObj;
    public UILabel kFavorLabel;

    public UISprite kSecretDisSpr;
    public UIButton kSecretStageBtn;
    public UILabel kSecretMainSubLabel;

    public GameObject kCharInfoObj;
    public FLocalizeText kCharInfoText;

    [Header( "[for Raid]" )]
    [SerializeField] private UISprite   _RaidMainSpr;
    [SerializeField] private GameObject _RaidHpObj;
    [SerializeField] private UISprite   _RaidHpSpr;
    [SerializeField] private UILabel    _RaidHpLabel;
    [SerializeField] private GameObject _RaidChangeCharObj;


    private ePos _epos;
    private int _index;
    private CharData _chardata;
    private GameTable.Character.Param _tabledata;

    private List<CharData> ArenaTowerTeamcharList = null;

    private void SetSecretStageDisable(ePos pos, bool disable)
    {
        if (kSecretStageBtn != null)
        {
            kSecretStageBtn.SetActive(pos == ePos.SecretQuest && disable);
        }
        if (kSecretDisSpr != null)
        {
            kSecretDisSpr.SetActive(disable);
        }
        if (kSecretMainSubLabel != null) {
            kSecretMainSubLabel.SetActive(false);
        }
    }

    public void UpdateSlot(ePos pos, int index, GameTable.Character.Param tabledata)
    {
        _epos = pos;
        _chardata = GameInfo.Instance.GetCharDataByTableID(tabledata.ID);
        _tabledata = tabledata;
        _index = index;

        kSlotbgSpr.gameObject.SetActive(false);
        kSelSpr.gameObject.SetActive(false);
        kMainCharSpr.gameObject.SetActive(false);
        kNoticeSpr.gameObject.SetActive(false);
        kGet.SetActive(false);
        kIconSpr.applyGradient = false;
        int grade = 1;
        int level = 1;

        kFavorObj.SetActive(_chardata != null);

        if (kCharInfoObj != null)
        {
            kCharInfoObj.SetActive(_chardata != null);
        }

        ShowRaidContents( false );

        if (kCharInfoText != null)
        {
            int stringId = 1046;
            if (_chardata == null)
            {
                GameClientTable.StoreDisplayGoods.Param storeDisplayGoods = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.CharacterID == tabledata.ID);
                if (storeDisplayGoods != null)
                {
                    if (storeDisplayGoods.StoreID == -1 && !string.IsNullOrEmpty(storeDisplayGoods.HideConType))
                    {
                        if (storeDisplayGoods.Category == (int)UIStorePanel.eStoreTabType.STORE_CHAR)
                        {
                            if (storeDisplayGoods.HideConType.Equals("HCT_NFS"))
                            {
                                stringId = 3302;
                                kCharInfoObj.SetActive(true);
                            }
                        }
                    }
                }
            }

            kCharInfoText.SetLabel(stringId);
        }

        bool isSecretStageLockFlag = false;
        if (_chardata != null)
        {
            grade = _chardata.Grade;
            level = _chardata.Level;

            if (_epos == ePos.Store)
            {
                kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/ListSlot/ListSlot_" + _tabledata.Icon + "_" + _tabledata.InitCostume.ToString() + ".png");
            }
            else
            {
                kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/ListSlot/ListSlot_" + _tabledata.Icon + "_" + _chardata.EquipCostumeID.ToString() + ".png");
            }

            kIconSpr.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            kFavorLabel.textlocalize = _chardata.FavorLevel.ToString();

            //if (_chardata.CUID == GameInfo.Instance.UserData.MainCharUID)
            //    kMainCharSpr.gameObject.SetActive(true);
            
            int lobbyBgCharSlotIndex = GameInfo.Instance.UserData.GetLobbyBgCharSlotIndex(_chardata.CUID);
            if (lobbyBgCharSlotIndex >= 0)
            {
                kMainCharSpr.gameObject.SetActive(true);
                kMainCharSpr.spriteName = string.Format("ico_Mainchara_number{0}", lobbyBgCharSlotIndex + 1);
            }

            if (_epos == ePos.Main)
            {
                if (GameSupport.CheckCharData(_chardata))
                {
                    kNoticeSpr.gameObject.SetActive(true);
                }
                else
                {
                    var weapondata = GameInfo.Instance.GetWeaponData(_chardata.EquipWeaponUID);
                    if (weapondata != null)
                    {
                        if (GameSupport.CheckWeaponData(weapondata))
                            kNoticeSpr.gameObject.SetActive(true);
                    }

                    for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
                    {
                        var carddata = GameInfo.Instance.GetCardData(_chardata.EquipCard[i]);
                        if (carddata != null)
                        {
                            if (GameSupport.CheckCardData(carddata))
                                kNoticeSpr.gameObject.SetActive(true);
                        }
                    }
                    
                    ItemData itemData = GameInfo.Instance.ItemList.Find(x =>
                        x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_PREFERENCE_PRESENT);

                    GameTable.LevelUp.Param levelUpTableData =
                            GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == _chardata.TableData.PreferenceLevelGroup && x.Level == _chardata.FavorLevel);

                    if (itemData != null && levelUpTableData != null)
                    {
                        if (0 < _chardata.FavorPreCnt && 0 < levelUpTableData.Exp)
                        {
                            kNoticeSpr.gameObject.SetActive(true);
                        }
                    }
                }
            }
            else if (_epos == ePos.BookCharList)
            {
                if (GameSupport.IsCharOpenTerms_Favor(_chardata))
                {
                    if (PlayerPrefs.HasKey("NCharBook_Favor_" + _chardata.TableID.ToString()))
                        kNoticeSpr.gameObject.SetActive(true);
                }
            }

            kBGSpr.SetActive(true);
            kBGFriend.SetActive(false);
            kBGDisSpr.gameObject.SetActive(false);
            kGradeSpr.gameObject.SetActive(true);
            kLevelLabel.gameObject.SetActive(true);
            kNameSpr.gameObject.SetActive(true);
            
            if (pos == ePos.SecretQuest)
            {
                isSecretStageLockFlag = _chardata.SecretQuestCount <= 0;
            }
        }
        else
        {
            var costumelist = GameInfo.Instance.GameTable.FindAllCostume(x => x.CharacterID == _tabledata.ID);
            if (costumelist.Count != 0)
            {
                //  19/03/08 윤주석 
                //  슬롯 이미지 관련 셋팅
                kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/ListSlot/ListSlot_" + _tabledata.Icon + "_" + costumelist[0].ID.ToString() + ".png");
                kIconSpr.applyGradient = true;
                kIconSpr.color          = new Color(1, 1, 1, 1);
                kIconSpr.GradientTop    = new Color(0.050f, 0.054f, 0.074f, 1.0f);
                kIconSpr.GradientBottom = new Color(0.556f, 0.556f, 0.556f, 0.0f);
            }
            kGet.SetActive(true);
            kGetLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1051), FLocalizeString.Instance.GetText(_tabledata.Name));

            kBGSpr.gameObject.SetActive(false);
            kBGDisSpr.gameObject.SetActive(true);
            kGradeSpr.gameObject.SetActive(false);
            kLevelLabel.gameObject.SetActive(false);
            kNameSpr.gameObject.SetActive(false);
        }

        kGradeSpr.spriteName = string.Format("grade_{0}", grade.ToString("D2"));  //"grade_0" + grade.ToString();
        kGradeSpr.MakePixelPerfect();
        kGradeSpr.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);

        kLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), level);
        Log.Show(_tabledata.Name + " / " + FLocalizeString.Instance.GetText(_tabledata.Name));
        kNameSpr.spriteName = "Name_Vertical_" + ((ePlayerCharType)_tabledata.ID).ToString();

        SetSecretStageDisable(_epos, isSecretStageLockFlag);
        
        if (_epos == ePos.SecretQuest)
        {
            UICharSeletePopup panel = ParentGO.GetComponent<UICharSeletePopup>();
            if (panel != null)
            {
                //메인 슬롯이 선택했다.
                if (panel.SeleteIndex == index) {
                    kSelSpr.gameObject.SetActive(true);
                    kSecretMainSubLabel.SetActive(panel.IsClearSecret);
                    kSecretMainSubLabel.textlocalize = FLocalizeString.Instance.GetText(1885);
                }
                //혹은 서브 슬롯이 선택 했다 (이거 번호로 표시되는 것이면, 해당 순서? 표시해줘야될듯...)
                else if (panel.SeleteSubIndexList.Exists(r => r == index)) {
                    kSelSpr.gameObject.SetActive(true);
                    kSecretMainSubLabel.SetActive(panel.IsClearSecret);
                    kSecretMainSubLabel.textlocalize = FLocalizeString.Instance.GetText(1886);
                }
            }
        }
        else if (_epos == ePos.SeletePopup)
        {
            UICharSeletePopup panel = ParentGO.GetComponent<UICharSeletePopup>();
            if (panel != null)
            {
                if (panel.SeleteIndex == index)
                    kSelSpr.gameObject.SetActive(true);
            }
        }
        else if (_epos == ePos.SeleteArenaPopup)
        {
            UICharSeletePopup panel = ParentGO.GetComponent<UICharSeletePopup>();
            if (panel != null)
            {
                int slotNum = (int)UIValue.Instance.GetValue(UIValue.EParamType.ArenaTeamCharSlot);
                if (panel.SeleteIndex == index)
                {
                    kSelSpr.gameObject.SetActive(true);
                }

                for (int i = 0; i < (int)eArenaTeamSlotPos._MAX_; i++)
                {
                    if (GameInfo.Instance.TeamcharList[i] == (int)eCOUNT.NONE)
                        continue;

                    if(GameInfo.Instance.TeamcharList[i] == _chardata.CUID)
                    {
                        kSlotbgSpr.gameObject.SetActive(true);
                        kSlotLabel.textlocalize = string.Format("{0}", (i + 1));
                        break;
                    }
                }
            }
        }
        else if (_epos == ePos.Store)
        {
            kNameSpr.SetActive(true);

            kGet.SetActive(false);
            kGradeSpr.SetActive(false);
            kLevelLabel.SetActive(false);
            kFavorObj.SetActive(false);
            kMainCharSpr.SetActive(false);

            kBGSpr.SetActive(_chardata == null);
            kBGDisSpr.SetActive(!kBGSpr.gameObject.activeSelf);

            if (kBGDisSpr.gameObject.activeSelf)
            {
                kIconSpr.applyGradient = true;
                kIconSpr.color = new Color(1, 1, 1, 1);
                kIconSpr.GradientTop = new Color(0.050f, 0.054f, 0.074f, 1.0f);
                kIconSpr.GradientBottom = new Color(0.556f, 0.556f, 0.556f, 0.0f);
            }
            else
            {
                kIconSpr.applyGradient = false;
            }

            UIStorePanel storePanel = ParentGO.GetComponent<UIStorePanel>();
            if (storePanel != null)
            {
                kSelSpr.SetActive(storePanel.CharacterSlotIndex == index);
            }
        }
        else if( _epos == ePos.RAID_PROLORGUE ) {
            kFavorObj.SetActive( false );
            kMainCharSpr.SetActive( false );
            kBGDisSpr.SetActive( false );
            kNameSpr.gameObject.SetActive( false );

            if( _RaidChangeCharObj ) {
                _RaidChangeCharObj.SetActive( true );
            }

            if( _index == 0 ) {
                _RaidMainSpr.SetActive( true );
			}
		}
        else if( _epos == ePos.SELECT_RAID_CHAR ) {
            _RaidHpObj.SetActive( true );
            _RaidHpLabel.textlocalize = string.Format( "HP {0}%", _chardata.RaidHpPercentage.ToString( "F1" ) );

            UICharSeletePopup panel = ParentGO.GetComponent<UICharSeletePopup>();
            if( panel != null ) {
                if( panel.SeleteIndex == index ) {
                    kSelSpr.gameObject.SetActive( true );
                }
            }
        }
        else if( _epos == ePos.RAID ) {
            kFavorObj.SetActive( false );
            kMainCharSpr.SetActive( false );
            kBGDisSpr.SetActive( false );

            ShowRaidContents( true );

            if( _index == 0 ) {
                _RaidMainSpr.SetActive( true );
            }
            else {
                _RaidMainSpr.SetActive( false );
            }

            _RaidHpSpr.fillAmount = _chardata.RaidHpPercentage / 100.0f;

            if( _RaidHpLabel ) {
                _RaidHpLabel.textlocalize = string.Format( "{0}%", _chardata.RaidHpPercentage.ToString( "F1" ) );
            }
        }
    }

    public void UpdateSlot(ePos pos, int index, CharData data, int selectedCharIndex, bool withoutFriendChar = false)
    {
        _epos = pos;
        GameInfo.Instance.GetArenaTowerCharList(ref ArenaTowerTeamcharList, withoutFriendChar);
        _chardata = data;
        _tabledata = data.TableData;
        _index = index;

        kSlotbgSpr.gameObject.SetActive(false);
        kSelSpr.gameObject.SetActive(false);
        kMainCharSpr.gameObject.SetActive(false);
        kNoticeSpr.gameObject.SetActive(false);
        kGet.SetActive(false);
        kIconSpr.applyGradient = false;
        int grade = 1;
        int level = 1;

        ShowRaidContents( false );

        if (_chardata != null)
        {
            grade = _chardata.Grade;
            level = _chardata.Level;

            kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/ListSlot/ListSlot_" + _tabledata.Icon + "_" + _chardata.EquipCostumeID.ToString() + ".png");
            kIconSpr.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            if (!withoutFriendChar && _chardata.CUID == GameInfo.Instance.UserData.MainCharUID)
            {
                kMainCharSpr.gameObject.SetActive(true);
            }
            
            kBGDisSpr.gameObject.SetActive(false);
            kGradeSpr.gameObject.SetActive(true);
            kLevelLabel.gameObject.SetActive(true);
            kNameSpr.gameObject.SetActive(true);

            // 배경 설정
            bool IsFriend = GameSupport.IsFriend(_chardata.CUID);
            kBGSpr.SetActive(!IsFriend);
            kBGFriend.SetActive(IsFriend);

            kGradeSpr.spriteName = string.Format("grade_{0}", grade.ToString("D2"));  //"grade_0" + grade.ToString();
            kGradeSpr.MakePixelPerfect();
            kGradeSpr.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);

            kLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), level);
            Log.Show(_tabledata.Name + " / " + FLocalizeString.Instance.GetText(_tabledata.Name));
            kNameSpr.spriteName = "Name_Vertical_" + ((ePlayerCharType)_tabledata.ID).ToString();

            /*            
            UICharSeletePopup panel = ParentGO.GetComponent<UICharSeletePopup>();
            if (panel != null)
            {
                int slotNum = (int)UIValue.Instance.GetValue(UIValue.EParamType.ArenaTowerTeamCharSlot);
                if (panel.SeleteIndex == index)
                {
                    kSelSpr.gameObject.SetActive(true);
                }
            }
            */
            if(selectedCharIndex == index)
            {
                kSelSpr.gameObject.SetActive(true);
            }

            //선봉,중견,대장 번호 설정
            List<long> _towerTeamCharList = GameSupport.GetArenaTowerTeamCharList();
            if (_towerTeamCharList != null && _towerTeamCharList.Count > 0)
            {
                for (int i = 0; i < _towerTeamCharList.Count; i++)
                {
                    if (_towerTeamCharList[i] == 0)
                        continue;
                    
                    if (_towerTeamCharList[i] == _chardata.CUID)
                    {
                        kSlotbgSpr.gameObject.SetActive(true);
                        kSlotLabel.textlocalize = string.Format("{0}", (i + 1));
                        break;
                    }
                }
            }
            
        }
    }

    public void UpdateSlot(ePos pos, int index, CharData charData, int costumeId = -1)
    {
        _epos = pos;
        _index = index;

        if (charData == null)
        {
            kSelSpr.SetActive(false);
            kGet.SetActive(false);
            kGradeSpr.SetActive(false);
            kIconSpr.SetActive(false);
            kLevelLabel.SetActive(false);
            kMainCharSpr.SetActive(false);
            kNoticeSpr.SetActive(false);
            kFavorObj.SetActive(false);
            kNameSpr.SetActive(false);

            ShowRaidContents( false );

            if( _epos == ePos.RAID_PROLORGUE || _epos == ePos.RAID ) {
                kBGDisSpr.SetActive( true );
            }

            return;
        }

        kIconSpr.SetActive(true);

        UpdateSlot(pos, index, charData.TableData);
        if (0 < costumeId)
        {
            kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/ListSlot/ListSlot_" + _tabledata.Icon + "_" + costumeId.ToString() + ".png");
        }
    }

	public void UpdateSlotFavorBuffChar( ePos pos, int index, GameTable.Character.Param tableData ) {
		UpdateSlot( pos, index, tableData );
		kMainCharSpr.SetActive( false );
	}

	public void OnClick_Slot()
    {
        // 추후 요청이 있을 시 오픈될 기능
        //if (_epos == ePos.Preset_Char)
        //{
        //    LobbyUIManager.Instance.HideUI("PresetPopup");
        //}
        //else if (_epos == ePos.Preset_Stage)
        //{
        //    UIValue.Instance.SetValue(UIValue.EParamType.CharSeletePopupType, (int)eCharSelectFlag.Preset);
        //    LobbyUIManager.Instance.ShowUI("CharSeletePopup", true);
        //}

        if( _epos == ePos.RAID_PROLORGUE ) {
            if( _chardata == null ) {
                return;
			}

            LobbyUIManager.Instance.JoinStageID = (int)UIValue.Instance.GetValue( UIValue.EParamType.StageID );
            LobbyUIManager.Instance.JoinCharSeleteIndex = -1;

            UIValue.Instance.SetValue( UIValue.EParamType.CharSelUID, _chardata.CUID );
            UIValue.Instance.SetValue( UIValue.EParamType.CharSelTableID, _chardata.TableData.ID );
            LobbyUIManager.Instance.SetPanelType( ePANELTYPE.CHARINFO );

            LobbyUIManager.Instance.HideUI( "StageDetailPopup" );
            return;
        }
        if( _epos == ePos.RAID ) {
            if( _chardata == null ) {
                return;
            }

            UIValue.Instance.SetValue( UIValue.EParamType.CharSelUID, _chardata.CUID );
            UIValue.Instance.SetValue( UIValue.EParamType.CharSelTableID, _chardata.TableData.ID );
            LobbyUIManager.Instance.SetPanelType( ePANELTYPE.CHARINFO );

            LobbyUIManager.Instance.HideUI( "RaidDetailPopup" );
            return;
        }

        if (ParentGO == null)
            return;

        if (_epos == ePos.Main)
        {
            if (_chardata == null)
            {
                //  캐릭터
                //UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, (long)-1);
                //UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, _tabledata.ID);
                UIStorePanel storePanel = LobbyUIManager.Instance.GetUI<UIStorePanel>("StorePanel");
                if (storePanel != null)
                {
                    storePanel.DirectShow(UIStorePanel.eStoreTabType.STORE_CHAR, _tabledata.ID);
                }
                LobbyUIManager.Instance.SetPanelType(ePANELTYPE.STORE);
            }
            else
            {
                //  코스튬
                UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, _chardata.CUID);
                UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, _tabledata.ID);
                LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);
            }

        }
		else if( _epos == ePos.SeletePopup || _epos == ePos.SeleteArenaPopup || _epos == ePos.SELECT_RAID_CHAR ) {
			UICharSeletePopup panel = ParentGO.GetComponent<UICharSeletePopup>();
			if( panel != null ) {
				panel.SetSeleteIndex( _index );
			}
		}
		else if(_epos == ePos.SeleteArenaTowerPopup)
        {
            UIArenaCharSelectPopup popup = ParentGO.GetComponent<UIArenaCharSelectPopup>();
            if (popup)
            {
                popup.SelectChar(_index);
            }
        }
        else if (_epos == ePos.BookCharList)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, _tabledata.ID);
            LobbyUIManager.Instance.ShowUI("BookCharInfoPopup", true);
        }
        else if (_epos == ePos.SecretQuest)
        {
            if (kSecretDisSpr.gameObject.activeSelf)
            {
                return;
            }
            
            UICharSeletePopup panel = ParentGO.GetComponent<UICharSeletePopup>();
            if (panel != null)
            {
                panel.SetSeleteIndex(_index);
            }
        }
        else if (_epos == ePos.Store)
        {
            UIStorePanel storePanel = ParentGO.GetComponent<UIStorePanel>();
            if (storePanel != null)
            {
                storePanel.CharacterSelectSlot(_index);
            }
        }
    }

    public void OnClick_Retry()
    {
        MessagePopup.CYNItem(
            FLocalizeString.Instance.GetText(1816),
            FLocalizeString.Instance.GetText(1817),
            eTEXTID.OK,
            eTEXTID.CANCEL,
            GameInfo.Instance.GameConfig.SecretResetTicketID,
            GameInfo.Instance.GameConfig.SecretResetTicketCount, 
            () =>
            {
                bool errorReturn = false;
                ItemData itemData = GameInfo.Instance.ItemList.Find(x => x.TableID == GameInfo.Instance.GameConfig.SecretResetTicketID);
                if (itemData == null)
                {
                    errorReturn = true;
                }
                else if (itemData.Count < GameInfo.Instance.GameConfig.SecretResetTicketCount)
                {
                    errorReturn = true;
                }

                if (errorReturn)
                {
                    MessageToastPopup.Show(FLocalizeString.Instance.GetText(110901));
                    return;
                }
                
                List<long> cuidList = new List<long> { _chardata.CUID };
                GameInfo.Instance.Send_ReqResetSecretCntChar(cuidList, OnNetRetry);
            }, 
            null
        );
    }

    public void OnBtnChangeRaidChar() {
        if ( _chardata == null ) {
            return;
		}

        if( _epos == ePos.RAID ) {
            UIValue.Instance.SetValue( UIValue.EParamType.CharSeletePopupType, (int)eCharSelectFlag.RAID );
        }
        else {
            UIValue.Instance.SetValue( UIValue.EParamType.CharSeletePopupType, (int)eCharSelectFlag.RAID_PROLOGUE );
        }

        UIValue.Instance.SetValue( UIValue.EParamType.SelectedRaidCuid, _chardata.CUID );
        UIValue.Instance.SetValue( UIValue.EParamType.SelectedRaidCharIndex, _index );

        LobbyUIManager.Instance.ShowUI( "CharSeletePopup", true );
    }

    public void OnBtnRecoverRaidCharHp() {
        if( _chardata.RaidHpPercentage >= 100.0f ) {
            MessagePopup.OK( eTEXTID.OK, 3334, null );
            return;
		}

        UIRaidRecoveryItemPopup popup = LobbyUIManager.Instance.GetUI<UIRaidRecoveryItemPopup>( "RaidRecoveryItemPopup" );
        popup.SetCharData( _chardata );
        popup.SetUIActive( true );
	}

    private void OnNetRetry(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }
        
        UICharSeletePopup panel = ParentGO.GetComponent<UICharSeletePopup>();
        if (panel != null)
        {
            panel.SetAllChar();
            panel.Renewal(true);
        }
    }

    private void ShowRaidContents( bool show ) {
        if( _RaidMainSpr ) {
            _RaidMainSpr.SetActive( show );
        }

        if( _RaidHpSpr ) {
            _RaidHpSpr.SetActive( show );
        }

        if( _RaidHpObj ) {
            _RaidHpObj.SetActive( show );
        }

        if( _RaidChangeCharObj ) {
            _RaidChangeCharObj.SetActive( show );
        }
	}
}
