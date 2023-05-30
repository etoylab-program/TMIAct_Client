using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eScopeDir
{
    None,
    Left,
    Right,
}

public enum eThrowType
{
    Throw = 0,
    Kick = 1,
    ThrowIdle = 2,
    KickIdle = 3,
}

public enum eThrowSoundType
{
    Balloon,
    Balloon2,
    Ball,
    Ball2,
}

public class ThrowingManager : MonoSingleton<ThrowingManager>
{
    public GameObject kThrowBeziersRoot;
    public Transform kLeftThrowerPos;
    public Transform kRightThrowerPos;

    private Player _player;
    private FigureUnit _leftThrower;
    private FigureUnit _rightThrower;

    private float _throwShotAniTime;
    private Coroutine _idleCoroutine;

    private Dictionary<string, List<ThrowObj>> _dicThrowObjs;
    private Dictionary<string, ThrowBezier> _dicThrowBeziers;
    public List<string> _throwBeziersKeys;

    private float _fTimer = 0f;

    private UIShootingModePanel _uiShootingModePanel;

    private bool bisInit = false;
    private bool bisGameStarted = false;

    private const int _throwObjPoolCount = 10;

    private WorldThrowShooting _worldThrowShooting;

    private float _worldTimer = 0f;
    private const float _worldTimeChecker = 0f;

    public float TimeChecker { get; private set; }
    
    private GameClientTable _gameclienttable = null;
    private List<GameClientTable.MiniGameShoot.Param> miniGameTable = new List<GameClientTable.MiniGameShoot.Param>();
    private int _currentThrowIdx = 0;

    private TimeSpan _timeSpan;

    
    
    public float kSpeed = 0.1f;
    public bool bIsCheatMode = false;

    public float _EndTimeFlag = 65f;
    public float kEndTimeDelay = 2f;

    private int _limitScore = 0;
    private const float _animIdleFlag = -1f;
    private int _comboCnt = 0;

    [Header("RotationAngle")]
    public float kLeftYAngle = 45f;
    public float kRightYangle = 135f;
    private Quaternion _leftAngle;
    private Quaternion _rightAngle;
    private Quaternion _defaultAngle;
    private Coroutine _resetRotationFunc;

    private void Awake() { Init(); }
    private void Start() { Init(); }

