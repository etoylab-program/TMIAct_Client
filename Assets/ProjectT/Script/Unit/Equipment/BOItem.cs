
using System.Collections.Generic;


public class BOItem : BattleOption
{
    public BOItem(int battleOptionSetId, Unit owner) : base(owner)
    {
        mToExecuteType = eToExecuteType.Item;
        mListBattleOptionSet.Clear();

        GameClientTable.BattleOptionSet.Param param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(battleOptionSetId);
        if (param == null)
            return;

        mListBattleOptionSet.Add(param);
        Parse(1);
    }

    public void ChangeSender(Unit sender)
    {
        for (int i = 0; i < ListBattleOptionData.Count; i++)
        {
            if (ListBattleOptionData[i].evt != null)
            {
                BuffEvent buffEvt = ListBattleOptionData[i].evt as BuffEvent;
                if (buffEvt != null)
                {
                    buffEvt.ChangeSender(sender);
                }
            }
        }
    }

    public void AddBattleOptionValue1(float add)
    {
        for (int i = 0; i < ListBattleOptionData.Count; i++)
        {
            ListBattleOptionData[i].value += (ListBattleOptionData[i].value * add);

            if (ListBattleOptionData[i].evt != null)
            {
                BuffEvent buffEvt = ListBattleOptionData[i].evt as BuffEvent;
                if (buffEvt != null)
                {
                    buffEvt.ChangeValue1(ListBattleOptionData[i].value);
                }
            }
        }
    }

    public float GetValueFromTable(int listIndex)
    {
        if(listIndex < 0 || listIndex >= ListBattleOptionData.Count)
        {
            return 0.0f;
        }

        GameClientTable.BattleOptionSet.Param param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(ListBattleOptionData[listIndex].battleOptionSetId);
        if (param == null)
        {
            return 0.0f;
        }

        return param.BOFuncValue;
    }
}
