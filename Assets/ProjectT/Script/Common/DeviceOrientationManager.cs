using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeviceOrientationManager : MonoSingleton<DeviceOrientationManager>
{
    public class cEdgeInset  //ScreenParameter
    {
        public int Left;
        public int Bottom;
        public int Right;
        public int Top;

        public int Width;
        public int Height;

        public cEdgeInset(int left, int bottom, int right, int top, int width, int height)
        {
            Left = left;
            Bottom = bottom;
            Right = right;
            Top = top;
            Width = width;
            Height = height;
        }
    }

    public class cRect
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public float Left;
        public float Bottom;
        public float Right;
        public float Top;
        public float ScreenWidth;
        public float ScreenHeight;

        public cRect()
        {
        }

        public cRect(float x, float y, float width, float height, float left, float bottom, float right, float top, float screenWidth, float screenHeight)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Left = left;
            Bottom = bottom;
            Right = right;
            Top = top;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
        }

        public cRect Flip()
        {
            return new cRect(-X, -Y, Width, Height, Right, Top, Left, Bottom, ScreenWidth, ScreenHeight);
        }
    }

    public enum ePHONESTATE
    {
        NONE,
        DEFAULT,
        IPHONE_X,
    }

    private bool m_bIsInit = false;
    private DeviceOrientation m_currentOrientation;
    public DeviceOrientation CurrentOrientation { get { return m_currentOrientation;} }
    private DeviceOrientation m_changeOrientation;
    private Dictionary<ePHONESTATE, cEdgeInset> m_dicEdgeInsets;
    private cRect m_leftRect;
    private cRect m_rightRect;
    private UIRoot m_CurrentSceneUIRoot = null;

    public ePHONESTATE CurrentPhoneState { get; set; }


    private void Awake() { Init(); }
    private void Start() { Init(); }
    void Init()
    {
        if (m_bIsInit)
            return;

        m_bIsInit = true;

        //191122 현재 아이폰만 적용 하기로함
#if UNITY_ANDROID && !UNITY_EDITOR
        DestroyImmediate(this.gameObject);
        return;
#endif


        m_dicEdgeInsets = new Dictionary<ePHONESTATE, cEdgeInset>();
        m_dicEdgeInsets.Add(ePHONESTATE.IPHONE_X, new cEdgeInset(80, 0, 0, 0, 2436, 1125));     //80은 노치 너비

        CurrentPhoneState = GetCurrentPhoneState();
#if !UNITY_EDITOR
        if(CurrentPhoneState != ePHONESTATE.IPHONE_X)
        {
            DestroyImmediate(this);
            return;
        }
#endif

        DontDestroyOnLoad();

        SetDeviceOrientation(GetDeviceOrientation());

        m_leftRect = CalculateNGUIRect();
        m_rightRect = m_leftRect != null ? m_leftRect.Flip() : null;
#if !UNITY_EDITOR
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
#endif
#if UNITY_EDITOR
        EditorChangeScreenOrientation(m_currentOrientation);
#endif
    }

    protected override void OnApplicationQuit()
    {
#if !UNITY_EDITOR
        base.OnApplicationQuit();
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
#endif
    }

    void OnApplicationFocus(bool focusStatus)
    {
#if !UNITY_EDITOR
        if(focusStatus)
        {
            CheckDeviceOrientation();
        }
#endif
    }

    //SceneLoaded Ended CallBack
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if(AppMgr.Instance.SceneType != AppMgr.eSceneType.Title)
        {
            if(!GameInfo.Instance.GameConfig.NotchTake)
            {
                DestroyImmediate(this);
                return;
            }
        }

#if !UNITY_EDITOR
        GetUIRoot();
#endif
    }

    void GetUIRoot()
    {
        if (GameObject.Find("UIRoot").GetComponent<UIRoot>() != null)
        {
            m_CurrentSceneUIRoot = GameObject.Find("UIRoot").GetComponent<UIRoot>();
        }
        else
        {
            Debug.LogError("UIRoot이름을 가진 오브젝트가 존재하지않습니다.");
            return;
        }
    }

    private ePHONESTATE GetCurrentPhoneState()
    {
#if UNITY_IOS || UNITY_EDITOR
        float temp = (float)((float)Screen.height / (float)Screen.width);

        if (temp <= 0.5f)   //세로가 긴 폰들은 0.5이하로 값이 나온다 예)0.462...
        {
            return ePHONESTATE.IPHONE_X;
        }
#endif

        return ePHONESTATE.DEFAULT;
    }


    DeviceOrientation GetDeviceOrientation()
    {

        if (Input.deviceOrientation == DeviceOrientation.Portrait || 
            Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown ||
            Input.deviceOrientation == DeviceOrientation.FaceUp || 
            Input.deviceOrientation == DeviceOrientation.FaceUp ||
            Input.deviceOrientation == DeviceOrientation.Unknown)
        {
            return DeviceOrientation.Unknown;
        }


        return Input.deviceOrientation;
    }

    public void SetDeviceOrientation(DeviceOrientation deviceOrientation)
    {
        m_currentOrientation = deviceOrientation;
        Debug.LogWarning("DeviceOrientation : " + m_currentOrientation);
        if (m_CurrentSceneUIRoot == null)
            GetUIRoot();
        ResizePanel(m_CurrentSceneUIRoot.gameObject);
    }

    private void Update()
    {
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            return;

        if (!m_bIsInit)
            return;
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.O))
            EditorChangeScreenOrientation(DeviceOrientation.LandscapeLeft);
        if(Input.GetKeyDown(KeyCode.I))
            EditorChangeScreenOrientation(DeviceOrientation.LandscapeRight);
