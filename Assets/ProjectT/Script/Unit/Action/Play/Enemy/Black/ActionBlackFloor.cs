
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionBlackFloor : ActionEnemyBase
{
	private eAnimation		mCurAni						= eAnimation.Skill02;
	private Projectile		mPjt						= null;
	private BoxCollider		mBoxFloor					= null;
	private Vector3			mFloorPos					= Vector3.zero;
	private List<Unit>		mListStayOnBoxColliderEnemy	= new List<Unit>();
	private float			mDuration					= 0.0f;


	public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
	{
		base.Init(tableId, listAddCharSkillParam);
		actionCommand = eActionCommand.BlackFloor;
	}

	public override void OnStart(IActionBaseParam param)
	{
		base.OnStart(param);

		if (mPjt == null)
		{
			List<Projectile> list = m_owner.aniEvent.GetAllProjectile(mCurAni);
			if (list.Count > 0)
			{
				mPjt = list[0];
			}

			mBoxFloor = mPjt.BoxCol;
			mDuration = mPjt.duration;
		}

		m_owner.PlayAniImmediate(mCurAni);
		m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();
	}

	public override IEnumerator UpdateAction()
	{
		while (!m_endUpdate)
		{
			m_checkTime += Time.fixedDeltaTime;
			if (m_checkTime >= m_aniCutFrameLength)
			{
				StopCoroutine("UpdateFloor");
				StartCoroutine("UpdateFloor");

				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	private IEnumerator UpdateFloor()
	{
		mFloorPos = mPjt.transform.position;

		BuffEvent buffEvt2 = new BuffEvent();
		WorldPVP worldPVP = World.Instance as WorldPVP;
		List<Unit> listEnemy = null;

		m_checkTime = 0.0f;
		mListStayOnBoxColliderEnemy.Clear();

		while (true)
		{
			m_checkTime += m_owner.fixedDeltaTime;
			if (m_checkTime >= mDuration || (worldPVP && (!worldPVP.IsBattleStart || m_owner.curHp <= 0.0f)))
			{
				for (int i = 0; i < mListStayOnBoxColliderEnemy.Count; i++)
				{
					mListStayOnBoxColliderEnemy[i].StayOnBoxCollider = null;
				}

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
				if (isEnemyInBox && enemy.StayOnBoxCollider == null)
				{
					enemy.StayOnBoxCollider = mBoxFloor;
					mListStayOnBoxColliderEnemy.Add(enemy);

					// 속도 감소
					mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
					mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
					mBuffEvt.battleOptionData.conditionType = BattleOption.eBOConditionType.StayOnBoxCollider;
					mBuffEvt.battleOptionData.buffIconFlash = false;

					mBuffEvt.Set(0, eEventSubject.Self, eEventType.EVENT_DEBUFF_SPEED_DOWN, enemy, 0.2f, 0.0f, 0.0f,
								 mDuration, 0.0f, mBuffEvt.effId, mBuffEvt.effId2, eBuffIconType.Debuff_Speed);

					EventMgr.Instance.SendEvent(mBuffEvt);
				}
				else if (!isEnemyInBox && enemy.StayOnBoxCollider)
				{
					enemy.StayOnBoxCollider = null;
					mListStayOnBoxColliderEnemy.Remove(enemy);
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}
}
