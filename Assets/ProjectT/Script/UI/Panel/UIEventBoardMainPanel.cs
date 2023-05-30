using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventBoardMainPanel : FComponent
{
    [Header("UIEventBoardMainPanel")]
    [SerializeField] private FList bannerList = null;
    [SerializeField] private List<GameObject> eventList = null;
    [SerializeField] private UITexture eventBgTex = null;

    private List<GameClientTable.EventPage.Param> _activeEventList = new List<GameClientTable.EventPage.Param>();
    private int _selectIndex = 0;
    private GameObject _currentGameObject = null;

    public override void Awake()
    {
        base.Awake();

        bannerList.EventUpdate = BannerUpdate;
        bannerList.EventGetItemCount = BannerCount;
        bannerList.InitBottomFixing();
        bannerList.UpdateList();

        foreach (GameObject eventObj in eventList)
        {
            eventObj.SetActive(false);
        }
    }

    public override void OnEnable()
    {
		_selectIndex = 0;

		SetBannerList();
		SetEventData();

		base.OnEnable();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        bannerList.RefreshNotMoveAllItem();
        bannerList.SpringSetFocus(_selectIndex, 0.5f);
    }

    private void SetBannerList()
    {
        _activeEventList.Clear();

        foreach (GameClientTable.EventPage.Param eventPage in GameInfo.Instance.GameClientTable.EventPages)
        {
            switch((eLobbyEventType)eventPage.Type)
            {
                case eLobbyEventType.Bingo:
                    {
                        GameTable.BingoEvent.Param bingoEvent = LobbyUIManager.Instance.GetBingoEvent(eventPage);
                        if (bingoEvent == null)
                        {
                            continue;
                        }
                    } break;
                case eLobbyEventType.Achieve:
                    {
                        GameTable.AchieveEvent.Param achieveEvent = LobbyUIManager.Instance.GetAchieveEvent(eventPage);
                        if (achieveEvent == null)
                        {
                            continue;
                        }
                    } break;
            }

            _activeEventList.Add(eventPage);
        }
    }

    private void SetEventData()
    {
        if (_selectIndex < 0 || _activeEventList.Count <= _selectIndex)
        {
            return;
        }

        GameClientTable.EventPage.Param eventPage = _activeEventList[_selectIndex];
        int eventTypeIndex = eventPage.Type - 1;
        if (eventTypeIndex < 0 || eventList.Count <= eventTypeIndex)
        {
            return;
        }

        _currentGameObject?.SetActive(false);

        switch ((eLobbyEventType)eventPage.Type)
        {
            case eLobbyEventType.Bingo:
                {
                    _currentGameObject = eventList[eventTypeIndex];

                    UIBingoEvent bingoEvent = _currentGameObject.GetComponent<UIBingoEvent>();
                    if (bingoEvent != null)
                    {
                        bingoEvent.SetData(eventPage.TypeValue);
                    }

                    GameTable.BingoEvent.Param bingoEventParam = GameInfo.Instance.GameTable.FindBingoEvent(eventPage.TypeValue);
                    if (bingoEventParam != null)
                    {
                        eventBgTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("ui", $"UI/UITexture/Event/{bingoEventParam.BGTaxture}.png");
                    }
                }
                break;
            case eLobbyEventType.Achieve:
                {
                    _currentGameObject = eventList[eventTypeIndex];

                    UIAchieveEvent achieveEvent = _currentGameObject.GetComponent<UIAchieveEvent>();
                    if (achieveEvent != null)
                    {
                        achieveEvent.SetData(eventPage.TypeValue);
                    }

                    GameTable.AchieveEvent.Param achieveEventParam = GameInfo.Instance.GameTable.FindAchieveEvent(eventPage.TypeValue);
                    if (achieveEventParam != null)
                    {
                        eventBgTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("ui", $"UI/UITexture/Event/{achieveEventParam.BGTaxture}.png");
                    }
                }
                break;
        }

        _currentGameObject?.SetActive(true);
    }

    private void BannerUpdate(int index, GameObject obj)
    {
        UIBannerSlot bannerSlot = obj.GetComponent<UIBannerSlot>();
        if (bannerSlot == null)
        {
            return;
        }

        if (bannerSlot.ParentGO == null)
        {
            bannerSlot.ParentGO = this.gameObject;
        }

        GameClientTable.EventPage.Param eventPage = null;
        if (0 <= index && index < _activeEventList.Count)
        {
            eventPage = _activeEventList[index];
        }

        bannerSlot.UpdateSlot(UIBannerSlot.ePosType.RenewalEvent, index, _selectIndex, eventPage);
    }

    private int BannerCount()
    {
        return _activeEventList.Count;
    }

    public void SelectBanner(int index)
    {
        if (index == _selectIndex)
        {
            return;
        }

        _selectIndex = index;

        SetEventData();
        Renewal();
    }
}
