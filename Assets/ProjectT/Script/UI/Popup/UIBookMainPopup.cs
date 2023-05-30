using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBookMainPopup : FComponent
{
    public UIButton kCharBookBtn;
    public UIButton kCostumeBookBtn;
    public UIButton kWeaponBookBtn;
    public UIButton kCardBookBtn;
    public UIButton kMonsterBookBtn;
    public UIGaugeUnit kCharBookGaugeUnit;
    public UIGaugeUnit kCostumeBookGaugeUnit;
    public UIGaugeUnit kWeaponBookGaugeUnit;
    public UIGaugeUnit kCardBookGaugeUnit;
    public UIGaugeUnit kMonsterBookGaugeUnit;
    public UISprite kCharBookNoticeSpr;
    public UISprite kCostumeBookNoticeSpr;
    public UISprite kWeaponBookNoticeSpr;
    public UISprite kCardBookNoticeSpr;
    public UISprite kMonsterBookNoticeSpr;    
    private int _maxweaponcount;
    private int _maxcardcount;
    private int _maxmonstercount;


    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        var bookweaponlist = GameInfo.Instance.GameClientTable.FindAllBook(x => x.Group == (int)eBookGroup.Weapon);
        var bookcardlist = GameInfo.Instance.GameClientTable.FindAllBook(x => x.Group == (int)eBookGroup.Supporter);
        var bookmonsterlist = GameInfo.Instance.GameClientTable.FindAllBook(x => x.Group == (int)eBookGroup.Monster);
        _maxweaponcount = bookweaponlist.Count;
        _maxcardcount = bookcardlist.Count;
        _maxmonstercount = bookmonsterlist.Count;
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        int now = GameInfo.Instance.CharList.Count;
        int max = GameInfo.Instance.GameTable.Characters.Count;
        string text = string.Format("{0:#,##0}/{1:#,##0}", now, max);
        float f = (float)now / (float)max;
        kCharBookGaugeUnit.InitGaugeUnit(f);
        kCharBookGaugeUnit.SetText(text);

        List<GameClientTable.Book.Param> costumeList = GameInfo.Instance.GameClientTable.FindAllBook(x => x.Group == (int)eBookGroup.Costume);

        now = GameInfo.Instance.CostumeList.Count;
        max = costumeList.Count;
        text = string.Format("{0:#,##0}/{1:#,##0}", now, max);
        f = (float)now / (float)max;
        kCostumeBookGaugeUnit.InitGaugeUnit(f);
        kCostumeBookGaugeUnit.SetText(text);

        now = GameInfo.Instance.WeaponBookList.Count;
        max = _maxweaponcount;
        text = string.Format("{0:#,##0}/{1:#,##0}", now, max);
        f = (float)now / (float)max;
        kWeaponBookGaugeUnit.InitGaugeUnit(f);
        kWeaponBookGaugeUnit.SetText(text);

        now = GameInfo.Instance.CardBookList.Count;
        max = _maxcardcount;
        text = string.Format("{0:#,##0}/{1:#,##0}", now, max);
        f = (float)now / (float)max;
        kCardBookGaugeUnit.InitGaugeUnit(f);
        kCardBookGaugeUnit.SetText(text);

        now = GameInfo.Instance.MonsterBookList.Count;
        max = _maxmonstercount;
        text = string.Format("{0:#,##0}/{1:#,##0}", now, max);
        f = (float)now / (float)max;
        kMonsterBookGaugeUnit.InitGaugeUnit(f);
        kMonsterBookGaugeUnit.SetText(text);

        kCharBookNoticeSpr.gameObject.SetActive(false);
        kWeaponBookNoticeSpr.gameObject.SetActive(false);
        kCardBookNoticeSpr.gameObject.SetActive(false);
        kMonsterBookNoticeSpr.gameObject.SetActive(false);
        kCostumeBookNoticeSpr.SetActive(false);
        if ((NotificationManager.Instance.NewCharBookCount + NotificationManager.Instance.NewFavorCharBookCount) != 0 )
            kCharBookNoticeSpr.gameObject.SetActive(true);
        if ((NotificationManager.Instance.NewCardBookCount + NotificationManager.Instance.NewFavorCardBookCount ) != 0 )
            kCardBookNoticeSpr.gameObject.SetActive(true);
        if (NotificationManager.Instance.NewWeaponBookCount != 0 )
            kWeaponBookNoticeSpr.gameObject.SetActive(true);
        if ( NotificationManager.Instance.NewMonsterBookCount != 0 )
            kMonsterBookNoticeSpr.gameObject.SetActive(true);
        if (NotificationManager.Instance.NewCostumeBookCount != (int)eCOUNT.NONE)
            kCostumeBookNoticeSpr.gameObject.SetActive(true);
    }

    public void OnClick_BackBtn()
    {
        OnClickClose();
    }

    public override void OnClickClose()
    {
        LobbyUIManager.Instance.Renewal("MenuPopup");

        UICharInfoPanel uICharInfoPanel = LobbyUIManager.Instance.GetActiveUI<UICharInfoPanel>("CharInfoPanel");
        if (uICharInfoPanel != null)
        {
            uICharInfoPanel.InitComponent();
            uICharInfoPanel.Renewal(true);

            UICharInfoTabStatusPanel charinfoTabPaenl = uICharInfoPanel.GetComponentInChildren<UICharInfoTabStatusPanel>();
            if (null != charinfoTabPaenl && charinfoTabPaenl.gameObject.activeSelf)
            {
                Log.Show("charinfoTabPaenl");
                charinfoTabPaenl.InitComponent();
                charinfoTabPaenl.Renewal(true);
            }

            UICharInfoTabWeaponPanel weaponinfoTabPaenl = uICharInfoPanel.GetComponentInChildren<UICharInfoTabWeaponPanel>();
            if (null != weaponinfoTabPaenl && weaponinfoTabPaenl.gameObject.activeSelf)
            {
                Log.Show("weaponinfoTabPaenl");
                weaponinfoTabPaenl.InitComponent();
                weaponinfoTabPaenl.Renewal(true);
            }

            UICharInfoTabSkillPanel charSkillinfoTabPaenl = uICharInfoPanel.GetComponentInChildren<UICharInfoTabSkillPanel>();
            if (null != charSkillinfoTabPaenl && charSkillinfoTabPaenl.gameObject.activeSelf)
            {
                Log.Show("charSkillinfoTabPaenl");
                charSkillinfoTabPaenl.InitComponent();
                charSkillinfoTabPaenl.Renewal(true);
            }

            UICharInfoTabSupporterPanel cardinfoTabPaenl = uICharInfoPanel.GetComponentInChildren<UICharInfoTabSupporterPanel>();
            if (null != cardinfoTabPaenl && cardinfoTabPaenl.gameObject.activeSelf)
            {
                Log.Show("cardinfoTabPaenl");
                cardinfoTabPaenl.InitComponent();
                cardinfoTabPaenl.Renewal(true);
            }

            UICharInfoTabCostumePanel costumeinfoTabPaenl = uICharInfoPanel.GetComponentInChildren<UICharInfoTabCostumePanel>();
            if (null != costumeinfoTabPaenl && costumeinfoTabPaenl.gameObject.activeSelf)
            {
                Log.Show("costumeinfoTabPaenl");
                costumeinfoTabPaenl.InitComponent();
                costumeinfoTabPaenl.Renewal(true);
            }
        }
        base.OnClickClose();
    }

    public void OnClick_CharBookBtn()
    {
        UIBookCharListPopup bookCharListPopup = LobbyUIManager.Instance.GetUI<UIBookCharListPopup>("BookCharListPopup");
        if (bookCharListPopup != null)
            bookCharListPopup.SetCharDefaultFilter();

        LobbyUIManager.Instance.ShowUI("BookCharListPopup", true);
    }

    public void OnClick_CostumeBookBtn()
    {
        LobbyUIManager.Instance.ShowUI("BookCostumeListPopup", true);
    }

    public void OnClick_WeaponBookBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.BookItemListType, (int)eBookGroup.Weapon);
        LobbyUIManager.Instance.ShowUI("BookItemListPopup", true);
    }

    public void OnClick_CardBookBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.BookItemListType, (int)eBookGroup.Supporter);
        LobbyUIManager.Instance.ShowUI("BookItemListPopup", true);
    }

    public void OnClick_MonsterBookBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.BookItemListType, (int)eBookGroup.Monster);
        LobbyUIManager.Instance.ShowUI("BookItemListPopup", true);
    }

    public override bool IsBackButton()
    {
        OnClick_BackBtn();
        return false;
    }
}
