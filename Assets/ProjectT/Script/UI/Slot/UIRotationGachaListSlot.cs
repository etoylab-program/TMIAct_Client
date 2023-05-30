using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIRotationGachaListSlot : FSlot
{
    [SerializeField] private UITexture kBannerTex;
    [SerializeField] private UILabel kTitleLabel;
    [SerializeField] private UILabel kDescLabel;
    
    [Header("Card")]
    [SerializeField] private GameObject kCardObj;
    [SerializeField] private UISprite kCardTypeSpr;

    [Header("Weapon")]
    [SerializeField] private GameObject kWeaponObj;
    [SerializeField] private FList kWeaponTexFList;

    private Coroutine _bgLoadCoroutine = null;
    private List<string> _weaponTexNameList = new List<string>();
    private bool _isResetPosition = false;

    private void Awake()
    {
        kWeaponTexFList.EventUpdate = OnEventUpdate_WeaponTex;
        kWeaponTexFList.EventGetItemCount = OnEventGetItemCount_WeaponTex;
        kWeaponTexFList.UpdateList();
    }

    private void LateUpdate()
    {
        if (_isResetPosition == false)
        {
            return;
        }

        _isResetPosition = false;
        kWeaponTexFList.ScrollView.ResetPosition();
    }

    private void OnEventUpdate_WeaponTex(int index, GameObject obj)
    {
        UITexture tex = obj.GetComponent<UITexture>();
        if (tex == null)
        {
            return;
        }

        string iconName = string.Empty;
        if (0 <= index && index < _weaponTexNameList.Count)
        {
            iconName = _weaponTexNameList[index];
        }

        tex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", Utility.AppendString("Icon/Set/Set_Chara_", iconName, ".png"));
    }

    private int OnEventGetItemCount_WeaponTex()
    {
        return _weaponTexNameList.Count;
    }

    public void UpdateSlot(GameTable.RotationGacha.Param param) 	//Fill parameter if you need
	{
        //string urlimg = "";
        GachaCategoryData data = GameInfo.Instance.ServerData.GachaCategoryList.Find(x => x.Text == param.ID);
        if (data == null) return;
        Debug.LogWarningFormat("data.UrlBGImage : {0}", data.UrlBGImage);
        GetBGTexture(kBannerTex, data.UrlBGImage, true, data.Localize[(int)eGachaLocalizeType.Background]);

        Enum.TryParse(data.Type, out eGachaRotationType type);

        switch(type)
        {
            case eGachaRotationType.Weapon:
                {
                    _weaponTexNameList.Clear();
                    string[] splits = Utility.Split(data.Value1, ',');
                    foreach (string split in splits)
                    {
                        int.TryParse(split, out int value);
                        GameTable.Character.Param characterParam = GameInfo.Instance.GameTable.FindCharacter(value);
                        if (characterParam == null)
                        {
                            continue;
                        }
                        _weaponTexNameList.Add(characterParam.Icon);
                    }

                    kWeaponTexFList.Reset();

                    if (kWeaponTexFList.ListItem.Count - kWeaponTexFList.AddRowOrColumn < _weaponTexNameList.Count)
                    {
                        kWeaponTexFList.ScrollView.contentPivot = UIWidget.Pivot.TopLeft;
                    }
                    else
                    {
                        kWeaponTexFList.ScrollView.contentPivot = UIWidget.Pivot.Center;
                    }

                    _isResetPosition = true;
                }
                break;
            case eGachaRotationType.Card:
                {
                    int.TryParse(data.Value1, out int value);
                    kCardTypeSpr.spriteName = $"SupporterType_{value}";
                }
                break;
        }

        kCardObj.SetActive(type == eGachaRotationType.Card);
        kWeaponObj.SetActive(type == eGachaRotationType.Weapon);

        kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(param.Name);
        if (kDescLabel != null)
        {
            kDescLabel.SetActive(true);

            System.DateTime currentTime = GameSupport.GetCurrentRealServerTime();
            System.DateTime nextDayTime = currentTime.AddDays(1);
            System.DateTime temp = new System.DateTime(nextDayTime.Year, nextDayTime.Month, nextDayTime.Day, 0, 0, 0);
            System.DateTime currentZoneTime = GameSupport.GetLocalTimeByServerTime(temp);

            kDescLabel.textlocalize = FLocalizeString.Instance.GetText(1709, currentZoneTime.Hour);
        }
        
    }

    public void ResetWeaponFList()
    {
        kWeaponTexFList.Reset();
    }

	public void OnClick_Slot()
	{

    }

    private void GetBGTexture(UITexture target, string url, bool platform, bool localize)
    {
        if (_bgLoadCoroutine != null)
        {
            StopCoroutine(_bgLoadCoroutine);
            _bgLoadCoroutine = null;
        }

        if (GameInfo.Instance.netFlag)
        {
			if (target.mainTexture != null)
			{
				DestroyImmediate(target.mainTexture, false);
				target.mainTexture = null;
			}

            target.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(url, platform, localize);
            if (target.mainTexture == null)
            {
                _bgLoadCoroutine = StartCoroutine(GetBGTextureAsync(target, url, platform, localize));
            }
        }
        else
        {
            target.mainTexture = (Texture2D)ResourceMgr.Instance.LoadFromAssetBundle("temp", "Temp/" + url);
        }
    }

    IEnumerator GetBGTextureAsync(UITexture target, string url, bool platform, bool localize)
    {
        while(this.gameObject.activeSelf)
        {
			if (target.mainTexture != null)
			{
				DestroyImmediate(target.mainTexture, false);
				target.mainTexture = null;
			}

            target.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(url, platform, localize);
            if(target.mainTexture != null)
                break;
            yield return null;
        }

        if(_bgLoadCoroutine != null)
            _bgLoadCoroutine = null;
    }
    
}
