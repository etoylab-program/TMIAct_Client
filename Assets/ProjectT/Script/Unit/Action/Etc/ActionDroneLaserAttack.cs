
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionDroneLaserAttack : ActionBase
{
    private DroneUnit           mOwnerDrone     = null;
    
    private List<LineRenderer>  mListLine       = new List<LineRenderer>();
    private ParticleSystem      mEffStartLaser  = null;
    private ParticleSystem      mEffLaserHit    = null;
    private bool                mbActiveEffs    = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.DroneLaserAttack;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.DroneFollowOwner;

        GameObject gObj = ResourceMgr.Instance.CreateFromAssetBundle("effect", "Effect/Supporter/prf_fx_supporter_skill_59_lazer.prefab");
        mListLine.AddRange(gObj.GetComponentsInChildren<LineRenderer>(true));

        mEffStartLaser = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>("effect", "Effect/Supporter/prf_fx_supporter_skill_59_shoot.prefab");
        mEffLaserHit = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>("effect", "Effect/Supporter/prf_fx_supporter_skill_59_hit.prefab");

        ActiveEffects(false);

        mOwnerDrone = m_owner as DroneUnit;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        if(mOwnerDrone.mainTarget == null)
        {
            m_endUpdate = true;
        }
        else
        {
            SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, "DroneAttack");
            ActiveEffects(true);

            mEffStartLaser.transform.position = Vector3.zero;
            mEffStartLaser.transform.rotation = Quaternion.identity;

            for (int i = 0; i < mListLine.Count; i++)
            {
                mListLine[i].transform.position = Vector3.zero;
                mListLine[i].transform.rotation = Quaternion.identity;
            }

            //mOwnerDrone.SetTweenToPos(new Vector3(0.0f, 0.1f, 0.0f));
        }
    }

    public override IEnumerator UpdateAction()
    {
        AniEvent.sEvent evt = new AniEvent.sEvent();
        evt.behaviour = eBehaviour.Attack;
        evt.hitEffectId = 0;
        evt.hitDir = eHitDirection.None;

        evt.isRangeAttack = true;

        float tick = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate && isPlaying)
        {
            if (mOwnerDrone.mainTarget && (!mOwnerDrone.mainTarget.IsActivate() || mOwnerDrone.mainTarget.curHp <= 0.0f))
            {
                Unit target = World.Instance.EnemyMgr.GetNearestTarget(mOwnerDrone.Owner, true);
                m_owner.SetMainTarget(target);
            }

            if (mOwnerDrone.mainTarget == null)
            {
                m_endUpdate = true;
            }
            else
            {
                bool isPlayerInputPause = false;
                if(mOwnerDrone.Owner.Input)
                {
                    isPlayerInputPause = mOwnerDrone.Owner.Input.isPause;
                }

                if (!Director.IsPlaying && !isPlayerInputPause)
                {
                    if (!mbActiveEffs)
                    {
                        ActiveEffects(true);
                    }

                    mOwnerDrone.UpdateMovement(mOwnerDrone.mainTarget);

                    Vector3 targetPos = mOwnerDrone.mainTarget.GetCenterPos();

                    mEffStartLaser.transform.position = mOwnerDrone.LaserProp.transform.position;
                    mEffStartLaser.transform.rotation = mOwnerDrone.LaserProp.transform.rotation;

                    mEffLaserHit.transform.position = targetPos;

                    for (int i = 0; i < mListLine.Count; i++)
                    {
                        mListLine[i].SetPosition(0, mOwnerDrone.LaserProp.transform.position);
                        mListLine[i].SetPosition(1, targetPos);
                    }

                    tick += Time.deltaTime;
                    if (tick >= mOwnerDrone.Tick)
                    {
                        mAtkEvt.SetWithSingleTarget(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, mOwnerDrone, BattleOption.eToExecuteType.Supporter, evt,
                                                    mOwnerDrone.attackPower, eAttackDirection.Skip, false, 0, EffectManager.eType.None,
                                                    mOwnerDrone.mainTarget.MainCollider, 0.0f, true, false, true);

                        EventMgr.Instance.SendEvent(mAtkEvt);
                        tick = 0.0f;
                    }
                }
                else if (mbActiveEffs)
                {
                    ActiveEffects(false);
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();

        ActiveEffects(false);
        mOwnerDrone.SetMainTarget(null);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        End();
    }

    private void End()
    {
        ActiveEffects(false);

        if (mOwnerDrone.IsActivateMesh())
        {
            SetNextAction(eActionCommand.DroneFollowOwner, null);
        }
    }

    private void ActiveEffects(bool play)
    {
        mbActiveEffs = play;

        for (int i = 0; i < mListLine.Count; i++)
        {
            mListLine[i].gameObject.SetActive(play);
        }

        mEffStartLaser.gameObject.SetActive(play);
        mEffLaserHit.gameObject.SetActive(play);
    }
}
