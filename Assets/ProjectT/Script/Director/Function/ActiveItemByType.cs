
using UnityEngine;


public class ActiveItemByType : MonoBehaviour
{
    [Header("[Property]")]
    public eContentsPosKind ItemType        = eContentsPosKind._NONE_;
    public GameObject[]     ActiveObjects;

    private ItemWakeUpDirector mDirectorItemWakeUp = null;


    private void Awake()
    {
        mDirectorItemWakeUp = GetComponentInParent<ItemWakeUpDirector>();

        for(int i = 0; i < ActiveObjects.Length; i++)
        {
            ActiveObjects[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
        if(mDirectorItemWakeUp == null || ItemType != mDirectorItemWakeUp.ItemType)
        {
            return;
        }

        for (int i = 0; i < ActiveObjects.Length; i++)
        {
            ActiveObjects[i].SetActive(true);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < ActiveObjects.Length; i++)
        {
            ActiveObjects[i].SetActive(false);
        }
    }
}
