
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIArenaResultPopup : FComponent
{
    [Serializable]
    public class sResultInfo
    {
        public string       AniName;
        public UILabel      LbResult;
        public UISprite     SprGrade;
        public UILabel      LbGrade;
        public UILabel      LbGradeAdd;
        public UILabel      LbCoin;
        public UILabel      LbCoinAdd;
        public UISprite[]   SprResultIcon;
        public UITexture[]  TexRender;
        public GameObject   WinObj;
        public GameObject   DefeatObj;
        public Animation    AniNewRecord;
        public UILabel      LbStreak;
        public UISprite     SprScoreBuff;
        public UISprite     SprCoinBuff;


        public void Init(Player[] playerChars, int guardianIndex)
        {
            for (int i = 0; i < SprResultIcon.Length; i++)
            {
                SprResultIcon[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < playerChars.Length; i++)
            {
                if (playerChars[i] == null)
                {
                    TexRender[i].mainTexture = null;
                }
            }

            if ( 0 <= guardianIndex ) {
                TexRender[(int)eArenaTeamSlotPos.GUARDIAN].transform.localPosition = TexRender[guardianIndex].transform.localPosition;
			}

            if (WinObj)
            {
                WinObj.SetActive(false);
            }

            if (DefeatObj)
            {
                DefeatObj.SetActive(false);
            }

            if (AniNewRecord)
            {
                AniNewRecord.Stop();
                AniNewRecord.gameObject.SetActive(false);
            }
        }
    }


    [Header("[Property]")]
    public sResultInfo  ResultSuccess;
    public sResultInfo  ResultDefeat;
    public sResultInfo  ResultPromotion;
    public GameObject   FriendPVPSuccess;
    public UITexture[]  TexFriendPVPSuccess;
    public GameObject   FriendPVPLose;
    public UITexture[]  TexFriendPVPLose;

    public bool PossibleToInput { get; set; } = false;

    private WorldPVP mWorldPVP = null;


    public override void OnEnable()
    {
        base.OnEnable();
        InitComponent();
    }

	public override void InitComponent() {
		base.InitComponent();

		if ( mWorldPVP == null ) {
			mWorldPVP = World.Instance as WorldPVP;
		}

		if ( !mWorldPVP.IsFriendPVP ) {
			ResultSuccess.Init( mWorldPVP.PlayerChars, mWorldPVP.GuardianIndex );
			ResultDefeat.Init( mWorldPVP.PlayerChars, mWorldPVP.GuardianIndex );
			ResultPromotion.Init( mWorldPVP.PlayerChars, mWorldPVP.GuardianIndex );

			PossibleToInput = true;

			if ( mWorldPVP.PromoteState != WorldPVP.ePromoteState.None ) {
				ShowPromotion();
			}
			else {
				if ( mWorldPVP.Result > 0 ) {
					ShowSuccess();
				}
				else {
					ShowDefeat();
				}
			}
		}
		else {
			for ( int i = 0; i < mWorldPVP.PlayerChars.Length; i++ ) {
				if ( mWorldPVP.PlayerChars[i] == null ) {
					TexFriendPVPSuccess[i].mainTexture = null;
					TexFriendPVPLose[i].mainTexture = null;
				}
			}

			if ( mWorldPVP.Result > 0 ) {
				FriendPVPSuccess.gameObject.SetActive( true );
			}
			else {
				FriendPVPLose.gameObject.SetActive( true );
			}

			if ( 0 <= mWorldPVP.GuardianIndex && (int)eArenaTeamSlotPos.GUARDIAN < TexFriendPVPSuccess.Length && (int)eArenaTeamSlotPos.GUARDIAN < TexFriendPVPLose.Length ) {
				TexFriendPVPSuccess[(int)eArenaTeamSlotPos.GUARDIAN].transform.localPosition = TexFriendPVPSuccess[mWorldPVP.GuardianIndex].transform.localPosition;
				TexFriendPVPLose[(int)eArenaTeamSlotPos.GUARDIAN].transform.localPosition = TexFriendPVPLose[mWorldPVP.GuardianIndex].transform.localPosition;
			}
		}
	}

	public void OnBtnClose()
    {
        mWorldPVP.ToLobby();
    }

    private void ShowSuccess()
    {
        if(mWorldPVP.IsNewRecord)
        {
            ResultSuccess.AniNewRecord.gameObject.SetActive(true);
            ResultSuccess.AniNewRecord.Play();
        }

        ResultSuccess.LbStreak.textlocalize = string.Format(FLocalizeString.Instance.GetText(1497), mWorldPVP.StreakCount);

        ResultSuccess.LbResult.textlocalize = FLocalizeString.Instance.GetText(1442);
        ResultSuccess.SprGrade.spriteName = mWorldPVP.ParamCurrentGrade.Icon;
        SetGradeAndCoin(ResultSuccess);
        
        SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, 3000, 1.0f);
        PlayUIAni(ResultSuccess, false);
    }

    private void ShowDefeat()
    {
        ResultDefeat.LbResult.textlocalize = FLocalizeString.Instance.GetText(1477);
        ResultDefeat.SprGrade.spriteName = mWorldPVP.ParamCurrentGrade.Icon;
        SetGradeAndCoin(ResultDefeat);

        SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, 3001, 1.0f);
        PlayUIAni(ResultDefeat, false);
    }

    private void ShowPromotion()
    {
        if (mWorldPVP.Result > 0)
        {
            SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, 3000, 1.0f);

            ResultPromotion.LbResult.textlocalize = FLocalizeString.Instance.GetText(1478);
            ResultPromotion.WinObj.SetActive(true);
        }
        else
        {
            SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, 3001, 1.0f);

            ResultPromotion.LbResult.textlocalize = FLocalizeString.Instance.GetText(1479);
            ResultPromotion.DefeatObj.SetActive(true);
        }

        SetGradeAndCoin(ResultPromotion);

        int winCount = 0;
        int defeatCount = 0;
        List<eArenaGradeUpFlag> list = GameSupport.IsArenaGradeUpWinLoseFlags();
        for(int i = 0; i < list.Count; i++)
        {
            ResultPromotion.SprResultIcon[i].gameObject.SetActive(true);

            if (list[i] == eArenaGradeUpFlag.WIN)
            {
                ResultPromotion.SprResultIcon[i].spriteName = "Arena_Slot_01";
                ++winCount;
            }
            else if(list[i] == eArenaGradeUpFlag.LOSE)
            {
                if (winCount >= 2 || defeatCount >= 2)
                {
                    ResultPromotion.SprResultIcon[i].gameObject.SetActive(false);
                }
                else
                {
                    ResultPromotion.SprResultIcon[i].spriteName = "Arena_Slot_02";
                    ++defeatCount;
                }
            }
            else
            {
                ResultPromotion.SprResultIcon[i].gameObject.SetActive(false);
            }
        }

        bool showPromotionPopup = mWorldPVP.PromoteState == WorldPVP.ePromoteState.Promotion ? true : false;
        PlayUIAni(ResultPromotion, showPromotionPopup);
    }

    private void SetGradeAndCoin(sResultInfo resultInfo)
    {
        if (resultInfo.LbGrade)
        {
            resultInfo.LbGrade.textlocalize = GameInfo.Instance.UserBattleData.Now_Score.ToString("#,##0");

            if(mWorldPVP.AddGrade > 0)
            {
                resultInfo.LbGradeAdd.color = Color.green;
                resultInfo.LbGradeAdd.textlocalize = string.Format("+{0}", mWorldPVP.AddGrade.ToString("#,##0"));
            }
            else if(mWorldPVP.AddGrade < 0)
            {
                resultInfo.LbGradeAdd.color = Color.red;
                resultInfo.LbGradeAdd.textlocalize = string.Format("{0}", mWorldPVP.AddGrade.ToString("#,##0"));
            }
            else
            {
                resultInfo.LbGradeAdd.color = Color.white;
                resultInfo.LbGradeAdd.textlocalize = string.Format("-", mWorldPVP.AddGrade.ToString("#,##0"));
            }

            if(resultInfo.SprScoreBuff)
            {
                resultInfo.SprScoreBuff.gameObject.SetActive(GameInfo.Instance.ArenaATK_Buff_Flag || GameInfo.Instance.ArenaDEF_Buff_Flag);
            }
        }

        resultInfo.LbCoin.textlocalize = GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.BATTLECOIN].ToString("#,##0");

        if (mWorldPVP.AddCoin > 0)
        {
            resultInfo.LbCoinAdd.color = Color.green;
            resultInfo.LbCoinAdd.textlocalize = string.Format("+{0}", mWorldPVP.AddCoin.ToString());
        }
        else if (mWorldPVP.AddCoin < 0)
        {
            resultInfo.LbCoinAdd.color = Color.red;
            resultInfo.LbCoinAdd.textlocalize = string.Format("{0}", mWorldPVP.AddCoin.ToString());
        }
        else
        {
            resultInfo.LbCoinAdd.color = Color.white;
            resultInfo.LbCoinAdd.textlocalize = string.Format("-", mWorldPVP.AddCoin.ToString());
        }

        if (resultInfo.SprCoinBuff)
        {
            resultInfo.SprCoinBuff.gameObject.SetActive(GameInfo.Instance.ArenaGold_Buff_Flag);
        }
    }

    private void PlayUIAni(sResultInfo resultInfo, bool showPromotionPopup)
    {
        UIAni.Play(resultInfo.AniName);
        StartCoroutine(WaitInput(UIAni[resultInfo.AniName].length, showPromotionPopup));
    }

    private IEnumerator WaitInput(float aniLength, bool showPromotionPopup)
    {
        yield return new WaitForSeconds(aniLength);

        if(showPromotionPopup)
        {
            FComponent comp = GameUIManager.Instance.ShowUI("ArenaGradeUpPopup", true);
            PossibleToInput = false;

            yield return new WaitForSeconds(comp.GetOpenAniTime());
        }

        /*
        while(true)
        {
            if (PossibleToInput && Input.GetMouseButton(0))
            {
                mWorldPVP.ToLobby();
                yield break;
            }

            yield return null;
        }
        */
    }
}
