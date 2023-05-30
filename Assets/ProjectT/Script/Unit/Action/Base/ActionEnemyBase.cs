
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEnemyBase : ActionBase
{
    [Header("[Battle Option]")]
    public int[] BOSetIds;

    protected Enemy         mOwnerEnemy = null;
    protected List<BOEnemy> mListBO     = new List<BOEnemy>();


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        mOwnerEnemy = m_owner as Enemy;

        if (BOSetIds != null)
        {
            mListBO.Clear();
            for (int i = 0; i < BOSetIds.Length; i++)
            {
                BOEnemy boEnemy = AddBOSet(BOSetIds[i]);

                if (mOwnerEnemy)
                {
                    mOwnerEnemy.ListBO.Add(boEnemy); // TimingType이 Use가 아닌것들의 배틀옵션을 실행하기 위해 소유자에도 리스트 생성해놓음
                }
            }
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        // Use는 내가 갖고 있는 배틀옵션만 실행
        for(int i = 0; i < mListBO.Count; i++)
        {
            mListBO[i].Execute(BattleOption.eBOTimingType.Use);
        }
    }

    public override void OnCancel()
    {
        if (mOwnerEnemy)
        {
            if (mOwnerEnemy.curShield > 0.0f && mOwnerEnemy.CurrentSuperArmor <= Unit.eSuperArmor.Lv2)
            {
                mOwnerEnemy.ForceSetSuperArmor(Unit.eSuperArmor.Lv2);
            }
            else
            {
                mOwnerEnemy.ForceSetSuperArmor(Unit.eSuperArmor.None);
            }
        }

        base.OnCancel();
    }

    public BOEnemy AddBOSet(int id)
    {
        BOEnemy bo = new BOEnemy(id, m_owner);
        mListBO.Add(bo);

        return bo;
    }

	public virtual void ExecuteBattleOption( BattleOption.eBOTimingType timingType, Projectile projectile = null ) {
		for ( int i = 0; i < mListBO.Count; i++ ) {
			mListBO[i].Execute( timingType, 0, projectile );
		}
	}
}
