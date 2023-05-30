using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UICharInfoTabCostumePanel : FComponent
{
    [SerializeField] private FList _CostumeListInstance;
    public UILabel kNameLabel;
    public UIButton kBuyBtn;
    public UILabel kBuySaleValueLabel;
    public UIGoodsUnit kBuyGoodsUnit;
    public UIButton kEquipBtn;
    public UIButton kEquipSetColorBtn;
    public UIButton kCharBuyBtn;    
    public GameObject kDeactivated;
    public UILabel kDeactivatedLabel;
    public UIButton kHairBtn;
    public UISprite kHairOn;
    public UISprite kHairOff;
    public UIButton kColorBtn;
    public UIButton kAttachBtn;
    public UISprite kAttachOn;
    public UISprite kAttachOff;
    public UIButton BtnOtherParts;
    public UISprite SprOtherParts;
    public UISprite SprOtherPartsOn;
    public UISprite SprOtherPartsOff;
    public UILabel kColorCountLabel;
    public GameObject kCostumeStat1;
    public GameObject kCostumeStat2;
    public UILabel kCostumeStatEmptyLabel;
    public UILabel kCostumeStat1StatName;
    public UILabel kCostumeStat1StatNum;
    public UILabel kCostumeStat2StatName1;
    public UILabel kCostumeStat2StatNum1;
    public UILabel kCostumeStat2StatName2;
    public UILabel kCostumeStat2StatNum2;
    public List<UILabel> kTotalStatName;
    public List<UILabel> kTotalStatNum;
    public GameObject kDyeico;
    public GameObject kDyeBtn;
    public UISprite kDyeSpr;
    [SerializeField] private UIAcquisitionInfoUnit acquisitionUnit;
    [SerializeField] private GameObject costumeInfoObj;
    [SerializeField] private UILabel costumeInfoLabel;
    
    private CharData _chardata;
    private GameTable.Character.Param _tabledata;
    private List<GameTable.Costume.Param> _costumelist = new List<GameTable.Costume.Param>();
    private int _seletecostumeid = -1;
    private GameClientTable.StoreDisplayGoods.Param _sendstoredisplaytable;

    private bool _usecostumehair = false;
    private bool _usecostumeattach = false;
    private int _costumecolormax;
    private int _costumecolor;
    private bool _costumeattach;
    private bool _costumehair;
    public CharData CharData { get { return _chardata; } }
    public int SeleteCostumeID { get { return _seletecostumeid; } }

    private bool mUseCostumeOtherParts  = false;
    private bool mCostumeOtherParts     = true;
    private System.DateTime _nowTime;

    public override void Awake()
    {
        
        base.Awake();

        if (this._CostumeListInstance == null) return;

        this._CostumeListInstance.EventUpdate = this._UpdateCostumeListSlot;
        this._CostumeListInstance.EventGetItemCount = this._GetCostumeElementCount;
        this._CostumeListInstance.UpdateList();

        Type = TYPE.Tab;
    }

    public override void OnEnable()
    {
        _CostumeListInstance.gameObject.SetActive(false);
        _CostumeListInstance.gameObject.SetActive(true);
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

        //UIValue.Instance.SetValue(UIValue.EParamType.CharCostumeID, 0);

        /*
        if( _chardata.EquipCostumeID == _seletecostumeid )
        {
            uint costumestateflag = 0;
            GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_HAIR, _costumehair);
            GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_1, _costumeattach);
            if (_chardata.CostumeColor != _costumecolor || _chardata.CostumeStateFlag != costumestateflag)
            {
                OnClick_EquipSetColorBtn();
            }
        }
        */
        //  코스튬 탭 나갈시 현재 장착하고 있는 코스튬으로 초기화
        SetSeleteCostumeID(_chardata.EquipCostumeID, true);

        RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject(RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.InGameOnly, false);
        RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject(RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.LobbyOnly, true);
    }

    public override void InitComponent()
    {
        _nowTime = GameInfo.Instance.GetNetworkTime();
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
        int tableid = (int)UIValue.Instance.GetValue(UIValue.EParamType.CharSelTableID);
        
        _chardata = GameInfo.Instance.GetCharData(uid);
        _tabledata = GameInfo.Instance.GameTable.FindCharacter(x => x.ID == tableid);

        SetCostumeList();

        RenderTargetChar.Instance.gameObject.SetActive(true);
        RenderTargetChar.Instance.InitRenderTargetChar(tableid, uid, false, eCharacterType.Character);

        int _selectCostumeID = (int)eCOUNT.NONE;

        var seletecostumeid = UIValue.Instance.GetValue(UIValue.EParamType.CharCostumeID);
        if (seletecostumeid == null)
        {
            if (_chardata != null)
                _selectCostumeID = _chardata.EquipCostumeID;
        }
        else
        {
            int charCostumeID = (int)seletecostumeid;
            if (_costumelist.Find(x => x.ID == charCostumeID) == null)
                _selectCostumeID = _chardata.EquipCostumeID;
            else
                _selectCostumeID = charCostumeID;
        }

        SetSeleteCostumeID(_selectCostumeID);

        if( RenderTargetChar.Instance.RenderPlayer ) {
            RenderTargetChar.Instance.ShowAttachedObject( false );
            RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject( RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.InGameOnly, false );
            RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject( RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.LobbyOnly, true );
        }
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        SetCostumeList();

        //kCharChangeBtn.gameObject.SetActive(false);
        //kEffectInfo.gameObject.SetActive(false);
        kBuyBtn.gameObject.SetActive(false);
        kBuySaleValueLabel.transform.parent.gameObject.SetActive(false);
        kEquipBtn.gameObject.SetActive(false);
        kEquipSetColorBtn.gameObject.SetActive(false);
        kCharBuyBtn.gameObject.SetActive(false);
        kDeactivated.SetActive(false);
        
        acquisitionUnit.SetActive(false);
        costumeInfoObj.SetActive(false);

        if (_usecostumeattach)
        {
            kAttachBtn.gameObject.SetActive(true);
            if( _costumeattach )
            {
                kAttachOn.gameObject.SetActive(false);
                kAttachOff.gameObject.SetActive(true);
            }
            else
            {
                kAttachOn.gameObject.SetActive(true);
                kAttachOff.gameObject.SetActive(false);
            }
        }
        else
            kAttachBtn.gameObject.SetActive(false);

        if(mUseCostumeOtherParts)
        {
            BtnOtherParts.gameObject.SetActive(true);

            if( _tabledata.ID == (int)ePlayerCharType.Sora ) {
                SprOtherParts.spriteName = "ico_Costume_Fox";
            }
            else {
                SprOtherParts.spriteName = "ico_FigureRoom_Fig";
            }

            if(mCostumeOtherParts)
            {
                SprOtherPartsOn.gameObject.SetActive(true);
                SprOtherPartsOff.gameObject.SetActive(false);
            }
            else
            {
                SprOtherPartsOn.gameObject.SetActive(false);
                SprOtherPartsOff.gameObject.SetActive(true);
            }
        }
        else
        {
            BtnOtherParts.gameObject.SetActive(false);
        }

        if (_costumecolormax > 1)
            kColorBtn.gameObject.SetActive(true);
        else
            kColorBtn.gameObject.SetActive(false);
        
        kColorCountLabel.textlocalize = (_costumecolor+1).ToString();
        
        var costumetabledata = _costumelist.Find(x => x.ID == _seletecostumeid);
        if (costumetabledata == null)
            return;

        if (string.IsNullOrEmpty(costumetabledata.HairModel))
            kHairBtn.gameObject.SetActive(false);
        else
        {
            kHairBtn.gameObject.SetActive(true);

            kHairOn.SetActive(_costumehair);
            kHairOff.SetActive(!_costumehair);
        }
            
        
        if( costumetabledata.ColorCnt == 1 )
            kColorBtn.gameObject.SetActive(false); 
        else
            kColorBtn.gameObject.SetActive(true);

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(costumetabledata.Name);
        kNameLabel.color = GameInfo.Instance.GameConfig.CostumeGradeColor[costumetabledata.Grade];

        kCostumeStat1.SetActive(false);
        kCostumeStat2.SetActive(false);
        kCostumeStatEmptyLabel.gameObject.SetActive(false);

        if ( GameSupport.IsOnCostumePassvie(costumetabledata) )
        {
            List<int> list = new List<int>();
            int count = GameSupport.GetCostumePassvieStatCount(costumetabledata);
            GameSupport.GetCostumePassvieStatList(costumetabledata, ref list);

            if ( count == 1 )
            {
                kCostumeStat1.SetActive(true);
                kCostumeStat1StatName.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.STAT_HP + list[0]);
                kCostumeStat1StatNum.textlocalize = string.Format("{0:#,##0}", GameSupport.GetCostumePassvieStat(costumetabledata, eCHARABILITY.HP + list[0]));

            }
            else
            {
                kCostumeStat2.SetActive(true);
                kCostumeStat2StatName1.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.STAT_HP + list[0]);
                kCostumeStat2StatNum1.textlocalize = string.Format("{0:#,##0}", GameSupport.GetCostumePassvieStat(costumetabledata, eCHARABILITY.HP + list[0]));
                kCostumeStat2StatName2.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.STAT_HP + list[1]);
                kCostumeStat2StatNum2.textlocalize = string.Format("{0:#,##0}", GameSupport.GetCostumePassvieStat(costumetabledata, eCHARABILITY.HP + list[1]));
            }
        }
        else
        {
            kCostumeStatEmptyLabel.gameObject.SetActive(true);
        }

        
        CharData chardata = GameInfo.Instance.GetCharDataByTableID(costumetabledata.CharacterID);
        if (chardata == null)
        {
            kCharBuyBtn.gameObject.SetActive(true);
            return;
        }

        kTotalStatName[0].textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.STAT_HP);
        kTotalStatName[1].textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.STAT_ATK);
        kTotalStatName[2].textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.STAT_DEF);
        kTotalStatName[3].textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.STAT_CRI);
        kTotalStatNum[0].textlocalize = string.Format("{0:#,##0}", GameSupport.GetCostumeStat(_chardata, eCHARABILITY.HP));
        kTotalStatNum[1].textlocalize = string.Format("{0:#,##0}", GameSupport.GetCostumeStat(_chardata, eCHARABILITY.ATK));
        kTotalStatNum[2].textlocalize = string.Format("{0:#,##0}", GameSupport.GetCostumeStat(_chardata, eCHARABILITY.DEF));
        kTotalStatNum[3].textlocalize = string.Format("{0:#,##0}", GameSupport.GetCostumeStat(_chardata, eCHARABILITY.CRI));

        if (_chardata.EquipCostumeID == _seletecostumeid)
        {
            uint costumestateflag = 0;
            GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_HAIR, _costumehair);
            GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_1, _costumeattach);
            GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_2, mCostumeOtherParts);
            if (_chardata.CostumeColor != _costumecolor || _chardata.CostumeStateFlag != costumestateflag)
            {
                kEquipSetColorBtn.gameObject.SetActive(true);
            }
            else
            {
                kDeactivated.SetActive(true);
                kDeactivatedLabel.textlocalize = FLocalizeString.Instance.GetText(1108);
            }            
        }
        else
        {
            if (GameInfo.Instance.HasCostume(_seletecostumeid)) //
            {
                //착용하기
                kEquipBtn.gameObject.SetActive(true);
            }
            else
            {
                if (!LobbyUIManager.Instance.IsShowCostumeSlot(costumetabledata))
                {
                    GameClientTable.Book.Param costumeBookData = GameInfo.Instance.GameClientTable.FindBook(x => x.ItemID == costumetabledata.ID && x.Group == (int)eBookGroup.Costume);
                    if (costumeBookData != null)
                    {
                        if (costumeBookData.AcquisitionID > (int) eCOUNT.NONE)
                        {
                            acquisitionUnit.SetActive(true);
                            acquisitionUnit.UpdateSlot(costumeBookData.AcquisitionID);
                        }
                        else
                        {
                            costumeInfoObj.SetActive(true);
                            costumeInfoLabel.textlocalize = FLocalizeString.Instance.GetText(costumeBookData.Desc);
                        }
                    }
                }
                else
                {
                    var storetable = GameInfo.Instance.GameTable.FindStore(costumetabledata.StoreID);
                    kBuyGoodsUnit.gameObject.SetActive(true);
                    kBuyGoodsUnit.InitGoodsUnit((eGOODSTYPE)storetable.PurchaseIndex, storetable.PurchaseValue);

                    kBuyBtn.gameObject.SetActive(true);
                    var storesaledata = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == costumetabledata.StoreID);
                    if (storesaledata != null)
                    {
                        int discountrate = GameSupport.GetStoreDiscountRate(storetable.ID);
                        int salevalue = storetable.PurchaseValue - (int)(storetable.PurchaseValue * (float)((float)discountrate / (float)eCOUNT.MAX_BO_FUNC_VALUE));


                        kBuySaleValueLabel.transform.parent.gameObject.SetActive(true);
                        kBuySaleValueLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), storetable.PurchaseValue);

                        kBuyGoodsUnit.InitGoodsUnit((eGOODSTYPE)storetable.PurchaseIndex, salevalue);
                    }
                }
            }
        }
        
        bool bDyeingActive = costumetabledata.UseDyeing != 0;
        if (bDyeingActive)
        {
            kColorBtn.gameObject.SetActive(true);
        }
        
        kDyeico.SetActive(bDyeingActive && _costumecolormax <= _costumecolor);
        kDyeBtn.SetActive(bDyeingActive && GameInfo.Instance.HasCostume(_seletecostumeid));
        kDyeSpr.gameObject.SetActive(bDyeingActive);
        
        _CostumeListInstance.RefreshNotMoveAllItem();
    }

    private void SetCostumeList()
    {
        if (_tabledata == null)
        {
            return;
        }

        _costumelist.Clear();
        List<GameTable.Costume.Param> costumelist = GameInfo.Instance.GameTable.FindAllCostume(x => x.CharacterID == _tabledata.ID);

        //보유한 코스튬
        List<GameTable.Costume.Param> possessionList = new List<GameTable.Costume.Param>();
        //구매가능 코스튬
        List<GameTable.Costume.Param> buyList = new List<GameTable.Costume.Param>();
        //구매불가 코스튬
        List<GameTable.Costume.Param> notBuyList = new List<GameTable.Costume.Param>();

        for (int i = 0; i < costumelist.Count; i++)
        {
            GameTable.Costume.Param data = costumelist[i];

            if (GameInfo.Instance.HasCostume(data.ID))
            {
                possessionList.Add(data);
                continue;
            }

            if (!LobbyUIManager.Instance.IsShowCostumeSlot(data))
            {
                GameClientTable.Book.Param costumeBookData = GameInfo.Instance.GameClientTable.FindBook(x => x.Group == (int)eBookGroup.Costume && x.ItemID == data.ID);
                if(costumeBookData != null)
                    notBuyList.Add(data);
                continue;
            }

            GameClientTable.StoreDisplayGoods.Param param = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == data.StoreID);
            if (param == null)
                continue;

            if (!GameSupport.IsStoreGoodsTimeExpire(param, _nowTime))
            {
                notBuyList.Add(data);
            }
            else
            {
                buyList.Add(data);
            }

        }

        possessionList.Sort(CompareFuncOrderNum);
        buyList.Sort(CompareFuncOrderNum);
        notBuyList.Sort(CompareFuncOrderNum);

        _costumelist.AddRange(possessionList);
        _costumelist.AddRange(buyList);
        _costumelist.AddRange(notBuyList);
    }

    private void _UpdateCostumeListSlot(int index, GameObject slotObject)
    {
        do
        {
            UICostumeListSlot card = slotObject.GetComponent<UICostumeListSlot>();
            if (null == card) break;

            GameTable.Costume.Param data = null;
            if (0 <= index && _costumelist.Count > index)
            {
                data = _costumelist[index];
            }

            card.ParentGO = this.gameObject;
            card.UpdateSlot(index, data);
        } while (false);
    }

    private int _GetCostumeElementCount()
    {
        return _costumelist.Count; //TempValue
    }
    public void OnClick_ViewBtn()
    {
        UICharInfoPanel CharInfoPanel = LobbyUIManager.Instance.GetUI("CharInfoPanel") as UICharInfoPanel;
        if(CharInfoPanel != null)
        {
            Log.Show(CharInfoPanel.kCharTex.transform.localPosition);
            CharViewer.ShowCharPopup("CharInfoPanel", CharInfoPanel.kCharTex.gameObject, CharInfoPanel.kCharTex.transform.parent);
            //UITopPanel toppanel = LobbyUIManager.Instance.GetUI<UITopPanel>("TopPanel");
            //toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.OUT);
            
        }
        
    }
    public void OnClick_HairBtn()
    {
        if (!_usecostumehair)
            return;

        //if (_chardata.EquipCostumeID != _seletecostumeid)
        //    return;

        _costumehair = !_costumehair;

        uint costumestateflag = 0;
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_HAIR, _costumehair);
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_1, _costumeattach);
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_2, mCostumeOtherParts);
        RenderTargetChar.Instance.SetCostumeBody(_seletecostumeid, _costumecolor, (int)costumestateflag, CharData.DyeingData);
        Renewal(true);
    }

    public void OnClick_ColorBtn()
    {
        _costumecolor += 1;
        int costumeColorMax = _costumecolormax;
        
        GameTable.Costume.Param costume = GameInfo.Instance.GameTable.FindCostume(_seletecostumeid);
        if (costume != null)
            costumeColorMax = _costumecolormax + costume.UseDyeing;
        
        if (_costumecolor >= costumeColorMax)
            _costumecolor = 0;

        uint costumestateflag = 0;
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_HAIR, _costumehair);
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_1, _costumeattach );
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_2, mCostumeOtherParts);
        RenderTargetChar.Instance.SetCostumeBody(_seletecostumeid, _costumecolor, (int)costumestateflag, CharData.DyeingData);
        Renewal(true);
    }
    
    public void Restore()
    {
        uint costumestateflag = 0;
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_HAIR, _costumehair);
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_1, _costumeattach );
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_2, mCostumeOtherParts);
        RenderTargetChar.Instance.SetCostumeBody(_seletecostumeid, _costumecolor, (int)costumestateflag, CharData.DyeingData);
        Renewal(true);
    }

    public void OnClick_DyeBtn()
    {
        UICostumeDyePopup popup = LobbyUIManager.Instance.GetUI("CostumeDyePopup") as UICostumeDyePopup;
        if (popup != null)
        {
            GameTable.Costume.Param costume = GameInfo.Instance.GameTable.FindCostume(_seletecostumeid);
            if (costume != null)
            {
                popup.SetCostume(costume);
                popup.SetUIActive(true);
            }
        }
    }
    
    public void OnClick_AttachBtn()
    {
        if (!_usecostumeattach)
            return;

        _costumeattach = !_costumeattach;

        uint costumestateflag = 0;
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_HAIR, _costumehair);
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_1, _costumeattach);
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_2, mCostumeOtherParts);
        RenderTargetChar.Instance.SetCostumeBody(_seletecostumeid, _costumecolor, (int)costumestateflag, CharData.DyeingData);
        Renewal(true);
    }

    public void OnBtnOtherParts()
    {
        if(!mUseCostumeOtherParts)
        {
            return;
        }

        mCostumeOtherParts = !mCostumeOtherParts;

        uint costumestateflag = 0;
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_HAIR, _costumehair);
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_1, _costumeattach);
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_2, mCostumeOtherParts);
        RenderTargetChar.Instance.SetCostumeBody(_seletecostumeid, _costumecolor, (int)costumestateflag, CharData.DyeingData);
        Renewal(true);
    }

    public void OnClick_BuyBtn()
    {
        var costumetabledata = _costumelist.Find(x => x.ID == _seletecostumeid);
        if (costumetabledata == null)
            return;

        var data = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == costumetabledata.StoreID);
        if (data == null)
            return;

        _sendstoredisplaytable = data;
        StoreBuyPopup.Show(_sendstoredisplaytable, ePOPUPGOODSTYPE.BASE_NONE_BTN, OnMsg_Purchase, OnMsg_Purchase_Cancel);
    }

    public void OnClick_EquipSetColorBtn()
    {
        if (_chardata.EquipCostumeID != _seletecostumeid)
            return;

        uint costumestateflag = 0;
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_HAIR, _costumehair);
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_1, _costumeattach);
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_2, mCostumeOtherParts);
        if (_chardata.CostumeColor != _costumecolor || _chardata.CostumeStateFlag != costumestateflag)
        {
			GameTable.Costume.Param find = GameInfo.Instance.GameTable.FindCostume(x => x.ID == _seletecostumeid);
			if (find != null && find.SubHairChange == 1)
			{
				bool isOn = GameSupport._IsOnBitIdx(costumestateflag, (int)(eCostumeStateFlag.CSF_HAIR));
				GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)(eCostumeStateFlag.CSF_HAIR), !isOn);
			}

			GameInfo.Instance.Send_ReqSetMainCostumeChar(_chardata.CUID, _seletecostumeid, _costumecolor, (int)costumestateflag, false, OnNetCharCostumeEquip);
        }
    }

    public void OnClick_EquipBtn()
    {
        var costumetabledata = _costumelist.Find(x => x.ID == _seletecostumeid);
        if (costumetabledata == null)
            return;

        uint costumestateflag = 0;
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_HAIR, _costumehair);
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_1, _costumeattach);
        GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_2, mCostumeOtherParts);

		if (costumetabledata.SubHairChange == 1)
		{
			bool isOn = GameSupport._IsOnBitIdx(costumestateflag, (int)(eCostumeStateFlag.CSF_HAIR));
			GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)(eCostumeStateFlag.CSF_HAIR), !isOn);
		}

		GameInfo.Instance.Send_ReqSetMainCostumeChar(_chardata.CUID, costumetabledata.ID, _costumecolor, (int)costumestateflag, true, OnNetCharCostumeEquip);
    }

    public void OnClick_CharBuyBtn()
    {
        var costumetabledata = _costumelist.Find(x => x.ID == _seletecostumeid);
        if (costumetabledata == null)
            return;
        CharData chardata = GameInfo.Instance.GetCharDataByTableID(costumetabledata.CharacterID);
        if (chardata != null)
            return;
        UIStorePanel storePanel = LobbyUIManager.Instance.GetUI<UIStorePanel>("StorePanel");
        if (storePanel != null)
        {
            storePanel.DirectShow(UIStorePanel.eStoreTabType.STORE_CHAR, costumetabledata.CharacterID);
        }
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.STORE);
    }



    public void OnMsg_Purchase()
    {
        var storetabledata = GameInfo.Instance.GameTable.FindStore(x => x.ID == _sendstoredisplaytable.StoreID);
        if (storetabledata == null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3009));
            return;
        }

		if ( (eGOODSTYPE)storetabledata.PurchaseIndex != eGOODSTYPE.NONE && (eGOODSTYPE)storetabledata.PurchaseIndex != eGOODSTYPE.GOODS ) {
			int purchaseValue = storetabledata.PurchaseValue;
			StoreSaleData storeSaleData = GameInfo.Instance.ServerData.StoreSaleList.Find( x => x.TableID == storetabledata.ID );
			if ( storeSaleData != null ) {
				int discountRate = GameSupport.GetStoreDiscountRate( storetabledata.ID );
				purchaseValue = storetabledata.PurchaseValue - (int)( storetabledata.PurchaseValue * (float)( (float)discountRate / (float)eCOUNT.MAX_BO_FUNC_VALUE ) );
			}

			if ( !GameSupport.IsCheckGoods( (eGOODSTYPE)storetabledata.PurchaseIndex, purchaseValue ) ) {
				return;
			}
		}

		GameInfo.Instance.Send_ReqStorePurchase(_sendstoredisplaytable.StoreID, false, 1, OnNetPurchase);
    }

    public void OnMsg_Purchase_Cancel()
    {

    }

    public void OnNetPurchase(int result, PktMsgType pktmsg)
    {
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");

        LobbyUIManager.Instance.Renewal("CharInfoPanel");

        Renewal(true);

        var storetabledata = GameInfo.Instance.GameTable.FindStore(x => x.ID == _sendstoredisplaytable.StoreID);
        if ((eREWARDTYPE)storetabledata.ProductType == eREWARDTYPE.COSTUME)  
        {
            GameTable.Costume.Param param = GameInfo.Instance.GameTable.FindCostume(SeleteCostumeID);

            string productName = FLocalizeString.Instance.GetText(param.Name);
            string str = FLocalizeString.Instance.GetText(3049, productName);

            MessageToastPopup.Show(str, true);

            VoiceMgr.Instance.PlayChar(eVOICECHAR.CostumeBuy, _chardata.TableID);
        }
    }

    public void OnNetCharCostumeEquip(int result, PktMsgType pktmsg)
    {
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
        int tableid = (int)UIValue.Instance.GetValue(UIValue.EParamType.CharSelTableID);

        RenderTargetChar.Instance.gameObject.SetActive(true);
        RenderTargetChar.Instance.InitRenderTargetChar(tableid, uid, true, eCharacterType.Character);

        bool refreshLobbyChar = false;
        for(int i = 0; i < GameInfo.Instance.UserData.ArrLobbyBgCharUid.Length; i++)
        {
            if(GameInfo.Instance.UserData.ArrLobbyBgCharUid[i] == uid)
            {
                refreshLobbyChar = true;
            }
        }

        //if (uid == GameInfo.Instance.UserData.MainCharUID)
        if(refreshLobbyChar)
        {
            Lobby.Instance.ChangeMainChar();
            Lobby.Instance.SetShowLobbyPlayer();
        }

        VoiceMgr.Instance.PlayChar(eVOICECHAR.CostumeChange, _chardata.TableID);
        Renewal(true);
    }

    public void SetSeleteCostumeID(int tableid, bool disable = false)
    {
        _seletecostumeid = tableid;


        long charuid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
        int chartableid = (int)UIValue.Instance.GetValue(UIValue.EParamType.CharSelTableID);
        CharData chardata = GameInfo.Instance.GetCharData(charuid);
        var costumetabledata = GameInfo.Instance.GameTable.FindCostume(tableid);
        if (costumetabledata == null || chardata == null)
            return;
        
        _usecostumehair = false;
        _usecostumeattach = false;
        mUseCostumeOtherParts = false;
        
        if ( chardata.EquipCostumeID == tableid )
        {
            _costumecolor = chardata.CostumeColor;
            _costumeattach = chardata.IsOnCostumeStateFlag(eCostumeStateFlag.CSF_ATTACH_1);
            _costumehair = chardata.IsOnCostumeStateFlag(eCostumeStateFlag.CSF_HAIR);
            mCostumeOtherParts = chardata.IsOnCostumeStateFlag(eCostumeStateFlag.CSF_ATTACH_2);
        }
        else
        {
            _costumecolor = 0;
            _costumeattach = false;

			_costumehair = false;
			if (!string.IsNullOrEmpty(costumetabledata.HairModel) && costumetabledata.SubHairChange == 1)
			{
				_costumehair = !_costumehair;
			}

			mCostumeOtherParts = false;
        }

        int costumeid = chardata.EquipCostumeID;
        int costumestateflag = chardata.CostumeStateFlag;
        int costumecolor = chardata.CostumeColor;

        uint flag = 0;
        GameSupport._DoOnOffBitIdx(ref flag, (int)eCostumeStateFlag.CSF_HAIR, _costumehair);
        GameSupport._DoOnOffBitIdx(ref flag, (int)eCostumeStateFlag.CSF_ATTACH_1, _costumeattach);
        GameSupport._DoOnOffBitIdx(ref flag, (int)eCostumeStateFlag.CSF_ATTACH_2, mCostumeOtherParts);

        chardata.EquipCostumeID = _seletecostumeid;
        chardata.CostumeColor = _costumecolor;
        chardata.CostumeStateFlag = (int)flag;

        RenderTargetChar.Instance.InitRenderTargetChar(chartableid, charuid, true, eCharacterType.Character);
        RenderTargetChar.Instance.RenderPlayer.aniEvent.aniFace.SetLayerWeight(1, 0f);

        if (!disable)
        {
            RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Lobby_Costume, 0, eFaceAnimation.Costume, 0);
        }
        else
        {
            var panel = LobbyUIManager.Instance.GetActiveUI<UICharInfoPanel>("CharInfoPanel");
            if (panel != null)
            {
                if (panel.CharinfoTab == UICharInfoPanel.eCHARINFOTAB.WEAPON)
                {
                    if (0 < chardata.EquipWeaponUID && RenderTargetChar.Instance.RenderPlayer.aniEvent.curAniType != eAnimation.Lobby_Weapon)
                    {
						panel.Renewal( true );
						RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Lobby_Weapon, 0, eFaceAnimation.Weapon, 0);
                    }
                }
                else
                    RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Idle01, 0, eFaceAnimation.Idle01, 0);
            }
        }

        chardata.EquipCostumeID = costumeid;
        chardata.CostumeStateFlag = costumestateflag;
        chardata.CostumeColor = costumecolor;

        _costumecolormax = costumetabledata.ColorCnt;

        if (costumetabledata.HairModel != string.Empty || costumetabledata.HairModel != "")
            _usecostumehair = true;

        if ( RenderTargetChar.Instance.RenderPlayer.costumeUnit.CostumeBody.kAttachList.Count != 0 )
            _usecostumeattach = true;

        if(RenderTargetChar.Instance.RenderPlayer.costumeUnit.CostumeBody.AttachOtherParts)
        {
            mUseCostumeOtherParts = true;
        }

        Renewal(true);
    }

    /// <summary>
    ///  19/03/12 윤주석
    ///  코스튬 선택으로 넘어온 경우 
    ///  보이지 않는 스크롤 아이템을 보여주게 하기 위한 함수
    /// </summary>
    /// <param name="number"></param>
    public void SetSpringScroll(int number)
    {
        _CostumeListInstance.SpringSetFocus(number, 0.5f);
    }

    public int CompareFuncOrderNum(GameTable.Costume.Param a, GameTable.Costume.Param b)
    {
        return a.OrderNum.CompareTo(b.OrderNum);

        //if (a.OrderNum < b.OrderNum) return -1; 
        //if (a.OrderNum > b.OrderNum) return 1;
        //return 0;
    }

	public int GetCostumeStateFlag()
	{
		uint flag = 0;

		GameSupport._DoOnOffBitIdx(ref flag, (int)eCostumeStateFlag.CSF_HAIR, _costumehair);
		GameSupport._DoOnOffBitIdx(ref flag, (int)eCostumeStateFlag.CSF_ATTACH_1, _costumeattach);
		GameSupport._DoOnOffBitIdx(ref flag, (int)eCostumeStateFlag.CSF_ATTACH_2, mCostumeOtherParts);

		return (int)flag;
	}
}
