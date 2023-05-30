using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;

public class UICharSkillSeletePopup : FComponent
{
    public UISprite kSkillOpenSpr;
    public UISprite kSkillTypeSpr;
    public UISprite kSkillNameSpr;
    public GameObject kSkilLock;
    public UILabel kLockLevelLabel;
    public UILabel kCommandLabel;
    public UILabel kSkillTimeLabel;
    public UILabel kSkillDescLabel;
    public UILabel kSkillSlotNumLabel;
    public UIButton kSkillGetBtn;
    public UIButton kEquipBtn;
    public UIButton kReleaseBtn;
    public UISprite kDisableSpr;
    public UILabel kDisableLabel;    
    public UISprite kCharSkillSpr;    
    public UITexture kCharSkilMovielTex;
    public List<UISkillSeleteListSlot> kSkillSlotList;
    public List<GameObject> kSkillCommandList;

    public List<UISprite> kSkillCondSprList;
    public List<UILabel> kSkillCondNameList;
    public List<UILabel> kSkillCondEffLabelList;
    public List<UISprite> kSkillCondHideList;

    private VideoPlayer _videoPlayer;
    private RenderTexture _videoRenderTex;
    //private Coroutine _videoCoroutine = null;    
    private CharData _chardata;

    private int _seleteslot;
    private int _seleteskillid;
    private List<GameTable.CharacterSkillPassive.Param> _skilllist = new List<GameTable.CharacterSkillPassive.Param>();
    private UISkillSeleteListSlot _seletedskillslot = null;
    public int SeleteSkillID { get { return _seleteskillid; } }

    string videoClipPath = string.Empty;
    VideoClip videoclip;
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
			_videoPlayer.waitForFirstFrame = true;
			kCharSkilMovielTex.mainTexture = _videoRenderTex;