    private void Init()
    {
        if (bisInit)
            return;

        bisInit = true;

        Log.Show("ThrowingManager Init");

        _gameclienttable = (GameClientTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/GameClient.asset");
        miniGameTable = _gameclienttable.MiniGameShoots;

        _leftThrower = GameSupport.CreateFigure("Figure/Hebiko", "Unit/Figure/Hebiko_F/hebiko_H");
        _rightThrower = GameSupport.CreateFigure("Figure/Shikanosuke", "Unit/Figure/Shikanosuke_F/shikanosuke_H");

        

        if (_leftThrower != null)
        {
            _leftThrower.transform.SetPositionAndRotation(kLeftThrowerPos.transform.position, kLeftThrowerPos.transform.rotation);
            //_leftThrower.transform.SetChildLayer(0);
            Utility.SetLayer(_leftThrower.gameObject, (int)eLayer.Default, true); // 레이어 변경은 이 함수만 쓰셈
            _leftThrower.PlayAniImmediate(eAnimation.ThrowIdle);
        }
        if (_rightThrower != null)
        {
            _rightThrower.transform.SetPositionAndRotation(kRightThrowerPos.transform.position, kRightThrowerPos.transform.rotation);
            //_rightThrower.transform.SetChildLayer(0);
            Utility.SetLayer(_rightThrower.gameObject, (int)eLayer.Default, true); // 레이어 변경은 이 함수만 쓰셈
            _rightThrower.PlayAniImmediate(eAnimation.ThrowIdle);
        }

        this.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);


        _dicThrowObjs = new Dictionary<string, List<ThrowObj>>();
        for(int i = 0; i < miniGameTable.Count; i++)
        {
            if (string.IsNullOrEmpty(miniGameTable[i].ModelPb))
                continue;
            if(!_dicThrowObjs.ContainsKey(miniGameTable[i].ModelPb))
            {
                List<ThrowObj> listTempObjs = new List<ThrowObj>();
                GameObject parentObj = new GameObject(miniGameTable[i].ModelPb);
                parentObj.transform.parent = this.gameObject.transform;
                parentObj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                parentObj.transform.localScale = Vector3.one;
                for (int j = 0; j < _throwObjPoolCount; j++)
                {
                    GameObject tempObj = GameObject.Instantiate(ResourceMgr.Instance.LoadFromAssetBundle("background", "BackGround/event_obj/" + miniGameTable[i].ModelPb + ".prefab") as GameObject);
                    tempObj.tag = "Enemy";
                    tempObj.name = miniGameTable[i].ModelPb + "_" + j.ToString();
                    tempObj.transform.parent = parentObj.transform;
                    tempObj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                    tempObj.transform.localScale = Vector3.one;// new Vector3(0.3f, 0.3f, 0.3f);

                    tempObj.SetActive(false);

                    ThrowObj tempThrow = tempObj.GetComponent<ThrowObj>();
                    listTempObjs.Add(tempThrow);
                }

                _dicThrowObjs.Add(miniGameTable[i].ModelPb, listTempObjs);
            }
        }

        _dicThrowBeziers = new Dictionary<string, ThrowBezier>();
        _throwBeziersKeys = new List<string>();

        int beziersCount = kThrowBeziersRoot.transform.childCount;
        for(int i = 0; i < beziersCount; i++)
        {
            GameObject tempBeziers = kThrowBeziersRoot.transform.GetChild(i).gameObject;

            if (tempBeziers == null || tempBeziers.GetComponent<ThrowBezier>() == null)
                continue;

            _dicThrowBeziers.Add(tempBeziers.name, tempBeziers.GetComponent<ThrowBezier>());
            _throwBeziersKeys.Add(tempBeziers.name);
        }

        _fTimer = 0f;
        _worldTimer = 0f;
        //Test

        _currentThrowIdx = 0;

        _defaultAngle = Quaternion.Euler(0f, 90f, 0f);
        _leftAngle = Quaternion.Euler(0f, kLeftYAngle, 0f);
        _rightAngle = Quaternion.Euler(0f, kRightYangle, 0f);

        bisGameStarted = false;

        _worldThrowShooting = World.Instance as WorldThrowShooting;

        SetSound();
    }

    public void ThrowMiniGameStart(Player player)
    {
        TimeChecker = 0f;
        _comboCnt = 0;
        _worldTimer = _EndTimeFlag;
        _limitScore = _worldThrowShooting.StageData.ConditionValue;

        _player = player;
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
            return;
        }

        _leftThrower.LookAtTarget(World.Instance.Player.transform.position);
        _rightThrower.LookAtTarget(World.Instance.Player.transform.position);


        _uiShootingModePanel = GameUIManager.Instance.GetUI<UIShootingModePanel>("ShootingModePanel");

        _timeSpan = TimeSpan.FromSeconds(_worldTimer);
        _uiShootingModePanel.UpdateTime(_timeSpan);

