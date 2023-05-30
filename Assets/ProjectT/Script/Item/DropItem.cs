
using System.Collections;
using UnityEngine;


public class DropItem : MonoBehaviour
{
    public enum eType
    {
        None = 0,
        Coin = 1,
        HP,
        SP,
        SuperArmor,
    }


	[Header("[Property]")]
    public eType		type;
    public AudioClip	fxSndGet;

	public GameClientTable.DropItem.Param	data		{ get; private set; }
	public BOItem							boItem		{ get; private set; }
	public bool								IsActive	{ get { return gameObject.activeSelf; } }

	private Unit				mOwner				= null;
	private Rigidbody			mRigidBody;
    private BoxCollider			mBoxCol;
    private ParticleSystem		mEffGet;
    private Vector3				mForce				= Vector3.zero;
	private bool				mReForce			= false;
	private float				mReduce				= 0.0f;
    private Collider[]			mCheckCollisions	= null;
    private bool				mPrepare			= false;
    private bool                mMoveToPlayer       = false;
	private WaitForFixedUpdate	mWaitForFixedUpdate = new WaitForFixedUpdate();
    private Transform			mChildTransform     = null;


	private void Awake()
    {
        mRigidBody = GetComponent<Rigidbody>();
        mBoxCol = GetComponent<BoxCollider>();
        mChildTransform = transform.GetChild(0);
    }

    public void Init(GameClientTable.DropItem.Param data)
    {
        this.data = data;
        this.type = (eType)data.Type;

        boItem = new BOItem(data.ItemAddBOSetID1, null);

        mEffGet = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>("effect", "Effect/Character/prf_fx_item_get.prefab");
        mEffGet.gameObject.SetActive(false);

        Utility.SetLayer(gameObject, (int)eLayer.DropItem, true);
    }

    public void Drop(Unit owner, Vector3 pos)
    {
		mOwner = owner;
        gameObject.SetActive(true);

        mRigidBody.useGravity = true;
        mPrepare = false;
        mReForce = false;
        mMoveToPlayer = false;

        transform.position = pos;

        mForce.x = (float)UnityEngine.Random.Range(-4, 5);
        mForce.y = (float)UnityEngine.Random.Range(5, 9);
        mForce.z = (float)UnityEngine.Random.Range(-4, 5);
        mReduce = 1.0f;

        if (World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.Z)
        {
            mForce.z = 0.0f;
        }
        else if (World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.X)
        {
            mForce.x = 0.0f;
        }

        mRigidBody.AddForce(mForce, ForceMode.Impulse);

        World.Instance.EnemyMgr.AddDropItem(this);

        Utility.IgnorePhysics(eLayer.DropItem, eLayer.Player);
		Utility.IgnorePhysics(eLayer.DropItem, eLayer.Enemy);
		Utility.SetPhysicsLayerCollision(eLayer.DropItem, eLayer.Floor);
        Utility.SetPhysicsLayerCollision(eLayer.DropItem, eLayer.Wall);
    }

    public void Get(Player player)
    {
		boItem.ChangeSender(player);
        boItem.Execute(BattleOption.eBOTimingType.GetItem);

        SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, fxSndGet, 0.5f);

        Vector3 pos = transform.position;
        pos.y += mBoxCol.size.y * 0.5f;
        mEffGet.transform.position = pos;
        mEffGet.gameObject.SetActive(true);
        EffectManager.Instance.RegisterStopEff(mEffGet, null);

