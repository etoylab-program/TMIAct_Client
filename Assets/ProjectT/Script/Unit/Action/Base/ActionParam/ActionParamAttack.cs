

public class ActionParamAttack : IActionBaseParam
{
    public int      index           { get; private set; }
    public Unit     target          { get; private set; }
    public float    atkRatio        { get; private set; }
    public bool     HasExtraAttack  { get; private set; }


    public ActionParamAttack()
    {
    }

    public ActionParamAttack(int index, Unit target, float atkRatio, bool extraAtk = false)
    {
        this.index = index;
        this.target = target;
        this.atkRatio = atkRatio;
        HasExtraAttack = extraAtk;
    }
}
