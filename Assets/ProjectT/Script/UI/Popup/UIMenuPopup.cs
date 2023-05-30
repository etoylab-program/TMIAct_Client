using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMenuPopup : FComponent
{

	[SerializeField] private FList _MenuListInstance;
	public UIButton kCloseBtn;
    private List<GameClientTable.Menu.Param> _menulist = new List<GameClientTable.Menu.Param>();
	public override void Awake()
	{
		base.Awake();

		if(this._MenuListInstance == null) return;
		
		this._MenuListInstance.EventUpdate = this._UpdateMenuListSlot;
		this._MenuListInstance.EventGetItemCount = this._GetMenuElementCount;
        this._MenuListInstance.InitBottomFixing();

    }
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        if (_menulist != null)
            _menulist.Clear();

#if UNITY_EDITOR
        for (int i = 0; i < GameInfo.Instance.GameClientTable.Menus.Count; i++)
        {
            var data = GameInfo.Instance.GameClientTable.Menus[i];
            if (data.PreVisible != 0) 
            {
                _menulist.Add(data);
            }
        }

#else
        for ( int i = 0; i < GameInfo.Instance.GameClientTable.Menus.Count; i++ )
        {
            var data = GameInfo.Instance.GameClientTable.Menus[i];
            if( data.PreVisible == 1 ) //일반 메뉴
            {
                _menulist.Add(data);
            }
            else if (data.PreVisible == 2) //언어선택
            {
                if (AppMgr.Instance.configData.m_ServiceType != Config.eServiceType.Japan || AppMgr.Instance.Nocheckdid == true)
                {
                    _menulist.Add(data);
                }
            }
            else if (data.PreVisible == 3) //스팀 전용 설정
            {
                if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam )
                {
                    _menulist.Add(data);
                }
            }
            else if (data.PreVisible == 99) //IOS 아닐 때 설정
            {
#if !UNITY_IOS
                _menulist.Add(data);
#endif
            }
            
        }
#endif


        this._MenuListInstance.UpdateList();
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        this._MenuListInstance.RefreshNotMove();
    }
 
	private void _UpdateMenuListSlot(int index, GameObject slotObject)
	{
		do
		{
            UIMenuListSlot slot = slotObject.GetComponent<UIMenuListSlot>();
            GameClientTable.Menu.Param data = null;
            if (0 <= index && _menulist.Count > index)
            {
                data = _menulist[index];
            }
            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(index, data);
        } while(false);
	}
	
	private int _GetMenuElementCount()
	{
        if (_menulist == null || _menulist.Count <= 0)
            return 0;

		return _menulist.Count;
	}
	
	public void OnClick_CloseBtn()
	{
        OnClickClose();
	}
}
