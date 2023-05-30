
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterRinkoDraggedAttack : ActionSupporterSkillBase
{
    private List<UnitCollider>  mListTargetCollider = null;
    private Vector3             mEffPos             = Vector3.zero;
    private float               mAttackPower        = 0.0f;
	private AniEvent.sEvent		mAniEvt				= null;


	public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterRinkoDraggedAttack;
	}

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParamFromBO = param as ActionParamFromBO;

		if (mAniEvt == null)
		{
			mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.GroggyAttack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f);
		}
	}

    public override IEnumerator UpdateAction()
    {
        Vector3 ownerPos = m_owner.posOnGround + (m_owner.transform.forward * m_owner.MainCollider.radius);
        mEffPos = ownerPos;

        mListTargetCollider = m_owner.GetEnemyColliderList();
        if (mListTargetCollider != null && mListTargetCollider.Count > 0)
        {
            //Vector3 centerPos = Vector3.zero;
            //for (int i = 0; i < mListTargetCollider.Count; i++)
            //{
            //    Unit target = mListTargetCollider[i].Owner;
            //    if (target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback())
            //    {
            //        continue;
            //    }

            //    centerPos += target.transform.position;
            //}

            //centerPos /= mListTargetCollider.Count;
            //float moveDist = Utility.GetDistanceWithoutY(centerPos, ownerPos);

            mEffPos = Vector3.zero;
            /*
            for (int i = 0; i < mListTargetCollider.Count; i++)
            {
                Unit target = mListTargetCollider[i].Owner;
                if (target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback())
                {
                    continue;
                }

                target.actionSystem.CancelCurrentAction();
                target.StopStepForward();

                if (mParamFromBO.battleOptionData.effId1 > 0)
                {
                    EffectManager.Instance.Play(target.GetCenterPos(), mParamFromBO.battleOptionData.effId1, EffectManager.eType.Each_Monster_Normal_Hit);
                }
            }

            yield return new WaitForSeconds(0.2f);
            */
            for (int i = 0; i < mListTargetCollider.Count; i++)
            {
                Unit target = mListTargetCollider[i].Owner;
                if (target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback())
                {
                    continue;
                }

                target.actionSystem.CancelCurrentAction();
                target.StopStepForward();
                target.rigidBody.velocity = Vector3.zero;

                if ( mParamFromBO.battleOptionData.effId1 > 0 ) {
                    EffectManager.Instance.Play( target.GetCenterPos(), mParamFromBO.battleOptionData.effId1, EffectManager.eType.Each_Monster_Normal_Hit );
                }

                Vector3 dir = Utility.GetDirWithoutY(m_owner.transform.position, target.transform.position);

                float moveDist = Utility.GetDistanceWithoutY(ownerPos, target.transform.position);
                Vector3 teleportPos = target.transform.position + (dir * moveDist);
                if (Physics.Linecast(target.transform.position, teleportPos, out RaycastHit hitInfo, 1 << (int)eLayer.Wall))
                {
                    Vector3 v = Utility.GetDirWithoutY(target.transform.position, hitInfo.point);
                    teleportPos = hitInfo.point + (v * target.MainCollider.radius);
                }

                target.SetInitialPosition(teleportPos, target.transform.rotation);
                mEffPos += teleportPos;

                if (mParamFromBO.battleOptionData.effId2 > 0)
                {
                    EffectManager.Instance.Play(target.GetCenterPos(), mParamFromBO.battleOptionData.effId2, EffectManager.eType.Each_Monster_Normal_Hit);
                }
            }

            mEffPos /= mListTargetCollider.Count;
            mAttackPower = m_owner.attackPower * mParamFromBO.battleOptionData.value;
        }

        if (mListTargetCollider == null || mListTargetCollider.Count <= 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(0.1f);

        mAtkEvt.Set(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Supporter, mAniEvt, mAttackPower, eAttackDirection.Skip,
                    false, 0, EffectManager.eType.None, m_owner.GetEnemyColliderList(), 0.0f, true);

        EventMgr.Instance.SendEvent(mAtkEvt);
    }
}
