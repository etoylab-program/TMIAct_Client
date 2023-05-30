
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ActionAppear : ActionEnemyBase
{
    private bool m_hasDirector = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Appear;
    }

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		mOwnerEnemy.ExecuteBattleOption( BattleOption.eBOTimingType.GameStart, 0, null );
		if ( mOwnerEnemy.child ) {
			mOwnerEnemy.child.ExecuteBattleOption( BattleOption.eBOTimingType.GameStart, 0, null );
		}

		mOwnerEnemy.StopBT();

		m_hasDirector = m_owner.HasDirector( "BossAppear" );
		if ( m_hasDirector ) {
			if ( World.Instance.HasDirector( "BossAppear" ) ) {
				m_hasDirector = true;
				World.Instance.PlayDirector( "BossAppear", EndBossAppear );
			}
			else {
				//World.Instance.playerCam.ExcludMask((int)eLayer.Player);
				World.Instance.InGameCamera.ExcludCullingMask( (int)eLayer.Player );
				m_owner.PlayDirector( "BossAppear", EndBossAppear );

				Director director = m_owner.GetDirector( "BossAppear" );
				if ( director.listAnimationActorInfo.Count > 0 ) {
					Utility.SetLayer( m_owner.gameObject, (int)eLayer.Default, true, (int)eLayer.Wall );
				}

				m_owner.checkRayCollision = false;
			}
		}
		else if ( m_owner.aniEvent.HasAni( eAnimation.Appear ) ) {
			m_aniLength = m_owner.PlayAniImmediate( eAnimation.Appear );
		}
		else {
			m_owner.PlayAniImmediate( eAnimation.Idle01 );
		}
	}

	public override IEnumerator UpdateAction()
    {
        if (!m_hasDirector && m_owner.aniEvent.HasAni(eAnimation.Appear))
        {
			//while (m_owner.aniEvent.IsAniPlaying(eAnimation.Appear) == eAniPlayingState.Playing)
			while(!m_endUpdate)
			{
				m_checkTime += Time.deltaTime;
				if(m_checkTime >= m_aniLength)
				{
					m_endUpdate = true;
				}

				yield return null;
			}
        }
    }

	public override void OnCancel() {
		base.OnCancel();

		for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
			World.Instance.ListPlayer[i].ExecuteBattleOption( BattleOption.eBOTimingType.OnEnemyAppear, 0, null );
		}
	}

	public override void OnEnd() {
		base.OnEnd();

		for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
			World.Instance.ListPlayer[i].ExecuteBattleOption( BattleOption.eBOTimingType.OnEnemyAppear, 0, null );
		}

		mOwnerEnemy = m_owner as Enemy;
		if( mOwnerEnemy ) {
			mOwnerEnemy.ResetBT();
			mOwnerEnemy.actionSystem.CancelCurrentAction();
		}
	}

	private void EndBossAppear()
    {
        /*@@
        Vector3 viewPortPt = Camera.main.WorldToViewportPoint(m_owner.GetHeadPos());//.transform.position);
        if (Utility.IsInViewPort(viewPortPt) == true)
        {
        }
        else
            World.Instance.playerCam.Turn(m_owner.transform.position);*/

        //World.Instance.enemyMgr.ResumeAll();

        /*
        Collider[] cols = Physics.OverlapSphere(m_owner.transform.position, m_owner.MainCollider.radius, 1 << (int)eLayer.Player);
        for(int i = 0; i < cols.Length; i++)
        {
            Player player = cols[i].GetComponent<Player>();
            if(player == null)
            {
                continue;
            }

            Vector3 edgePos = player.GetTargetCapsuleEdgePos(m_owner);
            edgePos.y = player.transform.position.y;

            player.SetInitialPosition(edgePos, player.transform.rotation);
        }
        */

        Utility.SetLayer(m_owner.gameObject, (int)eLayer.Enemy, true, (int)eLayer.Wall);
        m_owner.checkRayCollision = true;

        EventMgr.Instance.SendEvent(eEventSubject.World, eEventType.EVENT_GAME_ENEMY_BOSS_APPEAR, m_owner);
        mOwnerEnemy.actionSystem.CancelCurrentAction();

        OnEnd();
    }
}
