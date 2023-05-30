using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFacilityUpGradePopup : FComponent
{
    [Serializable]
    public class FacilityUpgradeInfo
    {
        public GameObject bgSprObj;
        public UILabel textLabel;
        public UILabel nowLabel;
        public UILabel nextLabel;
    }

    public UILabel kNameLabel;
    public UILabel kLevelLabel;
    public UIGoodsUnit kGoodsUnit;
    public List<FacilityUpgradeInfo> kInfoList;
    public List<UIItemListSlot> kMatList;

    public int FacilityId { get; set; } = 0;

    private FacilityData _facilitydata;
    private bool _bmat;


	public override void OnEnable() {
		_facilitydata = GameInfo.Instance.GetFacilityData( GameInfo.Instance.GetFacilityData( FacilityId ).TableData.ParentsID );
		base.OnEnable();
	}
 
    private void SetFacilityInfo(int index, int desc, int now, int next, bool active = true)
    {
        if (kInfoList.Count <= index)
        {
            return;
        }

        FacilityUpgradeInfo info = kInfoList[index];
        if (info == null)
        {
            return;
        }
        
        info.bgSprObj.SetActive(active);
        
        info.textLabel.SetActive(active);
        info.textLabel.textlocalize = FLocalizeString.Instance.GetText(desc);
        
        info.nowLabel.SetActive(active);
        info.nowLabel.textlocalize = $"{now:#,##0}";
        
        info.nextLabel.SetActive(active);
        info.nextLabel.textlocalize = $"{next:#,##0}";
    }
    
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        if (_facilitydata == null)
            return;

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_facilitydata.TableData.Name);
        kLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_AND_MAX_LV), _facilitydata.Level, _facilitydata.TableData.MaxLevel);

        int now = _facilitydata.Level;
        int next = _facilitydata.Level + 1;
        SetFacilityInfo(0, 1099, now, next);

        now = _facilitydata.GetEffectValue(_facilitydata.Level);
        next = _facilitydata.GetEffectValue(_facilitydata.Level + 1);
        SetFacilityInfo(1, _facilitydata.TableData.EffectDesc, now, next, 0 < _facilitydata.TableData.IncEffectValue);

        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == _facilitydata.TableData.LevelUpItemReq && x.Level == _facilitydata.Level);
        if (reqdata == null)
            return;
        for (int i = 0; i < kMatList.Count; i++)
            kMatList[i].gameObject.SetActive(false);
        kGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, reqdata.Gold, true);
        List<int> idlist = new List<int>();
        List<int> countlist = new List<int>();
        GameSupport.SetMatList(reqdata, ref idlist, ref countlist);
        _bmat = true;
        for (int i = 0; i < idlist.Count; i++)
        {
            kMatList[i].gameObject.SetActive(true);
            kMatList[i].UpdateSlot(UIItemListSlot.ePosType.Mat, i, GameInfo.Instance.GameTable.FindItem(idlist[i]));
            kMatList[i].kCountLabel.gameObject.transform.parent.gameObject.SetActive(true);
            int orgcut = GameInfo.Instance.GetItemIDCount(idlist[i]);
            int orgmax = countlist[i];
            string strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
            if (orgcut < orgmax)
            {
                _bmat = false;
                strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);
            }
            string strmatcount = string.Format(strHaveCntColor, string.Format(FLocalizeString.Instance.GetText(236), orgcut, orgmax));
            kMatList[i].SetCountLabel(strmatcount);
        }
    }

    public void OnClick_CloseBtn()
	{
        OnClickClose();
    }

    public override void OnClickClose()
    {
        LobbyUIManager.Instance.ShowUI("TopPanel", false);
        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.FACILITY)
            LobbyUIManager.Instance.ShowUI("FacilityPanel", false);
        else if (LobbyUIManager.Instance.PanelType == ePANELTYPE.FACILITYITEM)
            LobbyUIManager.Instance.ShowUI("FacilityItemPanel", false);

        base.OnClickClose();
    }

    public void OnClick_SeleteBtn()
	{
        if (_facilitydata == null)
            return;
        if (_facilitydata.Level >= _facilitydata.TableData.MaxLevel)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3007));
            return;
        }
        
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == _facilitydata.TableData.LevelUpItemReq && x.Level == _facilitydata.Level);
        if (reqdata == null)
            return;

        if (!_bmat)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3003));
            return;
        }

        if (!GameSupport.IsCheckGoods(eGOODSTYPE.GOLD, reqdata.Gold)) //금액부족
        {
            return;
        }

        GameInfo.Instance.Send_ReqFacilityUpgrade(_facilitydata.TableID, OnNetFacilityLevelUp);
    }

    public void OnNetFacilityLevelUp(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.HideUI("TopPanel", false);
        LobbyUIManager.Instance.HideUI("FacilityPanel", false);
        LobbyUIManager.Instance.HideUI("FacilityItemPanel", false);

        if (_facilitydata.Level.Equals(1))
        {
            LobbyFacility lobbyFacility = Lobby.Instance.GetLobbyFacility(_facilitydata.TableData.ParentsID);
            if (lobbyFacility != null)
            {
                PktInfoFacilityUpgrade pktInfoFacilityUpgrade = pktmsg as PktInfoFacilityUpgrade;
                lobbyFacility.FacilityActivation(_facilitydata, pktInfoFacilityUpgrade);
                CancelInvoke("FacilityTutorialFlag");
                Invoke("FacilityTutorialFlag", 2f);
            }

            base.OnClickClose();
        }
        else
        {
            LobbyFacility lobbyFacility = Lobby.Instance.GetLobbyFacility(_facilitydata.TableData.ParentsID);
            if (lobbyFacility != null)
            {
                lobbyFacility.SetFacilityUpGrade(_facilitydata);
            }
            base.OnClickClose();
        }
    }

    void FacilityTutorialFlag()
    {
        if (_facilitydata.TableData.EffectType.Equals("FAC_CHAR_EXP"))
            GameSupport.ShowTutorialFlag(eTutorialFlag.FAC_CHAR_EXP);
        else if (_facilitydata.TableData.EffectType.Equals("FAC_CHAR_SP"))
            GameSupport.ShowTutorialFlag(eTutorialFlag.FAC_CHAR_SP);
        else if (_facilitydata.TableData.EffectType.Equals("FAC_ITEM_COMBINE"))
            GameSupport.ShowTutorialFlag(eTutorialFlag.FAC_ITEM_COMBINE);
        else if (_facilitydata.TableData.EffectType.Equals("FAC_WEAPON_EXP"))
            GameSupport.ShowTutorialFlag(eTutorialFlag.FAC_WEAPON_EXP);
    }

    IEnumerator CheckResultPopup(PktMsgType pktmsg)
    {
        LobbyFacility lobbyFacility = Lobby.Instance.GetLobbyFacility(_facilitydata.TableData.ParentsID);
        if (lobbyFacility != null)
        {
            PktInfoFacilityUpgrade pktInfoFacilityUpgrade = pktmsg as PktInfoFacilityUpgrade;
            lobbyFacility.FacilityActivation(_facilitydata, pktInfoFacilityUpgrade);
        }

        base.OnClickClose();

        yield return null;
    }

   
}
