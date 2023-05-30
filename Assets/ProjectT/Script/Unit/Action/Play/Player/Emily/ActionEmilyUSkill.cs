
using System.Collections.Generic;
using UnityEngine;


public class ActionEmilyUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50017;
        mGroundEffId = 50018;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 8301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
