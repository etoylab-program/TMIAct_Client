using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGachaDetailPopup : FComponent
{    
    public class GachaDetailData
    {
        public int Grade;
        public string Name;
        public int Rate;
        public string Percent;
        public int ProductType;
        public int ProductIndex;
        public int Value;

        public GachaDetailData(int grade, string name, int rate, int type, int index, int value)
        {
            Grade = grade;
            Name = name;
            Rate = rate;
            ProductType = type;
            ProductIndex = index;
            Value = value;
        }

        static public int CompareFuncGrade(GachaDetailData a, GachaDetailData b)
        {
            if (a.Grade < b.Grade) return 1;
            if (a.Grade > b.Grade) return -1;
            return 0;
        }
    }

    public List<UILabel> kRateLabelList;
    public List<UISprite> kRatSprList;
    [SerializeField] private FList _CardListInstance;
    private List<GachaDetailData> _gachadetaillist = new List<GachaDetailData>();
    private int[] _gradelist = new int[(int)eGRADE.COUNT];
    private int _totalrate = 0;

    //public List<GameClientTable.GachaCard.Param> CardTableList { get { return _cardtablelist; } }
    public override void Awake()
	{
		base.Awake();

		if(this._CardListInstance == null) return;
		
		this._CardListInstance.EventUpdate = this._UpdateCardListSlot;
		this._CardListInstance.EventGetItemCount = this._GetCardElementCount;
        this._CardListInstance.InitBottomFixing();

    }
 
	public override void OnEnable()
	{
        InitComponent();

        base.OnEnable();
	}

    public override void InitComponent()
    {
        object obj = UIValue.Instance.GetValue(UIValue.EParamType.GachaDetailStoreID);
        if (obj == null)
            return;
        var storetable = GameInfo.Instance.GameTable.FindStore((int)obj);
        if (storetable == null)
            return;

        _gachadetaillist.Clear();
        for (int i = 0; i < _gradelist.Length; i++)
            _gradelist[i] = 0;

        var list = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == storetable.ProductIndex);
        _totalrate = 0;
        for( int i = 0; i < list.Count; i++ )
        {
            var data = list[i];
            if(data.ProductType == (int)eREWARDTYPE.WEAPON)
            {                
                var tabledata = GameInfo.Instance.GameTable.FindWeapon(data.ProductIndex);
                if (tabledata != null)
                    _gachadetaillist.Add(new GachaDetailData(tabledata.Grade, string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.REWARDTYPE + data.ProductType), FLocalizeString.Instance.GetText(tabledata.Name)), data.Prob, data.ProductType, data.ProductIndex, data.Value));
            }
            else if (data.ProductType == (int)eREWARDTYPE.GEM)
            {
                var tabledata = GameInfo.Instance.GameTable.FindGem(data.ProductIndex);
                if (tabledata != null)
                    _gachadetaillist.Add(new GachaDetailData(tabledata.Grade, string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.REWARDTYPE + data.ProductType), FLocalizeString.Instance.GetText(tabledata.Name)), data.Prob, data.ProductType, data.ProductIndex, data.Value));
            }
            else if (data.ProductType == (int)eREWARDTYPE.CARD)
            {
                var tabledata = GameInfo.Instance.GameTable.FindCard(data.ProductIndex);
                if (tabledata != null)
                    _gachadetaillist.Add(new GachaDetailData(tabledata.Grade, string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.REWARDTYPE + data.ProductType), FLocalizeString.Instance.GetText(tabledata.Name)), data.Prob, data.ProductType, data.ProductIndex, data.Value));
            }
            else if (data.ProductType == (int)eREWARDTYPE.ITEM)
            {
                var tabledata = GameInfo.Instance.GameTable.FindItem(data.ProductIndex);
                if (tabledata != null)
                    _gachadetaillist.Add(new GachaDetailData(tabledata.Grade, string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.REWARDTYPE + data.ProductType), FLocalizeString.Instance.GetText(tabledata.Name)), data.Prob, data.ProductType, data.ProductIndex, data.Value));
            }
            _totalrate += data.Prob;
        }
        
        for ( int i = 0; i < _gachadetaillist.Count; i++)
        {
            float selectPersent = (_gachadetaillist[i].Rate / (float)_totalrate) * 100.0f;
            _gachadetaillist[i].Percent = FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT, selectPersent);
            _gradelist[_gachadetaillist[i].Grade] += _gachadetaillist[i].Rate;
        }

        //_gachadetaillist.Sort(GachaDetailData.CompareFuncGrade);
        
        this._CardListInstance.UpdateList();
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        for (int i = 0; i < kRateLabelList.Count; i++)
        {
            kRateLabelList[i].textlocalize = "0.0%";
            kRateLabelList[i].alpha = 0.15f;
            kRatSprList[i].alpha = 0.15f;
        }

        for (int i = 0; i < kRateLabelList.Count; i++)
        {
            float selectPersent = (_gradelist[i+1] / (float)_totalrate) * 100.0f;
            kRateLabelList[i].textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONE_POINT_TEXT, selectPersent);
            if (selectPersent > 0.0f)
            {
                kRateLabelList[i].alpha = 1.0f;
                kRatSprList[i].alpha = 1.0f;
            }
        }
    }
 	
	private void _UpdateCardListSlot(int index, GameObject slotObject)
	{
        do
        {
            UIGachaDetailListSlot card = slotObject.GetComponent<UIGachaDetailListSlot>();
            if (null == card) break;
            card.ParentGO = this.gameObject;

            GachaDetailData data = null;
            if (0 <= index && _gachadetaillist.Count > index)
                data = _gachadetaillist[index];

            card.UpdateSlot(index, data);
        } while (false);
    }
	
	private int _GetCardElementCount()
	{
        return _gachadetaillist.Count;

    }
}
