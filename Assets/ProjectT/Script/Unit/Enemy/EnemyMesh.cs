
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyMesh : MonoBehaviour
{
    [Header("[Property]")]
    public UnitCollider[] ListCollider;

    private Unit                    mOwner              = null;
    private List<Transform>         mListRelatedBone    = new List<Transform>();
    private List<AniEvent.sEvent>   mListRelatedAniEvt  = new List<AniEvent.sEvent>();


    public void Init(Unit owner)
    {
        mOwner = owner;
    }

    public void Activate()
    {
        for(int i = 0; i < ListCollider.Length; i++)
        {
            ListCollider[i].Enable(true);
        }

        for(int i = 0; i < mListRelatedAniEvt.Count; i++)
        {
            mListRelatedAniEvt[i].SkipCheckEvent = false;
        }

        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        for (int i = 0; i < ListCollider.Length; i++)
        {
            ListCollider[i].Enable(false);
        }

        for (int i = 0; i < mListRelatedAniEvt.Count; i++)
        {
            mListRelatedAniEvt[i].SkipCheckEvent = true;
        }

        gameObject.SetActive(false);
    }

    public bool IsActivate()
    {
        return gameObject.activeSelf;
    }

	private void Start() {
		if ( mOwner == null || mOwner.aniEvent == null || ListCollider.Length <= 0 ) {
			return;
		}

		mListRelatedBone.Clear();
		mListRelatedAniEvt.Clear();

		for ( int i = 0; i < ListCollider.Length; i++ ) {
            if ( ListCollider[i] == null || ListCollider[i].transform.parent == null ) {
                continue;
			}

			Transform[] trs = ListCollider[i].transform.parent.GetComponentsInChildren<Transform>( true );
			for ( int j = 0; j < trs.Length; j++ ) {
				Transform tr = trs[j];
                if( tr == null ) {
                    continue;
				}

				if ( tr.GetComponent<UnitCollider>() || mListRelatedBone.Find( x => x.name.CompareTo( tr.name ) == 0 ) ) {
					continue;
				}

				mListRelatedBone.Add( tr );
			}
		}

		for ( int i = 0; i < mListRelatedBone.Count; i++ ) {
			List<AniEvent.sEvent> list = mOwner.aniEvent.GetAllEventByEffectsWithBoneName( mListRelatedBone[i].name );
			if ( list == null || list.Count <= 0 ) {
				continue;
			}

			mListRelatedAniEvt.AddRange( list );
		}
	}
}
