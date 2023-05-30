
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ActionPlayerDie : ActionBase
{
    private Player          mOwnerPlayer    = null;
    private Director        m_director      = null;
    private eAnimation      m_curAni        = eAnimation.Die;
    private ActionParamDie  mParamDie       = null;
    private bool            mbEnd           = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Die;

        SetNextAction(eActionCommand.None, null);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParamDie = param as ActionParamDie;

        mOwnerPlayer = m_owner as Player;
        mOwnerPlayer.StopBT();

        mbEnd = World.Instance.IsAllPlayersDead();

        if( World.Instance.StageType == eSTAGETYPE.STAGE_RAID || ( World.Instance.StageData != null && World.Instance.StageData.PlayerMode == 1 ) ) {
            World.Instance.UIPlay.DisableDiedSubCharUnit();
        }
    }

    private void Begin()
    {
        if (m_owner.curHp > 0.0f)
        {
            m_owner.SetDie();
        }

        m_owner.SetMainTarget(null);

        if (m_owner.aniEvent.HasAni(eAnimation.Die))
        {
            m_curAni = eAnimation.Die;
            if (mParamDie != null)
            {
                if (mParamDie.state == ActionParamDie.eState.Down)
                {
                    m_curAni = eAnimation.DownDie;
                }
            }
        }
        else
        {
            m_curAni = eAnimation.None;
        }

        m_director = null;
        if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
        {
            if (!m_owner.isGrounded)
            {
                m_owner.SetFallingRigidBody();
            }

            if (m_owner.aniEvent.IsPause())
            {
                m_owner.aniEvent.Resume();
            }

            m_curAni = eAnimation.Die;
        }
        else
        {
            if (mbEnd)
            {
                if( !mOwnerPlayer.IsHelper ) {
                    if( m_owner.HasDirector( "Groggy" ) == true ) {
                        m_director = m_owner.GetDirector( "Groggy" );
                        StartCoroutine( "WaitForGroundedAndPlayDirector", true );
                    }
                    else if( m_owner.HasDirector( "Die" ) == true ) {
                        m_director = m_owner.GetDirector( "Die" );
                        StartCoroutine( "WaitForGroundedAndPlayDirector", false );
                    }
                    else if( m_curAni != eAnimation.None ) {
                        m_owner.PlayAniImmediate( m_curAni );
                    }
                }
                else {
                    if( m_curAni != eAnimation.None ) {
                        m_owner.PlayAniImmediate( m_curAni );
                    }

                    m_director = null;
                }
            }
            else
            {
                m_director = null;
            }
        }
    }

	public override IEnumerator UpdateAction() {
		m_owner.PlayAniImmediate( eAnimation.Die );

		if( mbEnd && !mOwnerPlayer.IsHelper ) {
			m_owner.aniEvent.SetAniSpeed( 0.55f );
			World.Instance.SetSlowTime( 0.5f, 1.0f );
		}

		if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
			WorldPVP worldPVP = World.Instance as WorldPVP;
			if( worldPVP ) {
				worldPVP.IsAnyPlayerDead = true;
			}
		}

        if( mbEnd && !mOwnerPlayer.IsHelper ) {
            yield return new WaitForSeconds( 1.0f );
            m_owner.aniEvent.SetAniSpeed( 1.0f );
        }

		if( mbEnd ) {
			EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_PLAYER_DEAD, m_owner );
		}

		Begin();

		if( m_director == null && m_curAni != eAnimation.None ) {
			while( m_owner.aniEvent.IsAniPlaying( m_curAni ) == eAniPlayingState.Playing ) {
				yield return null;
			}
		}
		else {
			while( m_endUpdate == false ) {
				yield return null;
			}
		}
	}

	private IEnumerator WaitForGroundedAndPlayDirector(bool groggy)
    {
        if (m_owner.isGrounded == false)
        {
            m_owner.SetFallingRigidBody();
        }

        while(!m_owner.isGrounded)
        {
            yield return null;
        }

        m_owner.SetDieRigidbody();
        World.Instance.InGameCamera.ExcludCullingMask((int)eLayer.Enemy);
        World.Instance.InGameCamera.ExcludCullingMask((int)eLayer.EnemyGate);

        if (groggy)
        {
            m_owner.PlayDirector("Groggy", AfterGroggy);
        }
        else
        {
            m_owner.PlayDirector("Die", AfterDie);
        }

        GameUIManager.Instance.ShowUI("GameFailPanel", true);

        yield return null;
    }

	public override void OnEnd() {
		isPlaying = false;

		EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_REMOVE_HIT_TARGET, m_owner );

		if ( !mbEnd ) {
			EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_PLAYER_DEAD, m_owner );
		}
		else {
			OnEndCallback?.Invoke();
		}
	}

	private void AfterGroggy()
    {
        World.Instance.InGameCamera.ExcludCullingMask((int)eLayer.Enemy);
        World.Instance.InGameCamera.ExcludCullingMask((int)eLayer.EnemyGate);

        m_owner.PlayDirector("Die", AfterDie);
        mOwnerPlayer.LockChangeFace = true;
    }

    private void AfterDie()
    {
        m_endUpdate = true;
        mOwnerPlayer.LockChangeFace = false;
    }
}
