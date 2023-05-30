
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Aina : Player {
	private Transform	mEyePatchTransform = null;
	private Quaternion	mEyePatchOriginRotation = Quaternion.identity;


	public override void Init( int tableId, eCharacterType type, string faceAniControllerPath ) {
		base.Init( tableId, type, faceAniControllerPath );

		mEyePatchTransform = aniEvent.GetBoneByName( "Bone_eyepatch" );
		if ( mEyePatchTransform ) {
			mEyePatchOriginRotation = mEyePatchTransform.localRotation;
		}
	}

	protected override void OnEventDefence( IActionBaseParam param = null ) {
		if ( m_actionSystem.IsCurrentUSkillAction() == true ) {
			return;
		}

		if ( CheckTimingHoldAttack() ) {
			return;
		}

		ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
		ActionSelectSkillBase actionSkill = m_actionSystem.GetAction<ActionSelectSkillBase>( eActionCommand.Teleport );
		if ( actionSkill && actionSkill.PossibleToUse && actionDash && actionDash.IsPossibleToDashAttack() && GetMainTargetCollider( true ) ) {
			float checkTime = actionDash.GetEvadeCutFrameLength();
			if ( World.Instance.UIPlay.btnDash.deltaTime < checkTime ) {
				CommandAction( eActionCommand.Teleport, null );
				return;
			}
		}

		CommandAction( eActionCommand.Defence, param );
	}

	protected override void OnEventSpecialAtk() {
		if ( m_actionSystem.IsCurrentSkillAction() || m_actionSystem.IsCurrentUSkillAction() ) {
			return;
		}

		ActionBase action = m_actionSystem.GetAction( eActionCommand.AttackDuringAttack );
		if ( action == null ) {
			return;
		}

		if ( m_actionSystem.IsCurrentAction( eActionCommand.Attack01 ) ) {
			if ( !IsHelper ) {
				World.Instance.UIPlay.btnAtk.lockCharge = true;
			}

			CommandAction( eActionCommand.AttackDuringAttack, null );
		}
	}

	public void RestoreEyePatch() {
		if ( mEyePatchTransform == null ) {
			return;
		}

		mEyePatchTransform.localRotation = mEyePatchOriginRotation;
	}
}
