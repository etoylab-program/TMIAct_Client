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

    //���� ��ŵ ���� (���� 1ȸ ����)
    public bool IsSkipDirector { get; private set; }

    //�Ű������� �޾ƿ��� ������
    private GameClientTable.StoreDisplayGoods.Param storeDisplayTable;
    private GameTable.Store.Param storeTable;
    private int mSaleValue;
    private bool mbSaleApply;
    private bool mbFree;
    private UnityAction mCallBackOK;

    //���� ��� ������
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

    /*�����̼� ��í ���� ���� - �۾��� ������ �Ⱦ� �� ���Ƽ� ����
    //����
        UIAutoGachaPopup autoGachaPopup = LobbyUIManager.Instance.GetUI<UIAutoGachaPopup>("AutoGachaPopup");
        if (autoGachaPopup == null) {
            Debug.LogError("���� ��í �˾� ����");
            return;
        }
        autoGachaPopup.SetData(FLocalizeString.Instance.GetText(1717), FLocalizeString.Instance.GetText(1718, t.Hours, t.Minutes), bisSale, gachaTable.OpenCash, openCash, OnCallBack_BuyPopup_PurchaseRotationGachaTime);
        autoGachaPopup.SetUIActive(true);


    public void SetData(string title, string desc, bool bSaleApply, int purchaseValue, int saleValue, UnityAction callBackOK) {
        //���̺�
        storeDisplayTable = null;
        storeTable = null;

        //��ǰ ���� �� 
        mSaleValue = saleValue;
        mbSaleApply = bSaleApply;
        mbFree = false;

        mCallBackOK = callBackOK;

        //���� ��ȭ Ÿ��
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

        //�Ϲ� ���� / ���� ���� ������Ʈ �ʱ�ȭ
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

        //���̺�
        storeDisplayTable = storeDisplayTableData;
        storeTable = storeTableData;

        //��ǰ ���� �� 
        mSaleValue = saleValue;
        mbSaleApply = bSaleApply;
        mbFree = bFree;

        mCallBackOK = callBackOK;

        //���� ��ȭ Ÿ��
        mPurchaseType = (eREWARDTYPE)storeTable.PurchaseType;
        mPurchaseIndex = (eGOODSTYPE)storeTable.PurchaseIndex;
        mPurchaseValue = storeTable.PurchaseValue;

        //�ּ�(�ѹ��� �õ��� Ƚ��)
        mMaxSelectCount = GameInfo.Instance.GameConfig.GoldGachaTenMaxCnt;
        mMinCount = 10;
        mMaxCount = mMaxSelectCount * mMinCount;
        mMultiCount = mMinCount;

        //��� �⺻ �� ����
        IsSkipDirector = false;
        //��ŵ on/off
        _SkipToggle.SetToggle(IsSkipDirector ? 0 : 1, SelectEvent.Code);

        RefreshGachaCountSelect();
    }

    #region Button Function
    public void OnClick_BuyBtn() {

        if (mPurchaseType == eREWARDTYPE.GOODS) {
            if (GameSupport.IsCheckGoods(mPurchaseIndex, mAutoTotalPrice) == false)
                return;
        }

        //�ּ� �����̸� ���� UI�� ����� ������ �Ѵ�
        if (mMultiCount <= mMinCount) {
            UIGachaPanel panel = LobbyUIManager.Instance.GetActiveUI<UIGachaPanel>("GachaPanel");
            if (panel != null) {
                panel.SetAutoGacha(false);
            }
        }
        else {
            UIAutoGachaResultPopup autoGachaResultPopup = LobbyUIManager.Instance.GetUI<UIAutoGachaResultPopup>("AutoGachaResultPopup");
            if (autoGachaResultPopup == null) {
                Debug.LogError("���� ��í ��� �˾� ����");
                return;
            }
            //��í ���� Ƚ���� ���â�� �˷��ش�
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

        //���� ���� �� �ִ� �ִ�ġ�� ���Ѵ�
        long haveGold = GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.GOLD];
        if (mbSaleApply == true) {
            //�������� ���, �������� 1���� ��ǰ�� ������ ��ŭ ��尡 �� �ִ� ������ ����Ѵ�.
            long discountValue = mPurchaseValue - mSaleValue;
            haveGold += discountValue;
        }

        int calcCount = (int)(haveGold / mPurchaseValue);
        calcCount *= mMinCount;
        //Debug.Log($"<color=#FF9900>mbSaleApply : {mbSaleApply}  mPurchaseValue : {mPurchaseValue} My Gold : {haveGold}  CalcCount : {calcCount}</color>");

        SetMultiCount(calcCount);
    }
    #endregion

    //��í ���� ���� UI ����
    private void RefreshGachaCountSelect() {
        //��� ���� �� ��ŭ�� ���̰�
        _GachaName.textlocalize = string.Format(FLocalizeString.Instance.GetText(112),
            FLocalizeString.Instance.GetText(storeDisplayTable.Name),
            FLocalizeString.Instance.GetText(1443, mMultiCount));

        //�Ϲ� ���� / ���� ���� ������Ʈ �ʱ�ȭ
        _GoodsUnit.gameObject.SetActive(false);
        _SaleObj.SetActive(false);

        //���� ��í �� ��
        if (mbFree == true) {
            _GachaDesc.textlocalize = FLocalizeString.Instance.GetText(114);
            _GoodsBGSpr.gameObject.SetActive(false);
            _SelectGachaCountObj.SetActive(false);
        }
        //���� ��í�� �ƴϴ�.
        else {
            _GachaDesc.textlocalize = FLocalizeString.Instance.GetText(113);
            _GoodsBGSpr.gameObject.SetActive(true);

            //���� ���� ����
            mAutoTotalPrice = mbSaleApply ? mSaleValue : mPurchaseValue;

            //Ƽ�� �� �������� �Ҹ��ϴ� ��í �� ��� -> ī��Ʈ ���� x
            if (mPurchaseType == eREWARDTYPE.ITEM) {
                _SelectGachaCountObj.SetActive(false);

                //���� ���ο� ���� �Ϲ�/���� �׸� ǥ�ø� �Ѵ�.
                if (mbSaleApply == true)
                    SetSalePriceObject();
                else
                    SetNormalPriceObject();
            }
            else {
                _SelectGachaCountObj.SetActive(true);
                //+- ��ư ��Ƽ�� Ȱ��ȭ
                _MinusButton.isEnabled = mMultiCount > mMinCount;
                _PlusButton.isEnabled = mMultiCount < mMaxCount;
                //�����̴� ����ȭ
                float f = (float) mMultiCount / mMaxCount;
                _Slider.Set(f, false);


                //��í ���� Label ����
                _MultiGachaCount.textlocalize = mMultiCount.ToString();

                //�⺻ ���� ������ ��� : ���������� ǥ���Ѵ�.
                if (mMultiCount == mMinCount) {
                    if (mbSaleApply == true)
                        SetSalePriceObject();
                    else
                        SetNormalPriceObject();
                }
                //���� ��í�� ���� ������ ���� ���� ��� : ���� ������ ǥ�� ���� �ʰ� ���ļ� �����ش�.
                else {
                    int buyCount = mMultiCount / mMinCount;

                    //������ �ٽ� ������ش�.
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
        //���������� �����ϴ� ��� �ؽ��ĸ� �̿��ؼ� ���� ��ȭ �������� ǥ���Ѵ�.
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
        //���� ǥ��
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
