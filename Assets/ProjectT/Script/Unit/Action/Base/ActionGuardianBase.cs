using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGuardianBase : ActionBase {
	protected ActionSelectSkillBase					mOwnerActionSelectSkillBase = null;
	protected PlayerGuardian						mPlayerGuardian				= null;
	protected GameTable.CharacterSkillPassive.Param m_data						= null;

	protected float mValue1 = 0.0f;
	protected float mValue2 = 0.0f;
	protected float mValue3 = 0.0f;

	private bool	mIsSetValue		= false;


	public virtual void ExecuteBattleOption( BattleOption.eBOTimingType timingType, int actionTableId = 0, Projectile projectile = null ) {
		if ( mOwnerActionSelectSkillBase.BOCharSkill != null ) {
			mOwnerActionSelectSkillBase.BOCharSkill.Execute( timingType, actionTableId == 0 ? TableId : actionTableId, projectile );
		}
	}

	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		m_data = GameInfo.Instance.GameTable.FindCharacterSkillPassive( tableId );

		mPlayerGuardian = m_owner as PlayerGuardian;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		SetValues();

		ExecuteStartSkillBO();

		mPlayerGuardian.aniEvent.SetShaderAlpha( "_Color", 0.5f );

		m_owner.MainCollider.Enable( true );
		m_owner.MainCollider.Restore();
	}

	public override void OnCancel() {
		base.OnCancel();

		if ( mPlayerGuardian != null ) {
			mPlayerGuardian.EndAction();
		}

		m_owner.alwaysKinematic = true;
		m_owner.rigidBody.isKinematic = true;

		m_owner.transform.position = new Vector3( m_owner.transform.position.x, m_owner.posOnGround.y, m_owner.transform.position.z );

		if ( AppMgr.Instance.SceneType == AppMgr.eSceneType.Training ) {
			m_owner.aniEvent.PlayAni( eAnimation.Idle01 );
		}

		m_owner.MainCollider.Enable( true );
	}

	public override void OnEnd() {
		base.OnEnd();

		if ( mPlayerGuardian != null ) {
			mPlayerGuardian.EndAction();
		}

		m_owner.alwaysKinematic = true;
		m_owner.rigidBody.isKinematic = true;

		m_owner.transform.position = new Vector3( m_owner.transform.position.x, m_owner.posOnGround.y, m_owner.transform.position.z );

		if ( AppMgr.Instance.SceneType == AppMgr.eSceneType.Training ) {
			m_owner.aniEvent.PlayAni( eAnimation.Idle01 );
		}

		m_owner.MainCollider.Enable( true );
	}

	public override void ShowSkillNames( params GameTable.CharacterSkillPassive.Param[] datas ) {
		if ( mPlayerGuardian != null && mPlayerGuardian.IsCommandAction ) {
			return;
		}

		if ( mPlayerGuardian.OwnerPlayer && mPlayerGuardian.OwnerPlayer.IsHelper ) {
			return;
		}

		base.ShowSkillNames( datas );
	}

	public bool IsPossibleUse() {
		SetOwnerActionSelectSkillBase();

		if ( mOwnerActionSelectSkillBase == null ) {
			return false;
		}

		return mOwnerActionSelectSkillBase.PossibleToUse;
	}

	public bool IsExplicitStartCoolTime() {
		SetOwnerActionSelectSkillBase();

		if ( mOwnerActionSelectSkillBase == null ) {
			return false;
		}

		return mOwnerActionSelectSkillBase.ExplicitStartCoolTime;
	}

	public void StartCoolTime() {
		SetOwnerActionSelectSkillBase();

		if ( mOwnerActionSelectSkillBase == null ) {
			return;
		}

		mOwnerActionSelectSkillBase.StartCoolTime();
	}

	public float GetMaxCoolTime() {
		if ( mOwnerActionSelectSkillBase == null ) {
			return 0.0f;
		}

		return mOwnerActionSelectSkillBase.MaxCoolTime;
	}

	protected void ExecuteStartSkillBO() {
		if ( mPlayerGuardian == null || mPlayerGuardian.OwnerPlayer == null || mOwnerActionSelectSkillBase == null ) {
			return;
		}

		ExecuteBattleOption( BattleOption.eBOTimingType.StartSkill );
		ExecuteBattleOption( BattleOption.eBOTimingType.DuringSkill );

		if ( mPlayerGuardian.OwnerPlayer.boWeapon != null ) {
			mPlayerGuardian.OwnerPlayer.boWeapon.Execute( BattleOption.eBOTimingType.StartSkill, TableId, null );
			mPlayerGuardian.OwnerPlayer.boWeapon.Execute( BattleOption.eBOTimingType.DuringSkill, TableId, null );
		}

		if ( mPlayerGuardian.OwnerPlayer.boSupporter != null ) {
			mPlayerGuardian.OwnerPlayer.boSupporter.Execute( BattleOption.eBOTimingType.StartSkill, TableId, null );
		}
	}

	protected Vector3 GetTargetDir() {
		if ( mPlayerGuardian == null || mPlayerGuardian.OwnerPlayer == null ) {
			return m_owner.transform.forward;
		}

		Vector3 dir;
		Unit target = mPlayerGuardian.GetActionTarget();
		if ( target != null ) {
			dir = ( target.transform.position - m_owner.transform.position ).normalized;
		}
		else {
			dir = mPlayerGuardian.OwnerPlayer.transform.forward;
		}

		return dir;
	}

	private void SetValues() {
		if ( mIsSetValue ) {
			return;
		}

		SetOwnerActionSelectSkillBase();

		if ( mOwnerActionSelectSkillBase == null ) {
			return;
		}

		mValue1 = mOwnerActionSelectSkillBase.GetValue1();
		mValue2 = mOwnerActionSelectSkillBase.GetValue2();
		mValue3 = mOwnerActionSelectSkillBase.GetValue3();

		mIsSetValue = true;

		mBuffEvt.ActionSelectSkill = mOwnerActionSelectSkillBase;
	}

	private void SetOwnerActionSelectSkillBase() {
		if ( mOwnerActionSelectSkillBase != null ) {
			return;
		}

		PlayerGuardian playerGuardian = m_owner as PlayerGuardian;
		if ( playerGuardian == null || playerGuardian.OwnerPlayer == null ) {
			return;
		}

		mOwnerActionSelectSkillBase = playerGuardian.OwnerPlayer.actionSystem.GetAction( actionCommand ) as ActionSelectSkillBase;
	}
}
