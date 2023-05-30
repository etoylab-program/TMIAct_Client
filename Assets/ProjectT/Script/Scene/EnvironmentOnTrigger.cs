
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class EnvironmentOnTrigger : MonoBehaviour
{
    [System.Serializable]
    public class sChangeMaterialObject
    {
        public MeshRenderer Mesh;
        public Color        Color       = Color.white;
        public float        Emission    = 0.0f;

        [System.NonSerialized]
        public Animation Ani = null;
    }


    [Header("[Background Animation]")]
    public Animation        Ani;
    public AnimationClip    AniClip;

    [Header("[Change Material Object]")]
    public sChangeMaterialObject[] ChangeMtrlObjects;

    [Header("[Effect]")]
    public GameObject[] Effects;
    public bool         On;

    private BoxCollider mBoxCol = null;


    private void Awake()
    {
        for(int i = 0; i< ChangeMtrlObjects.Length; i++)
        {
            ChangeMtrlObjects[i].Ani = ChangeMtrlObjects[i].Mesh.GetComponent<Animation>();
            if(ChangeMtrlObjects[i].Ani)
            {
                ChangeMtrlObjects[i].Ani.playAutomatically = false;
            }
        }

        for(int i = 0; i < Effects.Length; i++)
        {
            Effects[i].gameObject.SetActive(false);
        }

        mBoxCol = GetComponent<BoxCollider>();
        mBoxCol.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (Ani.GetClip(AniClip.name))
        {
            Ani.RemoveClip(AniClip);
        }

        Ani.AddClip(AniClip, AniClip.name);
        Ani.clip = AniClip;
        Ani.Play(AniClip.name);

        mBoxCol.enabled = false;
        Invoke("OnTrigger", Ani.clip.length + 0.1f);
    }

    private void OnTrigger()
    {
        for (int i = 0; i < ChangeMtrlObjects.Length; i++)
        {
            sChangeMaterialObject obj = ChangeMtrlObjects[i];

            if (obj.Ani)
            {
                obj.Ani.Play();
            }

            if (obj.Mesh.material.HasProperty("_Color"))
            {
                obj.Mesh.material.SetColor("_Color", obj.Color);
            }

            if (obj.Mesh.material.HasProperty("_Emission_power"))
            {
                obj.Mesh.material.SetFloat("_Emission_power", obj.Emission);
            }
        }

        for (int i = 0; i < Effects.Length; i++)
        {
            Effects[i].gameObject.SetActive(On);
        }
    }
}
