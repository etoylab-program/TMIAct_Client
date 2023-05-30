
using System.Collections.Generic;
using UnityEngine;


public class ActionRinUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50041;
        mGroundEffId = 50042;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 20301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
