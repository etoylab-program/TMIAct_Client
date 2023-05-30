using System.Collections.Generic;
using UnityEngine;


public class ActionShisuiUSkill : ActionUSkillBase {
	protected override void LoadEffect() {
		mCameraEffId = 50053;
		mGroundEffId = 50054;
	}

	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		mTableId = 26301;
		base.Init( tableId, listAddCharSkillParam );
	}
}
