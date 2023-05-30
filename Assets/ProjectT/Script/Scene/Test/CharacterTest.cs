
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterTest : MonoBehaviour
{
    public enum eTouchControl
    {
        None = 0,
        Camera,
    }


    private eTouchControl m_eTouchCtrl = eTouchControl.None;
    private Touch m_touch1, m_touch2;
    private Vector3 m_beforeTouchPos = Vector3.zero;


    private void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        float delta = 0.0f;
        bool zoomInOut = false;
        Vector3 touchPos = Vector3.zero;

#if UNITY_EDITOR
        delta = Input.GetAxis("Mouse ScrollWheel") * -10.0f;
        touchPos = Input.mousePosition;

#else
            if (Input.touchCount >= 2)
            {
                zoomInOut = true;

                m_touch1 = Input.GetTouch(0);
                m_touch2 = Input.GetTouch(1);

                Vector2 touch1PrevPos = m_touch1.position - m_touch1.deltaPosition;
                Vector2 touch2PrevPos = m_touch2.position - m_touch2.deltaPosition;

                float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
                float touchDeltaMag = (m_touch1.position - m_touch2.position).magnitude;

                delta = (prevTouchDeltaMag - touchDeltaMag) * (Time.fixedDeltaTime * 4.0f);

                if (m_touch1.phase == TouchPhase.Ended)
                    touchPos = m_touch2.position;
                else
                    touchPos = m_touch1.position;
            }
            else if (Input.touchCount == 1)
            {
                zoomInOut = false;
                touchPos = Input.GetTouch(0).position;
            }
#endif
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + delta, 20.0f, 100.0f);

        if (Input.GetMouseButtonDown(0) == true)
        {
            m_beforeTouchPos = touchPos;
            m_eTouchCtrl = eTouchControl.Camera;
        }
        else if (Input.GetMouseButtonUp(0) == true)
            m_eTouchCtrl = eTouchControl.None;

        if (m_eTouchCtrl != eTouchControl.None && zoomInOut == false)
        {
            Vector3 cameraPos = Camera.main.transform.position;

            cameraPos.x = Mathf.Clamp(cameraPos.x - (m_beforeTouchPos.x - touchPos.x) * (Time.fixedDeltaTime * 0.1f), -3.0f, 2.0f);
            cameraPos.y = Mathf.Clamp(cameraPos.y + (m_beforeTouchPos.y - touchPos.y) * (Time.fixedDeltaTime * 0.1f), 0.2f, 2.0f);

            Camera.main.transform.position = cameraPos;
        }

        m_beforeTouchPos = touchPos;
    }
}
