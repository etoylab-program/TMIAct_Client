using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGachaResultPopup : FComponent
{
    public UILabel kNewCardLabel;
    public UIButton kBackBtn;
    public UIButton kNextBtn;
    public UIButton kCloseBtn;
    public UIButton kReBtn;
    public GameObject kMileage;
    public GameObject kDesire;
    public UIGoodsUnit kPointGoodsUnit;
    public UIGoodsUnit kDesireGoodsUnit;
    public GameObject kDesirePointMaxObj;
    public List<UIItemListSlot> kItemList;
    private List<bool> _newlist = new List<bool>();

    private GameObject DesireEff;
    private bool _bIsActiveEff = false;

    
	public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        bool bNew = false;

        if (DesireEff == null)
        {
            UIGoodsPopup goodsPopup = LobbyUIManager.Instance.GetUI<UIGoodsPopup>("GoodsPopup");
            if (goodsPopup != null)
            {
                DesireEff = ResourceMgr.Instance.CreateFromAssetBundle("effect", "Effect/UI/prf_fx_ui_desiregacha_marble.prefab");

                DesireEff.transform.parent = this.transform;
                DesireEff.transform.localPosition = Utility.GetNGUIAbsoluteLocalPos(new Vector3(0f, 0.9f, 0f));
                DesireEff.transform.localRotation = Quaternion.identity;
                DesireEff.transform.localScale = Vector3.one;

                DesireEff.SetActive(false);
            }
        }
        else
        {
            DesireEff.SetActive(false);
        }

        for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++)
        {
            if (GameInfo.Instance.RewardList[i].bNew)
            {
                CardData card = GameInfo.Instance.GetCardData(GameInfo.Instance.RewardList[i].UID);
                if (card != null)
                    bNew = true;
            }
        }
        if( _newlist.Count == 0 )
        {
            for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++)
                _newlist.Add(GameInfo.Instance.RewardList[i].bNew);
        }

        kNewCardLabel.gameObject.SetActive(false);
        kNextBtn.gameObject.SetActive(false);

        kBackBtn.gameObject.SetActive(false);
        kCloseBtn.gameObject.SetActive(false);
        kReBtn.gameObject.SetActive(false);

        if (bNew)
        {
            kNewCardLabel.gameObject.SetActive(true);
            kNextBtn.gameObject.SetActive(true);

        }
        else
        {
            kBackBtn.gameObject.SetActive(true);
            kCloseBtn.gameObject.SetActive(true);
            if(!LobbyUIManager.Instance.IsActiveUI("PackagePopup"))
                kReBtn.gameObject.SetActive(true);
        }

        if( GameSupport.IsTutorial() )
            kReBtn.gameObject.SetActive(false);

		UIGachaPanel gachaPanel = LobbyUIManager.Instance.GetActiveUI<UIGachaPanel>("GachaPanel");
		if(gachaPanel && gachaPanel.CurClickedGachaBtn && gachaPanel.CurClickedGachaBtn.IsFree100Gacha)
		{
			kReBtn.gameObject.SetActive(false);
		}

		AppMgr.Instance.CustomInput.ShowCursor(true);

        if (GameInfo.Instance.RewardGachaDesirePoint > (int)eCOUNT.NONE && !_bIsActiveEff)
        {
            if (DesireEff != null)
            {
                DesireEff.SetActive(true);
            }

            _bIsActiveEff = true;
        }
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UIGoodsPopup goodsPopup = LobbyUIManager.Instance.GetUI<UIGoodsPopup>("GoodsPopup");
            Debug.LogError(goodsPopup.kDesireGoodsUnit.kIconSpr.transform.position);
        }
    }
