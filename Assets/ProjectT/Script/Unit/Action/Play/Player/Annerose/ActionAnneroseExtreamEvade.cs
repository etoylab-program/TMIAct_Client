
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAnneroseExtreamEvade : ActionExtreamEvade {
	public override void OnStart( IActionBaseParam param ) {
		BOCharSkill.ChangeBattleOptionDuration( BattleOption.eBOTimingType.DuringSkill, TableId, mValue1 );

		base.OnStart( param );
		StartEvadingBuff();
	}

	private void StartEvadingBuff() {
		mOwnerPlayer.ExtreamEvading = true;

		StopCoroutine( "EndEvadingBuff" );
		StartCoroutine( "EndEvadingBuff", mValue1 );

		mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Buff;
		mBuffEvt.Set( m_data.ID, eEventSubject.Self, eEventType.EVENT_BUFF_CRITICAL_RATE_UP, m_owner, 0.3f + mValue3, 0.0f, 0.0f, mValue1, 0.0f, 0, 0, eBuffIconType.Buff_Cri );
		EventMgr.Instance.SendEvent( mBuffEvt );

		mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Buff;
		mBuffEvt.Set( m_data.ID + 1, eEventSubject.Self, eEventType.EVENT_BUFF_ATKPOWER_RATE_UP, m_owner, 0.2f, 0.0f, 0.0f, mValue1, 0.0f, 0, 0, eBuffIconType.Buff_Atk );
		EventMgr.Instance.SendEvent( mBuffEvt );

		mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Buff;
		mBuffEvt.Set( m_data.ID + 2, eEventSubject.Self, eEventType.EVENT_BUFF_DMG_RATE_DOWN, m_owner, 0.2f + mValue2, 0.0f, 0.0f, mValue1, 0.0f, 0, 0, eBuffIconType.Buff_Def );
		EventMgr.Instance.SendEvent( mBuffEvt );
	}

	public override void OnEnd() {
		base.OnEnd();
		IsSkillEnd = false;
	}

	private IEnumerator EndEvadingBuff( float duration ) {
		yield return StartCoroutine( ContinueDash() );

		GameObject parent = m_owner.aniEvent.gameObject;

		Transform bone = m_owner.aniEvent.GetBoneByName( "Bip001" );
		if( bone ) {
			parent = bone.gameObject;
		}

		int effId = 30038;
		if( duration > 4.0f ) {
			effId = 30039;
		}

		EffectManager.Instance.Play( parent, effId, EffectManager.eType.Common );
		ParticleSystem ps = EffectManager.Instance.GetEffectOrNull( effId, EffectManager.eType.Common, EffectManager.eParticleFindType.Active );
		if( ps ) {
			Utility.InitTransform( ps.gameObject, new Vector3( 0.00257f, 0.23f, -0.96f ), Quaternion.Euler( -20.4f, -83.2f, -102.6f ), Vector3.one );
		}

		yield return new WaitForSeconds( duration );

		mOwnerPlayer.ExtreamEvading = false;
		IsSkillEnd = true;

		EffectManager.Instance.StopEffImmediate( effId, EffectManager.eType.Common, null );
	}
}
