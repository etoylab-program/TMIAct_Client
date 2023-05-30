using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class UICostumeDyePopup : FComponent
{
    [Header("UICostumeDyePopup")]
    [SerializeField] private float changeTimeRate = 1;
    [SerializeField] private float maxSizeRate = 2;
    [SerializeField] private float maxHeightRate = 0.68f;
    [SerializeField] private float dyeCompleteWaitSec = 1;
    
    public ParticleSystem kShowerStartParticle;
    public ParticleSystem kShowerEndParticle;
    public GameObject kCurtainObj;
    public UISlider kSlider;
    
    [Header("Root1")]
    public GameObject kRoot1Obj;
    public UILabel kRoot1NameLabel;
    public UIItemListSlot kRoot1ItemSlot;
    public UIButton kRoot1OkBtn;
    public UISprite kRoot1OkDimdSpr;
    public List<FToggle> kRoot1LockList;
    public List<UICostumeDyeSlot> kRoot1SlotList;
    public UILabel kDescriptionLabel;
    public FToggle kRGBToggle;
    public UILabel kRGBDescriptionLabel;
    
    [Header("Root2")]
    public GameObject kRoot2Obj;
    public UIButton kRoot2ResetBtn;
    public List<FToggle> kRoot2LockList;
    public List<UISprite> kRoot2LockSprList;
    public List<UICostumeDyeSlot> kRoot2SlotList;
    public UILabel kRoot2ResetBtnLabel;
    public Animation kRoot2CompleteAnim;

    private int dyeItemCount
    {
        get
        {
            int itemCount = 0;
        
            ItemData itemData = GameInfo.Instance.GetItemData(GameInfo.Instance.GameConfig.RandomDyeingMat);
            if (itemData != null)
                itemCount = itemData.Count;
        
            return itemCount;
        }
    }
    
    private const byte LockFlagMaxCount = 3;
    
    private byte _lockFlag;
    private byte _lockFlagMax;
    
    private bool _bContinue;
    private bool _bRestore;
    private bool _bRoot1Enable = true;
    
    private bool _bClearCount = true;
    private int _matCount = 0;

    private Color[] _originColors = new Color[3];
    private Color[] _randomColors = new Color[9];
    private int[] _selects = new int[3];
    private GameTable.Costume.Param _costume;
    
    private float _originSize;
    private float _originHeight;
    private Camera _renderTargetCamera = null;
    
    private bool _bItemBuyRenewal;
    private Coroutine _completeCoroutine;
    private Coroutine _completeAnimCoroutine;
    private CharData _charData;

    private bool _bResetClick;
    private bool _bDyePartPeeking;
    private readonly Color[] _dyePartColors = { Color.red, Color.green, Color.blue };
    private readonly DyeingData _dyeingPartData = new DyeingData(new PktInfoConPosCharDetail.CostumeDyeing
    {
        isFirstDyeing_ = false,
        part1_ = new PktInfoColor { red_ = byte.MaxValue, green_ = byte.MinValue, blue_ = byte.MinValue },
        part2_ = new PktInfoColor { red_ = byte.MinValue, green_ = byte.MaxValue, blue_ = byte.MinValue },
        part3_ = new PktInfoColor { red_ = byte.MinValue, green_ = byte.MinValue, blue_ = byte.MaxValue },
    });
    
    public override void Awake()
    {
        base.Awake();

        foreach (FToggle toggle in kRoot1LockList)
        {
            toggle.EventCallBackToggle = OnEventTabSelectComponent;
        }
        
        foreach (FToggle toggle in kRoot2LockList)
        {
            toggle.EventCallBackToggle = OnEventTabSelectComponent;
        }
        
        kSlider.onChange.Add(new EventDelegate(OnValueChange));
        
        kRGBToggle.EventCallBack = OnEventTabSelect;
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);
        
        kRoot1Obj.SetActive(_bRoot1Enable);
        
        if (_bRoot1Enable)
        {
            kRoot2Obj.SetActive(!_bRoot1Enable);
            kDescriptionLabel.gameObject.SetActive(!_bDyePartPeeking);
            kRGBDescriptionLabel.gameObject.SetActive(_bDyePartPeeking);
            kRoot1ItemSlot.kInactiveSpr.gameObject.SetActive(_bDyePartPeeking);
            
            if (_bDyePartPeeking)
            {
                for (int i = 0; i < _dyePartColors.Length; i++)
                {
                    kRoot1SlotList[i].SetColor(_dyePartColors[i]);
                    kRoot1SlotList[i].SetRGBBtn(true);
                }

                for (int i = 0; i < kRoot1LockList.Count; i++)
                {
                    kRoot1LockList[i].gameObject.SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < _originColors.Length; i++)
                {
                    kRoot1SlotList[i].SetColor(_originColors[i]);
                    kRoot1SlotList[i].SetRGBBtn(false);
                }
            
                for (int i = 0; i < kRoot1LockList.Count; i++)
                {
                    kRoot1LockList[i].gameObject.SetActive(true);
                    
                    int nSelect = 0;
                    int flag = 1 << i;
                    if ((_lockFlag & flag) == flag)
                    {
                        nSelect = 1;
                    }
                
                    kRoot1LockList[i].SetToggle(nSelect, SelectEvent.Code);
                }
            }
            
            kRoot1OkBtn.enabled = !_bDyePartPeeking;
            kRoot1OkDimdSpr.gameObject.SetActive(_bDyePartPeeking);
        }
        else if (_bItemBuyRenewal == false)
        {
            for (int i = 0; i < _randomColors.Length; i++)
            {
                int part = kRoot2SlotList[i].Part;
                if (_selects.Length <= part)
                {
                    continue;
                }

                int index = kRoot2SlotList[i].Index;
                int flag = 1 << part;
                if ((_lockFlag & flag) == flag)
                {
                    kRoot2SlotList[i].SetColor(_originColors[part]);
                    kRoot2SlotList[i].SetLock(true);
                    kRoot2SlotList[i].SetActive(index == 0);
                    kRoot2SlotList[i].SetSlotActive(false);

                    if (_bResetClick && kRoot2SlotList[i].gameObject.activeSelf)
                    {
                        kRoot2SlotList[i].PlayAnimation();
                    }
                    continue;
                }
                
                kRoot2SlotList[i].SetColor(_randomColors[i]);
                kRoot2SlotList[i].SetLock(false);
                kRoot2SlotList[i].SetActive(true);
                kRoot2SlotList[i].SetSlotActive(_selects[part] == index);
                
                if (_bResetClick)
                {
                    kRoot2SlotList[i].PlayAnimation();
                }

                if (_selects[part] == index)
                {
                    SetPlayerColor(part, _randomColors[i]);
                }
            }
            
            for (int i = 0; i < kRoot2LockList.Count; i++)
            {
                int nSelect = 0;
                int flag = 1 << i;
                if ((_lockFlag & flag) == flag)
                {
                    nSelect = 1;
                    _selects[i] = -1;
                }
                
                kRoot2LockList[i].SetToggle(nSelect, SelectEvent.Code);
            }
        }

        _bItemBuyRenewal = false;
        
        CheckCount();
    }

    public override void OnEnable()
    {
        RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Idle01, 0, eFaceAnimation.Idle01, 0);
        _bDyePartPeeking = false;
        _bResetClick = false;
        
        kShowerStartParticle.gameObject.SetActive(false);
        kShowerEndParticle.gameObject.SetActive(false);
        
        kRoot2CompleteAnim.gameObject.SetActive(false);
        
        TopPanelBackButtonActive(false);
        
        _renderTargetCamera = RenderTargetChar.Instance.kRenderTargetCharCamera;
        _originSize = _renderTargetCamera.orthographicSize;
        _originHeight = _renderTargetCamera.transform.localPosition.y;
        
        ResetSelect();

        kRoot1NameLabel.textlocalize = FLocalizeString.Instance.GetText(_costume.Name);

        MyDyeingData dyeingData = GameInfo.Instance.GetDyeingData(_costume.ID);
        if (dyeingData != null)
        {
			int costumeFlag = 0;

			UICharInfoPanel charInfoPanel = LobbyUIManager.Instance.GetActiveUI<UICharInfoPanel>("CharInfoPanel");
			if(charInfoPanel)
			{
				UICharInfoTabCostumePanel costumePanel = charInfoPanel.kTabList[(int)UICharInfoPanel.eCHARINFOTAB.COSTUME] as UICharInfoTabCostumePanel;
				if (costumePanel)
				{
					costumeFlag = costumePanel.GetCostumeStateFlag();
				}
			}

			RenderTargetChar.Instance.SetCostumeBody(_costume.ID, _costume.ColorCnt, costumeFlag, dyeingData.DyeingData);
            RenderTargetChar.Instance.RenderPlayer.aniEvent.GetBones();
            
            _lockFlag = dyeingData.LockFlag;
    
            for (int i = 0; i < _originColors.Length; i++)
            {
                Color color = dyeingData.DyeingData.IsFirstDyeing ? 
                    RenderTargetChar.Instance.RenderPlayer.costumeUnit.CostumeBody.FindCostumeBaseColor(_costume.ID, i) : dyeingData.DyeingData.PartsColorList[i];
                
                _originColors[i] = color;
                _lockFlagMax |= (byte)(1 << i);
            }
        }
        
        GameTable.Item.Param itemTableData = GameInfo.Instance.GameTable.FindItem(GameInfo.Instance.GameConfig.RandomDyeingMat);
        if (itemTableData != null)
        {
            kRoot1ItemSlot.ParentGO = gameObject;
            kRoot1ItemSlot.UpdateSlot(UIItemListSlot.ePosType.Dye_MatItemSlot, (int)eCOUNT.NONE, itemTableData);
        }
        
        _charData = GameInfo.Instance.GetCharData(RenderTargetChar.Instance.RenderPlayer.Uid);
        
        OnValueChange();
        
        base.OnEnable();
    }

	public override void OnDisable() {
		if ( AppMgr.Instance.IsQuit ) {
			return;
		}

		base.OnDisable();

		TopPanelBackButtonActive( true );
		kSlider.value = 1;

		SetRenderTargetLocalPos();

		UICharInfoPanel panel = LobbyUIManager.Instance.GetUI( "CharInfoPanel" ) as UICharInfoPanel;
		if ( panel != null ) {
			panel.CostumeRestore( _bRestore );
		}

		_bRestore = false;

        if ( RenderTargetChar.Instance.RenderPlayer ) {
            RenderTargetChar.Instance.RenderPlayer.PlayAni( eAnimation.Idle01, 0, eFaceAnimation.Idle01, 0 );
        }
	}

	private void Update()
    {
        if (Mathf.Abs(AppMgr.Instance.CustomInput.Delta) > 0.1f)
        {
            kSlider.value -= AppMgr.Instance.CustomInput.Delta * 0.01f;
        }
    }
    
    private bool OnEventTabSelect(int nSelect, SelectEvent type)
    {
        if (type != SelectEvent.Click)
        {
            return false;
        }
        
        _bDyePartPeeking = !_bDyePartPeeking;

        DyeingData dyeingData = _dyeingPartData;
        if (!_bDyePartPeeking)
        {
            MyDyeingData myDyeingData = GameInfo.Instance.GetDyeingData(_costume.ID);
            if (myDyeingData != null)
            {
                dyeingData = myDyeingData.DyeingData;
                if (myDyeingData.DyeingData.IsFirstDyeing)
                {
                    dyeingData.PartsColorList.Clear();
                    dyeingData.PartsColorList.AddRange(_originColors);
                }
            }
        }

        LobbyPlayer renderPlayer = RenderTargetChar.Instance.RenderPlayer;
        if (renderPlayer != null && renderPlayer.costumeUnit != null && renderPlayer.costumeUnit.CostumeBody != null)
        {
            AniEvent aniEvent = renderPlayer.aniEvent;
            if (aniEvent != null)
            {
                for (int i = 0; i < dyeingData.PartsColorList.Count; i++)
                {
                    aniEvent.SetPartsColor(i, dyeingData.PartsColorList[i]);
                }
            }
        }

        Renewal();
        
        return true;
    }
    
    private bool OnEventTabSelectComponent(int nSelect, SelectEvent type, FToggle toggle)
    {
        if (_bDyePartPeeking)
        {
            return false;
        }
        
        if (type == SelectEvent.Enable || toggle == null || toggle.transform == null || toggle.transform.parent == null)
        {
            return false;
        }
        
        Match match = Regex.Match(toggle.transform.parent.name, "[0-9]");
        if (match.Length <= 0)
        {
            return false;
        }
        
        int index = Convert.ToInt32(match.Value) - 1;
        if (index < 0 || _selects.Length <= index)
        {
            return false;
        }
        
        kRoot2LockSprList[index].gameObject.SetActive(nSelect == 1);
        
        if (type == SelectEvent.Code)
        {
            return true;
        }
            
        byte flag = (byte)(1 << index);
        if (nSelect == 0)
        {
            _lockFlag ^= flag;
        }
        else
        {
            _lockFlag |= flag;
        }
        
        if (_bRoot1Enable)
        {
            Renewal();
        }
        else
        {
            CheckCount();
        }
        
        return true;
    }
    
    private void OnValueChange()
    {
        SetRenderTargetLocalPos((maxSizeRate - 1) * kSlider.value + 1, (1 - maxHeightRate) * kSlider.value);
    }

    private void ResetSelect()
    {
        for (int i = 0; i < _selects.Length; i++)
        {
            _selects[i] = 0;
        }
    }
    
    private bool ItemAndLockCheck()
    {
        if (_lockFlag == _lockFlagMax)
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(1745), null);
            return false;
        }
    
        if (!_bClearCount)
        {
            _bItemBuyRenewal = true;
            ItemBuyMessagePopup.ShowItemBuyPopup(GameInfo.Instance.GameConfig.RandomDyeingMat, dyeItemCount, _matCount);
            return false;
        }
    
        return true;
    }

    private void TopPanelBackButtonActive(bool active)
    {
        UITopPanel topPanel = LobbyUIManager.Instance.GetUI("TopPanel") as UITopPanel;
        if (topPanel)
        {
            topPanel.SetBackBtnActive(active);
        }
    }
    
    private bool CompareDiffLock()
    {
        bool result = false;
        MyDyeingData dyeingData = GameInfo.Instance.GetDyeingData(_costume.ID);
        if (dyeingData != null)
        {
            if (dyeingData.LockFlag != _lockFlag)
            {
                result = true;
            }
        }
        return result;
    }
    
    private void CheckCount()
    {
        // 최종 비용 = GameConfig.RandomDyeingCost + (잠금 개수 x GameConfig.RandomDyeingRockCost)
        int lockCount = GetLockCount(_lockFlag);
        _matCount = GameInfo.Instance.GameConfig.RandomDyeingCost + 
                    (lockCount < LockFlagMaxCount ? lockCount : LockFlagMaxCount - 1) * GameInfo.Instance.GameConfig.RandomDyeingRockCost;
        _bClearCount = _matCount <= dyeItemCount;
    
        eTEXTID textId = _bClearCount ? eTEXTID.GREEN_TEXT_COLOR : eTEXTID.RED_TEXT_COLOR;
        string strMatCount = string.Format(
            FLocalizeString.Instance.GetText(
                (int)textId, string.Format(FLocalizeString.Instance.GetText(236), _matCount, dyeItemCount)));
    
        kRoot1ItemSlot.SetCountLabel(strMatCount);
    
        strMatCount = string.Format(
            FLocalizeString.Instance.GetText(
                (int)textId, string.Format(FLocalizeString.Instance.GetText(213), _matCount)));
    
        kRoot2ResetBtnLabel.textlocalize = strMatCount;
    
        kRoot1OkBtn.isEnabled = _lockFlag != _lockFlagMax;
        kRoot2ResetBtn.isEnabled = _lockFlag != _lockFlagMax;
        if (_lockFlag == _lockFlagMax)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1745));
        }
    }
    
    private IEnumerator Root2Change()
    {
        CheckCount();
        
        kShowerStartParticle.gameObject.SetActive(true);
        kRoot2CompleteAnim.gameObject.SetActive(false);
        kCurtainObj.SetActive(true);
        
        SoundManager.Instance.PlayUISnd(83);
    
        float changeTime = kShowerStartParticle.main.duration * changeTimeRate;
    
        while (kShowerStartParticle.time < changeTime)
            yield return null;
        
        Renewal();
        
        float fAnimDelay = 0;
        if (!_bResetClick)
        {
            fAnimDelay = UIAni[AniNameList[2]].length;
            UIAni.Play(AniNameList[2]);
        }
        else
        {
            _bResetClick = false;
        }

        yield return new WaitForSeconds(fAnimDelay);
        
        kCurtainObj.SetActive(false);
    
        while (kShowerStartParticle.isPlaying)
            yield return null;
    
        kShowerStartParticle.gameObject.SetActive(false);
    }

    private int GetLockCount(byte lockFlag)
    {
        int result = 0;
        for (int i = 0; i < LockFlagMaxCount; i++)
        {
            int flag = 1 << i;
            if ((lockFlag & flag) == flag)
            {
                ++result;
            }
        }
        return result <= LockFlagMaxCount ? result : LockFlagMaxCount;
    }
    
    private void SetRenderTargetLocalPos(float rate = 1, float min = 0)
    {
        if (_renderTargetCamera == null)
            return;
    
        _renderTargetCamera.orthographicSize = _originSize * rate;
        
        Vector3 localPos = _renderTargetCamera.transform.localPosition;
        _renderTargetCamera.transform.localPosition = new Vector3(localPos.x, _originHeight - min, localPos.z);
    }
    
    private void SetPlayerColor(int index, Color color)
    {
        LobbyPlayer player = RenderTargetChar.Instance.RenderPlayer;
        player.aniEvent.SetMaskTexture(player.costumeUnit.CostumeBody.TexPartsColor);
        player.aniEvent.SetPartsColor(index, color);
    }

    public override void OnClickClose()
    {
        if (kCurtainObj.activeSelf)
        {
            return;
        }

        if (_bRoot1Enable)
        {
            if (CompareDiffLock())
            {
                GameInfo.Instance.Send_ReqCostumeDyeingLock(_costume.ID, _lockFlag, OnNetDyeLockAndExitResult);
            }
            else
            {
                base.OnClickClose();
            }
        }
        else
        {
            if (!LobbyUIManager.Instance.GetActiveUI("MessagePopup"))
            {
                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(1744), null);
                return;
            }

            base.OnClickClose();
        }
    }


    public void SetCostume(GameTable.Costume.Param costume)
    {
        _costume = costume;
    }

    public void DyeingContinue()
    {
        _bContinue = _bRestore = true;
        _bRoot1Enable = false;
        _bItemBuyRenewal = false;
        
        kRoot1Obj.SetActive(_bRoot1Enable);
        kRoot2Obj.SetActive(!_bRoot1Enable);
    }
    
    public void SetRandomColor(Color[] randomColors)
    {
        _randomColors = randomColors;
    }
    
    public void OnClick_Select(UICostumeDyeSlot slot)
    {
        if (slot == null || _selects.Length <= slot.Part)
        {
            return;
        }

        int flag = 1 << slot.Part;
        if ((_lockFlag & flag) == flag)
        {
            return;
        }
        
        UICostumeDyeSlot prevSlot = kRoot2SlotList.Find(x => x.Part == slot.Part && x.Index == _selects[slot.Part]);
        if (prevSlot == null)
        {
            return;
        }
        
        prevSlot.SetSlotActive(false);
        slot.SetSlotActive(true);
        
        SetPlayerColor(slot.Part, _randomColors[slot.Part * 3 + slot.Index]);
        
        _selects[slot.Part] = slot.Index;
    }

    public void OnClick_Root1OkBtn()
    {
        if (!ItemAndLockCheck() || !_bRoot1Enable)
        {
            return;
        }
        
        MessagePopup.YN(eTEXTID.USE, FLocalizeString.Instance.GetText(1747), () =>
        {
            if (CompareDiffLock())
            {
                GameInfo.Instance.Send_ReqCostumeDyeingLock(_costume.ID, _lockFlag, OnNetDyeLockAndRandomResult);
            }
            else
            {
                GameInfo.Instance.Send_ReqRandomCostumeDyeing(_costume.ID, OnNetDyeRandomColorResult);
            }
        });
    }
    
    private void OnNetDyeLockAndResult(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }
        
        PktInfoCostumeDyeingLock pktInfoCostumeLock = pktmsg as PktInfoCostumeDyeingLock;
        if (pktInfoCostumeLock == null)
        {
            return;
        }
        
        _lockFlag = pktInfoCostumeLock.lockFlag_;
        
        if (_costume.ID != _charData.EquipCostumeID || _costume.ColorCnt != _charData.CostumeColor)
        {
            GameInfo.Instance.Send_ReqSetMainCostumeChar(_charData.CUID, _costume.ID, _costume.ColorCnt,
                0, true, OnSetMainCostumeChar);
        }
        else
        {
            Utility.StopCoroutine(this, ref _completeCoroutine);
            _completeCoroutine = StartCoroutine(SetCostumeComplete(true));
        }
    }
    
    private void OnNetDyeLockAndRandomResult(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }
        
        PktInfoCostumeDyeingLock pktInfoCostumeLock = pktmsg as PktInfoCostumeDyeingLock;
        if (pktInfoCostumeLock == null)
        {
            return;
        }
        
        _lockFlag = pktInfoCostumeLock.lockFlag_;
        
        GameInfo.Instance.Send_ReqRandomCostumeDyeing(_costume.ID, OnNetDyeRandomColorResult);
    }
    
    private void OnNetDyeLockAndExitResult(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }
        
        PktInfoCostumeDyeingLock pktInfoCostumeLock = pktmsg as PktInfoCostumeDyeingLock;
        if (pktInfoCostumeLock == null)
        {
            return;
        }
        
        _lockFlag = pktInfoCostumeLock.lockFlag_;

        //SetUIActive(false);
        base.OnClickClose();
    }
    
    private void OnNetDyeRandomColorResult(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
        
        PktInfoRandomCostumeDyeing pktInfoRandomCostumeDyeing = pktmsg as PktInfoRandomCostumeDyeing;
        if (pktInfoRandomCostumeDyeing == null)
        {
            return;
        }

        LobbyPlayer lobbyPlayer = RenderTargetChar.Instance.RenderPlayer;
        if (lobbyPlayer != null)
        {
            lobbyPlayer.PlayAni(eAnimation.Idle01, 0, eFaceAnimation.Idle01, 0);
        }
        
        GameInfo.Instance.UserData.DyeingCostumeId = pktInfoRandomCostumeDyeing.costume_.tableID_;
    
        foreach (PktInfoUserCostumeColor.Piece v in pktInfoRandomCostumeDyeing.userCostumeColor_.infos_)
        {
            int index = v.partIndex_ * LockFlagMaxCount + v.colorIndex_;
            if (_randomColors.Length <= index)
            {
                continue;
            }
        
            _randomColors[index] = new Color32(v.color_.red_, v.color_.green_, v.color_.blue_, Byte.MaxValue);
        }
        
        _bRoot1Enable = false;
        
        ResetSelect();
        StartCoroutine(Root2Change());
    }

    public void OnClick_Root2OkBtn()
    {
        MessagePopup.YN(eTEXTID.USE, FLocalizeString.Instance.GetText(1746), () =>
        {
            GameInfo.Instance.Send_ReqSetCostumeDyeing(_selects, OnNetDyeSetColorResult);
        });
    }
    
    private void OnNetDyeSetColorResult(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
    
        PktInfoCostume.Piece pktInfoCostume = pktmsg as PktInfoCostume.Piece;
        if (pktInfoCostume == null)
        {
            return;
        }
        
        _originColors[0] = new Color32(pktInfoCostume.part1_.red_, pktInfoCostume.part1_.green_, pktInfoCostume.part1_.blue_, Byte.MaxValue);
        _originColors[1] = new Color32(pktInfoCostume.part2_.red_, pktInfoCostume.part2_.green_, pktInfoCostume.part2_.blue_, Byte.MaxValue);
        _originColors[2] = new Color32(pktInfoCostume.part3_.red_, pktInfoCostume.part3_.green_, pktInfoCostume.part3_.blue_, Byte.MaxValue);
        
        if (_lockFlag != pktInfoCostume.lockFlag_)
        {
            GameInfo.Instance.Send_ReqCostumeDyeingLock(_costume.ID, _lockFlag, OnNetDyeLockAndResult);
        }
        else
        {
            if (_costume.ID != _charData.EquipCostumeID || _costume.ColorCnt != _charData.CostumeColor)
            {
                GameInfo.Instance.Send_ReqSetMainCostumeChar(_charData.CUID, _costume.ID, _costume.ColorCnt,
                    0, true, OnSetMainCostumeChar);
            }
            else
            {
                Utility.StopCoroutine(this, ref _completeCoroutine);
                _completeCoroutine = StartCoroutine(SetCostumeComplete(true));
            }

			UICharInfoPanel charInfoPanel = LobbyUIManager.Instance.GetActiveUI("CharInfoPanel") as UICharInfoPanel;
			if (charInfoPanel != null)
			{
				UIValue.Instance.SetValue( UIValue.EParamType.CharCostumeID, _charData.EquipCostumeID );

				charInfoPanel.kTabList[(int)UICharInfoPanel.eCHARINFOTAB.COSTUME].InitComponent();
				charInfoPanel.kTabList[(int)UICharInfoPanel.eCHARINFOTAB.COSTUME].Renewal();

				OnValueChange();
			}
		}
    }
    
    private IEnumerator SetCostumeComplete(bool bPlayAni)
    {
        kCurtainObj.SetActive(true);

        Utility.StopCoroutine(this, ref _completeAnimCoroutine);
        _completeAnimCoroutine = StartCoroutine(CompleteAnim(bPlayAni));
        
        for (int i = 0; i < _originColors.Length; i++)
        {
            SetPlayerColor(i, _originColors[i]);
        }
        
        _bContinue = false;
        _bRoot1Enable = true;
        
        GameInfo.Instance.UserData.DyeingCostumeId = 0;
        
        kCurtainObj.SetActive(false);
        
        Renewal();
        
        while(kShowerEndParticle.isPlaying)
        {
            yield return null;
        }
        
        kShowerEndParticle.gameObject.SetActive(false);
    }
    
    private IEnumerator CompleteAnim(bool bPlayAni)
    {
        float delay = 0;
        if (RenderTargetChar.Instance.RenderPlayer != null)
        {
            delay = RenderTargetChar.Instance.RenderPlayer.aniEvent.GetAniClip(eAnimation.Lobby_Costume).length;

            if (bPlayAni)
            {
                delay = RenderTargetChar.Instance.RenderPlayer.PlayAni(eAnimation.Lobby_Costume, 0, eFaceAnimation.Costume, 0);
            }
        }
        
        bool refreshLobbyChar = false;
        for(int i = 0; i < GameInfo.Instance.UserData.ArrLobbyBgCharUid.Length; i++)
        {
            if(GameInfo.Instance.UserData.ArrLobbyBgCharUid[i] == _charData.CUID)
            {
                refreshLobbyChar = true;
                break;
            }
        }
        
        if(refreshLobbyChar)
        {
            Lobby.Instance.ChangeMainChar();
            Lobby.Instance.SetShowLobbyPlayer();
        }
        
        VoiceMgr.Instance.PlayChar(eVOICECHAR.CostumeChange, _charData.TableID);
        
        kShowerEndParticle.gameObject.SetActive(true);
        kRoot2CompleteAnim.gameObject.SetActive(true);
        
        SoundManager.Instance.PlayUISnd(85);
        
        yield return new WaitForSeconds(delay);
        
        kRoot2CompleteAnim.gameObject.SetActive(false);
    }

    private void OnSetMainCostumeChar(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        UICharInfoPanel charInfoPanel = LobbyUIManager.Instance.GetActiveUI("CharInfoPanel") as UICharInfoPanel;
        if (charInfoPanel == null)
        {
            return;
        }
        
        UIValue.Instance.SetValue(UIValue.EParamType.CharCostumeID, _costume.ID);

        charInfoPanel.kTabList[(int)UICharInfoPanel.eCHARINFOTAB.COSTUME].InitComponent();
        charInfoPanel.kTabList[(int)UICharInfoPanel.eCHARINFOTAB.COSTUME].Renewal();
        OnValueChange();
        
        Utility.StopCoroutine(this, ref _completeCoroutine);
        _completeCoroutine = StartCoroutine(SetCostumeComplete(false));
    }

    public void OnClick_Root2ResetBtn()
    {
        if (!ItemAndLockCheck())
        {
            return;
        }
        
        kShowerEndParticle.Stop();
        
        MessagePopup.YN(eTEXTID.USE, FLocalizeString.Instance.GetText(1757), () =>
        {
            GameInfo.Instance.Send_ReqSetCostumeDyeing(_selects, OnNetDyeReSetColorResult);
        });
    }
    
    private void OnNetDyeReSetColorResult(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
        
        PktInfoCostume.Piece pktInfoCostume = pktmsg as PktInfoCostume.Piece;
        if (pktInfoCostume == null)
        {
            return;
        }

        _bResetClick = true;
        
        _originColors[0] = new Color32(pktInfoCostume.part1_.red_, pktInfoCostume.part1_.green_, pktInfoCostume.part1_.blue_, Byte.MaxValue);
        _originColors[1] = new Color32(pktInfoCostume.part2_.red_, pktInfoCostume.part2_.green_, pktInfoCostume.part2_.blue_, Byte.MaxValue);
        _originColors[2] = new Color32(pktInfoCostume.part3_.red_, pktInfoCostume.part3_.green_, pktInfoCostume.part3_.blue_, Byte.MaxValue);

        for (int i = 0; i < _originColors.Length; i++)
        {
            SetPlayerColor(i, _originColors[i]);
        }
        
        _bContinue = false;
        
        GameInfo.Instance.UserData.DyeingCostumeId = 0;
        
        Lobby.Instance.ChangeMainChar();
        Lobby.Instance.SetShowLobbyPlayer();

        if (_lockFlag != pktInfoCostume.lockFlag_)
        {
            GameInfo.Instance.Send_ReqCostumeDyeingLock(_costume.ID, _lockFlag, OnNetDyeLockAndRandomResult);
        }
        else
        {
            GameInfo.Instance.Send_ReqRandomCostumeDyeing(_costume.ID, OnNetDyeRandomColorResult);
        }
    }

    public void OnClick_BackBtn()
    {
        OnClickClose();
    }
}
