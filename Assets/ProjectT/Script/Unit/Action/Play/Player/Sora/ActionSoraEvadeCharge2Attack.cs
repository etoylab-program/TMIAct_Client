using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSoraEvadeCharge2Attack : ActionSelectSkillBase {
    private Unit        mTarget = null;
    private eAnimation  mCurAni = eAnimation.EvadeCharge2;


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

        extraCancelCondition = new eActionCondition[3];
        extraCancelCondition[0] = eActionCondition.UseSkill;
        extraCancelCondition[1] = eActionCondition.UseQTE;
        extraCancelCondition[2] = eActionCondition.UseUSkill;

        superArmor = Unit.eSuperArmor.Lv2;

        if( mValue2 > 0.0f ) {
            mCurAni = eAnimation.EvadeCharge2_1;
        }

        if( mValue1 > 0.0f ) {
            List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllMeleeAttackEvent( mCurAni );
            for( int i = 0; i < list.Count; i++ ) {
                list[i].behaviour = eBehaviour.DownAttack;
			}
		}
    }

    public override void OnStart( IActionBaseParam param ) {
        base.OnStart( param );
        ShowSkillNames( m_data );

        if( FSaveData.Instance.AutoTargetingSkill ) {
            mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
            if( mTarget ) {
                m_owner.LookAtTarget( mTarget.transform.position );
            }
        }
        else {
            m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
        }

        m_aniLength = m_owner.PlayAniImmediate( mCurAni );
        m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();
    }

    public override IEnumerator UpdateAction() {
        while( m_endUpdate == false ) {
            m_checkTime += m_owner.fixedDeltaTime;
            if( m_checkTime >= m_aniCutFrameLength ) {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override float GetAtkRange() {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
        if( evt == null ) {
            Debug.LogError( mCurAni.ToString() + "공격 이벤트가 없네??" );
            return 0.0f;
        }
        else if( evt.visionRange <= 0.0f ) {
            Debug.LogError( mCurAni.ToString() + "Vistion Range가 0이네??" );
        }

        return evt.visionRange;
    }
}
