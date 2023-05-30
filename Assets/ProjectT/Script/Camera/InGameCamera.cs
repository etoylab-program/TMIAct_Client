
using AnimationOrTween;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;


public class InGameCamera : MonoBehaviour
{
    public enum EMode
    {
        DEFAULT = 0,
        SIDE,
        FOCUS,
        FIXED,
        FOLLOW_PLAYER,
        SIMPLE,
        PVP,

        _COUNT,
    }


    public interface ISetting
    {
    }

    [System.Serializable]
    public class sDefaultSetting : ISetting
    {
        public Vector3  Distance;
        public Vector2  LookAt;
        public Unit     Target;
        public bool     isTargetBoss;
        public float    T;
        public bool     LookAtPlayerBack;
        public float    LookAtPlayerBackAngle;
        public float    LookAtPlayerBackCheckTime;
        public float    LookAtPlayerBackDuration;
        public float    SmoothTimeRatio;
        public float    FOV;


        public sDefaultSetting()
        {
        }

        public sDefaultSetting(Vector3 distance, Vector3 lookAt)
        {
            Distance = distance;
            LookAt = lookAt;
        }

        public void Duplicate(ref sDefaultSetting defaultSetting)
        {
            defaultSetting.Distance = Distance;
            defaultSetting.LookAt = LookAt;
            defaultSetting.Target = Target;
            defaultSetting.isTargetBoss = isTargetBoss;
            defaultSetting.T = T;
            defaultSetting.LookAtPlayerBack = LookAtPlayerBack;
            defaultSetting.LookAtPlayerBackAngle = LookAtPlayerBackAngle;
            defaultSetting.LookAtPlayerBackCheckTime = LookAtPlayerBackCheckTime;
            defaultSetting.LookAtPlayerBackDuration = LookAtPlayerBackDuration;
            defaultSetting.SmoothTimeRatio = SmoothTimeRatio;
            defaultSetting.FOV = FOV;
        }
    }

    [System.Serializable]
    public class sSideSetting : ISetting
    {
        public Vector3          Distance;
        public Vector2          LookAt;
        public Unit.eAxisType   LockAxis;
        public float            SmoothTime;
        public float            ResumeTime;


        public sSideSetting()
        {
        }

        public sSideSetting(Vector3 distance, Vector2 lookAt, Unit.eAxisType lockAxis)
        {
            Distance = distance;
            LookAt = lookAt;
            LockAxis = lockAxis;
            SmoothTime = 0.2f;
            ResumeTime = 0.2f;
        }

        public void Save()
        {
            PlayerPrefs.SetInt("SAVE_STAGE_CAMERA_MODE_", (int)EMode.SIDE);

            PlayerPrefs.SetFloat("SAVE_STAGE_CAMERA_DISTANCE_X_", Distance.x);
            PlayerPrefs.SetFloat("SAVE_STAGE_CAMERA_DISTANCE_Y_", Distance.y);
            PlayerPrefs.SetFloat("SAVE_STAGE_CAMERA_DISTANCE_Z_", Distance.z);

            PlayerPrefs.SetFloat("SAVE_STAGE_CAMERA_LOOKAT_X_", LookAt.x);
            PlayerPrefs.SetFloat("SAVE_STAGE_CAMERA_LOOKAT_Y_", LookAt.y);

            PlayerPrefs.SetInt("SAVE_STAGE_CAMERA_LOCK_AXIS_", (int)LockAxis);

            PlayerPrefs.SetFloat("SAVE_STAGE_CAMERA_SMOOTH_TIME_", SmoothTime);
        }

        public void Load()
        {
            Distance.x = PlayerPrefs.GetFloat("SAVE_STAGE_CAMERA_DISTANCE_X_");
            Distance.y = PlayerPrefs.GetFloat("SAVE_STAGE_CAMERA_DISTANCE_Y_");
            Distance.z = PlayerPrefs.GetFloat("SAVE_STAGE_CAMERA_DISTANCE_Z_");

            LookAt.x = PlayerPrefs.GetFloat("SAVE_STAGE_CAMERA_LOOKAT_X_");
            LookAt.y = PlayerPrefs.GetFloat("SAVE_STAGE_CAMERA_LOOKAT_Y_");

            LockAxis = (Unit.eAxisType)PlayerPrefs.GetInt("SAVE_STAGE_CAMERA_LOCK_AXIS_");

            SmoothTime = PlayerPrefs.GetFloat("SAVE_STAGE_CAMERA_SMOOTH_TIME_");
        }

        public void RemoveSaveData()
        {
            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_MODE_");

            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_DISTANCE_X_");
            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_DISTANCE_Y_");
            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_DISTANCE_Z_");

            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_LOOKAT_X_");
            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_LOOKAT_Y_");

            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_LOCK_AXIS_");

            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_SMOOTH_TIME_");
        }
    }

    [System.Serializable]
    public class sFocusSetting : ISetting
    {
        public GameObject   Target;
        public float        FOV;
        public Vector3      Position;
        public Vector3      Rotation;

        public Quaternion   LookRotation;
    }

    [System.Serializable]
    public class sFixedSetting : ISetting
    {
        public Vector3  Position;
        public Vector3  Rotation;
        public float    FOV;


        public void Save()
        {
            PlayerPrefs.SetInt("SAVE_STAGE_CAMERA_MODE_", (int)EMode.FIXED);

            PlayerPrefs.SetFloat("SAVE_STAGE_CAMERA_POSITION_X_", Position.x);
            PlayerPrefs.SetFloat("SAVE_STAGE_CAMERA_POSITION_Y_", Position.y);
            PlayerPrefs.SetFloat("SAVE_STAGE_CAMERA_POSITION_Z_", Position.z);

            PlayerPrefs.SetFloat("SAVE_STAGE_CAMERA_ROTATION_X_", Rotation.x);
            PlayerPrefs.SetFloat("SAVE_STAGE_CAMERA_ROTATION_Y_", Rotation.y);
            PlayerPrefs.SetFloat("SAVE_STAGE_CAMERA_ROTATION_Z_", Rotation.z);

            PlayerPrefs.SetFloat("SAVE_STAGE_CAMERA_FOV_", FOV);
        }

