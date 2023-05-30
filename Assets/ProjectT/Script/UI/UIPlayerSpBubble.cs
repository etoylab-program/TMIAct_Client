
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIPlayerSpBubble : MonoBehaviour
{
    public UISprite sprNormal;
    //public UISprite sprSpecial;
    public ParticleSystem psOn;
    public ParticleSystem psPossibleSkill;

    public bool OnPossibleSkill { get; private set; } = false;

    private bool on = false;


    public void Init()
    {
        sprNormal.gameObject.SetActive(false);
        sprNormal.fillAmount = 0.0f;
        //sprSpecial.gameObject.SetActive(false);
        psOn.gameObject.SetActive(false);
        psPossibleSkill.gameObject.SetActive(false);

        on = false;
        OnPossibleSkill = false;
    }

    public void Fill(float amount, bool checkFillAmount = true)
    {
        if(checkFillAmount && sprNormal.fillAmount >= 1.0f)
        {
            return;
        }

        if(!sprNormal.gameObject.activeSelf)
        {
            sprNormal.fillAmount = 0.0f;
            sprNormal.gameObject.SetActive(true);
        }

        sprNormal.fillAmount = amount;
    }

    public void On(bool possibleSkill, bool full)
    {
        //sprSpecial.gameObject.SetActive(false);

        sprNormal.gameObject.SetActive(true);
        sprNormal.fillAmount = 1.0f;

        if (!on)
        {
            psOn.gameObject.SetActive(true);
            EffectManager.Instance.RegisterStopEff(psOn, transform);
            on = true;
        }

        if (!full)
        {
            if (possibleSkill && !OnPossibleSkill)
            {
                psPossibleSkill.gameObject.SetActive(true);
                OnPossibleSkill = true;
            }
        }
        else
            FullSp();
    }

    public void Off()
    {
        Init();
    }

    /*public void OffFullSp(bool possibleSkill)
    {
        //if (!sprSpecial.gameObject.activeSelf)
        //    return;

        if(!sprNormal.gameObject.activeSelf)
        {
            return;
        }

        On(possibleSkill, false);
    }*/

    private void FullSp()
    {
        //if (sprSpecial.gameObject.activeSelf)
        //    return;

        sprNormal.gameObject.SetActive(true);
        psOn.gameObject.SetActive(false);
        psPossibleSkill.gameObject.SetActive(false);

        //sprSpecial.gameObject.SetActive(true);
    }
}
