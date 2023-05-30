
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAttack : ActionEnemyAttackBase
{
    [Header("[Enemy Attack Property]")]
    public eAnimation ani;
    public eActionCommand actionCmd;

    private ActionParamAI mParam = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = actionCmd;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mParam = param as ActionParamAI;

        m_aniLength = m_owner.PlayAni(ani);
        
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(ani);
        if(m_aniCutFrameLength > 0.0f)
        {
            m_aniLength = m_aniCutFrameLength;
        }

        m_checkTime = 0.0f;

        if (m_owner.CurrentSuperArmor == Unit.eSuperArmor.Lv1)
        {
            m_owner.aniEvent.SetFlash(new Color(1.0f, 0.1f, 0.0f), 0.4f, 1.0f, m_aniLength);
        }

        bool getOnlyEnemy = false;

        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(ani);
        if(evt != null && evt.behaviour == eBehaviour.Projectile)
        {
            getOnlyEnemy = true;
        }

        UnitCollider targetCollider = m_owner.GetMainTargetCollider( getOnlyEnemy );
        if( targetCollider ) {
            if( !mParam.LookAtByRotate || m_owner.cmptRotate == null ) {
                LookAtTarget( targetCollider.Owner );
            }
            else {
                LookAtTargetByCmptRotate( targetCollider.Owner );
            }
        }
    }

    public override IEnumerator UpdateAction()
    {
        float firstAtkEventLength = m_owner.aniEvent.GetFirstAttackEventLength(ani);
        AniEvent.sAniInfo aniInfo = m_owner.aniEvent.GetAniInfo(ani);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            if (aniInfo.rotateToTarget && m_checkTime <= firstAtkEventLength)
            {
                m_checkTime += m_owner.fixedDeltaTime;//Time.fixedDeltaTime;

                UnitCollider targetCollider = m_owner.GetMainTargetCollider(false);
                if (targetCollider && targetCollider.Owner)
                {
                    LookAtTarget(targetCollider.Owner);
                }
            }
            else
            {
                m_checkTime += m_owner.fixedDeltaTime;//Time.fixedDeltaTime;
                if (m_checkTime >= m_aniLength)
                    m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetNextAttackEvent(ani, false);
        if (evt == null)
            return 0.0f;

        return Mathf.Max(evt.visionRange, evt.atkRange);
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_owner.aniEvent.SetOriginalShaderColor();
    }
}
