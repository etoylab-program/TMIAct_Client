using UnityEngine;
using System.Collections;

public class UIBannerListSlot : FSlot
{
    public UITexture kBannerTex;

    private BannerData _bannerdata;

    private Coroutine _bgLoadCoroutine = null;

	public void UpdateSlot(BannerData bannerdata) 	//Fill parameter if you need
	{
        _bannerdata = bannerdata;

        GetBGTexture(kBannerTex, _bannerdata.UrlImage);
    }
 
	public void OnClick_Slot()
	{
        if (_bannerdata.FunctionValue1 == "PackagePopup")
        {
            GameSupport.PaymentAgreement_Package(int.Parse(_bannerdata.FunctionValue2));
        }
        else
        {
            GameSupport.MoveUI(_bannerdata.FunctionType, _bannerdata.FunctionValue1, _bannerdata.FunctionValue2, _bannerdata.FunctionValue3);
        }
    }
 
    private void GetBGTexture(UITexture target, string url)
    {
        if(_bgLoadCoroutine != null)
        {
            StopCoroutine(_bgLoadCoroutine);
            _bgLoadCoroutine = null;
        }

        if(GameInfo.Instance.netFlag)
        {
			if (target.mainTexture != null)
			{
				DestroyImmediate(target.mainTexture, false);
				target.mainTexture = null;
			}

            target.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(url, true);
            if(target.mainTexture == null)
            {
                _bgLoadCoroutine = StartCoroutine(GetBGTextureAsync(target, url));
            }
        }
        else
        {
            target.mainTexture = (Texture2D)ResourceMgr.Instance.LoadFromAssetBundle("temp", "Temp/" + url);
        }
    }

    IEnumerator GetBGTextureAsync(UITexture target, string url)
    {
        while(this.gameObject.activeSelf)
        {
			if (target.mainTexture != null)
			{
				DestroyImmediate(target.mainTexture, false);
				target.mainTexture = null;
			}

            target.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(url, true);
			if (target.mainTexture != null)
			{
				break;
			}

			yield return null;
        }

        if(_bgLoadCoroutine != null)
            _bgLoadCoroutine = null;
    }
    
}
