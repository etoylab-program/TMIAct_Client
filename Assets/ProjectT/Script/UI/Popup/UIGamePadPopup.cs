using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGamePadPopup : FComponent
{
	public FTab kGamePadTab;
	public GameObject kGame;
	public GameObject kFigureRoom;
	//public List<FLocalizeText> GameKeyList;
	//public List<FLocalizeText> FigureKeyList;
	public override void Awake()
	{
		kGamePadTab.EventCallBack = OnGamePadTabSelect;

		base.Awake();
	}
 
	public override void OnEnable()
	{
		kGamePadTab.SetTab(0, SelectEvent.Code);
		base.OnEnable();
	}
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}

	private bool OnGamePadTabSelect(int nSelect, SelectEvent type)
	{
		if(nSelect == 0)
        {
			kGame.SetActive(true);
			kFigureRoom.SetActive(false);
		}
		else
        {
			kGame.SetActive(false);
			kFigureRoom.SetActive(true);
		}
		return true;
	}

	public void OnClick_BackBtn()
    {
		OnClickClose();
    }


}
