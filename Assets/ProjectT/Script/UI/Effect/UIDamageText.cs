
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIDamageText : MonoBehaviour
{
    public UILabel normalDamage;
    public UILabel criticalDamage;
    public UILabel charDamage;
    public float durationTime_;

	public bool IsHide { get; private set; } = true;
	public Unit Parent { get; private set; } = null;

    private UILabel m_lbCurrent = null;
    private int m_damage = 0;
    private Color m_color;
    private Vector3 mRand = Vector3.zero;


    private void Start()
    {
        normalDamage.gameObject.SetActive(false);
        criticalDamage.gameObject.SetActive(false);
        charDamage.gameObject.SetActive(false);
    }

    public void ShowDamage(Unit parent, bool isPlayer, bool critical, int damage)
    {
        if(World.Instance.StageType != eSTAGETYPE.STAGE_PVP && !World.Instance.UIPlay.gameObject.activeSelf)
        {
            return;
        }

        if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP && !World.Instance.UIPVP.gameObject.activeSelf)
        {
            return;
        }

        if (damage == 0)
            return;

        transform.position = Vector3.zero;
        StopCoroutine("Hide");

        if(m_lbCurrent != null)
            m_lbCurrent.gameObject.SetActive(false);

        Parent = parent;
        m_damage = damage;

        if (critical == true)
            m_lbCurrent = criticalDamage;
        else if (isPlayer == true)
            m_lbCurrent = charDamage;
        else
            m_lbCurrent = normalDamage;

        m_lbCurrent.text = string.Format("{0:#,##0}", damage);

        GameSupport.TweenerPlay(m_lbCurrent.gameObject);

        m_color = m_lbCurrent.color;
        m_color.a = 1.0f;
        m_lbCurrent.color = m_color;

        int x = Random.Range(-10, 10);
        int y = Random.Range(-17, 0);
        mRand = new Vector3((float)x / 100.0f, (float)y / 100.0f, 0.0f);

        IsHide = false;
        StartCoroutine("Hide");
    }

    public void HideDamage()
    {
        StopCoroutine("Hide");

        IsHide = true;
        Parent = null;

        if (m_lbCurrent)
        {
            m_lbCurrent.gameObject.SetActive(false);
        }
    }

    public void SetParent( Unit parent ) {
        Parent = parent;
        HideDamage();
    }

    private IEnumerator Hide()
    {
        yield return new WaitForSeconds(durationTime_);
        IsHide = true;

        m_lbCurrent.gameObject.SetActive(false);
        Parent = null;
    }

    private void Update()
    {
        if (Parent == null)
        {
            return;
        }

        Vector3 pos = Vector3.zero;
        Transform dummy = Parent.aniEvent ? Parent.aniEvent.GetBoneByName("hpbar") : null;
        if (dummy)
            pos = dummy.transform.position;
        else
        {
            pos = Parent.transform.position;
            if (Parent.aniEvent && Parent.aniEvent.IsLyingAni() == false)
                pos.y += Parent.MainCollider.height * 0.8f;
            else
                pos.y += Parent.MainCollider.height * 0.5f;
        }

        transform.position = Utility.GetUIPosFrom3DPos(World.Instance.InGameCamera.MainCamera, gameObject.layer, pos) + mRand;
    }
}