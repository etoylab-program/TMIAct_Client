
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionBehimosBrokenWings : ActionEnemyBase
{
    private EnemyParts m_enemy = null;
    private Director m_director = null;
    private bool m_played = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.BrokenWings;

        m_enemy = m_owner as EnemyParts;

        m_director = GameSupport.CreateDirector("drt_behimos_wing_destroyed");
        m_director.Init(m_enemy);

        m_played = false;
    }

	public override void OnStart( IActionBaseParam param ) {
		if( m_played ) {
			return;
		}

		base.OnStart( param );

		for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
			if( !World.Instance.ListPlayer[i].IsActivate() ) {
				continue;
			}

			World.Instance.ListPlayer[i].actionSystem.CancelCurrentAction();

			if( !World.Instance.ListPlayer[i].isGrounded ) {
				World.Instance.ListPlayer[i].transform.position = World.Instance.ListPlayer[i].posOnGround;
			}
		}

		m_enemy.SetKinematicRigidBody( true, true );
		m_enemy.transform.position = m_owner.posOnGround;

        if( m_director.listAnimationActorInfo.Count > 0 ) {
            Utility.SetLayer( m_enemy.gameObject, (int)eLayer.Default, true, (int)eLayer.Wall );
        }

		m_enemy.HideParts();
		m_enemy.StopBT();

		m_director.SetCallbackOnEnd2( EndDirector );
		m_director.Play();
	}

	public override IEnumerator UpdateAction() {
		if( m_played ) {
			yield break;
		}

		while( !m_endUpdate ) {
			yield return mWaitForFixedUpdate;
		}

		while( World.Instance.Player.Input.isPause ) {
			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnEnd()
    {
        if (m_played)
        {
            return;
        }

        base.OnEnd();
        m_enemy.ResetBT();

        m_played = true;
    }

    private void EndDirector()
    {
        m_endUpdate = true;

        m_enemy.SetGroundedRigidBody();
        Utility.SetLayer(m_enemy.gameObject, (int)eLayer.Enemy, true, (int)eLayer.Wall);
    }
}
