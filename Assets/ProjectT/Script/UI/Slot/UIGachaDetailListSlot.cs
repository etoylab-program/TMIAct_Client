
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIGachaDetailListSlot : FSlot {
	public UISprite					kBGSpr;
	public UISprite					kGradeSpr;
	public UILabel					kNameLabel;
	public UILabel					kRateLabel;
	public UISprite					kRateUpSpr;
	public UITexture				kWeaponCharTex;
	public UISprite					kCardTypeSpr;
	[SerializeField] private FList	_CharList;

	private int									_index				= 0;
	private UIGachaDetailPopup.GachaDetailData	_data				= null;
	private List<int>							mListAvailableChar	= new List<int>();


	public void UpdateSlot( int index, UIGachaDetailPopup.GachaDetailData data )
	{
		_index = index;
		_data = data;

		kGradeSpr.spriteName = "itemgrade_S_" + _data.Grade.ToString();
		kGradeSpr.MakePixelPerfect();

		kNameLabel.textlocalize = _data.Name;
		kRateLabel.textlocalize = _data.Percent;

		if( _data.Value != 0 ) {
			kRateUpSpr.gameObject.SetActive( true );
			kNameLabel.color = GameInfo.Instance.GameConfig.GachaPickupFontColor;
			kNameLabel.effectStyle = UILabel.Effect.Outline8;
			kRateLabel.color = GameInfo.Instance.GameConfig.GachaPickupFontColor;
			kRateLabel.effectStyle = UILabel.Effect.Outline8;
		}
		else {
			kRateUpSpr.gameObject.SetActive( false );
			kNameLabel.color = GameInfo.Instance.GameConfig.GachaNameColor;
			kNameLabel.effectStyle = UILabel.Effect.None;
			kRateLabel.color = GameInfo.Instance.GameConfig.GachaRateColor;
			kRateLabel.effectStyle = UILabel.Effect.None;
		}

		kWeaponCharTex.gameObject.SetActive( _data.ProductType == (int)eREWARDTYPE.WEAPON );

		kWeaponCharTex.gameObject.SetActive( false );
		kCardTypeSpr.gameObject.SetActive( false );

		mListAvailableChar.Clear();
		_CharList.gameObject.SetActive( false );

		if( _data.ProductType == (int)eREWARDTYPE.WEAPON ) {
			GameTable.Weapon.Param weaponTableData = GameInfo.Instance.GameTable.FindWeapon(x => x.ID == _data.ProductIndex);
			if( weaponTableData != null ) {
				string[] split = Utility.Split( weaponTableData.CharacterID, ',' );

				for( int i = 0; i < split.Length; i++ ) {
					int charTableId = Utility.SafeIntParse( split[i] );
					mListAvailableChar.Add( charTableId );
				}
				/*
                GameTable.Character.Param charTableData = GameInfo.Instance.GameTable.FindCharacter(x => x.ID == weaponTableData.CharacterID);
                if (charTableData != null)
                {
                    kWeaponCharTex.gameObject.SetActive(true);
                    kWeaponCharTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_Chara_" + charTableData.Icon + ".png");
                }
                */
			}

			_CharList.gameObject.SetActive( true );
			_CharList.UpdateList();
		}
		else if( _data.ProductType == (int)eREWARDTYPE.CARD ) {
			GameTable.Card.Param cardTableData = GameInfo.Instance.GameTable.FindCard(x => x.ID == _data.ProductIndex);
			if( cardTableData != null ) {
				kCardTypeSpr.gameObject.SetActive( true );
				kCardTypeSpr.spriteName = GameSupport.GetCardTypeSpriteName( (eSTAGE_CONDI)cardTableData.Type );
			}
		}
	}

	public void OnClick_Slot() {
		GameSupport.OpenRewardTableDataInfoPopup( new RewardData( _data.ProductType, _data.ProductIndex, 0 ) );
	}

	private void Awake() {
		_CharList.EventGetItemCount = GetAvailableCharCount;
		_CharList.EventUpdate = UpdateAvailableCharListSlot;
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
