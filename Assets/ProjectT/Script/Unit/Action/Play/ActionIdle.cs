
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ActionIdle : ActionBase {
	[Header("[Set Animations]")]
	public eAnimation	defaultAni;
	public eAnimation	afterAttackAni;
	public eAnimation	boringAni;
	public float		changeTime = 0.0f;

	private eAnimation	m_curAni			= eAnimation.None;
	private eAnimation	mOriginalDefaultAni	= eAnimation.None;
	private float		mForceEndTime		= 0.0f;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.Idle;

		extraCondition = new eActionCondition[1];
		extraCondition[0] = eActionCondition.Grounded;

		extraCancelCondition = new eActionCondition[1];
		extraCancelCondition[0] = eActionCondition.All;

		mOriginalDefaultAni = defaultAni;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		ActionParamAI aiParam = param as ActionParamAI;
		if( aiParam != null ) {
			mForceEndTime = Utility.GetRandom( aiParam.minValue, aiParam.maxValue, 10.0f );
		}

		if( afterAttackAni != eAnimation.None && m_owner.actionSystem.IsAnyAttackAction( m_owner.actionSystem.BeforeActionCommand ) ) {
			m_curAni = afterAttackAni;
		}
		else {
			m_curAni = defaultAni;
		}

		m_owner.PlayAni( m_curAni );

		//if ( m_owner is PlayerGuardian ) {
		//	m_owner.aniEvent.SetShaderAlpha( "_Color", 0.5f );
		//}
	}

	public override IEnumerator UpdateAction() {
		Player player = m_owner as Player;
		SoundManager.sSoundInfo sndInfo = null;
		int ckTimeCnt = 0;
		float checkForceEndTime = 0.0f;

		while( !m_endUpdate ) {
			if( mForceEndTime > 0.0f ) {
				checkForceEndTime += Time.fixedDeltaTime;
				if( checkForceEndTime >= mForceEndTime ) {
					break;
				}
			}

			if( boringAni != eAnimation.None ) {
				if( m_owner.aniEvent.IsAniPlaying( defaultAni ) == eAniPlayingState.Playing ) {
					m_checkTime += m_owner.fixedDeltaTime;
					if( m_checkTime >= changeTime ) {
						if( player && player.boSupporter != null && player.boSupporter.data != null ) {
							ckTimeCnt += 1;

							if( !GameSupport.IsShowGameStroyUI() && ckTimeCnt >= 10 ) {
								sndInfo = VoiceMgr.Instance.PlaySupporter( eVOICESUPPORTER.Wait, player.boSupporter.data.TableID );
								ckTimeCnt = 0;
							}
						}

						m_checkTime = 0.0f - ( ( sndInfo != null && sndInfo.clip ) ? sndInfo.clip.length : 0.0f );
					}
				}
				else if( m_owner.aniEvent.IsAniPlaying( boringAni, true ) == eAniPlayingState.End ) {
					m_curAni = defaultAni;
					m_owner.PlayAni( m_curAni );
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnEnd() {
		base.OnEnd();
		m_endUpdate = true;
	}

	public void RestoreDefaultAni() {
		defaultAni = mOriginalDefaultAni;
	}
}
