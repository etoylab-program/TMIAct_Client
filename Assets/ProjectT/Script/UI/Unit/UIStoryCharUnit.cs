using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryCharUnit
{
    public static UIStoryCharUnit CreateStoryUnit(UIStroyPopup popup, Transform parent)
    {
        GameObject obj = new GameObject();
        obj.transform.parent = parent;
        obj.transform.localScale = Vector3.one;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localPosition = Vector3.zero;

        UIStoryCharUnit uIStoryCharUnit = obj.AddComponent<UIStoryCharUnit>();
        uIStoryCharUnit.Init(popup);
        return uIStoryCharUnit;
    }

    public static UIStoryCharUnit CreateStoryUnit(UIStroyPopup popup, Transform parent, string path, string faceAnimatorPath, string yAxis = "")
    {
        GameObject obj = new GameObject();
        obj.transform.parent = parent;
        obj.transform.localScale = Vector3.one;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localPosition = Vector3.zero;
        float localY = 0f;
        if (!string.IsNullOrEmpty(yAxis))
        {
            yAxis = yAxis.Replace(" ", "");
            localY = Utility.SafeParse(yAxis);
        }

        UIStoryCharUnit uIStoryCharUnit = obj.AddComponent<UIStoryCharUnit>();
        uIStoryCharUnit.InitLoadChar(popup, path, faceAnimatorPath, localY);
        return uIStoryCharUnit;
    }
}

public class UIStoryCharUnit : MonoBehaviour
{
    public enum eStoryCharUnitState
    {
        Talk,
        Listen,
        Stand,
        None
    }

    public bool TalkChar;
    private TweenPosition kPosition;
    private TweenRotation kRotation;
    private TweenScale kScale;
    public Transform kRoot;
    private UIStroyPopup _stroypopup;
    private StroyCharUnit _stroycharunit;
    public bool IsCharUnit { get { return _stroycharunit == null ? false : true; } }

    private FigureUnit m_FigureUnit = null;
    private Light m_FigureLight;
    private float m_LocalZaxisPos = 0f;
    private float m_localYaxisPos = 0f;

    private eAnimation m_endAni;
    
    //TestCode
    private AudioSource m_VoicePlayer;
    private float[] samples = new float[128];
    float temp = 0;

    //CharRenderQ
    private const int m_CharRenderQ = 3002;
    private float m_moveToPos = 0f;

    //Weapon
    private Dictionary<string, AttachObject> m_weaponDic = new Dictionary<string, AttachObject>();

    private Vector3 m_defaultPos = Vector3.zero;

    private eStoryCharUnitState m_unitState = eStoryCharUnitState.None;
    public eStoryCharUnitState UnitState { get { return m_unitState; } }

    private bool m_textOnly = false;
    private Coroutine m_textOnlyCoroutine = null;

    private const float m_defaultMovePos = -100f;

    private bool m_charAlphaing = false;
    private Coroutine m_charAlphaCoroutine = null;
    private bool m_alphaFlag = false;
    private bool m_alphaEndMove = false;

    private Coroutine m_shakeCoroutine = null;
    private Vector3 m_shakeOriginPos;
    private bool m_bisShake = false;
    private float m_shakeAmount = 1f;

    private float m_defaultZ = 600f;

    public void Init(UIStroyPopup popup)
    {
        _stroypopup = popup;
        GameObject root = new GameObject("Root");
        kPosition = root.AddComponent<TweenPosition>();
        kPosition.enabled = false;
        kRotation = root.AddComponent<TweenRotation>();
        kScale = root.AddComponent<TweenScale>();

        root.transform.parent = this.transform;
        root.transform.localScale = Vector3.one;
        root.transform.localRotation = Quaternion.identity;
        root.transform.localPosition = new Vector3(1280f * 2f, m_localYaxisPos, 0);

        m_defaultPos = root.transform.localPosition;

        kRoot = root.transform;

        m_VoicePlayer = this.gameObject.AddComponent<AudioSource>();
        m_VoicePlayer.playOnAwake = false;
        m_VoicePlayer.outputAudioMixerGroup = SoundManager.Instance.GetAudioMixer(SoundManager.eSoundType.Voice);

        //DestroyChar();
    }

