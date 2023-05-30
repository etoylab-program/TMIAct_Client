
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIGameDialogPanel : FComponent
{
    [Header("Fade")]
    public UISprite sprFade;

    [Header("Center Text")]
    public GameObject centerTextObject;
    public UISprite sprCenterTextBg;
    public UILabel lbCenterText;

    [Header("Dialog")]
    public GameObject dialogObject;
    public UISprite sprDialogBg;
    public UILabel lbDialogName;
    public UILabel lbDialogText;

    [Header("Select")]
    public GameObject selectObject;
    public UISprite sprSelectBg;
    public UIGrid gridSelect;
    public UIButton[] btnSelects;

    public UISprite sprTouch;


	public void Start() {
		sprFade.width = sprSelectBg.width = Screen.width + 1;
		sprFade.height = sprSelectBg.height = Screen.height + 1;
	}

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    public void ShowDialog(bool show)
    {
        if (dialogObject == null)
        {
            Debug.LogError("Touch 라벨이 없습니다.");
            return;
        }

        if (show == true && gameObject.activeSelf == false)
            Show(true);

        dialogObject.gameObject.SetActive(show);
    }

    public void ShowSprFade(bool show)
    {
        sprFade.gameObject.SetActive(show);
        sprFade.color = new Color(sprFade.color.r, sprFade.color.g, sprFade.color.b, 1.0f);
    }

    public void ShowTouch(bool show)
    {
        if (sprTouch == null)
        {
            Debug.LogError("Touch 라벨이 없습니다.");
            return;
        }

        sprTouch.gameObject.SetActive(show);
    }

    public void ShowSelect(int selectCount, params string[] texts)
    {
        selectObject.SetActive(true);

        for (int i = 0; i < btnSelects.Length; i++)
        {
            if (i < selectCount)
            {
                UILabel lb = btnSelects[i].GetComponentInChildren<UILabel>(true);
                lb.textlocalize = texts[i];

                btnSelects[i].gameObject.SetActive(true);
            }
            else
                btnSelects[i].gameObject.SetActive(false);
        }

        gridSelect.Reposition();
    }

    public void HideSelect()
    {
        selectObject.SetActive(false);
    }

    public void OnSelectText1()
    {
        HideSelect();
        //World.Instance.storyMgr.OnSelectText1();
    }

    public void OnSelectText2()
    {
        HideSelect();
        //World.Instance.storyMgr.OnSelectText2();
    }

    public void OnSelectText3()
    {
        HideSelect();
        //World.Instance.storyMgr.OnSelectText3();
    }
}
