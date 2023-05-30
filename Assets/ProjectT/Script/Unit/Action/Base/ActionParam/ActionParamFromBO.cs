
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionParamFromBO : IActionBaseParam
{
    public BattleOption.sBattleOptionData battleOptionData { get; private set; }


    public ActionParamFromBO(BattleOption.sBattleOptionData battleOptionData)
    {
        this.battleOptionData = battleOptionData;
    }
}
