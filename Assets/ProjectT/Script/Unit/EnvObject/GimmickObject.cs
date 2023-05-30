
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using CodeStage.AntiCheat.ObscuredTypes;


public class GimmickObject : EnvObject {
    public enum eAttackTiming {
        None = 0,
        OnEnter,
        OnStay
    }

    public enum eCollisionCheckLayer {
        None = 0,
        Player,
        Enemy,
        All,
    }

    public enum eDamageType {
        Absolute = 0,
        Relative
    }


    [Header("[Gimmick Property]")]
    public eAttackTiming        AttackTiming    = eAttackTiming.None;
    public eCollisionCheckLayer ColCheckLayer   = eCollisionCheckLayer.Player;
    public bool                 KeepAlive       = false;
    public float                Tick            = 0.0f;
    public eDamageType          DamageType      = eDamageType.Absolute;
    public float                Damage          = 10;
    public bool                 OnlyDamage      = false;

    private float mCheckTime = 0.0f;


	public override void Init( int tableId, eCharacterType type, string faceAniControllerPath ) {
		m_attackPower = Damage;

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

		mCheckTime = Tick;
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
		if ( AttackTiming == eAttackTiming.None || m_curHp <= 0.0f || maxHitCount <= 0 ) {
			return false;
		}

		++mCurHitCount;
		if ( mCurHitCount >= maxHitCount ) {
			OnHit( attacker, toExecuteType );
		}
		else {
			if ( sndOnHit )
				SoundManager.Instance.PlaySnd( SoundManager.eSoundType.FX, sndOnHit, FSaveData.Instance.GetSEVolume() );

			if ( mPsDanger && ( mCurHitCount >= (int)( (float)maxHitCount * dangerPercentage ) ) )
				mPsDanger.gameObject.SetActive( true );

			transform.DOShakePosition( 0.2f, 0.15f, 8, 90.0f );
		}

		EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_REMOVE_HIT_TARGET, this );
		return true;
	}

	protected override void OnCollisionEnter( Collision col ) {
		if( AttackTiming != eAttackTiming.OnEnter ) {
			return;
		}

		Unit target = col.gameObject.GetComponent<Unit>();
		if( target == null ) {
			return;
		}

		OnEnter( target );
	}

	protected override void OnCollisionStay( Collision col ) {
		if( AttackTiming != eAttackTiming.OnStay ) {
			return;
		}

		Unit target = col.gameObject.GetComponent<Unit>();
		if( target == null ) {
			return;
		}

		if( mCheckTime >= Tick ) {
			OnEnter( target );
			mCheckTime = 0.0f;
		}
	}

	protected override void FixedUpdate() {
		if( World.Instance.IsPause || AttackTiming != eAttackTiming.OnStay ) {
			return;
		}

		mCheckTime += Time.fixedDeltaTime;
	}

	private void OnTriggerEnter( Collider col ) {
		if( AttackTiming != eAttackTiming.OnEnter ) {
			return;
		}

		Unit target = col.gameObject.GetComponent<Unit>();
		if( target == null ) {
			return;
		}

		OnEnter( target );
	}

	private void OnTriggerStay( Collider col ) {
		if( AttackTiming != eAttackTiming.OnStay ) {
			return;
		}

		Unit target = col.gameObject.GetComponent<Unit>();
		if( target == null ) {
			return;
		}

		if( mCheckTime >= Tick ) {
			if( OnEnter( target ) ) {
				mCheckTime = 0.0f;
			}
		}
	}

	private bool OnEnter( Unit target ) {
		switch( ColCheckLayer ) {
			case eCollisionCheckLayer.None:
				return false;

			case eCollisionCheckLayer.Player:
				if( target.gameObject.layer != (int)eLayer.Player ) {
					return false;
				}
				break;

			case eCollisionCheckLayer.Enemy:
				if( target.gameObject.layer != (int)eLayer.Enemy ) {
					return false;
				}
				break;

			case eCollisionCheckLayer.All:
				if( target.gameObject.layer != (int)eLayer.Player && target.gameObject.layer != (int)eLayer.Enemy ) {
					return false;
				}
				break;
		}

		OnHit( target, BattleOption.eToExecuteType.Unit );
		return true;
	}

	private void OnHit( Unit target, BattleOption.eToExecuteType toExecuteType ) {
		if( sndDie ) {
			SoundManager.Instance.PlaySnd( SoundManager.eSoundType.FX, sndDie, FSaveData.Instance.GetSEVolume() );
		}

		AniEvent.sEvent attackEvt = new AniEvent.sEvent();
		attackEvt.behaviour = eBehaviour.Attack;
		attackEvt.hitEffectId = 0;
		attackEvt.hitDir = eHitDirection.None;
		attackEvt.atkRatio = 1.0f;

		float damage = m_attackPower;
		if( DamageType == eDamageType.Relative ) {
			damage = target.maxHp * Damage;
		}

		AttackEvent atkEvent = new AttackEvent();
		atkEvent.SetWithSingleTarget( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, this, BattleOption.eToExecuteType.Unit, attackEvt,
									 damage, eAttackDirection.Skip, false, 0, EffectManager.eType.None, target.MainCollider, 0.0f, true, false, OnlyDamage );

		EventMgr.Instance.SendEvent( atkEvent );

		if( AllDropItemID > 0 || RandomDropItemID > 0 ) {
			DropItem();
		}

		if( !KeepAlive ) {
			if( mPsDead ) {
				mPsDead.transform.position += transform.position;
				mPsDead.transform.rotation = transform.rotation;
				mPsDead.gameObject.SetActive( true );

				EffectManager.Instance.RegisterStopEff( mPsDead, null );
			}

			Deactivate();
		}
	}
}
