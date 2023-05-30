
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public enum eBTType
{
    Root = 0,
    Sequence,
    Selector,
    Condition,
    Action,
    RemoveAction,
    HangingAround,
    CallFunc,
    Attack,
    Wait,
    CoolTime,
    Input,
    WaitAction,
}

public enum eCompareType
{
    None = 0,

    Equal,
    NotEqual,
    Less,
    LessEqual,
    Greater,
    GreaterEqual,
}

public enum eOwnerReferenceValue
{
    None = 0,

    CurHp,
    CurHpPercentage,
    CurSpPercentage,
    AniSpeed,
    HasTarget,
    HasHitTarget,
    IsTarget,
    InAttackRange,
	InDoubleAttackRange,
    InNormalAttackRange,
    InNormalAttackRangeForEnemy,
	IsNoAttack,
    IsReachTarget,
    IsState,
    IsAttacking,
    HasParts,
    HasAction,
    //NextPatternIndex,
    //NextPatternIndex2,
    CheckDistance,
    CheckDistanceForEnemy,
    CheckDistanceForOwnerPlayer,
    CheckCurrentAction,
    GlobalRandomValue1,
    GlobalRandomValue2,
    GlobalRandomValue3,
    GlobalRandomValue4,
    GlobalRandomValue5,
    LocalRandomValue,
    IsAttack,
    HasMesh,
    HasMinion,
    IsSummoningMinion,
    IsAllMinionDead,

    IsHit,
    IsActionPlaying,
    IsActionPlayingForOwnerPlayer,
    IsContinuingDash,
    IsTargetActionPlaying,
	IsTargetAnyAttacking,
    IsAnySkillAttacking,
	IsAnySkillAttackingForOwnerPlayer,
	IsSlow,
    IsTargetSlow,
    IsSkipAttack,
	IsSkipAttackForOwnerPlayer,

    IsAutoGuardianSkill,
    HasChargingAttack,
    HasHoldingDefBtnAttack,
    HasRushAttack,
    HasTeleport,
    HasAttackDuringAttack,
    HasTimingHoldAttack,

    IsMurasakiChargeComboAttack,

    HasActiveWeaponSkill,
    HasSupporterActiveSkill,

    IsRepeatSkill,

    CheckTimingHoldFrame,

    UseAttack02,

    IsDashBtnFlash,

    IsPVPStage,
}

public enum eOwnerCallFunc
{
    None = 0,

    SetTargetToPlayer,
    SetTargetToOtherObject,
}

public enum eButtonType
{
    None = 0,

    Attack,
    AttackHold,
    StartCharge,
    EndCharge,
    Dash,
    DashHold,
    WeaponSkill,
    USkill,
    SupporterSkill,

    TargetingAttack,
    EndTargetingAttack,

    DoubleDash,
}


public abstract class BTNode
{
    public enum eBTState
    {
        Success = 0,
        Fail,
        Running
    }


    protected string				m_name				= "";
    protected Unit					m_owner				= null;
    protected Player                mOwnerPlayer        = null;
    protected List<BTNode>			m_listChild			= new List<BTNode>();
	protected WaitForFixedUpdate	mWaitForFixedUpdate	= new WaitForFixedUpdate();

	public eBTState	btState	{ get; protected set; }
	public string	Name	{ get { return m_name; } }


    public BTNode(string name, Unit owner)
    {
        m_name = name;

        m_owner = owner;
        mOwnerPlayer = m_owner as Player;

		PlayerGuardian playerGuardian = m_owner as PlayerGuardian;
		if ( playerGuardian != null && playerGuardian.OwnerPlayer != null ) {
			mOwnerPlayer = playerGuardian.OwnerPlayer;
		}
	}

    public abstract IEnumerator Invoke(BTNode root);

    public void AddChild(BTNode node)
    {
        m_listChild.Add(node);
    }
}

// Child가 Success이면 순서대로 진행. 
// 모두 Success이거나 하나라도 Fail이면 종료.
public class BTSequence : BTNode
{
    public BTSequence(string name, Unit owner) : base(name, owner) { }

    public override IEnumerator Invoke(BTNode root)
    {
        for (int i = 0; i < m_listChild.Count; i++)
        {
            yield return m_listChild[i].Invoke(root);

            if (/*m_owner.isStopBT == true ||*/ m_listChild[i].btState == eBTState.Fail)
            {
                btState = eBTState.Fail;
                yield break;
            }
        }

        btState = eBTState.Success;
    }
}

// Child가 Success이면 순서대로 진행. 
// 하나라도 Fail이면 종료. 모두 Success면 루프
public class BTSequenceLoop : BTNode
{
    public BTSequenceLoop(string name, Unit owner) : base(name, owner) { }

    public override IEnumerator Invoke(BTNode root)
    {
        int index = 0;
        while (btState != eBTState.Fail)
        {
            yield return m_listChild[index].Invoke(root);

            /*if(m_owner.isStopBT == true)
            {
                btState = eBTState.Fail;
                yield break;
            }*/

            if (m_listChild[index].btState == eBTState.Fail)
                btState = eBTState.Fail;

            ++index;
            if (index >= m_listChild.Count)
                index = 0;
        }

        btState = eBTState.Success;
    }
}

// Child가 Fail이면 순서대로 진행.
// 모두 Fail이거나 하나라도 Success이면 종료
public class BTSelector : BTNode
{
    public BTSelector(string name, Unit owner) : base(name, owner) { }

    public override IEnumerator Invoke(BTNode root)
    {
        for (int i = 0; i < m_listChild.Count; i++)
        {
			/*
#if UNITY_EDITOR
			Player player = m_owner as Player;
			if (player && !player.OpponentPlayer)
			{
				Debug.LogError(m_owner.tableName + " Current BTNode in BTSelector : " + m_listChild[i].Name);
			}
#endif
*/
			yield return m_listChild[i].Invoke(root);

            /*if(m_owner.isStopBT == true)
            {
                btState = eBTState.Fail;
                yield break;
            }*/

            if (m_listChild[i].btState == eBTState.Success)
            {
                btState = eBTState.Success;
                yield break;
            }
        }

        btState = eBTState.Fail;
    }
}

// AI당 하나만 존재함
public class BTRoot : BTSelector
{
    public class RandomValue
    {
        public int Value { get; set; } = 0;
        public int Check { get; set; } = 0;
    }


    public RandomValue RandomValue1 = new RandomValue();
    public RandomValue RandomValue2 = new RandomValue();
    public RandomValue RandomValue3 = new RandomValue();
    public RandomValue RandomValue4 = new RandomValue();
    public RandomValue RandomValue5 = new RandomValue();

    private int mEndInvokeCount = 0;


    public BTRoot(string name, Unit owner) : base(name, owner) 
    { 
    }

