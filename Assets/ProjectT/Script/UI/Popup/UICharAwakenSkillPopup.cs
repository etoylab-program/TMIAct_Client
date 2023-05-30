
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UICharAwakenSkillPopup : FComponent
{
    [Header("[Property]")]
    public FList        SkillSlotList;
    public GameObject   DisableResetObj;

    public int UpgradeSlotIndex { get; set; } = -1;

    private List<GameTable.AwakeSkill.Param>    mListParam  = null;
    private bool                                mbSaveFocus = false;


    public override void Awake()
    {
        base.Awake();

        SkillSlotList.EventUpdate = UpdateAwakenSkillListSlot;
        SkillSlotList.EventGetItemCount = GetAwakenSkillElementCount;

        SkillSlotList.InitBottomFixing();
    }

    public override void OnEnable()
    {
		mListParam = GameInfo.Instance.GameTable.AwakeSkills;
		base.OnEnable();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);
        SkillSlotList.UpdateList();

        if (mbSaveFocus)
        {
            SkillSlotList.LoadSavedFocus();
            mbSaveFocus = false;
        }
        
        DisableResetObj.SetActive(!GameInfo.Instance.UserData.HasAwakenSkill());
    }

    public void SaveListFocus()
    {
        mbSaveFocus = true;
        SkillSlotList.SaveCurrentFocus();
    }

    public override void OnClickClose()
    {
        base.OnClickClose();

        UITopPanel topPanel = LobbyUIManager.Instance.GetUI<UITopPanel>("TopPanel");
        if (topPanel)
        {
            topPanel.SetTopStatePlay(UITopPanel.eTOPSTATE.CHAR);
        }
    }

    public void OnBtnReset()
    {
        LobbyUIManager.Instance.ShowUI("CharAwakenSkillResetPopup", true);
    }

    private void UpdateAwakenSkillListSlot(int index, GameObject slotObj)
    {
        UICharAwakenSkillListSlot slot = slotObj.GetComponent<UICharAwakenSkillListSlot>();
        if (slot == null || index < 0 || index >= mListParam.Count)
        {
            return;
        }

        slot.ParentGO = gameObject;
        slot.UpdateSlot(index, mListParam[index], index == UpgradeSlotIndex);

        if (index == UpgradeSlotIndex)
        {
            UpgradeSlotIndex = -1;
        }
    }

    private int GetAwakenSkillElementCount()
    {
        return mListParam.Count;
    }
}
