using UnityEngine;



public class UIThrowScope : MonoBehaviour
{
    

    public eScopeDir kScoprDir = eScopeDir.None;
    public ThrowScope kThrowScope;
    public GameObject kThrowOnObj;
    public GameObject kThrowOffObj;

    public Camera uiCamera { get; set; }
    private float _posX = 0f;
    private Vector3 _originLocalPos = Vector3.zero;
    private Vector3 _originPos = Vector3.zero;
    private ThrowObj _targetThrowObj = null;
    float UIHeight;

    private Unit.sIKObject _ikObj = null;

    void Start()
    {
        UIHeight = GameObject.Find("UI Root").GetComponent<UIRoot>().activeHeight;  // NGUI의 높이 픽셀

        Log.Show(UIHeight);

        //this.transform.localPosition = GetScopeScreenPos();
        _posX = this.transform.localPosition.x;
        _originLocalPos = this.transform.localPosition;

        Vector3 screenPos = uiCamera.WorldToScreenPoint(this.transform.position);
        _originPos = this.transform.position;
        _targetThrowObj = null;
    }

    // Update is called once per frame

    void Update()
    {
        if (uiCamera == null)
            return;
        if (kThrowScope == null)
            return;
        Vector3 screenPos = uiCamera.WorldToScreenPoint(this.transform.position); // 과녁을 스크린 포지션 바꿈
        //Vector3 screenPos = uiCamera.WorldToScreenPoint(_originPos); // 과녁을 스크린 포지션 바꿈
        _targetThrowObj = ThrowingManager.Instance.GetThrowObj(screenPos, kScoprDir);
        this.transform.localPosition = GetScopePosition();

        kThrowScope.SetBaxCast();

        kThrowOnObj.SetActive(kThrowScope.bIsTrigger);
        kThrowOffObj.SetActive(!kThrowScope.bIsTrigger);

        if (_targetThrowObj == null)
        {
            if(kThrowScope.gameObject.activeSelf)
                kThrowScope.gameObject.SetActive(false);
        }
        else
        {
            if (!kThrowScope.gameObject.activeSelf)
                kThrowScope.gameObject.SetActive(true);

            kThrowScope.transform.position = Utility.Get3DPos(Camera.main, screenPos, Vector3.Distance(Camera.main.transform.position, _targetThrowObj.transform.position));
        }
    }

    private Vector3 GetScopePosition()
    {
        float ScreenX = Screen.width;
        float ScreenY = Screen.height;
        float ScreenResolution = ScreenX / ScreenY;
        
        float UIWidth = Mathf.Floor(UIHeight * ScreenResolution);  //NGUI의 가로 픽셀. 
        Vector2 ScrRes = new Vector2(UIWidth / ScreenX, UIHeight / ScreenY);
        //targetposition을 게임카메라의 viewPort 좌표로 변경. 

        if(_targetThrowObj != null)
        {
            Vector3 screenPos = Camera.main.WorldToViewportPoint(_targetThrowObj.transform.position);

            //해당 좌표를 uiCamera의 World좌표로 변경. 
            transform.position = uiCamera.ViewportToWorldPoint(screenPos);
            //값 정리. 
            screenPos = transform.localPosition;
            screenPos.y = Mathf.RoundToInt(screenPos.y);
            screenPos.x = _posX;
            screenPos.z = 0.0f;

            if (screenPos.y >= UIHeight * 0.5f)
                screenPos.y = UIHeight * 0.5f;
            if (screenPos.y <= -UIHeight * 0.5f)
                screenPos.y = -UIHeight * 0.5f;


            return screenPos;
        }
        else
        {
            return Vector3.Lerp(this.transform.localPosition, _originLocalPos, Time.deltaTime * 10f);
        }
        
    }

    //public void GetPlayerIKObject()
    //{
    //    Player player = World.Instance.Player;

    //    if(kScoprDir == eScopeDir.Left)
    //    {
    //        //_ikObj = player.GetIKObject(Unit.eBipedIK.LeftHand);
    //    }
    //    else
    //    {
    //        //_ikObj = player.GetIKObject(Unit.eBipedIK.RightHand);
    //    }
    //}

    public void SetScopeScale(float scaleValue)
    {
        Vector3 scopeScale = new Vector3(scaleValue, scaleValue, scaleValue);

        kThrowScope.transform.localScale = scopeScale;

        scaleValue += 0.1f;
        scopeScale = new Vector3(scaleValue, scaleValue, scaleValue);

        kThrowOnObj.transform.localScale = scopeScale;
        kThrowOffObj.transform.localScale = scopeScale;
    }
}
