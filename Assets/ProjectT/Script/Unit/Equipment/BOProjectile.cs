using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOProjectile : BattleOption {
	public BOProjectile( int battleOptionSetId, Unit owner ) : base( owner ) {
		if ( GameInfo.Instance.GameClientTable == null ) {
			return;
		}

		mToExecuteType = eToExecuteType.Unit;
		mListBattleOptionSet.Clear();

		GameClientTable.BattleOptionSet.Param param = GameInfo.Instance.GameClientTable.FindBattleOptionSet( battleOptionSetId );
		if ( param == null ) {
			return;
		}

		mListBattleOptionSet.Add( param );
		Parse( 1 );
	}
}
