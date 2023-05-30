using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UserRankUpPopup
{
    public static UIUserRankUpPopup GetRankUpPopup()
    {
        UIUserRankUpPopup rankPopup = null;

        rankPopup = LobbyUIManager.Instance.GetUI<UIUserRankUpPopup>("UserRankUpPopup");

        return rankPopup;
    }

    public static void ShowRankUpPopup(ePANELTYPE resultType = ePANELTYPE.NULL)
    {
        UIUserRankUpPopup rankPopup = GetRankUpPopup();
        if(rankPopup != null)
        {
            rankPopup.ResultType = resultType;
            LobbyUIManager.Instance.ShowUI("UserRankUpPopup", true);
        }
    }
}

public class UIUserRankUpPopup : FComponent
{

    public UILabel kUserLankupLabel;
    public UILabel kDesceLabel;

    public UIItemListSlot kItemListSlot_Now;
    public UIGoodsUnit kGoodsUnit_Now;
    public UILabel kheaderLabel_Now;
    public UILabel kitemnameLabel_Now;

    public UIItemListSlot kItemListSlot_Next;
    public UIGoodsUnit kGoodsUnit_Next;
    public UILabel kheaderLabel_Next;
    public UILabel kitemnameLabel_Next;

    public UISprite kStampSpr_Now;
    public UISprite karrowSpr;
    public UIButton knextBtn;

    private ePANELTYPE _resultType;
    public ePANELTYPE ResultType
    {
        get { return _resultType; }
        set { _resultType = value; }
    }

    private bool _playAnim = false;


    public override void OnEnable()
    {
		Type = TYPE.Popup;

		InitComponent();
        base.OnEnable();
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();

        if(ResultType == ePANELTYPE.FACILITY)
        {
            UIFacilityResultPopup resultPopup = LobbyUIManager.Instance.GetUI<UIFacilityResultPopup>("FacilityResultPopup");
            if (resultPopup != null && resultPopup.gameObject.activeSelf)
                resultPopup.bContinue = true;
        }
        else
        {
            //  비활성화시 전투 결과 팝업의 경우 멈추었던 연출을 다시 진행시킵니다.
            UIBattleResultPopup resultPopup = LobbyUIManager.Instance.GetUI<UIBattleResultPopup>("BattleResultPopup");
            if (resultPopup != null && resultPopup.gameObject.activeSelf == true)
                resultPopup.bContinue = true;
        }
    }

