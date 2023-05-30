
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ActionUSkillBase : ActionBase
{
    protected Player                                mOwnerPlayer    = null;
    protected GameTable.CharacterSkillPassive.Param mParam          = null;
    protected int                                   mTableId        = 0;
    protected float                                 mHitTime        = 1.0f;
    protected Director                              mDirector       = null;
    protected Vector3                               mOwnerPos       = Vector3.zero;
    protected Quaternion                            mOwnerRot       = Quaternion.identity;
    protected BattleAreaManager                     mBattleAreaMgr  = null;
    protected int                                   mCameraEffId    = 0;
    protected int                                   mGroundEffId    = 0;
    protected Vector3                               mCenterPos      = Vector3.zero;
	protected AniEvent.sEvent						mAniEvt			= null;


    protected abstract void LoadEffect();

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, null);

        extraCondition = new eActionCondition[2];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;

        actionCommand = eActionCommand.USkill01;
        superArmor = Unit.eSuperArmor.None;

        mOwnerPlayer = m_owner as Player;
        mParam = GameInfo.Instance.GameTable.FindCharacterSkillPassive(mTableId);
		mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.Attack, 1, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f);

		LoadEffect();
    }

    public override void OnStart(IActionBaseParam param)
    {
        Debug.Log("ActionUSkillBase::OnStart");
        base.OnStart(param);

        m_owner.SetKinematicRigidBody(true);
        m_owner.UseSp(GameInfo.Instance.BattleConfig.USUseSP);
        m_owner.actionSystem2.CancelCurrentAction();
        m_owner.Input.Pause(true);
        m_owner.TemporaryInvincible = true;
        
        if(m_owner.AI)
        {
            m_owner.StopBT();
            Utility.SetLayer(m_owner.gameObject, (int)eLayer.Player, true, (int)eLayer.PostProcess);
        }

        mOwnerPos = Vector3.zero;
        mOwnerRot = Quaternion.identity;
        mBattleAreaMgr = null;

        World.Instance.InGameCamera.ResetPostProcess();

        if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
        {
            Player opponentPlayer = m_owner as Player;
            if (opponentPlayer && opponentPlayer.OpponentPlayer)
            {
                List<Unit> listTarget = World.Instance.EnemyMgr.GetActiveEnemies(m_owner);
                for (int i = 0; i < listTarget.Count; i++)
                {
                    Unit target = listTarget[i];
                    if (target == null || !target.IsActivate() || target.curHp <= 0.0f)
                    {
                        continue;
                    }

                    Utility.SetLayer(target.gameObject, (int)eLayer.Enemy, true, (int)eLayer.PostProcess);
                }
            }
        }
        else
        {
            mOwnerPlayer.SetMainTarget(null);

            for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                if( mOwnerPlayer == World.Instance.ListPlayer[i] ) {
                    continue;
                }

                World.Instance.ListPlayer[i].actionSystem.CancelCurrentAction();
                World.Instance.ListPlayer[i].StopBT();
            }
        }

        mOwnerPlayer.OnStartUSkill();
        mOwnerPlayer.UsingUltimateSkill = true;
    }

    public override IEnumerator UpdateAction()
    {
        World.Instance.EnemyMgr.PauseAll(m_owner);

        if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
        {
            m_owner.Input.LockBtnFlag(InputController.ELockBtnFlag.ATTACK | InputController.ELockBtnFlag.DASH | InputController.ELockBtnFlag.USKILL |
                                      InputController.ELockBtnFlag.WEAPON | InputController.ELockBtnFlag.SUPPORTER);
        }

        mBattleAreaMgr = World.Instance.EnemyMgr as BattleAreaManager;
        if (mBattleAreaMgr)
        {
            mOwnerPos = m_owner.transform.position;
            mOwnerRot = m_owner.transform.rotation;

            m_owner.SetInitialPosition(mBattleAreaMgr.GetCurrentBattleArea().transform.position, m_owner.transform.rotation);
        }

        mDirector = m_owner.GetDirector("USkill01");

		Player player = m_owner as Player;
		if ( player && player.Guardian ) {
			Utility.SetLayer( player.Guardian.gameObject, (int)eLayer.Player, true, (int)eLayer.PostProcess );
		}

		m_owner.PlayDirector("USkill01", EndDirector);

        while (!mDirector.isEnd)
        {
            Debug.Log("오의 연출 진행 중...");
            if (mDirector.isEnd)
            {
                Debug.Log("오의 연출 끝");
            }

            yield return null;
        }
    }

    public override void OnEnd()
    {
        isPlaying = false;
        m_curCancelDuringSameActionCount = 0;

        m_owner.ShowMesh(true);

        if (m_owner.curHp <= 0.0f)
        {
            ActionParamDie paramDie = null;

            ActionHit actionHit = m_owner.actionSystem.GetAction<ActionHit>(eActionCommand.Hit);
            if (actionHit && (actionHit.State == ActionHit.eState.Float || actionHit.State == ActionHit.eState.Down))
            {
                paramDie = new ActionParamDie(ActionParamDie.eState.Down, null);
            }

            SetNextAction(eActionCommand.Die, paramDie);
        }

        OnEndCallback?.Invoke();
    }

    public virtual void ForceEnd(bool cancelCurAction)
    {
    }

    protected void EndDirector()
    {
        StopCoroutine("EndUSkill");
        StartCoroutine("EndUSkill");
    }

    protected virtual IEnumerator EndUSkill()
    {
        mBattleAreaMgr = World.Instance.EnemyMgr as BattleAreaManager;
        if (mBattleAreaMgr)
        {
            m_owner.SetInitialPosition(mOwnerPos, mOwnerRot);
        }

        mCenterPos = World.Instance.EnemyMgr.GetCenterPosOfEnemies(m_owner);
        mCenterPos.y = m_owner.transform.position.y;

        if (World.Instance.InGameCamera.Mode == InGameCamera.EMode.DEFAULT)
        {
            //World.Instance.InGameCamera.PlayAnimation(mCameraAni, m_owner.gameObject, true);
            World.Instance.InGameCamera.SetUserSetting(new Vector3(0.0f, 3.0f, -6.0f), new Vector2(0.0f, 2.0f), mCenterPos);
        }

        World.Instance.EnemyMgr.PauseAll(m_owner);

        World.Instance.SkipControllerPauseInWorldPause = true;
        m_owner.Input.Pause(true);
        m_owner.LookAtTarget(mCenterPos);
        m_owner.SetKinematicRigidBody(false);

        float duration = EffectManager.Instance.Play(World.Instance.InGameCamera.gameObject, mCameraEffId, EffectManager.eType.Common);

        ParticleSystem ps = EffectManager.Instance.GetEffectOrNull(mGroundEffId, EffectManager.eType.Common);
		if (ps)
		{
			Vector3 pos = m_owner.transform.position;

			EffectManager.sEffInfo info = EffectManager.Instance.GetEffectInfoOrNull(mGroundEffId, EffectManager.eType.Common);
			if(info != null)
			{
				pos += m_owner.transform.forward * info.addedPos.z;
			}

			ps.transform.rotation = m_owner.transform.rotation;
			EffectManager.Instance.Play(pos, mGroundEffId, EffectManager.eType.Common);
		}

        float checkEffTime = 0.0f;
        bool doHit = false;

        m_owner.ShowMesh(false);
        yield return new WaitForSeconds(0.1f); // 이거 기다리는 동안에 OnEnd 호출이 돼서 슈퍼아머가 None으로 바뀜

        m_owner.ShowMesh(true);
        m_owner.StopPauseFrame();
        m_owner.PlayAniImmediate(eAnimation.USkillEnd, 0.0f, true);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (checkEffTime < duration)
        {
            checkEffTime += Time.fixedDeltaTime;
            if (!doHit && checkEffTime >= mHitTime)
            {
                if (World.Instance.InGameCamera.Mode == InGameCamera.EMode.DEFAULT)
                {
                    World.Instance.InGameCamera.EndUserSetting();
                }

                World.Instance.EnemyMgr.ResumeAll();
                //m_owner.Input.Pause(false);
                for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                    World.Instance.ListPlayer[i].Input.Pause( false );
                }

                if (m_owner.actionSystem.IsCurrentAction(eActionCommand.USkill01))
                {
                    m_owner.actionSystem.CancelCurrentAction();
                }

                ResetBT();
                m_owner.Input.LockBtnFlag(InputController.ELockBtnFlag.NONE);

                float damage = m_owner.GetUltimateSkillDefaultAtkPower();

                List< Unit2ndStatsTable.sOption> findAll = m_owner.unit2ndStats.FindStat(Unit2ndStatsTable.eSubjectType.USkillAtk, Unit2ndStatsTable.eIncreaseType.Damage);
                if (findAll != null)
                {
                    for (int i = 0; i < findAll.Count; i++)
                    {
                        damage += (damage * (findAll[i].value / (float)eCOUNT.MAX_RATE_VALUE));
                    }
                }

                mAtkEvt.Set(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Unit, mAniEvt, damage * mParam.Value1, eAttackDirection.Skip, 
                            false, 0, EffectManager.eType.None, m_owner.GetTargetColliderList(true), 0.0f, true, true);

                EventMgr.Instance.SendEvent(mAtkEvt);

                doHit = true;
            }

            yield return mWaitForFixedUpdate;
        }

        Debug.Log("오의 다 끝났다");
        
        //m_owner.RestoreSuperArmor(mChangedSuperArmorId);
        m_owner.TemporaryInvincible = false;

        ActionExtreamEvade actionExtreamEvade = m_owner.actionSystem.GetAction<ActionExtreamEvade>(eActionCommand.ExtreamEvade);
        if(actionExtreamEvade)
        {
            actionExtreamEvade.ForceEnd();
        }

        ResetBT();

        mOwnerPlayer.UsingUltimateSkill = false;
    }

	protected void ResetBT() {
        for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
            if( !World.Instance.EnemyMgr.HasAliveMonster() ) {
                World.Instance.ListPlayer[i].SetAutoMove( true );
            }
            else {
                World.Instance.ListPlayer[i].ResetBT();
            }

            if( mOwnerPlayer == World.Instance.ListPlayer[i] ) {
                continue;
            }
        }

        Player opponentPlayer = null;
        if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
            opponentPlayer = m_owner as Player;

            if( m_owner.AI ) {
                m_owner.ResetBT();

                if( opponentPlayer && opponentPlayer.OpponentPlayer ) {
                    Utility.SetLayer( m_owner.gameObject, (int)eLayer.Enemy, true, (int)eLayer.PostProcess );
                    if ( opponentPlayer.Guardian ) {
						Utility.SetLayer( opponentPlayer.Guardian.gameObject, (int)eLayer.EnemyClone, true, (int)eLayer.PostProcess );
					}
                }
            }
        }

        List<Unit> listTarget = World.Instance.EnemyMgr.GetActiveEnemies(m_owner);
        for( int i = 0; i < listTarget.Count; i++ ) {
            Unit target = listTarget[i];
            if( target == null || !target.IsActivate() || target.curHp <= 0.0f ) {
                continue;
            }

            if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
                if( opponentPlayer && opponentPlayer.OpponentPlayer ) {
                    Utility.SetLayer( target.gameObject, (int)eLayer.Player, true, (int)eLayer.TransparentFX );
					Player player = target as Player;
					if ( player.Guardian ) {
						Utility.SetLayer( player.Guardian.gameObject, (int)eLayer.PlayerClone, true, (int)eLayer.PostProcess );
					}
				}
            }

            target.ResetBT();
        }
    }
}
