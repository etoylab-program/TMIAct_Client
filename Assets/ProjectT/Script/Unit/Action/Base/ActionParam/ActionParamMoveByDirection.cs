
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionParamMoveByDirection : IActionBaseParam
{
    public bool     SkipRunStop { get; private set; }   = false;
    public Vector3  Dir         { get; private set; }   = Vector3.zero;
    
    public ActionParamMoveByDirection(Vector3 dir, bool skipRunStop)
    {
        Dir = dir;
        SkipRunStop = skipRunStop;
    }
}
