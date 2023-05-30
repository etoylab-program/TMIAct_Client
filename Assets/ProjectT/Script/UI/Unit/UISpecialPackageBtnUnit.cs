using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpecialPackageBtnUnit : FUnit
{
    public UILabel kEndTimeLabel;

    public bool Rendering { get; private set; }

    private UnexpectedPackageData _data;
    private Action _disableAction;
    
    public void Awake()
    {
        Rendering = false;
    }
    
    public void SetData(UnexpectedPackageData data, Action disableAction)
    {
        _data = data;
        _disableAction = disableAction;
        
        Rendering = true;
    }

    public void FixedUpdate()
    {
        if (Rendering == false)
        {
            _disableAction?.Invoke();
            _disableAction = null;
            return;
        }
        
        if (_data.IsPurchase)
        {
            Rendering = false;
            return;
        }
        
        TimeSpan remainTime = _data.EndTime - GameSupport.GetCurrentServerTime();
        if (remainTime.Ticks < 0)
        {
            Rendering = false;
            return;
        }
        
        kEndTimeLabel.textlocalize = FLocalizeString.Instance.GetText(232,
            remainTime.Hours, remainTime.Minutes, remainTime.Seconds);
    }

    public void OnClick_SpecialPackageBtn()
    {
        if (_data == null)
        {
            return;
        }
        
        GameTable.UnexpectedPackage.Param tableData = GameInfo.Instance.GameTable.FindUnexpectedPackage(_data.TableId);
        if (tableData == null)
        {
            return;
        }

        if (tableData.UnexpectedType == (int)eUnexpectedPackageType.FIRST_STAGE)
        {
            UISpecialBuyDailyPopup specialBuyDailyPopup = LobbyUIManager.Instance.GetUI("SpecialBuyDailyPopup") as UISpecialBuyDailyPopup;
            if (specialBuyDailyPopup != null)
            {
                specialBuyDailyPopup.SetGameTable(_data.TableId);
                specialBuyDailyPopup.SetUIActive(true);
            }
        }
        else
        {
            UISpecialBuyPopup specialBuyPopup = LobbyUIManager.Instance.GetUI("SpecialBuyPopup") as UISpecialBuyPopup;
            if (specialBuyPopup != null)
            {
                specialBuyPopup.SetGameTable(_data.TableId);
                specialBuyPopup.SetUIActive(true);
            }
        }
    }
}
