using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStoryCommunicationUnit : FUnit
{
    public enum CommunicationUnitType
    {
        TALK,
        LOG,
        NONE,
    }
    public CommunicationUnitType kType = CommunicationUnitType.LOG;

    public GameObject kRoot;
    public UIStoryCommunicationTalkUnit kPlayerTalk;
    public UIStoryCommunicationTalkUnit kNPCTalk;
	public FLabelTextShow kTextLabel;

    private bool bisInit = false;
    public bool IsInit { get { return bisInit; } }
    public void Init()
    {
        if (bisInit)
            return;
        bisInit = true;

        kTextLabel.SetText("", false);
        kPlayerTalk.Init();
        kNPCTalk.Init();
        switch (kType)
        {
            case CommunicationUnitType.TALK:
                {

                }
                break;
            case CommunicationUnitType.LOG:
                {
                    kRoot.SetActive(false);
                }
                break;
        }
    }

    public void Off()
    {
        kTextLabel.SetText("", false);

        kPlayerTalk.StopVoice();
        kNPCTalk.StopVoice();
        kPlayerTalk.Init();
        kNPCTalk.Init();
    }

    public void SetText( string text, bool b)
    {
        if (!kRoot.activeSelf)
            kRoot.SetActive(true);
        kTextLabel.SetText(text, b);
    }

    //VoiceOnly
    public void SetText(string name, string talkText, string voiceBundleName, string voiceFileName, bool mainPlayer, bool playVoice = false)
    {
        TalkSetting(talkText, mainPlayer);
        if (mainPlayer)
        {
            kPlayerTalk.SoloSetTalk(null, name, voiceBundleName, voiceFileName, playVoice);
        }
        else
        {
            kNPCTalk.SoloSetTalk(null, name, voiceBundleName, voiceFileName, playVoice);
        }
    }

    //SoloTalk
    public void SetText(Texture2D charTex, string talkText, string voiceBundleName, string voiceFileName, bool mainPlayer, bool playVoice = false)
    {
        TalkSetting(talkText, mainPlayer);
        if (mainPlayer)
        {
            kPlayerTalk.SoloSetTalk(charTex, "", voiceBundleName, voiceFileName, playVoice);
        }
        else
        {
            kNPCTalk.SoloSetTalk(charTex, "", voiceBundleName, voiceFileName, playVoice);
        }
    }

    //MultiTalk
    public void SetText(List<Texture2D> charTex, string talkText, string voiceBundleName, string[] voiceFileNames, bool mainPlayer, bool playVoice = false)
    {
        TalkSetting(talkText, mainPlayer);
        if (mainPlayer)
        {
            kPlayerTalk.MultiSetTalk(charTex, "", voiceBundleName, voiceFileNames, playVoice);
        }
        else
        {
            kNPCTalk.MultiSetTalk(charTex, "", voiceBundleName, voiceFileNames, playVoice);
        }
    }

    void TalkSetting(string talkText, bool mainPlayer)
    {
        if (!kRoot.activeSelf)
            kRoot.SetActive(true);

        kPlayerTalk.Init();
        kNPCTalk.Init();

        switch (kType)
        {
            case CommunicationUnitType.TALK:
                {
                    kTextLabel.SetText(talkText, true);
                }
                break;
            case CommunicationUnitType.LOG:
                {
                    kTextLabel.SetText(talkText, false);
                    if (mainPlayer)
                        kTextLabel.transform.localPosition = new Vector3(-200f, 0f, 0f);
                    else
                        kTextLabel.transform.localPosition = new Vector3(-274f, 0f, 0f);
                }
                break;
        }
    }

    public void OnClick_Slot()
	{
	}
}
