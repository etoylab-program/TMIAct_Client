
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionRinAttackHoldAttack : ActionSelectSkillBase
{
    private eAnimation  mCurAni = eAnimation.AttackHold;
    private Unit        mTarget = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        actionCommand = eActionCommand.AttackDuringAttack;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        superArmor = Unit.eSuperArmor.Lv1;

        if ( mValue3 > 0.0f ) {
            mCurAni = eAnimation.AttackHold_2;
		}
        else if ( mValue2 > 0.0f || mValue1 > 0.0f ) {
            mCurAni = eAnimation.AttackHold_1;
		}
    }

    public override void OnStart( IActionBaseParam param ) {
        base.OnStart( param );
        ShowSkillNames( m_data );

        m_aniLength = m_owner.PlayAniImmediate( mCurAni );
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( mCurAni );

        if ( FSaveData.Instance.AutoTargetingSkill ) {
            mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
            if ( mTarget ) {
                m_owner.LookAtTarget( mTarget.transform.position );
            }
        }
        else {
            m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
        }
    }

    public override IEnumerator UpdateAction() {
        while ( m_endUpdate == false ) {
            m_checkTime += m_owner.fixedDeltaTime;
            if ( m_checkTime >= m_aniCutFrameLength ) {
                m_endUpdate = true;
            }
            else if( mTarget && mTarget.curHp <= 0.0f ) {
                if ( FSaveData.Instance.AutoTargetingSkill ) {
                    mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
                    if ( mTarget ) {
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

    public override void OnCancel() {
        base.OnCancel();
        m_owner.StopStepForward();
    }

    public override float GetAtkRange() {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
        if ( evt == null ) {
            Debug.LogError( mCurAni.ToString() + "공격 이벤트가 없네??" );
            return 0.0f;
        }
        else if ( evt.visionRange <= 0.0f ) {
            Debug.LogError( mCurAni.ToString() + "Vistion Range가 0이네??" );
        }

        return evt.visionRange;
    }
}
