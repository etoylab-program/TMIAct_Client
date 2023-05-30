using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIArenaBattleConfirmPopup : FComponent
{
    [Header("Arena Team")]
    public UIArenaBattleListSlot kUserSlot;
    public UIArenaBattleListSlot kEnemySlot;
	
    [Space(5)]
    [Header("ATK UP")]
	public UIButton kATKupBtn;
    public UISprite kATKupSelSpr;
	public UILabel kATKupCntLabel;
    [Header("DEF UP")]
    public UIButton kDEFupBtn;
    public UISprite kDEFupSelSpr;
    public UILabel kDEFupCntLabel;
    [Header("GOLD UP")]
    public UIButton kGoldupBtn;
    public UISprite kGoldupSelSpr;
	public UIGoodsUnit kGoodsUnit;

    [Space(5)]
    public UIGoodsUnit kSerachGoods;
    public UIGoodsUnit kArenaBattleGoods;

    [Space(5)]
    [Header("Disable Objects In Friend PVP")]
    public GameObject[] DisableObjsInFriendPVP;


    private bool _bUseAtkUpItem = false;
    private long _atkItemUID = 0;

    private bool _bUseDefUpItem = false;
    private long _defItemUID = 0;

    private bool _bUseGold = false;

    private GameTable.Item.Param _atkItemTableData;
    private GameTable.Item.Param _defItemTableData;

    private GameTable.ArenaGrade.Param _arenaGradeTableData;

    private bool mbFriendPVP = false;


	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        kATKupSelSpr.gameObject.SetActive(false);
        kDEFupSelSpr.gameObject.SetActive(false);
        kGoldupSelSpr.gameObject.SetActive(false);

        _bUseAtkUpItem = false;
        _bUseDefUpItem = false;
        _bUseGold = false;

        _atkItemTableData = GameInfo.Instance.GameTable.FindItem(x => x.Type == (int)eITEMTYPE.MATERIAL && x.SubType == (int)eITEMSUBTYPE.MATERIAL_ARENA_ATKBUFF);
        _defItemTableData = GameInfo.Instance.GameTable.FindItem(x => x.Type == (int)eITEMTYPE.MATERIAL && x.SubType == (int)eITEMSUBTYPE.MATERIAL_ARENA_DEFBUFF);


        

        _arenaGradeTableData = GameInfo.Instance.GameTable.FindArenaGrade(x => x.GradeID == GameInfo.Instance.UserBattleData.Now_GradeId);
        //kSerachGoods.InitGoodsUnit(eGOODSTYPE.GOLD, GameInfo.Instance.GameConfig.mat)
    }
 
	public override void Renewal(bool bChildren)
	{
        if (GameInfo.Instance.MatchTeam == null)
            return;

		base.Renewal(bChildren);

        UseItemCheck();

        kATKupSelSpr.gameObject.SetActive(GameInfo.Instance.ArenaATK_Buff_Flag);
        kDEFupSelSpr.gameObject.SetActive(GameInfo.Instance.ArenaDEF_Buff_Flag);
        kGoldupSelSpr.gameObject.SetActive(GameInfo.Instance.ArenaGold_Buff_Flag);

        // 해당 UIValue 삭제는 백버튼을 눌렀을 때나 WorldPVP 안에서 해줌
        mbFriendPVP = UIValue.Instance.ContainsKey(UIValue.EParamType.IsFriendPVP);
        for (int i = 0; i < DisableObjsInFriendPVP.Length; i++)
        {
            DisableObjsInFriendPVP[i].SetActive(!mbFriendPVP);
        }

        kUserSlot.UpdateSlot(mbFriendPVP, null, false);
        kEnemySlot.UpdateSlot(mbFriendPVP, GameInfo.Instance.MatchTeam);

        //상성관계 나타내기
        kUserSlot.SetSynastryTeam(false);
        kEnemySlot.SetSynastryTeam(true);

        bool arenaAtkBuffFlag = mbFriendPVP ? false : GameInfo.Instance.ArenaATK_Buff_Flag;
        bool arenaDefBuffFlag = mbFriendPVP ? false : GameInfo.Instance.ArenaDEF_Buff_Flag;

        kUserSlot.SetUserTeamPower(arenaAtkBuffFlag, arenaDefBuffFlag);

        //kNameLabel.textlocalize = GameInfo.Instance.UserData.NickName;
        //kEnemyNameLabel.textlocalize = GameInfo.Instance.MatchTeam.UserNickName;
    }

    private void UseItemCheck()
    {
        int atkUpItemCnt = 0;
        ItemData atkup = GameInfo.Instance.ItemList.Find(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_ARENA_ATKBUFF);
        if (atkup != null)
        {
            atkUpItemCnt = GameInfo.Instance.GetItemIDCount(atkup.TableID);
            _atkItemUID = atkup.ItemUID;
        }

        {
            string strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);
            if (atkUpItemCnt >= 1)
            {
                strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
                _bUseAtkUpItem = true;
            }
            string strmatcount = string.Format(strHaveCntColor, string.Format(FLocalizeString.Instance.GetText(236), atkUpItemCnt, 1));
            kATKupCntLabel.textlocalize = strmatcount;
        }

        int defUpItemCnt = 0;
        ItemData defup = GameInfo.Instance.ItemList.Find(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_ARENA_DEFBUFF);
        if (defup != null)
        {
            defUpItemCnt = GameInfo.Instance.GetItemIDCount(defup.TableID);
            _defItemUID = defup.ItemUID;
        }

        {
            string strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);
            if (defUpItemCnt >= 1)
            {
                strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
                _bUseDefUpItem = true;
            }
            string strmatcount = string.Format(strHaveCntColor, string.Format(FLocalizeString.Instance.GetText(236), defUpItemCnt, 1));
            kDEFupCntLabel.textlocalize = strmatcount;
        }

        int coinBuffPrice = (int)(GameInfo.Instance.GameConfig.ArenaCoinBuffPrice - (GameInfo.Instance.GameConfig.ArenaCoinBuffPrice * GameSupport.GetEquipBadgeTotalValue(GameInfo.Instance.BadgeList, Unit2ndStatsTable.eBadgeOptType.CoinCostdown)));

        _bUseGold = GameInfo.Instance.UserData.IsGoods(eGOODSTYPE.GOLD, coinBuffPrice);
        kGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, coinBuffPrice, true);

        if (_bUseGold && (coinBuffPrice != GameInfo.Instance.GameConfig.ArenaCoinBuffPrice))
        {
            kGoodsUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), coinBuffPrice));
        }
        Debug.LogError(_arenaGradeTableData.MatchPrice);
        kSerachGoods.InitGoodsUnit(eGOODSTYPE.GOLD, _arenaGradeTableData.MatchPrice, true);

        // 필요 BP
        kArenaBattleGoods.InitGoodsUnit(eGOODSTYPE.BP, GameInfo.Instance.GameConfig.ArenaUseBP);

        if (!_bUseAtkUpItem)
            GameInfo.Instance.ArenaATK_Buff_Flag = false;
        if (!_bUseDefUpItem)
            GameInfo.Instance.ArenaDEF_Buff_Flag = false;
        if (!_bUseGold)
            GameInfo.Instance.ArenaGold_Buff_Flag = false;
    }
 
	public void OnClick_ATKupBtn()
	{
        if (!_bUseAtkUpItem)
            return;

        GameInfo.Instance.ArenaATK_Buff_Flag = !GameInfo.Instance.ArenaATK_Buff_Flag;

        Renewal(true);
	}
	
	public void OnClick_ATKupDetailBtn()
	{
        if (_atkItemTableData == null)
            return;

        UIValue.Instance.SetValue(UIValue.EParamType.ItemUID, (long)-1);
        UIValue.Instance.SetValue(UIValue.EParamType.ItemTableID, _atkItemTableData.ID);
        LobbyUIManager.Instance.ShowUI("ItemInfoPopup", true);
    }
	
	public void OnClick_DEFupBtn()
	{
        if (!_bUseDefUpItem)
            return;

        GameInfo.Instance.ArenaDEF_Buff_Flag = !GameInfo.Instance.ArenaDEF_Buff_Flag;

        Renewal(true);
    }
	
	public void OnClick_DEFupDetailBtn()
	{
        if (_defItemTableData == null)
            return;

        UIValue.Instance.SetValue(UIValue.EParamType.ItemUID, (long)-1);
        UIValue.Instance.SetValue(UIValue.EParamType.ItemTableID, _defItemTableData.ID);
        LobbyUIManager.Instance.ShowUI("ItemInfoPopup", true);
    }
	
	public void OnClick_GoldupBtn()
	{
        if (!_bUseGold)
            return;

        GameInfo.Instance.ArenaGold_Buff_Flag = !GameInfo.Instance.ArenaGold_Buff_Flag;

        Renewal(true);
    }

	
	public void OnClick_ArenaBattleListSlot()
	{
	}
	
	public void OnClick_SearchBtn()
	{
        if (!GameSupport.IsCheckGoods(eGOODSTYPE.GOLD, _arenaGradeTableData.MatchPrice))
        {
            return;
        }

        if (GameSupport.IsEmptyInEquipMainWeapon(ePresetKind.ARENA))
        {
            return;
        }

        if (GameSupport.ArenaTeamCheckFlag())
        {
            GameInfo.Instance.Send_ReqArenaEnemySearch(OnNet_AckArenaEnemySearch);
        }
    }
	
	public void OnClick_BattleBtn()
	{
        if (!GameSupport.ArenaTeamCheckFlag())
        {
            return;
        }

        if (ArenaTeamSkillCheck())
        {
            OnArenaStart();
        }
        else
        {
            ArenaTeamSkillOut();
        }
    }

    public void OnClick_BackBtn()
    {
        //MessagePopup.CYN(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3172), eTEXTID.YES, eTEXTID.NO, () => { OnClickClose(); });
        OnClickClose();
    }

    public void OnNet_AckArenaEnemySearch(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");

        PlayAnimtion(2);
        InitComponent();
        Renewal(true);
    }

    public void OnNet_AckArenaGameStart(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        GameInfo.Instance.IsFriendArenaGame = mbFriendPVP;
        
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.LobbyToArena);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, -1);
        LobbyUIManager.Instance.ShowUI("LoadingPopup", false);
        AppMgr.Instance.LoadScene(AppMgr.eSceneType.Stage, "Stage_PVP");
    }

    public void OnClick_CharTypeChartBtn()
    {
        LobbyUIManager.Instance.ShowUI("CharTypeChartPopup", true);
    }

    public override void OnClickClose()
    {
        if (!mbFriendPVP)
        {
            MessagePopup.CYN(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3172), eTEXTID.YES, eTEXTID.NO, () => { base.OnClickClose(); });
        }
        else
        {
            //if(LobbyUIManager.Instance.GetActiveUI("FriendPopup") == null)
            //{
            //    LobbyUIManager.Instance.ShowUI("FriendPopup", false);
            //}
            UIValue.Instance.RemoveValue(UIValue.EParamType.IsFriendPVP);
            base.OnClickClose();
        }
    }

    private bool ArenaTeamSkillCheck()
    {
        bool flag = true;

        for (int i = 0; i < GameInfo.Instance.TeamcharList.Count; i++)
        {
            if (GameInfo.Instance.TeamcharList[i] == (int)eCOUNT.NONE)
                continue;

            CharData chardata = GameInfo.Instance.GetCharData(GameInfo.Instance.TeamcharList[i]);
            if (chardata == null)
                continue;

            if (GameSupport.GetCharLastSkillSlotCheck(chardata))
            {
                flag = false;
                break;
            }
        }

        return flag;
    }

    private void ArenaTeamSkillOut()
    {
        for (int i = 0; i < GameInfo.Instance.TeamcharList.Count; i++)
        {
            if (GameInfo.Instance.TeamcharList[i] == (int)eCOUNT.NONE)
                continue;

            CharData chardata = GameInfo.Instance.GetCharData(GameInfo.Instance.TeamcharList[i]);
            if (chardata == null)
                continue;

            if (GameSupport.GetCharLastSkillSlotCheck(chardata))
            {
                Log.Show(chardata.TableID + " / SkillOUT", Log.ColorType.Red);

                chardata.EquipSkill[(int)eCOUNT.SKILLSLOT - 1] = (int)eCOUNT.NONE;
                GameInfo.Instance.Send_ReqApplySkillInChar(chardata.CUID, chardata.EquipSkill, OnArenaTeamSkillOut);
                break;
            }
        }
    }

    public void OnArenaTeamSkillOut(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        if (ArenaTeamSkillCheck())
        {
            OnArenaStart();
        }
        else
        {
            ArenaTeamSkillOut();
        }
    }

    private void OnArenaStart()
    {
        if (GameSupport.IsEmptyInEquipMainWeapon(ePresetKind.ARENA))
        {
            return;
        }

        List<long> buffItem = new List<long>();

        if (!mbFriendPVP)
        {
            if (!GameSupport.IsCheckTicketBP(GameInfo.Instance.GameConfig.ArenaUseBP))
            {
                return;
            }

            if (!GameSupport.ArenaTeamCheckFlag())
            {
                return;
            }

            if (GameInfo.Instance.ArenaATK_Buff_Flag)
                buffItem.Add(_atkItemUID);
            if (GameInfo.Instance.ArenaDEF_Buff_Flag)
                buffItem.Add(_defItemUID);
            Log.Show("ArenaBattle Start!!!", Log.ColorType.Red);

            for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
            {
                Log.Show(i + " / " + GameInfo.Instance.TeamcharList[i]);
            }

            Log.Show(GameInfo.Instance.MatchTeam.charlist);

            GameInfo.Instance.Send_ReqArenaGameStart(buffItem, GameInfo.Instance.ArenaGold_Buff_Flag, OnNet_AckArenaGameStart);
        }
        else
        {
            OnNet_AckArenaGameStart(0, null);
        }
    }
}
