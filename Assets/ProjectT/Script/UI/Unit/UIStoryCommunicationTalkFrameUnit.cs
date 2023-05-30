using UnityEngine;
using System.Collections;

public class UIStoryCommunicationTalkFrameUnit : FUnit
{
    public UITexture kCharTex;
    public GameObject kVoiceOnlyObj;
    public FLabelTextShow kVoiceOnlyNameLb;

    public void Init()
    {
        kCharTex.gameObject.SetActive(false);
        kVoiceOnlyObj.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
