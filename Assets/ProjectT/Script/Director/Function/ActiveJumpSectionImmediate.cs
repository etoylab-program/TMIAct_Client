
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveJumpSectionImmediate : MonoBehaviour
{
    public float Time = 0.0f;

    private Director mDirector = null;


    private void Awake()
    {
        mDirector = GetComponentInParent<Director>();
    }

    private void OnEnable()
    {
        mDirector.SetTime(Time);
    }
}
