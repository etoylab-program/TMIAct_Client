
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HitStun : ActionHitBase
{
    private float   mOriginalDuration   = 0.0f;
    private bool    mGroggy             = false;
    private float   mReduceTime         = 0.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Stun;
    }

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		if ( m_owner.AI ) {
			m_owner.StopBT();
		}
		else if ( m_owner.Input ) {
			m_owner.Input.Pause( true );
		}

		mGroggy = false;
        if ( m_owner.isGroggy || ( param as ActionParamHit ).attackerBehaviour == eBehaviour.GroggyAttack ) {
            mGroggy = true;
        }

        if ( mOriginalDuration == 0.0f ) {
            mOriginalDuration = m_owner.stunDuration;
        }

		mReduceTime = m_owner.stunDuration * m_owner.GetDebuffTimeReduceValue( BattleOption.eToExecuteType.Unit );

		m_owner.PlayAni( eAnimation.Stun );
		m_owner.reserveStun = false;

		if ( m_owner.isGrounded == false ) {
			m_owner.SetFallingRigidBody();
		}

		Invoke( "SendRemovePlayerHitTarget", m_owner.shakeDuration == 0.0f ? 0.15f : m_owner.shakeDuration );
	}

	public override IEnumerator UpdateAction()
    {
        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.deltaTime;
            if (m_checkTime >= (m_owner.stunDuration - mReduceTime) || m_owner.curHp <= 0.0f)
            {
                m_endUpdate = true;
            }

            yield return null;
        }
    }

    public override void OnEnd()
    {
        m_owner.aniEvent.StopCurEvent(false);
        base.OnEnd();

        if (m_owner as Player && m_owner.AI)
        {
            m_owner.ResetBT();
        }
        else if (m_owner.Input)
        {
            m_owner.Input.Pause(false);
        }

        m_owner.stunDuration = mOriginalDuration;
    }

    public override void OnCancel()
    {
        base.OnCancel();

        if (m_owner as Player && m_owner.AI)
        {
            m_owner.ResetBT();
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        ActionHitBase action = m_owner.actionSystem.GetAction<ActionHitBase>(eActionCommand.Hit);
        if (action == null || mGroggy)
        {
            return;
        }

        Parent.SetNextAction(eActionCommand.Hit, param);
        Terminate();
    }

    public void ChangeDuration(float duration)
    {
        m_owner.stunDuration = duration;
        m_checkTime = 0.0f;
    }

    public void Terminate()
    {
        m_owner.stunDuration = mOriginalDuration;
        m_checkTime = m_owner.stunDuration;

        m_owner.reserveStun = false;

        if (m_owner as Player && m_owner.AI)
        {
            m_owner.ResetBT();
        }
    }

    private void SendRemovePlayerHitTarget() {
        EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_REMOVE_HIT_TARGET, m_owner );
    }
}
