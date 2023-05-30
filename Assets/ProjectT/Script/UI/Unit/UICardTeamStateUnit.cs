using UnityEngine;
using System.Collections;

public class UICardTeamStateUnit : FUnit {
 

	public UISprite kNew;
	public UISprite kLevel;
	public UISprite kFaver;
 
	public void UpdateSlot(bool bNew, bool bLevel, bool bFavor) 	//Fill parameter if you need
	{
		if (bNew)
			kNew.spriteName = "item_new";
		else
			kNew.spriteName = "item_new_dimd";

		if (bLevel)
			kLevel.spriteName = "itemwake_03";
		else
			kLevel.spriteName = "itemwake_dimd_03";

		if (bFavor)
			kFaver.spriteName = "itemLove_0";
		else
			kFaver.spriteName = "itemLove_dimd_0";
	}
 
	public void OnClick_Slot()
	{
	}
 
}
