
using System.Collections.Generic;


public class ProjectileEvent : BaseEvent
{
    public List<Projectile>	ListProjectile	{ get; private set; } = new List<Projectile>();
	public float			Duration		{ get; private set; } = 0.0f;
	public float			Tick			{ get; private set; } = 0.0f;


    public ProjectileEvent()
    {
    }

    public ProjectileEvent(BattleOption.sBattleOptionData battleOptionData)
    {
        this.battleOptionData = battleOptionData;
    }

	public void Set(eEventSubject eventSubject, eEventType eventType, Unit unit, List<Projectile> listProjectile, float value, float value2, float value3,
					float duration = 0.0f, float tick = 0.0f)
    {
        this.eventSubject = eventSubject;
        this.eventType = eventType;
        this.value = value;
        this.value2 = value2;
		this.value3 = value3;

		Duration = duration;
		Tick = tick;

        sender = unit;

		for (int i = 0; i < listProjectile.Count; i++)
		{
			ListProjectile.Add(listProjectile[i]);
		}
    }
}
