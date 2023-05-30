
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uGIF;


public class UIFigureAlbumPopup : FComponent
{
    public class sImgInfo
    {
        public int  SelectedNumber { get; set; } = -1;

        public string       Path    { get; private set; } = string.Empty;
        public Texture2D    MainTex { get; private set; } = null;
        public bool         IsGIF   { get; private set; } = false;


        public sImgInfo(string imgPath)
        {
            Path = imgPath;
            SelectedNumber = -1;
            MainTex = null;
        }

        public int Release()
        {
            if(MainTex == null)
            {
                return -1;
            }

            int generation = System.GC.GetGeneration(MainTex);

            Object.DestroyImmediate(MainTex);
            MainTex = null;

            return generation;
        }

        public void LoadImage(UniGif gifDecoder)
        {
            MainTex = new Texture2D(2, 2);

            string ext = System.IO.Path.GetExtension(Path);
            ext.ToLower();

            if (ext.CompareTo(".gif") == 0 && gifDecoder != null)
            {
                IsGIF = true;

                string pngPath = Path.Replace(".gif", ".png");
                byte[] bytes = File.ReadAllBytes(pngPath);

                MainTex.LoadImage(bytes);
            }
            else if (ext.CompareTo(".png") == 0)
            {
                IsGIF = false;

                byte[] bytes = File.ReadAllBytes(Path);
                MainTex.LoadImage(bytes);
            }
        }
    }


    [Header("[Property]")]
    public UILabel          lbCount;
    public GameObject       ScreenShotObj;
    public UITexture        TexScreenShot;
    public UILabel          kNoneLabel;

    [Header("[ScreenShot List]")]
    public UIScreenShotListSlot[] ScreenShotList;

    [Header("[Loading]")]
    public GameObject       LoadingObj;
    public UIProgressBar    PBLoading;

    public UniGif GIFDecoder { get; private set; } = null;

    private int             mTotalCount     = 0;     
    private int             mCurrentPage    = 0;
    private List<sImgInfo>  mListImgInfo    = new List<sImgInfo>();
    private GIFEncoder      mGIFEncoder     = null;
    
    private List<UniGif.GifTexture> mListTexFrame       = null;
    private int                     mCurGIFFrameIndex   = 0;
    private float                   mCheckTime          = 0;
    private int                     mScreenShotIndex    = 0;
    private bool                    mbLockBtn           = false;


    public override void Awake()
    {
        base.Awake();

        mGIFEncoder = new GIFEncoder();
        mGIFEncoder.useGlobalColorTable = true;
        mGIFEncoder.repeat = 0;
        mGIFEncoder.FPS = 6;
        mGIFEncoder.quality = 1;

        GIFDecoder = new UniGif();
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        GetImagePaths();

        mCurrentPage = 0;
        LoadCurrentPage();

        ScreenShotObj.SetActive(false);

        mbLockBtn = false;
    }

    public int SelectTexture(int index)
    {
        if(GetSelectedTexCount() >= GameInfo.Instance.GameConfig.ScreenShotMaxSelectCount)
        {
            return -1;
        }

        if (index < 0 || index >= mListImgInfo.Count)
        {
            return -1;
        }

        mListImgInfo[index].SelectedNumber = GetSelectedTexCount();
        return mListImgInfo[index].SelectedNumber;
    }

    public bool UnselectTexture(int index)
    {
        if(index < 0 || index >= mListImgInfo.Count)
        {
            return false;
        }

        int currentNumber = mListImgInfo[index].SelectedNumber;

        for (int i = 0; i < mListImgInfo.Count; i++)
        {
            if (mListImgInfo[i].SelectedNumber > currentNumber)
            {
                --mListImgInfo[i].SelectedNumber;
            }
        }

        for(int i = 0; i < ScreenShotList.Length; i++)
        {
            if(ScreenShotList[i].SelectNumber > currentNumber)
            {
                ScreenShotList[i].RefreshSelectNumber();
            }
        }

        mListImgInfo[index].SelectedNumber = -1;
        return true;
    }