    public void InitRandomValues()
    {
        RandomValue1.Value = UnityEngine.Random.Range(1, 101);
        RandomValue1.Check = 0;

        RandomValue2.Value = UnityEngine.Random.Range(1, 101);
        RandomValue2.Check = 0;

        RandomValue3.Value = UnityEngine.Random.Range(1, 101);
        RandomValue3.Check = 0;

        RandomValue4.Value = UnityEngine.Random.Range(1, 101);
        RandomValue4.Check = 0;

        RandomValue5.Value = UnityEngine.Random.Range(1, 101);
        RandomValue5.Check = 0;
    }

    public override IEnumerator Invoke(BTNode root)
    {
        mEndInvokeCount = 0;

        while (m_owner.IsActivate() == true)
        {
            if (mEndInvokeCount == 0)
            {
                InitRandomValues();
            }

			/*
#if UNITY_EDITOR
			Player player = m_owner as Player;
			if (player && !player.OpponentPlayer)
			{
				Debug.LogError(m_owner.tableName + " Current BTNode in BTRoot : " + m_listChild[mEndInvokeCount].Name);
			}
#endif
*/
			yield return m_listChild[mEndInvokeCount].Invoke(root);

            ++mEndInvokeCount;
            if (mEndInvokeCount >= m_listChild.Count)
            {
                mEndInvokeCount = 0;
            }

            //m_owner.isHit = false;
        }
    }

    public IEnumerator UpdateTimer()
    {
        while(true)
        {
            yield return null;
        }
    }
}

public class BTCondition : BTNode
{
    private eCompareType m_eCompareType;
    private eOwnerReferenceValue m_eOwnerRefValue;
    private object m_value = null;
    private string m_param = null;


    public BTCondition(string name, Unit owner) : base(name, owner) 
    { 
    }

    public void Set(eOwnerReferenceValue reference, object value, eCompareType compareType)
    {
        m_eOwnerRefValue = reference;
        m_value = value;
        m_eCompareType = compareType;
    }

    public void Set(string reference, string value, string compareType, string param)
    {
        int rInt = 0;
        float rFloat = 0.0f;

        if (value == "OtherObject")
            m_value = World.Instance.EnemyMgr.otherObject;
        else if (value == "Player")
            m_value = World.Instance.Player;
        else if (value == "true" || value == "false")
            m_value = value == "true" ? true : false;
        else if (int.TryParse(value, out rInt) == true)
            m_value = rInt;
        else if (Utility.SafeTryParse(value, out rFloat) == true)
            m_value = rFloat;
        else
            m_value = value;

        m_param = param;

        m_eOwnerRefValue = (eOwnerReferenceValue)Enum.Parse(typeof(eOwnerReferenceValue), reference);
        if (m_eOwnerRefValue == eOwnerReferenceValue.None)
            Debug.LogError(reference + "는 eOwnerReferenceValue가 아닙니다.");

        if (!string.IsNullOrEmpty(compareType))
        {
            m_eCompareType = (eCompareType)Enum.Parse(typeof(eCompareType), compareType);
            if (m_eCompareType == eCompareType.None)
                Debug.LogError(compareType + "는 eCompareType이 아닙니다.");
        }
    }

