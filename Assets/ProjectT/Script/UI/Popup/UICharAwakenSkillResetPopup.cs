
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UICharAwakenSkillResetPopup : FComponent {
	[Header( "[Property]" )]
	public UILabel _AwakenSkillPointLabel;
	public UILabel _TicketLabel;
	public UILabel _CashLabel;

	private int			mTotalAwakenSkillPoint	= 0;
	private ItemData	mItemData				= null;


	public override void Renewal( bool bChildren = false ) {
		base.Renewal( bChildren );

		mTotalAwakenSkillPoint = 0;

		for ( int i = 0; i < GameInfo.Instance.UserData.ListAwakenSkillData.Count; i++ ) {
			AwakenSkillInfo info = GameInfo.Instance.UserData.ListAwakenSkillData[i];

			GameTable.AwakeSkill.Param param = GameInfo.Instance.GameTable.FindAwakeSkill( info.TableId );
			if ( param == null ) {
				Debug.LogError( info.TableId + "번 각성 스킬 테이블 데이터가 없습니다." );
				continue;
			}

			List<GameTable.ItemReqList.Param> find = GameInfo.Instance.GameTable.FindAllItemReqList( x => x.Group == param.ItemReqListID && x.Level < info.Level );
			for ( int j = 0; j < find.Count; j++ ) {
				mTotalAwakenSkillPoint += find[j].GoodsValue;
			}
		}

		mItemData = GameInfo.Instance.GetItemData( GameInfo.Instance.GameConfig.AwakeSkillClearItemID );

		_TicketLabel.textlocalize = "x1";
		_CashLabel.textlocalize = string.Format( "x{0}", GameInfo.Instance.GameConfig.AwakeSkillClearCash.ToString() );
		_AwakenSkillPointLabel.textlocalize = mTotalAwakenSkillPoint.ToString();
	}

	public void OnBtnUseTicket() {
		if ( mItemData == null ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3370 ) );
		}
		else {
			MessagePopup.OK( FLocalizeString.Instance.GetText( 1631 ), FLocalizeString.Instance.GetText( 1632 ),
							 mTotalAwakenSkillPoint, mItemData.ItemUID, OnUseTicket );
		}
	}

	public void OnBtnUseCash() {
		if ( !GameSupport.IsCheckGoods( eGOODSTYPE.CASH, GameInfo.Instance.GameConfig.AwakeSkillClearCash ) ) {
			return;
		}

		GameInfo.Instance.Send_ReqResetUserSkill( OnReqResetAwakenSkill );
	}

	public void OnBtnReset() {
		GameInfo.Instance.Send_ReqResetUserSkill( OnReqResetAwakenSkill );
	}

	private void OnUseTicket() {
		GameInfo.Instance.Send_ReqUseItem( mItemData.ItemUID, 1, 0, OnReqResetAwakenSkill );
	}

	private void OnReqResetAwakenSkill( int result, PktMsgType pktmsg ) {
		if ( result != 0 ) {
			return;
		}

		LobbyUIManager.Instance.Renewal( "TopPanel" );
		LobbyUIManager.Instance.Renewal( "GoodsPopup" );
		LobbyUIManager.Instance.Renewal( "CharInfoPanel" );
		LobbyUIManager.Instance.Renewal( "CharAwakenSkillPopup" );

		OnClickClose();
	}
}
