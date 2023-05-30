
using UnityEngine;
using System.Collections.Generic;


public class LobbyPlayer : Unit
{
    [Header("[Lobby Player Property]")]
    public Vector3  LobbyPos            = new Vector3(-0.2f, 0.0f, 0.65f);
    public float    LobbyRenderCamSize  = 0.88f;
    public string   ChangeWeaponName    = "";

    public long     Uid         { get; set; }
    public Light    CharLight   { get; private set; }

    private readonly float IDLE_INIT_TIME = 15.0f;
    private readonly float IDLE_TIME_RAND_MIN = 15.0f;
    private readonly float IDLE_TIME_RAND_MAX = 35.0f;

    private GameTable.Character.Param               mData           = null;
    private eAnimation                              mCurAni         = eAnimation.None;
    private float                                   mCheckTime      = 0.0f;
    private float                                   mAniRandTime    = 0.0f;
    private CharData                                mCharData       = null;
    private List<GameTable.LobbyAnimation.Param>    mTouchTableList = new List<GameTable.LobbyAnimation.Param>();
    private AudioSource                             mVoiceAudioSrc  = null;
    private float[]                                 mSamples        = new float[128];


    public override void SetData(int tableId)
    {
        mData = GameInfo.Instance.GetCharacterData(tableId);
        if (mData == null)
            return;

        m_tableId = tableId;
        m_maxHp = 1;
        m_curHp = m_maxHp;

        m_folder = Utility.GetFolderFromPath(mData.Model);

        m_originalSpeed = mData.MoveSpeed * 0.25f;
        m_curSpeed = m_originalSpeed;

        if (CharLight == null)
            CharLight = transform.GetComponentInChildren<Light>();
        
        mCharData = GameInfo.Instance.GetCharDataByTableID(tableId);
        
        List<GameTable.LobbyAnimation.Param> lobbyAnimList =
            GameInfo.Instance.GameTable.FindAllLobbyAnimation(x => x.Character == tableId && x.Face.Contains("Touch"));
        
        mTouchTableList.Clear();
        
        for (int i = 0; i < lobbyAnimList.Count; i++)
        {
            int flag = 1 << lobbyAnimList[i].No;
            if ((mCharData?.CharAniFlag & flag) == flag)
            {
                mTouchTableList.Add(lobbyAnimList[i]);
            }
            else
            {
                if (lobbyAnimList[i].LockType == 0)
                {
                    mTouchTableList.Add(lobbyAnimList[i]);
                }
            }
        }
    }

    public void CharLight_On()
    {
        CharLight.gameObject.SetActive(true);
    }

    public void CharLight_Off()
    {
        CharLight.gameObject.SetActive(false);
    }

    public void Init(eAnimation animation = eAnimation.Idle01)
    {
        mCurAni = animation;

        if (mCurAni != eAnimation.None)
        {
            PlayAni(mCurAni, 0, eFaceAnimation.Idle01, 0);
            mCheckTime = 0.0f;
            mAniRandTime = IDLE_INIT_TIME;
        }

        mVoiceAudioSrc = SoundManager.Instance.GetAudioSource(SoundManager.eSoundType.Voice);
    }

    public void Touch()
    {
		if (m_aniEvent == null)
		{
			return;
		}

		switch (mCurAni)
        {
            case eAnimation.None:
            case eAnimation.Enter01:
            case eAnimation.LobbyTouchMotion01:
            case eAnimation.LobbyTouchMotion02:
            case eAnimation.LobbyTouchMotion03:
            case eAnimation.LobbyTouchMotion04:
            case eAnimation.LobbyTouchMotion05:
            case eAnimation.LobbyTouchMotion06:
                return;
        }
        
        int count = mTouchTableList.Count;
        if(m_aniEvent.HasAni(eAnimation.LobbyTouchMotion03))
        {
            ++count;
        }

        int rand = Random.Range(0, count);
        if (mTouchTableList.Count <= rand)
        {
            mCurAni = eAnimation.LobbyTouchMotion03;
            PlayAni(eAnimation.LobbyTouchMotion03, 0, eFaceAnimation.Touch03, 0);
        }
        else
        {
            System.Enum.TryParse(mTouchTableList[rand].Animation, true, out eAnimation anim);
            System.Enum.TryParse(mTouchTableList[rand].Face, true, out eFaceAnimation faceAnim);

            mCurAni = anim;
            PlayAni(anim, 0, faceAnim, 0);
        }
        
        mCheckTime = mAniRandTime;
    }
    
