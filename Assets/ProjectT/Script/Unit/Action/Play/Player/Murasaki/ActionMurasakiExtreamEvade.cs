
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class ActionMurasakiExtreamEvade : ActionExtreamEvade
{
    private float mDuration = 0.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        mDuration = GameInfo.Instance.BattleConfig.BuffDuration + (GameInfo.Instance.BattleConfig.BuffDuration * (mValue2 / (float)eCOUNT.MAX_BO_FUNC_VALUE));
    }

    public override void OnStart(IActionBaseParam param)
    {
        BOCharSkill.ChangeBattleOptionDuration(BattleOption.eBOTimingType.DuringSkill, TableId, mDuration);

        base.OnStart(param);
        StartEvadingBuff();
    }

    private void StartEvadingBuff()
    {
        mOwnerPlayer.ExtreamEvading = true;

        StopCoroutine("EndEvadingBuff");
        StartCoroutine("EndEvadingBuff", mDuration);

        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
        mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
        mBuffEvt.Set(m_data.ID, eEventSubject.ActiveEnemies, eEventType.EVENT_DEBUFF_SPEED_DOWN, m_owner, mValue1, 0.0f, 0.0f, mDuration, 0.0f,
                     131, 0, eBuffIconType.Debuff_Speed);

        EventMgr.Instance.SendEvent(mBuffEvt);
    }

    public override void OnCancel()
    {
        base.OnCancel();
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

        m_checkTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            if (!World.Instance.IsPause)
                m_checkTime += Time.fixedDeltaTime;

            if (m_checkTime >= duration)
            {
                mOwnerPlayer.ExtreamEvading = false;
                IsSkillEnd = true;

                break;
            }

            yield return mWaitForFixedUpdate;
        }

        BOCharSkill.EndBattleOption(BattleOption.eBOTimingType.DuringSkill, TableId);
    }
}
