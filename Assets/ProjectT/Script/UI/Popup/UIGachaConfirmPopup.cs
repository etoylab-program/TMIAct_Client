using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGachaConfirmPopup : FComponent
{
    public UISprite kGradeSpr;
	public UILabel kNameBGLabel;
	public UILabel kNameLabel;
	public GameObject kNew;

    private RewardData _rewarddata;
    private string itemName;
    private Texture texGrade;


	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        object index = UIValue.Instance.GetValue(UIValue.EParamType.GachaRewardIndex, true);
        if (index == null)
        {
            kNew.SetActive(false);
            kGradeSpr.gameObject.SetActive(false);
            kNameBGLabel.SetActive(false);
            kNameLabel.SetActive(false);
            return;
        }
            

        _rewarddata = GameInfo.Instance.RewardList[(int)index];

        if (_rewarddata == null)
            return;

        int grade = (int)eGRADE.GRADE_NONE;
        int maxGrade = (int)eGRADE.GRADE_UR;

        kGradeSpr.gameObject.SetActive(true);
        if (_rewarddata.Type == (int)eREWARDTYPE.CARD)
        {
            CardData data = GameInfo.Instance.GetCardData(_rewarddata.UID);
            grade = data.TableData.Grade;
            maxGrade = (int)eGRADE.GRADE_UR;
            itemName = FLocalizeString.Instance.GetText(data.TableData.Name);
            kGradeSpr.spriteName = "itemgrade_L_" + grade.ToString();
        }
        else if (_rewarddata.Type == (int)eREWARDTYPE.WEAPON)
        {
            WeaponData data = GameInfo.Instance.GetWeaponData(_rewarddata.UID);
            grade = data.TableData.Grade;
            maxGrade = (int)eGRADE.GRADE_UR;
            itemName = FLocalizeString.Instance.GetText(data.TableData.Name);
            kGradeSpr.spriteName = "itemgrade_L_" + grade.ToString();
        }
        else if (_rewarddata.Type == (int)eREWARDTYPE.GEM)
        {
            GemData data = GameInfo.Instance.GetGemData(_rewarddata.UID);
            grade = data.TableData.Grade;
            maxGrade = (int)eGRADE.GRADE_SR;
            itemName = FLocalizeString.Instance.GetText(data.TableData.Name);
            kGradeSpr.gameObject.SetActive(false);
        }
        else if(_rewarddata.Type == (int)eREWARDTYPE.ITEM)
        {
            ItemData data = GameInfo.Instance.GetItemData(_rewarddata.UID);
            grade = data.TableData.Grade;
            maxGrade = (int)eGRADE.GRADE_SR;
            itemName = FLocalizeString.Instance.GetText(data.TableData.Name);
            kGradeSpr.gameObject.SetActive(false);
        }

        kNew.SetActive(_rewarddata.bNew);

        kNameBGLabel.SetActive(true);
        kNameLabel.SetActive(true);

        if (grade >= maxGrade)
            UIAni.Play(_aninamelist[1]);
        else
            UIAni.Play(_aninamelist[0]);

        AppMgr.Instance.CustomInput.ShowCursor(true);
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
        if (_rewarddata == null)
            return;

        kNameBGLabel.textlocalize = itemName;
        kNameLabel.textlocalize = itemName;
    }

    public void OnClick_SkipBtn()
    {
        DirectorUIManager.Instance.SkipGacha();
    }

    public override bool IsBackButton()
    {
        return false;
    }
}
