
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionRinEvadeCharge2Attack : ActionSelectSkillBase
{
    private List<Unit>  mListTarget     = null;
    private eAnimation  mCurAni         = eAnimation.EvadeCharge2;
    private Vector3     mBlackholePos   = Vector3.zero;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
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

        if ( mValue3 > 0.0f ) {
            mCurAni = eAnimation.EvadeCharge2_1;
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        if ( FSaveData.Instance.AutoTargetingSkill ) {
            Unit target = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
            if ( target ) {
                m_owner.LookAtTarget( target.transform.position );
            }
        }
        else {
            m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
        }

        m_aniLength = m_owner.PlayAniImmediate(mCurAni);
        m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();
    }

    public override IEnumerator UpdateAction()
    {
        if(mValue2 > 0.0f) {
            StopCoroutine( "UpdateBlackhole" );
            StartCoroutine( "UpdateBlackhole" );
		}

        while (m_endUpdate == false)
        {
			m_checkTime += m_owner.fixedDeltaTime;
			if ( m_checkTime >= m_aniCutFrameLength ) {
				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
        if (evt == null)
        {
            Debug.LogError(mCurAni.ToString() + "공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError(mCurAni.ToString() + "Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    private IEnumerator UpdateBlackhole() {
        /*
        Projectile pjt = null;

        List<Projectile> listPjt = m_owner.aniEvent.GetAllProjectile( mCurAni );
        if ( listPjt != null && listPjt.Count > 0 ) {
            pjt = listPjt[0];
        }
        */

        bool startBlackhole = false;
        bool end = false;
        float checkTime = 0.0f;

        while ( !end ) {
            checkTime += m_owner.fixedDeltaTime;
            if ( checkTime >= mValue1 ) {
                end = true;
            }

            if ( !startBlackhole && m_checkTime >= m_aniCutFrameLength ) {
                mListTarget = m_owner.GetEnemyList( true );

                for ( int i = 0; i < mListTarget.Count; i++ ) {
                    Unit target = mListTarget[i];
                    if ( target == null || target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback() || target.curHp <= 0.0f ) {
                        continue;
                    }

                    target.actionSystem.CancelCurrentAction();
                    target.StopStepForward();
                }

                mBlackholePos = m_owner.transform.position + ( m_owner.transform.forward * 2.0f );//pjt.transform.position;
                startBlackhole = true;
            }
            else if ( startBlackhole ) {
                for ( int i = 0; i < mListTarget.Count; i++ ) {
                    Unit target = mListTarget[i];
                    if ( target == null || target.MainCollider == null || target.cmptMovement == null ) {
                        continue;
                    }

                    if ( target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback() || target.curHp <= 0.0f ) {
                        continue;
                    }

                    if ( target.actionSystem && target.actionSystem.IsCurrentAction( eActionCommand.Appear ) ) {
                        continue;
                    }

                    target.StopStepForward();

                    Vector3 v = ( mBlackholePos - target.transform.position ).normalized;
                    v.y = 0.0f;

                    target.cmptMovement.UpdatePosition( v, Mathf.Max( GameInfo.Instance.BattleConfig.BlackholeMinSpeed, target.speed * 1.2f ), false );
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }
}
