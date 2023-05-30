
using System.Collections.Generic;
using UnityEngine;


public class ActionRinkoUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50008;
        mGroundEffId = 50009;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 4301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