    public void ShowScreenShot(int index)
    {
        mScreenShotIndex = index;

        if (mListImgInfo[mScreenShotIndex].IsGIF)
        {
            if (LoadGIF(mListImgInfo[mScreenShotIndex].Path))
            {
                ScreenShotObj.SetActive(true);
                TexScreenShot.mainTexture = mListTexFrame[mCurGIFFrameIndex].m_texture2d;
            }
            else
            {
                Debug.LogError(mListImgInfo[mScreenShotIndex].Path + "를 로드할 수 없습니다.");
            }
        }
        else
        {
            ScreenShotObj.SetActive(true);

            mListImgInfo[mScreenShotIndex].LoadImage(GIFDecoder);
            TexScreenShot.mainTexture = mListImgInfo[mScreenShotIndex].MainTex;
        }
    }

    public void UpdateGIFFrame(UITexture uiTex)
    {
        if (mListTexFrame == null)
        {
            return;
        }

        mCheckTime += Time.fixedDeltaTime;
        if (mCheckTime >= mListTexFrame[0].m_delaySec)
        {
            mCheckTime = 0.0f;

            if (++mCurGIFFrameIndex >= mListTexFrame.Count)
            {
                mCurGIFFrameIndex = 0;
            }

            uiTex.mainTexture = mListTexFrame[mCurGIFFrameIndex].m_texture2d;
        }
    }

    public void ReleaseGIF()
    {
        if (mListTexFrame == null)
        {
            return;
        }

        int compare = -1;
        for (int i = 0; i < mListTexFrame.Count; i++)
        {
            int generation = mListTexFrame[i].Release();
            if(generation > compare)
            {
                compare = generation;
            }

            mListTexFrame[i] = null;
        }

        mListTexFrame = null;

        if (compare >= 0)
        {
            //System.GC.Collect(compare);
        }
    }

    public bool IsFull()
    {
        if(!gameObject.activeSelf)
        {
            GetImagePaths();
        }

        if(mListImgInfo.Count >= mTotalCount)
        {
            return true;
        }

        return false;
    }

    public void OnBtnPrev()
    {
        if(mbLockBtn)
        {
            return;
        }

        int beforePage = mCurrentPage;

        if (mCurrentPage <= 0)
        {
            mCurrentPage = Mathf.CeilToInt((float)mListImgInfo.Count / (float)GameInfo.Instance.GameConfig.ScreenShotCountPerPage) - 1;
        }
        else
        {
            --mCurrentPage;
        }

        if (mCurrentPage < 0) mCurrentPage = 0;
        

        if (beforePage == mCurrentPage)
        {
            return;
        }

        mbLockBtn = true;
        LoadCurrentPage();

        StopCoroutine("CheckLoadImage");
        StartCoroutine("CheckLoadImage");
    }

    public void OnBtnNext()
    {
        if(mbLockBtn)
        {
            return;
        }

        int beforePage = mCurrentPage;

        int maxPage = Mathf.CeilToInt((float)mListImgInfo.Count / (float)GameInfo.Instance.GameConfig.ScreenShotCountPerPage) - 1;
        if (mCurrentPage >= maxPage)
        {
            mCurrentPage = 0;
        }
        else
        {
            ++mCurrentPage;
        }

        if(beforePage == mCurrentPage)
        {
            return;
        }

        mbLockBtn = true;
        LoadCurrentPage();

        StopCoroutine("CheckLoadImage");
        StartCoroutine("CheckLoadImage");
    }

    public void OnBtnMake()
    {
        if(mListImgInfo.Count <= 0 || GetSelectedTexCount() <= 0)
        {
            return;
        }

        if(IsFull())
        {
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(3154), null);
            return;
        }

