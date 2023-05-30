using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameFailPanel : FComponent
{
    public override void OnEnable()
    {
        base.OnEnable();
        PlayAnimtion(0);
    }
}