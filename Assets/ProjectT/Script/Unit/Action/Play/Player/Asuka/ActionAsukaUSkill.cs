
using System.Collections.Generic;
using UnityEngine;


public class ActionAsukaUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50025;
        mGroundEffId = 50026;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 11301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
