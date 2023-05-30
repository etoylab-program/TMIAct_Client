

public class ActionParamHit : IActionBaseParam
{
    public Unit             attacker                    { get; private set; }
    public eBehaviour       attackerBehaviour           { get; private set; }
    public float            attackerJumpPower           { get; private set; }
    public eHitDirection    hitDir                      { get; private set; }
    public int              hitEffId                    { get; private set; }
    public bool             critical                    { get; private set; }
    public float            attackerAniCutFrameLength   { get; private set; }
    public eHitState        hitState                    { get; private set; }
    public EAttackAttr      AtkAttr                     { get; private set; }
    public bool             skip                        { get; set; }


    public ActionParamHit()
    {
    }

    public ActionParamHit(Unit attacker, eBehaviour attackerBehaviour, float attackerJumpPower, eHitDirection hitDir, int hitEffId, bool critical,
                          float attackerAniCutFrameLength, eHitState hitState, EAttackAttr attr)
    {
        this.attacker = attacker;
        this.attackerBehaviour = attackerBehaviour;
        this.attackerJumpPower = attackerJumpPower;
        this.hitDir = hitDir;
        this.hitEffId = hitEffId;
        this.critical = critical;
        this.attackerAniCutFrameLength = attackerAniCutFrameLength;
        this.hitState = hitState;
        AtkAttr = attr;
        skip = false;
    }

    public void Set(Unit attacker, eBehaviour attackerBehaviour, float attackerJumpPower, eHitDirection hitDir, int hitEffId, bool critical,
                    float attackerAniCutFrameLength, eHitState hitState, EAttackAttr attr)
    {
        this.attacker = attacker;
        this.attackerBehaviour = attackerBehaviour;
        this.attackerJumpPower = attackerJumpPower;
        this.hitDir = hitDir;
        this.hitEffId = hitEffId;
        this.critical = critical;
        this.attackerAniCutFrameLength = attackerAniCutFrameLength;
        this.hitState = hitState;
        AtkAttr = attr;
        skip = false;
    }
}
