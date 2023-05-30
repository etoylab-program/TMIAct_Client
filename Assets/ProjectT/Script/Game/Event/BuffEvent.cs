
public class BuffEvent : BaseEvent
{
    public int              id              { get; private set; }
    public float            duration        { get; private set; }
    public float            tick            { get; private set; }
    public int              effId           { get; private set; }
    public int              effId2          { get; private set; }
    public eBuffIconType    buffIconType    { get; private set; }
    public float            ExtraValue      { get; private set; }
    public float            ExtraValue2     { get; private set; }

	public ActionSelectSkillBase ActionSelectSkill { get; set; } = null;

    public BuffEvent()
    {
    }

    public BuffEvent(BattleOption.sBattleOptionData battleOptionData)
    {
        this.battleOptionData = battleOptionData;
    }

    public void Set(int id, eEventSubject eventSubject, eEventType eventType, Unit caster, float value, float value2, float value3, float duration, float tick,
                    int effId, int effId2, eBuffIconType buffIconType, float extraValue = 0.0f, float extraValue2 = 0.0f)
    {
        sender = caster;

        this.id = id;
        this.eventSubject = eventSubject;
        this.eventType = eventType;
        this.value = value;
        this.value2 = value2;
        this.value3 = value3;
        this.duration = duration;
        this.tick = tick;
        this.effId = effId;
        this.effId2 = effId2;
        this.buffIconType = buffIconType;
        ExtraValue = extraValue;
        ExtraValue2 = extraValue2;

		if ( ActionSelectSkill != null && battleOptionData != null ) {
			switch ( battleOptionData.buffDebuffType ) {
				case eBuffDebuffType.Buff: {
					AddDuration( ActionSelectSkill.AddBuffDuration );
				}
				break;

				case eBuffDebuffType.Debuff: {
					AddDuration( ActionSelectSkill.AddDebuffDuration );
				}
				break;

				default: {

				}
				break;
			}
		}
	}

    public void ChangeSender(Unit sender)
    {
        this.sender = sender;
    }

    public void ChangeValue1(float value1)
    {
        value = value1;
    }

    public void ChangeDuration(float duration)
    {
        this.duration = duration;
    }

    public void AddDuration(float add)
    {
        duration += add;
    }
}
