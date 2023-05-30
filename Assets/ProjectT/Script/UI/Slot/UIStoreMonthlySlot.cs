using UnityEngine;
using System.Collections;

public class UIStoreMonthlySlot : FSlot {
 

	public UISprite kbgSpr;
	public UITexture kTex;
	public FLocalizeText kInstantlyDesc;
	public FLocalizeText kDailyDesc;
	public FLocalizeText kTotalDesc;
 
	private GameClientTable.StoreDisplayGoods.Param _storeDisplayGoodsData;

	public void UpdateSlot(GameClientTable.StoreDisplayGoods.Param storedisplaygoods) 	//Fill parameter if you need
	{
		_storeDisplayGoodsData = storedisplaygoods;
		if (_storeDisplayGoodsData == null)
			return;

		GameTable.Store.Param storeTableData = GameInfo.Instance.GameTable.FindStore(x => x.ID == _storeDisplayGoodsData.StoreID);
		if (storeTableData == null)
			return;			

		//월 정액 상품 데이터
		GameTable.MonthlyFee.Param monthlyFee = GameInfo.Instance.GameTable.FindMonthlyFee(x => x.MonthlyFeeID == storeTableData.ProductIndex);
		if (monthlyFee == null)
			return;

		GameTable.Random.Param instantlyRandom = GameInfo.Instance.GameTable.FindRandom(x => x.GroupID == monthlyFee.RewardID);
		GameTable.Random.Param dailyRandom = GameInfo.Instance.GameTable.FindRandom(x => x.GroupID == monthlyFee.D_RewardID);
		
		int instantly_Value = instantlyRandom.ProductValue;			//즉시 획득량
		int daily_Value = dailyRandom.ProductValue;					//일일 획득량
		int total_Value = instantly_Value + (daily_Value * GameInfo.Instance.GameConfig.MonthlyFeeTotalDay);        //총 획득량

		//kTex.mainTexture = (Texture2D)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/" + _storeDisplayGoodsData.Icon);
		GameSupport.LoadLocalizeTexture(kTex, "icon", "Icon/" + _storeDisplayGoodsData.Icon, _storeDisplayGoodsData.IconLocalize);

		kInstantlyDesc.SetLabel(instantly_Value.ToString("#,##0"));
		kDailyDesc.SetLabel(daily_Value.ToString("#,##0"));
		kTotalDesc.SetLabel(total_Value.ToString("#,##0"));
		//GameTable.Random.Param instantlyRandom = 
	}
 
	public void OnClick_Slot()
	{
	}
 
}
