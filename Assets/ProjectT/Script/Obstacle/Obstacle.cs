
using UnityEngine;


public class Obstacle : MonoBehaviour
{
    new Transform transform;

    private Vector3 m_deltaPos = Vector3.zero;
    private Vector3 m_beforePos = Vector3.zero;

    public Vector3 deltaPos { get { return m_deltaPos; } }


    private void Awake()
    {
        transform = GetComponent<Transform>();    
    }

    private void Start()
    {
        Vector3 originalPos = transform.position;
        m_beforePos = originalPos;
    }

    private void FixedUpdate()
    {
        m_deltaPos = transform.position - m_beforePos;
        m_deltaPos.y = 0.0f;

        m_beforePos = transform.position;
    }
}
