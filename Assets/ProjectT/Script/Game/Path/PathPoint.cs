
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathPoint : MonoBehaviour
{
    public PathGroup Group = null;


    public void EDInit(PathGroup group)
    {
        Group = group;

        transform.SetParent(group.transform);
        Utility.InitTransform(gameObject);
    }

    public void EDDelete()
    {
        Group.EDDeletePoint(this);
    }
}
