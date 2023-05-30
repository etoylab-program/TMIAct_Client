
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationEvent : MonoBehaviour
{
    public Func<UnityEngine.AnimationEvent, bool> OnFunc01;
    public Func<UnityEngine.AnimationEvent, bool> OnFunc02;
    public Func<UnityEngine.AnimationEvent, bool> OnFunc03;
    public Func<UnityEngine.AnimationEvent, bool> OnFunc04;
    public Func<UnityEngine.AnimationEvent, bool> OnFunc05;


    public void Func01(UnityEngine.AnimationEvent arg)
    {
        OnFunc01?.Invoke(arg);
    }

    public void Func02(UnityEngine.AnimationEvent arg)
    {
        OnFunc02?.Invoke(arg);
    }

    public void Func03(UnityEngine.AnimationEvent arg)
    {
        OnFunc03?.Invoke(arg);
    }

    public void Func04(UnityEngine.AnimationEvent arg)
    {
        OnFunc04?.Invoke(arg);
    }

    public void Func05(UnityEngine.AnimationEvent arg)
    {
        OnFunc05?.Invoke(arg);
    }
}
