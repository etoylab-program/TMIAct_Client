using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionList : MonoBehaviour
{
    public int kIndex;
    public float kSize = 0.8f;
    public List<GameObject> kList;
    
    public GameObject GetGameObject( int index )
    {
        return kList[index];
    }
    public PositionList GetPosition(int index)
    {
        return kList[index].GetComponent<PositionList>();
    }
}
