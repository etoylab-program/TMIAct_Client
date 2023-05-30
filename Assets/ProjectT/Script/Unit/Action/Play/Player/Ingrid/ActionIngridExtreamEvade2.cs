
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionIngridExtreamEvade2 : ActionExtreamEvade
{
    private List<Projectile>            mListPjtBlade   = new List<Projectile>();
    private AniEvent.sProjectileInfo    mPjtBladeInfo   = null;
    private AniEvent.sEvent             mAniEvt         = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        for (int i = 0; i < mValue1; i++)
        {
            Projectile pjt = GameSupport.CreateProjectile("Projectile/pjt_character_ingrid_evade_2.prefab");
            mListPjtBlade.Add(pjt);
        }

        mPjtBladeInfo = mOwnerPlayer.aniEvent.CreateProjectileInfo(mListPjtBlade[0]);
        mPjtBladeInfo.attach = true;
        mPjtBladeInfo.boneName = "Bip001 Head";

        mAniEvt = mOwnerPlayer.aniEvent.CreateEvent(eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f);
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
        StartCoroutine("EndEvadingBuff");
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
        m_owner.PlayAniImmediate(mParam.AniEvade);

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
                Vector3 dir = mParam.ExtraDashDir != Vector3.zero ? mParam.ExtraDashDir : actionDash.Dir;
                float dashSpeedRatio = actionDash.CurDashSpeedRatio > 0.0f ? actionDash.CurDashSpeedRatio : mParam.ExtraDashSpeedRatio;

                m_owner.cmptMovement.UpdatePosition(dir, m_owner.originalSpeed * dashSpeedRatio, true);
            }
            else
            {
                m_owner.ShowMesh(true);
                end = true;
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
        m_owner.TemporaryInvincible = false;

        mOwnerPlayer.ContinuingDash = false;
    }

    private IEnumerator EndEvadingBuff()
    {
        yield return StartCoroutine(ContinueDash());

        Unit target = World.Instance.EnemyMgr.GetNearestTarget(mOwnerPlayer, true);
        if (target)
        {
            for (int i = 0; i < mValue1; i++)
            {
                mListPjtBlade[i].Fire(mOwnerPlayer, BattleOption.eToExecuteType.Unit, mAniEvt, mPjtBladeInfo, target, TableId, null, null);
            }
        }

        mOwnerPlayer.ExtreamEvading = false;
        IsSkillEnd = true;
    }
}