    public bool EnterAnimation()
    {
		if(m_aniEvent == null)
		{
			return false;
		}

        if (0 < LobbyUIManager.Instance.ActivePopupList.Count)
        {
            return false;
        }
        
        GameTable.LobbyAnimation.Param tableData =
            GameInfo.Instance.GameTable.FindLobbyAnimation(x =>
                x.Character == Lobby.Instance.LobbyPlayer.tableId &&
                x.Animation.ToLower().Equals(eAnimation.Enter01.ToString().ToLower()));
        if (tableData == null)
        {
            return false;
        }
        
        CharData charData = GameInfo.Instance.GetCharDataByTableID(tableData.Character);
        if (charData == null)
        {
            return false;
        }
        
        int flag = 1 << tableData.No;
        if ((charData.CharAniFlag & flag) != flag)
        {
            return false;
        }
        
        System.Enum.TryParse(tableData.Animation, out eAnimation anim);
        System.Enum.TryParse(tableData.Face, out eFaceAnimation faceAnim);

        if (!Lobby.Instance.LobbyPlayer.m_aniEvent.HasAni(anim))
        {
            return false;
        }
        
        Lobby.Instance.LobbyPlayer.PlayAni(anim, 0, faceAnim, 0);
        
        mCurAni = anim;
        mCheckTime = mAniRandTime;
        
        return true;
    }
    
    protected override void FixedUpdate()
    {
		if(m_aniEvent == null)
		{
			return;
		}

        if (mCurAni == eAnimation.None)
        {
            if (m_aniEvent.IsAniPlaying(eAnimation.Lobby_Costume) == eAniPlayingState.End)
            {
                PlayAni(eAnimation.Idle01, 0, eFaceAnimation.Idle01, 0);
            }
            else if (m_aniEvent.IsAniPlaying(eAnimation.Lobby_Weapon) == eAniPlayingState.End)
            {
                PlayAni(eAnimation.Lobby_Weapon_Idle, 0, eFaceAnimation.Weapon_Idle, 0);

				PlayerGuardian playerGuardian = RenderTargetChar.Instance.AddedWeapon as PlayerGuardian;
				if ( playerGuardian ) {
					RenderTargetChar.Instance.AddedWeapon.PlayAni( eAnimation.Lobby_Weapon_Idle );
				}
			}
            else if (m_aniEvent.IsAniPlaying(eAnimation.Lobby_Selete) == eAniPlayingState.End)
            {
                PlayAni(eAnimation.Lobby_Selete_Idle, 0, eFaceAnimation.Selete_Idle, 0);
            }
        }
        else
        {
            mCheckTime += Time.fixedDeltaTime;
            if (mCheckTime >= mAniRandTime)
            {
                bool isLoop = false;
                if (mCurAni == eAnimation.Idle01)
                {
                    isLoop = true;
                }

                if (m_aniEvent.IsAniPlaying(mCurAni, isLoop) == eAniPlayingState.End)
                {
                    mCheckTime = 0.0f;
                    mAniRandTime = NextAniRandTime();

                    if (Lobby.Instance.LobbyPlayer == this && mCurAni == eAnimation.Idle01)
                    {
                        if (Random.Range(0, 2) == 0)
                        {
                            mCurAni = eAnimation.Idle02;
                            PlayAni(eAnimation.Idle02, 0, eFaceAnimation.Idle02, 0);

                            mCheckTime = mAniRandTime;
                        }
                        else
                        {
                            mCurAni = eAnimation.Idle03;
                            PlayAni(eAnimation.Idle03, 0, eFaceAnimation.Idle03, 0);

                            mCheckTime = mAniRandTime;
                        }
                    }
                    else
                    {
                        mCurAni = eAnimation.Idle01;
                        PlayAni(mCurAni, 0, eFaceAnimation.Idle01, 0);
                    }
                }
            }

            UpdateLipSync();
        }
    }

    public float NextAniRandTime()
    {
        return Random.Range(IDLE_TIME_RAND_MIN, IDLE_TIME_RAND_MAX);
    }

    public void UpdateLipSync()
    {
        if (m_aniEvent && m_aniEvent.aniFace && mVoiceAudioSrc && mVoiceAudioSrc.isPlaying)
        {
            if (VoiceMgr.Instance.CurVoicePlayingCharTableId == tableId)
            {
                mVoiceAudioSrc.GetSpectrumData(mSamples, 0, FFTWindow.BlackmanHarris);
                m_aniEvent.aniFace.SetLayerWeight(1, Mathf.Lerp(m_aniEvent.aniFace.GetLayerWeight(1), Mathf.Clamp(Mathf.Max(mSamples) * 10, 0, 100), 0.4f));
            }
            else
            {
                m_aniEvent.aniFace.SetLayerWeight(1, Mathf.Lerp(m_aniEvent.aniFace.GetLayerWeight(1), 0, 0.1f));
            }
        }
    }
}
