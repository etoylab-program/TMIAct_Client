
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionCloneAttack : ActionSelectSkillBase
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.CloneAttack;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.CloneAttack01, 0.0f, true);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            m_checkTime += Time.fixedDeltaTime;
            if(m_checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        DeactivateOwner();
    }

    public override void OnCancel()
    {
        base.OnCancel();
        DeactivateOwner();
    }

	private void DeactivateOwner() {
		if( !m_owner.IsActivate() ) {
			return;
		}

		m_owner.StopPauseFrame();
		m_owner.StopBT();
		m_owner.Deactivate();
	}
}
