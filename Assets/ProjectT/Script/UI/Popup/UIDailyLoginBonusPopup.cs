
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class DailyLoginBonusPopup
{
    public static UIDailyLoginBonusPopup GetDailyLoginBonusPopup()
    {
        UIDailyLoginBonusPopup mpopup = LobbyUIManager.Instance.GetUI<UIDailyLoginBonusPopup>("DailyLoginBonusPopup");
        return mpopup;
    }

    public static void OpenDailyLoginPopup()
    {
        GetDailyLoginBonusPopup().InitDailyLoginPopup();
    }

    public static void OpenGuerrillaLoginPopup(int groupId)
    {
        GetDailyLoginBonusPopup().InitGuerrillaLoginPopup(groupId);
    }

	public static void OpenLoginEventPopup(int loginEventInfoIndex)
	{
		GetDailyLoginBonusPopup().InitLoginEventPopup(loginEventInfoIndex);
	}
}

public class UIDailyLoginBonusPopup : FComponent
{
    [System.Serializable]
    public class DailyLoginSlot
    {
        public GameObject kRoot;
        public GameObject kEFF;
        public GameObject kDayParent;
        public GameObject kMarkParent;
        public GameObject kRewardParent;
    }

    public UITexture kloginbonusTex;
    public UILabel kloginbonusLabel;

    private GameObject kEFF;
    public UITexture kLineTex;
    public UITexture kcircleTex;

    public GameObject kGoDaysParents;
    public GameObject kGoMarksParents;
    public GameObject kGoRewardsParents;
    
    public DailyLoginSlot kSevenDay;
    public DailyLoginSlot kFiveDay;
    public DailyLoginSlot kThreeDay;
    public DailyLoginSlot kFifteenDay;

    public GameObject kDate;
    public UILabel kDateLabel;

	[Header("[30Days Login Bonus]")]
	public GameObject	_30DaysItemObj;
	public FList		_30DaysItemSlotList;
	public GameObject	_30DaysEffectObj;

    private List<UILabel> kDayNumberList = new List<UILabel>();
    private List<UISprite> kReciveMarkList = new List<UISprite>();
    private List<UIRewardListSlot> kRewardUIList = new List<UIRewardListSlot>();

    private List<RewardData> m_rewardList = new List<RewardData>();
    private int m_loginGroupID = -1;
    private int m_loginGroupCount = 0;
    private int m_loginGroupArrNumber = 0;
    private bool m_isClickClose = false;
    private Coroutine m_cr = null;

    //Guerrilla Values
    private int m_guerrillaGroupID = 0;
    private List<GuerrillaMissionData> m_guerrillaDataList = null;

    private Coroutine _bgLoadCoroutine = null;

	// Login Event
	private int					mLoginEventInfoIndex	= -1;
	private UIRewardListSlot	mCur30DaysRewardListSlot		= null;
	private int _absentReward = -1;
	private byte _rewardDay = 0;

    private void _DestroyMainTexture(UITexture texture)
	{
		if (texture.mainTexture != null)
		{
			if (string.IsNullOrEmpty(texture.mainTexture.name))
			{
				DestroyImmediate(texture.mainTexture, false);
			}
			texture.mainTexture = null;
		}
	}
	
	public override void Awake()
    {
        base.Awake();

		_30DaysItemSlotList.EventUpdate = Update30DaysItemSlotList;
		_30DaysItemSlotList.EventGetItemCount = Get30DaysItemListCount;

		//SetList();
	}

    public void InitDailyLoginPopup()
    {
        SetUIActive(true);
		mCur30DaysRewardListSlot = null;

		SetList();
        InitComponent();
    }

    public void InitGuerrillaLoginPopup(int groupId)
    {
        SetUIActive(true);

		mCur30DaysRewardListSlot = null;
		m_guerrillaGroupID = groupId;

		SetGuerrillaList();
        InitGuerrillaLoginBonus();
    }

	public void InitLoginEventPopup(int loginEventInfoIndex)
	{
		mCur30DaysRewardListSlot = null;
		_rewardDay = 0;

		if (!InitLoginEvent(loginEventInfoIndex))
		{
			SetUIActive(false);
		}
	}

