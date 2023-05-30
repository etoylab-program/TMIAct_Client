
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CodeStage.AntiCheat.ObscuredTypes;


public partial class UIGamePlayPanel : FComponent
{
    [Header("[Property]")]
    public UIWidget[]   widgets;
    public GameObject   center;
    public UILabel      lbTime;

    [Header("[Joystick]")]
    public Transform    joystick;
    public UISprite     joystickBg;
    public UISprite     joystickThumb;

    [Header("[Buttons]")]
    public UIButtonPressing btnAtk;
    public ParticleSystem   EffAtk;
    public UIButtonPressing btnDash;
    public ParticleSystem   EffDashFlash;
    public UIButton         btnSAtk;
    public UIButton         btnUSkill;
    public UISprite         sprUSkill;
    public UISprite         SprUSkillCoolTime;
    public UILabel          LbUSkillCoolTime;
    public UIButton         BtnPause;

    [Header("[Player]")]
    public UIHPGaugeUnit        m_playerHp;
    public UIPlayerSpBubble[]   playerSpBubbles;
    public ParticleSystem       psMaxSp;
    public UILabel              LbGold;

    [Header("[Target]")]
    public UILabel          lbTargetName;
    public UIHPGaugeUnit    m_TargetHP;
    public UIHPGaugeUnit    m_TargetShield;
    public UIBuffDebuffIcon buffDebuffIcon;

    [Header("[Monster]")]
    public GameObject   MonsterInfo;
    public UILabel      LbDieMobCount;

    [Header("[NPC]")]
    public UIHPGaugeUnit    DefenceObjHp;
    public UITexture        TexDefenceObj;

    [Header("[Supporter]")]
    public UIButton         btnSupporter;
    public UITexture        texSupporter;
    public UISprite         sprSupporterCoolTime;
    public UILabel          lbSupporterCoolTime;
    public UISprite         sprSupporterFrame;
    public UISprite         sprSupporterEmpty;
    public ParticleSystem   psSupporter;
    public UISprite         sprSupporterTalking;

    [Header("[Weapon Skill]")]
    public UIButton btnWpnSkill;
    public UISprite sprWndSkill;
    public UISprite sprWndEmpty;
    public UILabel  LbWpnCountTxt;

    [Header("[Weapon Change]")]
    public UIButton     BtnWpnChange;
    public UITexture    TexWpnIcon;
    public UISprite     SprWpnChangeCoolTime;
    public UILabel      LbWpnChangeCoomTime;

    [Header("[Skill CoolTime]")]
    public UICoolTimeSprite[] SkillCoolTimeSprite;

    [Header("[ScreenEffect]")]
    public ScreenEffect m_screenEffect;

    [Header("[For raid]")]
    [SerializeField] private UISubCharacterUnit[]   _SubCharUnits;
    [SerializeField] private FList                  _RaidRandomOptionList;

    public Unit CurrentActiveTarget { get; private set; } = null;

    private Player				                    m_player						= null;
    private float				                    m_spPerBubble					= 0;
    private bool				                    m_pause							= false;
    private bool				                    m_lockActiveTargetHp			= false;
    private ParticleSystem		                    psUSkill;
    private Coroutine                               mCrSupporterCoolTime            = null;
    private float				                    m_supporterCoolTime				= 0.0f;
    private bool				                    mbActiveSupporterBtnEff			= false;
    private CardData			                    m_mainSlotCard;
    private int					                    m_wpnReferenceBattleOptionSetId	= 0;
    private UIWidget			                    mWidgetCombo					= null;
    private bool				                    mSpFullVoice					= false;
    private bool				                    mPlayTimeLimitVoice				= false;
    private Coroutine                               mCrWeaponChangeCoolTime         = null;
	private WaitForFixedUpdate	                    mWaitForFixedUpdate				= new WaitForFixedUpdate();
    private bool                                    mShowSubCharBtns                = false;
    private List<GameClientTable.StageBOSet.Param>  mRaidRandomOptionList           = new List<GameClientTable.StageBOSet.Param>();


	public override void Awake() {
		base.Awake();

		m_pause = false;
		InitNormalModeBtns();

        if ( m_screenEffect && m_screenEffect.kComboFixed ) {
            mWidgetCombo = m_screenEffect.kComboFixed.GetComponent<UIWidget>();
        }

		if ( psSupporter ) {
			ActiveSupporterBtnEff( false );
		}

        if ( _RaidRandomOptionList ) {
            _RaidRandomOptionList.EventUpdate = UpdateRandomOptionListSlot;
            _RaidRandomOptionList.EventGetItemCount = GetRandomOptionListCount;
        }

		SetWidgetsAlpha( 1.0f );
		ShowRaidUI( false );
	}

	public override void OnEnable() {
		base.OnEnable();

		m_screenEffect.HideAll();
		EffAtk.gameObject.SetActive( false );
		EffDashFlash.gameObject.SetActive( false );

		if( World.Instance.StageType == eSTAGETYPE.STAGE_TRAINING ) {
			btnSupporter.gameObject.SetActive( false );
		}
		else {
            ShowRaidUI( World.Instance.TestHelperType != World.eTestHelperType.NONE || 
                        World.Instance.StageType == eSTAGETYPE.STAGE_RAID || 
                        GameInfo.Instance.IsRaidPrologue );

            if( m_player ) {
                if( m_player && m_player.IsSupporterCoolingTime ) {
                    StartUpdateSupporterCoolTime();
                }

                if( m_player.IsWeaponChangeCoolingTime ) {
                    Utility.StopCoroutine( World.Instance, ref mCrWeaponChangeCoolTime );
                    mCrWeaponChangeCoolTime = World.Instance.StartCoroutine( UpdateWeaponChangeCoolTime() );
                }
            }

			buffDebuffIcon.RestartCoroutine();
		}

		InitKeyMapping();
	}

