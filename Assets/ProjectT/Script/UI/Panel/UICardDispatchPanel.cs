
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UICardDispatchPanel : FComponent
{
    [SerializeField]
    private FList _listSlot = null;
    private int _listCount = 0;
    private List<GameTable.CardDispatchSlot.Param> tableDatas { get { return GameInfo.Instance.GameTable.CardDispatchSlots; } }

    public override void OnEnable()
	{
        InitComponent();
		base.OnEnable();
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();
		ReleaseSupports();
    }

    public override void InitComponent()
	{
        _listCount = tableDatas.Count;

        _listSlot.EventUpdate = _UpdateItemListSlot;
        _listSlot.EventGetItemCount = () => { return _listCount; };
        _listSlot.InitBottomFixing();
        _listSlot.UpdateList();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);
        _listSlot.RefreshNotMove();
    }
    private void _UpdateItemListSlot(int index, GameObject slotObject)
    {
        UICardDispatchListSlot slot = slotObject.GetComponent<UICardDispatchListSlot>();
        if (slot == null) return;

        int dataIndex = index + 1;
        var findData = tableDatas.Find(x => x.Index == dataIndex);
        if (findData == null) return;

        slot.UpdateSlot(index, findData);
    }

    private void ReleaseSupports()
    {
        var openList = GameInfo.Instance.Dispatches.FindAll(x => x.EndTime <= DateTime.MinValue || x.EndTime >= DateTime.MaxValue || x.EndTime.Ticks <= 0);
        if (openList == null || openList.Count == 0) return;

        foreach(var dispatch in openList)
        {
            var supports = GameInfo.Instance.CardList.FindAll(x =>
                x.PosKind == (int)eContentsPosKind.DISPATCH &&
                x.PosValue == dispatch.TableID);

            if (supports == null || supports.Count == 0) continue;

            foreach(var card in supports)
            {
                card.InitPos();
            }
            dispatch.RefreshData();
        }
    }
}
