using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StroyCharUnit : MonoBehaviour
{
    public int kTalkPos;
    public UITexture kBodyTex;
    public UITexture kFaceTex;
    public List<Texture> BodyList;
    public List<Texture> FaceList;
    private int _bodyindex;
    private int _faceindex;

    public void Init()
    {
        _bodyindex = -1;
        _faceindex = -1;
        kBodyTex.mainTexture = null;
        kFaceTex.mainTexture = null;
    }

    public bool Set( int body, int face )
    {
        if (body != -1)
        {
            if (!SetBody(body))
                return false;
        }
        if (face != -1)
        {
            if (!SetFace(face))
                return false;
        }
        return true;
    }

    public bool SetBody( int index )
    {
        if (kBodyTex == null)
            return false;
        if (0 <= index && BodyList.Count > index)
        {
            kBodyTex.mainTexture = BodyList[index];
            _bodyindex = index;
            return true;
        }

        return false;
    }

    public bool SetFace(int index)
    {
        if (kFaceTex == null)
            return false;
        if (0 <= index && FaceList.Count > index)
        {
            kFaceTex.mainTexture = FaceList[index];
            _faceindex = index;
            return true;
        }

        return false;
    }

    public void FadeIn(float duration)
    {
        TweenColor.SetTweenColor(kBodyTex.GetComponent<TweenColor>(), new Color(0.0f, 0.0f, 0.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);
        TweenColor.SetTweenColor(kFaceTex.GetComponent<TweenColor>(), new Color(0.0f, 0.0f, 0.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), duration, 0.0f, null);

    }

    public void FadeOut(float duration)
    {
        TweenColor.SetTweenColor(kBodyTex.GetComponent<TweenColor>(), kBodyTex.GetComponent<TweenColor>().value, new Color(0.0f, 0.0f, 0.0f, 0.0f), duration, 0.0f, null);
        TweenColor.SetTweenColor(kFaceTex.GetComponent<TweenColor>(), kFaceTex.GetComponent<TweenColor>().value, new Color(0.0f, 0.0f, 0.0f, 0.0f), duration, 0.0f, null);
    }

    public void SetColor(Color color, float duration)
    {
        TweenColor.SetTweenColor(kBodyTex.GetComponent<TweenColor>(), kBodyTex.GetComponent<TweenColor>().value, color, duration, 0.0f, null);
        TweenColor.SetTweenColor(kFaceTex.GetComponent<TweenColor>(), kFaceTex.GetComponent<TweenColor>().value, color, duration, 0.0f, null);
    }
}