	private DateTime mCheckLocalValueTime = DateTime.Now;
    public override IEnumerator Invoke(BTNode root)
    {
        btState = eBTState.Fail;

        if (m_owner.curHp <= 0.0f || m_eOwnerRefValue == eOwnerReferenceValue.None || m_value == null)// || m_owner.isStopBT == true)
        {
            yield break;
        }

        BTRoot btRoot = root as BTRoot;
        eActionCommand actionCommand = eActionCommand.None;

        float curHp = m_owner.curHp;
        float maxHp = m_owner.maxHp;
        if (m_owner.parent)
        {
            curHp = m_owner.parent.curHp;
            maxHp = m_owner.parent.maxHp;
        }

        float curSp = m_owner.curSp;
        if (m_owner.parent)
        {
            curSp = m_owner.parent.curSp;
        }

        int randHitCount = UnityEngine.Random.Range(5, 10);

        UnitCollider targetCollider = null;
        switch( m_eOwnerRefValue ) {
            case eOwnerReferenceValue.CurHp:
                btState = CheckValueType<float>( curHp );
                break;

            case eOwnerReferenceValue.CurHpPercentage:
                btState = CheckValueType<float>( ( curHp / maxHp ) * 100.0f );
                break;

            case eOwnerReferenceValue.CurSpPercentage:
                btState = CheckValueType<float>( ( curSp / GameInfo.Instance.BattleConfig.USMaxSP ) * 100.0f );
                break;

            case eOwnerReferenceValue.AniSpeed:
                btState = CheckValueType<float>( m_owner.aniEvent.aniSpeed );
                break;

            case eOwnerReferenceValue.HasTarget:
                targetCollider = m_owner.GetMainTargetCollider( false );
                if( targetCollider ) {
                    if( targetCollider.Owner.actionSystem.IsCurrentQTEAction() || targetCollider.Owner.actionSystem.IsCurrentUSkillAction() ) // QTE나 오의 사용 중엔 공격하지 않는다.
                        btState = eBTState.Fail;//m_owner.HasTarget());
                    else
                        btState = eBTState.Success;
                }
                else {
                    btState = CheckBoolType( targetCollider != null );
                    //btState = eBTState.Fail;
                }
                break;

            case eOwnerReferenceValue.HasHitTarget:
                btState = CheckBoolType( m_owner.HasHitTarget() );
                break;

            case eOwnerReferenceValue.IsTarget:
                btState = CheckObjectType( (UnityEngine.Object)m_value );
                break;

            case eOwnerReferenceValue.InAttackRange:
            case eOwnerReferenceValue.InDoubleAttackRange:
                actionCommand = Utility.GetActionCommandByString( m_param );
                btState = CheckBoolType( m_owner.CheckTargetDist( actionCommand, m_eOwnerRefValue == eOwnerReferenceValue.InDoubleAttackRange ? 1.8f : 1.2f ) );
                break;
            case eOwnerReferenceValue.InNormalAttackRange: {
				actionCommand = Utility.GetActionCommandByString( m_param );
				btState = CheckBoolType( m_owner.CheckTargetDist( actionCommand, 1.2f, false ) );
			}
            break;

            case eOwnerReferenceValue.InNormalAttackRangeForEnemy: {
				actionCommand = Utility.GetActionCommandByString( m_param );
				btState = CheckBoolType( m_owner.CheckTargetDist( actionCommand, 1.0f ) );
			}
            break;

			case eOwnerReferenceValue.IsNoAttack:
                btState = CheckBoolType( m_owner.isNoAttack );
                break;

            case eOwnerReferenceValue.IsReachTarget:
                targetCollider = m_owner.GetMainTargetCollider( false );
                if( targetCollider == null ) {
                    btState = eBTState.Fail;
                }
                else {
                    float dist = m_owner.GetDistance(targetCollider.Owner);
                    btState = dist <= Utility.SafeParse( m_param ) ? eBTState.Success : eBTState.Fail;
                }
                break;

            case eOwnerReferenceValue.IsState:
                btState = CheckStateType( (string)m_value );
                break;

            case eOwnerReferenceValue.IsAttacking:
                if( !m_owner.actionSystem.IsCurrentAnyAttackAction() )
                    btState = eBTState.Success;
                else
                    btState = eBTState.Fail;
                break;

            case eOwnerReferenceValue.HasParts:
                EnemyParts enemyParts = m_owner as EnemyParts;
                if( enemyParts == null )
                    btState = eBTState.Fail;
                else
                    btState = CheckBoolType( enemyParts.hasParts );
                break;

            case eOwnerReferenceValue.HasAction: {
				Unit unit = m_owner;
				eActionCommand checkCommand = Utility.GetActionCommandByString( (string)m_value );

				if ( unit.actionSystem.HasAction( checkCommand ) ) {
					if ( checkCommand == eActionCommand.USkill01 && mOwnerPlayer && mOwnerPlayer.IsHelper ) {
						btState = mOwnerPlayer.AutoUltimateSkill ? eBTState.Success : eBTState.Fail;
					}
					else {
						btState = eBTState.Success;
					}
				}
				else
					btState = eBTState.Fail;
			}
            break;

            /*case eOwnerReferenceValue.NextPatternIndex:
                btState = CheckValueType<int>(m_owner.patternIndex);
                break;

            case eOwnerReferenceValue.NextPatternIndex2:
                btState = CheckValueType<int>(m_owner.patternIndex2);
                break;*/

            case eOwnerReferenceValue.CheckDistance: {
				Unit unit = m_owner;
				if ( m_owner is PlayerGuardian && mOwnerPlayer != null ) {
					unit = mOwnerPlayer;
				}

				targetCollider = unit.GetMainTargetCollider( false );
				if ( targetCollider == null ) {
                    if ( m_owner is PlayerGuardian ) {
						if ( m_eCompareType == eCompareType.Greater || m_eCompareType == eCompareType.GreaterEqual ) {
							btState = eBTState.Success;
						}
                        else {
							btState = eBTState.Fail;
						}
					}
                    else {
						btState = eBTState.Fail;
					}
                }	
				else {
					Vector3 dest = targetCollider.Owner.transform.position;// + Vector3.Normalize(m_owner.transform.position - target.transform.position);
					dest.y = unit.transform.position.y;

					float dist = Vector3.Distance( unit.transform.position, dest );
                    if ( m_owner is PlayerGuardian ) {
						float originValue = 0.0f;
						if ( targetCollider.Owner.grade == Unit.eGrade.Boss ) {
							originValue = (float)m_value;
							m_value = originValue * 3.0f;
						}

						btState = CheckValueType<float>( dist );

						if ( targetCollider.Owner.grade == Unit.eGrade.Boss ) {
							m_value = originValue;
						}
					}
                    else {
						btState = CheckValueType<float>( dist );
					}
				}
			}
            break;

            case eOwnerReferenceValue.CheckDistanceForEnemy: {
				Unit unit = m_owner;
				if ( m_owner is PlayerGuardian && mOwnerPlayer != null ) {
					unit = mOwnerPlayer;
				}

				targetCollider = unit.GetMainTargetCollider( true, (float)m_value );
                if ( targetCollider ) {
					float originValue = 0.0f;
					if ( targetCollider.Owner.grade == Unit.eGrade.Boss ) {
						originValue = (float)m_value;
						m_value = originValue * 5.0f;
                    }

					float dist = Utility.GetDistanceWithoutY( targetCollider.Owner.transform.position, unit.transform.position );
					btState = CheckValueType<float>( dist );

					if ( targetCollider.Owner.grade == Unit.eGrade.Boss ) {
						m_value = originValue;
					}
				}
                else {
					if ( m_param.Equals("null") ) {
						btState = eBTState.Success;
					}
					else {
						btState = eBTState.Fail;
					}
				}
			}
            break;

            case eOwnerReferenceValue.CheckDistanceForOwnerPlayer: {
                float dist = 0.0f;
				PlayerGuardian playerGuardian = m_owner as PlayerGuardian;
				if ( playerGuardian != null && playerGuardian.OwnerPlayer != null ) {
					dist = Vector3.Distance( playerGuardian.OwnerPlayer.transform.position, m_owner.transform.position );
                }
                btState = CheckValueType<float>( dist );
            }
            break;

            case eOwnerReferenceValue.CheckCurrentAction:
                eActionCommand command = Utility.GetActionCommandByString((string)m_value);
                if( m_owner.actionSystem.BeforeActionCommand == command ) // 하나의 액션이 끝나야 다음 노드가 실행되므로 이전 액션을 검사해줘야함
                    btState = eBTState.Success;
                else
                    btState = eBTState.Fail;
                break;

            case eOwnerReferenceValue.GlobalRandomValue1:
                btRoot.RandomValue1.Check += (int)m_value;
                if( btRoot.RandomValue1.Value <= btRoot.RandomValue1.Check )
                    btState = eBTState.Success;
                else
                    btState = eBTState.Fail;
                break;

            case eOwnerReferenceValue.GlobalRandomValue2:
                btRoot.RandomValue2.Check += (int)m_value;
                if( btRoot.RandomValue2.Value <= btRoot.RandomValue2.Check )
                    btState = eBTState.Success;
                else
                    btState = eBTState.Fail;
                break;

            case eOwnerReferenceValue.GlobalRandomValue3:
                btRoot.RandomValue3.Check += (int)m_value;
                if( btRoot.RandomValue3.Value <= btRoot.RandomValue3.Check )
                    btState = eBTState.Success;
                else
                    btState = eBTState.Fail;
                break;

            case eOwnerReferenceValue.GlobalRandomValue4:
                btRoot.RandomValue4.Check += (int)m_value;
                if( btRoot.RandomValue4.Value <= btRoot.RandomValue4.Check )
                    btState = eBTState.Success;
                else
                    btState = eBTState.Fail;
                break;

            case eOwnerReferenceValue.GlobalRandomValue5:
                btRoot.RandomValue5.Check += (int)m_value;
                if( btRoot.RandomValue5.Value <= btRoot.RandomValue5.Check )
                    btState = eBTState.Success;
                else
                    btState = eBTState.Fail;
                break;

            case eOwnerReferenceValue.LocalRandomValue:
                if( ( DateTime.Now - mCheckLocalValueTime ).Seconds > 0.5f ) {
                    int random = UnityEngine.Random.Range(1, 101);// (int)(m_value) + 1);
                    btState = CheckValueType<int>( random );

                    mCheckLocalValueTime = DateTime.Now;
                }
                else {
                    btState = eBTState.Fail;
                }
                break;

            case eOwnerReferenceValue.IsAttack:
                btState = btState = CheckBoolType( m_owner.curSuccessAttack );
                if( btState == eBTState.Fail ) {
                    m_owner.ResetBT();
                }
                break;

            case eOwnerReferenceValue.HasMesh:
                if( m_owner.aniEvent.HasMesh( (string)m_value ) ) {
                    btState = eBTState.Success;
                }
                else {
                    btState = eBTState.Fail;
                }
                break;

            case eOwnerReferenceValue.HasMinion:
                Enemy enemy = m_owner as Enemy;
                if( enemy == null ) {
                    btState = eBTState.Fail;
                }
                else {
                    if( enemy.ListMinion.Count > 0 ) {
                        btState = eBTState.Success;
                    }
                    else {
                        btState = eBTState.Fail;
                    }
                }
                break;

            case eOwnerReferenceValue.IsSummoningMinion:
                enemy = m_owner as Enemy;
                if( enemy == null ) {
                    btState = eBTState.Fail;
                }
                else {
                    btState = CheckBoolType( enemy.SummoningMinion );
                }
                break;

            case eOwnerReferenceValue.IsAllMinionDead:
                enemy = m_owner as Enemy;
                if( enemy == null ) {
                    btState = eBTState.Fail;
                }
                else {
                    if( enemy.IsAllMinionDead() ) {
                        btState = eBTState.Success;
                    }
                    else {
                        btState = eBTState.Fail;
                    }
                }
                break;

            case eOwnerReferenceValue.IsHit: {
                Unit unit = m_owner;
                if ( m_owner is PlayerGuardian && mOwnerPlayer != null ) {
					unit = mOwnerPlayer;
				}

                ActionHit actionHit = unit.actionSystem.GetCurrentAction<ActionHit>();
                btState = CheckBoolType( unit.actionSystem.IsCurrentHitAction() || ( actionHit && actionHit.State == ActionHit.eState.None ) );
                /*
				if (!m_owner.actionSystem.IsCurrentHitAction())
                {
                    btState = eBTState.Fail;
                }
                else
                {
                    ActionHit actionHit = m_owner.actionSystem.GetCurrentAction<ActionHit>();
                    if (actionHit == null)
                    {
                        btState = eBTState.Fail;
                    }
                    else
                    {
                        btState = CheckBoolType(actionHit.State == ActionHit.eState.Normal);
                    }
                }
				*/
                }
                break;

            case eOwnerReferenceValue.IsActionPlaying:
                actionCommand = Utility.GetEnumByString<eActionCommand>( m_param );
                btState = CheckBoolType( m_owner.actionSystem.IsCurrentAction( actionCommand ) );
                break;

            case eOwnerReferenceValue.IsActionPlayingForOwnerPlayer: {
				actionCommand = Utility.GetEnumByString<eActionCommand>( m_param );
				bool isActionPlaying = false;

				ActionBase actionBase = mOwnerPlayer.actionSystem.GetAction( actionCommand );
				if ( actionBase ) {
					isActionPlaying = actionBase.isPlaying;
				}

				btState = CheckBoolType( isActionPlaying );
			}
            break;

			case eOwnerReferenceValue.IsContinuingDash:
                Player continuingDashPlayer = m_owner as Player;
                if( continuingDashPlayer == null ) {
                    btState = eBTState.Fail;
                }
                else {
                    btState = CheckBoolType( continuingDashPlayer.ContinuingDash );
                }
                break;

            case eOwnerReferenceValue.IsTargetActionPlaying:
                if( m_owner.mainTarget == null ) {
                    btState = eBTState.Fail;
                }
                else {
                    actionCommand = Utility.GetEnumByString<eActionCommand>( m_param );
                    btState = CheckBoolType( m_owner.mainTarget.actionSystem.IsCurrentAction( actionCommand ) );
                }
                break;

            case eOwnerReferenceValue.IsTargetAnyAttacking:
                List<Unit> listEnemy = World.Instance.EnemyMgr.GetActiveEnemies( null );
                if( listEnemy.Count <= 0 )
				//if (m_owner.mainTarget == null)
				{
					btState = eBTState.Fail;
				}
				else
				{
                    btState = eBTState.Fail;

                    for( int i = 0; i < listEnemy.Count; i++ ) {
                        if( listEnemy[i].actionSystem == null || listEnemy[i].actionSystem.currentAction == null ) {
                            continue;
						}

                        if( listEnemy[i].actionSystem.IsCurrentAnyAttackAction() ) {
                            btState = eBTState.Success;
                            break;
						}
                    }

                    /*
					Unit mainTarget = m_owner.mainTarget;
					if(mainTarget == null)
					{
						btState = eBTState.Fail;
					}
					else
					{
						ActionBase currentAction = mainTarget.actionSystem.currentAction;
						if (currentAction == null)
						{
							btState = eBTState.Fail;
						}
						else
						{
							actionCommand = currentAction.actionCommand;
							btState = m_owner.mainTarget.actionSystem.IsAnyAttackAction(actionCommand) ? eBTState.Success : eBTState.Fail;
						}
					}
                    */
				}
				break;

			case eOwnerReferenceValue.IsAnySkillAttacking:
                btState = CheckBoolType( m_owner.actionSystem.IsCurrentSkillAction() );
                break;

            case eOwnerReferenceValue.IsAnySkillAttackingForOwnerPlayer: {
				btState = CheckBoolType( mOwnerPlayer.actionSystem.IsCurrentSkillAction() );
			}
            break;

			case eOwnerReferenceValue.IsSlow:
                if (m_owner.aniEvent == null)
                {
                    btState = eBTState.Fail;
                }
                else
                {
                    btState = CheckBoolType(m_owner.aniEvent.aniSpeed < 1.0f);
                }
                break;

            case eOwnerReferenceValue.IsTargetSlow:
                if (m_owner.mainTarget == null || m_owner.aniEvent == null)
                {
                    btState = eBTState.Fail;
                }
                else
                {
                    btState = CheckBoolType(m_owner.mainTarget.aniEvent.aniSpeed < 1.0f);
                }
                break;

            case eOwnerReferenceValue.IsSkipAttack: {
                btState = CheckBoolType( m_owner.skipAttack );
            }
            break;

            case eOwnerReferenceValue.IsSkipAttackForOwnerPlayer: {
				btState = CheckBoolType( mOwnerPlayer.skipAttack );
			}
            break;

            case eOwnerReferenceValue.IsAutoGuardianSkill: {
				btState = CheckBoolType( mOwnerPlayer.AutoGuardianSkill );
			}
            break;

			case eOwnerReferenceValue.HasChargingAttack:
			case eOwnerReferenceValue.HasHoldingDefBtnAttack:
			case eOwnerReferenceValue.HasRushAttack:
			case eOwnerReferenceValue.HasTeleport:
			case eOwnerReferenceValue.HasAttackDuringAttack:
			case eOwnerReferenceValue.HasTimingHoldAttack: {
				string strActionCommand = ( m_eOwnerRefValue.ToString().Replace( "Has", "" ) );
				eActionCommand hasActionCommand = Utility.GetEnumByString<eActionCommand>( strActionCommand );

				ActionSelectSkillBase actionSelectSkill = m_owner.actionSystem.GetAction<ActionSelectSkillBase>( hasActionCommand );
				if ( actionSelectSkill ) {
					btState = CheckBoolType( actionSelectSkill.PossibleToUseInAI() && m_owner.CheckTargetDist( hasActionCommand ) );
				}
				else {
					btState = eBTState.Fail;
				}
			}
			break;

			case eOwnerReferenceValue.IsMurasakiChargeComboAttack:
                ActionMurasakiComboAttack murasakiComboAttack = m_owner.actionSystem.GetCurrentAction<ActionMurasakiComboAttack>();
                if(murasakiComboAttack == null)
                {
                    btState = eBTState.Fail;
                }
                else
                {
                    if(!murasakiComboAttack.IsLastAttack())
                    {
                        btState = eBTState.Success;
                    }
                    else 
                    { 
                        btState = eBTState.Fail;
                    }
                }
                break;

			case eOwnerReferenceValue.HasActiveWeaponSkill:
				if( mOwnerPlayer == null ) {
					btState = eBTState.Fail;
				}
				else {
					btState = CheckBoolType( mOwnerPlayer.HasWeaponActiveSkill() && mOwnerPlayer.IsActiveWeaponSkill() );

                    if( btState == eBTState.Success ) {
                        if( mOwnerPlayer.IsHelper && !mOwnerPlayer.AutoWeaponSkill ) {
                            btState = eBTState.Fail;
                        }
                    }
                }
				break;

			case eOwnerReferenceValue.HasSupporterActiveSkill:
				if( mOwnerPlayer == null ) {
					btState = eBTState.Fail;
				}
				else {
					btState = CheckBoolType( mOwnerPlayer.boSupporter != null && mOwnerPlayer.boSupporter.HasActiveSkill() &&
											 World.Instance.UIPlay.sprSupporterCoolTime.fillAmount <= 0.0f );

                    if( btState == eBTState.Success ) {
                        if( mOwnerPlayer.IsHelper && !mOwnerPlayer.AutoSupporterSkill ) {
                            btState = eBTState.Fail;
						}
					}
				}
				break;

			case eOwnerReferenceValue.IsRepeatSkill: {
				ActionSelectSkillBase actionSelectSkill = m_owner.actionSystem.GetCurrentAction<ActionSelectSkillBase>();
				if ( actionSelectSkill == null ) {
					btState = eBTState.Fail;
				}
				else {
					btState = CheckBoolType( actionSelectSkill.IsRepeatSkill );
				}
			}
			break;

			case eOwnerReferenceValue.CheckTimingHoldFrame:
				AniEvent.sAniInfo aniInfo = m_owner.aniEvent.GetAniInfo( m_owner.aniEvent.curAniType );

				if( m_owner.actionSystem.HasAction( eActionCommand.TimingHoldAttack ) &&
					 aniInfo.timingHoldFrame > 0.0f &&
					 m_owner.aniEvent.GetCurrentFrame() >= aniInfo.timingHoldFrame ) {
					btState = eBTState.Success;
				}
				else {
					btState = eBTState.Fail;
				}
				break;

			case eOwnerReferenceValue.UseAttack02:
                btState = CheckBoolType(m_owner.UseAttack02);
                break;

            case eOwnerReferenceValue.IsPVPStage:
                btState = CheckBoolType( World.Instance.StageType == eSTAGETYPE.STAGE_PVP );
                break;
		}
    }

