#define USE_DELEGATE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceCameraScreenShot : MonoBehaviour
{
#if USE_DELEGATE
    public delegate void OnDeviceCamStart();
    public delegate void OnDeviceCamEnd();

    public OnDeviceCamStart onDeviceCamStart;
    public OnDeviceCamEnd onDeviceCamEnd;
#endif
#if !USE_DELEGATE
    public UIButton startCamBtn;
    public UIButton EndCamBtn;
    public UIButton switchCamBtn;
    public GameObject BG;
#endif

    private bool camAvailable;
    private bool isFrontCam;
    private WebCamTexture backCam;
    private Texture defaultBackground;
    private WebCamDevice[] devices;
    private CameraClearFlags originClearFlag;
    private AspectRatioFitter fit;

    public Camera deviceCamera;
    public RawImage background;
    public bool IsCamAvailable { get { return this.camAvailable; } }

    private void Start()
    {
        originClearFlag = Camera.main.clearFlags;
        if (AnchorCtrlForNotch.IsiPhoneX)
        {
            fit = background.GetComponent<AspectRatioFitter>();
            if (fit != null)
            {
                fit.aspectRatio = 2202f / 1125f;
                fit.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
            }
        }
        Init();
    }

    private void Init()
    {
#if !USE_DELEGATE
        BG.SetActive(true);
        startCamBtn.gameObject.SetActive(true);
        EndCamBtn.gameObject.SetActive(false);
        switchCamBtn.gameObject.SetActive(false);
#endif
        Camera.main.clearFlags = originClearFlag;
        deviceCamera.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
        camAvailable = false;
        isFrontCam = false;
    }

    public void CamStart()
    {
#if USE_DELEGATE
        if (onDeviceCamStart != null)
        {
            onDeviceCamStart();
        }
#endif
#if !USE_DELEGATE
        BG.SetActive(false);
        EndCamBtn.gameObject.SetActive(true);
        startCamBtn.gameObject.SetActive(false);
        switchCamBtn.gameObject.SetActive(true);
#endif
        Camera.main.clearFlags = CameraClearFlags.Depth;
        deviceCamera.gameObject.SetActive(true);
        background.gameObject.SetActive(true);

        defaultBackground = background.texture;
        devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("No camera detected");
#if UNITY_EDITOR
            camAvailable = true;
            return;
#endif
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; ++i)
        {
#if !UNITY_EDITOR
            if (!devices[i].isFrontFacing)
#endif
            {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }
        if (backCam == null)
        {
            Debug.Log("Unable to find backcam");
            return;
        }

#if UNITY_IOS
        background.rectTransform.localScale = new Vector3(1, -1, 1);
#endif
        backCam.Play();
        background.texture = backCam;
        camAvailable = true;
    }
    public void CamEnd()
    {
        if (camAvailable && backCam != null)
            backCam.Stop();

#if USE_DELEGATE
        if (onDeviceCamEnd != null)
        {
            onDeviceCamEnd();
        }
#endif
        background.texture = null;
        Init();

    }

    public void OnClick_SwitchCamra()
    {
        if (camAvailable && backCam != null)
        {
            backCam.Stop();
            isFrontCam = !isFrontCam;
            for (int i = 0; i < devices.Length; ++i)
            {
                if (devices[i].isFrontFacing == isFrontCam)
                {
                    backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                }
            }

            if (backCam == null)
            {
                Debug.Log("Unable to find backcam");
                return;
            }

#if UNITY_IOS
				background.rectTransform.localScale = background.rectTransform.localScale == Vector3.one ? new Vector3(1, -1, 1) : Vector3.one;
#elif UNITY_ANDROID
                background.rectTransform.localScale = isFrontCam ? new Vector3(-1, 1, 1) : Vector3.one;
#endif
            backCam.Play();
            background.texture = backCam;
        }
    }

    public void SetBackCamActive(bool value)
    {
        if (camAvailable == false || backCam == null) return;
        if (value)
            backCam.Play();
        else
            backCam.Stop();
    }

}
