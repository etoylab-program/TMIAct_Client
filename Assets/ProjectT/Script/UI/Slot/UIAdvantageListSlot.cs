using UnityEngine;
using System.Collections;

public class UIAdvantageListSlot : FSlot
{
    public UISprite kBGSpr;
    public UITexture kCharIcoTex;
    public UISprite kFrameSpr;

    private int _index;

	public void UpdateSlot(int index, GameTable.Character.Param charTableData) 	//Fill parameter if you need
	{
        if (charTableData == null)
            return;

        _index = index;
        
        kCharIcoTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/MainSlot/MainSlot_" + charTableData.Icon + "_" + charTableData.InitCostume + ".png");
    }
 
	public void OnClick_Slot()
	{
        return;
	}
 
}
