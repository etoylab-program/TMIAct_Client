
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionOboroExtreamEvade : ActionExtreamEvade
{
    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        StartEvadingBuff();
    }

    private void StartEvadingBuff()
    {
        mOwnerPlayer.ExtreamEvading = true;

        float duration = GameInfo.Instance.BattleConfig.BuffDuration + mValue3;

        StopCoroutine("EndEvadingBuff");
        StartCoroutine("EndEvadingBuff", duration);

        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
        mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
        mBuffEvt.Set(m_data.ID, eEventSubject.ActiveEnemiesInRange, eEventType.EVENT_DEBUFF_SPEED_DOWN_AND_SKIP_CUR_ANI, m_owner, 10.0f, mValue1, mValue2, 
                     duration, 0.0f, 20063, mBuffEvt.effId2, eBuffIconType.Debuff_Speed);

        EventMgr.Instance.SendEvent(mBuffEvt);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        IsSkillEnd = false;
    }

    private IEnumerator EndEvadingBuff(float duration)
    {
        yield return StartCoroutine(ContinueDash());
        //m_owner.RestoreSuperArmor(superArmor, GetType());

        yield return new WaitForSeconds(duration);

        mOwnerPlayer.ExtreamEvading = false;
        IsSkillEnd = true;

        for (int i = 0; i < EventMgr.Instance.ListSendTarget.Count; i++)
        {
            Unit target = EventMgr.Instance.ListSendTarget[i];
            if (target == null || !target.IsActivate() || target.curHp <= 0.0f || target.actionSystem == null)
            {
                continue;
            }

            //target.SkipCurrentAni();
            //target.ResetBT();
        }
    }
}
