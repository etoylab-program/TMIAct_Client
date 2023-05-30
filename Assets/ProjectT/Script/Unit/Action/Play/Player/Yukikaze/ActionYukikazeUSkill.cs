
using System.Collections.Generic;
using UnityEngine;


public class ActionYukikazeUSkill : ActionUSkillBase
{
    protected override void LoadEffect()
    {
        mCameraEffId = 50005;
        mGroundEffId = 50006;

        //mAniCamera = ResourceMgr.Instance.LoadFromAssetBundle("effect", "Effect/Character/Animation/ani_ultimate_skill_finish_yukikaze.anim") as AnimationClip;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 3301;
        base.Init(tableId, listAddCharSkillParam);
    }
}
