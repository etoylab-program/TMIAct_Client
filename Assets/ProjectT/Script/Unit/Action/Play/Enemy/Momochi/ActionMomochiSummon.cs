
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMomochiSummon : ActionEnemyBase
{
    protected static int MAX_SUMMON_NUM = 2;

    protected Vector3       mCenterOfMap    = Vector3.zero;
    protected List<Enemy>   mListMinion     = new List<Enemy>();
    protected int           mCurSummonNum   = 0;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Summon;

        mCurSummonNum = 0;
    }

    public override void InitAfterOwnerInit()
    {
        mOwnerEnemy = m_owner as Enemy;

        int minionId = 0;
        Enemy minion = null;

        string[] split = Utility.Split(mOwnerEnemy.data.MinionId, ','); //mOwnerEnemy.data.MinionId.Split(',');
		for (int i = 0; i < split.Length; i++)
        {
            minionId = int.Parse(split[i]);

            minion = GameSupport.CreateEnemy(minionId);
            if (minion == null)
            {
                Debug.LogError(split[i] + "번 몬스터를 생성할 수 없습니다.");
                continue;
            }

            minion.IsSummonEnemy = true;
            mListMinion.Add(minion);
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        Vector3 playerPos = World.Instance.Player.transform.position;
        playerPos.y = mOwnerEnemy.transform.position.y;

        if (World.Instance.TestScene)
        {
            mCenterOfMap = Vector3.zero;
        }
        else
        {
            BattleAreaManager battleAreaMgr = World.Instance.EnemyMgr as BattleAreaManager;
            BattleArea battleArea = battleAreaMgr.GetCurrentBattleArea();

            mCenterOfMap = battleArea.center.transform.position;
        }

        mOwnerEnemy.SetInitialPosition(mCenterOfMap, Quaternion.LookRotation((playerPos - mOwnerEnemy.transform.position).normalized));

        m_aniCutFrameLength = mOwnerEnemy.aniEvent.GetCutFrameLength(eAnimation.Skill01);
        m_aniLength = mOwnerEnemy.PlayAni(eAnimation.Skill01);

        //World.Instance.InGameCamera.SetDefaultModeTarget(null, true);
    }

    public override IEnumerator UpdateAction()
    {
        Vector3 v = Vector3.zero;
        Vector3 minionPos = Vector3.zero;
        bool summoned = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            if(!summoned)
            {
                m_checkTime += m_owner.fixedDeltaTime;
                if(m_checkTime >= m_aniCutFrameLength)
                {
                    summoned = true;
                    for(int i = 0; i < mListMinion.Count; i++)
                    {
                        if (i == 0)
                        {
                            v = (mOwnerEnemy.transform.forward + mOwnerEnemy.transform.right).normalized;
                        }
                        else if (i == 1)
                        {
                            v = (mOwnerEnemy.transform.forward - mOwnerEnemy.transform.right).normalized;
                        }
                        else if (i == 2)
                        {
                            v = (-mOwnerEnemy.transform.forward + mOwnerEnemy.transform.right).normalized;
                        }
                        else if(i == 3)
                        {
                            v = (-mOwnerEnemy.transform.forward - mOwnerEnemy.transform.right).normalized;
                        }

                        minionPos = mOwnerEnemy.transform.position + (v * 1.5f);

                        mListMinion[i].SetInitialPosition(minionPos, mOwnerEnemy.transform.rotation);
                        mListMinion[i].Activate();

                        World.Instance.Player.AddEnemyNavigatorTarget();
                        World.Instance.Player.SetEnemyNavigatorTarget(mListMinion[i]);
                    }
                    
                    mOwnerEnemy.Hide();
                }
            }
            else
            {
                bool bAllDead = true;
                for(int i = 0; i < mListMinion.Count; i++)
                {
                    if(mListMinion[i].IsActivate())//mListMinion[i].curHp > 0.0f)
                    {
                        bAllDead = false;
                        break;
                    }
                }

                if(bAllDead)
                {
                    //World.Instance.InGameCamera.SetDefaultModeTarget(mOwnerEnemy, true);
                    mOwnerEnemy.Show();

                    m_endUpdate = true;
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        ++mCurSummonNum;
        if(mCurSummonNum >= MAX_SUMMON_NUM)
        {
            SetNextAction(eActionCommand.RemoveAction, null);
        }
    }
}
