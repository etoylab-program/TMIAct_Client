
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionPlayerMinionSummonMinion : ActionBase
{
    [Header("[Summon Property]")]
    public eAnimation AniSummon;

    private Unit                mSuperOwner     = null;
    private PlayerMinion        mOwnerMinion    = null;
    private List<PlayerMinion>  mListMinion     = new List<PlayerMinion>();
    private List<Vector3>       mListMinionPos  = new List<Vector3>();
    private List<int>           mRandomNumber   = new List<int>();


    public void SetSuperPlayer(Unit superOwner)
    {
        mSuperOwner = superOwner;
    }

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        actionCommand = eActionCommand.Summon;
        mOwnerMinion = m_owner as PlayerMinion;
    }

    public override void InitAfterOwnerInit()
    {
        string[] split = Utility.Split(mOwnerMinion.Data.MinionId, ','); //mOwnerMinion.Data.MinionId.Split(',');
		for (int i = 0; i < split.Length; i++)
        {
            int minionId = int.Parse(split[i]);

            PlayerMinion minion = GameSupport.CreatePlayerMinion(minionId, mSuperOwner);
            if (minion == null)
            {
                Debug.LogError(split[i] + "번 몬스터를 생성할 수 없습니다.");
                continue;
            }

            mListMinion.Add(minion);
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        if (mOwnerMinion.SummoningMinion)
        {
            m_endUpdate = true;
        }
        else
        {
            UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
            m_owner.LookAtTarget(targetCollider.GetCenterPos());

            m_aniLength = mOwnerMinion.PlayAni(AniSummon);
            m_aniCutFrameLength = mOwnerMinion.aniEvent.GetCutFrameLength(AniSummon);
        }
    }

    public override IEnumerator UpdateAction()
    {
        bool summon = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate)
        {
            m_checkTime += mOwnerMinion.fixedDeltaTime;

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
            Vector3 ownerPos = mOwnerMinion.transform.position;
            Vector3 dir = mOwnerMinion.GetTargetDirection(mOwnerMinion, i);

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

                mListMinion[i].SetInitialPosition(mListMinionPos[posIndex], Quaternion.LookRotation((mListMinionPos[posIndex] - mOwnerMinion.GetCenterPos()).normalized));
                mRandomNumber.RemoveAt(randomIndex);
            }
            else
            {
                mListMinion[i].SetInitialPosition(mOwnerMinion.transform.position, mOwnerMinion.transform.rotation);
            }

            mListMinion[i].Activate();
        }

        mOwnerMinion.SummoningMinion = true;
    }

    private void Update()
    {
        if(!mOwnerMinion.SummoningMinion)
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

        mOwnerMinion.SummoningMinion = false;
    }
}
