using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSoraEvadeChargeAttack : ActionSelectSkillBase {
	private Unit				mTarget			= null;
	private List<eAnimation>    mListCurAni		= new List<eAnimation>();
	private int					mCurAniIndex	= 0;
	private float				mDistance		= 4.0f;
	private bool				mPrepare		= false;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.HoldingDefBtnAttack;

		extraCondition = new eActionCondition[4];
		extraCondition[0] = eActionCondition.Grounded;
		extraCondition[1] = eActionCondition.NoUsingSkill;
		extraCondition[2] = eActionCondition.NoUsingQTE;
		extraCondition[3] = eActionCondition.NoUsingUSkill;

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		extraCancelCondition = new eActionCondition[3];
		extraCancelCondition[0] = eActionCondition.UseSkill;
		extraCancelCondition[1] = eActionCondition.UseQTE;
		extraCancelCondition[2] = eActionCondition.UseUSkill;

		superArmor = Unit.eSuperArmor.Lv1;

		mListCurAni.Clear();

		if( mValue1 <= 0.0f ) {
			mListCurAni.Add( eAnimation.EvadeCharge );
			mListCurAni.Add( eAnimation.EvadeCharge );
		}
		else {
			mListCurAni.Add( eAnimation.EvadeCharge_1 );
			mListCurAni.Add( eAnimation.EvadeCharge_1 );
			mListCurAni.Add( eAnimation.EvadeCharge_1 );
			mListCurAni.Add( eAnimation.EvadeCharge_1 );
		}

		mListCurAni.Add( eAnimation.EvadeCharge_2 );

		if( mValue2 > 0.0f ) {
			List<Projectile> list = m_owner.aniEvent.GetAllProjectile( eAnimation.EvadeCharge );
			for( int i = 0; i < list.Count; i++ ) {
				list[i].passEnemy = true;
			}

			list = m_owner.aniEvent.GetAllProjectile( eAnimation.EvadeCharge_1 );
			for( int i = 0; i < list.Count; i++ ) {
				list[i].passEnemy = true;
			}

			list = m_owner.aniEvent.GetAllProjectile( eAnimation.EvadeCharge_2 );
			for( int i = 0; i < list.Count; i++ ) {
				list[i].passEnemy = true;
			}
		}

		mCurAniIndex = 0;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ShowSkillNames( m_data );

		mCurAniIndex = 0;
		NextAttack();
	}

	public override IEnumerator UpdateAction() {
		float checkStepForwardTime = 0.0f;

		while( !m_endUpdate ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if( !mPrepare && m_checkTime >= m_aniCutFrameLength ) {
				if( mCurAniIndex >= mListCurAni.Count ) {
					m_endUpdate = true;
				}
				else {
					PrepareNextAttack();
				}
			}
			else if( mPrepare ) {
				checkStepForwardTime += m_owner.fixedDeltaTime;

				if( checkStepForwardTime >= 0.1f ) {
					NextAttack();
					checkStepForwardTime = 0.0f;
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnCancel() {
		base.OnCancel();
		m_owner.ShowMesh( true );
	}

	public override void OnEnd() {
		base.OnEnd();
		m_owner.ShowMesh( true );
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mListCurAni[0] );
		if( evt == null ) {
			Debug.LogError( mListCurAni[0].ToString() + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if( evt.visionRange <= 0.0f ) {
			Debug.LogError( mListCurAni[0].ToString() + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}

	private void PrepareNextAttack() {
		mPrepare = true;

		Vector3 dir = m_owner.Input.GetDirection() * mDistance;
		m_owner.StartStepForward( 0.1f, m_owner.transform.position + dir, null );
	}

	private void NextAttack() {
		mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
		if( mTarget ) {
			m_owner.LookAtTarget( mTarget.transform.position );
		}

		m_aniLength = m_owner.PlayAniImmediate( mListCurAni[mCurAniIndex++] );
		m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();

		m_checkTime = 0.0f;
		mPrepare = false;
	}
}
