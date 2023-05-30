
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveJumpSection : MonoBehaviour
{
    public float Time = 0.0f;

    private Director mDirector = null;
    private UICinematicPopup mUICinematic = null;


    private void Awake()
    {
        mDirector = GetComponentInParent<Director>();
        mUICinematic = GameUIManager.Instance.ShowUI("CinematicPopup", false) as UICinematicPopup;
    }

    private void OnEnable()
    {
        mUICinematic.ShowJumpSection(mDirector, Time);
    }

    private void OnDisable()
    {
        mUICinematic.HideJumpSection();
    }
}
