
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PassPortal : OtherObject//, IObserver
{
    public ParticleSystem psPass;


    public override void Init(int tableId, eCharacterType type, string faceAniControllerPath)
    {
        base.Init(tableId, type, faceAniControllerPath);
        SetKinematicRigidBody();
    }

    public void OnNotify(eObserverMsg observerMsg)
    {
        if (observerMsg == eObserverMsg.PASS_PORTAL)
            psPass.Play();
    }
}
