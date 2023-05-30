using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerUIManager : MonoBehaviour {

    //------------------------------------
    [SerializeField]
    private FList _listInstance = null;

    public float kBannerScrollTime = 3.0f;
    public float kCircleInterval = 1.0f;
    public GameObject kSelectCircle;        //배너밑에 동그란 게임오브젝트
    public GameObject kReleaseCircle;       //배너밑에 동그란  게임오브젝트 선택된 배너에 표시

    private List<BannerData> _bannerlist = new List<BannerData>();
    private List<GameObject> mBannerCircleList = new List<GameObject>();
    private int _index;
    private int _timeid = -1;

    private float _bannerSlotSizeX = 0;
    private eLANGUAGE _curLanguage;

    // Use this for initialization
    void Start()
    {
        if (null == this._listInstance) return;

        this._listInstance.EventUpdate = this._UpdateBannerListSlot;
        this._listInstance.EventGetItemCount = this._GetBannerElementCount;
        this._listInstance.ScrollView.onDragFinished = OnDragFinished;
        this._listInstance.ScrollView.onPressMoving = OnPressMoving;

        _index = 0;
        _bannerlist.Clear();
        //_bannerlist = GameInfo.Instance.ServerData.BannerList;//.FindAll(x => x.Type == 0);

        //배너 타입별로 보여지고 안보여지고 설정
        //_bannerlist = GameInfo.Instance.ServerData.BannerList.FindAll(x => (x.BannerType == (int)eBannerType.ROLLING || x.BannerType == (int)eBannerType.PACKAGE_BANNER));

        DateTime nowTime = GameInfo.Instance.GetNetworkTime();

        foreach (BannerData bannerData in GameInfo.Instance.ServerData.BannerList)
        {
            if (null == bannerData)
            {
                continue;
            }
            
            if(bannerData.BannerType == (int)eBannerType.ROLLING || bannerData.BannerType == (int)eBannerType.PACKAGE_BANNER)
            {
                DateTime bannerStartDate = bannerData.StartDate;
                DateTime bannerEndDate = bannerData.EndDate;
                if (bannerStartDate.Ticks <= nowTime.Ticks && bannerEndDate.Ticks >= nowTime.Ticks)
                {
                    _bannerlist.Add(bannerData);
                }
            }
        }

        this._listInstance.UpdateList();
        CreateReleaserCircleGameObject();
    }

    void OnEnable()
    {
        if( _timeid != -1 )
            FGlobalTimer.Instance.RemoveTimer(_timeid);
        _timeid = FGlobalTimer.Instance.AddTimer(kBannerScrollTime, OnClick_BannerRigthBtn);
        
        if (null == this._listInstance) return;

        if (null == this._listInstance.EventUpdate) return;

        _listInstance.UpdateList();

        //191116
        //배너를 클릭하여 다른 panel에 갔다오면 이상한현상 수정
        OnDragFinished();
        
        _curLanguage = FLocalizeString.Language;
    }
    
    private void Update()
    {
        if (_curLanguage != FLocalizeString.Language)
        {
            _curLanguage = FLocalizeString.Language;
            _listInstance.RefreshNotMove();
        }
    }
    
    void OnDisable()
    {
        FGlobalTimer.Instance.RemoveTimer(_timeid);
        _timeid = -1;
    }

    private void _UpdateBannerListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIBannerSlot bannerUnit = slotObject.GetComponent<UIBannerSlot>();
            if (null == bannerUnit)
                break;

            if (0 <= index && _bannerlist.Count > index)
            {
                BannerData data = _bannerlist[index];

                bannerUnit.UpdateSlot(UIBannerSlot.ePosType.Banner, index, data);
            }

        } while (false);
    }

    private int _GetBannerElementCount()
    {
        return _bannerlist.Count;
    }

    public void OnClick_BannerLeftBtn()
    {
        if (_bannerlist.Count == 0)
            return;

        int temp = _index;
        temp -= 1;
        if (temp < 0)
            temp = _bannerlist.Count - 1;

        SetPositionSelect(temp);
    }

    public void OnClick_BannerRigthBtn()
    {
        if (gameObject.activeInHierarchy == false)
            return;

        if (_bannerlist.Count == 0)
            return;

        if (_listInstance != null && _listInstance.ScrollView.isDragging == true)
            return;

            int temp = _index;
        temp += 1;
        if (temp >= _bannerlist.Count)
            temp = 0;

        SetPositionSelect(temp);
    }

    public void OnClick_BannerCircleUnit(GameObject _go)
    {
        int currentIndex = -1;
        int max = _bannerlist.Count;
        for (int i = 0; i < max; i++)
        {

            if (_go == mBannerCircleList[i])
            {
                currentIndex = i;
            }
        }

        if (currentIndex != -1) SetPositionSelect(currentIndex);
    }

    void SetPositionSelect(int i)
    {
        if (i < 0 || _bannerlist.Count <= i)
            return;

        _index = i;

        if (_listInstance.RowCount != 0)
        {
            _listInstance.SpringSetFocus(_index);
        }

        kSelectCircle.SetActive(true);

        FGlobalTimer.Instance.ResetTimer(_timeid);

        if (mBannerCircleList.Count != 0)
            kSelectCircle.transform.localPosition = mBannerCircleList[_index].transform.localPosition;
    }

    public void CreateReleaserCircleGameObject()
    {
        int maxCount = _bannerlist.Count;
        int needCount = maxCount - mBannerCircleList.Count;
        int minusCount = -needCount;

        if (needCount == 0)
            return;

        if (mBannerCircleList.Count >= maxCount)
        {

            for (int i = 0; i < minusCount; i++)
            {
                GameObject go = null;
                if (mBannerCircleList.Count != 0)
                {
                    go = mBannerCircleList[0];
                    Destroy(go);
                    mBannerCircleList.Remove(go);
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

                    mBannerCircleList.Add(go);
                    go.SetActive(true);
                }
            }
            kReleaseCircle.SetActive(false);
        }

        if (mBannerCircleList.Count == 0)
            return;

        float width = (float)mBannerCircleList[0].GetComponent<UISprite>().width + kCircleInterval;
        float totalwidth = width * mBannerCircleList.Count;
        float fstartx = -(totalwidth / 2.0f) + (width / 2);
        for (int i = 0; i < mBannerCircleList.Count; i++)
        {
            mBannerCircleList[i].transform.localPosition = new Vector3(fstartx + ((float)width * i), kSelectCircle.transform.localPosition.y, 1.0f);
            if (i == 0) SetPositionSelect(i);
        }

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
        if (_listInstance == null || _listInstance.ScrollView == null)
            return;

        if (_bannerSlotSizeX == 0)
        {
            UIWidget uIWidget = _listInstance.TargetItem.GetComponent<UIWidget>();
            _bannerSlotSizeX = uIWidget.localSize.x;
        }

        //  현재 위치 값
        float xPos = _listInstance.ScrollView.transform.localPosition.x;
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
        if (index > _bannerlist.Count)
        {
            index = _bannerlist.Count - 1;
        }
        
        //  서클로 현재 가운데 선택된 배너가 몇번째인지를 표시해줍니다.
        if (mBannerCircleList.Count != 0)
        {
            if (index >= mBannerCircleList.Count)
                index = mBannerCircleList.Count - 1;

            if (index >= 0)
                kSelectCircle.transform.localPosition = mBannerCircleList[index].transform.localPosition;
        }

        _index = index;
    }
}