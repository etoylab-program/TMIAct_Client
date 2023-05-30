
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AcrionMomochiCreatureBrokenArm : ActionEnemyBase
{
    [Header("[Property]")]
    public UnitCollider[] ListLeftArmHitBox;
    public UnitCollider[] ListRightArmHitBox;

    private EnemyMesh   mLeftArm        = null;
    private EnemyMesh   mRightArm       = null;
    private bool        mSkip           = false;

    private Director    mDirectorDestroyLeftArmWithRightArm     = null;
    private Director    mDirectorDestroyRightArmWithLeftArm     = null;
    private Director    mDirectorDestroyLeftArmWithoutRightArm  = null;
    private Director    mDirectorDestroyRightArmWithoutLeftArm  = null;
    private Director    mCurrentDirector                        = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.BrokenArm;

        mOwnerEnemy = m_owner as Enemy;

        mDirectorDestroyLeftArmWithRightArm = GameSupport.CreateDirector("drt_momochi_creature_armdestroy_left-right");
        mDirectorDestroyRightArmWithLeftArm = GameSupport.CreateDirector("drt_momochi_creature_armdestroy_right-left");
        mDirectorDestroyLeftArmWithoutRightArm = GameSupport.CreateDirector("drt_momochi_creature_armdestroy_left-none");
        mDirectorDestroyRightArmWithoutLeftArm = GameSupport.CreateDirector("drt_momochi_creature_armdestroy_right-none");
    }

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
			if( !World.Instance.ListPlayer[i].IsActivate() ) {
				continue;
			}

			World.Instance.ListPlayer[i].actionSystem.CancelCurrentAction();

			if( !World.Instance.ListPlayer[i].isGrounded ) {
				World.Instance.ListPlayer[i].transform.position = World.Instance.ListPlayer[i].posOnGround;
			}
		}

		mLeftArm = mOwnerEnemy.GetEnemyMeshByName( "mon_L_arm" );
		if( mLeftArm == null ) {
			Debug.LogError( "모모치 크리쳐에 mon_L_arm 메쉬를 찾을 수 없습니다." );
		}

		mRightArm = mOwnerEnemy.GetEnemyMeshByName( "mon_R_arm" );
		if( mRightArm == null ) {
			Debug.LogError( "모모치 크리쳐에 mon_R_arm 메쉬를 찾을 수 없습니다." );
		}

		if( !mLeftArm.IsActivate() && !mRightArm.IsActivate() ) {
			mSkip = true;
		}
		else {
			mSkip = false;

			if( mLeftArm.IsActivate() && mRightArm.IsActivate() ) {
				int leftArmHitCount = 0;
				for( int i = 0; i < ListLeftArmHitBox.Length; i++ ) {
					leftArmHitCount += ListLeftArmHitBox[i].HitCount;
				}

				int rightArmHitCount = 0;
				for( int i = 0; i < ListRightArmHitBox.Length; i++ ) {
					rightArmHitCount += ListRightArmHitBox[i].HitCount;
				}

				if( leftArmHitCount >= rightArmHitCount ) {
					mDirectorDestroyLeftArmWithRightArm.SetCallbackOnEnd2( DestroyLeftArm );
					mCurrentDirector = mDirectorDestroyLeftArmWithRightArm;
				}
				else {
					mDirectorDestroyRightArmWithLeftArm.SetCallbackOnEnd2( DestroyRightArm );
					mCurrentDirector = mDirectorDestroyRightArmWithLeftArm;
				}
			}
			else if( !mLeftArm.IsActivate() ) {
				mDirectorDestroyRightArmWithoutLeftArm.SetCallbackOnEnd2( DestroyRightArm );
				mCurrentDirector = mDirectorDestroyRightArmWithoutLeftArm;
			}
			else {
				mDirectorDestroyLeftArmWithoutRightArm.SetCallbackOnEnd2( DestroyLeftArm );
				mCurrentDirector = mDirectorDestroyLeftArmWithoutRightArm;
			}

			Utility.SetLayer( mOwnerEnemy.gameObject, (int)eLayer.Default, true );

			mCurrentDirector.Init( mOwnerEnemy );
			mCurrentDirector.Play();
		}
	}

	public override IEnumerator UpdateAction()
    {
        if (!mSkip)
        {
            while (mCurrentDirector.playableDirector.state == UnityEngine.Playables.PlayState.Playing)
            {
                yield return null;
            }

            Utility.SetLayer(mOwnerEnemy.gameObject, (int)eLayer.Enemy, true);
        }
    }

    public void DestroyLeftArm()
    {
        mLeftArm.Deactivate();
    }

    public void DestroyRightArm()
    {
        mRightArm.Deactivate();
    }
}
