using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.Purchasing;

public class UICharGiveGiftPopup : FComponent
{
	enum ePreferenceType
	{
		None,
		Good,
		VeryGood,
	}
	
	private enum SortOrder
	{
		Descending,
		Ascending,
		End,
	}
	
	[Header("UICharGiveGiftPopup")]
	[SerializeField] private FList charFavorRewardList = null;
	
	public UILabel kCharFavorLevelLabel;
	public UIGaugeUnit kFavorGauge;
	public FLocalizeText kItemLabel;
	public UIButton kGiftBtn;
	public UILabel kFavorAddLabel;
	public UILabel kGiftLabel;
	public Transform KCharTexTransform;
	public UILabel kMaterialnumLabel;
	public UISprite kOrderSpr;
	public GameObject kOrderObj;
	
	[Header("Animation")]
	public GameObject kAnimationObj;
	public UIItemListSlot kAnimItemSListSlot;
	public ParticleSystem kAnimDrainParticle;
	public ParticleSystem kAnimAppearParticle;
	public ParticleSystem kAnimOpenParticle;
	public ParticleSystem kAnimMoveParticle;
	public ParticleSystem kAnimPackageParticle;
	public UIButton kGiftAnimSkipBtn;
	
	private List<ItemData> _favorMatItemList;
	private List<ItemData> _selItemDataList = new List<ItemData>();
	private Animation _appearAnim;
	
	public List<ItemData> SelItemDataList => _selItemDataList;
	private CharData _charData;
	private GameTable.Character.Param _tableData;
	private bool _fakeReset = true;
	private int _fakeFavorLevel = -1;
	private int _fakeFavorExp = -1;
	
	private SortOrder _sortOrder = SortOrder.Descending;
	private List<GameTable.LevelUp.Param> _charFavorLevelUpList;
	
	private List<GameTable.Item.Param> _charPreferenceStep1List;
	private List<GameTable.Item.Param> _charPreferenceStep2List;
	
	private int _addValue = 0;
	private int _prevCharFavorLevel = 0;
	private int _prevCharFavorExp = 0;
	
	private Coroutine _giftAnimationCoroutine;
	private Coroutine _effectMoveCoroutine;

