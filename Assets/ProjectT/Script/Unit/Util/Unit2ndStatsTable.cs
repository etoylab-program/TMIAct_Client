
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Unit2ndStatsTable
{
    public enum eAddType
    {
        Weapon = 0,
        Skill,
    }

    public enum eSubjectType
    {
        None = 0,

        Player,

        NormalMob,
        EpicMob,
        BossMob,
        AllMob,

        HumanMob,
        MachineMob,
        DevilMob,

        NormalAtk,
        AirAtk,
        UpperAtk,
        ChargeAtk,
        RushAtk,
        USkillAtk,

        Critical,

        HpUp,
        AtkUp,
        DefUp,
        CriUp,

        SkillAtk,
        CoolTime,

        Penetrate,
        Sufferance,
    }

    public enum eBadgeOptType
    {
        None = 0,

        CoinCostdown,
        CoinEffUp,
        CheerAddRate,
        CheerRegenRate,
        CheerEffUp,
        WinRecoveryHP,
        CriResistUp,
        CriDefUp,
        StartCharUp,
        MidCharUp,
        LastCharUp,
        CharHPUp,
        CharATKUp,
        CharDEFUp,
        CharCRIUp,
    }

    public enum eIncreaseType
    {
        None = 0,

        Damage,
        Probability,
        Defence,
        Stat,
    }


    public class sOption
    {
        public eAddType         AddType;
        public eSubjectType     subjectType;
        public eIncreaseType    increaseType;
        public int              value;


        public sOption(eAddType addType, eSubjectType subjectType, eIncreaseType increaseType, int value)
        {
            AddType = addType;
            this.subjectType = subjectType;
            this.increaseType = increaseType;
            this.value = value;
        }
    }

    private List<sOption> m_listStats = new List<sOption>();
    public List<sOption> listStats { get { return m_listStats; } }


    public static eSubjectType GetSubjectType(string subject)
    {
		string[] strs = System.Enum.GetNames(typeof(eSubjectType));
		for(int i = 0; i < strs.Length; ++i)
		{
			if(strs[i].CompareTo(subject) == 0)
			{
				return (eSubjectType)i;
			}
		}
		/*
        foreach(eSubjectType type in (eSubjectType[])System.Enum.GetValues(typeof(eSubjectType)))
        {
            if (type.ToString() == subject)
                return type;
        }
		*/
        return eSubjectType.None;
    }

    public static eIncreaseType GetIncreaseType(string subject)
    {
		string[] strs = System.Enum.GetNames(typeof(eIncreaseType));
		for (int i = 0; i < strs.Length; ++i)
		{
			if (strs[i].CompareTo(subject) == 0)
			{
				return (eIncreaseType)i;
			}
		}
		/*
        foreach (eIncreaseType type in (eIncreaseType[])System.Enum.GetValues(typeof(eIncreaseType)))
        {
            if (type.ToString() == subject)
                return type;
        }
		*/
		return eIncreaseType.None;
    }


    public void Clear()
    {
        m_listStats.Clear();
    }

    public void AddStat(eAddType addType, eSubjectType subjectType, eIncreaseType increaseType, int value)
    {
        sOption option = new sOption(addType, subjectType, increaseType, value);
        m_listStats.Add(option);
    }

    public void RemoveStatByAddType(eAddType addType)
    {
        List<sOption> finds = m_listStats.FindAll(x => x.AddType == addType);
        for(int i = 0; i < finds.Count; i++)
        {
            m_listStats.Remove(finds[i]);
        }
    }

    public bool IsEmpty()
    {
        return m_listStats.Count <= 0;
    }

    public List<sOption> FindStat(eSubjectType subjectType)
    {
        List<sOption> listFind = m_listStats.FindAll(x => x.subjectType == subjectType);
        if (listFind.Count <= 0)
            return null;

        return listFind;
    }

    public List<sOption> FindStat(eIncreaseType increaseType)
    {
        List<sOption> listFind = m_listStats.FindAll(x => x.increaseType == increaseType);
        if (listFind.Count <= 0)
            return null;

        return listFind;
    }

    public List<sOption> FindStat(eSubjectType subjectType, eIncreaseType increaseType)
    {
        List<sOption> listFind = m_listStats.FindAll(x => x.subjectType == subjectType && x.increaseType == increaseType);
        if (listFind.Count <= 0)
            return null;

        return listFind;
    }

    public float CalcAttackPower(Unit attacker, Unit target, float defaultAttackPower)
    {
        float resultAttackPower = defaultAttackPower;

        List<sOption> listDamage = FindStat(eIncreaseType.Damage);
        if (listDamage == null)
            return resultAttackPower;

        for (int i = 0; i < listDamage.Count; i++)
        {
            switch (listDamage[i].subjectType)
            {
                case eSubjectType.Player:
                    if (target as Player == null)
                        continue;
                    break;

                case eSubjectType.NormalMob:
                    if (target.grade != Unit.eGrade.Normal)
                        continue;
                    break;

                case eSubjectType.EpicMob:
                    if (target.grade != Unit.eGrade.Epic)
                        continue;
                    break;

                case eSubjectType.BossMob:
                    if (target.grade != Unit.eGrade.Boss)
                        continue;
                    break;

                case eSubjectType.AllMob:
                    if (target == null)
                        continue;
                    break;

                case eSubjectType.HumanMob:
                    if (target.monType != Unit.eMonType.Human)
                        continue;
                    break;

                case eSubjectType.MachineMob:
                    if (target.monType != Unit.eMonType.Machine)
                        continue;
                    break;

                case eSubjectType.DevilMob:
                    if (target.monType != Unit.eMonType.Devil)
                        continue;
                    break;

                case eSubjectType.NormalAtk:
                    if (attacker.onAtkAniEvt == null || attacker.onAtkAniEvt.behaviour != eBehaviour.Attack || !attacker.isGrounded)
                        continue;
                    break;

                case eSubjectType.AirAtk:
                    if (attacker.onAtkAniEvt == null || attacker.onAtkAniEvt.behaviour != eBehaviour.Attack || attacker.isGrounded)
                        continue;
                    break;

                case eSubjectType.UpperAtk:
                    if (attacker.onAtkAniEvt == null || attacker.onAtkAniEvt.behaviour != eBehaviour.UpperAttack)
                        continue;
                    break;

                case eSubjectType.ChargeAtk:
                    if (attacker.actionSystem.currentAction == null || attacker.actionSystem.currentAction.actionCommand != eActionCommand.ChargingAttack)
                        continue;
                    break;

                case eSubjectType.RushAtk:
                    if (attacker.actionSystem.currentAction == null || attacker.actionSystem.currentAction.actionCommand != eActionCommand.RushAttack)
                        continue;
                    break;

                /*case eSubjectType.USkillAtk:
                    if (!attacker.actionSystem.IsCurrentUSkillAction())
                        continue;
                    break;*/

                default:
                    continue;
            }

            resultAttackPower += (resultAttackPower * listDamage[i].value / (float)eCOUNT.MAX_RATE_VALUE);
        }

        return resultAttackPower;
    }

    public float CalcCriticalRatio( float addCriDmgRatio )
    {
        float ratio = GameInfo.Instance.BattleConfig.CriticalDmgRatio + addCriDmgRatio;

        List<sOption> listFind = FindStat(eSubjectType.Critical, eIncreaseType.Damage);
        if (listFind == null)
            return ratio;

        for(int i = 0; i < listFind.Count; i++)
            ratio += (ratio * listFind[i].value / (float)eCOUNT.MAX_RATE_VALUE);

        return ratio;
    }

    public float CalcCriticalProbability(float criticalProbability)
    {
        float probability = criticalProbability;

        List<sOption> listFind = FindStat(eSubjectType.Critical, eIncreaseType.Probability);
        if (listFind == null)
            return probability;

        for (int i = 0; i < listFind.Count; i++)
            probability += (probability * listFind[i].value / (float)eCOUNT.MAX_RATE_VALUE);

        return probability;
    }

    public float CalcDamageDown(Unit attacker, Unit target, float defaultAttackPower)
    {
        float resultAttackPower = defaultAttackPower;

        List<sOption> listDamage = FindStat(eIncreaseType.Defence);
        if (listDamage == null)
            return resultAttackPower;

        for (int i = 0; i < listDamage.Count; i++)
        {
            switch (listDamage[i].subjectType)
            {
                case eSubjectType.Player:
                    if (attacker as Player == null)
                        continue;
                    break;

                case eSubjectType.NormalMob:
                    if (attacker.grade != Unit.eGrade.Normal)
                        continue;
                    break;

                case eSubjectType.EpicMob:
                    if (attacker.grade != Unit.eGrade.Epic)
                        continue;
                    break;

                case eSubjectType.BossMob:
                    if (attacker.grade != Unit.eGrade.Boss)
                        continue;
                    break;

                case eSubjectType.AllMob:
                    if (attacker == null)
                        continue;
                    break;

                case eSubjectType.HumanMob:
                    if (attacker.monType != Unit.eMonType.Human)
                        continue;
                    break;

                case eSubjectType.MachineMob:
                    if (attacker.monType != Unit.eMonType.Machine)
                        continue;
                    break;

                case eSubjectType.DevilMob:
                    if (attacker.monType != Unit.eMonType.Devil)
                        continue;
                    break;

                default:
                    continue;
            }

            resultAttackPower -= (resultAttackPower * listDamage[i].value / (float)eCOUNT.MAX_RATE_VALUE);
        }

        return resultAttackPower;
    }
}
