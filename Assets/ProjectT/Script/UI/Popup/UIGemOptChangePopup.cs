using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIGemOptChangePopup : FComponent {
	[Header( "UIGemOptChangePopup" )]
	public UISprite kGradeSpr;
	public UILabel kNameLabel;
	public UISprite kOptEFFSpr;
	public List<UIItemListSlot> kMatItemList;
	public List<UILabel> kHaveCountLabel;
	public List<UIGemOptUnit> kGemOptList;
	public UIGoodsUnit kGoldGoodsUnit;
	public UISprite kArrowSpr;

	[SerializeField] private List<GameObject> _MainObjList;
	[SerializeField] private GameObject _DetailRootObj;
	[SerializeField] private GameObject _DetailAutoGaugeObj;
	[SerializeField] private GameObject _ChangeEffObj;

	[SerializeField] private UIButton _ResetBtn;
	[SerializeField] private UIButton _ChangeBtn;
	[SerializeField] private UIButton _AutoChangeBtn;
	[SerializeField] private UIButton _AutoChangeDisBtn;
	[SerializeField] private UIButton _OptionNowSelectBtn;
	[SerializeField] private UIButton _OptionNextSelectBtn;

	[SerializeField] private UIGemOptUnit _GemOptionUnitNow;
	[SerializeField] private UIGemOptUnit _GemOptionUnitDetailNow;
	[SerializeField] private UIGemOptUnit _GemOptionUnitDetailNext;

	[SerializeField] private FToggle _ChangeOptionToggle;

	[SerializeField] private UISprite _ResetAutoSpr;
	[SerializeField] private UISprite _ResetAutoGaugeSpr;

	[SerializeField] private UILabel _ResetLabel;
	[SerializeField] private UILabel _SelectOptionTypeLabel;
	[SerializeField] private UILabel _GemOptionDetailNextTitleLabel;

	[SerializeField] private ParticleSystem _GaugeParticle;

	[Flags]
	private enum eOptionType {
		NONE = 0,
		MAIN = 1 << 0,
		AUTO = 1 << 1,
		SELECT = 1 << 2,
		END = 1 << 3,
	}

	private GemData mGemData;

	private Vector3 mOriginalEffPos;

	private eOptionType mOptionType;

	private int mSelectOptionIndex;
	private int mAutoOptionID;
	private int mAutoOptionMaxValue;

	private bool mIsNextRenewal;
	private bool mIsSelect;
	private bool mIsOptionMaxValue;

	private List<int> mMatIdList = new List<int>();
	private List<int> mMatCountList = new List<int>();
	private List<GameTable.GemRandOpt.Param> mGemRandOptParamList = new List<GameTable.GemRandOpt.Param>();

	private WaitForFixedUpdate mWaitForFixedUpdate = new WaitForFixedUpdate();

	public override void Awake() {
		base.Awake();

		_ChangeOptionToggle.EventCallBack = OnEventMaxValueTabSelect;
		_ResetAutoGaugeSpr.fillAmount = 0.0f;

		mOriginalEffPos = kOptEFFSpr.transform.localPosition;
	}

	public override void OnEnable() {
		InitComponent();
		base.OnEnable();
	}

	public override void InitComponent() {
		_ChangeOptionToggle.SetToggle( (int)eToggleType.On, SelectEvent.Code );
		_ChangeEffObj.SetActive( false );
		kOptEFFSpr.SetActive( false );
		_ResetBtn.isEnabled = true;

		_GaugeParticle.gameObject.SetActive( false );

		for ( int i = 0; i < kMatItemList.Count; i++ ) {
			kMatItemList[i].SetActive( false );
			kHaveCountLabel[i].SetActive( false );
		}

		mGemData = GameInfo.Instance.GetGemData( (long)UIValue.Instance.GetValue( UIValue.EParamType.GemUID ) );

		int selectIndex = -1;
		if ( mGemData != null ) {
			kGradeSpr.spriteName = Utility.AppendString( "itemgrade_L_", mGemData.TableData.Grade.ToString() );
			kNameLabel.textlocalize = FLocalizeString.Instance.GetText( mGemData.TableData.Name );

			mGemRandOptParamList = GameInfo.Instance.GameTable.FindAllGemRandOpt( x => x.GroupID == mGemData.TableData.RandOptGroup );

			GameTable.ItemReqList.Param itemReqListParam = GameInfo.Instance.GameTable.FindItemReqList( x => x.Group == mGemData.TableData.OptResetReqGroup );
			if ( itemReqListParam != null ) {
				mMatIdList.Clear();
				mMatCountList.Clear();

				GameSupport.SetMatList( itemReqListParam, ref mMatIdList, ref mMatCountList );

				kGoldGoodsUnit.InitGoodsUnit( eGOODSTYPE.GOLD, 0 < itemReqListParam.Gold ? itemReqListParam.Gold : itemReqListParam.GoodsValue, true );
			}

			if ( 0 <= mGemData.TempOptIndex ) {
				selectIndex = mGemData.TempOptIndex;
			}
		}

		SelectOption( selectIndex );

		mIsNextRenewal = true;
		mIsOptionMaxValue = true;

		mAutoOptionID = -1;
		mAutoOptionMaxValue = -1;

		if ( mIsSelect ) {
			InitFlag( eOptionType.SELECT );
		}
		else {
			InitFlag( eOptionType.MAIN );
		}
	}

	public override void Renewal( bool bChildren = false ) {
		base.Renewal( bChildren );

		bool isMain = CheckFlag( eOptionType.MAIN );

		for ( int i = 0; i < _MainObjList.Count; i++ ) {
			_MainObjList[i].SetActive( isMain );
		}

		_ChangeBtn.SetActive( isMain );
		_AutoChangeBtn.SetActive( isMain && 0 <= mAutoOptionID );
		_AutoChangeDisBtn.SetActive( isMain && mAutoOptionID < 0 );

		kArrowSpr.SetActive( !isMain );
		_ResetBtn.SetActive( !isMain );

		bool isAuto = CheckFlag( eOptionType.AUTO );
		bool isEnd = CheckFlag( eOptionType.END );

		_ResetAutoSpr.SetActive( !isMain && isAuto );

		_DetailAutoGaugeObj.SetActive( !isMain && !isEnd && isAuto );

		_DetailRootObj.SetActive( !isMain );
		_GemOptionUnitDetailNow.SetActive( !isMain );
		_GemOptionUnitDetailNext.SetActive( !isMain && !isEnd );
		_GemOptionDetailNextTitleLabel.SetActive( !isMain && !isEnd );

		bool isSelect = CheckFlag( eOptionType.SELECT );
		_OptionNowSelectBtn.SetActive( !isMain && isSelect );
		_OptionNextSelectBtn.SetActive( !isMain && isSelect );

		for ( int i = 0; i < mMatIdList.Count; i++ ) {
			if ( kMatItemList.Count <= i ) {
				break;
			}

			kMatItemList[i].SetActive( true );
			kMatItemList[i].UpdateSlot( UIItemListSlot.ePosType.Mat, i, GameInfo.Instance.GameTable.FindItem( mMatIdList[i] ) );

			int matCount = GameInfo.Instance.GetItemIDCount( mMatIdList[i] );
			int matMaxCount = mMatCountList[i];

			kMatItemList[i].SetCountLabel( FLocalizeString.Instance.GetText( matMaxCount <= matCount ? (int)eTEXTID.GREEN_TEXT_COLOR : (int)eTEXTID.RED_TEXT_COLOR, FLocalizeString.Instance.GetText( 236, matCount, matMaxCount ) ) );
		}

		if ( isMain ) {
			for ( int i = 0; i < kGemOptList.Count; i++ ) {
				if ( i < mGemData.Wake ) {
					kGemOptList[i].Opt( mGemData, i );
				}
				else {
					kGemOptList[i].Lock();
				}
			}

			if ( mIsSelect ) {
				SetGemOptionIndex( mSelectOptionIndex, ref _GemOptionUnitNow );
				SetGemOptionIndex( mSelectOptionIndex, ref _GemOptionUnitDetailNow );
			}
			else {
				_GemOptionUnitNow.Lock();
			}
		}
		else {
			SetGemOptionIndex( mSelectOptionIndex, ref _GemOptionUnitDetailNow );

			if ( mIsNextRenewal ) {
				SetGemOptionValue( mGemData.TempOptID, mGemData.TempOptValue, ref _GemOptionUnitDetailNext );
			}

			mIsNextRenewal = true;

			if ( isAuto ) {
				if ( isSelect || isEnd ) {
					_ResetLabel.textlocalize = FLocalizeString.Instance.GetText( 3349 );
				}
				else {
					_ResetLabel.textlocalize = FLocalizeString.Instance.GetText( 3348 );
				}
			}
			else {
				if ( 0 <= mGemData.TempOptIndex ) {
					_ResetLabel.textlocalize = FLocalizeString.Instance.GetText( 3350 );
				}
				else {
					_ResetLabel.textlocalize = FLocalizeString.Instance.GetText( 1159 );
				}
			}
		}

		UIGoodsPopup goodsPopup = LobbyUIManager.Instance.GetActiveUI<UIGoodsPopup>( "GoodsPopup" );
		if ( isAuto && !isSelect && !isEnd ) {
			goodsPopup?.PauseButton( UIGoodsPopup.eButtonPauseType.AP, UIGoodsPopup.eButtonPauseType.CASH );
			goodsPopup?.SetPauseMessage( FLocalizeString.Instance.GetText( 3367 ) );
		}
		else {
			goodsPopup?.PlayButton( UIGoodsPopup.eButtonPauseType.AP, UIGoodsPopup.eButtonPauseType.CASH );
		}
	}

	public override void OnClickClose() {
		if ( IsAutoIng() ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3367 ) );
			return;
		}

		if ( !CheckFlag( eOptionType.MAIN ) ) {
			if ( CheckFlag( eOptionType.END ) ) {
				InitFlag( eOptionType.MAIN );
				Renewal();
				return;
			}
		}

		base.OnClickClose();
	}

	public bool IsAutoIng() {
		return CheckFlag( eOptionType.AUTO ) && !CheckFlag( eOptionType.SELECT ) && !CheckFlag( eOptionType.END );
	}

	public void OnClick_BackBtn() {
		OnClickClose();
	}

	public void OnClick_NowSeletBtn() {
		if ( !mIsSelect ) {
			return;
		}

		GameInfo.Instance.Send_ReqResetOptSelectGem( mGemData.GemUID, false, OnNetGemSelete );
	}

	public void OnClick_NextSeletBtn() {
		if ( !mIsSelect ) {
			return;
		}

		GameInfo.Instance.Send_ReqResetOptSelectGem( mGemData.GemUID, true, OnNetGemSelete );
	}

	public void OnClick_OptBtn( int index ) {
		if ( !IsSelectCheck( index ) ) {
			return;
		}

		SelectOption( index );

		Renewal();
	}

	public void OnClick_LevelUpBtn() {
		if ( !IsPreInspectionCheck( isReset: true ) ) {
			return;
		}

		GameInfo.Instance.Send_ReqResetOptGem( mGemData.GemUID, mSelectOptionIndex, OnNetGemChange );
	}

	public void OnClick_AutoResetBtn() {
		if ( mAutoOptionID < 0 ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3139 ) );
			return;
		}

		if ( !IsPreInspectionCheck( isReset: true ) ) {
			return;
		}

		MessagePopup.YNAuto( eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText( 3363 ), FLocalizeString.Instance.GetText( 3351 ), AutoStart );
	}

	public void OnClick_OptionBtn() {
		UIGemOptChangeAutoPopup gemOptChangeAutoPopup = LobbyUIManager.Instance.GetUI<UIGemOptChangeAutoPopup>( "GemOptChangeAutoPopup" );
		if ( gemOptChangeAutoPopup == null ) {
			return;
		}

		if ( mGemData == null ) {
			return;
		}

		gemOptChangeAutoPopup.SetOptionList( ref mGemData, ref mGemRandOptParamList, ref mSelectOptionIndex );
		gemOptChangeAutoPopup.SetUIActive( true );
	}

	public void OnClick_HelpBtn() {
		UIGemOptChangeAutoPopup gemOptChangeAutoPopup = LobbyUIManager.Instance.GetUI<UIGemOptChangeAutoPopup>( "GemOptChangeAutoPopup" );
		if ( gemOptChangeAutoPopup == null ) {
			return;
		}

		gemOptChangeAutoPopup.SetHelp();
		gemOptChangeAutoPopup.SetUIActive( true );
	}

	public void OnClick_ResetBtn() {
		if ( CheckFlag( eOptionType.AUTO ) ) {
			if ( CheckFlag( eOptionType.SELECT ) || CheckFlag( eOptionType.END ) ) {
				if ( 0 <= mGemData.TempOptIndex ) {
					MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3139 ) );
					return;
				}

				AutoStart();
			}
			else {
				AddFlag( eOptionType.SELECT );

				StopCoroutine( nameof( AutoGemChangeOption ) );

				Renewal();
			}
		}
		else {
			if ( 0 <= mGemData.TempOptIndex ) {
				if ( mGemData.TempOptID < 0 ) {
					return;
				}

				if ( !IsPreInspectionCheck( isReset: false ) ) {
					return;
				}

				GameInfo.Instance.Send_ReqResetOptSelectGem( mGemData.GemUID, false, OnNetGemReChange );
			}
			else {
				OnClick_LevelUpBtn();
			}
		}
	}

	public void SetAutoOptionIndex( int index ) {
		if ( index < 0 || mGemRandOptParamList.Count <= index ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3139 ) );
			return;
		}

		if ( IsOptionMaxValueForSelectIndex( mGemRandOptParamList[index].ID ) ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3352 ) );
			return;
		}

		mAutoOptionID = mGemRandOptParamList[index].ID;
		mAutoOptionMaxValue = mGemRandOptParamList[index].RndStep;

		SetOptionTypeLabel( index );

		Renewal();
	}

	private bool OnEventMaxValueTabSelect( int nSelect, SelectEvent type ) {
		if ( type == SelectEvent.Enable ) {
			return false;
		}

		mIsOptionMaxValue = nSelect.Equals( (int)eToggleType.On );

		return true;
	}

	private void OnNetGemChange( int result, PktMsgType pktmsg ) {
		if ( result != 0 ) {
			return;
		}

		ClearFlag();

		StartCoroutine( nameof( GemChangeOption ) );
	}

	private void OnNetAutoGemChange( int result, PktMsgType pktmsg ) {
		if ( result != 0 ) {
			return;
		}

		_ResetAutoGaugeSpr.fillAmount = 0.0f;

		InitFlag( eOptionType.AUTO );

		StartCoroutine( nameof( AutoGemChangeOption ) );
	}

	private void OnNetAutoGemSelect( int result, PktMsgType pktmsg ) {
		if ( result != 0 ) {
			return;
		}

		mIsNextRenewal = false;
		Renewal();

		AutoStart();
	}

	private void OnNetGemSelete( int result, PktMsgType pktmsg ) {
		if ( result != 0 ) {
			return;
		}

		mIsNextRenewal = false;

		DeleteFlag( eOptionType.SELECT );

		if (CheckFlag( eOptionType.MAIN )) {
			InitFlag( eOptionType.MAIN );
		}
		else {
			AddFlag( eOptionType.END );
		}

		Renewal();

		LobbyUIManager.Instance.Renewal( "GemInfoPopup" );

		switch ( LobbyUIManager.Instance.PanelType ) {
			case ePANELTYPE.ITEM: {
				LobbyUIManager.Instance.InitComponent( "ItemPanel" );
				LobbyUIManager.Instance.Renewal( "ItemPanel" );
			}
			break;
			case ePANELTYPE.CHARINFO: {
				LobbyUIManager.Instance.Renewal( "CharInfoPanel" );
				LobbyUIManager.Instance.InitComponent( "WeaponGemSeletePopup" );
				LobbyUIManager.Instance.Renewal( "WeaponGemSeletePopup" );
			}
			break;
			default: {

			}
			break;
		}
	}

	private void OnNetGemReChange( int result, PktMsgType pktmsg ) {
		if ( result != 0 ) {
			return;
		}

		GameInfo.Instance.Send_ReqResetOptGem( mGemData.GemUID, mSelectOptionIndex, OnNetGemChange );
	}

	private IEnumerator NextGaugeAnimation() {
		mIsNextRenewal = false;
		Renewal();

		SetGemOptionValue( mGemData.TempOptID, 0, ref _GemOptionUnitDetailNext );

		GameTable.GemRandOpt.Param gemRandOptParam = mGemRandOptParamList.Find( x => x.ID == mGemData.TempOptID );
		if ( gemRandOptParam != null ) {
			_GemOptionUnitDetailNext.kTextLabel.textlocalize = FLocalizeString.Instance.GetText( gemRandOptParam.Desc, 0.0f );
		}

		kArrowSpr.SetActive( true );

		SoundManager.Instance.PlayUISnd( 25 );
		GameSupport.PlayParticle( _ChangeEffObj );

		yield return new WaitForSeconds( 1.0f );

		SetGemOptionValueAnim( mGemData.TempOptID, mGemData.TempOptValue, true, ref _GemOptionUnitDetailNext );

		yield return new WaitForSeconds( 1.5f );
	}

	private IEnumerator GemChangeOption() {
		LobbyUIManager.Instance.Renewal( "TopPanel" );
		LobbyUIManager.Instance.Renewal( "GoodsPopup" );
		LobbyUIManager.Instance.Renewal( "GemInfoPopup" );

		if ( !CheckFlag( eOptionType.AUTO ) ) {
			_ResetBtn.isEnabled = false;
		}

		yield return NextGaugeAnimation();

		if ( !CheckFlag( eOptionType.AUTO ) ) {
			_ResetBtn.isEnabled = true;
		}

		if ( !CheckFlag( eOptionType.AUTO ) ) {
			AddFlag( eOptionType.SELECT );
		}

		Renewal();
	}

	private IEnumerator AutoGemChangeOption() {
		yield return GemChangeOption();

		bool isChange = false;
		if ( mGemData.TempOptID == mAutoOptionID ) {
			if ( mGemData.RandOptID[mSelectOptionIndex] != mAutoOptionID ) {
				isChange = true;
			}
			else {
				isChange = mGemData.RandOptValue[mSelectOptionIndex] < mGemData.TempOptValue;
			}

			if ( mIsOptionMaxValue ) {
				if ( mGemData.TempOptValue == mAutoOptionMaxValue ) {
					AddFlag( eOptionType.MAIN );
					AddFlag( eOptionType.END );
				}
			}
			else {
				AddFlag( eOptionType.END );
			}

			if ( mGemData.TempOptValue == mAutoOptionMaxValue ) {
				SelectOption( -1 );
			}
		}

		if ( CheckFlag( eOptionType.END ) ) {
			GameInfo.Instance.Send_ReqResetOptSelectGem( mGemData.GemUID, isChange, OnNetGemSelete );
			yield break;
		}

		float waitTimeSec = 0;
		float totalTimeSec = GameInfo.Instance.GameConfig.GemOptAutoTimeSec;
		while ( waitTimeSec < totalTimeSec ) {
			waitTimeSec += Time.fixedDeltaTime;
			_ResetAutoGaugeSpr.fillAmount = waitTimeSec / totalTimeSec;
			yield return mWaitForFixedUpdate;
		}

		if ( _GaugeParticle.gameObject.activeSelf ) {
			_GaugeParticle.Play();
		}
		else {
			_GaugeParticle.gameObject.SetActive( true );
		}

		_ResetBtn.isEnabled = false;

		waitTimeSec = 0;
		totalTimeSec = 0.5f;
		while ( waitTimeSec < totalTimeSec ) {
			waitTimeSec += Time.fixedDeltaTime;
			yield return mWaitForFixedUpdate;
		}

		GameInfo.Instance.Send_ReqResetOptSelectGem( mGemData.GemUID, isChange, OnNetAutoGemSelect );

		_ResetBtn.isEnabled = true;
	}

	private void SelectOption( int index ) {
		mIsSelect = 0 <= index;
		mSelectOptionIndex = index;

		kOptEFFSpr.SetActive( 0 <= index );
		if ( kOptEFFSpr.gameObject.activeSelf ) {
			Vector3 pos = mOriginalEffPos;
			pos.y -= ( index * 50.0f );
			kOptEFFSpr.transform.localPosition = pos;
		}

		mAutoOptionID = -1;
		SetOptionTypeLabel( mAutoOptionID );
	}

	private void AutoStart() {
		if ( !IsPreInspectionCheck( isReset: true ) ) {
			if ( CheckFlag( eOptionType.AUTO ) ) {
				InitFlag( eOptionType.MAIN );
				Renewal();
			}
			return;
		}

		_GaugeParticle.gameObject.SetActive( false );

		GameInfo.Instance.Send_ReqResetOptGem( mGemData.GemUID, mSelectOptionIndex, OnNetAutoGemChange );
	}

	private bool IsBuying() {
		for ( int i = 0; i < mMatIdList.Count; i++ ) {
			if ( kMatItemList.Count <= i ) {
				break;
			}

			if ( !( mMatCountList[i] <= GameInfo.Instance.GetItemIDCount( mMatIdList[i] ) ) ) {
				return false;
			}
		}

		return true;
	}

	private bool IsSelectCheck( int selectOption ) {
		if ( mGemData == null ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 101007 ) );
			return false;
		}

		if ( mGemData.Wake <= selectOption ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3140 ) );
			return false;
		}

		return true;
	}

	private bool IsResetOptionCheck() {
		if ( !mIsSelect ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3138 ) );
			return false;
		}

		return true;
	}

	private bool IsPreInspectionCheck( bool isReset ) {
		if ( isReset && !IsResetOptionCheck() ) {
			return false;
		}

		GameTable.ItemReqList.Param itemReqListParam = GameInfo.Instance.GameTable.FindItemReqList( x => x.Group == mGemData.TableData.OptResetReqGroup );
		if ( itemReqListParam == null ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3002 ) );
			return false;
		}

		if ( !GameSupport.IsCheckGoods( eGOODSTYPE.GOLD, itemReqListParam.Gold ) ) {
			return false;
		}

		if ( !IsBuying() ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3003 ) );
			return false;
		}

		return true;
	}

	private bool IsOptionMaxValueForSelectIndex( int tID ) {
		if ( !mIsSelect ) {
			return false;
		}

		if ( tID == mGemData.RandOptID[mSelectOptionIndex] ) {
			GameTable.GemRandOpt.Param gemRandOptParam = mGemRandOptParamList.Find( x => x.ID == tID );
			if ( gemRandOptParam != null ) {
				if ( gemRandOptParam.RndStep <= mGemData.RandOptValue[mSelectOptionIndex] ) {
					return true;
				}
			}
		}

		return false;
	}

	private void SetOptionTypeLabel( int index ) {
		if ( index < 0 ) {
			_SelectOptionTypeLabel.textlocalize = FLocalizeString.Instance.GetText( 3353 );
			return;
		}

		int value = mGemRandOptParamList[index].Min + (int)( (float)mGemRandOptParamList[index].RndStep * mGemRandOptParamList[index].Value );
		float v = value / (float)eCOUNT.MAX_RATE_VALUE;

		if ( mGemRandOptParamList[index].EffectType.Contains( "Penetrate" ) || mGemRandOptParamList[index].EffectType.Contains( "Sufferance" ) ) {
			v /= 10.0f;
		}

		_SelectOptionTypeLabel.textlocalize = FLocalizeString.Instance.GetText( mGemRandOptParamList[index].Desc, v * 100.0f );
	}

	private void SetGemOptionIndex( int index, ref UIGemOptUnit gemOptUnit ) {
		if ( !gemOptUnit.gameObject.activeSelf ) {
			gemOptUnit.SetActive( true );
		}

		gemOptUnit.Opt( mGemData, index );
	}

	private void SetGemOptionValue( int index, int value, ref UIGemOptUnit gemOptUnit ) {
		if ( !gemOptUnit.gameObject.activeSelf ) {
			gemOptUnit.SetActive( true );
		}

		gemOptUnit.Opt( mGemData, index, value );
	}

	private void SetGemOptionValueAnim( int index, int value, bool isAnim, ref UIGemOptUnit gemOptUnit ) {
		if ( !gemOptUnit.gameObject.activeSelf ) {
			gemOptUnit.SetActive( true );
		}

		gemOptUnit.Opt( mGemData, index, value, isAnim );
	}

	private void ClearFlag() {
		mOptionType = 0;
	}

	private void InitFlag( eOptionType optionType ) {
		ClearFlag();
		AddFlag( optionType );
	}

	private void AddFlag( eOptionType optionType ) {
		mOptionType |= optionType;
	}

	private bool CheckFlag( eOptionType optionType ) {
		return ( mOptionType & optionType ) != eOptionType.NONE;
	}

	private void DeleteFlag( eOptionType optionType ) {
		mOptionType &= ~optionType;
	}
}