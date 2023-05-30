
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using CodeStage.AntiCheat.ObscuredTypes;


public class EnvObject : Unit {
    [Header("[Env Obj Property]")]
    public int                  maxHitCount     = 1;
    public float                power           = 10;
    public GameObject           OriginalObj     = null;
    public GameObject           DestroyedObj    = null;

    [Header("[Projectile]")]
    public Projectile projectile;

    [Header("[Drop Item]")]
    public int AllDropItemID    = 0;
    public int AllDropItemValue = 0;
    public int RandomDropItemID = 0;
    public int RandomItemValue  = 0;

    [Header("[Effect]")]
    public ParticleSystem psDanger;
    public float dangerPercentage = 0.7f;
    public ParticleSystem psDead;

    [Header("[Sound]")]
    public AudioClip sndOnHit;
    public AudioClip sndDie;


    protected int                       mCurHitCount    = 0;
    protected Projectile                mProjectile     = null;
    protected ParticleSystem            mPsDanger       = null;
    protected ParticleSystem            mPsDead         = null;
    private AniEvent.sEvent             mAniEvt         = new AniEvent.sEvent();
    private AniEvent.sProjectileInfo    mPjtInfo        = new AniEvent.sProjectileInfo();


	public override void Init( int tableId, eCharacterType type, string faceAniControllerPath ) {
		m_attackPower = power;

		m_rigidBody = GetComponent<Rigidbody>();
		m_originalMass = m_rigidBody.mass = 10000.0f;

		MainCollider = GetComponentInChildren<UnitCollider>();
		if( MainCollider == null ) {
			MainCollider = gameObject.AddComponent<UnitCollider>();
			MainCollider.Init();
		}

		m_contactNormal = Vector3.zero;

		m_listCmpt.Clear();
		m_listCmpt.AddRange( GetComponentsInChildren<CmptBase>() );

		m_actionSystem = gameObject.AddComponent<ActionSystem>();
		m_actionSystem.Init( this );
		m_actionSystem.AddAction( GetComponentsInChildren<ActionBase>() );

		m_cmptBuffDebuff = gameObject.AddComponent<CmptBuffDebuff>();
		m_cmptBuffDebuff.Unused = true;

		m_originalPos = transform.localPosition;
		m_scale = 1.0f;

		m_attacker = null;
		m_mainTarget = null;
		m_evadedTarget = null;

		isGrounded = true;
		isFloating = false;

		m_pause = false;
		m_pauseFrame = false;

		m_charType = type;

		isDying = false;
		isVisible = false;

		if( projectile ) {
			GameObject gObj = projectile.gameObject;
			mProjectile = ResourceMgr.Instance.Instantiate<Projectile>( ref gObj );
		}

		if( psDanger ) {
			GameObject gObj = psDanger.gameObject;
			mPsDanger = ResourceMgr.Instance.Instantiate<ParticleSystem>( ref gObj );
			mPsDanger.transform.SetParent( transform );
			Utility.InitTransform( mPsDanger.gameObject, mPsDanger.transform.position, mPsDanger.transform.rotation, mPsDanger.transform.localScale );
			mPsDanger.gameObject.SetActive( false );
		}

		if( psDead ) {
			GameObject gObj = psDead.gameObject;
			mPsDead = ResourceMgr.Instance.Instantiate<ParticleSystem>( ref gObj );
			mPsDead.gameObject.SetActive( false );
		}
	}

	public override void SetData( int tableId ) {
	}

	public override void Activate() {
		IsShow = true;
		gameObject.SetActive( true );

		m_prevPos = transform.position;

		if( alwaysKinematic ) {
			m_posOnGround = transform.position;
		}

		m_curHp = 1;
		mCurHitCount = 0;

		if( mPsDanger ) {
			mPsDanger.gameObject.SetActive( false );
		}

		if( mPsDead ) {
			mPsDead.gameObject.SetActive( false );
		}

		if( DestroyedObj ) {
			DestroyedObj.SetActive( false );
		}
	}

	public override void Deactivate() {
		if( DestroyedObj ) {
			m_curHp = 0.0f;

			if( OriginalObj ) {
				OriginalObj.SetActive( false );
				DestroyedObj.SetActive( true );
			}
		}
		else {
			base.Deactivate();
		}
	}

	public override bool OnHit( Unit attacker, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent attackerAniEvt, ObscuredFloat damage,
								ref bool isCritical, ref eHitState hitState, Projectile projectile, bool isUltimateSkill, bool skipMaxDamageRecord ) {
		if ( m_curHp <= 0.0f ) {
			return false;
		}

		++mCurHitCount;
		if ( mCurHitCount >= maxHitCount ) {
			OnHit( toExecuteType );
		}
		else {
			if ( sndOnHit ) {
				SoundManager.Instance.PlaySnd( SoundManager.eSoundType.FX, sndOnHit, FSaveData.Instance.GetSEVolume() );
			}

			if ( mPsDanger && ( mCurHitCount >= (int)( (float)maxHitCount * dangerPercentage ) ) ) {
				mPsDanger.gameObject.SetActive( true );
			}

			transform.DOShakePosition( 0.2f, 0.15f, 8, 90.0f );
		}

		EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_REMOVE_HIT_TARGET, this );
		return true;
	}

	public bool IsOverHit() {
		return mCurHitCount >= maxHitCount;
	}

	protected override void DropItem() {
		base.DropItem();
		DropItemMgr.Instance.DropItem( this );
	}

	private void OnHit( BattleOption.eToExecuteType toExecuteType ) {
		if( sndDie ) {
			SoundManager.Instance.PlaySnd( SoundManager.eSoundType.FX, sndDie, FSaveData.Instance.GetSEVolume() );
		}

		if( mProjectile ) {
			mProjectile.Fire( this, toExecuteType, mAniEvt, mPjtInfo, this, -1 );
		}

		if( AllDropItemID > 0 || RandomDropItemID > 0 ) {
			DropItem();
		}

		if( mPsDead ) {
			mPsDead.transform.position += transform.position;
			mPsDead.transform.rotation = transform.rotation;
			mPsDead.gameObject.SetActive( true );

			EffectManager.Instance.RegisterStopEff( mPsDead, null );
		}

		Deactivate();
	}
}
