
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestRagdoll : MonoBehaviour
{
    public Rigidbody[] testRigidBodies;
    public Vector3 addForce;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            for(int i = 0; i < testRigidBodies.Length; i++)
                testRigidBodies[i].AddForce(addForce);
        }
    }
}
