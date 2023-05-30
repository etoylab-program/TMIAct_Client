
using System;
using UnityEngine;


public class UIRaidPanel : FComponent {
	[Header("[Raid Panel]")]
	[SerializeField] private UILabel			_StepNumberLabel;
	[SerializeField] private UILabel			_RaidNameLabel;
	[SerializeField] private GameObject			_BestRecordOff;
	[SerializeField] private GameObject			_BestRecordOn;
	[SerializeField] private UIUserCharListSlot _UserCharListSlot;
	[SerializeField] private UILabel			_HighestScoreLabel;
	[SerializeField] private UILabel			_DateRemainLabel;
	[SerializeField] private UIButton			_EnterBtn;
	[SerializeField] private UILabel			_RemainTimeDescLb;
	[SerializeField] private FList				_StepListInstance;
	[SerializeField] private FList				_RankingListInstance;

	private RaidClearData			mRaidClearData		= null;
	private RaidRankData			mRaidRankData		= null;
	private int						mCurStep			= 1;
	private TimeAttackRankUserData	mSelectRankUserData = null;


	public override void Awake() {
		base.Awake();

		if( _StepListInstance != null ) {
			_StepListInstance.EventUpdate = UpdateStepListSlot;
			_StepListInstance.EventGetItemCount = GetStepElementCount;

			_StepListInstance.InitBottomFixing();
		}

		if( _RankingListInstance != null ) {
			_RankingListInstance.EventUpdate = UpdateRankingListSlot;
			_RankingListInstance.EventGetItemCount = GetRankingElementCount;

			_RankingListInstance.InitBottomFixing();
		}
	}

	public override void OnEnable() {
		InitComponent();
		base.OnEnable();
	}

	public override void InitComponent() {
		mCurStep = GameInfo.Instance.SelectedRaidLevel;

		SelectStep( mCurStep );

		_StepListInstance.UpdateList();
		_RankingListInstance.UpdateList();

		if( _StepListInstance.IsScroll && mCurStep > 1 ) {
			_StepListInstance.SpringSetFocus( mCurStep - 1, 1 );
		}
	}

	public override void Renewal( bool bChildren ) {
		base.Renewal( bChildren );

		_StepNumberLabel.textlocalize = string.Format( "{0} {1}", FLocalizeString.Instance.GetText( 3318 ), mCurStep.ToString( "D2" ) );
		_RaidNameLabel.textlocalize = FLocalizeString.Instance.GetText( GameInfo.Instance.RaidUserData.CurStageParam.Name );

		if( mRaidClearData != null ) {

			_BestRecordOn.SetActive( true );
			_BestRecordOff.SetActive( false );

			FLocalizeString.SetLabel( _RemainTimeDescLb, string.Format( FLocalizeString.Instance.GetText( 1044 ),
																		GameInfo.Instance.GameConfig.TimeAttackModeRecordDay ) );

			_UserCharListSlot.UpdateSlot( mRaidClearData );
			_HighestScoreLabel.textlocalize = GameSupport.GetTimeHighestScore( (int)mRaidClearData.HighestScore );
		}
		else {
			_BestRecordOn.SetActive( false );
			_BestRecordOff.SetActive( true );
		}

		_StepListInstance.RefreshNotMove();
		_RankingListInstance.UpdateList();
	}

	public override void OnUIOpen() {
		LobbyUIManager.Instance.kBlackScene.SetActive( false );
	}

	public void SelectStep( int step ) {
		mCurStep = step;
		
		mRaidClearData = GameInfo.Instance.GetRaidClearDataOrNull( mCurStep );
		mRaidRankData = GameInfo.Instance.GetRaidRankDataOrNull( mCurStep );

		GameInfo.Instance.SelectedRaidLevel = mCurStep;
		Renewal( true );
	}

	public void SelectRankUser( int index ) {
		if( mRaidRankData == null || mRaidRankData.RankUserList.Count <= 0 ) {
			return;
		}

		UIValue.Instance.SetValue( UIValue.EParamType.RankUserType, (int)eRankUserType.RAID );
		UIValue.Instance.SetValue( UIValue.EParamType.TimeAttackRankStageID, mCurStep );
		UIValue.Instance.SetValue( UIValue.EParamType.TimeAttackRankUUID, mRaidRankData.RankUserList[index].UUID );

		mSelectRankUserData = mRaidRankData.RankUserList[index];

		if( mSelectRankUserData.IsRaidFirstRanker ) {
			GameInfo.Instance.Send_ReqFirstRaidRankerDetail( mRaidRankData.StageTableID, mSelectRankUserData.UUID, mCurStep, OnNetRaidRankerDetail );
		}
		else {
			GameInfo.Instance.Send_ReqRaidRankerDetail( mRaidRankData.StageTableID, mSelectRankUserData.UUID, mCurStep, OnNetRaidRankerDetail );
		}
	}

	public void OnBtnEnter() {
		UIRaidDetailPopup raidDetailPopup = LobbyUIManager.Instance.GetUI<UIRaidDetailPopup>( "RaidDetailPopup" );

		if( raidDetailPopup ) {
			LobbyUIManager.Instance.ShowUI( "RaidDetailPopup", true );
		}
	}

	private void OnNetRaidRankerDetail( int result, PktMsgType pktmsg ) {
		if( result != 0 ) {
			return;
		}

		UIUserCharDetailPopup popup = LobbyUIManager.Instance.GetUI<UIUserCharDetailPopup>( "UserCharDetailPopup" );
		popup.SetFirstRankClearUser( mSelectRankUserData.IsRaidFirstRanker );
		popup.SetUIActive( true );
	}

	private int GetStepElementCount() {
		return GameInfo.Instance.RaidUserData.CurStep;
	}

	private void UpdateStepListSlot( int index, GameObject slotObject ) {
		UIRaidStepToggleBtnSlot slot = slotObject.GetComponent<UIRaidStepToggleBtnSlot>();
		if( slot == null ) {
			return;
		}

		slot.ParentGO = gameObject;
		slot.UpdateSlot( index );
	}

	private int GetRankingElementCount() {
		return mRaidRankData != null ? mRaidRankData.RankUserList.Count : 0;
	}

	private void UpdateRankingListSlot( int index, GameObject slotObject ) {
		if( mRaidRankData == null || mRaidRankData.RankUserList.Count <= 0 ) {
			return;
		}

		UIRankingListSlot slot = slotObject.GetComponent<UIRankingListSlot>();
		if( slot == null ) {
			return;
		}

		slot.ParentGO = gameObject;
		slot.UpdateSlot( index, mRaidRankData.RankUserList[index] );
	}
}