        StartCoroutine("MakeGIF");
    }

    public void OnBtnCancelSelect()
    {
        for(int i = 0; i < mListImgInfo.Count; i++)
        {
            mListImgInfo[i].SelectedNumber = -1;
        }

        for(int i = 0; i < ScreenShotList.Length; i++)
        {
            ScreenShotList[i].Unselect();
        }
    }

    public void OnBtnDelete()
    {
        int selectedTexCount = GetSelectedTexCount();
        if(selectedTexCount <= 0)
        {
            return;
        }

        string msg = string.Format(FLocalizeString.Instance.GetText(1417), selectedTexCount);
        MessagePopup.OKCANCEL(eTEXTID.APPLY, msg, DeleteSelectedScreenShot);
    }

    public void OnBtnShare()
    {
        if(GetSelectedTexCount() > 1)
        {
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(1419), null);
            return;
        }

        sImgInfo firstSelectedImgInfo = null;
        for(int i = 0; i < mListImgInfo.Count; i++)
        {
            if(mListImgInfo[i].SelectedNumber > -1)
            {
                firstSelectedImgInfo = mListImgInfo[i];
                break;
            }
        }

        if(firstSelectedImgInfo == null)
        {
            return;
        }

        if(firstSelectedImgInfo.IsGIF)
        {
            LoadGIF(firstSelectedImgInfo.Path);
        }

        ScreenShotPopup.ShowScreenShotPopup(this, firstSelectedImgInfo);
    }

    public void OnBtnScreenShotClosee()
    {
        TexScreenShot.mainTexture = null;
        ReleaseGIF();

        ScreenShotObj.SetActive(false);
    }

    public void OnBtnBack()
    {
        OnClickClose();
    }

    private void GetImagePaths()
    {
        mTotalCount = GameInfo.Instance.GameConfig.ScreenShotCountPerPage * GameInfo.Instance.GameConfig.ScreenShotPageCount;
        List<string> listImgPath = new List<string>();

        string[] files = Directory.GetFiles(FigureRoomScene.Instance.ScreenShotPath, "*.png");
        for (int i = 0; i < files.Length; i++)
        {
            string gifPath = files[i].Replace(".png", ".gif");
            if (File.Exists(gifPath))
            {
                continue;
            }

            //앨범에는 로비 BG안뜨도록
            if (Path.GetFileName(files[i]).Equals(GameInfo.Instance.GameConfig.LobbyBGFileName))
                continue;

            listImgPath.Add(files[i]);
        }

        files = Directory.GetFiles(FigureRoomScene.Instance.ScreenShotPath, "*.gif");
        for (int i = 0; i < files.Length; i++)
        {
            listImgPath.Add(files[i]);
        }

        listImgPath.Sort();

        // 저장된 스크린샷이 50개 이상이면 오래된 스크린샷부터 50장이 될때까지 지워줌
        if (listImgPath.Count > mTotalCount)
        {
            List<string> listDelete = new List<string>();

            int deleteCount = listImgPath.Count - mTotalCount;
            for (int i = 0; i < deleteCount; i++)
            {
                File.Delete(listImgPath[i]);
                listDelete.Add(listImgPath[i]);
            }

            for (int i = 0; i < listDelete.Count; i++)
            {
                listImgPath.Remove(listDelete[i]);
            }
        }

        mListImgInfo.Clear();
        for (int i = 0; i < listImgPath.Count; i++)
        {
            sImgInfo imgInfo = new sImgInfo(listImgPath[i]);
            mListImgInfo.Add(imgInfo);
        }
    }

    private void LoadCurrentPage()
    {
        int compare = -1;
        for(int i = 0; i < ScreenShotList.Length; i++)
        {
            int generation = ScreenShotList[i].Release();
            if(generation > compare)
            {
                compare = generation;
            }

            ScreenShotList[i].Show(false);
        }

        if (compare >= 0)
        {
            //System.GC.Collect(compare);
        }
        if(mListImgInfo.Count > 0)
        {
            for (int i = 0; i < ScreenShotList.Length; i++)
        {
            int index = i + (mCurrentPage * GameInfo.Instance.GameConfig.ScreenShotCountPerPage);
            if (index >= mListImgInfo.Count)
            {
                continue;
            }

            mListImgInfo[index].LoadImage(GIFDecoder);

            ScreenShotList[i].ParentGO = gameObject;
            ScreenShotList[i].UpdateSlot(index, mListImgInfo[index]);
        }
        }
        

        SetCurrentPageLabel();
    }

    private int GetSelectedTexCount()
    {
        int count = 0;
        for(int i = 0; i < mListImgInfo.Count; i++)
        {
            if(mListImgInfo[i].SelectedNumber > -1)
            {
                ++count;
            }
        }

        return count;
    }

    private IEnumerator CheckLoadImage()
    {
        yield return new WaitForSeconds(0.3f);
        mbLockBtn = false;
    }

    private void SetCurrentPageLabel()
    {
        kNoneLabel.SetActive(false);
        if (mListImgInfo.Count <= 0)
        {
            lbCount.textlocalize = "";

            kNoneLabel.SetActive(true);
        }
        else
        {
            int maxPage = Mathf.CeilToInt((float)mListImgInfo.Count / (float)GameInfo.Instance.GameConfig.ScreenShotCountPerPage);
            lbCount.textlocalize = string.Format("{0}/{1}", mCurrentPage + 1, maxPage);
        }
    }

    private IEnumerator MakeGIF()
    {
        List<sImgInfo> listSelectedImgInfo = new List<sImgInfo>();
        for (int i = 0; i < mListImgInfo.Count; i++)
        {
            if (mListImgInfo[i].SelectedNumber <= -1 || mListImgInfo[i].IsGIF)
            {
                continue;
            }

            listSelectedImgInfo.Add(mListImgInfo[i]);
        }

        if(listSelectedImgInfo.Count <= 0)
        {
            yield break;
        }

        LoadingObj.SetActive(true);
        PBLoading.value = 0.0f;

        yield return new WaitForSeconds(0.1f);

        using (MemoryStream gifStream = new MemoryStream())
        {
            mGIFEncoder.Start(gifStream);

            int generation = -1;
            for (int i = 0; i < listSelectedImgInfo.Count; i++)
            {
                PBLoading.value += 1.0f / listSelectedImgInfo.Count;
                listSelectedImgInfo[i].LoadImage(GIFDecoder);

                Image img = new Image(listSelectedImgInfo[i].MainTex); //listImg[i];
                img.ResizeBilinear((int)((float)Screen.width * GameInfo.Instance.GameConfig.ScreenShotGIFRatio), 
                                   (int)((float)Screen.height * GameInfo.Instance.GameConfig.ScreenShotGIFRatio));
                img.Flip();
                
                mGIFEncoder.AddFrame(img);

                if (i > 0)
                {
                    generation = listSelectedImgInfo[i].Release();
                    listSelectedImgInfo[i] = null;
                }

                if (generation >= 0)
                {
                    //System.GC.Collect(generation);
                }

                yield return null;
            }

            mGIFEncoder.Finish();

            string fileName = GameSupport.GetCurrentServerTime().ToString("yyyy-MM-dd_HH-mm-ss");
            File.WriteAllBytes(FigureRoomScene.Instance.ScreenShotPath + "/" + fileName + ".png", listSelectedImgInfo[0].MainTex.EncodeToPNG());

            generation = listSelectedImgInfo[0].Release();
            if(generation >= 0)
            {
                //System.GC.Collect(generation);
            }

            listSelectedImgInfo[0] = null;
            listSelectedImgInfo.Clear();

            using (FileStream fs = new FileStream(FigureRoomScene.Instance.ScreenShotPath + "/" + fileName + ".gif", FileMode.Create))
            {
                gifStream.WriteTo(fs);
                fs.Close();
            }

            LoadGIF(FigureRoomScene.Instance.ScreenShotPath + "/" + fileName + ".gif");
            gifStream.Close();
        }

        LoadingObj.SetActive(false);
        
        GetImagePaths();
        mCurrentPage = Mathf.CeilToInt((float)mListImgInfo.Count / (float)GameInfo.Instance.GameConfig.ScreenShotCountPerPage) - 1;
        LoadCurrentPage();

        ScreenShotPopup.ShowScreenShotPopup(this, mListImgInfo[mListImgInfo.Count - 1]);
    }

    private bool LoadGIF(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        GIFDecoder.GetTextureListCoroutine(bytes, (listTexFrame, loopCount, width, height) =>
        {
            mListTexFrame = listTexFrame;
        });

        mCurGIFFrameIndex = 0;
        mCheckTime = 0.0f;

        return (mListTexFrame != null && mListTexFrame.Count > 0);
    }

    private void DeleteSelectedScreenShot()
    {
        for (int i = 0; i < mListImgInfo.Count; i++)
        {
            sImgInfo imgInfo = mListImgInfo[i];
            if(imgInfo.SelectedNumber <= -1)
            {
                continue;
            }

            if (File.Exists(imgInfo.Path))
            {
                if(imgInfo.IsGIF)
                {
                    string pngPath = imgInfo.Path.Replace(".gif", ".png");
                    if(File.Exists(pngPath))
                    {
                        File.Delete(pngPath);
                    }
                }

                File.Delete(imgInfo.Path);
                mListImgInfo.RemoveAt(i);

                --i;
            }
        }

        int maxPage = Mathf.CeilToInt((float)mListImgInfo.Count / (float)GameInfo.Instance.GameConfig.ScreenShotCountPerPage) - 1;
        if(mCurrentPage > maxPage)
        {
            mCurrentPage = maxPage;
        }

        LoadCurrentPage();
    }

    private void FixedUpdate()
    {
        if(!ScreenShotObj.gameObject.activeSelf)
        {
            return;
        }

        UpdateGIFFrame(TexScreenShot);
    }
}
