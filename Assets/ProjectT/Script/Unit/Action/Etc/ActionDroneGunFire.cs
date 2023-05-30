
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionDroneGunFire : ActionBase
{
    private DroneUnit mOwnerDrone = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Attack01;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.DroneFollowOwner;

        mOwnerDrone = m_owner as DroneUnit;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        m_aniLength = mOwnerDrone.PlayAniImmediate(eAnimation.AttackIdle01);
    }

	public override IEnumerator UpdateAction() {
		yield return new WaitForSeconds( m_aniLength );

		bool playedAni = false;

		while( !m_endUpdate && isPlaying ) {
			bool isTargetDead = false;
			if( mOwnerDrone.mainTarget ) {
				EnvObject envObj = mOwnerDrone.mainTarget as EnvObject;
				if( envObj && ( envObj.curHp <= 0.0f || envObj.IsOverHit() ) ) {
					isTargetDead = true;
				}
				else if( mOwnerDrone.mainTarget.curHp <= 0.0f ) {
					isTargetDead = true;
				}
			}

			if( mOwnerDrone.Owner.Input && mOwnerDrone.Owner.Input.isPause ) {
				if( mOwnerDrone.aniEvent.curAniType == eAnimation.Attack01 ) {
					mOwnerDrone.PlayAni( eAnimation.Idle01 );
					playedAni = false;
				}
			}
			else if( mOwnerDrone.mainTarget == null || !mOwnerDrone.mainTarget.IsActivate() || isTargetDead ) {
				mOwnerDrone.GetMainTargetCollider( true );
			}

			if( mOwnerDrone.mainTarget == null || ( mOwnerDrone.Owner.Input && mOwnerDrone.Owner.Input.isPause ) ) {
				mOwnerDrone.UpdateMovement( null );

				if( mOwnerDrone.mainTarget == null && mOwnerDrone.aniEvent.curAniType == eAnimation.Attack01 ) {
					mOwnerDrone.PlayAni( eAnimation.Idle01 );
					playedAni = false;
				}
			}
			else {
				if( !playedAni ) {
					mOwnerDrone.PlayAniImmediate( eAnimation.Attack01 );
					playedAni = true;
				}

				if( AppMgr.Instance.CustomInput.InputType == BaseCustomInput.eInputType.Touch &&
					!mOwnerDrone.mainTarget.isVisible &&
					( mOwnerDrone.OwnerPlayer == null || !mOwnerDrone.OwnerPlayer.IsHelper ) ) {
					World.Instance.InGameCamera.TurnToTarget( mOwnerDrone.mainTarget.transform.position,
															 GameInfo.Instance.BattleConfig.CameraTurnSpeed,
															 false,
															 mOwnerDrone.mainTarget );
				}

				mOwnerDrone.UpdateMovement( mOwnerDrone.mainTarget );
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnCancel()
    {
        base.OnCancel();
        End();
    }

    public override void OnEnd()
    {
        base.OnEnd();
        End();
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.Attack01);
        if (evt == null)
        {
            return 0.0f;
        }

        return evt.visionRange;
    }

    private void End()
    {
        mOwnerDrone.SetMainTarget(null);
        //World.Instance.InGameCamera.SetDefaultModeTarget(null, false);
    }
}
