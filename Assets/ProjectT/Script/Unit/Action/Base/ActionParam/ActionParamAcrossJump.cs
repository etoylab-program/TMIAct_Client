
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionParamAcrossJump : IActionBaseParam
{
    public PortalEntry  PortalEntry { get; private set; }
    public Vector3      EndPos      { get; private set; }
    public float        Duration    { get; private set; }


    public ActionParamAcrossJump(PortalEntry portalEntry, Vector3 endPos, float duration)
    {
        PortalEntry = portalEntry;
        EndPos = endPos;
        Duration = duration;
    }
}
