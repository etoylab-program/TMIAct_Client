
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionNoahExtreamEvade : ActionExtreamEvade
{
    private float mDuration = 0.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        mDuration = GameInfo.Instance.BattleConfig.BuffDuration + GameInfo.Instance.BattleConfig.BuffDuration;
    }

    public override void OnStart(IActionBaseParam param)
    {
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
        float dashAniLength = m_owner.PlayAniImmediate(actionDash.CurAni, 0.0f, true);

        if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
        {
            m_owner.Input.LockBtnFlag(InputController.ELockBtnFlag.ATTACK | InputController.ELockBtnFlag.DASH | InputController.ELockBtnFlag.USKILL |
                                      InputController.ELockBtnFlag.WEAPON | InputController.ELockBtnFlag.SUPPORTER);
        }

        m_owner.Input.LockDirection(true);
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

        mOwnerPlayer.TemporaryInvincible = false;
        mOwnerPlayer.ContinuingDash = false;
    }

    private IEnumerator EndEvadingBuff(float duration)
    {
        StartAttack();
        yield return StartCoroutine(ContinueDash());

        m_checkTime = 0.0f;

        for(int i = 0; i < m_owner.listHitTarget.Count; i++)
        {
            Unit target = m_owner.listHitTarget[i];
            if(!target.actionSystem.currentAction || target.actionSystem.currentAction.actionCommand != eActionCommand.Hit)
            {
                continue;
            }

            ActionHit actionHit = target.actionSystem.GetCurrentAction<ActionHit>();
            if(actionHit == null || actionHit.State != ActionHit.eState.Float)
            {
                continue;
            }

            HitFloat hitFloat = actionHit.GetCurrentFloatHitOrNull();
            if(hitFloat == null)
            {
                continue;
            }

            hitFloat.SetDownIdleTime(5.0f);
        }

        /*
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
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

            yield return waitForFixedUpdate;
        }
        */

        mOwnerPlayer.ExtreamEvading = false;
        IsSkillEnd = true;

        BOCharSkill.EndBattleOption(BattleOption.eBOTimingType.DuringSkill, TableId);
    }

    private void StartAttack()
    {
        int effId = 30021;
        if (mValue2 > 3.0f)
        {
            effId = 30022;
        }

        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
        mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
        mBuffEvt.Set(m_data.ID, eEventSubject.ActiveEnemiesInRange, eEventType.EVENT_ACTION_HIT_UPPER_ATTACK, m_owner,
                     mValue1, mValue2, 0, 0, 0.0f, effId, mBuffEvt.effId2, eBuffIconType.None);

        EventMgr.Instance.SendEvent(mBuffEvt);
    }
}
