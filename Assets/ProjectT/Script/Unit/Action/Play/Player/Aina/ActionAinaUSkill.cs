
using System.Collections.Generic;
using UnityEngine;


public class ActionAinaUSkill : ActionUSkillBase {
	protected override void LoadEffect() {
		mCameraEffId = 50049;
		mGroundEffId = 50050;
	}

	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		mTableId = 24301;
		base.Init( tableId, listAddCharSkillParam );
	}
}
