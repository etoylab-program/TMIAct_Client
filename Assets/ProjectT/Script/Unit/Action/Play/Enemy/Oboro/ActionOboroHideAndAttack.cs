
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionOboroHideAndAttack : ActionEnemyAttackBase
{
    [Header("[Proprety]")]
    public float HideTime = 1.0f;

    protected UnitCollider mTargetCollider = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.HideAndAttack;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Grounded;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.Skill02);
    }

    public override IEnumerator UpdateAction()
    {
        yield return new WaitForSeconds(m_aniLength);

        m_owner.ShowMesh(false);
        //m_owner.SetSuperArmor(Unit.eSuperArmor.Invincible, GetType());
        m_owner.TemporaryInvincible = true;
        
        Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_checkTime < HideTime)
        {
            m_checkTime += Time.fixedDeltaTime;
            yield return mWaitForFixedUpdate;
        }

        if (mTargetCollider)
        {
            SetDest();
        }

        m_owner.ShowMesh(true);
        //m_owner.RestoreSuperArmor(Unit.eSuperArmor.Invincible, GetType());
        m_owner.TemporaryInvincible = false;

        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);

        eAnimation aniAtk = eAnimation.Attack08;
        if (Random.Range(0, 2) == 1)
        {
            aniAtk = eAnimation.Attack09;
        }

        m_aniLength = m_owner.PlayAniImmediate(aniAtk);
        yield return new WaitForSeconds(m_aniLength);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        m_owner.checkRayCollision = true;
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_owner.checkRayCollision = true;

        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
    }

    private void SetDest()
    {
        m_owner.checkRayCollision = false;

        Vector3 dest = mTargetCollider.Owner.transform.position - (mTargetCollider.Owner.transform.forward * 5.0f);
        dest.y = m_owner.transform.position.y;

		/*
        if(Physics.Linecast(dest, mTargetCollider.Owner.transform.position, (1 << (int)eLayer.Wall) | (1 << (int)eLayer.Wall_Inside) | (1 << (int)eLayer.EnvObject)))
        {
            dest = mTargetCollider.Owner.transform.position + (mTargetCollider.Owner.transform.forward * 5.0f);
            dest.y = m_owner.transform.position.y;
        }
        else
        {
			Collider[] cols = Physics.OverlapBox(dest, Vector3.one * 0.6f, Quaternion.identity, (1 << (int)eLayer.Wall) | (1 << (int)eLayer.Wall_Inside) | (1 << (int)eLayer.EnvObject));
			if (cols.Length > 0)
			{
				dest = mTargetCollider.Owner.transform.position + (mTargetCollider.Owner.transform.forward * (5.0f - (float)i));
				dest.y = m_owner.transform.position.y;
			}
		}
		*/

		dest = FindDest(dest);
        m_owner.SetInitialPosition(dest, Quaternion.LookRotation((mTargetCollider.Owner.transform.position - dest).normalized));
    }

	private Vector3 FindDest(Vector3 dest)
	{
		Vector3 find = dest;

		for (int i = 0; i < 10; ++i)
		{
			Collider[] cols = Physics.OverlapBox(find, Vector3.one * 0.6f, Quaternion.identity, (1 << (int)eLayer.Wall) | (1 << (int)eLayer.Wall_Inside) | (1 << (int)eLayer.EnvObject));
			if (cols.Length > 0)
			{
				find = mTargetCollider.Owner.transform.position + (mTargetCollider.Owner.transform.forward * (5.0f - (float)i));
				find.y = m_owner.transform.position.y;
			}
			else
			{
				return find;
			}
		}

		return m_owner.transform.position;

		/*
		Vector3 find = dest;

		for(int i = 0; i < 5; ++i)
		{
			Collider[] cols = Physics.OverlapBox(dest, Vector3.one * 0.6f, Quaternion.identity, (1 << (int)eLayer.Wall) | (1 << (int)eLayer.Wall_Inside) | (1 << (int)eLayer.EnvObject));
			if (cols.Length > 0)
			{
				find = mTargetCollider.Owner.transform.position + (mTargetCollider.Owner.transform.forward * (5.0f - (float)i));
				find.y = m_owner.transform.position.y;
			}
			else
			{
				return find;
			}
		}

		return dest;
		*/
	}
}
