using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGemLevelUpPopup : FComponent
{
	public UITexture kGemTex;
    public GameObject kLevelUpEff;
    public UILabel kNameLabel;
	public UILabel kExpLabel;
	public UILabel kLevelLabel;
    public UISprite kWakeSpr;
    public UISprite kGradeSpr;
    public UIStatusUnit kGemStatusUnit_00;
	public UIStatusUnit kGemStatusUnit_01;
	public UILabel kMaterialnumLabel;
    public UISprite kOrderSpr;
    public UIGaugeUnit kExpGaugeUnit;
	public UIButton kCancleBtn;
	public UIButton kLevelUpBtn;
	public UIGoodsUnit kGoldGoodsUnit;
    [SerializeField] private FList _GemListInstance;
    private GemData _gemdata;
    private bool _sortupdown = false;

    private List<GemData> _gemlist = new List<GemData>();
    private List<GemData> _matgemlist = new List<GemData>();

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

    public List<GemData> MatGemList { get { return _matgemlist; } }

    public UILabel kNoneLabel;
    public override void Awake()
	{
		base.Awake();

		if(this._GemListInstance == null) return;
		
		this._GemListInstance.EventUpdate = this._UpdateGemListSlot;
		this._GemListInstance.EventGetItemCount = this._GetGemElementCount;
        this._GemListInstance.InitBottomFixing();

    }
 
	public override void OnEnable()
	{
        kLevelUpEff.SetActive(false);
        InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        if(kNoneLabel != null)
            kNoneLabel.SetActive(false);

        long gemuid = (long)UIValue.Instance.GetValue(UIValue.EParamType.GemUID);
        _gemdata = GameInfo.Instance.GetGemData(gemuid);

        //_sortupdown = true;

        Utility.StopCoroutine(this, ref m_crGageExp);

        _nowlevel = _gemdata.Level;
        _curlevel = _gemdata.Level;
        _curexp = _gemdata.Exp;
        _gainexp = _gemdata.Exp;
        kGageAllGainExp = -1;
        _bsendlevelup = false;

        kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curlevel, GameSupport.GetGemMaxLevel());
        
        float fillAmount = GameSupport.GetGemLevelExpGauge(_gemdata, _gemdata.Level, _gemdata.Exp);
        kExpGaugeUnit.InitGaugeUnit(fillAmount);
        kExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (fillAmount * 100.0f)));

        int level = _gemdata.Level;
        int statusmain = GameSupport.GetTypeStatusGem(_gemdata.TableData.MainType, level, _gemdata.Wake, _gemdata.TableData);
        int statussub = GameSupport.GetTypeStatusGem(_gemdata.TableData.SubType, level, _gemdata.Wake, _gemdata.TableData);
        kGemStatusUnit_00.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + _gemdata.TableData.MainType, statusmain);
        if (_gemdata.TableData.MainType == _gemdata.TableData.SubType)
        {
            kGemStatusUnit_01.gameObject.SetActive(false);
        }
        else
        {
            kGemStatusUnit_01.gameObject.SetActive(true);
            kGemStatusUnit_01.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + _gemdata.TableData.SubType, statussub);
        }

        kExpLabel.textlocalize = "";

        _matgemlist.Clear();
        _gemlist.Clear();
        
        for (int i = 0; i < GameInfo.Instance.GemList.Count; i++)
        {
            if (GameInfo.Instance.GemList[i].Lock)
                continue;
            if (_gemdata.GemUID == GameInfo.Instance.GemList[i].GemUID)
                continue;

            //if (_gemdata.TableID == GameInfo.Instance.GemList[i].TableID)
            //    continue;

            WeaponData weapondata = GameInfo.Instance.GetEquipGemWeaponData(GameInfo.Instance.GemList[i].GemUID);
            if (weapondata != null)
                continue;
         
            _gemlist.Add(GameInfo.Instance.GemList[i]);
        }

        GemData.SortUp = _sortupdown;
        _gemlist.Sort(GemData.CompareFuncGradeLevel);

        this._GemListInstance.UpdateList();
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        kGemTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + _gemdata.TableData.Icon);
        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_gemdata.TableData.Name);
        kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _gemdata.Level, GameSupport.GetGemMaxLevel());

        kWakeSpr.spriteName = "itemGemwake_0" + _gemdata.Wake.ToString();
        kGradeSpr.spriteName = "itemgrade_L_" + _gemdata.TableData.Grade.ToString();
        //kGradeSpr.MakePixelPerfect();

        kMaterialnumLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(276), _matgemlist.Count, GameInfo.Instance.GameConfig.MatCount);
        this._GemListInstance.RefreshNotMove();

        int gold = GameSupport.GetGemLevelUpCost(_gemdata, _matgemlist);

        bool bGoldFlag = false;
        GuerrillaCampData campdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Upgrade_PriceRateDown, (int)eContentsPosKind.GEM);
        if (campdata != null)
        {
            bGoldFlag = true;
            gold -= (int)((float)gold * (float)((float)campdata.EffectValue / (float)eCOUNT.MAX_BO_FUNC_VALUE));
        }

        kGoldGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, gold, true, bGoldFlag);

        if(_gemlist.Count <= 0)
            {
                if(kNoneLabel != null)
                {
                    kNoneLabel.SetActive(true);
                    //kNoneLabel.textlocalize = FLocalizeString.Instance.GetText(1595);
                }
            }

        if (_sortupdown)
            kOrderSpr.spriteName = "ico_Filter2";
        else
            kOrderSpr.spriteName = "ico_Filter1";
    }
 

	
	private void _UpdateGemListSlot(int index, GameObject slotObject)
	{
		do
		{
            UIItemListSlot card = slotObject.GetComponent<UIItemListSlot>();
            if (null == card) break;
            card.ParentGO = this.gameObject;

            GemData data = null;
            if (0 <= index && _gemlist.Count > index)
                data = _gemlist[index];
            card.UpdateSlot(UIItemListSlot.ePosType.Gem_LevelUpMatList, index, data);
        } while(false);
	}
	
	private int _GetGemElementCount()
	{
		return _gemlist.Count; //TempValue
	}

    void FixedUpdate()
    {
        if (kGageAllGainExp == -1)
            return;
        
        int nowexp = _curexp + kGageAllGainExp;
        int level = GameSupport.GetGemExpLevel(_gemdata, nowexp);

        kExpLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PLUS_TEXT), kGageAllGainExp);
        if (_curlevel == _gemdata.Level)
            kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curlevel, GameSupport.GetGemMaxLevel());
        else
            kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curlevel, GameSupport.GetGemMaxLevel(), true);


        float fillAmount = GameSupport.GetGemLevelExpGauge(_gemdata, level, nowexp);
        kExpGaugeUnit.InitGaugeUnit(fillAmount);
        kExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (fillAmount * 100.0f)));

        if (_curlevel != level)
        {
            _curlevel = level;
            if (_curlevel >= GameSupport.GetGemMaxLevel())
                _curlevel = GameSupport.GetGemMaxLevel();

            int startlevel = _nowlevel;
            int statusmain = GameSupport.GetTypeStatusGem(_gemdata.TableData.MainType, startlevel, _gemdata.Wake, _gemdata.TableData);
            int statussub = GameSupport.GetTypeStatusGem(_gemdata.TableData.SubType, startlevel, _gemdata.Wake, _gemdata.TableData);
            int nextstatusmain = GameSupport.GetTypeStatusGem(_gemdata.TableData.MainType, _curlevel, _gemdata.Wake, _gemdata.TableData);
            int nextstatussub = GameSupport.GetTypeStatusGem(_gemdata.TableData.SubType, _curlevel, _gemdata.Wake, _gemdata.TableData);
            kGemStatusUnit_00.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + _gemdata.TableData.MainType, statusmain, nextstatusmain);

            if (_gemdata.TableData.MainType == _gemdata.TableData.SubType)
            {
                kGemStatusUnit_01.gameObject.SetActive(false);
            }
            else
            {
                kGemStatusUnit_01.gameObject.SetActive(true);
                kGemStatusUnit_01.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + _gemdata.TableData.SubType, statussub, nextstatussub);
            }
        }
    }

    private void SetExpGainTween()
    {
        Utility.StopCoroutine(this, ref m_crGageExp);
        
        _gainexp = GameSupport.GetGemMatExp(_matgemlist);
        int nRemainExp = GameSupport.GetRemainGemExpToMaxLevel(_gemdata);
        if (_gainexp >= nRemainExp)
            _gainexp = nRemainExp;

        m_crGageExp = StartCoroutine(Utility.UpdateCoroutineValue((x) => kGageAllGainExp = (int)x, kGageAllGainExp, _gainexp, 0.5f));
        
    }

    public bool AddMatGem(GemData gemdata, bool bmessage = true)
    {
        if (_bsendlevelup)
            return false;


        var matitem = _matgemlist.Find(x => x.GemUID == gemdata.GemUID);
        if (matitem != null)
        {
            DelMatGem(gemdata);
        }
        else
        {
            if (GameSupport.IsMaxLevelGem(_gemdata, _curlevel))
                return false;

            if (_matgemlist.Count < GameInfo.Instance.GameConfig.MatCount)
            {
                if (IsCheckMaxLevel())
                {
                    if (bmessage)
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3013));
                    return false;
                }
                else
                {
                    _matgemlist.Add(gemdata);
                }
            }
        }

        Renewal(true);

        SetExpGainTween();

        return true;
    }

    public void DelMatGem(GemData gemdata)
    {
        var matitem = _matgemlist.Find(x => x.GemUID == gemdata.GemUID);
        if (matitem == null)
            return;


        _matgemlist.Remove(matitem);
        Renewal(true);

        SetExpGainTween();
    }

    private bool IsCheckMaxLevel()
    {
        int exp = GameSupport.GetGemMatExp(_matgemlist);

        int level = GameSupport.GetGemExpLevel(_gemdata, _gemdata.Exp + exp);
        int maxlevel = GameSupport.GetGemMaxLevel();
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
        _nowlevel = _gemdata.Level;
        _curlevel = _gemdata.Level;
        _curexp = _gemdata.Exp;
        _gainexp = _gemdata.Exp;
        kGageAllGainExp = -1;

        _matgemlist.Clear();
        InitComponent();
        Renewal(true);
    }

    public void OnClick_AutoMatBtn()
	{
        if (_bsendlevelup)
            return;

        Utility.StopCoroutine(this, ref m_crGageExp);
        _nowlevel = _gemdata.Level;
        _curlevel = _gemdata.Level;
        _curexp = _gemdata.Exp;
        _gainexp = _gemdata.Exp;
        kGageAllGainExp = -1;

        _matgemlist.Clear();
        int count = GameInfo.Instance.GameConfig.MatCount;
        if (count > _gemlist.Count)
            count = _gemlist.Count;

        for (int i = 0; i < count; i++)
            AddMatGem(_gemlist[i], false); 
    }
	
	public void OnClick_OrderBtn()
	{
        if (_bsendlevelup)
            return;

        _sortupdown = !_sortupdown;
        GemData.SortUp = _sortupdown;
        _gemlist.Sort(GemData.CompareFuncGradeLevel);
        this._GemListInstance.UpdateList();
        Renewal(true);
    }
	
	public void OnClick_BackBtn()
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
        if (GameSupport.IsMaxLevelGem(_gemdata)) //최대 레벨
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3004));
            return;
        }
        int matcount = 0;
        int gold = 0;
   
        matcount = _matgemlist.Count;
        gold = GameSupport.GetGemLevelUpCost(_gemdata, _matgemlist);
        GuerrillaCampData campdata = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Upgrade_PriceRateDown, (int)eContentsPosKind.GEM);
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
        for (int i = 0; i < _matgemlist.Count; i++)
            matlist.Add(_matgemlist[i].GemUID);

        _sendlevel = _gemdata.Level;
        _sendexp = _gemdata.Exp;
        _bsendlevelup = true;
        GameInfo.Instance.Send_ReqLvUpGem(_gemdata.GemUID, matlist, OnNetGemLevelUp);
    }

    public void OnNetGemLevelUp(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            _bsendlevelup = false;
            return;
        }
        StartCoroutine(GemLevelUpResultCoroutine(pktmsg));
    }

    IEnumerator GemLevelUpResultCoroutine(PktMsgType pktmsg)
    {
        int success = 0;
        _nowlevel = _sendlevel;
        _curlevel = _sendlevel;
        _curexp = _sendexp;
        _gainexp = _sendexp;
        kGageAllGainExp = -1;

        kExpLabel.textlocalize = "";
        if (_curlevel == _gemdata.Level)
            kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curlevel, GameSupport.GetGemMaxLevel());
        else
            kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _curlevel, GameSupport.GetGemMaxLevel(), true);

        float fillAmount = GameSupport.GetGemLevelExpGauge(_gemdata, _sendlevel, _sendexp);
        kExpGaugeUnit.InitGaugeUnit(fillAmount);
        kExpGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (fillAmount * 100.0f)));

        int statusmain = GameSupport.GetTypeStatusGem(_gemdata.TableData.MainType, _nowlevel, _gemdata.Wake, _gemdata.TableData);
        int statussub = GameSupport.GetTypeStatusGem(_gemdata.TableData.SubType, _nowlevel, _gemdata.Wake, _gemdata.TableData);
        int nextstatusmain = GameSupport.GetTypeStatusGem(_gemdata.TableData.MainType, _nowlevel, _gemdata.Wake, _gemdata.TableData);
        int nextstatussub = GameSupport.GetTypeStatusGem(_gemdata.TableData.SubType, _nowlevel, _gemdata.Wake, _gemdata.TableData);
        kGemStatusUnit_00.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + _gemdata.TableData.MainType, statusmain, nextstatusmain);

        if (_gemdata.TableData.MainType == _gemdata.TableData.SubType)
        {
            kGemStatusUnit_01.gameObject.SetActive(false);
        }
        else
        {
            kGemStatusUnit_01.gameObject.SetActive(true);
            kGemStatusUnit_01.InitStatusUnit((int)eTEXTID.STAT_TEXT_ID + _gemdata.TableData.SubType, statussub, nextstatussub);
        }

        GameSupport.PlayParticle(kLevelUpEff);
        SoundManager.Instance.PlayUISnd(28);

        yield return new WaitForSeconds(2.0f);

        _gainexp = _gemdata.Exp - _sendexp;
        Utility.StopCoroutine(this, ref m_crGageExp);
        m_crGageExp = StartCoroutine(Utility.UpdateCoroutineValue((x) => kGageAllGainExp = (int)x, kGageAllGainExp, _gainexp, 1.0f));
        SoundManager.Instance.PlayUISnd(58);
        yield return new WaitForSeconds(1.0f);

        InitComponent();
        Renewal(true);

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("GemInfoPopup");
        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
        {
            itempanel.InitComponent();
            itempanel.Renewal(true);
        }
        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.CHARINFO)
        {
            LobbyUIManager.Instance.Renewal("CharInfoPanel");
            LobbyUIManager.Instance.InitComponent("WeaponGemSeletePopup");
            LobbyUIManager.Instance.Renewal("WeaponGemSeletePopup");
        }
        if (GameSupport.IsMaxLevelGem(_gemdata)) //최대 레벨
        {
            OnClickClose();
        }
        _bsendlevelup = false;
    }
}
