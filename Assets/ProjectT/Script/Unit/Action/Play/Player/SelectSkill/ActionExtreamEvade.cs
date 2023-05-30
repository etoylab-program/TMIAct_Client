
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionExtreamEvade : ActionSelectSkillBase
{
    protected ActionParamExtreamEvade mParam = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.ExtreamEvade;

        ExplicitStartCoolTime = true;
        mOwnerPlayer = m_owner as Player;

        conditionActionCommand = new eActionCommand[1];
        conditionActionCommand[0] = eActionCommand.Defence;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParam = param as ActionParamExtreamEvade;

        PossibleToUse = false;

        ShowSkillNames(m_data);
        StartCoolTime();
    }

    public void ForceEnd()
    {
        IsSkillEnd = true;
        mOwnerPlayer.ExtreamEvading = false;
    }

    protected virtual IEnumerator ContinueDash()
    {
        mOwnerPlayer.ContinuingDash = true;
        ActionDash actionDash = mOwnerPlayer.actionSystem.GetAction<ActionDash>(eActionCommand.Defence);

        m_checkTime = 0.0f;
        bool end = false;
        float dashAniLength = m_owner.PlayAniImmediate(mParam.AniEvade);

        if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
        {
            m_owner.Input.LockBtnFlag(InputController.ELockBtnFlag.ATTACK | InputController.ELockBtnFlag.DASH | InputController.ELockBtnFlag.USKILL |
                                      InputController.ELockBtnFlag.WEAPON | InputController.ELockBtnFlag.SUPPORTER);
        }

        m_owner.Input.LockDirection(true);
        //m_owner.SetSuperArmor(Unit.eSuperArmor.Invincible, GetType());
        m_owner.TemporaryInvincible = true;

        if (m_owner.isGrounded == true)
        {
            if (actionDash.ShowMeshOnFrontDash && !actionDash.IsNoDir)
            {
                m_owner.ShowMesh(false);
            }
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!end)
        {
            m_checkTime += Time.fixedDeltaTime;

            if (m_checkTime < actionDash.DashTime)
            {
                Vector3 dir = mParam.ExtraDashDir != Vector3.zero ? mParam.ExtraDashDir : actionDash.Dir;
                float dashSpeedRatio = actionDash.CurDashSpeedRatio > 0.0f ? actionDash.CurDashSpeedRatio : mParam.ExtraDashSpeedRatio;

                m_owner.cmptMovement.UpdatePosition(dir, m_owner.originalSpeed * dashSpeedRatio, true);
            }
            else if (m_checkTime >= dashAniLength)
            {
                end = true;
            }
            else if (m_checkTime >= actionDash.DashTime)
            {
                m_owner.ShowMesh(true);
            }

            yield return mWaitForFixedUpdate;
        }

        if (!m_owner.isGrounded)
        {
            m_owner.SetFallingRigidBody();
            while (!m_owner.isGrounded)
            {
                yield return null;
            }
        }

        if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
        {
            m_owner.Input.LockBtnFlag(InputController.ELockBtnFlag.NONE);
        }

        m_owner.Input.LockDirection(false);
        //m_owner.RestoreSuperArmor(Unit.eSuperArmor.Invincible, GetType());
        m_owner.TemporaryInvincible = false;

        mOwnerPlayer.ContinuingDash = false;
    }
}
