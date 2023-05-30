
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotateAxis : MonoBehaviour
{
    public enum eType
    {
        None = 0,

        X,
        Y,
        Z
    }


    [Header("[Axis]")]
    public Transform X;
    public Transform Y;
    public Transform Z;

    public eType        CurrentType     { get; private set; }
    public Transform    CurrentObject   { get; private set; }

    private Material mMatX;
    private Material mMatY;
    private Material mMatZ;


    public void Show(bool show)
    {
        Init();

        CurrentType = eType.None;
        CurrentObject = null;

        gameObject.SetActive(show);
    }

    public void SelectAxis(Transform selectedTransform)
    {
        Init();

        CurrentType = eType.None;
        CurrentObject = null;

        if (selectedTransform == X)
        {
            CurrentType = eType.X;
            CurrentObject = X;

            mMatX.SetFloat("_SrcBlend", 1.0f);
            mMatX.SetFloat("_DstBlend", 1.0f);
        }
        else if (selectedTransform == Y)
        {
            CurrentType = eType.Y;
            CurrentObject = Y;

            mMatY.SetFloat("_SrcBlend", 1.0f);
            mMatY.SetFloat("_DstBlend", 1.0f);
        }
        else if (selectedTransform == Z)
        {
            CurrentType = eType.Z;
            CurrentObject = Z;

            mMatZ.SetFloat("_SrcBlend", 1.0f);
            mMatZ.SetFloat("_DstBlend", 1.0f);
        }

        CurrentObject.localScale = new Vector3(1.0f, 1.0f, 2.0f);
    }

    private void Awake()
    {
        Renderer renderer = X.GetComponent<Renderer>();
        mMatX = renderer.material;

        renderer = Y.GetComponent<Renderer>();
        mMatY = renderer.material;

        renderer = Z.GetComponent<Renderer>();
        mMatZ = renderer.material;
    }

    private void Init()
    {
        X.transform.localScale = Vector3.one;
        Y.transform.localScale = Vector3.one;
        Z.transform.localScale = Vector3.one;

        mMatX.SetFloat("_SrcBlend", 3.0f);
        mMatY.SetFloat("_SrcBlend", 3.0f);
        mMatZ.SetFloat("_SrcBlend", 3.0f);

        mMatX.SetFloat("_DstBlend", 10.0f);
        mMatY.SetFloat("_DstBlend", 10.0f);
        mMatZ.SetFloat("_DstBlend", 10.0f);
    }
}
