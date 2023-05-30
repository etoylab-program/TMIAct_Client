using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//�ü� ���� ����ġ, ������ ���ձ� ����
public class UIFacilityItemUnit : FUnit
{
	public GameObject kInfo;
    public GameObject kLock;
    public UISprite kLockSpr;
    public UILabel kLockLevelLabel;
    public UILabel kTitleLabel;
    public UILabel kDescLabel;
    public UIGaugeUnit kTimeGaugeUnit;
	public UIItemListSlot kItemListSlot;
    public UITexture kIconTex;
    public UISprite kIconBgSpr;
	public UIButton kConfirmBtn;
    public UIButton kStartBtn;
    public UILabel kConfirmBtnLabel;
    public UIGoodsUnit kGoodsUnit;
    public GameObject kChangeBtn;
    public GameObject kSpeedBtn;
    public GameObject kStopBtn;

    [Header("Operation")]
    public GameObject kOperationInfo;
    public UIItemListSlot kOperationItemSlot;
    public GameObject kOperationStartBtn;
    public GameObject kOperationConfirmBtn;
    public GameObject kOperationStopBtn;
    public GameObject kOperationLock;
    public UILabel kOperationLockLevelLabel;
    public UILabel kOperationLabel;
    public UILabel kOperationInfoLabel;
    public GameObject kOperationGaugeObj;
    public UILabel kOperationGaugeDescLabel;
    public UIGaugeUnit kOperationGaugeUnit;
    
    private FacilityData _facilitydata;
    private GameTable.Item.Param _itemParam;
    private List<GameTable.FacilityOperationRoom.Param> _facilityOperationRoomList;

    private string _strTimeColor;           //������ ���� ���ο� ���� �������� �����ð� ���� ���� ����

    private float FacilityDataDenominator = 0;
    private string str232 = "";

