
using System.Collections.Generic;
using UnityEngine;


public class ActionKiraraUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50027;
        mGroundEffId = 50028;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 12301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
