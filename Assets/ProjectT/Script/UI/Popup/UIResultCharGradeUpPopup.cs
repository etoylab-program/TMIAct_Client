using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIResultCharGradeUpPopup : FComponent
{
    [Header("CharacterView")]
	public GameObject kCharView;
	public UITexture kCharTex;

    [Header("CharacterInfo")]
	public GameObject kCharInfo;
	public UILabel kCharNameLabel;
	public UISprite kCharGradeSpr;
	public UISprite kCharGradeEff;
    public UISprite SprComplete;
    public UISprite SprComplete2;

    [Header("Button")]
	public UIButton kConfirmBtn;


	private readonly int mOriginalDepth = 80;

	private long	m_uID				= 0;
    private Vector3	m_originalEffPos	= Vector3.zero;
    private bool	mbFirstAwaken		= false;
	private UIPanel	mPanel				= null;
    private Action  mConfirmAction      = null;

	public override void Awake() {
		base.Awake();

		m_originalEffPos = kCharGradeEff.transform.localPosition;
	}

	public override void OnEnable() {
		if ( mPanel == null ) {
			mPanel = GetComponent<UIPanel>();
		}

		InitComponent();
		base.OnEnable();
	}

	public override void InitComponent()
	{
        m_uID = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
        if(m_uID > 0)
        {
            SetCharacter();
        }

        //  연출이 들어가야하는 곳

        SetActive(kCharView, true);
        SetActive(kCharInfo, true);
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}

    public override void OnDisable()
    {
        base.OnDisable();

        mConfirmAction = null;
    }

    public void OnClick_ConfirmBtn()
	{
        DirectorUIManager.Instance.StopCharGradeUp();

        UICharInfoPanel charPanel = LobbyUIManager.Instance.GetActiveUI<UICharInfoPanel>("CharInfoPanel");
        if (charPanel && mbFirstAwaken)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1630));
        }

		mPanel.depth = mOriginalDepth;

        mConfirmAction?.Invoke();
    }

    public void SetConfirmAction(Action confirmAction)
    {
        mConfirmAction = confirmAction;
    }

    private void SetCharacter()
    {
        CharData charData = GameInfo.Instance.GetCharData(m_uID);
        kCharNameLabel.textlocalize = FLocalizeString.Instance.GetText(charData.TableData.Name);

        //  등급
        {
            kCharGradeSpr.spriteName = string.Format("grade_{0}", charData.Grade.ToString("D2"));  //"grade_0" + charData.Grade.ToString();
            kCharGradeSpr.MakePixelPerfect();

            bool bAwaken = charData.Grade >= (GameInfo.Instance.GameConfig.CharStartAwakenGrade + 1);
            int n = 0;
            mbFirstAwaken = false;

            if (bAwaken)
            {
                if(charData.Grade == GameInfo.Instance.GameConfig.CharWPStartGrade)
                {
                    mbFirstAwaken = true;
                }

                n = GameInfo.Instance.GameConfig.CharStartAwakenGrade;

                SprComplete.spriteName = "Txt_CharaAscended";
                SprComplete2.spriteName = "Txt_CharaAscended";
            }
            else
            {
                SprComplete.spriteName = "Txt_CharaGrageup";
                SprComplete2.spriteName = "Txt_CharaGrageup";
            }

            SprComplete.MakePixelPerfect();
            SprComplete2.MakePixelPerfect();

            Vector3 effPos = m_originalEffPos;
            effPos.x += (((charData.Grade - 1) - n) * 26.0f);
            kCharGradeEff.transform.localPosition = effPos;

            if (bAwaken)
            {
                kCharGradeEff.color = new Color(1.0f, 0.0f, 0.66f);
            }
            else
            {
                kCharGradeEff.color = new Color(0.68f, 0.51f, 0.0f);
            }
        }
    }

    private void SetActive(GameObject go, bool isActive)
    {
        if (go != null)
            go.SetActive(isActive);
    }

    public override bool IsBackButton()
    {
        return false;
    }
}
