
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BOCharacter : BattleOption
{
    public BOCharacter(int[] battleOptionSetIds, Unit owner) : base(owner)
    {
        mToExecuteType = eToExecuteType.Unit;
        mListBattleOptionSet.Clear();

        for (int i = 0; i < battleOptionSetIds.Length; i++)
        {
            if(!AddBattleOptionSet(battleOptionSetIds[i], 1))
            {
                continue;
            }
        }
    }

    public BOCharacter(int battleOptionSetId, Unit owner) : base(owner)
    {
        mToExecuteType = eToExecuteType.Unit;
        mListBattleOptionSet.Clear();

        AddBattleOptionSet(battleOptionSetId, 1);
    }

    public bool AddBattleOptionSet(int battleOptionSetId, int level)
    {
        GameClientTable.BattleOptionSet.Param param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(battleOptionSetId);
        if (param == null)
        {
            Debug.LogError(battleOptionSetId + "번 배틀옵션셋이 없음");
            return false;
        }

        mListBattleOptionSet.Add(param);
        Parse(param, level);

        return true;
    }

    protected void Parse(GameClientTable.BattleOptionSet.Param paramBOSet, int level, int actionTableId = -1)
    {
        if(paramBOSet == null)
        {
            return;
        }

        AddBattleOptionSet(paramBOSet, level, actionTableId);
    }
}
