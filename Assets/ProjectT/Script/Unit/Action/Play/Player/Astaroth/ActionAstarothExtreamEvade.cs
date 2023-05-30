
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAstarothExtreamEvade : ActionExtreamEvade
{
    private float                       mDuration   = 0.0f;
    private readonly int                mPjtCount   = 3;
    private Projectile[]                mArrPjt     = null;
    private AniEvent.sProjectileInfo[]  mArrPjtInfo = null;
    private AniEvent.sEvent             mAniEvt     = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        mDuration = GameInfo.Instance.BattleConfig.BuffDuration + mValue1;

        mArrPjt = new Projectile[mPjtCount];
        for (int i = 0; i < mArrPjt.Length; i++)
        {
            mArrPjt[i] = GameSupport.CreateProjectile("Projectile/pjt_character_astaroth_evade.prefab");
            mArrPjt[i].duration = mDuration + 4.0f;
            mArrPjt[i].transform.localEulerAngles = Vector3.up * (i * 120.0f);
        }

        mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 0.4f);
        mAniEvt.atkRatio = mValue2;

        mArrPjtInfo = new AniEvent.sProjectileInfo[mPjtCount];
        for (int i = 0; i < mArrPjtInfo.Length; i++)
        {
            mArrPjtInfo[i] = m_owner.aniEvent.CreateProjectileInfo(mArrPjt[i]);
            mArrPjtInfo[i].attach = true;
            mArrPjtInfo[i].boneName = "Bip001 Pelvis";
            mArrPjtInfo[i].followParentRot = true;
        }
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
        float dashAniLength = m_owner.PlayAniImmediate(actionDash.CurAni);

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
        for (int i = 0; i < mArrPjt.Length; i++)
        {
            mArrPjt[i].Fire(m_owner, BattleOption.eToExecuteType.Unit, mAniEvt, mArrPjtInfo[i], m_owner.evadedTarget, TableId);
        }

        yield return StartCoroutine(ContinueDash());

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
    }
}
