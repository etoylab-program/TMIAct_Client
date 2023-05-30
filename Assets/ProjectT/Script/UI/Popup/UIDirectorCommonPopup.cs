using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDirectorCommonPopup : FComponent
{
    public enum eUITYPE
    {
        NONE = 0,
        SKIP,
        CONFIRM,
        TAPTONEXT,
        SKIP_TAPTONEXT,
        
    }

    public UIButton kSkipBtn;
    public UIButton kConfirmBtn;
    public UIButton kTapToNextBtn;
    public GameObject kTapToNext;
    private eUITYPE _etype = eUITYPE.NONE;


    public override void OnEnable()
    {
		SetUIType( eUITYPE.NONE );
		base.OnEnable();
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        kSkipBtn.gameObject.SetActive(false);
        kTapToNextBtn.gameObject.SetActive(false);
        kTapToNext.gameObject.SetActive(false);
        kConfirmBtn.gameObject.SetActive(false);
        if (_etype == eUITYPE.SKIP)
        {
            kSkipBtn.gameObject.SetActive(true);
        }
        else if (_etype == eUITYPE.CONFIRM )
        {
            kConfirmBtn.gameObject.SetActive(true);
        }
        else if (_etype == eUITYPE.TAPTONEXT)
        {
            kTapToNextBtn.gameObject.SetActive(true);
            kTapToNext.gameObject.SetActive(true);
        }
        else if (_etype == eUITYPE.SKIP_TAPTONEXT)
        {
            kSkipBtn.gameObject.SetActive(true);
            kTapToNextBtn.gameObject.SetActive(true);
            kTapToNext.gameObject.SetActive(true);
        }
    }

    public void OnClick_SkipBtn()
    {
        DirectorUIManager.Instance.OnClick_Next();
        OnClickClose();
    }

    public void OnClick_ConfirmBtn()
    {
        DirectorUIManager.Instance.OnClick_Next();
        OnClickClose();
    }

    public void OnClick_TapToNextBtn()
    {
        DirectorUIManager.Instance.OnClick_Next();
        OnClickClose();
    }

    public override void OnClickClose()
    {
        base.OnClickClose();

        FComponent ui = LobbyUIManager.Instance.GetActiveUI("StorePanel");
        if (ui) ui.Renewal();
    }

    public override bool IsBackButton()
    {
        return false;
    }

    public void SetUIType(eUITYPE type)
    {
        _etype = type;
        Renewal(true);
    }
}
