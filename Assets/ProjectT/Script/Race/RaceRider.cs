using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eRaceSound
{
    effect_boost,
    effect_magnet,
    get_boost,
    get_coin,
    get_magnet,
    get_hp,
    jump,
    landing,
    move,
    move_line,
    move_speed,
    obj_crash,
    obj_destruction,
    _MAX_,
}

public class RaceRider : MonoBehaviour
{
    private bool bisInit = false;

    public bool kCheatMode = false;
    public bool kDebugMode = false;
    private UIBikeRaceModePanel _raceUI;

    /* v2로 재작업
    public UnityEngine.PostProcessing.PostProcessingBehaviour kPostProcessingBehaviour;
    private UnityEngine.PostProcessing.PostProcessingProfile _profile;
    private UnityEngine.PostProcessing.MotionBlurModel.Settings _motionblurSettings = new UnityEngine.PostProcessing.MotionBlurModel.Settings();*/

    [Header("Target")]
    public GameObject kTargetObj;
    public Transform kPlayerPos;
    public Animator kTargetAnimator;
    private Animator _playerAnimator;
    public Player kPlayer;

    [Header("Start Acceleration")]
    public float kFirstMaxSpeed = 50f;
    public float kFirstAccelerationTime = 2f;

    [Header("Acceleration Effect")]
    public Camera kCamera;
    public UnityStandardAssets.ImageEffects.RadialBlur kRadialBlur;
    private float _fieldValue = 0f;
    private float _originFieldOfViewValue;

    public float kHorizontalMoveTime = 0.3f;
    public float kMulHorizontalMoveTime = 1f;
    public float kSpeed = 0;
    public float kMaxSpeed = 200;

    private Vector3 _moveZ = Vector3.zero;
    private Vector3 _moveX = Vector3.zero;
    private Vector3 _zeroVec = Vector3.zero;
    private Rigidbody _rigidbody;
    private int _posState = 0;

    private bool _bisLRMove = false;
    private float _horizontalMoveDelta = 6f;
    private float _horizontalMoveTimeDelta = 0f;
    private Vector3 _horizontalVec;
    private Vector3 _horizontalDeltaVec;


    public Transform kFrontWheel;
    public Transform kBackWheel;

    [Header("Boost Values")]
    private bool _bIsBoost = false;
    public float kBoostSpeed = 150f;
    public float kBoostDuration = 5f;
    public float kBoostAccelerationTime = 1f;

    [Header("Magnet Values")]
    private bool _bisMagnet = false;
    public bool bIsMagnet { get { return _bisMagnet; } }
    public float kMagnetDistance = 12f;
    public float kMagnetDuration = 5f;

    private bool _bisDamageBlink = false;
    public float kDamageBlinkDuration = 2f;
    private int _damageBlinkCnt = 0;

    WaitForFixedUpdate _waitForFixedUpdate;

    [Header("Ground Check Values")]
    public bool kIsGround = false;
    [SerializeField] float _groundCheckHeight = 0.5f;
    [SerializeField] float _groundCheckRadius = 0.5f;
    [SerializeField] float _groundCheckDistance = 0.3f;
    [SerializeField] LayerMask _groundMask;
    private bool _previouslyGrounded;
    private RaycastHit _currentGroundInfo;

    private float _originSpeed = 0f;

    public AnimationCurve kMoveCurve;
    private eAnimation _animState = eAnimation.None;

    private GameObject _boostEffect;
    private GameObject _magnetEffect;
    private GameObject _runEffect;
    private GameObject _jumpEndEffect;

    private float _inputDelay = 0f;

    private Renderer[] _renders;

    [Range(0f, 1f)]
    public float kMinAnimSpeed = 0.7f;

    private float _animSpeed = 1f;

    IEnumerator _lrMoveCoroutine = null;

    private bool _jumpingObj = false;

	private WaitForFixedUpdate mWaitForFixedUpdate = new WaitForFixedUpdate();


