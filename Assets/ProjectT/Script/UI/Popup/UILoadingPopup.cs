using UnityEngine;
using System.Collections;

public class UILoadingPopup : FComponent
{
    public enum eLoadingType
    {
        NONE = 0,
        LobbyToStage = 1,
        StageToLobby = 2,
        LobbyToTitle = 3,
        LobbyToArena = 4,
        LobbyToTraining = 5,
    }

    public UILabel kNameLabel;
    public UILabel kDescLabel;
    public UISprite kGaugeSpr;
    public UITexture kBGTex;
    public GameObject kLoadingToolTipObj;

    private int _loadingtipgroup = 1;
    public override void OnEnable()
    {
        base.OnEnable();

        SoundManager.Instance.StopBgm();
        SoundManager.Instance.StopAmbienceSnd();

        kGaugeSpr.fillAmount = 0.0f;

        if (kLoadingToolTipObj != null)
            kLoadingToolTipObj.SetActive(true);

        int loadingtipgroup = 1;
        int type = (int)eLoadingType.NONE;
        kBGTex.mainTexture = null;
        var objtype = UIValue.Instance.GetValue(UIValue.EParamType.LoadingType);
        if (objtype != null)
            type = (int)objtype;

        if (type == (int)eLoadingType.LobbyToStage)
        {
            var obj = UIValue.Instance.GetValue(UIValue.EParamType.LoadingStage);
            if (obj != null)
            {
                int stageid = (int)obj;
                var stagedata = GameInfo.Instance.GameTable.FindStage((int)obj);
                if(stagedata != null)
                {
                    if (stagedata.StageType == (int)eSTAGETYPE.STAGE_EVENT || stagedata.StageType == (int)eSTAGETYPE.STAGE_EVENT_BONUS)
                    {
                        EventSetData eventSetData = GameInfo.Instance.GetEventSetData(stagedata.TypeValue);
                        if (eventSetData != null)
                        {
                            BannerData bannerdataBG = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.EVENT_STAGEBG && x.BannerTypeValue == eventSetData.TableID);
                            if(bannerdataBG != null)
                            {
								if (kBGTex.mainTexture != null)
								{
									DestroyImmediate(kBGTex.mainTexture, false);
									kBGTex.mainTexture = null;
								}

                                kBGTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(bannerdataBG.UrlImage, true, bannerdataBG.Localizes[(int)eBannerLocalizeType.Url]);
                            }
                            //kBGTex.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("story", eventSetData.TableData.StageBG) as Texture;
                            SetBGColor(1);
                        }
                    }
                    else if (stagedata.StageType == (int)eSTAGETYPE.STAGE_MAIN_STORY || stagedata.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK)
                    {
                        var chapterdata = GameInfo.Instance.GameClientTable.FindChapter(stagedata.Chapter);
                        if (chapterdata != null)
                        {
                            kBGTex.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("story", chapterdata.Bg) as Texture;
                            SetBGColor(stagedata.Difficulty);
                        }
                    }
                    else if(stagedata.StageType == (int)eSTAGETYPE.STAGE_SPECIAL)
                    {
                        if (kLoadingToolTipObj != null)
                            kLoadingToolTipObj.SetActive(false);
                        kBGTex.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("story", stagedata.Icon) as Texture;
                        SetBGColor(1);
                    }
                    else
                    {
                        kBGTex.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("story", stagedata.Icon) as Texture;
                        SetBGColor(1);
                    }
                    loadingtipgroup = stagedata.LoadingTip;
                }
            }
        }
        else if (type == (int)eLoadingType.StageToLobby)
        {
            loadingtipgroup = 2;
        }
        else if (type == (int)eLoadingType.LobbyToTitle)
        {
            loadingtipgroup = 3;
        }
        else if (type == (int)eLoadingType.LobbyToArena)
        {
            loadingtipgroup = 4;
        }
        else if (type == (int)eLoadingType.LobbyToTraining)
        {
            loadingtipgroup = 5;
        }

        if (kBGTex.mainTexture == null)
        {
            if (GameInfo.Instance.GameConfig.LoadingPopupBGList.Count >= type)
            {
                kBGTex.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("story", GameInfo.Instance.GameConfig.LoadingPopupBGList[type]) as Texture;
            }
            else
            {
                kBGTex.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("story", "Story/BG/AT_LOBBY.png") as Texture;
            }
            SetBGColor(1);
        }

        _loadingtipgroup = loadingtipgroup;

        kNameLabel.textlocalize = "";
        kDescLabel.textlocalize = "";

        SetTextTooltip();
    }

    private void SetTextTooltip()
    {
        var list = GameInfo.Instance.GameClientTable.FindAllLoadingTip(x => x.GroupID == _loadingtipgroup);
        if (list.Count == 0)
            return;

        int rand = Random.Range(0, list.Count);

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(list[rand].Name);
        kDescLabel.textlocalize = FLocalizeString.Instance.GetText(list[rand].Desc);
    }

    public void Update()
    {
        if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Select))//Input.GetMouseButtonDown(0))
        {
            SetTextTooltip();
        }

        if (AppMgr.Instance.Async == null)
        {
            SetUIActive(false);
        }
        else
        {
            kGaugeSpr.fillAmount = AppMgr.Instance.Async.progress;
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        //if (FGUIManager.Instance.IsTutorial())
        //    return;
        /*
        int move = (int)UIParameter.Instance.GetParameter(UIParameter.EParamType.Loading);
        if (move == 0)
        {
            FGUIManager.Instance.LobbyNoticePopup();
        }
        */
    }

    private void SetBGColor(int difficulty)
    {
        if (kBGTex == null)
            return;

        Material mtrl = kBGTex.material;
        if (difficulty == 1) // Easy
        {
            kBGTex.shader = null;
            kBGTex.color = Color.white;
        }
        else if (difficulty == 2) // Normal
        {
            kBGTex.shader = Shader.Find("eTOYLab/ColorDodge");
            kBGTex.color = new Color(0.549f, 0.482f, 0.819f);
        }
        else if (difficulty == 3) // Hard
        {
            kBGTex.shader = Shader.Find("eTOYLab/ColorDodge");
            kBGTex.color = new Color(0.85f, 0.45f, 0.45f);
        }
    }

    public override bool IsBackButton()
    {
        return false;
    }
}
