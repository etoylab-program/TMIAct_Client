
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;


public class EnemyParts : Enemy
{
    [Header("[Parent Bone]")]
    public string boneName;

    [Header("[Parts Property]")]
    public AudioClip clipBlowUp;
    public float blowUpHp;
    public float blowUpPower = 5.0f;

    private Transform m_parentBone;
    private Part[] m_parts;
    private AimConstraint[] m_aimConsts;
    private List<Quaternion> m_listAimRot = new List<Quaternion>();

    private bool m_activeConsts = false;
    private bool m_blowUp = false;

    public bool hasParts { get { return !m_blowUp; } }


    public override void Init(int tableId, eCharacterType type, string faceAniControllerPath)
    {
        base.Init(tableId, type, faceAniControllerPath);

        m_aimConsts = GetComponentsInChildren<AimConstraint>();
        m_parts = GetComponentsInChildren<Part>();

        if(!m_actionSystem.HasNoAction())
        {
            ActionBase actionBase = m_actionSystem.GetAction(eActionCommand.Hit);
            actionBase.OnStartCallback += BeginHit;
            actionBase.OnEndCallback += EndHit;
        }
    }

    public override void SetParent(Enemy parent)
    {
        base.SetParent(parent);

        if (m_parent)
        {
            m_parentBone = m_parent.aniEvent.GetBoneByName(boneName);
            if (m_parentBone)
            {
                transform.SetParent(m_parentBone);
                Utility.InitTransform(gameObject);
            }
        }
    }

    public override void Activate()
    {
        base.Activate();

        Retarget();
        ActiveConstraints(true);

        InitParts(false);
        m_blowUp = false;
	}

    public override void Deactivate()
    {
        base.Deactivate();

        for (int i = 0; i < m_parts.Length; i++)
            m_parts[i].gameObject.SetActive(false);
    }

    public override void Retarget()
    {
        base.Retarget();

        for (int i = 0; i < m_aimConsts.Length; i++)
        {
            for (int j = 0; j < m_aimConsts[i].sourceCount; j++)
            {
                m_aimConsts[i].RemoveSource(j);
            }
        }

        SetConstraintsSource();
    }

    private void SetConstraintsSource()
    {
        for (int i = 0; i < m_aimConsts.Length; i++)
        {
            ConstraintSource constSrc = new ConstraintSource
            {
                sourceTransform = m_mainTarget == null ? World.Instance.Player.transform : m_mainTarget.transform,
                weight = 1.0f
            };

            m_aimConsts[i].AddSource(constSrc);
        }
    }

    public void ActiveConstraints(bool active)
    {
        m_activeConsts = active;
        m_listAimRot.Clear();

        for (int i = 0; i < m_aimConsts.Length; i++)
        {
            m_aimConsts[i].constraintActive = active;
            m_listAimRot.Add(m_aimConsts[i].transform.rotation);
        }
    }

    public void LockConstraints(bool locked)
    {
        m_listAimRot.Clear();

        for (int i = 0; i < m_aimConsts.Length; i++)
        {
            m_aimConsts[i].locked = locked;

            if (locked)
                m_aimConsts[i].rotationAxis = Axis.None;
            else
                m_aimConsts[i].rotationAxis = Axis.X | Axis.Y | Axis.Z;

            m_listAimRot.Add(m_aimConsts[i].transform.rotation);
        }
    }

    private void BeginHit()
    {
        ActiveConstraints(false);
    }

    private void EndHit()
    {
        ActiveConstraints(true);
    }

    public override void OnDie()
    {
        base.OnDie();
        ActiveConstraints(false);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if(!m_blowUp)
        {
            float curHp = m_curHp;
            float maxHp = m_maxHp;

            if (m_parent)
            {
                curHp = m_parent.curHp;
                maxHp = m_parent.maxHp;
            }

            if((curHp / maxHp) * 100.0f < blowUpHp)
            {
                AddForceToParts(blowUpPower);
                SoundManager.Instance.PlaySnd(SoundManager.eSoundType.Monster, clipBlowUp, FSaveData.Instance.GetSEVolume());

                m_blowUp = true;
            }
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (!m_activeConsts)
        {
            for (int i = 0; i < m_aimConsts.Length; i++)
            {
                m_aimConsts[i].transform.rotation = m_listAimRot[i];
            }
        }
    }

    public void InitParts(bool initChild)
    {
        for (int i = 0; i < m_parts.Length; i++)
        {
            m_parts[i].Init(this);

            if (initChild && m_child)
            {
                EnemyParts enemyParts = m_child as EnemyParts;
                if (enemyParts)
                    enemyParts.InitParts(false);
            }
        }
    }

    public void AddForceToParts(float force)
    {
        for (int i = 0; i < m_parts.Length; i++)
            m_parts[i].AddForce(transform.position, force);
    }

    public void ShowParts()
    {
        m_blowUp = true;

        for (int i = 0; i < m_parts.Length; i++)
            m_parts[i].gameObject.SetActive(true);
    }

    public void HideParts()
    {
        m_blowUp = true;

        for (int i = 0; i < m_parts.Length; i++)
            m_parts[i].gameObject.SetActive(false);
    }
}