    public void InitLoadChar(UIStroyPopup popup, string path, string faceAnimatorPath, float yAxis = 0f)
    {
        m_localYaxisPos = yAxis;
        //m_localYaxisPos = (-Screen.height * 0.5f) - yAxis;

        float width = Screen.width;
        float height = Screen.width * 16 / 9;

        Vector2 screen = NGUITools.screenSize;
        float aspect = screen.x / screen.y;
        float initialAspect = (float)1280 / 720;
        float activeHeight = Mathf.RoundToInt(1280 / aspect);

        m_localYaxisPos -= 600;

        Init(popup);
        LoadChar(path, faceAnimatorPath);
    }

    public void SetRootPosY(float y)
    {
        this.transform.localPosition = new Vector3(0, y, 0);
    }

    public void ReSetPosistion()
    {
        kRoot.transform.localPosition = m_defaultPos;
    }

    public void DestroyChar()
    {
    }

    public bool LoadChar(string path, string faceAnimatorPath)
    {
        if (_stroycharunit != null)
            DestroyImmediate(_stroycharunit.gameObject);

        m_moveToPos = m_defaultMovePos;

        m_FigureUnit = GameSupport.CreateFigure(path, faceAnimatorPath);
        m_FigureUnit.aniEvent.PlayAni(eAnimation.Idle02);

        m_FigureUnit.transform.parent = kRoot;
        m_FigureUnit.transform.localRotation = new Quaternion(0, 180, 0, 1);
        m_FigureUnit.transform.localScale = new Vector3(700f, 700f, 700f);
        m_FigureUnit.transform.localPosition = Vector3.zero;
        m_FigureUnit.gameObject.layer = 5;
        m_FigureUnit.transform.SetChildLayer(5);

        m_FigureUnit.GetBones();

        if (m_FigureUnit.aniEvent.ListMtrl != null)
        {
            for (int i = 0; i < m_FigureUnit.aniEvent.ListMtrl.Count; i++)
            {
                if (m_FigureUnit.aniEvent.ListMtrl[i] != null)
                {
                    m_FigureUnit.aniEvent.ListMtrl[i].renderQueue = m_CharRenderQ;
                }
            }
        }

        for (int i = 0; i < m_FigureUnit.aniEvent.ListMtrl.Count; i++)
        {
            if (m_FigureUnit.aniEvent.ListMtrl[i] != null)
            {
                if (m_FigureUnit.aniEvent.ListMtrl[i].HasProperty("_Cutoff"))
                {
					m_FigureUnit.aniEvent.ListMtrl[i].SetFloat("_Cutoff", 0.1f);// 0.49f);
                }
            }
        }

        m_FigureLight = m_FigureUnit.transform.GetComponentInChildren<Light>(true);
        if (m_FigureLight != null)
        {
            m_FigureLight.gameObject.SetActive(false);
            //m_FigureLight.cullingMask = 1 << 5;
        }


        DynamicBone[] dynamicBones = m_FigureUnit.aniEvent.GetComponents<DynamicBone>();
        if (dynamicBones != null || dynamicBones.Length > 0)
        {
            foreach (DynamicBone dynamicBone in dynamicBones)
            {
                if (dynamicBone == null)
                    continue;
                if (dynamicBone.m_Root == null)
                {
                    Log.Show(m_FigureUnit.name + " / m_Root is NULL", Log.ColorType.Red);
                    continue;
                }
                dynamicBone.enabled = false;
            }
        }

        m_FigureUnit.aniEvent.SetShaderAlpha("_PartsColorR", 0f);
        m_FigureUnit.aniEvent.SetShaderAlpha("_PartsColorG", 0f);
        m_FigureUnit.aniEvent.SetShaderAlpha("_PartsColorB", 0f);
        //m_FigureUnit.aniEvent.GetBones();

        return true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (m_VoicePlayer.clip == null)
            {
                m_VoicePlayer.Play();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            PlayShakeChar();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            StopShakeChar();

        if(!m_textOnly)
            AudioSpectrumData();
    }

    public void PlayTextOnly(FLabelTextShow scrollText, float mouthTime, float mouthHeight)
    {
        m_textOnly = true;
        if (m_textOnlyCoroutine != null)
            StopCoroutine(m_textOnlyCoroutine);

        if(m_FigureUnit.aniEvent.aniFace != null)
            m_textOnlyCoroutine = StartCoroutine(TextOnlyAnimation(scrollText, mouthTime, mouthHeight));
    }

    public void StopTextOnly()
    {
        if (m_textOnlyCoroutine != null)
            StopCoroutine(m_textOnlyCoroutine);

        if (!m_textOnly)
            return;

        m_textOnly = false;

        if (m_FigureUnit.aniEvent.aniFace != null)
            m_FigureUnit.aniEvent.aniFace.SetLayerWeight(1, 0f);
    }

    IEnumerator TextOnlyAnimation(FLabelTextShow scrollText, float mouthTime, float mouthHeight)
    {
        while(m_textOnly && scrollText.TextScroll)
        {
            m_FigureUnit.aniEvent.aniFace.SetLayerWeight(1, UnityEngine.Random.Range(0.1f, mouthHeight));
            yield return new WaitForSeconds(mouthTime);
            m_FigureUnit.aniEvent.aniFace.SetLayerWeight(1, 0f);
            yield return new WaitForSeconds(mouthTime);
        }

        StopTextOnly();
    }

    public void SetAnimation(string bodyAni, string faceAni, string faceAniWeight = "1")
    {
        float faceWeight = 0f;
        if (!float.TryParse(faceAniWeight, out faceWeight))
        {
            //Debug.LogError("Value5 FaceAnimation Weight가 float형이 아닙니다. 0으로 초기화 합니다.");
            faceWeight = 0f;
        }


        if (!string.IsNullOrEmpty(bodyAni))
        {
            CancelInvoke("SetDefaultAnimation");
            bodyAni = bodyAni.Replace(" ", "");
			string[] anims = Utility.Split(bodyAni, ','); //bodyAni.Split(',');
			if (anims.Length == 1)
            {
                float delay = m_FigureUnit.PlayAni((eAnimation)System.Enum.Parse(typeof(eAnimation), anims[0]));
                m_endAni = eAnimation.None;
                Invoke("SetDefaultAnimation", delay + 0.1f);
            }
            else if(anims.Length == 2)
            {
                
                float delay = m_FigureUnit.PlayAni((eAnimation)System.Enum.Parse(typeof(eAnimation), anims[0]));
                if (anims[1].Equals("NONE"))
                {
                    m_endAni = eAnimation.None;
                }
                else
                {
                    m_endAni = (eAnimation)System.Enum.Parse(typeof(eAnimation), anims[1]);
                    Invoke("SetDefaultAnimation", delay);
                }
            }

        }
        if (!string.IsNullOrEmpty(faceAni))
        {
            faceAni = faceAni.Replace(" ", "");
			string[] faceAnims = Utility.Split(faceAni, ','); //faceAni.Split(',');
			if (m_FigureUnit.aniEvent.aniFace == null)
                return;
            for (int i = 0; i < faceAnims.Length; i++)
            {
                if (!string.IsNullOrEmpty(faceAnims[i]))
                {
                    if (!faceAnims[i].Equals("None"))
                    {
                        if(i == (int)eCOUNT.NONE)
                            m_FigureUnit.PlayFaceAni((eFaceAnimation)System.Enum.Parse(typeof(eFaceAnimation), faceAnims[i]), i);
                        if (i == 1)
                            m_FigureUnit.aniEvent.aniFace.SetLayerWeight(i, faceWeight);
                    }

                }
            }
        }
    }

    void SetDefaultAnimation()
    {
        if(m_endAni == eAnimation.None)
        {
            eAniPlayingState state = m_FigureUnit.aniEvent.IsAniPlaying(m_FigureUnit.aniEvent.curAniType);
            Debug.Log("Anim State : " + state + " / " + m_FigureUnit.aniEvent.curAniType);
            if(state == eAniPlayingState.End)
                m_FigureUnit.PlayAni(eAnimation.Idle02);
        }
        else
            m_FigureUnit.PlayAni(m_endAni);
    }

    public void SetShyFace(bool b)
    {
        if (m_FigureUnit.aniEvent.shyFaceMeshObj != null)
        {
            m_FigureUnit.aniEvent.shyFaceMeshObj.SetActive(b);
        }
    }

    public void SetPos(float pos)
    {
        kPosition.enabled = false;
        Vector3 vs = GetMovePosition(pos);
        vs.z = m_defaultZ;
        vs.y = m_localYaxisPos;
        kRoot.transform.localPosition = vs;
        m_moveToPos = pos;
        
    }

    public void SetZAxis(float pos)
    {
        if (m_charAlphaing)
        {
            if (m_charAlphaCoroutine != null)
                StopCoroutine(m_charAlphaCoroutine);
            m_charAlphaing = false;
            AlphaCoroutineEnded();
        }
        //SetPos(m_moveToPos);

        //if (m_moveToPos > m_defaultMovePos)
        //{

        //    m_moveToPos = m_defaultMovePos;
        //}
        Vector3 localPos = kRoot.transform.localPosition;
        m_LocalZaxisPos = pos * 300f;
        localPos.z = m_LocalZaxisPos;
        kRoot.transform.localPosition = localPos;
    }

    public void MovePos(float toPos, float duration, bool bNoneZaxis = false)
    {
        Vector3 vs = kRoot.transform.localPosition;
        vs.z = m_defaultZ;
        vs.y = m_localYaxisPos;
        Vector3 ve = GetMovePosition(toPos);
        ve.z = m_defaultZ;
        ve.y = m_localYaxisPos;

        if(bNoneZaxis)
        {
            vs.z = kRoot.transform.localPosition.z;
            ve.z = kRoot.transform.localPosition.z;
        }

        TweenPosition.SetTweenPosition(kPosition, vs, ve, duration, 0.0f, null);

        m_moveToPos = toPos;
    }

    public void SetPos(int pos, float duration)
    {
        Vector3 vs = _stroypopup.kCharPosList[pos].transform.localPosition;
        vs.z = m_defaultZ;
        vs.y = m_localYaxisPos;
        kRoot.localPosition = vs;
    }

    public void FadeIn(float duration)
    {
        if (_stroycharunit == null)
            return;
        _stroycharunit.FadeIn(duration);
    }

    public void FadeOut(float duration)
    {
        if (_stroycharunit == null)
            return;
        if (duration <= 0f)
            duration = 0.4f;
        _stroycharunit.FadeOut(duration);
    }

    public void Talk(float duration = 0.2f)        //말하기
    {
        
        //StopAllCoroutines();
        m_unitState = eStoryCharUnitState.Talk;
        //StopCoroutine("SetCharAlphaCoroutine");


        //m_FigureUnit.aniEvent.SetShaderColor("_Color", Color.white);
        m_FigureUnit.aniEvent.SetShaderFloat("_Value", 1f);

        m_FigureUnit.aniEvent.RestoreOriginalColor();

        for (int i = 0; i < m_FigureUnit.aniEvent.ListMtrl.Count; i++)
        {
            if (m_FigureUnit.aniEvent.ListMtrl[i].HasProperty("_OutlineColor"))
            {
                Color colorOutLine = m_FigureUnit.aniEvent.ListMtrl[i].GetColor("_OutlineColor");
                if (colorOutLine == null)
                    continue;
                colorOutLine.a = 0.5f;      //현규님 요청사항
                m_FigureUnit.aniEvent.ListMtrl[i].SetColor("_OutlineColor", colorOutLine);
            }
        }

        
    }

    public void Listen(float duration = 0.2f)      //듣기
    {
        
        //StopAllCoroutines();
        m_unitState = eStoryCharUnitState.Listen;
        //TweenScale.SetTweenScale(kScale, UITweener.Style.Once, kScale.value, new Vector3(1.0f, 1.0f, 1.0f), duration, 0.0f, null);
        //StopCoroutine("SetCharAlphaCoroutine");

        m_FigureUnit.aniEvent.SetShaderColor("_Color", new Color(0.5f, 0.5f, 0.5f, 1));
        m_FigureUnit.aniEvent.SetShaderFloat("_Value", 0.5f);

        m_FigureUnit.aniEvent.SetShaderColor( "_1st_ShadeColor", Color.gray );
        m_FigureUnit.aniEvent.SetShaderColor( "_2nd_ShadeColor", Color.gray );

        //m_FigureUnit.aniEvent.aniFace.SetLayerWeight(1, 0f);
        for (int i = 0; i < m_FigureUnit.aniEvent.ListMtrl.Count; i++)
        {
            if (m_FigureUnit.aniEvent.ListMtrl[i].HasProperty("_OutlineColor"))
            {
                Color colorOutLine = m_FigureUnit.aniEvent.ListMtrl[i].GetColor("_OutlineColor");
                if (colorOutLine == null)
                    continue;
                colorOutLine.a = 0.5f;      //현규님 요청사항
                m_FigureUnit.aniEvent.ListMtrl[i].SetColor("_OutlineColor", colorOutLine);
            }
        }

        
    }

    public void Stand(float duration = 0.2f)       //대기
    {
        
        //StopAllCoroutines();
        m_unitState = eStoryCharUnitState.Stand;
        //StopCoroutine("SetCharAlphaCoroutine");

        //m_FigureUnit.aniEvent.SetShaderColor("_Color", Color.white);
        m_FigureUnit.aniEvent.SetShaderFloat("_Value", 1f);

        m_FigureUnit.aniEvent.RestoreOriginalColor();

        for (int i = 0; i < m_FigureUnit.aniEvent.ListMtrl.Count; i++)
        {
            if (m_FigureUnit.aniEvent.ListMtrl[i].HasProperty("_OutlineColor"))
            {
                Color colorOutLine = m_FigureUnit.aniEvent.ListMtrl[i].GetColor("_OutlineColor");
                if (colorOutLine == null)
                    continue;
                colorOutLine.a = 0.5f;      //현규님 요청사항
                m_FigureUnit.aniEvent.ListMtrl[i].SetColor("_OutlineColor", colorOutLine);
            }
        }
        
    }

    public void UpdateSlot()    //Fill parameter if you need
    {
    }

    public void OnClick_Slot()
    {

    }
    public Vector3 GetMovePosition(float idx)
    {

        Vector3 resultPos = kRoot.transform.localPosition;

        //float tempMoveX = Screen.width / 5;
        float tempMoveX = 1280f / 5;

        resultPos.x = tempMoveX * idx;

        return resultPos;
    }

    IEnumerator SetColor(float duration)
    {
        float deltaTime = 0f;
        while (deltaTime < duration)
        {
            deltaTime += Time.deltaTime;

            yield return null;
        }
    }

    public void StopVoice()
    {
        m_VoicePlayer.Stop();
    }

    public float PlayVoice(string voiceBundleName, string voiceFileName)
    {
        float audioTime = 0f;
        if (!string.IsNullOrEmpty(voiceFileName))
        {
            m_VoicePlayer.clip = SoundManager.Instance.GetVoice(voiceBundleName.ToLower(), voiceBundleName + "/" + voiceFileName).clip;
            //m_VoicePlayer.volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.Voice, FSaveData.Instance.GetVoiceVolume()).Equals(0f) ? 0f : 1f;
            m_VoicePlayer.volume = SoundManager.Instance.GetVolume(SoundManager.eSoundType.Voice, FSaveData.Instance.GetVoiceVolume());
            m_VoicePlayer.Play();

            audioTime = m_VoicePlayer.clip.length;
        }

        return audioTime;
    }

    void AudioSpectrumData()
    {
        if (m_VoicePlayer.clip == null)
            return;
        if (m_FigureUnit.aniEvent.aniFace == null)
            return;
        if (m_VoicePlayer.isPlaying)
        {
            m_VoicePlayer.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);
            //float tempSample = samples[2] * 5f;
            //float tempSample
            
            m_FigureUnit.aniEvent.aniFace.SetLayerWeight(1, Mathf.Lerp(m_FigureUnit.aniEvent.aniFace.GetLayerWeight(1), Mathf.Clamp(Mathf.Max(samples) * 10, 0, 100), 0.4f));
            //temp = tempSample;
        }
        else
        {
            m_FigureUnit.aniEvent.aniFace.SetLayerWeight(1, Mathf.Lerp(m_FigureUnit.aniEvent.aniFace.GetLayerWeight(1), 0, 0.1f));
        }
    }

