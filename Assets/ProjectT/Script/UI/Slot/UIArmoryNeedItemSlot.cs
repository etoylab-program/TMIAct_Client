using UnityEngine;
using System.Collections;

public class UIArmoryNeedItemSlot : FSlot 
{
	public UIItemListSlot kWeaponSlot;
	public GameObject kEmptyObj;

	private GameClientTable.Book.Param _weaponBookTableData;
	public void UpdateSlot(GameClientTable.Book.Param weaponBookTableData) 	//Fill parameter if you need
	{
		kWeaponSlot.SetActive(false);
		kEmptyObj.SetActive(false);
		_weaponBookTableData = weaponBookTableData;
		if (_weaponBookTableData == null)
		{
			kEmptyObj.SetActive(true);
			return;
		}

		if (GameInfo.Instance.WeaponBookList.Find(x => x.TableID == _weaponBookTableData.ItemID) == null)
			kEmptyObj.SetActive(true);
		else
		{
			kWeaponSlot.SetActive(true);
			kWeaponSlot.UpdateSlot(UIItemListSlot.ePosType.Book, 0, _weaponBookTableData);
			kWeaponSlot.kInactiveSpr.SetActive(!GameSupport.GetEquipWeaponDepotTableID(_weaponBookTableData.ItemID));
		}
			
	}
 
	public void OnClick_Slot()
	{
	}
 

	
	public void OnClick_WeaponSlot()
	{
	}
}
