using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGachaGreetingsPopup : FComponent
{
    public GameObject kCardRoot;
    public UITexture kCardTex;
    public UITexture kfx_CardTex;
    public UILabel kNameLabel;
	public UILabel kTextLabel;
    public FLabelTextShow kLabelTextShow;
    private RewardData _rewarddata;
    private CardData _carddata;
    private bool _gacha = false;
 

	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        _gacha = (bool)UIValue.Instance.GetValue(UIValue.EParamType.GachaGreetings);
        int index = (int)UIValue.Instance.GetValue(UIValue.EParamType.GachaRewardIndex, true);
        _rewarddata = GameInfo.Instance.RewardList[index];
        _carddata = GameInfo.Instance.GetCardData(_rewarddata.UID);
        if (_rewarddata == null)
            return;
        if (_carddata == null)
            return;

        int seleteimage = 0;
        kCardTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("card", string.Format("Card/{0}_{1}.png", _carddata.TableData.Icon, seleteimage));
        kfx_CardTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("card", string.Format("Card/{0}_{1}.png", _carddata.TableData.Icon, seleteimage));


        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_carddata.TableData.Name);
        kTextLabel.textlocalize = "";

        kLabelTextShow.SetText(FLocalizeString.Instance.GetText(_carddata.TableData.Greetings), true);
       
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        if (_rewarddata == null)
            return;
        if (_carddata == null)
            return;
    }

    public void OnClick_NextBtn()
    {
        if (kLabelTextShow.TextScroll)
            kLabelTextShow.EndScroll();
        else
        {
            DirectorUIManager.Instance.NextGachaNewCardGreeings();
            /*
            if (_gacha)
                DirectorUIManager.Instance.NextGachaNewCardGreeings();
            else
                OnClickClose();
                */
        }
    }

    public override bool IsBackButton()
    {
        return false;
    }
}
