using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIUserCharDetailPopup : FComponent
{
	public UIButton kBackBtn;
	public UILabel kNameLabel;
	public UILabel kLevelLabel;
	public UISprite kGradeSpr;
    public List<UISkillListSlot> kSkillList;
    public GameObject kCard;
    public List<UIItemListSlot> kCardItemList;
    public GameObject kMainSkill;
    public UILabel kMainSkillNameLabel;
    public UILabel kMainSkillLevelLabel;
    public UILabel kMainSkillTimeLabel;
    public UILabel kMainSkillDesceLabel;
	public GameObject kWeapon;
    public UIItemListSlot kWeaponItemListSlot;
    public UIItemListSlot kSubWeaponItemListSlot;
    public UILabel kWeaponSkillNameLabel;
	public UILabel kWeaponSkillLevelLabel;
	public UILabel kWeaponSkillDescLabel;
	public UISprite kWeaponSkillCountSpr;
	public UILabel kWeaponSkillCountLabel;
	public GameObject kRank;
	public UISprite kIcon_1Spr;
	public UISprite kIcon_2Spr;
	public UILabel kRankLabel;
	public UITexture kCharTex;
    public GameObject kSocket;
    public List<UISprite> kGemSlotLockList;
    public List<UITexture> kGemTexList;
    public List<UIButton> kGemChangeBtnList;
    public List<UISprite> kSubGemSlotLockList;
    public List<UITexture> kSubGemTexList;
    public List<UIButton> kSubGemChangeBtnList;
    public GameObject kGem_Tooltip;
    public UIStatusUnit kGemStatusUnit_00;
    public UIStatusUnit kGemStatusUnit_01;
    public List<UIGemOptUnit> kGemOptList;

    [Header("Add Gem UR Grade")]
    [SerializeField] private List<UISprite> mainGemSetOptList = null;
    [SerializeField] private List<UISprite> subGemSetOptList = null;

    private TimeAttackRankUserData _timeattackrankuserdata;
    private CharData _chardata;
    private WeaponData _weapondata;
    private WeaponData _subWeaponData;
    private int _seletegemindex = -1;
    private int _selectSubGemIndex = -1;

    private eRankUserType _rankUserType = eRankUserType.NONE;
    private int _arenaRankUserCharSlotIdx = -1;
    private TeamData _arenaRankerData;

    private TeamCharData friendTeamData = null;

    private List<CardData> mListEquipCardData = new List<CardData>();

    private bool mIsFirstRaidClearUser = false;


	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        _timeattackrankuserdata = null;
        _chardata = null;
        _weapondata = null;
        _seletegemindex = -1;

        _subWeaponData = null;
        _selectSubGemIndex = -1;

        var rankerType = UIValue.Instance.GetValue(UIValue.EParamType.RankUserType);
        if (rankerType == null)
            return;

        kRankLabel.gameObject.SetActive(true);
        _rankUserType = (eRankUserType)rankerType;

		switch( _rankUserType ) {
			case eRankUserType.TIMEATTACK:
			case eRankUserType.RAID: {
				if( _rankUserType == eRankUserType.TIMEATTACK ) {
					var objid = UIValue.Instance.GetValue(UIValue.EParamType.TimeAttackRankStageID);
					var objuuid = UIValue.Instance.GetValue(UIValue.EParamType.TimeAttackRankUUID);
					if( objid == null )
						return;
					if( objuuid == null )
						return;

                    _timeattackrankuserdata = GameInfo.Instance.GetTimeAttackRankUserData( (int)objid, (long)objuuid );
                }
                else {
                    object obj = UIValue.Instance.GetValue( UIValue.EParamType.TimeAttackRankStageID );
                    if( obj == null ) {
                        return;
					}

                    int step = (int)obj;

                    obj = UIValue.Instance.GetValue( UIValue.EParamType.TimeAttackRankUUID );
                    if( obj == null ) {
                        return;
					}

                    long userUid = (long)obj;
                    _timeattackrankuserdata = GameInfo.Instance.GetRaidRankUserDataOrNull( step, userUid, mIsFirstRaidClearUser );
                }
				
				if( _timeattackrankuserdata != null ) {
					_chardata = _timeattackrankuserdata.CharData;
					Log.Show( _chardata.EquipCostumeID );
					_weapondata = _timeattackrankuserdata.WeaponData;
					_subWeaponData = _timeattackrankuserdata.SubWeaponData;
					RenderTargetChar.Instance.gameObject.SetActive( true );
					RenderTargetChar.Instance.InitRenderTargetChar( _timeattackrankuserdata.CharData.TableID, _timeattackrankuserdata.CharData.CUID, _chardata, true, eCharacterType.Character );
					RenderTargetChar.Instance.SetCostumeBody( _chardata.EquipCostumeID, _chardata.CostumeColor, _chardata.CostumeStateFlag, _chardata.DyeingData );
				}
			}
			break;
			case eRankUserType.ARENA: {
				_arenaRankUserCharSlotIdx = (int)UIValue.Instance.GetValue( UIValue.EParamType.ArenaTeamCharSlot );
				_arenaRankerData = GameInfo.Instance.ArenaRankerDetialData;
				_chardata = _arenaRankerData.charlist[_arenaRankUserCharSlotIdx].CharData;
				_weapondata = _arenaRankerData.charlist[_arenaRankUserCharSlotIdx].MainWeaponData;
				_subWeaponData = _arenaRankerData.charlist[_arenaRankUserCharSlotIdx].SubWeaponData;

				RenderTargetChar.Instance.gameObject.SetActive( true );
				RenderTargetChar.Instance.InitRenderTargetChar( _chardata.TableID, _chardata.CUID, _chardata, true, eCharacterType.Character );
				RenderTargetChar.Instance.SetCostumeBody( _chardata.EquipCostumeID, _chardata.CostumeColor, _chardata.CostumeStateFlag, _chardata.DyeingData );
			}
			break;
			case eRankUserType.ARENA_ENEMY: {
				_arenaRankUserCharSlotIdx = (int)UIValue.Instance.GetValue( UIValue.EParamType.ArenaTeamCharSlot );
				_arenaRankerData = GameInfo.Instance.MatchTeam;
				_chardata = _arenaRankerData.charlist[_arenaRankUserCharSlotIdx].CharData;
				_weapondata = _arenaRankerData.charlist[_arenaRankUserCharSlotIdx].MainWeaponData;
				_subWeaponData = _arenaRankerData.charlist[_arenaRankUserCharSlotIdx].SubWeaponData;

				RenderTargetChar.Instance.gameObject.SetActive( true );
				RenderTargetChar.Instance.InitRenderTargetChar( _chardata.TableID, _chardata.CUID, _chardata, true, eCharacterType.Character );
				RenderTargetChar.Instance.SetCostumeBody( _chardata.EquipCostumeID, _chardata.CostumeColor, _chardata.CostumeStateFlag, _chardata.DyeingData );
			}
			break;
			case eRankUserType.ARENATOWER_FRIEND: {
				friendTeamData = null;

				_arenaRankUserCharSlotIdx = (int)UIValue.Instance.GetValue( UIValue.EParamType.ArenaTeamCharSlot );
				long cuid = (long)UIValue.Instance.GetValue(UIValue.EParamType.ArenaTowerFriendCUID);

				var iterA = GameInfo.Instance.TowerFriendTeamData.GetEnumerator();
				while( iterA.MoveNext() ) {
					var iterB = iterA.Current.charlist.GetEnumerator();
					while( iterB.MoveNext() ) {
						if( iterB.Current.CharData.CUID == cuid ) {
							friendTeamData = iterB.Current;
							break;
						}
					}
					if( friendTeamData != null )
						break;
				}

				if( friendTeamData == null )
					return;

				_chardata = friendTeamData.CharData;
				_weapondata = friendTeamData.MainWeaponData;
				_subWeaponData = friendTeamData.SubWeaponData;

				RenderTargetChar.Instance.gameObject.SetActive( true );
				RenderTargetChar.Instance.InitRenderTargetChar( _chardata.TableID, _chardata.CUID, _chardata, true, eCharacterType.Character );
				RenderTargetChar.Instance.SetCostumeBody( _chardata.EquipCostumeID, _chardata.CostumeColor, _chardata.CostumeStateFlag, _chardata.DyeingData );
			}
			break;
		}
	}

	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_chardata.TableData.Name);
        kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _chardata.Level, GameSupport.GetCharMaxLevel(_chardata.Grade));
        kGradeSpr.spriteName = string.Format("grade_{0}", _chardata.Grade.ToString("D2"));  //"grade_0" + _chardata.Grade.ToString();
        kGradeSpr.MakePixelPerfect();

        mListEquipCardData.Clear();
        switch (_rankUserType)
        {
            case eRankUserType.TIMEATTACK:
            case eRankUserType.RAID:
                mListEquipCardData.AddRange(_timeattackrankuserdata.CardList);
                break;

            case eRankUserType.ARENA:
            case eRankUserType.ARENA_ENEMY:
                mListEquipCardData.AddRange(_arenaRankerData.charlist[_arenaRankUserCharSlotIdx].CardList);
                break;

            case eRankUserType.ARENATOWER_FRIEND:
                mListEquipCardData.AddRange(friendTeamData.CardList);
                break;
        }

        for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
        {
            var passviedata = _chardata.PassvieList.Find(x => x.SkillID == _chardata.EquipSkill[i]);
            kSkillList[i].ParentGO = this.gameObject;
            kSkillList[i].UpdateSlot(UISkillListSlot.ePOS.RANKING, i, passviedata, _chardata, mListEquipCardData, false);
        }

        for (int i = 0; i < kCardItemList.Count; i++)
            kCardItemList[i].gameObject.SetActive(false);

        switch (_rankUserType)
        {
            case eRankUserType.TIMEATTACK:
            case eRankUserType.RAID:
                SetTimeAttackRankUserInfo(); 
                break;

            case eRankUserType.ARENA:               
                SetArenaRankUserInfo(); 
                break;

            case eRankUserType.ARENA_ENEMY:         
                SetArenaEnemyDetailUserInfo(); 
                break;

            case eRankUserType.ARENATOWER_FRIEND:   
                SetArenaFriendDetailUserInfo(); 
                break;
        }

        kGem_Tooltip.SetActive(false);

        ShowToolTip(eWeaponSlot.MAIN, _seletegemindex);
        ShowToolTip(eWeaponSlot.SUB, _selectSubGemIndex);

    }

    public void SetFirstRankClearUser( bool isFirstRaidClearUser ) {
        mIsFirstRaidClearUser = isFirstRaidClearUser;
    }

    private void SetTimeAttackRankUserInfo()
    {
        if (_timeattackrankuserdata != null)
        {
            if( _timeattackrankuserdata.IsRaidFirstRanker ) {
                kIcon_1Spr.gameObject.SetActive( true );
                kIcon_2Spr.gameObject.SetActive( true );
            }
            else {
                if( _timeattackrankuserdata.Rank == 1 ) {
                    kIcon_1Spr.gameObject.SetActive( true );
                    kIcon_2Spr.gameObject.SetActive( false );
                }
                else {
                    kIcon_1Spr.gameObject.SetActive( false );
                    kIcon_2Spr.gameObject.SetActive( true );
                }
            }

            if( _timeattackrankuserdata.IsRaidFirstRanker ) {
                kRankLabel.textlocalize = string.Format( FLocalizeString.Instance.GetText( 3331 ) );
            }
            else {
                kRankLabel.textlocalize = string.Format( FLocalizeString.Instance.GetText( 1448 ), _timeattackrankuserdata.Rank.ToString( "#,##0" ) );
            }
        }

        for (int i = 0; i < _timeattackrankuserdata.CardList.Count; i++)
        {
            if (_timeattackrankuserdata.CardList[i] != null)
            {
                kCardItemList[i].gameObject.SetActive(true);
                kCardItemList[i].UpdateSlot(UIItemListSlot.ePosType.Info, -1, _timeattackrankuserdata.CardList[i]);
                kCardItemList[i].SetSortLabel(eContentsPosKind.CARD, _timeattackrankuserdata.CardList[i], UIItemPanel.eSortType.SkillLevel);
            }
        }

        kMainSkill.SetActive(false);
        SetMainSkillInfo(_timeattackrankuserdata.CardList[0]);

        SetWeaponInfo(_timeattackrankuserdata.MainGemList, _timeattackrankuserdata.SubGemList);

        
    }

    private void SetArenaRankFriendInfo()
    {
        //friendTeamData
        kIcon_1Spr.gameObject.SetActive(false);
        kIcon_2Spr.gameObject.SetActive(true);

        //kRankLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1448), _arenaRankerData.Rank.ToString("#,##0"));

        for (int i = 0; i < friendTeamData.CardList.Count; i++)
        {
            if(friendTeamData.CardList[i] != null)
            {
                kCardItemList[i].gameObject.SetActive(true);
                kCardItemList[i].UpdateSlot(UIItemListSlot.ePosType.Info, -1, friendTeamData.CardList[i]);
                kCardItemList[i].SetSortLabel(eContentsPosKind.CARD, friendTeamData.CardList[i], UIItemPanel.eSortType.SkillLevel);
            }
        }

        kMainSkill.SetActive(false);
        SetMainSkillInfo(friendTeamData.CardList[0]);

        SetWeaponInfo(friendTeamData.MainGemList, friendTeamData.SubGemList);
    }

    private void SetArenaRankUserInfo()
    {
        if (_arenaRankerData.Rank == 1)
        {
            kIcon_1Spr.gameObject.SetActive(true);
            kIcon_2Spr.gameObject.SetActive(false);
        }
        else
        {
            kIcon_1Spr.gameObject.SetActive(false);
            kIcon_2Spr.gameObject.SetActive(true);
        }
        kRankLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1448), _arenaRankerData.Rank.ToString("#,##0"));

        for (int i = 0; i < _arenaRankerData.charlist[_arenaRankUserCharSlotIdx].CardList.Count; i++)
        {
            if (_arenaRankerData.charlist[_arenaRankUserCharSlotIdx].CardList[i] != null)
            {
                kCardItemList[i].gameObject.SetActive(true);
                kCardItemList[i].UpdateSlot(UIItemListSlot.ePosType.Info, -1, _arenaRankerData.charlist[_arenaRankUserCharSlotIdx].CardList[i]);
                kCardItemList[i].SetSortLabel(eContentsPosKind.CARD, _arenaRankerData.charlist[_arenaRankUserCharSlotIdx].CardList[i], UIItemPanel.eSortType.SkillLevel);
            }
        }

        kMainSkill.SetActive(false);
        SetMainSkillInfo(_arenaRankerData.charlist[_arenaRankUserCharSlotIdx].CardList[0]);

        SetWeaponInfo(_arenaRankerData.charlist[_arenaRankUserCharSlotIdx].MainGemList, _arenaRankerData.charlist[_arenaRankUserCharSlotIdx].SubGemList);
    }

    private void SetArenaEnemyDetailUserInfo()
    {
        SetArenaRankUserInfo();

        if(_arenaRankerData.Rank <= 0)
        {
            kIcon_1Spr.gameObject.SetActive(false);
            kIcon_2Spr.gameObject.SetActive(false);
            kRank.SetActive(false);
        }
    }

    private void SetArenaFriendDetailUserInfo()
    {
        SetArenaRankFriendInfo();
        
        kIcon_1Spr.gameObject.SetActive(false);
        kIcon_2Spr.gameObject.SetActive(false);
        kRank.SetActive(false);
    }

    private void SetWeaponInfo(List<GemData> mainGemList, List<GemData> subGemList)
    {
        if (_weapondata == null)
        {
            kWeapon.SetActive(false);
        }
        else
        {
            kWeapon.SetActive(true);
            SetWeaponSlot(_weapondata, kWeaponItemListSlot, mainGemList, kGemTexList, kGemChangeBtnList, kGemSlotLockList, mainGemSetOptList);

            if (_weapondata.TableData.SkillEffectName > 0)
            {
                kWeaponSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(_weapondata.TableData.SkillEffectName);
                kWeaponSkillDescLabel.textlocalize = GameSupport.GetWeaponSkillDesc(_weapondata.TableData, _weapondata.SkillLv);
                kWeaponSkillLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.SKILL_LEVEL_TXT_NOW_LV, _weapondata.SkillLv, GameSupport.GetMaxSkillLevelCard());
                kWeaponSkillLevelLabel.transform.localPosition = new Vector3(kWeaponSkillNameLabel.transform.localPosition.x + kWeaponSkillNameLabel.printedSize.x + 10, kWeaponSkillLevelLabel.transform.localPosition.y, 0);
            }
            else
            {
                kWeaponSkillNameLabel.textlocalize = "";
                kWeaponSkillDescLabel.textlocalize = "";
                kWeaponSkillLevelLabel.textlocalize = "";
            }

            if (_weapondata.TableData.UseSP <= 0)
            {
                kWeaponSkillCountSpr.gameObject.SetActive(false);
                kWeaponSkillCountLabel.gameObject.SetActive(false);
            }
            else
            {
                kWeaponSkillCountSpr.gameObject.SetActive(true);
                kWeaponSkillCountLabel.gameObject.SetActive(true);
                kWeaponSkillCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), (_weapondata.TableData.UseSP / 100));
            }
        }

        if (_subWeaponData == null)
        {
            kSubWeaponItemListSlot.gameObject.SetActive(false);
            for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
            {
                kSubGemTexList[i].gameObject.SetActive(false);
                kSubGemChangeBtnList[i].gameObject.SetActive(false);
            }
        }
        else
        {
            kSubWeaponItemListSlot.gameObject.SetActive(true);
            SetWeaponSlot(_subWeaponData, kSubWeaponItemListSlot, subGemList, kSubGemTexList, kSubGemChangeBtnList, kSubGemSlotLockList, subGemSetOptList);
        }
    }

    private void SetMainSkillInfo(CardData mainCardData)
    {
        if (mainCardData == null)
            return;

        if (mainCardData != null)
        {
            if (mainCardData.TableData.MainSkillEffectName > 0)
            {
                kMainSkill.SetActive(true);
                kMainSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(mainCardData.TableData.MainSkillEffectName);
                kMainSkillDesceLabel.textlocalize = GameSupport.GetCardMainSkillDesc(mainCardData.TableData, mainCardData.Wake);
                if (mainCardData.Wake == 0)
                    kMainSkillLevelLabel.textlocalize = "";
                else
                    kMainSkillLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_NOW_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PLUS_TEXT), mainCardData.Wake));
                kMainSkillLevelLabel.transform.localPosition = new Vector3(kMainSkillNameLabel.transform.localPosition.x + kMainSkillNameLabel.printedSize.x + 10, kMainSkillNameLabel.transform.localPosition.y, 0);

                if (mainCardData.TableData.CoolTime == 0)
                {
                    kMainSkillTimeLabel.gameObject.SetActive(false);
                }
                else
                {
                    kMainSkillTimeLabel.gameObject.SetActive(true);
                    kMainSkillTimeLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(263), mainCardData.TableData.CoolTime);
                }
            }
        }
    }

    private void SetWeaponSlot(WeaponData slotdata, UIItemListSlot weaponSlot, List<GemData> gemList, List<UITexture> gemTexList, List<UIButton> gemChangeBtnList, List<UISprite> gemSlotLockList, List<UISprite> gemSetOptList)
    {
        weaponSlot.UpdateSlot(UIItemListSlot.ePosType.Info, -1, slotdata);
        weaponSlot.SetSortLabel(eContentsPosKind.WEAPON, slotdata, UIItemPanel.eSortType.SkillLevel);
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {
            gemTexList[i].gameObject.SetActive(false);
            gemChangeBtnList[i].gameObject.SetActive(false);
        }

        // 친구와 대전하기가 아니면
        if (!UIValue.Instance.ContainsKey(UIValue.EParamType.IsFriendPVP))
        {
            if (_rankUserType == eRankUserType.ARENA_ENEMY)
                return;
        }

        int slotmaxcount = GameSupport.GetWeaponGradeSlotCount(slotdata.TableData.Grade, 2);
        int slotcount = GameSupport.GetWeaponGradeSlotCount(slotdata.TableData.Grade, slotdata.Wake);

        if (null == gemList || gemList.Count <= 0)
            return;

        for (int i = 0; i < slotmaxcount; i++)
        {
            if (i >= slotcount)
                gemSlotLockList[i].gameObject.SetActive(true);
            else
                gemSlotLockList[i].gameObject.SetActive(false);
            gemChangeBtnList[i].gameObject.SetActive(true);

            GemData gemdata = gemList[i];
            if (gemdata == null)
                continue;
            if (gemdata.TableData != null)
            {
                gemTexList[i].gameObject.SetActive(true);
                gemTexList[i].mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + gemdata.TableData.Icon);
            }

            GameTable.GemSetType.Param gemSetTypeParam = GameInfo.Instance.GameTable.FindGemSetType(gemdata.SetOptID);
            gemSetOptList[i].SetActive(gemSetTypeParam != null);
            if (gemSetOptList[i].gameObject.activeSelf)
            {
                gemSetOptList[i].spriteName = gemSetTypeParam.Icon;
            }
        }
    }
 	
	public void OnClick_BackBtn()
	{
        OnClickClose();	
	}

    public void OnClick_GemChangeBtn( int index )
    {
        switch(_rankUserType)
        {
            case eRankUserType.TIMEATTACK:
                {
                    if (_timeattackrankuserdata.MainGemList[index].TableData == null)
                        return;
                }
                break;
            case eRankUserType.ARENA:
                {
                    if (_arenaRankerData.charlist[_arenaRankUserCharSlotIdx].MainGemList[index].TableData == null)
                        return;
                }
                break;
            case eRankUserType.ARENA_ENEMY:
                {
                    if (_arenaRankerData.charlist[_arenaRankUserCharSlotIdx].MainGemList[index].TableData == null)
                        return;
                }
                break;
        }
        
        _seletegemindex = index;
        _selectSubGemIndex = -1;
        Renewal(true);
    }

    public void OnClick_SubGemChangeBtn(int index)
    {
        switch(_rankUserType)
        {
            case eRankUserType.TIMEATTACK:
                {
                    if (_timeattackrankuserdata.SubGemList[index].TableData == null)
                        return;
                }
                break;
            case eRankUserType.ARENA:
                {
                    if (_arenaRankerData.charlist[_arenaRankUserCharSlotIdx].SubGemList[index].TableData == null)
                        return;
                }
                break;
            case eRankUserType.ARENA_ENEMY:
                {
                    if (_arenaRankerData.charlist[_arenaRankUserCharSlotIdx].SubGemList[index].TableData == null)
                        return;
                }
                break;
        }

        _selectSubGemIndex = index;
        _seletegemindex = -1;
        Renewal(true);
    }

    private void ShowToolTip(eWeaponSlot weaponSlot, int idx)
    {
        if (idx == -1)
            return;

        GemData gemdata = null;
        if (weaponSlot == eWeaponSlot.MAIN)
        {
            switch(_rankUserType)
            {
                case eRankUserType.TIMEATTACK:
                    gemdata = _timeattackrankuserdata.MainGemList[idx];
                    break;
                case eRankUserType.ARENA:
                    gemdata = _arenaRankerData.charlist[_arenaRankUserCharSlotIdx].MainGemList[idx];
                    break;
                case eRankUserType.ARENA_ENEMY:
                    gemdata = _arenaRankerData.charlist[_arenaRankUserCharSlotIdx].MainGemList[idx];
                    break;
                case eRankUserType.ARENATOWER_FRIEND:
                    gemdata = friendTeamData.MainGemList[idx];
                    break;
            }
        }
        else if (weaponSlot == eWeaponSlot.SUB)
        {
            switch (_rankUserType)
            {
                case eRankUserType.TIMEATTACK:
                    gemdata = _timeattackrankuserdata.SubGemList[idx];
                    break;
                case eRankUserType.ARENA:
                    gemdata = _arenaRankerData.charlist[_arenaRankUserCharSlotIdx].SubGemList[idx];
                    break;
                case eRankUserType.ARENA_ENEMY:
                    gemdata = _arenaRankerData.charlist[_arenaRankUserCharSlotIdx].SubGemList[idx];
                    break;
                case eRankUserType.ARENATOWER_FRIEND:
                    gemdata = friendTeamData.SubGemList[idx];
                    break;
            }

        }

        if (gemdata == null)
            return;

        kGem_Tooltip.SetActive(true);

        int statumain = GameSupport.GetTypeStatusGem(gemdata.TableData.MainType, gemdata.Level, gemdata.Wake, gemdata.TableData);
        int statusub = GameSupport.GetTypeStatusGem(gemdata.TableData.SubType, gemdata.Level, gemdata.Wake, gemdata.TableData);
        kGemStatusUnit_00.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + gemdata.TableData.MainType, statumain);
        if(gemdata.TableData.MainType == gemdata.TableData.SubType)
        {
            kGemStatusUnit_01.gameObject.SetActive(false);
        }
        else
        {
            kGemStatusUnit_01.gameObject.SetActive(true);
            kGemStatusUnit_01.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + gemdata.TableData.SubType, statusub);
        }

        for (int i = 0; i < kGemOptList.Count; i++)
        {
            kGemOptList[i].gameObject.SetActive(true);
            kGemOptList[i].Lock();
        }

        for (int i = 0; i < gemdata.Wake; i++)
            kGemOptList[i].Opt(gemdata, i);

    }

    public void OnClick_HideToolTip()
    {
        _seletegemindex = -1;
        _selectSubGemIndex = -1;
        kGem_Tooltip.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Select))//Input.GetMouseButtonDown(0))
        {
#if UNITY_EDITOR
            if (!UICamera.isOverUI)
            {
                if (_seletegemindex != -1)
                {
                    _seletegemindex = -1;
                    Renewal(true);
                }
            }
#else
            if(Input.touchCount > 0 && !UICamera.Raycast(Input.GetTouch(0).position))
            {
                if (_seletegemindex != -1)
                {
                    _seletegemindex = -1;
                    Renewal(true);
                }
            }
#endif
            /*if (!UICamera.isOverUI)
            {
                if (_seletegemindex != -1)
                {
                    _seletegemindex = -1;
                    Renewal(true);
                }
            }*/
        }
    }
}
