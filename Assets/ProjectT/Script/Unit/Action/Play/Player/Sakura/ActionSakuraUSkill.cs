
using System.Collections.Generic;
using UnityEngine;


public class ActionSakuraUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50003;
        mGroundEffId = 50004;

        //mAniCamera = ResourceMgr.Instance.LoadFromAssetBundle("effect", "Effect/Character/Animation/ani_ultimate_skill_finish_sakura.anim") as AnimationClip;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 2301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
