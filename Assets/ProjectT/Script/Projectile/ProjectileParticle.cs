
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileParticle : MonoBehaviour
{
    private Unit m_owner;
    private ParticleSystem[] m_psMainParticles = null;
    private float m_particleTime = 0.0f;
    private bool m_end = false;


    private void Awake()
    {
        m_psMainParticles = GetComponentsInChildren<ParticleSystem>(true);    

        if(m_psMainParticles != null)
            m_psMainParticles[0].Simulate(0.0f, true, false, true);
    }

    public void Init(Unit owner)
    {
        m_owner = owner;
        m_particleTime = 0.0f;
        m_end = false;
    }

    private void Update()
    {
        if (m_psMainParticles == null || m_end)
            return;

        float fixedDeltaTime = m_owner.fixedDeltaTime;
        if (fixedDeltaTime <= 0.0f)
        {
            fixedDeltaTime = Time.fixedDeltaTime;
        }

        m_particleTime += fixedDeltaTime;

        m_psMainParticles[0].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        for (int i = 0; i < m_psMainParticles.Length; i++)
        {
            bool useAutoRandomSeed = m_psMainParticles[i].useAutoRandomSeed;
            m_psMainParticles[i].useAutoRandomSeed = false;

            m_psMainParticles[i].Play(false);
            m_psMainParticles[i].Simulate(m_particleTime, false, false, true);

            m_psMainParticles[i].useAutoRandomSeed = useAutoRandomSeed;

            if (m_particleTime >= m_psMainParticles[0].main.duration)
            {
                m_psMainParticles[i].Play(false);
                m_psMainParticles[i].Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

                m_end = true;
            }
        }
    }
}
