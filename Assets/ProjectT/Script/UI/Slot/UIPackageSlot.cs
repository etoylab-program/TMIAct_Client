using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using eINT = ePackageSlotItemNameType;

public enum CharPackageType
{
    Normal = 0,
    Premium = 1,
    Limit = 2,
}

public class UIPackageSlot : FSlot 
{
    [Serializable]
    public class ItemName
    {
        public GameObject kItemNameObj;
        public UILabel kItemNameLabel;
        public UILabel kBenefitLabel;
    }

    public UIPriceUnit kPriceUnit;

    [Header("StoreItemName Property")]
    public List<ItemName> kItemNameList;

	public UIButton kInfoBtn;
	public UITexture kTex;
	public UITexture kPremiumEFF;

    public GameObject kWeeklyListBG;
    public UILabel kWeeklyItemNameLabel;
    public GameObject kMonthlyListBG;
    public UILabel kMonthlyItemNameLabel;

    private System.DateTime _nowTime;
    private UIPackagePopup.PackageBtnData _categoryData;
    private UIPackagePopup.ePackageUIType _uiType;
    private GameClientTable.StoreDisplayGoods.Param _clientStoreData;
    private GameTable.Store.Param _storeData;

    public void UpdateSlot(UIPackagePopup.PackageBtnData categoryData, UIPackagePopup.ePackageUIType uitype, GameClientTable.StoreDisplayGoods.Param data) 	//Fill parameter if you need
	{
        _nowTime = GameInfo.Instance.GetNetworkTime();
        _storeData = null;
        _categoryData = categoryData;
        _uiType = uitype;
        _clientStoreData = data;
        kPriceUnit.ParentGO = this.gameObject;
        foreach (ItemName itemName in kItemNameList)
        {
            itemName.kItemNameObj.SetActive(false);
        }
        kWeeklyListBG.SetActive(false);
        kMonthlyListBG.SetActive(false);
        kTex.gameObject.SetActive(false);
        kPremiumEFF.gameObject.SetActive(false);
        kInfoBtn.gameObject.SetActive(true);
		switch ( _uiType ) {
			case UIPackagePopup.ePackageUIType.CharJump: {
				string texPath = "Icon/" + _clientStoreData.Icon;
				GameSupport.LoadLocalizeTexture( kTex, "icon", texPath, _clientStoreData.IconLocalize );

				SetStoreData();
				kPriceUnit.UpdateSlot( data.StoreID, _nowTime, _uiType );
				kTex.SetActive( true );

				int labelIndex = (int)eINT.Common;
				if ( _clientStoreData.AdvancedPackage == (int)CharPackageType.Normal ) {
					kPriceUnit.SetVisiblePremiumBtn( false );
					kPremiumEFF.SetActive( false );
				}
				else if ( _clientStoreData.AdvancedPackage == (int)CharPackageType.Premium ) {
					//Premium
					labelIndex = (int)eINT.Premium;

					kPriceUnit.SetVisiblePremiumBtn( true );
					kPremiumEFF.SetActive( true );

					string effPath = "Icon/" + _clientStoreData.Icon + "_Effect";
					GameSupport.LoadLocalizeTexture( kPremiumEFF, "icon", effPath, _clientStoreData.IconLocalize );
				}
				else if ( _clientStoreData.AdvancedPackage == (int)CharPackageType.Limit ) {
					Log.Show( "Limit Package" );

					//Premium
					labelIndex = (int)eINT.Premium;

					kPriceUnit.SetVisiblePremiumBtn( true );
					kPremiumEFF.SetActive( true );

					string effPath = "Icon/" + _clientStoreData.Icon + "_Effect";
					GameSupport.LoadLocalizeTexture( kPremiumEFF, "icon", effPath, _clientStoreData.IconLocalize );
				}

				kItemNameList[labelIndex].kItemNameObj.SetActive( true );
				kItemNameList[labelIndex].kItemNameLabel.textlocalize = FLocalizeString.Instance.GetText( _clientStoreData.PackageLabel );

				int.TryParse( _clientStoreData.CmpstBnfts, out int benefitID );
				kItemNameList[labelIndex].kBenefitLabel.textlocalize = FLocalizeString.Instance.GetText( benefitID );
				kItemNameList[labelIndex].kBenefitLabel.gameObject.SetActive( true );
			}
			break;

			case UIPackagePopup.ePackageUIType.MonthlyFee: {
				kInfoBtn.gameObject.SetActive( false );
				SetStoreData();
				kPriceUnit.UpdateSlot( data.StoreID, _nowTime, _uiType );
				kPriceUnit.SetVisiblePremiumBtn( false );
			}
			break;

			case UIPackagePopup.ePackageUIType.Rank: {
				Log.Show( "Rank Package" );
				kInfoBtn.gameObject.SetActive( true );
				SetStoreData();
				kPriceUnit.UpdateSlot( data.StoreID, _nowTime, _uiType );
				kPriceUnit.SetVisiblePremiumBtn( false );
				kItemNameList[(int)eINT.Common].kItemNameObj.SetActive( true );
				kItemNameList[(int)eINT.Common].kItemNameLabel.textlocalize = FLocalizeString.Instance.GetText( _clientStoreData.Name );
				kItemNameList[(int)eINT.Common].kBenefitLabel.gameObject.SetActive( false );
			}
			break;

			case UIPackagePopup.ePackageUIType.Starter: {
				string texPath = "Icon/" + _clientStoreData.Icon;
				GameSupport.LoadLocalizeTexture( kTex, "icon", texPath, _clientStoreData.IconLocalize );
				kTex.SetActive( true );

				kInfoBtn.gameObject.SetActive( true );
				SetStoreData();
				kPriceUnit.UpdateSlot( data.StoreID, _nowTime, _uiType );
				kPriceUnit.SetVisiblePremiumBtn( false );
				kItemNameList[(int)eINT.Common].kItemNameObj.SetActive( true );
				kItemNameList[(int)eINT.Common].kItemNameLabel.textlocalize = FLocalizeString.Instance.GetText( _clientStoreData.Name );
			}
			break;

			case UIPackagePopup.ePackageUIType.Time: {
				string texPath = "Icon/" + _clientStoreData.Icon;
				GameSupport.LoadLocalizeTexture( kTex, "icon", texPath, _clientStoreData.IconLocalize );

				SetStoreData();
				kPriceUnit.UpdateSlot( data.StoreID, _nowTime, _uiType );
				kPriceUnit.SetVisiblePremiumBtn( false );
				kTex.SetActive( true );
				if ( _categoryData.kPackageItem.SubCategory == (int)UIPackagePopup.eTimePackageType.Day ) {
				}
				else if ( _categoryData.kPackageItem.SubCategory == (int)UIPackagePopup.eTimePackageType.Weekly ) {
					kWeeklyListBG.SetActive( true );
					kWeeklyItemNameLabel.textlocalize = FLocalizeString.Instance.GetText( _clientStoreData.Name );
				}
				else if ( _categoryData.kPackageItem.SubCategory == (int)UIPackagePopup.eTimePackageType.Monthly ) {
					kMonthlyListBG.SetActive( true );
					kMonthlyItemNameLabel.textlocalize = FLocalizeString.Instance.GetText( _clientStoreData.Name );
				}
			}
			break;

			case UIPackagePopup.ePackageUIType.Gacha: {
				Log.Show( "Gacha Package" );
			}
			break;
		}
	}

