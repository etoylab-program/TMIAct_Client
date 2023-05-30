
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionNoahEvadeDoubleSummon : ActionTeleport
{
    private List<PlayerMinion>  mListMinion     = new List<PlayerMinion>();
    private int                 mMinionCount    = 1;
    private float               mDuration       = 20.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        cancelActionCommand = new eActionCommand[2];
        cancelActionCommand[0] = eActionCommand.Defence;
        cancelActionCommand[1] = eActionCommand.TimingHoldAttack;

        mMinionCount = (int)mValue3;

        for (int i = 0; i < mMinionCount; i++)
        {
            int id = 14;

            if (m_data.IncValue1 > 0.0f)
            {
                /*
                int boSetId = ((int)m_data.IncValue2) + 30000000;

                ActionAttack actionAttack = minion.actionSystem.GetAction<ActionAttack>(eActionCommand.Attack02);
                if (actionAttack)
                {
                    actionAttack.AddBOSet(boSetId);
                }

                actionAttack = minion.actionSystem.GetAction<ActionAttack>(eActionCommand.Attack03);
                if (actionAttack)
                {
                    actionAttack.AddBOSet(boSetId);
                }
                */

                id = 15;
            }

            PlayerMinion minion = GameSupport.CreatePlayerMinion(id, mOwnerPlayer);
            mListMinion.Add(minion);
        }

        mDuration = mValue1;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.EvadeDouble);
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.EvadeDouble);
    }

    public override IEnumerator UpdateAction()
    {
        bool summon = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            m_checkTime += Time.fixedDeltaTime;
            if(!summon && m_checkTime >= m_aniCutFrameLength)
            {
                StartSummon();
                summon = true;
            }
            else if (m_checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override bool PossibleToUseInAI()
    {
        if (PossibleToUse && MaxCoolTime > 0.0f && IsAllMinionDeactivate())
        {
            return true;
        }

        return false;
    }

    private void StartSummon()
    {
        AllMinionDeactivate();

        for (int i = 0; i < mMinionCount; i++)
        {
            PlayerMinion minion = mListMinion[i];
            Utility.SetLayer(minion.gameObject, (int)eLayer.PlayerClone, true);

            Vector3 pos = mOwnerPlayer.transform.position;

            int randX = UnityEngine.Random.Range(-5, 6);
            int randY = UnityEngine.Random.Range(0, 4);
            int randZ = UnityEngine.Random.Range(-5, 6);

            pos.x += (float)randX / 10.0f;
            pos.y += (float)randY / 10.0f;
            pos.z += (float)randZ / 10.0f;

            minion.SetInitialPosition(pos, mOwnerPlayer.transform.rotation);
            minion.SetMinionAttackPower(mOwnerPlayer.gameObject.layer, mOwnerPlayer.attackPower * mValue2);
            minion.Activate();
            minion.SetFloatingRigidBody();
            minion.StartBT();

            minion.PlayAniImmediate(eAnimation.Appear);
            minion.StartDissolve(0.5f, true, new Color(0.169f, 0.0f, 0.47f));
        }

        StopCoroutine("UpdateMinion");
        StartCoroutine("UpdateMinion");
    }

    private void AllMinionDeactivate()
    {
        for (int i = 0; i < mMinionCount; i++)
        {
            mListMinion[i].StopBT();
            mListMinion[i].Deactivate();
        }
    }

    private bool IsAllMinionDeactivate()
    {
        for (int i = 0; i < mMinionCount; i++)
        {
            if(mListMinion[i].IsActivate())
            {
                return false;
            }
        }

        return true;
    }

    private IEnumerator UpdateMinion()
    {
        float checkTime = 0.0f;
        bool endUpdate = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!endUpdate)
        {
            checkTime += Time.fixedDeltaTime;
            if (checkTime >= mDuration)
            {
                endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }

        if (!World.Instance.IsEndGame && !World.Instance.ProcessingEnd)
        {
            float aniLength = 0.0f;
            for (int i = 0; i < mListMinion.Count; i++)
            {
                PlayerMinion minion = mListMinion[i];
                if (!minion.IsActivate())
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
}
