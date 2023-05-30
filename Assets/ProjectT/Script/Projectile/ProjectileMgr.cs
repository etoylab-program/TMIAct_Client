
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileMgr
{
    private List<Projectile> mListActiveProjectile = new List<Projectile>();


    public void AddActiveProjectile(Projectile projectile)
    {
        if (!projectile.IsActivate())
        {
            return;
        }

        mListActiveProjectile.Add(projectile);
    }

    public void RemoveProjectile(Projectile projectile)
    {
        Projectile find = mListActiveProjectile.Find(x => x == projectile);
        if (find == null)
        {
            return;
        }

        mListActiveProjectile.Remove(projectile);
    }

    public List<Projectile> GetProjectile(Vector3 comparePos, float distance)
    {
        List<Projectile> list = new List<Projectile>();
        for(int i = 0; i < mListActiveProjectile.Count; i++)
        {
            if (Vector3.Distance(mListActiveProjectile[i].transform.position, comparePos) <= distance)
                list.Add(mListActiveProjectile[i]);
        }

        return list;
    }

    public Projectile FindProjectileOrNull(Projectile pjt)
    {
        Projectile find = mListActiveProjectile.Find(x => x == pjt);
        return find;
    }

    public void DestroyAllProjectile(bool checkDontDestroyOnPlayDirector)
    {
        for (int i = 0; i < mListActiveProjectile.Count; i++)
        {
            if(checkDontDestroyOnPlayDirector && mListActiveProjectile[i].DontDestroyOnPlayDirector)
            {
                continue;
            }

            mListActiveProjectile[i].End();
            RemoveProjectile(mListActiveProjectile[i]);

            --i;
        }
    }

    private void FixedUpdate() {
        for( int i = 0; i < mListActiveProjectile.Count; i++ ) {
            if( mListActiveProjectile[i].owner.curHp <= 0.0f ) {
                RemoveProjectile( mListActiveProjectile[i] );
                --i;
            }
		}
    }
}
