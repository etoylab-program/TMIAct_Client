
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIArenaPlayPanel : FComponent
{
    public enum eSkillNameType
    {
        Character = 0,
        Weapon,
        Supporter,
    }


    [Serializable]
    public class sCharInfo
    {
        public UIUserCharListSlot   UserCharSlot;
        public UILabel              LbNumber;
        public GameObject           BgObj;
        public GameObject           SpeakObj;
        public GameObject           DefeatObj;
        public GameObject           SelectObj;

        private Vector3 mBgBackPos = Vector3.zero;


        public void Init(Player player, bool isOpponent)
        {
            if (player)
            {
                UserCharSlot.UpdateArenaTeamSlot(0, player.charData, false, true);
            }

            UserCharSlot.transform.localScale = Vector3.one * 0.5f;

            if(!isOpponent)
            {
                mBgBackPos = new Vector3(-14.0f, 0.0f, 0.0f);
            }
            else
            {
                mBgBackPos = new Vector3(14.0f, 0.0f, 0.0f);
            }

            LbNumber.color = new Color(0.356f, 0.356f, 0.356f, 1.0f);
            BgObj.transform.localPosition = mBgBackPos;

            SpeakObj.SetActive(false);
            DefeatObj.SetActive(false);
            SelectObj.SetActive(false);

            if(player == null)
            {
                Empty();
            }
            else if(player.curHp <= 0.0f)
            {
                Defeat();
            }
        }

        public void Empty()
        {
            UserCharSlot.transform.localScale = Vector3.zero;
            BgObj.transform.localPosition = mBgBackPos;
        }

        public void Select()
        {
            UserCharSlot.transform.localScale = Vector3.one * 0.7f;
            LbNumber.color = Color.white;
            BgObj.transform.localPosition = Vector3.zero;

            SelectObj.SetActive(true);
        }

        public void Defeat()
        {
            UserCharSlot.transform.localScale = Vector3.one * 0.5f;
            LbNumber.color = new Color(0.356f, 0.356f, 0.356f, 1.0f);
            BgObj.transform.localPosition = mBgBackPos;

            DefeatObj.SetActive(true);
        }

        public void ShowSpeak(bool show)
        {
            SpeakObj.SetActive(show);
        }
    }

    [Serializable]
    public class sUITeamInfo
    { 
        public UILabel              LbUserRank;
        public UILabel              LbUserName;
        public UIHPGaugeUnit        HpGaugeUnit;
        public UILabel              LbHp;
        public UISprite             SprHpDanger;
        public UIPlayerSpBubble[]   SpBubbles;
        public ParticleSystem       EffMaxSp;
        public Animation            AniSkillName;
        public UISprite             SprSkillName;
        public UILabel              LbSkillName;
        public UILabel              LbCharSkillName;
        public GameObject           IconSupporterObj;
        public UITexture            TexSupporter;
        public GameObject           IconWeaponObj;
        public UITexture            TexWeapon;
        public sCharInfo[]          CharInfos;

        [NonSerialized] public List<string> ListAniName = new List<string>();
        [NonSerialized] public Coroutine    CrSkillName = null;


        public void Init()
        {
            if (ListAniName.Count <= 0)
            {
                foreach (AnimationState aniState in AniSkillName)
                {
                    ListAniName.Add(aniState.clip.name);
                }
            }

            IconSupporterObj.SetActive(false);
            IconWeaponObj.SetActive(false);
        }

        public void InitAniSkillName()
        {
            Color color = SprSkillName.color;
            color.a = 0.01f;
            SprSkillName.color = color;

            color = LbSkillName.color;
            color.a = 0.01f;
            LbSkillName.color = color;

            color = LbCharSkillName.color;
            color.a = 0.01f;
            LbCharSkillName.color = color;

            CrSkillName = null;
        }
    }


    private readonly Color  CHAR_SKILL_NAME_COLOR   = new Color(0.945f, 0.0f, 0.0f, 1.0f);
    private readonly Color  WPN_SKILL_NAME_COLOR    = new Color(0.968f, 0.0f, 1.0f, 1.0f);
    private readonly Color  SUPPORTER_NAME_COLOR    = new Color(0.988f, 0.697f, 0.078f, 1.0f);

    [Header("[Battle Property]")]
    public Animation        AniBattleWin;
    public Animation        AniBattleDefeat;
    public UILabel          LbRemainTime;
    public FTab             TabGameSpeed;
    public UISprite         SprCheer;
    public UILabel          LbCheer;
    public UISprite         SprCheerGauge;
    public UILabel          LbCheerGauge;
    public ParticleSystem   EffCheer;
    public Animation        AniCheer;
    public UILabel          LbCheerBuff;
    public UISprite         SprAtkBuff;
    public UISprite         SprDefBuff;

    [Header("[Supporter]")]
    public GameObject   SupporterObj;
    public UITexture    TexSupporter;
    public UISprite     SprCoolTime;
    public UILabel      LbCoolTime;
    public FToggle      ToggleAutoSupporter;
    public UISprite     SprAutoSupporter;
    public UISprite     SprSupporterSpeak;

    [Header("[Player Team]")]
    public sUITeamInfo PlayerTeamInfo;

    [Header("[Opponent Team]")]
    public sUITeamInfo OpponentTeamInfo;

    public float AddCheerRate           { get; set; }   = 0.0f;
    public float AddCheerRegenRate      { get; set; }   = 0.0f;
    public float AddCheerValueUpRate    { get; set; }   = 0.0f;

    private WorldPVP    mWorldPVP       = null;
    private float       mCurCheerGauge  = 0.0f;


    public override void Awake()
    {
        base.Awake();

        mWorldPVP = World.Instance as WorldPVP;
        TabGameSpeed.EventCallBack = OnTabGameSpeed;
        ToggleAutoSupporter.EventCallBack = OnToggleAuto;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        InitComponent();
        Renewal(true);
    }

    public override void InitComponent()
    {
        base.InitComponent();

        InitBattleUI();

        SetTeamInfo(GameInfo.Instance.UserData.Level, GameInfo.Instance.UserData.GetNickName(), PlayerTeamInfo, mWorldPVP.PlayerChars, false);
        SetTeamInfo(GameInfo.Instance.MatchTeam.UserLv, GameInfo.Instance.MatchTeam.GetUserNickName(), OpponentTeamInfo, mWorldPVP.OpponentChars, true);

        TabGameSpeed.SetTab(GameInfo.Instance.PVPGameSpeedType, SelectEvent.Click);
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        SetCurrentChar(PlayerTeamInfo, mWorldPVP.PlayerChars, mWorldPVP.CurPlayerCharIndex, true);
        SetCurrentChar(OpponentTeamInfo, mWorldPVP.OpponentChars, mWorldPVP.CurOpponentCharIndex, false);

        AniBattleWin.gameObject.SetActive(false);
        AniBattleDefeat.gameObject.SetActive(false);
    }

    public void AddPlayerHp(Player player)
    {
        sUITeamInfo uiTeamInfo = null;
        if (!player.OpponentPlayer)
        {
            uiTeamInfo = PlayerTeamInfo;
        }
        else
        {
            uiTeamInfo = OpponentTeamInfo;
        }

        int add = (int)player.curHp - uiTeamInfo.HpGaugeUnit.Now;
        uiTeamInfo.HpGaugeUnit.Add(add);

        if (uiTeamInfo.SprHpDanger.gameObject.activeSelf && (player.curHp / player.maxHp) > GameInfo.Instance.BattleConfig.DangerHpRatio)
        {
            uiTeamInfo.HpGaugeUnit.kGaugeSpr.spriteName = "gauge_hp";
            uiTeamInfo.SprHpDanger.gameObject.SetActive(false);
        }
    }

    public void SubPlayerHp(Player player)
    {
        sUITeamInfo uiTeamInfo = null;
        if (!player.OpponentPlayer)
        {
            uiTeamInfo = PlayerTeamInfo;
        }
        else
        {
            uiTeamInfo = OpponentTeamInfo;
        }

        int sub = uiTeamInfo.HpGaugeUnit.Now - (int)player.curHp;
        
        uiTeamInfo.HpGaugeUnit.Sub(sub);
        uiTeamInfo.LbHp.textlocalize = string.Format("{0}/{1}", (int)player.curHp, (int)player.maxHp);

        if(!uiTeamInfo.SprHpDanger.gameObject.activeSelf && (player.curHp / player.maxHp) <= GameInfo.Instance.BattleConfig.DangerHpRatio)
        {
            uiTeamInfo.HpGaugeUnit.kGaugeSpr.spriteName = "gauge_hp_1";
            uiTeamInfo.SprHpDanger.gameObject.SetActive(true);
        }
    }

    public void AddPlayerSp(Player player)
    {
        sUITeamInfo uiTeamInfo = null;
        if (!player.OpponentPlayer)
        {
            uiTeamInfo = PlayerTeamInfo;
        }
        else
        {
            uiTeamInfo = OpponentTeamInfo;
        }

        float spPerBubble = GameInfo.Instance.BattleConfig.USUseSP / uiTeamInfo.SpBubbles.Length;

        int onCount = (int)(player.curSp / spPerBubble);
        if (onCount < uiTeamInfo.SpBubbles.Length)
        {
            uiTeamInfo.SpBubbles[onCount].Fill((player.curSp / spPerBubble) - onCount);
        }

        if (onCount >= 1)
        {
            float useWeaponSkillSp = player.boWeapon == null ? 0.0f : player.boWeapon.GetUseWeaponSkillSp();

            for (int i = 0; i < onCount; i++)
            {
                bool possibleSkill = useWeaponSkillSp > 0.0f && player.curSp >= useWeaponSkillSp;
                if (possibleSkill && !uiTeamInfo.SpBubbles[i].OnPossibleSkill)
                {
                    for (int j = 0; j < onCount - 1; j++)
                    {
                        uiTeamInfo.SpBubbles[j].psPossibleSkill.Play();
                    }
                }

                uiTeamInfo.SpBubbles[i].On(possibleSkill, player.curSp >= GameInfo.Instance.BattleConfig.USUseSP);
            }
        }

        uiTeamInfo.EffMaxSp.gameObject.SetActive(player.curSp >= GameInfo.Instance.BattleConfig.USUseSP);
    }

    public void SubPlayerSp(Player player, float beforeSp)
    {
        sUITeamInfo uiTeamInfo = null;
        if(!player.OpponentPlayer)
        {
            uiTeamInfo = PlayerTeamInfo;
        }
        else
        {
            uiTeamInfo = OpponentTeamInfo;
        }

        for (int i = 0; i < uiTeamInfo.SpBubbles.Length; i++)
        {
            uiTeamInfo.SpBubbles[i].Off();
        }

        if (player.curSp < GameInfo.Instance.BattleConfig.USUseSP)
        {
            float spPerBubble = GameInfo.Instance.BattleConfig.USUseSP / uiTeamInfo.SpBubbles.Length;
            float useWeaponSkillSp = player.boWeapon == null ? 0.0f : player.boWeapon.GetUseWeaponSkillSp();

            int onCount = (int)(player.curSp / spPerBubble);
            for (int i = 0; i < onCount; i++)
            {
                bool possibleSkill = useWeaponSkillSp > 0.0f && player.curSp >= useWeaponSkillSp;
                if (i > 0 && possibleSkill && !uiTeamInfo.SpBubbles[i].OnPossibleSkill)
                {
                    for (int j = 0; j < onCount - 1; j++)
                    {
                        uiTeamInfo.SpBubbles[j].psPossibleSkill.Play();
                    }
                }

                uiTeamInfo.SpBubbles[i].On(possibleSkill, player.curSp >= GameInfo.Instance.BattleConfig.USUseSP);
            }

            if (onCount < uiTeamInfo.SpBubbles.Length)
            {
                uiTeamInfo.SpBubbles[onCount].Fill((player.curSp / spPerBubble) - onCount);
            }
        }
    }

    /*
    public void StartSupporterCoolTime()
    {
        StopCoroutine("UpdateSupporterCoolTime");
        StartCoroutine("UpdateSupporterCoolTime");
    }
    */

    public void ShowSkillName(Player player, string name, eSkillNameType skillNameType)
    {
        if (player == null || string.IsNullOrEmpty(name) || !gameObject.activeSelf)
        {
            return;
        }

        sUITeamInfo uiTeamInfo = null;
        if (!player.OpponentPlayer)
        {
            uiTeamInfo = PlayerTeamInfo;
        }
        else
        {
            uiTeamInfo = OpponentTeamInfo;
        }

        switch(skillNameType)
        {
            case eSkillNameType.Character:
                uiTeamInfo.LbCharSkillName.textlocalize = name;

                uiTeamInfo.LbCharSkillName.gameObject.SetActive(true);
                uiTeamInfo.LbSkillName.gameObject.SetActive(false);

                uiTeamInfo.IconSupporterObj.SetActive(false);
                uiTeamInfo.IconWeaponObj.SetActive(false);
                
                uiTeamInfo.SprSkillName.color = CHAR_SKILL_NAME_COLOR;
                break;

            case eSkillNameType.Weapon:
                uiTeamInfo.LbSkillName.textlocalize = name;

                uiTeamInfo.LbCharSkillName.gameObject.SetActive(false);
                uiTeamInfo.LbSkillName.gameObject.SetActive(true);

                uiTeamInfo.IconSupporterObj.SetActive(false);
                uiTeamInfo.IconWeaponObj.SetActive(uiTeamInfo.TexWeapon.mainTexture != null);

                uiTeamInfo.SprSkillName.color = WPN_SKILL_NAME_COLOR;
                break;

            case eSkillNameType.Supporter:
                uiTeamInfo.LbSkillName.textlocalize = name;

                uiTeamInfo.LbCharSkillName.gameObject.SetActive(false);
                uiTeamInfo.LbSkillName.gameObject.SetActive(true);

                uiTeamInfo.IconSupporterObj.SetActive(uiTeamInfo.TexSupporter.mainTexture != null);
                uiTeamInfo.IconWeaponObj.SetActive(false);

                uiTeamInfo.SprSkillName.color = SUPPORTER_NAME_COLOR;
                break;
        }

        if (uiTeamInfo.CrSkillName != null)
        {
            Utility.StopCoroutine(this, ref uiTeamInfo.CrSkillName);
        }

        uiTeamInfo.CrSkillName = StartCoroutine(HideSkillName(uiTeamInfo));
    }

    public void UpdateTimer(float remainTime)
    {
        TimeSpan ts = TimeSpan.FromSeconds(remainTime);
        LbRemainTime.textlocalize = string.Format("{0}:{1}", ts.Minutes.ToString("D2"), ts.Seconds.ToString("D2"));
    }

    public void ShowBattleResult(bool win)
    {
        HideCheerBuff();

        if (win)
        {
            AniBattleWin.gameObject.SetActive(true);
        }
        else
        {
            AniBattleDefeat.gameObject.SetActive(true);
        }
    }

    public void HideBattleResult()
    {
        AniBattleWin.gameObject.SetActive(false);
        AniBattleDefeat.gameObject.SetActive(false);
    }

    public void InitCheer()
    {
        SprCheer.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
        LbCheer.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
        SprCheerGauge.fillAmount = 0.0f;
        LbCheerGauge.textlocalize = "0%";
        EffCheer.gameObject.SetActive(false);
        mCurCheerGauge = GameInfo.Instance.BattleConfig.CheerInitGauge;
    }

    public void AddCheerUp()
    {
        float addGauge = GameInfo.Instance.BattleConfig.CheerAddGauge + (GameInfo.Instance.BattleConfig.CheerAddGauge * AddCheerRate);
        mCurCheerGauge = Mathf.Clamp(mCurCheerGauge + addGauge, 0.0f, GameInfo.Instance.BattleConfig.CheerMaxGauge);
    }

    public void HideCheerBuff()
    {
        AniCheer.Stop();
        AniCheer.gameObject.SetActive(false);
    }

    public void UpdateSupporterCoolTime(float coolTime, float maxCoolTime)
    {
        SprCoolTime.gameObject.SetActive(coolTime > 0.0f);

        if (coolTime > 0.0f)
        {
            LbCoolTime.textlocalize = coolTime.ToString("F0");
            SprCoolTime.fillAmount = coolTime / maxCoolTime;
        }
        else
        {
            LbCoolTime.textlocalize = "";
            SprCoolTime.fillAmount = 0.0f;
        }
    }

    public void SupporterSpeak(float duration)
    {
        SprSupporterSpeak.gameObject.SetActive(true);
        Invoke("HideSupporterSpeak", duration);
    }

    public bool OnTabGameSpeed(int nSelect, SelectEvent type)
    {
        if(type == SelectEvent.Enable)
        {
            return false;
        }

        GameInfo.Instance.PVPGameSpeedType = nSelect;
        mWorldPVP.SetGameSpeed(GameInfo.Instance.BattleConfig.ArenaGameSpeed[nSelect]);

        return true;
    }

    public void OnBtnCheer()
    {
        if(mWorldPVP.IsEndGame || mWorldPVP.ProcessingEnd || mWorldPVP.IsAnyPlayerDead || SprCheerGauge.fillAmount < 1.0f)
        {
            return;
        }

        if(AniBattleWin.gameObject.activeSelf || AniBattleDefeat.gameObject.activeSelf)
        {
            return;
        }

        if (mWorldPVP.StartCheer(PlayerTeamInfo, LbCheerBuff))
        {
            AniCheer.gameObject.SetActive(true);
            AniCheer.Play();

            InitCheer();
        }
    }

    public void OnBtnSupporter()
    {
        if (mWorldPVP.IsEndGame || mWorldPVP.ProcessingEnd || mWorldPVP.IsAnyPlayerDead)
        {
            return;
        }

        if (AniBattleWin.gameObject.activeSelf || AniBattleDefeat.gameObject.activeSelf)
        {
            return;
        }

        Player player = mWorldPVP.GetCurrentPlayerTeamCharOrNull();
        if (player == null || !player.isGrounded || player.isPause || player.boSupporter == null || !player.boSupporter.HasActiveSkill())
        {
            return;
        }

        if (!player.StartSupporterSkill())
        {
            return;
        }

        string supporterName = FLocalizeString.Instance.GetText(player.boSupporter.data.TableData.Name);
        World.Instance.UIPVP.ShowSkillName(player, supporterName, UIArenaPlayPanel.eSkillNameType.Supporter);
    }

    public bool OnToggleAuto(int select, SelectEvent type)
    {
        if(select == 0)
        {
            GameInfo.Instance.PVPAutoSupporter = true;
            SprAutoSupporter.gameObject.SetActive(GameInfo.Instance.PVPAutoSupporter);
        }
        else
        {
            GameInfo.Instance.PVPAutoSupporter = false;
            SprAutoSupporter.gameObject.SetActive(GameInfo.Instance.PVPAutoSupporter);
        }

        return true;
    }

    public void OnBtnSurrender()
    {
        Player player = mWorldPVP.GetCurrentPlayerTeamCharOrNull();
        Player opponent = mWorldPVP.GetCurrentOpponentTeamCharOrNull();

        if(player.curSp >= GameInfo.Instance.BattleConfig.USMaxSP || opponent.curSp >= GameInfo.Instance.BattleConfig.USMaxSP)
        {
            return;
        }

        if (mWorldPVP.IsFriendPVP)
        {
            MessagePopup.OKCANCEL(eTEXTID.OK, FLocalizeString.Instance.GetText(3222), mWorldPVP.Surrender, null);
        }
        else
        {
            MessagePopup.OKCANCEL(eTEXTID.OK, FLocalizeString.Instance.GetText(3174), mWorldPVP.Surrender, null);
        }
    }

    /*
    private IEnumerator UpdateSupporterCoolTime()
    {
        yield return World.Instance.UIPlay.UpdateSupporterCoolTime();
    }
    */

    private void InitBattleUI()
    {
        for(int i = 0; i < PlayerTeamInfo.SpBubbles.Length; i++)
        {
            PlayerTeamInfo.SpBubbles[i].Init();
        }

        PlayerTeamInfo.InitAniSkillName();

        for (int i = 0; i < OpponentTeamInfo.SpBubbles.Length; i++)
        {
            OpponentTeamInfo.SpBubbles[i].Init();
        }

        OpponentTeamInfo.InitAniSkillName();

        if (!mWorldPVP.IsFriendPVP)
        {
            SprAtkBuff.gameObject.SetActive(GameInfo.Instance.ArenaATK_Buff_Flag);
            SprDefBuff.gameObject.SetActive(GameInfo.Instance.ArenaDEF_Buff_Flag);
        }
        else
        {
            SprAtkBuff.gameObject.SetActive(false);
            SprDefBuff.gameObject.SetActive(false);
        }
    }

    private void SetTeamInfo(int userLevel, string userName, sUITeamInfo uiTeamInfo, Player[] players, bool isOpponent)
    {
        uiTeamInfo.Init();

        uiTeamInfo.LbUserRank.textlocalize = userLevel.ToString();
        uiTeamInfo.LbUserName.textlocalize = userName;

        for (int i = 0; i < players.Length; i++)
        {
            Player player = players[i];
            uiTeamInfo.CharInfos[i].Init(player, isOpponent);
        }
    }

	private void SetCurrentChar( sUITeamInfo uiTeamInfo, Player[] players, int curPlayerIndex, bool isPlayer ) {
		uiTeamInfo.CharInfos[curPlayerIndex].Select();

		Player player = players[curPlayerIndex];
		if( ( player.curHp / player.maxHp ) <= GameInfo.Instance.BattleConfig.DangerHpRatio ) {
			uiTeamInfo.HpGaugeUnit.kGaugeSpr.spriteName = "gauge_hp_1";
			uiTeamInfo.SprHpDanger.gameObject.SetActive( true );
		}
		else {
			uiTeamInfo.HpGaugeUnit.kGaugeSpr.spriteName = "gauge_hp";
			uiTeamInfo.SprHpDanger.gameObject.SetActive( false );
		}

		int curHp = (int)player.curHp;
		int maxHp = (int)player.maxHp;

		uiTeamInfo.HpGaugeUnit.InitUIGaugeUnit( curHp, maxHp );
		uiTeamInfo.LbHp.textlocalize = string.Format( "{0}/{1}", curHp, maxHp );

		AddPlayerSp( player );
		uiTeamInfo.EffMaxSp.gameObject.SetActive( player.curSp >= GameInfo.Instance.BattleConfig.USUseSP );

		if( isPlayer ) {
			ShowSupporter( false );
		}

		if( player.boSupporter != null && player.boSupporter.data != null ) {
			uiTeamInfo.TexSupporter.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", string.Format( "Icon/Item/{0}.png", player.boSupporter.data.TableData.Icon ) );

			if( isPlayer && player.boSupporter.HasActiveSkill() ) {
				ShowSupporter( true );

				UpdateSupporterCoolTime( player.SupporterCoolingTime, player.boSupporter.data.TableData.CoolTime );
				TexSupporter.mainTexture = uiTeamInfo.TexSupporter.mainTexture;
			}
		}
		else {
			uiTeamInfo.TexSupporter.mainTexture = null;
		}

		if( player.boWeapon != null ) {
			uiTeamInfo.TexWeapon.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", string.Format( "Icon/Set/Set_{0}", player.boWeapon.data.TableData.Icon ) );
		}
		else {
			uiTeamInfo.TexWeapon.mainTexture = null;
		}
	}

	private void ShowSupporter(bool show)
    {
        SprSupporterSpeak.gameObject.SetActive(false);

        ToggleAutoSupporter.SetToggle(GameInfo.Instance.PVPAutoSupporter ? 0 : 1, SelectEvent.Enable);
        SprAutoSupporter.gameObject.SetActive(GameInfo.Instance.PVPAutoSupporter);
        SprCoolTime.gameObject.SetActive(!show);
        
        SupporterObj.SetActive(show);
    }

    private IEnumerator HideSkillName(sUITeamInfo uiTeamInfo)
    {
        uiTeamInfo.AniSkillName.Play(uiTeamInfo.ListAniName[0]);
        yield return new WaitForSeconds(1.0f);

        uiTeamInfo.AniSkillName.Play(uiTeamInfo.ListAniName[1]);
        yield return new WaitForSeconds(uiTeamInfo.AniSkillName[uiTeamInfo.ListAniName[1]].length);

        uiTeamInfo.InitAniSkillName();
    }

    private void HideSupporterSpeak()
    {
        SprSupporterSpeak.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        float cheerRegenGauge = GameInfo.Instance.BattleConfig.CheerRegenGauge + (GameInfo.Instance.BattleConfig.CheerRegenGauge * AddCheerRegenRate);
        float chargePerSec = GameInfo.Instance.BattleConfig.CheerMaxGauge / cheerRegenGauge;

        if (SprCheerGauge.fillAmount < 1.0f)
        {
            mCurCheerGauge = Mathf.Clamp(mCurCheerGauge + (chargePerSec * (Time.fixedDeltaTime * Time.timeScale)), 0.0f, GameInfo.Instance.BattleConfig.CheerMaxGauge);
            SprCheerGauge.fillAmount = mCurCheerGauge / GameInfo.Instance.BattleConfig.CheerMaxGauge;
            LbCheerGauge.textlocalize = string.Format("{0}%", (int)(SprCheerGauge.fillAmount * 100.0f));
        }
        else if (!EffCheer.gameObject.activeSelf)
        {
            SprCheer.color = Color.white;
            LbCheer.color = Color.white;
            SprCheerGauge.fillAmount = 1.0f;
            LbCheerGauge.textlocalize = "";
            EffCheer.gameObject.SetActive(true);
        }
    }
}
