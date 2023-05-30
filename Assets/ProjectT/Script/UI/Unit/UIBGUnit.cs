using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBGUnit : FUnit
{
    public UITexture texBg;

    private Animation _uiani = null;
    protected List<string> _aninamelist = new List<string>();
    private bool baniopen = false;
    private bool baniclose = false;
    private bool baniloop = false;

    public void Awake()
    {
        _uiani = GetComponent<Animation>();
        if (_uiani)
        {
            foreach (AnimationState aniState in _uiani)
                _aninamelist.Add(aniState.clip.name);

            if (_aninamelist.Count >= 1)
                baniopen = true;
            if (_aninamelist.Count >= 2)
                baniclose = true;
            if (_aninamelist.Count >= 3)
                baniloop = true;
        }
    }
    public void OnEnable()
    {

    }

    public void OnDisable()
    {

    }

    public void SetUIActive(bool _bActive, int depth = 0)
    {
        UIPanel panel = this.gameObject.GetComponent<UIPanel>();
        if (panel != null)
            panel.depth = depth;

        CancelInvoke("OnClose");
        CancelInvoke("OnLoop");


        if (_bActive)
        {
            this.gameObject.SetActive(true);
            if (_uiani != null)
            {
                if (baniopen)
                {
                    float ftime = _uiani[_aninamelist[0]].length;
                    _uiani.Play(_aninamelist[0]);
                    if(baniloop)
                        Invoke("OnLoop", ftime);
                }
            }
        }
        else
        {
            if (_uiani != null)
            {
                if (baniclose)
                {
                    float ftime = _uiani[_aninamelist[1]].length;
                    _uiani.Play(_aninamelist[1]);
                    Invoke("OnClose", ftime);
                }
                else
                {
                    OnClose();
                }
            }
            else
            {
                OnClose();
            }
        }

    }

    public virtual void OnClose()
    {
        gameObject.SetActive(false);
    }

    public float PlayAnimtion(int i)
    {
        float ftime = _uiani[_aninamelist[i]].length;
        _uiani.Play(_aninamelist[i]);
        return ftime;
    }

    public void OnLoop()
    {
        PlayAnimtion(2);
    }

    public void SetBgTexture(Texture2D tex, int difficulty)
    {
        if (texBg == null)
            return;

        texBg.mainTexture = tex;
        SetBgColor(difficulty);
    }

    public void SetBgColor(int difficulty)
    {
        if (texBg == null)
            return;

        Material mtrl = texBg.material;
        if (difficulty == 1) // Easy
        {
            texBg.shader = null;
            texBg.color = Color.white;
        }
        else if (difficulty == 2) // Normal
        {
            texBg.shader = Shader.Find("eTOYLab/ColorDodge");
            texBg.color = new Color(0.549f, 0.482f, 0.819f);
        }
        else if (difficulty == 3) // Hard
        {
            texBg.shader = Shader.Find("eTOYLab/ColorDodge");
            texBg.color = new Color(0.85f, 0.45f, 0.45f);
        }
    }
}