    public void UpdateSlot( int tableid ) 	//Fill parameter if you need
	{
        _facilitydata = GameInfo.Instance.GetFacilityData(tableid);
        if (_facilitydata == null)
        {
            return;
        }

        FacilityDataDenominator = GameSupport.GetFacilityTime(_facilitydata) * 60;
        str232 = FLocalizeString.Instance.GetText(232);

        bool bOperation = _facilitydata.TableData.EffectType.Equals("FAC_OPERATION_ROOM");
        kInfo.SetActive(!bOperation);
        kOperationInfo.SetActive(bOperation);
        kLock.SetActive(false);

        if (kOperationInfo.activeSelf)
        {
            kOperationConfirmBtn.SetActive(false);
            _facilityOperationRoomList = GameInfo.Instance.GameTable.FindAllFacilityOperationRoom(x => x.GroupID == _facilitydata.TableData.EffectValue);

            var firstParam = _facilityOperationRoomList.FirstOrDefault();
            if (firstParam != null)
            {
                kOperationLabel.textlocalize = FLocalizeString.Instance.GetText(firstParam.Name);
                kOperationInfoLabel.textlocalize = FLocalizeString.Instance.GetText(firstParam.Desc);
                _itemParam = GameInfo.Instance.GameTable.FindItem(firstParam.ProductIndex);
            }
            
            kOperationItemSlot.UpdateSlot(UIItemListSlot.ePosType.Mat, (int)eCOUNT.NONE, _itemParam);
        }
        
        if (_facilitydata.Level < _facilitydata.TableData.SlotOpenFacilityLv)       //���µǾ��� �������� ������ Ȱ��ȭ ������ ���� ǥ��
        {
            if (kOperationInfo.activeSelf)
            {
                kOperationLock.SetActive(true);
                kOperationStartBtn.SetActive(false);
                kOperationStopBtn.SetActive(false);
                kOperationLockLevelLabel.textlocalize = FLocalizeString.Instance.GetText(211, _facilitydata.TableData.SlotOpenFacilityLv);
                kOperationItemSlot.SetCountLabel(FLocalizeString.Instance.GetText(1792));
            }
            else
            {
                kLock.SetActive(true);
                kInfo.SetActive(false);
                kLockLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), _facilitydata.TableData.SlotOpenFacilityLv);
            }
        }
        else //�ü��� ���� �Ǿ�����
        {
            if (kOperationInfo.activeSelf)
            {
                kOperationLock.SetActive(false);
                Enum.TryParse(_facilitydata.Stats.ToString(), out eFACILITYSTATS stats);
                kOperationStartBtn.SetActive(stats == eFACILITYSTATS.WAIT);
                kOperationStopBtn.SetActive(stats == eFACILITYSTATS.USE);

                kOperationGaugeObj.SetActive(!kOperationStartBtn.activeSelf);
                kOperationInfoLabel.SetActive(kOperationStartBtn.activeSelf);
                
                if (kOperationStartBtn.activeSelf)
                {
                    kOperationItemSlot.SetCountLabel(FLocalizeString.Instance.GetText(1792));
                }
                else
                {
                    var charList = GameInfo.Instance.CharList.FindAll(x => x.OperationRoomTID == _facilitydata.TableID);
                    var findParam = _facilityOperationRoomList.Find(x => x.ParticipantCount == charList.Count);
                    if (findParam != null)
                    {
                        kOperationItemSlot.SetCountLabel(FLocalizeString.Instance.GetText(306, findParam.ProductValueMin, findParam.ProductValueMax));

                        var localName = FLocalizeString.Instance.GetText(findParam.Name);
                        var localValue = FLocalizeString.Instance.GetText(218, charList.Count, GameInfo.Instance.GameConfig.ParticipantMaxCount);
                        
                        kOperationLabel.textlocalize = FLocalizeString.Instance.GetText(220, localName, localValue);
                    }
                }

                _strTimeColor = FLocalizeString.Instance.GetText((int)eTEXTID.WHITE_TEXT_COLOR);
            }
            else
            {
                //������ �����Ͱ� �ִ��� �˻�
                CardData carddata = GameInfo.Instance.GetCardData(_facilitydata.EquipCardUID);
                if (carddata != null)
                {
                    kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Item/{0}.png", carddata.TableData.Icon));
                    kIconTex.gameObject.SetActive(true);
                    kIconBgSpr.spriteName = "itembgSlot_weapon_" + carddata.TableData.Grade.ToString();
                    kIconBgSpr.gameObject.SetActive(true);

                    _strTimeColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
                }
                else
                {
                    kIconTex.gameObject.SetActive(false);
                    kIconBgSpr.gameObject.SetActive(false);

                    _strTimeColor = FLocalizeString.Instance.GetText((int)eTEXTID.WHITE_TEXT_COLOR);
                }
                
                //��ϵ� ����, �������� �ִ��� Ȯ�� �� ������ ����
                if ( _facilitydata.Selete != (int)eCOUNT.NONE )     
                {
                    if (_facilitydata.TableData.EffectType == "FAC_WEAPON_EXP")
                    {
                        var weapondata = GameInfo.Instance.GetWeaponData(_facilitydata.Selete);
                        if (weapondata != null)
                        {
                            kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, -1, weapondata);
                            kItemListSlot.gameObject.SetActive(true);
                        }
                    }
                    else if (_facilitydata.TableData.EffectType == "FAC_ITEM_COMBINE")
                    {
                        var combinedata = GameInfo.Instance.GameTable.FindFacilityItemCombine((int)_facilitydata.Selete);
                        if (combinedata != null)
                        {
                            var itemdata = GameInfo.Instance.GameTable.FindItem(combinedata.ItemID);
                            if (itemdata != null)
                            {
                                kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, -1, itemdata);
                                kItemListSlot.SetCountLabel(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), combinedata.ItemCnt * _facilitydata.OperationCnt));
                                kItemListSlot.gameObject.SetActive(true);
                            }
                        }
                    }
                }
                else
                {
                    kItemListSlot.gameObject.SetActive(false);
                }
                
                kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(1126);

                //������ �������ο� ���� �ð���� �� Label�� ����
                float time = (GameSupport.GetFacilityTime(_facilitydata) * _facilitydata.OperationCnt * 60);
                kDescLabel.textlocalize = string.Format(_strTimeColor, GameSupport.GetFacilityTimeString(time));

                FacilityButtonActivetion(false, true);
                kStartBtn.gameObject.SetActive(false);

                if (_facilitydata.Stats == (int)eFACILITYSTATS.WAIT)        //�۵������� ����
                {
                    kChangeBtn.SetActive(true);
                    kTimeGaugeUnit.InitGaugeUnit(0f);
                }
                else
                {
                    kChangeBtn.SetActive(false);
                    //kConfirmBtn.gameObject.SetActive(true);
                    kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(1127);

                    var diffTime = _facilitydata.RemainTime - GameSupport.GetCurrentServerTime();
                    if (diffTime.Ticks > 0)     //������
                    {
                        FacilityButtonActivetion(false);
                        System.DateTime NowTime = GameSupport.GetCurrentServerTime();
                        string strremain = GameSupport.GetFacilityRemainTimeString(_facilitydata.RemainTime, NowTime);
                        kDescLabel.textlocalize = string.Format(_strTimeColor, strremain);

                        kConfirmBtnLabel.textlocalize = FLocalizeString.Instance.GetText(1124);

                        kTimeGaugeUnit.InitGaugeUnit(0.0f);

                    }
                    else  //�Ϸ�
                    {
                        FacilityButtonActivetion(true);
                        kDescLabel.textlocalize = string.Format(_strTimeColor, FLocalizeString.Instance.GetText(1125));

                        kTimeGaugeUnit.InitGaugeUnit(1.0f);
                        kConfirmBtnLabel.textlocalize = FLocalizeString.Instance.GetText(1125);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        FixedUpdate_Timer();
    }
    private void FixedUpdate_Timer()
    {
        if (_facilitydata == null)
        {
            return;
        }

        if (_facilitydata.Level == 0)
        {
            return;
        }

        if (_facilitydata.Stats == (int)eFACILITYSTATS.WAIT)
        {
            kChangeBtn.SetActive(true);
            return;
        }
    #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!GameInfo.Instance.netFlag)
                _facilitydata.RemainTime = GameSupport.GetCurrentServerTime();
        }
    #endif
        kChangeBtn.SetActive(false);
        var diffTime = _facilitydata.RemainTime - GameSupport.GetCurrentServerTime();
        if (diffTime.Ticks < 0)     //�Ϸ�
        {
            if (kOperationInfo.activeSelf)
            {
                if (kOperationConfirmBtn.activeSelf)
                {
                    return;
                }
                
                kOperationGaugeUnit.InitGaugeUnit(1.0f);
                kOperationGaugeDescLabel.textlocalize = FLocalizeString.Instance.GetText(1125);
                kOperationStopBtn.SetActive(false);
                kOperationConfirmBtn.SetActive(true);
                
                Lobby.Instance.CompleteFacility(_facilitydata.TableData.ParentsID, _facilitydata.TableID, true);
            }
            else
            {
                FacilityButtonActivetion(true);
                kDescLabel.textlocalize = string.Format(_strTimeColor, FLocalizeString.Instance.GetText(1125));

                kTimeGaugeUnit.InitGaugeUnit(1.0f);
                kConfirmBtnLabel.textlocalize = FLocalizeString.Instance.GetText(1125);
            }
            return;
        }

        //������            
        float f = 1.0f - ((float)diffTime.TotalSeconds / (FacilityDataDenominator * _facilitydata.OperationCnt));
        string strremain = GameSupport.GetFacilityRemainTimeString(_facilitydata.RemainTime, GameSupport.GetCurrentServerTime(), str232);
        if (kOperationInfo.activeSelf)
        {
            kOperationGaugeDescLabel.textlocalize = string.Format(_strTimeColor, strremain);
            kOperationGaugeUnit.InitGaugeUnit(f);
        }
        else
        {
            kDescLabel.textlocalize = string.Format(_strTimeColor, strremain);
            kTimeGaugeUnit.InitGaugeUnit(f);
        }
    }

    public void OnClick_Slot()
	{
	}
 

    public void OnClick_StartBtn()
    {
        int count = GameInfo.Instance.CharList.FindAll(x => x.OperationRoomTID > 0).Count;
        if (GameInfo.Instance.CharList.Count <= count || GameInfo.Instance.GameConfig.ParticipantMaxCount <= count)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3056));
            return;
        }
        
        if (_facilitydata == null)
        {
            return;
        }

        if (_facilitydata.Level == 0)
        {
            return;
        }

        if (_facilitydata.Stats != (int) eFACILITYSTATS.WAIT)
        {
            return;
        }

        if (kOperationInfo.activeSelf)
        {
            UIFacilityOperationSelectPopup popup = LobbyUIManager.Instance.GetUI<UIFacilityOperationSelectPopup>("FacilityOperationSelectPopup");
            if (popup != null)
            {
                popup.SetItem(_facilitydata, _itemParam);
                popup.SetUIActive(true);
            }
        }
        else
        {
            FacilityDataDenominator = GameSupport.GetFacilityTime(_facilitydata) * 60;
            
            var combinedata = GameInfo.Instance.GameTable.FindFacilityItemCombine((int)_facilitydata.Selete);
            if (combinedata == null)
                return;
            var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == combinedata.ItemReqGroup);
            if (reqdata == null)
                return;

            bool bmat = true;
            List<int> idlist = new List<int>();
            List<int> countlist = new List<int>();

            GameSupport.SetMatList(reqdata, ref idlist, ref countlist);

            for (int i = 0; i < idlist.Count; i++)
            {
                int orgcut = GameInfo.Instance.GetItemIDCount(idlist[i]);
                int orgmax = countlist[i];
                if (orgcut < orgmax)
                    bmat = false;
            }

            if (!GameSupport.IsCheckGoods(eGOODSTYPE.GOLD, reqdata.Gold))
            {
                return;
            }
            if (!bmat)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3003));
                return; //��� ���� 
            }

            //GameInfo.Instance.Send_ReqFacilityOperation(_facilitydata.TableID, -1, OnNetFacilityUse);
        }
    }

    //�ü� �۵����϶� ��� ��ư�� �ൿ�� ��
    public void OnClick_ConfirmBtn()
	{
        if (_facilitydata == null)
        {
            return;
        }
        if (_facilitydata.Level == 0)
        {
            return;
        }
            

        if (_facilitydata.Stats == (int)eFACILITYSTATS.WAIT && _facilitydata.Selete == (int)eCOUNT.NONE)
        {
            OnClick_ItemChanegeBtn();
            return;
        }

        if (_facilitydata.Stats == (int)eFACILITYSTATS.WAIT)
        {
            GameInfo.Instance.Send_ReqFacilityOperation(_facilitydata.TableID, -1, _facilitydata.OperationCnt, null, OnNetFacilityUse);
        }
        else if(_facilitydata.Stats == (int)eFACILITYSTATS.USE || _facilitydata.Stats == (int)eFACILITYSTATS.COMPLETE)
        {
            var diffTime = _facilitydata.RemainTime - GameSupport.GetCurrentServerTime();
            if (diffTime.Ticks > 0)     //������
            {
                //  �������� ��ҽ� Ȯ�� �޼���
                MessagePopup.CYN(eTEXTID.OK,
                                 3055,      //  ����Ͻðڽ��ϱ�?
                                 eTEXTID.OK,
                                 eTEXTID.CANCEL,
                                 () => { GameInfo.Instance.Send_ReqFacilityCancel(_facilitydata.TableID, OnNetFacilityCancel); },
                                 null);
                
            }
            else  //�Ϸ�
            {
                GameInfo.Instance.Send_ReqFacilityComplete(_facilitydata.TableID,0, PktInfoFacilityOperConfirmReq.TYPE._NONE_, OnNetFacilityComplete);
            }
        }
    }

    public void OnClick_ItemChanegeBtn()
    {
        if (_facilitydata == null)
            return;
        if (_facilitydata.Stats != (int)eFACILITYSTATS.WAIT)            //�������̰ų�, �Ϸ� ������϶� ���⺯�� �Ұ�
            return;
        

        UIValue.Instance.SetValue(UIValue.EParamType.FacilityID, _facilitydata.TableID);
        if (_facilitydata.TableData.EffectType == "FAC_WEAPON_EXP")
        {
            if (GameInfo.Instance.WeaponList.Count == 0)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3039));
                return;
            }
            
            if (GameInfo.Instance.WeaponList.Count - GameInfo.Instance.CharList.Count <= 0)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3039));
                return;
            }

            bool weaponFlag = false;
            for (int i = 0; i < GameInfo.Instance.WeaponList.Count; i++)
            {
                if (_facilitydata.Selete == GameInfo.Instance.WeaponList[i].WeaponUID || 
                    (GameInfo.Instance.GetEquipWeaponFacilityData(GameInfo.Instance.WeaponList[i].WeaponUID) == null &&
                    GameInfo.Instance.GetEquipWeaponCharData(GameInfo.Instance.WeaponList[i].WeaponUID) == null))
                {
                    weaponFlag = true;
                    break;
                }
            }

            if(!weaponFlag)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3039));
                return;
            }

            LobbyUIManager.Instance.ShowUI("FacilityWeaponSeletePopup", true);
        }
        else if (_facilitydata.TableData.EffectType == "FAC_ITEM_COMBINE")
            LobbyUIManager.Instance.ShowUI("FacilityItemCombinePopup", true);
    }

    public void OnClick_SuppoterChanegeBtn()
    {
        if (_facilitydata == null)
            return;
        if (_facilitydata.Level == 0)
            return;

        if (_facilitydata.Stats != (int)eFACILITYSTATS.WAIT)
            return;

        if (GameInfo.Instance.CardList.Count == 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3020));
            return;
        }

        //������ ��Ͽ� ǥ���Ұ� �ִ��� üũ
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

        if(!cardFlag)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3038));
            return;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.FacilityID, _facilitydata.TableID);
        LobbyUIManager.Instance.ShowUI("FacilityCardSeletePopup", true);
    }

    //��ÿϷ� ��ư
    public void OnClick_SpeedBtn()
    {
        Log.Show("��ÿϷ�!!!!");
        int needItemCnt = GameSupport.GetFacilityNeedSpeedItem(_facilitydata);

        if (GameInfo.Instance.GetItemIDCount(GameSupport.GetFacilitySpeedItemTableID()) < needItemCnt)
        {
            MessagePopup.CYNItemCash(
                FLocalizeString.Instance.GetText(1325),
                FLocalizeString.Instance.GetText(3064),
                eTEXTID.OK,
                eTEXTID.CANCEL,
                GameSupport.GetFacilitySpeedItemTableID(),
                needItemCnt,
                OnClickSpeedUpMessageOk,
                null);
        }
        else
        {
            //10021 �ü� �ð� ���� ������
            MessagePopup.CYNItem(
            FLocalizeString.Instance.GetText(1325),
            FLocalizeString.Instance.GetText(3064),
            eTEXTID.OK,
            eTEXTID.CANCEL,
            GameSupport.GetFacilitySpeedItemTableID(),
            needItemCnt,
            OnClickSpeedUpMessageOk,
            null);
        }
    }

    //��ÿϷ� �˾�
    public void OnClickSpeedUpMessageOk()
    {
        int needItemCnt = GameSupport.GetFacilityNeedSpeedItem(_facilitydata);

        GameTable.Item.Param speedItem = GameInfo.Instance.GameTable.FindItem(x => x.Type == (int)eITEMTYPE.MATERIAL && x.SubType == (int)eITEMSUBTYPE.MATERIAL_FACILITY_TIME);
        if (speedItem == null)
            return;

        var userItemCnt = GameInfo.Instance.GetItemIDCount(speedItem.ID);

        //��ÿϷ� ����
        if(userItemCnt >= needItemCnt)
        {
            var userItem = GameInfo.Instance.GetItemData(speedItem.ID);
            if(userItem != null)
            {
                GameInfo.Instance.Send_ReqFacilityComplete(_facilitydata.TableID, userItem.TableID, PktInfoFacilityOperConfirmReq.TYPE.ITEM, OnNetFacilityComplete);
            }
        }
        else //��ÿϷ� ����
        {
            //���簡 �����մϴ�.
            //MessageToastPopup.Show(FLocalizeString.Instance.GetText(3003));
            if (!GameSupport.IsCheckGoods(eGOODSTYPE.CASH, needItemCnt))
                return;

            GameInfo.Instance.Send_ReqFacilityComplete(_facilitydata.TableID, speedItem.ID, PktInfoFacilityOperConfirmReq.TYPE.CASH, OnNetFacilityComplete);
        }
    }


    public void OnNetFacilityUse(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;


        LobbyFacility lobbyFacility = Lobby.Instance.GetLobbyFacility(_facilitydata.TableData.ParentsID);
        Debug.LogError((lobbyFacility != null));
        if (lobbyFacility != null)
        {
            lobbyFacility.InitLobbyFacility(_facilitydata);
        }
        LobbyUIManager.Instance.Renewal("FacilityItemPanel");
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");

        var carddata = GameInfo.Instance.GetCardData(_facilitydata.EquipCardUID);
        if (carddata != null)
        {
            VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.FacilityStart, carddata.TableID);
        }

        //Renewal(true);
    }

    public void OnNetFacilityCancel(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyFacility lobbyFacility = Lobby.Instance.GetLobbyFacility(_facilitydata.TableData.ParentsID);
        if (lobbyFacility != null)
        {
            lobbyFacility.InitLobbyFacility(_facilitydata);
        }

        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3027));
        LobbyUIManager.Instance.Renewal("FacilityItemPanel");
        
    }

    public void OnNetFacilityComplete(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");

        PktInfoFacilityOperConfirmAck pktInfo = pktmsg as PktInfoFacilityOperConfirmAck;
        if (pktInfo != null)
        {
            if (!pktInfo.operEndFlag_)
            {
                OnNetFacilityCancel(0, pktmsg);
                return;
            }
        }

        LobbyFacility lobbyFacility = Lobby.Instance.GetLobbyFacility(_facilitydata.TableData.ParentsID);
        if (lobbyFacility != null)
        {
            lobbyFacility.InitLobbyFacility(_facilitydata);
        }

        LobbyUIManager.Instance.Renewal("FacilityItemPanel");
        LobbyUIManager.Instance.ShowUI("FacilityResultPopup", true);
        
        LocalPushNotificationManager.Instance.RemoveFacilityNotification(_facilitydata.TableID);
    }

    public eFACILITYSTATS GetFacilityState()
    {
        
        if (_facilitydata == null)
            return eFACILITYSTATS.WAIT;
        return (eFACILITYSTATS)_facilitydata.Stats;
    }

    public void SetBlankSlot()
    {
        if (_facilitydata != null)
        {
            _facilitydata.Selete = (int)eCOUNT.NONE;
            _facilitydata.EquipCardUID = (int)eCOUNT.NONE;
        }
    }

    private void FacilityButtonActivetion(bool enable, bool allEnable = false)
    {
        if(!allEnable)
        {
            kConfirmBtn.gameObject.SetActive(enable);
            kSpeedBtn.gameObject.SetActive(!enable);
            kStopBtn.gameObject.SetActive(!enable);
        }
        else
        {
            kConfirmBtn.gameObject.SetActive(enable);
            kSpeedBtn.gameObject.SetActive(enable);
            kStopBtn.gameObject.SetActive(enable);
        }
    }
}
