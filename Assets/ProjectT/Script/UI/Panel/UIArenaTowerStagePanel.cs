using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIArenaTowerStagePanel : FComponent {
	[Header("[Property]")]
	[SerializeField] private	UILabel					kStageTitle;
	[SerializeField] private	UILabel					kStageDesc;
	[SerializeField] private	UIArenaBattleListSlot	kUserSlot;
	[SerializeField] private	UILabel					kRewardTitle;
	[SerializeField] private	UIItemListSlot			kFirstRewardSlot;
	[SerializeField] private	UILabel					kStageInfoTitle;
	[SerializeField] private	FList					kStageInfoList;
    public						UIGoodsUnit				DirectiveGoodsUnit;
    public						UILabel					LbTimeLimit;


	private List<GameClientTable.HelpEnemyInfo.Param>	mEnemyInfoList		= new List<GameClientTable.HelpEnemyInfo.Param>();
	private int											mStageIndex			= 0;
	private GameTable.ArenaTower.Param					mArenaTowerParam	= null;
    private long										mSelectFriendCUID	= 0;
	private bool										mTestPlay			= false;


    public override void Awake()
    {
        base.Awake();

		if (kStageInfoList != null)
		{
			kStageInfoList.EventUpdate = UpdateListSlot;
			kStageInfoList.EventGetItemCount = () =>
			{
				if (mEnemyInfoList == null) return 0;
				return mEnemyInfoList.Count;
			};
		}
    }

	public override void Renewal( bool bChildren = false ) {
		base.Renewal( bChildren );

		mStageIndex = (int)UIValue.Instance.GetValue( UIValue.EParamType.ArenaTowerChoiceStage );
		mArenaTowerParam = GameInfo.Instance.GameTable.FindArenaTower( mStageIndex );

		mTestPlay = mStageIndex <= GameInfo.Instance.TowerClearID;
		GameInfo.Instance.IsTowerStageTestPlay = mTestPlay;

		FLocalizeString.SetLabel( kStageTitle, 5023 );
		FLocalizeString.SetLabel( kStageDesc, mArenaTowerParam.Desc );

		kFirstRewardSlot.SetActive( true );
		kFirstRewardSlot.ParentGO = gameObject;
		kFirstRewardSlot.UpdateSlot( mArenaTowerParam );

		kUserSlot.Update_TowerMainPanel();

		mEnemyInfoList.Clear();
		var list = GameInfo.Instance.GameClientTable.FindAllHelpEnemyInfo(x => (x.StageType == 2) && (x.StageID == mArenaTowerParam.ID));
		if ( list != null && list.Count > 0 ) {
			var iter = list.GetEnumerator();
			while ( iter.MoveNext() ) {
				mEnemyInfoList.Add( iter.Current );
			}
		}
		kStageInfoList.UpdateList();

		mSelectFriendCUID = 0;
		if ( GameInfo.Instance.ArenaTowerFriendContainer != null && GameInfo.Instance.ArenaTowerFriendContainer.Count > 0 ) {
			mSelectFriendCUID = GameInfo.Instance.ArenaTowerFriendContainer.GetValidValue();
		}

		DirectiveGoodsUnit.SetActive( false );

		int count = mSelectFriendCUID > 0 ? GameInfo.Instance.GameConfig.FriendCharUseItemCnt : 0;
		if ( count > 0 ) {
			int itemID = 10052;
			ItemData itemData = GameInfo.Instance.GetItemData(itemID);

			DirectiveGoodsUnit.SetActive( true );
			Texture t = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Item/Item_{0}.png", itemID));
			Color32 c = Color.white;
			if ( itemData == null || itemData.Count < count )
				c = Color.red;

			DirectiveGoodsUnit.InitGoodsUnitTexture( t, count, c );
		}

		LbTimeLimit.transform.parent.gameObject.SetActive( mArenaTowerParam.ConditionValue > 0 );
		if ( LbTimeLimit.gameObject.activeSelf ) {
			float timeLimit = (float)mArenaTowerParam.ConditionValue / 60.0f;
			LbTimeLimit.textlocalize = string.Format( FLocalizeString.Instance.GetText( 1761 ), ( (int)timeLimit ).ToString() );
		}
	}

	private void UpdateListSlot(int index, GameObject slotObj)
    {
		UIGameStageInfoSlot slot = slotObj.GetComponent<UIGameStageInfoSlot>();
		if (slot == null) return;
		slot.UpdateSlot(mEnemyInfoList[index]);
	}

	public void OnClick_Start()
    {
		if (!GameSupport.ArenaTowerTeamCheckFlag())
			return;

		List<long> _towerTeamCharList = GameSupport.GetArenaTowerTeamCharList().FindAll(x => x == 0);
		if(_towerTeamCharList.Count >= 3 )
        {
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(3226), null);
            return;
        }

		if (GameSupport.IsEmptyInEquipMainWeapon(ePresetKind.ARENA_TOWER))
		{
			return;
		}

		ulong friendUUID = 0;

        if (mSelectFriendCUID > 0)
        {
            var iterA = GameInfo.Instance.TowerFriendTeamData.GetEnumerator();
            while (iterA.MoveNext())
            {
                if (iterA.Current.charlist == null || iterA.Current.charlist.Count == 0)
                    continue;

                var iterB = iterA.Current.charlist.GetEnumerator();
                while (iterB.MoveNext())
                {
                    if (iterB.Current.CharData == null)
                        continue;

                    if (iterB.Current.CharData.CUID == mSelectFriendCUID)
                    {
                        friendUUID = (ulong)iterA.Current.UUID;
                        break;
                    }
                }

                if (friendUUID != 0)
                    break;
            }
        }

		if (friendUUID > 0)
		{
			//특별 지령석 체크
			var item = GameInfo.Instance.ItemList.Find(x => x.TableID == 10052);
			if (item == null || item.Count <= 0)
			{	
				MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3071), FLocalizeString.Instance.GetText(10110052)));
				return;
			}
		}

		if ( !mTestPlay ) {
			//스테이지 시작
			GameInfo.Instance.Send_ReqArenaTowerGameStart( friendUUID, (uint)mArenaTowerParam.ID, false, OnNetGameStart );
		}
		else {
			OnNetGameStart( 0, null );
		}
	}

	public void OnNetGameStart(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        GameInfo.Instance.SelecteStageTableId = mStageIndex;
        GameInfo.Instance.IsTowerStage = true;

		UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.LobbyToStage);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, mStageIndex);

		if (GameInfo.Instance.TowerClearID < mStageIndex)
        {
			UIValue.Instance.RemoveValue(UIValue.EParamType.ArenaTowerChoiceStage);
		}

        LobbyUIManager.Instance.ShowUI("LoadingPopup", false);
		//AppMgr.Instance.LoadScene(AppMgr.eSceneType.Stage, "Stage_roof_top_Tower_Test"); // mArenaTowerParam.Scene);
		AppMgr.Instance.LoadScene(AppMgr.eSceneType.Stage, mArenaTowerParam.Scene);
	}
}
