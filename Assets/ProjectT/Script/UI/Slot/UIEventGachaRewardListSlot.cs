using UnityEngine;
using System.Collections;

public class UIEventGachaRewardListSlot : FSlot {
	public UILabel kNameLabel;
	public UILabel kStockLabel;
	public UILabel kcountLabel;
	public UISprite kcountbgSpr;
	public GameObject kNormal;
	public GameObject kRare;
	public UILabel kRareLabel;

	public UISprite kBGSpr;
	public UISprite kIconSpr;
	public UITexture kIconTex;
	public UISprite kFrmGradeSpr;
	public UISprite kSelSpr;
	public UISprite kLockSpr;
	public UISprite kNewSpr;
	public UISprite kGradeSpr;
	public UISprite kInActiveSpr;
	public UILabel kCountLabel;
	public UILabel kSortLabel;
	public UISprite kTypeSpr;

	public UISprite kBingoEventCountSpr;
	public UILabel kItemNameLabel;
	public UILabel kMissionLabel;

	private RewardData mRewardData;


	public void UpdateSlot( GameTable.EventResetReward.Param eventResetRewardParam, int count, bool isDataTable ) {
		if ( eventResetRewardParam == null ) {
			return;
		}

		kNewSpr.gameObject.SetActive( false );
		kLockSpr.gameObject.SetActive( false );
		kSelSpr.gameObject.SetActive( false );
		kSortLabel.gameObject.SetActive( false );
		kInActiveSpr.gameObject.SetActive( false );
		// kStockLabel.SetActive( false );
		// kcountbgSpr.SetActive( false );
		kcountLabel.SetActive( true );
		// kNameLabel.SetActive( false );
		kBingoEventCountSpr.SetActive( false );
		kItemNameLabel.SetActive( false );
		kMissionLabel.SetActive( false );

		kNormal.SetActive( eventResetRewardParam.ResetFlag != 1 );
		kRare.SetActive( eventResetRewardParam.ResetFlag == 1 );

		mRewardData = new RewardData( 0, eventResetRewardParam.ProductType, eventResetRewardParam.ProductIndex, eventResetRewardParam.ProductValue, false );
		GameSupport.UpdateSlotByRewardData( ParentGO, this.gameObject, mRewardData, kFrmGradeSpr, kGradeSpr, kTypeSpr, kBGSpr, kCountLabel, kIconTex, kIconSpr, null );
		FLocalizeString.SetLabel( kNameLabel, GameSupport.GetProductName( mRewardData ) );

		kCountLabel.textlocalize = FLocalizeString.Instance.GetText( (int)eTEXTID.GOODSTEXT, eventResetRewardParam.ProductValue );

		if ( !isDataTable ) {
			if ( count <= 0 ) {
				kcountLabel.textlocalize = $"[FF0000FF]{count}[-]/{eventResetRewardParam.RewardCnt}";
				kInActiveSpr.gameObject.SetActive( true );
			}
			else {
				kcountLabel.textlocalize = FLocalizeString.Instance.GetText( 276, count, eventResetRewardParam.RewardCnt );
			}
		}
		else {
			kcountLabel.textlocalize = FLocalizeString.Instance.GetText( 276, eventResetRewardParam.RewardCnt, eventResetRewardParam.RewardCnt );
		}
	}

	public void UpdateSlot( GameTable.Random.Param randomParam, int receiveCount ) {
		if ( randomParam == null ) {
			return;
		}

		kNewSpr.SetActive( false );
		kLockSpr.SetActive( false );
		kSelSpr.SetActive( false );
		kSortLabel.SetActive( false );
		kInActiveSpr.SetActive( false );
		kStockLabel.SetActive( false );
		kcountbgSpr.SetActive( false );
		kcountLabel.SetActive( false );
		kNameLabel.SetActive( false );
		kBingoEventCountSpr.SetActive( false );
		kItemNameLabel.SetActive( true );
		kMissionLabel.SetActive( true );

		kNormal.SetActive( randomParam.Value != 0 );
		kRare.SetActive( randomParam.Value == 0 );

		mRewardData = new RewardData( 0, randomParam.ProductType, randomParam.ProductIndex, randomParam.ProductValue, false );
		GameSupport.UpdateSlotByRewardData( ParentGO, this.gameObject, mRewardData, kFrmGradeSpr, kGradeSpr, kTypeSpr, kBGSpr, kCountLabel, kIconTex, kIconSpr, null );
		FLocalizeString.SetLabel( kItemNameLabel, GameSupport.GetProductName( mRewardData ) );

		if ( randomParam.Value == 0 ) {
			kMissionLabel.textlocalize = FLocalizeString.Instance.GetText( 3288 );
		}
		else {
			kMissionLabel.textlocalize = FLocalizeString.Instance.GetText( 3289, randomParam.Value );
		}
	}

	public void OnClick_Slot() {

	}

	public void OnClick_ItemSlot() {
		if ( mRewardData == null ) {
			return;
		}

		if ( mRewardData.Type == (int)eREWARDTYPE.GOODS ) {
			return;
		}

		GameSupport.OpenRewardTableDataInfoPopup( mRewardData );
	}
}
