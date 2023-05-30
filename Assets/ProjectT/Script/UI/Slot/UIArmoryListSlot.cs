using UnityEngine;
using System.Collections;

public class UIArmoryListSlot : FSlot 
{
	public UIItemListSlot kWeaponSlot;
	public GameObject kLockObj;
	public UILabel kWeaponLockLabel;
	public UILabel kConfirmationLabel;
	public UILabel kWeaponTitleLabel;
	public UISprite kCardTypeSpr;
	public UISprite kShortfallSpr;

	private long _weaponUID;
	private bool _addFlag = false;
	private bool _subAddFlag = false;
	private WeaponData _weaponData = null;
	private GameTable.ItemReqList.Param _weaponArmoryLimitData;

	public void UpdateSlot(long weaponUID, GameTable.ItemReqList.Param itemLimitData) 	//Fill parameter if you need
	{
		_weaponUID = weaponUID;
		_weaponData = null;
		_weaponArmoryLimitData = itemLimitData;

		kLockObj.SetActive(false);
		kConfirmationLabel.SetActive(false);
		kWeaponTitleLabel.SetActive(false);

		_addFlag = GameInfo.Instance.WeaponArmoryData.ArmorySlotCnt < itemLimitData.Level;
		_subAddFlag = false;
		if (_addFlag)
		{
			//무기도감 오픈
			kWeaponSlot.UpdateSlot(UIItemListSlot.ePosType.None, 0, _weaponData);

			kLockObj.SetActive(true);
			kConfirmationLabel.SetActive(true);
			kWeaponTitleLabel.SetActive(true);

			int strColor = (int)eTEXTID.RED_TEXT_COLOR;
			int strDesc = 1614;     //확장불가

			if (_weaponArmoryLimitData.LimitLevel <= GameInfo.Instance.WeaponBookList.Count && (_weaponArmoryLimitData.Level - 1 == GameInfo.Instance.WeaponArmoryData.ArmorySlotCnt))
			{
				strColor = (int)eTEXTID.GREEN_TEXT_COLOR;
				strDesc = 1613;     //확장가능
				_subAddFlag = true;
			}

			kWeaponLockLabel.textlocalize = FLocalizeString.Instance.GetText(218, FLocalizeString.Instance.GetText(strColor, GameInfo.Instance.WeaponBookList.Count.ToString()), _weaponArmoryLimitData.LimitLevel.ToString());
			kConfirmationLabel.textlocalize = FLocalizeString.Instance.GetText(strColor, FLocalizeString.Instance.GetText(strDesc));
		}
		else
		{
			if (weaponUID == (int)eCOUNT.NONE)
			{
				kWeaponSlot.UpdateSlot(UIItemListSlot.ePosType.None, 0, _weaponData);
			}
			else
			{
				_weaponData = GameInfo.Instance.GetWeaponData(weaponUID);
				kWeaponSlot.UpdateSlot(UIItemListSlot.ePosType.EquipWeapon, 0, _weaponData);
			}
		}
	}
 
	public void OnClick_Slot()
	{
		if (_addFlag)
		{
			if (!_subAddFlag)
			{
				Debug.LogError("Not Opened");
				MessageToastPopup.Show(FLocalizeString.Instance.GetText(3232));
				return;
			}
			ItemReqListPopup.ShowItemReqListPopup(FLocalizeString.Instance.GetText(1618), FLocalizeString.Instance.GetText(1619), _weaponArmoryLimitData, OnItemReqCheck);
		}
		else
		{
			UIValue.Instance.SetValue(UIValue.EParamType.ArmoryWeaponUID, _weaponUID);
			LobbyUIManager.Instance.ShowUI("FacilityWeaponSeletePopup", true);
		}
	}

	public void OnItemReqCheck()
	{
		if (!GameSupport.IsItemReqList(_weaponArmoryLimitData))
		{
			//소재가 부족합니다.
			MessageToastPopup.Show(FLocalizeString.Instance.GetText(3003));
			return;
		}

		GameInfo.Instance.Send_ReqAddSlotInWpnDepot(1, OnNet_AckAddSlotInWpnDepot);
	}

	public void OnNet_AckAddSlotInWpnDepot(int result, PktMsgType pktmsg)
	{
		if (result != 0)
			return;

		LobbyUIManager.Instance.Renewal("ArmoryPopup");
		LobbyUIManager.Instance.Renewal("TopPanel");
	}


	public void OnClick_ItemListSlot()
	{
	}
}
