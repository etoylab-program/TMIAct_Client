using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//
//  튜토리얼 관련
//
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
public class UITutorialPopup : FComponent
{
    public enum eNEXTTYPE
    {
        BG_CLICK = 0,
        MASK_CLICK,
        CODE,
        
    };
    public FMask kMask;
    public List<GameObject> kInfoList;
    public GameObject kDesc;
    public UITexture kBlindTexture;
    public UISprite kCharSpr;
    public UILabel kText;
    public GameObject kTextBGSpr;
    public UIButton kMaskBtn;
    public UIButton kBGBtn;
    public UISprite kRound;
    public UISprite kHand;
    public UISprite kTouchSpr;
    public UIButton BtnTutorialSkip;

    private int _tutorialid = -1;
    private GameClientTable.Tutorial.Param _tutorialdata;
    private float oldtimescale;
    private bool mLock = false;

    public GameClientTable.Tutorial.Param TutorialData { get { return _tutorialdata; } }


    public override void OnEnable()
    {
        base.OnEnable();
        kMask.gameObject.SetActive(true);
        
        int tutorialstate = (int)UIValue.Instance.GetValue(UIValue.EParamType.TutorialState);
        int tutorialstep = (int)UIValue.Instance.GetValue(UIValue.EParamType.TutorialStep);

        var tutorialdata = GameInfo.Instance.GameClientTable.FindTutorial(x => x.StateID == tutorialstate && x.Step == tutorialstep);
        if (tutorialdata != null)
            _tutorialid = tutorialdata.ID;

        for (int i = 0; i < kInfoList.Count; i++)
            kInfoList[i].SetActive(false);

        mLock = false;

        if (GameSupport.IsTutorial())
            BtnTutorialSkip.gameObject.SetActive(AppMgr.Instance.TutorialSkipFlag);

        /*int w = 1280;
        int h = (int)(1280.0f * ((float)Screen.height / (float)Screen.width));

        if (AppMgr.Instance.WideScreen)
        {
            float f = ((float)Screen.width / (float)Screen.height);
            w = (int)((float)Screen.width / f);
            h = (int)((float)w * ((float)Screen.height / (float)Screen.width));
        }
        RenderTexture rendertex = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        kBlindTexture.mainTexture = rendertex;

        kBlindTexture.width = w;
        kBlindTexture.height = h;


        kMask.SetMaskRoot(rendertex);*/

        RenderTexture rendertex;
        Vector2 blientSize = kMask.SetViewMaskRoot(out rendertex);
        kBlindTexture.mainTexture = rendertex;

        kBlindTexture.width = (int)blientSize.x;
        kBlindTexture.height = (int)blientSize.y;


        Invoke("SetTutorial", 0.5f);
    }

	public void Start() {
		oldtimescale = Time.timeScale;
	}

	public override void OnDisable() {
		if ( AppMgr.Instance.IsQuit || kMask == null || kMask.gameObject == null ) {
			return;
		}

		base.OnDisable();

		kMask.gameObject.SetActive( false );
		_tutorialdata = null;
	}