    public void SetCharAlpha(bool b, float duration)
    {
        if (m_FigureUnit.aniEvent.ListMtrl != null)
        {
            m_charAlphaCoroutine = StartCoroutine(SetCharAlphaCoroutine(b, duration));
        }
    }

    public void SetCharAlpha(bool b, float pos, float duration)
    {
        m_moveToPos = pos;
        
        if (m_FigureUnit.aniEvent.ListMtrl != null)
        {
            if (m_charAlphaing)
            {
                if (m_charAlphaCoroutine != null)
                    StopCoroutine(m_charAlphaCoroutine);
                m_charAlphaing = false;
                AlphaCoroutineEnded();
            }
            m_charAlphaCoroutine = StartCoroutine(SetCharAlphaCoroutine(b, duration, true));
        }
    }

    IEnumerator SetCharAlphaCoroutine(bool b, float duration, bool endMove = false)
    {
        m_charAlphaing = true;
        m_alphaFlag = b;
        m_alphaEndMove = endMove;
        float EndTime = Time.time + duration;
        float lerpValue = 0f;

        if (duration <= (int)eCOUNT.NONE)
        {
            if (endMove)
            {
                AlphaCoroutineEnded();
            }
            yield break;
        }

        while(m_charAlphaing)
        {
            lerpValue = Mathf.Lerp(0f, 1f, (EndTime - Time.time) / duration);
            Color color = new Color((b ? lerpValue : 1f - lerpValue), (b ? lerpValue : 1f - lerpValue), (b ? lerpValue : 1f - lerpValue), (b ? lerpValue : 1f - lerpValue));

            /*
            m_FigureUnit.aniEvent.SetShaderColor("_Color", Color.white);
            m_FigureUnit.aniEvent.SetShaderFloat("_Value", 1f);
            */

            /*
            color.a = (b ? lerpValue : 1f - lerpValue);
            m_FigureUnit.aniEvent.SetShaderColor("_Color", color);
            m_FigureUnit.aniEvent.SetShaderFloat("_Value", (b ? lerpValue : 1f - lerpValue));
            */
            for (int i = 0; i < m_FigureUnit.aniEvent.ListMtrl.Count; i++)
            {
                if (m_FigureUnit.aniEvent.ListMtrl[i] != null)
                {
                    if (m_FigureUnit.aniEvent.ListMtrl[i].HasProperty("_Color"))
                    {
                        color.a = (b ? lerpValue : 1f - lerpValue);
                        m_FigureUnit.aniEvent.ListMtrl[i].SetColor("_Color", color);
                    }
                    if (m_FigureUnit.aniEvent.ListMtrl[i].HasProperty("_OutlineColor"))
                    {
                        Color colorOutLine = m_FigureUnit.aniEvent.ListMtrl[i].GetColor("_OutlineColor");
                        if (colorOutLine == null)
                            continue;
                        colorOutLine.a = (b ? lerpValue : 0.5f - lerpValue);
                        m_FigureUnit.aniEvent.ListMtrl[i].SetColor("_OutlineColor", colorOutLine);
                    }
                    if (m_FigureUnit.aniEvent.ListMtrl[i].HasProperty("_Value"))
                    {
                        m_FigureUnit.aniEvent.ListMtrl[i].SetFloat("_Value", (b ? lerpValue : 1f - lerpValue));
                    }

                    m_FigureUnit.aniEvent.ListMtrl[i].renderQueue = m_CharRenderQ;
                }
            }

            if(EndTime < Time.time)
            {
                m_charAlphaing = false;
                if (endMove)
                {
                    AlphaCoroutineEnded();
                }
                break;
            }

            yield return null;
        }

        if (m_FigureUnit.aniEvent.ListMtrl != null)
        {
            
        }

        
    }

