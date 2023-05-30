using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;

public class UIFirstCharSelectPopup : FComponent
{
    public GameObject kSelete;
    public GameObject kInfo;
    public UITexture kCharTex;
    public UILabel kNameLabel;
    public UILabel kCvLabel;
    public UILabel kIllustLabel;
    public UITextList kTextArea;
    public List<UIStatusUnit> kStatusUnit;

    public UISprite[] SprBtns;

    public UISprite kSkillNameSpr;
    public UITexture kCharSkilMovielTex;
    private VideoPlayer _videoPlayer;
    private RenderTexture _videoRenderTex;
    private string videoClipPath = string.Empty;
    private VideoClip videoclip;
    private List<GameTable.CharacterSkillPassive.Param> _charSkillList = new List<GameTable.CharacterSkillPassive.Param>();
    private int _skillIdx = (int)eCOUNT.NONE;

    public bool IsSelect { get; private set; } = true;

    private int _chartableid = 1;
    private Director mDirector = null;

    private int _charCostumeTableId = (int)eCOUNT.NONE;

    public override void Awake()
    {
        base.Awake();

        if (kCharSkilMovielTex != null && kCharSkilMovielTex.mainTexture == null)
        {
            _videoRenderTex = new RenderTexture((int)kCharSkilMovielTex.width, (int)kCharSkilMovielTex.height, kCharSkilMovielTex.depth);

            if (_videoPlayer == null)
                _videoPlayer = kCharSkilMovielTex.GetComponent<VideoPlayer>();

            //_videoPlayer.source = VideoSource.Url;

            _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            _videoPlayer.isLooping = false;
            _videoPlayer.targetTexture = _videoRenderTex;
            _videoPlayer.playbackSpeed = 1.0f;
            kCharSkilMovielTex.mainTexture = _videoRenderTex;

            _videoPlayer.loopPointReached += OnLoopPointReached;
        }
    }

    public override void OnEnable()
    {
        if (mDirector == null)
        {
            mDirector = World.Instance.GetDirector("Start");
            mDirector.Pause();
        }

        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        GameSupport.SendFireBaseLogEvent(eFireBaseLogType._Char_Selection);
        RenderTargetChar.Instance.gameObject.SetActive(false);

        SoundManager.Instance.PlayUISnd(68);
        //팝업 애니메이션 사용 예정
        PlayAnimtion(0);
        //kSelete.SetActive(true);
        //kInfo.SetActive(false);        
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        var chartabledata = GameInfo.Instance.GameTable.FindCharacter(_chartableid);
        if (chartabledata == null)
            return;

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(chartabledata.Name);
        kCvLabel.textlocalize = FLocalizeString.Instance.GetText(chartabledata.Name + 300000);
        kIllustLabel.textlocalize = FLocalizeString.Instance.GetText(chartabledata.Name + 400000);
        kTextArea.textLabel.textlocalize = "";
        kTextArea.Clear();

        string _text = FLocalizeString.Instance.GetText(chartabledata.Name + 100000);
        // 텍스트 리스트의 경우는 위젯의 사이즈를 재할당해야하기 때문에 lateUpdate 보다 빨리 들어오게 됨.
        // 위젯 사이즈 재할당후 텍스트를 설정해주면 uilabel 의 late update에서 자동으로 색 지정해줌.
        kTextArea.Add(_text);
        kTextArea.textLabel.textlocalize = _text;


        int thp = (int)((chartabledata.HP) * GameInfo.Instance.GameConfig.CharGradeStatRate[1]);
        int tatk = (int)((chartabledata.ATK) * GameInfo.Instance.GameConfig.CharGradeStatRate[1]);
        int tdef = (int)((chartabledata.DEF) * GameInfo.Instance.GameConfig.CharGradeStatRate[1]);
        int tcri = (int)((chartabledata.CRI) * GameInfo.Instance.GameConfig.CharGradeStatRate[1]);

        kStatusUnit[0].InitStatusUnit((int)eTEXTID.STAT_HP, thp);
        kStatusUnit[1].InitStatusUnit((int)eTEXTID.STAT_ATK, tatk);
        kStatusUnit[2].InitStatusUnit((int)eTEXTID.STAT_DEF, tdef);
        kStatusUnit[3].InitStatusUnit((int)eTEXTID.STAT_CRI, tcri);
    }

    public void OnClick_CharSeleteBtn(int index)
    {
        _chartableid = index;
        SetInfo();

        LoadChar();
        Renewal(true);
    }

    public void OnClick_Arrow_LBtn()
    {
        _chartableid -= 1;
        if (_chartableid <= 0)
            _chartableid = 3;

        LoadChar();
        Renewal(true);
    }

