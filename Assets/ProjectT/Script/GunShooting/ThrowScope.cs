using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowScope : MonoBehaviour
{
    public eScopeDir kScopeDir = eScopeDir.None;
    private ThrowObj _target;
    //private List<ThrowObj> _target = new List<ThrowObj>();
    private Vector3 _defaultPos = Vector3.zero;
    private Vector3 _deltaPos = Vector3.zero;
    private float _minDistance = 1f;

    private Ray _ray;
    private RaycastHit[] _hits;
    private RaycastHit[] _prevHits;
    private Vector3 _boxSize = new Vector3(0.3f, 0.3f, 10f);
    private float m_MaxDistance = 300.0f;
    public bool bIsTrigger { get; private set; }

    private void Awake()
    {
        bIsTrigger = false;

        _defaultPos = this.transform.position;
        _deltaPos = this.transform.position;
    }

    
    
    public void SetBaxCast()
    {
        _boxSize = new Vector3(this.transform.localScale.x * 1.5f, this.transform.localScale.y * 1.5f, this.transform.localScale.z * 10);
        _hits = Physics.BoxCastAll(this.transform.position, _boxSize, this.transform.forward, this.transform.localRotation, 20f, LayerMask.GetMask("Enemy"));
        if (_hits == null || _hits.Length <= 0)
        {
            bIsTrigger = false;
            Log.Show(kScopeDir.ToString() + " / IsTrigger : false");
        }
        else
        {
            bIsTrigger = true;
            Log.Show(kScopeDir.ToString() + " / IsTrigger : true" + " / " + _hits[0].collider.name);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (!Application.isPlaying)
            return;
        //Check if there has been a hit yet
        if (_hits == null || _hits.Length == 0)
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.forward * m_MaxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + transform.forward * m_MaxDistance, transform.localScale);
            
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, transform.forward * _hits[0].distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + transform.forward * _hits[0].distance, transform.localScale);
        }
    }
#endif

    public bool ShotThrowTarget()
    {
        if (!bIsTrigger)
            return false;
        for (int i = 0; i < _hits.Length; i++)
        {
            if (!_hits[i].collider.gameObject.activeSelf)
                continue;
            ThrowObj targetThrow = _hits[i].collider.GetComponent<ThrowObj>();
            if (targetThrow != null)
            {
                targetThrow.FireThrowObj();
            }
            //_hits[i].collider.SendMessage("FireThrowObj");
        }

        bIsTrigger = false;
        return true;


        //if (_target == null)
        //    return;

        //if (!bIsTrigger)
        //    return;
        //bIsTrigger = false;
        //_target.FireThrowObj();
        //_target = null;
    }

    private void ResetPosition()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, _defaultPos, Time.deltaTime * 5f);
    }

    private void SetLocalScale()
    {
        float temp = 0f;

        float tempPos = 1f + (this.transform.position.z / 10f);

        Vector3 localScale = new Vector3(tempPos, tempPos, 1f);

        this.transform.localScale = localScale;
    }
}
