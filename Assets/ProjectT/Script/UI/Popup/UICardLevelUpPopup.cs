using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICardLevelUpPopup : FComponent
{
    public GameObject kMatCard;
    public GameObject kMatItem;
    public UITexture kCardTex;
    public GameObject kLevelUpEff;
    public GameObject kLevelUpEff_High;
    public GameObject kSuccessType;
    public List<GameObject> kSuccessList;
    public UISprite kGradeSpr;
    public UILabel kNameLabel;
    public UILabel kEnchantLabel;
    public UILabel kLevelLabel;
    public UILabel kExpLabel;
    public UIGaugeUnit kExpGaugeUnit;
    public UISprite kWakeSpr;
    public UIStatusUnit kCharStatusUnit_HP;
    public UIStatusUnit kCharStatusUnit_DEF;
    public UILabel kMaterialnumLabel;
    public UILabel kMaterialLabel;
    public UISprite kOrderSpr;
    public UIButton kCancleBtn;
    public UIButton kLevelUpBtn;
    public UIGoodsUnit kGoldGoodsUnit;
    private CardData _carddata;
    private bool _bmatItem = true;
    private bool _sortupdown = true;
    [SerializeField] private FList _CardListInstance;
    [SerializeField] private FList _ItemListInstance;

    private List<CardData> _cardlist = new List<CardData>();
    private List<CardData> _matcardlist = new List<CardData>();
    private List<MatItemData> _itemlist = new List<MatItemData>();
    private List<MatItemData> _matitemlist = new List<MatItemData>();

    public int kGageAllGainExp = -1;
    private int _nowlevel = -1;
    private int _curlevel = -1;
    private int _curexp = -1;
    private int _gainexp = -1;
    private Coroutine m_crGageExp = null;

    private int _sendlevel;
    private int _sendexp;
    private bool _bsendlevelup = false;
    public bool IsSendLevelUp { get { return _bsendlevelup; } }
    public List<MatItemData> MatItemList { get { return _matitemlist; } }
    public List<CardData> MatCardList { get { return _matcardlist; } }

    public UILabel kNoneLabel;

    public override void Awake()
    {
        
        base.Awake();

        if (this._CardListInstance == null) return;

        this._CardListInstance.EventUpdate = this._UpdateCardListSlot;
        this._CardListInstance.EventGetItemCount = this._GetCardElementCount;
        this._CardListInstance.InitBottomFixing();

        if (this._ItemListInstance == null) return;

        this._ItemListInstance.EventUpdate = this._UpdateItemListSlot;
        this._ItemListInstance.EventGetItemCount = this._GetItemElementCount;
        this._ItemListInstance.InitBottomFixing();
    }

    public override void OnEnable()
    {
        kLevelUpEff.SetActive(false);
        kLevelUpEff_High.SetActive(false);
        kSuccessType.SetActive(false);
        for (int i = 0; i < kSuccessList.Count; i++)
            kSuccessList[i].SetActive(false);

        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        if(kNoneLabel != null)
            kNoneLabel.SetActive(false);

        long carduid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CardUID);
        _carddata = GameInfo.Instance.GetCardData(carduid);
        _bsendlevelup = false;

        var list = GameInfo.Instance.ItemList.FindAll(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_CARD_LEVELUP);
        if (list.Count != 0)
            _bmatItem = true;
        else
            _bmatItem = false;

        if( GameSupport.IsTutorial() )
            _bmatItem = true;

        SetMatType(_bmatItem);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        kGradeSpr.spriteName = "itemgrade_L_" + _carddata.TableData.Grade.ToString();
        //kGradeSpr.MakePixelPerfect();

        kWakeSpr.spriteName = "itemwake_0" + _carddata.Wake.ToString();
        kWakeSpr.MakePixelPerfect();

        kCardTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("card", string.Format("Card/{0}_{1}.png", _carddata.TableData.Icon, GameSupport.GetCardImageNum(_carddata)));
        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_carddata.TableData.Name);

        if (_carddata.EnchantLv > 0)
        {
            kEnchantLabel.SetActive(true);
            kEnchantLabel.textlocalize = string.Format("+{0}", _carddata.EnchantLv);
            kEnchantLabel.transform.localPosition = new Vector3(kNameLabel.transform.localPosition.x + (kNameLabel.printedSize.x * 0.5f) + 10, kNameLabel.transform.localPosition.y, 0);
        }
        else
        {
            kEnchantLabel.SetActive(false);
        }

        int gold = 0;

        if (_bmatItem)
        {
            int totalmatcount = 0;
            for (int i = 0; i < _matitemlist.Count; i++)
                totalmatcount += _matitemlist[i].Count;

            kMaterialnumLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(276), totalmatcount, GameInfo.Instance.GameConfig.MatCount);
            this._ItemListInstance.RefreshNotMove();

            gold = GameSupport.GetCardLevelUpItemCost(_carddata, _matitemlist);

            kMaterialLabel.textlocalize = FLocalizeString.Instance.GetText(1317);

            if(_itemlist.Count <= 0)
            {
                if(kNoneLabel != null)
                {
                    kNoneLabel.SetActive(true);
                    //kNoneLabel.textlocalize = FLocalizeString.Instance.GetText(1595);
                }
            }
        }
        else
        {
            kMaterialnumLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(276), _matcardlist.Count, GameInfo.Instance.GameConfig.MatCount);
            this._CardListInstance.RefreshNotMove();

            gold = GameSupport.GetCardLevelUpCost(_carddata, _matcardlist);

            kMaterialLabel.textlocalize = FLocalizeString.Instance.GetText(1319);

            if(_cardlist.Count <= 0)
            {
                if(kNoneLabel != null)
                {
                    kNoneLabel.SetActive(true);
                    //kNoneLabel.textlocalize = FLocalizeString.Instance.GetText(1595);
                }
            }
        }

        bool bGoldFlag = false;
        GuerrillaCampData campdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Upgrade_PriceRateDown, (int)eContentsPosKind.CARD);
        if (campdata != null)
        {
            bGoldFlag = true;
            gold -= (int)((float)gold * (float)((float)campdata.EffectValue / (float)eCOUNT.MAX_BO_FUNC_VALUE));
        }

        kGoldGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, gold, true, bGoldFlag);

        if (_sortupdown)
            kOrderSpr.spriteName = "ico_Filter1";
        else
            kOrderSpr.spriteName = "ico_Filter2";

    }

    private void _UpdateCardListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIItemListSlot card = slotObject.GetComponent<UIItemListSlot>();
            if (null == card) break;
            card.ParentGO = this.gameObject;

            CardData data = null;
            if (0 <= index && _cardlist.Count > index)
                data = _cardlist[index];
            card.UpdateSlot(UIItemListSlot.ePosType.Card_LevelUpMatList, index, data);
        } while (false);
    }

    private int _GetCardElementCount()
    {
        return _cardlist.Count; //TempValue
    }

    private void _UpdateItemListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIItemListSlot card = slotObject.GetComponent<UIItemListSlot>();
            if (null == card) break;
            card.ParentGO = this.gameObject;

            MatItemData data = null;
            if (0 <= index && _itemlist.Count > index)
                data = _itemlist[index];
            card.UpdateSlot(UIItemListSlot.ePosType.Card_LevelUpMatItemList, index, data.ItemData, data.Count);
        } while (false);
    }

    private int _GetItemElementCount()
    {
        return _itemlist.Count;
    }

    void FixedUpdate()
    {
        if (kGageAllGainExp == -1)
            return;

        int nowexp = _curexp + kGageAllGainExp;
        int level = GameSupport.GetCardExpLevel(_carddata, nowexp);

        kExpLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PLUS_TEXT), kGageAllGainExp);
        if (_curlevel == _carddata.Level)
            kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curlevel, GameSupport.GetMaxLevelCard(_carddata));
        else
            kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curlevel, GameSupport.GetMaxLevelCard(_carddata), true);
        

        float fillAmount = GameSupport.GetCardLevelExpGauge(_carddata, level, nowexp);
        kExpGaugeUnit.InitGaugeUnit(fillAmount);
        kExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (fillAmount * 100.0f)));

        if (_curlevel != level)
        {
            _curlevel = level;
            if (_curlevel >= GameSupport.GetMaxLevelCard(_carddata))
                _curlevel = GameSupport.GetMaxLevelCard(_carddata);

            kCharStatusUnit_HP.InitStatusUnit((int)eTEXTID.STAT_HP, _carddata.GetCardHP(_nowlevel), _carddata.GetCardHP(_curlevel));
            kCharStatusUnit_DEF.InitStatusUnit((int)eTEXTID.STAT_DEF, _carddata.GetCardDEF(_nowlevel), _carddata.GetCardDEF(_curlevel));
        }
    }

    private void SetExpGainTween()
    {
        Utility.StopCoroutine(this, ref m_crGageExp);
        if (_bmatItem)
            _gainexp = GameSupport.GetItemMatExp(_matitemlist);
        else
            _gainexp = GameSupport.GetCardMatExp(_matcardlist);
        int nRemainExp = GameSupport.GetRemainCardExpToMaxLevel(_carddata);
        if (_gainexp >= nRemainExp)
            _gainexp = nRemainExp;

        m_crGageExp = StartCoroutine(Utility.UpdateCoroutineValue((x) => kGageAllGainExp = (int)x, kGageAllGainExp, _gainexp, 0.5f));
    }

    public bool AddMatItem(ItemData matitemdata, bool bmessage = true)
    {
        if (_bsendlevelup)
            return false;

        int totalmatcount = 0;
        for (int i = 0; i < _matitemlist.Count; i++)
            totalmatcount += _matitemlist[i].Count;
        if (totalmatcount >= GameInfo.Instance.GameConfig.MatCount)
            return false;
        if (GameSupport.IsMaxLevelCard(_carddata, _curlevel))
            return false;


        var targetitem = _itemlist.Find(x => x.ItemData.ItemUID == matitemdata.ItemUID);
        if (targetitem == null)
            return false;
        int count = targetitem.Count - 1;
        if (count < 0)
            return false;

        if (IsCheckMaxLevel())
        {
            if (bmessage)
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3013));
            return false;
        }

        var matitem = _matitemlist.Find(x => x.ItemData.ItemUID == matitemdata.ItemUID);
        if (matitem == null)
        {
            _matitemlist.Add(new MatItemData(targetitem.ItemData, 1));
        }
        else
        {
            matitem.Count += 1;
        }
        targetitem.Count -= 1;

        Renewal(true);
        SetExpGainTween();

        return true;
    }

    public bool AddMatCard(CardData carddata, bool bmessage = true)
    {
        if (_bsendlevelup)
            return false;

        var matitem = _matcardlist.Find(x => x.CardUID == carddata.CardUID);
        if (matitem != null)
        {
            DelMatCard(carddata);
        }
        else
        {
            if (GameSupport.IsMaxLevelCard(_carddata, _curlevel))
                return false;

            if (_matcardlist.Count < GameInfo.Instance.GameConfig.MatCount)
            {
                if (IsCheckMaxLevel())
                {
                    if(bmessage)
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3013));
                    return false;
                }
                else
                {
                    _matcardlist.Add(carddata);
                }
            }
        }

        Renewal(true);

        SetExpGainTween();

        return true;
    }
    public void DelMatCard(CardData carddata)
    {
        var matitem = _matcardlist.Find(x => x.CardUID == carddata.CardUID);
        if (matitem == null)
            return;


        _matcardlist.Remove(matitem);
        Renewal(true);

        SetExpGainTween();
    }

    //  선택한 재료가 추가가능한지 레벨 체크
    private bool IsCheckMaxLevel()
    {
        int exp = 0;
        if (_bmatItem)
            exp = GameSupport.GetItemMatExp(_matitemlist);
        else
            exp = GameSupport.GetCardMatExp(_matcardlist);

        int level = GameSupport.GetCardExpLevel(_carddata, _carddata.Exp + exp);
        int maxlevel = GameSupport.GetMaxLevelCard(_carddata);
        if (level >= maxlevel)
        {
            return true;
        }
        return false;
    }

    public void OnClick_ResetBtn()
    {
        if (_bsendlevelup)
            return;

        Utility.StopCoroutine(this, ref m_crGageExp);
        _nowlevel = _carddata.Level;
        _curlevel = _carddata.Level;
        _curexp = _carddata.Exp;
        _gainexp = _carddata.Exp;
        kGageAllGainExp = -1;

        _matitemlist.Clear();
        _matcardlist.Clear();

        Renewal(true);
        SetMatType(_bmatItem);
    }

    public void OnClick_AutoMatBtn()
    {
        if (_bsendlevelup)
            return;

        Utility.StopCoroutine(this, ref m_crGageExp);
        _nowlevel = _carddata.Level;
        _curlevel = _carddata.Level;
        _curexp = _carddata.Exp;
        _gainexp = _carddata.Exp;
        kGageAllGainExp = -1;

        if (_bmatItem)
        {
            for (int i = 0; i < _matitemlist.Count; i++)
            {
                var data = _matitemlist[i];
                var find = _itemlist.Find(x => x.ItemData.ItemUID == data.ItemData.ItemUID);
                if (find != null)
                    find.Count += data.Count;
            }
            _matitemlist.Clear();
            int itemnum = 0;
            for (int i = 0; i < GameInfo.Instance.GameConfig.MatCount; i++)
            {
                var itemdata = CheckAutoItem(ref itemnum);
                if (itemdata != null)
                    AddMatItem(itemdata, false);
            }
        }
        else
        {
            _matcardlist.Clear();
            int count = GameInfo.Instance.GameConfig.MatCount;
            if (count > _cardlist.Count)
                count = _cardlist.Count;

            for (int i = 0; i < count; i++)
                AddMatCard(_cardlist[i], false);
        }
       
    }
   
    private ItemData CheckAutoItem(ref int num)
    {
        if (0 <= num && _itemlist.Count > num)
        {
            int count = _itemlist[num].Count - 1;
            if (count < 0)
            {
                num += 1;
                var data = CheckAutoItem(ref num);
                return data;

            }
            else
            {
                return _itemlist[num].ItemData;
            }
        }

        return null;
    }
   
    public void OnClick_OrderBtn()
    {
        if (_bsendlevelup)
            return;

        _sortupdown = !_sortupdown;
      
        if (_bmatItem)
        {
            MatItemData.SortUp = _sortupdown;
            _itemlist.Sort(MatItemData.CompareFuncGrade);
            this._ItemListInstance.UpdateList();
        }
        else
        {
            CardData.SortUp = _sortupdown;
            _cardlist.Sort(CardData.CompareFuncGradeLevel);
            this._CardListInstance.UpdateList();
        }
        
        Renewal(true);
    }

    public void OnClick_MaterialChangeBtn()
    {
        if (_bsendlevelup)
            return;

        SetMatType(!_bmatItem);
        Renewal(true);
    }

    public void OnClick_CancleBtn()
    {
        if (_bsendlevelup)
            return;
        OnClickClose();      
    }

    public override void OnClickClose()
    {
        if (_bsendlevelup)
            return;
        base.OnClickClose();
    }

    public void OnClick_LevelUpBtn()
    {
        if (_bsendlevelup)
            return;

        if (GameSupport.IsMaxLevelCard(_carddata)) //최대 레벨
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3004));
            return;
        }
        int matcount = 0;
        int gold = 0;
        if (_bmatItem)
        {
            for (int i = 0; i < _matitemlist.Count; i++)
                matcount += _matitemlist[i].Count;
            gold = GameSupport.GetCardLevelUpItemCost(_carddata, _matitemlist);
        }
        else
        {
            matcount = _matcardlist.Count;
            gold = GameSupport.GetCardLevelUpCost(_carddata, _matcardlist);
        }

        GuerrillaCampData campdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Upgrade_PriceRateDown, (int)eContentsPosKind.CARD);
        if (campdata != null)
        {
            gold -= (int)((float)gold * (float)((float)campdata.EffectValue / (float)eCOUNT.MAX_BO_FUNC_VALUE));
        }

        if (matcount == 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3016));
            return;
        }

        if (!GameSupport.IsCheckGoods(eGOODSTYPE.GOLD, gold))
        {
            return;
        }

       

        List<long> matlist = new List<long>();
        for (int i = 0; i < _matcardlist.Count; i++)
            matlist.Add(_matcardlist[i].CardUID);

        _sendlevel = _carddata.Level;
        _sendexp = _carddata.Exp;
        _bsendlevelup = true;
        GameInfo.Instance.Send_ReqLvUpCard(_carddata.CardUID, _bmatItem, matlist, _matitemlist, OnNetCardLevelUp);
        
    }

    

    public void OnNetCardLevelUp(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            _bsendlevelup = false;
            return;
        }       

        StartCoroutine(CardSkillLevelUpResultCoroutine(pktmsg));
    }

    IEnumerator CardSkillLevelUpResultCoroutine(PktMsgType pktmsg)
    {
        int success = 0;
        _nowlevel = _sendlevel;
        _curlevel = _sendlevel;
        _curexp = _sendexp;
        _gainexp = _sendexp;
        kGageAllGainExp = -1;

        kExpLabel.textlocalize = "";
        kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curlevel, GameSupport.GetMaxLevelCard(_carddata), true);
        float fillAmount = GameSupport.GetCardLevelExpGauge(_carddata, _sendlevel, _sendexp);
        kExpGaugeUnit.InitGaugeUnit(fillAmount);
        kExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (fillAmount * 100.0f)));
        kCharStatusUnit_HP.InitStatusUnit((int)eTEXTID.STAT_HP, _carddata.GetCardHP(_nowlevel), _carddata.GetCardHP(_nowlevel));
        kCharStatusUnit_DEF.InitStatusUnit((int)eTEXTID.STAT_DEF, _carddata.GetCardDEF(_nowlevel), _carddata.GetCardDEF(_nowlevel));

        if (pktmsg != null)
        {
            PktInfoCardGrow cardgrow = (PktInfoCardGrow)pktmsg;
            success = (int)cardgrow.growState_;
        }
        if (success == (int)eGrowState.SUCC_OVER_GRATE)
            GameSupport.PlayParticle(kLevelUpEff_High);
        else
            GameSupport.PlayParticle(kLevelUpEff);
        SoundManager.Instance.PlayUISnd(28 + success);

        yield return new WaitForSeconds(1.5f);

        kSuccessType.SetActive(true);
        for (int i = 0; i < kSuccessList.Count; i++)
            kSuccessList[i].SetActive(false);
        kSuccessList[success].SetActive(true);
        

        yield return new WaitForSeconds(0.6f);

        _gainexp = _carddata.Exp - _sendexp;
        Utility.StopCoroutine(this, ref m_crGageExp);
        m_crGageExp = StartCoroutine(Utility.UpdateCoroutineValue((x) => kGageAllGainExp = (int)x, kGageAllGainExp, _gainexp, 1.0f));
        SoundManager.Instance.PlayUISnd(58);
        yield return new WaitForSeconds(1.0f);

        if (_sendlevel != _carddata.Level)
            VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.LevelUp, _carddata.TableID);

        kSuccessType.SetActive(false);
        //InitComponent();
        SetMatType(_bmatItem);
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

        if ( LobbyUIManager.Instance.PanelType == ePANELTYPE.CHARINFO )
        {
            LobbyUIManager.Instance.Renewal("CharInfoPanel");
            LobbyUIManager.Instance.InitComponent("CharCardSeletePopup");
            LobbyUIManager.Instance.Renewal("CharCardSeletePopup");
        }

        


        if (GameSupport.IsMaxLevelCard(_carddata)) //최대 레벨
        {
            base.OnClickClose();
        }

        if (GameSupport.IsCardOpenTerms_Effect(_carddata))
        {
            UIValue.Instance.SetValue(UIValue.EParamType.SelectRewardType, eREWARDTYPE.CARD);
            UIValue.Instance.SetValue(UIValue.EParamType.CardUID, _carddata.CardUID);
            DirectorUIManager.Instance.PlayMaxLevel();
        }
        _bsendlevelup = false;

        if (GameSupport.IsTutorial())
            GameSupport.TutorialNext();
    }

    private void SetMatType(bool bmatitem)
    {
        _bmatItem = bmatitem;
        //_sortupdown = true;

        Utility.StopCoroutine(this, ref m_crGageExp);

        _nowlevel = _carddata.Level;
        _curlevel = _carddata.Level;
        _curexp = _carddata.Exp;
        _gainexp = _carddata.Exp;
        kGageAllGainExp = -1;


        kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curlevel, GameSupport.GetMaxLevelCard(_carddata));

        float fillAmount = GameSupport.GetCardLevelExpGauge(_carddata, _carddata.Level, _carddata.Exp);
        kExpGaugeUnit.InitGaugeUnit(fillAmount);
        kExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (fillAmount * 100.0f)));

        kCharStatusUnit_HP.InitStatusUnit((int)eTEXTID.STAT_HP, _carddata.GetCardHP(), _carddata.GetCardHP(_curlevel));
        kCharStatusUnit_DEF.InitStatusUnit((int)eTEXTID.STAT_DEF, _carddata.GetCardDEF(), _carddata.GetCardDEF(_curlevel));

        kExpLabel.textlocalize = "";


        _matitemlist.Clear();
        _itemlist.Clear();

        _matcardlist.Clear();
        _cardlist.Clear();

        if(kNoneLabel != null)
            kNoneLabel.SetActive(false);

        if (_bmatItem)
        {
            kMatCard.SetActive(false);
            kMatItem.SetActive(true);

            var list = GameInfo.Instance.ItemList.FindAll(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_CARD_LEVELUP);
            for (int i = 0; i < list.Count; i++)
                _itemlist.Add(new MatItemData(list[i], list[i].Count));

            MatItemData.SortUp = _sortupdown;
            _itemlist.Sort(MatItemData.CompareFuncGrade);

            this._ItemListInstance.UpdateList();

            if(_itemlist.Count <= 0)
            {
                if(kNoneLabel != null)
                {
                    kNoneLabel.SetActive(true);
                    //kNoneLabel.textlocalize = FLocalizeString.Instance.GetText(1595);
                }
            }
        }
        else
        {
            kMatCard.SetActive(true);
            kMatItem.SetActive(false);

            for (int i = 0; i < GameInfo.Instance.CardList.Count; i++)
            {
                if (GameInfo.Instance.CardList[i].Lock)
                    continue;
                if (_carddata.CardUID == GameInfo.Instance.CardList[i].CardUID)
                    continue;
                if (_carddata.TableID == GameInfo.Instance.CardList[i].TableID)
                    continue;

                CharData chardata = GameInfo.Instance.GetEquiCardCharData(GameInfo.Instance.CardList[i].CardUID);
                if (chardata != null)
                    continue;

                if (GameSupport.IsEquipAndUsingCardData(GameInfo.Instance.CardList[i].CardUID))
                    continue;

                _cardlist.Add(GameInfo.Instance.CardList[i]);
            }

            CardData.SortUp = _sortupdown;
            _cardlist.Sort(CardData.CompareFuncGradeLevel);
            
            this._CardListInstance.UpdateList();

            if(_cardlist.Count <= 0)
            {
                if(kNoneLabel != null)
                {
                    kNoneLabel.SetActive(true);
                    //kNoneLabel.textlocalize = FLocalizeString.Instance.GetText(1595);
                }
            }
        }
        
    }
}
