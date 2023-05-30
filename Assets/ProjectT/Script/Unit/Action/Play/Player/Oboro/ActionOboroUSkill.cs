
using System.Collections.Generic;
using UnityEngine;


public class ActionOboroUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50023;
        mGroundEffId = 50024;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 10301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