    private eBTState CheckBoolType(bool ownerRefValue)
    {
        if ((m_eCompareType == eCompareType.Equal && ownerRefValue == (bool)m_value) ||
            (m_eCompareType == eCompareType.NotEqual && ownerRefValue != (bool)m_value))
        {
            return eBTState.Success;
        }

        return eBTState.Fail;
    }

    private eBTState CheckObjectType(UnityEngine.Object unit)
    {
        UnitCollider targetCollider = m_owner.GetMainTargetCollider(false);
        if ((m_eCompareType == eCompareType.Equal && targetCollider.Owner == unit) || (m_eCompareType == eCompareType.NotEqual && targetCollider.Owner != unit))
            return eBTState.Success;

        return eBTState.Fail;
    }

    private eBTState CheckStateType(string state)
    {
        //if(state == "Stun")
        //    return CheckValueType<Unit.eAbnormalCondition>(Unit.eAbnormalCondition.Stun, m_owner.abnormalCondition);

        return eBTState.Fail;
    }

    private eBTState CheckValueType<T>(T ownerRefValue) where T : IComparable
    {
        switch (m_eCompareType)
        {
            case eCompareType.Equal:
                if (ownerRefValue.CompareTo((T)m_value) == 0)
                    return eBTState.Success;
                break;

            case eCompareType.NotEqual:
                if (ownerRefValue.CompareTo((T)m_value) != 0)
                    return eBTState.Success;
                break;

            case eCompareType.Less:
                if (ownerRefValue.CompareTo((T)m_value) < 0)
                    return eBTState.Success;
                break;

            case eCompareType.LessEqual:
                if (ownerRefValue.CompareTo((T)m_value) <= 0)
                    return eBTState.Success;
                break;

            case eCompareType.Greater:
                if (ownerRefValue.CompareTo((T)m_value) > 0)
                    return eBTState.Success;
                break;

            case eCompareType.GreaterEqual:
                if (ownerRefValue.CompareTo((T)m_value) >= 0)
                    return eBTState.Success;
                break;
        }

        return eBTState.Fail;
    }

