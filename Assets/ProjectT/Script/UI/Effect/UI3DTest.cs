using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eANCHOR
{
    TOPLEFT,
    TOP,
    TOPRIGHT,
    LEFT,
    CENTER,
    RIGHT,
    BOTTOMLEFT,
    BOTTOM,
    BOTTOMRIGHT,
}

public class UI3DTest : MonoBehaviour
{
    public List<UI3DTestObect> listTransform;
    public GameObject screenEffect;
    public GameObject effectCamera;
    public float angle;

    public void DamageTest()
    {
        screenEffect.SetActive(true);
        effectCamera.SetActive(true);
    }
}