    private void SetStoreData()
    {
        if (_clientStoreData == null)
        {
            _storeData = null;
            return;
        }

        _storeData = GameInfo.Instance.GameTable.FindStore(x => x.ID == _clientStoreData.StoreID);
    }
 
	public void OnClick_Slot()
	{
	}
 

	
	public void OnClick_InfoBtn()
	{
        if (_storeData == null)
        {
            Log.Show("Store Data is NULL", Log.ColorType.Red);
            return;
        }

        Log.Show("Show PackageInfo StoreID : " + _storeData.ID);
        PackageInfoPopup.ShowPackageInfoPopup(_storeData.ID, _uiType, _clientStoreData, () => 
            {
                BuyPackage();
            });
	}

    public void BuyPackage()
    {
        if (IAPManager.Instance.IsBuying)
            return;

        if (_storeData == null)
            return;

        if (!IAPManager.Instance.IAPNULL_CHECK())
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3197), null);
            return;
        }

        GameTable.Store.Param storeParam = GameInfo.Instance.GameTable.FindStore(x => x.ID == _storeData.ID);
        if (storeParam == null)
        {
            Debug.LogError("StoreParam is null");
            return;
        }
        
        int nCostumeId = -1;
        bool bRewardChar = false;
        bool bRewardCostume = false;
        
        List<GameTable.Random.Param> overlabList = new List<GameTable.Random.Param>();
        List<GameTable.Random.Param> rewardList = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == storeParam.ProductIndex);
        for (int i = 0; i < rewardList.Count; i++)
        {
            if (rewardList[i].ProductType == (int)eREWARDTYPE.CHAR)
            {
                CharData chardata = GameInfo.Instance.GetCharDataByTableID(rewardList[i].ProductIndex);
                if (chardata != null)
                {
                    overlabList.Add(rewardList[i]);
                }
                
                bRewardChar = true;
            }
            else if (rewardList[i].ProductType == (int)eREWARDTYPE.COSTUME)
            {
                for (int j = 0; j < GameInfo.Instance.CostumeList.Count; j++)
                {
                    if (GameInfo.Instance.CostumeList[j] == rewardList[i].ProductIndex)
                    {
                        overlabList.Add(rewardList[i]);
                    }
                }
                
                nCostumeId = rewardList[i].ProductIndex;
                bRewardCostume = true;
            }
        }

        //7766 인앱 연결
        //3113	{0} 상품을 구매 하시겠습니까?
        if (overlabList.Count > 0)
        {
            PackageBuyPopup.ShowPackageBuyPopup(storeParam.ID, overlabList, FLocalizeString.Instance.GetText((int)eTEXTID.TITLE_BUY), FLocalizeString.Instance.GetText((int)eTEXTID.OK), FLocalizeString.Instance.GetText((int)eTEXTID.CANCEL), OnMessageOK, OnMessageCancel);
        }
        else
        {
            string charName = string.Empty;
            if (!bRewardChar && bRewardCostume)
            {
                GameTable.Costume.Param costumeTableData = GameInfo.Instance.GameTable.FindCostume(nCostumeId);
                if (costumeTableData != null)
                {
                    CharData chardata = GameInfo.Instance.GetCharDataByTableID(costumeTableData.CharacterID);
                    if (chardata == null)
                    {
                        GameTable.Character.Param charTableData =
                            GameInfo.Instance.GameTable.FindCharacter(costumeTableData.CharacterID);
                        if (charTableData != null)
                        {
                            charName = charTableData.Icon.ToLower();
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(charName))
            {
                string title =
                    FLocalizeString.Instance.GetText(1769, FLocalizeString.Instance.GetText(_clientStoreData.Name));
                
                MessagePopup.OKCANCEL(eTEXTID.BUY, title, OnMessageOK, OnMessageCancel, () => OnMessageCharBuyMove(charName));
            }
            else
            {
                MessagePopup.OKCANCEL(eTEXTID.BUY, string.Format(FLocalizeString.Instance.GetText(110), FLocalizeString.Instance.GetText(_clientStoreData.Name)), OnMessageOK, OnMessageCancel);
            }
        }
    }

    public void OnMessageOK()
    {
        WaitPopup.Show();
        IAPManager.Instance.IsBuying = true;

        GameTable.Store.Param storeParam = GameInfo.Instance.GameTable.FindStore(x => x.ID == _storeData.ID);

#if UNITY_ANDROID
        if (string.IsNullOrEmpty(storeParam.AOS_ID))
        {
            WaitPopup.Hide();
            IAPManager.Instance.IsBuying = false;
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3028));
        }
        else
            IAPManager.Instance.BuyIAPProduct(storeParam.AOS_ID, OnIAPBuySuccess, OnIAPBuyFailed);
#elif UNITY_IOS
        if (string.IsNullOrEmpty(storeParam.IOS_ID))
        {
            WaitPopup.Hide();
            IAPManager.Instance.IsBuying = false;
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3028));
        }
        else
            IAPManager.Instance.BuyIAPProduct(storeParam.IOS_ID, OnIAPBuySuccess, OnIAPBuyFailed);
