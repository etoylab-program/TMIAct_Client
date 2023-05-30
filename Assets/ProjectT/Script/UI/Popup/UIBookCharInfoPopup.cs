using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBookCharInfoPopup : FComponent
{
	public UITexture    kCharTex;
    public UIButton     kCvPlayBtn;
    public UILabel      kNameLabel;
    public UILabel      kCvLabel;
    public UILabel      kIllustLabel;
    public UILabel      kTalkLabel;
    public UITextList   kTextList;
    public UIButton     kViewBtn;
    public UIButton     kReplayBtn;
    public GameObject   LockReplayObj;
    public UIButton     kFavorBtn;
    public GameObject   LockFavorObj;
    public UISprite     kFavorNoticeSpr;
    public UIButton     kArrow_RBtn;
    public UIButton     kArrow_LBtn;
    public UISprite     kTypeSpr;
    public UILabel      kTypeLabel;
    public UISprite     kMonTypeSpr;
    public UILabel      kMonTypeLabel;
    public UILabel      kFavorLabel;
    public UIButton     kTraingBtn;

    public List<UISprite> kLockSprList;

    private GameTable.Character.Param _chartabledata;
    private int _animationIndex = 0;

    private List<GameTable.Character.Param> _charlist = new List<GameTable.Character.Param>();

 
	public override void OnEnable()
	{
        _charlist.Clear();

        UIBookCharListPopup bookcharlistpopup = LobbyUIManager.Instance.GetUI<UIBookCharListPopup>("BookCharListPopup");
        if (bookcharlistpopup != null)
        {
            for (int i = 0; i < bookcharlistpopup.CharList.Count; i++)
            {
                GameTable.Character.Param chartabledata = bookcharlistpopup.CharList[i];
                _charlist.Add(chartabledata);
            }
        }

        if (_charlist.Count <= (int)eCOUNT.NONE)
        {
            for (int i = 0; i < GameInfo.Instance.GameTable.Characters.Count; i++)
            {
                CharData carddata = GameInfo.Instance.GetCharDataByTableID(GameInfo.Instance.GameTable.Characters[i].ID);
                if (carddata != null)
                    _charlist.Add(GameInfo.Instance.GameTable.Characters[i]);
            }

            for (int i = 0; i < GameInfo.Instance.GameTable.Characters.Count; i++)
            {
                CharData carddata = GameInfo.Instance.GetCharDataByTableID(GameInfo.Instance.GameTable.Characters[i].ID);
                if (carddata == null)
                    _charlist.Add(GameInfo.Instance.GameTable.Characters[i]);
            }
        }

        InitComponent();
		base.OnEnable();
	}

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();
        SoundManager.Instance.StopVoice();
        RenderTargetChar.Instance.DestroyRenderTarget();
    }

    public override void InitComponent()
	{
        int tableid = (int)UIValue.Instance.GetValue(UIValue.EParamType.BookItemID);
        _chartabledata = GameInfo.Instance.GameTable.FindCharacter(tableid);

        RenderTargetChar.Instance.gameObject.SetActive(true);
        RenderTargetChar.Instance.InitRenderTargetChar(tableid, -1, false, eCharacterType.Character);

        kArrow_RBtn.gameObject.SetActive(true);
        kArrow_LBtn.gameObject.SetActive(true);

        _animationIndex = (int)eCOUNT.NONE;

        if (LobbyUIManager.Instance.kBlackScene.activeSelf)
            LobbyUIManager.Instance.kBlackScene.SetActive(false);

        kFavorLabel.textlocalize = $"{(int)eCOUNT.NONE}";
        StartCoroutine(LockAnimation());
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        FLocalizeString.SetLabel(kNameLabel, _chartabledata.Name);
        FLocalizeString.SetLabel(kCvLabel, _chartabledata.Name + 300000);
        FLocalizeString.SetLabel(kIllustLabel, _chartabledata.Name + 400000);
        FLocalizeString.SetLabel(kTalkLabel, _chartabledata.Name + 500000);

        kTypeSpr.spriteName = string.Format("SupporterType_{0}", _chartabledata.Type);
        kTypeLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.CARDTYPE + _chartabledata.Type);
        kMonTypeSpr.spriteName = string.Format("TribeType_{0}", _chartabledata.MonType);
        kMonTypeLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.MON_TYPE_TEXT_START + _chartabledata.MonType);

        kTextList.textLabel.textlocalize = "";
        kTextList.Clear();

        string _text = Utility.AppendColorBBCodeString(FLocalizeString.Instance.GetText(_chartabledata.Name + 600000));
        // 텍스트 리스트의 경우는 위젯의 사이즈를 재할당해야하기 때문에 lateUpdate 보다 빨리 들어오게 됨.
        // 위젯 사이즈 재할당후 텍스트를 설정해주면 uilabel 의 late update에서 자동으로 색 지정해줌.
        kTextList.Add(_text);
        kTextList.textLabel.textlocalize = _text;

        if (FSaveData.Instance.HasKey("NCharBook_" + _chartabledata.ID.ToString()))
            FSaveData.Instance.DeleteKey("NCharBook_" + _chartabledata.ID.ToString());

        kFavorNoticeSpr.gameObject.SetActive(false);
        LockReplayObj.SetActive(false);
        LockFavorObj.SetActive(false);
        
        var chardata = GameInfo.Instance.CharList.Find(x => x.TableID == _chartabledata.ID);
        if (chardata != null)
        {
            if (GameSupport.IsCharOpenTerms_Favor(chardata))
            {
                if (PlayerPrefs.HasKey("NCharBook_Favor_" + _chartabledata.ID.ToString()))
                    kFavorNoticeSpr.gameObject.SetActive(true);
            }
            else
            {
                LockFavorObj.SetActive(true);
            }

            kFavorLabel.textlocalize = chardata.FavorLevel.ToString();
        }
        else
        {
            LockFavorObj.SetActive(true);
            LockReplayObj.SetActive(true);
        }

        if ( !AppMgr.Instance.configData.m_Network ) {
            LockFavorObj.SetActive( false );
		}

        kTraingBtn.SetActive(false);

        if (kTraingBtn != null && _chartabledata != null)
        {
            if (_chartabledata.TrainingRoom == (int)eCOUNT.NONE + 1)
                kTraingBtn.SetActive(true);
            else
            {
                if(chardata != null)
                    kTraingBtn.SetActive(true);
                else
                    kTraingBtn.SetActive(false);
            }
                
        }
    }

    public void OnClick_BackBtn()
	{
        OnClickClose();
    }

    public override void OnClickClose()
    {
        LobbyUIManager.Instance.Renewal("BookCharListPopup");
        base.OnClickClose();
    }

    public void OnClick_ViewBtn()
    {
        //Tween중에는 동작하지 않도록
        if (kCharTex.GetComponent<TweenPosition>() != null)
        {
            if (kCharTex.GetComponent<TweenPosition>().enabled)
                return;
        }
        CharViewer.ShowCharPopup("BookCharInfoPopup", kCharTex.gameObject, kCharTex.transform.parent);
    }

    public void OnClick_ReplayBtn()
    {
        DirectorUIManager.Instance.PlayCharBuy(_chartabledata.ID,
            () => { AppMgr.Instance.CustomInput.ShowCursor(true); });
    }

    public void OnClick_FavorBtn()
    {
        if (!GameInfo.Instance.GameConfig.TestMode && !AppMgr.Instance.Review)
        {
            var chardata = GameInfo.Instance.CharList.Find(x => x.TableID == _chartabledata.ID);
            if (chardata != null)
            {
                if (!GameSupport.IsCharOpenTerms_Favor(chardata))
                {
                    MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3153), GameInfo.Instance.GameConfig.CharOpenTermsLevel));
                    //MessageToastPopup.Show(FLocalizeString.Instance.GetText(3062));
                    return;
                }
            }
            else
            {
                MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3153), GameInfo.Instance.GameConfig.CharOpenTermsLevel));
                return;
            }
        }

        int scenarioGroupID = _chartabledata.ScenarioGroupID;
        string scenariobg = "char_" + _chartabledata.ID.ToString() + "_0";
        string scenarioSprite = _chartabledata.SpriteIcon;
        UIValue.Instance.SetValue(UIValue.EParamType.ScenarioGroupID, scenarioGroupID);
        UIValue.Instance.SetValue(UIValue.EParamType.ScenarioFavorBGStr, scenariobg);
        UIValue.Instance.SetValue(UIValue.EParamType.ScenarioFavorBGSprite, scenarioSprite);
        LobbyUIManager.Instance.ShowUI("BookCardCinemaPopup", true);

        PlayerPrefs.DeleteKey("NCharBook_Favor_" + _chartabledata.ID.ToString());
    }

    public void OnClick_CvPlayBtn()
    {
        //  캐릭터 보이스
        int voiceNum = Random.Range(1, (int)eVOICECHAR.Count);
        VoiceMgr.Instance.PlayChar((eVOICECHAR)voiceNum, _chartabledata.ID);
    }

    public void OnClick_Arrow_LBtn()
    {
        int id = GetNextBookID(true);
        if (id != -1)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, id);
            InitComponent();
            Renewal(true);
        }
    }

    public void OnClick_Arrow_RBtn()
    {
        int id = GetNextBookID(false);
        if (id != -1)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, id);
            InitComponent();
            Renewal(true);
        }
    }

    public void OnBtnTraining()
    {
        UIValue.Instance.RemoveValue(UIValue.EParamType.TemporaryCharTraining);

        long uid = 0;

        CharData charData = GameInfo.Instance.GetCharDataByTableID(_chartabledata.ID);
        if(charData == null)
        {
            int weaponTableID = _chartabledata.InitWeapon;
            WeaponBookData findWpnBook = GameInfo.Instance.WeaponBookList.Find(x => x.TableID == weaponTableID);
            if (findWpnBook == null)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.TemporaryWeaponBookTraining, weaponTableID);
            }

            uid = 100000 + (_chartabledata.ID - 1);
            NetLocalSvr.Instance.AddChar(_chartabledata.ID, uid);

            UIValue.Instance.SetValue(UIValue.EParamType.TemporaryCharTraining, uid);

            CharData tempCharData = GameInfo.Instance.CharList.Find(x => x.CUID == uid);
            if (tempCharData != null)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.TemporaryWeaponTraining, tempCharData.EquipWeaponUID);
            }

            UIValue.Instance.SetValue(UIValue.EParamType.TrainingCharTID, tempCharData.TableData.ID);

            charData = tempCharData;
        }
        else
        {
            uid = charData.CUID;
            UIValue.Instance.SetValue(UIValue.EParamType.TrainingCharTID, charData.TableData.ID);
        }

        if (GameSupport.IsEmptyInEquipMainWeapon(ePresetKind.STAGE, uid))
        {
            return;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.LobbyToTrainingRoom, "BookCharInfoPopup");

        GameInfo.Instance.SeleteCharUID = uid;
        UIValue.Instance.RemoveValue(UIValue.EParamType.ArenaCharInfoFlag);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.LobbyToTraining);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, -1);
        LobbyUIManager.Instance.ShowUI("LoadingPopup", false);
        AppMgr.Instance.LoadScene(AppMgr.eSceneType.Training, "Stage_skill_trainingroom");
    }

    private IEnumerator LockAnimation()
    {
        while (0 < kLockSprList.Count)
        {
            foreach (UISprite spr in kLockSprList)
            {
                spr.gameObject.SetActive(false);
            }
            
            kLockSprList[_animationIndex].gameObject.SetActive(true);
            ++_animationIndex;

            if (kLockSprList.Count <= _animationIndex)
            {
                _animationIndex = (int)eCOUNT.NONE;
            }
            
            float rnd = Random.Range(2, 4);
            yield return new WaitForSeconds(rnd);
        }
    }

    private int GetNextBookID(bool bleft)
    {
        if (_charlist.Count <= (int)eCOUNT.NONE)
            return -1;

        int index = -1;
        for (int i = 0; i < _charlist.Count; i++)
        {
            if (_chartabledata.ID == _charlist[i].ID)
            {
                index = i;
                break;
            }
        }

        if (bleft)
        {
            index -= 1;
            if (index < 0)
                index = _charlist.Count - 1;
        }
        else
        {
            index += 1;
            if (index >= _charlist.Count)
                index = 0;
        }
        if (index != -1)
            return _charlist[index].ID;

        return -1;
    }
}
