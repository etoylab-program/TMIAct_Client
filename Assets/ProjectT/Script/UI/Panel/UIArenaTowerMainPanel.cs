using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIArenaTowerMainPanel : FComponent {
    [Header("[Property]")]
    [SerializeField] private UIArenaBattleListSlot  kUserSlot;
	[SerializeField] private FList                  kArenaTowerList;
	[SerializeField] private UIScrollView           kArenaTowerScrollView;
    [SerializeField] private UILabel                _StartBtnLabel;


    public UIArenaTowerListSlot SelectedTowerListSlot   { get; private set; }   = null;
    public int                  CurrentFloor            { get; private set; }   = 5;

    private int     mArenaTowerListCount    = 0;
    private bool    mTestPlay               = false;

    public override void Awake()
    {
        base.Awake();

        float scrollWheelFactor = 0.25f;
    #if UNITY_EDITOR
        scrollWheelFactor = 1f;
    #elif UNITY_STANDALNO
		scrollWheelFactor = 1f;
    #endif
        kArenaTowerScrollView.scrollWheelFactor = scrollWheelFactor;

        kArenaTowerList.EventUpdate = UpdateListSlot;
        kArenaTowerList.EventGetItemCount = () => { return mArenaTowerListCount; };
        kArenaTowerList.InitBottomFixing();
        kArenaTowerList.UpdateList();
    }

    public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}

	public override void OnDisable() {
		base.OnDisable();
        SelectedTowerListSlot = null;
    }

	public override void InitComponent()
	{
        UIValue.Instance.SetValue(UIValue.EParamType.CardFormationType, eCharSelectFlag.ARENATOWER);
    }

    public override void Renewal( bool bChildren = false ) {
		base.Renewal( bChildren );

		mArenaTowerListCount = GameInfo.Instance.GameTable.ArenaTowers.Count;
		SelectedTowerListSlot = null;
		mTestPlay = false;

        if ( GameInfo.Instance.TowerClearID >= mArenaTowerListCount ) {
            CurrentFloor = mArenaTowerListCount;
        }
        else {
            CurrentFloor = GameInfo.Instance.TowerClearID + 1;
        }

        int choiceStageId = (int)UIValue.Instance.TryGetValue(UIValue.EParamType.ArenaTowerChoiceStage, -1, true);
        if (0 < choiceStageId)
        {
            CurrentFloor = choiceStageId;
        }

        kArenaTowerList.RefreshNotMoveAllItem();

        int focusIndex = mArenaTowerListCount - CurrentFloor;
        if ( focusIndex >= mArenaTowerListCount - 1 ) {
            focusIndex = mArenaTowerListCount - 2;
        }

        kArenaTowerList.SpringSetFocus(focusIndex, isImmediate: true);
		kUserSlot.Update_TowerMainPanel();
	}

	public override void OnUIOpen()
    {
        LobbyUIManager.Instance.kBlackScene.SetActive(false);
    }

    public void OnBtnEnter()
    {
        if (GameSupport.IsEmptyInEquipMainWeapon(ePresetKind.ARENA_TOWER))
        {
            return;
        }

        if (GameInfo.Instance.GameTable.ArenaTowers.Count <= 0)
        {
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(101033), null);
            return;
        }

        if ( !mTestPlay ) {
            int finalFloor = GameInfo.Instance.GameTable.ArenaTowers[GameInfo.Instance.GameTable.ArenaTowers.Count - 1].ID;
            if ( GameInfo.Instance.TowerClearID >= finalFloor ) {
                MessagePopup.OK( eTEXTID.OK, FLocalizeString.Instance.GetText( 3235 ), null );
                return;
            }
        }

        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.ARENATOWER_STAGE);
    }

    public void SetSelectedTowerListSlot( UIArenaTowerListSlot slot ) {
        SelectedTowerListSlot?.SetSelect( false );

        SelectedTowerListSlot = slot;
        CurrentFloor = SelectedTowerListSlot.ArenaTowerId;

        if ( SelectedTowerListSlot.State == UIArenaTowerListSlot.eState.UNLOCK ) {
            _StartBtnLabel.textlocalize = FLocalizeString.Instance.GetText( 1060 );
            mTestPlay = false;
        }
        else {
            _StartBtnLabel.textlocalize = FLocalizeString.Instance.GetText( 1832 );
            mTestPlay = true;
        }

		GameTable.ArenaTower.Param param = GameInfo.Instance.GameTable.ArenaTowers.Find( x => x.ID == CurrentFloor );
        if ( param != null ) {
			UIValue.Instance.SetValue( UIValue.EParamType.ArenaTowerChoiceStage, param.ID );
		}

		slot.SetSelect( true );
    }

	private void UpdateListSlot( int index, GameObject slotObj ) {
		UIArenaTowerListSlot slot = slotObj.GetComponent<UIArenaTowerListSlot>();
		if ( slot == null ) {
			return;
		}

		slot.UpdateSlot( this, index, mArenaTowerListCount - index );

		if ( slot.ArenaTowerId == CurrentFloor ) {
            slot.SetSelect( true );
		}
        else {
            slot.SetSelect( false );
		}
	}

    public void OnClick_PresetBtn()
    {
        if (GameInfo.Instance.ArenaTowerPresetDatas != null)
        {
            OnNet_PresetList(0, null);
        }
        else
        {
            GameInfo.Instance.Send_ReqGetUserPresetList(ePresetKind.ARENA_TOWER, 0, OnNet_PresetList);
        }
    }

    private void OnNet_PresetList(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        PktInfoUserPreset pktInfoUserPreset = pktmsg as PktInfoUserPreset;
        if (pktInfoUserPreset != null && pktInfoUserPreset.infos_.Count <= 0)
        {
            GameInfo.Instance.SetPresetData(ePresetKind.ARENA_TOWER, -1, GameInfo.Instance.GameConfig.ContPresetSlot);
        }

        UIPresetPopup presetPopup = LobbyUIManager.Instance.GetUI<UIPresetPopup>("PresetPopup");
        if (presetPopup == null)
        {
            return;
        }

        presetPopup.SetPresetData(eCharSelectFlag.ARENATOWER, ePresetKind.ARENA_TOWER);
        presetPopup.SetUIActive(true);
    }
}
