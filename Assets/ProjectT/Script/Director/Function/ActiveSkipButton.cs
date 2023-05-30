
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveSkipButton : MonoBehaviour
{
    private Director            mDirector       = null;
    private UICinematicPopup    mUICinematic    = null;


    private void Awake()
    {
        mDirector = GetComponentInParent<Director>();
        mUICinematic = GameUIManager.Instance.ShowUI("CinematicPopup", false) as UICinematicPopup;
    }

    private void OnEnable()
    {
        mUICinematic.ShowSkipButton(mDirector);
    }

    private void OnDisable()
    {
        mUICinematic.HideSkipButton();
    }
}
