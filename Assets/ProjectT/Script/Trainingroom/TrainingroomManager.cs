using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingroomManager : BaseEnemyMgr
{
    private static TrainingroomManager s_instance = null;
    public static TrainingroomManager Instance
    {
        get
        {
            return s_instance;
        }
    }

    private bool m_bIsInit = false;

    private Player m_Player = null;
    private CharData m_MainCharData = null;
    public InGameCamera kInGameCamera = null;
    //public PlayerCamera playerCamera = null;

    private GameTable mGameTable;
    private GameClientTable mGameClientTable;

    public int[] EnemyTableId;

    public int kSkillSlotIdx { get; set; }

    private UISkillTrainingPanel m_uiSkillTrainingPanel;
    private List<ActionBase> m_PrevAction = null;

    private ActionComboAttack m_DefaultAttackActionCopy;
    private ActionTargetingAttack mDefaultTargetingAtkActionCopy;


    private void Awake()
    {
        if (s_instance == null)
            s_instance = this;
        else
            Debug.LogError("GameUIManager Instance Error!");
    }

    public void Init(Player player)
    {
        if (m_bIsInit)
            return;

        kSkillSlotIdx = 1;

        m_bIsInit = true;
        mGameTable = (GameTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/Game.asset");
        mGameClientTable = (GameClientTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/GameClient.asset");

        m_Player = player;
        m_Player.SetInitialPosition(new Vector3(0f, 0f, -5f), Quaternion.identity);

        if (m_Player.charData.TableID != (int)ePlayerCharType.Emily)
        {
            DefaultAttackAction_Copy(m_Player.actionSystem.GetAction(eActionCommand.Attack01));
        }

        //InitEnvObjects();
        InitEnemy();

        m_MainCharData = m_Player.charData;

        m_uiSkillTrainingPanel = (UISkillTrainingPanel)GameUIManager.Instance.GetUI("SkillTrainingPanel");
        if (m_uiSkillTrainingPanel != null)
            m_uiSkillTrainingPanel.SetSkill(kSkillSlotIdx);

        m_Player.OnGameStart();
        m_Player.OnMissionStart();

        SetSkillSlot(kSkillSlotIdx);
        World.Instance.ShowGamePlayUI(0);

        base.Init();
    }

    private void InitEnemy()
    {
        if (EnemyTableId.Length <= 0)
        {
            return;
        }

        m_listEnemy.Clear();
        for (int i = 0; i < EnemyTableId.Length; i++)
        {
            Enemy enemy = CreateEnemy(EnemyTableId[i]);
            m_listEnemy.Add(enemy);
        }

        for (int i = 0; i < m_listEnemy.Count; i++)
        {
            if (i != 0)
                m_listEnemy[i].gameObject.SetActive(false);

            m_listEnemy[i].SetInitialPosition(new Vector3(0, 0, 5), Quaternion.identity);
            Enemy enemy = m_listEnemy[i] as Enemy;
        }


        m_listEnemy[0].Activate();
        m_listEnemy[0].LookAtTarget(m_Player.transform.position);
        maxMonsterCountInSpawnGroup = m_listEnemy.Count;
    }

    private Enemy CreateEnemy(int tableId)
    {
        GameClientTable.Monster.Param param = mGameClientTable.FindMonster(tableId);
        if (param == null)
        {
            Debug.LogError(tableId + "번 몬스터가 없습니다.");
            return null;
        }

        string strModel = Utility.AppendString("Unit/", param.ModelPb, ".prefab");

        Enemy enemy = ResourceMgr.Instance.CreateFromAssetBundle<Enemy>("unit", strModel);
        enemy.Init(tableId, eCharacterType.Monster, null);

        if (enemy.grade == Unit.eGrade.Boss)
        {
            EventMgr.Instance.SendEvent(eEventSubject.World, eEventType.EVENT_GAME_ENEMY_BOSS_APPEAR, enemy);
            enemy.actionSystem.CancelCurrentAction();
        }

        if (param.Child > 0)
        {
            Enemy child = CreateEnemy(param.Child);
            child.SetParent(enemy);

            enemy.SetChild(child);
        }

        return enemy;
    }

    public bool IsUsingSkill()
    {
        if (m_Player && m_Player.actionSystem)
        {
            if (m_Player.ExtreamEvading || (m_Player.actionSystem.currentAction && m_Player.actionSystem.IsSkillAction(m_Player.actionSystem.currentAction.actionCommand)))
            {
                return true;
            }
        }

        return false;
    }

    public void SetSkillSlot(int skillSlotIdx)
    {
        if (IsUsingSkill())
        {
            return;
        }

        m_Player.HideAllClone();            //분신 제거
        EffectManager.Instance.StopAll("Effect/Character/prf_fx_noah_dna_ring");   //차지 이펙트 제거

		Shisui shisui = m_Player as Shisui;
		if ( shisui ) {
			shisui.SetAuto( false );
		}

        if (m_PrevAction != null)
        {
            m_Player.actionSystem.CancelCurrentAction();
            m_Player.DeactivateAllPlayerMinion();

			if ( m_Player.Guardian ) {
				m_Player.Guardian.actionSystem.CancelCurrentAction();
			}

            World.Instance.ProjectileMgr.DestroyAllProjectile(false);

            if (m_Player.tableId == 13 && m_PrevAction.Count > 0 && m_PrevAction[0].actionCommand == eActionCommand.TimingHoldAttack)
            {
                ActionBase action = m_Player.actionSystem.GetAction(eActionCommand.ChargingAttack);
                if (action)
                {
                    action.RemoveEffect();
                    m_Player.actionSystem.RemoveAction(action.actionCommand);
                }

                action = m_Player.actionSystem.GetAction(eActionCommand.HoldingDefBtnAttack);
                if (action)
                {
                    action.RemoveEffect();
                    m_Player.actionSystem.RemoveAction(action.actionCommand);
                }

                action = m_Player.actionSystem.GetAction(eActionCommand.RushAttack);
                if (action)
                {
                    action.RemoveEffect();
                    m_Player.actionSystem.RemoveAction(action.actionCommand);
                }

                action = m_Player.actionSystem.GetAction(eActionCommand.Teleport);
                if (action)
                {
                    action.RemoveEffect();
                    m_Player.actionSystem.RemoveAction(action.actionCommand);
                }

                action = m_Player.actionSystem.GetAction(eActionCommand.AttackDuringAttack);
                if (action)
                {
                    action.RemoveEffect();
                    m_Player.actionSystem.RemoveAction(action.actionCommand);
                }
            }

            for (int i = 0; i < m_PrevAction.Count; i++)
            {
                ActionBase action = m_Player.actionSystem.GetAction(m_PrevAction[i].actionCommand);
                if (action == null)
                {
                    continue;
                }

                if (action.actionCommand == eActionCommand.Attack01)
                {
                    if (m_Player.charData.TableID == (int)ePlayerCharType.Astaroth)
                    {
                        ActionTargetingAttack actionTargetingAtk = m_Player.gameObject.AddComponent<ActionTargetingAttack>();
                        if (actionTargetingAtk)
                        {
                            CopyTargetingAttack(ref actionTargetingAtk);
                            m_Player.actionSystem.AddAction(actionTargetingAtk, 0, null);
                        }
                    }
                    else if (m_Player.charData.TableID != (int)ePlayerCharType.Emily)
                    {
                        ActionComboAttack comboAttack = m_Player.gameObject.AddComponent<ActionComboAttack>();
                        if (comboAttack != null)
                        {
                            CopyComboAttack(ref comboAttack);
                            m_Player.actionSystem.AddAction(comboAttack, 0, null);
                        }
                        
                    }
                    //else
                    //{
                    //    Emily emily = m_Player as Emily;
                    //    emily.CheckEmilyAttack();
                    //}
                }

                action.RemoveEffect();
                m_Player.actionSystem.RemoveAction(action.actionCommand);

                if ( m_Player.Guardian ) {
					m_Player.Guardian.actionSystem.RemoveAction( action.actionCommand );
				}
            }
        }

        if (m_PrevAction == null)
            m_PrevAction = new List<ActionBase>();
        m_PrevAction.Clear();

        kSkillSlotIdx = skillSlotIdx;
        if (m_uiSkillTrainingPanel != null)
            m_uiSkillTrainingPanel.SetSkill(kSkillSlotIdx);
        m_MainCharData = m_Player.charData;



        GameTable.CharacterSkillPassive.Param skillInfo = GameInfo.Instance.GameTable.FindCharacterSkillPassive(x => x.CharacterID == m_MainCharData.TableID && x.Slot == kSkillSlotIdx && x.ParentsID == -1 && (x.Type == (int)eCHARSKILLPASSIVETYPE.SELECT_NORMAL));
        m_MainCharData.PassvieList.Clear();
        m_MainCharData.PassvieList.Add(new PassiveData((m_MainCharData.TableID * 1000) + 301, 2));
        m_MainCharData.PassvieList.Add(new PassiveData(skillInfo.ID, 2));

        // 잉그리드 각성 스킬은 다른 스킬이 필요함
        if (skillInfo.ID == 13280)
        {
            m_MainCharData.PassvieList.Add(new PassiveData(13220, 2));
            m_MainCharData.PassvieList.Add(new PassiveData(13230, 2));
            m_MainCharData.PassvieList.Add(new PassiveData(13240, 2));
            m_MainCharData.PassvieList.Add(new PassiveData(13270, 2));
            m_MainCharData.PassvieList.Add(new PassiveData(13290, 2));
        }

        m_Player.SetMainTarget(m_listEnemy[0]);
        Log.Show(m_Player.charData.PassvieList[0].SkillID, Log.ColorType.Red);

        //m_Player.SetAction();
        m_Player.SetAction(true);
        m_Player.ExtreamEvading = false;

        for (int i = 0; i < m_Player.charData.PassvieList.Count; i++)
            Debug.Log(m_Player.charData.PassvieList[i].SkillID);
        //m_Player.OnGameStart();

        //bool isExtreamEvade = false;

        string[] split = null;
        System.Type type = null;
		split = Utility.Split(skillInfo.SkillAction, ','); //skillInfo.SkillAction.Split(',');
        for (int i = 0; i < split.Length; i++)
        {
            type = System.Type.GetType("Action" + split[i]);
            ActionBase action = m_Player.GetComponent(type) as ActionBase;
            if (action != null)// && action.actionCommand != eActionCommand.Attack01)
            {
                m_PrevAction.Add(action);
            }
        }

        UIGamePlayPanel uIGamePlayPanel = (UIGamePlayPanel)GameUIManager.Instance.GetUI("GamePlayPanel");
        if (uIGamePlayPanel != null)
        {
            uIGamePlayPanel.SetPlayer(m_Player, null);
        }

        for (int i = 0; i < m_PrevAction.Count; i++)
        {
            ActionCounterAttack actionCounterAtk = m_PrevAction[i] as ActionCounterAttack;

            if (actionCounterAtk != null || m_PrevAction[i].actionCommand == eActionCommand.ExtreamEvade ||
                m_PrevAction[i].actionCommand == eActionCommand.CounterAttack || m_PrevAction[i].actionCommand == eActionCommand.EmergencyAttack)
            {
                if (m_listEnemy[0].gameObject.activeSelf)
                    m_listEnemy[0].Deactivate();

                if (!m_listEnemy[1].IsActivate())
                {
                    m_listEnemy[1].Activate();
                    m_Player.SetMainTarget(m_listEnemy[1]);
                }

                AfterSetSkillSlot();
                return;
            }
        }

        if (m_listEnemy[1].gameObject.activeSelf)
            m_listEnemy[1].Deactivate();

        if (!m_listEnemy[0].IsActivate())
        {
            m_listEnemy[0].Activate();
            m_Player.SetMainTarget(m_listEnemy[0]);
        }

        AfterSetSkillSlot();
    }

    private void AfterSetSkillSlot()
    {
        if (m_Player.charData.TableID == (int)ePlayerCharType.Emily)
        {
            Emily emily = m_Player as Emily;
            emily.CheckEmilyAttack();
        }
        else
        {

        }

        m_Player.OnAfterChangeWeapon();
        GameUIManager.Instance.GetUI<UIGamePlayPanel>("GamePlayPanel").SetTrainingroomUI();
    }

    private void DefaultAttackAction_Copy(ActionBase baseAction)
    {
        if (m_DefaultAttackActionCopy != null || mDefaultTargetingAtkActionCopy != null)
        {
            return;
        }

        ActionComboAttack comboAttack = baseAction as ActionComboAttack;
        if (comboAttack == null)
        {
            ActionTargetingAttack actionTargetingAtk = baseAction as ActionTargetingAttack;
            if (actionTargetingAtk == null)
            {
                Log.Show("ComboAttack is NULL", Log.ColorType.Red);
                return;
            }

            mDefaultTargetingAtkActionCopy = gameObject.AddComponent<ActionTargetingAttack>();

            mDefaultTargetingAtkActionCopy.conditionActionCommand = actionTargetingAtk.conditionActionCommand;
            mDefaultTargetingAtkActionCopy.extraCondition = actionTargetingAtk.extraCondition;

            mDefaultTargetingAtkActionCopy.cancelActionCommand = actionTargetingAtk.cancelActionCommand;
            mDefaultTargetingAtkActionCopy.extraCancelCondition = actionTargetingAtk.extraCancelCondition;
            mDefaultTargetingAtkActionCopy.cancelDuringSameActionCount = actionTargetingAtk.cancelDuringSameActionCount;
            mDefaultTargetingAtkActionCopy.cancelDuringSameActionByCutFrame = actionTargetingAtk.cancelDuringSameActionByCutFrame;

            mDefaultTargetingAtkActionCopy.superArmor = actionTargetingAtk.superArmor;

            mDefaultTargetingAtkActionCopy.AttackAnimations = actionTargetingAtk.AttackAnimations;
        }
        else
        {
            m_DefaultAttackActionCopy = gameObject.AddComponent<ActionComboAttack>();

            m_DefaultAttackActionCopy.conditionActionCommand = comboAttack.conditionActionCommand;
            m_DefaultAttackActionCopy.extraCondition = comboAttack.extraCondition;

            m_DefaultAttackActionCopy.cancelActionCommand = comboAttack.cancelActionCommand;
            m_DefaultAttackActionCopy.extraCancelCondition = comboAttack.extraCancelCondition;
            m_DefaultAttackActionCopy.cancelDuringSameActionCount = comboAttack.cancelDuringSameActionCount;
            m_DefaultAttackActionCopy.cancelDuringSameActionByCutFrame = comboAttack.cancelDuringSameActionByCutFrame;

            m_DefaultAttackActionCopy.superArmor = comboAttack.superArmor;

            m_DefaultAttackActionCopy.attackAnimations = comboAttack.attackAnimations;
        }
    }

    private void CopyComboAttack(ref ActionComboAttack comboAttack)
    {
        comboAttack.conditionActionCommand = m_DefaultAttackActionCopy.conditionActionCommand;
        comboAttack.extraCondition = m_DefaultAttackActionCopy.extraCondition;

        comboAttack.cancelActionCommand = m_DefaultAttackActionCopy.cancelActionCommand;
        comboAttack.extraCancelCondition = m_DefaultAttackActionCopy.extraCancelCondition;
        comboAttack.cancelDuringSameActionCount = m_DefaultAttackActionCopy.cancelDuringSameActionCount;
        comboAttack.cancelDuringSameActionByCutFrame = m_DefaultAttackActionCopy.cancelDuringSameActionByCutFrame;

        comboAttack.superArmor = m_DefaultAttackActionCopy.superArmor;

        comboAttack.attackAnimations = m_DefaultAttackActionCopy.attackAnimations;
    }

    private void CopyTargetingAttack(ref ActionTargetingAttack targetingAttack)
    {
        targetingAttack.conditionActionCommand = mDefaultTargetingAtkActionCopy.conditionActionCommand;
        targetingAttack.extraCondition = mDefaultTargetingAtkActionCopy.extraCondition;

        targetingAttack.cancelActionCommand = mDefaultTargetingAtkActionCopy.cancelActionCommand;
        targetingAttack.extraCancelCondition = mDefaultTargetingAtkActionCopy.extraCancelCondition;
        targetingAttack.cancelDuringSameActionCount = mDefaultTargetingAtkActionCopy.cancelDuringSameActionCount;
        targetingAttack.cancelDuringSameActionByCutFrame = mDefaultTargetingAtkActionCopy.cancelDuringSameActionByCutFrame;

        targetingAttack.superArmor = mDefaultTargetingAtkActionCopy.superArmor;

        targetingAttack.AttackAnimations = mDefaultTargetingAtkActionCopy.AttackAnimations;
    }
}
