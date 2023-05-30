using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEffectActive : MonoBehaviour {

    public GameObject effectObject;
    public float Duration = 0.5f;

    private void OnEnable()
    {
        if(effectObject == this.gameObject)
        {
            StopCoroutine("EffectDisable");
            StartCoroutine("EffectDisable");
        }
    }

    public void SetEffectActive()
    {
        if(effectObject.gameObject.activeSelf)
            effectObject.SetActive(false);

        effectObject.SetActive(true);

        StopCoroutine("EffectDisable");
        StartCoroutine("EffectDisable");
    }

    IEnumerator EffectDisable()
    {
        yield return new WaitForSeconds(Duration);
        effectObject.SetActive(false);
        yield break;
    }
}