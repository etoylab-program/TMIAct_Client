using UnityEngine;
using System.Collections;

public class UIWeaponTypeSlot : FSlot {

    public UILabel kNameLabel;
	public UITexture kIcoTex;
    public FIndex kIndex;
	public void UpdateSlot(int idx, int charID, string fileName) 	//Fill parameter if you need
	{
        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(60110000 + charID);
        kIcoTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Set/Set_Chara_{0}.png", fileName));
        kIndex.kIndex = idx;
    }
 
	public void OnClick_Slot()
	{
	}
 
}
