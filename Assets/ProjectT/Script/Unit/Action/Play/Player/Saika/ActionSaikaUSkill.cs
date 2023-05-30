
using System.Collections.Generic;
using UnityEngine;


public class ActionSaikaUSkill : ActionUSkillBase {
	protected override void LoadEffect() {
		mCameraEffId = 50051;
		mGroundEffId = 50052;
	}

	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		mTableId = 25301;
		base.Init( tableId, listAddCharSkillParam );
	}
}
