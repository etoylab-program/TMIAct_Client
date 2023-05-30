

public class ActionParamDie : IActionBaseParam
{
    public enum eState
    {
        Normal = 0,
        Down,
    }


    public eState state { get; private set; }
    public Unit attacker { get; private set; }


    public ActionParamDie()
    {
    }

    public ActionParamDie(eState state, Unit attacker)
    {
        this.state = state;
        this.attacker = attacker;
    }
}
