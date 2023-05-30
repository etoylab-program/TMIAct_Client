
using UnityEngine;
using System.Collections;


public class UIRewardListSlot : FSlot
{
	public UISprite kBGSpr;
	public UITexture kIconTex;
    public UISprite kFrmGradeSpr;
    public UISprite kSelSpr;
	public UISprite kGradeSpr;
	public UILabel kCountLabel;
    public UIGoodsUnit kGoodsUnit;
    public UISprite kInactiveSpr;
    public UISprite kTypeSpr;

	[Header("[Login Bonus]")]
	public UILabel	LbDay;
	public UISprite	SprAttendance;
	public UISprite SprAbsence;
	public UIButton kLbRewardBtn;

	private RewardData _rewarddata;
    private bool _showpopup = false;

	private UIWidget mWidget = null;

	public void UpdateSlot(RewardData rewardData, bool bshowpopup, bool binactive = false) 	//Fill parameter if you need
	{
		LbDay.SetActive(false);
		SprAttendance.SetActive(false);
		SprAbsence.SetActive(false);

		_showpopup = bshowpopup;
        _rewarddata = rewardData;
        if (rewardData != null)
        {
            GameSupport.UpdateSlotByRewardData(ParentGO, this.gameObject, rewardData, kFrmGradeSpr, kGradeSpr, kTypeSpr, kBGSpr, kCountLabel, kIconTex, null, kGoodsUnit);
        }

        if (kInactiveSpr != null)
            kInactiveSpr.gameObject.SetActive(binactive);
    }

	public void UpdateSlot(int index, RewardData data, int attendance, bool direction, int pLoginEventAbsentReward) // attendance (0 : 결석, 1 : 출석, 2 : 대기)
	{
		SprAttendance.SetActive(false);
		SprAbsence.SetActive(false);

		SprAttendance.SetActive(attendance == 1 && !direction);
		SprAbsence.SetActive(attendance == 0);

		kLbRewardBtn.SetActive(SprAbsence.gameObject.activeSelf && (pLoginEventAbsentReward < 0 ? 0 : pLoginEventAbsentReward) != 0);

		_showpopup = true;
		_rewarddata = data;

		GameSupport.UpdateSlotByRewardData(ParentGO, this.gameObject, data, kFrmGradeSpr, kGradeSpr, kTypeSpr, kBGSpr, kCountLabel, kIconTex, null, kGoodsUnit);

		LbDay.SetActive(true);
		LbDay.textlocalize = (index + 1).ToString("D2");
	}

    public void IsInActive(bool binactive)
    {
        if (kInactiveSpr != null)
            kInactiveSpr.gameObject.SetActive(binactive);
    }
 
	public void OnClick_Slot()
	{
        if(ParentGO != null)
        {
	        UIPassRewardListSlot uiPassRewardListSlot = ParentGO.GetComponent<UIPassRewardListSlot>();
            if (uiPassRewardListSlot != null)
            {
	            uiPassRewardListSlot.OnClick_Slot(_rewarddata, this);
                return;
            }
        }

        if (_rewarddata.Type == (int)eREWARDTYPE.GOODS)
            return;
        if (!_showpopup)
            return;

        if(ParentGO != null)
        {
            if (ParentGO.GetComponent<UIGachaGuerrilaRewardSlot>() != null)
            {
                UIGachaGuerrilaRewardSlot uIGachaGuerrilaRewardSlot = ParentGO.GetComponent<UIGachaGuerrilaRewardSlot>();
                uIGachaGuerrilaRewardSlot.OnClick_Slot(_rewarddata);
            }
            else if (ParentGO.GetComponent<UIMessagePopup>() != null)
            {
                UIItemInfoPopup itemInfoPopup = LobbyUIManager.Instance.GetUI<UIItemInfoPopup>("ItemInfoPopup");
                if (itemInfoPopup != null)
                {
                    UIMessagePopup messagePopup = ParentGO.GetComponent<UIMessagePopup>();
                    UIPanel uiPanel = messagePopup.GetComponent<UIPanel>();
                    if (uiPanel != null)
                    {
                        itemInfoPopup.SetGemEvolutionItemInfo(uiPanel.depth, uiPanel.sortingLayerName, uiPanel.sortingOrder, uiPanel.useSortingOrder);
                    }
                }

                GameSupport.OpenRewardTableDataInfoPopup(_rewarddata);
            }
            else
            {
                GameSupport.OpenRewardTableDataInfoPopup(_rewarddata);
            }
        }
        else
        {
            GameSupport.OpenRewardTableDataInfoPopup(_rewarddata);
        }
    }

	public void OnClick_RewardBtn()
    {
		if (kLbRewardBtn.gameObject.activeSelf == false)
		{
			return;
		}

		if (string.IsNullOrEmpty(LbDay.textlocalize))
		{
			return;
		}

        MessagePopup.CYNReward(FLocalizeString.Instance.GetText(3286), FLocalizeString.Instance.GetText(3287), _rewarddata,
            (eREWARDTYPE)GameInfo.Instance.GameConfig.AbsentCostType, GameInfo.Instance.GameConfig.AbsentCostIndex, GameInfo.Instance.GameConfig.AbsentCostValue, eTEXTID.YES, eTEXTID.NO, () =>
        {
            eREWARDTYPE eRewardType = (eREWARDTYPE)GameInfo.Instance.GameConfig.AbsentCostType;
            int nCostIndex = GameInfo.Instance.GameConfig.AbsentCostIndex;

            if (GameInfo.Instance.GameConfig.AbsentCostValue <= LobbyUIManager.Instance.GetInvenCount(eRewardType, nCostIndex))
            {
                UIDailyLoginBonusPopup uiDailyLoginBonusPopup = ParentGO.GetComponent<UIDailyLoginBonusPopup>();
                if (uiDailyLoginBonusPopup == null)
                {
                    return;
                }

                byte.TryParse(LbDay.textlocalize, out byte nDay);

                uiDailyLoginBonusPopup.ReAttendance(nDay);
            }
            else
            {
                if (eRewardType == eREWARDTYPE.GOODS)
                {
                    MessageToastPopup.Show(FLocalizeString.Instance.GetText(100 + nCostIndex));
                }
                else
                {
                    MessageToastPopup.Show(FLocalizeString.Instance.GetText(3003));
                }
            }
        });
	}

	public bool IsVisible()
	{
		if(mWidget == null)
		{
			mWidget = GetComponent<UIWidget>();
		}

		return mWidget.isVisible;
	}

	public void ShowAttendance(bool show)
	{
		if (show)
		{
			SprAttendance.MakePixelPerfect();
		}

		SprAttendance.gameObject.SetActive(show);
	}

    public void SetCountLabel(string label)
    {
        kCountLabel.textlocalize = label;
    }
}


