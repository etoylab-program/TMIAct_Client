using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookCardUnit
{
    public static UIBookCardCinemaUnit CreateBookCardUnit(string unitName, string texPath, int depth , Transform parent = null)
    {
        GameObject obj = new GameObject(unitName);
        obj.transform.parent = parent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        UIBookCardCinemaUnit unit = obj.AddComponent<UIBookCardCinemaUnit>();
        unit.InitBookUnit(texPath, depth);

        return unit;
    }

    public static UIBookCardCinemaUnit CreateBookCardUnit(string unitName, string texPath, Transform parent = null)
    {
        GameObject obj = new GameObject(unitName);
        obj.transform.parent = parent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        UIBookCardCinemaUnit unit = obj.AddComponent<UIBookCardCinemaUnit>();
        unit.InitBookUnit(texPath, 2);

        return unit;
    }

    public static UIBookCardCinemaUnit CreateBookCardUnitWithSprite(string unitName, string texPath, string imgName, Transform parent = null)
    {
        GameObject obj = new GameObject(unitName);
        obj.transform.parent = parent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        UIBookCardCinemaUnit unit = obj.AddComponent<UIBookCardCinemaUnit>();
        unit.InitBookSpirteUnit(texPath, imgName, 2);

        return unit;
    }
}


public class UIBookCardCinemaUnit : MonoBehaviour
{
    public enum eBookUnitTex
    {
        BODY,
        FACE,
        NONE,
    }

    public enum eBookUnitPos
    {
        LeftOut = -5,
        Left = -2,
        CenterLeft = -1,
        Center = 0,
        CenterRight = 1,
        Right = 2,
        RightOut = 5,
    }

    class BookCardUnit
    {
        public UITexture tex;
        public TweenColor tweenColor;
    }

    class BookCardSpriteUnit
    {
        public UI2DSprite sprite;
        public TweenColor tweenColor;
    }

    bool bIsInit = false;
    public Vector3 kPosistion;
    public GameObject kUnitObj;
    private Dictionary<string, Texture> m_dicBodyTextures;
    private Dictionary<string, Texture> m_dicFaceTextures;
    private Dictionary<string, Sprite> m_dicBodySprites;
    private Dictionary<string, Sprite> m_dicFaceSprites;


    BookCardUnit m_bodyTextureSrc;
    BookCardUnit m_bodyTextureDest;

    BookCardUnit m_faceTextureSrc;
    BookCardUnit m_faceTextureDest;

    BookCardSpriteUnit m_bodySpriteSrc;
    BookCardSpriteUnit m_bodySpriteDest;

    BookCardSpriteUnit m_faceSpriteSrc;
    BookCardSpriteUnit m_faceSpriteDest;

    private int m_Depth = 2;

    private UIWidget m_widget = null;

    //private TweenScale m_tweenScale = null;
    private TweenPosition m_tweenPos = null;
    //CrossFade
    private const float m_DefailtFadeTime = 1f;
    private bool m_fadeing = false;
    private Coroutine m_fadeCoroutine = null;

    //ShakeValues
    private float m_shakeAmount = 1f;
    private Coroutine m_shakeCoroutine = null;
    private Vector3 m_shakeOriginPos;
    private bool m_bisShake = false;

    public void InitBookUnit(string filePath, int depath, eBookUnitTex target = eBookUnitTex.BODY)
    {
        m_Depth = depath;

        m_widget = this.gameObject.AddComponent<UIWidget>();
        //m_tweenScale = this.gameObject.AddComponent<TweenScale>();
        m_tweenPos = this.gameObject.AddComponent<TweenPosition>();

        kUnitObj = new GameObject(this.gameObject.name);
        kUnitObj.transform.parent = this.transform;
        kUnitObj.transform.localPosition = Vector3.zero;
        kUnitObj.transform.localScale = Vector3.one;

        InitUITextures(out m_bodyTextureSrc, depath, "BodyTex_0");
        InitUITextures(out m_bodyTextureDest, depath, "BodyTex_1");
        InitUITextures(out m_faceTextureSrc, depath + 1, "FaceTex_0");
        InitUITextures(out m_faceTextureDest, depath + 1, "FaceTex_1");

        SetBookUnitTexture(filePath, false, target);
    }

