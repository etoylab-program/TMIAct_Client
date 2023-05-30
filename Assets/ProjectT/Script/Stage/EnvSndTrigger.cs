
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class EnvSndTrigger : MonoBehaviour
{
    public AudioClip clip;

    private AudioSource			m_audioSrc			= null;
	private WaitForFixedUpdate	mWaitForFixedUpdate	= new WaitForFixedUpdate();


	private void Awake()
    {
        m_audioSrc = gameObject.AddComponent<AudioSource>();
        m_audioSrc.playOnAwake = false;
        m_audioSrc.volume = FSaveData.Instance.GetSEVolume();
        m_audioSrc.loop = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        m_audioSrc.clip = clip;
        m_audioSrc.volume = FSaveData.Instance.GetSEVolume();
        m_audioSrc.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        StopCoroutine("FadeOut");
        StartCoroutine("FadeOut");
    }

    private IEnumerator FadeOut()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_audioSrc.volume > 0.0f)
        {
            m_audioSrc.volume -= Time.fixedDeltaTime * 0.6f;
            yield return mWaitForFixedUpdate;
        }

        m_audioSrc.Stop();
    }
}
