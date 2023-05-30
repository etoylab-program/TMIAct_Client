using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public delegate void OnMessageRewardCallBack();
public class MessageRewardListPopup
{
    public static void RewardListMessage(string Title,string Msg,List<RewardData> rewards, OnMessageRewardCallBack callback = null)
    {
        UIRewardListPopup rewardListPopup = LobbyUIManager.Instance.ShowUI("RewardListPopup", true)as UIRewardListPopup;
        GameInfo.Instance.RewardList = rewards;
        if(rewardListPopup != null)
        {
            rewardListPopup.InitPopup(Title, Msg, rewards, callback);
        }
    }
}

public class UIRewardListPopup : FComponent
{
	public UILabel kTitleLabel;
	public UIButton kCloseBtn;
	public UILabel kTextLabel;
    public GameObject kRootObj;
	[SerializeField] private FList _RewardListInstance;

    private List<RewardData> _rewardList = new List<RewardData>();
    public OnMessageRewardCallBack _callback = null;

    public override void Awake()
	{
		base.Awake();

		if(this._RewardListInstance == null) return;
		
		this._RewardListInstance.EventUpdate = this._UpdateRewardListSlot;
		this._RewardListInstance.EventGetItemCount = this._GetRewardElementCount;
	}

    public override void OnEnable()
    {
        base.OnEnable();
        if (kRootObj != null)
            kRootObj.transform.localScale = Vector3.one;
    }

    public void InitPopup(string Title, string Msg, List<RewardData> rewards, OnMessageRewardCallBack callback)
    {
        //StopCoroutine(FadeIn());
        //StopCoroutine(FadeOut());

        _callback = callback;
        _rewardList.AddRange(rewards);

        FLocalizeString.SetLabel(kTitleLabel, Title);
        FLocalizeString.SetLabel(kTextLabel, Msg);

        //  보상 아이콘 스크롤 갱신을 위한 코드
        _RewardListInstance.SetListInactive();
        _RewardListInstance.ScrollView.contentPivot = _rewardList.Count > 7 ? UIWidget.Pivot.Left : UIWidget.Pivot.Center;

        _RewardListInstance.UpdateList();

        //StartCoroutine(FadeIn());
    }
    /*
    public override void SetUIActive(bool bActive, bool bAnimation = true)
    {
        base.SetUIActive(bActive, bAnimation);

        if(!bActive && bAnimation)
        {
            StartCoroutine(FadeOut());
        }

    }
    */
    public override void OnUIOpen()
    {
        base.OnUIOpen();

        _RewardListInstance.Panel.alpha = 1.0f;
    }


    private void _UpdateRewardListSlot(int index, GameObject slotObject)
	{
		do
        {
            UIRewardListSlot rewardSlot = slotObject.GetComponent<UIRewardListSlot>();
            if (rewardSlot == null) break;
            rewardSlot.ParentGO = gameObject;
            
            if (0 <= index && _rewardList.Count > index)
                rewardSlot.UpdateSlot(_rewardList[index], false);

            //Do UpdateListSlot
        } while(false);
    }
	
	private int _GetRewardElementCount()
	{
        return _rewardList.Count; //TempValue
	}
	
	public void OnClick_CloseBtn()
    {
        
        OnClickClose();
        
    }

    public override void OnClickClose()
    {
        if (IsPlayAnimtion())
            return;

        _rewardList.Clear();

        if (_callback != null)
            _callback();
        base.OnClickClose();
    }
    /*
    private IEnumerator FadeIn()
    {
        _RewardListInstance.Panel.alpha = 0.0f;
        yield return new WaitForSeconds(0.5f);
        float f = 0.0f;
        while (f < 1.0f)
        {
            f += (Time.deltaTime * 2.5f);
            _RewardListInstance.Panel.alpha = f;
            yield return null;
        }
        _RewardListInstance.Panel.alpha = 1.0f;
    }

    private IEnumerator FadeOut()
    {
        float f = _RewardListInstance.Panel.alpha;
        while (f > 0.0f)
        {
            f -= (Time.deltaTime * 2.5f);
            _RewardListInstance.Panel.alpha = f;
            yield return null;
        }
        _RewardListInstance.Panel.alpha = 0.0f;
    }
    */
    //public override bool IsBackButton()
    //{
    //    OnClick_CloseBtn();
    //    return false;
    //}
}
