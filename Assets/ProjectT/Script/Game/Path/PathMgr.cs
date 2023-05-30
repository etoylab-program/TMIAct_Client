
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathMgr : MonoSingleton<PathMgr>
{
    public List<PathGroup> ListPathGroup = new List<PathGroup>();


    public void GetNearestPathGroupIndexAndStartPointIndex(Vector3 pos, ref int groupIndex, ref int startPointIndex)
    {
        groupIndex = -1;
        startPointIndex = -1;

        float compare = 99999.0f;
        for(int i = 0; i < ListPathGroup.Count; i++)
        {
            int pointIndex = ListPathGroup[i].GetNearestPointIndex(pos);
            if(pointIndex == -1)
            {
                continue;
            }

            float dist = Vector3.Distance(pos, ListPathGroup[i].ListPoint[pointIndex].transform.position);
            if(dist < compare)
            {
                compare = dist;

                groupIndex = i;
                startPointIndex = pointIndex;
            }
        }
    }

    public List<Vector3> GetPathPointListOrNull(Vector3 pos)
    {
        int groupIndex = -1;
        int startPointIndex = -1;

        GetNearestPathGroupIndexAndStartPointIndex(pos, ref groupIndex, ref startPointIndex);
        if(groupIndex == -1 || startPointIndex == -1)
        {
            return null;
        }

        return ListPathGroup[groupIndex].GetPointList(startPointIndex);
    }

    public void EDAddPath(PathGroup group)
    {
        int index = ListPathGroup.Count;
        group.EDInit(this, index);

        ListPathGroup.Add(group);
    }

    public void EDDeletePath(int index)
    {
        PathGroup find = ListPathGroup.Find(x => x.Index == index);
        if(find == null)
        {
            return;
        }

        DestroyImmediate(find.gameObject);
        ListPathGroup.Remove(find);

        for(int i = index; i < ListPathGroup.Count; i++)
        {
            ListPathGroup[i].EDSetIndex(i);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Color originalColor = Gizmos.color;

        for (int i = 0; i < ListPathGroup.Count; i++)
        {
            if(ListPathGroup[i].ListPoint.Count <= 0)
            {
                continue;
            }

            for (int j = 0; j < ListPathGroup[i].ListPoint.Count; j++)
            {
                Gizmos.color = Color.yellow;

                Gizmos.DrawSphere(ListPathGroup[i].ListPoint[j].transform.position, 0.2f);

                if (j < ListPathGroup[i].ListPoint.Count - 1)
                {
                    Gizmos.DrawSphere(ListPathGroup[i].ListPoint[j + 1].transform.position, 0.2f);

                    // 선명하게 라인 보이도록 5번 그려줌
                    for (int k = 0; k < 5; k++)
                    {
                        Gizmos.DrawLine(ListPathGroup[i].ListPoint[j].transform.position, ListPathGroup[i].ListPoint[j + 1].transform.position);
                    }
                }
            }
        }

        Gizmos.color = originalColor;
    }
#endif
}
