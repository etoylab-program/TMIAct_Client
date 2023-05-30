
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ActionDie : ActionEnemyBase
{
    private bool        mHasDirector    = false;
    private eAnimation  mCurAni         = eAnimation.Die;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Die;

        mOwnerEnemy = m_owner as Enemy;
        SetNextAction(eActionCommand.None, null);
    }

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_REMOVE_HIT_TARGET, m_owner );

		m_owner.SetDie();
		m_owner.MainCollider.Enable( false );

		if ( m_owner.isGrounded ) {
			m_owner.SetDieRigidbody();
		}
		else {
			m_owner.SetFallingRigidBody();
		}

		if ( m_owner.beforeChangeUnit ) {
			m_owner.beforeChangeUnit.SetDie();
			m_owner.beforeChangeUnit.Deactivate();
		}

		if ( mOwnerEnemy.ChangeEnemy ) {
			BattleAreaManager battleAreaMgr = World.Instance.EnemyMgr as BattleAreaManager;
			if ( battleAreaMgr ) {
				BattleArea battleArea = battleAreaMgr.GetCurrentBattleArea();
				if ( battleArea ) {
					mOwnerEnemy.SetInitialPosition( battleArea.GetCenterPos(), transform.rotation );
				}
			}

			HUDResMgr.Instance.ChangeParent( mOwnerEnemy, mOwnerEnemy.ChangeEnemy );
		}

		float directorDuration = 0.0f;
		if ( mOwnerEnemy.grade == Unit.eGrade.Boss && World.Instance.HasDirector( "BossDie" ) ) {
			mHasDirector = true;
			directorDuration = World.Instance.GetDirectorDuration( "BossDie" );
		}
		else {
			mHasDirector = m_owner.HasDirector( "BossDie" );
			if ( mHasDirector ) {
				directorDuration = m_owner.GetDirectorDuration( "BossDie" );
			}
		}

		if ( !mHasDirector ) {
			if ( m_owner.child ) {
				m_owner.child.actionSystem.CancelCurrentAction();
				m_owner.child.CommandAction( eActionCommand.Die, null );
			}

			if ( m_owner.aniEvent.HasAni( eAnimation.Die ) ) {
				mCurAni = eAnimation.Die;
				if ( param != null ) {
					ActionParamDie dieParam = param as ActionParamDie;
					if ( dieParam != null && dieParam.state == ActionParamDie.eState.Down )
						mCurAni = eAnimation.DownDie;
				}
			}
			else {
				mCurAni = eAnimation.None;
			}

			if ( mCurAni != eAnimation.None ) {
				m_aniLength = m_owner.PlayAniImmediate( mCurAni );
			}
		}
		else {
			m_owner.PlayAniImmediate( eAnimation.Idle01 );
			mCurAni = eAnimation.None;

			if ( m_owner.child ) {
				m_owner.child.SetDie();
			}

			if ( !World.Instance.HasDirector( "BossDie" ) ) {
				Director director = m_owner.GetDirector( "BossDie" );
				if ( director.listAnimationActorInfo.Count > 0 ) {
					Utility.SetLayer( m_owner.gameObject, (int)eLayer.Default, true, (int)eLayer.Wall );
				}

				m_owner.checkRayCollision = false;
			}
		}

		if ( m_owner.grade == Unit.eGrade.Boss && mOwnerEnemy.ChangeEnemy == null ) {
			EndGameEvent evt = new EndGameEvent();
			evt.Set( eEventType.EVENT_GAME_ENEMY_BOSS_DIE, directorDuration );
			EventMgr.Instance.SendEvent( evt );
		}
		else if ( m_owner.grade < Unit.eGrade.Boss ) {
			BaseEvent evt = new BaseEvent();
			evt.eventType = eEventType.EVENT_GAME_ENEMY_DEAD;
			evt.eventSubject = eEventSubject.World;
			EventMgr.Instance.SendEvent( evt );
		}
	}

	public override IEnumerator UpdateAction() {
		if ( mCurAni != eAnimation.None ) {
			m_checkTime = 0.0f;

			while ( m_checkTime < m_aniLength ) {
				m_checkTime += Time.deltaTime;
				yield return null;
			}

			mOwnerEnemy.ShowShadow( false );
		}
		else {
			if ( mOwnerEnemy.ChangeEnemy == null ) {
				if ( !mHasDirector ) {
					yield return new WaitForSeconds( 2.0f * Time.timeScale );
				}
			}
			else {
				mOwnerEnemy.Deactivate();
			}

			World.Instance.InGameCamera.ExcludCullingMask( (int)eLayer.Player );

			if ( m_owner.cmptBuffDebuff != null ) {
				m_owner.cmptBuffDebuff.Clear();
			}

			m_owner.RestoreSpeed();

			if ( World.Instance.HasDirector( "BossDie" ) ) {
				World.Instance.PlayDirector( "BossDie", EndBossDie );
			}
			else {
				m_owner.PlayDirector( "BossDie", EndBossDie );
			}
		}
	}

	public override void OnEnd()
    {
        isPlaying = false;
        m_curCancelDuringSameActionCount = 0;

        if (GameSupport.IsInGameTutorial() && GameInfo.Instance.UserData.GetTutorialState() == (int)eTutorialState.TUTORIAL_STATE_Stage3Clear && GameInfo.Instance.UserData.GetTutorialStep() == 1)
        {
            GameInfo.Instance.UserData.SetTutorial(GameInfo.Instance.UserData.GetTutorialState(), 2);
            GameUIManager.Instance.HideUI("SkillTrainingPanel", false);

            World.Instance.ShowTutorialHUD(2, 5.0f);
            World.Instance.ActiveEnemyMgrOnTutorial();
        }

        if (!mHasDirector && OnEndCallback != null)
            OnEndCallback();
    }

	private void EndBossDie() {
        if( m_owner == null || m_owner.gameObject == null || mOwnerEnemy == null ) {
            OnEndCallback?.Invoke();
            return;
		}

		World.Instance.IsEndBossDie = false;

		Utility.SetLayer( m_owner.gameObject, (int)eLayer.Enemy, true, (int)eLayer.Wall );
		m_owner.checkRayCollision = true;

		if ( mOwnerEnemy.ChangeEnemy == null ) {
			World.Instance.IsEndBossDie = true;

			mOwnerEnemy.ShowShadow( false );
            OnEndCallback?.Invoke();

			if ( World.Instance.ClearMode == World.eClearMode.AllEnemyDead ) {
				mOwnerEnemy.Deactivate();
				EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_ALL_ENEMY_DEAD, null );
			}
        }
		else {
			World.Instance.SkipControllerPauseInWorldPause = false;

			for ( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
				if ( !World.Instance.ListPlayer[i].IsActivate() ) {
					continue;
				}

				if ( !World.Instance.ListPlayer[i].isGrounded ) {
					World.Instance.ListPlayer[i].transform.position = World.Instance.Player.posOnGround;
				}
			}

			mOwnerEnemy.ChangeEnemy.ShowMesh( true );
			mOwnerEnemy.ChangeEnemy.SetInitialPosition( mOwnerEnemy.transform.position, mOwnerEnemy.transform.rotation );
			mOwnerEnemy.ChangeEnemy.myBattleSpawnPoint = mOwnerEnemy.myBattleSpawnPoint;
			mOwnerEnemy.ChangeEnemy.SetGroundedRigidBody();

			mOwnerEnemy.ChangeEnemy.Activate();

			World.Instance.EnemyMgr.AddEnemy( mOwnerEnemy.ChangeEnemy );
			World.Instance.EnemyMgr.SpawnEnemy();

			World.Instance.ChangeBoss( mOwnerEnemy.ChangeEnemy );
		}
	}
}
