
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIAwakenSkillLevelUpPopup : FComponent {
	[Header( "[Property]" )]
	public UITexture		TexIcon;
	public UILabel			LbName;
	public UILabel			LbCurrentDesc;
	public UILabel			LbCurrentLevel;
	public UILabel			LbNextDesc;
	public UILabel			LbNextLevel;
	public UILabel			LbAwakenPoint;
	public UILabel			LbGold;
	public GameObject[]		OnOffObj;
	public ParticleSystem	_UpgradeEff;

	private AwakenSkillInfo				mCurrentAwakenSkillInfo			= null;
	private GameTable.AwakeSkill.Param	mCurrentParam					= null;
	private int							mCurrentAwakenSkillSlotIndex	= -1;
	private GameTable.ItemReqList.Param	mParamItemReqList				= null;
	private bool						mbLockBtn						= false;


	public override void OnEnable() {
		base.OnEnable();
		mbLockBtn = false;
	}

	public override void Renewal( bool bChildren = false ) {
		base.Renewal( bChildren );

		ActiveOnOffObj( true );

		UITopPanel topPanel = LobbyUIManager.Instance.GetUI<UITopPanel>( "TopPanel" );
		if ( topPanel ) {
			topPanel.SetTopStatePlay( UITopPanel.eTOPSTATE.AWAKEN_SKILL );
		}

		LbName.textlocalize = FLocalizeString.Instance.GetText( mCurrentParam.Name );

		int currentLevel = 0;
		if ( mCurrentAwakenSkillInfo == null ) {
			LbCurrentLevel.textlocalize = FLocalizeString.Instance.GetText( 1634 );
			LbNextLevel.textlocalize = "Lv.1";
		}
		else {
			currentLevel = mCurrentAwakenSkillInfo.Level;

			if ( currentLevel < mCurrentParam.MaxLevel ) {
				LbCurrentLevel.textlocalize = string.Format( "Lv.{0}", mCurrentAwakenSkillInfo.Level );
			}
			else {
				LbCurrentLevel.textlocalize = "MAX";
				ActiveOnOffObj( false );
			}
		}

		TexIcon.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", "Icon/CharAwakenSkill/" + mCurrentParam.Icon + ".png" );

		GameClientTable.BattleOptionSet.Param paramBOSet1 = GameInfo.Instance.GameClientTable.FindBattleOptionSet( mCurrentParam.SptAddBOSetID1 );

		if ( currentLevel > 0 ) {
			if ( paramBOSet1 != null ) {
				float incValue = ( currentLevel - 1 ) * paramBOSet1.BOFuncIncValue;
				LbCurrentDesc.textlocalize = FLocalizeString.Instance.GetText( mCurrentParam.Desc, paramBOSet1.BOFuncValue + incValue );
			}
			else {
				float incValue = ( currentLevel - 1 ) * mCurrentParam.SptSvrOptIncValue;
				LbCurrentDesc.textlocalize = FLocalizeString.Instance.GetText( mCurrentParam.Desc, mCurrentParam.SptSvrOptValue + incValue );
			}
		}
		else {
			LbCurrentDesc.textlocalize = "";
		}

		if ( currentLevel >= mCurrentParam.MaxLevel ) {
			LbNextLevel.textlocalize = "MAX";
			LbNextDesc.textlocalize = "-";
		}
		else {
			int nextLevel = currentLevel + 1;
			if ( nextLevel >= mCurrentParam.MaxLevel ) {
				LbNextLevel.textlocalize = "MAX";
			}
			else {
				LbNextLevel.textlocalize = string.Format( "Lv.{0}", nextLevel );
			}

			if ( paramBOSet1 != null ) {
				float incValue = ( nextLevel - 1 ) * paramBOSet1.BOFuncIncValue;
				LbNextDesc.textlocalize = FLocalizeString.Instance.GetText( mCurrentParam.Desc, paramBOSet1.BOFuncValue + incValue );
			}
			else {
				float incValue = ( nextLevel - 1 ) * mCurrentParam.SptSvrOptIncValue;
				LbNextDesc.textlocalize = FLocalizeString.Instance.GetText( mCurrentParam.Desc, mCurrentParam.SptSvrOptValue + incValue );
			}
		}

		mParamItemReqList = GameInfo.Instance.GameTable.FindItemReqList( x => x.Group == mCurrentParam.ItemReqListID && x.Level == currentLevel );
		if ( mParamItemReqList != null ) {
			int formatID = mParamItemReqList.Gold <= GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.GOLD] ? (int)eTEXTID.GOODSTEXT_W : (int)eTEXTID.GOODSTEXT_R;
			FLocalizeString.SetLabel( LbGold, formatID, mParamItemReqList.Gold );

			formatID = mParamItemReqList.GoodsValue <= GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.AWAKEPOINT] ? (int)eTEXTID.GOODSTEXT_W : (int)eTEXTID.GOODSTEXT_R;
			FLocalizeString.SetLabel( LbAwakenPoint, formatID, mParamItemReqList.GoodsValue );
		}
		else {
			ActiveOnOffObj( false );
		}
	}

	public void Set( int awakenSkillSlotIndex, AwakenSkillInfo awakenSkillInfo, GameTable.AwakeSkill.Param param ) {
		mCurrentAwakenSkillSlotIndex = awakenSkillSlotIndex;
		mCurrentAwakenSkillInfo = awakenSkillInfo;
		mCurrentParam = param;
	}

	public void OnBtnUpgrade() {
		if ( mbLockBtn ) {
			return;
		}

		if ( mParamItemReqList.GoodsValue > GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.AWAKEPOINT] ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3010 ) );
			return;
		}

		if ( !GameSupport.IsCheckGoods( eGOODSTYPE.GOLD, mParamItemReqList.Gold ) ) {
			return;
		}

		mbLockBtn = true;
		GameInfo.Instance.Send_ReqLvUpUserSkill( mCurrentParam.ID, OnReqLvUpUserSkill );
	}

	private void ActiveOnOffObj( bool active ) {
		for ( int i = 0; i < OnOffObj.Length; i++ ) {
			OnOffObj[i].SetActive( active );
		}
	}

	private void OnReqLvUpUserSkill( int result, PktMsgType pktmsg ) {
		Invoke( "ReleaseBtnLock", GameInfo.Instance.GameConfig.SkillLevelUpDelayTimeSec );

		if ( result != 0 ) {
			return;
		}

		SoundManager.Instance.PlaySnd( SoundManager.eSoundType.FX, 12, 1.0f );

		UICharAwakenSkillPopup popup = LobbyUIManager.Instance.GetUI<UICharAwakenSkillPopup>( "CharAwakenSkillPopup" );
		if ( popup ) {
			popup.UpgradeSlotIndex = mCurrentAwakenSkillSlotIndex;
		}

		LobbyUIManager.Instance.Renewal( "TopPanel" );
		LobbyUIManager.Instance.Renewal( "GoodsPopup" );
		LobbyUIManager.Instance.Renewal( "CharInfoPanel" );
		LobbyUIManager.Instance.Renewal( "CharAwakenSkillPopup" );

		mCurrentAwakenSkillInfo = GameInfo.Instance.UserData.ListAwakenSkillData.Find( x => x.TableId == mCurrentParam.ID );
		Renewal();

		_UpgradeEff.Play();
		//base.OnClickClose();
	}

	private void ReleaseBtnLock() {
		mbLockBtn = false;
	}
}
