using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


//---------------------------------------------------------------------------------------------------
//사운드 오브젝트 메니져
//생성된 오브젝트 를 관리한다. 휘발성인경우 관리하지 않는다 추후 필요시 오브젝트 풀로 제작한다.
//---------------------------------------------------------------------------------------------------

public class SoundManager : FMonoSingleton<SoundManager>
{
    public enum eSoundType
    {
        Primary,
        //Secondary,
        Ambience,
        FX,
        Voice,
        Story,
        SupporterVoice,
        Monster,
        Player,
        PassiveVoice,
        UI,
        DirectorBGM,
        DirectorFX,
    }
    public enum eType
    {
        BGM = 1,
        FxSnd,
        UI,
        Off,
    }

    public enum eVoiceType
    {
        None = 0,

        Character,
        Supporter,
        Story,
        Monster,
        Passive,
    }


    public class sSoundInfo
    {
        public int tableId;
        public string name;
        public eType type;
        public float volume;
        public AudioClip clip;
        //public bool isPlaying;

        public sSoundInfo(string name, SoundTable.Sound.Param sound, float volume)
        {
            if (sound == null)
            {
                Debug.LogError(tableId + "번 사운드가 존재하지 않습니다.");
                return;
            }
            this.name = string.IsNullOrEmpty(name) == false ? name : sound.ID.ToString();
            this.tableId = sound.ID;
            this.type = (eType)sound.Type;
            this.volume = sound.Volume * volume;
            this.clip = ResourceMgr.Instance.LoadFromAssetBundle("sound", "Sound/" + sound.Path) as AudioClip;
        }

        public sSoundInfo(string name, string path, float volume)
        {
            this.name = name;
            this.type = eType.FxSnd;
            this.volume = volume;
            //this.isPlaying = false;

            this.clip = ResourceMgr.Instance.LoadFromAssetBundle("sound", path) as AudioClip;
            if(clip == null)
            {
                clip = ResourceMgr.Instance.LoadFromAssetBundle("voicechar01", path) as AudioClip; // 임시
                if (clip == null)
                {
                    Debug.LogWarning(path + "를 불러올 수 없습니다.");
                }
            }
        }

        public sSoundInfo(string name, string bundle, string path, float volume)
        {
            this.name = name;
            this.type = eType.FxSnd;
            this.volume = volume;
            //this.isPlaying = false;

            this.clip = ResourceMgr.Instance.LoadFromAssetBundle(bundle, path) as AudioClip;
        }
    }

    private static float DEFAULT_BGM_VOLUME = 0.4f;
    private static float DEFAULT_FXSND_VOLUME = 1.0f;
    private static float DEFAULT_UISND_VOLUME = 1.0f;
    private static int MAX_LOAD_FX_SND_COUNT = 1;

    public AudioSource FxSndSrc { get; private set; }
    public AudioSource DirectorBGMSrc { get; private set; }
    public AudioSource DirectorFXSrc { get; private set; }

    private AudioSource m_primarySrc;
    //private AudioSource m_secondarySrc;
    private AudioSource m_ambienceSrc;
    private AudioSource m_voiceSrc;
    private AudioSource m_storySrc;
    private AudioSource m_supporterVoiceSrc;
    private AudioSource m_monsterSrc;
    private AudioSource m_playerSrc;
    private AudioSource m_passiveVoiceSrc;
    private AudioSource m_uiSndSrc;

    private SoundTable m_soundTable = null;
    private Dictionary<string, List<sSoundInfo>> m_dicAudioClip = new Dictionary<string, List<sSoundInfo>>();

    //AudioSource가 늘어나지 않도록 제한을 걸어둠 - 팀장님 지시
    private Dictionary<string, AudioSource> m_dicfxLoopSndSrc = new Dictionary<string, AudioSource>();
    private const int m_fxLoopSndSrcMaxCnt = 3;

    private int _bgmid = -10;
    public int BGMID { get { return _bgmid; } }
    public SoundTable SoundTable { get { return m_soundTable; } }

    public UnityEngine.Audio.AudioMixerGroup playerMixer;
    public UnityEngine.Audio.AudioMixerGroup enemyMixer;
    public UnityEngine.Audio.AudioMixerGroup bgmMixer;
    public UnityEngine.Audio.AudioMixerGroup uiMixer;
    public UnityEngine.Audio.AudioMixerGroup voiceMixer;
    public UnityEngine.Audio.AudioMixerGroup fxMixer;
    public UnityEngine.Audio.AudioMixerGroup directorBGMMixer;
    public UnityEngine.Audio.AudioMixerGroup directorFXmixer;

