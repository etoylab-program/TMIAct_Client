using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharInfoTabSupporterPanel : FComponent
{
    public List<UIItemListSlot> kCardList;
    public List<UISprite> kEmptyList;
    public List<UILabel> kLockList;
    public GameObject kMainSkill;
    public UILabel kMainSkillNameLabel;
	public UILabel kMainSkillLevelLabel;
    public UILabel kMainSkillTimeLabel;
	public UILabel kMainSkillDesceLabel;
    public List<UISkillListSlot> kSkillList;

    private CharData        mCharData       = null;
    private List<CardData>  mListCardData   = new List<CardData>();


	public override void OnEnable()
	{
		Type = TYPE.Tab;

		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        if( RenderTargetChar.Instance.RenderPlayer == null ) {
            return;
        }

        RenderTargetChar.Instance.ShowAttachedObject(false);
        RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject(RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.InGameOnly, false);
        RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject(RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.LobbyOnly, true);
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
        mCharData = GameInfo.Instance.GetCharData(uid);

        mListCardData.Clear();
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            CardData cardData = GameInfo.Instance.GetCardData(mCharData.EquipCard[i]);
            mListCardData.Add(cardData);
        }

        for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
        {
            var passviedata = mCharData.PassvieList.Find(x => x.SkillID == mCharData.EquipSkill[i]);
            kSkillList[i].ParentGO = this.gameObject;
            kSkillList[i].UpdateSlot(UISkillListSlot.ePOS.VIEW, i, passviedata, mCharData, mListCardData);
        }

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            var carddata = GameInfo.Instance.GetCardData(mCharData.EquipCard[i]);
            if(carddata == null)
            {
                kCardList[i].gameObject.SetActive(false);
                kEmptyList[i].gameObject.SetActive(true);
                kLockList[i].transform.parent.gameObject.SetActive(false);

                if (mCharData.Level < GameInfo.Instance.GameConfig.CharCardSlotLimitLevel[i]) // 레벨제한
                {
                    kLockList[i].transform.parent.gameObject.SetActive(true);
                    kLockList[i].textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), GameInfo.Instance.GameConfig.CharCardSlotLimitLevel[i]);
                }
            }
            else
            {
                kCardList[i].gameObject.SetActive(true);
                kEmptyList[i].gameObject.SetActive(false);
                kLockList[i].transform.parent.gameObject.SetActive(false);

                kCardList[i].ParentGO = this.gameObject;
                kCardList[i].UpdateSlot(UIItemListSlot.ePosType.EquipCard, i, carddata);
                kCardList[i].SetActiveItemStateUnit(false);

                if (GameSupport.CheckCardData(carddata))
                    kCardList[i].SetActiveNotice(true);
                else
                    kCardList[i].SetActiveNotice(false);
            }
        }

        var maincarddata = GameInfo.Instance.GetCardData(mCharData.EquipCard[(int)eCARDSLOT.SLOT_MAIN]);
        if(maincarddata == null)
        {
            kMainSkill.SetActive(false);
        }
        else
        {
            if (maincarddata.TableData.MainSkillEffectName > 0)
            {
                kMainSkill.SetActive(true);
                kMainSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(maincarddata.TableData.MainSkillEffectName);
                kMainSkillDesceLabel.textlocalize = GameSupport.GetCardMainSkillDesc(maincarddata.TableData, maincarddata.Wake);
                if (maincarddata.Wake == 0)
                    kMainSkillLevelLabel.textlocalize = "";
                else
                    kMainSkillLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_NOW_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PLUS_TEXT), maincarddata.Wake));
                kMainSkillLevelLabel.transform.localPosition = new Vector3(kMainSkillNameLabel.transform.localPosition.x + kMainSkillNameLabel.printedSize.x + 10, kMainSkillNameLabel.transform.localPosition.y, 0);

                if (maincarddata.TableData.CoolTime == 0)
                {
                    kMainSkillTimeLabel.gameObject.SetActive(false);
                }
                else
                {
                    kMainSkillTimeLabel.gameObject.SetActive(true);
                    kMainSkillTimeLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(263), maincarddata.TableData.CoolTime);
                }
            }
            else
                kMainSkill.SetActive(false);
        }
    }

    public void OnClick_CardChangeBtn(int index)
    {
        if (GameInfo.Instance.CardList.Count == 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3020));
            return;
        }

        //서포터 목록에 표시할게 있는지 체크
        bool cardFlag = false;
        for (int i = 0; i < GameInfo.Instance.CardList.Count; i++)
        {
            if (GameSupport.IsEquipAndUsingCardData(GameInfo.Instance.CardList[i].CardUID) == false)
            {
                cardFlag = true;
                break;
            }
        }

        if (!cardFlag)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3038));
            return;
        }

        if (mCharData.Level < GameInfo.Instance.GameConfig.CharCardSlotLimitLevel[index])
        {
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3012), GameInfo.Instance.GameConfig.CharCardSlotLimitLevel[index]));
            return;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.CharEquipCardSlot, index);
        LobbyUIManager.Instance.ShowUI("CharCardSeletePopup", true);
    }

    public void OnClick_SkillBtn()
    {
        UICharInfoPanel panel = LobbyUIManager.Instance.GetActiveUI<UICharInfoPanel>("CharInfoPanel");
        if (panel == null)
            return;

        panel.kMainTab.SetTab((int)UICharInfoPanel.eCHARINFOTAB.SKILL, SelectEvent.Code);
    }
}
