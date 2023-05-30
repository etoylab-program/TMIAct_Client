using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharInfoTabWeaponPanel : FComponent
{
    public UILabel kSkillNameLabel;
    public UILabel kSkillLevelLabel;
    public UILabel kSkillDescLabel;
    public UISprite kSPIconSpr;
    public UILabel  kSPLabel;
    [SerializeField] private UITexture  _SkillCharTex;
    [SerializeField] private UISprite   _DisableSkillCharSpr;
    public UIButton kWeaponChangeBtn;
    public UIButton kSubWeaponChangeBtn;

    public UIButton kWeaponSkinDelBtn;
    public UIButton kSubWeaponSkinDelBtn;

    public UISprite kWeaponNoticeSpr;
    public UIButton kInfoBtn;

    public UIItemListSlot kMainWeaponSlot;
    public UIItemListSlot kSubWeaponSlot;

    //MainWeaponGem
    public List<UISprite> kGemSlotLockList;
    public List<UITexture> kGemTexList;
    public List<UIButton> kGemChangeBtnList;

    //SubWeaponGem
    public List<UISprite> kSubGemSlotLockList;
    public List<UITexture> kSubGemTexList;
    public List<UIButton> kSubGemChangeBtnList;

    public UILabel kLockLabel;
    public GameObject kSubWeaponEmptyObj;
    public List<GameObject> kSubWeaponLockDisableObjs;


    [Header("Skin Values")]
    public UITexture kMainSkinTex;
    public UITexture kSubSkinTex;
    public UILabel kMainSkinLb;
    public UILabel kSubSkinLb;

    [Header("Add Gem UR Grade")]
    [SerializeField] private GameObject mainGemTooltipObj = null;
    [SerializeField] private UILabel mainGemTooltipLabel = null;
    [SerializeField] private List<UISprite> mainGemSetOptList = null;
    [SerializeField] private FToggle mainGemInfoToggle = null;

    [SerializeField] private GameObject subGemTooltipObj = null;
    [SerializeField] private UILabel subGemTooltipLabel = null;
    [SerializeField] private List<UISprite> subGemSetOptList = null;
    [SerializeField] private FToggle subGemInfoToggle = null;

	[Header( "CharInfoPanel" )]
    [SerializeField] private UIButton   _WeaponOffBtn   = null;
	[SerializeField] private UILabel    _WeaponOffLabel = null;

	private CharData _chardata;
    private WeaponData _mainWeapondata;
    private WeaponData _subWeaponData;

    private long _mainWeaponSkinId = 0;
    private long _subWeaponSkinId = 0;

    private string _mainSetOptStr;
    private string _subSetOptStr;

    public override void Awake()
	{
		base.Awake();
        
        Type = TYPE.Tab;

        mainGemInfoToggle.EventCallBack = OnEventMainGemTabSelect;
        subGemInfoToggle.EventCallBack = OnEventSubGemTabSelect;
    }
 
	public override void OnEnable()
	{
        InitComponent();
		base.OnEnable();
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();

		if ( RenderTargetChar.Instance.AddedWeapon && RenderTargetChar.Instance.AddedWeapon is PlayerGuardian ) {
			Utility.InitTransform( RenderTargetChar.Instance.AddedWeapon.aniEvent.gameObject );
		}

		StopCoroutine("ShowAttachedObject");
        RenderTargetChar.Instance.ShowAttachedObject(false);

        if ( RenderTargetChar.Instance.RenderPlayer && 
             ( RenderTargetChar.Instance.RenderPlayer.aniEvent.curAniType == eAnimation.Lobby_Weapon || 
               RenderTargetChar.Instance.RenderPlayer.aniEvent.curAniType == eAnimation.Lobby_Weapon_Idle ) ) { 
            RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Idle01, 0, eFaceAnimation.Idle01, 0);
        }
    }

    public override void InitComponent()
	{
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
        _chardata = GameInfo.Instance.GetCharData(uid);

        //RenderTargetChar.Instance.ShowAttachedObject(true);
        //RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Lobby_Weapon, 0);
        GameSupport.ShowTutorialFlag(eTutorialFlag.WEAPON);

		if ( _WeaponOffBtn != null ) {
			_WeaponOffBtn.SetActive( IsEnableWeaponOffBtn() );
			SetWeaponOffLabel();
		}
	}

    private IEnumerator ShowAttachedObject(float delay)
    {
        RenderTargetChar.Instance.ShowAttachedObject(false);
        yield return new WaitForSeconds(delay);

		SetHideWeapon();
	}

	private void SetHideWeapon() {
		RenderTargetChar.Instance.SetCostumeWeapon( _chardata, !_chardata.IsHideWeapon );
		RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject( RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.InGameOnly, !_chardata.IsHideWeapon );
		RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject( RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.LobbyOnly, _chardata.IsHideWeapon );
	}

	public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        mainGemTooltipObj.SetActive(false);
        subGemTooltipObj.SetActive(false);

        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
        _chardata = GameInfo.Instance.GetCharData(uid);

        if (RenderTargetChar.Instance.RenderPlayer != null && 0 < _chardata.EquipWeaponUID)
        {
            RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Lobby_Weapon, 0, eFaceAnimation.Weapon, 0);
            StartCoroutine("ShowAttachedObject", RenderTargetChar.Instance.RenderPlayer.aniEvent.GetCutFrameLength(eAnimation.Lobby_Weapon));
        }

        _mainWeaponSkinId = _chardata.EquipWeaponSkinTID;
        _subWeaponSkinId = _chardata.EquipWeapon2SkinTID;
        Log.Show("SkinIDs : " + _mainWeaponSkinId + " / " + _subWeaponSkinId);

        _mainWeapondata = GameInfo.Instance.GetWeaponData(_chardata.EquipWeaponUID);
        _subWeaponData = GameInfo.Instance.GetWeaponData(_chardata.EquipWeapon2UID);

        if (_mainWeapondata != null)
        {
            _mainSetOptStr = LobbyUIManager.Instance.GetGemSetOpt(_mainWeapondata.SlotGemUID);
            //RenderTargetWeapon.Instance.gameObject.SetActive(true);
            //RenderTargetWeapon.Instance.InitRenderTargetWeapon(_mainWeapondata.TableID, _mainWeapondata.WeaponUID, true);
        }
        else
        {
            _mainSetOptStr = string.Empty;
            RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Idle01, 0, eFaceAnimation.Idle01, 0);
            RenderTargetChar.Instance.ShowAttachedObject(false);
        }

        mainGemInfoToggle.SetToggle(string.IsNullOrEmpty(_mainSetOptStr) ? 1 : 0, SelectEvent.Code);

        if (_subWeaponData != null)
        {
            _subSetOptStr = LobbyUIManager.Instance.GetGemSetOpt(_subWeaponData.SlotGemUID);
        }
        else
        {
            _subSetOptStr = string.Empty;
        }

        subGemInfoToggle.SetToggle(string.IsNullOrEmpty(_subSetOptStr) ? 1 : 0, SelectEvent.Code);

        kMainWeaponSlot.ParentGO = this.gameObject;
        kMainWeaponSlot.UpdateSlot(UIItemListSlot.ePosType.EquipWeapon, 0, _mainWeapondata);
        
        SetWeaponInfo(_mainWeapondata, kMainWeaponSlot, kGemTexList, kGemChangeBtnList, kGemSlotLockList, _mainWeaponSkinId, kWeaponSkinDelBtn, kMainSkinTex, kMainSkinLb, mainGemSetOptList);

        kSubWeaponEmptyObj.SetActive(false);
        kLockLabel.transform.parent.gameObject.SetActive(false);
        
        //서브 웨폰 레벨제한
        if (GameInfo.Instance.GameConfig.CharWeaponSlotLimitLevel[(int)eWeaponSlot.SUB] > _chardata.Level)
        {
            kSubWeaponSlot.gameObject.SetActive(false);
            SetWeaponInfo(null, kSubWeaponSlot, kSubGemTexList, kSubGemChangeBtnList, kSubGemSlotLockList, _subWeaponSkinId, kSubWeaponSkinDelBtn, kSubSkinTex, kSubSkinLb, subGemSetOptList);
            kLockLabel.transform.parent.gameObject.SetActive(true);
            kLockLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), GameInfo.Instance.GameConfig.CharWeaponSlotLimitLevel[(int)eWeaponSlot.SUB]);
            kSubWeaponEmptyObj.SetActive(true);
            for (int i = 0; i < kSubWeaponLockDisableObjs.Count; i++)
                kSubWeaponLockDisableObjs[i].SetActive(false);
        }
        else
        {
            //서브웨폰 장착중이지 않을때
            if (_subWeaponData == null)
            {
                for (int i = 0; i < kSubWeaponLockDisableObjs.Count; i++)
                    kSubWeaponLockDisableObjs[i].SetActive(false);

                kSubWeaponSlot.gameObject.SetActive(false);
                kSubWeaponEmptyObj.SetActive(true);
            }
            else
            {
                for (int i = 0; i < kSubWeaponLockDisableObjs.Count; i++)
                    kSubWeaponLockDisableObjs[i].SetActive(true);

                kSubWeaponSlot.ParentGO = this.gameObject;
                kSubWeaponSlot.gameObject.SetActive(true);
                kSubWeaponSlot.UpdateSlot(UIItemListSlot.ePosType.EquipWeapon, 0, _subWeaponData);
            }

            SetWeaponInfo(_subWeaponData, kSubWeaponSlot, kSubGemTexList, kSubGemChangeBtnList, kSubGemSlotLockList, _subWeaponSkinId, kSubWeaponSkinDelBtn, kSubSkinTex, kSubSkinLb, subGemSetOptList);
        }

        _SkillCharTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", "Icon/Set/Set_Chara_All.png" );
        _DisableSkillCharSpr.gameObject.SetActive( false );

        //스킬 설명
        if (_mainWeapondata != null && _mainWeapondata.TableData.SkillEffectName > 0)
            {
            kSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(_mainWeapondata.TableData.SkillEffectName);
            kSkillDescLabel.textlocalize = GameSupport.GetWeaponSkillDesc(_mainWeapondata.TableData, _mainWeapondata.SkillLv);
            kSkillLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.SKILL_LEVEL_TXT_NOW_LV, _mainWeapondata.SkillLv, GameSupport.GetMaxSkillLevelCard());
            kSkillLevelLabel.transform.localPosition = new Vector3(kSkillNameLabel.transform.localPosition.x + kSkillNameLabel.printedSize.x + 10, kSkillLevelLabel.transform.localPosition.y, 0);

            if( _mainWeapondata.TableData.WpnBOActivate > 0 ) {
                GameTable.Character.Param characterParam = GameInfo.Instance.GameTable.FindCharacter(_mainWeapondata.TableData.WpnBOActivate);
                if (characterParam != null)
                {
                    _SkillCharTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_Chara_" + characterParam.Icon + ".png");
                }

                _DisableSkillCharSpr.gameObject.SetActive( _mainWeapondata.TableData.WpnBOActivate != _chardata.TableID );
            }
        }
        else
        {
            kSkillNameLabel.textlocalize = "";
            kSkillDescLabel.textlocalize = "";
            kSkillLevelLabel.textlocalize = "";
        }

        //스킬 정보
        if (_mainWeapondata != null)
        {
            kSPIconSpr.gameObject.SetActive(true);
            kSPLabel.gameObject.SetActive(true);
            kSPLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), (_mainWeapondata.TableData.UseSP / 100));
        }
        else
        {
            kSPIconSpr.gameObject.SetActive(false);
            kSPLabel.gameObject.SetActive(false);
        }
    }

	public void OnClick_WeaponOffBtn() {
		if ( IsEnableWeaponOffBtn() ) {
			_chardata.IsHideWeapon = !_chardata.IsHideWeapon;
			PlayerPrefs.SetInt( $"{_chardata.CUID}_IS_HIDE_WEAPON", _chardata.IsHideWeapon ? 1 : 0 );

			SetHideWeapon();
			SetWeaponOffLabel();
		}
	}

    private void SetWeaponOffLabel() {
        if ( _WeaponOffLabel == null ) {
            return;
        }

		_WeaponOffLabel.textlocalize = FLocalizeString.Instance.GetText( _chardata.IsHideWeapon ? 1888 : 1887 );
	}

	private bool IsEnableWeaponOffBtn() {
		System.Enum.TryParse( _chardata.TableData.Icon.ToUpper(), true, out eHideWeaponCharType result );
		if ( result == eHideWeaponCharType.NONE ) {
			return false;
		}

		return true;
	}

	//곡옥 정보 셋팅
	private void SetWeaponInfo(WeaponData weapondata, UIItemListSlot weaponListSlot, List<UITexture> gemTexList, List<UIButton> gemChangeBtnList, List<UISprite> gemSlotLockList, long skinId, UIButton skinDelBtn, UITexture skinIcoTex, UILabel skinNamelb, List<UISprite> gemSetOptList)
    {
        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {
            gemTexList[i].gameObject.SetActive(false);
            gemChangeBtnList[i].gameObject.SetActive(false);
        }

        if (weapondata != null)
        {
            int slotmaxcount = GameSupport.GetWeaponGradeSlotCount(weapondata.TableData.Grade, 2);
            int slotcount = GameSupport.GetWeaponGradeSlotCount(weapondata.TableData.Grade, weapondata.Wake);

            for (int i = 0; i < slotmaxcount; i++)
            {
                if (i >= slotcount)
                    gemSlotLockList[i].gameObject.SetActive(true);
                else
                    gemSlotLockList[i].gameObject.SetActive(false);

                gemChangeBtnList[i].gameObject.SetActive(true);
                GemData gemdata = GameInfo.Instance.GetGemData(weapondata.SlotGemUID[i]);
                if (gemdata != null)
                {
                    gemTexList[i].gameObject.SetActive(true);
                    gemTexList[i].mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + gemdata.TableData.Icon);
                    GameTable.GemSetType.Param gemSetTypeParam = GameInfo.Instance.GameTable.FindGemSetType(gemdata.SetOptID);
                    gemSetOptList[i].SetActive(gemSetTypeParam != null);
                    if (gemSetOptList[i].gameObject.activeSelf)
                    {
                        gemSetOptList[i].spriteName = gemSetTypeParam.Icon;
                    }
                }
                else
                {
                    gemSetOptList[i].SetActive(false);
                }
            }

            //각성 가능하면 레드닷 On
            weaponListSlot.SetActiveNotice(GameSupport.CheckWeaponData(weapondata));
        }

        //스킨 장착중일땐 해제 버튼만 On
        if (skinId != 0)
        {
            skinDelBtn.gameObject.SetActive(true);
            //skinDelBtn.normalSprite = "ico_Close";

            //판매했을수도 있으니...테이블데이터에서 가지고옴
            GameTable.Weapon.Param skinWeaponData = GameInfo.Instance.GameTable.FindWeapon(x => x.ID == skinId);
            if(skinWeaponData != null)
            {
                skinIcoTex.gameObject.SetActive(true);
                skinIcoTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_" + skinWeaponData.Icon);
                skinNamelb.textlocalize = FLocalizeString.Instance.GetText(skinWeaponData.Name);
            }
        }
        else
        {
            skinDelBtn.gameObject.SetActive(false);
            //skinDelBtn.normalSprite = "ico_FigureRoom_Rotation";
            skinIcoTex.gameObject.SetActive(false);
            skinIcoTex.mainTexture = null;
            skinNamelb.textlocalize = string.Empty;
        }
    }

    private bool OnEventMainGemTabSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
        {
            return false;
        }

        if (type == SelectEvent.Click)
        {
            if (nSelect == 1)
            {
                mainGemTooltipObj.SetActive(!mainGemTooltipObj.activeSelf);
                mainGemTooltipLabel.textlocalize = _mainSetOptStr;
            }

            return false;
        }

        return true;
    }

    private bool OnEventSubGemTabSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
        {
            return false;
        }

        if (type == SelectEvent.Click)
        {
            if (nSelect == 1)
            {
                subGemTooltipObj.SetActive(!subGemTooltipObj.activeSelf);
                subGemTooltipLabel.textlocalize = _subSetOptStr;
            }

            return false;
        }

        return true;
    }

    public void OnClick_MainSkinChange()
    {
        if (_mainWeapondata == null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3301));
            return;
        }

        Log.Show("OnClick_MainSkinChange");
        UIValue.Instance.SetValue(UIValue.EParamType.CharEquipWeaponSlot, (int)eWeaponSlot.MAIN);
        UIValue.Instance.SetValue(UIValue.EParamType.ChoicePopupType, (int)eChoicePopupType.SELECT_WEAPON_SKIN);
        LobbyUIManager.Instance.ShowUI("ChoiceitemPopup", true);
        
    }

    public void OnClick_SubSkinChange()
    {
        if (_mainWeapondata == null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3301));
            return;
        }

        Log.Show("OnClick_SubSkinChange", Log.ColorType.Red);
        UIValue.Instance.SetValue(UIValue.EParamType.CharEquipWeaponSlot, (int)eWeaponSlot.SUB);
        UIValue.Instance.SetValue(UIValue.EParamType.ChoicePopupType, (int)eChoicePopupType.SELECT_WEAPON_SKIN);
        LobbyUIManager.Instance.ShowUI("ChoiceitemPopup", true);
        
    }

    public void OnClick_MainSkinDel()
    {
        if (_mainWeapondata == null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3301));
            return;
        }

        GameInfo.Instance.Send_ReqEquipWeaponChar(_chardata.CUID, _chardata.CostumeColor, _chardata.CostumeStateFlag, _chardata.EquipWeaponUID, _chardata.EquipWeapon2UID, 0, _chardata.EquipWeapon2SkinTID, OnSkinChange);
    }

    public void OnClick_SubSkinDel()
    {
        if (_mainWeapondata == null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3301));
            return;
        }

        GameInfo.Instance.Send_ReqEquipWeaponChar(_chardata.CUID, _chardata.CostumeColor, _chardata.CostumeStateFlag, _chardata.EquipWeaponUID, _chardata.EquipWeapon2UID, _chardata.EquipWeaponSkinTID, 0, OnSkinChange);
    }

    public void OnSkinChange(int _result, PktMsgType pktMsg)
    {
        if (_result != 0)
            return;

		SetHideWeapon();

		Renewal(true);
    }

    public void OnClick_WeaponChangeBtn()
	{
        UIValue.Instance.SetValue(UIValue.EParamType.CharEquipWeaponSlot, (int)eWeaponSlot.MAIN);
        LobbyUIManager.Instance.ShowUI("CharWeaponSeletePopup", true);
    }

    public void OnClick_SubWeaponChangeBtn()
    {
        if (_mainWeapondata == null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3301));
            return;
        }

        if (GameInfo.Instance.GameConfig.CharWeaponSlotLimitLevel[(int)eWeaponSlot.SUB] > _chardata.Level)
        {
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3012), GameInfo.Instance.GameConfig.CharWeaponSlotLimitLevel[(int)eWeaponSlot.SUB]));
            return;
        }
        UIValue.Instance.SetValue(UIValue.EParamType.CharEquipWeaponSlot, (int)eWeaponSlot.SUB);
        LobbyUIManager.Instance.ShowUI("CharWeaponSeletePopup", true);
    }

    //SubWeapon Empty Btn
    public void OnClick_AddSubWeapon()
    {
        OnClick_SubWeaponChangeBtn();
    }
    
    public void OnClick_GemChangeBtn(int index)
	{
        int slotcount = GameSupport.GetWeaponGradeSlotCount(_mainWeapondata.TableData.Grade, _mainWeapondata.Wake);
        if (index >= slotcount)
        {
            int i = index - slotcount;
            if (i == 0)
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3114));
            else if (i == 1)
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3115));
            return;
        }

        if(GameInfo.Instance.GemList.Count <= 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1597));
            return;
        }


        if (GameSupport.IsWeaponEquipGemCount(_mainWeapondata.WeaponUID) <= (int)eCOUNT.NONE)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1597));
            return;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, _mainWeapondata.WeaponUID);
        UIValue.Instance.SetValue(UIValue.EParamType.WeaponGemIndex, index);
        LobbyUIManager.Instance.ShowUI("WeaponGemSeletePopup", true);
    }

    public void OnClick_SubGemChangeBtn(int index)
    {
        int slotcount = GameSupport.GetWeaponGradeSlotCount(_subWeaponData.TableData.Grade, _subWeaponData.Wake);
        if (index >= slotcount)
        {
            int i = index - slotcount;
            if (i == 0)
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3114));
            else if (i == 1)
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3115));
            return;
        }

        if(GameInfo.Instance.GemList.Count <= 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1597));
            return;
        }


        if (GameSupport.IsWeaponEquipGemCount(_subWeaponData.WeaponUID) <= (int)eCOUNT.NONE)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1597));
            return;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, _subWeaponData.WeaponUID);
        UIValue.Instance.SetValue(UIValue.EParamType.WeaponGemIndex, index);
        LobbyUIManager.Instance.ShowUI("WeaponGemSeletePopup", true);
    }

    public void OnClick_MainBgCloseBtn()
    {
        // Main Weapon Gem Tooltip Off
        mainGemTooltipObj.SetActive(false);
    }

    public void OnClick_SubBgCloseBtn()
    {
        // Sub Weapon Gem Tooltip Off
        subGemTooltipObj.SetActive(false);
    }

	public void OnNetCharCostumeEquip( int result, PktMsgType pktmsg ) {
        if ( result != 0) {
            return;
        }

		SetHideWeapon();

		Renewal( true );
	}
}
