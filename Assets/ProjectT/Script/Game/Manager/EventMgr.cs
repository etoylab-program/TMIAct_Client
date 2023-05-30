
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventMgr : MonoSingleton<EventMgr>
{
    public List<Unit> ListSendTarget = new List<Unit>();

    private BaseEvent mEvent = new BaseEvent();


    public void SendEvent(eEventSubject eventSubject, eEventType eventType, Unit sender)
    {
        mEvent.eventSubject = eventSubject;
        mEvent.eventType = eventType;
        mEvent.sender = sender;

        SendEvent(mEvent);
    }

	public void SendEvent( BaseEvent evt, System.Action callback = null, IActionBaseParam param = null ) {
		if ( evt == null ) {
			return;
		}

		if ( ( evt.eventSubject & eEventSubject.Self ) != 0 && evt.sender ) {
			evt.sender.OnEvent( evt, param );
		}

		if ( ( evt.eventSubject & eEventSubject.Player ) != 0 ) {
			if ( World.Instance.Player && World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
				World.Instance.Player.OnEvent( evt );
			}
			else if( evt.sender ){
				Player player = evt.sender as Player;
				if ( player ) {
					WorldPVP worldPVP = World.Instance as WorldPVP;
					if ( worldPVP ) {
						if ( player.OpponentPlayer ) {
							Player opponent = worldPVP.GetCurrentOpponentTeamCharOrNull();
							if ( opponent ) {
								opponent.OnEvent( evt );
							}
						}
						else if ( World.Instance.Player ) {
							World.Instance.Player.OnEvent( evt );
						}
					}
				}
			}
		}

		if ( ( evt.eventSubject & eEventSubject.PlayerAll ) != 0 ) {
			if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
				for ( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
					World.Instance.ListPlayer[i].OnEvent( evt );
				}
			}
			else if ( evt.sender ) {
				Player player = evt.sender as Player;
				if ( player ) {
					WorldPVP worldPVP = World.Instance as WorldPVP;
					if ( worldPVP ) {
						if ( player.OpponentPlayer ) {
							Player opponent = worldPVP.GetCurrentOpponentTeamCharOrNull();
							if ( opponent ) {
								opponent.OnEvent( evt );
							}
						}
						else {
							World.Instance.Player.OnEvent( evt );
						}
					}
				}
			}
		}

		if ( ( evt.eventSubject & eEventSubject.PlayerAllInRange ) != 0 ) {
			List<UnitCollider> find = evt.sender.GetAllyColliderListByAround( evt.sender.transform.position, evt.battleOptionData.targetValue );
			for ( int i = 0; i < find.Count; i++ ) {
				find[i].Owner.OnEvent( evt );
			}
		}

		if ( ( evt.eventSubject & eEventSubject.PlayerSummons ) != 0 && evt.sender ) {
			for ( int i = 0; i < World.Instance.ListPlayerSummons.Count; i++ ) {
				DroneUnit droneUnit = World.Instance.ListPlayerSummons[i] as DroneUnit;
				if ( droneUnit && droneUnit.Owner && droneUnit.Owner != evt.sender ) {
					continue;
				}
				else if ( droneUnit == null ) {
					PlayerMinion minion = World.Instance.ListPlayerSummons[i] as PlayerMinion;
					if ( minion && minion.Owner && minion.Owner != evt.sender ) {
						continue;
					}
					else if ( minion == null && World.Instance.ListPlayerSummons[i].cloneOwner && World.Instance.ListPlayerSummons[i].cloneOwner != evt.sender ) {
						continue;
					}
				}

				World.Instance.ListPlayerSummons[i].OnEvent( evt );
			}
		}

		if ( ( evt.eventSubject & eEventSubject.MainTarget ) != 0 ) {
			if ( evt.sender == null ) {
				return;
			}

			if ( evt.sender.mainTarget == null ) {
				UnitCollider unitCollider = evt.sender.GetMainTargetCollider( true );
				if ( unitCollider && unitCollider.Owner ) {
					evt.sender.SetMainTarget( unitCollider.Owner );
				}
			}

			if ( evt.sender.mainTarget ) {
				evt.sender.mainTarget.OnEvent( evt );
			}
		}

		if ( ( evt.eventSubject & eEventSubject.SelectTarget ) != 0 ) {
			if ( evt.SelectTarget == null ) {
				return;
			}

			evt.SelectTarget.OnEvent( evt );
		}

		if ( ( evt.eventSubject & eEventSubject.HitTargetList ) != 0 ) {
			if ( evt.sender == null ) {
				return;
			}

			List<Unit> listHitTarget = new List<Unit>();
			listHitTarget.AddRange( evt.sender.listHitTarget );

			Player player = evt.sender as Player;
			if ( player != null && player.Guardian != null ) {
				for ( int i = 0; i < player.Guardian.listHitTarget.Count; i++ ) {
					listHitTarget.Remove( player.Guardian.listHitTarget[i] );
				}
				listHitTarget.AddRange( player.Guardian.listHitTarget );
			}

			for ( int i = 0; i < listHitTarget.Count; i++ ) {
				if ( listHitTarget[i] == null || 
					!listHitTarget[i].IsActivate() ||
					listHitTarget[i].curHp <= 0.0f ||
					listHitTarget[i].CurrentSuperArmor == Unit.eSuperArmor.Invincible ||
					listHitTarget[i].TemporaryInvincible ) {
					continue;
				}

				listHitTarget[i].OnEvent( evt );
			}
		}

		if ( ( evt.eventSubject & eEventSubject.HitTargetListOnce ) != 0 ) {
			if ( evt.sender == null ) {
				return;
			}

			List<Unit> listHitTarget = new List<Unit>();
			listHitTarget.AddRange( evt.sender.listHitTarget );

			for ( int i = 0; i < listHitTarget.Count; i++ ) {
				if ( listHitTarget[i] == null || !listHitTarget[i].IsActivate() || listHitTarget[i].curHp <= 0.0f ) {
					continue;
				}

				listHitTarget[i].OnEvent( evt );
				SendEvent( eEventSubject.World, eEventType.EVENT_GAME_REMOVE_HIT_TARGET, listHitTarget[i] );
			}
		}

		if ( ( evt.eventSubject & eEventSubject.HitTargetDie ) != 0 ) {
			if ( evt.sender == null ) {
				return;
			}

			List<Unit> listHitTarget = evt.sender.listHitTarget;
			for ( int i = 0; i < listHitTarget.Count; i++ ) {
				if ( listHitTarget[i] == null || !listHitTarget[i].IsActivate() || listHitTarget[i].curHp > 0.0f ) {
					continue;
				}

				listHitTarget[i].OnEvent( evt );
			}
		}

		if ( ( evt.eventSubject & eEventSubject.OnHitTargetDie ) != 0 ) {
			if ( evt.sender == null ) {
				return;
			}

			List<Unit> listHitTarget = evt.sender.listHitTarget;
			for ( int i = 0; i < listHitTarget.Count; i++ ) {
				if ( listHitTarget[i] == null || !listHitTarget[i].IsActivate() || listHitTarget[i].curHp > 0.0f ) {
					continue;
				}

				evt.sender.OnEvent( evt );
			}
		}

		if ( ( evt.eventSubject & eEventSubject.ActiveEnemies ) != 0 ) {
			if ( evt.sender == null ) {
				return;
			}

			List<Unit> listEnemy = evt.sender.GetEnemyList( true );

			int nowCnt = 0;
			int maxCnt = 0;

			if ( evt.battleOptionData != null && evt.battleOptionData.targetValue > 0.0f ) {
				maxCnt = (int)evt.battleOptionData.targetValue;
			}
			else {
				maxCnt = listEnemy.Count;
			}

			int senderEnemyLayer = Utility.GetEnemyLayer( (eLayer)evt.sender.gameObject.layer );

			ListSendTarget.Clear();
			if ( evt.sender.mainTarget && ( ( senderEnemyLayer & ( 1 << evt.sender.mainTarget.gameObject.layer ) ) != 0 ) ) {
				ListSendTarget.Add( evt.sender.mainTarget );
				++nowCnt;
			}

			for ( int i = 0; i < listEnemy.Count; i++ ) {
				if ( listEnemy[i] == null || evt.sender.mainTarget && evt.sender.mainTarget == listEnemy[i] ) {
					continue;
				}

				if ( nowCnt < maxCnt ) {
					ListSendTarget.Add( listEnemy[i] );
					++nowCnt;
				}
				else
					break;
			}

			for ( int i = 0; i < ListSendTarget.Count; i++ ) {
				ListSendTarget[i].OnEvent( evt );
			}
		}

		if ( ( evt.eventSubject & eEventSubject.ActiveEnemiesInRange ) != 0 ) {
			if ( evt.sender == null ) {
				return;
			}

			/* eEventSubject.ActiveEnemiesInRange evt 구성
             * evt.value = 효과
             * evt.value2 = 범위(=evt.battleOptionData.targetValue)
             * evt.value3 = 인원(=evt.battleOptionData.value3) = 0 이하면 전체
            */
			float fAroundRadius = 0.0f;
			if ( evt.battleOptionData != null && evt.battleOptionData.targetValue > 0.0f ) {
				fAroundRadius = evt.battleOptionData.targetValue;
			}
			else {
				fAroundRadius = evt.value2;
			}

			Unit unit = evt.sender;
			if ( evt.CompareUnit ) {
				unit = evt.CompareUnit;
			}

			List<UnitCollider> listEnemyCollider = null;
			if ( unit is Enemy ) {
				listEnemyCollider = (unit as Enemy).GetColliderListFromAroundOrNull( unit.transform.position, fAroundRadius );
			}
			else {
				listEnemyCollider = evt.sender.GetTargetColliderListByAround( unit.transform.position, fAroundRadius );
			}

			if ( listEnemyCollider != null ) {
				int nowCnt = 0;
				int maxCnt = 0;

				if ( evt.battleOptionData != null && evt.battleOptionData.value3 > 0.0f ) {
					maxCnt = (int)evt.battleOptionData.value3;
				}
				else {
					maxCnt = (int)evt.value3;
				}

				if ( maxCnt == 0 ) {
					maxCnt = listEnemyCollider.Count;
				}

				BuffEvent buffEvt = evt as BuffEvent;

				bool checkSenderEnemyLayer = true;
				if ( buffEvt != null && buffEvt.ExtraValue2 >= 1.0f ) {
					checkSenderEnemyLayer = false;
				}

				ListSendTarget.Clear();

				if ( checkSenderEnemyLayer ) {
					int senderEnemyLayer = Utility.GetEnemyLayer( (eLayer)evt.sender.gameObject.layer );

					if ( evt.sender.mainTarget && ( ( senderEnemyLayer & ( 1 << evt.sender.mainTarget.gameObject.layer ) ) != 0 ) ) {
						ListSendTarget.Add( evt.sender.mainTarget );
						++nowCnt;
					}
				}

				for ( int i = 0; i < listEnemyCollider.Count; i++ ) {
					if ( listEnemyCollider[i] == null || evt.sender.mainTarget && evt.sender.mainTarget == listEnemyCollider[i].Owner ) {
						continue;
					}

					if ( nowCnt < maxCnt ) {
						ListSendTarget.Add( listEnemyCollider[i].Owner );
						++nowCnt;
					}
					else {
						break;
					}
				}

				for ( int i = 0; i < ListSendTarget.Count; i++ ) {
					ListSendTarget[i].OnEvent( evt );
				}
			}
		}

		if ( ( evt.eventSubject & eEventSubject.OnHitActionEnemies ) != 0 ) {
			if ( evt.sender == null ) {
				return;
			}

			List<Unit> listEnemy = evt.sender.GetEnemyList( true );
			for ( int i = 0; i < listEnemy.Count; i++ ) {
				Unit enemy = listEnemy[i];
				if ( enemy == null || enemy.actionSystem == null || enemy.actionSystem.currentAction == null ) {
					continue;
				}

				ActionHit actionHit = enemy.actionSystem.GetCurrentAction<ActionHit>();
				if ( actionHit == null ) {
					continue;
				}

				enemy.OnEvent( evt );
			}
		}

		if ( ( evt.eventSubject & eEventSubject.AppearEnemy ) != 0 ) {
			if ( evt.sender == null ) {
				return;
			}

			List<Unit> listEnemy = evt.sender.GetEnemyList( true );
			for ( int i = 0; i < listEnemy.Count; i++ ) {
				Unit enemy = listEnemy[i];
				if ( enemy == null || enemy.actionSystem == null ) {
					continue;
				}

				if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP && 
					 ( enemy.actionSystem.currentAction == null || !enemy.actionSystem.IsCurrentAction( eActionCommand.Appear ) ) ) {
					continue;
				}

				enemy.OnEvent( evt );
			}
		}

		if ( ( evt.eventSubject & eEventSubject.Caster ) != 0 && evt.sender ) {
			evt.sender.OnEvent( evt );
		}

		if ( ( evt.eventSubject & eEventSubject.Attacker ) != 0 && evt.sender && evt.sender.attacker ) {
			evt.sender.attacker.OnEvent( evt );
		}

		if ( ( evt.eventSubject & eEventSubject.Action ) != 0 && evt.sender ) {
			evt.sender.OnEvent( evt );
		}

		if ( ( evt.eventSubject & eEventSubject.World ) != 0 ) {
			World.Instance.OnEvent( evt );
		}
	}
}
