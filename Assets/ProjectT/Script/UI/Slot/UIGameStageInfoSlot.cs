using UnityEngine;
using System.Collections;

public class UIGameStageInfoSlot : FSlot
{
	public UISprite kbgSpr;
	public UILabel kDescLabel;
	public UILabel kNameLabel;
	public UILabel kTypeLabel;
    public UITexture kMonIconTex;

	public void UpdateSlot(GameClientTable.HelpEnemyInfo.Param data) 	//Fill parameter if you need
	{
        if (data == null)
            return;

        Log.Show(data.Name);

        kbgSpr.color = GameInfo.Instance.GameConfig.MonsterGradeColor[data.Grade];
        kNameLabel.color = GameInfo.Instance.GameConfig.MonsterGradeColor[data.Grade];

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(data.Name);
        kDescLabel.textlocalize = FLocalizeString.Instance.GetText(data.Desc);

        kTypeLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.MON_TYPE_TEXT_START + data.MonType);

        string imgPath = string.Format("Icon/{0}", data.Icon);

        kMonIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", imgPath);
	}
 
	public void OnClick_Slot()
	{
	}
 
}
