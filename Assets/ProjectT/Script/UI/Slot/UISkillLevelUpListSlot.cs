using UnityEngine;
using System.Collections;

public class UISkillLevelUpListSlot : FSlot
{
    public UIButton kSkillPolishBtn;
    public UISprite kSkillPolishSpr;
    public UILabel kLevelLabel;
    public GameObject kGet;
    public int kSkillSlot;
    private int _index;
    private CharData _chardata;
    private GameTable.CharacterSkillPassive.Param _tabledata;

    private GameObject _levelUpEffect = null;
    private Coroutine _coroutine = null;

    //[SerializeField]
    //private float m_effectTime = 0.5f;

    private void OnDisable()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public void UpdateSlot(int index, CharData chardata)   //Fill parameter if you need
    {
        //, int levelupskillid
        _index = index;
        _chardata = chardata;
        _tabledata = GameInfo.Instance.GameTable.FindCharacterSkillPassive(x => x.CharacterID == _chardata.TableID && x.Slot == kSkillSlot && (x.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_NORMAL || x.Type == (int)eCHARSKILLPASSIVETYPE.UPGRADE_ULTIMATE));
        
        GameSupport.SetSkillSprite(ref kSkillPolishSpr, _tabledata.Atlus, _tabledata.Icon);
        kSkillPolishBtn.normalSprite = _tabledata.Icon;

        if (_levelUpEffect != null)
            _levelUpEffect.SetActive(false);

        int level = 0;
        var passviedata = chardata.PassvieList.Find(x => x.SkillID == _tabledata.ID);
        if (passviedata != null)
        {
            level = passviedata.SkillLevel;
        }

        kLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), level);

        kGet.SetActive(false);
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == _tabledata.ItemReqListID && x.Level == level);
        if (reqdata != null)
        {
            if (_chardata.Level >= reqdata.LimitLevel) // 레벨제한
            {
                if( _chardata.PassviePoint >= reqdata.GoodsValue )
                    if ( GameInfo.Instance.UserData.IsGoods(eGOODSTYPE.GOLD, reqdata.Gold) )
                        kGet.SetActive(true);
            }
        }
    }


    public void OnClick_Slot()
    {
        if(ParentGO != null)
        {
            UICharInfoTabSkillPanel uICharInfoTabSkillPanel = ParentGO.GetComponent<UICharInfoTabSkillPanel>();
            if (uICharInfoTabSkillPanel != null)
                uICharInfoTabSkillPanel.HideSkillLevelUpEffectWithSlot();
        }

        var popup = LobbyUIManager.Instance.GetUI<UISkillPassvieLeveUpPopup>("SkillPassvieLeveUpPopup");
        if (popup != null)
        {
            popup.SetSkillSlot(this);
        }

        UIValue.Instance.SetValue(UIValue.EParamType.CharSkillPassiveID, _tabledata.ID);
        LobbyUIManager.Instance.ShowUI("SkillPassvieLeveUpPopup", true);
    }

    public void HideEffect()
    {
        StopCoroutine("ShowEffectCoroutine");
        if (_levelUpEffect != null)
            _levelUpEffect.SetActive(false);
    }

    public void ShowEffect()
    {
        StartCoroutine(ShowEffectCoroutine());
    }
    
    private IEnumerator ShowEffectCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        if (_levelUpEffect == null)
        {
            _levelUpEffect = GameSupport.CreateUIEffect("prf_fx_ui_character_skill_levelup", kLevelLabel.transform);
            if(_levelUpEffect != null )
                _levelUpEffect.transform.localPosition = new Vector3(0, 30.0f, 0);
        }

        if (_levelUpEffect != null)
        {
            _levelUpEffect.SetActive(false);
            _levelUpEffect.SetActive(true);
            SoundManager.Instance.PlayUISnd(10);
        }

        /*
        yield return new WaitForSeconds(m_effectTime);
        //  연출 진행후 라벨 갱신
        string strPersent = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONE_POINT_TEXT, ( value / (float)eCOUNT.MAX_RATE_VALUE * 100.0f )));
        FLocalizeString.SetLabel(kDescLabel, _tabledata.Desc, strPersent);
        FLocalizeString.SetLabel(kLevelLabel, (int)eTEXTID.LEVEL_TXT_NOW_LV, level);
        */
    }
}
