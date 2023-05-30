using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHoldGauge : MonoBehaviour
{
    public UITexture kGaugeTex;
    public UITexture kBGTex;
    private float _now;
    private float _max;

    public void Show(Vector3 pos, float time)
    {
        this.gameObject.SetActive(true);
        GameSupport.TweenerPlay(kGaugeTex.gameObject);
        GameSupport.TweenerPlay(kBGTex.gameObject);

        this.transform.position = pos;
        _now = 0.0f;
        _max = time;
        kGaugeTex.fillAmount = 0.0f;
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        float f = 0.0f;
        _now += Time.deltaTime;
        if(_now >= _max)
        {
            _now = _max;
        }
        f = _now / _max;
        kGaugeTex.fillAmount = f;
    }
}
