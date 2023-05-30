
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;

public class GameSupport
{
	public static bool IsCheckGoods( eGOODSTYPE etype, long value, bool skipToastMsg = false ) {
		if ( !GameInfo.Instance.UserData.IsGoods( etype, value ) ) {
            if ( etype == eGOODSTYPE.CASH ) {
                MessagePopup.CYN( eTEXTID.TITLE_NOTICE, string.Format( FLocalizeString.Instance.GetText( 3117 ), GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.CASH] ), eTEXTID.YES, eTEXTID.NO, OnMsg_CashBuy );
            }
            else if ( !skipToastMsg ) {
                if ( etype == eGOODSTYPE.RAIDPOINT ) {
                    MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3324 ) );
                }
                else {
                    MessageToastPopup.Show( FLocalizeString.Instance.GetText( (int)eTEXTID.GOODSTYPE_LACK_START + (int)etype ) );
                }
            }

			return false;
		}

		return true;
	}

	public static bool IsCheckGoods(GameTable.Store.Param storeTable, StoreSaleData storesaledata, int itemCount = 1)
    {
        eREWARDTYPE PurchaseType = (eREWARDTYPE)storeTable.PurchaseType;
        if (PurchaseType == eREWARDTYPE.GOODS)
        {
            return IsCheckGoods((eGOODSTYPE)storeTable.PurchaseIndex, GetDiscountPrice(storeTable.PurchaseValue, storesaledata) * itemCount);
        }
        else if (PurchaseType == eREWARDTYPE.ITEM)
        {
            ItemData item = GameInfo.Instance.ItemList.Find(x => x.TableID == storeTable.PurchaseIndex);

            //할인율 적용
            if (item == null || item.Count < GetDiscountPrice(storeTable.PurchaseValue, storesaledata) * itemCount)
            {
                //3071 [FFFF00FF]{0}[-]이(가) 부족합니다.
                var itemTable = GameInfo.Instance.GameTable.FindItem(storeTable.PurchaseIndex);
                string itemName = FLocalizeString.Instance.GetText(itemTable.Name);
                MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3071), itemName));
                return false;   
            }
        }
        else
            return false;

        return true;
    }
    public static void OnMsg_CashBuy()
    {
        GameSupport.PaymentAgreement_Cash();
    }
	
	public static int GetInvenCount() {
		int total = 0;

		total += GameInfo.Instance.CardList.Count;
		total += GameInfo.Instance.WeaponList.Count;
		total += GameInfo.Instance.GemList.Count;
		total += GameInfo.Instance.BadgeList.Count;
		
        List<ItemData> list = GameInfo.Instance.ItemList.FindAll( x => x.TableData != null && x.TableData.Type != (int)eITEMTYPE.EVENT );
		total += list.Count;
		
        return total;
	}

	public static bool IsCheckInven()
    {
#if UNITY_EDITOR
        if(!AppMgr.Instance.configData.m_Network)
        {
            return true;
        }
#endif

        int now = GetInvenCount();
        int max = GameInfo.Instance.UserData.ItemSlotCnt;

        if (now >= max)
        {
            if (max >= GameInfo.Instance.GameConfig.MaxItemSlotCount)
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3078));
            else
                MessagePopup.CYN(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3077), eTEXTID.YES, eTEXTID.NO, OnMsg_InvenExpansion);
            return false;
        }

        return true;
    }

    public static bool IsEmptyInEquipMainWeapon(ePresetKind kind, long cuid = 0)
    {
        List<long> cuidList = new List<long>();
        switch (kind)
        {
            case ePresetKind.STAGE:
                {
                    cuidList.Add(cuid);
                }
                break;
            case ePresetKind.ARENA:
                {
                    cuidList = GameInfo.Instance.TeamcharList;
                }
                break;
            case ePresetKind.ARENA_TOWER:
                {
                    cuidList = GetArenaTowerTeamCharList();
                }
                break;
        }

        foreach (long uid in cuidList)
        {
            CharData charData = GameInfo.Instance.GetArenaTowerCharData(uid);
            if (charData == null)
            {
                continue;
            }

            if (GameInfo.Instance.GetArenaTowerWeaponData(charData) != null)
            {
                continue;
            }

            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(3295), null);
            return true;
        }

        return false;
    }

    private static eGOODSTYPE mUseGoodsType = eGOODSTYPE.GOLD;
    private static int mGoodsCount = 0;
    public static void OnMsg_InvenExpansion()
    {
        int now = GameInfo.Instance.UserData.ItemSlotCnt;
        //if (now >= GameInfo.Instance.GameConfig.MaxItemSlotCount)

        if(now >= GameInfo.Instance.GameConfig.MaxItemSlotCount)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3079));
            return;
        }
        
        int add = GameInfo.Instance.GameConfig.AddItemSlotCount;

        string textName = string.Format(FLocalizeString.Instance.GetText(118), FLocalizeString.Instance.GetText(1002));
        string textDesc = string.Format(FLocalizeString.Instance.GetText(3211), FLocalizeString.Instance.GetText(1002), 
                                        now + add, GameInfo.Instance.GameConfig.MaxItemSlotCount, add);
        string text = string.Format("{0}\n{1}", textName, textDesc);

        if (now < GameInfo.Instance.GameConfig.AddItemSlotCashCount)
        {
            mUseGoodsType = eGOODSTYPE.GOLD;
            mGoodsCount = GameInfo.Instance.GameConfig.AddItemSlotGold;
        }
        else
        {
            mUseGoodsType = eGOODSTYPE.CASH;
            mGoodsCount = GameInfo.Instance.GameConfig.AddItemSlotCash;
        }

        MessagePopup.CYN(eTEXTID.TITLE_BUY, text, eTEXTID.YES, eTEXTID.NO, mUseGoodsType, mGoodsCount, OnMsg_InvenExpansionOK);
    }

    public static void OnMsg_InvenExpansionOK()
    {
        if(mUseGoodsType != eGOODSTYPE.NONE && !IsCheckGoods(mUseGoodsType, mGoodsCount))
        {
            return;
        }

        GameInfo.Instance.Send_ReqAddItemSlot(1, OnNetReqAddItemSlot);
    }

    public static void OnNetReqAddItemSlot(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3081));
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.InitComponent("ItemPanel");
        LobbyUIManager.Instance.Renewal("ItemPanel");
    }

    /// <summary>
    /// 배틀 옵션 조건 체크
    /// </summary>
	public static bool IsBOConditionCheck( BattleOption.eBOConditionType conditionType, float condValue, Unit owner, Unit attacker = null,
										   int actionTableId = -1, Projectile projectile = null ) {
		float checkHp = 0.0f;

		if ( conditionType == BattleOption.eBOConditionType.Groggy ) {
			ActionHit actionHit = owner.actionSystem.GetCurrentAction<ActionHit>();
            if ( actionHit == null || actionHit.State != ActionHit.eState.Stun ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.Down ) {
			ActionHit actionHit = owner.actionSystem.GetCurrentAction<ActionHit>();
            if ( actionHit == null || !actionHit.IsDownState() ) {
                return false;
            }
		}
        else if ( conditionType == BattleOption.eBOConditionType.Float ) {
            ActionHit actionHit = owner.actionSystem.GetCurrentAction<ActionHit>();
            if ( actionHit == null || !actionHit.IsFloatState() ) {
                return false;
            }
        }
        else if ( conditionType == BattleOption.eBOConditionType.HasBuff ) {
            if ( !owner.HasBuff() ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.HasDebuff ) {
            if ( !owner.HasDebuff() ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.HasDebuffGroggy ) {
			ActionHit actionHit = owner.actionSystem.GetCurrentAction<ActionHit>();
            if ( !owner.HasDebuff() && ( actionHit == null || actionHit.State != ActionHit.eState.Stun ) ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.HasDotAddiction ) {
            if ( !owner.HasBuffIcon( eBuffIconType.Debuff_Addiction ) ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.HasDotBleeding ) {
            if ( !owner.HasBuffIcon( eBuffIconType.Debuff_Bleeding ) ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.HasDotFlame ) {
            if ( !owner.HasBuffIcon( eBuffIconType.Debuff_Flame ) ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.MonTypeHuman ) {
            if ( owner.monType != Unit.eMonType.Human ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.MonTypeMachine ) {
            if ( owner.monType != Unit.eMonType.Machine ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.MonTypeDevil ) {
            if ( owner.monType != Unit.eMonType.Devil ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.MonGradeNormal ) {
            if ( owner.grade != Unit.eGrade.Normal ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.MonGradeEpic ) {
            if ( owner.grade != Unit.eGrade.Epic ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.MonGradeBoss ) {
            if ( owner.grade != Unit.eGrade.Boss ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.UnitTypePlayer ) {
            if ( owner as Player == null ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.UnitTypeEnemy ) {
            if ( owner as Enemy == null ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.HoldPosition ) {
            if ( owner.holdPositionRef <= 0 ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.NormalAttacking && !owner.actionSystem.IsCurrentNormalAttackAction() ) {
			return false;
		}
		else if ( conditionType == BattleOption.eBOConditionType.MeleeAttack ) {
			if ( owner.onAtkAniEvt == null || owner.onAtkAniEvt.isRangeAttack || ( projectile && !projectile.IsMelee ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.RangeAttack ) {
			if ( owner.onAtkAniEvt == null || !owner.onAtkAniEvt.isRangeAttack ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeAsagi ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Asagi ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeSakura ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Sakura ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeYukikaze ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Yukikaze ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeRinko ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Rinko ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeMurasaki ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Murasaki ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeJinglei ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Jinglei ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeShiranui ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Shiranui ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeEmily ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Emily ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeKurenai ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Kurenai ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeOboro ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Oboro ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeAsuka ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Asuka ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeKirara ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Kirara ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeIngrid ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Ingrid ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeNoah ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Noah ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeAstaroth ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Astaroth ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeTokiko ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Tokiko ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeShizuru ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Shizuru ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeFelicia ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Felicia ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeMaika ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Maika ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeRin ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Rin ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeSora ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Sora ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeAnnerose ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Annerose ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeNagi ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Nagi ) {
                return false;
            }
		}
        else if ( conditionType == BattleOption.eBOConditionType.CharTypeAina ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Aina ) {
                return false;
            }
        }
		else if ( conditionType == BattleOption.eBOConditionType.CharTypeSaika ) {
			if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Saika ) {
				return false;
			}
		}
        else if ( conditionType == BattleOption.eBOConditionType.CharTypeShisui ) {
            if ( owner as Player == null || owner.tableId != (int)ePlayerCharType.Shisui ) {
                return false;
            }
        }
        else if ( conditionType == BattleOption.eBOConditionType.RinkoChargeHomingAttack ||
				 conditionType == BattleOption.eBOConditionType.JingleiKikoshoAttack ||
				 conditionType == BattleOption.eBOConditionType.ShiranuiSpearAttack ||
				 conditionType == BattleOption.eBOConditionType.FeliciaEvadeCharge2Attack || 
                 conditionType == BattleOption.eBOConditionType.RinChargeAttack ||
                 conditionType == BattleOption.eBOConditionType.ShizuruEvadeChargeAttack ) {
			Type type = Type.GetType( "Action" + conditionType.ToString() );
			if ( type == null ) {
				return false;
			}

            if ( attacker == null ) {
                return false;
            }

			ActionSelectSkillBase action = attacker.GetComponent( type ) as ActionSelectSkillBase;
			if ( action == null || action.TableId != actionTableId ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.AbnormalFire ) {
			if ( !owner.IsCurrentAbnormalAttr( EAttackAttr.FIRE ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.AbnormalIce ) {
			if ( !owner.IsCurrentAbnormalAttr( EAttackAttr.ICE ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.AbnormalElectric ) {
			if ( !owner.IsCurrentAbnormalAttr( EAttackAttr.ELECTRIC ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.AbnormalPoison ) {
			if ( !owner.IsCurrentAbnormalAttr( EAttackAttr.POISON ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.AbnormalBleeding ) {
			if ( !owner.IsCurrentAbnormalAttr( EAttackAttr.BLEEDING ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.AbnormalFreeze ) {
			if ( !owner.IsCurrentAbnormalAttr( EAttackAttr.Freeze ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.SpeedDown ) {
			if ( owner.aniEvent == null || owner.aniEvent.aniSpeed >= 1.0f ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.CheckHpLessEqual10 ) {
			checkHp = owner.parent ? owner.parent.curHp : owner.curHp;
			if ( checkHp > ( owner.maxHp * 0.1f ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.CheckHpLessEqual20 ) {
			checkHp = owner.parent ? owner.parent.curHp : owner.curHp;
			if ( checkHp > ( owner.maxHp * 0.2f ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.CheckHpLessEqual30 ) {
			checkHp = owner.parent ? owner.parent.curHp : owner.curHp;
			if ( checkHp > ( owner.maxHp * 0.3f ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.CheckHpLessEqual40 ) {
			checkHp = owner.parent ? owner.parent.curHp : owner.curHp;
			if ( checkHp > ( owner.maxHp * 0.4f ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.CheckHpLessEqual50 ) {
			checkHp = owner.parent ? owner.parent.curHp : owner.curHp;
			if ( checkHp > ( owner.maxHp * 0.5f ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.CheckHpLessEqual60 ) {
			checkHp = owner.parent ? owner.parent.curHp : owner.curHp;
			if ( checkHp > ( owner.maxHp * 0.6f ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.CheckHpLessEqual70 ) {
			checkHp = owner.parent ? owner.parent.curHp : owner.curHp;
			if ( checkHp > ( owner.maxHp * 0.7f ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.CheckHpLessEqual80 ) {
			checkHp = owner.parent ? owner.parent.curHp : owner.curHp;
			if ( checkHp > ( owner.maxHp * 0.8f ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.CheckHpLessEqual90 ) {
			checkHp = owner.parent ? owner.parent.curHp : owner.curHp;
			if ( checkHp > ( owner.maxHp * 0.9f ) ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.PlayerSummons ) {
			if ( owner.charType != eCharacterType.Summons ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.SupportType ) {
			Player player = owner as Player;
			if ( player == null || player.CharAttrType != eCharAttrType.Support ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.ProtectType ) {
			Player player = owner as Player;
			if ( player == null || player.CharAttrType != eCharAttrType.Protect ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.AggressiveType ) {
			Player player = owner as Player;
			if ( player == null || player.CharAttrType != eCharAttrType.Aggressive ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.StageMain ) {
            if ( World.Instance.StageType != eSTAGETYPE.STAGE_MAIN_STORY ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.StageDaily ) {
            if ( World.Instance.StageType != eSTAGETYPE.STAGE_DAILY ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.StageEvent ) {
            if ( World.Instance.StageType != eSTAGETYPE.STAGE_EVENT ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.StageTower ) {
            if ( World.Instance.StageType != eSTAGETYPE.STAGE_TOWER ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.StageRaid ) {
            if ( World.Instance.StageType != eSTAGETYPE.STAGE_RAID ) {
                return false;
            }
		}
		else if ( conditionType == BattleOption.eBOConditionType.EnemyGradeLessEqual2 ) {
			Enemy enemy = owner as Enemy;
			if ( enemy == null || enemy.grade == Unit.eGrade.None || enemy.grade > Unit.eGrade.Epic ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.EnemyGradeEqual3 ) {
			Enemy enemy = owner as Enemy;
			if ( enemy == null || enemy.grade == Unit.eGrade.None || enemy.grade < Unit.eGrade.Boss ) {
				return false;
			}
		}
		else if ( conditionType == BattleOption.eBOConditionType.CheckSp ) {
			Player player = owner as Player;
			if ( player == null || player.curSp < condValue ) {
				return false;
			}
		}
        else if ( conditionType == BattleOption.eBOConditionType.FloatMeleeAttack ) {
			if ( owner.onAtkAniEvt == null || owner.onAtkAniEvt.isRangeAttack || ( projectile && !projectile.IsMelee ) ) {
				return false;
			}

			if ( owner.listHitTarget.Count <= 0 ) {
				return false;
			}

			bool isBattleOptionSend = false;
			for ( int i = 0; i < owner.listHitTarget.Count; i++ ) {
				if ( owner.listHitTarget[i] == null || owner.listHitTarget[i].actionSystem == null ) {
					continue;
				}

				ActionHit actionHit = owner.listHitTarget[i].actionSystem.GetCurrentAction<ActionHit>();
				if ( actionHit == null || !actionHit.IsFloatState() ) {
					continue;
				}

				isBattleOptionSend = true;
				break;
			}

			if ( !isBattleOptionSend ) {
				return false;
			}
		}
        else if ( conditionType == BattleOption.eBOConditionType.SkipEnemyAttack ) {
            if ( owner.skipAttack == false ) {
                return false;
            }
        }

		return true;
	}

	//AP 체크--------------------------------------------------------------------------------------------------------------------------------
	public static bool IsCheckTicketAP(int ticket)
    {
        if (!GameInfo.Instance.UserData.IsGoods(eGOODSTYPE.AP, ticket))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTYPE_LACK_START + (int)eGOODSTYPE.AP));
			UIRecoverySelectPopup uIRecoverySelectPopup = LobbyUIManager.Instance.GetUI<UIRecoverySelectPopup>( "RecoverySelectPopup" );
			if ( uIRecoverySelectPopup != null ) {
				uIRecoverySelectPopup.SetGoodsType( eGOODSTYPE.AP );
				uIRecoverySelectPopup.SetUIActive( true );
			}
            return false;
        }
        return true;
    }

    public static int GetMaxAP(bool isCharAddCK = true)
    {
        int ticketmax = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == GameInfo.Instance.GameConfig.AccountLevelUpGroup && x.Level == GameInfo.Instance.UserData.Level).Value1;
        if (isCharAddCK == true)
        {
            ticketmax += GameInfo.Instance.GameConfig.CharOpenAddAP * GameInfo.Instance.CharList.Count;
            if (IsActiveUserBuff(eBuffEffectType.Buff_MaxAP))
            {
                ticketmax += GetUserBuffEffectData(eBuffEffectType.Buff_MaxAP).TableData.Value;
            }
            
            ticketmax += GetFavorBuffValue(eBuffEffectType.Buff_MaxAP);
        }
        return ticketmax;
    }

    public static int GetMaxAPByRank(int nRank, bool isCharAddCK = true)
    {
        int ticketmax = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == GameInfo.Instance.GameConfig.AccountLevelUpGroup && x.Level == nRank).Value1;
        if (isCharAddCK == true)
        {
            ticketmax += GameInfo.Instance.GameConfig.CharOpenAddAP * GameInfo.Instance.CharList.Count;
            if (IsActiveUserBuff(eBuffEffectType.Buff_MaxAP))
            {
                ticketmax += GetUserBuffEffectData(eBuffEffectType.Buff_MaxAP).TableData.Value;
            }

			ticketmax += GetFavorBuffValue( eBuffEffectType.Buff_MaxAP );
		}
        return ticketmax;
    }

    //BP 체크--------------------------------------------------------------------------------------------------------------------------------
    public static bool IsCheckTicketBP(int ticket)
    {
        if (!GameInfo.Instance.UserData.IsGoods(eGOODSTYPE.BP, ticket))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTYPE_LACK_START + (int)eGOODSTYPE.BP));
			UIRecoverySelectPopup uIRecoverySelectPopup = LobbyUIManager.Instance.GetUI<UIRecoverySelectPopup>( "RecoverySelectPopup" );
			if ( uIRecoverySelectPopup != null ) {
				uIRecoverySelectPopup.SetGoodsType( eGOODSTYPE.BP );
				uIRecoverySelectPopup.SetUIActive( true );
			}
            return false;
        }
        return true;
    }

    //유저 마크--------------------------------------------------------------------------------------------------------------------------------
    //public static Texture GetUserMarkIconTexture()
    //{
    //    var data = GameInfo.Instance.GameTable.FindUserMark(GameInfo.Instance.UserData.UserMarkID);
    //    if (data == null)
    //        return null;
    //    return (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/User/" + data.Icon + ".png");
    //}

    //public static Texture GetUserMarkIconTexture(int markTID)
    //{
    //    var data = GameInfo.Instance.GameTable.FindUserMark(x => x.ID == markTID);
    //    if (data == null)
    //        return null;

    //    return (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/User/" + data.Icon + ".png");
    //}

    //계정 경험치--------------------------------------------------------------------------------------------------------------------------------
    public static bool IsMaxAccountLevel(int level)    //카드 최대 레벨
    {
        if (level >= GameInfo.Instance.GameConfig.AccountMaxLevel)
            return true;
        return false;
    }

    public static float GetAccountLevelExpGauge(int level, int exp)
    {
        float fillAmount = 0.0f;

        if (level >= GameInfo.Instance.GameConfig.AccountMaxLevel)
        {
            fillAmount = 1.0f;
        }
        else
        {
            int expmin = 0;
            int expmax = 0;
            GameSupport.GetAccountLevelExpMinMax(level, ref expmin, ref expmax);
            expmax -= expmin;
            int expnow = exp - expmin;
            fillAmount = (float)expnow / (float)expmax;
        }
        return fillAmount;
    }

    public static void GetAccountLevelExpMinMax(int level, ref int min, ref int max)    //계정경험치
    {

        var cardlevelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == GameInfo.Instance.GameConfig.AccountLevelUpGroup && x.Level == level);
        if (cardlevelupdata == null)
            return;

        if (level <= 1)
        {
            min = 0;
        }
        else
        {
            var beforecardlevelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == GameInfo.Instance.GameConfig.AccountLevelUpGroup && x.Level == level - 1);
            if (beforecardlevelupdata == null)
                return;
            min = beforecardlevelupdata.Exp;
        }
        max = cardlevelupdata.Exp;
    }

    public static int GetRemainAccountExpToMaxLevel(int userexp)
    {
        var levelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == GameInfo.Instance.GameConfig.AccountLevelUpGroup && x.Level == GameInfo.Instance.GameConfig.AccountMaxLevel - 1);
        if (levelupdata == null)
            return 0;
        return levelupdata.Exp - userexp;
    }

    public static int GetAccountExpLevel(int exp)
    {
        var leveluplist = GameInfo.Instance.GameTable.FindAllLevelUp(x => x.Group == GameInfo.Instance.GameConfig.AccountLevelUpGroup);
        if (leveluplist == null)
            return 0;

        int nMin = 0;
        int nMax = 0;
        int nResult = GameInfo.Instance.GameConfig.AccountMaxLevel;

        for (int i = 0; i < leveluplist.Count; i++)
        {
            nMax = leveluplist[i].Exp;
            if (exp < nMax && exp >= nMin)
            {
                nResult = leveluplist[i].Level;
                break;
            }
            nMin = leveluplist[i].Exp;
        }
        return nResult;
    }


    //캐릭터 경험치--------------------------------------------------------------------------------------------------------------------------------
    public static bool IsCharOpenTerms_Favor(CharData chardata)
    {
        if (chardata.Level >= GameInfo.Instance.GameConfig.CharOpenTermsLevel ||
            GameInfo.Instance.GameConfig.CharOpenTermsPreference <= chardata.FavorLevel)
            return true;
        //if (IsMaxCharLevel(chardata.Level, chardata.Grade) && IsMaxGrade(chardata.Grade))
        //    return true;
        return false;
    }

    public static int GetCharMaxLevel(int grade)
    {
        return GameInfo.Instance.GameConfig.CharMaxLevel[grade];
    }

    public static bool IsMaxCharLevel(int level, int grade)    //카드 최대 레벨
    {
        if (level >= GetCharMaxLevel(grade))
            return true;
        return false;
    }

    public static bool IsMaxGrade(int grade)
    {
        if (grade >= GameInfo.Instance.GameConfig.CharMaxGrade)
            return true;

        return false;
    }

    public static bool IsPossibleToCharAwaken(int currentCharLevel, int currentCharGrade)
    { // 6등급 이상에 최대 레벨이면 각성 가능
        if (currentCharGrade < GameInfo.Instance.GameConfig.CharStartAwakenGrade)
        {
            return false;
        }

        return IsMaxCharLevel(currentCharLevel, currentCharGrade);
    }

    public static float GetCharacterLevelExpGauge(int level, int grade, int exp)
    {
        float fillAmount = 0.0f;
        if (level >= GetCharMaxLevel(grade))
        {
            fillAmount = 1.0f;
        }
        else
        {
            int expmin = 0;
            int expmax = 0;
            GameSupport.GetCharacterLevelExpMinMax(level, ref expmin, ref expmax);
            expmax -= expmin;
            int expnow = exp - expmin;
            fillAmount = (float)expnow / (float)expmax;
        }
        return fillAmount;
    }

    public static void GetCharacterLevelExpMinMax(int level, ref int min, ref int max)  //캐릭터 경험치
    {
        var cardlevelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == GameInfo.Instance.GameConfig.CharLevelUpGroup && x.Level == level);
        if (cardlevelupdata == null)
            return;

        if (level <= 1)
        {
            min = 0;
        }
        else
        {
            var beforecardlevelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == GameInfo.Instance.GameConfig.CharLevelUpGroup && x.Level == level - 1);
            if (beforecardlevelupdata == null)
                return;
            min = beforecardlevelupdata.Exp;
        }
        max = cardlevelupdata.Exp;
    }

    public static int GetRemainCharExpToMaxLevel(int charexp, int grade)
    {
        var levelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == GameInfo.Instance.GameConfig.CharLevelUpGroup && x.Level == GameSupport.GetCharMaxLevel(grade) - 1);
        if (levelupdata == null)
            return 0;
        return levelupdata.Exp - charexp;
    }

    public static int GetCharacterExpLevel(int exp, int grade)
    {
        var leveluplist = GameInfo.Instance.GameTable.FindAllLevelUp(x => x.Group == GameInfo.Instance.GameConfig.CharLevelUpGroup);
        if (leveluplist == null)
            return 0;

        int nMin = 0;
        int nMax = 0;
        int nResult = GetCharMaxLevel(grade);

        for (int i = 0; i < leveluplist.Count; i++)
        {
            nMax = leveluplist[i].Exp;
            if (exp < nMax && exp >= nMin)
            {
                nResult = leveluplist[i].Level;
                break;
            }
            nMin = leveluplist[i].Exp;
        }
        return nResult;
    }



    //캐릭터 호감도 경험치--------------------------------------------------------------------------------------------------------------------------------
    public static float GetFavorLevelExpGauge(int level, int exp)
    {
        float fillAmount = 0.0f;
        if (level >= GameInfo.Instance.GameConfig.CharFavorMaxLevel)
        {
            fillAmount = 1.0f;
        }
        else
        {
            int expmin = 0;
            int expmax = 0;
            GameSupport.GetFavorLevelExpMinMax(level, ref expmin, ref expmax);
            expmax -= expmin;
            int expnow = exp - expmin;
            fillAmount = (float)expnow / (float)expmax;
        }
        return fillAmount;
    }

    public static void GetFavorLevelExpMinMax(int level, ref int min, ref int max)  //호감도 경험치
    {
        var cardlevelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == GameInfo.Instance.GameConfig.CharFavorLevelUpGroup && x.Level == level);
        if (cardlevelupdata == null)
            return;

        if (level <= 0)
        {
            min = 0;
        }
        else
        {
            var beforecardlevelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == GameInfo.Instance.GameConfig.CharFavorLevelUpGroup && x.Level == level - 1);
            if (beforecardlevelupdata == null)
                return;
            min = beforecardlevelupdata.Exp;
        }
        max = cardlevelupdata.Exp;
    }

    //카드 호감도 경험치--------------------------------------------------------------------------------------------------------------------------------
    public static float GetCardFavorLevelExpGauge(int tableid, int level, int exp)
    {
        float fillAmount = 0.0f;
        if (level >= GameInfo.Instance.GameConfig.CardFavorMaxLevel)
        {
            fillAmount = 1.0f;
        }
        else
        {
            int expmin = 0;
            int expmax = 0;
            GameSupport.GetCardFavorLevelExpMinMax(tableid, level, ref expmin, ref expmax);
            expmax -= expmin;
            int expnow = exp - expmin;
            fillAmount = (float)expnow / (float)expmax;
        }
        return fillAmount;
    }

    public static void GetCardFavorLevelExpMinMax(int tableid, int level, ref int min, ref int max)  //호감도 경험치
    {
        var tableData = GameInfo.Instance.GameTable.FindCard(tableid);
        if (tableData == null)
            return;

        var cardlevelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == tableData.FavorGroup && x.Level == level);
        if (cardlevelupdata == null)
            return;

        if (level <= 0)
        {
            min = 0;
        }
        else
        {
            var beforecardlevelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == tableData.FavorGroup && x.Level == level - 1);
            if (beforecardlevelupdata == null)
                return;
            min = beforecardlevelupdata.Exp;
        }
        max = cardlevelupdata.Exp;
    }

    public static int GetRemainCardFavorToMaxLevel(CardBookData cardbookdata)
    {
        var tableData = GameInfo.Instance.GameTable.FindCard(cardbookdata.TableID);
        if (tableData == null)
            return 0;

        var levelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == tableData.FavorGroup && x.Level == GameInfo.Instance.GameConfig.CardFavorMaxLevel - 1);
        if (levelupdata == null)
            return 0;

        return levelupdata.Exp - cardbookdata.FavorExp;
    }

    public static int GetCardFavorLevel(int tableid, int exp)
    {
        var tableData = GameInfo.Instance.GameTable.FindCard(tableid);
        if (tableData == null)
            return 0;

        var leveluplist = GameInfo.Instance.GameTable.FindAllLevelUp(x => x.Group == tableData.FavorGroup);
        if (leveluplist == null)
            return 0;

        int nMin = 0;
        int nMax = 0;
        int nResult = GameInfo.Instance.GameConfig.CardFavorMaxLevel;

        for (int i = 0; i < leveluplist.Count; i++)
        {
            nMax = leveluplist[i].Exp;
            if (exp < nMax && exp >= nMin)
            {
                nResult = leveluplist[i].Level;
                break;
            }
            nMin = leveluplist[i].Exp;
        }

        return nResult;
    }


    //카드 경험치--------------------------------------------------------------------------------------------------------------------------------
    public static int GetCardImageNum(CardData carddata)
    {
        if (IsMaxLevelCard(carddata) && IsMaxWakeCard(carddata))
            return 1;
        return 0;
    }
    public static float GetCardLevelExpGauge(CardData carddata, int level, int exp)
    {
        float fillAmount = 0.0f;

        if (level >= GetMaxLevelCard(carddata))
        {
            fillAmount = 1.0f;
        }
        else
        {
            int expmin = 0;
            int expmax = 0;
            GameSupport.GetCardLevelExpMinMax(carddata.TableData.LevelUpGroup, carddata.TableData.Grade, level, ref expmin, ref expmax);
            expmax -= expmin;
            int expnow = exp - expmin;
            fillAmount = (float)expnow / (float)expmax;
        }
        return fillAmount;
    }

    public static void GetCardLevelExpMinMax(int group, int grade, int level, ref int min, ref int max)    //계정경험치
    {
        var cardlevelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == group && x.Level == level);
        if (cardlevelupdata == null)
            return;

        if (level <= 1)
        {
            min = 0;
        }
        else
        {
            var beforecardlevelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == group && x.Level == level - 1);
            if (beforecardlevelupdata == null)
                return;
            min = beforecardlevelupdata.Exp;
        }
        max = cardlevelupdata.Exp;
    }

    //무기 경험치--------------------------------------------------------------------------------------------------------------------------------
    public static float GetWeaponLevelExpGauge(WeaponData carddata, int level, int exp)
    {
        float fillAmount = 0.0f;

        if (level >= GetWeaponMaxLevel(carddata))
        {
            fillAmount = 1.0f;
        }
        else
        {
            int expmin = 0;
            int expmax = 0;
            GameSupport.GetWeapoLevelExpMinMax(carddata.TableData.LevelUpGroup, carddata.TableData.Grade, level, ref expmin, ref expmax);
            expmax -= expmin;
            int expnow = exp - expmin;
            fillAmount = (float)expnow / (float)expmax;
        }
        return fillAmount;
    }

    public static void GetWeapoLevelExpMinMax(int group, int grade, int level, ref int min, ref int max)    //계정경험치
    {
        var cardlevelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == group && x.Level == level);
        if (cardlevelupdata == null)
            return;

        if (level <= 1)
        {
            min = 0;
        }
        else
        {
            var beforecardlevelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == group && x.Level == level - 1);
            if (beforecardlevelupdata == null)
                return;
            min = beforecardlevelupdata.Exp;
        }
        max = cardlevelupdata.Exp;
    }

    //캐릭터 생성--------------------------------------------------------------------------------------------------------------------------------
    public static LobbyPlayer CreateLobbyPlayer(CharData charData)
    {
        return CreateLobbyPlayer(charData.TableID, charData);
    }

    public static LobbyPlayer CreateLobbyPlayer(int tableid, CharData charData)
    {
        GameTable.Character.Param param = GameInfo.Instance.GetCharacterData(tableid);
        if (param == null)
            return null;

        string folder = Utility.GetFolderFromPath(param.Model);
        string name = Utility.GetNameFromPath(param.Model);

        string strModel = string.Format("Unit/{0}/{1}_L/{1}_L.prefab", folder, name);
        LobbyPlayer lobbyplayer = ResourceMgr.Instance.CreateFromAssetBundle<LobbyPlayer>("unit", strModel);
		if (lobbyplayer == null)
		{
			Debug.LogError(strModel + "을 만들지 못했습니다.");
			return null;
		}
		else if(FSaveData.Instance.Graphic <= 0)
		{
			lobbyplayer.RemoveShadow();
		}

        if (charData == null)
        {
            lobbyplayer.costumeUnit.InitCostumeChar(param.InitCostume, false);
		}
        else
        {
            lobbyplayer.costumeUnit.InitCostumeChar(charData, false);
        }

        lobbyplayer.Init(tableid, eCharacterType.Character, null);

        if (charData != null)
        {
            lobbyplayer.Uid = charData.CUID;
        }

		lobbyplayer.costumeUnit.SetCostume(charData, false);

        if (charData == null)
        {
			uint costumeFlag = 0;

			GameTable.Costume.Param find = GameInfo.Instance.GameTable.FindCostume(x => x.ID == param.InitCostume);
			if (find != null && find.SubHairChange == 1)
			{
				bool isOn = _IsOnBitIdx(costumeFlag, (int)(eCostumeStateFlag.CSF_HAIR));
				_DoOnOffBitIdx(ref costumeFlag, (int)(eCostumeStateFlag.CSF_HAIR), true);
			}

			//도감에서 열때 강제로 기본 코스튬 기본색상으로 셋팅
			lobbyplayer.costumeUnit.SetCostumeBody(param.InitCostume, (int)eCOUNT.NONE, (int)costumeFlag, null);
        }
        lobbyplayer.GetBones();

        return lobbyplayer;
    }

    //첫 캐릭터 생성
    public static LobbyPlayer CreateLobbyPlayer(int tableid, int costumeID, CharData charData, bool isBook = false)
    {
        GameTable.Character.Param param = GameInfo.Instance.GetCharacterData(tableid);
        if (param == null)
            return null;

        string folder = Utility.GetFolderFromPath(param.Model);
        string name = Utility.GetNameFromPath(param.Model);

        string strModel = string.Format("Unit/{0}/{1}_L/{1}_L.prefab", folder, name);
        LobbyPlayer lobbyplayer = ResourceMgr.Instance.CreateFromAssetBundle<LobbyPlayer>("unit", strModel);
		if (lobbyplayer == null)
		{
			Debug.LogError(strModel + "을 만들지 못했습니다.");
			return null;
		}
		else if (FSaveData.Instance.Graphic <= 0)
		{
			lobbyplayer.RemoveShadow();
		}

		if (charData == null)
        {
            lobbyplayer.costumeUnit.InitCostumeChar(costumeID, false);
		}
        else
        {
            lobbyplayer.costumeUnit.InitCostumeChar(charData, false);
        }

        lobbyplayer.Init(tableid, eCharacterType.Character, null);

        if (charData != null)
        {
            lobbyplayer.Uid = charData.CUID;
        }

        lobbyplayer.costumeUnit.SetCostume(charData, false);

        if (charData == null)
        {
			int costumeId = 0;
			if (!isBook && GameInfo.Instance.CharList.Count > 0)
			{
				costumeId = param.InitCostume;
			}
			else
			{
				costumeId = costumeID;
			}

			uint costumeFlag = 0;

			GameTable.Costume.Param find = GameInfo.Instance.GameTable.FindCostume(x => x.ID == costumeId);
			if (find != null && find.SubHairChange == 1)
			{
				bool isOn = _IsOnBitIdx(costumeFlag, (int)(eCostumeStateFlag.CSF_HAIR));
				_DoOnOffBitIdx(ref costumeFlag, (int)(eCostumeStateFlag.CSF_HAIR), true);
			}

			if (!isBook && GameInfo.Instance.CharList.Count > 0)
            {
                lobbyplayer.costumeUnit.SetCostumeBody(param.InitCostume, (int)eCOUNT.NONE, (int)costumeFlag, null);
            }
            else
            {
                lobbyplayer.costumeUnit.SetCostumeBody(costumeID, (int)eCOUNT.NONE, (int)costumeFlag, null);
            }
            //도감에서 열때 강제로 기본 코스튬 기본색상으로 셋팅
            
        }
        lobbyplayer.GetBones();

        return lobbyplayer;
    }

	public static Player CreatePlayer( CharData charData, bool isFriendData = false, bool opponentPlayer = false, 
                                       bool isHelper = false, bool isAutoPlay = false ) {
		GameTable.Character.Param param = GameInfo.Instance.GetCharacterData( charData.TableData.ID );
		if( param == null ) {
			Debug.LogError( charData.TableData.ID + "번은 존재 하지 않는 캐릭터 테이블 아이디 입니다." );
			return null;
		}

		string folder = Utility.GetFolderFromPath( param.Model );
		string name = Utility.GetNameFromPath( param.Model );

		string strModel = string.Format( "Unit/{0}/{1}_G/{1}_G.prefab", folder, name );
		Player player = ResourceMgr.Instance.CreateFromAssetBundle<Player>( "unit", strModel );
		if( player == null ) {
			Debug.LogError( strModel + "에 캐릭터를 불러올 수 없습니다." );
			return null;
		}
		else if( FSaveData.Instance.Graphic <= 0 ) {
			player.RemoveShadow();
		}

        player.IsHelper = isHelper;
        player.AutoPlay = isAutoPlay;

        if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP || isFriendData ) {
			player.SetCharData( charData );
		}

		player.OpponentPlayer = opponentPlayer;
		player.IsFriendChar = isFriendData;

		player.costumeUnit.InitCostumeChar( charData, true );
		player.Init( charData.TableData.ID, eCharacterType.Character, param.Face );

		// PVP 모드랑 친구 캐릭터는 따로 설정
		if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP && !isFriendData ) {
			player.costumeUnit.SetCostume( charData );
			player.GetBones();
		}

		//레이스 모드에서 클론 생성 안하도록 수정
		if( World.Instance.StageType != eSTAGETYPE.STAGE_SPECIAL && World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			player.CreateClone( player.CloneCount );
		}

        if ( charData.IsHideWeapon ) {
			player.costumeUnit.ShowObject( player.costumeUnit.Param.LobbyOnly, true );
		}

		if ( player.UseGuardian && World.Instance.StageType != eSTAGETYPE.STAGE_PVP && !isFriendData ) {
			player.costumeUnit.SetGuardianOwnerPlayer( player, false );

            if ( AppMgr.Instance.SceneType != AppMgr.eSceneType.Training ) {
				SetGuardianSkill( player, charData );
			}
		}

		return player;
	}

	public static PlayerMinion CreatePlayerMinion(int tableId, Unit owner)
    {
        GameClientTable.Monster.Param param = GameInfo.Instance.GetMonsterData(tableId);
        if (param == null)
        {
            Debug.LogError(tableId + "번은 존재 하지 않는 몬스터 테이블 아이디 입니다. (미니언 생성에 사용)");
            return null;
        }

        string name = FLocalizeString.Instance.GetText(param.Name);
        string platform = (AppMgr.Instance.ResPlatform == AppMgr.eResPlatform.aos && param.Platform == 1) ? "_aos" : "";
        string path = Utility.AppendString("Unit/", param.ModelPb + platform, ".prefab");

        GameObject gObj = ResourceMgr.Instance.CreateFromAssetBundle("unit", path);

        ActionEnemySummonMinion actionEnemySummonMinion = gObj.GetComponent<ActionEnemySummonMinion>();
        GameObject.DestroyImmediate(gObj.GetComponent<Enemy>());

        PlayerMinion minion = gObj.AddComponent<PlayerMinion>();
		if(minion && FSaveData.Instance.Graphic <= 0)
		{
			minion.RemoveShadow();
		}

        minion.Init(param.ID, eCharacterType.Monster, null);
        minion.Deactivate();

        if (minion.cmptBuffDebuff)
        {
            minion.cmptBuffDebuff.ResetOwner();
        }

        if (minion.cmptMovement)
        {
            minion.cmptMovement.ResetOwner();
        }

        if (minion.cmptRotate)
        {
            minion.cmptRotate.ResetOwner();
        }

        if (minion.cmptJump)
        {
            minion.cmptJump.ResetOwner();
        }

        owner.AddPlayerMinion(minion);
        minion.SetOwner(owner);

        if (actionEnemySummonMinion)
        {
            ActionPlayerMinionSummonMinion actionPlayerMinionSummonMinion = gObj.AddComponent<ActionPlayerMinionSummonMinion>();
            if (actionPlayerMinionSummonMinion)
            {
                actionPlayerMinionSummonMinion.SetSuperPlayer(owner);
                actionPlayerMinionSummonMinion.Init(0, null);
                actionPlayerMinionSummonMinion.InitAfterOwnerInit();
            }

            GameObject.DestroyImmediate(actionEnemySummonMinion);
        }

        return minion;
    }

    public static Enemy CreateEnemy(int tableId)
    {
        GameClientTable.Monster.Param param = GameInfo.Instance.GetMonsterData(tableId);
        if (param == null)
        {
            Debug.LogError(tableId + "번은 존재 하지 않는 몬스터 테이블 아이디 입니다.");
            return null;
        }

        string name = FLocalizeString.Instance.GetText(param.Name);
        string path = Utility.AppendString("Unit/", param.ModelPb, ".prefab");

        Enemy enemy = World.Instance.EnemyMgr.CreateEnemy(param);
        if (enemy == null)
        {
            return null;
        }
		else if(FSaveData.Instance.Graphic <= 0)
		{
			enemy.RemoveShadow();
		}

        enemy.CreateMonsterClone(enemy.CloneCount);
        return enemy;
    }

    public static Enemy CreateEnemyWithoutAddEnemy(int tableId)
    {
        GameClientTable.Monster.Param param = GameInfo.Instance.GetMonsterData(tableId);
        if (param == null)
        {
            Debug.LogError(tableId + "번은 존재 하지 않는 몬스터 테이블 아이디 입니다.");
            return null;
        }

        string name = FLocalizeString.Instance.GetText(param.Name);
        string path = Utility.AppendString("Unit/", param.ModelPb, ".prefab");

        Enemy enemy = World.Instance.EnemyMgr.CreateEnemyWithoutAddEnemy(param);
		if (enemy == null)
		{
			return null;
		}
		else if (FSaveData.Instance.Graphic <= 0)
		{
			enemy.RemoveShadow();
		}

		return enemy;
    }

    public static FigureUnit CreateFigure(int tableId, string faceAnimatorPath, bool inPrivateRoom)
    {
        GameTable.RoomFigure.Param param = GameInfo.Instance.GameTable.FindRoomFigure(tableId);
        if (param == null)
        {
            Debug.LogError(tableId + "번 피규어를 찾을 수 없습니다.");
            return null;
        }

        string name = System.IO.Path.GetFileName(param.Model);
        string platform = string.Empty;

#if UNITY_EDITOR
        if (param.Platform != 0)
            platform = AppMgr.Instance.ResPlatform == AppMgr.eResPlatform.aos ? "_aos" : "";
#endif

        string path = string.Format("Unit/{0}_F/{1}_F{2}.prefab", param.Model, name, platform);

        FigureUnit figure = ResourceMgr.Instance.CreateFromAssetBundle<FigureUnit>("unit", path);
        if (figure == null)
        {
            Debug.LogError(tableId + "번 피규어유닛을 생성할 수 없습니다.");
            return null;
        }
		else if (FSaveData.Instance.Graphic <= 0)
		{
			figure.RemoveShadow();
		}

		if (inPrivateRoom)
        {
            figure.withoutAniEvent = true;

            AniEvent aniEvent = figure.GetComponentInChildren<AniEvent>();
            if (aniEvent)
            {
                Animator animator = aniEvent.GetComponent<Animator>();

                GameObject.DestroyImmediate(aniEvent);
                GameObject.DestroyImmediate(animator);
            }
        }

        figure.Init(tableId, eCharacterType.Figure, faceAnimatorPath);

        if (inPrivateRoom && param.ContentsType == (int)eContentsPosKind.COSTUME)
        {
            figure.SetFaceAnimatorController();
        }

        return figure;
    }

    public static FigureUnit CreateFigure(string figurePath, string faceAnimatorPath)
    {
        string name = System.IO.Path.GetFileName(figurePath);
        string path = string.Format("Unit/{0}_F/{1}_F.prefab", figurePath, name);
        FigureUnit figure = ResourceMgr.Instance.CreateFromAssetBundle<FigureUnit>("unit", path);
        if (figure == null)
        {
            Debug.LogError(figurePath + " 경로에 피규어가 존재하지 않습니다.");
            return null;
        }
		else if (FSaveData.Instance.Graphic <= 0)
		{
			figure.RemoveShadow();
		}

        figure.Init(101001, eCharacterType.Figure, faceAnimatorPath);
        return figure;
    }

    public static FigureUnit CreateFigureWeapon(int tableId)
    {
        GameTable.RoomFigure.Param param = GameInfo.Instance.GameTable.FindRoomFigure(tableId);
        if (param == null)
        {
            Debug.LogError(tableId + "번 피규어 무기를 찾을 수 없습니다.");
            return null;
        }

        string name = System.IO.Path.GetFileName(param.Model);
        string path = string.Format("Item/Weapon/{0}.prefab", param.Model);
        FigureUnit figure = ResourceMgr.Instance.CreateFromAssetBundle<FigureUnit>("item", path);
        if (figure == null)
        {
            Debug.LogError(tableId + "번 피규어 무기를 생성할 수 없습니다.");
            return null;
        }

        figure.Init(tableId, eCharacterType.Figure, null);
        return figure;
    }

    //캐릭터 연출 파일--------------------------------------------------------------------------------------------------------------------------------
    public static Director CreateDirector(string path)
    {
        string fullPath = "Director/" + path;
        if (AppMgr.Instance.ResPlatform == AppMgr.eResPlatform.aos)
        {
            fullPath += "_aos";
        }
        Log.Show(fullPath);

        PlayableDirector playableDirector = ResourceMgr.Instance.CreateFromAssetBundle<PlayableDirector>("director", fullPath + ".prefab");
        if (playableDirector == null)
        {
            fullPath = fullPath.Replace("_aos", "");
            playableDirector = ResourceMgr.Instance.CreateFromAssetBundle<PlayableDirector>("director", fullPath + ".prefab");
        }

        Utility.InitTransform(playableDirector.gameObject);
        playableDirector.gameObject.SetActive(false);

        Director director = playableDirector.GetComponent<Director>();
        if (director == null)
        {
            Debug.LogError(path + " 연출 파일에 Director 스크립트가 없습니다.");
            return null;
        }

        return director;
    }

    public static Projectile CreateProjectile(string path)
    {
        UnityEngine.Object obj = ResourceMgr.Instance.LoadFromAssetBundle("projectile", path);
        if (obj == null)
        {
            Debug.LogError(path + " 발사체 에셋을 로드할 수 없습니다. ");
            return null;
        }

        Projectile projectile = ResourceMgr.Instance.Instantiate<Projectile>(ref obj);
        if (projectile == null)
        {
            Debug.LogError(path + " 발사체를 생성할 수 없습니다. ");
            return null;
        }

        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public static Unit CreateUnitWeapon(string path, bool bautoinit = true)
    {
        Unit unit = ResourceMgr.Instance.CreateFromAssetBundle<Unit>("unit", path + ".prefab");
        if (unit == null)
        {
            Debug.LogError(path + "무기를 로드할 수 없습니다.");
            return null;
        }

        if(bautoinit)
        {
            unit.Init(0, eCharacterType.Other, string.Empty);
            unit.Deactivate();
        }

        return unit;
    }

    public static ParticleSystem CreateParticle(ParticleSystem ps, Transform parent)
    {
        ParticleSystem instance = null;

        GameObject gObj = ps.gameObject;
        instance = ResourceMgr.Instance.Instantiate<ParticleSystem>(ref gObj);
        instance.transform.SetParent(parent);
        Utility.InitTransform(instance.gameObject);
        instance.gameObject.SetActive(false);

        return instance;
    }

    public static ParticleSystem CreateParticle(string path, Transform parent)
    {
        ParticleSystem instance = null;

        UnityEngine.Object obj = ResourceMgr.Instance.LoadFromAssetBundle("effect", path);
        if (obj == null)
        {
            Debug.LogError(path + " 이펙트 에셋을 로드할 수 없습니다. ");
            return null;
        }

        instance = ResourceMgr.Instance.Instantiate<ParticleSystem>(ref obj);
        instance.transform.SetParent(parent);
        Utility.InitTransform(instance.gameObject);
        instance.gameObject.SetActive(false);

        return instance;
    }

    public static GameObject CreateUIEffect(string effectName, Transform parent)
    {
        GameObject goEffect = null;
        goEffect = ResourceMgr.Instance.CreateFromAssetBundle("effect", string.Format("Effect/UI/{0}.prefab", effectName));
        if (goEffect != null)
        {
            goEffect.transform.SetParent(parent);
            goEffect.transform.localPosition = Vector3.zero;
            goEffect.transform.localScale = Vector3.one;
            goEffect.SetActive(false);
        }

        return goEffect;
    }

    public static void PlayParticle(GameObject obj)
    {
        obj.SetActive(true);
        ParticleSystem[] ps = obj.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < ps.Length; i++)
            ps[i].Stop();
        for (int i = 0; i < ps.Length; i++)
            ps[i].Play();
    }

    public static void StopParticle(GameObject obj)
    {
        obj.SetActive(false);
        ParticleSystem[] ps = obj.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < ps.Length; i++)
            ps[i].Stop();
    }

    //선택된 캐릭터--------------------------------------------------------------------------------------------------------------------------------
    public static CharData GetGameSeleteCharUID()
    {
        if (GameInfo.Instance.SeleteCharUID == -1)
            return GameInfo.Instance.GetMainChar();

        return GameInfo.Instance.GetCharData(GameInfo.Instance.SeleteCharUID);
    }

    public static CharData GetCharDataByUidOrNull(long uid)
    {
        if (uid <= 0)
        {
            return null;
        }

        return GameInfo.Instance.GetCharData(uid);
    }

    public static CharData GetGameSeleteTableId(int tableId)
    {
        return GameInfo.Instance.GetCharDataByTableID(tableId);
    }

    public static int GetCombatPower(CharData chardata, eWeaponSlot wpnSlot, eContentsPosKind contPos = eContentsPosKind._NONE_, PresetWeaponData presetMainWeaponData = null, PresetWeaponData presetSubWeaponData = null)
    {
        WeaponData mainSourWeaponData = null;
        if (contPos == eContentsPosKind.ARENA_TOWER)
        {
            mainSourWeaponData = GameInfo.Instance.GetArenaTowerWeaponData(chardata);
        }
        else
        {
            mainSourWeaponData = GameInfo.Instance.GetWeaponData(chardata.EquipWeaponUID);
        }

        WeaponData mainDestWeaponData = null;
        if (mainSourWeaponData != null && presetMainWeaponData != null)
        {
            mainDestWeaponData = mainSourWeaponData.PresetDataClone(presetMainWeaponData);
        }

        WeaponData mainWeaponData = mainDestWeaponData == null ? mainSourWeaponData : mainDestWeaponData;

        BattleConfig bc = GameInfo.Instance.BattleConfig;
        float totalCombatPower = 0;

        // 기본 스탯 전투력 계산
        if (presetMainWeaponData != null || presetSubWeaponData != null)
        {
            WeaponData subDestWeaponData = null;
            WeaponData subSourWeaponData = GameInfo.Instance.GetWeaponData(chardata.EquipWeapon2UID);
            if (subSourWeaponData != null && presetSubWeaponData != null)
            {
                subDestWeaponData = subSourWeaponData.PresetDataClone(presetSubWeaponData);
            }

            totalCombatPower += (int)(GetTotalHP(chardata, mainWeaponData, subDestWeaponData) * bc.CombatPowerBaseMag / bc.CombatPowerHPMag);
            totalCombatPower += (int)(GetTotalATK(chardata, mainWeaponData, subDestWeaponData) * bc.CombatPowerBaseMag / bc.CombatPowerATKMag);
            totalCombatPower += (int)(GetTotalDEF(chardata, mainWeaponData, subDestWeaponData) * bc.CombatPowerBaseMag / bc.CombatPowerDEFMag);
            totalCombatPower += (int)(GetTotalCRI(chardata, mainWeaponData, subDestWeaponData) * bc.CombatPowerBaseMag / bc.CombatPowerCRIMag);
        }
        else
        {
            totalCombatPower += (int)(GetTotalHP(chardata, wpnSlot) * bc.CombatPowerBaseMag / bc.CombatPowerHPMag);
            totalCombatPower += (int)(GetTotalATK(chardata, wpnSlot) * bc.CombatPowerBaseMag / bc.CombatPowerATKMag);
            totalCombatPower += (int)(GetTotalDEF(chardata, wpnSlot) * bc.CombatPowerBaseMag / bc.CombatPowerDEFMag);
            totalCombatPower += (int)(GetTotalCRI(chardata, wpnSlot) * bc.CombatPowerBaseMag / bc.CombatPowerCRIMag);
        }


        // 캐릭터 패시브 스킬 전투력 계산
        float totalCharSkillLv = 0;
        var upgradeskilllist = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.CharacterID == chardata.TableID && (x.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_NORMAL || x.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_ULTIMATE));
        for (int i = 0; i < upgradeskilllist.Count; i++)
        {
            var passviedata = chardata.PassvieList.Find(x => x.SkillID == upgradeskilllist[i].ID);
            if (passviedata != null)
            {
                totalCharSkillLv += passviedata.SkillLevel;
            }
        }

        if (contPos == eContentsPosKind.ARENA || contPos == eContentsPosKind.ARENA_TOWER)
        {
            totalCharSkillLv *= bc.ArenaCharSkillOptRate;
        }

        totalCombatPower += (int)(totalCharSkillLv * bc.CombatPowerBaseMag * bc.CombatPowerCharSkillMag);

        // 서포터 스킬 레벨 전투력 계산
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            CardData equipCard = GameInfo.Instance.GetCardData(chardata.EquipCard[i]);
            if (equipCard != null)
            {
                totalCombatPower += (int)(equipCard.SkillLv * bc.CombatPowerBaseMag * bc.CombatPowerSptSkillMag * bc.CombatPowerSptSkillRateByGrade[equipCard.TableData.Grade]);
            }
        }


        if (mainWeaponData == null)
        {
            return (int)totalCombatPower;
        }

        // 무기 스킬 레벨 전투력 계산
        totalCombatPower += (int)(mainWeaponData.SkillLv * bc.CombatPowerBaseMag * bc.CombatPowerWpnSkillMag * bc.CombatPowerWpnSkillRateByGrade[mainWeaponData.TableData.Grade]);

        // 곡옥 옵션 전투력 계산
        float totalGemOpt = 0;
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {
            if (mainWeaponData.SlotGemUID[i] == (int)eCOUNT.NONE)
            {
                continue;
            }

            GemData gemdata = null;
            if (contPos == eContentsPosKind.ARENA_TOWER)
            {
                if (presetMainWeaponData != null)
                {
                    gemdata = GameInfo.Instance.GetGemData(mainWeaponData.SlotGemUID[i]);
                }
                else
                {
                    gemdata = GameInfo.Instance.GetArenaTowerGemData(chardata, i);
                }
            }
            else
            {
                gemdata = GameInfo.Instance.GetGemData(mainWeaponData.SlotGemUID[i]);
            }

            if (gemdata == null)
            {
                continue;
            }

            for (int j = 0; j < (int)eCOUNT.GEMRANDOPT; j++)
            {
                if (gemdata.RandOptValue[j] > 0)
                {
                    totalGemOpt += gemdata.RandOptValue[j];
                }
            }
        }
        if (contPos == eContentsPosKind.ARENA || contPos == eContentsPosKind.ARENA_TOWER)
        {
            totalGemOpt *= bc.ArenaGemOptRate;
        }

        totalCombatPower += (int)(totalGemOpt * bc.CombatPowerBaseMag * bc.CombatPowerGemOptMag);

        return (int)(totalCombatPower);
    }

    public static int GetCombatPower(TeamCharData temachardata, eWeaponSlot wpnSlot = eWeaponSlot.MAIN)
    {   
        BattleConfig bc = GameInfo.Instance.BattleConfig;

        float totalCombatPower = 0;

        if (temachardata == null)
            return (int)(totalCombatPower);
        
        CharData chardata = temachardata.CharData;

        // 기본 스탯 전투력 계산
        totalCombatPower += (int)(GetTotalHP(chardata, wpnSlot) * bc.CombatPowerBaseMag / bc.CombatPowerHPMag); 
        totalCombatPower += (int)(GetTotalATK(chardata, wpnSlot) * bc.CombatPowerBaseMag / bc.CombatPowerATKMag); 
        totalCombatPower += (int)(GetTotalDEF(chardata, wpnSlot) * bc.CombatPowerBaseMag / bc.CombatPowerDEFMag); 
        totalCombatPower += (int)(GetTotalCRI(chardata, wpnSlot) * bc.CombatPowerBaseMag / bc.CombatPowerCRIMag); 

        // 캐릭터 패시브 스킬 전투력 계산
        float totalCharSkillLv = 0;
        var upgradeskilllist = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.CharacterID == chardata.TableID && (x.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_NORMAL || x.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_ULTIMATE));
        for (int i = 0; i < upgradeskilllist.Count; i++)
        {
            var passviedata = chardata.PassvieList.Find(x => x.SkillID == upgradeskilllist[i].ID);
            if (passviedata != null)
                totalCharSkillLv += passviedata.SkillLevel;
        }
        
        totalCombatPower += (int)(totalCharSkillLv * bc.CombatPowerBaseMag * bc.CombatPowerCharSkillMag); 

        // 서포터 스킬 레벨 전투력 계산
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            //CardData equipCard = GameInfo.Instance.GetCardData(chardata.EquipCard[i]);
            CardData equipCard = temachardata.GetCardData(i);
            if (equipCard != null)
                totalCombatPower += (int)(equipCard.SkillLv * bc.CombatPowerBaseMag * bc.CombatPowerSptSkillMag * bc.CombatPowerSptSkillRateByGrade[equipCard.TableData.Grade]);
        }
        
        // 무기 스킬 레벨 전투력 계산        
        WeaponData equipWpn = temachardata.MainWeaponData;
        totalCombatPower += (int)(equipWpn.SkillLv * bc.CombatPowerBaseMag * bc.CombatPowerWpnSkillMag * bc.CombatPowerWpnSkillRateByGrade[equipWpn.TableData.Grade]);
        
        // 곡옥 옵션 전투력 계산
        float totalGemOpt = 0;
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {
            if (equipWpn.SlotGemUID[i] == (int)eCOUNT.NONE)
                continue;
            //GemData gemdata = GameInfo.Instance.GetGemData(equipWpn.SlotGemUID[i]);
            GemData gemdata = temachardata.GetMainGemData(i);
            if (gemdata == null)
                continue;
            for (int j = 0; j < (int)eCOUNT.GEMRANDOPT; j++)
            {
                if (gemdata.RandOptValue[j] > 0)
                    totalGemOpt += gemdata.RandOptValue[j];
            }
        }
        
        totalCombatPower += (int)(totalGemOpt * bc.CombatPowerBaseMag * bc.CombatPowerGemOptMag);
        
        return (int)(totalCombatPower);
    }

    public static string GetCombatPowerString(CharData chardata, eWeaponSlot wpnSlot, eContentsPosKind contPos = eContentsPosKind._NONE_)
    {
        return string.Format(FLocalizeString.Instance.GetText(270), GetCombatPower(chardata, wpnSlot, contPos));
    }

    public static string GetCombatPowerString(TeamCharData teamCharData)
    {
        return string.Format(FLocalizeString.Instance.GetText(270), GetEnemyCombatPower(teamCharData, eWeaponSlot.MAIN, eContentsPosKind.ARENA_TOWER));
    }
    //무기 관련--------------------------------------------------------------------------------------------------------------------------------
    public static int GetWeaponMatExp(List<WeaponData> matlist)
    {
        int exptotal = 0;
        for (int i = 0; i < matlist.Count; i++)
        {
            exptotal += matlist[i].TableData.Exp;
            if (matlist[i].Exp != 0)
                exptotal += (int)((float)matlist[i].Exp * GameInfo.Instance.GameConfig.WeaponExpMatWeigth);
        }
        return exptotal;
    }

    public static int GetItemMatExp(List<MatItemData> matlist)
    {
        int exptotal = 0;
        for (int i = 0; i < matlist.Count; i++)
        {
            exptotal += matlist[i].ItemData.TableData.Value * matlist[i].Count;
        }
        return exptotal;
    }

    public static int GetWeaponLevelUpCost(WeaponData weapondata, List<WeaponData> matlist)
    {
        int matcnt = matlist.Count;
        int goldtotal = (int)(GameInfo.Instance.GameConfig.WeaponLevelupCostByLevel * weapondata.Level * matcnt);
        return goldtotal;
    }

    public static int GetWeaponLevelUpItemCost(WeaponData weapondata, List<MatItemData> matlist)
    {
        int count = 0;
        for (int i = 0; i < matlist.Count; i++)
            count += matlist[i].Count;
        int matcnt = count;
        int goldtotal = (int)(GameInfo.Instance.GameConfig.WeaponLevelupCostByLevel * weapondata.Level * matcnt);
        return goldtotal;
    }

    public static int GetRemainWeaponExpToMaxLevel(WeaponData weapondata)
    {
        var levelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == weapondata.TableData.LevelUpGroup && x.Level == GameSupport.GetWeaponMaxLevel(weapondata) - 1);
        if (levelupdata == null)
            return 0;
        return levelupdata.Exp - weapondata.Exp;
    }

    public static int GetWeaponSkillLevelUpCost(WeaponData weapondata, int cnt)
    {
        int goldtotal = (int)(GameInfo.Instance.GameConfig.WeaponSkillLevelupCostByGrade[weapondata.TableData.Grade] * cnt);
        return goldtotal;
    }

    public static bool IsMaxSkillLevelWeapon(int skilllv)
    {
        if (skilllv >= GameInfo.Instance.GameConfig.WeaponMaxSkillLevel )
            return true;
        return false;
    }

    public static bool IsMaxLevelWeapon(WeaponData weapondata)
    {
        if (weapondata.Level >= GetWeaponMaxLevel(weapondata))
            return true;
        return false;
    }

    public static bool IsMaxLevelWeapon(WeaponData weapondata, int level)
    {
        if (level >= GetWeaponMaxLevel(weapondata))
            return true;
        return false;
    }

    public static bool IsMaxWakeWeapon(WeaponData weapondata)
    {
        if (weapondata.Wake >= GameInfo.Instance.GameConfig.WeaponWakeMax[weapondata.TableData.Grade])
            return true;
        return false;
    }

    public static int GetMaxSkillLevelWeapon()
    {
        return GameInfo.Instance.GameConfig.WeaponMaxSkillLevel;
    }
    

    public static int GetWeaponMaxWake(WeaponData weapondata)
    {
        return GameInfo.Instance.GameConfig.WeaponWakeMax[weapondata.TableData.Grade];
    }

    public static int GetWeaponMaxLevel(WeaponData weapondata)
    {
        return GameInfo.Instance.GameConfig.WeaponMaxLevel[weapondata.TableData.Grade] + (GameInfo.Instance.GameConfig.WeaponWakeIncLevel[weapondata.TableData.Grade] * weapondata.Wake);
    }

    public static int GetWeaponMaxLevel(WeaponData weapondata, int wake)
    {
        return GameInfo.Instance.GameConfig.WeaponMaxLevel[weapondata.TableData.Grade] + (GameInfo.Instance.GameConfig.WeaponWakeIncLevel[weapondata.TableData.Grade] * wake);
    }

    public static int GetWeaponMaxLevel(int grade, int wake)
    {
        return GameInfo.Instance.GameConfig.WeaponMaxLevel[grade] + (GameInfo.Instance.GameConfig.WeaponWakeIncLevel[grade] * wake);
    }

    public static int GetWeaponExpLevel(WeaponData weapondata, int exp)
    {
        var leveluplist = GameInfo.Instance.GameTable.FindAllLevelUp(x => x.Group == weapondata.TableData.LevelUpGroup);
        if (leveluplist == null)
            return 0;

        int nMin = 0;
        int nMax = 0;
        int nResult = GameSupport.GetWeaponMaxLevel(weapondata);

        for (int i = 0; i < leveluplist.Count; i++)
        {
            nMax = leveluplist[i].Exp;
            if (exp < nMax && exp >= nMin)
            {
                nResult = leveluplist[i].Level;
                break;
            }
            nMin = leveluplist[i].Exp;
        }
        return nResult;
    }

    public static string GetWeaponSkillDesc(GameTable.Weapon.Param weapontabledata, int skilllv)
    {
        string text = string.Empty;
        float v1 = 0.0f; // {0}
        float v2 = 0.0f; // {1} 

        GameClientTable.BattleOptionSet.Param param = null;
        if (weapontabledata.WpnAddBOSetID1 > 0)
        {
            param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(weapontabledata.WpnAddBOSetID1);
            if (param != null)
                v1 = (float)param.BOFuncValue + (float)(param.BOFuncIncValue * (float)(skilllv - 1));
        }
        if (weapontabledata.WpnAddBOSetID2 > 0)
        {
            param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(weapontabledata.WpnAddBOSetID2);
            if (param != null)
                v2 = (float)param.BOFuncValue + (float)(param.BOFuncIncValue * (float)(skilllv - 1));
        }

        text = string.Format(FLocalizeString.Instance.GetText(weapontabledata.SkillEffectDesc), v1, v2);
        return text;
    }

    public static int GetWeaponGradeSlotCount(int grade, int wake )
    {
        return GameInfo.Instance.GameConfig.WeaponGradeSlot[grade] + (GameInfo.Instance.GameConfig.WeaponGradeSlotWakeInc[grade] * wake);
    }

    public static bool IsWeaponGemSlotAllEmpty(WeaponData weapondata)
    {
        int slotcount = GameSupport.GetWeaponGradeSlotCount(weapondata.TableData.Grade, weapondata.Wake);

        for (int i = 0; i < slotcount; i++)
        {
            if (weapondata.SlotGemUID[i] != (int)eCOUNT.NONE)
                return false;
        }

        return true;
    }

    public static bool IsWeaponOpenTerms_Effect(WeaponData weapondata)
    {
        if (GameInfo.Instance.BattleConfig.TestWeaponMaxWakeView == true)
            return true;
        if (IsMaxLevelWeapon(weapondata) && IsMaxWakeWeapon(weapondata))
            return true;
        return false;
    }

    public static int GetWeaponATK(int level, int wake, int skillLv, int enchnatLv, GameTable.Weapon.Param weapontabledata)
    {
        int value = (int)((weapontabledata.ATK + (int)(weapontabledata.IncATK * (level - 1))) * GameInfo.Instance.GameConfig.WeaponWakeStatRate[wake] * GameInfo.Instance.GameConfig.WeaponSkillLvStatRate[skillLv]);

        if (enchnatLv > 0)
        {
            var _EnchantTable = GameInfo.Instance.GameTable.Enchants.Find(x => x.GroupID == weapontabledata.EnchantGroup && x.Level == enchnatLv);
            if (_EnchantTable != null)
            {
                float fValue = value;
                fValue = fValue + (fValue * (_EnchantTable.IncreaseValue * 0.01f));
                value = (int)fValue;
            }
        }

        return value;
    }
    public static int GetWeaponCRI(int level, int wake, int skillLv, int enchnatLv, GameTable.Weapon.Param weapontabledata)
    {
        int value = (int)((weapontabledata.CRI + (int)(weapontabledata.IncCRI * (level - 1))) * GameInfo.Instance.GameConfig.WeaponWakeStatRate[wake] * GameInfo.Instance.GameConfig.WeaponSkillLvStatRate[skillLv]);
        if (enchnatLv > 0)
        {
            var _EnchantTable = GameInfo.Instance.GameTable.Enchants.Find(x => x.GroupID == weapontabledata.EnchantGroup && x.Level == enchnatLv);
            if (_EnchantTable != null)
            {
                float fValue = value;
                fValue = fValue + (fValue * (_EnchantTable.IncreaseValue * 0.01f));
                value = (int)fValue;
            }
        }
        return value;
    }
    public static int GetWeaponStatBySubSlot(int value, int skillLv)
    {
        return (int)((float)value * GameInfo.Instance.GameConfig.WeaponSubSlotStatBySLV[skillLv]);
    }
    //곡옥 관련--------------------------------------------------------------------------------------------------------------------------------
    //경험치--------------------------------------------------------------------------------------------------------------------------------
    public static float GetGemLevelExpGauge(GemData gemdata, int level, int exp)
    {
        float fillAmount = 0.0f;

        if (level >= GetGemMaxLevel())
        {
            fillAmount = 1.0f;
        }
        else
        {
            int expmin = 0;
            int expmax = 0;
            GameSupport.GetGemLevelExpMinMax(gemdata.TableData.LevelUpGroup, gemdata.TableData.Grade, level, ref expmin, ref expmax);
            expmax -= expmin;
            int expnow = exp - expmin;
            fillAmount = (float)expnow / (float)expmax;
        }
        return fillAmount;
    }

    public static void GetGemLevelExpMinMax(int group, int grade, int level, ref int min, ref int max)    //계정경험치
    {
        var cardlevelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == group && x.Level == level);
        if (cardlevelupdata == null)
            return;

        if (level <= 1)
        {
            min = 0;
        }
        else
        {
            var beforecardlevelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == group && x.Level == level - 1);
            if (beforecardlevelupdata == null)
                return;
            min = beforecardlevelupdata.Exp;
        }
        max = cardlevelupdata.Exp;
    }
    public static int GetGemMatExp(List<GemData> matlist)
    {
        int exptotal = 0;
        for (int i = 0; i < matlist.Count; i++)
        {
            exptotal += matlist[i].TableData.Exp;
            if (matlist[i].Exp != 0)
                exptotal += (int)((float)matlist[i].Exp * GameInfo.Instance.GameConfig.GemExpMatWeigth);
        }
        return exptotal;
    }

    public static int GetGemLevelUpCost(GemData gemdata, List<GemData> matlist)
    {
        int matcnt = matlist.Count;
        int goldtotal = (int)(GameInfo.Instance.GameConfig.GemLevelupCostByGrade[gemdata.TableData.Grade] * matcnt);
        return goldtotal;
    }

    public static int GetRemainGemExpToMaxLevel(GemData gemdata)
    {
        var levelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == gemdata.TableData.LevelUpGroup && x.Level == GameSupport.GetGemMaxLevel() - 1);
        if (levelupdata == null)
            return 0;
        return levelupdata.Exp - gemdata.Exp;
    }

    public static int GetGemExpLevel(GemData gemdata, int exp)
    {
        var leveluplist = GameInfo.Instance.GameTable.FindAllLevelUp(x => x.Group == gemdata.TableData.LevelUpGroup);
        if (leveluplist == null)
            return 0;

        int nMin = 0;
        int nMax = 0;
        int nResult = GameSupport.GetGemMaxLevel();

        for (int i = 0; i < leveluplist.Count; i++)
        {
            nMax = leveluplist[i].Exp;
            if (exp < nMax && exp >= nMin)
            {
                nResult = leveluplist[i].Level;
                break;
            }
            nMin = leveluplist[i].Exp;
        }
        return nResult;
    }

    public static bool IsMaxLevelGem(GemData gemdata)
    {
        if (gemdata.Level >= GameInfo.Instance.GameConfig.GemMaxLevel)
            return true;
        return false;
    }

    public static bool IsMaxLevelGem(GemData gemdata, int level)
    {
        if (level >= GameInfo.Instance.GameConfig.GemMaxLevel)
            return true;
        return false;
    }

    public static bool IsMaxWakeGem(GemData gemdata)
    {
        if (gemdata.Wake >= GameInfo.Instance.GameConfig.GemMaxWake)
            return true;
        return false;
    }

    public static int GetGemMaxLevel()
    {
        return GameInfo.Instance.GameConfig.GemMaxLevel;
    }

    public static bool IsGemWakeUp(GemData gemdata, bool bmat = true)
    {
        if (GameSupport.IsMaxLevelGem(gemdata))
        {
            if (!GameSupport.IsMaxWakeGem(gemdata))
            {
                if (bmat)
                {
                    var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == gemdata.TableData.WakeReqGroup && x.Level == gemdata.Wake);
                    if (reqdata != null)
                    {
                        if (IsItemReqList(reqdata))
                        {
                            return true; //각성 가능
                        }
                    }
                }
                else
                {
                    return true;
                }
            }

        }
    
        return false;
    }

    public static bool IsGemOptChange(GemData gemdata)
    {
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == gemdata.TableData.WakeReqGroup && x.Level == gemdata.Wake);
        if (reqdata != null)
        {
            if (IsItemReqList(reqdata))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsGemLevelUp()
    {
        int Now = 0;

        for (int i = 0; i < GameInfo.Instance.CharList.Count; i++)
        {
            var weapondata = GameInfo.Instance.GetWeaponData(GameInfo.Instance.CharList[i].EquipWeaponUID);
            if (weapondata != null)
            {
                for (int j = 0; j < (int)eCOUNT.WEAPONGEMSLOT; j++)
                {
                    if (weapondata.SlotGemUID[j] != (int)eCOUNT.NONE)
                        Now += 1;
                }
            }
        }
        if (Now < GameInfo.Instance.GemList.Count)
        {
            return true;
        }
        return false;
    }

    public static int GetTypeStatusGem(int i, int level, int wake, GameTable.Gem.Param gemtabledata)
    {
        if (i == (int)eGEMCOLOR.GREEN_HP)
            return (int)((gemtabledata.HP + (int)(gemtabledata.IncHP * (level - 1))) * GameInfo.Instance.GameConfig.GemWakeStatRate[wake]);
        else if (i == (int)eGEMCOLOR.RED_ATK)
            return (int)((gemtabledata.ATK + (int)(gemtabledata.IncATK * (level - 1))) * GameInfo.Instance.GameConfig.GemWakeStatRate[wake]);
        else if (i == (int)eGEMCOLOR.BLUE_DEF)
            return (int)((gemtabledata.DEF + (int)(gemtabledata.IncDEF * (level - 1))) * GameInfo.Instance.GameConfig.GemWakeStatRate[wake]);
        else if (i == (int)eGEMCOLOR.YELLOW_CRI)
            return (int)((gemtabledata.CRI + (int)(gemtabledata.IncCRI * (level - 1))) * GameInfo.Instance.GameConfig.GemWakeStatRate[wake]);
        return 0;
    }

    //카드 스킬 관련--------------------------------------------------------------------------------------------------------------------------------
    public static int GetCardMatExp(List<CardData> matlist)
    {
        int exptotal = 0;
        for (int i = 0; i < matlist.Count; i++)
        {
            exptotal += matlist[i].TableData.Exp;
            if (matlist[i].Exp != 0)
                exptotal += (int)((float)matlist[i].Exp * GameInfo.Instance.GameConfig.CardExpMatWeigth);
        }
        return exptotal;
    }

    public static int GetCardLevelUpCost(CardData carddata, List<CardData> matlist)
    {
        int matcnt = matlist.Count;
        int goldtotal = (int)(GameInfo.Instance.GameConfig.CardLevelupCostByLevel * carddata.Level * matcnt);
        return goldtotal;
    }


    public static int GetCardLevelUpItemCost(CardData carddata, List<MatItemData> matlist)
    {
        int count = 0;
        for (int i = 0; i < matlist.Count; i++)
            count += matlist[i].Count;
        int matcnt = count;
        int goldtotal = (int)(GameInfo.Instance.GameConfig.CardLevelupCostByLevel * carddata.Level * matcnt);
        return goldtotal;
    }

    public static int GetCardSkillLevelUpCost(CardData carddata, int cnt)
    {
        int goldtotal = (int)(GameInfo.Instance.GameConfig.CardSkillLevelupCostByGrade[carddata.TableData.Grade] * cnt);
        return goldtotal;
    }

    public static int GetRemainCardExpToMaxLevel(CardData carddata)
    {
        var levelupdata = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == carddata.TableData.LevelUpGroup && x.Level == GameSupport.GetMaxLevelCard(carddata) - 1);
        if (levelupdata == null)
            return 0;
        return levelupdata.Exp - carddata.Exp;
    }

    public static int GetMaxLevelCard(CardData carddata)
    {
        return GetMaxLevelCard(carddata.TableData.Grade, carddata.Wake);
    }

    public static int GetMaxLevelCard(CardData carddata, int wake)
    {
        return GetMaxLevelCard(carddata.TableData.Grade, wake);
    }

    public static int GetMaxLevelCard(int grade, int wake)
    {
        return GameInfo.Instance.GameConfig.CardMaxLevel[grade] + (GameInfo.Instance.GameConfig.CardWakeIncLevel[grade] * wake);
    }

    public static int GetMaxSkillLevelCard()
    {
        return GameInfo.Instance.GameConfig.CardMaxSkillLevel;
    }

    public static int GetCardMaxWake(CardData carddata)
    {
        return GameInfo.Instance.GameConfig.CardWakeMax[carddata.TableData.Grade];
    }

    public static bool IsMaxWakeCard(CardData carddata)
    {
        if (carddata.Wake >= GameInfo.Instance.GameConfig.CardWakeMax[carddata.TableData.Grade])
            return true;
        return false;
    }

    public static bool IsMaxWakeCard(int grade, int wake)
    {
        if (wake >= GameInfo.Instance.GameConfig.CardWakeMax[grade])
            return true;
        return false;
    }

    public static bool IsMaxLevelCard(CardData carddata)
    {
        if (carddata.Level >= GetMaxLevelCard(carddata))
            return true;
        return false;
    }

    public static bool IsMaxLevelCard(int level, int grade, int wake)
    {
        if (level >= GetMaxLevelCard(grade, wake))
            return true;
        return false;
    }


    public static bool IsMaxLevelCard(CardData carddata, int level )
    {
        if (level >= GetMaxLevelCard(carddata))
            return true;
        return false;
    }
  
    public static bool IsMaxSkillLevelCard(CardData carddata)
    {
        if (carddata.SkillLv >= GetMaxSkillLevelCard())
            return true;
        return false;
    }


    public static bool IsMaxSkillLevelCard(CardData carddata, int skilllv)
    {
        if (skilllv >= GetMaxSkillLevelCard())
            return true;
        return false;
    }

    public static int GetCardExpLevel(CardData carddata, int exp)
    {
        var leveluplist = GameInfo.Instance.GameTable.FindAllLevelUp(x => x.Group == carddata.TableData.LevelUpGroup);
        if (leveluplist == null)
            return 0;

        int nMin = 0;
        int nMax = 0;
        int nResult = GameSupport.GetMaxLevelCard(carddata);

        for (int i = 0; i < leveluplist.Count; i++)
        {
            nMax = leveluplist[i].Exp;
            if (exp < nMax && exp >= nMin)
            {
                nResult = leveluplist[i].Level;
                break;
            }
            nMin = leveluplist[i].Exp;
        }
        return nResult;
    }

    public static string GetCardSubSkillDesc(GameTable.Card.Param cardtabledata, int skilllv)
    {
        string text = string.Empty;
        float v1 = 0.0f; // {0}
        float v2 = 0.0f; // {1} 
        float v3 = 0.0f; // {2}

        GameClientTable.BattleOptionSet.Param param = null;
        if (cardtabledata.SptAddBOSetID1 > 0)
        {
            param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(cardtabledata.SptAddBOSetID1);
            if (param != null)
                v1 = (float)param.BOFuncValue + (float)(param.BOFuncIncValue * (float)(skilllv - 1));
        }
        if (cardtabledata.SptAddBOSetID2 > 0)
        {
            param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(cardtabledata.SptAddBOSetID2);
            if (param != null)
                v2 = (float)param.BOFuncValue + (float)(param.BOFuncIncValue * (float)(skilllv - 1));
        }
        v3 = (float)cardtabledata.SptSvrOptValue + (float)(cardtabledata.SptSvrOptIncValue * (float)(skilllv - 1));

        text = string.Format(FLocalizeString.Instance.GetText(cardtabledata.SkillEffectDesc), v1, v2, v3);
        return text;
    }

    public static string GetCardMainSkillDesc(GameTable.Card.Param cardtabledata, int wake)
    {
        string text = string.Empty;
        float v1 = 0.0f; // {0}
        float v2 = 0.0f; // {1} 
        float v3 = 0.0f; // {2}

        GameClientTable.BattleOptionSet.Param param = null;
        if (cardtabledata.SptMainBOSetID1 > 0)
        {
            param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(cardtabledata.SptMainBOSetID1);
            if (param != null)
                v1 = (float)param.BOFuncValue + (float)(param.BOFuncIncValue * (float)wake);
        }
        if (cardtabledata.SptMainBOSetID2 > 0)
        {
            param = GameInfo.Instance.GameClientTable.FindBattleOptionSet(cardtabledata.SptMainBOSetID2);
            if (param != null)
                v2 = (float)param.BOFuncValue + (float)(param.BOFuncIncValue * (float)wake);
        }
        v3 = (float)cardtabledata.SptSvrMainOptValue + (float)(cardtabledata.SptSvrMainOptIncValue * (float)wake);

        text = string.Format(FLocalizeString.Instance.GetText(cardtabledata.MainSkillEffectDesc), v1, v2, v3);
        return text;
    }

    public static int GetCardHP(int level, int wake, int skillLv, int enchnatLv, GameTable.Card.Param cardtabledata)
    {
        int value = (int)((cardtabledata.HP + (int)(cardtabledata.IncHP * (level - 1))) * GameInfo.Instance.GameConfig.CardWakeStatRate[wake] * GameInfo.Instance.GameConfig.CardSkillLvStatRate[skillLv]);
        if (enchnatLv > 0)
        {
            var _EnchantTable = GameInfo.Instance.GameTable.Enchants.Find(x => x.GroupID == cardtabledata.EnchantGroup && x.Level == enchnatLv);
            if (_EnchantTable != null)
            {
                float fValue = value;
                fValue = fValue + (fValue * (_EnchantTable.IncreaseValue * 0.01f));
                value = (int)fValue;
            }
        }

        return value;
    }

    public static int GetCardDEF(int level, int wake, int skillLv, int enchnatLv, GameTable.Card.Param cardtabledata)
    {
        int value = (int)((cardtabledata.DEF + (int)(cardtabledata.IncDEF * (level - 1))) * GameInfo.Instance.GameConfig.CardWakeStatRate[wake] * GameInfo.Instance.GameConfig.CardSkillLvStatRate[skillLv]);
        if (enchnatLv > 0)
        {
            var _EnchantTable = GameInfo.Instance.GameTable.Enchants.Find(x => x.GroupID == cardtabledata.EnchantGroup && x.Level == enchnatLv);
            if (_EnchantTable != null)
            {
                float fValue = value;
                fValue = fValue + (fValue * (_EnchantTable.IncreaseValue * 0.01f));
                value = (int)fValue;
            }
        }
        return value;
    }

    public static int GetCardStatBySubSlot(int value, int skillLv)
    {
        return (int)((float)value * GameInfo.Instance.GameConfig.CardSubSlotStatBySLV[skillLv]);
    }

    public static bool IsCardOpenTerms_Effect(CardData carddata)
    {
        if (GameInfo.Instance.BattleConfig.TestWeaponMaxWakeView == true)
            return true;
        if (IsMaxLevelCard(carddata) && IsMaxWakeCard(carddata))
            return true;
        return false;
    }
    public static bool IsCardOpenTerms_Favor(CardBookData cardbookdata)
    {
        if (cardbookdata.FavorLevel >= GameInfo.Instance.GameConfig.CardFavorMaxLevel)
            return true;
        return false;
    }
    
    public static bool IsMaxSkillLevelCardAll(int tableid)
    {
        var list = GameInfo.Instance.CardList.FindAll(x => x.TableID == tableid);
        for (int i = 0; i < list.Count; i++)
            if (IsMaxSkillLevelCard(list[i]))
                return true;
        return false;
    }

    public static int GetStageMissionCount(GameTable.Stage.Param tabledata)
    {
        int count = 0;
        if (tabledata.Mission_00 > 0)
            count += 1;
        if (tabledata.Mission_01 > 0)
            count += 1;
        if (tabledata.Mission_02 > 0)
            count += 1;
        return count;
    }

    public static void GetIsChapterSectionDifficultylist(int chapter, int section, ref List<bool> list, eSTAGETYPE stagetype = eSTAGETYPE.STAGE_MAIN_STORY, int eventid = -1)
    {
        for (int i = 0; i < 3; i++)
            list.Add(IsMainStoryChapterSection(chapter, section, i + 1, stagetype, eventid));
    }

    public static bool IsMainStoryChapterSection(int chapter, int section, int diff, eSTAGETYPE stagetype = eSTAGETYPE.STAGE_MAIN_STORY, int eventid = -1)
    {
        var stagedata = (stagetype == eSTAGETYPE.STAGE_EVENT) ?
            GameInfo.Instance.GameTable.FindStage(x => x.StageType == (int)stagetype && x.TypeValue == eventid && x.Difficulty == diff && x.Chapter == chapter && x.Section == section) :
            GameInfo.Instance.GameTable.FindStage(x => x.StageType == (int)stagetype && x.Difficulty == diff && x.Chapter == chapter && x.Section == section);

        if (stagedata == null)
            return false;

        if (stagedata.LimitStage == -1)
            return true;

        var _limitstagedata = GameInfo.Instance.StageClearList.Find(x => x.TableID == stagedata.LimitStage);
        if (_limitstagedata == null)
            return false;

        return true;
    }

    public static void GetIsChapterDifficultylist(int chapter, ref List<bool> list, eSTAGETYPE stagetype = eSTAGETYPE.STAGE_MAIN_STORY, int eventid = -1)
    {
        for (int i = 0; i < 3; i++)
            list.Add(IsMainStoryChapter(chapter, i + 1, stagetype, eventid));
    }

    public static bool IsMainStoryChapter(int chapter, int diff, eSTAGETYPE stagetype = eSTAGETYPE.STAGE_MAIN_STORY, int eventid = -1)
    {
        var list = (stagetype == eSTAGETYPE.STAGE_EVENT) ?
            GameInfo.Instance.GameTable.FindAllStage(x => x.StageType == (int)stagetype && x.TypeValue == eventid && x.Difficulty == diff && x.Chapter == chapter) :
            GameInfo.Instance.GameTable.FindAllStage(x => x.StageType == (int)stagetype && x.Difficulty == diff && x.Chapter == chapter);

        if (list.Count == 0)
            return false;

        if (list[0].LimitStage == -1)
            return true;

        var limitstagedata = GameInfo.Instance.StageClearList.Find(x => x.TableID == list[0].LimitStage);
        if (limitstagedata == null)
            return false;
        
        return true;
    }

    public static int GetEventSuitableStageID( int eventid )
    {
        int stageid = -1;
        var eventsetdata = GameInfo.Instance.GetEventSetData(eventid);
        if (eventsetdata == null)
            return -1;

        int laststageid = -1;
        var lastPlayStage = UIValue.Instance.GetValue(UIValue.EParamType.LastPlayStageID);
        if (lastPlayStage != null)
            laststageid = (int)lastPlayStage;

        var clearlist = GameInfo.Instance.StageClearList.FindAll(x => x.TableData.StageType == (int)eSTAGETYPE.STAGE_EVENT && x.TableData.TypeValue == eventsetdata.TableID);
        StageClearData.SortUp = true;
        clearlist.Sort(StageClearData.CompareFuncStageID);

        bool blaststorystage = false;
        var laststagedata = GameInfo.Instance.GameTable.FindStage(laststageid);
        if (laststagedata != null)
        {
            if (laststagedata.StageType == (int)eSTAGETYPE.STAGE_MAIN_STORY)
                blaststorystage = true;
        }
        else
        {
            //191213
            //기기이전으로 접속할시 기존 데이터가 없기 때문에 에러발생하여 예외처리함.
            if (clearlist.Count != 0)
            {
                if (clearlist[0].TableData.NextStage == -1)
                    stageid = clearlist[0].TableData.ID;
                else
                    stageid = clearlist[0].TableData.NextStage;
            }

            return stageid;
        }


        if (blaststorystage) //마지막 플레이가 스토리
        {
            if (clearlist.Count != 0)
            {
                if (clearlist[0].TableData.NextStage == -1)
                    stageid = clearlist[0].TableData.ID;
                else
                    stageid = clearlist[0].TableData.NextStage;
            }
        }
        else
        {
            StageClearData lastcleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == laststagedata.ID);
            StageClearData nextcleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == laststagedata.NextStage);

            if (laststagedata.NextStage == -1 || lastcleardata == null)
            {
                stageid = laststageid;
            }
            else
            {
                if (nextcleardata == null) //다음 스테이지 클리어전
                    stageid = laststagedata.NextStage;
                else
                    stageid = laststageid;
            }
        }

        return stageid;
    }

    public static int GetStorySuitableStageID(eSTAGETYPE stageType)
    {
        int stageid = 1;

        int laststageid = -1;
        var lastPlayStage = UIValue.Instance.GetValue(UIValue.EParamType.LastPlayStageID);
        if (lastPlayStage != null)
            laststageid = (int)lastPlayStage;

        List<StageClearData> clearlist = GameInfo.Instance.StageClearList.FindAll(x => x.TableData.StageType == (int)stageType && x.TableData.Chapter <= GameInfo.Instance.GameClientTable.Chapters.Count);
        StageClearData.SortUp = true;
        clearlist.Sort(StageClearData.CompareFuncStageID);

        bool blaststorystage = false;
        GameTable.Stage.Param laststagedata = GameInfo.Instance.GameTable.FindStage(laststageid);
        if(laststagedata != null )
        {
            if( laststagedata.StageType == (int)eSTAGETYPE.STAGE_MAIN_STORY )
                blaststorystage = true;
        }

        if(blaststorystage) //마지막 플레이가 스토리
        {
            StageClearData lastcleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == laststagedata.ID);
            StageClearData nextcleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == laststagedata.NextStage);

            if (laststagedata.NextStage == -1 || lastcleardata == null)
            {
                stageid = laststageid;
            }
            else
            {
                if(nextcleardata == null) //다음 스테이지 클리어전
                {
                    GameTable.Stage.Param nextTableStage = GameInfo.Instance.GameTable.FindStage(x => x.ID == lastcleardata.TableData.NextStage);
                    
                    if(nextTableStage != null)
                    {
                        GameClientTable.Chapter.Param chapterCheck = GameInfo.Instance.GameClientTable.FindChapter(x => x.ID == nextTableStage.Chapter);
                        Debug.LogError("LastClearData TableData Chapter : " + nextTableStage.Chapter);
                        if (chapterCheck == null)
                        {
                            stageid = laststageid;
                        }
                        else
                        {
                            stageid = laststagedata.NextStage;
                        }
                    }
                    else
                    {
                        stageid = laststageid;
                    }

                    
                }
                else
                {
                    
                    stageid = laststageid;
                }
            }
        }
        else
        {
            if (clearlist.Count != 0)
            {
                if( clearlist[0].TableData.NextStage == -1 )
                    stageid = clearlist[0].TableData.ID;
                else
                {
                    GameClientTable.Chapter.Param chapterCheck = GameInfo.Instance.GameClientTable.FindChapter(x => x.ID == clearlist[0].TableData.Chapter);
                    if(chapterCheck == null)
                    {
                        stageid = clearlist[0].TableData.ID;
                    }
                    else
                    {
                        GameTable.Stage.Param nextTableStage = GameInfo.Instance.GameTable.FindStage(x => x.ID == clearlist[0].TableData.NextStage);
                        if(nextTableStage == null)
                        {
                            stageid = clearlist[0].TableData.ID;
                        }
                        else
                        {
                            GameClientTable.Chapter.Param nextChapterCheck = GameInfo.Instance.GameClientTable.FindChapter(x => x.ID == nextTableStage.Chapter);
                            if(nextChapterCheck == null)
                            {
                                stageid = clearlist[0].TableData.ID;
                            }
                            else
                            {
                                stageid = clearlist[0].TableData.NextStage;
                            }
                        }
                        //stageid = clearlist[0].TableData.NextStage;
                    }
                }
                    
            }
        }

        return stageid;
    }

    public static bool IsAllEventMissionClear(int typeValue)
    {
        List<GameTable.Stage.Param> findAll = GameInfo.Instance.GameTable.FindAllStage(x => x.StageType == (int)eSTAGETYPE.STAGE_EVENT && 
                                                                                            x.TypeValue == typeValue);
        if(findAll == null || findAll.Count <= 0)
        {
            return false;
        }

        for(int i = 0; i < findAll.Count; i++)
        {
            List<StageClearData> findAllClearData = GameInfo.Instance.StageClearList.FindAll(x => x.TableID == findAll[i].ID);
            if(findAllClearData == null || findAllClearData.Count <= 0)
            {
                return false;
            }

            for(int j = 0; j < findAllClearData.Count; j++)
            {
                if(!findAllClearData[j].IsClearAll())
                {
                    return false;
                }
            }
        }
        
        return true;
    }

	public bool IsAllDailyStageClear(int difficult)
	{
		List<GameTable.Stage.Param> find1 = GameInfo.Instance.GameTable.FindAllStage(x => x.StageType == (int)eSTAGETYPE.STAGE_DAILY &&
																						  x.Difficulty == difficult);
		if (find1 == null || find1.Count <= 0)
		{
			return false;
		}

		List<StageClearData> find2 = GameInfo.Instance.StageClearList.FindAll(x => x.TableData.StageType == (int)eSTAGETYPE.STAGE_DAILY &&
																				   x.TableData.Difficulty == difficult);
		if (find2 == null || find2.Count <= 0 || find1.Count != find2.Count)
		{
			return false;
		}

		return true;
	}

	// 요일 값 활성화 여부 체크
	public static bool IsOnDayOfWeek(int dayFlag, int _dayIdx)
    {
        if (32 <= _dayIdx)
            return false;
        int checkFlag = 0x00000001 << _dayIdx;
        return (checkFlag == (dayFlag & (System.UInt32)checkFlag));
    }

    public static void TweenerReset(GameObject o)
    {
        o.SetActive(true);
        var list = o.GetComponentsInChildren<UITweener>();
        for (int i = 0; i < list.Length; i++)
        {
            list[i].tweenFactor = 0.0f;
        }
    }

    public static float TweenerPlay(GameObject o)
    {
        float duration = 0.0f;

        o.SetActive(true);
        var list = o.GetComponentsInChildren<UITweener>();
        for (int i = 0; i < list.Length; i++)
        {
            list[i].ResetToBeginning();
            list[i].PlayForward();

            if (list[i].duration > duration)
                duration = list[i].duration;
        }

        return duration;
    }

    public static float GetMaxTweenerDuration(GameObject o)
    {
        float duration = 0.0f;
        UITweener[] list = o.GetComponentsInChildren<UITweener>();
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i].duration > duration)
                duration = list[i].duration;
        }

        return duration;
    }

    public static string GetLevelText(int text_id, int now_level, int max_level = -1, bool next_now_level = false, bool next_max_level = false)
    {
        string text = string.Empty;
        string now_level_text = string.Empty;
        string max_level_text = string.Empty;

        if (next_now_level)
        {
            now_level_text = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText(text_id), now_level));
        }
        else
        {
            now_level_text = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_NOW_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText(text_id), now_level));
        }

        if (max_level != -1)
        {
            if (next_max_level)
            {
                max_level_text = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.SLASH_TXT_MAX_CNT), max_level));
            }
            else
            {
                max_level_text = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_MAX_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.SLASH_TXT_MAX_CNT), max_level));
            }
        }

        text = now_level_text + max_level_text;

        return text;
    }

    public static bool IsOnCostumePassvie(GameTable.Costume.Param param)
    {
        if (param.HP + param.ATK + param.DEF + param.CRI > 0)
            return true;
        return false;
    }
    public static int GetCostumePassvieStatCount(GameTable.Costume.Param param)
    {
        int totalstat = 0;

        if (param.HP > 0)
            totalstat += 1;
        if ( param.ATK > 0 )
            totalstat += 1;
        if ( param.DEF > 0 )
            totalstat += 1;
        if (  param.CRI > 0)
            totalstat += 1;
        return totalstat;
    }

    public static void GetCostumePassvieStatList(GameTable.Costume.Param param, ref List<int> list )
    {
        if (param.HP > 0)
            list.Add((int)eCHARABILITY.HP);
        if (param.ATK > 0)
            list.Add((int)eCHARABILITY.ATK);
        if (param.DEF > 0)
            list.Add((int)eCHARABILITY.DEF);
        if (param.CRI > 0)
            list.Add((int)eCHARABILITY.CRI);
    }

    public static int GetCostumePassvieStat(GameTable.Costume.Param param, eCHARABILITY eStat)
    {
        int stat = 0;
        switch (eStat)
        {
            case eCHARABILITY.HP: stat = param.HP; break;
            case eCHARABILITY.ATK: stat = param.ATK; break;
            case eCHARABILITY.DEF: stat = param.DEF; break;
            case eCHARABILITY.CRI: stat = param.CRI; break;
        }
        return stat;
    }

    public static int GetCostumeStat(CharData chardata, eCHARABILITY eStat)
    {
        int nStat = 0;
        for (int i = 0; i < GameInfo.Instance.CostumeList.Count; i++)
        {
            var tabledata = GameInfo.Instance.GameTable.FindCostume(x => x.ID == GameInfo.Instance.CostumeList[i]);
            if (tabledata != null)
            {
                if (tabledata.CharacterID == chardata.TableID)
                {
                    switch (eStat)
                    {
                        case eCHARABILITY.HP:       nStat += tabledata.HP;      break;
                        case eCHARABILITY.ATK:      nStat += tabledata.ATK;     break;
                        case eCHARABILITY.DEF:      nStat += tabledata.DEF;     break;
                        case eCHARABILITY.CRI:      nStat += tabledata.CRI;     break;
                    }
                }
            }
        }
        return nStat;
    }
    //----------------------------------------------------------------------------------------------------------------------------------------
    //통신 관련--------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------
    //77 시간관련 함수 GameSupport 이동

    /// <summary>
    /// 규칙 1. EndTime 은 반드시 isAddOneHourTime 을 True 로 할 것!!
    /// </summary>
    /// <param name="_origin"></param>
    /// <param name="isAddOneHourTime"></param>
    /// <returns></returns>
    public static DateTime GetTimeWithString(string _origin, bool isAddOneHourTime = false)
    {
        int _year = int.Parse(_origin.Substring(0, 4));
        int _month = int.Parse(_origin.Substring(_year.ToString().Length, 2));
        int _day = int.Parse(_origin.Substring(_year.ToString().Length + _month.ToString("D2").Length, 2));
        int _hour = int.Parse(_origin.Substring(_year.ToString().Length + _month.ToString("D2").Length + _day.ToString("D2").Length));

        int _addTime = 0;
        if (isAddOneHourTime)
            _addTime = 59;

        DateTime resultDateTime = new DateTime(_year, _month, _day, _hour, _addTime, _addTime);

        return GameSupport.GetLocalTimeByServerTime(resultDateTime);

        //return resultDateTime;
    }

    //운영툴에서 12시로 입력해도, 서버에서 1시간 더해서 오기때문에 12:59분으로 셋팅해줌
    public static DateTime GetMinusOneMinuteEndTime(DateTime _origin)
    {
        return _origin.Add(new TimeSpan(0, -1, 0));
    }


    public static string GetDateTimeFullString(DateTime _Time)
    {
        string time = string.Empty;
        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
        {
            time = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.TIME_D), _Time.Year, _Time.Month, _Time.Day, GetDay(_Time), _Time.Hour, _Time.Minute);
        }
        else
        {
            time = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.TIME_M), _Time.Day, GetMonth(_Time.Month), _Time.Year, _Time.Hour, _Time.Minute);
        }
        return time;
    }
    public static string GetStartTime(DateTime _startTime)
    {
        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
        {
            string time = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.TIME_D), _startTime.Year, _startTime.Month, _startTime.Day, GetDay(_startTime), _startTime.Hour, _startTime.Minute);
            return string.Format(FLocalizeString.Instance.GetText(302), time);
        }
        else
        {
            string time = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.TIME_M), _startTime.Day, GetMonth(_startTime.Month), _startTime.Year, _startTime.Hour, _startTime.Minute);
            return string.Format(FLocalizeString.Instance.GetText(302), time);
        }
    }

    public static string GetEndTime(DateTime _endTime, bool bAddUntilText = true)
    {
        Debug.Log(_endTime.Year + " / " + _endTime.Month + " / " + _endTime.Day + " / " + _endTime.Hour + " / " + _endTime.Minute);
        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
        {
            string time = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.TIME_D), _endTime.Year, _endTime.Month, _endTime.Day, GetDay(_endTime), _endTime.Hour, _endTime.Minute);
            if (bAddUntilText)
                return string.Format(FLocalizeString.Instance.GetText(303), time);
            else
                return time;
        }
        else
        {
            string time = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.TIME_M), _endTime.Day, GetMonth(_endTime.Month), _endTime.Year, _endTime.Hour, _endTime.Minute);
            if (bAddUntilText)
                return string.Format(FLocalizeString.Instance.GetText(303), time);//, (int)(GameInfo.Instance.ServerData.ServerTimeGap / 3600));
            else
                return time;
        }
    }

    public static string GetPackageEndTime(DateTime _endTime, bool bAddUntilText = true)
    {
        string time = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.TIME_M), _endTime.Day, GetMonth(_endTime.Month), _endTime.Year, _endTime.Hour, _endTime.Minute);
        if (bAddUntilText)
            return string.Format(FLocalizeString.Instance.GetText(1720), time);//, (int)(GameInfo.Instance.ServerData.ServerTimeGap / 3600));
        else
            return time;
    }

    public static string GetPeriodTime(DateTime _startTime, DateTime _endTime)
    {
        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
        {
            string times = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.TIME_D), _startTime.Year, _startTime.Month, _startTime.Day, GetDay(_startTime), _startTime.Hour, _startTime.Minute);
            string timee = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.TIME_D), _endTime.Year, _endTime.Month, _endTime.Day, GetDay(_endTime), _endTime.Hour, _endTime.Minute);
            return string.Format(FLocalizeString.Instance.GetText(304), times, timee);
        }
        else
        {
            string times = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.TIME_M), _startTime.Day, GetMonth(_startTime.Month), _startTime.Year, _startTime.Hour, _startTime.Minute);
            string timee = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.TIME_M), _endTime.Day, GetMonth(_endTime.Month), _endTime.Year, _endTime.Hour, _endTime.Minute);
            return string.Format(FLocalizeString.Instance.GetText(304), times, timee);
        }
        
    }

    public static string GetDay(DateTime _dt)
    {
        string resultStr = string.Empty;
        
        resultStr = FLocalizeString.Instance.GetText(190 + (int)_dt.DayOfWeek);

        return resultStr;
    }

    public static string GetMonth(int _dt)
    {
        string resultStr = string.Empty;

        resultStr = FLocalizeString.Instance.GetText(1560 + _dt);

        return resultStr;
    }

    public static string GetTimeString(int time)
    {
        string str = string.Empty;

        if (time == 0)
        {
            str = FLocalizeString.Instance.GetText(263, 0);
            return str;
        }

        int h = time / 3600;
        int m = (time % 3600) / 60;
        int s = time % 60;
        if (h != 0)
            str = FLocalizeString.Instance.GetText(261, h, m);
        else if (m != 0)
            str = FLocalizeString.Instance.GetText(262, m);
        else
            str = FLocalizeString.Instance.GetText(263, s);
        return str;
    }

    public static string GetTimeString(TimeSpan t)
    {
        string strDay = FLocalizeString.Instance.GetText(281, t.Days);
        string strHour = FLocalizeString.Instance.GetText(280, t.Hours);
        string strMin = FLocalizeString.Instance.GetText(262, t.Minutes);

        if (t.Days > (int)eCOUNT.NONE)
        {
            return string.Format("{0}{1}{2}", strDay, strHour, strMin); 
        }
        else
        {
            return string.Format("{0}{1}", strHour, strMin);
        }
    }

    public static string GetTimeStringWithDetail(int time)
    {
        string str = string.Empty;

        if(time == 0)
        {
            str = string.Format(FLocalizeString.Instance.GetText(263), 0);
            return str;
        }

        int h = time / 3600;
        int m = (time % 3600) / 60;
        int s = time % 60;
        if (h != 0)
            str = string.Format(FLocalizeString.Instance.GetText(280), h);
        if (m != 0)
            str = str + " " + string.Format(FLocalizeString.Instance.GetText(262), m);
        if(s != 0)
            str = str + " " + string.Format(FLocalizeString.Instance.GetText(263), s);
            
        return str;
    }
    
    public static string GetTimeHighestScore(int score)
    {
        string str = string.Empty;
        int ms = score % 1000;
        int time = score / 1000;
        
        int m = time / 60;
        int s = time % 60;

        str = string.Format(FLocalizeString.Instance.GetText(239), m,s, ms);
        
        return str;
    }

    private static string _str261 = string.Empty;
    private static string _str263 = string.Empty;    
    private static string _str264 = string.Empty;
    private static string _str281 = string.Empty;
    public static void ReLoadTimeString()
    {
        _str261 = FLocalizeString.Instance.GetText(261);
        _str263 = FLocalizeString.Instance.GetText(263);
        _str264 = FLocalizeString.Instance.GetText(264);
        _str281 = FLocalizeString.Instance.GetText(281);
    }

    public static string GetRemainTimeString(DateTime RemainTime, DateTime NowTime)
    {
        string str = "-";
        var diffTime = RemainTime - NowTime;
        if (diffTime.Ticks >= 0)
        {
            if (string.IsNullOrEmpty(_str261)) _str261 = FLocalizeString.Instance.GetText(261);
            if (string.IsNullOrEmpty(_str263)) _str263 = FLocalizeString.Instance.GetText(263);
            if (string.IsNullOrEmpty(_str264)) _str264 = FLocalizeString.Instance.GetText(264);
            if (string.IsNullOrEmpty(_str281)) _str281 = FLocalizeString.Instance.GetText(281);

            if (diffTime.Days != 0)
                str = string.Format(_str281, diffTime.Days);
            else if (diffTime.Hours != 0)
                str = string.Format(_str261, diffTime.Hours, diffTime.Minutes);
            else if (diffTime.Minutes != 0)
                str = string.Format(_str264, diffTime.Minutes, diffTime.Seconds);
            else
                str = string.Format(_str263, diffTime.Seconds);
        }
        return str;
    }

    public static string GetRemainTimeString_DayAndHours(DateTime RemainTime, DateTime NowTime)
    {
        string str = "-";   //  시간 표시가 불필요
        var diffTime = RemainTime - NowTime;
        if (diffTime.Ticks >= 0)
        {
            //  24시간 미만
            if (diffTime.Hours <= 24 && diffTime.Days == 0)
            {
                //  한 시간 미만
                if (diffTime.Minutes < 60 && diffTime.Hours == 0)
                    str = FLocalizeString.Instance.GetText(1298);   //  1시간 미만
                else
                    str = string.Format(FLocalizeString.Instance.GetText(280), diffTime.Hours); //  {0}시간
            }
            else
            {
                str = string.Format(FLocalizeString.Instance.GetText(281), diffTime.Days);  //  {0}일
            }
        }

        return str;
    }
    //시설에서 사용될 00:00:00 형식의 시간데이터
    public static string GetFacilityTimeString(float time)
    {
        string str = string.Empty;

        if (time == 0)
        {
            str = FLocalizeString.Instance.GetText(232, 0, 0, 0);
            return str;
        }

        int h = (int)time / 3600;
        int m = (int)(time % 3600) / 60;
        int s = (int)time % 60;

        str = FLocalizeString.Instance.GetText(232, h, m, s);

        return str;
    }

    public static string GetFacilityRemainTimeString(DateTime RemainTime, DateTime NowTime)
    {
        string str = FLocalizeString.Instance.GetText(232, 0, 0, 0);
        var diffTime = RemainTime - NowTime;
        if (diffTime.Ticks >= 0)
        {
            str = FLocalizeString.Instance.GetText(232, diffTime.Hours + (diffTime.Days * 24), diffTime.Minutes, diffTime.Seconds);
        }
        return str;
    }
    public static string GetFacilityRemainTimeString(DateTime RemainTime, DateTime NowTime, string FormatStr)
    {
        string _FormatStr = string.Format(FormatStr, 0, 0, 0);
        var diffTime = RemainTime - NowTime;
        if (diffTime.Ticks >= 0)
        {
            _FormatStr = string.Format(FormatStr, diffTime.Hours + (diffTime.Days * 24), diffTime.Minutes, diffTime.Seconds);
        }
        return _FormatStr;
    }

    //즉시 완료 시 필요한 아이템 갯수 반환(현재 남은시간 비례)
    public static int GetFacilityNeedSpeedItem(FacilityData facilitydata)
    {
        GameTable.Item.Param speedItem = GameInfo.Instance.GameTable.FindItem(x => x.Type == (int)eITEMTYPE.MATERIAL && x.SubType == (int)eITEMSUBTYPE.MATERIAL_FACILITY_TIME);
        if (speedItem == null)
            return 0;

        var diffTime = facilitydata.RemainTime - GameSupport.GetCurrentServerTime();
        return Mathf.CeilToInt((float)diffTime.TotalMinutes / speedItem.Value);
    }
    //즉시 완료 시 필요한 아이템 Table ID 반환
    public static int GetFacilitySpeedItemTableID()
    {
        GameTable.Item.Param speedItem = GameInfo.Instance.GameTable.FindItem(x => x.Type == (int)eITEMTYPE.MATERIAL && x.SubType == (int)eITEMSUBTYPE.MATERIAL_FACILITY_TIME);
        if (speedItem == null)
            return 0;

        return speedItem.ID;
    }

    /// <summary>
    ///   남은 시간 계산      ///
    ///   미래의 시간에서 현재 시간을 뺀 시간을 반환
    /// </summary>
    public static TimeSpan GetRemainTime(DateTime AfterTime)
    {
        return AfterTime - GetCurrentServerTime();
    }

    public static TimeSpan GetRemainTime(DateTime AfterTime, DateTime BeforeTime)
    {
        return AfterTime - BeforeTime;
    }

    /// <summary>
    /// GMT(UTC)시간을 기준으로 서버와의 시간을 더한 값(-값)을 더해서 서버시간의 근사치를 만들어 반환합니다.
    /// </summary>
    public static DateTime GetCurrentServerTime()
    {
        DateTime serverTime = DateTime.Now;
        if(GameInfo.Instance.netFlag == true )
        {
            serverTime = GetLocalTimeByServerTime(DateTime.UtcNow.AddSeconds(GameInfo.Instance.ServerData.ServerTimeGap));
        }

        return serverTime;
    }

    /// <summary>
    /// 한국 서버 시간 UTC + 9 + ServerTimeGap
    /// </summary>
    /// <returns></returns>
    public static DateTime GetCurrentRealServerTime()
    {
        //DateTime serverTime = DateTime.Now;
        DateTime serverTime = GameInfo.Instance.GetNetworkTime();
        if (GameInfo.Instance.netFlag == true)
        {
            serverTime = DateTime.UtcNow.AddSeconds(GameInfo.Instance.ServerData.ServerTimeGap);
        }

        return serverTime;
    }

    public static DateTime GetCurRealServerTime() {
        DateTime serverTime = GameInfo.Instance.GetNetworkTime();
        serverTime = serverTime.Add( -TimeZoneInfo.Local.BaseUtcOffset );

        serverTime = serverTime.AddSeconds( GameInfo.Instance.ServerData.ServerTimeGap );
        return serverTime;
    }

    /// <summary>
    /// UTC+0 시간을 기반으로 현지 시간을 구하는 함수 
    /// </summary>
    /// <param name="utc0Time">UTC+0 시간</param>
    /// <returns>현지 시간</returns>
    public static DateTime GetLocalTimeByUTC0(DateTime utc0Time)
    {
        return utc0Time.Add(TimeZoneInfo.Local.BaseUtcOffset);
    }

    /// <summary>
    /// 서버 기준 시간을 현지 시간으로 변환하는 함수
    /// </summary>
    /// <param name="serverTime">서버 기준 시간</param>
    /// <returns>현지 시간</returns>
    public static DateTime GetLocalTimeByServerTime(DateTime serverTime)
    {
        if (serverTime.Ticks == 0)
            return serverTime;

        DateTime utc0Time = serverTime.AddSeconds(-GameInfo.Instance.ServerData.ServerTimeGap);
        return GetLocalTimeByUTC0(utc0Time);
    }

    public static bool IsNightTime(int minHour, int maxHour)
    {
        DateTime now = DateTime.UtcNow.Add(TimeZoneInfo.Local.BaseUtcOffset);
        if(now.Hour >= minHour && now.Hour <= maxHour)
        {
            return true;
        }

        return false;
    }

    public static void SetMatList(GameTable.ItemReqList.Param reqdata, ref List<int> idlist, ref List<int> countlist)
    {
        if (reqdata.ItemID1 != -1)
        {
            idlist.Add(reqdata.ItemID1);
            countlist.Add(reqdata.Count1);
        }
        if (reqdata.ItemID2 != -1)
        {
            idlist.Add(reqdata.ItemID2);
            countlist.Add(reqdata.Count2);
        }
        if (reqdata.ItemID3 != -1)
        {
            idlist.Add(reqdata.ItemID3);
            countlist.Add(reqdata.Count3);
        }
        if (reqdata.ItemID4 != -1)
        {
            idlist.Add(reqdata.ItemID4);
            countlist.Add(reqdata.Count4);
        }
    }

    /// <summary>
    ///  해당 아이템의 드롭률
    /// </summary>
    public static float GetDropPersent(int randomGroupID,int productType,int productIndex)
    {
        int totalProb = 0;
        float selectPersent = 0;

        //  리워드 그룹 아이템들
        var randomGroup = GameInfo.Instance.GameTable.FindAllRandom(a => a.GroupID == randomGroupID);
        //  리워드 타입과 리워드 인덱스가 같은 아이템들
        var randoms = randomGroup.FindAll(a => a.ProductType == productType && a.ProductIndex == productIndex );
        
        //  total Prob
        for (int i = 0; i < randomGroup.Count; i++)
            totalProb += randomGroup[i].Prob;

        //  해당 아이템들의 드랍 퍼센트
        for (int i = 0; i < randoms.Count; i++)
            selectPersent += (randoms[i].Prob / (float)totalProb);

        //  백분율로 반환
        return selectPersent * 100;
    }

    //------------------------------------------------------------------------------------------------------------------------------------------
    //
    //
    //
    //------------------------------------------------------------------------------------------------------------------------------------------
    public static int GetTotalHP(CharData chardata, eWeaponSlot wpnSlot)
    {
        long mainWpnUID = chardata.EquipWeaponUID;
        long subWpnUID = chardata.EquipWeapon2UID;
        if (wpnSlot == eWeaponSlot.SUB)
        {
            mainWpnUID = chardata.EquipWeapon2UID;
            subWpnUID = chardata.EquipWeaponUID;
        }
        int statWpn = 0;
        WeaponData tempWpn = GameInfo.Instance.GetWeaponData(mainWpnUID);
        if (tempWpn != null)
        {
            statWpn += GetWeaponTotalHP(tempWpn);
        }
        tempWpn = GameInfo.Instance.GetWeaponData(subWpnUID);
        if (tempWpn != null)
        {
            statWpn += GetWeaponStatBySubSlot(GetWeaponTotalHP(tempWpn), tempWpn.SkillLv);
        }

        int statCard = 0;
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            CardData tempCard = GameInfo.Instance.GetCardData(chardata.EquipCard[i]);
            if (tempCard != null)
            {
                if (i == (int)eCARDSLOT.SLOT_MAIN)
                    statCard += tempCard.GetCardHP();
                else
                    statCard += GetCardStatBySubSlot(tempCard.GetCardHP(), tempCard.SkillLv);
            }   
        }

        return chardata.GetCharHP() + statWpn + statCard + GetCostumeStat(chardata, eCHARABILITY.HP);
    }

    public static int GetTotalHP(CharData chardata, WeaponData mainWeaponData = null, WeaponData subWeaponData = null)
    {
        int statWpn = 0;
        if (mainWeaponData != null)
        {
            statWpn += GetWeaponTotalHP(mainWeaponData);
        }

        if (subWeaponData != null)
        {
            statWpn += GetWeaponStatBySubSlot(GetWeaponTotalHP(subWeaponData), subWeaponData.SkillLv);
        }

        int statCard = 0;
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            CardData tempCard = GameInfo.Instance.GetCardData(chardata.EquipCard[i]);
            if (tempCard != null)
            {
                if (i == (int)eCARDSLOT.SLOT_MAIN)
                {
                    statCard += tempCard.GetCardHP();
                }
                else
                {
                    statCard += GetCardStatBySubSlot(tempCard.GetCardHP(), tempCard.SkillLv);
                }
            }
        }

        return chardata.GetCharHP() + statWpn + statCard + GetCostumeStat(chardata, eCHARABILITY.HP);
    }

    public static int GetTotalHP(CharData chardata, WeaponData mainWeaponData, WeaponData subWeaponData, List<CardData> listCard, List<GemData> mainGemData, List<GemData> subGemData)
    {
        int statWpn = 0;

        WeaponData tempWpn = mainWeaponData;
        if (tempWpn != null)
        {
            statWpn += GetWeaponTotalHP(tempWpn, mainGemData);
        }

        tempWpn = subWeaponData;
        if (tempWpn != null)
        {
            statWpn += GetWeaponStatBySubSlot(GetWeaponTotalHP(tempWpn, subGemData), tempWpn.SkillLv);
        }

        int statCard = 0;

        if (listCard != null && listCard.Count > 0)
        {
            for (int i = 0; i < listCard.Count; i++)
            {
                CardData tempCard = listCard[i];
                if (tempCard == null)
                {
                    continue;
                }

                if (i == (int)eCARDSLOT.SLOT_MAIN)
                {
                    statCard += tempCard.GetCardHP();
                }
                else
                {
                    statCard += GetCardStatBySubSlot(tempCard.GetCardHP(), tempCard.SkillLv);
                }
            }
        }

        return chardata.GetCharHP() + statWpn + statCard + GetCostumeStat(chardata, eCHARABILITY.HP);
    }

    public static int GetTotalATK(CharData chardata, eWeaponSlot wpnSlot)
    {
        long mainWpnUID = chardata.EquipWeaponUID;
        long subWpnUID = chardata.EquipWeapon2UID;
        if (wpnSlot == eWeaponSlot.SUB)
        {
            mainWpnUID = chardata.EquipWeapon2UID;
            subWpnUID = chardata.EquipWeaponUID;
        }
        int statWpn = 0;
        WeaponData tempWpn = GameInfo.Instance.GetWeaponData(mainWpnUID);
        if (tempWpn != null)
        {
            statWpn += GetWeaponTotalATK(tempWpn);
        }
        tempWpn = GameInfo.Instance.GetWeaponData(subWpnUID);
        if (tempWpn != null)
        {
            statWpn += GetWeaponStatBySubSlot(GetWeaponTotalATK(tempWpn), tempWpn.SkillLv);
        }

        return chardata.GetCharATK() + statWpn + GetCostumeStat(chardata, eCHARABILITY.ATK);
    }

    public static int GetTotalATK(CharData chardata, WeaponData mainWeaponData = null, WeaponData subWeaponData = null)
    {
        int statWpn = 0;
        if (mainWeaponData != null)
        {
            statWpn += GetWeaponTotalATK(mainWeaponData);
        }

        if (subWeaponData != null)
        {
            statWpn += GetWeaponStatBySubSlot(GetWeaponTotalATK(subWeaponData), subWeaponData.SkillLv);
        }

        return chardata.GetCharATK() + statWpn + GetCostumeStat(chardata, eCHARABILITY.ATK);
    }

    public static int GetTotalATK(CharData chardata, WeaponData mainWeaponData, WeaponData subWeaponData, List<GemData> mainGemData, List<GemData> subGemData)
    {
        int statWpn = 0;

        WeaponData tempWpn = mainWeaponData;
        if (tempWpn != null)
        {
            statWpn += GetWeaponTotalATK(tempWpn, mainGemData);
        }

        tempWpn = subWeaponData;
        if (tempWpn != null)
        {
            statWpn += GetWeaponStatBySubSlot(GetWeaponTotalATK(tempWpn, subGemData), tempWpn.SkillLv);
        }

        return chardata.GetCharATK() + statWpn + GetCostumeStat(chardata, eCHARABILITY.ATK);
    }

    public static int GetTotalDEF(CharData chardata, eWeaponSlot wpnSlot)
    {
        long mainWpnUID = chardata.EquipWeaponUID;
        long subWpnUID = chardata.EquipWeapon2UID;
        if (wpnSlot == eWeaponSlot.SUB)
        {
            mainWpnUID = chardata.EquipWeapon2UID;
            subWpnUID = chardata.EquipWeaponUID;
        }
        int statWpn = 0;
        WeaponData tempWpn = GameInfo.Instance.GetWeaponData(mainWpnUID);
        if (tempWpn != null)
        {
            statWpn += GetWeaponTotalDEF(tempWpn);
        }
        tempWpn = GameInfo.Instance.GetWeaponData(subWpnUID);
        if (tempWpn != null)
        {
            statWpn += GetWeaponStatBySubSlot(GetWeaponTotalDEF(tempWpn), tempWpn.SkillLv);
        }

        int statCard = 0;
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            CardData tempCard = GameInfo.Instance.GetCardData(chardata.EquipCard[i]);
            if (tempCard != null)
            {
                if (i == (int)eCARDSLOT.SLOT_MAIN)
                    statCard += tempCard.GetCardDEF();
                else
                    statCard += GetCardStatBySubSlot(tempCard.GetCardDEF(), tempCard.SkillLv);
            }
        }

        return chardata.GetCharDEF() + statWpn + statCard + GetCostumeStat(chardata, eCHARABILITY.DEF);
    }

    public static int GetTotalDEF(CharData chardata, WeaponData mainWeaponData = null, WeaponData subWeaponData = null)
    {
        int statWpn = 0;
        if (mainWeaponData != null)
        {
            statWpn += GetWeaponTotalDEF(mainWeaponData);
        }

        if (subWeaponData != null)
        {
            statWpn += GetWeaponStatBySubSlot(GetWeaponTotalDEF(subWeaponData), subWeaponData.SkillLv);
        }

        int statCard = 0;
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            CardData tempCard = GameInfo.Instance.GetCardData(chardata.EquipCard[i]);
            if (tempCard != null)
            {
                if (i == (int)eCARDSLOT.SLOT_MAIN)
                {
                    statCard += tempCard.GetCardDEF();
                }
                else
                {
                    statCard += GetCardStatBySubSlot(tempCard.GetCardDEF(), tempCard.SkillLv);
                }
            }
        }

        return chardata.GetCharDEF() + statWpn + statCard + GetCostumeStat(chardata, eCHARABILITY.DEF);
    }

    public static int GetTotalDEF(CharData chardata, WeaponData mainWeaponData, WeaponData subWeaponData, List<CardData> listCard, List<GemData> mainGemData, List<GemData> subGemData)
    {
        int statWpn = 0;

        WeaponData tempWpn = mainWeaponData;
        if (tempWpn != null)
        {
            statWpn += GetWeaponTotalDEF(tempWpn, mainGemData);
        }

        tempWpn = subWeaponData;
        if (tempWpn != null)
        {
            statWpn += GetWeaponStatBySubSlot(GetWeaponTotalDEF(tempWpn, subGemData), tempWpn.SkillLv);
        }

        int statCard = 0;

        if (listCard != null && listCard.Count > 0)
        {
            for (int i = 0; i < listCard.Count; i++)
            {
                CardData tempCard = listCard[i];
                if (tempCard == null)
                {
                    continue;
                }

                if (i == (int)eCARDSLOT.SLOT_MAIN)
                {
                    statCard += tempCard.GetCardDEF();
                }
                else
                {
                    statCard += GetCardStatBySubSlot(tempCard.GetCardDEF(), tempCard.SkillLv);
                }
            }
        }

        return chardata.GetCharDEF() + statWpn + statCard + GetCostumeStat(chardata, eCHARABILITY.DEF);
    }

    public static int GetTotalCRI(CharData chardata, eWeaponSlot wpnSlot)
    {
        long mainWpnUID = chardata.EquipWeaponUID;
        long subWpnUID = chardata.EquipWeapon2UID;
        if (wpnSlot == eWeaponSlot.SUB)
        {
            mainWpnUID = chardata.EquipWeapon2UID;
            subWpnUID = chardata.EquipWeaponUID;
        }
        int statWpn = 0;
        WeaponData tempWpn = GameInfo.Instance.GetWeaponData(mainWpnUID);
        if (tempWpn != null)
        {
            statWpn += GetWeaponTotalCRI(tempWpn);
        }
        tempWpn = GameInfo.Instance.GetWeaponData(subWpnUID);
        if (tempWpn != null)
        {
            statWpn += GetWeaponStatBySubSlot(GetWeaponTotalCRI(tempWpn), tempWpn.SkillLv);
        }

        return chardata.GetCharCRI() + statWpn + GetCostumeStat(chardata, eCHARABILITY.CRI);
    }

    public static int GetTotalCRI(CharData chardata, WeaponData mainWeaponData = null, WeaponData subWeaponData = null)
    {
        int statWpn = 0;
        if (mainWeaponData != null)
        {
            statWpn += GetWeaponTotalCRI(mainWeaponData);
        }

        if (subWeaponData != null)
        {
            statWpn += GetWeaponStatBySubSlot(GetWeaponTotalCRI(subWeaponData), subWeaponData.SkillLv);
        }

        return chardata.GetCharCRI() + statWpn + GetCostumeStat(chardata, eCHARABILITY.CRI);
    }

    public static int GetTotalCRI(CharData chardata, WeaponData mainWeaponData, WeaponData subWeaponData, List<GemData> mainGemData, List<GemData> subGemData)
    {
        int statWpn = 0;

        WeaponData tempWpn = mainWeaponData;
        if (tempWpn != null)
        {
            statWpn += GetWeaponTotalCRI(tempWpn, mainGemData);
        }

        tempWpn = subWeaponData;
        if (tempWpn != null)
        {
            statWpn += GetWeaponStatBySubSlot(GetWeaponTotalCRI(tempWpn, subGemData), tempWpn.SkillLv);
        }

        return chardata.GetCharCRI() + statWpn + GetCostumeStat(chardata, eCHARABILITY.CRI);
    }

    public static float GetMaxHp(CharData chardata, eWeaponSlot wpnSlot)
    {
        BattleConfig bc = GameInfo.Instance.BattleConfig;
        int health = GetTotalHP(chardata, wpnSlot);
        float hp = health * bc.HPRateMag;

        float r = hp + (hp * (GetTotalCardFormationEffectValue() / 100.0f));
        return r;
    }

    public static float GetMaxHp(CharData chardata, WeaponData mainWeaponData, WeaponData subWeaponData, List<CardData> listCard, 
                               List<GemData> mainGemData, List<GemData> subGemData, float tempHp)
    {
        int health = GetTotalHP(chardata, mainWeaponData, subWeaponData, listCard, mainGemData, subGemData);

        float r = (int)(health * GameInfo.Instance.BattleConfig.HPRateMag);
        r = r + (r * (tempHp / 100.0f));

        return r;
    }

    public static float GetAttackPower(CharData chardata, eWeaponSlot wpnSlot)
    {
        BattleConfig bc = GameInfo.Instance.BattleConfig;
        int attack = GetTotalATK(chardata, wpnSlot);

        float r = (int)(attack * bc.ATKRateMag);
        r = r + (r * (GetTotalWeaponDepotEffectValue() / 100.0f));

        return r;
    }

    public static float GetAttackPower(CharData chardata, WeaponData mainWeaponData, WeaponData subWeaponData, List<GemData> mainGemData, 
                                       List<GemData> subGemData, float tempAtk)
    {
        int attack = GetTotalATK(chardata, mainWeaponData, subWeaponData, mainGemData, subGemData);

        float r = (int)(attack * GameInfo.Instance.BattleConfig.ATKRateMag);
        r = r + (r * (tempAtk / 100.0f));

        return r;
    }

    public static int GetDefenceRate(CharData chardata, eWeaponSlot wpnSlot)
    {
        BattleConfig bc = GameInfo.Instance.BattleConfig;
        int defence = GetTotalDEF(chardata, wpnSlot);
        return (int)(defence * bc.DEFRateMag);
    }

    public static int GetDefenceRate(CharData chardata, WeaponData mainWeaponData, WeaponData subWeaponData, List<CardData> listCard, List<GemData> mainGemData, List<GemData> subGemData)
    {
        int defence = GetTotalDEF(chardata, mainWeaponData, subWeaponData, listCard, mainGemData, subGemData);
        return (int)(defence * GameInfo.Instance.BattleConfig.DEFRateMag);
    }

    public static int GetCriticalRate(CharData chardata, eWeaponSlot wpnSlot)
    {
        BattleConfig bc = GameInfo.Instance.BattleConfig;
        int penetrate = GetTotalCRI(chardata, wpnSlot);
        return (int)(penetrate * bc.CRIRateMag);
    }

    public static int GetCriticalRate(CharData chardata, WeaponData mainWeaponData, WeaponData subWeaponData, List<GemData> mainGemData, List<GemData> subGemData)
    {
        int penetrate = GetTotalCRI(chardata, mainWeaponData, subWeaponData, mainGemData, subGemData);
        return (int)(penetrate * GameInfo.Instance.BattleConfig.CRIRateMag);
    }

    public static int GetWeaponTotalHP(WeaponData weapondata)
    {
        if (weapondata == null)
            return 0;

        int total = 0;
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {
            if (weapondata.SlotGemUID[i] == (int)eCOUNT.NONE)
                continue;
            GemData gemdata = GameInfo.Instance.GetGemData(weapondata.SlotGemUID[i]);
            if (gemdata == null)
                continue;
            total += gemdata.GetGemHP();
        }
        return total;
    }

    public static int GetWeaponTotalHP(WeaponData weapondata, List<GemData> listgem)
    {
        if (weapondata == null)
            return 0;

        int total = 0;
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {            
            if (listgem == null || listgem.Count <= 0 || listgem.Count <= i)
                continue;

            GemData gemdata = listgem[i];
            if (gemdata == null || gemdata.TableData == null)
                continue;
            total += gemdata.GetGemHP();
        }
        return total;
    }

    public static int GetWeaponTotalATK(WeaponData weapondata)
    {
        if (weapondata == null)
            return 0;

        int total = weapondata.GetWeaponATK();
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {
            if (weapondata.SlotGemUID[i] == (int)eCOUNT.NONE)
                continue;
            GemData gemdata = GameInfo.Instance.GetGemData(weapondata.SlotGemUID[i]);
            if (gemdata == null)
                continue;
            total += gemdata.GetGemATK();
        }
        return total;
    }

    public static int GetWeaponTotalATK(WeaponData weapondata, List<GemData> listgem)
    {
        if (weapondata == null)
            return 0;

        int total = weapondata.GetWeaponATK();
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {   
            if (listgem == null || listgem.Count <= 0 || listgem.Count <= i)
                continue;

            GemData gemdata = listgem[i];
            if (gemdata == null || gemdata.TableData == null)
                continue;
            total += gemdata.GetGemATK();
        }
        return total;
    }

    public static int GetWeaponTotalDEF(WeaponData weapondata)
    {
        if (weapondata == null)
            return 0;

        int total = 0;
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {
            if (weapondata.SlotGemUID[i] == (int)eCOUNT.NONE)
                continue;
            GemData gemdata = GameInfo.Instance.GetGemData(weapondata.SlotGemUID[i]);
            if (gemdata == null)
                continue;
            total += gemdata.GetGemDEF();
        }
        return total;
    }

    public static int GetWeaponTotalDEF(WeaponData weapondata, List<GemData> listgem)
    {
        if (weapondata == null)
            return 0;

        int total = 0;
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {   
            if (listgem == null || listgem.Count <= 0 || listgem.Count <= i)
                continue;

            GemData gemdata = listgem[i];
            if (gemdata == null || gemdata.TableData == null)
                continue;
            total += gemdata.GetGemDEF();
        }
        return total;
    }

    public static int GetWeaponTotalCRI(WeaponData weapondata)
    {
        if (weapondata == null)
            return 0;

        int total = weapondata.GetWeaponCRI();
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {
            if (weapondata.SlotGemUID[i] == (int)eCOUNT.NONE)
                continue;
            GemData gemdata = GameInfo.Instance.GetGemData(weapondata.SlotGemUID[i]);
            if (gemdata == null)
                continue;
            total += gemdata.GetGemCRI();
        }
        return total;
    }

    public static int GetWeaponTotalCRI(WeaponData weapondata, List<GemData> listgem)
    {
        if (weapondata == null)
            return 0;

        int total = weapondata.GetWeaponCRI();
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {            
            if (listgem == null || listgem.Count <= 0 || listgem.Count <= i)
                continue;

            GemData gemdata = listgem[i];
            if (gemdata == null || gemdata.TableData == null)
                continue;
            total += gemdata.GetGemCRI();
        }
        return total;
    }

    public static void GetGemTotalStat(long[] _slotgemuid, ref int[] _statusAry)
    {
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {
            GemData gemdata = GameInfo.Instance.GetGemData(_slotgemuid[i]);
            if (gemdata == null)
                continue;
            _statusAry[(int)eCHARABILITY.HP] += gemdata.GetGemHP();
            _statusAry[(int)eCHARABILITY.ATK] += gemdata.GetGemATK();
            _statusAry[(int)eCHARABILITY.DEF] += gemdata.GetGemDEF();
            _statusAry[(int)eCHARABILITY.CRI] += gemdata.GetGemCRI();
        }
    }

    public static void GetCardTotalStat(long[] _slotcarduid, ref int[] _statusAry)
    {
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            CardData tempCard = GameInfo.Instance.GetCardData(_slotcarduid[i]);
            if (tempCard != null)
            {
                if (i == (int)eCARDSLOT.SLOT_MAIN)
                {
                    _statusAry[(int)eCHARABILITY.HP] += tempCard.GetCardHP();
                    _statusAry[(int)eCHARABILITY.DEF] += tempCard.GetCardDEF();
                }
                else
                {
                    _statusAry[(int)eCHARABILITY.HP] += GameSupport.GetCardStatBySubSlot(tempCard.GetCardHP(), tempCard.SkillLv);
                    _statusAry[(int)eCHARABILITY.DEF] += GameSupport.GetCardStatBySubSlot(tempCard.GetCardDEF(), tempCard.SkillLv);
                }
            }
        }
    }

    public static void GetWeaponTotalStat(long mainUID, long subUID, ref int[] _statusAry)
    {
        WeaponData tempWpn = GameInfo.Instance.GetWeaponData(mainUID);
        if (tempWpn != null)
        {
            _statusAry[(int)eCHARABILITY.ATK] += tempWpn.GetWeaponATK();
            _statusAry[(int)eCHARABILITY.CRI] += tempWpn.GetWeaponCRI();
        }

        tempWpn = GameInfo.Instance.GetWeaponData(subUID);
        if (tempWpn != null)
        {
            _statusAry[(int)eCHARABILITY.ATK] += GameSupport.GetWeaponStatBySubSlot(tempWpn.GetWeaponATK(), tempWpn.SkillLv);
            _statusAry[(int)eCHARABILITY.CRI] += GameSupport.GetWeaponStatBySubSlot(tempWpn.GetWeaponCRI(), tempWpn.SkillLv);
        }
    }

    public static void GetHiGradeColorGem(ref List<GemData> gemdatalist, List<long> gemlist)
    {
        for (int i = 0; i < GameInfo.Instance.GemList.Count; i++)
        {
            WeaponData date = GameInfo.Instance.GetEquipGemWeaponData(GameInfo.Instance.GemList[i].GemUID);
            if (date != null)
                continue;

            bool bcheck = false;
            for (int j = 0; j < gemlist.Count; j++)
            {
                if (gemlist[j] == GameInfo.Instance.GemList[i].GemUID)
                    bcheck = true;
            }
            if (bcheck)
                continue;
            gemdatalist.Add(GameInfo.Instance.GemList[i]);
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  
    //
    //------------------------------------------------------------------------------------------------------------------------------------------
    public static int GetGemEmptySlotIdx(WeaponData weapondata)
    {
        int slotcount = GameSupport.GetWeaponGradeSlotCount(weapondata.TableData.Grade, weapondata.Wake);

        for (int i = 0; i < slotcount; i++)
        {
            if (weapondata.SlotGemUID[i] == (int)eCOUNT.NONE)
                return i;
        }

        return -1;
    }

    public static int GetGemAutoEpuipList(WeaponData weapondata, ref List<long> gemlist, ref List<int> slotlist)
    {
        int slotcount = GameSupport.GetWeaponGradeSlotCount(weapondata.TableData.Grade, weapondata.Wake);
        if (slotcount == 0)
            return 0;

        bool bbin = false;
        for (int i = 0; i < slotcount; i++)
        {
            GemData gemdata = GameInfo.Instance.GetGemData(weapondata.SlotGemUID[i]);
            if (gemdata == null)
                bbin = true;
        }
        if (!bbin)
            return 0;

        List<GemData> gemdatalist = new List<GemData>();
        for (int i = 0; i < slotcount; i++)
        {
            GemData gemdata = GameInfo.Instance.GetGemData(weapondata.SlotGemUID[i]);
            if (gemdata != null)
                continue;

            gemdatalist.Clear();
            GetHiGradeColorGem(ref gemdatalist, gemlist);

            if (gemdatalist.Count == 0)
                continue;

            gemdatalist.Sort(GemData.CompareFuncGradeLevel);

            gemlist.Add(gemdatalist[0].GemUID);
            slotlist.Add(i);
        }
        return gemlist.Count;
    }

    // 시설 이용시간
    public static float GetFacilityTime(FacilityData facilitydata, int itemcnt = 1) //분
    {
        float min = (float)facilitydata.GetEffectTime() * itemcnt;
        if (facilitydata.TableData.EffectType == "FAC_ITEM_COMBINE")
        {
            if (facilitydata.Selete != -1)
            {
                var _facilityitemcombinedata = GameInfo.Instance.GameTable.FindFacilityItemCombine((int)facilitydata.Selete);
                if (_facilityitemcombinedata != null)
                    min = _facilityitemcombinedata.Time * itemcnt;
            }
            if (facilitydata.EquipCardUID != (int)eCOUNT.NONE)
            {
                var carddata = GameInfo.Instance.GetCardData(facilitydata.EquipCardUID);
                if (carddata != null)
                {
                    float rate = GameSupport.GetFacilitySptOptValRate(facilitydata, carddata);
                    min -= (min * rate);
                }
            }
        }

        if (facilitydata.EquipCardUID != (int)eCOUNT.NONE)
        {
            var carddata = GameInfo.Instance.GetCardData(facilitydata.EquipCardUID);
            if (carddata != null)
            {
                min -= ((float)min * (GameInfo.Instance.GameConfig.FacilityCardGradeBaseSubTimeRatio[carddata.TableData.Grade] + (carddata.Level * GameInfo.Instance.GameConfig.FacilityCardGradeSubTimeRatio[carddata.TableData.Grade])) * 0.01f);
            }
        }
        return min;
    }
    
    // 시설 아이템 조합기 선택시 남은시간 반환
    public static float GetFacilityItemCombineTime(int _inTime, FacilityData facilitydata)
    {
        float min = (float)_inTime;

        if (facilitydata.EquipCardUID != (int)eCOUNT.NONE)
        {
            var carddata = GameInfo.Instance.GetCardData(facilitydata.EquipCardUID);
            if (carddata != null)
            {
                min -= ((float)min * (GameInfo.Instance.GameConfig.FacilityCardGradeBaseSubTimeRatio[carddata.TableData.Grade] + (carddata.Level * GameInfo.Instance.GameConfig.FacilityCardGradeSubTimeRatio[carddata.TableData.Grade])) * 0.01f);
                float rate = GameSupport.GetFacilitySptOptValRate(facilitydata, carddata);
                min -= (min * rate);
            }
        }
        return min;
    }

    public static float GetFacilitySptOptValRate(FacilityData facilitydata, CardData carddata)
    {
        float optRate = 0.0f;
        if (facilitydata.TableData.EffectType.Equals("FAC_CHAR_SP"))
        {
            if (carddata.TableData.SptSvrOptType.Equals("SptSvrOpt_FacCharSPRateUp"))
            {
                optRate = (carddata.TableData.SptSvrOptValue + (carddata.TableData.SptSvrOptIncValue * (carddata.SkillLv - 1))) / (float)eCOUNT.MAX_BO_FUNC_VALUE;
            }
        }
        else if (facilitydata.TableData.EffectType.Equals("FAC_CHAR_EXP"))
        {
            if (carddata.TableData.SptSvrOptType.Equals("SptSvrOpt_FacCharExpRateUp"))
            {
                optRate = (carddata.TableData.SptSvrOptValue + (carddata.TableData.SptSvrOptIncValue * (carddata.SkillLv - 1))) / (float)eCOUNT.MAX_BO_FUNC_VALUE;
            }
        }
        else if (facilitydata.TableData.EffectType.Equals("FAC_ITEM_COMBINE"))
        {
            if (carddata.TableData.SptSvrOptType.Equals("SptSvrOpt_FacItemTimeDown"))
            {
                optRate = (carddata.TableData.SptSvrOptValue + (carddata.TableData.SptSvrOptIncValue * (carddata.SkillLv - 1))) / (float)eCOUNT.MAX_BO_FUNC_VALUE;
            }
        }
        else if (facilitydata.TableData.EffectType.Equals("FAC_WEAPON_EXP"))
        {
            if (carddata.TableData.SptSvrOptType.Equals("SptSvrOpt_FacWpnExpRateUp"))
            {
                optRate = (carddata.TableData.SptSvrOptValue + (carddata.TableData.SptSvrOptIncValue * (carddata.SkillLv - 1))) / (float)eCOUNT.MAX_BO_FUNC_VALUE;
            }
        }
        return optRate;
    }

    /// 시설 적용 효과
    public static int GetFacilityEffectValue(FacilityData facilitydata, bool isOptValue = false)
    {
        float effect = facilitydata.GetEffectValue();
        if (isOptValue == true)
        {
            if (facilitydata.EquipCardUID != (int)eCOUNT.NONE)
            {
                var carddata = GameInfo.Instance.GetCardData(facilitydata.EquipCardUID);
                if (carddata != null)
                {
                    float rate = GameSupport.GetFacilitySptOptValRate(facilitydata, carddata);
                    effect += (effect * rate);
                }
            }
        }
        return (int)effect;
    }

    public static string GetFacilityIconName(FacilityData facilityData)
    {
        string stricon = "";
        if (facilityData.TableData.EffectType.Equals("FAC_CHAR_EXP"))
        {
            stricon = "Facility_Combat.png";
        }
        else if (facilityData.TableData.EffectType.Equals("FAC_CHAR_SP"))
        {
            stricon = "Facility_Skill.png";
        }
        else if (facilityData.TableData.EffectType.Equals("FAC_ITEM_COMBINE"))
        {
            stricon = "Facility_Item.png";
        }
        else if (facilityData.TableData.EffectType.Equals("FAC_WEAPON_EXP"))
        {
            stricon = "Facility_WeaponForce.png";
        }
        else if (facilityData.TableData.EffectType.Equals("FAC_CARD_TRADE"))
        {
            stricon = "Facility_Trade.png";
        }
        return stricon;
    }

    public static bool IsItemReqList(GameTable.ItemReqList.Param reqdata)
    {
        if(reqdata == null)
        {
            return false;
        }

        List<int> idlist = new List<int>();
        List<int> countlist = new List<int>();
        SetMatList(reqdata, ref idlist, ref countlist);

        bool _bmat = true;

        for (int i = 0; i < idlist.Count; i++)
        {
            int orgcut = GameInfo.Instance.GetItemIDCount(idlist[i]);
            int orgmax = countlist[i];
            if (orgcut < orgmax)
            {
                _bmat = false;
            }
        }
        if( !GameInfo.Instance.UserData.IsGoods(eGOODSTYPE.GOLD, reqdata.Gold) )
            _bmat = false;

        return _bmat;
    }
    //------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  
    //
    //------------------------------------------------------------------------------------------------------------------------------------------
    public static void PlayLobbyBGM(int slot)
    {
        int nowbgm = SoundManager.Instance.BGMID;//FSoundGOManager.Instance.BGMID;
        int nextbgm = -1;
        string strnowbgm = string.Empty;
        if (slot == -1 || slot == 10 || slot == 11 || slot == 12 )
        {
            nextbgm = 1000;
        }
        else if (0 <= slot && (int)eCOUNT.ROOMCOUNT > slot)
        {
            var tabledata = GameInfo.Instance.GameTable.FindRoomTheme(x => x.ID == GameInfo.Instance.UserData.RoomThemeSlot);
            if (tabledata != null)
                nextbgm = tabledata.Bgm;
        }
        else
        {
            SoundManager.Instance.StopBgm();
        }

        if (nowbgm != nextbgm)
        {
            //  룸,로비 변경시 페이드 하지 않고 바로 변경되도록 하였습니다.
            SoundManager.Instance.PlayBgm(nextbgm, 1);
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  
    //
    //------------------------------------------------------------------------------------------------------------------------------------------

    // 아이콘 슬롯 정보 출력
    public static void UpdateSlotByRewardData(GameObject parentObj, GameObject mySelfObj, RewardData rewardData,
        UISprite kFrmGradeSpr, UISprite kGradeSpr, UISprite kTypeSpr, UISprite kBGSpr,
        UILabel kCountLabel, UITexture kIconTex, UISprite kIconSpr, UIGoodsUnit kGoodsUnit,
        UISprite kInactiveSpr = null)
    {
        kFrmGradeSpr.gameObject.SetActive(true);
        kBGSpr.gameObject.SetActive(true);
        kCountLabel.gameObject.SetActive(true);
        kIconTex.gameObject.SetActive(true);

        kGradeSpr.gameObject.SetActive(false);
        if(kTypeSpr)
            kTypeSpr.gameObject.SetActive(false);

        if (kIconSpr != null)
            kIconSpr.gameObject.SetActive(false);
        if (kGoodsUnit != null)
            kGoodsUnit.gameObject.SetActive(false);
        if (kInactiveSpr != null)
            kInactiveSpr.gameObject.SetActive(false);

        switch ((eREWARDTYPE)rewardData.Type)
        {
            case eREWARDTYPE.GOODS:
                {
                    kFrmGradeSpr.spriteName = "goodsfrm_1";
                    kBGSpr.spriteName = "goodsbgSlot_1";
                    kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), rewardData.Value);
                    kIconTex.gameObject.SetActive(false);

                    if (kIconSpr != null)
                    {
                        kIconSpr.gameObject.SetActive(true);
                        kIconSpr.spriteName = GameSupport.GetGoodsIconName((eGOODSTYPE)rewardData.Index);
                    }
                    if (kGoodsUnit != null)
                    {
                        kGoodsUnit.gameObject.SetActive(true);
                        kGoodsUnit.InitGoodsUnit((eGOODSTYPE)rewardData.Index, rewardData.Value);
                        kGoodsUnit.kIconSpr.spriteName = GameSupport.GetGoodsIconName((eGOODSTYPE)rewardData.Index);
                        kGoodsUnit.kTextLabel.gameObject.SetActive(false);
                        kGoodsUnit.gameObject.SetActive(true);
                    }
                }
                break;
            case eREWARDTYPE.CHAR:
                {
                    GameTable.Character.Param tabledata = GameInfo.Instance.GameTable.FindCharacter(rewardData.Index);
                    if (tabledata != null)
                    {
                        kFrmGradeSpr.spriteName = "goodsfrm_1";
                        kBGSpr.spriteName = "itembgSlot_weapon_" + (int)eGRADE.GRADE_UR;
                        kCountLabel.textlocalize = "";
                        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/MainSlot/MainSlot_" + tabledata.Icon + "_" + tabledata.InitCostume.ToString() + ".png");

                        if (kInactiveSpr != null)
                        {
                            CharData useChar = GameInfo.Instance.CharList.Find(x => x.TableID == tabledata.ID);
                            if (useChar != null)
                            {
                                kInactiveSpr.gameObject.SetActive(true);
                            }
                        }
                    }
                }
                break;
            case eREWARDTYPE.WEAPON:
                {
                    GameTable.Weapon.Param tabledata = GameInfo.Instance.GameTable.FindWeapon(rewardData.Index);
                    if (tabledata != null)
                    {
                        kFrmGradeSpr.spriteName = "itemfrm_weapon_" + tabledata.Grade.ToString();
                        kBGSpr.spriteName = "itembgSlot_weapon_" + tabledata.Grade.ToString();
                        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), 1);
                        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + tabledata.Icon);

                        kGradeSpr.gameObject.SetActive(true);
                        kGradeSpr.spriteName = "itemgrade_S_" + tabledata.Grade.ToString();
                    }
                }
                break;
            case eREWARDTYPE.GEM:
                {
                    GameTable.Gem.Param tabledata = GameInfo.Instance.GameTable.FindGem(rewardData.Index);
                    if (tabledata != null)
                    {
                        kFrmGradeSpr.spriteName = "itemfrm_" + tabledata.Grade.ToString();
                        kBGSpr.spriteName = "itembgSlot_" + tabledata.Grade.ToString();
                        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), 1);
                        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + tabledata.Icon);
                    }
                }
                break;
            case eREWARDTYPE.CARD:
                {
                    GameTable.Card.Param tabledata = GameInfo.Instance.GameTable.FindCard(rewardData.Index);
                    if (tabledata != null)
                    {
                        kFrmGradeSpr.spriteName = "itemfrm_weapon_" + tabledata.Grade.ToString();
                        kBGSpr.spriteName = "itembgSlot_weapon_" + tabledata.Grade.ToString();
                        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), 1);
                        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Item/{0}.png", tabledata.Icon));

                        kGradeSpr.gameObject.SetActive(true);
                        kGradeSpr.spriteName = "itemgrade_S_" + tabledata.Grade.ToString();
                        if (kTypeSpr != null)
                        {
                            kTypeSpr.gameObject.SetActive(true);
                            kTypeSpr.spriteName = "SupporterType_" + tabledata.Type.ToString();
                        }
                    }
                }
                break;
            case eREWARDTYPE.COSTUME:
                {
                    GameTable.Costume.Param tabledata = GameInfo.Instance.GameTable.FindCostume(rewardData.Index);
                    if (tabledata != null)
                    {
                        var chartabledata = GameInfo.Instance.GameTable.FindCharacter(tabledata.CharacterID);
                        if (chartabledata != null)
                        {
                            kFrmGradeSpr.spriteName = "goodsfrm_1";
                            kBGSpr.spriteName = "itembgSlot_" + tabledata.Grade.ToString();
                            kCountLabel.textlocalize = "";
                            kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/MainSlot/MainSlot_" + chartabledata.Icon + "_" + tabledata.ID.ToString() + ".png");

                            if (kInactiveSpr != null)
                            {
                                CharData useChar = GameInfo.Instance.CharList.Find(x => x.TableID == chartabledata.ID);
                                if (useChar == null)
                                {
                                    kInactiveSpr.gameObject.SetActive(true);
                                }
                                int useCostume = GameInfo.Instance.CostumeList.Find(x => x == tabledata.ID);
                                if (useCostume != 0)
                                {
                                    kInactiveSpr.gameObject.SetActive(true);
                                }
                            }
                        }
                    }
                }
                break;
            case eREWARDTYPE.ITEM:
                {
                    GameTable.Item.Param tabledata = GameInfo.Instance.GameTable.FindItem(rewardData.Index);
                    if (tabledata != null)
                    {
                        kFrmGradeSpr.spriteName = "itemfrm_" + tabledata.Grade.ToString();
                        kBGSpr.spriteName = "itembgSlot_" + tabledata.Grade.ToString();
                        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), rewardData.Value);
                        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + tabledata.Icon);
                    }
                }
                break;
            case eREWARDTYPE.USERMARK:
                {
                    GameTable.UserMark.Param tabledata = GameInfo.Instance.GameTable.FindUserMark(x => x.ID == rewardData.Index);
                    if (tabledata != null)
                    {
                        kFrmGradeSpr.spriteName = "goodsfrm_1";
                        kBGSpr.spriteName = "goodsbgSlot_1";
                        //kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), 1);
                        kCountLabel.textlocalize = "";

                        LobbyUIManager.Instance.GetUserMarkIcon(parentObj, mySelfObj, tabledata.ID, ref kIconTex);

                        if (kInactiveSpr != null)
                        {
                            bool useMark = GameInfo.Instance.UserMarkList.Contains(tabledata.ID);
                            if (useMark)
                            {
                                kInactiveSpr.gameObject.SetActive(true);
                            }
                        }
                    }
                }
                break;
            case eREWARDTYPE.BADGE:
                {
                    GameTable.BadgeOpt.Param tabledata = GameInfo.Instance.GameTable.FindBadgeOpt(rewardData.Index);
                    if (tabledata != null)
                    {
                        kFrmGradeSpr.spriteName = "goodsfrm_1";
                        kBGSpr.spriteName = "goodsbgSlot_1";
                        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), 0);
                        kIconTex.mainTexture = GameSupport.GetBadgeIcon(tabledata);
                    }
                }
                break;
            case eREWARDTYPE.BUFF:
                {
                    GameTable.Buff.Param tabledata = GameInfo.Instance.GameTable.FindBuff(rewardData.Index);
                    if (tabledata != null)
                    {
                        
                    }
                }
                break;
            case eREWARDTYPE.LOBBYTHEME:
                {
                    GameTable.LobbyTheme.Param tabledata = GameInfo.Instance.GameTable.FindLobbyTheme(rewardData.Index);
                    if (tabledata != null)
                    {
                        kFrmGradeSpr.spriteName = "goodsfrm_1";
                        kIconTex.gameObject.SetActive(true);
                        kCountLabel.textlocalize = "";
                        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/" + tabledata.Icon);
                        if (kInactiveSpr != null)
                        {
                            bool lobbytheme = GameInfo.Instance.UserLobbyThemeList.Contains(tabledata.ID);
                            if (lobbytheme)
                            {
                                kInactiveSpr.gameObject.SetActive(true);
                            }
                        }
                    }
                }
                break;
            case eREWARDTYPE.LOBBYANIMATION:
                {
                    GameTable.LobbyAnimation.Param tabledata = GameInfo.Instance.GameTable.FindLobbyAnimation(rewardData.Index);
                    if (tabledata != null)
                    {
                        kFrmGradeSpr.spriteName = "goodsfrm_1";
                        kBGSpr.spriteName = "goodsbgSlot_1";

                        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), rewardData.Value);
                        kIconTex.gameObject.SetActive(false);
                        
                        UnityEngine.Object atlasObj = FLocalizeAtlas.Instance.GetLocalizeAtlas("Lobby");
                        if (atlasObj != null)
                        {
                            GameObject obj = atlasObj as GameObject;
                            if (obj != null)
                            {
                                UIAtlas atlas = obj.GetComponent<UIAtlas>();
                                if (atlas != null)
                                {
                                    kIconSpr.atlas = atlas;
                                }
                            }
                        }

                        kIconSpr.spriteName = tabledata.Icon;
                        kIconSpr.gameObject.SetActive(true);
                    }
                }
                break;
            default:
                break;
        }
    }

	/// <summary>
	/// 재화별 이름 반환
	/// </summary>
	public static string GetProductName( RewardData data ) {
		if ( data == null ) {
			return "";
		}

		switch ( (eREWARDTYPE)data.Type ) {
			case eREWARDTYPE.GOODS: {
				string name = FLocalizeString.Instance.GetText( (int)eTEXTID.GOODSTYPE_TEXT_START + data.Index );
				string value = string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.GOODSTEXT ), data.Value );

				if ( (eGOODSTYPE)data.Index == eGOODSTYPE.CASH ) {
					return string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.SPACE_TYPE_TEXT ), name, string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.STACK_COUNT_TXT ), value ) );
				}
				else if ( (eGOODSTYPE)data.Index == eGOODSTYPE.RAIDPOINT ) {
					name = FLocalizeString.Instance.GetText( 1871 );
					return FLocalizeString.Instance.GetText( (int)eTEXTID.SPACE_TYPE_TEXT, name, FLocalizeString.Instance.GetText( (int)eTEXTID.STACK_COUNT_TXT, value ) );
				}
				else {
					return string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.SPACE_TYPE_TEXT ), value, name );
				}
			}
			case eREWARDTYPE.WEAPON: {
				GameTable.Weapon.Param param = GameInfo.Instance.GameTable.FindWeapon( data.Index );
				if ( param == null ) {
					return "";
				}

				return FLocalizeString.Instance.GetText( param.Name );
			}
			case eREWARDTYPE.GEM: {
				GameTable.Gem.Param param = GameInfo.Instance.GameTable.FindGem( data.Index );
				if ( param == null ) {
					return "";
				}

				return FLocalizeString.Instance.GetText( param.Name );
			}
			case eREWARDTYPE.CARD: {
				GameTable.Card.Param param = GameInfo.Instance.GameTable.FindCard( data.Index );
				if ( param == null ) {
					return "";
				}

				return FLocalizeString.Instance.GetText( param.Name );
			}
			case eREWARDTYPE.ITEM: {
				GameTable.Item.Param param = GameInfo.Instance.GameTable.FindItem( data.Index );
				if ( param == null ) {
					return "";
				}

				string name = FLocalizeString.Instance.GetText( param.Name );
				string value = string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.GOODSTEXT ), data.Value );
				return string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.SPACE_TYPE_TEXT ), name, string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.STACK_COUNT_TXT ), value ) );
			}
			case eREWARDTYPE.CHAR: {
				GameTable.Character.Param param = GameInfo.Instance.GameTable.FindCharacter( data.Index );
				if ( param == null ) {
					return "";
				}

				return FLocalizeString.Instance.GetText( param.Name );
			}
			case eREWARDTYPE.COSTUME: {
				GameTable.Costume.Param param = GameInfo.Instance.GameTable.FindCostume( data.Index );
				if ( param == null ) {
					return "";
				}

				return FLocalizeString.Instance.GetText( param.Name );
			}
			case eREWARDTYPE.PACKAGE: {
				return FLocalizeString.Instance.GetText( 1307 );
			}
			case eREWARDTYPE.USERMARK: {
				GameTable.UserMark.Param param = GameInfo.Instance.GameTable.FindUserMark( data.Index );
				if ( param == null ) {
					return "";
				}

				return FLocalizeString.Instance.GetText( param.Name );
			}
			case eREWARDTYPE.BADGE: {
				//곡옥 뽑기는 display 테이블에있는 이름으로 셋팅
				if ( data.Index == 0 ) {
					return "";
				}

				GameTable.BadgeOpt.Param param = GameInfo.Instance.GameTable.FindBadgeOpt( data.Index );
				if ( param == null ) {
					return "";
				}

				return FLocalizeString.Instance.GetText( param.Name );
			}
			case eREWARDTYPE.BUFF: {
				GameTable.Buff.Param param = GameInfo.Instance.GameTable.FindBuff( data.Index );
				if ( param == null ) {
					return "";
				}

				return FLocalizeString.Instance.GetText( param.Name );
			}
			case eREWARDTYPE.LOBBYTHEME: {
				GameTable.LobbyTheme.Param param = GameInfo.Instance.GameTable.FindLobbyTheme( data.Index );
				if ( param == null ) {
					return "";
				}

				return FLocalizeString.Instance.GetText( param.Name );
			}
			default: {
				return "";
			}
		}
	}

    public static string GetGoodsIconName(eGOODSTYPE etype, bool bIsGoodsUnit = false)
    {
        string iconName = string.Empty;

        if (etype == eGOODSTYPE.GOLD)
            iconName = "Goods_Gold";
        else if (etype == eGOODSTYPE.CASH)
            iconName = "Goods_Cash";
        else if (etype == eGOODSTYPE.SUPPORTERPOINT)
            iconName = "Goods_Supporterpoint";
        else if (etype == eGOODSTYPE.AP)
            iconName = "Goods_Ticket";
        else if (etype == eGOODSTYPE.BP)
            iconName = "Goods_TimeAttack";
        else if (etype == eGOODSTYPE.ROOMPOINT)
            iconName = "Goods_Roompoint";
        else if (etype == eGOODSTYPE.BATTLECOIN)
            iconName = "Goods_ArenaGold";
        else if (etype == eGOODSTYPE.FRIENDPOINT)
            iconName = "Goods_FP";
        else if (etype == eGOODSTYPE.DESIREPOINT)
            iconName = "Goods_DesireGacha";
        else if(etype == eGOODSTYPE.AWAKEPOINT)
        {
            iconName = "Goods_Wpoint";
        }
        else if( etype == eGOODSTYPE.RAIDPOINT ) {
            iconName = "Goods_VRGold";
        }
        else
            iconName = string.Empty;

        if (iconName != string.Empty && bIsGoodsUnit == true)
            iconName = string.Format("{0}_s", iconName);

        return iconName;
    }
    //------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  
    //
    //------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///  주간 미션 보상 확인
    /// </summary>
    public static bool IsComplateMissionRecive(uint ComplateFlag, int rewardCount)
    {
        //  2의 10승 보다 높으면 
        if (32 <= rewardCount)
            return false;

        int checkFlag = 0x0000000001 << rewardCount;
        return ( checkFlag == ( ComplateFlag & (System.UInt32)checkFlag ) );
    }
    
    /// <summary>
    ///  주간 미션 수령가능 갯수 확인
    /// </summary>
    /// <returns></returns>
    public static int GetPossibleReciveMissionRewardCount()
    {
        uint rewardFlag = GameInfo.Instance.WeekMissionData.fMissionRewardFlag;
        List<uint> missions = new List<uint>(GameInfo.Instance.WeekMissionData.fMissionRemainCntSlot);
        int possibleCount = 0;
        int recivedCount = 0;

        for (int i = 0; i< (int)eCOUNT.WEEKMISSIONCOUNT; i++)
        {
            //  수령 가능 갯수
            if(IsComplateMissionRecive(rewardFlag,i) != true && missions[i] == 0)
                possibleCount++;

            //  수령한 갯수
            if (IsComplateMissionRecive(rewardFlag, i) == true)
                recivedCount++;
        }
        
        int[] missionClearCount = { 2, 5, 7 };
        for(int i = 0; i< 3; i++)
        {
            //  클리어 갯수에 대한 수령 가능 갯수 체크
            if (recivedCount >= missionClearCount[i] && IsComplateMissionRecive(rewardFlag, 7+i) != true)
                possibleCount++;
        }

        return possibleCount;
    }

    /// <summary>
    ///  미션 스트링과 타입을 비교
    /// </summary>
    public static int CompareMissionString(string desc)
    {
        int num = 0;
        for (int i = 1; i < (int)eMISSIONTYPE._MAX_; i++)
        {
            string missionName = ( (eMISSIONTYPE)i ).ToString().ToUpper();
            if (string.Compare(desc, missionName, true) == 0)
            {
                num = i;
                break;
            }
        }

        return num;
    }

    //이벤트 관련--------------------------------------------------------------------------------------------------------------------------------
    public static bool IsPossibleLoginBonus(bool checkLoginEvent = true)
    {
        //  이전 로그인 타임에 1일을 더한 내일로그인 가능 시간 확인
        //DateTime tomorrow = GameSupport.GetLocalTimeByServerTime(GameInfo.Instance.UserData.LoginBonusRecentDate.AddDays(1));
        //TimeSpan span = GameSupport.GetRemainTime(tomorrow);
        //return (span.TotalHours <= 0);

        DateTime tomorrow = GameInfo.Instance.UserData.LoginBonusRecentDate.AddDays(1);
        DateTime tom = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day);
        TimeSpan span = GetRemainTime(tom, System.DateTime.UtcNow.AddSeconds(GameInfo.Instance.ServerData.ServerTimeGap));

		return (span.TotalHours <= 0);
    }

    //이벤트 관련--------------------------------------------------------------------------------------------------------------------------------
    //77 이벤트 입장 가능 여부
    public static int GetJoinEventState(int eventid)
    {
        EventSetData eventdata = GameInfo.Instance.GetEventSetData(eventid);
		if (eventdata == null || eventdata.TableData == null)
		{
			return (int)eEventState.EventNone;
		}

        DateTime eventStartTime = GetTimeWithString(eventdata.TableData.StartTime);
        DateTime eventEndTime = GetTimeWithString(eventdata.TableData.EndTime, true);
        DateTime eventOpenTime = GetTimeWithString(eventdata.TableData.PlayOpenTime);
        DateTime eventCloseTime = GetTimeWithString(eventdata.TableData.PlayCloseTime, true);

        DateTime nowTime = GameInfo.Instance.GetNetworkTime();

        int playstate = (int)eEventState.EventNone;

        if (eventStartTime > nowTime)
        {   // 이벤트 시작전
            playstate = (int)eEventState.EventNotStart;
        }
        else
        {
            if (eventCloseTime > nowTime)
            {   // 이벤트 플레이 가능
                playstate = (int)eEventState.EventPlaying;
            }
            else if (eventEndTime > nowTime)
            {   // 이벤트 보상 획득까지만 가능
                playstate = (int)eEventState.EventOnlyReward;
            }
            else
            {   // 이벤트 종료 상태
                playstate = (int)eEventState.EventEnd;
            }
        }
        return playstate;
    }
    //UI 이동--------------------------------------------------------------------------------------------------------------------------------
    public static eUIMOVEFUNCTIONTYPE GetGannerFunctionType(string subject)
    {
		string[] strs = Enum.GetNames(typeof(eUIMOVEFUNCTIONTYPE));
		for(int i = 0; i < strs.Length; ++i)
		{
			if(strs[i].CompareTo(subject) == 0)
			{
				return (eUIMOVEFUNCTIONTYPE)i;
			}
		}
		/*
        foreach (eUIMOVEFUNCTIONTYPE type in (eUIMOVEFUNCTIONTYPE[])System.Enum.GetValues(typeof(eUIMOVEFUNCTIONTYPE)))
        {
            if (type.ToString() == subject)
                return type;
        }
		*/
        return eUIMOVEFUNCTIONTYPE.NONE;
    }

    public static void MoveUI( string strtype, string strvalue1, string strvalue2, string strvalue3 )
    {
        if (LobbyUIManager.Instance.LoginBonusStep != eLoginBonusStep.End)
        {
            LobbyUIManager.Instance.StopDailyLoginPopup();
            if (LobbyUIManager.Instance.IsOnceLoginBonusPopup == false)
            {
                LobbyUIManager.Instance.LoginBonusStep = eLoginBonusStep.End;
            }
        }
        
        eUIMOVEFUNCTIONTYPE functiontype = GetGannerFunctionType(strtype);
        
        if (functiontype == eUIMOVEFUNCTIONTYPE.UIPANEL)
        {
            ePANELTYPE panel = LobbyUIManager.GetPanelType(strvalue1);
            string strpopup = string.Empty;
            bool isFacility = false;

            if (panel == ePANELTYPE.GACHA )
            {
                UIValue.Instance.SetValue(UIValue.EParamType.GachaTab, strvalue2);
                UIValue.Instance.SetValue(UIValue.EParamType.GachaTabValue03, strvalue3);
                LobbyUIManager.Instance.SetPanelType(panel);
            }
            else if (panel == ePANELTYPE.STORE)
            {
                UIStorePanel storePanel = LobbyUIManager.Instance.GetUI<UIStorePanel>("StorePanel");
                if (storePanel != null)
                {
                    Enum.TryParse(strvalue2, out UIStorePanel.eStoreTabType result);
                    storePanel.DirectShow(result);
                }
                LobbyUIManager.Instance.SetPanelType(panel);
            }
            else if (panel == ePANELTYPE.FACILITY || panel == ePANELTYPE.FACILITYITEM || panel == ePANELTYPE.FACILITYTRADE)
            {
                isFacility = true;
                int value2 = int.Parse(strvalue2) - 1; // 시설 인덱스 번호, 배열 시작이 0이기 때문에 -1
                FacilityData data = GameInfo.Instance.FacilityList[value2];

                if (data == null) return;
                if (GameInfo.Instance.UserData.Level < data.TableData.FacilityOpenUserRank)
                {
                    //지휘관 RANK가 부족합니다.
                    string msg = string.Format("{0}\n{1}", FLocalizeString.Instance.GetText(3205), string.Format(FLocalizeString.Instance.GetText(1365), data.TableData.FacilityOpenUserRank));
                    MessageToastPopup.Show(msg);
                    if (LobbyUIManager.Instance.LoginBonusStep != eLoginBonusStep.End)
                    {
                        LobbyUIManager.Instance.ShowDailyLoginPopup(false);
                    }
                    return;
                }

                if (LobbyUIManager.Instance.IsActiveUI("FacilityPanel") || LobbyUIManager.Instance.IsActiveUI("FacilityItemPanel"))
                {   
                    LobbyUIManager.Instance.ChangeFacility(data.TableData.EffectType);
                }
                else
                {
                    ////뒷 Lobby BG 제거 후 이동
                    LobbyUIManager.Instance.PanelBGAllHide();
                    LobbyUIManager.Instance.HideUI("CharInfoPanel");
                    LobbyUIManager.Instance.HideUI("ItemPanel");
                    LobbyUIManager.Instance.HideUI("GachaPanel");
                    LobbyUIManager.Instance.HideUI("StorePanel");
                    LobbyUIManager.Instance.HideUI("DailyPanel");
                    LobbyUIManager.Instance.HideUI("ArenaMainPanel");
                    LobbyUIManager.Instance.HideUI("ArenaTowerMainPanel"); 
                    LobbyUIManager.Instance.HideUI("ArenaBattleConfirmPopup");
                    LobbyUIManager.Instance.HideUI("EventmodeStoryResetGachaPanel", false);
                    UIMainPanel mainpanel = LobbyUIManager.Instance.GetActiveUI<UIMainPanel>("MainPanel");
                    if (mainpanel != null)
                        mainpanel.OnClick_FacilityBtn(value2);
                    else
                    {   
                        UIValue.Instance.SetValue(UIValue.EParamType.FacilityID, data.TableID);
                        var obj = UIValue.Instance.GetValue(UIValue.EParamType.FacilityID);
                        if (obj == null)
                            return;

                        var facilitydata = GameInfo.Instance.GetFacilityData((int)obj);
                        var list = GameInfo.Instance.FacilityList.FindAll(x => x.TableData.ParentsID == facilitydata.TableID);

                        GoToFacility();
                    }
                }
            }
            else if (panel.Equals(ePANELTYPE.EVENT_STORY_MAIN) || panel.Equals(ePANELTYPE.EVENT_CHANGE_MAIN) || panel.Equals(ePANELTYPE.EVENT_MISSION))
            {
                var stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == GameInfo.Instance.GameConfig.StageTypeOpenIDList[(int)eSTAGETYPE.STAGE_EVENT]);
                if (stagecleardata == null)
                {
                    var stagedata = GameInfo.Instance.GameTable.FindStage(GameInfo.Instance.GameConfig.StageTypeOpenIDList[(int)eSTAGETYPE.STAGE_EVENT]);
                    if (stagedata != null)
                        MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LOCKMSG + (int)UIButtonLock.eLOCKCHECKTYPE.CHAPTERCLEAR), stagedata.Chapter));
                    return;
                }

                int value2 = int.Parse(strvalue2);
                EventSetData eventdata = GameInfo.Instance.GetEventSetData(value2);
                if (eventdata == null)
                {
                    MessageToastPopup.Show(FLocalizeString.Instance.GetText(3054));
                    return;
                }
                int state = GameSupport.GetJoinEventState(eventdata.TableID);
                if (state < (int)eEventState.EventNone)
                {
                    if (state == (int)eEventState.EventNotStart)
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3052));
                    else
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3053));
                    return;
                }
                UIValue.Instance.SetValue(UIValue.EParamType.EventID, value2);
                LobbyUIManager.Instance.SetPanelType(panel);
            }
            else if (panel.Equals(ePANELTYPE.STORY))
            {
                if (strvalue2 != string.Empty)
                {
                    int value2 = int.Parse(strvalue2);
                    var stagetabledata = GameInfo.Instance.GameTable.FindStage(x => x.ID == value2);
                    bool bmove = false;
                    if (stagetabledata.LimitStage == -1)
                        bmove = true;
                    else
                    {
                        var limitstagedata = GameInfo.Instance.StageClearList.Find(x => x.TableID == stagetabledata.LimitStage);
                        if (limitstagedata != null)
                            bmove = true;
                    }
                    if (!bmove)
                    {
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3063));
                        return;
                    }
                    else
                    {
                        UIValue.Instance.SetValue(UIValue.EParamType.StageID, value2);
                        UIValue.Instance.SetValue(UIValue.EParamType.StoryStageID, value2);
                        strpopup = "StageDetailPopup";
                    }
                }
                else
                {
                    int stageid = GameSupport.GetStorySuitableStageID(eSTAGETYPE.STAGE_MAIN_STORY);
                    UIValue.Instance.SetValue(UIValue.EParamType.StoryStageID, stageid);
                }
                LobbyUIManager.Instance.SetPanelType(panel);
            }
            else if (panel.Equals(ePANELTYPE.DAILY))
            {
                var stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == GameInfo.Instance.GameConfig.StageTypeOpenIDList[(int)eSTAGETYPE.STAGE_DAILY]);
                if (stagecleardata == null)
                {
                    var stagedata = GameInfo.Instance.GameTable.FindStage(GameInfo.Instance.GameConfig.StageTypeOpenIDList[(int)eSTAGETYPE.STAGE_DAILY]);
                    if (stagedata != null)
                        MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LOCKMSG + (int)UIButtonLock.eLOCKCHECKTYPE.CHAPTERCLEAR), stagedata.Chapter));
                    return;
                }

                if (strvalue2 != string.Empty)
                {
                    int value2 = int.Parse(strvalue2);
                    var stagetabledata = GameInfo.Instance.GameTable.FindStage(x => x.ID == value2);
                    bool bmove = false;
                    if (stagetabledata.LimitStage == -1)
                        bmove = true;
                    else
                    {
                        var limitstagedata = GameInfo.Instance.StageClearList.Find(x => x.TableID == stagetabledata.LimitStage);
                        if (limitstagedata != null)
                            bmove = true;
                    }
                    if (!bmove)
                    {
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3063));
                        return;
                    }
                    else
                    {
                        UIValue.Instance.SetValue(UIValue.EParamType.StageID, value2);
                        strpopup = "StageDetailPopup";
                    }
                }
                LobbyUIManager.Instance.SetPanelType(panel);
            }
            else if (panel.Equals(ePANELTYPE.TIMEATTACK))
            {
                var stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == GameInfo.Instance.GameConfig.StageTypeOpenIDList[(int)eSTAGETYPE.STAGE_TIMEATTACK]);
                if (stagecleardata == null)
                {
                    var stagedata = GameInfo.Instance.GameTable.FindStage(GameInfo.Instance.GameConfig.StageTypeOpenIDList[(int)eSTAGETYPE.STAGE_TIMEATTACK]);
                    if (stagedata != null)
                        MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LOCKMSG + (int)UIButtonLock.eLOCKCHECKTYPE.CHAPTERCLEAR), stagedata.Chapter));
                    return;
                }

                if (strvalue2 != string.Empty)
                {
                    int value2 = int.Parse(strvalue2);
                    var stagetabledata = GameInfo.Instance.GameTable.FindStage(x => x.ID == value2);
                    bool bmove = false;
                    if (stagetabledata.LimitStage == -1)
                        bmove = true;
                    else
                    {
                        var limitstagedata = GameInfo.Instance.StageClearList.Find(x => x.TableID == stagetabledata.LimitStage);
                        if (limitstagedata != null)
                            bmove = true;
                    }
                    if (!bmove)
                    {
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3063));
                        return;
                    }
                    else
                    {
                        UIValue.Instance.SetValue(UIValue.EParamType.StageID, value2);
                        strpopup = "StageDetailPopup";
                    }
                }
                LobbyUIManager.Instance.SetPanelType(panel);
            }
            else if (panel.Equals(ePANELTYPE.EVENT_STORY_STAGE))
            {
                int value2 = int.Parse(strvalue2);
                int value3 = int.Parse(strvalue3);

                EventSetData eventdata = GameInfo.Instance.GetEventSetData(value2);
                if (eventdata == null)
                {
                    MessageToastPopup.Show(FLocalizeString.Instance.GetText(3054));
                    return;
                }

                int state = GameSupport.GetJoinEventState(eventdata.TableID);
                if (state < (int)eEventState.EventNone)
                {
                    if (state == (int)eEventState.EventNotStart)
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3052));
                    else
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3053));
                    return;
                }
                
                var stagetabledata = GameInfo.Instance.GameTable.FindStage(x => x.ID == value2);
                bool bmove = false;
                if (stagetabledata.LimitStage == -1)
                    bmove = true;
                else
                {
                    var stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == stagetabledata.LimitStage);
                    if (stagecleardata != null)
                        bmove = true;
                }
                if (!bmove)
                {
                    MessageToastPopup.Show(FLocalizeString.Instance.GetText(3063));
                    return;
                }
                else
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.EventID, value2);
                    UIValue.Instance.SetValue(UIValue.EParamType.StageID, value3);
                    UIValue.Instance.SetValue(UIValue.EParamType.EventStageID, value3);
                    LobbyUIManager.Instance.SetPanelType(panel);
                    strpopup = "StageDetailPopup";
                }
            }
            else if (panel.Equals(ePANELTYPE.ARENATOWER))
            {
                if(IsLockArenaTower())
                {
                    return;
                }

                LobbyUIManager.Instance.SetPanelType(panel);
            }
            else if (panel.Equals(ePANELTYPE.ITEM))
            {
                int value2 = 0;
                if (!int.TryParse(strvalue2, out value2))
                    return;

                UIValue.Instance.SetValue(UIValue.EParamType.ItemTab, value2);
                LobbyUIManager.Instance.SetPanelType(panel);

                strpopup = strvalue3;
                if(strpopup.Equals("WeaponGradeUpPopup"))
                {
                    if (LobbyUIManager.Instance.GetActiveUI("WeaponInfoPopup") == null)
                        LobbyUIManager.Instance.ShowUI("WeaponInfoPopup", true);
                    else
                    {
                        LobbyUIManager.Instance.InitComponent("WeaponInfoPopup");
                        LobbyUIManager.Instance.Renewal("WeaponInfoPopup");
                    }
                }
            }
            else if (panel.Equals(ePANELTYPE.CHARINFO))
            {
                LobbyUIManager.Instance.SetPanelType(panel);
                strpopup = strvalue2;
            }
            else if (panel.Equals(ePANELTYPE.CARD_DISPATCH))
            {
                GameTable.CardDispatchSlot.Param data = GameInfo.Instance.GameTable.CardDispatchSlots.Find(x => x.Index == 1);
                if (data == null)
                {
                    return;
                }
                
                UserData userdata = GameInfo.Instance.UserData;
                if (userdata.Level < data.NeedRank)
                {
                    //지휘관 RANK가 부족합니다.
                    string msg = string.Format("{0}\n{1}", FLocalizeString.Instance.GetText(3205), string.Format(FLocalizeString.Instance.GetText(1365), data.NeedRank));
                    MessageToastPopup.Show(msg);
                    if (LobbyUIManager.Instance.LoginBonusStep != eLoginBonusStep.End)
                    {
                        LobbyUIManager.Instance.ShowDailyLoginPopup(false);
                    }
                    return;
                }
                
                LobbyUIManager.Instance.SetPanelType(panel);
            }
            else if( panel.Equals( ePANELTYPE.RAID ) ) {
                if( GameInfo.Instance.CharList.Count < 3 ) {
                    MessagePopup.OK( eTEXTID.OK, FLocalizeString.Instance.GetText( 3336 ), null );
                    return;
                }

                if( IsRaidEnd() ) {
                    MessagePopup.OK( eTEXTID.OK, 3332, null );
                    return;
                }

                LobbyUIManager.Instance.SetPanelType( ePANELTYPE.RAID_MAIN );
            }
            else
            {
                if (panel != ePANELTYPE.NULL)
                {
                    LobbyUIManager.Instance.SetPanelType(panel);
                }
            }

            if (isFacility == false)
            {
                if (LobbyUIManager.Instance.IsActiveUI("FacilityPanel") || LobbyUIManager.Instance.IsActiveUI("FacilityItemPanel"))
                {
                    if (LobbyUIManager.Instance.IsActiveUI("FacilityPanel"))
                        LobbyUIManager.Instance.HideUI("FacilityPanel");
                    else
                    {
                        LobbyUIManager.Instance.HideUI("FacilityItemPanel");
                        LobbyUIManager.Instance.HideUI("FacilityItemCombinePopup");
                    }
                        

                    Lobby.Instance.MoveToLobby();
                }

                if (strpopup != string.Empty)
                {
                    if (LobbyUIManager.Instance.GetActiveUI(strpopup) == null)
                        LobbyUIManager.Instance.ShowUI(strpopup, true);
                    else
                    {
                        LobbyUIManager.Instance.InitComponent(strpopup);
                        LobbyUIManager.Instance.Renewal(strpopup);
                    }
                }
                else
                    LobbyUIManager.Instance.HideAll(FComponent.TYPE.Popup, true);
            }
        }
        else if (functiontype == eUIMOVEFUNCTIONTYPE.UIPOPUP)
        {
            if( strvalue1.Equals( "DailyLoginBonusPopup" ) ) {
                LobbyUIManager.Instance.LoginBonusStep = eLoginBonusStep.Step01;
                LobbyUIManager.Instance.ShowDailyLoginPopup( false );
            }
            else if( strvalue1.Equals( "ArenaStorePopup" ) ) {
                if( GameInfo.Instance.UserData.Level < GameInfo.Instance.GameConfig.ArenaOpenRank ) {
                    MessageToastPopup.Show( string.Format( FLocalizeString.Instance.GetText( (int)eTEXTID.LOCKMSG ), GameInfo.Instance.GameConfig.ArenaOpenRank ) );
                    return;
                }
                int value2 = int.Parse(strvalue2) - 1; // 상점에서 탭+1(kMainTab.kSelectTab + 1) 처리를 하기 때문에 보내기 전 -1 처리
                UIValue.Instance.SetValue( UIValue.EParamType.ArenaStoreTab, value2 );
                LobbyUIManager.Instance.ShowUI( "ArenaStorePopup", true );
            }
            else if( strvalue1.Equals( "Pass" ) ) {
                UIPassMissionPopup popup = LobbyUIManager.Instance.GetUI<UIPassMissionPopup>("PassMissionPopup");
                if( popup != null ) {
                    int.TryParse( strvalue2, out int result );
                    popup.SetForcePassTab( result );
                    popup.SetUIActive( true );
                }
            }
            else if( strvalue1.Equals( "PackagePopup" ) ) {
                UIPackagePopup popup = LobbyUIManager.Instance.GetUI<UIPackagePopup>("PackagePopup");
                if( popup ) {
                    int.TryParse( strvalue2, out int result );

                    popup.PackageStoreID = result;
                    popup.SetUIActive( true );
                }
            }
			else if ( strvalue1.Equals( "RaidStorePopup" ) ) {
				if ( GameInfo.Instance.CharList.Count < 3 ) {
					MessagePopup.OK( eTEXTID.OK, FLocalizeString.Instance.GetText( 3336 ), null );
					return;
				}

				if ( GameSupport.IsRaidEnd() ) {
					MessagePopup.OK( eTEXTID.OK, 3332, null );
					return;
				}

				UIRaidStorePopup raidStorePopup = LobbyUIManager.Instance.GetUI<UIRaidStorePopup>( "RaidStorePopup" );
				raidStorePopup.ForceSelectTab( Utility.SafeIntParse( strvalue2 ) );
				raidStorePopup.SetUIActive( true );
			}
			else {
				LobbyUIManager.Instance.ShowUI( strvalue1, true );
			}
		}
        else if (functiontype == eUIMOVEFUNCTIONTYPE.WEBVIEW)
        {
            int value1 = int.Parse(strvalue1);
            GameSupport.OpenWebView(FLocalizeString.Instance.GetText(value1), strvalue2);
        }
    }

    public static void GoToFacility()
    {
        LobbyDoorPopup.Show(() => 
        {
            var obj = UIValue.Instance.GetValue(UIValue.EParamType.FacilityID);
            if (obj == null)
            {
                Lobby.Instance.MoveToLobby();
                LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN);
            }
            else
            {
                var facilitydata = GameInfo.Instance.GetFacilityData((int)obj);
                var list = GameInfo.Instance.FacilityList.FindAll(x => x.TableData.ParentsID == facilitydata.TableID);
                var data = GameInfo.Instance.FacilityList[(int)obj - 1];
                Lobby.Instance.MoveToFacility(data.TableID);

                if (list.Count.Equals(1))
                    LobbyUIManager.Instance.SetPanelType(ePANELTYPE.FACILITY);
                else
                    LobbyUIManager.Instance.SetPanelType(ePANELTYPE.FACILITYITEM);
            }
        });
    }

    public static void OpenRewardDataInfoPopup(RewardData reward)
    {
        if (reward.Type == (int)eREWARDTYPE.WEAPON)         //무기
        {
            WeaponData data = GameInfo.Instance.GetWeaponData(reward.UID);
            if (data != null)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, reward.UID);
                LobbyUIManager.Instance.ShowUI("WeaponInfoPopup", true);
            }
            else
            {
                var tabledata = GameInfo.Instance.GameTable.FindWeapon(reward.Index);
                if (tabledata != null)
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, (long)-1);
                    UIValue.Instance.SetValue(UIValue.EParamType.WeaponTableID, reward.Index);
                    LobbyUIManager.Instance.ShowUI("WeaponInfoPopup", true);
                }
            }
        }
        else if (reward.Type == (int)eREWARDTYPE.GEM)            //곡옥
        {
            GemData data = GameInfo.Instance.GetGemData(reward.UID);
            if (data != null)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.GemUID, reward.UID);
                LobbyUIManager.Instance.ShowUI("GemInfoPopup", true);
            }
            else
            {
                var tabledata = GameInfo.Instance.GameTable.FindGem(reward.Index);
                if (tabledata != null)
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.GemUID, (long)-1);
                    UIValue.Instance.SetValue(UIValue.EParamType.GemTableID, reward.Index);
                    LobbyUIManager.Instance.ShowUI("GemInfoPopup", true);
                }
            }
        }
        else if (reward.Type == (int)eREWARDTYPE.CARD)
        {
            CardData data = GameInfo.Instance.GetCardData(reward.UID);
            if (data != null)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.CardUID, reward.UID);
                LobbyUIManager.Instance.ShowUI("CardInfoPopup", true);
            }
            else
            {
                var tabledata = GameInfo.Instance.GameTable.FindCard(reward.Index);
                if (tabledata != null)
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.CardUID, (long)-1);
                    UIValue.Instance.SetValue(UIValue.EParamType.CardTableID, reward.Index);
                    LobbyUIManager.Instance.ShowUI("CardInfoPopup", true);
                }
            }

        }
        else if (reward.Type == (int)eREWARDTYPE.ITEM)
        {
            ItemData data = GameInfo.Instance.GetItemData(reward.UID);
            if (data != null && data.TableData.Type != (int)eITEMTYPE.EVENT)
            {
                if (IsPackageInfoPopupItem(data.TableData) == true)
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.ItemTableID, data.TableID);
                    UIValue.Instance.SetValue(UIValue.EParamType.ItemUID, data.ItemUID);
                    LobbyUIManager.Instance.ShowUI("ItemPackageInfoPopup", true);
                }
                else
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.ItemUID, reward.UID);
                    LobbyUIManager.Instance.ShowUI("ItemInfoPopup", true);
                }
            }
            else
            {
                var tabledata = GameInfo.Instance.GameTable.FindItem(reward.Index);
                bool isPackageInfo = tabledata != null && IsPackageInfoPopupItem(tabledata);
                if (isPackageInfo)
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.ItemTableID, tabledata.ID);
                    UIValue.Instance.SetValue(UIValue.EParamType.ItemUID, (long)-1);
                    LobbyUIManager.Instance.ShowUI("ItemPackageInfoPopup", true);
                }
                else
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.ItemUID, (long)-1);
                    UIValue.Instance.SetValue(UIValue.EParamType.ItemTableID, reward.Index);
                    UIValue.Instance.SetValue(UIValue.EParamType.ItemTableCount, reward.Value);
                    LobbyUIManager.Instance.ShowUI("ItemInfoPopup", true);
                }
            }
        }
    }

    public static void OpenRewardTableDataInfoPopup(RewardData reward)
    {
        if (reward.Type == (int)eREWARDTYPE.WEAPON)         //무기
        {
            var tabledata = GameInfo.Instance.GameTable.FindWeapon(reward.Index);
            if (tabledata != null)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, (long)-1);
                UIValue.Instance.SetValue(UIValue.EParamType.WeaponTableID, reward.Index);
                LobbyUIManager.Instance.ShowUI("WeaponInfoPopup", true);
            }
        }
        else if (reward.Type == (int)eREWARDTYPE.GEM)            //곡옥
        {
            var tabledata = GameInfo.Instance.GameTable.FindGem(reward.Index);
            if (tabledata != null)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.GemUID, (long)-1);
                UIValue.Instance.SetValue(UIValue.EParamType.GemTableID, reward.Index);
                LobbyUIManager.Instance.ShowUI("GemInfoPopup", true);
            }
        }
        else if (reward.Type == (int)eREWARDTYPE.CARD)
        {
            var tabledata = GameInfo.Instance.GameTable.FindCard(reward.Index);
            if (tabledata != null)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.CardUID, (long)-1);
                UIValue.Instance.SetValue(UIValue.EParamType.CardTableID, reward.Index);
                LobbyUIManager.Instance.ShowUI("CardInfoPopup", true);
            }
        }
        else if (reward.Type == (int)eREWARDTYPE.ITEM)
        {
            var tabledata = GameInfo.Instance.GameTable.FindItem(reward.Index);
            if (tabledata != null && tabledata.Type != (int)eITEMTYPE.EVENT)
            {
                if (IsPackageInfoPopupItem(tabledata) == true)
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.ItemTableID, tabledata.ID);
                    UIValue.Instance.SetValue(UIValue.EParamType.ItemUID, (long)-1);

                    FComponent ui = LobbyUIManager.Instance.GetActiveUI("ItemPackageInfoPopup");
                    if (ui == null)
                        LobbyUIManager.Instance.ShowUI("ItemPackageInfoPopup", true);
                    else
                    {
                        ui.PlayAnimtion(0);
                        LobbyUIManager.Instance.Renewal("ItemPackageInfoPopup");
                    }
                }
                else
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.ItemUID, (long)-1);
                    UIValue.Instance.SetValue(UIValue.EParamType.ItemTableID, reward.Index);
                    UIValue.Instance.SetValue(UIValue.EParamType.ItemTableCount, reward.Value);
                    LobbyUIManager.Instance.ShowUI("ItemInfoPopup", true);
                }
            }
        }
        else if(reward.Type == (int)eREWARDTYPE.COSTUME)
        {
            CharViewer.CreateCharPopup(reward);
        }
        else if (reward.Type == (int)eREWARDTYPE.CHAR)
        {
            var tabledata = GameInfo.Instance.GameTable.FindCharacter(reward.Index);
            if (tabledata != null)
            {
                RewardData rewardData = new RewardData(reward.Type, reward.Index, reward.Value);
                rewardData.Type = (int)eREWARDTYPE.COSTUME;
                rewardData.Index = tabledata.InitCostume;
                CharViewer.CreateCharPopup(rewardData);
            }
        }
    }

    public static bool IsPackageInfoPopupItem(GameTable.Item.Param tabledata)
    {
        // ItemPackageInfoPopup이 아닌 IteminfoPopup으로 출력할 아이템 체크 (=SellPrice가 0이면)
        // 일반적으로 아이템 아이콘이 존재하지 않는 특수한 보상을 지급하고자 할 때 사용
        if (tabledata.SellPrice == 0)
            return false;

        if (tabledata.Type == (int)eITEMTYPE.USE &&
            (tabledata.SubType == (int)eITEMSUBTYPE.USE_STORE_PACKAGE || tabledata.SubType == (int)eITEMSUBTYPE.USE_SELECT_ITEM
            || tabledata.SubType == (int)eITEMSUBTYPE.USE_RANDOM_ITEM || tabledata.SubType == (int)eITEMSUBTYPE.USE_PACKAGE_ITEM))
        {
            return true;
        }

        return false;
    }

    public static bool IsAbleSellItem(GameTable.Item.Param tabledata)
    {
        if (tabledata.Type == (int)eITEMTYPE.USE)
            return false;

        if (tabledata.SellPrice == 0)
            return false;

        return true;
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  비트 검사
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public static bool _IsOnBitIdx(uint _targetVal, int _flagIdx)
    {
        uint checkFlag = (uint)(0x00000001 << (int)_flagIdx);
        return (checkFlag == (_targetVal & checkFlag));
    }

    public static void _DoOnOffBitIdx(ref uint _targetVal, int _flagIdx, bool _on )
    {
        if (_on)
            _DoOnBitIdx(ref _targetVal, _flagIdx);
        else
            _DoOffBitIdx(ref _targetVal, _flagIdx);
    }
    public static void _DoOnBitIdx(ref uint _targetVal, int _flagIdx)
    {
        uint checkFlag = (uint)(0x00000001 << (int)_flagIdx);
        _targetVal |= checkFlag;
    }
    public static void _DoOffBitIdx(ref uint _targetVal, int _flagIdx)
    {
        uint checkFlag = (uint)(0x00000001 << (int)_flagIdx);
        _targetVal &= 0xFFFFFFFF ^ checkFlag;
    }


    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  각종 검사 (레드닷) 캐릭터
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public static bool CheckCharData(CharData chardata)
    {
        if (IsCharGradeUp(chardata))
            return true;
        if (IsCharPassiveSkillUp(chardata))
            return true;
        if (IsCharSkillUp(chardata))
            return true;
        if (IsCharOpenSkillSlot(chardata))
            return true;
        return false;
    }

    public static bool IsCharGradeUp(CharData chardata, bool bmat = true )//등급 가능 체크
    {
        if (!IsMaxGrade(chardata.Grade))
        {
            if (IsMaxCharLevel(chardata.Level, chardata.Grade))
            {
                if(bmat)
                {
                    var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == chardata.TableData.WakeReqGroup && x.Level == chardata.Grade);
                    if (IsItemReqList(reqdata))
                    {
                        return true; //등급 가능
                    }
                }
                else
                {
                    return true; //등급 가능
                }
                
            }
        }
        return false;
    }

    public static bool IsCharAwakenUp(CharData chardata, bool bmat = true)//각성 가능 체크
    {
        if (!IsMaxGrade(chardata.Grade))
        {
            if ((chardata.Level == GetCharMaxLevel(chardata.Grade)) &&
                (chardata.Grade >= GameInfo.Instance.GameConfig.CharStartAwakenGrade))
            {
                if (bmat)
                {
                    var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == chardata.TableData.WakeReqGroup && x.Level == chardata.Grade);
                    if (IsItemReqList(reqdata))
                    {
                        return true; //등급 가능
                    }
                }
                else
                {
                    return true; //등급 가능
                }
            }
        }
        return false;
    }

    public static bool IsCharPassiveSkillUp(CharData chardata, bool bmat = true )//패시브 스킬 업그레이드 가능
    {
        var upgradeskilllist = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.CharacterID == chardata.TableID && (x.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_NORMAL || x.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_ULTIMATE));
        for (int i = 0; i < upgradeskilllist.Count; i++)
        {
            int level = 0;
            var passviedata = chardata.PassvieList.Find(x => x.SkillID == upgradeskilllist[i].ID);
            if (passviedata != null)
                level = passviedata.SkillLevel;

            var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == upgradeskilllist[i].ItemReqListID && x.Level == level);
            if (reqdata != null)
            {
                if (chardata.Level >= reqdata.LimitLevel) // 레벨제한
                {
                    if (bmat)
                    {
                        if (chardata.PassviePoint >= reqdata.GoodsValue)
                            if (GameInfo.Instance.UserData.IsGoods(eGOODSTYPE.GOLD, reqdata.Gold))
                                return true;//스킬 레벨업 가능
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static bool IsCharSkillUp(CharData chardata, bool bmat = true) //습득 스킬
    {
        var seleteskilllist = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.CharacterID == chardata.TableID && (x.Type == (int)eCHARSKILLPASSIVETYPE.SELECT_NORMAL));
        for (int i = 0; i < seleteskilllist.Count; i++)
        {
            var passviedata = chardata.PassvieList.Find(x => x.SkillID == seleteskilllist[i].ID);
            if (passviedata == null)
            {
                var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == seleteskilllist[i].ItemReqListID);
                if (reqdata != null)
                {
                    if (chardata.Level >= reqdata.LimitLevel) // 레벨제한
                    {
                        if(bmat)
                        {
                            if (chardata.PassviePoint >= reqdata.GoodsValue)
                                if (GameInfo.Instance.UserData.IsGoods(eGOODSTYPE.GOLD, reqdata.Gold))
                                    return true;
                        }
                        else
                        {
                            return true;
                        }                        
                    }
                }
            }
        }
        return false;
    }

    public static bool IsCharOpenSkillSlot(CharData chardata) //스킬 슬롯 오픈
    {
        for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
        {
            if (chardata.EquipSkill[i] == (int)eCOUNT.NONE)
            {
                if (chardata.Level >= GameInfo.Instance.GameConfig.CharSkillSlotLimitLevel[i]) // 레벨제한
                {
                    if (PlayerPrefs.HasKey(string.Format("UserData_SlotEffect_{0}_{1}", chardata.TableData.ID.ToString(), i)))
                    {
                        return true;//슬롯 오픈
                    }
                }
            }
        }
        return false;
    }

    public static void IsCharOpenSkillSlot(CharData chardata, int oldLv, int nowLv) //스킬 슬롯 오픈
    {
        for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
        {
            if (oldLv < GameInfo.Instance.GameConfig.CharSkillSlotLimitLevel[i] && nowLv >= GameInfo.Instance.GameConfig.CharSkillSlotLimitLevel[i]) // 레벨제한
            {
                if (!PlayerPrefs.HasKey(string.Format("UserData_SlotEffect_{0}_{1}", chardata.TableData.ID.ToString(), i)))
                {
                    PlayerPrefs.SetInt(string.Format("UserData_SlotEffect_{0}_{1}", chardata.TableData.ID.ToString(), i), 1);
                }
            }
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  각종 검사 (레드닷) 서포터 관련
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public static bool CheckCardData(CardData carddata)
    {
        if (IsCardWakeUp(carddata))
            return true;
        if (IsCardSkillUp(carddata))
            return true;
        if (IsPossibleEnchant(carddata.TableData.EnchantGroup, carddata.EnchantLv))
            return true;

        return false;
    }

    public static bool IsCardWakeUp(CardData carddata, bool bmat = true)//각성
    {
        if (IsEquipAndUsingCardData(carddata.CardUID))
            return false;
        if (GetCardMaxWake(carddata) != 0)
        {
            if (IsMaxLevelCard(carddata))
            {
                if (!IsMaxWakeCard(carddata))
                {
                    if (bmat)
                    {
                        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == carddata.TableData.WakeReqGroup && x.Level == carddata.Wake);
                        if (reqdata != null)
                        {
                            if (IsItemReqList(reqdata))
                            {
                                return true;//각성 가능
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static bool IsCardSkillUp(CardData carddata, bool bitemcheck = false)//스킬레벨업
    {
        if (IsEquipAndUsingCardData(carddata.CardUID))
            return false;
        if (!IsMaxSkillLevelCard(carddata))
        {
            var list = GameInfo.Instance.CardList.FindAll(x => x.TableID == carddata.TableID && x.CardUID != carddata.CardUID && x.Lock == false);
            bool blevelup = false;
            int gold = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (GameInfo.Instance.GetEquiCardCharData(list[i].CardUID) != null)
                    continue;
                if (IsEquipAndUsingCardData(list[i].CardUID))
                    continue;
                blevelup = true;
                gold = GetCardSkillLevelUpCost(carddata, 1);
                break;
            }
            if (blevelup && gold != 0)
            {
                return true;//스킬레벨업 가능
            }
            if(bitemcheck)
            {
                var itemlist = GameInfo.Instance.ItemList.FindAll(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_CARD_SLVUP && x.TableData.Grade == carddata.TableData.Grade);
                if(itemlist.Count != 0)
                {
                    blevelup = true;
                    gold = GameSupport.GetCardSkillLevelUpCost(carddata, 1);
                }
                if (blevelup && gold != 0)
                {
                    return true;//스킬레벨업 가능
                }
            }
        }
        return false;
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  각종 검사 (레드닷) 무기 관련
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public static bool CheckWeaponData(WeaponData weapondata)
    {
        if (IsWeaponWakeUp(weapondata))
            return true;
        if (IsWeaponSkillUp(weapondata))
            return true;
        if (IsPossibleEnchant(weapondata.TableData.EnchantGroup, weapondata.EnchantLv))
            return true;
        return false;
    }

    public static bool IsWeaponWakeUp(WeaponData weapondata, bool bmat = true)
    {
        if (GameInfo.Instance.GetEquipWeaponFacilityData(weapondata.WeaponUID) != null)
            return false;
        if (GetWeaponMaxWake(weapondata) != 0) 
        {
            if (IsMaxLevelWeapon(weapondata))
            {
                if (!IsMaxWakeWeapon(weapondata))
                {
                    if (bmat)
                    {
                        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == weapondata.TableData.WakeReqGroup && x.Level == weapondata.Wake);
                        if (reqdata != null)
                        {
                            if (IsItemReqList(reqdata))
                            {
                                return true; //각성 가능
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static bool IsWeaponSkillUp(WeaponData weapondata, bool bitemcheck = false)//스킬레벨업
    {
        if (GameInfo.Instance.GetEquipWeaponFacilityData(weapondata.WeaponUID) != null)
            return false;

        if (GameSupport.GetEquipWeaponDepot(weapondata.WeaponUID))
            return false;

        if (!IsMaxSkillLevelWeapon(weapondata.SkillLv))
        {
            var list = GameInfo.Instance.WeaponList.FindAll(x => x.TableID == weapondata.TableID && x.WeaponUID != weapondata.WeaponUID && x.Lock == false);
            bool blevelup = false;
            int gold = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (GameInfo.Instance.GetEquipWeaponCharData(list[i].WeaponUID) != null)
                    continue;
                if (GameInfo.Instance.GetEquipWeaponFacilityData(list[i].WeaponUID) != null)
                    continue;
                if (GameSupport.GetEquipWeaponDepot(list[i].WeaponUID))
                    continue;

                blevelup = true;
                gold = GetWeaponSkillLevelUpCost(weapondata, 1);
                break;
            }
            if (blevelup && gold != 0)
            {
                return true;//스킬레벨업 가능
            }
            if (bitemcheck)
            {
                var itemlist = GameInfo.Instance.ItemList.FindAll(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_WEAPON_SLVUP && x.TableData.Grade == weapondata.TableData.Grade);
                if (itemlist.Count != 0)
                {
                    blevelup = true;
                    gold = GameSupport.GetWeaponSkillLevelUpCost(weapondata, 1);
                }
                if (blevelup && gold != 0)
                {
                    return true;//스킬레벨업 가능
                }
            }
        }
        return false;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  각종 검사 (레드닷) 시설 관련
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public static bool IsFacilityActive(FacilityData facilitydata)
    {
        if (facilitydata.Level == 0)
        {
            if (GameInfo.Instance.UserData.Level >= facilitydata.TableData.FacilityOpenUserRank)
            {
                var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == facilitydata.TableData.LevelUpItemReq && x.Level == facilitydata.Level);
                if (reqdata != null)
                {
                    if (IsItemReqList(reqdata))
                    {
                        return true; //활성 가능
                    }
                }
            }
        }
        return false;

    }
    public static bool IsFacilityLevelUp(FacilityData facilitydata)
    {
        if (facilitydata.Level >= facilitydata.TableData.MaxLevel)
        {
            return false;
        }
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == facilitydata.TableData.LevelUpItemReq && x.Level == facilitydata.Level);
        if (reqdata != null)
        {
            if (IsItemReqList(reqdata))
            {
                return true; //레벨업 가능
            }
        }
        return false;
    }
    public static bool IsFacilityComplete(FacilityData facilitydata)
    {
        if (facilitydata.Stats == (int)eFACILITYSTATS.WAIT)
            return false;
        var diffTime = facilitydata.RemainTime - GameSupport.GetCurrentServerTime();
        if (diffTime.Ticks <= 0)     //완료
        {
            return true;
        }
        return false;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  (레드닷) 업적 관련
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public static bool IsAchievementComplete(AchieveData achievedata)
    {
		if (achievedata.TableData == null)
		{
			Debug.LogError(achievedata.GroupID + "번 업적 데이터가 없습니다.");
			return false;
		}

		bool brewardcomplete = achievedata.bTotalComplete;

        int NowValue = achievedata.Value;
        int MaxValue = achievedata.TableData.AchieveValue;

        GetAchievementNowMaxValue(achievedata, ref NowValue, ref MaxValue);

        if (!brewardcomplete)
        {
            if (NowValue >= MaxValue)
            {
                return true;
            }
        }
        return false;
    }

    public static void GetAchievementNowMaxValue(AchieveData achievedata, ref int nowValue, ref int maxValue)
    {
        if (achievedata.TableData == null)
        {
            return;
        }

        GetAchieveNowMaxValue(achievedata.TableData.AchieveType, achievedata.TableData.AchieveIndex, ref nowValue, ref maxValue);
    }

    public static eAchieveEventType IsAchieveEventComplete(AchieveEventData achieveEventData)
    {
        if (achieveEventData.TableData == null)
        {
            return eAchieveEventType.Complete;
        }

        int nowValue = achieveEventData.Value;
        int maxValue = achieveEventData.TableData.AchieveValue;
        GetAchieveEventNowMaxValue(achieveEventData, ref nowValue, ref maxValue);

        if (nowValue >= maxValue)
        {
            return eAchieveEventType.Reward;
        }

        return eAchieveEventType.Ing;
    }

    public static void GetAchieveEventNowMaxValue(AchieveEventData achieveEventData, ref int nowValue, ref int maxValue)
    {
        if (achieveEventData.TableData == null)
        {
            return;
        }

        GetAchieveNowMaxValue(achieveEventData.TableData.AchieveType, achieveEventData.TableData.AchieveIndex, ref nowValue, ref maxValue);
    }

    public static void GetAchieveNowMaxValue(string achieveType, int achieveIndex, ref int nowValue, ref int maxValue)
    {
        if (achieveType == "AM_Adv_StoryClear")
        {
            nowValue = 0;
            maxValue = 1;

            StageClearData stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == achieveIndex);
            if (stagecleardata != null)
            {
                nowValue = 1;
            }
        }
        else if (achieveType == "AM_Gro_CharLvCount")
        {
            nowValue = GameInfo.Instance.CharList.FindAll(x => x.Level >= achieveIndex).Count;
        }
        else if (achieveType == "AM_Gro_UserRank")
        {
            nowValue = GameInfo.Instance.UserData.Level;
        }
        else if (achieveType == "AM_Gro_CharLv")
        {
            CharData chardata = GameInfo.Instance.GetCharDataByTableID(achieveIndex);
            if (chardata != null)
            {
                nowValue = chardata.Level;
            }
        }
        else if (achieveType == "AM_Gro_CharGrade")
        {
            CharData chardata = GameInfo.Instance.GetCharDataByTableID(achieveIndex);
            if (chardata != null)
            {
                nowValue = chardata.Grade;
            }
        }
        else if( achieveType == "AM_Adv_RaidClearLevel" ) {
            List<RaidClearData> find = GameInfo.Instance.RaidUserData.StageClearList.FindAll( x => x.StageTableID == achieveIndex );

            nowValue = 0;
            for( int i = 0; i < find.Count; i++ ) {
                if( find[i].Step > nowValue ) {
                    nowValue = find[i].Step;
				}
			}//
		}
    }

    //상점 결제 동의 처리--------------------------------------------------------------------------------------------------------------------------------
    public static void PaymentAgreement_Cash()
    {
        bool b = PlayerPrefs.HasKey(SAVETYPE.ADULTCERTIFICATION.ToString());

        if (AppMgr.Instance.configData.m_ServiceType != Config.eServiceType.Japan)
            b = true;

        if(b)
        {
            if (LobbyUIManager.Instance.GetActiveUI("ApBpRecoverySelectPopup") != null)
            {
                LobbyUIManager.Instance.HideUI("ApBpRecoverySelectPopup", true);
            }
            LobbyUIManager.Instance.ShowUI("CashBuyPopup", true);
        }
        else
        {
            MessagePopup.CYNCheck(FLocalizeString.Instance.GetText(1195), FLocalizeString.Instance.GetText(1196), FLocalizeString.Instance.GetText(1197), FLocalizeString.Instance.GetText(1198), OnMsg_PaymentAgreement_Cash_Yes, null);
        }
    }

    public static void OnMsg_PaymentAgreement_Cash_Yes()
    {
        if (LobbyUIManager.Instance.GetActiveUI("ApBpRecoverySelectPopup") != null)
        {
            LobbyUIManager.Instance.HideUI("ApBpRecoverySelectPopup", true);
        }

        LobbyUIManager.Instance.ShowUI("CashBuyPopup", true);

        UIMessagePopup popup = MessagePopup.GetMessagePopup();
        if(popup)
        {
            if( popup.kCheckToggle.kSelect == 0 )
                PlayerPrefs.SetInt(SAVETYPE.ADULTCERTIFICATION.ToString(), 1);
        }
    }

    public static void PaymentAgreement_Package(int packageStoreId = 0, string uiName = "PackagePopup")
    {
        UIPackagePopup packagePopup = LobbyUIManager.Instance.GetUI<UIPackagePopup>("PackagePopup");
        packagePopup.PackageStoreID = packageStoreId;

        bool b = PlayerPrefs.HasKey(SAVETYPE.ADULTCERTIFICATION.ToString());
        if (AppMgr.Instance.configData.m_ServiceType != Config.eServiceType.Japan)
            b = true;

        if (b)
        {
            LobbyUIManager.Instance.ShowUI(uiName, true);
        }
        else
        {
            MessagePopup.CYNCheck(FLocalizeString.Instance.GetText(1195), FLocalizeString.Instance.GetText(1196), FLocalizeString.Instance.GetText(1197), FLocalizeString.Instance.GetText(1198), OnMsg_PaymentAgreement_Package_Yes, null);
        }
    }

    public static void OnMsg_PaymentAgreement_Package_Yes()
    {
        LobbyUIManager.Instance.ShowUI("PackagePopup", true);

        UIMessagePopup popup = MessagePopup.GetMessagePopup();
        if (popup)
        {
            if (popup.kCheckToggle.kSelect == 0)
                PlayerPrefs.SetInt(SAVETYPE.ADULTCERTIFICATION.ToString(), 1);
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  특수문자 검사 - 특수문자가 포함되어있다면 TRUE를 반환
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public static bool CheckSpecialText(string txt)
    {
        string str = @"[~!@\#$%^&*\()\=+|\\/:;?""<>'\s]";
        System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(str);
        return rex.IsMatch(txt);
    }

    //-----------------------------------------------------------------------
    // 정규식 검사
    // ID, Password
    // 영문,숫자만 허용
    // 6~14글자
    //-----------------------------------------------------------------------
    public static bool CheckRegularExpressions(string inputStr)
    {
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^[0-9a-zA-Z]{6,14}$");
        return regex.IsMatch(inputStr);
    }

    //상점 관련 처리--------------------------------------------------------------------------------------------------------------------------------
    public static bool IsStoreGoodsTimeExpire(GameClientTable.StoreDisplayGoods.Param param, DateTime curTime)
    {
        int chkNumeric = 0;
        if (int.TryParse(param.HideConType, out chkNumeric))
        {
            System.TimeSpan ts = GameSupport.GetRemainTime(GameSupport.GetTimeWithString(param.HideConType, true), curTime);
            if (ts.TotalSeconds > 0)
                return true;
            else
                return false;
        }

        return true;
    }

    public static bool IsStoreGoodsTimeExpireByGachaCategoryData(GameClientTable.StoreDisplayGoods.Param param, DateTime curTime)
    {
        foreach (GachaCategoryData data in GameInfo.Instance.ServerData.GachaCategoryList)
        {
            if (data == null) continue;

            System.TimeSpan tsStart = GetRemainTime(data.StartDate, curTime);
            System.TimeSpan tsEnd = GetRemainTime(data.EndDate, curTime);

            if (tsStart.TotalSeconds >= 0)
                continue;

            if (tsEnd.TotalSeconds <= 0)
                continue;

            if (data.StoreID1 == param.StoreID || data.StoreID2 == param.StoreID || data.StoreID3 == param.StoreID || data.StoreID4 == param.StoreID)
            {
                return true;
            }
        }

        return false;
    }



    public static GachaCategoryData GetGachaCategoryData(int StoreID)
    {
        for (int i = 0; i < GameInfo.Instance.ServerData.GachaCategoryList.Count; i++)
        {
            GachaCategoryData data = GameInfo.Instance.ServerData.GachaCategoryList[i];
            if (data == null) continue;

            if (data.StoreID1 == StoreID || data.StoreID2 == StoreID || data.StoreID3 == StoreID || data.StoreID4 == StoreID)
            {
                return data;
            }
        }
        return null;
    }

    public static int GetStoreDiscountRate(int storeid )
    {
        var storesaledata = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == storeid);
        if (storesaledata == null)
            return 0;

        return storesaledata.DiscountRate;
    }

    public static int GetDiscountPrice(int originValue, StoreSaleData storeSaleData)
    {
        if (storeSaleData != null && storeSaleData.DiscountRate > 0)
        {
            originValue = (int)Mathf.Ceil((float)originValue * (1f - ((float)storeSaleData.DiscountRate * 0.01f)));

            if (originValue < 1 && storeSaleData.DiscountRate < 100)
                originValue = 1;
        }

        return originValue;
    }

    //패키지 구매여부
    public static bool IsHaveStoreData(int storeid)
    {
        StoreData storedata = GameInfo.Instance.GetStoreData(storeid);
        if (storedata == null)
            return false;       //구매 안함

        if (storedata.TypeVal <= (int)eCOUNT.NONE)
            return false;

        return true;            //구매 했음
    }

    //남은 구매 횟수
    public static int GetLimitedCnt(int storeID)
    {
        int result = 0;
        StoreSaleData storeSaleData = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == storeID);
        if (storeSaleData == null)
            return result;

        if (storeSaleData.LimitType == (int)eStoreSaleKind.LimitDate_Day || storeSaleData.LimitType == (int)eStoreSaleKind.LimitDate_Weekly ||
            storeSaleData.LimitType == (int)eStoreSaleKind.LimitDate_Monthly)
        {
            StoreData storedata = GameInfo.Instance.GetStoreData(storeID);
            if (storedata == null)
            {
                result = storeSaleData.LimitValue;
            }
            else
            {
                System.DateTime remainTime = storedata.GetResetTime();
                var diffTime = remainTime - GameInfo.Instance.GetNetworkTime();
                if (diffTime.Ticks < 0)
                {
                    result = storeSaleData.LimitValue;
                }
                else
                {
                    result = storeSaleData.LimitValue - (int)storedata.TypeVal;
                }
            }
            

            return result;
        }
        else
        {
            StoreData storedata = GameInfo.Instance.GetStoreData(storeID);
            if (storedata == null)
            {
                return storeSaleData.LimitValue;
            }

            result = storeSaleData.LimitValue - (int)storedata.TypeVal;

            return result;
        }

        return result;
    }

    public static bool IsStoreSaleApply(int storeid)
    {
        var storesaledata = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == storeid);
        if (storesaledata == null)
            return false;

        var storemydata = GameInfo.Instance.GetStoreData(storeid);
        if (storemydata == null)
        {
            return true;
        }

        if (storesaledata.LimitType == (int)eStoreSaleKind.CycleMinute)
        {
            System.DateTime remaintime = storemydata.GetTime(); //new System.DateTime(storemydata.TypeVal);
            var diffTime = remaintime - GameSupport.GetCurrentServerTime();
            if (diffTime.Ticks < 0)
            {
                return true;
            }
        }
        else if (storesaledata.LimitType == (int)eStoreSaleKind.LimitDate_Day || storesaledata.LimitType == (int)eStoreSaleKind.LimitDate_Weekly ||
            storesaledata.LimitType == (int)eStoreSaleKind.LimitDate_Monthly)
        {
            System.DateTime remainTime = storemydata.GetResetTime();
            var diffTime = remainTime - GameInfo.Instance.GetNetworkTime();
            if (diffTime.Ticks < 0 || storemydata.TypeVal < storesaledata.LimitValue)
            {
                return true;
            }
        }
        else
        {
            if (storemydata.TypeVal < storesaledata.LimitValue)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsStoreSaleCheck(int oldstoreid, int storeid)
    {
        var storesaledata = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == storeid);
        if (storesaledata == null)
            return false;

        var storemydata = GameInfo.Instance.GetStoreData(storeid);
        if (storemydata == null)
        {
            StoreSaleData oldSaleData = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == oldstoreid);
            if (oldSaleData != null)
            {
                if (oldSaleData.LimitType != (int)eStoreSaleKind.CycleMinute)
                {
                    StoreData oldStoreData = GameInfo.Instance.GetStoreData(oldstoreid);
                    if (oldStoreData != null)
                    {
                        if ((int)oldStoreData.TypeVal < storesaledata.LimitValue)
                            return true;
                        else
                            return false;
                    }
                    else
                        return true;
                }
            }
            else
                return true;
        }

        if (storesaledata.LimitType == (int)eStoreSaleKind.CycleMinute)
        {
            System.DateTime remaintime = storemydata.GetTime(); //new System.DateTime(storemydata.TypeVal);
            var diffTime = remaintime - GameSupport.GetCurrentServerTime();
            if (diffTime.Ticks < 0)
            {
                return true;
            }
        }
        else
        {
            int addOldCnt = (int)eCOUNT.NONE;
            StoreSaleData oldSaleData = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == oldstoreid);
            if (oldSaleData != null)
            {
                if (oldSaleData.LimitType != (int)eStoreSaleKind.CycleMinute)
                {
                    StoreData oldStoreData = GameInfo.Instance.GetStoreData(oldstoreid);
                    if (oldStoreData != null)
                        addOldCnt = (int)oldStoreData.TypeVal;
                }
                else
                {
                    addOldCnt = oldSaleData.LimitValue;
                }                    
            }

            if (storeid == oldstoreid)
            {
                addOldCnt = (int)eCOUNT.NONE;
            }
            
            if (storemydata.TypeVal + addOldCnt < storesaledata.LimitValue)
            {
                return true;
            }
        }
        return false;
    }

	public static bool IsShowStoreDisplay( GameClientTable.StoreDisplayGoods.Param displaydata ) {
		if ( displaydata == null ) {
			return false;
		}

		if ( displaydata.ShowHaveStoreID != 0 ) {
			StoreData storemydata = GameInfo.Instance.GetStoreData( displaydata.ShowHaveStoreID );
            if ( storemydata == null ) {
                return false;
            }
		}
		if ( displaydata.HideHaveStoreID != 0 ) {
            StoreData storemyfastdata = GameInfo.Instance.GetStoreData( displaydata.HideHaveStoreID );
			if ( storemyfastdata != null ) {
				return IsStoreCountFlag( displaydata.StoreID );
			}
		}

		if ( displaydata.HideConType == "HCT_RankLess" ) {
            if ( GameInfo.Instance.UserData == null || GameInfo.Instance.UserData.Level < displaydata.HideConValue ) {
                return false;
            }
		}
		else if ( displaydata.HideConType == "HCT_HaveChar" )     //캐릭터 보유 여부
		{
			CharData chardata = GameInfo.Instance.GetCharDataByTableID( displaydata.HideConValue );
            if ( chardata != null ) {
                return false;
            }
		}
		else if ( displaydata.HideConType == "HCT_HaveUserMark" )     //유저마크 보유 여부
		{
            if ( GameInfo.Instance.IsUserMark( displaydata.HideConValue ) ) {
                return false;
            }
		}
		else if ( displaydata.HideConType == "HCT_HaveCostume " )     //코스튬 보유여부
		{
            if ( GameInfo.Instance.CostumeList == null || GameInfo.Instance.CostumeList.Contains( displaydata.HideConValue ) ) {
                return false;
            }
		}

		return IsStoreCountFlag( displaydata.StoreID );
	}

	//운영툴 할인상품목록 조건으로 체크(시간, 수량) 체크
	public static bool IsStoreCountFlag(int storeID)
    {
        GameTable.Store.Param storeParam = GameInfo.Instance.GameTable.FindStore(x => x.ID == storeID);
        if (storeParam != null)
        {
            if (IsExclusiveSaleOrSeason(storeParam.SaleType))
            {
                if (IsStoreSaleApply(storeParam.ID) == false)
                    return false;
            }
        }

        return true;
    }

    public static bool IsExclusiveSaleOrSeason(int saleType)
    {
        return (1 == saleType) ? true : false;
    }

    public static bool IsGuerrillaCampaign(GuerrillaCampData data)
    {
        if (data.Type == "GC_WeeklyMissionSet_Assign")
            return false;

        DateTime nowTime = GameSupport.GetCurrentServerTime();
        if (data.StartDate > nowTime)   //시작 전
            return false;
        if (data.EndDate < nowTime)     //종료 이후
            return false;

        return true;  
    }

    //false면 보임, true일때만 안보임
    public static bool IsUnVisibleGuerrillaMission(GuerrillaMissionData data, bool allMissionCheck = false)
    {
        if (allMissionCheck)
            return false;

        //가챠는 항상 있기때문에 제외(201215)
        if (data.Type == "GM_LoginBonus" || data.Type == "GM_MailReward" || data.Type == "GM_BuyStoreGacha_Cnt")
            return true;

        return false;
    }

    //게릴라 미션 완료, 시간 체크
    public static bool IsGuerrillaMission(GuerrillaMissionData data, bool allMissionCheck = false)
    {
        if (IsUnVisibleGuerrillaMission(data, allMissionCheck))
            return false;

        GllaMissionData userMissionData = GameInfo.Instance.GllaMissionList.Find(x => x.GroupID == data.GroupID);
        if(userMissionData != null)
        {
            if (userMissionData.Step > data.GroupOrder)
                return false;
        }

        DateTime nowTime = GameSupport.GetCurrentServerTime();
        if (data.StartDate > nowTime)       //시작 전
            return false;
        if (data.EndDate < nowTime)         //종료 이후
            return false;

        return true;
    }

    //게릴라 미션 시간 체크
    public static bool IsGuerrillaMissionTimeCheck(GuerrillaMissionData data)
    {
        DateTime nowTime = GameSupport.GetCurrentServerTime();
        if (data.StartDate > nowTime)
            return false;
        if (data.EndDate < nowTime)
            return false;

        return true;
    }

    //게릴라 미션 완료 체크
    public static bool IsGuerrillaMissionComplete(GuerrillaMissionData data)
    {
        if (IsUnVisibleGuerrillaMission(data))
            return false;

        GllaMissionData userMissionData = GameInfo.Instance.GllaMissionList.Find(x => x.GroupID == data.GroupID);
        if (userMissionData != null)
        {
            if (userMissionData.Count >= data.Count)
            {
                //완료한 미션 체크
                if (userMissionData.Step > data.GroupOrder)
                    return false;

                return true;
            }
        }

        return false;
    }

    public static void GetGuerrillaMissionListWithTimeByGroupId(ref List<GuerrillaMissionData> data, int groupID)
    {
        DateTime nowTime = GameSupport.GetCurrentServerTime();

        List<GuerrillaMissionData> missionList = GameInfo.Instance.ServerData.GuerrillaMissionList.FindAll(x => x.GroupID == groupID);
        for (int i = 0; i < missionList.Count; i++)
        {
            if (missionList[i].StartDate > nowTime)   //시작 전
                continue;
            if (missionList[i].EndDate < nowTime)     //종료 이후
                continue;

            data.Add(missionList[i]);           
        }
    }

    public static void GetGuerrillaMissionListWithTimeByType(ref List<GuerrillaMissionData> data, string type)
    {
        DateTime nowTime = GameSupport.GetCurrentServerTime();

        List<GuerrillaMissionData> missionList = GameInfo.Instance.ServerData.GuerrillaMissionList.FindAll(x => x.Type == type);
        for (int i = 0; i < missionList.Count; i++)
        {
            if (missionList[i].StartDate > nowTime)   //시작 전
                continue;
            if (missionList[i].EndDate < nowTime)     //종료 이후
                continue;

            data.Add(missionList[i]);
        }
    }

    public static eGuerrillaCampaignType GetGuerrillaCampaignType(string subject)
    {
		string[] strs = Enum.GetNames(typeof(eGuerrillaCampaignType));
		for(int i = 0; i < strs.Length; ++i)
		{
			if(strs[i].CompareTo(subject) == 0)
			{
				return (eGuerrillaCampaignType)i;
			}
		}
		/*
        foreach (eGuerrillaCampaignType type in (eGuerrillaCampaignType[])System.Enum.GetValues(typeof(eGuerrillaCampaignType)))
        {
            if (type.ToString() == subject)
                return type;
        }
		*/
        return eGuerrillaCampaignType.None;
    }

    public static string GetGuerrillaCampaignSprName(string data)
    {
        string strsprname = "ico_CampaignText0";
        if (data == "GC_StageClear_ExpRateUP")//경험치 획득량 증가
            strsprname = "ico_CampaignText1";
        else if (data == "GC_StageClear_GoldRateUP")//골드 획득량 증가
            strsprname = "ico_CampaignText2";
        else if (data == "GC_StageClear_ItemCntUP")//아이템 획득수 증가
            strsprname = "ico_CampaignText3";
        else if (data == "GC_StageClear_APRateDown")//AP 소모량 감소
            strsprname = "ico_CampaignText4";
        else if (data == "GC_StageClear_FavorRateUP")//서포터 호감도 획득량 증가
            strsprname = "ico_CampaignText5";
        else if (data == "GC_Upgrade_ExpRateUP")//강화 시 경험치 획득량 증가
            strsprname = "ico_CampaignText1";
        else if (data == "GC_Upgrade_PriceRateDown")//강화 시 골드 소모량 감소
            strsprname = "ico_CampaignText6";
        else if (data == "GC_Upgrade_SucNorRateDown")//강화 시 일반 성공 확률 감소
            strsprname = "ico_CampaignText7";
        else if (data == "GC_ItemSell_PriceRateUp")//골드 획득량 증가
            strsprname = "ico_CampaignText2";
        else if (data == "GC_Arena_CoinRateUP")//아레나 코인 획득량 증가
            strsprname = "ico_BattleCoinGetUp_s";
        else if (data == "GC_Rotation_OpenCashSale")    //로테이션 가챠 오픈 비용 감소
            strsprname = "ico_CampaignText8";
        else if (data == "GC_Stage_Multiple")           //스테이지 배수 플래그
            strsprname = "ico_CampaignText9";
        else if( data == "GC_Gacha_DPUP" ) {        // 염원의 기운 획득량 증가
            strsprname = "ico_CampaignText10";
		}
        return strsprname;
    }

    public static GuerrillaCampData GetOnGuerrillaCampaignType(eGuerrillaCampaignType type, int condi)
    {
        for (int i = 0; i < GameInfo.Instance.ServerData.GuerrillaCampList.Count; i++)
        {
            if (GameInfo.Instance.ServerData.GuerrillaCampList[i].Type == type.ToString() && (GameInfo.Instance.ServerData.GuerrillaCampList[i].Condition == condi || GameInfo.Instance.ServerData.GuerrillaCampList[i].Condition == (int)eCOUNT.NONE))
            {
                if (IsGuerrillaCampaign(GameInfo.Instance.ServerData.GuerrillaCampList[i]))
                    return GameInfo.Instance.ServerData.GuerrillaCampList[i];
            }
        }

        return null;
    }

    public static int GetGuerrillaCampaignDataCount( eGuerrillaCampaignType type, int condi ) {
        int count = 0;

        for ( int i = 0; i < GameInfo.Instance.ServerData.GuerrillaCampList.Count; i++ ) {

            if ( GameInfo.Instance.ServerData.GuerrillaCampList[i].Type == type.ToString() && 
                 ( GameInfo.Instance.ServerData.GuerrillaCampList[i].Condition == condi || 
                   GameInfo.Instance.ServerData.GuerrillaCampList[i].Condition == (int)eCOUNT.NONE ) ) {

                if ( IsGuerrillaCampaign( GameInfo.Instance.ServerData.GuerrillaCampList[i] ) ) {
                    ++count;
                }
            }
        }

        return count;
    }

    public static bool IsShowGameStroyUI()
    {
        UIStroyPopup uiStoryPopup = GameUIManager.Instance.GetActiveUI<UIStroyPopup>("StroyPopup");
        UIStoryCommunicationPopup uiStoryCommunicationPopup = GameUIManager.Instance.GetActiveUI<UIStoryCommunicationPopup>("StoryCommunicationPopup");
        if (uiStoryPopup || uiStoryCommunicationPopup)
        {
            return true;
        }
        return false;
    }


    public static bool IsShowStoryStage(GameTable.Stage.Param tabledata)
    {
        bool b = false;
        if (!string.IsNullOrEmpty(tabledata.ScenarioDrt_Start))
            b = true;
        else if (!string.IsNullOrEmpty(tabledata.ScenarioDrt_BossAppear))
            b = true;
        else if (!string.IsNullOrEmpty(tabledata.ScenarioDrt_BossDie))
            b = true;
        else if (!string.IsNullOrEmpty(tabledata.ScenarioDrt_EndMission))
            b = true;
        else if (!string.IsNullOrEmpty(tabledata.ScenarioDrt_AfterEndMission))
            b = true;
        return b;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  캐릭터 스킬
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public static bool IsCharSkillCond( CharData chardata, GameTable.CharacterSkillPassive.Param tabledata )
    {
        int type = tabledata.CondType;// % 3;
        if ( tabledata.CondType == (int)eCHARSKILLCONDITION.ASSIST_CARD_CNT || tabledata.CondType == (int)eCHARSKILLCONDITION.PROTECT_CARD_CNT || tabledata.CondType == (int)eCHARSKILLCONDITION.DOMINATE_CARD_CNT)
        {
            int count = chardata.GetEquipCardTypeCount(type);
            if (count >= tabledata.CondValue)
                return true;
        }
        else if (tabledata.CondType == (int)eCHARSKILLCONDITION.MAIN_ASSIST_CARD || tabledata.CondType == (int)eCHARSKILLCONDITION.MAIN_PROTECT_CARD || tabledata.CondType == (int)eCHARSKILLCONDITION.MAIN_DOMINATE_CARD)
        {
            if (chardata.IsEquipCardTypeIndex((int)eCARDSLOT.SLOT_MAIN, tabledata.CondType - ((int)eCARDTYPE.COUNT - 1)))
                return true;
        }
        else if (tabledata.CondType == (int)eCHARSKILLCONDITION.ANY_CARD_CNT)
        {
            int count = chardata.GetEquipCardCount();
            if (count >= tabledata.CondValue)
                return true;
        }
        return false;         
    }

    public static bool IsCharSkillCondWithTableData(CharData chardata, List<CardData> listEquipCardData, GameTable.CharacterSkillPassive.Param tabledata)
    {
        int type = tabledata.CondType;// % 3;
        if (tabledata.CondType == (int)eCHARSKILLCONDITION.ASSIST_CARD_CNT || tabledata.CondType == (int)eCHARSKILLCONDITION.PROTECT_CARD_CNT || tabledata.CondType == (int)eCHARSKILLCONDITION.DOMINATE_CARD_CNT)
        {
            int count = chardata.GetEquipCardTypeCountWithTableData(listEquipCardData, type);
            if (count >= tabledata.CondValue)
                return true;
        }
        else if (tabledata.CondType == (int)eCHARSKILLCONDITION.MAIN_ASSIST_CARD || tabledata.CondType == (int)eCHARSKILLCONDITION.MAIN_PROTECT_CARD || tabledata.CondType == (int)eCHARSKILLCONDITION.MAIN_DOMINATE_CARD)
        {
            if (chardata.IsEquipCardTypeIndexWithTableData(listEquipCardData[(int)eCARDSLOT.SLOT_MAIN], tabledata.CondType - ((int)eCARDTYPE.COUNT - 1)))
                return true;
        }
        else if (tabledata.CondType == (int)eCHARSKILLCONDITION.ANY_CARD_CNT)
        {
            int count = chardata.GetEquipCardCountWithTableData();
            if (count >= tabledata.CondValue)
                return true;
        }
        return false;
    }

    // 지원부대 버프에 따른 Hp 증가 계산
    public static float GetAddHpFromCardFormations() 
    {
        float addHpRate = 0.0f;

        for (int i = 0; i < GameInfo.Instance.GameTable.CardFormations.Count; i++)
        {
            GameTable.CardFormation.Param param = GameInfo.Instance.GameTable.CardFormations[i];

            int hasSupporterCount = 0;
            int maxLevelSupporterCount = 0;
            int maxFavorSupporterCount = 0;

            CardBookData cardBookData1 = GameInfo.Instance.GetCardBookData(param.CardID1);
            if(cardBookData1 != null)
            {
                ++hasSupporterCount;

                if(cardBookData1.IsOnFlag(eBookStateFlag.MAX_WAKE_AND_LV))
                {
                    ++maxLevelSupporterCount;
                }

                if(cardBookData1.IsOnFlag(eBookStateFlag.MAX_FAVOR_LV))
                {
                    ++maxFavorSupporterCount;
                }
            }

            CardBookData cardBookData2 = GameInfo.Instance.GetCardBookData(param.CardID2);
            if(cardBookData2 != null || (cardBookData2 == null && param.CardID2 == 0))
            {
                ++hasSupporterCount;

                if (cardBookData2 != null && cardBookData2.IsOnFlag(eBookStateFlag.MAX_WAKE_AND_LV))
                {
                    ++maxLevelSupporterCount;
                }

                if (cardBookData2 != null && cardBookData2.IsOnFlag(eBookStateFlag.MAX_FAVOR_LV))
                {
                    ++maxFavorSupporterCount;
                }
            }

            CardBookData cardBookData3 = GameInfo.Instance.GetCardBookData(param.CardID3);
            if (cardBookData3 != null || (cardBookData3 == null && param.CardID3 == 0))
            {
                ++hasSupporterCount;

                if (cardBookData3 != null && cardBookData3.IsOnFlag(eBookStateFlag.MAX_WAKE_AND_LV))
                {
                    ++maxLevelSupporterCount;
                }

                if (cardBookData3 != null && cardBookData3.IsOnFlag(eBookStateFlag.MAX_FAVOR_LV))
                {
                    ++maxFavorSupporterCount;
                }
            }

            if(hasSupporterCount >= 3)
            {
                addHpRate += param.GetHP;
            }

            if(maxLevelSupporterCount >= 3)
            {
                addHpRate += param.LevelHP;
            }

            if(maxFavorSupporterCount >= 3)
            {
                addHpRate += param.FavorHP;
            }
        }

        return addHpRate;
    }

    // 장착 중인 아레나 타워 문양
    public static List<BadgeData> GetEquipTowerBadgeList()
    {
        List<BadgeData> equipList = new List<BadgeData>();

        for (int i = 0; i < (int)eBadgeSlot.THIRD; i++)
        {
            BadgeData badgedata = GameInfo.Instance.BadgeList.Find(x => x.PosSlotNum == i && x.PosKind == (int)eContentsPosKind.ARENA_TOWER);
            if (badgedata == null)
                equipList.Add(null);
            else
                equipList.Add(badgedata);
        }

        return equipList;
    }

    public static bool IsTutorial()
    {
        if (!AppMgr.Instance.configData._Tutorial)
            return false;

        byte[] bytes = GameInfo.Instance.UserData.GetTutorialByte();

        int tutorialstate = bytes[(int)eTutorial.State];
        int tutorialstep = bytes[(int)eTutorial.Step];

        if(tutorialstate == (int)eTutorialState.TUTORIAL_STATE_EndTutorial)
        {
            return false;
        }

        return true;
    }

    public static bool IsInLobbyTutorial()
    {
        if (!AppMgr.Instance.configData._Tutorial)
            return false;


        byte[] bytes = GameInfo.Instance.UserData.GetTutorialByte();

        int tutorialstate = bytes[(int)eTutorial.State];
        int tutorialstep = bytes[(int)eTutorial.Step];

        if (tutorialstate == (int)eTutorialState.TUTORIAL_STATE_EndTutorial)
            return false;

        if (tutorialstate == (int)eTutorialState.TUTORIAL_STATE_Init ||
            tutorialstate == (int)eTutorialState.TUTORIAL_STATE_Stage2Clear ||
            tutorialstate == (int)eTutorialState.TUTORIAL_STATE_Stage3Clear)
            return false;

        return true;
    }

	public static bool IsInGameTutorial() {
        if( !AppMgr.Instance.configData._Tutorial || World.Instance.TestScene || World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
            return false;
        }

		byte[] bytes = GameInfo.Instance.UserData.GetTutorialByte();

		int tutorialstate = bytes[(int)eTutorial.State];
		int tutorialstep = bytes[(int)eTutorial.Step];

        if( tutorialstate == (int)eTutorialState.TUTORIAL_STATE_EndTutorial ) {
            return false;
        }

        if( tutorialstate == (int)eTutorialState.TUTORIAL_STATE_Init ||
            tutorialstate == (int)eTutorialState.TUTORIAL_STATE_Stage2Clear ||
            tutorialstate == (int)eTutorialState.TUTORIAL_STATE_Stage3Clear ) {
            return true;
        }

		return false;
	}




	public static void TutorialNext()
    {
        var popup = LobbyUIManager.Instance.GetActiveUI<UITutorialPopup>("TutorialPopup");
        if (popup == null)
            return;
        popup.Next();
    }

    public static void ShowTutorial()
    {
        int state = GameInfo.Instance.UserData.GetTutorialState();

        LobbyUIManager.Instance.HideAll(FComponent.TYPE.Popup, false);
        
        if (state == (int)eTutorialState.TUTORIAL_STATE_CardLevelUp) //캐릭터 서포터 장착창으로 연결
        {
            var chardata = GameInfo.Instance.GetMainChar();
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, chardata.CUID);
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, chardata.TableID);
            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.SUPPORTER);
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);
        }
        else if (state == (int)eTutorialState.TUTORIAL_STATE_SkillEquip)  //캐릭터 서포터 장착창, 스킬선택 팝업 연결 0번슬롯 선택
        {
            var chardata = GameInfo.Instance.GetMainChar();
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, chardata.CUID);
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, chardata.TableID);
            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.SKILL);
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);

            UIValue.Instance.SetValue(UIValue.EParamType.CharEquipCardSlot, 0);
            LobbyUIManager.Instance.ShowUI("CharSkillSeletePopup", true);
        }
        else
        {
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN);
        }

        UIValue.Instance.SetValue(UIValue.EParamType.TutorialState, state);
        UIValue.Instance.SetValue(UIValue.EParamType.TutorialStep, 1);
        LobbyUIManager.Instance.ShowUI("TutorialPopup", false);
    }
    public static bool ShowTutorialFlag( eTutorialFlag etf )
    {
        if (!AppMgr.Instance.configData._Tutorial)
            return false;

        if (GameInfo.Instance.UserData.IsOnTutorialFlag(etf))
            return false;

        UIValue.Instance.SetValue(UIValue.EParamType.TutorialState, (int)eTutorialState.TUTORIAL_FLAG + (int)etf);
        UIValue.Instance.SetValue(UIValue.EParamType.TutorialStep, 1);
        LobbyUIManager.Instance.ShowUI("TutorialPopup", false);

        return true;
    }

    //서포터 아이콘 이미지 이름 반환
    public static string GetCardTypeSpriteName(eSTAGE_CONDI type)
    {
        string result = string.Empty;

        switch(type)
        {
            case eSTAGE_CONDI.ASSIST_CARD_CNT:
                result = "SupporterType_1";
                break;
            case eSTAGE_CONDI.PROTECT_CARD_CNT:
                result = "SupporterType_2";
                break;
            case eSTAGE_CONDI.DOMINATE_CARD_CNT:
                result = "SupporterType_3";
                break;
            default:
                break;
        }

        return result;
    }

    //서포터 장착 조건 체크
    public static bool GetCardCondiCheck(CharData charData, eSTAGE_CONDI cardType, int equipCardCnt)
    {
        if (charData == null)
            return false;
        if (cardType == eSTAGE_CONDI.NONE)
            return false;

        int tempCnt = 0;

        for(int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            CardData carddata = GameInfo.Instance.GetCardData(charData.EquipCard[i]);
            if (carddata != null)
            {
                if (carddata.Type == (int)cardType)
                    tempCnt++;
            }
        }

        return (tempCnt >= equipCardCnt);
    }

    //현재 날짜 기준으로 해당 주의 일요일을 가지고 온다.
    public static DateTime GetCurrentWeekArenaEndTime()
    {
        if (GameInfo.Instance.IsConnect)
        {
            return GameInfo.Instance.ServerData.ArenaSeasonEndTime;
        }
        else
        {
            DateTime nowTime = GameSupport.GetCurrentServerTime();

            if ((int)nowTime.DayOfWeek == GameInfo.Instance.GameConfig.ArenaEndDay)
                return nowTime;

            DateTime endDayTime = nowTime.AddDays((int)eDayOfWeek._END_DAY_ - (int)nowTime.DayOfWeek);

            return new DateTime(endDayTime.Year, endDayTime.Month, endDayTime.Day, GameInfo.Instance.GameConfig.ArenaEndTime, 0, 0);
        }
    }

    public static DateTime GetCuttentWeekEndTime()
    {
        
        DateTime nowTime = GameSupport.GetCurrentServerTime();
        DateTime endDayTime;
        if ((int)nowTime.DayOfWeek == GameInfo.Instance.GameConfig.WeeklyResetDay)
            endDayTime = nowTime.AddDays((int)eDayOfWeek._END_DAY_);
        else
            endDayTime = nowTime.AddDays((int)eDayOfWeek._END_DAY_ - (int)nowTime.DayOfWeek);

        return new DateTime(endDayTime.Year, endDayTime.Month, endDayTime.Day, GameInfo.Instance.GameConfig.WeeklyResetTime, 0, 0);
        
    }

    //아레나 입장 가능 여부 확인
    public static eArenaState ArenaPlayFlag()
    {
        //0 = 시즌 진행중
        //1 = 시즌 종료
        //GameInfo.Instance.ServerData.ArenaState = 1;
        if (GameInfo.Instance.ServerData.ArenaState == 0)
        {
            DateTime nowTime = GameSupport.GetCurrentServerTime();
            //nowTime = new DateTime(2020, 2, 9, 23, 25, 0);
            DateTime endDay = GetCurrentWeekArenaEndTime();

            //보상 받을 시즌과 현재 진행중인 시즌이 다르면 보상받고 진행.
            if(endDay != GameInfo.Instance.UserBattleData.Now_RewardDate)
            {
                return eArenaState.REWARD;
            }

            //현재 날짜가 종료날짜일때 시간 비교
            if (nowTime.Day.Equals(endDay.Day))
            {
                //플레이 불가능
                if (endDay <= nowTime && nowTime <= endDay.AddHours(GameInfo.Instance.GameConfig.ArenaEndResultHour))
                    return eArenaState.COMBINE;
            }

            return eArenaState.PLAYING;
        }
        else if(GameInfo.Instance.ServerData.ArenaState == 1)
        {
            return eArenaState.COMBINE;
        }

        return eArenaState.COMBINE;
    }

    //현재 점수로 현재 등급 반환
    public static int GetArenaGradeWithNowPoint(int score, eArenaGradeFlag flag)
    {
        int result = 0;

        List<GameTable.ArenaGrade.Param> arenaGradeList = GameInfo.Instance.GameTable.ArenaGrades;

        for(int i = 0; i < arenaGradeList.Count; i++)
        {
            if (arenaGradeList[i].ReqScore <= score)
            {
                if (flag == eArenaGradeFlag.GRADE)
                    result = arenaGradeList[i].Grade;
                else if (flag == eArenaGradeFlag.GRADE_ID)
                    result = arenaGradeList[i].GradeID;                         
            }
            else
            {
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// 아레나 팀에 배치된지 검사
    /// </summary>
    /// <param name="charuid"></param>
    /// <returns></returns>
    public static List<long> ArenaCharChange(long charuid, int slotNum)
    {
        List<long> _teamCharList = new List<long>();
        for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
            _teamCharList.Add(GameInfo.Instance.TeamcharList[i]);

        //이미 아레나 팀에 배치된 캐릭터인지 검사
        bool beTeam = false;
        for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
        {
            if (_teamCharList[i].Equals(charuid))
            {
                beTeam = true;
                break;
            }
        }

        if (beTeam)      //이미 배치된 캐릭터 교체 or 해제
        {
            if (_teamCharList[slotNum] == charuid)
            {
                //해제
                _teamCharList[slotNum] = (int)eCOUNT.NONE;
            }
            else
            {
                //교체 or 장착
                if (_teamCharList[slotNum] != (int)eCOUNT.NONE)
                {
                    for (int i = 0; i < _teamCharList.Count; i++)
                    {
                        if (_teamCharList[i] == charuid)
                        {
                            long temp = _teamCharList[slotNum];
                            _teamCharList[slotNum] = charuid;
                            _teamCharList[i] = temp;
                            break;
                        }
                    }
                }
                else
                {
                    //해제
                    for (int i = 0; i < _teamCharList.Count; i++)
                    {
                        if (_teamCharList[i].Equals(charuid))
                        {
                            _teamCharList[i] = (int)eCOUNT.NONE;
                            break;
                        }
                    }
                    _teamCharList[slotNum] = charuid;
                }
            }
        }
        else
        {
            _teamCharList[slotNum] = charuid;
        }

        return _teamCharList;
    }

    //아레나 배경 설정
    public static void SetArenaBG(int bgIndex, bool state)
    {
        if (state)
        {
            LobbyUIManager.Instance.BG_Arena(bgIndex, "Story/BG/TA0_NC020a.png");
        }
        else
        {
            LobbyUIManager.Instance.BG_Arena(bgIndex, "Story/BG/TA0_NC020b.png");
        }
    }

    //문양 정보로 문양 아이콘 반환
    public static Texture GetBadgeIcon(BadgeData badgeData)
    {
        if (badgeData == null)
            return null;
        else
        {
            GameTable.BadgeOpt.Param data = GameInfo.Instance.GameTable.FindBadgeOpt(x => x.OptionID == badgeData.OptID[(int)eCOUNT.NONE]);
            return (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + data.Icon);
        }
    }

    public static Texture GetBadgeIcon(GameTable.BadgeOpt.Param badgeTableData)
    {
        if (badgeTableData == null)
            return null;
        else
        {
            return (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + badgeTableData.Icon);
        }
    }

    //아레나 팀 편성 체크
    public static bool ArenaTeamCheckFlag()
    {
        //대장 슬롯이 비었을때
        if(GameInfo.Instance.TeamcharList[(int)eArenaTeamSlotPos.LAST_POS] == (int)eCOUNT.NONE)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3161));
            return false;
        }

        if(GameInfo.Instance.TeamcharList[(int)eArenaTeamSlotPos.START_POS] != (int)eCOUNT.NONE &&
            GameInfo.Instance.TeamcharList[(int)eArenaTeamSlotPos.MID_POS] == (int)eCOUNT.NONE)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3162));
            return false;
        }


        return true;
    }

    //문양 남은 강화 횟수가 0인지 아닌지 체크
    public static bool IsBadgeMaxLvCntCheck(BadgeData badgedata)
    {
        if (badgedata.RemainLvCnt <= 0)
            return false;
        else
            return true;
    }

    //아레나 승급전 승,패여부 비트연산
    public static List<eArenaGradeUpFlag> IsArenaGradeUpWinLoseFlags()
    {
        List<eArenaGradeUpFlag> result = new List<eArenaGradeUpFlag>();

        for (int i = 0; i < GameInfo.Instance.GameConfig.ArenaPromotionCnt; i++)
        {
            result.Add(eArenaGradeUpFlag.NONE);
        }

        for (int i = 0; i < GameInfo.Instance.GameConfig.ArenaPromotionCnt - GameInfo.Instance.UserBattleData.Now_PromotionRemainCnt; i++)
        {
            if (GameSupport._IsOnBitIdx((uint)GameInfo.Instance.UserBattleData.Now_PromotionWinCnt, i))
                result[i] = eArenaGradeUpFlag.WIN;
            else
                result[i] = eArenaGradeUpFlag.LOSE;
        }

        return result;
    }

    /// <summary>
    /// 장착중인 문양 반환
    /// </summary>
    /// <returns></returns>
    public static List<BadgeData> GetEquipBadgeList(eContentsPosKind posKind)
    {
        List<BadgeData> equipList = new List<BadgeData>();

        for(int i = 0; i < (int)eBadgeSlot.THIRD; i++)
        {
            BadgeData badgedata = GameInfo.Instance.BadgeList.Find(x => x.PosSlotNum == i && x.PosKind == (int)posKind);
            if (badgedata == null)
                equipList.Add(null);
            else
                equipList.Add(badgedata);                
        }

        return equipList;
    }

    /// <summary>
    /// 아레나 팀 전투력
    /// </summary>
    /// <returns></returns>
    public static int GetArenaTeamPower(eContentsPosKind poskind, PresetData presetData = null, eContentsPosKind presetPosKind = eContentsPosKind._NONE_)
    {
        if (poskind == eContentsPosKind.PRESET && presetData == null)
        {
            return 0;
        }

        float result = 0;
        List<long> _charlist = null;

        switch(poskind)
        {
            case eContentsPosKind.ARENA: 
                _charlist = GameInfo.Instance.TeamcharList; 
                break;

            case eContentsPosKind.ARENA_TOWER: 
                _charlist = GetArenaTowerTeamCharList(); 
                break;

            case eContentsPosKind.PRESET:
                {
                    _charlist = new List<long>();
                    for (int i = 0; i < (int)eArenaTeamSlotPos._MAX_; i++)
                    {
                        long charUid = 0;
                        if (i < presetData.CharDatas.Length)
                        {
                            if (presetData.CharDatas[i] != null)
                            {
                                charUid = presetData.CharDatas[i].CharUid;
                            }
                        }
                        _charlist.Add(charUid);
                    }
                } 
            break;

            case eContentsPosKind.RAID:
                _charlist = GameInfo.Instance.RaidUserData.CharUidList;
                break;

            default: return 0;
        }

        for(int i = 0; i < _charlist.Count; i++)
        {
            if (_charlist[i] == (int)eCOUNT.NONE)
                continue;

            CharData chardata = null;
            switch (poskind)
            {
                case eContentsPosKind.ARENA:
                case eContentsPosKind.RAID:
                    chardata = GameInfo.Instance.GetCharData(_charlist[i]); 
                    break;

                case eContentsPosKind.ARENA_TOWER: chardata = GameInfo.Instance.GetArenaTowerCharData(_charlist[i]); break;
                case eContentsPosKind.PRESET:
                    {
                        CharData sourData = GameInfo.Instance.GetCharData(_charlist[i]);
                        if (sourData != null)
                        {
                            chardata = sourData.PresetDataClone(presetData.CharDatas[i]);
                        }
                    } break;
            }
            if (chardata != null)
            {
                if (presetPosKind == eContentsPosKind._NONE_)
                {
                    result += GetCombatPower(chardata, eWeaponSlot.MAIN, poskind);
                }
                else
                {
                    result += GetCombatPower(chardata, eWeaponSlot.MAIN, presetPosKind,
                        presetData.CharDatas[i].WeaponDatas[(int)eWeaponSlot.MAIN], presetData.CharDatas[i].WeaponDatas[(int)eWeaponSlot.SUB]);
                }
            }
        }
        result *= GameInfo.Instance.BattleConfig.TeamPowerCharMag;

        float totalBadgeOpt = 0;
        List<BadgeData> equipList = null;
        switch(poskind)
        {
            case eContentsPosKind.PRESET:
                {
                    equipList = new List<BadgeData>();
                    foreach(long badgeUid in presetData.BadgeUdis)
                    {
                        equipList.Add(GameInfo.Instance.BadgeList.Find(x => x.BadgeUID == badgeUid));
                    }
                } break;
            default:
                {
                    equipList = GetEquipBadgeList(poskind);
                } break;
        }
        // 메인 옵션 체크
        int mainOptID = 0;
        int mainOptEquipCnt = 0;
        if (equipList[(int)eBadgeSlot.FIRST - 1] != null)
        {
            mainOptID = equipList[(int)eBadgeSlot.FIRST - 1].OptID[(int)eBadgeOptSlot.FIRST];
            if (poskind == eContentsPosKind.PRESET)
            {
                foreach (BadgeData badgeData in equipList)
                {
                    if (badgeData == null)
                    {
                        continue;
                    }

                    if (mainOptID == badgeData.OptID[(int)eBadgeOptSlot.FIRST])
                    {
                        ++mainOptEquipCnt;
                    }
                }
            }
            else
            {
                mainOptEquipCnt = GetMainOptEquipCnt(equipList, mainOptID);
            }
        }

        foreach (BadgeData badgeData in equipList)
        {
            if (badgeData == null)
            {
                continue;
            }

            for (int i = (int)eBadgeOptSlot.FIRST; i < (int)eBadgeOptSlot._MAX_; i++)
            {
                GameTable.BadgeOpt.Param opt = GameInfo.Instance.GameTable.BadgeOpts.Find(x => x.OptionID == badgeData.OptID[i]);
                if (opt == null)
                {
                    continue;
                }

                if (badgeData.OptID[i] == mainOptID)
                {
                    float optValue = (badgeData.OptVal[i] + badgeData.Level) * opt.BattlePowerRate;
                    totalBadgeOpt += optValue + (optValue * GameInfo.Instance.GameConfig.BadgeSetAddRate[mainOptEquipCnt]);
                }
                else
                {
                    totalBadgeOpt += (badgeData.OptVal[i] + badgeData.Level) * opt.BattlePowerRate;
                }
            }
        }

        result += (result * totalBadgeOpt * GameInfo.Instance.BattleConfig.TeamPowerBadgeMag);

        return (int)result;
    }

    /// <summary>
    /// 아레나 대전 상대 팀 전투력
    /// </summary>
    /// <returns></returns>
    public static int GetArenaEnemyTeamPower()
    {
        float result = 0;

        for (int i = 0; i < GameInfo.Instance.MatchTeam.charlist.Count; i++)
        {
            if (GameInfo.Instance.MatchTeam.charlist[i] == null)
                continue;

            if (GameInfo.Instance.MatchTeam.charlist[i].CharData.TableID == (int)eCOUNT.NONE ||
                GameInfo.Instance.MatchTeam.charlist[i].CharData.TableData == null)
                continue;

            result += GetEnemyCombatPower(GameInfo.Instance.MatchTeam.charlist[i], eWeaponSlot.MAIN, eContentsPosKind.ARENA);
        }

        // 승급전이 아닌 경우 연승에 의한 전투력 증감
        if (GameInfo.Instance.UserBattleData.Now_PromotionRemainCnt <= 0)
        {
            float fRate = Mathf.Max(GameInfo.Instance.BattleConfig.ArenaWinLoseBuffRate * GameInfo.Instance.UserBattleData.Now_WinLoseCnt, -0.99f);
            result += (result * fRate);
        }
        result *= GameInfo.Instance.BattleConfig.TeamPowerCharMag;

        float totalBadgeOpt = 0;
        List<BadgeData> equipList = GameInfo.Instance.MatchTeam.badgelist;
        if (equipList.Count > 0)
        {
            // 메인 옵션 체크
            int mainOptID = 0;
            int mainOptEquipCnt = 0;
            if (equipList[(int)eBadgeSlot.FIRST - 1] != null)
            {
                mainOptID = equipList[(int)eBadgeSlot.FIRST - 1].OptID[(int)eBadgeOptSlot.FIRST];
                mainOptEquipCnt = GameSupport.GetMainOptEquipCnt(equipList, mainOptID);
            }
            for (int i = 0; i < equipList.Count; i++)
            {
                if (equipList[i] == null)
                    continue;
                BadgeData badgedata = equipList[i];
                for (int j = (int)eBadgeOptSlot.FIRST; j < (int)eBadgeOptSlot._MAX_; j++)
                {
                    GameTable.BadgeOpt.Param opt = GameInfo.Instance.GameTable.BadgeOpts.Find(x => x.OptionID == badgedata.OptID[j]);
                    if (opt == null)
                        continue;
                    if (badgedata.OptID[j] == mainOptID)
                    {
                        float optValue = (badgedata.OptVal[j] + badgedata.Level) * opt.BattlePowerRate;
                        totalBadgeOpt += optValue + (optValue * GameInfo.Instance.GameConfig.BadgeSetAddRate[mainOptEquipCnt]);
                    }
                    else
                        totalBadgeOpt += (badgedata.OptVal[j] + badgedata.Level) * opt.BattlePowerRate;
                }
            }
            result += (result * totalBadgeOpt * GameInfo.Instance.BattleConfig.TeamPowerBadgeMag);
        }

        return (int)(result);
    }

    /// <summary>
    /// 아레나 더미 캐릭터 팀 전투력 계산
    /// </summary>
    /// <param name="teamCharData"></param>
    /// <param name="wpnSlot"></param>
    /// <param name="contPos"></param>
    /// <returns></returns>
    public static int GetEnemyCombatPower(TeamCharData teamCharData, eWeaponSlot wpnSlot, eContentsPosKind contPos = eContentsPosKind._NONE_)
    {
        BattleConfig bc = GameInfo.Instance.BattleConfig;

        float totalCombatPower = 0;

        CharData chardata = teamCharData.CharData;

        Action<int> LogCombatPower = (idx) =>
        {
            Debug.Log(string.Format("Index {0}, Power {1}", idx, totalCombatPower));
        };

        // 기본 스탯 전투력 계산
        totalCombatPower += (int)(GetTotalHP(chardata, teamCharData.MainWeaponData, teamCharData.SubWeaponData, teamCharData.CardList, teamCharData.MainGemList, teamCharData.SubGemList) * bc.CombatPowerBaseMag / bc.CombatPowerHPMag); LogCombatPower(0);
        totalCombatPower += (int)(GetTotalATK(chardata, teamCharData.MainWeaponData, teamCharData.SubWeaponData, teamCharData.MainGemList, teamCharData.SubGemList) * bc.CombatPowerBaseMag / bc.CombatPowerATKMag); LogCombatPower(1);
        totalCombatPower += (int)(GetTotalDEF(chardata, teamCharData.MainWeaponData, teamCharData.SubWeaponData, teamCharData.CardList, teamCharData.MainGemList, teamCharData.SubGemList) * bc.CombatPowerBaseMag / bc.CombatPowerDEFMag); LogCombatPower(2);
        totalCombatPower += (int)(GetTotalCRI(chardata, teamCharData.MainWeaponData, teamCharData.SubWeaponData, teamCharData.MainGemList, teamCharData.SubGemList) * bc.CombatPowerBaseMag / bc.CombatPowerCRIMag); LogCombatPower(3);

        // 캐릭터 패시브 스킬 전투력 계산
        float totalCharSkillLv = 0;
        var upgradeskilllist = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.CharacterID == chardata.TableID && (x.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_NORMAL || x.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_ULTIMATE));
        for (int i = 0; i < upgradeskilllist.Count; i++)
        {
            var passviedata = chardata.PassvieList.Find(x => x.SkillID == upgradeskilllist[i].ID);
            if (passviedata != null)
                totalCharSkillLv += passviedata.SkillLevel;
        }
        if (contPos == eContentsPosKind.ARENA || contPos == eContentsPosKind.ARENA_TOWER)
            totalCharSkillLv *= bc.ArenaCharSkillOptRate;
        totalCombatPower += (int)(totalCharSkillLv * bc.CombatPowerBaseMag * bc.CombatPowerCharSkillMag); LogCombatPower(4);

        // 서포터 스킬 레벨 전투력 계산
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            if(i >= teamCharData.CardList.Count)
            {
                break;
            }

            //CardData equipCard = GameInfo.Instance.GetCardData(chardata.EquipCard[i]);
            CardData equipCard = teamCharData.CardList[i];
            if (equipCard != null)
                totalCombatPower += (int)(equipCard.SkillLv * bc.CombatPowerBaseMag * bc.CombatPowerSptSkillMag * bc.CombatPowerSptSkillRateByGrade[equipCard.TableData.Grade]);
        }
        LogCombatPower(5);
        // 무기 스킬 레벨 전투력 계산
        //WeaponData equipWpn = GameInfo.Instance.GetWeaponData(chardata.EquipWeaponUID);
        WeaponData equipWpn = teamCharData.MainWeaponData;
        totalCombatPower += (int)(equipWpn.SkillLv * bc.CombatPowerBaseMag * bc.CombatPowerWpnSkillMag * bc.CombatPowerWpnSkillRateByGrade[equipWpn.TableData.Grade]);
        LogCombatPower(6);
        // 곡옥 옵션 전투력 계산
        float totalGemOpt = 0;
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {
            if (equipWpn.SlotGemUID[i] == (int)eCOUNT.NONE)
                continue;
            //GemData gemdata = GameInfo.Instance.GetGemData(equipWpn.SlotGemUID[i]);
            GemData gemdata = teamCharData.MainGemList[i];
            if (gemdata == null)
                continue;
            for (int j = 0; j < (int)eCOUNT.GEMRANDOPT; j++)
            {
                if (gemdata.RandOptValue[j] > 0)
                    totalGemOpt += gemdata.RandOptValue[j];
            }
        }
        if (contPos == eContentsPosKind.ARENA || contPos == eContentsPosKind.ARENA_TOWER)
            totalGemOpt *= bc.ArenaGemOptRate;
        totalCombatPower += (int)(totalGemOpt * bc.CombatPowerBaseMag * bc.CombatPowerGemOptMag);
        LogCombatPower(7);
        return (int)(totalCombatPower);
    }

    /// <summary>
    /// 아레나 더미 TeamCharData생성
    /// </summary>
    /// <param name="charTableId"></param>
    /// <returns></returns>
    public static TeamCharData CreateArenaEnemyTeamCharData(int charTableId, GameClientTable.NPCCharRnd.Param npcRnd)
    {
        TeamCharData teamCharData = new TeamCharData();

        GameTable.Character.Param param = GameInfo.Instance.GameTable.FindCharacter(charTableId);

        if (param == null)
        {
            Debug.LogError(charTableId + " 번 캐릭터가 없습니다.");
            return null;
        }

        teamCharData.CharData = new CharData();
        teamCharData.CharData.TableID = charTableId;
        teamCharData.CharData.TableData = param;

        int charId = charTableId * 1000;
        int randIndex = 0;

        // 코스튬 - 랜덤생성 (한정판매 코스튬은 판매기간에만 가능)
        List<GameTable.Costume.Param> listCostume = GameInfo.Instance.GameTable.FindAllCostume(x => x.CharacterID == charTableId && x.PreVisible != (int)eCOUNT.NONE);
        randIndex = UnityEngine.Random.Range(0, listCostume.Count);

        GameTable.Costume.Param selCostume = listCostume[randIndex];
        teamCharData.CharData.EquipCostumeID = selCostume.ID;
        Debug.Log("###### PVP 코스튬 ID : " + selCostume.ID);
        //코스튬 색상랜덤
        teamCharData.CharData.CostumeColor = UnityEngine.Random.Range(0, selCostume.ColorCnt);
        Debug.Log("###### PVP 코스튬 색상 : " + teamCharData.CharData.CostumeColor);

        teamCharData.CharData.Grade = npcRnd.CharGrade;
        teamCharData.CharData.Level = GameSupport.GetNPCCharCndSplitRandomValue(npcRnd.CharLv);

        //메인 곡옥
        teamCharData.MainGemList.Clear();
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {
            teamCharData.MainGemList.Add(null);
        }

        //메인 곡옥
        //teamCharData.MainGemList = CreateArenaDummyGemList();

        //무기
        int wTid = -1;
        teamCharData.MainWeaponData = CreateArenaDummyWeaponData( charId, npcRnd, ref wTid );
        teamCharData.CharData.EquipWeaponUID = teamCharData.MainWeaponData.WeaponUID;

        //스킬
        teamCharData.CharData.PassvieList.Clear();
        teamCharData.CharData.PassvieList.Add(new PassiveData(charId + 301, 1));        //오의
        teamCharData.CharData.EquipSkill[0] = charId + 301;

        //스킬 캐릭터에 장착
        List<int> skillIds = CreateArenaDummyCharEquipSkillList(charId, teamCharData.CharData.Level);
        if (skillIds.Count > 0)
        {
            for (int i = 0; i < skillIds.Count; i++)
            {
                //teamCharData.CharData.PassvieList.Add(new PassiveData(skillIds[i], 1));
                teamCharData.CharData.PassvieList.Insert(0, new PassiveData(skillIds[i], 1));
                if (i < (int)eCOUNT.SKILLSLOT - 1)
                {
                    teamCharData.CharData.EquipSkill[i] = skillIds[i];
                }
            }
        }

        //패시브 스킬 캐릭터에 적용
        skillIds = CreateArenaOpponentCharPassiveSkillList(charTableId);
        if (skillIds.Count > 0)
        {
            for (int i = 0; i < skillIds.Count; i++)
            {
                teamCharData.CharData.PassvieList.Add(new PassiveData(skillIds[i], 1 + (int)((float)teamCharData.CharData.Level * GameInfo.Instance.BattleConfig.ArenaEnemyCharPassvieRate)));
            }
        }

        //서포터
        teamCharData.CardList = CreateArenaDummyCardList(npcRnd);
        for (int i = 0; i < (int)eCardSlotPosMax.CHAR; i++)
        {
            if(teamCharData.CardList[i] == null)
            {
                Log.Show("Enemy Card Pos : " + i + " is NULL");
                continue;
            }
            if(teamCharData.CardList[i].TableID != (int)eCOUNT.NONE)
            {
                teamCharData.CharData.EquipCard[i] = teamCharData.CardList[i].TableID;
            }
        }
        return teamCharData;
    }

    /// <summary>
    /// 아레나 더미 카드
    /// </summary>
    /// <param name="npcRnd"></param>
    /// <returns></returns>
    public static List<CardData> CreateArenaDummyCardList(GameClientTable.NPCCharRnd.Param npcRnd)
    {
        List<CardData> cards = new List<CardData>();

        List<int> listSupporterIds = new List<int>();
        List<GameTable.Card.Param> listAllCards = GameInfo.Instance.GameTable.FindAllCard(x => x.Grade == npcRnd.SptGrade && x.PreVisible != (int)eCOUNT.NONE);

        int randIndex = 0;

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            randIndex = UnityEngine.Random.Range(0, listAllCards.Count);
            listSupporterIds.Add(listAllCards[randIndex].ID);

            listAllCards.RemoveAt(randIndex);
        }

        for (int i = 0; i < listSupporterIds.Count; i++)
        {
            if (i >= (int)eCOUNT.CARDSLOT)
                continue;

            CardData carddata = new CardData(0, listSupporterIds[i]);
            carddata.Wake = npcRnd.SptWake;
            carddata.Level = GameSupport.GetNPCCharCndSplitRandomValue(npcRnd.SptLv);
            carddata.SkillLv = npcRnd.SptSLv;
            carddata.Type = carddata.TableData != null ? carddata.TableData.Type : 0;

            cards.Add(carddata);
        }


        return cards;
    }

    /// <summary>
    /// 아레나 더미 캐릭터 랜덤 스킬 장착
    /// </summary>
    /// <param name="charIdx"></param>
    /// <returns></returns>
    public static List<int> CreateArenaDummyCharEquipSkillList(int charIdx, int charLevel)
    {
        List<int> listSkillId = new List<int>();

        List<GameTable.CharacterSkillPassive.Param> listAllDefaultSkill = new List<GameTable.CharacterSkillPassive.Param>();
        for (int i = 0; i < 10; i++)
        {
            int skillIndex = charIdx + 200 + (i * 10);
            GameTable.CharacterSkillPassive.Param paramSkill = GameInfo.Instance.GameTable.FindCharacterSkillPassive(skillIndex);
            if (paramSkill == null)
            {
                Debug.LogError(skillIndex + " 번 캐릭터 스킬은 없습니다.");
                continue;
            }

            GameTable.ItemReqList.Param paramItemReqList = GameInfo.Instance.GameTable.FindItemReqList(paramSkill.ItemReqListID);
            if (paramItemReqList != null && charLevel < paramItemReqList.LimitLevel)
            {
                continue;
            }

            listAllDefaultSkill.Add(paramSkill);
        }

        // 스킬 커맨드 중복 안되게끔 무작위로 선택
        int randIndex = UnityEngine.Random.Range(0, listAllDefaultSkill.Count);

        List<GameTable.CharacterSkillPassive.Param> listSkill = new List<GameTable.CharacterSkillPassive.Param>();
        listSkill.Add(listAllDefaultSkill[randIndex]);

        List<GameTable.CharacterSkillPassive.Param> listFindSkill = listAllDefaultSkill.FindAll(x => x.CommandIndex != listSkill[0].CommandIndex);
        randIndex = UnityEngine.Random.Range(0, listFindSkill.Count);
        listSkill.Add(listFindSkill[randIndex]);

        listFindSkill = listAllDefaultSkill.FindAll(x => x.CommandIndex != listSkill[0].CommandIndex && x.CommandIndex != listSkill[1].CommandIndex);
        randIndex = UnityEngine.Random.Range(0, listFindSkill.Count);
        listSkill.Add(listFindSkill[randIndex]);

        for (int i = 0; i < listSkill.Count; i++)
        {
            listSkillId.Add(listSkill[i].ID);
        }

        return listSkillId;
    }

    /// <summary>
    /// 아레나 상대 캐릭터 패시브 스킬 적용
    /// </summary>
    /// <param name="charIdx"></param>
    /// <returns></returns>
    public static List<int> CreateArenaOpponentCharPassiveSkillList(int charIdx)
    {
        List<int> listSkillId = new List<int>();

        var upgradeskilllist = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.CharacterID == charIdx && (x.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_NORMAL || x.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_ULTIMATE) && x.Slot < 10);
        for (int i = 0; i < upgradeskilllist.Count; i++)
        {
            listSkillId.Add(upgradeskilllist[i].ID);
        }

        return listSkillId;
    }

    /// <summary>
    /// 아레나 더미 곡옥 장착 - 현재 사용 안함
    /// </summary>
    /// <returns></returns>
    public static List<GemData> CreateArenaDummyGemList()
    {
        List<GemData> gemDatas = new List<GemData>();

        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {
            GameTable.Gem.Param gem = GameInfo.Instance.GameTable.Gems[UnityEngine.Random.Range(0, GameInfo.Instance.GameTable.Gems.Count)];
            GemData gemdata = new GemData((int)eCOUNT.NONE, gem.ID);

            gemdata.GemUID = i;

            gemdata.Level = UnityEngine.Random.Range(1, GameInfo.Instance.GameConfig.GemMaxLevel);
            gemdata.Wake = UnityEngine.Random.Range(1, GameInfo.Instance.GameConfig.GemMaxWake);
            
            for (int j = 0; j < gemdata.Wake; j++)
            {
                List<GameTable.GemRandOpt.Param> randops = GameInfo.Instance.GameTable.FindAllGemRandOpt(x => x.GroupID == gem.RandOptGroup);
                int randIdx = UnityEngine.Random.Range(0, randops.Count);
                gemdata.RandOptID[j] = randops[randIdx].ID;
                gemdata.RandOptValue[j] = UnityEngine.Random.Range(randops[randIdx].Min, randops[randIdx].Max + 1);
            }

            gemDatas.Add(gemdata);
        }

        return gemDatas;
    }

	/// <summary>
	/// 아레나 더미 무기
	/// </summary>
	/// <param name="charIdx">캐릭터 TID</param>
	/// <param name="npcRnd">랜덤 테이블</param>
	/// <param name="weaponTableID"></param>
	/// <returns></returns>
	public static WeaponData CreateArenaDummyWeaponData( int charIdx, GameClientTable.NPCCharRnd.Param npcRnd, ref int weaponTableID ) {
        int wTid = weaponTableID;

        WeaponData weapondata = new WeaponData();

		List<GameTable.Weapon.Param> listWeapon = GameInfo.Instance.GameTable.FindAllWeapon(x => x.ID >= charIdx && x.ID <= charIdx + 999 && x.Grade == npcRnd.WpnGrade && x.PreVisible != (int)eCOUNT.NONE);
		if( listWeapon.Count <= 0 ) {
			listWeapon = GameInfo.Instance.GameTable.FindAllWeapon( x => x.ID >= charIdx && x.ID <= charIdx + 999 && x.PreVisible != (int)eCOUNT.NONE );
		}

		string[] split = null;

		if( listWeapon.Count <= 0 ) {
			for( int i = 0; i < GameInfo.Instance.GameTable.Weapons.Count; i++ ) {
                if( npcRnd.WpnGrade != GameInfo.Instance.GameTable.Weapons[i].Grade || GameInfo.Instance.GameTable.Weapons[i].PreVisible == (int)eCOUNT.NONE ) {
                    continue;
				}

				split = Utility.Split( GameInfo.Instance.GameTable.Weapons[i].CharacterID, ',' );
				if( split == null || split.Length <= 0 ) {
					continue;
				}

				for( int j = 0; j < split.Length; j++ ) {
					if( Utility.SafeIntParse( split[j] ) * 1000 == charIdx ) {
						listWeapon.Add( GameInfo.Instance.GameTable.Weapons[i] );
					}
				}
			}
		}

        for( int i = 0; i < listWeapon.Count; i++ ) {
            if( listWeapon[i].PreVisible == 0 ) {
                listWeapon.RemoveAt( i );
                --i;
            }
		}

		int randIndex = UnityEngine.Random.Range(0, listWeapon.Count);
		GameTable.Weapon.Param selWeapon = listWeapon[randIndex];

		if( weaponTableID != -1 ) {
			selWeapon = GameInfo.Instance.GameTable.FindWeapon( x => x.ID == wTid );
			if( selWeapon == null ) {
				int findId = weaponTableID % 1000;
				bool find = false;

				for( int i = 0; i < GameInfo.Instance.GameTable.Weapons.Count; i++ ) {
					if( GameInfo.Instance.GameTable.Weapons[i].ID % 1000 != findId ) {
						continue;
					}

					split = Utility.Split( GameInfo.Instance.GameTable.Weapons[i].CharacterID, ',' );
					if( split == null || split.Length <= 0 ) {
						continue;
					}

					for( int j = 0; j < split.Length; j++ ) {
						if( Utility.SafeIntParse( split[j] ) * 1000 == charIdx ) {
							selWeapon = GameInfo.Instance.GameTable.Weapons[i];
                            weaponTableID = selWeapon.ID;

                            find = true;
							break;
						}
					}

					if( find ) {
						break;
					}
				}
			}
		}

		weapondata.TableData = selWeapon;
		weapondata.TableID = selWeapon.ID;
		weapondata.Wake = npcRnd.WpnWake;
		weapondata.Level = GameSupport.GetNPCCharCndSplitRandomValue( npcRnd.WpnLv );
		weapondata.SkillLv = npcRnd.WpnSLv;


		return weapondata;
	}

	/// <summary>
	/// 아레나 더미 문양
	/// </summary>
	/// <param name="npcRnd"></param>
	/// <returns></returns>
	public static List<BadgeData> CreateArenaDummyBadgeList(GameClientTable.NPCCharRnd.Param npcRnd)
    {
        List<BadgeData> badgelist = new List<BadgeData>();

        for (int i = 0; i < (int)eBadgeSlot.THIRD; i++)
            badgelist.Add(new BadgeData());

        List<GameTable.BadgeOpt.Param> badgeAllOpts = new List<GameTable.BadgeOpt.Param>();
        badgeAllOpts.AddRange(GameInfo.Instance.GameTable.BadgeOpts);

        int badgeCnt = GameSupport.GetNPCCharCndSplitRandomValue(npcRnd.BadgeCnt);

        for (int i = 0; i < badgeCnt; i++)
        {
            badgelist[i].Level = UnityEngine.Random.Range(0, GameInfo.Instance.GameConfig.BadgeLvCnt + 1);

            int randIndex = UnityEngine.Random.Range(0, badgeAllOpts.Count);
            badgelist[i].PosSlotNum = i;
            badgelist[i].PosKind = (int)eContentsPosKind.ARENA;
            badgelist[i].OptID[(int)eBadgeOptSlot.FIRST] = badgeAllOpts[randIndex].OptionID;
            badgelist[i].OptVal[(int)eBadgeOptSlot.FIRST] = UnityEngine.Random.Range(GameInfo.Instance.GameConfig.BadgeMinOptVal, GameInfo.Instance.GameConfig.BadgeMaxOptVal + 1);

            badgeAllOpts.RemoveAt(randIndex);
            for (int j = (int)eBadgeOptSlot.SECOND; j < (int)eBadgeOptSlot._MAX_; j++)
            {
                randIndex = UnityEngine.Random.Range(0, badgeAllOpts.Count);
                badgelist[i].OptID[j] = badgeAllOpts[randIndex].OptionID;
                badgelist[i].OptVal[j] = UnityEngine.Random.Range(GameInfo.Instance.GameConfig.BadgeMinOptVal, GameInfo.Instance.GameConfig.BadgeMaxOptVal + 1);
                badgeAllOpts.RemoveAt(randIndex);
            }
        }

        return badgelist;
    }

    /// <summary>
    /// (,)구분으로 랜덤 값 반환
    /// </summary>
    /// <param name="targetValue"></param>
    /// <returns></returns>
    public static int GetNPCCharCndSplitRandomValue(string targetValue)
    {
        string targetV = targetValue.Replace(" ", "");
		string[] targetArray = Utility.Split(targetV, ','); //targetV.Split(',');

        if (targetArray.Length == 1)
            return int.Parse(targetValue);

        int minV = int.Parse(targetArray[0]);
        int maxV = int.Parse(targetArray[1]);

        return UnityEngine.Random.Range(minV, maxV + 1);
    }

    /// <summary>
    /// 메인옵션 장착갯수 반환
    /// </summary>
    /// <param name="badgeList">장착한 문양 리스트</param>
    /// <param name="mainOptID">메인 옵션 ID</param>
    /// <returns></returns>
    public static int GetMainOptEquipCnt(List<BadgeData> badgeList, int mainOptID)
    {
        int result = 0;
        for (int i = 0; i < badgeList.Count; i++)
        {
            if(badgeList[i] == null)
            {
                continue;
            }
            
            switch((eContentsPosKind)badgeList[i].PosKind)
            {
                case eContentsPosKind.ARENA:
                case eContentsPosKind.ARENA_TOWER:
                    break;
                default:
                    continue;
            }

            if (mainOptID == badgeList[i].OptID[(int)eBadgeOptSlot.FIRST])
            {
                result++;
            }
        }

        return result;
    }

    //Unit2ndStatsTable.eBadgeOptType optType = Utility.GetEnumByString<Unit2ndStatsTable.eBadgeOptType>(splits[1]);
    public static float GetEquipBadgeTotalValue(List<BadgeData> badgeList, Unit2ndStatsTable.eBadgeOptType optType, eContentsPosKind posKind = eContentsPosKind.ARENA)
    {
        float result = 0;

        List<BadgeData> equipList = badgeList.FindAll(x => x.PosKind == (int)posKind);

        if (null == equipList || equipList.Count <= 0)
            return 0.0f;

        // 메인 옵션 체크
        int mainOptID = 0;
        int mainOptEquipCnt = 0;
        if (equipList[(int)eBadgeSlot.FIRST - 1] != null)
        {
            mainOptID = equipList[(int)eBadgeSlot.FIRST - 1].OptID[(int)eBadgeOptSlot.FIRST];
            mainOptEquipCnt = GameSupport.GetMainOptEquipCnt(equipList, mainOptID);
        }

        for (int i = 0; i < equipList.Count; i++)
        {
            if (equipList[i] == null)
                continue;

            BadgeData badgedata = equipList[i];

            for (int j = (int)eBadgeOptSlot.FIRST; j < (int)eBadgeOptSlot._MAX_; j++)
            {
                GameTable.BadgeOpt.Param opt = GameInfo.Instance.GameTable.BadgeOpts.Find(x => x.OptionID == badgedata.OptID[j]);
                if (opt == null)
                    continue;

                string str = opt.EffectType;
				string[] splits = Utility.Split(str, '_'); //str.Split('_');
                Unit2ndStatsTable.eBadgeOptType tempType = Utility.GetEnumByString<Unit2ndStatsTable.eBadgeOptType>(splits[1]);

                if (tempType == optType)
                {
                    if (badgedata.OptID[j] == mainOptID)
                    {
                        float optValue = ((badgedata.OptVal[j] + badgedata.Level) * opt.IncEffectValue);
                        result += optValue + (optValue * GameInfo.Instance.GameConfig.BadgeSetAddRate[mainOptEquipCnt]);
                    }
                    else
                        result += (badgedata.OptVal[j] + badgedata.Level) * opt.IncEffectValue;
                }
            }
        }

        return result / (float)eCOUNT.MAX_RATE_VALUE;
    }

    //패스미션 활성화 여부 반환
    public static int GetActivePassMission()
    {
        List<GameTable.PassSet.Param> passSetList = GameInfo.Instance.GameTable.PassSets;

        if (passSetList == null || passSetList.Count <= 0)
            return 0;

        DateTime nowTime = GameInfo.Instance.GetNetworkTime();

        int passID = 0;

        for(int i = 0; i < passSetList.Count; i++)
        {
            DateTime starttime = GetTimeWithString(passSetList[i].StartTime, false);
            DateTime endtime = GetTimeWithString(passSetList[i].EndTime, true);

            if(starttime < nowTime && endtime > nowTime)
            {
                passID = passSetList[i].PassID;
                break;
            }
        }

        return passID;
    }

    //SP티켓 구매 체크
    public static bool GetUseSPItem(int passSetId)
    {
        bool result = false;
        PassSetData data = GameInfo.Instance.UserData.PassSetDataList.Find(x => x.PassID == passSetId);
        if (data != null)
        {
            System.DateTime buyTime = data.PassBuyEndTime;
            if (buyTime != default(System.DateTime) && GameSupport.GetCurrentServerTime() < buyTime)
            {
                result = true;
            }
        }
        
        return result;
    }

    //현재 미션 완료 포인트
    public static int GetPassMissionPoint(int passSetId)
    {
        PassSetData passSetData = GameInfo.Instance.UserData.PassSetDataList.Find(x => x.PassID == passSetId);
        if (passSetData == null)
        {
            return 0;
        }

        int result = 0;
        switch ((ePassSystemType)passSetId)
        {
            case ePassSystemType.Gold:
            {
                List<PassMissionData> passMissionList = GameInfo.Instance.PassMissionData.FindAll(x => x.PassID == passSetId);
                if (passMissionList == null || passMissionList.Count <= 0)
                {
                    return 0;
                }
                for (int i = 0; i < passMissionList.Count; i++)
                {
                    if (passMissionList[i].PassMissionState != 0)
                    {
                        result += passMissionList[i].PassMissionTableData.RewardPoint;
                    }
                }
            } break;
            case ePassSystemType.Rank:
            {
                result = GameInfo.Instance.UserData.Level;
            } break;
            case ePassSystemType.Story:
            {
                GameTable.PassSet.Param passSetParam = GameInfo.Instance.GameTable.FindPassSet(x => x.Type == (int)ePassSystemType.Story);
                if (passSetParam != null)
                {
                    List<GameTable.Random.Param> randomParam = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == passSetParam.N_RewardID || x.GroupID == passSetParam.S_RewardID);
                    if (randomParam != null)
                    {
                        foreach (GameTable.Random.Param random in randomParam)
                        {
                            if (GameInfo.Instance.StageClearList.Any(x => x.TableID == random.Value))
                            {
                                if (result < random.Value)
                                {
                                    result = random.Value;
                                }
                            }
                        }
                    }
                }
            } break;
        }

        return result;
    }

    //패스미션 보상획득 가능한지
    public static bool GetPassRewardFlag(int passSetId)
    {
        PassSetData passSetData = GameInfo.Instance.UserData.PassSetDataList.Find(x => x.PassID == passSetId);
        if (passSetData == null || passSetData.PassTableData == null)
        {
            return false;
        }

        int missionCurPoint = GetPassMissionPoint(passSetId);
        if (passSetData.Pass_NormalReward < missionCurPoint)
        {
            List<GameTable.Random.Param> normalReward = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == passSetData.PassTableData.N_RewardID);
            foreach (var reward in normalReward)
            {
                if (reward.Value <= missionCurPoint && reward.Value > passSetData.Pass_NormalReward)
                {
                    return true;
                }
            }
        }
        if (GameSupport.GetUseSPItem(passSetId))
        {
            if (passSetData.Pass_SPReward < missionCurPoint)
            {
                List<GameTable.Random.Param> spReward = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == passSetData.PassTableData.S_RewardID);
                foreach (var reward in spReward)
                {
                    if (reward.Value <= missionCurPoint && reward.Value > passSetData.Pass_SPReward)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //패스미션 미션완료 가능한지
    public static bool GetPassMissionFlag()
    {
        int PassID = GetActivePassMission();
        if (PassID <= (int)eCOUNT.NONE)
            return false;

        List<PassMissionData> passMissionDataList = GameInfo.Instance.PassMissionData.FindAll(x => x.PassID == PassID);

        for (int i = 0; i < passMissionDataList.Count; i++)
        {
            if (passMissionDataList[i].PassMissionState == (int)eCOUNT.NONE)
            {
                if (passMissionDataList[i].PassMissionValue == (int)eCOUNT.NONE)
                    return true;
            }
        }

        return false;
    }

    public static void LoadLocalizeTexture(UITexture texture, string bundleName, string path, bool blocalize )
    {
        if (texture == null)
            return;

        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan || !blocalize)
        {
            if (!path.Contains(".png"))
            {
                path += ".png";
            }
            
            texture.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle(bundleName, path );
        }
        else
        {
            string strname = Utility.GetStringFirstIndexOf(path, '.') + "-" + FLocalizeString.Language.ToString() + ".png";

            texture.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle(bundleName, strname);
        }
    }
    //Review 평점 관련
    public static void ShowReviewPopup()
    {
#if !UNITY_EDITOR
        if (AppMgr.Instance.Review)
            return;
#endif
        if (GameInfo.Instance.UserData.Level >= GameInfo.Instance.GameConfig.ReviewOpenRank)
        {
            if (PlayerPrefs.HasKey("REVIEW_STORE"))
                return;
            PlayerPrefs.SetString("REVIEW_STORE", "OPEN");
            PlayerPrefs.Save();

            MessagePopup.CYN(
                FLocalizeString.Instance.GetText(1512),
                FLocalizeString.Instance.GetText(1513),
                FLocalizeString.Instance.GetText((int)eTEXTID.YES),
                FLocalizeString.Instance.GetText((int)eTEXTID.CANCEL),
                () => 
                {
                    Log.Show("GoTo Review Stroe!!");

#if !UNITY_EDITOR
#if UNITY_ANDROID
                    if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
                    {
                        Application.OpenURL("market://details?id=com.GREMORY.ActionTaimanin");
                    }
                    else
                    {
                        Application.OpenURL("market://details?id=com.GREMORYGames.ActionTaimanin");
                    }
#elif UNITY_IOS
                    UnityEngine.iOS.Device.RequestStoreReview();
#endif
#endif
                    if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
                    {
                        Application.OpenURL("steam://store/1335200");
                    }
                }
                );
        }
    }


    //Friend Community
    public static string GetFriendLastConnectTimeString(DateTime lastConnectTime)
    {
        System.DateTime curTime = GameSupport.GetCurrentServerTime();
        string remainTime = GameSupport.GetRemainTimeString_DayAndHours(curTime, lastConnectTime);     //시간 or 일

        return string.Format(FLocalizeString.Instance.GetText(1524), remainTime);
    }

    public static void ShowFriendPopupWithSerachFriendList(string serachUUID, bool serachFailMsg = false)
    {
        //시간 값이 초기화 안됬을때
        if(GameInfo.Instance.CommunityData.FriendSerachTime == default(System.DateTime))
        {
            Log.Show("ShowFriendPopupWithSerachFriendList : " + GameInfo.Instance.CommunityData.FriendSerachTime, Log.ColorType.Red);
            GameInfo.Instance.Send_ReqFriendSuggestList(serachUUID, OnAckFriendSuggestList);
        }
        else
        {
            TimeSpan ts = GameSupport.GetCurrentServerTime() - GameInfo.Instance.CommunityData.FriendSerachTime;
            if(ts.Seconds < GameInfo.Instance.GameConfig.FriendRecommendTimeSec)
            {
                Log.Show("Failed ShowFriendPopupWithSerachFriendList : " + GameInfo.Instance.CommunityData.FriendSerachTime, Log.ColorType.Yellow);


                if (serachFailMsg)
                {
                    //검색할수 있는 시간이 아님
                    MessageToastPopup.Show(FLocalizeString.Instance.GetText(3189));
                }

                if(!LobbyUIManager.Instance.IsActiveUI("FriendPopup"))
                    LobbyUIManager.Instance.ShowUI("FriendPopup", true);
            }
            else
            {
                Log.Show("Success ShowFriendPopupWithSerachFriendList : " + GameInfo.Instance.CommunityData.FriendSerachTime);
                GameInfo.Instance.Send_ReqFriendSuggestList(serachUUID, OnAckFriendSuggestList);
            }
        }
    }

    public static void OnAckFriendSuggestList(int result, PktMsgType pktMsg)
    {
        if (result != 0)
            return;

        if (!LobbyUIManager.Instance.IsActiveUI("FriendPopup"))
            LobbyUIManager.Instance.ShowUI("FriendPopup", true);
        else
            LobbyUIManager.Instance.Renewal("FriendPopup");
    }

    public static bool IsFriendRoom()
    {
        return GameInfo.Instance.FriendRoomUserUUID > 0;
    }

    public static bool GetFriendPointTakeCheck()
    {
        List<FriendUserData> pointTakeflagOnList = GameInfo.Instance.CommunityData.FriendList.FindAll(x => x.FriendPointTakeFlag);
        if (pointTakeflagOnList == null || pointTakeflagOnList.Count <= 0)
            return false;

        return true;
    }

    public static string GetServiceTypeWebAddr(string webviewAddr)
    {
        string str = webviewAddr;
        if (AppMgr.Instance.configData.m_ServiceType != Config.eServiceType.Japan)
        {
            str += GameInfo.Instance.GameConfig.WebLanguageAddrList[(int)FLocalizeString.Language];
        }        
        return str;
    }


    public static void OpenWebView(string webviewTitle, string webviewAddr)
    {
        if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
        {
            Application.OpenURL(webviewAddr);
        }
        else
        {
            UIValue.Instance.SetValue(UIValue.EParamType.WebViewTitle, webviewTitle);
            UIValue.Instance.SetValue(UIValue.EParamType.WebViewAddr, webviewAddr);
            LobbyUIManager.Instance.ShowUI("WebViewPopup", true);
        }
    }

    public static void OpenWebView_ServerRelocate(string webviewTitle, string webviewAddr)
    {
        if (AppMgr.Instance.configData == null || AppMgr.Instance.configData.m_ServiceType != Config.eServiceType.Japan)
            return;

        if (!AppMgr.Instance.HasContentFlag(eContentType.SERVER_RELOCATE))
            return;        

        if (!GameInfo.Instance.IsShowServerRelocateNotice)
        {
            GameInfo.Instance.IsShowServerRelocateNotice = true;
            OpenWebView(webviewTitle, webviewAddr);
        }
    }

    public static void IsShowBannerPopup(bool bnotice)
    {
        //List<BannerData> bannerdataList = GameInfo.Instance.ServerData.BannerList.FindAll(x => x.BannerType == (int)eBannerType.LOGIN_PACKAGE_BG && x.FunctionValue1 != string.Empty);
        System.DateTime curServerTime = GameInfo.Instance.GetNetworkTime();
        List<BannerData> bannerdataList = GameInfo.Instance.ServerData.BannerList.FindAll(x =>
            {
                if(x.BannerType == (int)eBannerType.LOGIN_PACKAGE_BG && x.FunctionValue1 != string.Empty)
                {
                    if((x.EndDate - curServerTime).TotalSeconds > 0)
                        return true;
                }
                return false;
            });

        if (null == bannerdataList || bannerdataList.Count <= 0) return;

        BannerPopup.ShowBannerPopup(bnotice);
        //LobbyUIManager.Instance.ShowUI("BannerPopup", true);
    }

    /// <summary>
    /// 1$ 짜리 특별 구매 팝업을 열수 있는 지 여부
    /// </summary>
    /// <returns></returns>
    public static bool IsShowSpecialBuyPopup()
    {
        //레벨 제한 체크
        if (GameInfo.Instance.UserData.Level >= GameInfo.Instance.GameConfig.SpecialBuyLimitLv)
            return false;

        //로비에 1시간 유지되는 UI표시 체크
        string remainTimeTickStr = PlayerPrefs.GetString(eSpecialLocalData.SPECIAL_BUY_POPUP_REMAIN_TIME.ToString(), "0");
        DateTime remainTime = new DateTime(long.Parse(remainTimeTickStr));
        if (remainTime.Ticks != 0)
            return false;

        DateTime nowTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        //팝업 오픈 체크 날짜
        string getTimeCheckTickStr = PlayerPrefs.GetString(eSpecialLocalData.SPECIAL_BUY_POPUP_OPEN_CHECK_DATE.ToString(), "0");
        DateTime getTime = new DateTime(long.Parse(getTimeCheckTickStr));

        //당일 스테이지 플레이 횟수 체크용 20회 이상일때 팝업 오픈
        int stagePlayCheckCnt = PlayerPrefs.GetInt(eSpecialLocalData.SPECIAL_BUY_POPUP_STAGE_PLAY_CNT.ToString(), 1);
        stagePlayCheckCnt++;

        //팝업 닫기 버튼으로 닫은 카운트
        int closeCnt = PlayerPrefs.GetInt(eSpecialLocalData.SPECIAL_BUY_POPUP_CLOSE_CNT.ToString(), 0);

        //이 함수에 들어온 날짜가 다르면 초기화
        if (getTime != nowTime)
        {
            //오픈체크, 스테이지 플레이 횟수 초기화
            PlayerPrefs.SetString(eSpecialLocalData.SPECIAL_BUY_POPUP_OPEN_CHECK_DATE.ToString(), nowTime.Ticks.ToString());
            PlayerPrefs.SetInt(eSpecialLocalData.SPECIAL_BUY_POPUP_STAGE_PLAY_CNT.ToString(), 1);

            getTimeCheckTickStr = nowTime.Ticks.ToString();
            getTime = new DateTime(long.Parse(getTimeCheckTickStr));

            stagePlayCheckCnt = 1;
        }

        //테이블에 상품이 있어도 운영툴 배너에 특별구매 이미지 등록이 안되어있으면 false
        BannerData bannerdata = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.SPECIAL_BUY);
        if (bannerdata == null)
            return false;

        int storeId = bannerdata.BannerTypeValue;

        //one doller 상품
        int oldStoreID = 2019;

        //구매횟수 확인
        //if (!IsStoreSaleApply(storeId))
        //    return false;
        if (!IsStoreSaleCheck(oldStoreID, storeId))
            return false;

        //각 챕터 - 쉬움 - 5스테이지 클리어 체크
        List<GameClientTable.Chapter.Param> chapterList = GameInfo.Instance.GameClientTable.Chapters;

        List<StageClearData> stageClearList = new List<StageClearData>();
        for (int i = 0; i < chapterList.Count; i++)
        {
            StageClearData clearData = GameInfo.Instance.StageClearList.Find(x => x.TableData.Chapter == chapterList[i].ID && x.TableData.StageType == (int)eSTAGETYPE.STAGE_MAIN_STORY &&
                                                                            x.TableData.Difficulty == GameInfo.Instance.GameConfig.SpecialCheckDifficulty && x.TableData.Section == GameInfo.Instance.GameConfig.SpecialCheckSection);
            if (clearData == null)
                continue;

            stageClearList.Add(clearData);
        }

        //새롭게 클리어한 스테이지가 메인 - 초급 - 5섹션 인지 확인 여부
        var clearStageId = UIValue.Instance.GetValue(UIValue.EParamType.FirstClearStageID, true);
        if (clearStageId != null)
        {
            StageClearData sectionCleatData = GameInfo.Instance.StageClearList.Find(x => x.TableData.ID == (int)clearStageId && x.TableData.StageType == (int)eSTAGETYPE.STAGE_MAIN_STORY
                                                                        && x.TableData.Difficulty == GameInfo.Instance.GameConfig.SpecialCheckDifficulty && x.TableData.Section == GameInfo.Instance.GameConfig.SpecialCheckSection);

            if (sectionCleatData != null)
                return true;
        }


        //이미 모든 챕터의 쉬움, 5스테이지를 클리어 한 유저
        if (chapterList.Count == stageClearList.Count)
        {
            PlayerPrefs.SetInt(eSpecialLocalData.SPECIAL_BUY_POPUP_STAGE_PLAY_CNT.ToString(), stagePlayCheckCnt);

            //스테이지 시도 횟수가 딱 SpecialBuyOpenStageCnt와 같을때만 열림
            if (stagePlayCheckCnt == GameInfo.Instance.GameConfig.SpecialBuyOpenStageCnt)
            {
                //구매 안하고 닫은 횟수 비교
                if (closeCnt >= GameInfo.Instance.GameConfig.SpecialBuyCloseCnt)
                {
                    string nextWeekStr = PlayerPrefs.GetString(eSpecialLocalData.SPECIAL_BUY_POPUP_NEXT_WEEK.ToString(), "0");
                    DateTime nextWeekOpenTime = new DateTime(long.Parse(nextWeekStr));

                    if (nextWeekOpenTime.Ticks <= nowTime.Ticks)
                    {
                        //일주일 이상 지났기 때문에 초기화
                        PlayerPrefs.SetString(eSpecialLocalData.SPECIAL_BUY_POPUP_NEXT_WEEK.ToString(), "0");
                        PlayerPrefs.SetInt(eSpecialLocalData.SPECIAL_BUY_POPUP_CLOSE_CNT.ToString(), 0);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    public static bool SpecialBuyPopupWithRemainTime()
    {
        //테이블에 상품이 있어도 운영툴 배너에 특별구매 이미지 등록이 안되어있으면 false
        BannerData bannerdata = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.SPECIAL_BUY);
        if (bannerdata == null)
            return false;

        //one doller 상품
        int oldStoreID = 2019;
        int storeId = bannerdata.BannerTypeValue;

        if (!IsStoreSaleCheck(oldStoreID, storeId))
            return false;

        //로비에 1시간 유지되는 UI표시 체크
        string remainTimeTickStr = PlayerPrefs.GetString(eSpecialLocalData.SPECIAL_BUY_POPUP_REMAIN_TIME.ToString(), "0");
        DateTime remainTime = new DateTime(long.Parse(remainTimeTickStr));
        if (remainTime.Ticks == 0)
            return false;

        DateTime nowTime = DateTime.Now;
        if (remainTime.Ticks > nowTime.Ticks)
            return true;

        //남은시간 초기화
        PlayerPrefs.SetString(eSpecialLocalData.SPECIAL_BUY_POPUP_REMAIN_TIME.ToString(), "0");

        //팝업 닫기 버튼으로 닫은 카운트 증가
        int closeCnt = PlayerPrefs.GetInt(eSpecialLocalData.SPECIAL_BUY_POPUP_CLOSE_CNT.ToString(), 0);
        PlayerPrefs.SetInt("SPECIAL_BUY_POPUP_CLOSE_CNT", closeCnt + 1);

        return false;
    }

    //피처드 땜에 임시로 적용(2.2.16 글로벌)
    public static bool IsEquipAndUsingCardData(long uid)
    {
        if (GameInfo.Instance.GetEquiCardFacilityData(uid) != null)
            return true;
        if (GameInfo.Instance.GetUsingDispatchData(uid) != null)
            return true;

        return false;
    }

    public static eREWARDTYPE IsRewardTypeWithStoreDisplayTable(GameClientTable.StoreDisplayGoods.Param data)
    {
        GameTable.Store.Param storeTableData = GameInfo.Instance.GameTable.FindStore(x => x.ID == data.StoreID);

        return (eREWARDTYPE)storeTableData.ProductType;
    }

    //월정액 상품을 구매 했는지 여부
    public static bool IsHaveMonthlyData(int tableID)
    {
        MonthlyData monthlyData = GameInfo.Instance.UserMonthlyData.MonthlyDataList.Find(x => x.MonthlyTableID == tableID);

        if (monthlyData == null)
            return false;

        DateTime nowTime = GetCurrentServerTime();
        TimeSpan timeSpan = monthlyData.MonthlyEndTime - nowTime;

        if (timeSpan.Ticks <= 0)
            return false;

        return true;
    }

    //프리미엄 월정액 상품 연장 노출 여부
    public static bool PremiumMonthlyDateFlag(int tableID)
    {
        if (!IsHaveMonthlyData(tableID))
            return false;

        MonthlyData monthlyData = GameInfo.Instance.UserMonthlyData.MonthlyDataList.Find(x => x.MonthlyTableID == tableID);

        DateTime nowTime = GetCurrentServerTime();
        TimeSpan timeFlag = monthlyData.MonthlyEndTime - nowTime;

        if (timeFlag.TotalMinutes > 0 && timeFlag.TotalMinutes < GameInfo.Instance.GameConfig.MonthlyFeeLimitMin)
            return true;

        return false;
    }

    //프리미엄 월정액 활성화 여부
    public static bool IsPremiumMonthlyFlag()
    {
        return IsHaveMonthlyData((int)eMonthlyType.PREMIUM);
    }

    //유저 버프

    public static bool UserBuffEndTimeFlag(BuffEffectData buffEffectData)
    {
        DateTime nowTime = GetCurrentServerTime();

        if (nowTime.Ticks < buffEffectData.BuffEndTime.Ticks)
            return true;

        return false;
    }

    public static eBuffEffectType GetBuffEffectType(int tableID)
    {
        BuffEffectData buffEffectData = GameInfo.Instance.UserBuffEffectData.BuffEffectDataList.Find(x => x.BuffTableID == tableID);
        if(buffEffectData == null)
            return eBuffEffectType.NONE;

        if (!UserBuffEndTimeFlag(buffEffectData))
            return eBuffEffectType.NONE;

        return buffEffectData.BuffEffectType;
    }

    public static BuffEffectData GetUserBuffEffectData(int tableID, int condition = 0)
    {
        BuffEffectData buffEffectData = GameInfo.Instance.UserBuffEffectData.BuffEffectDataList.Find(x => x.BuffTableID == tableID);
        if (buffEffectData == null)
            return null;

        if (!UserBuffEndTimeFlag(buffEffectData))
            return null;

        return buffEffectData;
    }

    public static BuffEffectData GetUserBuffEffectData(eBuffEffectType buffType, int condition = 0)
    {
        BuffEffectData buffEffectData;

        if (buffType == eBuffEffectType.Buff_CharExpAll)
        {
            buffEffectData = GameInfo.Instance.UserBuffEffectData.BuffEffectDataList.Find(x => x.TableData.UseType == 0 && x.BuffEffectType == buffType && x.TableData.Condition == condition);
        }
        else
        {
            buffEffectData = GameInfo.Instance.UserBuffEffectData.BuffEffectDataList.Find(x => x.TableData.UseType == 0 && x.BuffEffectType == buffType);
        }

        if (buffEffectData == null)
            return null;

        if (!UserBuffEndTimeFlag(buffEffectData))
            return null;

        return buffEffectData;

    }
    
    public static int GetFavorBuffValue(eBuffEffectType buffType)
    {
        if(GameInfo.Instance.UserData == null || GameInfo.Instance.UserData.ArrFavorBuffCharUid == null || GameInfo.Instance.UserBuffEffectData == null)
        {
            return 0;
        }

        int result = 0;
        
        List<BuffEffectData> buffList = GameInfo.Instance.UserBuffEffectData.BuffEffectDataList.FindAll(x => x.TableData.UseType == 1 && x.BuffEffectType == buffType);
        
        foreach (long uId in GameInfo.Instance.UserData.ArrFavorBuffCharUid )
        {
            if (uId <= 0)
            {
                continue;
            }

			CharData charData = GameInfo.Instance.GetCharData(uId);
			if(charData == null || charData.TableData == null)
			{
				continue;
			}

			int buffId = charData.TableData.PreferenceBuff;
            foreach(BuffEffectData buff in buffList)
            {
				if(buff == null || buff.TableData == null)
				{
					continue;
				}

                if (buffId == buff.BuffTableID)
                {
                    result += buff.TableData.Value;
                }
            }
        }
        
        return result;
    }
    
    public static bool IsActiveUserBuff(eBuffEffectType buffType, int condition = 0)
    {
        if (buffType == eBuffEffectType.NONE)
            return false;

        BuffEffectData buffEffectData = GetUserBuffEffectData(buffType, condition);
        if (buffEffectData == null)
            return false;

        return UserBuffEndTimeFlag(buffEffectData);
    }

    public static int GetActiveUserBuffEffect()
    {
        if (GameInfo.Instance.UserBuffEffectData.BuffEffectDataList.Count <= (int)eCOUNT.NONE)
            return 0;

        int result = 0;

        for (int i = 0; i < GameInfo.Instance.UserBuffEffectData.BuffEffectDataList.Count; i++)
        {
            BuffEffectData data = GameInfo.Instance.UserBuffEffectData.BuffEffectDataList[i];
            if (data.TableData.UseType == 0 && IsActiveUserBuff(data.BuffEffectType, data.TableData.Condition))
            {
                result++;
            }
            else
            {
                if (data.TableData.UseType < 1)
                {
                    GameInfo.Instance.UserBuffEffectData.BuffEffectDataList.RemoveAt(i);        //버프 종료된것 제거
                }
                else
                {
                    foreach (long uId in GameInfo.Instance.UserData.ArrFavorBuffCharUid)
                    {
                        if (uId <= 0)
                        {
                            continue;
                        }

                        int buffId = GameInfo.Instance.GetCharData(uId).TableData.PreferenceBuff;
                        if (buffId == data.BuffTableID)
                        {
                            result++;
                            break;
                        }
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 시간이 지난 버프 정리(삭제)
    /// </summary>
    public static void RemoveTimeOverUserBuffEffect()
    {
        Stack<int> rmStack = new Stack<int>();
        
        for (int i = 0; i < GameInfo.Instance.UserBuffEffectData.BuffEffectDataList.Count; i++)
        {
            BuffEffectData data = GameInfo.Instance.UserBuffEffectData.BuffEffectDataList[i];

            if (!UserBuffEndTimeFlag(data))
                rmStack.Push(i);
        }

        foreach (int number in rmStack)
        {
            GameInfo.Instance.UserBuffEffectData.BuffEffectDataList.RemoveAt(number);
        }
    }
        

    public static bool GetCharLastSkillSlotCheck(CharData chardata)
    {
        bool flag = false;

        if (GameSupport.IsActiveUserBuff(eBuffEffectType.Buff_SkillSlot))       //스킬슬롯 버프 진행중
        {
            int exSkill = (chardata.TableID * 1000) + 301;

            if (chardata.EquipSkill[(int)eCOUNT.SKILLSLOT - 1] == exSkill)
            {
                flag = true;
            }
        }
        else
        {
            if (chardata.EquipSkill[(int)eCOUNT.SKILLSLOT - 1] != (int)eCOUNT.NONE)
            {
                flag = true;
            }
        }

        return flag;
    }

    public static int IsWeaponEquipGemCount(long weaponUID)
    {
        int result = 0;

        for (int i = 0; i < GameInfo.Instance.GemList.Count; i++)
        {
            WeaponData tempWeapon = GameInfo.Instance.GetEquipGemWeaponData(GameInfo.Instance.GemList[i].GemUID);
            if (tempWeapon == null)
            {
                result++;
            }
            else
            {
                if (tempWeapon.WeaponUID == weaponUID)
                    result++;
            }
        }

        return result;
    }

    //자식 중에 서브 웨폰 부모 찾기
    public static Transform GetSubWeaponBoneOrNull(Transform parent, bool isLeft, int idx = 1)
    {
        string subWeaponBoneName = string.Format("sub_weapon_{0}_{1}", isLeft ? "L" : "R", idx);

        Transform[] trs = parent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < trs.Length; i++)
        {
            Transform tr = trs[i];
            if (tr == null || !tr.name.Equals(subWeaponBoneName))
            {
                continue;
            }

            return tr;
        }

        return null;
    }

    public static bool IsLockArena()
    {
        if (GameInfo.Instance.GameConfig.TestMode)
            return false;

        if (GameInfo.Instance.UserData.Level < GameInfo.Instance.GameConfig.ArenaOpenRank)
        {
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LOCKMSG), GameInfo.Instance.GameConfig.ArenaOpenRank));
            return true;
        }

        if (GameInfo.Instance.UserBattleData.Now_GradeId == (int)eCOUNT.NONE)
        {
            //아레나 오픈 레벨은 됬지만, 아레나 플레이를 한번도 하지 않음.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3217));     //아레나 대전에 참전한 기록이 없습니다. 
            return true;
        }

        return false;
    }

    public static bool IsLockArenaTower()
    {
        if (AppMgr.Instance.Review || GameInfo.Instance.GameConfig.TestMode)
            return false;

        if (GameInfo.Instance.UserData.Level < GameInfo.Instance.GameConfig.ArenaOpenRank)
        {
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LOCKMSG), GameInfo.Instance.GameConfig.ArenaOpenRank));
            return true;
        }
        if (GameInfo.Instance.UserData.ArenaPrologueValue < 100)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1456));
            return true;
        }
        return false;
    }

    public static float GetTotalCardFormationEffectValue()
    {
        List<GameTable.CardFormation.Param> cardFormationList = GameInfo.Instance.GameTable.CardFormations;

        float result = 0f;

        for (int i = 0; i < cardFormationList.Count; i++)
        {
            bool newFlag = true;
            bool levelFlag = true;
            bool favorFlag = true;

            GetAddCardFormationEffectValue(cardFormationList[i].CardID1, ref newFlag, ref levelFlag, ref favorFlag);
            GetAddCardFormationEffectValue(cardFormationList[i].CardID2, ref newFlag, ref levelFlag, ref favorFlag);
            GetAddCardFormationEffectValue(cardFormationList[i].CardID3, ref newFlag, ref levelFlag, ref favorFlag);

            if (newFlag)
            {
                result += cardFormationList[i].GetHP;
                if (levelFlag)
                    result += cardFormationList[i].LevelHP;
                if (favorFlag)
                    result += cardFormationList[i].FavorHP;
            }
                
            
        }

        return result;
    }

    public static void GetAddCardFormationEffectValue(int cardTableID, ref bool newFlag, ref bool levelFlag, ref bool favorFlag)
    {
        //float result = 0f;

        if (cardTableID == (int)eCOUNT.NONE)
            return;

		/*
        GameTable.Card.Param cardTableData = GameInfo.Instance.GameTable.FindCard(x => x.ID == cardTableID);
        if (cardTableData == null)
            return;

        GameClientTable.Book.Param clientBook = GameInfo.Instance.GameClientTable.FindBook(x => x.Group == (int)eBookGroup.Supporter && x.ItemID == cardTableID);
        if (clientBook == null)
        {
            Debug.LogError(cardTableID + " CardClient id NULL");
            return;
        }
		*/

        CardBookData cardbookdata = GameInfo.Instance.CardBookList.Find(x => x.TableID == cardTableID);
        if (cardbookdata == null)
            newFlag = false;
        else
        {
            bool bLevel = cardbookdata.IsOnFlag(eBookStateFlag.MAX_WAKE_AND_LV);
            bool bFavor = cardbookdata.IsOnFlag(eBookStateFlag.MAX_FAVOR_LV);

            if (!bLevel)
                levelFlag = false;
            if (!bFavor)
                favorFlag = false;
        }

        return;
    }

    public static int GetSelectCardFormationID()
    {
        int cardFormationID = 0;

        var cardFmId = UIValue.Instance.GetValue(UIValue.EParamType.CardFormationType);
        if (cardFmId != null)
        {
            eCharSelectFlag cardForIDType = (eCharSelectFlag)cardFmId;

            if (cardForIDType == eCharSelectFlag.ARENA)
            {
                cardFormationID = GameInfo.Instance.UserData.ArenaCardFormationID;
            }
            else if (cardForIDType == eCharSelectFlag.ARENATOWER)
            {
                cardFormationID = GameInfo.Instance.UserData.ArenaTowerCardFormationID;
            }
            else if( cardForIDType == eCharSelectFlag.RAID ) {
                cardFormationID = GameInfo.Instance.RaidUserData.CardFormationId;
            }
            else
            {
                cardFormationID = GameInfo.Instance.UserData.CardFormationID;
            }

        }
        else
        {
            cardFormationID = GameInfo.Instance.UserData.CardFormationID;
        }

        return cardFormationID;
    }

    //무기고 해당 세트옵션에 해당하는 무기 장착 수
    public static int GetEquipArmoryWeaponDepot(int wpnDepotGroupID)
    {
        int result = 0;

        for (int i = 0; i < GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList.Count; i++)
        {
            WeaponData weaponData = GameInfo.Instance.GetWeaponData(GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList[i]);
			string[] depotFlag = Utility.Split(weaponData.TableData.WpnDepotSetFlag, ','); //weaponData.TableData.WpnDepotSetFlag.Split(',');
			for (int j = 0; j < depotFlag.Length; j++)
            {
                if (depotFlag[j].Equals(wpnDepotGroupID.ToString()))
                {
                    result++;
                }
            }
        }

        return result;
    }

    //해당 무기가 해당 옵션에 해당하는지 여부
    public static bool GetArmoryWeaponDepotFlagCheck(int wpnDepotGroupID, int weaponTableID)
    {
        GameTable.Weapon.Param weaponTableData = GameInfo.Instance.GameTable.FindWeapon(x => x.ID == weaponTableID);
        if (weaponTableData == null)
            return false;

		string[] depotFlag = Utility.Split(weaponTableData.WpnDepotSetFlag, ','); //weaponTableData.WpnDepotSetFlag.Split(',');
		for (int i = 0; i < depotFlag.Length; i++)
        {
            if (depotFlag[i].Equals(wpnDepotGroupID.ToString()))
            {
                return true;
            }
        }

        return false;
    }

    public static bool GetEquipWeaponDepot(long weaponUID)
    {
        WeaponData weapondata = GameInfo.Instance.GetWeaponData(weaponUID);
        if (weapondata == null)
            return false;

        for (int i = 0; i < GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList.Count; i++)
        {
            if (GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList[i] == weaponUID)
            {
                return true;
            }
        }
        return false;
    }

    public static bool GetEquipWeaponDepotTableID(int weaponTableID)
    {
        for (int i = 0; i < GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList.Count; i++)
        {
            WeaponData weapondata = GameInfo.Instance.GetWeaponData(GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList[i]);
            if (weapondata == null)
                continue;

            if (weapondata.TableID == weaponTableID)
                return true;
        }
        return false;
    }

    public static bool GetEquipWeaponDepotWeaponUID(long weaponUID)
    {
        WeaponData weapondata = GameInfo.Instance.GetWeaponData(weaponUID);
        if (weapondata == null)
            return false;


        for (int i = 0; i < GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList.Count; i++)
        {
            WeaponData armoryWeapon = GameInfo.Instance.GetWeaponData(GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList[i]);
            if (weapondata == null)
                continue;

            if (weapondata.TableID == armoryWeapon.TableID)
                return true;
        }
        return false;
    }

    public static float GetWeaponDepotEffectValue(int wpnDepotGroupID)
    {
        float result = 0;

        int equipCnt = GetEquipArmoryWeaponDepot(wpnDepotGroupID);

        List<GameClientTable.WpnDepotSet.Param> wpnList = GameInfo.Instance.GameClientTable.FindAllWpnDepotSet(x => x.GroupID == wpnDepotGroupID);

        for (int i = 0; i < wpnList.Count; i++)
        {
            if (wpnList[i].ReqCnt <= equipCnt)
                result += wpnList[i].BonusATK;
        }

        return result;
    }

    public static float GetTotalWeaponDepotEffectValue()
    {
        float result = 0f;

        for (int i = 0; i < GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList.Count; i++)
        {
            WeaponData weapondata = GameInfo.Instance.GetWeaponData(GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList[i]);
            if (weapondata == null)
                continue;

            //GameConfig_Balance.WpnDepotBaseRatio[무기 등급] + (무기 Lv * GameConfig_Balance.WpnDepotLvRatio[무기 등급]) (0.1=0.1%)
            result += GameInfo.Instance.GameConfig.WpnDepotBaseRatio[weapondata.TableData.Grade] + (weapondata.Level * GameInfo.Instance.GameConfig.WpnDepotLvRatio[weapondata.TableData.Grade]);
        }

        List<GameClientTable.WpnDepotSet.Param> wpnDepotList = new List<GameClientTable.WpnDepotSet.Param>();
        for (int i = 0; i < GameInfo.Instance.GameClientTable.WpnDepotSets.Count; i++)
        {
            if (wpnDepotList.Find(x => x.GroupID == GameInfo.Instance.GameClientTable.WpnDepotSets[i].GroupID) == null)
                wpnDepotList.Add(GameInfo.Instance.GameClientTable.WpnDepotSets[i]);
        }

        for (int i = 0; i < wpnDepotList.Count; i++)
            result += GetWeaponDepotEffectValue(wpnDepotList[i].GroupID);

        return result;
    }


    /// <summary>
    /// 아레나 타워 팀에 배치된지 검사
    /// </summary>
    /// <param name="charuid"></param>
    /// <returns></returns>
    public static List<long> ArenaTowerCharChange(long charuid, int slotNum)
    {
        List<long> _teamCharList = new List<long>();
        for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
            _teamCharList.Add(GameInfo.Instance.TowercharList[i]);

        //이미 아레나 팀에 배치된 캐릭터인지 검사
        bool beTeam = false;
        if (charuid > 0)
        {
            for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
            {
                if (_teamCharList[i].Equals(charuid))
                {
                    beTeam = true;
                    break;
                }
            }
        }

        if (beTeam)      //이미 배치된 캐릭터 교체 or 해제
        {
            if (_teamCharList[slotNum] == charuid)
            {
                //해제
                _teamCharList[slotNum] = (int)eCOUNT.NONE;
            }
            else
            {
                //교체 or 장착
                if (_teamCharList[slotNum] != (int)eCOUNT.NONE)
                {
                    for (int i = 0; i < _teamCharList.Count; i++)
                    {
                        if (_teamCharList[i] == charuid)
                        {
                            long temp = _teamCharList[slotNum];
                            _teamCharList[slotNum] = charuid;
                            _teamCharList[i] = temp;
                            break;
                        }
                    }
                }
                else
                {
                    //해제
                    for (int i = 0; i < _teamCharList.Count; i++)
                    {
                        if (_teamCharList[i].Equals(charuid))
                        {
                            _teamCharList[i] = (int)eCOUNT.NONE;
                            break;
                        }
                    }
                    _teamCharList[slotNum] = charuid;
                }
            }
        }
        else
        {
            _teamCharList[slotNum] = charuid;
        }

        return _teamCharList;
    }

    public static void ShowCharSeletePopupArenaTower(int slotIdx, eCharSelectFlag _charSelectFlag)
    {
        if (IsLockArena()) return;

        UIValue.Instance.SetValue(UIValue.EParamType.ArenaTowerTeamCharSlot, (int)slotIdx);
        UIValue.Instance.SetValue(UIValue.EParamType.CharSeletePopupType, (int)_charSelectFlag);
        //LobbyUIManager.Instance.ShowUI("CharSeletePopup", true);
        LobbyUIManager.Instance.ShowUI("ArenaCharSeletePopup", true);
    }

    public static bool IsFriend(long uid)
    {
        if(uid == 0)
        {
            return false;
        }

        return GameInfo.Instance.GetCharData(uid) == null;
    }

    public static List<long> GetArenaTowerTeamCharList()
    {
        List<long> _towerTeamCharList = new List<long>();
        var iter = GameInfo.Instance.TowercharList.GetEnumerator();
        while (iter.MoveNext())
        {
            _towerTeamCharList.Add(iter.Current);
        }

        //메모리에 임시 저장되어 있는 친구 캐릭터 ID를 합쳐준다.
        for (int i = 0; i < _towerTeamCharList.Count; i++)
        {
            long v = 0;
            if (GameInfo.Instance.ArenaTowerFriendContainer.TryGetValue(i, out v))
            {
                if (v != 0)
                    _towerTeamCharList[i] = v;
            }
        }

        return _towerTeamCharList;
    }

    public static List<AllyPlayerData> GetAllyPlayerDataList()
    {
        List<AllyPlayerData> ListAllyPlayerData = new List<AllyPlayerData>();

        var iter = GameInfo.Instance.TowercharList.GetEnumerator();
        while (iter.MoveNext())
        {
            CharData charData = GetCharDataByUidOrNull(iter.Current);
            ListAllyPlayerData.Add(new AllyPlayerData(charData, null));
        }

        //메모리에 임시 저장되어 있는 친구 캐릭터 ID를 합쳐준다.
        for (int i = 0; i < ListAllyPlayerData.Count; i++)
        {
            long v = 0;
            if (GameInfo.Instance.ArenaTowerFriendContainer.TryGetValue(i, out v))
            {
                if (v != 0)
                {
                    for(int j = 0; j < GameInfo.Instance.TowerFriendTeamData.Count; j++)
                    {
                        TeamCharData teamCharData = GameInfo.Instance.TowerFriendTeamData[j].charlist.Find(x => x.CharData.CUID == v);
                        if(teamCharData != null)
                        {
                            ListAllyPlayerData[i].FriendCharData = teamCharData;
                        }
                    }
                }
            }
        }

        return ListAllyPlayerData;
    }

    public static bool ArenaTowerTeamCheckFlag()
    {
        List<long>  teamlist = GetArenaTowerTeamCharList();
        //대장 슬롯이 비었을때
        if (teamlist[(int)eArenaTeamSlotPos.LAST_POS] == (int)eCOUNT.NONE)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3161));
            return false;
        }

        if (teamlist[(int)eArenaTeamSlotPos.START_POS] != (int)eCOUNT.NONE &&
            teamlist[(int)eArenaTeamSlotPos.MID_POS] == (int)eCOUNT.NONE)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3162));
            return false;
        }


        return true;
    }

    public static GameTable.InfluenceMissionSet.Param GetCurrentInfluenceMissionSet(bool isRewardTime = false)
    {
        var list = GameInfo.Instance.GameTable.InfluenceMissionSets;
        for (int i = 0; i < list.Count; i++) 
        {
            TimeSpan remainStartTime = GetRemainTime(GetTimeWithString(list[i].StartTime));            
            TimeSpan remainEndTime = GetRemainTime(GetTimeWithString(list[i].EndTime, true));
            if(isRewardTime)
                remainEndTime = GetRemainTime(GetTimeWithString(list[i].RewardTime, true));

            if (remainStartTime.TotalSeconds <= 0 && remainEndTime.TotalSeconds >= 0)
            {
                return list[i];
            }
        }
        return null;
    }

    public static int GetCurrentInfluenceEventItemID()
    {
        GameTable.InfluenceMissionSet.Param param = GetCurrentInfluenceMissionSet(true);
        if (param != null) return param.EventItemID;
        return 0;
    }

    public static bool IsMaxEnchantLevel(int EnchantGroupID, int CurLv)
    {
        if (CurLv <= 0) return false;

        var _EnchantTable = GameInfo.Instance.GameTable.Enchants.Find(x => x.GroupID == EnchantGroupID && x.Level == CurLv + 1);
        if (_EnchantTable == null) return true;

        return false;
    }   

    public static bool IsHowToBeStrongNotice(CharData CharacterData, GameClientTable.HowToBeStrong.Param Param)
    {
        if (CharacterData == null) return false;
        if (Param == null) return false;

        switch (Param.Group)
        {
            case 1:
                {
                    if (Param.Index == 1)
                    {
                        //캐릭터 진급
                        if (IsCharGradeUp(CharacterData))
                        {
                            return CharacterData.Grade < GameInfo.Instance.GameConfig.CharStartAwakenGrade;
                        }
                    }
                    else if (Param.Index == 2)
                    {
                        //캐릭터 스킬 패시브 스킬 강화 가능, 스킬 획득 가능
                        return (IsCharSkillUp(CharacterData) || IsCharPassiveSkillUp(CharacterData));                            
                    }
                    else if (Param.Index == 3)
                    {
                        //캐릭터 각성
                        return IsCharAwakenUp(CharacterData);
                    }
                }
                break;
            case 2:
                {
                    if (Param.Index == 0)
                    {
                        //서포터 강화/각성
                        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
                        {
                            var carddata = GameInfo.Instance.GetCardData(CharacterData.EquipCard[i]);
                            if (carddata == null) continue;
                            var list = GameInfo.Instance.ItemList.FindAll(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_CARD_LEVELUP);
                            if (list.Count != 0 && !IsMaxLevelCard(carddata) || IsCardWakeUp(carddata))                            
                                return true;                            
                        }
                    }
                    else if (Param.Index == 1)
                    {
                        //서포터 스킬 강화
                        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
                        {
                            var carddata = GameInfo.Instance.GetCardData(CharacterData.EquipCard[i]);
                            if (carddata == null) continue;
                            if (IsCardSkillUp(carddata))                            
                                return true;                            
                        }
                    }
                    else if (Param.Index == 4)
                    {
                        //서포터 인챈트
                        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
                        {
                            var carddata = GameInfo.Instance.GetCardData(CharacterData.EquipCard[i]);
                            if (carddata == null) continue;
                            if (IsPossibleEnchant(carddata.TableData.EnchantGroup, carddata.EnchantLv))
                                return true;
                        }
                    }
                }
                break;
            case 3:
                {
                    var _weapondata = GameInfo.Instance.GetWeaponData(CharacterData.EquipWeaponUID);
                    if (_weapondata == null)
                        break;

                    if (Param.Index == 0)
                    {
                        //무기 강화
                        var list = GameInfo.Instance.ItemList.FindAll(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_WEAPON_LEVELUP);
                        if (list.Count != 0 && !IsMaxLevelWeapon(_weapondata))
                            return true;

                    }
                    else if (Param.Index == 1)
                    {
                        //무기 제련
                        if (IsWeaponWakeUp(_weapondata))
                            return true;
                    }
                    else if (Param.Index == 2)
                    {
                        //무기 스킬 강화
                        if (IsWeaponSkillUp(_weapondata))
                            return true;
                    }
                    else if (Param.Index == 4)
                    {
                        //무기 인챈트
                        if (IsPossibleEnchant(_weapondata.TableData.EnchantGroup, _weapondata.EnchantLv))
                            return true;
                    }
                }
                break;
            case 4:
                {
                    var _weapondata = GameInfo.Instance.GetWeaponData(CharacterData.EquipWeaponUID);
                    if (_weapondata == null)
                        break;

                    for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
                    {
                        if (_weapondata.SlotGemUID[i] == (int)eCOUNT.NONE)
                            continue;

                        GemData gemdata = GameInfo.Instance.GetGemData(_weapondata.SlotGemUID[i]);
                        if (gemdata == null) continue;

                        if (Param.Index == 0)
                        {
                            //곡옥 강화
                            if (IsGemLevelUp() && !IsMaxLevelGem(gemdata))
                                return true;
                        }
                        else if (Param.Index == 1)
                        {
                            //곡옥 연마
                            if (IsGemWakeUp(gemdata))
                                return true;
                        }
                        else if (Param.Index == 2)
                        {
                            //재설정
                            if (IsGemOptChange(gemdata))
                                return true;
                        }
                    }
                }
                break;
        }

        return false;
    }

    public static bool IsPossibleEnchant(int EnchatnGroupID, int CurLevel)
    {
        //레벨 검사
        var _EnchantTable = GameInfo.Instance.GameTable.Enchants.Find(x => x.GroupID == EnchatnGroupID && x.Level == CurLevel);
        if (_EnchantTable == null) return false;


        //재료 보유 검사
        var matItemReqList = GameInfo.Instance.GameTable.ItemReqLists.FindAll(x => x.Group == _EnchantTable.ItemReqListID);
        if (matItemReqList == null || matItemReqList.Count <= 0) return false;

        System.Func<int, int, bool> FuncEnoughMaterial = (itemid, needCnt) =>
        {
            int curCnt = GameInfo.Instance.GetItemIDCount(itemid);

            if (curCnt < needCnt)
                return false;

            return true;
        };

        for (int i = 0; i < matItemReqList.Count; i++)
        {
            if (matItemReqList[i].ItemID1 > 0)
                if (!FuncEnoughMaterial(matItemReqList[i].ItemID1, matItemReqList[i].Count1)) 
                    return false;

            if (matItemReqList[i].ItemID2 > 0)
                if (!FuncEnoughMaterial(matItemReqList[i].ItemID2, matItemReqList[i].Count2))
                    return false;

            if (matItemReqList[i].ItemID3 > 0)
                if (!FuncEnoughMaterial(matItemReqList[i].ItemID3, matItemReqList[i].Count3))
                    return false;

            if (matItemReqList[i].ItemID4 > 0)
                if (!FuncEnoughMaterial(matItemReqList[i].ItemID4, matItemReqList[i].Count4))
                    return false;

        }
        return true;
    }

    public static bool HasStoreProduct(eREWARDTYPE productType, int productIndex)
    {
        if (productType == eREWARDTYPE.COSTUME)
        {
            if (GameInfo.Instance.HasCostume(productIndex))
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(111404));
                return true;
            }
        }
        else if (productType == eREWARDTYPE.CHAR)
        {
            var chardata = GameInfo.Instance.CharList.Find(x => x.TableID == productIndex);
            if (chardata != null)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(111404));
                return true;
            }
        }

        return false;
    }

    public static int CompareWithFComponentDepth(FComponent lhs, FComponent rhs)
    {
        int lDepth = lhs.GetPanelDepth();
        int rDepth = rhs.GetPanelDepth();

        if(lDepth < rDepth)
        {
            return 1;
        }
        else if(rDepth > lDepth)
        {
            return -1;
        }

        return 0;
    }

    public static eEventTarget GetWelcomeEventState()
    {
        if (GameInfo.Instance.DailyMissionData == null || GameInfo.Instance.DailyMissionData.Infos == null || GameInfo.Instance.DailyMissionData.Infos.Count <= 0)
            return eEventTarget.NONE;

        //첫번째 Game.DailyMissionSet 정보를 얻어온다.
        var _DailyMissionSetParam = GameInfo.Instance.GameTable.DailyMissionSets.Find(x => x.ID == GameInfo.Instance.DailyMissionData.Infos[0].GroupID);
        if (_DailyMissionSetParam == null)
        {
            return eEventTarget.NONE;
        }

        return (eEventTarget)_DailyMissionSetParam.EventTarget;
    }

    public static bool HasDailyEventState(eEventTarget target)
    {
        List<GameTable.DailyMissionSet.Param> dailyMissionSetDataList = GameInfo.Instance.GameTable.FindAllDailyMissionSet(x => x.EventTarget == (int)target);

        if (null == dailyMissionSetDataList || dailyMissionSetDataList.Count <= (int)eCOUNT.NONE)
            return false;

        if (GameInfo.Instance.DailyMissionData == null || GameInfo.Instance.DailyMissionData.Infos == null || GameInfo.Instance.DailyMissionData.Infos.Count <= 0)
            return false;

        System.DateTime curTime = GameInfo.Instance.GetNetworkTime();

        bool bShow = false;

        for (int i = 0; i < dailyMissionSetDataList.Count; i++)
        {
            if (HasDailyEventPlayPossibleFlag(dailyMissionSetDataList[i]))
            {
                bShow = true;
                if (!DailyEventLastDayCheck(dailyMissionSetDataList[i]))
                {
                    bShow = IsValidDailyEventState(target);
                }

                if (bShow)
                {
                    break;
                }
                
            }
        }

        return bShow;
    }

    public static bool HasDailyEventState(GameTable.DailyMissionSet.Param missionSetData)
    {
        if (GameInfo.Instance.DailyMissionData == null || GameInfo.Instance.DailyMissionData.Infos == null || GameInfo.Instance.DailyMissionData.Infos.Count <= 0)
            return false;

        System.DateTime curTime = GameInfo.Instance.GetNetworkTime();

        bool bShow = false;

        if (HasDailyEventPlayPossibleFlag(missionSetData))
        {
            bShow = true;
            if (!DailyEventLastDayCheck(missionSetData))
            {
                bShow = IsValidDailyEventState(missionSetData);
            }
        }

        return bShow;
    }

    public static bool DailyEventLastDayCheck(GameTable.DailyMissionSet.Param missionSetData)
    {
        List<DailyMissionData.Piece> missionDataList = GameInfo.Instance.DailyMissionData.Infos.FindAll(x => x.GroupID == missionSetData.ID);

        if (null == missionDataList || missionDataList.Count <= (int)eCOUNT.NONE)
            return false;

        System.DateTime curTime = GameInfo.Instance.GetNetworkTime();

        if (missionDataList[missionDataList.Count - 1].EndTime < curTime)
            return false;

        return true;
    }

    public static bool HasDailyEventPlayPossibleFlag(GameTable.DailyMissionSet.Param missionSetData)
    {
        List<DailyMissionData.Piece> missionDataList = GameInfo.Instance.DailyMissionData.Infos.FindAll(x => x.GroupID == missionSetData.ID);

        if (null == missionDataList || missionDataList.Count <= (int)eCOUNT.NONE)
            return false;

        System.DateTime curTime = GameInfo.Instance.GetNetworkTime();
        if (missionDataList[(int)eCOUNT.NONE].StartTime > curTime)
            return false;

        //if (missionDataList[missionDataList.Count - 1].EndTime < curTime)
        //    return false;
       
        return !IsAllReceivedReward(missionDataList);
    }

    public static bool IsAllReceivedReward(List<DailyMissionData.Piece> missionDataList)
    {
        bool IsAllReceived = true;

        for (int i = 0; i < missionDataList.Count; i++)
        {
            DailyMissionData.Piece dailyMissinoData = missionDataList[i];

            List<GameTable.DailyMission.Param> missionList = GameInfo.Instance.GameTable.FindAllDailyMission(x => x.GroupID == dailyMissinoData.GroupID && x.Day == dailyMissinoData.Day);
            if (null == missionList || missionList.Count <= (int)eCOUNT.NONE)
                break;

            for (int j = 0; j < missionList.Count; j++)
            {
                if (!IsComplateMissionRecive(dailyMissinoData.RwdFlag, j))
                {
                    IsAllReceived = false;
                    break;
                }
            }

            if (IsAllReceived && !IsComplateMissionRecive(dailyMissinoData.RwdFlag, (int)PktInfoMission.Daily.Piece.ENUM.DAY_1))
                IsAllReceived = false;

            if(IsAllReceived && dailyMissinoData.Day == (int)eCOUNT.NONE + 1 && !IsComplateMissionRecive(dailyMissinoData.RwdFlag, (int)PktInfoMission.Daily.Piece.ENUM.COM_1))
                IsAllReceived = false;

            if (!IsAllReceived)
                break;
        }

        if (IsAllReceived)
        {
            DailyMissionData.Piece lastDayData = missionDataList[missionDataList.Count - 1];
            if (null != lastDayData)
            {
                System.DateTime curTime = GameInfo.Instance.GetNetworkTime();

                if (lastDayData.EndTime < curTime)
                    IsAllReceived = false;
            }
        }

        return IsAllReceived;
    }

    public static bool IsValidDailyEventState(eEventTarget eTarget)
    {
        List<GameTable.DailyMissionSet.Param> dailyEventSetList = GameInfo.Instance.GameTable.FindAllDailyMissionSet(x => x.EventTarget == (int)eTarget);
        if (null == dailyEventSetList || dailyEventSetList.Count <= (int)eCOUNT.NONE)
            return false;

        if (GameInfo.Instance.DailyMissionData == null || GameInfo.Instance.DailyMissionData.Infos == null || GameInfo.Instance.DailyMissionData.Infos.Count <= 0)
            return false;

        bool state = false;

        for (int i = 0; i < dailyEventSetList.Count; i++)
        {
            state = IsValidDailyEventState(dailyEventSetList[i]);
            if (state)
                break;
        }

        return state;
    }

    public static bool IsValidDailyEventState(GameTable.DailyMissionSet.Param missionSetData)
    {
        if (GameInfo.Instance.DailyMissionData == null || GameInfo.Instance.DailyMissionData.Infos == null || GameInfo.Instance.DailyMissionData.Infos.Count <= 0)
            return false;

        List<DailyMissionData.Piece> missionList = GameInfo.Instance.DailyMissionData.Infos.FindAll(x => x.GroupID == missionSetData.ID);
        if (null == missionList || missionList.Count <= (int)eCOUNT.NONE)
            return false;

        bool state = false;

        for (int i = 0; i < missionList.Count; i++)
        {
            List<GameTable.DailyMission.Param> dailyMission = GameInfo.Instance.GameTable.FindAllDailyMission(x => x.GroupID == missionList[(int)eCOUNT.NONE].GroupID && x.Day == missionList[i].Day);

            if (null == dailyMission || dailyMission.Count <= (int)eCOUNT.NONE)
                return false;

            for (int j = 0; j < dailyMission.Count; j++)
            {
                if (missionList[i].NoVal[j] == (int)eCOUNT.NONE && !GameSupport.IsComplateMissionRecive(missionList[i].RwdFlag, j))
                {
                    state = true;
                    break;
                }
            }
            if (state) break;
        }

        // n일차 최종 보상 확인
        if (!state)
        {
            for (int i = 0; i < missionList.Count; i++)
            {
                if (GameSupport.IsComplateMissionRecive(missionList[i].RwdFlag, (int)PktInfoMission.Daily.Piece.ENUM.DAY_1))
                    continue;

                bool isAllZero = true;
                for (int j = 0; j < missionList[i].NoVal.Length; j++)
                {
                    if (missionList[i].NoVal[j] > (int)eCOUNT.NONE)
                    {
                        isAllZero = false;
                        break;
                    }
                }
                if (isAllZero)
                {
                    state = true;
                    break;
                }
            }
        }

        // 마지막 최종 보상 확인
        if (!state)
        {
            bool isReceiveLastReward = true;

            bool lastReward = GameSupport.IsComplateMissionRecive(missionList[(int)eCOUNT.NONE].RwdFlag, (int)PktInfoMission.Daily.Piece.ENUM.DAY_1);
            if (!lastReward)
            {
                bool flag = true;
                for (int j = 0; j < missionList.Count; j++)
                {
                    flag &= GameSupport.IsComplateMissionRecive(missionList[j].RwdFlag, (int)PktInfoMission.Daily.Piece.ENUM.DAY_1);
                    if (!flag)
                        break;
                }

                isReceiveLastReward &= flag;
                state = isReceiveLastReward;
            }
        }

        return state;
    }

    //월정액 종료 체크
    public static bool IsEndCheckMonthly(int monthlyType = (int)eMonthlyType.PREMIUM)
    {
        MonthlyData monthlydata = GameInfo.Instance.UserMonthlyData.MonthlyDataList.Find(x => x.MonthlyTableID == monthlyType);
        if (monthlydata == null)
            return false;

        //
        if (GameInfo.Instance.UserData.PrevLoginTime < GameInfo.Instance.UserData.LoginBonusRecentDate)
        {
            DateTime prevLoginTime = GetLocalTimeByServerTime(GameInfo.Instance.UserData.PrevLoginTime);
            DateTime prevCheckTime = new DateTime(prevLoginTime.Year, prevLoginTime.Month, prevLoginTime.Day, 0, 0, 0);

            DateTime nowLoginTime = GetLocalTimeByServerTime(GameInfo.Instance.UserData.LoginBonusRecentDate);
            DateTime nowCheckTime = new DateTime(nowLoginTime.Year, nowLoginTime.Month, nowLoginTime.Day, 0, 0, 0);

            DateTime addday = monthlydata.MonthlyEndTime.AddDays(1);
            DateTime monthNextDay = new DateTime(addday.Year, addday.Month, addday.Day, 0, 0, 0);

            if (prevCheckTime < monthNextDay && monthNextDay <= nowCheckTime)
                return true;
        }

        return false;
    }

    //현재 DailyEvent Day 얻기
    public static int GetCurrentDailyEventDay(eEventTarget target)
    {
        if (GameInfo.Instance.DailyMissionData == null || GameInfo.Instance.DailyMissionData.Infos == null || GameInfo.Instance.DailyMissionData.Infos.Count <= 0)
            return -1;

        var list = GameInfo.Instance.DailyMissionData.Infos.FindAll(x => x.GroupID == (int)target);
        if (list == null || list.Count == 0)
            return -1;

        int _SelectDay = 1;
        //System.DateTime curTime = GetCurrentServerTime();
        System.DateTime curTime = GameInfo.Instance.GetNetworkTime();
        for (int i = 0; i < list.Count; i++)
        {
            int result_0 = System.DateTime.Compare(list[i].StartTime, curTime);
            int result_1 = System.DateTime.Compare(list[i].EndTime, curTime);
            if (result_0 <= 0 && result_1 >= 0)
            {
                break;
            }
            _SelectDay++;
        }

        return _SelectDay;
    }

    public static bool GetCurrentDailyFlag(eEventTarget target)
    {
        if (GameInfo.Instance.DailyMissionData == null || GameInfo.Instance.DailyMissionData.Infos == null || GameInfo.Instance.DailyMissionData.Infos.Count <= 0)
            return false;

        List<DailyMissionData.Piece> list = GameInfo.Instance.DailyMissionData.Infos.FindAll(x => x.GroupID == (int)target);
        if (list == null || list.Count == 0)
            return false;

        System.DateTime curTime = GameInfo.Instance.GetNetworkTime();

        if (list[(int)eCOUNT.NONE].StartTime > curTime)
            return false;

        if (list[list.Count - 1].EndTime > curTime)
            return true;

        return false;
    }

    public static void ShowBuyMatItemPopup(GameTable.ItemReqList.Param reqdata)
    {
        List<int> idlist = new List<int>();
        List<int> countlist = new List<int>();

        GameSupport.SetMatList(reqdata, ref idlist, ref countlist);
        for (int i = 0; i < idlist.Count; i++)
        {
            int orgcut = GameInfo.Instance.GetItemIDCount(idlist[i]);
            int orgmax = countlist[i];
            if (orgcut < orgmax)
            {
                ItemBuyMessagePopup.ShowItemBuyPopup(idlist[i], orgcut, orgmax);
                return;
            }
        }
    }

	public static void FirebaseLogEvent(string name, string parameterName, string value)
	{
        Firebase.FirebaseApp.CheckDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == Firebase.DependencyStatus.Available)
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent(name, parameterName, value);
            }
        });
    }

    //210928 신규 - 해시키 없을때만 로그 전송
    public static void SendFireBaseLogEvent(eFireBaseLogType logType)
    {
        if (logType != eFireBaseLogType._RUN)
        {
            if (!PlayerPrefs.HasKey(eFireBaseLogType._RUN.ToString()))
                return;
        }

		if (PlayerPrefs.HasKey(eFireBaseLogType._STOP_.ToString()))
            return;

        if (!PlayerPrefs.HasKey(logType.ToString()))
        {
#if UNITY_EDITOR
            string serverType = "E";
#else
            string serverType = AppMgr.Instance.ServerType == AppMgr.eServerType.LIVE ? "L" : "T";
            //string serverType = AppMgr.Instance.IsLive ? "L" : "T";
#endif
            PlayerPrefs.SetInt(logType.ToString(), 1);
#if !UNITY_EDITOR
			Firebase.Analytics.FirebaseAnalytics.LogEvent(serverType + logType.ToString());
#endif
			Log.Show("########### Firebase Log : " + serverType + logType.ToString(), Log.ColorType.Red);
            //Debug.LogError("########### Firebase Log : " + serverType + logType.ToString());
            if (logType == eFireBaseLogType._Prev_Device || logType == eFireBaseLogType._Prev_Server || logType == eFireBaseLogType._1_1_Tutorial_Skip ||
                logType == eFireBaseLogType._1_1_Tutorial_Card_Equip_Skip || logType == eFireBaseLogType._1_1_Tutorial_Card_LevelUp_Skip || logType == eFireBaseLogType._1_1_Tutorial_1_2_Join_Skip ||
                logType == eFireBaseLogType._1_2_Tutorial_Skip || logType == eFireBaseLogType._1_2_Tutorial_Char_Skill_Leam_Skip || logType == eFireBaseLogType._1_2_Tutorial_Char_Skill_Equip_Skip ||
                logType == eFireBaseLogType._1_2_Tutorial_1_3_Join_Skip || logType == eFireBaseLogType._1_3_Tutorial_Skip || logType == eFireBaseLogType._1_3_Tutorial_Gacha_Skip ||
                logType == eFireBaseLogType._1_3_Tutorial_Mail_Skip)
            {
                PlayerPrefs.SetInt(eFireBaseLogType._STOP_.ToString(), 1);
            }
        }
    }

    public static void SendFireBaseLogEvent(eFireBaseLogType logType, string parID, int parVal)
    {
        if (!PlayerPrefs.HasKey(eFireBaseLogType._RUN.ToString()))
            return;

        if (PlayerPrefs.HasKey(eFireBaseLogType._STOP_.ToString()))
            return;

        if (!PlayerPrefs.HasKey(logType.ToString()))
        {
#if UNITY_EDITOR
            string serverType = "E";
#else
            string serverType = AppMgr.Instance.ServerType == AppMgr.eServerType.LIVE ? "L" : "T";
            //string serverType = AppMgr.Instance.IsLive ? "L" : "T";
#endif
            PlayerPrefs.SetInt(logType.ToString(), 1);
#if !UNITY_EDITOR
			Firebase.Analytics.FirebaseAnalytics.LogEvent(serverType + logType.ToString(), parID, parVal);
#endif
			Log.Show("Firebase Log : " + serverType + logType.ToString());
            //Debug.LogError("########### Firebase Log : " + serverType + logType.ToString());
            if (logType == eFireBaseLogType._Prev_Device || logType == eFireBaseLogType._Prev_Server || logType == eFireBaseLogType._1_1_Tutorial_Skip ||
                logType == eFireBaseLogType._1_1_Tutorial_Card_Equip_Skip || logType == eFireBaseLogType._1_1_Tutorial_Card_LevelUp_Skip || logType == eFireBaseLogType._1_1_Tutorial_1_2_Join_Skip ||
                logType == eFireBaseLogType._1_2_Tutorial_Skip || logType == eFireBaseLogType._1_2_Tutorial_Char_Skill_Leam_Skip || logType == eFireBaseLogType._1_2_Tutorial_Char_Skill_Equip_Skip ||
                logType == eFireBaseLogType._1_2_Tutorial_1_3_Join_Skip || logType == eFireBaseLogType._1_3_Tutorial_Skip || logType == eFireBaseLogType._1_3_Tutorial_Gacha_Skip ||
                logType == eFireBaseLogType._1_3_Tutorial_Mail_Skip)
            {
                PlayerPrefs.SetInt(eFireBaseLogType._STOP_.ToString(), 1);
            }
        }
    }

    //해당 스테이지 클리어 전까지 로비에서 손가락 표시 여부
    public static bool ShowLobbyStageHand()
    {
        if (GameSupport.IsTutorial())
            return false;

        StageClearData stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == GameInfo.Instance.GameConfig.ShowStageHand);
        if (stagecleardata == null)
        {
            return true;
        }

        return false;
    }

    public static bool ShowLobbyStageHand(int stageID)
    {
        if (!ShowLobbyStageHand())
            return false;

        if (GameInfo.Instance.StageClearList.Count <= (int)eCOUNT.NONE)
        {
            if (stageID == 1)       //튜토리얼 1-1에서 스킵할시 강제로 1-1만 켜주기
            {
                return true;
            }

            return false;
        }
        else
        {
            StageClearData stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == stageID);

            if (stagecleardata != null)
                return false;

            List<StageClearData> lastclearstage = GameInfo.Instance.StageClearList.FindAll(x => x.TableData.StageType == (int)eSTAGETYPE.STAGE_MAIN_STORY && x.TableData.Difficulty == 1);
            if (lastclearstage == null)
                return false;

            if (lastclearstage[lastclearstage.Count - 1].TableData.NextStage == stageID)
                return true;
        }

        

        return false;
    }

    public static void SetSkillSprite(ref UISprite uISprite, string atlasName, string spriteName)
    {
        UISpriteData haveSprite = uISprite.GetSprite(spriteName);
        if (haveSprite == null)
        {
            GameObject haveAtlasObj = ResourceMgr.Instance.LoadFromAssetBundle("ui", Utility.AppendString("UI/Atlas/", atlasName, ".prefab")) as GameObject;
            if (haveAtlasObj == null)
            {
                return;
            }

            UIAtlas haveAtlas = haveAtlasObj.GetComponent<UIAtlas>();
            if (haveAtlas != null)
            {
                uISprite.atlas = haveAtlas;
            }

            FLocalizeSprite localizeSprite = uISprite.GetComponent<FLocalizeSprite>();
            if (localizeSprite != null)
            {
                localizeSprite.OnLocalize();
            }
        }

        uISprite.spriteName = spriteName;
        uISprite.MakePixelPerfect();
    }

    public static bool IsRaidEnd() {
        DateTime curTime = GetCurrentServerTime();
        DateTime endTime = GameInfo.Instance.ServerData.RaidSeasonEndTime;

        if( curTime >= endTime && curTime <= endTime.AddHours( GameInfo.Instance.GameConfig.RaidEndResultHour ) ) {
            return true;
		}

        return false;
    }
	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	//
	//  가디언 스킬
	//
	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public static void SetGuardianSkill( Player player, CharData charData ) {
        if ( player == null || charData == null ) {
            return;
        }

		for ( int i = 0; i < charData.EquipSkill.Length; i++ ) {
			GameTable.CharacterSkillPassive.Param characterSkillPassiveParam = GameInfo.Instance.GameTable.FindCharacterSkillPassive( charData.EquipSkill[i] );
			if ( characterSkillPassiveParam == null ) {
                continue;
            }

			List<GameTable.CharacterSkillPassive.Param> listCondSkill = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive( x => x.ParentsID == charData.EquipSkill[i] && x.Type == (int)eCHARSKILLPASSIVETYPE.CONDITION_SKILL );
			for ( int j = 0; j < listCondSkill.Count; j++ ) {
				if ( !IsCharSkillCond( charData, listCondSkill[j] ) ) {
					listCondSkill.Remove( listCondSkill[j] );
					--j;
				}
			}

			if ( listCondSkill.Count > 0 ) {
				listCondSkill.Insert( 0, characterSkillPassiveParam );

				characterSkillPassiveParam = listCondSkill[listCondSkill.Count - 1];
				listCondSkill.Remove( characterSkillPassiveParam );
			}

			for ( int index = 0; index < (int)eCOUNT.WEAPONSLOT; index++ ) {
				player.AddGuardianSkillAction( characterSkillPassiveParam, listCondSkill, index );
			}
		}
	}
}
