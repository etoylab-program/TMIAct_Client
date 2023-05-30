
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionYukikazeUpperAttack : ActionUpperAttack
{
    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= m_aniCutFrameLength)
            {
                if (m_owner.GetMainTargetCollider(true))
                    SetNextAction(eActionCommand.AirShotAttack, null);

                m_endUpdate = true;
            }
            else if (m_checkTime >= m_aniLength)
                m_endUpdate = true;

            yield return mWaitForFixedUpdate;
        }
    }
}
