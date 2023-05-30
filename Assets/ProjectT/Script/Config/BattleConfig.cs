
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;


[System.Serializable]
public class BattleConfig : ScriptableObject
{
    [Header("Test")]
    [SerializeField] private float _TestPlayerAtkPower = 0.0f;
    [SerializeField] private bool _TestWeaponMaxWakeView = false;

    [Header("BattleStatus")]
    [SerializeField] private float _HPRateMag = 1.0f;
    [SerializeField] private float _ATKRateMag = 0.3f;
    [SerializeField] private float _DEFRateMag = 0.38f;
    [SerializeField] private float _CRIRateMag = 0.21f;

    [Header("CombatPower")]
    [SerializeField] private float _CombatPowerHPMag = 2.02f;
    [SerializeField] private float _CombatPowerATKMag = 1.28f;
    [SerializeField] private float _CombatPowerDEFMag = 1.74f;
    [SerializeField] private float _CombatPowerCRIMag = 1.56f;
    [SerializeField] private float _CombatPowerBaseMag = 2.5f;
    [SerializeField] private float _CombatPowerCharSkillMag = 12.5f;
    [SerializeField] private float _CombatPowerSptSkillMag = 155.0f;
    [SerializeField] private List<float> _CombatPowerSptSkillRateByGrade;
    [SerializeField] private float _CombatPowerWpnSkillMag = 365.0f;
    [SerializeField] private List<float> _CombatPowerWpnSkillRateByGrade;
    [SerializeField] private float _CombatPowerGemOptMag = 2.5f;
    [SerializeField] public float TeamPowerCharMag = 0.1f;
    [SerializeField] public float TeamPowerBadgeMag = 0.001f;

    [Header("BattleBalance")]
    [SerializeField] private float _CriticalDmgRatio = 1.75f;
    [SerializeField] private float _MinDmgRndRatio = 0.95f;
    [SerializeField] private float _MaxDmgRndRatio = 1.05f;
    [SerializeField] private float _ComboClearTime = 3.0f;
    [SerializeField] private int _MinDropItemNum = 1;
    [SerializeField] private int _MaxDropItemNum = 5;
    [SerializeField] private float _EvadeAtkFrameGap = 10.0f;
    [SerializeField] private float _EvadeProjectileDistance = 5.0f;
    [SerializeField] private float _EvadeSlowMotionAniRate = 0.3f;
    [SerializeField] private float _EvadeSlowTimeScale = 0.3f;
    [SerializeField] private float _CriticalPauseFrameRatio = 1.5f;
    [SerializeField] private float _SprintStartConditionTime = 1.5f;
    [SerializeField] private float _SprintSpeedRatio = 1.0f;
    [SerializeField] private float _BuffDuration = 4.0f;
    [SerializeField] private float _PlayerWalkSpeedRatio = 0.65f;
    [SerializeField] private float _BlackholeMinSpeed = 6.0f;
    [SerializeField] private float _WeaponChangeTime = 20.0f;
    public float MaxSufferance = 10.0f;

    [Header("BattleBalance for Emily")]
    [SerializeField] public float EmilyDroneAttackRatio = 0.5f;
    [SerializeField] public float EmilySpeedAtAttack = 0.5f;

    [Header("Super Armor")]
    [SerializeField] private float _SuperArmorRegenStartTime = 6.0f;
    [SerializeField] private float _SuperArmorRegenSpeedTime = 60.0f;

    [Header("UltimateSkill")]
    [SerializeField] private ObscuredFloat _USMaxSP = 1000.0f;
    [SerializeField] private ObscuredFloat _USUseSP = 1000.0f;
    [SerializeField] private ObscuredFloat _USInitSP = 0.0f;
    [SerializeField] private ObscuredFloat _USAddSPByCombo = 2.0f;
    [SerializeField] private ObscuredFloat _USSPRegenSpeedTime = 180.0f;

    [Header("CharSkill")]
    [SerializeField] private float _CharSkillDmgRatio = 0.3f;

    [Header("NPCBattleAI")]
    [SerializeField] private float _AttackRangeAllowGap = 1.0f;

