using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventRulePopup : FComponent
{
    public enum eRulePopupType
    {
        NONE = 0,
        EVENT_RULE = 1,
        ARENA_RULE = 2,
        FAVOR_RULE = 3,
        RAID_RULE = 4,
    }

    public UILabel kTitleLabel;
    //public UISprite kEventRuleSpr;
    public UITexture kEventRuleTex;
    public UISprite kArenaRuleSpr;
    public List<string> kArenaNameList;
    public List<string> kFavorNameList;
    public GameObject kSelectCircle;        //배너밑에 동그란 게임오브젝트
    public GameObject kReleaseCircle;       //배너밑에 동그란  게임오브젝트 선택된 배너에 표시
    public int kCircleInterval = 3;

    public GameObject kEventRuleMaskObj;

    [Header("Story")]
    public GameObject kStoryObj;
    public List<UIEventRuleUnit> kStoryUnitList;
    
    [Header("Exchange")]
    public GameObject kExchangeObj;
    public List<UIEventRuleUnit> kExchangeUnitList;

    [Header("[Raid]")]
    [SerializeField] private UISprite       _RaidRuleSpr;
    [SerializeField] private List<string>   _RaidNameList;
    
    private List<string> kSprNameList = new List<string>();
    private List<GameObject> _circlelist = new List<GameObject>();
    private int _index;
    private eRulePopupType _ruleType = eRulePopupType.NONE;
    
    private int _currentIndex;
    private eEventRewardKind _eventRewardKind = eEventRewardKind._NONE_;
    
    
	public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        _index = 0;
        kEventRuleTex.gameObject.SetActive(false);
        kArenaRuleSpr.gameObject.SetActive(false);
        _RaidRuleSpr.SetActive( false );

        _currentIndex = _index;
        kEventRuleMaskObj.gameObject.SetActive(false);
        kStoryObj.SetActive(false);
        kExchangeObj.SetActive(false);
        foreach (UIEventRuleUnit unit in kStoryUnitList)
        {
            unit.Init();
            unit.SetActive(false);
        }
        
        foreach (UIEventRuleUnit unit in kExchangeUnitList)
        {
            unit.Init();
            unit.SetActive(false);
        }
        
        kSprNameList.Clear();

        _ruleType = (eRulePopupType)UIValue.Instance.GetValue(UIValue.EParamType.RulePopupType);

        if (_ruleType == eRulePopupType.EVENT_RULE)
        {
            kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(1098);
            
            int eventId = (int)UIValue.Instance.GetValue(UIValue.EParamType.EventID);
            
            kEventRuleMaskObj.gameObject.SetActive(true);
            GameTable.EventSet.Param eventTableData = GameInfo.Instance.GameTable.FindEventSet(eventId);
            _eventRewardKind = (eEventRewardKind)eventTableData.EventType;
            switch (_eventRewardKind)
            {
                case eEventRewardKind.RESET_LOTTERY:
                    kStoryObj.SetActive(true);
                    foreach (UIEventRuleUnit unit in kStoryUnitList)
                    {
                        unit.SetEventId(eventId);
                    }
                    break;
                case eEventRewardKind.EXCHANGE:
                    kExchangeObj.SetActive(true);
                    foreach (UIEventRuleUnit unit in kExchangeUnitList)
                    {
                        unit.SetEventId(eventId);
                    }
                    break;
            }
        }
        else if (_ruleType == eRulePopupType.ARENA_RULE)
        {
            kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(1462);
            kArenaRuleSpr.gameObject.SetActive(true);
            for (int i = 0; i < kArenaNameList.Count; i++)
            {
                kSprNameList.Add(kArenaNameList[i]);
            }
        }
        else if (_ruleType == eRulePopupType.FAVOR_RULE)
        {
            kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(3263);
            kArenaRuleSpr.gameObject.SetActive(true);
            for (int i = 0; i < kFavorNameList.Count; i++)
            {
                kSprNameList.Add(kFavorNameList[i]);
            }
        }
        else if( _ruleType == eRulePopupType.RAID_RULE ) {
            kTitleLabel.textlocalize = FLocalizeString.Instance.GetText( 1854 );
            _RaidRuleSpr.SetActive( true );

            for( int i = 0; i < _RaidNameList.Count; i++ ) {
                kSprNameList.Add( _RaidNameList[i] );
            }
        }

        CreateReleaserCircleGameObject();
    }

	public override void Renewal( bool bChildren ) {
		base.Renewal( bChildren );

        if( _ruleType == eRulePopupType.EVENT_RULE ) {
            switch( _eventRewardKind ) {
                case eEventRewardKind.RESET_LOTTERY:
                    kStoryUnitList[_currentIndex].SetActive( false );
                    _currentIndex = _index;
                    if( _currentIndex < kStoryUnitList.Count ) {
                        kStoryUnitList[_currentIndex].SetActive( true );
                    }
                    break;

                case eEventRewardKind.EXCHANGE:
                    kExchangeUnitList[_currentIndex].SetActive( false );
                    _currentIndex = _index;
                    if( _currentIndex < kExchangeUnitList.Count ) {
                        kExchangeUnitList[_currentIndex].SetActive( true );
                    }
                    break;
            }
        }
        else if( _ruleType == eRulePopupType.ARENA_RULE || _ruleType == eRulePopupType.FAVOR_RULE ) {
            kArenaRuleSpr.spriteName = kSprNameList[_index];
        }
        else if( _ruleType == eRulePopupType.RAID_RULE ) {
            _RaidRuleSpr.spriteName = kSprNameList[_index];
        }

		if( _circlelist.Count != 0 )
			kSelectCircle.transform.localPosition = _circlelist[_index].transform.localPosition;
	}

	public void OnClick_Arrow_LBtn()
    {
        int maxCount = kSprNameList.Count;
        if (_ruleType == eRulePopupType.EVENT_RULE)
        {
            switch (_eventRewardKind)
            {
                case eEventRewardKind.RESET_LOTTERY:
                    maxCount = kStoryUnitList.Count;
                    break;
                case eEventRewardKind.EXCHANGE:
                    maxCount = kExchangeUnitList.Count;
                    break;
            }
        }
        
        int nTemp = _index - 1;
        if (nTemp < 0)
        {
            nTemp = maxCount - 1;
            if (nTemp < 0)
            {
                nTemp = (int)eCOUNT.NONE;
            }
        }

        _index = nTemp;
        
        Renewal(true);
    }

    public void OnClick_Arrow_RBtn()
    {
        int maxCount = kSprNameList.Count;
        if (_ruleType == eRulePopupType.EVENT_RULE)
        {
            switch (_eventRewardKind)
            {
                case eEventRewardKind.RESET_LOTTERY:
                    maxCount = kStoryUnitList.Count;
                    break;
                case eEventRewardKind.EXCHANGE:
                    maxCount = kExchangeUnitList.Count;
                    break;
            }
        }
        
        int temp = _index + 1;
        if (temp >= maxCount)
        {
            temp = (int)eCOUNT.NONE;
        }
        
        _index = temp;
        
        Renewal(true);
    }

    public void OnClick_CloseBtn()
    {
        OnClickClose();
    }

    public void OnClick_CircleBtn(GameObject _go)
    {
        int currentIndex = -1;

        int max = _circlelist.Count;
        for (int i = 0; i < max; i++)
        {
            if (_go == _circlelist[i])
            {
                currentIndex = i;
            }
        }

        if (currentIndex != -1)
        {
            _index = currentIndex;
            Renewal(true);
        }
    }


    private void CreateReleaserCircleGameObject()
    {
        int maxCount = kSprNameList.Count;
        if (_ruleType == eRulePopupType.EVENT_RULE)
        {
            switch (_eventRewardKind)
            {
                case eEventRewardKind.RESET_LOTTERY:
                    maxCount = kStoryUnitList.Count;
                    break;
                case eEventRewardKind.EXCHANGE:
                    maxCount = kExchangeUnitList.Count;
                    break;
            }
        }
        
        int needCount = maxCount - _circlelist.Count;
        int minusCount = -needCount;

        if (needCount == 0)
            return;

        if (_circlelist.Count >= maxCount)
        {

            for (int i = 0; i < minusCount; i++)
            {
                GameObject go = null;
                if (_circlelist.Count != 0)
                {
                    go = _circlelist[0];
                    Destroy(go);
                    _circlelist.Remove(go);
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

                    _circlelist.Add(go);
                    go.SetActive(true);
                }
            }
            kReleaseCircle.SetActive(false);
        }
        
        if (_circlelist.Count == 0)
            return;

        float width = (float)_circlelist[0].GetComponent<UISprite>().width + kCircleInterval;
        float totalwidth = width * _circlelist.Count;
        float fstartx = -(totalwidth / 2.0f) + (width / 2);
        for (int i = 0; i < _circlelist.Count; i++)
        {
            _circlelist[i].transform.localPosition = new Vector3(fstartx + ((float)width * i), kSelectCircle.transform.localPosition.y, 1.0f);
            //if (i == 0) SetPositionSelect(i);
        }
        
    }

}
