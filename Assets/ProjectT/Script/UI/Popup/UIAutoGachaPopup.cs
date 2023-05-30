using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIAutoGachaPopup : FComponent {
    [Header("- UIAutoGachaPopup")]
    [SerializeField] private UILabel _GachaName;
    [SerializeField] private UILabel _GachaDesc;

    [SerializeField] private UITexture _ItemTex;
    [SerializeField] private UIGoodsUnit _GoodsUnit;
    [SerializeField] private UISprite _GoodsBGSpr;
    //Sale Objects
    [SerializeField] private GameObject _SaleObj;
    [SerializeField] private UIGoodsUnit _SaleGoodsUnit;
    [SerializeField] private UILabel _SaleDiscountValueLabel;

    [Header("- Select Gacha Count")]
    [SerializeField] private GameObject _SelectGachaCountObj;
    [SerializeField] private FToggle _SkipToggle;
    [SerializeField] private UILabel _MultiGachaCount;
    [SerializeField] private UIButton _MinusButton;
    [SerializeField] private UIButton _PlusButton;
    [SerializeField] private UISlider _Slider;

    //연출 스킵 여부 (최초 1회 이후)
    public bool IsSkipDirector { get; private set; }

    //매개변수로 받아오는 데이터
    private GameClientTable.StoreDisplayGoods.Param storeDisplayTable;
    private GameTable.Store.Param storeTable;
    private int mSaleValue;
    private bool mbSaleApply;
    private bool mbFree;
    private UnityAction mCallBackOK;

    //내부 사용 데이터
    private int mMultiCount;
    private int mMinCount;
    private int mMaxCount;
    private int mMaxSelectCount;

    private eREWARDTYPE mPurchaseType;
    private eGOODSTYPE mPurchaseIndex;
    private int mPurchaseValue;
    private int mAutoTotalPrice;

    public override void Awake() {
        base.Awake();

        _Slider.onChange.Add(new EventDelegate(OnChange_Slide));

        _SkipToggle.EventCallBack = OnToggleSkip;
    }

    /*로테이션 가챠 오픈 구매 - 작업은 했지만 안쓸 거 같아서 보류
    //사용법
        UIAutoGachaPopup autoGachaPopup = LobbyUIManager.Instance.GetUI<UIAutoGachaPopup>("AutoGachaPopup");
        if (autoGachaPopup == null) {
            Debug.LogError("오토 가챠 팝업 없음");
            return;
        }
        autoGachaPopup.SetData(FLocalizeString.Instance.GetText(1717), FLocalizeString.Instance.GetText(1718, t.Hours, t.Minutes), bisSale, gachaTable.OpenCash, openCash, OnCallBack_BuyPopup_PurchaseRotationGachaTime);
        autoGachaPopup.SetUIActive(true);


    public void SetData(string title, string desc, bool bSaleApply, int purchaseValue, int saleValue, UnityAction callBackOK) {
        //테이블
        storeDisplayTable = null;
        storeTable = null;

        //상품 세부 값 
        mSaleValue = saleValue;
        mbSaleApply = bSaleApply;
        mbFree = false;

        mCallBackOK = callBackOK;

        //구매 재화 타입
        mPurchaseType = eREWARDTYPE.GOODS;
        mPurchaseIndex = eGOODSTYPE.CASH;
        mPurchaseValue = purchaseValue;

        RefreshRotationGachaOpen(title, desc);
    }

    public void RefreshRotationGachaOpen(string _title, string _desc) {
        _GachaName.textlocalize = _title;
        _GachaDesc.textlocalize = _desc;

        _GoodsBGSpr.gameObject.SetActive(true);
        //_SelectGachaCountObj.SetActive(false);

        //일반 구매 / 할인 구매 오브젝트 초기화
        _GoodsUnit.gameObject.SetActive(false);
        _SaleObj.SetActive(false);

        if (mbSaleApply == true)
            SetSalePriceObject(mPurchaseValue);
        else
            SetNormalPriceObject(mPurchaseValue);
    }
    */

    public void SetData(GameClientTable.StoreDisplayGoods.Param storeDisplayTableData, GameTable.Store.Param storeTableData,
        int saleValue, bool bSaleApply, bool bFree, UnityAction callBackOK) {

        //테이블
        storeDisplayTable = storeDisplayTableData;
        storeTable = storeTableData;

        //상품 세부 값 
        mSaleValue = saleValue;
        mbSaleApply = bSaleApply;
        mbFree = bFree;

        mCallBackOK = callBackOK;

        //구매 재화 타입
        mPurchaseType = (eREWARDTYPE)storeTable.PurchaseType;
        mPurchaseIndex = (eGOODSTYPE)storeTable.PurchaseIndex;
        mPurchaseValue = storeTable.PurchaseValue;

        //최소(한번에 시도할 횟수)
        mMaxSelectCount = GameInfo.Instance.GameConfig.GoldGachaTenMaxCnt;
        mMinCount = 10;
        mMaxCount = mMaxSelectCount * mMinCount;
        mMultiCount = mMinCount;

        //토글 기본 값 세팅
        IsSkipDirector = false;
        //스킵 on/off
        _SkipToggle.SetToggle(IsSkipDirector ? 0 : 1, SelectEvent.Code);

        RefreshGachaCountSelect();
    }

    #region Button Function
    public void OnClick_BuyBtn() {

        if (mPurchaseType == eREWARDTYPE.GOODS) {
            if (GameSupport.IsCheckGoods(mPurchaseIndex, mAutoTotalPrice) == false)
                return;
        }

        //최소 선택이면 기존 UI로 결과가 나오게 한다
        if (mMultiCount <= mMinCount) {
            UIGachaPanel panel = LobbyUIManager.Instance.GetActiveUI<UIGachaPanel>("GachaPanel");
            if (panel != null) {
                panel.SetAutoGacha(false);
            }
        }
        else {
            UIAutoGachaResultPopup autoGachaResultPopup = LobbyUIManager.Instance.GetUI<UIAutoGachaResultPopup>("AutoGachaResultPopup");
            if (autoGachaResultPopup == null) {
                Debug.LogError("오토 가챠 결과 팝업 없음");
                return;
            }
            //가챠 선택 횟수를 결과창에 알려준다
            autoGachaResultPopup.SetData(mMultiCount, mMinCount);
        }

        SetUIActive(false);
        mCallBackOK?.Invoke();
    }

    public void OnClick_PlusTenBtn() {
        if (mMultiCount >= mMaxCount)
            return;

        SetMultiCount(mMultiCount + mMinCount);
    }

    public void OnClick_MinusTenBtn() {
        if (mMultiCount <= mMinCount)
            return;

        SetMultiCount(mMultiCount - mMinCount);
    }

    public void OnClick_MinBtn() {
        if (mMultiCount == mMinCount)
            return;

        SetMultiCount(mMinCount);
    }

    public void OnClick_MaxBtn() {
        if (mMultiCount == mMaxCount)
            return;

        //내가 뽑을 수 있는 최대치를 구한다
        long haveGold = GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.GOLD];
        if (mbSaleApply == true) {
            //할인중일 경우, 할인중인 1개의 상품의 할인율 만큼 골드가 더 있는 것으로 계산한다.
            long discountValue = mPurchaseValue - mSaleValue;
            haveGold += discountValue;
        }

        int calcCount = (int)(haveGold / mPurchaseValue);
        calcCount *= mMinCount;
        //Debug.Log($"<color=#FF9900>mbSaleApply : {mbSaleApply}  mPurchaseValue : {mPurchaseValue} My Gold : {haveGold}  CalcCount : {calcCount}</color>");

        SetMultiCount(calcCount);
    }
    #endregion

    //가챠 수량 선택 UI 갱신
    private void RefreshGachaCountSelect() {
        //배수 선택 한 만큼만 보이게
        _GachaName.textlocalize = string.Format(FLocalizeString.Instance.GetText(112),
            FLocalizeString.Instance.GetText(storeDisplayTable.Name),
            FLocalizeString.Instance.GetText(1443, mMultiCount));

        //일반 구매 / 할인 구매 오브젝트 초기화
        _GoodsUnit.gameObject.SetActive(false);
        _SaleObj.SetActive(false);

        //무료 가챠 일 때
        if (mbFree == true) {
            _GachaDesc.textlocalize = FLocalizeString.Instance.GetText(114);
            _GoodsBGSpr.gameObject.SetActive(false);
            _SelectGachaCountObj.SetActive(false);
        }
        //무료 가챠가 아니다.
        else {
            _GachaDesc.textlocalize = FLocalizeString.Instance.GetText(113);
            _GoodsBGSpr.gameObject.SetActive(true);

            //최종 가격 선택
            mAutoTotalPrice = mbSaleApply ? mSaleValue : mPurchaseValue;

            //티켓 등 아이템을 소모하는 가챠 일 경우 -> 카운트 선택 x
            if (mPurchaseType == eREWARDTYPE.ITEM) {
                _SelectGachaCountObj.SetActive(false);

                //할인 여부에 따라 일반/할인 항목 표시를 한다.
                if (mbSaleApply == true)
                    SetSalePriceObject();
                else
                    SetNormalPriceObject();
            }
            else {
                _SelectGachaCountObj.SetActive(true);
                //+- 버튼 액티브 활성화
                _MinusButton.isEnabled = mMultiCount > mMinCount;
                _PlusButton.isEnabled = mMultiCount < mMaxCount;
                //슬라이더 동기화
                float f = (float) mMultiCount / mMaxCount;
                _Slider.Set(f, false);


                //가챠 수량 Label 갱신
                _MultiGachaCount.textlocalize = mMultiCount.ToString();

                //기본 수량 선택일 경우 : 할인정보를 표시한다.
                if (mMultiCount == mMinCount) {
                    if (mbSaleApply == true)
                        SetSalePriceObject();
                    else
                        SetNormalPriceObject();
                }
                //연속 가챠를 위한 수량을 선택 했을 경우 : 할인 정보를 표시 하지 않고 합쳐서 보여준다.
                else {
                    int buyCount = mMultiCount / mMinCount;

                    //가격을 다시 계산해준다.
                    if (mbSaleApply == true)
                        mAutoTotalPrice = mSaleValue + (mPurchaseValue * (buyCount - 1));
                    else
                        mAutoTotalPrice = mPurchaseValue * buyCount;

                    SetNormalPriceObject();
                }
            }
        }
    }

    private void SetPriceIconTexture(eREWARDTYPE purchaseType, eGOODSTYPE purchaseIndex) {
        //아이템으로 구매하는 경우 텍스쳐를 이용해서 구매 재화 아이템을 표시한다.
        if (purchaseType == eREWARDTYPE.ITEM) {
            _ItemTex.gameObject.SetActive(true);
            var data = GameInfo.Instance.GameTable.FindItem((int)purchaseIndex);
            if (data != null)
                _ItemTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + data.Icon);
        }
        else {
            _ItemTex.gameObject.SetActive(false);
        }
    }

    private void SetSalePriceObject() {
        _SaleObj.SetActive(true);
        _SaleGoodsUnit.kIconSpr.gameObject.SetActive(mPurchaseType != eREWARDTYPE.ITEM);
        _SaleGoodsUnit.InitGoodsUnit(mPurchaseIndex, mAutoTotalPrice, false, mbSaleApply);
        //원가 표시
        _SaleDiscountValueLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), mPurchaseValue);
        
        SetPriceIconTexture(mPurchaseType, mPurchaseIndex);
    }

    private void SetNormalPriceObject() {
        _GoodsUnit.gameObject.SetActive(true);
        _GoodsUnit.kIconSpr.gameObject.SetActive(mPurchaseType != eREWARDTYPE.ITEM);
        _GoodsUnit.InitGoodsUnit(mPurchaseIndex, mAutoTotalPrice, false, false);

        SetPriceIconTexture(mPurchaseType, mPurchaseIndex);
    }

    private void SetMultiCount(int _count) {
        mMultiCount = Mathf.Clamp(_count, mMinCount, mMaxCount);

        RefreshGachaCountSelect();
    }

    private void OnChange_Slide() {
        int count = (int)(_Slider.value * mMaxSelectCount);

        count *= mMinCount;

        SetMultiCount(count);
    }

    private bool OnToggleSkip(int select, SelectEvent type) {
        IsSkipDirector = select == 0 ? true : false;
        Debug.Log($"<color=#4499FF>IsSkipDirector = {IsSkipDirector}</color>");

        return true;
    }
}
