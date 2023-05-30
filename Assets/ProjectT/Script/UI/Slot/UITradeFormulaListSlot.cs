using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITradeFormulaListSlot : FSlot
{
    [Serializable]
    public class NormalFormula
    {
        public UISprite gradeIconSpr;
        public UILabel countLabel;
        public UILabel probLabel;
    }

    [Serializable]
    public class AddFormula
    {
        public UILabel nameLabel;
        public UILabel descLabel;
        public UILabel countLabel;
        public UISprite itemSpr;
    }

    [SerializeField] private NormalFormula normalFormula;
    [SerializeField] private AddFormula addFormula;

    public void UpdateSlot(GameTable.FacilityTrade.Param param)
    {
        if (param == null)
        {
            SetState(false);
            return;
        }

        SetState(true);
        
        normalFormula.gradeIconSpr.spriteName = $"itemgrade_L_{param.MaterialGrade}";
        normalFormula.countLabel.textlocalize = FLocalizeString.Instance.GetText(213, param.MaterialCount);

        int percent = (int)(param.SuccessProb * 0.001f);
        if (percent <= 0)
        {
            percent = 0;
        }

        int height = 30;
        string probStr = FLocalizeString.Instance.GetText(1652, percent);
        if (param.TradeGroup == 2)
        {
            height = 60;
            probStr = FLocalizeString.Instance.GetText(1815, probStr);
        }
        
        normalFormula.probLabel.height = height;
        normalFormula.probLabel.textlocalize = probStr;
    }

    public void UpdateSlot(GameClientTable.FacilityTradeHelp.Param param)
    {
        if (param == null)
        {
            SetState(false);
            return;
        }
        
        SetState(true);

        addFormula.nameLabel.textlocalize = FLocalizeString.Instance.GetText(param.Name);
        addFormula.descLabel.textlocalize = FLocalizeString.Instance.GetText(param.Desc);
        addFormula.countLabel.textlocalize = FLocalizeString.Instance.GetText(213, param.Count);
        addFormula.itemSpr.spriteName = param.Icon;
    }

    private void SetState(bool state)
    {
        normalFormula.gradeIconSpr.SetActive(state);
        normalFormula.countLabel.SetActive(state);
        normalFormula.probLabel.SetActive(state);

        addFormula.nameLabel.SetActive(state);
        addFormula.descLabel.SetActive(state);
        addFormula.countLabel.SetActive(state);
        addFormula.itemSpr.SetActive(state);
    }
}
