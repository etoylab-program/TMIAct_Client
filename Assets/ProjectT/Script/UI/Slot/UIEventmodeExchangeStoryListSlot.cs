
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIEventmodeExchangeStoryListSlot : FSlot {
 

	public UILabel knameLabel;
	public GameObject kLockObj;
	public UILabel kLockDescLabel;

    private int _eventId;
    private int _scenarioGroupID;

    public void UpdateSlot(int eventId, UIEventmodeExchangeStoryPopup.EventStoryValues eventStoryValues)
    {
        _eventId = eventId;
        kLockObj.SetActive(true);

        List<GameTable.EventExchangeReward.Param> ListFind = GameInfo.Instance.GameTable.FindAllEventExchangeReward(x => x.EventID == _eventId &&
                                                                                                                         eventStoryValues.ListOpenCondition.Find(y => x.IndexID == y.ItemIndexID) != null &&
                                                                                                                         eventStoryValues.ListOpenCondition.Find(y => x.RewardStep == y.ItemRewardStepIndex) != null);
        if (ListFind == null || ListFind.Count <= 0)
        {
            return;
        }
        
        _scenarioGroupID = eventStoryValues.ScenarioGroupID;
        knameLabel.textlocalize = FLocalizeString.Instance.GetText(eventStoryValues.ItemString);

        //잠금 조건에 나올 아이템 이름 설정
        if (eventStoryValues.ListOpenCondition.Count == 2)
        {
            string[] arrItemName = new string[2];
            for(int i = 0; i < ListFind.Count; i++)
            {
                RewardData rewarddata = new RewardData(ListFind[i].ProductType, ListFind[i].ProductIndex, ListFind[i].ProductValue);
                arrItemName[i] = GameSupport.GetProductName(rewarddata);
            }

            kLockDescLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(3244), arrItemName[0], arrItemName[1]);
        }
        else if(eventStoryValues.ListOpenCondition.Count == 1)
        {
            RewardData rewarddata = new RewardData(ListFind[0].ProductType, ListFind[0].ProductIndex, ListFind[0].ProductValue);
            string itemName = GameSupport.GetProductName(rewarddata);

            kLockDescLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1516), itemName);
        }
        else
        {
            kLockDescLabel.textlocalize = FLocalizeString.Instance.GetText(3245);
        }

        List<EventSetData> ListEventSetData = GameInfo.Instance.EventSetDataList.FindAll(x => x.TableID == _eventId &&
                                                                                              eventStoryValues.ListOpenCondition.Find(y => x.RewardStep == y.ItemRewardStepIndex) != null);

        bool releaseLock = false;
        for (int i = 0; i < ListFind.Count; i++)
        {
            for (int j = 0; j < ListEventSetData.Count; j++)
            {
                for(int k = 0; k < eventStoryValues.ListOpenCondition.Count; k++)
                {
                    int useCnt = ListEventSetData[j].RewardItemCount[eventStoryValues.ListOpenCondition[k].ItemIndexID - 1];
                    if (ListFind[i].ExchangeCnt != 0 && useCnt == 0)
                    {
                        releaseLock = true;
                        break;
                    }
                }

                if(releaseLock)
                {
                    break;
                }
            }

            if (releaseLock)
            {
                break;
            }
        }

        kLockObj.SetActive(!releaseLock);
    }
 
	public void OnClick_Slot()
	{
        //Log.Show(_tabledata.IndexID, Log.ColorType.Red);
        if (kLockObj.activeSelf)
            return;

        Log.Show("########");
        LobbyUIManager.Instance.HideUI("EventmodeExchangeMainPanel", false);
        LobbyUIManager.Instance.HideUI("TopPanel", false);
        ScenarioMgr.Instance.Open(_scenarioGroupID, () => {
            //LobbyUIManager.Instance.SetPanelType(ePANELTYPE.EVENT_CHANGE_MAIN);
            LobbyUIManager.Instance.ShowUI("EventmodeExchangeMainPanel", false);
            LobbyUIManager.Instance.ShowUI("TopPanel", false);
        });

        LobbyUIManager.Instance.HideUI("EventmodeExchangeStoryPopup");
    }
 
}
