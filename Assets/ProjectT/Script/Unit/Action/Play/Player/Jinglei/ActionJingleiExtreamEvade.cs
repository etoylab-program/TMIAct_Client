
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionJingleiExtreamEvade : ActionExtreamEvade
{
    private float mDuration         = 0.0f;
    private float mAddDurationRatio = 0.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        mAddDurationRatio = mValue1 / (float)eCOUNT.MAX_BO_FUNC_VALUE;
        mDuration = GameInfo.Instance.BattleConfig.BuffDuration + (GameInfo.Instance.BattleConfig.BuffDuration * mAddDurationRatio);

		if ( mValue2 > 0.0f ) {
			BOCharSkill.AddBattleOption( (int)mValue2, tableId );
		}

		if ( mValue3 > 0.0f ) {
			BOCharSkill.ChangeBattleOptionValue1( (int)mValue2, tableId, mValue3 / (float)eCOUNT.MAX_BO_FUNC_VALUE );
		}
    }

    public override void OnStart(IActionBaseParam param)
    {
        //BOCharSkill.ChangeBattleOptionDuration(BattleOption.eBOTimingType.DuringSkill, TableId, mDuration);
        if (mAddDurationRatio > 0.0f)
        {
            BOCharSkill.IncreaseBattleOptionDuration(BattleOption.eBOTimingType.DuringSkill, TableId, mAddDurationRatio);
        }

		BattleOption.sBattleOptionData boData = BOCharSkill.ListBattleOptionData.Find( x => x.timingType == BattleOption.eBOTimingType.DuringSkill && x.actionTableId == TableId );
		if ( boData != null && boData.AddDuration > 0.0f ) {
			mDuration = boData.originalDuration + ( boData.originalDuration * mAddDurationRatio ) + boData.AddDuration;
		}

        base.OnStart(param);
        StartEvadingBuff();
    }

    private void StartEvadingBuff()
    {
        mOwnerPlayer.ExtreamEvading = true;

        StopCoroutine("EndEvadingBuff");
        StartCoroutine("EndEvadingBuff", mDuration);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        IsSkillEnd = false;
    }

    protected override IEnumerator ContinueDash()
    {
        mOwnerPlayer.ContinuingDash = true;
        ActionDash actionDash = mOwnerPlayer.actionSystem.GetAction<ActionDash>(eActionCommand.Defence);

        m_checkTime = 0.0f;
        bool end = false;
        float dashAniLength = m_owner.PlayAniImmediate(actionDash.CurAni);

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

        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!end)
        {
            m_checkTime += Time.fixedDeltaTime;

            if (m_checkTime < actionDash.DashTime)
            {
                m_owner.cmptMovement.UpdatePosition(actionDash.Dir, m_owner.originalSpeed * actionDash.CurDashSpeedRatio, true);
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

        m_owner.Input.LockDirection(false);

        if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
        {
            m_owner.Input.LockBtnFlag(InputController.ELockBtnFlag.NONE);
        }

        //m_owner.RestoreSuperArmor(Unit.eSuperArmor.Invincible, GetType());
        mOwnerPlayer.TemporaryInvincible = false;
        mOwnerPlayer.ContinuingDash = false;
        //m_owner.SetSuperArmor(superArmor, GetType());
    }

    private IEnumerator EndEvadingBuff(float duration)
    {
        yield return StartCoroutine(ContinueDash());
        //m_owner.RestoreSuperArmor(superArmor, GetType());

        m_checkTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            if (!World.Instance.IsPause)
            {
                m_checkTime += Time.fixedDeltaTime;
            }

            if (m_checkTime >= duration)
            {
                mOwnerPlayer.ExtreamEvading = false;
                IsSkillEnd = true;

                break;
            }

            yield return mWaitForFixedUpdate;
        }

        BOCharSkill.EndBattleOption(BattleOption.eBOTimingType.DuringSkill, TableId);
    }
}