    void AlphaCoroutineEnded()
    {
        if(m_alphaEndMove)
            SetPos(m_moveToPos);

        Color color = new Color((m_alphaFlag ? 0f : 1f), (m_alphaFlag ? 0f : 1f), (m_alphaFlag ? 0f : 1f), (m_alphaFlag ? 0f : 1f));

        /*
        color.a = (m_alphaFlag ? 0f : 1f);
        m_FigureUnit.aniEvent.SetShaderColor("_Color", color);
        m_FigureUnit.aniEvent.SetShaderFloat("_Value", (m_alphaFlag ? 0f : 1f));
        */
        for (int i = 0; i < m_FigureUnit.aniEvent.ListMtrl.Count; i++)
        {
            if (m_FigureUnit.aniEvent.ListMtrl[i] != null)
            {
                if (m_FigureUnit.aniEvent.ListMtrl[i].HasProperty("_Color"))
                {
                    color.a = (m_alphaFlag ? 0f : 1f);
                    m_FigureUnit.aniEvent.ListMtrl[i].SetColor("_Color", color);
                }

				if ( m_FigureUnit.aniEvent.ListMtrl[i].HasProperty( "_BaseColor" ) ) {
					color.a = ( m_alphaFlag ? 0f : 1f );
					m_FigureUnit.aniEvent.ListMtrl[i].SetColor( "_BaseColor", color );
				}

                if (m_FigureUnit.aniEvent.ListMtrl[i].HasProperty("_OutlineColor"))
                {
                    Color colorOutLine = m_FigureUnit.aniEvent.ListMtrl[i].GetColor("_OutlineColor");
                    if (colorOutLine == null)
                        continue;
                    colorOutLine.a = (m_alphaFlag ? 0f : 0.5f);
                    m_FigureUnit.aniEvent.ListMtrl[i].SetColor("_OutlineColor", colorOutLine);
                }
                if (m_FigureUnit.aniEvent.ListMtrl[i].HasProperty("_Value"))
                {
                    m_FigureUnit.aniEvent.ListMtrl[i].SetFloat("_Value", (m_alphaFlag ? 0f : 1f));
                }

                m_FigureUnit.aniEvent.ListMtrl[i].renderQueue = m_CharRenderQ;
            }
        }
    }

