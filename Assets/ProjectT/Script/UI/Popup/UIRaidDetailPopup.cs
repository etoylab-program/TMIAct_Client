
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIRaidDetailPopup : FComponent {
	[Header( "[Raid Info]" )]
	[SerializeField] private UILabel			_StepLabel;
	[SerializeField] private UILabel			_TitleLabel;

	[Header( "[Chars Info]" )]
	[SerializeField] private UILabel			_CombatPowerLabel;
	[SerializeField] private UICharListSlot[]	_CharListSlots;

	[Header( "[Card Formation]" )]
	[SerializeField] private GameObject			_CardFormationObj;
	[SerializeField] private UILabel			_CardFormationNameLabel;

	[Header( "[Buff Item]" )]
	[SerializeField] private UISprite			_AtkPowerUpSelectSpr;
	[SerializeField] private UILabel			_AtkPowerUpCountLabel;
	[SerializeField] private UISprite           _HpUpSelectSpr;
	[SerializeField] private UILabel            _HpUpCountLabel;

	[Header( "[Drop Item]" )]
	[SerializeField] private FList				_DropItemList;
	[SerializeField] private UILabel			_DefaultRaidPoint;

	[Header( "[AI Option]" )]
	[SerializeField] private FToggle			_AutoSupporterSkill;
	[SerializeField] private FToggle            _AutoWeaponSkill;
	[SerializeField] private FToggle            _AutoUltimateSkill;

	[Header( "[Etc.]" )]
	[SerializeField] private UILabel			_NeedBpCountLabel;
	[SerializeField] private ParticleSystem		_UseEff;
	[SerializeField] private UIButton			_EnemyInfoBtn;

	public List<GameTable.Random.Param> DropItemParamList { get; private set; } = new List<GameTable.Random.Param>();

	private List<long>				mSortedCharUidList		= new List<long>();
	private GameTable.Stage.Param	mStageParam				= null;
	private int						mCardFormationId		= 0;
	private GameTable.Item.Param	mAtkPowerUpItemParam	= null;
	private long					mAtkPowerUpItemUid		= 0;
	private GameTable.Item.Param	mHpUpItemParam			= null;
	private long					mHpUpItemUid			= 0;
	private int						mStageCondiDropItemCnt	= 0;
	private bool					mCardEquipFlag			= false;


	public override void Awake() {
		base.Awake();

		_DropItemList.EventUpdate = UpdateDropItemListSlot;
		_DropItemList.EventGetItemCount = GetDropItemElementCount;

		_AutoSupporterSkill.EventCallBack = OnToggleAutoSupporterSkill;
		_AutoWeaponSkill.EventCallBack = OnToggleAutoWeaponSkill;
		_AutoUltimateSkill.EventCallBack = OnToggleAutoUltimateSkill;
	}

	public override void OnEnable() {
		UIValue.Instance.SetValue( UIValue.EParamType.CardFormationType, eCharSelectFlag.RAID );

		int point = (int)( (float)GameInfo.Instance.GameConfig.RewardRaidPoint + ( (float)GameInfo.Instance.GameConfig.RewardRaidPoint * GameInfo.Instance.GameConfig.RaidPointStepRate * (float)( GameInfo.Instance.SelectedRaidLevel - 1 ) ) );
		_DefaultRaidPoint.textlocalize = point.ToString();
		_UseEff.Stop();

		base.OnEnable();
	}

	public override void OnDisable() {
		base.OnDisable();
		FSaveData.Instance.SaveRaidData();
	}

	public override void Renewal( bool bChildren = false ) {
		base.Renewal( bChildren );

		if( CheckRaidCharsInfo() ) {
			RenewalUIs();
		}
	}

	public void ChangeRaidChar( int index, long newCharUid ) {
		if( index < 0 || index >= GameInfo.Instance.RaidUserData.CharUidList.Count || GameInfo.Instance.RaidUserData.CharUidList[index] == newCharUid ) {
			return;
		}

		for( int i = 0; i < GameInfo.Instance.RaidUserData.CharUidList.Count; i++ ) {
			if( i == index ) {
				continue;
			}

			if( GameInfo.Instance.RaidUserData.CharUidList[i] == newCharUid ) {
				long uid = GameInfo.Instance.RaidUserData.CharUidList[index];
				GameInfo.Instance.RaidUserData.CharUidList[i] = uid;

				break;
			}
		}

		GameInfo.Instance.RaidUserData.CharUidList[index] = newCharUid;
		GameInfo.Instance.Send_ReqSetRaidTeam( GameInfo.Instance.RaidUserData.CharUidList, (uint)GameSupport.GetSelectCardFormationID(), OnNetSetRaidTeam );
	}

	public void PlayUseHpItemEffAndRenewal() {
		_UseEff.Stop( true, ParticleSystemStopBehavior.StopEmittingAndClear );
		_UseEff.Play();

		Renewal( true );
	}

	public void OnBtnPreset() {
		if (GameInfo.Instance.RaidPresetDatas != null)
		{
			OnNet_PresetList(0, null);
		}
		else
		{
			GameInfo.Instance.Send_ReqGetUserPresetList(ePresetKind.RAID, 0, OnNet_PresetList);
		}
	}

	private void OnNet_PresetList(int result, PktMsgType pktmsg)
	{
		if (result != 0)
		{
			return;
		}

		PktInfoUserPreset pktInfoUserPreset = pktmsg as PktInfoUserPreset;
		if (pktInfoUserPreset != null && pktInfoUserPreset.infos_.Count <= 0)
		{
			GameInfo.Instance.SetPresetData(ePresetKind.RAID, -1, GameInfo.Instance.GameConfig.ContPresetSlot);
		}

		UIPresetPopup presetPopup = LobbyUIManager.Instance.GetUI<UIPresetPopup>("PresetPopup");
		if (presetPopup == null)
		{
			return;
		}

		presetPopup.SetPresetData(eCharSelectFlag.RAID, ePresetKind.RAID);
		presetPopup.SetUIActive(true);
	}

	public void OnBtnChangeCardFormation() {
		LobbyUIManager.Instance.ShowUI( "ArmoryPopup", true );
	}

	public void OnBtnCardFormationInfo() {
		if( mCardFormationId == (int)eCOUNT.NONE ) {
			return;
		}

		CardTeamToolTipPopup.Show( mCardFormationId, _CardFormationObj, CardTeamToolTipPopup.eCardToolTipDir.LEFT );
	}

	public void OnBtnSelectAtkUpItem() {
		if( mAtkPowerUpItemUid == 0 ) {
			return;
		}

		GameInfo.Instance.RaidAtkBuffRateFlag ^= true;
		SetBuffItem();
	}

	public void OnBtnAtkUpItemInfo() {
		if( mAtkPowerUpItemParam == null ) {
			return;
		}

		UIValue.Instance.SetValue( UIValue.EParamType.ItemUID, (long)-1 );
		UIValue.Instance.SetValue( UIValue.EParamType.ItemTableID, mAtkPowerUpItemParam.ID );
		LobbyUIManager.Instance.ShowUI( "ItemInfoPopup", true );
	}

	public void OnBtnSelectHpUpItem() {
		if( mHpUpItemUid == 0 ) {
			return;
		}

		GameInfo.Instance.RaidHpBuffRateFlag ^= true;
		SetBuffItem();
	}

	public void OnBtnHpUpItemInfo() {
		if( mHpUpItemParam == null ) {
			return;
		}

		UIValue.Instance.SetValue( UIValue.EParamType.ItemUID, (long)-1 );
		UIValue.Instance.SetValue( UIValue.EParamType.ItemTableID, mHpUpItemParam.ID );
		LobbyUIManager.Instance.ShowUI( "ItemInfoPopup", true );
	}

	public void OnBtnEnemyInfo() {
		UIValue.Instance.SetValue( UIValue.EParamType.StageID, mStageParam.ID );
		UIValue.Instance.SetValue( UIValue.EParamType.StageType, 1 );

		LobbyUIManager.Instance.ShowUI( "EnemyInfoPopup", true );
	}

	public void OnBtnStart() {
		if( mStageParam == null || GameInfo.Instance.RaidUserData == null ) {
			return;
		}

		if( !GameSupport.IsCheckTicketBP( mStageParam.Ticket ) || !GameSupport.IsCheckInven() ) {
			return;
		}

		for( int i = 0; i < GameInfo.Instance.RaidUserData.CharUidList.Count; i++ ) {
			if( GameSupport.IsEmptyInEquipMainWeapon( ePresetKind.STAGE, GameInfo.Instance.RaidUserData.CharUidList[i] ) ) {
				return;
			}

			CharData charData = GameInfo.Instance.GetCharData( GameInfo.Instance.RaidUserData.CharUidList[i] );
			if ( charData == null ) {
				return;
			}

			if( charData.RaidHpPercentage <= 0.0f ) {
				MessagePopup.OK( eTEXTID.OK, FLocalizeString.Instance.GetText( 3333 ), null );
				return;
			}
			else if( GameSupport.GetCharLastSkillSlotCheck( charData ) ) {
				charData.EquipSkill[(int)eCOUNT.SKILLSLOT - 1] = (int)eCOUNT.NONE;
				GameInfo.Instance.Send_ReqApplySkillInChar( charData.CUID, charData.EquipSkill, OnNetRemoveSkillAndGameStart );

				return;
			}
		}

		long atkPowerUpItemUid = GameInfo.Instance.RaidAtkBuffRateFlag ? mAtkPowerUpItemUid : 0;
		long hpUpItemUid = GameInfo.Instance.RaidHpBuffRateFlag ? mHpUpItemUid : 0;

		GameInfo.Instance.Send_ReqRaidStageStart( mStageParam.ID, GameInfo.Instance.SelectedRaidLevel, atkPowerUpItemUid, hpUpItemUid, OnNetGameStart );
	}

	private bool CheckRaidCharsInfo() {
		if( GameInfo.Instance.RaidUserData.CharUidList[0] == 0 ) {
			SortCharUidByCombatPower();

			GameInfo.Instance.RaidUserData.CharUidList[0] = GameInfo.Instance.GetMainCharUID();
			GameInfo.Instance.RaidUserData.CharUidList[1] = mSortedCharUidList[0];
			GameInfo.Instance.RaidUserData.CharUidList[2] = mSortedCharUidList[1];

			GameInfo.Instance.Send_ReqSetRaidTeam( GameInfo.Instance.RaidUserData.CharUidList, (uint)GameSupport.GetSelectCardFormationID(), OnNetSetRaidTeam );
			return false;
		}

		return true;
	}

	private void SortCharUidByCombatPower() {
		mSortedCharUidList.Clear();
		mSortedCharUidList.Capacity = GameInfo.Instance.CharList.Count - 1;

		for( int i = 0; i < GameInfo.Instance.CharList.Count; i++ ) {
			if( GameInfo.Instance.CharList[i].CUID == GameInfo.Instance.GetMainCharUID() ) {
				continue;
			}

			mSortedCharUidList.Add( GameInfo.Instance.CharList[i].CUID );
		}

		if( mSortedCharUidList.Count > 0 ) {
			mSortedCharUidList.Sort( delegate ( long lhs, long rhs ) {
				CharData lData = GameInfo.Instance.GetCharData( lhs );
				CharData rData = GameInfo.Instance.GetCharData( rhs );

				if( lData.CombatPower < rData.CombatPower ) {
					return 1;
				}
				else if( lData.CombatPower > rData.CombatPower ) {
					return -1;
				}

				return 0;
			} );
		}
	}

	private void RenewalUIs() {
		if ( GameInfo.Instance.RaidUserData == null || GameInfo.Instance.RaidUserData.CurStageParam == null ) {
			return;
		}

		mStageParam = GameInfo.Instance.RaidUserData.CurStageParam;
		UIValue.Instance.SetValue( UIValue.EParamType.StageID, mStageParam.ID );

		_StepLabel.textlocalize = string.Format( "{0} {1}", FLocalizeString.Instance.GetText( 3318 ), GameInfo.Instance.SelectedRaidLevel.ToString( "D2" ) );
		_TitleLabel.textlocalize = FLocalizeString.Instance.GetText( mStageParam.Name );

		mAtkPowerUpItemParam = GameInfo.Instance.GameTable.FindItem( x => x.Type == (int)eITEMTYPE.MATERIAL &&
																		  x.SubType == (int)eITEMSUBTYPE.MATERIAL_RAID_ATKBUFF );

		mHpUpItemParam = GameInfo.Instance.GameTable.FindItem( x => x.Type == (int)eITEMTYPE.MATERIAL &&
																	x.SubType == (int)eITEMSUBTYPE.MATERIAL_RAID_HPBUFF );

		_AutoSupporterSkill.SetToggle( FSaveData.Instance.RaidAutoSupporterSkill ? 0 : 1, SelectEvent.Code );
		_AutoWeaponSkill.SetToggle( FSaveData.Instance.RaidAutoWeaponSkill ? 0 : 1, SelectEvent.Code );
		_AutoUltimateSkill.SetToggle( FSaveData.Instance.RaidAutoUltimateSkill ? 0 : 1, SelectEvent.Code );

		if( GameInfo.Instance.UserData.IsGoods( eGOODSTYPE.BP, mStageParam.Ticket ) ) {
			_NeedBpCountLabel.textlocalize = string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.GOODSTEXT_W ), mStageParam.Ticket );
		}
		else {
			_NeedBpCountLabel.textlocalize = string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.GOODSTEXT_R ), mStageParam.Ticket );
		}

		GameClientTable.HelpEnemyInfo.Param enemyInfoData = GameInfo.Instance.GameClientTable.FindHelpEnemyInfo( x => x.StageID == mStageParam.ID );
		_EnemyInfoBtn.gameObject.SetActive( enemyInfoData != null );

		SetRaidCharsInfo();
		SetCardFormation();
		SetBuffItem();
		SetDropItems();
	}

	private void SetRaidCharsInfo() {
		_CombatPowerLabel.textlocalize = string.Format( "{0} {1}", FLocalizeString.Instance.GetText( 320 ),
																   string.Format( FLocalizeString.Instance.GetText( 222 ),
																				  GameSupport.GetArenaTeamPower( eContentsPosKind.RAID ) ) );

		for( int i = 0; i < _CharListSlots.Length; i++ ) {
			if( i >= GameInfo.Instance.RaidUserData.CharUidList.Count || GameInfo.Instance.RaidUserData.CharUidList[i] == 0 ) {
				_CharListSlots[i].SetActive( false );
				continue;
			}

			CharData charData = GameInfo.Instance.GetCharData( GameInfo.Instance.RaidUserData.CharUidList[i] );
			_CharListSlots[i].UpdateSlot( UICharListSlot.ePos.RAID, i, charData );
		}
	}

	private void SetCardFormation() {
		mCardFormationId = GameSupport.GetSelectCardFormationID();

		if( mCardFormationId == (int)eCOUNT.NONE ) {
			_CardFormationNameLabel.textlocalize = FLocalizeString.Instance.GetText( 1617 );
		}
		else {
			GameTable.CardFormation.Param cardFormation = GameInfo.Instance.GameTable.FindCardFormation(x => x.ID == mCardFormationId);
			_CardFormationNameLabel.textlocalize = FLocalizeString.Instance.GetText( cardFormation.Name );
		}
	}

	private void SetBuffItem() {
		int count = 0;

		// Atk power up
		mAtkPowerUpItemUid = 0;
		ItemData itemData = GameInfo.Instance.ItemList.Find( x => x.TableData.Type == (int)eITEMTYPE.MATERIAL &&
																  x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_RAID_ATKBUFF );
		if ( itemData != null ) {
			count = GameInfo.Instance.GetItemIDCount( itemData.TableID );
			mAtkPowerUpItemUid = itemData.ItemUID;
		}

		string str = FLocalizeString.Instance.GetText( ( count >= 1 ) ? (int)eTEXTID.GREEN_TEXT_COLOR : (int)eTEXTID.RED_TEXT_COLOR );

		str = string.Format( str, string.Format( FLocalizeString.Instance.GetText( 236 ), count, 1 ) );
		_AtkPowerUpCountLabel.textlocalize = str;

		if ( GameInfo.Instance.RaidAtkBuffRateFlag ) {
			GameInfo.Instance.RaidAtkBuffRateFlag = 0 < count;
		}

		_AtkPowerUpSelectSpr.SetActive( GameInfo.Instance.RaidAtkBuffRateFlag );

		// Hp up
		count = 0;

		mHpUpItemUid = 0;
		itemData = GameInfo.Instance.ItemList.Find( x => x.TableData.Type == (int)eITEMTYPE.MATERIAL &&
														 x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_RAID_HPBUFF );
		if ( itemData != null ) {
			count = GameInfo.Instance.GetItemIDCount( itemData.TableID );
			mHpUpItemUid = itemData.ItemUID;
		}

		str = FLocalizeString.Instance.GetText( ( count >= 1 ) ? (int)eTEXTID.GREEN_TEXT_COLOR : (int)eTEXTID.RED_TEXT_COLOR );

		str = string.Format( str, string.Format( FLocalizeString.Instance.GetText( 236 ), count, 1 ) );
		_HpUpCountLabel.textlocalize = str;

		if ( GameInfo.Instance.RaidHpBuffRateFlag ) {
			GameInfo.Instance.RaidHpBuffRateFlag = 0 < count;
		}

		_HpUpSelectSpr.SetActive( GameInfo.Instance.RaidHpBuffRateFlag );
	}

	private void SetDropItems() {
		DropItemParamList.Clear();
		AddDropItemList( mStageParam.N_DropID + ( GameInfo.Instance.SelectedRaidLevel - 1 ) );

		_DropItemList.UpdateList();
	}

	private void AddDropItemList( int dropId ) {
		List<GameTable.Random.Param> list = GameInfo.Instance.GameTable.FindAllRandom( x => x.GroupID == dropId );

		if ( list == null || list.Count <= 0 ) { // 현재(221208)는 100단계 이후 드랍 정보는 없음.
			list = GameInfo.Instance.GameTable.FindAllRandom( x => x.GroupID >= 6000 && x.GroupID <= 6999 ); // 레이드 드랍 정보는 6000번대만 사용
			list = list.FindAll( x => x.GroupID == list[list.Count - 1].GroupID );
		}

		for ( int i = 0; i < list.Count; i++ ) {
			GameTable.Random.Param param = DropItemParamList.Find( x => x.ProductType == list[i].ProductType && x.ProductIndex == list[i].ProductIndex );

			if ( param == null ) {
				DropItemParamList.Add( list[i] );
			}
			else {
				param.ProductValue = list[i].ProductValue > param.ProductValue ? list[i].ProductValue : param.ProductValue;
			}
		}
	}

	private void UpdateDropItemListSlot( int index, GameObject slotObject ) {
		UIItemListSlot slot = slotObject.GetComponent<UIItemListSlot>();
		slot.ParentGO = gameObject;

		GameTable.Random.Param randomParam = DropItemParamList[index];

		if( randomParam.ProductType == (int)eREWARDTYPE.WEAPON ) {
			GameTable.Weapon.Param weaponParam = GameInfo.Instance.GameTable.FindWeapon( randomParam.ProductIndex );
			if( weaponParam != null ) {
				slot.UpdateSlot( UIItemListSlot.ePosType.RewardTable, index, weaponParam );
			}
		}
		else if( randomParam.ProductType == (int)eREWARDTYPE.GEM ) {
			GameTable.Gem.Param gemParam = GameInfo.Instance.GameTable.FindGem( randomParam.ProductIndex );
			if( gemParam != null ) {
				slot.UpdateSlot( UIItemListSlot.ePosType.RewardTable, index, gemParam );
			}
		}
		else if( randomParam.ProductType == (int)eREWARDTYPE.CARD ) {
			GameTable.Card.Param cardParam = GameInfo.Instance.GameTable.FindCard(randomParam.ProductIndex);
			if( cardParam != null ) {
				slot.UpdateSlot( UIItemListSlot.ePosType.RewardTable, index, cardParam );
			}
		}
		else if( randomParam.ProductType == (int)eREWARDTYPE.ITEM ) {
			GameTable.Item.Param itemParam = GameInfo.Instance.GameTable.FindItem(randomParam.ProductIndex);
			if( itemParam != null ) {
				slot.UpdateSlot( UIItemListSlot.ePosType.RewardTable, index, itemParam );
			}
		}
		else if( randomParam.ProductType == (int)eREWARDTYPE.GOODS ) {
			slot.UpdateSlot( UIItemListSlot.ePosType.RewardTable, index, randomParam );
		}

		if( mStageParam.Condi_Type != (int)eSTAGE_CONDI.NONE && 
			mStageParam.Condi_Type != (int)eSTAGE_CONDI.NOT_CHECK_CONDI && 
			index < mStageCondiDropItemCnt ) {

			slot.SetCardTypeFlag( (eSTAGE_CONDI)mStageParam.Condi_Type, mCardEquipFlag );
		}

		string str = FLocalizeString.Instance.GetText( (int)eTEXTID.PERCENT_ONETWO_POINT_TEXT,
													   GameSupport.GetDropPersent(randomParam.GroupID, randomParam.ProductType, randomParam.ProductIndex ) );
		slot.SetCountText( str );
	}

	private int GetDropItemElementCount() {
		return DropItemParamList.Count;
	}

	private bool OnToggleAutoSupporterSkill( int nSelect, SelectEvent type ) {
		FSaveData.Instance.RaidAutoSupporterSkill = nSelect == 0 ? true : false;
		return true;
	}

	private bool OnToggleAutoWeaponSkill( int nSelect, SelectEvent type ) {
		FSaveData.Instance.RaidAutoWeaponSkill = nSelect == 0 ? true : false;
		return true;
	}

	private bool OnToggleAutoUltimateSkill( int nSelect, SelectEvent type ) {
		FSaveData.Instance.RaidAutoUltimateSkill = nSelect == 0 ? true : false;
		return true;
	}

	private void OnNetSetRaidTeam( int result, PktMsgType pkt ) {
		if( result != 0 ) {
			return;
		}

		RenewalUIs();
	}

	private void OnNetRemoveSkillAndGameStart( int result, PktMsgType pktmsg ) {
		if( result != 0 ) {
			return;
		}

		long atkPowerUpItemUid = GameInfo.Instance.RaidAtkBuffRateFlag ? mAtkPowerUpItemUid : 0;
		long hpUpItemUid = GameInfo.Instance.RaidHpBuffRateFlag ? mHpUpItemUid : 0;

		GameInfo.Instance.Send_ReqRaidStageStart( mStageParam.ID, GameInfo.Instance.SelectedRaidLevel, atkPowerUpItemUid, hpUpItemUid, OnNetGameStart );
	}

	private void OnNetGameStart( int result, PktMsgType pktmsg ) {
		if( result != 0 ) {
			return;
		}

		int stageId = (int)UIValue.Instance.GetValue( UIValue.EParamType.StageID );
		GameTable.Stage.Param stageParam = GameInfo.Instance.GameTable.FindStage( stageId );

		GameInfo.Instance.SelecteStageTableId = stageId;

		UIValue.Instance.SetValue( UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.LobbyToStage );
		UIValue.Instance.SetValue( UIValue.EParamType.LoadingStage, stageId );

		LobbyUIManager.Instance.ShowUI( "LoadingPopup", false );
		AppMgr.Instance.LoadScene( AppMgr.eSceneType.Stage, stageParam.Scene );
	}
}
