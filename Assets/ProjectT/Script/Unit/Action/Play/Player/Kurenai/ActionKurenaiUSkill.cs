
using System.Collections.Generic;
using UnityEngine;


public class ActionKurenaiUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50019;
        mGroundEffId = 50020;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 9301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
