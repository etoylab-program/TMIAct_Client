
using System.Collections.Generic;
using UnityEngine;


public class ActionFeliciaUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50037;
        mGroundEffId = 50038;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 18301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
