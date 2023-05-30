
using System.Collections.Generic;
using UnityEngine;


public class ActionMaikaUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50039;
        mGroundEffId = 50040;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 19301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
