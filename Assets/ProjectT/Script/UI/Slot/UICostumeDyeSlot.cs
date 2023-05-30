using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICostumeDyeSlot : FSlot
{
    [SerializeField] private int mPart = -1;
    [SerializeField] private int mIndex = -1;
    
    public UISprite kColorSpr;
    public UISprite kLockSpr;
    public UISprite kSelSpr;
    public Animation kResultAnim;
    public UIButton kRGBBtn;
    
    public int Part => mPart;
    public int Index => mIndex;
    
    private Coroutine _resultCoroutine;
    private void Awake()
    {
        if (kResultAnim != null)
        {
            kResultAnim.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        PlayAnimation();
    }

    private IEnumerator ResultAnim()
    {
        if (kResultAnim != null)
        {
            kResultAnim.gameObject.SetActive(false);
        }

        yield return new WaitForEndOfFrame();

        if (kResultAnim != null)
        {
            kResultAnim.gameObject.SetActive(true);
            
            SoundManager.Instance.PlayUISnd(84);

            while (kResultAnim.isPlaying)
            {
                yield return null;
            }
            
            kResultAnim.gameObject.SetActive(false);
        }
    }
    
    public void SetSlotActive(bool active)
    {
        if (kSelSpr != null)
            kSelSpr.gameObject.SetActive(active);
    }
    
    public void SetLock(bool bLock)
    {
        if (kLockSpr != null)
            kLockSpr.gameObject.SetActive(bLock);
    }
    
    public void SetColor(Color color)
    {
        if (kColorSpr != null)
            kColorSpr.color = color;
    }

    public void PlayAnimation()
    {
        Utility.StopCoroutine(this, ref _resultCoroutine);
        _resultCoroutine = StartCoroutine(ResultAnim());
    }
    
    public void SetRGBBtn(bool active)
    {
        if (kRGBBtn != null)
            kRGBBtn.gameObject.SetActive(active);
    }
}
