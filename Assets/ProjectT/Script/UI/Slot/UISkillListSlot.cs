using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISkillListSlot : FSlot
{
    public enum ePOS
    {
        EQUIPED = 0,
        VIEW,
        RANKING,
    }
    public UISprite kSkillSlotSpr;
    public UISprite kSkillNameSpr;
    //public UISprite kOpenSpr;
    public List<UISprite> kOpenOnList;
    public List<UISprite> kOpenOffList;
    public UISprite kEmptySpr;
    public UILabel kSlotLabel;
    public UISprite kNumberSpr;
    public UILabel kNumberLabel;
    public UISprite kSelSpr;
    public GameObject kLock;
    public UILabel kLockLevelLabel;
    public GameObject kGet;
    private ePOS _pos;
    private int _index;
    private CharData _chardata;
    private PassiveData _passivedata;
    private int _skillid;
    private GameTable.CharacterSkillPassive.Param _tabledata;
    private GameObject _effectObject = null;
    private bool _bmine = true;
    public int kSkillSlot;

    public void UpdateSlot(ePOS pos, int index, PassiveData passivedata, CharData chardata, List<CardData> listEquipCardData, bool bmine = true)
    {
        _pos = pos;
        _index = index;
        _passivedata = passivedata;
        _chardata = chardata;
        _bmine = bmine;
        _tabledata = null;
       

        kSelSpr.gameObject.SetActive(false);
        kSlotLabel.gameObject.SetActive(false);
        kSkillSlotSpr.gameObject.SetActive(false);
        kEmptySpr.gameObject.SetActive(false);
        kNumberSpr.gameObject.SetActive(false);
        kLock.SetActive(false);
        kGet.SetActive(false);
        if (_effectObject != null)
            _effectObject.SetActive(false);

        kSlotLabel.gameObject.SetActive(true);
        kNumberSpr.gameObject.SetActive(true);
        kNumberLabel.textlocalize = (_index + 1).ToString("0#");

        //kOpenSpr.spriteName = "SkillSlot_00";

        for (int i = 0; i < kOpenOnList.Count; i++)
        {
            kOpenOnList[i].gameObject.SetActive(false);
            kOpenOffList[i].gameObject.SetActive(false);
        }

        if (_passivedata == null)
        {
            kEmptySpr.gameObject.SetActive(true);

            if (_index == (int)eCOUNT.SKILLSLOT - 1)
            {
                if (_pos == ePOS.RANKING)
                {
                    kLock.SetActive(true);
                }
                else
                {
                    if (!GameSupport.IsActiveUserBuff(eBuffEffectType.Buff_SkillSlot))
                    {
                        kLock.SetActive(true);
                    }
                }
            }
            else
            {
                if (_chardata == null)
                {
                    return;
                }

                if (_chardata.Level < GameInfo.Instance.GameConfig.CharSkillSlotLimitLevel[index]) // 레벨제한
                {
                    //  잠금 활성화, 잠금 텍스트
                    kLock.SetActive(true);
                    FLocalizeString.SetLabel(kLockLevelLabel, (int)eTEXTID.LEVEL_TXT_NOW_LV, GameInfo.Instance.GameConfig.CharSkillSlotLimitLevel[index]);
                }
                else
                {
                    if (bmine)
                    {
                        string str = string.Format("UserData_SlotEffect_{0}_{1}", _chardata.TableData.ID.ToString(), index);
                        if (PlayerPrefs.HasKey(str))
                        {
                            ShowEffect();
                            //PlayerPrefs.SetInt(str, 1);
                            PlayerPrefs.DeleteKey(str);
                        }
                    }
                }
            }
            
        }
        else
        {
            if (_pos == ePOS.RANKING)
            {
                int exSkill = (chardata.TableID * 1000) + 301;
                if (_passivedata.SkillID == exSkill)
                {
                    kLock.SetActive(true);
                    return;
                }
            }

            kSkillSlotSpr.gameObject.SetActive(true);
            //kSkillSlotSpr.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Skill/" + _passivedata.TableData.Icon + ".png") as Texture2D;

            GameSupport.SetSkillSprite(ref kSkillNameSpr, _passivedata.TableData.Atlus, _passivedata.TableData.Icon);

            var list = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.ParentsID == _passivedata.TableData.ID && x.Type == (int)eCHARSKILLPASSIVETYPE.CONDITION_SKILL);
            for (int i = 0; i < list.Count; i++)
            {
                int icon = (list[i].CondType % 3);
                if (list[i].CondType == (int)eCHARSKILLCONDITION.ANY_CARD_CNT)
                    icon = 0;
                else if (icon == 0)
                    icon = 3;


                kOpenOffList[i].spriteName = string.Format("SkillSlot_{0}_{1}", icon, 0);
                kOpenOnList[i].spriteName = string.Format("SkillSlot_{0}_{1}", icon, 1);

                if(_pos != ePOS.RANKING)
                {
                    if (GameSupport.IsCharSkillCond(_chardata, list[i]))
                        kOpenOnList[i].gameObject.SetActive(true);
                    else
                        kOpenOffList[i].gameObject.SetActive(true);
                }
                else
                {
                    if (GameSupport.IsCharSkillCondWithTableData(_chardata, listEquipCardData, list[i]))
                        kOpenOnList[i].gameObject.SetActive(true);
                    else
                        kOpenOffList[i].gameObject.SetActive(true);
                }
            }

            //200122 - 기존 유저 스킬오픈 알람 삭제용
            if (bmine)
            {
                string str = string.Format("UserData_SlotEffect_{0}_{1}", _chardata.TableData.ID.ToString(), index);
                if (PlayerPrefs.HasKey(str))
                {
                    PlayerPrefs.DeleteKey(str);
                }
            }
        }
    }

    public void OnClick_Slot()
    {
        if (ParentGO == null)
            return;
        if (!_bmine)
            return;

        if (_pos == ePOS.EQUIPED)
        {
            if (_index == -1)
                return;

            //프리미엄 스킬슬롯인지 확인
            if (_index == (int)eCOUNT.SKILLSLOT - 1)
            {
                if (!GameSupport.IsHaveMonthlyData((int)eMonthlyType.PREMIUM) && !GameSupport.IsActiveUserBuff(eBuffEffectType.Buff_SkillSlot))
                {
                    GameTable.Store.Param monthlyStoreTable = GameInfo.Instance.GameTable.FindStore(x => x.ProductType == (int)eREWARDTYPE.MONTHLYFEE && x.ProductIndex == (int)eMonthlyType.PREMIUM);
                    GameClientTable.StoreDisplayGoods.Param monthlyGoods = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == monthlyStoreTable.ID);

                    string itemName = FLocalizeString.Instance.GetText(monthlyGoods.Name);

                    MessagePopup.OKCANCEL(eTEXTID.OK, FLocalizeString.Instance.GetText(3216, itemName, itemName), () => 
                    {
                        GameSupport.PaymentAgreement_Package(2022);
                        if(LobbyUIManager.Instance.IsActiveUI("CharInfoPanel"))
                        {
                            UICharInfoPanel uICharInfoPanel = LobbyUIManager.Instance.GetActiveUI<UICharInfoPanel>("CharInfoPanel");
                            if (uICharInfoPanel != null)
                            {
                                uICharInfoPanel.SetDefaultTab();
                            }

                            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.SKILL);
                        }
                    });
                    return;
                }
            }
            else
            {
                if (_chardata == null)
                {
                    return;
                }

                if (_chardata.Level < GameInfo.Instance.GameConfig.CharSkillSlotLimitLevel[_index]) // 레벨제한
                {
                    MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3012), GameInfo.Instance.GameConfig.CharSkillSlotLimitLevel[_index]));
                    return;
                }
            }

            

            if (ParentGO != null)
            {
                UICharInfoTabSkillPanel uICharInfoTabSkillPanel = ParentGO.GetComponent<UICharInfoTabSkillPanel>();
                if (uICharInfoTabSkillPanel != null)
                    uICharInfoTabSkillPanel.HideSkillLevelUpEffectWithSlot();
            }

            UIValue.Instance.SetValue(UIValue.EParamType.CharEquipSkillSlot, _index);
            LobbyUIManager.Instance.ShowUI("CharSkillSeletePopup", true);
        }
      
    }

    public void OnNetReqLvUpSkill(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("CharInfoPanel");
        LobbyUIManager.Instance.Renewal("CharSkillSeletePopup");

        ShowEffect();
    }

    public void ShowEffect()
    {
        StartCoroutine(ShowEffectCoroutine());
    }
    
    private IEnumerator ShowEffectCoroutine()
    {
        if (_pos == ePOS.EQUIPED)
        {
            if (_effectObject == null)
                _effectObject = GameSupport.CreateUIEffect("prf_fx_ui_skillslot_activate", this.transform);
            _effectObject.SetActive(false);

            yield return new WaitForSeconds(0.2f);

            if (_effectObject != null)
            {
                _effectObject.SetActive(true);
                SoundManager.Instance.PlayUISnd(12);
            }
        }

        yield return null;
    }

}

