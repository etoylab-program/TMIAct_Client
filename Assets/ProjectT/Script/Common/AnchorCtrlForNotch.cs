using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorCtrlForNotch : MonoBehaviour
{
    private enum RatioMode
    {
        Lock,
        FitToNotch
    }

#if UNITY_EDITOR
    public static bool IsiPhoneX { get { return (Screen.width == 2436f && Screen.height == 1125f); } }
#else
    public static bool IsiPhoneX { get { return (SystemInfo.deviceModel == iPhoneX_Identifier_A || SystemInfo.deviceModel == iPhoneX_Identifier_B); } }
#endif
    private const string iPhoneX_Identifier_A = "iPhone10,3";
    private const string iPhoneX_Identifier_B = "iPhone10,6";
    private const string G7ThinQ_Identifier = "LGE LM-G710N";

    private const float moveAmount = 90f;
    [SerializeField] private RatioMode mode = RatioMode.Lock;
    [SerializeField] private GameObject blackPanel_L;
    [SerializeField] private GameObject blackPanel_R;
    [SerializeField] private List<UIAnchor> anchorList;


    private void Awake()
    {
        //if (mode != RatioMode.Lock)
        //{
        //    blackPanel_L.SetActive(false);
        //    blackPanel_R.SetActive(false);
        //}

        if (anchorList == null || anchorList.Count == 0)
            return;
        for (int i = 0; i < anchorList.Count; ++i)
        {
            anchorList[i].enabled = false;
        }
    }


    private void Start()
    {
        if (mode == RatioMode.Lock)
            return;

#if UNITY_EDITOR
        if (Screen.width == 2436f && Screen.height == 1125f)
        {
            StartCoroutine(this.coRePotitionUI());
            notchApplied = true;
        }
#else
        string deviceModel = SystemInfo.deviceModel;
        if (deviceModel == iPhoneX_Identifier_A || deviceModel == iPhoneX_Identifier_B)//|| SystemInfo.deviceModel == G7ThinQ_Identifier)
        {
            StartCoroutine(this.coRePotitionUI());
            this.enabled = false;
        }
#endif
    }
    IEnumerator coRePotitionUI()
    {
        //if (mode == RatioMode.Lock)
        //    moveAmount = (Screen.width - (Screen.height / 720f * 1280f)) * 0.5f;
        //else
            //moveAmount = 90f;//140.625f;

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < anchorList.Count; ++i)
        {
            UIAnchor anchor = anchorList[i];
            switch (anchor.side)
            {
                case UIAnchor.Side.BottomLeft:
                case UIAnchor.Side.Left:
                case UIAnchor.Side.TopLeft:
                    {
                        anchor.pixelOffset.x += moveAmount;
                    }
                    break;

                case UIAnchor.Side.BottomRight:
                case UIAnchor.Side.Right:
                case UIAnchor.Side.TopRight:
                    {
                        anchor.pixelOffset.x -= moveAmount;
                    }
                    break;
            }
            anchor.enabled = true;
        }
        float magnification = Screen.height / 720f;
        float panelPosX = (Screen.width / magnification) * 0.5f - moveAmount / magnification;
        blackPanel_L.transform.localPosition = new Vector3(-panelPosX, 0, 0);
        blackPanel_R.transform.localPosition = new Vector3(panelPosX, 0, 0);
        //blackPanel_L.SetActive(true);
        //blackPanel_R.SetActive(true);
        UICamera.onScreenResize();
    }

#if UNITY_EDITOR
    public bool apply;

    private bool notchApplied = false;


    private void OnDrawGizmos()
    {
        if (apply)
        {
            GetAllAnchors();
            apply = false;
        }
    }


    private void GetAllAnchors()
    {
        anchorList = new List<UIAnchor>();
        UIRoot root = FindObjectOfType<UIRoot>();
        if (root != null)
        {
            UIAnchor[] anchors = root.GetComponentsInChildren<UIAnchor>(true);
            if (anchors != null && anchors.Length > 0)
            {
                anchorList.AddRange(anchors);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            DeactivateNotch();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (anchorList == null || anchorList.Count == 0)
                return;

            if (notchApplied)
            {
                DeactivateNotch();
            }

            StartCoroutine(this.coRePotitionUI());
            UICamera.onScreenResize();
        }
    }

    private void DeactivateNotch()
    {
        if (anchorList == null || anchorList.Count == 0)
            return;
        for (int i = 0; i < anchorList.Count; ++i)
        {
            UIAnchor anchor = anchorList[i];
            anchor.pixelOffset = Vector2.zero;
        }
        UICamera.onScreenResize();
        blackPanel_L.SetActive(false);
        blackPanel_R.SetActive(false);
    }
#endif
}
