using UnityEngine;
using System.Collections;

public class UICostumeListSlot : FSlot {


    public UISprite kBGSpr;
    public UISprite kBGDisSpr;
    public UISprite kSelSpr;
    public UISprite kTrySpr;
    public UISprite kAttachSpr;
    public UITexture kIconTex;
    public UISprite kDyeingSpr;
    public UISprite kImpossibleSpr;
    public UISprite kNoticeSpr;
    private int _index;
    private GameTable.Costume.Param _costumetabledata;

    private void SetCostumeSlot()
    {
        kTrySpr.gameObject.SetActive(false);
        kAttachSpr.gameObject.SetActive(false);
        kSelSpr.gameObject.SetActive(false);
        kImpossibleSpr.gameObject.SetActive(false);
        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/ListSlot/" + _costumetabledata.Icon);
        kIconTex.color = new Color(1, 1, 1, 1);
        if (kNoticeSpr != null)
            kNoticeSpr.SetActive(false);
    }

    public void UpdateSlot( int index, GameTable.Costume.Param tabledata  ) 	//Fill parameter if you need
	{
        if (tabledata == null)
        {
            return;
        }

        _index = index;
        _costumetabledata = tabledata;
        SetCostumeSlot();

         UICharInfoTabCostumePanel panel = ParentGO.GetComponent<UICharInfoTabCostumePanel>();
        if (panel == null)
            return;

        if (panel.SeleteCostumeID == _costumetabledata.ID)
        {
            Log.Show(panel.SeleteCostumeID + " / " + _costumetabledata.ID + " / " +  FLocalizeString.Instance.GetText(_costumetabledata.Name));
            kSelSpr.gameObject.SetActive(panel.SeleteCostumeID == _costumetabledata.ID);
            panel.SetSpringScroll(_index);
        }

        CharData chardata = panel.CharData;

        bool bdis = false;
        
        if (chardata == null)
        {
            bdis = true;
        }
        else
        {
            if (chardata.EquipCostumeID == _costumetabledata.ID)
            {
                kTrySpr.gameObject.SetActive(true);
            }
            else
            {
				if ( !GameInfo.Instance.HasCostume( _costumetabledata.ID ) ) {
					bdis = true;
					SetImpossible();
				}
			}
        }

        SetActive(kBGSpr.gameObject,false);
        SetActive(kBGDisSpr.gameObject, false);

        if (bdis)
        {
            //  비활성화
            //kBGSpr.spriteName = "frmcostume_dis";
            SetActive(kBGDisSpr.gameObject, true);
            kIconTex.color = new Color(1, 1, 1, 0.6f);
        }
        else
        {
            //  활성화
            //kBGSpr.spriteName = "frmcostume";
            SetActive(kBGSpr.gameObject, true);
        }
        
        kDyeingSpr.gameObject.SetActive(_costumetabledata.UseDyeing != 0);
    }

    public void UpdateSlot(int selectIdx, int index, GameClientTable.Book.Param bookdata)
    {
        _index = index;
        _costumetabledata = GameInfo.Instance.GameTable.FindCostume(x => x.ID == bookdata.ItemID);
        SetCostumeSlot();

        kSelSpr.gameObject.SetActive(selectIdx == _costumetabledata.ID);

        bool bdis = false;

		if ( !GameInfo.Instance.HasCostume( _costumetabledata.ID ) ) {
			bdis = true;
			SetImpossible();
		}

		SetActive(kBGSpr.gameObject, false);
        SetActive(kBGDisSpr.gameObject, false);

        if (bdis)
        {
            //  비활성화
            //kBGSpr.spriteName = "frmcostume_dis";
            SetActive(kBGDisSpr.gameObject, true);
            kIconTex.color = new Color(1, 1, 1, 0.6f);
        }
        else
        {
            //  활성화
            //kBGSpr.spriteName = "frmcostume";
            SetActive(kBGSpr.gameObject, true);
        }

        if (FSaveData.Instance.HasKey("NCostumeBook_" + _costumetabledata.ID.ToString()))
        {
            if (kNoticeSpr != null)
                kNoticeSpr.SetActive(true);
        }

        kDyeingSpr.gameObject.SetActive(_costumetabledata.UseDyeing != 0);
    }


    public void OnClick_Slot()
	{
        UICharInfoTabCostumePanel panel = ParentGO.GetComponent<UICharInfoTabCostumePanel>();
        if (panel != null)
        {
            panel.SetSeleteCostumeID(_costumetabledata.ID);
            return;
        }

        UIBookCostumeListPopup popup = ParentGO.GetComponent<UIBookCostumeListPopup>();
        if (popup != null)
        {
            Debug.Log("CostumeSlot OnClick_Slot");
            FSaveData.Instance.DeleteKey("NCostumeBook_" + _costumetabledata.ID.ToString());

            if (kNoticeSpr != null)
                kNoticeSpr.SetActive(false);

            popup.OnClick_CostumeListSlot(_costumetabledata.ID);
            return;
        }
    }
 
    private void SetActive(GameObject go, bool isActive)
    {
        if (go != null)
            go.SetActive(isActive);
    }

	private void SetImpossible() {
		if ( _costumetabledata.StoreID <= -1 || _costumetabledata.PreVisible <= (int)eCOUNT.NONE ) {
			kImpossibleSpr.gameObject.SetActive( true );
		}
		else {
			GachaCategoryData gachaCategoryData = GameSupport.GetGachaCategoryData( _costumetabledata.StoreID );
			if ( gachaCategoryData != null ) {
				kImpossibleSpr.gameObject.SetActive( GameSupport.GetCurrentServerTime() < gachaCategoryData.StartDate || gachaCategoryData.EndDate < GameSupport.GetCurrentServerTime() );
			}
			else {
				bool isImpossible = true;
				GameTable.Store.Param storeParam = GameInfo.Instance.GameTable.FindStore( _costumetabledata.StoreID );
				if ( storeParam != null ) {
					isImpossible = storeParam.SaleType != 0;
				}
				kImpossibleSpr.SetActive( isImpossible );
			}
		}
	}
}