	void Start()
    {
        if (_renders == null || _renders.Length <= 0)
            _renders = this.GetComponentsInChildren<Renderer>();

        _waitForFixedUpdate = new WaitForFixedUpdate();
        _rigidbody = this.GetComponent<Rigidbody>();

        _posState = 0;
        _damageBlinkCnt = (int)(kDamageBlinkDuration / 0.2f);

        _originFieldOfViewValue = kCamera.fieldOfView;
        kTargetAnimator.applyRootMotion = true;
        bisInit = true;

        _boostEffect = kTargetObj.transform.Find("Booster_Effect").gameObject;
        _magnetEffect = kTargetObj.transform.Find("Magnet_Effect").gameObject;
        _runEffect = kTargetObj.transform.Find("Run_Effect").gameObject;
        _jumpEndEffect = kTargetObj.transform.Find("Landing_Effect").gameObject;

        /* v2로 재작업
        if (kPostProcessingBehaviour != null)
        {
            if (kPostProcessingBehaviour.profile != null)
                _profile = kPostProcessingBehaviour.profile;
        }*/

        _boostEffect.SetActive(false);
        _magnetEffect.SetActive(false);

        SetSounds();

        //Set Ground Check Values
        //_groundCheckHeight = 0.3f;
        //_groundCheckRadius = 0.5f;
        //_groundCheckDistance = 1.5f;

        //_groundMask = 1 << 14; //Floor
    }

    public void OnTriggerEnter(Collider other)
    {
#if UNITY_EDITOR
        if (kCheatMode)
            return;
#endif
        if (other.tag.Equals("Wall"))
        {
            PlayRaceFXSound(eRaceSound.obj_crash);
            PlayRaceFXSound(eRaceSound.obj_destruction);
            if (!_bIsBoost)
            {
                if(!_bisDamageBlink)
                {
                    _bisDamageBlink = true;
                    kSpeed = 20f;
                    if (_raceUI.DamageRiderHP())
                    {
                        Invoke("StopDamageBlink", kDamageBlinkDuration);

                        StopCoroutine("WallTriggerBlink");
                        StartCoroutine(WallTriggerBlink(_damageBlinkCnt));
                    }
                }
            }

            if (other.transform.parent.name.Contains("barricade"))
                other.transform.parent.gameObject.SetActive(false);
            else
                other.gameObject.SetActive(false);

            EffectManager.Instance.Play(other.transform.position, 1, EffectManager.eType.Each_Monster_Normal_Hit);
        }
        else if (other.name.Contains("coin"))     //코인 아이템 획득
        {
            _raceUI.AddCoin();
            other.gameObject.SetActive(false);
        }
        else if(other.name.Contains("magnet"))        //자석
        {
            SetMagnet();
        }
        else if (other.name.Contains("booster"))       //부스터
        {
            SetBoost();
        }
        else if (other.name.Contains("hprecovery"))       //회복
        {
            _raceUI.AddRiderHP();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("jump"))
        {
            if(!_jumpingObj)
            {
                _jumpingObj = true;
                _originSpeed = kSpeed;
                kSpeed = kMaxSpeed;
                Log.Show("OnCollisionEnter JUMP : " + _jumpingObj, Log.ColorType.Blue);
            }
            else
                Log.Show("OnCollisionEnter JUMP : " + _jumpingObj, Log.ColorType.Red);
        }
    }

    void StopDamageBlink()
    {
        _bisDamageBlink = false;
    }

    public void StartRaceMode(float _delay, UIBikeRaceModePanel.RaceModeDelegate _del)
    {
        _raceUI = GameUIManager.Instance.GetUI("BikeRaceModePanel") as UIBikeRaceModePanel;
        kPlayer = World.Instance.Player;

        

        _playerAnimator = kPlayer.GetComponentInChildren<Animator>();

        DynamicShadowProjector.DrawTargetObject drawTargetObject = kPlayer.GetComponentInChildren<DynamicShadowProjector.DrawTargetObject>();
        if (drawTargetObject != null)
            drawTargetObject.target = this.transform;

        TargetPlayAniImmediate(eAnimation.Race_Idle);

        if (_del != null)
            _del();

        if (kPlayer == null)
            kPlayer = this.GetComponentInChildren<Player>();

        StartCoroutine(StartAcceleration());
    }
    
