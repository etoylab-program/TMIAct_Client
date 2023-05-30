
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shiranui : Player
{
    public Shader CloneShader { get; protected set; }


    public override void Init(int tableId, eCharacterType type, string faceAniControllerPath)
    {
        base.Init(tableId, type, faceAniControllerPath);
        CloneShader = ResourceMgr.Instance.LoadFromAssetBundle("shader", "Shader/effect/fx_shiranui_evasion_skill.shader") as Shader; //Shader.Find("etoylab_effect/fx_shiranui_evasion_skill");
        if(CloneShader == null)
        {
            Debug.LogError("시라누이 클론 셰이더를 못찾는다!!");
        }
    }

    public override void HideAllClone()
    {
        base.HideAllClone();

        if (m_actionSystem2)
        {
            ActionSupporterShiranuiSummon action = m_actionSystem2.GetAction<ActionSupporterShiranuiSummon>(eActionCommand.SupporterShiranuiSummon);
            if(action)
            {
                action.ForceEnd();
            }
        }
    }
}
