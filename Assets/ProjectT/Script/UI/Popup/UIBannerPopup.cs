using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BannerPopup
{
    public static UIBannerPopup GetBannerPopup()
    {
        UIBannerPopup popup = LobbyUIManager.Instance.GetUI<UIBannerPopup>("BannerPopup");
        return popup;
    }

    public static void ShowBannerPopup(bool bnotice)
    {
        UIBannerPopup popup = GetBannerPopup();
        if (popup == null)
            return;

        popup.InitBannerPopup(bnotice);
    }
}

public class UIBannerPopup : FComponent
{
    [SerializeField] private FList _BannerPopupListInstance;

    public GameObject kSelectCircle;        //배너밑에 동그란 게임오브젝트
    public GameObject kReleaseCircle;       //배너밑에 동그란  게임오브젝트 선택된 배너에 표시

    private List<BannerData> _bannerdataList = new List<BannerData>();
    private List<GameObject> _bannerCircleList = new List<GameObject>();

    private int _index;
    private int _timeid = -1;
    private float _circleInterval = 1f;
    private float _bannerSlotSizeX = 0;

    private bool _bNotice = false;

    public override void Awake()
	{
		base.Awake();

        if (this._BannerPopupListInstance == null) return;

        this._BannerPopupListInstance.EventUpdate = this._UpdateBannerListSlot;
        this._BannerPopupListInstance.EventGetItemCount = this._GetBannerElementCount;
        this._BannerPopupListInstance.ScrollView.onDragFinished = OnDragFinished;
        this._BannerPopupListInstance.ScrollView.onPressMoving = OnPressMoving;

        
        _bannerdataList.Clear();
    }

    public void InitBannerPopup(bool bnotice)
    {
        _bNotice = bnotice;
        SetUIActive(true);
        InitComponent();
    }
 
	public override void InitComponent()
	{        
        _bannerdataList = GameInfo.Instance.ServerData.BannerList.FindAll(x => x.BannerType == (int)eBannerType.LOGIN_PACKAGE_BG &&
            x.StartDate <= GameInfo.Instance.GetNetworkTime() && x.EndDate >= GameInfo.Instance.GetNetworkTime());

        _BannerPopupListInstance.UpdateList();

        CreateReleaserCircleGameObject();

        _index = 0;

        for(int i = 0; i < _bannerdataList.Count; i++)
        {
            GameClientTable.StoreDisplayGoods.Param data = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.PanelType == (int)eSD_PanelType.PACKAGE && x.StoreID == _bannerdataList[i].BannerTypeValue);
            if( data == null ) {
                int a = 0;
                a = 1;
			}
            if (!GameSupport.IsShowStoreDisplay(data))
                continue;
            _index = i;
            break;
        }

