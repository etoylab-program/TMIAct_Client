
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveUIPrefab : MonoBehaviour
{
    private Director mDirector = null;
    private UICinematicPopup mUICinematic = null;

    public string kActiveUIName = string.Empty;
    public bool kDisableOff = true;


    private void Awake()
    {
        mDirector = GetComponentInParent<Director>();
        mUICinematic = GameUIManager.Instance.ShowUI("CinematicPopup", false) as UICinematicPopup;
    }

    private void OnEnable()
    {
        mUICinematic.ShowCinematicUI(kActiveUIName);
    }

    private void OnDisable()
    {
        if(kDisableOff)
        {
            mUICinematic.HideCinematicUI(kActiveUIName);
        }
    }
}
