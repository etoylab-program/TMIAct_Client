
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSakuraExtreamEvadeSummon : ActionExtreamEvade {
	public class sActiveMinionInfo {
		public PlayerMinion	Minion;
		public Coroutine	Cr;
	}


	private int[]                   mArrEnemyTableId    = new int[] {10, 11, 12, 13};
	private List<PlayerMinion>      mListMinion         = new List<PlayerMinion>();
	private List<sActiveMinionInfo> mListActiveMinion   = new List<sActiveMinionInfo>();
	private int                     mMaxMinionCount     = 3;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.ExtreamEvade;

		for( int i = 0; i < mMaxMinionCount; i++ ) {
			PlayerMinion minion = GameSupport.CreatePlayerMinion(mArrEnemyTableId[(int)mValue1], mOwnerPlayer);
			mListMinion.Add( minion );
		}

		mListActiveMinion.Clear();
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		StartSummon();
	}

	public override void OnEnd() {
		base.OnEnd();
		IsSkillEnd = false;
	}

	private void StartSummon() {
		PlayerMinion minion = GetInactiveMinionOrNull();
		if( minion ) {
			mOwnerPlayer.ExtreamEvading = true;

			if( mOwnerPlayer.gameObject.layer == (int)eLayer.Player ) {
				Utility.SetLayer( minion.gameObject, (int)eLayer.PlayerClone, true );
			}
			else {
				Utility.SetLayer( minion.gameObject, (int)eLayer.EnemyClone, true );
			}

			minion.SetInitialPosition( mOwnerPlayer.transform.position, mOwnerPlayer.transform.rotation );
			minion.SetMinionAttackPower( mOwnerPlayer.gameObject.layer, mOwnerPlayer.attackPower * mValue2 );
			minion.Activate();
			minion.StopBT();

			if( SetAddAction ) {
				minion.SetSpeedRate( AddActionValue1 );
			}

			minion.PlayAniImmediate( eAnimation.Appear );
			minion.StartDissolve( 0.5f, true, new Color( 0.169f, 0.0f, 0.47f ) );

			sActiveMinionInfo info = new sActiveMinionInfo();
			info.Minion = minion;
			info.Cr = World.Instance.StartCoroutine( EndSummon( info, GameInfo.Instance.BattleConfig.BuffDuration * 3.0f ) );

			mListActiveMinion.Add( info );
		}
	}

	private IEnumerator EndSummon( sActiveMinionInfo activeMinionInfo, float duration ) {
		yield return StartCoroutine( ContinueDash() );
		activeMinionInfo.Minion.ResetBT();

		if( mListActiveMinion.Count < mMaxMinionCount ) {
			mOwnerPlayer.ExtreamEvading = false;
			IsSkillEnd = true;
		}

		m_endUpdate = false;
		m_checkTime = 0.0f;

		while( !m_endUpdate ) {
			m_checkTime += Time.fixedDeltaTime;
			if( mOwnerPlayer.curHp <= 0.0f || m_checkTime >= duration || World.Instance.IsEndGame || World.Instance.ProcessingEnd ) {
				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}

		activeMinionInfo.Minion.StopBT();

		if( !World.Instance.IsEndGame && !World.Instance.ProcessingEnd ) {
			float aniLength = activeMinionInfo.Minion.PlayAniImmediate(eAnimation.Die);
			activeMinionInfo.Minion.StartDissolve( aniLength, false, new Color( 0.169f, 0.0f, 0.47f ) );

			yield return new WaitForSeconds( aniLength );
		}

		activeMinionInfo.Minion.Deactivate();
		mListActiveMinion.Remove( activeMinionInfo );

		mOwnerPlayer.ExtreamEvading = false;
		IsSkillEnd = true;
	}

	private PlayerMinion GetInactiveMinionOrNull() {
		for( int i = 0; i < mListMinion.Count; i++ ) {
			if( !mListMinion[i].IsActivate() ) {
				return mListMinion[i];
			}
		}

		return null;
	}
}
