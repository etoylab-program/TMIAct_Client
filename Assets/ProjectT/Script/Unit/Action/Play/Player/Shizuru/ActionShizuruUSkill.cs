
using System.Collections.Generic;
using UnityEngine;


public class ActionShizuruUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50035;
        mGroundEffId = 50036;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 17301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