        public void Load()
        {
            Position.x = PlayerPrefs.GetFloat("SAVE_STAGE_CAMERA_POSITION_X_");
            Position.y = PlayerPrefs.GetFloat("SAVE_STAGE_CAMERA_POSITION_Y_");
            Position.z = PlayerPrefs.GetFloat("SAVE_STAGE_CAMERA_POSITION_Z_");

            Rotation.x = PlayerPrefs.GetFloat("SAVE_STAGE_CAMERA_ROTATION_X_");
            Rotation.y = PlayerPrefs.GetFloat("SAVE_STAGE_CAMERA_ROTATION_Y_");
            Rotation.z = PlayerPrefs.GetFloat("SAVE_STAGE_CAMERA_ROTATION_Z_");

            FOV = PlayerPrefs.GetFloat("SAVE_STAGE_CAMERA_FOV_");
        }

        public void RemoveSaveData()
        {
            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_MODE_");

            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_POSITION_X_");
            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_POSITION_Y_");
            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_POSITION_Z_");

            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_ROTATION_X_");
            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_ROTATION_Y_");
            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_ROTATION_Z_");

            PlayerPrefs.DeleteKey("SAVE_STAGE_CAMERA_FOV_");
        }
    }

    [System.Serializable]
    public class sFollowPlayerSetting : ISetting
    {
        public Vector3  Distance;
        public Vector2  LookAt;
        public float    SmoothTime;
    }

    [System.Serializable]
    public class sSimpleSetting : ISetting
    {
        public Vector3 Distance;
    }

    public class sAnimationData
    {
        public Animation        Animation;
        public AnimationClip    Clip;
        public bool             IsPlaying;
        public float            Time;
        public GameObject       Parent;
    }

    public class sTurnData
    {
        public bool     Turning;
        public float    StartAngle;
        public float    DestAngle;
        public float    Duration;
        public float    T;
        public float    Angle;
        public bool     PuasePlayerInput;
        public Unit     CheckVisibleTarget;
        public Vector3  CheckVisiblePos;
    }

    public class sShakeData
    {
        public bool     Shaking     = false;
        public float    Duration;
        public float    Power;
        public float    Speed;
        public float    Magnitude;
        public Vector3  Noise;
    }


    public static Vector3   DEFAULT_CAMERA_DISTANCE = new Vector3(0.0f, 2.3f, -5.0f);
    public static Vector2   DEFAULT_CAMERA_LOOKAT   = new Vector2(0.0f, 1.3f);
    public static float     DEFAULT_FOV             = 55.0f;

    // Inspector Proprety
    public EMode                Mode                = EMode.DEFAULT;
    public sDefaultSetting      DefaultSetting      = new sDefaultSetting();
    public sDefaultSetting      UserSetting         = new sDefaultSetting();
    public sSideSetting         SideSetting         = new sSideSetting();
    public sFocusSetting        FocusSetting        = new sFocusSetting();
    public sFixedSetting        FixedSetting        = new sFixedSetting();
    public sFollowPlayerSetting FollowPlayerSetting = new sFollowPlayerSetting();
    public sSimpleSetting       SimpleSetting       = new sSimpleSetting();
    public sFixedSetting        PVPSetting          = new sFixedSetting();

    public bool     SkipCollisionWall   { get; set; }           = false;
    public bool     UsingUserSetting    { get; private set; }   = false;
    public Light    PlayerLight         { get; private set; }   = null;
    public Camera   MainCamera          { get; private set; }   = null;

    private readonly float  MIN_ROT_X           = -12.0f;
    private readonly float  MAX_ROT_X           = 45.0f;
    private readonly float  MIN_FOV             = 20.0f;
    private readonly float  MAX_FOV             = 80.0f;
    private readonly float  DEFAULT_SMOOTH_TIME = 0.05f;
    
    private int                 mOriginalCullingMask    = 0;
    private bool                mModeChanging = false;
    private PostProcessLayer    mPostProcessLayer			= null;
    private PostProcessVolume   mPostProcessVolume			= null;
    private PostProcessProfile  mOriginalProfile			= null;
    private Bloom               mBloom						= null;
    private ColorGrading        mColorGrading				= null;
    private AmbientOcclusion    mAmbientOcclusion			= null;
	private float				mOriginalPostProcessWeight	= 0.0f;
    private Texture2D           mTexLUT						= null;
    private float               mMaxContribution			= 1.0f;
    private RadialBlur          mRadialBlur					= null;
    private Player              mPlayer						= null;
    private sAnimationData      mAnimationData				= new sAnimationData();
    private sTurnData           mTurnData					= new sTurnData();
    private sShakeData          mShakeData					= new sShakeData();
    private bool                mStartCtrl					= false;
    private int                 mFingerId					= -1;
    private Vector2             mRelativeTouchPos			= Vector2.zero;
    private Vector3             mBeforeTouchPos				= Vector3.zero;
    private Vector3             mVelocity					= Vector3.zero;
    private TimeSpan            mCheckBtnTime				= TimeSpan.Zero;
	private WaitForFixedUpdate	mWaitForFixedUpdate			= new WaitForFixedUpdate();


    public void EnableCamera(bool enable)
    {
        if (MainCamera == null)
        {
            return;
        }

        MainCamera.enabled = enable;
    }

    public void SetPlayer(Player player)
    {
        mPlayer = player;
        if(mPlayer == null)
        {
            Debug.LogError("플레이어를 설정할 수 없습니다.");
            return;
        }

        InitPostProcess();
    }

    public void SetMode(EMode mode, ISetting setting, float duration = 0.0f)
    {
        Mode = mode;
        EndTouch();

        if(Mode == EMode.DEFAULT)
        {
            (setting as sDefaultSetting).Duplicate(ref DefaultSetting);
            SetDefaultMode(DefaultSetting.Distance, DefaultSetting.LookAt);
        }
        else if(Mode == EMode.SIDE)
        {
            SideSetting = setting as sSideSetting;
            SetSideMode(SideSetting.Distance, SideSetting.LookAt, SideSetting.LockAxis);
        }
        else if(Mode == EMode.FOCUS)
        {
            FocusSetting = setting as sFocusSetting;
            SetFocusMode(FocusSetting.Target, FocusSetting.FOV, FocusSetting.Position, FocusSetting.Rotation);
        }
        else if(Mode == EMode.FIXED)
        {
            FixedSetting = setting as sFixedSetting;
            SetFixedMode(FixedSetting.Position, FixedSetting.Rotation, DEFAULT_FOV);
        }
        else if(Mode == EMode.FOLLOW_PLAYER)
        {
            FollowPlayerSetting = setting as sFollowPlayerSetting;
            SetFollowPlayerMode(FollowPlayerSetting.Distance, FollowPlayerSetting.LookAt);
        }

        if(duration > 0.0f && mPlayer.Input)
        {
            mModeChanging = true;

            mPlayer.Input.Pause(true);
            Invoke("ResumePlayerInput", duration );
        }
    }

