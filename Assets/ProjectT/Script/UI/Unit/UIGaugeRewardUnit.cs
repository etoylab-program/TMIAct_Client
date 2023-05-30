using UnityEngine;
using System.Collections;

public class UIGaugeRewardUnit : FUnit
{
	[SerializeField] private UISprite IconSpr;
	[SerializeField] private UITexture IconTexture;
	[SerializeField] private UISprite CheckSpr;
	[SerializeField] private UILabel IconTextLabel;
	[SerializeField] private UILabel RewardTextLabel;

	[SerializeField] private UISprite BGSpr;
	[SerializeField] private UISprite EffectSpr;

	private GameTable.Random.Param RewardTable;
	private bool IsCheck;
	private string IconText;
	private string RewardText;
	private bool IsEnableBG = false;

	private uint BitFlag = 0;
	private byte ChoiceBit = 0;

	private OnRewardByteCallBack _byteCallBack;

	public void SetData(GameTable.Random.Param _reward, bool _isCheck)
    {
		SetCallBack(null);
		BitFlag = 0;
        ChoiceBit = 0;

        RewardTable = _reward;
		IsCheck = _isCheck;

		IconText = RewardTable == null ? string.Empty : string.Format("{0}", RewardTable.ProductValue);
		RewardText = RewardTable == null ? string.Empty : string.Format("{0:###,###}", RewardTable.Value);

		SetIcon();

		CheckSpr.SetActive(IsCheck);
		IconTextLabel.text = IconText;
		RewardTextLabel.text = RewardText;
    }

	public void SetData(GameTable.Random.Param _reward, uint _bitflag, byte _choicebit, bool _EnableBG)
    {
		SetCallBack(null);
		BitFlag = _bitflag;
        ChoiceBit = _choicebit;
		IsEnableBG = _EnableBG;

		RewardTable = _reward;
        IsCheck = GameSupport.IsComplateMissionRecive(BitFlag, ChoiceBit);

        IconText = RewardTable == null ? string.Empty : string.Format("{0}", RewardTable.ProductValue);
		//RewardText = RewardTable == null ? string.Empty : string.Format("{0:###,###}", RewardTable.Value);		
		RewardText = RewardTable == null ? string.Empty : RewardTable.Value.ToKMB();

		SetIcon();

        CheckSpr.SetActive(IsCheck);
        IconTextLabel.text = IconText;
        RewardTextLabel.text = RewardText;

        BGSpr.SetActive(false);
        EffectSpr.SetActive(false);

        if (!IsCheck && IsEnableBG)
		{
            BGSpr.SetActive(true);
            EffectSpr.SetActive(true);
        }
    }

	public void SetCallBack(OnRewardByteCallBack callback) { _byteCallBack = callback; }

	private void SetIcon()
	{
		IconSpr.SetActive(false);
		IconTexture.SetActive(false);

		switch ((eREWARDTYPE)RewardTable.ProductType)
		{
			case eREWARDTYPE.GOODS:
				IconSpr.SetActive(true);
				IconSpr.spriteName = GameSupport.GetGoodsIconName((eGOODSTYPE)RewardTable.ProductIndex);
				break;
			case eREWARDTYPE.ITEM:
				var tabledata = GameInfo.Instance.GameTable.FindItem(RewardTable.ProductIndex);
				if (tabledata != null)
                {
					IconTexture.SetActive(true);
					IconTexture.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + tabledata.Icon);
				}
				break;
			default:
				IconSpr.spriteName = string.Empty;
				break;
		}
	}

    public void OnClick()
    {
		if (IsCheck)
		{
			//아이템 확인
			RewardData _rewarddata = new RewardData(RewardTable.ProductType, RewardTable.ProductIndex, RewardTable.ProductValue);
			GameSupport.OpenRewardTableDataInfoPopup(_rewarddata);
			return;
		}

		if (!IsEnableBG)
		{
            //아이템 확인
            RewardData _rewarddata = new RewardData(RewardTable.ProductType, RewardTable.ProductIndex, RewardTable.ProductValue);
            GameSupport.OpenRewardTableDataInfoPopup(_rewarddata);
            return;
		}

		if (_byteCallBack != null)
        {	
			_byteCallBack(RewardTable, ChoiceBit);
		}
    }
}
