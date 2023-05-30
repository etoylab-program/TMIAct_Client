using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResultCardGradeUpPopup : FComponent
{
    [Header("CardInfo")]
    public GameObject kCardInfo;
    public UISprite kCardWakeSpr;
    public GameObject kCardWakeEff;
    public UISprite kCardGradeSpr;
    public UILabel kCardNameLabel;
    public UILabel kCardLevelLabel;

    [Header("CardStatus")]
    public GameObject kMainSkill;
    public UILabel kMainSkillNameLabel;
    public UILabel kMainSkillLevelLabel;
    public UILabel kMainSkillTimeLabel;
    public UILabel kMainSkillDesceLabel;

    [Header("Button")]
    public UIButton kConfirmBtn;

    private CardData _carddata = null;
    private Vector3 m_originalEffPos;

    private bool m_isPassibleBack = false;

	public override void Awake() {
		base.Awake();

		m_originalEffPos = kCardWakeEff.transform.localPosition;
	}

	public override void OnEnable()
    {
		InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CardUID);
        _carddata = GameInfo.Instance.GetCardData(uid);

        kCardInfo.gameObject.SetActive(true);
        kMainSkill.gameObject.SetActive(true);

        
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

      
        kCardGradeSpr.spriteName = "itemgrade_L_" + _carddata.TableData.Grade.ToString();
        //kCardGradeSpr.MakePixelPerfect();

        kCardNameLabel.textlocalize = FLocalizeString.Instance.GetText(_carddata.TableData.Name);
        kCardLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _carddata.Level, GameSupport.GetMaxLevelCard(_carddata));

        //  등급 관련
        kCardWakeSpr.spriteName = "itemwake_0" + (_carddata.Wake - 1).ToString();

        Vector3 effPos = m_originalEffPos;
        effPos.y -= ((_carddata.Wake - 1) * 22f);
        kCardWakeEff.transform.localPosition = effPos;

        //  스킬 관련
        if (_carddata.TableData.MainSkillEffectName > 0)
        {
            kMainSkill.SetActive(true);
            kMainSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(_carddata.TableData.MainSkillEffectName);
            kMainSkillDesceLabel.textlocalize = GameSupport.GetCardMainSkillDesc(_carddata.TableData, _carddata.Wake);
            if (_carddata.Wake == 0)
                kMainSkillLevelLabel.textlocalize = "";
            else
                kMainSkillLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_NOW_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PLUS_TEXT), _carddata.Wake));
            kMainSkillLevelLabel.transform.localPosition = new Vector3(kMainSkillNameLabel.transform.localPosition.x + kMainSkillNameLabel.printedSize.x + 10, kMainSkillNameLabel.transform.localPosition.y, 0);

            if (_carddata.TableData.CoolTime == 0)
            {
                kMainSkillTimeLabel.gameObject.SetActive(false);
            }
            else
            {
                kMainSkillTimeLabel.gameObject.SetActive(true);
                kMainSkillTimeLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(263), _carddata.TableData.CoolTime);
            }
        }
        else
            kMainSkill.SetActive(false);
    }

    public void OnClick_ConfirmBtn()
    {
        DirectorUIManager.Instance.StopItemWakeUp();
    }
    public override bool IsBackButton()
    {
        return false;
    }

    public void UIAniEventChangeIcon()
    {
        kCardWakeSpr.spriteName = "itemwake_0" + _carddata.Wake.ToString();

        if (GameSupport.IsMaxWakeCard(_carddata))
            VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.WakeMax, _carddata.TableID);
        else
            VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.WakeUp, _carddata.TableID);
    }
}
