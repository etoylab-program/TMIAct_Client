
using System.Collections.Generic;
using UnityEngine;


public class ActionIngridUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50029;
        mGroundEffId = 50030;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 13301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