    [Header("Camara")]
    [SerializeField] private float _ShakePower = 2.0f;
    [SerializeField] private float _ShakePowerMin = 0.5f;
    [SerializeField] private float _ShakePowerMax = 1.5f;
    [SerializeField] public float CameraTurnSpeed = 1.0f;
    [SerializeField] public float CameraTargetLookSpeed = 3.0f;

    [Header("PauseFrame")]
    [SerializeField] private bool _SamePauseFrameOnHit = true;
    [SerializeField] private float _PauseFrameGapRatio = 0.5f;

    [Header("Player")]
    public float    ChangePlayerInvincibleTime  = 2.0f;
    public float    ReleaseTargetingTime        = 3.0f;
    public bool     AutoPlay                    = false;

    [Header("Player UI")]
    [SerializeField] private float _DangerHpRatio = 0.3f;
    [SerializeField] private List<int> _PlaySupporterVoiceByComboCnt;

    [Header("Buff/Debuff UI")]
    [SerializeField] private float _BuffDebuffIconFlashTime = 3.0f;
    [SerializeField] private float _BuffDebuffIconFlashDuration = 0.3f;

    [Header("Arena")]
    [SerializeField] private float _ArenaAtkBuffRate = 0.24f;
    [SerializeField] private float _ArenaDefBuffRate = 0.24f;
    [SerializeField] private float _ArenaHPRate = 10.0f;
    [SerializeField] private float _ArenaGemOptRate = 0.05f;
    [SerializeField] private float _ArenaCharSkillOptRate = 0.1f;
    [SerializeField] public float ArenaEnemyCharPassvieRate = 0.25f;
    [SerializeField] private float _ArenaTime = 90.0f;
    [SerializeField] public float ArenaWinLoseBuffRate = 0.075f;
    [SerializeField] private List<float> _ArenaGameSpeed;

    [Header("Arena/Cheer")]
    [SerializeField] public float CheerMaxGauge = 1000.0f;
    [SerializeField] public float CheerUseGauge = 1000.0f;
    [SerializeField] public float CheerInitGauge = 0.0f;
    [SerializeField] public float CheerAddGauge = 4.0f;
    [SerializeField] public float CheerRegenGauge = 120.0f;
    [SerializeField] public List<int> CheerEffBOList;
    [SerializeField] public List<int> CheerEffDescList;

    [Header("Raid")]
    [SerializeField] public float RaidAtkBuffRate = 0.24f;
    [SerializeField] public float RaidHPBuffRate = 0.24f;
    [SerializeField] public float RaidIncAtkRate = 0.1f;
    [SerializeField] public float RaidIncHPRate = 0.1f;
    [SerializeField] public float RaidIncShieldRate = 0.1f;
    [SerializeField] public float MonsterLimitDef = 30000.0f;
    [SerializeField] public float MonsterLimitSpd = 20.0f;
    [SerializeField] public float MonsterLimitCriRate = 50.0f;
    [SerializeField] public float MonsterLimitCriDmg = 100.0f;
    [SerializeField] public float MonsterLimitCriReg = 50.0f;
    [SerializeField] public float MonsterLimitCriDef = 100.0f;
    [SerializeField] public float MonsterLimitPenetrate = 30.0f;
          

    public float TestPlayerAtkPower { get { return _TestPlayerAtkPower; } }
    public bool TestWeaponMaxWakeView { get { return _TestWeaponMaxWakeView; } }

    public float HPRateMag { get { return _HPRateMag; } }
    public float ATKRateMag { get { return _ATKRateMag; } }
    public float DEFRateMag { get { return _DEFRateMag; } }
    public float CRIRateMag { get { return _CRIRateMag; } }

