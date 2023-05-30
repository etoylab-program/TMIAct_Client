
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionChargeAttack : ActionSelectSkillBase
{
    [Header("[Effect]")]
    public ParticleSystem effStart;
    public ParticleSystem[] effCharge;

    protected int                   m_chargeCount       = 0;
    protected int                   m_beforeChargeCount = 0;

    protected ParticleSystem        m_psStart;
    protected List<ParticleSystem>  m_listPSCharge      = new List<ParticleSystem>();

    protected UnitCollider          mTargetCollider     = null;
    protected List<Unit>            mListTarget         = new List<Unit>();
    protected AnimationClip         mCameraAniIn        = null;
    protected AnimationClip         mCameraAniDoing     = null;
    protected AnimationClip         mCameraAniOut       = null;

    protected bool                  mbStartAttack       = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.ChargingAttack;

        conditionActionCommand = new eActionCommand[2];
        conditionActionCommand[0] = eActionCommand.Idle;
        conditionActionCommand[1] = eActionCommand.MoveByDirection;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        superArmor = Unit.eSuperArmor.Lv1;

        mCameraAniIn = ResourceMgr.Instance.LoadFromAssetBundle("animation_char", "Animation_Char/Camera/ani_camera_character_chargeattack_in.anim") as AnimationClip;
        mCameraAniDoing = ResourceMgr.Instance.LoadFromAssetBundle("animation_char", "Animation_Char/Camera/ani_camera_character_chargeattack_middle.anim") as AnimationClip;
        mCameraAniOut = ResourceMgr.Instance.LoadFromAssetBundle("animation_char", "Animation_Char/Camera/ani_camera_character_chargeattack_out.anim") as AnimationClip;

        SkipBOExecuteOnStart = true; // 차지 어택은 StartSkill 배틀옵션을 OnStart에서 안보내고 차지 공격 시에 보내준다.
        ExplicitStartCoolTime = true;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        m_chargeCount = m_beforeChargeCount = 0;
        mbStartAttack = false;
    }

    private IEnumerator PlayDoingCameraAni(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!m_endUpdate && World.Instance.InGameCamera.Mode == InGameCamera.EMode.DEFAULT)
        {
            World.Instance.InGameCamera.PlayAnimation(mCameraAniDoing, null);
        }
    }

    protected virtual void PlayEffCharge(int index)
    {
        for(int i = 0; i < m_listPSCharge.Count; i++)
        {
            if (i == index)
                m_listPSCharge[i].gameObject.SetActive(true);
            else
                m_listPSCharge[i].gameObject.SetActive(false);
        }
    }

    protected virtual void StopAllEffCharge()
    {
		if (m_psStart)
		{
			m_psStart.gameObject.SetActive(false);
		}

		for (int i = 0; i < m_listPSCharge.Count; i++)
		{
			m_listPSCharge[i].gameObject.SetActive(false);
		}
    }

    protected virtual void StartChargeAttack()
    {
        /*if (!m_endUpdate && World.Instance.InGameCamera.Mode == InGameCamera.EMode.DEFAULT)
        {
            World.Instance.InGameCamera.PlayAnimation(mCameraAniOut, null);
        }*/

        SkipConditionCheck = true;
        mbStartAttack = true;
        StartCoolTime();

        if (m_data != null && SkipBOExecuteOnStart)
        {
            ExecuteStartSkillBO();
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        StopAllEffCharge();
        mbStartAttack = false;
        SkipConditionCheck = false;
    }

    public override void OnCancel()
    {
        base.OnCancel();

        StopAllEffCharge();
        mbStartAttack = false;
        SkipConditionCheck = false;

        m_chargeCount = 0;
        m_endUpdate = true;
    }
}