#elif !DISABLESTEAMWORKS
        if (storeParam.PurchaseValue <= 0)
        {
            WaitPopup.Hide();
            IAPManager.Instance.IsBuying = false;
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3028));
        }
        else
            IAPManager.Instance.BuyIAPProduct(storeParam.ID.ToString(), OnIAPBuySuccess, OnIAPBuyFailed);
#endif
    }

    public void OnIAPBuySuccess()
    {
        if (this.ParentGO == null)
            return;

        if (this.ParentGO.GetComponent<UIPackagePopup>() != null)
        {
            UIPackagePopup packagepopup = this.ParentGO.GetComponent<UIPackagePopup>();
            packagepopup.OnIAPBuySuccess(_storeData.ID);
        }


    }

    public void OnIAPBuyFailed()
    {
        WaitPopup.Hide();
        IAPManager.Instance.IsBuying = false;

        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3212));
    }

    public void OnMessageCancel()
    {
        WaitPopup.Hide();
        IAPManager.Instance.IsBuying = false;
    }

    public void OnMessageCharBuyMove(string charName)
    {
        WaitPopup.Hide();
        IAPManager.Instance.IsBuying = false;
        
        var comp = ParentGO.GetComponent<UIPackagePopup>();
        if (comp != null)
        {
            comp.MoveCharPackage(charName);
        }
    }

    private void CheckPackage()
    {

    }
}
