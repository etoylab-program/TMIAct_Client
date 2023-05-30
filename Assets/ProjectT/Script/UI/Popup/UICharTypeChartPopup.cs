using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharTypeChartPopup : FComponent
{
	public UIButton kCloseBtn;
	[SerializeField] private FList _CharTypeChartListInstance;
	public UISprite klineSpr;
	public UISprite kAdvantageSpr;
	public UISprite kDisadvantageSpr;

    private List<GameClientTable.HelpCharInfo.Param> _helpCharInfoList;

	public override void Awake()
	{
		base.Awake();

		if(this._CharTypeChartListInstance == null) return;

        this._CharTypeChartListInstance.InitBottomFixing();
		this._CharTypeChartListInstance.EventUpdate = this._UpdateCharTypeChartListSlot;
		this._CharTypeChartListInstance.EventGetItemCount = this._GetCharTypeChartElementCount;
	}
 
	public override void OnEnable()
	{
		_helpCharInfoList = GameInfo.Instance.GameClientTable.HelpCharInfos;
		base.OnEnable();
	}
 
	public override void Renewal(bool bChildren)
	{
        _CharTypeChartListInstance.UpdateList();
        base.Renewal(bChildren);

    }

    private void _UpdateCharTypeChartListSlot(int index, GameObject slotObject)
	{
		do
		{
            UICharTypeChartListSlot slot = slotObject.GetComponent<UICharTypeChartListSlot>();

            if (null == slot) break;

            GameClientTable.HelpCharInfo.Param data = null;
            if(0 <= index && _helpCharInfoList.Count > index)
            {
                data = _helpCharInfoList[index];
            }

            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(index, data);
		}while(false);
	}
	
	private int _GetCharTypeChartElementCount()
	{
        if(_helpCharInfoList == null)
		    return 0;

        return _helpCharInfoList.Count;
    }
	
	public void OnClick_CloseBtn()
	{
        OnClickClose();
	}
}
