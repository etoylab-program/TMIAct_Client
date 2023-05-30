
using UnityEngine;


public class InputEvent : BaseEvent
{
    public Vector3  dir;
    public Vector3  beforeDir;
    public bool     SkipRunStop;


    public InputEvent()
    {
        eventSubject = eEventSubject.Self;

        dir = Vector3.zero;
        beforeDir = Vector3.zero;
        SkipRunStop = false;
    }
}
