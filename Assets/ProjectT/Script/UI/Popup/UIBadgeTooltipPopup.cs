using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BadgeToolTipPopup
{
    public enum eToolTipDir
    {
        LEFT,
        RIGHT,
        NONE,
    }

    public static UIBadgeTooltipPopup GetBadgeToolTipPopup()
    {
        UIBadgeTooltipPopup popup = null;

        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
            popup = LobbyUIManager.Instance.GetUI<UIBadgeTooltipPopup>("BadgeTooltipPopup");

        return popup;
    }

    public static void Show(List<BadgeData> badgeList, GameObject target, eToolTipDir dir, eCharSelectFlag charSelectFlag, eContentsPosKind poskind)
    {
        UIBadgeTooltipPopup popup = GetBadgeToolTipPopup();
        if (popup == null)
            return;

        popup.InitBadgeToolTip(badgeList, target, dir, charSelectFlag, poskind);
    }
}

public class UIBadgeTooltipPopup : FComponent
{
    [System.Serializable]
    public class BadgeToolTipSubOpt
    {
        public UILabel kSubOptLabel;
        public GameObject kSubOptSlotObj;
        public UILabel kSubOptSlotLabel;
    }

	public GameObject kBG_L;
	public GameObject kBG_R;
	public UILabel kMainOptDescLabel;
    public UITexture kMainOptIcon;
	public UILabel kSetDescLabel;
	public UISprite kbgSpr;
    public GameObject kRootObj;
    public List<BadgeToolTipSubOpt> kSubOptList;

    private eContentsPosKind _contentsPosKind = eContentsPosKind._NONE_;
 

    public void InitBadgeToolTip(List<BadgeData> badgeList, GameObject target, BadgeToolTipPopup.eToolTipDir dir, eCharSelectFlag charSelectFlag, eContentsPosKind poskind)
    {
        _contentsPosKind = poskind;
        CancelInvoke("OnClickClose");
        SetUIActive(true);

        for (int i = 0; i < kSubOptList.Count; i++)
        {
            kSubOptList[i].kSubOptLabel.gameObject.SetActive(false);
            kSubOptList[i].kSubOptSlotObj.SetActive(false);
        }

        kMainOptIcon.gameObject.SetActive(false);        

        kBG_L.SetActive(dir == BadgeToolTipPopup.eToolTipDir.LEFT);
        kBG_R.SetActive(dir == BadgeToolTipPopup.eToolTipDir.RIGHT);

        Vector3 targetPos = UICamera.mainCamera.ViewportToScreenPoint(target.transform.localPosition);

        kRootObj.transform.localPosition = (dir == BadgeToolTipPopup.eToolTipDir.LEFT) ? 
            (new Vector3(target.transform.localPosition.x + 100f, 0, 0)) :
            (new Vector3(target.transform.localPosition.x - 100f, 0, 0));

        int mainOptID = 0;
        int mainOptEquipCnt = 0;
        kMainOptDescLabel.textlocalize = "-";

        BadgeData mainBadge = null;
        if (charSelectFlag == eCharSelectFlag.Preset)
        {
            int firstIndex = (int)eBadgeSlot.FIRST - 1;
            if (firstIndex < badgeList.Count)
            {
                mainBadge = badgeList[firstIndex];
            }
        }
        else
        {
            mainBadge = badgeList.Find(x => x.PosKind == (int)_contentsPosKind && x.PosSlotNum == ((int)eBadgeSlot.FIRST - 1));
        }
            
        if (mainBadge != null)
        {
            GameTable.BadgeOpt.Param mainParam = GameInfo.Instance.GameTable.BadgeOpts.Find(x => x.OptionID == mainBadge.OptID[(int)eBadgeOptSlot.FIRST]);
            if (mainParam != null)
            {
                kMainOptIcon.gameObject.SetActive(true);
                kMainOptIcon.mainTexture = GameSupport.GetBadgeIcon(mainBadge);

                mainOptID = mainBadge.OptID[(int)eBadgeOptSlot.FIRST];
                mainOptEquipCnt = GameSupport.GetMainOptEquipCnt(badgeList, mainOptID);
                kMainOptDescLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.YELLOW_TEXT_COLOR), FLocalizeString.Instance.GetText(mainParam.Desc + 100000));
            }
        }

        kSetDescLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1476), mainOptEquipCnt, (GameInfo.Instance.GameConfig.BadgeSetAddRate[mainOptEquipCnt] * 100f));

        int subOptLabelIdx = 0;
        for (int i = 0; i < ((int)eBadgeSlot._MAX_ - 1); i++)
        {
            BadgeData badgedata = null;
            if (charSelectFlag == eCharSelectFlag.Preset)
            {
                if (i < badgeList.Count)
                {
                    badgedata = badgeList[i];
                }
            }
            else
            {
                badgedata = badgeList.Find(x => x.PosKind == (int)_contentsPosKind && x.PosSlotNum == i);
            }

            if (badgedata == null)
            {
                continue;
            }

            for (int j = (int)eBadgeOptSlot.FIRST; j < (int)eBadgeOptSlot._MAX_; j++)
            {
                int optId = badgedata.OptID[j];
                string strOpt = GetBadgeOptDesc(badgedata, (eBadgeOptSlot)j, mainOptID, mainOptEquipCnt);
                if (!string.IsNullOrEmpty(strOpt))
                {
                    kSubOptList[subOptLabelIdx].kSubOptLabel.gameObject.SetActive(true);
                    kSubOptList[subOptLabelIdx].kSubOptSlotObj.SetActive(true);

                    kSubOptList[subOptLabelIdx].kSubOptLabel.textlocalize = strOpt;

                    int slotNum = badgedata.PosSlotNum + 1;
                    if (charSelectFlag == eCharSelectFlag.Preset)
                    {
                        slotNum = i + 1;
                    }

                    kSubOptList[subOptLabelIdx].kSubOptSlotLabel.textlocalize = slotNum.ToString();

                    subOptLabelIdx++;
                }
            }
        }
    }

    void OnClick()
    {
        SetUIActive(false);
    }

    private string GetBadgeOptDesc(BadgeData badgedata, eBadgeOptSlot optSlot, int mainOptID = 0, int equipCnt = 0)
    {
        string result = string.Empty;

        GameTable.BadgeOpt.Param param = GameInfo.Instance.GameTable.BadgeOpts.Find(x => x.OptionID == badgedata.OptID[(int)optSlot]);
        if(param == null)
        {
            return string.Empty;
        }
        else
        {
            float optValue = ((badgedata.OptVal[(int)optSlot] + badgedata.Level) * param.IncEffectValue) / (float)eCOUNT.MAX_RATE_VALUE * 100.0f;
            if (param.OptionID == mainOptID)
            {
                float addVal = GameInfo.Instance.GameConfig.BadgeSetAddRate[equipCnt] * optValue;
                Log.Show(param.Desc);

                string oriStr = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT), optValue));
                string addStr = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.BLUE_TEXT_COLOR), string.Format("(+{0})", string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETHREE_POINT_TEXT), addVal)));

                result = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.YELLOW_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText(param.Desc), string.Format(FLocalizeString.Instance.GetText(220), oriStr, addStr)));
            }
            else
            {
                result = string.Format(FLocalizeString.Instance.GetText(param.Desc), 
                    string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT), optValue)));
            }
        }
        return result;
    }
}
