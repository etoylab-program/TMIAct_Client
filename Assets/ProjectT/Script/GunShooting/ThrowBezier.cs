using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBezier : MonoBehaviour
{
    public GameObject kStartPos;
    public GameObject kHeightPos;
    public GameObject kEndPos;

    [Header("Debug Property")]
    public bool kbIsDebug = false;
    public Color kDebugColor = Color.cyan;
    private int SEGMENT_COUNT = 50;
    private LineRenderer _lineRenderer;
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!kbIsDebug)
            return;

        if (kStartPos == null || kHeightPos == null || kEndPos == null)
            return;

        //Gizmos.DrawCube(kStartPos.transform.position, Vector3.one);
        //Gizmos.DrawCube(kHeightPos.transform.position, Vector3.one);
        //Gizmos.DrawCube(kEndPos.transform.position, Vector3.one);

        Gizmos.color = kDebugColor;

        for (int i = 1; i <= SEGMENT_COUNT; i++)
        {
            float t = i / (float)SEGMENT_COUNT;

            Vector3 pos = CalculateCubicBezierPoint(t, kStartPos.transform.position, kHeightPos.transform.position, kEndPos.transform.position);

            Gizmos.DrawSphere(pos, 0.2f);
        }

        //DebugDrawLine();
    }

    private void DebugDrawLine()
    {
        if (!kbIsDebug)
        {
            if (_lineRenderer != null)
                _lineRenderer.enabled = false;
            return;
        }

        if (_lineRenderer == null)
            _lineRenderer = kStartPos.AddComponent<LineRenderer>();

        _lineRenderer.enabled = true;

        for (int i = 1; i <= SEGMENT_COUNT; i++)
        {
            float t = i / (float)SEGMENT_COUNT;

            Vector3 pos = CalculateCubicBezierPoint(t, kStartPos.transform.position, kHeightPos.transform.position, kEndPos.transform.position);
            _lineRenderer.SetVertexCount(i);
            _lineRenderer.SetPosition((i - 1), pos);
        }
        _lineRenderer.startWidth = 0.3f;
        _lineRenderer.endWidth = 0.3f;
        
    }
#endif
    Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uu * p0;
        p += 3 * u * t * p1;
        //p += 3 * u * tt * p2;
        p += tt * p2;

        return p;
    }

    public Vector3 GetPoint(float t)
    {
        return CalculateCubicBezierPoint(t, kStartPos.transform.position, kHeightPos.transform.position, kEndPos.transform.position);
    }
}
