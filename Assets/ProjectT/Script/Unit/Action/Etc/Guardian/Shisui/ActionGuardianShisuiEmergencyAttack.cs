using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;


public class ActionGuardianShisuiEmergencyAttack : ActionGuardianBase {
	private Vector2					mTeleportRatio		= Vector2.zero;
	private ParticleSystem			mEffectTo8Sec		= null;
	private ParticleSystem			mEffectTo10Sec		= null;

	private ActionParamForCallback	mBuffEndCallback	= null;
	private float					mOriginMass			= 0.0f;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		actionCommand = eActionCommand.EmergencyAttack;

		conditionActionCommand = new eActionCommand[1];
		conditionActionCommand[0] = eActionCommand.Defence;

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.EmergencyAttack;

		mTeleportRatio = new Vector2( 1.5f, 2.5f );

		mEffectTo8Sec = GameSupport.CreateParticle( "Effect/Character/prf_fx_Shisui_ATK_evadecounter_02_Shield_8sec.prefab", mPlayerGuardian.OwnerPlayer.transform );
		mEffectTo10Sec = GameSupport.CreateParticle( "Effect/Character/prf_fx_Shisui_ATK_evadecounter_02_Shield_10sec.prefab", mPlayerGuardian.OwnerPlayer.transform );

		mEffectTo8Sec.transform.rotation = Quaternion.Euler( new Vector3( 270.0f, 0.0f, 0.0f ) );
		mEffectTo10Sec.transform.rotation = Quaternion.Euler( new Vector3( 270.0f, 0.0f, 0.0f ) );

		mBuffEndCallback = new ActionParamForCallback( OnBuffEndCallback );

		mOriginMass = m_owner.rigidBody.mass;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		ShowSkillNames( m_data );
	}

	public override IEnumerator UpdateAction() {
		if ( mPlayerGuardian != null ) {
			yield return mPlayerGuardian.TeleportGuardian( PlayerGuardian.eArrowType.UP_BACK, mTeleportRatio, false );
		}

		m_checkTime = 0.0f;
		m_aniLength = m_owner.PlayAniImmediate( eAnimation.EvadeCounter );

		m_owner.alwaysKinematic = false;
		m_owner.rigidBody.isKinematic = false;
		m_owner.rigidBody.mass = 55000.0f;

		mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Buff;
		mBuffEvt.Set( TableId, eEventSubject.Self, eEventType.EVENT_BUFF_DMG_RATE_DOWN, mPlayerGuardian.OwnerPlayer, mValue1, 0.0f, 0.0f, mValue2, 0.0f, 0, 0, eBuffIconType.Buff_Def );
		EventMgr.Instance.SendEvent( mBuffEvt, null, mBuffEndCallback );

		if ( mValue2 > 8.0f ) {
			mEffectTo10Sec.gameObject.SetActive( true );
		}
		else {
			mEffectTo8Sec.gameObject.SetActive( true );
		}

		while ( m_endUpdate == false ) {
			m_checkTime += Time.fixedDeltaTime;

			if ( m_checkTime >= m_aniLength ) {
				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}

		m_owner.rigidBody.mass = mOriginMass;
	}

	private void OnBuffEndCallback() {
		mEffectTo8Sec.gameObject.SetActive( false );
		mEffectTo10Sec.gameObject.SetActive( false );
	}
}