    IEnumerator StartRace(float _delay, UIBikeRaceModePanel.RaceModeDelegate _del)
    {
        yield return new WaitForSeconds(_delay);
        if (_del != null)
            _del();

        if (kPlayer == null)
            kPlayer = this.GetComponentInChildren<Player>();

        StartCoroutine(StartAcceleration());
    }

    public void EndGame()
    {
        StopAllCoroutines();
    }

    void FixedUpdate()
    {
        if (!bisInit)
            return;

        if (World.Instance.IsEndGame || World.Instance.IsPause)
            return;

        /* v2로 재작업
        //모션블러 값조정(속도에 따라 바뀜)
        if(_profile != null)
        {
            _motionblurSettings.sampleCount = (int)(kSpeed * 0.1f);
            _motionblurSettings.shutterAngle = kSpeed;
            _motionblurSettings.frameBlending = kSpeed / kBoostSpeed;
            _profile.motionBlur.settings = _motionblurSettings;
        }
        */
        if (kSpeed * 0.01f < 0.5f)
            SoundManager.Instance.PlayFxLoopSnd(eRaceSound.move.ToString(), 0.5f);
        else
            SoundManager.Instance.PlayFxLoopSnd(eRaceSound.move.ToString(), kSpeed * 0.01f);

        kBackWheel.Rotate(Vector3.right, kSpeed);// * Time.deltaTime);
        kFrontWheel.Rotate(Vector3.right, kSpeed);// * Time.deltaTime);

        GroundCheck();

        if(kRadialBlur != null)
        {
            _fieldValue = Mathf.Clamp(kSpeed / kMaxSpeed, 0, 2);
            kRadialBlur.fSampleDist = _fieldValue;
        }
#if UNITY_EDITOR
        //JumpTest
        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    if (bIsGround)
        //    {
        //        _originSpeed = kSpeed;
        //        _rigidbody.AddForce(0, 30, 0, ForceMode.Impulse);

        //    }
        //}

        if (Input.GetKey(KeyCode.Return))
        {
            float fSpeed = 2f * Time.fixedDeltaTime * 5f;
            kSpeed += fSpeed;
            //speedLb.text = kSpeed.ToString("N2");
            _moveZ.z = kSpeed;
            //moveZ.z *= -1f;
            _rigidbody.MovePosition(this.transform.position + _moveZ);
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            SetMagnet();
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            SetBoost();
        }
#endif
    }

    IEnumerator WallTriggerBlink(int num)
    {
        int cnt = 1;

        WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
        while (cnt <= num)
        {
            SetRenderersEnable(false);
            yield return waitForSeconds;
            SetRenderersEnable(true);
            yield return waitForSeconds;
            cnt++;
        }
        SetRenderersEnable(true);
    }

    void SetRenderersEnable(bool enable)
    {
        if (_renders == null || _renders.Length <= 0)
            return;

        foreach (Renderer renderer in _renders)
            renderer.enabled = enable;

        kPlayer.aniEvent.ShowMesh(enable);
    }

    public void BikeMoveHorizontal(int dir)
    {
        //좌우 이동 입력이 너무 빠르면 무시
        if (_inputDelay.Equals(Time.time) || (Time.time - _inputDelay) <= 0.1f)
            return;

        _inputDelay = Time.time;

        _posState += dir;

        if(_posState < -1)
        {
            _posState = -1;
            return;
        }

        if(_posState > 1)
        {
            _posState = 1;
            return;
        }
        
        _bisLRMove = true;


        //코루틴을 강제로 종료 시키기 위한 작업
        if (_lrMoveCoroutine != null)
            StopCoroutine(_lrMoveCoroutine);
        //StopCoroutine("LRMove");
        _lrMoveCoroutine = LRMove((float)dir);
        StartCoroutine(_lrMoveCoroutine);
    }
    
