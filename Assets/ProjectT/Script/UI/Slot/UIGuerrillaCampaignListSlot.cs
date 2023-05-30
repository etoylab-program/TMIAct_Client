using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGuerrillaCampaignListSlot : FSlot
{
	public GameObject kNotReward;
	public UILabel kNotDecsLabel;
	public UILabel kNotNameLabel;
	public UILabel kNotTimeLabel;
	public UIButton kNotWarpBtn;
    public UICampaignMarkUnit kCampaignMarkUnit;
    //public UISprite kNotIconSpr;
    public GameObject kReward;
    public UIRewardListSlot kRewardListSlot;
    public UILabel kRewardDesclabel;
    public UILabel kRewardNameLabel;
    public UILabel kRewardTimeLabel;
    public UIButton kRewardWarpBtn;

	public UILabel kDescLabel;
	public UIGaugeUnit kGaugeUnit;
	public UISprite kGaugeEFFSpr;
	public UISprite kBGTopSpr;
	public UILabel kNameLabel;
	public UILabel kTimeLabel;
	public UISprite kBGEFFSpr;

    public UIButton kGetBtn;

    private UIGuerrillaCampaignPopup.GuerrillaSlotItem _guerrillacampdata;
    private GllaMissionData _userGuerrillacampdata;
    private int _index;
    public void UpdateSlot( int index, UIGuerrillaCampaignPopup.GuerrillaSlotItem guerrillacampdata) 	//Fill parameter if you need
	{
        if ( guerrillacampdata == null ) {
            return;
        }

        _index = index;
        _guerrillacampdata = guerrillacampdata;
		_userGuerrillacampdata = null;

        kReward.SetActive(false);
        kNotReward.SetActive(true);
        //kNotWarpBtn.gameObject.SetActive(false);
        kRewardWarpBtn.gameObject.SetActive(false);

        kGetBtn.gameObject.SetActive(false);

        kNotNameLabel.textlocalize = FLocalizeString.Instance.GetText(_guerrillacampdata.Name);
        kNotDecsLabel.textlocalize = FLocalizeString.Instance.GetText(_guerrillacampdata.Desc);
        kRewardNameLabel.textlocalize = FLocalizeString.Instance.GetText(_guerrillacampdata.Name);
        kRewardDesclabel.textlocalize = FLocalizeString.Instance.GetText(_guerrillacampdata.Desc);
        if (guerrillacampdata.GuerrillaType == (int)UIGuerrillaCampaignPopup.eGuerrillaType.Campaign)
        {
            kCampaignMarkUnit.kCampaignTextSpr.spriteName = GameSupport.GetGuerrillaCampaignSprName(guerrillacampdata.Type);
            kCampaignMarkUnit.kCampaignTextSpr.MakePixelPerfect();
        }
        else if(guerrillacampdata.GuerrillaType == (int)UIGuerrillaCampaignPopup.eGuerrillaType.Mission)
        {
            kNotReward.SetActive(false);
            kReward.SetActive(true);
            kRewardListSlot.UpdateSlot(new RewardData(guerrillacampdata.RewardType, guerrillacampdata.RewardIndex, guerrillacampdata.RewardValue), true);
            _userGuerrillacampdata = GameInfo.Instance.GllaMissionList.Find(x => x.GroupID == _guerrillacampdata.GroupID);// && x.Step == _guerrillacampdata.GroupOrder);
            if(_userGuerrillacampdata == null)
            {
                kGaugeUnit.SetText(string.Format("0/{0}", guerrillacampdata.Count));
                kGaugeUnit.InitGaugeUnit(0f, 1);
            }
            else
            {
                kGaugeUnit.SetText(string.Format("{0}/{1}", _userGuerrillacampdata.Count, guerrillacampdata.Count));
                kGaugeUnit.InitGaugeUnit((float)((float)_userGuerrillacampdata.Count / (float)guerrillacampdata.Count), 1);

                //받기버튼
                if (_userGuerrillacampdata.Count >= guerrillacampdata.Count)
                {
                    kGetBtn.gameObject.SetActive(true);
                }
                Log.Show(guerrillacampdata.Type + " / " + guerrillacampdata.Condition + " / " + _userGuerrillacampdata.Count + " / " + guerrillacampdata.Count, Log.ColorType.Green);
            }
        }

        System.TimeSpan diffTime = _guerrillacampdata.EndDate - GameSupport.GetCurrentServerTime();
        if (diffTime.Days > 365 * 2)
        {
            kNotTimeLabel.textlocalize = FLocalizeString.Instance.GetText(1716);
            kRewardTimeLabel.textlocalize = FLocalizeString.Instance.GetText(1716);
        }
        else
        {
            string remaintime = GameSupport.GetRemainTimeString(_guerrillacampdata.EndDate, GameSupport.GetCurrentServerTime());
            kNotTimeLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1261), remaintime);
            kRewardTimeLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1261), remaintime);
        }
        
    }
 
	public void OnClick_Slot()
	{
        if (_userGuerrillacampdata == null)
            return;

        if(_userGuerrillacampdata.Count >= _guerrillacampdata.Count)
        {
            Log.Show("OnClick_Slot");
            
            GameInfo.Instance.Send_ReqRewardGllaMission(_userGuerrillacampdata.GroupID, OnRewardGuerrillMission);
        }
    }

    public void OnRewardGuerrillMission(int result, PktMsgType pktmsg)
    {
        //3122 게릴라 미션을 달성했습니다. 선물함을 확인해주세요.
        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3122));

        UIGuerrillaCampaignPopup parentPopup = ParentGO.GetComponent<UIGuerrillaCampaignPopup>();
        if (parentPopup != null)
            parentPopup.InitComponent();
    }
	
	public void OnClick_NotWarpBtn()
	{
        ClickWarp();
    }
	
	public void OnClick_WarpBtn()
	{
        ClickRewardWarp();
    }

    /*
    // 콘텐츠 위치 분류 위키 링크 -> http://10.10.10.90/Mediawiki/index.php/Project-A/일반/ProjectDefine/eContentsPosKind
    public enum eContentsPosKind : System.Byte
    {
        _NONE_ = 0,    // 사용되지 않음.
        _START_ = 1,

        CHAR = _START_,
        FACILITY,
        COSTUME,
        MONSTER,
        CARD,
        GEM,
        ITEM,
        WEAPON,

        _END_,

        _MAX_ = _END_,
    };
    */

    private void ClickRewardWarp()
    {
    }

    private void ClickWarp()
    {
        if (_guerrillacampdata.Type == eGuerrillaCampaignType.GC_StageClear_ExpRateUP.ToString() ||     //경험치 획득량 증가
            _guerrillacampdata.Type == eGuerrillaCampaignType.GC_StageClear_GoldRateUP.ToString() ||    //골드 획득량 증가
            _guerrillacampdata.Type == eGuerrillaCampaignType.GC_StageClear_ItemCntUP.ToString() ||     //아이템 획득수 증가
            _guerrillacampdata.Type == eGuerrillaCampaignType.GC_StageClear_APRateDown.ToString() ||    //AP 소모량 감소
            _guerrillacampdata.Type == eGuerrillaCampaignType.GC_StageClear_FavorRateUP.ToString() ||   //서포터 호감도 획득량 증가
            _guerrillacampdata.Type == eGuerrillaCampaignType.GC_Stage_Multiple.ToString())             //스테이지 x배수
        {
            GameSupport.MoveUI(eUIMOVEFUNCTIONTYPE.UIPANEL.ToString(), ePANELTYPE.STORYMAIN.ToString(), "", "");
        }
        else if (_guerrillacampdata.Type == eGuerrillaCampaignType.GC_Upgrade_ExpRateUP.ToString() || //강화 시 경험치 획득량 증가
                 _guerrillacampdata.Type == eGuerrillaCampaignType.GC_Upgrade_PriceRateDown.ToString() || //강화 시 골드 소모량 감소
                 _guerrillacampdata.Type == eGuerrillaCampaignType.GC_Upgrade_SucNorRateDown.ToString() || //강화 시 일반 성공 확률 감소
                 _guerrillacampdata.Type == eGuerrillaCampaignType.GC_ItemSell_PriceRateUp.ToString()) //판매 시 골드 획득량 증가
        {
            GameSupport.MoveUI(eUIMOVEFUNCTIONTYPE.UIPANEL.ToString(), ePANELTYPE.ITEM.ToString(), "0", "");
        }
        else if (_guerrillacampdata.Type == eGuerrillaCampaignType.GC_Arena_CoinRateUP.ToString()) //아레나 코인 획득량 증가
        {
            if (GameInfo.Instance.UserData.Level < GameInfo.Instance.GameConfig.ArenaOpenRank)
            {
                MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LOCKMSG), GameInfo.Instance.GameConfig.ArenaOpenRank));
                return;
            }
            GameSupport.MoveUI(eUIMOVEFUNCTIONTYPE.UIPANEL.ToString(), ePANELTYPE.ARENA.ToString(), "", "");
        }
        else if (_guerrillacampdata.Type == eGuerrillaCampaignType.GC_Rotation_OpenCashSale.ToString())
        {
            GameSupport.MoveUI(eUIMOVEFUNCTIONTYPE.UIPANEL.ToString(), ePANELTYPE.GACHA.ToString(), "ROTATION", "");
        }
        else if( _guerrillacampdata.Type == eGuerrillaCampaignType.GC_Gacha_DPUP.ToString() ) {
            GameSupport.MoveUI( eUIMOVEFUNCTIONTYPE.UIPANEL.ToString(), ePANELTYPE.GACHA.ToString(), "DESIRE", "" ); // 염원의 기운 획득량 증가
        }
		else if ( _guerrillacampdata.Type.ToLower() == eGuerrillaCampaignType.GC_RAID_HPRESTORE.ToString().ToLower() ) {
			UITopPanel topPanel = LobbyUIManager.Instance.GetUI<UITopPanel>( "TopPanel" );
			if ( topPanel != null ) {
				topPanel.SetTopStatePlay( UITopPanel.eTOPSTATE.STAGE );
			}
			GameSupport.MoveUI( eUIMOVEFUNCTIONTYPE.UIPANEL.ToString(), ePANELTYPE.RAID_MAIN.ToString(), "", "" );
		}
	}
}