	public override void Awake()
	{
		base.Awake();
		
		if (charFavorRewardList != null)
		{
			charFavorRewardList.EventUpdate = _UpdateCharFavorRewardListSlot;
			charFavorRewardList.EventGetItemCount = _GetCharFavorRewardListElementCount;
		}

		if (kAnimAppearParticle != null)
		{
			_appearAnim = kAnimAppearParticle.gameObject.GetComponent<Animation>();
		}
	}
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}

	public override void OnDisable()
	{
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();
		
		UICharInfoPanel panel = LobbyUIManager.Instance.GetActiveUI("CharInfoPanel") as UICharInfoPanel;
		if (panel != null)
		{
			panel.Renewal(false);
		}

        if (RenderTargetChar.Instance.RenderPlayer)
        {
            RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Idle01, 0, eFaceAnimation.Idle01, 0);
        }
	}
	
	public override void InitComponent()
	{
		_sortOrder = SortOrder.Descending;
		
		_selItemDataList.Clear();
		_fakeReset = true;
		
		kGiftAnimSkipBtn.SetActive(false);
		
		_charFavorLevelUpList = GameInfo.Instance.GameTable.FindAllLevelUp(x => x.Group == _tableData.PreferenceLevelGroup);
		_favorMatItemList = GameInfo.Instance.ItemList.FindAll(x => x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_PREFERENCE_PRESENT);
		_favorMatItemList.Sort(StepItemSort);
		
		SetGaugeData(_charData.FavorLevel, _charData.FavorExp);
		
		charFavorRewardList.UpdateList();
	}
	
	public override void Renewal(bool bChildren = false)
	{
		base.Renewal(bChildren);
		
		_addValue = (int)eCOUNT.NONE;
		
		foreach(ItemData itemData in _selItemDataList)
		{
			_addValue += GetAddValue(itemData) * itemData.Count;
		}
		
		kOrderObj.SetActive(true);
		
		kMaterialnumLabel.textlocalize =
			FLocalizeString.Instance.GetText(218, GetSelItemDataCount(), _charData.FavorPreCnt);
		kFavorAddLabel.textlocalize = FLocalizeString.Instance.GetText(3259, _addValue);
		kGiftLabel.textlocalize = FLocalizeString.Instance.GetText(3253, 
			_charData.FavorPreCnt - GetSelItemDataCount(), GameInfo.Instance.GameConfig.DayPresentCount);
	}
	
	private void FixedUpdate()
	{
		if(RenderTargetChar.Instance.RenderPlayer != null)
		{
			RenderTargetChar.Instance.RenderPlayer.UpdateLipSync();
		}
		
		if (_charData == null)
		{
			return;
		}
		
		if (_selItemDataList.Count <= 0)
		{
			if (_fakeReset)
			{
				_fakeReset = false;
				_fakeFavorLevel = _charData.FavorLevel;
				_fakeFavorExp = _charData.FavorExp;
			}
			
			return;
		}
		
		_fakeReset = true;
		
		if (_charData.FavorExp + _addValue <= (int)_fakeFavorExp)
		{
			return;
		}
		
		_fakeFavorExp += 5;
		
		SetGaugeData(_fakeFavorLevel, _fakeFavorExp, true);
	}

	private int GetAddValue(ItemData itemData)
	{
		int result = (int)eCOUNT.NONE;
		
		if (itemData != null)
		{
			ePreferenceType preferenceType = GetPreferenceType(itemData.TableID);
			switch (preferenceType)
			{
				case ePreferenceType.VeryGood:
					result = Mathf.CeilToInt(itemData.TableData.Value * GameInfo.Instance.GameConfig.PreferenceStepRate1);
					break;
				case ePreferenceType.Good:
					result = Mathf.CeilToInt(itemData.TableData.Value * GameInfo.Instance.GameConfig.PreferenceStepRate2);
					break;
				case ePreferenceType.None:
					result = itemData.TableData.Value;
					break;
			}
		}
		
		return result;
	}
	
	private void _UpdateCharFavorRewardListSlot(int index, GameObject slotObject)
	{
		UIItemListSlot slot = slotObject.GetComponent<UIItemListSlot>();
		if (null == slot)
		{
			return;
		}
		
		ItemData data = null;
		if (0 <= index && _favorMatItemList.Count > index)
		{
			data = _favorMatItemList[index];
			
			int nNumber = (int)eCOUNT.FAVOR_ITEM_NORMAL;
			bool bEnable = false;
			if (_charPreferenceStep1List.Find(x => x.ID == data.TableID) != null)
			{
				nNumber = (int)eCOUNT.FAVOR_ITEM_VERY_GOOD;
				bEnable = true;
			}
			else if (_charPreferenceStep2List.Find(x => x.ID == data.TableID) != null)
			{
				nNumber = (int)eCOUNT.FAVOR_ITEM_GOOD;
				bEnable = true;
			}
			
			slot.SetHeart(nNumber, bEnable);
		}
		
		slot.ParentGO = this.gameObject;
		slot.UpdateSlot(UIItemListSlot.ePosType.Char_FavorLevelUpMatList, index, data);
	}

	private int _GetCharFavorRewardListElementCount()
	{
		return _favorMatItemList.Count;
	}
	
	public bool SetFavorMatItem(ItemData itemData, int inputCount = 1, bool renewal = true)
	{
		GameTable.LevelUp.Param last = _charFavorLevelUpList.LastOrDefault();
		if (last != null && last.Level <= _charData.FavorLevel)
		{
			return false;
		}
		
		if (0 < _charFavorLevelUpList.Count)
		{
			if (_charFavorLevelUpList[_charFavorLevelUpList.Count - 2].Exp <= _charData.FavorExp + _addValue)
			{
				return false;
			}
		}
		
		if (_charData.FavorPreCnt <= GetSelItemDataCount())
		{
			return false;
		}
		
		int index = _selItemDataList.FindIndex(x => x.ItemUID == itemData.ItemUID);

		if (index < 0)
		{
			ItemData newData = new ItemData(itemData.ItemUID, itemData.TableID);
			newData.Count += inputCount;
			_selItemDataList.Add(newData);
		}
		else
		{
			if (itemData.Count <= _selItemDataList[index].Count)
			{
				return false;
			}
			
			_selItemDataList[index].Count += inputCount;
		}
		
		charFavorRewardList.RefreshNotMove();

		if (renewal)
		{
			Renewal();
		}

		return true;
	}

	public int GetSelItemDataCount()
	{
		int result = (int) eCOUNT.NONE;
		
		foreach (ItemData itemData in _selItemDataList)
		{
			result += itemData.Count;
		}
		
		return result;
	}
	
	private int SetGaugeData(int level, int exp, bool fake = false)
	{
		GameTable.LevelUp.Param prev = _charFavorLevelUpList.Find(x => x.Level == level - 1);
		GameTable.LevelUp.Param cur = _charFavorLevelUpList.Find(x => x.Level == level);
		
		while (0 < cur.Exp && cur.Exp <= exp)
		{
			++level;
			prev = _charFavorLevelUpList.Find(x => x.Level == level - 1);
			cur = _charFavorLevelUpList.Find(x => x.Level == level);
		}
		
		int textColorFormat = (int)eTEXTID.WHITE_TEXT_COLOR;
		if (fake)
		{
			textColorFormat = (int)eTEXTID.GREEN_TEXT_COLOR;
		}
		
		kCharFavorLevelLabel.textlocalize = FLocalizeString.Instance.GetText(
			_charData.FavorLevel == level ? (int)eTEXTID.WHITE_TEXT_COLOR : textColorFormat, level);
		
		float rate = 1;
		string text = FLocalizeString.Instance.GetText(221);
		
		if (0 < cur.Exp)
		{
			int expTerm = cur.Exp - exp;
			int gaugeTerm = cur.Exp;
			if (prev != null)
			{
				gaugeTerm = cur.Exp - prev.Exp;
			}

			rate = (gaugeTerm - expTerm) / (float)gaugeTerm;
			text = FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT, rate * 100);
		}

		text = FLocalizeString.Instance.GetText(textColorFormat, text);
		
		kFavorGauge.InitGaugeUnit(rate);
		kFavorGauge.SetText(text);

		return level;
	}
	
	private ePreferenceType GetPreferenceType(int tableId)
	{
		if (_charPreferenceStep1List.Any(x => x.ID == tableId))
		{
			return ePreferenceType.VeryGood;
		}
		
		if (_charPreferenceStep2List.Any(x => x.ID == tableId))
		{
			return ePreferenceType.Good;
		}
		
		return ePreferenceType.None;
	}
	
	public void SetData(CharData charData, GameTable.Character.Param tableData, List<GameTable.Item.Param> step1List, List<GameTable.Item.Param> step2List)
	{
		_charData = charData;
		_tableData = tableData;
		_charPreferenceStep1List = step1List;
		_charPreferenceStep2List = step2List;
	}
	
	public void OnClick_CancelSelect()
	{
		_selItemDataList.Clear();
		SetGaugeData(_charData.FavorLevel, _charData.FavorExp);
		charFavorRewardList.RefreshNotMove();
		Renewal();
	}
	public void OnClick_GiftBtn()
	{
		if (0 < GameInfo.Instance.GameConfig.DayPresentCount)
		{
			if (_charData.FavorPreCnt <= 0)
			{
				MessageToastPopup.Show(FLocalizeString.Instance.GetText(3260));
				return;
			}
		}
		
		if (_charFavorLevelUpList.Count - 1 <= _charData.FavorLevel)
		{
			MessageToastPopup.Show(FLocalizeString.Instance.GetText(3261));
			return;
		}
		
		if (GetSelItemDataCount() <= 0)
		{
			MessageToastPopup.Show(FLocalizeString.Instance.GetText(1109));
			return;
		}
		
		_prevCharFavorLevel = _charData.FavorLevel;
		_prevCharFavorExp = _charData.FavorExp;
		
		GameInfo.Instance.Send_ReqGivePresentChar(_charData.CUID, _selItemDataList.ToArray(), OnNetGiftFavorItemResult);
	}

	public void OnClick_SelectCancel()
	{
		if (_selItemDataList.Count <= (int)eCOUNT.NONE)
		{
			return;
		}
		
		_selItemDataList.Clear();
		charFavorRewardList.RefreshNotMove();
		
		SetGaugeData(_charData.FavorLevel, _charData.FavorExp);
		Renewal();
	}

	public void OnClick_AutoSelect()
	{
		if (_charData.FavorPreCnt <= GetSelItemDataCount())
		{
			return;
		}
		
		bool exit = false;
		foreach(ItemData data in _favorMatItemList)
		{
			for (int i = 0; i < data.Count; i++)
			{
				int index = _selItemDataList.FindIndex(x => x.ItemUID == data.ItemUID);
				if (0 <= index)
				{
					if (data.Count <= _selItemDataList[index].Count)
					{
						continue;
					}
				}
				
				if (!SetFavorMatItem( data ))
				{
					exit = true;
					break;
				}
			}

			if (exit)
			{
				break;
			}
		}
		
		Renewal();
	}

	public void OnClick_ChangeOrder()
	{
		++_sortOrder;

		if (SortOrder.End <= _sortOrder)
		{
			_sortOrder = SortOrder.Descending;
		}

		switch (_sortOrder)
		{
			case SortOrder.Descending:
				kOrderSpr.spriteName = "ico_Filter2";
				break;
			case SortOrder.Ascending:
				kOrderSpr.spriteName = "ico_Filter1";
				break;
		}

		_favorMatItemList.Sort(StepItemSort);
		charFavorRewardList.RefreshNotMove();
	}
	
	private IEnumerator EffectMoveAnimation()
	{
		kAnimMoveParticle.gameObject.SetActive(true);
		var targetPosX = Math.Abs(KCharTexTransform.localPosition.x) + Math.Abs(kAnimationObj.transform.localPosition.x);
		var tweener = kAnimMoveParticle.transform.DOLocalMoveX(targetPosX * -1, 1);
		while(tweener.IsPlaying())
		{
			yield return null;
		}
		
		kAnimMoveParticle.gameObject.SetActive(false);
		kAnimMoveParticle.transform.localPosition = Vector3.zero;
		
		kAnimDrainParticle.gameObject.SetActive(true);
	}
	
	private IEnumerator GiftAnimation(ePreferenceType preferenceType)
	{
		kOrderObj.SetActive(false);
		
		// Animation Init
		RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Idle01, 0, eFaceAnimation.Idle01, 0);
		
		// Gift Button & Item List Active Off
		kGiftBtn.gameObject.SetActive(false);
		charFavorRewardList.Panel.gameObject.SetActive(false);
		
		SoundManager.Instance.PlayUISnd(86);
		
		kAnimationObj.SetActive(true);
		kAnimAppearParticle.gameObject.SetActive(true);
		_appearAnim.Play("ani_fx_ui_friendship_box_appear");
		
		// Present Animation Active On
		while(_appearAnim.isPlaying)
		{
			if (0.7 <= kAnimAppearParticle.time && kAnimAppearParticle.time < 1)
			{
				if (!kAnimOpenParticle.gameObject.activeSelf)
				{
					kAnimOpenParticle.gameObject.SetActive(true);
				}
			}
			else if (1.32 <= kAnimAppearParticle.time && kAnimAppearParticle.time < 2)
			{
				if (!kAnimPackageParticle.gameObject.activeSelf)
				{
					kAnimPackageParticle.gameObject.SetActive(true);
				}
			}
			else if (2.4 <= kAnimAppearParticle.time && _effectMoveCoroutine == null)
			{
				_effectMoveCoroutine = StartCoroutine(EffectMoveAnimation());
			}
			
			yield return null;
		}
		
		Utility.StopCoroutine(this, ref _effectMoveCoroutine);
		
		kAnimOpenParticle.gameObject.SetActive(false);
		kAnimPackageParticle.gameObject.SetActive(false);
		kAnimDrainParticle.gameObject.SetActive(false);
		kAnimationObj.SetActive(false);
		
		kGiftAnimSkipBtn.SetActive(true);
		
		// Recv Motion
		eAnimation bodyAnim = eAnimation.Accept03;
		eFaceAnimation faceAnim = eFaceAnimation.Accept03;
		switch (preferenceType)
		{
			case ePreferenceType.VeryGood:
				bodyAnim = eAnimation.Accept01;
				faceAnim = eFaceAnimation.Accept01;
				break;
			case ePreferenceType.Good:
				bodyAnim = eAnimation.Accept02;
				faceAnim = eFaceAnimation.Accept02;
				break;
		}
		
		float aniWaitTime = RenderTargetChar.Instance.RenderPlayer.PlayAni(bodyAnim, 0, faceAnim, 0);
		yield return new WaitForSeconds(aniWaitTime);
		RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Idle01, 0, eFaceAnimation.Idle01, 0);
		
		// Gauge Increase
		int favorExp = _prevCharFavorExp;
		int favorLevel = _prevCharFavorLevel;
		bool bPlayCountSound = true;
		bool bLevelUpPopupEnable = false;
		
		WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
		while (true)
		{
			if (_charData.FavorExp <= favorExp)
			{
				break;
			}
			
			if (bPlayCountSound && !bLevelUpPopupEnable)
			{
				bPlayCountSound = false;
				SoundManager.Instance.PlayUISnd(59);
			}
			
			favorExp += 5;
			
			favorLevel = SetGaugeData(favorLevel, favorExp);
			
			yield return waitForEndOfFrame;

			bPlayCountSound = bLevelUpPopupEnable = LevelUpPopupCheck(favorLevel);
			
			while (bLevelUpPopupEnable)
			{
				if (LobbyUIManager.Instance.GetActiveUI("CharFavorLevelUpPopup"))
				{
					bLevelUpPopupEnable = false;
					break;
				}
				
				yield return waitForEndOfFrame;
			}
			
			while (LobbyUIManager.Instance.GetActiveUI("CharFavorLevelUpPopup"))
			{
				yield return waitForEndOfFrame;
			}
		}
		
		SoundManager.Instance.StopUISnd();
		
		kGiftAnimSkipBtn.SetActive(false);
		
		Renewal();
		
		// Gift Button & Item List Active On
		charFavorRewardList.Panel.gameObject.SetActive(true);
		kGiftBtn.gameObject.SetActive(true);
		
		Utility.StopCoroutine(this, ref _giftAnimationCoroutine);
		
		yield return null;
	}
	
	public void OnClick_SkipBtn()
	{
		kGiftAnimSkipBtn.SetActive(false);
		
		SkipGiftAnimation();
	}
	
	private bool LevelUpPopupCheck(int favorLevel)
	{
		if (_prevCharFavorLevel < favorLevel)
		{
			SoundManager.Instance.StopUISnd();
			
			// Level Up
			UICharFavorLevelUpPopup popup = LobbyUIManager.Instance.GetUI("CharFavorLevelUpPopup") as UICharFavorLevelUpPopup;
			if (popup != null)
			{
				bool bLevelUpMax = false;
				var last = _charFavorLevelUpList.LastOrDefault();
				if (last != null)
				{
					if (last.Level <= favorLevel)
					{
						bLevelUpMax = true;

						string buffStr = string.Empty;
						GameTable.Buff.Param buffTableData = GameInfo.Instance.GameTable.FindBuff(_tableData.PreferenceBuff);
						if (buffTableData != null)
						{
							buffStr = FLocalizeString.Instance.GetText(buffTableData.Name);
						}
						
						popup.SetLevelUpMaxLabel(favorLevel, buffStr);
					}
				}

				GameTable.LevelUp.Param levelUp = _charFavorLevelUpList.Find(x => x.Level == favorLevel);
				if (levelUp != null)
				{
					popup.SetLevelUpLabel(favorLevel - 1, favorLevel, levelUp.Value1);
				}
				
				popup.SetLevelUp(bLevelUpMax);
				popup.SetUIActive(true, false);
						
				_prevCharFavorLevel = favorLevel;
			}

			return true;
		}

		return false;
	}
	
	private void SkipGiftAnimation()
	{
		SoundManager.Instance.StopUISnd();
		
		Utility.StopCoroutine(this, ref _effectMoveCoroutine);
		Utility.StopCoroutine(this, ref _giftAnimationCoroutine);

		if (RenderTargetChar.Instance.RenderPlayer != null)
		{
			RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Idle01, 0, eFaceAnimation.Idle01, 0);
		}
		
		kAnimationObj.SetActive(false);
		kAnimDrainParticle.gameObject.SetActive(false);
		kAnimAppearParticle.gameObject.SetActive(false);
		kAnimOpenParticle.gameObject.SetActive(false);
		kAnimMoveParticle.gameObject.SetActive(false);
		kAnimPackageParticle.gameObject.SetActive(false);
	
		kAnimMoveParticle.transform.localPosition = Vector3.zero;
		
		kGiftAnimSkipBtn.SetActive(false);
		
		SetGaugeData(_charData.FavorLevel, _charData.FavorExp);

		LevelUpPopupCheck(_charData.FavorLevel);
		
		Renewal();
		
		// Gift Button & Item List Active On
		charFavorRewardList.Panel.gameObject.SetActive(true);
		kGiftBtn.gameObject.SetActive(true);
	}
	
	private void OnNetGiftFavorItemResult(int result, PktMsgType pktmsg)
	{
		if (result != 0)
		{
			return;
		}

		PktInfoGivePresentCharAck PktInfoGivePresentChar = pktmsg as PktInfoGivePresentCharAck;
		if (PktInfoGivePresentChar == null)
		{
			return;
		}

		GameTable.LevelUp.Param levelUpParam = GameInfo.Instance.GameTable.FindLevelUp( x => x.Group == _charData.TableData.PreferenceLevelGroup && x.Level == _charData.FavorLevel );
		if ( levelUpParam != null && levelUpParam.Exp < 0 ) {
			int selectIndex = -1;
			for ( int i = 0; i < GameInfo.Instance.UserData.ArrFavorBuffCharUid.Length; i++ ) {
				long cuid = GameInfo.Instance.UserData.ArrFavorBuffCharUid[i];
				if ( 0 < cuid ) {
					CharData charData = GameInfo.Instance.GetCharData( cuid );
					if ( charData == null ) {
						continue;
					}

					levelUpParam = GameInfo.Instance.GameTable.FindLevelUp( x => x.Group == _charData.TableData.PreferenceLevelGroup && x.Level == charData.FavorLevel );
					if ( levelUpParam == null ) {
						continue;
					}

					if ( levelUpParam.Exp < 0 ) {
						continue;
					}
				}

				selectIndex = i;
				break;
			}

			if ( 0 <= selectIndex ) {
				GameInfo.Instance.Send_ReqChangePreferenceNum( _charData.CUID, selectIndex, null );
			}
		}

		_favorMatItemList = GameInfo.Instance.ItemList.FindAll(x => x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_PREFERENCE_PRESENT);
		_favorMatItemList.Sort(StepItemSort);
		
		// Item Data Copy & Update
		ePreferenceType preferenceType = ePreferenceType.None;
		ItemData bestItemData = _selItemDataList.FirstOrDefault();
		foreach (ItemData itemData in _selItemDataList)
		{
			ePreferenceType tempPreferenceType = GetPreferenceType(itemData.TableID);
			if (preferenceType < tempPreferenceType)
			{
				preferenceType = tempPreferenceType;
				bestItemData = itemData;
			}
		}
		
		kAnimItemSListSlot.UpdateSlot(UIItemListSlot.ePosType.Char_FavorLevelUpMatList, 0, bestItemData);
		
		_selItemDataList.Clear();
		
		charFavorRewardList.UpdateList(true, false);
		
		SetGaugeData(_prevCharFavorLevel, _prevCharFavorExp);
		
		_giftAnimationCoroutine = StartCoroutine(GiftAnimation(preferenceType));
	}
	
	private int StepItemSort(ItemData f, ItemData b)
	{
		GameTable.Item.Param stepFir01 = _charPreferenceStep1List.Find(x => x.ID == f.TableID);
		GameTable.Item.Param stepSec01 = _charPreferenceStep1List.Find(x => x.ID == b.TableID);
		GameTable.Item.Param stepFir02 = _charPreferenceStep2List.Find(x => x.ID == f.TableID);
		GameTable.Item.Param stepSec02 = _charPreferenceStep2List.Find(x => x.ID == b.TableID);

		int stepFirPoint = 1;
		if (stepFir01 != null)
		{
			stepFirPoint = 3;
		}
		else if (stepFir02 != null)
		{
			stepFirPoint = 2;
		}
			
		int stepSecPoint = 1;
		if (stepSec01 != null)
		{
			stepSecPoint = 3;
		}
		else if (stepSec02 != null)
		{
			stepSecPoint = 2;
		}

		int prev = 1;
		int next = -1;
		if (_sortOrder == SortOrder.Descending)
		{
			prev = -1;
			next = 1;
		}
		
		if (stepFirPoint == stepSecPoint)
		{
			if (f.TableData.Grade > b.TableData.Grade)
			{
				return prev;
			}

			if (f.TableData.Grade < b.TableData.Grade)
			{
				return next;
			}

			if (f.TableData.Grade == b.TableData.Grade)
			{
				if (f.TableID > b.TableID)
				{
					return prev;
				}

				if (f.TableID < b.TableID)
				{
					return next;
				}
			}
		}
		
		if (stepFirPoint > stepSecPoint)
		{
			return prev;
		}

		if (stepFirPoint < stepSecPoint)
		{
			return next;
		}

		return 0;
	}
	
	public void OnClick_BackBtn()
	{
		FavorExit();
	}
	
	public bool FavorExit()
	{
		bool result = false;
		if (kGiftAnimSkipBtn.gameObject.activeSelf)
		{
			if (_giftAnimationCoroutine != null)
			{
				SkipGiftAnimation();
			}
		}
		else
		{
			if (_giftAnimationCoroutine == null)
			{
				SetUIActive(false);
				result = true;
			}
		}

		return result;
	}
}
