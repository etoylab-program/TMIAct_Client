using UnityEngine;
using System.Collections;

public class UIMailListSlot : FSlot {


    public UIRewardListSlot kRewardListSlot;
	public UISprite kbgSpr;
	public UILabel kItemNameLabel;
	public UILabel ktextLabel;
	public UILabel kTimeLabel;
	public UIButton kConfirmBtn;

    private ulong m_mailUID = 0;

	public void UpdateSlot(MailData mailData) 	//Fill parameter if you need
	{
        if (mailData == null)
            return;
        
        m_mailUID = mailData.MailUID;
        
        RewardData product = new RewardData(mailData.ProductType,(int)mailData.ProductIndex, (int)mailData.ProductValue);
        //  메일 타이틀 글
        kItemNameLabel.textlocalize = GameSupport.GetProductName(product);
        //  메일 설명 글
        SetMailText(mailData.MailType,mailData.MailTypeValue);
        //  메일 삭제까지 남은 일수
        SetRemainTime(mailData.RemainTime);
        //  보상 아이콘
        kRewardListSlot.UpdateSlot(product, true);
    }
 	
    /// <summary>
    ///  해당 메일 수령 버튼
    /// </summary>
	public void OnClick_ConfirmBtn()
	{
        UIMailBoxPopup mailBox = LobbyUIManager.Instance.GetActiveUI<UIMailBoxPopup>("MailBoxPopup");
        if (mailBox == null)
            return;

        mailBox.ReciveMailStart(m_mailUID);

    }

    /// <summary>
    ///  메일 타입별 텍스트
    /// </summary>
    public void SetMailText(eMailType TypeID, string Value)
    {
        Log.Show(TypeID);
        switch(TypeID)
        {
            case eMailType.Rank: // 랭크 달성 보상
            case eMailType.Login: // 로그인 보상
            case eMailType.Pass_N: // 패스 일반 보상
            case eMailType.Pass_S: // 패스 특별 보상
            case eMailType.MonthlyFee:
            case eMailType.MonthlyFee_P:
			case eMailType.LoginEvent:
            case eMailType.RankPass_N:
            case eMailType.RankPass_S:
                {
                    ktextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.MAIL_TYPE_TEXT_START + (int)TypeID), Value);
                }
                break;
            case eMailType.StoryPass_N:
            case eMailType.StoryPass_S:
                {
                    int.TryParse(Value, out int result);
                    GameTable.Stage.Param param = 
                        GameInfo.Instance.GameTable.FindStage(x => x.StageType == (int) eSTAGETYPE.STAGE_MAIN_STORY && x.ID == result);
                    if (param != null)
                    {
                        ktextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.MAIL_TYPE_TEXT_START + (int)TypeID), param.Chapter);
                    }
                }
                break;
            case eMailType.Weekly: // 주간 미션 보상
            case eMailType.InitReward: // 초기 지급 보상
            case eMailType.ArenaReward: // 아레나 보상
                {
                    ktextLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.MAIL_TYPE_TEXT_START + (int)TypeID);
                }
                break;
            case eMailType.Message:
                {
                    ktextLabel.textlocalize = Value;
                }
                break;
            case eMailType.StringNumber:
                {
                    ktextLabel.textlocalize = FLocalizeString.Instance.GetText(int.Parse(Value));
                }
                break;
            case eMailType.LoginMonthly: {
				ktextLabel.textlocalize = FLocalizeString.Instance.GetText( 3392, int.Parse( Value ) );
			}
            break;

		}
    }

    /// <summary>
    ///  메일 삭제까지 남은 일수
    /// </summary>
    public void SetRemainTime(System.DateTime Time)
    {
        System.DateTime curTime = GameSupport.GetCurrentServerTime();
        string remainTime = GameSupport.GetRemainTimeString_DayAndHours(Time, curTime);     //시간 or 일

        FLocalizeString.SetLabel(kTimeLabel, 1261, remainTime);
    }
}
