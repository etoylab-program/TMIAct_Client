using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSoraExtreamEvade : ActionExtreamEvade {
	public int		PjtCount			{ get; private set; } = 0;
	public float	PjtAttackPowerRatio	{ get; private set; } = 0.0f;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		PjtCount = Mathf.RoundToInt( mValue1 );
		PjtAttackPowerRatio = mValue2;
	}

	public override void OnStart( IActionBaseParam param ) {
		BOCharSkill.ChangeBattleOptionDuration( BattleOption.eBOTimingType.DuringSkill, TableId, mValue1 );

		base.OnStart( param );
		StartEvadingBuff();
	}

	private void StartEvadingBuff() {
		float duration = 6.0f;
		mOwnerPlayer.ExtreamEvading = true;
		
		StopCoroutine( "EndEvadingBuff" );
		StartCoroutine( "EndEvadingBuff", duration );

		mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Buff;
		mBuffEvt.Set( m_data.ID, eEventSubject.Self, eEventType.EVENT_BUFF_SPEED_UP, m_owner, 0.2f, 0.0f, 0.0f, duration, 0.0f, 0, 0, eBuffIconType.Buff_Speed );
		EventMgr.Instance.SendEvent( mBuffEvt );
	}

	public override void OnEnd() {
		base.OnEnd();
		IsSkillEnd = false;
	}

	public bool OnProjectileHit( Unit hitTarget ) {
		if( mValue3 <= 0.0f ) {
			return false;
		}

		m_owner.AddHp( BattleOption.eToExecuteType.Unit, hitTarget.LastDamage * 0.03f, false );
		return true;
	}

	private IEnumerator EndEvadingBuff( float duration ) {
		yield return StartCoroutine( ContinueDash() );

		EffectManager.Instance.Play( m_owner, 30036, EffectManager.eType.Common, 0.0f, true );
		yield return new WaitForSeconds( duration );

		mOwnerPlayer.ExtreamEvading = false;
		IsSkillEnd = true;

		EffectManager.Instance.StopEffImmediate( 30036, EffectManager.eType.Common, null );
	}
}
