using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResultSlot : MonoBehaviour
{
    public Animation ani;    

    public void PlayClear()
    {
        bool b = false;
        if (ani)
            b = ani.Play("MissionInfoClear");
    }

    public void PlayIn()
    {
        bool b = false;
        if (ani)
            b = ani.Play("[IN]MissionInfoSlot");
    }
}