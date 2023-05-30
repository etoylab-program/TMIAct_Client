
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIBookWeaponInfoPopup : FComponent {
	public UITexture					kWeaponTex;
	public UISprite						kGradeSpr;
	public UILabel						kNameLabel;
	public UILabel						kDescLabel;
	public UIButton						kChangeBtn;
	public GameObject					LockChangeObj;
	public UITextList					kTextList;
	public UILabel						kBookNoLabel;
	public UIButton						kArrow_RBtn;
	public UIButton						kArrow_LBtn;
	public UITexture					kEquipCharTex;
	[SerializeField] private FList		_AvailableCharCharList;
	[SerializeField] private UILabel    _AvailableCharLabel;

	private GameTable.Weapon.Param              _weapontabledata;
	private WeaponBookData                      _weaponbookdata;
	private List<GameClientTable.Book.Param>    _havebooklist       = new List<GameClientTable.Book.Param>();
	private int                                 _nowIndex           = 0;
	private bool                                _bwake;
	private bool                                _bchange;
	private List<int>                           mListAvailableChar  = new List<int>();


	public override void Awake() {
		base.Awake();

		_AvailableCharCharList.EventGetItemCount = GetAvailableCharCount;
		_AvailableCharCharList.EventUpdate = UpdateAvailableCharListSlot;
	}

	public override void OnEnable() {
		InitComponent();
		base.OnEnable();

		kArrow_RBtn.gameObject.SetActive( true );
		kArrow_LBtn.gameObject.SetActive( true );

		int tableid = (int)UIValue.Instance.GetValue( UIValue.EParamType.BookItemID );

		// 보유 도감 리스트
		_havebooklist.Clear();
		_nowIndex = 0;
		_havebooklist = GameInfo.Instance.GameClientTable.FindAllBook( x => x.Group == (int)eBookGroup.Weapon );

		for( int idx = 0; idx < _havebooklist.Count; idx++ ) {
			if( _havebooklist[idx].ItemID == tableid ) {
				_nowIndex = idx;
				break;
			}
		}
	}

	public override void OnDisable() {
		if( AppMgr.Instance.IsQuit ) {
			return;
		}

		base.OnDisable();
		RenderTargetChar.Instance.DestroyRenderTarget();
	}

	public override void InitComponent() {
		int tableid = (int)UIValue.Instance.GetValue( UIValue.EParamType.BookItemID );
		_weapontabledata = GameInfo.Instance.GameTable.FindWeapon( tableid );
		_weaponbookdata = GameInfo.Instance.GetWeaponBookData( _weapontabledata.ID );

		if( _weaponbookdata != null ) {
			_bchange = _weaponbookdata.IsOnFlag( eBookStateFlag.MAX_WAKE_AND_LV );
		}
		else {
			_bchange = false;
		}

		_bwake = _bchange;

		RenderTargetWeapon.Instance.gameObject.SetActive( true );
		RenderTargetWeapon.Instance.InitRenderTargetWeapon( _weapontabledata.ID, -1, true );
		RenderTargetWeapon.Instance.ShowWeaponEffect( IsWakeMax() );

		GameClientTable.Book.Param data = GameInfo.Instance.GameClientTable.FindBook( x => x.Group == (int)eBookGroup.Weapon && x.ItemID == tableid );
		if( data == null ) {
			return;
		}

		kBookNoLabel.textlocalize = string.Format( FLocalizeString.Instance.GetText( 216 ), data.Num );
	}

	public override void Renewal( bool bChildren ) {
		base.Renewal( bChildren );

		kGradeSpr.spriteName = "itemgrade_L_" + _weapontabledata.Grade.ToString();
		FLocalizeString.SetLabel( kNameLabel, _weapontabledata.Name );

		_AvailableCharLabel.textlocalize = FLocalizeString.Instance.GetText( 1840 );
		mListAvailableChar.Clear();
		
		string[] split = Utility.Split( _weapontabledata.CharacterID, ',' );
		for( int i = 0; i < split.Length; i++ ) {
			mListAvailableChar.Add( Utility.SafeIntParse( split[i] ) );
		}

		_AvailableCharCharList.UpdateList();

		kTextList.textLabel.textlocalize = "";
		kTextList.Clear();
		kTextList.Add(Utility.AppendColorBBCodeString(FLocalizeString.Instance.GetText(_weapontabledata.Desc)));

		LockChangeObj.SetActive( !_bchange );

		if ( !AppMgr.Instance.configData.m_Network ) {
			LockChangeObj.SetActive( false );
		}
	}

	public void OnClick_BackBtn() {
		OnClickClose();
	}

	public override void OnClickClose() {
		LobbyUIManager.Instance.Renewal( "BookItemListPopup" );
		base.OnClickClose();
	}

	public void OnClick_ChangeBtn() {
		if( !GameInfo.Instance.GameConfig.TestMode && !AppMgr.Instance.Review ) {
			if( !_bchange ) {
				MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3062 ) );
				return;
			}
		}

		_bwake = !_bwake;
		RenderTargetWeapon.Instance.ShowWeaponEffect( _bwake );
	}

	public void OnClick_ViewBtn() {
		CharViewer.ShowWeaponViewPopup( "BookWeaponInfoPopup", kWeaponTex.gameObject, kWeaponTex.transform.parent );
	}

	public void OnClick_Arrow_LBtn() {
		int id = GetNextBookID(true);
		if( id != -1 ) {
			SendBookNewConfirm( id );
			UIValue.Instance.SetValue( UIValue.EParamType.BookItemID, id );
			InitComponent();
			Renewal( true );
		}
	}

	public void OnClick_Arrow_RBtn() {
		int id = GetNextBookID(false);
		if( id != -1 ) {
			SendBookNewConfirm( id );
			UIValue.Instance.SetValue( UIValue.EParamType.BookItemID, id );
			InitComponent();
			Renewal( true );
		}
	}

	public void OnClick_WeaponInfoBtn() {
		UIWeaponInfoPopup weaponInfoPopup = LobbyUIManager.Instance.GetUI<UIWeaponInfoPopup>( "WeaponInfoPopup" );
		if ( weaponInfoPopup == null ) {
			return;
		}

		UIValue.Instance.SetValue( UIValue.EParamType.WeaponUID, (long)-1 );
		UIValue.Instance.SetValue( UIValue.EParamType.WeaponTableID, _weapontabledata.ID );

		weaponInfoPopup.SetWakeMaxFromBook( IsWakeMax() );
		weaponInfoPopup.SetUIActive( true );
	}

	private bool IsWakeMax() {
		if ( !_bchange ) {
			return false;
		}

		if ( !_bwake ) {
			return false;
		}

		return true;
	}

	private int GetNextBookID( bool bleft ) {
		if( bleft ) {
			_nowIndex -= 1;
			if( _nowIndex < 0 ) {
				_nowIndex = _havebooklist.Count - 1;
			}
		}
		else {
			_nowIndex += 1;
			if( _nowIndex >= _havebooklist.Count ) {
				_nowIndex = 0;
			}
		}

		return _havebooklist[_nowIndex].ItemID;
	}

	private void SendBookNewConfirm( int id ) {
		GameTable.Weapon.Param weapontabledata = GameInfo.Instance.GameTable.FindWeapon(id);
		if( weapontabledata == null ) {
			return;
		}

		WeaponBookData weaponbookdata = GameInfo.Instance.GetWeaponBookData(weapontabledata.ID);
		if( weaponbookdata == null ) {
			return;
		}

		if( weaponbookdata.IsOnFlag( eBookStateFlag.NEW_CHK ) ) {
			return;
		}

		GameInfo.Instance.Send_ReqBookNewConfirm( (int)eBookGroup.Weapon, weaponbookdata.TableID, null );
	}

	private int GetAvailableCharCount() {
		return mListAvailableChar.Count;
	}

	private void UpdateAvailableCharListSlot( int index, GameObject slotObject ) {
		if( mListAvailableChar.Count == 0 ) {
			return;
		}

		UIGachaDetailCharListSlot slot = slotObject.GetComponent<UIGachaDetailCharListSlot>();
		if( slot == null ) {
			return;
		}

		slot.ParentGO = gameObject;
		GameTable.Character.Param param = GameInfo.Instance.GameTable.FindCharacter( mListAvailableChar[index] );
		slot.UpdateSlot( index, param == null ? null : param.Icon );
	}
}
