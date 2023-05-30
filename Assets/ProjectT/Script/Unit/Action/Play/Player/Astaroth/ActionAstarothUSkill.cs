
using System.Collections.Generic;
using UnityEngine;


public class ActionAstarothUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50031;
        mGroundEffId = 50032;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 15301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
