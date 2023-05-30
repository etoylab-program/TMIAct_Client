
using System.Collections.Generic;
using UnityEngine;


public class ActionAnneroseUSkill : ActionUSkillBase {
	protected override void LoadEffect() {
		mCameraEffId = 50045;
		mGroundEffId = 50046;
	}

	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		mTableId = 22301;
		base.Init( tableId, listAddCharSkillParam );
	}
}
