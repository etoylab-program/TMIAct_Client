
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMomochiCreatureSummon : ActionEnemyBase
{
    protected Vector3 mCenterOfMap = Vector3.zero;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Summon;
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
            mOwnerEnemy.ListMinion.Add(minion);
        }
    }

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		Vector3 playerPos = World.Instance.Player.transform.position;
		playerPos.y = World.Instance.Player.posOnGround.y;

		if( World.Instance.TestScene ) {
			mCenterOfMap = Vector3.zero;
		}
		else {
			BattleAreaManager battleAreaMgr = World.Instance.EnemyMgr as BattleAreaManager;
			BattleArea battleArea = battleAreaMgr.GetCurrentBattleArea();

			mCenterOfMap = battleArea.center.transform.position;
		}

		m_aniCutFrameLength = mOwnerEnemy.aniEvent.GetCutFrameLength( eAnimation.Skill01 );
		m_aniLength = mOwnerEnemy.PlayAni( eAnimation.Skill01 );

		mOwnerEnemy.SummoningMinion = true;
	}

	public override IEnumerator UpdateAction()
    {
        Vector3 v = Vector3.zero;
        Vector3 minionPos = Vector3.zero;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= m_aniCutFrameLength)
            {
                for (int i = 0; i < mOwnerEnemy.ListMinion.Count; i++)
                {
                    if (i <= 1)
                    {
                        v = mOwnerEnemy.transform.right * (3.0f * (i + 1));
                    }
                    else if (i <= 3)
                    {
                        v = -mOwnerEnemy.transform.right * (3.0f * (i - 1));
                    }

                    minionPos = mCenterOfMap + v;

                    mOwnerEnemy.ListMinion[i].SetInitialPosition(minionPos, mOwnerEnemy.transform.rotation);
                    mOwnerEnemy.ListMinion[i].Activate();

                    World.Instance.Player.AddEnemyNavigatorTarget();
                    World.Instance.Player.SetEnemyNavigatorTarget(mOwnerEnemy.ListMinion[i]);
                }

                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }
    }
}
