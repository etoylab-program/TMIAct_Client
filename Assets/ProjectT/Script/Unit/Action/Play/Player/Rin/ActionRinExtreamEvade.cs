
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionRinExtreamEvade : ActionExtreamEvade
{
    public override void OnStart(IActionBaseParam param)
    {
        BOCharSkill.ChangeBattleOptionDuration( BattleOption.eBOTimingType.DuringSkill, TableId, mValue1 + AddBuffDuration );

        base.OnStart(param);
        StartEvadingBuff();
    }

    private void StartEvadingBuff()
    {
        mOwnerPlayer.ExtreamEvading = true;

        StopCoroutine( "EndEvadingBuff" );
        StartCoroutine( "EndEvadingBuff", mValue1 + AddBuffDuration );

        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Buff;
        mBuffEvt.Set( m_data.ID, eEventSubject.Self, eEventType.EVENT_BUFF_SPEED_UP, m_owner, 0.2f, 0.0f, 0.0f, mValue1, 0.0f, 0, 0, eBuffIconType.Buff_Speed );
        EventMgr.Instance.SendEvent( mBuffEvt );
    }

    public override void OnEnd()
    {
        base.OnEnd();
        IsSkillEnd = false;
    }

    private IEnumerator EndEvadingBuff(float duration)
    {
        yield return StartCoroutine(ContinueDash());

        GameObject parent = m_owner.aniEvent.gameObject;

        Transform bone = m_owner.aniEvent.GetBoneByName( "Bip001" );
        if ( bone ) {
            parent = bone.gameObject;
		}

        EffectManager.Instance.Play( parent, 30034, EffectManager.eType.Common );
        ParticleSystem ps = EffectManager.Instance.GetEffectOrNull( 30034, EffectManager.eType.Common );
        if ( ps ) {
            Utility.InitTransform( ps.gameObject, new Vector3( 0.00257f, 0.23f, -0.96f ), Quaternion.Euler( -20.4f, -83.2f, -102.6f ), Vector3.one );
		}

        yield return new WaitForSeconds(duration);

        mOwnerPlayer.ExtreamEvading = false;
        IsSkillEnd = true;

        EffectManager.Instance.StopEffImmediate( 30034, EffectManager.eType.Common, null );
    }
}
