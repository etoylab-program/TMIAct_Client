
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIGameContinuePanel : FComponent
{
    public UISprite sprBg;
    public UILabel lbCounting;

    private int m_counting;


	public override void OnEnable() {
		sprBg.width = Screen.width + 1;
		sprBg.height = Screen.height + 1;

		base.OnEnable();

		m_counting = 10;
		lbCounting.textlocalize = string.Format( FLocalizeString.Instance.GetText( 2002 ), m_counting.ToString() );

		StartCoroutine( "UpdateCounting" );
	}

	public void OnBtnOK()
    {
        //World.Instance.OnContinueMission();
        gameObject.SetActive(false);
    }

    public void OnBtnCancel()
    {
        //World.Instance.OnEndMission();
        gameObject.SetActive(false);
    }

    private IEnumerator UpdateCounting()
    {
        while(m_counting > 0)
        {
            yield return new WaitForSeconds(1.0f);

            --m_counting;
            lbCounting.textlocalize = string.Format(FLocalizeString.Instance.GetText(2002), m_counting.ToString());
        }

        OnBtnCancel();
    }
}