
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BattleOption
{
    public enum eToExecuteType
    {
        Unit = 0,
        Supporter,
        Weapon,
        Item,
		Temporary,
    }

    public enum eBOTimingType
    {
        None = 0,

        Use,
        UseAction,          // 서포터, 무기 스킬 액션
        AddCall,            // 다른 배틀 옵션에서 호출
        OnHit,              // 대미지가 들어갔을 때
        OnHitAction,        // 피격 모션 시
        OnAttack,           // 공격 적중 시
        OnNormalAttack,     // 일반 공격 적중 시
        OnSkillAttack,      // 스킬 공격 적중 시
        OnSkillEnd,         // 스킬 공격 종료 시
        OnCriAttack,        // 크리 공격 적중 시
        OnCriNormalAttack,  // 일반 공격 크리 적중 시
        OnCriSkillAttack,   // 스킬 공격 크리 적중 시
        OnMaxSP,            // 대마력 게이지가 가득차는 순간
        OnEvade,            // 회피 시
        OnEmergencyEvade,   // 긴급 회피 시
		OnAllEvade,			// 모든 회피 시
        GetItem,
        OnDie,
        StartSkill,
        DuringSkill,        // 스킬 중 (해당 액션이 끝날 때 EndBattleOption 함수 호출 해줘야 함)
        OnStartAddAction,   // 추가 액션이 시작될 때 (해당 액션에서 직접 구현해줘야함)
        OnRecovery,         // 회복 시

        // 버프/디버프 스탯으로 들어갈 타이밍타입들
        GameStart,
        MaxSP,              // 대마력 게이지가 가득차 있는 상태

        ManualCall,         // 수동으로 배틀옵션 호출

        MissionStart,       // GameStart 바로 다음

		OnEnemyAppear,		// 몬스터 등장 시
        OnEnemyDie,
		OnProjectileFire,   // 프로젝타일을 발사할때
		OnProjectileHit,    // 프로젝타일에 맞았을 시
    }

    public enum eBOConditionType
    {
        None = 0,
        Groggy,
        Down,
        Float,
        HoldPosition,
        CancelOnHit,
        OnEmergencyEvade,
        HasBuff,
        HasDebuff,
        HasDebuffGroggy,
        HasDotAddiction,
        HasDotBleeding,
        HasDotFlame,
        ComboCountAsValue,
        MonTypeHuman,
        MonTypeMachine,
        MonTypeDevil,
        MonGradeNormal,
        MonGradeEpic,
        MonGradeBoss,
        SelfTypeHuman,
        SelfTypeMachine,
        SelfTypeDevil,
        UnitTypePlayer,
        UnitTypeEnemy,
        NormalAttacking,    // 일반 공격 중
        MeleeAttack,
        RangeAttack,
        CharTypeAsagi,
        CharTypeSakura,
        CharTypeYukikaze,
        CharTypeRinko,
        CharTypeMurasaki,
        CharTypeJinglei,
        CharTypeShiranui,
        CharTypeEmily,
        CharTypeKurenai,
        CharTypeOboro,
        CharTypeAsuka,
        CharTypeKirara,
        CharTypeIngrid,
        CharTypeNoah,
        CharTypeAstaroth,
        CharTypeTokiko,
        CharTypeShizuru,
		CharTypeFelicia,
		CharTypeMaika,
		CharTypeRin,
        CharTypeSora,
        CharTypeAnnerose,
        CharTypeNagi,
        CharTypeAina,
        CharTypeSaika,
        CharTypeShisui,
        Character,
        CharacterSkillId,

        SakuraChargeAttack,
        MurasakiChargeAttack,
        RinkoRushAttack,
        IngridComboAttack,
        AsagiChargeAttack,
        SakuraJumpThrowAttack,
        RinkoChargeHomingAttack,
        MurasakiChainRushAttack,
        JingleiKikoshoAttack,
        KurenaiTimingHoldAttack,
        OboroChargeAttack,
		ShiranuiSpearAttack,
		FeliciaEvadeCharge2Attack,
		AsukaTeleportAttack,
		KiraraEvadeCharge2Attack,
		YukikazeChargeLightningBallAttack,
        IngridEvadeChargeAttack,
        NoahEvadeChargeAttack,
        AsagiJumpDownAttack,
        AstarothAttackHoldAttack,
        KiraraExtreamEvade,
        SakuraExtreamEvadeSummon,
        TokikoTimingHold2Attack,
        EmilySummonDrone,
        RinChargeAttack,
        AstarothExtreamEvade2,
        ShiranuiTornadoAttack,
        KurenaiChargeAttack,
        ShizuruEvadeChargeAttack,

        AbnormalFire,
        AbnormalIce,
        AbnormalElectric,
        AbnormalPoison,
        AbnormalBleeding,
		AbnormalFreeze,

		SpeedDown,
        StayOnProjectile,
        StayOnBoxCollider,

        CheckHpLessEqual10,
        CheckHpLessEqual20,
        CheckHpLessEqual30,
        CheckHpLessEqual40,
        CheckHpLessEqual50,
        CheckHpLessEqual60,
        CheckHpLessEqual70,
        CheckHpLessEqual80,
        CheckHpLessEqual90,

        PlayerSummons,

        SupportType,        // 지원 속성
        ProtectType,        // 보호 속성
        AggressiveType,     // 제압 속성

        StageMain,
        StageDaily,
        StageEvent,
        StageTower,
        StageRaid,

		EnemyGradeLessEqual2,
		EnemyGradeEqual3,

        CheckSp,

        FloatMeleeAttack,

		ShiranuiComboAttack,
        ShiranuiChargeSummonAttack,
        ShiranuiEvadeSummon,
		SaikaAttackHoldAttack,
		ShiranuiEvadeClone,
		SkipEnemyAttack,
	}

    public enum eBOAtkConditionType
    {
        None = 0,
        MeleeAttack,
        RangeAttack,
        NormalAttack,
        SkillAttack,
        SkillAction,
        UltimateSkill,
        WeaponSkill,

		MaikaDefaultAttack, // 마이카 기본 공격, 기본 공격 강화
    }

    public enum eBOTargetType
    {
        None = 0,

        ActiveEnemies,
        ActiveEnemiesInRange,
        HitTargetList,
        HitTargetDie,       // 플레이어가 적을 죽였을 때 그 적에게 메세지 보냄
        OnHitTargetDie,     // 플레이어가 적을 죽였을 때 플레이어에게 메세지 보냄
        OnHitActionEnemies, // 히트 액션을 하고 있는 적들만
        Player,
        MainTarget,
        Self,
        SelfAction,
        Caster,
        PlayerSummons,      // 플레이어 소환수
		AppearEnemy,
        Attacker,
        SelectTarget,
        PlayerAll,
        HitTargetListOnce,
        PlayerAllInRange,

        All,
    }

    public enum eBOAddCallTiming
    {
        None = 0,

        OnStart,
        OnSend,
        OnEnd,
    }

    public enum eBOFuncType
    {
        None = 0,

        SprintTimeCut,
        OnlyLastAttack,
        LastAttackKnockBack,
        CriRateUp,
        CriRateDown,
		CriRateUpWithEnemyCountRatio,
		CriRateDownWithEnemyCountRatio,
		CriDmgUp,
        CriDmgDown,
        CriResistUp,
        CriResistDown,
        CriDefUp,
        CriDefDown,
        ClearDebuff,
        RemoveDebuff,
        RemoveDebuffAndIncapable,
        ClearBuff,
        RemoveBuff,
        IgnoreCriRate,
        DotDmg,
        BloodSucking,
        DotHeal,
        Heal,
        HealOnDieInTime,
        DotHealAbs,
        HealAbs,
        DotHealCostHP,
        HealCostHP,
        DotHealCostHPCondiHP,
        AddSP,
        AddSPPercentage,
        DecreaseSP,
        DecreaseSPPercentage,
        SpeedUp,
        SpeedUpOnEffect,
        SpeedDown,
        SpeedDownToTarget,
        AtkPowerRateUpToShield,
        AtkPowerRateDownToShield,
        UltimateSkillAtkPowerRateUp,
        UltimateSkillAtkPowerRateDown,
        DmgRateDown,
		DmgRateUpWithEnemyCountRatio,
		DmgRateDownWithEnemyCountRatio,
        DmgRateDownWithAccumDamage,
		DmgRateDownByBuffCount,
		DmgRateUp,
        AtkPowerRateUp,
        AtkPowerRateUpOnce,
        AtkPowerRateUpTimeInc,
		AtkPowerRateUpByBuffCount,
        AtkPowerRateUpByEnemyDebuffStack,
        AtkPowerRateUpInc,
        AtkPowerRateUpWithEnemyCountRatio,
        AtkPowerRateDown,
        AtkPowerRateDownWithEnemyCountRatio,
        HealRateUp,
        HealRateDown,
        DotShieldDmg,
        CommandAction,
        CommandActionCustom1,
        FireProjectile,
		FireProjectileRegardlessTargetCount,
		FireProjectileRepeat,
		Reflection,
        ReflectionAround,
        RandomKnockback,
        Attack,
        UpperAttack,
        KnockBackAttack,
        DownAttack,
        StunAttack,
        GroggyAttack,
		AttackCritical,
        AttackRepeat,
        HoldPosition,
        SendHoldPositionToTarget,
        SendDotDmgToTarget,
        SendAttackToTarget,
        AddDropItemRate,
        AddDropItemCnt,
        SuperArmor,
        SuperArmorTimeInc,
        ConditionalSuperArmor,
        IncreaseSuperArmorDurationBySkill,
        AddAttack,
        AddSpAttack,
        ComboResetByHit,
        SkipEnemyAttack,
        DecreaseSupporterCoolTime,
        DecreaseSupporterCoolTimeInRealTime,
        DecreaseSkillCoolTime,
        DecreaseActionSkillCoolTime,
        DecreaseActionSkillCoolTimeInRealTime,
        DecreaseOneActionSkillCoolTimeSec,
        DecreaseOnlyActionSkillCoolTimeSec,
        IncreaseSPRegenRate,
        IncreaseSPAddRate,
        DecreaseSPAddRate,
        IncreaseSPAddRateTimeInc,
        DecreaseSPAddRateTimeInc,
        ImmuneAttackAttr,
        ImmuneAtkPowerRateDown,
        ImmuneDmgRateUp,
        ImmuneSpeedDown,
        DebuffTimeReduce,
        DebuffTimePromote,
        SetAddAction,
        MovementInverse,
        AddMaxHp,
        Freeze,
        UseCloneAttack2,
        Marking,
        AttackUsingAccumDamage,
        HoldSupprterSkillCoolTime,
        AddBuffDebuffDuration,
        IncreaseSummonsAttackPower,
        Blackhole,
        Hide,
		SummonPlayerMinion,
        SummonPlayerMinionByDieEnemyCount,
        ResetActionSkillCoolTimeByUseSP,
		BreakShieldImmediate,
		ElectricShock,
		LightningAttack,
        FireAura,
        Pushed,
        ResetAllSkillCoolTime,
        Death,
        IncreaseAddActionValue,
        SetExtraPlayerFunc,
        SetDebuffImmune,
        PenetrateUp,
        SufferanceUp,
        IncreaseCharSkillCoolTime,
        IncreaseCharSupporterSkillCoolTime,
        UseSPPercentage,
        ResetRandomSkillCoolTime,
    }

    public enum eBOCallAddBOSetTiming
    {
        None = 0,

        OnStart,
        OnEnd,
    }

    public enum eSkillType
    {
        Passive = 0,
        Active,
        ConditionalActive_HasAction = 2, // 조건부 액티브이고 액션으로 조건 체크
    }


	public class sBattleOptionData {
		public eToExecuteType       toExecuteType;

		public int                  battleOptionSetId;
		public eBOTimingType        timingType;
		public eBOFuncType          funcType;
		public float                targetValue;
		public string               projectileName;
		public eBOConditionType     startCondtionType;
		public eBOConditionType     preConditionType;
		public eBOConditionType     conditionType;
		public eBOAtkConditionType  atkConditionType;
		public float                CondValue;
		public eBOTargetType        targetType;
		public eBuffDebuffType      buffDebuffType;
		public eBOAddCallTiming     addCallTiming;
		public int                  referenceBattleOptionSetId;
		public int                  actionTableId;
		public int                  CheckActionTableId;
		public BaseEvent            evt;

		public float                value;
		public float                value2;
		public float                value3;
		public float                duration;
		public float                originalDuration;
		public float                tick;
		public int                  maxStack;

		public float                startDelay;
		public int                  randomStart;

		public int                  effType;
		public int                  noDelayStartEffId;
		public int                  startEffId;
		public int                  endEffId;
		public int                  effId1;
		public int                  effId2;

		public sBattleOptionData    dataOnEndCall;

		public bool                 useOnce;
		public bool                 useOnceAndIgnoreFunc;

		public bool                 buffIconFlash               = true;
		public bool                 repeat                      = false;
		public float                repeatDelay                 = 0.0f;

		public float                CoolTime                    = -1.0f;
		public float                CheckCoolTime               = 0.0f;
		public Coroutine            CrCoolTime                  = null;

		public bool                 use                         = false;
		public DateTime             useTime                     = new DateTime(0);

		public Projectile           Pjt                         = null;
		public int[]                MinionIds                   = null;

        public float                AddDuration                 = 0.0f;

		public eBOConditionType[]   ActionConditionType         = null;

		private WaitForFixedUpdate  mWaitForFixedUpdate         = new WaitForFixedUpdate();


		public void OnEnd() {
		}

		public IEnumerator UpdateCoolTime() {
			while( true ) {
				if( !World.Instance.IsPause ) {
					CheckCoolTime += Time.fixedDeltaTime;
				}

				yield return mWaitForFixedUpdate;
			}
		}
	}


	public List<sBattleOptionData> ListBattleOptionData { get; private set; } = new List<sBattleOptionData>();

    protected Unit                                          mOwner                  = null;
    protected eToExecuteType                                mToExecuteType          = eToExecuteType.Unit;
    protected List<GameClientTable.BattleOptionSet.Param>   mListBattleOptionSet    = new List<GameClientTable.BattleOptionSet.Param>();
    protected eActionCommand                                mActionCommand          = eActionCommand.None;
	protected WaitForFixedUpdate							mWaitForFixedUpdate		= new WaitForFixedUpdate();


	protected BattleOption(Unit owner)
    {
        SetOwner(owner);
    }

    public void SetOwner(Unit owner)
    {
        mOwner = owner;
    }

    public virtual bool HasActiveSkill()
    {
        return false;
    }

    public bool HasBattleOption(eBOTimingType timingType, eBOFuncType funcType)
    {
        List<sBattleOptionData> findAll = ListBattleOptionData.FindAll(x => x.timingType == timingType);
        if (findAll == null || findAll.Count <= 0)
        {
            return false;
        }

        for(int i = 0; i < findAll.Count; i++)
        {
            if(findAll[i].funcType == funcType || (findAll[i].dataOnEndCall != null && findAll[i].dataOnEndCall.funcType == funcType))
            {
                return true;
            }
        }

        return false;
    }

    public void DeleteBattleOptionSet(int id)
    {
        sBattleOptionData find = ListBattleOptionData.Find(x => x.battleOptionSetId == id);
        if(find == null)
        {
            Debug.Log("삭제할 " + id + "번 배틀 옵션 셋이 존재하지 않습니다.");
            return;
        }

        Utility.StopCoroutine(World.Instance, ref find.CrCoolTime);
        ListBattleOptionData.Remove(find);
    }

    protected virtual void Parse(int level, int actionTableId = -1)
    {
        for (int i = 0; i < mListBattleOptionSet.Count; i++)
        {
            if(!AddBattleOptionSet(mListBattleOptionSet[i], level, actionTableId))
            {
                continue;
            }
        }
    }

    public bool AddBattleOptionSet(GameClientTable.BattleOptionSet.Param paramBOSet, int level, int actionTableId)
    {
		Player player = mOwner as Player;

        // 액션 추가
        if (!string.IsNullOrEmpty(paramBOSet.BOAction))
        {
            Type type = System.Type.GetType("Action" + paramBOSet.BOAction);
            ActionBase action = (ActionBase)mOwner.gameObject.AddComponent(type);
            if (action != null)
            {
                if ( player && player.UseGuardian ) {
                    player.AddAfterActionType( type );
                }

                if (paramBOSet.BOIndependentActionSystem <= 0)
                {
                    mOwner.actionSystem.AddAction(action, 0, null);
                }
                else if(mOwner.ActionSystem3)
                {
                    action.IndependentActionSystem = true;
                    mOwner.ActionSystem3.AddAction(action, 0, null);
                }
            }

            mActionCommand = Utility.GetActionCommandByString(paramBOSet.BOAction);
        }

        GameClientTable.BattleOption.Param param = GameInfo.Instance.GameClientTable.FindBattleOption(paramBOSet.BattleOptionID);
        if (param == null)
        {
            return false;
        }

        sBattleOptionData data = CreateBattleOptionData(paramBOSet, param, level, actionTableId);
        if (data != null)
        {
            if (param.BOAddBOSetID > 0)
            {
                GameClientTable.BattleOptionSet.Param paramAddBOSet = GameInfo.Instance.GameClientTable.FindBattleOptionSet(param.BOAddBOSetID);
                param = GameInfo.Instance.GameClientTable.FindBattleOption(paramAddBOSet.BattleOptionID);

				// 액션 추가
				if (!string.IsNullOrEmpty(paramAddBOSet.BOAction))
				{
					Type type = System.Type.GetType("Action" + paramAddBOSet.BOAction);
					ActionBase action = (ActionBase)mOwner.gameObject.AddComponent(type);
					if (action != null)
					{
						if ( player && player.UseGuardian ) {
							player.AddAfterActionType( type );
						}

						if (paramAddBOSet.BOIndependentActionSystem <= 0)
						{
							mOwner.actionSystem.AddAction(action, 0, null);
						}
						else if (mOwner.ActionSystem3)
						{
							action.IndependentActionSystem = true;
							mOwner.ActionSystem3.AddAction(action, 0, null);
						}
					}

					mActionCommand = Utility.GetActionCommandByString(paramAddBOSet.BOAction);
				}

				sBattleOptionData dataOnEndCall = CreateBattleOptionData(paramAddBOSet, param, level, actionTableId);
                data.dataOnEndCall = dataOnEndCall;
            }

            ListBattleOptionData.Add(data);
        }
        return true;
    }

    protected sBattleOptionData CreateBattleOptionData(GameClientTable.BattleOptionSet.Param paramBOSet, GameClientTable.BattleOption.Param paramBO, 
                                                       int level, int actionTableId)
    {
		eBOTimingType timingType = Utility.GetEnumByString<eBOTimingType>(paramBO.CheckTimingType);
		eBOConditionType startConditionType = Utility.GetEnumByString<eBOConditionType>(paramBO.StartCondType);
		eBOConditionType preConditionType = Utility.GetEnumByString<eBOConditionType>(paramBO.BOPreCondType);
		eBOConditionType conditionType = Utility.GetEnumByString<eBOConditionType>(paramBO.BOCondType);
		eBOAtkConditionType atkConditionType = Utility.GetEnumByString<eBOAtkConditionType>(paramBO.BOAtkCondType);
		eBOTargetType targetType = Utility.GetEnumByString<eBOTargetType>(paramBO.BOTarType);

		eEventSubject eventSubject = eEventSubject.None;
        if (targetType == eBOTargetType.Self)
            eventSubject = eEventSubject.Self;
        if( targetType == eBOTargetType.Player )
            eventSubject = eEventSubject.Player;
        else if( targetType == eBOTargetType.PlayerAll )
            eventSubject = eEventSubject.PlayerAll;
        else if( targetType == eBOTargetType.MainTarget )
            eventSubject = eEventSubject.MainTarget;
        else if( targetType == eBOTargetType.HitTargetList )
            eventSubject = eEventSubject.HitTargetList;
        else if( targetType == eBOTargetType.HitTargetDie )
            eventSubject = eEventSubject.HitTargetDie;
        else if( targetType == eBOTargetType.OnHitTargetDie )
            eventSubject = eEventSubject.OnHitTargetDie;
        else if( targetType == eBOTargetType.OnHitActionEnemies ) {
            eventSubject = eEventSubject.OnHitActionEnemies;
        }
        else if( targetType == eBOTargetType.Caster )
            eventSubject = eEventSubject.Caster;
        else if( targetType == eBOTargetType.SelfAction )
            eventSubject = eEventSubject.Action;
        else if( targetType == eBOTargetType.ActiveEnemies )
            eventSubject = eEventSubject.ActiveEnemies;
        else if( targetType == eBOTargetType.ActiveEnemiesInRange )
            eventSubject = eEventSubject.ActiveEnemiesInRange;
        else if( targetType == eBOTargetType.PlayerSummons ) {
            eventSubject = eEventSubject.PlayerSummons;
        }
        else if( targetType == eBOTargetType.AppearEnemy ) {
            eventSubject = eEventSubject.AppearEnemy;
        }
        else if( targetType == eBOTargetType.Attacker ) {
            eventSubject = eEventSubject.Attacker;
        }
        else if( targetType == eBOTargetType.SelectTarget ) {
            eventSubject = eEventSubject.SelectTarget;
        }
        else if( targetType == eBOTargetType.All ) {
            eventSubject = eEventSubject.Player;
            eventSubject |= eEventSubject.ActiveEnemies;
        }
        else if( targetType == eBOTargetType.HitTargetListOnce ) {
            eventSubject = eEventSubject.HitTargetListOnce;
        }
        else if ( targetType == eBOTargetType.PlayerAllInRange ) {
            eventSubject = eEventSubject.PlayerAllInRange;
		}

        eBuffDebuffType buffDebuffType = Utility.GetEnumByString<eBuffDebuffType>(paramBO.BOBuffType);
        eBOAddCallTiming addCallTiming = Utility.GetEnumByString<eBOAddCallTiming>(paramBO.BOAddCallTiming);
		eBOFuncType funcType = Utility.GetEnumByString<eBOFuncType>(paramBO.BOFuncType);

        sBattleOptionData data = new sBattleOptionData();
        data.toExecuteType = mToExecuteType;
        data.battleOptionSetId = paramBOSet.ID;
        data.duration = paramBOSet.BOBuffDurTime;
        data.originalDuration = data.duration;
        data.tick = paramBOSet.BOBuffTurnTime;
        data.maxStack = paramBOSet.BOBuffStackValue;
        data.timingType = timingType;
        data.funcType = funcType;
		data.startCondtionType = startConditionType;
        data.preConditionType = preConditionType;
        data.conditionType = conditionType;
        data.atkConditionType = atkConditionType;
        data.CondValue = paramBO.CondValue;
		data.targetType = targetType;
        data.targetValue = paramBO.BOTarValue;
        data.projectileName = paramBOSet.BOProjectile;
        data.buffDebuffType = buffDebuffType;
        data.addCallTiming = addCallTiming;
        data.referenceBattleOptionSetId = paramBOSet.BOReferenceSetID;
        data.actionTableId = actionTableId;
        data.CheckActionTableId = 0;
        data.startDelay = paramBOSet.BOStartDelay;
        data.randomStart = paramBOSet.BORandomStart;
        data.effType = paramBOSet.BOEffectType;
        data.noDelayStartEffId = paramBOSet.BONoDelayStartEffId;
        data.startEffId = paramBOSet.BOStartEffectId;
        data.endEffId = paramBOSet.BOEndEffectId;
        data.effId1 = paramBOSet.BOEffectIndex;
        data.effId2 = paramBOSet.BOEffectIndex2;
        data.useOnce = paramBOSet.UseOnce == 0 ? false : true;
		data.useOnceAndIgnoreFunc = paramBO.UseOnceAndIgnoreFunc == 0 ? false : true;
		data.buffIconFlash = paramBO.BuffIconFlash == 0 ? false : true;
        data.repeat = paramBO.Repeat == 0 ? false : true;
        data.repeatDelay = paramBO.RepeatDelay;
        data.CoolTime = paramBOSet.BOCoolTime;
        data.CheckCoolTime = data.CoolTime;
        data.CrCoolTime = null;

		if ( !string.IsNullOrEmpty( paramBO.BOActionCondType ) ) {
			string[] splits = paramBO.BOActionCondType.Split( ',' );
			data.ActionConditionType = new eBOConditionType[splits.Length];
			for ( int s = 0; s < splits.Length; s++ ) {
				data.ActionConditionType[s] = Utility.GetEnumByString<eBOConditionType>( splits[s] );
			}
		}

        float incValue = Mathf.Max(0, (level - 1)) * (paramBOSet.BOFuncIncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE);

        eEventType selectEventType = eEventType.EVENT_NONE;
        switch (funcType)
        {
            //=================================================================================
            // 액션/프로젝타일
            //=================================================================================

            // 전력 질주까지 걸리는 시간 단축
            case eBOFuncType.SprintTimeCut:
                data.value = paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE;

                data.evt = new ActionEvent();
                ((ActionEvent)data.evt).Set(eventSubject, eEventType.EVENT_ACTION_SET_SPRINT_START_TIME_RATE, mOwner, eActionCommand.None, data.value, paramBOSet.BOEffectIndex);
                break;

            // 일반 지상 공격이 무조건 막타로 발동
            case eBOFuncType.OnlyLastAttack:
                data.value = 1.0f;

                data.evt = new ActionEvent();
                ((ActionEvent)data.evt).Set(eventSubject, eEventType.EVENT_ACTION_SET_ONLY_LAST_MELEE_ATTACK, mOwner, eActionCommand.None, data.value, paramBOSet.BOEffectIndex);
                break;

            // 일반 공격의 마지막 타격 시 넉백 공격
            case eBOFuncType.LastAttackKnockBack:
                data.value = 1.0f;

                data.evt = new ActionEvent();
                ((ActionEvent)data.evt).Set(eventSubject, eEventType.EVENT_ACTION_SET_LAST_MELEE_ATTACK_KNOCKBACK, mOwner, eActionCommand.None, data.value, paramBOSet.BOEffectIndex);
                break;

            case eBOFuncType.CommandAction:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue; // AniEvent.atkRatio 
                data.value2 = (paramBOSet.BOFuncValue2 / (float)eCOUNT.MAX_BO_FUNC_VALUE);          // 해당 액션 효과 % 값 2
                data.value3 = paramBOSet.BOFuncValue3;                                              // 해당 액션 효과 값 3

                data.evt = new ActionEvent(data);
                ((ActionEvent)data.evt).Set(eventSubject, eEventType.EVENT_ACTION_COMMAND_FROM_BATTLE_OPTION, mOwner, mActionCommand, data.value, paramBOSet.BOEffectIndex);
                break;

            case eBOFuncType.CommandActionCustom1:
                data.value = paramBOSet.BOFuncValue2;                                                   
                data.duration = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1));     
                data.originalDuration = data.duration;

                data.evt = new ActionEvent(data);
                ((ActionEvent)data.evt).Set(eventSubject, eEventType.EVENT_ACTION_COMMAND_FROM_BATTLE_OPTION, mOwner, mActionCommand, data.value, paramBOSet.BOEffectIndex);
                break;

            case eBOFuncType.Attack:
			case eBOFuncType.AttackCritical:
				data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue; // atkRatio
                data.value2 = paramBOSet.BOFuncValue2;
                data.value3 = paramBOSet.BOFuncValue3; // eBOTargetType.ActiveEnemiesInRange인 경우 인원

				selectEventType = eEventType.EVENT_ACTION_HIT_ATTACK;
				if(funcType == eBOFuncType.AttackCritical)
				{
					selectEventType = eEventType.EVENT_ACTION_HIT_ATTACK_CRITICAL;
				}

				data.evt = new ActionEvent(data);
                ((ActionEvent)data.evt).Set(eventSubject, selectEventType, mOwner, mActionCommand, data.value2, paramBOSet.BOEffectIndex);
                break;

            case eBOFuncType.UpperAttack:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue; // atkRatio
                data.value3 = paramBOSet.BOFuncValue3; // eBOTargetType.ActiveEnemiesInRange인 경우 인원

                data.evt = new ActionEvent(data);
                ((ActionEvent)data.evt).Set(eventSubject, eEventType.EVENT_ACTION_HIT_UPPER_ATTACK, mOwner, mActionCommand, data.value2, paramBOSet.BOEffectIndex);
                break;

            case eBOFuncType.KnockBackAttack:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue; // atkRatio
                data.value3 = paramBOSet.BOFuncValue3; // eBOTargetType.ActiveEnemiesInRange인 경우 인원

                data.evt = new ActionEvent(data);
                ((ActionEvent)data.evt).Set(eventSubject, eEventType.EVENT_ACTION_HIT_KNOCKBACK_ATTACK, mOwner, mActionCommand, data.value2, paramBOSet.BOEffectIndex);
                break;

            case eBOFuncType.DownAttack:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue; // atkRatio
                data.value3 = paramBOSet.BOFuncValue3; // eBOTargetType.ActiveEnemiesInRange인 경우 인원

                data.evt = new ActionEvent(data);
                ((ActionEvent)data.evt).Set(eventSubject, eEventType.EVENT_ACTION_HIT_DOWN_ATTACK, mOwner, mActionCommand, data.value2, paramBOSet.BOEffectIndex);
                break;

            case eBOFuncType.StunAttack:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue; // atkRatio
                data.value3 = paramBOSet.BOFuncValue3; // eBOTargetType.ActiveEnemiesInRange인 경우 인원

                data.evt = new ActionEvent(data);
                ((ActionEvent)data.evt).Set(eventSubject, eEventType.EVENT_ACTION_HIT_STUN_ATTACK, mOwner, mActionCommand, data.value2, paramBOSet.BOEffectIndex);
                break;

            case eBOFuncType.GroggyAttack:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue; // atkRatio
                data.value3 = paramBOSet.BOFuncValue3; // eBOTargetType.ActiveEnemiesInRange인 경우 인원

                data.evt = new ActionEvent(data);
                ((ActionEvent)data.evt).Set(eventSubject, eEventType.EVENT_ACTION_HIT_GROGGY_ATTACK, mOwner, mActionCommand, data.value2, paramBOSet.BOEffectIndex);
                break;

            case eBOFuncType.AttackRepeat:
                data.value = ( paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE ) + incValue; // 공격력
                data.value2 = paramBOSet.BOFuncValue2;  
                data.value3 = paramBOSet.BOFuncValue3; // eBOTargetType.ActiveEnemiesInRange인 경우 인원

                selectEventType = eEventType.EVENT_ACTION_HIT_ATTACK_REPEAT;

                data.evt = new ActionEvent( data );
                ( (ActionEvent)data.evt ).Set( eventSubject, selectEventType, mOwner, mActionCommand, data.value2, paramBOSet.BOEffectIndex );
                break;

            case eBOFuncType.FireProjectile:
			case eBOFuncType.FireProjectileRegardlessTargetCount:
			case eBOFuncType.FireProjectileRepeat:
				data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue; // AniEvent.atkRatio 
                data.value2 = paramBOSet.BOFuncValue2 == -1 ? World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup : paramBOSet.BOFuncValue2;
				data.value3 = paramBOSet.BOFuncValue3;

				List<Projectile> listProjectile = new List<Projectile>();
                for (int i = 0; i < data.value2 + Utility.ADDED_PROJECTILE_COUNT; i++)
                {
                    Projectile projectile = GameSupport.CreateProjectile("Projectile/" + data.projectileName + ".prefab");
                    listProjectile.Add(projectile);
                }

                data.evt = new ProjectileEvent(data);

				selectEventType = eEventType.EVENT_ACTION_FIRE_PROJECTILE;
				if (funcType == eBOFuncType.FireProjectileRegardlessTargetCount)
				{
					selectEventType = eEventType.EVENT_ACTION_FIRE_PROJECTILE_REGARDLESS_TARGET_COUNT;
				}
				else if(funcType == eBOFuncType.FireProjectileRepeat)
				{
					selectEventType = eEventType.EVENT_ACTION_FIRE_PROJECTILE_REPEAT;
				}

                ((ProjectileEvent)data.evt).Set(eventSubject, selectEventType, mOwner, listProjectile, data.value, data.value2, data.value3, data.duration, data.tick);
                break;


            //=================================================================================
            // 버프/디버프 스탯 또는 버프/디버프 (플레이어 전용 효과)
            //=================================================================================

            // 쉴드에 주는 피해 증가
            case eBOFuncType.AtkPowerRateUpToShield:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_BUFF_ATKPOWER_TO_SHIELD_RATE_UP;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_ATKPOWER_TO_SHIELD_RATE_UP;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 쉴드에 주는 피해 감소
            case eBOFuncType.AtkPowerRateDownToShield:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_DEBUFF_ATKPOWER_TO_SHIELD_RATE_DOWN;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_ATKPOWER_TO_SHIELD_RATE_DOWN;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 오의로 주는 피해 증가
            case eBOFuncType.UltimateSkillAtkPowerRateUp:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_BUFF_ULTIMATESKILL_ATKPOWER_RATE_UP;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_ULTIMATESKILL_ATKPOWER_RATE_UP;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 오의로 주는 피해 감소
            case eBOFuncType.UltimateSkillAtkPowerRateDown:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_DEBUFF_ULTIMATESKILL_ATKPOWER_RATE_DOWN;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_ULTIMATESKILL_ATKPOWER_RATE_DOWN;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;


            //=================================================================================
            // 버프/디버프 스탯 또는 버프/디버프 (유닛 공통)
            //=================================================================================

            // 크리티컬 확률 증가
            case eBOFuncType.CriRateUp:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_BUFF_CRITICAL_RATE_UP;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_CRITICAL_RATE_UP;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 크리티컬 확률 감소
            case eBOFuncType.CriRateDown:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_DEBUFF_CRITICAL_RATE_DOWN;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_CRITICAL_RATE_DOWN;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

			// 활성화된 적의 수에 따라 크리 확률 증감
			case eBOFuncType.CriRateUpWithEnemyCountRatio:
			case eBOFuncType.CriRateDownWithEnemyCountRatio:
				data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

				selectEventType = eEventType.EVENT_STAT_CRITICAL_RATE_UP_WITH_ACTIVE_ENEMY_COUNT_RATIO;
				if(funcType == eBOFuncType.CriRateDownWithEnemyCountRatio)
				{
					selectEventType = eEventType.EVENT_STAT_CRITICAL_RATE_DOWN_WITH_ACTIVE_ENEMY_COUNT_RATIO;
				}

				data.evt = new BuffEvent(data);
				((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
										  data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
				break;

			// 크리티컬 데미지 증가
			case eBOFuncType.CriDmgUp:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_BUFF_CRITICAL_DMG_UP;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_CRITICAL_DMG_UP;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 크리티컬 데미지 감소
            case eBOFuncType.CriDmgDown:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_DEBUFF_CRITICAL_DMG_DOWN;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_CRITICAL_DMG_DOWN;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 크리티컬 저항 증가
            case eBOFuncType.CriResistUp:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_BUFF_CRITICAL_RESIST_UP;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_CRITICAL_RESIST_UP;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 크리티컬 저항 감소
            case eBOFuncType.CriResistDown:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_DEBUFF_CRITICAL_RESIST_DOWN;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_CRITICAL_RESIST_DOWN;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 크리티컬 방어력 증가
            case eBOFuncType.CriDefUp:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_BUFF_CRITICAL_DEF_UP;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_CRITICAL_DEF_UP;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 크리티컬 방어력 감소
            case eBOFuncType.CriDefDown:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_DEBUFF_CRITICAL_DEF_DOWN;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_CRITICAL_DEF_DOWN;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 속도 증가
            case eBOFuncType.SpeedUp:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_BUFF_SPEED_UP;
				if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
				{
					selectEventType = eEventType.EVENT_STAT_SPEED_UP;
				}

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 이펙트 위에 있을 때 속도 증가
            case eBOFuncType.SpeedUpOnEffect:
                data.value = ( paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE ) + incValue;

                data.evt = new BuffEvent( data );
                ( (BuffEvent)data.evt ).Set( data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_SPEED_UP_ON_EFFECT, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType );
                break;

            // 속도 감소
            case eBOFuncType.SpeedDown:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
                data.value3 = paramBOSet.BOFuncValue3;

                selectEventType = eEventType.EVENT_DEBUFF_SPEED_DOWN;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_SPEED_DOWN;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 일정 시간 마다 속도 감소
            case eBOFuncType.SpeedDownToTarget:
                data.value = paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE;  // 속도 감소량
                data.value2 = paramBOSet.BOFuncValue2;                                  // 지속 시간
                data.value3 = paramBOSet.BOFuncValue3;                                  // 적용 범위

                selectEventType = eEventType.EVENT_DEBUFF_SPEED_DOWN_TO_TARGET;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 받는 피해 감소
            case eBOFuncType.DmgRateDown:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
                data.value2 = paramBOSet.BOFuncValue2;

                selectEventType = eEventType.EVENT_BUFF_DMG_RATE_DOWN;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_DMG_RATE_DOWN;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

			// 활성화된 적의 수에 따라 받는 피해 증감
			case eBOFuncType.DmgRateUpWithEnemyCountRatio:
			case eBOFuncType.DmgRateDownWithEnemyCountRatio:
                data.value = Mathf.Max(0.0f, (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue);

				if (funcType == eBOFuncType.DmgRateUpWithEnemyCountRatio)
				{
					selectEventType = eEventType.EVENT_STAT_DMG_RATE_UP_WITH_ACTIVE_ENEMY_COUNT_RATIO;
				}
				else
				{
					selectEventType = eEventType.EVENT_STAT_DMG_RATE_DOWN_WITH_ACTIVE_ENEMY_COUNT_RATIO;
				}

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 받는 피해를 감소하고 감소된 피해를 누적
            case eBOFuncType.DmgRateDownWithAccumDamage:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_BUFF_DMG_RATE_DOWN_WITH_ACCUM_DAMAGE;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

			// 버프 개수에 따른 받는 대미지 감소
			case eBOFuncType.DmgRateDownByBuffCount:
				data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

				//selectEventType = eEventType.EVENT_BUFF_DMG_RATE_DOWN;
				if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
				{
					selectEventType = eEventType.EVENT_STAT_DMG_RATE_DOWN_BY_BUFF_COUNT;
				}

				data.evt = new BuffEvent(data);
				((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
										  data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
				break;

			// 받는 피해 증가
			case eBOFuncType.DmgRateUp:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_DEBUFF_DMG_RATE_UP;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_DMG_RATE_UP;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 주는 피해 증가
            case eBOFuncType.AtkPowerRateUp:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_BUFF_ATKPOWER_RATE_UP;
                if ((timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f) && data.repeat == false)
                {
                    selectEventType = eEventType.EVENT_STAT_ATKPOWER_RATE_UP;
                }

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 한번만 주는 피해 증가
            case eBOFuncType.AtkPowerRateUpOnce:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_STAT_ATKPOWER_RATE_UP_ONCE;
                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 주는 피해 증가 (BOFuncValue를 시간으로 사용. 레벨에 따라 시간 증가)
            case eBOFuncType.AtkPowerRateUpTimeInc:
                data.duration = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1));
                data.value = paramBOSet.BOFuncValue2 / (float)eCOUNT.MAX_BO_FUNC_VALUE;

                selectEventType = eEventType.EVENT_BUFF_ATKPOWER_RATE_UP;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

			// 버프 개수에 따른 주는 피해 증가
			case eBOFuncType.AtkPowerRateUpByBuffCount:
				data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

				//selectEventType = eEventType.EVENT_BUFF_ATKPOWER_RATE_UP;
				if ((timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f) && data.repeat == false)
				{
					selectEventType = eEventType.EVENT_STAT_ATKPOWER_RATE_UP_BY_BUFF_COUNT;
				}

				data.evt = new BuffEvent(data);
				((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
										  data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
				break;

            // 적의 특정 디버프 중첩에 따른 주는 피해 증가 
            case eBOFuncType.AtkPowerRateUpByEnemyDebuffStack:
                data.value = ( paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE ) + incValue;
                data.value2 = paramBOSet.BOFuncValue2;

                selectEventType = eEventType.EVENT_STAT_ATKPOWER_RATE_UP_BY_ENEMY_DEBUFF_STACK;

                data.evt = new BuffEvent( data );
                ( (BuffEvent)data.evt ).Set( data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType );
                break;

            // 특정 시간 마다 주는 피해 계속 증가
            case eBOFuncType.AtkPowerRateUpInc:
                data.value = ( paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE ) + incValue;
                data.value2 = paramBOSet.BOFuncValue2; // 최대 중첩 횟수

                data.evt = new BuffEvent( data );
                ( (BuffEvent)data.evt ).Set( data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_ATKPOWER_RATE_UP, mOwner, 
                    data.value, data.value2, data.value3, data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, 
                    (eBuffIconType)paramBO.BOBuffIconType );
                break;

            // 활성화된 적의 수에 따라 주는 피해 증가
            case eBOFuncType.AtkPowerRateUpWithEnemyCountRatio:
                data.value = ( paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE ) + incValue;

                selectEventType = eEventType.EVENT_STAT_ATKPOWER_RATE_UP_WITH_ACTIVE_ENEMY_COUNT_RATIO;

                data.evt = new BuffEvent( data );
                ( (BuffEvent)data.evt ).Set( data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType );
                break;

            // 주는 피해 감소
            case eBOFuncType.AtkPowerRateDown:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_DEBUFF_ATKPOWER_RATE_DOWN;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_ATKPOWER_RATE_DOWN;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 활성화된 적의 수에 따라 주는 피해 감소
            case eBOFuncType.AtkPowerRateDownWithEnemyCountRatio:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_STAT_ATKPOWER_RATE_DOWN_WITH_ACTIVE_ENEMY_COUNT_RATIO;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 회복 능력 증가
            case eBOFuncType.HealRateUp:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_BUFF_HEAL_RATE_UP;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_HEAL_RATE_UP;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 회복 능력 감소
            case eBOFuncType.HealRateDown:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_DEBUFF_HEAL_RATE_DOWN;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_HEAL_RATE_DOWN;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 디버프 유지 시간 감소
            case eBOFuncType.DebuffTimeReduce:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_BUFF_TIME_DEBUFF_REDUCE;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_TIME_DEBUFF_REDUCE;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 디버프 유지 시간 증가
            case eBOFuncType.DebuffTimePromote:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_DEBUFF_TIME_DEBUFF_PROMOTE;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_TIME_DEBUFF_PROMOTE;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 공격 적중 시 SP 회복량 증가
            case eBOFuncType.IncreaseSPAddRate:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_BUFF_SP_ADD_INC_RATE;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_SP_ADD_INC_RATE;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 공격 적중 시 SP 회복량 감소
            case eBOFuncType.DecreaseSPAddRate:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                selectEventType = eEventType.EVENT_DEBUFF_SP_ADD_DEC_RATE;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_SP_ADD_DEC_RATE;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 공격 적중 시 SP 회복량 증가 (시간 증가형)
            case eBOFuncType.IncreaseSPAddRateTimeInc:
                data.value = (paramBOSet.BOFuncValue2 / (float)eCOUNT.MAX_BO_FUNC_VALUE);
                data.duration = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1));

                selectEventType = eEventType.EVENT_BUFF_SP_ADD_INC_RATE;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_SP_ADD_INC_RATE;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 공격 적중 시 SP 회복량 감소 (시간 증가형)
            case eBOFuncType.DecreaseSPAddRateTimeInc:
                data.value = (paramBOSet.BOFuncValue2 / (float)eCOUNT.MAX_BO_FUNC_VALUE);
                data.duration = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1));

                selectEventType = eEventType.EVENT_DEBUFF_SP_ADD_DEC_RATE;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f)
                    selectEventType = eEventType.EVENT_STAT_SP_ADD_DEC_RATE;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 관통 증가
            case eBOFuncType.PenetrateUp:
                data.value = ( paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE ) + incValue;

                selectEventType = eEventType.EVENT_BUFF_PENETRATE_UP;
                if( ( timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f ) && data.repeat == false ) {
                    selectEventType = eEventType.EVENT_STAT_PENETRATE_UP;
                }

                data.evt = new BuffEvent( data );
                ( (BuffEvent)data.evt ).Set( data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType );
                break;

            // 인내 증가
            case eBOFuncType.SufferanceUp:
                data.value = ( paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE ) + incValue;

                selectEventType = eEventType.EVENT_BUFF_SUFFERANCE_UP;
                if( ( timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP || data.duration == 0.0f ) && data.repeat == false ) {
                    selectEventType = eEventType.EVENT_STAT_SUFFERANCE_UP;
                }

                data.evt = new BuffEvent( data );
                ( (BuffEvent)data.evt ).Set( data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType );
                break;


            //=================================================================================
            // 버프/디버프
            //=================================================================================

            // 도트 데미지
            case eBOFuncType.DotDmg:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
                data.value2 = paramBOSet.BOFuncValue2; // 도트 속성(EAttackAttr)
                data.value3 = paramBOSet.BOFuncValue3;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_DEBUFF_DOT_DMG, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 타겟에 도트 데미지를 보냄
            case eBOFuncType.SendDotDmgToTarget:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue; // 공격력 
                data.value2 = paramBOSet.BOFuncValue2;                                              // Range
                data.value3 = paramBOSet.BOFuncValue3;                                              // Tick
                data.targetValue = paramBO.BOTarValue;                                              // 인원 수

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_DOT_DMG_TO_TARGET, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 타겟에 공격 보냄
            case eBOFuncType.SendAttackToTarget:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue; // 공격력 
                data.value2 = paramBOSet.BOFuncValue2;                                              // Range
                data.value3 = paramBOSet.BOFuncValue3;                                              // Tick
                data.targetValue = paramBO.BOTarValue;                                              // 인원 수

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_ATTACK_TO_TARGET, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 흡혈
            case eBOFuncType.BloodSucking:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_BLOODSUCKING, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 도트 회복 (최대 HP 비율로 회복)
            case eBOFuncType.DotHeal:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_DOT_HEAL, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 즉시 회복 (최대 HP 비율로 회복)
            case eBOFuncType.Heal:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
                data.value3 = paramBOSet.BOFuncValue3; // 죽었을 때 회복이 가능한지 여부

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_HEAL, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 일정 시간 내에 죽었을 때 즉시 회복 (최대 HP 비율)
            case eBOFuncType.HealOnDieInTime:
                data.value = ( paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE ) + incValue;
                data.value3 = paramBOSet.BOFuncValue3; // 죽었을 때 회복이 가능한지 여부

                data.evt = new BuffEvent( data );
                ( (BuffEvent)data.evt ).Set( data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_HEAL_ON_DIE_IN_TIME, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType );
                break;

            // 도트 회복 (절대 수치로 회복)
            case eBOFuncType.DotHealAbs:
                data.value = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1));

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_DOT_HEAL_ABS, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 즉시 회복 (절대 수치로 회복)
            case eBOFuncType.HealAbs:
                data.value = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1));

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_HEAL_ABS, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 도트 회복 (HP 소모량의 일정 비율 회복)
            case eBOFuncType.DotHealCostHP:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_DOT_HEAL_COST_HP, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 즉시 회복 (HP 소모량의 일정 비율 회복)
            case eBOFuncType.HealCostHP:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_HEAL_COST_HP, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // HP 조건 영구 도트 회복 (HP 소모량의 일정 비율 회복)
            case eBOFuncType.DotHealCostHPCondiHP:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
                data.value2 = (paramBOSet.BOFuncValue2 / (float)eCOUNT.MAX_BO_FUNC_VALUE);  // HP 최소 비율 조건 (0.5 -> 50% 이상인 경우에만 발동)

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_DOT_HEAL_COST_HP_CONDI, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 즉시 SP 회복 (절대 수치)
            case eBOFuncType.AddSP:
                data.value = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1));

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_ADD_SP, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 즉시 SP 회복 (퍼센트로 표현, 스킬 설명 문구 표현 시 %로 표현하기 위한 조치)
            case eBOFuncType.AddSPPercentage:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
                data.value *= GameInfo.Instance.BattleConfig.USMaxSP;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_ADD_SP, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // SP 지속 감소 (절대 수치)
            case eBOFuncType.DecreaseSP:
                data.value = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1));

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_DEBUFF_DECREASE_SP, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // SP 지속 감소 (퍼센트로 표현, 스킬 설명 문구 표현 시 %로 표현하기 위한 조치)
            case eBOFuncType.DecreaseSPPercentage:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
                data.value *= GameInfo.Instance.BattleConfig.USMaxSP;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_DEBUFF_DECREASE_SP, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // SP 강제 사용
            case eBOFuncType.UseSPPercentage: {
                data.value = ( paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE ) + incValue;
                data.value *= GameInfo.Instance.BattleConfig.USMaxSP;

                data.evt = new BuffEvent( data );
                ( (BuffEvent)data.evt ).Set( data.battleOptionSetId, eventSubject, eEventType.EVENT_DEBUFF_USE_SP, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType );
            }
            break;

            // 쉴드 데미지 감소
            case eBOFuncType.DotShieldDmg:
                data.value = mOwner.attackPower * ((paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue);

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_DEBUFF_DOT_SHIELD_DMG, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 피해 반사 (공격자 단일 대상)
            case eBOFuncType.Reflection:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue; // 기본 데미지 배율
                data.value2 = paramBOSet.BOFuncValue2;                                              // 강한 반사 eBehaviour 타입
                data.value3 = paramBOSet.BOFuncValue3;                                              // 강한 반사 횟수

                selectEventType = eEventType.EVENT_BUFF_REFLECTION;
                if(data.timingType == eBOTimingType.OnHit)
                {
                    selectEventType = eEventType.EVENT_ACTION_REFLECTION;
                }

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 피해 반사 (범위)
            case eBOFuncType.ReflectionAround:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue; // 기본 데미지 배율
                data.value2 = paramBOSet.BOFuncValue2;                                              // 강한 반사 eBehaviour 타입
                data.value3 = paramBOSet.BOFuncValue3;                                              // 강한 반사 횟수

                selectEventType = eEventType.EVENT_BUFF_REFLECTION_AROUND;
                if (data.timingType == eBOTimingType.OnHit)
                {
                    selectEventType = eEventType.EVENT_ACTION_REFLECTION_AROUND; // 이건 뭐야?
                }

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 랜덤으로 넉백 공격
            case eBOFuncType.RandomKnockback:
                data.value = paramBOSet.BOFuncValue + incValue;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_RANDOM_KNOCKBACK, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 이동 제한 
            case eBOFuncType.HoldPosition:
                data.duration = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1)); // 유지 시간
                data.value2 = data.targetValue; // eBOTargetType.ActiveEnemiesInRange인 경우 반경

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_DEBUFF_HOLD_POSITION, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 타겟에 이동 제한을 건다
            case eBOFuncType.SendHoldPositionToTarget:
                data.value = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1)); // 이동 제한을 걸었을 때 유지 시간
                data.value2 = data.targetValue; // eBOTargetType.ActiveEnemiesInRange인 경우 반경
                data.value3 = paramBOSet.BOFuncValue3; // eBOTargetType.ActiveEnemiesInRange인 경우 인원

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_HOLD_POSITION, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 슈퍼아머
            case eBOFuncType.SuperArmor:
                data.value = paramBOSet.BOFuncValue;    // 슈퍼아머 레벨

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_SUPER_ARMOR, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 슈퍼아머 시간 증가형 (스킬 레벨에 의해 효과가 증가되어야 하는 무기, 서포터 스킬의 경우 효과가 무적이면 증가시켜줄 값은 시간뿐)
            case eBOFuncType.SuperArmorTimeInc:
                data.value = paramBOSet.BOFuncValue2;                                                   // 슈퍼아머 레벨
                data.duration = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1));     // 슈퍼아머 유지 시간
                data.originalDuration = data.duration;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_SUPER_ARMOR, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 조건부 슈퍼아머
            case eBOFuncType.ConditionalSuperArmor:
                data.value = paramBOSet.BOFuncValue;    // 조건 타입 (ECondtionSuperArmorType.GRADE[1], ECondtionSuperArmorType.MON_TYPE[2])
                data.value2 = paramBOSet.BOFuncValue2;  // 비교할 값 (eGrade:Normal,Epic,Boss / eMonType:Human,Machine,Devil)
                data.value3 = paramBOSet.BOFuncValue3;  // 슈퍼아머 값 (현재 피격 모션 여부에만 영향, eSuperArmor.Invincible[3]이여도 데미지 받음)

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_CONDITIONAL_SUPER_ARMOR, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 스킬에 의한 슈퍼 아머 지속 시간 증가
            case eBOFuncType.IncreaseSuperArmorDurationBySkill:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue; // 지속 시간 증가 배율

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_INCREASE_SUPER_ARMOR_BY_SKILL, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 크리티컬 없음
            case eBOFuncType.IgnoreCriRate:
                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_IGNORE_CRI_RATE, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 모든 디버프 제거
            case eBOFuncType.ClearDebuff:
                data.evt = new BuffEvent(data);

                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_CLEAR_DEBUFF, mOwner, data.value, data.value2, data.value3,
                                          data.duration + incValue, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 디버프 제거
            case eBOFuncType.RemoveDebuff:
                data.value = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1));

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_REMOVE_DEBUFF, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 디버프 및 행동 불능 제거
            case eBOFuncType.RemoveDebuffAndIncapable:
                data.value = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1));

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_REMOVE_DEBUFF_AND_INCAPABLE, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 모든 버프 제거
            case eBOFuncType.ClearBuff:
                data.evt = new BuffEvent(data);

                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_DEBUFF_CLEAR_BUFF, mOwner, data.value, data.value2, data.value3,
                                          data.duration + incValue, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 버프 제거
            case eBOFuncType.RemoveBuff:
                data.value = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1)); // 개수
                data.value2 = paramBOSet.BOFuncValue2;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_DEBUFF_REMOVE_BUFF, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 적들 공격 불가
            case eBOFuncType.SkipEnemyAttack:
                data.duration = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1)); // 유지 시간
                data.value2 = data.targetValue; // eBOTargetType.ActiveEnemiesInRange인 경우 반경

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_DEBUFF_SKIP_ATTACK, mOwner, data.value, data.value2, data.value3,
                                          data.duration + incValue, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 이동 반전
            case eBOFuncType.MovementInverse:
                data.evt = new BuffEvent(data);

                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_DEBUFF_MOVEMENT_INVERSE, mOwner, data.value, data.value2, data.value3,
                                          data.duration + incValue, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 최대 HP 증가
            case eBOFuncType.AddMaxHp:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_ADD_MAX_HP, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 빙결
            case eBOFuncType.Freeze:
                data.value = -1.0f;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_DEBUFF_FREEZE, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 클론에 공격2 명령
            case eBOFuncType.UseCloneAttack2:
                data.value = paramBOSet.BOFuncValue;    // 확률 (%)
                data.value2 = paramBOSet.BOFuncValue2;  // 간격 (초)
                data.value3 = paramBOSet.BOFuncValue3 / (float)eCOUNT.MAX_BO_FUNC_VALUE;  // 공격력 배율 

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_COMMAND_CLONE_ATTACK02, mOwner, 
                                          data.value, data.value2, data.value3, data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2,
                                          (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 마킹
            case eBOFuncType.Marking:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue; // 마킹 관련 기능에서 사용할 인자
                data.value2 = paramBOSet.BOFuncValue2; // 마킹 타입
                data.value3 = paramBOSet.BOFuncValue3 / (float)eCOUNT.MAX_BO_FUNC_VALUE; // 딜레이

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_DEBUFF_MARKING, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 서포터 스킬 쿨타임 정지 (적은 디버프 정지)
            case eBOFuncType.HoldSupprterSkillCoolTime:
                data.duration = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1)); // 유지 시간
                data.value2 = data.targetValue; // eBOTargetType.ActiveEnemiesInRange인 경우 반경

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_DEBUFF_HOLD_SUPPORTER_SKILL_COOLTIME, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 버프나 디버프의 Duration 값 증가/감소
            case eBOFuncType.AddBuffDebuffDuration:
                data.value = paramBOSet.BOFuncValue + ( paramBOSet.BOFuncIncValue * ( level - 1 ) );     // 증가/감소 값 (초)
                data.value2 = paramBOSet.BOFuncValue2;              // 버프/디버프 타입 (1이면 버프, 2면 디버프)
                data.value3 = paramBOSet.BOFuncValue3;              // 개수

                selectEventType = eEventType.EVENT_BUFF_ADD_BUFF_DEBUFF_DURATION;
                if (timingType == eBOTimingType.GameStart || timingType == eBOTimingType.MissionStart || timingType == eBOTimingType.MaxSP)
                {
                    selectEventType = eEventType.EVENT_STAT_ADD_BUFF_DEBUFF_DURATION;
                }

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

			// 감전 디버프
			case eBOFuncType.ElectricShock:
				data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
				data.value2 = paramBOSet.BOFuncValue2; // 도트 속성(EAttackAttr)
				data.value3 = paramBOSet.BOFuncValue3;

				data.evt = new BuffEvent(data);
				((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_DEBUFF_ELECTRIC_SHOCK, mOwner, data.value, data.value2, data.value3,
										  data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
				break;

			// 번개 공격
			case eBOFuncType.LightningAttack:
				data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
				data.value2 = paramBOSet.BOFuncValue2; // 범위

				data.evt = new BuffEvent(data);
				((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_LIGHTNING_ATTACK, mOwner, data.value, data.value2, data.value3,
										  data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
				break;

            // 화염 오오라
            case eBOFuncType.FireAura:
                data.value = ( paramBOSet.BOFuncValue / ( float )eCOUNT.MAX_BO_FUNC_VALUE ) + incValue; // 범위 안에 적이 받는 피해
                data.value2 = ( paramBOSet.BOFuncValue2 / ( float )eCOUNT.MAX_BO_FUNC_VALUE ) + incValue; // 범위 안에 적에게 주는 공격력 %
                data.value3 = paramBOSet.BOFuncValue3; // 범위

                data.evt = new BuffEvent( data );
                ( ( BuffEvent )data.evt ).Set( data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_FIRE_AURA, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, ( eBuffIconType )paramBO.BOBuffIconType );
                break;

            // 디버프 면역
            case eBOFuncType.SetDebuffImmune:
                data.value = paramBOSet.BOFuncValue; // 디버프 타입

                data.evt = new BuffEvent( data );
                ( (BuffEvent)data.evt ).Set( data.battleOptionSetId, eventSubject, eEventType.EVENT_BUFF_SET_DEBUFF_IMMUNE, mOwner, data.value, data.value2, data.value3,
                                             data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType );
                break;

            //=================================================================================
            // 기타 등등
            //=================================================================================

            // 아이템 드랍 확률 증가형
            case eBOFuncType.AddDropItemRate:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.value = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1));    // 드랍 확률 (레벨에 따라 증가)
                data.evt.value2 = paramBOSet.BOFuncValue2;                                              // 드랍 아이템 ID
                data.evt.value3 = paramBOSet.BOFuncValue3;                                              // 드랍 아이템 개수
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_ITEM_DROP;
                break;

            // 아이템 드랍 개수 증가형
            case eBOFuncType.AddDropItemCnt:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.value = paramBOSet.BOFuncValue3;                                               // 드랍 확률
                data.evt.value2 = paramBOSet.BOFuncValue2;                                              // 드랍 아이템 ID
                data.evt.value3 = paramBOSet.BOFuncValue + (paramBOSet.BOFuncIncValue * (level - 1));   // 드랍 아이템 개수 (레벨에 따라 증가)
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_ITEM_DROP;
                break;

            // 추가 공격
            case eBOFuncType.AddAttack:
                data.evt = new BaseEvent();
                data.evt.SetBattleOptionData(data);

                data.evt.sender = mOwner;
                data.evt.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
                data.evt.value2 = paramBOSet.BOFuncValue2;
                data.evt.value3 = paramBOSet.BOFuncValue3;
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_ATTACK_ADD;
                break;

            // SP를 사용한 추가 공격
            case eBOFuncType.AddSpAttack:
                data.evt = new BaseEvent();
                data.evt.SetBattleOptionData( data );

                data.evt.sender = mOwner;
                data.evt.value = ( paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE ) + incValue; // 공격력 비율
                data.evt.value2 = paramBOSet.BOFuncValue2; // 공격 횟수
                data.evt.value3 = paramBOSet.BOFuncValue3; // SP 소모량
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_SP_ATTACK_ADD;
                break;

            // 콤보 리셋 조건이 시간 제한 없고 피격 시로 바뀜
            case eBOFuncType.ComboResetByHit:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_COMBO_RESET_BY_HIT;
                break;

            // 특정 공격 속성을 가진 공격 무시
            case eBOFuncType.ImmuneAttackAttr:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.value = paramBOSet.BOFuncValue;
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_IMMUNE_ATKATTR;
                break;

            // 공격력 감소 면역
            case eBOFuncType.ImmuneAtkPowerRateDown:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_IMMUNE_ATKPOWER_RATE_DOWN;
                break;

            // 방어력 감소 면역
            case eBOFuncType.ImmuneDmgRateUp:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_IMMUNE_DMG_RATE_UP;
                break;

            // 속도 감소 면역
            case eBOFuncType.ImmuneSpeedDown:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_IMMUNE_SPEED_DOWN;
                break;

            // 서포터 스킬 쿨타임 감소
            case eBOFuncType.DecreaseSupporterCoolTime:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_SUPPORTER_COOL_TIME_DECREASE;
                break;

            // 게임 중 서포터 스킬 쿨타임 감소
            case eBOFuncType.DecreaseSupporterCoolTimeInRealTime:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_SUPPORTER_COOL_TIME_DECREASE_IN_REAL_TIME;
                break;

            // 캐릭터 스킬 쿨타임 감소
            case eBOFuncType.DecreaseSkillCoolTime:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_SKILL_COOL_TIME_DECREASE;
                break;

            case eBOFuncType.DecreaseActionSkillCoolTime:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.SetBattleOptionData(data);
                data.evt.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_SKILL_ACTION_COOL_TIME_DECREASE;
                break;

            case eBOFuncType.DecreaseActionSkillCoolTimeInRealTime:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.SetBattleOptionData(data);
                data.evt.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_SKILL_ACTION_COOL_TIME_DECREASE_IN_REAL_TIME;
                break;

            // SP를 소모하여 캐릭터 스킬 쿨타임 초기화
            case eBOFuncType.ResetActionSkillCoolTimeByUseSP:	
				data.evt = new BaseEvent();

				data.evt.sender = mOwner;
				data.evt.SetBattleOptionData(data);
				data.evt.value = paramBOSet.BOFuncValue + (incValue * (float)eCOUNT.MAX_BO_FUNC_VALUE); // 발동 확률 (백분율)
				data.evt.value2 = paramBOSet.BOFuncValue2;			// 소모 SP
				data.evt.eventSubject = eventSubject;
				data.evt.eventType = eEventType.EVENT_SKILL_RESET_ACTION_COOL_TIME_BY_USE_SP;
				break;

            case eBOFuncType.ResetAllSkillCoolTime:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.SetBattleOptionData( data );
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_SKILL_RESET_ALL_COOL_TIME;
                break;

            // 특정 SkillId(value2)의 스킬을 제외한 나머지 스킬들 중 하나의 재사용 시간을 리셋
            case eBOFuncType.ResetRandomSkillCoolTime: {
                data.value = paramBOSet.BOFuncValue + ( incValue * (float)eCOUNT.MAX_BO_FUNC_VALUE ); // 리셋 확률
                data.value2 = paramBOSet.BOFuncValue2; // 제외할 스킬 Id

                data.evt = new BuffEvent( data );
                ( ( BuffEvent )data.evt ).Set( data.battleOptionSetId, eventSubject, eEventType.EVENT_RANDOM_SKILL_RESET, mOwner, 
                                               data.value, data.value2, data.value3, data.duration, data.tick, 
                                               paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, ( eBuffIconType )paramBO.BOBuffIconType );
            }
            break;

            // 특정 액션에 쿨타임 감소(Set, 누적 아님)
            case eBOFuncType.DecreaseOneActionSkillCoolTimeSec:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.SetBattleOptionData(data);
                data.evt.value = paramBOSet.BOFuncValue + (incValue * (float)eCOUNT.MAX_BO_FUNC_VALUE);	// 초 단위
                data.evt.value2 = paramBOSet.BOFuncValue2;			// 액션 테이블 아이디
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_SKILL_ONE_ACTION_COOL_TIME_DECREASE_SEC;
                break;

            // 특정 액션에 누적으로 쿨타임 감소
            case eBOFuncType.DecreaseOnlyActionSkillCoolTimeSec:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.SetBattleOptionData( data );
                data.evt.value = paramBOSet.BOFuncValue + ( incValue * (float)eCOUNT.MAX_BO_FUNC_VALUE );	// 초 단위
                data.evt.value2 = paramBOSet.BOFuncValue2;  // 액션 테이블 아이디
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_SKILL_ONLY_ACTION_COOL_TIME_DECREASE_SEC;
                break;

            // SP 회복 속도 증가
            case eBOFuncType.IncreaseSPRegenRate:
                data.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;

				data.evt = new BuffEvent(data);
				((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_SP_REGEN_INC_RATE, mOwner, data.value, data.value2, data.value3,
										  data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
				break;

            // 특정 액션에 추가 액션 발동 여부 설정
            case eBOFuncType.SetAddAction:
                data.evt = new BaseEvent();
                data.evt.SetBattleOptionData(data);

                data.evt.sender = mOwner;
                data.evt.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
                data.evt.value2 = paramBOSet.BOFuncValue2;
                data.evt.value3 = paramBOSet.BOFuncValue3;
                data.evt.battleOptionData.duration = paramBOSet.BOBuffDurTime;
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_SET_ADD_ACTION;
                break;

            // 특정 액션에 AddActionValue 값 증가
            case eBOFuncType.IncreaseAddActionValue:
                data.evt = new BaseEvent();
                data.evt.SetBattleOptionData( data );

                data.evt.sender = mOwner;
                data.evt.value = ( paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE ) + incValue;  // 증가값
				data.evt.value2 =  paramBOSet.BOFuncValue2; // AddActionValue 인덱스(1~3)
				data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_INCREASE_ADD_ACTION_VALUE;
                break;

            // 빈 배틀옵션 (BOAddBOSetID 호출만을 위한 경우 등)
            case eBOFuncType.None:
                data.evt = new BaseEvent();
                data.evt.SetBattleOptionData(data);

                data.evt.sender = mOwner;
                data.evt.eventSubject = eEventSubject.None;
                data.evt.eventType = eEventType.EVENT_NONE;
                break;

            case eBOFuncType.AttackUsingAccumDamage:
                selectEventType = eEventType.EVENT_ATTACK_USING_ACCUM_DAMAGE;

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, selectEventType, mOwner, data.value, data.value2, data.value3,
                                          data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 소환수들 공격력 증가
            case eBOFuncType.IncreaseSummonsAttackPower:
                data.evt = new BaseEvent();

                data.evt.sender = mOwner;
                data.evt.value = (paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE) + incValue;
                data.evt.eventSubject = eventSubject;
                data.evt.eventType = eEventType.EVENT_SUMMONS_ATTACK_POWER_INCREASE;
                break;

            // 적 끌어당기기
            case eBOFuncType.Blackhole:
                data.value = paramBOSet.BOFuncValue; // 범위

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_BLACKHOLE, mOwner,
                                          data.value, data.value2, data.value3, data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2,
                                          (eBuffIconType)paramBO.BOBuffIconType);
                break;

            // 숨기
            case eBOFuncType.Hide:
                data.value = paramBOSet.BOFuncValue;    // 숨기했을 때 행동 옵션 (0 : 모든 행동 불가, 1 : 이동, 대쉬만 가능, 2 : 공격만 가능, 3 : 모두 가능)
                data.value2 = paramBOSet.BOFuncValue2;  // 숨기 끝났을 때 행동 옵션 (0 : 적에게 순간이동, 1 : 없음, 2 : 적 뒤로 순간이동)

                data.evt = new BuffEvent(data);
                ((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_HIDE, mOwner,
                                          data.value, data.value2, data.value3, data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2,
                                          (eBuffIconType)paramBO.BOBuffIconType);
                break;

			// 소환
			case eBOFuncType.SummonPlayerMinion:
				data.duration = paramBOSet.BOFuncValue + (incValue * (float)eCOUNT.MAX_BO_FUNC_VALUE);  // 소환 유지 시간
                data.value = paramBOSet.BOFuncValue2;                                                   // 어그로 값

				string[] ids = Utility.Split(paramBOSet.MinionIds, ','); //paramBOSet.MinionIds.Split(',');
				if (ids != null && ids.Length > 0)
				{
					data.MinionIds = new int[ids.Length];
				}

				for(int i = 0; i < ids.Length; i++)
				{
					data.MinionIds[i] = Utility.SafeIntParse(ids[i]);
					PlayerMinion minion = GameSupport.CreatePlayerMinion(data.MinionIds[i], mOwner);
				}

				data.evt = new BuffEvent(data);
				((BuffEvent)data.evt).Set(data.battleOptionSetId, eventSubject, eEventType.EVENT_SUMMON_PLAYER_MINION, mOwner,
										  data.value, data.value2, data.value3, data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2,
										  (eBuffIconType)paramBO.BOBuffIconType);
				break;

            // 적을 죽인 수에 따른 소환
            case eBOFuncType.SummonPlayerMinionByDieEnemyCount:
                data.value  = ( paramBOSet.BOFuncValue / (float)eCOUNT.MAX_BO_FUNC_VALUE ) + incValue;  // 공격력
                data.value2 = paramBOSet.BOFuncValue2;                                                  // 어그로 값
                data.value3 = paramBOSet.BOFuncValue3;                                                  // 소환할 수 있는 죽은 몬스터 수

                ids = Utility.Split(paramBOSet.MinionIds, ',');
                if( ids != null && ids.Length > 0 ) {
                    data.MinionIds = new int[ids.Length];
                }

                for( int i = 0; i < ids.Length; i++ ) {
                    data.MinionIds[i] = Utility.SafeIntParse( ids[i] );
                    PlayerMinion minion = GameSupport.CreatePlayerMinion(data.MinionIds[i], mOwner);
                }

                data.evt = new BuffEvent( data );
                ((BuffEvent)data.evt).Set( data.battleOptionSetId, eventSubject, eEventType.EVENT_SUMMON_PLAYER_MINION_BY_DIE_ENEMY_COUNT, mOwner, 
                    data.value, data.value2, data.value3, paramBOSet.BOBuffDurTime, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2,
                    (eBuffIconType)paramBO.BOBuffIconType );
                break;

            // 적 쉴드 즉시 해제
            case eBOFuncType.BreakShieldImmediate:
				data.evt = new BaseEvent();

				data.evt.sender = mOwner;
				data.evt.eventSubject = eventSubject;
				data.evt.eventType = eEventType.EVENT_DEBUFF_BREAK_SHIELD;
				break;

            // 밀어내기
            case eBOFuncType.Pushed:
                data.value = paramBOSet.BOFuncValue2; // 속도

                data.evt = new BuffEvent( data );
                ( (BuffEvent)data.evt ).Set( data.battleOptionSetId, eventSubject, eEventType.EVENT_PUSHED, mOwner,
                                          data.value, data.value2, data.value3, data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2,
                                          (eBuffIconType)paramBO.BOBuffIconType );
                break;

            // 즉사 스킬
            case eBOFuncType.Death:
                data.value = paramBOSet.BOFuncValue; // 확률

                data.evt = new BuffEvent( data );
                ( (BuffEvent)data.evt ).Set( data.battleOptionSetId, eventSubject, eEventType.EVENT_DEATH, mOwner,
                                          data.value, data.value2, data.value3, data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2,
                                          (eBuffIconType)paramBO.BOBuffIconType );
                break;

            // 플레이어에 추가 기능 설정
            case eBOFuncType.SetExtraPlayerFunc:
                data.evt = new BuffEvent( data );
                ( (BuffEvent)data.evt ).Set( data.battleOptionSetId, eventSubject, eEventType.EVENT_SET_EXTRA_PLAYER_FUNC, mOwner,
                                             data.value, data.value2, data.value3, data.duration, data.tick, 
                                             paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2, (eBuffIconType)paramBO.BOBuffIconType );
                break;

            // 플레이어 스킬 재사용 시간 증가
            case eBOFuncType.IncreaseCharSkillCoolTime:
                data.value = paramBOSet.BOFuncValue; // 초 단위

                data.evt = new BuffEvent( data );
                ( (BuffEvent)data.evt ).Set( data.battleOptionSetId, eventSubject, eEventType.EVENT_INCREASE_CHAR_SKILL_COOL_TIME, mOwner,
                                          data.value, data.value2, data.value3, data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2,
                                          (eBuffIconType)paramBO.BOBuffIconType );
                break;

            // 플레이어 서포터 스킬 재사용 시간 증가
            case eBOFuncType.IncreaseCharSupporterSkillCoolTime:
                data.value = paramBOSet.BOFuncValue; // 초 단위

                data.evt = new BuffEvent( data );
                ( (BuffEvent)data.evt ).Set( data.battleOptionSetId, eventSubject, eEventType.EVENT_INCREASE_CHAR_SUPPORTER_SKILL_COOL_TIME, mOwner,
                                          data.value, data.value2, data.value3, data.duration, data.tick, paramBOSet.BOEffectIndex, paramBOSet.BOEffectIndex2,
                                          (eBuffIconType)paramBO.BOBuffIconType );
                break;
        }

        World.Instance.AddBO(this);
        return data;
    }

	public void Execute( eBOTimingType timingType, int actionTableId = 0, Projectile projectile = null, bool skipRepeatBattleOption = false ) {
		int checkActionTableId = -1;

		List<sBattleOptionData> findAll = null;
		if ( timingType == eBOTimingType.StartSkill || timingType == eBOTimingType.OnSkillAttack || timingType == eBOTimingType.DuringSkill ) {// 스킬 시전, 스킬 적중 시, 스킬 사용 중 배틀옵션은 해당 스킬만 사용 가능 하도록 스킬 액션 TableId를 검사한다.

			if ( projectile && actionTableId < -1 && mOwner.actionSystem2 ) // 서포터 스킬은 TableId를 InstanceId로 사용하기 때문에 값이 -1보다 한참 적음.
			{
				ActionSupporterSkillBase actionSupporter = mOwner.actionSystem2.GetActionOrNullByTableId<ActionSupporterSkillBase>( actionTableId );
				if ( actionSupporter == null ) {
					return;
				}

				checkActionTableId = actionSupporter.TableId;
			}
			else {
				ActionSelectSkillBase action = mOwner.actionSystem.GetCurrentAction<ActionSelectSkillBase>();
				if ( action == null || ( action && actionTableId > 0 && action.TableId != actionTableId ) ) {
					if ( projectile ) {
						action = mOwner.actionSystem.GetActionOrNullByTableId<ActionSelectSkillBase>( actionTableId );
						if ( action == null ) {
							return;
						}

						checkActionTableId = action.TableId;
					}
					else {
						bool isFind = false;
						Player ownerPlayer = mOwner as Player;
						if ( ownerPlayer != null && ownerPlayer.Guardian != null ) {
							ActionGuardianBase actionGuardianBase = ownerPlayer.Guardian.actionSystem.GetCurrentAction<ActionGuardianBase>();
							if ( actionGuardianBase != null ) {
								checkActionTableId = actionGuardianBase.TableId;
								isFind = true;
							}
						}

						if ( isFind == false ) {
							return;
						}
					}
				}
				else if ( action && action.TableId == actionTableId ) {
					checkActionTableId = action.TableId;
				}
			}

			findAll = ListBattleOptionData.FindAll( x => x.timingType == timingType && actionTableId == checkActionTableId );
			if ( findAll == null || findAll.Count <= 0 ) {
				return;
			}
		}
		else {
			findAll = ListBattleOptionData.FindAll( x => x.timingType == timingType );
			if ( findAll == null || findAll.Count <= 0 ) {
				return;
			}
		}

		for ( int i = 0; i < findAll.Count; i++ ) {
			if ( skipRepeatBattleOption && findAll[i].repeat ) {
				continue;
			}

			if ( timingType == eBOTimingType.OnNormalAttack ) {
				if ( findAll[i].preConditionType != eBOConditionType.None && findAll[i].evt != null ) {
					if ( !GameSupport.IsBOConditionCheck( findAll[i].preConditionType, findAll[i].CondValue, findAll[i].evt.sender, null, -1, projectile ) ) {
						continue;
					}
				}
			}

			if ( ( timingType != eBOTimingType.GameStart && timingType != eBOTimingType.MissionStart ) && findAll[i].randomStart > -1 ) {
				if ( UnityEngine.Random.Range( 0, 100 ) >= findAll[i].randomStart )
					continue;
			}

			if ( findAll[i].buffDebuffType == eBuffDebuffType.Debuff && ( mOwner as Enemy ) && mOwner.HoldSupporterSkillCoolTime ) {
				continue;
			}

			if ( findAll[i].startCondtionType != eBOConditionType.None && findAll[i].evt != null ) {
				if ( !GameSupport.IsBOConditionCheck( findAll[i].startCondtionType, findAll[i].CondValue, findAll[i].evt.sender ) ) {
					continue;
				}
			}

			findAll[i].CheckActionTableId = checkActionTableId;
			findAll[i].Pjt = projectile;

			if ( mOwner && findAll[i].startDelay > 0 ) {
				mOwner.StartCoroutine( SendEvent( timingType, findAll[i] ) );
			}
			else {
				DoSendEvent( timingType, findAll[i] );
			}
		}
	}

	private IEnumerator SendEvent(eBOTimingType timingType, sBattleOptionData boData)
    {
        if (boData.noDelayStartEffId > 0)
        {
            if (boData.evt.sender)
            {
                EffectManager.Instance.Play(boData.evt.sender, boData.noDelayStartEffId, (EffectManager.eType)boData.effType);
            }
            else //sender가 없으면(아이템 획득에 따른 배틀 옵션 호출, class BOItem) 플레이어로 이펙트 출력
            {
                EffectManager.Instance.Play(World.Instance.Player, boData.noDelayStartEffId, (EffectManager.eType)boData.effType);
            }
        }

        yield return new WaitForSeconds(boData.startDelay);
        DoSendEvent(timingType, boData);
    }

    private void DoSendEvent(eBOTimingType timingType, sBattleOptionData boData)
    {
		TimeSpan tsPastTime = TimeSpan.FromTicks(DateTime.Now.Ticks - boData.useTime.Ticks);
        Log.Show(boData.battleOptionSetId + "번 배틀옵션 사용 텀은 " + tsPastTime.TotalSeconds + "초", Log.ColorType.Red);

        if(boData.CoolTime > -1.0f && boData.CheckCoolTime < boData.CoolTime) //tsPastTime.TotalSeconds <= boData.CoolTime)
        {
            return;
        }

        if( boData.conditionType == eBOConditionType.CheckSp ) {
            if( !GameSupport.IsBOConditionCheck( eBOConditionType.CheckSp, boData.CondValue, mOwner ) ) {
                return;
			}
		}

        if(boData.CoolTime > -1.0f)
        {
            boData.CheckCoolTime = 0.0f;

            if (boData.CrCoolTime == null)
            {
                boData.CrCoolTime = World.Instance.StartCoroutine(boData.UpdateCoolTime());
            }
        }

        if (boData.startEffId > 0)
        {
            if (boData.evt.sender)
                EffectManager.Instance.Play(boData.evt.sender, boData.startEffId, (EffectManager.eType)boData.effType);
            else //sender가 없으면(아이템 획득에 따른 배틀 옵션 호출, class BOItem) 플레이어로 이펙트 출력
                EffectManager.Instance.Play(World.Instance.Player, boData.startEffId, (EffectManager.eType)boData.effType);
        }

        if (timingType == eBOTimingType.Use && boData.conditionType == eBOConditionType.ComboCountAsValue)
        {
            boData.evt.value = Mathf.Min(boData.targetValue, (float)mOwner.comboCount) * boData.value;
            boData.evt.sender.ClearCombo();
        }

        boData.useTime = DateTime.Now;
        EventMgr.Instance.SendEvent(boData.evt);
        Log.Show(boData.evt.battleOptionData.battleOptionSetId + "번 배틀옵션셋 사용!!!", Log.ColorType.Green);

        if (boData.dataOnEndCall != null)
        {
            if (boData.addCallTiming == eBOAddCallTiming.OnStart)
            {
                if (timingType == eBOTimingType.Use && boData.dataOnEndCall.conditionType == eBOConditionType.ComboCountAsValue)
                    boData.dataOnEndCall.evt.value = boData.evt.value;

				if (boData.evt != null && boData.dataOnEndCall.evt != null)
				{
					boData.dataOnEndCall.evt.sender = boData.evt.sender;
				}

				if (boData.dataOnEndCall.startDelay > 0.0f)
                {
                    World.Instance.StartCoroutine(DelayedAddCall(boData.dataOnEndCall));
                }
                else
                {
                    boData.dataOnEndCall.useTime = DateTime.Now;
                    EventMgr.Instance.SendEvent(boData.dataOnEndCall.evt);
                    Log.Show(boData.dataOnEndCall.evt.battleOptionData.battleOptionSetId + "번 배틀옵션셋 사용 (애드콜)!!!", Log.ColorType.Green);
                }
            }
        }

        if(boData.useOnce)
        {
            FSaveData.Instance.AddUsedBOSetId(boData.battleOptionSetId);
            Utility.StopCoroutine(World.Instance, ref boData.CrCoolTime);

            ListBattleOptionData.Remove(boData);
        }
    }

    private IEnumerator DelayedAddCall(sBattleOptionData data)
    {
        float checkTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (checkTime < data.startDelay)
        {
            checkTime += Time.fixedDeltaTime;
            yield return mWaitForFixedUpdate;
        }

        //yield return new WaitForSeconds(data.startDelay);

        data.useTime = DateTime.Now;
        EventMgr.Instance.SendEvent(data.evt);
        Log.Show(data.evt.battleOptionData.battleOptionSetId + "번 배틀옵션셋 사용 (딜레이드애드콜)!!!", Log.ColorType.Green);
    }

    public sBattleOptionData GetBattleOptionInfo(int id)
    {
        return ListBattleOptionData.Find(x => x.battleOptionSetId == id);
    }

    public sBattleOptionData GetBattleOptionInfo(eBOTimingType timingType)
    {
        return ListBattleOptionData.Find(x => x.timingType == timingType);
    }

    public sBattleOptionData GetBattleOptionInfo(eBOConditionType conditionType)
    {
        return ListBattleOptionData.Find(x => x.conditionType == conditionType);
    }

    /*
    public void UpdateCheckCoolTime()
    {
        if (World.Instance.IsPause)
        {
            return;
        }

        mCheckCoolTime += Time.fixedDeltaTime;
    }
    */
}
