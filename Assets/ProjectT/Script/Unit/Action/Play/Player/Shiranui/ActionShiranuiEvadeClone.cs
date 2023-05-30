
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionShiranuiEvadeClone : ActionExtreamEvade
{
    private Shiranui                    mShiranui               = null;
    private bool                        mIsAddAction            = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        IsCommandCloneAttack2 = true;
        mShiranui = m_owner as Shiranui;
	}

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
		
        mIsAddAction = SetAddAction;

        StartCoroutine(MakeClone());
    }

    private IEnumerator MakeClone()
    {
        int cloneIndex = m_owner.GetDeactivateCloneIndex(1);
        if (cloneIndex >= 1)
        {
            m_owner.SetCloneAttackPowerRate(cloneIndex, mValue2);
        }

        yield return StartCoroutine(ContinueDash());

        mOwnerPlayer.ExtreamEvading = false;
        
        if (cloneIndex < 1)
        {
            yield break;
        }

        Vector3 clonePos = m_owner.transform.position;
        Quaternion cloneRot = m_owner.transform.rotation;

        float length = EffectManager.Instance.Play(m_owner, 50015, EffectManager.eType.Common);
        yield return new WaitForSeconds(length * 0.3f);

        Unit clone = m_owner.GetClone(cloneIndex);

        ActionCloneShiranuiSpecialAttack cloneShiranuiSpecialAttack = clone.gameObject.GetComponent<ActionCloneShiranuiSpecialAttack>();
		if ( cloneShiranuiSpecialAttack == null ) {
			cloneShiranuiSpecialAttack = clone.gameObject.AddComponent<ActionCloneShiranuiSpecialAttack>();
            if ( cloneShiranuiSpecialAttack != null ) {
				clone.actionSystem.AddAction( cloneShiranuiSpecialAttack, 0, null );
			}
		}

        m_owner.ShowClone(cloneIndex, clonePos, cloneRot);
        m_owner.SetCloneShader(cloneIndex, mShiranui.CloneShader);
        m_owner.AddAIToClone(cloneIndex, "CharacterClone");

		ActionShiranuiComboAttack shiranuiComboAttack = m_owner.actionSystem.GetAction<ActionShiranuiComboAttack>( eActionCommand.Attack01 );
		if ( shiranuiComboAttack != null ) {
			clone.actionSystem.RemoveAction( clone.gameObject.GetComponent<ActionComboAttack>() );

			ActionCloneShiranuiComboAttack cloneShiranuiComboAttack = clone.gameObject.GetComponent<ActionCloneShiranuiComboAttack>();
            if ( cloneShiranuiComboAttack == null ) {
				cloneShiranuiComboAttack = clone.gameObject.AddComponent<ActionCloneShiranuiComboAttack>();
			}

			if ( cloneShiranuiComboAttack != null ) {
				cloneShiranuiComboAttack.AttackRatio = shiranuiComboAttack.GetValue1();
				cloneShiranuiComboAttack.AttackAddAction = mIsAddAction;

				if ( mIsAddAction ) {
					cloneShiranuiComboAttack.AddActionValue1 = AddActionValue1;
					cloneShiranuiComboAttack.AddActionValue2 = AddActionValue2;
					cloneShiranuiComboAttack.AddActionValue3 = AddActionValue3;
				}

				clone.actionSystem.AddAction( cloneShiranuiComboAttack, 0, null );
			}
		}

        if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
        {
            clone.SetLockAxis(Unit.eAxisType.Z);
        }
        else
        {
            clone.SetLockAxis(Unit.eAxisType.None);
        }

        clone.UseAttack02 = false;
        float checkTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_owner.curHp > 0.0f)
        {
            checkTime += Time.fixedDeltaTime;
            if (checkTime >= mValue1 || World.Instance.IsEndGame)
            {
                break;
            }

            yield return mWaitForFixedUpdate;
        }

        m_owner.HideClone(cloneIndex);
        EffectManager.Instance.Play(clone.transform.position, 50016, EffectManager.eType.Common);
    }
}
