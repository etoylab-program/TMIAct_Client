using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyFacility : MonoBehaviour
{
    public int kTableID;
    public Transform kWaitPos;
    public Transform kPlayPos;
    public List<Transform> kPlayPosList;
    public Transform kFacilityCamera;
    private  Camera m_FacilityCamera;

    public List<GameObject> kFacilityObj;

    public List<GameObject> kEffects;
    private List<FacilityData> _facilitydata = new List<FacilityData>();

    private LayerMask m_OriginLayerMask;
    private Vector3 m_OriginObjectPos;

    RenderTexture m_RenderTexture;
    public RenderTexture GetRenderTexture
    {
        get { return m_RenderTexture; }
    }

    private List<GameObject> m_UpgreadEffects = new List<GameObject>();

    //List<LobbyPlayer> m_Unit = new List<LobbyPlayer>();
    private Dictionary<int, LobbyPlayer> _units = new Dictionary<int, LobbyPlayer>();
    LobbyPlayer m_Unit;
    private float m_AniDelay = 0;
    bool m_EffPlaying = false;
    long m_UnitID = 0;

    private AudioSource m_activeAudioSource;
    private AudioClip m_activeAudioClip;
    private AudioClip m_levelupAudioClip;

    public Coroutine kUpgreadEffectFlag = null;

    private void OnEnable()
    {
        if (m_FacilityCamera == null)
            m_FacilityCamera = kFacilityCamera.GetComponent<Camera>();

        m_EffPlaying = false;
        kUpgreadEffectFlag = null;
    }

    void Create_FacilityChar()
    {
        if(m_Unit != null)
        {
            DestroyImmediate(m_Unit.gameObject);
        }

        m_UnitID = _facilitydata[0].Selete;
        CharData chardata = GameInfo.Instance.GetCharData(m_UnitID);

        if (m_Unit == null)
        {
            m_Unit = GameSupport.CreateLobbyPlayer(chardata);
            m_Unit.transform.parent = this.transform;
            m_Unit.transform.localPosition = kPlayPos.localPosition;
            m_Unit.transform.localRotation = kPlayPos.localRotation;
            m_Unit.transform.localScale = kPlayPos.localScale;
            
            m_Unit.CharLight_On();

            m_Unit.costumeUnit.ShowObject(m_Unit.costumeUnit.Param.InGameOnly, false);
            m_Unit.costumeUnit.ShowObject(m_Unit.costumeUnit.Param.LobbyOnly, true);
        }
        System.TimeSpan diffTime = _facilitydata[0].RemainTime - GameSupport.GetCurrentServerTime();
        if(diffTime.Ticks > 0)      //진행중
        {
            if (_facilitydata[0].TableData.EffectType == "FAC_CHAR_EXP")
            {
                m_Unit.ShowShadow(false);
                m_AniDelay = m_Unit.PlayAniImmediate(eAnimation.Lobby_Facility_EXP, eFaceAnimation.Facility, 0);
            }
            else if (_facilitydata[0].TableData.EffectType == "FAC_CHAR_SP")
            {
                //m_AniDelay = m_Unit.PlayAniImmediate(eAnimation.Lobby_Facility_Skill, eFaceAnimation.Facility, 0);
                StartCoroutine(FacilityAni_Skill());
            }

            if (m_EffPlaying)
            {
                return;
            }
            m_EffPlaying = true;
            //if (_facilitydata[0].TableData.EffectType == "FAC_CHAR_SP")
            //    StartCoroutine(FacilityAni_Skill(m_AniDelay));
        }
        else  //완료
        {
            CompleteFacility();
        }
    }

    void Create_FacilityChars(int index)
    {
        _units.TryGetValue(index, out LobbyPlayer lobbyPlayer);

        if (_facilitydata.Count <= index)
        {
            return;
        }

        FacilityData data = _facilitydata[index];
        if (data == null)
        {
            return;
        }
        
        List<CharData> charDatas = GameInfo.Instance.CharList.FindAll(x => x.OperationRoomTID == data.TableID);
        if (charDatas.Count <= 0)
        {
            return;
        }

        CharData charData = null;
        if (lobbyPlayer == null)
        {
            charData = charDatas[Random.Range(0, charDatas.Count)];
        }
        else
        {
            charData = GameInfo.Instance.CharList.Find(x => x.CUID == lobbyPlayer.Uid);

            Delete_FacilityChars(index);
        }

        lobbyPlayer = GameSupport.CreateLobbyPlayer(charData);

        if (lobbyPlayer == null)
        {
            return;
        }
            
        Transform lpTransform = lobbyPlayer.transform;
        lpTransform.parent = this.transform;

        if (index < kPlayPosList.Count)
        {
            lpTransform.localPosition = kPlayPosList[index].localPosition;
            lpTransform.localRotation = kPlayPosList[index].localRotation;
            lpTransform.localScale = kPlayPosList[index].localScale;
        }
            
        lobbyPlayer.CharLight_On();
        eLayer layer = eLayer.Player + index;
        lobbyPlayer.CharLight.cullingMask = 1 << (int)layer;
        Utility.ChangeLayersRecursively(lobbyPlayer.transform, layer.ToString());
        
        lobbyPlayer.PlayAniImmediate(eAnimation.Lobby_Facility_Operations, eFaceAnimation.Facility, 0);

        if (lobbyPlayer.costumeUnit != null)
        {
            lobbyPlayer.costumeUnit.ShowObject(lobbyPlayer.costumeUnit.Param.InGameOnly, false);
            lobbyPlayer.costumeUnit.ShowObject(lobbyPlayer.costumeUnit.Param.LobbyOnly, true);
        }

        _units.Add(index, lobbyPlayer);
    }

    public void Delete_FacilityChars(int index)
    {
        if (_units.TryGetValue(index, out LobbyPlayer lobbyPlayer))
        {
            _units.Remove(index);
            Destroy(lobbyPlayer.gameObject);
        }
    }

    IEnumerator FacilityAni_Skill()
    {
        float animDelay = 0f;
        ParticleSystem[] particles = kEffects[0].GetComponentsInChildren<ParticleSystem>(true);
        while(this.gameObject.activeSelf)
        {
            animDelay = m_Unit.PlayAniImmediate(eAnimation.Lobby_Facility_Skill, eFaceAnimation.Facility, 0);

            m_activeAudioSource.volume = FSaveData.Instance.GetSEVolume();
            m_activeAudioSource.Play();

            Invoke("AcvieSndPlayInvoke", 2.1f);
            Invoke("AcvieSndPlayInvoke", 3.1f);
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Stop(true);
                particles[i].Play(true);
            }

            yield return new WaitForSeconds(animDelay);
        }
        
    }

    void AcvieSndPlayInvoke()
    {
        m_activeAudioSource.PlayOneShot(m_activeAudioClip);
    }

    private void SeVolumeChange(float volume)
    {
        m_activeAudioSource.volume = volume;
    }
    
    public void InitLobbyFacility(FacilityData facilitydata)
    {
        _facilitydata.Clear();

        if (m_activeAudioSource == null)
        {
            m_activeAudioSource = this.gameObject.AddComponent<AudioSource>();
            m_activeAudioSource.playOnAwake = false;
            m_activeAudioSource.loop = false;
            m_activeAudioSource.volume = FSaveData.Instance.GetSEVolume();
            m_activeAudioSource.outputAudioMixerGroup = SoundManager.Instance.GetAudioMixer(SoundManager.eSoundType.UI);
            
            SoundManager.Instance.AddSeVolumeChangeAction(SeVolumeChange);
        }

        bool bEffectOnlyOne = false;
        if (facilitydata.TableData.EffectType.Equals("FAC_CHAR_EXP"))
        {
            if (m_activeAudioClip == null)
            {
                m_activeAudioClip = ResourceMgr.Instance.LoadFromAssetBundle("sound", "Sound/UI/ui_facility01_loop.wav") as AudioClip;
                m_activeAudioSource.loop = true;
                m_activeAudioSource.clip = m_activeAudioClip;
            }
        }
        else if (facilitydata.TableData.EffectType.Equals("FAC_CHAR_SP"))
        {
            if(m_activeAudioClip == null)
            {
                m_activeAudioClip = ResourceMgr.Instance.LoadFromAssetBundle("sound", "Sound/UI/ui_facility02_laser.wav") as AudioClip;
                m_activeAudioSource.clip = m_activeAudioClip;
            }
        }
        else if (facilitydata.TableData.EffectType.Equals("FAC_WEAPON_EXP"))
        {
            if(m_activeAudioClip == null)
            {
                m_activeAudioClip = ResourceMgr.Instance.LoadFromAssetBundle("sound", "Sound/UI/ui_facility03_loop.wav") as AudioClip;
                m_activeAudioSource.loop = true;
                m_activeAudioSource.clip = m_activeAudioClip;
            }
        }
        else if (facilitydata.TableData.EffectType.Equals("FAC_ITEM_COMBINE"))
        {
            if(m_activeAudioClip == null)
            {
                m_activeAudioClip = ResourceMgr.Instance.LoadFromAssetBundle("sound", "Sound/UI/ui_facility04_laser.wav") as AudioClip;
                m_activeAudioSource.loop = true;
                m_activeAudioSource.clip = m_activeAudioClip;
            }
        }
        else if (facilitydata.TableData.EffectType.Equals("FAC_CARD_TRADE"))
        {
            if (m_activeAudioClip == null)
            {
                m_activeAudioClip = ResourceMgr.Instance.LoadFromAssetBundle("sound", "Sound/UI/ui_facility05_loop.wav") as AudioClip;
                m_activeAudioSource.loop = true;
                m_activeAudioSource.clip = m_activeAudioClip;
            }
        }
        else if (facilitydata.TableData.EffectType.Equals("FAC_OPERATION_ROOM"))
        {
            bEffectOnlyOne = true;
            if (m_activeAudioClip == null)
            {
                m_activeAudioClip = ResourceMgr.Instance.LoadFromAssetBundle("sound", "Sound/UI/ui_facility06_loop.wav") as AudioClip;
                m_activeAudioSource.loop = true;
                m_activeAudioSource.clip = m_activeAudioClip;
            }
        }

        List<FacilityData> facilitylist = GameInfo.Instance.FacilityList.FindAll(x => x.TableData.ParentsID == facilitydata.TableData.ParentsID);
        for (int i = 0; i < facilitylist.Count; i++)
        {
            _facilitydata.Add(facilitylist[i]);
        }
        
        bool bEffectEnable = false;
        for(int i = 0; i < _facilitydata.Count; i++)
        {
            if (_facilitydata[i].TableData.SlotOpenFacilityLv <= facilitydata.Level && facilitydata.Level > 0)
            {
                if (i < kFacilityObj.Count)
                {
                    kFacilityObj[i].SetActive(true);
                }

                if (bEffectOnlyOne)
                {
                    if (_facilitydata[i].Stats != (int) eFACILITYSTATS.WAIT)
                    {
                        bEffectEnable = true;
                        Create_FacilityChars(i);
                    }
                    else
                    {
                        Delete_FacilityChars(i);
                    }
                    continue;
                }
                
                if (_facilitydata[i].Stats == (int)eFACILITYSTATS.WAIT)
                {
                    DisableFacilityEffect(i);
                }
                else
                {
                    ActiveFacilityEffect(i);
                }
            }
            else
            {
                if (i < kFacilityObj.Count)
                {
                    kFacilityObj[i].SetActive(false);
                }

                if (bEffectOnlyOne)
                {
                    continue;
                }
                
                DisableFacilityEffect(i);
            }
        }

        if (bEffectOnlyOne)
        {
            if (bEffectEnable)
            {
                ActiveFacilityEffect((int)eFacilityEffect.ActivateLoop);
            }
            else
            {
                DisableFacilityEffect((int)eFacilityEffect.ActivateLoop);
            }
        }
    }
    public void SetAudioClip(string clipname)
    {
        if (m_activeAudioClip == null)
        {
            m_activeAudioClip = ResourceMgr.Instance.LoadFromAssetBundle("sound", clipname) as AudioClip;
            m_activeAudioSource.loop = true;
            m_activeAudioSource.clip = m_activeAudioClip;
            return;
        }

        m_activeAudioClip = ResourceMgr.Instance.LoadFromAssetBundle("sound", clipname) as AudioClip;
        m_activeAudioSource.loop = true;
        m_activeAudioSource.clip = m_activeAudioClip;
    }

    public void SetFacilityUpGrade(FacilityData facilitydata)
    {
        SetFacilityData(facilitydata);
        SoundManager.Instance.PlayUISnd(18);
        GameObject effObj = null;

        if(facilitydata.TableData.EffectType.Equals("FAC_CHAR_EXP"))
        {
            if(m_UpgreadEffects.Count <= 0)
            {
                effObj = ResourceMgr.Instance.CreateFromAssetBundle("effect", "Effect/Background/prf_fx_facility_01_levelup.prefab");
                if(effObj != null)
                {
                    effObj.transform.parent = kFacilityObj[0].transform.parent;
                    effObj.transform.localPosition = Vector3.zero;
                    effObj.transform.localRotation = Quaternion.identity;
                    m_UpgreadEffects.Add(effObj);
                }
            }
        }
        else if(facilitydata.TableData.EffectType.Equals("FAC_CHAR_SP"))
        {
            if (m_UpgreadEffects.Count <= 0)
            {
                effObj = ResourceMgr.Instance.CreateFromAssetBundle("effect", "Effect/Background/prf_fx_facility_02_levelup.prefab");
                if (effObj != null)
                {
                    effObj.transform.parent = kFacilityObj[0].transform.parent;
                    effObj.transform.localPosition = Vector3.zero;
                    effObj.transform.localRotation = Quaternion.identity;
                    m_UpgreadEffects.Add(effObj);
                }
            }
        }
        else if(facilitydata.TableData.EffectType.Equals("FAC_WEAPON_EXP"))
        {
            if (!m_UpgreadEffects.Count.Equals(kFacilityObj.Count))
            {
                for (int i = m_UpgreadEffects.Count; i < kFacilityObj.Count; i++)
                {
                    if (kFacilityObj[i].activeSelf)
                    {
                        effObj = ResourceMgr.Instance.CreateFromAssetBundle("effect", string.Format("Effect/Background/prf_fx_facility_03_levelup_0{0}.prefab", i + 1));
                        if (effObj != null)
                        {
                            effObj.transform.parent = kFacilityObj[i].transform.parent;
                            effObj.transform.localPosition = Vector3.zero;
                            effObj.transform.localRotation = Quaternion.identity;
                            m_UpgreadEffects.Add(effObj);
                        }
                    }
                }
            }
        }
        else if(facilitydata.TableData.EffectType.Equals("FAC_ITEM_COMBINE"))
        {
            if(!m_UpgreadEffects.Count.Equals(kFacilityObj.Count))
            {
                for(int i = m_UpgreadEffects.Count; i < kFacilityObj.Count; i++)
                {
                    if(kFacilityObj[i].activeSelf)
                    {
                        effObj = ResourceMgr.Instance.CreateFromAssetBundle("effect", string.Format("Effect/Background/prf_fx_facility_04_levelup_0{0}.prefab", i + 1));
                        if (effObj != null)
                        {
                            effObj.transform.parent = kFacilityObj[i].transform.parent;
                            effObj.transform.localPosition = Vector3.zero;
                            effObj.transform.localRotation = Quaternion.identity;
                            m_UpgreadEffects.Add(effObj);
                        }
                    }
                }
            }
        }
        else if (facilitydata.TableData.EffectType.Equals("FAC_CARD_TRADE"))
        {
            if (m_UpgreadEffects.Count <= 0)
            {
                effObj = ResourceMgr.Instance.CreateFromAssetBundle("effect", "Effect/Background/prf_fx_facility_05_levelup.prefab");
                if (effObj != null)
                {
                    effObj.transform.parent = kFacilityObj[0].transform.parent;
                    effObj.transform.localPosition = Vector3.zero;
                    effObj.transform.localRotation = Quaternion.identity;
                    m_UpgreadEffects.Add(effObj);
                }
            }
        }
        else if (facilitydata.TableData.EffectType.Equals("FAC_OPERATION_ROOM"))
        {
            if (m_UpgreadEffects.Count <= 0)
            {
                effObj = kEffects[(int)eFacilityEffect.Appear];
                if (effObj != null)
                {
                    m_UpgreadEffects.Add(effObj);
                }
            }
        }

        kUpgreadEffectFlag = StartCoroutine(UpgreadEffectFlag(facilitydata));
    }

    IEnumerator UpgreadEffectFlag(FacilityData facilitydata, float _time = 2f)
    {
        if(m_UpgreadEffects.Count <= 0)
        {
            FacilityActivation(facilitydata);

            yield break;
        }

        for(int i = 0; i < m_UpgreadEffects.Count; i++)
        {
            m_UpgreadEffects[i].SetActive(true);
        }

        if (facilitydata.TableData.EffectType.Equals("FAC_OPERATION_ROOM"))
        {
            _time = PlayOperationUpgradeSound();
        }

        yield return new WaitForSeconds(_time);
        
        if (facilitydata.TableData.EffectType.Equals("FAC_OPERATION_ROOM"))
        {
            SetFacilityActiveSound();
        }
        
        for (int i = 0; i < m_UpgreadEffects.Count; i++)
        {
            m_UpgreadEffects[i].SetActive(false);
        }
        FacilityActivation(facilitydata);

        kUpgreadEffectFlag = null;
    }
    
    public void FacilityActivation(FacilityData facilitydata, PktInfoFacilityUpgrade pktinfo = null)
    {
        Log.Show("FacilityActivation");
        SoundManager.Instance.PlayUISnd(17);

        SetFacilityData(facilitydata);
        m_UnitID = 0;
        GameObject effObj = null;
        m_activeAudioSource.Play();

        System.Action<int> ActionSetFacilityEffect = (idx) =>
        {
            if (facilitydata.Stats != (int)eFACILITYSTATS.WAIT)
                ActiveFacilityEffect(0);
            else
                DisableFacilityEffect(0);
        };

        System.Func<string, bool> Func_CreateEffect_Level_1 = (path) =>
        {
            effObj = ResourceMgr.Instance.CreateFromAssetBundle("effect", path);
            if (effObj != null)
            {
                effObj.transform.parent = kFacilityObj[0].transform.parent;
                effObj.transform.localPosition = Vector3.zero;
                effObj.transform.localRotation = Quaternion.identity;
                kUpgreadEffectFlag = StartCoroutine(SetActivationEffect(0, effObj));
                return true;
            }
            return false;
        };

        System.Func<string, int, bool> Func_CreateEffect_Level = (path, idx) =>
        {

            effObj = ResourceMgr.Instance.CreateFromAssetBundle("effect", string.Format(path, idx + 1));
            if (effObj != null)
            {
                effObj.transform.parent = kFacilityObj[idx].transform.parent;
                effObj.transform.localPosition = Vector3.zero;
                effObj.transform.localRotation = Quaternion.identity;
                kUpgreadEffectFlag = StartCoroutine(SetActivationEffect(idx, effObj));
                return true;
            }

            return false;
        };

        System.Action RenewalUI = () =>
        {
            if (LobbyUIManager.Instance.PanelType == ePANELTYPE.FACILITY)
                LobbyUIManager.Instance.ShowUI("FacilityPanel", true);
            else if (LobbyUIManager.Instance.PanelType == ePANELTYPE.FACILITYITEM)
                LobbyUIManager.Instance.ShowUI("FacilityItemPanel", true);

            LobbyUIManager.Instance.Renewal("TopPanel");
            LobbyUIManager.Instance.Renewal("GoodsPopup");
            LobbyUIManager.Instance.Renewal("FacilityPanel");
            LobbyUIManager.Instance.Renewal("FacilityItemPanel");
        };

        if (facilitydata.TableData.EffectType.Equals("FAC_CHAR_EXP"))
        {   
            ActionSetFacilityEffect(0);
            if (facilitydata.Level.Equals(1))
            {
                if (Func_CreateEffect_Level_1("Effect/Background/prf_fx_facility_01_appear.prefab")) 
                    return;
            }

            if (facilitydata.Level > 1)
            {
                LobbyUIManager.Instance.ShowUI("TopPanel", false);
                RenewalUI();
                return;
            }
        }
        else if (facilitydata.TableData.EffectType.Equals("FAC_CHAR_SP"))
        {   
            ActionSetFacilityEffect(0);
            if (facilitydata.Level.Equals(1))
            {
                if (Func_CreateEffect_Level_1("Effect/Background/prf_fx_facility_02_appear.prefab"))
                    return;
            }

            if (facilitydata.Level > 1)
            {
                LobbyUIManager.Instance.ShowUI("TopPanel", false);               
                RenewalUI();
                return;
            }
            
        }
        else if (facilitydata.TableData.EffectType.Equals("FAC_WEAPON_EXP"))
        {
            for (int i = 0; i < _facilitydata.Count; i++)
            {
                ActionSetFacilityEffect(i);
            }
            
            if (facilitydata.Level.Equals(1))
            {
                if (Func_CreateEffect_Level_1("Effect/Background/prf_fx_facility_03_appear_01.prefab"))
                    return;
            }
            else
            {
                for (int i = 1; i < _facilitydata.Count; i++)
                {
                    if (_facilitydata[i].TableData.SlotOpenFacilityLv == facilitydata.Level)
                    {
                        if (Func_CreateEffect_Level("Effect/Background/prf_fx_facility_03_appear_0{0}.prefab", i))
                            break;
                    }
                }
            }
        }
        else if (facilitydata.TableData.EffectType.Equals("FAC_ITEM_COMBINE"))
        {
            for (int i = 0; i < _facilitydata.Count; i++)
            {                
                ActionSetFacilityEffect(i);
            }

            if (facilitydata.Level.Equals(1))
            {
                if (Func_CreateEffect_Level_1("Effect/Background/prf_fx_facility_04_appear_01.prefab"))
                    return;
            }
            else
            {
                for (int i = 1; i < _facilitydata.Count; i++)
                {
                    if (_facilitydata[i].TableData.SlotOpenFacilityLv == facilitydata.Level)
                    {
                        if (Func_CreateEffect_Level("Effect/Background/prf_fx_facility_04_appear_0{0}.prefab", i))
                            break;
                    }
                }
            }
        }
        else if (facilitydata.TableData.EffectType.Equals("FAC_CARD_TRADE"))
        {
            ActionSetFacilityEffect(0);
            if (facilitydata.Level.Equals(1))
            {
                if (Func_CreateEffect_Level_1("Effect/Background/prf_fx_facility_05_appear.prefab"))
                    return;
            }

            if (facilitydata.Level > 1)
            {
                LobbyUIManager.Instance.ShowUI("TopPanel", false);
                RenewalUI();
                return;
            }
        }
        else if (facilitydata.TableData.EffectType.Equals("FAC_OPERATION_ROOM"))
        {
            ActionSetFacilityEffect(0);
            if (facilitydata.Level.Equals(1))
            {
                effObj = kEffects[(int)eFacilityEffect.Appear];
                if (effObj != null)
                {
                    effObj.SetActive(true);
                    kUpgreadEffectFlag = StartCoroutine(SetActivationEffect(0, effObj, operation: true));
                }
            }
        }

        if (effObj == null)
        {   
            RenewalUI();
            LobbyUIManager.Instance.ShowUI("TopPanel", false);
        }
    }

    IEnumerator SetActivationEffect(int objIdx, GameObject effObj, float fActiveTime = 2f, bool operation = false)
    {
        if (operation)
        {
            fActiveTime = PlayOperationUpgradeSound();
        }
        
        yield return new WaitForSeconds(fActiveTime * 0.5f);
        kFacilityObj[objIdx].SetActive(true);
        yield return new WaitForSeconds(fActiveTime * 0.5f);
        
        if (operation)
        {
            SetFacilityActiveSound();
        }
        
        effObj.SetActive(false);

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("FacilityPanel");
        LobbyUIManager.Instance.Renewal("FacilityItemPanel");

        LobbyUIManager.Instance.ShowUI("TopPanel", false);
        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.FACILITY)
            LobbyUIManager.Instance.ShowUI("FacilityPanel", true);
        else if (LobbyUIManager.Instance.PanelType == ePANELTYPE.FACILITYITEM)
            LobbyUIManager.Instance.ShowUI("FacilityItemPanel", true);

        kUpgreadEffectFlag = null;
    }

    void SetFacilityData(FacilityData facilitydata)
    {
        for (int i = 0; i < _facilitydata.Count; i++)
        {
            if (_facilitydata[i].TableID.Equals(facilitydata.TableID))
            {
                _facilitydata[i] = facilitydata;
                break;
            }
        }
    }

    int GetFacilityDataIdx(int tableId)
    {
        for (int i = 0; i < _facilitydata.Count; i++)
        {
            if (_facilitydata[i].TableID.Equals(tableId))
            {
                return i + 1;
            }
        }

        return 1;
    }

    private float PlayOperationUpgradeSound()
    {
        if (m_levelupAudioClip == null)
        {
            m_levelupAudioClip = ResourceMgr.Instance.LoadFromAssetBundle("sound", "Sound/UI/ui_facility06_laser.wav") as AudioClip;
            if (m_levelupAudioClip == null)
            {
                return 0;
            }
        }

        m_activeAudioSource.loop = false;
        m_activeAudioSource.clip = m_levelupAudioClip;
        m_activeAudioSource.Play();

        return m_activeAudioSource.clip.length;
    }

    private void SetFacilityActiveSound()
    {
        m_activeAudioSource.loop = true;
        m_activeAudioSource.clip = m_activeAudioClip;
    }

    public void ActiveFacilityEffect(int idx)
    {
        if (kEffects == null || kEffects.Count <= 0)
            return;
        m_activeAudioSource.Play();
        if (idx < kEffects.Count)
        {
            kEffects[idx].SetActive(true);
        }

        if (_facilitydata.Count <= idx) return;

        if (_facilitydata[idx].TableData.EffectType.Equals("FAC_CHAR_EXP") || _facilitydata[idx].TableData.EffectType.Equals("FAC_CHAR_SP"))
        {
            Create_FacilityChar();
        }        
    }

    public void DisableFacilityEffect()
    {
        kEffects[0].SetActive(false);
        m_activeAudioSource.Stop();
        if (m_Unit != null)
        {
            StopAllCoroutines();
            CancelInvoke();
            m_Unit.gameObject.SetActive(false);
        }
    }

    public void DisableFacilityEffect(int idx)
    {
        StopAllCoroutines();
        CancelInvoke();
        Log.Show("DisableFacilityEffect(int idx)");
        if (kEffects == null || kEffects.Count <= 0)
            return;
        if (idx < kEffects.Count)
        {
            kEffects[idx].SetActive(false);
        }

        bool actived = false;
        for(int i = 0; i < kEffects.Count; i++)
        {
            if (kEffects[i].activeSelf)
            {
                actived = true;
                break;
            }
        }

        if (!actived)
            m_activeAudioSource.Stop();
        
        if (m_Unit != null)
        {
            m_Unit.gameObject.SetActive(false);
        }
    }

    public void SetRenderTexture(UITexture renderTex, bool flag)
    {
        if(flag)
        {
            m_OriginLayerMask = m_FacilityCamera.cullingMask;
            m_FacilityCamera.cullingMask = 1 << 29;
            if(kFacilityObj.Count > 0)
            {
                kFacilityObj[0].layer = 29;
                kFacilityObj[0].transform.SetChildLayer(29);
            }
                

            if(m_RenderTexture == null)
                m_RenderTexture = new RenderTexture(renderTex.width, renderTex.height, renderTex.depth);
            m_FacilityCamera.targetTexture = m_RenderTexture;
            renderTex.alpha = 1f;
            m_FacilityCamera.depth = 3;
            renderTex.mainTexture = m_RenderTexture;
        }
        else
        {
            m_FacilityCamera.cullingMask = m_OriginLayerMask;
            if (kFacilityObj.Count > 0)
            {
                kFacilityObj[0].layer = 0;
                kFacilityObj[0].transform.SetChildLayer(0);
            }

            renderTex.alpha = 0.1f;
            m_FacilityCamera.targetTexture = null;
            m_FacilityCamera.depth = -1;
            
        }
    }

	public void CompleteFacility() {
		StopAllCoroutines();
		CancelInvoke();

        if( m_activeAudioSource.isPlaying ) {
            m_activeAudioSource.Stop();
        }

        if( kEffects == null || kEffects.Count <= 0 ) {
            return;
        }

		kEffects[0].SetActive( false );

		if( m_Unit != null ) {
			if( _facilitydata[0].TableData != null && _facilitydata[0].TableData.EffectType == "FAC_CHAR_EXP" ) {
				m_Unit.ShowShadow( false );
			}

			if( m_Unit.aniEvent.curAniType != eAnimation.Idle01 ) {
				m_Unit.PlayAniImmediate( eAnimation.Idle01 );
			}
		}
	}

	public void CompleteFacility(int tableId)
    {
        StopAllCoroutines();
        CancelInvoke();

        int index = _facilitydata.FindIndex(x => x.TableID == tableId);
        if (index < 0)
        {
            return;
        }
        
        if (_units.TryGetValue(index, out LobbyPlayer lobbyPlayer))
        {
            if (lobbyPlayer.aniEvent.curAniType != eAnimation.Idle01)
            {
                lobbyPlayer.PlayAniImmediate(eAnimation.Idle01);
            }
        }
    }
}
