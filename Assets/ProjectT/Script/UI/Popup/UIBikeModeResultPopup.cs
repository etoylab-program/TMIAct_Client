using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
public class BikeModeResultPopup
{
    static UIBikeModeResultPopup kPopup;

    public static UIBikeModeResultPopup GetBikeResultPopup()
    {
        return GameUIManager.Instance.ShowUI("BikeModeResultPopup", true) as UIBikeModeResultPopup;
    }

    public static void Show(bool _result)
    {
        UIBikeModeResultPopup mpopup = GetBikeResultPopup();
        if (mpopup == null)
            return;

        mpopup.InitBikeResultPopup(_result);
    }

    public static void Show(bool _result, int _lifeCnt, ObscuredInt _goldCnt)
    {
        UIBikeModeResultPopup mpopup = GetBikeResultPopup();
        if (mpopup == null)
            return;

        mpopup.InitBikeResultPopup(_result, _lifeCnt, _goldCnt);
    }

    public static void ScoreResultShot(bool result, ObscuredInt score, ObscuredInt goleCnt)
    {
        UIBikeModeResultPopup mpopup = GetBikeResultPopup();
        if (mpopup == null)
            return;

        mpopup.InitScoreResultPopup(result, score, goleCnt);
    }
}

public class UIBikeModeResultPopup : FComponent
{
	public GameObject kSuccess;
	public UILabel kSuccessTitleLabel;
	public UITexture kLightTex;
	public UILabel kGoldLabel;
	public UILabel kCashLabel;
    public GameObject kGetCashObj;
	public GameObject kFailed;
	public UILabel kFailedTitleLabel;
	public UIButton kReStartBtn;
	public UILabel kTicketLabel;

    public GameObject kPlayerHpObjRoot;
    public List<Animation> kPlayerHpAnims;
    List<GameObject> kPlayerHpObj = new List<GameObject>();

    public GameObject kScoreObj;
    public UILabel kScoreLabel;
    private ObscuredInt _score = 0;

    private ObscuredInt _goldCnt = 0;
    private int _cashCnt = 0;