        Invoke("GameStart", 2f);
    }

    private void GameStart()
    {
        bisGameStarted = true;
        _uiShootingModePanel.ShootingGameStart();
    }

    private void Update()
    {
        if (!bisGameStarted)
            return;
//#if UNITY_EDITOR
//        if (Input.GetKeyDown(KeyCode.A))
//        {
//            ShotThrowObj(miniGameTable[0]);
//            return;
//        }
//        if(Input.GetKeyDown(KeyCode.S))
//        {
//            _worldTimer = 0f;
//        }
//        if(Input.GetKeyDown(KeyCode.Q))
//        {
//            ShootingSuccessCheck();
//        }
//#endif
        _timeSpan = TimeSpan.FromSeconds(_worldTimer);
        _uiShootingModePanel.UpdateTime(_timeSpan);

        
        if(_worldTimer <= _worldTimeChecker)
        {
            if(!GetActiveThrowObj())
            {
                _worldTimer = _worldTimeChecker;
                bisGameStarted = false;
                StartCoroutine(GameEndTimeDelay());
            }
            return;
        }

        _worldTimer -= Time.deltaTime * Time.timeScale;


        TimeChecker += Time.deltaTime * Time.timeScale;

        if (_currentThrowIdx >= miniGameTable.Count)
            return;
        if (miniGameTable[_currentThrowIdx].Time <= TimeChecker)
        {
            ShotThrowObj(miniGameTable[_currentThrowIdx]);
            _currentThrowIdx++;
        }
    }

    IEnumerator GameEndTimeDelay()
    {
        yield return new WaitForSeconds(kEndTimeDelay);

        ShootingSuccessCheck();
    }

    public void PlayShooting(eScopeDir dir)
    {
        if (_resetRotationFunc != null)
            StopCoroutine(_resetRotationFunc);

        switch(dir)
        {
            case eScopeDir.Left:
                {
                    //_player.LookAtTarget(_leftThrower.transform.position);
                    SetPlayerRotation(_leftAngle);
                }
                break;
            case eScopeDir.Right:
                {
                    //_player.LookAtTarget(_rightThrower.transform.position);
                    SetPlayerRotation(_rightAngle);
                }
                break;
        }
        World.Instance.Player.StopAllCoroutines();
        _throwShotAniTime = World.Instance.Player.PlayAniImmediateToBackAniWithAlpha(eAnimation.ThrowShot, 0f, 1f, true, eAnimation.ThrowIdle);

        _resetRotationFunc = StartCoroutine(PlayerRotationReset(_throwShotAniTime));
    }

    IEnumerator PlayerRotationReset(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetPlayerRotation(_defaultAngle);
    }

    private void SetPlayerRotation(Quaternion rot)
    {
        _player.transform.rotation = rot;
    }

    private void ShotThrowObj(GameClientTable.MiniGameShoot.Param targetObj)
    {
        if (_dicThrowObjs == null || _dicThrowObjs.Count <= 0)
            return;

        if(targetObj.MoveSpeed.Equals(_animIdleFlag))
        {
            for (int i = 0; i < _dicThrowObjs[targetObj.ModelPb].Count; i++)
            {
                if (_dicThrowObjs[targetObj.ModelPb][i].bisThrowing)
                    continue;

                _dicThrowObjs[targetObj.ModelPb][i].ShowThrow( _dicThrowBeziers[targetObj.TagetPattern] );
                _dicThrowObjs[targetObj.ModelPb][i].gameObject.SetActive(true);
                break;
            }

            if (targetObj.TagetPattern.Equals("T_1") || targetObj.TagetPattern.Equals("T_2") || targetObj.TagetPattern.Equals("T_3") ||
                targetObj.TagetPattern.Equals("T_7") || targetObj.TagetPattern.Equals("T_8") || targetObj.TagetPattern.Equals("T_9"))
            {
                _leftThrower.StopAllCoroutines();
                if (targetObj.ThrowAniType == (int)eThrowType.ThrowIdle)
                {
                    _leftThrower.PlayAniImmediateToBackAni(eAnimation.ThrowIdle);
                    return;
                }
                else if(targetObj.ThrowAniType == (int)eThrowType.KickIdle)
                {
                    _leftThrower.PlayAniImmediateToBackAni(eAnimation.KickIdle);
                    return;
                }
            }
            else
            {
                _rightThrower.StopAllCoroutines();
                if (targetObj.ThrowAniType == (int)eThrowType.ThrowIdle)
                {
                    _rightThrower.PlayAniImmediateToBackAni(eAnimation.ThrowIdle);
                    return;
                }
                else if (targetObj.ThrowAniType == (int)eThrowType.KickIdle)
                {
                    _rightThrower.PlayAniImmediateToBackAni(eAnimation.KickIdle);
                    return;
                }
            }
                
        }

        ThrowObj throwTarget = null;

        for (int i = 0; i < _dicThrowObjs[targetObj.ModelPb].Count; i++)
        {
            if (!_dicThrowObjs[targetObj.ModelPb][i].bisThrowing && _dicThrowObjs[targetObj.ModelPb][i].gameObject.activeSelf)
            {
                throwTarget = _dicThrowObjs[targetObj.ModelPb][i];
                break;
            }
        }

        if( throwTarget != null)
        {
            throwTarget.ShotThrow(_dicThrowBeziers[targetObj.TagetPattern], targetObj, kSpeed);
        }
        else
        {
            for (int i = 0; i < _dicThrowObjs[targetObj.ModelPb].Count; i++)
            {
                if (_dicThrowObjs[targetObj.ModelPb][i].bisThrowing)
                    continue;


                _dicThrowObjs[targetObj.ModelPb][i].ShotThrow(
                    _dicThrowBeziers[targetObj.TagetPattern],
                    targetObj, kSpeed
                    );

                _dicThrowObjs[targetObj.ModelPb][i].gameObject.SetActive(true);
                break;
            }
        }

        if (targetObj.TagetPattern.Equals("T_1") || targetObj.TagetPattern.Equals("T_2") || targetObj.TagetPattern.Equals("T_3") ||
                targetObj.TagetPattern.Equals("T_7") || targetObj.TagetPattern.Equals("T_8") || targetObj.TagetPattern.Equals("T_9"))
        {
            if (targetObj.ThrowAniType == (int)eThrowType.Throw)
                _leftThrower.PlayAniImmediateToBackAni(eAnimation.ThrowShot, 0f, true, eAnimation.ThrowIdle);
            else
                _leftThrower.PlayAniImmediateToBackAni(eAnimation.KickShot, 0f, true, eAnimation.Idle02);
        }
        else
        {
            if (targetObj.ThrowAniType == (int)eThrowType.Throw)
                _rightThrower.PlayAniImmediateToBackAni(eAnimation.ThrowShot, 0f, true, eAnimation.ThrowIdle);
            else
                _rightThrower.PlayAniImmediateToBackAni(eAnimation.KickShot, 0f, true, eAnimation.Idle02);
        }
    }

    private bool GetActiveThrowObj()
    {
        bool result = false;

        foreach (KeyValuePair<string, List<ThrowObj>> throwObjlist in _dicThrowObjs)
        {
            for (int i = 0; i < throwObjlist.Value.Count; i++)
            {
                if (throwObjlist.Value[i].gameObject.activeSelf)
                {
                    result = true;
                    return result;
                }
            }
        }

        return result;
    }

    public ThrowObj GetThrowObj(Vector3 screenPos, eScopeDir scopeDir)
    {
        float ScreenX = Screen.width;
        float ScreenY = Screen.height;
        float ScreenResolution = ScreenX / ScreenY;

        //float UIWidth = Mathf.Floor(UIHeight * ScreenResolution);  //NGUI의 가로 픽셀. 

        ThrowObj throwObj = null;
        float distance = float.MaxValue;


        Vector3 targetScreenPos = Vector3.zero;
        foreach(KeyValuePair<string, List<ThrowObj>> throwObjlist in _dicThrowObjs)
        {
            for(int i = 0; i < throwObjlist.Value.Count; i++)
            {
                if (!throwObjlist.Value[i].bisThrowing || !throwObjlist.Value[i].gameObject.activeSelf)
                    continue;

                targetScreenPos = Camera.main.WorldToScreenPoint(throwObjlist.Value[i].transform.position);

                //float dis = targetScreenPos.x;

                if (scopeDir == eScopeDir.Left)
                {
                    if (throwObjlist.Value[i].bIsLeftTrigger)
                        continue;
                    if (targetScreenPos.x > ScreenX * 0.5f)
                        continue;

                    if(throwObj == null)
                    {
                        throwObj = throwObjlist.Value[i];
                    }
                    else
                    {
                        if (Camera.main.WorldToScreenPoint(throwObj.transform.position).x < targetScreenPos.x)
                            throwObj = throwObjlist.Value[i];
                    }
                }
                else
                {
                    if (throwObjlist.Value[i].bIsRightTrigger)
                        continue;
                    if (targetScreenPos.x < ScreenX * 0.5f)
                        continue;

                    if (throwObj == null)
                    {
                        throwObj = throwObjlist.Value[i];
                    }
                    else
                    {
                        if (Camera.main.WorldToScreenPoint(throwObj.transform.position).x > targetScreenPos.x)
                            throwObj = throwObjlist.Value[i];
                    }
                }

                //Vector3 targetPos = throwObjlist.Value[i].transform.position;
                //targetPos.x = 0f;

                //float dis = Vector3.Distance(throwObjlist.Value[i].transform.position, Utility.Get3DPos(Camera.main, screenPos, Vector3.Distance(Camera.main.transform.position, throwObjlist.Value[i].transform.position)));
                ////float dis = Vector3.Distance(throwObjlist.Value[i].transform.position, screenPos);
                //if (dis < distance)
                //{
                //    distance = dis;
                //    throwObj = throwObjlist.Value[i];
                //}
            }
        }

        return throwObj;
    }

    public ThrowObj GetThrowObj(string throwName)
    {
        foreach (KeyValuePair<string, List<ThrowObj>> throwObjlist in _dicThrowObjs)
        {
            for (int i = 0; i < throwObjlist.Value.Count; i++)
            {
                if (throwObjlist.Value[i].name.Equals(throwName))
                    return throwObjlist.Value[i];
            }
        }

        return null;
    }

    public void AddScore(Vector3 pos, int score)
    {
        _comboCnt++;
        _uiShootingModePanel.AddScore(pos, score, _comboCnt);
    }

    public int GetScore()
    {
        return _uiShootingModePanel._currentScore;
    }
    
    public void DamageHp()
    {
        _comboCnt = 0;
#if UNITY_EDITOR
        if (bIsCheatMode)
            return;
#endif
        TimeChecker = -1f;
        _currentThrowIdx = 0;
        _uiShootingModePanel.DamageHp();
    }

    private void ShootingSuccessCheck()
    {
        _worldThrowShooting.Player.StopAllCoroutines();
        _worldThrowShooting.Player.aniEvent.SetShaderAlpha("_Color", 1f);
        bisGameStarted = false;
        HideThrowObjs();

        if (GetScore() >= _limitScore)
        {
            _worldThrowShooting.OnShootingGameSuccess();
        }
        else
        {
            _worldThrowShooting.OnShootingGameFailed();
        }
    }

    private void HideThrowObjs()
    {
        foreach (KeyValuePair<string, List<ThrowObj>> throwObjlist in _dicThrowObjs)
        {
            for (int i = 0; i < throwObjlist.Value.Count; i++)
            {
                throwObjlist.Value[i].gameObject.SetActive(false); ;
            }
        }
    }

    private void SetSound()
    {

        GameClientTable.MiniGameShoot.Param balloonSnd = miniGameTable.Find(x => x.EffectType == (int)eThrowSoundType.Balloon);
        if(balloonSnd != null)
        {
            MiniGameAddSound(eThrowSoundType.Balloon.ToString(), "Sound/" + balloonSnd.Hitsnd, 1f, 5);
            MiniGameAddSound(eThrowSoundType.Balloon.ToString() + "_Failed", "Sound/" + balloonSnd.Hitsnd, 1f, 3);
        }

        GameClientTable.MiniGameShoot.Param balloonSnd2 = miniGameTable.Find(x => x.EffectType == (int)eThrowSoundType.Balloon2);
        if (balloonSnd != null)
        {
            MiniGameAddSound(eThrowSoundType.Balloon2.ToString(), "Sound/" + balloonSnd2.Hitsnd, 1f, 5);
            MiniGameAddSound(eThrowSoundType.Balloon2.ToString() + "_Failed", "Sound/" + balloonSnd2.Hitsnd, 1f, 3);
        }

        GameClientTable.MiniGameShoot.Param ballSnd = miniGameTable.Find(x => x.EffectType == (int)eThrowSoundType.Ball);
        if(ballSnd != null)
        {
            MiniGameAddSound(eThrowSoundType.Ball.ToString(), "Sound/" + ballSnd.Hitsnd, 1f, 5);
            MiniGameAddSound(eThrowSoundType.Ball.ToString() + "_Failed", "Sound/" + ballSnd.Hitsnd, 1f, 3);
        }

        GameClientTable.MiniGameShoot.Param ballSnd2 = miniGameTable.Find(x => x.EffectType == (int)eThrowSoundType.Ball2);
        if(ballSnd2 != null)
        {
            MiniGameAddSound(eThrowSoundType.Ball2.ToString(), "Sound/" + ballSnd2.Hitsnd, 1f, 5);
            MiniGameAddSound(eThrowSoundType.Ball2.ToString() + "_Failed", "Sound/" + ballSnd2.Hitsnd, 1f, 3);
        }
    }

    private void MiniGameAddSound(string name, string path, float volume, int soundCnt)
    {
        for (int i = 0; i < soundCnt; i++)
            SoundManager.Instance.AddAudioClip(name, path, volume);
    }
}
