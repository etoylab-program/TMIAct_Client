
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Part : MonoBehaviour
{
    [Header("[Effect]")]
    public ParticleSystem psDestroy;

    private Unit m_owner;
    private Transform m_parent;
    private Rigidbody m_rigidBody;
    private CapsuleCollider m_capsuleCol;
    //private BoxCollider m_boxCol;
    private List<Material> m_listMtrl = new List<Material>();

    private ParticleSystem m_psDestroy;


    private void Awake()
    {
        m_parent = transform.parent;
        m_rigidBody = GetComponent<Rigidbody>();
        //m_boxCol = GetComponent<BoxCollider>();
        m_capsuleCol = GetComponent<CapsuleCollider>();

        if (psDestroy)
        {
            GameObject gObj = psDestroy.gameObject;
            m_psDestroy = ResourceMgr.Instance.Instantiate<ParticleSystem>(ref gObj);
            m_psDestroy.transform.SetParent(transform);
            Utility.InitTransform(m_psDestroy.gameObject);
            m_psDestroy.gameObject.SetActive(false);
        }
    }

    public void Init(Unit owner)
    {
        m_owner = owner;
        gameObject.SetActive(true);

        transform.parent = m_parent;

        m_rigidBody.useGravity = false;
        m_rigidBody.isKinematic = true;
        if(m_capsuleCol != null)
            m_capsuleCol.isTrigger = true;

        Utility.InitTransform(m_rigidBody.gameObject);

        m_listMtrl.Clear();

        SkinnedMeshRenderer[] skinnedMeshs = GetComponents<SkinnedMeshRenderer>();
        for(int i = 0; i < skinnedMeshs.Length; i++)
        {
            for(int j = 0; j < skinnedMeshs[i].materials.Length; j++)
                m_listMtrl.Add(skinnedMeshs[i].materials[j]);
        }

        MeshRenderer[] meshs = GetComponents<MeshRenderer>();
        for (int i = 0; i < meshs.Length; i++)
        {
            for (int j = 0; j < meshs[i].materials.Length; j++)
                m_listMtrl.Add(meshs[i].materials[j]);
        }

        for (int i = 0; i < m_listMtrl.Count; i++)
            m_owner.aniEvent.AddMtrl(m_listMtrl[i]);

        if (m_psDestroy)
        {
            m_psDestroy.transform.SetParent(transform);
            Utility.InitTransform(m_psDestroy.gameObject);
            m_psDestroy.gameObject.SetActive(false);
        }
    }

    public void AddForce(Vector3 originPos, float speed)
    {
        if (transform.parent == null)
            return;

        transform.parent = null;

        if (m_capsuleCol != null)
            m_capsuleCol.isTrigger = false;

        m_rigidBody.useGravity = true;
        m_rigidBody.isKinematic = false;

        Vector3 dir = (transform.position - originPos).normalized;
        m_rigidBody.AddForce(dir * speed, ForceMode.VelocityChange);

        for (int i = 0; i < m_listMtrl.Count; i++)
            m_owner.aniEvent.RemoveMtrl(m_listMtrl[i]);

        if (m_psDestroy)
        {
            m_psDestroy.transform.SetParent(null);
            m_psDestroy.gameObject.SetActive(true);
        }
    }
}
