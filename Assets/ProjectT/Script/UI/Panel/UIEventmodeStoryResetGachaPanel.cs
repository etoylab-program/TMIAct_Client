using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEventmodeStoryResetGachaPanel : FComponent {
	[Header( "UIEventmodeStoryResetGachaPanel" )]
	[SerializeField] private FList _EventGachaRewardListInstance;

	[SerializeField] private UISprite _DisResetSpr;

	[SerializeField] private UITexture _IconTex;
	[SerializeField] private UITexture _BgTex;
	[SerializeField] private UITexture _OnceGachaTex;
	[SerializeField] private UITexture _MultiGachaTex;

	[SerializeField] private UILabel _CountLabel;
	[SerializeField] private UILabel _BoxLabel;
	[SerializeField] private UILabel _GoodUnitLabel;
	[SerializeField] private UILabel _OnceGachaLabel;
	[SerializeField] private UILabel _MultiGachaLabel;
	[SerializeField] private UILabel _MultiGachaCount;

	private EventSetData mEventSetData;
	private List<GameTable.EventResetReward.Param> mEventResetRewardParamList = new List<GameTable.EventResetReward.Param>();
	private List<CardBookData> mPreCardBookList = new List<CardBookData>();

	private int mEventID;
	private int mMaxRewardItemCount;
	private int mEventItemCount;
	private int mMultiCount;

	private bool mIsRareStop;

	public override void Awake() {
		base.Awake();

		_EventGachaRewardListInstance.EventUpdate = OnEventRewardListUpdate;
		_EventGachaRewardListInstance.EventGetItemCount = OnEventRewardListGetItemCount;
		_EventGachaRewardListInstance.UpdateList();
	}

	public override void OnEnable() {
		InitComponent();
		base.OnEnable();
	}

	public override void InitComponent() {
		mEventID = (int)UIValue.Instance.GetValue( UIValue.EParamType.EventID );

		SetEventRewardBase();

		mEventItemCount = 0;
		mMultiCount = 2;

		SetRareStop( true );
	}

	public override void Renewal( bool bChildren = false ) {
		base.Renewal( bChildren );

		SetRewardMaxCount();

		mEventItemCount = GameInfo.Instance.GetItemIDCount( mEventSetData.TableData.EventItemID1 );
		_GoodUnitLabel.textlocalize = mEventItemCount.ToString();

		_OnceGachaLabel.textlocalize = GameInfo.Instance.GameConfig.EvtResetGachaReqCnt.ToString();
		_CountLabel.textlocalize = FLocalizeString.Instance.GetText( (int)eTEXTID.GOODSTEXT, mEventSetData.Count + 1 );
		_BoxLabel.textlocalize = FLocalizeString.Instance.GetText( 1309, mMaxRewardItemCount, GetTotalRewardItemCount() );
		_BoxLabel.transform.localPosition = new Vector3( _CountLabel.transform.localPosition.x + _CountLabel.printedSize.x + 5, _BoxLabel.transform.localPosition.y, 0 );

		SetResetBtn();
		SetMultiGachaBtn();

		_EventGachaRewardListInstance.SpringSetFocus( 0, isImmediate: true );
		_EventGachaRewardListInstance.RefreshAllItem();
	}

	public void OnClick_resetBtn() {
		if ( _DisResetSpr.gameObject.activeSelf ) {
			GameTable.EventResetReward.Param flagCheck = mEventResetRewardParamList.Find( x => x.ResetFlag == 1 );
			if ( flagCheck != null ) {
				RewardData reward = new RewardData( 0, flagCheck.ProductType, flagCheck.ProductIndex, flagCheck.ProductValue, false );
				MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3133, GameSupport.GetProductName( reward ) ) );
			}
			else {
				MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3134 ) );
			}
		}
		else {
			if ( mMaxRewardItemCount > 0 ) {
				MessagePopup.OKCANCEL( eTEXTID.OK, 3135, SetEventRewardReset, null, false );
			}
			else {
				SetEventRewardReset();
			}
		}
	}

	public void OnClick_listBtn() {
		LobbyUIManager.Instance.ShowUI( "EventmodeStoryResetGachaPopup", true );
	}

	public void OnClick_OnceGachaBtn() {
		EventGacha( 1 );
	}

	public void OnClick_MultiGachaBtn() {
		bool isAuto = GameInfo.Instance.GameConfig.EvtResetGachaMaxNum < mMultiCount;

		if ( isAuto ) {
			EventAutoGacha();
		}
		else {
			EventGacha( mMultiCount );
		}
	}

	public void OnClick_PlusTenBtn() {
		if ( !IsTenOverGacha() ) {
			return;
		}

		if ( mMaxRewardItemCount <= 0 ) {
			return;
		}

		mMultiCount += 10;

		int maxRewardCount = GetMaxGachaCount();
		if ( maxRewardCount < mMultiCount ) {
			mMultiCount = maxRewardCount;
		}

		if ( mMaxRewardItemCount < mMultiCount ) {
			mMultiCount = mMaxRewardItemCount;
		}

		SetMultiGachaLabel();
	}

	public void OnClick_MinusTenBtn() {
		if ( !IsTenOverGacha() ) {
			return;
		}

		if ( mMaxRewardItemCount < GameInfo.Instance.GameConfig.EvtResetGachaMaxNum ) {
			return;
		}

		mMultiCount -= 10;

		if ( mMultiCount < GameInfo.Instance.GameConfig.EvtResetGachaMaxNum ) {
			mMultiCount = GameInfo.Instance.GameConfig.EvtResetGachaMaxNum;
		}

		SetMultiGachaLabel();
	}

	public void SetRareStop( bool isRareStop ) {
		mIsRareStop = isRareStop;
	}

	private bool HaveTicketCheck( int count ) {
		int reqCnt = GameInfo.Instance.GameConfig.EvtResetGachaReqCnt * count;

		if ( mEventItemCount < GameInfo.Instance.GameConfig.EvtResetGachaReqCnt || reqCnt > mEventItemCount || reqCnt < GameInfo.Instance.GameConfig.EvtResetGachaReqCnt ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3136 ) );
			return false;
		}

		if ( mMaxRewardItemCount <= 0 ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3137 ) );
			return false;
		}

		if ( mMaxRewardItemCount < count ) {
			//보상 초과, 이와 같은 경우는 매우 제한적, 발생 빈도도 낮고 설명 문구 표현도 어려워서 별도 메시지없이 return 처리만
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3242 ) );
			return false;
		}

		int state = GameSupport.GetJoinEventState( mEventID );
		if ( state < (int)eEventState.EventNone ) {
			if ( state == (int)eEventState.EventNotStart ) {
				MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3052 ) );
			}
			else {
				MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3053 ) );
			}
			return false;
		}

		return true;
	}

	private void EventGacha( int number ) {
		if ( !HaveTicketCheck( number ) ) {
			return;
		}

		mPreCardBookList.Clear();
		mPreCardBookList.AddRange( GameInfo.Instance.CardBookList );

		GameInfo.Instance.Send_ReqEventRewardTake( mEventSetData.TableID, number, mEventSetData.RewardStep, 0, OnNetEventRewardTake );
	}

	private void EventAutoGacha() {
		if ( !HaveTicketCheck( mMultiCount ) ) {
			return;
		}

		SetRareStop( true );

		mPreCardBookList.Clear();
		mPreCardBookList.AddRange( GameInfo.Instance.CardBookList );

		int index = mEventResetRewardParamList.FindIndex( x => x.ResetFlag == 1 );
		int rareRemainCount = 0;
		if ( 0 <= index && index < mEventSetData.RewardItemCount.Count ) {
			rareRemainCount = mEventSetData.RewardItemCount[index];
		}

		if ( 0 < rareRemainCount ) {
			string descStr = FLocalizeString.Instance.GetText( 3364, mMultiCount * GameInfo.Instance.GameConfig.EvtResetGachaReqCnt, mMultiCount, GameInfo.Instance.GameConfig.EvtResetGachaMaxNum );
			MessagePopup.YNEventGacha( eTEXTID.OK, descStr, StartAuto, OnEventTabSelect );
		}
		else {
			SetRareStop( false );
			StartAuto();
		}
	}

	private void StartAuto() {
		UIEventmodeStoryAutoGachaPopup eventmodeStoryAutoGachaPopup = LobbyUIManager.Instance.GetUI<UIEventmodeStoryAutoGachaPopup>( "EventmodeStoryAutoGachaPopup" );
		if ( eventmodeStoryAutoGachaPopup == null ) {
			return;
		}
		eventmodeStoryAutoGachaPopup.SetData( mIsRareStop, mMultiCount, ref mEventSetData, ref mEventResetRewardParamList, ref mPreCardBookList, ref _BgTex );
		eventmodeStoryAutoGachaPopup.SetUIActive( true );
	}

	private bool OnEventTabSelect( int nSelect, SelectEvent type ) {
		if ( type == SelectEvent.Enable ) {
			return false;
		}

		SetRareStop( nSelect.Equals( (int)eToggleType.On ) );

		return true;
	}

	private void OnEventRewardListUpdate( int index, GameObject slotObject ) {
		UIEventGachaRewardListSlot slot = slotObject.GetComponent<UIEventGachaRewardListSlot>();
		if ( slot == null ) {
			return;
		}

		Log.Show( mEventResetRewardParamList.Count + " / " + index, Log.ColorType.Red );

		if ( slot.ParentGO == null ) {
			slot.ParentGO = gameObject;
		}

		GameTable.EventResetReward.Param eventResetRewardParam = null;
		if ( 0 <= index && index < mEventResetRewardParamList.Count ) {
			eventResetRewardParam = mEventResetRewardParamList[index];
		}

		int itemCount = 0;
		if ( 0 <= index && index < mEventSetData.RewardItemCount.Count ) {
			itemCount = mEventSetData.RewardItemCount[index];
		}

		slot.UpdateSlot( eventResetRewardParam, itemCount, false );
	}

	private int OnEventRewardListGetItemCount() {
		return mEventResetRewardParamList.Count;
	}

	private void OnNetEventRewardReset( int result, PktMsgType pktmsg ) {
		if ( result != 0 ) {
			return;
		}

		SetEventRewardBase();
		Renewal();
	}

	private void OnNetEventRewardTake( int result, PktMsgType pktmsg ) {
		if ( result != 0 ) {
			return;
		}

		DirectorUIManager.Instance.PlayRewardOpen( FLocalizeString.Instance.GetText( (int)eTEXTID.REWARD_POPUP_TITLE ), FLocalizeString.Instance.GetText( (int)eTEXTID.REWARD_POPUP_TEXT ), GameInfo.Instance.RewardList, OnCallbackNewCard );

		LobbyUIManager.Instance.Renewal( "TopPanel" );
		LobbyUIManager.Instance.Renewal( "GoodsPopup" );

		SetEventRewardBase();
		Renewal();
	}

	public void OnCallbackNewCard() {
		DirectorUIManager.Instance.PlayNewCardGreeings( mPreCardBookList );
	}

	private bool IsTenOverGacha() {
		return GameInfo.Instance.GameConfig.EvtResetGachaMaxNum < GetMaxGachaCount();
	}

	private int GetMaxGachaCount() {
		return mEventItemCount / GameInfo.Instance.GameConfig.EvtResetGachaReqCnt;
	}

	private int GetTotalRewardItemCount() {
		if ( mEventResetRewardParamList == null ) {
			return 0;
		}

		int result = 0;
		for ( int i = 0; i < mEventResetRewardParamList.Count; i++ ) {
			result += mEventResetRewardParamList[i].RewardCnt;
		}

		return result;
	}

	private void SetMultiGachaBtn() {
		if ( mMaxRewardItemCount <= 2 || mEventItemCount < GameInfo.Instance.GameConfig.EvtResetGachaReqCnt * 2 ) {
			mMultiCount = 2;
		}
		else {
			mMultiCount = mEventItemCount / GameInfo.Instance.GameConfig.EvtResetGachaReqCnt;

			if ( mMaxRewardItemCount < mMultiCount ) {
				mMultiCount = mMaxRewardItemCount;
			}
		}

		SetMultiGachaLabel();
	}

	private void SetMultiGachaLabel() {
		_MultiGachaCount.textlocalize = mMultiCount.ToString();
		_MultiGachaLabel.textlocalize = ( mMultiCount * GameInfo.Instance.GameConfig.EvtResetGachaReqCnt ).ToString();
	}

	private void SetResetBtn() {
		bool isActive = 0 < mMaxRewardItemCount;

		for ( int i = 0; i < mEventResetRewardParamList.Count; i++ ) {
			if ( mEventResetRewardParamList[i].ResetFlag == 1 ) {
				isActive = 0 < mEventSetData.RewardItemCount[i];
				if ( isActive == true ) {
					break;
				}
			}
		}

		_DisResetSpr.SetActive( isActive );
	}

	private void SetRewardMaxCount() {
		mMaxRewardItemCount = 0;

		if ( mEventSetData == null ) {
			return;
		}

		for ( int i = 0; i < mEventSetData.RewardItemCount.Count; i++ ) {
			mMaxRewardItemCount += mEventSetData.RewardItemCount[i];
		}
	}

	private void SetEventRewardReset() {
		int state = GameSupport.GetJoinEventState( mEventID );
		if ( state < (int)eEventState.EventNone ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( state == (int)eEventState.EventNotStart ? 3052 : 3053 ) );
		}
		else {
			GameInfo.Instance.Send_ReqEventRewardReset( mEventSetData.TableID, OnNetEventRewardReset );
		}
	}

	private void SetEventRewardBase() {
		mEventSetData = GameInfo.Instance.GetEventSetData( mEventID );
		mEventResetRewardParamList = GameInfo.Instance.GameTable.FindAllEventResetReward( x => x.EventID == mEventID && x.RewardStep == mEventSetData.RewardStep );

		LobbyUIManager.Instance.BG_Event( kBGIndex, mEventSetData.TableID, 0 );

		BannerData bannerdataBG = GameInfo.Instance.ServerData.BannerList.Find( x => x.BannerType == (int)eBannerType.EVENT_MAINBG && x.BannerTypeValue == mEventSetData.TableID );
		if ( bannerdataBG != null ) {
			if ( _BgTex.mainTexture != null ) {
				DestroyImmediate( _BgTex.mainTexture, false );
				_BgTex.mainTexture = null;
			}

			_BgTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL( bannerdataBG.UrlImage, true, bannerdataBG.Localizes[(int)eBannerLocalizeType.Url] );
		}

		Texture iconTex = null;
		GameTable.Item.Param itemPram = GameInfo.Instance.GameTable.FindItem( x => x.ID == mEventSetData.TableData.EventItemID1 );
		if ( itemPram != null ) {
			iconTex = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", Utility.AppendString( "Icon/Item/", itemPram.Icon ) );
		}

		_IconTex.mainTexture = _OnceGachaTex.mainTexture = _MultiGachaTex.mainTexture = iconTex;
	}
}
