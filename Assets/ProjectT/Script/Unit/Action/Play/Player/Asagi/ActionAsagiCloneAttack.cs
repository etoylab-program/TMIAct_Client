
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAsagiCloneAttack : ActionSelectSkillBase {

	private int			mCloneCount			= 5;
	private List<Unit>	mListClone			= new List<Unit>();
	private float		mCloneAtkAniLength	= 0.0f;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		actionCommand = eActionCommand.HoldingDefBtnAttack;

		extraCondition = new eActionCondition[4];
		extraCondition[0] = eActionCondition.Grounded;
		extraCondition[1] = eActionCondition.NoUsingSkill;
		extraCondition[2] = eActionCondition.NoUsingQTE;
		extraCondition[3] = eActionCondition.NoUsingUSkill;

		extraCancelCondition = new eActionCondition[3];
		extraCancelCondition[0] = eActionCondition.UseSkill;
		extraCancelCondition[1] = eActionCondition.UseQTE;
		extraCancelCondition[2] = eActionCondition.UseUSkill;

		superArmor = Unit.eSuperArmor.Lv1;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ShowSkillNames( m_data );

		float angle = 360.0f / mCloneCount;

		mListClone.Clear();
		for ( int i = 0; i < mCloneCount; i++ ) {
			int cloneIndex = m_owner.GetDeactivateCloneIndex( 0 );
			if ( cloneIndex == -1 ) {
				continue;
			}

			Quaternion r = Quaternion.Euler( 0.0f, angle * mListClone.Count, 0.0f );
			Vector3 pos = m_owner.transform.position + ( ( r * m_owner.transform.forward ) * 0.8f );

			Unit clone = m_owner.GetClone( cloneIndex );
			m_owner.ShowClone( cloneIndex, pos, Quaternion.LookRotation( pos - m_owner.transform.position ) );

			if ( mValue1 >= 1.0f ) {
				List<AniEvent.sEvent> list = clone.aniEvent.GetAllMeleeAttackEvent( eAnimation.CloneAttack01 );
				for ( int j = 0; j < list.Count; j++ ) {
					list[j].behaviour = eBehaviour.KnockBackAttack;
				}
			}

			mListClone.Add( clone );
		}

		if ( mListClone.Count <= 0 ) {
			m_endUpdate = true;
		}
	}

	public override IEnumerator UpdateAction() {
		if ( !m_endUpdate ) {
			m_aniLength = m_owner.PlayAniImmediate( eAnimation.EvadeCharge2 );
			m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( eAnimation.EvadeCharge2 );

			bool playCloneAttack = false;

			while ( !m_endUpdate ) {
				m_checkTime += Time.fixedDeltaTime;

				if ( !playCloneAttack ) {
					if ( m_checkTime >= m_aniCutFrameLength ) {
						for ( int i = 0; i < mListClone.Count; i++ ) {
							ActionCloneAttack action = mListClone[i].actionSystem.GetAction<ActionCloneAttack>( eActionCommand.CloneAttack );
							if ( action ) {
								action.SetTableId( TableId );
							}

							mListClone[i].CommandAction( eActionCommand.CloneAttack, null );
							mCloneAtkAniLength = mListClone[i].aniEvent.GetAniLength( eAnimation.CloneAttack01 );
						}

						playCloneAttack = true;

						StopCoroutine( "DelayedDeactivateClones" );
						StartCoroutine( "DelayedDeactivateClones" );
					}
				}
				else {
					if ( m_checkTime >= m_aniLength ) {
						m_endUpdate = true;
					}
				}

				yield return mWaitForFixedUpdate;
			}
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( eAnimation.EvadeCharge2 );
		if ( evt == null ) {
			Debug.LogError( "ChargeBrandish 공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( "ChargeBrandish Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}

	public override void OnCancel() {
		base.OnCancel();

		StopCoroutine( "DelayedDeactivateClones" );
		DeactivateClones();
	}

	private void DeactivateClones() {
		for ( int i = 0; i < mListClone.Count; i++ ) {
			Unit clone = mListClone[i];
			if ( !clone.IsActivate() ) {
				continue;
			}

			clone.StopPauseFrame();
			clone.StopBT();
			clone.Deactivate();
		}
	}

	private IEnumerator DelayedDeactivateClones() {
		yield return new WaitForSeconds( mCloneAtkAniLength );
		DeactivateClones();
	}
}