    private bool _animEnded = false;
    private float _hpAniStartTime = 2.3f;

 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}

    public void InitBikeResultPopup(bool _result)
    {
        kReStartBtn.gameObject.SetActive(false);
        
        kSuccess.SetActive(_result);
        kFailed.SetActive(!_result);
    }

    public void InitBikeResultPopup(bool result, int lifeCnt, ObscuredInt goldCnt)
    {
        InitBikeResultPopup(result);

        if(kScoreObj != null)
            kScoreObj.SetActive(false);

        if (result)
        {
            StartCoroutine(PlaySuccessAni(lifeCnt, goldCnt));
            //  바이크 모드시에만 시간을 기록합니다.
            kGetCashObj.gameObject.SetActive(false);
            if (World.Instance.StageData.StageType == (int)eSTAGETYPE.STAGE_SPECIAL && World.Instance.StageData.Chapter == (int)eSTAGE_SPECIAL_TYPE.BIKE)
            {
                kCashLabel.textlocalize = string.Format("x{0}", GameInfo.Instance.GameConfig.SpecialModeRewardCash);
            }
        }
        else
        {
            StartCoroutine(PlayFailedAni());
        }

        AppMgr.Instance.CustomInput.ShowCursor(true);
    }

    public void InitScoreResultPopup(bool result, ObscuredInt score, ObscuredInt goldCnt)
    {
        InitBikeResultPopup(result);
        if(result)
        {
            kScoreObj.SetActive(true);
            kPlayerHpObjRoot.SetActive(false);
            StartCoroutine(PlayThrowSuccessAni(goldCnt, score));
            kGetCashObj.gameObject.SetActive(false);
            if (World.Instance.StageData.StageType == (int)eSTAGETYPE.STAGE_SPECIAL && World.Instance.StageData.Chapter == (int)eSTAGE_SPECIAL_TYPE.THROW)
            {
                kCashLabel.textlocalize = string.Format("x{0}", GameInfo.Instance.GameConfig.SpecialModeRewardCash);
            }
        }
        else
        {
            StartCoroutine(PlayFailedAni());
        }
    }

    private void Update()
    {
        if (kScoreObj != null && kScoreObj.activeSelf)
        {
            kScoreLabel.textlocalize = string.Format("{0:#,##0}", _score);
        }
        kGoldLabel.textlocalize = string.Format("x{0:#,##0}", _goldCnt);
    }


    IEnumerator PlaySuccessAni(int lifeCnt, ObscuredInt goldCnt)
    {
        _animEnded = true;
        _goldCnt = 0;
        kGoldLabel.textlocalize = string.Format("x{0:#,##0}", _goldCnt);

        float delay = 0;
        delay = PlayAnimtion(0);
        
        //할당된 오브젝트를 전부 꺼준다.
        if (kPlayerHpAnims != null && kPlayerHpAnims.Count > 0)
        {
            for (int i = 0; i < kPlayerHpAnims.Count; i++)
            {
                kPlayerHpObj[i].SetActive(false);
            }
        }
        yield return new WaitForSeconds(_hpAniStartTime);
        //남은 라이프 만큼 켜줌과 동시에 애니메이션 실행(하트가 차는 애니메이션)
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
        for (int i = 0; i < lifeCnt; i++)
        {
            kPlayerHpAnims[i].Play("BikeModeReward_HP");
            kPlayerHpObj[i].SetActive(true);

            //다음 하트가 나오는 시간(명랑님과 확인완료)
            yield return waitForSeconds;
        }
        yield return StartCoroutine(Utility.UpdateCoroutineValue((x) => _goldCnt = (ObscuredInt)x, _goldCnt, goldCnt, 0.5f));

        if (World.Instance.StageData.StageType == (int)eSTAGETYPE.STAGE_SPECIAL)
        {
            //골드 연출 끝날때까지 대기
            yield return new WaitForSeconds(0.4f);
            kGetCashObj.SetActive(true);
        }
            

        _animEnded = false;
    }

    IEnumerator PlayFailedAni()
    {
        _animEnded = true;
        float delay = 0;
        PlayAnimtion(1);
        while (!IsPlayAnimtion())
            yield return null;

        
        _animEnded = false;
    }

    IEnumerator PlayThrowSuccessAni(ObscuredInt goldCnt, ObscuredInt score)
    {
        _animEnded = true;
        _goldCnt = 0;
        _score = 0;
        kGoldLabel.textlocalize = string.Format("x{0:#,##0}", _goldCnt);

        float delay = 0;
        delay = PlayAnimtion(0);

        ////할당된 오브젝트를 전부 꺼준다.
        //if (kPlayerHpAnims != null && kPlayerHpAnims.Count > 0)
        //{
        //    for (int i = 0; i < kPlayerHpAnims.Count; i++)
        //    {
        //        kPlayerHpObj[i].SetActive(false);
        //    }
        //}

        yield return StartCoroutine(Utility.UpdateCoroutineValue((x) => _score = (ObscuredInt)x, _score, score, 0.5f));

        yield return StartCoroutine(Utility.UpdateCoroutineValue((x) => _goldCnt = (ObscuredInt)x, _goldCnt, goldCnt, 0.5f));

        if (World.Instance.StageData.StageType == (int)eSTAGETYPE.STAGE_SPECIAL)
        {
            //골드 연출 끝날때까지 대기
            yield return new WaitForSeconds(0.4f);
            kGetCashObj.SetActive(true);
        }


        _animEnded = false;
    }
 
	public override void InitComponent()
	{
        if (kPlayerHpAnims != null && kPlayerHpAnims.Count > 0)
        {
            
            for(int i = 0; i < kPlayerHpAnims.Count; i++)
            {
                kPlayerHpObj.Add(kPlayerHpAnims[i].transform.GetChild(0).gameObject);
            }
        }
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}
 
	//public void OnClick_ReStartBtn()
	//{
	
	//}
	
	public void OnClick_ExitBtn()
	{
        StartCoroutine("LoadLobby");
    }

    private IEnumerator LoadLobby()
    {
        yield return new WaitForSeconds(0.1f);

        UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.StageToLobby);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, -1);
        GameUIManager.Instance.ShowUI("LoadingPopup", false);
        AppMgr.Instance.LoadScene(AppMgr.eSceneType.Lobby, "Lobby");
    }
}
