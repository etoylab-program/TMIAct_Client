
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionNoahUSkill : ActionUSkillBase
{
	protected override void LoadEffect()
    {
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        mTableId = 14301;
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

		if( !mOwnerPlayer.IsHelper ) {
			World.Instance.UIPlay.EffAtk.gameObject.SetActive( false );
		}

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			m_owner.PlayAniImmediate( eAnimation.USkillEnd, 0.0f, true );
		}

		if( cancelCurAction ) {
			m_owner.actionSystem.CancelCurrentAction();
		}

		mOwnerPlayer.transform.localScale = Vector3.one;
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
		}

		m_owner.LookAtTarget( mCenterPos );

		ActionComboAttack actionComboAtk = mOwnerPlayer.actionSystem.GetAction<ActionComboAttack>(eActionCommand.Attack01);
		if( actionComboAtk ) {
			actionComboAtk.DontUse = true;
		}

		m_owner.ShowMesh( true );
		m_owner.StopPauseFrame();

		// Idle 애니 변경
		ActionIdle actionIdle = mOwnerPlayer.actionSystem.GetAction<ActionIdle>(eActionCommand.Idle);
		if( actionIdle == null ) {
			Debug.LogError( "노아 Idle 액션이 왜 없지???" );
		}
		else {
			actionIdle.defaultAni = eAnimation.USkillIdle;
		}

		m_owner.PlayAniImmediate( eAnimation.USkillIdle );
		mOwnerPlayer.transform.localScale = Vector3.one * 2.8f;

		yield return new WaitForSeconds( 0.1f );

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

		BattleAreaManager battleAreaMgr = World.Instance.EnemyMgr as BattleAreaManager;
		bool skipLastAttck = false;

		m_checkTime = 0.0f;

		while( m_checkTime < mParam.Value2 ) {
			if( World.Instance.IsEndGame || World.Instance.ProcessingEnd || ( battleAreaMgr && battleAreaMgr.IsPortalOrPathOpen() ) ) {
				skipLastAttck = true;
				break;
			}

			if( !World.Instance.IsPause && !Director.IsPlaying && !mOwnerPlayer.Input.isPause ) {
				m_checkTime += Time.fixedDeltaTime;

				if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
					World.Instance.UIPlay.UpdateUSkillCoolTime( mOwnerPlayer, m_checkTime, mParam.Value2 );
				}
			}

			yield return mWaitForFixedUpdate;
		}

		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			World.Instance.UIPlay.ShowUSkillCoolTime( mOwnerPlayer, false );
		}

		if( !mOwnerPlayer.IsHelper ) {
			World.Instance.UIPlay.EffAtk.gameObject.SetActive( false );
		}

		if( !World.Instance.IsEndGame ) {
			if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
				mOwnerPlayer.Input.Pause( true );
			}

			m_owner.actionSystem.CancelCurrentAction();

			if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
				m_owner.StopBT();
			}

			LookAtTarget();

			if( !skipLastAttck ) {
				yield return new WaitForSeconds( mOwnerPlayer.PlayAniImmediate( eAnimation.USkillAttack03 ) );
			}

			m_aniLength = m_owner.PlayAniImmediate( eAnimation.USkillEnd, 0.0f, true );

			if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
				World.Instance.EnemyMgr.PauseAll( m_owner );

				m_checkTime = 0.0f;
				while( m_checkTime < m_aniLength ) {
					m_checkTime += m_owner.fixedDeltaTime;
					yield return mWaitForFixedUpdate;
				}

				Vector3 scale = mOwnerPlayer.transform.localScale;

				m_checkTime = 0.0f;
				while( mOwnerPlayer.transform.localScale.x > 1.0f ) {
					scale.x = Mathf.Clamp( scale.x - ( 7.0f * m_owner.deltaTime ), 1.0f, 2.8f );
					scale.y = Mathf.Clamp( scale.y - ( 7.0f * m_owner.deltaTime ), 1.0f, 2.8f );
					scale.z = Mathf.Clamp( scale.z - ( 7.0f * m_owner.deltaTime ), 1.0f, 2.8f );

					mOwnerPlayer.transform.localScale = scale;
					yield return mWaitForFixedUpdate;
				}
			}
		}

		mOwnerPlayer.transform.localScale = Vector3.one;
		EndUSkillMode();
	}

	private void TransformUSkillMode()
    {
        mOwnerPlayer.TemporaryInvincible = true;
		mOwnerPlayer.SetTemporarySpeedRate(-0.3f);

		/*
		m_owner.unitBuffStats.AddBuffStat(eBuffDebuffType.Buff, TableId, UnitBuffStats.eBuffStatType.Speed, UnitBuffStats.eIncreaseType.Decrease, 0.3f,
										  -1, BattleOption.eToExecuteType.Temporary, BattleOption.eBOConditionType.None, BattleOption.eBOConditionType.None,
										  BattleOption.eBOAtkConditionType.UltimateSkill, TableId, false);

		m_owner.SetSpeedRateByCalcBuff(BattleOption.eToExecuteType.Temporary, false, true);
		*/

		// 콤보 어택 애니 변경
		ActionComboAttack actionComboAtk = mOwnerPlayer.actionSystem.GetAction<ActionComboAttack>(eActionCommand.Attack01);
        if (actionComboAtk == null)
        {
            Debug.LogError("노아 콤보어택 액션이 왜 없지???");
            return;
        }
        else
        {
            actionComboAtk.DontUse = false;

            actionComboAtk.attackAnimations = new eAnimation[6];
            actionComboAtk.attackAnimations[0] = eAnimation.USkillAttack01;
            actionComboAtk.attackAnimations[1] = eAnimation.USkillAttack02;
            actionComboAtk.attackAnimations[2] = eAnimation.USkillAttack01;
            actionComboAtk.attackAnimations[3] = eAnimation.USkillAttack02;
            actionComboAtk.attackAnimations[4] = eAnimation.USkillAttack01;
            actionComboAtk.attackAnimations[5] = eAnimation.USkillAttack02;

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

        // 이동 애니 변경
        ActionMoveByDirection actionMove = mOwnerPlayer.actionSystem.GetAction<ActionMoveByDirection>(eActionCommand.MoveByDirection);
        if(actionMove == null)
        {
            Debug.LogError("노아 이동 액션이 왜 없지???");
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
			Debug.LogError( "노아 콤보어택 액션이 왜 없지???" );
			return;
		}
		else {
			actionComboAtk.RestoreAttackAnimations();
		}

		// Idle 애니 변경
		ActionIdle actionIdle = mOwnerPlayer.actionSystem.GetAction<ActionIdle>(eActionCommand.Idle);
		if( actionIdle == null ) {
			Debug.LogError( "노아  Idle 액션이 왜 없지???" );
			return;
		}
		else {
			actionIdle.RestoreDefaultAni();
		}

		// 이동 애니 변경
		ActionMoveByDirection actionMove = mOwnerPlayer.actionSystem.GetAction<ActionMoveByDirection>(eActionCommand.MoveByDirection);
		if( actionMove == null ) {
			Debug.LogError( "노아 이동 액션이 왜 없지???" );
			return;
		}
		else {
			actionMove.RestoreMoveAni();
			actionMove.SkipStopAni = false;
		}

		mOwnerPlayer.ExtreamEvading = false;
		m_owner.PlayAni( eAnimation.Idle01 );
	}

	private void LookAtTarget()
    {
        Unit target = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
        if (target)
        {
            m_owner.LookAtTarget(target.transform.position);
        }
    }
}
