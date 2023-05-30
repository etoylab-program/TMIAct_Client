using UnityEngine;
using System.Collections;

public class UIInfluenceRankingListSlot : FSlot
{
    [SerializeField] private GameObject k1stDeco;    
    [SerializeField] private UILabel kLeftTopLabel;
    [SerializeField] private UILabel kLeftBottomLabel;
    [SerializeField] private UISprite kLeftBG;

    [SerializeField] private UILabel kRightTopLabel;
    [SerializeField] private UILabel kRightBottomLabel;

    [SerializeField] private UITexture kUserBadgeTexture;

    [SerializeField] private GameObject MeObj;

    [SerializeField] private UISprite kOutLineSpr;

    private Color _1stLeftBGColor = new Color(0.7843f, 0.5529f, 0f, 1f);
    private Color _commonLeftBGColor = new Color(0.298f, 0f, 0f, 1f);

    private InfluenceRankData.Piece Param;

    public void UpdateData(InfluenceRankData.Piece _param)
    {
        Param = _param;
        SetState(false);
        if (Param == null) return;

        kLeftTopLabel.SetActive(true);
        kLeftBottomLabel.SetActive(true);
        kRightTopLabel.SetActive(true);
        kRightBottomLabel.SetActive(true);
        kUserBadgeTexture.SetActive(true);

        kLeftTopLabel.textlocalize = string.Format("{0:#,#}", Param.InfluencePoint);
        kLeftBottomLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1053), Param.Rank);

        kRightTopLabel.textlocalize = string.Format("{0}", Param.Level);
        kRightBottomLabel.textlocalize = Param.GetNickName();

        LobbyUIManager.Instance.GetUserMarkIcon(ParentGO, this.gameObject, (int)Param.MarkID, ref kUserBadgeTexture);

        if (GameInfo.Instance.UserData.UUID == (long)Param.UUID)
            MeObj.SetActive(true);

        kLeftBG.SetActive(true);
        kOutLineSpr.SetActive(true);
        if (Param.Rank == 1)
        {
            k1stDeco.SetActive(true);
            kLeftBG.color = _1stLeftBGColor;
            kOutLineSpr.color = _1stLeftBGColor;
        }
        else
        {
            kLeftBG.color = _commonLeftBGColor;
            kOutLineSpr.color = _commonLeftBGColor;
        }


    }

    private void SetState(bool state)
    {
        k1stDeco.SetActive(state);
        kLeftTopLabel.SetActive(state);
        kLeftBottomLabel.SetActive(state);
        kLeftBG.SetActive(state);

        kRightTopLabel.SetActive(state);
        kRightBottomLabel.SetActive(state);

        kUserBadgeTexture.SetActive(state);

        MeObj.SetActive(state);

        kOutLineSpr.SetActive(state);
    }
}
