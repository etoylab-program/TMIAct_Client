
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CmptBuffDebuff : CmptBase
{
    public class sExecute
    {
        public BuffEvent    buffEvt;
        public float        value;
        public float        checkDuration;
        public float        checkTick;
        public int          curStackCount;
        public bool         changeStackCount;
        public float        repeatDelay;
        public float        AddDuration;

        public ActionSelectSkillBase StartOwnerAction = null;

        public Coroutine                        cr          = null;
        public Coroutine                        crRepeat    = null;
        public Action                           OnEnd       = null;
        public Func<int, bool>                  OnRemove    = null;
        public UIBuffDebuffIcon.sActiveIconInfo uiIconInfo  = null;

        public uint ChangedSuperArmorId = 0;


        public sExecute(Unit owner, BuffEvent buffEvt, Action callback, UIBuffDebuffIcon.sActiveIconInfo uiIconInfo)
        {
            this.buffEvt = buffEvt;
            this.uiIconInfo = uiIconInfo;

            value = buffEvt.value;

            float debuffTimeReduce = 0.0f;
            if (buffEvt.battleOptionData != null && buffEvt.battleOptionData.buffDebuffType == eBuffDebuffType.Debuff && buffEvt.duration > 0.0f)
            {
                debuffTimeReduce = buffEvt.duration * owner.GetDebuffTimeReduceValue(buffEvt.battleOptionData != null ? buffEvt.battleOptionData.toExecuteType : BattleOption.eToExecuteType.Unit);
            }

            checkDuration = debuffTimeReduce;
            checkTick = 0.0f;
            curStackCount = 1;
            changeStackCount = false;
            repeatDelay = buffEvt.battleOptionData != null ? buffEvt.battleOptionData.repeatDelay : 0.0f;
            AddDuration = 0.0f;

            StartOwnerAction = owner.actionSystem.GetCurrentAction<ActionSelectSkillBase>();

#if UNITY_EDITOR
            if (StartOwnerAction)
            {
                ActionSelectSkillBase child = StartOwnerAction.GetLastChildAction();
                if (child)
                {
                    StartOwnerAction = child;
                }
            }
#endif

            cr = null;
            crRepeat = null;
            OnEnd = callback;

            ChangedSuperArmorId = 0;
        }

        public void Overlap(Unit owner, ref bool buffIconAdd)
        {
            float debuffTimeReduce = 0.0f;
            if (buffEvt.battleOptionData != null && buffEvt.battleOptionData.buffDebuffType == eBuffDebuffType.Debuff && buffEvt.duration > 0.0f)
            {
                debuffTimeReduce = buffEvt.duration * owner.GetDebuffTimeReduceValue(buffEvt.battleOptionData != null ? buffEvt.battleOptionData.toExecuteType : BattleOption.eToExecuteType.Unit);
            }

            checkDuration = debuffTimeReduce;

            if (uiIconInfo != null)
            {
                if (uiIconInfo.cr != null)
                    uiIconInfo.ResetCheckDuration();
                else
                    buffIconAdd = true;
            }

            if (curStackCount >= buffEvt.battleOptionData.maxStack)
            {
                return;
            }

            curStackCount = Mathf.Clamp(curStackCount + 1, 1, buffEvt.battleOptionData.maxStack);
            Debug.Log( owner.name + "의 배틀옵션 아이디 : " + buffEvt.battleOptionData.battleOptionSetId + "의 " + buffEvt.battleOptionData.funcType + "타입 배틀 옵션 스택 카운트 : " + curStackCount);

            changeStackCount = true;
        }
    }


    public bool Unused { get; set; } = false;

    private Dictionary<int, sExecute>   mDicExecute         = new Dictionary<int, sExecute>();
    private Dictionary<int, sExecute>   mDicExecuteRepeat   = new Dictionary<int, sExecute>();
    private BuffEvent                   mBuffEvt            = new BuffEvent();
    private AttackEvent                 mAtkEvt             = new AttackEvent();
    private List<sExecute>              mListTempExecute    = new List<sExecute>();
	private WaitForFixedUpdate			mWaitForFixedUpdate	= new WaitForFixedUpdate();


    public bool IsExecution(eEventType eventType)
    {
        foreach(KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            if (kv.Value.buffEvt.eventType == eventType)
                return true;
        }

        return false;
    }

    public bool HasBuff()
    {
        foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            if (kv.Value.buffEvt.battleOptionData.buffDebuffType == eBuffDebuffType.Buff)
                return true;
        }

        return false;
    }

    public bool HasDebuff()
    {
        foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            if (kv.Value.buffEvt.battleOptionData.buffDebuffType == eBuffDebuffType.Debuff)
                return true;
        }

        return false;
    }

    public bool HasDebuff(eEventType eventType)
    {
        foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            if (kv.Value.buffEvt.battleOptionData.buffDebuffType == eBuffDebuffType.Debuff && kv.Value.buffEvt.eventType == eventType)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasBuffIcon(eBuffIconType bufficon)
    {
        foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            if (kv.Value.buffEvt.buffIconType == bufficon)
                return true;
        }

        return false;
    }

    public sExecute FindBuffDebuffByFuncTypeOrNull(BattleOption.eBOFuncType funcType, int exceptId)
    {
        foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            if(kv.Key == exceptId)
            {
                continue;
            }

            if(kv.Value.buffEvt.battleOptionData != null && kv.Value.buffEvt.battleOptionData.funcType == funcType)
            {
                return kv.Value;
            }
        }

        return null;
    }

    public sExecute FindBuffDebuffByIdOrNull(int id)
    {
        foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            if (kv.Key == id)
            {
                return kv.Value;
            }
        }

        return null;
    }

	public int GetCount(eBuffDebuffType type)
	{
		int count = 0;

		foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
		{
			if (kv.Value.buffEvt.battleOptionData.buffDebuffType == type)
			{
				++count;
			}
		}

		return count;
	}

    public int GetStackCountByIconType( eBuffIconType type ) {
        int count = 0;

        foreach( KeyValuePair<int, sExecute> kv in mDicExecute ) {
            if( kv.Value.buffEvt.buffIconType == type ) {
                count += kv.Value.curStackCount;
            }
        }

        return count;
    }

    public void RemoveSuperArmorBuff(int execeptId)
    {
        sExecute find = FindBuffDebuffByFuncTypeOrNull(BattleOption.eBOFuncType.SuperArmor, execeptId);
        if (find != null)
        {
            RemoveExecute(find.buffEvt.id);
        }
        else
        {
            find = FindBuffDebuffByFuncTypeOrNull(BattleOption.eBOFuncType.SuperArmorTimeInc, execeptId);
            if (find != null)
            {
                RemoveExecute(find.buffEvt.id);
            }
            else
            {
                find = FindBuffDebuffByFuncTypeOrNull(BattleOption.eBOFuncType.SuperArmorTimeInc, execeptId);
                if (find != null)
                {
                    RemoveExecute(find.buffEvt.id);
                }
            }
        }
    }

	public void RemoveSpeedUpBuff()
	{
		sExecute find = FindBuffDebuffByFuncTypeOrNull(BattleOption.eBOFuncType.SpeedUp, -1);
		if (find != null)
		{
			RemoveExecute(find.buffEvt.id);
		}
	}

	public void AddDebuffDuration(eBuffDebuffType buffType, int count, float add)
    {
        mListTempExecute.Clear();
        foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            if (kv.Value.buffEvt == null || kv.Value.buffEvt.battleOptionData == null)
            {
                continue;
            }

            if (kv.Value.buffEvt.battleOptionData.buffDebuffType != buffType)
            {
                continue;
            }

            mListTempExecute.Add(kv.Value);
        }

        for(int i = 0; i < count; i++)
        {
            if(mListTempExecute.Count <= 0)
            {
                return;
            }

            sExecute execute = mListTempExecute[UnityEngine.Random.Range(0, mListTempExecute.Count)];
            if(execute == null || execute.uiIconInfo == null)
            {
                return;
            }

            execute.AddDuration += add;
            execute.uiIconInfo.checkDuration -= add;

            mListTempExecute.Remove(execute);
        }
    }

    public void Execute(BuffEvent evt, Action callbackOnEnd = null)
    {
		if( Unused ) {
			return;
		}

		if( evt.battleOptionData != null ) {
			if( evt.battleOptionData.targetType != BattleOption.eBOTargetType.Self && 
                evt.battleOptionData.buffDebuffType == eBuffDebuffType.Debuff &&
				( m_owner.CurrentSuperArmor == Unit.eSuperArmor.Invincible || m_owner.TemporaryInvincible ) ) {
				return;
			}

			if( evt.battleOptionData.preConditionType != BattleOption.eBOConditionType.None &&
				evt.battleOptionData.timingType != BattleOption.eBOTimingType.GameStart ) {
				if( !GameSupport.IsBOConditionCheck( evt.battleOptionData.preConditionType, evt.battleOptionData.CondValue, m_owner, evt.sender,
													 evt.battleOptionData.CheckActionTableId, evt.battleOptionData.Pjt ) ) {
					return;
				}
			}

            if( evt.battleOptionData.conditionType != BattleOption.eBOConditionType.None &&
                evt.battleOptionData.timingType != BattleOption.eBOTimingType.GameStart &&
                evt.battleOptionData.buffDebuffType == eBuffDebuffType.Debuff ) {
                if( !GameSupport.IsBOConditionCheck( evt.battleOptionData.conditionType, evt.battleOptionData.CondValue, evt.sender, m_owner,
                                                     evt.battleOptionData.CheckActionTableId, evt.battleOptionData.Pjt ) ) {
                    return;
                }
            }

            if( evt.battleOptionData.buffDebuffType == eBuffDebuffType.Debuff && evt.duration > 0.0f ) {
				if( m_owner.HasDebuffImmuneType( eDebuffImmuneType_f.ATTACK_RATE_DOWN ) ) {
					if( evt.eventType == eEventType.EVENT_DEBUFF_ATKPOWER_RATE_DOWN ) {
						return;
					}
				}
				
                if( m_owner.HasDebuffImmuneType( eDebuffImmuneType_f.DMG_RATE_UP ) ) {
					if( evt.eventType == eEventType.EVENT_DEBUFF_DMG_RATE_UP ) {
						return;
					}
				}
				
                if( m_owner.HasDebuffImmuneType( eDebuffImmuneType_f.SPEED_DOWN ) ) {
					if( evt.eventType == eEventType.EVENT_DEBUFF_SPEED_DOWN ||
						evt.eventType == eEventType.EVENT_DEBUFF_SPEED_DOWN_AND_SKIP_CUR_ANI ||
						evt.eventType == eEventType.EVENT_DEBUFF_SPEED_DOWN_TO_TARGET ) {
						return;
					}
				}
				
                if( m_owner.HasDebuffImmuneType( eDebuffImmuneType_f.CRITICAL_RATE_DOWN ) ) {
                    if( evt.eventType == eEventType.EVENT_DEBUFF_CRITICAL_RATE_DOWN ) {
                        return;
                    }
				}
				
                if( m_owner.HasDebuffImmuneType( eDebuffImmuneType_f.CRITICAL_DMG_DOWN ) ) {
                    if( evt.eventType == eEventType.EVENT_DEBUFF_CRITICAL_DMG_DOWN ) {
                        return;
                    }
				}
			}
		}

		if (evt.duration == 0.0f)
        {
            Debug.LogError("버프/디버프 컴포넌트에서는 Duration 값이 필요합니다. : " + evt.eventType);
            return;
        }

		if( mDicExecute.ContainsKey( evt.id ) ) {
			if( evt.battleOptionData != null && evt.battleOptionData.maxStack > 1 ) // 중첩 효과가 들어간 버프
			{
				bool buffIconAdd = false;
				mDicExecute[evt.id].Overlap( m_owner, ref buffIconAdd );

				if( buffIconAdd == true ) {
					mDicExecute[evt.id].uiIconInfo = ShowOwnerBuffDebuffIcon( evt );
				}

				return;
			}
			else {
				RemoveExecute( evt.id, true );
			}
		}
		else if( evt.battleOptionData != null ) {
            // 슈퍼아머 배틀옵션일 경우 기존 슈퍼아머 버프를 지우고 새로 넣는다.
            if( evt.battleOptionData.funcType == BattleOption.eBOFuncType.SuperArmor || 
                evt.battleOptionData.funcType == BattleOption.eBOFuncType.SuperArmorTimeInc ||
                evt.battleOptionData.funcType == BattleOption.eBOFuncType.ConditionalSuperArmor ) {
				RemoveSuperArmorBuff( evt.id );
			}
			else if( evt.battleOptionData.funcType == BattleOption.eBOFuncType.SkipEnemyAttack ) {
                sExecute find = FindBuffDebuffByFuncTypeOrNull( BattleOption.eBOFuncType.SkipEnemyAttack, 0 );
                if( find != null ) {
                    RemoveExecute( find.buffEvt.id );
				}
			}
		}

		UIBuffDebuffIcon.sActiveIconInfo uiIconInfo = ShowOwnerBuffDebuffIcon(evt);

        sExecute execute = new sExecute(m_owner, evt, callbackOnEnd, uiIconInfo);
        mDicExecute.Add(evt.id, execute);

        switch (evt.eventType)
        {
            case eEventType.EVENT_DEBUFF_DOT_DMG:
                execute.cr = World.Instance.StartCoroutine(UpdateDotDmg(evt.id, execute));
                break;

            case eEventType.EVENT_BUFF_BLOODSUCKING:
                {
                    /*if (evt.listArgs.Count <= 0)
                    {
                        Debug.LogError("EVENT_BUFF_BLOODSUCKING에 필요한 데미지 정보가 없습니다.");
                        return;
                    }*/

                    execute.value = m_owner.lastAttackPower * evt.value;//evt.listArgs[0] * evt.value;
                    if (execute.buffEvt.duration <= 0.0f)
                    {
                        m_owner.AddHp(execute.buffEvt.battleOptionData.toExecuteType, execute.value, false );
                        RemoveExecute(evt.id);
                    }
                    else
                        execute.cr = World.Instance.StartCoroutine(UpdateBloodSucking(evt.id, execute));
                }
                break;

            case eEventType.EVENT_BUFF_DOT_HEAL:
                execute.cr = World.Instance.StartCoroutine(UpdateDotHeal(evt.id, execute));
                break;

            case eEventType.EVENT_BUFF_DOT_HEAL_ABS:
                execute.cr = World.Instance.StartCoroutine(UpdateDotHealAbs(evt.id, execute));
                break;

            case eEventType.EVENT_BUFF_DOT_HEAL_COST_HP:
                execute.cr = World.Instance.StartCoroutine(UpdateDotHealCostHP(evt.id, execute));
                break;

            case eEventType.EVENT_BUFF_DOT_HEAL_COST_HP_CONDI:
                execute.cr = World.Instance.StartCoroutine(UpdateDotHealCostHPCondi(evt.id, execute));
                break;

            case eEventType.EVENT_BUFF_SPEED_UP:
            case eEventType.EVENT_DEBUFF_SPEED_DOWN:
            case eEventType.EVENT_DEBUFF_SPEED_DOWN_AND_SKIP_CUR_ANI:
                {
                    UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                    if (evt.eventType == eEventType.EVENT_DEBUFF_SPEED_DOWN || evt.eventType == eEventType.EVENT_DEBUFF_SPEED_DOWN_AND_SKIP_CUR_ANI)
                    {
                        increaseType = UnitBuffStats.eIncreaseType.Decrease;
                    }

                    execute.cr = World.Instance.StartCoroutine(UpdateSpeed(evt.id, execute, increaseType, evt.eventType == eEventType.EVENT_DEBUFF_SPEED_DOWN_AND_SKIP_CUR_ANI));
                }
                break;

            case eEventType.EVENT_BUFF_SPEED_UP_ON_EFFECT:
                execute.cr = World.Instance.StartCoroutine( UpdateSpeedUpOnEffect( evt.id, execute ) );
                break;

            case eEventType.EVENT_DEBUFF_SPEED_DOWN_TO_TARGET:
                execute.cr = World.Instance.StartCoroutine(UpdateSpeedDownToTarget(evt.id, execute));
                break;

            case eEventType.EVENT_BUFF_DMG_RATE_DOWN:
            case eEventType.EVENT_DEBUFF_DMG_RATE_UP:
                {
                    UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Decrease;
                    if (evt.eventType == eEventType.EVENT_DEBUFF_DMG_RATE_UP)
                        increaseType = UnitBuffStats.eIncreaseType.Increase;

                    execute.cr = World.Instance.StartCoroutine(UpdateDmgRate(evt.id, execute, increaseType));
                }
                break;

            case eEventType.EVENT_BUFF_DMG_RATE_DOWN_WITH_ACCUM_DAMAGE:
                {
                    execute.cr = World.Instance.StartCoroutine(UpdateDmgRateDownWithAccumDamage(evt.id, execute, UnitBuffStats.eIncreaseType.Decrease));
                }
                break;

            case eEventType.EVENT_BUFF_ATKPOWER_RATE_UP:
            case eEventType.EVENT_DEBUFF_ATKPOWER_RATE_DOWN:
                {
                    UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                    if (evt.eventType == eEventType.EVENT_DEBUFF_ATKPOWER_RATE_DOWN)
                        increaseType = UnitBuffStats.eIncreaseType.Decrease;

                    execute.cr = World.Instance.StartCoroutine(UpdateAtkPowerRate(evt.id, execute, increaseType));
                }
                break;

            case eEventType.EVENT_BUFF_ATKPOWER_TO_SHIELD_RATE_UP:
            case eEventType.EVENT_DEBUFF_ATKPOWER_TO_SHIELD_RATE_DOWN:
                {
                    UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                    if (evt.eventType == eEventType.EVENT_DEBUFF_ATKPOWER_TO_SHIELD_RATE_DOWN)
                        increaseType = UnitBuffStats.eIncreaseType.Decrease;

                    execute.cr = World.Instance.StartCoroutine(UpdateAtkPowerToShieldRate(evt.id, execute, increaseType));
                }
                break;

            case eEventType.EVENT_BUFF_ULTIMATESKILL_ATKPOWER_RATE_UP:
            case eEventType.EVENT_DEBUFF_ULTIMATESKILL_ATKPOWER_RATE_DOWN:
                {
                    UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                    if (evt.eventType == eEventType.EVENT_DEBUFF_ULTIMATESKILL_ATKPOWER_RATE_DOWN)
                        increaseType = UnitBuffStats.eIncreaseType.Decrease;

                    execute.cr = World.Instance.StartCoroutine(UpdateUltimateskillAtkPowerRate(evt.id, execute, increaseType));
                }
                break;

            case eEventType.EVENT_BUFF_HEAL_RATE_UP:
            case eEventType.EVENT_DEBUFF_HEAL_RATE_DOWN:
                {
                    UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                    if (evt.eventType == eEventType.EVENT_DEBUFF_HEAL_RATE_DOWN)
                        increaseType = UnitBuffStats.eIncreaseType.Decrease;

                    execute.cr = World.Instance.StartCoroutine(UpdateHealRate(evt.id, execute, increaseType));
                }
                break;

            case eEventType.EVENT_BUFF_HEAL_ON_DIE_IN_TIME: {
                execute.cr = World.Instance.StartCoroutine( UpdateHealOnDieInTime( evt.id, execute ) );
            }
            break;

            case eEventType.EVENT_BUFF_SP_ADD_INC_RATE:
            case eEventType.EVENT_DEBUFF_SP_ADD_DEC_RATE:
                {
                    UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                    if (evt.eventType == eEventType.EVENT_DEBUFF_SP_ADD_DEC_RATE)
                        increaseType = UnitBuffStats.eIncreaseType.Decrease;

                    execute.cr = World.Instance.StartCoroutine(UpdateSPAddRate(evt.id, execute, increaseType));
                }
                break;

            case eEventType.EVENT_BUFF_TIME_DEBUFF_REDUCE:
            case eEventType.EVENT_DEBUFF_TIME_DEBUFF_PROMOTE:
                {
                    UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                    if (evt.eventType == eEventType.EVENT_DEBUFF_TIME_DEBUFF_PROMOTE)
                        increaseType = UnitBuffStats.eIncreaseType.Decrease;

                    execute.cr = World.Instance.StartCoroutine(UpdateDebuffDurTime(evt.id, execute, increaseType));
                }
                break;

            case eEventType.EVENT_BUFF_CRITICAL_RATE_UP:
            case eEventType.EVENT_DEBUFF_CRITICAL_RATE_DOWN:
                {
                    UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                    if (evt.eventType == eEventType.EVENT_DEBUFF_CRITICAL_RATE_DOWN)
                        increaseType = UnitBuffStats.eIncreaseType.Decrease;

                    execute.cr = World.Instance.StartCoroutine(UpdateCriticalRate(evt.id, execute, increaseType));
                }
                break;

            case eEventType.EVENT_BUFF_CRITICAL_DMG_UP:
            case eEventType.EVENT_DEBUFF_CRITICAL_DMG_DOWN:
                {
                    UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                    if (evt.eventType == eEventType.EVENT_DEBUFF_CRITICAL_DMG_DOWN)
                        increaseType = UnitBuffStats.eIncreaseType.Decrease;

                    execute.cr = World.Instance.StartCoroutine(UpdateCriticalDmg(evt.id, execute, increaseType));
                }
                break;

            case eEventType.EVENT_BUFF_CRITICAL_RESIST_UP:
            case eEventType.EVENT_DEBUFF_CRITICAL_RESIST_DOWN:
                {
                    UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                    if (evt.eventType == eEventType.EVENT_BUFF_CRITICAL_RESIST_UP)
                        increaseType = UnitBuffStats.eIncreaseType.Decrease;

                    execute.cr = World.Instance.StartCoroutine(UpdateCriticalResist(evt.id, execute, increaseType));
                }
                break;

            case eEventType.EVENT_BUFF_CRITICAL_DEF_UP:
            case eEventType.EVENT_DEBUFF_CRITICAL_DEF_DOWN:
                {
                    UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                    if (evt.eventType == eEventType.EVENT_BUFF_CRITICAL_DEF_UP)
                        increaseType = UnitBuffStats.eIncreaseType.Decrease;

                    execute.cr = World.Instance.StartCoroutine(UpdateCriticalDef(evt.id, execute, increaseType));
                }
                break;

			case eEventType.EVENT_BUFF_PENETRATE_UP: {
				UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
				execute.cr = World.Instance.StartCoroutine( UpdatePenetrate( evt.id, execute, increaseType ) );
			}
			break;

            case eEventType.EVENT_BUFF_SUFFERANCE_UP: {
                UnitBuffStats.eIncreaseType increaseType = UnitBuffStats.eIncreaseType.Increase;
                execute.cr = World.Instance.StartCoroutine( UpdateSufferance( evt.id, execute, increaseType ) );
            }
            break;

            case eEventType.EVENT_DEBUFF_HOLD_POSITION:
                execute.cr = World.Instance.StartCoroutine(UpdateHoldPosition(evt.id, execute));
                break;

            case eEventType.EVENT_DEBUFF_SKIP_ATTACK:
                execute.cr = World.Instance.StartCoroutine(UpdateSkipAttack(evt.id, execute));
                break;

            case eEventType.EVENT_DEBUFF_DOT_SHIELD_DMG:
                {
                    if (m_owner.curShield > 0.0f)
                        execute.cr = World.Instance.StartCoroutine(UpdateShieldDmg(evt.id, execute));
                }
                break;

            case eEventType.EVENT_BUFF_REFLECTION:
                execute.cr = World.Instance.StartCoroutine(UpdateReflection(evt.id, execute));
                break;

            case eEventType.EVENT_BUFF_REFLECTION_AROUND:
                execute.cr = World.Instance.StartCoroutine(UpdateReflectionAround(evt.id, execute));
                break;

            case eEventType.EVENT_BUFF_RANDOM_KNOCKBACK:
                execute.cr = World.Instance.StartCoroutine(UpdateRandomKnockback(evt.id, execute));
                break;

            case eEventType.EVENT_BUFF_SUPER_ARMOR:
                execute.cr = World.Instance.StartCoroutine(UpdateSuperArmor(evt.id, execute));
                break;

            case eEventType.EVENT_BUFF_CONDITIONAL_SUPER_ARMOR:
                execute.cr = World.Instance.StartCoroutine(UpdateConditionalSuperArmor(evt.id, execute));
                break;

            case eEventType.EVENT_BUFF_HOLD_POSITION:
                execute.cr = World.Instance.StartCoroutine(UpdateSendHoldPositionToTarget(evt.id, execute));
                break;

            case eEventType.EVENT_DEBUFF_DECREASE_SP:
                execute.cr = World.Instance.StartCoroutine(UpdateDecreaseSP(evt.id, execute));
                break;

            case eEventType.EVENT_DEBUFF_MOVEMENT_INVERSE:
                execute.cr = World.Instance.StartCoroutine(UpdateMovementInverse(evt.id, execute));
                break;

            case eEventType.EVENT_BUFF_DOT_DMG_TO_TARGET:
                execute.cr = World.Instance.StartCoroutine(UpdateSendDotDmgToTarget(evt.id, execute));
                break;

            case eEventType.EVENT_BUFF_ATTACK_TO_TARGET:
                execute.cr = World.Instance.StartCoroutine(UpdateSendAttackToTarget(evt.id, execute));
                break;

            case eEventType.EVENT_DEBUFF_FREEZE:
                execute.cr = World.Instance.StartCoroutine(UpdateFreeze(evt.id, execute));
                break;

            case eEventType.EVENT_BUFF_COMMAND_CLONE_ATTACK02:
                execute.cr = World.Instance.StartCoroutine(UpdateCommandCloneAttack2(evt.id, execute));
                break;

            case eEventType.EVENT_DEBUFF_MARKING:
                execute.cr = World.Instance.StartCoroutine(UpdateMarking(evt.id, execute));
                break;

            case eEventType.EVENT_DEBUFF_HOLD_SUPPORTER_SKILL_COOLTIME:
                execute.cr = World.Instance.StartCoroutine(UpdateHoldSupporterSkillCoolTime(evt.id, execute));
                break;

            case eEventType.EVENT_BLACKHOLE:
                execute.cr = World.Instance.StartCoroutine(UpdateBlackhole(evt.id, execute));
                break;

            case eEventType.EVENT_HIDE:
                execute.cr = World.Instance.StartCoroutine(UpdateHide(evt.id, execute));
                break;

			case eEventType.EVENT_SUMMON_PLAYER_MINION:
				execute.cr = World.Instance.StartCoroutine(UpdateSummon(evt.id, execute));
				break;

            case eEventType.EVENT_SUMMON_PLAYER_MINION_BY_DIE_ENEMY_COUNT:
                execute.cr = World.Instance.StartCoroutine( UpdateSummonByDieEnemyCount( evt.id, execute ) );
                break;

            case eEventType.EVENT_SP_REGEN_INC_RATE:
				execute.cr = World.Instance.StartCoroutine(UpdateSpRegenIncRate(evt.id, execute));
				break;

			case eEventType.EVENT_DEBUFF_ELECTRIC_SHOCK:
				execute.cr = World.Instance.StartCoroutine(UpdateElectricShock(evt.id, execute));
				break;

			case eEventType.EVENT_BUFF_LIGHTNING_ATTACK:
				execute.cr = World.Instance.StartCoroutine(UpdateLightningAttack(evt.id, execute));
				break;

            case eEventType.EVENT_BUFF_FIRE_AURA:
                execute.cr = World.Instance.StartCoroutine( UpdateFireAura( evt.id, execute ) );
                break;
		}
    }

    public void RemoveExecute(int id, bool manualHideEff = false, bool forceRemove = false)
    {
        if (!mDicExecute.ContainsKey(id))
        {
            return;
        }

        sExecute execute = mDicExecute[id];
        execute.OnRemove?.Invoke(id);

        if (execute.uiIconInfo != null)
        {
#if UNITY_EDITOR
            if (execute.buffEvt != null && execute.buffEvt.battleOptionData != null && execute.buffEvt.battleOptionData.duration <= -1)
#else
            if (execute.buffEvt != null && execute.buffEvt.battleOptionData != null && execute.buffEvt.battleOptionData.duration == -1)
#endif
            {
                m_owner.uiBuffDebuffIcon.HideActiveIcon(execute.uiIconInfo);
            }
            else
            {
				if (execute.uiIconInfo.duration <= 0)
				{
					m_owner.uiBuffDebuffIcon.HideActiveIcon(execute.uiIconInfo);
				}
				else
				{
					execute.uiIconInfo.checkDuration = execute.uiIconInfo.duration;
				}
            }
        }

        if (execute.cr != null)
        {
            Utility.StopCoroutine(World.Instance, ref execute.cr); //execute.checkDuration = execute.buffEvt.duration;
        }

        if (execute.buffEvt != null)
        {
            if (!manualHideEff)
            {
                if (execute.buffEvt.effId > 0)
                {
                    EffectManager.Instance.StopEffImmediate(execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType, null);
                }

                if (execute.buffEvt.effId2 > 0)
                {
                    EffectManager.Instance.StopEffImmediate(execute.buffEvt.effId2, (EffectManager.eType)execute.buffEvt.battleOptionData.effType, null);
                }
            }

            if (execute.buffEvt.battleOptionData != null)
            {
                execute.buffEvt.battleOptionData.OnEnd();
            }
        }

        execute.OnEnd?.Invoke();

        execute.StartOwnerAction = null;
        mDicExecute.Remove(id);

        if (execute.buffEvt.battleOptionData.repeat)
        {
            Utility.StopCoroutine(World.Instance, ref execute.crRepeat);

            if (!forceRemove)
            {
                execute.crRepeat = World.Instance.StartCoroutine(WaitRepeatAndExecute(id, execute));
            }
        }
    }

    public void Clear()
    {
        mListRemoveId.Clear();
        foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            mListRemoveId.Add(kv.Key);
        }

        for (int i = 0; i < mListRemoveId.Count; i++)
        {
            RemoveExecute(mListRemoveId[i], false, true);
        }

        mDicExecute.Clear();

        ClearAllRepeat();
    }

    private List<int> mListRemoveId = new List<int>();
    private List<int> mListRemoveRepeatId = new List<int>();

    public void ClearAllRepeat()
    {
        mListRemoveId.Clear();
        foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            if (kv.Value.buffEvt.battleOptionData.repeat)
            {
                mListRemoveId.Add(kv.Key);
            }
        }

        for (int i = 0; i < mListRemoveId.Count; i++)
        {
            RemoveExecute(mListRemoveId[i], false, true);
        }

        // 반복 버프/디버프는 코루틴을 따로 타는게 있어서 그 코루틴 정지
        foreach (KeyValuePair<int, sExecute> kv in mDicExecuteRepeat)
        {
            Utility.StopCoroutine(World.Instance, ref kv.Value.crRepeat);
        }

        mDicExecuteRepeat.Clear();
    }

    public void ClearAllDebuff()
    {
        mListRemoveId.Clear();
        foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            if (kv.Value.buffEvt.battleOptionData.buffDebuffType == eBuffDebuffType.Debuff)
            {
                mListRemoveId.Add(kv.Key);
            }
        }

        for(int i = 0; i < mListRemoveId.Count; i++)
        {
            RemoveExecute(mListRemoveId[i], false, true);
        }

        // 반복 버프/디버프는 코루틴을 따로 타는게 있어서 그 코루틴 정지
        mListRemoveRepeatId.Clear();
        foreach (KeyValuePair<int, sExecute> kv in mDicExecuteRepeat)
        {
            if (kv.Value.buffEvt.battleOptionData.buffDebuffType == eBuffDebuffType.Debuff)
            {
                mListRemoveRepeatId.Add(kv.Key);
                Utility.StopCoroutine(World.Instance, ref kv.Value.crRepeat);
            }
        }

        for (int i = 0; i < mListRemoveRepeatId.Count; i++)
        {
            mDicExecuteRepeat.Remove(mListRemoveRepeatId[i]);
        }
    }

    public void RemoveDebuffById(int id)
    {
        mListRemoveId.Clear();

        foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            if (kv.Value.buffEvt.battleOptionData.buffDebuffType == eBuffDebuffType.Debuff && kv.Key == id)
            {
                mListRemoveId.Add(kv.Key);
            }
        }

        for (int i = 0; i < mListRemoveId.Count; i++)
        {
            RemoveExecute(mListRemoveId[i], false, true);
        }

        // 반복 버프/디버프는 코루틴을 따로 타는게 있어서 그 코루틴 정지
        foreach (KeyValuePair<int, sExecute> kv in mDicExecuteRepeat)
        {
            for (int i = 0; i < mListRemoveId.Count; i++)
            {
                if (kv.Key == mListRemoveId[i])
                {
                    Utility.StopCoroutine(World.Instance, ref kv.Value.crRepeat);
                }
            }
        }

        for (int i = 0; i < mListRemoveId.Count; i++)
        {
            mDicExecuteRepeat.Remove(mListRemoveId[i]);
        }
    }

    public void RemoveDebuff(int num, bool withRemoveIncapable)
    {
        // 행동 불능에 걸려있으면 우선적으로 제거하고 디버프 제거 개수를 하나 줄임
        if (withRemoveIncapable && m_owner.actionSystem && m_owner.actionSystem.currentAction && 
           m_owner.actionSystem.currentAction.actionCommand == eActionCommand.Hit)
        {
            ActionHit actionHit = m_owner.actionSystem.GetCurrentAction<ActionHit>();
            if(actionHit && actionHit.State == ActionHit.eState.Stun)
            {
                HitStun hitStun = actionHit.GetCurrentStunHitOrNull();
                if(hitStun)
                {
                    hitStun.Terminate();
                    --num;
                }
            }
        }

        if(num <= 0)
        {
            return;
        }

        int removeCount = 0;
        mListRemoveId.Clear();

        foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            if (kv.Value.buffEvt.battleOptionData.buffDebuffType == eBuffDebuffType.Debuff)
            {
                mListRemoveId.Add(kv.Key);
                ++removeCount;

                if(removeCount >= num)
                { 
                    break;
                }
            }
        }

        for (int i = 0; i < mListRemoveId.Count; i++)
        {
            RemoveExecute(mListRemoveId[i], false, true);
        }

        // 반복 버프/디버프는 코루틴을 따로 타는게 있어서 그 코루틴 정지
        foreach (KeyValuePair<int, sExecute> kv in mDicExecuteRepeat)
        {
            for (int i = 0; i < mListRemoveId.Count; i++)
            {
                if (kv.Key == mListRemoveId[i])
                {
                    Utility.StopCoroutine(World.Instance, ref kv.Value.crRepeat);
                }
            }
        }

        for (int i = 0; i < mListRemoveId.Count; i++)
        {
            mDicExecuteRepeat.Remove(mListRemoveId[i]);
        }
    }

    public void RemoveDebuff(eEventType eventType)
    {
        mListRemoveId.Clear();
        foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            if (kv.Value.buffEvt.battleOptionData.buffDebuffType == eBuffDebuffType.Debuff && kv.Value.buffEvt.eventType == eventType)
            {
                mListRemoveId.Add(kv.Key);
            }
        }

        for (int i = 0; i < mListRemoveId.Count; i++)
        {
            RemoveExecute(mListRemoveId[i], false, true);
        }

        // 반복 버프/디버프는 코루틴을 따로 타는게 있어서 그 코루틴 정지
        mListRemoveRepeatId.Clear();
        foreach (KeyValuePair<int, sExecute> kv in mDicExecuteRepeat)
        {
            if (kv.Value.buffEvt.battleOptionData.buffDebuffType == eBuffDebuffType.Debuff && kv.Value.buffEvt.eventType == eventType)
            {
                mListRemoveRepeatId.Add(kv.Key);
                Utility.StopCoroutine(World.Instance, ref kv.Value.crRepeat);
            }
        }

        for (int i = 0; i < mListRemoveRepeatId.Count; i++)
        {
            mDicExecuteRepeat.Remove(mListRemoveRepeatId[i]);
        }
    }

    public void ClearAllBuff()
    {
        mListRemoveId.Clear();
        foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            if (kv.Value.buffEvt.battleOptionData.buffDebuffType == eBuffDebuffType.Buff)
            {
                mListRemoveId.Add(kv.Key);
            }
        }

        for (int i = 0; i < mListRemoveId.Count; i++)
        {
            RemoveExecute(mListRemoveId[i], false, true);
        }

        // 반복 버프/디버프는 코루틴을 따로 타는게 있어서 그 코루틴 정지
        mListRemoveRepeatId.Clear();
        foreach (KeyValuePair<int, sExecute> kv in mDicExecuteRepeat)
        {
            if (kv.Value.buffEvt.battleOptionData.buffDebuffType == eBuffDebuffType.Buff)
            {
                mListRemoveRepeatId.Add(kv.Key);
                Utility.StopCoroutine(World.Instance, ref kv.Value.crRepeat);
            }
        }

        for (int i = 0; i < mListRemoveRepeatId.Count; i++)
        {
            mDicExecuteRepeat.Remove(mListRemoveRepeatId[i]);
        }
    }

    public void RemoveBuff(int num, bool checkDuration = false)
    {
        int removeCount = 0;
        mListRemoveId.Clear();

        foreach (KeyValuePair<int, sExecute> kv in mDicExecute)
        {
            if (kv.Value.buffEvt.battleOptionData.buffDebuffType == eBuffDebuffType.Buff)
            {
                if(checkDuration && kv.Value.buffEvt.duration <= 0.0f)
                {
                    continue;
                }

                mListRemoveId.Add(kv.Key);
                ++removeCount;

                if (removeCount >= num)
                {
                    break;
                }
            }
        }

        for (int i = 0; i < mListRemoveId.Count; i++)
        {
            RemoveExecute(mListRemoveId[i], false, true);
        }

        // 반복 버프/디버프는 코루틴을 따로 타는게 있어서 그 코루틴 정지
        foreach (KeyValuePair<int, sExecute> kv in mDicExecuteRepeat)
        {
            for (int i = 0; i < mListRemoveId.Count; i++)
            {
                if (kv.Key == mListRemoveId[i])
                {
                    Utility.StopCoroutine(World.Instance, ref kv.Value.crRepeat);
                }
            }
        }

        for (int i = 0; i < mListRemoveId.Count; i++)
        {
            mDicExecuteRepeat.Remove(mListRemoveId[i]);
        }
    }

    private UIBuffDebuffIcon.sActiveIconInfo ShowOwnerBuffDebuffIcon(BuffEvent evt)
    {
        if (evt.buffIconType == eBuffIconType.None)
            return null;

        float debuffTime = evt.duration;
        if (evt.battleOptionData != null && evt.battleOptionData.buffDebuffType == eBuffDebuffType.Debuff && evt.duration > 0.0f)
        {
            debuffTime = evt.duration - (evt.duration * m_owner.GetDebuffTimeReduceValue(evt.battleOptionData != null ? evt.battleOptionData.toExecuteType : BattleOption.eToExecuteType.Unit));
        }

        bool buffIconFlash = evt.battleOptionData != null ? evt.battleOptionData.buffIconFlash : true;

        UIBuffDebuffIcon.sActiveIconInfo uiIconInfo = null;
        if (m_owner.grade == Unit.eGrade.Boss && m_owner == World.Instance.UIPlay.CurrentActiveTarget)
        {
            uiIconInfo = World.Instance.UIPlay.buffDebuffIcon.Add(evt.buffIconType, debuffTime, buffIconFlash);
        }
        else if (m_owner.uiBuffDebuffIcon)
            uiIconInfo = m_owner.uiBuffDebuffIcon.Add(evt.buffIconType, debuffTime, buffIconFlash);

        return uiIconInfo;
    }

    private IEnumerator UpdateDotDmg(int id, sExecute execute)
    {
        AniEvent.sEvent evt = new AniEvent.sEvent();
        evt.hitDir = eHitDirection.None;
        evt.hitEffectId = execute.buffEvt.effId;
        evt.atkAttr = (EAttackAttr)execute.buffEvt.value2;

        m_owner.AddAbnormalAttr(evt.atkAttr);

        if (execute.buffEvt.effId2 > 0)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.effId2, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
        }

        float dmg = execute.buffEvt.sender.attackPower;
        if (execute.buffEvt.battleOptionData.atkConditionType == BattleOption.eBOAtkConditionType.SkillAttack)
        {
            dmg = execute.buffEvt.sender.LastNormalAttackPower;
        }

        float duration = execute.buffEvt.duration + execute.AddDuration;
        execute.checkTick = 0.0f; //execute.buffEvt.tick;
        // 시작 시 바로 한번 걸어줌
        mAtkEvt.SetWithSingleTarget( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, execute.buffEvt.sender, execute.buffEvt.battleOptionData.toExecuteType, evt,
                                                dmg * execute.value * execute.curStackCount, eAttackDirection.Skip, false, 0, EffectManager.eType.None,
                                                m_owner.MainCollider, 0.0f, true, false, true );

        Debug.LogError( "도트 대미지 : " + dmg * execute.value * execute.curStackCount );
        EventMgr.Instance.SendEvent( mAtkEvt );

        while (m_owner.curHp > 0.0f && execute.checkDuration < duration)
        {
            execute.checkTick += Time.fixedDeltaTime;
            if(!Director.IsPlaying && execute.checkTick >= execute.buffEvt.tick)
            {
                execute.checkDuration += execute.buffEvt.tick;

                if( execute.checkDuration < duration ) {
                    execute.checkTick = 0.0f;

                    if( execute.checkDuration <= duration ) {
                        mAtkEvt.SetWithSingleTarget( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, execute.buffEvt.sender, execute.buffEvt.battleOptionData.toExecuteType, evt,
                                                    dmg * execute.value * execute.curStackCount, eAttackDirection.Skip, false, 0, EffectManager.eType.None,
                                                    m_owner.MainCollider, 0.0f, true, false, true );

                        Debug.LogError( "도트 대미지 : " + dmg * execute.value * execute.curStackCount );
                        EventMgr.Instance.SendEvent( mAtkEvt );
                    }
                }
            }

            yield return mWaitForFixedUpdate;
        }

        m_owner.RemoveAbnormalAttr(evt.atkAttr);
        RemoveExecute(id, execute.buffEvt.value3 > 0 ? true : false);
    }

    private IEnumerator UpdateBloodSucking(int id, sExecute execute)
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f && execute.checkDuration < (execute.buffEvt.duration + execute.AddDuration))
        {
            execute.checkTick += Time.fixedDeltaTime;
            if (execute.checkTick >= execute.buffEvt.tick)
            {
                execute.checkDuration += execute.checkTick;
                execute.checkTick = 0.0f;

                m_owner.AddHp(execute.buffEvt.battleOptionData.toExecuteType, execute.value * execute.curStackCount, false );
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateDotHeal(int id, sExecute execute)
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f && execute.checkDuration < (execute.buffEvt.duration + execute.AddDuration))
        {
            execute.checkTick += Time.fixedDeltaTime;
            if (execute.checkTick >= execute.buffEvt.tick)
            {
                execute.checkDuration += execute.checkTick;
                execute.checkTick = 0.0f;

                m_owner.AddHpPercentage(execute.buffEvt.battleOptionData.toExecuteType, execute.value, false );
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateDotHealAbs(int id, sExecute execute)
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f && execute.checkDuration < (execute.buffEvt.duration + execute.AddDuration))
        {
            execute.checkTick += Time.fixedDeltaTime;
            if (execute.checkTick >= execute.buffEvt.tick)
            {
                execute.checkDuration += execute.checkTick;
                execute.checkTick = 0.0f;

                m_owner.AddHp(execute.buffEvt.battleOptionData.toExecuteType, execute.value, false );
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateDotHealCostHP(int id, sExecute execute)
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f && execute.checkDuration < (execute.buffEvt.duration + execute.AddDuration))
        {
            execute.checkTick += Time.fixedDeltaTime;
            if (execute.checkTick >= execute.buffEvt.tick)
            {
                execute.checkDuration += execute.checkTick;
                execute.checkTick = 0.0f;

                m_owner.AddHpPerCost(execute.buffEvt.battleOptionData.toExecuteType, execute.value, false );
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

	private IEnumerator UpdateDotHealCostHPCondi( int id, sExecute execute ) {
		while( m_owner.curHp > 0.0f && ( ( m_owner.curHp / m_owner.maxHp ) >= execute.buffEvt.value2 ) ) {
			execute.checkTick += Time.fixedDeltaTime;

			if( execute.checkTick >= execute.buffEvt.tick ) {
				execute.checkTick = 0.0f;

				if( !World.Instance.IsEndGame && !World.Instance.ProcessingEnd ) {
					m_owner.AddHpPerCost( execute.buffEvt.battleOptionData.toExecuteType, execute.value, false );
				}
			}

			yield return mWaitForFixedUpdate;
		}

		RemoveExecute( id );
	}

	private IEnumerator UpdateSpeed(int id, sExecute execute, UnitBuffStats.eIncreaseType increaseType, bool skipCurAni = false)
    {
        execute.OnRemove = OnRemoveUpdateSpeed;

        if( execute.value >= 1.0f ) {
            skipCurAni = true;
		}
            
        m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.Speed, increaseType, id, execute.value, -1, false, true);

        if (execute.buffEvt.effId > 0)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
        }

        m_owner.SetSpeedRateByCalcBuff(execute.buffEvt.battleOptionData.toExecuteType, true, true);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if (execute.checkDuration >= (execute.buffEvt.duration + execute.AddDuration))
                {
                    break;
                }
                else if (execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnBoxCollider && m_owner.StayOnBoxCollider == null)
                {
                    break;
                }
            }
            else if (execute.buffEvt.duration == -2)
            {
                if (execute.StartOwnerAction == null || execute.StartOwnerAction.IsSkillEnd)
                {
                    break;
                }
            }
            else
            {
                if(execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnProjectile && m_owner.StayOnProjectile == null)
                {
                    break;
                }
                else if (execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnBoxCollider && m_owner.StayOnBoxCollider == null)
                {
                    break;
                }
                else if (execute.StartOwnerAction && execute.StartOwnerAction.ForceQuitBuffDebuff)
                {
                    break;
                }
            }

            if (execute.changeStackCount)
            {
                execute.changeStackCount = false;

                if (execute.buffEvt.battleOptionData.value3 > 0.0f)
                {
                    if (execute.curStackCount >= execute.buffEvt.battleOptionData.maxStack && 
                        execute.buffEvt.battleOptionData.addCallTiming == BattleOption.eBOAddCallTiming.OnSend)
                    {
                        EffectManager.Instance.Play(m_owner, execute.buffEvt.battleOptionData.dataOnEndCall.startEffId, 
                                                    (EffectManager.eType)execute.buffEvt.battleOptionData.dataOnEndCall.effType);

                        if( execute.buffEvt.battleOptionData.dataOnEndCall.targetType == BattleOption.eBOTargetType.Self ) {
                            execute.buffEvt.battleOptionData.dataOnEndCall.evt.sender = m_owner;
                        }

                        execute.buffEvt.battleOptionData.dataOnEndCall.useTime = System.DateTime.Now;

                        EventMgr.Instance.SendEvent(execute.buffEvt.battleOptionData.dataOnEndCall.evt);
                        Log.Show(execute.buffEvt.battleOptionData.dataOnEndCall.evt.battleOptionData.battleOptionSetId + "번 배틀옵션셋 사용 (애드콜)!!!", Log.ColorType.Green);

                        break;
                    }
                }
                else
                {
                    m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.Speed, increaseType, id, execute.value, execute.curStackCount);
                    m_owner.SetSpeedRateByCalcBuff(execute.buffEvt.battleOptionData.toExecuteType, true, true);
                }
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);

        if ( skipCurAni && m_owner.curHp > 0.0f )
        {
            m_owner.SkipCurrentAni();
			m_owner.ResetBT();
		}
    }

    private bool OnRemoveUpdateSpeed(int id)
    {
        BattleOption.eToExecuteType toExecuteType = BattleOption.eToExecuteType.Unit;
        if(mDicExecute[id].buffEvt != null && mDicExecute[id].buffEvt.battleOptionData != null)
        {
            toExecuteType = mDicExecute[id].buffEvt.battleOptionData.toExecuteType;
        }

        m_owner.unitBuffStats.RemoveBuffStat(id);
        m_owner.SetSpeedRateByCalcBuff(toExecuteType, true, true);

        return true;
    }

    private IEnumerator UpdateSpeedUpOnEffect( int id, sExecute execute ) {
        execute.OnRemove = OnRemoveUpdateSpeedUpOnEffect;

        EffectManager.Instance.Play( m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType );
        ParticleSystem ps = EffectManager.Instance.GetEffectOrNull( execute.buffEvt.effId, EffectManager.eType.Common );
        BoxCollider boxCol = ps.GetComponent<BoxCollider>();

        execute.uiIconInfo.checkDuration = execute.uiIconInfo.duration;

        while( m_owner.curHp > 0.0f ) {
			execute.checkDuration += Time.fixedDeltaTime;
			if( execute.checkDuration >= ( execute.buffEvt.duration + execute.AddDuration ) ) {
				break;
			}

            if( World.Instance.ListPlayer.Count > 0 ) {
                for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                    bool isInBox = boxCol.bounds.Contains( World.Instance.ListPlayer[i].transform.position );

                    if( isInBox && World.Instance.ListPlayer[i].StayOnBoxCollider == null ) {
                        UIBuffDebuffIcon.sActiveIconInfo uiIconInfo = ShowOwnerBuffDebuffIcon( execute.buffEvt );
                        execute.uiIconInfo = uiIconInfo;

                        World.Instance.ListPlayer[i].StayOnBoxCollider = boxCol;

                        World.Instance.ListPlayer[i].unitBuffStats.AddBuffStatAndStackOnExecute( execute.buffEvt, UnitBuffStats.eBuffStatType.Speed, UnitBuffStats.eIncreaseType.Increase, id, execute.value, -1, false, true );
                        World.Instance.ListPlayer[i].SetSpeedRateByCalcBuff( execute.buffEvt.battleOptionData.toExecuteType, true, true );
                    }
                    else if( !isInBox && World.Instance.ListPlayer[i].StayOnBoxCollider ) {
                        World.Instance.ListPlayer[i].StayOnBoxCollider = null;

                        World.Instance.ListPlayer[i].unitBuffStats.RemoveBuffStat( id );
                        World.Instance.ListPlayer[i].SetSpeedRateByCalcBuff( execute.buffEvt.battleOptionData.toExecuteType, true, true );

                        execute.uiIconInfo.checkDuration = execute.uiIconInfo.duration;
                    }
                }
            }
            else {
                bool isInBox = boxCol.bounds.Contains( m_owner.transform.position );

                if( isInBox && m_owner.StayOnBoxCollider == null ) {
                    m_owner.StayOnBoxCollider = boxCol;

                    m_owner.unitBuffStats.AddBuffStatAndStackOnExecute( execute.buffEvt, UnitBuffStats.eBuffStatType.Speed, UnitBuffStats.eIncreaseType.Increase, id, execute.value, -1, false, true );
                    m_owner.SetSpeedRateByCalcBuff( execute.buffEvt.battleOptionData.toExecuteType, true, true );
                }
                else if( !isInBox && m_owner.StayOnBoxCollider ) {
                    m_owner.StayOnBoxCollider = null;

                    m_owner.unitBuffStats.RemoveBuffStat( id );
                    m_owner.SetSpeedRateByCalcBuff( execute.buffEvt.battleOptionData.toExecuteType, true, true );
                }
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute( id );
    }

    private bool OnRemoveUpdateSpeedUpOnEffect( int id ) {
        BattleOption.eToExecuteType toExecuteType = BattleOption.eToExecuteType.Unit;
        if( mDicExecute[id].buffEvt != null && mDicExecute[id].buffEvt.battleOptionData != null ) {
            toExecuteType = mDicExecute[id].buffEvt.battleOptionData.toExecuteType;
        }

        if( World.Instance.ListPlayer.Count > 0 ) {
            for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                World.Instance.ListPlayer[i].StayOnBoxCollider = null;
                World.Instance.ListPlayer[i].unitBuffStats.RemoveBuffStat( id );
                World.Instance.ListPlayer[i].SetSpeedRateByCalcBuff( toExecuteType, true, true );
            }
        }
        else {
            m_owner.StayOnBoxCollider = null;
            m_owner.unitBuffStats.RemoveBuffStat( id );
            m_owner.SetSpeedRateByCalcBuff( toExecuteType, true, true );
        }

        return true;
    }

    private IEnumerator UpdateSpeedDownToTarget(int id, sExecute execute)
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            execute.checkTick += Time.fixedDeltaTime;
            if(!Director.IsPlaying && !PortalEntry.IsAcrossByAniPlaying && execute.checkTick >= execute.buffEvt.tick)
            {
                if (execute.buffEvt.effId > 0)
                {
                    EffectManager.Instance.Play(m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
                }

                mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
                mBuffEvt.battleOptionData.buffIconFlash = true;
                mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;

                mBuffEvt.Set(mBuffEvt.battleOptionData.battleOptionSetId, eEventSubject.ActiveEnemiesInRange, eEventType.EVENT_DEBUFF_SPEED_DOWN, m_owner,
                             execute.buffEvt.battleOptionData.value, execute.buffEvt.battleOptionData.value3, 0.0f, execute.buffEvt.battleOptionData.value2, 0.0f,
                             mBuffEvt.effId, mBuffEvt.effId2, eBuffIconType.Debuff_Speed);

                EventMgr.Instance.SendEvent(mBuffEvt);
                execute.checkTick = 0.0f;
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateDmgRate(int id, sExecute execute, UnitBuffStats.eIncreaseType increaseType)
    {
        execute.OnRemove = OnRemoveBuffStat;

        m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.Damage, increaseType, id, execute.value, -1, false, true);

        if(execute.buffEvt.battleOptionData.value2 > 0.0f) {
            m_owner.TemporaryIgnoreHitAni = ( Unit.ignoreHitAniCond_t )execute.buffEvt.battleOptionData.value2;
        }

        if (execute.buffEvt.effId > 0)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if(execute.checkDuration >= (execute.buffEvt.duration + execute.AddDuration))
                {
                    break;
                }
                else if (execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnBoxCollider && m_owner.StayOnBoxCollider == null)
                {
                    break;
                }
            }
            else if (execute.buffEvt.duration == -2)
            {
                if (execute.StartOwnerAction == null || execute.StartOwnerAction.IsSkillEnd)
                {
                    break;
                }
            }
            else
            {
                if (execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnProjectile && m_owner.StayOnProjectile == null)
                {
                    break;
                }
                else if (execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnBoxCollider && m_owner.StayOnBoxCollider == null)
                {
                    break;
                }
                else if (execute.StartOwnerAction && execute.StartOwnerAction.ForceQuitBuffDebuff)
                {
                    break;
                }
            }

            if (execute.buffEvt.battleOptionData.preConditionType == BattleOption.eBOConditionType.HoldPosition)
            {
                if (m_owner.holdPositionRef <= 0)
                {
                    break;
                }
            }

            if (execute.changeStackCount)
            {
                m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.Damage, increaseType, id, execute.value, execute.curStackCount);
                execute.changeStackCount = false;
            }

            yield return mWaitForFixedUpdate;
        }

        m_owner.TemporaryIgnoreHitAni = Unit.ignoreHitAniCond_t.NONE;
        RemoveExecute(id);
    }

    private bool OnRemoveBuffStat(int id)
    {
        m_owner.unitBuffStats.RemoveBuffStat(id);
        return true;
    }

    private IEnumerator UpdateDmgRateDownWithAccumDamage(int id, sExecute execute, UnitBuffStats.eIncreaseType increaseType)
    {
        execute.OnRemove = OnRemoveDmgRateDownWithAccumDamage;

        m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.Damage, increaseType, id, execute.value);
        m_owner.SetMarkingType(Unit.eMarkingType.AccumDamageDown, 0.0f);

        if (execute.buffEvt.effId > 0)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if (execute.checkDuration >= (execute.buffEvt.duration + execute.AddDuration))
                {
                    break;
                }
            }
            else if (execute.buffEvt.duration == -2)
            {
                if (execute.StartOwnerAction == null || execute.StartOwnerAction.IsSkillEnd)
                {
                    break;
                }
            }
            else
            {
                if (execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnProjectile && m_owner.StayOnProjectile == null)
                {
                    break;
                }
                else if (execute.StartOwnerAction && execute.StartOwnerAction.ForceQuitBuffDebuff)
                {
                    break;
                }
            }

            if (execute.buffEvt.battleOptionData.preConditionType == BattleOption.eBOConditionType.HoldPosition)
            {
                if (m_owner.holdPositionRef <= 0)
                {
                    break;
                }
            }

            if (execute.changeStackCount)
            {
                m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.Damage, increaseType, id, execute.value, execute.curStackCount);
                execute.changeStackCount = false;
            }

            yield return mWaitForFixedUpdate;
        }

        if (execute.buffEvt.battleOptionData.addCallTiming == BattleOption.eBOAddCallTiming.OnSend)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.battleOptionData.dataOnEndCall.startEffId,
                                        (EffectManager.eType)execute.buffEvt.battleOptionData.dataOnEndCall.effType);

            execute.buffEvt.battleOptionData.dataOnEndCall.useTime = DateTime.Now;
            execute.buffEvt.battleOptionData.dataOnEndCall.value = m_owner.MarkingInfo.AccumValue;

            EventMgr.Instance.SendEvent(execute.buffEvt.battleOptionData.dataOnEndCall.evt);
        }

        RemoveExecute(id);
    }

    private bool OnRemoveDmgRateDownWithAccumDamage(int id)
    {
        m_owner.unitBuffStats.RemoveBuffStat(id);
        m_owner.SetMarkingType(Unit.eMarkingType.None, 0.0f);

        return true;
    }

	private IEnumerator UpdateHealRate( int id, sExecute execute, UnitBuffStats.eIncreaseType increaseType ) {
		execute.OnRemove = OnRemoveBuffStat;

		m_owner.unitBuffStats.AddBuffStatAndStackOnExecute( execute.buffEvt, UnitBuffStats.eBuffStatType.Heal, increaseType, id, execute.value, -1, 
                                                            false, true );

		if( execute.buffEvt.effId > 0 ) {
			EffectManager.Instance.Play( m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType );
		}

		while( m_owner.curHp > 0.0f && execute.checkDuration < ( execute.buffEvt.duration + execute.AddDuration ) ) {
			execute.checkDuration += Time.fixedDeltaTime;

			if( execute.changeStackCount ) {
				m_owner.unitBuffStats.AddBuffStatAndStackOnExecute( execute.buffEvt, UnitBuffStats.eBuffStatType.Heal, increaseType, id, execute.value, execute.curStackCount );
				execute.changeStackCount = false;
			}

			yield return mWaitForFixedUpdate;
		}

		RemoveExecute( id );
	}

    private IEnumerator UpdateHealOnDieInTime( int id, sExecute execute ) {
        execute.OnRemove = OnRemoveHealOnDieInTime;
        m_owner.HoldDying = true;

        if ( execute.buffEvt.effId > 0 ) {
            EffectManager.Instance.Play( m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType );
        }

        while ( execute.checkDuration < ( execute.buffEvt.duration + execute.AddDuration ) ) {
            execute.checkDuration += Time.fixedDeltaTime;

            if ( m_owner.curHp <= 0.0f ) {
                m_owner.cmptBuffDebuff.Clear();
                m_owner.AddHpPercentage( execute.buffEvt.battleOptionData.toExecuteType, execute.buffEvt.value, true );
                break;
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute( id );
    }

    private bool OnRemoveHealOnDieInTime( int id ) {
        m_owner.HoldDying = false;
        return true;
	}

    private IEnumerator UpdateSPAddRate(int id, sExecute execute, UnitBuffStats.eIncreaseType increaseType)
    {
        execute.OnRemove = OnRemoveBuffStat;

        m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.AddSPRate, increaseType, id, execute.value);

        if (execute.buffEvt.effId > 0)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f && execute.checkDuration < (execute.buffEvt.duration + execute.AddDuration))
        {
            execute.checkDuration += Time.fixedDeltaTime;

            if (execute.changeStackCount)
            {
                m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.AddSPRate, increaseType, id, execute.value, execute.curStackCount);
                execute.changeStackCount = false;
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateDebuffDurTime(int id, sExecute execute, UnitBuffStats.eIncreaseType increaseType)
    {
        execute.OnRemove = OnRemoveBuffStat;

        m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.DebuffDurTime, increaseType, id, execute.value);

        if (execute.buffEvt.effId > 0)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f && execute.checkDuration < (execute.buffEvt.duration + execute.AddDuration))
        {
            execute.checkDuration += Time.fixedDeltaTime;

            if (execute.changeStackCount)
            {
                m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.DebuffDurTime, increaseType, id, execute.value, execute.curStackCount);
                execute.changeStackCount = false;
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

	private IEnumerator UpdateAtkPowerRate( int id, sExecute execute, UnitBuffStats.eIncreaseType increaseType ) {
		execute.OnRemove = OnRemoveBuffStat;

        float rate = execute.value;
        m_owner.unitBuffStats.AddBuffStatAndStackOnExecute( execute.buffEvt, UnitBuffStats.eBuffStatType.AttackPower, increaseType, id, rate, -1, false, true );

		if( execute.buffEvt.effId > 0 ) {
			EffectManager.Instance.Play( m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType );
		}

        if( execute.buffEvt.effId2 > 0 ) {
            EffectManager.Instance.Play( m_owner, execute.buffEvt.effId2, (EffectManager.eType)execute.buffEvt.battleOptionData.effType );
        }

        if( execute.buffEvt.battleOptionData.addCallTiming == BattleOption.eBOAddCallTiming.OnSend ) {
            EffectManager.Instance.Play( m_owner, execute.buffEvt.battleOptionData.dataOnEndCall.startEffId,
                                        (EffectManager.eType)execute.buffEvt.battleOptionData.dataOnEndCall.effType );

            execute.buffEvt.battleOptionData.dataOnEndCall.useTime = DateTime.Now;
            EventMgr.Instance.SendEvent( execute.buffEvt.battleOptionData.dataOnEndCall.evt );
        }

        int incStackCount = 0;

		while( m_owner.curHp > 0.0f ) {
			if( execute.buffEvt.duration > 0.0f ) {
				execute.checkDuration += Time.fixedDeltaTime;
				if( execute.checkDuration >= ( execute.buffEvt.duration + execute.AddDuration ) ) {
					break;
				}
			}
			else if( execute.buffEvt.duration == -2 ) {
				if( execute.StartOwnerAction == null || execute.StartOwnerAction.IsSkillEnd ) {
					break;
				}
			}
			else {
				if( execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnProjectile && m_owner.StayOnProjectile == null ) {
					break;
				}
				else if( execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnBoxCollider && m_owner.StayOnBoxCollider == null ) {
					break;
				}
				else if( execute.StartOwnerAction && execute.StartOwnerAction.ForceQuitBuffDebuff ) {
					break;
				}
			}

            if( execute.buffEvt.tick > 0.0f ) { // tick이 0보다 크면 스탯 누적
                if( incStackCount < ( execute.buffEvt.value2 - 1 ) ) { // 최대 중첩 확인
                    execute.checkTick += Time.fixedDeltaTime;

                    if( execute.checkTick >= execute.buffEvt.tick ) {
                        execute.checkTick = 0.0f;

                        rate += execute.value;
                        m_owner.unitBuffStats.AddBuffStatAndStackOnExecute( execute.buffEvt, UnitBuffStats.eBuffStatType.AttackPower, increaseType, id, rate, -1, false, true );

                        ++incStackCount;

                        if( execute.buffEvt.effId2 > 0 ) {
                            EffectManager.Instance.Play( m_owner, execute.buffEvt.effId2, (EffectManager.eType)execute.buffEvt.battleOptionData.effType );
                        }
                    }
                }
			}
			else {
				if( execute.changeStackCount ) {
					m_owner.unitBuffStats.AddBuffStatAndStackOnExecute( execute.buffEvt, UnitBuffStats.eBuffStatType.AttackPower, increaseType, id, execute.value, execute.curStackCount, false, true );
					execute.changeStackCount = false;
				}
			}

			yield return mWaitForFixedUpdate;
		}

		RemoveExecute( id );
	}

	private IEnumerator UpdateAtkPowerToShieldRate(int id, sExecute execute, UnitBuffStats.eIncreaseType increaseType)
    {
        execute.OnRemove = OnRemoveBuffStat;

        m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.AttackPowerToShield, increaseType, id, execute.value);

        if (execute.buffEvt.effId > 0)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f && execute.checkDuration < (execute.buffEvt.duration + execute.AddDuration))
        {
            execute.checkDuration += Time.fixedDeltaTime;

            if (execute.changeStackCount)
            {
                m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.AttackPowerToShield, increaseType, id, execute.value, execute.curStackCount);
                execute.changeStackCount = false;
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateUltimateskillAtkPowerRate(int id, sExecute execute, UnitBuffStats.eIncreaseType increaseType)
    {
        execute.OnRemove = OnRemoveBuffStat;

        m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.UltimateSkillAtkPower, increaseType, id, execute.value);

        if (execute.buffEvt.effId > 0)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f && execute.checkDuration < (execute.buffEvt.duration + execute.AddDuration))
        {
            execute.checkDuration += Time.fixedDeltaTime;

            if (execute.changeStackCount)
            {
                m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.UltimateSkillAtkPower, increaseType, id, execute.value, execute.curStackCount);
                execute.changeStackCount = false;
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

	private IEnumerator UpdateCriticalRate( int id, sExecute execute, UnitBuffStats.eIncreaseType increaseType ) {
		execute.OnRemove = OnRemoveBuffStat;

		m_owner.unitBuffStats.AddBuffStatAndStackOnExecute( execute.buffEvt, UnitBuffStats.eBuffStatType.CriticalRate, increaseType, id, execute.value );

		if( execute.buffEvt.effId > 0 ) {
			EffectManager.Instance.Play( m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType );
		}

		while( m_owner.curHp > 0.0f ) {
			if( execute.buffEvt.duration > 0.0f ) {
				execute.checkDuration += Time.fixedDeltaTime;
				if( execute.checkDuration >= ( execute.buffEvt.duration + execute.AddDuration ) ) {
					break;
				}
				else if( execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnBoxCollider && m_owner.StayOnBoxCollider == null ) {
					break;
				}
			}
			else if( execute.buffEvt.duration == -2 ) {
				if( execute.StartOwnerAction == null || execute.StartOwnerAction.IsSkillEnd ) {
					break;
				}
			}
			else {
				if( execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnProjectile && m_owner.StayOnProjectile == null ) {
					break;
				}
				else if( execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnBoxCollider && m_owner.StayOnBoxCollider == null ) {
					break;
				}
				else if( execute.StartOwnerAction && execute.StartOwnerAction.ForceQuitBuffDebuff ) {
					break;
				}
			}

			if( execute.changeStackCount ) {
				m_owner.unitBuffStats.AddBuffStatAndStackOnExecute( execute.buffEvt, UnitBuffStats.eBuffStatType.CriticalRate, increaseType, id, execute.value, execute.curStackCount );
				execute.changeStackCount = false;
			}

			yield return mWaitForFixedUpdate;
		}

		RemoveExecute( id );
	}

	private IEnumerator UpdateCriticalDmg(int id, sExecute execute, UnitBuffStats.eIncreaseType increaseType)
    {
        execute.OnRemove = OnRemoveBuffStat;

        m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.CriticalDmg, increaseType, id, execute.value);

        if (execute.buffEvt.effId > 0)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f && execute.checkDuration < (execute.buffEvt.duration + execute.AddDuration))
        {
            execute.checkDuration += Time.fixedDeltaTime;

            if (execute.changeStackCount)
            {
                m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.CriticalDmg, increaseType, id, execute.value, execute.curStackCount);
                execute.changeStackCount = false;
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateCriticalResist(int id, sExecute execute, UnitBuffStats.eIncreaseType increaseType)
    {
        execute.OnRemove = OnRemoveBuffStat;

        m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.CriticalResist, increaseType, id, execute.value);

        if (execute.buffEvt.effId > 0)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f && execute.checkDuration < (execute.buffEvt.duration + execute.AddDuration))
        {
            execute.checkDuration += Time.fixedDeltaTime;

            if (execute.changeStackCount)
            {
                m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.CriticalResist, increaseType, id, execute.value, execute.curStackCount);
                execute.changeStackCount = false;
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateCriticalDef(int id, sExecute execute, UnitBuffStats.eIncreaseType increaseType)
    {
        execute.OnRemove = OnRemoveBuffStat;

        m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.CriticalDef, increaseType, id, execute.value);

        if (execute.buffEvt.effId > 0)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f && execute.checkDuration < (execute.buffEvt.duration + execute.AddDuration))
        {
            execute.checkDuration += Time.fixedDeltaTime;

            if (execute.changeStackCount)
            {
                m_owner.unitBuffStats.AddBuffStatAndStackOnExecute(execute.buffEvt, UnitBuffStats.eBuffStatType.CriticalDef, increaseType, id, execute.value, execute.curStackCount);
                execute.changeStackCount = false;
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdatePenetrate( int id, sExecute execute, UnitBuffStats.eIncreaseType increaseType ) {
        execute.OnRemove = OnRemovePenetrate;

        m_owner.AddPenetrate( execute.value );

        if( execute.buffEvt.effId > 0 ) {
            EffectManager.Instance.Play( m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType );
        }

        if( execute.buffEvt.effId2 > 0 ) {
            EffectManager.Instance.Play( m_owner, execute.buffEvt.effId2, (EffectManager.eType)execute.buffEvt.battleOptionData.effType );
        }

        while( m_owner.curHp > 0.0f ) {
            if( execute.buffEvt.duration > 0.0f ) {
                execute.checkDuration += Time.fixedDeltaTime;
                if( execute.checkDuration >= ( execute.buffEvt.duration + execute.AddDuration ) ) {
                    break;
                }
            }
            else if( execute.buffEvt.duration == -2 ) {
                if( execute.StartOwnerAction == null || execute.StartOwnerAction.IsSkillEnd ) {
                    break;
                }
            }
            else {
                if( execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnProjectile && m_owner.StayOnProjectile == null ) {
                    break;
                }
                else if( execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnBoxCollider && m_owner.StayOnBoxCollider == null ) {
                    break;
                }
                else if( execute.StartOwnerAction && execute.StartOwnerAction.ForceQuitBuffDebuff ) {
                    break;
                }
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute( id );
    }

    private bool OnRemovePenetrate( int id ) {
        sExecute execute = mDicExecute[id];
        m_owner.AddPenetrate( -execute.value );

        return true;
    }

    private IEnumerator UpdateSufferance( int id, sExecute execute, UnitBuffStats.eIncreaseType increaseType ) {
        execute.OnRemove = OnRemoveSufferance;

        m_owner.AddSufferance( execute.value );

        if( execute.buffEvt.effId > 0 ) {
            EffectManager.Instance.Play( m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType );
        }

        if( execute.buffEvt.effId2 > 0 ) {
            EffectManager.Instance.Play( m_owner, execute.buffEvt.effId2, (EffectManager.eType)execute.buffEvt.battleOptionData.effType );
        }

        while( m_owner.curHp > 0.0f ) {
            if( execute.buffEvt.duration > 0.0f ) {
                execute.checkDuration += Time.fixedDeltaTime;
                if( execute.checkDuration >= ( execute.buffEvt.duration + execute.AddDuration ) ) {
                    break;
                }
            }
            else if( execute.buffEvt.duration == -2 ) {
                if( execute.StartOwnerAction == null || execute.StartOwnerAction.IsSkillEnd ) {
                    break;
                }
            }
            else {
                if( execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnProjectile && m_owner.StayOnProjectile == null ) {
                    break;
                }
                else if( execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnBoxCollider && m_owner.StayOnBoxCollider == null ) {
                    break;
                }
                else if( execute.StartOwnerAction && execute.StartOwnerAction.ForceQuitBuffDebuff ) {
                    break;
                }
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute( id );
    }

    private bool OnRemoveSufferance( int id ) {
        sExecute execute = mDicExecute[id];
        m_owner.AddSufferance( -execute.value );

        return true;
    }

    private IEnumerator UpdateHoldPosition(int id, sExecute execute)
    {
        execute.OnRemove = ReleaseHoldPosition;

        ++m_owner.holdPositionRef;

        bool setKinematicLate = false;
        if (m_owner.isGrounded)
        {
            m_owner.SetKinematicRigidBody();
        }
        else
        {
            setKinematicLate = true;
        }

        //bool playHideEffect = false;
        if (execute.buffEvt.effId > 0)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if (execute.checkDuration >= (execute.buffEvt.duration + execute.AddDuration))
                {
                    break;
                }

                if(setKinematicLate && m_owner.isGrounded)
                {
                    m_owner.SetKinematicRigidBody();
                    setKinematicLate = false;
                }

                /*
                if (!playHideEffect && execute.checkDuration >= execute.buffEvt.duration - 0.5f && execute.buffEvt.effId2 > 0)
                {
                    EffectManager.Instance.Play(m_owner, execute.buffEvt.effId2, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
                    playHideEffect = true;
                }
                */
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id, true);
    }

    private bool ReleaseHoldPosition(int id)
    {
        m_owner.holdPositionRef = Mathf.Max(0, m_owner.holdPositionRef - 1);
        if(m_owner.holdPositionRef > 0)
        {
            return false;
        }

        if (!Director.IsPlaying)
        {
            m_owner.SetKinematicRigidBody(false);
        }

        if (mDicExecute.ContainsKey(id))
        {
            sExecute execute = mDicExecute[id];

            if (execute.buffEvt.effId > 0)
            {
                EffectManager.Instance.StopEffImmediate(execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType, null);
            }

            if (execute.buffEvt.effId2 > 0)
            {
                EffectManager.Instance.Play(m_owner, execute.buffEvt.effId2, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
            }
        }

        return true;
    }

    private IEnumerator UpdateSkipAttack(int id, sExecute execute)
    {
        execute.OnRemove = OnRemoveSkipAttack;
        m_owner.skipAttack = true;

        if (execute.buffEvt.effId > 0)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if (execute.checkDuration >= (execute.buffEvt.duration + execute.AddDuration))
                {
                    break;
                }
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private bool OnRemoveSkipAttack(int id)
    {
        m_owner.skipAttack = false;
        return true;
    }

    private IEnumerator UpdateShieldDmg(int id, sExecute execute)
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f && m_owner.curShield > 0.0f && execute.checkDuration < (execute.buffEvt.duration + execute.AddDuration))
        {
            execute.checkDuration += Time.fixedDeltaTime;
            m_owner.SubShield((execute.value * execute.curStackCount) / (execute.buffEvt.tick / Time.fixedDeltaTime));

            yield return mWaitForFixedUpdate;
        }

        if (m_owner.curShield <= 0.0f && execute.buffEvt.battleOptionData != null && execute.buffEvt.battleOptionData.dataOnEndCall != null)
        {
            if(execute.buffEvt.battleOptionData.effId1 > 0)
            {
                EffectManager.Instance.Play(m_owner, execute.buffEvt.sender, execute.buffEvt.battleOptionData.effId1,
                                            (EffectManager.eType)execute.buffEvt.battleOptionData.effType,
                                            execute.buffEvt.battleOptionData.effId2, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
            }
            
            execute.buffEvt.battleOptionData.dataOnEndCall.evt.value = execute.buffEvt.battleOptionData.dataOnEndCall.value * execute.curStackCount;
            EventMgr.Instance.SendEvent(execute.buffEvt.battleOptionData.dataOnEndCall.evt);
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateReflection(int id, sExecute execute)
    {
        int rand = execute.buffEvt.battleOptionData.randomStart;

        bool checkHit = false;
        int reflectionCount = 0;
        float atkRatio = 0.0f;
        int playEffID = 0;

        AniEvent.sEvent evt = new AniEvent.sEvent();
        evt.behaviour = eBehaviour.Attack;
        evt.hitEffectId = 1;
        evt.hitDir = eHitDirection.None;
        evt.atkDirection = eAttackDirection.Skip;
        evt.atkRatio = 1.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if (execute.checkDuration >= (execute.buffEvt.duration + execute.AddDuration))
                {
                    break;
                }
            }

            if (!checkHit && m_owner.attacker && m_owner.actionSystem.IsCurrentAction(eActionCommand.Hit))
            {
                if (rand > 0 && UnityEngine.Random.Range(0, 100) >= rand)
                {
                    checkHit = true;
                    continue;
                }

                ++reflectionCount;
                if (execute.buffEvt.value3 > 0 && reflectionCount >= execute.buffEvt.value3)
                {
                    evt.behaviour = (eBehaviour)execute.buffEvt.value2;
                    reflectionCount = 0;
                    playEffID = execute.buffEvt.effId2;
                    atkRatio = execute.value + execute.value;
                }
                else
                {
                    evt.behaviour = eBehaviour.Attack;
                    playEffID = execute.buffEvt.effId;
                    atkRatio = execute.value;
                }

                if ((EffectManager.eType)execute.buffEvt.battleOptionData.effType == EffectManager.eType.Common)
                    EffectManager.Instance.Play(m_owner, playEffID, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
                else
                    EffectManager.Instance.Play(m_owner.attacker, playEffID, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);

                //m_owner.attacker.OnHit(m_owner, evt, m_owner.attacker.attackPower * execute.value, false, ref hitState);
                mAtkEvt.SetWithSingleTarget(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, execute.buffEvt.battleOptionData.toExecuteType, evt, 
                                            m_owner.attackPower * atkRatio, eAttackDirection.Skip, false, 0, EffectManager.eType.None, 
                                            m_owner.attacker.MainCollider, 0.0f, true);

                EventMgr.Instance.SendEvent(mAtkEvt);

                checkHit = true;
            }
            else if (checkHit && !m_owner.actionSystem.IsCurrentAction(eActionCommand.Hit))
            {
                checkHit = false;
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateReflectionAround(int id, sExecute execute)
    {
        int rand = execute.buffEvt.battleOptionData.randomStart;

        bool checkHit = false;
        int reflectionCount = 0;
        float atkRatio = 0.0f;
        int playEffID = 0;

        AniEvent.sEvent evt = new AniEvent.sEvent();
        evt.behaviour = eBehaviour.Attack;
        evt.hitEffectId = 1;
        evt.hitDir = eHitDirection.None;
        evt.atkDirection = eAttackDirection.Around;
        evt.atkDirAngle = 360.0f;
        evt.atkRange = execute.buffEvt.battleOptionData.targetValue;
        evt.atkRatio = 1.0f;

        if(execute.buffEvt.value3 <= 0.0f && execute.buffEvt.value2 > 0.0f)
        {
            evt.behaviour = (eBehaviour)execute.buffEvt.value2;
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if (execute.checkDuration >= (execute.buffEvt.duration + execute.AddDuration))
                {
                    break;
                }
            }

            if (!checkHit && m_owner.attacker && m_owner.actionSystem.IsCurrentAction(eActionCommand.Hit))
            {
                if(rand > 0 && UnityEngine.Random.Range(0, 100) >= rand)
                {
                    checkHit = true;
                    continue;
                }

                ++reflectionCount;
                if (execute.buffEvt.value3 > 0 && reflectionCount >= execute.buffEvt.value3)
                {
                    evt.behaviour = (eBehaviour)execute.buffEvt.value2;
                    reflectionCount = 0;
                    playEffID = execute.buffEvt.effId2;
                    atkRatio = execute.value + execute.value;
                }
                else
                {
                    evt.behaviour = eBehaviour.Attack;
                    playEffID = execute.buffEvt.effId;
                    atkRatio = execute.value;
                }

                if ((EffectManager.eType)execute.buffEvt.battleOptionData.effType == EffectManager.eType.Common)
                    EffectManager.Instance.Play(m_owner, playEffID, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
                else
                    EffectManager.Instance.Play(m_owner.attacker, playEffID, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);

                m_owner.OnAttack(evt, atkRatio, true);
                checkHit = true;

                if(execute.buffEvt.battleOptionData.addCallTiming == BattleOption.eBOAddCallTiming.OnSend)
                {
                    EffectManager.Instance.Play(m_owner, execute.buffEvt.battleOptionData.dataOnEndCall.startEffId, 
                                                (EffectManager.eType)execute.buffEvt.battleOptionData.dataOnEndCall.effType);

                    execute.buffEvt.battleOptionData.dataOnEndCall.useTime = DateTime.Now;
                    EventMgr.Instance.SendEvent(execute.buffEvt.battleOptionData.dataOnEndCall.evt);
                }
            }
            else if (checkHit && !m_owner.actionSystem.IsCurrentAction(eActionCommand.Hit))
            {
                checkHit = false;
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateRandomKnockback(int id, sExecute execute)
    {
        AniEvent.sEvent evt = new AniEvent.sEvent();
        evt.behaviour = eBehaviour.KnockBackAttack;
        evt.hitEffectId = 1;
        evt.hitDir = eHitDirection.None;
        evt.atkDirection = eAttackDirection.Around;
        evt.atkDirAngle = 360.0f;
        evt.atkRange = execute.buffEvt.battleOptionData.targetValue;
        evt.atkRatio = 1.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            execute.checkTick += Time.fixedDeltaTime;
            if (execute.checkTick >= execute.buffEvt.tick)
            {
                execute.checkTick = 0.0f;
                if (UnityEngine.Random.Range(0, 100) <= (int)execute.value)
                {
                    m_owner.OnAttack(evt, 1.0f, false);
                }
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateSuperArmor(int id, sExecute execute)
    {
        execute.OnRemove = ReleaseSuperArmor;
        execute.ChangedSuperArmorId = m_owner.SetSuperArmor((Unit.eSuperArmor)execute.value);

        if (execute.buffEvt.effId > 0)
        {
            ParticleSystem effectParticle = EffectManager.Instance.GetEffectOrNull( execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType );
            if ( effectParticle != null ) {
                if ( effectParticle.gameObject.layer != m_owner.gameObject.layer ) {
                    Utility.SetLayer( effectParticle.gameObject, m_owner.gameObject.layer, true );
                }
            }

            EffectManager.Instance.Play(m_owner, execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType, 0.0f, true);
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if (execute.checkDuration >= (execute.buffEvt.duration + execute.AddDuration))
                {
                    break;
                }
            }
            else if (execute.buffEvt.duration == -2)
            {
                if (execute.StartOwnerAction == null || execute.StartOwnerAction.IsSkillEnd)
                {
                    break;
                }
            }
            else if (execute.StartOwnerAction && execute.StartOwnerAction.ForceQuitBuffDebuff)
            {
                break;
            }

            yield return mWaitForFixedUpdate;
        }

        if (execute.buffEvt.effId > 0)
        {
            EffectManager.Instance.StopEffImmediate(execute.buffEvt.effId, (EffectManager.eType)execute.buffEvt.battleOptionData.effType, null);
        }

        RemoveExecute(id);
    }

    private bool ReleaseSuperArmor(int id)
    {
        sExecute execute = mDicExecute[id];
        if(execute == null)
        {
            Debug.LogError("Execute 객체가 먼저 지워지면 안되는디?????");
            return false;
        }

        m_owner.RestoreSuperArmor(execute.ChangedSuperArmorId);
        return true;
    }

    private IEnumerator UpdateConditionalSuperArmor(int id, sExecute execute)
    {
        m_owner.ConditionalSuperArmor.Set((Unit.ESuperArmorConditionType)execute.buffEvt.value, (int)execute.buffEvt.value2, (int)execute.buffEvt.value2);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if (execute.checkDuration >= (execute.buffEvt.duration + execute.AddDuration))
                {
                    break;
                }
            }

            yield return mWaitForFixedUpdate;
        }

        m_owner.ConditionalSuperArmor.Set(Unit.ESuperArmorConditionType.NONE, 0, 0);
        RemoveExecute(id);
    }

    private IEnumerator UpdateSendHoldPositionToTarget(int id, sExecute execute)
    {
        int rand = execute.buffEvt.battleOptionData.randomStart;
        bool checkUse = false;
        /* eEventSubject.ActiveEnemiesInRange evt 구성
         * evt.value = 효과
         * evt.value2 = 범위
         * evt.value3 = 인원
        */
        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
        mBuffEvt.battleOptionData.effType = execute.buffEvt.battleOptionData.effType;
        mBuffEvt.battleOptionData.preConditionType = execute.buffEvt.battleOptionData.preConditionType;
        mBuffEvt.battleOptionData.conditionType = execute.buffEvt.battleOptionData.conditionType;
        mBuffEvt.battleOptionData.atkConditionType = execute.buffEvt.battleOptionData.atkConditionType;
        mBuffEvt.Set(execute.buffEvt.id, eEventSubject.ActiveEnemiesInRange, eEventType.EVENT_DEBUFF_HOLD_POSITION, m_owner, 0.0f,
                     execute.buffEvt.value2, execute.buffEvt.value3, execute.buffEvt.value, 0.0f, execute.buffEvt.effId, execute.buffEvt.effId2,
                     eBuffIconType.Debuff_Nomove);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if (execute.checkDuration >= (execute.buffEvt.duration + execute.AddDuration))
                {
                    break;
                }
            }

            execute.checkTick += Time.fixedDeltaTime;
            if (execute.checkTick >= execute.buffEvt.tick)
            {
                execute.checkTick = 0.0f;

                if (rand > -1)
                {
                    if (UnityEngine.Random.Range(0, 100) < rand)
                        checkUse = true;
                }
                else
                    checkUse = true;

                if (checkUse == true)
                {
                    EventMgr.Instance.SendEvent(mBuffEvt);
                    checkUse = false;
                }
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateDecreaseSP(int id, sExecute execute)
    {
        float fChargePerSec = GameInfo.Instance.BattleConfig.USMaxSP / GameInfo.Instance.BattleConfig.USSPRegenSpeedTime;

        Player player = m_owner as Player;
        if (player)
        {
            fChargePerSec += fChargePerSec * player.SPRegenIncRate;
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if (execute.checkDuration >= (execute.buffEvt.duration + execute.AddDuration))
                {
                    break;
                }
            }
            else if(m_owner.curSp <= fChargePerSec)
            {
                break;
            }

            execute.checkTick += Time.fixedDeltaTime;
            if (execute.checkTick >= execute.buffEvt.tick)
            {
                m_owner.UseSp(execute.value, true);
                execute.checkTick = 0.0f;
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateMovementInverse(int id, sExecute execute)
    {
        execute.OnRemove = OnRemoveMovementInverse;

        //AppMgr.Instance.CustomInput.InverseYAxis = true;
        //AppMgr.Instance.CustomInput.InverseXAxis = true;
        m_owner.InverseXAxis = true;
        m_owner.InverseYAxis = true;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if (execute.checkDuration >= (execute.buffEvt.duration + execute.AddDuration))
                {
                    break;
                }
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private bool OnRemoveMovementInverse(int id)
    {
        //AppMgr.Instance.CustomInput.InverseYAxis = false;
        //AppMgr.Instance.CustomInput.InverseXAxis = false;
        m_owner.InverseXAxis = false;
        m_owner.InverseYAxis = false;

        return true;
    }

    private IEnumerator UpdateSendDotDmgToTarget(int id, sExecute execute)
    {
        int rand = execute.buffEvt.battleOptionData.randomStart;
        bool checkUse = false;
        /* eEventSubject.ActiveEnemiesInRange evt 구성
         * evt.value = 효과
         * evt.value2 = 범위
         * evt.value3 = 인원
        */
        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
        mBuffEvt.battleOptionData.effType = execute.buffEvt.battleOptionData.effType;
        mBuffEvt.battleOptionData.preConditionType = execute.buffEvt.battleOptionData.preConditionType;
        mBuffEvt.battleOptionData.conditionType = execute.buffEvt.battleOptionData.conditionType;
        mBuffEvt.battleOptionData.atkConditionType = execute.buffEvt.battleOptionData.atkConditionType;
        mBuffEvt.Set(execute.buffEvt.id, eEventSubject.ActiveEnemiesInRange, eEventType.EVENT_DEBUFF_DOT_DMG, m_owner, execute.buffEvt.value,
                     execute.buffEvt.value2, execute.buffEvt.battleOptionData.targetValue, execute.buffEvt.duration, execute.buffEvt.value3, 
                     execute.buffEvt.effId, execute.buffEvt.effId2, eBuffIconType.Debuff_Bleeding);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            /*
             * Duration 값을 도트 데미지에서 쓰기 때문에 이 while문은 무조건 (오너가 살아있는 한)무한이라고 가정함.
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if (execute.checkDuration >= execute.buffEvt.duration)
                {
                    break;
                }
            }
            */

            execute.checkTick += Time.fixedDeltaTime;
            if (execute.checkTick >= execute.buffEvt.tick)
            {
                execute.checkTick = 0.0f;

                if (rand > -1)
                {
                    if (UnityEngine.Random.Range(0, 100) < rand)
                        checkUse = true;
                }
                else
                    checkUse = true;

                if (checkUse == true)
                {
                    EventMgr.Instance.SendEvent(mBuffEvt);
                    checkUse = false;
                }
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateSendAttackToTarget(int id, sExecute execute)
    {
        int rand = execute.buffEvt.battleOptionData.randomStart;
        bool checkUse = false;
        /* eEventSubject.ActiveEnemiesInRange evt 구성
         * evt.value = 효과
         * evt.value2 = 범위
         * evt.value3 = 인원
        */
        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
        mBuffEvt.battleOptionData.effType = execute.buffEvt.battleOptionData.effType;
        mBuffEvt.battleOptionData.preConditionType = execute.buffEvt.battleOptionData.preConditionType;
        mBuffEvt.battleOptionData.conditionType = execute.buffEvt.battleOptionData.conditionType;
        mBuffEvt.battleOptionData.atkConditionType = execute.buffEvt.battleOptionData.atkConditionType;
        mBuffEvt.Set(execute.buffEvt.id, eEventSubject.ActiveEnemiesInRange, eEventType.EVENT_ACTION_HIT_ATTACK, m_owner, execute.buffEvt.value,
                     execute.buffEvt.value2, execute.buffEvt.battleOptionData.targetValue, execute.buffEvt.duration, execute.buffEvt.value3,
                     execute.buffEvt.effId, execute.buffEvt.effId2, eBuffIconType.None);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            /*
             * Duration 값을 도트 데미지에서 쓰기 때문에 이 while문은 무조건 (오너가 살아있는 한)무한이라고 가정함.
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if (execute.checkDuration >= execute.buffEvt.duration)
                {
                    break;
                }
            }
            */

            execute.checkTick += Time.fixedDeltaTime;
            if (execute.checkTick >= execute.buffEvt.tick)
            {
                execute.checkTick = 0.0f;

                if (rand > -1)
                {
                    if (UnityEngine.Random.Range(0, 100) < rand)
                        checkUse = true;
                }
                else
                    checkUse = true;

                if (checkUse == true)
                {
                    EventMgr.Instance.SendEvent(mBuffEvt);
                    checkUse = false;
                }
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator WaitRepeatAndExecute(int id, sExecute execute)
    {
		if (!mDicExecuteRepeat.ContainsKey(id))
		{
			mDicExecuteRepeat.Add(id, execute);
		}

        float checkTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(checkTime < execute.repeatDelay)
        {
            checkTime += Time.fixedDeltaTime;
            yield return mWaitForFixedUpdate;
        }

        //yield return new WaitForSeconds(execute.repeatDelay);
        Execute(execute.buffEvt, execute.OnEnd);

        mDicExecuteRepeat.Remove(id);
    }

	private IEnumerator UpdateFreeze( int id, sExecute execute ) {
		execute.OnRemove = OnRemoveUpdateFreeze;

		m_owner.unitBuffStats.AddBuffStatAndStackOnExecute( execute.buffEvt, UnitBuffStats.eBuffStatType.Speed, UnitBuffStats.eIncreaseType.Decrease,
														    id, 10.0f, -1, false, true );

		m_owner.SetSpeedRateByCalcBuff( execute.buffEvt.battleOptionData.toExecuteType, true, false );

		EffectManager.Instance.Play( m_owner, 8001, EffectManager.eType.Common, 0.0f, true );

		Color color = new Color(0.6f, 0.85f, 0.92f);
		m_owner.aniEvent.SetFlash( color, 0.0f, 0.2f, execute.buffEvt.duration );

		while( m_owner.curHp > 0.0f ) {
			if( execute.buffEvt.duration > 0.0f ) {
				execute.checkDuration += Time.fixedDeltaTime;

				if( execute.checkDuration >= ( execute.buffEvt.duration + execute.AddDuration ) ) {
					break;
				}
			}
			else if( execute.buffEvt.duration == -2 ) {
				if( execute.StartOwnerAction == null || execute.StartOwnerAction.IsSkillEnd ) {
					break;
				}
			}
			else if( execute.StartOwnerAction && execute.StartOwnerAction.ForceQuitBuffDebuff ) {
				break;
			}

			if( execute.changeStackCount ) {
				m_owner.unitBuffStats.AddBuffStatAndStackOnExecute( execute.buffEvt, UnitBuffStats.eBuffStatType.Speed, UnitBuffStats.eIncreaseType.Decrease,
																   id, 10.0f, execute.curStackCount );
				execute.changeStackCount = false;

				m_owner.SetSpeedRateByCalcBuff( execute.buffEvt.battleOptionData.toExecuteType, true, false );
			}

			if( m_owner.aniEvent.GetShaderColor( "_RimColor", 0 ) != color ) {
				m_owner.aniEvent.SetShaderColor( "_RimColor", color );
				m_owner.aniEvent.SetShaderFloat( "_RimMin", 0.0f );
				m_owner.aniEvent.SetShaderFloat( "_RimMax", 0.2f );
			}

			yield return mWaitForFixedUpdate;
		}

		RemoveExecute( id );
	}

	private bool OnRemoveUpdateFreeze(int id)
    {
        BattleOption.eToExecuteType toExecuteType = BattleOption.eToExecuteType.Unit;
        if (mDicExecute[id].buffEvt != null && mDicExecute[id].buffEvt.battleOptionData != null)
        {
            toExecuteType = mDicExecute[id].buffEvt.battleOptionData.toExecuteType;
        }

        EffectManager.Instance.StopEffImmediate(8001, EffectManager.eType.Common, m_owner.transform);

        m_owner.unitBuffStats.RemoveBuffStat(id);
        m_owner.SetSpeedRateByCalcBuff(toExecuteType, true, false);
        m_owner.aniEvent.SetOriginalShaderColor();

        return true;
    }

    private IEnumerator UpdateCommandCloneAttack2(int id, sExecute execute)
    {
        execute.OnRemove = null;

        ActionBase action = m_owner.actionSystem.GetActionOrNull(x => x.IsCommandCloneAttack2);
        if(action == null)
        {
            yield break;
        }

        float checkInterval = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            Unit clone = action.owner.GetActiveClone();
            if (clone && !clone.UseAttack02)
            {
                checkInterval += Time.fixedDeltaTime;
                if (checkInterval >= execute.buffEvt.value2)
                {
                    if (UnityEngine.Random.Range(0, 100) < execute.buffEvt.value)
                    {
                        clone.UseAttack02 = true;
                        clone.SetIncreaseSkillAtkValue(execute.buffEvt.value3);

                        clone.actionSystem.CancelCurrentAction();
                        clone.ResetBT();
                    }

                    checkInterval = 0.0f;
                }
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private IEnumerator UpdateMarking(int id, sExecute execute)
    {
        execute.OnRemove = RemoveGetHpMarking;

        Unit.eMarkingType markingType = (Unit.eMarkingType)execute.buffEvt.value2;
        m_owner.SetMarkingType(markingType, execute.buffEvt.value, execute.buffEvt.value3, 0.0f, execute.buffEvt.effId);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if (execute.checkDuration >= (execute.buffEvt.duration + execute.AddDuration))
                {
                    break;
                }
				else if (execute.buffEvt.battleOptionData.conditionType == BattleOption.eBOConditionType.StayOnBoxCollider && m_owner.StayOnBoxCollider == null)
				{
					break;
				}
			}
            else if (execute.buffEvt.duration == -2)
            {
                if (execute.StartOwnerAction == null || execute.StartOwnerAction.IsSkillEnd)
                {
                    break;
                }
            }
            else if (execute.StartOwnerAction && execute.StartOwnerAction.ForceQuitBuffDebuff)
            {
                break;
            }

			yield return mWaitForFixedUpdate;
        }

        bool manualHideEff = false;

        if(markingType == Unit.eMarkingType.EndOfMarkingAttack)
        {
            mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
            mBuffEvt.battleOptionData.effId2 = -1;
            mBuffEvt.battleOptionData.value3 = 0.2f;

            mBuffEvt.Set(execute.buffEvt.battleOptionData.battleOptionSetId, eEventSubject.Self, eEventType.EVENT_ACTION_HIT_ATTACK, m_owner,
                         execute.buffEvt.value, 0.0f, 0.0f, 0.0f, 0.0f, 0, 0, eBuffIconType.None, execute.buffEvt.sender.attackPower);

            EffectManager.Instance.Play(m_owner, execute.buffEvt.battleOptionData.effId1, EffectManager.eType.Each_Monster_Normal_Hit);
            EventMgr.Instance.SendEvent(mBuffEvt);

            manualHideEff = true;

            /*
            mBuffEvt.SelectTarget = m_owner;

            mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
            mBuffEvt.battleOptionData.effId2 = -1;
            mBuffEvt.battleOptionData.value3 = 0.2f;
            
            mBuffEvt.Set(execute.buffEvt.battleOptionData.battleOptionSetId, eEventSubject.SelectTarget, eEventType.EVENT_ACTION_HIT_ATTACK, execute.buffEvt.sender,
                         execute.buffEvt.value, 0.0f, 0.0f, 0.0f, 0.0f, 0, 0, eBuffIconType.None, execute.buffEvt.sender.attackPower, 0.0f);

            EffectManager.Instance.Play(m_owner, execute.buffEvt.battleOptionData.effId1, EffectManager.eType.Each_Monster_Normal_Hit);
            EventMgr.Instance.SendEvent(mBuffEvt);

            manualHideEff = true;
            */
        }
        else if(markingType == Unit.eMarkingType.EndOfMarkingRangeAttack)
        {
            mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
            mBuffEvt.battleOptionData.effId2 = -1;
            mBuffEvt.battleOptionData.value3 = 0.2f;

            mBuffEvt.Set(execute.buffEvt.battleOptionData.battleOptionSetId, eEventSubject.ActiveEnemiesInRange, eEventType.EVENT_ACTION_HIT_ATTACK, m_owner, 
                         execute.buffEvt.value, execute.buffEvt.battleOptionData.targetValue, 0.0f, 0.0f, 0.0f, 0, 0, eBuffIconType.None, 
                         execute.buffEvt.sender.attackPower, 1.0f);

            EffectManager.Instance.Play(m_owner, execute.buffEvt.battleOptionData.effId1, EffectManager.eType.Each_Monster_Normal_Hit);
            EventMgr.Instance.SendEvent(mBuffEvt);

            manualHideEff = true;
        }

        RemoveExecute(id, manualHideEff);
    }

    private bool RemoveGetHpMarking(int id)
    {
        m_owner.SetMarkingType(Unit.eMarkingType.None, 0.0f);
        return true;
    }

    private IEnumerator UpdateHoldSupporterSkillCoolTime(int id, sExecute execute)
    {
        execute.OnRemove = RemoveHoldSupporterSkillCoolTime;

        m_owner.HoldSupporterSkillCoolTime = true;

        if (execute.buffEvt.battleOptionData.effId1 > 0)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.battleOptionData.effId1, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            if (execute.buffEvt.duration > 0.0f)
            {
                execute.checkDuration += Time.fixedDeltaTime;
                if (execute.checkDuration >= (execute.buffEvt.duration + execute.AddDuration))
                {
                    break;
                }
            }
            else if (execute.buffEvt.duration == -2)
            {
                if (execute.StartOwnerAction == null || execute.StartOwnerAction.IsSkillEnd)
                {
                    break;
                }
            }
            else if (execute.StartOwnerAction && execute.StartOwnerAction.ForceQuitBuffDebuff)
            {
                break;
            }

            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

    private bool RemoveHoldSupporterSkillCoolTime(int id)
    {
        m_owner.HoldSupporterSkillCoolTime = false;
        return true;
    }

	private IEnumerator UpdateBlackhole( int id, sExecute execute ) {
		if ( m_owner == null || execute == null || execute.buffEvt == null || execute.buffEvt.battleOptionData == null ) {
			yield break;
		}

		Vector3 blackholePos = m_owner.transform.position;

		if ( execute.buffEvt.battleOptionData.effId1 > 0 ) {
			EffectManager.Instance.Play( blackholePos, execute.buffEvt.battleOptionData.effId1, (EffectManager.eType)execute.buffEvt.battleOptionData.effType );
		}

		List<UnitCollider> list = m_owner.GetTargetColliderListByAround( m_owner.transform.position, execute.buffEvt.value );
		if ( list == null ) {
			RemoveExecute( id );
			yield break;
		}

		for ( int i = 0; i < list.Count; i++ ) {
			Unit target = list[i].Owner;
			if ( target == null || target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback() ) {
				continue;
			}

			if ( target.actionSystem == null || target.actionSystem.IsCurrentAction( eActionCommand.Appear ) || target.curHp <= 0.0f ) {
				continue;
			}

			target.actionSystem.CancelCurrentAction();
			target.StopStepForward();
		}

		while ( execute.checkDuration < execute.buffEvt.duration ) {
			execute.checkDuration += Time.fixedDeltaTime;

			for ( int i = 0; i < list.Count; i++ ) {
				Unit target = list[i].Owner;
				if ( target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback() ) {
					continue;
				}

				if ( target.actionSystem == null || target.actionSystem.IsCurrentAction( eActionCommand.Appear ) || target.curHp <= 0.0f ) {
					continue;
				}

				target.StopStepForward();

				Vector3 v = ( blackholePos - target.transform.position ).normalized;
				v.y = 0.0f;

				target.cmptMovement.UpdatePosition( v, Mathf.Max( GameInfo.Instance.BattleConfig.BlackholeMinSpeed, target.speed * 1.2f ), false );
			}

			yield return mWaitForFixedUpdate;
		}

		RemoveExecute( id );
	}

	private IEnumerator UpdateHide(int id, sExecute execute)
    {
        execute.OnRemove = RemoveHide;

        if (execute.buffEvt.battleOptionData.effId1 > 0)
        {
            EffectManager.Instance.Play(m_owner, execute.buffEvt.battleOptionData.effId1, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
        }

        m_owner.AllowAction = (Unit.eAllowAction)execute.buffEvt.value;
        m_owner.actionSystem.CancelCurrentAction();
        m_owner.TemporaryInvincible = true;
        m_owner.ShowMesh(false, true);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (execute.checkDuration < execute.buffEvt.duration)
        {
            execute.checkDuration += Time.fixedDeltaTime;
            yield return mWaitForFixedUpdate;
        }

        RemoveExecute(id);
    }

	private bool RemoveHide( int id ) {
		if ( mDicExecute[id].buffEvt.value2 == 0 || mDicExecute[id].buffEvt.value2 == 2 ) {
			UnitCollider unitCollider = m_owner.GetMainTargetCollider( true );

			if ( unitCollider && unitCollider.Owner ) {
                Vector3 dest = Vector3.zero;

                if ( mDicExecute[id].buffEvt.value2 == 0 ) {
                    dest = m_owner.GetTargetCapsuleEdgePos( unitCollider.Owner );
                }
                else if ( mDicExecute[id].buffEvt.value2 == 2 ) {
                    dest = unitCollider.Owner.transform.position - ( unitCollider.Owner.transform.forward );
                    Vector3 dir = Utility.GetDirWithoutY( dest, m_owner.transform.position );
                    float dist = Utility.GetDistanceWithoutY( dest, m_owner.transform.position );

                    RaycastHit hitInfo;
                    if ( Physics.Raycast( m_owner.transform.position, dir, out hitInfo, dist, 
                                          ( 1 << (int)eLayer.EnvObject ) | ( 1 << (int)eLayer.Wall ) ) ) {
                        dest = m_owner.GetTargetCapsuleEdgePos( unitCollider.Owner );
                    }
                }

                dest.y = m_owner.transform.position.y;
                m_owner.SetInitialPosition( dest, m_owner.transform.rotation );

                m_owner.LookAtTarget( unitCollider.transform.position );
            }
		}

        m_owner.AllowAction = Unit.eAllowAction.All;
		m_owner.TemporaryInvincible = false;
		m_owner.ReleaseLockShowMesh();
		m_owner.ShowMesh( true );

		return true;
	}

	private IEnumerator UpdateSummon(int id, sExecute execute)
	{
		execute.OnRemove = RemoveSummon;

		if (execute.buffEvt.battleOptionData.effId1 > 0)
		{
			EffectManager.Instance.Play(m_owner, execute.buffEvt.battleOptionData.effId1, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
		}

        m_owner.DeactivatePlayerMinionByIds(execute.buffEvt.battleOptionData.MinionIds);

        PlayerMinion minion = m_owner.GetMinionOrNullByIds(execute.buffEvt.battleOptionData.MinionIds);
        if(minion)
        {
            if (m_owner.gameObject.layer == (int)eLayer.Player)
            {
                Utility.SetLayer(minion.gameObject, (int)eLayer.PlayerClone, true);
            }
            else
            {
                Utility.SetLayer(minion.gameObject, (int)eLayer.EnemyClone, true);
            }

            minion.SetInitialPosition(m_owner.transform.position, m_owner.transform.rotation);
            minion.SetMinionAttackPower(m_owner.gameObject.layer, m_owner.attackPower);
            minion.SetAggroValue((int)execute.buffEvt.value);
            minion.Activate();
            minion.StopBT();

            minion.PlayAniImmediate(eAnimation.Appear);
            minion.StartDissolve(0.5f, true, new Color(0.169f, 0.0f, 0.47f));
            minion.ResetBT();

            bool end = false;
            float checkTime = 0.0f;

            //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            while (!end)
            {
                checkTime += Time.fixedDeltaTime;
                if (m_owner.curHp <= 0.0f || checkTime >= execute.buffEvt.duration || World.Instance.IsEndGame || World.Instance.ProcessingEnd)
                {
                    end = true;
                }

                yield return mWaitForFixedUpdate;
            }

            minion.StopBT();

            if (!World.Instance.IsEndGame && !World.Instance.ProcessingEnd)
            {
                float aniLength = minion.PlayAniImmediate(eAnimation.Die);
                minion.StartDissolve(aniLength, false, new Color(0.169f, 0.0f, 0.47f));

                yield return new WaitForSeconds(aniLength);
            }

            minion.Deactivate();
        }

		RemoveExecute(id);
	}

	private bool RemoveSummon(int id)
	{
		return true;
	}

    private IEnumerator UpdateSummonByDieEnemyCount( int id, sExecute execute ) {
        Player player = m_owner as Player;
        if( player == null ) {
            yield break;
		}

        bool startSummon = false;

        while( m_owner.curHp > 0.0f ) {
            if( !startSummon && player.CheckMonsterDieCountForSummon > 0 && player.CheckMonsterDieCountForSummon >= (int)execute.buffEvt.value3 ) {
                startSummon = true;
                player.CheckMonsterDieCountForSummon = 0;

                StartCoroutine( StartSummon( execute ) );
            }
            else if( startSummon && player.CheckMonsterDieCountForSummon < (int)execute.buffEvt.value3 ) {
                startSummon = false;
            }
            
            yield return mWaitForFixedUpdate;
		}

        RemoveExecute( id );
    }

    private IEnumerator StartSummon( sExecute execute ) {
        if( execute.buffEvt.battleOptionData.effId1 > 0 ) {
            EffectManager.Instance.Play( m_owner, execute.buffEvt.battleOptionData.effId1, (EffectManager.eType)execute.buffEvt.battleOptionData.effType );
        }

        //m_owner.DeactivatePlayerMinionByIds( execute.buffEvt.battleOptionData.MinionIds );

        PlayerMinion minion = m_owner.GetDeactivateMinionOrNullByIds( execute.buffEvt.battleOptionData.MinionIds );
        if( minion ) {
            if( m_owner.gameObject.layer == (int)eLayer.Player ) {
                Utility.SetLayer( minion.gameObject, (int)eLayer.PlayerClone, true );
            }
            else {
                Utility.SetLayer( minion.gameObject, (int)eLayer.EnemyClone, true );
            }

            minion.SetInitialPosition( m_owner.transform.position, m_owner.transform.rotation );
            minion.SetMinionAttackPower( m_owner.gameObject.layer, m_owner.attackPower * execute.buffEvt.value );
            minion.SetAggroValue( (int)execute.buffEvt.value2 );
            minion.Activate();
            minion.StopBT();

            minion.PlayAniImmediate( eAnimation.Appear );
            minion.StartDissolve( 0.5f, true, new Color( 0.169f, 0.0f, 0.47f ) );
            minion.ResetBT();

            bool end = false;
            float checkTime = 0.0f;

            while( !end ) {
                checkTime += Time.fixedDeltaTime;
                if( m_owner.curHp <= 0.0f || checkTime >= execute.buffEvt.duration || World.Instance.IsEndGame || World.Instance.ProcessingEnd ) {
                    end = true;
                }

                yield return mWaitForFixedUpdate;
            }

            minion.StopBT();

            if( !World.Instance.IsEndGame && !World.Instance.ProcessingEnd && minion.IsActivate() ) {
                float aniLength = minion.PlayAniImmediate(eAnimation.Die);
                minion.StartDissolve( aniLength, false, new Color( 0.169f, 0.0f, 0.47f ) );

                yield return new WaitForSeconds( aniLength );
            }

            minion.Deactivate();
        }
    }

    private IEnumerator UpdateSpRegenIncRate(int id, sExecute execute)
	{
		execute.OnRemove = RemoveSpRegenIncRate;
		m_owner.SpRegenIncRate(execute.buffEvt.value);

		bool end = false;
		float checkTime = 0.0f;

		//WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
		while (!end)
		{
			checkTime += Time.fixedDeltaTime;
			if(execute.buffEvt.duration > -1 && checkTime >= execute.buffEvt.duration)
			{
				end = true;
			}
			else if (m_owner.curHp <= 0.0f || World.Instance.IsEndGame || World.Instance.ProcessingEnd || 
				m_owner.curSp >= GameInfo.Instance.BattleConfig.USMaxSP)
			{
				end = true;
			}

			yield return mWaitForFixedUpdate;
		}

		RemoveExecute(id);
	}

	private bool RemoveSpRegenIncRate(int id)
	{
		m_owner.SpRegenIncRate(-mDicExecute[id].value);
		return true;
	}

	private IEnumerator UpdateElectricShock(int id, sExecute execute)
	{
		AniEvent.sEvent evt = new AniEvent.sEvent();
		evt.hitDir = eHitDirection.None;
		evt.hitEffectId = execute.buffEvt.effId;
		evt.atkAttr = (EAttackAttr)execute.buffEvt.value2;

		m_owner.AddAbnormalAttr(evt.atkAttr);

		if (execute.buffEvt.effId2 > 0)
		{
			EffectManager.Instance.Play(m_owner, execute.buffEvt.effId2, (EffectManager.eType)execute.buffEvt.battleOptionData.effType);
		}

		float duration = execute.buffEvt.duration + execute.AddDuration;

		mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
		mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.None;
		mBuffEvt.battleOptionData.preConditionType = BattleOption.eBOConditionType.AbnormalElectric;

		mBuffEvt.Set(m_owner.GetInstanceID(), eEventSubject.Self, eEventType.EVENT_DEBUFF_DMG_RATE_UP, m_owner,
					 execute.buffEvt.value, 0.0f, 0.0f, duration, 0.0f, 0, mBuffEvt.effId2, eBuffIconType.None);

		EventMgr.Instance.SendEvent(mBuffEvt);

		while (m_owner.curHp > 0.0f && execute.checkDuration < duration)
		{
			execute.checkTick += Time.fixedDeltaTime;
			if (!Director.IsPlaying && execute.checkTick >= execute.buffEvt.tick)
			{
				execute.checkDuration += execute.checkTick;
				execute.checkTick = 0.0f;

				if (execute.checkDuration <= duration)
				{
					mAtkEvt.SetWithSingleTarget(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, execute.buffEvt.sender, execute.buffEvt.battleOptionData.toExecuteType, evt,
												0.0f, eAttackDirection.Skip, false, 0, EffectManager.eType.None, m_owner.MainCollider, 0.0f, 
												false, false, false);

					EventMgr.Instance.SendEvent(mAtkEvt);
				}
			}

			yield return mWaitForFixedUpdate;
		}

		m_owner.RemoveAbnormalAttr(evt.atkAttr);
		RemoveExecute(id, execute.buffEvt.value3 > 0 ? true : false);
	}

	private IEnumerator UpdateLightningAttack(int id, sExecute execute)
	{
		AniEvent.sEvent evt = new AniEvent.sEvent();
		evt.hitDir = eHitDirection.None;
		evt.hitEffectId = execute.buffEvt.effId;
		evt.behaviour = eBehaviour.StunAttack;

		float attackPower = execute.buffEvt.sender.attackPower * execute.buffEvt.value;

		while (m_owner.curHp > 0.0f)
		{
			execute.checkTick += Time.fixedDeltaTime;
			if (!Director.IsPlaying && execute.checkTick >= execute.buffEvt.tick)
			{
				execute.checkTick = 0.0f;

				UnitCollider unitCol = m_owner.GetMainTargetCollider(true);
				if (unitCol)
				{
					if (execute.buffEvt.effId > 0)
					{
						EffectManager.Instance.Play(unitCol.Owner.transform.position, execute.buffEvt.effId, EffectManager.eType.Common);
					}

					mAtkEvt.SetWithSingleTarget(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, execute.buffEvt.sender, execute.buffEvt.battleOptionData.toExecuteType, evt,
												attackPower, eAttackDirection.Skip, false, 0, EffectManager.eType.None, unitCol, 0.0f,
												true, false, false);

					EventMgr.Instance.SendEvent(mAtkEvt);
				}
			}

			yield return mWaitForFixedUpdate;
		}

		RemoveExecute(id, execute.buffEvt.value3 > 0 ? true : false);
	}

    private ParticleSystem mEffAura = null;
    private IEnumerator UpdateFireAura( int id, sExecute execute ) {
        execute.OnRemove = RemoveFireAura;

        if ( mEffAura == null ) {
            mEffAura = GameSupport.CreateParticle( "Effect/Supporter/prf_fx_supporter_skill_99_aura.prefab", null );

            mEffAura.transform.SetParent( m_owner.transform );
            Utility.InitTransform( mEffAura.gameObject );
        }

        mEffAura.gameObject.SetActive( true );

        BoxCollider boxCol = mEffAura.GetComponent<BoxCollider>();
        boxCol.size = new Vector3( execute.buffEvt.value3, boxCol.size.y, execute.buffEvt.value3 );

        AniEvent.sEvent evt = new AniEvent.sEvent();
        evt.hitDir = eHitDirection.None;
        evt.hitEffectId = execute.buffEvt.effId;
        evt.behaviour = eBehaviour.Attack;

        float attackPower = execute.buffEvt.sender.attackPower * execute.buffEvt.value2;

        List<Unit> listEnemy = null;
        List<UnitCollider> listContainEnemy = new List<UnitCollider>();
        List<Unit> listStayOnBoxEnemy = new List<Unit>();

        while ( m_owner.curHp > 0.0f && !World.Instance.IsEndGame && !World.Instance.ProcessingEnd ) {
            if ( !Director.IsPlaying ) {
                listContainEnemy.Clear();

                listEnemy = m_owner.GetEnemyList( true );
                for ( int i = 0; i < listEnemy.Count; ++i ) {
                    if ( listEnemy[i].curHp <= 0.0f ) {
                        continue;
                    }

                    if ( boxCol.bounds.Contains( listEnemy[i].transform.position ) ) {
                        listContainEnemy.Add( listEnemy[i].MainCollider );

                        if ( listEnemy[i].StayOnBoxCollider == null ) {
                            listEnemy[i].StayOnBoxCollider = boxCol;
                            listStayOnBoxEnemy.Add( listEnemy[i] );

                            mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
                            mBuffEvt.battleOptionData.conditionType = BattleOption.eBOConditionType.StayOnBoxCollider;
                            mBuffEvt.battleOptionData.buffIconFlash = false;

                            mBuffEvt.Set( id, eEventSubject.Self, eEventType.EVENT_DEBUFF_DMG_RATE_UP, listEnemy[i], execute.buffEvt.value, 0.0f, 0.0f,
                                          -1, 0.0f, 0, 0, eBuffIconType.Debuff_Def );

                            EventMgr.Instance.SendEvent( mBuffEvt );
                        }
                    }
                    else if ( listEnemy[i].StayOnBoxCollider ) {
                        listEnemy[i].StayOnBoxCollider = null;
                        listStayOnBoxEnemy.Remove( listEnemy[i] );
                    }
                }

                execute.checkTick += Time.fixedDeltaTime;
                if ( execute.checkTick >= execute.buffEvt.tick ) {
                    execute.checkTick = 0.0f;

                    mAtkEvt.Set( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, execute.buffEvt.sender, execute.buffEvt.battleOptionData.toExecuteType,
                                 evt, attackPower, eAttackDirection.Skip, false, 0, EffectManager.eType.None, listContainEnemy, 0.0f, true, false, true );

                    EventMgr.Instance.SendEvent( mAtkEvt );
                }
            }

            yield return mWaitForFixedUpdate;
        }

        for ( int i = 0; i < listStayOnBoxEnemy.Count; i++ ) {
            listStayOnBoxEnemy[i].StayOnBoxCollider = null;
        }

        RemoveExecute( id );
    }

    private bool RemoveFireAura( int id ) {
        mEffAura.gameObject.SetActive( false );
        return true;
    }
}
