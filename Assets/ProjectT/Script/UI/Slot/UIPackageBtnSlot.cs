using UnityEngine;
using System.Collections;

public class UIPackageBtnSlot : FSlot 
{
    public GameObject kSelObj;
    public UILabel kSelLabel;
    public GameObject kNonSelObj;
    public UILabel kNonSelLabel;
    public UIBannerSlot kBennerSlot;

    private int _index;
    private UIPackagePopup.PackageBtnData _packageBtnData;

    private void Awake()
    {
    }

    public void UpdateSlot(int index, int selIndex, UIPackagePopup.ePackageCategory selCategory, UIPackagePopup.PackageBtnData packageBtnData) 	//Fill parameter if you need
	{
        _index = index;
        _packageBtnData = packageBtnData;
        if (_packageBtnData.kBtnType == UIPackagePopup.ePackageBtnType.CategoryBtn)
        {
            kBennerSlot.gameObject.SetActive(false);
            SetCategoryBtnText();
            kSelObj.SetActive(_packageBtnData.kCategoryType == selCategory);
            kNonSelObj.SetActive(_packageBtnData.kCategoryType != selCategory);
        }
        else if (_packageBtnData.kBtnType == UIPackagePopup.ePackageBtnType.PackageItem)
        {
            kBennerSlot.gameObject.SetActive(true);
            kSelObj.SetActive(false);
            kNonSelObj.SetActive(false);
            kBennerSlot.UpdateSlot(UIBannerSlot.ePosType.Package, _index, selIndex, _packageBtnData.kPackageItem);
        }
        //kSelObj.SetActive(_index == selIndex);
        //kNonSelObj.SetActive(_index != selIndex);
    }

    private void SetCategoryBtnText()
    {
        switch (_packageBtnData.kCategoryType)
        {
            case UIPackagePopup.ePackageCategory.New_Package:
                {
                    kSelLabel.textlocalize = FLocalizeString.Instance.GetText(1581);
                    kNonSelLabel.textlocalize = FLocalizeString.Instance.GetText(1581);
                }
                break;
            case UIPackagePopup.ePackageCategory.Char_Package:
                {
                    kSelLabel.textlocalize = FLocalizeString.Instance.GetText(1582);
                    kNonSelLabel.textlocalize = FLocalizeString.Instance.GetText(1582);
                }
                break;
            case UIPackagePopup.ePackageCategory.Time_Package:
                {
                    kSelLabel.textlocalize = FLocalizeString.Instance.GetText(1583);
                    kNonSelLabel.textlocalize = FLocalizeString.Instance.GetText(1583);
                }
                break;
        }
    }
 
	public void OnClick_Slot()
	{
        if (this.ParentGO == null)
            return;

        if (this.ParentGO.GetComponent<UIPackagePopup>() != null)
        {
            UIPackagePopup packagePopup = this.ParentGO.GetComponent<UIPackagePopup>();
            packagePopup.OnClick_PackbtnList(_index, _packageBtnData);
        }
	}
}
