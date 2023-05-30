using System.Collections.Generic;
using UnityEngine;

public class UICharWeaponSeletePopup : FComponent
{
	public UIButton kBackBtn;
	public UIStatusUnit kCharStatusUnit_ATK;
	public UIStatusUnit kCharStatusUnit_CRI;
	public UISprite kGradeSpr;
	public UILabel kNameLabel;
	public UILabel kLevelLabel;
	public UISprite kWakeSpr;
	public UILabel kSkillNameLabel;
    public UILabel kSkillLevelLabel;
    public UILabel kSkillDescLabel;
    public UISprite kSPIconSpr;
    public UILabel kSPLabel;
    public UIStatusUnit kWeaponStatusUnit_ATK;
	public UIStatusUnit kWeaponStatusUnit_CRI;
    [SerializeField] private UITexture  _SkillCharTex;
    [SerializeField] private UISprite   _DisableSkillCharSpr;
    public List<UISprite> kGemSlotLockList;
    public List<UITexture> kGemTexList;
    public List<UIButton> kGemChangeBtnList;

    public UIButton kChangeBtn;
    public UILabel kChangeBtnLb;
    public UISprite kWeaponSlotSpr;
    public UILabel kWeaponNameLabel;
    [SerializeField] private FList _ItemListInstance;

    [Header("Add Gem UR Grade")]
    [SerializeField] private List<UISprite> gemSetOptList = null;

    private List<WeaponData> _weaponlist = new List<WeaponData>();
    private WeaponData _weapondata;
    private CharData _chardata;
    private int[] _statusAry = new int[(int)eCHARABILITY._MAX_];

    private eWeaponSlot _weaponSlotType;

    public long SeleteWeaponUID
    {
        get
        {
            if (_weapondata == null)
                return -1;
            return _weapondata.WeaponUID;
        }
    }
    public override void Awake()
	{
		base.Awake();

		if(this._ItemListInstance == null) return;
		
		this._ItemListInstance.EventUpdate = this._UpdateItemListSlot;
		this._ItemListInstance.EventGetItemCount = this._GetItemElementCount;
        this._ItemListInstance.InitBottomFixing();

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

        //  UI가 닫힐시 이전 팝업을 갱신합니다(랜더 텍스쳐용)
        var ui = LobbyUIManager.Instance.GetActiveUI<UICharInfoPanel>("CharInfoPanel");
        if(ui != null)
        {
            ui.Renewal(true);
        }
    }

	public override void InitComponent() {
		long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
		_chardata = GameInfo.Instance.GetCharData( uid );

		_weaponlist.Clear();
		for( int i = 0; i < GameInfo.Instance.WeaponList.Count; i++ ) {
			if( GameInfo.Instance.GetEquipWeaponFacilityData( GameInfo.Instance.WeaponList[i].WeaponUID ) != null ) {
				continue;
			}

			if( GameSupport.GetEquipWeaponDepot( GameInfo.Instance.WeaponList[i].WeaponUID ) ) {
				continue;
			}

            bool alreadyEquipAnotherChar = false;

            for( int j = 0; j < GameInfo.Instance.CharList.Count; j++ ) {
                if( ( GameInfo.Instance.WeaponList[i].WeaponUID == GameInfo.Instance.CharList[j].EquipWeaponUID ||
                      GameInfo.Instance.WeaponList[i].WeaponUID == GameInfo.Instance.CharList[j].EquipWeapon2UID ) &&
                      GameInfo.Instance.CharList[j] != _chardata) {
                    alreadyEquipAnotherChar = true;
                    break;
				}

            }

            if( alreadyEquipAnotherChar ) {
                continue;
			}

			for( int j = 0; j < GameInfo.Instance.WeaponList[i].ListCharId.Count; j++ ) {
				if( GameInfo.Instance.WeaponList[i].ListCharId[j] == _chardata.TableID ) {
					if( _chardata.EquipWeaponUID != GameInfo.Instance.WeaponList[i].WeaponUID && 
                        _chardata.EquipWeapon2UID != GameInfo.Instance.WeaponList[i].WeaponUID ) {
						_weaponlist.Add( GameInfo.Instance.WeaponList[i] );
					}
				}
			}

			/*
            if( _chardata.TableID != GameInfo.Instance.WeaponList[i].TableData.CharacterID )
                continue;
            */
		}

		var slotType = UIValue.Instance.GetValue(UIValue.EParamType.CharEquipWeaponSlot);
		if( slotType == null ) {
			Log.Show( "#### Weapon Equip Failed ... ####", Log.ColorType.Red );
			return;
		}

		_weaponSlotType = (eWeaponSlot)slotType;

		WeaponData.SortUp = false;
		_weaponlist.Sort( WeaponData.CompareFuncGradeLevel );

		if( _weaponSlotType == eWeaponSlot.MAIN ) {
			if( _chardata.EquipWeapon2UID != (int)eCOUNT.NONE ) {
				Log.Show( "Weapon2 : " + _chardata.EquipWeapon2UID );
				
                WeaponData tempWeapon = GameInfo.Instance.GetWeaponData(_chardata.EquipWeapon2UID);
                if( tempWeapon != null ) {
                    _weaponlist.Insert( 0, tempWeapon );
                }
			}

			WeaponData equipWeaponData = GameInfo.Instance.GetWeaponData(_chardata.EquipWeaponUID);
			if( equipWeaponData != null ) {
				_weaponlist.Insert( 0, equipWeaponData );
			}
		}
		else if( _weaponSlotType == eWeaponSlot.SUB ) {
			WeaponData equipWeaponData = GameInfo.Instance.GetWeaponData(_chardata.EquipWeaponUID);
			if( equipWeaponData != null ) {
				_weaponlist.Insert( 0, equipWeaponData );
			}

			if( _chardata.EquipWeapon2UID != (int)eCOUNT.NONE ) {
				Log.Show( "Weapon2 : " + _chardata.EquipWeapon2UID );
				
                WeaponData tempWeapon = GameInfo.Instance.GetWeaponData(_chardata.EquipWeapon2UID);
                if( tempWeapon != null ) {
                    _weaponlist.Insert( 0, tempWeapon );
                }
			}
		}

		_weapondata = null;
        if( _weaponlist.Count != 0 ) {
            _weapondata = _weaponlist[0];
        }

        for( int i = 0; i < (int)eCHARABILITY._MAX_; i++ ) {
            _statusAry[i] = 0;
        }

		GameSupport.GetWeaponTotalStat( _chardata.EquipWeaponUID, _chardata.EquipWeapon2UID, ref _statusAry );
		SetChangeBtnText();

		_ItemListInstance.UpdateList();

        foreach (UISprite uiSprite in gemSetOptList)
        {
            uiSprite.SetActive(false);
        }
    }

	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        long nowMainWpnUID = _chardata.EquipWeaponUID;
        long nowSubWpnUID = _chardata.EquipWeapon2UID;
        int[] nowStatusAry = new int[(int)eCHARABILITY._MAX_];

