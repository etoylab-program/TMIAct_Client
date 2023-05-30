
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIArenaVsPanel : FComponent
{
    [Serializable]
    public class sUICharInfo
    {
        public UILabel      LbName;
        public UILabel      LbLevel;
        public UISprite     SprGrade;
        public UITexture    TexChar;
        public UILabel      LbOrder;
    }


    [Header("[Player]")]
    public sUICharInfo UIPlayerInfo;

    [Header("[Opponent]")]
    public sUICharInfo UIOpponentInfo;
    

    private WorldPVP    mWorldPVP   = null;
    private Player      mPlayer     = null;
    private Player      mOpponent   = null;


    public override void OnEnable()
    {
        base.OnEnable();
        Renewal(true);
    }

	public override void Renewal( bool bChildren = false ) {
		base.Renewal( bChildren );

		if ( mWorldPVP == null ) {
			mWorldPVP = World.Instance as WorldPVP;
		}

		mPlayer = mWorldPVP.GetCurrentPlayerTeamCharOrNull();
		SetUICharInfo( mPlayer, mWorldPVP.CurPlayerCharIndex, UIPlayerInfo );

		mOpponent = mWorldPVP.GetCurrentOpponentTeamCharOrNull();
		SetUICharInfo( mOpponent, mWorldPVP.CurOpponentCharIndex, UIOpponentInfo );
	}

	private void SetUICharInfo(Player player, int order, sUICharInfo uiCharInfo)
    {
        uiCharInfo.LbName.textlocalize = player.tableName;
        uiCharInfo.LbLevel.textlocalize = string.Format("Lv.{0}/{1}", player.charData.Level, GameInfo.Instance.GameConfig.CharMaxLevel[player.charData.Grade]);
        uiCharInfo.SprGrade.spriteName = string.Format("grade_{0}", player.charData.Grade.ToString("D2"));
        uiCharInfo.LbOrder.textlocalize = FLocalizeString.Instance.GetText(1482 + order);
    }
}
