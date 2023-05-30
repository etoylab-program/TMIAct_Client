
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleAreaNavigator : MonoBehaviour
{
    public Vector3 Dest { get; private set; } = Vector3.zero;

    private Player              mOwner;
    private Vector3             mRelativePos;
    private float               mAngle          = 0.0f;
    private List<MeshRenderer>  mListRenderer   = new List<MeshRenderer>();
    private List<Material>      mListMtrl       = new List<Material>();


    private void Awake()
    {
        mListRenderer.AddRange(GetComponentsInChildren<MeshRenderer>());
        for (int i = 0; i < mListRenderer.Count; i++)
        {
            mListMtrl.Add(mListRenderer[i].material);
        }
    }

    public void Init(Player owner)
    {
        mOwner = owner;

        mRelativePos = new Vector3(0.0f, mOwner.MainCollider.height, 0.0f);
        Dest = Vector3.zero;

        Show(false);
    }

    public bool IsShow()
    {
        return mListRenderer[0].enabled;
    }

    public void Show(bool show)
    {
        for (int i = 0; i < mListRenderer.Count; i++)
        {
            mListRenderer[i].enabled = show;
        }
    }

    public void SetNextBattleArea(Vector3 nextBattleAreaPos)
    {
        Dest = nextBattleAreaPos;

        for (int i = 0; i < mListRenderer.Count; i++)
        {
            mListRenderer[i].enabled = true;
        }

        for (int i = 0; i < mListMtrl.Count; i++)
        {
            mListMtrl[i].SetColor("_Color", new Color(0.16f, 0.47f, 1.0f));
        }
    }

    private void FixedUpdate()
    {
        if (!mOwner.IsActivate() || mOwner.curHp <= 0.0f)
        {
            Show(false);
            return;
        }

        Vector3 v1 = Dest;
        Vector3 v2 = mOwner.transform.position;
        v1.y = v2.y = 0.0f;

        Vector3 v = Vector3.Normalize(v1 - v2);
        if (v == Vector3.zero)
        {
            return;
        }

        Quaternion q = Quaternion.LookRotation(v);
        transform.position = mOwner.transform.position + (q * mRelativePos);

        Quaternion r = Quaternion.Euler(0.0f, 0.0f, mAngle);
        transform.rotation = q * r;

        mAngle += 80.0f * Time.fixedDeltaTime;
        if(mAngle >= 360.0f)
        {
            mAngle = 0.0f;
        }
    }
}
