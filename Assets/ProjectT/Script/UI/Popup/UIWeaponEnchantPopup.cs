using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWeaponEnchantPopup : FComponent
{
    public UISprite kGradeSpr;
    public UILabel kNameLabel;
    public UILabel kEnchantLabel;
    public UILabel kLevelLabel;
    public UISprite kWakeSpr;

    public UITexture kWeaponTexture;

    public UISprite kLevelArrowSpr;
    public UILabel kBeforeLvLabel;
    public UILabel kAfterLvLabel;
    public UILabel kSuccessRateLabel;
    public UILabel kAbilityLabel;

    public GameObject kDirectStart;
    public GameObject kDirectSuccess;
    public GameObject kDirectFail;
    public UILabel kSuccessLvLabel;
    public UILabel kFailLvLabel;


    public List<UIItemListSlot> kMaterialItemList;
    public List<UILabel> kMaterialCountLabelList;

    public UILabel kNeedGoldLabel;

    private WeaponData _WeaponData = null;
    private GameTable.Enchant.Param _EnchantTable;

    private int CurLv, NextLv;
    private bool IsLackMaterial;
    private int ReqGold;
    private List<GameTable.ItemReqList.Param> ItemReqList;
    private bool IsResultDirect = false;

    GameObject startFX, successFX, failFX;
    private bool IsPossibleCloseFX = true;

    public override void OnEnable()
    {        
        base.OnEnable();

        System.Func<string, Transform, GameObject> FuncCreateFX = (prefabName, parentObj) =>
        {
            GameObject FXObj = ResourceMgr.Instance.CreateFromAssetBundle("effect", string.Format("Effect/UI/{0}.prefab", prefabName));
            if (FXObj == null) return null;

            FXObj.transform.parent = parentObj;
            //FXObj.transform.localPosition = Utility.GetNGUIAbsoluteLocalPos(new Vector3(0f, 0.9f, 0f));
            FXObj.transform.localPosition = Vector3.zero;
            FXObj.transform.localRotation = Quaternion.identity;
            FXObj.transform.localScale = Vector3.one;
            FXObj.SetActive(false);

            return FXObj;
        };

        startFX = FuncCreateFX("prf_fx_ui_enchant_start", kDirectSuccess.transform.parent);
        successFX = FuncCreateFX("prf_fx_ui_enchant_success", kDirectSuccess.transform.parent);
        failFX = FuncCreateFX("prf_fx_ui_enchant_fail", kDirectSuccess.transform.parent);
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();
        if (startFX != null)
            DestroyImmediate(startFX); startFX = null;
        if (successFX != null)
            DestroyImmediate(successFX); successFX = null;
        if (failFX != null)
            DestroyImmediate(failFX); failFX = null;
    }

    public override void InitComponent()
    {
        SetState(false);
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.EnchantUID);
        _WeaponData = null;
        _WeaponData = GameInfo.Instance.WeaponList.Find(x => x.WeaponUID == uid);
        ReqGold = 0;
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
        
        InitComponent();
        if (_WeaponData == null) return;

        kWeaponTexture.SetActive(true);
        kWakeSpr.SetActive(true);
        kNameLabel.SetActive(true);
        kLevelLabel.SetActive(true);
        kLevelArrowSpr.SetActive(true);
        kBeforeLvLabel.SetActive(true);
        kGradeSpr.SetActive(true);
        kSuccessRateLabel.SetActive(true);
        kAbilityLabel.SetActive(true);

        //kWeaponTexture.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("card", string.Format("Card/{0}_{1}.png", _WeaponData.TableData.Icon, GameSupport.GetCardImageNum(_WeaponData)));        

        kGradeSpr.spriteName = string.Format("itemgrade_L_{0}", _WeaponData.TableData.Grade);
        kWakeSpr.spriteName = string.Format("itemwake_0{0}", _WeaponData.Wake);
        kWakeSpr.MakePixelPerfect();

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_WeaponData.TableData.Name);        
        kLevelLabel.textlocalize = GameSupport.GetLevelText((int)eTEXTID.LEVEL_TXT_NOW_LV, _WeaponData.Level, GameSupport.GetWeaponMaxLevel(_WeaponData.TableData.Grade, _WeaponData.Wake));

        // Enchant
        if (_WeaponData.EnchantLv > 0)
        {
            kEnchantLabel.SetActive(true);
            kEnchantLabel.textlocalize = string.Format("+{0}", _WeaponData.EnchantLv);
            kEnchantLabel.transform.localPosition = new Vector3(kNameLabel.transform.localPosition.x + kNameLabel.printedSize.x + 10, kNameLabel.transform.localPosition.y, 0);
        }

        kBeforeLvLabel.textlocalize = string.Format("+{0}", _WeaponData.EnchantLv);

        CurLv = _WeaponData.EnchantLv;
        NextLv = GetNextLevel(CurLv);

        _EnchantTable = GameInfo.Instance.GameTable.Enchants.Find(x => x.GroupID == _WeaponData.TableData.EnchantGroup && x.Level == CurLv);

        if (NextLv > 0)
        {
            kAfterLvLabel.SetActive(true);
            kAfterLvLabel.textlocalize = string.Format("+{0}", NextLv);
        }
        else
        {
            //최대 레벨 처리
            kLevelArrowSpr.SetActive(false);
            kBeforeLvLabel.SetActive(false);
            kSuccessRateLabel.SetActive(false);
            kAbilityLabel.SetActive(false);
        }

        if (_EnchantTable == null)
            return;

        kSuccessRateLabel.textlocalize = string.Format("{0}{1}%", FLocalizeString.Instance.GetText(1466), GetSuccessRate());
        kAbilityLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1667), GetAbilityValue(_WeaponData.EnchantLv), GetAbilityValue(_WeaponData.EnchantLv + 1));

        kNeedGoldLabel.SetActive(true);
        kNeedGoldLabel.textlocalize = "0";
        //재료설정        
        IsLackMaterial = false;
        var matItemReqList = GameInfo.Instance.GameTable.ItemReqLists.FindAll(x => x.Group == _EnchantTable.ItemReqListID);
        if (matItemReqList != null && matItemReqList.Count > 0)
        {
            System.Func<int, int, int, bool> FuncSetItemListSlot = (curSlotIdx, itemid, needCnt) =>
            {
                UIItemListSlot slot = kMaterialItemList[curSlotIdx].GetComponent<UIItemListSlot>();

                if (slot == null || itemid <= 0) return false;
                GameTable.Item.Param p = GameInfo.Instance.GameTable.Items.Find(x => x.ID == itemid);
                if (p == null) return false;

                slot.SetActive(true);
                slot.ParentGO = gameObject;
                slot.UpdateSlot(UIItemListSlot.ePosType.RewardDataInfo, 0, p);

                int curCnt = GameInfo.Instance.GetItemIDCount(itemid);                
                string strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);

                if (curCnt < needCnt)
                {
                    strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);
                    IsLackMaterial = true;
                }
                string strmatcount = string.Format(strHaveCntColor, string.Format(FLocalizeString.Instance.GetText(236), curCnt, needCnt));
                slot.SetCountLabel(strmatcount);
                return true;
            };

            int curSlot = 0;
            for (int i = 0; i < matItemReqList.Count; i++)
            {
                if (curSlot >= kMaterialItemList.Count) break;

                if (FuncSetItemListSlot(curSlot, matItemReqList[i].ItemID1, matItemReqList[i].Count1)) curSlot++;
                if (FuncSetItemListSlot(curSlot, matItemReqList[i].ItemID2, matItemReqList[i].Count2)) curSlot++;
                if (FuncSetItemListSlot(curSlot, matItemReqList[i].ItemID3, matItemReqList[i].Count3)) curSlot++;
                if (FuncSetItemListSlot(curSlot, matItemReqList[i].ItemID4, matItemReqList[i].Count4)) curSlot++;
            }

            kNeedGoldLabel.textlocalize = string.Format("{0:#,#}", matItemReqList[0].Gold);
            ReqGold = matItemReqList[0].Gold;
		}
    }

    private void SetState(bool state)
    {
        kGradeSpr.SetActive(state);
        kNameLabel.SetActive(state);
        kEnchantLabel.SetActive(state);
        kLevelLabel.SetActive(state);
        kWakeSpr.SetActive(state);

        kWeaponTexture.SetActive(state);

        kLevelArrowSpr.SetActive(state);
        kBeforeLvLabel.SetActive(state);
        kAfterLvLabel.SetActive(state);
        kSuccessRateLabel.SetActive(state);
        kAbilityLabel.SetActive(state);

        kDirectStart.SetActive(state);
        kDirectSuccess.SetActive(state);
        kDirectFail.SetActive(state);

        for (int i = 0; i < kMaterialItemList.Count; i++)
            kMaterialItemList[i].SetActive(state);

        for (int i = 0; i < kMaterialCountLabelList.Count; i++)
            kMaterialCountLabelList[i].SetActive(state); ;

        kNeedGoldLabel.SetActive(state);
    }

    private int GetNextLevel(int curLv)
    {
        if (_WeaponData == null || _WeaponData.TableData == null) return -1;

        var _EnchantTable = GameInfo.Instance.GameTable.Enchants.Find(x => x.GroupID == _WeaponData.TableData.EnchantGroup && x.Level == curLv + 1);
        if (_EnchantTable == null) return -1;
        return _EnchantTable.Level;
    }

    private int GetSuccessRate()
    {           
        if (_EnchantTable == null) return 0;
        return (int)(_EnchantTable.Prob * 0.001f);
    }

    private int GetAbilityValue(int enchantLv)
    {
        var _EnchantTable = GameInfo.Instance.GameTable.Enchants.Find(x => x.GroupID == _WeaponData.TableData.EnchantGroup && x.Level == enchantLv);
        if (_EnchantTable == null) return 0;        
        return _EnchantTable.IncreaseValue;
    }

    public override void OnClickClose()
    {
        if(Director.IsPlaying)
        {
            return;
        }

        base.OnClickClose();
    }

    public void OnClick_Enchant()
    {
        Debug.Log("OnClick_Enchant");
        if (IsResultDirect) return;

        if (NextLv <= 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3241));
            return;
        }
        //재료부족 검사
        if (IsLackMaterial)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3042));
            return;
        }
        // 골드부족 검사
        if (ReqGold > GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.GOLD])
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTYPE_LACK_START));
            return;
        }

        GameInfo.Instance.Send_ReqEnchantWeapon(_WeaponData.WeaponUID, OnNetAckEnchantWeapon);
        IsResultDirect = true;
    }

    private void OnNetAckEnchantWeapon(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        StartCoroutine(CoDirectStart(pktmsg as PktInfoWeaponGrow));
    }

    public void OnClick_CloseFX()
    {
        if (!IsPossibleCloseFX) return;
        IsResultDirect = false;
    }

    private IEnumerator CoDirectStart(PktInfoWeaponGrow pAck)
    {
        IsPossibleCloseFX = false;
        Director.IsPlaying = true;

        startFX.SetActive(true);
        ParticleSystem ps = startFX.GetComponent<ParticleSystem>();

        SoundManager.Instance.PlayUISnd(80);
        yield return new WaitForSeconds(ps.main.duration);

        startFX.SetActive(false);

        if (pAck.growState_ == eGrowState.FAIL)
            yield return CoDirectFail(pAck.retEnc_);
        else
            yield return CoDirectSuccess(pAck.retEnc_);

        Director.IsPlaying = false;

        Renewal(true);
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("WeaponInfoPopup");
    }

    private IEnumerator CoDirectSuccess(int lv)
    {
        Animation Ani = kDirectSuccess.GetComponent<Animation>();
        if (Ani == null)
            yield break;

        kSuccessLvLabel.textlocalize = string.Format("+{0}", lv);
        Ani.Stop();

        List<string> AniNameList = new List<string>();
        foreach (AnimationState aniState in Ani)
            AniNameList.Add(aniState.clip.name);

        float aniLength = Ani[AniNameList[0]].length;
        Ani.Play(AniNameList[0]);
        successFX.SetActive(true);

        yield return new WaitForSeconds(aniLength * 0.5f);

        kDirectSuccess.SetActive(true);
        SoundManager.Instance.PlayUISnd(81);

        yield return new WaitForSeconds(1f);
        IsPossibleCloseFX = true;

        while (IsResultDirect)
            yield return null;

        successFX.SetActive(false);
        kDirectSuccess.SetActive(false);
    }

    private IEnumerator CoDirectFail(int lv)
    {
        Animation Ani = kDirectFail.GetComponent<Animation>();
        if (Ani == null)
            yield break;

        kFailLvLabel.textlocalize = string.Format("+{0}", lv);
        Ani.Stop();

        List<string> AniNameList = new List<string>();
        foreach (AnimationState aniState in Ani)
            AniNameList.Add(aniState.clip.name);

        float aniLength = Ani[AniNameList[0]].length;
        Ani.Play(AniNameList[0]);
        failFX.SetActive(true);

        yield return new WaitForSeconds(aniLength * 0.5f);
        
        kDirectFail.SetActive(true);
        SoundManager.Instance.PlayUISnd(82);

        yield return new WaitForSeconds(1f);
        IsPossibleCloseFX = true;

        while (IsResultDirect)
            yield return null;

        failFX.SetActive(false);
        kDirectFail.SetActive(false);
    }
}
