
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIScreenShotListSlot : FSlot
{
    [Header("[Property]")]
    public UITexture    TexScreenShot;
    public UISprite     SprSelBg;
    public UILabel      LbSelNumber;
    public UISprite     SprGIF;

    public int SelectNumber { get; private set; }

    private UIFigureAlbumPopup          mUIFigureAlbumPopup = null;
    private int                         mIndex              = 0;
    private UIFigureAlbumPopup.sImgInfo mImgInfo            = null;


    public void UpdateSlot(int index, UIFigureAlbumPopup.sImgInfo imgInfo)
    {
        if (mUIFigureAlbumPopup == null)
        {
            mUIFigureAlbumPopup = ParentGO.GetComponent<UIFigureAlbumPopup>();
        }

        mIndex = index;
        mImgInfo = imgInfo;

        Show(true);

        SprGIF.gameObject.SetActive(imgInfo.IsGIF);

        if (imgInfo.SelectedNumber > -1)
        {
            Select();
        }
        else
        {
            Unselect();
        }

        TexScreenShot.mainTexture = mImgInfo.MainTex;
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    public int Release()
    {
        if(mImgInfo == null)
        {
            return -1;
        }

        int compare = -1;
        if (mImgInfo.MainTex != null)
        {
            int generation = mImgInfo.Release();
            if (generation > compare)
            {
                compare = generation;
            }
        }
        
        return compare;
    }

    public void OnBtnSelect()
    {
        LobbyUIManager.Instance.kHoldGauge.Hide();

        if (SelectNumber > -1)
        {
            if (mUIFigureAlbumPopup.UnselectTexture(mIndex))
            {
                Unselect();
            }
        }
        else
        {
            SelectNumber = mUIFigureAlbumPopup.SelectTexture(mIndex);
            if (SelectNumber == -1)
            {
                return;
            }

            Select();
        }
    }

    public void OnBtnPressStart()
    {
        LobbyUIManager.Instance.kHoldGauge.Show(this.transform.position, 1.5f);
    }

    public void OnBtnPressing()
    {
        LobbyUIManager.Instance.kHoldGauge.Hide();
        mUIFigureAlbumPopup.ShowScreenShot(mIndex);
    }

    public void OnBtnPressed()
    {
        LobbyUIManager.Instance.kHoldGauge.Hide();
    }

    public void RefreshSelectNumber()
    {
        --SelectNumber;
        LbSelNumber.textlocalize = (SelectNumber + 1).ToString();
    }

    public void Select()
    {
        SelectNumber = mImgInfo.SelectedNumber;

        SprSelBg.gameObject.SetActive(true);
        LbSelNumber.textlocalize = (SelectNumber + 1).ToString();
    }

    public void Unselect()
    {
        SelectNumber = -1;
        SprSelBg.gameObject.SetActive(false);
    }

    /*private void FixedUpdate()
    {
        if (mImgInfo == null || mImgInfo.ListTexFrame == null)
        {
            return;
        }

        if(mUIFigureAlbumPopup.ScreenShotObj.gameObject.activeSelf)
        {
            return;
        }

        UIScreenShotPopup uiScreenShotPopup = LobbyUIManager.Instance.GetActiveUI<UIScreenShotPopup>("ScreenShotPopup");
        if(uiScreenShotPopup)
        {
            Debug.Log("스크린샷 팝업이 떠있네~");
            return;
        }

        mImgInfo.UpdateGIFFrame(TexScreenShot);
    }*/
}
 
