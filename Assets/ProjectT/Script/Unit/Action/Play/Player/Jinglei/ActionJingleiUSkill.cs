
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionJingleiUSkill : ActionUSkillBase
{

	protected override void LoadEffect()
    {
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 6301;
        base.Init(tableId, listAddCharSkillParam);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mOwnerPlayer.UsingUltimateSkill = true;
        mOwnerPlayer.cmptBuffDebuff.RemoveSuperArmorBuff(0);
		mOwnerPlayer.cmptBuffDebuff.RemoveSpeedUpBuff();
    }

	public override void ForceEnd( bool cancelCurAction ) {
		StopCoroutine( "CheckTransformTime" );

		EffectManager.Instance.StopEffImmediate( 30011, EffectManager.eType.Common, null );
		EffectManager.Instance.StopEffImmediate( 30012, EffectManager.eType.Common, null );
		EffectManager.Instance.StopEffImmediate( 30013, EffectManager.eType.Common, null );
		EffectManager.Instance.StopEffImmediate( 30014, EffectManager.eType.Common, null );

		if( !mOwnerPlayer.IsHelper ) {
			World.Instance.UIPlay.EffAtk.gameObject.SetActive( false );
		}

		if( cancelCurAction ) {
			m_owner.actionSystem.CancelCurrentAction();
		}

		EndUSkillMode();
	}

	protected override IEnumerator EndUSkill() {
		mBattleAreaMgr = World.Instance.EnemyMgr as BattleAreaManager;
		if( mBattleAreaMgr ) {
			m_owner.SetInitialPosition( mOwnerPos, mOwnerRot );
		}

		mCenterPos = World.Instance.EnemyMgr.GetCenterPosOfEnemies( m_owner );
		mCenterPos.y = m_owner.transform.position.y;

		if( World.Instance.InGameCamera.Mode == InGameCamera.EMode.DEFAULT ) {
			World.Instance.InGameCamera.SetUserSetting( new Vector3( 0.0f, 3.0f, -6.0f ), new Vector2( 0.0f, 2.0f ), mCenterPos );
		}

		World.Instance.EnemyMgr.PauseAll( m_owner );
		World.Instance.SkipControllerPauseInWorldPause = true;

		for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
			World.Instance.ListPlayer[i].Input.Pause( true );
			World.Instance.ListPlayer[i].ShowMesh( false );
		}

		m_owner.LookAtTarget( mCenterPos );

		ActionComboAttack actionComboAtk = mOwnerPlayer.actionSystem.GetAction<ActionComboAttack>(eActionCommand.Attack01);
		if( actionComboAtk ) {
			actionComboAtk.DontUse = true;
		}

		yield return new WaitForSeconds( 0.1f );

		EffectManager.Instance.Play( m_owner, 30011, EffectManager.eType.Common, 0.0f, true );
		EffectManager.Instance.Play( m_owner, 30012, EffectManager.eType.Common, 0.0f, true );
		EffectManager.Instance.Play( m_owner, 30013, EffectManager.eType.Common, 0.0f, true );
		EffectManager.Instance.Play( m_owner, 30014, EffectManager.eType.Common, 0.0f, true );

		m_owner.ShowMesh( true );
		m_owner.StopPauseFrame();
		m_owner.PlayAniImmediate( eAnimation.USkillIdle );

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			while( World.Instance.UIPlay.widgets[0].alpha < 1.0f ) {
				yield return null;
			}
		}

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			World.Instance.UIPlay.ShowUSkillCoolTime( mOwnerPlayer, true );
		}

		yield return new WaitForSeconds( 0.1f );

		if( World.Instance.InGameCamera.Mode == InGameCamera.EMode.DEFAULT ) {
			World.Instance.InGameCamera.EndUserSetting();
		}

		for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
			World.Instance.ListPlayer[i].Input.Pause( false );
			World.Instance.ListPlayer[i].SetKinematicRigidBody( false );
		}

		if( !mOwnerPlayer.IsHelper ) {
			World.Instance.UIPlay.EffAtk.gameObject.SetActive( true );
		}

		World.Instance.EnemyMgr.ResumeAll();
		ResetBT();

		StopCoroutine( "CheckTransformTime" );
		StartCoroutine( "CheckTransformTime" );
	}

	private IEnumerator CheckTransformTime() {
		TransformUSkillMode();
		World.Instance.UIPlay.UpdateUSkillCoolTime( mOwnerPlayer, 0.0f, mParam.Value2 );

		m_checkTime = 0.0f;
		while( m_checkTime < mParam.Value2 ) {
			if( World.Instance.IsEndGame ) {
				break;
			}

			if( !World.Instance.IsPause && !Director.IsPlaying ) {
				m_checkTime += m_owner.deltaTime;

				if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
					World.Instance.UIPlay.UpdateUSkillCoolTime( mOwnerPlayer, m_checkTime, mParam.Value2 );
				}
			}

			yield return null;
		}

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			World.Instance.UIPlay.ShowUSkillCoolTime( mOwnerPlayer, false );
		}

		EffectManager.Instance.StopEffImmediate( 30011, EffectManager.eType.Common, null );
		EffectManager.Instance.StopEffImmediate( 30012, EffectManager.eType.Common, null );
		EffectManager.Instance.StopEffImmediate( 30013, EffectManager.eType.Common, null );
		EffectManager.Instance.StopEffImmediate( 30014, EffectManager.eType.Common, null );

		if( !mOwnerPlayer.IsHelper ) {
			World.Instance.UIPlay.EffAtk.gameObject.SetActive( false );
		}

		if( !World.Instance.IsEndGame ) {
			m_owner.actionSystem.CancelCurrentAction();

			if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
				m_aniLength = m_owner.PlayAniImmediate( eAnimation.USkillEnd, 0.0f, true );
				mOwnerPlayer.Input.Pause( true );
				
				World.Instance.EnemyMgr.PauseAll( m_owner );

				m_checkTime = 0.0f;
				while( m_checkTime < m_aniLength ) {
					m_checkTime += m_owner.fixedDeltaTime;
					yield return null;
				}
			}
		}

		EndUSkillMode();
	}

	private void TransformUSkillMode()
    {
        mOwnerPlayer.TemporaryInvincible = true;
		mOwnerPlayer.SetTemporarySpeedRate(0.5f);

		// 콤보 어택 애니 변경
		ActionComboAttack actionComboAtk = mOwnerPlayer.actionSystem.GetAction<ActionComboAttack>(eActionCommand.Attack01);
        if (actionComboAtk == null)
        {
            Debug.LogError("진레이 콤보어택 액션이 왜 없지???");
            return;
        }
        else
        {
            actionComboAtk.DontUse = false;

            actionComboAtk.attackAnimations = new eAnimation[4];
            actionComboAtk.attackAnimations[0] = eAnimation.USkillAttack01;
            actionComboAtk.attackAnimations[1] = eAnimation.USkillAttack02;
            actionComboAtk.attackAnimations[2] = eAnimation.USkillAttack03;
            actionComboAtk.attackAnimations[3] = eAnimation.USkillAttack04;

            float uSkillDamage = mOwnerPlayer.GetUltimateSkillDefaultAtkPower();
            List<Unit2ndStatsTable.sOption> findAll = m_owner.unit2ndStats.FindStat(Unit2ndStatsTable.eSubjectType.USkillAtk, Unit2ndStatsTable.eIncreaseType.Damage);
            if (findAll != null)
            {
                for (int i = 0; i < findAll.Count; i++)
                {
                    uSkillDamage += (uSkillDamage * (findAll[i].value / (float)eCOUNT.MAX_RATE_VALUE));
                }
            }

            for (int i = 0; i < actionComboAtk.attackAnimations.Length; i++)
            {
                List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent(actionComboAtk.attackAnimations[i]);
                for (int j = 0; j < listAtkEvt.Count; j++)
                {
                    listAtkEvt[j].atkRatio = (uSkillDamage * mParam.Value1) / mOwnerPlayer.attackPower;
                }
            }
        }

        if(World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
        {
			for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
				World.Instance.ListPlayer[i].Input.LockBtnFlag( InputController.ELockBtnFlag.NONE );
			}
        }

        // Idle 애니 변경
        ActionIdle actionIdle = mOwnerPlayer.actionSystem.GetAction<ActionIdle>(eActionCommand.Idle);
        if(actionIdle == null)
        {
            Debug.LogError("진레이 Idle 액션이 왜 없지???");
            return;
        }
        else
        {
            actionIdle.defaultAni = eAnimation.USkillIdle;
        }

        // 이동 애니 변경
        ActionMoveByDirection actionMove = mOwnerPlayer.actionSystem.GetAction<ActionMoveByDirection>(eActionCommand.MoveByDirection);
        if(actionMove == null)
        {
            Debug.LogError("진레이 이동 액션이 왜 없지???");
            return;
        }
        else
        {
            actionMove.moveAni = eAnimation.USkillRun;
            actionMove.walkAni = eAnimation.USkillRun;

            actionMove.SkipStopAni = true;
        }
    }

	private void EndUSkillMode() {
		if( World.Instance.IsEndGame ) {
			return;
		}

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			World.Instance.UIPlay.ShowUSkillCoolTime( mOwnerPlayer, false );
		}

		mOwnerPlayer.TemporaryInvincible = false;
		mOwnerPlayer.UsingUltimateSkill = false;
		mOwnerPlayer.SetTemporarySpeedRate( 0.0f );

		for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
			World.Instance.ListPlayer[i].Input.Pause( false );
		}

		World.Instance.EnemyMgr.ResumeAll();

		// 콤보 어택 애니 변경
		ActionComboAttack actionComboAtk = mOwnerPlayer.actionSystem.GetAction<ActionComboAttack>(eActionCommand.Attack01);
		if( actionComboAtk == null ) {
			Debug.LogError( "진레이 콤보어택 액션이 왜 없지???" );
			return;
		}
		else {
			actionComboAtk.RestoreAttackAnimations();
		}

		// Idle 애니 변경
		ActionIdle actionIdle = mOwnerPlayer.actionSystem.GetAction<ActionIdle>(eActionCommand.Idle);
		if( actionIdle == null ) {
			Debug.LogError( "진레이 Idle 액션이 왜 없지???" );
			return;
		}
		else {
			actionIdle.RestoreDefaultAni();
		}

		// 이동 애니 변경
		ActionMoveByDirection actionMove = mOwnerPlayer.actionSystem.GetAction<ActionMoveByDirection>(eActionCommand.MoveByDirection);
		if( actionMove == null ) {
			Debug.LogError( "진레이 이동 액션이 왜 없지???" );
			return;
		}
		else {
			actionMove.RestoreMoveAni();
			actionMove.SkipStopAni = false;
		}

		mOwnerPlayer.ExtreamEvading = false;
		m_owner.PlayAni( eAnimation.Idle01 );
	}
}
