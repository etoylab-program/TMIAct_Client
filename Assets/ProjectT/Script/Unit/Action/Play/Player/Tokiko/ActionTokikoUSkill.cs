
using System.Collections.Generic;
using UnityEngine;


public class ActionTokikoUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50033;
        mGroundEffId = 50034;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 16301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
