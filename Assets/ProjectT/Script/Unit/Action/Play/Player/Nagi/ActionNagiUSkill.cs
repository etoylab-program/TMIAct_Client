
using System.Collections.Generic;
using UnityEngine;


public class ActionNagiUSkill : ActionUSkillBase {
	protected override void LoadEffect() {
		mCameraEffId = 50047;
		mGroundEffId = 50048;
	}

	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		mTableId = 23301;
		base.Init( tableId, listAddCharSkillParam );
	}
}
