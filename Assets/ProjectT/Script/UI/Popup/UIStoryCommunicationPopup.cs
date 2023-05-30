using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStoryCommunicationPopup : FComponent
{
    public enum eNEXTTYPE
    {
        NEXT = 0,
        TIMENEXT,
        WAIT,
    }

    public UIButton kNextBtn;
    public UIStoryCommunicationUnit kMainTalk;
    //public List<UIStoryCommunicationUnit> kUnitList;
    public GameObject kCharRoot;
    public ActiveVisualNovel activevn = null;
    private int _index;
    private bool _bAuto;

    private Unit m_MainUnit;
    private FigureUnit m_FigureUnit;
    private Player m_MainPlayer;

    private Vector3 m_OriginPos;
    private Quaternion m_OriginRot;

    public FList kStoryCommunicationLogList;

    private Dictionary<string, Texture2D> m_dicCharTextures = new Dictionary<string, Texture2D>();
    private List<Texture2D> m_sendCharTextures = new List<Texture2D>();
    private Dictionary<string, AudioClip> m_dicSoundEffects;
    private const int m_CommunicationLogCount = 5;
    private int m_MainCharPos = 0;
    private int mId = 0;

    private eScenarioType m_currentType = eScenarioType.NONE;

    //LogList
    public List<int> m_LogIndexList = new List<int>();

 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();

        if (activevn)
            activevn.director.ActiveCanvas(false);
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		activevn = null;

        if(m_MainPlayer != null)
        {
            World.Instance.InGameCamera.RestoreCullingMask();
            World.Instance.InGameCamera.SetPlayerLightPositionAndRotation(Vector3.zero, Vector3.zero);

            m_MainPlayer.LockChangeFace = false;
            m_MainPlayer.EnableDefaultFace();
            
            m_MainPlayer.PlayAni(eAnimation.AttackIdle01);
                       
            World.Instance.InGameCamera.SetDefaultMode();

            if ( m_MainPlayer.Guardian ) {
                m_MainPlayer.Guardian.ShowMesh( true );
            }
        }
        m_MainPlayer = null;

        if (m_LogIndexList != null)
            m_LogIndexList.Clear();

        ScenarioMgr.Instance.OnPopupEnd();
        base.OnDisable();
    }

    public override void InitComponent()
	{
        _index = 1;

        var obj = UIValue.Instance.GetValue(UIValue.EParamType.StroyID);
        if (obj == null)
            return;

        //kStoryCommunicationLogList.InitBottomFixing();
        if (kStoryCommunicationLogList.EventGetItemCount == null)
            kStoryCommunicationLogList.EventGetItemCount = GetSlotCount;
        if (kStoryCommunicationLogList.EventUpdate == null)
            kStoryCommunicationLogList.EventUpdate = UpdateSlot;
        kStoryCommunicationLogList.UpdateList();
        mId = (int)obj;

        kMainTalk.Init();

        ScenarioMgr.Instance.InitScenarioScene(mId);
        _bAuto = false;
        kNextBtn.gameObject.SetActive(false);

        if(AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
            World.Instance.InGameCamera.ExcludCullingMask((int)eLayer.PlayerClone);

        AppMgr.Instance.CustomInput.ShowCursor(true);

        m_MainUnit = m_MainPlayer = World.Instance.Player;
        if(m_MainPlayer != null)
        {
            /*
            m_MainPlayer.SetKinematicRigidBody();
            m_MainPlayer.GetController().Pause(true);
            World.Instance.playerCam.enabled = false;
            m_MainCharPos = m_MainUnit.tableId;
            m_MainPlayer.MainCollider.enabled = false;
            m_MainPlayer.rigidBody.velocity = Vector3.zero;
            m_MainPlayer.aniEvent.StopAni(m_MainPlayer.aniEvent.curAniType);
            m_OriginPos = m_MainUnit.transform.localPosition;
            m_OriginRot = m_MainUnit.transform.localRotation;
            */
            m_MainPlayer.EnableBoneFace();
            m_MainPlayer.LockChangeFace = true;

            m_MainCharPos = m_MainUnit.tableId;

            World.Instance.InGameCamera.SetFocusMode(m_MainUnit.gameObject, 30.0f, m_MainPlayer.CommunicationCameraPos, m_MainPlayer.CommunicationCameraRot);
            World.Instance.InGameCamera.SetPlayerLightPositionAndRotation(Vector3.zero, new Vector3(13.0f, 15.0f, 16.0f));

            m_MainUnit.actionSystem.CancelCurrentAction();
            m_MainUnit.transform.position = m_MainUnit.posOnGround;
            m_MainUnit.SetGroundedRigidBody();

            if ( m_MainPlayer.Guardian ) {
                m_MainPlayer.Guardian.ShowMesh( false );
            }
        }
        else
        {
            Debug.LogWarning("플레이어 설정이 필요함");
            
            m_MainUnit = m_FigureUnit = GameSupport.CreateFigure(101001, null, false);
            m_MainUnit.name = "asagi_G";
            m_MainUnit.aniEvent.GetComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)ResourceMgr.Instance.LoadFromAssetBundle("unit", "Unit/Character/Asagi_G/asagi_G.controller");

            TextAsset aniEventFile = ResourceMgr.Instance.LoadFromAssetBundle("unit", "Unit/Character/Asagi_G/asagi_G.bytes") as TextAsset;
            TextAsset aniSndEventFile = ResourceMgr.Instance.LoadFromAssetBundle("unit", "Unit/Character/Asagi_G/asagi_G_snd.bytes") as TextAsset;
            m_MainUnit.aniEvent.Init(m_MainUnit, "Character", null);
            m_MainUnit.aniEvent.LoadEvent(aniEventFile, aniSndEventFile);
            m_MainCharPos = 1;

            m_MainUnit.transform.parent = kCharRoot.transform;
            m_MainUnit.transform.localRotation = new Quaternion(0f, 180f, 0f, 1f);
            m_MainUnit.transform.localPosition = Vector3.zero;
            m_MainUnit.transform.localScale = new Vector3(700f, 700f, 700f);
            m_OriginPos = m_MainUnit.transform.localPosition;
            m_OriginRot = m_MainUnit.transform.localRotation;
            //Layer UI로 셋팅
            m_MainUnit.transform.SetChildLayer(5);
        }

        Player player = m_MainUnit as Player;
        if (player && player.costumeUnit && player.aniEvent)
        {
            m_MainUnit.costumeUnit.ChangeAllWeaponName(player.ChangeWeaponName);
            m_MainUnit.aniEvent.Rebind();
        }

        mbClose = false;

        kMainTalk.Off();
        float aniStartTime = m_MainUnit.aniEvent.PlayAni(eAnimation.Communication_Start);
        m_MainUnit.aniEvent.PlayAni(eAnimation.Communication_Idle, true, 0.15f, aniStartTime);
        float ftime = GetOpenAniTime();
        Invoke("Next", ftime);
    }

    private void Update()
    {
        AudioSpectrumData();
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}

    public void OnClick_NextBtn()
    {
        var param = ScenarioMgr.Instance.GetScenarioParam(_index);
        if (param == null)
            return;
       
        if (param.Type == "CHAR_TALK")
        {
            if (kMainTalk.kTextLabel.TextScroll)
            {
                kMainTalk.SetText(param.Value2, false);
                return;
            }
            else
            {
                if (param.Voice != "" && param.Voice != string.Empty)
                    SoundManager.Instance.StopVoice();
            }
        }

        _index += 1;
        Next();
       
    }

    public void OnClick_SkipBtn()
    {
        /*
        //Communication_End 애니메이션을 Play중이면 리턴
        if (m_MainUnit.aniEvent.IsAniPlaying(eAnimation.Communication_End) != eAniPlayingState.None)
            return;
            */

        if(mbClose)
        {
            return;
        }

        mbClose = true;

        int stageTableId = -1;
        if (World.Instance.StageInfoType == World.eStageInfoType.New)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("SkipCommunication", new Firebase.Analytics.Parameter("StageId", stageTableId),
                                                                               new Firebase.Analytics.Parameter("StoryId", mId));
        }

        StopCoroutine("WaitForEndAni");
        StartCoroutine("WaitForEndAni");
    }

    private IEnumerator WaitForEndAni()
    {
        if (m_MainUnit)
        {
            yield return new WaitForSeconds(m_MainUnit.PlayAni(eAnimation.Communication_End) * 0.7f);
        }

        AppMgr.Instance.CustomInput.ShowCursor(false);

        kMainTalk.Off();
        OnClickClose();
    }

    bool mbClose = false;
    public void Next()
    {
        var param = ScenarioMgr.Instance.GetScenarioParam(_index);
        if (param == null)
            return;

        kNextBtn.gameObject.SetActive(false);

        float fwait = 0.0f;
        float ftimescale = 1.0f;
        if (_bAuto)
            ftimescale = 0.2f;

        float ftime = param.Time * ftimescale;
        m_currentType = (eScenarioType)System.Enum.Parse(typeof(eScenarioType), param.Type);
        switch(m_currentType)
        {
            case eScenarioType.CHAR_DESTROY:
                {
                    kMainTalk.Init();

                }
                break;
            case eScenarioType.CHAR_TALK:
                {
                    kMainTalk.Off();

                    int charPos = (int)param.Pos;

                    param.Value3 = param.Value3.Replace(" ", "");
                    if (!string.IsNullOrEmpty(param.Value3))
                    {
                        if (!m_dicCharTextures.ContainsKey(param.Value3))
                        {
                            string texPath = string.Format("{0}.png", param.Value3);
                            Texture2D tex = ResourceMgr.Instance.LoadFromAssetBundle("icon", texPath) as Texture2D;
                            m_dicCharTextures.Add(param.Value3, tex);
                        }
                        kMainTalk.SetText(m_dicCharTextures[param.Value3], param.Value2, param.BundlePath, param.Voice, charPos.Equals(m_MainCharPos), true);
                    }
                    else
                    {
                        //VoiceOnly
                        kMainTalk.SetText(param.Value1, param.Value2, param.BundlePath, param.Voice, charPos.Equals(m_MainCharPos), true);
                    }
                    if(m_MainPlayer != null)
                        m_MainPlayer.EnableBoneFace();

                    m_LogIndexList.Add(_index);
                    if (m_LogIndexList.Count > 4)
                        m_LogIndexList.RemoveAt(0);
                    kStoryCommunicationLogList.UpdateList();
                }
                break;
            case eScenarioType.CHAR_TALK_S:
                {
                    kMainTalk.Off();
                    param.Value4 = param.Value4.Replace(" ", "");
                    param.Value3 = param.Value3.Replace(" ", "");
                    param.Voice = param.Voice.Replace(" ", "");
					string[] charPos = Utility.Split(param.Value4, ','); //param.Value4.Split(',');
					string[] charImg = Utility.Split(param.Value3, ','); //param.Value3.Split(',');
					string[] voice = Utility.Split(param.Voice, ','); //param.Voice.Split(',');

					m_sendCharTextures.Clear();

                    for (int i = 0; i < charImg.Length; i++)
                    {
                        if (!m_dicCharTextures.ContainsKey(charImg[i]))
                        {
                            string texPath = string.Format("{0}.png", charImg[i]);
                            Texture2D tex = ResourceMgr.Instance.LoadFromAssetBundle("icon", texPath) as Texture2D;
                            m_dicCharTextures.Add(charImg[i], tex);
                        }

                        m_sendCharTextures.Add(m_dicCharTextures[charImg[i]]);
                    }
                    if (m_MainPlayer != null)
                        m_MainPlayer.EnableBoneFace();
                    kMainTalk.SetText(m_sendCharTextures, param.Value2, param.BundlePath, voice, false, true);

                    m_LogIndexList.Add(_index);
                    if (m_LogIndexList.Count > 4)
                        m_LogIndexList.RemoveAt(0);


                    kStoryCommunicationLogList.UpdateList();
                }
                break;
            case eScenarioType.CHAR_SET:
                {
                    if (!string.IsNullOrEmpty(param.Value2))
                    {
                        try
                        {
                            m_MainUnit.PlayAni((eAnimation)System.Enum.Parse(typeof(eAnimation), param.Value2));
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogWarning(param.Value2 + " Animation is NULL \n" + e);
                        }
                    }
                    if (!string.IsNullOrEmpty(param.Value3))
                    {
                        try
                        {
                            m_MainUnit.PlayFaceAni((eFaceAnimation)System.Enum.Parse(typeof(eFaceAnimation), param.Value3));
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogWarning(param.Value3 + " FaceAnimation is NULL \n" + e);
                        }
                    }
                }
                break;
            case eScenarioType.SE_PLAY:
                {
                    if (m_dicSoundEffects == null)
                        m_dicSoundEffects = new Dictionary<string, AudioClip>();

                    if (!m_dicSoundEffects.ContainsKey(param.Value1))
                    {
                        SoundManager.Instance.AddAudioClip(param.Value1, param.Value2, FSaveData.Instance.GetSEVolume());
                        AudioClip ac = SoundManager.Instance.GetSnd(param.Value1);
                        m_dicSoundEffects.Add(param.Value1, ac);
                    }

                    if (param.Value3.Equals("LOOP"))
                    {
                        SoundManager.Instance.PlayFxLoopSnd(param.Value1, m_dicSoundEffects[param.Value1], FSaveData.Instance.GetSEVolume());
                    }
                    else
                    {
                        SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, m_dicSoundEffects[param.Value1], FSaveData.Instance.GetSEVolume());
                    }
                }
                break;
            case eScenarioType.SE_STOP:
                {
                    if (null != m_dicSoundEffects)
                    {
                        if (m_dicSoundEffects.ContainsKey(param.Value1))
                        {
                            SoundManager.Instance.PlayFxLoopSnd(param.Value1, FSaveData.Instance.GetSEVolume(), true);
                        }
                    }
                }
                break;
            case eScenarioType.CLOSE:
                {
                    if (m_MainUnit != null)
                    {
                        /*
                        AttachObject[] attachObjs = m_MainUnit.GetComponentsInChildren<AttachObject>(true);
                        for (int i = 0; i < attachObjs.Length; i++)
                        {
                            attachObjs[i].gameObject.SetActive(true);
                        }
                        */

                        float length = m_MainUnit.PlayAni(eAnimation.Communication_End);
                        mbClose = true;
                         
                        Invoke("CloseAfterEndAni", length);
                    }
                    else
                    {
                        CloseAfterEndAni();
                    }

                    return;
                }
            case eScenarioType.DELAY: {
                ftime = param.Time;
            }
            break;

            default:
                {
                    Debug.LogWarning(m_currentType.ToString() + " 해당하는 액션이 없습니다.");
                }
                break;
        }
        
        if (param.Next == (int)eNEXTTYPE.NEXT)
        {
            _index += 1;
            Next();
        }
        else if (param.Next == (int)eNEXTTYPE.TIMENEXT)
        {
            Invoke("AutoNext", ftime);
        }
        else if (param.Next == (int)eNEXTTYPE.WAIT)
        {
            if (_bAuto)
            {
                Invoke("AutoNext", ftime);
            }
            else
            {
                if (fwait == 0.0f)
                    ActiveNextBtn();
                else
                    Invoke("ActiveNextBtn", ftime);
            }
        }

    }

    private void CloseAfterEndAni()
    {
        if (activevn != null)
            activevn.EndVisualNovel();

        SetUIActive(false, true);
        AppMgr.Instance.CustomInput.ShowCursor(false);

        if (m_MainUnit)
        {
            m_MainUnit.PlayAni(eAnimation.AttackIdle01);
        }
        kMainTalk.Off();
    }

    public void AutoNext()
    {
        OnClick_NextBtn();
    }

    public void ActiveNextBtn()
    {
        kNextBtn.gameObject.SetActive(true);
    }

    /*public override void OnClose()
    {
        base.OnClose();
        ScenarioMgr.Instance.OnPopupEnd();
    }*/

    private int GetSlotCount()
    {
        if (m_LogIndexList.Count - 1 > 0)
            return m_LogIndexList.Count - 1;
        return 0;
        //return m_LogIndexList.Count;
    }

    private void UpdateSlot(int index, GameObject slotObj)
    {
        do
        {
            UIStoryCommunicationUnit slot = slotObj.GetComponent<UIStoryCommunicationUnit>();
            if (null == slot)
                break;

            //Scenario_KORTable.Scenario.Param logParam = ScenarioMgr.Instance.GetScenarioParam(_index - (m_CommunicationLogCount - index));
            ScenarioParam logParam = null;
            if (0 <= index && m_LogIndexList.Count - 1 > index)
                logParam = ScenarioMgr.Instance.GetScenarioParam(m_LogIndexList[index]);

            if (logParam == null)
                break;

            if (!slot.IsInit)
                slot.Init();

            slot.Off();

            if (logParam.Type.Equals("CHAR_TALK"))
            {
                int charPos = (int)logParam.Pos;

                logParam.Value3 = logParam.Value3.Replace(" ", "");
                if (!string.IsNullOrEmpty(logParam.Value3))
                {
                    if (!m_dicCharTextures.ContainsKey(logParam.Value3))
                    {
                        string texPath = string.Format("{0}.png", logParam.Value3);
                        Texture2D tex = ResourceMgr.Instance.LoadFromAssetBundle("icon", texPath) as Texture2D;
                        m_dicCharTextures.Add(logParam.Value3, tex);
                    }
                    slot.SetText(m_dicCharTextures[logParam.Value3], logParam.Value2, logParam.BundlePath, logParam.Voice, charPos.Equals(m_MainCharPos));
                }
                else
                {
                    //VoiceOnly
                    slot.SetText(logParam.Value1, logParam.Value2, logParam.BundlePath, logParam.Voice, charPos.Equals(m_MainCharPos));
                }
            }
            else if (logParam.Type.Equals("CHAR_TALK_S"))
            {
                logParam.Value4 = logParam.Value4.Replace(" ", "");
                logParam.Value3 = logParam.Value3.Replace(" ", "");
                logParam.Voice = logParam.Voice.Replace(" ", "");
				string[] charPos = Utility.Split(logParam.Value4, ','); //logParam.Value4.Split(',');
				string[] charImg = Utility.Split(logParam.Value3, ','); //logParam.Value3.Split(',');
				string[] voice = Utility.Split(logParam.Voice, ','); //logParam.Voice.Split(',');

				m_sendCharTextures.Clear();

                for (int i = 0; i < charImg.Length; i++)
                {
                    if (!m_dicCharTextures.ContainsKey(charImg[i]))
                    {
                        string texPath = string.Format("{0}.png", logParam.Value3);
                        Texture2D tex = ResourceMgr.Instance.LoadFromAssetBundle("icon", texPath) as Texture2D;
                        m_dicCharTextures.Add(logParam.Value3, tex);
                    }

                    m_sendCharTextures.Add(m_dicCharTextures[charImg[i]]);
                }

                slot.SetText(m_sendCharTextures, logParam.Value2, logParam.BundlePath, voice, true);
            }
        } while (false);
    }

    void AudioSpectrumData()
    {
        float f = kMainTalk.kPlayerTalk.AudioSpectrumData();

        if (m_MainUnit.aniEvent.aniFace == null)
            return;

        m_MainUnit.aniEvent.aniFace.SetLayerWeight(1, Mathf.Lerp(m_MainUnit.aniEvent.aniFace.GetLayerWeight(1), Mathf.Clamp(f * 10, 0, 1), 0.4f));
    }
}
