
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldTestScene : World
{
    public override void Init(int tableId, eSTAGETYPE stageType = eSTAGETYPE.STAGE_NONE)
    {
        base.Init(tableId, stageType);
        PostInit();
    }
}
