using UnityEngine;
using System.Collections;

public class UIStoryTalkLogSlot : FSlot {
 

	public UILabel kCharNameLabel;
	public UILabel kTalkTextLabel;
    public GameObject kVoiceBtn;
    public GameObject kStoryword;
    private ScenarioParam _param;

    public void UpdateSlot(ScenarioParam param)  //Fill parameter if you need
    {
        _param = param;
        //  대사 치는  캐릭 이름
        kCharNameLabel.textlocalize = param.Value1;
        //  대사 내용
        kTalkTextLabel.textlocalize = param.Value2;

        if (string.IsNullOrEmpty(_param.Voice))
            kVoiceBtn.SetActive(false);
        else
            kVoiceBtn.SetActive(true);

        if (!string.IsNullOrEmpty(param.TremIndex) || !string.IsNullOrEmpty(param.TremLogOnly))
            kStoryword.SetActive(true);
        else
            kStoryword.SetActive(false);
    }
 
	public void OnClick_Slot()
    {
        //  사운드 버튼으로 사용
        VoiceMgr.Instance.PlayStory(_param.BundlePath, _param.Voice);
    }
 
    public void OnClick_StoryWord()
    {
        if(!string.IsNullOrEmpty(_param.TremIndex))
        {
            MessagePopup.TextLong(FLocalizeString.Instance.GetText(1368), GetTrem(_param.TremIndex));
        }
        else if(!string.IsNullOrEmpty(_param.TremLogOnly))
        {
            MessagePopup.TextLong(FLocalizeString.Instance.GetText(1368), GetTrem(_param.TremLogOnly));
        }
    }

    string GetTrem(string treamParam)
    {
        System.Text.StringBuilder str = new System.Text.StringBuilder();
        string trems = treamParam.Replace(" ", "");

		string[] trem = Utility.Split(trems, ','); //trems.Split(',');

        for (int i = 0; i < trem.Length; i++)
        {
            TremParam param = ScenarioMgr.Instance.GetScenarioTremParam(int.Parse(trem[i]));
            str.Append(string.Format("{0}\n\n", param.Title));
            str.Append(param.Desc);
            str.Append("\n\n\n");
        }

        return str.ToString();
    }
}

