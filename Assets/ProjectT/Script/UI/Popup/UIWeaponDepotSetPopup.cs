using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWeaponDepotSetPopup : FComponent
{
	public UILabel kValueText;
	public UILabel kTotalLabel;
	[SerializeField] private FList _EquippedWeaponListInstance;
	public UILabel kSupportTeamLabel;
	public UILabel kSelDepotSetLabel;
	[SerializeField] private FList _ArsenalListInstance;

	private int _selectEffectIdx = 0;
	private List<GameClientTable.WpnDepotSet.Param> _weaponDepotSetList = new List<GameClientTable.WpnDepotSet.Param>();
	private List<GameClientTable.WpnDepotSet.Param> _selectDepotSetList = new List<GameClientTable.WpnDepotSet.Param>();

	private List<GameClientTable.Book.Param> _weaponBookList = new List<GameClientTable.Book.Param>();
	private List<GameClientTable.Book.Param> _selectWeaponBookList = new List<GameClientTable.Book.Param>();

	public override void Awake()
	{
		base.Awake();

		if(this._EquippedWeaponListInstance == null) return;
		
		this._EquippedWeaponListInstance.EventUpdate = this._UpdateEquippedWeaponListSlot;
		this._EquippedWeaponListInstance.EventGetItemCount = this._GetEquippedWeaponElementCount;
		this._EquippedWeaponListInstance.InitBottomFixing();
		if (this._ArsenalListInstance == null) return;
		
		this._ArsenalListInstance.EventUpdate = this._UpdateArsenalListSlot;
		this._ArsenalListInstance.EventGetItemCount = this._GetArsenalElementCount;
	}
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
		_weaponBookList = GameInfo.Instance.GameClientTable.Books.FindAll(x => x.Group == (int)eBookGroup.Weapon);

		_weaponDepotSetList.Clear();

		for (int i = 0; i < GameInfo.Instance.GameClientTable.WpnDepotSets.Count; i++)
		{
			if (_weaponDepotSetList.Find(x => x.GroupID == GameInfo.Instance.GameClientTable.WpnDepotSets[i].GroupID) == null)
			{
				_weaponDepotSetList.Add(GameInfo.Instance.GameClientTable.WpnDepotSets[i]);
			}
		}
		_selectEffectIdx = 0;
		_EquippedWeaponListInstance.UpdateList();
	}
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

		_EquippedWeaponListInstance.Refresh();

		_selectDepotSetList = GameInfo.Instance.GameClientTable.FindAllWpnDepotSet(x => x.GroupID == _weaponDepotSetList[_selectEffectIdx].GroupID);

		string descStr = string.Empty;
		int equipCnt = GameSupport.GetEquipArmoryWeaponDepot(_weaponDepotSetList[_selectEffectIdx].GroupID);
		Debug.LogError("SelIdx : " + _weaponDepotSetList[_selectEffectIdx].GroupID + " / " + equipCnt);
		for (int i = 0; i < _selectDepotSetList.Count; i++)
		{
			int colorNum = (int)eTEXTID.WHITE_TEXT_COLOR;
			if (_selectDepotSetList[i].ReqCnt <= equipCnt)
			{
				colorNum = (int)eTEXTID.GREEN_TEXT_COLOR;
			}

			string tempStr = FLocalizeString.Instance.GetText(colorNum, FLocalizeString.Instance.GetText(1621, _selectDepotSetList[i].ReqCnt, _selectDepotSetList[i].BonusATK.ToString("N2")));
			descStr += tempStr;
			descStr += "\n";
		}

		kValueText.textlocalize = descStr;

		_selectWeaponBookList.Clear();

		for (int i = 0; i < _weaponBookList.Count; i++)
		{
			if (GameSupport.GetArmoryWeaponDepotFlagCheck(_weaponDepotSetList[_selectEffectIdx].GroupID, _weaponBookList[i].ItemID))
				_selectWeaponBookList.Add(_weaponBookList[i]);
		}

		kTotalLabel.textlocalize = string.Format("{0} {1}%", FLocalizeString.Instance.GetText(1615), 
				FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR, FLocalizeString.Instance.GetText((int)eTEXTID.PLUS_TEXT, GameSupport.GetTotalWeaponDepotEffectValue().ToString("N2"))));

		kSelDepotSetLabel.textlocalize = FLocalizeString.Instance.GetText(_selectDepotSetList[(int)eCOUNT.NONE].Name);

		int weaponEquipCnt = GameSupport.GetEquipArmoryWeaponDepot(_selectDepotSetList[(int)eCOUNT.NONE].GroupID);

		kSupportTeamLabel.textlocalize = FLocalizeString.Instance.GetText(1622, FLocalizeString.Instance.GetText(218, weaponEquipCnt, _selectWeaponBookList.Count));
		_ArsenalListInstance.UpdateList();
	}
 
	private void _UpdateEquippedWeaponListSlot(int index, GameObject slotObject)
	{
		do
		{
			UIArmorySetEffectSlot slot = slotObject.GetComponent<UIArmorySetEffectSlot>();
			if (slot == null) break;

			slot.ParentGO = this.gameObject;
			if (0 <= index && _weaponDepotSetList.Count > index)
			{
				slot.UpdateSlot(index, _selectEffectIdx, _weaponDepotSetList[index].GroupID);
			}
		}while(false);
	}
	
	private int _GetEquippedWeaponElementCount()
	{
		return _weaponDepotSetList.Count;
	}

	public void SetClick_WeaponDepotSetIdx(int index)
	{
		_selectEffectIdx = index;
		Renewal(true);
	}
	
	private void _UpdateArsenalListSlot(int index, GameObject slotObject)
	{
		do
		{
			UIArmoryNeedItemSlot slot = slotObject.GetComponent<UIArmoryNeedItemSlot>();
			if (slot == null) break;

			slot.ParentGO = this.gameObject;
			if (0 <= index && _selectWeaponBookList.Count > index)
			{
				slot.UpdateSlot(_selectWeaponBookList[index]);
			}
		} while(false);
	}
	
	private int _GetArsenalElementCount()
	{
		return _selectWeaponBookList.Count;
	}

	
	public void OnClick_closeBtn()
	{
		OnClickClose();
	}

	public override void OnClickClose()
	{
		base.OnClickClose();
	}
}