    public void InitBookSpirteUnit(string filePath, string imgName, int depath, eBookUnitTex target = eBookUnitTex.BODY)
    {
        m_Depth = depath;

        m_widget = this.gameObject.AddComponent<UIWidget>();
        //m_tweenScale = this.gameObject.AddComponent<TweenScale>();
        m_tweenPos = this.gameObject.AddComponent<TweenPosition>();

        kUnitObj = new GameObject(this.gameObject.name);
        kUnitObj.transform.parent = this.transform;
        kUnitObj.transform.localPosition = Vector3.zero;
        kUnitObj.transform.localScale = Vector3.one;

        InitUISprites(out m_bodySpriteSrc, depath, "BodySp_0");
        InitUISprites(out m_bodySpriteDest, depath, "BodySp_1");
        InitUISprites(out m_faceSpriteSrc, depath + 1, "FaceSp_0");
        InitUISprites(out m_faceSpriteDest, depath + 1, "FaceSp_1");

        SetBookUnitSprite(filePath, imgName, false);
    }

    public void UnitFadeInOut(bool fadeIn, float fadeTime)
    {
        if (m_widget == null)
            return;

        if(fadeIn && m_widget.alpha.Equals(1f))
            return;
        if (!fadeIn && m_widget.alpha.Equals(0f))
            return;
    }

    IEnumerator UnitFade(bool fadeInOut, float fadeTime)
    {
        float EndTime = Time.time + fadeTime;
        yield return null;
    }

    //전체 경로에서 파일이름만 떼주기
    string FilePathToFileName(string str)
    {
        str = str.Replace(" ", "");
		string[] splitStr = Utility.Split(str, '/'); //str.Split('/');

		return splitStr[splitStr.Length - 1];
    }

    //UITexture 생성 및 기본 설정
    void InitUITextures(out BookCardUnit uitexture, int depath, string objName)
    {
        GameObject obj = new GameObject();
        obj.name = objName;
        obj.transform.parent = kUnitObj.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        uitexture = new BookCardUnit();

        uitexture.tex = obj.AddComponent<UITexture>();
        uitexture.tex.depth = depath;
        uitexture.tex.enabled = false;

        uitexture.tweenColor = obj.AddComponent<TweenColor>();
    }

    void InitUISprites(out BookCardSpriteUnit uisprites, int depath, string objName)
    {
        GameObject obj = new GameObject();
        obj.name = objName;
        obj.transform.parent = kUnitObj.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        uisprites = new BookCardSpriteUnit();

        uisprites.sprite = obj.AddComponent<UI2DSprite>();
        uisprites.sprite.depth = depath;
        uisprites.sprite.enabled = false;

        uisprites.tweenColor = obj.AddComponent<TweenColor>();
    }

    //이미지 로드 및 추가
    Texture AddTexture(string filePath, eBookUnitTex target)
    {
        if (m_dicBodyTextures == null)
            m_dicBodyTextures = new Dictionary<string, Texture>();
        if (m_dicFaceTextures == null)
            m_dicFaceTextures = new Dictionary<string, Texture>();

        filePath = filePath.Replace(" ", "");
		string[] splitStr = Utility.Split(filePath, '/'); //filePath.Split('/');

		Dictionary<string, Texture> tempDic = (target == eBookUnitTex.FACE) ? m_dicFaceTextures : m_dicBodyTextures;
        string fileName = FilePathToFileName(filePath);

        if (tempDic.ContainsKey(fileName))
        {
            Debug.Log("Already Load Texture");
        }
        else
        {
            Texture loadTexture = LoadBookUnitTexture(filePath);
            tempDic.Add(fileName, loadTexture);
        }

        return tempDic[fileName];
    }

