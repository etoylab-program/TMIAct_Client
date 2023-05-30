using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class UIArenaTowerListSlot : FSlot {
    public enum eState { 
        LOCK = 0, 
        UNLOCK, 
        CLEAR, 
    }

    private enum eStair {
		LEFT_LOCK = 0, 
        LEFT_UNLOCK,
		RIGHT_LOCK, 
        RIGHT_UNLOCK,
		MAX
	}


    [Header("[Property]")]
    public TowerFloor   kTowerLeft;
    public TowerFloor   kTowerRight;
    public UISprite[]   kStairSprs;


    public eState   State           { get; private set; } = eState.LOCK;
    public int      ArenaTowerId    { get; private set; } = 0;

    private GameTable.ArenaTower.Param  mArenaTowerParam        = null;
    private TowerFloor                  mSelectedTower          = null;
    private UIArenaTowerMainPanel       mArenaTowerMainPanel    = null;


    public void Init()
    {
        ArenaTowerId = 0;
        mArenaTowerParam = null;
        mSelectedTower = null;
        kTowerLeft.Init();
        kTowerRight.Init();

        var iter = kStairSprs.GetEnumerator();
        while(iter.MoveNext())
        {
            UISprite s = iter.Current as UISprite;
            s.SetActive(false);
        }
    }

	public void UpdateSlot( UIArenaTowerMainPanel arenaTowerMainPanel, int index, int ID ) {
		Init();
        mArenaTowerMainPanel = arenaTowerMainPanel;

        ArenaTowerId = ID;
        if ( ArenaTowerId <= 0 ) {
            return;
        }

		mArenaTowerParam = GameInfo.Instance.GameTable.ArenaTowers.Find( x => x.ID == ArenaTowerId );
        if ( mArenaTowerParam == null ) {
            return;
        }

		State = eState.LOCK;
        if ( mArenaTowerParam.ID == GameInfo.Instance.TowerClearID + 1 ) {
            State = eState.UNLOCK;
        }
        else if ( mArenaTowerParam.ID <= GameInfo.Instance.TowerClearID ) {
            State = eState.CLEAR;
        }

		//홀수 : 오른쪽, 짝수 : 왼쪽
		bool isRight = ArenaTowerId % 2 == 0;
		mSelectedTower = isRight ? kTowerRight : kTowerLeft;
		mSelectedTower.Update( mArenaTowerParam, State, this.gameObject );

        if (State != eState.LOCK && mArenaTowerMainPanel.CurrentFloor == ArenaTowerId)
        {
            mArenaTowerMainPanel.SetSelectedTowerListSlot(this);
        }

        if ( index > 0 ) {
			if ( isRight ) {
				kStairSprs[State == eState.CLEAR ? (int)eStair.RIGHT_UNLOCK : (int)eStair.RIGHT_LOCK].SetActive( true );
			}
			else {
				kStairSprs[State == eState.CLEAR ? (int)eStair.LEFT_UNLOCK : (int)eStair.LEFT_LOCK].SetActive( true );
			}
		}
	}

	public void OnClick_Floor()
    {
        /*
        if (State != eState.UNLOCK)
            return;

        Debug.Log(string.Format("OnClick_Floor ID : {0}", mArenaTowerParam.ID));

        UIValue.Instance.SetValue(UIValue.EParamType.ArenaTowerChoiceStage, mArenaTowerParam.ID);
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.ARENATOWER_STAGE);
        */
    }

    public void OnClick_FriendInfo()
    {
        if (GameInfo.Instance.CommunityData.FriendList.Count <= 0)
        {
            //등록된 친구가 없습니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1536));
            return;
        }

        //List<FriendUserData> clearList = GameInfo.Instance.CommunityData.FriendList.FindAll(x => x.ClearArenaTowerId >= mArenaTowerParam.ID);
        //if(clearList == null || clearList.Count <= 0)
        //{
        //    //등록된 친구가 없습니다.
        //    MessageToastPopup.Show(FLocalizeString.Instance.GetText(1536));
        //    return;
        //}

        UIValue.Instance.SetValue(UIValue.EParamType.ArenaTowerChoiceStage, mArenaTowerParam.ID);
        LobbyUIManager.Instance.ShowUI("FriendTooltipPopup", true);
    }

    public void OnBtnSelect() {
        mArenaTowerMainPanel.SetSelectedTowerListSlot( this );
    }

    public void SetSelect( bool select ) {
        mSelectedTower.SetSelect( select );
    }


    [Serializable]
    public class TowerFloor
    {
        public GameObject       kSelectObj;
        public GameObject       kLockObj;
        public GameObject       kFirendObj;
        public UISprite         kBossSpr;
        public GameObject       kCompleteObj;
        public UIItemListSlot   kItemSlot;
        public UILabel          kFloorNameLbl;
        public UISprite         kFloorNameEnableSpr;
        public UISprite         kFloorNameDisableSpr;
        public UISprite         kFloorLockBGSpr;
        public UISprite         kFloorUnLockBGSpr;
        public UIButton         _SelectBtn;

        private GameTable.ArenaTower.Param  mArenaTowerParam    = null;
        private bool                        mSelected           = false;


        public void Init()
        {
            kSelectObj?.SetActive(false);
            kLockObj?.SetActive(false);
            kFirendObj?.SetActive(false);
            kBossSpr?.SetActive(false);
            kCompleteObj?.SetActive(false);
            kItemSlot?.SetActive(false);
            kFloorNameLbl?.SetActive(false);
            kFloorNameEnableSpr?.SetActive(false);
            kFloorNameDisableSpr?.SetActive(false);
            kFloorLockBGSpr?.SetActive(false);
            kFloorUnLockBGSpr?.SetActive(false);
            _SelectBtn?.SetActive( false );
        }

        public void Update(GameTable.ArenaTower.Param param, eState state, GameObject parent)
        {
            mArenaTowerParam = param;
            if (mArenaTowerParam == null)
            {
                Init();
                return;
            }

            kFirendObj.SetActive(true);

            kFloorNameLbl.SetActive(true);
            kFloorNameLbl.text = string.Format("F{0}", mArenaTowerParam.ID);

            kBossSpr.SetActive(mArenaTowerParam.Type == 1);

            kItemSlot.SetActive(true);
            kItemSlot.ParentGO = parent;
            kItemSlot.UpdateSlot(mArenaTowerParam);

            _SelectBtn?.SetActive( true );

            switch ( state ) {
                case eState.LOCK:
                    kFloorLockBGSpr.SetActive( true );
                    kFloorNameDisableSpr.SetActive( true );
                    kLockObj.SetActive( true );
                    break;

                case eState.UNLOCK:
                    kFloorNameEnableSpr.SetActive( true );
                    kFloorUnLockBGSpr.SetActive( true );
                    //kSelectObj.SetActive( true );
                    break;

                case eState.CLEAR:
                    kFloorLockBGSpr.SetActive( true );
                    kFloorUnLockBGSpr.SetActive( true );
                    kCompleteObj.SetActive( true );
                    break;
            }

            mSelected = false;
        }

        public void SetSelect( bool select ) {
            kSelectObj.SetActive( select );
        }
	}
}
