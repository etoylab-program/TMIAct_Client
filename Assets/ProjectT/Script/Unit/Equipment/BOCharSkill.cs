
using System.Collections.Generic;


public class BOCharSkill : BattleOption
{
    public BOCharSkill(GameTable.CharacterSkillPassive.Param paramCharSkill, Unit owner) : base(owner)
    {
        mToExecuteType = eToExecuteType.Unit;
        mListBattleOptionSet.Clear();

        if (paramCharSkill.CharAddBOSetID1 > 0)
        {
            GameClientTable.BattleOptionSet.Param param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(paramCharSkill.CharAddBOSetID1);
            if (param == null)
            {
                Debug.LogError(string.Format("{0}번 배틀옵션셋 데이터가 없습니다.", paramCharSkill.CharAddBOSetID1));
                return;
            }

            mListBattleOptionSet.Add(param);
        }

        if (paramCharSkill.CharAddBOSetID2 > 0)
        {
            GameClientTable.BattleOptionSet.Param param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(paramCharSkill.CharAddBOSetID2);
            if (param == null)
            {
                Debug.LogError(string.Format("{0}번 배틀옵션셋 데이터가 없습니다.", paramCharSkill.CharAddBOSetID2));
                return;
            }

            mListBattleOptionSet.Add(param);
        }

        Parse(1, paramCharSkill.ID);
    }

    public void AddBattleOption(int battleOptionSetId, int charSkillTableId)
    {
        GameClientTable.BattleOptionSet.Param find = mListBattleOptionSet.Find(x => x.ID == battleOptionSetId);
        if(find != null)
        {
            return;
        }

        GameClientTable.BattleOptionSet.Param param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(battleOptionSetId);
        if (param == null)
        {
            Debug.LogError(string.Format("추가할 {0}번 배틀옵션셋 데이터가 없습니다.", battleOptionSetId));
            return;
        }

        mListBattleOptionSet.Add(param);
        AddBattleOptionSet(param, 1, charSkillTableId);
    }

    public void AddBattleOptionData(sBattleOptionData data)
    {
        sBattleOptionData find = ListBattleOptionData.Find(x => x == data);
        if (find != null)
        {
            return;
        }

        ListBattleOptionData.Add(data);
    }

    public void ChangeBattleOptionValue1(int battleOptionSetId, int actionTableId, float value1)
    {
        List<sBattleOptionData> listFind = ListBattleOptionData.FindAll(x => x.battleOptionSetId == battleOptionSetId && x.actionTableId == actionTableId);
        if (listFind == null || listFind.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < listFind.Count; i++)
        {
            listFind[i].value = value1;

            if (listFind[i].evt != null)
            {
                BuffEvent buffEvt = listFind[i].evt as BuffEvent;
                if (buffEvt != null)
                {
                    buffEvt.ChangeValue1(value1);
                }
            }
        }
    }

    public void ChangeBattleOptionDuration(eBOTimingType timingType, int actionTableId, float duration)
    {
        List<sBattleOptionData> listFind = ListBattleOptionData.FindAll(x => x.timingType == timingType && x.actionTableId == actionTableId);
        if (listFind == null || listFind.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < listFind.Count; i++)
        {
            listFind[i].duration = duration;
            listFind[i].originalDuration = duration;

            if (listFind[i].evt != null)
            {
                BuffEvent buffEvt = listFind[i].evt as BuffEvent;
                if(buffEvt != null)
                {
                    buffEvt.ChangeDuration(duration);
                }
            }
        }
    }

    public void ChangeBattleOptionDuration(int battleOptionSetId, int actionTableId, float duration)
    {
        List<sBattleOptionData> listFind = ListBattleOptionData.FindAll(x => x.battleOptionSetId == battleOptionSetId && x.actionTableId == actionTableId);
        if (listFind == null || listFind.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < listFind.Count; i++)
        {
            listFind[i].duration = duration;
            listFind[i].originalDuration = duration;

            if (listFind[i].evt != null)
            {
                BuffEvent buffEvt = listFind[i].evt as BuffEvent;
                if (buffEvt != null)
                {
                    buffEvt.ChangeDuration(duration);
                }
            }
        }
    }

    public void IncreaseBattleOptionDuration(eBOTimingType timingType, int actionTableId, float addRatio)
    {
        List<sBattleOptionData> listFind = ListBattleOptionData.FindAll(x => x.timingType == timingType && x.actionTableId == actionTableId);
        if (listFind == null || listFind.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < listFind.Count; i++)
        {
            listFind[i].duration = listFind[i].originalDuration + (listFind[i].originalDuration * addRatio) + listFind[i].AddDuration;

            if (listFind[i].evt != null)
            {
                BuffEvent buffEvt = listFind[i].evt as BuffEvent;
                if (buffEvt != null)
                {
                    buffEvt.ChangeDuration(listFind[i].duration);
                }
            }
        }
    }

    public void ChangeBattleOptionSetRandomStart( int battleOptionSetId, int randomStart ) {
        List<sBattleOptionData> listFind = ListBattleOptionData.FindAll( x => x.battleOptionSetId == battleOptionSetId );
        if( listFind == null || listFind.Count <= 0 ) {
            return;
        }

        for( int i = 0; i < listFind.Count; i++ ) {
            listFind[i].randomStart = randomStart;
        }
    }

    public void EndBattleOption(eBOTimingType timingType, int actionTableId)
    {
        List<sBattleOptionData> listFind = ListBattleOptionData.FindAll(x => x.timingType == timingType && x.actionTableId == actionTableId);
        if(listFind == null || listFind.Count <= 0)
        {
            return;
        }

        for(int i = 0; i < listFind.Count; i++)
        {
            mOwner.EndBattleOption(listFind[i].battleOptionSetId);
        }
    }
}