    IEnumerator LRMove(float moveDir)
    {
        //속도에 따른 좌우이동 사운드 재생
        if(kSpeed * 0.01f < 0.5f)
            PlayRaceFXSound(eRaceSound.move_line, 0.5f);
        else
            PlayRaceFXSound(eRaceSound.move_line, kSpeed * 0.01f);
        float frameRate = Time.fixedDeltaTime * kHorizontalMoveTime;
        
        _horizontalVec = this.transform.position;
        float fhoriTime = 0f;

        _animSpeed = kSpeed * 0.02f;
        if (_animSpeed <= kMinAnimSpeed)
            _animSpeed = kMinAnimSpeed;

        if (!_bIsBoost)
        {
            if (moveDir >= 1f)
            {
                kHorizontalMoveTime = TargetPlayAnim(eAnimation.Race_RightMove);
                _horizontalDeltaVec.x += _horizontalMoveDelta;
               
            }
            else
            {
                kHorizontalMoveTime = TargetPlayAnim(eAnimation.Race_LeftMove);
                _horizontalDeltaVec.x -= _horizontalMoveDelta;
                
            }
        }
        else
        {
            if (moveDir >= 1f)
            {
                kHorizontalMoveTime = TargetPlayAnim(eAnimation.Race_BoostRightMove);
                _horizontalDeltaVec.x += _horizontalMoveDelta;
            }
            else
            {
                kHorizontalMoveTime = TargetPlayAnim(eAnimation.Race_BoostLeftMove);
                _horizontalDeltaVec.x -= _horizontalMoveDelta;

            }
        }

        kHorizontalMoveTime /= _animSpeed;
        fhoriTime = kHorizontalMoveTime;
        
        //시간 초기화
        _horizontalMoveTimeDelta = 0f;

        //좌우 이동전 x값 저장
        float tempX = _rigidbody.position.x;
        
        //좌우 이동전 위치를 저장
        _horizontalVec = _rigidbody.position;

        //커브 애니메이션 체크 값
        float animValue = 0;

        bool bendAnim = false;
        float cutTime = kPlayer.aniEvent.GetCutFrameLength(_animState);
        //시간, 커브애니메이션의 값을 토대로 좌우 이동

        float animTime = 0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (_bisLRMove)
        {
            _horizontalMoveTimeDelta += Time.fixedDeltaTime * _animSpeed;
            animTime += Time.fixedDeltaTime;
            _horizontalVec = _rigidbody.position;
            animValue = (kMoveCurve.Evaluate(_horizontalMoveTimeDelta / kHorizontalMoveTime));
            _horizontalVec.x = tempX + animValue * ((_posState * _horizontalMoveDelta) - tempX);
            _rigidbody.MovePosition(_horizontalVec);

            if (animTime >= cutTime)
            {
                if (!bendAnim)
                {
                    bendAnim = true;
                    if (_animSpeed < 1f)
                    {
                        TargetPlayAnim(eAnimation.Race_Idle);
                    }
                }
            }

            if (_horizontalMoveTimeDelta >= kHorizontalMoveTime)// * kMulHorizontalMoveTime)
            {
                _bisLRMove = false;
                _horizontalMoveTimeDelta = 0f;
                break;

            }

            yield return mWaitForFixedUpdate;
        }

        _bisLRMove = false;
    }

