
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveBossName : MonoBehaviour
{
    [Header("[Property]")]
    public UICinematicPopup.EAnchorBossName Anchor = UICinematicPopup.EAnchorBossName.BOTTOM_RIGHT;

    private Director            mDirector       = null;
    private Enemy               mOwnerEnemy     = null;
    private UICinematicPopup    mUICinematic    = null;


    private void Awake()
    {
        mDirector = GetComponentInParent<Director>();
        mOwnerEnemy = mDirector.Owner as Enemy;
        mUICinematic = GameUIManager.Instance.ShowUI("CinematicPopup", false) as UICinematicPopup;
    }

    private void OnEnable()
    {
        if(mDirector.Owner == null)
        {
            return;
        }
        else if(mOwnerEnemy == null)
        {
            mOwnerEnemy = mDirector.Owner as Enemy;
        }

        mUICinematic.ShowBossName(mDirector, Anchor, mOwnerEnemy.data);
    }

    private void OnDisable()
    {
        mUICinematic.HideBossName();
    }
}
