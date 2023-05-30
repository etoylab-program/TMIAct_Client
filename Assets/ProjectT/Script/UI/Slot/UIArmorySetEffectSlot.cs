using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIArmorySetEffectSlot : FSlot {
 

	public UISprite kSelectSpr;
	public UILabel kNameLabel;
	public UILabel kCntLabel;
	public UILabel kTotalEffectLabel;

	private int _index = 0;
	private int _selectIdx = 0;
	private int _setEffectGroupID = 0;

    public void UpdateSlot(int index, int selectIdx, int setEffectGroupID) 	//Fill parameter if you need
	{
		_index = index;
		_selectIdx = selectIdx;
		_setEffectGroupID = setEffectGroupID;
		kSelectSpr.SetActive(index == selectIdx);

		List<GameClientTable.WpnDepotSet.Param> wpnDepotList = GameInfo.Instance.GameClientTable.FindAllWpnDepotSet(x => x.GroupID == setEffectGroupID);

		kNameLabel.textlocalize = FLocalizeString.Instance.GetText(wpnDepotList[0].Name);

		int wpnDepotCnt = GameSupport.GetEquipArmoryWeaponDepot(_setEffectGroupID);

		int equipStrColor = (int)eTEXTID.WHITE_TEXT_COLOR;
		if (wpnDepotCnt > 0)
			equipStrColor = (int)eTEXTID.GREEN_TEXT_COLOR;
		string equipStr = FLocalizeString.Instance.GetText(equipStrColor, wpnDepotCnt);

		kCntLabel.textlocalize = FLocalizeString.Instance.GetText(218, equipStr, wpnDepotList[wpnDepotList.Count - 1].ReqCnt);

		kTotalEffectLabel.textlocalize = string.Format("+{0}%", GameSupport.GetWeaponDepotEffectValue(_setEffectGroupID).ToString("N2"));
	}
 
	public void OnClick_Slot()
	{
		if (ParentGO == null)
			return;

		UIWeaponDepotSetPopup uIWeaponDepotSetPopup = ParentGO.GetComponent<UIWeaponDepotSetPopup>();
		if (uIWeaponDepotSetPopup != null)
		{
			uIWeaponDepotSetPopup.SetClick_WeaponDepotSetIdx(_index);
		}
	}
}
