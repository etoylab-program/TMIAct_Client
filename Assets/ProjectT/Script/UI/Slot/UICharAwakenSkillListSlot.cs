
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UICharAwakenSkillListSlot : FSlot {
	[Header( "[Property]" )]
	public UITexture		TexIcon;
	public UILabel			LbName;
	public UILabel			LbDesc;
	public UILabel			LbLevel;
	public ParticleSystem	PsUpgrade;

	private int							mIndex					= -1;
	private AwakenSkillInfo				mCurrentAwakenSkillInfo	= null;
	private GameTable.AwakeSkill.Param	mCurrentParam			= null;
	private bool						mbMaxLevel				= false;


	public void UpdateSlot( int index, GameTable.AwakeSkill.Param param, bool isUpgrade ) {
		mIndex = index;
		mCurrentParam = param;
		mCurrentAwakenSkillInfo = GameInfo.Instance.UserData.ListAwakenSkillData.Find( x => x.TableId == param.ID );

		mbMaxLevel = false;

		TexIcon.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", "Icon/CharAwakenSkill/" + param.Icon + ".png" );
		LbName.textlocalize = FLocalizeString.Instance.GetText( param.Name );

		GameClientTable.BattleOptionSet.Param paramBOSet1 = GameInfo.Instance.GameClientTable.FindBattleOptionSet( param.SptAddBOSetID1 );
		int level = mCurrentAwakenSkillInfo == null ? 1 : mCurrentAwakenSkillInfo.Level;

		if ( paramBOSet1 != null ) {
			float incValue = ( level - 1 ) * paramBOSet1.BOFuncIncValue;
			LbDesc.textlocalize = FLocalizeString.Instance.GetText( param.Desc, paramBOSet1.BOFuncValue + incValue );
		}
		else {
			float incValue = ( level - 1 ) * mCurrentParam.SptSvrOptIncValue;
			LbDesc.textlocalize = FLocalizeString.Instance.GetText( param.Desc, mCurrentParam.SptSvrOptValue + incValue );
		}

		if ( mCurrentAwakenSkillInfo == null ) {
			LbLevel.textlocalize = FLocalizeString.Instance.GetText( 1634 );
		}
		else {
			if ( mCurrentAwakenSkillInfo.Level >= param.MaxLevel ) {
				mbMaxLevel = true;
				LbLevel.textlocalize = "MAX";
			}
			else {
				LbLevel.textlocalize = string.Format( "Lv.{0}", mCurrentAwakenSkillInfo.Level );
			}
		}

		/*
		if ( isUpgrade ) {
			PsUpgrade.Play();
		}
		*/
	}

	public void OnBtnClick() {
		if ( mbMaxLevel ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3004 ) );
		}
		else {
			UIAwakenSkillLevelUpPopup popup = LobbyUIManager.Instance.GetUI<UIAwakenSkillLevelUpPopup>( "AwakenSkillLevelUpPopup" );
			if ( popup ) {
				UICharAwakenSkillPopup parentPopup = ParentGO.GetComponent<UICharAwakenSkillPopup>();
				if ( parentPopup ) {
					parentPopup.SaveListFocus();
				}

				popup.Set( mIndex, mCurrentAwakenSkillInfo, mCurrentParam );
				LobbyUIManager.Instance.ShowUI( "AwakenSkillLevelUpPopup", true );
			}
		}
	}
}
