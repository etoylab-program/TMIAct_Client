
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIArenaCharSelectPopup : FComponent
{
    [Header("[Property]")]
    public FTab     TabType;
    public FList    FListChar;
    public FList    FListFriendChar;
    public UILabel  LbCombatPower;
    public UILabel  LbSelectBtn;
    public UILabel  LbEmpty;

    private List<CharData>  mListCharData               = new List<CharData>();
    private List<long>      mListTowerTeamCharUID       = null;
    private int             mSelectedSlotIndex          = 0;
    private int             mSelectCharIndex            = 0;
    private int             mSelectedFriendCharIndex    = 0;
    private bool            mbSelectFriendCharList      = false;


    public override void Awake()
    {
        base.Awake();

        TabType.EventCallBack = OnTabType;

        FListChar.EventUpdate = UpdateCharListSlot;
        FListChar.EventGetItemCount = GetCharListCount;

        FListFriendChar.EventUpdate = UpdateFriendCharListSlot;
        FListFriendChar.EventGetItemCount = GetFriendCharListCount;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        InitComponent();
    }

    public override void InitComponent()
    {
        mSelectedSlotIndex = -1;
        mSelectCharIndex = -1;
        mSelectedFriendCharIndex = -1;
        mbSelectFriendCharList = false;

        LbSelectBtn.textlocalize = FLocalizeString.Instance.GetText(1095);
        LbCombatPower.textlocalize = "";

        mListTowerTeamCharUID = GameSupport.GetArenaTowerTeamCharList();
        mSelectedSlotIndex = (int)UIValue.Instance.GetValue(UIValue.EParamType.ArenaTowerTeamCharSlot);

        GameInfo.Instance.ArenaTowerFriendContainer.TryGetValue(mSelectedSlotIndex, out long curFriendCUID);

        if (curFriendCUID > 0)
        {
            TabType.SetTab(1, SelectEvent.Code);
        }
        else
        {
            TabType.SetTab(0, SelectEvent.Code);
        }

        LbEmpty.gameObject.SetActive(false);
    }

    public void SelectChar(int index)
    {
        mSelectCharIndex = index;
        bool clearFlag = false;

        if (!mbSelectFriendCharList)
        {
            FListChar.RefreshNotMove();
            LbCombatPower.textlocalize = GameSupport.GetCombatPowerString(mListCharData[index], eWeaponSlot.MAIN, eContentsPosKind.ARENA_TOWER);
            
            if (mListTowerTeamCharUID[mSelectedSlotIndex] == mListCharData[mSelectCharIndex].CUID)
            {
                clearFlag = true;
            }
        }
        else
        {
            FListFriendChar.RefreshNotMove();
            LbCombatPower.textlocalize = GameSupport.GetCombatPowerString(mListCharData[index], eWeaponSlot.MAIN, eContentsPosKind.ARENA_TOWER);

            if (mListTowerTeamCharUID[mSelectedSlotIndex] == mListCharData[mSelectCharIndex].CUID)
            {
                clearFlag = true;
                mSelectedFriendCharIndex = mSelectCharIndex;
            }
        }

        if (clearFlag)
        {
            LbSelectBtn.textlocalize = FLocalizeString.Instance.GetText(1121);
        }
        else
        {
            LbSelectBtn.textlocalize = FLocalizeString.Instance.GetText(1095);
        }
    }

	public void OnBtnSelect() {
		if ( mSelectCharIndex < 0 || mSelectCharIndex >= mListCharData.Count ) {
			return;
		}

		List<long> listNewCharUid = null;
		long selectedCharUid = mListCharData[mSelectCharIndex].CUID;

		if ( !mbSelectFriendCharList ) {
			GameInfo.Instance.ArenaTowerFriendContainer.TryGetValue( mSelectedSlotIndex, out long curFriendCUID );
			if ( curFriendCUID > 0 ) {
				// 친구 캐릭터의 슬롯 위치를 바꿔줘야 한다.
				int tmpidx = -1;
				for ( int i = 0; i < GameInfo.Instance.TowercharList.Count; i++ ) {
					if ( GameInfo.Instance.TowercharList[i] == selectedCharUid ) {
						tmpidx = i;
						break;
					}
				}

				if ( tmpidx >= 0 ) {
					GameInfo.Instance.ArenaTowerFriendContainer.SwapReplace( tmpidx, curFriendCUID );
				}
				else {
					GameInfo.Instance.ArenaTowerFriendContainer.Remove( mSelectedSlotIndex );
				}
			}
			else {
				// 선택된 슬롯에 친구가 있으면 삭제
				GameInfo.Instance.ArenaTowerFriendContainer.Remove( mSelectedSlotIndex );
			}

			listNewCharUid = GameSupport.ArenaTowerCharChange( selectedCharUid, mSelectedSlotIndex );
		}
		else {
			int CurSlotIdx = GameInfo.Instance.ArenaTowerFriendContainer.GetKeyByValue( selectedCharUid );

			// 변경하려는 슬롯에 이미 친구 캐릭터가 있을 경우
			if ( CurSlotIdx < 0 && GameInfo.Instance.ArenaTowerFriendContainer.TryGetValue( mSelectedSlotIndex, out long uid ) && GameSupport.IsFriend( uid ) ) {
				GameInfo.Instance.ArenaTowerFriendContainer.Remove( mSelectedSlotIndex );
			}

			if ( CurSlotIdx >= 0 ) {
				// 슬롯에 할당되어 있던 친구 캐릭터
				if ( CurSlotIdx == mSelectedSlotIndex ) {
					// 슬롯의 위치가 같으면 슬롯에서 제거
					GameInfo.Instance.ArenaTowerFriendContainer.Remove( mSelectedSlotIndex );
					selectedCharUid = 0;
				}
				else {
					//슬롯의 위치가 다르면 스왑
					GameInfo.Instance.ArenaTowerFriendContainer.SwapReplace( mSelectedSlotIndex, selectedCharUid );

					selectedCharUid = 0;

					// 유저 플레이어 덱 위치 보정
					if ( GameInfo.Instance.TowercharList != null && mSelectedSlotIndex < GameInfo.Instance.TowercharList.Count ) {
						selectedCharUid = GameInfo.Instance.TowercharList[mSelectedSlotIndex];
						mSelectedSlotIndex = CurSlotIdx;
					}
				}
			}
			else {
				if ( GameInfo.Instance.ArenaTowerFriendContainer.ValidCount() >= 1 ) {
					// 친구는 1명만 슬롯에 장착할 수 있음                        
					MessageToastPopup.Show( FLocalizeString.Instance.GetText( 1626 ) );
					return;
				}

				// 슬롯에 처음할당 하는 친구 캐릭터
				GameInfo.Instance.ArenaTowerFriendContainer.Assign( mSelectedSlotIndex, selectedCharUid );
				selectedCharUid = 0;
			}

			// 내 플레이어블 캐릭터 슬롯 변경 유무 체크
			List<long> _oldTeamCharList = new List<long>();
			for ( int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++ ) {
				if ( i >= GameInfo.Instance.TowercharList.Count ) {
					break;
				}

				_oldTeamCharList.Add( GameInfo.Instance.TowercharList[i] );
			}

			listNewCharUid = GameSupport.ArenaTowerCharChange( selectedCharUid, mSelectedSlotIndex );

			bool IsEqual = true;
			for ( int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++ ) {
				if ( i >= listNewCharUid.Count || i >= _oldTeamCharList.Count ) {
					continue;
				}

				if ( _oldTeamCharList[i] != listNewCharUid[i] ) {
					IsEqual = false;
					break;
				}
			}

			if ( IsEqual ) {
				OnReqSetArenaTowerTeam( 0, null );
				return;
			}
		}

		GameInfo.Instance.Send_ReqSetArenaTowerTeam( listNewCharUid, (uint)GameInfo.Instance.UserData.ArenaTowerCardFormationID, OnReqSetArenaTowerTeam );
	}

	private bool OnTabType(int nSelect, SelectEvent type)
    {
        if(type == SelectEvent.Enable)
        {
            return false;
        }

        LbEmpty.gameObject.SetActive(false);
        mSelectCharIndex = -1;

        if (nSelect == 0)
        {
            ShowMyCharList();
        }
        else
        {
            ShowFriendCharList();
        }

        return true;
    }

    private void UpdateCharListSlot(int index, GameObject slotObj)
    {
        UICharListSlot slot = slotObj.GetComponent<UICharListSlot>();
        if (slot == null || index < 0 || index > mListCharData.Count)
        {
            return;
        }

        CharData data = mListCharData[index];

        slot.ParentGO = gameObject;
        slot.UpdateSlot(UICharListSlot.ePos.SeleteArenaTowerPopup, index, data, mSelectCharIndex, true);
    }

    private int GetCharListCount()
    {
        return mListCharData.Count;
    }

    private void UpdateFriendCharListSlot(int index, GameObject slotObj)
    {
        UIArenaTowerSelectFriendCharSlot slot = slotObj.GetComponent<UIArenaTowerSelectFriendCharSlot>();
        if (slot == null || index < 0 || index > GameInfo.Instance.TowerFriendTeamData.Count)
        {
            return;
        }

        TeamData data = GameInfo.Instance.TowerFriendTeamData[index];

        slot.ParentGO = gameObject;
        slot.UpdateSlot(index, data, mSelectedFriendCharIndex, mSelectCharIndex);
    }

    private int GetFriendCharListCount()
    {
        return GameInfo.Instance.TowerFriendTeamData.Count;
    }

    private void ShowMyCharList()
    {
        mbSelectFriendCharList = false;

        FListChar.gameObject.SetActive(true);
        FListFriendChar.gameObject.SetActive(false);

        mListCharData.Clear();
        GameInfo.Instance.GetArenaTowerCharList(ref mListCharData, true);

        if (mListTowerTeamCharUID[mSelectedSlotIndex] != 0)
        {
            for (int i = 0; i < mListCharData.Count; i++)
            {
                if (mListCharData[i].CUID == mListTowerTeamCharUID[mSelectedSlotIndex])
                {
                    SelectChar(i);
                    break;
                }
            }
        }

        FListChar.UpdateList();
    }

    private void ShowFriendCharList()
    {
        mbSelectFriendCharList = true;

        FListChar.gameObject.SetActive(false);
        FListFriendChar.gameObject.SetActive(true);

        mListCharData.Clear();
        GameInfo.Instance.GetArenaTowerCharList(ref mListCharData, false, true);

        GameInfo.Instance.ArenaTowerFriendContainer.TryGetValue(mSelectedSlotIndex, out long curFriendCUID);
        if (curFriendCUID != 0)
        {
            for (int i = 0; i < mListCharData.Count; i++)
            {
                if (mListCharData[i].CUID == mListTowerTeamCharUID[mSelectedSlotIndex])
                {
                    SelectChar(i);
                    break;
                }
            }
        }

        FListFriendChar.UpdateList();

        if(mListCharData.Count <= 0)
        {
            LbEmpty.gameObject.SetActive(true);
        }
    }

    private void OnReqSetArenaTowerTeam(int result, PktMsgType pktMsg)
    {
        if (result != 0)
        {
            return;
        }

        LobbyUIManager.Instance.Renewal("ArenaTowerMainPanel");
        LobbyUIManager.Instance.Renewal("ArenaTowerStagePanel");

        OnClickClose();
    }
}
