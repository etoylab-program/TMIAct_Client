using UnityEngine;
using System.Collections;

public class UIUserIconListSlot : FSlot
{
    public UISprite kBGSpr;
    public UITexture kIcoTex;
    public UISprite kSelSpr;
    public UISprite kLockSpr;

    [Header("Circle")]
    public UISprite kCircleSpr;
    public UITexture kCircleTex;

    private int _index;

    private GameTable.UserMark.Param _userMarkParam;
    private GameTable.CircleMark.Param _circleMarkParam;

    public void UpdateSlot(int index, GameTable.UserMark.Param data, int seleteId)  //Fill parameter if you need
    {
        _index = index;
        _userMarkParam = data;

        if (data == null)
        {
            return;
        }
        
        kIcoTex.SetActive(true);

        LobbyUIManager.Instance.GetUserMarkIcon(ParentGO, this.gameObject, _userMarkParam.ID, ref kIcoTex);

        bool isHaveUserMark = GameInfo.Instance.IsUserMark(_userMarkParam.ID);
        if (isHaveUserMark)
        {
            kIcoTex.color = new Color(1, 1, 1, 1);
        }
        else
        {
            kLockSpr.gameObject.SetActive(true);
            kIcoTex.color = GameInfo.Instance.GameConfig.TextColor[1];
        }

        kSelSpr.gameObject.SetActive(_userMarkParam.ID == seleteId);
        kLockSpr.gameObject.SetActive(!isHaveUserMark);
    }

    public void UpdateSlot(int index, GameTable.CircleMark.Param data, int selectTableId)
    {
        _index = index;
        _circleMarkParam = data;

        if (data == null)
        {
            return;
        }
        
        kIcoTex.SetActive(false);

        eCircleMarkType circleFlagType = (eCircleMarkType)_circleMarkParam.Marktype;

        kCircleTex.SetActive(circleFlagType != eCircleMarkType.COLOR);
        kCircleSpr.SetActive(circleFlagType == eCircleMarkType.COLOR);

        if (kCircleTex.gameObject.activeSelf)
        {
            kCircleTex.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture(_circleMarkParam.ID);
        }

        if (kCircleSpr.gameObject.activeSelf)
        {
            kCircleSpr.color = LobbyUIManager.Instance.GetCircleMarkColor(_circleMarkParam.ID);
        }

        bool isHaveMark = GameInfo.Instance.IsCircleMark(circleFlagType, _circleMarkParam.ID);
        kSelSpr.gameObject.SetActive(_circleMarkParam.ID == selectTableId);
        kLockSpr.gameObject.SetActive(!isHaveMark);
    }

    public void OnClick_Slot()
    {
        UIUserIconSeletePopup userIconSeletePopup = ParentGO.GetComponent<UIUserIconSeletePopup>();
        if (userIconSeletePopup != null)
        {
            if (_userMarkParam == null)
            {
                return;
            }

            userIconSeletePopup.SetSeleteID(_userMarkParam.ID);
        }

        UICircleMarkChangePanel circleLobbyMarkBuyPanel = ParentGO.GetComponent<UICircleMarkChangePanel>();
        if (circleLobbyMarkBuyPanel != null)
        {
            if (_circleMarkParam == null)
            {
                return;
            }

            circleLobbyMarkBuyPanel.SetSeleteID(_circleMarkParam.ID);
        }
    }
}