    public void OnClick_Arrow_RBtn()
    {
        _chartableid += 1;
        if (_chartableid > 3)
            _chartableid = 1;

        LoadChar();
        Renewal(true);
    }

    public void OnClick_CvPlayBtn()
    {
        SoundManager.Instance.StopVoice();
        VoiceMgr.Instance.PlayChar(eVOICECHAR.FirstGreetings, _chartableid);
    }

    public void OnClick_OKBtn()
    {
        var chartabledata = GameInfo.Instance.GameTable.FindCharacter(_chartableid);
        if (chartabledata == null)
            return;

        MessagePopup.CYN(eTEXTID.TITLE_NOTICE, string.Format(FLocalizeString.Instance.GetText(3124), FLocalizeString.Instance.GetText(chartabledata.Name)), eTEXTID.YES, eTEXTID.NO, OnMsg_CharSelect);
    }

    public void OnMsg_CharSelect()
    {
        GameInfo.Instance.Send_ReqAddCharacter(_chartableid, OnSuccessCharSelect);
    }

    private void OnSuccessCharSelect(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        int stageId = (int)UIValue.Instance.GetValue(UIValue.EParamType.StageID);

        if (_chartableid == (int)ePlayerCharType.Asagi)
            GameSupport.SendFireBaseLogEvent(eFireBaseLogType._Char_Selection_Asagi);
        else if (_chartableid == (int)ePlayerCharType.Sakura)
            GameSupport.SendFireBaseLogEvent(eFireBaseLogType._Char_Selection_Sakura);
        else if (_chartableid == (int)ePlayerCharType.Yukikaze)
            GameSupport.SendFireBaseLogEvent(eFireBaseLogType._Char_Selection_Yukikaze);

        CharData charData = GameInfo.Instance.GetCharDataByTableID(_chartableid);
        GameInfo.Instance.Send_ReqStageStart(stageId, charData.CUID, 0, false, false, null, OnSuccessStageStart);
        
    }

    private void OnSuccessStageStart(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        GameSupport.SendFireBaseLogEvent(eFireBaseLogType._Char_Selection_Complete);

        int stageId = (int)UIValue.Instance.GetValue(UIValue.EParamType.StageID);
        GameTable.Stage.Param param = GameInfo.Instance.GameTable.FindStage(stageId);

        if (!GameInfo.Instance.netFlag)
            GameInfo.Instance.MaxNormalBoxCount = Random.Range(param.N_DropMinCnt, param.N_DropMaxCnt + 1);

        mDirector.Resume();
        SetUIActive(false, false);
    }

    public void OnClick_CancleBtn()
    {
        SetSelete();
        Renewal(true);
    }

