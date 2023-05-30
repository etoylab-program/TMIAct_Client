using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIObjectDestroy : MonoBehaviour
{
    public float duration;

    private void OnEnable()
    {
        StartCoroutine("DestroyObject");
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
