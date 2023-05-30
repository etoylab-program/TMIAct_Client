using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharInfoPanel : FComponent
{
    public enum eCHARINFOTAB
    {
        NONE = -1,
        STATUS,
        WEAPON,
        SKILL,
        SUPPORTER,
        COSTUME,
        FAVOR,
        COUNT,
    }

    public UITexture kCharTex;
    public FTab kMainTab;
    public List<UISprite> kNoticeList;
    public List<FComponent> kTabList;
    public UILabel kCombatPowerLabel;
    public UIButton kPackageBtn;

    private eCHARINFOTAB _eCharinfotab = eCHARINFOTAB.NONE;
    private CharData _chardata;

    public CharData CharData { get { return _chardata; } }
    public eCHARINFOTAB CharinfoTab { get { return _eCharinfotab;  } }

    private Unit m_rotTarget;
    private bool m_selectCharTex = false;
    private float m_srcRotValue = 0f;
    private float m_destRotValue = 0f;
    private float m_rotSpeed = 10f;

    public override void Awake()
	{
		base.Awake();

        kMainTab.EventCallBack = OnTabMainSelect;
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

		_eCharinfotab = eCHARINFOTAB.NONE;
        RenderTargetChar.Instance.DestroyRenderTarget();
        base.OnDisable();
    }

    public override void InitComponent()
	{
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
        int tableid = (int)UIValue.Instance.GetValue(UIValue.EParamType.CharSelTableID);
        _chardata = GameInfo.Instance.GetCharData(uid);

        for (int i = 0; i < (int)eCHARINFOTAB.COUNT; i++)
        {
            if (kTabList[i] != null)
                kTabList[i].SetUIActive(false, false);
        }

        eCHARINFOTAB tab = eCHARINFOTAB.STATUS;
        object obj = UIValue.Instance.GetValue(UIValue.EParamType.CharInfoTab);
        if (obj != null)
        {
            UIValue.Instance.RemoveValue(UIValue.EParamType.CharInfoTab);

            if (_eCharinfotab == eCHARINFOTAB.NONE)
                tab = (eCHARINFOTAB)obj;
        }

        int charID = _chardata.TableID * 1000;
        int skillTableID = charID + 301;

        RenderTargetChar.Instance.gameObject.SetActive(true);
        RenderTargetChar.Instance.InitRenderTargetChar(_chardata.TableID, _chardata.CUID, false, eCharacterType.Character);

        kMainTab.SetTab((int)tab, SelectEvent.Code);

        m_selectCharTex = false;

        //버프 중 스킬 슬롯 여는게 없으면 마지막 스킬슬롯에 장착되어있는 스킬 해제
        if (!GameSupport.IsActiveUserBuff(eBuffEffectType.Buff_SkillSlot))
        {
            if (_chardata.EquipSkill[(int)eCOUNT.SKILLSLOT - 1] != (int)eCOUNT.NONE)
            {
                _chardata.EquipSkill[(int)eCOUNT.SKILLSLOT - 1] = (int)eCOUNT.NONE;
                GameInfo.Instance.Send_ReqApplySkillInChar(_chardata.CUID, _chardata.EquipSkill, OnNetCharSkillRelease);
            }
        }
        else
        {
            //4번째 스킬슬롯에 오의가 장착되어있으면 해제
            if (_chardata.EquipSkill[(int)eCOUNT.SKILLSLOT - 1] == skillTableID)
            {
                _chardata.EquipSkill[(int)eCOUNT.SKILLSLOT - 1] = (int)eCOUNT.NONE;
                GameInfo.Instance.Send_ReqApplySkillInChar(_chardata.CUID, _chardata.EquipSkill, OnNetCharSkillRelease);
            }
        }
        
        int costumeId = (int)GameInfo.Instance.UserData.DyeingCostumeId;
        if (0 < costumeId)
        {
            kMainTab.SetTab((int)eCHARINFOTAB.COSTUME, SelectEvent.Code);
            
            UICharInfoTabCostumePanel panel = kTabList[(int) eCHARINFOTAB.COSTUME] as UICharInfoTabCostumePanel;
            if (panel == null)
                return;
            
            panel.SetSeleteCostumeID(costumeId);
            
            GameInfo.Instance.Send_ReqUserCostumeColor(costumeId, OnNetDyeingInfoResult);
        }
    }
    
    public void CostumeRestore(bool bRestore)
    {
        UICharInfoTabCostumePanel panel = kTabList[(int) eCHARINFOTAB.COSTUME] as UICharInfoTabCostumePanel;
        if (panel == null)
            return;
        
        if (bRestore)
            panel.SetSeleteCostumeID(_chardata.EquipCostumeID);
        else
            panel.Restore();
    }
    
    private void OnNetDyeingInfoResult(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
        
        GameTable.Costume.Param costume = GameInfo.Instance.GameTable.FindCostume((int) GameInfo.Instance.UserData.DyeingCostumeId);
        if (costume == null)
            return;
        
        CharData charData = GameInfo.Instance.GetCharDataByTableID(costume.CharacterID);
        if (charData == null)
            return;

        UICostumeDyePopup popup = LobbyUIManager.Instance.GetUI("CostumeDyePopup") as UICostumeDyePopup;
        if (popup && !popup.gameObject.activeSelf)
        {
            PktInfoUserCostumeColor pktInfoUserCostumeColor = pktmsg as PktInfoUserCostumeColor;
            if (pktInfoUserCostumeColor == null)
            {
                return;
            }
            
            List<Color> randomColorList = new List<Color>();
            foreach (PktInfoUserCostumeColor.Piece info in pktInfoUserCostumeColor.infos_)
            {
                randomColorList.Add(new Color32(info.color_.red_, info.color_.green_, info.color_.blue_, Byte.MaxValue));
            }
    
            popup.SetRandomColor(randomColorList.ToArray());
            popup.DyeingContinue();
            popup.SetCostume(costume);
            popup.SetUIActive(true);
        }
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        GetRotTarget();

        for (int i = 0; i < kNoticeList.Count; i++)
            kNoticeList[i].gameObject.SetActive(false);

		if (kNoticeList.Count > 0 && GameSupport.IsCharGradeUp(_chardata))
		{
			kNoticeList[0].gameObject.SetActive(true);
		}

        var weapondata = GameInfo.Instance.GetWeaponData(_chardata.EquipWeaponUID);
        if (weapondata != null)
        {
			if (kNoticeList.Count > 1 && GameSupport.CheckWeaponData(weapondata))
			{
				kNoticeList[1].gameObject.SetActive(true);
			}
        }

		if (kNoticeList.Count > 2 &&
			(GameSupport.IsCharPassiveSkillUp(_chardata) || GameSupport.IsCharOpenSkillSlot(_chardata) || GameSupport.IsCharSkillUp(_chardata)))
		{
			kNoticeList[2].gameObject.SetActive(true);
		}

        for( int i = 0; i < (int)eCOUNT.CARDSLOT; i++ )
        {
            var carddata = GameInfo.Instance.GetCardData(_chardata.EquipCard[i]);
            if (carddata != null)
            {
                if (GameSupport.CheckCardData(carddata))
                {
					if (kNoticeList.Count > 3)
					{
						kNoticeList[3].gameObject.SetActive(true);
					}

                    break;
                }
            }
        }

        UnexpectedPackageData unexpectedPackageData = null;
        GameTable.UnexpectedPackage.Param unexpectedPackageParam = GameInfo.Instance.GameTable.FindUnexpectedPackage(x => x.Value1 == _chardata.TableID && x.Value2 == GameInfo.Instance.GameConfig.WakeUPCheckLevel);
        if (unexpectedPackageParam != null)
        {
            foreach (var keyValue in GameInfo.Instance.UnexpectedPackageDataDict)
            {
                unexpectedPackageData = keyValue.Value.Find(x => x.TableId == unexpectedPackageParam.ID);
                if (unexpectedPackageData != null)
                {
                    break;
                }
            }
        }

        kPackageBtn.SetActive(_chardata.Level == GameInfo.Instance.GameConfig.WakeUPCheckLevel && unexpectedPackageData == null);

        //  해당 선택된 탭만 갱신합니다.
        int tabIndex = (int)_eCharinfotab;
		if (tabIndex >= 0 && tabIndex < kTabList.Count && kTabList[(int)_eCharinfotab] != null)
		{
			kTabList[tabIndex].Renewal(bChildren);
		}

        UITopPanel toppanel = LobbyUIManager.Instance.GetActiveUI<UITopPanel>("TopPanel");
        toppanel.Renewal(true);

        kCombatPowerLabel.textlocalize = GameSupport.GetCombatPowerString(_chardata, eWeaponSlot.MAIN);
        
        NotificationManager.Instance.CheckNotification(NotificationManager.eTYPE.ITEM_TAB_FAVOR);
    }

    bool OnTabMainSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Click)
            UIValue.Instance.RemoveValue(UIValue.EParamType.CharCostumeID);

        SetCharInfoTab((eCHARINFOTAB)nSelect);

        return true;
    }

	public void SetCharInfoTab( eCHARINFOTAB etab ) {
		int index = (int)etab;

		if ( kTabList[index].gameObject.activeSelf ) {
			return;
		}

		_eCharinfotab = etab;

		//탭 누를때 로테이션 초기화
		if ( m_rotTarget != null ) {
			m_rotTarget.aniEvent.transform.localRotation = Quaternion.identity;
		}

		for ( int i = 0; i < (int)eCHARINFOTAB.COUNT; i++ ) {
			if ( kTabList[i] != null ) {
				if ( kTabList[i].gameObject.activeSelf )
					kTabList[i].SetUIActive( false, true );
			}
		}

		if ( etab != eCHARINFOTAB.SKILL ) {
			UICharInfoTabSkillPanel skillTab = kTabList[(int)eCHARINFOTAB.SKILL].GetComponent<UICharInfoTabSkillPanel>();
			if ( skillTab != null )
				skillTab.HideSkillLevelUpEffectWithSlot();
		}

		if ( kTabList[index] != null ) {
			kTabList[index].SetUIActive( true, true );
		}
	}

	private void CharSkillCheck()
    {
        bool flag = false;
        int charID = _chardata.TableID * 1000;
        int skillTableID = charID + 301;

        if(_chardata.EquipSkill[(int)eCOUNT.SKILLSLOT-1].Equals(skillTableID))
        {
            _chardata.EquipSkill[(int)eCOUNT.SKILLSLOT - 1] = (int)eCOUNT.NONE;
            GameInfo.Instance.Send_ReqApplySkillInChar(_chardata.CUID, _chardata.EquipSkill, OnNetCharSkillRelease);
            return;
        }

        kTabList[(int)_eCharinfotab].SetUIActive(true, true);
    }

    public void OnNetCharSkillRelease(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

            Log.Show("Skill Out!!!", Log.ColorType.Red);
    }

    public override bool IsBackButton()
    {
        return true;
    }

    void Update()
    {
        //코스튬 변경시 기존에 물고있던게 null이 되기 때문에 한번 검사.
        if (m_rotTarget == null)
            GetRotTarget();

        if (m_rotTarget == null)
            return;

        if(AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Select)) //Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(LobbyUIManager.Instance.uiCamera.cachedCamera.ScreenPointToRay(AppMgr.Instance.CustomInput.GetTouchPos()), out RaycastHit hitInfo, Mathf.Infinity))
            {
                Log.Show(hitInfo.collider.gameObject.name, Log.ColorType.Red);
                if(hitInfo.collider.gameObject.name.Equals(kCharTex.gameObject.name))
                {
                    m_selectCharTex = true;
                }
            }
        }

        if(AppMgr.Instance.CustomInput.GetButton(BaseCustomInput.eKeyKind.Select))
        {
            if(m_selectCharTex)
                CharRotation(AppMgr.Instance.CustomInput.GetTouchPos());
        }
        else
        {
            m_selectCharTex = false;
            m_srcRotValue = m_destRotValue = 0f;
        }

        if(AppMgr.Instance.CustomInput.GetButtonUp(BaseCustomInput.eKeyKind.Select))
        {
            m_selectCharTex = false;
            m_srcRotValue = m_destRotValue = 0f;
        }
    }

    void GetRotTarget()
    {
        if (RenderTargetChar.Instance.Figure != null)
        {
            m_rotTarget = RenderTargetChar.Instance.Figure;
        }
        if (RenderTargetChar.Instance.RenderPlayer != null)
        {
            m_rotTarget = RenderTargetChar.Instance.RenderPlayer;
        }
        if (RenderTargetChar.Instance.RenderEnemy != null)
        {
            m_rotTarget = RenderTargetChar.Instance.RenderEnemy;
        }
    }

	private void CharRotation( Vector2 pos ) {
		m_srcRotValue = pos.x;

		if ( m_destRotValue.Equals( 0.0f ) ) {
			m_destRotValue = m_srcRotValue;
			return;
		}

        Vector3 rot;
        rot.x = m_rotTarget.aniEvent.transform.localRotation.x;
        rot.y = m_rotTarget.aniEvent.transform.localRotation.eulerAngles.y - ( ( m_srcRotValue - m_destRotValue ) * m_rotSpeed * Time.deltaTime );
        rot.z = m_rotTarget.aniEvent.transform.localRotation.z;

        m_rotTarget.aniEvent.transform.localRotation = Quaternion.Euler( rot );

        if ( RenderTargetChar.Instance.AddedWeapon != null && RenderTargetChar.Instance.AddedWeapon is PlayerGuardian ) {
			RenderTargetChar.Instance.AddedWeapon.aniEvent.transform.localRotation = m_rotTarget.aniEvent.transform.localRotation;
		}

		m_destRotValue = m_srcRotValue;
	}

	public void SetDefaultTab()
    {
        _eCharinfotab = eCHARINFOTAB.NONE;
    }

    public void OnClick_PresetBtn()
    {
        if (GameInfo.Instance.CharPresetDataDict.ContainsKey(_chardata.TableID))
        {
            OnNet_PresetList(0, null);
        }
        else
        {
            GameInfo.Instance.Send_ReqGetUserPresetList(ePresetKind.CHAR, _chardata.CUID, OnNet_PresetList);
        }
    }

    public void OnClick_PackageBtn()
    {
        MessagePopup.YN(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3337), () => { GameInfo.Instance.Send_ReqCharLvUnexpectedPackageHardOpen(_chardata.CUID, OnNet_PackageEnable); });
    }

    public void OnNet_PackageEnable(int result, PktMsgType pktmsg)
    {
        Renewal(false);

        LobbyUIManager.Instance.CheckAddSpecialPopup();
        LobbyUIManager.Instance.ShowAddSpecialPopup();
    }

    private void OnNet_PresetList(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        PktInfoUserPreset pktInfoUserPreset = pktmsg as PktInfoUserPreset;
        if (pktInfoUserPreset != null && pktInfoUserPreset.infos_.Count <= 0)
        {
            GameInfo.Instance.SetPresetData(ePresetKind.CHAR, -1, GameInfo.Instance.GameConfig.CharPresetSlot, _chardata.TableID);
        }

        UIPresetPopup presetPopup = LobbyUIManager.Instance.GetUI<UIPresetPopup>("PresetPopup");
        if (presetPopup == null)
        {
            return;
        }

        presetPopup.SetPresetData(eCharSelectFlag.Preset, ePresetKind.CHAR, _chardata.CUID);
        presetPopup.SetUIActive(true);
    }
}