    public void OnClick_CharBtn()
    {
        if (RenderTargetChar.Instance.RenderPlayer.aniEvent.curAniType != eAnimation.Lobby_Selete)
        {
            RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Lobby_Selete, 0, eFaceAnimation.Selete, 0);
            SoundManager.Instance.StopVoice();
            VoiceMgr.Instance.PlayChar(eVOICECHAR.PlayStageSel, _chartableid);
        }
    }

    public void OnClick_RandomCostume()
    {
        Debug.Log("OnClick_RandomCostume");

        GameTable.Character.Param chartabledata = GameInfo.Instance.GameTable.FindCharacter(_chartableid);
        if (chartabledata == null)
            return;

        List<GameTable.Costume.Param> costumelist = GameInfo.Instance.GameTable.FindAllCostume(x => x.CharacterID == _chartableid);
        if (costumelist == null || costumelist.Count <= (int)eCOUNT.NONE)
            return;

        int randIdx = Random.Range((int)eCOUNT.NONE, costumelist.Count - 1);
        int randID = costumelist[randIdx].ID;

        while (randID == _charCostumeTableId)
        {
            randIdx = Random.Range((int)eCOUNT.NONE, costumelist.Count - 1);
            randID = costumelist[randIdx].ID;
        }

        _charCostumeTableId = randID;

        RenderTargetChar.Instance.InitRenderTargetChar(_chartableid, -1, _charCostumeTableId, eCharacterType.Character);
        RenderTargetChar.Instance.SetWeaponAttachTableData(chartabledata.InitWeapon, false, false);

        RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Lobby_Selete, 0, eFaceAnimation.Selete, 0);
        RenderTargetChar.Instance.ShowAttachedObject(true);
    }

    public void OnClick_CostumeDetailView()
    {
        Debug.Log("OnClick_CostumeDetailView");

        CharViewer.ShowCharPopup(this.gameObject.name, kCharTex.gameObject, kCharTex.transform.parent);
    }

    public void OnClick_NextSkill()
    {
        _skillIdx++;
        if (_skillIdx >= _charSkillList.Count)
            _skillIdx = (int)eCOUNT.NONE;

        GameSupport.SetSkillSprite(ref kSkillNameSpr, _charSkillList[_skillIdx].Atlus, _charSkillList[_skillIdx].Icon);

        VideoStreamingCheck(_charSkillList[_skillIdx].SkillMovie);
    }
    public void OnClick_PrevSkill()
    {
        _skillIdx--;
        if (_skillIdx < (int)eCOUNT.NONE)
            _skillIdx = _charSkillList.Count - 1;

        GameSupport.SetSkillSprite(ref kSkillNameSpr, _charSkillList[_skillIdx].Atlus, _charSkillList[_skillIdx].Icon);

        VideoStreamingCheck(_charSkillList[_skillIdx].SkillMovie);
    }

    private void SetSelete()
    {
        if (SprBtns != null)
        {
            for (int i = 0; i < SprBtns.Length; i++)
            {
                SprBtns[i].color = Color.white;
            }
        }

        IsSelect = true;
        PlayAnimtion(3);
        SoundManager.Instance.PlayUISnd(68);
    }
    private void SetInfo()
    {
        IsSelect = false;
        PlayAnimtion(2);
        SoundManager.Instance.PlayUISnd(69);
    }

    private void LoadChar()
    {
        StopCoroutine("ShowAttachedObject");

        SoundManager.Instance.StopVoice();
        VoiceMgr.Instance.PlayChar(eVOICECHAR.PlayStageSel, _chartableid);

        GameTable.Character.Param chartabledata = GameInfo.Instance.GameTable.FindCharacter(_chartableid);
        if (chartabledata == null)
            return;

        _charSkillList.Clear();
        _charSkillList = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.CharacterID == _chartableid && x.ParentsID == -1 && (x.Type == (int)eCHARSKILLPASSIVETYPE.SELECT_NORMAL));

        _skillIdx = (int)eCOUNT.NONE;

        _charCostumeTableId = chartabledata.InitCostume;

        RenderTargetChar.Instance.gameObject.SetActive(true);
        RenderTargetChar.Instance.InitRenderTargetChar(_chartableid, -1, false, eCharacterType.Character);
        RenderTargetChar.Instance.SetWeaponAttachTableData(chartabledata.InitWeapon, false, false);
        
        RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Lobby_Selete, 0, eFaceAnimation.Selete, 0);
        RenderTargetChar.Instance.ShowAttachedObject(true);
        //StartCoroutine("ShowAttachedObject", RenderTargetChar.Instance.RenderPlayer.aniEvent.GetCutFrameLength(eAnimation.Lobby_Weapon));

        GameSupport.SetSkillSprite(ref kSkillNameSpr, _charSkillList[_skillIdx].Atlus, _charSkillList[_skillIdx].Icon);

        VideoStreamingCheck(_charSkillList[_skillIdx].SkillMovie);
    }

    private IEnumerator ShowAttachedObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        RenderTargetChar.Instance.ShowAttachedObject(true);
    }

    public void OnLoopPointReached(VideoPlayer video)
    {
        VideoStreamingCheck(string.Empty);
    }

    private void VideoStreamingCheck(string path)
    {
        if (string.IsNullOrEmpty(path) == true)
        {
            //Stop -> Play
            _videoPlayer.Stop();
            _videoPlayer.Play();
        }
        else
        {
            string videoPath = $"Movie/Skill/{path}.mp4";
            if (videoClipPath != videoPath)
            {
                videoClipPath = videoPath;
                StopCoroutine(CoVideoPlay(videoClipPath));
                StartCoroutine(CoVideoPlay(videoClipPath));
                return;
            }

            if (videoclip)
            {
                //kCharSkillSpr.gameObject.SetActive(false);
                _videoPlayer.clip = videoclip;
                _videoPlayer.Play();
            }
            else
            {
                //kCharSkillSpr.gameObject.SetActive(true);
                kCharSkilMovielTex.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator CoVideoPlay(string path)
    {
        yield return null;
        
        string filePath = path;
        if (AppMgr.Instance.GetResLoadType() != Config.eResLoadType.Folder)
        {
            filePath = path.Replace(".mp4", "");
        }
        
        videoclip = ResourceMgr.Instance.LoadFromAssetBundle("movie", filePath) as VideoClip;

        if (videoclip)
        {
            //kCharSkillSpr.gameObject.SetActive(false);
            _videoPlayer.clip = videoclip;
            _videoPlayer.Play();
        }
        else
        {
            //kCharSkillSpr.gameObject.SetActive(true);
            kCharSkilMovielTex.gameObject.SetActive(false);
        }
    }
}
