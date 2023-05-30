

public class StackEvent : BaseEvent
{
    public Unit caster { get; private set; }
    public BattleOption.eBOTimingType timingType { get; private set; }
    public BattleOption.eBOFuncType funcType { get; private set; }
    public GameClientTable.BattleOptionSet.Param paramBOSet { get; private set; }
    public int executeBattleOptionIdOnMaxStack { get; private set; }
    public eEventType executeEventOnMaxStack { get; private set; }
    public string stackEffIds { get; private set; }


    public void Set(eEventSubject eventSubject, eEventType eventType, Unit caster, BattleOption.eBOTimingType timingType, BattleOption.eBOFuncType funcType, 
                    float value, GameClientTable.BattleOptionSet.Param paramBOSet, int executeBattleOptionIdOnMaxStack, eEventType executeEventOnMaxStack,
                    string stackEffIds)
    {
        this.eventSubject = eventSubject;
        this.eventType = eventType;

        this.caster = caster;
        this.timingType = timingType;
        this.funcType = funcType;
        this.value = value;
        this.paramBOSet = paramBOSet;
        this.executeBattleOptionIdOnMaxStack = executeBattleOptionIdOnMaxStack;
        this.executeEventOnMaxStack = executeEventOnMaxStack;
        this.stackEffIds = stackEffIds;
    }
}
