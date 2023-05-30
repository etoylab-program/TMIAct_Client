
using System.Collections.Generic;
using UnityEngine;


public class ActionShiranuiUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50013;
        mGroundEffId = 50014;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 7301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
