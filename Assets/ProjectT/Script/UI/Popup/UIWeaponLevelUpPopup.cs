using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Holoville.HOTween;

public class UIWeaponLevelUpPopup : FComponent
{
    public UITexture kWeaponTex;
    public GameObject kLevelUpEff;
    public GameObject kLevelUpEff_High;
    public GameObject kSuccessType;
    public List<GameObject> kSuccessList;
    public UISprite kGradeSpr;
    public UISprite kWakeSpr;
    public UILabel kNameLabel;
    public UILabel kEnchantLabel;
    public UILabel kExpLabel;
	public UILabel kLevelLabel;
    public UIGaugeUnit kExpGaugeUnit;
    public UIStatusUnit kWeaponStatusUnit_ATK;
	public UIStatusUnit kWeaponStatusUnit_CRI;
    public UILabel kMaterialnumLabel;
    public UILabel kMaterialLabel;
    public UIButton kCancleBtn;
	public UIButton kLevelUpBtn;
	public UIGoodsUnit kGoldGoodsUnit;
    public UISprite kOrderSpr;
    public GameObject kMatWeapon;
    public GameObject kMatItem;
    [SerializeField] private FList _WeaponListInstance;
    [SerializeField] private FList _ItemListInstance;
    private WeaponData _weapondata;
    private bool _bmatItem = true;
    private bool _sortupdown = true;
    private List<WeaponData> _weaponlist = new List<WeaponData>();
    private List<WeaponData> _matweaponlist = new List<WeaponData>();
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
    public List<WeaponData> MatWeaponList { get { return _matweaponlist; } }

    public UILabel kNoneLabel;