    //  팝업 최초 생성시 리스트 셋팅
    private void SetList()
    {
        kSevenDay.kRoot.SetActive(true);

        kFiveDay.kRoot.SetActive(false);
        kThreeDay.kRoot.SetActive(false);
        kFifteenDay.kRoot.SetActive(false);
        kDate.SetActive(false);
		_30DaysItemObj.SetActive(false);

		kDayNumberList.Clear();
        kReciveMarkList.Clear();
        kRewardUIList.Clear();

        kEFF = kSevenDay.kEFF;

        if (kGoDaysParents != null)
            kDayNumberList.AddRange(kGoDaysParents.GetComponentsInChildren<UILabel>(true));

        if (kGoMarksParents != null)
            kReciveMarkList.AddRange(kGoMarksParents.GetComponentsInChildren<UISprite>(true));

        if (kGoRewardsParents != null)
            kRewardUIList.AddRange(kGoRewardsParents.GetComponentsInChildren<UIRewardListSlot>(true));
    }

    public void SetGuerrillaList()
    {
        kSevenDay.kRoot.SetActive(false);
        kFiveDay.kRoot.SetActive(false);
        kThreeDay.kRoot.SetActive(false);
        kFifteenDay.kRoot.SetActive(false);
		_30DaysItemObj.SetActive(false);

		kDate.SetActive(true);

        m_guerrillaDataList = new List<GuerrillaMissionData>();
        m_guerrillaDataList.Clear();

        GameSupport.GetGuerrillaMissionListWithTimeByGroupId(ref m_guerrillaDataList, m_guerrillaGroupID);

        if (m_guerrillaDataList == null)
        {
            Log.Show(m_guerrillaGroupID + " 게릴라 미션 데이터가 없습니다.", Log.ColorType.Red);
            return;
        }

        kDayNumberList.Clear();
        kReciveMarkList.Clear();
        kRewardUIList.Clear();

        if (m_guerrillaDataList.Count == 7)
        {
            kSevenDay.kRoot.SetActive(true);
            kEFF = kSevenDay.kEFF;
            kDayNumberList.AddRange(kSevenDay.kDayParent.GetComponentsInChildren<UILabel>(true));
            kReciveMarkList.AddRange(kSevenDay.kMarkParent.GetComponentsInChildren<UISprite>(true));
            kRewardUIList.AddRange(kSevenDay.kRewardParent.GetComponentsInChildren<UIRewardListSlot>(true));
        }
        else if(m_guerrillaDataList.Count == 5)
        {
            kFiveDay.kRoot.SetActive(true);
            kEFF = kFiveDay.kEFF;
            kDayNumberList.AddRange(kFiveDay.kDayParent.GetComponentsInChildren<UILabel>(true));
            kReciveMarkList.AddRange(kFiveDay.kMarkParent.GetComponentsInChildren<UISprite>(true));
            kRewardUIList.AddRange(kFiveDay.kRewardParent.GetComponentsInChildren<UIRewardListSlot>(true));
        }
        else if(m_guerrillaDataList.Count == 3)
        {
            kThreeDay.kRoot.SetActive(true);
            kEFF = kThreeDay.kEFF;
            kDayNumberList.AddRange(kThreeDay.kDayParent.GetComponentsInChildren<UILabel>(true));
            kReciveMarkList.AddRange(kThreeDay.kMarkParent.GetComponentsInChildren<UISprite>(true));
            kRewardUIList.AddRange(kThreeDay.kRewardParent.GetComponentsInChildren<UIRewardListSlot>(true));
        }
        else if (m_guerrillaDataList.Count == 15)
        {
            kFifteenDay.kRoot.SetActive(true);
            kEFF = kFifteenDay.kEFF;
            kDayNumberList.AddRange(kFifteenDay.kDayParent.GetComponentsInChildren<UILabel>(true));
            kReciveMarkList.AddRange(kFifteenDay.kMarkParent.GetComponentsInChildren<UISprite>(true));
            kRewardUIList.AddRange(kFifteenDay.kRewardParent.GetComponentsInChildren<UIRewardListSlot>(true));
        }

		kDateLabel.textlocalize = GameSupport.GetEndTime(m_guerrillaDataList[0].EndDate.AddSeconds(-1));        
    }

