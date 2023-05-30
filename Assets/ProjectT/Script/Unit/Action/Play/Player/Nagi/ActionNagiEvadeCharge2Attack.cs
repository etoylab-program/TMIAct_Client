
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionNagiEvadeCharge2Attack : ActionSelectSkillBase {
    private eAnimation  mCurAni     = eAnimation.EvadeCharge2;
    private float       mEndTime    = 3.0f;
    private float       mCurEndTime = 0.0f;
    private bool        mAttack     = false;
    private Unit        mTarget     = null;


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

        superArmor = Unit.eSuperArmor.Lv1;

        if( mValue1 > 0.0f ) {
            mCurAni = eAnimation.EvadeCharge2_1;
        }
    }

    public override void OnStart( IActionBaseParam param ) {
        base.OnStart( param );

        if( FSaveData.Instance.AutoTargetingSkill ) {
            mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
            if( mTarget ) {
                m_owner.LookAtTarget( mTarget.transform.position );
            }
        }
        else {
            m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
        }

        m_owner.PlayAniImmediate( mCurAni );

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
            else if( mTarget && mTarget.curHp <= 0.0f ) {
                if( FSaveData.Instance.AutoTargetingSkill ) {
                    mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
                    if( mTarget ) {
                        m_owner.LookAtTarget( mTarget.transform.position );
                    }
                }
                else {
                    m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
                }
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
        BOCharSkill.EndBattleOption( BattleOption.eBOTimingType.DuringSkill, TableId );
    }

    public override float GetAtkRange() {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
        if( evt == null ) {
            return 0.0f;
        }

        return evt.visionRange;
    }
}