    public override void Awake()
	{
		base.Awake();

		if(this._WeaponListInstance == null) return;
		


		this._WeaponListInstance.EventUpdate = this._UpdateWeaponListSlot;
		this._WeaponListInstance.EventGetItemCount = this._GetWeaponElementCount;
        this._WeaponListInstance.InitBottomFixing();

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

        SetInitWeaponInfo();

        var list = GameInfo.Instance.ItemList.FindAll(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_WEAPON_LEVELUP);
        if (list.Count != 0)
            _bmatItem = true;
        else
            _bmatItem = false;

        SetMatType(_bmatItem);
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        kGradeSpr.spriteName = "itemgrade_L_" + _weapondata.TableData.Grade.ToString();
        //kGradeSpr.MakePixelPerfect();

        kWakeSpr.spriteName = "itemwake_0" + _weapondata.Wake.ToString();
        kWakeSpr.MakePixelPerfect();
        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_weapondata.TableData.Name);
        kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curlevel, GameSupport.GetWeaponMaxLevel(_weapondata));

        if (_weapondata.EnchantLv > 0)
        {
            kEnchantLabel.SetActive(true);
            kEnchantLabel.textlocalize = string.Format("+{0}", _weapondata.EnchantLv);
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

            gold = GameSupport.GetWeaponLevelUpItemCost(_weapondata, _matitemlist);

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
            kMaterialnumLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(276), _matweaponlist.Count, GameInfo.Instance.GameConfig.MatCount);
            this._WeaponListInstance.RefreshNotMove();

            gold = GameSupport.GetWeaponLevelUpCost(_weapondata, _matweaponlist);

            kMaterialLabel.textlocalize = FLocalizeString.Instance.GetText(1318);

            if(_weaponlist.Count <= 0)
            {
                if(kNoneLabel != null)
                {
                    kNoneLabel.SetActive(true);
                    //kNoneLabel.textlocalize = FLocalizeString.Instance.GetText(1595);
                }
            }
        }

        bool bGoldFlag = false;

        GuerrillaCampData campdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Upgrade_PriceRateDown, (int)eContentsPosKind.WEAPON);
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
 
	private void _UpdateWeaponListSlot(int index, GameObject slotObject)
	{
        do
        {
            UIItemListSlot card = slotObject.GetComponent<UIItemListSlot>();
            if (null == card) break;
            card.ParentGO = this.gameObject;

            WeaponData data = null;
            if (0 <= index && _weaponlist.Count > index)
                data = _weaponlist[index];
            card.UpdateSlot(UIItemListSlot.ePosType.Weapon_LevelUpMatList, index, data);
        } while (false);
    }
	
	private int _GetWeaponElementCount()
	{
		return _weaponlist.Count;
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
            card.UpdateSlot(UIItemListSlot.ePosType.Weapon_LevelUpMatItemList, index, data.ItemData, data.Count);
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
        int level = GameSupport.GetWeaponExpLevel(_weapondata, nowexp);

        kExpLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PLUS_TEXT), kGageAllGainExp);
        if (_curlevel == _weapondata.Level)
            kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curlevel, GameSupport.GetWeaponMaxLevel(_weapondata));
        else
            kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curlevel, GameSupport.GetWeaponMaxLevel(_weapondata), true);

        float fillAmount = GameSupport.GetWeaponLevelExpGauge(_weapondata, level, nowexp);
        kExpGaugeUnit.InitGaugeUnit(fillAmount);
        kExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (fillAmount * 100.0f)));
        
        if (_curlevel != level)
        {
            _curlevel = level;
            if (_curlevel >= GameSupport.GetWeaponMaxLevel(_weapondata))
                _curlevel = GameSupport.GetWeaponMaxLevel(_weapondata);

            kWeaponStatusUnit_ATK.InitStatusUnit((int)eTEXTID.STAT_ATK, _weapondata.GetWeaponATK(_nowlevel), _weapondata.GetWeaponATK(_curlevel));
            kWeaponStatusUnit_CRI.InitStatusUnit((int)eTEXTID.STAT_CRI, _weapondata.GetWeaponCRI(_nowlevel), _weapondata.GetWeaponCRI(_curlevel));
        }
    }

    private void SetInitWeaponInfo()
    {
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.WeaponUID);
        _weapondata = GameInfo.Instance.GetWeaponData(uid);

        RenderTargetWeapon.Instance.gameObject.SetActive(true);
        RenderTargetWeapon.Instance.InitRenderTargetWeapon(_weapondata.TableID, _weapondata.WeaponUID, true);
        _bsendlevelup = false;
    }

    private void SetExpGainTween()
    {
        Utility.StopCoroutine(this, ref m_crGageExp);
        
        if( _bmatItem )
            _gainexp = GameSupport.GetItemMatExp(_matitemlist);
        else 
            _gainexp = GameSupport.GetWeaponMatExp(_matweaponlist);

        int nRemainExp = GameSupport.GetRemainWeaponExpToMaxLevel(_weapondata);
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
        if (GameSupport.IsMaxLevelWeapon(_weapondata, _curlevel))
            return false;


        var targetitem = _itemlist.Find(x => x.ItemData.ItemUID == matitemdata.ItemUID);
        if (targetitem == null)
            return false;
        int count = targetitem.Count - 1;
        if (count < 0)
            return false;

        if (IsCheckMaxLevel())
        {
            if(bmessage)
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

    public bool AddMatWeapon(WeaponData matweapondata, bool bmessage = true)
    {
        if (_bsendlevelup)
            return false;

        var matitem = _matweaponlist.Find(x => x.WeaponUID == matweapondata.WeaponUID);
        if (matitem != null)
        {
            DelMatWeapon(matweapondata);
        }
        else
        {
            if (GameSupport.IsMaxLevelWeapon(_weapondata, _curlevel))
                return false;

            if (_matweaponlist.Count < GameInfo.Instance.GameConfig.MatCount)
            {
                if (IsCheckMaxLevel())
                {
                    if(bmessage)
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3013));
                    return false;
                }
                else
                {
                    _matweaponlist.Add(matweapondata);
                }
            }
        }

        Renewal(true);

        SetExpGainTween();

        return true;
    }
    public void DelMatWeapon(WeaponData matweapondata)
    {
        var matitem = _matweaponlist.Find(x => x.WeaponUID == matweapondata.WeaponUID);
        if (matitem == null)
            return;


        _matweaponlist.Remove(matitem);
        Renewal(true);

        SetExpGainTween();
    }

    private bool IsCheckMaxLevel()
    {
        int exp = 0;
        if(_bmatItem)
            exp = GameSupport.GetItemMatExp(_matitemlist);
        else 
            exp = GameSupport.GetWeaponMatExp(_matweaponlist);
        int level = GameSupport.GetWeaponExpLevel(_weapondata, _weapondata.Exp + exp);
        int maxlevel = GameSupport.GetWeaponMaxLevel(_weapondata);

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

        _nowlevel = _weapondata.Level;
        _curlevel = _weapondata.Level;
        _curexp = _weapondata.Exp;
        _gainexp = _weapondata.Exp;
        kGageAllGainExp = -1;

        _matitemlist.Clear();
        _matweaponlist.Clear();
        Renewal(true);
        SetMatType(_bmatItem);
    }

    public void OnClick_AutoMatBtn()
    {
        if (_bsendlevelup)
            return;

        Utility.StopCoroutine(this, ref m_crGageExp);

        _nowlevel = _weapondata.Level;
        _curlevel = _weapondata.Level;
        _curexp = _weapondata.Exp;
        _gainexp = _weapondata.Exp;
        kGageAllGainExp = -1;

        if (_bmatItem)
        {
            for( int i = 0; i < _matitemlist.Count; i++ )
            {
                var data = _matitemlist[i];
                var find = _itemlist.Find(x => x.ItemData.ItemUID == data.ItemData.ItemUID);
                if (find != null)
                    find.Count += data.Count;
            }
            _matitemlist.Clear();
            int itemnum = 0;
            for( int i = 0; i < GameInfo.Instance.GameConfig.MatCount; i++ )
            {
                var itemdata = CheckAutoItem(ref itemnum);
                if(itemdata != null)
                    AddMatItem(itemdata, false);
            }
        }
        else
        {
            _matweaponlist.Clear();
            int count = GameInfo.Instance.GameConfig.MatCount;
            if (count > _weaponlist.Count)
                count = _weaponlist.Count;

            for (int i = 0; i < count; i++)
                AddMatWeapon(_weaponlist[i], false);
        }        
    }

    private ItemData CheckAutoItem( ref int num )
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

        if(_bmatItem)
        {
            MatItemData.SortUp = _sortupdown;
            _itemlist.Sort(MatItemData.CompareFuncGrade);
            this._ItemListInstance.UpdateList();
        }
        else
        {
            WeaponData.SortUp = _sortupdown;
            _weaponlist.Sort(WeaponData.CompareFuncGradeLevel);
            this._WeaponListInstance.UpdateList();
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
        if (GameSupport.IsMaxLevelWeapon(_weapondata)) //최대 레벨
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
            gold = GameSupport.GetWeaponLevelUpItemCost(_weapondata, _matitemlist);
        }
        else
        {
            matcount = _matweaponlist.Count;
            gold = GameSupport.GetWeaponLevelUpCost(_weapondata, _matweaponlist);
        }

        GuerrillaCampData campdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Upgrade_PriceRateDown, (int)eContentsPosKind.WEAPON);
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
        for (int i = 0; i < _matweaponlist.Count; i++)
            matlist.Add(_matweaponlist[i].WeaponUID);

        _sendlevel = _weapondata.Level;
        _sendexp = _weapondata.Exp;
        _bsendlevelup = true;
        GameInfo.Instance.Send_ReqLvUpWeapon(_weapondata.WeaponUID, _bmatItem, matlist, _matitemlist,OnNetWeaponLevelUp);
    }

    
    public void OnNetWeaponLevelUp(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            _bsendlevelup = false;
            return;
        }
        StartCoroutine(WeaponLevelUpResultCoroutine(pktmsg));
    }

    IEnumerator WeaponLevelUpResultCoroutine(PktMsgType pktmsg)
    {
        int success = 0;
        _nowlevel = _sendlevel;
        _curlevel = _sendlevel;
        _curexp = _sendexp;
        _gainexp = _sendexp;
        kGageAllGainExp = -1;

        kExpLabel.textlocalize = "";
        kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curlevel, GameSupport.GetWeaponMaxLevel(_weapondata), true);
        float fillAmount = GameSupport.GetWeaponLevelExpGauge(_weapondata, _sendlevel, _sendexp);
        kExpGaugeUnit.InitGaugeUnit(fillAmount);
        kExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (fillAmount * 100.0f)));

        
        if (pktmsg != null)
        {
            PktInfoWeaponGrow cardgrow = (PktInfoWeaponGrow)pktmsg;
            success = (int)cardgrow.growState_;
        }
        if( success == (int)eGrowState.SUCC_OVER_GRATE )
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


        _gainexp = _weapondata.Exp - _sendexp;
        Utility.StopCoroutine(this, ref m_crGageExp);
        m_crGageExp = StartCoroutine(Utility.UpdateCoroutineValue((x) => kGageAllGainExp = (int)x, kGageAllGainExp, _gainexp, 1.0f));
        SoundManager.Instance.PlayUISnd(58);
        yield return new WaitForSeconds(1.0f);

        kSuccessType.SetActive(false);
        //InitComponent();
        SetMatType(_bmatItem);
        
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("WeaponInfoPopup");

        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
        {
            itempanel.InitComponent();
            itempanel.Renewal(true);
        }

        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.CHARINFO)
        {
            LobbyUIManager.Instance.Renewal("CharInfoPanel");
            LobbyUIManager.Instance.InitComponent("CharWeaponSeletePopup");
            LobbyUIManager.Instance.Renewal("CharWeaponSeletePopup");
        }

        SetInitWeaponInfo();
        Renewal(true);

        if (GameSupport.IsMaxLevelWeapon(_weapondata)) //최대 레벨
        {
            base.OnClickClose();
        }

        if (GameSupport.IsWeaponOpenTerms_Effect(_weapondata))
        {
            UIValue.Instance.SetValue(UIValue.EParamType.SelectRewardType, eREWARDTYPE.WEAPON);
            UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, _weapondata.WeaponUID);
            DirectorUIManager.Instance.PlayMaxLevel();
            RenderTargetWeapon.Instance.ShowWeaponEffect(true);
        }
        _bsendlevelup = false;
    }
    
    private void SetMatType(bool bmatitem)
    {
        _bmatItem = bmatitem;
        //_sortupdown = true;

        Utility.StopCoroutine(this, ref m_crGageExp);

        _nowlevel = _weapondata.Level;
        _curlevel = _weapondata.Level;
        _curexp = _weapondata.Exp;
        _gainexp = _weapondata.Exp;
        kGageAllGainExp = -1;

        kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curlevel, GameSupport.GetWeaponMaxLevel(_weapondata));

        float fillAmount = GameSupport.GetWeaponLevelExpGauge(_weapondata, _weapondata.Level, _weapondata.Exp);
        kExpGaugeUnit.InitGaugeUnit(fillAmount);
        kExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (fillAmount * 100.0f)));

        kWeaponStatusUnit_ATK.InitStatusUnit((int)eTEXTID.STAT_ATK, _weapondata.GetWeaponATK(), _weapondata.GetWeaponATK(_nowlevel));
        kWeaponStatusUnit_CRI.InitStatusUnit((int)eTEXTID.STAT_CRI, _weapondata.GetWeaponCRI(), _weapondata.GetWeaponCRI(_nowlevel));

        kExpLabel.textlocalize = "";

        _matitemlist.Clear();
        _itemlist.Clear();

        _matweaponlist.Clear();
        _weaponlist.Clear();

        if(kNoneLabel != null)
            kNoneLabel.SetActive(false);

        if (_bmatItem)
        {
            kMatWeapon.SetActive(false);
            kMatItem.SetActive(true);

            var list = GameInfo.Instance.ItemList.FindAll(x => x.TableData.Type == (int)eITEMTYPE.MATERIAL && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_WEAPON_LEVELUP);
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
            kMatWeapon.SetActive(true);
            kMatItem.SetActive(false);

            for (int i = 0; i < GameInfo.Instance.WeaponList.Count; i++)
            {
                if (GameInfo.Instance.WeaponList[i].Lock)
                    continue;
                if (_weapondata.WeaponUID == GameInfo.Instance.WeaponList[i].WeaponUID)
                    continue;
                if (_weapondata.TableID == GameInfo.Instance.WeaponList[i].TableID)
                    continue;
                if (GameSupport.GetEquipWeaponDepot(GameInfo.Instance.WeaponList[i].WeaponUID))
                    continue;

                CharData chardata = GameInfo.Instance.GetEquipWeaponCharData(GameInfo.Instance.WeaponList[i].WeaponUID);
                if (chardata != null)
                    continue;
                if (GameInfo.Instance.GetEquipWeaponFacilityData(GameInfo.Instance.WeaponList[i].WeaponUID) != null)
                    continue;

                _weaponlist.Add(GameInfo.Instance.WeaponList[i]);
            }
            WeaponData.SortUp = _sortupdown;
            _weaponlist.Sort(WeaponData.CompareFuncGradeLevel);

            this._WeaponListInstance.UpdateList();

            if(_weaponlist.Count <= 0)
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
