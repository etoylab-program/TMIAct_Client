
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ActionSystem : MonoBehaviour
{
    private Unit                        m_owner             = null;
    private IActionBaseParam            m_param             = null;
    private Coroutine                   m_crCurrentAction   = null;
    private ActionBase                  m_currentAction     = null;
    private List<ActionSelectSkillBase> mListTemp           = new List<ActionSelectSkillBase>();

    public List<ActionBase> ListAction              { get; private set; }   = new List<ActionBase>();
    public eActionCommand   BeforeActionCommand     { get; private set; }   = eActionCommand.None;
    public ActionHit.eState BeforeHitActionState    { get; private set; }   = ActionHit.eState.Normal;
    public eActionCommand   NextActionCommand       { get; private set; }   = eActionCommand.None;


    public ActionBase currentAction
    {
        get
        {
            if(m_currentAction != null && m_currentAction.actionCommand == eActionCommand.Idle)
                CancelCurrentAction();

            return m_currentAction;
        }
    }


    public void Init(Unit owner)
    {
        if(owner == null)
        {
            Debug.LogError("ActionSystem::오너를 설정할 수 없습니다.");
            return;
        }

        m_owner = owner;
        m_currentAction = null;
    }

    public bool AddAction(ActionBase action, int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam, 
                          GameClientTable.BattleOptionSet.Param paramBOSet = null)
    {
        if (action == null)
            return false;

        ActionBase find = ListAction.Find(x => x == action);
        if (find != null)
            return false;

        if(paramBOSet != null)
        {
            action.SetBattleOptionSetParam(paramBOSet);
        }

        action.Init(tableId, listAddCharSkillParam);
        ListAction.Add(action);

        return true;
    }

    public void AddAction(ActionBase[] actions)
    {
        ListAction.AddRange(actions);

        for (int i = 0; i < ListAction.Count; i++)
            ListAction[i].Init(0, null);
    }

    public void RemoveAction(eActionCommand command)
    {
        ActionBase action = GetAction(command);
        if (action == null)
            return;

        ListAction.Remove(action);
        DestroyImmediate(action);
    }

	public void RemoveAction( ActionBase action ) {
		ActionBase findAction = ListAction.Find( x => x == action );
		if ( findAction == null ) {
			return;
		}

		ListAction.Remove( action );
		DestroyImmediate( action );
	}

    public void LoadAfterEnemyMgrInit()
    {
        for(int i = 0; i < ListAction.Count; i++)
        {
            ListAction[i].LoadAfterEnemyMgrInit();
        }
    }

    public bool HasAction(eActionCommand command)
    {
        for ( int i = 0; i < ListAction.Count; i++ ) {
            if ( ListAction[i].actionCommand == command ) {
                return true;
			}
		}

        return false;
    }

    public void ClearAction()
    {
        ListAction.Clear();
    }

	private IEnumerator UpdateAction() {
        if ( m_owner == null || m_currentAction == null ) {
            yield break;
		}

		m_owner.StopStepForward();

		m_currentAction.OnStart( m_param );
		m_crCurrentAction = StartCoroutine( m_currentAction.UpdateAction() );

		// ActionSelectSkillBase를 상속 받은 액션들은 쿨타임 있음
		ActionSelectSkillBase actionSelectSkillBase = m_currentAction as ActionSelectSkillBase;
		if ( actionSelectSkillBase && !actionSelectSkillBase.ExplicitStartCoolTime ) {
			actionSelectSkillBase.StartCoolTime();
		}
        else {
            ActionGuardianBase actionGuardianBase = m_currentAction as ActionGuardianBase;
            if ( actionGuardianBase != null && actionGuardianBase.IsPossibleUse() ) {
				actionGuardianBase.StartCoolTime();
			}
        }

		yield return m_crCurrentAction;

		m_currentAction.OnEnd();
		m_owner.OnEndAction();

		if ( m_currentAction == null ) {
			yield break;
		}

		if ( m_currentAction.nextAction == eActionCommand.RemoveAction ) {
			RemoveAction( m_currentAction.actionCommand );
			m_currentAction = null;

			if ( m_owner.Input ) {
				DoAction( eActionCommand.Idle, m_param );
			}
		}
		else if ( m_currentAction.nextAction != eActionCommand.None ) {
			eActionCommand nextAction = m_currentAction.nextAction;
			IActionBaseParam nextParam = m_currentAction.nextParam;

			m_currentAction.SetNextAction( eActionCommand.None, null );
			m_currentAction = null;

			DoAction( nextAction, nextParam, false );
		}
		else if ( m_owner.curHp > 0.0f ) {
			SetBeforeActionCommand( m_currentAction.actionCommand );
			m_currentAction = null;

			if ( m_owner.Input ) {
				Player player = m_owner as Player;
				if ( player && player.tableId == (int)ePlayerCharType.Noah && player.ExtreamEvading ) {
					yield break;
				}

            #if UNITY_EDITOR
				// 연결된 스킬 액션들의 SkillEnd 플래그 처리를 위한 코드
				if ( BeforeActionCommand != eActionCommand.None ) {
					ActionSelectSkillBase beforeAction = GetAction<ActionSelectSkillBase>( BeforeActionCommand );
					if ( beforeAction && beforeAction.Child ) {
						beforeAction.SetChildSkillEndFlag( true );
					}
					else if ( beforeAction && beforeAction.ExecuteAction && beforeAction.ExecuteAction.Child ) {
						beforeAction.ExecuteAction.SetChildSkillEndFlag( true );
					}
				}
            #endif

				DoAction( eActionCommand.Idle, m_param, false );
			}
		}
	}

	private void SetBeforeActionCommand(eActionCommand actionCommand)
    {
        BeforeActionCommand = actionCommand;
        if (BeforeActionCommand == eActionCommand.Hit)
        {
            ActionHit actionHit = m_currentAction as ActionHit;
            if (actionHit)
            {
                BeforeHitActionState = actionHit.State;
            }
        }
    }

    public ActionBase DoAction(eActionCommand actionCommand, IActionBaseParam param, bool checkAniSpeed = true)
    {
        if(World.Instance.IsEndGame || m_owner.IgnoreAction == actionCommand)
        {
            if( World.Instance.IsEndGame && actionCommand == eActionCommand.Die ) {
                m_owner.Deactivate();
			}

            return null;
        }

		if( m_owner.skipAttack && IsAnyAttackAction( actionCommand ) ) {
            ActionBase atkAction = GetAction( actionCommand );

            if( !atkAction.IsIgnoreAttackSkip ) {
                return null;
            }
		}

		ActionBase action = GetAction(actionCommand);
        if (action == null || action.DontUse)
        {
            return null;
        }

        // 빙결 피격 직후 간헐적으로 빙결이 풀릴떄까지 무적 판정으로 유지되는 현상 수정 (애니메이션 스피드가 0이면 액션 실행 안함)
        if (checkAniSpeed && m_owner.curHp > 0.0f && m_owner.aniEvent && m_owner.aniEvent.aniSpeed <= 0.0f)
        {
            return null;
        }

        Player player = m_owner as Player;
        if (player && player.UsingUltimateSkill)
        {
            if (IsSkillAction(actionCommand) || actionCommand == eActionCommand.Defence)
            {
                return null;
            }
        }

        ActionSelectSkillBase actionSelectSkillBase = action as ActionSelectSkillBase;
        if (actionSelectSkillBase && !actionSelectSkillBase.PossibleToUse)
        {
            Debug.LogWarning(action.actionCommand + "액션은 쿨타임 중!!");
            return null;
        }
        else {
            ActionGuardianBase actionGuardianBase = action as ActionGuardianBase;
            if ( actionGuardianBase && !actionGuardianBase.IsPossibleUse() ) {
				Debug.LogWarning( action.actionCommand + "액션은 쿨타임 중!!" );
				return null;
            }
        }

        NextActionCommand = actionCommand;

        m_param = param;
        if (currentAction != null)
        {
            if ((currentAction.cancelActionCommand == null || currentAction.cancelActionCommand.Length <= 0) &&
                (currentAction.extraCancelCondition == null || currentAction.extraCancelCondition.Length <= 0) || currentAction.SkipConditionCheck)
            {
                return null;
            }

            bool exit = true;
            if (action.conditionActionCommand != null && action.conditionActionCommand.Length > 0)
            {
                for (int i = 0; i < action.conditionActionCommand.Length; i++)
                {
                    if (m_currentAction.actionCommand == action.conditionActionCommand[i])
                    {
                        // (기본 콤보 공격 때문에) 현재 액션과 들어온 액션이 다를 경우에만 현재 액션을 캔슬
                        if (m_currentAction.actionCommand != actionCommand)
                        {
                            ActionHit actionHit = m_currentAction as ActionHit;
                            if (m_currentAction.actionCommand != eActionCommand.Hit || (actionHit && actionHit.State == ActionHit.eState.Normal))
                            {
                                SetBeforeActionCommand(m_currentAction.actionCommand);
                                CancelCurrentAction();
                            }
                        }
                        else
                        {
                            if (!m_currentAction.CancelActionDuringSameAction())
                                return null;
                        }

                        exit = false;
                        break;
                    }
                }

                if (exit == true)
                    return null;
            }

            if (action.extraCondition != null && action.extraCondition.Length > 0)
            {
                for (int i = 0; i < action.extraCondition.Length; i++)
                {
                    switch (action.extraCondition[i])
                    {
                        case eActionCondition.Grounded:
                            if (m_owner.isGrounded == false)
                                return null;
                            break;

                        case eActionCondition.Jumping:
                            if (m_owner.isGrounded == true)
                                return null;
                            break;

                        case eActionCondition.NoUsingSkill:
                            if (IsCurrentSkillAction() == true)
                                return null;
                            break;

                        case eActionCondition.NoUsingQTE:
                            if (IsCurrentQTEAction() == true)
                                return null;
                            break;

                        case eActionCondition.NoUsingUSkill:
                            if (IsCurrentUSkillAction() == true)
                                return null;
                            break;

                        case eActionCondition.UseSkill:
                            if (IsCurrentSkillAction() == false)
                                return null;
                            break;

                        case eActionCondition.UseQTE:
                            if (IsCurrentQTEAction() == false)
                                return null;
                            break;

                        case eActionCondition.UseUSkill:
                            if (IsCurrentUSkillAction() == false)
                                return null;
                            break;
                    }
                }
            }

            if (m_currentAction != null)
            {
                SetBeforeActionCommand(m_currentAction.actionCommand);
                bool cancel = false;

                if (IsHitAction(actionCommand) || IsSkillAction(actionCommand) || IsQTEAction(actionCommand))
                    cancel = true;
                else if (m_currentAction.cancelActionCommand != null)
                {
                    for (int i = 0; i < m_currentAction.cancelActionCommand.Length; i++)
                    {
                        if (m_currentAction.cancelActionCommand[i] == actionCommand)
                        {
                            cancel = true;
                            break;
                        }
                    }
                }

                if (m_currentAction.extraCancelCondition != null && m_currentAction.extraCancelCondition.Length > 0)
                {
                    for (int i = 0; i < m_currentAction.extraCancelCondition.Length; i++)
                    {
                        if (m_currentAction.extraCancelCondition[i] == eActionCondition.Grounded)
                        {
                            if (m_owner.isGrounded == true)
                            {
                                cancel = true;
                                break;
                            }
                        }
                        else if (m_currentAction.extraCancelCondition[i] == eActionCondition.Jumping)
                        {
                            if (m_owner.isGrounded == false)
                            {
                                cancel = true;
                                break;
                            }
                        }
                        else if (m_currentAction.extraCancelCondition[i] == eActionCondition.All)
                        {
                            cancel = true;
                            break;
                        }
                        else if (m_currentAction.extraCancelCondition[i] == eActionCondition.NoUsingSkill)
                        {
                            if (!IsSkillAction(action.actionCommand))
                            {
                                cancel = true;
                                break;
                            }
                        }
                        else if (m_currentAction.extraCancelCondition[i] == eActionCondition.NoUsingQTE)
                        {
                            if (!IsQTEAction(action.actionCommand))
                            {
                                cancel = true;
                                break;
                            }
                        }
                        else if (m_currentAction.extraCancelCondition[i] == eActionCondition.NoUsingUSkill)
                        {
                            if (!IsUSkillAction(action.actionCommand))
                            {
                                cancel = true;
                                break;
                            }
                        }
                        else if (m_currentAction.extraCancelCondition[i] == eActionCondition.UseSkill)
                        {
                            if (IsSkillAction(action.actionCommand))
                            {
                                cancel = true;
                                break;
                            }
                        }
                        else if (m_currentAction.extraCancelCondition[i] == eActionCondition.UseQTE)
                        {
                            if (IsQTEAction(action.actionCommand))
                            {
                                cancel = true;
                                break;
                            }
                        }
                        else if (m_currentAction.extraCancelCondition[i] == eActionCondition.UseUSkill)
                        {
                            if (IsUSkillAction(action.actionCommand))
                            {
                                cancel = true;
                                break;
                            }
                        }
                    }
                }

                if (cancel == true)
                    CancelCurrentAction();
                else
                    return null;
            }
        }
        else
        {
            SetBeforeActionCommand(eActionCommand.None);
        }

        //Debug.Log(m_owner.name + "'s Current Action : " + action.actionCommand);
        m_currentAction = action;

        StartCoroutine("UpdateAction");
        return action;
    }

    public void CancelCurrentAction()
    {
        if (m_currentAction == null)
        {
            return;
        }

        if (m_currentAction != null)
        {
            m_currentAction.OnCancel();

            ActionSelectSkillBase actionSelectSkillBase = m_currentAction as ActionSelectSkillBase;
            if (actionSelectSkillBase && m_owner.IsActivate())
            {
                m_owner.PlayAni(eAnimation.Idle01);
            }
        }

        if(m_crCurrentAction != null)
            Utility.StopCoroutine(this, ref m_crCurrentAction);

        StopCoroutine("UpdateAction");
        m_currentAction = null;
    }

    public bool HasNoAction()
    {
        if (ListAction.Count <= 0)
            return true;

        return false;
    }

    public T GetCurrentAction<T>() where T : ActionBase
    {
        if (currentAction == null)
            return null;

        /*
        if (currentAction.isPlaying == false)
        {
            CancelCurrentAction();
            return null;
        }
        */

        return (currentAction as T);
    }

	public ActionBase GetBeforeActionOrNull()
	{
		if(BeforeActionCommand == eActionCommand.None)
		{
			return null;
		}

		return GetAction(BeforeActionCommand);
	}

	public ActionBase GetAction( eActionCommand actionCommand ) {
		ActionBase action = ListAction.Find(x => x.actionCommand == actionCommand);
		if( action == null ) {
			//Debug.Log( actionCommand.ToString() + "액션이 없습니다." );
		}

		return action;
	}

	public ActionBase GetActionOrNull(System.Predicate<ActionBase> match)
    {
        ActionBase find = ListAction.Find(match);
        return find;
    }

    public T GetAction<T>(eActionCommand actionCommand) where T : ActionBase
    {
        return (GetAction(actionCommand) as T);
    }

    public T GetActionOrNullByTableId<T>(int tableId) where T : ActionBase
    {
        ActionBase action = ListAction.Find(x => x.TableId == tableId);
        if(action == null)
        {
			for(int i = 0; i < ListAction.Count; i++)
			{
				ActionSelectSkillBase actionSelectSkill = ListAction[i] as ActionSelectSkillBase;
				if(actionSelectSkill == null)
				{
					continue;
				}

				if(actionSelectSkill.ParentTableId == tableId)
				{
					return (actionSelectSkill as T);
				}
			}

            Debug.Log(tableId + "번 액션이 없습니다.");
        }

        return (action as T);
    }

    public List<T> GetActionList<T>() where T : ActionBase
    {
        List<T> list = new List<T>();

        for(int i = 0; i < ListAction.Count; i++)
        {
            T t = ListAction[i] as T;
            if(t != null)
            {
                list.Add(t);
            }
        }

        return list;
    }

    public ActionSelectSkillBase GetSelectSkillActionDuringCooltimeOrNull()
    {
        mListTemp.Clear();

        for (int i = 0; i < ListAction.Count; i++)
        {
            ActionSelectSkillBase action = ListAction[i] as ActionSelectSkillBase;
            if(action == null || action.CoolTime <= 0.0f)
            {
                continue;
            }

            mListTemp.Add(action);
        }

        if(mListTemp.Count <= 0)
        {
            return null;
        }

        return mListTemp[Random.Range(0, mListTemp.Count)];
    }

    /*public ActionQTEBase[] GetQTEActions()
    {
        List<ActionQTEBase> list = new List<ActionQTEBase>();
        for(int i = 0; i < ListAction.Count; i++)
        {
            ActionQTEBase actionQTE = ListAction[i] as ActionQTEBase;
            if (actionQTE != null)
                list.Add(actionQTE);
        }

        return list.ToArray();
    }*/

    public bool IsCurrentAction(eActionCommand actionCommand)
    {
        if (currentAction == null || currentAction.actionCommand != actionCommand)
            return false;

        return true;
    }

	public bool IsCurrentAnyAttackAction() {
		if( currentAction == null )
			return false;

		if( ( currentAction.actionCommand > eActionCommand.__StartAttack && currentAction.actionCommand < eActionCommand.__EndAttack ) ||
            ( currentAction.actionCommand > eActionCommand.__StartExtraAttack && currentAction.actionCommand < eActionCommand.__EndExtraAttack ) ||
            ( currentAction.actionCommand > eActionCommand.__StartSkill && currentAction.actionCommand < eActionCommand.__EndSkill ) ||
			( currentAction.actionCommand > eActionCommand.__StartQTE && currentAction.actionCommand < eActionCommand.__EndQTE ) ||
			( currentAction.actionCommand > eActionCommand.__StartUSkill && currentAction.actionCommand < eActionCommand.__EndUSkill ) ||
			( currentAction.actionCommand > eActionCommand.__StartWpnSkill && currentAction.actionCommand < eActionCommand.__EndWpnSkill ) ||
			currentAction.actionCommand == eActionCommand.AttackPattern ) {
			return true;
		}

		return false;
	}

	public bool IsCurrentNormalAttackAction() {
        if( currentAction ) {
            if( ( currentAction.actionCommand > eActionCommand.__StartAttack && currentAction.actionCommand < eActionCommand.__EndAttack ) ||
                ( currentAction.actionCommand > eActionCommand.__StartExtraAttack && currentAction.actionCommand < eActionCommand.__EndExtraAttack ) ||
                ( currentAction.actionCommand == eActionCommand.AttackPattern ) ) {
                return true;
            }
        }

		Player player = m_owner as Player;
		if( player ) {
			Unit unitWeapon = player.GetCurrentUnitWeaponOrNull();
			if( unitWeapon ) {
				return unitWeapon.actionSystem.IsCurrentNormalAttackAction();
			}
		}

        if( currentAction == null ) {
            return false;
		}

		ActionSelectSkillBase action = currentAction as ActionSelectSkillBase;
		if( action ) {
			return action.IsNormalAttack;
		}

		return false;
	}

	public bool IsCurrentSkillAction()
    {
        if (currentAction == null)
            return false;

        if (currentAction.actionCommand > eActionCommand.__StartSkill && currentAction.actionCommand < eActionCommand.__EndSkill)
            return true;

        if (currentAction.actionCommand > eActionCommand.__StartWpnSkill && currentAction.actionCommand < eActionCommand.__EndWpnSkill)
            return true;

        return false;
    }

    public bool IsCurrentWeaponSkillAction()
    {
        if (currentAction == null)
        {
            return false;
        }

        if (currentAction.actionCommand > eActionCommand.__StartWpnSkill && currentAction.actionCommand < eActionCommand.__EndWpnSkill)
        {
            return true;
        }

        return false;
    }

    public bool IsCurrentQTEAction()
    {
        if (currentAction == null)
            return false;

        if (currentAction.actionCommand > eActionCommand.__StartQTE && currentAction.actionCommand < eActionCommand.__EndQTE)
            return true;

        return false;
    }

    public bool IsCurrentUSkillAction()
    {
        if (currentAction == null)
            return false;

        if (currentAction.actionCommand > eActionCommand.__StartUSkill && currentAction.actionCommand < eActionCommand.__EndUSkill)
            return true;

        return false;
    }

    public bool IsCurrentHitAction()
    {
        if (currentAction == null)
            return false;

        if (currentAction.actionCommand > eActionCommand.__StartHit && currentAction.actionCommand < eActionCommand.__EndHit)
            return true;

        return false;
    }

	public bool IsAnyAttackAction( eActionCommand actionCommand ) {
		if( actionCommand == eActionCommand.None )
			return false;

		if( ( actionCommand > eActionCommand.__StartAttack && actionCommand < eActionCommand.__EndAttack ) ||
            ( actionCommand > eActionCommand.__StartExtraAttack && actionCommand < eActionCommand.__EndExtraAttack ) ||
            ( actionCommand > eActionCommand.__StartSkill && actionCommand < eActionCommand.__EndSkill ) ||
			( actionCommand > eActionCommand.__StartQTE && actionCommand < eActionCommand.__EndQTE ) ||
			( actionCommand > eActionCommand.__StartUSkill && actionCommand < eActionCommand.__EndUSkill ) ||
			( actionCommand > eActionCommand.__StartBossSkill && actionCommand < eActionCommand.__EndBossSkill ) ||
			( actionCommand > eActionCommand.__StartWpnSkill && actionCommand < eActionCommand.__EndWpnSkill ) ) {
			return true;
		}

		return false;
	}

	public bool IsNormalAttackAction( eActionCommand actionCommand ) {
		if( actionCommand == eActionCommand.None ) {
			return false;
		}

		if( actionCommand > eActionCommand.__StartAttack && actionCommand < eActionCommand.__EndAttack ||
            actionCommand > eActionCommand.__StartExtraAttack && actionCommand < eActionCommand.__EndExtraAttack ) {
			return true;
		}

		Player player = m_owner as Player;
		if( player ) {
			Unit unitWeapon = player.GetCurrentUnitWeaponOrNull();
			if( unitWeapon ) {
				return unitWeapon.actionSystem.IsNormalAttackAction( actionCommand );
			}
		}

		return false;
	}

	public bool IsSkillAction(eActionCommand actionCommand)
    {
        if (actionCommand == eActionCommand.None)
            return false;

        if (actionCommand > eActionCommand.__StartSkill && actionCommand < eActionCommand.__EndSkill)
            return true;

        if (actionCommand > eActionCommand.__StartWpnSkill && actionCommand < eActionCommand.__EndWpnSkill)
            return true;

        return false;
    }

    public bool IsQTEAction(eActionCommand actionCommand)
    {
        if (actionCommand == eActionCommand.None)
            return false;

        if (actionCommand > eActionCommand.__StartQTE && actionCommand < eActionCommand.__EndQTE)
            return true;

        return false;
    }

    public bool IsUSkillAction(eActionCommand actionCommand)
    {
        if (actionCommand == eActionCommand.None)
            return false;

        if (actionCommand > eActionCommand.__StartUSkill && actionCommand < eActionCommand.__EndUSkill)
            return true;

        return false;
    }

    public bool IsHitAction(eActionCommand actionCommand)
    {
        if (actionCommand == eActionCommand.None)
            return false;

        if (actionCommand > eActionCommand.__StartHit && actionCommand < eActionCommand.__EndHit)
            return true;

        return false;
    }

    public bool IsSupporterAction(eActionCommand actionCommand)
    {
        if (actionCommand == eActionCommand.None)
            return false;

        if (actionCommand > eActionCommand.__StartSupporterSkill && actionCommand < eActionCommand.__EndSupporterSkill)
            return true;

        return false;
    }

    public bool IsMoveAction(eActionCommand actionCommand)
    {
        if(actionCommand == eActionCommand.MoveByDirection || actionCommand == eActionCommand.MoveToTarget)
        {
            return true;
        }

        return false;
    }

    public bool IsDashAction(eActionCommand actionCommand)
    {
        if(actionCommand == eActionCommand.Dash || actionCommand == eActionCommand.BackDash)
        {
            return true;
        }

        return false;
    }

    public void IncreaseSuperArmorDurationBySkill(float addRatio)
    {
        for(int i = 0; i < ListAction.Count; i++)
        {
            ActionSelectSkillBase action = ListAction[i] as ActionSelectSkillBase;
            if(action == null)
            {
                continue;
            }

            action.IncreaseSuperArmorDuration(addRatio);
        }
    }

	public void ResetAllSetAddActioin()
	{
		for(int i = 0; i < ListAction.Count; i++)
		{
			ActionSelectSkillBase action = ListAction[i] as ActionSelectSkillBase;
			if (action == null)
			{
				continue;
			}

			action.ResetAddAction();
		}
	}
}
