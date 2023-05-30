
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class ActionAsagiExtreamEvade : ActionExtreamEvade
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        ExtreamEvadeProfile = ResourceMgr.Instance.LoadFromAssetBundle("etc", "Etc/PostProcess/slowmotion_invert.asset") as PostProcessProfile;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        StartEvadingBuff();
    }

    private void StartEvadingBuff()
    {
        mOwnerPlayer.ExtreamEvading = true;

        float duration = GameInfo.Instance.BattleConfig.BuffDuration + (GameInfo.Instance.BattleConfig.BuffDuration * (mValue2 / (float)eCOUNT.MAX_BO_FUNC_VALUE));

        if( !mOwnerPlayer.IsHelper ) {
            World.Instance.InGameCamera.ChangePostProcessProfile( ExtreamEvadeProfile, duration );
        }

        StopCoroutine("EndEvadingBuff");
        StartCoroutine("EndEvadingBuff", duration);

        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
        mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
        mBuffEvt.Set(m_data.ID, eEventSubject.ActiveEnemies, eEventType.EVENT_DEBUFF_SPEED_DOWN, m_owner, mValue1, 0.0f, 0.0f, duration, 0.0f,
                     mBuffEvt.effId, mBuffEvt.effId2, eBuffIconType.Debuff_Speed);

        EventMgr.Instance.SendEvent(mBuffEvt);
    }

    public override void OnCancel()
    {
        base.OnCancel();

        if( !mOwnerPlayer.IsHelper ) {
            World.Instance.InGameCamera.ResetPostProcess();
        }
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
    }
}
