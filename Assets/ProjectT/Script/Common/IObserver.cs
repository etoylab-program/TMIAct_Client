

using System.Collections.Generic;


public enum eObserverMsg
{
    None = 0,

    // Game Msg
    PLAYER_DEAD,

    ENEMY_DEAD,
    ALL_ENEMY_DEAD,

    OTHER_OBJECT_DEAD,
    PASS_PORTAL,

    END_TIMER,

    // QTE Condition Msg
    SUCCESS_EVADE,

    HIT_DOWN_ATTACK,
    HIT_KNOCKBACK_ATTACK,
    HIT_UPPER_ATTACK,

    END_QTE_ACTION,
}


public interface IObserver
{
    void OnNotify(eObserverMsg observerMsg); 
}

public interface IObserverSubject
{
    List<IObserver> m_listObserver { get; set; }

    void Notify(eObserverMsg observerMsg);
    void AddObserver(IObserver observer);
    void RemoveObserver(IObserver observer);
}
