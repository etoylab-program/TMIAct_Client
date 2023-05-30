using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGameBuffDebuffinfoPopup : FComponent
{

	[SerializeField] private FList _BuffListInstance;
	[SerializeField] private FList _DebuffListListInstance;

    private List<GameClientTable.HelpBuffDebuff.Param> _buffList;
    private List<GameClientTable.HelpBuffDebuff.Param> _DeBuffList;

	public override void Awake()
	{
		base.Awake();

		if(this._BuffListInstance == null) return;
		
		this._BuffListInstance.EventUpdate = this._UpdateBuffListSlot;
		this._BuffListInstance.EventGetItemCount = this._GetBuffElementCount;
        this._BuffListInstance.InitBottomFixing();
		if(this._DebuffListListInstance == null) return;
		
		this._DebuffListListInstance.EventUpdate = this._UpdateDebuffListListSlot;
		this._DebuffListListInstance.EventGetItemCount = this._GetDebuffElementCount;
        this._DebuffListListInstance.InitBottomFixing();
	}
 
	public override void OnEnable()
	{
		_buffList = GameInfo.Instance.GameClientTable.FindAllHelpBuffDebuff( x => x.BuffType == (int)eBuffDebuffType.Buff );
		_DeBuffList = GameInfo.Instance.GameClientTable.FindAllHelpBuffDebuff( x => x.BuffType == (int)eBuffDebuffType.Debuff );

		base.OnEnable();
	}

	private void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnClick_BackBtn();
        }
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        _BuffListInstance.UpdateList();
        _DebuffListListInstance.UpdateList();
	}
 

	
	private void _UpdateBuffListSlot(int index, GameObject slotObject)
	{
		do
		{
            UIBuffDebuffSlotSlot slot = slotObject.GetComponent<UIBuffDebuffSlotSlot>();
            GameClientTable.HelpBuffDebuff.Param data = null;
            if (null == slot) break;
            if (0 <= index && _buffList.Count > index)
            {
                data = _buffList[index];
            }
            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(data);
        } while(false);
	}
	
	private int _GetBuffElementCount()
	{
        if(_buffList == null || _buffList.Count <= 0)
		    return 0; //TempValue

        return _buffList.Count;
    }
	
	private void _UpdateDebuffListListSlot(int index, GameObject slotObject)
	{
		do
		{
            UIBuffDebuffSlotSlot slot = slotObject.GetComponent<UIBuffDebuffSlotSlot>();
            GameClientTable.HelpBuffDebuff.Param data = null;
            if (null == slot) break;
            if (0 <= index && _DeBuffList.Count > index)
            {
                data = _DeBuffList[index];
            }
            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(data);
        } while(false);
	}
	
	private int _GetDebuffElementCount()
	{
        if(_DeBuffList == null || _DeBuffList.Count <= 0)
		    return 0; //TempValue

        return _DeBuffList.Count;
    }

	
	public void OnClick_BackBtn()
	{
        OnClose();
        //OnClickClose();
	}
}