	public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
    }

   
    private Transform mPopupObj = null;
    private int mMaskIndex = -1;

    void SetTutorial()
    {
        _tutorialdata = GameInfo.Instance.GameClientTable.FindTutorial(x=>x.ID == _tutorialid);
        if(_tutorialdata == null)
        {
            SetUIActive(false);
            return;
        }
       
        Time.timeScale = _tutorialdata.TimeScale;

        for( int i = 0; i < kInfoList.Count; i++ )
            kInfoList[i].SetActive(true);

        kDesc.SetActive(true);
        kDesc.transform.localPosition = new Vector3(0.0f, _tutorialdata.DescPosY, 0.0f);
        kText.textlocalize = FLocalizeString.Instance.GetText(_tutorialdata.Desc);//_tutorialdata.Text;
        kText.pivot = (UIWidget.Pivot)_tutorialdata.DescPivot;//UIWidget.Pivot.Center;
        kText.transform.localPosition = Vector3.zero;

        

        if (_tutorialdata.Mask)
        {
            kTouchSpr.gameObject.SetActive(false);
            kRound.gameObject.SetActive(true);
            kHand.gameObject.SetActive(true);
            for (int i = 0; i < kMask.kMaskList.Count; i++)
                kMask.kMaskList[i].gameObject.SetActive(false);
            mMaskIndex = _tutorialdata.MaskIndex;
            kMask.kMaskList[mMaskIndex].gameObject.SetActive(true);

            mPopupObj = null;
            var popup = LobbyUIManager.Instance.GetUI(_tutorialdata.Popup);
            if (popup != null)
            {
                mPopupObj = popup.transform.Find(_tutorialdata.PopupObject);
                if (mPopupObj != null)
                {
                    SetMaskPosition();   
                }
            }
          
        }
        else
        {
            kTouchSpr.gameObject.SetActive(true);
            kRound.gameObject.SetActive(false);
            for( int i = 0; i < kMask.kMaskList.Count; i++ )
                kMask.kMaskList[i].gameObject.SetActive(false);

        }

        kCharSpr.transform.localPosition = new Vector3(_tutorialdata.CharPosX, 0.0f, 0.0f);

        if ( _tutorialdata.HandX == 0 && _tutorialdata.HandY == 0 )
        {
            kHand.gameObject.SetActive(false);
        }
        else
        {
            
            kHand.gameObject.SetActive(true);
            kHand.gameObject.GetComponent<TweenPosition>().enabled = false;
            kHand.transform.localPosition = new Vector3(kMaskBtn.transform.localPosition.x + _tutorialdata.HandX, kMaskBtn.transform.localPosition.y + _tutorialdata.HandY, 0.0f);
             
            if (_tutorialdata.HandMX != 0 || _tutorialdata.HandMY != 0)
            {
                TweenPosition.SetTweenPosition(kHand.gameObject.GetComponent<TweenPosition>(), (UITweener.Style)_tutorialdata.HandStyle, kHand.transform.localPosition, new Vector3(kMaskBtn.transform.localPosition.x + _tutorialdata.HandMX, kMaskBtn.transform.localPosition.y + _tutorialdata.HandMY, 0), _tutorialdata.HandSpeed, 0.0f, null);
                kHand.gameObject.GetComponent<TweenPosition>().Play(true);
            }

            kHand.transform.localRotation = Quaternion.Euler(0, 0, _tutorialdata.HandAngleZ);
        }


        if (_tutorialdata.NextType == (int)eNEXTTYPE.BG_CLICK )
        {
            kMaskBtn.gameObject.SetActive(false);
            kBGBtn.gameObject.SetActive(true);
        }
        if (_tutorialdata.NextType == (int)eNEXTTYPE.MASK_CLICK)
        {
            kMaskBtn.gameObject.SetActive(true);
            kBGBtn.gameObject.SetActive(false);
        }

        //튜토리얼 상태 최종확인
        //if(StageManager.Instance != null)
        //    StageManager.Instance.CheckTutorialCreateObject(tutorial);
    }

    private void SetMaskPosition()
    {
        if(mMaskIndex < 0 || mMaskIndex >= kMask.kMaskList.Count || mPopupObj == null)
        {
            return;
        }

        Camera worldCam = NGUITools.FindCameraForLayer((int)eLayer.LobbyCamera);
        Camera nguiCam = NGUITools.FindCameraForLayer((int)eLayer.UI);

        Vector3 v = nguiCam.WorldToViewportPoint(mPopupObj.transform.position);
        v.z = worldCam.orthographicSize;
        Vector3 f = worldCam.ViewportToWorldPoint(v);
        Vector3 fv = Utility.GetUIPosFrom3DPos(worldCam, (int)eLayer.UI, f);

        float maskx = fv.x;
        float masky = fv.y;

        kMaskBtn.transform.position = new Vector3(maskx, masky, 0.0f);
        kMaskBtn.GetComponent<UIWidget>().width = _tutorialdata.MaskWidth;
        kMaskBtn.GetComponent<UIWidget>().height = _tutorialdata.MaskHeight;
        kMaskBtn.GetComponent<BoxCollider>().size = new Vector3(_tutorialdata.MaskWidth, _tutorialdata.MaskHeight, 0.0f);

        kRound.transform.position = new Vector3(maskx, masky, 0.0f);
        //kRound.width = _tutorialdata.MaskWidth;
        //kRound.height = _tutorialdata.MaskHeight;

        kMask.kMaskList[mMaskIndex].transform.localPosition = kMaskBtn.transform.localPosition; // new Vector3(maskx, masky, 0.0f);
        kMask.kMaskList[mMaskIndex].width = _tutorialdata.MaskWidth;
        kMask.kMaskList[mMaskIndex].height = _tutorialdata.MaskHeight;
    }

    private void Update()
    {
        if (_tutorialdata == null || !_tutorialdata.Mask)
        {
            return;
        }

        SetMaskPosition();
    }

    public void OnClickMaskBtn()
    {
        if (_tutorialdata == null || mLock)
            return;

        if (_tutorialdata.NextType != (int)eNEXTTYPE.MASK_CLICK)
        {
            OnClickBG();
            return;
        }

        Debug.Log("OnClickMaskBtn");
        NextTutorial();
    }

    
    public void OnClickBG()
    {
        if (_tutorialdata == null || mLock)
            return;

        if (_tutorialdata.NextType != (int)eNEXTTYPE.BG_CLICK)
            return;

        Debug.Log("OnClickBG");
        NextTutorial();
    }

    void ActionTutorial()   //77889 튜토리얼
    {
        int tutorialstate = _tutorialdata.StateID;
        int tutorialstep = _tutorialdata.Step;

        if (tutorialstate == (int)eTutorialState.TUTORIAL_STATE_CardEquip)
        {
            GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_1_Tutorial_Card_Equip);
            if (tutorialstep == 6)
            {
                LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARMAIN);
            }
            else if (tutorialstep == 7)
            {
                var chardata = GameInfo.Instance.GetMainChar();

                UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, chardata.CUID);
                UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, chardata.TableID);
                UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.STATUS);
                LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);
            }
            else if (tutorialstep == 9)
            {
                UICharInfoPanel panel = LobbyUIManager.Instance.GetUI<UICharInfoPanel>("CharInfoPanel");
                panel.kMainTab.SetTab(3, SelectEvent.Click);
            }
            else if (tutorialstep == 11)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.CharEquipCardSlot, 0);
                LobbyUIManager.Instance.ShowUI("CharCardSeletePopup", true);
            }
            else if (tutorialstep == 12)
            {
                GameInfo.Instance.UserData.SetTutorial(_tutorialdata.SendPacket, 1);

                UICharCardSeletePopup popup = LobbyUIManager.Instance.GetUI<UICharCardSeletePopup>("CharCardSeletePopup");
                popup.OnClick_ChangeBtn();
            }
        }
        else if (tutorialstate == (int)eTutorialState.TUTORIAL_STATE_CardLevelUp)
        {
            GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_1_Tutorial_Card_LevelUp);
            if (tutorialstep == 2)
            {
                var chardata = GameInfo.Instance.GetMainChar();
                var carddata = GameInfo.Instance.GetCardData(chardata.EquipCard[0]);
                UIValue.Instance.SetValue(UIValue.EParamType.CardUID, carddata.CardUID);
                LobbyUIManager.Instance.ShowUI("CardInfoPopup", true);
            }
            else if (tutorialstep == 3)
            {
                UICardInfoPopup popup = LobbyUIManager.Instance.GetUI<UICardInfoPopup>("CardInfoPopup");
                popup.OnClick_LevelUpBtn();
            }
            else if (tutorialstep == 5)
            {
                UICardLevelUpPopup popup = LobbyUIManager.Instance.GetUI<UICardLevelUpPopup>("CardLevelUpPopup");
                popup.OnClick_AutoMatBtn();
            }
            else if (tutorialstep == 6)
            {
                GameInfo.Instance.UserData.SetTutorial(_tutorialdata.SendPacket, 1);

                UICardLevelUpPopup popup = LobbyUIManager.Instance.GetUI<UICardLevelUpPopup>("CardLevelUpPopup");
                popup.OnClick_LevelUpBtn();
            }
            else if (tutorialstep == 8)
            {
                LobbyUIManager.Instance.HomeBtnEvent();
            }
        }
        else if (tutorialstate == (int)eTutorialState.TUTORIAL_STATE_Stage2Join)
        {
            GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_1_Tutorial_1_2_Join);
            if (tutorialstep == 1)
            {
                LobbyUIManager.Instance.SetPanelType(ePANELTYPE.STORYMAIN);
            }
            else if (tutorialstep == 2)
            {
                UIStoryMainPanel panel = LobbyUIManager.Instance.GetUI<UIStoryMainPanel>("StoryMainPanel");
                panel.OnClick_MainStoryBtn();
            }
            else if (tutorialstep == 3)
            {
                UIStoryPanel panel = LobbyUIManager.Instance.GetUI<UIStoryPanel>("StoryPanel");
                panel.kStoryUnitList[1].OnClick_Slot();
            }
            else if (tutorialstep == 4)
            {
                GameInfo.Instance.UserData.SetTutorial((int)eTutorialState.TUTORIAL_STATE_Stage2Clear, 1);

                UIStageDetailPopup popup = LobbyUIManager.Instance.GetUI<UIStageDetailPopup>("StageDetailPopup");
                popup.OnClick_StartBtn();
            }
        }
        else if (tutorialstate == (int)eTutorialState.TUTORIAL_STATE_SkillSpt)
        {
            GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_2_Tutorial_Char_Skill_Leam);
            if (tutorialstep == 3)
            {
                LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARMAIN);
            }
            else if (tutorialstep == 4)
            {
                var chardata = GameInfo.Instance.GetMainChar();

                UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, chardata.CUID);
                UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, chardata.TableID);
                UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.STATUS);
                LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);
            }
            else if (tutorialstep == 5)
            {
                UICharInfoPanel panel = LobbyUIManager.Instance.GetUI<UICharInfoPanel>("CharInfoPanel");
                panel.kMainTab.SetTab(2, SelectEvent.Click);
            }
            else if (tutorialstep == 7)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.CharEquipSkillSlot, 0);
                LobbyUIManager.Instance.ShowUI("CharSkillSeletePopup", true);
            }
            else if (tutorialstep == 9)
            {
                UICharSkillSeletePopup popup = LobbyUIManager.Instance.GetUI<UICharSkillSeletePopup>("CharSkillSeletePopup");
                popup.SetSeleteSkill(popup.kSkillSlotList[0].TableData.ID, popup.kSkillSlotList[0]);
            }
            else if (tutorialstep == 10)
            {
                UICharSkillSeletePopup popup = LobbyUIManager.Instance.GetUI<UICharSkillSeletePopup>("CharSkillSeletePopup");
                popup.OnClick_SkillGet();
            }
            else if (tutorialstep == 11)
            {
                GameInfo.Instance.UserData.SetTutorial(_tutorialdata.SendPacket, 1);

                UISkillPassvieLeveUpPopup popup = LobbyUIManager.Instance.GetUI<UISkillPassvieLeveUpPopup>("SkillPassvieLeveUpPopup");
                popup.OnClick_UpgradeBtn();
            }
        }
        else if (tutorialstate == (int)eTutorialState.TUTORIAL_STATE_SkillEquip)
        {
            GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_2_Tutorial_Char_Skill_Equip);
            if (tutorialstep == 1)
            {
                GameInfo.Instance.UserData.SetTutorial(_tutorialdata.SendPacket, 1);

                UICharSkillSeletePopup popup = LobbyUIManager.Instance.GetUI<UICharSkillSeletePopup>("CharSkillSeletePopup");
                popup.OnClick_EquipBtn();
            }
            else if(tutorialstep == 3)
            {
                LobbyUIManager.Instance.HomeBtnEvent();
            }
        }
        else if (tutorialstate == (int)eTutorialState.TUTORIAL_STATE_Stage3Join)
        {
            GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_2_Tutorial_1_3_Join);
            if (tutorialstep == 1)
            {
                LobbyUIManager.Instance.SetPanelType(ePANELTYPE.STORYMAIN);
            }
            else if (tutorialstep == 2)
            {
                UIStoryMainPanel panel = LobbyUIManager.Instance.GetUI<UIStoryMainPanel>("StoryMainPanel");
                panel.OnClick_MainStoryBtn();
            }
            else if (tutorialstep == 3)
            {
                UIStoryPanel panel = LobbyUIManager.Instance.GetUI<UIStoryPanel>("StoryPanel");
                panel.kStoryUnitList[2].OnClick_Slot();
            }
            else if (tutorialstep == 4)
            {
                GameInfo.Instance.UserData.SetTutorial((int)eTutorialState.TUTORIAL_STATE_Stage3Clear, 1);

                UIStageDetailPopup popup = LobbyUIManager.Instance.GetUI<UIStageDetailPopup>("StageDetailPopup");
                popup.OnClick_StartBtn();
            }
        }
        else if (tutorialstate == (int)eTutorialState.TUTORIAL_STATE_Gacha)
        {
            GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_3_Tutorial_Gacha);
            if (tutorialstep == 4)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.GachaTab, "CASH");
                LobbyUIManager.Instance.SetPanelType(ePANELTYPE.GACHA);
            }
            else if (tutorialstep == 7)
            {
                UIGachaPanel panel = LobbyUIManager.Instance.GetUI<UIGachaPanel>("GachaPanel");
                panel.OnClick_GachaBtnUnit_00();
            }
            else if (tutorialstep == 8)
            {
                GameInfo.Instance.UserData.SetTutorial(_tutorialdata.SendPacket, 1);

                UIBuyPopup popup = LobbyUIManager.Instance.GetUI<UIBuyPopup>("BuyPopup");
                popup.OnClick_BuyBtn();
            }
            else if (tutorialstep == 10)
            {
                LobbyUIManager.Instance.BackBtnEvent();
            }
            
        }
        else if (tutorialstate == (int)eTutorialState.TUTORIAL_STATE_Mail)
        {
            GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_3_Tutorial_Mail);
            if (tutorialstep == 2)
            {
                UIMainPanel panel = LobbyUIManager.Instance.GetUI<UIMainPanel>("MainPanel");
                panel.OnClick_MailBox();
            }
            else if (tutorialstep == 4)
            {
                GameInfo.Instance.UserData.SetTutorial((int)eTutorialState.TUTORIAL_STATE_EndTutorial, 1);
            }
        }
        if (_tutorialdata == null)
            return;

        if (_tutorialdata.SendPacket != 0)
        {
            for (int i = 0; i < kInfoList.Count; i++)
                kInfoList[i].SetActive(false);
        }
        
    }

    public void NextTutorial()
    {
        mLock = true; // 다음 튜토리얼로 넘어가기 전까진 버튼들에 락을 걸어줌

        Time.timeScale = oldtimescale;
        ActionTutorial();

        if (_tutorialdata == null)
            return;

        if (_tutorialdata.SendPacket != 0 && _tutorialdata.NextID != -1)
            return;

        if (_tutorialdata.NextTime == 0.0f)
        {
            Next();
        }
        else
        {
            for (int i = 0; i < kInfoList.Count; i++)
                kInfoList[i].SetActive(false);

            Invoke("Next", _tutorialdata.NextTime);
        }
    }

    public void Next()
    {
        if (_tutorialdata == null)
            return;

        if (_tutorialdata.NextID == -1)
        {
            if (_tutorialdata.StateID >= (int)eTutorialState.TUTORIAL_FLAG)
            {
                Debug.Log("Send_ReqSetTutorialFlag");
                GameInfo.Instance.Send_ReqSetTutorialFlag(_tutorialdata.StateID - (int)eTutorialState.TUTORIAL_FLAG, OnNetSetTutorialFlag);
                SetUIActive(false);
            }
            else
            {
                if (_tutorialdata.StateID == (int)eTutorialState.TUTORIAL_STATE_EndTutorial)
                {
                    LobbyUIManager.Instance.InitComponent("MainPanel");
                    LobbyUIManager.Instance.Renewal("MainPanel");
                }

                SetUIActive(false);
            }
        }
        else
        {
            mLock = false;
            
            _tutorialid += 1;
            Debug.Log("Next Tutorial ID : " + _tutorialid);
            
            SetTutorial();
        }
    }
    public void OnNetSetTutorialFlag(int result, PktMsgType pktmsg)
    {
        SetUIActive(false);
    }

    public override bool IsBackButton()
    {
        return false;
    }

    public void HideTutorialSkipButton()
    {
        BtnTutorialSkip.gameObject.SetActive(false);
    }

    public void OnBtnTutorialSkip()
    {
        MessagePopup.OKCANCEL(eTEXTID.OK, 3220, StartTutorialSkip, null, false);
    }

    private void StartTutorialSkip()
    {
        if (GameSupport.IsTutorial())
        {
            if (_tutorialdata.StateID == (int)eTutorialState.TUTORIAL_STATE_CardEquip)
                GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_1_Tutorial_Card_Equip_Skip);
            else if (_tutorialdata.StateID == (int)eTutorialState.TUTORIAL_STATE_CardLevelUp)
                GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_1_Tutorial_Card_LevelUp_Skip);
            else if (_tutorialdata.StateID == (int)eTutorialState.TUTORIAL_STATE_Stage2Join)
                GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_1_Tutorial_1_2_Join_Skip);
            else if (_tutorialdata.StateID == (int)eTutorialState.TUTORIAL_STATE_SkillSpt)
                GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_2_Tutorial_Char_Skill_Leam_Skip);
            else if (_tutorialdata.StateID == (int)eTutorialState.TUTORIAL_STATE_SkillEquip)
                GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_2_Tutorial_Char_Skill_Equip_Skip);
            else if (_tutorialdata.StateID == (int)eTutorialState.TUTORIAL_STATE_Stage3Join)
                GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_2_Tutorial_1_3_Join_Skip);
            else if (_tutorialdata.StateID == (int)eTutorialState.TUTORIAL_STATE_Gacha)
                GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_3_Tutorial_Gacha_Skip);
            else if (_tutorialdata.StateID == (int)eTutorialState.TUTORIAL_STATE_Mail)
                GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_3_Tutorial_Mail_Skip);
            GameInfo.Instance.Send_ReqSetTutorialVal((int)eTutorialState.TUTORIAL_STATE_EndTutorial, 1, OnSkipTutorial);
        }
        else
        {
            GameInfo.Instance.Send_ReqSetTutorialFlag(_tutorialdata.StateID - (int)eTutorialState.TUTORIAL_FLAG, OnNetSetTutorialFlag);
            SetUIActive(false);
        }
    }

    public void OnSkipTutorial(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        if (!GameSupport.IsInGameTutorial())
        {
            UIMainPanel panelMain = LobbyUIManager.Instance.GetActiveUI<UIMainPanel>("MainPanel");
            if (panelMain == null)
            {
                List<FComponent> list = new List<FComponent>();

                FComponent[] comps = LobbyUIManager.Instance.GetComponentsInChildren<FComponent>();
                for (int i = 0; i < comps.Length; i++)
                {
                    FComponent comp = comps[i];
                    if (comp.name.Equals("GoodsPopup") || comp.name.Equals("TopPanel") || comp.name.Equals("MainPanel") ||
                        comp.name.Equals("TutorialPopup") || comp.name.Equals("LoadingPopup") || comp.name.Equals("WaitPopup"))
                    {
                        continue;
                    }

                    list.Add(comps[i]);
                }

                if (list.Count > 0)
                {
                    list.Sort(GameSupport.CompareWithFComponentDepth);
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].OnClickClose();
                    }

                    LobbyUIManager.Instance.BackBtnEvent();
                    LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN, true);
                }
            }
            else
            {
                LobbyUIManager.Instance.InitComponent("MainPanel");
                LobbyUIManager.Instance.Renewal("MainPanel");
            }
        }

        SetUIActive(false);
    }
}
