using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPackageItemInfoUnit : FUnit
{
    public GameObject kItem_2;
    public List<UIRewardListSlot> kItem_2_RewardListSlot;
    public GameObject kItem_3;
    public List<UIRewardListSlot> kItem_3_RewardListSlot;
    public GameObject kItem_4;
    public List<UIRewardListSlot> kItem_4_RewardListSlot;
    public GameObject kItem_5;
    public List<UIRewardListSlot> kItem_5_RewardListSlot;
    public GameObject kItem_6;
    public List<UIRewardListSlot> kItem_6_RewardListSlot;
    public GameObject kItem_Args;

    [SerializeField] private FList _item_argsListInxtance;
    private List<GameTable.Random.Param> _rewardList = new List<GameTable.Random.Param>();

    private int _storeID;

    public void UpdateSlot(int storeID)
    {
        Init();

        _storeID = storeID;

        GameTable.Store.Param storeTableData = GameInfo.Instance.GameTable.FindStore(x => x.ID == _storeID);
        if (storeTableData == null)
            return;

        _rewardList.Clear();

        _rewardList = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == storeTableData.ProductIndex && x.ProductType != (int)eREWARDTYPE.BUFF);
        if (_rewardList == null || _rewardList.Count <= 0)
            return;

        if(_rewardList.Count == 2)
            SetRewardListSlot(kItem_2, kItem_2_RewardListSlot);
        else if(_rewardList.Count == 3)
            SetRewardListSlot(kItem_3, kItem_3_RewardListSlot);
        else if(_rewardList.Count == 4)
            SetRewardListSlot(kItem_4, kItem_4_RewardListSlot);
        else if(_rewardList.Count == 5)
            SetRewardListSlot(kItem_5, kItem_5_RewardListSlot);
        else if(_rewardList.Count == 6)
            SetRewardListSlot(kItem_6, kItem_6_RewardListSlot);
        else
        {
            kItem_Args.SetActive(true);
            _item_argsListInxtance.UpdateList();
        }

    }

    private void Init()
    {
        kItem_2.SetActive(false);
        kItem_3.SetActive(false);
        kItem_4.SetActive(false);
        kItem_5.SetActive(false);
        kItem_6.SetActive(false);
        kItem_Args.SetActive(false);

        if (this._item_argsListInxtance == null) return;

        if (_item_argsListInxtance.EventUpdate == null)
            _item_argsListInxtance.EventUpdate = this._UpdatePackageItemInfoListSlot;

        if (_item_argsListInxtance.EventGetItemCount == null)
            _item_argsListInxtance.EventGetItemCount = this._GetPackageItemInfoListCount;

        this._item_argsListInxtance.InitBottomFixing();
    }

    private void _UpdatePackageItemInfoListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIRewardListSlot slot = slotObject.GetComponent<UIRewardListSlot>();
            if (null == slot) break;
            slot.ParentGO = this.gameObject;

            GameTable.Random.Param data = null;
            if (0 <= index && _rewardList.Count > index)
                data = _rewardList[index];

            slot.UpdateSlot(new RewardData(data.ProductType, data.ProductIndex, data.ProductValue), true);
        } while (false);
    }

    private int _GetPackageItemInfoListCount()
    {
        if (_rewardList == null || _rewardList.Count <= 0)
            return 0;

        return _rewardList.Count;
    }

    private void SetRewardListSlot(GameObject target, List<UIRewardListSlot> targetList)
    {
        target.SetActive(true);
        for(int i = 0; i < _rewardList.Count; i++)
        {
            targetList[i].UpdateSlot(new RewardData(_rewardList[i].ProductType, _rewardList[i].ProductIndex, _rewardList[i].ProductValue), true);
        }
    }
}
