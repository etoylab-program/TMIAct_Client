using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharCardSeletePopup : FComponent
{
    public UIStatusUnit kCharStatusUnit_HP;
    public UIStatusUnit kCharStatusUnit_DEF;
    public UISprite kGradeSpr;
	public UILabel kNameLabel;
    public UILabel kLevelLabel;
    public UISprite kWakeSpr;
    public GameObject kFavor;
    public UILabel kFavorLevelLabel;
    public UISprite kFavorGaugeSpr;
    public UILabel kSlotDescLabel;
    public UISprite kSlotSpr;
    public UISprite kTypeSpr;
    public UILabel kTypeLabel;
    public UILabel kSkillNameLabel;
	public UILabel kSkillLevelLabel;
	public UILabel kSkillDesceLabel;
    public GameObject kMainSkill;
    public UILabel kMainSkillNameLabel;
    public UILabel kMainSkillLevelLabel;
    public UILabel kMainSkillTimeLabel;
    public UILabel kMainSkillDesceLabel;

    public UIButton kChangeBtn;
    public UIButton kRemoveBtn;
    public UIStatusUnit kSupporterStatusUnit_HP;
    public UIStatusUnit kSupporterStatusUnit_DEF;
    [SerializeField] private FList _ItemListInstance;
    public FTab kFilterTab;
    private int _filterType = (int)eCOUNT.NONE;

    private List<CardData> _cardlist = new List<CardData>();
    private CharData _chardata;
    private CardData _carddata;
    private CardBookData _cardbookdata;
    private int[] _statusAry = new int[(int)eCHARABILITY._MAX_];
    private int _seleteslot = 0;
    public long SeleteCardUID
    {
        get
        {
            if (_carddata == null)
                return -1;
            return _carddata.CardUID;
        }
    }

    public override void Awake()
	{
		base.Awake();

		if(this._ItemListInstance == null) return;
		
		this._ItemListInstance.EventUpdate = this._UpdateItemListSlot;
		this._ItemListInstance.EventGetItemCount = this._GetItemElementCount;
        this._ItemListInstance.InitBottomFixing();

        kFilterTab.EventCallBack = OnFilterTabSelect;
    }
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        _chardata = null;
        
        _seleteslot = (int)UIValue.Instance.GetValue(UIValue.EParamType.CharEquipCardSlot);

        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
        _chardata = GameInfo.Instance.GetCharData(uid);

        _filterType = (int)eCOUNT.NONE;
        kFilterTab.SetTab(_filterType, SelectEvent.Enable);

        SetCardList();
    }

    private void SetCardList()
    {
        _cardlist.Clear();
        for (int i = 0; i < GameInfo.Instance.CardList.Count; i++)
        {
            if (GameSupport.IsEquipAndUsingCardData(GameInfo.Instance.CardList[i].CardUID))
                continue;

            if (_chardata.EquipCard[(int)eCARDSLOT.SLOT_MAIN] != GameInfo.Instance.CardList[i].CardUID &&
                _chardata.EquipCard[(int)eCARDSLOT.SLOT_SUB1] != GameInfo.Instance.CardList[i].CardUID &&
                _chardata.EquipCard[(int)eCARDSLOT.SLOT_SUB2] != GameInfo.Instance.CardList[i].CardUID)
            {
                if (_filterType == (int)eCOUNT.NONE)
                    _cardlist.Add(GameInfo.Instance.CardList[i]);
                else
                {
                    //if(GameInfo.Instance.CardList[i].TableData.Type == _filterType)
                    //    _cardlist.Add(GameInfo.Instance.CardList[i]);

                    if (GameInfo.Instance.CardList[i].Type == _filterType)
                        _cardlist.Add(GameInfo.Instance.CardList[i]);
                }
            }
        }

        CardData.SortUp = false;
        _cardlist.Sort(CardData.CompareFuncGradeLevel);

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            if (_seleteslot != i)
                if (_chardata.EquipCard[i] != (int)eCOUNT.NONE)
                {
                    if (_filterType == (int)eCOUNT.NONE)
                        _cardlist.Insert(0, GameInfo.Instance.GetCardData(_chardata.EquipCard[i]));
                    else
                    {
                        CardData equipCard = GameInfo.Instance.GetCardData(_chardata.EquipCard[i]);
                        if(equipCard.Type == _filterType)
                            _cardlist.Insert(0, equipCard);
                    }
                }
                    
        }

        if (_chardata.EquipCard[_seleteslot] != (int)eCOUNT.NONE)
        {
            if (_filterType == (int)eCOUNT.NONE)
                _cardlist.Insert(0, GameInfo.Instance.GetCardData(_chardata.EquipCard[_seleteslot]));
            else
            {
                CardData equipCard = GameInfo.Instance.GetCardData(_chardata.EquipCard[_seleteslot]);
                if (equipCard.Type == _filterType)
                    _cardlist.Insert(0, equipCard);
            }
        }

        _carddata = null;
        if (_cardlist.Count != 0)
        {
            _carddata = _cardlist[0];
            _cardbookdata = GameInfo.Instance.GetCardBookData(_carddata.TableID);
        }

        for (int i = 0; i < (int)eCHARABILITY._MAX_; i++)
            _statusAry[i] = 0;

        GameSupport.GetCardTotalStat(_chardata.EquipCard, ref _statusAry);

        _ItemListInstance.UpdateList();
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        if (_carddata == null)
            return;

        long[] nowEquipCard = new long[(int)eCOUNT.CARDSLOT];
        int[] nowStatusAry = new int[(int)eCHARABILITY._MAX_];
        int oldSlot = -1;

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            // 이미 배치 중인 서포터면 해당 슬롯 번호 임시 저장
            if (_chardata.EquipCard[i] == _carddata.CardUID)
                oldSlot = i;
            nowEquipCard[i] = _chardata.EquipCard[i];
        }
        if (oldSlot >= 0)
        {
            nowEquipCard[oldSlot] = (int)eCOUNT.NONE;
            if (_chardata.EquipCard[_seleteslot] != (int)eCOUNT.NONE && _chardata.EquipCard[_seleteslot] != _carddata.CardUID)
                nowEquipCard[oldSlot] = _chardata.EquipCard[_seleteslot];
        }
        nowEquipCard[_seleteslot] = _carddata.CardUID;

        GameSupport.GetCardTotalStat(nowEquipCard, ref nowStatusAry);

        kCharStatusUnit_HP.InitStatusUnit((int)eTEXTID.STAT_HP, _statusAry[(int)eCHARABILITY.HP], nowStatusAry[(int)eCHARABILITY.HP]);
        kCharStatusUnit_DEF.InitStatusUnit((int)eTEXTID.STAT_DEF, _statusAry[(int)eCHARABILITY.DEF], nowStatusAry[(int)eCHARABILITY.DEF]);
        
        if (_chardata.EquipCard[_seleteslot] == _carddata.CardUID)
        {
            kChangeBtn.gameObject.SetActive(false);
            kRemoveBtn.gameObject.SetActive(true);
        }
        else
        {
            kChangeBtn.gameObject.SetActive(true);
            kRemoveBtn.gameObject.SetActive(false);
        }

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_carddata.TableData.Name);
        kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _carddata.Level, GameSupport.GetMaxLevelCard(_carddata));
        

        kGradeSpr.spriteName = "itemgrade_L_" + _carddata.TableData.Grade.ToString();
        //kGradeSpr.MakePixelPerfect();

        kWakeSpr.spriteName = "itemwake_0" + _carddata.Wake.ToString();
        kWakeSpr.MakePixelPerfect();

        kSupporterStatusUnit_HP.InitStatusUnit((int)eTEXTID.STAT_HP, _carddata.GetCardHP());
        kSupporterStatusUnit_DEF.InitStatusUnit((int)eTEXTID.STAT_DEF, _carddata.GetCardDEF());

        if (_seleteslot == (int)eCARDSLOT.SLOT_MAIN)
            kSlotDescLabel.textlocalize = FLocalizeString.Instance.GetText(_carddata.TableData.Name);
        else
            kSlotDescLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(283), FLocalizeString.Instance.GetText(_carddata.TableData.Name), _carddata.SkillLv, GameInfo.Instance.GameConfig.CardSubSlotStatBySLV[_carddata.SkillLv] * (float)eCOUNT.MAX_BO_FUNC_VALUE);
        kSlotSpr.spriteName = "ico_SupporterSet_" + _seleteslot.ToString();

        kTypeSpr.spriteName = "SupporterType_" + _carddata.Type.ToString();
        kTypeLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.CARDTYPE + _carddata.Type);

        kSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(_carddata.TableData.SkillEffectName);
        kSkillDesceLabel.textlocalize = GameSupport.GetCardSubSkillDesc(_carddata.TableData, _carddata.SkillLv);
        kSkillLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.SKILL_LEVEL_TXT_NOW_LV, _carddata.SkillLv, GameSupport.GetMaxSkillLevelCard());
        kSkillLevelLabel.transform.localPosition = new Vector3(kSkillNameLabel.transform.localPosition.x + kSkillNameLabel.printedSize.x + 10, kSkillLevelLabel.transform.localPosition.y, 0);

        if (_carddata.TableData.MainSkillEffectName > 0)
        {
            kMainSkill.SetActive(true);
            kMainSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(_carddata.TableData.MainSkillEffectName);
            kMainSkillDesceLabel.textlocalize = GameSupport.GetCardMainSkillDesc(_carddata.TableData, _carddata.Wake);
            if (_carddata.Wake == 0)
                kMainSkillLevelLabel.textlocalize = "";
            else
                kMainSkillLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_NOW_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PLUS_TEXT), _carddata.Wake));
            kMainSkillLevelLabel.transform.localPosition = new Vector3(kMainSkillNameLabel.transform.localPosition.x + kMainSkillNameLabel.printedSize.x + 10, kMainSkillNameLabel.transform.localPosition.y, 0);

            if (_carddata.TableData.CoolTime == 0)
            {
                kMainSkillTimeLabel.gameObject.SetActive(false);
            }
            else
            {
                kMainSkillTimeLabel.gameObject.SetActive(true);
                kMainSkillTimeLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(263), _carddata.TableData.CoolTime);
            }
        }
        else
            kMainSkill.SetActive(false);

        var cardbookdata = GameInfo.Instance.GetCardBookData(_carddata.TableID);
        if (cardbookdata != null)
        {
            kFavor.SetActive(true);
            kFavorGaugeSpr.fillAmount = GameSupport.GetCardFavorLevelExpGauge(cardbookdata.TableID, cardbookdata.FavorLevel, cardbookdata.FavorExp);
            kFavorLevelLabel.textlocalize = cardbookdata.FavorLevel.ToString();
        }
        else
        {
            kFavor.SetActive(false);
        }

        

        _ItemListInstance.RefreshNotMove();
    }
 	
	private void _UpdateItemListSlot(int index, GameObject slotObject)
	{
        do
        {
            UIItemListSlot card = slotObject.GetComponent<UIItemListSlot>();
            if (null == card) break;

            CardData data = null;
            if (0 <= index && _cardlist.Count > index)
            {
                data = _cardlist[index];
            }

            card.ParentGO = this.gameObject;
            card.UpdateSlot(UIItemListSlot.ePosType.Card_SeleteList, index, data);
        } while (false);
    }
	
	private int _GetItemElementCount()
	{
        return _cardlist.Count;
    }

    public void SetSeleteCardUID(long uid)
    {
        var carddata = GameInfo.Instance.GetCardData(uid);
        if (carddata == null)
            return;
       
        _carddata = carddata;
        _cardbookdata = GameInfo.Instance.GetCardBookData(_carddata.TableID);
        Renewal(true);
    }

    public void OnClick_BackBtn()
	{
        OnClickClose();
	}

    public void OnClick_ChangeBtn()
	{
        if (_carddata == null)
            return;

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            CardData tempCard = GameInfo.Instance.GetCardData(_chardata.EquipCard[i]);
            if (tempCard != null)
            {
                if (tempCard.TableID == _carddata.TableID && tempCard.CardUID != _carddata.CardUID && i != _seleteslot)
                {
                    MessageToastPopup.Show(FLocalizeString.Instance.GetText(3121));
                    return;
                }
            }
        }
        if (!_chardata.IsEquipCard(_carddata.CardUID))
        {
            CharData chardata = GameInfo.Instance.GetEquiCardCharData(_carddata.CardUID);
            if (chardata != null)
            {
                MessagePopup.CYN(eTEXTID.TITLE_NOTICE, string.Format(FLocalizeString.Instance.GetText(3116), FLocalizeString.Instance.GetText(chardata.TableData.Name), FLocalizeString.Instance.GetText(_chardata.TableData.Name)), eTEXTID.YES, eTEXTID.NO, OnMsg_ChangeCardPos);
                return;
            }
        }
        FacilityData facilitydata = GameInfo.Instance.GetEquiCardFacilityData(_carddata.CardUID);
        if( facilitydata != null )
        {
            if( facilitydata.Stats == (int)eFACILITYSTATS.WAIT )
            {
                MessagePopup.CYN(eTEXTID.TITLE_NOTICE, string.Format(FLocalizeString.Instance.GetText(3116), FLocalizeString.Instance.GetText(facilitydata.TableData.Name), FLocalizeString.Instance.GetText(_chardata.TableData.Name)), eTEXTID.YES, eTEXTID.NO, OnMsg_ChangeCardPos);
                return;
            }
            else
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3120));
                return;
            }
        }

        GameInfo.Instance.Send_ReqApplyPosCard(_carddata.CardUID, _chardata.CUID, _seleteslot, OnNetCardCharEquip);
    }

    public void OnClick_RemoveBtn()
    {
        if (_carddata == null)
            return;

        GameInfo.Instance.Send_ReqApplyOutPosCard(_carddata.CardUID, OnNetCardCharRemove);
    }

    public  void OnMsg_ChangeCardPos()
    {
        GameInfo.Instance.Send_ReqApplyPosCard(_carddata.CardUID, _chardata.CUID, _seleteslot, OnNetCardCharEquip);
    }

    public void OnNetCardCharEquip(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("CharInfoPanel");
        LobbyUIManager.Instance.Renewal("PresetPopup");

        OnClick_BackBtn();

        VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.Equip, _carddata.TableID);

        if (GameSupport.IsTutorial())
            GameSupport.TutorialNext();
    }

    public void OnNetCardCharRemove(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("CharInfoPanel");
        LobbyUIManager.Instance.Renewal("PresetPopup");

        OnClick_BackBtn();
    }

    private bool OnFilterTabSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
            return true;

        _filterType = nSelect;
        SetCardList();
        Renewal(true);

        return true;
    }
}
