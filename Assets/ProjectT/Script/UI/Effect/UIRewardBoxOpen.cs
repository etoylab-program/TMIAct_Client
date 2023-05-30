using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRewardBoxOpen : MonoBehaviour {

    public TweenScale boxScale;
    public TweenScale slotScale;
    public float Opendelay = 0.2f;

    private void OnEnable()
    {
        boxScale.ResetToBeginning();
        slotScale.ResetToBeginning();
    }

    public void OpenBox()
    {        
        boxScale.enabled = true;
        slotScale.enabled = true;
        Invoke("CloseBox", Opendelay);
    }
    
    void CloseBox()
    {
        gameObject.SetActive(false);
    }
}