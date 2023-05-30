
public class ActionParamForPlayerSkill : IActionBaseParam
{
    public enum eState
    {
        Down = 0,
        Stun,
    }


    public Unit attacker { get; private set; }
    public eState state { get; private set; }


    public ActionParamForPlayerSkill(Unit attacker, eState state)
    {
        this.attacker = attacker;
        this.state = state;
    }
}
