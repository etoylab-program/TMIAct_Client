
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Noah : Player
{
    private ParticleSystem  mEffDna     = null;
    private bool            mbPlayDna   = false;


    public override void OnGameStart()
    {
        base.OnGameStart();
        mEffDna = EffectManager.Instance.GetEffectOrNull(30023, EffectManager.eType.Common);
    }

    public override void OnGameEnd()
    {
        if(UsingUltimateSkill)
        {
            ActionNoahUSkill action = m_actionSystem.GetAction<ActionNoahUSkill>(eActionCommand.USkill01);
            if(action)
            {
                action.ForceEnd(true);
            }
        }

        Transform[] child = GetComponentsInChildren<Transform>(true);
        for(int i = 0; i < child.Length; i++)
        {
            if(child[i].name.CompareTo("fbx_fx_noah_claw") == 0)
            {
                Utility.SetLayer(child[i].gameObject, gameObject.layer, true);
                break;
            }
        }
    }

    public override void OnPVPSurrender()
    {
        base.OnPVPSurrender();

        if (UsingUltimateSkill)
        {
            ActionNoahUSkill action = m_actionSystem.GetAction<ActionNoahUSkill>(eActionCommand.USkill01);
            if (action)
            {
                action.ForceEnd(true);
            }
        }
    }

    protected override void OnEventDefence( IActionBaseParam param = null )
    {
        if (m_actionSystem.IsCurrentUSkillAction() == true)
        {
            return;
        }

        if (CheckTimingHoldAttack())
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

	protected override void OnEventSpecialAtk() {
		if( m_actionSystem.IsCurrentSkillAction() || m_actionSystem.IsCurrentUSkillAction() ) {
			return;
		}

		ActionBase action = m_actionSystem.GetAction(eActionCommand.AttackDuringAttack);
		if( action == null ) {
			return;
		}

		if( m_actionSystem.IsCurrentAction( eActionCommand.Attack01 ) ) {
			if( !IsHelper ) {
				World.Instance.UIPlay.btnAtk.lockCharge = true;
			}

			CommandAction( eActionCommand.AttackDuringAttack, null );
		}
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();

		if( mEffDna && OpponentPlayer && gameObject.layer != mEffDna.gameObject.layer ) {
			Utility.SetLayer( mEffDna.gameObject, gameObject.layer, true );
		}

		bool playDnaEff = m_aniEvent.curAniType == eAnimation.Jump01 || 
						  m_aniEvent.curAniType == eAnimation.Jump02 || 
						  m_aniEvent.curAniType == eAnimation.Jump03 ||
						  m_aniEvent.curAniType == eAnimation.Idle01 || 
						  m_aniEvent.curAniType == eAnimation.Stun ||
						  m_actionSystem.IsCurrentAction( eActionCommand.MoveByDirection ) ||
						  m_aniEvent.curAniType.ToString().Contains( "Communication_" ) ||
						  m_aniEvent.curAniType.ToString().Contains( "AcrossArea" );

		if( !mbPlayDna && playDnaEff ) {
			if( !UsingUltimateSkill && !World.Instance.IsEndGame ) {
				mbPlayDna = true;
				EffectManager.Instance.Play( this, 30023, EffectManager.eType.Common, 0.0f, true, false );
			}
		}
		else {
			if( !playDnaEff || UsingUltimateSkill || World.Instance.IsEndGame ) {
				mbPlayDna = false;
				EffectManager.Instance.StopEffImmediate( 30023, EffectManager.eType.Common, null );
			}

			if( mbPlayDna && World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
				WorldPVP worldPVP = World.Instance as WorldPVP;
				if( worldPVP ) {
					Player opponent = worldPVP.GetCurrentOpponentTeamCharOrNull();
					if( opponent && opponent.UsingUltimateSkill ) {
						EffectManager.Instance.StopEffImmediate( 30023, EffectManager.eType.Common, null );
					}
				}
			}
		}

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
			EffectManager.Instance.Play( this, 30020, EffectManager.eType.Common );

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
