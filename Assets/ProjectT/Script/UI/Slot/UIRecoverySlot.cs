using UnityEngine;
using System.Collections;

public class UIRecoverySlot : FSlot {
 

	public UILabel kExplanLabel;
	public UILabel kResultLabel;
	public GameObject kOff;
	public GameObject kOn;
	public UILabel kCountLabel;
	public UILabel kMaxLabel;
	public UILabel kQuantityLabel;
	public UIGoodsUnit kGoodsUnit;
	public UILabel kQuantityCountLabel;

	private eGOODSTYPE _goodsType = eGOODSTYPE.NONE;
	private bool _select = false;
	private bool _bCash = false;

	private GameTable.Store.Param _storeTableData = null;
	private GameTable.Item.Param _itemTableData = null;
	private int _nowValue;

	private int _maxValue = (int)eCOUNT.NONE;

	private int _addValue = (int)eCOUNT.NONE;
	private int _minValue = (int)eCOUNT.NONE;

	private int _currentIdx = (int)eCOUNT.NONE;

	private long _quantityCnt = (int)eCOUNT.NONE;

	public void SetSlot(eGOODSTYPE goodsType, int itemTableID, bool select, bool bCash = false)
	{
		_goodsType = goodsType;
		_select = select;
		_storeTableData = GameInfo.Instance.GameTable.FindStore(x => x.ID == itemTableID);
		if (null == _storeTableData)
			return;

		//_maxValue = GameInfo.Instance.GameConfig.MatCount;
		_maxValue = 1;

		_currentIdx = (int)eCOUNT.NONE + 1;
		UpdateSlot(select, bCash);
	}

	public void UpdateSlot(bool select, bool bCash = false) 	//Fill parameter if you need
	{
		_select = select;
		_bCash = bCash;

		if (_bCash)
		{
			kGoodsUnit.InitGoodsUnit(eGOODSTYPE.CASH, GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.CASH]);

			_quantityCnt = GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.CASH];

			_addValue = _storeTableData.ProductValue;