    //게임 시작 후 지정된 속도까지 강제로 가속
    IEnumerator StartAcceleration()
    {
        if (_rigidbody == null)
            _rigidbody = this.GetComponent<Rigidbody>();
        float EndTime = Time.fixedTime + kFirstAccelerationTime;
        float clampVal = 0f;
        while(this.gameObject.activeSelf)
        {
            clampVal = 1f - Mathf.Clamp((EndTime - Time.fixedTime) / kFirstAccelerationTime, 0, 1);
            kSpeed = 20f + (clampVal * kFirstMaxSpeed);
            _moveZ.z = kSpeed * Time.fixedDeltaTime;
            _rigidbody.MovePosition(this.transform.position + _moveZ);

            if(EndTime < Time.fixedTime)
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(BikeMove());
    }

    //바이크가 앞으로 가는 로직
    IEnumerator BikeMove()
    {
        PlayMoveAudio();
        float fTime = 0f;
        float fSpeed = (5f / 2f) * Time.fixedDeltaTime;
        while(this.gameObject.activeSelf)
        {
            if(World.Instance.IsPause)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }
            fTime += Time.fixedDeltaTime;

            kSpeed += fSpeed;
            if (!_bIsBoost)
            {
                if (kSpeed >= kMaxSpeed)
                {
                    kSpeed = kMaxSpeed;
                }
            }
            else
                kSpeed = kBoostSpeed;

            _moveZ.z = kSpeed * Time.fixedDeltaTime;
            _rigidbody.MovePosition(this.transform.position + _moveZ);
            _moveZ = _zeroVec;
            yield return new WaitForFixedUpdate();

        }

    }

   //땅에 붙어있는지 체크
    void GroundCheck()
    {
        _previouslyGrounded = kIsGround;

        Ray ray = new Ray(this.transform.position + Vector3.up * _groundCheckHeight, Vector3.down);

        if (Physics.Raycast(ray, out _currentGroundInfo, _groundCheckDistance))//, _groundMask))
        {
            if(_currentGroundInfo.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Floor")))
            {
                if (!kIsGround)
                {
                    if (!_bIsBoost)
                    {
                        TargetPlayAniImmediate(eAnimation.Race_Jump);
                        kSpeed = _originSpeed;
                    }
                    else
                    {
                        TargetPlayAniImmediate(eAnimation.Race_BoostJumpEnd);
                    }
                    _jumpingObj = false;
                    Log.Show("JumpEnd", Log.ColorType.Red);
                    _jumpEndEffect.SetActive(true);
                    Invoke("LandingEffectOff", 1f);
                    kIsGround = true;
                }
            }
            
        }
        else
        {
            if(kIsGround)
            {
                PlayRaceFXSound(eRaceSound.jump);
            }
            kIsGround = false;
        }
    }

    //점프 후 착지 이펙트 OFF
    void LandingEffectOff()
    {
        _jumpEndEffect.SetActive(false);
    }

    //부스트 시작
    public void SetBoost()
    {
        PlayRaceFXSound(eRaceSound.effect_boost);
        _originSpeed = kSpeed;
        kSpeed = kBoostSpeed;
        _boostEffect.SetActive(true);
        _raceUI.kBoostGauge.SetBuffGaugeUnit(kBoostDuration);
        if (_bIsBoost)               //부스트 중에 무스트를 또 먹었을때 애니메이션 변경없이 간다.
        {
            _bIsBoost = true;
            CancelInvoke("StopBoost");
            CancelInvoke("SetBoostAnim");
            Invoke("StopBoost", kBoostDuration);
            TargetPlayAniImmediate(eAnimation.Race_Boosting);
            Invoke("SetBoostAnim", kBoostDuration);
        }
        else
        {
            _bIsBoost = true;
            CancelInvoke("StopBoost");
            CancelInvoke("SetBoostAnim");
            Invoke("StopBoost", kBoostDuration);
            float booststartdelay = TargetPlayAniImmediate(eAnimation.Race_BoostStart);
            Invoke("SetBoostAnim", booststartdelay);
        }
    }

    //부스트 애니 시작
    void SetBoostAnim()
    {
        if(_bIsBoost)
            TargetPlayAniImmediate(eAnimation.Race_Boosting);
        else
        {
            float delay = TargetPlayAniImmediate(eAnimation.Race_BoostEnd);
            Invoke("TargetPlayAniWithIdle", delay);
        }
            
    }

    //부스트 중지
    void StopBoost()
    {
        _raceUI.kBoostGauge.StopBuffGaugeUnit();
        kSpeed = _originSpeed;
        _bIsBoost = false;
        _boostEffect.SetActive(false);
        SetBoostAnim();
    }
    
    //자석 시작
    public void SetMagnet()
    {
        PlayRaceFXSound(eRaceSound.effect_magnet);
        _raceUI.kMagnetGauge.SetBuffGaugeUnit(kMagnetDuration);
        _magnetEffect.SetActive(true);
        _bisMagnet = true;
        CancelInvoke("StopMagnet");
        Invoke("StopMagnet", kMagnetDuration);
    }

    //자석 중지
    void StopMagnet()
    {
        _raceUI.kMagnetGauge.StopBuffGaugeUnit();
        _bisMagnet = false;
        _magnetEffect.SetActive(false);
    }


    //부스트 끝나고 돌아올 애니메이션
    float TargetPlayAniWithIdle()
    {
        return TargetPlayAniImmediate(eAnimation.Race_Idle);
    }

    //애니메이션 실행
    public float TargetPlayAniImmediate(eAnimation anim)
    {
        if (kPlayer == null)
            return 0f;

        _animState = anim;

        kTargetAnimator.speed = _animSpeed;
        kPlayer.aniEvent.SetAniSpeed(_animSpeed);
        kTargetAnimator.Play(anim.ToString(), 0, 0);

        return kPlayer.PlayAniImmediate(anim, 0);
    }


    //Crossfade애니메이션 실행
    public float TargetPlayAnim(eAnimation anim)
    {
        if (kPlayer == null)
            return 0f;
        _animState = anim;

        kTargetAnimator.speed = _animSpeed;
        kPlayer.aniEvent.SetAniSpeed(_animSpeed);

        //AniEvent CrossFade 기본값을 참고해서 값을 넣어줌
        kTargetAnimator.CrossFade(anim.ToString(), 0.15f, 0, 0);
        return kPlayer.PlayAni(anim);
    }

    public void PlayRaceFXSound(eRaceSound raceSnd)
    {
        SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, raceSnd.ToString());
    }

