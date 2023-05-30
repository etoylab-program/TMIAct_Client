
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterShiranuiSummon : ActionSupporterSkillBase
{
    private Unit    mCloneShiranui  = null;
    private float   mDuration       = 0.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterShiranuiSummon;

        IsCommandCloneAttack2 = true;
        CreateCloneShiranui();
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParamFromBO = param as ActionParamFromBO;

        mDuration = mParamFromBO.battleOptionData.duration;
        StartCoroutine(ShowClone(mDuration));
    }

    public void ForceEnd()
    {
        m_checkTime = mDuration;
    }

    private IEnumerator ShowClone(float duration)
    {
        Vector3 startPos = m_owner.transform.position;
        Quaternion startRot = m_owner.transform.rotation;

        float length = EffectManager.Instance.Play(m_owner, 50015, EffectManager.eType.Common);
        yield return new WaitForSeconds(length * 0.3f);

        if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
        {
            mCloneShiranui.SetLockAxis(Unit.eAxisType.Z);
        }
        else
        {
            mCloneShiranui.SetLockAxis(Unit.eAxisType.None);
        }

        mCloneShiranui.SetInitialPosition(startPos, startRot);
        mCloneShiranui.SetAttackPower(m_owner.attackPower * mParamFromBO.battleOptionData.value);
        mCloneShiranui.UseAttack02 = false;

		mCloneShiranui.Activate();
        mCloneShiranui.StartBT();

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(m_owner.curHp > 0.0f)
        {
            m_checkTime += Time.fixedDeltaTime;
            if(m_checkTime >= mDuration || World.Instance.IsEndGame || World.Instance.ProcessingEnd)
            {
                break;
            }

            yield return mWaitForFixedUpdate;
        }

        mCloneShiranui.Deactivate();
        EffectManager.Instance.Play(mCloneShiranui, 50016, EffectManager.eType.Common);
    }

    private void CreateCloneShiranui()
    {
        GameTable.Character.Param param = GameInfo.Instance.GetCharacterData(7);
        if (param == null)
        {
            return;
        }

        string folder = Utility.GetFolderFromPath(param.Model);
        string name = Utility.GetNameFromPath(param.Model);

        string strModel = string.Format("Unit/{0}/{1}_G/{1}_G.prefab", folder, name);
        mCloneShiranui = ResourceMgr.Instance.CreateFromAssetBundle<Unit>("unit", strModel);

        CharData charData = new CharData();
        charData.TableID = 7;
        charData.TableData = param;
        charData.Grade = 1;
        charData.Level = 1;

        GameTable.Costume.Param paramCostume = GameInfo.Instance.GameTable.FindCostume(x => x.CharacterID == 7 && x.PreVisible != (int)eCOUNT.NONE);
        charData.EquipCostumeID = paramCostume.ID;
        charData.CostumeColor = 0;

        GameTable.Weapon.Param paramWeapon = GameInfo.Instance.GameTable.FindWeapon(7002);
        WeaponData weaponData = new WeaponData();
        weaponData.TableID = paramWeapon.ID;
        weaponData.TableData = paramWeapon;
        weaponData.Wake = 0;
        weaponData.Level = 1;
        weaponData.SkillLv = 1;

        mCloneShiranui.costumeUnit.InitCostumeChar(charData, true);
        mCloneShiranui.InitClone(m_owner, folder);
        mCloneShiranui.costumeUnit.SetCostume(charData, weaponData, null);
        mCloneShiranui.SetSpeed(param.MoveSpeed);

        mCloneShiranui.aniEvent.GetBones();

        mCloneShiranui.aniEvent.SetShaderColor("_Color", Color.white);
        mCloneShiranui.aniEvent.SetShaderColor("_HColor", Color.white);
        mCloneShiranui.aniEvent.SetShaderColor("_SColor", Color.black);

        if (FSaveData.Instance.Graphic >= 2)
        {
            mCloneShiranui.aniEvent.SetShaderColor("_RimColor", new Color(1.0f, 1.0f, 1.0f, 0.6f));
            mCloneShiranui.aniEvent.SetShaderFloat("_RimMin", 0.636f);

            mCloneShiranui.aniEvent.SetShaderColor("_OutlineColor", Color.white);
            mCloneShiranui.aniEvent.SetShaderFloat("_Outline", 0.5f);
        }

        Utility.SetLayer(mCloneShiranui.gameObject, (int)eLayer.PlayerClone, true);

        Shader shader = ResourceMgr.Instance.LoadFromAssetBundle("shader", "Shader/effect/fx_shiranui_evasion_skill.shader") as Shader;
        mCloneShiranui.aniEvent.SetShader(shader);

        mCloneShiranui.AddAI("CharacterClone");

        ActionComboAttack actionAtk = mCloneShiranui.gameObject.AddComponent<ActionComboAttack>();
        if (actionAtk)
        {
            actionAtk.attackAnimations = new eAnimation[5];
            actionAtk.attackAnimations[0] = eAnimation.Attack01;
            actionAtk.attackAnimations[1] = eAnimation.Attack02;
            actionAtk.attackAnimations[2] = eAnimation.Attack03;
            actionAtk.attackAnimations[3] = eAnimation.Attack04;
            actionAtk.attackAnimations[4] = eAnimation.Attack05_1;

            mCloneShiranui.actionSystem.AddAction(actionAtk, 0, null);
        }
    }
}
