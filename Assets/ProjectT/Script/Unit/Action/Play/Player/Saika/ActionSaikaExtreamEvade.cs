using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSaikaExtreamEvade : ActionExtreamEvade {
	private ParticleSystem mEffectParticleSystem = null;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		mEffectParticleSystem = GameSupport.CreateParticle( "Effect/Character/prf_fx_saika_attack_evade_eye.prefab", null );
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		StartEvadingBuff();
	}

	private void StartEvadingBuff() {
		mOwnerPlayer.ExtreamEvading = true;

		StopCoroutine( "EndEvading" );
		StartCoroutine( "EndEvading", mValue1 );
	}

	public override void OnEnd() {
		base.OnEnd();
		IsSkillEnd = false;
	}

	private IEnumerator EndEvading( float duration ) {
		StartCoroutine( nameof( EffectPlay ) );

		yield return StartCoroutine( ContinueDash() );

		yield return new WaitForSeconds( duration );

		mOwnerPlayer.ExtreamEvading = false;
		IsSkillEnd = true;
	}

	private IEnumerator EffectPlay() {
		mEffectParticleSystem.gameObject.SetActive( true );

		float waitTime = 0.0f;

		while ( waitTime < mEffectParticleSystem.main.duration ) {
			waitTime += Time.fixedDeltaTime;

			mEffectParticleSystem.transform.position = m_owner.GetCenterPos() + Vector3.up * 1.5f;
			
			yield return mWaitForFixedUpdate;
		}

		mEffectParticleSystem.gameObject.SetActive( false );
	}
}
