
using System;
using System.Collections;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;


public partial class Player : Unit {
	public bool IsSupporterCoolingTime { get; private set; } = false;
	public ObscuredFloat SupporterCoolingTime { get; private set; } = 0.0f;
	public bool OnlyExtraCoolingTime { get; private set; } = false;

	private Coroutine mCrUpdateCoolTime = null;
	private WaitForSeconds mWaitForPointTwoSec = new WaitForSeconds( 0.2f );
	private bool mbContinueCoolTime = false;


	public void InitSupporterCoolTime( bool stopCoroutine ) {
		if ( stopCoroutine ) {
			Utility.StopCoroutine( World.Instance, ref mCrUpdateCoolTime );
		}

		SupporterCoolingTime = 0.0f;
		IsSupporterCoolingTime = false;
		mbContinueCoolTime = false;
		OnlyExtraCoolingTime = false;

		if ( World.Instance.StageType == eSTAGETYPE.STAGE_PVP && !OpponentPlayer && m_boSupporter != null && m_boSupporter.data != null ) {
			World.Instance.UIPVP.UpdateSupporterCoolTime( SupporterCoolingTime, m_boSupporter.data.TableData.CoolTime );
		}
	}

	public bool StartSupporterSkill() {
		if ( IsSupporterCoolingTime || World.Instance.IsPause || !isGrounded || isPause || boSupporter == null || !boSupporter.HasActiveSkill() || m_curHp <= 0.0f ) {
			return false;
		}

		SupporterCoolingTime = Mathf.Clamp( boSupporter.data.TableData.CoolTime - ( boSupporter.data.TableData.CoolTime * SupporterCoolTimeDecRate ), 0, boSupporter.data.TableData.CoolTime );

#if UNITY_EDITOR
		if ( GameInfo.Instance.GameConfig.TestMode ) {
			SupporterCoolingTime = 3.0f;
		}
#endif

		Utility.StopCoroutine( World.Instance, ref mCrUpdateCoolTime );
		mCrUpdateCoolTime = World.Instance.StartCoroutine( UpdateSupporterCoolTime() );

		return true;
	}

	public bool UseSupporterActiveSkill() {
		if ( m_boSupporter == null || m_curHp <= 0.0f || HoldSupporterSkillCoolTime ) {
			return false;
		}

		if ( World.Instance.StageType == eSTAGETYPE.STAGE_PVP && !OpponentPlayer ) {
			SoundManager.sSoundInfo info = VoiceMgr.Instance.PlaySupporter( eVOICESUPPORTER.Skill, m_boSupporter.data.TableID );
			if ( info != null && info.clip ) {
				World.Instance.UIPVP.SupporterSpeak( info.clip.length );
			}
		}

		m_boSupporter.Execute( BattleOption.eBOTimingType.Use );
		m_boSupporter.Execute( BattleOption.eBOTimingType.UseAction );

		return true;
	}

	public void ContinueSupporterCoolTime( float coolTime ) {
		mbContinueCoolTime = false;

		if ( coolTime <= 0.0f ) {
			return;
		}

		mbContinueCoolTime = true;
		SupporterCoolingTime = coolTime;

		Utility.StopCoroutine( World.Instance, ref mCrUpdateCoolTime );
		mCrUpdateCoolTime = World.Instance.StartCoroutine( UpdateSupporterCoolTime() );
	}

	public void DecreaseSupporterCoolTimeInRealTime( float value ) {
		SupporterCoolingTime = Mathf.Max( 0.0f, SupporterCoolingTime - value );
	}

	public void InitSupporterCoolTimeDecRate() {
		SupporterCoolTimeDecRate = 0.0f;

		if ( !OpponentPlayer && m_boSupporter != null && m_boSupporter.data != null ) {
			World.Instance.UIPVP.UpdateSupporterCoolTime( SupporterCoolingTime, m_boSupporter.data.TableData.CoolTime );
		}
	}

	public float GetSupporterCoolTimeFillAmount() {
		if ( !IsSupporterCoolingTime || boSupporter == null ) {
			return 0.0f;
		}

		return SupporterCoolingTime / boSupporter.data.TableData.CoolTime;
	}

	public void AddExtraSupporterCoolTime( float add ) {
		if ( m_boSupporter.data.TableData.CoolTime <= 0 ) {
			return;
		}

		SupporterCoolingTime += add;

		if ( !IsSupporterCoolingTime ) {
			OnlyExtraCoolingTime = true;

			Utility.StopCoroutine( World.Instance, ref mCrUpdateCoolTime );
			mCrUpdateCoolTime = World.Instance.StartCoroutine( UpdateSupporterCoolTime() );

			World.Instance.UIPlay.StartUpdateSupporterCoolTime();
		}
	}

	private IEnumerator UpdateSupporterCoolTime() {
		IsSupporterCoolingTime = true;

		if ( !mbContinueCoolTime && !OnlyExtraCoolingTime ) {
			yield return mWaitForPointTwoSec;

			if ( !UseSupporterActiveSkill() ) {
				IsSupporterCoolingTime = false;
				yield break;
			}

			// 1-2 서포터 스킬 사용 튜토리얼에선 서포터 쿨타임 강제로 3초만 적용
			if ( GameSupport.IsInGameTutorial() &&
				GameInfo.Instance.UserData.GetTutorialState() == (int)eTutorialState.TUTORIAL_STATE_Stage2Clear &&
				GameInfo.Instance.UserData.GetTutorialStep() < 4 ) {
				if ( GameInfo.Instance.UserData.GetTutorialStep() == 3 ) {
					GameInfo.Instance.UserData.SetTutorial( GameInfo.Instance.UserData.GetTutorialState(), 4 );
					World.Instance.ShowTutorialHUD( 3, 5.0f );
					World.Instance.ActiveEnemyMgrOnTutorial();
				}
				else {
					SupporterCoolingTime = 3.0f;
				}
			}
		}

		WorldPVP worldPVP = World.Instance as WorldPVP;

		while ( SupporterCoolingTime > 0.0f ) {
			if ( !HoldSupporterSkillCoolTime ) {
				if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
					SupporterCoolingTime -= Time.fixedDeltaTime;
				}
				else if ( worldPVP.IsBattleStart ) {
					SupporterCoolingTime -= Time.fixedDeltaTime;

					if ( !OpponentPlayer ) {
						World.Instance.UIPVP.UpdateSupporterCoolTime( SupporterCoolingTime, m_boSupporter.data.TableData.CoolTime );
					}
				}
			}

			yield return mWaitForFixedUpdate;
		}

		if ( worldPVP && !OpponentPlayer && !OnlyExtraCoolingTime ) {
			SoundManager.sSoundInfo info = VoiceMgr.Instance.PlaySupporter( eVOICESUPPORTER.SkillGauge, m_boSupporter.data.TableID );
			if ( info != null && info.clip ) {
				World.Instance.UIPVP.SupporterSpeak( info.clip.length );
			}
		}

		InitSupporterCoolTime( false );
	}
}
