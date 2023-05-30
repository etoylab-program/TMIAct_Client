

public class ActionEvent : BaseEvent {
	public eActionCommand	ActionCommand	{ get; private set; }
	public int				EffId			{ get; private set; }


	public ActionEvent() {
	}

	public ActionEvent( BattleOption.sBattleOptionData battleOptionData ) {
		this.battleOptionData = battleOptionData;
	}

	public void Set( eEventSubject eventSubject, eEventType eventType, Unit unit, eActionCommand actionCommand, float value, int effId ) {
		sender = unit;
		ActionCommand = actionCommand;
		EffId = effId;

		this.eventSubject = eventSubject;
		this.eventType = eventType;
		this.value = value;
	}
}