    Sprite AddSprite(string filePath, string imgName, eBookUnitTex target)
    {
        if (m_dicBodySprites == null)
            m_dicBodySprites = new Dictionary<string, Sprite>();
        if (m_dicFaceSprites == null)
            m_dicFaceSprites = new Dictionary<string, Sprite>();

        filePath = filePath.Replace(" ", "");
		string[] splitStr = Utility.Split(filePath, '/'); //filePath.Split('/');

		Dictionary<string, Sprite> tempDic = (target == eBookUnitTex.FACE) ? m_dicFaceSprites : m_dicBodySprites;
        //string fileName = //FilePathToFileName(filePath);

        if (tempDic.ContainsKey(imgName))
        {
            Debug.Log("Already Load Texture");
        }
        else
        {
            Sprite loadSprite = LoadBookUnitSprite(filePath, imgName);
            tempDic.Add(imgName, loadSprite);
        }

        return tempDic[imgName];
    }

    //Body, Face ... 중 어느 부위의 이미지를 바꿀지 오브젝트 반환
    BookCardUnit GetCurrentTaeget(eBookUnitTex target)
    {
        if (target == eBookUnitTex.FACE)
            return m_faceTextureSrc;
        else if (target == eBookUnitTex.BODY)
            return m_bodyTextureSrc;

        return m_bodyTextureSrc;
    }

    BookCardSpriteUnit GetCurrentTarget(eBookUnitTex target)
    {
        if (target == eBookUnitTex.FACE)
            return m_faceSpriteSrc;
        else if (target == eBookUnitTex.BODY)
            return m_bodySpriteSrc;

        return m_bodySpriteSrc;
    }

    //이미지 설정 하기
    public void SetBookUnitTexture(string filePath, bool fade = false, eBookUnitTex target = eBookUnitTex.BODY)
    {
        if (fade)
            CrossFadeUnitTexture(filePath, m_DefailtFadeTime, target);
        else
            ChangeUnitTexture(filePath, target);
    }

    public void SetBookUnitTexture(string filePath, float fadeTime, eBookUnitTex target = eBookUnitTex.BODY)
    {
        CrossFadeUnitTexture(filePath, fadeTime, target);
    }

    public void SetBookUnitSprite(string filePath, string imgName, bool fade = false, eBookUnitTex target = eBookUnitTex.BODY)
    {
        Debug.Log(filePath + " / " + imgName);
        if (fade)
            CrossFadeUnitSprite(filePath, imgName, m_DefailtFadeTime, target);
        else
            ChangeUnitSprite(filePath, imgName, target);
    }

    public void SetBookUnitSprite(string filePath, string imgName, float fadeTime, eBookUnitTex target = eBookUnitTex.BODY)
    {
        CrossFadeUnitSprite(filePath, imgName, fadeTime, target);
    }

    //이미지 바꾸기(등록 된 이미지가 없으면 추가)
    void ChangeUnitTexture(string filePath, eBookUnitTex target)
    {
        BookCardUnit changeTarget = null;
        switch(target)
        {
            case eBookUnitTex.FACE:
                changeTarget = m_faceTextureSrc;
                break;
            case eBookUnitTex.BODY:
                changeTarget = m_bodyTextureSrc;
                break;
        }
        
        if(changeTarget == null)
        {
            Debug.Log("UITexture is NULL!!");
            return;
        }
        
        if(changeTarget.tex.mainTexture != null && changeTarget.tex.mainTexture.name.Equals(FilePathToFileName(filePath)))
        {
            Debug.Log("Current Image!!");
            return;
        }
        changeTarget.tex.mainTexture = AddTexture(filePath, target);
        changeTarget.tex.MakePixelPerfect();
        changeTarget.tex.enabled = true;
    }

    void ChangeUnitSprite(string filePath, string imgName, eBookUnitTex target)
    {
        BookCardSpriteUnit changeTarget = null;
        switch(target)
        {
            case eBookUnitTex.FACE:
                changeTarget = m_faceSpriteSrc;
                break;
            case eBookUnitTex.BODY:
                changeTarget = m_bodySpriteSrc;
                break;
        }

        if(changeTarget == null)
        {
            Debug.LogError("UISprite is null");
            return;
        }

        if(changeTarget.sprite.sprite2D != null && changeTarget.sprite.sprite2D.name.Equals(imgName))
        {
            Debug.Log("Current Image!!");
            return;
        }

        changeTarget.sprite.sprite2D = AddSprite(filePath,imgName, target);
        changeTarget.sprite.MakePixelPerfect();
        changeTarget.sprite.enabled = true;
    }

