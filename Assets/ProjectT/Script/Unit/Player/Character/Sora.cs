
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;


public class Sora : Player {
	private List<Projectile>            mListPjt			= new List<Projectile>();
	private AniEvent.sProjectileInfo	mPjtInfo			= null;
	private AniEvent.sEvent             mAniEvt				= null;
	private ActionSoraExtreamEvade      mActionExtreamEvade	= null;


	public override void Init( int tableId, eCharacterType type, string faceAniControllerPath ) {
		base.Init( tableId, type, faceAniControllerPath );
		CancelComboAttack = false;

		for( int i = 0; i < 10; i++ ) {
			Projectile pjt = GameSupport.CreateProjectile( "Projectile/pjt_character_foxsora_homingarrow.prefab" );
			mListPjt.Add( pjt );
		}

		mAniEvt = m_aniEvent.CreateEvent( eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f );
		mPjtInfo = m_aniEvent.CreateProjectileInfo( mListPjt[0] );
	}

	public override bool OnAttack( AniEvent.sEvent evt, float atkRatio, bool notAniEventAtk ) {
		if( ExtreamEvading ) {
			FireProjectile();
		}

		return base.OnAttack( evt, atkRatio, notAniEventAtk );
	}

	public override bool OnHit( Unit attacker, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent attackerAniEvt, ObscuredFloat damage, 
								ref bool isCritical, ref eHitState hitState, Projectile projectile, bool isUltimateSkill, bool skipMaxDamageRecord ) {
		bool breakShield = base.OnHit( attacker, toExecuteType, attackerAniEvt, damage, ref isCritical, ref hitState, 
									   projectile, isUltimateSkill, skipMaxDamageRecord );

		if( hitState == eHitState.Success && Utility.IsSpecialAttack( attackerAniEvt, projectile ) ) {
			ResetComboAttackIndex();
		}

		return breakShield;
	}

	public override void OnStartSkill( ActionSelectSkillBase currentSkillAction ) {
		if( currentSkillAction == null || currentSkillAction.actionCommand == eActionCommand.Attack01 ) {
			return;
		}

		ResetComboAttackIndex();
	}

	public override void OnStartUSkill() {
		ResetComboAttackIndex();
	}

	protected override bool OnFire( AniEvent.sEvent evt ) {
		if( ExtreamEvading ) {
			FireProjectile();
		}

		return base.OnFire( evt );
	}

	protected override void OnEventDefence( IActionBaseParam param = null ) {
		if( m_actionSystem.IsCurrentUSkillAction() == true ) {
			return;
		}

		if( CheckTimingHoldAttack() ) {
			return;
		}

		ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
		ActionSelectSkillBase actionSkill = m_actionSystem.GetAction<ActionSelectSkillBase>(eActionCommand.Teleport);
		if( actionSkill && actionSkill.PossibleToUse && actionDash && actionDash.IsPossibleToDashAttack() && GetMainTargetCollider( true ) ) {
			float checkTime = actionDash.GetEvadeCutFrameLength();
			if( World.Instance.UIPlay.btnDash.deltaTime < checkTime ) {
				CommandAction( eActionCommand.Teleport, null );
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
			EffectManager.Instance.Play( this, 1028, EffectManager.eType.Common );

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

	private void ResetComboAttackIndex() {
		ActionComboAttack action = m_actionSystem.GetAction<ActionComboAttack>(eActionCommand.Attack01);
		if( action == null ) {
			Debug.LogError( "소라 콤보 어택 액션이 왜 없어???" );
			return;
		}

		action.ResetComboAttackIndex();
	}

	private void FireProjectile() {
		if( mActionExtreamEvade == null ) {
			mActionExtreamEvade = m_actionSystem.GetAction<ActionSoraExtreamEvade>( eActionCommand.ExtreamEvade );
		}

		if( mActionExtreamEvade == null ) {
			return;
		}

		for( int i = 0; i < mActionExtreamEvade.PjtCount; i++ ) {
			if( mListPjt[i].IsActivate() ) {
				continue;
			}

			UnitCollider targetCollider = GetMainTargetCollider(true);
			if( targetCollider ) {
				mPjtInfo.followParentRot = true;
				mPjtInfo.addedPosition = GetCenterPos() - transform.position;
				mPjtInfo.notAniEventAtk = true;

				mAniEvt.atkRatio = mActionExtreamEvade.PjtAttackPowerRatio;

				mListPjt[i].OnHitFunc = mActionExtreamEvade.OnProjectileHit;
				mListPjt[i].Fire( this, BattleOption.eToExecuteType.Supporter, mAniEvt, mPjtInfo, targetCollider.Owner, -1 );
			}
		}
	}
}
