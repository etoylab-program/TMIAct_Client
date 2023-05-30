using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using eKeyKind = BaseCustomInput.eKeyKind;

public class UIKeySetupPopup : FComponent
{

	[SerializeField] private FList _KeySetupListInstance;

	private List<eKeyKind> _listKeys;
	public override void Awake()
	{
		base.Awake();

		if(this._KeySetupListInstance == null) return;
		
		this._KeySetupListInstance.EventUpdate = this._UpdateKeySetupListSlot;
		this._KeySetupListInstance.EventGetItemCount = this._GetKeySetupElementCount;
	}
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
		_listKeys = new List<eKeyKind>();

		for(eKeyKind k = eKeyKind.None + 1; k < eKeyKind.PCCount; k++)
        {
			_listKeys.Add(k);
		}

		_KeySetupListInstance.UpdateList();
	}
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}
	
	private void _UpdateKeySetupListSlot(int index, GameObject slotObject)
	{
		do
		{
			//Do UpdateListSlot
			UIKeySetupListSlot slot = slotObject.GetComponent<UIKeySetupListSlot>();
			slot.ParentGO = gameObject;
			slot.SetData(_listKeys[index]);

			slot.UpdateSlot();

		}while(false);
	}
	
	private int _GetKeySetupElementCount()
	{
		return _listKeys.Count; //TempValue
	}

	
	public void OnClick_BackBtn()
	{
		OnClickClose();
	}
	
	public void OnClick_InitSetBtn()
	{
		AppMgr.Instance.CustomInput.InitPCKeyMapping();
		Refresh();
	}

	public void Refresh()
    {
		AppMgr.Instance.CustomInput.SavePCKeySettings();
		_KeySetupListInstance.RefreshNotMove();
	}
}