	public void ContinueSupporterCoolTime( float coolTime ) {
		if( m_player == null || coolTime <= 0.0f ) {
			return;
		}

        m_player.ContinueSupporterCoolTime( coolTime );
        StartUpdateSupporterCoolTime();
    }

    public void StartUpdateSupporterCoolTime() {
        Utility.StopCoroutine( World.Instance, ref mCrSupporterCoolTime );
        mCrSupporterCoolTime = World.Instance.StartCoroutine( UpdateSupporterCoolTime() );
    }

	public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();
		buffDebuffIcon.StopAllCoroutines();
    }

    public float ShowUI(int aniIndex)
    {
        SetWidgetsAlpha(0.0f);

        SetUIActive(true, false);
        float delay = PlayAnimtion(aniIndex) + 0.1f;

        if (World.Instance.StageType == eSTAGETYPE.STAGE_TRAINING)
        {
            btnSupporter.gameObject.SetActive(false);
        }
        else
        {
            if (mbActiveSupporterBtnEff)
            {
                psSupporter.gameObject.SetActive(false);

                CancelInvoke("ShowSupporterBtnEffect");
                Invoke("ShowSupporterBtnEffect", delay);
            }
        }

        ActiveUSkill(false);
        Invoke("CheckUSkillBtn", delay);

        for (int i = 0; i < SkillCoolTimeSprite.Length; i++)
            SkillCoolTimeSprite[i].Show(false);

        Invoke("ShowSelectedSkill", delay);
        return delay;
    }

    private void ShowSupporterBtnEffect()
    {
        ActiveSupporterBtnEff(true);
    }
    
    private void CheckUSkillBtn()
    {
        if (m_player.curSp >= GameInfo.Instance.BattleConfig.USUseSP)
            ActiveUSkill(true);
    }

    public override void SetUIActive(bool _bActive, bool _bAnimation = true)
    {
        base.SetUIActive(_bActive, _bAnimation);
        m_screenEffect.gameObject.SetActive(_bActive);
    }

    public void DisableAllParticles()
    {
        for(int i = 0; i < playerSpBubbles.Length; i++)
        {
            playerSpBubbles[i].Off();
        }

        psMaxSp.gameObject.SetActive(false);
        psSupporter.gameObject.SetActive(false);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
    }

	/*public float SetGameModeUI(eSTAGETYPE stageType, bool init, int arg)
    {
        if (mStageType == stageType)
        {
            return 0.0f;
        }

        if (!init)
        {
            mStageType = stageType;
        }

        return 0.0f;
    }*/

	public void InitNormalModeBtns() {
		m_playerHp.gameObject.SetActive( true );

		m_spPerBubble = GameInfo.Instance.BattleConfig.USUseSP / playerSpBubbles.Length;
        for( int i = 0; i < playerSpBubbles.Length; i++ ) {
            playerSpBubbles[i].Init();
        }

		m_screenEffect.gameObject.SetActive( true );

		ActiveTargetGauge( false );
		ShowWpnSkillBtn( null );

		psUSkill = btnUSkill.GetComponentInChildren<ParticleSystem>( true );
		ActiveUSkill( false );
		ShowUSkillCoolTime( m_player, false );

        if( psMaxSp ) {
            psMaxSp.gameObject.SetActive( false );
        }

		buffDebuffIcon.Init( null );
		m_lockActiveTargetHp = false;

		ShowSupporterBtn( false, false );
		ActiveSupporterBtnEff( false );
		InitSupporterCoolTime( false );

        for( int i = 0; i < SkillCoolTimeSprite.Length; i++ ) {
            SkillCoolTimeSprite[i].Show( false );
        }

		mPlayTimeLimitVoice = false;
	}

	private void InitKeyMapping()
    {
        if (!PlayerPrefs.HasKey("KeyMappingType"))
        {
            return;
        }

        int select = 0;
        if( PlayerPrefs.GetInt("KeyMappingType") == 0)
        {
            select = 0;

            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Attack, true);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Dash, true);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.WeaponSkill, true);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.SupporterSkill, true);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.USkill, true);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.ChangeWeapon, true);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Pause, true);
        }
        else
        {
            select = 1;

            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Attack, false);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Dash, false);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.WeaponSkill, false);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.SupporterSkill, false);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.USkill, false);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.ChangeWeapon, false);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Pause, false);
        }

        PlayerPrefs.SetInt("KeyMappingType", select);
    }

	public void InitSupporterCoolTime( bool stopCoroutine ) {
		if( stopCoroutine ) {
            Utility.StopCoroutine( World.Instance, ref mCrSupporterCoolTime );
        }

		sprSupporterCoolTime.fillAmount = 0.0f;
		lbSupporterCoolTime.textlocalize = "";
	}

	public void Update() {
		if( m_wpnReferenceBattleOptionSetId > 0 ) {
			UpdateWeaponReferenceBattleOption();
		}

        if( mShowSubCharBtns && _SubCharUnits != null && _SubCharUnits.Length >= 2 ) {
            if( Input.GetAxis( "GamePad L Trigger" ) != 0.0f ) {
                _SubCharUnits[0].OnBtnChangeChar();
			}
            else if( Input.GetAxis( "GamePad R Trigger" ) != 0.0f ) {
                _SubCharUnits[1].OnBtnChangeChar();
            }
        }
	}

	public void SetPlayer( Player player, CardData supporterData ) {
        if( player == null || player.IsHelper ) {
            return;
		}

		m_player = player;
		m_playerHp.InitUIGaugeUnit( (int)m_player.curHp, (int)m_player.maxHp );

		HideSupporterTalking();

		if( texSupporter && supporterData != null ) {
			m_mainSlotCard = supporterData;
			m_supporterCoolTime = supporterData.TableData.CoolTime;

			ShowSupporterBtn( true, m_player.boSupporter.HasActiveSkill() );
			texSupporter.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", string.Format( "Icon/Item/{0}.png", supporterData.TableData.Icon ) );
		}
		else {
			m_mainSlotCard = null;
			m_supporterCoolTime = 0.0f;

			ShowSupporterBtn( false, false );
		}

		ShowWpnSkillBtn( m_player );
        WeaponData waitWpnData = player.GetWaitWeaponDataOrNull();

        if( waitWpnData == null ) {
			BtnWpnChange.gameObject.SetActive( false );
		}
		else {
            BtnWpnChange.gameObject.SetActive( true );
            TexWpnIcon.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", string.Format( "Icon/Set/Set_{0}", waitWpnData.TableData.Icon ) );

            SprWpnChangeCoolTime.fillAmount = 0.0f;
			LbWpnChangeCoomTime.textlocalize = string.Empty;
		}

		SetwpnReferenceBattleOptionSetId();

		// 선택 스킬 정보
		string[] split = null;
		System.Type type = null;
		GameTable.CharacterSkillPassive.Param param = null;
		for( int i = 0; i < (int)eCOUNT.SKILLSLOT; i++ ) {
			if( i >= SkillCoolTimeSprite.Length ) {
				break;
			}

			if( GameSupport.IsInGameTutorial() && i >= (int)eCOUNT.SKILLSLOT - 1 ) {
				SkillCoolTimeSprite[i].Show( false );
				continue;
			}

			param = GameInfo.Instance.GameTable.FindCharacterSkillPassive( player.charData.EquipSkill[i] );
			if( param == null || string.IsNullOrEmpty( param.SkillAction ) ) {
				SkillCoolTimeSprite[i].Show( false );
				continue;
			}

			split = Utility.Split( param.SkillAction, ',' ); //param.SkillAction.Split(',');
			if( split.Length <= 0 ) {
				SkillCoolTimeSprite[i].Show( false );
				continue;
			}

			type = System.Type.GetType( "Action" + split[0] );
			if( type == null ) {
				SkillCoolTimeSprite[i].Show( false );
				continue;
			}

			ActionSelectSkillBase actionSelectedSkill = player.GetComponent( type ) as ActionSelectSkillBase;
			if( actionSelectedSkill == null ) {
				SkillCoolTimeSprite[i].Show( false );
				continue;
			}

			SkillCoolTimeSprite[i].Show( true );
			SkillCoolTimeSprite[i].Init( actionSelectedSkill, param.Atlus, param.Icon, param.UpIcon );
		}
	}

	public void SetTrainingroomUI()
    {
        string[] split = null;
        System.Type type = null;
        for (int i = 0; i < m_player.charData.PassvieList.Count; i++)
        {
            if (i >= SkillCoolTimeSprite.Length)
            {
                break;
            }

            if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Training && i >= (int)eCOUNT.SKILLSLOT - 1)
            {
                SkillCoolTimeSprite[i].Show(false);
                continue;
            }

			split = Utility.Split(m_player.charData.PassvieList[i].TableData.SkillAction, ','); //m_player.charData.PassvieList[i].TableData.SkillAction.Split(',');
			if (split.Length <= 0)
            {
                SkillCoolTimeSprite[i].Show(false);
                continue;
            }

            type = System.Type.GetType("Action" + split[0]);// split.Length - 1]);
            if (type == null)
            {
                SkillCoolTimeSprite[i].Show(false);
                continue;
            }

            ActionSelectSkillBase actionSelectedSkill = m_player.GetComponent(type) as ActionSelectSkillBase;
            if (actionSelectedSkill == null)
            {
                SkillCoolTimeSprite[i].gameObject.SetActive(false);
                continue;
            }

            SkillCoolTimeSprite[i-1].Show(true);
            SkillCoolTimeSprite[i-1].Init(actionSelectedSkill, m_player.charData.PassvieList[i].TableData.Atlus, m_player.charData.PassvieList[i].TableData.Icon, m_player.charData.PassvieList[i].TableData.UpIcon);
        }
    }

    private string mStrTime = "";
    public void UpdateTime(TimeSpan ts, bool includeMilliseconds, bool timeLimit)
    {
        if(includeMilliseconds)
        {
            mStrTime = string.Format("{0}:{1}.{2}", ts.Minutes.ToString("D2"), ts.Seconds.ToString("D2"), (ts.Milliseconds / 10).ToString("D2"));
        }
        else
        {
            mStrTime = string.Format("{0}:{1}", ts.Minutes.ToString("D2"), ts.Seconds.ToString("D2"));
        }

        if(timeLimit)
        {
            if(ts.Minutes <= 0 && ts.Seconds <= 45)
            {
                lbTime.color = Color.red;

                if(!mPlayTimeLimitVoice)
                {
                    if (m_mainSlotCard != null)
                    {
                        VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.TimeLimit, m_mainSlotCard.TableID);
                    }

                    mPlayTimeLimitVoice = true;
                }
            }
            else
            {
                lbTime.color = new Color(0.98f, 0.76f, 0.07f);
            }
        }
        else
        {
            lbTime.color = Color.white;
        }

        lbTime.textlocalize = mStrTime;
    }

    private void ShowSupporterBtn(bool show, bool hasUseSkill)
    {
        texSupporter.gameObject.SetActive(show);
        sprSupporterCoolTime.gameObject.SetActive(show);
        lbSupporterCoolTime.gameObject.SetActive(show);
        sprSupporterFrame.gameObject.SetActive(show);

        if (show == true)
            sprSupporterEmpty.gameObject.SetActive(false);

        if (show && hasUseSkill)
        {
            ActiveSupporterBtnEff(show);
        }
        else
        {
            ActiveSupporterBtnEff(false);
        }
    }

    public void ShowSupporterTalking(float duration)
    {
        sprSupporterTalking.color = Color.white;
        sprSupporterTalking.gameObject.SetActive(true);

        StopCoroutine("UpdateSupporterTalking");
        StartCoroutine("UpdateSupporterTalking", duration);
    }

    public void HideSupporterTalking()
    {
        StopCoroutine("UpdateSupporterTalking");
        sprSupporterTalking.gameObject.SetActive(false);
    }

	public void ShowUSkillCoolTime( Player player, bool show ) {
		if( player && player.IsHelper ) {
			return;
		}

		SprUSkillCoolTime.gameObject.SetActive( show );
		LbUSkillCoolTime.gameObject.SetActive( show );

		btnDash.gameObject.SetActive( !show );
		btnWpnSkill.gameObject.SetActive( !show );
	}

	public void UpdateUSkillCoolTime( Player player, float curCoolTime, float maxCoolTime ) {
		if( player == null || player.IsHelper ) {
			return;
		}

        if( !SprUSkillCoolTime.gameObject.activeSelf ) {
            ShowUSkillCoolTime( player, this );
		}

		SprUSkillCoolTime.fillAmount = 1.0f - ( curCoolTime / maxCoolTime );
		LbUSkillCoolTime.textlocalize = ( maxCoolTime - curCoolTime ).ToString( "F0" );
	}

	private IEnumerator UpdateSupporterTalking(float duration)
    {
        Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        bool add = false;

        float checkTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (checkTime < duration)
        {
            checkTime += Time.fixedDeltaTime;

            if (!add)
            {
                color.a -= Time.fixedDeltaTime / (duration * 0.3f);
                if (color.a <= 0.0f)
                {
                    color.a = 0.0f;
                    add = true;
                }

                sprSupporterTalking.color = color;
            }
            else
            {
                color.a += Time.fixedDeltaTime / (duration * 0.3f);
                if (color.a >= 1.0f)
                {
                    color.a = 1.0f;
                    add = false;
                }

                sprSupporterTalking.color = color;
            }

            yield return mWaitForFixedUpdate;
        }

        sprSupporterTalking.gameObject.SetActive(false);
    }

	public void ShowWpnSkillBtn( Player player ) {
        if( player && player.IsHelper ) {
            return;
		}

		bool hasWeaponSkill = player == null ? false : player.HasWeaponActiveSkill();
		bool active = player == null ? false : player.IsActiveWeaponSkill();

		sprWndSkill.gameObject.SetActive( hasWeaponSkill );
		sprWndEmpty.gameObject.SetActive( !hasWeaponSkill );

		if( hasWeaponSkill ) {
            if( active ) {
                sprWndSkill.color = new Color( sprWndSkill.color.r, sprWndSkill.color.g, sprWndSkill.color.b, 0.7f );
            }
            else {
                sprWndSkill.color = new Color( sprWndSkill.color.r, sprWndSkill.color.g, sprWndSkill.color.b, 0.3f );
            }
		}
	}

	public void ShowSelectedSkill()
    {
        if (World.Instance.StageType == eSTAGETYPE.STAGE_TRAINING)
        {
            SetTrainingroomUI();
        }
        else
        {
            for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
            {
                if (i >= SkillCoolTimeSprite.Length)
                    break;

                if (m_player.charData.EquipSkill[i] == (int)eCOUNT.NONE || (GameSupport.IsInGameTutorial() && i >= (int)eCOUNT.SKILLSLOT - 1))
                {
                    SkillCoolTimeSprite[i].Show(false);
                    continue;
                }
                
                SkillCoolTimeSprite[i].Show(true);
            }
        }
    }

	public void AddPlayerHp( Player player ) {
        if( player == null || player.IsHelper ) {
            return;
		}

        int add = (int)player.curHp - m_playerHp.Now;
		m_playerHp.Add( add );

		float f = player.curHp / player.maxHp;
        if( f > GameInfo.Instance.BattleConfig.DangerHpRatio ) {
            m_screenEffect.dangerHp.SetActive( false );
        }
	}

	public void SubPlayerHp( Player player )
	{
        if( player == null || player.IsHelper ) {
            return;
        }

        int sub = m_playerHp.Now - (int)player.curHp;
		m_playerHp.Sub( sub );

		float f = player.curHp / player.maxHp;
        if( f <= GameInfo.Instance.BattleConfig.DangerHpRatio ) {
            if( !m_screenEffect.dangerHp.activeSelf ) {
                if( m_mainSlotCard != null && !GameSupport.IsShowGameStroyUI() )
                    VoiceMgr.Instance.PlaySupporter( eVOICESUPPORTER.Crisis, m_mainSlotCard.TableID );

                m_screenEffect.dangerHp.SetActive( true );
            }

            m_screenEffect.ShowDamage();
        }
        else if( m_screenEffect.dangerHp.activeSelf ) {
            m_screenEffect.dangerHp.SetActive( false );
        }
	}

	public void InitPlayerSp( Player player, float useWeaponSkillSp ) {
        if( player == null ) {
            return;
		}

        if( player.IsHelper ) {
            UISubCharacterUnit subCharUnit = GetSubCharUnitByPlayerOrNull( player );
            if( subCharUnit ) {
                subCharUnit.InitPlayerSp();
            }
        }
        else {
            for( int i = 0; i < playerSpBubbles.Length; i++ ) {
                playerSpBubbles[i].Init();
            }

            m_spPerBubble = GameInfo.Instance.BattleConfig.USUseSP / playerSpBubbles.Length;
            AddPlayerSp( player, useWeaponSkillSp );
        }
	}

	public void AddPlayerSp( Player player, float useWeaponSkillSp ) {
        if( player == null ) {
            return;
        }

        if( player.IsHelper ) {
            UISubCharacterUnit subCharUnit = GetSubCharUnitByPlayerOrNull( player );
            if( subCharUnit ) {
                subCharUnit.AddPlayerSp();
            }
        }
        else {
            int onCount = (int)(player.curSp / m_spPerBubble);
            if( onCount < playerSpBubbles.Length ) {
                playerSpBubbles[onCount].Fill( ( player.curSp / m_spPerBubble ) - onCount );
            }

            if( onCount >= 1 ) {
                for( int i = 0; i < onCount; i++ ) {
                    bool possibleSkill = useWeaponSkillSp > 0.0f && player.curSp >= useWeaponSkillSp;
                    if( possibleSkill && !playerSpBubbles[i].OnPossibleSkill ) {
                        for( int j = 0; j < onCount - 1; j++ ) {
                            playerSpBubbles[j].psPossibleSkill.Stop( true, ParticleSystemStopBehavior.StopEmittingAndClear );
                            playerSpBubbles[j].psPossibleSkill.Play();
                        }
                    }

                    playerSpBubbles[i].On( possibleSkill, player.curSp >= GameInfo.Instance.BattleConfig.USUseSP );
                }
            }

            ActiveUSkill( player.curSp >= GameInfo.Instance.BattleConfig.USUseSP );
        }
	}

	public void SubPlayerSp( Player player, float beforeSp, float useWeaponSkillSp ) {
        if( player == null ) {
            return;
        }

        if( player.IsHelper ) {
            UISubCharacterUnit subCharUnit = GetSubCharUnitByPlayerOrNull( player );
            if( subCharUnit ) {
                subCharUnit.SubPlayerSp();
            }
        }
        else {
            for( int i = 0; i < playerSpBubbles.Length; i++ ) {
                playerSpBubbles[i].Off();
            }

            if( player.curSp < GameInfo.Instance.BattleConfig.USUseSP ) {
                ActiveUSkill( false );

                int onCount = (int)(player.curSp / m_spPerBubble);

                for( int i = 0; i < onCount; i++ ) {
                    bool possibleSkill = useWeaponSkillSp > 0.0f && player.curSp >= useWeaponSkillSp;

                    if( i > 0 && possibleSkill && !playerSpBubbles[i].OnPossibleSkill ) {
                        for( int j = 0; j < onCount - 1; j++ ) {
                            playerSpBubbles[j].psPossibleSkill.Stop( true, ParticleSystemStopBehavior.StopEmittingAndClear );
                            playerSpBubbles[j].psPossibleSkill.Play();
                        }
                    }

                    playerSpBubbles[i].On( possibleSkill, player.curSp >= GameInfo.Instance.BattleConfig.USUseSP );
                }

                if( onCount < playerSpBubbles.Length ) {
                    playerSpBubbles[onCount].Fill( ( player.curSp / m_spPerBubble ) - onCount );
                }

                mSpFullVoice = false;
            }
        }
	}

	public void DecreasePlayerSp( Player player, float useWeaponSkillSp ) {
        if( player == null ) {
            return;
        }

        if( player.IsHelper ) {
            UISubCharacterUnit subCharUnit = GetSubCharUnitByPlayerOrNull( player );
            if( subCharUnit ) {
                subCharUnit.DecreasePlayerSp();
            }
        }
        else {
            if( player.curSp < GameInfo.Instance.BattleConfig.USUseSP ) {
                ActiveUSkill( false );

                bool possibleSkill = useWeaponSkillSp > 0.0f && player.curSp >= useWeaponSkillSp;
                if( !possibleSkill ) {
                    for( int i = 0; i < playerSpBubbles.Length; i++ ) {
                        playerSpBubbles[i].psPossibleSkill.Stop();
                    }
                }

                int onCount = (int)(player.curSp / m_spPerBubble);
                for( int i = onCount + 1; i < playerSpBubbles.Length; i++ ) {
                    playerSpBubbles[i].Off();
                }

                if( onCount < playerSpBubbles.Length ) {
                    playerSpBubbles[onCount].Off();
                    playerSpBubbles[onCount].Fill( ( player.curSp / m_spPerBubble ) - onCount, false );
                }

                mSpFullVoice = false;
            }
        }
	}

	private void ActiveUSkill(bool active)
    {
        if(active && UIAni.isPlaying)
        {
            return;
        }

        if (active)
        {
            sprUSkill.color = new Color(sprUSkill.color.r, sprUSkill.color.g, sprUSkill.color.b, 1.0f);
            if (m_mainSlotCard != null && !GameSupport.IsShowGameStroyUI() && !mSpFullVoice)
            {
                mSpFullVoice = true;
                VoiceMgr.Instance.PlaySupporter(eVOICESUPPORTER.GaugeCharge, m_mainSlotCard.TableID);
            }
        }
        else
            sprUSkill.color = new Color(sprUSkill.color.r, sprUSkill.color.g, sprUSkill.color.b, 0.3f);

        if (psUSkill)
            psUSkill.gameObject.SetActive(active);

        if (psMaxSp)
            psMaxSp.gameObject.SetActive(active);
    }

    /*public void AddPlayerMarble(float curMarble, float maxMarble)
    {
        int add = (int)curMarble - m_playerMarble.Now;
        m_playerMarble.Add(add);

        if (curMarble >= maxMarble && psMaxMarble)
            ShowMarbleUIs(true);
    }

    public void SubPlayerMarble(float curMarble, float maxMarble)
    {
        int sub = m_playerMarble.Now - (int)curMarble;
        m_playerMarble.Sub(sub);

        if (m_playerMarble.Now <= 0)
        {
            ShowQTEBtn(false);
            ShowMarbleUIs(false);
        }
    }

    private void ShowMarbleUIs(bool show)
    {
        if (psMaxMarble)
            psMaxMarble.gameObject.SetActive(show);

        sprIconMarble.gameObject.SetActive(show);
    }*/

    public void ActiveTargetGauge(bool active, Unit target = null)
    {
        if (m_lockActiveTargetHp)
            return;

        if(target)
        {
            CurrentActiveTarget = target;
        }

        if (m_TargetHP.Now <= 0)
            active = false;

        m_TargetHP.gameObject.SetActive(active);

        if (lbTargetName)
            lbTargetName.gameObject.SetActive(active);

        if(m_TargetShield.Max <= 0)
            m_TargetShield.gameObject.SetActive(false);
        else
            m_TargetShield.gameObject.SetActive(active);

        if (!active)
        {
            buffDebuffIcon.End(false);
        }
    }

    public void UpdateTargetHp(Unit target, string name, float curHp, float hp, int hpGaugeInitValue = 0)
    {
        if (hpGaugeInitValue != 0)
            m_TargetHP._accGaugeMax = hpGaugeInitValue;

        if (curHp <= 0)
        {
            m_TargetHP.Set((int)curHp, (int)hp);
            ActiveTargetGauge(false);

            CurrentActiveTarget = null;
            return;
        }

        if(CurrentActiveTarget != target)
        {
            buffDebuffIcon.RestartCoroutine();
        }

        CurrentActiveTarget = target;

        if (!m_TargetHP.gameObject.activeSelf)
        {
            if(!m_lockActiveTargetHp)
                m_TargetHP.gameObject.SetActive(true);

            m_TargetHP.Set((int)curHp, (int)hp);

            if (lbTargetName && !m_lockActiveTargetHp)
                lbTargetName.gameObject.SetActive(true);
        }
        else if(m_TargetHP.Max != hp)
        {
            m_TargetHP.Set((int)curHp, (int)hp);

            if (lbTargetName && !m_lockActiveTargetHp)
                lbTargetName.gameObject.SetActive(true);
        }

        int sub = m_TargetHP.Now - (int)curHp;
        m_TargetHP.Sub(sub);

        if (lbTargetName)
            lbTargetName.textlocalize = name;
    }

    public void UpdateTargetShield(float curShield, float maxShield)
    {
        if (maxShield <= 0.0f)
        {
            m_TargetShield.gameObject.SetActive(false);
            return;
        }

        if (!m_TargetShield.gameObject.activeSelf)
        {
            if (!m_lockActiveTargetHp)
                m_TargetShield.gameObject.SetActive(true);

            m_TargetShield.Set((int)curShield, (int)maxShield);
        }
        else if(m_TargetShield.Max != maxShield)
        {
            m_TargetShield.Set((int)curShield, (int)maxShield);
        }

        int sub = m_TargetShield.Now - (int)curShield;
        m_TargetShield.Sub(sub);
    }

    public void ActiveNPCGauge(bool active, OtherObject npc)
    {
        if(active)
        {
            DefenceObjHp.Set((int)npc.curHp, (int)npc.maxHp);
        }

        DefenceObjHp.gameObject.SetActive(active);

        TexDefenceObj.mainTexture = npc.Portrait;
        TexDefenceObj.gameObject.SetActive(active);
    }

    public void UpdateNPCGauge(float curHp, float hp, int hpGaugeInitValue = 0)
    {
        if (hpGaugeInitValue != 0)
        {
            DefenceObjHp._accGaugeMax = hpGaugeInitValue;
        }

        int sub = DefenceObjHp.Now - (int)curHp;
        DefenceObjHp.Sub(sub);
    }

    public void ActiveMonsetCount(bool active, int maxCount)
    {
        MonsterInfo.gameObject.SetActive(active);
        LbDieMobCount.textlocalize = string.Format("{0}/{1}", 0, maxCount);
    }

    public void UpdateMonsterCount(int count, int maxCount)
    {
        LbDieMobCount.textlocalize = string.Format("{0}/{1}", count, maxCount);
    }

	public void UpdateCombo( Player player, bool ignoreTime ) {

		if ( !gameObject.activeSelf ) {
			StopCoroutine( "FadeHideCombo" );
			m_screenEffect.HideCombo();

			return;
		}

		if ( player.comboCount <= 0 ) {
			StopCoroutine( "FadeHideCombo" );
			StartCoroutine( "FadeHideCombo", 0.7f );
		}
		else {
			StopCoroutine( "FadeHideCombo" );

			if ( mWidgetCombo ) {
				mWidgetCombo.alpha = 1.0f;
			}

			m_screenEffect.ShowCombo( player.comboCount, ignoreTime );

			if ( m_mainSlotCard != null && !GameSupport.IsShowGameStroyUI() ) {
				for ( int i = 0; i < GameInfo.Instance.BattleConfig.PlaySupporterVoiceByComboCnt.Count; i++ ) {
                    if ( player.comboCount == GameInfo.Instance.BattleConfig.PlaySupporterVoiceByComboCnt[i] ) {
                        VoiceMgr.Instance.PlaySupporter( eVOICESUPPORTER.Combo, m_mainSlotCard.TableID, i + 1 );
                    }
				}
			}
		}
	}

	private IEnumerator FadeHideCombo( float delay ) {
		float alpha = 1.0f;

		while ( alpha > 0.0f ) {
			alpha -= Time.fixedDeltaTime / delay;

            if ( mWidgetCombo ) {
                mWidgetCombo.alpha = alpha;
            }

			yield return mWaitForFixedUpdate;
		}

        if ( m_screenEffect ) {
            m_screenEffect.HideCombo();
        }
	}

	public void OnBtnPause() {
		if( World.Instance.IsEndGame || ( World.Instance.Player && World.Instance.Player.curHp <= 0.0f ) ) {
			return;
		}

		if( AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam && Director.CurrentPlaying ) {
			Director.CurrentPlaying.Pause();

			UICinematicPopup cinematicPopup = GameUIManager.Instance.GetUI<UICinematicPopup>("CinematicPopup");
			if( cinematicPopup ) {
				cinematicPopup.ShowPausePopup();
			}

			return;
		}

		if( World.Instance.IsPause || m_pause || widgets[0].alpha < 1.0f ) {
			return;
		}

		UIGameStoryPausePopup popup = GameUIManager.Instance.GetUI("GameStoryPausePopup") as UIGameStoryPausePopup;
		if( popup == null || popup.gameObject.activeSelf ) {
			return;
		}

		GameUIManager.Instance.ShowUI( "GameStoryPausePopup", false );
	}

	public void OnBtnSupporter() {
		if( m_player == null || m_player.Input == null || 
            m_screenEffect == null || m_screenEffect.kBossWarning == null || m_screenEffect.kBossWarning.gameObject.activeSelf || 
            m_mainSlotCard == null || m_player.IsSupporterCoolingTime || widgets[0].alpha < 1.0f ) {
			return;
		}

        InputController inputCtrl = m_player.Input as InputController;
        if( inputCtrl == null || ( ( inputCtrl.LockBtn & InputController.ELockBtnFlag.SUPPORTER ) != 0 ) ) {
            return;
        }

        StartSupporterSkill();
	}

	private void StartSupporterSkill() {
        if( !m_player.StartSupporterSkill() ) {
            return;
		}

        if( m_mainSlotCard.TableData.Grade == (int)eGRADE.GRADE_UR ) {
			SoundManager.Instance.PlayUISnd( 62 );
		}
		else if( m_mainSlotCard.TableData.Grade == (int)eGRADE.GRADE_SR ) {
			SoundManager.Instance.PlayUISnd( 63 );
		}
		else {
			SoundManager.Instance.PlayUISnd( 64 );
		}

		if( !GameSupport.IsShowGameStroyUI() ) {
			VoiceMgr.Instance.PlaySupporter( eVOICESUPPORTER.Skill, m_mainSlotCard.TableID );
		}

        Texture tex = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "card", string.Format( "Card/{0}_{1}.png", 
                                                                                                m_mainSlotCard.TableData.Icon, 
                                                                                                GameSupport.GetCardImageNum( m_mainSlotCard ) ) );

        float duration = m_screenEffect.ShowSupporterSkill( true, m_mainSlotCard.TableData.Grade, tex );

		InputController inputCtrl = m_player.Input as InputController;
		inputCtrl.LockBtnFlag( InputController.ELockBtnFlag.USKILL );

		sprSupporterCoolTime.fillAmount = 1.0f;

        Invoke( "UnlockUSkillBtn", duration );
        Invoke( "DelayedUseSupporterActiveSkill", 0.2f );

        StartUpdateSupporterCoolTime();
    }

	private void UnlockUSkillBtn() {
		InputController inputCtrl = m_player.Input as InputController;
		inputCtrl.LockBtnFlag( InputController.ELockBtnFlag.NONE );
	}

	public IEnumerator UpdateSupporterCoolTime() {
		while( m_player.IsSupporterCoolingTime ) {
            sprSupporterCoolTime.fillAmount = m_player.GetSupporterCoolTimeFillAmount();
			lbSupporterCoolTime.textlocalize = m_player.SupporterCoolingTime.ToString( "F0" );

			if( mbActiveSupporterBtnEff ) {
				ActiveSupporterBtnEff( false );
			}

			yield return mWaitForFixedUpdate;
		}

        if( m_mainSlotCard != null && !GameSupport.IsShowGameStroyUI() && !m_player.OnlyExtraCoolingTime ) {
            VoiceMgr.Instance.PlaySupporter( eVOICESUPPORTER.SkillGauge, m_mainSlotCard.TableID );
        }

		InitSupporterCoolTime( false );
		ActiveSupporterBtnEff( true );
	}

	public void OnBtnWpnSkill()
    {
		if(m_player == null || m_player.Input == null || widgets[0].alpha < 1.0f )
		{
			return;
		}

        InputController inputCtrl = m_player.Input as InputController;
        if (inputCtrl == null || ((inputCtrl.LockBtn & InputController.ELockBtnFlag.WEAPON) != 0))
        {
            return;
        }

        m_player.StartWpnSkill();
    }

	public void OnBtnChangeWeapon() {
		if( !BtnWpnChange.gameObject.activeSelf || SprWpnChangeCoolTime.fillAmount > 0.0f ) {
			return;
		}

		if( World.Instance.IsEndGame || World.Instance.IsPause || m_player.Input.isPause || m_player.isPause || widgets[0].alpha < 1.0f ||
			m_player.UsingUltimateSkill ) {
			return;
		}

        // 무기 교체는 대기 및 이동 시에만 가능
        if( m_player.actionSystem && m_player.actionSystem.currentAction && 
            m_player.actionSystem.currentAction.actionCommand != eActionCommand.Idle && 
            m_player.actionSystem.currentAction.actionCommand != eActionCommand.MoveByDirection ) {
            return;
        }
         
        if( m_player.actionSystem.currentAction ) {
			if( m_player.actionSystem.currentAction.actionCommand != eActionCommand.Idle &&
			   m_player.actionSystem.currentAction.actionCommand != eActionCommand.MoveByDirection ) {
				return;
			}
		}

		m_player.ChangeWeapon( false );
	}

	public void SetWidgetsAlpha( float alpha, bool includeSkillEff = false ) {
		if( alpha <= 0.0f ) {
			alpha = 0.01f; // 알파가 0이면 애니메이션에서 알파 조절 못함

			if( includeSkillEff ) {
				ActiveUSkill( false );
				ActiveSupporterBtnEff( false );

				for( int i = 0; i < playerSpBubbles.Length; i++ ) {
					playerSpBubbles[i].gameObject.SetActive( false );
				}
			}
		}
		else if( alpha >= 1.0f && includeSkillEff ) {
			if( m_player.curSp >= GameInfo.Instance.BattleConfig.USUseSP ) {
				ActiveUSkill( true );
			}

			if( m_player.IsSupporterCoolingTime && m_player.boSupporter != null && m_player.boSupporter.HasActiveSkill() ) {
				ActiveSupporterBtnEff( true );
			}

			for( int i = 0; i < playerSpBubbles.Length; i++ ) {
				playerSpBubbles[i].gameObject.SetActive( true );
			}
		}

		for( int i = 0; i < widgets.Length; i++ ) {
			widgets[i].alpha = alpha;
		}
	}

	public bool IsSelectBtns()
    {
        if(UICamera.selectedObject == null)
        {
            return false;
        }

        if(UICamera.selectedObject == btnAtk.gameObject || UICamera.selectedObject == btnDash.gameObject || UICamera.selectedObject == btnUSkill.gameObject ||
           UICamera.selectedObject == btnWpnSkill.gameObject || UICamera.selectedObject == btnSupporter.gameObject)
        {
            return true;
        }

        return false;
    }

	public void ChangeWeapon( Player player, WeaponData data ) {
		if( player == null || player.IsHelper || !BtnWpnChange.gameObject.activeSelf ) {
			return;
		}

		TexWpnIcon.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", string.Format( "Icon/Set/Set_{0}", data.TableData.Icon ) );

		if( !GameInfo.Instance.GameConfig.TestMode ) {
			SprWpnChangeCoolTime.fillAmount = player.WeaponChangeCoolingTime;
			LbWpnChangeCoomTime.textlocalize = GameInfo.Instance.BattleConfig.WeaponChangeTime.ToString();

            Utility.StopCoroutine( World.Instance, ref mCrWeaponChangeCoolTime );
            mCrWeaponChangeCoolTime = World.Instance.StartCoroutine( UpdateWeaponChangeCoolTime() );
        }
		else {
			SprWpnChangeCoolTime.fillAmount = 0.0f;
			LbWpnChangeCoomTime.textlocalize = string.Empty;
		}

		SetwpnReferenceBattleOptionSetId();
		Debug.Log( "UIGamePlayPanel::ChangeWeapon" );
	}

	private IEnumerator UpdateWeaponChangeCoolTime()
    {
        while( m_player.IsWeaponChangeCoolingTime )
        {
            SprWpnChangeCoolTime.fillAmount = m_player.WeaponChangeCoolingTime / GameInfo.Instance.BattleConfig.WeaponChangeTime;
            LbWpnChangeCoomTime.textlocalize = m_player.WeaponChangeCoolingTime.ToString( "F0" );

            yield return mWaitForFixedUpdate;
        }

        SprWpnChangeCoolTime.fillAmount = 0.0f;
        LbWpnChangeCoomTime.textlocalize = string.Empty;
    }

	private void SetwpnReferenceBattleOptionSetId()
    {
        LbWpnCountTxt.textlocalize = string.Empty;
        m_wpnReferenceBattleOptionSetId = 0;
        for (int i = 0; i < m_player.boWeapon.ListBattleOptionData.Count; i++)
        {
            if (m_player.boWeapon.ListBattleOptionData[i].referenceBattleOptionSetId > 0)
            {
                m_wpnReferenceBattleOptionSetId = m_player.boWeapon.ListBattleOptionData[i].referenceBattleOptionSetId;
                break;
            }
        }
    }

    private void UpdateWeaponReferenceBattleOption()
    {
        UnitBuffStats.sInfo buffinfo = m_player.unitBuffStats.GetBuffStat(m_wpnReferenceBattleOptionSetId);
        if (buffinfo != null)
        {
            LbWpnCountTxt.textlocalize = buffinfo.stackCount.ToString();
        }
        else
        {
            LbWpnCountTxt.textlocalize = string.Empty;
        }
    }

    public void UpdateWeaponReferenceBattleOption(int stackCount)
    {
        if (stackCount <= 0)
        {
            LbWpnCountTxt.textlocalize = string.Empty;
        }
        else
        {
            LbWpnCountTxt.textlocalize = stackCount.ToString();
        }
    }

    private void ActiveSupporterBtnEff(bool active)
    {
        mbActiveSupporterBtnEff = active;
        psSupporter.gameObject.SetActive(active);
    }

    public void ShowRaidUI( bool show ) {
        mShowSubCharBtns = show;

        for( int i = 0; i < _SubCharUnits.Length; i++ ) {
            if( show && _SubCharUnits[i].IsCurPlayerDead() ) {
                continue;
			}

            _SubCharUnits[i].AddPlayerSp();
            _SubCharUnits[i].SetActive( show );
        }

        _RaidRandomOptionList.gameObject.SetActive( show );
        mRaidRandomOptionList.Clear();

        if( show ) {
            WorldStage worldStage = World.Instance as WorldStage;
            if( worldStage ) {
                mRaidRandomOptionList.AddRange( worldStage.RaidStageBOSetList );
                _RaidRandomOptionList.UpdateList();
            }
        }
    }

    public void SetSubCharUnit( int index, Player player ) {
        if( index < 0 || index >= _SubCharUnits.Length ) {
            return;
		}

        _SubCharUnits[index].Set( index, player );
	}

    public void DisableDiedSubCharUnit() {
        for( int i = _SubCharUnits.Length - 1; i >= 0; i-- ) {
            if( _SubCharUnits[i].IsCurPlayerDead() ) {
                _SubCharUnits[i].Disable();
            }
		}
    }

    public void StartCoolTimeAllSubCharBtn() {
        for( int i = 0; i < _SubCharUnits.Length; i++ ) {
            if( _SubCharUnits[i].IsCurPlayerDead() ) {
                continue;
			}

            _SubCharUnits[i].StartCoolTime();
		}
	}

    public UISubCharacterUnit GetSubCharUnitByIndexOrNull( int index ) {
        if( index < 0 || index >= _SubCharUnits.Length ) {
            return null;
		}

        _SubCharUnits[index].GetButtonComponent();
        return _SubCharUnits[index];
	}

    public UISubCharacterUnit GetSubCharUnitByPlayerOrNull( Player player ) {
        for( int i = 0; i < _SubCharUnits.Length; i++ ) {
            if( _SubCharUnits[i].CurPlayer == player ) {
                _SubCharUnits[i].GetButtonComponent();
                return _SubCharUnits[i];
            }
		}

        return null;
    }

    private void UpdateRandomOptionListSlot( int index, GameObject slotObject ) {
        UIRaidRandomOptionSlot slot = slotObject.GetComponent<UIRaidRandomOptionSlot>();
        slot.ParentGO = gameObject;

        slot.UpdateSlot( index, mRaidRandomOptionList[index] );
    }

    private int GetRandomOptionListCount() {
        return mRaidRandomOptionList.Count;
    }
}
