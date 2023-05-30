
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UITradableListSlot : FSlot {
	public UISprite					kGradeIconSpr;
	public UILabel					kNameLabel;
	public UITexture				kWeaponCharTex;
	public UISprite					kCardTypeSpr;
	[SerializeField] private FList	_AvailableCharList;

	private GameTable.Card.Param	CardParam			= null;
	private GameTable.Weapon.Param	WeaponParam			= null;
	private RewardData				rewardData			= null;
	private List<int>               mListAvailableChar	= new List<int>();


	public void UpdateSlot( GameTable.Card.Param param ) {
		kWeaponCharTex.gameObject.SetActive( false );
		kCardTypeSpr.gameObject.SetActive( false );

		rewardData = null;
		CardParam = param;
		SetState( false );
		if( CardParam == null )
			return;

		SetState( true );
		kGradeIconSpr.spriteName = string.Format( "itemgrade_L_{0}", CardParam.Grade );
		kNameLabel.textlocalize = FLocalizeString.Instance.GetText( CardParam.Name );

		rewardData = new RewardData();
		rewardData.Type = (int)eREWARDTYPE.CARD;
		rewardData.Index = CardParam.ID;

		GameTable.Card.Param cardTableData = GameInfo.Instance.GameTable.FindCard(x => x.ID == CardParam.ID);
		if( cardTableData != null ) {
			kCardTypeSpr.gameObject.SetActive( true );
			kCardTypeSpr.spriteName = GameSupport.GetCardTypeSpriteName( (eSTAGE_CONDI)cardTableData.Type );
		}
	}

	public void UpdateSlot( GameTable.Weapon.Param param ) {
		kWeaponCharTex.gameObject.SetActive( false );
		kCardTypeSpr.gameObject.SetActive( false );

		rewardData = null;
		WeaponParam = param;
		SetState( false );
		if( WeaponParam == null )
			return;

		SetState( true );
		kGradeIconSpr.spriteName = string.Format( "itemgrade_L_{0}", WeaponParam.Grade );
		kNameLabel.textlocalize = FLocalizeString.Instance.GetText( WeaponParam.Name );

		rewardData = new RewardData();
		rewardData.Type = (int)eREWARDTYPE.WEAPON;
		rewardData.Index = WeaponParam.ID;

		GameTable.Weapon.Param weaponTableData = GameInfo.Instance.GameTable.FindWeapon(x => x.ID == WeaponParam.ID);
		if( weaponTableData != null ) {
			string[] split = Utility.Split( weaponTableData.CharacterID, ',' );
			mListAvailableChar.Clear();

			for( int i = 0; i < split.Length; i++ ) {
				int charTableId = Utility.SafeIntParse( split[i] );
				mListAvailableChar.Add( charTableId );
			}

			_AvailableCharList.UpdateList();

			/*
            GameTable.Character.Param charTableData = GameInfo.Instance.GameTable.FindCharacter(x => x.ID == weaponTableData.CharacterID);
            if (charTableData != null)
            {
                kWeaponCharTex.gameObject.SetActive(true);
                kWeaponCharTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_Chara_" + charTableData.Icon + ".png");
            }
            */
		}
	}

	public void OnClick_DetailInfo() {

		if( rewardData == null )
			return;

		if( rewardData.Type == (int)eREWARDTYPE.GOODS )
			return;

		GameSupport.OpenRewardTableDataInfoPopup( rewardData );

	}

	private void SetState( bool state ) {
		kGradeIconSpr.SetActive( state );
		kNameLabel.SetActive( state );
	}

	private void Awake() {
		_AvailableCharList.EventGetItemCount = GetAvailableCharCount;
		_AvailableCharList.EventUpdate = UpdateAvailableCharListSlot;
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
