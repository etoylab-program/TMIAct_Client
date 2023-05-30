
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionDroneFollowOwner : ActionBase
{
    private DroneUnit mOwnerDrone = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.DroneFollowOwner;

        cancelActionCommand = new eActionCommand[2];
        cancelActionCommand[0] = eActionCommand.DroneLaserAttack;
        cancelActionCommand[1] = eActionCommand.Attack01;

        mOwnerDrone = m_owner as DroneUnit;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mOwnerDrone.PlayAni(eAnimation.Idle01);
        mOwnerDrone.GetMainTargetCollider(true);

        //mOwnerDrone.SetTweenToPos(new Vector3(0.0f, 0.5f, 0.0f));
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate)
        {
            if (mOwnerDrone.mainTarget == null)
            {
                mOwnerDrone.GetMainTargetCollider(false);
            }

            mOwnerDrone.UpdateMovement(null);
            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
