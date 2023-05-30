using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIArenaRankingListPopup : FComponent
{
    [SerializeField]
    private FList _pvpRankingList;
    private ArenaRankingListData _arenaRankingListData;

    private int _userIdx;

	public override void Awake()
	{
		base.Awake();

        if (this._pvpRankingList == null) return;

        this._pvpRankingList.InitBottomFixing();
        this._pvpRankingList.EventUpdate = this._UpdatePvpRankingListSlot;
        this._pvpRankingList.EventGetItemCount = this._GetPvpRankingElementCount;
    }
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        _arenaRankingListData = GameInfo.Instance.ArenaRankingList;

        _userIdx = -1;
        for(int i = 0; i < _arenaRankingListData.RankingSimpleList.Count; i++)
        {
            if(_arenaRankingListData.RankingSimpleList[i].UUID == GameInfo.Instance.UserData.UUID)
            {
                _userIdx = i;
                break;
            }
        }

        _pvpRankingList.UpdateList();

        if (_userIdx != -1)
        {
            if (_pvpRankingList.IsScroll)
            {
                if (_userIdx > _pvpRankingList.RowCount)
                {
                    _pvpRankingList.SpringSetFocus(_userIdx);
                }
            }

            _userIdx = -1;
        }
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

       
    }

	private void _UpdatePvpRankingListSlot(int index, GameObject slotObject)
	{
		do
		{
            UIPvpAllRewardSlot slot = slotObject.GetComponent<UIPvpAllRewardSlot>();
            if (null == slot) break;


            TeamData data = null;
            if (0 <= index && _arenaRankingListData.RankingSimpleList.Count > index)
            {
                data = _arenaRankingListData.RankingSimpleList[index];
            }

            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(index, data);

        } while(false);
	}
	
	private int _GetPvpRankingElementCount()
	{
        if (_arenaRankingListData == null || _arenaRankingListData.RankingSimpleList.Count <= 0)
            return 0;
		return _arenaRankingListData.RankingSimpleList.Count; //TempValue
    }
	
	public void OnClick_CloseBtn()
	{
        OnClickClose();
	}
}