	public override void InitComponent()
    {
        m_rewardList.Clear();

        m_isClickClose = false;
        kEFF.gameObject.SetActive(false);

        //  현재 로그인 그룹 ID
        m_loginGroupID = GameInfo.Instance.UserData.LoginBonusGroupID;
        //  현재 로그인 그룹 출석 횟수
        m_loginGroupCount = GameInfo.Instance.UserData.LoginBonusGroupCnt;
        //  배열 넘버용
        m_loginGroupArrNumber = m_loginGroupCount - 1;

        Log.Show(m_loginGroupCount + " / " + m_loginGroupArrNumber);

        List<GameTable.LoginBonus.Param> list = GameInfo.Instance.GameTable.FindAllLoginBonus(a => a.LoginGroupID == m_loginGroupID);
        GameTable.LoginBonus.Param nowDay = list.Find(a=> a.LoginCnt == m_loginGroupCount);

        for (int i = 0; i < list.Count; i++)
        {
            //  출석 횟수 7일
            if (i + 1 > (int)eCOUNT.LOGINBONUSMAX)
                break;

            var dayReward = GameInfo.Instance.GameTable.FindRandom(list[i].RewardGroupID);
            var rewardInfo = new RewardData(dayReward.ProductType,
                                            dayReward.ProductIndex,
                                            dayReward.ProductValue);

            m_rewardList.Add(rewardInfo);

            SetItems(i, rewardInfo, m_loginGroupCount);
        }

        //  하단 텍스트 셋팅
        FLocalizeString.SetLabel(kloginbonusLabel, 3040, GameSupport.GetProductName(m_rewardList[m_loginGroupArrNumber]));

        //  배경 셋팅
        _DestroyMainTexture(kloginbonusTex);
        GameSupport.LoadLocalizeTexture(kloginbonusTex, "icon", string.Format("Icon/LoginBonus/{0}.png", nowDay.BGimg), true);
        //kloginbonusTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/LoginBonus/{0}.png", nowDay.BGimg)); ;

        //bool isAction = System.Convert.ToBoolean(UIValue.Instance.GetValue(UIValue.EParamType.LoginBonusRewardReceiving));
        bool isAction = true;
        if (isAction == true)
        {
            //  움직이는 연출 시작
            m_cr = StartCoroutine(ShowDirect());
        }
        else
        {
            ShowNotDirect();
        }
    }

	public void InitGuerrillaLoginBonus()
    {
        m_rewardList.Clear();
        m_isClickClose = false;
        kEFF.gameObject.SetActive(false);

        GllaMissionData missionData = GameInfo.Instance.GllaMissionList.Find(x => x.GroupID == m_guerrillaGroupID);

        m_loginGroupID = m_guerrillaGroupID;
        m_loginGroupCount = missionData.Count;
        m_loginGroupArrNumber = m_loginGroupCount - 1;

        List<GuerrillaMissionData> list = new List<GuerrillaMissionData>();
        list.Clear();

        GameSupport.GetGuerrillaMissionListWithTimeByGroupId(ref list, m_guerrillaGroupID);

        for (int i = 0; i < list.Count; i++)
        {
            if (i + 1 > list.Count)
                break;

            var rewardInfo = new RewardData(list[i].RewardType,
                                            list[i].RewardIndex,
                                            list[i].RewardValue);

            m_rewardList.Add(rewardInfo);

			SetItems(i, rewardInfo, m_loginGroupCount);
        }

        if ( m_rewardList.Count <= m_loginGroupArrNumber ) {
			MessagePopup.OK( eTEXTID.OK, FLocalizeString.Instance.GetText( 3368 ), null );
			SetUIActive( false, false );
			return;
		}

		FLocalizeString.SetLabel(kloginbonusLabel, 3040, GameSupport.GetProductName(m_rewardList[m_loginGroupArrNumber]));

        BannerData bannerData = GameInfo.Instance.ServerData.BannerList.Find(x =>
            x.BannerType == (int)eBannerType.GLLA_MISSION_LOGIN &&
            x.BannerTypeValue == m_guerrillaGroupID);

        if (bannerData == null) {
            Debug.LogWarning($"<color=#FF9900> 배너 데이터 NULL! </color>");
        }
        else {
            GetBGTexture(kloginbonusTex, bannerData.UrlImage);
        }

		bool isAction = true;
		if (isAction == true)
		{
			//  움직이는 연출 시작
			m_cr = StartCoroutine(ShowDirect());
		}
		else
		{
			ShowNotDirect();
		}
	}

