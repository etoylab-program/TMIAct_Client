using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIAcquisitionInfoUnit : FUnit
{
    public List<UIButton> kBtnList;
    public List<UILabel> kLabelList;

    private int _acquisitionid;
    private List<GameClientTable.Acquisition.Param> _infolist = new List<GameClientTable.Acquisition.Param>();

    public void UpdateSlot(int acquisitionid ) 	//Fill parameter if you need
	{
        _acquisitionid = acquisitionid;
        for (int i = 0; i < kBtnList.Count; i++)
            kBtnList[i].gameObject.SetActive(false);

        _infolist.Clear();
        _infolist = GameInfo.Instance.GameClientTable.FindAllAcquisition(x => x.GroupID == acquisitionid);
        int count = _infolist.Count;
        if (count > kBtnList.Count)
            count = kBtnList.Count;

        for (int i = 0; i < count; i++)
        {
            kBtnList[i].gameObject.SetActive(true);
            kLabelList[i].textlocalize = FLocalizeString.Instance.GetText(_infolist[i].Desc);
        }
    }
 	
	public void OnClick_AcquisitionBtn( int i )
	{
        var data = _infolist[i];
        if (LobbyUIManager.Instance.GetActiveUI("GachaResultPopup") != null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3128));
            return;
        }
        
        if (LobbyUIManager.Instance.GetActiveUI("StageDetailPopup") != null)
        {
            LobbyUIManager.Instance.HideUI("StoryPanel");
        }
        
        LobbyUIManager.Instance.HideAll(FComponent.TYPE.Popup, false);
        
        GameSupport.MoveUI(data.Type, data.Value1, data.Value2, data.Value3);
	}
}
