
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAnneroseEvadeCharge2Attack : ActionCounterAttack {
    private eAnimation  mCurAni     = eAnimation.EvadeCharge2_1;
    private float       mEndTime    = 3.0f;
    private float       mCurEndTime = 0.0f;
    private bool        mAttack     = false;


    public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
        base.Init( tableId, listAddCharSkillParam );
        actionCommand = eActionCommand.HoldingDefBtnAttack;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        superArmor = Unit.eSuperArmor.Lv2;

        if( mValue3 > 0.0f ) {
            mCurAni = eAnimation.EvadeCharge2_2;
        }

        List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllAttackEvent( mCurAni );
        if( mValue1 > 0.0f ) {
            for( int i = 0; i < list.Count; i++ ) {
                list[i].behaviour = eBehaviour.GroggyAttack;
			}
		}
        else if( mValue2 > 0.0f ) {
            for( int i = 0; i < list.Count; i++ ) {
                list[i].behaviour = eBehaviour.KnockBackAttack;
            }
        }
    }

    public override void OnStart( IActionBaseParam param ) {
        base.OnStart( param );
        m_owner.PlayAniImmediate( eAnimation.EvadeCharge2 );

        mCurEndTime = mEndTime;
        mAttack = false;

        ShowSkillNames( m_data );
    }

    public override IEnumerator UpdateAction() {
        while( !m_endUpdate ) {
            m_checkTime += m_owner.fixedDeltaTime;
            if( m_checkTime >= mCurEndTime ) {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnUpdating( IActionBaseParam param ) {
        if( mAttack ) {
            return;
        }

        mAttack = true;
        m_checkTime = 0.0f;

        m_owner.PlayAniImmediate( mCurAni );
        mCurEndTime = m_owner.aniEvent.GetCurCutFrameLength();
    }

    public override void OnEnd() {
        base.OnEnd();

        ForceQuitBuffDebuff = true;
        BOCharSkill.EndBattleOption( BattleOption.eBOTimingType.DuringSkill, TableId );
    }

	public override void OnCancel() {
		base.OnCancel();
        ForceQuitBuffDebuff = true;
    }

	public override float GetAtkRange() {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
        if( evt == null ) {
            return 0.0f;
        }

        return evt.visionRange;
    }
}
