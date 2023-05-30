using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBingoResetPopup : FComponent
{
    [SerializeField] private UIGoodsUnit goodsUnit = null;

    private Action _yesAction = null;
    private Action _noAction = null;
    private int _goodsNumber = 0;

    public void SetData(int goodsNumber, Action yesAction, Action noAction = null)
    {
        _goodsNumber = goodsNumber;
        goodsUnit.InitGoodsUnit(eGOODSTYPE.CASH, goodsNumber, true);

        _yesAction = yesAction;
        _noAction = noAction;
    }

    public void OnClick_YesBtn()
    {
        if (GameInfo.Instance.UserData.IsGoods(eGOODSTYPE.CASH, _goodsNumber))
        {
            _yesAction?.Invoke();
        }
        else
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(101));
        }

        OnClickClose();
    }

    public void OnClick_NoBtn()
    {
        _noAction?.Invoke();

        OnClickClose();
    }
}
