using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceFollowBike : MonoBehaviour
{
    public Transform kTarget;
    public Vector3 m_localTargetPosition;

    public Vector3 kTargetSpeed = Vector3.zero;
    void Start()
    {
        if (kTarget != null)
        {
            m_localTargetPosition = transform.InverseTransformPoint(kTarget.position);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(kTarget != null)
        {
            Vector3 targetPos = transform.TransformPoint(m_localTargetPosition + kTargetSpeed);
            transform.position += kTarget.position - targetPos;
        }
    }
}
