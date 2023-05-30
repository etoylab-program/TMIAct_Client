using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eVOICEGROUP
{
    None = 0,
    VOICECHAR,
    VOICESUPPORTER,
    VOICEMONSTER,
}

public enum eVOICECHAR
{
    None = 0,
    FirstGreetings,
    GradeUp,
    GradeMax,
    Emotion,        //20
    Greetings,
    ChangeWeapon,   //2
    ChangeWeaponUp,
    SkillSet,
    CostumeBuy,
    CostumeChange,  //2
    MainChar,
    PlayStageSel,
    UIMove,
    UIChar,
    UIInven,
    UIGacha,
    UIStore,
    UIMission,
    UIToLobby,
    Start,
    Ultimate,
    Death,
    Revival,
    Giveup,
    Success,
    Failure,
    AttackWeakness, //7
    AttackStrength, //7
    AttackCharge,   //2
    AttackLast,     //5
    BeAttackedW,    //3
    BeAttackedS,    //3
    ArenaWin,       //3
    WeaponSkill,
    Count,
}

public enum eVOICESUPPORTER
{
    None = 0,
    Greetings,
    LevelUp,
    SkillLvUp,
    SkillLvMax,
    FavorUp,
    FavorMax,
    WakeUp,
    WakeMax,
    Equip,
    FacilityStart,
    FacilityComplete,
    FacilityHelp,
    DispatchStart,
    DispatchComplete,
    Wait,
    Warning,            //2
    Crisis,
    StatusEffect,
    TimeLimit,
    Combo,              //4
    SkillGauge,
    Skill,
    GaugeCharge,
    Count,
}

public enum eVOICEMONSTER
{
    None = 0,
    Test,
    Count,
}

public enum eVOICEARENATYPE
{
    None = 0,
    ArenaEnter,
    ArenaCheer,
    Count,
}

public enum eVOICEARENATYPENAME
{
    None = 0,
    ArenaEnterR,    // 출전 랜덤
    ArenaEnterS,    // 출전 같은 캐릭
    ArenaEnterC,    // 출전 인연
    ArenaCheerR,    // 응원 랜덤
    ArenaCheerC,    // 응원 인연
    Count,
}

/*
사운드 파일
C_(캐릭터번호)_(분류)_(넘버)
서포터
S_(서포터번호)_(분류)_(넘버)
*/
public class VoiceMgr : FMonoSingleton<VoiceMgr>
{
    //private Dictionary<eVOICECHAR,int> m_dicVoiceRndCount = new Dictionary<eVOICECHAR,int>();
    public int CurVoicePlayingCharTableId { get; private set; } = 0;

    void Awake()
    {
        DontDestroyOnLoad();
        Init();
    }

    private void Init()
    {
        //m_dicVoiceRndCount.Add(eVOICECHAR.AttackCharge, 2);
    }
   
    public SoundManager.sSoundInfo PlayChar(eVOICECHAR kind, int id, int index = -1)
    {
        if( id == -1 ) {
            return null;
		}

        int num = 0;
        if (index == -1)
        {
            var tabledata = SoundManager.Instance.SoundTable.FindVoiceRndCount(x => x.Group == (int)eVOICEGROUP.VOICECHAR && x.ID == (int)kind);
            if (tabledata == null)
                return null;

            num = Random.Range(0, tabledata.Count) + 1;
        }
        else
        {
            num = index;
        }

        CurVoicePlayingCharTableId = id;

        string file = string.Format("C_{0}_{1}_{2}.ogg", id.ToString(), kind.ToString(), num.ToString());
        return SoundManager.Instance.PlayVoice(SoundManager.eVoiceType.Character, "voicechar01", "VoiceChar01/" + file);
    }

    public SoundManager.sSoundInfo PlayPVPChar(int tableId, eVOICEARENATYPE type, eVOICEARENATYPENAME typeName, int index, int rndCount)
    {
        int num = 0;
        if(index == -1)
        {
            num = Random.Range(1, rndCount + 1);
        }
        else
        {
            num = index;
        }

        string file = string.Format("C_{0}_{1}_{2}.ogg", tableId.ToString(), typeName.ToString(), num.ToString());
        return SoundManager.Instance.PlayVoice(SoundManager.eVoiceType.Character, "voicechar01", "VoiceChar01/" + file, true);
    }

	public SoundManager.sSoundInfo PlaySupporter( eVOICESUPPORTER kind, int id, int index = -1 )
	{
		if( AppMgr.Instance.IsInGameScene() )
		{
			if( World.Instance.IsEndGame || World.Instance.ProcessingEnd || Director.IsPlaying )
			{
				if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage || World.Instance.TestScene )
				{
					World.Instance.UIPlay.HideSupporterTalking();
				}

				return null;
			}
		}

		int num = 0;
		if( index == -1 )
		{
			var tabledata = SoundManager.Instance.SoundTable.FindVoiceRndCount( x => x.Group == (int)eVOICEGROUP.VOICESUPPORTER && x.ID == (int)kind );
            if( tabledata == null )
            {
                return null;
            }

			num = Random.Range( 0, tabledata.Count ) + 1;
		}
		else
		{
			num = index;
		}

		string file = string.Format("S_{0}_{1}_{2}.ogg", id.ToString(), kind.ToString(), num.ToString());
		SoundManager.sSoundInfo info = SoundManager.Instance.PlayVoice( SoundManager.eVoiceType.Supporter, "voicesupporter01", "VoiceSupporter01/" + file );

		if( info != null && info.clip != null )
		{
			if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage || World.Instance.TestScene )
			{
				if( World.Instance.UIPlay.gameObject.activeSelf )
				{
					World.Instance.UIPlay.ShowSupporterTalking( info.clip.length * 0.7f );
				}
			}
		}

		return info;
	}

	public SoundManager.sSoundInfo PlayStory(string bundlePath, string file)
    {
        //string bundle = string.Format("voicechapter{0:00}", chapter );
        //string bundlepath = string.Format("VoiceChapter{0:00}", chapter);

        return SoundManager.Instance.PlayVoice(SoundManager.eVoiceType.Story, bundlePath, bundlePath + "/" + file);
    }

    public SoundManager.sSoundInfo PlayStory(string bundlePath, string file, float volume)
    {
        return SoundManager.Instance.PlayVoice(SoundManager.eVoiceType.Story, bundlePath, bundlePath + "/" + file, volume);
    }

    public SoundManager.sSoundInfo PlayMonster(eVOICEMONSTER kind, int id, int index = -1)
    {
        id = 1;

        int num = 0;
        if (index == -1)
        {
            var tabledata = SoundManager.Instance.SoundTable.FindVoiceRndCount(x => x.Group == (int)eVOICEGROUP.VOICEMONSTER && x.ID == (int)kind);
            if (tabledata == null)
                return null;

            num = Random.Range(0, tabledata.Count) + 1;
        }
        else
        {
            num = index;
        }

        string file = string.Format("M_{0}_{1}_{2}.ogg", id.ToString(), kind.ToString(), num.ToString());
        return SoundManager.Instance.PlayVoice(SoundManager.eVoiceType.Monster, "voicemonster01", "VoiceMonster01/" + file);
    }

    public SoundManager.sSoundInfo PlayPassiveSkill(int charTableId, int id)
    {
        string file = string.Format("P_{0}_{1}.ogg", charTableId, id);
        return SoundManager.Instance.PlayVoice(SoundManager.eVoiceType.Passive, "voicechar01", "VoiceChar01/" + file);
    }
}
