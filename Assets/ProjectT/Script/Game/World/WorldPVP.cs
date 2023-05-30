
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;


public partial class WorldPVP : World
{
    public enum ePromoteState
    {
        None = 0,
        Testing,        // 승급전 진행 중
        Promotion,      // 승급 성공
        Failure         // 승급 실패
    }


    [System.Serializable]
    public class sTestPVPCharData
    {
        public int      TableId         = 1;
        public int      WeaponId        = 1;
        public int[]    SkillIds;
        public int[]    SupporterIds;
    }


    [Header("[PVP Property]")]
    public bool         RandomCharacters        = true;
    public Camera[]     CamRenderTex            = null;
    public PVPEnemyMgr  PVPEnemyMgr             = null;
    public Vector3      StartPlayerPos          = new Vector3(0.0f, 0.0f, -3.5f);
    public Vector3      StartOpponentPos        = new Vector3(0.0f, 0.0f, 3.5f);

    [Header("[PVP Test Player Team]")]
    public sTestPVPCharData[]   PlayerTeam;
    public int                  PlayerNpcCharRndTableID = 31;
    public bool                 InvinciblePlayerTeam    = false;

    [Header("[PVP Test Opponent Team]")]
    public sTestPVPCharData[]   OpponentTeam;
    public int                  OpponentNpcCharRndTableID   = 31;
    public bool                 InvincibleOpponentTeam      = false;

    public bool     AsTestScene             { get; set; }           = false;
    public bool     IsBattleStart           { get; set; }           = false;
    public bool     IsAnyPlayerDead         { get; set; }           = false;
    public Player[] PlayerChars             { get; private set; }   = new Player[(int)eArenaTeamSlotPos._MAX_];
    public int      CurPlayerCharIndex      { get; private set; }   = 0;
    public Player[] OpponentChars           { get; private set; }   = new Player[(int)eArenaTeamSlotPos._MAX_];
    public int      CurOpponentCharIndex    { get; private set; }   = 0;

    public float                        TotalGameTime       { get; private set; }   = 0.0f;
    public long                         AddGrade            { get; private set; }   = 0;
    public long                         AddCoin             { get; private set; }   = 0;
    public ePromoteState                PromoteState        { get; private set; }   = ePromoteState.None;
    public int                          StreakCount         { get; private set; }   = 0;
    public bool                         IsNewRecord         { get; private set; }   = false;          
    public int                          CurrentGrade        { get; private set; }   = 1;
    public GameTable.ArenaGrade.Param   ParamBeforeGrade    { get; private set; }   = null;
    public GameTable.ArenaGrade.Param   ParamCurrentGrade   { get; private set; }   = null;
    public byte                         Result              { get; private set; }   = 0;
    public float                        RemainTime          { get; private set; }   = 0.0f;
    public bool                         IsFriendPVP         { get; private set; }   = false;
	public int                          GuardianIndex       { get; private set; }   = -1;

    private List<CharData>  mListTeamChar               = new List<CharData>(); // GameInfo.Instance.TeamcharList 참조
    private int[]           mPlayerCharsRenderTexLayer  = new int[3] { (int)eLayer.Player, (int)eLayer.Enemy, (int)eLayer.RenderTarget };
    private BOItem[]        mBOCheers                   = null;
    private Coroutine       mCrStatCheer                = null;
    private TeamData        mOpponentTeamData           = null;
    private int             mBeforePlayerCharIndex      = -1;
    private int             mBeforeOpponentCharIndex    = -1;
    private int             mBeforeStreak               = 0;
    private GameObject      mPlayerTagObj               = null;
    private bool            mbPlayingVoice              = false;
    private float[]         mAudioSamples               = new float[128];
    private bool            mbSurrender                 = false;

    private float           mPlayerAddHpRate            = 0.0f;
    private float           mOpponentAddHpRate          = 0.0f;

    private float           mUIPrepareOpenTime          = 0.0f;
    private float           mUIPrepareCloseTime         = 0.0f;
    private float           mUIPlayOpenTime             = 0.0f;
    private float           mUIPlayCloseTime            = 0.0f;

    private bool            mSaveAutoTargeting          = false;
    private bool            mSaveAutoTargetingSkill     = false;


    public override void Init(int tableId, eSTAGETYPE stageType = eSTAGETYPE.STAGE_NONE)
    {
#if UNITY_EDITOR
        //InvinciblePlayer = true;
#endif

        mSaveAutoTargeting = FSaveData.Instance.AutoTargeting;
        mSaveAutoTargetingSkill = FSaveData.Instance.AutoTargetingSkill;

        // PVP에서 수동 타겟팅은 없음
        FSaveData.Instance.AutoTargeting = true;
        FSaveData.Instance.AutoTargetingSkill = true;

        if (AsTestScene)
        {
            //GameInfo.Instance.ArenaATK_Buff_Flag = true;
            //GameInfo.Instance.ArenaDEF_Buff_Flag = true;

            /*
            for(int i = 0; i < OpponentTeam.Length; i++)
            {
                OpponentTeam[i].TableId = Random.Range(1, 7);
            }
            */

            CreateTestPlayerTeamData();
            CreateTestOpponentTeamData();
        }
        else
        {
            mListTeamChar.Clear();
            for(int i = 0; i < GameInfo.Instance.TeamcharList.Count; i++)
            {
                CharData charData = GameInfo.Instance.GetCharData(GameInfo.Instance.TeamcharList[i]);
                mListTeamChar.Add(charData);
            }

            mOpponentTeamData = GameInfo.Instance.MatchTeam;
        }

        base.Init(tableId, stageType);

        mBgm = LoadAudioClip("BGM/snd_bgm_pvp_01.ogg");
        Time.timeScale = 1.0f;
        mbPlayingVoice = false;
        mCrStatCheer = null;
        mbSurrender = false;

        TotalGameTime = 0.0f;
        AddGrade = GameInfo.Instance.UserBattleData.Now_Score;
        AddCoin = GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.BATTLECOIN];
        PromoteState = GameInfo.Instance.UserBattleData.Now_PromotionRemainCnt > 0 ? ePromoteState.Testing : ePromoteState.None;
        mBeforeStreak = GameInfo.Instance.UserBattleData.SR_BestWinningStreak;

        ParamBeforeGrade = GameInfo.Instance.GameTable.FindArenaGrade(Mathf.Max(1, (int)GameInfo.Instance.UserBattleData.Now_GradeId));
        CurrentGrade = ParamBeforeGrade.Grade;

        EnemyMgr = PVPEnemyMgr;
        EnemyMgr.Clear();
        
        UIPVP = GameUIManager.Instance.GetUI<UIArenaPlayPanel>("ArenaPlayPanel");
        UIPVP.InitCheer();

        mOpponentTeamData = GameInfo.Instance.MatchTeam;

        GameUIManager.Instance.HideUI("GamePlayPanel", false);
        GameUIManager.Instance.HideUI("ArenaVsPanel", false);
        GameUIManager.Instance.HideUI("ArenaPlayPanel", false);
        GameUIManager.Instance.HideUI("ArenaResultPopup", false);

        GameInfo.Instance.SetCardFormationData(GameInfo.Instance.UserData.ArenaCardFormationID);

        CreatePlayerChars();
        CreateOpponentChars();

        mPlayerTagObj = ResourceMgr.Instance.CreateFromAssetBundle("unit", "Unit/PVPPlayerTag.prefab");
        mPlayerTagObj.SetActive(false);

        mBOCheers = new BOItem[GameInfo.Instance.BattleConfig.CheerEffBOList.Count];
        for (int i = 0; i < mBOCheers.Length; i++)
        {
            mBOCheers[i] = new BOItem(GameInfo.Instance.BattleConfig.CheerEffBOList[i], World.Instance.Player);
        }

