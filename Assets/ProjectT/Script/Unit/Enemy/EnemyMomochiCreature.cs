
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyMomochiCreature : Enemy
{
    [Header("[Momochi Creature Property]")]
    public Transform HeadPos;


    public override Vector3 GetHeadPos(float heightRatio = 0.9f)
    {
        return HeadPos.position;
    }
}