    //미리 정의 해논 UITexture 1/2 를 크로스페이드, 마지막에는 설정한 이미지가 1에 보여짐(2는 fade용도로 사용후 disable)
    void CrossFadeUnitTexture(string filePath, float fadeTime, eBookUnitTex target)
    {
        string fileName = FilePathToFileName(filePath);

        BookCardUnit fadeInTarget = null;
        BookCardUnit fadeOutTarget = null;

        if(target == eBookUnitTex.FACE)
        {
            fadeInTarget = m_faceTextureSrc;
            fadeOutTarget = m_faceTextureDest;
        }
        else if(target == eBookUnitTex.BODY)
        {
            fadeInTarget = m_bodyTextureSrc;
            fadeOutTarget = m_bodyTextureDest;
        }

        if(fadeInTarget.tex.mainTexture.name.Equals(FilePathToFileName(filePath)))
        {
            Debug.Log("Current Image!!");
            return;
        }

        fadeOutTarget.tex.mainTexture = fadeInTarget.tex.mainTexture;
        fadeInTarget.tex.mainTexture = AddTexture(filePath, target);
        fadeInTarget.tex.alpha = 0f;
        fadeOutTarget.tex.alpha = 1f;
        fadeOutTarget.tex.enabled = true;

        fadeInTarget.tex.MakePixelPerfect();
        fadeOutTarget.tex.MakePixelPerfect();

        CrossFade(fadeTime, "", target);
        //m_fadeCoroutine = StartCoroutine(CrossFadeTexture(fadeInTarget.tex, fadeOutTarget.tex, fadeTime));
    }
    void CrossFadeUnitSprite(string filePath, string imgName, float fadeTime, eBookUnitTex target)
    {
        BookCardSpriteUnit fadeInTarget = null;
        BookCardSpriteUnit fadeOutTarget = null;

        if (target == eBookUnitTex.FACE)
        {
            fadeInTarget  = m_faceSpriteSrc;
            fadeOutTarget = m_faceSpriteDest;
        }
        else if (target == eBookUnitTex.BODY)
        {
            fadeInTarget = m_bodySpriteSrc;
            fadeOutTarget = m_bodySpriteDest;
        }

        if (fadeInTarget.sprite.sprite2D.name.Equals(imgName))
        {
            Debug.Log("Current Image!!");
            return;
        }

        fadeOutTarget.sprite.sprite2D = fadeInTarget.sprite.sprite2D;
        fadeInTarget.sprite.sprite2D = AddSprite(filePath, imgName, target);
        fadeInTarget.sprite.alpha = 0f;
        fadeOutTarget.sprite.alpha = 1f;
        fadeOutTarget.sprite.enabled = true;

        fadeInTarget.sprite.MakePixelPerfect();
        fadeOutTarget.sprite.MakePixelPerfect();

        CrossFade(fadeTime, imgName, target);
        //m_fadeCoroutine = StartCoroutine(CrossFadeTexture(fadeInTarget.tex, fadeOutTarget.tex, fadeTime));
    }
    //설정 되어있는 텍스쳐의 크기를 따라간다.
    public void SetBookUnitTextureSize(eBookUnitTex target)
    {
        GetCurrentTaeget(target).tex.MakePixelPerfect();
    }

    //특정 사이즈로 설정
    public void SetBookUnitTextureSize(int width, int height, eBookUnitTex target)
    {
        GetCurrentTaeget(target).tex.SetDimensions(width, height);
    }

    Texture LoadBookUnitTexture(string fileName)
    {
        fileName = fileName.Replace(" ", "");
		string[] str = Utility.Split(fileName, '/'); //fileName.Split('/');
        if (str.Length < 2)
            return null;

        Texture resultTex = ResourceMgr.Instance.LoadFromAssetBundle(str[0].ToLower(), fileName + ".png") as Texture;

        return resultTex;
    }