        Invoke("PrepareBattle", 1.0f);

        PostInit();
        EndIntro();

        IsFriendPVP = false;
        if (UIValue.Instance.ContainsKey(UIValue.EParamType.IsFriendPVP))
        {
            IsFriendPVP = (bool)UIValue.Instance.GetValue(UIValue.EParamType.IsFriendPVP, true);
        }
    }

    public Player GetCurrentPlayerTeamCharOrNull()
    {
        if(CurPlayerCharIndex < 0 || CurPlayerCharIndex >= PlayerChars.Length)
        {
            return null;
        }

        return PlayerChars[CurPlayerCharIndex];
    }

    public Player GetCurrentOpponentTeamCharOrNull()
    {
        if (CurOpponentCharIndex < 0 || CurOpponentCharIndex >= OpponentChars.Length)
        {
            return null;
        }

        return OpponentChars[CurOpponentCharIndex];
    }

    public bool StartCheer(UIArenaPlayPanel.sUITeamInfo uiTeamInfo, UILabel lbCheerBuff)
    {
        Player currentPlayer = GetCurrentPlayerTeamCharOrNull();
        Player currentOpponent = GetCurrentOpponentTeamCharOrNull();

        if (currentPlayer.actionSystem.IsCurrentAction(eActionCommand.Die) || currentOpponent.actionSystem.IsCurrentAction(eActionCommand.Die))
        {
            return false;
        }

        if(currentPlayer.curHp <= 0.0f || currentOpponent.curHp <= 0.0f)
        {
            return false;
        }

        int rand = UnityEngine.Random.Range(0, mBOCheers.Length);
        if (UIPVP.AddCheerValueUpRate > 0)
        {
            mBOCheers[rand].AddBattleOptionValue1(UIPVP.AddCheerValueUpRate);
        }

        mBOCheers[rand].SetOwner(currentPlayer);
        mBOCheers[rand].ChangeSender(currentPlayer);
        mBOCheers[rand].Execute(BattleOption.eBOTimingType.Use);

        float value = mBOCheers[rand].ListBattleOptionData[0].value * (float)eCOUNT.MAX_BO_FUNC_VALUE;
        lbCheerBuff.text = string.Format(FLocalizeString.Instance.GetText(GameInfo.Instance.BattleConfig.CheerEffDescList[rand]), value);

        PlayCheerVoice(uiTeamInfo);
        return true;
    }

    public void Surrender()
    {
        if(mbSurrender || IsEndGame || ProcessingEnd)
        {
            return;
        }

        mbSurrender = true;

        Player player = PlayerChars[CurPlayerCharIndex];
        player.OnPVPSurrender();

        Player opponent = OpponentChars[CurOpponentCharIndex];
        opponent.OnPVPSurrender();

        if(opponent.curHp <= 0.0f)
        {
            opponent.SetHp(1.0f);
        }

        player.SetHp(0.0f);
        player.PlayAni(eAnimation.Die);

        EndGame(eEventType.EVENT_GAME_PLAYER_DEAD, null);
    }

    public override void ToLobby()
    {
        Time.timeScale = 1.0f;

        FSaveData.Instance.AutoTargeting = mSaveAutoTargeting;
        FSaveData.Instance.AutoTargetingSkill = mSaveAutoTargetingSkill;

        GameUIManager.Instance.HideUI("ArenaVsPanel", false);
        GameUIManager.Instance.HideUI("ArenaPlayPanel", false);
        GameUIManager.Instance.HideUI("ArenaResultPopup", false);

        base.ToLobby();
    }

    protected override void EndGame(eEventType eventType, Unit sender)
    {
        if (IsEndGame)
        {
            return;
        }

        IsBattleStart = false;

        Debug.Log("WorldPVP::EndGame^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
        StopCoroutine("UpdateTimer");

        InGameCamera.ResetPostProcess();
        mPlayerTagObj.SetActive(false);

        Player player = PlayerChars[CurPlayerCharIndex];
        player.AI.StopBT();
        player.OnAfterPVPBattle();
        //player.actionSystem.CancelCurrentAction();

        ActionUSkillBase actionUSkillBase = null;
        if (player.UsingUltimateSkill)
        {
            actionUSkillBase = player.actionSystem.GetAction<ActionUSkillBase>(eActionCommand.USkill01);
            if(actionUSkillBase)
            {
                actionUSkillBase.ForceEnd(true);
            }
        }

        Player opponent = OpponentChars[CurOpponentCharIndex];
        opponent.AI.StopBT();
        opponent.OnAfterPVPBattle();

        if (opponent.UsingUltimateSkill)
        {
            actionUSkillBase = opponent.actionSystem.GetAction<ActionUSkillBase>(eActionCommand.USkill01);
            if (actionUSkillBase)
            {
                actionUSkillBase.ForceEnd(true);
            }
        }
        //opponent.actionSystem.CancelCurrentAction();

        /*
        if (!player.isGrounded)
        {
            player.SetFallingRigidBody();
        }

        if (!opponent.isGrounded)
        {
            player.SetFallingRigidBody();
        }
        */

        Player winner = null;
        Player loser = null;

        if (opponent.curHp <= 0.0f)
        {
            winner = player;
            loser = opponent;
        }
        else
        {
            winner = opponent;
            loser = player;
        }

        ++winner.PVPWinCount;
        loser.actionSystem.CancelCurrentAction();
        loser.InitSupporterCoolTime(true);

        ProjectileMgr.DestroyAllProjectile(false);
        EffectManager.Instance.StopAll("Effect/Character/prf_fx_noah_dna_ring");

        if(winner.curHp <= 0.0f)
        {
            winner.SetHp(1.0f);
            UIPVP.AddPlayerHp(winner);
        }

        UIPVP.ShowBattleResult(!winner.OpponentPlayer);

        //float aniLength = winner.PlayAni(eAnimation.Win);
        Invoke("CheckEndBattle", 1.5f);
    }

	public override void AllUnitRetarget() {
		Player player = GetCurrentPlayerTeamCharOrNull();
		if( player ) {
			player.Retarget();
		}

		Player opponent = GetCurrentOpponentTeamCharOrNull();
		if( opponent ) {
			opponent.Retarget();
		}
	}

	protected override void Update()
    {
        if(mbPlayingVoice)
        {
            UpdatePlayerMouth();
        }

        if(IsEndGame || mPlayerTagObj == null || !mPlayerTagObj.activeSelf)
        {
            return;
        }

        mPlayerTagObj.transform.position = new Vector3(PlayerChars[CurPlayerCharIndex].transform.position.x,
                                                       PlayerChars[CurPlayerCharIndex].transform.position.y + PlayerChars[CurPlayerCharIndex].Height + 0.6f,
                                                       PlayerChars[CurPlayerCharIndex].transform.position.z);

        //mPlayerTagObj.transform.Rotate(0.0f, 30.0f * Time.deltaTime, 0.0f, Space.Self);

        Player player = PlayerChars[CurPlayerCharIndex];
        if(player && player.transform.position.y < 0.0f)
        {
            Vector3 pos = player.transform.position;
            pos.y = 0.0f;
            player.transform.position = pos;
        }

        Player opponent = OpponentChars[CurOpponentCharIndex];
        if(opponent && opponent.transform.position.y < 0.0f)
        {
            Vector3 pos = opponent.transform.position;
            pos.y = 0.0f;
            opponent.transform.position = pos;
        }

#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			opponent.StopBT();
		}
#endif
	}

    private void CreatePlayerChars()
    {
        int firstCharIndex = -1;
        for (int i = 0; i < mListTeamChar.Count; i++)
        {
            CharData charData = mListTeamChar[i];
            if(charData == null)
            {
                continue;
            }

            if(firstCharIndex == -1)
            {
                firstCharIndex = i;
            }

            PlayerChars[i] = GameSupport.CreatePlayer(charData);

            if (GameInfo.Instance.SelectedCardFormationTID > 0 && GameInfo.Instance.CardFormationTableData != null)
            {
                PlayerChars[i].AddBOCharBattleOptionSet(GameInfo.Instance.CardFormationTableData.FormationBOSetID1, 1);
                PlayerChars[i].AddBOCharBattleOptionSet(GameInfo.Instance.CardFormationTableData.FormationBOSetID2, 1);
            }

            for (int j = 0; j < GameInfo.Instance.UserData.ListAwakenSkillData.Count; j++)
            {
                AwakenSkillInfo info = GameInfo.Instance.UserData.ListAwakenSkillData[j];
                if (info.Level <= 0)
                {
                    continue;
                }

                GameTable.AwakeSkill.Param param = GameInfo.Instance.GameTable.FindAwakeSkill(info.TableId);
                if (param == null)
                {
                    Debug.LogError(info.TableId + "번 각성 스킬 정보가 없습니다.");
                    continue;
                }

                PlayerChars[i].AddBOCharBattleOptionSet(param.SptAddBOSetID1, info.Level);
                PlayerChars[i].AddBOCharBattleOptionSet(param.SptAddBOSetID2, info.Level);
            }

            PlayerChars[i].SetStatsForPVPPlayer(i);
            PlayerChars[i].Set2ndStatsForPVPPlayer();
            PlayerChars[i].costumeUnit.SetCostume(charData);
            PlayerChars[i].GetBones();
            PlayerChars[i].AddAIController(PlayerChars[i].charData.TableData.AI);

            Utility.ForceChangeTransparentFXLayer(PlayerChars[i].gameObject, (int)eLayer.Player, true);

            PlayerChars[i].SetLockAxis(Unit.eAxisType.Z);
            //PlayerChars[i].Input.LockDirection(true);
            PlayerChars[i].Input.LockBtnFlag(InputController.ELockBtnFlag.ATTACK | InputController.ELockBtnFlag.DASH | InputController.ELockBtnFlag.USKILL |
                                              InputController.ELockBtnFlag.WEAPON | InputController.ELockBtnFlag.SUPPORTER);

            PlayerChars[i].AddDirector();
            PlayerChars[i].CreateClone(PlayerChars[i].CloneCount);
            PlayerChars[i].Deactivate();
            PlayerChars[i].OnAfterCreateInPVP();

			if ( PlayerChars[i].UseGuardian ) {
                PlayerChars[i].costumeUnit.SetGuardianOwnerPlayer( PlayerChars[i], true );
				if ( PlayerChars[i].Guardian ) {
                    Utility.SetLayer( PlayerChars[i].Guardian.gameObject, (int)eLayer.Player, true );
					PlayerChars[i].Guardian.SetLockAxis( Unit.eAxisType.Z );
				}
				GameSupport.SetGuardianSkill( PlayerChars[i], charData );
			}

			PlayerChars[i].OnAfterAttackPower();
			PlayerChars[i].PVPWinCount = 0;
        }

        CurPlayerCharIndex = firstCharIndex;
        mBeforePlayerCharIndex = -1;

        SetBadgeStats(PlayerChars, false, GameSupport.GetEquipBadgeList(eContentsPosKind.ARENA));
    }

    private void CreateOpponentChars()
    {
        int firstCharIndex = -1;
        for (int i = 0; i < mOpponentTeamData.charlist.Count; i++)
        {
            TeamCharData teamCharData = mOpponentTeamData.charlist[i];
            if(teamCharData == null || teamCharData.CharData == null || teamCharData.CharData.TableID <= 0)
            {
                continue;
            }

            if (firstCharIndex == -1)
            {
                firstCharIndex = i;
            }

            OpponentChars[i] = GameSupport.CreatePlayer(teamCharData.CharData, false, true);

            if (mOpponentTeamData.CardFormtaionID > 0)
            {
                GameTable.CardFormation.Param param = GameInfo.Instance.GameTable.FindCardFormation(mOpponentTeamData.CardFormtaionID);
                if (GameInfo.Instance.CardFormationTableData != null)
                {
                    OpponentChars[i].AddBOCharBattleOptionSet(param.FormationBOSetID1, 1);
                    OpponentChars[i].AddBOCharBattleOptionSet(param.FormationBOSetID2, 1);
                }
            }

            for (int j = 0; j < teamCharData.ListAwakenSkillInfo.Count; j++)
            {
                AwakenSkillInfo info = teamCharData.ListAwakenSkillInfo[j];
                if (info.Level <= 0)
                {
                    continue;
                }

                GameTable.AwakeSkill.Param param = GameInfo.Instance.GameTable.FindAwakeSkill(info.TableId);
                if (param == null)
                {
                    Debug.LogError(info.TableId + "번 각성 스킬 정보가 없습니다.");
                    continue;
                }

                OpponentChars[i].AddBOCharBattleOptionSet(param.SptAddBOSetID1, info.Level);
                OpponentChars[i].AddBOCharBattleOptionSet(param.SptAddBOSetID2, info.Level);
            }

            OpponentChars[i].SetOpponentPlayer(teamCharData.CharData.TableData.AI);

            float teamHp = 0.0f;
            float teamAtk = 0.0f;

            teamHp = mOpponentTeamData.TeamHP;
            teamAtk = mOpponentTeamData.TeamATK;

            OpponentChars[i].SetAction(TestScene != null, teamCharData.CardList);
            OpponentChars[i].SetStatsForPVPOpponent(teamCharData.CardList, teamCharData.MainWeaponData, teamCharData.SubWeaponData, teamCharData.MainGemList, 
                                                    teamCharData.SubGemList, i, teamHp, teamAtk);
            OpponentChars[i].Set2ndStatsForPVPOpponent(teamCharData.MainGemList);
            OpponentChars[i].costumeUnit.SetCostume(teamCharData.CharData, teamCharData.MainWeaponData, teamCharData.SubWeaponData);
            OpponentChars[i].GetBones();

            Utility.SetLayer(OpponentChars[i].gameObject, (int)eLayer.Enemy, true, (int)eLayer.PostProcess);
            Utility.ForceChangeTransparentFXLayer(OpponentChars[i].gameObject, (int)eLayer.Enemy, true);

            OpponentChars[i].SetLockAxis(Unit.eAxisType.Z);
            //OpponentChars[i].Input.LockDirection(true);
            OpponentChars[i].Input.LockBtnFlag(InputController.ELockBtnFlag.ATTACK | InputController.ELockBtnFlag.DASH | InputController.ELockBtnFlag.USKILL |
                                                InputController.ELockBtnFlag.WEAPON | InputController.ELockBtnFlag.SUPPORTER);

            OpponentChars[i].AddDirector();
            OpponentChars[i].CreateClone(OpponentChars[i].CloneCount, teamCharData.CharData, teamCharData.MainWeaponData, teamCharData.SubWeaponData);
            OpponentChars[i].Deactivate();
            OpponentChars[i].OnAfterCreateInPVP();

			if ( OpponentChars[i].UseGuardian ) {
				OpponentChars[i].costumeUnit.SetGuardianOwnerPlayer( OpponentChars[i], true );
				if ( OpponentChars[i].Guardian ) {
					Utility.SetLayer( OpponentChars[i].Guardian.gameObject, (int)eLayer.Enemy, true );
					OpponentChars[i].Guardian.SetLockAxis( Unit.eAxisType.Z );
				}
				GameSupport.SetGuardianSkill( OpponentChars[i], teamCharData.CharData );
			}

			OpponentChars[i].OnAfterAttackPower();
			OpponentChars[i].PVPWinCount = 0;
        }

        CurOpponentCharIndex = firstCharIndex;
        mBeforeOpponentCharIndex = -1;

        SetBadgeStats(OpponentChars, true, mOpponentTeamData.badgelist);
    }

    private void SetBadgeStats(Player[] players, bool isOpponent, List<BadgeData> listBadge)
    {
        if(listBadge == null || listBadge.Count <= 0)
        {
            return;
        }

        List<ObscuredFloat> listPlayerMaxHp = new List<ObscuredFloat>();
        List<ObscuredFloat> listPlayerAttackPower = new List<ObscuredFloat>();
        List<ObscuredFloat> listPlayerDefenceRate = new List<ObscuredFloat>();
        List<ObscuredFloat> listPlayerCriticalRate = new List<ObscuredFloat>();

        for(int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                listPlayerMaxHp.Add(players[i].maxHp);
                listPlayerAttackPower.Add(players[i].attackPower);
                listPlayerDefenceRate.Add(players[i].defenceRate);
                listPlayerCriticalRate.Add(players[i].criticalRate);
            }
            else
            {
                listPlayerMaxHp.Add(0.0f);
                listPlayerAttackPower.Add(0.0f);
                listPlayerDefenceRate.Add(0.0f);
                listPlayerCriticalRate.Add(0.0f);
            }
        }

        // 메인 옵션 체크
        int mainOptID = 0;
        int mainOptEquipCnt = 0;

        if (listBadge[(int)eBadgeSlot.FIRST - 1] != null)
        {
            mainOptID = listBadge[(int)eBadgeSlot.FIRST - 1].OptID[(int)eBadgeOptSlot.FIRST];
            mainOptEquipCnt = GameSupport.GetMainOptEquipCnt(listBadge, mainOptID);
        }

        for (int i = 0; i < listBadge.Count; i++)
        {
            BadgeData data = listBadge[i];
            if(data == null)
            {
                continue;
            }

            for (int j = 0; j < data.OptID.Length; j++)
            {
                if (data.OptID[j] <= 0)
                {
                    continue;
                }

                GameTable.BadgeOpt.Param param = GameInfo.Instance.GameTable.FindBadgeOpt(x => x.OptionID == data.OptID[j]);
                if (param == null)
                {
                    continue;
                }

                float value = ((data.OptVal[j] + data.Level) * param.IncEffectValue) / (float)eCOUNT.MAX_RATE_VALUE;
                if (mainOptID == data.OptID[j])
                {
                    value += (value * GameInfo.Instance.GameConfig.BadgeSetAddRate[mainOptEquipCnt]);
                }

                string str = param.EffectType;
				string[] splits = Utility.Split(str, '_'); //str.Split('_');
                Unit2ndStatsTable.eBadgeOptType optType = Utility.GetEnumByString<Unit2ndStatsTable.eBadgeOptType>(splits[1]);
                if (optType == Unit2ndStatsTable.eBadgeOptType.CheerAddRate) // 응원 게이지 충전량 증가
                {
                    UIPVP.AddCheerRate += value;
                }
                else if (!isOpponent && optType == Unit2ndStatsTable.eBadgeOptType.CheerRegenRate) // 응원 게이지 충전 속도 증가
                {
                    UIPVP.AddCheerRegenRate += value;
                }
                else if (!isOpponent && optType == Unit2ndStatsTable.eBadgeOptType.CheerEffUp) // 응원 효과 증가
                {
                    UIPVP.AddCheerValueUpRate += value;
                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.WinRecoveryHP) // 라운드 승리 시 HP 회복량
                {
                    if(isOpponent)
                    {
                        mOpponentAddHpRate += value;
                    }
                    else
                    {
                        mPlayerAddHpRate += value;
                    }
                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.CriResistUp) // 치명 저항 확률 증가
                {

                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.CriDefUp) // 치명 피해량 감소
                {

                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.StartCharUp) // 선봉 캐릭터 능력(HP, 공격력) 강화
                {
                    if (players[(int)eArenaTeamSlotPos.START_POS])
                    {
                        players[(int)eArenaTeamSlotPos.START_POS].AddMaxHp(listPlayerMaxHp[(int)eArenaTeamSlotPos.START_POS], value);
                        players[(int)eArenaTeamSlotPos.START_POS].AddAttackPower(listPlayerAttackPower[(int)eArenaTeamSlotPos.START_POS], value);
                    }
                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.MidCharUp) // 중견 캐릭터 능력(HP, 공격력) 강화
                {
                    if (players[(int)eArenaTeamSlotPos.MID_POS])
                    {
                        players[(int)eArenaTeamSlotPos.MID_POS].AddMaxHp(listPlayerMaxHp[(int)eArenaTeamSlotPos.MID_POS], value);
                        players[(int)eArenaTeamSlotPos.MID_POS].AddAttackPower(listPlayerAttackPower[(int)eArenaTeamSlotPos.MID_POS], value);
                    }
                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.LastCharUp) // 대장 캐릭터 능력(HP, 공격력) 강화
                {
                    if (players[(int)eArenaTeamSlotPos.LAST_POS])
                    {
                        players[(int)eArenaTeamSlotPos.LAST_POS].AddMaxHp(listPlayerMaxHp[(int)eArenaTeamSlotPos.LAST_POS], value);
                        players[(int)eArenaTeamSlotPos.LAST_POS].AddAttackPower(listPlayerAttackPower[(int)eArenaTeamSlotPos.LAST_POS], value);
                    }
                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.CharHPUp) // 전 캐릭터 체력 강화
                {
                    for(int k = 0; k < players.Length; k++)
                    {
                        if(players[k] == null)
                        {
                            continue;
                        }

                        players[k].AddMaxHp(listPlayerMaxHp[k], value);
                    }
                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.CharATKUp) // 전 캐릭터 공격력 강화
                {
                    for (int k = 0; k < players.Length; k++)
                    {
                        if (players[k] == null)
                        {
                            continue;
                        }

                        players[k].AddAttackPower(listPlayerAttackPower[k], value);
                    }
                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.CharDEFUp) // 전 캐릭터 방어력 강화
                {
                    for (int k = 0; k < players.Length; k++)
                    {
                        if (players[k] == null)
                        {
                            continue;
                        }

                        players[k].AddDefenceRate(listPlayerDefenceRate[k], value);
                    }
                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.CharCRIUp) // 전 캐릭터 치명확률 강화 
                {
                    for (int k = 0; k < players.Length; k++)
                    {
                        if (players[k] == null)
                        {
                            continue;
                        }

                        players[k].AddCriticalRate(listPlayerCriticalRate[k], value);
                    }
                }
            }
        }
    }

    private void PrepareBattle()
    {
        Time.timeScale = 1.0f;

        FComponent comp = GameUIManager.Instance.ShowUI("ArenaVsPanel", true);
        mUIPrepareOpenTime = comp.GetOpenAniTime();
        mUIPrepareCloseTime = comp.GetCloseAniTime();

        ActiveChars();
    }

    private void ActiveChars()
    {
        InGameCamera.ExcludCullingMask((int)eLayer.Player);
        InGameCamera.ExcludCullingMask((int)eLayer.Enemy);
        InGameCamera.ExcludCullingMask((int)eLayer.TransparentFX);

		// Player
        Player = PlayerChars[CurPlayerCharIndex];
        Player.SetInitialPosition(new Vector3(-20.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 180.0f, 0.0f));
        Player.alwaysKinematic = true;
        Player.SetKinematicRigidBody();
        Player.costumeUnit.ChangeAllWeaponName(Player.ChangeWeaponName);

        Player opponent = OpponentChars[CurOpponentCharIndex];
        opponent.SetInitialPosition(new Vector3(20.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 180.0f, 0.0f));
        opponent.alwaysKinematic = true;
        opponent.SetKinematicRigidBody();
        opponent.costumeUnit.ChangeAllWeaponName(opponent.ChangeWeaponName);

        Player.Activate();
        Player.InitSupporterCoolTimeDecRate();
		Player.aniEvent.Rebind();

        if (mBeforePlayerCharIndex > -1)
        {
            Player.AddSp(PlayerChars[mBeforePlayerCharIndex].curSp);
        }
        else
        {
            Player.AddSp(0.0f);
        }

        Player.SetMainTarget(opponent);
        Player.lookTargetOnHit = true;
        Player.EnableBoneFace();
        float aniLength = Player.PlayAni(eAnimation.Lobby_Weapon, 0, eFaceAnimation.Weapon, 0);
        Invoke("PlayPlayerWeaponIdleAni", aniLength);

		if ( Player.UseGuardian && Player.Guardian != null ) {
			Player.Guardian.SetInitialPosition( Player.transform.position, Player.transform.rotation );
			Player.Guardian.Activate();
			Player.Guardian.PlayAni( eAnimation.Lobby_Weapon );
		}

		CamRenderTex[0].gameObject.SetActive(true);
        CamRenderTex[0].transform.SetParent(Player.transform);
        CamRenderTex[0].orthographicSize = Player.PVPRenderTexCamSize;

        if (Player.tableId == 8)
        {
            Utility.InitTransform(CamRenderTex[0].gameObject, new Vector3(-0.25f, 1.3f, 3.0f), Quaternion.Euler(0.0f, 180.0f, 0.0f), Vector3.one);
        }
        else
        {
            Utility.InitTransform(CamRenderTex[0].gameObject, new Vector3(-0.25f, 1.15f, 3.0f), Quaternion.Euler(0.0f, 180.0f, 0.0f), Vector3.one);
        }

		// Opponent
        opponent.Activate();
        opponent.InitSupporterCoolTimeDecRate();

		if (mBeforeOpponentCharIndex > -1)
        {
            opponent.AddSp(OpponentChars[mBeforeOpponentCharIndex].curSp);
        }
        else
        {
            opponent.AddSp(0.0f);
        }

        opponent.SetMainTarget(Player);
        opponent.lookTargetOnHit = true;
        opponent.EnableBoneFace();
        aniLength = opponent.PlayAni(eAnimation.Lobby_Weapon, 0, eFaceAnimation.Weapon, 0);
        Invoke("PlayOpponentWeaponIdleAni", aniLength);

		if ( opponent.UseGuardian && opponent.Guardian != null ) {
			opponent.Guardian.SetInitialPosition( opponent.transform.position, opponent.transform.rotation );
			opponent.Guardian.Activate();
			opponent.Guardian.PlayAni( eAnimation.Lobby_Weapon );
		}

		CamRenderTex[1].gameObject.SetActive(true);
        CamRenderTex[1].transform.SetParent(opponent.transform);
        CamRenderTex[1].orthographicSize = opponent.PVPRenderTexCamSize;

        if (opponent.tableId == 8)
        {
            Utility.InitTransform(CamRenderTex[1].gameObject, new Vector3(0.25f, 1.3f, 3.0f), Quaternion.Euler(0.0f, 180.0f, 0.0f), Vector3.one);
        }
        else
        {
            Utility.InitTransform(CamRenderTex[1].gameObject, new Vector3(0.25f, 1.15f, 3.0f), Quaternion.Euler(0.0f, 180.0f, 0.0f), Vector3.one);
        }

        EnemyMgr.AddEnemy(Player);
        EnemyMgr.AddEnemy(opponent);

        InGameCamera.SetPlayer(Player);

        PlayEnterVoice(opponent);
        Invoke("CloseStartUI", 5.0f);
	}

    private void PlayEnterVoice(Player opponentPlayer)
    {
        SoundTable.VoiceArenaType.Param param = SoundManager.Instance.SoundTable.FindVoiceArenaType(x => x.CharID == Player.tableId && x.Type == (int)eVOICEARENATYPE.ArenaEnter);
        if(param == null)
        {
            return;
        }

        eVOICEARENATYPENAME typeName = eVOICEARENATYPENAME.ArenaEnterR;
        int index = -1;

		string[] split = Utility.Split(param.ConnectChar, ','); //param.ConnectChar.Split(',');
		for (int i = 0; i < split.Length; i++)
        {
            if(opponentPlayer.tableId == int.Parse(split[i]))
            {
                typeName = eVOICEARENATYPENAME.ArenaEnterC;
                index = opponentPlayer.tableId;

                break;
            }
            else if(opponentPlayer.tableId == Player.tableId)
            {
                typeName = eVOICEARENATYPENAME.ArenaEnterS;
                index = 1;

                break;
            }
        }

        VoiceMgr.Instance.PlayPVPChar(Player.tableId, eVOICEARENATYPE.ArenaEnter, typeName, index, param.RndCnt);
        mbPlayingVoice = true;
    }

    private void PlayCheerVoice(UIArenaPlayPanel.sUITeamInfo uiTeamInfo)
    {
        if(PlayerChars.Length <= 1)
        {
            return;
        }

        eVOICEARENATYPENAME typeName = eVOICEARENATYPENAME.ArenaCheerR;
        List<int> listConnectionPlayerIndex = new List<int>();
        List<int> listPlayerIndexExceptCurPlayer = new List<int>();
        int cheerCharIndex = -1;

        // 팀원들 중 현재 플레이어와 인연인 캐릭터를 찾는다.
        SoundTable.VoiceArenaType.Param param = null;
        for (int i = 0; i < PlayerChars.Length; i++)
        {
            if(PlayerChars[i] == null || i == CurPlayerCharIndex)
            {
                continue;
            }

            listPlayerIndexExceptCurPlayer.Add(i);

            param = SoundManager.Instance.SoundTable.FindVoiceArenaType(x => x.CharID == PlayerChars[i].tableId && x.Type == (int)eVOICEARENATYPE.ArenaCheer);
            if(param == null)
            {
                continue;
            }

			string[] split = Utility.Split(param.ConnectChar, ','); //param.ConnectChar.Split(',');
            for(int j = 0; j < split.Length; j++)
            {
                if(PlayerChars[CurPlayerCharIndex].tableId == int.Parse(split[j]))
                {
                    listConnectionPlayerIndex.Add(i);
                }
            }
        }

        if(param == null)
        {
            return;
        }

        if(listConnectionPlayerIndex.Count > 0)
        { // 현재 플레이어와 인연인 팀원 중에 랜덤으로 선택해서 응원
            typeName = eVOICEARENATYPENAME.ArenaCheerC;

            int rand = Random.Range(0, listConnectionPlayerIndex.Count);
            cheerCharIndex = listConnectionPlayerIndex[rand];
        }
        else if(listPlayerIndexExceptCurPlayer.Count > 0)
        {// 인연인 팀원이 없으면 팀원 중 랜덤으로 선택해서 응원
            int rand = Random.Range(0, listPlayerIndexExceptCurPlayer.Count);
            cheerCharIndex = listPlayerIndexExceptCurPlayer[rand];
        }

        if(cheerCharIndex == -1 || cheerCharIndex >= uiTeamInfo.CharInfos.Length)
        {
            return;
        }
        
        int index = -1;
        if(typeName == eVOICEARENATYPENAME.ArenaCheerC)
        {
            index = PlayerChars[CurPlayerCharIndex].tableId;
        }

        SoundManager.sSoundInfo soundInfo = VoiceMgr.Instance.PlayPVPChar(PlayerChars[cheerCharIndex].tableId, eVOICEARENATYPE.ArenaCheer, typeName, index, param.RndCnt);
        if(soundInfo == null || soundInfo.clip == null)
        {
            return;
        }

        if(mCrStatCheer != null)
        {
            Utility.StopCoroutine(this, ref mCrStatCheer);
        }

        mCrStatCheer = StartCoroutine(ShowSpeakUI(uiTeamInfo.CharInfos[cheerCharIndex], soundInfo.clip.length));
    }

    private IEnumerator ShowSpeakUI(UIArenaPlayPanel.sCharInfo uiCharInfo, float delay)
    {
        uiCharInfo.ShowSpeak(true);

        yield return new WaitForSeconds(delay);
        uiCharInfo.ShowSpeak(false);
    }

    private int GetPlayerIndexByTableId(int tableId)
    {
        for(int i = 0; i < PlayerChars.Length; i++)
        {
            if(PlayerChars[i] && PlayerChars[i].tableId == tableId && i != CurPlayerCharIndex)
            {
                return i;
            }
        }

        return -1;
    }

    private void PlayPlayerWeaponIdleAni()
    {
        PlayerChars[CurPlayerCharIndex].PlayAni(eAnimation.Lobby_Weapon_Idle, 0, eFaceAnimation.Weapon_Idle, 0);

		if ( PlayerChars[CurPlayerCharIndex].UseGuardian && PlayerChars[CurPlayerCharIndex].Guardian != null ) {
			PlayerChars[CurPlayerCharIndex].Guardian.PlayAni( eAnimation.Lobby_Weapon_Idle );
		}
	}

    private void PlayOpponentWeaponIdleAni()
    {
        OpponentChars[CurOpponentCharIndex].PlayAni(eAnimation.Lobby_Weapon_Idle, 0, eFaceAnimation.Weapon_Idle, 0);

		if ( OpponentChars[CurOpponentCharIndex].UseGuardian && OpponentChars[CurOpponentCharIndex].Guardian != null ) {
            OpponentChars[CurOpponentCharIndex].Guardian.PlayAni( eAnimation.Lobby_Weapon_Idle );
		}
	}

    private void CloseStartUI()
    {
        InGameCamera.RestoreCullingMask();
        mbPlayingVoice = false;

        for (int i = 0; i < CamRenderTex.Length; i++)
        {
            CamRenderTex[i].gameObject.SetActive(false);
        }

        PlayerChars[CurPlayerCharIndex].alwaysKinematic = false;
        PlayerChars[CurPlayerCharIndex].SetInitialPosition(StartPlayerPos, Quaternion.Euler(0.0f, 90.0f, 0.0f));
        PlayerChars[CurPlayerCharIndex].SetGroundedRigidBody();
        PlayerChars[CurPlayerCharIndex].PlayAni(eAnimation.AttackIdle01);

        if ( PlayerChars[CurPlayerCharIndex].Guardian ) {
			PlayerChars[CurPlayerCharIndex].Guardian.SetInitialPosition( StartPlayerPos - PlayerChars[CurPlayerCharIndex].transform.forward, PlayerChars[CurPlayerCharIndex].transform.rotation );
		}

        OpponentChars[CurOpponentCharIndex].alwaysKinematic = false;
        OpponentChars[CurOpponentCharIndex].SetInitialPosition(StartOpponentPos, Quaternion.Euler(0.0f, -90.0f, 0.0f));
        OpponentChars[CurOpponentCharIndex].SetGroundedRigidBody();
        OpponentChars[CurOpponentCharIndex].PlayAni(eAnimation.AttackIdle01);

		if ( OpponentChars[CurOpponentCharIndex].Guardian ) {
			OpponentChars[CurOpponentCharIndex].Guardian.SetInitialPosition( StartOpponentPos - OpponentChars[CurOpponentCharIndex].transform.forward, OpponentChars[CurOpponentCharIndex].transform.rotation );
		}

        GameUIManager.Instance.HideUI("ArenaVsPanel", true);
        Invoke("ShowPlayUI", mUIPrepareCloseTime);
    }

    private void ShowPlayUI()
    {
        ProcessingEnd = false;

        FComponent comp = GameUIManager.Instance.ShowUI("ArenaPlayPanel", true);
        mUIPlayOpenTime = comp.GetOpenAniTime();
        mUIPlayCloseTime = comp.GetCloseAniTime();

        RemainTime = GameInfo.Instance.BattleConfig.ArenaTime;
        UIPVP.UpdateTimer(RemainTime);

        PlayerChars[CurPlayerCharIndex].OnGameStart();
        PlayerChars[CurPlayerCharIndex].OnMissionStart();

        OpponentChars[CurOpponentCharIndex].OnGameStart();
        OpponentChars[CurOpponentCharIndex].OnMissionStart();

		Invoke("StartBattle", mUIPlayOpenTime);
    }

	private void StartBattle() {
		IsBattleStart = true;
		IsAnyPlayerDead = false;

		UIPVP.OnTabGameSpeed( GameInfo.Instance.PVPGameSpeedType, SelectEvent.Click );
		Time.timeScale = GameSpeed;

		StopCoroutine( "UpdateTimer" );
		StartCoroutine( "UpdateTimer" );

		PlayerChars[CurPlayerCharIndex].StartBT();
		OpponentChars[CurOpponentCharIndex].StartBT();

        if ( PlayerChars[CurPlayerCharIndex].Guardian ) {
            Utility.SetLayer( PlayerChars[CurPlayerCharIndex].Guardian.gameObject, (int)eLayer.PlayerClone, true );
			PlayerChars[CurPlayerCharIndex].Guardian.StartBT();
		}

		if ( OpponentChars[CurOpponentCharIndex].Guardian ) {
			Utility.SetLayer( OpponentChars[CurOpponentCharIndex].Guardian.gameObject, (int)eLayer.EnemyClone, true );
			OpponentChars[CurOpponentCharIndex].Guardian.StartBT();
		}

		PlayerChars[CurPlayerCharIndex].ExecuteBattleOption( BattleOption.eBOTimingType.OnEnemyAppear, 0, null );
		OpponentChars[CurOpponentCharIndex].ExecuteBattleOption( BattleOption.eBOTimingType.OnEnemyAppear, 0, null );

		mPlayerTagObj.SetActive( true );
	}

	private IEnumerator UpdateTimer()
    {
        float delta = 1.0f;
        
        WaitForSeconds waitForSeconds = new WaitForSeconds(delta);
        while(RemainTime > 0.0f)
        {
            if (!Director.IsPlaying)
            {
                if (EnableDamage)
                {
                    RemainTime -= delta;
                }

                UIPVP.UpdateTimer(RemainTime);

                TotalGameTime += delta;
            }

            yield return waitForSeconds;
        }

        RemainTime = 0.0f;
        UIPVP.UpdateTimer(RemainTime);

        Player player = PlayerChars[CurPlayerCharIndex];
        player.StopBT();
        player.HideAllClone();
        player.actionSystem.CancelCurrentAction();

        if ( player.Guardian ) {
			player.Guardian.StopBT();
			player.Guardian.actionSystem.CancelCurrentAction();
		}
        
        Player opponent = OpponentChars[CurOpponentCharIndex];
        opponent.StopBT();
        opponent.HideAllClone();
        opponent.actionSystem.CancelCurrentAction();

		if ( opponent.Guardian ) {
			opponent.Guardian.StopBT();
			opponent.Guardian.actionSystem.CancelCurrentAction();
		}

        Player loser = null;
        Player winner = null;

        if ((player.curHp / player.maxHp) < (opponent.curHp / opponent.maxHp))
        {
            loser = player;
            winner = opponent;
        }
        else // hp가 같으면 내가 이기는 걸로
        {
            loser = opponent;
            winner = player;
        }

        loser.SetHp(0.0f);
        loser.PlayAni(eAnimation.Die);

        ProcessingEnd = true;
        EndGame(eEventType.EVENT_GAME_PLAYER_DEAD, null);

        /*
        AniEvent.sEvent evt = loser.aniEvent.GetFirstAttackEvent(eAnimation.Attack01);
        evt.behaviour = eBehaviour.Attack;
        eHitState hitState = eHitState.Success;

        bool isCritical = false;
        loser.OnHit(winner, BattleOption.eToExecuteType.Unit, evt, loser.curHp, ref isCritical, ref hitState, null, false);
        */
    }

	private void CheckEndBattle() {
		GuardianIndex = -1;

        IsBattleStart = false;

		PlayerChars[CurPlayerCharIndex].actionSystem.CancelCurrentAction();
		PlayerChars[CurPlayerCharIndex].DeactivateAllPlayerMinion();
		PlayerChars[CurPlayerCharIndex].Deactivate();

		OpponentChars[CurOpponentCharIndex].actionSystem.CancelCurrentAction();
		OpponentChars[CurOpponentCharIndex].DeactivateAllPlayerMinion();
		OpponentChars[CurOpponentCharIndex].Deactivate();

		bool isWin = false;
		if( !mbSurrender ) {
			if( OpponentChars[CurOpponentCharIndex].curHp <= 0.0f ) {
				mBeforeOpponentCharIndex = CurOpponentCharIndex;
				CurOpponentCharIndex = GetNextCharIndex( OpponentChars, CurOpponentCharIndex );

				if( CurOpponentCharIndex >= (int)eArenaTeamSlotPos._MAX_ ) {
					isWin = true;
					IsEndGame = true;
				}
				else {
					mBeforePlayerCharIndex = -1;

					if( PlayerChars[CurPlayerCharIndex].curHp <= 0.0f ) {
						PlayerChars[CurPlayerCharIndex].SetHp( 1.0f );
					}

					PlayerChars[CurPlayerCharIndex].AddHpPerCost( BattleOption.eToExecuteType.Unit, mPlayerAddHpRate, false );
				}
			}
			else {
				mBeforePlayerCharIndex = CurPlayerCharIndex;
				CurPlayerCharIndex = GetNextCharIndex( PlayerChars, CurPlayerCharIndex );

				if( CurPlayerCharIndex >= (int)eArenaTeamSlotPos._MAX_ ) {
					IsEndGame = true;
				}
				else {
					mBeforeOpponentCharIndex = -1;

					if( OpponentChars[CurOpponentCharIndex].curHp <= 0.0f ) {
						OpponentChars[CurOpponentCharIndex].SetHp( 1.0f );
					}

					OpponentChars[CurOpponentCharIndex].AddHpPerCost( BattleOption.eToExecuteType.Unit, mOpponentAddHpRate, false );
				}
			}
		}
		else {
			IsEndGame = true;
			isWin = false;
		}

		UIPVP.HideBattleResult();

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        if ( !IsEndGame ) {
			GameUIManager.Instance.HideUI( "ArenaPlayPanel", true );
			Invoke( "PrepareBattle", 1.0f );
		}
		else {
			MessagePopup.ClosePopup();
			ProcessingEnd = true;

			Time.timeScale = 1.0f;
			GameUIManager.Instance.HideUI( "ArenaPlayPanel", false );

			for( int i = 0; i < (int)eArenaTeamSlotPos._MAX_; i++ ) {
				if( PlayerChars[i] == null ) {
					continue;
				}

				Utility.SetLayer( PlayerChars[i].gameObject, mPlayerCharsRenderTexLayer[i], true );
                if ( PlayerChars[i].Guardian ) {
					Utility.SetLayer( PlayerChars[i].Guardian.gameObject, (int)eLayer.RenderTargetWeapon, true );
				}
			}

			InGameCamera.ExcludCullingMask( (int)eLayer.Player );
			InGameCamera.ExcludCullingMask( (int)eLayer.Enemy );

            bool isHaveGuardianChar = false;
			float rotY = isWin ? 180.0f : -110.0f;
			for( int i = 0; i < (int)eArenaTeamSlotPos._MAX_; i++ ) {
				if( PlayerChars[i] ) {
					PlayerChars[i].actionSystem.CancelCurrentAction();
                    
                    PlayerChars[i].Activate();
					PlayerChars[i].SetKinematicRigidBody();
					PlayerChars[i].SetInitialPosition( Vector3.zero, Quaternion.Euler( 0.0f, 180.0f, 0.0f ) );
                    PlayerChars[i].costumeUnit.ChangeAllWeaponName( PlayerChars[i].ChangeWeaponName );

					if ( PlayerChars[i].Guardian ) {
						GuardianIndex = i;
						PlayerChars[i].Guardian.Activate();
						PlayerChars[i].Guardian.MainCollider.Enable( false );
						PlayerChars[i].Guardian.SetInitialPosition( Vector3.zero + Vector3.forward, Quaternion.Euler( 0.0f, 180.0f, 0.0f ) );
					}


                    CamRenderTex[i].transform.SetParent( PlayerChars[i].transform );
					CamRenderTex[i].orthographicSize = PlayerChars[i].PVPRenderTexCamSize;

					float x = 0.0f;
					if( isWin && i == 2 ) {
						x = 0.1f;
					}
					else if( !isWin ) {
						x = 2.5f;
					}

					Vector3 v = new Vector3(x, 1.1f, 1.2f);
					if( PlayerChars[i].tableId == 8 || PlayerChars[i].tableId == 15 ) { // 이거 짜증나네 시간날때 정리 한번 해야됨
						v = -PlayerChars[i].transform.forward * 1.5f;
						v.y = 1.3f;
						Utility.InitTransform( CamRenderTex[i].gameObject, v, Quaternion.Euler( 0.0f, 180.0f, 0.0f ), Vector3.one );
					}
					else if( PlayerChars[i].tableId == 10 || PlayerChars[i].tableId == 12 ) {
						v = new Vector3( 0.3f, 1.1f, 1.2f );
						Utility.InitTransform( CamRenderTex[i].gameObject, v, Quaternion.Euler( 0.0f, -165.0f, 0.0f ), Vector3.one );
					}
					else if( PlayerChars[i].tableId == 16 ) {
						if( !isWin ) {
							Utility.InitTransform( CamRenderTex[i].gameObject, new Vector3( -1.3f, 1.0f, 1.2f ), Quaternion.Euler( 0.0f, 130.0f, 0.0f ), Vector3.one );
						}
						else {
							Utility.InitTransform( CamRenderTex[i].gameObject, new Vector3( -0.05f, 1.0f, 1.2f ), Quaternion.Euler( 0.0f, 180.0f, 0.0f ), Vector3.one );
						}
					}
					else if( PlayerChars[i].tableId == 18 || PlayerChars[i].tableId == 19 || PlayerChars[i].tableId == 25 || PlayerChars[i].tableId == 26 ) {
						v = -PlayerChars[i].transform.forward * 1.5f;
						v.y = 1.1f;
                        v.z = 1.2f;
                        Utility.InitTransform( CamRenderTex[i].gameObject, v, Quaternion.Euler( 0.0f, 180.0f, 0.0f ), Vector3.one );

						if ( PlayerChars[i].tableId == 25 ) {
							PlayerChars[i].aniEvent.Rebind();
						}
					}
					else if( PlayerChars[i].tableId == 21 || PlayerChars[i].tableId == 23 ) {
						v = -PlayerChars[i].transform.forward * 1.5f;
						v.y = 1.0f;
                        v.z = 1.2f;
						Utility.InitTransform( CamRenderTex[i].gameObject, v, Quaternion.Euler( 0.0f, 180.0f, 0.0f ), Vector3.one );
					}
					else {
						Utility.InitTransform( CamRenderTex[i].gameObject, new Vector3( x, 1.1f, 1.2f ), Quaternion.Euler( 0.0f, rotY, 0.0f ), Vector3.one );
					}

					CamRenderTex[i].gameObject.SetActive( true );

					if ( PlayerChars[i].Guardian && (int)eArenaTeamSlotPos.GUARDIAN < CamRenderTex.Length ) {
						isHaveGuardianChar = true;
						CamRenderTex[(int)eArenaTeamSlotPos.GUARDIAN].transform.SetParent( PlayerChars[i].Guardian.transform );
						CamRenderTex[(int)eArenaTeamSlotPos.GUARDIAN].orthographicSize = PlayerChars[i].PVPRenderTexCamSize;

						Utility.InitTransform( CamRenderTex[(int)eArenaTeamSlotPos.GUARDIAN].gameObject, CamRenderTex[i].transform.localPosition, CamRenderTex[i].transform.localRotation, CamRenderTex[i].transform.localScale );
					}
				}
				else {
					CamRenderTex[i].gameObject.SetActive( false );
				}

				if( OpponentChars[i] ) {
					OpponentChars[i].actionSystem.CancelCurrentAction();
				}
			}

			if ( (int)eArenaTeamSlotPos.GUARDIAN < CamRenderTex.Length ) {
				CamRenderTex[(int)eArenaTeamSlotPos.GUARDIAN].gameObject.SetActive( isHaveGuardianChar );
			}

			Utility.StopCoroutine( this, ref mCrTimer );

			Result = 0;
			for( int i = 0; i < PlayerChars.Length; i++ ) {
				if( PlayerChars[i] == null ) {
					continue;
				}

				PlayerChars[i].EnableBoneFace();
				if( isWin ) {
					PlayerChars[i].PlayAni( eAnimation.ArenaWin );
                    if ( PlayerChars[i].Guardian ) {
						PlayerChars[i].Guardian.PlayAni( eAnimation.ArenaWin );
					}

					if( PlayerChars[i].curHp > 0.0f && Result == 0 ) {
						Result = (byte)( i + 1 );
					}
				}
				else {
					PlayerChars[i].PlayAni( eAnimation.Stun );
					if ( PlayerChars[i].Guardian ) {
						PlayerChars[i].Guardian.PlayAni( eAnimation.Stun );
					}
				}
			}

			if( !AsTestScene ) {
				if( !IsFriendPVP ) {
					System.UInt32 maxDamage = 0;
					for( int i = 0; i < PlayerChars.Length; i++ ) {
						if( PlayerChars[i] == null ) {
							continue;
						}

						if( PlayerChars[i].MaxAttackPower > maxDamage ) {
							maxDamage = (System.UInt32)PlayerChars[i].MaxAttackPower;
						}
					}

					GameInfo.Instance.Send_ReqArenaGameEnd( Result, (uint)( TotalGameTime * 1000.0f ), IsFriendPVP,
						                                    (System.UInt32)mOpponentTeamData.Score, maxDamage, ShowBattleResult );
				}
				else {
					ShowBattleResult( 0, null );
				}
			}
			else {
				ParamCurrentGrade = ParamBeforeGrade;

				if( Result > 0 ) {
					StreakCount = 324;
					IsNewRecord = true;
				}

				PlayResultVoice();
				GameUIManager.Instance.ShowUI( "ArenaResultPopup", false );
			}
		}
	}

	private int GetNextCharIndex(Player[] chars, int curIndex)
    {
        ++curIndex;
        for(int i = curIndex; i < chars.Length; i++)
        {
            if(chars[i] == null)
            {
                continue;
            }

            curIndex = i;
            break;
        }

        return curIndex;
    }

    private void ShowBattleResult(int result, PktMsgType pkt)
    {
        if(result != 0)
        {
            Debug.LogError("ShowBattleResult 실패");
            return;
        }

        if (!IsFriendPVP)
        {
            PktInfoArenaGameEndAck pktArenaEnd = pkt as PktInfoArenaGameEndAck;
            if (pktArenaEnd == null)
            {
                Debug.LogError("아레나 종료 패킷이 PktInfoArenaGameEndAck 타입이 아님");
                return;
            }

            PlayResultVoice();

            //GameinfoRecv.RecvAckArenaGameEnd에서 처리됨, GameInfo.Instance.UserData 데이터는 GameInfo나 Recv 패킷에서만 처리
            //GameInfo.Instance.UserData.BPRemainTime = pktArenaEnd.BPNextTime_.GetTime();

            AddGrade = GameInfo.Instance.UserBattleData.Now_Score - AddGrade;
            AddCoin = GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.BATTLECOIN] - AddCoin;

            ParamCurrentGrade = GameInfo.Instance.GameTable.FindArenaGrade((int)pktArenaEnd.record_.nowGradeID_);
            int grade = ParamCurrentGrade.Grade;

            if (PromoteState == ePromoteState.Testing && grade != CurrentGrade)
            {
                PromoteState = grade > CurrentGrade ? ePromoteState.Promotion : ePromoteState.Failure;
            }

            StreakCount = pktArenaEnd.record_.nowWinLoseCnt_;
            IsNewRecord = pktArenaEnd.record_.nowWinLoseCnt_ > mBeforeStreak;
        }
        else
        {
            PlayResultVoice();

            AddGrade = 0;
            AddCoin = 0;

            ParamCurrentGrade = GameInfo.Instance.GameTable.FindArenaGrade(GameInfo.Instance.UserBattleData.Now_GradeId);
            PromoteState = ePromoteState.None;
            StreakCount = 0;
            IsNewRecord = false;
        }

        GameUIManager.Instance.ShowUI("ArenaResultPopup", false);
    }

    private void PlayResultVoice()
    {
        mbPlayingVoice = true;

        if (Result > 0)
        {
            int charIndex = Result - 1;
            VoiceMgr.Instance.PlayChar(eVOICECHAR.ArenaWin, PlayerChars[charIndex].tableId, PlayerChars[charIndex].PVPWinCount);
        }
        else
        {
            VoiceMgr.Instance.PlayChar(eVOICECHAR.Giveup, PlayerChars[PlayerChars.Length - 1].tableId, -1);
        }
    }

    private float AudioSpectrumData()
    {
        AudioSource audioSrc = SoundManager.Instance.GetAudioSource(SoundManager.eSoundType.Voice);
        if (audioSrc == null)
        {
            return 0.0f;
        }

        audioSrc.GetSpectrumData(mAudioSamples, 0, FFTWindow.Rectangular);
        return Mathf.Max(mAudioSamples);
    }

    private void UpdatePlayerMouth()
    {
        if (Player.aniEvent.aniFace == null)
        {
            return;
        }

        float spectrumData = AudioSpectrumData();

        float a = Player.aniEvent.aniFace.GetLayerWeight(1);
        float b = Mathf.Clamp(spectrumData * 10, 0, 1);

        Player.aniEvent.aniFace.SetLayerWeight(1, Mathf.Lerp(a, b, 0.4f));
    }
}
