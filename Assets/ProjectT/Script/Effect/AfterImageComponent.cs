
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AfterImageComponent : MonoBehaviour {
	public class AfterImage {
		public SkinnedMeshRenderer	renderer;
		public Mesh					mesh;
		public int					subMeshCount;
		public List<Material>		listMtrl = new List<Material>();
		public float				showStartTime;
		public float				duration;
		public float				alpha;
		public bool					needRemove = false;
		public Quaternion			quat;
		public Vector3				pos;
		public Vector3				scale;
		public Matrix4x4			mat;
		public bool					baked = false;
	}


	private float				_duration;
	private float				_interval;
	private float				_fadeTime;
	private Color				m_color				= new Color( 0.345f, 0.164f, 0.313f );
	private List<AfterImage>	_imageList			= new List<AfterImage>();
	private Unit				m_parent;
	private WaitForFixedUpdate	mWaitForFixedUpdate	= new WaitForFixedUpdate();


	public void SetParent( Unit unit ) {
		m_parent = unit;
	}

	public void Play( SkinnedMeshRenderer[] renderers, float duration, float interval, float fadeout, Color color ) {
		_duration = duration;
		_interval = interval;
		_fadeTime = fadeout;

		m_color = color;

		StartCoroutine( DoAddImage( renderers ) );
	}

	public void Stop() {
		StopAllCoroutines();

		_imageList.RemoveAll( x => true );
	}

	private IEnumerator DoAddImage( SkinnedMeshRenderer[] renderers ) {
		float startTime = Time.realtimeSinceStartup;

		WaitForSeconds waitForSeconds = new WaitForSeconds( _interval );
		while ( true ) {
			CreateImage( renderers );

			if ( ( Time.realtimeSinceStartup - startTime ) * Time.timeScale > _duration ) {
				break;
			}

			if ( _interval > _duration )
				yield break;

			yield return waitForSeconds;
		}
	}

	private void CreateImage( SkinnedMeshRenderer[] renderers ) {
		Transform t = transform;
		List<Material> listMtrl = new List<Material>();
		for ( int i = 0; i < renderers.Length; ++i ) {
			var item = renderers[i];
			if ( !item.gameObject.activeInHierarchy ) {
				continue;
			}

			var tK = item.transform;

			listMtrl.Clear();
			for ( int j = 0; j < renderers[i].materials.Length; j++ ) {
				listMtrl.Add( new Material( renderers[i].materials[j] ) );
			}

			var mesh = new Mesh();
			item.BakeMesh( mesh );

			int subMeshCnt = mesh.subMeshCount;

			mesh.RecalculateBounds();
			mesh.RecalculateNormals();

			AfterImage afterImg = new AfterImage {
				renderer = item,
				mesh = mesh,
				subMeshCount = subMeshCnt,
				alpha = 1.0f,
				showStartTime = Time.realtimeSinceStartup,
				duration = _duration,
				quat = tK.transform.rotation,
				pos = tK.transform.position,
				scale = tK.transform.lossyScale,
				baked = false,
			};

			afterImg.listMtrl.AddRange( listMtrl );
			_imageList.Add( afterImg );

			for ( int j = 0; j < _imageList.Count; j++ )
				_imageList[j].mat.SetTRS( tK.transform.position, tK.transform.rotation, tK.transform.localScale );

			StartCoroutine( FadeOut( afterImg ) );
		}
	}

	private void Update() {
		foreach ( var item in _imageList ) {
			for ( int i = 0; i < item.subMeshCount; i++ ) {
				item.mat.SetTRS( item.pos, item.quat, item.scale );

				if ( !item.baked ) {
					item.renderer.BakeMesh( item.mesh );
					item.baked = true;
				}

				Graphics.DrawMesh( item.mesh, item.mat, item.listMtrl[i], (int)eLayer.Default, Camera.main, i );
			}
		}
	}

	private IEnumerator FadeOut( AfterImage afterImg ) {
		if ( _fadeTime > 0.0f ) {
			afterImg.alpha = 0.75f;

			while ( afterImg.alpha > 0.0f ) {
				afterImg.alpha -= Time.fixedDeltaTime / _fadeTime;

				for ( int i = 0; i < afterImg.listMtrl.Count; i++ ) {
					if ( !afterImg.listMtrl[i].HasProperty( "_Color" ) && !afterImg.listMtrl[i].HasProperty( "_BaseColor" ) ) {
						afterImg.needRemove = true;
						_imageList.Remove( afterImg );

						continue;
					}

					bool isNewShader = false;
					if ( afterImg.listMtrl[i].HasProperty( "_BaseColor" ) ) {
						isNewShader = true;
					}

					Color color = Color.white;
					if ( !isNewShader ) {
						color = afterImg.listMtrl[i].GetColor( "_Color" );
					}
					else {
						color = afterImg.listMtrl[i].GetColor( "_BaseColor" );
					}

					color.a = afterImg.alpha;

					if ( !isNewShader ) {
						afterImg.listMtrl[i].SetColor( "_Color", color );

						if ( afterImg.listMtrl[i].HasProperty( "_OutlineColor" ) ) {
							Color outlineColor = afterImg.listMtrl[i].GetColor( "_OutlineColor" );
							outlineColor.a = afterImg.alpha;
							afterImg.listMtrl[i].SetColor( "_OutlineColor", outlineColor );
						}
					}
					else {
						afterImg.listMtrl[i].SetColor( "_BaseColor", color );

						if ( afterImg.listMtrl[i].HasProperty( "_Outline_Color" ) ) {
							Color outlineColor = afterImg.listMtrl[i].GetColor( "_Outline_Color" );
							outlineColor.a = afterImg.alpha;
							afterImg.listMtrl[i].SetColor( "_Outline_Color", outlineColor );
						}
					}
				}

				yield return mWaitForFixedUpdate;
			}
		}

		afterImg.needRemove = true;
		_imageList.Remove( afterImg );
	}
}
