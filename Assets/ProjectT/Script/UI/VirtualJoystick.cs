
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VirtualJoystick : MonoBehaviour
{
    public UISprite sprBg;
    public UISprite sprThumb;
    public int directionCount = 64;

    private static float MAX_THUMB_RADIUS = 0.25f;
    private static float MIN_MOVE_DISTANCE = 60.0f;

    private Vector3 m_dir = Vector3.zero;
    private Vector3 m_beforeDir = Vector3.zero;
    private Vector3 m_touchPos = Vector3.zero;
    private bool m_startCtrl = false;
    private Vector3 m_thumbPos = Vector3.zero;

    public Vector3 rawDir { get { return m_dir; } }
    public Vector3 dir { get { return Utility.Get3DDirFrom2DDir(m_dir); } }


    public void Init()
    {
        sprBg.transform.localPosition = Vector3.zero;
        sprThumb.transform.localPosition = Vector3.zero;

        m_dir = Vector3.zero;
        m_beforeDir = Vector3.zero;
        m_touchPos = Vector3.zero;
        m_startCtrl = false;
        m_thumbPos = Vector3.zero;
    }

    private void Update()
    {
        if(AppMgr.Instance.CustomInput.IsNoTouch() || UICamera.mainCamera == null)
        {
            Init();
            return;
        }

        m_touchPos = AppMgr.Instance.CustomInput.GetTouchPos();
        if (!Utility.IsInJoystickArea(m_touchPos))
        {
            return;
        }

        Vector3 uiTouchPos = UICamera.mainCamera.ScreenToWorldPoint(m_touchPos);

        if (AppMgr.Instance.CustomInput.IsTouchBegin)
        {
            sprBg.transform.localPosition = Vector3.zero;
            sprThumb.transform.localPosition = Vector3.zero;

            m_thumbPos = Vector3.zero;
            m_dir = Vector3.zero;

            m_startCtrl = true;
        }
        else if (AppMgr.Instance.CustomInput.IsTouchEnd)
        {
            Init();
        }

        if (!m_startCtrl)
        {
            return;
        }

        Vector3 relativeMousePos = uiTouchPos - transform.position;

        float rad = Mathf.Atan2(relativeMousePos.y, relativeMousePos.x);
        float x = Mathf.Cos(rad) * MAX_THUMB_RADIUS;
        float y = Mathf.Sin(rad) * MAX_THUMB_RADIUS;

        m_thumbPos.x = Mathf.Abs(relativeMousePos.x) < Mathf.Abs(x) ? relativeMousePos.x : x;
        m_thumbPos.y = Mathf.Abs(relativeMousePos.y) < Mathf.Abs(y) ? relativeMousePos.y : y;
        m_thumbPos.z = 0.0f;

        sprThumb.transform.position = transform.position + m_thumbPos;
        if (sprThumb.transform.position == Vector3.zero)
        {
            sprThumb.alpha = 0.4f;
        }
        else
        {
            sprThumb.alpha = 0.8f;
        }

        m_dir = (sprThumb.transform.localPosition - sprBg.transform.localPosition) / MAX_THUMB_RADIUS;

        if (Vector3.Magnitude(m_dir) <= MIN_MOVE_DISTANCE)
        {
            m_dir = Vector3.zero;
        }
        else if (m_dir != m_beforeDir)
        {
            Vector3 v1 = Vector3.Normalize(m_dir);
            Vector3 v2 = Vector3.Normalize(m_beforeDir);
            float dot = Vector3.Dot(v1, v2);

            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            if (angle >= (360.0f / (float)directionCount))
            {
                m_beforeDir = m_dir;
            }
        }
    }
}
