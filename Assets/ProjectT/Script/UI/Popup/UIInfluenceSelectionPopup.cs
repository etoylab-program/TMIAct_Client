using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UIInfluenceSelectionPopup : FComponent
{   
    [SerializeField] private FList kInfluenceSelectList;
    [SerializeField] private ParticleSystem kParticle;
    [SerializeField] private UITexture kBGTexture;
    [SerializeField] private UILabel kDesciptionLabel;

    private int SelectGroupID = 1;
    private GameTable.InfluenceMissionSet.Param InfMissionSetTable;
    private List<GameTable.InfluenceInfo.Param> InfInfoTables;
    
    private bool IsInfluence => InfInfoTables != null && InfInfoTables.Count > 1;
    

    public override void OnEnable()
    {
        Lobby.Instance.SetLobbyBG(false);
        base.OnEnable();
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();
        Lobby.Instance.SetLobbyBG(true);
    }

    public override void OnClose()
    {        
        base.OnClose();
    }

    public override void InitComponent()
    {
        InfMissionSetTable = GameSupport.GetCurrentInfluenceMissionSet();
        if (InfMissionSetTable == null)
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(100012), base.OnClickClose);           
            return;
        }
        SelectGroupID = InfMissionSetTable.ID;
        InfInfoTables = GameInfo.Instance.GameTable.FindAllInfluenceInfo(x => x.GroupID == SelectGroupID);

        string iconRegularPath = "Icon/ForceSelection/ServerInfluence_11.png";
        if (InfInfoTables != null && InfInfoTables.Count > 0)
            iconRegularPath = $"Icon/ForceSelection/{InfInfoTables[0].Icon}.png";

        kDesciptionLabel.textlocalize = FLocalizeString.Instance.GetText(IsInfluence ? 1641 : 1723);
        kBGTexture.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", iconRegularPath);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
        InitComponent();

        kInfluenceSelectList.UpdateList();
    }

    private void UpdateListSlot(int index, GameObject slotObj)
    {
        UIInfluenceSelectListSlot slot = slotObj.GetComponent<UIInfluenceSelectListSlot>();
        if (slot == null) return;

        slot.UpdateSlot(InfInfoTables[index]);
    }
    
    public void OnClieck_LeftInfluence()
    {
        Choice_Influence(0);
    }

    public void OnClieck_RightInfluence()
    {
        Choice_Influence(1);
    }
    
    private void Choice_Influence(int index)
    {
        if (InfInfoTables.Count <= 0)
            return;
        
        if (InfInfoTables.Count <= index)
            index = 0;
        
        PlayParticle();
        
        Send_ReqInfluenceChoice(InfInfoTables[index]);
    }
    
    private void PlayParticle()
    {
        if (kParticle == null) return;

        kParticle.Stop();
        kParticle.Play();
    }

    private void Send_ReqInfluenceChoice(GameTable.InfluenceInfo.Param param)
    {
        if (GameSupport.GetCurrentInfluenceMissionSet() == null)
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(100012), base.OnClickClose);
            return;
        }
        string desc = string.Format(FLocalizeString.Instance.GetText(IsInfluence ? 3234 : 3246), FLocalizeString.Instance.GetText(param.Name));
        MessagePopup.OKCANCEL(eTEXTID.TITLE_NOTICE, desc,
            () => { GameInfo.Instance.Send_ReqInfluenceChoice((uint)param.No, OnNetAckInfluenceChoice); },
            null);
    }

    private void OnNetAckInfluenceChoice(int result, PktMsgType pktmsg)
    {
        GameInfo.Instance.Send_ReqGetInfluenceInfo((int result1, PktMsgType pktmsg1) =>
        {
            LobbyUIManager.Instance.HideUI("InfluenceSelectionPopup", true);
            LobbyUIManager.Instance.ShowUI("InfluenceMainPopup", true);
        });
    }

}
