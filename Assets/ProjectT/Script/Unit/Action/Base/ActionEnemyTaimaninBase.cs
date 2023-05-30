
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEnemyTaimaninBase : ActionEnemyBase
{
    [Header("[Property]")]
    public float AttackRange = 2.0f;

    protected UnitCollider mTargetCollider = null;


    public override float GetAtkRange()
    {
        return AttackRange;
    }
}
