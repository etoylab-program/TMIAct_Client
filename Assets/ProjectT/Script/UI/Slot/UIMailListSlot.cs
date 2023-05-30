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
        //  ���� Ÿ��Ʋ ��
        kItemNameLabel.textlocalize = GameSupport.GetProductName(product);
        //  ���� ���� ��
        SetMailText(mailData.MailType,mailData.MailTypeValue);
        //  ���� �������� ���� �ϼ�
        SetRemainTime(mailData.RemainTime);
        //  ���� ������
        kRewardListSlot.UpdateSlot(product, true);
    }
 	
    /// <summary>
    ///  �ش� ���� ���� ��ư
    /// </summary>
	public void OnClick_ConfirmBtn()
	{
        UIMailBoxPopup mailBox = LobbyUIManager.Instance.GetActiveUI<UIMailBoxPopup>("MailBoxPopup");
        if (mailBox == null)
            return;

        mailBox.ReciveMailStart(m_mailUID);

    }

    /// <summary>
    ///  ���� Ÿ�Ժ� �ؽ�Ʈ
    /// </summary>
    public void SetMailText(eMailType TypeID, string Value)
    {
        Log.Show(TypeID);
        switch(TypeID)
        {
            case eMailType.Rank: // ��ũ �޼� ����
            case eMailType.Login: // �α��� ����
            case eMailType.Pass_N: // �н� �Ϲ� ����
            case eMailType.Pass_S: // �н� Ư�� ����
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
            case eMailType.Weekly: // �ְ� �̼� ����
            case eMailType.InitReward: // �ʱ� ���� ����
            case eMailType.ArenaReward: // �Ʒ��� ����
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
    ///  ���� �������� ���� �ϼ�
    /// </summary>
    public void SetRemainTime(System.DateTime Time)
    {
        System.DateTime curTime = GameSupport.GetCurrentServerTime();
        string remainTime = GameSupport.GetRemainTimeString_DayAndHours(Time, curTime);     //�ð� or ��

        FLocalizeString.SetLabel(kTimeLabel, 1261, remainTime);
    }
}