    private eBTState CheckValueType<T>(T ownerRefValue, T value) where T : IComparable
    {
        switch (m_eCompareType)
        {
            case eCompareType.Equal:
                if (ownerRefValue.CompareTo(value) == 0)
                    return eBTState.Success;
                break;

            case eCompareType.NotEqual:
                if (ownerRefValue.CompareTo(value) != 0)
                    return eBTState.Success;
                break;

            case eCompareType.Less:
                if (ownerRefValue.CompareTo(value) < 0)
                    return eBTState.Success;
                break;

            case eCompareType.LessEqual:
                if (ownerRefValue.CompareTo(value) <= 0)
                    return eBTState.Success;
                break;

            case eCompareType.Greater:
                if (ownerRefValue.CompareTo(value) > 0)
                    return eBTState.Success;
                break;

            case eCompareType.GreaterEqual:
                if (ownerRefValue.CompareTo(value) >= 0)
                    return eBTState.Success;
                break;
        }

        return eBTState.Fail;
    }
}

public class BTCoolTime : BTNode
{
    private float m_coolTime = 0.0f;
    private float m_curTime = 0.0f;
    private bool m_start = false;


    public BTCoolTime(string name, Unit owner) : base(name, owner) { }


    public void Set(float coolTime)
    {
        m_coolTime = coolTime;
        m_curTime = 0.0f;

        m_start = false;
    }

    public override IEnumerator Invoke(BTNode root)
    {
        if(!m_start)
        {
            m_owner.StartCoroutine(UpdateCoolTime());
            m_start = true;
        }

        if (m_curTime >= m_coolTime)
        {
            m_start = false;
            m_curTime = 0.0f;

            btState = eBTState.Success;
        }
        else
            btState = eBTState.Fail;

        yield return null;
    }

    private IEnumerator UpdateCoolTime()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            m_curTime += m_owner.fixedDeltaTime;
            if (m_curTime >= m_coolTime)
                yield break;

