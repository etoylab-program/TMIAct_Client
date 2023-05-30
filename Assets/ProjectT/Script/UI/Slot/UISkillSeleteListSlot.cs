using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkillSeleteListSlot : FSlot
{
    public UISprite kNameSpr;
    //public UISprite kOpenSpr;
    public List<UISprite> kOpenOnList;
    public List<UISprite> kOpenOffList;
    public UISprite kTypeSpr;
    public UISprite kSelSpr;
    public GameObject kLock;
    public UILabel kLockLevelLabel;
    public GameObject kGet;
    public GameObject kSlot;
    public UILabel kNumberLabel;
    private int _index;
    private CharData _chardata;
    private PassiveData _passivedata;
    private int _skillid;
    private GameTable.CharacterSkillPassive.Param _tabledata;
    private GameObject _effectObject = null;
    private bool _bmine = true;
    public int kSkillSlot;
    public GameTable.CharacterSkillPassive.Param TableData { get { return _tabledata; } }

    private void OnEnable()
    {
        if (_effectObject != null)
            _effectObject.SetActive(false);
    }

    public void UpdateSlot(int index, CharData chardata, bool bmine = true)
    {
        _tabledata = null;
        _passivedata = null;

        _chardata = chardata;
        _tabledata = GameInfo.Instance.GameTable.FindCharacterSkillPassive(x => x.CharacterID == _chardata.TableID && x.Slot == kSkillSlot && x.ParentsID == -1 && (x.Type == (int)eCHARSKILLPASSIVETYPE.SELECT_NORMAL));
        _passivedata = chardata.PassvieList.Find(x => x.SkillID == _tabledata.ID);

        _index = index;
        _skillid = _tabledata.ID;
        _bmine = bmine;

        kSelSpr.gameObject.SetActive(false);
        kNameSpr.gameObject.SetActive(true);
        kTypeSpr.gameObject.SetActive(false);
        kLock.SetActive(false);
        kGet.SetActive(false);
        kSlot.SetActive(false);
        


        for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
        {
            if (chardata.EquipSkill[i] == _skillid)
            {
                kSlot.SetActive(true);
                kNumberLabel.textlocalize = (i + 1).ToString("0#");
            }
        }
        

        //kOpenSpr.spriteName = "SkillSlot_00";

        if( _tabledata.GroupID > 0 )
        {
            kTypeSpr.gameObject.SetActive(true);
            kTypeSpr.spriteName = "SkillType_0" + _tabledata.GroupID.ToString();
        }

        GameSupport.SetSkillSprite(ref kNameSpr, _tabledata.Atlus, _tabledata.Icon);

        for (int i = 0; i < kOpenOnList.Count; i++)
        {
            kOpenOnList[i].gameObject.SetActive(false);
            kOpenOffList[i].gameObject.SetActive(false);
        }

        var list = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.ParentsID == _skillid && x.Type == (int)eCHARSKILLPASSIVETYPE.CONDITION_SKILL);
        for (int i = 0; i < list.Count; i++)
        {
            int icon = (list[i].CondType % 3);
            if (list[i].CondType == (int)eCHARSKILLCONDITION.ANY_CARD_CNT)
                icon = 0;
            else if (icon == 0)
                icon = 3;


            kOpenOffList[i].spriteName = string.Format("SkillSlot_{0}_{1}", icon, 0);
            kOpenOnList[i].spriteName = string.Format("SkillSlot_{0}_{1}", icon, 1);

            if (GameSupport.IsCharSkillCond(_chardata, list[i]))
                kOpenOnList[i].gameObject.SetActive(true);
            else
                kOpenOffList[i].gameObject.SetActive(true);
        }



        if ( _passivedata == null )
        {
            var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == _tabledata.ItemReqListID);
            if (reqdata != null)
            {
                if (_chardata.Level < reqdata.LimitLevel) // 레벨제한
                {
                    kLock.SetActive(true);
                    kLockLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), reqdata.LimitLevel);
                }
                else
                {
                    kGet.SetActive(true);
                }
            }
        }


        UICharSkillSeletePopup popup = ParentGO.GetComponent<UICharSkillSeletePopup>();
        if (popup != null)
        {
            if (popup.SeleteSkillID == _tabledata.ID)
            {
                kSelSpr.gameObject.SetActive(true);
            }


        }


    }

    public void OnClick_Slot()
    {
        if (ParentGO == null)
            return;

        Debug.Log("OnClick_Slot");
        if (!_bmine)
            return;

        UICharSkillSeletePopup panel = ParentGO.GetComponent<UICharSkillSeletePopup>();
        if (panel == null)
            return;

        panel.SetSeleteSkill(_tabledata.ID, this);
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
        if (_effectObject == null)
            _effectObject = GameSupport.CreateUIEffect("prf_fx_ui_skillslot_name_activate", this.transform);

        if (_effectObject != null)
        {
            _effectObject.SetActive(false);
            _effectObject.SetActive(true);
            SoundManager.Instance.PlayUISnd(11);
            var par = _effectObject.GetComponent<ParticleSystem>();
            if(par != null)
            {
                yield return new WaitForSeconds(par.duration);
            }
            _effectObject.SetActive(false);
        }

        yield return null;
    }

}