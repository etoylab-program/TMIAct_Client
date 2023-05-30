
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitBuffStats
{
    public enum eBuffStatType
    {
        None = 0,

        Heal,                   // 회복 능력
        Damage,                 // 받는 피해
        AttackPower,            // 주는 피해
        AttackPowerToShield,    // 쉴드에 주는 피해
        UltimateSkillAtkPower,  // 오의로 주는 피해
        CriticalRate,           // 크리티컬 확률
        CriticalDmg,            // 크리티컬 데미지
        CriticalResist,         // 크리티컬 저항
        CriticalDef,            // 크리티컬 방어력
        Speed,                  // 속도
        DebuffDurTime,          // 상태이상 (디버프 & 스턴등) 유지 시간
        AddSPRate,              // 공격 시 SP 회복량
		Penetrate,
    }

    public enum eIncreaseType
    {
        None = 0,

        Increase,
        Decrease,
    }


	public class sInfo {
		public eBuffDebuffType                  type;
		public int                              id;
		public eBuffStatType                    buffStatType;
		public eIncreaseType                    increaseType;
		public BattleOption.eToExecuteType      toExecuteType;
		public BattleOption.eBOConditionType    preConditionType;
		public BattleOption.eBOConditionType    conditionType;
		public BattleOption.eBOAtkConditionType atkConditionType;
		public float                            value;
		public int                              randomStart;
		public int                              actionTableId;
		public bool                             once;
		public bool                             WithEnemyCountRatio = false;
		public bool                             WithBuffCountRatio  = false;
		public int								DebufIconType		= 0;
		public int                              maxStack;
		public int                              stackCount          = 1;


		public sInfo( eBuffDebuffType type, int id, eBuffStatType buffStatType, eIncreaseType increaseType, float value, int randomStart,
					  BattleOption.eToExecuteType toExecuteType, BattleOption.eBOConditionType preConditionType, BattleOption.eBOConditionType conditionType,
					  BattleOption.eBOAtkConditionType atkConditionType, int actionTableId, bool once,
					  bool withEnemyCountRatio = false, bool withBuffCountRatio = false, int debuffIconType = 0 ) {
			this.type = type;

			Copy( id, buffStatType, increaseType, value, randomStart, toExecuteType, preConditionType, conditionType, atkConditionType, actionTableId,
				  once, withEnemyCountRatio, withBuffCountRatio, debuffIconType );
		}

		public sInfo( eBuffDebuffType type, int id, eBuffStatType buffStatType, eIncreaseType increaseType, float value, int randomStart,
					  BattleOption.eToExecuteType toExecuteType, BattleOption.eBOConditionType preConditionType, BattleOption.eBOConditionType conditionType,
					  BattleOption.eBOAtkConditionType atkConditionType, int maxStack, int actionTableId, bool once,
					  bool withEnemyCountRatio = false, bool withBuffCountRatio = false, int debuffIconType = 0 ) {
			this.type = type;
			this.maxStack = maxStack;
			stackCount = 1;

			Copy( id, buffStatType, increaseType, value, randomStart, toExecuteType, preConditionType, conditionType, atkConditionType, actionTableId,
				  false, withEnemyCountRatio, withBuffCountRatio, debuffIconType );
		}

		public void Copy( int id, eBuffStatType buffStatType, eIncreaseType increaseType, float value, int randomStart,
						  BattleOption.eToExecuteType toExecuteType, BattleOption.eBOConditionType preConditionType, BattleOption.eBOConditionType conditionType,
						  BattleOption.eBOAtkConditionType atkConditionType, int actionTableId, bool once, 
						  bool withEnemyCountRatio, bool withBuffCountRatio, int debuffIconType = 0 ) {
			this.id = id;
			this.buffStatType = buffStatType;
			this.increaseType = increaseType;
			this.value = value;
			this.randomStart = randomStart;
			this.toExecuteType = toExecuteType;
			this.preConditionType = preConditionType;
			this.conditionType = conditionType;
			this.atkConditionType = atkConditionType;
			this.actionTableId = actionTableId;
			this.once = once;
			WithEnemyCountRatio = withEnemyCountRatio;
			WithBuffCountRatio = withBuffCountRatio;
			DebufIconType = debuffIconType;
		}

		public void Overlap() {
			stackCount = Mathf.Clamp( stackCount + 1, 1, maxStack );
		}

		public void Overlap( int stackCount ) {
			this.stackCount = Mathf.Clamp( stackCount, 1, maxStack );
		}
	}


	private Unit        mOwner      = null;
    private List<sInfo> mListBuff   = new List<sInfo>();


    public UnitBuffStats(Unit owner)
    {
        mOwner = owner;
    }

    public void Clear()
    {
        mListBuff.Clear();
    }
    private sInfo GetBuffInfo(int id)
    {
        for (int i = 0; i < mListBuff.Count; i++)
        {
            if (mListBuff[i].id == id)
            {
                return mListBuff[i];                
            }
        }
        return null;
    }

    public void AddBuffStatAndStackOnExecute(BuffEvent buffEvt, eBuffStatType buffStatType, eIncreaseType increaseType, int id, float value, 
											 int curStackCount = -1, bool once = false, bool ignoreRandom = false)
    {
		int randomStart = ignoreRandom ? 0 : buffEvt.battleOptionData.randomStart;

		if (buffEvt.battleOptionData.maxStack > 1)
        {
            AddStackBuffStat(buffEvt.battleOptionData.buffDebuffType, id, buffStatType, increaseType, value, randomStart, 
							 buffEvt.battleOptionData.toExecuteType, buffEvt.battleOptionData.preConditionType, buffEvt.battleOptionData.conditionType, 
							 buffEvt.battleOptionData.atkConditionType, buffEvt.battleOptionData.maxStack, buffEvt.battleOptionData.actionTableId, 
							 curStackCount);
        }
        else
        {
            AddBuffStat(buffEvt.battleOptionData.buffDebuffType, id, buffStatType, increaseType, value, randomStart,
                        buffEvt.battleOptionData.toExecuteType, buffEvt.battleOptionData.preConditionType, buffEvt.battleOptionData.conditionType,
                        buffEvt.battleOptionData.atkConditionType, buffEvt.battleOptionData.actionTableId, once);
        }
    }

	public void AddBuffStatAndStack( BuffEvent buffEvt, eBuffStatType buffStatType, eIncreaseType increaseType, bool once = false,
									bool withEnemyCountRatio = false, bool withBuffCountRatio = false, int debuffIconType = 0 ) {
		if( buffEvt.battleOptionData.maxStack > 1 ) {
			AddStackBuffStat( buffEvt.battleOptionData.buffDebuffType, buffEvt.id, buffStatType,
							  increaseType, buffEvt.value, buffEvt.battleOptionData.randomStart, buffEvt.battleOptionData.toExecuteType,
							  buffEvt.battleOptionData.preConditionType, buffEvt.battleOptionData.conditionType, buffEvt.battleOptionData.atkConditionType,
							  buffEvt.battleOptionData.maxStack, buffEvt.battleOptionData.actionTableId, -1, 
                              withEnemyCountRatio, withBuffCountRatio, debuffIconType );
		}
		else {
			if( buffEvt.battleOptionData.maxStack <= -1 ) {
				once = true;
			}

			AddBuffStat( buffEvt.battleOptionData.buffDebuffType, buffEvt.id, buffStatType, increaseType, buffEvt.value, buffEvt.battleOptionData.randomStart,
						 buffEvt.battleOptionData.toExecuteType, buffEvt.battleOptionData.preConditionType, buffEvt.battleOptionData.conditionType,
						 buffEvt.battleOptionData.atkConditionType, buffEvt.battleOptionData.actionTableId, once, 
                         withEnemyCountRatio, withBuffCountRatio, debuffIconType );
		}
	}

	public void AddBuffStat( eBuffDebuffType type, int id, eBuffStatType buffStatType, eIncreaseType increaseType, float value, int randomStart,
							 BattleOption.eToExecuteType toExecuteType, BattleOption.eBOConditionType preConditionType,
							 BattleOption.eBOConditionType conditionType, BattleOption.eBOAtkConditionType atkConditionType,
							 int actionTableId, bool once, bool withEnemyCountRatio = false, bool withBuffCountRatio = false, int debuffIconType = 0 ) {
		sInfo find = GetBuffInfo(id);

		if( find != null ) {
			find.Copy( id, buffStatType, increaseType, value, randomStart, toExecuteType, preConditionType, conditionType, atkConditionType, actionTableId,
					   once, withEnemyCountRatio, withBuffCountRatio, debuffIconType );
		}
		else {
			sInfo info = new sInfo( type, id, buffStatType, increaseType, value, randomStart, toExecuteType, preConditionType, conditionType, atkConditionType,
								    actionTableId, once, withEnemyCountRatio, withBuffCountRatio, debuffIconType );
			mListBuff.Add( info );
		}
	}

	public void AddStackBuffStat( eBuffDebuffType type, int id, eBuffStatType buffStatType, eIncreaseType increaseType, float value, int randomStart,
								  BattleOption.eToExecuteType toExecuteType, BattleOption.eBOConditionType preConditionType,
								  BattleOption.eBOConditionType conditionType, BattleOption.eBOAtkConditionType atkConditionType,
								  int maxStack, int actionTableId, int curStackCount = -1,
								  bool withEnemyCountRatio = false, bool withBuffCountRatio = false, int debuffIconType = 0 ) {
		sInfo find = GetBuffInfo(id);

		if( find != null ) {
			if( curStackCount == -1 ) {
				find.Overlap();
			}
			else {
				find.Overlap( curStackCount );
			}
		}
		else {
			sInfo info = new sInfo( type, id, buffStatType, increaseType, value, randomStart, toExecuteType, preConditionType, conditionType, atkConditionType,
								    maxStack, actionTableId, false, withEnemyCountRatio, withBuffCountRatio, debuffIconType );
			mListBuff.Add( info );
		}
	}

	public void RemoveBuffStat(int id)
    {
        //sInfo find = mListBuff.Find(x => x.id == id);
        sInfo find = GetBuffInfo(id);

        if (find == null)
            return;

        Log.Show(id + "번 버프스탯 제거!!!", Log.ColorType.Blue);
        mListBuff.Remove(find);
    }

    public void RemoveAllBuffStat()
    {
        mListBuff.Clear();
    }

    public bool IsEmpty()
    {
        return mListBuff.Count <= 0;
    }

    List<sInfo> findBuffStat = new List<sInfo>();
    public List<sInfo> FindBuffStat(eBuffStatType buffStatType, eIncreaseType increaseType)
    {
        //List<sInfo> find = mListBuff.FindAll(x => x.buffStatType == buffStatType && x.increaseType == increaseType);
        findBuffStat.Clear();
        for (int i = 0; i < mListBuff.Count; i++)
        {
            if(mListBuff[i].buffStatType == buffStatType && mListBuff[i].increaseType == increaseType)
            {
                findBuffStat.Add(mListBuff[i]);
            }
        }

        if (findBuffStat == null || findBuffStat.Count <= 0)
            return null;

        return findBuffStat;
    }

    public sInfo GetBuffStat(int id)
    {
        //return mListBuff.Find(x => x.id == id);
        for (int i = 0; i < mListBuff.Count; i++)
        {
            if(mListBuff[i].id == id)
            {
                return mListBuff[i];
            }
        }
        return null;
    }

    public bool HasBuff()
    {
        List<sInfo> findAll = mListBuff.FindAll(x => x.type == eBuffDebuffType.Buff);
        if (findAll == null || findAll.Count <= 0)
            return false;

        return true;
    }

    public bool HasDebuff()
    {
        List<sInfo> findAll = mListBuff.FindAll(x => x.type == eBuffDebuffType.Debuff);
        if (findAll == null || findAll.Count <= 0)
            return false;

        return true;
    }

	public float CalcBuffStat( BattleOption.eToExecuteType toExecuteType, eBuffStatType buffStatType, eIncreaseType increaseType, float defaultValue,
							   bool isRangeAttack, bool isNormalAttck, bool isUltimateSkill, bool isWeaponSkill, Unit target ) {
		float result = defaultValue;

		List<sInfo> find = FindBuffStat(buffStatType, increaseType);
        if( find == null ) {
            return result;
        }

		for( int i = 0; i < find.Count; i++ ) {
			if( find[i].randomStart > 0 ) {
                if( UnityEngine.Random.Range( 0, 100 ) >= find[i].randomStart ) {
                    continue;
                }
			}

			if( find[i].preConditionType != BattleOption.eBOConditionType.None ) {
                if( !GameSupport.IsBOConditionCheck( find[i].preConditionType, 0.0f, mOwner ) ) {
                    continue;
                }
			}

			if( find[i].conditionType != BattleOption.eBOConditionType.None ) {
                if( !GameSupport.IsBOConditionCheck( find[i].conditionType, 0.0f, target ) ) {
                    continue;
                }
			}

			if( find[i].atkConditionType != BattleOption.eBOAtkConditionType.None ) {
				if( find[i].atkConditionType == BattleOption.eBOAtkConditionType.MeleeAttack && isRangeAttack ) {
					continue;
				}
				else if( find[i].atkConditionType == BattleOption.eBOAtkConditionType.RangeAttack && !isRangeAttack ) {
					continue;
				}
				else if( find[i].atkConditionType == BattleOption.eBOAtkConditionType.NormalAttack && !isNormalAttck ) {
					continue;
				}
				else if( find[i].atkConditionType == BattleOption.eBOAtkConditionType.SkillAttack && isNormalAttck ) {
					continue;
				}
				else if( find[i].atkConditionType == BattleOption.eBOAtkConditionType.UltimateSkill ) {
					if( !isUltimateSkill ) {
						continue;
					}
				}
				else if( find[i].atkConditionType == BattleOption.eBOAtkConditionType.SkillAction ) {
					if( toExecuteType != find[i].toExecuteType ) {
						continue;
					}
					if( mOwner.LastProjectile ) {
						ActionSelectSkillBase action = mOwner.actionSystem.GetActionOrNullByTableId<ActionSelectSkillBase>(mOwner.LastProjectile.OwnerActionTableId);
						if( action == null || action.TableId != find[i].actionTableId ) {
							continue;
						}
					}
					else {
						ActionSelectSkillBase action = mOwner.actionSystem.GetCurrentAction<ActionSelectSkillBase>();
						if( action == null || action.TableId != find[i].actionTableId ) {
							bool isFind = false;
							Player player = mOwner as Player;
							if ( player != null && player.Guardian != null ) {
								ActionGuardianBase actionGuardianBase = player.Guardian.actionSystem.GetCurrentAction<ActionGuardianBase>();
								if ( actionGuardianBase != null && actionGuardianBase.TableId == find[i].actionTableId ) {
									isFind = true;
								}
							}

							if ( isFind == false ) {
								continue;
							}
						}
					}
				}
				else if( find[i].atkConditionType == BattleOption.eBOAtkConditionType.WeaponSkill ) {
					if( !isWeaponSkill ) {
						continue;
					}
				}
				else if( find[i].atkConditionType == BattleOption.eBOAtkConditionType.MaikaDefaultAttack ) {
					if( mOwner.tableId != (int)ePlayerCharType.Maika ) {
						continue;
					}

					ActionSelectSkillBase action = mOwner.actionSystem.GetCurrentAction<ActionSelectSkillBase>();
					if( action == null || ( action.actionCommand != eActionCommand.Attack01 && action.actionCommand != eActionCommand.ChargingAttack ) ) {
						continue;
					}
				}
			}

			float enemyCountRatio = 0.0f;

			if( find[i].WithEnemyCountRatio ) {
				List<Unit> listActiveEnemy = World.Instance.EnemyMgr.GetActiveEnemies(mOwner);

				if( listActiveEnemy != null && listActiveEnemy.Count > 0 ) {
					enemyCountRatio = listActiveEnemy.Count;
				}
			}

			float buffCountRatio = 0.0f;
			if( mOwner.cmptBuffDebuff && find[i].WithBuffCountRatio ) {
				buffCountRatio = mOwner.cmptBuffDebuff.GetCount( eBuffDebuffType.Buff );
			}

			float stackCountRatio = 0;
			if( target && target.cmptBuffDebuff && find[i].DebufIconType != 0 ) {
				stackCountRatio = target.cmptBuffDebuff.GetStackCountByIconType( (eBuffIconType)find[i].DebufIconType );
			}

            if( buffStatType == UnitBuffStats.eBuffStatType.CriticalRate ) { 
				if( increaseType == eIncreaseType.Increase ) {
					result += ( find[i].value * find[i].stackCount * (float)eCOUNT.MAX_PROBABILITY );
					result += ( result * ( find[i].value * enemyCountRatio ) );
					result += ( result * ( find[i].value * buffCountRatio ) );
					result += ( result * ( find[i].value * stackCountRatio ) );
				}
				else {
					result -= ( find[i].value * find[i].stackCount * (float)eCOUNT.MAX_PROBABILITY );
					result -= ( result * ( find[i].value * enemyCountRatio ) );
					result -= ( result * ( find[i].value * buffCountRatio ) );
					result -= ( result * ( find[i].value * stackCountRatio ) );
				}
			}
            else if( buffStatType == UnitBuffStats.eBuffStatType.Speed || buffStatType == UnitBuffStats.eBuffStatType.DebuffDurTime ) {
                if( increaseType == eIncreaseType.Increase ) {
                    result += ( find[i].value * find[i].stackCount );
                }
                else {
                    result -= ( find[i].value * find[i].stackCount );
                }
			}
			else {
				if( increaseType == eIncreaseType.Increase ) {
					result += result * ( find[i].value * find[i].stackCount );
					result += ( result * ( find[i].value * enemyCountRatio ) );
					result += ( result * ( find[i].value * buffCountRatio ) );
					result += ( result * ( find[i].value * stackCountRatio ) );
				}
				else {
					result -= result * ( find[i].value * find[i].stackCount );
					result -= ( result * ( find[i].value * enemyCountRatio ) );
					result -= ( result * ( find[i].value * buffCountRatio ) );
					result -= ( result * ( find[i].value * stackCountRatio ) );
				}
			}

			if( find[i].once ) {
				mListBuff.Remove( find[i] );
			}
		}

		if( mOwner.MarkingInfo.MarkingType == Unit.eMarkingType.AccumDamageDown && buffStatType == eBuffStatType.Damage && increaseType == eIncreaseType.Decrease ) {
			mOwner.MarkingInfo.AccumValue += ( defaultValue - result );
		}

		return result;
	}
}
