
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMaikaExtreamEvade : ActionExtreamEvade
{
	private List<PlayerMinion>	mListMinion		= new List<PlayerMinion>();
	private int					mMinionCount	= 1;
	private float				mDuration		= 8.0f;


	public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
	{
		base.Init(tableId, listAddCharSkillParam);

		mMinionCount = (int)mValue1;
		mDuration = mValue3;

		for (int i = 0; i < mMinionCount; i++)
		{
			PlayerMinion minion = GameSupport.CreatePlayerMinion(22, mOwnerPlayer);
			mListMinion.Add(minion);
		}
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
		StartSummon();

		WorldPVP worldPVP = World.Instance as WorldPVP;
		while (true)
		{
			m_checkTime += m_owner.fixedDeltaTime;
			if (m_checkTime >= mDuration || (worldPVP && (!worldPVP.IsBattleStart || m_owner.curHp <= 0.0f)))
			{
				mOwnerPlayer.ExtreamEvading = false;
				yield break;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	private void StartSummon()
	{
		AllMinionDeactivate();

		for (int i = 0; i < mMinionCount; i++)
		{
			PlayerMinion minion = mListMinion[i];
			Utility.SetLayer(minion.gameObject, (int)eLayer.PlayerClone, true);

			Vector3 pos = mOwnerPlayer.transform.position;

			if (i == 0)
			{
				pos += (mOwnerPlayer.transform.right * 2.0f);
			}
			else if (i == 1)
			{
				pos -= (mOwnerPlayer.transform.right * 2.0f);
			}
			else
			{
				int randX = UnityEngine.Random.Range(-5, 6);
				int randY = UnityEngine.Random.Range(0, 4);
				int randZ = UnityEngine.Random.Range(-5, 6);

				pos.x += (float)randX / 10.0f;
				pos.y += (float)randY / 10.0f;
				pos.z += (float)randZ / 10.0f;
			}

			minion.SetInitialPosition(pos, mOwnerPlayer.transform.rotation);
			minion.SetMinionAttackPower(mOwnerPlayer.gameObject.layer, mOwnerPlayer.attackPower * mValue2);
			minion.Activate();
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
			if (mListMinion[i].IsActivate())
			{
				return false;
			}
		}

		return true;
	}

	private IEnumerator UpdateMinion() {
		float checkTime = 0.0f;
		bool endUpdate = false;

		while( !endUpdate ) {
			checkTime += Time.fixedDeltaTime;
			if( checkTime >= mDuration || m_owner.curHp <= 0.0f ) {
				endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}

		if( !World.Instance.IsEndGame && !World.Instance.ProcessingEnd ) {
			float aniLength = 0.0f;
			for( int i = 0; i < mListMinion.Count; i++ ) {
				PlayerMinion minion = mListMinion[i];
				if( !minion.IsActivate() ) {
					continue;
				}

				minion.StopBT();

				aniLength = minion.PlayAniImmediate( eAnimation.Die );
				minion.StartDissolve( aniLength, false, new Color( 0.169f, 0.0f, 0.47f ) );
			}

			yield return new WaitForSeconds( aniLength );
		}

		AllMinionDeactivate();
	}
}