    public void SetCharRotation(float yAngle, float duration)
    {
        TweenRotation.SetTweenRotation(kRotation, UITweener.Style.Once, kRoot.transform.localRotation.eulerAngles, new Vector3(0, yAngle, 0), duration, 0f, null);
    }

    public void SetCharLocalScale(float scale)
    {
        kRoot.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void ShowWeapon(string bundlePath)
    {
		//if (m_weapon == null)
		//{
		//    SetWeapon(bundlePath);
		//}
		//else
		//{
		//    bundlePath = bundlePath.Replace(" ", "");
		//    string[] path = bundlePath.Split('/');

		//    string tempWeaponName = path[path.Length - 1];

		//    if (m_weaponName.Equals(tempWeaponName))
		//    {
		//        m_weapon.gameObject.SetActive(true);
		//        return;
		//    }

		//    SetWeapon(bundlePath);
		//}

		if (m_weaponDic == null || m_weaponDic.Count <= 0)
        {
            SetWeapon(bundlePath);
        }
        else
        {
            bundlePath = bundlePath.Replace(" ", "");
			string[] path = Utility.Split(bundlePath, '/'); //bundlePath.Split('/');

			string tempWeaponName = path[path.Length - 1];

            if(m_weaponDic.ContainsKey(tempWeaponName))
            {
                m_weaponDic[tempWeaponName].gameObject.SetActive(true);
                return;
            }

            SetWeapon(bundlePath);
        }
	}

    public void SetWeapon(string bundlePath)
    {
        bundlePath = bundlePath.Replace(" ", "");
		string[] path = Utility.Split(bundlePath, '/'); //bundlePath.Split('/');

        string tempWeaponName = path[path.Length - 1];

        if (m_weaponDic.ContainsKey(tempWeaponName))
            return;

        GameObject weapon = ResourceMgr.Instance.CreateFromAssetBundle(path[0].ToLower(), bundlePath + ".prefab");
        if(weapon != null)
        {
            AttachObject attachWeapon = weapon.GetComponent<AttachObject>();
            Transform tr = m_FigureUnit.aniEvent.GetBoneByName(attachWeapon.kBoneName);

            weapon.transform.SetChildLayer(5);
            weapon.transform.parent = tr;
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            weapon.transform.localScale = Vector3.one;
            attachWeapon.ActiveEffect(false);
            m_FigureUnit.GetBones();

			if (!string.IsNullOrEmpty(m_FigureUnit.FigureChangeWeaponName) && m_FigureUnit.aniEvent)
			{
				weapon.name = m_FigureUnit.FigureChangeWeaponName;
				m_FigureUnit.aniEvent.Rebind();
			}

			m_weaponDic.Add(tempWeaponName, attachWeapon);
        }
    }

    public void HideWeapon()
    {
        //if (m_weapon == null)
        //    return;
        //m_weapon.gameObject.SetActive(false);

        if (m_weaponDic == null || m_weaponDic.Count <= 0)
            return;

        foreach(KeyValuePair<string, AttachObject> attachWeapon in m_weaponDic)
        {
            if(attachWeapon.Value != null)
                attachWeapon.Value.gameObject.SetActive(false);
        }
    }

    public void PlayShakeChar(float amount = 1f)
    {
        m_shakeAmount = amount;

        if(!m_bisShake)
        {
            m_bisShake = true;
            m_shakeCoroutine = StartCoroutine(ShakeObject());
        }
        
    }

    IEnumerator ShakeObject()
    {
        m_shakeOriginPos = this.transform.localPosition;
        float timer = 0;
        while (m_bisShake)
        {
            this.transform.localPosition = (Vector3)UnityEngine.Random.insideUnitCircle * m_shakeAmount + m_shakeOriginPos;

            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void StopShakeChar()
    {
        m_bisShake = false;
        if (m_shakeCoroutine != null)
            StopCoroutine(m_shakeCoroutine);
    }
}
