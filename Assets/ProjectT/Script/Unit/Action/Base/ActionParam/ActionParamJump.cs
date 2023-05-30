
using UnityEngine;


public class ActionParamJump : IActionBaseParam
{
    public float jumpPower { get; private set; }
    public Vector3 dir { get; private set; }
    public float dist { get; private set; }
    

    public ActionParamJump(float jumpPower, Vector3 dir, float dist)
    {
        this.jumpPower = jumpPower;
        this.dir = dir;
        this.dist = dist;
    }
}
