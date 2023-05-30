using UnityEngine;
using System.Collections;

public class UIRankingListSlot : FSlot {
 

	public UISprite kIcon_1Spr;
	public UISprite kIcon_2Spr;
	public UILabel kRankLabel;
	public UILabel kNameLabel;
	public UILabel kLevelLabel;
	public UILabel kTimeCountLabel;
	public UITexture kUserIconTex;
    public UIUserCharListSlot kUserCharListSlot;

    private int _index;
    private TimeAttackRankUserData _rankuserdata;

    public void UpdateSlot( int index, TimeAttackRankUserData rankuserdata) 	//Fill parameter if you need
	{
        _index = index;
        _rankuserdata = rankuserdata;

        if( _rankuserdata.IsRaidFirstRanker ) {
            kIcon_1Spr.gameObject.SetActive( true );
            kIcon_2Spr.gameObject.SetActive( true );
        }
        else {
            if( _rankuserdata.Rank == 1 ) {
                kIcon_1Spr.gameObject.SetActive( true );
                kIcon_2Spr.gameObject.SetActive( false );
            }
            else {
                kIcon_1Spr.gameObject.SetActive( false );
                kIcon_2Spr.gameObject.SetActive( true );
            }
        }

        if( _rankuserdata.IsRaidFirstRanker ) {
            kRankLabel.textlocalize = FLocalizeString.Instance.GetText( 3331 );
        }
        else {
            kRankLabel.textlocalize = string.Format( FLocalizeString.Instance.GetText( 1448 ), _rankuserdata.Rank.ToString( "#,##0" ) );
        }

        kNameLabel.textlocalize = _rankuserdata.GetUserNickName();
        kLevelLabel.textlocalize = _rankuserdata.UserRank.ToString();
        
        var data = GameInfo.Instance.GameTable.FindUserMark(_rankuserdata.UserMark);
        if (data != null)
            LobbyUIManager.Instance.GetUserMarkIcon(ParentGO, this.gameObject, data.ID, ref kUserIconTex);
        else
            kUserIconTex.mainTexture = null;


        kUserCharListSlot.UpdateSlot(_rankuserdata.CharData, false);
        kTimeCountLabel.textlocalize = GameSupport.GetTimeHighestScore(_rankuserdata.HighestScore);
    }

	public void OnClick_Slot() {
        if( ParentGO == null ) {
            return;
        }

		UITimeAttackPanel storeltimeattackpanel = ParentGO.GetComponent<UITimeAttackPanel>();
        if( storeltimeattackpanel ) {
            storeltimeattackpanel.ClickRankUser( _index );
        }
        else {
            UIRaidPanel raidPanel = ParentGO.GetComponent<UIRaidPanel>();
            if( raidPanel ) {
                raidPanel.SelectRankUser( _index );
            }
		}
	}
}