			kExplanLabel.textlocalize = FLocalizeString.Instance.GetText(1752, _goodsType.ToString(), _addValue);
			_maxValue = (int)GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.CASH] / _storeTableData.PurchaseValue;
		}
		else
		{
			_itemTableData = GameInfo.Instance.GameTable.FindItem(x => x.ID == _storeTableData.ProductIndex);
			if (null == _itemTableData)
				return;

			Texture tex = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Item/{0}", _itemTableData.Icon));

			kGoodsUnit.InitGoodsUnitTexture(tex);

			_quantityCnt = GameInfo.Instance.GetItemIDCount(_itemTableData.ID);

			_addValue = _goodsType == eGOODSTYPE.AP ? GameSupport.GetMaxAP() : (int)eCOUNT.NONE + 1;

			if(_goodsType == eGOODSTYPE.AP)
				kExplanLabel.textlocalize = FLocalizeString.Instance.GetText(1753, _goodsType.ToString());
			else
				kExplanLabel.textlocalize = FLocalizeString.Instance.GetText(1755, _goodsType.ToString(), (int)eCOUNT.NONE + 1);

			//_maxValue = (int)(_quantityCnt / _storeTableData.PurchaseValue);
			_maxValue = GameInfo.Instance.GetItemIDCount(_itemTableData.ID);
		}

		if (_maxValue > GameInfo.Instance.GameConfig.MatCount)
			_maxValue = GameInfo.Instance.GameConfig.MatCount;

		if (_maxValue <= (int)eCOUNT.NONE)
			_maxValue = 1;

		bool matFlag = MatCheck();

		if (!matFlag)
		{
			kQuantityCountLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR, _quantityCnt.ToString("#,##0"));
		}
		else
		{
			kQuantityCountLabel.textlocalize = _quantityCnt.ToString("#,##0");
		}
		

		kOff.SetActive(!_select);
		kOn.SetActive(_select);

		SetCountLabel();
	}
 
	public void OnClick_MinusBtn()
	{
		if (!_select)
			return;

		_currentIdx--;
		if (_currentIdx <= (int)eCOUNT.NONE)
			_currentIdx = (int)eCOUNT.NONE + 1;

		SetCountLabel();
	}
	
	public void OnClick_PlusBtn()
	{
		if (!_select)
			return;

		_currentIdx++;
		if (_currentIdx >= _maxValue)
			_currentIdx = _maxValue;

		SetCountLabel();
	}
	
	public void OnClick_MaxBtn()
	{
		if (!_select)
			return;

		_currentIdx = _maxValue;

		SetCountLabel();
	}

	private void SetCountLabel()
	{
		string apStr = (_goodsType == eGOODSTYPE.AP) ? FLocalizeString.Instance.GetText(244) : FLocalizeString.Instance.GetText(245);

		int addAp = _currentIdx * _addValue;
		long resultAp = GameInfo.Instance.UserData.Goods[(int)_goodsType] + addAp;

		int strColor = (int)eTEXTID.WHITE_TEXT_COLOR;

		if (_bCash)
		{
			if (!MatCheck())
				strColor = (int)eTEXTID.RED_TEXT_COLOR;

			kQuantityCountLabel.textlocalize = FLocalizeString.Instance.GetText(strColor, _quantityCnt.ToString("#,##0"));
			kCountLabel.textlocalize = FLocalizeString.Instance.GetText(213, FLocalizeString.Instance.GetText(strColor, _currentIdx * _storeTableData.PurchaseValue));

			kResultLabel.textlocalize = string.Format("{0} : {1}/{2}[+{3}]",
			apStr,
			FLocalizeString.Instance.GetText((int)eTEXTID.YELLOW_TEXT_COLOR, resultAp.ToString("#,##0")),
			_goodsType == eGOODSTYPE.AP ? GameSupport.GetMaxAP().ToString("#,##0") : GameInfo.Instance.GameConfig.BPMaxCount.ToString("#,##0"),
			addAp.ToString("#,##0"));
		}
		else
		{
			if (!MatCheck())
				strColor = (int)eTEXTID.RED_TEXT_COLOR;

			kQuantityCountLabel.textlocalize = FLocalizeString.Instance.GetText(strColor, _quantityCnt.ToString("#,##0"));
			kCountLabel.textlocalize = FLocalizeString.Instance.GetText(213, FLocalizeString.Instance.GetText(strColor, _currentIdx));

			kResultLabel.textlocalize = string.Format("{0} : {1}/{2}[+{3}]",
			apStr,
			FLocalizeString.Instance.GetText((int)eTEXTID.YELLOW_TEXT_COLOR, resultAp.ToString("#,##0")),
			_goodsType == eGOODSTYPE.AP ? GameSupport.GetMaxAP().ToString("#,##0") : GameInfo.Instance.GameConfig.BPMaxCount.ToString("#,##0"),
			addAp.ToString("#,##0"));
		}
	}

	public bool MatCheck()
	{
		bool result = false;

		if (_bCash)
		{
			if (_currentIdx * _storeTableData.PurchaseValue > GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.CASH])
				result = false;
			else
				result = true;
		}
		else
		{
			if (_maxValue <= (int)eCOUNT.NONE)
				result = false;
			else
			{
				if (_currentIdx > GameInfo.Instance.GetItemIDCount(_itemTableData.ID))
					result = false;
				else
					result = true;
			}
		}

		return result;
	}

	public int GetStoreTableID()
	{
		if (_storeTableData == null)
			return (int)eCOUNT.NONE;

		return _storeTableData.ID;
	}

	public int GetUseItemCount()
	{
		return _currentIdx;
	}

	public int GetRecoveryValue()
	{
		if (_bCash)
		{
			return _currentIdx * _storeTableData.ProductValue;
		}
		else
		{
			return _currentIdx * _addValue;
		}
	}

	public GameTable.Item.Param GetItemData()
	{
		return _itemTableData;
	}

	public GameTable.Store.Param GetStoreTableDataOrNull() {
		return _storeTableData;
	}
}
