
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIGameResultPanel : FComponent
{
    public UILabel kTitleLabel;
    public UILabel kTitleLabel2;
    public UILabel kTitleLabel3;

    public UILabel kComboLabel;
    public UILabel kTimeLabel;
    
    public GameObject kStageInfo;
    public List<GameObject> kStageInfoList;
    public GameObject kMissionInfo;
    public List<GameObject> kMissionStarList;
    public List<GameObject> kMissionInfoList;
    public List<UILabel> kMissionLabelList;
    public List<GameObject> kMissionClearList;

    public GameObject kTimeAttackInfo;
    public GameObject kRecordNew;
    public UILabel kRecordLabel;
    //public GameObject kMissionCompletePopup;

    public GameObject kConfirm;
    public UIButton kConfirmBtn;

    [Header("[Tower]")]
    public GameObject               AllyInfo;
    public List<UIUserCharListSlot> ListAllyCharInfo;

    [Header( "[For raid]" )]
    [SerializeField] private UILabel _RaidStepLabel;

    private int _starcount = 0;
    private World.EMissionClearStatus[] m_missionCompletes = new World.EMissionClearStatus[3];
    private bool _bcashconfirm = false;

    private Coroutine _corClearUI = null;


	public override void OnEnable() {
		base.OnEnable();

		StopCoroutine( "StartStage" );
		StopCoroutine( "StartTimeAttack" );

		_bcashconfirm = false;

		if( World.Instance.StageType != eSTAGETYPE.STAGE_TIMEATTACK && World.Instance.StageType != eSTAGETYPE.STAGE_TOWER ) {
			if( GameInfo.Instance.netFlag ) {
				if( GameInfo.Instance.PktProduct != null && GameInfo.Instance.PktProduct.goodsInfos_.Count != 0 ) {
					for( int i = 0; i < GameInfo.Instance.PktProduct.goodsInfos_.Count; i++ ) {
                        if( GameInfo.Instance.PktProduct.goodsInfos_[i].type_ == eGOODSTYPE.CASH ) {
                            _bcashconfirm = true;
                        }
					}
				}
			}
			else {
				if( GameInfo.Instance.GameResultData.Goods[(int)eGOODSTYPE.CASH] != 0 ) {
					_bcashconfirm = true;
				}
			}
		}

		if( World.Instance.StageType == eSTAGETYPE.STAGE_TIMEATTACK ) {
			_corClearUI = StartCoroutine( "StartTimeAttack" );
		}
		else if( World.Instance.StageType == eSTAGETYPE.STAGE_TOWER || World.Instance.StageType == eSTAGETYPE.STAGE_RAID ) {
			_corClearUI = StartCoroutine( "StartTowerStage" );
		}
		else {
			_corClearUI = StartCoroutine( "StartStage" );
		}
	}

	private void Update() {
        if( null == _corClearUI ) {
            return;
        }

		if( AppMgr.Instance.CustomInput.GetButtonDown( BaseCustomInput.eKeyKind.Select ) ) {
			StopCoroutine( _corClearUI );
			_corClearUI = null;

			if( World.Instance.StageType == eSTAGETYPE.STAGE_TIMEATTACK ) {
				SkipTimeAttack();
			}
			else if( World.Instance.StageType == eSTAGETYPE.STAGE_TOWER || World.Instance.StageType == eSTAGETYPE.STAGE_RAID ) {
				SkipTowerStage();
			}
			else {
				SkipStage();
			}

			AppMgr.Instance.CustomInput.ShowCursor( true );
		}
	}

	public override void Renewal( bool bChildren ) {
		base.Renewal( bChildren );
		TimeSpan ts = TimeSpan.FromSeconds(World.Instance.ProgressTime);

		int stageNameId = 0;
		if( World.Instance.StageType == eSTAGETYPE.STAGE_TOWER ) {
			WorldStage worldStage = World.Instance as WorldStage;
			stageNameId = worldStage.TowerStageData.Name;
		}
		else {
			stageNameId = World.Instance.StageData.Name;
		}

		kTitleLabel2.textlocalize = kTitleLabel3.textlocalize = kTitleLabel.textlocalize = FLocalizeString.Instance.GetText( stageNameId );
		kComboLabel.textlocalize = World.Instance.Player.maxCombo.ToString();
		kTimeLabel.textlocalize = string.Format( "{0:D2}:{1:D2}", ts.Minutes, ts.Seconds );

        _RaidStepLabel.SetActive( false );

        if( World.Instance.StageType == eSTAGETYPE.STAGE_TIMEATTACK ) {
			kRecordLabel.textlocalize = GameSupport.GetTimeHighestScore( (int)ts.TotalMilliseconds );
		}
		else {
			if( World.Instance.StageType != eSTAGETYPE.STAGE_TOWER && World.Instance.StageType != eSTAGETYPE.STAGE_RAID ) {
				AllyInfo.SetActive( false );
				kMissionInfo.SetActive( false );

				_starcount = 0;
				m_missionCompletes[0] = World.Instance.Mission01;
				m_missionCompletes[1] = World.Instance.Mission02;
				m_missionCompletes[2] = World.Instance.Mission03;

				RenewalMission( 0, World.Instance.StageData.Mission_00 );
				RenewalMission( 1, World.Instance.StageData.Mission_01 );
				RenewalMission( 2, World.Instance.StageData.Mission_02 );

				for( int i = 0; i < kMissionInfoList.Count; i++ )
					kMissionInfoList[i].gameObject.SetActive( false );
			}
			else {
                if( World.Instance.StageType != eSTAGETYPE.STAGE_RAID ) {
                    kTitleLabel2.textlocalize = kTitleLabel3.textlocalize = kTitleLabel.textlocalize = string.Format( FLocalizeString.Instance.GetText( 60607 ), FLocalizeString.Instance.GetText( stageNameId ) );
                }
                else {
                    kTitleLabel2.textlocalize = kTitleLabel3.textlocalize = kTitleLabel.textlocalize = FLocalizeString.Instance.GetText( stageNameId );
                }

				AllyInfo.SetActive( false );
				kMissionInfo.SetActive( false );

				if( World.Instance.StageType == eSTAGETYPE.STAGE_TOWER ) {
					List<AllyPlayerData> list = GameSupport.GetAllyPlayerDataList();
					for( int i = 0; i < ListAllyCharInfo.Count; i++ ) {
						if( i >= list.Count ) {
							break;
						}

						ListAllyCharInfo[i].CharSelectFlag = eCharSelectFlag.ARENATOWER_STAGE;
						ListAllyCharInfo[i].UpdateSlot( list[i].GetCharDataOrNull() );
					}
				}
				else {
                    _RaidStepLabel.SetActive( true );
                    _RaidStepLabel.textlocalize = string.Format( "{0} {1}", FLocalizeString.Instance.GetText( 3318 ), GameInfo.Instance.SelectedRaidLevel.ToString( "D2" ) );

                    for( int i = 0; i < ListAllyCharInfo.Count; i++ ) {
                        if( i >= World.Instance.ListPlayer.Count ) {
                            ListAllyCharInfo[i].SetActive( false );
                            break;
                        }

                        if( ListAllyCharInfo[i] == null || World.Instance.ListPlayer[i] == null ) {
                            ListAllyCharInfo[i].SetActive( false );
                            continue;
                        }

                        ListAllyCharInfo[i].SetActive( true );
                        ListAllyCharInfo[i].CharSelectFlag = eCharSelectFlag.RAID;
                        ListAllyCharInfo[i].UpdateThreePlayerSlot( World.Instance.ListPlayer[i] );
                    }
                }
			}
		}
	}

	private void RenewalMission(int index, int missionid)
    {
        GameClientTable.StageMission.Param missiondata = GameInfo.Instance.GameClientTable.StageMissions.Find(x => x.ID == missionid);
        if (missiondata == null)
            return;

        //kMissionLabelList[index].gameObject.SetActive(true);
        kMissionLabelList[index].textlocalize = string.Format(FLocalizeString.Instance.GetText(missiondata.Desc), missiondata.ConditionValue);
        _starcount += 1;

        kMissionStarList[index].gameObject.SetActive(false);
        kMissionClearList[index].gameObject.SetActive(false);

        StageClearData clearData = GameInfo.Instance.StageClearList.Find(x => x.TableID == World.Instance.StageData.ID);
        if (m_missionCompletes[index] != World.EMissionClearStatus.FAIL)
        {
            kMissionStarList[index].gameObject.SetActive(true);

            if(clearData != null)
                kMissionClearList[index].gameObject.SetActive(true);
        }
    }

    private IEnumerator StartStage()
    {
        StageClearData stageClearData = GameInfo.Instance.StageClearList.Find(x => x.TableID == World.Instance.StageData.ID);

        kConfirm.SetActive(false);
        kConfirmBtn.gameObject.SetActive(false);
        kTimeAttackInfo.SetActive(false);

        float opentime = GetOpenAniTime();
        yield return new WaitForSeconds(opentime+0.2f);


        //yield return new WaitForSeconds(0.2f);

        kMissionInfo.SetActive(true);

        WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
        for (int i = 0; i < _starcount; i++)
        {
            kMissionInfoList[i].SetActive(true);

            if(m_missionCompletes[i] == World.EMissionClearStatus.CLEARED)
            {
                kMissionStarList[i].gameObject.SetActive(true);

                UISprite spr = kMissionStarList[i].GetComponent<UISprite>();
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1.0f);
            }

            yield return waitForSeconds;

            if (m_missionCompletes[i] == World.EMissionClearStatus.CLEAR)
            {
                kMissionStarList[i].gameObject.SetActive(true);

                SoundManager.Instance.PlayUISnd(22);
                kMissionInfoList[i].GetComponent<UIResultSlot>().PlayClear();

                yield return waitForSeconds;
            }
        }

        ShowRewardPopup();

        kConfirm.SetActive(true);
        kConfirmBtn.gameObject.SetActive(true);

        yield return null;
    }

    private void SkipStage()
    {
        this.SetEndFrameAnimation((int)eCOUNT.NONE);
        kMissionInfo.SetActive(true);

        for (int i = 0; i < _starcount; i++)
        {
            kMissionInfoList[i].SetActive(true);

            if (m_missionCompletes[i] == World.EMissionClearStatus.CLEARED)
            {
                kMissionStarList[i].gameObject.SetActive(true);

                UISprite spr = kMissionStarList[i].GetComponent<UISprite>();
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1.0f);
            }

            if (m_missionCompletes[i] == World.EMissionClearStatus.CLEAR)
            {
                kMissionStarList[i].gameObject.SetActive(true);

                SoundManager.Instance.PlayUISnd(22);
                kMissionInfoList[i].GetComponent<UIResultSlot>().PlayClear();

            }
        }

        ShowRewardPopup();

        kConfirm.SetActive(true);
        kConfirmBtn.gameObject.SetActive(true);
    }


    private IEnumerator StartTimeAttack()
    {
        kConfirm.SetActive(false);
        kConfirmBtn.gameObject.SetActive(false);
        kMissionInfo.SetActive(false);
        kRecordNew.SetActive(false);

        float opentime = GetOpenAniTime();
        yield return new WaitForSeconds(opentime + 0.2f);

        kTimeAttackInfo.SetActive(true);
        
        int prevClearScore = 0;
        var prevScore = UIValue.Instance.GetValue(UIValue.EParamType.TimeAttackClearScore);
        if (prevScore != null)
            prevClearScore = (int)prevScore;

        var timeattackcleardata = GameInfo.Instance.GetTimeAttackClearData(World.Instance.StageData.ID);
        if(prevClearScore == 0)
        {
            kRecordNew.SetActive(true);
        }
        else
        {
            if (timeattackcleardata.HighestScore < prevClearScore)
                kRecordNew.SetActive(true);
        }
        

        yield return new WaitForSeconds(0.2f);

        ShowRewardPopup();

        kConfirm.SetActive(true);
        kConfirmBtn.gameObject.SetActive(true);

        yield return null;
    }

    private void SkipTimeAttack()
    {
        this.SetEndFrameAnimation((int)eCOUNT.NONE);

        kTimeAttackInfo.SetActive(true);

        int prevClearScore = 0;
        var prevScore = UIValue.Instance.GetValue(UIValue.EParamType.TimeAttackClearScore);
        if (prevScore != null)
            prevClearScore = (int)prevScore;

        var timeattackcleardata = GameInfo.Instance.GetTimeAttackClearData(World.Instance.StageData.ID);
        if (prevClearScore == 0)
        {
            kRecordNew.SetActive(true);
        }
        else
        {
            if (timeattackcleardata.HighestScore < prevClearScore)
                kRecordNew.SetActive(true);
        }

        ShowRewardPopup();

        kConfirm.SetActive(true);
        kConfirmBtn.gameObject.SetActive(true);
    }

    private IEnumerator StartTowerStage()
    {
        kConfirm.SetActive(false);
        kConfirmBtn.gameObject.SetActive(false);
        kTimeAttackInfo.SetActive(false);
        kRecordNew.SetActive( GameInfo.Instance.IsNewRaidReocrd );

        float opentime = GetOpenAniTime();
        yield return new WaitForSeconds(opentime + 0.2f);

        AllyInfo.SetActive(true);

        ShowRewardPopup();

        kConfirm.SetActive(true);
        kConfirmBtn.gameObject.SetActive(true);

        yield return null;
    }

    private void SkipTowerStage()
    {
        this.SetEndFrameAnimation((int)eCOUNT.NONE);

        kRecordNew.SetActive( GameInfo.Instance.IsNewRaidReocrd );
        AllyInfo.SetActive(true);

        ShowRewardPopup();

        kConfirm.SetActive(true);
        kConfirmBtn.gameObject.SetActive(true);
    }

    private void ShowRewardPopup()
    {
        if (_bcashconfirm)
        {
            GameUIManager.Instance.ShowUI("GameRewardPopup", true);
            _bcashconfirm = false;
        }
    }

    public void OnClick_Exit()
    {
        World.Instance.OnEndResult();
    }
}