
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
===============================================================================

    GameClientTable에 CommonEffect와 연결되어 플레이할 이펙트 (스크린이펙트 UI 안에 존재)
    이펙트 매니저에서 CommonEffect를 플레이할 때 스크린이펙트 클래스에서 플레이 함수 호출

===============================================================================
*/

public class ConnectCommonEffect : MonoBehaviour
{
    [Header("[Property]")]
    public int              CommonEffectId  = 0;
    public ParticleSystem   Effect;


    public void Play()
    {
        Effect.gameObject.SetActive(true);

        Effect.Play();
        EffectManager.Instance.RegisterStopEff(Effect, null);
    }

    private void Awake()
    {
        Effect.gameObject.SetActive(false);
    }
}
