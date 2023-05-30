using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
#if AR_MODE
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;


public class ARManager : MonoBehaviour
{
    public GameObject kObjectToPlace;
    public GameObject placementIndicator;


    private ARSession arSession;
    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycastManager;

    private Camera currentCamera;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    public GameObject disableCam;
    bool flag = false;

    private GameObject kARSessionPre;
    private GameObject kAROriginPre;

    private UICamera _uiCamera;
    private Camera _uiCameraComp;
    void Start()
    {
        if (_uiCamera == null)
        {
            _uiCamera = UICamera.FindCameraForLayer((int)eLayer.UI);
            _uiCameraComp = _uiCamera.GetComponent<Camera>();
        }
        arSession = null;
        arOrigin = null;
        currentCamera = null;

        UnityEngine.Object placeObj = ResourceMgr.Instance.LoadFromAssetBundle("ui", "UI/ARMarker.prefab");
        if(placeObj != null)
            placementIndicator = Instantiate(placeObj) as GameObject;
    }

    public void SetARCamera(bool b)
    {
        if(b)
        {
            if (kARSessionPre == null)
            {
                kARSessionPre = new GameObject("ARSession");
                arSession = kARSessionPre.AddComponent<ARSession>();
                kARSessionPre.AddComponent<ARInputManager>();
                arSession.enabled = false;

                StartCoroutine(ARCheckAvailability());
            }
            
        }
        else
        {
            if (arSession != null)
                DestroyImmediate(arSession.gameObject);
            if (arOrigin != null)
                DestroyImmediate(arOrigin.gameObject);

            kARSessionPre = null;
            kAROriginPre = null;
            arSession = null;
            arOrigin = null;
            currentCamera = null;
        }
    }

    IEnumerator ARCheckAvailability()
    {
        if(ARSession.state == ARSessionState.None || ARSession.state == ARSessionState.CheckingAvailability)
        {
            yield return ARSession.CheckAvailability();
        }

        if (ARSession.state == ARSessionState.Unsupported || ARSession.state == ARSessionState.NeedsInstall || ARSession.state == ARSessionState.Installing)
        {
            string arStr = string.Empty;
#if UNITY_ANDROID
            arStr = "ARCore";
#elif UNITY_IOS
            arStr = "ARKit";
#endif
            //ARCore or ARKit을 지원하지 않는 기기입니다.
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3102), arStr));
            UIFigureRoomFreeLookPanel roomFreeLookPanel = (UIFigureRoomFreeLookPanel)LobbyUIManager.Instance.GetUI("FigureRoomFreeLookPanel");
            if(roomFreeLookPanel != null)
            {
                roomFreeLookPanel.OnBtnBack();
            }
            yield break;
        }
        else
        {
            arSession.enabled = true;
            if (kAROriginPre == null)
            {
                kAROriginPre = new GameObject("ARSessionOrigin");
                arOrigin = kAROriginPre.AddComponent<ARSessionOrigin>();
                arRaycastManager = kAROriginPre.AddComponent<ARRaycastManager>();
                kAROriginPre.AddComponent<ARPlaneManager>();
                kAROriginPre.transform.position = Vector3.zero;

                GameObject arCamera = new GameObject("ARCamera");
                currentCamera = arCamera.AddComponent<Camera>();
                currentCamera.clearFlags = CameraClearFlags.SolidColor;
                currentCamera.backgroundColor = Color.black;
                currentCamera.cullingMask = 1 << 10;
                currentCamera.depth = 1;
                currentCamera.nearClipPlane = 0.1f;
                currentCamera.farClipPlane = 20f;

                UnityEngine.SpatialTracking.TrackedPoseDriver tracked = arCamera.AddComponent<UnityEngine.SpatialTracking.TrackedPoseDriver>();
                tracked.SetPoseSource(UnityEngine.SpatialTracking.TrackedPoseDriver.DeviceType.GenericXRDevice, UnityEngine.SpatialTracking.TrackedPoseDriver.TrackedPose.ColorCamera);
                arCamera.AddComponent<ARCameraManager>();
                arCamera.AddComponent<ARCameraBackground>();

                arCamera.transform.parent = arOrigin.transform;
                arCamera.transform.localPosition = Vector3.zero;

                arOrigin.camera = currentCamera;
            }

            Debug.LogError(ARSession.state);
        }
    }

    void Update()
    {
        if (arOrigin == null || arOrigin == null)
            return;
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        //if (placementPoseIsValid || ((Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Began)) || Input.GetMouseButtonDown(0)))
        if(placementPoseIsValid && ((Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Began)) || 
                                     AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Select)))//Input.GetMouseButtonDown(0)))
        {
            if (UICamera.hoveredObject == null)
            {
                PlaceObject();
            }
            else
            {
                if(UICamera.hoveredObject.name.Equals("UIRoot"))
                {
                    PlaceObject();
                }
                else
                {
                    //UIObject
                    //Log.Show(UICamera.hoveredObject.name);
                }
            }
        }

    }

    void PlaceObject()
    {
        if (FigureRoomScene.Instance.SelectedFigureInfo == null)
        {
            Debug.LogError("Select Figure is NULL");
            return;
        }

        if (!FigureRoomScene.Instance.SelectedFigureInfo.figure.gameObject.activeSelf)
            FigureRoomScene.Instance.SelectedFigureInfo.figure.gameObject.SetActive(true);
        FigureRoomScene.Instance.SelectedFigureInfo.figure.gameObject.transform.position = placementPose.position;
    }

    private void UpdatePlacementPose()
    {
        Vector3 screenCenter = currentCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        arRaycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;

        if(placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            Vector3 cameraForward = currentCamera.transform.forward;
            Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    public void UpdatePlacementIndicator()
    {
        if(placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    //public void OnClickARBtn()
    //{
    //    flag = !flag;

    //    if (flag)
    //    {
    //        if (arSession == null)
    //        {
    //            var session = (GameObject)ResourceMgr.Instance.LoadFromAssetBundle("ui", "UI/AR_Session.prefab");
    //            if (session != null)
    //            {
    //                GameObject sessionObj = GameObject.Instantiate(session) as GameObject;
    //                arSession = sessionObj.GetComponent<ARSession>();
    //            }
    //        }
    //        if (arOrigin == null)
    //        {
    //            GameObject origin = (GameObject)ResourceMgr.Instance.LoadFromAssetBundle("ui", "UI/AR_Session_Origin.prefab");
    //            if (origin != null)
    //            {
    //                GameObject originObj = GameObject.Instantiate(origin) as GameObject;
    //                arOrigin = originObj.GetComponent<ARSessionOrigin>();
    //                arRaycastManager = originObj.GetComponent<ARRaycastManager>();
    //                currentCamera = arOrigin.camera;
    //            }
    //        }

    //        disableCam.SetActive(false);
    //    }
    //    else
    //    {
    //        if (arSession != null)
    //            DestroyImmediate(arSession.gameObject);
    //        if (arOrigin != null)
    //            DestroyImmediate(arOrigin.gameObject);

    //        arSession = null;
    //        arOrigin = null;
    //        currentCamera = null;

    //        disableCam.SetActive(true);
    //    }
    //}
}
#endif