            yield return mWaitForFixedUpdate;
        }
    }
}

public class BTAction : BTNode
{
    private eActionCommand  mActionCommand;
    private ActionBase      mAction;
    private eDirection      mDirection          = eDirection.None;
    private float           mMinValue           = 0.0f;
    private float           mMaxValue           = 0.0f;
    private eActionCommand  mDistActionCommand  = eActionCommand.None;
    private bool            mPosByTargetDir     = false;
    private bool            mbLookAtByRotate    = false;


    public BTAction(string name, Unit owner) : base(name, owner) 
    { 
    }

    public void Set(eActionCommand actionCommand)
    {
        mActionCommand = actionCommand;
        mAction = m_owner.actionSystem.GetAction(actionCommand);
    }

    public void Set(string actionCommand, string direction, float min, float max, string distCommand, string posByTargetDir, bool lookAtByRotate)
    {
        mActionCommand = Utility.GetActionCommandByString(actionCommand);

        mAction = m_owner.actionSystem.GetAction(mActionCommand);
        if (mAction == null)
        {
            Debug.LogWarning(m_owner.tableName + "의 " + mActionCommand + " Action이 없어!!");
        }

        if (!string.IsNullOrEmpty(direction))
        {
            mDirection = Utility.GetEnumByString<eDirection>(direction);
        }

        mMinValue = min;
        mMaxValue = max;

        if (!string.IsNullOrEmpty(distCommand))
        {
            mDistActionCommand = Utility.GetActionCommandByString(distCommand);
        }

        if (!string.IsNullOrEmpty(posByTargetDir))
        {
            mPosByTargetDir = posByTargetDir == "true" ? true : false;
        }

        mbLookAtByRotate = lookAtByRotate;
    }

    public override IEnumerator Invoke(BTNode root)
    {
        btState = eBTState.Running;

        UnitCollider unitCollider = m_owner.GetMainTargetCollider(true);
        if(unitCollider && unitCollider.Owner && !unitCollider.Owner.IsShowMesh && unitCollider.Owner.TemporaryInvincible)
        {
            if (mActionCommand == eActionCommand.MoveToTarget || m_owner.actionSystem.IsAnyAttackAction(mActionCommand))
            {
                btState = eBTState.Fail;
                yield break;
            }
        }

        if(mActionCommand == eActionCommand.Die)
        {
            m_owner.aniEvent.SetAniSpeed(1.0f);
        }

        if (m_owner.actionSystem.IsAnyAttackAction(mActionCommand) && m_owner.IsAttackImpossible())
        {
            btState = eBTState.Fail;
            yield break;
        }

        m_owner.CommandAction(mActionCommand, new ActionParamAI(mDirection, mMinValue, mMaxValue, mDistActionCommand, mPosByTargetDir, mbLookAtByRotate));

        if (mAction == null)
        {
            Debug.LogWarning("BTAction에 Action이 없어!!!!!!");
            Debug.LogWarning("BTAction::ActionCommand : " + mActionCommand);
            Debug.LogWarning("BTAction::DistActionCommand : " + mDistActionCommand);

            btState = eBTState.Fail;
            yield break;
        }

        if (!mAction.isPlaying)
        {
            mAction.OnCancel();
            btState = eBTState.Fail;

            yield break;
        }

        while (mAction.isPlaying == true)
        {
            yield return null;
        }

        btState = eBTState.Success;
    }
}

public class BTRemoveAction : BTNode
{
    private eActionCommand actionCommand;

    public BTRemoveAction(string name, Unit owner) : base(name, owner) { }


    public void Set(string command)
    {
        actionCommand = Utility.GetActionCommandByString(command);
    }

    public override IEnumerator Invoke(BTNode root)
    {
        m_owner.actionSystem.RemoveAction(actionCommand);
        btState = eBTState.Success;

        yield return null;
    }
}

public class BTHangingAround : BTNode
{
    private ActionHangingAround m_actionHangingAround;
    private ActionParamHangingAround m_param;
    private eActionCommand m_distActionCommand;


    public BTHangingAround(string name, Unit owner) : base(name, owner) { }

    public void Set(float minDuration, float maxDuration, string distCommand, string randomCheckDist, string direction, string turn)
    {
        m_actionHangingAround = m_owner.actionSystem.GetAction<ActionHangingAround>(eActionCommand.HangingAround);
        if (m_actionHangingAround == null)
        {
            Debug.LogError(m_owner.name + "에 ActionHangingAround를 추가해 주세요.");
            return;
        }

        if (!string.IsNullOrEmpty(distCommand))
            m_distActionCommand = Utility.GetActionCommandByString(distCommand);
        else
            m_distActionCommand = eActionCommand.None;

        bool isRandomCheckDist = false;
        if (!string.IsNullOrEmpty(randomCheckDist))
            isRandomCheckDist = randomCheckDist == "true" ? true : false;

        eDirection[] directions = null;
        if(direction != null)
        {
			string[] split = Utility.Split(direction, ','); //direction.Split(',');
			if (split.Length > 0)
            {
                directions = new eDirection[split.Length];
                for (int i = 0; i < split.Length; i++)
                {
                    split[i] = split[i].Trim();
                    directions[i] = Utility.GetEnumByString<eDirection>(split[i]);
                    if (directions[i] == eDirection.None)
                        Debug.LogError("잘못된 방향이 들어왔다. " + split[i]);
                }
            }
        }

        bool isTurn = turn == "true" ? true : false;
        m_param = new ActionParamHangingAround(minDuration, maxDuration, m_distActionCommand, isRandomCheckDist, directions, isTurn);
    }

    public override IEnumerator Invoke(BTNode root)
    {
        btState = eBTState.Running;
        m_owner.CommandAction(eActionCommand.HangingAround, m_param);

        while (m_actionHangingAround.isPlaying)
            yield return null;

        btState = eBTState.Success;
    }
}

public class BTCallFunc : BTNode
{
    private eOwnerCallFunc m_eCallFunc;


    public BTCallFunc(string name, Unit owner) : base(name, owner) { }

    public void Set(eOwnerCallFunc callFunc)
    {
        m_eCallFunc = callFunc;
    }

    public void Set(string callFunc)
    {
        m_eCallFunc = (eOwnerCallFunc)Enum.Parse(typeof(eOwnerCallFunc), callFunc);
        if (m_eCallFunc == eOwnerCallFunc.None)
        {
            Debug.LogError(m_eCallFunc + "는 eOwnerCallFunc이 아닙니다.");
            return;
        }
    }

    public override IEnumerator Invoke(BTNode root)
    {
        btState = eBTState.Success;

        switch (m_eCallFunc)
        {
            case eOwnerCallFunc.SetTargetToPlayer:
                m_owner.SetMainTarget(World.Instance.Player);
                break;

            case eOwnerCallFunc.SetTargetToOtherObject:
                m_owner.SetMainTarget(World.Instance.EnemyMgr.otherObject);
                break;

            default:
                btState = eBTState.Fail;
                break;
        }

        yield return null;
    }
}

