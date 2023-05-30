using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICardSkillLevelUpPopup : FComponent
{
	public UIButton kBackBtn;
	public UISprite kGradeSpr;
	public UILabel kNameLabel;
	public UITexture kCardTex;
	public UILabel kSkillNameLabel;
	public UILabel kSkillLevelLabel;
	public UILabel kSkillDescLabel;

    public UIButton kLevelUpBtn;
	public UIGoodsUnit kGoldGoodsUnit;
    public UIItemListSlot kItemListSlot;
    public UIItemListSlot kMatItemListSlot;
    public UISprite kMatEmptySpr;
    public UIStatusUnit kSupporterStatusUnit_HP;
    public UIStatusUnit kSupporterStatusUnit_DEF;

    [SerializeField] private FList _ItemListInstance;

    private List<MatSkillData> _matlist = new List<MatSkillData>();
    private MatSkillData _selmatdata = null;
    private CardData _carddata;
    private bool _bsendskilllvup = false;



    public MatSkillData SelMatData { get { return _selmatdata; } }


    public override void Awake()
	{
		base.Awake();

		if(this._ItemListInstance == null) return;
		
		this._ItemListInstance.EventUpdate = this._UpdateItemListSlot;
		this._ItemListInstance.EventGetItemCount = this._GetItemElementCount;
        this._ItemListInstance.InitBottomFixing();

    }
 
	public override void OnEnable()
	{
        _bsendskilllvup = false;
        InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CardUID);
        _carddata = GameInfo.Instance.GetCardData(uid);
        
         _selmatdata = null;

        _matlist.Clear();

        for (int i = 0; i < GameInfo.Instance.CardList.Count; i++)
        {
            if (GameInfo.Instance.CardList[i].Lock)
                continue;
            if (_carddata.CardUID == GameInfo.Instance.CardList[i].CardUID)
                continue;
            CharData chardata = GameInfo.Instance.GetEquiCardCharData(GameInfo.Instance.CardList[i].CardUID);
            if (chardata != null)
                continue;
            if (GameSupport.IsEquipAndUsingCardData(GameInfo.Instance.CardList[i].CardUID))
                continue;
            if (_carddata.TableID == GameInfo.Instance.CardList[i].TableID)
                _matlist.Add(new MatSkillData(GameInfo.Instance.CardList[i]));
        }

        var list = GameInfo.Instance.ItemList.FindAll(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_CARD_SLVUP && x.TableData.Grade == _carddata.TableData.Grade );
        for (int i = 0; i < list.Count; i++)
            _matlist.Add(new MatSkillData(list[i]));

        _ItemListInstance.UpdateList();
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_carddata.TableData.Name);
        kGradeSpr.spriteName = "itemgrade_L_" + _carddata.TableData.Grade.ToString();
        //kGradeSpr.MakePixelPerfect();

        kCardTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("card", string.Format("Card/{0}_{1}.png", _carddata.TableData.Icon, GameSupport.GetCardImageNum(_carddata)));

        int skilllevel = _carddata.SkillLv;
        if (_selmatdata != null)
        {
            if (_selmatdata.Type == MatSkillData.eTYPE.ITEM)
                skilllevel += _selmatdata.ItemData.TableData.Value;
            else if (_selmatdata.Type == MatSkillData.eTYPE.CARD)
                skilllevel += _selmatdata.CardData.SkillLv;
            if (GameSupport.GetMaxSkillLevelCard() < skilllevel)
                skilllevel = GameSupport.GetMaxSkillLevelCard();
        }

        float addPercent = (GameInfo.Instance.GameConfig.CardSkillLvStatRate[skilllevel] - GameInfo.Instance.GameConfig.CardSkillLvStatRate[_carddata.SkillLv]) * 100.0f;

        kSupporterStatusUnit_HP.InitStatusUnit((int)eTEXTID.STAT_HP, GameSupport.GetCardHP(_carddata.Level, _carddata.Wake, _carddata.SkillLv, _carddata.EnchantLv, _carddata.TableData), GameSupport.GetCardHP(_carddata.Level, _carddata.Wake, skilllevel, _carddata.EnchantLv, _carddata.TableData), addPercent);
        kSupporterStatusUnit_DEF.InitStatusUnit((int)eTEXTID.STAT_DEF, GameSupport.GetCardDEF(_carddata.Level, _carddata.Wake, _carddata.SkillLv, _carddata.EnchantLv, _carddata.TableData), GameSupport.GetCardDEF(_carddata.Level, _carddata.Wake, skilllevel, _carddata.EnchantLv, _carddata.TableData), addPercent);

        kSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(_carddata.TableData.SkillEffectName);
        kSkillDescLabel.textlocalize = GameSupport.GetCardSubSkillDesc(_carddata.TableData, skilllevel);
        kSkillLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.SKILL_LEVEL_TXT_NOW_LV, skilllevel, GameSupport.GetMaxSkillLevelCard());
        kSkillLevelLabel.transform.localPosition = new Vector3(kSkillNameLabel.transform.localPosition.x + kSkillNameLabel.printedSize.x + 10, kSkillLevelLabel.transform.localPosition.y, 0);

        kItemListSlot.ParentGO = this.gameObject;
        kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, 0, _carddata);

        int gold = 0;
        if (_selmatdata != null)
        {
            kMatEmptySpr.gameObject.SetActive(false);
            kMatItemListSlot.gameObject.SetActive(true);
            kMatItemListSlot.ParentGO = this.gameObject;
            if (_selmatdata.Type == MatSkillData.eTYPE.ITEM)
                kMatItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, 0, _selmatdata.ItemData);
            else if (_selmatdata.Type == MatSkillData.eTYPE.CARD)
                kMatItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, 0, _selmatdata.CardData);

            gold = GameSupport.GetCardSkillLevelUpCost(_carddata, 1);

            kLevelUpBtn.isEnabled = true;
        }
        else
        {
            kMatEmptySpr.gameObject.SetActive(true);
            kMatItemListSlot.gameObject.SetActive(false);
            
            kLevelUpBtn.isEnabled = false;
        }

        _ItemListInstance.RefreshNotMove();

        kGoldGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, gold, true);
    }
 	
	private void _UpdateItemListSlot(int index, GameObject slotObject)
	{
		do
		{
            UIItemListSlot card = slotObject.GetComponent<UIItemListSlot>();
            if (null == card) break;
            card.ParentGO = this.gameObject;

            MatSkillData data = null;
            if (0 <= index && _matlist.Count > index)
                data = _matlist[index];

            card.ParentGO = this.gameObject;
            if (data.Type == MatSkillData.eTYPE.ITEM)
                card.UpdateSlot(UIItemListSlot.ePosType.Card_SkillLevelUpMatItemList, index, data.ItemData);
            else if (data.Type == MatSkillData.eTYPE.CARD)
                card.UpdateSlot(UIItemListSlot.ePosType.Card_SkillLevelUpMatList, index, data.CardData);
        } while(false);
	}
	
	private int _GetItemElementCount()
	{
        return _matlist.Count;
    }

    public void OnClick_ItemSlot()
    {
        _selmatdata = null;
        Renewal(true);
    }

    public void OnClick_BackBtn()
	{
        OnClickClose();
	}
	
	public void OnClick_CancleBtn()
	{
        OnClickClose();
    }
	
	public void OnClick_LevelUpBtn()
	{
        if (_selmatdata == null)
            return;
        if( _bsendskilllvup )
            return;

        int gold = GameSupport.GetCardSkillLevelUpCost(_carddata, 1);

        if (!GameSupport.IsCheckGoods(eGOODSTYPE.GOLD, gold))
        {
            return;
        }

        if (GameSupport.IsMaxSkillLevelCard(_carddata))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3059));
            return;
        }

        _bsendskilllvup = true;
        if (_selmatdata.Type == MatSkillData.eTYPE.ITEM)
            GameInfo.Instance.Send_ReqSkillLvUpCard(_carddata.CardUID, (int)_selmatdata.Type, _selmatdata.ItemData.ItemUID, OnNetSkillLvUpCard);
        else if (_selmatdata.Type == MatSkillData.eTYPE.CARD)
            GameInfo.Instance.Send_ReqSkillLvUpCard(_carddata.CardUID, (int)_selmatdata.Type, _selmatdata.CardData.CardUID, OnNetSkillLvUpCard);
    }

    public void SeletMatItem(ItemData itemdata)
    {
        if (GameSupport.IsMaxSkillLevelCard(_carddata))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3059));
            return;
        }
        _selmatdata = new MatSkillData(itemdata);
        Renewal(true);
    }

    public void SeletMatCard(CardData carddata)
    {
        if (GameSupport.IsMaxSkillLevelCard(_carddata))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3059));
            return;
        }
        _selmatdata = new MatSkillData(carddata);
        Renewal(true);
    }

    public void OnNetSkillLvUpCard(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        StartCoroutine(SkillLvUpCardResultCoroutine());
    }

    IEnumerator SkillLvUpCardResultCoroutine()
    {
        LobbyUIManager.Instance.ShowUI("ResultCardSkillLevelUpPopup", true);

        yield return new WaitForSeconds(2.0f);

        InitComponent();
        Renewal(true);
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("CardInfoPopup");

        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
        {   
            itempanel.InitComponent();
            itempanel.Renewal(true);
        }

        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.CHARINFO)
        {
            LobbyUIManager.Instance.Renewal("CharInfoPanel");
            LobbyUIManager.Instance.InitComponent("CharCardSeletePopup");
            LobbyUIManager.Instance.Renewal("CharCardSeletePopup");
        }
        _bsendskilllvup = false;
    }


}
