
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveDialog : MonoBehaviour
{
    [Header("[Property]")]
    public int GroupId;
    public int Num;

    private Director            mDirector       = null;
    private UICinematicPopup    mUICinematic    = null;


    private void Awake()
    {
        mDirector = GetComponentInParent<Director>();
    }

    private void OnEnable()
    {
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
        {
            mUICinematic = TitleUIManager.Instance.ShowUI("CinematicPopup", false) as UICinematicPopup;
        }
        else
        {
            mUICinematic = GameUIManager.Instance.ShowUI("CinematicPopup", false) as UICinematicPopup;
        }

        mUICinematic.ShowDialog(mDirector, GroupId, Num);
    }

    private void OnDisable()
    {
        mUICinematic.HideDialog();
    }
}