    public void SetInitialDefaultMode()
    {
        SetDefaultMode(DEFAULT_CAMERA_DISTANCE, DEFAULT_CAMERA_LOOKAT, false);

        transform.position = mPlayer.transform.position + (Quaternion.Euler(0.0f, mPlayer.transform.eulerAngles.y, 0.0f) * DefaultSetting.Distance);
        transform.LookAt(mPlayer.transform.position + (transform.right * DefaultSetting.LookAt.x) + (Vector3.up * DefaultSetting.LookAt.y));

        MainCamera.fieldOfView = DEFAULT_FOV;
    }

    public void SetPlayerBackCamera(float duration)
    {
        if (UsingUserSetting || Mode != EMode.DEFAULT)
        {
            return;
        }

        if(duration <= 0.0f)
        {
            duration = 0.1f;
        }

        InitRelativePos(0.0f);
        SetDefaultMode(DefaultSetting.Distance, DefaultSetting.LookAt, true, 1.0f, duration);
    }

    public void SetDefaultMode(Vector3 distance, Vector2 lookAt, bool lookAtPlayerBack = true, float smoothTimeRatio = 1.0f, float lookAtPlayerBackDuration = 0.75f)
    {
        if(mPlayer == null)
        {
            Debug.LogError("플레이어가 없음.");
            return;
        }

        Mode = EMode.DEFAULT;
        EndTouch();

        distance.z = -Mathf.Abs(distance.z);

        DefaultSetting.Distance = distance;
        DefaultSetting.LookAt = lookAt;
        DefaultSetting.LookAtPlayerBack = lookAtPlayerBack;
        DefaultSetting.LookAtPlayerBackAngle = mPlayer.transform.rotation.eulerAngles.y;
        DefaultSetting.LookAtPlayerBackCheckTime = 0.0f;
        DefaultSetting.LookAtPlayerBackDuration = lookAtPlayerBackDuration;
        DefaultSetting.SmoothTimeRatio = smoothTimeRatio;
        DefaultSetting.FOV = DEFAULT_FOV;

        MainCamera.fieldOfView = DefaultSetting.FOV;
    }

    public void SetDefaultMode(sDefaultSetting defaultSetting, bool lookAtPlayerBack = true)
    {
        SetDefaultMode(defaultSetting.Distance, defaultSetting.LookAt, lookAtPlayerBack);
    }

    public void SetDefaultMode(bool lookAtPlayerBack, float smoothTimeRatio)
    {
        SetDefaultMode(DefaultSetting.Distance, DefaultSetting.LookAt, lookAtPlayerBack, smoothTimeRatio);
    }

    public void SetDefaultMode()
    {
        SetDefaultMode(DEFAULT_CAMERA_DISTANCE, DEFAULT_CAMERA_LOOKAT);
    }

    /*
    public void SetDefaultModeTarget(Unit target, bool isTargetBoss)
    {
        if (target == null && Mode == EMode.DEFAULT)
        {
            SetDefaultMode();
        }

        //DefaultSetting.Distance.z *= 1.2f;

        DefaultSetting.Target = target;
        DefaultSetting.isTargetBoss = isTargetBoss;
        DefaultSetting.T = 0.0f;
    }
    */

    public void SetUserSetting(Vector3 distance, Vector2 lookAt, bool lookAtPlayerBack, float smoothTimeRatio, Unit target, float FOV)
    {
        if (mPlayer == null)
        {
            Debug.LogError("플레이어가 없음.");
            return;
        }

        UsingUserSetting = true;
        Mode = EMode.DEFAULT;

        EndTouch();
        StopTurnToTarget();
        InitRelativePos(0.0f);

        distance.z = -Mathf.Abs(distance.z);

        UserSetting.Distance = distance;
        UserSetting.LookAt = lookAt;
        UserSetting.LookAtPlayerBack = lookAtPlayerBack;
        UserSetting.LookAtPlayerBackAngle = mPlayer.transform.rotation.eulerAngles.y;
        UserSetting.LookAtPlayerBackCheckTime = 0.0f;
        UserSetting.LookAtPlayerBackDuration = 0.75f;
        UserSetting.SmoothTimeRatio = smoothTimeRatio;
        UserSetting.Target = target;
        UserSetting.isTargetBoss = false;
        UserSetting.T = 0.0f;
        UserSetting.FOV = FOV;

        StopCoroutine("UpdateFOV");
        StartCoroutine("UpdateFOV", UserSetting);
    }

    public void SetUserSetting(Vector3 distance, Vector2 lookAt, Vector3 targetPos)
    {
        if (mPlayer == null)
        {
            Debug.LogError("플레이어가 없음.");
            return;
        }

        UsingUserSetting = true;
        Mode = EMode.DEFAULT;

        EndTouch();
        StopTurnToTarget();
        InitRelativePos(0.0f);

        distance.z = -Mathf.Abs(distance.z);

        UserSetting.Distance = distance;
        UserSetting.LookAt = lookAt;
        UserSetting.LookAtPlayerBack = false;
        UserSetting.LookAtPlayerBackAngle = mPlayer.transform.rotation.eulerAngles.y;
        UserSetting.LookAtPlayerBackCheckTime = 0.0f;
        UserSetting.LookAtPlayerBackDuration = 0.75f;
        UserSetting.SmoothTimeRatio = 1.0f;
        UserSetting.Target = null;
        UserSetting.isTargetBoss = false;
        UserSetting.T = 0.0f;
        UserSetting.FOV = DEFAULT_FOV;

        targetPos.y = mPlayer.transform.position.y;
        Quaternion q = Quaternion.LookRotation(targetPos - mPlayer.transform.position);

        transform.position = mPlayer.transform.position + (q * UserSetting.Distance);
        transform.position = CheckCollisionWall(mPlayer.transform.position + (Vector3.up * UserSetting.Distance.y));

        transform.LookAt(mPlayer.transform.position + (transform.right * UserSetting.LookAt.x) + (Vector3.up * UserSetting.LookAt.y));

        StopCoroutine("UpdateFOV");
        StartCoroutine("UpdateFOV", UserSetting);
    }

    public void EndUserSetting()
    {
        if(!UsingUserSetting)
        {
            return;
        }

        UsingUserSetting = false;

        StopCoroutine("UpdateFOV");
        StartCoroutine("UpdateFOV", DefaultSetting);
    }