	private bool InitLoginEvent(int loginEventInfoIndex)
	{
		if(loginEventInfoIndex < 0 || loginEventInfoIndex >= GameInfo.Instance.UserData.ListLoginEventInfo.Count)
		{
			return false;
		}

		mLoginEventInfoIndex = loginEventInfoIndex;
		m_isClickClose = false;

		kSevenDay.kRoot.SetActive(false);
		kFiveDay.kRoot.SetActive(false);
		kThreeDay.kRoot.SetActive(false);
		kFifteenDay.kRoot.SetActive(false);
		kDate.SetActive(false);

		kDayNumberList.Clear();
		kReciveMarkList.Clear();
		kRewardUIList.Clear();

		UserData.sLoginEventInfo info = GameInfo.Instance.UserData.ListLoginEventInfo[mLoginEventInfoIndex];
		if(info.LatestRewardDay == 0)
		{
			return false;
		}

		GameTable.LoginEvent.Param param = GameInfo.Instance.GameTable.FindLoginEvent(info.TableId);
		if (param == null)
		{
			return false;
		}

        System.DateTime endTime = GameSupport.GetTimeWithString(param.EndTime, true);
        if (endTime < GameSupport.GetCurrentServerTime())
        {
            return false;
        }

		_absentReward = param.AbsentReward;

		List<GameTable.Random.Param> listRandom = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == param.RewardGroupID);
		if (listRandom == null || listRandom.Count <= 0 || info.LatestRewardDay > (info.EndDay + _absentReward))
		{
			return false;
		}

        bool bOverEndDay = m_isClickClose = info.LatestRewardDay > info.EndDay;
        if (bOverEndDay)
        {
            bool bAllReward = true;
            for (int i = 0; i < info.EndDay; i++)
            {
                ulong flag = (ulong)(1 << i);
                if ((info.RewardInfoFlag & flag) == 0)
                {
                    bAllReward = false;
                    break;
                }
            }

            if (bAllReward)
            {
                return false;
            }
        }
        
		m_rewardList.Clear();
		for (int i = 0; i < listRandom.Count; ++i)
		{
			RewardData data = new RewardData(listRandom[i].ProductType, listRandom[i].ProductIndex, listRandom[i].ProductValue);
			m_rewardList.Add(data);
		}

