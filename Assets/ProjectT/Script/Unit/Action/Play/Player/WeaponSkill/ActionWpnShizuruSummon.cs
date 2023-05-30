
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionWpnShizuruSummon : ActionWeaponSkillBase
{
    private int[]               mArrMinionTableId   = new int[] { 21, 21, 21 };
    private List<PlayerMinion>  mListMinion         = new List<PlayerMinion>();
    private PlayerMinion        mActiveMinion       = null;
    private Coroutine           mCr                 = null;
    private ActionParamFromBO   mParamFromBO        = null;
    private bool                mbCast              = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.WpnShizuruSummon;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        superArmor = Unit.eSuperArmor.Invincible;
        mCurAni = eAnimation.XmasSword;

        mOwnerPlayer = m_owner as Player;

        for (int i = 0; i < mArrMinionTableId.Length; i++)
        {
            PlayerMinion minion = GameSupport.CreatePlayerMinion(mArrMinionTableId[i], mOwnerPlayer);
            mListMinion.Add(minion);
        }

        mActiveMinion = null;
        mCr = null;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParamFromBO = param as ActionParamFromBO;

        m_aniLength = m_owner.PlayAni(mCurAni);
        m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();

        mbCast = false;

        UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
        if (targetCollider)
        {
            m_owner.LookAtTarget(targetCollider.GetCenterPos());
            m_owner.SetMainTarget(targetCollider.Owner);
        }
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;

            if (m_checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }
            else if(!mbCast && m_checkTime >= m_aniCutFrameLength)
            {
                mbCast = true;
                StartSummon();

                if (mParamFromBO.battleOptionData.addCallTiming == BattleOption.eBOAddCallTiming.OnSend)
                {
                    if (mParamFromBO.battleOptionData.timingType == BattleOption.eBOTimingType.Use &&
                        mParamFromBO.battleOptionData.dataOnEndCall.conditionType == BattleOption.eBOConditionType.ComboCountAsValue)
                    {
                        mParamFromBO.battleOptionData.dataOnEndCall.evt.value = mParamFromBO.battleOptionData.evt.value;
                    }

                    EffectManager.Instance.Play(m_owner, mParamFromBO.battleOptionData.dataOnEndCall.startEffId, (EffectManager.eType)mParamFromBO.battleOptionData.dataOnEndCall.effType);

                    mParamFromBO.battleOptionData.dataOnEndCall.useTime = System.DateTime.Now;
                    EventMgr.Instance.SendEvent(mParamFromBO.battleOptionData.dataOnEndCall.evt);

                    Log.Show(mParamFromBO.battleOptionData.dataOnEndCall.evt.battleOptionData.battleOptionSetId + "번 배틀옵션셋 사용 (애드콜)!!!", Log.ColorType.Green);
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }

    private void StartSummon()
    {
        if(mActiveMinion)
        {
            Utility.StopCoroutine(World.Instance, ref mCr);
            mActiveMinion.Deactivate();
        }

        mActiveMinion = mListMinion[Random.Range(0, mListMinion.Count)];
        if (mActiveMinion)
        {
            if (mOwnerPlayer.gameObject.layer == (int)eLayer.Player)
            {
                Utility.SetLayer(mActiveMinion.gameObject, (int)eLayer.PlayerClone, true);
            }
            else
            {
                Utility.SetLayer(mActiveMinion.gameObject, (int)eLayer.EnemyClone, true);
            }

            Vector3 pos = mOwnerPlayer.transform.position + (mOwnerPlayer.transform.forward * 7.0f);
            if(Physics.Raycast(mOwnerPlayer.transform.position, (pos - mOwnerPlayer.transform.position).normalized, out RaycastHit hit,
                               Vector3.Distance(pos, mOwnerPlayer.transform.position), 1 << (int)eLayer.Wall))
            {
                pos = mOwnerPlayer.transform.position - (mOwnerPlayer.transform.forward * 1.5f);
            }

            mActiveMinion.SetInitialPosition(pos, mOwnerPlayer.transform.rotation);
            mActiveMinion.SetMinionAttackPower(mOwnerPlayer.gameObject.layer, mOwnerPlayer.attackPower);
            mActiveMinion.Activate();
            mActiveMinion.StopBT();

            mActiveMinion.PlayAniImmediate(eAnimation.Appear);
            mActiveMinion.StartDissolve(0.5f, true, new Color(0.169f, 0.0f, 0.47f));

            mCr = World.Instance.StartCoroutine(EndSummon(mParamFromBO.battleOptionData.value * (float)eCOUNT.MAX_BO_FUNC_VALUE));
        }
    }

    private IEnumerator EndSummon(float duration)
    {
        mActiveMinion.ResetBT();

        bool end = false;
        float checkTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!end)
        {
            checkTime += Time.fixedDeltaTime;
            if (mOwnerPlayer.curHp <= 0.0f || checkTime >= duration || World.Instance.IsEndGame || World.Instance.ProcessingEnd)
            {
                end = true;
            }

            yield return mWaitForFixedUpdate;
        }

        mActiveMinion.StopBT();

        if (!World.Instance.IsEndGame && !World.Instance.ProcessingEnd)
        {
            float aniLength = mActiveMinion.PlayAniImmediate(eAnimation.Die);
            mActiveMinion.StartDissolve(aniLength, false, new Color(0.169f, 0.0f, 0.47f));

            yield return new WaitForSeconds(aniLength);
        }

        mActiveMinion.Deactivate();
    }
}
