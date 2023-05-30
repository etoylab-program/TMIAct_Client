using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResultWeaponSkillLevelUpPopup : FComponent
{
    [Header("WeaponView")]
    public GameObject kWeaponView;
    public UITexture kWeaponTex;

    [Header("WeaponInfo")]
    public GameObject kWeaponInfo;
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
    public UISprite kSPIconSpr;
    public UILabel kSPLabel;

    [Header("Button")]
    public UIButton kConfirmBtn;

    [Header("Effect")]
    public GameObject kEffectObject;

    private Coroutine _coroutine = null;
    private WeaponData _weapondata = null;

    private bool _isPassibleBack = false;


    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();

        _isPassibleBack = false;
    }

    public override void InitComponent()
    {
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.WeaponUID);
        _weapondata = GameInfo.Instance.GetWeaponData(uid);
        //  연출이 들어가야하는 곳
        _coroutine = StartCoroutine(ShowEffect());

        kWeaponInfo.SetActive(false);
        kSkillView.SetActive(true);

        SoundManager.Instance.PlayUISnd(53);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        kSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(_weapondata.TableData.SkillEffectName);
        kSkillDesceLabel.textlocalize = GameSupport.GetWeaponSkillDesc(_weapondata.TableData, _weapondata.SkillLv);

        kSkillLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.SKILL_LEVEL_TXT_NOW_LV, _weapondata.SkillLv, GameSupport.GetMaxSkillLevelWeapon());
        kSkillLevelLabel.transform.localPosition = new Vector3(kSkillNameLabel.transform.localPosition.x + kSkillNameLabel.printedSize.x + 10, kSkillNameLabel.transform.localPosition.y, 0);

        if (_weapondata.TableData.UseSP <= 0)
        {
            kSPIconSpr.gameObject.SetActive(false);
            kSPLabel.gameObject.SetActive(false);
        }
        else
        {
            kSPIconSpr.gameObject.SetActive(true);
            kSPLabel.gameObject.SetActive(true);
            kSPLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), (_weapondata.TableData.UseSP / 100));
        }
    }


    public void OnClick_ConfirmBtn()
    {
        OnClickClose();
    }

    private IEnumerator ShowEffect()
    {
        if (kEffectObject != null)
        {
            kEffectObject.SetActive(false);
            kEffectObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.2f);

        //RenderTargetWeapon.Instance.SetActiveParentFirst(true);

        yield return new WaitForSeconds(2.0f);

        _isPassibleBack = true;
    }

    public override bool IsBackButton()
    {
        return _isPassibleBack;
    }
}
