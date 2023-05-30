
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEmilyHomingAttack : ActionSelectSkillBase
{
    public class sPjtData
    {
        public Projectile               Pjt     = null;
        public AniEvent.sProjectileInfo Info    = null;
    }


    private int             mPjtCount       = 4;
    private List<sPjtData>  mListPjtData    = new List<sPjtData>();
    private List<sPjtData>  mListPjtData2   = new List<sPjtData>();
    private AniEvent.sEvent mAniEvt         = null;
    private List<sPjtData>  mListCurPjtData = null;
    private float           mOriginalValue2 = 0.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.AttackDuringAttack;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        superArmor = Unit.eSuperArmor.Lv1;

        mOriginalValue2 = mValue2;
        mPjtCount = (int)mValue1;

        for (int i = 0; i < mPjtCount; i++)
        {
            sPjtData data = new sPjtData();

            data.Pjt = GameSupport.CreateProjectile("Projectile/pjt_character_emily_homing.prefab");

            data.Info = m_owner.aniEvent.CreateProjectileInfo(data.Pjt);
            data.Info.attach = true;
            data.Info.boneName = "bone_armor";
            data.Info.followParentRot = true;

            mListPjtData.Add(data);
        }

        // 에밀리 17번 무기로 발사체 교체했을 때 사용
        for (int i = 0; i < mPjtCount + 1; i++)
        {
            sPjtData data = new sPjtData();

            data.Pjt = GameSupport.CreateProjectile("Projectile/pjt_character_emily_homing_wskill.prefab");

            data.Info = m_owner.aniEvent.CreateProjectileInfo(data.Pjt);
            data.Info.attach = true;
            data.Info.boneName = "bone_armor";
            data.Info.followParentRot = true;

            mListPjtData2.Add(data);
        }

        mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, mValue2);
        ChangeProjectile(0);
    }

    public override void OnStart(IActionBaseParam param)
    {
        if (mOwnerPlayer.boWeapon != null && mOwnerPlayer.boWeapon.data != null && mOwnerPlayer.boWeapon.data.TableID == 8017) // 아...
        {
            mAniEvt.atkRatio = mOriginalValue2 + (mOriginalValue2 * 0.5f);
            DecreaseSkillCoolTimeValueRatio = 0.5f;

            ChangeProjectile(1);
        }
        else
        {
            mAniEvt.atkRatio = mOriginalValue2;
            DecreaseSkillCoolTimeValueRatio = 0.0f;

            ChangeProjectile(0);
        }

        base.OnStart(param);

        UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
        if(targetCollider == null)
        {
            return;
        }

        SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, "DroneHomingAttack");

        for (int i = 0; i < mListCurPjtData.Count; i++)
        {
            mListCurPjtData[i].Pjt.Fire(m_owner, BattleOption.eToExecuteType.Unit, mAniEvt, mListCurPjtData[i].Info, targetCollider.Owner, TableId);
        }

        SetNextAction(eActionCommand.Nothing, null);
    }

    public override IEnumerator UpdateAction()
    {
        yield return null;
    }

	public override float GetAtkRange() {
		return 20.0f;
	}

	private void ChangeProjectile(int index)
    {
        if(index == 0)
        {
            mListCurPjtData = mListPjtData;
        }
        else
        {
            mListCurPjtData = mListPjtData2;
        }
    }
}
