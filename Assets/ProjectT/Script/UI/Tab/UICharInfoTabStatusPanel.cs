using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharInfoTabStatusPanel : FComponent
{
	public UILabel kNameLabel;
    public UILabel kCombatPowerLabel;
    //public UISprite kMainCharSpr;
	public UIButton kMainCharBtn;
	public UILabel kLevelLabel;
    public UISprite kGradeSpr;
	public UIButton kGradeUpBtn;
    public UIGaugeUnit kGaugeUnit;
    public List<UIStatusUnit> kStatusUnitList;

    public UIButton BtnAwaken;
    public UIButton BtnAwakenSkill;

    public UISprite SprChangeMainCharDisable;
    public UISprite SprLobbyBgCharSlot;

    [Header("[Type]")]
    public UISprite SprAttrType;
    public UILabel  LbAttrType;
    public UISprite SprUnitType;
    public UILabel  LbUnitType;

    private CharData _chardata;


	public override void OnEnable()
	{
		Type = TYPE.Tab;

		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        if( RenderTargetChar.Instance.RenderPlayer == null ) {
            return;
        }
         
        RenderTargetChar.Instance.ShowAttachedObject( false );
        RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject( RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.InGameOnly, false );
        RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject( RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.LobbyOnly, true );
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        //  InitComponent 에 들어오지 않고 Renewal로 들어오는 경우 발생
        //  데이터 업데이트도 여기서 합니다.
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
        int tableid = (int)UIValue.Instance.GetValue(UIValue.EParamType.CharSelTableID);
        _chardata = GameInfo.Instance.GetCharData(uid);

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_chardata.TableData.Name);

        kMainCharBtn.gameObject.SetActive(true);
        SprChangeMainCharDisable.gameObject.SetActive(false);

        if (_chardata.CUID == GameInfo.Instance.UserData.MainCharUID)
        {
            //kMainCharSpr.gameObject.SetActive(true);
            //kMainCharBtn.gameObject.SetActive(false);

            SprChangeMainCharDisable.gameObject.SetActive(true);
        }
        else
        {
            //kMainCharSpr.gameObject.SetActive(false);
            //kMainCharBtn.gameObject.SetActive(true);
        }

        kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _chardata.Level, GameSupport.GetCharMaxLevel(_chardata.Grade));

        kGradeSpr.spriteName = string.Format("grade_{0}", _chardata.Grade.ToString("D2"));  //"grade_0" + _chardata.Grade.ToString();
        kGradeSpr.MakePixelPerfect();
      
        //경험치
        float fillAmount = GameSupport.GetCharacterLevelExpGauge(_chardata.Level, _chardata.Grade, _chardata.Exp);
        kGaugeUnit.InitGaugeUnit(fillAmount);

        if (GameSupport.IsMaxCharLevel(_chardata.Level, _chardata.Grade))
            kGaugeUnit.SetText(FLocalizeString.Instance.GetText(221));
        else
            kGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT), (fillAmount * 100.0f)));

        string strhp = "";
        string stratk = "";
        string strdef = "";
        string strcri = "";
        int chp = _chardata.GetCharHP();
        int catk = _chardata.GetCharATK();
        int cdef = _chardata.GetCharDEF();
        int ccri = _chardata.GetCharCRI();
        int thp = GameSupport.GetTotalHP(_chardata, eWeaponSlot.MAIN);
        int tatk = GameSupport.GetTotalATK(_chardata, eWeaponSlot.MAIN);
        int tdef = GameSupport.GetTotalDEF(_chardata, eWeaponSlot.MAIN);
        int tcri = GameSupport.GetTotalCRI(_chardata, eWeaponSlot.MAIN);
        
        if(thp != chp)
            strhp = string.Format("({0:#,##0}+{1:#,##0})", chp, thp - chp);

        if (tatk != catk)
            stratk = string.Format("({0:#,##0}+{1:#,##0})", catk, tatk - catk);

        if (tdef != cdef)
            strdef = string.Format("({0:#,##0}+{1:#,##0})", cdef, tdef - cdef);

        if (tcri != ccri)
            strcri = string.Format("({0:#,##0}+{1:#,##0})", ccri, tcri - ccri);
 
        kStatusUnitList[0].InitStatusUnit((int)eTEXTID.STAT_HP, thp);
        kStatusUnitList[1].InitStatusUnit((int)eTEXTID.STAT_ATK, tatk);
        kStatusUnitList[2].InitStatusUnit((int)eTEXTID.STAT_DEF, tdef);
        kStatusUnitList[3].InitStatusUnit((int)eTEXTID.STAT_CRI, tcri);
        kStatusUnitList[0].SetAddText(strhp);
        kStatusUnitList[1].SetAddText(stratk);
        kStatusUnitList[2].SetAddText(strdef);
        kStatusUnitList[3].SetAddText(strcri);

        BtnAwaken.gameObject.SetActive(false);
        BtnAwakenSkill.gameObject.SetActive(false);

        if (_chardata.Grade >= GameInfo.Instance.GameConfig.CharStartAwakenGrade)
        {
            kGradeUpBtn.gameObject.SetActive(false);

            if (!GameSupport.IsMaxGrade(_chardata.Grade))
            {
                BtnAwaken.gameObject.SetActive(true);
            }

            if(_chardata.Grade >= GameInfo.Instance.GameConfig.CharWPStartGrade)
            {
                BtnAwakenSkill.gameObject.SetActive(true);
            }
        }
        else
        {
            if (GameSupport.IsMaxGrade(_chardata.Grade))
                kGradeUpBtn.gameObject.SetActive(false);
            else
                kGradeUpBtn.gameObject.SetActive(true);
        }

        kCombatPowerLabel.textlocalize = GameSupport.GetCombatPowerString(_chardata, eWeaponSlot.MAIN);

        SprLobbyBgCharSlot.gameObject.SetActive(false);

        int lobbyBgCharSlotIndex = GameInfo.Instance.UserData.GetLobbyBgCharSlotIndex(_chardata.CUID);
        if (lobbyBgCharSlotIndex >= 0)
        {
            SprLobbyBgCharSlot.gameObject.SetActive(true);
            SprLobbyBgCharSlot.spriteName = string.Format("ico_Mainchara_number{0}", lobbyBgCharSlotIndex + 1);
        }

        SprAttrType.spriteName = string.Format("SupporterType_{0}", _chardata.TableData.Type);
        LbAttrType.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.CARDTYPE + _chardata.TableData.Type);

        SprUnitType.spriteName = string.Format("TribeType_{0}", _chardata.TableData.MonType);
        LbUnitType.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.MON_TYPE_TEXT_START + _chardata.TableData.MonType);
    }
 	
	public void OnClick_MainCharBtn()
	{
        if (_chardata.CUID == GameInfo.Instance.GetMainCharUID())
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3023));
            return;
        }

        MessagePopup.CYN(eTEXTID.TITLE_NOTICE, 3021, eTEXTID.OK, eTEXTID.CANCEL, OnMsg_MainChar);
    }

    public void OnClick_GradeUpBtn()
    {
        LobbyUIManager.Instance.ShowUI("CharGradeUpPopup", true);
    }

    public void OnBtnAwakenSkill()
    {
        LobbyUIManager.Instance.ShowUI("CharAwakenSkillPopup", true);
    }

    public void OnMsg_MainChar()
    {
        GameInfo.Instance.Send_ReqChangeMainChar(_chardata.CUID, OnNetCharMain);
    }

    public void OnNetCharMain(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        Lobby.Instance.ChangeMainChar();

        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3022));

        VoiceMgr.Instance.PlayChar(eVOICECHAR.MainChar, _chardata.TableID);

        Renewal(true);
    }
}
