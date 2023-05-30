using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISpecialModeChangePopup : FComponent
{

	public UIButton kBackBtn;
	[SerializeField] private FList _SpecialModeChangeListInstance;
	public UIButton kChangeBtn;
    public UILabel kChangeItemCntLabel;

    private List<GameTable.Stage.Param> m_specialModeStageList = new List<GameTable.Stage.Param>();

    private int m_originSelectSlotIndex = 0;
    private int m_selectSlotIndex = 0;
    private bool m_useMatItem;
    private GameTable.Item.Param m_changeItem;


    public override void Awake()
	{
		base.Awake();

		if(this._SpecialModeChangeListInstance == null) return;
		
		this._SpecialModeChangeListInstance.EventUpdate = this._UpdateSpecialModeChangeListSlot;
		this._SpecialModeChangeListInstance.EventGetItemCount = this._GetSpecialModeChangeElementCount;
	}
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        m_useMatItem = true;
        m_specialModeStageList = GameInfo.Instance.GameTable.FindAllStage(x => x.StageType == (int)eSTAGETYPE.STAGE_SPECIAL);

        m_changeItem = GameInfo.Instance.GameTable.FindItem(x => x.Type == (int)eITEMTYPE.MATERIAL && x.SubType == (int)eITEMSUBTYPE.MATERIAL_SPECIAL_CHANGE);
        
        int orgcut = GameInfo.Instance.GetItemIDCount(m_changeItem.ID);
        int orgmax = GameInfo.Instance.GameConfig.SpecialModeChangeItemCnt;
        string strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
        if (orgcut < orgmax)
        {
            strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);
            m_useMatItem = false;
        }
        string strmatcount = string.Format(strHaveCntColor, string.Format(FLocalizeString.Instance.GetText(236), orgcut, orgmax));
        kChangeItemCntLabel.textlocalize= strmatcount;

        for(int i = 0; i < m_specialModeStageList.Count; i++)
        {
            if(m_specialModeStageList[i].ID == GameInfo.Instance.UserData.NextPlaySpecialModeTableID)
            {
                m_originSelectSlotIndex = m_selectSlotIndex = i;
                break;
            }
        }
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        _SpecialModeChangeListInstance.UpdateList();

    }
	
	private void _UpdateSpecialModeChangeListSlot(int index, GameObject slotObject)
	{
		do
		{
            UISpecialModeChangeListSlot slot = slotObject.GetComponent<UISpecialModeChangeListSlot>();

            if (null == slot) break;

            GameTable.Stage.Param data = null;
            if(0 <= index && m_specialModeStageList.Count > index)
            {
                data = m_specialModeStageList[index];
            }

            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(index, data, index == m_selectSlotIndex);

        } while(false);
	}
	
	private int _GetSpecialModeChangeElementCount()
	{
		return m_specialModeStageList.Count;
	}
	
	public void OnClick_BackBtn()
	{
        OnClickClose();
	}

    public void OnClick_Slot(int idx)
    {
        m_selectSlotIndex = idx;
        Renewal(true);
    }
	
	public void OnClick_ChangeBtn()
	{
        if(m_originSelectSlotIndex == m_selectSlotIndex)
        {
            OnClickClose();
            return;
        }

        if(!m_useMatItem)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3042));
            return;
        }

        ItemData itemdata = GameInfo.Instance.GetItemData(m_changeItem.ID);
        if(itemdata != null)
        {
            MessagePopup.CYNItem(
            eTEXTID.OK,
            FLocalizeString.Instance.GetText(3192),
            eTEXTID.OK,
            eTEXTID.CANCEL,
            itemdata.TableID,
            GameInfo.Instance.GameConfig.SpecialModeChangeItemCnt, 
            () => 
            {
                GameInfo.Instance.Send_ReqUseItem(itemdata.ItemUID, GameInfo.Instance.GameConfig.SpecialModeChangeItemCnt, m_specialModeStageList[m_selectSlotIndex].ID, OnAckUseItemStageSpecial);
            }, 
            null
            );
        }
	}

    public void OnAckUseItemStageSpecial(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        //밖에 팝업 갱신
        UIValue.Instance.SetValue(UIValue.EParamType.StageID, GameInfo.Instance.UserData.NextPlaySpecialModeTableID);

        LobbyUIManager.Instance.InitComponent("SpecialModePopup");
        LobbyUIManager.Instance.Renewal("SpecialModePopup");

        OnClickClose();
    }
    
}