    public float CombatPowerHPMag { get { return _CombatPowerHPMag; } }
    public float CombatPowerATKMag { get { return _CombatPowerATKMag; } }
    public float CombatPowerDEFMag { get { return _CombatPowerDEFMag; } }
    public float CombatPowerCRIMag { get { return _CombatPowerCRIMag; } }
    public float CombatPowerBaseMag { get { return _CombatPowerBaseMag; } }
    public float CombatPowerCharSkillMag { get { return _CombatPowerCharSkillMag; } }
    public float CombatPowerSptSkillMag { get { return _CombatPowerSptSkillMag; } }
    public List<float> CombatPowerSptSkillRateByGrade { get { return _CombatPowerSptSkillRateByGrade; } }
    public float CombatPowerWpnSkillMag { get { return _CombatPowerWpnSkillMag; } }
    public List<float> CombatPowerWpnSkillRateByGrade { get { return _CombatPowerWpnSkillRateByGrade; } }
    public float CombatPowerGemOptMag { get { return _CombatPowerGemOptMag; } }

    public float CriticalDmgRatio { get { return _CriticalDmgRatio; } }
    public float MinDmgRndRatio { get { return _MinDmgRndRatio; } }
    public float MaxDmgRndRatio { get { return _MaxDmgRndRatio; } }
    public float SuperArmorRegenStartTime { get { return _SuperArmorRegenStartTime; } }
    public float SuperArmorRegenSpeedTime { get { return _SuperArmorRegenSpeedTime; } }
    public float ComboClearTime { get { return _ComboClearTime; } }
    public int MinDropItemNum { get { return _MinDropItemNum; } }
    public int MaxDropItemNum { get { return _MaxDropItemNum; } }
    public float EvadeAtkFrameGap { get { return _EvadeAtkFrameGap; } }
    public float EvadeProjectileDistance { get { return _EvadeProjectileDistance; } }
    public float EvadeSlowMotionAniRate { get { return _EvadeSlowMotionAniRate; } }
    public float EvadeSlowTimeScale { get { return _EvadeSlowTimeScale; } }
    public float CriticalPauseFrameRatio { get { return _CriticalPauseFrameRatio; } }
    public float SprintStartConditionTime { get { return _SprintStartConditionTime; } }
    public float SprintSpeedRatio { get { return _SprintSpeedRatio; } }
    public float BuffDuration { get { return _BuffDuration; } }
    public float PlayerWalkSpeedRatio { get { return _PlayerWalkSpeedRatio;} }
    public float BlackholeMinSpeed { get { return _BlackholeMinSpeed;} }
    public float WeaponChangeTime { get { return _WeaponChangeTime; } }

    public ObscuredFloat USMaxSP { get { return _USMaxSP; } }
    public ObscuredFloat USUseSP { get { return _USUseSP; } }
    public ObscuredFloat USInitSP { get { return _USInitSP; } }
    public ObscuredFloat USAddSPByCombo { get { return _USAddSPByCombo; } }
    public ObscuredFloat USSPRegenSpeedTime { get { return _USSPRegenSpeedTime; } }

    public float CharSkillDmgRatio { get { return _CharSkillDmgRatio; } }

    public float AttackRangeAllowGap { get { return _AttackRangeAllowGap; } }

    public float ShakePower { get { return _ShakePower; } }
    public float ShakePowerMin { get { return _ShakePowerMin; } }
    public float ShakePowerMax { get { return _ShakePowerMax; } }

    public bool SamePauseFrameOnHit { get { return _SamePauseFrameOnHit; } }
    public float PauseFrameGapRatio { get { return _PauseFrameGapRatio; } }

    public float DangerHpRatio { get { return _DangerHpRatio; } }
    public List<int> PlaySupporterVoiceByComboCnt { get { return _PlaySupporterVoiceByComboCnt; } }

    public float BuffDebuffIconFlashTime { get { return _BuffDebuffIconFlashTime; } }
    public float BuffDebuffIconFlashDuration { get { return _BuffDebuffIconFlashDuration;} }

    public float ArenaAtkBuffRate { get { return _ArenaAtkBuffRate; } }
    public float ArenaDefBuffRate { get { return _ArenaDefBuffRate; } }
    public float ArenaHPRate { get { return _ArenaHPRate; } }
    public float ArenaGemOptRate { get { return _ArenaGemOptRate; } }
    public float ArenaCharSkillOptRate { get { return _ArenaCharSkillOptRate; } }
    public float ArenaTime { get { return _ArenaTime; } }
    public List<float> ArenaGameSpeed { get { return _ArenaGameSpeed; } }    
}
