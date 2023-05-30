using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEventmodeStoryMainPanel : FComponent
{

	public UILabel kDayLabel;
	public UIButton kEventEnterBtn;
	public UIButton kEventGachaBtn;
	public UILabel kEventGachaLabel;
	public UISprite kEventGachaSpr;
	public UISprite kicon_bgSpr;
	public UIButton kEventRuleBtn;
    public UISprite kEventEndSpr;
    public UITexture kBGTex;
    public UITexture kLogoTex;
    public UITexture kGoodUnitTex;
    public UILabel kGoodUnitLb;
    private bool _bisplayingevent;

    private System.DateTime _eventstarttime;
    private System.DateTime _eventendtime;
    private System.DateTime _eventopentime;
    private System.DateTime _eventclosetime;

    private UIBGUnit m_chapterBgUnit = null;

 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        int eventId = (int)UIValue.Instance.GetValue(UIValue.EParamType.EventID);
        EventSetData eventsetdata = GameInfo.Instance.GetEventSetData(eventId);

        //200122 - 현재 적용 안됨 프리팹에 있는 배경으로 대체중
        LobbyUIManager.Instance.BG_Event(kBGIndex, eventsetdata.TableID, 0);

        //nowTime = new System.DateTime(2019, 4, 30, 6, 1, 0);
        _eventstarttime = GameSupport.GetTimeWithString(eventsetdata.TableData.StartTime);
        _eventendtime = GameSupport.GetTimeWithString(eventsetdata.TableData.EndTime, true);
        _eventopentime = GameSupport.GetTimeWithString(eventsetdata.TableData.PlayOpenTime);
        _eventclosetime = GameSupport.GetTimeWithString(eventsetdata.TableData.PlayCloseTime, true);

        //GameInfo.Instance.GameTable.FindItem(x => x.ID == (int)eventSetData.TableData.EventItemID1).Icon
        string path = "Icon/Item/" + GameInfo.Instance.GameTable.FindItem(x => x.ID == (int)eventsetdata.TableData.EventItemID1).Icon;
        kGoodUnitTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", path);
        kGoodUnitLb.textlocalize = GameInfo.Instance.GetItemIDCount((int)eventsetdata.TableData.EventItemID1).ToString();

        _bisplayingevent = false;

        BannerData bannerdataBG = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.EVENT_MAINBG && x.BannerTypeValue == eventsetdata.TableID);
        if (bannerdataBG != null)
        {
			if (kBGTex.mainTexture != null)
			{
				DestroyImmediate(kBGTex.mainTexture, false);
				kBGTex.mainTexture = null;
			}
			
			kBGTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(bannerdataBG.UrlImage, true, 
		        bannerdataBG.Localizes[(int)eBannerLocalizeType.Url]);
        }

        BannerData bannerdataLogo = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.EVENT_LOGO && x.BannerTypeValue == eventsetdata.TableID);
        if (bannerdataLogo != null)
        {
			if (kLogoTex.mainTexture != null)
			{
				DestroyImmediate(kLogoTex.mainTexture, false);
				kLogoTex.mainTexture = null;
			}

	        kLogoTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(bannerdataLogo.UrlImage,
		        localizeFlag: bannerdataLogo.Localizes[(int)eBannerLocalizeType.Url]);
        }
        kEventEndSpr.gameObject.SetActive(true);

        GameSupport.ShowTutorialFlag(eTutorialFlag.EVENT);

        // 이벤트 상태 확인
        int state = GameSupport.GetJoinEventState(eventsetdata.TableID);
        if (state > (int)eEventState.EventNone)
        {
            if (state == (int)eEventState.EventPlaying)
            {   // 이벤트 플레이 가능
                _bisplayingevent = true;
                kDayLabel.textlocalize = GameSupport.GetEndTime(_eventclosetime);
                kEventEndSpr.gameObject.SetActive(false);
                return;
            }
            else if (state == (int)eEventState.EventOnlyReward)
            {   // 이벤트 보상 획득까지만 가능
                _bisplayingevent = false;
                kDayLabel.textlocalize = GameSupport.GetEndTime(_eventendtime);
                return;
            }
        }
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}
 
	public void OnClick_EventEnterBtn()
	{
        if (_bisplayingevent)
        {
            int eventId = (int)UIValue.Instance.GetValue(UIValue.EParamType.EventID);
            int stageid = GameSupport.GetEventSuitableStageID(eventId);
            UIValue.Instance.SetValue(UIValue.EParamType.EventStageID, stageid);
            UIValue.Instance.SetValue(UIValue.EParamType.EventStagePopupType, (int)eEventStageType.EVENT);
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.EVENT_STORY_STAGE);
        }
        else
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1299));
    }
	
	public void OnClick_EventGachaBtn()
	{
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.EVENT_STORY_GACHA);
    }
	
	public void OnClick_EventRuleBtn()
	{
        UIValue.Instance.SetValue(UIValue.EParamType.RulePopupType, (int)UIEventRulePopup.eRulePopupType.EVENT_RULE);
        LobbyUIManager.Instance.ShowUI("EventRulePopup", true);
    }

    string GetDay(System.DateTime _dt)
    {
        string resultStr = string.Empty;
        resultStr = FLocalizeString.Instance.GetText(190 + (int)_dt.DayOfWeek);
        return resultStr;
    }
}
