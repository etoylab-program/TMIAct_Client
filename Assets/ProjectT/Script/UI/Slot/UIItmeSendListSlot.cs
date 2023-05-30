using UnityEngine;
using System.Collections;

public class UIItmeSendListSlot : FSlot 
{
	public UILabel kItemSendSlotLabel;

	private int _index;
	private GameClientTable.Acquisition.Param _data;

	public void UpdateSlot(int index, GameClientTable.Acquisition.Param data) 	//Fill parameter if you need
	{
		_index = index;
		_data = data;

		kItemSendSlotLabel.textlocalize = FLocalizeString.Instance.GetText(_data.Desc);
	}
 
	public void OnClick_Slot()
	{
		if (_data == null)
			return;

		if (ParentGO == null)
			return;
		
		LobbyUIManager.Instance.HideAll( FComponent.TYPE.Popup, false );

		GameSupport.MoveUI(_data.Type, _data.Value1, _data.Value2, _data.Value3);
	}
 
}
