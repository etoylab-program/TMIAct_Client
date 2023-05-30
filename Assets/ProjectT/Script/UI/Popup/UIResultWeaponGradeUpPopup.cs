using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIResultWeaponGradeUpPopup : FComponent
{
    [Header("WeaponInfo")]
    public GameObject kWeaponInfo;
	public UISprite kWeaponWakeSpr;
    public GameObject kWeaponWakeEff;
	public UISprite kWaeponGradeSpr;
	public UILabel kWeaponNameLabel;
	public UILabel kWeaponLevelLabel;

    [Header("WeaponSlot")]
    public GameObject kWeaponSlot;
    public List<UISprite> kNextSlotList;
    public List<UITexture> kNextSlotIconList;
    public List<UISprite> kNextSlotLockList;

    [Header("Button")]
    public UIButton kConfirmBtn;

    private WeaponData _weapondata = null;
    private Vector3 m_originalEffPos;

	public override void Awake() {
		base.Awake();

		m_originalEffPos = kWeaponWakeEff.transform.localPosition;
	}

	public override void OnEnable()
	{
		long uid = (long)UIValue.Instance.GetValue( UIValue.EParamType.WeaponUID );
		_weapondata = GameInfo.Instance.GetWeaponData( uid );
		kWeaponInfo.SetActive( true );

		base.OnEnable();
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        kWaeponGradeSpr.spriteName = "itemgrade_L_" + _weapondata.TableData.Grade.ToString();
        //kWaeponGradeSpr.MakePixelPerfect();

        kWeaponNameLabel.textlocalize = FLocalizeString.Instance.GetText(_weapondata.TableData.Name);
        kWeaponLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _weapondata.Level, GameSupport.GetWeaponMaxLevel(_weapondata));

        //  등급 관련
        kWeaponWakeSpr.spriteName = "itemwake_0" + (_weapondata.Wake - 1).ToString();

        Vector3 effPos = m_originalEffPos;
        effPos.y -= ((_weapondata.Wake - 1) * 22.0f);
        kWeaponWakeEff.transform.localPosition = effPos;


        int wakemax = GameSupport.GetWeaponMaxWake(_weapondata);
        int slotmax = GameSupport.GetWeaponGradeSlotCount(_weapondata.TableData.Grade, wakemax);
        int slotnow = GameSupport.GetWeaponGradeSlotCount(_weapondata.TableData.Grade, _weapondata.Wake);

        for (int i = 0; i < kNextSlotList.Count; i++)
            kNextSlotList[i].gameObject.SetActive(false);


        for (int i = 0; i < slotmax; i++)
        {
            kNextSlotList[i].gameObject.SetActive(true);
            kNextSlotIconList[i].gameObject.SetActive(false);
            kNextSlotLockList[i].gameObject.SetActive(false);
            if (i >= slotnow)
                kNextSlotLockList[i].gameObject.SetActive(true);

            GemData gemdata = GameInfo.Instance.GetGemData(_weapondata.SlotGemUID[i]);
            if (gemdata != null)
            {
                kNextSlotIconList[i].gameObject.SetActive(true);
                kNextSlotIconList[i].mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + gemdata.TableData.Icon);
            }
        }
    }
 	
	public void OnClick_ConfirmBtn()
    {
        DirectorUIManager.Instance.StopItemWakeUp();
	}

    public override bool IsBackButton()
    {
        return false;
    }

    public void UIAniEventChangeIcon()
    {
        kWeaponWakeSpr.spriteName = "itemwake_0" + _weapondata.Wake.ToString();
    }
}
