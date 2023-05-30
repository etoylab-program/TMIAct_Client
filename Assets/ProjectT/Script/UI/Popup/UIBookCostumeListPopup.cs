using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBookCostumeListPopup : FComponent
{
	public UILabel kHaveLabel;
	public UILabel kHaveLimitedLabel;
	public UITexture kCharTex;
	public UISprite kDyeSpr;
	public UILabel kCostumeNameLabel;
	[SerializeField] private FList _BookCostumeListInstance;
	public UISprite kicoSpr;
	public GameObject kCostumeInfo;
	public FLocalizeText kCostumeInfoLabel;
	public UISprite kCostumeInfoSpr;
	public GameObject kSetCostume;

	public GameObject kBuyBtn;
	public UILabel kBuySaleValueLabel;
	public UIGoodsUnit kBuyGoodsUnit;

	public GameObject kNotBuyObjs;
	public UILabel kNotBuyDesc;

	public UIButton kHairBtn;
	public UISprite kHairOn;
	public UISprite kHairOff;
	public UIButton kColorBtn;
	public UILabel kColorCountLabel;
	public UIButton kAttachBtn;
	public UISprite kAttachOn;
	public UISprite kAttachOff;
	public UIAcquisitionInfoUnit kAcquisitionInfoUnit;

	private int _selectID = 0;
	private List<GameClientTable.Book.Param> _costumeList = new List<GameClientTable.Book.Param>();
	private GameTable.Costume.Param _costumeData;

	private bool _usecostumehair = false;
	private bool _usecostumeattach = false;
	private int _costumecolormax;
	private int _costumecolor;
	private bool _costumeattach;
	private bool _costumehair;

	private bool mUseCostumeOtherParts = false;
	private bool mCostumeOtherParts = true;

	private GameClientTable.StoreDisplayGoods.Param _sendstoredisplaytable;
	private eFilterFlag _charFilter = eFilterFlag.ALL;
	public override void Awake()
	{
		base.Awake();

		if(this._BookCostumeListInstance == null) return;
		
		this._BookCostumeListInstance.EventUpdate = this._UpdateBookCostumeListSlot;
		this._BookCostumeListInstance.EventGetItemCount = this._GetBookCostumeElementCount;
		this._BookCostumeListInstance.InitBottomFixing();
	}
 
	public override void OnEnable()
	{
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
		SoundManager.Instance.StopVoice();
		RenderTargetChar.Instance.DestroyRenderTarget();
	}

    public override void InitComponent()
	{
		_charFilter = eFilterFlag.ALL;

		_costumeList.Clear();
		_costumeList = GameInfo.Instance.GameClientTable.FindAllBook(x => x.Group == (int)eBookGroup.Costume);

		_selectID = _costumeList[(int)eCOUNT.NONE].ItemID;
		RenderTargetChar.Instance.gameObject.SetActive(true);
		SetCostumeChar();
		_BookCostumeListInstance.UpdateList();

		kHaveLimitedLabel.textlocalize = string.Format("/{0:#,##0}", _costumeList.Count);

		UIItemFilterPopup filterPopup = LobbyUIManager.Instance.GetUI<UIItemFilterPopup>("ItemFilterPopup");
		if (filterPopup != null)
		{
			filterPopup.DefailtTab = true;
		}
	}
 
	public override void Renewal(bool bChildren)
	{
        base.Renewal(bChildren);

		int now = GameInfo.Instance.CostumeList.Count;

		kHaveLabel.textlocalize = string.Format("{0:#,##0}", now);

		kBuyBtn.gameObject.SetActive(false);
		kBuySaleValueLabel.transform.parent.gameObject.SetActive(false);
		kNotBuyObjs.gameObject.SetActive(false);
		kAcquisitionInfoUnit.gameObject.SetActive(false);
		kDyeSpr.SetActive(_costumeData.UseDyeing != (int)eCOUNT.NONE);

		if (!GameInfo.Instance.HasCostume(_selectID))
		{
			if (!LobbyUIManager.Instance.IsShowCostumeSlot(_costumeData))
			{
				GameClientTable.Book.Param costumeBookData = GameInfo.Instance.GameClientTable.FindBook(x => x.ItemID == _selectID && x.Group == (int)eBookGroup.Costume);
				if (costumeBookData != null)
				{
					if (costumeBookData.AcquisitionID > (int)eCOUNT.NONE)
					{
						kAcquisitionInfoUnit.gameObject.SetActive(true);
						kAcquisitionInfoUnit.UpdateSlot(costumeBookData.AcquisitionID);
					}
					else
					{
						//구매 불가
						kNotBuyObjs.SetActive(true);
						kNotBuyDesc.textlocalize = FLocalizeString.Instance.GetText(costumeBookData.Desc);
					}
				}
			}
			else
			{
				var storetable = GameInfo.Instance.GameTable.FindStore(_costumeData.StoreID);
				kBuyGoodsUnit.gameObject.SetActive(true);
				kBuyGoodsUnit.InitGoodsUnit((eGOODSTYPE)storetable.PurchaseIndex, storetable.PurchaseValue);

				kBuyBtn.gameObject.SetActive(true);
				var storesaledata = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == _costumeData.StoreID);
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
		_BookCostumeListInstance.RefreshNotMove();
	}

	private void SetCostumeChar()
	{
		_costumeData = GameInfo.Instance.GameTable.FindCostume(x => x.ID == _selectID);
		if (_costumeData == null)
			return;

		_usecostumehair = false;
		_usecostumeattach = false;

		_costumehair = false;
		if (!string.IsNullOrEmpty(_costumeData.HairModel) && _costumeData.SubHairChange == 1)
		{
			_costumehair = !_costumehair;
		}

		_costumecolor = 0;
		_costumecolormax = _costumeData.ColorCnt;
		mUseCostumeOtherParts = false;
		mCostumeOtherParts = false;

		RenderTargetChar.Instance.InitRenderTargetChar(_costumeData.CharacterID, -1, _selectID, eCharacterType.Character, true);
		RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Lobby_Costume, 0, eFaceAnimation.Costume, 0);

		if (_costumeData.HairModel != string.Empty || _costumeData.HairModel != "")
		{
			_usecostumehair = true;
			
		}

		if (RenderTargetChar.Instance.RenderPlayer.costumeUnit.CostumeBody.kAttachList.Count != 0)
			_usecostumeattach = true;

		if (RenderTargetChar.Instance.RenderPlayer.costumeUnit.CostumeBody.AttachOtherParts)
		{
			mUseCostumeOtherParts = true;
		}

		kCostumeNameLabel.textlocalize = FLocalizeString.Instance.GetText(_costumeData.Name);
		kCostumeNameLabel.color = GameInfo.Instance.GameConfig.CostumeGradeColor[_costumeData.Grade];
		CostumePropertyBtnRenewal();
	}
	
	private void _UpdateBookCostumeListSlot(int index, GameObject slotObject)
	{
		do
		{
			UICostumeListSlot slot = slotObject.GetComponent<UICostumeListSlot>();
			if (null == slot) break;

			GameClientTable.Book.Param data = null;
			if (0 <= index && _costumeList.Count > index)
				data = _costumeList[index];

			slot.ParentGO = this.gameObject;
			slot.UpdateSlot(_selectID, index, data);

		}while(false);
	}
	
	private int _GetBookCostumeElementCount()
	{
		return _costumeList.Count;
	}

	public void OnClick_CostumeListSlot(int selectId)
	{
		if (_selectID.Equals(selectId))
			return;

		_selectID = selectId;
		SetCostumeChar();
		Renewal(false);
	}
	
	public void OnClick_BackBtn()
	{
		OnClickClose();
	}
	
	public void OnClick_CostumeBuyBtn()
	{
		if (_costumeData == null)
			return;

		CharData chardata = GameInfo.Instance.GetCharDataByTableID(_costumeData.CharacterID);
		if (chardata == null)
		{
			MessagePopup.OKCANCEL(eTEXTID.OK,
									  3051,
									  () => {
										  LobbyUIManager.Instance.HideUI("BookMainPopup", false);

										  UIStorePanel storePanel = LobbyUIManager.Instance.GetUI<UIStorePanel>("StorePanel");
										  if (storePanel != null)
                                          {
											  storePanel.DirectShow(UIStorePanel.eStoreTabType.STORE_CHAR, _costumeData.CharacterID);
                                          }
										  LobbyUIManager.Instance.SetPanelType(ePANELTYPE.STORE);
										  OnClickClose();
									  },
									  null);
		}
		else
		{
			GameClientTable.StoreDisplayGoods.Param data = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == _costumeData.StoreID);
			if (data == null)
				return;

			_sendstoredisplaytable = data;
			StoreBuyPopup.Show(_sendstoredisplaytable, ePOPUPGOODSTYPE.BASE_NONE_BTN, OnMsg_Purchase, OnMsg_Purchase_Cancel);
		}
	}

	public void OnMsg_Purchase()
	{
		GameTable.Store.Param storetabledata = GameInfo.Instance.GameTable.FindStore(x => x.ID == _sendstoredisplaytable.StoreID);
		if (storetabledata == null)
		{
			MessageToastPopup.Show(FLocalizeString.Instance.GetText(3009));
			return;
		}

		if ((eGOODSTYPE)storetabledata.PurchaseIndex != eGOODSTYPE.NONE && (eGOODSTYPE)storetabledata.PurchaseIndex != eGOODSTYPE.GOODS)
		{
			if (!GameSupport.IsCheckGoods((eGOODSTYPE)storetabledata.PurchaseIndex, storetabledata.PurchaseValue))
				return;
		}

		GameInfo.Instance.Send_ReqStorePurchase(_sendstoredisplaytable.StoreID, false, 1, OnNetPurchase);
	}

	public void OnNetPurchase(int result, PktMsgType pktmsg)
	{
		LobbyUIManager.Instance.Renewal("TopPanel");
		LobbyUIManager.Instance.Renewal("GoodsPopup");

		LobbyUIManager.Instance.Renewal("CharInfoPanel");

		string productName = FLocalizeString.Instance.GetText(_costumeData.Name);
		string str = FLocalizeString.Instance.GetText(3049, productName);

		MessageToastPopup.Show(str, true);

		Renewal(true);
	}

	public void OnMsg_Purchase_Cancel()
	{

	}

	public void OnClick_CostumeViewBtn()
	{
		if (kCharTex.GetComponent<TweenPosition>() != null)
		{
			if (kCharTex.GetComponent<TweenPosition>().enabled)
				return;
		}

		CharViewer.ShowCharPopup("BookCostumeListPopup", kCharTex.gameObject, kCharTex.transform.parent);
	}
	
	public void OnClick_HairBtn()
	{
		Log.Show("OnClick_HairBtn");

		if (!_usecostumehair)
			return;

		_costumehair = !_costumehair;

		uint costumestateflag = 0;
		GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_HAIR, _costumehair);
		GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_1, _costumeattach);
		GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_2, mCostumeOtherParts);
		RenderTargetChar.Instance.SetCostumeBody(_selectID, _costumecolor, (int)costumestateflag, null);

		CostumePropertyBtnRenewal();
	}
	
	public void OnClick_AttachBtn()
	{
		Log.Show("OnClick_AttachBtn");

		if (!_usecostumeattach)
			return;

		_costumeattach = !_costumeattach;

		uint costumestateflag = 0;
		GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_HAIR, _costumehair);
		GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_1, _costumeattach);
		GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_2, mCostumeOtherParts);
		RenderTargetChar.Instance.SetCostumeBody(_selectID, _costumecolor, (int)costumestateflag, null);

		CostumePropertyBtnRenewal();
	}
	
	public void OnClick_ColorBtn()
	{
		Log.Show("OnClick_ColorBtn");

		_costumecolor += 1;
		int costumeColorMax = _costumecolormax;

		//GameTable.Costume.Param costume = GameInfo.Instance.GameTable.FindCostume(_seletecostumeid);
		//if (costume != null)
		//	costumeColorMax = _costumecolormax + costume.UseDyeing;

		if (_costumecolor >= costumeColorMax)
			_costumecolor = 0;

		uint costumestateflag = 0;
		GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_HAIR, _costumehair);
		GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_1, _costumeattach);
		GameSupport._DoOnOffBitIdx(ref costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_2, mCostumeOtherParts);
		RenderTargetChar.Instance.SetCostumeBody(_selectID, _costumecolor, (int)costumestateflag, null);

		CostumePropertyBtnRenewal();
	}

	private void CostumePropertyBtnRenewal()
	{
		if (_usecostumeattach)
		{
			kAttachBtn.gameObject.SetActive(true);
			if (_costumeattach)
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

		if (string.IsNullOrEmpty(_costumeData.HairModel))
			kHairBtn.gameObject.SetActive(false);
		else
		{
			kHairBtn.gameObject.SetActive(true);
			kHairOn.SetActive(_costumehair);
			kHairOff.SetActive(!_costumehair);
		}
			

		if (_costumeData.ColorCnt == 1)
			kColorBtn.gameObject.SetActive(false);
		else
			kColorBtn.gameObject.SetActive(true);

		kColorCountLabel.textlocalize = (_costumecolor + 1).ToString();

	}

	public void OnClick_FilterBtn()
	{
		UIValue.Instance.SetValue(UIValue.EParamType.FilterOpenUI, UIItemFilterPopup.eFilterOpenUI.BookCostumeListPopup.ToString());
		//  추후 정렬 필터로 추가됩니다.
		LobbyUIManager.Instance.ShowUI("ItemFilterPopup", true);
	}

	public void SetFilterWeapon(eFilterFlag charFilter)
	{
		if (_charFilter == charFilter)
		{
			return;
		}

		_charFilter = charFilter;

		_costumeList.Clear();

		if (_charFilter == eFilterFlag.ALL)
		{
			_costumeList  = GameInfo.Instance.GameClientTable.FindAllBook(x => x.Group == (int)eBookGroup.Costume);
		}
		else
		{
			//글로벌은 캐릭터 출시가 달라서 별도로 characterId 설정
			List<int> charID = new List<int>();
			for (int i = 0; i < GameInfo.Instance.GameTable.Characters.Count; i++)
			{
				charID.Add(GameInfo.Instance.GameTable.Characters[i].ID);
			}

			List<GameClientTable.Book.Param> costumeDummy = GameInfo.Instance.GameClientTable.FindAllBook(x => x.Group == (int)eBookGroup.Costume);
			for (int i = 0; i < costumeDummy.Count; i++)
			{
				GameTable.Costume.Param costumedata = GameInfo.Instance.GameTable.FindCostume(x => x.ID == costumeDummy[i].ItemID && ((_charFilter & (eFilterFlag)(1 << charID.FindIndex(y => y == x.CharacterID))) == (eFilterFlag)(1 << charID.FindIndex(y => y == x.CharacterID))));
				if (costumedata != null)
				{
					_costumeList.Add(costumeDummy[i]);
				}
			}
		}
		_BookCostumeListInstance.UpdateList();
		OnClick_CostumeListSlot(_costumeList[(int)eCOUNT.NONE].ItemID);
	}

	public override void OnClickClose()
    {
		NotificationManager.Instance.Init();
		LobbyUIManager.Instance.Renewal("BookMainPopup");
		base.OnClickClose();
    }
}
