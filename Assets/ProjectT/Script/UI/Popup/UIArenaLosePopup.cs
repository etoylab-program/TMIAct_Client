using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIArenaLosePopup : FComponent
{
	public UIArenaBattleListSlot kArenaBattleListSlot;
	public UIButton kCloseBtn;
 

	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
        kArenaBattleListSlot.UpdateSlot(false, GameInfo.Instance.MatchTeam, true);
	}
 
	public void OnClick_CloseBtn()
	{
        OnClickClose();
	}
}
