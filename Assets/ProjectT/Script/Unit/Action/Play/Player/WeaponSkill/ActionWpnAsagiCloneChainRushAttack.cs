
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionWpnAsagiCloneChainRushAttack : ActionWeaponSkillBase
{
    private int                 mMaxCloneCount  = 2;
    private List<Unit>          ListClone       = new List<Unit>();
    private ActionParamFromBO   mParamFromBO    = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.WpnAsagiCloneChainRushAttack;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.NoUsingSkill;
        extraCondition[1] = eActionCondition.NoUsingQTE;
        extraCondition[2] = eActionCondition.NoUsingUSkill;
        extraCondition[3] = eActionCondition.Grounded;

        IsCommandCloneAttack2 = true;

        ListClone.Clear();
        for (int i = 0; i < mMaxCloneCount; i++)
        {
            Unit clone = CreateCloneOrNull();
            if(clone == null)
            {
                continue;
            }

            ListClone.Add(clone);
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParamFromBO = param as ActionParamFromBO;

        StartCoroutine(ShowClone());
    }

    public void ForceEnd()
    {
        m_endUpdate = true;
    }

    private IEnumerator ShowClone()
    {
        float startInterval = 1.0f;
        float interval = 2.5f;

        for (int i = 0; i < mMaxCloneCount; i++)
        {
            Unit clone = ListClone[i];

            if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
            {
                clone.SetLockAxis(Unit.eAxisType.Z);
            }
            else
            {
                clone.SetLockAxis(Unit.eAxisType.None);
            }

            Vector3 startPos = m_owner.transform.position - (m_owner.transform.right * (startInterval - (i * interval)));

            clone.SetInitialPosition(startPos, m_owner.transform.rotation);
            clone.SetAttackPower(m_owner.attackPower * mParamFromBO.battleOptionData.value);
            clone.Activate();
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < mMaxCloneCount; i++)
        {
            ListClone[i].CommandAction(eActionCommand.CloneHomingAttack, null);
        }

        bool isFirstAtk = true;
        int firstAtkCount = 0;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(m_owner.curHp > 0.0f && !m_endUpdate)
        {
            if((!isFirstAtk && ListClone[0].actionSystem.currentAction == null) || World.Instance.IsEndGame)
            {
                m_endUpdate = true;
            }
            else
            {
                if (isFirstAtk)
                {
                    for (int i = 0; i < mMaxCloneCount; i++)
                    {
                        if (ListClone[i].actionSystem.currentAction == null)
                        {
                            ListClone[i].CommandAction(eActionCommand.RushAttack, null);
                            ++firstAtkCount;
                        }
                    }

                    if(firstAtkCount >= mMaxCloneCount)
                    {
                        isFirstAtk = false;
                    }
                }
                else
                {
                    for (int i = 0; i < mMaxCloneCount; i++)
                    {
                        ActionAsagiChainRushAttack action = ListClone[i].actionSystem.GetCurrentAction<ActionAsagiChainRushAttack>();
                        if (action)
                        {
                            action.OnUpdating(null);
                        }
                    }
                }
            }

            yield return mWaitForFixedUpdate;
        }

        for (int i = 0; i < mMaxCloneCount; i++)
        {
            ListClone[i].Deactivate();
        }
    }

    private Unit CreateCloneOrNull()
    {
        GameTable.Character.Param param = GameInfo.Instance.GetCharacterData(1);
        if (param == null)
        {
            return null;
        }

        string folder = Utility.GetFolderFromPath(param.Model);
        string name = Utility.GetNameFromPath(param.Model);

        string strModel = string.Format("Unit/{0}/{1}_G/{1}_G.prefab", folder, name);
        Unit clone = ResourceMgr.Instance.CreateFromAssetBundle<Unit>("unit", strModel);

        CharData charData = new CharData();
        charData.TableID = 1;
        charData.TableData = param;
        charData.Grade = 1;
        charData.Level = 1;

        GameTable.Costume.Param paramCostume = GameInfo.Instance.GameTable.FindCostume(x => x.CharacterID == 1 && x.PreVisible != (int)eCOUNT.NONE);
        charData.EquipCostumeID = paramCostume.ID;
        charData.CostumeColor = 0;

        GameTable.Weapon.Param paramWeapon = GameInfo.Instance.GameTable.FindWeapon(1001);
        WeaponData weaponData = new WeaponData();
        weaponData.TableID = paramWeapon.ID;
        weaponData.TableData = paramWeapon;
        weaponData.Wake = 0;
        weaponData.Level = 1;
        weaponData.SkillLv = 1;

        clone.costumeUnit.InitCostumeChar(charData, true);
        clone.InitClone(m_owner, folder);
        clone.costumeUnit.SetCostume(charData, weaponData, null);
        clone.SetSpeed(param.MoveSpeed);

        clone.aniEvent.GetBones();

        clone.aniEvent.SetShaderColor("_Color", Color.white);
        clone.aniEvent.SetShaderColor("_HColor", Color.white);
        clone.aniEvent.SetShaderColor("_SColor", Color.black);

        if (FSaveData.Instance.Graphic >= 2)
        {
            clone.aniEvent.SetShaderColor("_RimColor", new Color(1.0f, 1.0f, 1.0f, 0.6f));
            clone.aniEvent.SetShaderFloat("_RimMin", 0.636f);

            clone.aniEvent.SetShaderColor("_OutlineColor", Color.white);
            clone.aniEvent.SetShaderFloat("_Outline", 0.5f);
        }

        Utility.SetLayer(clone.gameObject, (int)eLayer.PlayerClone, true);
        clone.AddCloneDefaultAction();

        ActionCloneHomingAttack actionCloneHomingAtk = clone.gameObject.AddComponent<ActionCloneHomingAttack>();
        if(actionCloneHomingAtk)
        {
            clone.actionSystem.AddAction(actionCloneHomingAtk, 0, null);
        }

        ActionAsagiChainRushAttack actionChainRushAtk = clone.gameObject.AddComponent<ActionAsagiChainRushAttack>();
        if (actionChainRushAtk)
        {
            actionChainRushAtk.SkipShowNames = true;
            actionChainRushAtk.SkipFirstAni = true;
            actionChainRushAtk.AddExtraHit(4);

            clone.actionSystem.AddAction(actionChainRushAtk, 0, null);
        }

        return clone;
    }
}
