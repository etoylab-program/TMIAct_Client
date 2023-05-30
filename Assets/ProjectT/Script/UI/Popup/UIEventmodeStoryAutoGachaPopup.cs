using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventmodeStoryAutoGachaPopup : FComponent {
	[Header( "UIEventmodeStoryAutoGachaPopup" )]
	[SerializeField] private FList _RewardFList;

	[SerializeField] private UITexture _BgTex;

	[SerializeField] private UIButton _NextBtn;
	[SerializeField] private UIButton _CloseBtn;
	[SerializeField] private UIButton _StopBtn;

	[SerializeField] private UILabel _InfoLabel;
	[SerializeField] private UILabel _NewCardLabel;
	[SerializeField] private UILabel _GachaCountLabel;
	[SerializeField] private UILabel _TicketCountLabel;

	[SerializeField] private UIGaugeUnit _GaugeUnit;

	[SerializeField] private Animation _BoxDirectorAnim;

	[SerializeField] private ParticleSystem _BoxOpenParticle;

	private enum eGachaType {
		ING,
		SUPPORTER,
		END,
	}

	private EventSetData mEventSetData;

	private List<GameTable.EventResetReward.Param> mEventResetRewardParamList;
	private List<CardBookData> mPreCardBookList;
	private List<RewardData> mRewardList = new List<RewardData>();
	private AnimationClip[] mBoxDirectorAnimClips;

	private int mMultiCount;

	private bool mIsRareStop;
	private bool mIsFirst;

	private eGachaType mGachaType;

	private WaitForFixedUpdate mWaitForFixedUpdate = new WaitForFixedUpdate();

	public override void Awake() {
		base.Awake();

		mBoxDirectorAnimClips = new AnimationClip[_BoxDirectorAnim.GetClipCount()];
		int index = 0;
		foreach ( AnimationState aniState in _BoxDirectorAnim ) {
			mBoxDirectorAnimClips[index] = aniState.clip;
			++index;
		}

		_GaugeUnit.InitGaugeUnit( 0.0f );

		_RewardFList.EventUpdate = OnEventRewardFListUpdate;
		_RewardFList.EventGetItemCount = OnEventRewardFListGetItemCount;
		_RewardFList.UpdateList();
	}

	public override void InitComponent() {
		base.InitComponent();

		_StopBtn.isEnabled = true;
		_BoxOpenParticle.gameObject.SetActive( false );

		mIsFirst = true;
		mGachaType = eGachaType.ING;

		mRewardList.Clear();
	}

	public override void OnEnable() {
		InitComponent();
		base.OnEnable();
	}

	public override void Renewal( bool bChildren = false ) {
		base.Renewal( bChildren );

		_NewCardLabel.SetActive( mGachaType == eGachaType.SUPPORTER );
		_NextBtn.SetActive( mGachaType == eGachaType.SUPPORTER );
		_StopBtn.SetActive( mGachaType == eGachaType.ING );
		_CloseBtn.SetActive( mGachaType == eGachaType.END );

		int maxCount = GameInfo.Instance.GameConfig.EvtResetGachaMaxNum;
		if ( mMultiCount < maxCount ) {
			maxCount = mMultiCount;
		}
		_InfoLabel.textlocalize = FLocalizeString.Instance.GetText( 3359, maxCount );
		_GachaCountLabel.textlocalize = FLocalizeString.Instance.GetText( 1443, mMultiCount );
		_TicketCountLabel.textlocalize = FLocalizeString.Instance.GetText( 3354, GameInfo.Instance.GetItemIDCount( mEventSetData.TableData.EventItemID1 ) );

		_RewardFList.SpringSetFocus( 0, isImmediate: true );
		_RewardFList.RefreshNotMoveAllItem();

		if ( mIsFirst ) {
			mIsFirst = false;
			StartCoroutine( nameof( AutoGachaAnimation ) );
		}
	}

	public override void OnClickClose() {
		if ( mGachaType == eGachaType.ING ) {
			return;
		}

		base.OnClickClose();

		LobbyUIManager.Instance.Renewal( "EventmodeStoryResetGachaPanel" );
	}

	public bool IsAutoIng() {
		return mGachaType == eGachaType.ING;
	}

	public void SetData( bool isRareStop, int multiCount, ref EventSetData eventSetData, ref List<GameTable.EventResetReward.Param> eventResetRewardParamList, ref List<CardBookData> preCardBookList, ref UITexture bgTex ) {
		mIsRareStop = isRareStop;
		mMultiCount = multiCount;

		mEventSetData = eventSetData;
		mEventResetRewardParamList = eventResetRewardParamList;
		mPreCardBookList = preCardBookList;

		_BgTex.mainTexture = bgTex.mainTexture;
	}

	public void OnClick_AutoNextBtn() {
		OnCallbackNewCard();
		mGachaType = eGachaType.END;

		Renewal();
	}

	public void OnClick_AutoStopBtn() {
		_StopBtn.isEnabled = false;

		mGachaType = eGachaType.END;
	}

	private void OnCallbackNewCard() {
		DirectorUIManager.Instance.PlayNewCardGreeings( mPreCardBookList );
	}

	private void OnEventRewardFListUpdate( int index, GameObject obj ) {
		UIItemListSlot slot = obj.GetComponent<UIItemListSlot>();
		if ( slot == null ) {
			return;
		}

		if ( slot.ParentGO == null ) {
			slot.ParentGO = this.gameObject;
		}

		RewardData rewardData = null;
		if ( 0 <= index && index < mRewardList.Count ) {
			rewardData = mRewardList[index];
		}

		slot.UpdateSlotRewardDataByCount( UIItemListSlot.ePosType.REWARD_DATA_INFO_NOT_SELL, index, rewardData );
	}

	private int OnEventRewardFListGetItemCount() {
		return mRewardList.Count;
	}

	private IEnumerator AutoGachaAnimation() {
		_BoxDirectorAnim.Play( mBoxDirectorAnimClips[0].name );

		_StopBtn.isEnabled = true;

		float waitTimeSec = 0.0f;
		float totalTimeSec = GameInfo.Instance.GameConfig.EvtResetGachaAutoTimeSec;
		while ( waitTimeSec < totalTimeSec ) {
			waitTimeSec += Time.fixedDeltaTime;
			_GaugeUnit.InitGaugeUnit( waitTimeSec / totalTimeSec );
			yield return mWaitForFixedUpdate;
		}

		_StopBtn.isEnabled = false;

		int gachaCount = mMultiCount < GameInfo.Instance.GameConfig.EvtResetGachaMaxNum ? mMultiCount : GameInfo.Instance.GameConfig.EvtResetGachaMaxNum;
		GameInfo.Instance.Send_ReqEventRewardTake( mEventSetData.TableID, gachaCount, mEventSetData.RewardStep, 0, OnNetEventRewardAutoTake );
	}

	private IEnumerator AutoGachaAnimationResult() {

		_BoxDirectorAnim.Play( mBoxDirectorAnimClips[1].name );
		_BoxOpenParticle.gameObject.SetActive( true );

		SoundManager.Instance.PlayUISnd( 43 );

		Renewal();

		float waitTimeSec = 0.0f;
		while ( waitTimeSec < mBoxDirectorAnimClips[1].length ) {
			waitTimeSec += Time.fixedDeltaTime;
			yield return mWaitForFixedUpdate;
		}

		_BoxOpenParticle.gameObject.SetActive( false );

		if ( mGachaType == eGachaType.ING ) {
			StartCoroutine( nameof( AutoGachaAnimation ) );
		}
	}

	private void OnNetEventRewardAutoTake( int result, PktMsgType pktmsg ) {
		if ( result != 0 ) {
			return;
		}

		GameTable.EventResetReward.Param eventResetRewardParam = mEventResetRewardParamList.Find( x => x.ResetFlag == 1 );
		RewardData rareRewardData = null;
		if ( eventResetRewardParam != null ) {
			rareRewardData = new RewardData( eventResetRewardParam.ProductType, eventResetRewardParam.ProductIndex, eventResetRewardParam.ProductValue );
		}

		bool haveNewCard = false;
		bool haveRareItem = false;
		for ( int i = 0; i < GameInfo.Instance.RewardList.Count; i++ ) {
			// New Support Check
			if ( !haveNewCard && DirectorUIManager.Instance.IsNewCard( GameInfo.Instance.RewardList[i], ref mPreCardBookList ) ) {
				haveNewCard = true;
			}

			// Rare Check
			if ( !haveRareItem && mIsRareStop ) {
				if ( rareRewardData != null ) {
					if ( rareRewardData.Type == GameInfo.Instance.RewardList[i].Type && rareRewardData.Index == GameInfo.Instance.RewardList[i].Index ) {
						haveRareItem = true;
					}
				}
			}
		}

		if ( haveNewCard || haveRareItem ) {
			mGachaType = eGachaType.END;
		}

		// Decrease Count
		mMultiCount -= GameInfo.Instance.GameConfig.EvtResetGachaMaxNum;
		if ( mMultiCount <= 0 ) {
			mMultiCount = 0;
			mGachaType = eGachaType.END;
		}

		if ( haveNewCard ) {
			mGachaType = eGachaType.SUPPORTER;
		}

		for ( int i = 0; i < mRewardList.Count; i++ ) {
			mRewardList[i].NewCount = 0;
		}

		for ( int i = 0; i < GameInfo.Instance.RewardList.Count; i++ ) {
			RewardData originData = GameInfo.Instance.RewardList[i];
			RewardData rewardData = mRewardList.Find( x => x.Index == originData.Index );
			if ( rewardData != null ) {
				++rewardData.Count;
			}
			else {
				rewardData = new RewardData( originData.Type, originData.Index, originData.Value, 1, 0 );
				mRewardList.Add( rewardData );
			}

			++rewardData.NewCount;
		}

		StartCoroutine( nameof( AutoGachaAnimationResult ) );
	}
}
