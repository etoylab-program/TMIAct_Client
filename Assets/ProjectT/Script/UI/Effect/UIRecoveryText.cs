
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIRecoveryText : MonoBehaviour
{
    public UILabel lbChar;
    public float durationTime_;

	public bool IsHide { get; private set; } = true;
	public Unit Parent { get; private set; } = null;


	private UILabel m_lbCurrent = null;
    private int m_recovery = 0;
    private Color m_color;
	private Vector3 mRand = Vector3.zero;


    private void Start()
    {
        lbChar.gameObject.SetActive(false);
	}

    public void ShowRecovery(Unit parent, bool isPlayer, int recovery)
    {
        if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP && !World.Instance.UIPlay.gameObject.activeSelf)
        {
            return;
        }

        if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP && !World.Instance.UIPVP.gameObject.activeSelf)
        {
            return;
        }

        if(recovery == 0)
        {
            return;
        }

        transform.position = Vector3.zero;
        StopCoroutine("Hide");

        if(m_lbCurrent != null)
            m_lbCurrent.gameObject.SetActive(false);

        Parent = parent;
        m_recovery = recovery;

        if (isPlayer)
            m_lbCurrent = lbChar;
        else
            m_lbCurrent = lbChar;

        m_lbCurrent.text = string.Format("+{0:#,##0}", recovery);

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

    private IEnumerator Hide()
    {
        yield return new WaitForSeconds(durationTime_);
        IsHide = true;

        m_lbCurrent.gameObject.SetActive(false);
        Parent = null;
    }

	private void Update() {
        if ( Parent == null || World.Instance.InGameCamera == null || World.Instance.InGameCamera.MainCamera == null ) {
            return;
        }

		Vector3 pos = Vector3.zero;
		Transform dummy = null;

        if ( Parent.aniEvent ) {
            dummy = Parent.aniEvent.GetBoneByName( "hpbar" );
        }

        if ( dummy ) {
            pos = dummy.transform.position;
        }
        else {
            pos = Parent.transform.position;

            if ( Parent.aniEvent == null || !Parent.aniEvent.IsLyingAni() )
                pos.y += Parent.MainCollider.height * 0.8f;
            else
                pos.y += Parent.MainCollider.height * 0.5f;
        }

		transform.position = Utility.GetUIPosFrom3DPos( World.Instance.InGameCamera.MainCamera, gameObject.layer, pos ) + mRand;
	}
}