using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UIStoryCommunicationTalkUnit : FUnit
{
    public enum CommunicationType
    {
        Player,
        NPC,
        NONE,
    }
    public CommunicationType kType = CommunicationType.NONE;

    //Solo Property
    public UIStoryCommunicationTalkFrameUnit kSoloTalk;

    //Multi Property
    public List<UIStoryCommunicationTalkFrameUnit> kMultiTalks;


    public GameObject kVolumes;
    public List<AudioSource> kVoicePlayers;
    private const int m_DefailtVoicePlayer = 0;

    private float maxHeight = 180f;
    private float[] samples = new float[128]; //샘플//샘플수는 64가 최소 2의 거듭제곱으로 해야함
    private UISprite[] m_VolumeList;
    public void Init()
    {
        kSoloTalk.Init();
        for (int i = 0; i < kMultiTalks.Count; i++)
            kMultiTalks[i].Init();

        kVolumes.SetActive(false);
        m_VolumeList = kVolumes.transform.GetComponentsInChildren<UISprite>();
        for(int i = 0; i < kVoicePlayers.Count; i++)
            kVoicePlayers[i].playOnAwake = false;
        
    }

    public void PlayVoice(string voiceBundleName, string voiceFileName)
    {
        voiceFileName = voiceFileName.Replace(" ", "");
        kVolumes.SetActive(true);
        if (!string.IsNullOrEmpty(voiceFileName))
        {
            kVoicePlayers[m_DefailtVoicePlayer].clip = SoundManager.Instance.GetVoice(voiceBundleName.ToLower(), voiceBundleName + "/" + voiceFileName).clip;
            //kVoicePlayers[m_DefailtVoicePlayer].volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.Voice, FSaveData.Instance.GetVoiceVolume().Equals(0f) ? 0f : 1f);
            kVoicePlayers[m_DefailtVoicePlayer].volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.Voice, FSaveData.Instance.GetVoiceVolume());
            kVoicePlayers[m_DefailtVoicePlayer].Play();
        }
    }

    public void PlayVoice(string voiceBundleName, string[] voiceFileName)
    {
        kVolumes.SetActive(true);

        for(int i = 0; i < voiceFileName.Length; i++)
        {
            if(!string.IsNullOrEmpty(voiceFileName[i]))
            {
                kVoicePlayers[i].clip = SoundManager.Instance.GetVoice(voiceBundleName.ToLower(), voiceBundleName + "/" + voiceFileName[i]).clip;
                //kVoicePlayers[i].volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.Voice, FSaveData.Instance.GetVoiceVolume().Equals(0f) ? 0f : 1f);
                kVoicePlayers[i].volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.Voice, FSaveData.Instance.GetVoiceVolume());
                kVoicePlayers[i].Play();
            }
        }
    }

    public void StopVoice()
    {
        for(int i = 0; i < kVoicePlayers.Count; i++)
        {
            if (kVoicePlayers[i].isPlaying)
                kVoicePlayers[i].Stop();
        }
    }

    public void SoloSetTalk(Texture2D tex, string name, string voiceBundleName, string voiceFileName, bool playVoice = false)
    {
        for (int i = 0; i < kMultiTalks.Count; i++)
        {
            if(kMultiTalks[i].gameObject.activeSelf)
                kMultiTalks[i].gameObject.SetActive(false);
        }

        if (!kSoloTalk.gameObject.activeSelf)
            kSoloTalk.gameObject.SetActive(true);

        if(tex == null)
        {
            kSoloTalk.kVoiceOnlyObj.SetActive(true);
            kSoloTalk.kCharTex.gameObject.SetActive(false);
            kSoloTalk.kVoiceOnlyNameLb.SetText(name, false, false);
        }
        else
        {
            kSoloTalk.kVoiceOnlyObj.SetActive(false);
            kSoloTalk.kCharTex.gameObject.SetActive(true);
            kSoloTalk.kCharTex.mainTexture = tex;
        }

        if (playVoice)
            PlayVoice(voiceBundleName, voiceFileName);
    }

    public void MultiSetTalk(List<Texture2D> texs, string name, string voiceBundleName, string[] voiceFileNames, bool playVoice = false)
    {
        if (!kSoloTalk.gameObject.activeSelf)
            kSoloTalk.gameObject.SetActive(false);

        kSoloTalk.kVoiceOnlyObj.SetActive(false);

        for (int i = 0; i < texs.Count; i++)
        {
            kMultiTalks[i].gameObject.SetActive(true);
            kMultiTalks[i].kCharTex.gameObject.SetActive(true);
            kMultiTalks[i].kCharTex.mainTexture = texs[i];
        }


        if (playVoice)
            PlayVoice(voiceBundleName, voiceFileNames);
    }

    private void Update()
    {
        if (kVoicePlayers == null)
            return;

        if (kVoicePlayers[m_DefailtVoicePlayer].clip == null)
            return;

        AudioSpectrumData();
    }

    public float AudioSpectrumData()
    {
        if (m_VolumeList == null)
            return 0;
        kVoicePlayers[m_DefailtVoicePlayer].GetSpectrumData(samples, 0, FFTWindow.Rectangular);

        switch(kType)
        {
            case CommunicationType.Player:
                //정방향
                for (int i = 0; i < m_VolumeList.Length; i++)
                {
                    m_VolumeList[i].width = 4;
                    m_VolumeList[i].height = (int)(samples[i] * maxHeight) + 4;
                }
                break;
            case CommunicationType.NPC:
                {
                    //역방향
                    for (int i = 0; i < m_VolumeList.Length; i++)
                    {
                        m_VolumeList[i].width = 4;
                        m_VolumeList[i].height = (int)(samples[m_VolumeList.Length - i] * maxHeight) + 4;
                    }
                }
                break;
        }

        return Mathf.Max(samples);
    }
}