		BannerData bannerData = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.LOGIN_EVENT_BG && 
																				  x.BannerTypeValue == info.TableId);
		if(bannerData != null)
		{
			GetBGTexture(kloginbonusTex, bannerData.UrlImage);
		}

        int nLatestRewardDay = bOverEndDay ? info.EndDay : info.LatestRewardDay;

        // 하단 텍스트 셋팅
        FLocalizeString.SetLabel(kloginbonusLabel, 3040, GameSupport.GetProductName(m_rewardList[nLatestRewardDay - 1]));

        if (bOverEndDay)
        {
            kloginbonusLabel.textlocalize = string.Empty;
        }

		_30DaysItemObj.SetActive(true);
		_30DaysItemSlotList.UpdateList();
        _30DaysItemSlotList.SpringSetFocus(nLatestRewardDay - 1, isImmediate: true);
        _30DaysEffectObj.SetActive(false);

		SetUIActive(true);

		StopCoroutine("StartDirection");
		StartCoroutine("StartDirection");

		return true;
	}

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
    }

    private void SetItems(int dayNum,RewardData reward,int userCount)
    {
        //  표기용 일수 셋팅
        kDayNumberList[dayNum].textlocalize = string.Format("{0:0#}", dayNum+1);
        //  표기용 보상 셋팅
        kRewardUIList[dayNum].UpdateSlot(reward, true);
        //  표기용 수령 아이콘 표시
        kReciveMarkList[dayNum].gameObject.SetActive(dayNum < userCount);
        kReciveMarkList[dayNum].transform.localScale = Vector3.one;
        kReciveMarkList[dayNum].color = Color.white;

        //  표기용 수령 애니메이션
        Animation anim = kReciveMarkList[dayNum].GetComponent<Animation>();
        anim.enabled = ( dayNum == userCount - 1 );
    }

    private IEnumerator ShowDirect()
    {
        kReciveMarkList[m_loginGroupArrNumber].gameObject.SetActive(false);
        kloginbonusLabel.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);

        SoundManager.Instance.PlayUISnd(16);
        
        kReciveMarkList[m_loginGroupArrNumber].gameObject.SetActive(true);

        Animation anim = kReciveMarkList[m_loginGroupArrNumber].GetComponent<Animation>();
        if (anim != null)
        {
            anim.Rewind();
            anim.Play();
            //  수령 연출시 수령 이펙트 위치 변경
            kEFF.transform.position = kRewardUIList[m_loginGroupArrNumber].transform.position;
            kEFF.gameObject.SetActive(true);
        }
        
        yield return new WaitForSeconds(1.0f);
        kloginbonusLabel.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);
        m_isClickClose = true;
    }

    /// <summary>
    ///  이미 로그인 보너스를 수령한 경우
    /// </summary>
    private void ShowNotDirect()
    {
        kReciveMarkList[m_loginGroupArrNumber].gameObject.SetActive(true);
        Animation anim = kReciveMarkList[m_loginGroupArrNumber].GetComponent<Animation>();
        if (anim != null)
        {
            //anim.Rewind();
            //anim.Play();
            //  수령 연출시 수령 이펙트 위치 변경
            //kEFF.transform.position = kRewardUIList[m_loginGroupCount].transform.position;
            kEFF.gameObject.SetActive(false);
        }

        kloginbonusLabel.gameObject.SetActive(true);

        m_isClickClose = true;
    }

    public void OnClick_BGBtn()
    {
        if(m_isClickClose == false)
        {
			if (mCur30DaysRewardListSlot == null)
			{
				if (m_cr != null)
				{
					StopCoroutine(m_cr);
					m_cr = null;
				}

				if (m_loginGroupArrNumber >= 0 && m_loginGroupArrNumber < kReciveMarkList.Count)
				{
					//  애니메이션 정지
					Animation anim = kReciveMarkList[m_loginGroupArrNumber].GetComponent<Animation>();
					anim.Rewind();

					//  도장
					kReciveMarkList[m_loginGroupArrNumber].gameObject.SetActive(true);
				}

				if (m_loginGroupArrNumber >= 0 && m_loginGroupArrNumber < kRewardUIList.Count)
				{
					kEFF.transform.position = kRewardUIList[m_loginGroupArrNumber].transform.position;
					kEFF.gameObject.SetActive(true);
				}
			}
			else
			{
				StopDirection();
			}

			//  하단 텍스트
			kloginbonusLabel.gameObject.SetActive(true);

            //  버튼클릭가능 여부
            m_isClickClose = true;
        }
        else
        {
            LobbyUIManager.Instance.HideUI("DailyLoginBonusPopup");
        }
    }

    public void ReAttendance(params byte[] pDays)
    {
        GameInfo.Instance.Send_ReqEventLgnRewardTake(GameInfo.Instance.UserData.ListLoginEventInfo[mLoginEventInfoIndex].TableId, pDays, _OnNet_ReAttendance);
    }

    private void _OnNet_ReAttendance(int result, PktMsgType pktmsg)
    {
	    if (pktmsg is PktInfoEvtLgnRwdAck pktInfoEvtLgnRwdAck)
	    {
		    byte rwdDay = pktInfoEvtLgnRwdAck.rwdDays_.vals_.LastOrDefault();
		    if (0 < rwdDay)
		    {
			    _rewardDay = rwdDay;
			    FLocalizeString.SetLabel(kloginbonusLabel, 3040, GameSupport.GetProductName(m_rewardList[_rewardDay - 1]));
		    }
	    }
	    
	    _30DaysItemSlotList.RefreshNotMove();
	    
	    _rewardDay = 0;
	    
	    StopCoroutine("StartDirection");
	    StartCoroutine("StartDirection");
	    
	    LobbyUIManager.Instance.Renewal("TopPanel");
    }

    private void GetBGTexture(UITexture target, string url)
    {
        if (_bgLoadCoroutine != null)
        {
            StopCoroutine(_bgLoadCoroutine);
            _bgLoadCoroutine = null;
        }

        if (GameInfo.Instance.netFlag)
        {
            _bgLoadCoroutine = StartCoroutine(GetBGTextureAsync(target, url));
        }
        else
        {
            target.mainTexture = (Texture2D)ResourceMgr.Instance.LoadFromAssetBundle("temp", "Temp/" + url);
        }
    }

    IEnumerator GetBGTextureAsync(UITexture target, string url)
    {            
        _DestroyMainTexture(target);

        while (this.gameObject.activeSelf)
        {
            target.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(url, true);
            if (target.mainTexture != null)
                break;

            yield return new WaitForSeconds(0.5f);
        }

        if (_bgLoadCoroutine != null)
            _bgLoadCoroutine = null;
    }

	private void Update30DaysItemSlotList(int index, GameObject slotObj)
	{
		UIRewardListSlot slot = slotObj.GetComponent<UIRewardListSlot>();
		if (slot == null || index < 0 || index >= 30)
		{
			return;
		}

		UserData.sLoginEventInfo info = GameInfo.Instance.UserData.ListLoginEventInfo[mLoginEventInfoIndex];
		int attendance = 0; // 0 : 결석, 1 : 출석, 2 : 대기
		bool direction = false;

		ulong check = (ulong)(1 << index);
		if((info.RewardInfoFlag & check) == check)
		{
			attendance = 1;

			int nRewardDay = 0 < _rewardDay ? _rewardDay : info.LatestRewardDay;
			if(index + 1 == nRewardDay)
			{
				direction = true;
				mCur30DaysRewardListSlot = slot;
			}
		}
		else if(index > (info.LatestRewardDay - 1))
		{
			attendance = 2;
		}

		slot.ParentGO = gameObject;
		slot.UpdateSlot(index, m_rewardList[index], attendance, direction, _absentReward);
	}

	private int Get30DaysItemListCount()
	{
		return m_rewardList.Count;
	}

	private IEnumerator StartDirection()
	{
		if (mCur30DaysRewardListSlot == null)
		{
			yield break;
		}

		kloginbonusLabel.gameObject.SetActive(false);
		yield return new WaitForSeconds(1.0f);

		SoundManager.Instance.PlayUISnd(16);
		
		Animation anim = mCur30DaysRewardListSlot.SprAttendance.GetComponent<Animation>();
		if (anim != null)
		{
			mCur30DaysRewardListSlot.ShowAttendance(true);

			anim.Rewind();
			anim.Play();

			_30DaysEffectObj.SetActive(true);
		}

		yield return new WaitForSeconds(1.0f);
		kloginbonusLabel.gameObject.SetActive(true);

		yield return new WaitForSeconds(1.0f);
		m_isClickClose = true;
	}

	private void StopDirection()
	{
		StopCoroutine("StartDirection");

		if (mCur30DaysRewardListSlot)
		{
			mCur30DaysRewardListSlot.ShowAttendance(true);

			Animation anim = mCur30DaysRewardListSlot.SprAttendance.GetComponent<Animation>();
			if (anim != null)
			{
				anim.Rewind();
			}

			_30DaysEffectObj.SetActive(true);
		}
	}

	private void Update()
	{
		if (_30DaysEffectObj && mCur30DaysRewardListSlot != null)
		{
			_30DaysEffectObj.transform.position = mCur30DaysRewardListSlot.transform.position;

			bool isVisible = mCur30DaysRewardListSlot.IsVisible();
			if (_30DaysEffectObj.activeSelf != isVisible)
			{
				_30DaysEffectObj.SetActive(isVisible);
			}
		}
	}
}