    private float m_masterFXVol = 1f;

    private readonly List<Action<float>> _seVolumeChangeActionList = new List<Action<float>>();

    public void AddSeVolumeChangeAction(Action<float> action)
    {
        if (_seVolumeChangeActionList.Any(x => x == action))
        {
            return;
        }
        
        _seVolumeChangeActionList.Add(action);
    }

    public void ClearBgmVolumeChangeAction()
    {
        _seVolumeChangeActionList.Clear();
    }
    
    private void Awake()
    {
        DontDestroyOnLoad();
        Init();
    }

    public void LoadTable()
    {
        m_soundTable = (SoundTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/Sound.asset");
    }

    public void Init()
    {
        LoadTable();

        //  원래 진행 사운드
        if (m_primarySrc == null)
        {
            m_primarySrc = gameObject.AddComponent<AudioSource>();
            m_primarySrc.playOnAwake = false;
            m_primarySrc.volume = DEFAULT_BGM_VOLUME;
            m_primarySrc.loop = true;
        }

        /*/  교체 사운드 
        if (m_secondarySrc == null)
        {
            m_secondarySrc = gameObject.AddComponent<AudioSource>();
            m_secondarySrc.playOnAwake = false;
            m_secondarySrc.volume = DEFAULT_BGM_VOLUME;
            m_secondarySrc.loop = true;
            m_secondarySrc.enabled = false;
        }*/

        // 
        if (m_ambienceSrc == null)
        {
            m_ambienceSrc = gameObject.AddComponent<AudioSource>();
            m_ambienceSrc.playOnAwake = false;
            m_ambienceSrc.volume = DEFAULT_BGM_VOLUME;
            m_ambienceSrc.loop = true;
        }

        //  이펙트
        if (FxSndSrc == null)
        {
            FxSndSrc = gameObject.AddComponent<AudioSource>();
            FxSndSrc.playOnAwake = false;
            FxSndSrc.volume = DEFAULT_FXSND_VOLUME;
            FxSndSrc.loop = false;
        }

        //  보이스
        if (m_voiceSrc == null)
        {
            m_voiceSrc = gameObject.AddComponent<AudioSource>();
            m_voiceSrc.playOnAwake = false;
            m_voiceSrc.volume = DEFAULT_FXSND_VOLUME;
            m_voiceSrc.loop = false;
        }

        // 서포터 보이스
        if (m_supporterVoiceSrc == null)
        {
            m_supporterVoiceSrc = gameObject.AddComponent<AudioSource>();
            m_supporterVoiceSrc.playOnAwake = false;
            m_supporterVoiceSrc.volume = DEFAULT_FXSND_VOLUME;
            m_supporterVoiceSrc.loop = false;
        }

        // 스토리 보이스
        if (m_storySrc == null)
        {
            m_storySrc = gameObject.AddComponent<AudioSource>();
            m_storySrc.playOnAwake = false;
            m_storySrc.volume = DEFAULT_FXSND_VOLUME;
            m_storySrc.loop = false;
        }

        //Enemy, Monster
        if (m_monsterSrc == null)
        {
            m_monsterSrc = gameObject.AddComponent<AudioSource>();
            m_monsterSrc.playOnAwake = false;
            m_monsterSrc.volume = DEFAULT_FXSND_VOLUME;
            m_monsterSrc.loop = false;
        }

        //Player
        if (m_playerSrc == null)
        {
            m_playerSrc = gameObject.AddComponent<AudioSource>();
            m_playerSrc.playOnAwake = false;
            m_playerSrc.volume = DEFAULT_BGM_VOLUME;
            m_playerSrc.loop = false;
        }

        if (m_passiveVoiceSrc == null)
        {
            m_passiveVoiceSrc = gameObject.AddComponent<AudioSource>();
            m_passiveVoiceSrc.playOnAwake = false;
            m_passiveVoiceSrc.volume = DEFAULT_FXSND_VOLUME;
            m_passiveVoiceSrc.loop = false;
        }

        //  UI
        if (m_uiSndSrc == null)
        {
            m_uiSndSrc = gameObject.AddComponent<AudioSource>();
            m_uiSndSrc.playOnAwake = false;
            m_uiSndSrc.volume = DEFAULT_UISND_VOLUME;
            m_uiSndSrc.loop = false;
        }

        //Director
        if(DirectorBGMSrc == null)
        {
            DirectorBGMSrc = gameObject.AddComponent<AudioSource>();
            DirectorBGMSrc.playOnAwake = false;
            DirectorBGMSrc.volume = DEFAULT_FXSND_VOLUME;
            DirectorBGMSrc.loop = true;
        }
        if(DirectorFXSrc == null)
        {
            DirectorFXSrc = gameObject.AddComponent<AudioSource>();
            DirectorFXSrc.playOnAwake = false;
            DirectorFXSrc.volume = DEFAULT_FXSND_VOLUME;
            DirectorFXSrc.loop = false;
        }
        
        SetMixer();

        SetDefaultVolumes();
    }

    public void Release()
    {
        foreach (KeyValuePair<string, List<sSoundInfo>> kv in m_dicAudioClip)
        {
            if (kv.Value == null || kv.Value.Count <= 0)
            {
                continue;
            }

            for (int i = 0; i < kv.Value.Count; i++)
            {
                if (kv.Value[i].clip)
                {
                    kv.Value[i].clip.UnloadAudioData();
                    kv.Value[i].clip = null;
                }

                kv.Value[i] = null;
            }
        }

        m_dicAudioClip.Clear();
    }

    //앱 실행시 한번 적용
    public void SetDefaultVolumes()
    {
        SetBgmVolme(FSaveData.Instance.GetBGVolume());
        SetFxVolume(FSaveData.Instance.GetSEVolume());
        SetVoiceVolume(FSaveData.Instance.GetVoiceVolume());
    }

    void GetMixer()
    {
        if (playerMixer == null || enemyMixer == null || bgmMixer == null || uiMixer == null || voiceMixer == null || fxMixer == null)
        {
            UnityEngine.Audio.AudioMixer mixer = (UnityEngine.Audio.AudioMixer)ResourceMgr.Instance.LoadFromAssetBundle("sound", "Sound/Mixer/Master.mixer");
			if(mixer == null)
			{
				return;
			}

            playerMixer = mixer.FindMatchingGroups("Player")[0];
            enemyMixer = mixer.FindMatchingGroups("Enemy")[0];
            bgmMixer = mixer.FindMatchingGroups("BGM")[0];
            uiMixer = mixer.FindMatchingGroups("UI")[0];
            voiceMixer = mixer.FindMatchingGroups("Voice")[0];
            fxMixer = mixer.FindMatchingGroups("FX")[0];
            directorBGMMixer = mixer.FindMatchingGroups("DirectorBGM")[0];
            directorFXmixer = mixer.FindMatchingGroups("DirectorFX")[0];
        }
    }

    public UnityEngine.Audio.AudioMixerGroup GetAudioMixer(eSoundType soundType)
    {
        return GetAudioSource(soundType).outputAudioMixerGroup;
    }

    void SetMixer()
    {
        GetMixer();

        if (m_playerSrc != null && playerMixer != null)
            m_playerSrc.outputAudioMixerGroup = playerMixer;
        if (m_monsterSrc != null && enemyMixer != null)
            m_monsterSrc.outputAudioMixerGroup = enemyMixer;
        if (FxSndSrc != null && fxMixer != null)
            FxSndSrc.outputAudioMixerGroup = fxMixer;

        if (m_uiSndSrc != null && uiMixer != null)
            m_uiSndSrc.outputAudioMixerGroup = uiMixer;
        if (m_voiceSrc != null && voiceMixer != null)
            m_voiceSrc.outputAudioMixerGroup = voiceMixer;
        if (m_primarySrc != null && bgmMixer != null)
            m_primarySrc.outputAudioMixerGroup = bgmMixer;

        if (m_storySrc != null && voiceMixer != null)
            m_storySrc.outputAudioMixerGroup = voiceMixer;

        if (DirectorBGMSrc != null && directorBGMMixer != null)
            DirectorBGMSrc.outputAudioMixerGroup = directorBGMMixer;
        if (DirectorFXSrc != null && directorFXmixer != null)
            DirectorFXSrc.outputAudioMixerGroup = directorFXmixer;
    }

    //#region ADD
    public void AddAudioClip(string name, string path, float volume)
    {
        if (m_dicAudioClip.ContainsKey(name) && m_dicAudioClip[name].Count >= MAX_LOAD_FX_SND_COUNT)
        {
            return;
        }

        sSoundInfo soundInfo = new sSoundInfo(name, path, volume);
        if (soundInfo.clip == null)
        {
            return;
        }

        AddAudioClip(soundInfo);
    }

    public void AddAudioClip(string name, int tableId, float volume)
    {
        SoundTable.Sound.Param param = m_soundTable.FindSound(tableId);
        if (param == null)
        {
            Debug.LogError(tableId + "번 사운드가 존재하지 않습니다.");
            return;
        }

        if (m_dicAudioClip.ContainsKey(name) && m_dicAudioClip[name].Count >= MAX_LOAD_FX_SND_COUNT)
        {
            return;
        }

        sSoundInfo soundInfo = new sSoundInfo(name, param, volume);
        AddAudioClip(soundInfo);
    }

    public void AddAudioClip(string name, SoundTable.Sound.Param param, float volume)
    {
        if (param == null)
        {
            Debug.LogError("해당 사운드가 존재하지 않습니다.");
            return;
        }

        if (m_dicAudioClip.ContainsKey(name) && m_dicAudioClip[name].Count >= MAX_LOAD_FX_SND_COUNT)
        {
            return;
        }

        sSoundInfo soundInfo = new sSoundInfo(name, param, volume);
        AddAudioClip(soundInfo);
    }

    private void AddAudioClip(sSoundInfo soundInfo)
    {
        if (soundInfo.clip == null)
        {
            Debug.LogWarning(soundInfo.name + "를 불러올 수 없습니다.");
            return;
        }

        if (!m_dicAudioClip.ContainsKey(soundInfo.name))
        {
            List<sSoundInfo> list = new List<sSoundInfo>();
            list.Add(soundInfo);

            m_dicAudioClip.Add(soundInfo.name, list);
        }
        else
            m_dicAudioClip[soundInfo.name].Add(soundInfo);
    }

    //#endregion
    //#region Base
    /// <summary>
    /// 이름을 가지고 m_dicAudioClip에 있는 사운드를 반환합니다.
    /// </summary>
    public AudioClip GetSnd(string name, int idx = 0)
    {
        if (m_dicAudioClip.ContainsKey(name) == false)
        {
            Debug.LogWarning(name + "오디오가 없습니다.");
            return null;
        }

        return m_dicAudioClip[name][idx].clip;
    }

    public AudioSource GetAudioSource(eSoundType soundType)
    {
        AudioSource ao = null;
        switch (soundType)
        {
            case eSoundType.Ambience:
                ao = m_ambienceSrc;
                break;
            case eSoundType.FX:
                ao = FxSndSrc;
                break;
            case eSoundType.Monster:
                ao = m_monsterSrc;
                break;
            case eSoundType.PassiveVoice:
                ao = m_passiveVoiceSrc;
                break;
            case eSoundType.Player:
                ao = m_playerSrc;
                break;
            case eSoundType.Primary:
                ao = m_primarySrc;
                break;
            //case eSoundType.Secondary:
            //    ao = m_secondarySrc;
            //    break;
            case eSoundType.Story:
                ao = m_storySrc;
                break;
            case eSoundType.SupporterVoice:
                ao = m_supporterVoiceSrc;
                break;
            case eSoundType.UI:
                ao = m_uiSndSrc;
                break;
            case eSoundType.Voice:
                ao = m_voiceSrc;
                break;
            case eSoundType.DirectorBGM:
                ao = DirectorBGMSrc;
                break;
            case eSoundType.DirectorFX:
                ao = DirectorFXSrc;
                break;
        }

        return ao;
    }

    public float GetVolume(eSoundType soundType, float sourceVol)
    {
        float resultVol = sourceVol;
        switch(soundType)
        {
            case eSoundType.Ambience:
                resultVol *= FSaveData.Instance.GetBGVolume();
                break;
            case eSoundType.FX:
                resultVol *= m_masterFXVol;
                break;
            case eSoundType.Monster:
                resultVol *= FSaveData.Instance.GetSEVolume();
                break;
            case eSoundType.PassiveVoice:
                
                break;
            case eSoundType.Player:
                resultVol *= FSaveData.Instance.GetSEVolume();
                break;
            case eSoundType.Primary:
                resultVol *= FSaveData.Instance.GetBGVolume();
                break;
            /*case eSoundType.Secondary:
                
                break;*/
            case eSoundType.Story:
                
                break;
            case eSoundType.SupporterVoice:
                
                break;
            case eSoundType.UI:
                
                break;
            case eSoundType.Voice:
                resultVol *= FSaveData.Instance.GetVoiceVolume();
                break;
            case eSoundType.DirectorBGM:
                resultVol *= FSaveData.Instance.GetBGVolume();
                break;
            case eSoundType.DirectorFX:
                resultVol *= FSaveData.Instance.GetSEVolume();
                break;
        }
        return resultVol;
    }
    
    public sSoundInfo PlaySnd(eSoundType soundType, int tableID, float volume)
    {
        AudioSource src = GetAudioSource(soundType);
        if (src == null)
            return null;

        SoundTable.Sound.Param param = m_soundTable.FindSound(tableID);
        if (param == null)
            return null;

        sSoundInfo info = new sSoundInfo(string.Empty, param, volume);
        src.PlayOneShot(info.clip, GetVolume(soundType,info.volume));
        return info;
    }
    public void PlaySnd(eSoundType soundType, AudioClip clip, float volume)
    {
        AudioSource src = GetAudioSource(soundType);

        if (clip == null || src == null)
            return;

        src.PlayOneShot(clip, GetVolume(soundType, volume));
    }

	public void StopSnd( eSoundType soundType ) {
		AudioSource src = GetAudioSource( soundType );
		if ( src == null ) {
			return;
		}

		src.Stop();
	}

    public sSoundInfo PlayDelayedSnd(eSoundType soundType, float delay, int tableID, float volume)
    {
        AudioSource src = GetAudioSource(soundType);

        if (src == null)
            return null;

        SoundTable.Sound.Param param = m_soundTable.FindSound(tableID);
        if (param == null)
            return null;

        sSoundInfo info = new sSoundInfo(string.Empty, param, volume);
        StartCoroutine(DelayedSnd(src, delay, info.clip, GetVolume(soundType, info.volume)));
        return info;
    }

    public void PlayDelayedSnd(eSoundType soundType, float delay, AudioClip clip, float volume)
    {
        AudioSource src = GetAudioSource(soundType);

        if (src == null)
            return;

        StartCoroutine(DelayedSnd(src, delay, clip, GetVolume(soundType, volume)));
    }

    IEnumerator DelayedSnd(AudioSource src, float delay, AudioClip clip, float volume)
    {
        yield return new WaitForSeconds(delay);
        src.PlayOneShot(clip, volume);
    }

    public void PlaySnd(eSoundType soundType, string name)
    {
        AudioSource src = GetAudioSource(soundType);
        if(src == null)
        {
            Debug.LogWarning(soundType + "오디오 소스가 없습니다.");
            return;
        }
        if(m_dicAudioClip.ContainsKey(name) == false)
        {
            Debug.LogWarning(name + " 오디오가 없습니다.");
            return;
        }

        sSoundInfo info = null;
        for(int i = 0; i < m_dicAudioClip[name].Count; i++)
        {
            //if(!m_dicAudioClip[name][i].isPlaying)
            {
                info = m_dicAudioClip[name][i];
                info.volume = FSaveData.Instance.GetSEVolume();
                break;
            }
        }

        if (info == null)
            return;

        //info.isPlaying = true;

        Debug.Log(info.clip.name);
        src.PlayOneShot(info.clip, GetVolume(soundType, info.volume));
        //StartCoroutine(EndSnd(info));
    }
    public void PlaySnd(eSoundType soundType, string name, float volume)
    {
        AudioSource src = GetAudioSource(soundType);
        if(src == null)
        {
            Debug.LogError("오디오 소스가 없습니다.");
            return;
        }
        if(m_dicAudioClip.ContainsKey(name) == false)
        {
            Debug.LogError(name + " 오디오가 없습니다.");
            return;
        }

        sSoundInfo info = null;
        for(int i = 0; i < m_dicAudioClip[name].Count; i++)
        {
            //if(!m_dicAudioClip[name][i].isPlaying)
            {
                info = m_dicAudioClip[name][i];
                info.volume = FSaveData.Instance.GetSEVolume();
                break;
            }
        }

        if (info == null)
            return;

        //info.isPlaying = true;
        src.PlayOneShot(info.clip, GetVolume(soundType, volume));
        //StartCoroutine(EndSnd(info));
    }
    /*IEnumerator EndSnd(sSoundInfo info)
    {
        yield return new WaitForSeconds(info.clip.length);
        info.isPlaying = false;
    }*/
    //#endregion
    //#region UI

    public void PlayUISnd(int tableID, GameObject go = null)
    {
        SoundTable.Sound.Param param = m_soundTable.FindSound(tableID);
        if (param == null)
        {
            if(go != null)            
                Debug.LogWarning(string.Format("{0}번 사운드가 존재하지 않습니다. ({1})", tableID, go.GetFullName()));
            
            else            
                Debug.LogWarning(tableID + "번 사운드가 존재하지 않습니다.");            
            return;
        }

        sSoundInfo soundInfo = new sSoundInfo(string.Empty, param, FSaveData.Instance.GetSEVolume());
        m_uiSndSrc.PlayOneShot(soundInfo.clip, GetVolume(eSoundType.UI, soundInfo.volume));
    }
    
    public void StopUISnd()
    {
        m_uiSndSrc.Stop();
    }

    //#endregion
    

    //#region FX
    public void PlayFxSnd(AudioClip clip, float volume)
    {
        if(clip == null)
        {
            return;
        }

        AudioSource src = GetAudioSource(eSoundType.FX);
        if(src == null)
        {
            return;
        }

        src.PlayOneShot(clip, GetVolume(eSoundType.FX, volume));
    }

    void AddFxLoopSnd(string name, AudioClip clip, float volume)
    {
        if(m_dicfxLoopSndSrc.Count >= m_fxLoopSndSrcMaxCnt)
        {
            Debug.LogError("Loop Audiosource의 갯수가 " + m_fxLoopSndSrcMaxCnt + "개를 넘을수 없습니다.");
            return;
        }

        if (m_dicfxLoopSndSrc.ContainsKey(name) == false)
        {
            AudioSource ao = gameObject.AddComponent<AudioSource>();
            ao.playOnAwake = false;
            ao.loop = true;
            ao.clip = clip;
            ao.outputAudioMixerGroup = playerMixer;
            m_dicfxLoopSndSrc.Add(name, ao);
        }

        AddPlayFxLoopSnd(name, GetVolume(eSoundType.FX, volume));
    }

    void AddPlayFxLoopSnd(string name, float volume)
    {
        if (m_dicAudioClip.ContainsKey(name) == false)
        {
            Debug.LogError(name + "오디오가 없습니다.###");
            return;
        }

        if (m_dicfxLoopSndSrc.ContainsKey(name) == false)
        {
            AddFxLoopSnd(name, GetSnd(name), volume);
        }

        m_dicfxLoopSndSrc[name].volume = GetVolume(eSoundType.FX, volume);
        if (!m_dicfxLoopSndSrc[name].isPlaying)
            m_dicfxLoopSndSrc[name].Play();
    }

    public void PlayFxLoopSnd(AudioClip clip, float volume)
    {
        AddFxLoopSnd(clip.name, clip, volume);
    }

    public void PlayFxLoopSnd(string name, AudioClip clip, float volume)
    {
        AddFxLoopSnd(name, clip, volume);
    }

    public void PlayFxLoopSnd(string name, float volume, bool pause = false)
    {
        if (pause)
        {
            AudioSource ao = GetFxLoopSndSrc(name);
            if (ao != null)
                ao.Pause();
        }
        else
        {
            AddPlayFxLoopSnd(name, volume);
        }

    }

    public AudioSource GetFxLoopSndSrc(string name)
    {
        if (m_dicfxLoopSndSrc.ContainsKey(name) == false)
        {
            Debug.LogError(name + "오디오소스가 없습니다.");
            return null;
        }

        return m_dicfxLoopSndSrc[name];
    }

    //#endregion

    //#region VOICE

    public void PlayVoice(AudioClip clip, float volume)
    {
        if (clip == null || m_voiceSrc == null)
            return;

        m_voiceSrc.PlayOneShot(clip, GetVolume(eSoundType.Voice, volume));
    }

    public void PlayVoice(int tableID, float volume)
    {
        SoundTable.Sound.Param param = m_soundTable.FindSound(tableID);
        if (param == null)
            return;

        sSoundInfo soundInfo = new sSoundInfo(string.Empty, param, volume);// FSaveData.Instance.GetVoiceVolume());
        m_voiceSrc.PlayOneShot(soundInfo.clip, GetVolume(eSoundType.Voice, soundInfo.volume));
    }
    
    public sSoundInfo PlayVoice(eVoiceType voiceType, string bundle, string path, bool playOneShot = false)
    {
        sSoundInfo soundInfo = new sSoundInfo(string.Empty, bundle, path, FSaveData.Instance.GetVoiceVolume());

        AudioSource src = null;
        switch(voiceType)
        {
            case eVoiceType.Character:
                src = m_voiceSrc;
                break;

            case eVoiceType.Supporter:
                src = m_supporterVoiceSrc;
                break;

            case eVoiceType.Story:
                src = m_storySrc;
                break;

            case eVoiceType.Monster:
                src = m_storySrc;
                break;

            case eVoiceType.Passive:
                src = m_passiveVoiceSrc;
                break;
        }

        src.clip = soundInfo.clip;
        src.volume = GetVolume(eSoundType.Voice, soundInfo.volume);

        if (!playOneShot)
        {
            if (src.isPlaying)
            {
                return soundInfo;
            }

            src.Play();
        }
        else
        {
            src.PlayOneShot(src.clip, src.volume);
        }

        return soundInfo;
    }

    public sSoundInfo PlayVoice(eVoiceType voiceType, string bundle, string path, float volume)
    {
        sSoundInfo soundInfo = new sSoundInfo(string.Empty, bundle, path, FSaveData.Instance.GetVoiceVolume());

        AudioSource src = null;
        switch (voiceType)
        {
            case eVoiceType.Character:
                src = m_voiceSrc;
                break;

            case eVoiceType.Supporter:
                src = m_supporterVoiceSrc;
                break;

            case eVoiceType.Story:
                src = m_storySrc;
                break;

            case eVoiceType.Monster:
                src = m_storySrc;
                break;

            case eVoiceType.Passive:
                src = m_passiveVoiceSrc;
                break;
        }

        if (src.isPlaying)
            return soundInfo;

        src.clip = soundInfo.clip;
        src.volume = GetVolume(eSoundType.Voice, volume);
        src.Play();

        return soundInfo;
    }

    public sSoundInfo GetVoice(string bundle, string path)
    {
        sSoundInfo soundInfo = new sSoundInfo(string.Empty, bundle, path, FSaveData.Instance.GetVoiceVolume());

        return soundInfo;
    }

    public void StopVoice()
    {
        if (m_voiceSrc != null)
            m_voiceSrc.Stop();

        if (m_supporterVoiceSrc)
            m_supporterVoiceSrc.Stop();

        if (m_storySrc)
            m_storySrc.Stop();
    }

    //#endregion

    //#region BGM

    public AudioClip GetBgm()
    {
        if (m_primarySrc.clip == null)
            return null;

        return m_primarySrc.clip;
    }

    public void PlayBgm(AudioClip clip, float volume, bool isLoop = true)
    {
        if (clip == null || m_primarySrc == null)
            return;

        m_primarySrc.clip = clip;
        m_primarySrc.volume = GetVolume(eSoundType.Primary, volume);
        m_primarySrc.loop = isLoop;

        m_primarySrc.Play();
    }


    public void PlayBgm(int tableID, float volume, bool isLoop = true)
    {
        SoundTable.Sound.Param param = m_soundTable.FindSound(tableID);
        if (param == null || m_primarySrc == null)
            return;

        sSoundInfo soundInfo = new sSoundInfo("", param, FSaveData.Instance.GetBGVolume());

        m_primarySrc.clip = soundInfo.clip;
        m_primarySrc.volume = GetVolume(eSoundType.Primary, volume);
        m_primarySrc.loop = isLoop;

        m_primarySrc.Play();

        _bgmid = tableID;
    }


    public void PlayBgm(int tableID, string name, bool isLoop = true)
    {
        SoundTable.Sound.Param param = m_soundTable.FindSound(tableID);
        if (param == null)
        {
            Debug.LogError(tableID + "의 ID를 가진 사운드가 존재하지 않습니다.");
            return;
        }

        if (m_dicAudioClip.ContainsKey(name) == false)
        {
            AddAudioClip(name, param, param.Volume);
        }

        if (m_dicAudioClip[name] == null || m_dicAudioClip[name].Count <= 0)
        {
            Debug.LogError(tableID + "에 TValue가 비어있습니다.");
            return;
        }

        m_primarySrc.enabled = true;
        m_primarySrc.clip = m_dicAudioClip[name][0].clip;
        m_primarySrc.volume = GetVolume(eSoundType.Primary, m_dicAudioClip[name][0].volume);
        m_primarySrc.loop = isLoop;

        m_primarySrc.Play();

        _bgmid = tableID;
    }

    public void SetBgmVolme( float volume )
    {
        m_primarySrc.volume = volume;
        DirectorBGMSrc.volume = volume;
    }

    //191006
    public void SetFxVolume(float volume)
    {
        m_masterFXVol = volume;
        //FxSndSrc.volume = volume;
        DirectorFXSrc.volume = volume;
        Log.Show("SerFXVolume : " + volume);
    }

    public void SetVoiceVolume(float volume)
    {
        m_voiceSrc.volume = volume;
    }

    public void StopBgm()
    {
        _bgmid = -10;
        m_primarySrc.Stop();
        //StartCoroutine(FadeAudioOff());
    }

    public void PauseBgm()
    {
        if(m_primarySrc == null)
        {
            return;
        }

        m_primarySrc.Pause();
    }

    public void ResumeBgm()
    {
        if(m_primarySrc == null)
        {
            return;
        }

        m_primarySrc.UnPause();
    }

    public void SetSeVolumeChange(float volume)
    {
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
        {
            foreach (Action<float> action in _seVolumeChangeActionList)
            {
                action?.Invoke(volume);
            }
        }
    }

    /*public void FadeBgm(string name, bool loop, float volume)
    {
        SoundTable.Sound.Param param = m_soundTable.FindSound(a => a.Name == name);
        if (param == null)
        {
            Debug.LogError(name + "이름의 사운드가 존재하지 않습니다.");
            return;
        }

        if (m_dicAudioClip.ContainsKey(name) == false)
        {
            AddAudioClip(name, param, FSaveData.Instance.GetBGVolume());
        }

        if (m_dicAudioClip[name] == null || m_dicAudioClip[name].Count <= 0)
        {
            Debug.LogError(name + "에 TValue가 비어있습니다.");
            return;
        }

        m_primarySrc.Play();
        StartCoroutine(FadeAudio(m_dicAudioClip[name][0].clip, loop, GetVolume(eSoundType.Ambience, volume)));

        _bgmid = param.ID;
    }


    public void FadeBgm(int tableID, bool loop, float volume)
    {
        SoundTable.Sound.Param param = m_soundTable.FindSound(tableID);
        if (param == null)
        {
            Debug.LogError(tableID + "의 사운드가 존재하지 않습니다.");
            return;
        }

        if (m_dicAudioClip.ContainsKey(param.Name) == false)
        {
            AddAudioClip(param.Name, param, FSaveData.Instance.GetBGVolume());
        }

        if (m_dicAudioClip[param.Name] == null || m_dicAudioClip[param.Name].Count <= 0)
        {
            Debug.LogError(param.Name + "에 TValue가 비어있습니다.");
            return;
        }

        m_primarySrc.Play();
        StartCoroutine(FadeAudio(m_dicAudioClip[param.Name][0].clip, loop, GetVolume(eSoundType.Ambience, volume)));

        _bgmid = tableID;
    }


    public void FadeBgm(AudioClip clip, bool loop, float volume)
    {
        if (clip == null)
            return;

        StartCoroutine(FadeAudio(clip, loop, GetVolume(eSoundType.Ambience, volume)));
    }

    //#endregion


    private IEnumerator FadeAudioOff()
    {
        while (m_primarySrc.volume > 0.0f)
        {
            m_primarySrc.volume -= ( Time.deltaTime * 0.5f );
            yield return null;
        }
    }


    private IEnumerator FadeAudio(AudioClip newAudio, bool loop, float volume)
    {
        m_secondarySrc.enabled = true;
        m_secondarySrc.clip = newAudio;
        m_secondarySrc.volume = 0.0f;
        m_secondarySrc.Play();

        while (m_primarySrc.volume > 0.0f)
        {
            m_primarySrc.volume -= ( Time.deltaTime * 0.5f );
            m_secondarySrc.volume += ( Time.deltaTime * 0.5f );

            yield return null;
        }

        AudioSource backupSrc = m_primarySrc;
        m_primarySrc = m_secondarySrc;
        m_secondarySrc = backupSrc;

        m_primarySrc.volume = volume;
        m_primarySrc.loop = loop;

        m_secondarySrc.volume = 0.0f;
        m_secondarySrc.enabled = false;
    }*/

    public void PlayAmbienceSnd(AudioClip clip, float volume, bool isLoop = true)
    {
        if (clip == null || m_ambienceSrc == null)
            return;

        m_ambienceSrc.clip = clip;
        m_ambienceSrc.volume = GetVolume(eSoundType.Ambience, volume);
        m_ambienceSrc.loop = isLoop;

        m_ambienceSrc.Play();
    }

    public void StopAmbienceSnd()
    {
        if (m_ambienceSrc == null)
            return;

        m_ambienceSrc.Stop();
    }

    /// <summary>
    ///  ID를 가지고 AudioClip를 생성하고 반환합니다.
    /// </summary>
    public AudioClip GetAudioClip(int id)
    {
        SoundTable.Sound.Param param = m_soundTable.FindSound(id);
        if (param == null)
            return null;

        AudioClip clip = ResourceMgr.Instance.LoadFromAssetBundle("sound", param.Path) as AudioClip;
        return clip;
    }

    //Enemy MixerGroup Controller
    public void SetMonsterSndMixerdB(float changedB)
    {
        enemyMixer.audioMixer.SetFloat("EnemyVol", changedB);
    }

}
