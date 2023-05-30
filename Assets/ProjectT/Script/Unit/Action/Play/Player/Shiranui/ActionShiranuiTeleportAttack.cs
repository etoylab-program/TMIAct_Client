
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionShiranuiTeleportAttack : ActionSelectSkillBase {
	private Shiranui mShiranui = null;
	private float mDuration = 0.05f;
	private UnitCollider mTargetCollider = null;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.EmergencyAttack;

		conditionActionCommand = new eActionCommand[1];
		conditionActionCommand[0] = eActionCommand.Defence;

		superArmor = Unit.eSuperArmor.Lv2;

		mShiranui = m_owner as Shiranui;

		if ( mValue1 > 0.0f ) {
			List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllAttackEvent( eAnimation.TeleportAttack );
			for ( int i = 0; i < list.Count; i++ ) {
				list[i].behaviour = eBehaviour.StunAttack;
			}
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		Utility.IgnorePhysics( eLayer.Player, eLayer.Enemy );
		Utility.IgnorePhysics( eLayer.Player, eLayer.EnemyGate );

		mTargetCollider = m_owner.attacker ? m_owner.attacker.MainCollider : m_owner.GetMainTargetCollider( true );
		if ( mTargetCollider ) {
			ShowSkillNames( m_data );
			m_aniLength = m_owner.PlayAniImmediate( eAnimation.Teleport );
		}
	}

	public override IEnumerator UpdateAction() {
		if ( mTargetCollider ) {
			yield return new WaitForSeconds( m_aniLength /2);

			m_owner.SetCloneShader( 0, mShiranui.CloneShader );
			m_owner.ShowClone( 0, m_owner.transform.position, m_owner.transform.rotation );

			PrepareTeleport();

			yield return new WaitForSeconds( mDuration );

			m_owner.ShowMesh( true );
			m_owner.LookAtTarget( mTargetCollider.GetCenterPos() );

			m_aniLength = m_owner.PlayAniImmediate( eAnimation.TeleportAttack );
			yield return new WaitForSeconds( m_aniLength );

			m_owner.HideClone( 0 );
			EffectManager.Instance.Play( m_owner.GetClone( 0 ), 50015, EffectManager.eType.Common );
		}
		else {
			m_owner.actionSystem.CancelCurrentAction();
		}
	}

	public override void OnEnd() {
		base.OnEnd();

		Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.Enemy );
		Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.EnemyGate );

		m_owner.checkRayCollision = true;

		if ( mTargetCollider ) {
			m_owner.LookAtTarget( mTargetCollider.GetCenterPos() );
		}
	}

	public override void OnCancel() {
		base.OnCancel();

		Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.Enemy );
		Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.EnemyGate );

		m_owner.checkRayCollision = true;
		m_owner.HideClone( 0 );
	}

	public override float GetAtkRange() {
		return 20.0f;
	}

	private void PrepareTeleport() {
		m_owner.ShowMesh( false );
		m_owner.checkRayCollision = false;

		bool toBack = false;

		if ( mTargetCollider != null ) {
			if ( !Physics.Raycast( mTargetCollider.Owner.GetCenterPos(), -mTargetCollider.Owner.transform.forward, 1.5f,
								 ( 1 << (int)eLayer.Wall ) | ( 1 << (int)eLayer.EnvObject ) ) && !mTargetCollider.Owner.noBack ) {
				toBack = true;
			}

			Vector3 checkPos = Vector3.zero;
			Vector3 pos = m_owner.GetCenterPos();
			Vector3 dir = Vector3.zero;
			Vector3 targetCenterPos = mTargetCollider.GetCenterPos();

			RaycastHit hitInfo;

			int enemyLayer = Utility.GetEnemyLayer( (eLayer)m_owner.gameObject.layer );
			enemyLayer ^= ( 1 << (int)eLayer.EnvObject );

			if ( toBack ) {
				checkPos = targetCenterPos - ( mTargetCollider.Owner.transform.forward * 50.0f );

				dir = ( targetCenterPos - checkPos ).normalized;
				dir.y = 0.0f;

				if ( Physics.Raycast( checkPos, dir, out hitInfo, Mathf.Infinity, enemyLayer ) ) {
					pos = hitInfo.point - ( mTargetCollider.Owner.transform.forward * 1.2f );
				}
			}
			else {
				checkPos = targetCenterPos + ( mTargetCollider.Owner.transform.forward * 50.0f );

				dir = ( targetCenterPos - checkPos ).normalized;
				dir.y = 0.0f;

				if ( Physics.Raycast( checkPos, dir, out hitInfo, Mathf.Infinity, enemyLayer ) ) {
					pos = hitInfo.point + ( mTargetCollider.Owner.transform.forward * 1.2f );
				}
			}

			Collider[] cols = Physics.OverlapBox( pos, Vector3.one * 0.6f, Quaternion.identity, ( 1 << (int)eLayer.Wall ) | ( 1 << (int)eLayer.EnvObject ) );
			if ( cols.Length > 0 || !Physics.Raycast( new Vector3( pos.x, m_owner.GetCenterPos().y, pos.z ), -Vector3.up, 1.0f, 1 << (int)eLayer.Floor ) ) {
				pos = m_owner.transform.position;
			}

			pos.y = m_owner.transform.position.y;
			m_owner.rigidBody.MovePosition( pos );
		}
	}
}