    public override void InitComponent()
    {
        _playAnim = false;

        kItemListSlot_Now.gameObject.SetActive(false);
        kItemListSlot_Next.gameObject.SetActive(false);
        kGoodsUnit_Now.gameObject.SetActive(false);
        kGoodsUnit_Next.gameObject.SetActive(false);
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        int userLevel = (int)UIValue.Instance.GetValue(UIValue.EParamType.UserLevel);
        GameTable.RankUPReward.Param nowReward, nextReward;

        //  현재 랭크의 보상
        nowReward = GameInfo.Instance.GameTable.FindRankUPReward(userLevel);
        SetReward(nowReward, false);
        kDesceLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(nowReward.MessageString),
            string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), userLevel),
            string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), GameSupport.GetMaxAPByRank(userLevel)));

        //  다음 랭크의 보상
        nextReward = GameInfo.Instance.GameTable.FindRankUPReward(userLevel + 1);
        SetReward(nextReward, true);

        RewindAnimation(0);
        PlayAnimtion(0, PlayMode.StopAll);

        _playAnim = true;
    }

    private void Update()
    {
        if (!_playAnim)
        {
            return;
        }

        if (UIAni.IsPlaying(_aninamelist[0]) == false)
        {
            _playAnim = false;
        }
        
        if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Select))
        {
            _playAnim = false;

            LobbyUIManager.Instance.EnableBlockUI(false);

            this.SetEndFrameAnimation((int)eCOUNT.NONE);
            if (!knextBtn.gameObject.activeSelf)
            {
                knextBtn.SetActive(true);
            }
        }

        if (_playAnim == false)
        {
            _NoticeCheck();
        }
    }


    public override void OnUIOpen()
    {
        
    }

    public void OnClick_NextBtn()
    {
        OnClickClose();
    }

    public override void OnClickClose()
    {
        LobbyUIManager.Instance.HideUI("UserRankUpPopup", false);
        //base.OnClickClose();
    }
    
    private void _NoticeCheck()
    {
        int userLevel = (int)UIValue.Instance.GetValue(UIValue.EParamType.UserLevel);

        var facilitylist = GameInfo.Instance.FacilityList.FindAll(x => x.TableData.ID == x.TableData.ParentsID);
        for( int i = 0; i < facilitylist.Count; i++ )
        {
            if (facilitylist[i].Level == 0)
            {
                if (userLevel == facilitylist[i].TableData.FacilityOpenUserRank)
                {
                    string sprname = "";
                    if (facilitylist[i].TableData.EffectType == "FAC_CHAR_EXP")
                    {
                        sprname = "ico_Combat";
                    }
                    else if (facilitylist[i].TableData.EffectType == "FAC_CHAR_SP")
                    {
                        sprname = "ico_Skill";
                    }
                    else if (facilitylist[i].TableData.EffectType == "FAC_WEAPON_EXP")
                    {
                        sprname = "ico_WeaponForce";
                    }
                    else if (facilitylist[i].TableData.EffectType == "FAC_ITEM_COMBINE")
                    {
                        sprname = "ico_item";
                    }
                    else if (facilitylist[i].TableData.EffectType == "FAC_CARD_TRADE")
                    {
                        sprname = "ico_exchange";
                    }
                    else if (facilitylist[i].TableData.EffectType == "FAC_OPERATION_ROOM")
                    {
                        sprname = "ico_Operation";
                    }

                    string text = string.Format(FLocalizeString.Instance.GetText(4101), FLocalizeString.Instance.GetText(facilitylist[i].TableData.Name));
                    NoticePopup.ShowText(UINoticePopup.eTYPE.SPR, text, sprname, UINoticePopup.eRUNTYPE.NONE, 0, 0);
                    return;
                }
            }
        }
        if (userLevel == GameInfo.Instance.GameConfig.PrivateRoomOpenRank)
        {
            string sprname = "ico_CollectionRoom";
            NoticePopup.ShowText(UINoticePopup.eTYPE.SPR, FLocalizeString.Instance.GetText(4102), sprname, UINoticePopup.eRUNTYPE.NONE, 0, 0);
        }
        if (userLevel == GameInfo.Instance.GameConfig.ArenaOpenRank)
        {
            string sprname = "ico_Arena_red";
            NoticePopup.ShowText(UINoticePopup.eTYPE.SPR, FLocalizeString.Instance.GetText(4110), sprname, UINoticePopup.eRUNTYPE.NONE, 0, 0);
        }

        //랭크업 시, 서포터 파견 슬롯 활성 알림
        var dispatchslot = GameInfo.Instance.GameTable.CardDispatchSlots.Find(x => userLevel == x.NeedRank);
        if (dispatchslot != null)
        {
            string sprname = "ico_SupporterDisPatch";
            string str = string.Format(FLocalizeString.Instance.GetText(4112), dispatchslot.Index);
            NoticePopup.ShowText(UINoticePopup.eTYPE.SPR, str, sprname, UINoticePopup.eRUNTYPE.NONE, 0, 0);
        }
    }

    /// <summary>
    /// 랭크에 맞게 보상 표시
    /// </summary>
    private void SetReward(GameTable.RankUPReward.Param rankReward, bool isNext)
    {
        if (rankReward == null)
            return;

        //  랜덤 그룹의 보상
        var reward = GameInfo.Instance.GameTable.FindRandom(rankReward.RewardGroupID);
        if (reward == null)
            return;

        //  해당 데이터를 표시해줄 오브젝트 셋팅
        UIItemListSlot kItemListSlot = isNext == false ? kItemListSlot_Now : kItemListSlot_Next;
        UIGoodsUnit kGoodsUnit = isNext == false ? kGoodsUnit_Now : kGoodsUnit_Next;
        UILabel kitemnameLabel = isNext == false ? kitemnameLabel_Now : kitemnameLabel_Next;

        //  보상데이터로 변환
        RewardData product = new RewardData(reward.ProductType, reward.ProductIndex, reward.ProductValue);

        switch ((eREWARDTYPE)product.Type)
        {
            case eREWARDTYPE.GOODS:
                {
                    kGoodsUnit.InitGoodsUnit((eGOODSTYPE)product.Index, product.Value);
                    kGoodsUnit.kIconSpr.spriteName = GameSupport.GetGoodsIconName((eGOODSTYPE)product.Index);
                    string name = FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTYPE_TEXT_START + product.Index);
                    string value = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), product.Value);
                    if ((eGOODSTYPE)product.Index == eGOODSTYPE.CASH)
                        kitemnameLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.SPACE_TYPE_TEXT), name, string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), value));
                    else
                        kitemnameLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.SPACE_TYPE_TEXT), value, name);

                    kGoodsUnit.kTextLabel.gameObject.SetActive(false);
                    kGoodsUnit.gameObject.SetActive(true);
                }
                break;
            case eREWARDTYPE.ITEM:
                {
                    GameTable.Item.Param item = GameInfo.Instance.GameTable.FindItem(product.Index);
                    string name = FLocalizeString.Instance.GetText(item.Name);
                    string value = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), product.Value);
                    kitemnameLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.SPACE_TYPE_TEXT), name, string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), value));

                    kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.RewardTable, 0, item);
                    kItemListSlot.SetCountLabel(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), product.Value));
                    kItemListSlot.gameObject.SetActive(true);
                }
                break;
        }
    }
}
