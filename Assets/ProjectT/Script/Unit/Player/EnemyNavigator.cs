
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyNavigator : MonoBehaviour {
	public float	CheckStartAtkFrame	{ get; private set; } = 15.0f;
	public Unit		Target				{ get; private set; } = null;

	private Vector3             mRelativePos;
	private Color               mDefaultColor       = Color.white;
	private bool                mChangingColor      = false;
	private List<MeshRenderer>  mListRenderer       = new List<MeshRenderer>();
	private List<Material>      mListMtrl           = new List<Material>();
	private bool                mbShow              = false;
	private WaitForFixedUpdate  mWaitForFixedUpdate = new WaitForFixedUpdate();


	public void Init( Color defaultColor, float y = 1.0f ) {
		mRelativePos = new Vector3( 0.0f, y, 0.5f );
		Target = null;
		mChangingColor = false;
		mDefaultColor = defaultColor;

		mbShow = true;
		Show( false );
	}

	public void Show( bool show ) {
		if( mbShow == show ) {
			return;
		}

		mbShow = show;
		if( !show ) {
			StopCoroutine( "ChangingColor" );
			mChangingColor = false;
		}

		for( int i = 0; i < mListRenderer.Count; i++ ) {
			mListRenderer[i].enabled = show;
		}
	}

	public bool HasTarget() {
		if( Target ) {
			return true;
		}

		return false;
	}

	public void SetTarget( Unit target ) {
		if( Target == target ) {
			return;
		}

		Target = target;
		if( target ) {
			Show( true );
		}
		else {
			Show( false );
		}

		for( int i = 0; i < mListMtrl.Count; i++ ) {
			mListMtrl[i].SetColor( "_Color", mDefaultColor );
		}

		mChangingColor = false;
	}

	private void Awake() {
		mListRenderer.AddRange( GetComponentsInChildren<MeshRenderer>() );

		for( int i = 0; i < mListRenderer.Count; i++ ) {
			mListMtrl.Add( mListRenderer[i].material );
		}
	}

	private void FixedUpdate() {
		if( !World.Instance.Player.IsActivate() || World.Instance.Player.curHp <= 0.0f ) {
			Show( false );
			return;
		}

		if( Target == null ) {
			return;
		}

		if( Target.curHp <= 0.0f ) {
			SetTarget( null );
			return;
		}

		if( Target.isVisible || !Target.IsActivate() ) {
			Show( false );
			return;
		}

		Show( true );

		bool isCurAttackAni = Target.aniEvent ? Target.aniEvent.IsCurAnyAttackAni() : false;
		if( !mChangingColor && isCurAttackAni ) {
			float atkStartLength = Target.aniEvent.GetFirstAttackEventLength(Target.aniEvent.curAniType);
			mChangingColor = true;

			StopCoroutine( "ChangingColor" );
			StartCoroutine( "ChangingColor", atkStartLength );
		}
		else if( !isCurAttackAni ) {
			mChangingColor = false;

			for( int i = 0; i < mListMtrl.Count; i++ ) {
				mListMtrl[i].SetColor( "_Color", mDefaultColor );
			}
		}

		Vector3 v1 = Target.transform.position;
		Vector3 v2 = World.Instance.Player.transform.position;
		v1.y = v2.y = 0.0f;

		Quaternion q = Quaternion.LookRotation(Vector3.Normalize(v1 - v2));
		transform.position = World.Instance.Player.transform.position + ( q * mRelativePos );

		v1 = Target.transform.position;
		v2 = World.Instance.Player.transform.position;

		q = Quaternion.LookRotation( Vector3.Normalize( v1 - v2 ) );
		transform.rotation = q;
	}

	private IEnumerator ChangingColor( float atkStartLength ) {
		float time = 0.0f;
		Color color = mDefaultColor;

		for( int i = 0; i < mListMtrl.Count; i++ ) {
			mListMtrl[i].SetColor( "_Color", color );
		}

		while( time / atkStartLength <= 1.0f ) {
			time += Time.fixedDeltaTime;

			color.g = color.b = 1.0f - ( time / atkStartLength );

			for( int i = 0; i < mListMtrl.Count; i++ ) {
				mListMtrl[i].SetColor( "_Color", color );
			}

			yield return mWaitForFixedUpdate;
		}
	}
}
