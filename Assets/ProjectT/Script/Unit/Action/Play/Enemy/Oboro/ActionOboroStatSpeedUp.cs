
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionOboroStatSpeedUp : ActionEnemyBase
{
    private float           mValue  = 0.0f;
    private ParticleSystem  mEff    = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.StatSpeedUp;

        mEff = GameSupport.CreateParticle("Effect/Monster/prf_fx_monster_oboro_skill_loop.prefab", m_owner.transform);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        ActionParamAI paramAI = param as ActionParamAI;
        mValue = Utility.GetRandom(paramAI.minValue, paramAI.maxValue, 10.0f);

        m_aniLength = m_owner.PlayAni(eAnimation.Skill01);
        
        mEff.gameObject.SetActive(true);
        EffectManager.Instance.RegisterStopEffAtOwnerDie(mEff, m_owner);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if(m_checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        mBuffEvt.Set(0, eEventSubject.Self, eEventType.EVENT_STAT_SPEED_UP, m_owner, mValue, 0.0f, 0.0f, 0.0f, 0.0f, 0, 0, eBuffIconType.None);
        EventMgr.Instance.SendEvent(mBuffEvt);
    }
}
