using UnityEngine;
using System.Collections;

public class UIInfluenceSelectListSlot : FSlot
{
	[SerializeField] private UITexture InfluTexture;
	[SerializeField] private UILabel InfluNameLabel;
	[SerializeField] private UILabel InfluDescLabel;


	GameTable.InfluenceInfo.Param param;

	private string iconRegularPath;

    private void Awake()
    {
		iconRegularPath = "Icon/ForceSelection/{0}.png";
	}

    public void UpdateSlot(GameTable.InfluenceInfo.Param _param) 	//Fill parameter if you need
	{
		SetUI(false);
		param = _param;

		if (param == null) return;

		SetUI(true);
		InfluNameLabel.textlocalize = FLocalizeString.Instance.GetText(param.Name);
		InfluDescLabel.textlocalize = FLocalizeString.Instance.GetText(param.Desc);
		InfluTexture.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format(iconRegularPath, param.Icon));

	}
 
	public void OnClick_Slot()
	{
		if (param == null) return;
		
		string desc = string.Format(FLocalizeString.Instance.GetText(3234), InfluNameLabel.text);
		MessagePopup.OKCANCEL(eTEXTID.TITLE_NOTICE, desc,
			() => { GameInfo.Instance.Send_ReqInfluenceChoice((uint)param.No, OnNetAckInfluenceChoice); }, 
			null);
	}
 
	private void SetUI(bool state)
    {
		InfluTexture.SetActive(state);
		InfluNameLabel.SetActive(state);
		InfluDescLabel.SetActive(state);
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
