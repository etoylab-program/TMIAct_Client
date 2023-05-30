using System.Collections.Generic;
using UnityEngine;


public class ActionSoraUSkill : ActionUSkillBase {
	protected override void LoadEffect() {
		mCameraEffId = 50043;
		mGroundEffId = 50044;
	}

	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		mTableId = 21301;
		base.Init( tableId, listAddCharSkillParam );
	}
}
