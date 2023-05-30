
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionShizuruEvadeChargeAttack : ActionSelectSkillBase
{
    private Unit        mTarget = null;
    private eAnimation  mCurAni = eAnimation.EvadeCharge;


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

        if (mValue1 >= 1.0f)
        {
            mCurAni = eAnimation.EvadeCharge_1;
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

		if ( SetAddAction ) {
			mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Buff;

			if ( World.Instance.ListPlayer.Count > 0 ) {
				for ( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
					if ( AddActionValue2 < Vector3.Distance( World.Instance.ListPlayer[i].GetCenterPos(), m_owner.GetCenterPos() ) ) {
						continue;
					}

					mBuffEvt.Set( m_data.ID, eEventSubject.Self, eEventType.EVENT_BUFF_ATKPOWER_RATE_UP, World.Instance.ListPlayer[i],
								  AddActionValue1, 0.0f, 0.0f, AddActionDuration, 0.0f, mBuffEvt.effId, mBuffEvt.effId2, eBuffIconType.Buff_Atk );

					EventMgr.Instance.SendEvent( mBuffEvt );
				}
			}
			else {
				mBuffEvt.Set( m_data.ID, eEventSubject.Self, eEventType.EVENT_BUFF_ATKPOWER_RATE_UP, m_owner,
							  AddActionValue1, 0.0f, 0.0f, AddActionDuration, 0.0f, mBuffEvt.effId, mBuffEvt.effId2, eBuffIconType.Buff_Atk );

				EventMgr.Instance.SendEvent( mBuffEvt );
			}
		}

        m_aniLength = m_owner.PlayAniImmediate(mCurAni);
    }

    public override IEnumerator UpdateAction()
    {
        float firstAtkEventLength = m_owner.aniEvent.GetFirstAttackEventLength(mCurAni);
        
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if(m_checkTime <= firstAtkEventLength)
            {
                if (FSaveData.Instance.AutoTargetingSkill)
                {
                    mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
                    if (mTarget)
                    {
                        m_owner.LookAtTarget(mTarget.transform.position);
                    }
                }
                else
                {
                    m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
                }
            }
            else if (m_checkTime >= m_aniLength)
            {
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
}