        if (_weapondata != null)
        {
            switch (_weaponSlotType)
            {
                case eWeaponSlot.MAIN:
                    {
                        nowMainWpnUID = _weapondata.WeaponUID;
                        // 보조무기를 선택한 거라면 스위칭
                        if (_chardata.EquipWeapon2UID == _weapondata.WeaponUID)
                        {
                            nowSubWpnUID = _chardata.EquipWeaponUID;
                        }
                    }
                    break;
                case eWeaponSlot.SUB:
                    {
                        nowSubWpnUID = _weapondata.WeaponUID;
                        // 주무기를 선택한 거라면 스위칭
                        if (_chardata.EquipWeaponUID == _weapondata.WeaponUID)
                        {
                            nowMainWpnUID = _chardata.EquipWeapon2UID;
                        }
                    }
                    break;
            }
        }

        GameSupport.GetWeaponTotalStat(nowMainWpnUID, nowSubWpnUID, ref nowStatusAry);

        kCharStatusUnit_ATK.InitStatusUnit((int)eTEXTID.STAT_ATK, _statusAry[(int)eCHARABILITY.ATK], nowStatusAry[(int)eCHARABILITY.ATK]);
        kCharStatusUnit_CRI.InitStatusUnit((int)eTEXTID.STAT_CRI, _statusAry[(int)eCHARABILITY.CRI], nowStatusAry[(int)eCHARABILITY.CRI]);

        kWeaponSlotSpr.spriteName = string.Format("ico_WeaponSet_{0}", (int)_weaponSlotType);

        if (_weapondata != null)
        {
            if (_weaponSlotType == eWeaponSlot.MAIN)
                kWeaponNameLabel.textlocalize = FLocalizeString.Instance.GetText(_weapondata.TableData.Name);
            else
                kWeaponNameLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(283), FLocalizeString.Instance.GetText(_weapondata.TableData.Name), _weapondata.SkillLv, GameInfo.Instance.GameConfig.WeaponSubSlotStatBySLV[_weapondata.SkillLv] * (float)eCOUNT.MAX_BO_FUNC_VALUE);

            kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_weapondata.TableData.Name);
            kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _weapondata.Level, GameSupport.GetWeaponMaxLevel(_weapondata));

            kGradeSpr.spriteName = "itemgrade_L_" + _weapondata.TableData.Grade.ToString();
            //kGradeSpr.MakePixelPerfect();

            kWakeSpr.spriteName = "itemwake_0" + _weapondata.Wake.ToString();
            kWakeSpr.MakePixelPerfect();

            kWeaponStatusUnit_ATK.InitStatusUnit((int)eTEXTID.STAT_ATK, _weapondata.GetWeaponATK());
            kWeaponStatusUnit_CRI.InitStatusUnit((int)eTEXTID.STAT_CRI, _weapondata.GetWeaponCRI());
        }
        else
        {
            kWeaponNameLabel.textlocalize = string.Empty;

            kNameLabel.textlocalize = string.Empty;
            kLevelLabel.textlocalize = string.Empty;

            kGradeSpr.spriteName = "itemgrade_L_1";
            //kGradeSpr.MakePixelPerfect();

            kWakeSpr.spriteName = "itemwake_00";
            kWakeSpr.MakePixelPerfect();

            kWeaponStatusUnit_ATK.InitStatusUnit((int)eTEXTID.STAT_ATK, 0);
            kWeaponStatusUnit_CRI.InitStatusUnit((int)eTEXTID.STAT_CRI, 0);
        }

        _SkillCharTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", "Icon/Set/Set_Chara_All.png" );
        _DisableSkillCharSpr.gameObject.SetActive( false );

        if (_weapondata != null && _weapondata.TableData.SkillEffectName > 0)
        {
            kSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(_weapondata.TableData.SkillEffectName);
            kSkillDescLabel.textlocalize = GameSupport.GetWeaponSkillDesc(_weapondata.TableData, _weapondata.SkillLv);
            kSkillLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.SKILL_LEVEL_TXT_NOW_LV, _weapondata.SkillLv, GameSupport.GetMaxSkillLevelCard());
            kSkillLevelLabel.transform.localPosition = new Vector3(kSkillNameLabel.transform.localPosition.x + kSkillNameLabel.printedSize.x + 10, kSkillLevelLabel.transform.localPosition.y, 0);

            if( _weapondata.TableData.WpnBOActivate > 0 ) {
                GameTable.Character.Param characterParam = GameInfo.Instance.GameTable.FindCharacter(_weapondata.TableData.WpnBOActivate);
                if (characterParam != null)
                {
                    _SkillCharTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Set/Set_Chara_" + characterParam.Icon + ".png");
                }

                _DisableSkillCharSpr.gameObject.SetActive( _weapondata.TableData.WpnBOActivate != _chardata.TableID );
            }
        }
        else
        {
            kSkillNameLabel.textlocalize = "";
            kSkillDescLabel.textlocalize = "";
            kSkillLevelLabel.textlocalize = "";
        }

        for (int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++)
        {
            kGemTexList[i].gameObject.SetActive(false);
            kGemChangeBtnList[i].gameObject.SetActive(false);
        }

        int slotmaxcount = 0;
        int slotcount = 0;
        if (_weapondata != null)
        {
            slotmaxcount = GameSupport.GetWeaponGradeSlotCount(_weapondata.TableData.Grade, 2);
            slotcount = GameSupport.GetWeaponGradeSlotCount(_weapondata.TableData.Grade, _weapondata.Wake);
        }

        for (int i = 0; i < slotmaxcount; i++)
        {
            if (i >= slotcount)
                kGemSlotLockList[i].gameObject.SetActive(true);
            else
                kGemSlotLockList[i].gameObject.SetActive(false);

            kGemChangeBtnList[i].gameObject.SetActive(true);
            GemData gemdata = GameInfo.Instance.GetGemData(_weapondata.SlotGemUID[i]);
            if (gemdata != null)
            {
                kGemTexList[i].gameObject.SetActive(true);
                kGemTexList[i].mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + gemdata.TableData.Icon);
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

        if (_weapondata != null && _weapondata.TableData.UseSP <= 0)
        {
            kSPIconSpr.gameObject.SetActive(false);
            kSPLabel.gameObject.SetActive(false);
        }
        else
        {
            kSPIconSpr.gameObject.SetActive(true);
            kSPLabel.gameObject.SetActive(true);
            if (_weapondata != null)
            {
                kSPLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), (_weapondata.TableData.UseSP / 100));
            }
            else
            {
                kSPLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), 0);
            }
        }

        _ItemListInstance.RefreshNotMove();
    }
 	
	private void _UpdateItemListSlot(int index, GameObject slotObject)
	{
		do
		{
            UIItemListSlot card = slotObject.GetComponent<UIItemListSlot>();
            if (null == card) break;

            WeaponData data = null;
            if (0 <= index && _weaponlist.Count > index)
            {
                data = _weaponlist[index];
            }

            card.ParentGO = this.gameObject;
            card.UpdateSlot(UIItemListSlot.ePosType.Weapon_SeleteList, index, data);
        } while(false);
	}
	
	private int _GetItemElementCount()
	{
		return _weaponlist.Count;
	}

    public void SetSeleteWeaponUID(long uid)
    {
        var weapondata = GameInfo.Instance.GetWeaponData(uid);
        if (weapondata == null)
            return;
        _weapondata = weapondata;

        SetChangeBtnText();

        Renewal(true);
    }

    private void SetChangeBtnText()
    {
        kChangeBtnLb.textlocalize = FLocalizeString.Instance.GetText(13);

        if (_weaponSlotType == eWeaponSlot.SUB)
        {
            //보조무기는 해제도 가능하기에 텍스트 변경
            if (_weapondata.WeaponUID == _chardata.EquipWeapon2UID)
                kChangeBtnLb.textlocalize = FLocalizeString.Instance.GetText(14);
        }
    }

    public void OnClick_BackBtn()
	{
        OnClickClose();
	}

    public void OnClick_GemChangeBtn(int index)
    {
        int slotcount = GameSupport.GetWeaponGradeSlotCount(_weapondata.TableData.Grade, _weapondata.Wake);
        if (index >= slotcount)
        {
            int i = index - slotcount;
            if (i == 0)
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3114));
            else if (i == 1)
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3115));
            return;
        }
        UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, _weapondata.WeaponUID);
        UIValue.Instance.SetValue(UIValue.EParamType.WeaponGemIndex, index);
        LobbyUIManager.Instance.ShowUI("WeaponGemSeletePopup", true);
    }

    public void OnClick_ChangeBtn()
	{
        if (_weapondata == null)
            return;

        long mainSlotUID = _chardata.EquipWeaponUID;
        long subSlotUID = _chardata.EquipWeapon2UID;

        int mainTID = 0;
        if (0 < mainSlotUID)
        {
            mainTID = GameInfo.Instance.GetWeaponData(mainSlotUID).TableID;
        }
        int subTID = 0;
        if (subSlotUID != (int)eCOUNT.NONE)
            subTID = GameInfo.Instance.GetWeaponData(subSlotUID).TableID;

        switch(_weaponSlotType)
        {
            case eWeaponSlot.MAIN:
                {
                    

                    //주무기 그대로 선택 - 창 닫기
                    if (mainSlotUID == _weapondata.WeaponUID)
                    {
                        OnClick_BackBtn();
                        return;
                    }
                    //보조무기 선택
                    if (subSlotUID == _weapondata.WeaponUID)
                    {
                        //교체
                        long tempUID = mainSlotUID;
                        mainSlotUID = subSlotUID;
                        subSlotUID = tempUID;
                    }
                    else
                    {
                        if (subTID == _weapondata.TableID)
                        {
                            //주무기, 보조무기 같은무기 장착 제한
                            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3157));
                            return;
                        }
                        else
                            mainSlotUID = _weapondata.WeaponUID;
                    }
                }
                break;
            case eWeaponSlot.SUB:
                {
                    

                    //보조무기 그대로 선택 - 해제
                    if (subSlotUID == _weapondata.WeaponUID)
                    {
                        _chardata.EquipWeapon2SkinTID = (int)eCOUNT.NONE;
                        subSlotUID = (int)eCOUNT.NONE;
                    }
                    else if (mainSlotUID == _weapondata.WeaponUID)
                    {
                        if(subSlotUID == (int)eCOUNT.NONE)
                        {
                            //주무기는 해제할 수 없음
                            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3156));
                            return;
                        }
                        else
                        {
                            //교체
                            long tempUID = mainSlotUID;
                            mainSlotUID = subSlotUID;
                            subSlotUID = tempUID;
                        }
                    }
                    else
                    {
                        if (mainTID == _weapondata.TableID)
                        {
                            //주무기, 보조무기 같은무기 장착 제한
                            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3157));
                            return;
                        }
                        else
                            subSlotUID = _weapondata.WeaponUID;
                    }
                }
                break;
        }
        
        Log.Show(_weaponSlotType);

        GameInfo.Instance.Send_ReqEquipWeaponChar(_chardata.CUID, _chardata.CostumeColor, _chardata.CostumeStateFlag, mainSlotUID, subSlotUID, _chardata.EquipWeaponSkinTID, _chardata.EquipWeapon2SkinTID, OnNetWeaponCharEquip);
    }

    public void OnNetWeaponCharEquip(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("CharInfoPanel");
        LobbyUIManager.Instance.Renewal("PresetPopup");

        OnClickClose();

        if (LobbyUIManager.Instance.GetActiveUI<UICharInfoTabWeaponPanel>("CharInfoTabWeaponPanel") != null)
        {
			RenderTargetChar.Instance.SetCostumeWeapon( _chardata, !_chardata.IsHideWeapon );
			RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Lobby_Weapon, 0, eFaceAnimation.Weapon, 0);
            VoiceMgr.Instance.PlayChar(eVOICECHAR.ChangeWeapon, _chardata.TableID);
        }
    }
}