    public void TurnToTarget(Vector3 targetPos, float duration, bool reset = false)
    {
        if(Mode != EMode.DEFAULT)
        {
            return;
        }

        if(reset)
        {
            StopTurnToTarget();
        }
        else if(mTurnData.Turning)
        {
            return;
        }

        mTurnData.Turning = true;
        mTurnData.StartAngle = transform.eulerAngles.y;
        mTurnData.DestAngle = Quaternion.LookRotation(targetPos - mPlayer.transform.position).eulerAngles.y;
        mTurnData.Duration = duration;
        mTurnData.T = 0.0f;
        mTurnData.Angle = 0.0f;
        mTurnData.PuasePlayerInput = false;
        mTurnData.CheckVisibleTarget = null;
        mTurnData.CheckVisiblePos = Vector3.zero;
    }

    public void TurnToTarget(Vector3 targetPos, float duration, bool reset, Unit checkVisibleTarget)
    {
        if (Mode != EMode.DEFAULT || mTurnData.Turning)
        {
            return;
        }

        if (reset)
        {
            StopTurnToTarget();
        }

        mTurnData.Turning = true;
        mTurnData.StartAngle = transform.eulerAngles.y;
        mTurnData.DestAngle = Quaternion.LookRotation(targetPos - mPlayer.transform.position).eulerAngles.y;
        mTurnData.Duration = duration;
        mTurnData.T = 0.0f;
        mTurnData.Angle = 0.0f;
        mTurnData.PuasePlayerInput = false;
        mTurnData.CheckVisibleTarget = checkVisibleTarget;
        mTurnData.CheckVisiblePos = Vector3.zero;
    }

    public void TurnToTarget(Vector3 targetPos, float duration, bool reset, Vector3 checkVisiblePos)
    {
        if (Mode != EMode.DEFAULT || mTurnData.Turning)
        {
            return;
        }

        if (reset)
        {
            StopTurnToTarget();
        }

        mTurnData.Turning = true;
        mTurnData.StartAngle = transform.eulerAngles.y;
        mTurnData.DestAngle = Quaternion.LookRotation(targetPos - mPlayer.transform.position).eulerAngles.y;
        mTurnData.Duration = duration;
        mTurnData.T = 0.0f;
        mTurnData.Angle = 0.0f;
        mTurnData.PuasePlayerInput = false;
        mTurnData.CheckVisibleTarget = null;
        mTurnData.CheckVisiblePos = checkVisiblePos;
    }

    public void StopTurnToTarget()
    {
        if (mTurnData.Turning)
        {
            Log.Show("InGameCamera::StopTurnToTarget", Log.ColorType.Navy);
        }

        mTurnData.Turning = false;
        mTurnData.Angle = 0.0f;
        mTurnData.CheckVisibleTarget = null;
        mTurnData.CheckVisiblePos = Vector3.zero;
    }

    public void SetSideMode(Vector3 distance, Vector2 lookAt, Unit.eAxisType lockAxis, float smoothTime = 0.2f)
    {
        if (mPlayer == null)
        {
            Debug.LogError("플레이어가 없음.");
            return;
        }

        Mode = EMode.SIDE;
        EndTouch();

        SideSetting.Distance = distance;
        SideSetting.LookAt = lookAt;
        SideSetting.LockAxis = lockAxis;
        SideSetting.SmoothTime = smoothTime;

        mPlayer.SetLockAxis(lockAxis);
    }

    public void SetSideMode(sSideSetting sideSetting)
    {
        if (mPlayer == null)
        {
            Debug.LogError("플레이어가 없음.");
            return;
        }

        Mode = EMode.SIDE;
        EndTouch();

        SideSetting.Distance = sideSetting.Distance;
        SideSetting.LookAt = sideSetting.LookAt;
        SideSetting.SmoothTime = sideSetting.SmoothTime;

        transform.position = mPlayer.transform.position + SideSetting.Distance;
        transform.LookAt(mPlayer.transform.position + (Vector3.up * SideSetting.LookAt.y));
    }

    public void SetFocusMode(GameObject target, float FOV, Vector3 position, Vector3 rotation)
    {
        if (mPlayer == null)
        {
            Debug.LogError("플레이어가 없음.");
            return;
        }

        if(target == null)
        {
            Debug.LogError("타겟이 없음");
            return;
        }

        Mode = EMode.FOCUS;
        EndTouch();

        FocusSetting.Target = target;
        FocusSetting.FOV = FOV;
        FocusSetting.Position = position;
        FocusSetting.Rotation = rotation;

        Vector3 x = (FocusSetting.Target.transform.right * FocusSetting.Position.x);
        Vector3 y = (FocusSetting.Target.transform.up * FocusSetting.Position.y);
        Vector3 z = (FocusSetting.Target.transform.forward * FocusSetting.Position.z);
        Vector3 pos = FocusSetting.Target.transform.position + x + y + z;

        FocusSetting.LookRotation = Quaternion.LookRotation((FocusSetting.Target.transform.position + x + y) - pos);
    }

