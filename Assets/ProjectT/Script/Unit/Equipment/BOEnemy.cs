
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BOEnemy : BattleOption
{
    public BOEnemy(int battleOptionSetId, Unit owner) : base(owner)
    {
        mToExecuteType = eToExecuteType.Unit;
        mListBattleOptionSet.Clear();

        GameClientTable.BattleOptionSet.Param param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(battleOptionSetId);
        if (param == null)
        {
            return;
        }

        mListBattleOptionSet.Add(param);
        Parse(1);
    }
}
