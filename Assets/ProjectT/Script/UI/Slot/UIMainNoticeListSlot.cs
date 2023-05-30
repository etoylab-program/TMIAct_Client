using UnityEngine;
using System.Collections;

public class UIMainNoticeListSlot : FSlot
{
    public UISprite kBGSpr;
    public UISprite kSelSpr;
    public UILabel kTextLabel;

    private int _index;
    private NoticeBaseData _data;

    public void UpdateSlot(int index, NoticeBaseData data)  //Fill parameter if you need
    {
        _index = index;
        _data = data;

        kBGSpr.gameObject.SetActive(false);
        kSelSpr.gameObject.SetActive(false);
        kTextLabel.textlocalize = _data.Text;
    }

    public void OnClick_Slot()
    {
        if (_data.Type == NoticeBaseData.eTYPE.USER_ACHIEVEMENT)
        {
            //LobbyUIManager.Instance.ShowUI("MenuPopup", true);
            UIValue.Instance.SetValue(UIValue.EParamType.UserInfoPopup, 1);
            LobbyUIManager.Instance.ShowUI("UserInfoPopup", true);
        }
        else if (_data.Type == NoticeBaseData.eTYPE.CHAR_FAVOR)
        {
            LobbyUIManager.Instance.ShowUI("MenuPopup", true);
            LobbyUIManager.Instance.ShowUI("BookMainPopup", true);
            LobbyUIManager.Instance.ShowUI("BookCharListPopup", true);
            UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, _data.Value1);
            LobbyUIManager.Instance.ShowUI("BookCharInfoPopup", true);
        }
        else if (_data.Type == NoticeBaseData.eTYPE.CHAR_GRADEUP || _data.Type == NoticeBaseData.eTYPE.CHAR_AWAKEN)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, _data.uuid);
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, _data.Value1);
            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.STATUS);
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);

        }
        else if (_data.Type == NoticeBaseData.eTYPE.CHAR_SKILLOPEN)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, _data.uuid);
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, _data.Value1);
            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.SKILL);
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);
        }
        else if (_data.Type == NoticeBaseData.eTYPE.CHAR_SkLLSLOTOPEN)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, _data.uuid);
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, _data.Value1);
            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.SKILL);
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);
        }
        else if (_data.Type == NoticeBaseData.eTYPE.CHAR_CARDSLOTOPEN)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, _data.uuid);
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, _data.Value1);
            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.SUPPORTER);
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);
        }
        else if (_data.Type == NoticeBaseData.eTYPE.FACILITY_OPEN)
        {
            if (ParentGO == null)
                return;
            UIMainPanel panel = ParentGO.GetComponent<UIMainPanel>();
            if (panel == null)
                return;
            panel.OnClick_FacilityBtn(_data.Value1);
        }
        else if (_data.Type == NoticeBaseData.eTYPE.BOOK_OPEN || _data.Type == NoticeBaseData.eTYPE.CARD_FAVOR)
        {
            LobbyUIManager.Instance.ShowUI("BookMainPopup", true);
            UIValue.Instance.SetValue(UIValue.EParamType.BookItemListType, _data.Value1);
            LobbyUIManager.Instance.ShowUI("BookItemListPopup", true);

            bool bnewclick = false;
            if (_data.Value1 == (int)eBookGroup.Weapon)
            {
                WeaponBookData bookdata = GameInfo.Instance.WeaponBookList.Find(x => x.TableID == _data.Value2);
                if (bookdata == null)
                    return;

                UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, _data.Value2);
                LobbyUIManager.Instance.ShowUI("BookWeaponInfoPopup", true);

                if (!bookdata.IsOnFlag(eBookStateFlag.NEW_CHK))
                    bnewclick = true;
            }
            else if (_data.Value1 == (int)eBookGroup.Supporter)
            {
                CardBookData bookdata = GameInfo.Instance.CardBookList.Find(x => x.TableID == _data.Value2);
                if (bookdata == null)
                    return;

                UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, _data.Value2);
                LobbyUIManager.Instance.ShowUI("BookCardInfoPopup", true);

                if (!bookdata.IsOnFlag(eBookStateFlag.NEW_CHK))
                    bnewclick = true;
            }

            else if (_data.Value1 == (int)eBookGroup.Monster)
            {
                MonsterBookData bookdata = GameInfo.Instance.MonsterBookList.Find(x => x.TableID == _data.Value2);
                if (bookdata == null)
                    return;

                UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, _data.Value2);
                LobbyUIManager.Instance.ShowUI("BookMonsterInfoPopup", true);

                if (!bookdata.IsOnFlag(eBookStateFlag.NEW_CHK))
                    bnewclick = true;
            }

            if (bnewclick)
                GameInfo.Instance.Send_ReqBookNewConfirm(_data.Value1, _data.Value2, OnNetSetBookState);
        }
        else if (_data.Type == NoticeBaseData.eTYPE.WMISSION_COMPLATE)
        {
            LobbyUIManager.Instance.ShowUI("WeeklyMissionPopup", true);
        }
        else if (_data.Type == NoticeBaseData.eTYPE.CARD_WAKE)
        {
            var carddata = GameInfo.Instance.GetCardData(_data.uuid);
            if (carddata == null)
                return;
            UIValue.Instance.SetValue(UIValue.EParamType.CardUID, _data.uuid);
            LobbyUIManager.Instance.ShowUI("CardInfoPopup", true);
            LobbyUIManager.Instance.ShowUI("CardGradeUpPopup", true);
        }
        else if (_data.Type == NoticeBaseData.eTYPE.CARD_SKILLLEVELUP)
        {
            var carddata = GameInfo.Instance.GetCardData(_data.uuid);
            if (carddata == null)
                return;
            UIValue.Instance.SetValue(UIValue.EParamType.CardUID, _data.uuid);
            LobbyUIManager.Instance.ShowUI("CardInfoPopup", true);
            LobbyUIManager.Instance.ShowUI("CardSkillLevelUpPopup", true);
        }
        else if (_data.Type == NoticeBaseData.eTYPE.WEAPON_WAKE)
        {
            var weapondata = GameInfo.Instance.GetWeaponData(_data.uuid);
            if (weapondata == null)
                return;
            UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, _data.uuid);
            LobbyUIManager.Instance.ShowUI("WeaponInfoPopup", true);
            LobbyUIManager.Instance.ShowUI("WeaponGradeUpPopup", true);
        }
        else if (_data.Type == NoticeBaseData.eTYPE.WEAPON_SKILLLEVELUP)
        {
            var weapondata = GameInfo.Instance.GetWeaponData(_data.uuid);
            if (weapondata == null)
                return;
            UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, _data.uuid);
            LobbyUIManager.Instance.ShowUI("WeaponInfoPopup", true);
            LobbyUIManager.Instance.ShowUI("WeaponSkillLevelUpPopup", true);
        }
        else if (_data.Type == NoticeBaseData.eTYPE.MAIL)
        {
            GameInfo.Instance.Send_ReqMailList(0, (uint)GameInfo.Instance.GameConfig.MaxMailCnt, false, OnShowMailPopup);
        }
        else if (_data.Type == NoticeBaseData.eTYPE.BOOK_OPEN_COUNT_CHAR || _data.Type == NoticeBaseData.eTYPE.BOOK_OPEN_COUNT_COSTUME ||
            _data.Type == NoticeBaseData.eTYPE.BOOK_OPEN_COUNT_CARD || _data.Type == NoticeBaseData.eTYPE.BOOK_OPEN_COUNT_WEAPON || _data.Type == NoticeBaseData.eTYPE.BOOK_OPEN_COUNT_MONSTER)
        {
            LobbyUIManager.Instance.ShowUI("BookMainPopup", true);
        }
        else if (_data.Type == NoticeBaseData.eTYPE.DISPATCH_OPEN)
        {
            if (ParentGO == null)
                return;
            UIMainPanel panel = ParentGO.GetComponent<UIMainPanel>();
            if (panel == null)
                return;
            panel.OnClick_CardDispatch();
        }

    }
    private void OnShowMailPopup(int result, PktMsgType pktmsg)
    {
        LobbyUIManager.Instance.ShowUI("MailBoxPopup", true);
    }

    public void OnNetSetBookState(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;


    }
}