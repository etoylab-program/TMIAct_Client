using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStoryBGUnit : FUnit
{
    public delegate void OnBGChangeEvent();

    public Color kBGColor;
    public UISprite kBGSpr;

    public UISprite kBGRotateSpr;
    public UISprite kBGSecondRotateSpr;
    public UISprite kBGAlphaSpr;
    public UISprite kBGMoveSpr;

    public UIPanel kRotationPanel;
    public UIPanel kMovePanel;

    public UITexture kCircleEyeTex;

    public List<UITexture> kBGList;
    public List<UITexture> kAddList;

    public List<UI2DSprite> kBGSpList;
    public List<UI2DSprite> kAddSpList;

    private bool _bg;
    private int _index = 0;
    private int _addIndex = 0;

    private int _indexSp = 0;
    private int _addIndexSp = 0;

    private string m_bundlePath;
    private string m_fileName;
    private string m_imgName;

    private Coroutine m_eyeCoroutine = null;
    private const float m_eyeCloseTime = 6f;
    private const float m_eyeOpenTime = 3f;

    private OnBGChangeEvent m_BGEvent;

    private bool m_widgetCallback = false;
    private bool m_blurState = false;
    private float m_blurValue = 0f;

	public void OnDisable() {
		CancelInvoke();
	}

    public void Init()
    {
        _index = 0;
        _addIndex = 0;
        _indexSp = 0;
        _addIndexSp = 0;
        for (int i = 0; i < kBGList.Count; i++)
        {
            kBGList[i].mainTexture = null;
            kBGList[i].alpha = 0f;
            kBGList[i].transform.localPosition = Vector3.zero;
        }
        _bg = false;

        for (int i = 0; i < kAddList.Count; i++)
        {
            kAddList[i].mainTexture = null;
            kAddList[i].alpha = 0f;
            kAddList[i].transform.localPosition = Vector3.zero;
        }

        for(int i = 0; i < kBGSpList.Count; i++)
        {
            kBGSpList[i].width = 1280;
            kBGSpList[i].height = 960;
            kBGSpList[i].sprite2D = null;
            kBGSpList[i].depth = 0;
            kBGSpList[i].alpha = 0f;
            kBGSpList[i].transform.localPosition = Vector3.zero;
        }

        for(int i = 0; i < kAddSpList.Count; i++)
        {
            kAddSpList[i].width = 1280;
            kAddSpList[i].height = 960;
            kAddSpList[i].sprite2D = null;
            kAddSpList[i].depth = 1;
            kAddSpList[i].alpha = 0f;
            kAddSpList[i].transform.localPosition = Vector3.zero;
        }

        m_blurState = false;
        m_blurValue = 0f;

        if (!m_widgetCallback)
        {
            m_widgetCallback = true;
            for (int i = 0; i < kBGList.Count; i++)
            {
                UIWidget widget = kBGList[i].GetComponent<UIWidget>();
                if (widget != null)
                {
                    widget.onRender += BlurState;
                }
            }

            for (int i = 0; i < kAddList.Count; i++)
            {
                UIWidget widget = kAddList[i].GetComponent<UIWidget>();
                if (widget != null)
                {
                    widget.onRender += BlurState;
                }
            }

            for (int i = 0; i < kBGSpList.Count; i++)
            {
                UIWidget widget = kBGSpList[i].GetComponent<UIWidget>();
                if (widget != null)
                {
                    widget.onRender += BlurState;
                }
            }

            for (int i = 0; i < kAddSpList.Count; i++)
            {
                UIWidget widget = kAddSpList[i].GetComponent<UIWidget>();
                if (widget != null)
                {
                    widget.onRender += BlurState;
                }
            }
        }

        if (kCircleEyeTex != null)
        {
            kCircleEyeTex.width = Screen.width;
            kCircleEyeTex.height = Screen.height;
            kCircleEyeTex.material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 0.0f));
            kCircleEyeTex.material.SetFloat("_Hardness", 5f);
            m_eyeCoroutine = null;
        }

        ResetPanelPos();
        //kBGSpr.color = new Color(kBGColor.r, kBGColor.g, kBGColor, 0.0f);
    }

    public void DisableTexture()
    {
        if(kBGList != null)
        {
            for (int i = 0; i < kBGList.Count; i++)
            {
                kBGList[i].alpha = 0f;
            }
        }

        if (kAddList != null)
        {
            for (int i = 0; i < kAddList.Count; i++)
            {
                kAddList[i].alpha = 0f;
            }
        }

        if (kBGSpList != null)
        {
            for (int i = 0; i < kBGSpList.Count; i++)
            {
                kBGSpList[i].alpha = 0f;
            }
        }

        if (kAddSpList != null)
        {
            for (int i = 0; i < kAddSpList.Count; i++)
            {
                kAddSpList[i].alpha = 0f;
            }
        }

        m_blurState = false;
        m_blurValue = 0f;
    }

    public void ResetPanelPos()
    {
        kRotationPanel.useSortingOrder = true;
        kRotationPanel.sortingOrder = 3005;
        kRotationPanel.transform.localPosition = new Vector3(0, 0, -this.transform.localPosition.z);

        kMovePanel.useSortingOrder = true;
        kMovePanel.sortingOrder = 3005;
        kMovePanel.transform.localPosition = new Vector3(0, 0, -this.transform.localPosition.z);
    }

    public void ShowBackColor(bool bshow)
    {
        if (bshow)
            TweenColor.SetTweenColor(kBGSpr.GetComponent<TweenColor>(), new Color(0.0f, 0.0f, 0.0f, 0.0f), kBGColor, 2.0f, 0.0f, null);
        else
            TweenColor.SetTweenColor(kBGSpr.GetComponent<TweenColor>(), kBGColor, new Color(0.0f, 0.0f, 0.0f, 0.0f), 2.0f, 0.0f, null);
    }


    public void FadeIn(string strbg, float duration, bool pixelPerfect = false)
    {
        kBGList[_index].mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("story", "Story/BG/" + strbg + ".png");
        if (pixelPerfect)
            kBGList[_index].MakePixelPerfect();
        TweenColor.SetTweenColor(kBGList[_index].GetComponent<TweenColor>(), new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
    }

    public void FadeInWithAddTex(string strbg, float duration)
    {
        kAddList[_addIndex].mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("story", "Story/BG/" + strbg + ".png");

        TweenColor.SetTweenColor(kAddList[_addIndex].GetComponent<TweenColor>(), new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
    }

    public void FadeOut(float duration)
    {
        TweenColor.SetTweenColor(kBGList[_index].GetComponent<TweenColor>(), kBGList[_index].color, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
        _index += 1;
        if (_index >= kBGList.Count)
            _index = 0;
    }

    public void FadeOutSprite(float duration)
    {
        TweenColor.SetTweenColor(kBGSpList[_indexSp].GetComponent<TweenColor>(), kBGSpList[_indexSp].color, new Color(1f, 1f, 1f, 0f), duration, 0f, null);
        _indexSp += 1;
        if (_indexSp >= kBGSpList.Count)
            _indexSp = 0;
    }

    public void FadeOutWithAddTex(float duration)
    {
        TweenColor.SetTweenColor(kAddList[_addIndex].GetComponent<TweenColor>(), kAddList[_addIndex].color, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
        _addIndex += 1;
        if (_addIndex >= kAddList.Count)
            _addIndex = 0;
    }

    public void FadeOutWithAddTexSprite(float duration)
    {
        TweenColor.SetTweenColor(kAddSpList[_addIndexSp].GetComponent<TweenColor>(), kAddSpList[_addIndexSp].color, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
        _addIndexSp += 1;
        if (_addIndexSp >= kAddSpList.Count)
            _addIndexSp = 0;
    }

    void SetTexture()
    {
        if (string.IsNullOrEmpty(m_imgName) == false)
        {
            return;
        }
        
        SetTexture(m_bundlePath, m_fileName);
    }

    public void SetTexture(string strbg)
    {
        kBGList[_index].mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("story", "Story/BG/" + strbg + ".png");
        kBGList[_index].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void SetTexture(string bundleName, string fileName, bool pixelPerfect = false)
    {
        if (kBGSpList[_indexSp].sprite2D != null)
        {
            kBGSpList[_indexSp].alpha = 0f;
            kBGSpList[_indexSp].sprite2D = null;
            kAddSpList[_addIndexSp].alpha = 0f;
            kAddSpList[_addIndexSp].sprite2D = null;
        }

        kBGList[_index].mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle(bundleName.ToLower(), fileName + ".png");
        if (pixelPerfect)
            kBGList[_index].MakePixelPerfect();
        kBGList[_index].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void SetSprite(string bundleName, string fileName, string imgName, bool pixelPerfect = false)
    {
        if (kBGList[_index].mainTexture != null)
        {
            kBGList[_index].alpha = 0f;
            kBGList[_index].mainTexture = null;
            kAddList[_addIndex].alpha = 0f;
            kAddList[_addIndex].mainTexture = null;
        }
        
        Object[] sprites = ResourceMgr.Instance.LoadFromAssetBundleWithSubObject(bundleName.ToLower(), fileName + ".png");
        Log.Show(imgName + " / " + fileName + " # " + sprites.Length, Log.ColorType.Yellow);
        for (int i = 0; i < sprites.Length; i++)
        {
            if(sprites[i].name.Equals(imgName))
            {
                kBGSpList[_indexSp].sprite2D = (Sprite)sprites[i];
                break;
            }
        }

        kBGSpList[_indexSp].color = new Color(1f, 1f, 1f, 1f);
    }

    public void SetAddSprite(string bundleName, string fileName, string imgName, bool pixelPerfect = false)
    {
        Object[] sprites = ResourceMgr.Instance.LoadFromAssetBundleWithSubObject(bundleName.ToLower(), fileName + ".png");

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].name.Equals(imgName))
            {
                kAddSpList[_addIndexSp].sprite2D = (Sprite)sprites[i];
                break;
            }
        }
    }

    void SetSprite()
    {
        if (string.IsNullOrEmpty(m_imgName))
        {
            return;
        }
        
        SetSprite(m_bundlePath, m_fileName, m_imgName);
    }

    void SetTextureWithAddTex(Vector3 pos)
    {
        SetTextureWithAddTex(m_bundlePath, m_fileName, pos);
    }

    public void SetTextureWithAddTex(string strbg)
    {
        kAddList[_addIndex].mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("story", "Story/BG/" + strbg + ".png");
        kAddList[_addIndex].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void SetTextureWithAddTex(string bundleName, string fileName, Vector3 pos)
    {
        for (int i = 0; i < kAddList.Count; i++)
            kAddList[i].transform.localPosition = pos;
        kAddList[_addIndex].mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle(bundleName.ToLower(), fileName + ".png");
        kAddList[_addIndex].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        kAddList[_addIndex].MakePixelPerfect();
    }

    void SetSpriteWithAddSprite(Vector3 pos)
    {
        SetSpriteWithAddSprite(m_bundlePath, m_fileName, m_imgName, pos);
    }

    public void SetSpriteWithAddSprite(string bundleName, string fileName, string imgName, Vector3 pos)
    {
        for (int i = 0; i < kAddList.Count; i++)
            kAddSpList[i].transform.localPosition = pos;
        SetAddSprite(bundleName, fileName, imgName);
        kAddSpList[_addIndexSp].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        kAddSpList[_addIndexSp].MakePixelPerfect();
    }

    public void FadeIn(string bundleName, string fileName, float duration, bool pixelPerfect = false)
    {
        kBGList[_index].mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle(bundleName.ToLower(), fileName + ".png");
        kBGList[_index].alpha = 0f;
        if (pixelPerfect)
            kBGList[_index].MakePixelPerfect();
        TweenColor.SetTweenColor(kBGList[_index].GetComponent<TweenColor>(), new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.01f, null);
    }

    public void FadeInSprite(string bundleName, string fileName, string imgName, float duration, bool pixelPerfect = false)
    {
        SetSprite(bundleName, fileName, imgName, pixelPerfect);
        if (pixelPerfect)
            kBGSpList[_indexSp].MakePixelPerfect();
        TweenColor.SetTweenColor(kBGSpList[_indexSp].GetComponent<TweenColor>(), new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
    }

    public void FadeInWithAddTex(string bundleName, string fileName, float duration, Vector3 pos)
    {
        SetTextureWithAddTex(bundleName, fileName, pos);
        kAddList[_addIndex].alpha = 0f;
        TweenColor.SetTweenColor(kAddList[_addIndex].GetComponent<TweenColor>(), new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
    }

    public void FadeInWithAddSprite(string bundleName, string fileName, string imgName, float duration, Vector3 pos)
    {
        SetSpriteWithAddSprite(bundleName, fileName, imgName, pos);
		kAddSpList[_addIndexSp].alpha = 0.0f;
		TweenColor.SetTweenColor(kAddSpList[_addIndexSp].GetComponent<TweenColor>(), new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
    }

    public void ChangeBG(string bundleName, string fileName, float duration, bool pixelPerfect = false)
    {
        FadeOut(duration);
        FadeIn(bundleName, fileName, duration, pixelPerfect);
    }

    public void ChangeBGSprite(string bundleNAme, string fileName, string imgName, float duration, bool pixelPerfect = false)
    {
        FadeOutSprite(duration);
        FadeInSprite(bundleNAme, fileName, imgName, duration, pixelPerfect);
    }

    public void ChangeBG(int type, string strbg, float duration, bool pixelPerfect = false)
    {
        FadeOut(duration);
        FadeIn(strbg, duration, pixelPerfect);
    }

    public void ChangeBG(string bundleName, string fileName, float duration, Vector3 pos)
    {
        for (int i = 0; i < kBGList.Count; i++)
            kBGList[i].transform.localPosition = pos;

        FadeOut(duration);
        FadeIn(bundleName, fileName, duration);
    }

    public void ChangeBGSprite(string bundleName, string fileName, string imgName, float duration, Vector3 pos)
    {
        for (int i = 0; i < kBGSpList.Count; i++)
            kBGSpList[i].transform.localPosition = pos;

        FadeOutSprite(duration);
        FadeInSprite(bundleName, fileName, imgName, duration);
    }

    public void ChangeBG(int type, string strbg, float duration, Vector3 pos)
    {
        for (int i = 0; i < kBGList.Count; i++)
            kBGList[i].transform.localPosition = pos;

        FadeOut(duration);
        FadeIn(strbg, duration);
    }

    public void SetBGColor(Color color, float duration)
    {
        if (duration > 0f)
            TweenColor.SetTweenColor(kBGList[_index].GetComponent<TweenColor>(), kBGList[_index].color, color, duration, 0f, null);
        else
            kBGList[_index].color = color;

        SetBGColorSprite(color, duration);
    }

    public void SetBGColorSprite(Color color, float duration)
    {
        if (duration > 0f)
        {
            TweenColor.SetTweenColor(kBGSpList[_indexSp].GetComponent<TweenColor>(), kBGSpList[_indexSp].color, color, duration, 0f, null);
            TweenColor.SetTweenColor(kAddSpList[_addIndexSp].GetComponent<TweenColor>(), kAddSpList[_addIndexSp].color, color, duration, 0f, null);
        }
        else
        {
            kBGSpList[_indexSp].color = color;
            kAddSpList[_addIndexSp].color = color;
        }
    }

    public void RotateChangeBG(string bundleName, string fileName, string imgName, Color color, bool reverse = false, float duration = 2f)
    {
        float waitTime = duration * 0.5f;

        kBGRotateSpr.color = color;
        kBGSecondRotateSpr.color = color;

        color.a = 0f;
        kBGAlphaSpr.color = color;

        kBGRotateSpr.invert = reverse;
        kBGSecondRotateSpr.invert = !reverse;

        Color centerColor = color;
        centerColor.a = 1f;

        AnimationCurve animAlpha = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f) });
        AnimationCurve animColor = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(waitTime, 1f), new Keyframe(duration, 0f) });

        TweenAlpha.SetTweenAlpha(kBGAlphaSpr.GetComponent<TweenAlpha>(), duration, 0f, 1f, animAlpha);
        TweenFill.SetFill(kBGRotateSpr.GetComponent<TweenFill>(), 0f, 1f, waitTime, 0f, null);
        TweenFill.SetFill(kBGSecondRotateSpr.GetComponent<TweenFill>(), 1f, 0f, waitTime, waitTime, null);
        TweenColor.SetTweenColor(kBGSecondRotateSpr.GetComponent<TweenColor>(), kBGSecondRotateSpr.color, color, duration, 0f, animColor);
        Invoke("DisableRotateSpr", waitTime);
        Invoke("BGEventDelay", waitTime);
        Invoke("DisableSecondRotateSpr", duration);

        m_bundlePath = bundleName;
        m_fileName = fileName;
        m_imgName = imgName;

        if (string.IsNullOrEmpty(imgName))
            Invoke("SetTexture", duration * 0.5f);
        else
            Invoke("SetSprite", duration * 0.5f);
    }

    public void RotateChangeBG(string bundleName, string fileName, string imgName, string color, bool reverse = false, float duration = 2f, OnBGChangeEvent bgEvent = null)
    {
        color = color.Replace(" ", "");
		string[] colors = Utility.Split(color, ','); //color.Split(',');
		m_BGEvent = bgEvent;
        if (colors.Length < 4)
        {
            Debug.LogError("색상 값이 잘못 들어갔습니다.");
            RotateChangeBG(bundleName, fileName, imgName, Color.white, reverse, duration);
        }
        else
        {
            Color col = new Color(Utility.SafeParse(colors[0]), Utility.SafeParse(colors[1]), Utility.SafeParse(colors[2]), 1f);
            RotateChangeBG(bundleName, fileName, imgName, col, reverse, duration);
        }
    }

    void DisableRotateSpr()
    {
        kBGRotateSpr.GetComponent<TweenFill>().enabled = false;
        kBGRotateSpr.fillAmount = 0f;
    }

    void DisableSecondRotateSpr()
    {
        kBGSecondRotateSpr.GetComponent<TweenFill>().enabled = false;
        kBGSecondRotateSpr.fillAmount = 0f;
        kBGSecondRotateSpr.alpha = 0f;
        kBGAlphaSpr.alpha = 0f;
    }

    public void MoveChangeBG(string moveDir, string color, string bundleName, string fileName, string imgName, float duration = 2f, OnBGChangeEvent bgEvent = null)
    {

        color = color.Replace(" ", "");
		string[] colors = Utility.Split(color, ','); //color.Split(',');
        m_BGEvent = bgEvent;
        if (colors.Length < 3)
        {
            Debug.LogError("색상 값이 잘못 들어갔습니다.");
            kBGMoveSpr.color = Color.white;
        }
        else
        {
            Color col = new Color(Utility.SafeParse(colors[0]), Utility.SafeParse(colors[1]), Utility.SafeParse(colors[2]), 1f);
            kBGMoveSpr.color = col;
        }

        if (moveDir.Equals("DOWN_UP") || moveDir.Equals("UP_DOWN"))
        {
            kBGMoveSpr.transform.localRotation = Quaternion.identity;
        }
        else if(moveDir.Equals("LEFT_RIGHT") || moveDir.Equals("RIGHT_LEFT"))
        {
            kBGMoveSpr.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        }
        else
        {
            Debug.LogError(moveDir + " / 방향 값 입력이 잘못되었습니다.");
            return;
        }
        kBGMoveSpr.width = Screen.width;
        kBGMoveSpr.height = Screen.height * 4;

        if (!kBGMoveSpr.gameObject.activeSelf)
            kBGMoveSpr.gameObject.SetActive(true);

        if(moveDir.Equals("DOWN_UP"))
            TweenPosition.SetTweenPosition(kBGMoveSpr.GetComponent<TweenPosition>(), new Vector3(0, -Screen.height * 4, 0), new Vector3(0, Screen.height * 4, 0), duration, 0, null);
        else if(moveDir.Equals("UP_DOWN"))
            TweenPosition.SetTweenPosition(kBGMoveSpr.GetComponent<TweenPosition>(), new Vector3(0, Screen.height * 4, 0), new Vector3(0, -Screen.height * 4, 0), duration, 0, null);
        else if(moveDir.Equals("LEFT_RIGHT"))
            TweenPosition.SetTweenPosition(kBGMoveSpr.GetComponent<TweenPosition>(), new Vector3(-Screen.width * 4, 0, 0), new Vector3(Screen.width * 4, 0, 0), duration, 0, null);
        else if(moveDir.Equals("RIGHT_LEFT"))
            TweenPosition.SetTweenPosition(kBGMoveSpr.GetComponent<TweenPosition>(), new Vector3(Screen.width * 4, 0, 0), new Vector3(-Screen.width * 4, 0, 0), duration, 0, null);

        m_bundlePath = bundleName;
        m_fileName = fileName;
        m_imgName = imgName;

        if (string.IsNullOrEmpty(imgName))
            Invoke("SetTexture", duration * 0.5f);
        else
            Invoke("SetSprite", duration * 0.5f);

        Invoke("BGEventDelay", duration * 0.5f);
    }

    public void ColorOutChangeBG(string bundleName, string fileName, string imgName, Color color, float duration = 2f)
    {
        color.a = 0f;
        kBGSpr.color = color;

        color.a = 1f;

        AnimationCurve animColor = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f) });
        TweenColor.SetTweenColor(kBGSpr.GetComponent<TweenColor>(), kBGSpr.color, color, duration, 0f, animColor);

        m_bundlePath = bundleName;
        m_fileName = fileName;
        m_imgName = imgName;
        if (string.IsNullOrEmpty(imgName))
            Invoke("SetTexture", duration * 0.5f);
        else
            Invoke("SetSprite", duration * 0.5f);
    }

    public void EyeChangeBG(string bundleName, string fileName, string imgName, float duration = 2f)
    {

        StopCoroutine("CircleEyeEffect");

        m_bundlePath = bundleName;
        m_fileName = fileName;
        m_imgName = imgName;
        kCircleEyeTex.color = Color.black;

        m_eyeCoroutine = StartCoroutine(CircleEyeEffect(duration));
    }

    IEnumerator CircleEyeEffect(float duration)
    {
        kCircleEyeTex.material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 1.0f));
        yield return StartCoroutine(CircleEyeEffect(false, duration * 2f));
        if (string.IsNullOrEmpty(m_imgName))
            SetTexture();
        else
            SetSprite();
        yield return null;
        yield return StartCoroutine(CircleEyeEffect(true, duration * 2f));

        kCircleEyeTex.material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 0.0f));
    }

    public void EyeChangeBG(string bundleName, string fileName, string imgName, bool eyeFlag,  float duration = 2f)
    {
        if (m_eyeCoroutine != null)
            StopCoroutine(m_eyeCoroutine);

        m_bundlePath = bundleName;
        m_fileName = fileName;
        m_imgName = imgName;
        kCircleEyeTex.color = Color.black;
        kCircleEyeTex.material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 1.0f));
        if (string.IsNullOrEmpty(imgName))
            SetTexture();
        else
            SetSprite();
        m_eyeCoroutine = StartCoroutine(CircleEyeEffect(eyeFlag, duration));
    }

    IEnumerator CircleEyeEffect(bool eyeFlag, float duration)
    {
        //float duration = (eyeFlag ? m_eyeCloseTime : m_eyeOpenTime);
        float endTime = Time.time + duration;

        float lerpValue = 0f;

        while(this.gameObject.activeSelf)
        {
            if(eyeFlag)
                lerpValue = Mathf.Lerp(1f, 0f, (endTime - Time.time) / duration);
            else
                lerpValue = Mathf.Lerp(0f, 1f, (endTime - Time.time) / duration);


            kCircleEyeTex.drawCall.dynamicMaterial.SetFloat("_Hardness", (eyeFlag ? lerpValue : lerpValue) * 5f);

            if(endTime < Time.time)
            {
                break;
            }

            yield return null;
        }

        kCircleEyeTex.material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 0.0f));
    }

    public void BGEventDelay()
    {
        if (m_BGEvent != null)
            m_BGEvent();

        m_BGEvent = null;
    }

    public void BlurOn(float shaderValue)
    {
        Log.Show("BlurOn");
        m_blurValue = shaderValue;
        m_blurState = true;
        
        
    }

    public void BlurOff()
    {
        Log.Show("BlurOff", Log.ColorType.Red);
        m_blurState = false;
        m_blurValue = 0f;
        
    }

    public void BlurState(Material mat)
    {
        if(m_blurState)
            mat.SetFloat("_Distance", m_blurValue);
        else
            mat.SetFloat("_Distance", 0f);
    }

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.E))
    //    {
    //        EyeChangeBG("favor", "Favor/char_1", "char_1_0");
    //    }
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        EyeChangeBG("favor", "Favor/char_1", "char_2_0");
    //    }
    //}
}
