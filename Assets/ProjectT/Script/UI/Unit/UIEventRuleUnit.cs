using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using eBT = eBannerType;
using eRN = eEventRuleTexName;
using eRT = eEventRuleTexType;

public class UIEventRuleUnit : FUnit
{
    [SerializeField] private int kPage = -1;
    [SerializeField] private UILabel kEventNameLabel = null;
    [SerializeField] private List<UITexture> kTexList = null;

    private int _eventId;
    
    private void OnEnable()
    {
        GameTable.EventSet.Param tableData = GameInfo.Instance.GameTable.FindEventSet(_eventId);
        if (tableData == null)
        {
            return;
        }
        
        switch (tableData.EventType)
        {
            case (int)eEventRewardKind.RESET_LOTTERY:
                if (kPage == 0)
                {
                    _SetTexture(eRN.EventBgTex,   eBT.EVENT_MAINBG,  eRT.MainUrl);
                    _SetTexture(eRN.StageBgTex,   eBT.EVENT_STAGEBG, eRT.MainUrl);
                    _SetTexture(eRN.EventLogoTex, eBT.EVENT_LOGO,    eRT.MainUrl);

                    if (kEventNameLabel != null)
                    {
                        kEventNameLabel.textlocalize = FLocalizeString.Instance.GetText(tableData.Name);
                    }
                }
                else
                {
                    _SetTexture(eRN.EventBgTex, eBT.EVENT_MAINBG, eRT.MainUrl);
                    _SetTexture(eRN.CardTex,    eBT.EVENT_RULEBG, eRT.AssetBundle, true);
                }
                break;
            case (int)eEventRewardKind.EXCHANGE:
                if (kPage == 0)
                {
                    _SetTexture(eRN.EventBgTex,   eBT.EVENT_MAINBG, eRT.MainUrl);
                    _SetTexture(eRN.EventLogoTex, eBT.EVENT_LOGO,   eRT.MainUrl);
                }
                else
                {
                    _SetTexture(eRN.CardTex, eBT.EVENT_RULEBG, eRT.SubUrl);
                }
                break;
        }
    }
    
    private IEnumerator TextureLoadURL(int index, string url, bool platform, bool localize)
    {
        UITexture texture = kTexList[index];
        while (texture.mainTexture == null)
        {
            texture.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(url, platform, localize);
            yield return null;
        }
    }
    
    private void _SetTexture(eRN kind, eBT bannerType, eRT type, bool asset2SubUrl = false)
    {
        BannerData bannerData = 
            GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)bannerType && x.BannerTypeValue == _eventId);
        if (bannerData == null)
        {
            return;
        }

        if (asset2SubUrl && type == eEventRuleTexType.AssetBundle)
        {
            if (string.IsNullOrEmpty(bannerData.FunctionValue2))
            {
                type = eEventRuleTexType.SubUrl;
            }
        }

        bool platform = true;
        switch (bannerType)
        {
            case eBT.EVENT_LOGO:
                platform = false;
                break;
        }
        
        string kindStr = kind.ToString().ToLower();
        int index = kTexList.FindIndex(x => x.gameObject.name.ToLower().Equals(kindStr));
        if (index < 0)
        {
            return;
        }
        
        switch (type)
        {
            case eRT.MainUrl:
            {
                StartCoroutine(TextureLoadURL(index, bannerData.UrlImage, platform, bannerData.Localizes[(int)eBannerLocalizeType.Url]));
                break;
            }
            case eRT.SubUrl:
            {
                StartCoroutine(TextureLoadURL(index, bannerData.UrlAddImage1, platform, bannerData.Localizes[(int)eBannerLocalizeType.AddUrl1]));
                break;
            }
            case eRT.AssetBundle:
            {
                if (System.Int32.TryParse(bannerData.FunctionValue2, out int result))
                {
                    kTexList[index].mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("card", $"Card/Supporter_{result:D3}_0.png") as Texture;
                }
                break;
            }
        }
    }
    
    public void Init()
    {
        _eventId = -1;

        foreach (UITexture texture in kTexList)
        {
            texture.mainTexture = null;
        }
    }
    
    public void SetEventId(int eventId)
    {
        _eventId = eventId;
    }
}
