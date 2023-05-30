using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectorUIManager : FMonoSingleton<DirectorUIManager>
{
    public delegate void OnDirectorUICallBack();
    public enum eDUTYPE
    {
        NONE = 0,
        GACHA,
        GACHAGOLD,
        GACHARESTORATION,
        CHARBUY,
        CHARGRADEUP,
        MAXLEVEL,
        WAKEUPCARD,
        WAKEUPWEAPON,
        WAKEUPGEM,
    }
    private eDUTYPE _etype = eDUTYPE.NONE;
    private Director _directorGacha01 = null;
    private Director _directorGacha05 = null;
    private Director _directorGacha10 = null;
    private Director _directorGachaGold = null;
    private Director _directorGachaRestoration = null;
    private GachaDirector _curGachaDirector = null;
    private Director _curOpenDirector = null;
    private Director _directorOpenN = null;
    private Director _directorOpenSR = null;
    private Director _directorOpenUR = null;
    private float _checkLookTime = 0.0f;
    private Director _directorcharbuy = null;
    private Director _directorchargradeup = null;
    private Director _directormaxlevel = null;
    private Director _directoritemwakeup = null;
    private OnDirectorUICallBack directoruicallback = null;

    private Coroutine mCr = null;
    private Coroutine mGachaCr = null;
    private Coroutine mGachaOpenCr = null;
    void Awake()
    {
        //DontDestroyOnLoad();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  로딩
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void LoadGacha()
    {
        if (_directorGacha01 == null)
        {
            _directorGacha01 = ResourceMgr.Instance.CreateFromAssetBundle<Director>("director", "director/drt_gacha_01s.prefab");
            _directorGacha01.transform.parent = this.gameObject.transform;
        }

        if (_directorGacha05 == null)
        {
            _directorGacha05 = ResourceMgr.Instance.CreateFromAssetBundle<Director>("director", "director/drt_gacha_05s.prefab");
            _directorGacha05.transform.parent = this.gameObject.transform;
        }

        if (_directorGacha10 == null)
        {
            _directorGacha10 = ResourceMgr.Instance.CreateFromAssetBundle<Director>("director", "director/drt_gacha_10s.prefab");
            _directorGacha10.transform.parent = this.gameObject.transform;
        }

        if (_directorOpenN == null)
        {
            _directorOpenN = ResourceMgr.Instance.CreateFromAssetBundle<Director>("director", "director/drt_gacha_open_n.prefab");
            _directorOpenN.Init(null);
            _directorOpenN.transform.parent = this.gameObject.transform;
        }

        if (_directorOpenSR == null)
        {
            _directorOpenSR = ResourceMgr.Instance.CreateFromAssetBundle<Director>("director", "director/drt_gacha_open_sr.prefab");
            _directorOpenSR.Init(null);
            _directorOpenSR.transform.parent = this.gameObject.transform;
        }

        if (_directorOpenUR == null)
        {
            _directorOpenUR = ResourceMgr.Instance.CreateFromAssetBundle<Director>("director", "director/drt_gacha_open_ur.prefab");
            _directorOpenUR.Init(null);
            _directorOpenUR.transform.parent = this.gameObject.transform;
        }

        _directorGacha01.gameObject.SetActive(false);
        _directorGacha05.gameObject.SetActive(false);
        _directorGacha10.gameObject.SetActive(false);
        _directorOpenN.gameObject.SetActive(false);
        _directorOpenSR.gameObject.SetActive(false);
        _directorOpenUR.gameObject.SetActive(false);
    }

    public void LoadGachaGold()
    {
        if (_directorGachaGold == null)
        {
            _directorGachaGold = ResourceMgr.Instance.CreateFromAssetBundle<Director>("director", "director/drt_gacha_gold.prefab");
            _directorGachaGold.transform.parent = this.gameObject.transform;
        }
        _directorGachaGold.gameObject.SetActive(false);
    }

    public void LoadGachaRestoration()
    {
        if (_directorGachaRestoration == null)
        {
            _directorGachaRestoration = ResourceMgr.Instance.CreateFromAssetBundle<Director>("director", "director/drt_gacha_gold_02.prefab");
            _directorGachaRestoration.transform.parent = this.gameObject.transform;
        }
        _directorGachaRestoration.gameObject.SetActive(false);
    }

    public bool LoadCharBuy(int tableid)
    {
        var tabledata = GameInfo.Instance.GameTable.FindCharacter(tableid);
        if (tabledata == null)
            return false;
        string path = string.Empty;
        if (AppMgr.Instance.ResPlatform == AppMgr.eResPlatform.aos)
            path = string.Format("director/{0}_aos.prefab", tabledata.BuyDrt);
        else
            path = string.Format("director/{0}.prefab", tabledata.BuyDrt);


        if (_directorcharbuy != null)
            DestroyImmediate(_directorcharbuy.gameObject);

        if (_directorcharbuy == null)
        {
            _directorcharbuy = ResourceMgr.Instance.CreateFromAssetBundle<Director>("director", path);
            if (_directorcharbuy == null)
                return false;

            _directorcharbuy.transform.parent = this.gameObject.transform;
        }

        _directorcharbuy.gameObject.SetActive(false);
        return true;
    }

    public bool LoadCharGradeUp(int tableid)
    {
        var tabledata = GameInfo.Instance.GameTable.FindCharacter(tableid);
        if (tabledata == null)
            return false;
        string path = string.Empty;
        if (AppMgr.Instance.ResPlatform == AppMgr.eResPlatform.aos)
            path = string.Format("director/{0}_aos.prefab", tabledata.GradeUpDrt);
        else
            path = string.Format("director/{0}.prefab", tabledata.GradeUpDrt);

        if (_directorchargradeup != null)
            DestroyImmediate(_directorchargradeup.gameObject);

        if (_directorchargradeup == null)
        {
            _directorchargradeup = ResourceMgr.Instance.CreateFromAssetBundle<Director>("director", path);
            _directorchargradeup.transform.parent = this.gameObject.transform;
        }
        _directorchargradeup.gameObject.SetActive(false);
        return true;
    }

    public bool LoadMaxLevel()
    {
        string path = string.Format("director/drt_item_outlook_change.prefab");

        if (_directormaxlevel != null)
            DestroyImmediate(_directormaxlevel.gameObject);

        if (_directormaxlevel == null)
        {
            _directormaxlevel = ResourceMgr.Instance.CreateFromAssetBundle<Director>("director", path);
            _directormaxlevel.transform.parent = this.gameObject.transform;
        }
        _directormaxlevel.gameObject.SetActive(false);
        return true;
    }


    public bool LoadItemWakeUp()
    {
        string path = string.Format("director/drt_item_gradeup.prefab");

        if (_directoritemwakeup != null)
            DestroyImmediate(_directoritemwakeup.gameObject);

        if (_directoritemwakeup == null)
        {
            _directoritemwakeup = ResourceMgr.Instance.CreateFromAssetBundle<Director>("director", path);
            _directoritemwakeup.transform.parent = this.gameObject.transform;
        }
        _directoritemwakeup.gameObject.SetActive(false);
        return true;
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  가차 관련 연출
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void PlayGacha(int type)
    {
        _etype = eDUTYPE.GACHA;
        directoruicallback = null;

        LoadGacha();

        mGachaCr = StartCoroutine(GachaResultCoroutine(type));
    }

    public void SkipGacha()
    {
        if(mGachaCr != null)
            StopCoroutine(mGachaCr);
        if(mGachaOpenCr != null)
            StopCoroutine(mGachaOpenCr);

        mGachaCr = null;
        mGachaOpenCr = null;

        
        LobbyUIManager.Instance.ShowTemporaryActiveUI();
        LobbyUIManager.Instance.Renewal("GachaPanel");
        LobbyUIManager.Instance.HideUI("GachaConfirmPopup", false);
        if (null != _curGachaDirector && _curGachaDirector.gameObject.activeSelf)
            _curGachaDirector.End();
        if(_curOpenDirector != null)
            _curOpenDirector.End();

        SoundManager.Instance.PlayUISnd(19);

        LobbyUIManager.Instance.ShowUI("GachaResultPopup", true);
        UIValue.Instance.RemoveValue(UIValue.EParamType.GachaRewardIndex);
        SoundManager.Instance.PlayBgm(Lobby.Instance.GetCurrentBGMID(), 1.0f);
    }



    public void PlayGachaNewCardGreeings()
    {
        StartCoroutine(GachaNewCardGreeingsCoroutine());
    }

    public void NextGachaNewCardGreeings()
    {
        _checkLookTime = 100.0f;
    }

    private IEnumerator GachaResultCoroutine(int type)
    {
        LobbyUIManager.Instance.HideTemporaryActiveUI(null);
        SoundManager.Instance.StopBgm();

        Director curDirector = null;
        if (type == 1)
            curDirector = _directorGacha01;
        else if (type == 2)
            curDirector = _directorGacha05;
        else
            curDirector = _directorGacha10;

        // 첫번째 연출
        _curGachaDirector = curDirector.GetComponentInChildren<GachaDirector>();
        _curGachaDirector.Init(null);
        _curGachaDirector.Play();
        LobbyUIManager.Instance.ShowUI("GachaConfirmPopup", false);

        while (curDirector.isEnd == false)
            yield return null;

        LobbyUIManager.Instance.HideUI("GachaConfirmPopup", false);

        curDirector.gameObject.SetActive(false);
        AppMgr.Instance.CustomInput.ShowCursor(true);

        mGachaOpenCr = StartCoroutine("GachaCardOpen");

        yield return null;
    }

    private IEnumerator GachaCardOpen()
    {
        _curOpenDirector = null;

        int grade_N = (int)eGRADE.GRADE_R;
        int grade_SR = (int)eGRADE.GRADE_SR;
        int grade_UR = (int)eGRADE.GRADE_UR;
        for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++)
        {
            if (GameInfo.Instance.RewardList[i].Type == (int)eREWARDTYPE.CARD)
            {
                grade_N = (int)eGRADE.GRADE_R;
                grade_SR = (int)eGRADE.GRADE_SR;
                grade_UR = (int)eGRADE.GRADE_UR;
            }
            else if (GameInfo.Instance.RewardList[i].Type == (int)eREWARDTYPE.WEAPON)
            {
                grade_N = (int)eGRADE.GRADE_R;
                grade_SR = (int)eGRADE.GRADE_SR;
                grade_UR = (int)eGRADE.GRADE_UR;
            }
            else if (GameInfo.Instance.RewardList[i].Type == (int)eREWARDTYPE.GEM)
            {
                grade_N = (int)eGRADE.GRADE_N;
                grade_SR = (int)eGRADE.GRADE_R;
                grade_UR = (int)eGRADE.GRADE_SR;
            }
            else if (GameInfo.Instance.RewardList[i].Type == (int)eREWARDTYPE.ITEM)
            {
                grade_N = (int)eGRADE.GRADE_N;
                grade_SR = (int)eGRADE.GRADE_R;
                grade_UR = (int)eGRADE.GRADE_SR;
            }

            if (_curGachaDirector.listGrade[i] <= grade_N)
                _curOpenDirector = _directorOpenN;
            else if (_curGachaDirector.listGrade[i] <= grade_SR)
                _curOpenDirector = _directorOpenSR;
            else if (_curGachaDirector.listGrade[i] == grade_UR)
                _curOpenDirector = _directorOpenUR;

            UIValue.Instance.SetValue(UIValue.EParamType.GachaGreetings, true);
            UIValue.Instance.SetValue(UIValue.EParamType.GachaRewardIndex, i);
            LobbyUIManager.Instance.ShowUI("GachaConfirmPopup", false);

            GachaItemOpen gachaItemOpen = _curOpenDirector.GetComponent<GachaItemOpen>();
            gachaItemOpen.Init(GameInfo.Instance.RewardList[i]);

            _curOpenDirector.Init(null);
            _curOpenDirector.Play();
            while (!_curOpenDirector.isEnd)
                yield return null;

            LobbyUIManager.Instance.HideUI("GachaConfirmPopup", false);
        }

        SoundManager.Instance.PlayUISnd(19);

        LobbyUIManager.Instance.ShowTemporaryActiveUI();
        SoundManager.Instance.PlayBgm(Lobby.Instance.GetCurrentBGMID(), 1.0f);

        LobbyUIManager.Instance.Renewal("GachaPanel");

        LobbyUIManager.Instance.ShowUI("GachaResultPopup", true);

        yield return null;
    }

    private IEnumerator GachaNewCardGreeingsCoroutine()
    {
        LobbyUIManager.Instance.HideTemporaryActiveUI(null);

        float closepopuptime = LobbyUIManager.Instance.GetUI("GachaGreetingsPopup").GetCloseAniTime();

        UIGachaPanel gachaPanel = LobbyUIManager.Instance.GetUI<UIGachaPanel>("GachaPanel");

        for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++)
        {
            CardData card = GameInfo.Instance.GetCardData(GameInfo.Instance.RewardList[i].UID);
            if (card == null)
                continue;

            if (!GameInfo.Instance.RewardList[i].bNew) //@@ 테스트용 주석
                continue;

            if (card.TableData.Greetings == -1)
                continue;

            var data = gachaPanel.CardBookList.Find(x => x.TableID == GameInfo.Instance.RewardList[i].Index);
            if (data != null)
                continue;

            gachaPanel.CardBookList.Add(new CardBookData(GameInfo.Instance.RewardList[i].Index, 0));

            var cilp = VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.Greetings, card.TableID);

            UIValue.Instance.SetValue(UIValue.EParamType.GachaGreetings, true);
            UIValue.Instance.SetValue(UIValue.EParamType.GachaRewardIndex, i);
            LobbyUIManager.Instance.ShowUI("GachaGreetingsPopup", true);

            float fmaxtime = 5.0f;
            if (cilp != null)
                if (cilp.clip != null)
                    fmaxtime = cilp.clip.length + 1.0f;


            _checkLookTime = 0.0f;
            while (_checkLookTime < fmaxtime)
            {
                _checkLookTime += Time.deltaTime;
                yield return null;
            }

            yield return null;

            LobbyUIManager.Instance.HideUI("GachaGreetingsPopup", false);
            SoundManager.Instance.StopVoice();
        }

        //  보상을 모두 확인 하였으므로 보상리스트를 클리어합니다.
        for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++)
            GameInfo.Instance.RewardList[i].bNew = false;


        SoundManager.Instance.PlayUISnd(19);

        SoundManager.Instance.PlayBgm(Lobby.Instance.GetCurrentBGMID(), 1.0f);

        LobbyUIManager.Instance.ShowTemporaryActiveUI();

        LobbyUIManager.Instance.InitComponent("GachaResultPopup");
        LobbyUIManager.Instance.Renewal("GachaResultPopup");

    }

    public int GetGachaNewCount()
    {
        UIGachaPanel gachaPanel = LobbyUIManager.Instance.GetUI<UIGachaPanel>("GachaPanel");
        int count = 0;
        for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++)
        {
            CardData card = GameInfo.Instance.GetCardData(GameInfo.Instance.RewardList[i].UID);
            if (card == null)
                continue;

            if (!GameInfo.Instance.RewardList[i].bNew) //@@ 테스트용 주석
                continue;

            if (card.TableData.Greetings == -1)
                continue;

            var data = gachaPanel.CardBookList.Find(x => x.TableID == GameInfo.Instance.RewardList[i].Index);
            if (data != null)
                continue;

            count += 1;
        }

        return count;
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  골드 가챠 연출
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void PlayGachaGold(bool bAuto)
    {
        _etype = eDUTYPE.GACHAGOLD;
        directoruicallback = null;

        LoadGachaGold();

        StartCoroutine(GachaGoldResultCoroutine(bAuto));
    }

    // 복각 가챠
    public void PlayGachaRestoration()
    {
        _etype = eDUTYPE.GACHARESTORATION;
        directoruicallback = null;

        LoadGachaRestoration();

        StartCoroutine(GachaGoldResultCoroutine(false));
    }

    public void SkipGachaGold()
    {
        if (_etype == eDUTYPE.GACHAGOLD)
        {
            _directorGachaGold.End();
        }
        else if(_etype == eDUTYPE.GACHARESTORATION)
        {
            _directorGachaRestoration.End();
        }
    }

    private IEnumerator GachaGoldResultCoroutine(bool bAuto)
    {
        LobbyUIManager.Instance.HideTemporaryActiveUI(null);

        SoundManager.Instance.StopBgm();

        Director curDirector = null;
        if (_etype == eDUTYPE.GACHAGOLD)
        {
            curDirector = _directorGachaGold;
        }
        else if(_etype == eDUTYPE.GACHARESTORATION)
        {
            curDirector = _directorGachaRestoration;
        }

        curDirector.Init(null);
        curDirector.Play();

        while (curDirector.isEnd == false)
            yield return null;

        curDirector.gameObject.SetActive(false);

        if (bAuto == true) {
            UIAutoGachaResultPopup autoGachaResultPopup = LobbyUIManager.Instance.GetUI<UIAutoGachaResultPopup>("AutoGachaResultPopup");
            if (autoGachaResultPopup == null) {
                Debug.LogError("오토 가챠 결과 팝업 없음");
            }
            else {
                //LobbyUIManager.Instance.ShowUI("AutoGachaResultPopup", true);

                // Renewal 로 갱신해도 되나,빠르게 연출 Skip 시 Renewal 이 호출이 가끔 안되는 문제가 있어서 분리함.
                autoGachaResultPopup.SetUIActive(true);
                autoGachaResultPopup.RefreshGachaResult();
            }
        }
        else {
            LobbyUIManager.Instance.ShowUI("GachaResultPopup", true);
        }

        yield return new WaitForSeconds(0.2f);
        LobbyUIManager.Instance.ShowTemporaryActiveUI();

        SoundManager.Instance.PlayBgm(Lobby.Instance.GetCurrentBGMID(), 1.0f);
        AppMgr.Instance.CustomInput.ShowCursor(true);

        yield return null;
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  보상 개봉 연출
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void PlayRewardOpen(string Title, string Msg, List<RewardData> rewards, OnMessageRewardCallBack callback = null)
    {
        _etype = eDUTYPE.GACHAGOLD;
        directoruicallback = null;

        LoadGachaGold();

        StartCoroutine(RewardOpenResultCoroutine(Title, Msg, rewards, callback));
    }

    private IEnumerator RewardOpenResultCoroutine(string Title, string Msg, List<RewardData> rewards, OnMessageRewardCallBack callback)
    {
        LobbyUIManager.Instance.HideTemporaryActiveUI(null);

        SoundManager.Instance.StopBgm();

        Director curDirector = _directorGachaGold;

        curDirector.Init(null);
        curDirector.Play();

        while (curDirector.isEnd == false)
            yield return null;

        curDirector.gameObject.SetActive(false);

        MessageRewardListPopup.RewardListMessage(Title, Msg, rewards, callback);
        yield return new WaitForSeconds(0.2f);
        LobbyUIManager.Instance.ShowTemporaryActiveUI();

        SoundManager.Instance.PlayBgm(Lobby.Instance.GetCurrentBGMID(), 1.0f);
        AppMgr.Instance.CustomInput.ShowCursor(true);

        yield return null;
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  캐릭터 구매 관련 연출
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void PlayCharBuy(int tableid, OnDirectorUICallBack callback)
    {
        if (!LoadCharBuy(tableid))
            return;
        _etype = eDUTYPE.CHARBUY;
        directoruicallback = callback;

        Utility.StopCoroutine(this, ref mCr);
        mCr = StartCoroutine(CharBuyCoroutine());
    }

    public void StopCharBuy()
    {
        if (_directorcharbuy == null)
            return;

        _directorcharbuy.End();

        _directorcharbuy.gameObject.SetActive(false);
        LobbyUIManager.Instance.ShowTemporaryActiveUI();
        LobbyUIManager.Instance.HideUI("DirectorCommonPopup", true);
        SoundManager.Instance.PlayBgm(Lobby.Instance.GetCurrentBGMID(), 1.0f);

        if (directoruicallback != null)
            directoruicallback();
    }


    private IEnumerator CharBuyCoroutine()
    {
        LobbyUIManager.Instance.HideTemporaryActiveUI(null);
        SoundManager.Instance.StopBgm();

        LobbyUIManager.Instance.ShowUI("DirectorCommonPopup", true);
        UIDirectorCommonPopup popup = LobbyUIManager.Instance.GetActiveUI<UIDirectorCommonPopup>("DirectorCommonPopup");
        popup.SetUIType(UIDirectorCommonPopup.eUITYPE.NONE);

        _directorcharbuy.Init(null);
        _directorcharbuy.Play();
        float duration = _directorcharbuy.GetDuration();

        Director curDirector = _directorcharbuy;

        //curDirector.gameObject.SetActive(true); // Director 클래스에 Play 함수안에서 켜줌. 미리 켜주면 안됨.
        curDirector.Init(null);
        curDirector.Play();

        //popup.SetUIType(UIDirectorCommonPopup.eUITYPE.SKIP);

        if (LobbyUIManager.Instance.GetActiveUI("BookCharInfoPopup") != null)
        {
            popup.SetUIType(UIDirectorCommonPopup.eUITYPE.SKIP);
        }


        yield return new WaitForSeconds(duration * 0.9f);
        popup.SetUIType(UIDirectorCommonPopup.eUITYPE.TAPTONEXT);

        while (curDirector.isEnd == false)
            yield return null;

        AppMgr.Instance.CustomInput.ShowCursor(true);
        //StopCharBuy로 이동
        //
    }


    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  캐릭터 등급업 관련 연출
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void PlayCharGradeUp(int tableid)
    {
        if (!LoadCharGradeUp(tableid))
            return;

        _etype = eDUTYPE.CHARGRADEUP;
        directoruicallback = null;

        StartCoroutine(CharGradeUpCoroutine());

    }
    public void StopCharGradeUp()
    {
        if (_directorchargradeup == null)
            return;

        _directorchargradeup.End();
    }


    private IEnumerator CharGradeUpCoroutine()
    {
        LobbyUIManager.Instance.HideTemporaryActiveUI(null);
        SoundManager.Instance.StopBgm();

        Director curDirector = _directorchargradeup;

        //curDirector.gameObject.SetActive(true); // Director 클래스에 Play 함수안에서 켜줌. 미리 켜주면 안됨.
        curDirector.Init(null);
        curDirector.Play();

        curDirector.LockInput(true);//진급 연출이 끝나고 확인 버튼이 아닌 다른곳을 터치시 연출이 멈추는 현상때문에 추가

        yield return new WaitForSeconds(curDirector.GetDuration() * 0.4f);
        LobbyUIManager.Instance.ShowUI("ResultCharGradeUpPopup", true);


        while (curDirector.isEnd == false)
            yield return null;

        curDirector.gameObject.SetActive(false);
        LobbyUIManager.Instance.ShowTemporaryActiveUI();
        LobbyUIManager.Instance.HideUI("ResultCharGradeUpPopup", true);
        LobbyUIManager.Instance.HideUI("CharGradeUpPopup", true);
        SoundManager.Instance.PlayBgm(Lobby.Instance.GetCurrentBGMID(), 1.0f);

        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.CHARINFO)
            LobbyUIManager.Instance.Renewal("CharInfoPanel");

        AppMgr.Instance.CustomInput.ShowCursor(true);
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  각성, 레벨 최대치 모양변경
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void PlayMaxLevel()
    {
        if (!LoadMaxLevel())
            return;

        ItemChangeDirector director = _directormaxlevel.GetComponent<ItemChangeDirector>();
        eREWARDTYPE type = (eREWARDTYPE)UIValue.Instance.GetValue(UIValue.EParamType.SelectRewardType);
        long uID = 0;

        switch (type)
        {
            case eREWARDTYPE.WEAPON:
                uID = (long)UIValue.Instance.GetValue(UIValue.EParamType.WeaponUID);
                WeaponData weaponData = GameInfo.Instance.GetWeaponData(uID);

                director.InitWeapon(weaponData);
                break;

            case eREWARDTYPE.CARD:
                uID = (long)UIValue.Instance.GetValue(UIValue.EParamType.CardUID);
                CardData cardData = GameInfo.Instance.GetCardData(uID);

                director.InitCard(cardData);
                break;
        }

        _etype = eDUTYPE.MAXLEVEL;
        directoruicallback = null;

        StartCoroutine(MaxLevelCoroutine());

    }
    public void StopMaxLevel()
    {
        if (_directormaxlevel == null)
            return;

        _directormaxlevel.End();
    }


    private IEnumerator MaxLevelCoroutine()
    {
        LobbyUIManager.Instance.HideTemporaryActiveUI(null);
        SoundManager.Instance.StopBgm();

        LobbyUIManager.Instance.ShowUI("DirectorCommonPopup", true);
        UIDirectorCommonPopup popup = LobbyUIManager.Instance.GetActiveUI<UIDirectorCommonPopup>("DirectorCommonPopup");

        Director curDirector = _directormaxlevel;

        //curDirector.gameObject.SetActive(true); // Director 클래스에 Play 함수안에서 켜줌. 미리 켜주면 안됨.
        curDirector.Init(null);
        curDirector.Play();

        yield return new WaitForSeconds(curDirector.GetDuration());
        popup.SetUIType(UIDirectorCommonPopup.eUITYPE.CONFIRM);

        while (curDirector.isEnd == false)
            yield return null;

        curDirector.gameObject.SetActive(false);
        LobbyUIManager.Instance.ShowTemporaryActiveUI();
        SoundManager.Instance.PlayBgm(Lobby.Instance.GetCurrentBGMID(), 1.0f);

        AppMgr.Instance.CustomInput.ShowCursor(true);
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  무기, 곡옥, 서포터 등급업
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void PlayWeaponWakeUp(WeaponData weapondata)
    {
        if (!LoadItemWakeUp())
            return;

        _etype = eDUTYPE.WAKEUPWEAPON;
        directoruicallback = null;

        ItemWakeUpDirector director = _directoritemwakeup.GetComponent<ItemWakeUpDirector>();
        if (director != null)
            director.InitWeapon(weapondata);

        //200117 - 사운드 연출파일에서 재생
        //SoundManager.Instance.PlayUISnd(52);
        StartCoroutine(ItemWakeUpCoroutine());
    }
    public void PlayCardWakeUp(CardData carddata)
    {
        if (!LoadItemWakeUp())
            return;

        _etype = eDUTYPE.WAKEUPCARD;
        directoruicallback = null;

        ItemWakeUpDirector director = _directoritemwakeup.GetComponent<ItemWakeUpDirector>();
        if (director != null)
            director.InitCard(carddata);

        //200117 - 사운드 연출파일에서 재생
        //SoundManager.Instance.PlayUISnd(66);
        StartCoroutine(ItemWakeUpCoroutine());
    }
    public void PlayGemWakeUp(GemData gemndata)
    {
        if (!LoadItemWakeUp())
            return;

        _etype = eDUTYPE.WAKEUPGEM;
        directoruicallback = null;

        ItemWakeUpDirector director = _directoritemwakeup.GetComponent<ItemWakeUpDirector>();
        if (director != null)
            director.InitGem(gemndata);

        //200117 - 사운드 연출파일에서 재생
        //SoundManager.Instance.PlayUISnd(23);
        StartCoroutine(ItemWakeUpCoroutine());
    }

    public void StopItemWakeUp()
    {
        if (_directoritemwakeup == null)
            return;

        _directoritemwakeup.End();
    }


    private IEnumerator ItemWakeUpCoroutine()
    {
        LobbyUIManager.Instance.HideTemporaryActiveUI(null);
        SoundManager.Instance.StopBgm();

        Director curDirector = _directoritemwakeup;

        //curDirector.gameObject.SetActive(true); // Director 클래스에 Play 함수안에서 켜줌. 미리 켜주면 안됨.
        curDirector.Init(null);
        curDirector.Play();

        float deleytime = curDirector.GetDuration() * 0.6f;
        ItemWakeUpDirector itemwakedirector = _directoritemwakeup.GetComponent<ItemWakeUpDirector>();
        if (itemwakedirector != null)
            deleytime = itemwakedirector.kDelayShowPopup;

        yield return new WaitForSeconds(deleytime);
        if (_etype == eDUTYPE.WAKEUPWEAPON)
            LobbyUIManager.Instance.ShowUI("ResultWeaponGradeUpPopup", true);
        else if (_etype == eDUTYPE.WAKEUPCARD)
            LobbyUIManager.Instance.ShowUI("ResultCardGradeUpPopup", true);
        else if (_etype == eDUTYPE.WAKEUPGEM)
            LobbyUIManager.Instance.ShowUI("ResultGemGradeUpPopup", true);

        while (curDirector.isEnd == false)
            yield return null;

        curDirector.gameObject.SetActive(false);
        LobbyUIManager.Instance.ShowTemporaryActiveUI();
        if (_etype == eDUTYPE.WAKEUPWEAPON)
            LobbyUIManager.Instance.HideUI("ResultWeaponGradeUpPopup", false);
        else if (_etype == eDUTYPE.WAKEUPCARD)
            LobbyUIManager.Instance.HideUI("ResultCardGradeUpPopup", false);
        else if (_etype == eDUTYPE.WAKEUPGEM)
            LobbyUIManager.Instance.HideUI("ResultGemGradeUpPopup", false);
        SoundManager.Instance.PlayBgm(Lobby.Instance.GetCurrentBGMID(), 1.0f);

        AppMgr.Instance.CustomInput.ShowCursor(true);
    }
	//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	//
	//  클릭
	//
	//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	public void PlayNewCardGreeings( List<CardBookData> precardbooklist ) {
		StartCoroutine( NewCardGreeingsCoroutine( precardbooklist ) );
	}

	public bool IsNewCard( RewardData rewardData, ref List<CardBookData> preCardBookList ) {
		if ( rewardData.Type != (int)eREWARDTYPE.CARD ) {
			return false;
		}

		CardData cardData = GameInfo.Instance.GetCardData( rewardData.UID );
		if ( cardData == null ) {
			return false;
		}

		if ( cardData.TableData.Greetings == -1 ) {
			return false;
		}

		CardBookData cardBookData = preCardBookList.Find( x => x.TableID == rewardData.Index );
		if ( cardBookData != null ) {
			return false;
		}

		return true;
	}

	private IEnumerator NewCardGreeingsCoroutine( List<CardBookData> precardbooklist ) {
		LobbyUIManager.Instance.HideTemporaryActiveUI( null );
		for ( int i = 0; i < GameInfo.Instance.RewardList.Count; i++ ) {
			if ( !IsNewCard( GameInfo.Instance.RewardList[i], ref precardbooklist ) ) {
				continue;
			}

			precardbooklist.Add( new CardBookData( GameInfo.Instance.RewardList[i].Index, 0 ) );

			UIValue.Instance.SetValue( UIValue.EParamType.GachaGreetings, false );
			UIValue.Instance.SetValue( UIValue.EParamType.GachaRewardIndex, i );
			LobbyUIManager.Instance.ShowUI( "GachaGreetingsPopup", true );

			float fmaxtime = 5.0f;

			SoundManager.sSoundInfo cilp = VoiceMgr.Instance.PlaySupporter( eVOICESUPPORTER.Greetings, GameInfo.Instance.RewardList[i].Index );
			if ( cilp != null ) {
				if ( cilp.clip != null ) {
					fmaxtime = cilp.clip.length + 1.0f;
				}
			}

			_checkLookTime = 0.0f;
			while ( _checkLookTime < fmaxtime ) {
				_checkLookTime += Time.deltaTime;
				yield return null;
			}

			yield return null;

			LobbyUIManager.Instance.HideUI( "GachaGreetingsPopup", false );
			SoundManager.Instance.StopVoice();
		}

		SoundManager.Instance.PlayUISnd( 19 );
		SoundManager.Instance.PlayBgm( Lobby.Instance.GetCurrentBGMID(), 1.0f );
		LobbyUIManager.Instance.ShowTemporaryActiveUI();
	}

	//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	//
	//  클릭
	//
	//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	public void OnClick_Next()
    {
        Utility.StopCoroutine(this, ref mCr);

        if (_etype == eDUTYPE.CHARBUY)
        {
            StopCharBuy();
        }
        else if (_etype == eDUTYPE.CHARGRADEUP)
        {
            StopCharGradeUp();
        }
        else if (_etype == eDUTYPE.MAXLEVEL)
        {
            StopMaxLevel();
        }
        else if (_etype == eDUTYPE.WAKEUPWEAPON || _etype == eDUTYPE.WAKEUPCARD || _etype == eDUTYPE.WAKEUPGEM)
        {
            StopItemWakeUp();
        }

        AppMgr.Instance.CustomInput.ShowCursor(true);
    }
}


