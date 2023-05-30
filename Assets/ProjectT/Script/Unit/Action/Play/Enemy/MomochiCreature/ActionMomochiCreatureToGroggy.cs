
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMomochiCreatureToGroggy : ActionEnemyBase
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.ToGroggy;

        mOwnerEnemy = m_owner as Enemy;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mOwnerEnemy.SummoningMinion = false;
        //World.Instance.InGameCamera.SetDefaultModeTarget(mOwnerEnemy, true);
    }

    public override IEnumerator UpdateAction()
    {
        yield return null;
    }

    public override void OnEnd()
    {
        mOwnerEnemy.BreakShield();
        World.Instance.Player.UpdateTargetHp(mOwnerEnemy);

        ActionParamHit param = new ActionParamHit(null, eBehaviour.GroggyAttack, 0.0f, eHitDirection.None, -1, false, 0.0f, eHitState.OnlyEffect, EAttackAttr.NORMAL);
        SetNextAction(eActionCommand.Hit, param);
    }
}