#endif

    public override void Renewal(bool bChildren)
    {
        for (int i = 0; i < kItemList.Count; i++)
            kItemList[i].gameObject.SetActive(false);

        kMileage.SetActive(false);
        if ( GameInfo.Instance.RewardGachaSupporterPoint != 0 )
        {
            kMileage.SetActive(true);
            kPointGoodsUnit.InitGoodsUnit(eGOODSTYPE.SUPPORTERPOINT, GameInfo.Instance.RewardGachaSupporterPoint);
        }

        kDesire.SetActive(false);
        kDesirePointMaxObj.SetActive(false);
        if (GameInfo.Instance.RewardGachaDesirePoint != 0)
        {
            kDesire.SetActive(true);
            kDesireGoodsUnit.InitGoodsUnit(eGOODSTYPE.DESIREPOINT, GameInfo.Instance.RewardGachaDesirePoint);

            if (GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.DESIREPOINT] >= GameInfo.Instance.GameConfig.LimitMaxDP)
                kDesirePointMaxObj.SetActive(true);
        }

        /*
        if (GameInfo.Instance.RewardList.Count == 10)
            kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(1130);
        else
            kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(1129);
        */
        for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++)
        {
            RewardData reward = GameInfo.Instance.RewardList[i];
            SetItemList(reward, i);
        }
    }

    private void SetItemList(RewardData reward, int index)
    {
       

        if (reward.Type == (int)eREWARDTYPE.WEAPON )         //무기
        {
            WeaponData data = GameInfo.Instance.GetWeaponData(reward.UID);
            if (data != null)
            {
                kItemList[index].ParentGO = this.gameObject;
                kItemList[index].UpdateSlot(UIItemListSlot.ePosType.Result, index, data);
            }
        }
        else if (reward.Type == (int)eREWARDTYPE.GEM)            //곡옥
        {
            GemData data = GameInfo.Instance.GetGemData(reward.UID);
            if (data != null)
            {
                kItemList[index].ParentGO = this.gameObject;
                kItemList[index].UpdateSlot(UIItemListSlot.ePosType.Result, index, data);
            }
        }
        else if (reward.Type == (int)eREWARDTYPE.CARD)
        {
            CardData data = GameInfo.Instance.GetCardData(reward.UID);
            if (data != null)
            {
                kItemList[index].ParentGO = this.gameObject;
                kItemList[index].UpdateSlot(UIItemListSlot.ePosType.Result, index, data);
            }
        }
        else if (reward.Type == (int)eREWARDTYPE.ITEM)
        {
            ItemData data = GameInfo.Instance.GetItemData(reward.UID);
            if (data != null)
            {
                kItemList[index].ParentGO = this.gameObject;
                kItemList[index].UpdateSlot(UIItemListSlot.ePosType.Result, index, data, reward.Value);
            }
        }

        if (_newlist[index])
        {
            kItemList[index].kNewSpr.gameObject.SetActive(true);
            kItemList[index].kNewSpr.spriteName = "item_new";
        }
        else
        {
            kItemList[index].kNewSpr.gameObject.SetActive(false);
        }
        kItemList[index].gameObject.SetActive(true);
    }

    public void OnClick_NextBtn()
    {
        DesireEffectHide();

        DirectorUIManager.Instance.PlayGachaNewCardGreeings();
    }
    public void OnClick_BackBtn()
    {
        OnClickClose();
    }

    public void OnClick_ReBtn()
    {
        DesireEffectHide();

        UIGachaPanel panel = LobbyUIManager.Instance.GetActiveUI<UIGachaPanel>("GachaPanel");
        if (panel == null)
            return;

        _newlist.Clear();
        OnClickClose(); 
        panel.ReGacha();
    }

    public override void OnClickClose()
    {
        DesireEffectHide();
        _bIsActiveEff = false;
        _newlist.Clear();
        base.OnClickClose();

        if (GameSupport.IsTutorial())
        {
            if (GameInfo.Instance.UserData.GetTutorialState() == (int)eTutorialState.TUTORIAL_STATE_Mail)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.TutorialState, (int)eTutorialState.TUTORIAL_STATE_Gacha);
                UIValue.Instance.SetValue(UIValue.EParamType.TutorialStep, 9);
                LobbyUIManager.Instance.ShowUI("TutorialPopup", false);
            }
        }
    }

    public void DesireEffectHide()
    {
        if (DesireEff != null)
        {
            DesireEff.SetActive(false);
        }
    }

    public override bool IsBackButton()
    {
        //신규 서포터 있을땐 연출 나와야해서 안먹히도록 적용
        if(kNewCardLabel.gameObject.activeSelf)
            return false;

        return true;
    }
}