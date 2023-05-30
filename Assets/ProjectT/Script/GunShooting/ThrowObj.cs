using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObj : MonoBehaviour
{
    public float kRotationSpeed = 10f;
    public bool bisThrowing { get; private set; }


    private ThrowBezier _throwBezier;
    private float fTime = 0f;

    private float _originSpeed;

//    [HideInInspector]
    public bool bIsRightTrigger = false;
//    [HideInInspector]
    public bool bIsLeftTrigger = false;

    private int _fireScore = 0;

    private GameClientTable.MiniGameShoot.Param _throwObjInfo;

    private void Awake()
    {
        bisThrowing = false;
        fTime = 0f;

        _throwObjInfo = null;
    }

    private void Update()
    {
        if (!bisThrowing)
            return;

        this.transform.position = _throwBezier.GetPoint(fTime);
        this.transform.Rotate(fTime * kRotationSpeed * Time.timeScale, fTime * kRotationSpeed * Time.timeScale, fTime * kRotationSpeed * Time.timeScale);

        _originSpeed += Time.deltaTime * 0.2f;
        fTime += Time.deltaTime * _originSpeed;
        
        if (fTime >= 1f)
        {
            bisThrowing = false;
            SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, ((eThrowSoundType)_throwObjInfo.EffectType).ToString() + "_Failed");
            ThrowingManager.Instance.DamageHp();

            this.gameObject.SetActive(false);
        }
    }

    public void ShotThrow(ThrowBezier throwBezier, GameClientTable.MiniGameShoot.Param targetObj, float speedFlag)
    {
        _throwBezier = throwBezier;

        this.transform.position = _throwBezier.kStartPos.transform.position;
        _throwObjInfo = targetObj;
        fTime = 0f;
        _originSpeed = _throwObjInfo.MoveSpeed * speedFlag;
        bIsRightTrigger = false;
        bIsLeftTrigger = false;
        _fireScore = _throwObjInfo.Score;
        bisThrowing = true;
    }

    public void ShowThrow(ThrowBezier throwBezier)
    {
        _throwBezier = throwBezier;
        this.transform.position = _throwBezier.kStartPos.transform.position;
        fTime = 0f;
        bisThrowing = false;
    }

    public float GetPositionY()
    {
        return this.transform.position.y;
    }

    public float GetPositionZ()
    {
        return this.transform.position.x;
    }

    public void FireThrowObj()
    {
        bIsRightTrigger = false;
        bIsLeftTrigger = false;
        EffectManager.Instance.Play(this.transform.position, (int)_throwObjInfo.EffectType, EffectManager.eType.Each_Monster_Normal_Hit);

        SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, ((eThrowSoundType)_throwObjInfo.EffectType).ToString());
        Log.Show((int)_throwObjInfo.EffectType, Log.ColorType.Red);
        bisThrowing = false;
        ThrowingManager.Instance.AddScore(this.transform.position, _fireScore);
        this.gameObject.SetActive(false);
    }
}
