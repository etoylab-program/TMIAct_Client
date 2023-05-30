
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public partial class Projectile : MonoBehaviour
{
    public enum eType
    {
        Forward = 0,
        Parabola,
        Zigzag,
        Effect,
        AnimationOnTarget,
        Homing,
        TargetListHoming,
        Stay,
    }

    public enum eDamageType
    {
        None = 0,
        Once,
        Dot,
    }

    public enum eStartChildType
    {
        ParentHit = 0,
        Delay,
    }

    public enum EProjectileAtkAttr
    {
        NORMAL = 0,
        KNOCKBACK,
        DOWN,
        UPPER,
        STUN,
        GROGGY,
        ONLY_DAMAGE,
    }


    [System.Serializable]
	public class sChildInfo
	{
		public Projectile		Child			= null;
		public eStartChildType	StartChildType	= eStartChildType.ParentHit;
		public float			StartChildDelay	= 0.0f;
		public bool				ChildIgnoreY	= false;
	}


    // Property
    [HideInInspector] public bool DontDestroyOnPlayDirector = false;
    [HideInInspector] public GameObject main;
    [HideInInspector] public string audioPath;
    [HideInInspector] public float volume = 0.4f;
    [HideInInspector] public eDamageType damageType = eDamageType.None;
    [HideInInspector] public float atkRatio = 1.0f;
    [HideInInspector] public EAttackAttr AttackAttr = EAttackAttr.NONE;
    [HideInInspector] public bool IsMelee = false;
    [HideInInspector] public bool enableLayerEnemy = true;
    [HideInInspector] public bool enableLayerFloor = false;
    [HideInInspector] public bool enableLayerWall = false;
    [HideInInspector] public bool enableLayerWallInside = false;
    [HideInInspector] public float collisionDelay = 0.0f;
    [HideInInspector] public bool passEnemy = false;
    [HideInInspector] public float tick = 0.0f;
    [HideInInspector] public eType type = eType.Forward;
    [HideInInspector] public bool hasTarget = true;
    [HideInInspector] public float value1 = 0.0f; 
    [HideInInspector] public float value2 = 0.0f; 
    [HideInInspector] public float accel = 0.0f;
    [HideInInspector] public float duration = 0.0f;
    [HideInInspector] public bool prepareHoming = true; // 유도탄에 준비시간을 넣을건지 여부
    [HideInInspector] public EasingFunction.Ease ease = EasingFunction.Ease.Linear;
    [HideInInspector] public bool ToTarget = false;
	[HideInInspector] public bool onGround = true;
    [HideInInspector] public EProjectileAtkAttr ProjectileAtkAttr = EProjectileAtkAttr.NORMAL;
    [HideInInspector] public bool homingRandomTarget = true;
    [HideInInspector] public bool addRandomPos = false;
	[HideInInspector] public bool SelfRotate = false;
    [HideInInspector] public bool skipWhenTargetDie = false;

    [HideInInspector] public float delayedHide;
    [HideInInspector] public ParticleSystem psHit;
    [HideInInspector] public string audioHitPath;
    [HideInInspector] public float volumeHit = 0.4f;

    [HideInInspector] public float ShakeDuration = 0.0f;
    [HideInInspector] public float ShakePower = 1.0f;
    [HideInInspector] public float ShakeSpeed = 1.0f;

    // Child Property
    [HideInInspector] public Projectile child;
    [HideInInspector] public eStartChildType startChildType = eStartChildType.ParentHit;
    [HideInInspector] public float startChildDelay = 0.0f;
    [HideInInspector] public bool childIgnoreY = false;

    // Extend child
    public sChildInfo[] ExtendChildArr;

	// Battle Option
	public int[] _BOSetIds;

    public System.Func<Unit, bool>  OnHitFunc   = null;
    public System.Action            OnHideFunc  = null;

    public int                          OwnerActionTableId  { get; private set; } = 0;
    public ActionBase                   OwnerAction         { get; private set; } = null;
    public BattleOption.eToExecuteType  ToExecuteType       { get; private set; } = 0;
	public BoxCollider					BoxCol				{ get { return m_boxCol; } }

    private bool m_endUpdate = false;
    private float m_time = 0.0f;
    private float m_t = 0.0f;
    //private System.TimeSpan m_ts;
    //private long m_dotTicks;
    private bool mbFirstHitCheck = false;

    private bool mbStartCheckTime = false;
    private float mCheckTime = 0.0f;

    private List<GameObject>    mListDotTarget  = new List<GameObject>();
    private List<GameObject>    mListOnceTarget = new List<GameObject>();

    private Rigidbody m_rigidBody = null;

    private BoxCollider m_boxCol = null;
    private ParticleSystem m_psMain = null;
    private ProjectileParticle m_projectileParticle = null;
    private Animation m_aniMain = null;
    private Vector3 m_originalPosMain;
    private Vector3 m_originalRotMain;

    private EProjectileAtkAttr projectileAtkAttr = EProjectileAtkAttr.NORMAL;

    private Vector3 m_pos;
    private bool m_hit = false;
    private System.Action m_onHit = null;
    private System.Func<int, bool>[] mOnHitForExtendChild;

    private AudioClip fxSndFire = null;
    private AudioClip fxSndHit = null;

    // Raycast Collision
    private float m_minExtent;
    private float m_partialExtent;
    private float m_sqrMinExtent;
    private Vector3 m_prevPos;
    private float m_skinWidth = 0.1f;

    // Info
    private Unit m_owner;
    private AniEvent.sEvent m_evt;
    private AniEvent.sProjectileInfo m_info;
    private Vector3 m_rootPos;
    private Vector3 m_parentRot;
    private Vector3 m_bonePos;
    //private Unit m_target = null;
    //private List<Unit> m_listTarget = new List<Unit>();
    public UnitCollider TargetCollider { get; private set; } = null;
    private List<UnitCollider>  mListTargetCollider = new List<UnitCollider>();

	private WaitForFixedUpdate mWaitForFixedUpdate = new WaitForFixedUpdate();

    //== Test
    [HideInInspector] public Unit         testTarget = null;
    [HideInInspector] public GameObject   testParent = null;
    //==
    public Unit                     owner               { get { return m_owner; } } 
    public AniEvent.sEvent          evt                 { get { return m_evt; } }
    public AniEvent.sProjectileInfo info                { get { return m_info; } }
    public float                    OriginalAtkRatio    { get; private set; }       = 0.0f;
    public float                    AddedAtkRatio       { get; private set; }       = 0.0f;
	public List<BOProjectile>       ListBO              { get; protected set; } = new List<BOProjectile>();


    private void Awake()
    {
        if (m_rigidBody == null)
        {
            m_rigidBody = gameObject.AddComponent<Rigidbody>();
            m_rigidBody.useGravity = false;
            m_rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            m_rigidBody.isKinematic = true;
        }

        if (main != null)
        {
            m_boxCol = main.GetComponent<BoxCollider>();
            m_psMain = main.GetComponent<ParticleSystem>();
            m_aniMain = main.GetComponent<Animation>();

            m_originalPosMain = main.transform.position;
            m_originalRotMain = main.transform.eulerAngles;

            //if (m_psMain && m_projectileParticle == null)
            //    m_projectileParticle = m_psMain.gameObject.AddComponent<ProjectileParticle>();
        }

        Init();

        if (psHit != null)
            psHit.gameObject.SetActive(false);

        if(!string.IsNullOrEmpty(audioPath))
            fxSndFire = ResourceMgr.Instance.LoadFromAssetBundle("sound", "Sound/" + audioPath) as AudioClip;

        if (!string.IsNullOrEmpty(audioHitPath))
            fxSndHit = ResourceMgr.Instance.LoadFromAssetBundle("sound", "Sound/" + audioHitPath) as AudioClip;
    }

    private void Init()
    {
		ListBO.Clear();
		for ( int i = 0; i < _BOSetIds.Length; i++ ) {
			BOProjectile bo = new BOProjectile( _BOSetIds[i], m_owner );
			ListBO.Add( bo );
		}

        m_endUpdate = false;
        m_time = 0.0f;
        m_t = 0.0f;
        m_hit = false;

        //m_listTarget.Clear();
        mListTargetCollider.Clear();
        mListOnceTarget.Clear();

        Hide(true);
        InitRaycastCollision();

        if (m_projectileParticle)
            m_projectileParticle.Init(m_owner);

        if (child != null)
            child.Hide(true);

        if ( ExtendChildArr != null ) {
            for ( int i = 0; i < ExtendChildArr.Length; i++ ) {
                ExtendChildArr[i].Child.Hide( true );
			}
		}
    }

    private void InitRaycastCollision()
    {
        if (m_rigidBody == null || m_boxCol == null)
            return;

        m_prevPos = m_rigidBody.position;
        m_minExtent = Mathf.Min(Mathf.Min(m_boxCol.bounds.extents.x, m_boxCol.bounds.extents.y), m_boxCol.bounds.extents.z);
        m_partialExtent = m_minExtent * (1.0f - m_skinWidth);
        m_sqrMinExtent = m_minExtent * m_minExtent;
    }

    public void Show()
    {
        if (main == null)
            return;

        main.gameObject.SetActive( true );

        ParticleSystem ps = main.GetComponentInChildren<ParticleSystem>();
        if ( ps ) {
            ps.Stop( true, ParticleSystemStopBehavior.StopEmittingAndClear );
            ps.Play( true );
        }

        if (m_boxCol != null)
        {
            m_boxCol.enabled = false;
            StartCoroutine("EnableCollider", collisionDelay);
        }
    }

    public void Hide(bool init, bool hideImmediate = false)
    {
        if (main == null)
            return;

        if (m_boxCol != null)
            m_boxCol.enabled = false;

        if (init || delayedHide <= 0.0f || hideImmediate)
        {
            ParticleSystem[] ps = main.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < ps.Length; i++)
            {
                ps[i].Stop();
                ps[i].Simulate(0.0f, true, true);
            }

            main.gameObject.SetActive(false);
            mbStartCheckTime = false;

            if (World.Instance.ProjectileMgr != null)
                World.Instance.ProjectileMgr.RemoveProjectile(this);

            for(int i = 0; i < mListDotTarget.Count; i++)
            {
                Unit target = mListDotTarget[i].GetComponent<Unit>();
                if(target)
                {
                    target.StayOnProjectile = null;
                }
            }

            mListDotTarget.Clear();

            if(!init)
            {
                OnHideFunc?.Invoke();
            }
        }
        else
            StartCoroutine("DelayedHide");
    }

    public void End()
    {
        m_endUpdate = true;
    }

    private IEnumerator DelayedHide()
    {
        OnHideFunc?.Invoke();
        yield return new WaitForSeconds(delayedHide);

        ParticleSystem[] ps = main.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < ps.Length; i++)
            ps[i].Stop();

        for (int i = 0; i < mListDotTarget.Count; i++)
        {
            if( mListDotTarget[i] == null ) {
                continue;
			}

            Unit target = mListDotTarget[i].GetComponent<Unit>();
            if (target)
            {
                target.StayOnProjectile = null;
            }
        }

        mListDotTarget.Clear();

        main.gameObject.SetActive(false);
        mbStartCheckTime = false;

        if (World.Instance.ProjectileMgr != null)
            World.Instance.ProjectileMgr.RemoveProjectile(this);
    }

	public void ExecuteBattleOption( BattleOption.eBOTimingType timingType, int actionTableId, Projectile projectile, bool skipWeaponBO = false ) {
		for ( int i = 0; i < ListBO.Count; i++ ) {
			ListBO[i].Execute( timingType, 0, null );
		}
	}

	public bool IsActivate() {
		if ( ( main && main.gameObject.activeSelf ) || ( child && child.IsActivate() ) ) {
			return true;
		}

        if ( ExtendChildArr != null ) {
            for ( int i = 0; i < ExtendChildArr.Length; i++ ) {
                if ( ExtendChildArr[i].Child.IsActivate() ) {
                    return true;
				}
			}
		}

		return false;
	}

	public void SetAddAtkRatio( float add ) {
        AddedAtkRatio = add;
	}

	public void Fire( Unit owner, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent evt, AniEvent.sProjectileInfo info, Unit target,
                      int ownerActionTableId, Projectile parent = null, ActionBase ownerCurrentAction = null,
                      bool rootPosOnTarget = false, bool rootPosIsParentPos = false ) {

		if ( main == null || evt == null ) {
			return;
		}

		mListOnceTarget.Clear();

		m_owner = owner;
		m_info = info;

		m_evt = null;
		if ( evt != null ) {
			m_evt = AniEvent.CreateEvent( evt );
		}

		if ( parent == null ) {
			OriginalAtkRatio = evt.atkRatio;
			m_evt.atkRatio = ( OriginalAtkRatio + AddedAtkRatio ) * atkRatio;
		}
		else {
			OriginalAtkRatio = parent.OriginalAtkRatio;
			m_evt.atkRatio = ( parent.OriginalAtkRatio + AddedAtkRatio ) * atkRatio;
		}

		ToExecuteType = toExecuteType;
		OwnerActionTableId = ownerActionTableId;
		OwnerAction = ownerCurrentAction;

		if ( !rootPosIsParentPos ) {
			Unit basePosUnit = !rootPosOnTarget ? m_owner : target;

			if ( basePosUnit ) {
				m_bonePos = Vector3.zero;
				Vector3 boneRot = Vector3.zero;

				if ( m_info != null && m_info.boneName != "None" ) {
					Transform bone = basePosUnit.aniEvent.GetBoneByName( m_info.boneName );

					if ( bone != null ) {
						m_bonePos = bone.transform.position;
						boneRot = Quaternion.LookRotation( bone.transform.forward ).eulerAngles;
					}
				}

				m_rootPos = parent == null ? basePosUnit.transform.position : Vector3.zero;
				if ( m_bonePos != Vector3.zero ) {
					m_rootPos = m_bonePos;
				}

				m_parentRot = Vector3.zero;

                if ( m_info != null ) {
                    if ( m_info.followParentRot ) {
                        m_rootPos = ( basePosUnit.transform.rotation * m_info.addedPosition ) + m_rootPos;

                        if ( boneRot == Vector3.zero ) {
                            m_parentRot = basePosUnit.transform.rotation.eulerAngles;
                        }
                        else {
                            m_parentRot = boneRot;
                        }
                    }
                    else {
                        m_rootPos += m_info.addedPosition;
                    }
                }
			}
		}
		else if ( parent ) {
			m_rootPos = parent.transform.position;
		}

        Vector3 addedRot = ( m_info != null ) ? m_info.addedRotation : Vector3.zero;
		main.transform.rotation = Quaternion.Euler( m_parentRot + addedRot );

		if ( target ) {
			TargetCollider = target.GetNearestColliderFromPos( m_owner.transform.position );
		}

		m_onHit = null;
		StartProjectile();

        if ( World.Instance.ProjectileMgr != null ) {
            World.Instance.ProjectileMgr.AddActiveProjectile( this );
        }

        if ( child ) {
            StartChildProjectile();
        }

        mOnHitForExtendChild = null;

        if ( ExtendChildArr != null ) {
            StartExtendChildProjectile();
		}

        if ( fxSndFire ) {
            SoundManager.Instance.PlaySnd( SoundManager.eSoundType.FX, fxSndFire, volume * FSaveData.Instance.GetSEVolume() );
        }

		ExecuteBattleOption( BattleOption.eBOTimingType.OnProjectileFire, 0, this );
	}

	private void StartProjectile()
    {
        Init();
        Show();

        switch (type)
        {
            case eType.Forward:
                StopCoroutine("UpdateForward");
                StartCoroutine("UpdateForward");
                break;

            case eType.Parabola:
                StopCoroutine("UpdateParabola");
                StartCoroutine("UpdateParabola");
                break;

            case eType.Zigzag:
                StopCoroutine("UpdateZigzag");
                StartCoroutine("UpdateZigzag");
                break;

            case eType.Homing:
                StopCoroutine("UpdateHoming");
                StartCoroutine("UpdateHoming");
                break;

            case eType.TargetListHoming:
                StopCoroutine("UpdateTargetListHoming");
                StartCoroutine("UpdateTargetListHoming");
                break;

            case eType.Effect:
                StopCoroutine("UpdateEffect");
                StartCoroutine("UpdateEffect");
                break;

            case eType.AnimationOnTarget:
                StopCoroutine("UpdateAnimationOnTarget");
                StartCoroutine("UpdateAnimationOnTarget");
                break;

            case eType.Stay:
                break;
        }
    }

	private IEnumerator EnableCollider( float delay ) {
        if( m_boxCol == null ) {
            yield break;
        }

		bool end = false;
		float time = 0.0f;

		while( !end ) {
			time += Time.fixedDeltaTime;
			if( time >= delay ) {
				m_boxCol.enabled = true;
				mbFirstHitCheck = false;
                mbStartCheckTime = true;
                mCheckTime = tick;

                mListDotTarget.Clear();

				if( damageType == eDamageType.Dot && duration > 0.0f ) {
					StopCoroutine( "DisableCollider" );
					StartCoroutine( "DisableCollider", ( collisionDelay + duration ) - tick );
				}

				end = true;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	private IEnumerator DisableCollider(float delay)
    {
        if (m_boxCol == null)
            yield break;

        bool end = false;
        float time = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!end)
        {
            time += Time.fixedDeltaTime;
            if (time >= delay)
            {
                m_boxCol.enabled = false;
                end = true;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    private void CheckRaycastCollision()
    {
        Vector3 v1 = m_rigidBody.position;
        Vector3 v2 = m_prevPos;

        /*if (type == eType.Forward)
        {
            float targetHeadY = m_target.GetHeadPos().y;
            if (m_rigidBody.position.y >= targetHeadY)
            {
                v1.y = targetHeadY;
                v2.y = targetHeadY;
            }
        }*/

        Vector3 deltaPos = v1 - v2;
        if (m_boxCol.enabled && deltaPos.sqrMagnitude > m_sqrMinExtent)
        {
            int layer = 0;
            if(enableLayerEnemy == true)
                layer |= Utility.GetEnemyLayer((eLayer)m_owner.gameObject.layer);
            //if (enableLayerEnemy == true)
            //    layer |= 1 << (int)eLayer.DefenceObject;
            if (enableLayerFloor == true)
                layer |= 1 << (int)eLayer.Floor;
            if (enableLayerWall == true)
                layer |= 1 << (int)eLayer.Wall;
            //if (enableLayerWallInside == true)
            //    layer |= 1 << (int)eLayer.Wall_Inside;

            RaycastHit hitInfo;
            if (Physics.Raycast(v2, deltaPos.normalized, out hitInfo, deltaPos.sqrMagnitude, layer))
            {
                if (hitInfo.collider == null)
                    return;

                //m_rigidBody.position = hitInfo.point - (deltaPos / deltaPos.sqrMagnitude) * m_partialExtent;

                if (!passEnemy)
                {
                    transform.position = hitInfo.point - (deltaPos / deltaPos.sqrMagnitude) * m_partialExtent;
                }

                OnTriggerEnter(hitInfo.collider);
            }
            else
            {
                Collider[] cols = Physics.OverlapBox(v1, m_boxCol.size * 0.5f, m_boxCol.transform.rotation, layer);
                for(int i = 0; i < cols.Length; i++)
                {
                    if(cols[i] == null)
                    {
                        continue;
                    }

                    OnTriggerEnter(cols[i]);
                }
            }
        }

        m_prevPos = m_rigidBody.position;
    }

    private IEnumerator UpdateForward()
    {
        Vector3 forward = m_owner.transform.forward;
        Vector3 right = m_owner.transform.right;
        Vector3 up = m_owner.transform.up;

        Transform bone = owner.aniEvent.GetBoneByName(m_info.boneName);
        if (bone)
        {
            forward = bone.transform.forward;
            //right = bone.transform.right;
            //up = bone.transform.up;
        }

        if(ToTarget)
        {
			/*
            Vector3 v1 = (m_owner.transform.position + m_owner.transform.forward) - m_owner.transform.position;
			Vector3 v2 = m_owner.transform.forward;
			if (TargetCollider)
			{
				v2 = TargetCollider.Owner.transform.position - m_owner.transform.position;
			}

            float angle = Vector3.SignedAngle(v1.normalized, v2.normalized, up);
            forward = Quaternion.AngleAxis(angle, up) * forward;
			*/
			forward = m_owner.transform.forward;
		}
        else if (m_info.useAddRotOnFire || value2 > 0.0f)
        {
            forward = (Quaternion.AngleAxis(m_info.addedRotOnFire.x, right) * Quaternion.AngleAxis(m_info.addedRotOnFire.y + value2, up)) * forward;
        }

        if (m_info.ignoreYAxis)
            forward.y = 0.0f;

		if (SelfRotate)
		{
			main.transform.rotation = Quaternion.LookRotation(forward);
		}

		Vector3 destPos = m_rootPos + (forward * value1);
        Vector3 pos = Vector3.zero;

        transform.position = m_rootPos;

        m_prevPos = m_rootPos;
        m_endUpdate = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate && m_owner.curHp > 0.0f)
        {
            m_time += Time.fixedDeltaTime / duration;
            m_t = Mathf.SmoothStep(0.0f, 1.0f, m_time);
            if (m_t >= 1.0f)
                m_endUpdate = true;

            pos.x = EasingFunction.GetEasingFunction(ease)(m_rootPos.x, destPos.x, m_t);
            pos.y = EasingFunction.GetEasingFunction(ease)(m_rootPos.y, destPos.y, m_t);
            pos.z = EasingFunction.GetEasingFunction(ease)(m_rootPos.z, destPos.z, m_t);

            m_rigidBody.MovePosition(pos);

            CheckRaycastCollision();
            yield return mWaitForFixedUpdate;
        }

        Hide(false);
    }

    private IEnumerator UpdateParabola()
    {
        Vector3 forward = m_owner.transform.forward;
        Vector3 right = m_owner.transform.right;
        Vector3 up = m_owner.transform.up;

        Transform bone = owner.aniEvent.GetBoneByName(m_info.boneName);
        if (bone)
        {
            forward = bone.transform.forward;
            right = bone.transform.right;
            up = bone.transform.up;
        }

        if (m_info.useAddRotOnFire)
        {
            forward = (Quaternion.AngleAxis(m_info.addedRotOnFire.x, right) * Quaternion.AngleAxis(m_info.addedRotOnFire.y, up)) * forward;
        }

        Vector3 targetPos = Vector3.zero;
        /*if(hasTarget && TargetCollider)
        {
            targetPos = TargetCollider.Owner.posOnGround;
        }
        else*/
        {
            /*
            float f = 2.0f;
            if(TargetCollider)
            {
                f = Utility.GetDistanceWithoutY(m_owner.transform.position, TargetCollider.GetCenterPos());
            }
            */

            targetPos = m_owner.posOnGround + (forward * (value1 * 5.0f));
            targetPos.y = m_owner.posOnGround.y;
        }

        float dist = Utility.GetDistanceWithoutY(m_owner.transform.position, targetPos);

        Vector3 destPos = targetPos;
        Vector3 pos = Vector3.zero;
        Vector3 p = Vector3.zero;

        transform.position = m_rootPos;

        m_prevPos = m_rootPos;
        m_endUpdate = false;

        value2 = value2 == 0.0f ? 1.0f : value2;
        duration = dist / (10.0f * value2);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate && m_owner.curHp > 0.0f)
        {
            m_time += Time.fixedDeltaTime / duration;
            m_t = Mathf.SmoothStep(0.0f, 1.0f, m_time);
            if (m_t >= 1.0f)
                m_endUpdate = true;

            p = (Vector3.Lerp(m_rootPos, destPos, 0.5f)) + (Vector3.up * value1);
            pos = Utility.Bezier(m_rootPos, p, destPos, m_t);

            m_rigidBody.MovePosition(pos);

            CheckRaycastCollision();
            yield return mWaitForFixedUpdate;
        }

        Hide(false);
    }

    private IEnumerator UpdateZigzag()
    {
        Vector3 prepareDir = (-m_owner.transform.forward + Vector3.up + (m_owner.transform.right * (float)Random.Range(-1, 2))).normalized * value2;
        Vector3 targetDir = Vector3.zero;
        Vector3 targetPos = TargetCollider != null ? TargetCollider.GetCenterPos() : m_owner.transform.position + (m_owner.transform.forward * 3.0f);
        Vector3 v = Vector3.zero;
        Vector3 pos = Vector3.zero;
        Quaternion q = Quaternion.identity;
        float speed = value1;
        float endTime = 0.0f;

        transform.position = m_rootPos;
        m_prevPos = m_rootPos;
        m_endUpdate = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate && m_owner.curHp > 0.0f)
        {
            endTime += Time.fixedDeltaTime;
            if (endTime >= 3.0f)
                m_endUpdate = true;

            m_time += Time.fixedDeltaTime / duration;
            m_t = Mathf.SmoothStep(1.0f, 0.0f, m_time);

            if (m_t > 0.0f)
                targetDir = (targetPos - m_rigidBody.position).normalized;
            else
                transform.rotation = Quaternion.LookRotation(targetDir);

            v = (prepareDir * speed * m_t) + (targetDir * speed * (1.0f - m_t));

            speed = EasingFunction.GetEasingFunction(ease)(value1, value1 * accel, 1.0f - m_t);
            pos = m_rigidBody.position + (v * Time.fixedDeltaTime);

            m_rigidBody.MovePosition(pos);

            CheckRaycastCollision();
            yield return mWaitForFixedUpdate;
        }

        Hide(false);
    }

    private IEnumerator UpdateHoming()
    {
        transform.position = m_rootPos;
        m_prevPos = m_rootPos;

        //m_target = m_owner.GetRandomTarget();
        if (homingRandomTarget || TargetCollider == null)
        {
            TargetCollider = m_owner.GetRandomTargetCollider();
        }

        if (TargetCollider != null)
        {
            m_endUpdate = false;
            m_time = 0.0f;
            m_t = 0.0f;

            float f = 1.0f;
            if (Random.Range(0, 1) == 0)
                f = -1.0f;

            Vector3 v1 = m_owner.transform.forward;
            if (prepareHoming)
            {
                v1 += (Vector3.up * ((float)Random.Range(0, 3) * 0.7f)) + (m_owner.transform.right * f * ((float)Random.Range(0, 3) * 0.7f));
            }

            Vector3 targetPos = TargetCollider.GetCenterPos();
            Vector3 v = Vector3.zero;
            Vector3 v2 = (targetPos - transform.position).normalized;

            float endTime = 0.0f;
            float checkDelayTime = 0.0f;

            //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            while (!m_endUpdate && m_owner.curHp > 0.0f)
            {
                endTime += Time.fixedDeltaTime;
                if(duration > 0.0f && endTime >= duration)
                {
                    m_endUpdate = true;
                }

                checkDelayTime += Time.fixedDeltaTime;

                m_time += Time.fixedDeltaTime / 0.5f;
                m_t = Mathf.SmoothStep(0.0f, 1.0f, m_time);

                if (TargetCollider.Owner && TargetCollider.Owner.IsActivate() && TargetCollider.Owner.curHp > 0.0f)
                {
                    if (checkDelayTime >= value2)
                    {
                        targetPos = TargetCollider.GetCenterPos();
                        v2 = (targetPos - transform.position).normalized;

                        checkDelayTime = 0.0f;
                    }

                    //v2 = (targetPos - transform.position).normalized;
                    v = ((v1 * (1.0f - m_t)) + (v2 * m_t)).normalized;

                    if(main)
                    {
                        main.transform.rotation = Quaternion.LookRotation((TargetCollider.GetCenterPos() - transform.position).normalized);
                    }
                }
                else
                {
                    v = v1;
                    if (m_t >= 1.0f)
                    {
                        m_endUpdate = true;
                    }
                }

                Vector3 vRigidBodyPos = m_rigidBody.position + (v * value1 * Time.fixedDeltaTime);
                if (0.1f <= Vector3.Distance(vRigidBodyPos, targetPos))
                {
                    m_rigidBody.MovePosition(vRigidBodyPos);
                }
                
                CheckRaycastCollision();

                yield return mWaitForFixedUpdate;
            }
        }

        Hide(false);
    }

    private IEnumerator UpdateTargetListHoming()
    {
        float randPosX = 0.0f;
        float randPosY = 0.0f;
        float randDelay = 0.0f;

        if(addRandomPos) // 포지션에 약간의 랜덤 값을 더하기 위해 사용 (+ 시작 딜레이)
        {
            randPosX = Utility.GetRandom(-0.5f, 0.5f, 10.0f);
            randPosY = Utility.GetRandom(-0.5f, 0.5f, 10.0f);
            randDelay = Utility.GetRandom(0.0f, 0.3f, 10.0f);
        }

        transform.position = m_rootPos + new Vector3(randPosX, randPosY, 0.0f);
        m_prevPos = m_rootPos + new Vector3(randPosX, randPosY, 0.0f);

        /*m_listTarget.Clear();
        m_listTarget.AddRange(m_owner.GetTargetList());
        m_owner.SortTargetListByNearDistance(ref m_listTarget);*/

        /*m_target = null;
        if (m_listTarget.Count > 0)
            m_target = m_listTarget[0];*/

        mListTargetCollider.Clear();
        mListTargetCollider.AddRange(m_owner.GetTargetColliderList(true));

        if (mListTargetCollider.Count > 0)
        {
            m_owner.SortTargetColliderListByNearDistance(ref mListTargetCollider);
            TargetCollider = mListTargetCollider[0];

            m_endUpdate = false;
            m_time = 0.0f;
            m_t = 0.0f;
            float endTime = 0.0f;
            float checkDelayTime = 0.0f;
            bool start = false;

            Vector3 v = Vector3.zero;

            //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            while (!m_endUpdate && m_owner.curHp > 0.0f && mListTargetCollider.Count > 0)
            {
                if (!start)
                {
                    checkDelayTime += Time.fixedDeltaTime;
                    if (checkDelayTime >= randDelay)
                    {
                        start = true;
                    }
                }
                else
                {
                    endTime += Time.fixedDeltaTime;
                    if (duration > 0.0f && endTime >= duration)
                    {
                        m_endUpdate = true;
                    }

                    m_time += Time.fixedDeltaTime / 0.5f;
                    m_t = Mathf.SmoothStep(0.0f, 1.0f, m_time);

                    Vector3 targetPos = TargetCollider.GetCenterPos();
                    transform.rotation = Quaternion.LookRotation(new Vector3(targetPos.x, 0.0f, targetPos.z));

                    v = (targetPos - transform.position).normalized;
                    m_rigidBody.MovePosition(m_rigidBody.position + (v * value1 * Time.fixedDeltaTime));

                    CheckRaycastCollision();

                    if (TargetCollider.Owner.curHp <= 0.0f || !TargetCollider.Owner.IsActivate())
                    {
                        mListTargetCollider.Remove(TargetCollider);
                        if (mListTargetCollider.Count > 0)
                        {
                            m_owner.SortTargetColliderListByNearDistance(ref mListTargetCollider);
                            TargetCollider = mListTargetCollider[0];
                        }
                    }
                }

                yield return mWaitForFixedUpdate;
            }
        }

        Hide(false);
    }

    private IEnumerator UpdateEffect()
    {
        transform.position = m_rootPos;
        m_endUpdate = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate && m_owner.curHp > 0.0f && !World.Instance.IsEndGame && !World.Instance.ProcessingEnd)
        {
            m_time += Time.fixedDeltaTime / duration;
            m_t = Mathf.SmoothStep(0.0f, 1.0f, m_time);
            if( m_t >= 1.0f ) {
                m_endUpdate = true;
            }
            else if( skipWhenTargetDie && TargetCollider ) {
                if( TargetCollider.Owner.curHp <= 0.0f || TargetCollider.Owner.actionSystem.IsCurrentAction( eActionCommand.Appear ) ) {
                    m_endUpdate = true;
				}
            }

            if (m_info.attach)
            {
                m_bonePos = Vector3.zero;
                if (m_info.boneName != "None")
                {
                    Transform bone = m_owner.aniEvent.GetBoneByName(m_info.boneName);
                    if (bone != null)
                        m_bonePos = bone.transform.position;
                }

                m_parentRot = Vector3.zero;
                if (m_info.followParentRot)
                {
                    m_rootPos = m_bonePos + (m_owner.transform.rotation * m_info.addedPosition);
                    m_parentRot = m_owner.transform.rotation.eulerAngles;
                }
                else
                    m_rootPos += m_info.addedPosition;

                m_rigidBody.MovePosition(m_rootPos);
                main.transform.rotation = Quaternion.Euler(m_parentRot + m_info.addedRotation);
            }

            yield return mWaitForFixedUpdate;
        }

        Hide(false, m_owner.curHp <= 0.0f);
    }

    private IEnumerator UpdateAnimationOnTarget()
    {
        m_pos = TargetCollider != null ? TargetCollider.GetCenterPos() : m_owner.transform.position + (m_owner.transform.forward * 3.0f); //TargetCollider.GetCenterPos();

        if (onGround)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(m_pos, -Vector3.up, out hitInfo, Mathf.Infinity, 1 << (int)eLayer.Floor))
                m_pos.y = hitInfo.point.y + 0.01f;
        }

        transform.position = m_pos;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_time < duration && m_owner.curHp > 0.0f)
        {
            m_time += Time.fixedDeltaTime;

            if(m_aniMain != null)
                m_aniMain[m_aniMain.clip.name].speed = m_owner.aniEvent.aniSpeed;

            yield return mWaitForFixedUpdate;
        }

        //yield return new WaitForSeconds(duration);

        Hide(false);
    }

    private void OnHit(GameObject hitObj)
    {
        if(damageType == eDamageType.Once && mListOnceTarget.Count > 0)
        {
            GameObject find = mListOnceTarget.Find(x => x == hitObj);
            if(find)
            {
                if(type == eType.TargetListHoming && TargetCollider) 
                {
                    UnitCollider unitCollider = hitObj.GetComponent<UnitCollider>();
                    if (unitCollider)
                    {
                        mListTargetCollider.Remove(unitCollider);

                        if (mListTargetCollider.Count > 0)
                        {
                            TargetCollider = mListTargetCollider[0];
                        }
                    }
                }

                return;
            }
        }

        m_hit = true;

        if (psHit != null)
        {
            psHit.gameObject.SetActive(true);
            psHit.transform.SetParent(null);
            psHit.transform.position = m_rigidBody.position;
            psHit.transform.rotation = Quaternion.Euler(m_parentRot + m_info.addedRotation);

            EffectManager.Instance.RegisterStopEff(psHit, null);
        }

        if (damageType != eDamageType.None && Utility.IsEnemyLayer((eLayer)m_owner.gameObject.layer, (eLayer)hitObj.layer))
        {
            UnitCollider unitCollider = hitObj.GetComponent<UnitCollider>();
            if(unitCollider && (unitCollider.Owner as FigureUnit) == null && !unitCollider.Owner.ignoreHit)
            {
                m_owner.OnProjectileHit(this, unitCollider, info.notAniEventAtk);
                OnHitFunc?.Invoke(unitCollider.Owner);

                if(damageType == eDamageType.Once)
                {
                    mListOnceTarget.Add(hitObj);
                }

				if( ShakeDuration > 0.0f ) {
					World.Instance.InGameCamera.PlayShake( m_owner, ShakeDuration, ShakePower, ShakeSpeed, 0.2f );
				}

				ExecuteBattleOption( BattleOption.eBOTimingType.OnProjectileHit, 0, this );
			}
        }

        if (m_onHit != null)
        {
            m_onHit();
            m_onHit = null;
        }

        if ( mOnHitForExtendChild != null ) {
            for ( int i = 0; i < mOnHitForExtendChild.Length; i++ ) {
                if ( mOnHitForExtendChild[i] != null ) {
                    mOnHitForExtendChild[i]?.Invoke( i );
                    mOnHitForExtendChild[i] = null;
                }
            }
		}

        if (fxSndHit)
            SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, fxSndHit, volumeHit * FSaveData.Instance.GetSEVolume());

        if (TargetCollider && type == eType.TargetListHoming && hitObj == TargetCollider.gameObject)
        {
            mListTargetCollider.Remove(TargetCollider);
            if (mListTargetCollider.Count > 0)
            {
                TargetCollider = mListTargetCollider[0];
                return;
            }
        }

        if (/*hitEnemy &&*/ passEnemy)
            return;

        if(m_psMain == null)
            m_endUpdate = true;
    }

	private void OnDotHit( GameObject hitObj ) {
		if ( !Utility.IsEnemyLayer( (eLayer)m_owner.gameObject.layer, (eLayer)hitObj.layer ) ) {
			return;
		}

		UnitCollider unitCollider = hitObj.GetComponent<UnitCollider>();
		if ( unitCollider && unitCollider.Owner && ( unitCollider.Owner as FigureUnit ) == null ) {
			unitCollider.Owner.StayOnProjectile = this;

			m_owner.OnProjectileHit( this, unitCollider, info.notAniEventAtk );
			OnHitFunc?.Invoke( unitCollider.Owner );
		}
	}

	private bool CheckLayer(int enemyLayer, int colliderLayer)
    {
        bool checkLayer = false;

        if (enableLayerEnemy && ((enemyLayer & (1 << colliderLayer)) != 0))
        {
            checkLayer = true;
        }

        if (m_owner.CompareTag("Player") && enableLayerEnemy && colliderLayer == (int)eLayer.EnvObject)
        {
            checkLayer = true;
        }

        if (enableLayerFloor && colliderLayer == (int)eLayer.Floor)
        {
            checkLayer = true;
        }

        if (enableLayerWall && colliderLayer == (int)eLayer.Wall)
        {
            checkLayer = true;
        }

        return checkLayer;
    }

    private void OnTriggerEnter(Collider other)
    {
		if( m_owner == null || !m_owner.IsActivate() || damageType != eDamageType.Once || gameObject.layer == other.gameObject.layer ) {
			return;
		}

		int enemyLayer = Utility.GetEnemyLayer((eLayer)m_owner.gameObject.layer);
        if (!CheckLayer(enemyLayer, other.gameObject.layer))
        {
            return;
        }

        Unit unit = other.gameObject.GetComponent<Unit>();
        if(unit && (unit.curHp <= 0.0f || unit as GimmickObject))
        {
            return;
        }

        EnvironmentOnTrigger envOnTrigger = other.gameObject.GetComponent<EnvironmentOnTrigger>();
        if(envOnTrigger)
        {
            return;
        }

        OnHit(other.gameObject);
    }

	private void OnTriggerStay( Collider other ) {
		if ( m_owner == null || other == null || m_boxCol == null ) {
			return;
		}

		if ( m_boxCol.enabled == false || damageType != eDamageType.Dot || gameObject.layer == other.gameObject.layer ) {
			return;
		}

		int enemyLayer = Utility.GetEnemyLayer( (eLayer)m_owner.gameObject.layer );
		if ( !CheckLayer( enemyLayer, other.gameObject.layer ) ) {
			return;
		}

		UnitCollider unitCollider = other.GetComponent<UnitCollider>();
		if ( unitCollider && unitCollider.Owner && ( unitCollider.Owner as FigureUnit ) == null ) {
			if ( unitCollider.Owner.ignoreHit || unitCollider.Owner.curHp <= 0.0f ) {
				return;
			}
		}

		if ( !mbFirstHitCheck || mCheckTime <= tick ) {
			GameObject find = mListDotTarget.Find( x => x == other.gameObject );
			if ( find == null ) {
				mListDotTarget.Add( other.gameObject );

				if ( !mbFirstHitCheck && mCheckTime > tick ) {
					for ( int i = 0; i < mListDotTarget.Count; i++ ) {
						OnDotHit( mListDotTarget[i] );
					}

					mCheckTime = 0.0f;
				}

				mbFirstHitCheck = true;
			}

			return;
		}

		for ( int i = 0; i < mListDotTarget.Count; i++ ) {
			OnDotHit( mListDotTarget[i] );
		}

		mCheckTime = 0.0f;
	}

	private void OnTriggerExit(Collider other)
    {
        if(mListDotTarget.Count <= 0)
        {
            return;
        }

        GameObject find = mListDotTarget.Find(x => x == other.gameObject);
        if(find)
        {
            Unit target = find.GetComponent<Unit>();
            if(target)
            {
                target.StayOnProjectile = null;
            }

            mListDotTarget.Remove(find);
        }
    }

    private void FixedUpdate()
    {
        if(!mbStartCheckTime || World.Instance.IsPause)
        {
            return;
        }

        mCheckTime += Time.fixedDeltaTime;
    }

    public void SetProjectileAtkAttr(EProjectileAtkAttr atkAttr)
    {
        ProjectileAtkAttr = atkAttr;

        if(child)
        {
            child.SetProjectileAtkAttr(atkAttr);
        }
    }

    private void StartChildProjectile()
    {
		if (startChildType == eStartChildType.ParentHit)
		{
			m_onHit = FireChild;
		}
		else if (startChildType == eStartChildType.Delay)
		{
			if (startChildDelay <= 0.0f)
			{
				FireChild();
			}
			else
			{
				StartCoroutine("DelayedFireChild");
			}
		}
    }

    private void StartExtendChildProjectile() {
        if ( mOnHitForExtendChild == null ) {
            mOnHitForExtendChild = new System.Func<int, bool>[ExtendChildArr.Length];
        }

        for ( int i = 0; i < ExtendChildArr.Length; i++ ) {
            if ( ExtendChildArr[i].StartChildType == eStartChildType.ParentHit ) {
                mOnHitForExtendChild[i] = FireExtendChild;
            }
            else if ( ExtendChildArr[i].StartChildType == eStartChildType.Delay ) {
                if ( ExtendChildArr[i].StartChildDelay <= 0.0f ) {
                    FireExtendChild( i );
				}
                else {
                    StartCoroutine( "FireExtendChild", i );
				}
			}
        }
    }

    private AniEvent.sProjectileInfo GetChildInfo()
    {
        AniEvent.sProjectileInfo childInfo = new AniEvent.sProjectileInfo();
        childInfo.projectile = child;
        childInfo.boneName = "None";
        childInfo.addedPosition = transform.position;
        childInfo.addedRotation = Vector3.zero;
        childInfo.attach = false;
        childInfo.followParentRot = false;

        if (childIgnoreY)
        {
            /*float y = 0.01f;
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, -transform.up, out hitInfo, Mathf.Infinity, (1 << (int)eLayer.Floor) | (1 << (int)eLayer.Obstacle)))
                y = hitInfo.point.y + 0.01f;*/

            childInfo.addedPosition.y = m_owner.posOnGround.y + 0.01f;//y;
        }

        return childInfo;
    }

    private IEnumerator DelayedFireChild()
    {
        yield return new WaitForSeconds(startChildDelay);
        child.Fire(m_owner, ToExecuteType, m_evt, GetChildInfo(), TargetCollider ? TargetCollider.Owner : null, OwnerActionTableId, this, OwnerAction);
    }

    private void FireChild()
    {
        child.Fire(m_owner, ToExecuteType, m_evt, GetChildInfo(), TargetCollider ? TargetCollider.Owner : null, OwnerActionTableId, this, OwnerAction);
    }

    private IEnumerator DelayedFireExtendChild() {
        yield return new WaitForSeconds( startChildDelay );
        child.Fire( m_owner, ToExecuteType, m_evt, GetChildInfo(), TargetCollider ? TargetCollider.Owner : null, OwnerActionTableId, this, OwnerAction );
    }

    private bool FireExtendChild( int index ) {
        if ( ExtendChildArr == null || index < 0 || index >= ExtendChildArr.Length ) {
            return false;
		}

        ExtendChildArr[index].Child.Fire( m_owner, ToExecuteType, m_evt, GetChildInfo(), 
                                    TargetCollider ? TargetCollider.Owner : null, OwnerActionTableId, this, OwnerAction );

        return true;
    }

    private IEnumerator DelayedFireExtendChild( int index ) {
        if ( ExtendChildArr == null || index < 0 || index >= ExtendChildArr.Length ) {
            yield break;
        }

        yield return new WaitForSeconds( ExtendChildArr[index].StartChildDelay );

        ExtendChildArr[index].Child.Fire( m_owner, ToExecuteType, m_evt, GetChildInfo(),
                                    TargetCollider ? TargetCollider.Owner : null, OwnerActionTableId, this, OwnerAction );
    }

    /*public void TestFire()
    {
        if(testParent == null)
        {
            Debug.LogError("부모를 지정해주세요.");
            return;
        }

        transform.position = testParent.transform.position;
        transform.rotation = testParent.transform.rotation;

        m_target = testTarget;
        StartProjectile();
    }*/
}