        gameObject.SetActive(false);
    }

    public void MoveToPlayer()
    {
        if (!gameObject.activeSelf)
            return;

        Utility.SetPhysicsLayerCollision(eLayer.DropItem, eLayer.Player);
		if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
		{
			Utility.SetPhysicsLayerCollision(eLayer.DropItem, eLayer.Enemy);
		}

		Utility.IgnorePhysics(eLayer.DropItem, eLayer.Floor);
        Utility.IgnorePhysics(eLayer.DropItem, eLayer.Wall);

        mRigidBody.useGravity = false;
        mMoveToPlayer = true;
        mPrepare = true;

        StartCoroutine("UpdateMove");
    }

    private IEnumerator UpdateMove()
    {
        float time = 0.0f;
        float t = 0.0f;
        Vector3 v1 = ((transform.position - World.Instance.Player.transform.position) + transform.up).normalized * 0.1f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (gameObject.activeSelf)
        {
            time += Time.fixedDeltaTime / 0.5f;
            t = Mathf.SmoothStep(0.0f, 1.0f, time);

            Vector3 v2 = (World.Instance.Player.GetCenterPos() - transform.position).normalized;
            Vector3 v = ((v1 * (1.0f - t)) + (v2 * t)).normalized;

            mRigidBody.MovePosition(mRigidBody.position + (v * 32.0f * Time.fixedDeltaTime));
            yield return mWaitForFixedUpdate;
        }
    }

	private void FixedUpdate() {
		if( mRigidBody.useGravity ) {
			if( mRigidBody.velocity == Vector3.zero ) {
				mPrepare = true;
				Utility.SetPhysicsLayerCollision( eLayer.DropItem, eLayer.Player );
				
                if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
					Utility.SetPhysicsLayerCollision( eLayer.DropItem, eLayer.Enemy );
				}
			}
		}

		if( mPrepare ) {
			int layerMask = 1 << (int)eLayer.Player;
			if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
				layerMask |= 1 << (int)eLayer.Enemy;
			}

            if( mMoveToPlayer ) {
                float dist = Vector3.Distance( transform.position, World.Instance.Player.transform.position );
                if( dist <= mBoxCol.size.sqrMagnitude * 1.5f ) {
                    World.Instance.Player.GetItem( this );
                    Get( World.Instance.Player );
                }
            }
            else {
                mCheckCollisions = Physics.OverlapBox( transform.position, mBoxCol.size, transform.rotation, layerMask );
                if( mCheckCollisions.Length > 0 ) {
                    for( int i = 0; i < mCheckCollisions.Length; i++ ) {
                        if( !mCheckCollisions[i].CompareTag( "Player" ) && World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
                            continue;
                        }

                        Player player = mCheckCollisions[i].gameObject.GetComponent<Player>();
                        if( mMoveToPlayer && player.IsHelper ) {
                            continue;
                        }

                        if( mOwner == null || mOwner && mOwner == player ) {
                            player.GetItem( this );
                            Get( player );

                            break;
                        }
                    }
                }
            }
		}

		if( mChildTransform != null && type == eType.Coin ) {
			mChildTransform.Rotate( Vector3.up, 70.0f * Time.fixedDeltaTime );
		}
	}

	private void OnCollisionEnter(Collision collision)
    {
        /*
        if(mPrepare)
        {
            mReduce = 0.0f;
            mRigidBody.velocity = Vector3.zero;

            return;
        }
        */

        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Wall"))
        {
            mReduce = Mathf.Clamp(mReduce -= 0.3f, 0.0f, 1.0f);
            if (mReduce > 0.0f)
            {
                mRigidBody.AddForce(((mForce * 0.4f) + collision.contacts[0].normal) * mReduce, ForceMode.Impulse);
            }
            else
            {
                mRigidBody.velocity = Vector3.zero;
            }
        }
        else if(!mReForce && collision.gameObject.layer == (int)eLayer.Wall && IsRootParentEnemy(collision.gameObject))
        {
            Utility.IgnorePhysics(eLayer.DropItem, eLayer.Wall);
            mReForce = true;

            mRigidBody.useGravity = true;
            mPrepare = false;

            mForce.x = (float)UnityEngine.Random.Range(-4, 5);
            mForce.y = (float)UnityEngine.Random.Range(5, 9);
            mForce.z = (float)UnityEngine.Random.Range(-4, 5);
            mReduce = 1.0f;

            mRigidBody.AddForce(mForce, ForceMode.Impulse);

            Utility.IgnorePhysics(eLayer.DropItem, eLayer.Player);
			Utility.IgnorePhysics(eLayer.DropItem, eLayer.Enemy);
            Utility.SetPhysicsLayerCollision(eLayer.DropItem, eLayer.Floor);
            Utility.SetPhysicsLayerCollision(eLayer.DropItem, eLayer.Wall);
        }
    }

    private bool IsRootParentEnemy(GameObject gObj)
    {
        Transform find = gObj.transform;
        while(true)
        {
            if(find.parent == null)
            {
                break;
            }

            find = find.parent;
        }

        if(find.gameObject.layer == (int)eLayer.Enemy)
        {
            return true;
        }

        return false;
    }
}
