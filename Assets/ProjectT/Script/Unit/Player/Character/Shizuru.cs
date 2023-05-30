
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shizuru : Player
{
    public override void Init(int tableId, eCharacterType type, string faceAniControllerPath)
    {
        base.Init(tableId, type, faceAniControllerPath);

        Transform tf = m_aniEvent.transform.Find("fbx_resevine_win");
        if(tf)
        {
            Utility.SetLayer(tf.gameObject, gameObject.layer, true);
        }
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
                    ActionShizuruDash2Attack actionRuash2Atk = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionShizuruDash2Attack;
                    if (actionRuash2Atk)
                    {
                        actionRuash2Atk.OnUpdating(null);
                    }
                    else if (currentAction != null && currentAction.actionCommand == eActionCommand.Attack01)
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

        if (CheckTimingHoldAttack())
        {
            return;
        }

        ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
        ActionSelectSkillBase action = m_actionSystem.GetAction<ActionSelectSkillBase>(eActionCommand.Teleport);
        if (action && action.PossibleToUse && actionDash && actionDash.IsPossibleToDashAttack() && GetMainTargetCollider(true))
        {
            float checkTime = actionDash.GetEvadeCutFrameLength();
            if (World.Instance.UIPlay.btnDash.deltaTime < checkTime)
            {
                CommandAction(eActionCommand.Teleport, null);
                return;
            }
        }

        CommandAction( eActionCommand.Defence, param );
    }

    public override float GetAirAttackJumpPower()
    {
        if (m_cmptJump == null)
        {
            return 0.0f;
        }

        return m_cmptJump.m_jumpPower;
    }

	protected override void FixedUpdate() {
		base.FixedUpdate();

		if( IsHelper || !m_actionSystem.HasAction( eActionCommand.TimingHoldAttack ) ) {
			return;
		}

		AniEvent.sAniInfo aniInfo = m_aniEvent.GetAniInfo( m_aniEvent.curAniType );
		if( aniInfo.timingHoldFrame <= 0.0f ) {
			if( World.Instance.UIPlay.EffDashFlash.gameObject.activeSelf ) {
				World.Instance.UIPlay.EffDashFlash.gameObject.SetActive( false );
			}

			return;
		}

		if( m_aniEvent.GetCurrentFrame() >= aniInfo.timingHoldFrame ) {
			EffectManager.Instance.Play( this, 1020, EffectManager.eType.Common );

			if( !World.Instance.UIPlay.EffDashFlash.gameObject.activeSelf ) {
				World.Instance.UIPlay.EffDashFlash.gameObject.SetActive( true );
			}
		}
		else {
			if( World.Instance.UIPlay.EffDashFlash.gameObject.activeSelf ) {
				World.Instance.UIPlay.EffDashFlash.gameObject.SetActive( false );
			}
		}
	}
}
