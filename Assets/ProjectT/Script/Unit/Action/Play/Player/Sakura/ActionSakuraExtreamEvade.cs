
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSakuraExtreamEvade : ActionExtreamEvade
{
    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        StartEvadingBuff();
    }

    private void StartEvadingBuff()
    {
        mOwnerPlayer.ExtreamEvading = true;

        float duration = GameInfo.Instance.BattleConfig.BuffDuration;

        StopCoroutine("EndEvadingBuff");
        StartCoroutine("EndEvadingBuff", duration);

        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
        mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
        mBuffEvt.Set(m_data.ID, eEventSubject.ActiveEnemies, eEventType.EVENT_DEBUFF_HOLD_POSITION, m_owner, 0.0f, 0.0f, 0.0f, duration, 0.0f, 129, 130, eBuffIconType.Debuff_Nomove);

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
    }
}
