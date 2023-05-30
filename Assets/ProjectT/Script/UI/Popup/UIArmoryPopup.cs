using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIArmoryPopup : FComponent
{
    [Serializable]
    public class UICharInfo
    {
        public UISprite CharNameSpr;
        public UISprite CharGradeSpr;
        public UITexture CharIconTex;
        public UILabel CharLevelLabel;
        public GameObject FavorLockObj;
        public UILabel FavorBuffLabel;
    }

	public GameObject kCardTeamNoneObj;
	public UICardArmoryTeamSlot kCardArmoryTeamSlot;
	public UIButton kChangeBtn;
	public UILabel kHPLabel;
	public UISprite kHPIcon;
	public UISprite kCardIconSpr;
	public UIButton kHelpBtn;
	public UILabel kATKLabel;
	public UISprite kATKIcon;
	[SerializeField] private FList _ArmoryListInstance;
	[SerializeField] private UIScrollView scrollView = null;
	[SerializeField] private UISprite botArrowSpr = null;
	[SerializeField] private List<UICharInfo> favorList = null;
	[SerializeField] private UIPanel _CenterScrollPanel = null;

	private int _cardFormationID = 0;
	private List<GameTable.ItemReqList.Param> _weaponReqList;

	public override void Awake()
	{
		base.Awake();

		if(this._ArmoryListInstance == null) return;
		
		this._ArmoryListInstance.EventUpdate = this._UpdateArmoryListSlot;
		this._ArmoryListInstance.EventGetItemCount = this._GetArmoryElementCount;

        scrollView.onPressMoving = CheckScrollArrow;

		int interval = (int)Math.Abs( _CenterScrollPanel.transform.localPosition.y * 2 );
		float finalSizeY = _CenterScrollPanel.GetViewSize().y + interval;
		if ( Screen.height < finalSizeY ) {
			_CenterScrollPanel.baseClipRegion = new Vector4(
				_CenterScrollPanel.baseClipRegion.x, _CenterScrollPanel.baseClipRegion.y,
				_CenterScrollPanel.baseClipRegion.z, Screen.height - interval );
		}
	}
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
		_weaponReqList = GameInfo.Instance.GameTable.FindAllItemReqList(x => x.Group == GameInfo.Instance.GameConfig.WpnDepotItemReqGroup);

		_ArmoryListInstance.UpdateList();

        scrollView.ResetPosition();

		CheckScrollArrow();
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
		_cardFormationID = GameSupport.GetSelectCardFormationID();
		
		SetCardTeam();
		SetWeaponArmory();
		SetFavorBuffChar();
	}

	private void CheckScrollArrow()
	{
		Vector3 constraint = scrollView.panel.CalculateConstrainOffset(scrollView.bounds.min, scrollView.bounds.max);
		botArrowSpr.SetActive(constraint.y >= 0);
	}

	private void _UpdateArmoryListSlot(int index, GameObject slotObject)
	{
		do
		{
			UIArmoryListSlot slot = slotObject.GetComponent<UIArmoryListSlot>();
			if (null == slot) break;
			slot.ParentGO = this.gameObject;

			long weaponUID = 0;
			if (0 <= index && GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList.Count > index)
			{
				weaponUID = GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList[index];
			}

			slot.UpdateSlot(weaponUID, _weaponReqList[index]);

		} while(false);
	}
	
	private int _GetArmoryElementCount()
	{
		return _weaponReqList.Count;
	}
	
	public void OnClick_ChangeBtn()
	{
		LobbyUIManager.Instance.ShowUI("CardFormationPopup", true);
	}
	
	public void OnClick_HelpBtn()
	{
		LobbyUIManager.Instance.ShowUI("WeaponDepotSetPopup", true);
	}

	public void OnClick_CharChangeBtn(int index)
	{
        UIValue.Instance.SetValue(UIValue.EParamType.CharSeletePopupType, (int)eCharSelectFlag.FAVOR_BUFF_CHAR);
		UIValue.Instance.SetValue(UIValue.EParamType.SelectFavorBuffCuid, GameInfo.Instance.UserData.ArrFavorBuffCharUid[index]);
        UIValue.Instance.SetValue(UIValue.EParamType.SelectFavorBuffCharIndex, index);

        LobbyUIManager.Instance.ShowUI("CharSeletePopup", true);
	}
	
	public void OnClick_BackBtn()
	{
		OnClickClose();
	}

    public override void OnClickClose()
    {
		LobbyUIManager.Instance.Renewal("UserInfoPopup");
		LobbyUIManager.Instance.Renewal("ArenaMainPanel");
		LobbyUIManager.Instance.Renewal("ArenaBattleConfirmPopup");
		LobbyUIManager.Instance.Renewal("CharMainPanel");

		UIRaidDetailPopup raidPopup = LobbyUIManager.Instance.GetActiveUI<UIRaidDetailPopup>( "RaidDetailPopup" );
		if( raidPopup ) {
			raidPopup.Renewal( true );
		}
		else {
			LobbyUIManager.Instance.Renewal( "StageDetailPopup" );
		}

		base.OnClickClose();
    }

	private void SetCardTeam()
	{
		//if (GameInfo.Instance.UserData.CardFormationID == (int)eCOUNT.NONE)
		if (_cardFormationID == (int)eCOUNT.NONE)
		{
			kCardTeamNoneObj.SetActive(true);
			kCardArmoryTeamSlot.SetActive(false);
		}
		else
		{
			kCardTeamNoneObj.SetActive(false);
			kCardArmoryTeamSlot.SetActive(true);
			kCardArmoryTeamSlot.UpdateSlot();
		}

		kHPLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, GameSupport.GetTotalCardFormationEffectValue().ToString("F2"));
	}

	private void SetWeaponArmory()
	{
		_ArmoryListInstance.RefreshNotMove();
		
		kATKLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT, GameSupport.GetTotalWeaponDepotEffectValue().ToString("F2"));
	}

	private void SetFavorBuffChar()
	{
        for (int i = 0; i < GameInfo.Instance.UserData.ArrFavorBuffCharUid.Length; i++)
        {
            if (favorList.Count <= i)
            {
                break;
            }

            UICharInfo info = favorList[i];

            CharData charData = GameInfo.Instance.GetCharData(GameInfo.Instance.UserData.ArrFavorBuffCharUid[i]);

            info.CharNameSpr.SetActive(charData != null);
            info.CharGradeSpr.SetActive(charData != null);
            info.CharIconTex.SetActive(charData != null);

            if (charData != null)
            {
                // Char
                info.CharNameSpr.spriteName = Utility.AppendString("Name_Horizontal_", ((ePlayerCharType)charData.TableData.ID).ToString());
                info.CharGradeSpr.spriteName = Utility.AppendString("grade_", charData.Grade.ToString("D2"));
                info.CharIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon",
                    Utility.AppendString("Icon/Char/ListSlot/ListSlot_", charData.TableData.Icon, "_", charData.EquipCostumeID.ToString(), ".png"));
				info.CharLevelLabel.textlocalize = FLocalizeString.Instance.GetText( 211, charData.Level );

                // Favor
                GameTable.LevelUp.Param levelUpParam =
                    GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == charData.TableData.PreferenceLevelGroup && x.Level == charData.FavorLevel);
                if (levelUpParam != null)
                {
                    info.FavorLockObj.SetActive(0 <= levelUpParam.Exp);
                }

                GameTable.Buff.Param buffParam = GameInfo.Instance.GameTable.FindBuff(charData.TableData.PreferenceBuff);
                if (buffParam != null)
                {
                    info.FavorBuffLabel.textlocalize = FLocalizeString.Instance.GetText(buffParam.Name);
                }
            }
            else
            {
                info.FavorLockObj.SetActive(true);
                info.CharLevelLabel.textlocalize = info.FavorBuffLabel.textlocalize = string.Empty;
            }
        }
    }
}
