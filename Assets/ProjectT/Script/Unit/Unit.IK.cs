
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using RootMotion;
using RootMotion.FinalIK;


public abstract partial class Unit : MonoBehaviour//, IObserverSubject
{
    public enum eBipedIK
    {
        Body = 0,
        LeftHand,
        LeftShoulder,
        RightHand,
        RightShoulder,
        LeftFoot,
        LeftThigh,
        RightFoot,
        RightThigh,

        Count,
    }

    public class sIKObject
    {
        public eBipedIK bipedType;

        public Transform biped;
        public Transform bipedExtra;

        public GameObject targetLook; // 본에 붙어 있는 관상용 오브젝트
        public GameObject targetMovement; // 실제 움직일 타겟

        public Vector3 originalPos;
        public Quaternion originalRot;

        public bool scaling = false;
        public string name = null;

        public GameObject targetLookAt;
        public Vector3 lookAtAddPos;


        public void SetBiped(eBipedIK bipedIK, Transform biped, bool scaling, int nameTableId)
        {
            bipedType = bipedIK;
            this.biped = biped;

            if (biped)
            {
                targetLook.transform.position = biped.position;
                targetLook.transform.rotation = biped.rotation;

                if (targetMovement)
                {
                    targetMovement.transform.position = biped.position;
                    targetMovement.transform.rotation = biped.rotation;
                }

                originalPos = biped.position;
                originalRot = biped.rotation;
            }

            this.scaling = scaling;

            if (nameTableId > 0)
                name = FLocalizeString.Instance.GetText(nameTableId);
        }

        public void SetBipedAll(eBipedIK ikType, Transform biped, bool scaling, Vector3 addPos, int nameTableId)
        {
            SetBiped(ikType, biped, scaling, nameTableId);

            lookAtAddPos = addPos;
            targetLookAt.transform.position = biped.position + addPos;
        }

        public void Reset()
        {
            biped.transform.localScale = Vector3.one;

            targetLook.transform.localPosition = originalPos;
            targetLook.transform.localRotation = originalRot;
            targetLook.transform.localScale = Vector3.one;// * 0.125f;

            if (targetMovement)
            {
                targetMovement.transform.localPosition = originalPos;
                targetMovement.transform.localRotation = originalRot;
            }
        }
    }


    protected BipedIK m_bipedIK = null;
    protected List<sIKObject> m_listIKObject = new List<sIKObject>();
    protected sIKObject m_ikObjBreast = null;

    public List<sIKObject> listIKObject { get { return m_listIKObject; } }


    protected void InitIK()
    {
        m_bipedIK = gameObject.AddComponent<BipedIK>();
        BipedReferences.AutoDetectReferences(ref m_bipedIK.references, transform, new BipedReferences.AutoDetectParams(false, false));
        if (!m_bipedIK.references.isFilled)
        {
            m_bipedIK = null;
            Debug.Log("본 구조가 캐릭터랑 달라서 IK 못씀 " + name);

            return;
        }

        m_bipedIK.InitiateBipedIK();
        m_bipedIK.SetToDefaults();

        if (m_bipedIK == null)
            return;

        for (int i = 0; i < (int)eBipedIK.Count; i++)
        {
            sIKObject ikObj = new sIKObject();

            ikObj.targetLook = ResourceMgr.Instance.CreateFromAssetBundle("unit", "Unit/FigureSelectBone.prefab");
            ikObj.targetLook.name = ((eBipedIK)i).ToString() + "_TargetLook";
            ikObj.targetLook.transform.SetParent(transform);
            ikObj.targetLook.GetComponent<MeshRenderer>().material.renderQueue = 3001;
            Utility.InitTransform(ikObj.targetLook, Vector3.zero, Quaternion.identity, Vector3.one);// * 0.1f);
            ikObj.targetLook.SetActive(false);

            Transform[] finds = ikObj.targetLook.GetComponentsInChildren<Transform>();
            for (int j = 0; j < finds.Length; j++)
            {
                if (finds[j].name == "movement")
                {
                    ikObj.targetMovement = finds[j].gameObject;
                    ikObj.targetMovement.name = ((eBipedIK)i).ToString() + "_movement";
                    ikObj.targetMovement.transform.SetParent(transform);

                    break;
                }
            }

            m_listIKObject.Add(ikObj);
        }
    }

    public void EnableIK(bool enable)
    {
        if (m_bipedIK)
            m_bipedIK.enabled = enable;
    }

    public sIKObject GetIKObject(eBipedIK bipedIK)
    {
        if (m_listIKObject.Count <= 0)
            return null;

        return m_listIKObject[(int)bipedIK];
    }
}
