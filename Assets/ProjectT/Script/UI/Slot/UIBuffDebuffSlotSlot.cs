using UnityEngine;
using System.Collections;

public class UIBuffDebuffSlotSlot : FSlot {
 

	public UITexture kicoTex;
	public UILabel kDescLabel;
 
	public void UpdateSlot(GameClientTable.HelpBuffDebuff.Param buffData) 	//Fill parameter if you need
	{
        if (buffData == null) return;

        string imgPath = string.Format("UI/UITexture/{0}", buffData.Icon);

        kicoTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("ui", imgPath);
        kDescLabel.textlocalize = FLocalizeString.Instance.GetText(buffData.Desc);
	}
 
	public void OnClick_Slot()
	{
	}
 
}
