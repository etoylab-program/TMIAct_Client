
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEnemyUseBattleOption : ActionEnemyBase
{
	[Header("[Animation]")]
	public eAnimation StartAni = eAnimation.None;

	[Header("[Hide Mesh On Start]")]
	public GameObject[] HideMeshes;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam )
	{
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.UseBattleOption;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		m_owner.ExecuteBattleOption( BattleOption.eBOTimingType.Use, 0, null );

		if ( superArmor == Unit.eSuperArmor.Invincible ) {
			m_owner.TemporaryInvincible = true;
		}

		if ( StartAni != eAnimation.None ) {
			m_aniLength = m_owner.PlayAniImmediate( StartAni );
		}

		if ( HideMeshes != null ) {
			for ( int i = 0; i < HideMeshes.Length; i++ ) {
				HideMeshes[i].SetActive( false );
			}
		}
	}

	public override IEnumerator UpdateAction()
	{
		while( m_checkTime < m_aniLength )
		{
			m_checkTime += m_owner.deltaTime;
			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnEnd()
	{
		base.OnEnd();
		m_owner.TemporaryInvincible = false;
	}

	public override void OnCancel()
	{
		base.OnEnd();
		m_owner.TemporaryInvincible = false;
	}
}
