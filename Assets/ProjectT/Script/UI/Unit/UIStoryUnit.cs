using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStoryUnit : FUnit {

    public int kNum;
    
    public GameObject kOpen;
    public GameObject kClose;
    public UISprite kBGSpr;
    public List<UISprite> kStarBGList;
    public List<UISprite> kStarList;
    public UILabel kOpenNumLabel;
    public UILabel kCloseNumLabel;
    public UISprite kStorySpr;

    public GameObject m_goChaptherRoad = null;
    public GameObject m_goLastPlayMarker = null;

    private int _index;
    private GameTable.Stage.Param _tabledata;
    private StageClearData _stagecleardata;
    private bool _bactive;
    private UIButton _stageBtn;

    private StageClearData mStageClearData = null;

    public UISprite kCardTypeSpr;

    public GameObject kHandUnit;

    public bool IsActive { get { return _bactive;  } }
    public void UpdateSlot(int index, GameTable.Stage.Param tabledata ) 	//Fill parameter if you need
	{
        //인덱스, 데이터 갱신
        _index = index;
        _tabledata = tabledata;

        //1-1 첫스테이지면 조건없이 활성화
        if (tabledata.LimitStage == -1)
            SetStageOpened(true);
        else
        {
            //ClearList에 전 스테이지가 있는지 여부
            var limitStage = GameInfo.Instance.StageClearList.Find(x => x.TableID == _tabledata.LimitStage);
            if (limitStage == null)
            {
                SetStageOpened(false);
                m_goLastPlayMarker.SetActive(false);
            }
            else
            {
                SetStageOpened(true);
            }
        }

        kStorySpr.gameObject.SetActive(false);
        if( GameSupport.IsShowStoryStage(tabledata) )
            kStorySpr.gameObject.SetActive(true);

        SetClearMissions();
    }

    public void OnClick_Slot()
	{
        if(!_bactive)
        {
            return;
        }
        UIValue.Instance.SetValue(UIValue.EParamType.StageID, _tabledata.ID);
        LobbyUIManager.Instance.ShowUI("StageDetailPopup", true);
    }

    public bool IsAllClearMission()
    {
        if(mStageClearData == null)
        {
            return false;
        }

        return mStageClearData.IsClearAll();
    }

    void SetStageOpened(bool bOpened)
    {
        //텍스트 설정
        kOpenNumLabel.textlocalize = _tabledata.Section.ToString("D2");
        kCloseNumLabel.textlocalize = _tabledata.Section.ToString("D2");

        //Open, Close 오브젝트 갱신
        kOpen.SetActive(bOpened);
        kClose.SetActive(!bOpened);
        _bactive = bOpened;

        //해당 스테이지가 교환형 이벤트 라면 추가 설정 필요.
        GameTable.EventSet.Param eventSetData = GameInfo.Instance.GameTable.EventSets.Find(x => x.EventID == _tabledata.TypeValue && _tabledata.StageType == (int)eSTAGETYPE.STAGE_EVENT);
        if (eventSetData != null)
        {
            if (eventSetData.EventType == (int)eEventRewardKind.EXCHANGE)
            {
                kOpenNumLabel.textlocalize = FLocalizeString.Instance.GetText(_tabledata.Name);
                kCloseNumLabel.textlocalize = FLocalizeString.Instance.GetText(_tabledata.Name);

                if (_tabledata.Condi_Type != (int)eSTAGE_CONDI.NONE && kCardTypeSpr != null)
                {
                    kCardTypeSpr.spriteName = GameSupport.GetCardTypeSpriteName((eSTAGE_CONDI)_tabledata.Condi_Type);
                }
            }
        }

        if (_bactive)
        {
            if( _tabledata.StageType == (int)eSTAGETYPE.STAGE_EVENT )
            {
                if( _tabledata.ID == (int)UIValue.Instance.GetValue(UIValue.EParamType.EventStageID) )
                    m_goLastPlayMarker.SetActive(true);
                else
                    m_goLastPlayMarker.SetActive(false);
            }
            else if(_tabledata.StageType == (int)eSTAGETYPE.STAGE_PVP_PROLOGUE)
            {
                if (_tabledata.ID == (int)UIValue.Instance.GetValue(UIValue.EParamType.ArenaPrologueStageID))
                    m_goLastPlayMarker.SetActive(true);
                else
                    m_goLastPlayMarker.SetActive(false);
            }
            else
            {
                if (_tabledata.ID == (int)UIValue.Instance.GetValue(UIValue.EParamType.StoryStageID))
                {
                    m_goLastPlayMarker.SetActive(true);
                    if (kHandUnit != null)
                    {
                        kHandUnit.gameObject.SetActive(GameSupport.ShowLobbyStageHand(_tabledata.ID));
                    }
                    
                }
                else
                    m_goLastPlayMarker.SetActive(false);
            }
            /*
            if (GameInfo.Instance.StageClearList.Find(x => x.TableID == _tabledata.ID) == null)
                m_goLastPlayMarker.SetActive(true);
            else
                m_goLastPlayMarker.SetActive(false);
            */
        }
        else
        {
            m_goLastPlayMarker.SetActive(false);
        }

        //_bactive에 따른 배경 이미지 변경 - 더 좋은 방법 구상중...
        if (kBGSpr.spriteName.Equals("btn_MainStoryList") || kBGSpr.spriteName.Equals("btn_MainStoryList_off"))
        {
            if (_bactive)
                kBGSpr.spriteName = "btn_MainStoryList";
            else
                kBGSpr.spriteName = "btn_MainStoryList_off";
        }
        else if(kBGSpr.spriteName.Equals("btn_MainStoryList") || kBGSpr.spriteName.Equals("btn_MainStoryList_off"))
        {
            if (_bactive)
                kBGSpr.spriteName = "btn_MainStoryList";
            else
                kBGSpr.spriteName = "btn_MainStoryList_off";
        }

        //위 배경이미지 변경을 한대로 버튼의 이미지도 바꿔줘야한다.
        if (_stageBtn == null)
            _stageBtn = this.GetComponent<UIButton>();
        if (_stageBtn != null)
            _stageBtn.SetButtonSpriteName(kBGSpr.spriteName);

        //라인 오브젝트가 있다면 활성화 여부에 따라 활성화
        if (m_goChaptherRoad != null)
        {
            m_goChaptherRoad.SetActive(_bactive);
        }

    }

    void SetClearMissions()
    {
        //미션이 몇개있는지 검색 후 적용
        kStarBGList[0].gameObject.SetActive(_tabledata.Mission_00 == -1 ? false : true);
        kStarBGList[1].gameObject.SetActive(_tabledata.Mission_01 == -1 ? false : true);
        kStarBGList[2].gameObject.SetActive(_tabledata.Mission_02 == -1 ? false : true);

        //미션 클리어 오브젝트 일단 모두 끈다..
        for (int i = 0; i < kStarList.Count; i++)
            kStarList[i].gameObject.SetActive(false);

        mStageClearData = GameInfo.Instance.StageClearList.Find(x => x.TableID == _tabledata.ID);
        if (mStageClearData != null)
        {
            for (int i = 0; i < mStageClearData.GetClearCount(); i++)
            {
                kStarList[i].gameObject.SetActive(true);
            }
        }
    }
}
