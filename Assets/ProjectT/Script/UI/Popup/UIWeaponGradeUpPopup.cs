using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWeaponGradeUpPopup : FComponent
{
	
	public UISprite kWakeNextSpr;
	public UISprite kWakeEff;
	public UITexture kWeaponNextTex;
    public List<UISprite> kNextSlotList;
    public List<UITexture> kNextSlotIconList;
    public List<UISprite> kNextSlotLockList;
    public UISprite kSlotEFFSpr;
    public UISprite kGradeSpr;
	public UILabel kNameLabel;
	public UILabel kLevelNextLabel;
    public List<UIItemListSlot> kMatItemList;
    public List<UILabel> kHaveCountLabel;
    public UIGoodsUnit kGoldGoodsUnit;
    public UIButton kCancleBtn;
	public UIButton kLevelUpBtn;
    private WeaponData _weapondata;
    private bool _bmat = true;
    private Vector3 m_originalEffPos;

    private bool _showEffect = false;

	public override void Awake() {
		base.Awake();

		m_originalEffPos = kWakeEff.transform.localPosition;
	}

	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}

    public override void InitComponent()
	{
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.WeaponUID);
        _weapondata = GameInfo.Instance.GetWeaponData(uid);
        _bmat = true;
        _showEffect = false;
        //  팝업 등장 애니메이션으로 인한  랜더 타겟 변경 딜레이
        Invoke("InvokeShowWeapon", 0.1f);
    }

    private void InvokeShowWeapon()
    {
        RenderTargetWeapon.Instance.InitRenderTargetWeapon(_weapondata.TableID, _weapondata.WeaponUID, true);
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
        _bmat = true;
        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_weapondata.TableData.Name);
        kGradeSpr.spriteName = "itemgrade_L_" + _weapondata.TableData.Grade.ToString();
        //kGradeSpr.MakePixelPerfect();

        // 제련 시 레벨이 초기화된다는 정보 전달을 위해 레벨 1로 표기
        kLevelNextLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, 1, GameSupport.GetWeaponMaxLevel(_weapondata, _weapondata.Wake + 1), false, true);

        kWakeNextSpr.spriteName = "itemwake_0" + (_weapondata.Wake + 1).ToString();
        kWakeNextSpr.MakePixelPerfect();
        Vector3 pos = m_originalEffPos;
        pos.y = -((float)_weapondata.Wake * 22.0f);
        kWakeEff.transform.localPosition = pos;

        for (int i = 0; i < kMatItemList.Count; i++)
        {
            kMatItemList[i].gameObject.SetActive(false);
            kHaveCountLabel[i].gameObject.SetActive(false);
        }

        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == _weapondata.TableData.WakeReqGroup && x.Level == _weapondata.Wake);
        if (reqdata != null)
        {
            List<int> idlist = new List<int>();
            List<int> countlist = new List<int>();
            GameSupport.SetMatList(reqdata, ref idlist, ref countlist);
            for (int i = 0; i < idlist.Count; i++)
            {
                kMatItemList[i].gameObject.SetActive(true);
                kMatItemList[i].UpdateSlot(UIItemListSlot.ePosType.Mat, i, GameInfo.Instance.GameTable.FindItem(idlist[i]));
                int orgcut = GameInfo.Instance.GetItemIDCount(idlist[i]);
                int orgmax = countlist[i];
                string strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
                if (orgcut < orgmax)
                {
                    _bmat = false;
                    strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);
                }
                string strmatcount = string.Format(strHaveCntColor, string.Format(FLocalizeString.Instance.GetText(236), orgcut, orgmax));
                kMatItemList[i].SetCountLabel(strmatcount);
                //kHaveCountLabel[i].gameObject.SetActive(true);
                //string strhavecount = string.Format(strHaveCntColor, string.Format(FLocalizeString.Instance.GetText(279), orgcut));
                //kHaveCountLabel[i].textlocalize = strhavecount;
            }
            kGoldGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, reqdata.Gold, true);
        }

        int wakemax = GameSupport.GetWeaponMaxWake(_weapondata);
        int slotmax = GameSupport.GetWeaponGradeSlotCount(_weapondata.TableData.Grade, wakemax);
        int slotnow = GameSupport.GetWeaponGradeSlotCount(_weapondata.TableData.Grade, _weapondata.Wake);
        int slotnext = GameSupport.GetWeaponGradeSlotCount(_weapondata.TableData.Grade, _weapondata.Wake+1);


        for (int i = 0; i < kNextSlotList.Count; i++)
            kNextSlotList[i].gameObject.SetActive(false);


        for (int i = 0; i < slotmax; i++)
        {
            kNextSlotList[i].gameObject.SetActive(true);
            kNextSlotIconList[i].gameObject.SetActive(false);
            kNextSlotLockList[i].gameObject.SetActive(false);
            if (i >= slotnext)
                kNextSlotLockList[i].gameObject.SetActive(true);

            if (i == slotnext-1)
                kSlotEFFSpr.transform.localPosition = kNextSlotList[i].transform.localPosition;

            GemData gemdata = GameInfo.Instance.GetGemData(_weapondata.SlotGemUID[i]);
            if (gemdata != null)
            {
                kNextSlotIconList[i].gameObject.SetActive(true);
                kNextSlotIconList[i].mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + gemdata.TableData.Icon);
            }
        }

    }

    public void OnClick_CancleBtn()
	{
        OnClickClose();
    }

    public override void OnClickClose()
    {
        if (_showEffect)
            return;
        LobbyUIManager.Instance.InitComponent("WeaponInfoPopup");
        base.OnClickClose();
    }

    public void OnClick_LevelUpBtn()
	{
        if (GameSupport.GetWeaponMaxWake(_weapondata) == 0 )
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3015));
            return;
        }
        if (GameSupport.IsMaxWakeWeapon(_weapondata))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3014));
            return;
        }
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == _weapondata.TableData.WakeReqGroup && x.Level == _weapondata.Wake);
        if (reqdata == null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3002));
            return;
        }
        if (!GameSupport.IsCheckGoods(eGOODSTYPE.GOLD, reqdata.Gold))
        {
            return;
        }
        if (!_bmat)
        {
            //재료 부족 
            if(reqdata != null)
            {
                GameSupport.ShowBuyMatItemPopup(reqdata);
            }
            return;            
        }
        _showEffect = true;
        Director.IsPlaying = true;
        GameInfo.Instance.Send_ReqWakeWeapon(_weapondata.WeaponUID, OnNetWeaponWake);
    }

    public void OnNetWeaponWake(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        StartCoroutine(WeaponWakeResultCoroutine());
    }

    IEnumerator WeaponWakeResultCoroutine()
    {
        DirectorUIManager.Instance.PlayWeaponWakeUp(_weapondata);

        yield return new WaitForSeconds(1.0f);

        base.OnClickClose();
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("WeaponInfoPopup");
        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
        {
            itempanel.InitComponent();
            itempanel.Renewal(true);
        }
        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.CHARINFO)
        {
            LobbyUIManager.Instance.Renewal("CharInfoPanel");
            LobbyUIManager.Instance.InitComponent("CharWeaponSeletePopup");
            LobbyUIManager.Instance.Renewal("CharWeaponSeletePopup");
        }

        InitComponent();
        Renewal(true);
    }

    public override bool IsBackButton()
    {
        return !_showEffect;
    }
}
