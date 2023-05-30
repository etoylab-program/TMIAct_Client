using UnityEngine;
using System.Collections;

public class UIUserWelcomeLeftListSlot : FSlot {

    [SerializeField] private UISprite UnSelectSpr;
    [SerializeField] private UILabel UnSelectLabel;
    [SerializeField] private UISprite SelectSpr;
    [SerializeField] private UILabel SelectLabel;
    [SerializeField] private UISprite NoticeSpr;
    private UIUserWelcomeEventPopup uiPopup;
    private DailyMissionData.Piece MisstionData;

    public void UpdateSlot(int index, DailyMissionData.Piece piece, UIUserWelcomeEventPopup popup) 	//Fill parameter if you need
	{
        MisstionData = piece;
        uiPopup = popup;
        SetState(false);

        if (uiPopup.SelectDay == index)
        {
            SelectSpr.SetActive(true);
            SelectLabel.SetActive(true);

            SelectLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1603), piece.Day);
        }
        else
        {
            UnSelectSpr.SetActive(true);
            UnSelectLabel.SetActive(true);

            UnSelectLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1603), piece.Day);
        }

        //Notice
        bool noticeSprState = false;

        //n일 미션 테이블 데이터 확인        
        var _DailyMissionParams = GameInfo.Instance.GameTable.DailyMissions.FindAll(x => x.GroupID == piece.GroupID && x.Day == piece.Day);
        int max = _DailyMissionParams.Count;
        for (int i = 0; i < max; i++)
        {
            if (piece.NoVal[i] == 0)
            {
                if (!GameSupport.IsComplateMissionRecive(piece.RwdFlag, i))
                {
                    noticeSprState = true;
                    break;
                }
            }
        }
        NoticeSpr.SetActive(noticeSprState);        
    }


    private void SetState(bool state)
    {
        UnSelectSpr.SetActive(state);
        UnSelectLabel.SetActive(state);
        SelectSpr.SetActive(state);
        SelectLabel.SetActive(state);
        NoticeSpr.SetActive(state);
    }

    public void OnClick_Slot()
    {
        if (uiPopup == null) return;

        uiPopup.SetSelectDay(MisstionData.Day - 1);
    }
}


