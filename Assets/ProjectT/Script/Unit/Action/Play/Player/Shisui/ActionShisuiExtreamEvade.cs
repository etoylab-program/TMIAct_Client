using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionShisuiExtreamEvade : ActionExtreamEvade {
	private ParticleSystem mEffectParticleSystem = null;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		mEffectParticleSystem = GameSupport.CreateParticle( "Effect/Character/prf_fx_Shisui_ATK_extremeevade.prefab", null );
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		StartEvadingBuff();
	}

	private void StartEvadingBuff() {
		mOwnerPlayer.ExtreamEvading = true;

		StopCoroutine( "EndEvading" );
		StartCoroutine( "EndEvading", 0.0f );
	}

	public override void OnEnd() {
		base.OnEnd();
		IsSkillEnd = false;
	}

	private IEnumerator EndEvading( float duration ) {
		StartCoroutine( nameof( EffectPlay ) );

		yield return StartCoroutine( ContinueDash() );

		yield return new WaitForSeconds( duration );

		mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Buff;
		mBuffEvt.Set( m_data.ID, eEventSubject.Self, eEventType.EVENT_BUFF_DOT_HEAL, m_owner, mValue2 / (float)eCOUNT.MAX_BO_FUNC_VALUE, 0.0f, 0.0f, mValue1, mValue3, 0, 0, eBuffIconType.Buff_HPPlus );
		EventMgr.Instance.SendEvent( mBuffEvt );

		mOwnerPlayer.ExtreamEvading = false;
		IsSkillEnd = true;
	}

	private IEnumerator EffectPlay() {
		mEffectParticleSystem.gameObject.SetActive( true );

		float waitTime = 0.0f;

		while ( waitTime < mEffectParticleSystem.main.duration ) {
			waitTime += Time.fixedDeltaTime;

			mEffectParticleSystem.transform.position = m_owner.GetCenterPos();
			
			yield return mWaitForFixedUpdate;
		}

		mEffectParticleSystem.gameObject.SetActive( false );
	}
}