            _videoPlayer.loopPointReached += OnLoopPointReached;
        }

    }
    
    public override void OnEnable()
    {
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

		if (Lobby.Instance && Lobby.Instance.lobbyCamera)
		{
			Lobby.Instance.lobbyCamera.EnablePostProcess(true);
		}

		/*if (_videoCoroutine != null)
        {
            StopCoroutine(_videoCoroutine);
            _videoCoroutine = null;
        }*/

		if (kCharSkillSpr)
		{
			kCharSkillSpr.gameObject.SetActive(true);
		}
    }

    public override void InitComponent()
    {
        Lobby.Instance.lobbyCamera.EnablePostProcess(false);

        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
        _chardata = GameInfo.Instance.GetCharData(uid);
        _seletedskillslot = null;

        kCharSkillSpr.gameObject.SetActive(true);
        kCharSkilMovielTex.gameObject.SetActive(false);

        _seleteslot = 0;
        _seleteskillid = 0;
        var obj = UIValue.Instance.GetValue(UIValue.EParamType.CharEquipSkillSlot);
        if (obj != null)
        {
            _seleteslot = (int)obj;
            _seleteskillid = _chardata.EquipSkill[_seleteslot];
        }


        _skilllist.Clear();
        _skilllist = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.CharacterID == _chardata.TableID && x.ParentsID == -1 && (x.Type == (int)eCHARSKILLPASSIVETYPE.SELECT_NORMAL));

        int skillSlot = 0;
        if (_seleteskillid == (int)eCOUNT.NONE )
        {
            var data = GameInfo.Instance.GameTable.FindCharacterSkillPassive(x => x.CharacterID == _chardata.TableID && x.ParentsID == -1 && x.Type == (int)eCHARSKILLPASSIVETYPE.SELECT_NORMAL && x.Slot == 1 );
            if (data != null)
            {
                _seleteskillid = data.ID;
                skillSlot = data.Slot;
            }
            else
            {
                _seleteskillid = _skilllist[0].ID;
                skillSlot = _skilllist[0].Slot;
            }
        }
        else
        {
            GameTable.CharacterSkillPassive.Param param = GameInfo.Instance.GameTable.FindCharacterSkillPassive(x => x.ID == _seleteskillid);
            if(param != null)
            {
                skillSlot = param.Slot;
            }
        }

        /*
        for( int i = 0; i < _skilllist.Count; i++ )
        {
            if (_skilllist[i].ID == _seleteskillid)
                _seletedskillslot = kSkillSlotList[i];
        }
        */

        _seletedskillslot = kSkillSlotList.Find(x => x.kSkillSlot == skillSlot);
        if (_seletedskillslot == null)
        {
            _seletedskillslot = kSkillSlotList[0];
        }

        videoClipPath = string.Empty;
        videoclip = null;

        if (_seletedskillslot != null)
        {
            SetSeleteSkill(_seleteskillid, _seletedskillslot);
        }
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        kSkilLock.SetActive(false);

        GameTable.CharacterSkillPassive.Param characterSkillPassiveParam =
            GameInfo.Instance.GameTable.FindCharacterSkillPassive(x => x.CharacterID == _chardata.TableID && x.Slot == 7 && x.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_ULTIMATE);
        GameSupport.SetSkillSprite(ref kCharSkillSpr, characterSkillPassiveParam.Atlus, Utility.AppendString("Icon_", _chardata.TableData.Icon, "_001"));
        kSkillSlotNumLabel.textlocalize = (_seleteslot+1).ToString();

        var tabledata = GameInfo.Instance.GameTable.FindCharacterSkillPassive(_seleteskillid);
        if (tabledata == null)
            return;

        var passivedata = _chardata.PassvieList.Find(x => x.SkillID == _seleteskillid);

        //kSkillNameLabel.textlocalize = FLocalizeString.Instance.GetText(tabledata.Name);
        kSkillDescLabel.textlocalize = FLocalizeString.Instance.GetText(tabledata.Desc);
        if( tabledata.CoolTime == 0 )
            kSkillTimeLabel.textlocalize = "-";
        else
            kSkillTimeLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(263), tabledata.CoolTime);

        kSkillOpenSpr.spriteName = "SkillSlot_00";
        if (tabledata.GroupID > 0)
        {
            kSkillTypeSpr.gameObject.SetActive(true);
            kSkillTypeSpr.spriteName = "SkillType_0" + tabledata.GroupID.ToString();
        }
        else
            kSkillTypeSpr.gameObject.SetActive(false);

        GameSupport.SetSkillSprite(ref kSkillNameSpr, tabledata.Atlus, tabledata.Icon);

        kSkillDescLabel.color = new Color(kSkillDescLabel.color.r, kSkillDescLabel.color.g, kSkillDescLabel.color.b, 0.5f);
        kSkillTimeLabel.color = new Color(kSkillTimeLabel.color.r, kSkillTimeLabel.color.g, kSkillTimeLabel.color.b, 0.5f);

        if (passivedata != null)
        {

            kSkillDescLabel.color = new Color(kSkillDescLabel.color.r, kSkillDescLabel.color.g, kSkillDescLabel.color.b, 1.0f);
            kSkillTimeLabel.color = new Color(kSkillTimeLabel.color.r, kSkillTimeLabel.color.g, kSkillTimeLabel.color.b, 1.0f);
        }
        else
        {
            var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == tabledata.ItemReqListID);
            if (reqdata != null)
            {
                if (_chardata.Level < reqdata.LimitLevel) // 레벨제한
                {
                    kSkilLock.SetActive(true);
                    kLockLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), reqdata.LimitLevel);
                }
            }
        }

        for (int i = 0; i < kSkillCommandList.Count; i++)
            kSkillCommandList[i].SetActive(false);

        int index = tabledata.CommandIndex;
        if (0 <= index && kSkillCommandList.Count > index)
        {
            kSkillCommandList[index].SetActive(true);
        }

        for ( int i = 0; i < kSkillSlotList.Count; i++ )
        {
            if (0 <= i && _skilllist.Count > i)
            {
                var data = _skilllist[i];

                kSkillSlotList[i].ParentGO = this.gameObject;
                kSkillSlotList[i].UpdateSlot(i, _chardata);
            }
        }

        kSkillGetBtn.gameObject.SetActive(false);
        kEquipBtn.gameObject.SetActive(false);
        kReleaseBtn.gameObject.SetActive(false);
        kDisableSpr.gameObject.SetActive(false);

        if (_chardata.EquipSkill[_seleteslot] == _seleteskillid)
        {
            kReleaseBtn.gameObject.SetActive(true);
        }
        else
        {
            if(passivedata == null)
            {
                var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == tabledata.ItemReqListID);
                if (reqdata != null)
                {
                    if (_chardata.Level < reqdata.LimitLevel) // 레벨제한
                    {
                        kDisableSpr.gameObject.SetActive(true);
                        kDisableLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1028), reqdata.LimitLevel);
                    }
                    else
                    {
                        kSkillGetBtn.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                kEquipBtn.gameObject.SetActive(true);
            }
        }

        var list = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.ParentsID == _seleteskillid && x.Type == (int)eCHARSKILLPASSIVETYPE.CONDITION_SKILL);

        int bgWidth = 580;
        {
            UISprite spr = kSkillCondSprList[0].transform.parent.GetComponent<UISprite>();
            if (spr != null)
                bgWidth = spr.width;
        }

        for ( int i = 0; i < list.Count; i++ )
        {
            kSkillCondNameList[i].textlocalize = string.Format(FLocalizeString.Instance.GetText(4200 + list[i].CondType), list[i].CondValue);
            int icon = (list[i].CondType % 3);
            if (list[i].CondType == (int)eCHARSKILLCONDITION.ANY_CARD_CNT)
                icon = 0;
            else if (icon == 0)
                icon = 3;

            kSkillCondSprList[i].spriteName = "SupporterType_" + icon.ToString();
            kSkillCondEffLabelList[i].textlocalize = FLocalizeString.Instance.GetText(list[i].Desc);
            kSkillCondEffLabelList[i].transform.localPosition = new Vector3(kSkillCondNameList[i].transform.localPosition.x + kSkillCondNameList[i].printedSize.x + 10, kSkillCondNameList[i].transform.localPosition.y, 0);

            //kSkillCondEffLabelList Widget 크기 계산
            int w = bgWidth - (kSkillCondSprList[i].width + (int)kSkillCondNameList[i].transform.localPosition.x + (int)kSkillCondNameList[i].printedSize.x + 40);
            kSkillCondEffLabelList[i].width = w;


            if (GameSupport.IsCharSkillCond(_chardata, list[i]))
                kSkillCondHideList[i].gameObject.SetActive(false);
            else
                kSkillCondHideList[i].gameObject.SetActive(true);
        }
    }


    public void OnClick_BackBtn()
    {
        OnClickClose();
    }
    public void OnClick_EquipBtn()
    {
        var passivedata = _chardata.PassvieList.Find(x => x.SkillID == _seleteskillid);
        if (passivedata == null)
            return;

        for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
        {
            if(_chardata.EquipSkill[i] == _seleteskillid)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3099));
                return;
            }
        }

        for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
        {
            if (_seleteslot == i)
                continue;
            var data = GameInfo.Instance.GameTable.FindCharacterSkillPassive(_chardata.EquipSkill[i]);
            if (data == null)
                continue;
            if (data.GroupID <= 0)
                continue;
            if (data.GroupID == passivedata.TableData.GroupID && data.ID != passivedata.TableData.ID)
            {
                //MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3113), FLocalizeString.Instance.GetText(passivedata.TableData.Name)));
                MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3113), FLocalizeString.Instance.GetText(data.Name)));
                return;
            }
        }

        _chardata.EquipSkill[_seleteslot] = _seleteskillid;

        GameInfo.Instance.Send_ReqApplySkillInChar(_chardata.CUID, _chardata.EquipSkill, OnNetCharSkillEquip);
    }
    public void OnClick_ReleaseBtn()
    {
        _chardata.EquipSkill[_seleteslot] = (int)eCOUNT.NONE;
        GameInfo.Instance.Send_ReqApplySkillInChar(_chardata.CUID, _chardata.EquipSkill, OnNetCharSkillEquip);
    }

    public void OnClick_SkillGet()
    {
        if (_seletedskillslot == null)
            return;

        var popup = LobbyUIManager.Instance.GetUI<UISkillPassvieLeveUpPopup>("SkillPassvieLeveUpPopup");
        if (popup != null)
        {
            popup.SetSkillSlot(_seletedskillslot);
        }

        UIValue.Instance.SetValue(UIValue.EParamType.CharSkillPassiveID, _seleteskillid);
        LobbyUIManager.Instance.ShowUI("SkillPassvieLeveUpPopup", true);
    }


    public void SetSeleteSkill(int id, UISkillSeleteListSlot slot = null)
    {
        var tabledata = GameInfo.Instance.GameTable.FindCharacterSkillPassive(id);
        if (tabledata == null)
            return;

        _seleteskillid = id;
        _seletedskillslot = slot;

        if ( tabledata.SkillMovie != "" || tabledata.SkillMovie != string.Empty )
        {
            SetVideoActive(true, string.Format("Movie/Skill/{0}.mp4", tabledata.SkillMovie));
        }
        else
        {
            kCharSkillSpr.gameObject.SetActive(true);
            kCharSkilMovielTex.gameObject.SetActive(false);
        }

        Renewal(true);
    }

    public void OnLoopPointReached(VideoPlayer video)
    {
        /*if (_videoCoroutine != null)
        {
            StopCoroutine(_videoCoroutine);
            _videoCoroutine = null;
        }*/

        //  이전 재생했던 데이터를 그대로 재생
        // _videoCoroutine = StartCoroutine(VideoStreamingCheck(string.Empty));
        VideoStreamingCheck(string.Empty);
    }

    private void SetVideoActive(bool bActive, string path)
    {
        if (bActive)
        {
            /*if (_videoCoroutine != null)
            {
                StopCoroutine(_videoCoroutine);
                _videoCoroutine = null;
            }

            _videoCoroutine = StartCoroutine(VideoStreamingCheck(path));*/
            VideoStreamingCheck(path);
        }

        kCharSkilMovielTex.gameObject.SetActive(bActive);
        kCharSkillSpr.gameObject.SetActive(!bActive);
    }

    
    private void VideoStreamingCheck(string path)
    {
        //        if(string.IsNullOrEmpty(path) == true)
        //        {
        //            _videoPlayer.Play();
        //        }
        //        else
        //        {
        //#if UNITY_EDITOR
        //            _videoPlayer.url = "file://" + Application.dataPath + "/StreamingAssets/" + path;
        //#elif UNITY_ANDROID
        //        _videoPlayer.url = "jar:file://" + Application.dataPath + "!/assets/" + path;
        //#elif UNITY_IOS
        //        _videoPlayer.url = "file://" + Application.dataPath + "/Raw/" + path;
        //#endif

        //            _videoPlayer.Play();
        //        }

        if (string.IsNullOrEmpty(path) == true)
        {
            //Stop -> Play
            _videoPlayer.Stop();
            _videoPlayer.Play();
        }
        else
        {
            //Log.Show(path);
            //VideoClip clip = ResourceMgr.Instance.LoadFromAssetBundle("movie", path) as VideoClip;

            if (videoClipPath != path)
            {
                videoClipPath = path;
                StopCoroutine(CoVideoPlay(videoClipPath));
                StartCoroutine(CoVideoPlay(videoClipPath));
                return;
            }

            if (videoclip)
            {
                kCharSkillSpr.gameObject.SetActive(false);
                _videoPlayer.clip = videoclip;
                _videoPlayer.Play();
            }
            else
            {
                kCharSkillSpr.gameObject.SetActive(true);
                kCharSkilMovielTex.gameObject.SetActive(false);
            }


            //string[] videoClipPaths = path.Split('.');
            //if (videoClipPaths[0] != videoClipPath)
            //{
            //    videoClipPath = videoClipPaths[0];
            //    videoclip = Resources.Load(videoClipPath) as VideoClip;
            //}
            //else
            //{
            //    videoClipPath = videoClipPaths[0];                
            //    if (videoclip == null)
            //        videoclip = Resources.Load(videoClipPath) as VideoClip;
            //}

            //if (videoclip)
            //{
            //    kCharSkillSpr.gameObject.SetActive(false);
            //    _videoPlayer.clip = videoclip;
            //    _videoPlayer.Play();
            //}
            //else
            //{
            //    kCharSkillSpr.gameObject.SetActive(true);
            //    kCharSkilMovielTex.gameObject.SetActive(false);
            //}
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
            kCharSkillSpr.gameObject.SetActive(false);
            _videoPlayer.clip = videoclip;
            _videoPlayer.Play();
        }
        else
        {
            kCharSkillSpr.gameObject.SetActive(true);
            kCharSkilMovielTex.gameObject.SetActive(false);
        }
    }

    public void OnNetCharSkillEquip(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("CharInfoPanel");
        LobbyUIManager.Instance.Renewal("PresetPopup");

        OnClickClose();
        VoiceMgr.Instance.PlayChar(eVOICECHAR.SkillSet, _chardata.TableID);
        //Renewal(true);

        if (GameSupport.IsTutorial())
            GameSupport.TutorialNext();
    }


}
