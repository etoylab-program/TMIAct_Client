
public class EndGameEvent : BaseEvent
{
    public float duration { get; private set; }


    public EndGameEvent()
    {
        eventSubject = eEventSubject.World;
    }

    public void Set(eEventType eventType, float duration)
    {
        this.eventType = eventType;
        this.duration = duration;
    }
}
