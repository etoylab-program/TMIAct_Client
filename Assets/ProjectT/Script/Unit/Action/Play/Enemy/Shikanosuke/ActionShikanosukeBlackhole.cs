
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionShikanosukeBlackhole : ActionEnemyBase
{
	private Vector3				mBlackholePos	= Vector3.zero;
	private List<Unit>			mListTarget		= null;
	private float				mDuration		= 0.0f;
	private List<Projectile>	mListPjt		= null;


	public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
	{
		base.Init(tableId, listAddCharSkillParam);
		actionCommand = eActionCommand.ShikanosukeBlackhole;
	}

	public override void OnStart(IActionBaseParam param)
	{
		base.OnStart(param);

		mListPjt = m_owner.aniEvent.GetAllProjectile(eAnimation.Attack03);
		if (mListPjt.Count > 0)
		{
			mDuration = mListPjt[0].duration;

			m_owner.PlayAniImmediate(eAnimation.Attack03);
			m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength() + 0.1f;

			mDuration += m_aniCutFrameLength;

			StopCoroutine( "UpdateBlackhole" );
			StartCoroutine( "UpdateBlackhole" );
		}
	}

	public override IEnumerator UpdateAction()
	{
		float checkTime = 0.0f;
		while( !m_endUpdate ) {
			checkTime += m_owner.fixedDeltaTime;
			
			if( checkTime >= m_aniCutFrameLength ) {
				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnCancel()
	{
		base.OnCancel();

		m_checkTime = mDuration;
		m_endUpdate = true;
	}

	private IEnumerator UpdateBlackhole() {
		bool start = false;
		bool end = false;

		while( !end ) {
			m_checkTime += Time.fixedDeltaTime;

			if( m_checkTime >= mDuration || Director.IsPlaying || World.Instance.IsEndGame || World.Instance.ProcessingEnd ) {
				end = true;
			}
			else {
				if( !start && m_checkTime >= m_aniCutFrameLength ) {
					mListTarget = m_owner.GetEnemyList( true );
					for( int i = 0; i < mListTarget.Count; i++ ) {
						Unit target = mListTarget[i];
						if( target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback() ) {
							continue;
						}

						target.StopStepForward();
					}

					start = true;
					mBlackholePos = mListPjt[0].transform.position;
				}
				else if( start ) {
					for( int i = 0; i < mListTarget.Count; i++ ) {
						Unit target = mListTarget[i];
						if( target == null || target.MainCollider == null || target.curHp <= 0.0f || target.cmptMovement == null ) {
							continue;
						}

						if( target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback() ) {
							continue;
						}

						target.StopStepForward();

						Vector3 v = (mBlackholePos - target.transform.position).normalized;
						v.y = 0.0f;

						target.cmptMovement.UpdatePosition( v, Mathf.Max( GameInfo.Instance.BattleConfig.BlackholeMinSpeed, target.speed * 1.2f ), false );
					}
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}
}
