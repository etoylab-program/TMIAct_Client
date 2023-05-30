using UnityEngine;
using System.Collections;

public class UIBikeRace_BuffGaugeUnit : FUnit
{
    public UIPanel kUIPanel;
    public UISprite kGaugeBgSpr;
    public UISprite kGaugeSpr;
    public UISprite kIcoSpr;
    public UISprite kIcoSprForeground;

    float _duration = 0f;

    private void Start()
    {
        kUIPanel.alpha = 0.2f;
        kGaugeSpr.fillAmount = 0f;
    }

    public void SetBuffGaugeUnit(float duration)
    {
        _duration = duration;
        kUIPanel.alpha = 1f;
        kGaugeBgSpr.fillAmount = 1f;

        StopAllCoroutines();
        StartCoroutine(PlayBuffGaugeUnit());
    }

    IEnumerator PlayBuffGaugeUnit()
    {
        float StratTime = Time.time;
        float EndTime = StratTime + _duration;
        float clampVal = 0f;

        while(true)
        {
            clampVal = Mathf.Clamp((EndTime - Time.time) / _duration, 0f, 1f);
            kGaugeSpr.fillAmount = clampVal;

            if (EndTime < Time.time)
                break;
            
            yield return null;
        }
    }

    public void StopBuffGaugeUnit()
    {
        StopAllCoroutines();
        kUIPanel.alpha = 0.2f;
        kGaugeSpr.fillAmount = 0f;
    }
}