        SetPositionSelect(_index);
    }
 
	public override void Renewal(bool bChildren)
	{
        //kBannerTex.mainTexture = null;

        if (null == _bannerdataList)
            return;

        //kBannerTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(_bannerdata[0].UrlImage, true);

        base.Renewal(bChildren);
	}
 
    public void OnClick_ShowPackagePopup()
    {
        OnClickClose();
    }
	
	public void OnClick_BackBtn()
	{
        OnClickClose();
    }
	
	public void OnClick_CloseBtn()
	{
        if (_bNotice)
        {
            if (GameSupport.IsEndCheckMonthly((int)eMonthlyType.NORMAL))
            {
                MessagePopup.CYN(FLocalizeString.Instance.GetText(1689), FLocalizeString.Instance.GetText(1690), FLocalizeString.Instance.GetText((int)eTEXTID.OK), FLocalizeString.Instance.GetText(915),
                    () => { LobbyUIManager.Instance.ShowUI("CashBuyPopup", true); }, 
                    () => 
                        {
                            if (GameSupport.IsEndCheckMonthly((int)eMonthlyType.PREMIUM))
                            {
                                MessagePopup.CYN(FLocalizeString.Instance.GetText(1691), FLocalizeString.Instance.GetText(1692), FLocalizeString.Instance.GetText((int)eTEXTID.OK), FLocalizeString.Instance.GetText(915),
                                    () => { GameSupport.PaymentAgreement_Package(2022); });
                            }
                        });
            }
            else
            {
                if (GameSupport.IsEndCheckMonthly((int)eMonthlyType.PREMIUM))
                {
                    MessagePopup.CYN(FLocalizeString.Instance.GetText(1691), FLocalizeString.Instance.GetText(1692), FLocalizeString.Instance.GetText((int)eTEXTID.OK), FLocalizeString.Instance.GetText(915),
                        () => { GameSupport.PaymentAgreement_Package(2022); });
                }
            }
        }
        OnClickClose();
	}

    public override void OnClickClose()
    {
        _bNotice = false;
        base.OnClickClose();
    }

    public void OnClick_LeftBtn()
    {
        if (_bannerdataList.Count == 0)
            return;

        if (_BannerPopupListInstance != null && _BannerPopupListInstance.ScrollView.isDragging == true)
            return;

        int temp = _index;
        temp -= 1;
        if (temp < 0)
            temp = _bannerdataList.Count - 1;

        SetPositionSelect(temp);
    }

    public void OnClieck_RightBtn()
    {
        if (_bannerdataList.Count == 0)
            return;

        if (_BannerPopupListInstance != null && _BannerPopupListInstance.ScrollView.isDragging == true)
            return;

        int temp = _index;
        temp += 1;
        if (temp >= _bannerdataList.Count)
            temp = 0;

        SetPositionSelect(temp);
    }

    public void CreateReleaserCircleGameObject()
    {
        int maxCount = _bannerdataList.Count;
        int needCount = maxCount - _bannerCircleList.Count;
        int minusCount = -needCount;

        if (needCount == 0)
            return;

        if (_bannerCircleList.Count >= maxCount)
        {

            for (int i = 0; i < minusCount; i++)
            {
                GameObject go = null;
                if (_bannerCircleList.Count != 0)
                {
                    go = _bannerCircleList[0];
                    Destroy(go);
                    _bannerCircleList.Remove(go);
                }
            }
        }
        else
        {
            for (int i = 0; i < needCount; i++)
            {
                GameObject go = GameObject.Instantiate(kReleaseCircle) as GameObject;
                if (go != null && this.gameObject != null)
                {
                    Transform t = go.transform;
                    t.parent = kReleaseCircle.transform.parent;
                    t.localPosition = Vector3.zero;
                    t.localRotation = Quaternion.identity;
                    t.localScale = Vector3.one;
                    go.layer = this.gameObject.layer;

                    _bannerCircleList.Add(go);
                    go.SetActive(true);
                }
            }
            kReleaseCircle.SetActive(false);
        }

        if (_bannerCircleList.Count == 0)
            return;

        float width = (float)_bannerCircleList[0].GetComponent<UISprite>().width + _circleInterval;
        float totalwidth = width * _bannerCircleList.Count;
        float fstartx = -(totalwidth / 2.0f) + (width / 2);
        for (int i = 0; i < _bannerCircleList.Count; i++)
        {
            _bannerCircleList[i].transform.localPosition = new Vector3(fstartx + ((float)width * i), kSelectCircle.transform.localPosition.y, 1.0f);
            if (i == 0) SetPositionSelect(i);
        }

    }

    private void _UpdateBannerListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIBannerListSlot slot = slotObject.GetComponent<UIBannerListSlot>();
            if (null == slot) break;

            slot.ParentGO = this.gameObject;

            BannerData data = null;
            if (0 <= index && _bannerdataList.Count > index)
                data = _bannerdataList[index];

            slot.UpdateSlot(data);

        } while (false);
    }

    private int _GetBannerElementCount()
    {
        if (_bannerdataList == null || _bannerdataList.Count == 0)
            return 0;
        return _bannerdataList.Count;
    }

    void SetPositionSelect(int i)
    {
        if (i < 0 || _bannerdataList.Count <= i)
            return;

        _index = i;

        if (_BannerPopupListInstance.RowCount != 0)
        {
            _BannerPopupListInstance.SpringSetFocus(_index);
        }

        kSelectCircle.SetActive(true);

        FGlobalTimer.Instance.ResetTimer(_timeid);

        if (_bannerCircleList.Count != 0)
            kSelectCircle.transform.localPosition = _bannerCircleList[_index].transform.localPosition;
    }

    /// <summary>
    /// 스크롤뷰에서 드래그를 땟을때
    /// </summary>
    public void OnDragFinished()
    {
        SetPositionSelect(_index);
    }

    /// <summary>
    ///  스크롤뷰에서 드래그중일떄 
    /// </summary>
    public void OnPressMoving()
    {
        if (_BannerPopupListInstance == null || _BannerPopupListInstance.ScrollView == null)
            return;

        if (_bannerSlotSizeX == 0)
        {
            UIWidget uIWidget = _BannerPopupListInstance.TargetItem.GetComponent<UIWidget>();
            _bannerSlotSizeX = uIWidget.localSize.x;
        }

        //  현재 위치 값
        float xPos = _BannerPopupListInstance.ScrollView.transform.localPosition.x;
        //  현재 위치해야하는 인덱스
        int index = Mathf.Abs(System.Convert.ToInt32(xPos / _bannerSlotSizeX));

        //  슬롯이 0이상의 경우(중심이 왼쪽으로 이동하기 때문에 -값)
        if (xPos < 0)
        {
            //  슬롯의 / 2 크기로 높은지 낮은지를 판단
            float value = xPos % _bannerSlotSizeX;
            //  근사치 인덱스 셋팅
            if (value > _bannerSlotSizeX * 0.5f)
                index += 1;
        }
        else
        {
            index = 0;
        }

        //  슬롯이 배너 갯수를 넘어간 경우
        if (index > _bannerdataList.Count)
        {
            index = _bannerdataList.Count - 1;
        }

        //  서클로 현재 가운데 선택된 배너가 몇번째인지를 표시해줍니다.
        if (_bannerCircleList.Count != 0)
        {
            if (index >= _bannerCircleList.Count)
                index = _bannerCircleList.Count - 1;

            if (index >= 0)
                kSelectCircle.transform.localPosition = _bannerCircleList[index].transform.localPosition;
        }

        _index = index;
    }
}
