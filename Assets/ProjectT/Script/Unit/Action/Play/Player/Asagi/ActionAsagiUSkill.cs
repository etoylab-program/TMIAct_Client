
using System.Collections.Generic;
using UnityEngine;


public class ActionAsagiUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50001;
        mGroundEffId = 50002;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 1301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
