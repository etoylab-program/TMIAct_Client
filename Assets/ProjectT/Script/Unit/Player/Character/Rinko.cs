
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rinko : Player
{
    protected override void OnEventDefence( IActionBaseParam param = null )
    {
        if (m_actionSystem.IsCurrentUSkillAction() == true)
        {
            return;
        }

        if(CheckTimingHoldAttack())
        {
            return;
        }

        ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
        ActionTeleport actionTeleport = m_actionSystem.GetAction<ActionTeleport>(eActionCommand.Teleport);
        if (actionTeleport && actionTeleport.PossibleToUse && actionDash && actionDash.IsPossibleToDashAttack() && GetMainTargetCollider(true))
        {
            float checkTime = actionDash.GetEvadeCutFrameLength();
            if (World.Instance.UIPlay.btnDash.deltaTime < checkTime)
            {
                CommandAction(eActionCommand.Teleport, null);
                return;
            }
        }

        CommandAction( eActionCommand.Defence, param );
    }

	protected override void FixedUpdate() {
		base.FixedUpdate();

		if( IsHelper || !m_actionSystem.HasAction( eActionCommand.TimingHoldAttack ) ) {
			return;
		}

		AniEvent.sAniInfo aniInfo = m_aniEvent.GetAniInfo( m_aniEvent.curAniType );
		if( aniInfo.timingHoldFrame <= 0.0f ) {
			if( World.Instance.UIPlay.EffDashFlash.gameObject.activeSelf ) {
				World.Instance.UIPlay.EffDashFlash.gameObject.SetActive( false );
			}

			return;
		}

		if( m_aniEvent.GetCurrentFrame() >= aniInfo.timingHoldFrame ) {
			EffectManager.Instance.Play( this, 402, EffectManager.eType.Common );

			if( !World.Instance.UIPlay.EffDashFlash.gameObject.activeSelf ) {
				World.Instance.UIPlay.EffDashFlash.gameObject.SetActive( true );
			}
		}
		else {
			if( World.Instance.UIPlay.EffDashFlash.gameObject.activeSelf ) {
				World.Instance.UIPlay.EffDashFlash.gameObject.SetActive( false );
			}
		}
	}
}
