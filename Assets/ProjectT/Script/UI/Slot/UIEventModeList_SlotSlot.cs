using UnityEngine;
using System.Collections;

public class UIEventModeList_SlotSlot : FSlot
{
	public UILabel kstateLabel;
	public UILabel kday;
    //public UISprite kBannerSpr;
    public UITexture kBannerTex;
    //public UITexture kEvent_Banner_Tex;
    public UITexture kLogoTex;
    public UISprite kEventOffSpr;

    private EventSetData _eventsetdata;

    
    public void UpdateSlot(EventSetData eventSetData) 	//Fill parameter if you need
	{
        //77 시간관련 함수 GameSupport 이동 (배너에서 클릭시에도 입장가능 체크및 불가이유 보여줘야함) 추후 시간지나서 게임진입도 같은 로직으로 처리 되어야함
        //77 관련 코드 정리
        Log.Show(eventSetData.TableID);
        _eventsetdata = eventSetData;

        System.DateTime eventStartTime = GameSupport.GetTimeWithString(_eventsetdata.TableData.StartTime);
        System.DateTime eventEndTime = GameSupport.GetTimeWithString(_eventsetdata.TableData.EndTime, true);
        System.DateTime eventOpenTime = GameSupport.GetTimeWithString(_eventsetdata.TableData.PlayOpenTime);
        System.DateTime eventCloseTime = GameSupport.GetTimeWithString(_eventsetdata.TableData.PlayCloseTime, true);
        System.DateTime nowTime = GameSupport.GetCurrentServerTime();

        int state = GameSupport.GetJoinEventState(_eventsetdata.TableID);

        kBannerTex.gameObject.SetActive(false);
        kstateLabel.gameObject.SetActive(false);
        kday.gameObject.SetActive(false);

        

        if (state == (int)eEventState.EventPlaying)
        {   // 이벤트 플레이 가능
            BannerData bannerdata = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.EVENT_BANNER_NOW && x.BannerTypeValue == _eventsetdata.TableID);
            if(bannerdata != null)
            {
                kBannerTex.gameObject.SetActive(true);
                if (kBannerTex.mainTexture != null)
                {
                    DestroyImmediate(kBannerTex.mainTexture, false);
                    kBannerTex.mainTexture = null;
                }

                kBannerTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(bannerdata.UrlImage,
                    localizeFlag: bannerdata.Localizes[(int)eBannerLocalizeType.Url]);
            }
            kstateLabel.gameObject.SetActive(true);
            FLocalizeString.SetLabel(kstateLabel, 1310);
            kday.gameObject.SetActive(true);
            kday.textlocalize = GameSupport.GetEndTime(eventCloseTime);
        }
        else if (state == (int)eEventState.EventOnlyReward)
        {
            BannerData bannerdata = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.EVENT_BANNER_PAST && x.BannerTypeValue == _eventsetdata.TableID);
            if (bannerdata != null)
            {
                kBannerTex.gameObject.SetActive(true);
                if (kBannerTex.mainTexture != null)
                {
                    DestroyImmediate(kBannerTex.mainTexture, false);
                    kBannerTex.mainTexture = null;
                }

                kBannerTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(bannerdata.UrlImage,
                    localizeFlag: bannerdata.Localizes[(int)eBannerLocalizeType.Url]);
            }
            kstateLabel.gameObject.SetActive(true);
            FLocalizeString.SetLabel(kstateLabel, 1311);
            kday.gameObject.SetActive(true);
            kday.textlocalize = GameSupport.GetEndTime(eventEndTime);
        }
        
        {
            kEventOffSpr.SetActive(state == (int)eEventState.EventOnlyReward);
            
            BannerData bannerData = GameInfo.Instance.ServerData.BannerList.Find(x =>
                x.BannerType == (int)eBannerType.EVENT_LOGO && x.BannerTypeValue == _eventsetdata.TableID);
            if (bannerData != null)
            {
                kLogoTex.gameObject.SetActive(true);
                kLogoTex.mainTexture = null;
                StartCoroutine(GetTexture(kLogoTex, bannerData.UrlImage, bannerData.Localizes[(int)eBannerLocalizeType.Url]));
            }
        }
    }
    
    private IEnumerator GetTexture(UITexture texture, string url, bool localize)
    {
        while (texture.mainTexture == null)
        {
            texture.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(url, localizeFlag: localize);
            yield return null;
        }
    }

    public void OnClick_Slot()
	{
        if (_eventsetdata == null)
            return;

        int state = GameSupport.GetJoinEventState(_eventsetdata.TableID);

        if (state < (int)eEventState.EventNone)
        {
            if (state == (int)eEventState.EventNotStart)
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3052));
            else
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3053));
            return;
        }

        LobbyUIManager.Instance.HideUI("EventModePopup");

        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.FACILITYITEM || LobbyUIManager.Instance.PanelType == ePANELTYPE.FACILITY)
        {
            if (LobbyUIManager.Instance.PanelType == ePANELTYPE.FACILITY) LobbyUIManager.Instance.HideUI("FacilityPanel");
            else if (LobbyUIManager.Instance.PanelType == ePANELTYPE.FACILITYITEM) LobbyUIManager.Instance.HideUI("FacilityItemPanel");

            LobbyDoorPopup.Show(() => 
            {
                Lobby.Instance.MoveToLobby();
                GoToEventPanel(); 
            });

            return;
        }

        GoToEventPanel();
    }

    private void GoToEventPanel()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.EventID, _eventsetdata.TableData.EventID);
        switch ((eEventRewardKind)_eventsetdata.TableData.EventType)
        {
            case eEventRewardKind.RESET_LOTTERY:
                {
                    LobbyUIManager.Instance.SetPanelType(ePANELTYPE.EVENT_STORY_MAIN);
                }
                break;
            case eEventRewardKind.MISSION:
                break;
            case eEventRewardKind.EXCHANGE:
                {
                    Log.Show((eEventRewardKind)_eventsetdata.TableData.EventType);
                    LobbyUIManager.Instance.SetPanelType(ePANELTYPE.EVENT_CHANGE_MAIN);
                }
                break;
        }
    }
}
