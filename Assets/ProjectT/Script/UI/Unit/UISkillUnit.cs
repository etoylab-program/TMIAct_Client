using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class UISkillUnit : FUnit
{
    public UISprite kTitleSpr;
    public List<GameObject> kCommands;

    public void UpdateSlot(GameTable.CharacterSkillPassive.Param skillInfo) 	//Fill parameter if you need
	{
        GameSupport.SetSkillSprite(ref kTitleSpr, skillInfo.Atlus, skillInfo.Icon);

        for (int i = 0; i < kCommands.Count; i++)
        {
            kCommands[i].gameObject.SetActive(false);
        }

        int index = skillInfo.CommandIndex;
        kCommands[index].SetActive(true);

    }
}
