
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterAstarotSummon : ActionSupporterSkillBase
{
    private List<PlayerMinion>  mListMinion = new List<PlayerMinion>();
    private float               mDuration   = 20.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterAstarotSummon;
    }

    public override void LoadAfterEnemyMgrInit()
    {
        int minionCount = 1;
        if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
        {
            if (World.Instance.EnemyMgr.MaxSpawnMonsterCount > 0)
            {
                minionCount = Mathf.Min(World.Instance.EnemyMgr.MaxSpawnMonsterCount, World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup);
            }
            else
            {
                minionCount = World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup;
            }
        }

        for (int i = 0; i < minionCount; i++)
        {
            PlayerMinion minion = GameSupport.CreatePlayerMinion(16, mOwnerPlayer);
            mListMinion.Add(minion);
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParamFromBO = param as ActionParamFromBO;

        List<Unit> list = mOwnerPlayer.GetEnemyListByAbnormalAttr(EAttackAttr.FIRE);
        StartSummon(list.Count);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate)
        {
            m_checkTime += Time.fixedDeltaTime;
            if(m_checkTime >= mDuration)
            {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }

        if (!World.Instance.IsEndGame && !World.Instance.ProcessingEnd)
        {
            float aniLength = 0.0f;
            for (int i = 0; i < mListMinion.Count; i++)
            {
                PlayerMinion minion = mListMinion[i];
                if(!minion.IsActivate())
                {
                    continue;
                }

                minion.StopBT();

                aniLength = minion.PlayAniImmediate(eAnimation.Die);
                minion.StartDissolve(aniLength, false, new Color(0.169f, 0.0f, 0.47f));
            }

            yield return new WaitForSeconds(aniLength);
        }

        AllMinionDeactivate();
    }

    public override void OnCancel()
    {
        base.OnCancel();
        AllMinionDeactivate();
    }

    private void StartSummon(int minionCount)
    {
        AllMinionDeactivate();

        Vector3 centerPos = World.Instance.EnemyMgr.GetCenterPosOfEnemies(mOwnerPlayer);

        for (int i = 0; i < minionCount; i++)
        {
            if(i >= World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup)
            {
                break;
            }

            PlayerMinion minion = mListMinion[i];
            Utility.SetLayer(minion.gameObject, (int)eLayer.PlayerClone, true);

            Vector3 pos = centerPos;
            pos.y = m_owner.posOnGround.y;

            int randX = UnityEngine.Random.Range(-30, 31);
            int randZ = UnityEngine.Random.Range(-30, 31);

            pos.x += (float)randX / 10.0f;
            pos.z += (float)randZ / 10.0f;

            minion.SetInitialPosition(pos, mOwnerPlayer.transform.rotation);
            minion.SetMinionAttackPower(mOwnerPlayer.gameObject.layer, mOwnerPlayer.attackPower * mParamFromBO.battleOptionData.value);
            minion.Activate();
            minion.StartBT();

            minion.PlayAniImmediate(eAnimation.Appear);
            minion.StartDissolve(0.5f, true, new Color(0.169f, 0.0f, 0.47f));
        }
    }

    private void AllMinionDeactivate()
    {
        for (int i = 0; i < mListMinion.Count; i++)
        {
            mListMinion[i].StopBT();
            mListMinion[i].Deactivate();
        }
    }
}
