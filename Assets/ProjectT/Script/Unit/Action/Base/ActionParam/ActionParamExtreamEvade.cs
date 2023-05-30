
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionParamExtreamEvade : IActionBaseParam
{
    public eAnimation   AniEvade            { get; private set; }
    public float        AddedDuration       { get; private set; }
    public Vector3      ExtraDashDir        { get; private set; }
    public float        ExtraDashSpeedRatio { get; private set; }


    public ActionParamExtreamEvade()
    {
    }

    public ActionParamExtreamEvade(eAnimation aniEvade, float addedDuration, Vector3 extraDashDir, float extraDashSpeedRatio)
    {
        AniEvade = aniEvade;
        AddedDuration = addedDuration;
        ExtraDashDir = extraDashDir;
        ExtraDashSpeedRatio = extraDashSpeedRatio;
    }
}
