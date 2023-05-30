
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.FinalIK;


public class Yukikaze : Player
{
    public override void Init(int tableId, eCharacterType type, string faceAniControllerPath)
    {
        base.Init(tableId, type, faceAniControllerPath);

        /*ActionYukikazeChargeAttack actionChargeAtk = m_actionSystem.GetAction<ActionYukikazeChargeAttack>(eActionCommand.ChargingAttack);
        if (actionChargeAtk)
        {
            World.Instance.uiPlay.btnAtk.m_maxChargeCount = 1;

            if (actionChargeAtk.passiveDatas[0].level >= eActionLevel.Lv1)
            {
                GameTable.CharacterSkillPassive.Param param = GameInfo.Instance.GameTable.FindCharacterSkillPassive(actionChargeAtk.passiveDatas[0].id);
                World.Instance.uiPlay.btnAtk.m_maxChargeCount = (int)param.Value1;
            }
        }*/

        m_bipedIK = gameObject.AddComponent<BipedIK>();
        BipedReferences.AutoDetectReferences(ref m_bipedIK.references, transform, new BipedReferences.AutoDetectParams(false, true));
        m_bipedIK.InitiateBipedIK();
        m_bipedIK.SetToDefaults();

        InitIK();
        //ShowIKLook(false, JointController.eJointCtrlType.Translate);

        for (int i = 0; i < m_listIKObject.Count; i++)
        {
            if(i != (int)eBipedIK.LeftHand && i != (int)eBipedIK.RightHand)
                m_listIKObject[i].biped = null;
        }

        m_listIKObject[(int)eBipedIK.LeftHand].SetBiped(eBipedIK.LeftHand, m_bipedIK.references.leftHand, true, 0);
        m_bipedIK.solvers.leftHand.target = m_listIKObject[(int)eBipedIK.LeftHand].targetMovement.transform;
        m_bipedIK.solvers.leftHand.IKPositionWeight = 1.0f;
        m_bipedIK.solvers.leftHand.IKRotationWeight = 0.0f;

        m_listIKObject[(int)eBipedIK.RightHand].SetBiped(eBipedIK.RightHand, m_bipedIK.references.rightHand, true, 0);
        m_bipedIK.solvers.rightHand.target = m_listIKObject[(int)eBipedIK.RightHand].targetMovement.transform;
        m_bipedIK.solvers.rightHand.IKPositionWeight = 1.0f;
        m_bipedIK.solvers.rightHand.IKRotationWeight = 0.0f;

        m_listIKObject[(int)eBipedIK.Body].SetBiped(eBipedIK.Body, m_bipedIK.references.spine[0], true, 0);
        m_bipedIK.solvers.spine.target = m_listIKObject[(int)eBipedIK.Body].targetMovement.transform;
        m_bipedIK.solvers.spine.IKPositionWeight = 1.0f;

        EnableIK(false);
    }

    protected override void OnEventAtk()
    {
        if (!m_actionSystem.IsCurrentUSkillAction())
        {
            bool isJumpAttack = false;

            ActionBase currentAction = m_actionSystem.currentAction;
            if (currentAction != null && (currentAction.actionCommand == eActionCommand.Jump || currentAction.actionCommand == eActionCommand.UpperJump ||
                                          currentAction.actionCommand == eActionCommand.JumpAttack) && isGrounded == false && m_cmptJump.highest >= 0.8f)
            {
                if (World.Instance.EnemyMgr.HasFloatingEnemy())
                {
                    if (currentAction != null && currentAction.actionCommand == eActionCommand.JumpAttack)
                    {
                        isJumpAttack = true;
                        currentAction.OnUpdating(null);
                    }
                    else
                    {
                        isJumpAttack = true;
                        CommandAction(eActionCommand.JumpAttack, null);
                    }
                }
            }

            if(!isJumpAttack)
            {
                ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
                if (actionDash && actionDash.IsPossibleToDashAttack())
                {
                    CommandAction(eActionCommand.RushAttack, null);
                }
                else
                {
                    if (m_actionSystem.IsCurrentAction(eActionCommand.Attack01) || m_actionSystem.IsCurrentAction(eActionCommand.GroundSkillCombo) ||
                        m_actionSystem.IsCurrentAction(eActionCommand.AirShotAttack) || m_actionSystem.IsCurrentAction(eActionCommand.HoldingDefBtnAttack))
                    {
                        currentAction.OnUpdating(null);
                    }
                    else
                    {
                        CommandAction(eActionCommand.Attack01, null);
                    }
                }
            }
        }
    }

    protected override void OnEventDefence( IActionBaseParam param = null )
    {
        if (m_actionSystem.IsCurrentUSkillAction() == true)
        {
            return;
        }

        CommandAction( eActionCommand.Defence, param );
    }

	protected override void OnEventSpecialAtk() {
		if( m_actionSystem.IsCurrentSkillAction() || m_actionSystem.IsCurrentUSkillAction() ) {
			return;
		}

		ActionYukikazeComboAttack actionCombo = m_actionSystem.GetAction<ActionYukikazeComboAttack>(eActionCommand.Attack01);
		ActionYukikazeGroundSkillCombo actionGroundSkillCombo = m_actionSystem.GetAction<ActionYukikazeGroundSkillCombo>(eActionCommand.GroundSkillCombo);

        // 지상 공격 후 원거리 콤보 공격
        if( actionCombo && actionGroundSkillCombo && m_actionSystem.IsCurrentAction( eActionCommand.Attack01 ) == true ) {
			if( actionCombo && actionCombo.IsAfterLastAttackCutFrame() ) {
				CommandAction( eActionCommand.GroundSkillCombo, null );
				return;
			}
		}

		// 띄우기 및 내려찍기
		ActionUpperAttack actionUpperAtk = m_actionSystem.GetAction<ActionUpperAttack>(eActionCommand.AttackDuringAttack);
        if( actionUpperAtk == null ) {
            return;
        }

		if( m_actionSystem.IsCurrentAction( eActionCommand.Attack01 ) ) {
			if( !IsHelper ) {
				World.Instance.UIPlay.btnAtk.lockCharge = true;
			}

			CommandAction( eActionCommand.AttackDuringAttack, null );
		}
	}
}
