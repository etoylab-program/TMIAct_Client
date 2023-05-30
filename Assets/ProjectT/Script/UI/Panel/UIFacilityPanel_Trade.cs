using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class UIFacilityPanel : FComponent
{
    private void InitTradeInfo()
    {
        if (_facilitydata != null &&
            _facilitydata.TableData.EffectType == "FAC_CARD_TRADE")
        {
            _facilitydata.Selete = 0;
            GameInfo.Instance.ClearTradeMaterialUID();
        }
    }
    private void SetTradeObject(bool state)
    {
        kTradeTimeTitleLabel.SetActive(state);
        kTradeTimeDescLabel.SetActive(state);
        kTradeTimeGauge.SetActive(state);
        kTradeIconTexture.SetActive(state);
        kTradeCompleteObj.SetActive(state);
        kTradeReadyObj.SetActive(state);
        kTradePercentLabel.SetActive(state);

        kTradeItemMaterialObj.SetActive(state);
        if (kTradeItemList != null)
        {
            for (int i = 0; i < kTradeItemList.Count; i++)
            {
                if (kTradeItemList[i] != null) kTradeItemList[i].SetActive(state);
            }
        }

        if (kTradeItemEmptyList != null)
        {
            for (int i = 0; i < kTradeItemEmptyList.Count; i++)
            {
                if (kTradeItemEmptyList[i] != null) kTradeItemEmptyList[i].SetActive(state);
            }
        }

        if (kTradeItemLockList != null)
        {
            for (int i = 0; i < kTradeItemLockList.Count; i++)
            {
                if (kTradeItemLockList[i] != null) kTradeItemLockList[i].SetActive(state);
            }
        }
    }

    private void Renewal_Trade()
    {
        InitTradeType();

        kInfo.gameObject.SetActive(true);
        kInfoNormalObj.SetActive(false);
        kInfoTradeObj.SetActive(true);
        kInfoEffectTitleLabel.gameObject.SetActive(true);
        kInfoEffectDescLabel.gameObject.SetActive(true);

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_facilitydata.TableData.Name);
        kLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), _facilitydata.Level);
        kLevelLabel.transform.localPosition = new Vector3(kNameLabel.transform.localPosition.x + kNameLabel.printedSize.x + 10, kNameLabel.transform.localPosition.y, 0);

        //효과 타이틀 및 적용 효과값 텍스트 적용 및 좌표 수정
        kInfoEffectTitleLabel.textlocalize = FLocalizeString.Instance.GetText(_facilitydata.TableData.EffectDesc);
        kInfoEffectDescLabel.textlocalize = string.Format("{0:#,##0}", GameSupport.GetFacilityEffectValue(_facilitydata, true));
        kInfoEffectDescLabel.transform.localPosition = new Vector3(kInfoEffectTitleLabel.transform.localPosition.x + kInfoEffectTitleLabel.printedSize.x + 10, kInfoEffectTitleLabel.transform.localPosition.y, 0);

        Renewal_TradeSupporter();
        Renewal_TradeMaterialInfo();

        kTradeTimeResetLabel.textlocalize = FLocalizeString.Instance.GetText(1658);

        switch ((eFACILITYSTATS)_facilitydata.Stats)
        {
            case eFACILITYSTATS.WAIT: Renewal_Trade_WAIT(); break;
            case eFACILITYSTATS.USE: Renewal_Trade_USE(); break;
            case eFACILITYSTATS.COMPLETE: Renewal_Trade_COMPLETE(); break;
        }
        
        kFunctionSelectToggle.SetToggle(_selectAddOnParam == null ? 0 : 1, SelectEvent.Code);
    }
    private void Renewal_TradeSupporter()
    {
        _strTimeInfoColor = FLocalizeString.Instance.GetText((int)eTEXTID.WHITE_TEXT_COLOR);
        CardData carddata = GameInfo.Instance.GetCardData(_facilitydata.EquipCardUID);

        if (carddata != null)
        {
            _strTimeInfoColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);

            kTradeIconTexture.SetActive(true);
            kTradeIconTexture.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Item/{0}.png", carddata.TableData.Icon));

            kTradeIconBgSprite.gameObject.SetActive(true);
            kTradeIconBgSprite.spriteName = "itembgSlot_weapon_" + carddata.TableData.Grade.ToString();
        }

        //효과 타이틀 및 적용 효과값 텍스트 적용 및 좌표 수정
        kInfoEffectTitleLabel.textlocalize = FLocalizeString.Instance.GetText(_facilitydata.TableData.EffectDesc);
        kInfoEffectDescLabel.textlocalize = string.Format("{0:#,##0}", GameSupport.GetFacilityEffectValue(_facilitydata, true));
        kInfoEffectDescLabel.transform.localPosition = new Vector3(kInfoEffectTitleLabel.transform.localPosition.x + kInfoEffectTitleLabel.printedSize.x + 10, kInfoEffectTitleLabel.transform.localPosition.y, 0);


        kTradeTimeTitleLabel.SetActive(true);
        kTradeTimeDescLabel.SetActive(true);

        kTradeTimeTitleLabel.textlocalize = FLocalizeString.Instance.GetText(1654);
        float time = GameSupport.GetFacilityTime(_facilitydata) * 60;
        kTradeTimeDescLabel.textlocalize = string.Format(_strTimeInfoColor, GameSupport.GetFacilityTimeString(time));

    }

    private void Renewal_TradeMaterialInfo()
    {
        // Empty/Lock 슬롯 상태 설정
        int itemMaxCount = kTradeItemList.Count;
        int itemEmptyCount = _facilitydata.TableData.EffectValue + (_facilitydata.Level - 1);
        int NextReqLevel = 1;

        for (int i = 0; i < itemMaxCount; i++)
        {
            if (i < itemEmptyCount)
            {
                // 재료데이터 설정에 따른 Empty 슬로 상태 설정
                if (GameInfo.Instance.TradeMaterialUIDList.Length == 0)
                {
                    kTradeItemEmptyList[i].SetActive(true);
                }
                else if (i >= GameInfo.Instance.TradeMaterialUIDList.Length || GameInfo.Instance.TradeMaterialUIDList[i] <= 0)
                    kTradeItemEmptyList[i].SetActive(true);
            }
            else
            {
                kTradeItemLockList[i].SetActive(true);
                kTradeItemLockLabelList[i].textlocalize = string.Format(FLocalizeString.Instance.GetText(1365), _facilitydata.Level + NextReqLevel++);
            }
        }
        
        // 재료 아이템 슬롯 
        for (int i = 0; i < GameInfo.Instance.TradeMaterialUIDList.Length; i++)
        {
            if (GameInfo.Instance.TradeMaterialUIDList[i] <= 0) continue;
            if (kTradeItemList[i] == null) continue;
            UIItemListSlot slot = kTradeItemList[i].GetComponent<UIItemListSlot>();
            if (slot == null) continue;            

            if (eCurTradeType == eTradeType.CARD)
            {
                CardData w = GameInfo.Instance.CardList.Find(x => x.CardUID== GameInfo.Instance.TradeMaterialUIDList[i]);
                if (w != null)
                {
                    kTradeItemList[i].SetActive(true);
                    slot.UpdateSlot(UIItemListSlot.ePosType.None, i, w);
                }
            }
            else if (eCurTradeType == eTradeType.WEAPON)
            {
                WeaponData w = GameInfo.Instance.WeaponList.Find(x => x.WeaponUID == GameInfo.Instance.TradeMaterialUIDList[i]);
                if (w != null)
                {
                    kTradeItemList[i].SetActive(true);                    
                    slot.UpdateSlot(UIItemListSlot.ePosType.None, i, w);
                }
            }

        }
    }

    private void Renewal_Trade_WAIT()
    {
        Debug.Log("Renewal_Trade_WAIT");
        
        kTradeTimeGauge.SetActive(true);
        kTradeReadyObj.SetActive(true);
        kTradePercentLabel.SetActive(true);
        kTradeItemMaterialObj.SetActive(true);

        kTradeTimeGauge.InitGaugeUnit(0f);

        //등급업 성공 확률
        SetSuccessProb();

    }

    private void Renewal_Trade_USE()
    {
        Debug.Log("Renewal_Trade_USE");

        kTradeTimeGauge.SetActive(true);        
        kTradeCompleteObj.SetActive(true);
        kTradePercentLabel.SetActive(true);
        kTradeItemMaterialObj.SetActive(true);
    }

    private void Renewal_Trade_COMPLETE()
    {
        Debug.Log("Renewal_Trade_COMPLETE");

        kTradeTimeGauge.SetActive(true);
        kTradeCompleteObj.SetActive(true);
        kTradePercentLabel.SetActive(true);
        kTradeItemMaterialObj.SetActive(true);

        kTradeTimeGauge.InitGaugeUnit(1f);
        kTradeTimeDescLabel.textlocalize = string.Format(_strTimeInfoColor, FLocalizeString.Instance.GetText(1125));
    }

	public void OnClick_TradeStart() {
        if( _facilitydata == null || _facilitydata.Level == 0 || _facilitydata.Stats != (int)eFACILITYSTATS.WAIT ) {
            return;
        }

		if( GameInfo.Instance.IsEmptyTradeMaterialUID() ) {
			//2장 이상의 재료를 선택해야 합니다.
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3237 ) );
			return;
		}

		eContentsPosKind eKind = eContentsPosKind.CARD;
        if( eCurTradeType == eTradeType.WEAPON ) {
            eKind = eContentsPosKind.WEAPON;
        }

		int MaterialCount = GameInfo.Instance.GetListTradeMaterialUID().Count;
		if( MaterialCount <= 0 ) {
			//2장 이상의 재료를 선택해야 합니다.
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3237 ) );
			return;
		}

		int CurGrade = 0;
		if( eCurTradeType == eTradeType.CARD ) {
			CardData targetData = GameInfo.Instance.CardList.Find( x => x.CardUID == (long)GameInfo.Instance.GetListTradeMaterialUID()[0] );
            if( targetData == null ) {
                return;
            }

			CurGrade = targetData.TableData.Grade;
		}
		else if( eCurTradeType == eTradeType.WEAPON ) {
			WeaponData targetData = GameInfo.Instance.WeaponList.Find( x => x.WeaponUID == (long)GameInfo.Instance.GetListTradeMaterialUID()[0] );
            if( targetData == null ) {
                return;
            }

			CurGrade = targetData.TableData.Grade;
		}

        if( CurGrade <= 0 ) {
            return;
        }

		var tradeTable = GameInfo.Instance.GameTable.FindFacilityTrade( x => x.MaterialGrade == CurGrade && x.MaterialCount == MaterialCount );
		if( tradeTable == null ) {
			if( CurGrade == (int)eGRADE.GRADE_UR ) {
				//UR 등급은 3장으로만 교환을 진행할 수 있습니다.
				MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3236 ) );
			}
			else if( MaterialCount < 2 ) {
				//2장 이상의 재료를 선택해야 합니다.
				MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3237 ) );
			}

			return;
		}

		// 추가 아이템으로 지정한 대마인이 한정 무기 지급 가능한지 체크
		if( eCurTradeType == eTradeType.WEAPON && 4 <= MaterialCount && CurGrade == (int)eGRADE.GRADE_UR ) {
			if( _selectAddOnParam != null && _selectAddOnParam.AddFuncType == (int)eTRADE_ADDON_TYPE.WEAPON_CHAR_SET ) {
				bool isLimitedWeapon = false;

				string[] split = null;
				for( int i = 0; i < GameInfo.Instance.GameTable.Weapons.Count; i++ ) {
					split = Utility.Split( GameInfo.Instance.GameTable.Weapons[i].CharacterID, ',' );

					for( int j = 0; j < split.Length; j++ ) {
						if( Utility.SafeIntParse( split[j] ) == _selectAddOnParam.AddFuncValue && GameInfo.Instance.GameTable.Weapons[i].Tradeable == 2 ) {
							isLimitedWeapon = true;
						}
					}
				}

                /*
                bool isLimitedWeapon = GameInfo.Instance.GameTable.Weapons.Any(x =>
                    x.CharacterID == _selectAddOnParam.AddFuncValue && x.Tradeable == 2);
                */

                if( isLimitedWeapon == false ) {
					MessagePopup.OK( eTEXTID.OK, FLocalizeString.Instance.GetText( 200064 ), null );
					return;
				}
			}
		}

		FacilityDataDenominator = GameSupport.GetFacilityTime( _facilitydata ) * 60;

		_precardbooklist.Clear();
		_precardbooklist.AddRange( GameInfo.Instance.CardBookList );

		GameInfo.Instance.Send_ReqFacilityOperation_Trade( _facilitydata.TableID, tradeTable.ID, 1, GameInfo.Instance.GetListTradeMaterialUID(), eKind, 
                                                           _selectAddOnParam?.ID ?? 0, OnNet_AckFacilityOperation_Trade );
	}

	public void OnClick_TradeGuide()
    {
        if (_facilitydata == null ||
           _facilitydata.Level == 0 ||
           _facilitydata.Stats != (int)eFACILITYSTATS.WAIT)
            return;

        LobbyUIManager.Instance.ShowUI("FacilityTradeDetailPopup", true);
    }

    public void OnClick_TradeTypeChange()
    {
        _selectAddOnParam = null;
        
        if (_facilitydata == null ||
            _facilitydata.Level == 0 ||
            _facilitydata.Stats != (int)eFACILITYSTATS.WAIT)
            return;

        switch (eCurTradeType)
        {
            case eTradeType.CARD: eCurTradeType = eTradeType.WEAPON; break;
            case eTradeType.WEAPON: eCurTradeType = eTradeType.CARD; break;
        }
        
        GameInfo.Instance.ClearTradeMaterialUID();

        SetTradeType(eCurTradeType);

        Renewal(false);
    }

    public bool GradeExcessCheck(object item, UIItemListSlot.ePosType type)
    {
        bool result = false;
        bool bItem = true;
        int itemGrade = -1;
        switch (type)
        {
            case UIItemListSlot.ePosType.FacilityMaterial_Card:
            {
                if (item is CardData cardData)
                {
                    itemGrade = cardData.TableData.Grade;
                }
                break;
            }
            case UIItemListSlot.ePosType.FacilityMaterial_Weapon:
            {
                if (item is WeaponData weaponData)
                {
                    itemGrade = weaponData.TableData.Grade;
                }
                break;
            }
            default:
            {
                bItem = false;
                break;
            }
        }

        if (bItem)
        {
            if (_selectAddOnParam != null && 0 < _selectAddOnParam.UseGrade)
            {
                result = _selectAddOnParam.UseGrade <= itemGrade;
            }
        }
        else
        {
            if (item is GameTable.FacilityTradeAddon.Param addOnParam)
            {
                if (0 < addOnParam.UseGrade)
                {
                    foreach(UIItemListSlot tradeItem in kTradeItemList)
                    {
                        if (tradeItem.gameObject.activeSelf == false)
                        {
                            continue;
                        }

                        if (tradeItem.CardData != null)
                        {
                            itemGrade = tradeItem.CardData.TableData.Grade;
                        }
                        else if (tradeItem.WeaponData != null)
                        {
                            itemGrade = tradeItem.WeaponData.TableData.Grade;
                        }

                        result = addOnParam.UseGrade <= itemGrade;
                        if (result)
                        {
                            break;
                        }
                    }
                }
            }
        }

        return result;
    }
    
    public void OnClick_TradeMaterial(int idx)
    {
        if (_facilitydata == null ||
            _facilitydata.Level == 0 ||
            _facilitydata.Stats != (int)eFACILITYSTATS.WAIT)
            return;

        if (eCurTradeType == eTradeType.NONE)
            return;

        if (eCurTradeType == eTradeType.CARD)
        {
            //서포터 목록에 표시할게 있는지 체크
            bool cardFlag = false;
            for (int i = 0; i < GameInfo.Instance.CardList.Count; i++)
            {
                if (_facilitydata.EquipCardUID == GameInfo.Instance.CardList[i].CardUID ||
                   (GameSupport.IsEquipAndUsingCardData(GameInfo.Instance.CardList[i].CardUID) == false &&
                   GameInfo.Instance.GetEquiCardCharData(GameInfo.Instance.CardList[i].CardUID) == null))
                {
                    cardFlag = true;
                    break;
                }
            }

            if (!cardFlag)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3038));
                return;
            }

            UIValue.Instance.SetValue(UIValue.EParamType.FacilityID, _facilitydata.TableID);
            UIValue.Instance.SetValue(UIValue.EParamType.TradeMaterial, idx);

            LobbyUIManager.Instance.ShowUI("FacilityCardSeletePopup", true);
        }
        else if (eCurTradeType == eTradeType.WEAPON)
        {   
            ShowFacilityWeaponSeletePopup(idx);
        }
    }


    private void InitTradeType()
    {
        eCurTradeType = (eTradeType)PlayerPrefs.GetInt(TradeTypePrefsKey, 0);

        if (eCurTradeType == eTradeType.NONE) eCurTradeType = eTradeType.CARD;

        SetTradeType(eCurTradeType);
    }

    private void SetTradeType(eTradeType eType)
    {
        PlayerPrefs.SetInt(TradeTypePrefsKey, (int)eType);

        SetTradeItemEmptySprite();
    }

    private void SetTradeItemEmptySprite()
    {
        string SpriteName = "ico_Supporter";
        if (eCurTradeType == eTradeType.WEAPON)
            SpriteName = "ico_Weapon";

        for (int i = 0; i < kTradeItemEmptySprList.Count; i++)
        {
            if (kTradeItemEmptyList[i] == null) continue;
            kTradeItemEmptySprList[i].spriteName = SpriteName;
        }
    }

    public void ShowFacilityWeaponSeletePopup(int idx)
    {
        if (GameInfo.Instance.WeaponList.Count == 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3020));
            return;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.FacilityID, _facilitydata.TableID);
        UIValue.Instance.SetValue(UIValue.EParamType.TradeMaterial, idx);

        LobbyUIManager.Instance.ShowUI("FacilityWeaponSeletePopup", true);
    }

    List<RewardData> rewards = new List<RewardData>();
    public void OnNet_AckFacilityOperation_Trade(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        GameInfo.Instance.ClearTradeMaterialUID();
        PktInfoFacilityOperationAck pAck = pktmsg as PktInfoFacilityOperationAck;

        rewards.Clear();
        if (eCurTradeType == eTradeType.CARD)
        {
            for (int i = 0; i < pAck.products_.cards_.infos_.Count; i++)
            {
                if (pAck.products_.cards_.infos_[i] == null) continue;
                var piece = pAck.products_.cards_.infos_[i];
                RewardData data = new RewardData((long)piece.cardUID_, (int)eREWARDTYPE.CARD, (int)piece.tableID_, 1, false);

                CardBookData bookdata = _precardbooklist.Find(x => x.TableID == data.Index);
                if (bookdata == null)
                    data.bNew = true;

                rewards.Add(data);
            }
        }
        else if (eCurTradeType == eTradeType.WEAPON)
        {
            if (pAck.products_.weapons_.infos_.Count > 0)
            {
                //새롭게 얻은 아이템                
                for (int i = 0; i < pAck.products_.weapons_.infos_.Count; i++)
                {
                    if (pAck.products_.weapons_.infos_[i] == null) continue;
                    var piece = pAck.products_.weapons_.infos_[i];
                    RewardData data = new RewardData((long)piece.weaponUID_, (int)eREWARDTYPE.WEAPON, (int)piece.tableID_, 1, false);
                    rewards.Add(data);
                }
            }
        }

        foreach (PktInfoAddItem.Piece addItem in pAck.products_.addItemInfos_.infos_)
        {
            long itemUid = 0;
            ItemData itemData = GameInfo.Instance.GetItemData((int)addItem.tid_);
            if (itemData != null)
            {
                itemUid = itemData.ItemUID;
            }
            else
            {
                foreach (PktInfoItem.Piece item in pAck.products_.items_.infos_)
                {
                    if (item.tableID_.Equals(addItem.tid_))
                    {
                        itemUid = (long)item.itemUID_;
                        break;
                    }
                }
            }
            rewards.Add(new RewardData(itemUid, (int)eREWARDTYPE.ITEM, (int)addItem.tid_, (int)addItem.addCnt_, false));
        }

        if (rewards.Count > 0)
            StartCoroutine(CoRewardListMessage());
    }

    private IEnumerator CoRewardListMessage()
    {
        LobbyFacility facility =  Lobby.Instance.GetLobbyFacility(_facilitydata.TableID);

        facility.SetAudioClip("Sound/UI/ui_facility05_laser.wav");
        Lobby.Instance.ActivationRoomEffect(_facilitydata.TableID, 3);
        yield return new WaitForSeconds(1.8f);
        Lobby.Instance.DisabledRoomEffect(_facilitydata.TableID, 3);

        string message = string.Empty;
        if (_selectAddOnParam != null)
        {
            if (_selectAddOnParam.AddFuncType == (int)eTRADE_ADDON_TYPE.DECOMPOSITION)
            {
                message = FLocalizeString.Instance.GetText(1814);
            }

            _selectAddOnParam = null;
        }
        
        MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText(1657), message, rewards, 
            () => 
            {
                if (eCurTradeType == eTradeType.CARD)
                {
                    DirectorUIManager.Instance.PlayNewCardGreeings(_precardbooklist);
                }
            });

        yield return null;
        facility.SetAudioClip("Sound/UI/ui_facility05_loop.wav");
        Lobby.Instance.ActivationRoomEffect(_facilitydata.TableID);

        Renewal(true);
    }

    private void SetSuccessProb()
    {
        List<ulong> materialUIDs = GameInfo.Instance.GetListTradeMaterialUID();        
        int Percent = 0;

        Action<int> SetLabel = (p) =>
        {
            if (_selectAddOnParam != null)
            {
                if (_selectAddOnParam.AddFuncType == (int)eTRADE_ADDON_TYPE.PROBABILITY_UP)
                {
                    p += _selectAddOnParam.AddFuncValue / 100 * p;
                }
            }
            kTradePercentLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1652), p);
        };

        if (materialUIDs.Count < 1)
        {
            SetLabel(Percent);            
            return;
        }

        int selectGrade = 0;
        switch (eCurTradeType)
        {
            case eTradeType.CARD:
                {
                    CardData w1 = GameInfo.Instance.CardList.Find(x => x.CardUID == (long)materialUIDs[0]);
                    for (int i = 1; i < materialUIDs.Count; i++)
                    {
                        CardData w2 = GameInfo.Instance.CardList.Find(x => x.CardUID == (long)materialUIDs[i]);
                        if (w1.TableData.Grade != w2.TableData.Grade)
                        {
                            SetLabel(Percent);
                            return;
                        }
                    }
                    selectGrade = w1.TableData.Grade;
                }
                break;
            case eTradeType.WEAPON:
                {
                    WeaponData w1 = GameInfo.Instance.WeaponList.Find(x => x.WeaponUID == (long)materialUIDs[0]);
                    for (int i = 1; i< materialUIDs.Count; i++)
                    {
                        WeaponData w2 = GameInfo.Instance.WeaponList.Find(x => x.WeaponUID == (long)materialUIDs[i]);
                        if(w1.TableData.Grade != w2.TableData.Grade)
                        {
                            SetLabel(Percent);
                            return;
                        }
                    }
                    selectGrade = w1.TableData.Grade;                    
                }
                break;
        }

        var FacilityTradeTable = GameInfo.Instance.GameTable.FindFacilityTrade(x => x.MaterialGrade == selectGrade && x.MaterialCount == materialUIDs.Count);
        if (FacilityTradeTable == null)
        {
            SetLabel(0);
            return;
        }

        Percent = (int)(FacilityTradeTable.SuccessProb * 0.001f);
        SetLabel(Percent);
    }
}