    public void PlayRaceFXSound(eRaceSound raceSnd, float volume)
    {
        SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, raceSnd.ToString(), volume);
    }

    void SetSounds()
    {
        for (int i = 0; i < (int)eRaceSound._MAX_; i++)
        {
            string name = ((eRaceSound)i).ToString();
            string path = "Sound/FX/RaceMode/snd_racemode_" + name + ".wav";
            float volume = 1f;
            for (int j = 0; j < 5; j++)
            {
                SoundManager.Instance.AddAudioClip(name, path, volume);
            }
        }
    }

    public void PlayMoveAudio(bool pause = false)
    {
        SoundManager.Instance.PlayFxLoopSnd(eRaceSound.move.ToString(), kSpeed * 0.01f, pause);
    }

#if UNITY_EDITOR
    /// <summary>
    /// GroundCheck Debug함수
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + Vector3.up * _groundCheckHeight, _groundCheckRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position + Vector3.up * _groundCheckHeight, Vector3.down * (_groundCheckDistance * _groundCheckRadius));
    }

    private void OnGUI()
    {
        if (!kDebugMode)
            return;

        GUI.Label(new Rect(10, 100, 500, 30), string.Format("Speed : {0}", kSpeed));
        GUI.Label(new Rect(10, 130, 500, 30), string.Format("HMoveTime : {0}", kHorizontalMoveTime));
        GUI.Label(new Rect(10, 160, 500, 30), string.Format("AnimSpeed : {0}", _animSpeed));
    }
#endif
}