public class BTInput : BTNode
{
    private eDirection      mDirection      = eDirection.None;
    private eButtonType     mButton         = eButtonType.None;
    private bool            mbRepeat        = false;
    private float           mWaitTime       = 0.0f;
    private float           mWaitAniTime    = 0.0f;
    private float           mHeighest       = 0.0f;
    private float           mCheckTime      = 0.0f;
    private float           mMinChargeTime  = 0.0f;
    private float           mMaxChargeTime  = 0.0f;
    private eAnimation      mWaitAni        = eAnimation.None;
    private bool            mEndLoop        = false;
    private ActionParamAI   mParamAI        = new ActionParamAI();
    private InputEvent      mInputEvent     = new InputEvent();
    private Vector3         mDir            = Vector3.zero;
    private Vector3         mBeforeDir      = Vector3.zero;


    public BTInput(string name, Unit owner) : base(name, owner) 
    { 
    }

    public void Set(string direction, string button, string repeat, string waitTime, string waitCutFrame, string waitAni, string heighest, 
                    string minChargeTime, string maxChargeTime)
    {
        mDirection = Utility.GetEnumByString<eDirection>(direction);
        mButton = Utility.GetEnumByString<eButtonType>(button);
        mbRepeat = (!string.IsNullOrEmpty(repeat) && repeat.CompareTo("true") == 0) ? true : false;
        mWaitTime = string.IsNullOrEmpty(waitTime) ? 0.0f : Utility.SafeParse(waitTime);
        mWaitAniTime = 0.0f;
        mHeighest = string.IsNullOrEmpty(heighest) ? 0.0f : Utility.SafeParse(heighest);
        mMinChargeTime = string.IsNullOrEmpty(minChargeTime) ? 0.0f : Utility.SafeParse(minChargeTime);
        mMaxChargeTime = string.IsNullOrEmpty(maxChargeTime) ? 0.0f : Utility.SafeParse(maxChargeTime);
        mCheckTime = 0.0f;

        if (!string.IsNullOrEmpty(waitAni))
        {
            mWaitAni = GetWaitAni(waitAni);
            if(mWaitAni != eAnimation.None)
            {
                mWaitAniTime = m_owner.aniEvent.GetAniLength(mWaitAni) * 1.25f;
            }
        }
        if(!string.IsNullOrEmpty(waitCutFrame))
        {
            mWaitAni = GetWaitAni(waitCutFrame);
            if (mWaitAni != eAnimation.None)
            {
                mWaitAniTime = m_owner.aniEvent.GetCutFrameLength(mWaitAni) * 0.85f;
            }
        }
    }

	public override IEnumerator Invoke( BTNode root ) {
		if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
			WorldPVP worldPVP = World.Instance as WorldPVP;

			if( worldPVP && worldPVP.IsAnyPlayerDead ) {
				CommandDirection( Vector3.zero );
				btState = eBTState.Success;

				yield break;
			}

            UnitCollider unitCollider = m_owner.GetMainTargetCollider( true );
            if( unitCollider && unitCollider.Owner && !unitCollider.Owner.IsShowMesh && unitCollider.Owner.TemporaryInvincible ) {
                btState = eBTState.Success;
                yield break;
            }
        }

		mParamAI.Direction = mDirection;
        mBeforeDir = Vector3.zero;

		Vector3 v = Utility.GetDirectionVector(m_owner, mDirection);
		CommandDirection( v );

		btState = eBTState.Running;

		do {
			if( mButton != eButtonType.None ) {
				switch( mButton ) {
					case eButtonType.Attack:
						SendEvent( eEventType.EVENT_PLAYER_INPUT_ATK, false );

						btState = eBTState.Running;

						mCheckTime = 0.0f;
						while( mWaitAniTime > 0.0f && mCheckTime < mWaitAniTime ) {
							mCheckTime += Time.deltaTime;
							yield return null;
						}

						break;

					case eButtonType.TargetingAttack:
						SendEvent( eEventType.EVENT_PLAYER_INPUT_ATK_PRESS_START, false );
						break;

					case eButtonType.EndTargetingAttack:
						SendEvent( eEventType.EVENT_PLAYER_INPUT_ATK_TOUCH_END, false );
						break;

					case eButtonType.AttackHold:
						SendEvent( eEventType.EVENT_PLAYER_INPUT_SPECIAL_ATK, false );
						break;

					case eButtonType.StartCharge:
						SendEvent( eEventType.EVENT_PLAYER_INPUT_CHARGE_ATK_START, false, null, mParamAI );

						int chargeCount = UnityEngine.Random.Range(1, 3);
						float chargeTime = World.Instance.UIPlay.btnAtk.m_chargeTime * chargeCount;

						if( mMinChargeTime > 0.0f || mMaxChargeTime > 0.0f ) {
							chargeTime = Utility.GetRandom( mMinChargeTime, mMaxChargeTime, 10.0f );
						}

						mCheckTime = 0.0f;

						while( mCheckTime < chargeTime ) {
							mCheckTime += Time.deltaTime;
							yield return null;
						}
						break;

					case eButtonType.EndCharge:
						SendEvent( eEventType.EVENT_PLAYER_INPUT_CHARGE_ATK_END, false, null, mParamAI );
						break;

					case eButtonType.Dash: {
                        SendEvent( eEventType.EVENT_PLAYER_INPUT_DEFENCE, false, null, mParamAI );
                    }
                    break;

					case eButtonType.DashHold: {
                        SendEvent( eEventType.EVENT_PLAYER_INPUT_JUMP, false );
                    }
                    break;

					case eButtonType.USkill:
						if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
							WorldPVP worldPVP = World.Instance as WorldPVP;

							if( worldPVP && worldPVP.RemainTime < 1.0f ) {
								btState = eBTState.Success;
							}
						}

						if( btState == eBTState.Running ) {
							SendEvent( eEventType.EVENT_PLAYER_INPUT_ULTIMATE_SKILL, false );
						}

						break;

					case eButtonType.WeaponSkill:
						StartWpnSkill();
						break;

					case eButtonType.SupporterSkill:
						StartSupporterSkill();
						break;

					case eButtonType.DoubleDash: {
						StartTeleportSkill();
					}
					break;

					default:
						break;
				}
			}

