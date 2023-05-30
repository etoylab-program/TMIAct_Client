
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;


public class OtherObject : Unit {
	[Header("[Defence Object Property]")]
	public float FearDistance = 5.0f;

	public GameClientTable.Monster.Param	Data		{ get; private set; } = null;
	public Texture2D						Portrait	{ get; private set; } = null;
	public int								HitCount	{ get; private set; } = 0;

	private ActionParamHit  mActionParamHit = new ActionParamHit();
	private bool            mbFear          = false;


	public override void Init( int tableId, eCharacterType type, string faceAniControllerPath ) {
		Deactivate();
		base.Init( tableId, type, faceAniControllerPath );

		ActionBase actionBase = m_actionSystem.GetAction(eActionCommand.Hit);
		actionBase.OnEndCallback = OnHitEnd;

		Activate();
		SetKinematicRigidBody();

		World.Instance.Player.SetOtherObjectNavigator( this );
		World.Instance.UIPlay.ActiveNPCGauge( true, this );

		mbFear = false;
	}

	public override void SetData( int tableId ) {
		Data = GameInfo.Instance.GetMonsterData( tableId );
		if( Data == null ) {
			return;
		}

		m_tableId = tableId;
		m_maxHp = Data.MaxHP;
		m_curHp = m_maxHp;
		m_originalSpeed = Data.MoveSpeed;
		m_curSpeed = m_originalSpeed;
		m_attackPower = Data.AttackPower;
		m_folder = Utility.GetFolderFromPath( Data.ModelPb );

		Portrait = ResourceMgr.Instance.LoadFromAssetBundle( "icon", "icon/monster/" + Data.Portrait ) as Texture2D;
		HitCount = 0;
	}

	public override bool OnHit( Unit attacker, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent attackerAniEvt, ObscuredFloat damage, 
								ref bool isCritical, ref eHitState hitState, Projectile projectile, bool isUltimateSkill, bool skipMaxDamageRecord ) {
		base.OnHit( attacker, toExecuteType, attackerAniEvt, damage, ref isCritical, ref hitState, projectile, isUltimateSkill, skipMaxDamageRecord );
		if( hitState == eHitState.Success ) {
			++HitCount;

			if( !isVisible ) {
				World.Instance.UIPlay.m_screenEffect.ShowHUD( FLocalizeString.Instance.GetText( 1518 ), null, false, GameInfo.Instance.GameConfig.DefenceModeWarningNoticeDuration );
			}

			mActionParamHit.Set( attacker, eBehaviour.Attack, attacker.GetAirAttackJumpPower() * floatingJumpPowerRatio, attackerAniEvt.hitDir,
								attackerAniEvt.hitEffectId, isCritical, attacker.aniEvent.GetCurCutFrameLength(), hitState, attackerAniEvt.atkAttr );

			if( m_curHp > 0.0f ) {
				World.Instance.UIPlay.UpdateNPCGauge( m_curHp, m_maxHp );
				CommandAction( eActionCommand.Hit, mActionParamHit );
			}
			else {
				PlayAni( eAnimation.Die );
				EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_OTHER_OBJECT_DEAD, this );
			}
		}

		return false;
	}

	private void OnHitEnd() {
		EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_REMOVE_HIT_TARGET, this );

		if ( m_curHp <= 0.0f ) {
			return;
		}

		if ( mbFear ) {
			PlayAni( eAnimation.Idle02 );
		}
		else {
			PlayAni( eAnimation.Idle01 );
		}
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();

		Collider[] cols = Physics.OverlapSphere(transform.position, FearDistance, 1 << (int)eLayer.Enemy);
		if( cols.Length <= 0 && mbFear ) {
			PlayAni( eAnimation.Idle01 );
			mbFear = false;
		}
		else if( cols.Length > 0 && !mbFear ) {
			PlayAni( eAnimation.Idle02 );
			mbFear = true;
		}
	}
}
