
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEnemySummonMinion : ActionEnemyBase
{
    [Header("[Summon Property]")]
    public eAnimation AniSummon;

    private List<Enemy>     mListMinion     = new List<Enemy>();
    private List<Vector3>   mListMinionPos  = new List<Vector3>();
    private List<int>       mRandomNumber   = new List<int>();


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
		/*
        if (mOwnerEnemy == null)
        {
            return;
        }
		*/
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Summon;
    }

    public override void InitAfterOwnerInit()
    {
        if(mOwnerEnemy == null)
        {
            return;
        }

		string[] split = Utility.Split(mOwnerEnemy.data.MinionId, ','); //mOwnerEnemy.data.MinionId.Split(',');
		for (int i = 0; i < split.Length; i++)
        {
			int minionId = 0;
			if (!Utility.SafeTryIntParse(split[i], out minionId) || minionId <= 0)
			{
				Debug.Log(mOwnerEnemy.data.MinionId + " 미니언 아이디 잘못됨.");
				continue;
			}

            Enemy minion = GameSupport.CreateEnemy(minionId);
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

        if (mOwnerEnemy.SummoningMinion)
        {
            m_endUpdate = true;
        }
        else
        {
            UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
            m_owner.LookAtTarget(targetCollider.GetCenterPos());

            m_aniLength = mOwnerEnemy.PlayAni(AniSummon);
            m_aniCutFrameLength = mOwnerEnemy.aniEvent.GetCutFrameLength(AniSummon);
        }
    }

    public override IEnumerator UpdateAction()
    {
        bool summon = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate)
        {
            m_checkTime += mOwnerEnemy.fixedDeltaTime;

            if(m_checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }
            else if(!summon && m_checkTime >= m_aniCutFrameLength)
            {
                summon = true;
                SummonMinions();
            }

            yield return mWaitForFixedUpdate;
        }
    }

    private void SummonMinions()
    {
        mListMinionPos.Clear();

        for (int i = 0; i < (int)Unit.eDirection.All; i++)
        {
            Vector3 ownerPos = mOwnerEnemy.transform.position;
            Vector3 dir = mOwnerEnemy.GetTargetDirection(mOwnerEnemy, i);

            if(Physics.Raycast(ownerPos, dir, out RaycastHit hitInfo, 2.0f, (1 << (int)eLayer.Wall) | (1 << (int)eLayer.Wall_Inside)))
            {
                continue;
            }

            mListMinionPos.Add(ownerPos + (dir * 2.0f));
        }

        mRandomNumber.Clear();
        for(int i = 0; i < mListMinionPos.Count; i++)
        {
            mRandomNumber.Add(i);
        }

        for (int i = 0; i < mListMinion.Count; i++)
        {
            if (mRandomNumber.Count > 0 && mListMinionPos.Count > 0)
            {
                int randomIndex = Random.Range(0, mRandomNumber.Count);
                int posIndex = mRandomNumber[randomIndex];

                mListMinion[i].SetInitialPosition(mListMinionPos[posIndex], Quaternion.LookRotation((mListMinionPos[posIndex] - mOwnerEnemy.GetCenterPos()).normalized));
                mRandomNumber.RemoveAt(randomIndex);
            }
            else
            {
                mListMinion[i].SetInitialPosition(mOwnerEnemy.transform.position, mOwnerEnemy.transform.rotation);
            }

            mListMinion[i].Activate();
        }

        mOwnerEnemy.SummoningMinion = true;
    }

    private void Update()
    {
        if(mOwnerEnemy == null || !mOwnerEnemy.SummoningMinion)
        {
            return;
        }

        for(int i = 0; i < mListMinion.Count; i++)
        {
            if(mListMinion[i].IsActivate())
            {
                return;
            }
        }

        mOwnerEnemy.SummoningMinion = false;
    }
}
