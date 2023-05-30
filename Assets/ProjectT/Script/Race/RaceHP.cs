using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceHP : MonoBehaviour
{
    public UISprite kActiveHp;

    void Start()
    {
        kActiveHp.gameObject.SetActive(true);
    }
    
    public void SetHpState(bool _state)
    {
        kActiveHp.gameObject.SetActive(_state);
    }
}