			if( mHeighest > 0.0f ) {
				while( m_owner.cmptJump.highest < mHeighest ) {
					yield return null;
				}
			}
			else if( mWaitTime > 0.0f ) {
				mCheckTime = 0.0f;
				mEndLoop = false;

				while( mCheckTime < mWaitTime ) {
					mCheckTime += Time.deltaTime;

					if( m_owner.actionSystem.IsCurrentAction( eActionCommand.MoveByDirection ) && mCheckTime >= mWaitTime ) {
						mEndLoop = true;
					}

					yield return null;
				}

				CommandDirection( Vector3.zero );
			}
			else if( mWaitTime <= 0.0f && 
					 mButton == eButtonType.None && 
                     mDirection == eDirection.ToTarget &&
                     m_owner.actionSystem.IsCurrentAction( eActionCommand.MoveByDirection ) &&
                     m_owner.CheckTargetDist( eActionCommand.Attack01 ) ) {
				break;
			}
		}
		while( !mEndLoop && mbRepeat && m_owner.actionSystem.currentAction && m_owner.actionSystem.currentAction.isPlaying );

		btState = eBTState.Success;
	}

	private void CommandDirection( Vector3 dir ) {
        UnitCollider targetCollider = m_owner.GetMainTargetCollider( true );
        if( targetCollider == null ) {
            dir = Vector3.zero;
		}

        mDir = dir;

        if( mDir != mBeforeDir ) {
            SendEvent( eEventType.EVENT_PLAYER_INPUT_DIR, false );
            mBeforeDir = mDir;
        }
    }

    private void SendEvent( eEventType type, bool skipRunStop, System.Action callback = null, IActionBaseParam param = null ) {
        if( m_owner == null || !m_owner.IsActivate() ) {
            return;
        }

        if( World.Instance.InGameCamera.Mode == InGameCamera.EMode.SIDE ) {
            if( World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.Z ) {
                mDir.y = 0.0f;
            }
            else if( World.Instance.InGameCamera.SideSetting.LockAxis == Unit.eAxisType.X ) {
                mDir.x = 0.0f;
            }
        }

        mInputEvent.sender = m_owner;
        mInputEvent.eventType = type;
        mInputEvent.dir = mDir;
        mInputEvent.beforeDir = mBeforeDir;
        mInputEvent.SkipRunStop = skipRunStop;

        EventMgr.Instance.SendEvent( mInputEvent, callback, param );
    }

    private eAnimation GetWaitAni(string aniName)
    {
        eAnimation ani = eAnimation.None;

        if (aniName.CompareTo("Attack01") == 0)
        {
            ActionComboAttack actionComboAtk = m_owner.actionSystem.GetAction<ActionComboAttack>(eActionCommand.Attack01);
            if (actionComboAtk)
            {
                ani = actionComboAtk.attackAnimations[0];
            }
        }
        else if (aniName.CompareTo("Attack02") == 0)
        {
            ActionComboAttack actionComboAtk = m_owner.actionSystem.GetAction<ActionComboAttack>(eActionCommand.Attack01);
            if (actionComboAtk)
            {
                ani = actionComboAtk.attackAnimations[1];
            }
        }
        else if (aniName.CompareTo("Attack03") == 0)
        {
            ActionComboAttack actionComboAtk = m_owner.actionSystem.GetAction<ActionComboAttack>(eActionCommand.Attack01);
            if (actionComboAtk)
            {
                ani = actionComboAtk.attackAnimations[2];
            }
        }
        else if (aniName.CompareTo("Attack04") == 0)
        {
            ActionComboAttack actionComboAtk = m_owner.actionSystem.GetAction<ActionComboAttack>(eActionCommand.Attack01);
            if (actionComboAtk)
            {
                ani = actionComboAtk.attackAnimations[3];
            }
        }
        else if (aniName.CompareTo("Attack05") == 0)
        {
            ActionComboAttack actionComboAtk = m_owner.actionSystem.GetAction<ActionComboAttack>(eActionCommand.Attack01);
            if (actionComboAtk)
            {
                ani = actionComboAtk.attackAnimations[4];
            }
        }
        else
        {
            ani = Utility.GetEnumByString<eAnimation>(aniName);
        }

        return ani;
    }

    private void StartTeleportSkill() {
		if ( World.Instance.IsEndGame || World.Instance.IsPause || m_owner.isPause || m_owner.curHp <= 0.0f ) {
			return;
		}

        m_owner.CommandAction( eActionCommand.Teleport, null );
	}

	private void StartWpnSkill() {
		if( World.Instance.IsEndGame || World.Instance.IsPause || m_owner.isPause || m_owner.curHp <= 0.0f ) {
			return;
		}

		if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
			Player player = m_owner as Player;
			if( !player.HasWeaponActiveSkill() || !player.IsActiveWeaponSkill() ) {
				return;
			}

			ActionWeaponSkillBase actionWeaponSkill = player.actionSystem.GetCurrentAction<ActionWeaponSkillBase>();
			if( actionWeaponSkill != null ) {
				return;
			}

			ActionSelectSkillBase actionSelectSkill = player.actionSystem.GetCurrentAction<ActionSelectSkillBase>();
			if( actionSelectSkill ) {
				return;
			}

			string wpnSkillName = FLocalizeString.Instance.GetText(player.boWeapon.data.TableData.Name);

			World.Instance.UIPVP.ShowSkillName( player, wpnSkillName, UIArenaPlayPanel.eSkillNameType.Weapon );
			EventMgr.Instance.SendEvent( eEventSubject.Self, eEventType.EVENT_PLAYER_INPUT_WEAPON_SKILL, m_owner );
		}
        else {
            Player player = m_owner as Player;
            if( player ) {
                if( !player.IsHelper ) {
                    World.Instance.UIPlay.OnBtnWpnSkill();
                }
                else {
                    player.StartWpnSkill();
				}
            }
		}
	}

	private void StartSupporterSkill() {
        if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
            Player player = m_owner as Player;

            if( !player.OpponentPlayer && !GameInfo.Instance.PVPAutoSupporter || m_owner.curHp <= 0.0f ) {
                return;
            }

            if( !player.isGrounded || player.isPause || player.boSupporter == null || !player.boSupporter.HasActiveSkill() ) {
                return;
            }

            if (!player.StartSupporterSkill())
            {
                return;
            }

            string supporterName = FLocalizeString.Instance.GetText(player.boSupporter.data.TableData.Name);
            World.Instance.UIPVP.ShowSkillName(player, supporterName, UIArenaPlayPanel.eSkillNameType.Supporter);
        }
        else {
            Player player = m_owner as Player;
            if( player && !player.IsHelper ) {
                World.Instance.UIPlay.OnBtnSupporter();
            }
            else if( player && player.IsHelper ) {
                if( !player.OpponentPlayer && m_owner.curHp <= 0.0f || !player.isGrounded || player.isPause || 
                    player.boSupporter == null || !player.boSupporter.HasActiveSkill() ) {
                    return;
                }

                if( !player.StartSupporterSkill() ) {
                    return;
                }
            }
		}
	}
}

public class BTWaitAction : BTNode
{
    private eActionCommand mActionCommand = eActionCommand.None;


    public BTWaitAction(string name, Unit owner) : base(name, owner)
    {
    }

    public void Set(string command)
    {
        mActionCommand = Utility.GetEnumByString<eActionCommand>(command);
    }

    public override IEnumerator Invoke(BTNode root)
    {
        /*
        btState = eBTState.Running;
        if(mActionCommand != eActionCommand.None)
        {
            while(m_owner.actionSystem.IsCurrentAction(mActionCommand))
            {
                yield return null;
            }
        }
        */
        btState = eBTState.Success;
        yield return null;
    }
}
