
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ActionHit : ActionHitBase
{
    public enum eState
    {
        None = -1,
        Normal,
        Float,
        Down,
        KnockBack,
        Stun,
        StandUp,
        Die,
        Abnormal,
    }


    public eState State { get; private set; }

    private List<ActionHitBase> mListHitAction      = new List<ActionHitBase>();
    private byte                mCurHitStateIndex   = 0;
    private List<eState>        mListNextState      = new List<eState>();
    private bool                mSkipUpdate         = false;

    private HitNormal       mHitNormal      = null;
    private HitFloat        mHitFloat       = null;
    private HitDown         mHitDown        = null;
    private HitKnockBack    mHitKnockBack   = null;
    private HitStun         mHitStun        = null;
    private HitStandUp      mHitStandUp     = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Hit;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.Defence;

        mHitNormal = gameObject.AddComponent<HitNormal>();
        mHitFloat = gameObject.AddComponent<HitFloat>();
        mHitDown = gameObject.AddComponent<HitDown>();
        mHitKnockBack = gameObject.AddComponent<HitKnockBack>();
        mHitStun = gameObject.AddComponent<HitStun>();
        mHitStandUp = gameObject.AddComponent<HitStandUp>();

        // eState 순서대로 Add해야함
        mListHitAction.Add(mHitNormal);
        mListHitAction.Add(mHitFloat);
        mListHitAction.Add(mHitDown);
        mListHitAction.Add(mHitKnockBack);
        mListHitAction.Add(mHitStun);
        mListHitAction.Add(mHitStandUp);

        for (int i = 0; i < mListHitAction.Count; i++)
        {
            mListHitAction[i].Init(tableId, listAddCharSkillParam);
            mListHitAction[i].Parent = this;
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mSkipUpdate = false;
        mListNextState.Clear();

        if (m_owner.curHp > 0.0f)
        {
            m_owner.LockDie(false);

            if (m_hitParam.attackerBehaviour == eBehaviour.UpperAttack)
            {
                if (!m_owner.IsImmuneFloat())
                {
                    m_owner.LockDie(true);
                    State = eState.Float;

                    mListNextState.Add(eState.StandUp);
                }
                else
                {
                    SetNextAction(eActionCommand.Immune, m_hitParam);
                    mSkipUpdate = true;
                }
            }
            else if (m_hitParam.attackerBehaviour == eBehaviour.DownAttack)
            {
                if (!m_owner.IsImmuneDown())
                {
                    State = eState.Down;
                    mListNextState.Add(eState.StandUp);
                }
                else
                {
                    SetNextAction(eActionCommand.Immune, m_hitParam);
                    mSkipUpdate = true;
                }
            }
            else if (m_hitParam.attackerBehaviour == eBehaviour.KnockBackAttack)
            {
                if (!m_owner.IsImmuneKnockback())
                {
                    State = eState.KnockBack;

                    if (m_owner.GetKnockBackType() == Unit.eKnockBackType.KnockDown)
                        mListNextState.Add(eState.StandUp);
                }
                else
                {
                    SetNextAction(eActionCommand.Immune, m_hitParam);
                    mSkipUpdate = true;
                }
            }
            else if (m_hitParam.attackerBehaviour == eBehaviour.StunAttack || m_hitParam.attackerBehaviour == eBehaviour.GroggyAttack)
            {
                State = eState.Stun;
            }
            else
            {
                State = eState.Normal;
            }

            if (!mSkipUpdate)
            {
                mCurHitStateIndex = (byte)State;
                mListHitAction[mCurHitStateIndex].OnStart(m_hitParam);
            }
        }
        else
            SetNextAction(eActionCommand.Die, new ActionParamDie(ActionParamDie.eState.Normal, m_hitParam.attacker));

        if (m_owner.child)
        {
            m_owner.child.actionSystem.CancelCurrentAction();
            m_owner.child.CommandAction(eActionCommand.Hit, param);
        }
    }

    public override IEnumerator UpdateAction()
    {
        if (mSkipUpdate || m_owner.curHp <= 0.0f)
            yield break;

        yield return mListHitAction[mCurHitStateIndex].UpdateAction();

        for(int i = 0; i < mListNextState.Count; i++)
        {
            byte nextStateIndex = (byte)mListNextState[i];
            if ((eState)nextStateIndex == eState.StandUp && m_owner.curHp <= 0.0f)
            {
                SetNextAction(eActionCommand.Die, new ActionParamDie(ActionParamDie.eState.Down, m_hitParam.attacker));
                break;
            }
            else
            {
                mListHitAction[mCurHitStateIndex].OnEnd();

                mCurHitStateIndex = (byte)mListNextState[i];
                State = (eState)mCurHitStateIndex;

                mListHitAction[mCurHitStateIndex].OnStart(m_hitParam);
                yield return mListHitAction[mCurHitStateIndex].UpdateAction();
            }
        }

        while (m_owner.aniEvent.isPauseFrame)
            yield return null;
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        mListHitAction[mCurHitStateIndex].OnUpdating(param);

        if (m_owner.curHp <= 0.0f && !m_owner.isLockDie)
        {
            m_owner.actionSystem.CancelCurrentAction();
            m_owner.CommandAction(eActionCommand.Die, null);
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();

        mListHitAction[mCurHitStateIndex].OnCancel();
        State = eState.None;

        //World.Instance.RemoveHitTarget(m_owner);
    }

	public override void OnEnd() {
		base.OnEnd();
		State = eState.None;

		m_owner.LockDie( false );
		mListHitAction[mCurHitStateIndex].OnEnd();

		if ( mCurHitStateIndex != (int)eState.Stun ) { // 스턴은 OnStart에서 RemoveHitTarget을 함
			EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_REMOVE_HIT_TARGET, m_owner );
		}
	}

	public bool IsDownState() // 다운상태는 HitFloat에서 애니메이션이 DownIdle이나 DownHit일 경우에만 해당. 기존에 있던 HitDown은 사라짐
    {
        if (State != eState.Float)
            return false;

        HitFloat hitFloat = mListHitAction[mCurHitStateIndex] as HitFloat;
        if (!hitFloat)
            return false;

        if (hitFloat.state != HitFloat.eState.DownIdle)
            return false;

        return true;
    }

	public bool IsFloatState() {
        if ( State != eState.Float ) {
            return false;
        }

		HitFloat hitFloat = mListHitAction[mCurHitStateIndex] as HitFloat;
        if ( !hitFloat ) {
            return false;
        }

		return true;
	}

	public HitStun GetCurrentStunHitOrNull()
    {
        if(State != eState.Stun)
        {
            return null;
        }

        return mListHitAction[mCurHitStateIndex] as HitStun;
    }

    public HitFloat GetCurrentFloatHitOrNull()
    {
        if (State != eState.Float)
        {
            return null;
        }

        return mListHitAction[mCurHitStateIndex] as HitFloat;
    }
}
