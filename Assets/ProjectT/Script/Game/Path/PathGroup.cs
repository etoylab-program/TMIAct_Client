
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathGroup : MonoBehaviour
{
    public PathMgr              Mgr         = null;
    public BattleArea           Area        = null;
    public int                  Index       = -1;
    public List<PathPoint>      ListPoint   = new List<PathPoint>();

    private List<Vector3> mListResult = new List<Vector3>();


    public void EDInit(PathMgr mgr, int index)
    {
        Mgr = mgr;
        Index = index;

        EDSetIndex(index);

        transform.SetParent(mgr.transform);
        Utility.InitTransform(gameObject);
    }

    public void EDInit(BattleArea area, int index)
    {
        Area = area;
        Index = index;

        EDSetIndex(index);

        transform.SetParent(area.transform);
        Utility.InitTransform(gameObject);
    }

    public void EDSetIndex(int index)
    {
        Index = index;
        name = index.ToString();
    }

    public PathPoint EDCreatePoint()
    {
        GameObject gObj = new GameObject(ListPoint.Count.ToString());

        PathPoint point = gObj.AddComponent<PathPoint>();
        point.EDInit(this);

        if(ListPoint.Count > 0)
        {
            point.transform.localPosition = ListPoint[ListPoint.Count - 1].transform.localPosition;
        }

        ListPoint.Add(point);
        return point;
    }

    public void EDDeletePoint(PathPoint point)
    {
        PathPoint find = ListPoint.Find(x => x == point);
        if(find == null)
        {
            return;
        }

        DestroyImmediate(find.gameObject);
        ListPoint.Remove(find);
    }

    public int GetNearestPointIndex(Vector3 pos)
    {
        int index = -1;

        float compare = 99999.0f;
        for (int i = 0; i < ListPoint.Count; i++)
        {
            float dist = Vector3.Distance(pos, ListPoint[i].transform.position);
            if (dist < compare)
            {
                compare = dist;
                index = i;
            }
        }

        return index;
    }

    public List<Vector3> GetPointList(int startIndex)
    {
        mListResult.Clear();
        for (int i = startIndex; i < ListPoint.Count; i++)
        {
            mListResult.Add(ListPoint[i].transform.position);
        }

        return mListResult;
    }
}