#endif
#if !UNITY_EDITOR
        CheckDeviceOrientation();
#endif
    }

    bool CheckDeviceOrientation()
    {
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            return false;
        m_changeOrientation = GetDeviceOrientation();

        if (m_changeOrientation == DeviceOrientation.Unknown)
            return false;
        if (m_currentOrientation != m_changeOrientation)
        {
            SetDeviceOrientation(m_changeOrientation);
            return true;
        }

        return false;
    }

    private cEdgeInset GetPhoneEdgeInset()
    {
        cEdgeInset _edgeInset = null;
        m_dicEdgeInsets.TryGetValue(CurrentPhoneState, out _edgeInset);

        return _edgeInset;
    }

    private cRect CalculateNGUIRect()
    {
        cEdgeInset edgeInset = GetPhoneEdgeInset();
        if (edgeInset == null)
        {
            return null;
        }
        if (m_CurrentSceneUIRoot == null)
            GetUIRoot();

        if (m_CurrentSceneUIRoot == null)
            return null;

        float pixelSizeAdjustment = m_CurrentSceneUIRoot.pixelSizeAdjustment;
        Vector2 screen = NGUITools.screenSize;
        float widthScale = 1f;
        float heightScale = 1f;

#if UNITY_EDITOR
        widthScale = screen.x / edgeInset.Width;
        heightScale = screen.y / edgeInset.Height;
#endif

        float width = screen.x - (edgeInset.Left - edgeInset.Right) * widthScale;
        float height = screen.y - (edgeInset.Top - edgeInset.Bottom) * heightScale;
        cRect rect = new cRect
        {
            Width = width * pixelSizeAdjustment,
            Height = height * pixelSizeAdjustment,
            X = (edgeInset.Left - edgeInset.Right) * pixelSizeAdjustment / 2 * widthScale,
            Y = (edgeInset.Bottom - edgeInset.Top) * pixelSizeAdjustment / 2 * heightScale,
            Left = edgeInset.Left * pixelSizeAdjustment * widthScale,
            Bottom = edgeInset.Bottom * pixelSizeAdjustment * heightScale,
            Right = edgeInset.Right * pixelSizeAdjustment * widthScale,
            Top = edgeInset.Top * pixelSizeAdjustment * heightScale,
            ScreenWidth = edgeInset.Width * pixelSizeAdjustment * widthScale,
            ScreenHeight = edgeInset.Height * pixelSizeAdjustment * heightScale,

        };

        return rect;
    }

    public cRect GetRect()
    {
        if (m_rightRect == null || m_leftRect == null)
        {
            return null;
        }

        if (m_currentOrientation == DeviceOrientation.LandscapeRight )
        {
            return m_rightRect;
        }
        else
        {
            return m_leftRect;
        }
    }

    public void ResizePanel(GameObject go, bool clip = false)
    {
        Log.Show("####");
        if (GetRect() != null)
        {
            cRect rect = GetRect();
            UIPanel[] panels = go.GetComponentsInChildren<UIPanel>(true);
            if (panels != null && panels.Length > 0)
            {
                /*
                panel.clipping = clip ? UIDrawCall.Clipping.SoftClip : UIDrawCall.Clipping.ConstrainButDontClip;
                panel.SetRect(rect.X, rect.Y, rect.Width, rect.Height);

                var rects = go.GetComponentsInChildren<UIRect>(true);
                var count = rects.Length;
                for (int i = 0; i < count; i++)
                {
                    rects[i].UpdateAnchors();
                }

                UIAnchor[] anchors = go.GetComponentsInChildren<UIAnchor>(true);
                foreach (UIAnchor anchor in anchors)
                {
                    anchor.enabled = true;
                }
                */

                foreach (UIPanel panel in panels)
                {
                    if (panel.gameObject.name.Contains("BGUnit") || panel.gameObject.name.Contains("UIRoot"))
                    {
                        continue;
                    }

                    if(panel.clipping == UIDrawCall.Clipping.None || panel.clipping == UIDrawCall.Clipping.ConstrainButDontClip)
                    {
                        panel.clipping = UIDrawCall.Clipping.ConstrainButDontClip;
                        panel.SetRect(rect.X, rect.Y, rect.Width, rect.Height);

                    }

                    UIRect[] rects = panel.GetComponentsInChildren<UIRect>(true);
                    for (int i = 0; i < rects.Length; i++)
                        rects[i].UpdateAnchors();

                    UIAnchor[] anchors = panel.GetComponentsInChildren<UIAnchor>(true);
                    foreach (UIAnchor anchor in anchors)
                    {
                        if (!anchor.kAnchorManagerInfluence)
                        {
                            continue;
                        }



                        anchor.container = panel.gameObject;
                        anchor.enabled = true;
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
public void EditorChangeScreenOrientation(DeviceOrientation deviceOrientation)
    {
        m_changeOrientation = deviceOrientation;
        if(CurrentPhoneState == ePHONESTATE.IPHONE_X)
        {
            SetDeviceOrientation(m_changeOrientation);
        }

    }
#endif
}
