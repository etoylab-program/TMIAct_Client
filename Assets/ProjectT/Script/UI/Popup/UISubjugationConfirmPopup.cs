using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISubjugationConfirmPopup : FComponent
{
    [Header("UISubjugationConfirmPopup")]
    [SerializeField] private UILabel apLabel = null;
    [SerializeField] private UILabel fastQuestTicketLabel = null;
    [SerializeField] private UILabel noticeLabel = null;

    private CharData _charData = null;
    private int _stageId = 0;
    private int _multipleIndex = 0;
    private long _selectCUid = -1;
    private bool _multiGCFlag = false;
    private bool _isSecretQuest = false;
    private List<long> _subCUIDList;

    public override void OnEnable()
    {
        base.OnEnable();

        ItemData item = GameInfo.Instance.ItemList.Find(x => x.TableID.Equals(GameInfo.Instance.GameConfig.FastQuestTicketID));
        int multiple = GameInfo.Instance.GameConfig.MultipleList[_multipleIndex];
        if (_multiGCFlag)
        {
            GameTable.Stage.Param stageParam = GameInfo.Instance.GameTable.FindStage(_stageId);
            GuerrillaCampData multigcdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Stage_Multiple, stageParam.StageType);
            if (multigcdata != null)
            {
                multiple = multigcdata.EffectValue;
            }
        }

        fastQuestTicketLabel.textlocalize = FLocalizeString.Instance.GetText(218, item?.Count ?? 0, multiple);

        string notice = FLocalizeString.Instance.GetText(1730);

        if (_isSecretQuest == true)
            notice = $"{notice}\n{FLocalizeString.Instance.GetText(1884)}";

        noticeLabel.textlocalize = notice;
    }

    public void SetValue(CharData charData, int stageId, int multipleIndex, long selectCUid, string apText, bool multiGCFlag)
    {
        _charData = charData;
        _stageId = stageId;
        _multipleIndex = multipleIndex;
        _selectCUid = selectCUid;
        _multiGCFlag = multiGCFlag;
        _subCUIDList = null;

        apLabel.textlocalize = apText;

        _isSecretQuest = false;
    }

    public void SetValue(int multipleIndex, string apText, bool multiGCFlag)
    {
        _multipleIndex = multipleIndex;
        _multiGCFlag = multiGCFlag;
        apLabel.textlocalize = apText;
        _subCUIDList = null;

        _isSecretQuest = false;
    }

    //비밀 임무용
    public void SetValue(CharData charData, int stageId, int multipleIndex, long selectCUid, string apText, List<long> subCUIDList ) {
        _charData = charData;
        _stageId = stageId;
        _multipleIndex = multipleIndex;
        _selectCUid = selectCUid;
        _multiGCFlag = false;

        _subCUIDList = new List<long>();
        for (int i = 0; i < subCUIDList.Count; i++) {
            _subCUIDList.Add(subCUIDList[i]);
        }

        apLabel.textlocalize = apText;

        _isSecretQuest = true;
    }


    public void OnClick_YesBtn()
    {
        if (GameSupport.GetCharLastSkillSlotCheck(_charData))
        {
            _charData.EquipSkill[(int)eCOUNT.SKILLSLOT - 1] = (int)eCOUNT.NONE;
            GameInfo.Instance.Send_ReqApplySkillInChar(_charData.CUID, _charData.EquipSkill, OnSkillOutGameFastQuestTicket);
            return;
        }

        GameInfo.Instance.Send_ReqStageStart(_stageId, _selectCUid, _multipleIndex, true, _multiGCFlag, _subCUIDList, OnNetFastQuestTicketStart);
    }
    
    private void OnSkillOutGameFastQuestTicket(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        Log.Show("Skill Out!!!", Log.ColorType.Red);
        GameInfo.Instance.Send_ReqStageStart(_stageId, _selectCUid, _multipleIndex, true, _multiGCFlag, _subCUIDList, OnNetFastQuestTicketStart);
    }
    
    private void OnNetFastQuestTicketStart(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        GameTable.Stage.Param stageData = GameInfo.Instance.GameTable.FindStage(_stageId);
        if (stageData == null)
        {
            return;
        }

        if (!GameInfo.Instance.netFlag)
        {
            GameInfo.Instance.MaxNormalBoxCount = Random.Range(stageData.N_DropMinCnt, stageData.N_DropMaxCnt + 1);
        }

        GameInfo.Instance.Send_ReqStageEnd(_stageId, GameInfo.Instance.SeleteCharUID, 100, stageData.RewardGold,
            GameInfo.Instance.MaxNormalBoxCount, true, true, true, OnNetGameResult);
    }
    
    private void OnNetGameResult(int result, PktMsgType pktmsg)
    {
        UIBattleResultPopup popup = LobbyUIManager.Instance.GetUI("BattleResultPopup") as UIBattleResultPopup;
        if (popup == null)
        {
            return;
        }
        
        if (popup.isActiveAndEnabled)
        {
            popup.Rewind();
        }
        else
        {
            popup.EnableFastQuestTicket();
            popup.SetUIActive(true, false);
        }

        UIStageDetailPopup stageDetailPopup = LobbyUIManager.Instance.GetUI("StageDetailPopup") as UIStageDetailPopup;
        if (stageDetailPopup != null)
        {
            stageDetailPopup.SetMultipleIndex(_multipleIndex, _multiGCFlag);
        }
        
        LobbyUIManager.Instance.Renewal("StageDetailPopup");

        //비밀 임무 팝업 이였을 경우 닫아준다.
        if (_isSecretQuest == true) {
            UICharSeletePopup charSeletePopup = LobbyUIManager.Instance.GetUI("CharSeletePopup") as UICharSeletePopup;
            if (charSeletePopup != null) {
                charSeletePopup.OnClickClose();
            }
        }
        
            
        OnClickClose();
    }

    public void OnClick_NoBtn()
    {
        OnClickClose();
    }
}
