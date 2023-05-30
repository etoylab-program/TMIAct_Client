
using System.Collections.Generic;
using UnityEngine;


public class ActionMurasakiUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50010;
        mGroundEffId = 50011;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 5301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
