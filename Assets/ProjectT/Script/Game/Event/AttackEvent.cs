
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;


public class AttackEvent : BaseEvent {
	public bool							SkipCheckOwnerAction	{ get; set; }			= false;
	public bool							SkipMaxDamageRecord		{ get; set; }			= false;
	public BattleOption.eToExecuteType	ToExecuteType			{ get; private set; }
	public AniEvent.sEvent				aniEvent				{ get; private set; }
	public ObscuredFloat				atkPower				{ get; private set; }
	public eAttackDirection				atkDir					{ get; private set; }
	public bool							isCritical				{ get; private set; }
	public int							attackerEffId			{ get; private set; }
	public EffectManager.eType			attackerEffType			{ get; private set; }
	public List<UnitCollider>			listTargetCollider		{ get; private set; }	= new List<UnitCollider>();
	public float						delay					{ get; private set; }
	public bool							notAniEventAtk			{ get; private set; }
	public bool							isUltimateSkill			{ get; private set; }	= false;
	public bool							onlyDamageHit			{ get; private set; }	= false;
	public int							ActionTableId			{ get; private set; }	= 0;
	public Projectile					Pjt						{ get; private set; }	= null;
	public DroneUnit                    DroneUnit				{ get; private set; }	= null;


	public AttackEvent() {
		eventSubject = eEventSubject.World;
	}

	public void Set( eEventType eventType, Unit attacker, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent aniEvent, ObscuredFloat atkPower,
					 eAttackDirection atkDir, bool isCritical, int attackerEffId, EffectManager.eType attackerEffType, List<UnitCollider> list, float delay,
					 bool notAniEventAtk, bool isUltimateSkill = false, bool onlyDamageHit = false, int actionTableId = 0, Projectile projectile = null,
					 int maxListCount = 0, DroneUnit droneUnit = null ) {
		sender = attacker;

		this.eventType = eventType;
		this.ToExecuteType = toExecuteType;
		this.aniEvent = aniEvent;
		this.atkPower = atkPower;
		this.atkDir = atkDir;
		this.isCritical = isCritical;
		this.attackerEffId = attackerEffId;
		this.attackerEffType = attackerEffType;
		this.notAniEventAtk = notAniEventAtk;
		this.isUltimateSkill = isUltimateSkill;
		this.onlyDamageHit = onlyDamageHit;
		this.delay = delay;

		listTargetCollider.Clear();

		if( list != null && list.Count > 0 ) {
			for( int i = 0; i < list.Count; i++ ) {
				if( maxListCount > 0 && i >= maxListCount ) {
					continue;
				} 

				listTargetCollider.Add( list[i] );
			}
		}

		ActionTableId = actionTableId;
		Pjt = projectile;

		DroneUnit = droneUnit;
	}

	public void SetWithSingleTarget( eEventType eventType, Unit attacker, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent aniEvent,
									 ObscuredFloat atkPower, eAttackDirection atkDir, bool isCritical, int attackerEffId, EffectManager.eType attackerEffType,
									 UnitCollider targetCollider, float delay, bool notAniEventAtk, bool isUltimateSkill = false, bool onlyDamageHit = false,
									 int actionTableId = 0, Projectile projectile = null ) {
		sender = attacker;

		this.eventType = eventType;
		this.ToExecuteType = toExecuteType;
		this.aniEvent = aniEvent;
		this.atkPower = atkPower;
		this.atkDir = atkDir;
		this.isCritical = isCritical;
		this.attackerEffId = attackerEffId;
		this.attackerEffType = attackerEffType;
		this.notAniEventAtk = notAniEventAtk;
		this.isUltimateSkill = isUltimateSkill;
		this.onlyDamageHit = onlyDamageHit;
		this.delay = delay;

		listTargetCollider.Clear();
		listTargetCollider.Add( targetCollider );

		ActionTableId = actionTableId;
		Pjt = projectile;
	}

	public bool IsRangeAttack() {
		if ( Pjt == null ) {
			return false;
		}

		return !Pjt.IsMelee;
	}
}
