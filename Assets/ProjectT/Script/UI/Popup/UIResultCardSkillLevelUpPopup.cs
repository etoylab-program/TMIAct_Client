using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResultCardSkillLevelUpPopup : FComponent
{
    [Header("CardView")]
    public GameObject kCardView;
    public UITexture kCardTex;
    public UIButton kCardBtn;

    [Header("CardInfo")]
    public GameObject kCardInfo;
    public UISprite kGradeSpr;
    public UILabel kNameLabel;
    public UILabel kExpLabel;
    public UIGaugeUnit kExpGaugeUnit;
    public UILabel kLevelLabel;

    [Header("Skill")]
    public GameObject kSkillView;
    public UILabel kSkillNameLabel;
    public UILabel kSkillLevelLabel;
    public UILabel kSkillDesceLabel;

    [Header("Buttons")]
    public UIButton kConfirmBtn;

    private CardData _carddata = null;


    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CardUID);

        _carddata = GameInfo.Instance.GetCardData(uid);

        //  실제로 보여지는 오브젝트
        kCardView.SetActive(true);
        kCardInfo.SetActive(false);
        kSkillView.SetActive(true);

        SoundManager.Instance.PlayUISnd(5);

        if( GameSupport.IsMaxSkillLevelCard(_carddata) )
            VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.SkillLvMax, _carddata.TableID);
        else
            VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.SkillLvUp, _carddata.TableID);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        kCardTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("card", string.Format("Card/{0}_{1}.png", _carddata.TableData.Icon, GameSupport.GetCardImageNum(_carddata)));

        float fillAmount = GameSupport.GetCardLevelExpGauge(_carddata, _carddata.Level, _carddata.Exp);
        kExpGaugeUnit.InitGaugeUnit(fillAmount);
        kExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (fillAmount * 100.0f)));

        FLocalizeString.SetLabel(kSkillNameLabel, _carddata.TableData.SkillEffectName);
        kSkillDesceLabel.textlocalize = GameSupport.GetCardSubSkillDesc(_carddata.TableData, _carddata.SkillLv);

        kSkillLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.SKILL_LEVEL_TXT_NOW_LV, _carddata.SkillLv, GameSupport.GetMaxSkillLevelCard());
        kSkillLevelLabel.transform.localPosition = new Vector3(kSkillNameLabel.transform.localPosition.x + kSkillNameLabel.printedSize.x + 10, kSkillNameLabel.transform.localPosition.y, 0);
    }

    public void OnClick_CardBtn()
    {

    }

    public void OnClick_ConfirmBtn()
    {
        OnClickClose();
    }
}
