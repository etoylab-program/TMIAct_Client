
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIRaidMainPanel : FComponent {
	[Header( "[Raid Main Panel]" )]
	[SerializeField] private UILabel		_TitleLabel;
	[SerializeField] private UILabel		_RemainTimeLabel;
	[SerializeField] private UIGoodsUnit	_GoodsUnit;
	[SerializeField] private UILabel		_AmountPerDayLabel;
	[SerializeField] private UITexture		_BgTex;
	[SerializeField] private UILabel		_DailyAcquisitionLabel;


	public override void OnEnable() {
		InitComponent();
		base.OnEnable();
	}

	public override void InitComponent() {
		string platform = "";
		if( AppMgr.Instance.ResPlatform == AppMgr.eResPlatform.aos ) {
			platform = "_aos";
		}
			
		string texPath = string.Format( "UI/UITexture/Raid/Raid_FallBG_{0}-KOR{1}.png", GameInfo.Instance.ServerData.RaidCurrentSeason.ToString( "D2" ), platform );
		_BgTex.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle( "ui", texPath ) as Texture2D;

		GameTable.Stage.Param param = GameInfo.Instance.GameTable.FindStage( x => x.StageType == (int)eSTAGETYPE.STAGE_RAID && x.TypeValue == GameInfo.Instance.ServerData.RaidCurrentSeason );
		_TitleLabel.textlocalize = FLocalizeString.Instance.GetText( param.Name );

		string str = string.Format( FLocalizeString.Instance.GetText(1452), GameInfo.Instance.ServerData.RaidSeasonEndTime.Year,
																			GameInfo.Instance.ServerData.RaidSeasonEndTime.Month,
																			GameInfo.Instance.ServerData.RaidSeasonEndTime.Day );

		str = string.Format( "{0} ({1}) {2}:{3}", str, 
												  FLocalizeString.Instance.GetText( 190 + (int)GameInfo.Instance.ServerData.RaidSeasonEndTime.DayOfWeek ), 
												  GameInfo.Instance.ServerData.RaidSeasonEndTime.Hour.ToString( "D2" ), 
												  GameInfo.Instance.ServerData.RaidSeasonEndTime.Minute.ToString( "D2" ) );

		_RemainTimeLabel.textlocalize = string.Format( FLocalizeString.Instance.GetText( 1399 ), str );
	}

	public override void Renewal( bool bChildren ) {
		base.Renewal( bChildren );

		_GoodsUnit.InitGoodsUnit( eGOODSTYPE.RAIDPOINT, GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.RAIDPOINT] );

		_AmountPerDayLabel.textlocalize = FLocalizeString.Instance.GetText(3335, GameInfo.Instance.RaidUserData.DailyRaidPoint,
																				 GameInfo.Instance.GameConfig.RaidPointDailyLimit );
	}

	public void OnBtnHelp() {
		UIValue.Instance.SetValue( UIValue.EParamType.RulePopupType, (int)UIEventRulePopup.eRulePopupType.RAID_RULE );
		LobbyUIManager.Instance.ShowUI( "EventRulePopup", true );
	}

	public void OnBtnShop() {
		UIRaidStorePopup popup = LobbyUIManager.Instance.GetUI<UIRaidStorePopup>( "RaidStorePopup" );
		if( popup ) {
			popup.SetUIActive( true );
		}
	}

	public void OnBtnEnter() {
		GameInfo.Instance.Send_ReqRaidRankingList( OnNetRaidRankingList );
	}

	private void OnNetRaidRankingList( int result, PktMsgType pkt ) {
		if( result != 0 ) {
			return;
		}

		GameInfo.Instance.Send_ReqRaidFirstRankingList( OnNetRaidFirstRankingList );
	}

	private void OnNetRaidFirstRankingList( int result, PktMsgType pkt ) {
		if( result != 0 ) {
			return;
		}

		GameInfo.Instance.SelectedRaidLevel = GameInfo.Instance.RaidUserData.CurStep;
		LobbyUIManager.Instance.SetPanelType( ePANELTYPE.RAID );
	}
}
