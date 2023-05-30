using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFriendTooltipPopup : FComponent
{
	[SerializeField] private FList _FriendList;
    [SerializeField] private UILabel LbEmpty;

	private List<FriendUserData> ClearFriendList = null;
	private int ChoiceStageID = 0;


    public override void Awake()
	{
		base.Awake();

        _FriendList.EventUpdate = UpdateListSlot;
        _FriendList.EventGetItemCount = () => { return (ClearFriendList == null) ? 0 : ClearFriendList.Count; };
        _FriendList.InitBottomFixing();
    }
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        ChoiceStageID = (int)UIValue.Instance.GetValue(UIValue.EParamType.ArenaTowerChoiceStage, true);
        ClearFriendList = GameInfo.Instance.CommunityData.FriendList.FindAll(x => x.ClearArenaTowerID >= ChoiceStageID);
        //ClearFriendList = GameInfo.Instance.CommunityData.FriendList;

        _FriendList.gameObject.SetActive(false);
        LbEmpty.gameObject.SetActive(false);
    }

    public void OnAnimationEvent()
    {
        if (ClearFriendList.Count > 0)
        {
            _FriendList.gameObject.SetActive(true);
            LbEmpty.gameObject.SetActive(false);

            _FriendList.UpdateList();
        }
        else
        {
            _FriendList.gameObject.SetActive(false);
            LbEmpty.gameObject.SetActive(true);
        }
    }

    private void UpdateListSlot(int index, GameObject slotObj)
    {
		UIFriendTooltipListSlot slot = slotObj.GetComponent<UIFriendTooltipListSlot>();
        if (slot == null) return;
		slot.UpdateSlot(index, ClearFriendList[index]);
	}
}
