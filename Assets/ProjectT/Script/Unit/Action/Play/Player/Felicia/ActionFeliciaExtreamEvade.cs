
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionFeliciaExtreamEvade : ActionExtreamEvade
{
	private float			mDuration	= 0.0f;
	private ParticleSystem	mEffFloor	= null;
	private BoxCollider		mBoxFloor	= null;
	private Vector3			mFloorPos	= Vector3.zero;

	public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
	{
		base.Init(tableId, listAddCharSkillParam);

		if (mValue2 > 0.0f)
		{
			mEffFloor = GameSupport.CreateParticle("Effect/Character/prf_fx_felicia_battle_evade_skill_upgrade.prefab", null);
		}
		else
		{
			mEffFloor = GameSupport.CreateParticle("Effect/Character/prf_fx_felicia_battle_evade_skill.prefab", null);
		}

		mBoxFloor = mEffFloor.GetComponent<BoxCollider>();
		mDuration = mEffFloor.main.duration;
	}

	public override void OnStart(IActionBaseParam param)
	{
		base.OnStart(param);

		StopCoroutine("UpdateSkill");
		StartCoroutine("UpdateSkill");
	}

	private IEnumerator UpdateSkill()
	{
		yield return StartCoroutine(ContinueDash());

		mFloorPos = m_owner.posOnGround;// + (m_owner.transform.forward * 1.25f);

		mEffFloor.transform.position = mFloorPos;
		mEffFloor.gameObject.SetActive(true);
		EffectManager.Instance.RegisterStopEffByDuration(mEffFloor, null, mDuration);

		float healPercentage = mValue3 / (float)eCOUNT.MAX_BO_FUNC_VALUE;

		WorldPVP worldPVP = World.Instance as WorldPVP;
		List<Unit> listEnemy = null;

		m_checkTime = 0.0f;

		//WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
		while (true)
		{
			m_checkTime += m_owner.fixedDeltaTime;
			if (m_checkTime >= mDuration || (worldPVP && (!worldPVP.IsBattleStart || m_owner.curHp <= 0.0f)))
			{
				mOwnerPlayer.ExtreamEvading = false;
				EffectManager.Instance.StopEffImmediate(mEffFloor, null);

				yield break;
			}

			listEnemy = m_owner.GetEnemyList(true);
			for (int i = 0; i < listEnemy.Count; i++)
			{
				Unit enemy = listEnemy[i];
				if (enemy.curHp <= 0.0f)
				{
					continue;
				}

				bool isEnemyInBox = mBoxFloor.bounds.Contains(enemy.transform.position);
				if (isEnemyInBox && enemy.MarkingInfo.MarkingType != Unit.eMarkingType.GiveHp)
				{
					mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
					mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;

					mBuffEvt.Set(m_data.ID, eEventSubject.Self, eEventType.EVENT_DEBUFF_MARKING, enemy, healPercentage, 2.0f, 1.0f, mValue1, 0.0f,
								 30028, mBuffEvt.effId2, eBuffIconType.Debuff_Temp6);

					EventMgr.Instance.SendEvent(mBuffEvt);
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}
}
