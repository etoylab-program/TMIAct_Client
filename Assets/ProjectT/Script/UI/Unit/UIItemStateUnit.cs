using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemStateUnit : FUnit
{
    public UITexture kIconTex;
    //public UISprite kBGSpr;
    //public UISprite kStateSpr;
    //public UILabel kStateLabel;

    //  무기 장착(장착중), 곡옥 장착(장착중), 서포터 장착(서포터)
    public void InitItemStateUnit(CharData chardata, CardData carddata)
    {
        if (chardata == null)
            return;
        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Set/Set_Chara_{0}.png", chardata.TableData.Icon));
    }

    public void InitItemStateUnit(CharData chardata, WeaponData weapondata)
    {
        if (chardata == null)
            return;
        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Set/Set_Chara_{0}.png", chardata.TableData.Icon));
    }


    public void InitItemStateUnit(WeaponData weapondata, GemData gemdata )
    {
        if (weapondata == null)
            return;
        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_" + weapondata.TableData.Icon);
    }

    //  무기 시설(시설), 서포터 시설(시설)
    public void InitItemStateUnit(FacilityData facilityData)
    {
        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_" + GameSupport.GetFacilityIconName(facilityData));
    }

    // 문양 - 아레나 장착
    public void InitItemStateUnit(BadgeData badgedata)
    {
        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_Common.png");
    }

    public void InitItemStateUnit(GameTable.Character.Param charTableData)
    {
        if (charTableData == null)
            return;
        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Set/Set_Chara_{0}.png", charTableData.Icon));
    }

    public void InitItemStateUnit(Dispatch dispatch)
    {
        if (dispatch == null)
            return;

        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_Facility_SptDispatch.png");
    }

    public void InitArmoryStateUnit(WeaponData weapondata)
    {
        kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_Wpn_Depot.png");
    }
}
