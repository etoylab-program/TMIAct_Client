
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LoopBlock : MonoBehaviour
{
    public GameObject[] blocks;
    public float blockInterval = 10.0f;
    public float reassemblyDistance = 15.0f;
    public float speed = 10.0f;

    private List<GameObject> m_listBlock = new List<GameObject>();
    private float checkDistance = 0.0f;


    private void Start()
    {
        checkDistance = 0.0f;

        m_listBlock.Clear();
        m_listBlock.AddRange(blocks);
    }

    private void Update()
    {
        checkDistance += speed * Time.deltaTime;
        if (checkDistance >= reassemblyDistance)
        {
            ReassemblyBlock();
            checkDistance = 0.0f;
        }
        else
        {
            for (int i = 0; i < blocks.Length; i++)
                blocks[i].transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }

    private void ReassemblyBlock()
    {
        Vector3 backupPos = m_listBlock[0].transform.position;

        Vector3 newPos = m_listBlock[m_listBlock.Count - 1].transform.position;
        newPos.y -= blockInterval;

        m_listBlock[0].transform.position = newPos;

        m_listBlock.Insert(m_listBlock.Count, m_listBlock[0]);
        m_listBlock.RemoveAt(0);
    }
}
