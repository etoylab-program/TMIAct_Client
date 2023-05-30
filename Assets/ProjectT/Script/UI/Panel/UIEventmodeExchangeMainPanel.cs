using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEventmodeExchangeMainPanel : FComponent
{
    [System.Serializable]
    public class UIGoodsTex
    {
        public UIGoodsUnit goodsUnit;
        public UITexture texIcon;
        public GameObject bgSpr;

        public void SetActive(bool b)
        {
            goodsUnit.gameObject.SetActive(b);
            bgSpr.SetActive(b);
        }
    }

	public UIButton kEventEnterBtn;
	public UISprite kEndTex;
	public UISprite kEventEnterSpr;
	public UIButton kEventExchangeBtn;
	public UILabel kEventExchangeLabel;
	public UISprite kEventExchangeSpr;
	//public UISprite kEventLogoSpr;
	public UISprite kDaySpr;
	public UILabel kDayLabel;
	//public UISprite kBGSpr;
	public UIButton kEventRuleBtn;
	public UILabel kRuleLabel;

    public UITexture kBGTex;
    public UITexture kLogoTex;

    public List<UIGoodsTex> kGoodsUnits;

    private EventSetData _eventSetData;

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

        Log.Show(eventId);
        _eventSetData = GameInfo.Instance.GetEventSetData(eventId);

        if (_eventSetData == null)
            return;

        //200122 - 현재 적용 안됨 프리팹에 있는 배경으로 대체중
        LobbyUIManager.Instance.BG_Event(kBGIndex, _eventSetData.TableID, 0);

        _eventstarttime = GameSupport.GetTimeWithString(_eventSetData.TableData.StartTime);
        _eventendtime = GameSupport.GetTimeWithString(_eventSetData.TableData.EndTime, true);
        _eventopentime = GameSupport.GetTimeWithString(_eventSetData.TableData.PlayOpenTime);
        _eventclosetime = GameSupport.GetTimeWithString(_eventSetData.TableData.PlayCloseTime, true);

        //보유 아이템 셋팅
        SetGoods();

        _bisplayingevent = false;

        BannerData bannerdataBG = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.EVENT_MAINBG && x.BannerTypeValue == _eventSetData.TableID);
        if(bannerdataBG != null)
        {
			if (kBGTex.mainTexture != null)
			{
				DestroyImmediate(kBGTex.mainTexture, false);
				kBGTex.mainTexture = null;
			}

            kBGTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(bannerdataBG.UrlImage, true, bannerdataBG.Localizes[(int)eBannerLocalizeType.Url]);
        }

        BannerData bannerdataLogo = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.EVENT_LOGO && x.BannerTypeValue == _eventSetData.TableID);
        if(bannerdataLogo != null)
        {
			if (kLogoTex.mainTexture != null)
			{
				DestroyImmediate(kLogoTex.mainTexture, false);
				kLogoTex.mainTexture = null;
			}

            kLogoTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(bannerdataLogo.UrlImage, localizeFlag: bannerdataLogo.Localizes[((int)eBannerLocalizeType.Url)]);
        }

        kEndTex.gameObject.SetActive(true);

        GameSupport.ShowTutorialFlag(eTutorialFlag.EVENT);

        int eventState = GameSupport.GetJoinEventState(_eventSetData.TableID);
        if(eventState > (int)eEventState.EventNone)
        {
            if (eventState == (int)eEventState.EventPlaying)
            {
                _bisplayingevent = true;
                kDayLabel.textlocalize = GameSupport.GetEndTime(_eventclosetime);
                kEndTex.gameObject.SetActive(false);
            }
            else if(eventState == (int)eEventState.EventOnlyReward)
            {
                _bisplayingevent = false;
                kDayLabel.textlocalize = GameSupport.GetEndTime(_eventendtime);
            }
        }


	}
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}
 
    private void SetGoods()
    {
        SetGoods(kGoodsUnits[0], _eventSetData.TableData.EventItemID1);
        SetGoods(kGoodsUnits[1], _eventSetData.TableData.EventItemID2);
        SetGoods(kGoodsUnits[2], _eventSetData.TableData.EventItemID3);
        SetGoods(kGoodsUnits[3], _eventSetData.TableData.EventItemID4);
        SetGoods(kGoodsUnits[4], _eventSetData.TableData.EventItemID5);
        SetGoods(kGoodsUnits[5], _eventSetData.TableData.EventItemID6);
    }

    private void SetGoods(UIGoodsTex uIGoodsTex, int itemTableID)
    {
        if(itemTableID == 0)
        {
            uIGoodsTex.SetActive(false);
            return;
        }

        uIGoodsTex.SetActive(true);

        string texPath = "Icon/Item/" + GameInfo.Instance.GameTable.FindItem(x => x.ID == itemTableID).Icon;

        uIGoodsTex.texIcon.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", texPath);
        uIGoodsTex.goodsUnit.kTextLabel.textlocalize = GameInfo.Instance.GetItemIDCount(itemTableID).ToString();
    }
	
	public void OnClick_EventEnterBtn()
	{
        if (_bisplayingevent)
        {
            int eventId = (int)UIValue.Instance.GetValue(UIValue.EParamType.EventID);
            int stageId = GameSupport.GetEventSuitableStageID(eventId);
            UIValue.Instance.SetValue(UIValue.EParamType.EventStageID, stageId);
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.EVENT_CHANGE_STAGE);
        }
        else
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1299));
	}
	
	public void OnClick_EventExchangeBtn()
	{
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.EVENT_CHANGE_REWARD);
    }
	
	public void OnClick_EventRuleBtn()
	{
        UIValue.Instance.SetValue(UIValue.EParamType.RulePopupType, (int)UIEventRulePopup.eRulePopupType.EVENT_RULE);
        LobbyUIManager.Instance.ShowUI("EventRulePopup", true);
    }

    public void OnClick_StoryViewBtn()
    {
        Log.Show(_eventSetData.TableID);
        LobbyUIManager.Instance.ShowUI("EventmodeExchangeStoryPopup", true);
    }
}
