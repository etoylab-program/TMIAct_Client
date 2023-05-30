
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporter119Attack : ActionSupporterSkillBase {

	private ParticleSystem	mEffFloor	= null;
	private BoxCollider		mBoxFloor	= null;
	private Vector3			mFloorPos	= Vector3.zero;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.Supporter119Attack;

		mEffFloor = GameSupport.CreateParticle( "Effect/Supporter/prf_fx_supporter_skill_119.prefab", null );
        mBoxFloor = mEffFloor.GetComponent<BoxCollider>();
    }

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		mParamFromBO = param as ActionParamFromBO;
	}

	public override IEnumerator UpdateAction() {
        float duration = mParamFromBO.battleOptionData.duration;
        mFloorPos = m_owner.posOnGround + ( m_owner.transform.forward * 1.25f );

        mEffFloor.transform.position = mFloorPos;
        mEffFloor.gameObject.SetActive( true );
        EffectManager.Instance.RegisterStopEffByDuration( mEffFloor, null, duration );

        m_checkTime = 0.0f;
        float tickCheckTime = mParamFromBO.battleOptionData.tick;

        WorldPVP worldPVP = World.Instance as WorldPVP;

        while ( true ) {
            m_checkTime += m_owner.fixedDeltaTime;
            if ( m_checkTime >= duration || ( worldPVP && ( !worldPVP.IsBattleStart || m_owner.curHp <= 0.0f ) ) ) {
                mOwnerPlayer.ExtreamEvading = false;
                EffectManager.Instance.StopEffImmediate( mEffFloor, null );

                if ( World.Instance.ListPlayer.Count > 0 ) {
                    for ( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                        World.Instance.ListPlayer[i].StayOnBoxCollider = null;
                    }
                }
                else {
                    m_owner.StayOnBoxCollider = null;
                }

                yield break;
            }

            tickCheckTime += m_owner.fixedDeltaTime;

            if ( tickCheckTime >= mParamFromBO.battleOptionData.tick ) {
                if ( World.Instance.ListPlayer.Count > 0 ) {
                    for ( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                        if ( World.Instance.ListPlayer[i].curHp <= 0.0f || !World.Instance.ListPlayer[i].IsActivate() ) {
                            continue;
                        }

                        if ( mBoxFloor.bounds.Contains( World.Instance.ListPlayer[i].transform.position ) ) {
                            World.Instance.ListPlayer[i].AddHpPercentage( BattleOption.eToExecuteType.Unit, mParamFromBO.battleOptionData.value, false );
                        }
                    }
                }
                else {
                    if ( m_owner.curHp > 0.0f && mBoxFloor.bounds.Contains( m_owner.transform.position ) ) {
                        m_owner.AddHpPercentage( BattleOption.eToExecuteType.Unit, mParamFromBO.battleOptionData.value, false );
                    }
                }

                tickCheckTime = 0.0f;
            }

            if ( World.Instance.ListPlayer.Count > 0 ) {
                for ( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                    if ( World.Instance.ListPlayer[i].curHp <= 0.0f || !World.Instance.ListPlayer[i].IsActivate() ) {
                        continue;
                    }

                    bool contains = mBoxFloor.bounds.Contains( World.Instance.ListPlayer[i].transform.position );

                    if ( contains && World.Instance.ListPlayer[i].StayOnBoxCollider == null ) {
                        World.Instance.ListPlayer[i].StayOnBoxCollider = mBoxFloor;
                        SendBattleOption( World.Instance.ListPlayer[i] );
                    }
                    else if ( !contains && World.Instance.ListPlayer[i].StayOnBoxCollider ) {
                        World.Instance.ListPlayer[i].StayOnBoxCollider = null;
                    }
                }
            }
            else {
                bool contains = mBoxFloor.bounds.Contains( m_owner.transform.position );

                if ( m_owner.curHp > 0.0f && contains && m_owner.StayOnBoxCollider == null ) {
                    m_owner.StayOnBoxCollider = mBoxFloor;
                    SendBattleOption( m_owner );
                }
                else if ( m_owner.curHp > 0.0f && !contains && m_owner.StayOnBoxCollider ) {
                    m_owner.StayOnBoxCollider = null;
				}
            }

            yield return mWaitForFixedUpdate;
        }
    }

    private void SendBattleOption( Unit sender ) {
        if ( mParamFromBO.battleOptionData.addCallTiming != BattleOption.eBOAddCallTiming.OnSend ) {
            return;
        }
		
        if ( mParamFromBO.battleOptionData.timingType == BattleOption.eBOTimingType.Use &&
			 mParamFromBO.battleOptionData.dataOnEndCall.conditionType == BattleOption.eBOConditionType.ComboCountAsValue ) {
			mParamFromBO.battleOptionData.dataOnEndCall.evt.value = mParamFromBO.battleOptionData.evt.value;
		}

		EffectManager.Instance.Play( m_owner, mParamFromBO.battleOptionData.dataOnEndCall.startEffId, (EffectManager.eType)mParamFromBO.battleOptionData.dataOnEndCall.effType );

		mParamFromBO.battleOptionData.dataOnEndCall.useTime = System.DateTime.Now;
        mParamFromBO.battleOptionData.dataOnEndCall.evt.sender = sender;

        EventMgr.Instance.SendEvent( mParamFromBO.battleOptionData.dataOnEndCall.evt );

		Log.Show( mParamFromBO.battleOptionData.dataOnEndCall.evt.battleOptionData.battleOptionSetId + "번 배틀옵션셋 사용 (애드콜)!!!", Log.ColorType.Green );

	}
}