    public void SetFixedMode(Vector3 position, Vector3 rotation, float fov, bool immediate = false)
    {
        if (mPlayer == null)
        {
            Debug.LogError("플레이어가 없음.");
            return;
        }

        Mode = EMode.FIXED;
        EndTouch();

        FixedSetting.Position = position;
        FixedSetting.Rotation = rotation;
        MainCamera.fieldOfView = FixedSetting.FOV = fov;

        if(immediate)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(rotation);
        }
    }

    public void SetFixedMode(sFixedSetting fixedSetting)
    {
        if (mPlayer == null)
        {
            Debug.LogError("플레이어가 없음.");
            return;
        }

        Mode = EMode.FIXED;
        EndTouch();

        FixedSetting.Position = fixedSetting.Position;
        FixedSetting.Rotation = fixedSetting.Rotation;
        FixedSetting.FOV = fixedSetting.FOV;

        transform.position = fixedSetting.Position;
        transform.rotation = Quaternion.Euler(fixedSetting.Rotation);
    }

    public void SetFollowPlayerMode(Vector3 distance, Vector2 lookAt, float smoothTime = 0.1f)
    {
        if (mPlayer == null)
        {
            Debug.LogError("플레이어가 없음.");
            return;
        }

        Mode = EMode.FOLLOW_PLAYER;
        EndTouch();

        FollowPlayerSetting.Distance = distance;
        FollowPlayerSetting.LookAt = lookAt;
        FollowPlayerSetting.SmoothTime = smoothTime;
    }

    public void SetSimpleMode(Vector3 distance)
    {
        if (mPlayer == null)
        {
            Debug.LogError("플레이어가 없음.");
            return;
        }

        Mode = EMode.SIMPLE;
        EndTouch();

        SimpleSetting.Distance = distance;
    }

    public void PlayAnimation(AnimationClip clip, GameObject parent, bool initTransform = false)
    {
        StopAni();

        mAnimationData.Clip = clip;
        mAnimationData.IsPlaying = true;
        mAnimationData.Time = 0.0f;
        mAnimationData.Parent = parent;

        if (parent)
        {
            transform.SetParent(parent.transform);
            Utility.InitTransform(MainCamera.gameObject);

            if(initTransform)
            {
                Utility.InitTransform(gameObject);
            }
        }

        mAnimationData.Animation.AddClip(clip, clip.name);
        mAnimationData.Animation.clip = clip;
        mAnimationData.Animation.Play(clip.name);
    }

    public void StopAni()
    {
        mAnimationData.IsPlaying = false;

        if (mAnimationData.Animation.clip != null)
        {
            mAnimationData.Animation.Stop();
            mAnimationData.Animation.RemoveClip(mAnimationData.Animation.clip);
            mAnimationData.Animation.clip = null;

            if (mAnimationData.Parent)
            {
                transform.SetParent(null);
                mAnimationData.Parent = null;
            }

            Utility.InitTransform(MainCamera.gameObject);
        }

        MainCamera.fieldOfView = DEFAULT_FOV;
    }

    public void SetCullingMask(int layer)
    {
        MainCamera.cullingMask = layer;
    }

    public void AddCullingMask(int layer)
    {
        MainCamera.cullingMask |= (1 << layer);
    }

    public void ExcludCullingMask(int layer)
    {
        MainCamera.cullingMask &= ~(1 << layer);
    }

    public void RestoreCullingMask()
    {
        MainCamera.cullingMask = mOriginalCullingMask;
    }

    public void InitRelativePos(float duration)
    {
        if(mRelativeTouchPos == Vector2.zero)
        {
            return;
        }

        StopCoroutine("UpdateInitRelativePos");
        StartCoroutine("UpdateInitRelativePos", duration);
    }

    public void ResetPostProcess()
    {
        if(mPostProcessVolume == null)
        {
            return;
        }

        DisableMotionBlur();
        mColorGrading.active = false;

        StopCoroutine("UpdateChangeProfile");
        StopCoroutine("UpdateLUT");

        ChangePostProcessProfile(mOriginalProfile, 0.0f);
		mPostProcessVolume.weight = mOriginalPostProcessWeight;
    }

    public void EnablePostEffects(bool enable)
    {
        if (mPostProcessVolume == null)
        {
            return;
        }

        if (mBloom)
        {
            mBloom.active = enable;
        }

        if(mAmbientOcclusion)
        {
            mAmbientOcclusion.active = enable;
        }
    }

    public void ChangePostProcessProfile(PostProcessProfile postProcessProfile, float duration)
    {
        if(mPostProcessVolume == null)
        {
            return;
        }

        mPostProcessVolume.profile = postProcessProfile;
		mPostProcessVolume.weight = 1.0f;

		if (duration > 0.0f)
        {
            StopCoroutine("UpdateChangeProfile");
            StartCoroutine("UpdateChangeProfile", duration);
        }
    }

    public void EnableLUT(float duration, Texture2D texLUT, float maxContribution)
    {
        if (mPostProcessVolume == null)
        {
            return;
        }

        mTexLUT = texLUT;
        mMaxContribution = maxContribution;

        StopCoroutine("UpdateLUT");
        StartCoroutine("UpdateLUT", duration);
    }

    public void EnableMotionBlur(float duration, float gradation = 0.0f)
    {
        if (mRadialBlur == null)
        {
            return;
        }

        mRadialBlur.enabled = true;

        if (duration > 0.0f)
        {
            StopCoroutine("StopMotionBlur");
            StartCoroutine("StopMotionBlur", duration);
        }

        if (gradation > 0.0f)
        {
            StopCoroutine("UpdateGradationMotionBlur");
            StartCoroutine("UpdateGradationMotionBlur", gradation);
        }
    }

    public void DisableMotionBlur()
    {
        if (mRadialBlur == null)
        {
            return;
        }

        StopCoroutine("StopMotionBlur");
        StopCoroutine("UpdateGradationMotionBlur");

        mRadialBlur.enabled = false;
    }

	public void PlayShake( Unit unit, float duration, float power, float speed, float magnitude ) {
		if( mShakeData.Shaking ) {
			return;
		}

        Player player = unit as Player;
        if( player == null || player.IsHelper ) {
            return;
		}

		mShakeData.Duration = duration;
		mShakeData.Power = power;
		mShakeData.Speed = speed * 5.0f;
		mShakeData.Magnitude = magnitude;

		StopCoroutine( "UpdateShake" );
		StartCoroutine( "UpdateShake" );
	}

	public void StopShake()
    {
        mShakeData.Shaking = false;
        StopCoroutine("UpdateShake");
    }

    public bool IsAnimationPlayingInPlayer()
    {
        if (mAnimationData.IsPlaying && mAnimationData.Parent)
        {
            return true;
        }

        return false;
    }

    public void ActivePlayerLight(bool active)
    {
        if(PlayerLight == null)
        {
            Debug.LogError("캐릭터 라이트가 없습니다.");
            return;
        }

        PlayerLight.gameObject.SetActive(active);
    }

    public void SetPlayerLightPositionAndRotation(Vector3 position, Vector3 rotation, bool local = true)
    {
        if(PlayerLight == null)
        {
            Debug.LogError("플레이어 라이트가 없습니다.");
            return;
        }

        if (local)
        {
            PlayerLight.transform.localPosition = position;
            PlayerLight.transform.localEulerAngles = rotation;
        }
        else
        {
            PlayerLight.transform.position = position;
            PlayerLight.transform.eulerAngles = rotation;
        }
    }

    private void Awake()
    {
        MainCamera = GetComponentInChildren<Camera>();
        MainCamera.fieldOfView = DEFAULT_FOV;
        MainCamera.allowMSAA = false;
        Utility.InitTransform(MainCamera.gameObject);

        mOriginalCullingMask = MainCamera.cullingMask;
        PlayerLight = GetComponentInChildren<Light>();

        InitPostProcess();

        mAnimationData.Animation = MainCamera.GetComponent<Animation>();
        if (mAnimationData.Animation == null)
        {
            mAnimationData.Animation = MainCamera.gameObject.AddComponent<Animation>();
        }

        mAnimationData.Animation.playAutomatically = false;

        MainCamera.nearClipPlane = Mathf.Max(0.1f, MainCamera.nearClipPlane);
    }

    private void InitPostProcess()
    {
        mPostProcessLayer = GetComponentInChildren<PostProcessLayer>();
        mPostProcessVolume = GetComponentInChildren<PostProcessVolume>();

        mOriginalProfile = ResourceMgr.Instance.LoadFromAssetBundle("etc", "Etc/PostProcess/scene_roof_top.asset") as PostProcessProfile;
        mPostProcessVolume.profile = mOriginalProfile;

        mPostProcessLayer.enabled = true;

        mBloom = mPostProcessVolume.profile.GetSetting<Bloom>();
        mColorGrading = mPostProcessVolume.profile.GetSetting<ColorGrading>();
        mAmbientOcclusion = mPostProcessVolume.profile.GetSetting<AmbientOcclusion>();

		mOriginalPostProcessWeight = mPostProcessVolume.weight;

		mRadialBlur = GetComponentInChildren<RadialBlur>();
        mRadialBlur.enabled = false;
    }

    private void Update()
    {
		if( mPlayer == null || mPlayer.Input == null || Mode != EMode.DEFAULT || !World.Instance.UIPlay.gameObject.activeSelf || 
            !World.Instance.EnemyMgr.gameObject.activeSelf || World.Instance.IsEndGame ) {
			EndTouch();
			return;
		}

		CheckInitialCameraSetting();

        bool touchBegin = AppMgr.Instance.CustomInput.IsTouchBegin;
        bool touchEnd = AppMgr.Instance.CustomInput.IsTouchEnd;
        Vector3 touchPos = AppMgr.Instance.CustomInput.GetTouchPos();

#if !UNITY_EDITOR
        if(AppMgr.Instance.CustomInput.InputType == BaseCustomInput.eInputType.Touch)
        {
            touchBegin = false;
            touchEnd = false;

            Touch touch = AppMgr.Instance.CustomInput.GetTouchOutJoystickArea();
            if(touch.fingerId >= 0)
            {
                touchBegin = touch.phase == TouchPhase.Began;
                touchEnd = touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
                touchPos = touch.position;
            }
        }
#endif

        if (touchBegin)
        {
            mStartCtrl = true;
            mBeforeTouchPos = touchPos;

        }
        else if(touchEnd)
        {
            EndTouch();
        }

        if(!UsingUserSetting || !DefaultSetting.isTargetBoss)
        {
            bool input = false;

            if (Utility.IsInJoystickArea(touchPos) || AppMgr.Instance.CustomInput.IsOverUI())
            {
                EndTouch();
                return;
            }

            if (AppMgr.Instance.CustomInput.InputType == BaseCustomInput.eInputType.Touch && mStartCtrl)
            {
                input = true;

				float magicNumber = ((float)FSaveData.Instance.CameraSensitivity - 5.0f);
				if(magicNumber > 0.0f)
				{
					magicNumber *= 2.0f;
				}
				else
				{
					magicNumber = Mathf.Pow(Mathf.Abs(magicNumber) + 1.0f, -2.0f);
				}

                mRelativeTouchPos.x = (touchPos.x - mBeforeTouchPos.x) * (0.4f * magicNumber);
                mRelativeTouchPos.y = Mathf.Clamp(mRelativeTouchPos.y + ((mBeforeTouchPos.y - touchPos.y) * (0.2f * magicNumber)), MIN_ROT_X, MAX_ROT_X);

                StopTurnToTarget();
            }
            else if(AppMgr.Instance.CustomInput.InputType == BaseCustomInput.eInputType.KMGP)
            {
                input = true;

                float x = AppMgr.Instance.CustomInput.GetMouseXAxis();
                float y = AppMgr.Instance.CustomInput.GetMouseYAxis();

                mRelativeTouchPos.x = x * (AppMgr.Instance.CustomInput.Sensitivity * 0.7f);
                mRelativeTouchPos.y = Mathf.Clamp(mRelativeTouchPos.y - y, MIN_ROT_X, MAX_ROT_X);

                if (x != 0.0f || y != 0.0f)
                {
                    StopTurnToTarget();
                }
            }

            if (input)
            {
                if (touchPos != mBeforeTouchPos)
                {
                    mCheckBtnTime = TimeSpan.Zero;
                }

                mBeforeTouchPos = touchPos;
                DefaultSetting.LookAtPlayerBack = false;
            }
        }
    }

    public void EndTouch()
    {
        mStartCtrl = false;
        mFingerId = -1;
        mRelativeTouchPos.x = 0.0f;
    }

    private void CheckInitialCameraSetting()
    {
        if (UsingUserSetting || Mode != EMode.DEFAULT || AppMgr.Instance.CustomInput.IsOverUI())
        {
            return;
        }

        if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Select)) //Input.GetMouseButtonDown(0))
        {
            if (mCheckBtnTime == TimeSpan.Zero)
            {
                mCheckBtnTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
                Debug.Log("mCheckBtnTime = TimeSpan.FromTicks(DateTime.Now.Ticks);");
            }
            else
            {
                double sec = (TimeSpan.FromTicks(DateTime.Now.Ticks) - mCheckBtnTime).TotalSeconds;
                Debug.Log(sec);

                if (sec <= 0.3f)
                {
                    SetPlayerBackCamera(0.75f);
                }

                mCheckBtnTime = TimeSpan.Zero;
            }
        }
    }

    private void FixedUpdate()
    {
        if(mPlayer == null || World.Instance.IsEndGame)
        {
            return;
        }

        switch (Mode)
        {
            case EMode.DEFAULT:
				UpdateDefaultMode();
				break;

			case EMode.SIDE:
                UpdateSideMode();
                break;

            case EMode.FIXED:
                UpdateFixedMode();
                break;

            case EMode.FOLLOW_PLAYER:
                UpdateFollowPlayer();
                break;
        }

        UpdateAnimation();
    }

    private void LateUpdate()
    {
        if (mPlayer == null || World.Instance.IsEndGame)
        {
            return;
        }

        switch (Mode)
        {
            /*
            case EMode.SIDE:
                UpdateSideMode();
                break;
                */

            case EMode.FOCUS:
                UpdateFocusMode();
                break;

                /*
            case EMode.FIXED:
                UpdateFixedMode();
                break;

            case EMode.FOLLOW_PLAYER:
                UpdateFollowPlayer();
                break;
                */

            case EMode.SIMPLE:
                transform.position += mPlayer.transform.position - transform.TransformPoint(SimpleSetting.Distance);
                break;

            case EMode.PVP:
                UpdatePVPMode();
                break;
        }
    }

    private void UpdateDefaultMode()
    {
        if(mAnimationData.IsPlaying && mAnimationData.Parent)
        {
            return;
        }

        sDefaultSetting currentSetting = DefaultSetting;
        if(UsingUserSetting)
        {
            currentSetting = UserSetting;
        }

        Vector3 playerPos = mPlayer.transform.position;
        Vector3 targetPos = Vector3.zero;
        Vector3 v1 = Vector3.zero;
        Vector3 v2 = Vector3.zero;

        if (currentSetting.Target)
        {
            if (currentSetting.isTargetBoss)
            {
                currentSetting.LookAt.x = 1.0f;
            }
            else
            {
                currentSetting.LookAt.x = 0.0f;
            }

            playerPos += (transform.right * currentSetting.LookAt.x) + (Vector3.up * (currentSetting.LookAt.y * 0.3f));
            targetPos = currentSetting.Target.MainCollider.GetCenterPos();

            v1 = playerPos;
            v2 = targetPos + (transform.right * currentSetting.LookAt.x);
        }
        else
        {
            v1 = (playerPos - (transform.right * currentSetting.LookAt.x));
            v2 = (transform.position - (transform.right * currentSetting.LookAt.x));
        }

        Quaternion qLook = Quaternion.identity;
        if (currentSetting.Target == null)
        {
            qLook = Quaternion.LookRotation(v1 - v2);
        }
        else
        {
            qLook = Quaternion.LookRotation(v2 - v1);
        }

        if (mTurnData.Turning)
        {
            if(currentSetting.LookAtPlayerBack)
            {
                currentSetting.LookAtPlayerBack = false;
            }

            bool isVisible = false;
            if (mTurnData.CheckVisibleTarget)
            {
                Vector3 viewPortPt = Camera.main.WorldToViewportPoint(mTurnData.CheckVisibleTarget.GetCenterPos());
                isVisible = Utility.IsInViewPort(viewPortPt, 0.2f, 0.8f);
            }

            if (mTurnData.T >= 1.0f || 
                (mTurnData.CheckVisibleTarget && (isVisible || mTurnData.CheckVisibleTarget.curHp <= 0.0f)) ||
                (mTurnData.CheckVisiblePos != Vector3.zero && Utility.IsInViewPort(MainCamera.WorldToViewportPoint(mTurnData.CheckVisiblePos))))
            {
                StopTurnToTarget();
            }
            else
            {
                mTurnData.T += (mPlayer.fixedDeltaTime / mTurnData.Duration);
                mTurnData.Angle = Mathf.LerpAngle(mTurnData.StartAngle, mTurnData.DestAngle, mTurnData.T) - qLook.eulerAngles.y;
                mRelativeTouchPos.y = Mathf.LerpAngle(mRelativeTouchPos.y, 0.0f, mTurnData.T);
            }
        }
        else if (currentSetting.LookAtPlayerBack)
        {
            if (currentSetting.LookAtPlayerBackCheckTime >= currentSetting.LookAtPlayerBackDuration)
            {
                currentSetting.LookAtPlayerBack = false;
                currentSetting.LookAtPlayerBackCheckTime = currentSetting.LookAtPlayerBackDuration;
            }
            else
            {
                currentSetting.LookAtPlayerBackCheckTime += Time.fixedDeltaTime;
                float t = currentSetting.LookAtPlayerBackCheckTime / currentSetting.LookAtPlayerBackDuration;

                float angle = Mathf.LerpAngle(transform.eulerAngles.y, currentSetting.LookAtPlayerBackAngle, t);
                qLook = Quaternion.Euler(0.0f, angle, 0.0f);
            }
        }
        else
        {
            mTurnData.Angle = 0.0f;
        }

        Vector3 dist = currentSetting.Distance + mPlayer._ExtraCameraDistance;
        dist.z -= (mPlayer.transform.localScale.x - 1.0f) * 1.11f;

        Quaternion q = Quaternion.Euler(mRelativeTouchPos.y, qLook.eulerAngles.y + mRelativeTouchPos.x + mTurnData.Angle, 0.0f);
        transform.position = Vector3.SmoothDamp(transform.position, (playerPos + (q * dist)), ref mVelocity, DEFAULT_SMOOTH_TIME * currentSetting.SmoothTimeRatio);
        transform.position = CheckCollisionWall(mPlayer.transform.position + (Vector3.up * currentSetting.Distance.y));

        if (currentSetting.Target == null)
        {
            transform.LookAt(playerPos + (transform.right * currentSetting.LookAt.x) + (Vector3.up * currentSetting.LookAt.y) + mShakeData.Noise);
        }
        else
        {
            transform.LookAt(((playerPos - (transform.right * currentSetting.LookAt.x)) + targetPos) * 0.5f + mShakeData.Noise);
        }
    }

	private void UpdateSideMode()
    {
        if((!mModeChanging && mPlayer.Input.GetDirection() != Vector3.zero) || mPlayer.IsAutoMove)
        {
            SideSetting.SmoothTime = 0.0f;
        }

        transform.position = Vector3.SmoothDamp(transform.position, mPlayer.transform.position + SideSetting.Distance, ref mVelocity, SideSetting.SmoothTime);
        transform.position = CheckCollisionWall(mPlayer.transform.position + (Vector3.up * SideSetting.LookAt.y));

        transform.LookAt(mPlayer.transform.position + (Vector3.up * SideSetting.LookAt.y));
    }

    private void UpdateFocusMode()
    {
        if(FocusSetting.Target == null)
        {
            return;
        }

        Vector3 x = (FocusSetting.Target.transform.right * FocusSetting.Position.x);
        Vector3 y = (FocusSetting.Target.transform.up * FocusSetting.Position.y);
        Vector3 z = (FocusSetting.Target.transform.forward * FocusSetting.Position.z);
        Vector3 pos = FocusSetting.Target.transform.position + x + y + z;

        Quaternion q = Quaternion.LookRotation((FocusSetting.Target.transform.position + x + y) - pos);

        transform.position = pos;
        transform.rotation = q * Quaternion.Euler(FocusSetting.Rotation);

        MainCamera.fieldOfView = FocusSetting.FOV;
    }

    public void UpdateFixedMode()
    {
        transform.position = Vector3.SmoothDamp(transform.position, FixedSetting.Position, ref mVelocity, 0.1f);
        transform.rotation = Quaternion.Euler(FixedSetting.Rotation);

        MainCamera.fieldOfView = FixedSetting.FOV;
    }

    private void UpdateFollowPlayer()
    {
        transform.position = Vector3.SmoothDamp(transform.position, mPlayer.transform.position + (Quaternion.Euler(0.0f, mPlayer.transform.eulerAngles.y, 0.0f) * FollowPlayerSetting.Distance), ref mVelocity, FollowPlayerSetting.SmoothTime);
        transform.position = CheckCollisionWall(mPlayer.transform.position + (Vector3.up * FollowPlayerSetting.LookAt.y));

        transform.LookAt(mPlayer.transform.position + (transform.right * FollowPlayerSetting.LookAt.x) + (Vector3.up * FollowPlayerSetting.LookAt.y));
    }

    private void UpdatePVPMode()
    {
        if (!World.Instance.IsEndGame)
        {
            Vector3 playerPos = mPlayer.MainCollider.GetCenterPos();
            Vector3 opponentPos = mPlayer.mainTarget.MainCollider.GetCenterPos();

            Vector3 center = Vector3.Lerp(playerPos, opponentPos, 0.5f) + PVPSetting.Position;
            float distance = Vector3.Distance(playerPos, opponentPos);

            transform.position = center - (transform.forward * (Mathf.Max(PVPSetting.FOV, distance / 1.2f)));
            transform.LookAt(center);
            transform.rotation = Quaternion.Euler(PVPSetting.Rotation);
        }
        else
        {
            transform.SetPositionAndRotation(new Vector3(0.0f, 1.2f, -1.8f), Quaternion.Euler(5.0f, 0.0f, 0.0f));
        }
    }

    private Vector3 CheckCollisionWall(Vector3 startPos)
    {
        Vector3 newPos = transform.position;

        if (SkipCollisionWall)
        {
            return newPos;
        }

        if (Physics.Linecast(startPos, transform.position, out RaycastHit hitInfo, 1 << (int)eLayer.Wall_Inside))
        {
            //Vector3 dir = (transform.position - startPos).normalized;
            newPos = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);// + dir;
        }

        return newPos;
    }

    private IEnumerator UpdateFOV(sDefaultSetting currentSetting)
    {
        float time = 0.0f;
        float duration = 0.3f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (time < 1.0f)
        {
            time += Time.fixedDeltaTime / duration;
            MainCamera.fieldOfView = Mathf.SmoothStep(MainCamera.fieldOfView, currentSetting.FOV, time);

            yield return mWaitForFixedUpdate;
        }

        MainCamera.fieldOfView = currentSetting.FOV;
    }

    private IEnumerator UpdateInitRelativePos(float duration)
    {
        float time = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (time < 1.0f)
        {
            time += Time.fixedDeltaTime / duration;

            mRelativeTouchPos.x = Mathf.SmoothStep(mRelativeTouchPos.x, 0.0f, time);
            mRelativeTouchPos.y = Mathf.SmoothStep(mRelativeTouchPos.y, 0.0f, time);

            MainCamera.fieldOfView = Mathf.SmoothStep(MainCamera.fieldOfView, DEFAULT_FOV, time);

            yield return mWaitForFixedUpdate;
        }

        mRelativeTouchPos = Vector2.zero;
    }

    private void ResumePlayerInput()
    {
        mModeChanging = false;

        if(Director.IsPlaying)
        {
            return;
        }

        mPlayer.Input.Pause(false);
    }

    private IEnumerator UpdateChangeProfile(float duration)
    {
        float time = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (time < duration)
        {
            time += Time.fixedDeltaTime;
            yield return mWaitForFixedUpdate;
        }

        ChangePostProcessProfile(mOriginalProfile, 0.0f);
		mPostProcessVolume.weight = mOriginalPostProcessWeight;
	}

    private IEnumerator UpdateLUT(float duration)
    {
        mColorGrading.active = true;
        mColorGrading.ldrLut.value = mTexLUT;
        mColorGrading.ldrLutContribution.value = mMaxContribution;

        float time = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (time < duration)
        {
            if (!World.Instance.IsPause)
            {
                time += Time.fixedDeltaTime;
            }

            yield return mWaitForFixedUpdate;
        }

        mColorGrading.active = false;
    }

    private IEnumerator UpdateGradationMotionBlur(float gradation)
    {
        float gradationTime = 0.0f;

        mRadialBlur.fSampleStrength = 1.5f;
        if (gradation > 0.0f)
            mRadialBlur.fSampleStrength = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (mRadialBlur.fSampleStrength <= 1.5f)
        {
            gradationTime += Time.fixedDeltaTime / gradation;
            mRadialBlur.fSampleStrength = Mathf.Lerp(0.0f, 1.5f, gradationTime);

            yield return mWaitForFixedUpdate;
        }

        mRadialBlur.fSampleStrength = 1.5f;
    }

    private IEnumerator StopMotionBlur(float duration)
    {
        yield return new WaitForSeconds(duration);
        mRadialBlur.enabled = false;
    }

    private IEnumerator UpdateShake()
    {
        mShakeData.Shaking = true;
        
        Vector3 originalCamPos = transform.position;
        float elapsed = 0.0f;

        while (elapsed < mShakeData.Duration)
        {
            elapsed += Time.deltaTime / mShakeData.Duration;

            float damper = 1.0f - Mathf.Clamp(2.0f * elapsed - 1.0f, 0.0f, 1.0f);
            float alpha = mShakeData.Speed * elapsed;

            mShakeData.Noise.x = Mathf.PerlinNoise(alpha, 0.0f) * 2.0f - 1.0f;
            mShakeData.Noise.y = Mathf.PerlinNoise(0.0f, alpha) * 2.0f - 1.0f;

            mShakeData.Noise.x *= (mShakeData.Magnitude * mShakeData.Power) * damper;
            mShakeData.Noise.y *= (mShakeData.Magnitude * mShakeData.Power) * damper;

            yield return null;
        }

        mShakeData.Noise = Vector3.zero;
        mShakeData.Shaking = false;
    }

    private void UpdateAnimation()
    {
        if (!mAnimationData.IsPlaying || mAnimationData.Clip.wrapMode == WrapMode.Loop)
        {
            return;
        }

        mAnimationData.Time += mPlayer.fixedDeltaTime;
        if (mAnimationData.Time >= mAnimationData.Clip.length)
        {
            StopAni();
        }
    }
}
