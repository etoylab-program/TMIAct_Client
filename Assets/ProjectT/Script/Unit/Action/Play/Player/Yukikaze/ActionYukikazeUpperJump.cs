
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionYukikazeUpperJump : ActionUpperJump
{
    protected override eAnimation GetStartAni()
    {
        return eAnimation.Jump01;
    }
}
