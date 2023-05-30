using UnityEngine;
using System.Collections;

public class UIHowToBeStrongListSlot : FSlot
{
	[SerializeField] private UILabel kNameLabel;
	[SerializeField] private UISprite kNoticeSpr;
	[SerializeField] private UISprite kDisableSpr;
	[SerializeField] private UISprite kEnableSpr;
	[SerializeField] private UILabel kEnableLabel;

	private int _index;
	private GameClientTable.HowToBeStrong.Param Param;
	private CharData _CharData;

	public void UpdateSlot(int index, int selectIndex, string title)
	{
		_index = index;
		
		kNameLabel.SetActive(index != selectIndex);
		kEnableSpr.SetActive(index == selectIndex);
		kEnableLabel.SetActive(kEnableSpr.gameObject.activeSelf);
		
		kNoticeSpr.SetActive(false);
		
		kNameLabel.textlocalize = kEnableLabel.textlocalize = title;
	}
	
	public void UpdateSlot(int index, GameClientTable.HowToBeStrong.Param _param)
	{
		Param = _param;
		
		kNameLabel.textlocalize = FLocalizeString.Instance.GetText(Param.BtnText);
		SetNotice();
	}

	public void UpdateDetailSlot(int index, GameClientTable.HowToBeStrong.Param _param)
    {
        Param = _param;

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(Param.BtnText);

		kDisableSpr.SetActive(false);
		kEnableSpr.SetActive(false);

        UIHowToBeStrongV2Popup popup = ParentGO.GetComponent<UIHowToBeStrongV2Popup>();
        if (popup == null) return;

		if ((int)popup.CurState == Param.Group && popup.CurSelect == Param.Index)
			kEnableSpr.SetActive(true);
		else
			kDisableSpr.SetActive(true);

		SetNotice();
	}
 
	public void OnClick_Slot()
	{
		{
			UIHowToBeStrongV2Popup popup = ParentGO.GetComponent<UIHowToBeStrongV2Popup>();
			if (popup != null)
			{
				popup.SetState((UIHowToBeStrongV2Popup.eSTATE)Param.Group, Param.Index);
			}
		}
		{
			UIFacilityTradeDetailPopup popup = ParentGO.GetComponent<UIFacilityTradeDetailPopup>();
			if (popup != null)
			{
				popup.SelectTabList(_index);
			}
		}
	}

	private void SetNotice()
    {
		kNoticeSpr.SetActive(false);

		long cuid = (long)UIValue.Instance.GetValue(UIValue.EParamType.HowToBeStrongCUID);
		_CharData = GameInfo.Instance.GetCharData(cuid);

		if (_CharData == null) return;

        kNoticeSpr.SetActive(GameSupport.IsHowToBeStrongNotice(_CharData, Param));
    }
}
