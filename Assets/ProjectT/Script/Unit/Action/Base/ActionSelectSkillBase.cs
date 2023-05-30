
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public abstract class ActionSelectSkillBase : ActionBase {
	//== get; set
	public int						GroupId							{ get; set; }
	public int						Index							{ get; set; } = 0;        // 캐릭터 스킬이 복수의 액션으로 이루어 질 때 사용
	public bool						IsLastSkill						{ get; set; } = true;     // 캐릭터 스킬이 복수일 때 사용. 마지막 스킬인지 여부
	public ActionSelectSkillBase	Child							{ get; set; } = null;
	public float					DecreaseSkillCoolTimeValue		{ get; set; } = 0.0f;
	public bool						SetAddAction					{ get; set; } = false;
	public float					AddActionValue1					{ get; set; } = 1.0f;
	public float					AddActionValue2					{ get; set; } = 0.0f;
	public float					AddActionValue3					{ get; set; } = 0.0f;
	public float					AddActionDuration				{ get; set; } = 0.0f;
	public float					AddBuffDuration					{ get; set; } = 0.0f;
	public float					AddDebuffDuration				{ get; set; } = 0.0f;
	//== get; protected set;
	public bool						PossibleToUse					{ get; protected set; }
	public bool						ExplicitStartCoolTime			{ get; protected set; } = false;
	public PostProcessProfile		ExtreamEvadeProfile				{ get; protected set; } = null;
	public Texture2D				TexLUT							{ get; protected set; } = null;
	public bool						IsSkillEnd						{ get; protected set; } = false;
	public bool						SkipBOExecuteOnStart			{ get; protected set; } = false;
	public bool						IsRepeatSkill					{ get; protected set; } = false;    // 스킬 사용 후 버튼 연타가 필요한지 여부
	public bool						ForceQuitBuffDebuff				{ get; protected set; } = false;    // CmptBuffDebuff에서 돌고 있는 버프/디버프를 강제 종료할지 여부
	public float					DecreaseSkillCoolTimeValueRatio	{ get; protected set; } = 0.0f;
	public ActionSelectSkillBase	ExecuteAction					{ get; protected set; } = null;     // 이 액션을 실행시킨 액션
	public bool						IsNormalAttack					{ get; protected set; } = false; // 마이카 기본 공격 강화 액션을 차지 공격으로 만들어놔서 그 차지 공격을 스킬이 아닌 기본 공격으로 만들게끔 할 플래그
	public float					mDecreaseSkillCoolTimeSec		{ get; protected set; } = 0.0f;
	//== get; private set;
	public BOCharSkill				BOCharSkill						{ get; private set; } = null;
	public float					CoolTime						{ get; private set; } = 0.0f;
	public float					MaxCoolTime						{ get; private set; } = 0.0f;
	public float					IncreaseCoolingTime				{ get; private set; } = 0.0f;
	public float					IncreaseCollingTime2			{ get; private set; } = 0.0f;
	public float					AddExtraCoolingTime				{ get; private set; } = 0.0f;
	public bool						PassingCoolTime					{ get; private set; } = false;
	public int						ParentTableId					{ get; private set; } = -1;

	protected GameTable.CharacterSkillPassive.Param	m_data					= null;
	protected Player								mOwnerPlayer			= null;
	protected float									mValue1					= 0.0f;
	protected float									mValue2					= 0.0f;
	protected float									mValue3					= 0.0f;
	protected Unit.eSuperArmor						mLastAtkSuperArmor		= Unit.eSuperArmor.None;
	protected Vector3								mDir					= Vector3.zero;
	protected float									mCalcMaxCoolTime		= 0.0f;
	protected bool									mOnlyExtraCoolingTime	= false;

	private Coroutine	mCr				= null;
	private int			mAddActionEffId	= 0;

	public virtual void ResetAddAction() {
		SetAddAction = false;
	}

	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		m_data = GameInfo.Instance.GameTable.FindCharacterSkillPassive( tableId );
		mOwnerPlayer = m_owner as Player;

		if ( m_data != null ) {
			ParentTableId = m_data.ParentsID;

			mValue1 = m_data.Value1;
			mValue2 = m_data.Value2;
			mValue3 = m_data.Value3;
			mLastAtkSuperArmor = (Unit.eSuperArmor)m_data.LastAtkSuperArmor;

			if ( Index <= m_data.AddBOSetActionIndex ) {
				BOCharSkill = new BOCharSkill( m_data, mOwnerPlayer );

#if UNITY_EDITOR
				Log.Show( "============================================", Log.ColorType.Green );
				Log.Show( "= " + TableId + "번 스킬에 " + Index + "번 액션이 갖고 있는 배틀옵션 셋 IDs", Log.ColorType.Green );
				for ( int i = 0; i < BOCharSkill.ListBattleOptionData.Count; i++ ) {
					Log.Show( "= " + BOCharSkill.ListBattleOptionData[i].battleOptionSetId, Log.ColorType.Green );
				}
				Log.Show( "============================================", Log.ColorType.Green );
#endif
			}
		}

		CoolTime = 0.0f;
		MaxCoolTime = m_data != null ? m_data.CoolTime : 0.0f;
		mCalcMaxCoolTime = MaxCoolTime;
		GroupId = 0;
		PossibleToUse = true;
		ForceQuitBuffDebuff = false;

		if ( listAddCharSkillParam != null && listAddCharSkillParam.Count > 0 ) {
			for ( int i = 0; i < listAddCharSkillParam.Count; i++ ) {
				AddSkillInfo( listAddCharSkillParam[i] );
			}
		}

		if ( MaxCoolTime > 0.0f ) {
			superArmor = Unit.eSuperArmor.Lv1;
		}

		mBuffEvt.ActionSelectSkill = this;
	}

	public override void ShowSkillNames( params GameTable.CharacterSkillPassive.Param[] datas ) {
		if ( mOwnerPlayer.IsHelper ) {
			return;
		}

		base.ShowSkillNames( datas );
	}

	public void AddSkillInfo( GameTable.CharacterSkillPassive.Param param ) {
		if ( m_data == null ) {
			return;
		}

		mValue1 += param.Value1;
		mValue2 += param.Value2;
		mValue3 += param.Value3;

		mLastAtkSuperArmor = (Unit.eSuperArmor)param.LastAtkSuperArmor;

		if ( BOCharSkill != null ) {
			if ( param.CharAddBOSetID1 > 0 ) {
				BOCharSkill.AddBattleOption( param.CharAddBOSetID1, TableId );
			}

			if ( param.CharAddBOSetID2 > 0 ) {
				BOCharSkill.AddBattleOption( param.CharAddBOSetID2, TableId );
			}

#if UNITY_EDITOR
			Log.Show( "============================================", Log.ColorType.Green );
			Log.Show( "= " + TableId + "번 스킬에 " + Index + "번 액션이 갖고 있는 배틀옵션 셋 IDs", Log.ColorType.Green );
			for ( int i = 0; i < BOCharSkill.ListBattleOptionData.Count; i++ ) {
				Log.Show( "= " + BOCharSkill.ListBattleOptionData[i].battleOptionSetId, Log.ColorType.Green );
			}
			Log.Show( "============================================", Log.ColorType.Green );
#endif
		}
	}

	public void AddSkillInfoByBOSetId( int BOSetId ) {
		if ( m_data == null ) {
			return;
		}

		if ( BOCharSkill != null ) {
			BOCharSkill.AddBattleOption( BOSetId, TableId );

#if UNITY_EDITOR
			Log.Show( "============================================", Log.ColorType.Green );
			Log.Show( "= " + TableId + "번 스킬에 " + Index + "번 액션이 갖고 있는 배틀옵션 셋 IDs", Log.ColorType.Green );
			for ( int i = 0; i < BOCharSkill.ListBattleOptionData.Count; i++ ) {
				Log.Show( "= " + BOCharSkill.ListBattleOptionData[i].battleOptionSetId, Log.ColorType.Green );
			}
			Log.Show( "============================================", Log.ColorType.Green );
#endif
		}
	}

	public virtual void ExecuteBattleOption( BattleOption.eBOTimingType timingType, int actionTableId = 0, Projectile projectile = null ) {
		if ( BOCharSkill != null ) {
			BOCharSkill.Execute( timingType, actionTableId == 0 ? TableId : actionTableId, projectile );
		}
	}

	public virtual void IncreaseSuperArmorDuration( float addRatio ) {
		if ( m_data == null ) {
			return;
		}

		BattleOption.sBattleOptionData boData = BOCharSkill.GetBattleOptionInfo( m_data.CharAddBOSetID1 );
		ChangeBOCharSkillSuperArmorDuration( boData, addRatio );

		boData = BOCharSkill.GetBattleOptionInfo( m_data.CharAddBOSetID2 );
		ChangeBOCharSkillSuperArmorDuration( boData, addRatio );
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		IsSkillEnd = false;
		SetChildSkillEndFlag( false );

		ForceQuitBuffDebuff = false;
		mCr = null;

		if ( m_data != null && !SkipBOExecuteOnStart ) {
			ExecuteStartSkillBO();
		}

		mOwnerPlayer.OnStartSkill( this );
	}

	public virtual void StartCoolTime() {
		if ( m_data == null || MaxCoolTime <= 0.0f ) {
			return;
		}

		CoolTime = IncreaseCoolingTime + IncreaseCollingTime2;
		PossibleToUse = false;
		PassingCoolTime = true;

		Utility.StopCoroutine( AppMgr.Instance, ref mCr );
		mCr = AppMgr.Instance.StartCoroutine( UpdateCoolTime() );
	}

	public virtual void StopCoolTime() {
		if ( m_data == null || mCalcMaxCoolTime <= 0.0f ) {
			return;
		}

		Utility.StopCoroutine( AppMgr.Instance, ref mCr );

		CoolTime = IncreaseCoolingTime + IncreaseCollingTime2;
		PossibleToUse = true;
		PassingCoolTime = false;
		AddExtraCoolingTime = 0.0f;
		mOnlyExtraCoolingTime = false;

		if ( Child ) {
			Child.StopCoolTime();
		}
	}

	public override void OnCancel() {
		if ( isPlaying == false ) {
			return;
		}

		isCancel = true;
		isPlaying = false;

		m_owner.ShowMesh( true );
		m_owner.RestoreSuperArmor( mChangedSuperArmorId );

		if ( !m_owner.isGrounded ) {
			m_owner.SetFallingRigidBody();
		}

		OnEndCallback?.Invoke();

		IsSkillEnd = true;
		SetAddAction = false;

		Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.Enemy );
		Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.EnemyGate );
	}

	public override void OnEnd() {
		if ( MaxCoolTime > 0.0f && Child == null || ( Child != null && IsLastSkill && actionCommand != eActionCommand.Attack01 ) ) {
			m_owner.ExecuteBattleOption( BattleOption.eBOTimingType.OnSkillEnd, 0, null );
		}

		base.OnEnd();

		IsSkillEnd = true;
		SetAddAction = false;

		Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.Enemy );
		Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.EnemyGate );
	}

	public virtual bool PossibleToUseInAI() {
		if ( m_owner.actionSystem && m_owner.actionSystem.IsCurrentAction( eActionCommand.Hit ) ) {
			return false;
		}

		if ( PossibleToUse ) {
			return true;
		}

		return false;
	}

	public float GetValue1() {
		return mValue1;
	}

	public float GetValue2() {
		return mValue2;
	}

	public float GetValue3() {
		return mValue3;
	}

	public void DecreaseSkillCoolTime( float ratio ) {
		CoolTime = Mathf.Max( 0.0f, CoolTime + ( MaxCoolTime * ratio ) );
	}

	public void DecreaseSkillCoolTimeBySec( float sec ) {
		CoolTime += sec;
	}

	public void SetIncreaseCoolingTimeBySec( float sec ) {
		IncreaseCoolingTime = sec;
	}

	public void IncreaseCoolingTime2BySec( float sec ) {
		IncreaseCollingTime2 += sec;
	}

	public ActionSelectSkillBase GetLastChildAction() {
		ActionSelectSkillBase find = Child;
		while ( find ) {
			if ( find.Child == null ) {
				return find;
			}

			find = find.Child;
		}

		return null;
	}

	public void SetChildSkillEndFlag( bool set ) {
		ActionSelectSkillBase find = Child;
		while ( find ) {
			find.IsSkillEnd = set;
			find = find.Child;
		}
	}

	public void AddExtraCoolTime( float add ) {
		float decreaseCoolTime = DecreaseSkillCoolTimeValue + ( DecreaseSkillCoolTimeValue * DecreaseSkillCoolTimeValueRatio );

		AddExtraCoolingTime += add - ( add * decreaseCoolTime ) - ( add * mOwnerPlayer.DecreaseSkillCoolTimeValue );
		if ( AddExtraCoolingTime <= 0.0f ) {
			return;
		}

		if ( PossibleToUse ) {
			mOnlyExtraCoolingTime = true;
			StartCoolTime();
		}
		else {
			CoolTime = Mathf.Max( 0.0f, CoolTime - add );
		}
	}

	public float GetMaxCoolTime() {
		return Mathf.Min( MaxCoolTime, ( mOnlyExtraCoolingTime ? 0.0f : mCalcMaxCoolTime ) + AddExtraCoolingTime );
	}

	public void SetAddActionAutoRelease( float releaseTime, int effId ) {
		SetAddAction = true;

		if ( effId > 0 ) {
			mAddActionEffId = effId;
			EffectManager.Instance.Play( m_owner, effId, EffectManager.eType.Common );
		}

		Invoke( "ReleaseSetAddAction", releaseTime );
	}

	public void SetOwnerPlayer( Player ownerPlayer ) {
		mOwnerPlayer = ownerPlayer;
	}

	protected virtual IEnumerator UpdateCoolTime() {
		if ( !mOnlyExtraCoolingTime ) {
			float decreaseCoolTime = DecreaseSkillCoolTimeValue + ( DecreaseSkillCoolTimeValue * DecreaseSkillCoolTimeValueRatio );

			mCalcMaxCoolTime = MaxCoolTime - ( MaxCoolTime * decreaseCoolTime ) - ( MaxCoolTime * mOwnerPlayer.DecreaseSkillCoolTimeValue ) - mDecreaseSkillCoolTimeSec;

#if UNITY_EDITOR
			if ( GameInfo.Instance.GameConfig.TestMode ) {
				mCalcMaxCoolTime = 3.0f;
			}
#endif

			if ( World.Instance.StageType == eSTAGETYPE.STAGE_TRAINING ) {
				mCalcMaxCoolTime = 3.0f;
			}

			// 1-3 스킬 사용 튜토리얼에선 쿨타임 강제로 3초만 적용
			if ( GameSupport.IsInGameTutorial() &&
				GameInfo.Instance.UserData.GetTutorialState() == (int)eTutorialState.TUTORIAL_STATE_Stage3Clear &&
				GameInfo.Instance.UserData.GetTutorialStep() == 1 ) {
				mCalcMaxCoolTime = 3.0f;
			}
		}

		Debug.Log( "액션 쿨타임 시작 : " + mCalcMaxCoolTime.ToString( "F3" ) + "초" );
		while ( CoolTime < GetMaxCoolTime() ) {
			CoolTime += Time.fixedDeltaTime;
			yield return mWaitForFixedUpdate;
		}

		Debug.Log( "액션 쿨타임 끝" );
		PossibleToUse = true;
		PassingCoolTime = false;
		AddExtraCoolingTime = 0.0f;
		mOnlyExtraCoolingTime = false;
	}

	protected virtual void UpdateMove( float speed ) {
		Vector3 dir = m_owner.Input.GetRawDirection();
		bool isAI = false;

		if ( m_param != null ) {
			mParamAI = m_param as ActionParamAI;

			if ( mParamAI != null ) {
				dir = Utility.GetDirectionVector( m_owner, mParamAI.Direction );
				isAI = true;
			}
		}

		if ( dir != Vector3.zero ) {
			if ( m_owner.aniEvent.IsAniPlaying( eAnimation.Run ) != eAniPlayingState.Playing ) {
				m_owner.PlayAniImmediate( eAnimation.Run );
			}

			if ( !isAI ) {
				Vector3 cameraRight = World.Instance.InGameCamera.transform.right;
				Vector3 cameraForward = World.Instance.InGameCamera.transform.forward;

				mDir = Vector3.zero;
				mDir.x = ( dir.x * cameraRight.x ) + ( dir.y * cameraForward.x );
				mDir.z = ( dir.x * cameraRight.z ) + ( dir.y * cameraForward.z );
				mDir.y = 0.0f;
			}
			else {
				mDir = dir;
			}

			m_owner.cmptRotate.UpdateRotation( mDir, true );
			m_owner.cmptMovement.UpdatePosition( mDir, speed, false );
		}
		else {
			m_owner.PlayAni( eAnimation.Idle01 );
		}
	}

	protected void ExecuteStartSkillBO() {
		ExecuteBattleOption( BattleOption.eBOTimingType.StartSkill );
		ExecuteBattleOption( BattleOption.eBOTimingType.DuringSkill );

		if ( mOwnerPlayer.boWeapon != null ) {
			mOwnerPlayer.boWeapon.Execute( BattleOption.eBOTimingType.StartSkill, TableId, null );
			mOwnerPlayer.boWeapon.Execute( BattleOption.eBOTimingType.DuringSkill, TableId, null );
		}

		if ( mOwnerPlayer.boSupporter != null ) {
			mOwnerPlayer.boSupporter.Execute( BattleOption.eBOTimingType.StartSkill, TableId, null );
		}
	}

	private void ChangeBOCharSkillSuperArmorDuration( BattleOption.sBattleOptionData boData, float addRatio ) {
		if ( boData == null ) {
			return;
		}
		else if ( boData.funcType != BattleOption.eBOFuncType.SuperArmor &&
				boData.funcType != BattleOption.eBOFuncType.SuperArmorTimeInc &&
				boData.funcType != BattleOption.eBOFuncType.ConditionalSuperArmor ) {
			return;
		}

		BOCharSkill.ChangeBattleOptionDuration( boData.battleOptionSetId, TableId, boData.duration + ( boData.duration * addRatio ) );
	}

	private void ReleaseSetAddAction() {
		SetAddAction = false;

		if ( mAddActionEffId > 0 ) {
			EffectManager.Instance.StopEffImmediate( mAddActionEffId, EffectManager.eType.Common, null );
		}
	}
}
