using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI3DTestObect : MonoBehaviour
{
    public eANCHOR anchor;
    public Transform tf;

    UI3DTest ui3DTest;
    float angle;
    
    float rotationX =0;
    float rotationY =0;
    float valueX    =0;
    float valueY    =0;

    private void Start()
    {
        tf.transform.rotation = Quaternion.Euler(0, 0, 0);
        ui3DTest = transform.GetComponentInParent<UI3DTest>();
        angle = ui3DTest.angle;
        if (angle == 0)
            angle = 5f;
        
        SetRotation();
    }

    public void SetRotation()
    {
        switch (anchor)
        {
            case eANCHOR.TOPLEFT:
                rotationX = -angle;
                rotationY = -angle;      
                break;
            case eANCHOR.TOP:
                rotationX = -angle; 
                rotationY = 0; 
                break;
            case eANCHOR.TOPRIGHT:
                rotationX = -angle; 
                rotationY = angle;
                break;
            case eANCHOR.LEFT:
                rotationX = 0;
                rotationY = -angle; 
                break;
            case eANCHOR.CENTER:
                rotationX = 0f; 
                rotationY = 0f; 
                break;
            case eANCHOR.RIGHT:
                rotationX = 0f; 
                rotationY = angle; 
                break;
            case eANCHOR.BOTTOMLEFT:
                rotationX = angle; 
                rotationY = -angle;
                break;
            case eANCHOR.BOTTOM:
                rotationX = angle;
                rotationY = 0f; 
                break;                                           
            case eANCHOR.BOTTOMRIGHT:
                rotationX = angle;
                rotationY = angle;
                break;
        }
        tf.transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        tf.transform.localPosition = new Vector3(0,0,angle);
    }
}