using UnityEngine;
using System.Collections;

public class UISpecialModeChangeListSlot : FSlot
{
 

	public UILabel kNameLabel;
	public UISprite kLineSpr;
	public UITexture kImageTex;
	public UISprite kSetSpr;
	public UISprite kDisSpr;
	public UISprite kSelectSpr;

    private int m_index = 0;
    private GameTable.Stage.Param m_stagedata;
    private bool bsetStage = false;
    

    public void UpdateSlot(int index, GameTable.Stage.Param stagedata, bool bselected) 	//Fill parameter if you need
	{
        m_index = index;
        m_stagedata = stagedata;

        FLocalizeString.SetLabel(kNameLabel, m_stagedata.Name);

        string minigameTexName = "MiniGame_" + m_stagedata.Chapter + ".png";
        Texture2D minigameTex = (Texture2D)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Minigame/" + minigameTexName);
        if(minigameTex != null)
        {
            kImageTex.mainTexture = minigameTex;
            kImageTex.MakePixelPerfect();
        }

        if (stagedata.ID == GameInfo.Instance.UserData.NextPlaySpecialModeTableID)
            bsetStage = true;

        kSetSpr.gameObject.SetActive(bsetStage);
        kSelectSpr.gameObject.SetActive(bselected);
        kDisSpr.gameObject.SetActive(bselected);
    }
 
	public void OnClick_Slot()
	{
        if (ParentGO == null)
            return;

        UISpecialModeChangePopup popup = ParentGO.GetComponent<UISpecialModeChangePopup>();
        if(popup != null)
        {
            popup.OnClick_Slot(m_index);
        }
	}
 
}