    Sprite LoadBookUnitSprite(string fileName, string imgName)
    {
        fileName = fileName.Replace(" ", "");
		string[] str = Utility.Split(fileName, '/'); //fileName.Split('/');
		if (str.Length < 2)
            return null;

        Object[] sprites = ResourceMgr.Instance.LoadFromAssetBundleWithSubObject(str[0].ToLower(), fileName + ".png");
        Sprite result = null;
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].name.Equals(imgName))
            {
                result = (Sprite)sprites[i];
                break;
            }
        }

        return result;
    }

    public bool BookUnitActiveSelf()
    {
        if (kUnitObj == null)
            return false;

        return kUnitObj.activeSelf;
    }

    public void SetBookUnitActive(bool visible)
    {
        if (kUnitObj != null)
            kUnitObj.SetActive(visible);
    }

    public void PlayShakeCard(float amount = 1f)
    {
        m_shakeAmount = amount;

        m_bisShake = true;
        m_shakeCoroutine = StartCoroutine(ShakeObject());
        
    }

    IEnumerator ShakeObject()
    {
        m_shakeOriginPos = this.transform.localPosition;
        float timer = 0f;

        while(m_bisShake)
        {
            this.transform.localPosition = (Vector3)UnityEngine.Random.insideUnitCircle * m_shakeAmount + m_shakeOriginPos;

            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void StopShakeCard()
    {
        m_bisShake = false;
        if (m_shakeCoroutine != null)
            StopCoroutine(m_shakeCoroutine);
    }

#region UnitEvent
    public void SetPosistion(string imgName, Vector3 pos, eBookUnitTex target = eBookUnitTex.BODY)
    {
        switch(target)
        {
            case eBookUnitTex.FACE:
                {
                    if(string.IsNullOrEmpty(imgName))
                    {
                        m_faceTextureSrc.tex.transform.localPosition = pos;
                        m_faceTextureDest.tex.transform.localPosition = pos;
                    }
                    else
                    {
                        m_faceSpriteSrc.sprite.transform.localPosition = pos;
                        m_faceSpriteDest.sprite.transform.localPosition = pos;
                    }
                    
                }
                break;
            case eBookUnitTex.BODY:
                {
                    if (string.IsNullOrEmpty(imgName))
                    {
                        m_bodyTextureSrc.tex.transform.localPosition = pos;
                        m_bodyTextureDest.tex.transform.localPosition = pos;
                    }
                    else
                    {
                        m_bodySpriteSrc.sprite.transform.localPosition = pos;
                        m_bodySpriteDest.sprite.transform.localPosition = pos;
                    }
                        
                }
                break;
        }
    }

    public void Talk(string imgName, float duration)        //말하기
    {
        //TweenScale.SetTweenScale(m_tweenScale, UITweener.Style.Once, m_tweenScale.value, new Vector3(1.1f, 1.1f, 1.0f), duration, 0.0f, null);
        SetColor(imgName, new Color(1, 1, 1, 1), duration);
    }

    public void Listen(string imgName, float duration)      //듣기
    {
        //TweenScale.SetTweenScale(m_tweenScale, UITweener.Style.Once, m_tweenScale.value, new Vector3(1.0f, 1.0f, 1.0f), duration, 0.0f, null);
        SetColor(imgName, new Color(0.5f, 0.5f, 0.5f, 1), duration);
    }

    public void Stand(string imgName, float duration)       //대기
    {
        //TweenScale.SetTweenScale(m_tweenScale, UITweener.Style.Once, m_tweenScale.value, new Vector3(1.0f, 1.0f, 1.0f), duration, 0.0f, null);
        SetColor(imgName, new Color(1, 1, 1, 1), duration);
    }

    public void SetCharLocalScale(float scale)
    {
        Log.Show(kUnitObj.transform.localScale, Log.ColorType.Red);
        kUnitObj.transform.localScale = new Vector3(scale, scale, scale);
        Log.Show(kUnitObj.transform.localScale);
    }

    public void SetColor(string imgName, Color color, float duration)       //색상 조절 - Fade
    {
        if(string.IsNullOrEmpty(imgName))
        {
            TweenColor.SetTweenColor(m_bodyTextureSrc.tweenColor, m_bodyTextureSrc.tweenColor.value, color, duration, 0.0f, null);
            TweenColor.SetTweenColor(m_faceTextureSrc.tweenColor, m_faceTextureSrc.tweenColor.value, color, duration, 0.0f, null);
        }
        else
        {
            TweenColor.SetTweenColor(m_bodySpriteSrc.tweenColor, m_bodySpriteSrc.tweenColor.value, color, duration, 0.0f, null);
            TweenColor.SetTweenColor(m_faceSpriteSrc.tweenColor, m_faceSpriteSrc.tweenColor.value, color, duration, 0.0f, null);
        }
        
    }

    public void SetColor(string imgName, Color color)       //색상 조절
    {
        if(string.IsNullOrEmpty(imgName))
        {
            m_bodyTextureSrc.tex.color = color;
            m_faceTextureSrc.tex.color = color;
        }
        else
        {
            m_bodySpriteSrc.sprite.color = color;
            m_faceSpriteSrc.sprite.color = color;
        }
        
    }

    public void FadeIn(string imgName, float duration)      //현재 색상 유지하며 FadeIn
    {
        if(string.IsNullOrEmpty(imgName))
        {
            TweenColor.SetTweenColor(m_bodyTextureSrc.tweenColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
            TweenColor.SetTweenColor(m_faceTextureSrc.tweenColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
        }
        else
        {
            TweenColor.SetTweenColor(m_bodySpriteSrc.tweenColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
            TweenColor.SetTweenColor(m_faceSpriteSrc.tweenColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
        }
    }

    public void FadeIn(float duration)
    {
        if(null != m_bodyTextureSrc && null != m_faceTextureSrc)
        {
            TweenColor.SetTweenColor(m_bodyTextureSrc.tweenColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
            TweenColor.SetTweenColor(m_faceTextureSrc.tweenColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
        }
        else if(null != m_bodySpriteSrc && null != m_faceSpriteSrc)
        {
            TweenColor.SetTweenColor(m_bodySpriteSrc.tweenColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
            TweenColor.SetTweenColor(m_faceSpriteSrc.tweenColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
        }
    }

    public void FadeOut(string imgName, float duration)     //현재 색상 유지하며 FadeOut
    {
        if(string.IsNullOrEmpty(imgName))
        {
            TweenColor.SetTweenColor(m_bodyTextureSrc.tweenColor, m_bodyTextureSrc.tweenColor.value, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
            TweenColor.SetTweenColor(m_faceTextureSrc.tweenColor, m_faceTextureSrc.tweenColor.value, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
        }
        else
        {
            TweenColor.SetTweenColor(m_bodySpriteSrc.tweenColor, m_bodySpriteSrc.tweenColor.value, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
            TweenColor.SetTweenColor(m_faceSpriteSrc.tweenColor, m_faceSpriteSrc.tweenColor.value, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
        }
    }

    public void FadeOut(float duration)
    {
        if(null != m_bodyTextureSrc && null != m_faceTextureSrc)
        {
            TweenColor.SetTweenColor(m_bodyTextureSrc.tweenColor, m_bodyTextureSrc.tweenColor.value, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
            TweenColor.SetTweenColor(m_faceTextureSrc.tweenColor, m_faceTextureSrc.tweenColor.value, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
        }
        else if(null != m_bodySpriteSrc && null != m_faceSpriteSrc)
        {
            TweenColor.SetTweenColor(m_bodySpriteSrc.tweenColor, m_bodySpriteSrc.tweenColor.value, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
            TweenColor.SetTweenColor(m_faceSpriteSrc.tweenColor, m_faceSpriteSrc.tweenColor.value, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
        }
    }

    public void FadeInBlack(string imgName, float duration)     //까만색에서 원래 색상으로 FadeIn
    {
        if(string.IsNullOrEmpty(imgName))
        {
            TweenColor.SetTweenColor(m_bodyTextureSrc.tweenColor, new Color(0.0f, 0.0f, 0.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
            TweenColor.SetTweenColor(m_faceTextureSrc.tweenColor, new Color(0.0f, 0.0f, 0.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
        }
        else
        {
            TweenColor.SetTweenColor(m_bodySpriteSrc.tweenColor, new Color(0.0f, 0.0f, 0.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
            TweenColor.SetTweenColor(m_faceSpriteSrc.tweenColor, new Color(0.0f, 0.0f, 0.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
        }
        
    }

    public void FadeOutBlack(string imgName, float duration)        //현재 샋상에서 까만색으로 FadeOut
    {
        if(string.IsNullOrEmpty(imgName))
        {
            TweenColor.SetTweenColor(m_bodyTextureSrc.tweenColor, m_bodyTextureSrc.tweenColor.value, new Color(0.0f, 0.0f, 0.0f, 0.0f), duration, 0.0f, null);
            TweenColor.SetTweenColor(m_faceTextureSrc.tweenColor, m_faceTextureSrc.tweenColor.value, new Color(0.0f, 0.0f, 0.0f, 0.0f), duration, 0.0f, null);
        }
        else
        {
            TweenColor.SetTweenColor(m_bodySpriteSrc.tweenColor, m_bodyTextureSrc.tweenColor.value, new Color(0.0f, 0.0f, 0.0f, 0.0f), duration, 0.0f, null);
            TweenColor.SetTweenColor(m_faceSpriteSrc.tweenColor, m_faceTextureSrc.tweenColor.value, new Color(0.0f, 0.0f, 0.0f, 0.0f), duration, 0.0f, null);
        }
    }

    //현재 색상 유지하며 CrossFade / FACE의 경우에만 얼굴만 변경
    public void CrossFade(float duration, string imgName, eBookUnitTex target = eBookUnitTex.BODY)
    {
        switch(target)
        {
            case eBookUnitTex.FACE:
                {
                    if(string.IsNullOrEmpty(imgName))
                    {
                        TweenColor.SetTweenColor(m_faceTextureSrc.tweenColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
                        TweenColor.SetTweenColor(m_faceTextureDest.tweenColor, m_faceTextureDest.tweenColor.value, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
                    }
                    else
                    {
                        TweenColor.SetTweenColor(m_faceSpriteSrc.tweenColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
                        TweenColor.SetTweenColor(m_faceSpriteDest.tweenColor, m_faceSpriteDest.tweenColor.value, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
                    }
                    
                }
                break;
            case eBookUnitTex.BODY:
                {
                    if (string.IsNullOrEmpty(imgName))
                    {
                        TweenColor.SetTweenColor(m_bodyTextureSrc.tweenColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
                        TweenColor.SetTweenColor(m_faceTextureSrc.tweenColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);

                        TweenColor.SetTweenColor(m_bodyTextureDest.tweenColor, m_bodyTextureDest.tweenColor.value, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
                        TweenColor.SetTweenColor(m_faceTextureDest.tweenColor, m_faceTextureDest.tweenColor.value, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
                    }
                    else
                    {
                        TweenColor.SetTweenColor(m_bodySpriteSrc.tweenColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
                        TweenColor.SetTweenColor(m_faceSpriteSrc.tweenColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);

                        TweenColor.SetTweenColor(m_bodySpriteDest.tweenColor, m_bodySpriteDest.tweenColor.value, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
                        TweenColor.SetTweenColor(m_faceSpriteDest.tweenColor, m_faceSpriteDest.tweenColor.value, new Color(1.0f, 1.0f, 1.0f, 0.0f), duration, 0.0f, null);
                    }
                    
                }
                break;
        }
        
    }

    public void SetUnitPos(float podIdx)
    {
        this.transform.localPosition = GetMovePosition(podIdx);
    }

    public void MoveTo(float toPos, float duration)
    {
        TweenPosition.SetTweenPosition(m_tweenPos, this.transform.localPosition, GetMovePosition(toPos), duration, 0, null);
    }

    public Vector3 GetMovePosition(float pos)
    {
        Vector3 resultPos = Vector3.zero;

        float tempMoveX = 1280f / 5;

        resultPos.x = tempMoveX * pos;
        /*
        float f = (float)Screen.width / (float)Screen.height;
        if (f >= 1.6f) // 16:10
        {
            resultPos.y= (-Screen.height / (3f * f));
        }
        else if (f >= 1.3f)  // 4:3
        {
            resultPos.y = 0f;
        }
        else //Default 16:9
        {

            resultPos.y = (-Screen.height / 4f);
        }*/


        return resultPos;
    }
    #endregion
}
