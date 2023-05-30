using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGemOptChangeAutoPopup : FComponent {
	[Header( "UIGemOptChangeAutoPopup" )]
	[SerializeField] private FList _OptionFList;

	[SerializeField] private GameObject _OptionRootObj;
	[SerializeField] private GameObject _HelpRootObj;

	private GemData mGemData;

	private List<GameTable.GemRandOpt.Param> mGemRandOptParamList = new List<GameTable.GemRandOpt.Param>();

	private bool mIsHelp;

	private int mSelectOptionIndex;

	public override void Awake() {
		base.Awake();

		_OptionFList.EventUpdate = OnEventOptionFListUpdate;
		_OptionFList.EventGetItemCount = OnEventGetItemCount;
		_OptionFList.UpdateList();
	}

	public override void InitComponent() {
		base.InitComponent();

		_OptionRootObj.SetActive( !mIsHelp );
		_HelpRootObj.SetActive( mIsHelp );

		if ( !mIsHelp ) {
			_OptionFList.RefreshNotMoveAllItem();
			_OptionFList.SpringSetFocus( 0, isImmediate: true );
		}
	}

	public override void OnEnable() {
		InitComponent();
		base.OnEnable();
	}

	public void SetOptionList( ref GemData gemData, ref List<GameTable.GemRandOpt.Param> gemRandOptParamList, ref int selectOptionIndex ) {
		mGemData = gemData;
		mGemRandOptParamList = gemRandOptParamList;

		mIsHelp = false;

		mSelectOptionIndex = selectOptionIndex;
	}

	public void SetHelp() {
		mIsHelp = true;
	}

	public void SelectOption( int index ) {
		if ( index < 0 && mGemRandOptParamList.Count <= index ) {
			return;
		}

		UIGemOptChangePopup gemOptChangePopup = LobbyUIManager.Instance.GetActiveUI<UIGemOptChangePopup>( "GemOptChangePopup" );
		if ( gemOptChangePopup != null ) {
			gemOptChangePopup.SetAutoOptionIndex( index );
		}

		OnClickClose();
	}

	private void OnEventOptionFListUpdate( int index, GameObject obj ) {
		UIGemChangeOptionListSlot slot = obj.GetComponent<UIGemChangeOptionListSlot>();
		if ( slot == null ) {
			return;
		}
		GameTable.GemRandOpt.Param param = null;
		if ( 0 <= index && index < mGemRandOptParamList.Count ) {
			param = mGemRandOptParamList[index];
		}

		if ( slot.ParentGO == null ) {
			slot.ParentGO = this.gameObject;
		}

		slot.UpdateSlot( index, param, ref mGemData, ref mSelectOptionIndex );
	}

	private int OnEventGetItemCount() {
		return mGemRandOptParamList.Count;
	}
}
