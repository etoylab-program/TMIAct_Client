
using System.Collections;
using UnityEngine;


public class ActionMurasakiUpperJump : ActionUpperJump
{
    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            switch ((eState)mState.current)
            {
                case eState.Start:
                    if (m_owner.isFalling)
                    {
                        m_endUpdate = true;
                    }
                    break;
            }

            m_owner.cmptJump.UpdateJump();

            if ((eState)mState.current != eState.End)
            {
                m_owner.cmptMovement.UpdatePosition(Vector3.zero, 0.0f, true);
                m_owner.cmptRotate.UpdateRotation(Vector3.zero, false);
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        SetNextAction(eActionCommand.AirDownAttack, null);
    }
}
