
using UnityEngine;


public abstract class CmptBase : MonoBehaviour
{
    protected Unit m_owner = null;


    protected virtual void Awake()
    {
        ResetOwner();
    }

    public void ResetOwner()
    {
        m_owner = GetComponent<Unit>();
    }
}
