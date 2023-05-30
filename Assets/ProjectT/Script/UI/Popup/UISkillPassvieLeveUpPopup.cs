using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISkillPassvieLeveUpPopup : FComponent
{
    public GameObject kUpgrade;
    public GameObject kSelect;
    public GameObject kPoint;
    public GameObject kGold;
    public UISprite kSkillSpr;
    public UILabel kNameLabel;
    public UILabel kBefore_LevelLabel;
    public UILabel kBefore_DecsLabel;
    public UILabel kNext_LevelLabel;
    public UILabel kNext_DecsLabel;
    public UILabel kPointLabel;
	public UILabel kGoldLabel;
    public UISprite kDisableSpr;
    public UILabel kDisableLabel;
    public UIButton kUpgradeBtn;
    public UISprite kSelectSkillIconSpr;
    public UILabel kSelectNameLabel;

    //public UITexture kCharTexture;
    private int                     _sendskill;
    private UISkillLevelUpListSlot  _sendskillslot          = null;
    private UISkillSeleteListSlot   _sendskillseleteslot    = null;
    private bool                    mbLockUpgradeBtn        = false;
    private ParticleSystem          mLevelUpEff             = null; 


    public override void Awake()
	{
		base.Awake();
        mLevelUpEff = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>( "effect", "Effect/UI/prf_fx_ui_character_skill_levelup.prefab" );
        mLevelUpEff.transform.SetParent( kSkillSpr.transform );
        Utility.InitTransform( mLevelUpEff.gameObject );
    }
 
	public override void OnEnable()
	{
		_sendskill = -1;
		mbLockUpgradeBtn = false;

		base.OnEnable();
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
        int tableid = (int)UIValue.Instance.GetValue(UIValue.EParamType.CharSelTableID);
        var chardata = GameInfo.Instance.GetCharData(uid);
        if (chardata == null)
            return;

        int skillid = (int)UIValue.Instance.GetValue(UIValue.EParamType.CharSkillPassiveID);

        var tabledata = GameInfo.Instance.GameTable.FindCharacterSkillPassive(skillid);
        if (tabledata == null)
            return;

        int level = 0;
        bool isMaxLv = false;

        if ( tabledata.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_NORMAL || tabledata.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_ULTIMATE)
        {
            kUpgrade.SetActive(true);
            kSelect.SetActive(false);
            GameSupport.SetSkillSprite(ref kSkillSpr, tabledata.Atlus, tabledata.Icon);
        
            float value = 0;
            var passviedata = chardata.PassvieList.Find(x => x.SkillID == skillid);
            if (passviedata != null)
                level = passviedata.SkillLevel;

            int beforelevel = level;
            if (beforelevel >= tabledata.MaxLevel)
            {
                kBefore_LevelLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV_MAX);
                isMaxLv = true;
            }
            else
                kBefore_LevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), beforelevel);

            int nextlevel = level + 1;
            if (nextlevel >= tabledata.MaxLevel)
                kNext_LevelLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV_MAX);
            else
                kNext_LevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), nextlevel);

            if (tabledata.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_NORMAL || tabledata.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_ULTIMATE)
            {
                kNameLabel.textlocalize = FLocalizeString.Instance.GetText(tabledata.Name);
                value = tabledata.Value1 + (tabledata.IncValue1 * (float)(beforelevel - 1));
                string strPersent = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR),
                                                  FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONE_POINT_TEXT, (value / (float)eCOUNT.MAX_RATE_VALUE * 100.0f)));
                FLocalizeString.SetLabel(kBefore_DecsLabel, tabledata.Desc, strPersent);
                if (isMaxLv == false)
                    value = tabledata.Value1 + (tabledata.IncValue1 * (float)(nextlevel - 1));
                strPersent = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR),
                                                  FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONE_POINT_TEXT, (value / (float)eCOUNT.MAX_RATE_VALUE * 100.0f)));
                FLocalizeString.SetLabel(kNext_DecsLabel, tabledata.Desc, strPersent);
            }
            else
            {
                FLocalizeString.SetLabel(kNameLabel, tabledata.Name);
                FLocalizeString.SetLabel(kBefore_DecsLabel, tabledata.Desc);
                FLocalizeString.SetLabel(kNext_DecsLabel, tabledata.Desc);
            }
        }
        else
        {
            kUpgrade.SetActive(false);
            kSelect.SetActive(true);
            GameSupport.SetSkillSprite(ref kSelectSkillIconSpr, tabledata.Atlus, tabledata.Icon);
            kSelectNameLabel.textlocalize = FLocalizeString.Instance.GetText(tabledata.Name);
        }
        
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == tabledata.ItemReqListID && x.Level == level);
        if (reqdata != null)
        {
            kPoint.gameObject.SetActive(true);
            kGold.gameObject.SetActive(true);

            //  골드
            int formatID = reqdata.Gold <= GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.GOLD] ? (int)eTEXTID.GOODSTEXT_W : (int)eTEXTID.GOODSTEXT_R;
            FLocalizeString.SetLabel(kGoldLabel, formatID, reqdata.Gold);

            //  포인트
            formatID = reqdata.GoodsValue <= chardata.PassviePoint ? (int)eTEXTID.GOODSTEXT_W : (int)eTEXTID.GOODSTEXT_R;
            FLocalizeString.SetLabel(kPointLabel, formatID, reqdata.GoodsValue);

            if (chardata.Level < reqdata.LimitLevel) // 레벨제한
            {
                kUpgradeBtn.gameObject.SetActive(false);
                kDisableSpr.gameObject.SetActive(true);
                kDisableLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1365), reqdata.LimitLevel);
            }
            else
            {
                kUpgradeBtn.gameObject.SetActive(true);
                kDisableSpr.gameObject.SetActive(false);
            }
        }
        else
        {
            kPoint.gameObject.SetActive(false);
            kGold.gameObject.SetActive(false);
            kUpgradeBtn.gameObject.SetActive(false);
            kDisableSpr.gameObject.SetActive(false);
        }
    }
 
    public void OnClick_CancelBtn()
	{
        OnClickClose();
	}

	public void OnClick_UpgradeBtn() {
        if( mbLockUpgradeBtn ) {
            return;
		}

		long uid = (long)UIValue.Instance.GetValue( UIValue.EParamType.CharSelUID );
		int tableid = (int)UIValue.Instance.GetValue( UIValue.EParamType.CharSelTableID );

		CharData chardata = GameInfo.Instance.GetCharData( uid );
        if ( chardata == null ) {
            return;
        }
		
        int skillid = (int)UIValue.Instance.GetValue( UIValue.EParamType.CharSkillPassiveID );

		GameTable.CharacterSkillPassive.Param tabledata = GameInfo.Instance.GameTable.FindCharacterSkillPassive( skillid );
        if ( tabledata == null ) {
            return;
        }

		int level = 0;
		//  스킬을 이미 배운 상태인 경우 배운 스킬의 레벨로 갱신합니다.
		var passviedata = chardata.PassvieList.Find( x => x.SkillID == skillid );
		if ( passviedata != null ) {
			if ( passviedata.SkillLevel >= tabledata.MaxLevel ) {
				MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3007 ) );
				return;
			}

			level = passviedata.SkillLevel;
		}

		GameTable.ItemReqList.Param reqdata = GameInfo.Instance.GameTable.FindItemReqList( x => x.Group == tabledata.ItemReqListID && x.Level == level );
        if ( reqdata == null ) {
            return;
        }

		//레벨 제한 확인
		if ( chardata.Level < reqdata.LimitLevel ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3008 ) );
			return;
		}

		//스킬 포인트 확인     
		if ( reqdata.GoodsValue > chardata.PassviePoint ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3010 ) );
			return;
		}

		//재화 확인
		if ( !GameSupport.IsCheckGoods( eGOODSTYPE.GOLD, reqdata.Gold ) ) {
			return;
		}

		_sendskill = skillid;
        mbLockUpgradeBtn = true;

        GameInfo.Instance.Send_ReqLvUpSkill( chardata.CUID, skillid, OnNetCharSkillPassiveOpen );
	}

	public void OnNetCharSkillPassiveOpen( int result, PktMsgType pktmsg ) {
		Invoke( "ReleaseUpgradeBtnLock", GameInfo.Instance.GameConfig.SkillLevelUpDelayTimeSec );

		if ( result != 0 ) {
			return;
		}

		if ( GameSupport.IsTutorial() ) {
			if ( _sendskillslot != null ) {
				_sendskillslot.ShowEffect();
			}
			_sendskillslot = null;
		}

		if ( _sendskillseleteslot != null ) {
			_sendskillseleteslot.ShowEffect();
		}
		_sendskillseleteslot = null;

		mLevelUpEff.Stop( true, ParticleSystemStopBehavior.StopEmittingAndClear );
		mLevelUpEff.Play();

        SoundManager.Instance.PlayUISnd( 11 );

		LobbyUIManager.Instance.Renewal( "TopPanel" );
		LobbyUIManager.Instance.Renewal( "GoodsPopup" );
		LobbyUIManager.Instance.Renewal( "CharInfoPanel" );
		LobbyUIManager.Instance.Renewal( "CharSkillSeletePopup" );

		if ( GameSupport.IsTutorial() ) {
			GameSupport.TutorialNext();
			OnClickClose();
		}
		else {
			bool isClose = false;
			PktInfoSkillLvUp pktInfo = pktmsg as PktInfoSkillLvUp;
			if ( pktInfo != null ) {
				GameTable.CharacterSkillPassive.Param characterSkillPassiveParam = GameInfo.Instance.GameTable.FindCharacterSkillPassive( (int)pktInfo.skillTID_ );
				if ( characterSkillPassiveParam != null ) {
					isClose = characterSkillPassiveParam.Type == (int)eCHARSKILLPASSIVETYPE.SELECT_NORMAL;
				}
			}

			if ( isClose ) {
				OnClickClose();
			}
			else {
				Renewal( true );
			}
		}
	}

	public void SetSkillSlot(UISkillLevelUpListSlot sendskillslot)
    {
        _sendskillslot = sendskillslot;
        _sendskillseleteslot = null;
    }

    public void SetSkillSlot(UISkillSeleteListSlot sendskillslot)
    {
        _sendskillseleteslot = sendskillslot;
        _sendskillslot = null;
    }

    private void ReleaseUpgradeBtnLock() {
        mbLockUpgradeBtn = false;
	}
}
